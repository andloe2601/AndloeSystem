using Andloe.Entidad.Facturacion;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;

namespace Andloe.Data
{
    public class FacturaRepository
    {
        public const string TIPO_COT = "COT";
        public const string TIPO_PF = "PF";
        public const string TIPO_FAC = "FAC";

        public const string EST_BORRADOR = "BORRADOR";
        public const string EST_FINALIZADA = "FINALIZADA";
        public const string EST_ANULADA = "ANULADA";

        public const string PAGO_CONTADO = "CONTADO";
        public const string PAGO_CREDITO = "CREDITO";

        private const string CFG_EMPRESA_DEFECTO_ID = "EMPRESA_DEFECTO_ID";
        private const string CFG_SUCURSAL_DEFECTO_ID = "SUCURSAL_DEFECTO_ID";
        private const string CFG_ALMACEN_DEFECTO_ID = "ALMACEN_DEFECTO_ID";
        private const string CFG_FACTURA_SERIE = "FACTURA_SERIE";
        private const string CFG_STOCK_NEGATIVO = "STOCK_NEGATIVO";

        // =========================
        // Schema cache (por ejecución/tx)
        // =========================
        private sealed class SchemaCache
        {
            public readonly Dictionary<string, HashSet<string>> Cols = new(StringComparer.OrdinalIgnoreCase);
            public (string whereSql, bool hasEmpresa, bool hasSucursal)? ExistenciaWhere;
        }

        // ============================================================
        // ✅ SistemaConfig
        // ============================================================
        private static string? GetConfig(SqlConnection cn, SqlTransaction? tx, string clave)
        {
            using var cmd = new SqlCommand(@"
SELECT TOP(1) Valor
FROM dbo.SistemaConfig
WHERE Clave = @c;", cn, tx);

            // OJO: el tamaño del parámetro no cambia el tamaño real de la columna.
            cmd.Parameters.Add("@c", SqlDbType.VarChar, 200).Value = (clave ?? "").Trim();

            var obj = cmd.ExecuteScalar();
            var s = obj == null || obj == DBNull.Value ? null : Convert.ToString(obj);
            return string.IsNullOrWhiteSpace(s) ? null : s.Trim();
        }

        private static int? GetConfigInt(SqlConnection cn, SqlTransaction? tx, string clave)
        {
            var s = GetConfig(cn, tx, clave);
            if (string.IsNullOrWhiteSpace(s)) return null;
            return int.TryParse(s.Trim(), out var n) && n > 0 ? n : (int?)null;
        }

        private static bool GetConfigBool(SqlConnection cn, SqlTransaction? tx, string clave)
        {
            var s = GetConfig(cn, tx, clave);
            if (string.IsNullOrWhiteSpace(s)) return false;

            s = s.Trim().ToUpperInvariant();
            return s == "1" || s == "TRUE" || s == "SI" || s == "S" || s == "YES" || s == "Y";
        }

        private static (int empresaId, int? sucursalId, int almacenId) ResolveDefaults(SqlConnection cn, SqlTransaction tx)
        {
            var emp = GetConfigInt(cn, tx, CFG_EMPRESA_DEFECTO_ID);
            var suc = GetConfigInt(cn, tx, CFG_SUCURSAL_DEFECTO_ID);
            var alm = GetConfigInt(cn, tx, CFG_ALMACEN_DEFECTO_ID);

            if (!emp.HasValue) throw new InvalidOperationException($"Falta '{CFG_EMPRESA_DEFECTO_ID}' en SistemaConfig.");
            if (!alm.HasValue) throw new InvalidOperationException($"Falta '{CFG_ALMACEN_DEFECTO_ID}' en SistemaConfig.");

            return (emp.Value, suc, alm.Value);
        }

        private static string NormalizeTipo(string tipoDocumento)
        {
            if (string.IsNullOrWhiteSpace(tipoDocumento))
                throw new ArgumentException("tipoDocumento requerido.");

            tipoDocumento = tipoDocumento.Trim().ToUpperInvariant();

            if (tipoDocumento != TIPO_COT && tipoDocumento != TIPO_PF && tipoDocumento != TIPO_FAC)
                throw new ArgumentException("tipoDocumento inválido. Use COT/PF/FAC.");

            return tipoDocumento;
        }

        private static string ResolveNumeradorCodigo(SqlConnection cn, SqlTransaction tx, string tipoDocumento)
        {
            tipoDocumento = (tipoDocumento ?? "").Trim().ToUpperInvariant();

            if (tipoDocumento == TIPO_COT) return "COT";
            if (tipoDocumento == TIPO_PF) return "PF";

            var serie = GetConfig(cn, tx, CFG_FACTURA_SERIE);
            if (string.IsNullOrWhiteSpace(serie))
                throw new InvalidOperationException($"Falta configuración '{CFG_FACTURA_SERIE}' en SistemaConfig (ej: VC).");

            return serie.Trim().ToUpperInvariant();
        }

        private static string GetNextNumerador(SqlConnection cn, SqlTransaction tx, string codigo, string prefFallback, int lenFallback)
        {
            using (var cmd = new SqlCommand(@"
IF NOT EXISTS(SELECT 1 FROM dbo.NumeradorSecuencia WHERE Codigo=@cod)
BEGIN
    INSERT INTO dbo.NumeradorSecuencia(Codigo,Prefijo,Longitud,Actual)
    VALUES(@cod,@pref,@len,0);
END", cn, tx))
            {
                cmd.Parameters.Add("@cod", SqlDbType.VarChar, 20).Value = codigo;
                cmd.Parameters.Add("@pref", SqlDbType.VarChar, 10).Value = prefFallback;
                cmd.Parameters.Add("@len", SqlDbType.Int).Value = lenFallback;
                cmd.ExecuteNonQuery();
            }

            using var cmd2 = new SqlCommand(@"
UPDATE dbo.NumeradorSecuencia WITH(UPDLOCK, HOLDLOCK)
SET Actual = Actual + 1
OUTPUT INSERTED.Prefijo, INSERTED.Longitud, INSERTED.Actual
WHERE Codigo=@cod;", cn, tx);

            cmd2.Parameters.Add("@cod", SqlDbType.VarChar, 20).Value = codigo;

            using var rd = cmd2.ExecuteReader();
            if (!rd.Read())
                throw new InvalidOperationException($"No se pudo generar numerador '{codigo}'.");

            var pref = rd.IsDBNull(0) ? prefFallback : rd.GetString(0);
            var lon = rd.IsDBNull(1) ? lenFallback : rd.GetInt32(1);
            var act = rd.GetInt32(2);

            if (string.IsNullOrWhiteSpace(pref)) pref = prefFallback;
            if (lon <= 0) lon = lenFallback;

            return pref + act.ToString().PadLeft(lon, '0');
        }

        // ============================================================
        // ✅ ANULACION: REVERSO INVENTARIO (FAC)
        // ============================================================
        // ============================================================
        // ✅ ANULAR (solo FAC FINALIZADA) + REVERSO INVENTARIO
        // ============================================================
        public void AnularFactura(int facturaId, string usuarioAnula, string motivo)
        {
            if (facturaId <= 0) throw new ArgumentException("facturaId inválido.", nameof(facturaId));

            using var cn = Db.GetOpenConnection();
            using var tx = cn.BeginTransaction(IsolationLevel.Serializable);
            var cache = new SchemaCache();

            try
            {
                var cab = GetCabeceraMin(facturaId, cn, tx);
                if (cab == null) throw new InvalidOperationException("Factura no existe.");

                if (!string.Equals(cab.TipoDocumento, TIPO_FAC, StringComparison.OrdinalIgnoreCase))
                    throw new InvalidOperationException("Solo se puede anular un documento tipo FACTURA (FAC).");

                if (!string.Equals(cab.Estado, EST_FINALIZADA, StringComparison.OrdinalIgnoreCase))
                    throw new InvalidOperationException("Solo se puede anular una factura en estado FINALIZADA.");

                // 1) devolver stock (entrada) => +Cantidad
                RevertirStockPorFactura(facturaId, cn, tx, cache);

                // 2) movimiento inventario por anulación (entrada) => +Cantidad
                CrearInvMovimientoPorAnulacionFactura(facturaId, usuarioAnula, cn, tx, cache);

                // 3) estado ANULADA + observacion
                var usr = string.IsNullOrWhiteSpace(usuarioAnula) ? "SYSTEM" : usuarioAnula.Trim();
                var mot = (motivo ?? "").Trim();

                using (var cmd = new SqlCommand(@"
UPDATE dbo.FacturaCab
SET Estado = @est,
    Observacion = CASE
        WHEN Observacion IS NULL OR LTRIM(RTRIM(Observacion)) = '' THEN @obs
        ELSE Observacion + CHAR(10) + @obs
    END
WHERE FacturaId = @id;", cn, tx))
                {
                    cmd.Parameters.Add("@id", SqlDbType.Int).Value = facturaId;
                    cmd.Parameters.Add("@est", SqlDbType.VarChar, 15).Value = EST_ANULADA;

                    var obs = $"ANULADA por {usr} el {DateTime.Now:yyyy-MM-dd HH:mm}. Motivo: {mot}";
                    cmd.Parameters.Add("@obs", SqlDbType.NVarChar, 400).Value = obs;

                    cmd.ExecuteNonQuery();
                }

                tx.Commit();
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }


        private static void RevertirStockPorFactura(int facturaId, SqlConnection cn, SqlTransaction tx, SchemaCache cache)
        {
            var items = LeerItemsFacturaAgrupados(facturaId, cn, tx);
            if (items.Count == 0) return;

            var ctx = ObtenerContextoFactura(facturaId, cn, tx, cache);

            foreach (var it in items)
            {
                var cod = (it.ProductoCodigo ?? "").Trim();
                if (string.IsNullOrWhiteSpace(cod)) continue;
                if (it.Cantidad <= 0m) continue;

                // ✅ ANULACION = ENTRADA => +Cantidad
                ActualizarExistencia(ctx, cod, +Math.Abs(it.Cantidad), cn, tx, cache);
            }
        }


        private static void CrearInvMovimientoPorAnulacionFactura(int facturaId, string? usuario, SqlConnection cn, SqlTransaction tx, SchemaCache cache)
        {
            var usr = string.IsNullOrWhiteSpace(usuario) ? "SYSTEM" : usuario.Trim();

            var items = LeerItemsFacturaAgrupados(facturaId, cn, tx);
            if (items.Count == 0) return;

            var ctx = ObtenerContextoFactura(facturaId, cn, tx, cache);
            var almacenId = ctx.almacenId;

            var invMovId = InsertInvMovimientoCab_AnulacionFactura(facturaId, usr, almacenId, cn, tx, cache);

            // ✅ aquí sí pasamos el tipo para que el Lin calcule el signo bien
            var items = LeerLineasFacturaDetParaInvMov(facturaId, cn, tx, cache);
            InsertInvMovimientoLin(invMovId, facturaId, usr, items, "ANULACION_VENTA", cn, tx, cache);


        }
        

        private static long InsertInvMovimientoCab_AnulacionFactura(int facturaId, string usuario, int almacenId, SqlConnection cn, SqlTransaction tx, SchemaCache cache)
        {
            var cols = GetColumns(cn, tx, "dbo.InvMovimientoCab", cache);

            var fechaCol = Pick(cols, "Fecha");
            var tipoCol = Pick(cols, "Tipo");
            var usuarioCol = Pick(cols, "Usuario");
            var estadoCol = Pick(cols, "Estado");

            if (fechaCol == null || tipoCol == null || usuarioCol == null || estadoCol == null)
                throw new InvalidOperationException("InvMovimientoCab debe tener columnas: Fecha, Tipo, Usuario, Estado.");

            var origenCol = Pick(cols, "Origen");
            var origenIdCol = Pick(cols, "OrigenId");
            var obsCol = Pick(cols, "Observacion");

            // ✅ NUEVO: almacenes
            var almOriCol = Pick(cols, "AlmacenIdOrigen");
            var almDesCol = Pick(cols, "AlmacenIdDestino");

            var fechaCreCol = Pick(cols, "FechaCreacion");
            var usrCreCol = Pick(cols, "UsuarioCreacion");

            var fecha = DateTime.Now;
            var tipo = "ANULACION_VENTA";
            var estado = "CONFIRMADO";
            var origen = "FACTURA_ANULACION";
            var obs = $"Entrada por anulación factura #{facturaId}";

            var colList = new List<string>();
            var valList = new List<string>();

            colList.Add(SqlQuote(fechaCol)); valList.Add("@Fecha");
            colList.Add(SqlQuote(tipoCol)); valList.Add("@Tipo");
            colList.Add(SqlQuote(usuarioCol)); valList.Add("@Usuario");
            colList.Add(SqlQuote(estadoCol)); valList.Add("@Estado");

            if (origenCol != null) { colList.Add(SqlQuote(origenCol)); valList.Add("@Origen"); }
            if (origenIdCol != null) { colList.Add(SqlQuote(origenIdCol)); valList.Add("@OrigenId"); }
            if (obsCol != null) { colList.Add(SqlQuote(obsCol)); valList.Add("@Obs"); }

            // ✅ llenar ambos almacenes SI existen en la tabla
            if (almOriCol != null) { colList.Add(SqlQuote(almOriCol)); valList.Add("@AlmOri"); }
            if (almDesCol != null) { colList.Add(SqlQuote(almDesCol)); valList.Add("@AlmDes"); }

            if (fechaCreCol != null) { colList.Add(SqlQuote(fechaCreCol)); valList.Add("@FechaCre"); }
            if (usrCreCol != null) { colList.Add(SqlQuote(usrCreCol)); valList.Add("@UsrCre"); }

            var sql = $@"
INSERT INTO dbo.InvMovimientoCab
({string.Join(",", colList)})
OUTPUT INSERTED.InvMovId
VALUES
({string.Join(",", valList)});";

            using var cmd = new SqlCommand(sql, cn, tx);

            cmd.Parameters.Add("@Fecha", SqlDbType.DateTime2).Value = fecha;
            cmd.Parameters.Add("@Tipo", SqlDbType.VarChar, 20).Value = tipo;
            cmd.Parameters.Add("@Usuario", SqlDbType.NVarChar, 100).Value = usuario;
            cmd.Parameters.Add("@Estado", SqlDbType.VarChar, 12).Value = estado;

            if (origenCol != null) cmd.Parameters.Add("@Origen", SqlDbType.VarChar, 20).Value = origen;
            if (origenIdCol != null) cmd.Parameters.Add("@OrigenId", SqlDbType.BigInt).Value = (long)facturaId;
            if (obsCol != null) cmd.Parameters.Add("@Obs", SqlDbType.NVarChar, 200).Value = obs;

            // ✅ mismo almacén en ambos (por diseño de reverso)
            if (almOriCol != null) cmd.Parameters.Add("@AlmOri", SqlDbType.Int).Value = almacenId;
            if (almDesCol != null) cmd.Parameters.Add("@AlmDes", SqlDbType.Int).Value = almacenId;

            if (fechaCreCol != null) cmd.Parameters.Add("@FechaCre", SqlDbType.DateTime).Value = DateTime.Now;
            if (usrCreCol != null) cmd.Parameters.Add("@UsrCre", SqlDbType.VarChar, 30).Value =
                usuario.Length > 30 ? usuario.Substring(0, 30) : usuario;

            var id = Convert.ToInt64(cmd.ExecuteScalar() ?? 0L);
            if (id <= 0) throw new InvalidOperationException("No se pudo generar InvMovId (anulación).");
            return id;
        }




        // ============================================================
        // ✅ HISTORIAL
        // ============================================================
        public List<FacturaHistorialDto> ListarHistorial(
    DateTime? desde,
    DateTime? hasta,
    string? tipoDocumento,
    string? estado,
    string? texto)
        {
            var list = new List<FacturaHistorialDto>();

            using var cn = Db.GetOpenConnection();

            var sql = @"
SELECT
    FacturaId,
    TipoDocumento,
    NumeroDocumento,
    FechaDocumento,
    Estado,
    TipoPago,
    NombreCliente,
    DocumentoCliente,
    ISNULL(TotalGeneral,0) AS TotalGeneral
FROM dbo.FacturaCab
WHERE 1=1
";

            if (desde.HasValue) sql += " AND FechaDocumento >= @desde";
            if (hasta.HasValue) sql += " AND FechaDocumento < DATEADD(day,1,@hasta)";

            if (!string.IsNullOrWhiteSpace(tipoDocumento))
                sql += " AND TipoDocumento = @tipo";

            if (!string.IsNullOrWhiteSpace(estado))
                sql += " AND Estado = @est";

            if (!string.IsNullOrWhiteSpace(texto))
            {
                sql += @"
 AND (
        NumeroDocumento LIKE '%' + @txt + '%'
     OR NombreCliente  LIKE '%' + @txt + '%'
     OR DocumentoCliente LIKE '%' + @txt + '%'
     OR eNCF LIKE '%' + @txt + '%'
 )
";
            }

            sql += " ORDER BY FacturaId DESC;";

            using var cmd = new SqlCommand(sql, cn);

            if (desde.HasValue) cmd.Parameters.Add("@desde", SqlDbType.Date).Value = desde.Value.Date;
            if (hasta.HasValue) cmd.Parameters.Add("@hasta", SqlDbType.Date).Value = hasta.Value.Date;

            if (!string.IsNullOrWhiteSpace(tipoDocumento))
                cmd.Parameters.Add("@tipo", SqlDbType.VarChar, 10).Value = tipoDocumento.Trim().ToUpperInvariant();

            if (!string.IsNullOrWhiteSpace(estado))
                cmd.Parameters.Add("@est", SqlDbType.VarChar, 15).Value = estado.Trim().ToUpperInvariant();

            if (!string.IsNullOrWhiteSpace(texto))
                cmd.Parameters.Add("@txt", SqlDbType.NVarChar, 100).Value = texto.Trim();

            using var rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                list.Add(new FacturaHistorialDto
                {
                    FacturaId = rd.GetInt32(0),
                    TipoDocumento = rd.IsDBNull(1) ? "" : rd.GetString(1),
                    NumeroDocumento = rd.IsDBNull(2) ? null : rd.GetString(2),
                    FechaDocumento = rd.IsDBNull(3) ? DateTime.MinValue : rd.GetDateTime(3),
                    Estado = rd.IsDBNull(4) ? "" : rd.GetString(4),
                    TipoPago = rd.IsDBNull(5) ? "" : rd.GetString(5),
                    NombreCliente = rd.IsDBNull(6) ? "" : rd.GetString(6),
                    DocumentoCliente = rd.IsDBNull(7) ? null : rd.GetString(7),
                    TotalGeneral = rd.IsDBNull(8) ? 0m : rd.GetDecimal(8)
                });
            }

            return list;
        }

        // ============================================================
        // ✅ CONVERTIR COT/PF -> FAC
        // ============================================================
        public string ConvertirCOTPFaFAC(int facturaId, string usuarioFinaliza)
        {
            if (facturaId <= 0) throw new ArgumentException("facturaId inválido.");

            using var cn = Db.GetOpenConnection();
            using var tx = cn.BeginTransaction(IsolationLevel.Serializable);

            try
            {
                CabMin cab;

                using (var cmd = new SqlCommand(@"
SELECT FacturaId, TipoDocumento, NumeroDocumento, Estado, ISNULL(TotalGeneral,0)
FROM dbo.FacturaCab
WHERE FacturaId=@id;", cn, tx))
                {
                    cmd.Parameters.Add("@id", SqlDbType.Int).Value = facturaId;

                    using var rd = cmd.ExecuteReader();
                    if (!rd.Read())
                        throw new InvalidOperationException("Documento no existe.");

                    cab = new CabMin
                    {
                        FacturaId = SafeGetInt(rd, 0),
                        TipoDocumento = SafeGetString(rd, 1, ""),
                        NumeroDocumento = rd.IsDBNull(2) ? null : rd.GetString(2),
                        Estado = SafeGetString(rd, 3, ""),
                        TotalGeneral = SafeGetDecimal(rd, 4, 0m)
                    };
                }

                var tipo = (cab.TipoDocumento ?? "").Trim().ToUpperInvariant();
                if (tipo != TIPO_COT && tipo != TIPO_PF)
                    throw new InvalidOperationException("Solo se puede convertir COT o PF a FAC.");

                // (opcional) si quieres obligar que solo se convierta en BORRADOR:
                // if (!string.Equals(cab.Estado, EST_BORRADOR, StringComparison.OrdinalIgnoreCase))
                //     throw new InvalidOperationException("Solo se puede convertir un documento en estado BORRADOR.");

                var numeroOrigen = (cab.NumeroDocumento ?? "").Trim();
                if (string.IsNullOrWhiteSpace(numeroOrigen))
                    throw new InvalidOperationException("El documento origen no tiene NumeroDocumento, no se puede referenciar.");

                // Recalcular antes de numerar FAC
                RecalcularTotales(facturaId, cn, tx);

                // Nuevo numero FAC
                var codigoNumerador = ResolveNumeradorCodigo(cn, tx, TIPO_FAC);
                var nuevoNumero = GetNextNumerador(cn, tx, codigoNumerador, prefFallback: "V", lenFallback: 6);

                // Solo si existe la columna
                var hasRef = HasColumn(cn, tx, "dbo.FacturaCab", "NumeroDocumentoRef");

                var sqlUpdate = @"
UPDATE dbo.FacturaCab
SET TipoDocumento   = @tipo,
    NumeroDocumento = @num" + (hasRef ? ", NumeroDocumentoRef = @ref" : "") + @"
WHERE FacturaId = @id;";

                using (var cmdUp = new SqlCommand(sqlUpdate, cn, tx))
                {
                    cmdUp.Parameters.Add("@id", SqlDbType.Int).Value = facturaId;
                    cmdUp.Parameters.Add("@tipo", SqlDbType.VarChar, 10).Value = TIPO_FAC;
                    cmdUp.Parameters.Add("@num", SqlDbType.VarChar, 20).Value = nuevoNumero;

                    if (hasRef)
                        cmdUp.Parameters.Add("@ref", SqlDbType.VarChar, 20).Value = numeroOrigen;

                    cmdUp.ExecuteNonQuery();
                }

                tx.Commit();
                return nuevoNumero;
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }



        // ============================================================
        // ✅ BLOQUEO NEGATIVO POR PRODUCTO (FIX REAL: clave corta <= 50)
        // ============================================================

        // Parte de producto para clave: si es larga, usa 4 chars + hash8 (total 12)
        private static string BuildProdPartForKey(string productoCodigo)
        {
            productoCodigo = (productoCodigo ?? "").Trim().ToUpperInvariant();
            if (productoCodigo.Length <= 12) return productoCodigo;

            // hash8 estable
            var bytes = Encoding.UTF8.GetBytes(productoCodigo);
            using var sha1 = SHA1.Create();
            var h = sha1.ComputeHash(bytes);
            var hash8 = BitConverter.ToString(h).Replace("-", "").Substring(0, 8); // 8 hex

            return productoCodigo.Substring(0, 4) + hash8; // 12 chars
        }

        private static string BuildKeyBloqNegProdShort(int empresaId, int? sucursalId, int almacenId, string productoCodigo)
        {
            // Garantiza <= 50 (con emp/suc/alm de hasta 10 dígitos y prodPart 12)
            var suc = (sucursalId.HasValue && sucursalId.Value > 0) ? sucursalId.Value : 0;
            var prodPart = BuildProdPartForKey(productoCodigo);

            // Ej: SNBP_1_0_5_ABCD12EF34AA
            return $"SNBP_{empresaId}_{suc}_{almacenId}_{prodPart}";
        }

        private static string BuildKeyBloqNegProdLong(int empresaId, int? sucursalId, int almacenId, string productoCodigo)
        {
            // Formato viejo (si tu columna Clave ya es > 50 y existe data)
            var suc = (sucursalId.HasValue && sucursalId.Value > 0) ? sucursalId.Value : 0;
            productoCodigo = (productoCodigo ?? "").Trim();

            return
                "STOCK_NEGATIVO_BLOQ_PROD_EMP_" + empresaId +
                "_SUC_" + suc +
                "_ALM_" + almacenId +
                "_" + productoCodigo;
        }

        private static bool BloqueaNegativoPorProducto(int empresaId, int? sucursalId, int almacenId, string productoCodigo, SqlConnection cn, SqlTransaction tx)
        {
            productoCodigo = (productoCodigo ?? "").Trim();
            if (string.IsNullOrWhiteSpace(productoCodigo)) return false;

            // 1) clave corta (la correcta)
            var kShort = BuildKeyBloqNegProdShort(empresaId, sucursalId, almacenId, productoCodigo);
            if (GetConfigBool(cn, tx, kShort)) return true;

            // 2) fallback SUC=0 corto (por si la factura no guarda sucursal o config está genérico)
            var kShort0 = BuildKeyBloqNegProdShort(empresaId, 0, almacenId, productoCodigo);
            if (GetConfigBool(cn, tx, kShort0)) return true;

            // 3) compat: clave larga (si tu DB permite guardar >50)
            var kLong = BuildKeyBloqNegProdLong(empresaId, sucursalId, almacenId, productoCodigo);
            if (GetConfigBool(cn, tx, kLong)) return true;

            var kLong0 = BuildKeyBloqNegProdLong(empresaId, 0, almacenId, productoCodigo);
            return GetConfigBool(cn, tx, kLong0);
        }

        // ============================================================
        // ✅ Helpers columnas
        // ============================================================
        private static HashSet<string> GetColumns(SqlConnection cn, SqlTransaction tx, string tableFullName, SchemaCache cache)
        {
            if (cache.Cols.TryGetValue(tableFullName, out var existing))
                return existing;

            var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            using var cmd = new SqlCommand(@"
SELECT c.name
FROM sys.columns c
WHERE c.object_id = OBJECT_ID(@t);", cn, tx);

            cmd.Parameters.Add("@t", SqlDbType.NVarChar, 200).Value = tableFullName;

            using var rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                var n = rd.IsDBNull(0) ? "" : rd.GetString(0);
                if (!string.IsNullOrWhiteSpace(n)) set.Add(n);
            }

            cache.Cols[tableFullName] = set;
            return set;
        }

        private static string? Pick(HashSet<string> cols, params string[] candidates)
        {
            foreach (var c in candidates)
                if (cols.Contains(c)) return c;
            return null;
        }

        private static string SafeGetString(SqlDataReader rd, int i, string def = "")
    => rd.IsDBNull(i) ? def : rd.GetString(i);

        private static int SafeGetInt(SqlDataReader rd, int i, int def = 0)
            => rd.IsDBNull(i) ? def : Convert.ToInt32(rd.GetValue(i));

        private static int? SafeGetIntN(SqlDataReader rd, int i)
            => rd.IsDBNull(i) ? (int?)null : Convert.ToInt32(rd.GetValue(i));

        private static decimal SafeGetDecimal(SqlDataReader rd, int i, decimal def = 0m)
            => rd.IsDBNull(i) ? def : Convert.ToDecimal(rd.GetValue(i));

        private static DateTime SafeGetDateTime(SqlDataReader rd, int i, DateTime? def = null)
            => rd.IsDBNull(i) ? (def ?? DateTime.MinValue) : Convert.ToDateTime(rd.GetValue(i));

        private static DateTime? SafeGetDateTimeN(SqlDataReader rd, int i)
            => rd.IsDBNull(i) ? (DateTime?)null : Convert.ToDateTime(rd.GetValue(i));

        private static string SqlQuote(string name) => "[" + name.Replace("]", "]]") + "]";

        private static decimal ObtenerCostoPromedio(FacturaCtx ctx, string productoCodigo, SqlConnection cn, SqlTransaction tx, SchemaCache cache)
        {
            var w = BuildExistenciaWhere(cn, tx, cache);

            // OJO: Existencia tiene CostoPromedio en tu código
            using var cmd = new SqlCommand($@"
SELECT TOP(1) ISNULL(CostoPromedio, 0)
FROM dbo.Existencia
WHERE {w.whereSql};", cn, tx);

            cmd.Parameters.Add("@alm", SqlDbType.Int).Value = ctx.almacenId;
            cmd.Parameters.Add("@prod", SqlDbType.VarChar, 20).Value = productoCodigo;

            if (w.hasEmpresa) cmd.Parameters.Add("@emp", SqlDbType.Int).Value = ctx.empresaId;
            if (w.hasSucursal) cmd.Parameters.Add("@suc", SqlDbType.Int).Value = (object?)ctx.sucursalId ?? DBNull.Value;

            var v = cmd.ExecuteScalar();
            var costo = (v == null || v == DBNull.Value) ? 0m : Convert.ToDecimal(v);

            // redondeo a 6 decimales porque tu columna es decimal(18,6)
            return Math.Round(costo, 6, MidpointRounding.AwayFromZero);
        }

        private static (int? categoriaId, int? subcategoriaId, decimal? costoFallback) ObtenerDatosProducto(string productoCodigo, SqlConnection cn, SqlTransaction tx)
        {
            // ¿Existe la columna Estado? (sin asumir)
            bool hasEstado;
            using (var cmdC = new SqlCommand(@"
SELECT COUNT(1)
FROM sys.columns
WHERE object_id = OBJECT_ID('dbo.Producto')
  AND name = 'Estado';", cn, tx))
            {
                hasEstado = Convert.ToInt32(cmdC.ExecuteScalar() ?? 0) > 0;
            }

            var sql = @"
SELECT TOP(1)
    CategoriaId,
    SubcategoriaId,

    -- costo fallback (coalesce de columnas nuevas y NAV)
    COALESCE(
        NULLIF(CAST(PrecioCompraPromedio AS decimal(18,6)), 0),
        NULLIF(CAST(PrecioCoste        AS decimal(18,6)), 0),
        NULLIF(CAST([Precio compra promedio] AS decimal(18,6)), 0),
        NULLIF(CAST([Precio coste]         AS decimal(18,6)), 0),
        NULLIF(CAST([Ultimo precio compra] AS decimal(18,6)), 0)
    ) AS CostoFallback
FROM dbo.Producto
WHERE [Nº] = @cod"
        + (hasEstado ? " AND Estado = 1" : "") + @"
ORDER BY 
    ISNULL([Fecha últ_ modificación], '19000101') DESC,
    ISNULL(FechaCreacion, '19000101') DESC;";

            using var cmd = new SqlCommand(sql, cn, tx);
            cmd.Parameters.Add("@cod", SqlDbType.VarChar, 20).Value = (productoCodigo ?? "").Trim();

            using var rd = cmd.ExecuteReader();
            if (!rd.Read())
                return (null, null, null);

            int? cat = rd.IsDBNull(0) ? (int?)null : Convert.ToInt32(rd.GetValue(0));
            int? sub = rd.IsDBNull(1) ? (int?)null : Convert.ToInt32(rd.GetValue(1));
            decimal? costo = rd.IsDBNull(2) ? (decimal?)null : Convert.ToDecimal(rd.GetValue(2));

            if (costo.HasValue)
                costo = Math.Round(costo.Value, 6, MidpointRounding.AwayFromZero);

            return (cat, sub, costo);
        }


        // ============================================================
        // ✅ Contexto de factura (Empresa/Sucursal/Almacen)
        // ============================================================
        private static (int empresaId, int? sucursalId, int almacenId) GetFacturaContextOrDefault(int facturaId, SqlConnection cn, SqlTransaction tx, SchemaCache cache)
        {
            var cols = GetColumns(cn, tx, "dbo.FacturaCab", cache);

            string? colEmp = Pick(cols, "EmpresaId");
            string? colSuc = Pick(cols, "SucursalId");
            string? colAlm = Pick(cols, "AlmacenId", "AlmacenIdOrigen", "AlmacenOrigenId");

            int? emp = null;
            int? suc = null;
            int? alm = null;

            var selectCols = new List<string>();
            if (colEmp != null) selectCols.Add($"{SqlQuote(colEmp)} AS EmpresaId");
            if (colSuc != null) selectCols.Add($"{SqlQuote(colSuc)} AS SucursalId");
            if (colAlm != null) selectCols.Add($"{SqlQuote(colAlm)} AS AlmacenId");

            if (selectCols.Count > 0)
            {
                using var cmd = new SqlCommand($@"
SELECT TOP(1) {string.Join(",", selectCols)}
FROM dbo.FacturaCab
WHERE FacturaId=@id;", cn, tx);

                cmd.Parameters.Add("@id", SqlDbType.Int).Value = facturaId;

                using var rd = cmd.ExecuteReader();
                if (rd.Read())
                {
                    if (colEmp != null && !rd.IsDBNull(rd.GetOrdinal("EmpresaId")))
                        emp = Convert.ToInt32(rd["EmpresaId"]);

                    if (colSuc != null && !rd.IsDBNull(rd.GetOrdinal("SucursalId")))
                        suc = Convert.ToInt32(rd["SucursalId"]);

                    if (colAlm != null && !rd.IsDBNull(rd.GetOrdinal("AlmacenId")))
                        alm = Convert.ToInt32(rd["AlmacenId"]);
                }
            }

            var defs = ResolveDefaults(cn, tx);

            var empresaId = (emp.HasValue && emp.Value > 0) ? emp.Value : defs.empresaId;
            int? sucursalId = (suc.HasValue && suc.Value > 0) ? suc.Value : defs.sucursalId;
            var almacenId = (alm.HasValue && alm.Value > 0) ? alm.Value : defs.almacenId;

            return (empresaId, sucursalId, almacenId);
        }

        private static bool PermiteStockNegativo(int empresaId, int? sucursalId, int almacenId, SqlConnection cn, SqlTransaction tx)
        {
            if (sucursalId.HasValue)
            {
                var k1 = $"{CFG_STOCK_NEGATIVO}_EMP_{empresaId}_SUC_{sucursalId.Value}_ALM_{almacenId}";
                if (GetConfigBool(cn, tx, k1)) return true;
            }

            var k2 = $"{CFG_STOCK_NEGATIVO}_EMP_{empresaId}_ALM_{almacenId}";
            if (GetConfigBool(cn, tx, k2)) return true;

            var k3 = $"{CFG_STOCK_NEGATIVO}_ALM_{almacenId}";
            if (GetConfigBool(cn, tx, k3)) return true;

            var k4 = $"{CFG_STOCK_NEGATIVO}_EMP_{empresaId}";
            if (GetConfigBool(cn, tx, k4)) return true;

            return GetConfigBool(cn, tx, CFG_STOCK_NEGATIVO);
        }

        // ============================================================
        // ✅ CREAR BORRADOR
        // ============================================================
        public int CrearBorrador(string tipoDocumento, string usuarioCreacion)
        {
            tipoDocumento = NormalizeTipo(tipoDocumento);
            var usr = string.IsNullOrWhiteSpace(usuarioCreacion) ? "SYSTEM" : usuarioCreacion.Trim();

            using var cn = Db.GetOpenConnection();
            using var tx = cn.BeginTransaction(IsolationLevel.Serializable);
            var cache = new SchemaCache();

            try
            {
                var (empresaId, sucursalId, almacenId) = ResolveDefaults(cn, tx);

                var codigoNumerador = ResolveNumeradorCodigo(cn, tx, tipoDocumento);
                var fallbackPref = codigoNumerador switch
                {
                    "COT" => "COT-",
                    "PF" => "PF-",
                    _ => "V"
                };

                var numero = GetNextNumerador(cn, tx, codigoNumerador, fallbackPref, 6);

                using var cmd = new SqlCommand(@"
INSERT INTO dbo.FacturaCab
(
    TipoDocumento, NumeroDocumento, FechaDocumento, FechaVencimiento,
    ClienteId, NombreCliente, DocumentoCliente,
    SubTotal, TotalDescuento, TotalImpuesto, TotalGeneral,
    TipoPago, TerminoPagoId, DiasCredito,
    Estado, Observacion,
    UsuarioCreacion, FechaCreacion,
    UsuarioFinaliza, FechaFinaliza,
    ClienteCodigo,
    AlmacenId, EmpresaId, SucursalId
)
OUTPUT INSERTED.FacturaId
VALUES
(
    @TipoDocumento, @NumeroDocumento, GETDATE(), NULL,
    NULL, @NombreCliente, NULL,
    0, 0, 0, 0,
    @TipoPago, NULL, NULL,
    @Estado, NULL,
    @UsuarioCreacion, GETDATE(),
    NULL, NULL,
    NULL,
    @AlmacenId, @EmpresaId, @SucursalId
);", cn, tx);

                cmd.Parameters.Add("@TipoDocumento", SqlDbType.VarChar, 10).Value = tipoDocumento;
                cmd.Parameters.Add("@NumeroDocumento", SqlDbType.VarChar, 20).Value = numero;
                cmd.Parameters.Add("@NombreCliente", SqlDbType.NVarChar, 300).Value = "CONSUMIDOR FINAL";
                cmd.Parameters.Add("@TipoPago", SqlDbType.VarChar, 10).Value = PAGO_CONTADO;
                cmd.Parameters.Add("@Estado", SqlDbType.VarChar, 15).Value = EST_BORRADOR;
                cmd.Parameters.Add("@UsuarioCreacion", SqlDbType.VarChar, 30).Value = usr;

                cmd.Parameters.Add("@EmpresaId", SqlDbType.Int).Value = empresaId;
                cmd.Parameters.Add("@SucursalId", SqlDbType.Int).Value = (object?)sucursalId ?? DBNull.Value;
                cmd.Parameters.Add("@AlmacenId", SqlDbType.Int).Value = almacenId;

                var facturaId = Convert.ToInt32(cmd.ExecuteScalar());
                tx.Commit();
                return facturaId;
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }

        public int CrearBorrador(string tipoDocumento, string usuarioCreacion, int empresaId, int? sucursalId, int almacenId)
        {
            tipoDocumento = NormalizeTipo(tipoDocumento);

            if (empresaId <= 0) throw new ArgumentException("empresaId inválido.");
            if (almacenId <= 0) throw new ArgumentException("almacenId inválido.");

            var usr = string.IsNullOrWhiteSpace(usuarioCreacion) ? "SYSTEM" : usuarioCreacion.Trim();

            using var cn = Db.GetOpenConnection();
            using var tx = cn.BeginTransaction(IsolationLevel.Serializable);

            try
            {
                var codigoNumerador = ResolveNumeradorCodigo(cn, tx, tipoDocumento);
                var fallbackPref = codigoNumerador switch
                {
                    "COT" => "COT-",
                    "PF" => "PF-",
                    _ => "V"
                };

                var numero = GetNextNumerador(cn, tx, codigoNumerador, fallbackPref, 6);

                using var cmd = new SqlCommand(@"
INSERT INTO dbo.FacturaCab
(
    TipoDocumento, NumeroDocumento, FechaDocumento, FechaVencimiento,
    ClienteId, NombreCliente, DocumentoCliente,
    SubTotal, TotalDescuento, TotalImpuesto, TotalGeneral,
    TipoPago, TerminoPagoId, DiasCredito,
    Estado, Observacion,
    UsuarioCreacion, FechaCreacion,
    UsuarioFinaliza, FechaFinaliza,
    ClienteCodigo,
    AlmacenId, EmpresaId, SucursalId
)
OUTPUT INSERTED.FacturaId
VALUES
(
    @TipoDocumento, @NumeroDocumento, GETDATE(), NULL,
    NULL, @NombreCliente, NULL,
    0, 0, 0, 0,
    @TipoPago, NULL, NULL,
    @Estado, NULL,
    @UsuarioCreacion, GETDATE(),
    NULL, NULL,
    NULL,
    @AlmacenId, @EmpresaId, @SucursalId
);", cn, tx);

                cmd.Parameters.Add("@TipoDocumento", SqlDbType.VarChar, 10).Value = tipoDocumento;
                cmd.Parameters.Add("@NumeroDocumento", SqlDbType.VarChar, 20).Value = numero;

                cmd.Parameters.Add("@NombreCliente", SqlDbType.NVarChar, 300).Value = "CONSUMIDOR FINAL";
                cmd.Parameters.Add("@TipoPago", SqlDbType.VarChar, 10).Value = PAGO_CONTADO;
                cmd.Parameters.Add("@Estado", SqlDbType.VarChar, 15).Value = EST_BORRADOR;
                cmd.Parameters.Add("@UsuarioCreacion", SqlDbType.VarChar, 30).Value = usr;

                cmd.Parameters.Add("@EmpresaId", SqlDbType.Int).Value = empresaId;
                cmd.Parameters.Add("@SucursalId", SqlDbType.Int).Value = (object?)sucursalId ?? DBNull.Value;
                cmd.Parameters.Add("@AlmacenId", SqlDbType.Int).Value = almacenId;

                var facturaId = Convert.ToInt32(cmd.ExecuteScalar());

                tx.Commit();
                return facturaId;
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }

        // ============================================================
        // ✅ setters borrador
        // ============================================================
        public void SetFechaDocumentoBorrador(int facturaId, DateTime fechaDocumento)
        {
            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
UPDATE dbo.FacturaCab
SET FechaDocumento = @f
WHERE FacturaId = @id
  AND Estado = 'BORRADOR';", cn);

            cmd.Parameters.Add("@id", SqlDbType.Int).Value = facturaId;
            cmd.Parameters.Add("@f", SqlDbType.DateTime).Value = fechaDocumento;

            cmd.ExecuteNonQuery();
        }

        public void SetTipoDocumentoBorrador(int facturaId, string tipoDocumento)
        {
            tipoDocumento = NormalizeTipo(tipoDocumento);

            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
UPDATE dbo.FacturaCab
SET TipoDocumento = @t
WHERE FacturaId = @id
  AND Estado = 'BORRADOR';", cn);

            cmd.Parameters.Add("@id", SqlDbType.Int).Value = facturaId;
            cmd.Parameters.Add("@t", SqlDbType.VarChar, 10).Value = tipoDocumento;
            cmd.ExecuteNonQuery();
        }

        public void SetCliente(int facturaId, int? clienteId, string nombreCliente, string? documentoCliente)
        {
            if (string.IsNullOrWhiteSpace(nombreCliente))
                nombreCliente = "CONSUMIDOR FINAL";

            using var cn = Db.GetOpenConnection();

            var sql = @"
UPDATE dbo.FacturaCab
SET ClienteId = @cli,
    NombreCliente = @nom,
    DocumentoCliente = @doc
WHERE FacturaId = @id;";

            using var cmd = new SqlCommand(sql, cn);

            cmd.Parameters.Add("@id", SqlDbType.Int).Value = facturaId;
            cmd.Parameters.Add("@cli", SqlDbType.Int).Value = (object?)clienteId ?? DBNull.Value;
            cmd.Parameters.Add("@nom", SqlDbType.NVarChar, 300).Value = nombreCliente.Trim();
            cmd.Parameters.Add("@doc", SqlDbType.NVarChar, 60).Value = (object?)documentoCliente ?? DBNull.Value;

            cmd.ExecuteNonQuery();
        }

        public void SetDireccionCliente(int facturaId, string? direccion)
        {
            if (facturaId <= 0) return;

            using var cn = Db.GetOpenConnection();

            // si no existe la columna, no hacemos nada (sin error)
            if (!HasColumn(cn, null, "dbo.FacturaCab", "DireccionCliente"))
                return;

            using var cmd = new SqlCommand(@"
UPDATE dbo.FacturaCab
SET DireccionCliente = @dir
WHERE FacturaId = @id;", cn);

            cmd.Parameters.Add("@id", SqlDbType.Int).Value = facturaId;
            cmd.Parameters.Add("@dir", SqlDbType.NVarChar, 400).Value =
                string.IsNullOrWhiteSpace(direccion) ? (object)DBNull.Value : direccion.Trim();

            cmd.ExecuteNonQuery();
        }

       

        // ============================================================
        // ✅ IMPUESTO / CODBARRA
        // ============================================================
        public (int? impuestoId, decimal impuestoPct) ObtenerImpuestoPorProducto(string productoCodigo, decimal fallbackPct)
        {
            if (string.IsNullOrWhiteSpace(productoCodigo))
                return (null, NormalizarPct(fallbackPct));

            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
SELECT TOP(1)
    i.ImpuestoId,
    i.Porcentaje
FROM dbo.Producto p
INNER JOIN dbo.Impuesto i ON i.ImpuestoId = p.ImpuestoId
WHERE p.[Nº] = @cod
  AND i.Estado = 1;", cn);

            cmd.Parameters.Add("@cod", SqlDbType.VarChar, 20).Value = productoCodigo.Trim();

            using var rd = cmd.ExecuteReader();
            if (!rd.Read())
                return (null, NormalizarPct(fallbackPct));

            var id = rd.IsDBNull(0) ? (int?)null : rd.GetInt32(0);
            var pct = rd.IsDBNull(1) ? fallbackPct : rd.GetDecimal(1);
            return (id, NormalizarPct(pct));
        }

        public string? ObtenerPrimerCodigoBarra(string productoCodigo)
        {
            if (string.IsNullOrWhiteSpace(productoCodigo)) return null;

            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
SELECT TOP(1) [Cód_ barras]
FROM dbo.CodBarras
WHERE [Nº producto] = @cod
ORDER BY [Cód_ barras];", cn);

            cmd.Parameters.Add("@cod", SqlDbType.VarChar, 20).Value = productoCodigo.Trim();

            var val = cmd.ExecuteScalar();
            var cb = val == null || val == DBNull.Value ? null : Convert.ToString(val);
            return string.IsNullOrWhiteSpace(cb) ? null : cb.Trim();
        }

        private sealed class InvMovItem
        {
            public string ProductoCodigo = "";
            public decimal Cantidad;
            public string? Unidad;
            public decimal PrecioUnitarioVenta;
            public decimal TotalLineaVenta;
            public string? Lote;
            public string? Serial;
        }

        private static List<InvMovItem> LeerLineasFacturaDetParaInvMov(int facturaId, SqlConnection cn, SqlTransaction tx, SchemaCache cache)
        {
            var cols = GetColumns(cn, tx, "dbo.FacturaDet", cache);

            var hasUnidad = cols.Contains("Unidad");
            var hasPrecio = cols.Contains("PrecioUnitario");
            var hasTotalLinea = cols.Contains("TotalLinea");

            // Lote/Serial: no asumo nombres, pruebo varios
            var colLote = Pick(cols, "Lote", "LoteCodigo", "LoteId");
            var colSerial = Pick(cols, "Serial", "Serie", "NumeroSerie", "NoSerie");

            // requeridos mínimos reales
            if (!cols.Contains("ProductoCodigo") || !cols.Contains("Cantidad"))
                throw new InvalidOperationException("FacturaDet debe tener ProductoCodigo y Cantidad para generar InvMovimientoLin.");

            // armar select dinámico
            var select = new List<string>
    {
        "ProductoCodigo",
        "Cantidad",
        (hasUnidad ? "Unidad" : "NULL AS Unidad"),
        (hasPrecio ? "PrecioUnitario" : "0 AS PrecioUnitario"),
        (hasTotalLinea ? "TotalLinea" : "0 AS TotalLinea"),
        (colLote != null ? SqlQuote(colLote) + " AS Lote" : "NULL AS Lote"),
        (colSerial != null ? SqlQuote(colSerial) + " AS Serial" : "NULL AS Serial")
    };

            using var cmd = new SqlCommand($@"
SELECT {string.Join(",", select)}
FROM dbo.FacturaDet
WHERE FacturaId = @id
ORDER BY FacturaDetId;", cn, tx);

            cmd.Parameters.Add("@id", SqlDbType.Int).Value = facturaId;

            var list = new List<InvMovItem>();

            using var rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                var cod = rd.IsDBNull(0) ? "" : rd.GetString(0);
                cod = (cod ?? "").Trim();
                if (string.IsNullOrWhiteSpace(cod)) continue;

                var cant = rd.IsDBNull(1) ? 0m : Convert.ToDecimal(rd.GetValue(1));
                if (cant <= 0m) continue;

                var unidad = rd.IsDBNull(2) ? null : Convert.ToString(rd.GetValue(2))?.Trim();
                var precio = rd.IsDBNull(3) ? 0m : Convert.ToDecimal(rd.GetValue(3));
                var totalLinea = rd.IsDBNull(4) ? 0m : Convert.ToDecimal(rd.GetValue(4));

                var lote = rd.IsDBNull(5) ? null : Convert.ToString(rd.GetValue(5))?.Trim();
                var serial = rd.IsDBNull(6) ? null : Convert.ToString(rd.GetValue(6))?.Trim();

                list.Add(new InvMovItem
                {
                    ProductoCodigo = cod,
                    Cantidad = cant,
                    Unidad = string.IsNullOrWhiteSpace(unidad) ? null : unidad,
                    PrecioUnitarioVenta = Math.Round(precio, 6, MidpointRounding.AwayFromZero),
                    TotalLineaVenta = Math.Round(totalLinea, 2, MidpointRounding.AwayFromZero),
                    Lote = string.IsNullOrWhiteSpace(lote) ? null : lote,
                    Serial = string.IsNullOrWhiteSpace(serial) ? null : serial,
                });
            }

            return list;
        }



        // ============================================================
        // ✅ DETALLE: Add / Update / Delete
        // ============================================================
        public int AddLineaConUnidad(
            int facturaId,
            string productoCodigo,
            string? codBarra,
            string descripcion,
            string unidad,
            int? impuestoId,
            decimal cantidad,
            decimal precioUnitario,
            decimal impuestoPctFallback,
            decimal descuentoPct
        )
        {
            if (facturaId <= 0) throw new ArgumentException("facturaId inválido.");
            if (string.IsNullOrWhiteSpace(productoCodigo)) throw new ArgumentException("productoCodigo requerido.");

            productoCodigo = productoCodigo.Trim();
            descripcion = (descripcion ?? "").Trim();
            if (descripcion.Length > 150) descripcion = descripcion.Substring(0, 150);

            unidad = (unidad ?? "").Trim();
            if (unidad.Length > 10) unidad = unidad.Substring(0, 10);

            var impTuple = ObtenerImpuestoPorProducto(productoCodigo, impuestoPctFallback);
            var impIdFinal = impTuple.impuestoId ?? impuestoId;
            var impPctFinal = impTuple.impuestoPct;

            descuentoPct = NormalizarPct(descuentoPct);
            impPctFinal = NormalizarPct(impPctFinal);

            ValidarLinea(descripcion, cantidad, precioUnitario, impPctFinal);

            using var cn = Db.GetOpenConnection();
            using var tx = cn.BeginTransaction();

            try
            {
                var descMonto = CalcDescuentoMonto(cantidad, precioUnitario, descuentoPct);
                var impuestoMonto = CalcItbisLineaSobreNeto(cantidad, precioUnitario, descMonto, impPctFinal);
                var totalLinea = Math.Round((cantidad * precioUnitario) - descMonto + impuestoMonto, 2, MidpointRounding.AwayFromZero);

                using var cmd = new SqlCommand(@"
INSERT INTO dbo.FacturaDet
(
    FacturaId, ProductoCodigo, CodBarra, Descripcion, Unidad,
    Cantidad, PrecioUnitario,
    DescuentoPct, DescuentoMonto,
    ImpuestoId,
    ImpuestoPct, ImpuestoMonto,
    TotalLinea,
    Precio, ItbisPct, ItbisMonto
)
OUTPUT INSERTED.FacturaDetId
VALUES
(
    @id, @prod, @cb, @desc, @uni,
    @cant, @punit,
    @dpct, @dmto,
    @impId,
    @ipct, @imto,
    @tot,
    @precio, @itbispct, @itbismto
);", cn, tx);

                cmd.Parameters.Add("@id", SqlDbType.Int).Value = facturaId;
                cmd.Parameters.Add("@prod", SqlDbType.VarChar, 20).Value = productoCodigo;
                cmd.Parameters.Add("@cb", SqlDbType.VarChar, 50).Value = (object?)codBarra ?? DBNull.Value;
                cmd.Parameters.Add("@desc", SqlDbType.NVarChar, 150).Value = descripcion;
                cmd.Parameters.Add("@uni", SqlDbType.VarChar, 10).Value = unidad;

                cmd.Parameters.Add("@impId", SqlDbType.Int).Value = (object?)impIdFinal ?? DBNull.Value;

                var pCant = cmd.Parameters.Add("@cant", SqlDbType.Decimal); pCant.Precision = 18; pCant.Scale = 2; pCant.Value = cantidad;
                var pUnit = cmd.Parameters.Add("@punit", SqlDbType.Decimal); pUnit.Precision = 18; pUnit.Scale = 2; pUnit.Value = precioUnitario;

                var pDpct = cmd.Parameters.Add("@dpct", SqlDbType.Decimal); pDpct.Precision = 9; pDpct.Scale = 4; pDpct.Value = descuentoPct;
                var pDmto = cmd.Parameters.Add("@dmto", SqlDbType.Decimal); pDmto.Precision = 18; pDmto.Scale = 2; pDmto.Value = descMonto;

                var pIpct = cmd.Parameters.Add("@ipct", SqlDbType.Decimal); pIpct.Precision = 9; pIpct.Scale = 4; pIpct.Value = impPctFinal;
                var pImto = cmd.Parameters.Add("@imto", SqlDbType.Decimal); pImto.Precision = 18; pImto.Scale = 2; pImto.Value = impuestoMonto;

                var pTot = cmd.Parameters.Add("@tot", SqlDbType.Decimal); pTot.Precision = 18; pTot.Scale = 2; pTot.Value = totalLinea;

                var pPrecio = cmd.Parameters.Add("@precio", SqlDbType.Decimal); pPrecio.Precision = 18; pPrecio.Scale = 2; pPrecio.Value = precioUnitario;
                var pItbPct = cmd.Parameters.Add("@itbispct", SqlDbType.Decimal); pItbPct.Precision = 9; pItbPct.Scale = 4; pItbPct.Value = impPctFinal;
                var pItbMto = cmd.Parameters.Add("@itbismto", SqlDbType.Decimal); pItbMto.Precision = 18; pItbMto.Scale = 2; pItbMto.Value = impuestoMonto;

                var detId = Convert.ToInt32(cmd.ExecuteScalar());

                RecalcularTotales(facturaId, cn, tx);
                tx.Commit();
                return detId;
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }

        public void UpdateLineaConUnidad(
            int facturaDetId,
            string descripcion,
            string unidad,
            decimal cantidad,
            decimal precioUnitario,
            decimal impuestoPct,
            decimal descuentoPct
        )
        {
            impuestoPct = NormalizarPct(impuestoPct);
            descuentoPct = NormalizarPct(descuentoPct);
            ValidarLinea(descripcion, cantidad, precioUnitario, impuestoPct);

            unidad = string.IsNullOrWhiteSpace(unidad) ? "" : unidad.Trim();
            if (unidad.Length > 10) unidad = unidad.Substring(0, 10);

            using var cn = Db.GetOpenConnection();
            using var tx = cn.BeginTransaction();

            try
            {
                int facturaId = GetFacturaIdByDet(facturaDetId, cn, tx);

                var descMonto = CalcDescuentoMonto(cantidad, precioUnitario, descuentoPct);
                var impuestoMonto = CalcItbisLineaSobreNeto(cantidad, precioUnitario, descMonto, impuestoPct);
                var totalLinea = Math.Round((cantidad * precioUnitario) - descMonto + impuestoMonto, 2, MidpointRounding.AwayFromZero);

                using var cmd = new SqlCommand(@"
UPDATE dbo.FacturaDet
SET Descripcion    = @desc,
    Unidad         = @uni,
    Cantidad       = @cant,
    PrecioUnitario = @punit,
    DescuentoPct   = @dpct,
    DescuentoMonto = @dmto,
    ImpuestoPct    = @ipct,
    ImpuestoMonto  = @imto,
    TotalLinea     = @tot,
    Precio         = @precio,
    ItbisPct       = @itbispct,
    ItbisMonto     = @itbismto
WHERE FacturaDetId = @det;", cn, tx);

                cmd.Parameters.Add("@det", SqlDbType.Int).Value = facturaDetId;

                var desc = (descripcion ?? "").Trim();
                if (desc.Length > 150) desc = desc.Substring(0, 150);
                cmd.Parameters.Add("@desc", SqlDbType.NVarChar, 150).Value = desc;

                cmd.Parameters.Add("@uni", SqlDbType.VarChar, 10).Value = unidad;

                var pCant = cmd.Parameters.Add("@cant", SqlDbType.Decimal); pCant.Precision = 18; pCant.Scale = 2; pCant.Value = cantidad;
                var pUnit = cmd.Parameters.Add("@punit", SqlDbType.Decimal); pUnit.Precision = 18; pUnit.Scale = 2; pUnit.Value = precioUnitario;

                var pDpct = cmd.Parameters.Add("@dpct", SqlDbType.Decimal); pDpct.Precision = 9; pDpct.Scale = 4; pDpct.Value = descuentoPct;
                var pDmto = cmd.Parameters.Add("@dmto", SqlDbType.Decimal); pDmto.Precision = 18; pDmto.Scale = 2; pDmto.Value = descMonto;

                var pIpct = cmd.Parameters.Add("@ipct", SqlDbType.Decimal); pIpct.Precision = 9; pIpct.Scale = 4; pIpct.Value = impuestoPct;
                var pImto = cmd.Parameters.Add("@imto", SqlDbType.Decimal); pImto.Precision = 18; pImto.Scale = 2; pImto.Value = impuestoMonto;

                var pTot = cmd.Parameters.Add("@tot", SqlDbType.Decimal); pTot.Precision = 18; pTot.Scale = 2; pTot.Value = totalLinea;

                var pPrecio = cmd.Parameters.Add("@precio", SqlDbType.Decimal); pPrecio.Precision = 18; pPrecio.Scale = 2; pPrecio.Value = precioUnitario;
                var pItbPct = cmd.Parameters.Add("@itbispct", SqlDbType.Decimal); pItbPct.Precision = 9; pItbPct.Scale = 4; pItbPct.Value = impuestoPct;
                var pItbMto = cmd.Parameters.Add("@itbismto", SqlDbType.Decimal); pItbMto.Precision = 18; pItbMto.Scale = 2; pItbMto.Value = impuestoMonto;

                cmd.ExecuteNonQuery();

                RecalcularTotales(facturaId, cn, tx);
                tx.Commit();
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }

        public void DeleteLinea(int facturaDetId)
        {
            using var cn = Db.GetOpenConnection();
            using var tx = cn.BeginTransaction();

            try
            {
                int facturaId = GetFacturaIdByDet(facturaDetId, cn, tx);

                using (var cmd = new SqlCommand("DELETE FROM dbo.FacturaDet WHERE FacturaDetId=@id;", cn, tx))
                {
                    cmd.Parameters.Add("@id", SqlDbType.Int).Value = facturaDetId;
                    cmd.ExecuteNonQuery();
                }

                RecalcularTotales(facturaId, cn, tx);
                tx.Commit();
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }

        // ============================================================
        // ✅ FINALIZAR
        // ============================================================
        public string Finalizar(int facturaId, bool esCredito, int? terminoPagoId, int? diasCreditoManual, string usuarioFinaliza)
            => Finalizar(facturaId, esCredito, terminoPagoId, diasCreditoManual, usuarioFinaliza, permitirStockNegativo: false);

        public string Finalizar(int facturaId, bool esCredito, int? terminoPagoId, int? diasCreditoManual, string usuarioFinaliza, bool permitirStockNegativo)
        {
            using var cn = Db.GetOpenConnection();
            using var tx = cn.BeginTransaction(IsolationLevel.Serializable);
            var cache = new SchemaCache();

            try
            {
                var cab = GetCabeceraMin(facturaId, cn, tx);
                if (cab == null) throw new InvalidOperationException("Factura no existe.");

                if (!string.Equals(cab.Estado, EST_BORRADOR, StringComparison.OrdinalIgnoreCase))
                    throw new InvalidOperationException("Solo se puede finalizar una factura en BORRADOR.");

                RecalcularTotales(facturaId, cn, tx);

                using (var cmdChk = new SqlCommand(@"
SELECT COUNT(1)
FROM dbo.FacturaDet
WHERE FacturaId = @id;", cn, tx))
                {
                    cmdChk.Parameters.Add("@id", SqlDbType.Int).Value = facturaId;
                    var lineas = Convert.ToInt32(cmdChk.ExecuteScalar() ?? 0);

                    if (lineas <= 0)
                        throw new InvalidOperationException("No se puede finalizar una factura sin productos.");
                }

                using (var cmdTot = new SqlCommand(@"
SELECT ISNULL(TotalGeneral,0)
FROM dbo.FacturaCab
WHERE FacturaId = @id;", cn, tx))
                {
                    cmdTot.Parameters.Add("@id", SqlDbType.Int).Value = facturaId;
                    var total = Convert.ToDecimal(cmdTot.ExecuteScalar() ?? 0m);

                    if (total <= 0m)
                        throw new InvalidOperationException("No se puede finalizar una factura con total en cero.");
                }

                // ✅ SOLO FAC: stock + movimiento inv
                if (string.Equals(cab.TipoDocumento, TIPO_FAC, StringComparison.OrdinalIgnoreCase))
                {
                    var ctx = GetFacturaContextOrDefault(facturaId, cn, tx, cache);

                    // si el caller forzó true -> pasa; si no, usa config global
                    var permitirNegativo = permitirStockNegativo || PermiteStockNegativo(ctx.empresaId, ctx.sucursalId, ctx.almacenId, cn, tx);

                    // ✅ APLICAR BLOQUEO POR PRODUCTO AQUÍ (ya corregido con clave corta)
                    DescontarStockPorFactura(facturaId, cn, tx, cache, permitirNegativo);

                    CrearInvMovimientoPorFactura(facturaId, usuarioFinaliza, cn, tx, cache);
                }

                string numero = cab.NumeroDocumento ?? "";
                if (string.IsNullOrWhiteSpace(numero))
                {
                    var codigoNumerador = ResolveNumeradorCodigo(cn, tx, cab.TipoDocumento);

                    var fallbackPref = codigoNumerador switch
                    {
                        "COT" => "COT-",
                        "PF" => "PF-",
                        _ => "V"
                    };

                    numero = GetNextNumerador(cn, tx, codigoNumerador, fallbackPref, 6);
                }

                var tipoPago = esCredito ? PAGO_CREDITO : PAGO_CONTADO;
                var usr = string.IsNullOrWhiteSpace(usuarioFinaliza) ? "SYSTEM" : usuarioFinaliza.Trim();

                DateTime? fechaVenc = null;
                int? diasCreditoFinal = null;

                if (esCredito)
                {
                    if (!terminoPagoId.HasValue)
                        throw new InvalidOperationException("Crédito requiere TerminoPagoId.");

                    using (var cmdDel = new SqlCommand("DELETE FROM dbo.FacturaPagoPlan WHERE FacturaId=@id;", cn, tx))
                    {
                        cmdDel.Parameters.Add("@id", SqlDbType.Int).Value = facturaId;
                        cmdDel.ExecuteNonQuery();
                    }

                    var plan = GenerarPlanPago(facturaId, terminoPagoId.Value, diasCreditoManual, usr, cn, tx);
                    fechaVenc = plan.ultimaFechaVenc;
                    diasCreditoFinal = plan.diasCreditoUsado;
                }
                else
                {
                    terminoPagoId = null;
                    diasCreditoFinal = null;
                    fechaVenc = null;

                    using (var cmdDel = new SqlCommand("DELETE FROM dbo.FacturaPagoPlan WHERE FacturaId=@id;", cn, tx))
                    {
                        cmdDel.Parameters.Add("@id", SqlDbType.Int).Value = facturaId;
                        cmdDel.ExecuteNonQuery();
                    }
                }

                using (var cmd = new SqlCommand(@"
UPDATE dbo.FacturaCab
SET NumeroDocumento  = @num,
    TipoPago         = @tpago,
    TerminoPagoId    = @term,
    DiasCredito      = @dias,
    FechaVencimiento = @venc,
    Estado           = @est,
    UsuarioFinaliza  = @usr,
    FechaFinaliza    = GETDATE()
WHERE FacturaId=@id;", cn, tx))
                {
                    cmd.Parameters.Add("@id", SqlDbType.Int).Value = facturaId;
                    cmd.Parameters.Add("@num", SqlDbType.VarChar, 20).Value = numero;
                    cmd.Parameters.Add("@tpago", SqlDbType.VarChar, 10).Value = tipoPago;
                    cmd.Parameters.Add("@term", SqlDbType.Int).Value = (object?)terminoPagoId ?? DBNull.Value;
                    cmd.Parameters.Add("@dias", SqlDbType.Int).Value = (object?)diasCreditoFinal ?? DBNull.Value;
                    cmd.Parameters.Add("@venc", SqlDbType.DateTime).Value = (object?)fechaVenc ?? DBNull.Value;
                    cmd.Parameters.Add("@est", SqlDbType.VarChar, 15).Value = EST_FINALIZADA;
                    cmd.Parameters.Add("@usr", SqlDbType.VarChar, 30).Value = usr;
                    cmd.ExecuteNonQuery();
                }

                tx.Commit();
                return numero;
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }

        // ============================================================
        // ✅ STOCK / EXISTENCIA (optimizada con cache)
        // ============================================================
        private sealed class FacturaCtx
        {
            public int empresaId;
            public int? sucursalId;
            public int almacenId;
            public int AlmacenId => almacenId;
        }

        private static FacturaCtx ObtenerContextoFactura(int facturaId, SqlConnection cn, SqlTransaction tx, SchemaCache cache)
        {
            var cols = GetColumns(cn, tx, "dbo.FacturaCab", cache);

            var colEmp = Pick(cols, "EmpresaId");
            var colSuc = Pick(cols, "SucursalId");
            var colAlm = Pick(cols, "AlmacenId", "AlmacenIdOrigen", "AlmacenOrigenId");

            int empresaId = 0;
            int? sucursalId = null;
            int almacenId = 0;

            var selectCols = new List<string>();
            if (colEmp != null) selectCols.Add(SqlQuote(colEmp) + " AS EmpresaId");
            if (colSuc != null) selectCols.Add(SqlQuote(colSuc) + " AS SucursalId");
            if (colAlm != null) selectCols.Add(SqlQuote(colAlm) + " AS AlmacenId");

            if (selectCols.Count > 0)
            {
                using var cmd = new SqlCommand($@"
SELECT TOP(1) {string.Join(",", selectCols)}
FROM dbo.FacturaCab
WHERE FacturaId=@id;", cn, tx);

                cmd.Parameters.Add("@id", SqlDbType.Int).Value = facturaId;

                using var rd = cmd.ExecuteReader();
                if (rd.Read())
                {
                    if (colEmp != null && !rd.IsDBNull(rd.GetOrdinal("EmpresaId")))
                        empresaId = Convert.ToInt32(rd["EmpresaId"]);

                    if (colSuc != null && !rd.IsDBNull(rd.GetOrdinal("SucursalId")))
                        sucursalId = Convert.ToInt32(rd["SucursalId"]);

                    if (colAlm != null && !rd.IsDBNull(rd.GetOrdinal("AlmacenId")))
                        almacenId = Convert.ToInt32(rd["AlmacenId"]);
                }
            }

            if (empresaId <= 0)
                empresaId = GetConfigInt(cn, tx, CFG_EMPRESA_DEFECTO_ID) ?? 1;

            if (almacenId <= 0)
                almacenId = GetConfigInt(cn, tx, CFG_ALMACEN_DEFECTO_ID) ?? 1;

            return new FacturaCtx { empresaId = empresaId, sucursalId = sucursalId, almacenId = almacenId };
        }

        private static (string whereSql, bool hasEmpresa, bool hasSucursal) BuildExistenciaWhere(SqlConnection cn, SqlTransaction tx, SchemaCache cache)
        {
            if (cache.ExistenciaWhere.HasValue)
                return cache.ExistenciaWhere.Value;

            var cols = GetColumns(cn, tx, "dbo.Existencia", cache);

            var hasEmpresa = cols.Contains("EmpresaId");
            var hasSucursal = cols.Contains("SucursalId");

            var parts = new List<string>
            {
                "AlmacenId = @alm",
                "ProductoCodigo = @prod"
            };

            if (hasEmpresa) parts.Add("EmpresaId = @emp");
            if (hasSucursal) parts.Add("SucursalId = @suc");

            var result = (string.Join(" AND ", parts), hasEmpresa, hasSucursal);
            cache.ExistenciaWhere = result;
            return result;
        }

        private static decimal ObtenerExistenciaActual(FacturaCtx ctx, string productoCodigo, SqlConnection cn, SqlTransaction tx, SchemaCache cache)
        {
            var w = BuildExistenciaWhere(cn, tx, cache);

            using var cmd = new SqlCommand($@"
SELECT TOP(1) Cantidad
FROM dbo.Existencia WITH (UPDLOCK, HOLDLOCK)
WHERE {w.whereSql};", cn, tx);

            cmd.Parameters.Add("@alm", SqlDbType.Int).Value = ctx.almacenId;
            cmd.Parameters.Add("@prod", SqlDbType.VarChar, 20).Value = productoCodigo;

            if (w.hasEmpresa) cmd.Parameters.Add("@emp", SqlDbType.Int).Value = ctx.empresaId;
            if (w.hasSucursal) cmd.Parameters.Add("@suc", SqlDbType.Int).Value = (object?)ctx.sucursalId ?? DBNull.Value;

            var v = cmd.ExecuteScalar();
            if (v == null || v == DBNull.Value) return 0m;
            return Convert.ToDecimal(v);
        }

        private static void ActualizarExistencia(FacturaCtx ctx, string productoCodigo, decimal delta, SqlConnection cn, SqlTransaction tx, SchemaCache cache)
        {
            var w = BuildExistenciaWhere(cn, tx, cache);

            using (var cmd = new SqlCommand($@"
UPDATE dbo.Existencia
SET Cantidad = Cantidad + @d
WHERE {w.whereSql};", cn, tx))
            {
                var pd = cmd.Parameters.Add("@d", SqlDbType.Decimal);
                pd.Precision = 18; pd.Scale = 4; pd.Value = delta;

                cmd.Parameters.Add("@alm", SqlDbType.Int).Value = ctx.almacenId;
                cmd.Parameters.Add("@prod", SqlDbType.VarChar, 20).Value = productoCodigo;

                if (w.hasEmpresa) cmd.Parameters.Add("@emp", SqlDbType.Int).Value = ctx.empresaId;
                if (w.hasSucursal) cmd.Parameters.Add("@suc", SqlDbType.Int).Value = (object?)ctx.sucursalId ?? DBNull.Value;

                var rows = cmd.ExecuteNonQuery();
                if (rows > 0) return;
            }

            var cols = GetColumns(cn, tx, "dbo.Existencia", cache);
            var hasEmpresa = cols.Contains("EmpresaId");
            var hasSucursal = cols.Contains("SucursalId");

            var insertCols = new List<string> { "ProductoCodigo", "AlmacenId", "Cantidad", "CostoPromedio" };
            var insertVals = new List<string> { "@prod", "@alm", "@cant", "@cost" };

            if (hasEmpresa) { insertCols.Add("EmpresaId"); insertVals.Add("@emp"); }
            if (hasSucursal) { insertCols.Add("SucursalId"); insertVals.Add("@suc"); }

            using var cmd2 = new SqlCommand($@"
INSERT INTO dbo.Existencia ({string.Join(",", insertCols)})
VALUES ({string.Join(",", insertVals)});", cn, tx);

            cmd2.Parameters.Add("@prod", SqlDbType.VarChar, 20).Value = productoCodigo;
            cmd2.Parameters.Add("@alm", SqlDbType.Int).Value = ctx.almacenId;

            var pc = cmd2.Parameters.Add("@cant", SqlDbType.Decimal);
            pc.Precision = 18; pc.Scale = 4; pc.Value = delta;

            var pcp = cmd2.Parameters.Add("@cost", SqlDbType.Decimal);
            pcp.Precision = 18; pcp.Scale = 6; pcp.Value = 0m;

            if (hasEmpresa) cmd2.Parameters.Add("@emp", SqlDbType.Int).Value = ctx.empresaId;
            if (hasSucursal) cmd2.Parameters.Add("@suc", SqlDbType.Int).Value = (object?)ctx.sucursalId ?? DBNull.Value;

            cmd2.ExecuteNonQuery();
        }

        // ✅ AQUI ESTÁ EL CAMBIO: BLOQUEAR NEGATIVO POR PRODUCTO (ya con clave corta válida)
        private static void DescontarStockPorFactura(int facturaId, SqlConnection cn, SqlTransaction tx, SchemaCache cache, bool permitirNegativo)
        {
            var items = LeerItemsFacturaAgrupados(facturaId, cn, tx);
            if (items.Count == 0) return;

            var ctx = ObtenerContextoFactura(facturaId, cn, tx, cache);

            foreach (var it in items)
            {
                var actual = ObtenerExistenciaActual(ctx, it.ProductoCodigo, cn, tx, cache);

                // ✅ Si el producto está bloqueado, NUNCA permitir negativo
                var bloqueado = BloqueaNegativoPorProducto(ctx.empresaId, ctx.sucursalId, ctx.almacenId, it.ProductoCodigo, cn, tx);

                if (bloqueado && actual < it.Cantidad)
                    throw new InvalidOperationException(
                        $"Stock insuficiente para '{it.ProductoCodigo}'. (Bloqueado para negativo) Disponible: {actual:N4}, requerido: {it.Cantidad:N4}"
                    );

                // regla normal
                if (!permitirNegativo && actual < it.Cantidad)
                    throw new InvalidOperationException(
                        $"Stock insuficiente para '{it.ProductoCodigo}'. Disponible: {actual:N4}, requerido: {it.Cantidad:N4}"
                    );

                ActualizarExistencia(ctx, it.ProductoCodigo, -it.Cantidad, cn, tx, cache);
            }
        }


        // ============================================================
        // ✅ INVENTARIO: MOVIMIENTO POR FACTURA (FAC)
        // ============================================================
        private static void CrearInvMovimientoPorFactura(int facturaId, string? usuario, SqlConnection cn, SqlTransaction tx, SchemaCache cache)
        {
            var usr = string.IsNullOrWhiteSpace(usuario) ? "SYSTEM" : usuario.Trim();

            var items = LeerItemsFacturaAgrupados(facturaId, cn, tx);
            if (items.Count == 0) return;

            var items = LeerLineasFacturaDetParaInvMov(facturaId, cn, tx, cache);
            InsertInvMovimientoLin(invMovId, facturaId, usr, items, "VENTA", cn, tx, cache);





        }

        private static List<(string ProductoCodigo, decimal Cantidad)> LeerItemsFacturaAgrupados(int facturaId, SqlConnection cn, SqlTransaction tx)
        {
            using var cmd = new SqlCommand(@"
SELECT ProductoCodigo, Cantidad
FROM dbo.FacturaDet
WHERE FacturaId = @id;", cn, tx);

            cmd.Parameters.Add("@id", SqlDbType.Int).Value = facturaId;

            using var rd = cmd.ExecuteReader();

            var raw = new List<(string ProductoCodigo, decimal Cantidad)>();
            while (rd.Read())
            {
                var cod = rd.IsDBNull(0) ? "" : rd.GetString(0);
                var cant = rd.IsDBNull(1) ? 0m : rd.GetDecimal(1);

                cod = (cod ?? "").Trim();
                if (string.IsNullOrWhiteSpace(cod)) continue;
                if (cant <= 0m) continue;

                raw.Add((cod, cant));
            }

            return raw
                .GroupBy(x => x.ProductoCodigo, StringComparer.OrdinalIgnoreCase)
                .Select(g => (ProductoCodigo: g.Key, Cantidad: g.Sum(x => x.Cantidad)))
                .ToList();
        }

        private static decimal ResolverCostoUnitario(FacturaCtx ctx, string productoCodigo, SqlConnection cn, SqlTransaction tx, SchemaCache cache)
        {
            // 1) Existencia.CostoPromedio
            var costoExist = ObtenerCostoPromedioExistencia(ctx, productoCodigo, cn, tx, cache);
            if (costoExist > 0m) return costoExist;

            // 2) fallback Producto
            var prod = ObtenerDatosProducto(productoCodigo, cn, tx);
            if (prod.costoFallback.HasValue && prod.costoFallback.Value > 0m)
                return Math.Round(prod.costoFallback.Value, 6, MidpointRounding.AwayFromZero);

            return 0m;
        }

        private static long InsertInvMovimientoCab(int facturaId, string usuario, SqlConnection cn, SqlTransaction tx, SchemaCache cache)
        {
            var cols = GetColumns(cn, tx, "dbo.InvMovimientoCab", cache);

            var fechaCol = Pick(cols, "Fecha");
            var tipoCol = Pick(cols, "Tipo");
            var usuarioCol = Pick(cols, "Usuario");
            var estadoCol = Pick(cols, "Estado");

            if (fechaCol == null || tipoCol == null || usuarioCol == null || estadoCol == null)
                throw new InvalidOperationException("InvMovimientoCab debe tener columnas: Fecha, Tipo, Usuario, Estado.");

            var origenCol = Pick(cols, "Origen");
            var origenIdCol = Pick(cols, "OrigenId");
            var obsCol = Pick(cols, "Observacion");
            var fechaCreCol = Pick(cols, "FechaCreacion");
            var usrCreCol = Pick(cols, "UsuarioCreacion");

            // ✅ NUEVO: almacenes
            var almOriCol = Pick(cols, "AlmacenIdOrigen");
            var almDesCol = Pick(cols, "AlmacenIdDestino");

            // ✅ sacar el AlmacenId de la factura (tu ctx ya lo trae)
            var ctx = ObtenerContextoFactura(facturaId, cn, tx, cache);
            var almacenId = ctx.almacenId; // o ctx.AlmacenId si ya pusiste la property

            var fecha = DateTime.Now;
            var tipo = "VENTA";
            var estado = "CONFIRMADO";
            var origen = "FACTURA";
            var obs = $"Salida por factura #{facturaId}";

            var colList = new List<string>();
            var valList = new List<string>();

            colList.Add(SqlQuote(fechaCol)); valList.Add("@Fecha");
            colList.Add(SqlQuote(tipoCol)); valList.Add("@Tipo");
            colList.Add(SqlQuote(usuarioCol)); valList.Add("@Usuario");
            colList.Add(SqlQuote(estadoCol)); valList.Add("@Estado");

            if (origenCol != null) { colList.Add(SqlQuote(origenCol)); valList.Add("@Origen"); }
            if (origenIdCol != null) { colList.Add(SqlQuote(origenIdCol)); valList.Add("@OrigenId"); }
            if (obsCol != null) { colList.Add(SqlQuote(obsCol)); valList.Add("@Obs"); }

            // ✅ incluir almacenes si existen
            if (almOriCol != null) { colList.Add(SqlQuote(almOriCol)); valList.Add("@AlmOri"); }
            if (almDesCol != null) { colList.Add(SqlQuote(almDesCol)); valList.Add("@AlmDes"); }

            if (fechaCreCol != null) { colList.Add(SqlQuote(fechaCreCol)); valList.Add("@FechaCre"); }
            if (usrCreCol != null) { colList.Add(SqlQuote(usrCreCol)); valList.Add("@UsrCre"); }

            var sql = $@"
INSERT INTO dbo.InvMovimientoCab
({string.Join(",", colList)})
OUTPUT INSERTED.InvMovId
VALUES
({string.Join(",", valList)});";

            using var cmd = new SqlCommand(sql, cn, tx);

            cmd.Parameters.Add("@Fecha", SqlDbType.DateTime2).Value = fecha;
            cmd.Parameters.Add("@Tipo", SqlDbType.VarChar, 20).Value = tipo;
            cmd.Parameters.Add("@Usuario", SqlDbType.NVarChar, 100).Value = usuario;
            cmd.Parameters.Add("@Estado", SqlDbType.VarChar, 12).Value = estado;

            if (origenCol != null) cmd.Parameters.Add("@Origen", SqlDbType.VarChar, 20).Value = origen;
            if (origenIdCol != null) cmd.Parameters.Add("@OrigenId", SqlDbType.BigInt).Value = (long)facturaId;
            if (obsCol != null) cmd.Parameters.Add("@Obs", SqlDbType.NVarChar, 200).Value = obs;

            // ✅ llenar ambos almacenes (trazabilidad)
            if (almOriCol != null) cmd.Parameters.Add("@AlmOri", SqlDbType.Int).Value = almacenId;
            if (almDesCol != null) cmd.Parameters.Add("@AlmDes", SqlDbType.Int).Value = almacenId;

            if (fechaCreCol != null) cmd.Parameters.Add("@FechaCre", SqlDbType.DateTime).Value = DateTime.Now;
            if (usrCreCol != null) cmd.Parameters.Add("@UsrCre", SqlDbType.VarChar, 30).Value =
                usuario.Length > 30 ? usuario.Substring(0, 30) : usuario;

            var id = Convert.ToInt64(cmd.ExecuteScalar() ?? 0L);
            if (id <= 0) throw new InvalidOperationException("No se pudo generar InvMovId en InvMovimientoCab.");
            return id;
        }
        private static void InsertInvMovimientoLin(
     long invMovId,
     int facturaId,
     string usuario,
     List<InvMovItem> items,
     string tipoMovimiento, // "VENTA" o "ANULACION_VENTA"
     SqlConnection cn,
     SqlTransaction tx,
     SchemaCache cache)
        {
            if (invMovId <= 0) throw new ArgumentException("invMovId inválido.", nameof(invMovId));
            if (items == null || items.Count == 0) return;

            var esAnulacion = string.Equals(tipoMovimiento, "ANULACION_VENTA", StringComparison.OrdinalIgnoreCase);
            var ctx = ObtenerContextoFactura(facturaId, cn, tx, cache);

            var cols = GetColumns(cn, tx, "dbo.InvMovimientoLin", cache);

            // flags de columnas (sin asumir)
            var hasAlm = cols.Contains("AlmacenId");
            var hasCat = cols.Contains("CategoriaId");
            var hasSub = cols.Contains("SubcategoriaId");
            var hasUni = cols.Contains("Unidad");
            var hasCT = cols.Contains("CostoTotal");
            var hasPUV = cols.Contains("PrecioUnitarioVenta");
            var hasTLV = cols.Contains("TotalLineaVenta");
            var hasLote = cols.Contains("Lote");
            var hasSer = cols.Contains("Serial");

            // Tu tabla base obligatoria:
            // InvMovId, Linea, ProductoCodigo, Cantidad, CostoUnitario, Usuario
            var colList = new List<string> { "InvMovId", "Linea", "ProductoCodigo", "Cantidad", "CostoUnitario", "Usuario" };
            var valList = new List<string> { "@InvMovId", "@Linea", "@Prod", "@Cant", "@Costo", "@Usr" };

            if (hasAlm) { colList.Add("AlmacenId"); valList.Add("@Alm"); }
            if (hasCat) { colList.Add("CategoriaId"); valList.Add("@Cat"); }
            if (hasSub) { colList.Add("SubcategoriaId"); valList.Add("@Sub"); }

            if (hasUni) { colList.Add("Unidad"); valList.Add("@Uni"); }
            if (hasCT) { colList.Add("CostoTotal"); valList.Add("@CostoTot"); }
            if (hasPUV) { colList.Add("PrecioUnitarioVenta"); valList.Add("@PUV"); }
            if (hasTLV) { colList.Add("TotalLineaVenta"); valList.Add("@TLV"); }
            if (hasLote) { colList.Add("Lote"); valList.Add("@Lote"); }
            if (hasSer) { colList.Add("Serial"); valList.Add("@Serial"); }

            var sql = $@"
INSERT INTO dbo.InvMovimientoLin
({string.Join(",", colList)})
VALUES
({string.Join(",", valList)});";

            using var cmd = new SqlCommand(sql, cn, tx);

            cmd.Parameters.Add("@InvMovId", SqlDbType.BigInt).Value = invMovId;

            var pLinea = cmd.Parameters.Add("@Linea", SqlDbType.Int);
            var pProd = cmd.Parameters.Add("@Prod", SqlDbType.VarChar, 20);

            var pCant = cmd.Parameters.Add("@Cant", SqlDbType.Decimal);
            pCant.Precision = 18; pCant.Scale = 4;

            var pCosto = cmd.Parameters.Add("@Costo", SqlDbType.Decimal);
            pCosto.Precision = 18; pCosto.Scale = 6;

            // Usuario (max 30)
            var usr = string.IsNullOrWhiteSpace(usuario) ? "SYSTEM" : usuario.Trim();
            if (usr.Length > 30) usr = usr.Substring(0, 30);
            cmd.Parameters.Add("@Usr", SqlDbType.VarChar, 30).Value = usr;

            SqlParameter? pAlm = null; if (hasAlm) pAlm = cmd.Parameters.Add("@Alm", SqlDbType.Int);
            SqlParameter? pCat = null; if (hasCat) pCat = cmd.Parameters.Add("@Cat", SqlDbType.Int);
            SqlParameter? pSub = null; if (hasSub) pSub = cmd.Parameters.Add("@Sub", SqlDbType.Int);

            SqlParameter? pUni = null; if (hasUni) pUni = cmd.Parameters.Add("@Uni", SqlDbType.VarChar, 10);
            SqlParameter? pCT = null; if (hasCT)
            {
                pCT = cmd.Parameters.Add("@CostoTot", SqlDbType.Decimal);
                pCT.Precision = 18; pCT.Scale = 6;
            }

            SqlParameter? pPUV = null; if (hasPUV)
            {
                pPUV = cmd.Parameters.Add("@PUV", SqlDbType.Decimal);
                pPUV.Precision = 18; pPUV.Scale = 6;
            }

            SqlParameter? pTLV = null; if (hasTLV)
            {
                pTLV = cmd.Parameters.Add("@TLV", SqlDbType.Decimal);
                pTLV.Precision = 18; pTLV.Scale = 2;
            }

            SqlParameter? pLote = null; if (hasLote) pLote = cmd.Parameters.Add("@Lote", SqlDbType.VarChar, 50);
            SqlParameter? pSer = null; if (hasSer) pSer = cmd.Parameters.Add("@Serial", SqlDbType.VarChar, 50);

            var linea = 1;
            foreach (var it in items)
            {
                var cod = (it.ProductoCodigo ?? "").Trim();
                if (string.IsNullOrWhiteSpace(cod)) continue;
                if (it.Cantidad <= 0m) continue;

                pLinea.Value = linea++;
                pProd.Value = cod;

                // ✅ VENTA (-) / ANULACION (+)
                var cantMov = esAnulacion ? Math.Abs(it.Cantidad) : -Math.Abs(it.Cantidad);
                pCant.Value = cantMov;

                // ✅ costo unitario (Existencia -> fallback Producto)
                var costoUnit = ResolverCostoUnitario(ctx, cod, cn, tx, cache);
                pCosto.Value = costoUnit;

                // ✅ almacén
                if (hasAlm) pAlm!.Value = ctx.almacenId;

                // ✅ Cat/Sub determinístico desde Producto
                if (hasCat || hasSub)
                {
                    var prodInfo = ObtenerDatosProducto(cod, cn, tx);
                    if (hasCat) pCat!.Value = (object?)prodInfo.categoriaId ?? DBNull.Value;
                    if (hasSub) pSub!.Value = (object?)prodInfo.subcategoriaId ?? DBNull.Value;
                }

                // ✅ Unidad / venta / lote / serial (desde FacturaDet)
                if (hasUni) pUni!.Value = (object?)it.Unidad ?? DBNull.Value;

                if (hasPUV) pPUV!.Value = it.PrecioUnitarioVenta;
                if (hasTLV) pTLV!.Value = it.TotalLineaVenta;

                // ✅ costo total = abs(cantidad) * costoUnit (independiente del signo del movimiento)
                if (hasCT) pCT!.Value = Math.Round(Math.Abs(it.Cantidad) * costoUnit, 6, MidpointRounding.AwayFromZero);

                if (hasLote) pLote!.Value = (object?)it.Lote ?? DBNull.Value;
                if (hasSer) pSer!.Value = (object?)it.Serial ?? DBNull.Value;

                cmd.ExecuteNonQuery();
            }
        }



        // ============================================================
        // ✅ PLAN PAGO / CALCULOS / CAB/DET / HISTORIAL
        // ============================================================
        private static decimal CalcDescuentoMonto(decimal cantidad, decimal precio, decimal descuentoPct)
        {
            descuentoPct = NormalizarPct(descuentoPct);
            var baseLinea = cantidad * precio;
            var desc = baseLinea * (descuentoPct / 100m);
            return Math.Round(desc, 2, MidpointRounding.AwayFromZero);
        }

        private static decimal CalcItbisLineaSobreNeto(decimal cantidad, decimal precio, decimal descuentoMonto, decimal itbisPct)
        {
            itbisPct = NormalizarPct(itbisPct);
            var baseLinea = (cantidad * precio) - descuentoMonto;
            if (baseLinea < 0m) baseLinea = 0m;
            var itbis = baseLinea * (itbisPct / 100m);
            return Math.Round(itbis, 2, MidpointRounding.AwayFromZero);
        }

        private static void ValidarLinea(string descripcion, decimal cantidad, decimal precio, decimal itbisPct)
        {
            if (string.IsNullOrWhiteSpace(descripcion))
                throw new ArgumentException("Descripción requerida.");

            if (cantidad <= 0) throw new ArgumentException("Cantidad inválida.");
            if (precio < 0) throw new ArgumentException("Precio inválido.");
            if (itbisPct < 0 || itbisPct > 100) throw new ArgumentException("Impuesto % inválido.");
        }

        private static int GetFacturaIdByDet(int detId, SqlConnection cn, SqlTransaction tx)
        {
            using var cmd = new SqlCommand("SELECT FacturaId FROM dbo.FacturaDet WHERE FacturaDetId=@det;", cn, tx);
            cmd.Parameters.Add("@det", SqlDbType.Int).Value = detId;

            var val = cmd.ExecuteScalar();
            if (val == null || val == DBNull.Value) throw new InvalidOperationException("Detalle no existe.");
            return Convert.ToInt32(val);
        }

        private static void RecalcularTotales(int facturaId, SqlConnection cn, SqlTransaction tx)
        {
            using var cmd = new SqlCommand(@"
;WITH X AS
(
    SELECT
        SUM(Cantidad * PrecioUnitario) AS Subt,
        SUM(ISNULL(DescuentoMonto,0))  AS DescT,
        SUM(ISNULL(ImpuestoMonto,0))   AS Itb
    FROM dbo.FacturaDet
    WHERE FacturaId=@id
)
UPDATE dbo.FacturaCab
SET SubTotal       = ISNULL((SELECT Subt  FROM X),0),
    TotalDescuento = ISNULL((SELECT DescT FROM X),0),
    TotalImpuesto  = ISNULL((SELECT Itb   FROM X),0)
  , TotalGeneral   = ISNULL((SELECT Subt  FROM X),0)
                 - ISNULL((SELECT DescT FROM X),0)
                 + ISNULL((SELECT Itb   FROM X),0)
WHERE FacturaId=@id;", cn, tx);

            cmd.Parameters.Add("@id", SqlDbType.Int).Value = facturaId;
            cmd.ExecuteNonQuery();
        }

        private sealed class CabMin
        {
            public int FacturaId { get; set; }
            public string TipoDocumento { get; set; } = "";
            public string? NumeroDocumento { get; set; }
            public string Estado { get; set; } = "";
            public decimal TotalGeneral { get; set; }
        }

        private static CabMin? GetCabeceraMin(int facturaId, SqlConnection cn, SqlTransaction tx)
        {
            using var cmd = new SqlCommand(@"
SELECT FacturaId, TipoDocumento, NumeroDocumento, Estado, ISNULL(TotalGeneral,0)
FROM dbo.FacturaCab
WHERE FacturaId=@id;", cn, tx);

            cmd.Parameters.Add("@id", SqlDbType.Int).Value = facturaId;

            using var rd = cmd.ExecuteReader();
            if (!rd.Read()) return null;

            return new CabMin
            {
                FacturaId = SafeGetInt(rd, 0),
                TipoDocumento = SafeGetString(rd, 1, ""),
                NumeroDocumento = rd.IsDBNull(2) ? null : rd.GetString(2),
                Estado = SafeGetString(rd, 3, ""),
                TotalGeneral = SafeGetDecimal(rd, 4, 0m)
            };
        }


        private static (DateTime ultimaFechaVenc, int diasCreditoUsado) GenerarPlanPago(
            int facturaId,
            int terminoPagoId,
            int? diasCreditoManual,
            string usuario,
            SqlConnection cn,
            SqlTransaction tx)
        {
            decimal total;
            using (var cmdTot = new SqlCommand("SELECT ISNULL(TotalGeneral,0) FROM dbo.FacturaCab WHERE FacturaId=@id;", cn, tx))
            {
                cmdTot.Parameters.Add("@id", SqlDbType.Int).Value = facturaId;
                total = Convert.ToDecimal(cmdTot.ExecuteScalar() ?? 0m);
            }

            DateTime fechaBase;
            using (var cmdF = new SqlCommand("SELECT FechaDocumento FROM dbo.FacturaCab WHERE FacturaId=@id;", cn, tx))
            {
                cmdF.Parameters.Add("@id", SqlDbType.Int).Value = facturaId;
                var v = cmdF.ExecuteScalar();
                fechaBase = (v == null || v == DBNull.Value) ? DateTime.Today : Convert.ToDateTime(v).Date;
            }

            int diasPlazo = 0;
            int? cantCuotas = null;
            int? frecDias = null;

            using (var cmd = new SqlCommand(@"
SELECT DiasPlazo, CantCuotas, FrecuenciaDias
FROM dbo.TerminoPago
WHERE TerminoPagoId=@id AND Estado=1;", cn, tx))
            {
                cmd.Parameters.Add("@id", SqlDbType.Int).Value = terminoPagoId;
                using var rd = cmd.ExecuteReader();
                if (!rd.Read()) throw new InvalidOperationException("TerminoPago inválido o inactivo.");

                diasPlazo = rd.IsDBNull(0) ? 0 : rd.GetInt32(0);
                cantCuotas = rd.IsDBNull(1) ? (int?)null : rd.GetInt32(1);
                frecDias = rd.IsDBNull(2) ? (int?)null : rd.GetInt32(2);
            }

            if (diasCreditoManual.HasValue && diasCreditoManual.Value > 0)
            {
                var venc = fechaBase.AddDays(diasCreditoManual.Value);
                InsertCuota(cn, tx, facturaId, 1, venc, total, usuario);
                return (venc, diasCreditoManual.Value);
            }

            if (diasPlazo > 0)
            {
                var venc = fechaBase.AddDays(diasPlazo);
                InsertCuota(cn, tx, facturaId, 1, venc, total, usuario);
                return (venc, diasPlazo);
            }

            if (!cantCuotas.HasValue || cantCuotas.Value <= 0)
                throw new InvalidOperationException("TerminoPago sin DiasPlazo y sin CantCuotas (no puede generar plan).");

            var n = cantCuotas.Value;
            var freq = (frecDias.HasValue && frecDias.Value > 0) ? frecDias.Value : 30;

            var cuotaBase = Math.Round(total / n, 2, MidpointRounding.AwayFromZero);
            decimal acumulado = 0m;
            DateTime ultimoVenc = fechaBase;

            for (int i = 1; i <= n; i++)
            {
                var venc = fechaBase.AddDays(freq * i);
                ultimoVenc = venc;

                decimal monto = cuotaBase;
                if (i == n)
                    monto = Math.Round(total - acumulado, 2, MidpointRounding.AwayFromZero);

                InsertCuota(cn, tx, facturaId, i, venc, monto, usuario);
                acumulado += monto;
            }

            var diasCredito = (int)(ultimoVenc.Date - fechaBase.Date).TotalDays;
            return (ultimoVenc, diasCredito);
        }

        private static void InsertCuota(SqlConnection cn, SqlTransaction tx, int facturaId, int numCuota, DateTime venc, decimal monto, string usuario)
        {
            using var cmd = new SqlCommand(@"
INSERT INTO dbo.FacturaPagoPlan
(FacturaId, NumCuota, FechaVencimiento, MontoCuota, MontoPagado, Estado, FechaPagoTotal, Usuario)
VALUES
(@fac, @n, @fv, @m, 0, 'PENDIENTE', NULL, @usr);", cn, tx);

            cmd.Parameters.Add("@fac", SqlDbType.Int).Value = facturaId;
            cmd.Parameters.Add("@n", SqlDbType.Int).Value = numCuota;
            cmd.Parameters.Add("@fv", SqlDbType.Date).Value = venc.Date;

            var pm = cmd.Parameters.Add("@m", SqlDbType.Decimal);
            pm.Precision = 18; pm.Scale = 2; pm.Value = monto;

            cmd.Parameters.Add("@usr", SqlDbType.VarChar, 30).Value = string.IsNullOrWhiteSpace(usuario) ? "SYSTEM" : usuario.Trim();
            cmd.ExecuteNonQuery();
        }

        private static decimal NormalizarPct(decimal pct)
        {
            if (pct < 0m) pct = 0m;
            if (pct > 100m) pct = 100m;
            return Math.Round(pct, 4, MidpointRounding.AwayFromZero);
        }

        // Alias por compatibilidad (si tu UI lo llama así)
        public string ConvertirCotPfAFac_GenerandoNuevoNumero(int facturaId, string usuarioFinaliza)
        {
            return ConvertirCOTPFaFAC(facturaId, usuarioFinaliza);
        }

        // Alias por compatibilidad (si tu UI lo llama así)
        public FacturaCabDto? ObtenerCabPorNumero(string numeroDocumento)
        {
            if (string.IsNullOrWhiteSpace(numeroDocumento)) return null;

            using var cn = Db.GetOpenConnection();

            var hasDir = HasColumn(cn, null, "dbo.FacturaCab", "DireccionCliente");

            var sql = @"
SELECT TOP(1)
    FacturaId, TipoDocumento, NumeroDocumento, FechaDocumento, FechaVencimiento,
    ClienteId, NombreCliente, DocumentoCliente," + (hasDir ? " DireccionCliente," : "") + @"
    ISNULL(SubTotal,0), ISNULL(TotalDescuento,0), ISNULL(TotalImpuesto,0), ISNULL(TotalGeneral,0),
    TipoPago, TerminoPagoId, DiasCredito, Estado, Observacion,
    eNCF, TrackId, CodigoSeguridad
FROM dbo.FacturaCab
WHERE NumeroDocumento = @num
   OR eNCF = @num
ORDER BY FacturaId DESC;";

            using var cmd = new SqlCommand(sql, cn);
            cmd.Parameters.Add("@num", SqlDbType.VarChar, 50).Value = numeroDocumento.Trim();

            using var rd = cmd.ExecuteReader();
            if (!rd.Read()) return null;

            int i = 0;

            var dto = new FacturaCabDto
            {
                FacturaId = rd.GetInt32(i++),
                TipoDocumento = rd.GetString(i++),

                NumeroDocumento = rd.IsDBNull(i) ? null : rd.GetString(i),
            };
            i++; // ✅ MUY IMPORTANTE

            dto.FechaDocumento = SafeGetDateTime(rd, i++, DateTime.Today);
            dto.FechaVencimiento = SafeGetDateTimeN(rd, i++);

            dto.ClienteId = SafeGetIntN(rd, i++);
            dto.NombreCliente = SafeGetString(rd, i++, "");

            // ✅ FIX: aquí faltaba i++
            dto.DocumentoCliente = rd.IsDBNull(i) ? null : rd.GetString(i);
            i++; // ✅ MUY IMPORTANTE

            if (hasDir)
            {
                dto.DireccionCliente = rd.IsDBNull(i) ? null : rd.GetString(i);
                i++;
            }
            else dto.DireccionCliente = null;

            dto.SubTotal = SafeGetDecimal(rd, i++);
            dto.TotalDescuento = SafeGetDecimal(rd, i++);
            dto.TotalImpuesto = SafeGetDecimal(rd, i++);
            dto.TotalGeneral = SafeGetDecimal(rd, i++);

            dto.TipoPago = SafeGetString(rd, i++, "");
            dto.TerminoPagoId = SafeGetIntN(rd, i++);
            dto.DiasCredito = SafeGetIntN(rd, i++);

            dto.Estado = SafeGetString(rd, i++, "");

            dto.Observacion = rd.IsDBNull(i) ? null : rd.GetString(i);
            i++;

            dto.ENCF = rd.IsDBNull(i) ? null : rd.GetString(i);
            i++;

            dto.TrackId = rd.IsDBNull(i) ? null : rd.GetString(i);
            i++;

            dto.CodigoSeguridad = rd.IsDBNull(i) ? null : rd.GetString(i);
            i++;

            return dto;
        }

        // ============================================================
        // ✅ LECTURA CAB / DET / COMPLETA
        // ============================================================
        public FacturaCabDto? ObtenerCab(int facturaId)
        {
            using var cn = Db.GetOpenConnection();

            var hasDir = HasColumn(cn, null, "dbo.FacturaCab", "DireccionCliente");

            var sql = @"
SELECT
    FacturaId, TipoDocumento, NumeroDocumento, FechaDocumento, FechaVencimiento,
    ClienteId, NombreCliente, DocumentoCliente," + (hasDir ? " DireccionCliente," : "") + @"
    ISNULL(SubTotal,0), ISNULL(TotalDescuento,0), ISNULL(TotalImpuesto,0), ISNULL(TotalGeneral,0),
    TipoPago, TerminoPagoId, DiasCredito, Estado, Observacion,
    eNCF, TrackId, CodigoSeguridad
FROM dbo.FacturaCab
WHERE FacturaId=@id;";

            using var cmd = new SqlCommand(sql, cn);
            cmd.Parameters.Add("@id", SqlDbType.Int).Value = facturaId;

            using var rd = cmd.ExecuteReader();
            if (!rd.Read()) return null;

            int i = 0;

            var dto = new FacturaCabDto
            {
                FacturaId = SafeGetInt(rd, i++),
                TipoDocumento = SafeGetString(rd, i++, ""),

                // ✅ FIX: aquí faltaba i++
                NumeroDocumento = rd.IsDBNull(i) ? null : rd.GetString(i),
            };
            i++; // ✅ MUY IMPORTANTE

            dto.FechaDocumento = SafeGetDateTime(rd, i++, DateTime.Today);
            dto.FechaVencimiento = SafeGetDateTimeN(rd, i++);

            dto.ClienteId = SafeGetIntN(rd, i++);
            dto.NombreCliente = SafeGetString(rd, i++, "");

            // ✅ FIX: aquí también faltaba i++
            dto.DocumentoCliente = rd.IsDBNull(i) ? null : rd.GetString(i);
            i++; // ✅ MUY IMPORTANTE

            if (hasDir)
            {
                dto.DireccionCliente = rd.IsDBNull(i) ? null : rd.GetString(i);
                i++;
            }
            else dto.DireccionCliente = null;

            dto.SubTotal = SafeGetDecimal(rd, i++);
            dto.TotalDescuento = SafeGetDecimal(rd, i++);
            dto.TotalImpuesto = SafeGetDecimal(rd, i++);
            dto.TotalGeneral = SafeGetDecimal(rd, i++);

            dto.TipoPago = SafeGetString(rd, i++, "");
            dto.TerminoPagoId = SafeGetIntN(rd, i++);
            dto.DiasCredito = SafeGetIntN(rd, i++);

            dto.Estado = SafeGetString(rd, i++, "");

            dto.Observacion = rd.IsDBNull(i) ? null : rd.GetString(i);
            i++;

            dto.ENCF = rd.IsDBNull(i) ? null : rd.GetString(i);
            i++;

            dto.TrackId = rd.IsDBNull(i) ? null : rd.GetString(i);
            i++;

            dto.CodigoSeguridad = rd.IsDBNull(i) ? null : rd.GetString(i);
            i++;

            return dto;
        }

        private static bool HasColumn(SqlConnection cn, SqlTransaction? tx, string tableFullName, string columnName)
        {
            using var cmd = new SqlCommand(@"
SELECT COUNT(1)
FROM sys.columns
WHERE object_id = OBJECT_ID(@t)
  AND name = @c;", cn, tx);

            cmd.Parameters.Add("@t", SqlDbType.NVarChar, 200).Value = tableFullName;
            cmd.Parameters.Add("@c", SqlDbType.NVarChar, 200).Value = columnName;

            var n = Convert.ToInt32(cmd.ExecuteScalar() ?? 0);
            return n > 0;
        }


        public void SetCreditoBorrador(int facturaId, bool esCredito, int? terminoPagoId, int? diasCredito)
        {
            if (facturaId <= 0) throw new ArgumentException("facturaId inválido.");

            // Si NO es crédito, limpiamos todo
            if (!esCredito)
            {
                terminoPagoId = null;
                diasCredito = null;
            }
            else
            {
                // Si es crédito, terminoPagoId debe existir
                if (!terminoPagoId.HasValue || terminoPagoId.Value <= 0)
                    throw new InvalidOperationException("Crédito requiere TerminoPagoId.");

                // diasCredito puede ser null o >= 0 (si quieres obligarlo, cambia aquí)
                if (diasCredito.HasValue && diasCredito.Value < 0)
                    diasCredito = 0;
            }

            using var cn = Db.GetOpenConnection();

            using var cmd = new SqlCommand(@"
UPDATE dbo.FacturaCab
SET TipoPago      = @tp,
    TerminoPagoId = @term,
    DiasCredito   = @dias
WHERE FacturaId   = @id
  AND Estado      = 'BORRADOR';", cn);

            cmd.Parameters.Add("@id", SqlDbType.Int).Value = facturaId;
            cmd.Parameters.Add("@tp", SqlDbType.VarChar, 10).Value = esCredito ? PAGO_CREDITO : PAGO_CONTADO;

            cmd.Parameters.Add("@term", SqlDbType.Int).Value = (object?)terminoPagoId ?? DBNull.Value;
            cmd.Parameters.Add("@dias", SqlDbType.Int).Value = (object?)diasCredito ?? DBNull.Value;

            cmd.ExecuteNonQuery();
        }


        private static decimal ObtenerCostoPromedioExistencia(
    FacturaCtx ctx,
    string productoCodigo,
    SqlConnection cn,
    SqlTransaction tx,
    SchemaCache cache)
        {
            if (ctx == null) return 0m;
            if (string.IsNullOrWhiteSpace(productoCodigo)) return 0m;

            var sql = @"
SELECT TOP(1)
    CostoPromedio
FROM dbo.Existencia
WHERE ProductoCodigo = @prod
  AND AlmacenId = @alm
  AND EmpresaId = @emp;";

            using var cmd = new SqlCommand(sql, cn, tx);
            cmd.Parameters.Add("@prod", SqlDbType.VarChar, 20).Value = productoCodigo.Trim();
            cmd.Parameters.Add("@alm", SqlDbType.Int).Value = ctx.almacenId;
            cmd.Parameters.Add("@emp", SqlDbType.Int).Value = ctx.empresaId;

            var val = cmd.ExecuteScalar();
            if (val == null || val == DBNull.Value)
                return 0m;

            try
            {
                var costo = Convert.ToDecimal(val);
                return costo > 0m
                    ? Math.Round(costo, 6, MidpointRounding.AwayFromZero)
                    : 0m;
            }
            catch
            {
                return 0m;
            }
        }


        // ============================================================
        // ✅ DIRECCION CLIENTE (persistencia en FacturaCab)
        //    - No depende del DTO (FacturaCabDto no tiene la propiedad)
        //    - No truena si la columna no existe
        // ============================================================


        public string? ObtenerDireccionCliente(int facturaId)
        {
            if (facturaId <= 0) return null;

            using var cn = Db.GetOpenConnection();

            // si no existe la columna, devolvemos null sin fallar
            if (!HasColumn(cn, null, "dbo.FacturaCab", "DireccionCliente"))
                return null;

            using var cmd = new SqlCommand(@"
SELECT DireccionCliente
FROM dbo.FacturaCab
WHERE FacturaId = @id;", cn);

            cmd.Parameters.Add("@id", SqlDbType.Int).Value = facturaId;

            var obj = cmd.ExecuteScalar();
            var s = (obj == null || obj == DBNull.Value) ? null : Convert.ToString(obj);
            return string.IsNullOrWhiteSpace(s) ? null : s.Trim();
        }

        public class FacturaCompletaDto
        {
            public FacturaCabDto Cab { get; set; } = new FacturaCabDto();
            public List<FacturaDetDto> Det { get; set; } = new List<FacturaDetDto>();
        }

        public List<FacturaDetDto> ListarDet(int facturaId)
        {
            var list = new List<FacturaDetDto>();
            using var cn = Db.GetOpenConnection();

            using var cmd = new SqlCommand(@"
SELECT
    FacturaDetId,
    FacturaId,
    ProductoCodigo,
    CodBarra,
    Descripcion,
    Unidad,
    Cantidad,
    PrecioUnitario,
    DescuentoPct,
    DescuentoMonto,
    ImpuestoId,
    ImpuestoPct,
    ImpuestoMonto,
    TotalLinea,
    Precio,
    ItbisPct,
    ItbisMonto
FROM dbo.FacturaDet
WHERE FacturaId = @id
ORDER BY FacturaDetId;", cn);

            cmd.Parameters.Add("@id", SqlDbType.Int).Value = facturaId;

            using var rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                int i = 0;

                var dto = new FacturaDetDto();

                dto.FacturaDetId = SafeGetInt(rd, i++);
                dto.FacturaId = SafeGetInt(rd, i++);

                dto.ProductoCodigo = SafeGetString(rd, i++, "");
                dto.CodBarra = rd.IsDBNull(i) ? null : rd.GetString(i); i++;
                dto.Descripcion = SafeGetString(rd, i++, "");
                dto.Unidad = SafeGetString(rd, i++, "");

                dto.Cantidad = SafeGetDecimal(rd, i++, 0m);
                dto.Precio = SafeGetDecimal(rd, i++, 0m); // PrecioUnitario

                dto.DescuentoPct = SafeGetDecimal(rd, i++, 0m);
                dto.DescuentoMonto = SafeGetDecimal(rd, i++, 0m);

                dto.ImpuestoId = SafeGetIntN(rd, i++);

                // ImpuestoPct/ImpuestoMonto
                dto.ItbisPct = SafeGetDecimal(rd, i++, 0m);
                dto.ItbisMonto = SafeGetDecimal(rd, i++, 0m);

                dto.TotalLinea = SafeGetDecimal(rd, i++, 0m);

                // Precio / ItbisPct / ItbisMonto (compat)
                dto.Precio = SafeGetDecimal(rd, i++, dto.Precio);
                dto.ItbisPct = SafeGetDecimal(rd, i++, dto.ItbisPct);
                dto.ItbisMonto = SafeGetDecimal(rd, i++, dto.ItbisMonto);

                list.Add(dto);
            }

            return list;
        }



        public FacturaCompletaDto? ObtenerCompleta(int facturaId)
        {
            var cab = ObtenerCab(facturaId);
            if (cab == null) return null;

            var det = ListarDet(facturaId);

            return new FacturaCompletaDto { Cab = cab, Det = det };
        }
    }
 }

