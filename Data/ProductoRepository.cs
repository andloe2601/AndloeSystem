using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using Andloe.Entidad;

namespace Andloe.Data
{
    public class ProductoRepository
    {
        // ===================== HELPERS =====================
        private static SqlParameter AddDec(SqlCommand cmd, string name, decimal? value, byte precision = 18, byte scale = 2)
        {
            var p = cmd.Parameters.Add(name, SqlDbType.Decimal);
            p.Precision = precision;
            p.Scale = scale;
            p.Value = (object?)value ?? DBNull.Value;
            return p;
        }

        private static string SqlQuote(string name) => "[" + name.Replace("]", "]]") + "]";

        private static HashSet<string> GetColumns(SqlConnection cn, string tablaFullName, SqlTransaction? tx = null)
        {
            var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            using var cmd = new SqlCommand(@"
SELECT c.name
FROM sys.columns c
WHERE c.object_id = OBJECT_ID(@t);", cn, tx);

            cmd.Parameters.Add("@t", SqlDbType.NVarChar, 200).Value = tablaFullName;

            using var rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                var n = rd.IsDBNull(0) ? "" : rd.GetString(0);
                if (!string.IsNullOrWhiteSpace(n))
                    set.Add(n);
            }

            return set;
        }

        private static string? DetectarColumna(SqlConnection cn, string tablaFullName, string[] candidatos, SqlTransaction? tx = null)
        {
            var cols = GetColumns(cn, tablaFullName, tx);

            foreach (var nombre in candidatos)
                if (cols.Contains(nombre))
                    return nombre;

            return null;
        }

        private static int GetDefaultEmpresaId(SqlConnection cn, SqlTransaction? tx = null)
        {
            var colId = DetectarColumna(cn, "dbo.Empresa", new[] { "EmpresaId", "Id", "Nº", "No", "Codigo" }, tx);
            if (string.IsNullOrWhiteSpace(colId))
                return 1;

            using var cmd = new SqlCommand($@"
SELECT TOP (1) {SqlQuote(colId)}
FROM dbo.Empresa
ORDER BY {SqlQuote(colId)};", cn, tx);

            var v = cmd.ExecuteScalar();
            return (v == null || v == DBNull.Value) ? 1 : Convert.ToInt32(v);
        }

        private static int GetDefaultAlmacenId(SqlConnection cn, SqlTransaction? tx = null)
        {
            var colId = DetectarColumna(cn, "dbo.Almacen", new[] { "AlmacenId", "Id", "Nº", "No", "Codigo" }, tx);
            if (string.IsNullOrWhiteSpace(colId))
                return 1;

            using var cmd = new SqlCommand($@"
SELECT TOP (1) {SqlQuote(colId)}
FROM dbo.Almacen
ORDER BY {SqlQuote(colId)};", cn, tx);

            var v = cmd.ExecuteScalar();
            return (v == null || v == DBNull.Value) ? 1 : Convert.ToInt32(v);
        }

        private static decimal ObtenerExistenciaActualInterno(string codigo, SqlConnection cn, SqlTransaction? tx = null)
        {
            using var cmd = new SqlCommand(@"
SELECT ISNULL(SUM(e.Cantidad), 0)
FROM dbo.Existencia e
WHERE e.ProductoCodigo = @Codigo;", cn, tx);

            cmd.Parameters.Add("@Codigo", SqlDbType.VarChar, 20).Value = codigo;
            var val = cmd.ExecuteScalar();
            return (val == null || val == DBNull.Value) ? 0m : Convert.ToDecimal(val);
        }

        private static void UpsertExistencia(
            string codigo,
            int empresaId,
            int almacenId,
            decimal delta,
            SqlConnection cn,
            SqlTransaction tx)
        {
            using var cmd = new SqlCommand(@"
MERGE dbo.Existencia AS target
USING (
    SELECT
        @EmpresaId     AS EmpresaId,
        @AlmacenId     AS AlmacenId,
        @ProductoCodigo AS ProductoCodigo
) AS source
ON  target.EmpresaId      = source.EmpresaId
AND target.AlmacenId      = source.AlmacenId
AND target.ProductoCodigo = source.ProductoCodigo

WHEN MATCHED THEN
    UPDATE SET Cantidad = ISNULL(target.Cantidad, 0) + @Delta

WHEN NOT MATCHED THEN
    INSERT (EmpresaId, AlmacenId, ProductoCodigo, Cantidad)
    VALUES (@EmpresaId, @AlmacenId, @ProductoCodigo, @Delta);", cn, tx);

            cmd.Parameters.Add("@EmpresaId", SqlDbType.Int).Value = empresaId;
            cmd.Parameters.Add("@AlmacenId", SqlDbType.Int).Value = almacenId;
            cmd.Parameters.Add("@ProductoCodigo", SqlDbType.VarChar, 20).Value = codigo;

            var pDelta = cmd.Parameters.Add("@Delta", SqlDbType.Decimal);
            pDelta.Precision = 18;
            pDelta.Scale = 2;
            pDelta.Value = delta;

            cmd.ExecuteNonQuery();
        }

        public string ObtenerUnidadPorCodigo(string productoCodigo, string unidadDefault = "UND")
        {
            if (string.IsNullOrWhiteSpace(productoCodigo))
                return unidadDefault;

            productoCodigo = productoCodigo.Trim();

            using var cn = Db.GetOpenConnection();

            const string colCodigo = "Nº";

            var colUnidad = DetectarColumna(cn, "dbo.Producto", new[]
            {
                "Unidad medida base",
                "UnidadMedidaCodigo",
                "Unidad medida venta",
                "Unidad medida compra"
            });

            if (string.IsNullOrWhiteSpace(colUnidad))
                return unidadDefault;

            var sql = $@"
SELECT TOP(1)
    NULLIF(LTRIM(RTRIM(CONVERT(nvarchar(50), {SqlQuote(colUnidad)}))), '')
FROM dbo.Producto
WHERE {SqlQuote(colCodigo)} = @cod;";

            using var cmd = new SqlCommand(sql, cn);
            cmd.Parameters.Add("@cod", SqlDbType.VarChar, 20).Value = productoCodigo;

            var val = cmd.ExecuteScalar();
            var unidad = (val == null || val == DBNull.Value) ? "" : Convert.ToString(val);
            unidad = (unidad ?? "").Trim();

            return string.IsNullOrWhiteSpace(unidad) ? unidadDefault : unidad;
        }

        // ===================== UNIDADES =====================
        public List<(string CodigoDGII, string Nombre)> ListarUnidadesActivas()
        {
            var list = new List<(string, string)>();

            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
SELECT CodigoDGII, Descripcion
FROM dbo.ECFUnidadMedida
WHERE Activo = 1
ORDER BY Descripcion;", cn);

            using var rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                list.Add((
                    rd.IsDBNull(0) ? "" : rd.GetString(0),
                    rd.IsDBNull(1) ? "" : rd.GetString(1)
                ));
            }

            return list;
        }

        public (int UnidadMedidaECFId, string CodigoDGII, string Descripcion)? ObtenerUnidadFiscalPorCodigo(string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo)) return null;
            codigo = codigo.Trim().ToUpperInvariant();

            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
SELECT TOP (1)
    UnidadMedidaECFId,
    CodigoDGII,
    Descripcion
FROM dbo.ECFUnidadMedida
WHERE Activo = 1
  AND UPPER(LTRIM(RTRIM(CodigoDGII))) = @Codigo
ORDER BY UnidadMedidaECFId;", cn);

            cmd.Parameters.Add("@Codigo", System.Data.SqlDbType.VarChar, 10).Value = codigo;

            using var rd = cmd.ExecuteReader();
            if (!rd.Read()) return null;

            return
            (
                rd.IsDBNull(0) ? 0 : Convert.ToInt32(rd.GetValue(0)),
                rd.IsDBNull(1) ? "" : Convert.ToString(rd.GetValue(1)) ?? "",
                rd.IsDBNull(2) ? "" : Convert.ToString(rd.GetValue(2)) ?? ""
            );
        }

        // ===================== PRODUCTO (lectura básica) =====================
        public Producto? ObtenerPorCodigo(string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo)) return null;
            codigo = codigo.Trim();

            using var cn = Db.GetOpenConnection();

            using var cmd = new SqlCommand(@"
SELECT
    p.ProductoId,                          -- 0
    p.[Nº]                        AS Codigo,                -- 1
    p.[Descripción]               AS Descripcion,           -- 2
    p.DescripcionFiscal,                                    -- 3
    p.[Descripción alias]         AS Referencia,            -- 4
    p.[Unidad medida base]        AS UnidadBase,            -- 5
    p.UnidadMedidaCodigo,                                   -- 6
    p.UnidadMedidaECFId,                                    -- 7
    um.CodigoDGII                AS UnidadMedidaECFId,      -- 8
    p.[Precio venta]              AS PrecioVenta,           -- 9
    p.[Precio coste]              AS PrecioCoste,           -- 10
    p.PrecioCompraPromedio,                                 -- 11
    p.PrecioIncluyeITBIS,                                  -- 12
    p.CodigoItemFiscal,                                    -- 13
    p.TipoProducto,                                        -- 14
    p.EsExento,                                            -- 15
    p.[Precio x Mayor]            AS PrecioMayor,          -- 16
    p.[Ultimo precio compra]      AS UltimoPrecioCompra,   -- 17
    ISNULL((
        SELECT SUM(e.Cantidad)
        FROM dbo.Existencia e
        WHERE e.ProductoCodigo = p.[Nº]
    ), 0)                          AS StockActual,          -- 18
    p.[% fijo beneficio]          AS PorcFijoBeneficio,    -- 19
    p.Estado,                                              -- 20
    p.CategoriaId,                                         -- 21
    p.SubcategoriaId,                                      -- 22
    p.ImpuestoId,                                          -- 23
    p.FechaCreacion                                        -- 24
FROM dbo.Producto p
LEFT JOIN dbo.ECFUnidadMedida um
    ON um.UnidadMedidaECFId = p.UnidadMedidaECFId
WHERE p.[Nº] = @c;", cn);

            cmd.Parameters.Add("@c", SqlDbType.VarChar, 20).Value = codigo;

            using var rd = cmd.ExecuteReader();
            if (!rd.Read()) return null;

            return new Producto
            {
                ProductoId = rd.IsDBNull(0) ? 0 : Convert.ToInt32(rd.GetValue(0)),
                Codigo = rd.IsDBNull(1) ? string.Empty : rd.GetString(1),
                Descripcion = rd.IsDBNull(2) ? string.Empty : rd.GetString(2),
                DescripcionFiscal = rd.IsDBNull(3) ? string.Empty : Convert.ToString(rd.GetValue(3)) ?? string.Empty,
                Referencia = rd.IsDBNull(4) ? string.Empty : Convert.ToString(rd.GetValue(4)) ?? string.Empty,
                UnidadBase = rd.IsDBNull(5) ? string.Empty : Convert.ToString(rd.GetValue(5)) ?? string.Empty,
                UnidadMedidaCodigo = rd.IsDBNull(6) ? string.Empty : Convert.ToString(rd.GetValue(6)) ?? string.Empty,
                UnidadMedidaECFId = rd.IsDBNull(7) ? (int?)null : Convert.ToInt32(rd.GetValue(7)),
                UnidadMedidaDGII = rd.IsDBNull(8) ? string.Empty : Convert.ToString(rd.GetValue(8)) ?? string.Empty,
                PrecioVenta = rd.IsDBNull(9) ? 0m : Convert.ToDecimal(rd.GetValue(9)),
                PrecioCoste = rd.IsDBNull(10) ? 0m : Convert.ToDecimal(rd.GetValue(10)),
                PrecioCompraPromedio = rd.IsDBNull(11) ? 0m : Convert.ToDecimal(rd.GetValue(11)),
                PrecioIncluyeITBIS = !rd.IsDBNull(12) && Convert.ToBoolean(rd.GetValue(12)),
                CodigoItemFiscal = rd.IsDBNull(13) ? string.Empty : Convert.ToString(rd.GetValue(13)) ?? string.Empty,
                TipoProducto = rd.IsDBNull(14) ? 1 : Convert.ToInt32(rd.GetValue(14)),
                EsExento = !rd.IsDBNull(15) && Convert.ToBoolean(rd.GetValue(15)),
                PrecioMayor = rd.IsDBNull(16) ? 0m : Convert.ToDecimal(rd.GetValue(16)),
                UltimoPrecioCompra = rd.IsDBNull(17) ? 0m : Convert.ToDecimal(rd.GetValue(17)),
                StockActual = rd.IsDBNull(18) ? 0m : Convert.ToDecimal(rd.GetValue(18)),
                PorcFijoBeneficio = rd.IsDBNull(19) ? 0m : Convert.ToDecimal(rd.GetValue(19)),
                Estado = rd.IsDBNull(20) ? 1 : Convert.ToInt32(rd.GetValue(20)),
                CategoriaId = rd.IsDBNull(21) ? (int?)null : Convert.ToInt32(rd.GetValue(21)),
                SubcategoriaId = rd.IsDBNull(22) ? (int?)null : Convert.ToInt32(rd.GetValue(22)),
                ImpuestoId = rd.IsDBNull(23) ? (int?)null : Convert.ToInt32(rd.GetValue(23)),
                FechaCreacion = rd.IsDBNull(24) ? (DateTime?)null : Convert.ToDateTime(rd.GetValue(24))
            };
        }

        /// <summary>
        /// Busca por código de producto [Nº], por [Cod_ Referencia] o por tabla dbo.CodBarras.[Cód_ barras].
        /// </summary>
        public Producto? ObtenerPorCodigoOBarras(string entrada)
        {
            if (string.IsNullOrWhiteSpace(entrada)) return null;
            entrada = entrada.Trim();

            var p = ObtenerPorCodigo(entrada);
            if (p != null) return p;

            using var cn = Db.GetOpenConnection();

            using (var cmd = new SqlCommand(@"
SELECT TOP 1 p.[Nº]
FROM dbo.CodBarras b
JOIN dbo.Producto p ON p.[Nº] = b.[Nº producto]
WHERE b.[Cód_ barras] = @x;", cn))
            {
                cmd.Parameters.Add("@x", SqlDbType.VarChar, 22).Value = entrada;
                var codigo = cmd.ExecuteScalar() as string;
                if (!string.IsNullOrWhiteSpace(codigo))
                    return ObtenerPorCodigo(codigo.Trim());
            }

            using (var cmd = new SqlCommand(@"
SELECT TOP 1 p.[Nº]
FROM dbo.Producto p
WHERE p.[Cod_ Referencia] = @x;", cn))
            {
                cmd.Parameters.Add("@x", SqlDbType.VarChar, 50).Value = entrada;
                var codigo = cmd.ExecuteScalar() as string;
                if (!string.IsNullOrWhiteSpace(codigo))
                    return ObtenerPorCodigo(codigo.Trim());
            }

            return null;
        }

        /// <summary>
        /// Devuelve item POS: Código, Descripción, PrecioUnit, ItbisPct (desde tabla Impuesto).
        /// </summary>
        public (string ProductoCodigo, string Descripcion, decimal PrecioUnit, decimal ItbisPct)? GetItemPOS(string entrada)
        {
            if (string.IsNullOrWhiteSpace(entrada)) return null;
            entrada = entrada.Trim();

            using var cn = Db.GetOpenConnection();

            using var cmd = new SqlCommand(@"
DECLARE @cod varchar(20);

SELECT TOP 1 @cod = p.[Nº]
FROM dbo.Producto p
WHERE p.[Nº] = @x;

IF (@cod IS NULL)
BEGIN
    SELECT TOP 1 @cod = p.[Nº]
    FROM dbo.CodBarras b
    JOIN dbo.Producto p ON p.[Nº] = b.[Nº producto]
    WHERE b.[Cód_ barras] = @x;
END

IF (@cod IS NULL)
BEGIN
    SELECT TOP 1 @cod = p.[Nº]
    FROM dbo.Producto p
    WHERE p.[Cod_ Referencia] = @x;
END

SELECT TOP 1
    p.[Nº] AS Codigo,
    COALESCE(NULLIF(p.[Descripción alias],''), p.[Descripción], p.[Nº]) AS Descripcion,
    CASE WHEN ISNULL(p.[Precio venta],0) > 0 THEN p.[Precio venta]
         ELSE ISNULL(p.[Precio coste],0)
    END AS PrecioUnit,
    ISNULL(i.Porcentaje, 0) AS ItbisPct
FROM dbo.Producto p
LEFT JOIN dbo.Impuesto i ON i.ImpuestoId = p.ImpuestoId
WHERE p.[Nº] = @cod;", cn);

            cmd.Parameters.Add("@x", SqlDbType.VarChar, 50).Value = entrada;

            using var rd = cmd.ExecuteReader();
            if (!rd.Read()) return null;

            var codigo = rd.GetString(0);
            var desc = rd.GetString(1);
            var precio = rd.IsDBNull(2) ? 0m : rd.GetDecimal(2);
            var itbis = rd.IsDBNull(3) ? 0m : rd.GetDecimal(3);

            return (codigo, desc, precio, itbis);
        }

        // ===================== STOCK =====================
        public decimal ObtenerStockActual(string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo)) return 0m;
            codigo = codigo.Trim();

            using var cn = Db.GetOpenConnection();
            return ObtenerExistenciaActualInterno(codigo, cn);
        }

        public decimal ObtenerStockActual(string codigo, SqlConnection cn, SqlTransaction tx)
        {
            if (string.IsNullOrWhiteSpace(codigo)) return 0m;
            codigo = codigo.Trim();

            return ObtenerExistenciaActualInterno(codigo, cn, tx);
        }

        public void RestarStock(string codigo, decimal cantidad, SqlConnection cn, SqlTransaction tx, bool permitirNegativo = false)
        {
            if (string.IsNullOrWhiteSpace(codigo)) throw new ArgumentException("codigo requerido.");
            codigo = codigo.Trim();

            if (cantidad <= 0) return;

            decimal stockActual = ObtenerExistenciaActualInterno(codigo, cn, tx);

            if (!permitirNegativo && stockActual < cantidad)
                throw new InvalidOperationException(
                    $"Stock insuficiente para {codigo}. Existencia: {stockActual:N2}, requerido: {cantidad:N2}.");

            var pendientes = cantidad;

            var filas = new List<(int EmpresaId, int AlmacenId, decimal Cantidad)>();
            using (var cmdSel = new SqlCommand(@"
SELECT e.EmpresaId, e.AlmacenId, ISNULL(e.Cantidad,0) AS Cantidad
FROM dbo.Existencia e WITH (UPDLOCK, ROWLOCK)
WHERE e.ProductoCodigo = @Codigo
  AND ISNULL(e.Cantidad,0) > 0
ORDER BY e.AlmacenId, e.EmpresaId;", cn, tx))
            {
                cmdSel.Parameters.Add("@Codigo", SqlDbType.VarChar, 20).Value = codigo;

                using var rd = cmdSel.ExecuteReader();
                while (rd.Read())
                {
                    filas.Add((
                        rd.GetInt32(0),
                        rd.GetInt32(1),
                        rd.IsDBNull(2) ? 0m : rd.GetDecimal(2)
                    ));
                }
            }

            foreach (var fila in filas)
            {
                if (pendientes <= 0) break;

                var descontar = Math.Min(fila.Cantidad, pendientes);
                if (descontar <= 0) continue;

                using var cmdUpd = new SqlCommand(@"
UPDATE dbo.Existencia
SET Cantidad = ISNULL(Cantidad,0) - @Cantidad
WHERE EmpresaId = @EmpresaId
  AND AlmacenId = @AlmacenId
  AND ProductoCodigo = @Codigo;", cn, tx);

                cmdUpd.Parameters.Add("@EmpresaId", SqlDbType.Int).Value = fila.EmpresaId;
                cmdUpd.Parameters.Add("@AlmacenId", SqlDbType.Int).Value = fila.AlmacenId;
                cmdUpd.Parameters.Add("@Codigo", SqlDbType.VarChar, 20).Value = codigo;

                var pCant = cmdUpd.Parameters.Add("@Cantidad", SqlDbType.Decimal);
                pCant.Precision = 18;
                pCant.Scale = 2;
                pCant.Value = descontar;

                cmdUpd.ExecuteNonQuery();
                pendientes -= descontar;
            }

            if (pendientes > 0)
            {
                if (!permitirNegativo)
                    throw new InvalidOperationException(
                        $"No fue posible completar la rebaja de stock para {codigo}. Pendiente: {pendientes:N2}.");

                var empresaId = GetDefaultEmpresaId(cn, tx);
                var almacenId = GetDefaultAlmacenId(cn, tx);

                UpsertExistencia(codigo, empresaId, almacenId, -pendientes, cn, tx);
            }
        }

        public void SumarStock(string codigo, decimal cantidad, SqlConnection cn, SqlTransaction tx)
        {
            if (string.IsNullOrWhiteSpace(codigo)) throw new ArgumentException("codigo requerido.");
            codigo = codigo.Trim();

            if (cantidad <= 0) return;

            var empresaId = GetDefaultEmpresaId(cn, tx);
            var almacenId = GetDefaultAlmacenId(cn, tx);

            UpsertExistencia(codigo, empresaId, almacenId, cantidad, cn, tx);
        }

        // ===================== PRODUCTO (INSERT) =====================
        public void InsertBasico(Producto p)
        {
            if (p == null) throw new ArgumentNullException(nameof(p));

            p.NormalizeDefaults();
            p.Validate();

            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
INSERT INTO dbo.Producto
(
    [Nº],
    [Descripción],
    Descripcion,
    DescripcionFiscal,
    [Descripción alias],
    [Unidad medida base],
    UnidadMedidaCodigo,
    UnidadMedidaECFId,
    [Precio venta],
    [Precio coste],
    [Precio x Mayor],
    PrecioCompraPromedio,
    PrecioIncluyeITBIS,
    CodigoItemFiscal,
    TipoProducto,
    EsExento,
    CategoriaId,
    SubcategoriaId,
    ImpuestoId,
    Estado,
    [% fijo beneficio],
    FechaCreacion
)
VALUES
(
    @cod,
    @desc,
    @desc,
    @descFiscal,
    @ref,
    @ub,
    @um,
    @umEcf,
    @pv,
    @pc,
    @pmay,
    @pcprom,
    @incl,
    @codFiscal,
    @tipoProd,
    @esExento,
    @cat,
    @sub,
    @imp,
    @est,
    @bf,
    GETDATE()
);", cn);

            cmd.Parameters.Add("@cod", SqlDbType.VarChar, 20).Value = p.Codigo;
            cmd.Parameters.Add("@desc", SqlDbType.NVarChar, 200).Value = (object?)p.Descripcion ?? DBNull.Value;
            cmd.Parameters.Add("@descFiscal", SqlDbType.NVarChar, 250).Value = (object?)p.DescripcionFiscal ?? DBNull.Value;
            cmd.Parameters.Add("@ref", SqlDbType.NVarChar, 80).Value = (object?)p.Referencia ?? DBNull.Value;

            cmd.Parameters.Add("@ub", SqlDbType.VarChar, 10).Value = (object?)p.UnidadBase ?? DBNull.Value;
            cmd.Parameters.Add("@um", SqlDbType.VarChar, 10).Value = (object?)p.UnidadMedidaCodigo ?? DBNull.Value;
            cmd.Parameters.Add("@umEcf", SqlDbType.Int).Value = (object?)p.UnidadMedidaECFId ?? DBNull.Value;

            cmd.Parameters.Add("@umDgii", SqlDbType.VarChar, 10).Value =
    string.IsNullOrWhiteSpace(p.UnidadMedidaDGII) ? (object)DBNull.Value : p.UnidadMedidaDGII;

            AddDec(cmd, "@pv", p.PrecioVenta);
            AddDec(cmd, "@pc", p.PrecioCoste);
            AddDec(cmd, "@pmay", p.PrecioMayor);
            AddDec(cmd, "@pcprom", p.PrecioCompraPromedio);
            AddDec(cmd, "@bf", p.PorcFijoBeneficio);

            cmd.Parameters.Add("@incl", SqlDbType.Bit).Value = p.PrecioIncluyeITBIS;
            cmd.Parameters.Add("@codFiscal", SqlDbType.NVarChar, 50).Value = (object?)p.CodigoItemFiscal ?? DBNull.Value;
            cmd.Parameters.Add("@tipoProd", SqlDbType.Int).Value = p.TipoProducto;
            cmd.Parameters.Add("@esExento", SqlDbType.Bit).Value = p.EsExento;
            cmd.Parameters.Add("@cat", SqlDbType.Int).Value = (object?)p.CategoriaId ?? DBNull.Value;
            cmd.Parameters.Add("@sub", SqlDbType.Int).Value = (object?)p.SubcategoriaId ?? DBNull.Value;
            cmd.Parameters.Add("@imp", SqlDbType.Int).Value = (object?)p.ImpuestoId ?? DBNull.Value;
            cmd.Parameters.Add("@est", SqlDbType.Int).Value = p.Estado;

            cmd.ExecuteNonQuery();
        }

        // ===================== PRODUCTO (UPDATE) =====================
        public void ActualizarBasico(Producto p)
        {
            if (p == null) throw new ArgumentNullException(nameof(p));

            p.NormalizeDefaults();
            p.Validate();

            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
UPDATE dbo.Producto
SET
    [Descripción]        = @desc,
    Descripcion          = @desc,
    DescripcionFiscal    = @descFiscal,
    [Descripción alias]  = @ref,
    [Unidad medida base] = @ub,
    UnidadMedidaCodigo   = @um,
    UnidadMedidaECFId    = @umEcf,
    [Precio venta]       = @pv,
    [Precio coste]       = @pc,
    [Precio x Mayor]     = @pmay,
    PrecioCompraPromedio = @pcprom,
    PrecioIncluyeITBIS   = @incl,
    CodigoItemFiscal     = @codFiscal,
    TipoProducto         = @tipoProd,
    EsExento             = @esExento,
    [% fijo beneficio]   = @bf,
    CategoriaId          = @cat,
    SubcategoriaId       = @sub,
    ImpuestoId           = @imp,
    Estado               = @est
WHERE [Nº] = @cod;", cn);

            cmd.Parameters.Add("@cod", SqlDbType.VarChar, 20).Value = p.Codigo;

            cmd.Parameters.Add("@desc", SqlDbType.NVarChar, 200).Value = (object?)p.Descripcion ?? DBNull.Value;
            cmd.Parameters.Add("@descFiscal", SqlDbType.NVarChar, 250).Value = (object?)p.DescripcionFiscal ?? DBNull.Value;
            cmd.Parameters.Add("@ref", SqlDbType.NVarChar, 80).Value = (object?)p.Referencia ?? DBNull.Value;

            cmd.Parameters.Add("@ub", SqlDbType.VarChar, 10).Value = (object?)p.UnidadBase ?? DBNull.Value;
            cmd.Parameters.Add("@um", SqlDbType.VarChar, 10).Value = (object?)p.UnidadMedidaCodigo ?? DBNull.Value;
            cmd.Parameters.Add("@umEcf", SqlDbType.Int).Value = (object?)p.UnidadMedidaECFId ?? DBNull.Value;

            AddDec(cmd, "@pv", p.PrecioVenta);
            AddDec(cmd, "@pc", p.PrecioCoste);
            AddDec(cmd, "@pmay", p.PrecioMayor);
            AddDec(cmd, "@pcprom", p.PrecioCompraPromedio);
            AddDec(cmd, "@bf", p.PorcFijoBeneficio);

            cmd.Parameters.Add("@incl", SqlDbType.Bit).Value = p.PrecioIncluyeITBIS;
            cmd.Parameters.Add("@codFiscal", SqlDbType.NVarChar, 50).Value = (object?)p.CodigoItemFiscal ?? DBNull.Value;
            cmd.Parameters.Add("@tipoProd", SqlDbType.Int).Value = p.TipoProducto;
            cmd.Parameters.Add("@esExento", SqlDbType.Bit).Value = p.EsExento;
            cmd.Parameters.Add("@cat", SqlDbType.Int).Value = (object?)p.CategoriaId ?? DBNull.Value;
            cmd.Parameters.Add("@sub", SqlDbType.Int).Value = (object?)p.SubcategoriaId ?? DBNull.Value;
            cmd.Parameters.Add("@imp", SqlDbType.Int).Value = (object?)p.ImpuestoId ?? DBNull.Value;
            cmd.Parameters.Add("@est", SqlDbType.Int).Value = p.Estado;

            var n = cmd.ExecuteNonQuery();
            if (n == 0)
                throw new InvalidOperationException("No se actualizó el producto. Verifica que el código exista.");
        }

        // ===================== LISTADO + ELIMINAR =====================
        public List<Producto> Listar(string? buscar, int top = 200)
        {
            var list = new List<Producto>();

            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
SELECT TOP (@top)
       p.[Nº]                 AS Codigo,
       p.[Descripción]        AS Descripcion,
       p.[Unidad medida base] AS UnidadBase,
       p.[Precio venta]       AS PrecioVenta,
       p.[Precio coste]       AS PrecioCoste,
       ISNULL((
            SELECT SUM(e.Cantidad)
            FROM dbo.Existencia e
            WHERE e.ProductoCodigo = p.[Nº]
       ), 0)                  AS StockActual,
       p.Estado
FROM dbo.Producto p
WHERE (@b IS NULL OR @b = '')
   OR (p.[Nº] LIKE @bx OR p.[Descripción] LIKE @bx)
ORDER BY p.[Descripción];", cn);

            cmd.Parameters.Add("@top", SqlDbType.Int).Value = top <= 0 ? 200 : top;

            var b = (buscar ?? string.Empty).Trim();
            cmd.Parameters.Add("@b", SqlDbType.NVarChar, 100).Value = string.IsNullOrEmpty(b) ? (object)DBNull.Value : b;
            cmd.Parameters.Add("@bx", SqlDbType.NVarChar, 100).Value = "%" + b + "%";

            using var rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                list.Add(new Producto
                {
                    Codigo = rd.GetString(0),
                    Descripcion = rd.IsDBNull(1) ? string.Empty : rd.GetString(1),
                    UnidadBase = rd.IsDBNull(2) ? string.Empty : rd.GetString(2),
                    PrecioVenta = rd.IsDBNull(3) ? 0m : rd.GetDecimal(3),
                    PrecioCoste = rd.IsDBNull(4) ? 0m : rd.GetDecimal(4),
                    StockActual = rd.IsDBNull(5) ? 0m : rd.GetDecimal(5),
                    Estado = rd.IsDBNull(6) ? 1 : rd.GetInt32(6)
                });
            }

            return list;
        }

        public void Eliminar(string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo)) throw new ArgumentException("codigo requerido.");
            codigo = codigo.Trim();

            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(
                "UPDATE dbo.Producto SET Estado = 0 WHERE [Nº]=@cod;", cn);

            cmd.Parameters.Add("@cod", SqlDbType.VarChar, 20).Value = codigo;

            var n = cmd.ExecuteNonQuery();
            if (n == 0)
                throw new InvalidOperationException("Producto no encontrado.");
        }

        public void EliminarFisico(string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo)) throw new ArgumentException("codigo requerido.");
            codigo = codigo.Trim();

            using var cn = Db.GetOpenConnection();
            using var tx = cn.BeginTransaction();

            try
            {
                using (var cmd1 = new SqlCommand(
                    "DELETE FROM dbo.CodBarras WHERE [Nº producto]=@cod;", cn, tx))
                {
                    cmd1.Parameters.Add("@cod", SqlDbType.VarChar, 20).Value = codigo;
                    cmd1.ExecuteNonQuery();
                }

                using (var cmdEx = new SqlCommand(
                    "DELETE FROM dbo.Existencia WHERE ProductoCodigo=@cod;", cn, tx))
                {
                    cmdEx.Parameters.Add("@cod", SqlDbType.VarChar, 20).Value = codigo;
                    cmdEx.ExecuteNonQuery();
                }

                using (var cmd2 = new SqlCommand(
                    "DELETE FROM dbo.Producto WHERE [Nº]=@cod;", cn, tx))
                {
                    cmd2.Parameters.Add("@cod", SqlDbType.VarChar, 20).Value = codigo;
                    var n = cmd2.ExecuteNonQuery();
                    if (n == 0)
                        throw new InvalidOperationException("Producto no encontrado.");
                }

                tx.Commit();
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }

        // ============== CÓDIGOS DE BARRA (CRUD parcial) ==============
        public string CrearCodigoBarraAuto(string codigoProd, string usuario)
        {
            if (string.IsNullOrWhiteSpace(codigoProd)) throw new ArgumentException("codigoProd requerido.");
            codigoProd = codigoProd.Trim();

            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand("dbo.sp_CodBarra_CrearAuto", cn)
            { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.Add("@NoProducto", SqlDbType.VarChar, 20).Value = codigoProd;
            cmd.Parameters.Add("@Usuario", SqlDbType.VarChar, 30).Value = (usuario ?? "").Trim();

            var pOut = cmd.Parameters.Add("@CodigoGenerado", SqlDbType.VarChar, 22);
            pOut.Direction = ParameterDirection.Output;

            cmd.ExecuteNonQuery();
            return Convert.ToString(pOut.Value) ?? "";
        }

        public void CrearCodigoBarraManual(string codigoBarras, string codigoProd, string? usuario)
        {
            if (string.IsNullOrWhiteSpace(codigoBarras)) throw new ArgumentException("codigoBarras requerido.");
            if (string.IsNullOrWhiteSpace(codigoProd)) throw new ArgumentException("codigoProd requerido.");

            codigoBarras = codigoBarras.Trim();
            codigoProd = codigoProd.Trim();

            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand("dbo.sp_CodBarra_AgregarManual", cn)
            { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.Add("@CodigoBarras", SqlDbType.VarChar, 22).Value = codigoBarras;
            cmd.Parameters.Add("@NoProducto", SqlDbType.VarChar, 20).Value = codigoProd;
            cmd.Parameters.Add("@Usuario", SqlDbType.VarChar, 30).Value = (object?)usuario?.Trim() ?? DBNull.Value;

            cmd.ExecuteNonQuery();
        }

        public List<CodigoBarra> ListarCodigosBarras(string codigoProd)
        {
            if (string.IsNullOrWhiteSpace(codigoProd)) return new List<CodigoBarra>();
            codigoProd = codigoProd.Trim();

            var list = new List<CodigoBarra>();
            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
SELECT [Cód_ barras],[Nº producto],[Tipo],Usuario,[Ultima fecha utilización]
FROM dbo.CodBarras
WHERE [Nº producto]=@p
ORDER BY [Ultima fecha utilización] DESC;", cn);

            cmd.Parameters.Add("@p", SqlDbType.VarChar, 20).Value = codigoProd;

            using var rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                list.Add(new CodigoBarra
                {
                    CodigoBarras = rd.GetString(0),
                    NoProducto = rd.GetString(1),
                    Tipo = rd.IsDBNull(2) ? 0 : rd.GetInt32(2),
                    Usuario = rd.IsDBNull(3) ? null : rd.GetString(3),
                    UltimaFechaUtilizacion = rd.IsDBNull(4) ? (DateTime?)null : rd.GetDateTime(4)
                });
            }
            return list;
        }

        public void EliminarCodigoBarra(string codigoBarras)
        {
            if (string.IsNullOrWhiteSpace(codigoBarras)) throw new ArgumentException("codigoBarras requerido.");
            codigoBarras = codigoBarras.Trim();

            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(
                "DELETE FROM dbo.CodBarras WHERE [Cód_ barras]=@c;", cn);

            cmd.Parameters.Add("@c", SqlDbType.VarChar, 22).Value = codigoBarras;
            cmd.ExecuteNonQuery();
        }

        // =========================
        //   LISTADO PARA PROMOS (con ITBIS real)
        // =========================
        public List<ProductoListaDto> ListarParaPromo(string? filtro = null, int top = 200)
        {
            var list = new List<ProductoListaDto>();

            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
SELECT TOP(@top)
       p.[Nº]           AS Codigo,
       p.[Descripción]  AS Descripcion,
       p.[Precio venta] AS PrecioVenta,
       ISNULL(i.Porcentaje,0) AS ItbisPct
FROM dbo.Producto p
LEFT JOIN dbo.Impuesto i ON i.ImpuestoId = p.ImpuestoId
WHERE (@filtro IS NULL
       OR p.[Nº]          LIKE @like
       OR p.[Descripción] LIKE @like)
ORDER BY p.[Descripción];", cn);

            cmd.Parameters.Add("@top", SqlDbType.Int).Value = top <= 0 ? 200 : top;

            if (string.IsNullOrWhiteSpace(filtro))
            {
                cmd.Parameters.Add("@filtro", SqlDbType.NVarChar, 100).Value = DBNull.Value;
                cmd.Parameters.Add("@like", SqlDbType.NVarChar, 100).Value = DBNull.Value;
            }
            else
            {
                filtro = filtro.Trim();
                cmd.Parameters.Add("@filtro", SqlDbType.NVarChar, 100).Value = filtro;
                cmd.Parameters.Add("@like", SqlDbType.NVarChar, 100).Value = "%" + filtro + "%";
            }

            using var rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                list.Add(new ProductoListaDto
                {
                    Codigo = rd.GetString(0),
                    Descripcion = rd.GetString(1),
                    PrecioVenta = rd.IsDBNull(2) ? 0m : rd.GetDecimal(2),
                    ItbisPct = rd.IsDBNull(3) ? 0m : rd.GetDecimal(3)
                });
            }

            return list;
        }

        // DTO simple para grilla de códigos de barra
        public class CodigoBarra
        {
            public string? CodigoBarras { get; set; }
            public string? NoProducto { get; set; }
            public int Tipo { get; set; }
            public string? Usuario { get; set; }
            public DateTime? UltimaFechaUtilizacion { get; set; }
        }
    

    public byte[]? ObtenerImagen(string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo))
                return null;

            codigo = codigo.Trim();

            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
SELECT TOP (1) Imagen
FROM dbo.Producto
WHERE [Nº] = @cod;", cn);

            cmd.Parameters.Add("@cod", SqlDbType.VarChar, 20).Value = codigo;

            var val = cmd.ExecuteScalar();
            if (val == null || val == DBNull.Value)
                return null;

            return (byte[])val;
        }

        public void GuardarImagen(string codigo, byte[]? imagenBytes)
        {
            if (string.IsNullOrWhiteSpace(codigo))
                throw new ArgumentException("codigo requerido.");

            codigo = codigo.Trim();

            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
UPDATE dbo.Producto
SET Imagen = @img
WHERE [Nº] = @cod;", cn);

            cmd.Parameters.Add("@cod", SqlDbType.VarChar, 20).Value = codigo;
            cmd.Parameters.Add("@img", SqlDbType.VarBinary, -1).Value =
                (object?)imagenBytes ?? DBNull.Value;

            var n = cmd.ExecuteNonQuery();
            if (n == 0)
                throw new InvalidOperationException("No se pudo guardar la imagen del producto.");
        }

        public void QuitarImagen(string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo))
                throw new ArgumentException("codigo requerido.");

            codigo = codigo.Trim();

            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
UPDATE dbo.Producto
SET Imagen = NULL
WHERE [Nº] = @cod;", cn);

            cmd.Parameters.Add("@cod", SqlDbType.VarChar, 20).Value = codigo;
            cmd.ExecuteNonQuery();
        }
    }
}