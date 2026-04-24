using Andloe.Data;
using Andloe.Entidad;
using Andloe.Logica.DGII;
using Data;
using Logica;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Andloe.Logica
{
    public sealed class PosFiscalService
    {
        private readonly ClienteRepository _clienteRepo = new();
        private readonly ProductoRepository _prodRepo = new();

        private const string TIPO_DOCUMENTO_FACTURA = "FAC";
        private const string TIPO_PAGO_CONTADO = "CONTADO";
        private const string ESTADO_FACTURA = "FINALIZADA";
        private const string ESTADO_ECF = "BORRADOR";
        private const string TIPO_ORIGEN_POS = "POS";
        private const string UNIDAD_DEFAULT = "UND";

        public PosFiscalResult GenerarFacturaFiscalDesdeVenta(
            long ventaId,
            int tipoECFId,
            int tipoPagoECFId,
            string formaPagoFiscal,
            string usuario)
        {
            if (ventaId <= 0)
                throw new InvalidOperationException("VentaId inválido.");

            if (tipoECFId <= 0)
                throw new InvalidOperationException("TipoECFId inválido.");

            if (tipoPagoECFId <= 0)
                throw new InvalidOperationException("TipoPagoECFId inválido.");

            if (string.IsNullOrWhiteSpace(formaPagoFiscal))
                throw new InvalidOperationException("FormaPagoFiscal inválida.");

            if (string.IsNullOrWhiteSpace(usuario))
                throw new InvalidOperationException("Usuario inválido.");

            int facturaId;
            string numeroDocumento;

            using (var cn = Db.GetOpenConnection())
            using (var tx = cn.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    var venta = ObtenerVentaCab(cn, tx, ventaId);
                    if (venta == null)
                        throw new InvalidOperationException($"No se encontró la venta {ventaId}.");

                    var existente = BuscarFacturaExistentePorOrigen(cn, tx, venta.NoDocumento);
                    if (existente != null)
                    {
                        tx.Commit();
                        return existente;
                    }

                    var lineas = ObtenerVentaLineas(cn, tx, ventaId);
                    if (lineas.Count == 0)
                        throw new InvalidOperationException("La venta no tiene líneas para facturar.");

                    var tipoEcf = ObtenerTipoEcf(cn, tx, tipoECFId);
                    if (tipoEcf == null)
                        throw new InvalidOperationException($"No existe ECFTipoDocumento con id {tipoECFId}.");

                    var tipoPago = ObtenerTipoPagoEcf(cn, tx, tipoPagoECFId);
                    if (tipoPago == null)
                        throw new InvalidOperationException($"No existe ECFTipoPago con id {tipoPagoECFId}.");

                    var contexto = ResolverContextoFactura(cn, tx, venta);
                    var cliente = ObtenerClienteSnapshot(venta.ClienteCodigo);

                    if (tipoEcf.AplicaCompradorRnc && string.IsNullOrWhiteSpace(cliente.RncCedula))
                        throw new InvalidOperationException(
                            "El tipo de comprobante seleccionado requiere documento del comprador y la venta no tiene RNC/Cédula válido.");

                    numeroDocumento = ResolverNumeroDocumento(cn, tx, venta);

                    facturaId = InsertarFacturaCab(
                        cn,
                        tx,
                        venta,
                        cliente,
                        contexto,
                        numeroDocumento,
                        tipoECFId,
                        tipoPagoECFId,
                        formaPagoFiscal,
                        usuario);

                    InsertarFacturaDet(cn, tx, facturaId, lineas);
                    InsertarFacturaPago(cn, tx, facturaId, venta, usuario);
                    InsertarFacturaFormaPago(cn, tx, facturaId, ventaId);
                    CrearEcfDocumentoBase(
                        cn,
                        tx,
                        facturaId,
                        tipoECFId,
                        venta.Total);

                    MarcarVentaComoFacturadaFiscalmente(cn, tx, ventaId, facturaId);

                    tx.Commit();
                }
                catch
                {
                    tx.Rollback();
                    throw;
                }
            }

            return ProcesarDocumentoFiscalGenerado(facturaId, numeroDocumento, usuario);
        }

        private VentaCabLite? ObtenerVentaCab(SqlConnection cn, SqlTransaction tx, long ventaId)
        {
            using var cmd = new SqlCommand(@"
SELECT TOP (1)
    VentaId,
    NoDocumento,
    Fecha,
    ClienteCodigo,
    MonedaCodigo,
    TasaCambio,
    Subtotal,
    DescuentoTotal,
    ImpuestoTotal,
    Total,
    Estado,
    Usuario,
    Observacion,
    FechaCreacion,
    MontoPago,
    MontoCambio,
    ClienteId,
    SubTotalMoneda,
    ItbisMoneda,
    TotalMoneda,
    NombreCliente,
    EmailCliente,
    TelefonoCliente,
    POS_CajaId,
    POS_CajaNumero,
    CajaId,
    TerminoPagoId
FROM dbo.VentaCab
WHERE VentaId = @VentaId;", cn, tx);

            cmd.Parameters.Add("@VentaId", SqlDbType.BigInt).Value = ventaId;

            using var rd = cmd.ExecuteReader();
            if (!rd.Read())
                return null;

            return new VentaCabLite
            {
                VentaId = rd.GetInt64(0),
                NoDocumento = rd.GetString(1),
                Fecha = rd.GetDateTime(2),
                ClienteCodigo = rd.IsDBNull(3) ? null : rd.GetString(3),
                MonedaCodigo = rd.GetString(4),
                TasaCambio = rd.IsDBNull(5) ? 1m : rd.GetDecimal(5),
                Subtotal = rd.GetDecimal(6),
                DescuentoTotal = rd.GetDecimal(7),
                ImpuestoTotal = rd.GetDecimal(8),
                Total = rd.GetDecimal(9),
                Estado = rd.GetString(10),
                Usuario = rd.IsDBNull(11) ? null : rd.GetString(11),
                Observacion = rd.IsDBNull(12) ? null : rd.GetString(12),
                FechaCreacion = rd.GetDateTime(13),
                MontoPago = rd.IsDBNull(14) ? 0m : rd.GetDecimal(14),
                MontoCambio = rd.IsDBNull(15) ? 0m : rd.GetDecimal(15),
                ClienteId = rd.IsDBNull(16) ? (int?)null : rd.GetInt32(16),
                SubTotalMoneda = rd.IsDBNull(17) ? rd.GetDecimal(6) : rd.GetDecimal(17),
                ItbisMoneda = rd.IsDBNull(18) ? rd.GetDecimal(8) : rd.GetDecimal(18),
                TotalMoneda = rd.IsDBNull(19) ? rd.GetDecimal(9) : rd.GetDecimal(19),
                NombreCliente = rd.IsDBNull(20) ? null : rd.GetString(20),
                EmailCliente = rd.IsDBNull(21) ? null : rd.GetString(21),
                TelefonoCliente = rd.IsDBNull(22) ? null : rd.GetString(22),
                PosCajaId = rd.IsDBNull(23) ? (int?)null : rd.GetInt32(23),
                PosCajaNumero = rd.IsDBNull(24) ? null : rd.GetString(24),
                CajaId = rd.IsDBNull(25) ? (int?)null : rd.GetInt32(25),
                TerminoPagoId = rd.GetInt32(26)
            };
        }

        private List<VentaLinLite> ObtenerVentaLineas(SqlConnection cn, SqlTransaction tx, long ventaId)
        {
            var lista = new List<VentaLinLite>();

            using var cmd = new SqlCommand(@"
SELECT
    VentaId,
    Linea,
    NoProducto,
    Descripcion,
    Cantidad,
    PrecioUnitario,
    DescuentoLinea,
    ItbisPorc,
    ItbisMonto,
    Importe,
    ProductoCodigo,
    PrecioUnit,
    DescuentoMoneda,
    ImporteMoneda,
    ItbisMoneda,
    TotalMoneda,
    DescuentoPct,
    DescuentoMonto
FROM dbo.VentaLin
WHERE VentaId = @VentaId
ORDER BY Linea;", cn, tx);

            cmd.Parameters.Add("@VentaId", SqlDbType.BigInt).Value = ventaId;

            using var rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                lista.Add(new VentaLinLite
                {
                    VentaId = rd.GetInt64(0),
                    Linea = rd.GetInt32(1),
                    NoProducto = rd.GetString(2),
                    Descripcion = rd.GetString(3),
                    Cantidad = rd.GetDecimal(4),
                    PrecioUnitario = rd.GetDecimal(5),
                    DescuentoLinea = rd.GetDecimal(6),
                    ItbisPorc = rd.GetDecimal(7),
                    ItbisMonto = rd.GetDecimal(8),
                    Importe = rd.GetDecimal(9),
                    ProductoCodigo = rd.IsDBNull(10) ? null : rd.GetString(10),
                    PrecioUnit = rd.IsDBNull(11) ? rd.GetDecimal(5) : rd.GetDecimal(11),
                    DescuentoMoneda = rd.IsDBNull(12) ? rd.GetDecimal(6) : rd.GetDecimal(12),
                    ImporteMoneda = rd.IsDBNull(13) ? rd.GetDecimal(9) : rd.GetDecimal(13),
                    ItbisMoneda = rd.IsDBNull(14) ? rd.GetDecimal(8) : rd.GetDecimal(14),
                    TotalMoneda = rd.IsDBNull(15)
                        ? Math.Round(rd.GetDecimal(9) + rd.GetDecimal(8), 2)
                        : rd.GetDecimal(15),
                    DescuentoPct = rd.GetDecimal(16),
                    DescuentoMonto = rd.GetDecimal(17)
                });
            }

            return lista;
        }

        private TipoEcfLite? ObtenerTipoEcf(SqlConnection cn, SqlTransaction tx, int tipoECFId)
        {
            using var cmd = new SqlCommand(@"
SELECT TOP (1)
    TipoECFId,
    CodigoTipoECF,
    Prefijo,
    Nombre,
    AplicaCompradorRnc
FROM dbo.ECFTipoDocumento
WHERE TipoECFId = @TipoECFId;", cn, tx);

            cmd.Parameters.Add("@TipoECFId", SqlDbType.Int).Value = tipoECFId;

            using var rd = cmd.ExecuteReader();
            if (!rd.Read())
                return null;

            return new TipoEcfLite
            {
                TipoECFId = rd.GetInt32(0),
                CodigoTipoECF = rd.GetString(1),
                Prefijo = rd.GetString(2),
                Nombre = rd.GetString(3),
                AplicaCompradorRnc = rd.GetBoolean(4)
            };
        }

        private TipoPagoEcfLite? ObtenerTipoPagoEcf(SqlConnection cn, SqlTransaction tx, int tipoPagoECFId)
        {
            using var cmd = new SqlCommand(@"
SELECT TOP (1)
    TipoPagoECFId,
    CodigoDGII,
    Descripcion
FROM dbo.ECFTipoPago
WHERE TipoPagoECFId = @TipoPagoECFId;", cn, tx);

            cmd.Parameters.Add("@TipoPagoECFId", SqlDbType.Int).Value = tipoPagoECFId;

            using var rd = cmd.ExecuteReader();
            if (!rd.Read())
                return null;

            return new TipoPagoEcfLite
            {
                TipoPagoECFId = rd.GetInt32(0),
                CodigoDGII = rd.GetString(1),
                Descripcion = rd.GetString(2)
            };
        }

        private PosFiscalResult? BuscarFacturaExistentePorOrigen(SqlConnection cn, SqlTransaction tx, string numeroOrigen)
        {
            using var cmd = new SqlCommand(@"
SELECT TOP (1)
    FacturaId,
    NumeroDocumento,
    eNCF,
    TrackId,
    EstadoECF
FROM dbo.FacturaCab
WHERE TipoOrigen = @TipoOrigen
  AND NumeroOrigen = @NumeroOrigen
ORDER BY FacturaId DESC;", cn, tx);

            cmd.Parameters.Add("@TipoOrigen", SqlDbType.VarChar, 3).Value = TIPO_ORIGEN_POS;
            cmd.Parameters.Add("@NumeroOrigen", SqlDbType.VarChar, 30).Value = numeroOrigen;

            using var rd = cmd.ExecuteReader();
            if (!rd.Read())
                return null;

            return new PosFiscalResult
            {
                FacturaId = rd.GetInt32(0),
                NumeroDocumento = rd.GetString(1),
                ENCF = rd.IsDBNull(2) ? null : rd.GetString(2),
                TrackId = rd.IsDBNull(3) ? null : rd.GetString(3),
                EstadoECF = rd.IsDBNull(4) ? null : rd.GetString(4)
            };
        }

        private FacturaContextoLite ResolverContextoFactura(SqlConnection cn, SqlTransaction tx, VentaCabLite venta)
        {
            var almacenId = ConfigService.AlmacenPosOrigenId;
            if (almacenId <= 0)
                throw new InvalidOperationException("No está configurado el almacén POS de origen.");

            var cajaId = venta.CajaId ?? venta.PosCajaId;
            if (!cajaId.HasValue || cajaId.Value <= 0)
                throw new InvalidOperationException("La venta no tiene CajaId asociado.");

            using var cmd = new SqlCommand(@"
SELECT TOP (1)
    c.CajaId,
    c.SucursalId,
    s.EmpresaId
FROM dbo.Caja c
LEFT JOIN dbo.Sucursal s ON s.SucursalId = c.SucursalId
WHERE c.CajaId = @CajaId;", cn, tx);

            cmd.Parameters.Add("@CajaId", SqlDbType.Int).Value = cajaId.Value;

            using var rd = cmd.ExecuteReader();
            if (!rd.Read())
                throw new InvalidOperationException($"No se pudo resolver el contexto de caja para CajaId={cajaId.Value}.");

            var sucursalId = rd.IsDBNull(1) ? (int?)null : rd.GetInt32(1);
            var empresaId = rd.IsDBNull(2) ? (int?)null : rd.GetInt32(2);

            if (!empresaId.HasValue || empresaId.Value <= 0)
                throw new InvalidOperationException("No se pudo resolver EmpresaId desde la caja/sucursal.");

            return new FacturaContextoLite
            {
                CajaId = cajaId.Value,
                SucursalId = sucursalId,
                EmpresaId = empresaId.Value,
                AlmacenId = almacenId
            };
        }

        private ClienteSnapshotLite ObtenerClienteSnapshot(string? clienteCodigo)
        {
            if (string.IsNullOrWhiteSpace(clienteCodigo))
                throw new InvalidOperationException(
                    "La venta no tiene ClienteCodigo. No se puede generar factura fiscal sin cliente configurado.");

            var dto = _clienteRepo.BuscarPorCodigoORnc(clienteCodigo);
            if (dto == null)
                throw new InvalidOperationException(
                    $"No se encontró el cliente '{clienteCodigo}' para generar la factura fiscal.");

            if (string.IsNullOrWhiteSpace(dto.Nombre))
                throw new InvalidOperationException(
                    $"El cliente '{clienteCodigo}' no tiene nombre válido para facturación fiscal.");

            return new ClienteSnapshotLite
            {
                Codigo = dto.Codigo,
                Nombre = dto.Nombre,
                RncCedula = dto.RncCedula,
                Direccion = dto.Direccion,
                Correo = null
            };
        }

        private string ResolverNumeroDocumento(SqlConnection cn, SqlTransaction tx, VentaCabLite venta)
        {
            var candidato = (venta.NoDocumento ?? string.Empty).Trim();

            if (!string.IsNullOrWhiteSpace(candidato) &&
                candidato.Length <= 20 &&
                !ExisteNumeroDocumento(cn, tx, candidato))
            {
                return candidato;
            }

            var generado = GenerarNumeroDocumentoTemporal(venta.VentaId, venta.Fecha);

            if (!ExisteNumeroDocumento(cn, tx, generado))
                return generado;

            for (var i = 1; i <= 999; i++)
            {
                var alt = $"F{venta.Fecha:yyMMddHHmm}{i:0000000}";
                if (alt.Length > 20)
                    alt = alt.Substring(0, 20);

                if (!ExisteNumeroDocumento(cn, tx, alt))
                    return alt;
            }

            throw new InvalidOperationException("No se pudo generar un NumeroDocumento único para la factura.");
        }

        private bool ExisteNumeroDocumento(SqlConnection cn, SqlTransaction tx, string numeroDocumento)
        {
            using var cmd = new SqlCommand(@"
SELECT COUNT(1)
FROM dbo.FacturaCab
WHERE NumeroDocumento = @NumeroDocumento;", cn, tx);

            cmd.Parameters.Add("@NumeroDocumento", SqlDbType.VarChar, 20).Value = numeroDocumento;

            var n = Convert.ToInt32(cmd.ExecuteScalar() ?? 0);
            return n > 0;
        }

        private string GenerarNumeroDocumentoTemporal(long ventaId, DateTime fecha)
        {
            var numero = $"F{fecha:yyMMddHHmmss}{(ventaId % 10000000):0000000}";
            if (numero.Length > 20)
                numero = numero.Substring(0, 20);

            return numero;
        }

        private int InsertarFacturaCab(
    SqlConnection cn,
    SqlTransaction tx,
    VentaCabLite venta,
    ClienteSnapshotLite cliente,
    FacturaContextoLite contexto,
    string numeroDocumento,
    int tipoECFId,
    int tipoPagoECFId,
    string formaPagoFiscal,
    string usuario)
        {
            var nombreClienteFiscal = Coalesce(cliente.Nombre, venta.NombreCliente);
            if (string.IsNullOrWhiteSpace(nombreClienteFiscal))
                throw new InvalidOperationException(
                    "La venta no tiene nombre de cliente fiscal válido. Verifique la configuración del cliente en POS.");

            var tipoIngresoId = 1;

            var prefijo = ResolverPrefijoEcfDesdeTipoId(tipoECFId);
            var tipoId = int.Parse(prefijo.Substring(1, 2));

            var facRepo = new FacturaRepository();
            var fechaVencimientoSecuencia = facRepo.ObtenerFechaVencimientoSecuencia(
                empresaId: contexto.EmpresaId,
                sucursalId: contexto.SucursalId ?? 1,
                cajaId: contexto.CajaId,
                tipoId: tipoId,
                prefijo: prefijo);

            if (!fechaVencimientoSecuencia.HasValue)
                throw new InvalidOperationException("No existe rango NCF válido para este tipo de comprobante.");

            using var cmd = new SqlCommand(@"
INSERT INTO dbo.FacturaCab
(
    TipoDocumento,
    NumeroDocumento,
    FechaDocumento,
    FechaVencimiento,
    ClienteId,
    NombreCliente,
    DocumentoCliente,
    SubTotal,
    TotalDescuento,
    TotalImpuesto,
    TotalGeneral,
    TipoPago,
    TerminoPagoId,
    DiasCredito,
    Estado,
    Observacion,
    UsuarioCreacion,
    FechaCreacion,
    UsuarioFinaliza,
    FechaFinaliza,
    ClienteCodigo,
    AlmacenId,
    EmpresaId,
    SucursalId,
    NumeroOrigen,
    TipoOrigen,
    ConvertidaAFactura,
    DireccionCliente,
    TipoECFId,
    TipoPagoECFId,
    EsElectronica,
    RncCompradorSnapshot,
    RazonSocialCompradorSnapshot,
    CorreoCompradorSnapshot,
    DireccionCompradorSnapshot,
    MontoGravadoTotal,
    MontoExentoTotal,
    EstadoECF,
    TipoPagoECFHeader,
    TipoIngresoId,
    FechaVencimientoSecuencia
)
OUTPUT INSERTED.FacturaId
VALUES
(
    @TipoDocumento,
    @NumeroDocumento,
    @FechaDocumento,
    @FechaVencimiento,
    @ClienteId,
    @NombreCliente,
    @DocumentoCliente,
    @SubTotal,
    @TotalDescuento,
    @TotalImpuesto,
    @TotalGeneral,
    @TipoPago,
    @TerminoPagoId,
    @DiasCredito,
    @Estado,
    @Observacion,
    @UsuarioCreacion,
    @FechaCreacion,
    @UsuarioFinaliza,
    @FechaFinaliza,
    @ClienteCodigo,
    @AlmacenId,
    @EmpresaId,
    @SucursalId,
    @NumeroOrigen,
    @TipoOrigen,
    @ConvertidaAFactura,
    @DireccionCliente,
    @TipoECFId,
    @TipoPagoECFId,
    @EsElectronica,
    @RncCompradorSnapshot,
    @RazonSocialCompradorSnapshot,
    @CorreoCompradorSnapshot,
    @DireccionCompradorSnapshot,
    @MontoGravadoTotal,
    @MontoExentoTotal,
    @EstadoECF,
    @TipoPagoECFHeader,
    @TipoIngresoId,
    @FechaVencimientoSecuencia
);", cn, tx);

            cmd.Parameters.Add("@TipoDocumento", SqlDbType.VarChar, 10).Value = TIPO_DOCUMENTO_FACTURA;
            cmd.Parameters.Add("@NumeroDocumento", SqlDbType.VarChar, 20).Value = numeroDocumento;
            cmd.Parameters.Add("@FechaDocumento", SqlDbType.DateTime).Value = venta.Fecha;
            cmd.Parameters.Add("@FechaVencimiento", SqlDbType.DateTime).Value = DBNull.Value;
            cmd.Parameters.Add("@ClienteId", SqlDbType.Int).Value = (object?)venta.ClienteId ?? DBNull.Value;
            cmd.Parameters.Add("@NombreCliente", SqlDbType.NVarChar, 300).Value = nombreClienteFiscal;
            cmd.Parameters.Add("@DocumentoCliente", SqlDbType.NVarChar, 60).Value =
                (object?)cliente.RncCedula ?? DBNull.Value;

            AddDecimal(cmd, "@SubTotal", venta.Subtotal);
            AddDecimal(cmd, "@TotalDescuento", venta.DescuentoTotal);
            AddDecimal(cmd, "@TotalImpuesto", venta.ImpuestoTotal);
            AddDecimal(cmd, "@TotalGeneral", venta.Total);

            cmd.Parameters.Add("@TipoPago", SqlDbType.VarChar, 10).Value = TIPO_PAGO_CONTADO;
            cmd.Parameters.Add("@TerminoPagoId", SqlDbType.Int).Value = venta.TerminoPagoId;
            cmd.Parameters.Add("@DiasCredito", SqlDbType.Int).Value = DBNull.Value;
            cmd.Parameters.Add("@Estado", SqlDbType.VarChar, 15).Value = ESTADO_FACTURA;
            cmd.Parameters.Add("@Observacion", SqlDbType.NVarChar, 600).Value =
                $"Generada desde POS. VentaId={venta.VentaId}";

            cmd.Parameters.Add("@UsuarioCreacion", SqlDbType.VarChar, 30).Value = usuario;
            cmd.Parameters.Add("@FechaCreacion", SqlDbType.DateTime).Value = DateTime.Now;
            cmd.Parameters.Add("@UsuarioFinaliza", SqlDbType.VarChar, 30).Value = usuario;
            cmd.Parameters.Add("@FechaFinaliza", SqlDbType.DateTime).Value = DateTime.Now;
            cmd.Parameters.Add("@ClienteCodigo", SqlDbType.VarChar, 30).Value =
                (object?)venta.ClienteCodigo ?? DBNull.Value;
            cmd.Parameters.Add("@AlmacenId", SqlDbType.Int).Value = contexto.AlmacenId;
            cmd.Parameters.Add("@EmpresaId", SqlDbType.Int).Value = contexto.EmpresaId;
            cmd.Parameters.Add("@SucursalId", SqlDbType.Int).Value =
                (object?)contexto.SucursalId ?? DBNull.Value;
            cmd.Parameters.Add("@NumeroOrigen", SqlDbType.VarChar, 30).Value = venta.NoDocumento;
            cmd.Parameters.Add("@TipoOrigen", SqlDbType.VarChar, 3).Value = TIPO_ORIGEN_POS;
            cmd.Parameters.Add("@ConvertidaAFactura", SqlDbType.Bit).Value = true;
            cmd.Parameters.Add("@DireccionCliente", SqlDbType.NVarChar, 800).Value =
                (object?)cliente.Direccion ?? DBNull.Value;
            cmd.Parameters.Add("@TipoECFId", SqlDbType.Int).Value = tipoECFId;
            cmd.Parameters.Add("@TipoPagoECFId", SqlDbType.Int).Value = tipoPagoECFId;
            cmd.Parameters.Add("@EsElectronica", SqlDbType.Bit).Value = true;
            cmd.Parameters.Add("@RncCompradorSnapshot", SqlDbType.VarChar, 20).Value =
                (object?)cliente.RncCedula ?? DBNull.Value;
            cmd.Parameters.Add("@RazonSocialCompradorSnapshot", SqlDbType.NVarChar, 400).Value =
                nombreClienteFiscal;
            cmd.Parameters.Add("@CorreoCompradorSnapshot", SqlDbType.NVarChar, 400).Value =
                (object?)Coalesce(cliente.Correo, venta.EmailCliente) ?? DBNull.Value;
            cmd.Parameters.Add("@DireccionCompradorSnapshot", SqlDbType.NVarChar, 800).Value =
                (object?)cliente.Direccion ?? DBNull.Value;

            AddDecimal(cmd, "@MontoGravadoTotal", venta.Subtotal);
            AddDecimal(cmd, "@MontoExentoTotal", 0m);

            cmd.Parameters.Add("@EstadoECF", SqlDbType.VarChar, 20).Value = ESTADO_ECF;
            cmd.Parameters.Add("@TipoPagoECFHeader", SqlDbType.VarChar, 1).Value = formaPagoFiscal;

            cmd.Parameters.Add("@TipoIngresoId", SqlDbType.Int).Value = tipoIngresoId;
            cmd.Parameters.Add("@FechaVencimientoSecuencia", SqlDbType.Date).Value = fechaVencimientoSecuencia;

            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        private void InsertarFacturaDet(
     SqlConnection cn,
     SqlTransaction tx,
     int facturaId,
     IEnumerable<VentaLinLite> lineas)
        {
            foreach (var l in lineas.OrderBy(x => x.Linea))
            {
                using var cmd = new SqlCommand(@"
INSERT INTO dbo.FacturaDet
(
    FacturaId,
    ProductoCodigo,
    Descripcion,
    DescripcionFiscal,
    Unidad,
    UnidadMedidaDGII,
    Cantidad,
    PrecioUnitario,
    DescuentoPct,
    DescuentoMonto,
    ImpuestoPct,
    ImpuestoMonto,
    TotalLinea,
    Precio,
    ItbisPct,
    ItbisMonto,
    CodBarra,
    NumeroLineaECF,
    MontoGravado,
    MontoExento,
    EsExento,
    PrecioUnitarioBase,
    SubtotalLineaAntesImpuesto,
    IndicadorITBISIncluido,
    CodigoProducto,
    SubtotalLinea,
    BaseImponible
)
VALUES
(
    @FacturaId,
    @ProductoCodigo,
    @Descripcion,
    @DescripcionFiscal,
    @Unidad,
    @UnidadMedidaDGII,
    @Cantidad,
    @PrecioUnitario,
    @DescuentoPct,
    @DescuentoMonto,
    @ImpuestoPct,
    @ImpuestoMonto,
    @TotalLinea,
    @Precio,
    @ItbisPct,
    @ItbisMonto,
    @CodBarra,
    @NumeroLineaECF,
    @MontoGravado,
    @MontoExento,
    @EsExento,
    @PrecioUnitarioBase,
    @SubtotalLineaAntesImpuesto,
    @IndicadorITBISIncluido,
    @CodigoProducto,
    @SubtotalLinea,
    @BaseImponible
);", cn, tx);

                cmd.Parameters.Add("@FacturaId", SqlDbType.Int).Value = facturaId;
                var productoCodigo = Coalesce(l.ProductoCodigo, l.NoProducto) ?? "";
                var unidad = _prodRepo.ObtenerUnidadPorCodigo(productoCodigo, UNIDAD_DEFAULT) ?? UNIDAD_DEFAULT;

                cmd.Parameters.Add("@ProductoCodigo", SqlDbType.VarChar, 20).Value =
                    string.IsNullOrWhiteSpace(productoCodigo) ? DBNull.Value : productoCodigo;

                cmd.Parameters.Add("@Descripcion", SqlDbType.NVarChar, 300).Value = l.Descripcion;
                cmd.Parameters.Add("@DescripcionFiscal", SqlDbType.NVarChar, 500).Value =
                    string.IsNullOrWhiteSpace(l.Descripcion) ? "ITEM POS" : l.Descripcion;

                cmd.Parameters.Add("@Unidad", SqlDbType.VarChar, 10).Value = unidad;
                cmd.Parameters.Add("@UnidadMedidaDGII", SqlDbType.VarChar, 5).Value = "43";

                AddDecimal(cmd, "@Cantidad", l.Cantidad, 18, 2);
                AddDecimal(cmd, "@PrecioUnitario", l.PrecioUnitario);
                AddDecimal(cmd, "@DescuentoPct", l.DescuentoPct, 9, 2);
                AddDecimal(cmd, "@DescuentoMonto", l.DescuentoMonto);
                AddDecimal(cmd, "@ImpuestoPct", l.ItbisPorc, 9, 4);
                AddDecimal(cmd, "@ImpuestoMonto", l.ItbisMonto);
                AddDecimal(cmd, "@TotalLinea", l.TotalMoneda);
                AddDecimal(cmd, "@Precio", l.PrecioUnitario);
                AddDecimal(cmd, "@ItbisPct", l.ItbisPorc, 9, 4);
                AddDecimal(cmd, "@ItbisMonto", l.ItbisMonto);

                cmd.Parameters.Add("@CodBarra", SqlDbType.VarChar, 50).Value = DBNull.Value;
                cmd.Parameters.Add("@NumeroLineaECF", SqlDbType.Int).Value = l.Linea;

                var montoExento = l.ItbisPorc <= 0 ? l.ImporteMoneda : 0m;
                var montoGravado = l.ItbisPorc > 0 ? l.ImporteMoneda : 0m;

                AddDecimal(cmd, "@MontoGravado", montoGravado);
                AddDecimal(cmd, "@MontoExento", montoExento);
                cmd.Parameters.Add("@EsExento", SqlDbType.Bit).Value = l.ItbisPorc <= 0;
                AddDecimal(cmd, "@PrecioUnitarioBase", l.PrecioUnitario, 18, 4);
                AddDecimal(cmd, "@SubtotalLineaAntesImpuesto", l.ImporteMoneda);
                cmd.Parameters.Add("@IndicadorITBISIncluido", SqlDbType.Bit).Value = l.PrecioIncluyeITBIS;
                cmd.Parameters.Add("@CodigoProducto", SqlDbType.VarChar, 20).Value =
                    (object?)Coalesce(l.ProductoCodigo, l.NoProducto) ?? DBNull.Value;
                AddDecimal(cmd, "@SubtotalLinea", l.ImporteMoneda, 18, 6);
                AddDecimal(cmd, "@BaseImponible", l.ImporteMoneda, 18, 6);

                cmd.ExecuteNonQuery();
            }
        }

        private void InsertarFacturaPago(
            SqlConnection cn,
            SqlTransaction tx,
            int facturaId,
            VentaCabLite venta,
            string usuario)
        {
            using var cmd = new SqlCommand(@"
INSERT INTO dbo.FacturaPago
(
    FacturaId,
    FechaPago,
    Monto,
    MedioPago,
    Referencia,
    Observacion,
    Usuario
)
VALUES
(
    @FacturaId,
    @FechaPago,
    @Monto,
    @MedioPago,
    @Referencia,
    @Observacion,
    @Usuario
);", cn, tx);

            cmd.Parameters.Add("@FacturaId", SqlDbType.Int).Value = facturaId;
            cmd.Parameters.Add("@FechaPago", SqlDbType.DateTime).Value = venta.Fecha;
            AddDecimal(cmd, "@Monto", venta.Total);
            cmd.Parameters.Add("@MedioPago", SqlDbType.VarChar, 20).Value = "POS";
            cmd.Parameters.Add("@Referencia", SqlDbType.NVarChar, 120).Value = $"Venta POS {venta.NoDocumento}";
            cmd.Parameters.Add("@Observacion", SqlDbType.NVarChar, 400).Value = "Generado desde POS";
            cmd.Parameters.Add("@Usuario", SqlDbType.VarChar, 30).Value = usuario;

            cmd.ExecuteNonQuery();
        }

        private void InsertarFacturaFormaPago(
            SqlConnection cn,
            SqlTransaction tx,
            int facturaId,
            long ventaId)
        {
            var pagos = ObtenerPagosPos(cn, tx, ventaId);

            if (pagos.Count == 0)
                throw new InvalidOperationException(
                    "La venta no tiene pagos registrados en POS_Pago. No se puede generar FacturaFormaPago.");

            var orden = 1;

            foreach (var p in pagos)
            {
                if (string.IsNullOrWhiteSpace(p.FormaPagoCodigo))
                    throw new InvalidOperationException(
                        "Un pago POS no tiene FormaPagoCodigo. La data está inconsistente.");

                using var cmd = new SqlCommand(@"
INSERT INTO dbo.FacturaFormaPago
(
    FacturaId,
    FormaPago,
    MontoPago,
    Orden
)
VALUES
(
    @FacturaId,
    @FormaPago,
    @MontoPago,
    @Orden
);", cn, tx);

                cmd.Parameters.Add("@FacturaId", SqlDbType.Int).Value = facturaId;
                cmd.Parameters.Add("@FormaPago", SqlDbType.VarChar, 2).Value = p.FormaPagoCodigo;
                AddDecimal(cmd, "@MontoPago", p.Monto);
                cmd.Parameters.Add("@Orden", SqlDbType.Int).Value = orden++;

                cmd.ExecuteNonQuery();
            }
        }

        private List<PagoPosLite> ObtenerPagosPos(SqlConnection cn, SqlTransaction tx, long ventaId)
        {
            var lista = new List<PagoPosLite>();

            using var cmd = new SqlCommand(@"
SELECT
    p.Monto,
    p.FormaPagoCodigo
FROM dbo.POS_Pago p
WHERE p.VentaId = @VentaId
ORDER BY p.PagoId;", cn, tx);

            cmd.Parameters.Add("@VentaId", SqlDbType.BigInt).Value = ventaId;

            using var rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                lista.Add(new PagoPosLite
                {
                    Monto = rd.GetDecimal(0),
                    FormaPagoCodigo = rd.IsDBNull(1) ? null : rd.GetString(1)
                });
            }

            return lista;
        }

        private void CrearEcfDocumentoBase(
            SqlConnection cn,
            SqlTransaction tx,
            int facturaId,
            int tipoECFId,
            decimal montoTotal)
        {
            using var existeCmd = new SqlCommand(@"
SELECT COUNT(1)
FROM dbo.ECFDocumento
WHERE FacturaId = @FacturaId;", cn, tx);

            existeCmd.Parameters.Add("@FacturaId", SqlDbType.Int).Value = facturaId;

            var existe = Convert.ToInt32(existeCmd.ExecuteScalar() ?? 0);
            if (existe > 0)
                return;

            using var cmd = new SqlCommand(@"
INSERT INTO dbo.ECFDocumento
(
    FacturaId,
    TipoECF,
    ENCF,
    EstadoDGII,
    TrackId,
    XmlSinFirmar,
    XmlFirmado,
    RespuestaDGII,
    FechaGenerado,
    IntentosEnvio,
    UltimoError,
    XmlEnviado,
    XmlRespuesta,
    CodigoRespuestaDGII,
    RespuestaDGIITexto,
    MontoTotalDocumento,
    EstadoProceso,
    OrigenEmision,
    Activo
)
VALUES
(
    @FacturaId,
    @TipoECF,
    @ENCF,
    @EstadoDGII,
    @TrackId,
    NULL,
    NULL,
    NULL,
    SYSDATETIME(),
    0,
    NULL,
    NULL,
    NULL,
    NULL,
    NULL,
    @MontoTotalDocumento,
    @EstadoProceso,
    @OrigenEmision,
    1
);", cn, tx);

            cmd.Parameters.Add("@FacturaId", SqlDbType.Int).Value = facturaId;
            cmd.Parameters.Add("@TipoECF", SqlDbType.Int).Value = tipoECFId;
            cmd.Parameters.Add("@ENCF", SqlDbType.VarChar, 20).Value = string.Empty;
            cmd.Parameters.Add("@EstadoDGII", SqlDbType.VarChar, 20).Value = "PENDIENTE";
            cmd.Parameters.Add("@TrackId", SqlDbType.VarChar, 80).Value = DBNull.Value;
            AddDecimal(cmd, "@MontoTotalDocumento", montoTotal);
            cmd.Parameters.Add("@EstadoProceso", SqlDbType.VarChar, 20).Value = "BORRADOR";
            cmd.Parameters.Add("@OrigenEmision", SqlDbType.VarChar, 30).Value = "POS";

            cmd.ExecuteNonQuery();
        }

        private FacturaCabEcfLite? ObtenerFacturaCabParaEcf(SqlConnection cn, int facturaId)
        {
            using var cmd = new SqlCommand(@"
SELECT TOP (1)
    FacturaId,
    TipoECFId,
    ENCF
FROM dbo.FacturaCab
WHERE FacturaId = @FacturaId;", cn);

            cmd.Parameters.Add("@FacturaId", SqlDbType.Int).Value = facturaId;

            using var rd = cmd.ExecuteReader();
            if (!rd.Read())
                return null;

            return new FacturaCabEcfLite
            {
                FacturaId = rd.GetInt32(0),
                TipoECFId = rd.IsDBNull(1) ? 0 : rd.GetInt32(1),
                ENCF = rd.IsDBNull(2) ? null : rd.GetString(2)
            };
        }

        private static string ResolverPrefijoEcfDesdeTipoId(int tipoEcfId)
        {
            return tipoEcfId switch
            {
                1 => "E31",
                2 => "E32",
                3 => "E33",
                4 => "E34",
                5 => "E41",
                6 => "E43",
                7 => "E44",
                8 => "E45",
                9 => "E46",
                10 => "E47",
                _ => throw new InvalidOperationException("TipoECFId no reconocido para resolver prefijo e-CF.")
            };
        }

        private PosFiscalResult ProcesarDocumentoFiscalGenerado(int facturaId, string numeroDocumento, string usuario)
        {
            if (facturaId <= 0)
                throw new InvalidOperationException("FacturaId inválido para procesar e-CF.");

            try
            {
                using var cn = Db.GetOpenConnection();

                var cab = ObtenerFacturaCabParaEcf(cn, facturaId);
                if (cab == null)
                    throw new InvalidOperationException($"No se pudo cargar la factura {facturaId} para e-CF.");

                if (cab.TipoECFId <= 0)
                    throw new InvalidOperationException("La factura no tiene TipoECFId válido.");

                var prefijo = ResolverPrefijoEcfDesdeTipoId(cab.TipoECFId);
                var tipoEcf = int.Parse(prefijo.Substring(1, 2));

                var s = SesionService.Current;
                var cajaId = s.CajaId ?? 0;

                var encf = string.IsNullOrWhiteSpace(cab.ENCF)
                    ? new ECFSqlRepository().GenerarENcf(
                        empresaId: s.EmpresaId,
                        sucursalId: s.SucursalId,
                        cajaId: cajaId,
                        facturaId: facturaId,
                        tipoEcf: tipoEcf,
                        prefijo: prefijo)
                    : cab.ENCF;

                if (string.IsNullOrWhiteSpace(encf))
                    throw new InvalidOperationException("No se pudo generar el eNCF para la factura.");

                var ecfSvc = new ECFService();
                ecfSvc.GenerarXmlPendiente(facturaId, tipoEcf, encf);
                var alanube = new ECFAlanubeService();
                var resp = alanube.EnviarFactura(facturaId, usuario);

                using var cmd = new SqlCommand(@"
SELECT TOP (1)
    d.ENCF,
    d.TrackId,
    d.EstadoProceso
FROM dbo.ECFDocumento d
WHERE d.FacturaId = @FacturaId
ORDER BY d.ECFDocumentoId DESC;", cn);

                cmd.Parameters.Add("@FacturaId", SqlDbType.Int).Value = facturaId;

                using var rd = cmd.ExecuteReader();
                if (!rd.Read())
                    throw new InvalidOperationException(
                        $"No se encontró ECFDocumento luego de generar XML pendiente. FacturaId={facturaId}");

                return new PosFiscalResult
                {
                    FacturaId = facturaId,
                    NumeroDocumento = numeroDocumento,
                    ENCF = rd.IsDBNull(0) ? null : rd.GetString(0),
                    TrackId = rd.IsDBNull(1) ? null : rd.GetString(1),
                    EstadoECF = rd.IsDBNull(2) ? null : rd.GetString(2),
                    EcfDocumentoCreado = true
                };
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    $"La factura fiscal fue creada, pero falló el procesamiento e-CF. FacturaId={facturaId}. Detalle: {ex.Message}",
                    ex);
            }
        }

        private void MarcarVentaComoFacturadaFiscalmente(
            SqlConnection cn,
            SqlTransaction tx,
            long ventaId,
            int facturaId)
        {
            using var cmd = new SqlCommand(@"
UPDATE dbo.VentaCab
SET Observacion =
    CASE
        WHEN Observacion IS NULL OR LTRIM(RTRIM(Observacion)) = ''
            THEN CONCAT('POS | FacturaFiscalId=', @FacturaId)
        ELSE CONCAT(Observacion, ' | FacturaFiscalId=', @FacturaId)
    END,
    FechaActualizacion = GETDATE()
WHERE VentaId = @VentaId;", cn, tx);

            cmd.Parameters.Add("@FacturaId", SqlDbType.Int).Value = facturaId;
            cmd.Parameters.Add("@VentaId", SqlDbType.BigInt).Value = ventaId;

            cmd.ExecuteNonQuery();
        }

        private static string? Coalesce(params string?[] values)
        {
            foreach (var v in values)
            {
                if (!string.IsNullOrWhiteSpace(v))
                    return v;
            }

            return null;
        }

        private static void AddDecimal(
            SqlCommand cmd,
            string name,
            decimal value,
            byte precision = 18,
            byte scale = 2)
        {
            var p = cmd.Parameters.Add(name, SqlDbType.Decimal);
            p.Precision = precision;
            p.Scale = scale;
            p.Value = value;
        }

        

        private sealed class VentaCabLite
        {
            public long VentaId { get; set; }
            public string NoDocumento { get; set; } = "";
            public DateTime Fecha { get; set; }
            public string? ClienteCodigo { get; set; }
            public string MonedaCodigo { get; set; } = "DOP";
            public decimal TasaCambio { get; set; }
            public decimal Subtotal { get; set; }
            public decimal DescuentoTotal { get; set; }
            public decimal ImpuestoTotal { get; set; }
            public decimal Total { get; set; }
            public string Estado { get; set; } = "";
            public string? Usuario { get; set; }
            public string? Observacion { get; set; }
            public DateTime FechaCreacion { get; set; }
            public decimal MontoPago { get; set; }
            public decimal MontoCambio { get; set; }
            public int? ClienteId { get; set; }
            public decimal SubTotalMoneda { get; set; }
            public decimal ItbisMoneda { get; set; }
            public decimal TotalMoneda { get; set; }
            public string? NombreCliente { get; set; }
            public string? EmailCliente { get; set; }
            public string? TelefonoCliente { get; set; }
            public int? PosCajaId { get; set; }
            public string? PosCajaNumero { get; set; }
            public int? CajaId { get; set; }
            public int TerminoPagoId { get; set; }
        }

        private sealed class VentaLinLite
        {
            public long VentaId { get; set; }
            public int Linea { get; set; }
            public string NoProducto { get; set; } = "";
            public string Descripcion { get; set; } = "";
            public decimal Cantidad { get; set; }
            public decimal PrecioUnitario { get; set; }
            public decimal DescuentoLinea { get; set; }
            public decimal ItbisPorc { get; set; }
            public decimal ItbisMonto { get; set; }
            public decimal Importe { get; set; }
            public string? ProductoCodigo { get; set; }
            public decimal PrecioUnit { get; set; }
            public decimal DescuentoMoneda { get; set; }
            public decimal ImporteMoneda { get; set; }
            public decimal ItbisMoneda { get; set; }
            public decimal TotalMoneda { get; set; }
            public decimal DescuentoPct { get; set; }
            public decimal DescuentoMonto { get; set; }
            public bool PrecioIncluyeITBIS { get; set; }
        }

        private sealed class TipoEcfLite
        {
            public int TipoECFId { get; set; }
            public string CodigoTipoECF { get; set; } = "";
            public string Prefijo { get; set; } = "";
            public string Nombre { get; set; } = "";
            public bool AplicaCompradorRnc { get; set; }
        }

        private sealed class TipoPagoEcfLite
        {
            public int TipoPagoECFId { get; set; }
            public string CodigoDGII { get; set; } = "";
            public string Descripcion { get; set; } = "";
        }

        private sealed class FacturaContextoLite
        {
            public int CajaId { get; set; }
            public int? SucursalId { get; set; }
            public int EmpresaId { get; set; }
            public int AlmacenId { get; set; }
        }

        private sealed class ClienteSnapshotLite
        {
            public string? Codigo { get; set; }
            public string? Nombre { get; set; }
            public string? RncCedula { get; set; }
            public string? Direccion { get; set; }
            public string? Correo { get; set; }
        }

        private sealed class PagoPosLite
        {
            public decimal Monto { get; set; }
            public string? FormaPagoCodigo { get; set; }
        }

        private sealed class FacturaCabEcfLite
        {
            public int FacturaId { get; set; }
            public int TipoECFId { get; set; }
            public string? ENCF { get; set; }
        }
    }
}