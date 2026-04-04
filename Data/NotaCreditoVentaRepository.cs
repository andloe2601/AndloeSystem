using Andloe.Entidad;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;

namespace Andloe.Data
{
    public sealed class NotaCreditoVentaRepository
    {
        private static bool HasColumn(SqlConnection cn, string tableFullName, string columnName, SqlTransaction? tx = null)
        {
            using var cmd = new SqlCommand(@"
SELECT 1
FROM sys.columns
WHERE object_id = OBJECT_ID(@t)
  AND name = @c;", cn, tx);

            cmd.Parameters.Add("@t", SqlDbType.NVarChar, 200).Value = tableFullName;
            cmd.Parameters.Add("@c", SqlDbType.NVarChar, 128).Value = columnName;

            return cmd.ExecuteScalar() != null;
        }

        private static string BuildVentaCabSelectSql(SqlConnection cn)
        {
            return @"
SELECT TOP (1)
    v.VentaId,
    v.NoDocumento,
    v.Fecha,
    ISNULL(v.ClienteCodigo,'') AS ClienteCodigo,
    ISNULL(v.NombreCliente,'') AS ClienteNombre,
    NULLIF(LTRIM(RTRIM(CONVERT(varchar(20), c.RNC_Cedula))), '') AS DocumentoCliente,
    ISNULL(v.MonedaCodigo, 'DOP') AS MonedaCodigo,
    ISNULL(v.TasaCambio, 1) AS TasaCambio,
    ISNULL(v.TotalMoneda,0) AS TotalMoneda,
    ISNULL(v.Estado,'') AS Estado,
    " + (HasColumn(cn, "dbo.VentaCab", "EmpresaId") ? "ISNULL(v.EmpresaId,0)" : "CAST(0 AS int)") + @" AS EmpresaId,
    " + (HasColumn(cn, "dbo.VentaCab", "SucursalId") ? "ISNULL(v.SucursalId,0)" : "CAST(0 AS int)") + @" AS SucursalId,
    " + (HasColumn(cn, "dbo.VentaCab", "AlmacenId") ? "ISNULL(v.AlmacenId,0)" : "CAST(0 AS int)") + @" AS AlmacenId,
    " + (HasColumn(cn, "dbo.VentaCab", "eNCF") ? "NULLIF(LTRIM(RTRIM(v.eNCF)), '')" : "CAST(NULL AS varchar(50))") + @" AS ENcfOrigen
FROM dbo.VentaCab v
LEFT JOIN dbo.Cliente c ON c.[Código] = v.ClienteCodigo
WHERE v.VentaId = @VentaId;";
        }

        public List<VentaOrigenDto> BuscarVentas(string? filtro, int top = 100)
        {
            var list = new List<VentaOrigenDto>();

            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
SELECT TOP (@Top)
    v.VentaId,
    v.NoDocumento,
    v.Fecha,
    ISNULL(v.ClienteCodigo,'') AS ClienteCodigo,
    ISNULL(v.NombreCliente,'') AS ClienteNombre,
    NULLIF(LTRIM(RTRIM(CONVERT(varchar(20), c.RNC_Cedula))), '') AS DocumentoCliente,
    ISNULL(v.MonedaCodigo,'DOP') AS MonedaCodigo,
    ISNULL(v.TasaCambio,1) AS TasaCambio,
    ISNULL(v.TotalMoneda,0) AS TotalMoneda,
    ISNULL(v.Estado,'') AS Estado
FROM dbo.VentaCab v
LEFT JOIN dbo.Cliente c ON c.[Código] = v.ClienteCodigo
WHERE (@Filtro IS NULL OR @Filtro = ''
       OR v.NoDocumento LIKE @Like
       OR ISNULL(v.NombreCliente,'') LIKE @Like
       OR ISNULL(v.ClienteCodigo,'') LIKE @Like
       OR CONVERT(varchar(20), v.VentaId) = @Filtro)
ORDER BY v.VentaId DESC;", cn);

            cmd.Parameters.Add("@Top", SqlDbType.Int).Value = top <= 0 ? 100 : top;

            var f = (filtro ?? "").Trim();
            cmd.Parameters.Add("@Filtro", SqlDbType.NVarChar, 120).Value =
                string.IsNullOrWhiteSpace(f) ? DBNull.Value : (object)f;
            cmd.Parameters.Add("@Like", SqlDbType.NVarChar, 120).Value = "%" + f + "%";

            using var rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                list.Add(new VentaOrigenDto
                {
                    VentaId = rd.GetInt64(0),
                    NoDocumento = rd.IsDBNull(1) ? "" : rd.GetString(1),
                    Fecha = rd.IsDBNull(2) ? DateTime.Now : rd.GetDateTime(2),
                    ClienteCodigo = rd.IsDBNull(3) ? "" : rd.GetString(3),
                    ClienteNombre = rd.IsDBNull(4) ? "" : rd.GetString(4),
                    DocumentoCliente = rd.IsDBNull(5) ? null : rd.GetString(5),
                    MonedaCodigo = rd.IsDBNull(6) ? "DOP" : rd.GetString(6),
                    TasaCambio = rd.IsDBNull(7) ? 1m : Convert.ToDecimal(rd.GetValue(7), CultureInfo.InvariantCulture),
                    TotalMoneda = rd.IsDBNull(8) ? 0m : Convert.ToDecimal(rd.GetValue(8), CultureInfo.InvariantCulture),
                    Estado = rd.IsDBNull(9) ? "" : rd.GetString(9)
                });
            }

            return list;
        }

        public NotaCreditoVentaDto? CargarVentaOrigen(long ventaId)
        {
            if (ventaId <= 0) return null;

            using var cn = Db.GetOpenConnection();

            using var cmdCab = new SqlCommand(BuildVentaCabSelectSql(cn), cn);
            cmdCab.Parameters.Add("@VentaId", SqlDbType.BigInt).Value = ventaId;

            using var rdCab = cmdCab.ExecuteReader();
            if (!rdCab.Read()) return null;

            var dto = new NotaCreditoVentaDto
            {
                Cab = new NotaCreditoVentaCabDto
                {
                    VentaIdOrigen = ventaId,
                    Fecha = DateTime.Now,
                    NoDocumentoOrigen = rdCab.IsDBNull(1) ? "" : rdCab.GetString(1),
                    ClienteCodigo = rdCab.IsDBNull(3) ? "" : rdCab.GetString(3),
                    ClienteNombre = rdCab.IsDBNull(4) ? "" : rdCab.GetString(4),
                    DocumentoCliente = rdCab.IsDBNull(5) ? null : rdCab.GetString(5),
                    MonedaCodigo = rdCab.IsDBNull(6) ? "DOP" : rdCab.GetString(6),
                    TasaCambio = rdCab.IsDBNull(7) ? 1m : Convert.ToDecimal(rdCab.GetValue(7), CultureInfo.InvariantCulture),
                    TotalMoneda = rdCab.IsDBNull(8) ? 0m : Convert.ToDecimal(rdCab.GetValue(8), CultureInfo.InvariantCulture),
                    Estado = "EMITIDO",
                    EmpresaId = rdCab.IsDBNull(10) ? 0 : Convert.ToInt32(rdCab.GetValue(10), CultureInfo.InvariantCulture),
                    SucursalId = rdCab.IsDBNull(11) ? 0 : Convert.ToInt32(rdCab.GetValue(11), CultureInfo.InvariantCulture),
                    AlmacenId = rdCab.IsDBNull(12) ? 0 : Convert.ToInt32(rdCab.GetValue(12), CultureInfo.InvariantCulture),
                    ENcfOrigen = rdCab.IsDBNull(13) ? null : rdCab.GetString(13),
                }
            };

            rdCab.Close();

            using var cmdLin = new SqlCommand(@"
SELECT
    ISNULL(vl.Linea, 0) AS Linea,
    ISNULL(vl.ProductoCodigo,'') AS ProductoCodigo,
    ISNULL(p.[Descripción], ISNULL(vl.ProductoCodigo,'')) AS Descripcion,
    ISNULL(vl.Cantidad,0) AS Cantidad,
    ISNULL(vl.PrecioUnit,0) AS PrecioUnit,
    ISNULL(vl.ItbisPorc,0) AS ItbisPct,
    ISNULL(vl.DescuentoMoneda,0) AS DescuentoMoneda,
    ISNULL(vl.ImporteMoneda, ISNULL(vl.TotalMoneda,0) - ISNULL(vl.ItbisMoneda,0)) AS SubTotalMoneda,
    ISNULL(vl.ItbisMoneda,0) AS ItbisMoneda,
    ISNULL(vl.TotalMoneda,0) AS TotalMoneda
FROM dbo.VentaLin vl
LEFT JOIN dbo.Producto p ON p.[Nº] = vl.ProductoCodigo
WHERE vl.VentaId = @VentaId
ORDER BY vl.Linea;", cn);

            cmdLin.Parameters.Add("@VentaId", SqlDbType.BigInt).Value = ventaId;

            using var rdLin = cmdLin.ExecuteReader();
            while (rdLin.Read())
            {
                var cant = rdLin.IsDBNull(3) ? 0m : Convert.ToDecimal(rdLin.GetValue(3), CultureInfo.InvariantCulture);

                dto.Lineas.Add(new NotaCreditoVentaLinDto
                {
                    Seleccionado = true,
                    Linea = rdLin.GetInt32(0),
                    ProductoCodigo = rdLin.IsDBNull(1) ? "" : rdLin.GetString(1),
                    Descripcion = rdLin.IsDBNull(2) ? "" : rdLin.GetString(2),
                    CantidadOriginal = cant,
                    CantidadNC = cant,
                    PrecioUnit = rdLin.IsDBNull(4) ? 0m : Convert.ToDecimal(rdLin.GetValue(4), CultureInfo.InvariantCulture),
                    ItbisPct = rdLin.IsDBNull(5) ? 0m : Convert.ToDecimal(rdLin.GetValue(5), CultureInfo.InvariantCulture),
                    DescuentoMoneda = rdLin.IsDBNull(6) ? 0m : Convert.ToDecimal(rdLin.GetValue(6), CultureInfo.InvariantCulture),
                    SubTotalMoneda = rdLin.IsDBNull(7) ? 0m : Convert.ToDecimal(rdLin.GetValue(7), CultureInfo.InvariantCulture),
                    ItbisMoneda = rdLin.IsDBNull(8) ? 0m : Convert.ToDecimal(rdLin.GetValue(8), CultureInfo.InvariantCulture),
                    TotalMoneda = rdLin.IsDBNull(9) ? 0m : Convert.ToDecimal(rdLin.GetValue(9), CultureInfo.InvariantCulture),
                });
            }

            return dto;
        }

        public List<NcfTipoDto> ListarTiposNcfNotaCredito()
        {
            var list = new List<NcfTipoDto>();

            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
SELECT TipoId, Codigo, Descripcion
FROM dbo.NCF_Tipo
WHERE Codigo LIKE 'E%'
  AND (
        UPPER(Descripcion) LIKE '%CREDITO%'
        OR UPPER(Descripcion) LIKE '%NOTA%'
      )
ORDER BY Codigo;", cn);

            using var rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                list.Add(new NcfTipoDto
                {
                    TipoId = rd.GetInt32(0),
                    Codigo = rd.IsDBNull(1) ? "" : rd.GetString(1),
                    Descripcion = rd.IsDBNull(2) ? "" : rd.GetString(2)
                });
            }

            return list;
        }

        private static void ReponerExistencia(string productoCodigo, decimal cantidad, int empresaId, int almacenId, SqlConnection cn, SqlTransaction tx)
        {
            using var cmd = new SqlCommand(@"
MERGE dbo.Existencia AS target
USING (
    SELECT
        @EmpresaId AS EmpresaId,
        @AlmacenId AS AlmacenId,
        @ProductoCodigo AS ProductoCodigo
) AS source
ON target.EmpresaId = source.EmpresaId
AND target.AlmacenId = source.AlmacenId
AND target.ProductoCodigo = source.ProductoCodigo
WHEN MATCHED THEN
    UPDATE SET Cantidad = ISNULL(target.Cantidad,0) + @Cantidad
WHEN NOT MATCHED THEN
    INSERT (EmpresaId, AlmacenId, ProductoCodigo, Cantidad)
    VALUES (@EmpresaId, @AlmacenId, @ProductoCodigo, @Cantidad);", cn, tx);

            cmd.Parameters.Add("@EmpresaId", SqlDbType.Int).Value = empresaId;
            cmd.Parameters.Add("@AlmacenId", SqlDbType.Int).Value = almacenId;
            cmd.Parameters.Add("@ProductoCodigo", SqlDbType.VarChar, 20).Value = productoCodigo;

            var pCant = cmd.Parameters.Add("@Cantidad", SqlDbType.Decimal);
            pCant.Precision = 18;
            pCant.Scale = 4;
            pCant.Value = cantidad;

            cmd.ExecuteNonQuery();
        }

        private (string Prefijo, int Longitud, int Actual, string Codigo)? ObtenerNumeradorNcV(SqlConnection cn, SqlTransaction? tx = null)
        {
            using var cmd = new SqlCommand(@"
IF OBJECT_ID('dbo.NumeradorSecuencia') IS NULL
BEGIN
    SELECT CAST(NULL AS varchar(20)) AS Codigo,
           CAST(NULL AS varchar(20)) AS Prefijo,
           CAST(NULL AS int) AS Longitud,
           CAST(NULL AS int) AS Actual
    WHERE 1 = 0;
    RETURN;
END

SELECT TOP (1)
    Codigo,
    ISNULL(Prefijo,'') AS Prefijo,
    ISNULL(Longitud,0) AS Longitud,
    ISNULL(Actual,0) AS Actual
FROM dbo.NumeradorSecuencia
WHERE Codigo IN ('NCV','NC','NOTACREDITO','NCR','CRNOTE')
ORDER BY CASE Codigo
            WHEN 'NCV' THEN 1
            WHEN 'NC' THEN 2
            WHEN 'NOTACREDITO' THEN 3
            WHEN 'NCR' THEN 4
            ELSE 5
         END;", cn, tx);

            using var rd = cmd.ExecuteReader();
            if (!rd.Read()) return null;

            return (
                rd.IsDBNull(1) ? "" : rd.GetString(1),
                rd.IsDBNull(2) ? 0 : Convert.ToInt32(rd.GetValue(2)),
                rd.IsDBNull(3) ? 0 : Convert.ToInt32(rd.GetValue(3)),
                rd.IsDBNull(0) ? "" : rd.GetString(0)
            );
        }

        public string ObtenerProximoNoDocumentoNcPreview()
        {
            using var cn = Db.GetOpenConnection();

            var num = ObtenerNumeradorNcV(cn);
            if (!num.HasValue)
                return "NC-" + DateTime.Now.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture);

            var prefijo = string.IsNullOrWhiteSpace(num.Value.Prefijo) ? "NC" : num.Value.Prefijo.Trim();
            var longitud = num.Value.Longitud <= 0 ? 8 : num.Value.Longitud;
            var actual = num.Value.Actual < 0 ? 0 : num.Value.Actual;

            var siguiente = actual + 1;
            return prefijo + siguiente.ToString().PadLeft(longitud, '0');
        }

        private string GenerarNoDocumentoNc(SqlConnection cn, SqlTransaction tx)
        {
            var num = ObtenerNumeradorNcV(cn, tx);
            if (!num.HasValue)
                return "NC-" + DateTime.Now.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture);

            var prefijo = string.IsNullOrWhiteSpace(num.Value.Prefijo) ? "NC" : num.Value.Prefijo.Trim();
            var longitud = num.Value.Longitud <= 0 ? 8 : num.Value.Longitud;
            var actual = num.Value.Actual < 0 ? 0 : num.Value.Actual;
            var codigo = num.Value.Codigo;

            var siguiente = actual + 1;

            using (var cmdUpd = new SqlCommand(@"
UPDATE dbo.NumeradorSecuencia
SET Actual = @Actual
WHERE Codigo = @Codigo;", cn, tx))
            {
                cmdUpd.Parameters.Add("@Actual", SqlDbType.Int).Value = siguiente;
                cmdUpd.Parameters.Add("@Codigo", SqlDbType.VarChar, 20).Value = codigo;
                cmdUpd.ExecuteNonQuery();
            }

            return prefijo + siguiente.ToString().PadLeft(longitud, '0');
        }

        public long CrearNotaCredito(NotaCreditoVentaDto dto, string usuario)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            if (dto.Cab.VentaIdOrigen.GetValueOrDefault() <= 0) throw new InvalidOperationException("Debe seleccionar una venta origen.");
            if (string.IsNullOrWhiteSpace(dto.Cab.ClienteCodigo)) throw new InvalidOperationException("Cliente requerido.");

            var lineas = new List<NotaCreditoVentaLinDto>();

            foreach (var ln in dto.Lineas)
            {
                if (!ln.Seleccionado || ln.CantidadNC <= 0) continue;

                if (ln.CantidadNC > ln.CantidadOriginal)
                    throw new InvalidOperationException($"La cantidad NC no puede exceder la cantidad original. Producto: {ln.ProductoCodigo}.");

                var factor = ln.CantidadOriginal <= 0 ? 0m : (ln.CantidadNC / ln.CantidadOriginal);
                var sub = Math.Round(ln.SubTotalMoneda * factor, 2, MidpointRounding.AwayFromZero);
                var itb = Math.Round(ln.ItbisMoneda * factor, 2, MidpointRounding.AwayFromZero);
                var tot = Math.Round(sub + itb, 2, MidpointRounding.AwayFromZero);
                var dtoMon = Math.Round(ln.DescuentoMoneda * factor, 2, MidpointRounding.AwayFromZero);

                lineas.Add(new NotaCreditoVentaLinDto
                {
                    Seleccionado = true,
                    Linea = lineas.Count + 1,
                    ProductoCodigo = ln.ProductoCodigo,
                    Descripcion = ln.Descripcion,
                    CantidadOriginal = ln.CantidadOriginal,
                    CantidadNC = ln.CantidadNC,
                    PrecioUnit = ln.PrecioUnit,
                    ItbisPct = ln.ItbisPct,
                    DescuentoMoneda = dtoMon,
                    SubTotalMoneda = sub,
                    ItbisMoneda = itb,
                    TotalMoneda = tot
                });
            }

            if (lineas.Count == 0)
                throw new InvalidOperationException("Debe indicar al menos una línea con cantidad > 0.");

            decimal subTotal = 0m, itbis = 0m, total = 0m;
            foreach (var x in lineas)
            {
                subTotal += x.SubTotalMoneda;
                itbis += x.ItbisMoneda;
                total += x.TotalMoneda;
            }

            subTotal = Math.Round(subTotal, 2, MidpointRounding.AwayFromZero);
            itbis = Math.Round(itbis, 2, MidpointRounding.AwayFromZero);
            total = Math.Round(total, 2, MidpointRounding.AwayFromZero);

            using var cn = Db.GetOpenConnection();
            using var tx = cn.BeginTransaction();

            try
            {
                var noDocumento = GenerarNoDocumentoNc(cn, tx);

                long ncId;
                using (var cmdCab = new SqlCommand(@"
DECLARE @Out table (NCId bigint);

INSERT INTO dbo.NotaCreditoCab
(
    NoDocumento,
    Fecha,
    VentaIdOrigen,
    ClienteCodigo,
    MonedaCodigo,
    TasaCambio,
    SubTotalMoneda,
    ItbisMoneda,
    TotalMoneda,
    Usuario,
    Estado,
    FechaCreacion,
    MovimientoIdConta
)
OUTPUT inserted.NCId INTO @Out(NCId)
VALUES
(
    @NoDocumento,
    SYSDATETIME(),
    @VentaIdOrigen,
    @ClienteCodigo,
    @MonedaCodigo,
    @TasaCambio,
    @SubTotalMoneda,
    @ItbisMoneda,
    @TotalMoneda,
    @Usuario,
    'EMITIDO',
    SYSDATETIME(),
    NULL
);

SELECT MAX(NCId) FROM @Out;", cn, tx))
                {
                    cmdCab.Parameters.Add("@NoDocumento", SqlDbType.VarChar, 30).Value = noDocumento;
                    cmdCab.Parameters.Add("@VentaIdOrigen", SqlDbType.BigInt).Value = dto.Cab.VentaIdOrigen!.Value;
                    cmdCab.Parameters.Add("@ClienteCodigo", SqlDbType.VarChar, 20).Value = dto.Cab.ClienteCodigo.Trim();
                    cmdCab.Parameters.Add("@MonedaCodigo", SqlDbType.VarChar, 3).Value =
                        string.IsNullOrWhiteSpace(dto.Cab.MonedaCodigo) ? "DOP" : dto.Cab.MonedaCodigo.Trim();

                    var pTc = cmdCab.Parameters.Add("@TasaCambio", SqlDbType.Decimal);
                    pTc.Precision = 18;
                    pTc.Scale = 6;
                    pTc.Value = dto.Cab.TasaCambio <= 0 ? 1m : dto.Cab.TasaCambio;

                    var pSub = cmdCab.Parameters.Add("@SubTotalMoneda", SqlDbType.Decimal);
                    pSub.Precision = 18;
                    pSub.Scale = 2;
                    pSub.Value = subTotal;

                    var pItb = cmdCab.Parameters.Add("@ItbisMoneda", SqlDbType.Decimal);
                    pItb.Precision = 18;
                    pItb.Scale = 2;
                    pItb.Value = itbis;

                    var pTot = cmdCab.Parameters.Add("@TotalMoneda", SqlDbType.Decimal);
                    pTot.Precision = 18;
                    pTot.Scale = 2;
                    pTot.Value = total;

                    cmdCab.Parameters.Add("@Usuario", SqlDbType.NVarChar, 200).Value =
                        string.IsNullOrWhiteSpace(usuario) ? DBNull.Value : (object)usuario.Trim();

                    ncId = Convert.ToInt64(cmdCab.ExecuteScalar(), CultureInfo.InvariantCulture);
                }

                foreach (var ln in lineas)
                {
                    using (var cmd = new SqlCommand(@"
INSERT INTO dbo.NotaCreditoLin
(
    NCId,
    Linea,
    ProductoCodigo,
    Cantidad,
    PrecioUnit,
    ItbisPct,
    DescuentoMoneda,
    SubTotalMoneda,
    ItbisMoneda,
    TotalMoneda
)
VALUES
(
    @NCId,
    @Linea,
    @ProductoCodigo,
    @Cantidad,
    @PrecioUnit,
    @ItbisPct,
    @DescuentoMoneda,
    @SubTotalMoneda,
    @ItbisMoneda,
    @TotalMoneda
);", cn, tx))
                    {
                        cmd.Parameters.Add("@NCId", SqlDbType.BigInt).Value = ncId;
                        cmd.Parameters.Add("@Linea", SqlDbType.Int).Value = ln.Linea;
                        cmd.Parameters.Add("@ProductoCodigo", SqlDbType.VarChar, 20).Value =
                            string.IsNullOrWhiteSpace(ln.ProductoCodigo) ? DBNull.Value : (object)ln.ProductoCodigo.Trim();

                        var pCant = cmd.Parameters.Add("@Cantidad", SqlDbType.Decimal);
                        pCant.Precision = 18;
                        pCant.Scale = 4;
                        pCant.Value = ln.CantidadNC;

                        var pPre = cmd.Parameters.Add("@PrecioUnit", SqlDbType.Decimal);
                        pPre.Precision = 18;
                        pPre.Scale = 2;
                        pPre.Value = ln.PrecioUnit;

                        var pPct = cmd.Parameters.Add("@ItbisPct", SqlDbType.Decimal);
                        pPct.Precision = 9;
                        pPct.Scale = 4;
                        pPct.Value = ln.ItbisPct;

                        var pDto = cmd.Parameters.Add("@DescuentoMoneda", SqlDbType.Decimal);
                        pDto.Precision = 18;
                        pDto.Scale = 2;
                        pDto.Value = ln.DescuentoMoneda;

                        var pSub = cmd.Parameters.Add("@SubTotalMoneda", SqlDbType.Decimal);
                        pSub.Precision = 18;
                        pSub.Scale = 2;
                        pSub.Value = ln.SubTotalMoneda;

                        var pItb = cmd.Parameters.Add("@ItbisMoneda", SqlDbType.Decimal);
                        pItb.Precision = 18;
                        pItb.Scale = 2;
                        pItb.Value = ln.ItbisMoneda;

                        var pTot = cmd.Parameters.Add("@TotalMoneda", SqlDbType.Decimal);
                        pTot.Precision = 18;
                        pTot.Scale = 2;
                        pTot.Value = ln.TotalMoneda;

                        cmd.ExecuteNonQuery();
                    }

                    if (!string.IsNullOrWhiteSpace(ln.ProductoCodigo)
                        && ln.CantidadNC > 0
                        && dto.Cab.EmpresaId > 0
                        && dto.Cab.AlmacenId > 0)
                    {
                        ReponerExistencia(ln.ProductoCodigo.Trim(), ln.CantidadNC, dto.Cab.EmpresaId, dto.Cab.AlmacenId, cn, tx);
                    }
                }

                tx.Commit();
                return ncId;
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }

        public string GenerarENcf(long ncId, int tipoId, int empresaId, int sucursalId, int? cajaId, string usuario)
        {
            if (ncId <= 0) throw new ArgumentException("NCId inválido.", nameof(ncId));
            if (tipoId <= 0) throw new ArgumentException("TipoId inválido.", nameof(tipoId));
            if (empresaId <= 0) throw new ArgumentException("EmpresaId inválido.", nameof(empresaId));
            if (sucursalId <= 0) throw new ArgumentException("SucursalId inválido.", nameof(sucursalId));

            using var cn = Db.GetOpenConnection();
            using var tx = cn.BeginTransaction(IsolationLevel.Serializable);

            try
            {
                string? codigoTipo;
                string? descripcionTipo;

                using (var cmdTipo = new SqlCommand(@"
SELECT Codigo, Descripcion
FROM dbo.NCF_Tipo WITH (UPDLOCK, HOLDLOCK)
WHERE TipoId = @TipoId;", cn, tx))
                {
                    cmdTipo.Parameters.Add("@TipoId", SqlDbType.Int).Value = tipoId;

                    using var rdTipo = cmdTipo.ExecuteReader();
                    if (!rdTipo.Read())
                        throw new InvalidOperationException("No existe el TipoId en NCF_Tipo.");

                    codigoTipo = rdTipo.IsDBNull(0) ? null : rdTipo.GetString(0);
                    descripcionTipo = rdTipo.IsDBNull(1) ? null : rdTipo.GetString(1);
                }

                if (string.IsNullOrWhiteSpace(codigoTipo))
                    throw new InvalidOperationException("NCF_Tipo sin código válido.");

                if (!codigoTipo.StartsWith("E", StringComparison.OrdinalIgnoreCase))
                    throw new InvalidOperationException("El tipo seleccionado no es un e-NCF.");

                if (string.IsNullOrWhiteSpace(descripcionTipo) ||
                    (!descripcionTipo.ToUpperInvariant().Contains("CREDITO") &&
                     !descripcionTipo.ToUpperInvariant().Contains("NOTA")))
                {
                    throw new InvalidOperationException("El tipo seleccionado no corresponde a una Nota de Crédito electrónica.");
                }

                using (var cmdExiste = new SqlCommand(@"
SELECT TOP(1) e.NCF
FROM dbo.NCF_Emision e WITH (UPDLOCK, HOLDLOCK)
WHERE e.Origen = 'NC_VENTA'
  AND e.OrigenId = @NCId;", cn, tx))
                {
                    cmdExiste.Parameters.Add("@NCId", SqlDbType.BigInt).Value = ncId;
                    var ya = Convert.ToString(cmdExiste.ExecuteScalar());
                    if (!string.IsNullOrWhiteSpace(ya))
                    {
                        tx.Commit();
                        return ya!;
                    }
                }

                int rangoId;
                long secDesde;
                long secHasta;

                using (var cmdRango = new SqlCommand(@"
SELECT TOP (1)
    r.RangoId,
    r.SecuenciaDesde,
    r.SecuenciaHasta
FROM dbo.NCF_Rango r WITH (UPDLOCK, HOLDLOCK)
WHERE r.EmpresaId = @EmpresaId
  AND r.SucursalId = @SucursalId
  AND (r.CajaId = @CajaId OR r.CajaId IS NULL)
  AND r.TipoId = @TipoId
  AND r.Prefijo = @Prefijo
  AND CONVERT(date, GETDATE()) BETWEEN r.VigenciaDesde AND r.VigenciaHasta
ORDER BY CASE WHEN r.CajaId IS NULL THEN 1 ELSE 0 END, r.RangoId DESC;", cn, tx))
                {
                    cmdRango.Parameters.Add("@EmpresaId", SqlDbType.Int).Value = empresaId;
                    cmdRango.Parameters.Add("@SucursalId", SqlDbType.Int).Value = sucursalId;
                    cmdRango.Parameters.Add("@CajaId", SqlDbType.Int).Value = (object?)cajaId ?? DBNull.Value;
                    cmdRango.Parameters.Add("@TipoId", SqlDbType.Int).Value = tipoId;
                    cmdRango.Parameters.Add("@Prefijo", SqlDbType.VarChar, 4).Value = codigoTipo.Trim().ToUpperInvariant();

                    using var rd = cmdRango.ExecuteReader();
                    if (!rd.Read())
                        throw new InvalidOperationException("No existe NCF_Rango válido para Empresa/Sucursal/Caja/Tipo y vigencia.");

                    rangoId = rd.GetInt32(0);
                    secDesde = Convert.ToInt64(rd.GetValue(1), CultureInfo.InvariantCulture);
                    secHasta = Convert.ToInt64(rd.GetValue(2), CultureInfo.InvariantCulture);
                }

                long secActual;
                using (var cmdMax = new SqlCommand(@"
SELECT ISNULL(MAX(Secuencia), 0)
FROM dbo.NCF_Emision WITH (UPDLOCK, HOLDLOCK)
WHERE RangoId = @RangoId;", cn, tx))
                {
                    cmdMax.Parameters.Add("@RangoId", SqlDbType.Int).Value = rangoId;
                    secActual = Convert.ToInt64(cmdMax.ExecuteScalar(), CultureInfo.InvariantCulture);

                    if (secActual < secDesde - 1)
                        secActual = secDesde - 1;
                }

                var siguiente = secActual + 1;
                if (siguiente < secDesde || siguiente > secHasta)
                    throw new InvalidOperationException("El rango NCF seleccionado está agotado.");

                var encf = codigoTipo.Trim().ToUpperInvariant() + siguiente.ToString().PadLeft(8, '0');

                using (var cmdIns = new SqlCommand(@"
INSERT INTO dbo.NCF_Emision
(
    TipoId,
    RangoId,
    Secuencia,
    NCF,
    Fecha,
    Origen,
    OrigenId,
    Estado
)
VALUES
(
    @TipoId,
    @RangoId,
    @Secuencia,
    @NCF,
    SYSDATETIME(),
    'NC_VENTA',
    @OrigenId,
    'EMITIDO'
);", cn, tx))
                {
                    cmdIns.Parameters.Add("@TipoId", SqlDbType.Int).Value = tipoId;
                    cmdIns.Parameters.Add("@RangoId", SqlDbType.Int).Value = rangoId;
                    cmdIns.Parameters.Add("@Secuencia", SqlDbType.BigInt).Value = siguiente;
                    cmdIns.Parameters.Add("@NCF", SqlDbType.VarChar, 20).Value = encf;
                    cmdIns.Parameters.Add("@OrigenId", SqlDbType.BigInt).Value = ncId;
                    cmdIns.ExecuteNonQuery();
                }

                tx.Commit();
                return encf;
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }

        public NotaCreditoVentaDto? Obtener(long ncId)
        {
            if (ncId <= 0) return null;

            using var cn = Db.GetOpenConnection();

            using var cmdCab = new SqlCommand(@"
SELECT TOP (1)
    nc.NCId,
    nc.NoDocumento,
    nc.Fecha,
    nc.VentaIdOrigen,
    nc.ClienteCodigo,
    ISNULL(c.[Nombre], '') AS ClienteNombre,
    NULLIF(LTRIM(RTRIM(CONVERT(varchar(20), c.RNC_Cedula))), '') AS DocumentoCliente,
    nc.MonedaCodigo,
    nc.TasaCambio,
    nc.SubTotalMoneda,
    nc.ItbisMoneda,
    nc.TotalMoneda,
    nc.Usuario,
    nc.Estado,
    e.NCF AS ENCF
FROM dbo.NotaCreditoCab nc
LEFT JOIN dbo.Cliente c ON c.[Código] = nc.ClienteCodigo
LEFT JOIN dbo.NCF_Emision e ON e.Origen = 'NC_VENTA' AND e.OrigenId = nc.NCId
WHERE nc.NCId = @NCId;", cn);

            cmdCab.Parameters.Add("@NCId", SqlDbType.BigInt).Value = ncId;

            using var rdCab = cmdCab.ExecuteReader();
            if (!rdCab.Read()) return null;

            var dto = new NotaCreditoVentaDto
            {
                Cab = new NotaCreditoVentaCabDto
                {
                    NCId = rdCab.GetInt64(0),
                    NoDocumento = rdCab.IsDBNull(1) ? "" : rdCab.GetString(1),
                    Fecha = rdCab.IsDBNull(2) ? DateTime.Now : rdCab.GetDateTime(2),
                    VentaIdOrigen = rdCab.IsDBNull(3) ? null : rdCab.GetInt64(3),
                    ClienteCodigo = rdCab.IsDBNull(4) ? "" : rdCab.GetString(4),
                    ClienteNombre = rdCab.IsDBNull(5) ? "" : rdCab.GetString(5),
                    DocumentoCliente = rdCab.IsDBNull(6) ? null : rdCab.GetString(6),
                    MonedaCodigo = rdCab.IsDBNull(7) ? "DOP" : rdCab.GetString(7),
                    TasaCambio = rdCab.IsDBNull(8) ? 1m : Convert.ToDecimal(rdCab.GetValue(8), CultureInfo.InvariantCulture),
                    SubTotalMoneda = rdCab.IsDBNull(9) ? 0m : Convert.ToDecimal(rdCab.GetValue(9), CultureInfo.InvariantCulture),
                    ItbisMoneda = rdCab.IsDBNull(10) ? 0m : Convert.ToDecimal(rdCab.GetValue(10), CultureInfo.InvariantCulture),
                    TotalMoneda = rdCab.IsDBNull(11) ? 0m : Convert.ToDecimal(rdCab.GetValue(11), CultureInfo.InvariantCulture),
                    Usuario = rdCab.IsDBNull(12) ? null : rdCab.GetString(12),
                    Estado = rdCab.IsDBNull(13) ? "" : rdCab.GetString(13),
                    ENCF = rdCab.IsDBNull(14) ? null : rdCab.GetString(14),
                }
            };

            rdCab.Close();

            using var cmdLin = new SqlCommand(@"
SELECT
    nl.Linea,
    ISNULL(nl.ProductoCodigo,'') AS ProductoCodigo,
    ISNULL(p.[Descripción], ISNULL(nl.ProductoCodigo,'')) AS Descripcion,
    ISNULL(nl.Cantidad,0) AS Cantidad,
    ISNULL(nl.PrecioUnit,0) AS PrecioUnit,
    ISNULL(nl.ItbisPct,0) AS ItbisPct,
    ISNULL(nl.DescuentoMoneda,0) AS DescuentoMoneda,
    ISNULL(nl.SubTotalMoneda,0) AS SubTotalMoneda,
    ISNULL(nl.ItbisMoneda,0) AS ItbisMoneda,
    ISNULL(nl.TotalMoneda,0) AS TotalMoneda
FROM dbo.NotaCreditoLin nl
LEFT JOIN dbo.Producto p ON p.[Nº] = nl.ProductoCodigo
WHERE nl.NCId = @NCId
ORDER BY nl.Linea;", cn);

            cmdLin.Parameters.Add("@NCId", SqlDbType.BigInt).Value = ncId;

            using var rdLin = cmdLin.ExecuteReader();
            while (rdLin.Read())
            {
                dto.Lineas.Add(new NotaCreditoVentaLinDto
                {
                    Seleccionado = true,
                    Linea = rdLin.GetInt32(0),
                    ProductoCodigo = rdLin.IsDBNull(1) ? "" : rdLin.GetString(1),
                    Descripcion = rdLin.IsDBNull(2) ? "" : rdLin.GetString(2),
                    CantidadNC = rdLin.IsDBNull(3) ? 0m : Convert.ToDecimal(rdLin.GetValue(3), CultureInfo.InvariantCulture),
                    PrecioUnit = rdLin.IsDBNull(4) ? 0m : Convert.ToDecimal(rdLin.GetValue(4), CultureInfo.InvariantCulture),
                    ItbisPct = rdLin.IsDBNull(5) ? 0m : Convert.ToDecimal(rdLin.GetValue(5), CultureInfo.InvariantCulture),
                    DescuentoMoneda = rdLin.IsDBNull(6) ? 0m : Convert.ToDecimal(rdLin.GetValue(6), CultureInfo.InvariantCulture),
                    SubTotalMoneda = rdLin.IsDBNull(7) ? 0m : Convert.ToDecimal(rdLin.GetValue(7), CultureInfo.InvariantCulture),
                    ItbisMoneda = rdLin.IsDBNull(8) ? 0m : Convert.ToDecimal(rdLin.GetValue(8), CultureInfo.InvariantCulture),
                    TotalMoneda = rdLin.IsDBNull(9) ? 0m : Convert.ToDecimal(rdLin.GetValue(9), CultureInfo.InvariantCulture),
                });
            }

            return dto;
        }
    }
}