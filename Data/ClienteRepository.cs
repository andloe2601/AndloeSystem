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
        public List<Cliente> Listar(string? filtro = null, int top = 200)
        {
            var list = new List<Cliente>();
            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
SELECT TOP(@top)
    [Código], [Nombre], [RNC_Cedula], [Telefono], [Email], [Direccion],
    [Tipo], [Estado], [FechaCreacion],
    CreditoMaximo, CodDivisas, CodTerminoPagos, CodVendedor, CodAlmacen,
    ClienteId, DescuentoPctMax
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
                list.Add(new Cliente
                {
                    Codigo = rd.GetString(0),
                    Nombre = rd.GetString(1),
                    RncCedula = rd.IsDBNull(2) ? null : rd.GetString(2),
                    Telefono = rd.IsDBNull(3) ? null : rd.GetString(3),
                    Email = rd.IsDBNull(4) ? null : rd.GetString(4),
                    Direccion = rd.IsDBNull(5) ? null : rd.GetString(5),

                    Tipo = rd.IsDBNull(6) ? (byte)0 : rd.GetByte(6),
                    Estado = rd.IsDBNull(7) ? (byte)1 : rd.GetByte(7),
                    FechaCreacion = rd.IsDBNull(8) ? (DateTime?)null : rd.GetDateTime(8),

                    CreditoMaximo = rd.IsDBNull(9) ? (decimal?)null : rd.GetDecimal(9),
                    CodDivisas = rd.IsDBNull(10) ? null : rd.GetString(10),
                    CodTerminoPagos = rd.IsDBNull(11) ? null : rd.GetString(11),
                    CodVendedor = rd.IsDBNull(12) ? null : rd.GetString(12),
                    CodAlmacen = rd.IsDBNull(13) ? null : rd.GetString(13),

                    ClienteId = rd.IsDBNull(14) ? 0 : rd.GetInt32(14),
                    DescuentoPctMax = rd.IsDBNull(15) ? (decimal?)null : rd.GetDecimal(15)
                });
            }
            return list;
        }

        // ✅ MEJORADO: devuelve ClienteId también
        public ClienteDto? BuscarPorCodigoORnc(string valor)
        {
            if (string.IsNullOrWhiteSpace(valor)) return null;

            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
SELECT TOP(1)
       c.ClienteId,
       c.[Código]      AS Codigo,
       c.[Nombre],
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
                RncCedula = rd.IsDBNull(3) ? null : rd.GetString(3),
                Direccion = rd.IsDBNull(4) ? null : rd.GetString(4),
                Telefono = rd.IsDBNull(5) ? null : rd.GetString(5),
                Tipo = rd.IsDBNull(6) ? (byte)0 : rd.GetByte(6)
            };
        }

        // ✅ CREA CLIENTE desde DGII (solo datos necesarios) y devuelve ClienteId
        // - NO depende de DgiiRncEntry
        // - Limpia acentos y "�"
        // - Evita duplicar si ya existe RNC
        public int CrearDesdeDgii(string rnc, string nombre, string? nombreComercial, string usuario)
        {
            rnc = LimpiarDocumento(rnc) ?? "";
            if (string.IsNullOrWhiteSpace(rnc))
                throw new Exception("RNC inválido.");

            var nombreFinal = EscogerNombre(nombre, nombreComercial);
            if (string.IsNullOrWhiteSpace(nombreFinal))
                nombreFinal = "CLIENTE";

            // Serializable = evita que 2 usuarios creen el mismo cliente al mismo tiempo
            using var cn = Db.GetOpenConnection();
            using var tx = cn.BeginTransaction(IsolationLevel.Serializable);

            try
            {
                // 1) Si ya existe por RNC, devolverlo
                var existeId = GetClienteIdPorRnc(cn, tx, rnc);
                if (existeId > 0)
                {
                    tx.Commit();
                    return existeId;
                }

                // 2) Crear cliente con numerador (SP genera el siguiente Código)
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

                // 3) Asegurar valores mínimos y devolver ClienteId
                using var cmd = new SqlCommand(@"
UPDATE dbo.Cliente
SET [Nombre] = @nom,
    [RNC_Cedula] = @rnc,
    [Tipo] = ISNULL([Tipo], 0),
    [Estado] = ISNULL([Estado], 1),
    [FechaCreacion] = ISNULL([FechaCreacion], GETDATE())
WHERE [Código] = @cod;

SELECT TOP(1) ClienteId
FROM dbo.Cliente
WHERE [Código] = @cod;", cn, tx);

                cmd.Parameters.Add("@nom", SqlDbType.NVarChar, 120).Value =
                    (object)(nombreFinal.Length > 120 ? nombreFinal[..120] : nombreFinal);

                cmd.Parameters.Add("@rnc", SqlDbType.VarChar, 20).Value = rnc;
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
SELECT [Código], [Nombre], [RNC_Cedula], [Telefono], [Email], [Direccion], DescuentoPctMax,
       [Tipo], [Estado], [FechaCreacion],
       CreditoMaximo, CodDivisas, CodTerminoPagos, CodVendedor, CodAlmacen,
       ClienteId
FROM dbo.Cliente
WHERE [Código]=@c;", cn);

            cmd.Parameters.Add("@c", SqlDbType.VarChar, 20).Value = codigo;

            using var rd = cmd.ExecuteReader();
            if (!rd.Read()) return null;

            return new Cliente
            {
                Codigo = rd.GetString(0),
                Nombre = rd.GetString(1),
                RncCedula = rd.IsDBNull(2) ? null : rd.GetString(2),
                Telefono = rd.IsDBNull(3) ? null : rd.GetString(3),
                Email = rd.IsDBNull(4) ? null : rd.GetString(4),
                Direccion = rd.IsDBNull(5) ? null : rd.GetString(5),
                DescuentoPctMax = rd.IsDBNull(6) ? (decimal?)null : rd.GetDecimal(6),
                Tipo = rd.IsDBNull(7) ? (byte)0 : rd.GetByte(7),
                Estado = rd.IsDBNull(8) ? (byte)1 : rd.GetByte(8),
                FechaCreacion = rd.IsDBNull(9) ? (DateTime?)null : rd.GetDateTime(9),
                CreditoMaximo = rd.IsDBNull(10) ? (decimal?)null : rd.GetDecimal(10),
                CodDivisas = rd.IsDBNull(11) ? null : rd.GetString(11),
                CodTerminoPagos = rd.IsDBNull(12) ? null : rd.GetString(12),
                CodVendedor = rd.IsDBNull(13) ? null : rd.GetString(13),
                CodAlmacen = rd.IsDBNull(14) ? null : rd.GetString(14),
                ClienteId = rd.IsDBNull(15) ? 0 : rd.GetInt32(15)
            };
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
       DescuentoPctMax=@des
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

        // =========================
        // Limpieza (sin acentos / sin � / doc solo numeros)
        // =========================
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
    }
}
