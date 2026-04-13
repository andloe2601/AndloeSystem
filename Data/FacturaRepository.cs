using Andloe.Entidad;
using Andloe.Entidad.Facturacion;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Data;

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


        public FacturaRepository()
        {
        }

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

        public DateTime? ObtenerFechaVencimientoSecuencia(int empresaId, int sucursalId, int? cajaId, int tipoId, string prefijo)
        {
            using var cn = Db.GetOpenConnection();

            using var cmd = new SqlCommand(@"
SELECT TOP 1 FechaVencimientoSecuencia
FROM dbo.NCF_Rango
WHERE EmpresaId = @EmpresaId
  AND SucursalId = @SucursalId
  AND (CajaId = @CajaId OR CajaId IS NULL)
  AND TipoId = @TipoId
  AND Prefijo = @Prefijo
  AND ActivoParaEmisionECF = 1
ORDER BY CASE WHEN CajaId IS NULL THEN 1 ELSE 0 END, RangoId DESC;
", cn);

            cmd.Parameters.Add("@EmpresaId", SqlDbType.Int).Value = empresaId;
            cmd.Parameters.Add("@SucursalId", SqlDbType.Int).Value = sucursalId;
            cmd.Parameters.Add("@CajaId", SqlDbType.Int).Value = (object?)cajaId ?? DBNull.Value;
            cmd.Parameters.Add("@TipoId", SqlDbType.Int).Value = tipoId;
            cmd.Parameters.Add("@Prefijo", SqlDbType.VarChar, 4).Value = prefijo;

            var result = cmd.ExecuteScalar();

            if (result == null || result == DBNull.Value)
                return null;

            return Convert.ToDateTime(result);
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

        public void SetFechaVencimientoBorrador(int facturaId, DateTime? fechaVencimiento)
        {
            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
UPDATE dbo.FacturaCab
SET FechaVencimiento = @FechaVencimiento
WHERE FacturaId = @FacturaId;", cn);

            cmd.Parameters.Add("@FacturaId", SqlDbType.Int).Value = facturaId;
            cmd.Parameters.Add("@FechaVencimiento", SqlDbType.DateTime).Value =
                (object?)fechaVencimiento ?? DBNull.Value;

            cmd.ExecuteNonQuery();
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

            var ctx = ObtenerContextoFactura(facturaId, cn, tx, cache);
            var almacenId = ctx.almacenId;

            // 1. Crear cabecera del movimiento de anulación
            var invMovId = InsertInvMovimientoCab_AnulacionFactura(facturaId, usr, almacenId, cn, tx, cache);

            // 2. Leer líneas con información completa
            var items = LeerLineasFacturaDetParaInvMov(facturaId, cn, tx, cache);

            if (items.Count == 0) return;

            // 3. Insertar líneas del movimiento (tipo ANULACION_VENTA = entrada, cantidad positiva)
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

        public void SetCodVendedorBorrador(int facturaId, string? codVendedor)
        {
            if (facturaId <= 0) return;

            using var cn = Db.GetOpenConnection();

            // si no existe la columna, no hacemos nada (sin error)
            if (!HasColumn(cn, null, "dbo.FacturaCab", "CodVendedor"))
                return;

            codVendedor = string.IsNullOrWhiteSpace(codVendedor) ? null : codVendedor.Trim();

            using var cmd = new SqlCommand(@"
UPDATE dbo.FacturaCab
SET CodVendedor = @cod
WHERE FacturaId = @id
  AND Estado = 'BORRADOR';", cn);

            cmd.Parameters.Add("@id", SqlDbType.Int).Value = facturaId;
            cmd.Parameters.Add("@cod", SqlDbType.VarChar, 20).Value = (object?)codVendedor ?? DBNull.Value;

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
        private sealed class LineaFiscalData
        {
            public string CodigoItemFiscal { get; set; } = "";
            public string DescripcionFiscal { get; set; } = "";
            public string UnidadMedidaCodigo { get; set; } = "";
            public int? UnidadMedidaECFId { get; set; }
            public string UnidadMedidaDGII { get; set; } = "";
            public int TipoProducto { get; set; } = 1;
            public int IndicadorBienOServicio { get; set; } = 1;
            public bool PrecioIncluyeITBIS { get; set; }
            public bool EsExento { get; set; }

            public decimal BaseImponible { get; set; }
            public decimal MontoGravado { get; set; }
            public decimal MontoExento { get; set; }
            public decimal SubtotalLineaAntesImpuesto { get; set; }
            public decimal ItbisMonto { get; set; }
            public decimal TotalLinea { get; set; }
            public int IndicadorITBISIncluido { get; set; }
        }

        private static decimal Round2(decimal value)
            => Math.Round(value, 2, MidpointRounding.AwayFromZero);

        private static decimal CalcBaseBruta(decimal cantidad, decimal precioUnitario)
            => Round2(cantidad * precioUnitario);

        private static void AddOrSetDecimalParameter(SqlCommand cmd, string name, decimal? value, byte precision = 18, byte scale = 2)
        {
            if (cmd.Parameters.Contains(name))
            {
                cmd.Parameters[name].Value = (object?)value ?? DBNull.Value;
                return;
            }

            var p = cmd.Parameters.Add(name, SqlDbType.Decimal);
            p.Precision = precision;
            p.Scale = scale;
            p.Value = (object?)value ?? DBNull.Value;
        }

        private static void AddOrSetIntParameter(SqlCommand cmd, string name, int? value)
        {
            if (cmd.Parameters.Contains(name))
            {
                cmd.Parameters[name].Value = (object?)value ?? DBNull.Value;
                return;
            }

            cmd.Parameters.Add(name, SqlDbType.Int).Value = (object?)value ?? DBNull.Value;
        }

        private static void AddOrSetStringParameter(SqlCommand cmd, string name, string? value, int size, SqlDbType dbType = SqlDbType.VarChar)
        {
            var val = string.IsNullOrWhiteSpace(value) ? (object)DBNull.Value : value.Trim();

            if (cmd.Parameters.Contains(name))
            {
                cmd.Parameters[name].Value = val;
                return;
            }

            cmd.Parameters.Add(name, dbType, size).Value = val;
        }

        private static Producto ObtenerProductoFiscalObligatorio(string productoCodigo)
        {
            var prodRepo = new ProductoRepository();
            var producto = prodRepo.ObtenerPorCodigo(productoCodigo);

            if (producto == null)
                throw new InvalidOperationException($"No se encontró el producto '{productoCodigo}' para completar datos fiscales.");

            producto.NormalizeDefaults();
            return producto;
        }

        private static LineaFiscalData ConstruirLineaFiscalDesdeProducto(
            Producto producto,
            string descripcionLinea,
            string unidadLinea,
            decimal cantidad,
            decimal precioUnitario,
            decimal descuentoPct,
            decimal impuestoPct)
        {
            if (producto == null)
                throw new ArgumentNullException(nameof(producto));

            producto.NormalizeDefaults();

            descuentoPct = NormalizarPct(descuentoPct);
            impuestoPct = NormalizarPct(impuestoPct);

            var descripcionFiscal = string.IsNullOrWhiteSpace(producto.DescripcionFiscal)
                ? (string.IsNullOrWhiteSpace(descripcionLinea) ? producto.Descripcion : descripcionLinea.Trim())
                : producto.DescripcionFiscal.Trim();

            var codigoItemFiscal = string.IsNullOrWhiteSpace(producto.CodigoItemFiscal)
                ? producto.Codigo.Trim()
                : producto.CodigoItemFiscal.Trim();

            var unidadCodigo = string.IsNullOrWhiteSpace(producto.UnidadMedidaCodigo)
                ? (string.IsNullOrWhiteSpace(unidadLinea) ? "UND" : unidadLinea.Trim().ToUpperInvariant())
                : producto.UnidadMedidaCodigo.Trim().ToUpperInvariant();

            var unidadDgii = string.IsNullOrWhiteSpace(producto.UnidadMedidaDGII)
                ? unidadCodigo
                : producto.UnidadMedidaDGII.Trim().ToUpperInvariant();

            var tipoProducto = producto.TipoProducto is 1 or 2 ? producto.TipoProducto : 1;
            var indicadorBienOServicio = tipoProducto == 2 ? 2 : 1;

            var baseBruta = CalcBaseBruta(cantidad, precioUnitario);
            var descuentoMonto = CalcDescuentoMonto(cantidad, precioUnitario, descuentoPct);
            var subtotalAntesImpuesto = Round2(baseBruta - descuentoMonto);
            if (subtotalAntesImpuesto < 0m) subtotalAntesImpuesto = 0m;

            decimal baseImponible = 0m;
            decimal montoGravado = 0m;
            decimal montoExento = 0m;
            decimal itbisMonto = 0m;

            var esExento = producto.EsExento || !producto.ImpuestoId.HasValue || impuestoPct <= 0m;

            if (esExento)
            {
                montoExento = subtotalAntesImpuesto;
            }
            else
            {
                baseImponible = subtotalAntesImpuesto;
                montoGravado = subtotalAntesImpuesto;
                itbisMonto = CalcItbisLineaSobreNeto(cantidad, precioUnitario, descuentoMonto, impuestoPct);
            }

            var totalLinea = Round2(subtotalAntesImpuesto + itbisMonto);

            return new LineaFiscalData
            {
                CodigoItemFiscal = codigoItemFiscal,
                DescripcionFiscal = descripcionFiscal,
                UnidadMedidaCodigo = unidadCodigo,
                UnidadMedidaECFId = producto.UnidadMedidaECFId,
                UnidadMedidaDGII = unidadDgii,
                TipoProducto = tipoProducto,
                IndicadorBienOServicio = indicadorBienOServicio,
                PrecioIncluyeITBIS = producto.PrecioIncluyeITBIS,
                EsExento = esExento,
                BaseImponible = baseImponible,
                MontoGravado = montoGravado,
                MontoExento = montoExento,
                SubtotalLineaAntesImpuesto = subtotalAntesImpuesto,
                ItbisMonto = itbisMonto,
                TotalLinea = totalLinea,
                IndicadorITBISIncluido = producto.PrecioIncluyeITBIS ? 1 : 0
            };
        }

        private static HashSet<string> GetColumns(SqlConnection cn, string tableFullName, SqlTransaction tx)
        {
            var cache = new SchemaCache();
            return GetColumns(cn, tx, tableFullName, cache);
        }

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

            var producto = ObtenerProductoFiscalObligatorio(productoCodigo);

            descripcion = string.IsNullOrWhiteSpace(descripcion)
                ? producto.Descripcion
                : descripcion.Trim();

            if (descripcion.Length > 150)
                descripcion = descripcion.Substring(0, 150);

            unidad = string.IsNullOrWhiteSpace(unidad)
                ? producto.UnidadMedidaCodigo
                : unidad.Trim().ToUpperInvariant();

            if (unidad.Length > 10)
                unidad = unidad.Substring(0, 10);

            var impTuple = ObtenerImpuestoPorProducto(productoCodigo, impuestoPctFallback);
            var impIdFinal = producto.EsExento ? null : (impTuple.impuestoId ?? impuestoId ?? producto.ImpuestoId);
            var impPctFinal = producto.EsExento ? 0m : impTuple.impuestoPct;

            descuentoPct = NormalizarPct(descuentoPct);
            impPctFinal = NormalizarPct(impPctFinal);

            ValidarLinea(descripcion, cantidad, precioUnitario, impPctFinal);

            var descMonto = CalcDescuentoMonto(cantidad, precioUnitario, descuentoPct);
            var fiscal = ConstruirLineaFiscalDesdeProducto(
                producto,
                descripcion,
                unidad,
                cantidad,
                precioUnitario,
                descuentoPct,
                impPctFinal
            );

            using var cn = Db.GetOpenConnection();
            using var tx = cn.BeginTransaction();

            try
            {
                var detCols = GetColumns(cn, "dbo.FacturaDet", tx);

                var insertCols = new List<string>
                {
                    "FacturaId",
                    "ProductoCodigo",
                    "CodBarra",
                    "Descripcion",
                    "Unidad",
                    "Cantidad",
                    "PrecioUnitario",
                    "DescuentoPct",
                    "DescuentoMonto",
                    "ImpuestoId",
                    "ImpuestoPct",
                    "ImpuestoMonto",
                    "TotalLinea",
                    "Precio",
                    "ItbisPct",
                    "ItbisMonto"
                };

                var insertVals = new List<string>
                {
                    "@id",
                    "@prod",
                    "@cb",
                    "@desc",
                    "@uni",
                    "@cant",
                    "@punit",
                    "@dpct",
                    "@dmto",
                    "@impId",
                    "@ipct",
                    "@imto",
                    "@tot",
                    "@precio",
                    "@itbispct",
                    "@itbismto"
                };

                void AddOptional(string col, string param)
                {
                    if (detCols.Contains(col))
                    {
                        insertCols.Add(col);
                        insertVals.Add(param);
                    }
                }

                AddOptional("CodigoItemFiscal", "@CodigoItemFiscal");
                AddOptional("DescripcionFiscal", "@DescripcionFiscal");
                AddOptional("UnidadMedidaCodigo", "@UnidadMedidaCodigo");
                AddOptional("UnidadMedidaECFId", "@UnidadMedidaECFId");
                AddOptional("UnidadMedidaDGII", "@UnidadMedidaDGII");
                AddOptional("TipoProducto", "@TipoProducto");
                AddOptional("IndicadorBienOServicio", "@IndicadorBienOServicio");
                AddOptional("IndicadorITBISIncluido", "@IndicadorITBISIncluido");
                AddOptional("BaseImponible", "@BaseImponible");
                AddOptional("MontoGravado", "@MontoGravado");
                AddOptional("MontoExento", "@MontoExento");
                AddOptional("SubtotalLineaAntesImpuesto", "@SubtotalLineaAntesImpuesto");

                var sql = $@"
INSERT INTO dbo.FacturaDet
(
    {string.Join(", ", insertCols)}
)
OUTPUT INSERTED.FacturaDetId
VALUES
(
    {string.Join(", ", insertVals)}
);";

                using var cmd = new SqlCommand(sql, cn, tx);

                cmd.Parameters.Add("@id", SqlDbType.Int).Value = facturaId;
                cmd.Parameters.Add("@prod", SqlDbType.VarChar, 20).Value = productoCodigo;
                cmd.Parameters.Add("@cb", SqlDbType.VarChar, 50).Value = string.IsNullOrWhiteSpace(codBarra) ? (object)DBNull.Value : codBarra.Trim();
                cmd.Parameters.Add("@desc", SqlDbType.NVarChar, 150).Value = descripcion;
                cmd.Parameters.Add("@uni", SqlDbType.VarChar, 10).Value = unidad;
                cmd.Parameters.Add("@impId", SqlDbType.Int).Value = (object?)impIdFinal ?? DBNull.Value;

                var pCant = cmd.Parameters.Add("@cant", SqlDbType.Decimal);
                pCant.Precision = 18; pCant.Scale = 2; pCant.Value = cantidad;

                var pUnit = cmd.Parameters.Add("@punit", SqlDbType.Decimal);
                pUnit.Precision = 18; pUnit.Scale = 2; pUnit.Value = precioUnitario;

                var pDpct = cmd.Parameters.Add("@dpct", SqlDbType.Decimal);
                pDpct.Precision = 9; pDpct.Scale = 4; pDpct.Value = descuentoPct;

                var pDmto = cmd.Parameters.Add("@dmto", SqlDbType.Decimal);
                pDmto.Precision = 18; pDmto.Scale = 2; pDmto.Value = descMonto;

                var pIpct = cmd.Parameters.Add("@ipct", SqlDbType.Decimal);
                pIpct.Precision = 9; pIpct.Scale = 4; pIpct.Value = impPctFinal;

                var pImto = cmd.Parameters.Add("@imto", SqlDbType.Decimal);
                pImto.Precision = 18; pImto.Scale = 2; pImto.Value = fiscal.ItbisMonto;

                var pTot = cmd.Parameters.Add("@tot", SqlDbType.Decimal);
                pTot.Precision = 18; pTot.Scale = 2; pTot.Value = fiscal.TotalLinea;

                var pPrecio = cmd.Parameters.Add("@precio", SqlDbType.Decimal);
                pPrecio.Precision = 18; pPrecio.Scale = 2; pPrecio.Value = precioUnitario;

                var pItbPct = cmd.Parameters.Add("@itbispct", SqlDbType.Decimal);
                pItbPct.Precision = 9; pItbPct.Scale = 4; pItbPct.Value = impPctFinal;

                var pItbMto = cmd.Parameters.Add("@itbismto", SqlDbType.Decimal);
                pItbMto.Precision = 18; pItbMto.Scale = 2; pItbMto.Value = fiscal.ItbisMonto;

                AddOrSetStringParameter(cmd, "@CodigoItemFiscal", fiscal.CodigoItemFiscal, 50);
                AddOrSetStringParameter(cmd, "@DescripcionFiscal", fiscal.DescripcionFiscal, 250, SqlDbType.NVarChar);
                AddOrSetStringParameter(cmd, "@UnidadMedidaCodigo", fiscal.UnidadMedidaCodigo, 10);
                AddOrSetIntParameter(cmd, "@UnidadMedidaECFId", fiscal.UnidadMedidaECFId);
                AddOrSetStringParameter(cmd, "@UnidadMedidaDGII", fiscal.UnidadMedidaDGII, 10);
                AddOrSetIntParameter(cmd, "@TipoProducto", fiscal.TipoProducto);
                AddOrSetIntParameter(cmd, "@IndicadorBienOServicio", fiscal.IndicadorBienOServicio);
                AddOrSetIntParameter(cmd, "@IndicadorITBISIncluido", fiscal.IndicadorITBISIncluido);

                AddOrSetDecimalParameter(cmd, "@BaseImponible", fiscal.BaseImponible);
                AddOrSetDecimalParameter(cmd, "@MontoGravado", fiscal.MontoGravado);
                AddOrSetDecimalParameter(cmd, "@MontoExento", fiscal.MontoExento);
                AddOrSetDecimalParameter(cmd, "@SubtotalLineaAntesImpuesto", fiscal.SubtotalLineaAntesImpuesto);

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

        // ============================================================
        // ✅ DETALLE: Add / Update / Delete
        // ============================================================
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

        using var cn = Db.GetOpenConnection();
        using var tx = cn.BeginTransaction();

        try
        {
            int facturaId;
            string productoCodigo;
            int? impuestoIdActual = null;

            using (var cmdGet = new SqlCommand(@"
SELECT TOP (1)
    FacturaId,
    ProductoCodigo,
    ImpuestoId
FROM dbo.FacturaDet
WHERE FacturaDetId = @det;", cn, tx))
            {
                cmdGet.Parameters.Add("@det", SqlDbType.Int).Value = facturaDetId;

                using var rd = cmdGet.ExecuteReader();
                if (!rd.Read())
                    throw new InvalidOperationException("Detalle no existe.");

                facturaId = rd.IsDBNull(0) ? 0 : Convert.ToInt32(rd.GetValue(0));
                productoCodigo = rd.IsDBNull(1) ? "" : Convert.ToString(rd.GetValue(1)) ?? "";
                impuestoIdActual = rd.IsDBNull(2) ? (int?)null : Convert.ToInt32(rd.GetValue(2));
            }

            if (facturaId <= 0 || string.IsNullOrWhiteSpace(productoCodigo))
                throw new InvalidOperationException("No se pudo obtener información fiscal del detalle.");

            var producto = ObtenerProductoFiscalObligatorio(productoCodigo);

            descripcion = string.IsNullOrWhiteSpace(descripcion)
                ? producto.Descripcion
                : descripcion.Trim();

            if (descripcion.Length > 150)
                descripcion = descripcion.Substring(0, 150);

            unidad = string.IsNullOrWhiteSpace(unidad)
                ? producto.UnidadMedidaCodigo
                : unidad.Trim().ToUpperInvariant();

            if (unidad.Length > 10)
                unidad = unidad.Substring(0, 10);

            var impTuple = ObtenerImpuestoPorProducto(productoCodigo, impuestoPct);
            var impIdFinal = producto.EsExento ? null : (impTuple.impuestoId ?? impuestoIdActual ?? producto.ImpuestoId);
            var impPctFinal = producto.EsExento ? 0m : impTuple.impuestoPct;
            impPctFinal = NormalizarPct(impPctFinal);

            var descMonto = CalcDescuentoMonto(cantidad, precioUnitario, descuentoPct);
            var fiscal = ConstruirLineaFiscalDesdeProducto(
                producto,
                descripcion,
                unidad,
                cantidad,
                precioUnitario,
                descuentoPct,
                impPctFinal
            );

            var detCols = GetColumns(cn, "dbo.FacturaDet", tx);

            var setParts = new List<string>
                {
                    "Descripcion = @desc",
                    "Unidad = @uni",
                    "Cantidad = @cant",
                    "PrecioUnitario = @punit",
                    "DescuentoPct = @dpct",
                    "DescuentoMonto = @dmto",
                    "ImpuestoId = @impId",
                    "ImpuestoPct = @ipct",
                    "ImpuestoMonto = @imto",
                    "TotalLinea = @tot",
                    "Precio = @precio",
                    "ItbisPct = @itbispct",
                    "ItbisMonto = @itbismto"
                };

            void AddOptionalSet(string col, string param)
            {
                if (detCols.Contains(col))
                    setParts.Add($"{col} = {param}");
            }

            AddOptionalSet("CodigoItemFiscal", "@CodigoItemFiscal");
            AddOptionalSet("DescripcionFiscal", "@DescripcionFiscal");
            AddOptionalSet("UnidadMedidaCodigo", "@UnidadMedidaCodigo");
            AddOptionalSet("UnidadMedidaECFId", "@UnidadMedidaECFId");
            AddOptionalSet("UnidadMedidaDGII", "@UnidadMedidaDGII");
            AddOptionalSet("TipoProducto", "@TipoProducto");
            AddOptionalSet("IndicadorBienOServicio", "@IndicadorBienOServicio");
            AddOptionalSet("IndicadorITBISIncluido", "@IndicadorITBISIncluido");
            AddOptionalSet("BaseImponible", "@BaseImponible");
            AddOptionalSet("MontoGravado", "@MontoGravado");
            AddOptionalSet("MontoExento", "@MontoExento");
            AddOptionalSet("SubtotalLineaAntesImpuesto", "@SubtotalLineaAntesImpuesto");

            var sql = $@"
UPDATE dbo.FacturaDet
SET {string.Join("," + Environment.NewLine + "    ", setParts)}
WHERE FacturaDetId = @det;";

            using var cmd = new SqlCommand(sql, cn, tx);

            cmd.Parameters.Add("@det", SqlDbType.Int).Value = facturaDetId;
            cmd.Parameters.Add("@desc", SqlDbType.NVarChar, 150).Value = descripcion;
            cmd.Parameters.Add("@uni", SqlDbType.VarChar, 10).Value = unidad;
            cmd.Parameters.Add("@impId", SqlDbType.Int).Value = (object?)impIdFinal ?? DBNull.Value;

            var pCant = cmd.Parameters.Add("@cant", SqlDbType.Decimal);
            pCant.Precision = 18; pCant.Scale = 2; pCant.Value = cantidad;

            var pUnit = cmd.Parameters.Add("@punit", SqlDbType.Decimal);
            pUnit.Precision = 18; pUnit.Scale = 2; pUnit.Value = precioUnitario;

            var pDpct = cmd.Parameters.Add("@dpct", SqlDbType.Decimal);
            pDpct.Precision = 9; pDpct.Scale = 4; pDpct.Value = descuentoPct;

            var pDmto = cmd.Parameters.Add("@dmto", SqlDbType.Decimal);
            pDmto.Precision = 18; pDmto.Scale = 2; pDmto.Value = descMonto;

            var pIpct = cmd.Parameters.Add("@ipct", SqlDbType.Decimal);
            pIpct.Precision = 9; pIpct.Scale = 4; pIpct.Value = impPctFinal;

            var pImto = cmd.Parameters.Add("@imto", SqlDbType.Decimal);
            pImto.Precision = 18; pImto.Scale = 2; pImto.Value = fiscal.ItbisMonto;

            var pTot = cmd.Parameters.Add("@tot", SqlDbType.Decimal);
            pTot.Precision = 18; pTot.Scale = 2; pTot.Value = fiscal.TotalLinea;

            var pPrecio = cmd.Parameters.Add("@precio", SqlDbType.Decimal);
            pPrecio.Precision = 18; pPrecio.Scale = 2; pPrecio.Value = precioUnitario;

            var pItbPct = cmd.Parameters.Add("@itbispct", SqlDbType.Decimal);
            pItbPct.Precision = 9; pItbPct.Scale = 4; pItbPct.Value = impPctFinal;

            var pItbMto = cmd.Parameters.Add("@itbismto", SqlDbType.Decimal);
            pItbMto.Precision = 18; pItbMto.Scale = 2; pItbMto.Value = fiscal.ItbisMonto;

            AddOrSetStringParameter(cmd, "@CodigoItemFiscal", fiscal.CodigoItemFiscal, 50);
            AddOrSetStringParameter(cmd, "@DescripcionFiscal", fiscal.DescripcionFiscal, 250, SqlDbType.NVarChar);
            AddOrSetStringParameter(cmd, "@UnidadMedidaCodigo", fiscal.UnidadMedidaCodigo, 10);
            AddOrSetIntParameter(cmd, "@UnidadMedidaECFId", fiscal.UnidadMedidaECFId);
            AddOrSetStringParameter(cmd, "@UnidadMedidaDGII", fiscal.UnidadMedidaDGII, 10);
            AddOrSetIntParameter(cmd, "@TipoProducto", fiscal.TipoProducto);
            AddOrSetIntParameter(cmd, "@IndicadorBienOServicio", fiscal.IndicadorBienOServicio);
            AddOrSetIntParameter(cmd, "@IndicadorITBISIncluido", fiscal.IndicadorITBISIncluido);

            AddOrSetDecimalParameter(cmd, "@BaseImponible", fiscal.BaseImponible);
            AddOrSetDecimalParameter(cmd, "@MontoGravado", fiscal.MontoGravado);
            AddOrSetDecimalParameter(cmd, "@MontoExento", fiscal.MontoExento);
            AddOrSetDecimalParameter(cmd, "@SubtotalLineaAntesImpuesto", fiscal.SubtotalLineaAntesImpuesto);

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
        public void SetDatosFiscalesBorrador(
            int facturaId,
            int tipoEcfId,
            int tipoIngresoId,
            int tipoPagoEcfId,
            bool esElectronica,
            string? encf,
            DateTime? fechaVencimientoSecuencia,
            int? indicadorMontoGravado,
            string? rncCompradorSnapshot,
            string? razonSocialCompradorSnapshot,
            string? correoCompradorSnapshot,
            string? direccionCompradorSnapshot,
            string? municipioCompradorSnapshot,
            string? provinciaCompradorSnapshot,
            decimal? montoGravadoTotal,
            decimal? montoExentoTotal,
            decimal? totalItbisRetenido,
            decimal? totalIsrRetencion,
            decimal? totalOtrosImpuestos,
            string? estadoEcf,
            int? tipoPagoEcfHeader,
            DateTime? fechaLimitePago,
            string? identificadorExtranjeroSnapshot)
        {
            if (facturaId <= 0) throw new ArgumentException("facturaId inválido.");

            using var cn = Db.GetOpenConnection();

            using var cmd = new SqlCommand(@"
UPDATE dbo.FacturaCab
SET TipoECFId = @TipoECFId,
    TipoIngresoId = @TipoIngresoId,
    TipoPagoECFId = @TipoPagoECFId,
    EsElectronica = @EsElectronica,
    eNCF = @eNCF,
    FechaVencimientoSecuencia = @FechaVencimientoSecuencia,
    IndicadorMontoGravado = @IndicadorMontoGravado,
    RncCompradorSnapshot = @RncCompradorSnapshot,
    RazonSocialCompradorSnapshot = @RazonSocialCompradorSnapshot,
    CorreoCompradorSnapshot = @CorreoCompradorSnapshot,
    DireccionCompradorSnapshot = @DireccionCompradorSnapshot,
    MunicipioCompradorSnapshot = @MunicipioCompradorSnapshot,
    ProvinciaCompradorSnapshot = @ProvinciaCompradorSnapshot,
    MontoGravadoTotal = @MontoGravadoTotal,
    MontoExentoTotal = @MontoExentoTotal,
    TotalITBISRetenido = @TotalITBISRetenido,
    TotalISRRetencion = @TotalISRRetencion,
    TotalOtrosImpuestos = @TotalOtrosImpuestos,
    EstadoECF = @EstadoECF,
    TipoPagoECFHeader = @TipoPagoECFHeader,
    FechaLimitePago = @FechaLimitePago,
    IdentificadorExtranjeroSnapshot = @IdentificadorExtranjeroSnapshot
WHERE FacturaId = @FacturaId
  AND Estado = 'BORRADOR';", cn);

            cmd.Parameters.Add("@FacturaId", SqlDbType.Int).Value = facturaId;
            cmd.Parameters.Add("@TipoECFId", SqlDbType.Int).Value = tipoEcfId;
            cmd.Parameters.Add("@TipoIngresoId", SqlDbType.Int).Value = tipoIngresoId;
            cmd.Parameters.Add("@TipoPagoECFId", SqlDbType.Int).Value = tipoPagoEcfId;
            cmd.Parameters.Add("@EsElectronica", SqlDbType.Bit).Value = esElectronica;
            cmd.Parameters.Add("@eNCF", SqlDbType.VarChar, 20).Value =
                string.IsNullOrWhiteSpace(encf) ? (object)DBNull.Value : encf.Trim();
            cmd.Parameters.Add("@FechaVencimientoSecuencia", SqlDbType.Date).Value =
                (object?)fechaVencimientoSecuencia ?? DBNull.Value;
            cmd.Parameters.Add("@IndicadorMontoGravado", SqlDbType.Int).Value =
                (object?)indicadorMontoGravado ?? DBNull.Value;

            cmd.Parameters.Add("@RncCompradorSnapshot", SqlDbType.VarChar, 20).Value =
                string.IsNullOrWhiteSpace(rncCompradorSnapshot) ? (object)DBNull.Value : rncCompradorSnapshot.Trim();
            cmd.Parameters.Add("@RazonSocialCompradorSnapshot", SqlDbType.NVarChar, 200).Value =
                string.IsNullOrWhiteSpace(razonSocialCompradorSnapshot) ? (object)DBNull.Value : razonSocialCompradorSnapshot.Trim();
            cmd.Parameters.Add("@CorreoCompradorSnapshot", SqlDbType.NVarChar, 200).Value =
                string.IsNullOrWhiteSpace(correoCompradorSnapshot) ? (object)DBNull.Value : correoCompradorSnapshot.Trim();
            cmd.Parameters.Add("@DireccionCompradorSnapshot", SqlDbType.NVarChar, 300).Value =
                string.IsNullOrWhiteSpace(direccionCompradorSnapshot) ? (object)DBNull.Value : direccionCompradorSnapshot.Trim();
            cmd.Parameters.Add("@MunicipioCompradorSnapshot", SqlDbType.NVarChar, 100).Value =
                string.IsNullOrWhiteSpace(municipioCompradorSnapshot) ? (object)DBNull.Value : municipioCompradorSnapshot.Trim();
            cmd.Parameters.Add("@ProvinciaCompradorSnapshot", SqlDbType.NVarChar, 100).Value =
                string.IsNullOrWhiteSpace(provinciaCompradorSnapshot) ? (object)DBNull.Value : provinciaCompradorSnapshot.Trim();

            var pMg = cmd.Parameters.Add("@MontoGravadoTotal", SqlDbType.Decimal);
            pMg.Precision = 18; pMg.Scale = 2; pMg.Value = (object?)montoGravadoTotal ?? DBNull.Value;

            var pMe = cmd.Parameters.Add("@MontoExentoTotal", SqlDbType.Decimal);
            pMe.Precision = 18; pMe.Scale = 2; pMe.Value = (object?)montoExentoTotal ?? DBNull.Value;

            var pItbisRet = cmd.Parameters.Add("@TotalITBISRetenido", SqlDbType.Decimal);
            pItbisRet.Precision = 18; pItbisRet.Scale = 2; pItbisRet.Value = (object?)totalItbisRetenido ?? DBNull.Value;

            var pIsrRet = cmd.Parameters.Add("@TotalISRRetencion", SqlDbType.Decimal);
            pIsrRet.Precision = 18; pIsrRet.Scale = 2; pIsrRet.Value = (object?)totalIsrRetencion ?? DBNull.Value;

            var pOtros = cmd.Parameters.Add("@TotalOtrosImpuestos", SqlDbType.Decimal);
            pOtros.Precision = 18; pOtros.Scale = 2; pOtros.Value = (object?)totalOtrosImpuestos ?? DBNull.Value;

            cmd.Parameters.Add("@EstadoECF", SqlDbType.VarChar, 20).Value =
                string.IsNullOrWhiteSpace(estadoEcf) ? (object)DBNull.Value : estadoEcf.Trim();
            cmd.Parameters.Add("@TipoPagoECFHeader", SqlDbType.Int).Value =
                (object?)tipoPagoEcfHeader ?? DBNull.Value;
            cmd.Parameters.Add("@FechaLimitePago", SqlDbType.Date).Value =
                (object?)fechaLimitePago ?? DBNull.Value;
            cmd.Parameters.Add("@IdentificadorExtranjeroSnapshot", SqlDbType.VarChar, 40).Value =
                string.IsNullOrWhiteSpace(identificadorExtranjeroSnapshot) ? (object)DBNull.Value : identificadorExtranjeroSnapshot.Trim();

            cmd.ExecuteNonQuery();
        }

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

                using (var cmdFiscal = new SqlCommand(@"
UPDATE dbo.FacturaCab
SET TipoPagoECFId = CASE
                        WHEN TipoPagoECFId IS NULL OR TipoPagoECFId = 0
                        THEN CASE WHEN @esCredito = 1 THEN 2 ELSE 1 END
                        ELSE TipoPagoECFId
                    END,
    TipoPagoECFHeader = CASE
                            WHEN TipoPagoECFHeader IS NULL OR TipoPagoECFHeader = 0
                            THEN CASE WHEN @esCredito = 1 THEN 2 ELSE 1 END
                            ELSE TipoPagoECFHeader
                        END,
    EsElectronica = CASE
                        WHEN eNCF IS NOT NULL AND LTRIM(RTRIM(eNCF)) <> '' THEN 1
                        ELSE ISNULL(EsElectronica, 0)
                    END,
    FechaVencimientoSecuencia = CASE
                                    WHEN FechaVencimientoSecuencia IS NULL AND eNCF IS NOT NULL AND LTRIM(RTRIM(eNCF)) <> ''
                                    THEN CONVERT(date, '2099-12-31')
                                    ELSE FechaVencimientoSecuencia
                                END,
    FechaLimitePago = CASE
                          WHEN @esCredito = 1 THEN ISNULL(@venc, FechaLimitePago)
                          ELSE FechaLimitePago
                      END,
    EstadoECF = CASE
                    WHEN eNCF IS NOT NULL AND LTRIM(RTRIM(eNCF)) <> '' AND (EstadoECF IS NULL OR LTRIM(RTRIM(EstadoECF)) = '')
                    THEN 'PENDIENTE'
                    ELSE EstadoECF
                END
WHERE FacturaId = @id;", cn, tx))
                {
                    cmdFiscal.Parameters.Add("@id", SqlDbType.Int).Value = facturaId;
                    cmdFiscal.Parameters.Add("@esCredito", SqlDbType.Bit).Value = esCredito;
                    cmdFiscal.Parameters.Add("@venc", SqlDbType.DateTime).Value = (object?)fechaVenc ?? DBNull.Value;
                    cmdFiscal.ExecuteNonQuery();
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

            // 1. Crear cabecera del movimiento
            var invMovId = InsertInvMovimientoCab(facturaId, usr, cn, tx, cache);

            // 2. Leer líneas con información completa
            var items = LeerLineasFacturaDetParaInvMov(facturaId, cn, tx, cache);

            if (items.Count == 0) return;

            // 3. Insertar líneas del movimiento (tipo VENTA = salida, cantidad negativa)
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

            var hasVen = HasColumn(cn, null, "dbo.FacturaCab", "CodVendedor");

            var sql = @"
SELECT TOP(1)
    FacturaId, TipoDocumento, NumeroDocumento, FechaDocumento, FechaVencimiento,
    ClienteId, NombreCliente, DocumentoCliente," + (hasDir ? " DireccionCliente," : "") + (hasVen ? " CodVendedor," : "") + @"
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

            if (hasVen)
            {
                dto.CodVendedor = rd.IsDBNull(i) ? null : rd.GetString(i);
                i++;
            }
            else dto.CodVendedor = null;


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

            var hasVen = HasColumn(cn, null, "dbo.FacturaCab", "CodVendedor");

            var sql = @"
SELECT
    FacturaId, TipoDocumento, NumeroDocumento, FechaDocumento, FechaVencimiento,
    ClienteId, NombreCliente, DocumentoCliente," + (hasDir ? " DireccionCliente," : "") + (hasVen ? " CodVendedor," : "") + @"
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

            if (hasVen)
            {
                dto.CodVendedor = rd.IsDBNull(i) ? null : rd.GetString(i);
                i++;
            }
            else dto.CodVendedor = null;


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
    

    // ============================================================
// ✅ e-CF: snapshot comprador + validación + XML base
// ============================================================
private void EjecutarSnapshotComprador(SqlConnection cn, SqlTransaction tx, int facturaId)
        {
            using var cmd = new SqlCommand("dbo.FacturaCab_ActualizarSnapshotComprador", cn, tx);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@FacturaId", SqlDbType.Int).Value = facturaId;
            cmd.ExecuteNonQuery();
        }

        private List<(string Tipo, string Campo, string Mensaje)> ValidarParaEcf(SqlConnection cn, SqlTransaction tx, int facturaId)
        {
            var lista = new List<(string Tipo, string Campo, string Mensaje)>();

            using var cmd = new SqlCommand("dbo.Factura_ValidarParaECF", cn, tx);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@FacturaId", SqlDbType.Int).Value = facturaId;

            using var rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                var tipo = rd["Tipo"] == DBNull.Value ? "" : Convert.ToString(rd["Tipo"]) ?? "";
                var campo = rd["Campo"] == DBNull.Value ? "" : Convert.ToString(rd["Campo"]) ?? "";
                var mensaje = rd["Mensaje"] == DBNull.Value ? "" : Convert.ToString(rd["Mensaje"]) ?? "";
                lista.Add((tipo, campo, mensaje));
            }

            return lista;
        }

        private string GenerarXmlBaseEcf(SqlConnection cn, SqlTransaction tx, int facturaId)
        {
            using var cmd = new SqlCommand("dbo.Factura_ECF_GenerarXmlBase", cn, tx);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@FacturaId", SqlDbType.Int).Value = facturaId;

            var obj = cmd.ExecuteScalar();
            return obj == null || obj == DBNull.Value ? string.Empty : Convert.ToString(obj) ?? string.Empty;
        }

        private (int TipoECFId, string CodigoTipoECF, string ENcf) ObtenerMetaEcf(SqlConnection cn, SqlTransaction tx, int facturaId)
        {
            using var cmd = new SqlCommand(@"
SELECT TOP (1)
    ISNULL(f.TipoECFId, 0) AS TipoECFId,
    CASE ISNULL(f.TipoECFId, 0)
        WHEN 1 THEN '31'
        WHEN 2 THEN '32'
        WHEN 3 THEN '33'
        WHEN 4 THEN '34'
        WHEN 5 THEN '41'
        WHEN 6 THEN '43'
        WHEN 7 THEN '44'
        WHEN 8 THEN '45'
        WHEN 9 THEN '46'
        WHEN 10 THEN '47'
        ELSE ''
    END AS CodigoTipoECF,
    ISNULL(f.eNCF, '') AS eNCF
FROM dbo.FacturaCab f
WHERE f.FacturaId = @FacturaId;", cn, tx);

            cmd.Parameters.Add("@FacturaId", SqlDbType.Int).Value = facturaId;

            using var rd = cmd.ExecuteReader();
            if (!rd.Read())
                throw new InvalidOperationException("No se pudo obtener metadata e-CF de la factura.");

            return
            (
                rd.IsDBNull(0) ? 0 : Convert.ToInt32(rd.GetValue(0)),
                rd.IsDBNull(1) ? "" : Convert.ToString(rd.GetValue(1)) ?? "",
                rd.IsDBNull(2) ? "" : Convert.ToString(rd.GetValue(2)) ?? ""
            );
        }

        private void GuardarXmlBaseEcf(SqlConnection cn, SqlTransaction tx, int facturaId, string xmlBase, string usuario = "SYSTEM")
        {
            if (string.IsNullOrWhiteSpace(xmlBase))
                throw new InvalidOperationException("El XML base e-CF está vacío.");

            var meta = ObtenerMetaEcf(cn, tx, facturaId);

            using var cmd = new SqlCommand(@"
IF EXISTS (SELECT 1 FROM dbo.ECFDocumentoFirmado WHERE FacturaId = @FacturaId)
BEGIN
    UPDATE dbo.ECFDocumentoFirmado
       SET TipoECFId = @TipoECFId,
           CodigoTipoECF = @CodigoTipoECF,
           eNCF = @eNCF,
           XmlBase = @XmlBase,
           EstadoFirma = ISNULL(EstadoFirma, 'PENDIENTE'),
           FechaActualizacion = SYSDATETIME(),
           UsuarioActualizacion = @Usuario
     WHERE FacturaId = @FacturaId;
END
ELSE
BEGIN
    INSERT INTO dbo.ECFDocumentoFirmado
    (
        FacturaId,
        TipoECFId,
        CodigoTipoECF,
        eNCF,
        XmlBase,
        EstadoFirma,
        IntentosFirma,
        Activo,
        FechaCreacion,
        UsuarioCreacion
    )
    VALUES
    (
        @FacturaId,
        @TipoECFId,
        @CodigoTipoECF,
        @eNCF,
        @XmlBase,
        'PENDIENTE',
        0,
        1,
        SYSDATETIME(),
        @Usuario
    );
END", cn, tx);

            cmd.Parameters.Add("@FacturaId", SqlDbType.Int).Value = facturaId;
            cmd.Parameters.Add("@TipoECFId", SqlDbType.Int).Value = meta.TipoECFId;
            cmd.Parameters.Add("@CodigoTipoECF", SqlDbType.VarChar, 5).Value =
                string.IsNullOrWhiteSpace(meta.CodigoTipoECF) ? (object)DBNull.Value : meta.CodigoTipoECF;
            cmd.Parameters.Add("@eNCF", SqlDbType.VarChar, 50).Value =
                string.IsNullOrWhiteSpace(meta.ENcf) ? (object)DBNull.Value : meta.ENcf;
            cmd.Parameters.Add("@XmlBase", SqlDbType.Xml).Value = xmlBase;
            cmd.Parameters.Add("@Usuario", SqlDbType.VarChar, 30).Value = usuario;
            cmd.ExecuteNonQuery();
        }

        private void GuardarDocumentoEcf(SqlConnection cn, SqlTransaction tx, int facturaId, string xmlBase, string usuario = "SYSTEM")
        {
            if (string.IsNullOrWhiteSpace(xmlBase))
                throw new InvalidOperationException("El XML base e-CF está vacío.");

            var meta = ObtenerMetaEcf(cn, tx, facturaId);

            using var cmd = new SqlCommand(@"
IF EXISTS (SELECT 1 FROM dbo.ECFDocumento WHERE FacturaId = @FacturaId)
BEGIN
    UPDATE dbo.ECFDocumento
       SET TipoECF = @TipoECFId,
           ENCF = @eNCF,
           XmlSinFirmar = @XmlBase,
           EstadoDGII = ISNULL(NULLIF(EstadoDGII,''), 'PENDIENTE'),
           EstadoProceso = ISNULL(NULLIF(EstadoProceso,''), 'PENDIENTE'),
           FechaGenerado = ISNULL(FechaGenerado, SYSDATETIME()),
           UltimoError = NULL
     WHERE FacturaId = @FacturaId;
END
ELSE
BEGIN
    INSERT INTO dbo.ECFDocumento
    (
        FacturaId,
        TipoECF,
        ENCF,
        EstadoDGII,
        XmlSinFirmar,
        FechaGenerado,
        IntentosEnvio,
        IntentosConsulta,
        EstadoProceso,
        Activo
    )
    VALUES
    (
        @FacturaId,
        @TipoECFId,
        @eNCF,
        'PENDIENTE',
        @XmlBase,
        SYSDATETIME(),
        0,
        0,
        'PENDIENTE',
        1
    );
END", cn, tx);

            cmd.Parameters.Add("@FacturaId", SqlDbType.Int).Value = facturaId;
            cmd.Parameters.Add("@TipoECFId", SqlDbType.Int).Value = meta.TipoECFId;
            cmd.Parameters.Add("@eNCF", SqlDbType.VarChar, 20).Value =
                string.IsNullOrWhiteSpace(meta.ENcf) ? (object)DBNull.Value : meta.ENcf;
            cmd.Parameters.Add("@XmlBase", SqlDbType.NVarChar, -1).Value = xmlBase;

            cmd.ExecuteNonQuery();
        }

        private void ProcesarEcfPostGuardado(SqlConnection cn, SqlTransaction tx, int facturaId, bool procesarEcf, string usuario = "SYSTEM")
        {
            if (!procesarEcf) return;

            EjecutarSnapshotComprador(cn, tx, facturaId);

            var validaciones = ValidarParaEcf(cn, tx, facturaId);
            var errores = validaciones
                .Where(x => string.Equals(x.Tipo, "ERROR", StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (errores.Count > 0)
            {
                var detalle = string.Join(Environment.NewLine, errores.Select(x => $"{x.Campo}: {x.Mensaje}"));
                throw new InvalidOperationException("La factura no pasó validación e-CF:" + Environment.NewLine + detalle);
            }

            var xmlBase = GenerarXmlBaseEcf(cn, tx, facturaId);
            GuardarXmlBaseEcf(cn, tx, facturaId, xmlBase, usuario);
            GuardarDocumentoEcf(cn, tx, facturaId, xmlBase, usuario);
        }

        public void ProcesarEcfFactura(int facturaId, string usuario = "SYSTEM")
        {
            if (facturaId <= 0)
                throw new ArgumentException("facturaId inválido.", nameof(facturaId));

            using var cn = Db.GetOpenConnection();
            using var tx = cn.BeginTransaction(IsolationLevel.ReadCommitted);

            try
            {
                ProcesarEcfPostGuardado(cn, tx, facturaId, true, usuario);
                tx.Commit();
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }
    }
}

