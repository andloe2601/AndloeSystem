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

        private static HashSet<string> GetColumns(SqlConnection cn, string tablaFullName)
        {
            var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            using var cmd = new SqlCommand(@"
SELECT c.name
FROM sys.columns c
WHERE c.object_id = OBJECT_ID(@t);", cn);

            cmd.Parameters.Add("@t", SqlDbType.NVarChar, 200).Value = tablaFullName;

            using var rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                var n = rd.IsDBNull(0) ? "" : rd.GetString(0);
                if (!string.IsNullOrWhiteSpace(n)) set.Add(n);
            }
            return set;
        }

        private static string? DetectarColumna(SqlConnection cn, string tablaFullName, string[] candidatos)
        {
            // ✅ Optimización: lee columnas 1 vez
            var cols = GetColumns(cn, tablaFullName);

            foreach (var nombre in candidatos)
                if (cols.Contains(nombre))
                    return nombre;

            return null;
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
        public List<(string Codigo, string Nombre)> ListarUnidadesActivas()
        {
            var list = new List<(string, string)>();

            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(
                "SELECT Codigo,Nombre FROM dbo.UnidadMedida WHERE Estado=1 ORDER BY Nombre", cn);

            using var rd = cmd.ExecuteReader();
            while (rd.Read())
                list.Add((rd.GetString(0), rd.GetString(1)));

            return list;
        }

        // ===================== PRODUCTO (lectura básica) =====================
        public Producto? ObtenerPorCodigo(string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo)) return null;
            codigo = codigo.Trim();

            using var cn = Db.GetOpenConnection();

            // Nota: aquí asumes que estas columnas existen. Si tu tabla cambia, esto revienta.
            using var cmd = new SqlCommand(@"
SELECT
    p.[Nº]                 AS Codigo,
    p.[Descripción]        AS Descripcion,
    p.[Descripción alias]  AS Referencia,
    p.[Unidad medida base] AS UnidadBase,
    p.UnidadMedidaCodigo   AS UnidadMedidaCodigo,

    p.[Precio venta]       AS PrecioVenta,
    p.[Precio coste]       AS PrecioCoste,
    p.[Precio compra promedio] AS PrecioCompraPromedio,

    p.[Precio x Mayor]         AS PrecioMayor,
    p.[Ultimo precio compra]   AS UltimoPrecioCompra,

    p.[Cantidad presupuesto]   AS StockActual,
    p.[% fijo beneficio]       AS PorcFijoBeneficio,
    p.Estado,
    p.CategoriaId,
    p.SubcategoriaId,
    p.ImpuestoId
FROM dbo.Producto p
WHERE p.[Nº] = @c;", cn);

            cmd.Parameters.Add("@c", SqlDbType.VarChar, 20).Value = codigo;

            using var rd = cmd.ExecuteReader();
            if (!rd.Read()) return null;

            return new Producto
            {
                Codigo = rd.GetString(0),
                Descripcion = rd.IsDBNull(1) ? null : rd.GetString(1),
                Referencia = rd.IsDBNull(2) ? null : rd.GetString(2),
                UnidadBase = rd.IsDBNull(3) ? null : rd.GetString(3),
                UnidadMedidaCodigo = rd.IsDBNull(4) ? null : rd.GetString(4),

                PrecioVenta = rd.IsDBNull(5) ? 0 : rd.GetDecimal(5),
                PrecioCoste = rd.IsDBNull(6) ? (decimal?)null : rd.GetDecimal(6),
                PrecioCompraPromedio = rd.IsDBNull(7) ? (decimal?)null : rd.GetDecimal(7),

                PrecioMayor = rd.IsDBNull(8) ? (decimal?)null : rd.GetDecimal(8),
                UltimoPrecioCompra = rd.IsDBNull(9) ? (decimal?)null : rd.GetDecimal(9),

                StockActual = rd.IsDBNull(10) ? 0 : rd.GetDecimal(10),
                PorcFijoBeneficio = rd.IsDBNull(11) ? (decimal?)null : rd.GetDecimal(11),
                Estado = rd.IsDBNull(12) ? 1 : Convert.ToInt32(rd.GetValue(12)),
                CategoriaId = rd.IsDBNull(13) ? (int?)null : rd.GetInt32(13),
                SubcategoriaId = rd.IsDBNull(14) ? (int?)null : rd.GetInt32(14),
                ImpuestoId = rd.IsDBNull(15) ? (int?)null : rd.GetInt32(15),
            };
        }

        /// <summary>
        /// Busca por código de producto [Nº], por [Cod_ Referencia] o por tabla dbo.CodBarras.[Cód_ barras].
        /// </summary>
        public Producto? ObtenerPorCodigoOBarras(string entrada)
        {
            if (string.IsNullOrWhiteSpace(entrada)) return null;
            entrada = entrada.Trim();

            // 1) Por [Nº]
            var p = ObtenerPorCodigo(entrada);
            if (p != null) return p;

            using var cn = Db.GetOpenConnection();

            // 2) Por CodBarras -> Producto
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

            // 3) Por [Cod_ Referencia]
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

-- 1) por Nº
SELECT TOP 1 @cod = p.[Nº]
FROM dbo.Producto p
WHERE p.[Nº] = @x;

-- 2) por barra
IF (@cod IS NULL)
BEGIN
    SELECT TOP 1 @cod = p.[Nº]
    FROM dbo.CodBarras b
    JOIN dbo.Producto p ON p.[Nº] = b.[Nº producto]
    WHERE b.[Cód_ barras] = @x;
END

-- 3) por Cod_ Referencia
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
            using var cmd = new SqlCommand(@"
SELECT ISNULL([Cantidad presupuesto], 0)
FROM dbo.Producto
WHERE [Nº] = @Codigo;", cn);

            cmd.Parameters.Add("@Codigo", SqlDbType.VarChar, 20).Value = codigo;

            var val = cmd.ExecuteScalar();
            return (val == null || val == DBNull.Value) ? 0m : Convert.ToDecimal(val);
        }

        public decimal ObtenerStockActual(string codigo, SqlConnection cn, SqlTransaction tx)
        {
            if (string.IsNullOrWhiteSpace(codigo)) return 0m;
            codigo = codigo.Trim();

            using var cmd = new SqlCommand(@"
SELECT ISNULL([Cantidad presupuesto], 0)
FROM dbo.Producto
WHERE [Nº] = @Codigo;", cn, tx);

            cmd.Parameters.Add("@Codigo", SqlDbType.VarChar, 20).Value = codigo;

            var val = cmd.ExecuteScalar();
            return (val == null || val == DBNull.Value) ? 0m : Convert.ToDecimal(val);
        }

        public void RestarStock(string codigo, decimal cantidad, SqlConnection cn, SqlTransaction tx, bool permitirNegativo = false)
        {
            if (string.IsNullOrWhiteSpace(codigo)) throw new ArgumentException("codigo requerido.");
            codigo = codigo.Trim();

            if (cantidad <= 0) return;

            decimal stockActual;

            using (var cmdSel = new SqlCommand(@"
SELECT ISNULL([Cantidad presupuesto], 0)
FROM dbo.Producto WITH (UPDLOCK, ROWLOCK)
WHERE [Nº] = @Codigo;", cn, tx))
            {
                cmdSel.Parameters.Add("@Codigo", SqlDbType.VarChar, 20).Value = codigo;

                var val = cmdSel.ExecuteScalar();
                if (val == null || val == DBNull.Value)
                    throw new InvalidOperationException($"Producto {codigo} no existe en la tabla Producto.");

                stockActual = Convert.ToDecimal(val);
            }

            if (!permitirNegativo && stockActual < cantidad)
                throw new InvalidOperationException(
                    $"Stock insuficiente para {codigo}. Existencia: {stockActual:N2}, requerido: {cantidad:N2}.");

            using (var cmdUpd = new SqlCommand(@"
UPDATE dbo.Producto
SET [Cantidad presupuesto] = [Cantidad presupuesto] - @Cantidad
WHERE [Nº] = @Codigo;", cn, tx))
            {
                cmdUpd.Parameters.Add("@Codigo", SqlDbType.VarChar, 20).Value = codigo;

                var pCant = cmdUpd.Parameters.Add("@Cantidad", SqlDbType.Decimal);
                pCant.Precision = 18; pCant.Scale = 2; pCant.Value = cantidad;

                cmdUpd.ExecuteNonQuery();
            }
        }

        public void SumarStock(string codigo, decimal cantidad, SqlConnection cn, SqlTransaction tx)
        {
            if (string.IsNullOrWhiteSpace(codigo)) throw new ArgumentException("codigo requerido.");
            codigo = codigo.Trim();

            if (cantidad <= 0) return;

            using var cmd = new SqlCommand(@"
UPDATE dbo.Producto
SET [Cantidad presupuesto] = [Cantidad presupuesto] + @Cantidad
WHERE [Nº] = @Codigo;", cn, tx);

            cmd.Parameters.Add("@Codigo", SqlDbType.VarChar, 20).Value = codigo;

            var pCant = cmd.Parameters.Add("@Cantidad", SqlDbType.Decimal);
            pCant.Precision = 18; pCant.Scale = 2; pCant.Value = cantidad;

            cmd.ExecuteNonQuery();
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
    [Descripción alias],
    [Unidad medida base],
    UnidadMedidaCodigo,
    [Precio venta],
    [Precio coste],
    [Precio x Mayor],
    CategoriaId,
    SubcategoriaId,
    ImpuestoId,
    Estado,
    [% fijo beneficio]
)
VALUES
(
    @cod,
    @desc,
    @ref,
    @ub,
    @um,
    @pv,
    @pc,
    @pmay,
    @cat,
    @sub,
    @imp,
    @est,
    @bf
);", cn);

            cmd.Parameters.Add("@cod", SqlDbType.VarChar, 20).Value = p.Codigo;
            cmd.Parameters.Add("@desc", SqlDbType.NVarChar, 60).Value = (object?)p.Descripcion ?? DBNull.Value;
            cmd.Parameters.Add("@ref", SqlDbType.NVarChar, 80).Value = (object?)p.Referencia ?? DBNull.Value;

            cmd.Parameters.Add("@ub", SqlDbType.VarChar, 10).Value = (object?)p.UnidadBase ?? DBNull.Value;
            cmd.Parameters.Add("@um", SqlDbType.VarChar, 10).Value = (object?)p.UnidadMedidaCodigo ?? DBNull.Value;

            AddDec(cmd, "@pv", p.PrecioVenta);
            AddDec(cmd, "@pc", p.PrecioCoste);
            AddDec(cmd, "@pmay", p.PrecioMayor);
            AddDec(cmd, "@bf", p.PorcFijoBeneficio);

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

            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
UPDATE dbo.Producto
SET
    [Descripción]        = @desc,
    [Descripción alias]  = @ref,
    [Unidad medida base] = @ub,
    UnidadMedidaCodigo   = @um,
    [Precio venta]       = @pv,
    [Precio coste]       = @pc,
    [Precio x Mayor]     = @pmay,
    [% fijo beneficio]   = @bf,
    CategoriaId          = @cat,
    SubcategoriaId       = @sub,
    ImpuestoId           = @imp,
    Estado               = @est
WHERE [Nº] = @cod;", cn);

            cmd.Parameters.Add("@cod", SqlDbType.VarChar, 20).Value = p.Codigo;

            cmd.Parameters.Add("@desc", SqlDbType.NVarChar, 60).Value = (object?)p.Descripcion ?? DBNull.Value;
            cmd.Parameters.Add("@ref", SqlDbType.NVarChar, 80).Value = (object?)p.Referencia ?? DBNull.Value;

            cmd.Parameters.Add("@ub", SqlDbType.VarChar, 10).Value = (object?)p.UnidadBase ?? DBNull.Value;
            cmd.Parameters.Add("@um", SqlDbType.VarChar, 10).Value = (object?)p.UnidadMedidaCodigo ?? DBNull.Value;

            AddDec(cmd, "@pv", p.PrecioVenta);
            AddDec(cmd, "@pc", p.PrecioCoste);
            AddDec(cmd, "@pmay", p.PrecioMayor);
            AddDec(cmd, "@bf", p.PorcFijoBeneficio);

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
       p.[Nº]                   AS Codigo,
       p.[Descripción]          AS Descripcion,
       p.[Unidad medida base]   AS UnidadBase,
       p.[Precio venta]         AS PrecioVenta,
       p.[Precio coste]         AS PrecioCoste,
       p.[Cantidad presupuesto] AS StockActual,
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
                    Descripcion = rd.IsDBNull(1) ? null : rd.GetString(1),
                    UnidadBase = rd.IsDBNull(2) ? null : rd.GetString(2),
                    PrecioVenta = rd.IsDBNull(3) ? 0 : rd.GetDecimal(3),
                    PrecioCoste = rd.IsDBNull(4) ? (decimal?)null : rd.GetDecimal(4),
                    StockActual = rd.IsDBNull(5) ? 0 : rd.GetDecimal(5),
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
    }
}
