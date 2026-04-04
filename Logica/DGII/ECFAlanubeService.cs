using Andloe.Data;
using Andloe.Data.DGII;
using Andloe.Data.Fiscal;
using Andloe.Entidad;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.Json;

namespace Andloe.Logica.DGII
{
    public sealed class ECFAlanubeService
    {
        private readonly AlanubeClient _alanubeClient;
        private readonly ECFDocumentoRepository _ecfDocumentoRepository;

        public ECFAlanubeService()
        {
            _alanubeClient = new AlanubeClient();
            _ecfDocumentoRepository = new ECFDocumentoRepository();
        }

        public string GenerarPayloadJson(int facturaId)
        {
            if (facturaId <= 0)
                throw new ArgumentException("FacturaId inválido.", nameof(facturaId));

            var cab = CargarCabecera(facturaId)
                      ?? throw new InvalidOperationException("No se encontró cabecera fiscal.");

            var det = CargarDetalle(facturaId);
            ValidarDocumento(cab, det);

            var request = MapToAlanubeRequest(cab, det);

            return JsonSerializer.Serialize(request, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false
            });
        }

        public AlanubeEmitResponseDto EnviarFactura(int facturaId, string? usuario = null)
        {
            if (facturaId <= 0)
                throw new ArgumentException("FacturaId inválido.", nameof(facturaId));

            var json = GenerarPayloadJson(facturaId);
            var cab = CargarCabecera(facturaId)
                      ?? throw new InvalidOperationException("No se encontró cabecera fiscal.");

            var tipoReal = ResolverTipoEcfReal(cab.ENCF);

            try
            {
                _ecfDocumentoRepository.GuardarRespuestaAlanubePorFactura(
                    facturaId,
                    trackId: null,
                    status: "PENDIENTE",
                    legalStatus: "PENDIENTE",
                    codigo: null,
                    mensaje: "Payload Alanube generado.",
                    rawJson: json);

                AlanubeEmitResponseDto resp = tipoReal switch
                {
                    31 => _alanubeClient.EmitirFactura31(json),
                    32 => _alanubeClient.EmitirFactura32(json),
                    _ => throw new NotSupportedException($"Tipo e-CF no soportado para Alanube Sandbox: {tipoReal}")
                };

                var track = Limpia(resp.GetTrackOrId());
                var status = Limpia(resp.Status);
                var legalStatus = Limpia(resp.LegalStatus);
                var message = Limpia(resp.Message);

                _ecfDocumentoRepository.GuardarRespuestaAlanubePorFactura(
                    facturaId,
                    trackId: track,
                    status: status,
                    legalStatus: legalStatus,
                    codigo: null,
                    mensaje: message,
                    rawJson: resp.RawJson);

                return resp;
            }
            catch (Exception ex)
            {
                _ecfDocumentoRepository.MarcarErrorPorFactura(facturaId, ex.Message);
                throw;
            }
        }

        public AlanubeStatusResponseDto ConsultarFactura(int facturaId)
        {
            if (facturaId <= 0)
                throw new ArgumentException("FacturaId inválido.", nameof(facturaId));

            var cab = CargarCabecera(facturaId)
                      ?? throw new InvalidOperationException("No se encontró cabecera fiscal.");

            var tipoReal = ResolverTipoEcfReal(cab.ENCF);

            var trackId = Limpia(_ecfDocumentoRepository.ObtenerTrackId(facturaId));
            if (string.IsNullOrWhiteSpace(trackId))
                throw new InvalidOperationException("El documento no tiene TrackId para consultar en Alanube.");

            try
            {
                AlanubeStatusResponseDto resp = tipoReal switch
                {
                    31 => _alanubeClient.ConsultarFactura31(trackId),
                    32 => _alanubeClient.ConsultarFactura32(trackId),
                    _ => throw new NotSupportedException($"Tipo e-CF no soportado para Alanube Sandbox: {tipoReal}")
                };

                _ecfDocumentoRepository.RegistrarConsultaAlanubePorFactura(
                    facturaId,
                    status: Limpia(resp.Status),
                    legalStatus: Limpia(resp.LegalStatus),
                    codigo: Limpia(resp.Code),
                    mensaje: Limpia(resp.Message),
                    rawJson: resp.RawJson);

                return resp;
            }
            catch (Exception ex)
            {
                _ecfDocumentoRepository.MarcarErrorPorFactura(facturaId, ex.Message);
                throw;
            }
        }

        private static void ValidarDocumento(
            EcfFacturaCabeceraRow cab,
            IReadOnlyCollection<EcfFacturaDetalleRow> det)
        {
            var tipoReal = ResolverTipoEcfReal(cab.ENCF);

            if (tipoReal is not 31 and not 32)
                throw new InvalidOperationException($"Alanube Sandbox solo está preparado para 31 y 32. Tipo real: {tipoReal}");

            if (cab.FechaDocumento == default)
                throw new InvalidOperationException("La factura no tiene FechaDocumento.");

            if ((cab.TotalGeneral ?? 0m) <= 0m)
                throw new InvalidOperationException("La factura no tiene TotalGeneral válido.");

            if (det.Count == 0)
                throw new InvalidOperationException("La factura no tiene detalle.");

            foreach (var item in det)
            {
                if (item.Cantidad <= 0m)
                    throw new InvalidOperationException($"La línea {item.FacturaDetId} tiene cantidad inválida.");

                if (item.PrecioUnitario < 0m)
                    throw new InvalidOperationException($"La línea {item.FacturaDetId} tiene precio inválido.");
            }
        }

        private static AlanubeInvoiceRequestDto MapToAlanubeRequest(
            EcfFacturaCabeceraRow cab,
            IReadOnlyCollection<EcfFacturaDetalleRow> det)
        {
            var tipoReal = ResolverTipoEcfReal(cab.ENCF);

            return new AlanubeInvoiceRequestDto
            {
                DocumentType = tipoReal == 31 ? "fiscal" : "consumer",
                ENcf = cab.ENCF,
                IssueDate = cab.FechaDocumento,

                Issuer = new AlanubePartyDto
                {
                    TaxId = Limpia(cab.EmisorRnc),
                    Name = Limpia(cab.EmisorNombre),
                    Email = Limpia(cab.EmisorEmail),
                    Address = Limpia(cab.EmisorDireccion)
                },

                Buyer = new AlanubePartyDto
                {
                    TaxId = Limpia(cab.RncCompradorSnapshot ?? cab.DocumentoCliente),
                    Name = Limpia(cab.RazonSocialCompradorSnapshot ?? cab.NombreCliente),
                    Email = Limpia(cab.CorreoCompradorSnapshot),
                    Address = Limpia(cab.DireccionCompradorSnapshot ?? cab.DireccionCliente)
                },

                IncomeType = cab.TipoIngresoId?.ToString(),
                PaymentType = cab.TipoPagoECFId?.ToString(),
                Notes = Limpia(cab.Observacion),

                TaxedAmount = cab.MontoGravadoTotal ?? 0m,
                ExemptAmount = cab.MontoExentoTotal ?? 0m,
                TaxAmount = cab.TotalImpuesto ?? 0m,
                DiscountAmount = cab.TotalDescuento ?? 0m,
                TotalAmount = cab.TotalGeneral ?? 0m,

                Items = det
                    .OrderBy(x => x.NumeroLineaECF ?? x.FacturaDetId)
                    .Select((x, i) => new AlanubeInvoiceItemDto
                    {
                        LineNumber = x.NumeroLineaECF ?? (i + 1),
                        Code = Limpia(x.CodigoItemFiscal ?? x.CodigoProducto ?? x.ProductoCodigo),
                        Description = Limpia(x.DescripcionFiscal ?? x.Descripcion) ?? "ITEM",
                        UnitCode = Limpia(x.UnidadMedidaDGII ?? x.Unidad),
                        Quantity = x.Cantidad,
                        UnitPrice = x.PrecioUnitario,
                        TaxedAmount = x.MontoGravado ?? 0m,
                        ExemptAmount = x.MontoExento ?? 0m,
                        TaxAmount = x.ItbisMonto ?? 0m,
                        DiscountAmount = x.DescuentoMonto ?? 0m
                    })
                    .ToList()
            };
        }

        private static int ResolverTipoEcfReal(string? encf)
        {
            var v = (encf ?? "").Trim().ToUpperInvariant();

            if (string.IsNullOrWhiteSpace(v))
                throw new InvalidOperationException("El documento no tiene eNCF.");

            if (v.StartsWith("E31")) return 31;
            if (v.StartsWith("E32")) return 32;
            if (v.StartsWith("E34")) return 34;

            throw new InvalidOperationException("No se pudo determinar el tipo e-CF desde el eNCF: " + v);
        }

        private static string? Limpia(string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) return null;
            return value.Trim();
        }

        private static EcfFacturaCabeceraRow? CargarCabecera(int facturaId)
        {
            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
SELECT TOP(1)
    fc.FacturaId,
    fc.EmpresaId,
    fc.SucursalId,
    fc.ClienteId,
    fc.TipoECFId,
    fc.TipoIngresoId,
    fc.TipoPagoECFId,
    fc.FechaDocumento,
    fc.eNCF,
    fc.NombreCliente,
    fc.DocumentoCliente,
    fc.DireccionCliente,
    fc.RncCompradorSnapshot,
    fc.RazonSocialCompradorSnapshot,
    fc.CorreoCompradorSnapshot,
    fc.DireccionCompradorSnapshot,
    fc.TotalDescuento,
    fc.TotalImpuesto,
    fc.TotalGeneral,
    fc.MontoGravadoTotal,
    fc.MontoExentoTotal,
    fc.Observacion,
    e.RazonSocial AS EmisorNombre,
    e.RNC AS EmisorRnc,
    e.Email AS EmisorEmail,
    e.Direccion AS EmisorDireccion
FROM dbo.vw_ECFFacturaCabecera fc
LEFT JOIN dbo.Empresa e
    ON e.EmpresaId = fc.EmpresaId
WHERE fc.FacturaId = @FacturaId;", cn);

            cmd.Parameters.Add("@FacturaId", SqlDbType.Int).Value = facturaId;

            using var rd = cmd.ExecuteReader();
            if (!rd.Read()) return null;

            return new EcfFacturaCabeceraRow
            {
                FacturaId = rd.GetInt32(rd.GetOrdinal("FacturaId")),
                EmpresaId = rd.GetInt32(rd.GetOrdinal("EmpresaId")),
                SucursalId = rd.GetInt32(rd.GetOrdinal("SucursalId")),
                ClienteId = rd.GetInt32(rd.GetOrdinal("ClienteId")),
                TipoECFId = rd.GetInt32(rd.GetOrdinal("TipoECFId")),
                TipoIngresoId = rd["TipoIngresoId"] == DBNull.Value ? null : Convert.ToInt32(rd["TipoIngresoId"]),
                TipoPagoECFId = rd["TipoPagoECFId"] == DBNull.Value ? null : Convert.ToInt32(rd["TipoPagoECFId"]),
                FechaDocumento = Convert.ToDateTime(rd["FechaDocumento"]),
                ENCF = rd["eNCF"] == DBNull.Value ? null : Convert.ToString(rd["eNCF"]),
                NombreCliente = rd["NombreCliente"] == DBNull.Value ? null : Convert.ToString(rd["NombreCliente"]),
                DocumentoCliente = rd["DocumentoCliente"] == DBNull.Value ? null : Convert.ToString(rd["DocumentoCliente"]),
                DireccionCliente = rd["DireccionCliente"] == DBNull.Value ? null : Convert.ToString(rd["DireccionCliente"]),
                RncCompradorSnapshot = rd["RncCompradorSnapshot"] == DBNull.Value ? null : Convert.ToString(rd["RncCompradorSnapshot"]),
                RazonSocialCompradorSnapshot = rd["RazonSocialCompradorSnapshot"] == DBNull.Value ? null : Convert.ToString(rd["RazonSocialCompradorSnapshot"]),
                CorreoCompradorSnapshot = rd["CorreoCompradorSnapshot"] == DBNull.Value ? null : Convert.ToString(rd["CorreoCompradorSnapshot"]),
                DireccionCompradorSnapshot = rd["DireccionCompradorSnapshot"] == DBNull.Value ? null : Convert.ToString(rd["DireccionCompradorSnapshot"]),
                TotalDescuento = rd["TotalDescuento"] == DBNull.Value ? null : Convert.ToDecimal(rd["TotalDescuento"]),
                TotalImpuesto = rd["TotalImpuesto"] == DBNull.Value ? null : Convert.ToDecimal(rd["TotalImpuesto"]),
                TotalGeneral = rd["TotalGeneral"] == DBNull.Value ? null : Convert.ToDecimal(rd["TotalGeneral"]),
                MontoGravadoTotal = rd["MontoGravadoTotal"] == DBNull.Value ? null : Convert.ToDecimal(rd["MontoGravadoTotal"]),
                MontoExentoTotal = rd["MontoExentoTotal"] == DBNull.Value ? null : Convert.ToDecimal(rd["MontoExentoTotal"]),
                Observacion = rd["Observacion"] == DBNull.Value ? null : Convert.ToString(rd["Observacion"]),
                EmisorNombre = rd["EmisorNombre"] == DBNull.Value ? null : Convert.ToString(rd["EmisorNombre"]),
                EmisorRnc = rd["EmisorRnc"] == DBNull.Value ? null : Convert.ToString(rd["EmisorRnc"]),
                EmisorEmail = rd["EmisorEmail"] == DBNull.Value ? null : Convert.ToString(rd["EmisorEmail"]),
                EmisorDireccion = rd["EmisorDireccion"] == DBNull.Value ? null : Convert.ToString(rd["EmisorDireccion"])
            };
        }

        private static List<EcfFacturaDetalleRow> CargarDetalle(int facturaId)
        {
            var list = new List<EcfFacturaDetalleRow>();

            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
SELECT
    FacturaDetId,
    FacturaId,
    NumeroLineaECF,
    ProductoCodigo,
    CodigoProducto,
    Descripcion,
    DescripcionFiscal,
    Unidad,
    Cantidad,
    PrecioUnitario,
    DescuentoMonto,
    ItbisMonto,
    MontoGravado,
    MontoExento,
    UnidadMedidaDGII,
    CodigoItemFiscal
FROM dbo.vw_ECFFacturaDetalle
WHERE FacturaId = @FacturaId
ORDER BY ISNULL(NumeroLineaECF, 999999), FacturaDetId;", cn);

            cmd.Parameters.Add("@FacturaId", SqlDbType.Int).Value = facturaId;

            using var rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                list.Add(new EcfFacturaDetalleRow
                {
                    FacturaDetId = Convert.ToInt32(rd["FacturaDetId"]),
                    FacturaId = Convert.ToInt32(rd["FacturaId"]),
                    NumeroLineaECF = rd["NumeroLineaECF"] == DBNull.Value ? null : Convert.ToInt32(rd["NumeroLineaECF"]),
                    ProductoCodigo = rd["ProductoCodigo"] == DBNull.Value ? null : Convert.ToString(rd["ProductoCodigo"]),
                    CodigoProducto = rd["CodigoProducto"] == DBNull.Value ? null : Convert.ToString(rd["CodigoProducto"]),
                    Descripcion = rd["Descripcion"] == DBNull.Value ? null : Convert.ToString(rd["Descripcion"]),
                    DescripcionFiscal = rd["DescripcionFiscal"] == DBNull.Value ? null : Convert.ToString(rd["DescripcionFiscal"]),
                    Unidad = rd["Unidad"] == DBNull.Value ? null : Convert.ToString(rd["Unidad"]),
                    Cantidad = Convert.ToDecimal(rd["Cantidad"]),
                    PrecioUnitario = Convert.ToDecimal(rd["PrecioUnitario"]),
                    DescuentoMonto = rd["DescuentoMonto"] == DBNull.Value ? null : Convert.ToDecimal(rd["DescuentoMonto"]),
                    ItbisMonto = rd["ItbisMonto"] == DBNull.Value ? null : Convert.ToDecimal(rd["ItbisMonto"]),
                    MontoGravado = rd["MontoGravado"] == DBNull.Value ? null : Convert.ToDecimal(rd["MontoGravado"]),
                    MontoExento = rd["MontoExento"] == DBNull.Value ? null : Convert.ToDecimal(rd["MontoExento"]),
                    UnidadMedidaDGII = rd["UnidadMedidaDGII"] == DBNull.Value ? null : Convert.ToString(rd["UnidadMedidaDGII"]),
                    CodigoItemFiscal = rd["CodigoItemFiscal"] == DBNull.Value ? null : Convert.ToString(rd["CodigoItemFiscal"])
                });
            }

            return list;
        }

        private sealed class EcfFacturaCabeceraRow
        {
            public int FacturaId { get; set; }
            public int EmpresaId { get; set; }
            public int SucursalId { get; set; }
            public int ClienteId { get; set; }
            public int TipoECFId { get; set; }
            public int? TipoIngresoId { get; set; }
            public int? TipoPagoECFId { get; set; }
            public DateTime FechaDocumento { get; set; }
            public string? ENCF { get; set; }

            public string? NombreCliente { get; set; }
            public string? DocumentoCliente { get; set; }
            public string? DireccionCliente { get; set; }

            public string? RncCompradorSnapshot { get; set; }
            public string? RazonSocialCompradorSnapshot { get; set; }
            public string? CorreoCompradorSnapshot { get; set; }
            public string? DireccionCompradorSnapshot { get; set; }

            public decimal? TotalDescuento { get; set; }
            public decimal? TotalImpuesto { get; set; }
            public decimal? TotalGeneral { get; set; }
            public decimal? MontoGravadoTotal { get; set; }
            public decimal? MontoExentoTotal { get; set; }

            public string? Observacion { get; set; }

            public string? EmisorNombre { get; set; }
            public string? EmisorRnc { get; set; }
            public string? EmisorEmail { get; set; }
            public string? EmisorDireccion { get; set; }
        }

        private sealed class EcfFacturaDetalleRow
        {
            public int FacturaDetId { get; set; }
            public int FacturaId { get; set; }
            public int? NumeroLineaECF { get; set; }
            public string? ProductoCodigo { get; set; }
            public string? CodigoProducto { get; set; }
            public string? Descripcion { get; set; }
            public string? DescripcionFiscal { get; set; }
            public string? Unidad { get; set; }
            public decimal Cantidad { get; set; }
            public decimal PrecioUnitario { get; set; }
            public decimal? DescuentoMonto { get; set; }
            public decimal? ItbisMonto { get; set; }
            public decimal? MontoGravado { get; set; }
            public decimal? MontoExento { get; set; }
            public string? UnidadMedidaDGII { get; set; }
            public string? CodigoItemFiscal { get; set; }
        }
    }
}