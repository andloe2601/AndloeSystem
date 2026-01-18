using System;
using System.Collections.Generic;
using System.Data;
using Andloe.Data;
using Andloe.Entidad;
using Microsoft.Data.SqlClient;

namespace Andloe.Logica
{
    public class KardexService
    {
        /// <summary>
        /// Devuelve existencia inicial (antes de fechaDesde) + lista de movimientos
        /// entre fechaDesde y fechaHasta, con existencia acumulada.
        /// </summary>
        public (decimal ExistenciaInicial, List<KardexMovimientoDto> Movimientos) ObtenerKardex(
            string productoCodigo,
            int? almacenId,
            DateTime fechaDesde,
            DateTime fechaHasta)
        {
            if (string.IsNullOrWhiteSpace(productoCodigo))
                throw new ArgumentException("Producto requerido.", nameof(productoCodigo));

            if (fechaHasta < fechaDesde)
                throw new ArgumentException("FechaHasta no puede ser menor que FechaDesde.");

            using var cn = Db.GetOpenConnection();

            // ================= EXISTENCIA INICIAL (antes de fechaDesde) =================
            decimal existenciaInicial;
            using (var cmdIni = new SqlCommand(@"
SELECT ISNULL(SUM(
           CASE 
               WHEN c.Tipo = 'ENTRADA' THEN l.Cantidad
               WHEN c.Tipo = 'SALIDA'  THEN -l.Cantidad
               ELSE 0
           END), 0)
FROM dbo.InvMovimientoCab c
JOIN dbo.InvMovimientoLin l ON l.InvMovId = c.InvMovId
WHERE l.ProductoCodigo = @Prod
  AND c.Fecha < @Desde
  AND (@Alm IS NULL OR c.AlmacenIdOrigen = @Alm OR c.AlmacenIdDestino = @Alm);", cn))
            {
                cmdIni.Parameters.Add("@Prod", SqlDbType.VarChar, 20).Value = productoCodigo;
                cmdIni.Parameters.Add("@Desde", SqlDbType.DateTime).Value = fechaDesde;
                cmdIni.Parameters.Add("@Alm", SqlDbType.Int).Value = (object?)almacenId ?? DBNull.Value;

                var val = cmdIni.ExecuteScalar();
                existenciaInicial = (val == null || val == DBNull.Value)
                    ? 0m
                    : Convert.ToDecimal(val);
            }

            var movimientos = new List<KardexMovimientoDto>();

            // ================= MOVIMIENTOS ENTRE DESDE / HASTA =================
            using (var cmd = new SqlCommand(@"
SELECT
    c.Fecha,
    c.Tipo,
    c.Origen,
    c.OrigenId,
    ISNULL(a.Nombre, '') AS AlmacenNombre,
    l.Cantidad
FROM dbo.InvMovimientoCab c
JOIN dbo.InvMovimientoLin l ON l.InvMovId = c.InvMovId
LEFT JOIN dbo.Almacen a ON a.AlmacenId = ISNULL(c.AlmacenIdDestino, c.AlmacenIdOrigen)
WHERE l.ProductoCodigo = @Prod
  AND c.Fecha >= @Desde
  AND c.Fecha < DATEADD(day, 1, @Hasta)
  AND (@Alm IS NULL OR c.AlmacenIdOrigen = @Alm OR c.AlmacenIdDestino = @Alm)
ORDER BY c.Fecha, c.InvMovId, l.Linea;", cn))
            {
                cmd.Parameters.Add("@Prod", SqlDbType.VarChar, 20).Value = productoCodigo;
                cmd.Parameters.Add("@Desde", SqlDbType.DateTime).Value = fechaDesde;
                cmd.Parameters.Add("@Hasta", SqlDbType.DateTime).Value = fechaHasta;
                cmd.Parameters.Add("@Alm", SqlDbType.Int).Value = (object?)almacenId ?? DBNull.Value;

                using var rd = cmd.ExecuteReader();

                var existencia = existenciaInicial;

                while (rd.Read())
                {
                    var fecha = rd.GetDateTime(0);
                    var tipo = rd.IsDBNull(1) ? "" : rd.GetString(1);
                    var origen = rd.IsDBNull(2) ? "" : rd.GetString(2);
                    var origenIdObj = rd.IsDBNull(3) ? null : rd.GetValue(3);
                    var almacenNombre = rd.IsDBNull(4) ? "" : rd.GetString(4);
                    var cantidad = rd.IsDBNull(5) ? 0m : rd.GetDecimal(5);

                    decimal entrada = 0m;
                    decimal salida = 0m;

                    // Ajusta estos códigos de Tipo según tu diseño real:
                    if (string.Equals(tipo, "ENTRADA", StringComparison.OrdinalIgnoreCase))
                    {
                        entrada = cantidad;
                    }
                    else if (string.Equals(tipo, "SALIDA", StringComparison.OrdinalIgnoreCase))
                    {
                        salida = cantidad;
                    }
                    else
                    {
                        // Otros tipos (si tienes COMPRA, AJUSTE+, AJUSTE-, etc.)
                        // Por defecto lo tratamos como entrada positiva.
                        if (cantidad > 0)
                            entrada = cantidad;
                    }

                    existencia += entrada - salida;

                    movimientos.Add(new KardexMovimientoDto
                    {
                        Fecha = fecha,
                        Tipo = tipo,
                        Origen = origen,
                        NumeroDocumento = origenIdObj?.ToString() ?? "",
                        Almacen = almacenNombre,
                        Entrada = entrada,
                        Salida = salida,
                        Existencia = existencia
                    });
                }
            }

            return (existenciaInicial, movimientos);
        }
    }
}
