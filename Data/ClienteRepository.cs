using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Text;
using Microsoft.Data.SqlClient;
using Andloe.Entidad;

namespace Andloe.Data
{
    public class ClienteRepository
    {
        public const string CODIGO_CONSUMIDOR_FINAL = "C-000001";

        public List<Cliente> Listar(string? filtro = null, int top = 200)
        {
            var list = new List<Cliente>();
            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
SELECT TOP(@top)
    [Código], [Nombre], [RNC_Cedula], [Telefono], [Email], [Direccion],
    [Tipo], [Estado], [FechaCreacion],
    CreditoMaximo, CodDivisas, CodTerminoPagos, CodVendedor, CodAlmacen,
    ClienteId, DescuentoPctMax,
    RazonSocialFiscal, NombreComercialFiscal, TipoIdentificacionFiscal,
    MunicipioCodigo, ProvinciaCodigo, PaisCodigo, CorreoFiscal,
    EsContribuyente, TipoClienteFiscal, ValidadoDGII, FechaValidacionDGII,
    EstadoRncDGII, IdentificadorExtranjero, EsExtranjero
FROM dbo.Cliente
WHERE (@filtro IS NULL OR [Código] LIKE @like OR [Nombre] LIKE @like OR [RNC_Cedula] LIKE @like)
ORDER BY [FechaCreacion] DESC, [Código] DESC;", cn);

            cmd.Parameters.Add("@top", SqlDbType.Int).Value = top;

            if (string.IsNullOrWhiteSpace(filtro))
            {
                cmd.Parameters.Add("@filtro", SqlDbType.NVarChar, 100).Value = DBNull.Value;
                cmd.Parameters.Add("@like", SqlDbType.NVarChar, 100).Value = DBNull.Value;
            }
            else
            {
                cmd.Parameters.Add("@filtro", SqlDbType.NVarChar, 100).Value = filtro;
                cmd.Parameters.Add("@like", SqlDbType.NVarChar, 100).Value = "%" + filtro + "%";
            }

            using var rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                list.Add(MapCliente(rd));
            }
            return list;
        }

        public ClienteDto? BuscarPorCodigoORnc(string valor)
        {
            if (string.IsNullOrWhiteSpace(valor)) return null;

            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
SELECT TOP(1)
       c.ClienteId,
       c.[Código]      AS Codigo,
       c.[Nombre],
       c.CodVendedor,
       c.[RNC_Cedula],
       c.[Direccion],
       c.[Telefono],
       c.[Tipo]
FROM dbo.Cliente c
WHERE c.[RNC_Cedula] = @v
   OR c.[Código]      = @v;", cn);

            cmd.Parameters.Add("@v", SqlDbType.VarChar, 50).Value = valor.Trim();

            using var rd = cmd.ExecuteReader();
            if (!rd.Read()) return null;

            return new ClienteDto
            {
                ClienteId = rd.IsDBNull(0) ? 0 : rd.GetInt32(0),
                Codigo = rd.IsDBNull(1) ? "" : rd.GetString(1),
                Nombre = rd.IsDBNull(2) ? "" : rd.GetString(2),
                CodVendedor = rd.IsDBNull(3) ? null : rd.GetString(3),
                RncCedula = rd.IsDBNull(4) ? null : rd.GetString(4),
                Direccion = rd.IsDBNull(5) ? null : rd.GetString(5),
                Telefono = rd.IsDBNull(6) ? null : rd.GetString(6),
                Tipo = rd.IsDBNull(7) ? (byte)0 : rd.GetByte(7)
            };
        }

        public ClienteDto ObtenerConsumidorFinal()
        {
            var cli = BuscarPorCodigoORnc(CODIGO_CONSUMIDOR_FINAL);

            if (cli == null || cli.ClienteId <= 0)
                throw new Exception(
                    $"No existe el cliente por defecto '{CODIGO_CONSUMIDOR_FINAL}'. " +
                    $"Crea dbo.Cliente con Código='{CODIGO_CONSUMIDOR_FINAL}'."
                );

            return cli;
        }

        public void ValidarConsumidorFinal_ErpPro()
        {
            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
SELECT TOP(1) ClienteId, [Código], [Nombre], [Estado]
FROM dbo.Cliente
WHERE [Código] = @cod;", cn);

            cmd.Parameters.Add("@cod", SqlDbType.VarChar, 20).Value = CODIGO_CONSUMIDOR_FINAL;

            using var rd = cmd.ExecuteReader();
            if (!rd.Read())
                throw new Exception($"ERP PRO: Falta cliente Consumidor Final. Crea Código='{CODIGO_CONSUMIDOR_FINAL}'.");

            var estado = rd.IsDBNull(3) ? (byte)1 : rd.GetByte(3);
            if (estado == 0)
                throw new Exception($"ERP PRO: Cliente '{CODIGO_CONSUMIDOR_FINAL}' está INACTIVO (Estado=0). Actívalo.");
        }

        public int CrearDesdeDgii(string rnc, string nombre, string? nombreComercial, string usuario)
        {
            rnc = LimpiarDocumento(rnc) ?? "";
            if (string.IsNullOrWhiteSpace(rnc))
                throw new Exception("RNC inválido.");

            var nombreFinal = EscogerNombre(nombre, nombreComercial);
            if (string.IsNullOrWhiteSpace(nombreFinal))
                nombreFinal = "CLIENTE";

            using var cn = Db.GetOpenConnection();
            using var tx = cn.BeginTransaction(IsolationLevel.Serializable);

            try
            {
                var existeId = GetClienteIdPorRnc(cn, tx, rnc);
                if (existeId > 0)
                {
                    tx.Commit();
                    return existeId;
                }

                var codigoGenerado = CrearAutoTx(
                    cn, tx,
                    nombre: (nombreFinal.Length > 120 ? nombreFinal[..120] : nombreFinal),
                    rncCedula: rnc,
                    telefono: null,
                    email: null,
                    direccion: null,
                    creditoMaximo: null,
                    codDivisas: null,
                    codTerminoPagos: null,
                    codVendedor: null,
                    codAlmacen: null
                );

                using var cmd = new SqlCommand(@"
UPDATE dbo.Cliente
SET [Nombre] = @nom,
    [RNC_Cedula] = @rnc,
    [Tipo] = ISNULL([Tipo], 0),
    [Estado] = ISNULL([Estado], 1),
    [FechaCreacion] = ISNULL([FechaCreacion], GETDATE()),
    RazonSocialFiscal = @razon,
    NombreComercialFiscal = @nombreComercial,
    TipoIdentificacionFiscal = 1,
    ValidadoDGII = 1,
    FechaValidacionDGII = GETDATE(),
    EstadoRncDGII = 'ACTIVO',
    EsExtranjero = 0,
    EsContribuyente = 1
WHERE [Código] = @cod;

SELECT TOP(1) ClienteId
FROM dbo.Cliente
WHERE [Código] = @cod;", cn, tx);

                cmd.Parameters.Add("@nom", SqlDbType.NVarChar, 120).Value =
                    (object)(nombreFinal.Length > 120 ? nombreFinal[..120] : nombreFinal);
                cmd.Parameters.Add("@rnc", SqlDbType.VarChar, 20).Value = rnc;
                cmd.Parameters.Add("@razon", SqlDbType.VarChar, 200).Value = (object?)LimpiarTexto(nombre) ?? DBNull.Value;
                cmd.Parameters.Add("@nombreComercial", SqlDbType.VarChar, 200).Value = (object?)LimpiarTexto(nombreComercial) ?? DBNull.Value;
                cmd.Parameters.Add("@cod", SqlDbType.VarChar, 20).Value = codigoGenerado;

                var idObj = cmd.ExecuteScalar();
                var nuevoId = (idObj == null || idObj == DBNull.Value) ? 0 : Convert.ToInt32(idObj);

                if (nuevoId <= 0)
                    throw new Exception("Se creó el cliente pero no se pudo obtener ClienteId.");

                tx.Commit();
                return nuevoId;
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }

        private static int GetClienteIdPorRnc(SqlConnection cn, SqlTransaction tx, string rnc)
        {
            using var cmd = new SqlCommand(@"
SELECT TOP(1) ClienteId
FROM dbo.Cliente
WHERE [RNC_Cedula] = @rnc;", cn, tx);

            cmd.Parameters.Add("@rnc", SqlDbType.VarChar, 20).Value = rnc;

            var v = cmd.ExecuteScalar();
            if (v == null || v == DBNull.Value) return 0;
            return Convert.ToInt32(v);
        }

        private static string EscogerNombre(string? nombre, string? nombreComercial)
        {
            var nc = LimpiarTexto(nombreComercial);
            if (!string.IsNullOrWhiteSpace(nc)) return nc;

            var n = LimpiarTexto(nombre);
            return string.IsNullOrWhiteSpace(n) ? "" : n;
        }

        public Cliente? ObtenerPorCodigo(string codigo)
        {
            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
SELECT [Código], [Nombre], [RNC_Cedula], [Telefono], [Email], [Direccion],
       [Tipo], [Estado], [FechaCreacion],
       CreditoMaximo, CodDivisas, CodTerminoPagos, CodVendedor, CodAlmacen,
       ClienteId, DescuentoPctMax,
       RazonSocialFiscal, NombreComercialFiscal, TipoIdentificacionFiscal,
       MunicipioCodigo, ProvinciaCodigo, PaisCodigo, CorreoFiscal,
       EsContribuyente, TipoClienteFiscal, ValidadoDGII, FechaValidacionDGII,
       EstadoRncDGII, IdentificadorExtranjero, EsExtranjero
FROM dbo.Cliente
WHERE [Código]=@c;", cn);

            cmd.Parameters.Add("@c", SqlDbType.VarChar, 20).Value = codigo;

            using var rd = cmd.ExecuteReader();
            if (!rd.Read()) return null;

            return MapCliente(rd);
        }

        public string CrearAuto(
            string nombre, string? rncCedula = null, string? telefono = null, string? email = null, string? direccion = null,
            decimal? creditoMaximo = null, string? codDivisas = null, string? codTerminoPagos = null,
            string? codVendedor = null, string? codAlmacen = null)
        {
            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand("dbo.sp_Cliente_CrearAuto", cn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.Add("@Nombre", SqlDbType.NVarChar, 120).Value = nombre;
            cmd.Parameters.Add("@RNC_Cedula", SqlDbType.VarChar, 20).Value = (object?)rncCedula ?? DBNull.Value;
            cmd.Parameters.Add("@Telefono", SqlDbType.VarChar, 20).Value = (object?)telefono ?? DBNull.Value;
            cmd.Parameters.Add("@Email", SqlDbType.VarChar, 100).Value = (object?)email ?? DBNull.Value;
            cmd.Parameters.Add("@Direccion", SqlDbType.NVarChar, 200).Value = (object?)direccion ?? DBNull.Value;

            cmd.Parameters.Add("@CreditoMaximo", SqlDbType.Decimal).Value = (object?)creditoMaximo ?? DBNull.Value;
            cmd.Parameters["@CreditoMaximo"].Precision = 18;
            cmd.Parameters["@CreditoMaximo"].Scale = 2;

            cmd.Parameters.Add("@CodDivisas", SqlDbType.VarChar, 10).Value = (object?)codDivisas ?? DBNull.Value;
            cmd.Parameters.Add("@CodTerminoPagos", SqlDbType.VarChar, 20).Value = (object?)codTerminoPagos ?? DBNull.Value;
            cmd.Parameters.Add("@CodVendedor", SqlDbType.VarChar, 20).Value = (object?)codVendedor ?? DBNull.Value;
            cmd.Parameters.Add("@CodAlmacen", SqlDbType.VarChar, 20).Value = (object?)codAlmacen ?? DBNull.Value;

            var pOut = cmd.Parameters.Add("@CodigoGenerado", SqlDbType.VarChar, 20);
            pOut.Direction = ParameterDirection.Output;

            cmd.ExecuteNonQuery();
            return (string)pOut.Value;
        }

        private static string CrearAutoTx(
            SqlConnection cn, SqlTransaction tx,
            string nombre, string? rncCedula = null, string? telefono = null, string? email = null, string? direccion = null,
            decimal? creditoMaximo = null, string? codDivisas = null, string? codTerminoPagos = null,
            string? codVendedor = null, string? codAlmacen = null)
        {
            using var cmd = new SqlCommand("dbo.sp_Cliente_CrearAuto", cn, tx)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.Add("@Nombre", SqlDbType.NVarChar, 120).Value = nombre;
            cmd.Parameters.Add("@RNC_Cedula", SqlDbType.VarChar, 20).Value = (object?)rncCedula ?? DBNull.Value;
            cmd.Parameters.Add("@Telefono", SqlDbType.VarChar, 20).Value = (object?)telefono ?? DBNull.Value;
            cmd.Parameters.Add("@Email", SqlDbType.VarChar, 100).Value = (object?)email ?? DBNull.Value;
            cmd.Parameters.Add("@Direccion", SqlDbType.NVarChar, 200).Value = (object?)direccion ?? DBNull.Value;

            cmd.Parameters.Add("@CreditoMaximo", SqlDbType.Decimal).Value = (object?)creditoMaximo ?? DBNull.Value;
            cmd.Parameters["@CreditoMaximo"].Precision = 18;
            cmd.Parameters["@CreditoMaximo"].Scale = 2;

            cmd.Parameters.Add("@CodDivisas", SqlDbType.VarChar, 10).Value = (object?)codDivisas ?? DBNull.Value;
            cmd.Parameters.Add("@CodTerminoPagos", SqlDbType.VarChar, 20).Value = (object?)codTerminoPagos ?? DBNull.Value;
            cmd.Parameters.Add("@CodVendedor", SqlDbType.VarChar, 20).Value = (object?)codVendedor ?? DBNull.Value;
            cmd.Parameters.Add("@CodAlmacen", SqlDbType.VarChar, 20).Value = (object?)codAlmacen ?? DBNull.Value;

            var pOut = cmd.Parameters.Add("@CodigoGenerado", SqlDbType.VarChar, 20);
            pOut.Direction = ParameterDirection.Output;

            cmd.ExecuteNonQuery();
            return (string)pOut.Value;
        }

        public void Actualizar(Cliente c)
        {
            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
UPDATE dbo.Cliente
   SET [Nombre]=@n, [RNC_Cedula]=@r, [Telefono]=@t, [Email]=@e, [Direccion]=@d,
       [Tipo]=@ti, [Estado]=@es,
       CreditoMaximo=@cm, CodDivisas=@div, CodTerminoPagos=@ter, CodVendedor=@ven, CodAlmacen=@alm,
       DescuentoPctMax=@des,
       RazonSocialFiscal=@razon, NombreComercialFiscal=@nomCom,
       TipoIdentificacionFiscal=@tipoId, MunicipioCodigo=@mun, ProvinciaCodigo=@prov, PaisCodigo=@pais,
       CorreoFiscal=@correoFiscal, EsContribuyente=@contrib, TipoClienteFiscal=@tipoCliFiscal,
       ValidadoDGII=@validado, FechaValidacionDGII=@fechaVal, EstadoRncDGII=@estadoRnc,
       IdentificadorExtranjero=@idExt, EsExtranjero=@extranjero
 WHERE [Código]=@c;", cn);

            cmd.Parameters.Add("@n", SqlDbType.NVarChar, 120).Value = c.Nombre;
            cmd.Parameters.Add("@r", SqlDbType.VarChar, 20).Value = (object?)c.RncCedula ?? DBNull.Value;
            cmd.Parameters.Add("@t", SqlDbType.VarChar, 20).Value = (object?)c.Telefono ?? DBNull.Value;
            cmd.Parameters.Add("@e", SqlDbType.VarChar, 100).Value = (object?)c.Email ?? DBNull.Value;
            cmd.Parameters.Add("@d", SqlDbType.NVarChar, 200).Value = (object?)c.Direccion ?? DBNull.Value;
            cmd.Parameters.Add("@ti", SqlDbType.TinyInt).Value = c.Tipo;
            cmd.Parameters.Add("@es", SqlDbType.TinyInt).Value = c.Estado;

            var pCm = cmd.Parameters.Add("@cm", SqlDbType.Decimal);
            pCm.Precision = 18; pCm.Scale = 2;
            pCm.Value = (object?)c.CreditoMaximo ?? DBNull.Value;

            cmd.Parameters.Add("@div", SqlDbType.VarChar, 10).Value = (object?)c.CodDivisas ?? DBNull.Value;
            cmd.Parameters.Add("@ter", SqlDbType.VarChar, 20).Value = (object?)c.CodTerminoPagos ?? DBNull.Value;
            cmd.Parameters.Add("@ven", SqlDbType.VarChar, 20).Value = (object?)c.CodVendedor ?? DBNull.Value;
            cmd.Parameters.Add("@alm", SqlDbType.VarChar, 20).Value = (object?)c.CodAlmacen ?? DBNull.Value;

            var pDes = cmd.Parameters.Add("@des", SqlDbType.Decimal);
            pDes.Precision = 5; pDes.Scale = 2;
            pDes.Value = (object?)c.DescuentoPctMax ?? DBNull.Value;

            cmd.Parameters.Add("@razon", SqlDbType.VarChar, 200).Value = (object?)c.RazonSocialFiscal ?? DBNull.Value;
            cmd.Parameters.Add("@nomCom", SqlDbType.VarChar, 200).Value = (object?)c.NombreComercialFiscal ?? DBNull.Value;
            cmd.Parameters.Add("@tipoId", SqlDbType.TinyInt).Value = (object?)c.TipoIdentificacionFiscal ?? DBNull.Value;
            cmd.Parameters.Add("@mun", SqlDbType.VarChar, 20).Value = (object?)c.MunicipioCodigo ?? DBNull.Value;
            cmd.Parameters.Add("@prov", SqlDbType.VarChar, 20).Value = (object?)c.ProvinciaCodigo ?? DBNull.Value;
            cmd.Parameters.Add("@pais", SqlDbType.VarChar, 10).Value = (object?)c.PaisCodigo ?? DBNull.Value;
            cmd.Parameters.Add("@correoFiscal", SqlDbType.VarChar, 150).Value = (object?)c.CorreoFiscal ?? DBNull.Value;
            cmd.Parameters.Add("@contrib", SqlDbType.Bit).Value = c.EsContribuyente;
            cmd.Parameters.Add("@tipoCliFiscal", SqlDbType.TinyInt).Value = (object?)c.TipoClienteFiscal ?? DBNull.Value;
            cmd.Parameters.Add("@validado", SqlDbType.Bit).Value = c.ValidadoDGII;
            cmd.Parameters.Add("@fechaVal", SqlDbType.DateTime).Value = (object?)c.FechaValidacionDGII ?? DBNull.Value;
            cmd.Parameters.Add("@estadoRnc", SqlDbType.VarChar, 50).Value = (object?)c.EstadoRncDGII ?? DBNull.Value;
            cmd.Parameters.Add("@idExt", SqlDbType.VarChar, 50).Value = (object?)c.IdentificadorExtranjero ?? DBNull.Value;
            cmd.Parameters.Add("@extranjero", SqlDbType.Bit).Value = c.EsExtranjero;
            cmd.Parameters.Add("@c", SqlDbType.VarChar, 20).Value = c.Codigo;

            cmd.ExecuteNonQuery();
        }

        public void Eliminar(string codigo)
        {
            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"DELETE FROM dbo.Cliente WHERE [Código]=@c;", cn);
            cmd.Parameters.Add("@c", SqlDbType.VarChar, 20).Value = codigo;
            cmd.ExecuteNonQuery();
        }

        public List<ClienteDto> ListarCombo(int top = 200)
        {
            var list = new List<ClienteDto>();

            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
SELECT TOP(@top) [Código], [Nombre]
FROM dbo.Cliente
ORDER BY [Nombre];", cn);

            cmd.Parameters.Add("@top", SqlDbType.Int).Value = top;

            using var rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                list.Add(new ClienteDto
                {
                    Codigo = rd.GetString(0),
                    Nombre = rd.GetString(1)
                });
            }
            return list;
        }

        public DataTable ListarVentasPorCierre(long cierreId)
        {
            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
SELECT 
    v.VentaId,
    v.NoDocumento,
    v.Fecha,
    v.ClienteCodigo,
    v.MonedaCodigo,
    v.SubTotalMoneda,
    v.ItbisMoneda,
    v.TotalMoneda,
    v.Estado,
    v.MedioPagoId,
    mp.Nombre AS MedioPago,
    v.MontoPago
FROM dbo.CajaCierreCab c
INNER JOIN dbo.VentaCab v
    ON v.POS_CajaNumero = c.CajaNumero
   AND v.Fecha         >= c.FechaDesde
   AND v.Fecha         <= c.FechaHasta
LEFT JOIN dbo.MedioPago mp
    ON mp.MedioPagoId = v.MedioPagoId
WHERE c.CierreId = @CierreId
  AND v.Estado   = 'FACTURADA'
ORDER BY v.Fecha;", cn);

            cmd.Parameters.Add("@CierreId", SqlDbType.BigInt).Value = cierreId;

            using var da = new SqlDataAdapter(cmd);
            var dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        private static string? LimpiarTexto(string? s)
        {
            if (string.IsNullOrWhiteSpace(s)) return null;

            s = s.Replace("�", "");
            var normalized = s.Normalize(NormalizationForm.FormD);

            var sb = new StringBuilder(normalized.Length);
            foreach (var ch in normalized)
            {
                var uc = CharUnicodeInfo.GetUnicodeCategory(ch);
                if (uc != UnicodeCategory.NonSpacingMark) sb.Append(ch);
            }

            return sb.ToString().Normalize(NormalizationForm.FormC).Trim();
        }

        public ClienteDto? BuscarPorRnc(string rnc)
        {
            rnc = (rnc ?? "").Trim();
            if (string.IsNullOrWhiteSpace(rnc)) return null;

            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
SELECT TOP(1)
       c.ClienteId,
       c.[Código]      AS Codigo,
       c.[Nombre],
       c.[RNC_Cedula]
FROM dbo.Cliente c
WHERE c.[RNC_Cedula] = @v;", cn);

            cmd.Parameters.Add("@v", SqlDbType.VarChar, 20).Value = rnc;

            using var rd = cmd.ExecuteReader();
            if (!rd.Read()) return null;

            return new ClienteDto
            {
                ClienteId = rd.IsDBNull(0) ? 0 : rd.GetInt32(0),
                Codigo = rd.IsDBNull(1) ? "" : rd.GetString(1),
                Nombre = rd.IsDBNull(2) ? "" : rd.GetString(2),
                CodVendedor = null,
                RncCedula = rd.IsDBNull(3) ? null : rd.GetString(3),
            };
        }

        private static string? LimpiarDocumento(string? s)
        {
            if (string.IsNullOrWhiteSpace(s)) return null;

            s = s.Replace("�", "").Trim();
            var sb = new StringBuilder();

            foreach (var ch in s)
                if (char.IsDigit(ch)) sb.Append(ch);

            var doc = sb.ToString();
            return string.IsNullOrWhiteSpace(doc) ? null : doc;
        }

        private static Cliente MapCliente(SqlDataReader rd)
        {
            return new Cliente
            {
                Codigo = rd.IsDBNull(0) ? "" : Convert.ToString(rd.GetValue(0)) ?? "",
                Nombre = rd.IsDBNull(1) ? "" : Convert.ToString(rd.GetValue(1)) ?? "",
                RncCedula = rd.IsDBNull(2) ? null : Convert.ToString(rd.GetValue(2)),
                Telefono = rd.IsDBNull(3) ? null : Convert.ToString(rd.GetValue(3)),
                Email = rd.IsDBNull(4) ? null : Convert.ToString(rd.GetValue(4)),
                Direccion = rd.IsDBNull(5) ? null : Convert.ToString(rd.GetValue(5)),

                Tipo = rd.IsDBNull(6) ? (byte)0 : Convert.ToByte(rd.GetValue(6)),
                Estado = rd.IsDBNull(7) ? (byte)1 : Convert.ToByte(rd.GetValue(7)),
                FechaCreacion = rd.IsDBNull(8) ? (DateTime?)null : Convert.ToDateTime(rd.GetValue(8)),
                CreditoMaximo = rd.IsDBNull(9) ? (decimal?)null : Convert.ToDecimal(rd.GetValue(9)),
                CodDivisas = rd.IsDBNull(10) ? null : Convert.ToString(rd.GetValue(10)),
                CodTerminoPagos = rd.IsDBNull(11) ? null : Convert.ToString(rd.GetValue(11)),
                CodVendedor = rd.IsDBNull(12) ? null : Convert.ToString(rd.GetValue(12)),
                CodAlmacen = rd.IsDBNull(13) ? null : Convert.ToString(rd.GetValue(13)),
                ClienteId = rd.IsDBNull(14) ? 0 : Convert.ToInt32(rd.GetValue(14)),
                DescuentoPctMax = rd.IsDBNull(15) ? (decimal?)null : Convert.ToDecimal(rd.GetValue(15)),

                RazonSocialFiscal = rd.FieldCount > 16 && !rd.IsDBNull(16) ? Convert.ToString(rd.GetValue(16)) : null,
                NombreComercialFiscal = rd.FieldCount > 17 && !rd.IsDBNull(17) ? Convert.ToString(rd.GetValue(17)) : null,
                TipoIdentificacionFiscal = rd.FieldCount > 18 && !rd.IsDBNull(18)
    ? Convert.ToByte(rd.GetValue(18))
    : null,
                MunicipioCodigo = rd.FieldCount > 19 && !rd.IsDBNull(19) ? Convert.ToString(rd.GetValue(19)) : null,
                ProvinciaCodigo = rd.FieldCount > 20 && !rd.IsDBNull(20) ? Convert.ToString(rd.GetValue(20)) : null,
                PaisCodigo = rd.FieldCount > 21 && !rd.IsDBNull(21) ? Convert.ToString(rd.GetValue(21)) : null,
                CorreoFiscal = rd.FieldCount > 22 && !rd.IsDBNull(22) ? Convert.ToString(rd.GetValue(22)) : null,
                EsContribuyente = rd.FieldCount > 23 && !rd.IsDBNull(23) && Convert.ToBoolean(rd.GetValue(23)),
                TipoClienteFiscal = rd.FieldCount > 24 && !rd.IsDBNull(24) ? Convert.ToByte(rd.GetValue(24)) : null,
                ValidadoDGII = rd.FieldCount > 25 && !rd.IsDBNull(25) && Convert.ToBoolean(rd.GetValue(25)),
                FechaValidacionDGII = rd.FieldCount > 26 && !rd.IsDBNull(26) ? Convert.ToDateTime(rd.GetValue(26)) : null,
                EstadoRncDGII = rd.FieldCount > 27 && !rd.IsDBNull(27) ? Convert.ToString(rd.GetValue(27)) : null,
                IdentificadorExtranjero = rd.FieldCount > 28 && !rd.IsDBNull(28) ? Convert.ToString(rd.GetValue(28)) : null,
                EsExtranjero = rd.FieldCount > 29 && !rd.IsDBNull(29) && Convert.ToBoolean(rd.GetValue(29))
            };
        }
    }
}
