using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using Andloe.Entidad;

namespace Andloe.Data
{
    public class KardexRepository
    {
        public List<KardexLineaDto> ListarKardex(
            string productoCodigo,
            DateTime? fechaDesde = null,
            DateTime? fechaHasta = null)
        {
            var listRaw = new List<(DateTime Fecha, string Tipo, string? Origen,
                                    long? OrigenId, string ProductoCodigo,
                                    decimal Cantidad, string Usuario, string? Observacion)>();

            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
SELECT c.Fecha, c.Tipo, c.Origen, c.OrigenId,
       l.ProductoCodigo, l.Cantidad,
       c.Usuario, c.Observacion
FROM dbo.InvMovimientoCab c
JOIN dbo.InvMovimientoLin l ON l.InvMovId = c.InvMovId
WHERE l.ProductoCodigo = @prod
  AND (@fDesde IS NULL OR c.Fecha >= @fDesde)
  AND (@fHasta IS NULL OR c.Fecha < DATEADD(DAY, 1, @fHasta))
ORDER BY c.Fecha, c.InvMovId, l.Linea;", cn);

            cmd.Parameters.Add("@prod", SqlDbType.VarChar, 20).Value = productoCodigo;

            if (fechaDesde.HasValue)
                cmd.Parameters.Add("@fDesde", SqlDbType.DateTime2).Value = fechaDesde.Value;
            else
                cmd.Parameters.Add("@fDesde", SqlDbType.DateTime2).Value = DBNull.Value;

            if (fechaHasta.HasValue)
                cmd.Parameters.Add("@fHasta", SqlDbType.DateTime2).Value = fechaHasta.Value;
            else
                cmd.Parameters.Add("@fHasta", SqlDbType.DateTime2).Value = DBNull.Value;

            using (var rd = cmd.ExecuteReader())
            {
                while (rd.Read())
                {
                    listRaw.Add((
                        rd.GetDateTime(0),
                        rd.GetString(1),                             // Tipo
                        rd.IsDBNull(2) ? null : rd.GetString(2),     // Origen
                        rd.IsDBNull(3) ? (long?)null : rd.GetInt64(3),
                        rd.GetString(4),                             // Producto
                        rd.GetDecimal(5),                            // Cantidad
                        rd.IsDBNull(6) ? "" : rd.GetString(6),       // Usuario
                        rd.IsDBNull(7) ? null : rd.GetString(7)      // Obs
                    ));
                }
            }

            // Calcular saldo acumulado
            var result = new List<KardexLineaDto>();
            decimal saldo = 0m;

            foreach (var r in listRaw)
            {
                decimal entrada = 0m;
                decimal salida = 0m;

                // Regla simple: Tipo='ENTRADA' suma, 'SALIDA' resta
                if (r.Tipo.Equals("ENTRADA", StringComparison.OrdinalIgnoreCase))
                    entrada = r.Cantidad;
                else if (r.Tipo.Equals("SALIDA", StringComparison.OrdinalIgnoreCase))
                    salida = r.Cantidad;
                else
                {
                    // Otros tipos (AJUSTE): podrías usar signo por Origen o por cantidad.
                    // Aquí lo dejamos como entrada positiva si la cantidad > 0.
                    if (r.Cantidad >= 0)
                        entrada = r.Cantidad;
                    else
                        salida = Math.Abs(r.Cantidad);
                }

                saldo += entrada - salida;

                result.Add(new KardexLineaDto
                {
                    Fecha = r.Fecha,
                    Tipo = r.Tipo,
                    Origen = r.Origen,
                    OrigenId = r.OrigenId,
                    ProductoCodigo = r.ProductoCodigo,
                    CantidadEntrada = entrada,
                    CantidadSalida = salida,
                    Saldo = saldo,
                    Usuario = r.Usuario,
                    Observacion = r.Observacion
                });
            }

            return result;
        }
    }
}
