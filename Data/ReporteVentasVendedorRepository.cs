using Andloe.Entidad;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace Andloe.Data
{
    public sealed class ReporteVentasVendedorRepository
    {
        public List<VentasPorVendedorRowDto> Resumen(DateTime desde, DateTime hasta, string? codVendedor)
        {
            var list = new List<VentasPorVendedorRowDto>();

            var cod = string.IsNullOrWhiteSpace(codVendedor) ? null : codVendedor.Trim();

            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
SELECT
    ISNULL(f.CodVendedor,'') AS CodVendedor,
    -- Si no tienes tabla vendedores join, te muestro solo codigo (sin inventar)
    ISNULL(f.CodVendedor,'') AS Vendedor,

    COUNT(1) AS CantFacturas,
    ISNULL(SUM(f.SubTotal),0) AS SubTotal,
    ISNULL(SUM(f.TotalDescuento),0) AS Descuento,
    ISNULL(SUM(f.TotalImpuesto),0) AS Itbis,
    ISNULL(SUM(f.TotalGeneral),0) AS TotalGeneral
FROM dbo.FacturaCab f
WHERE
    f.TipoDocumento = 'FAC'
    AND UPPER(LTRIM(RTRIM(ISNULL(f.Estado,'')))) = 'FINALIZADA'
    AND (UPPER(LTRIM(RTRIM(ISNULL(f.Estado,'')))) <> 'ANULADA')
    AND f.FechaDocumento >= @Desde
    AND f.FechaDocumento <  DATEADD(DAY, 1, @Hasta)
    AND (@CodVend IS NULL OR ISNULL(f.CodVendedor,'') = @CodVend)
GROUP BY ISNULL(f.CodVendedor,'')
ORDER BY ISNULL(f.CodVendedor,'');", cn);

            cmd.Parameters.Add("@Desde", SqlDbType.DateTime).Value = desde.Date;
            cmd.Parameters.Add("@Hasta", SqlDbType.DateTime).Value = hasta.Date;
            cmd.Parameters.Add("@CodVend", SqlDbType.VarChar, 20).Value = (object?)cod ?? DBNull.Value;

            using var rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                list.Add(new VentasPorVendedorRowDto
                {
                    CodVendedor = rd.IsDBNull(0) ? "" : rd.GetString(0),
                    Vendedor = rd.IsDBNull(1) ? "" : rd.GetString(1),
                    CantFacturas = rd.GetInt32(2),
                    SubTotal = rd.GetDecimal(3),
                    Descuento = rd.GetDecimal(4),
                    Itbis = rd.GetDecimal(5),
                    TotalGeneral = rd.GetDecimal(6),
                });
            }

            return list;
        }

        public List<VentasPorVendedorDetalleDto> Detalle(DateTime desde, DateTime hasta, string? codVendedor)
        {
            var list = new List<VentasPorVendedorDetalleDto>();
            var cod = string.IsNullOrWhiteSpace(codVendedor) ? null : codVendedor.Trim();

            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
SELECT
    f.FacturaId,
    ISNULL(f.NumeroDocumento,'') AS NumeroDocumento,
    f.FechaDocumento,
    ISNULL(f.CodVendedor,'') AS CodVendedor,
    ISNULL(f.CodVendedor,'') AS Vendedor,
    ISNULL(f.NombreCliente,'') AS Cliente,
    ISNULL(f.TotalGeneral,0) AS TotalGeneral,
    ISNULL(f.Estado,'') AS Estado
FROM dbo.FacturaCab f
WHERE
    f.TipoDocumento = 'FAC'
    AND UPPER(LTRIM(RTRIM(ISNULL(f.Estado,'')))) = 'FINALIZADA'
    AND (UPPER(LTRIM(RTRIM(ISNULL(f.Estado,'')))) <> 'ANULADA')
    AND f.FechaDocumento >= @Desde
    AND f.FechaDocumento <  DATEADD(DAY, 1, @Hasta)
    AND (@CodVend IS NULL OR ISNULL(f.CodVendedor,'') = @CodVend)
ORDER BY f.FechaDocumento DESC, f.FacturaId DESC;", cn);

            cmd.Parameters.Add("@Desde", SqlDbType.DateTime).Value = desde.Date;
            cmd.Parameters.Add("@Hasta", SqlDbType.DateTime).Value = hasta.Date;
            cmd.Parameters.Add("@CodVend", SqlDbType.VarChar, 20).Value = (object?)cod ?? DBNull.Value;

            using var rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                list.Add(new VentasPorVendedorDetalleDto
                {
                    FacturaId = rd.GetInt32(0),
                    NumeroDocumento = rd.IsDBNull(1) ? "" : rd.GetString(1),
                    FechaDocumento = rd.GetDateTime(2),
                    CodVendedor = rd.IsDBNull(3) ? "" : rd.GetString(3),
                    Vendedor = rd.IsDBNull(4) ? "" : rd.GetString(4),
                    Cliente = rd.IsDBNull(5) ? "" : rd.GetString(5),
                    TotalGeneral = rd.GetDecimal(6),
                    Estado = rd.IsDBNull(7) ? "" : rd.GetString(7),
                });
            }

            return list;
        }
    }
}