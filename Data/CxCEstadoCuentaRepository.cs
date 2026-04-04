using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Andloe.Entidad.CxC;
using Microsoft.Data.SqlClient;

namespace Andloe.Data
{
    public sealed class CxCEstadoCuentaRepository
    {
        public List<CxCEstadoCuentaDto> ListarEstadoCuentaCliente(int clienteId, DateTime? desde = null, DateTime? hasta = null)
        {
            var list = new List<CxCEstadoCuentaDto>();

            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
SELECT
    ClienteId,
    Fecha,
    TipoMovimiento,
    Documento,
    Referencia,
    Debe,
    Haber,
    Monto,
    FechaVencimiento,
    OrigenId,
    Descripcion
FROM dbo.vw_CxCEstadoCuentaCliente
WHERE ClienteId = @ClienteId
  AND (@Desde IS NULL OR Fecha >= @Desde)
  AND (@Hasta IS NULL OR Fecha <= @Hasta)
ORDER BY Fecha, Documento, OrigenId;", cn);

            cmd.Parameters.Add("@ClienteId", SqlDbType.Int).Value = clienteId;
            cmd.Parameters.Add("@Desde", SqlDbType.Date).Value = (object?)desde?.Date ?? DBNull.Value;
            cmd.Parameters.Add("@Hasta", SqlDbType.Date).Value = (object?)hasta?.Date ?? DBNull.Value;

            using var rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                list.Add(new CxCEstadoCuentaDto
                {
                    ClienteId = rd.GetInt32(0),
                    Fecha = rd.GetDateTime(1),
                    TipoMovimiento = rd.IsDBNull(2) ? "" : rd.GetString(2),
                    Documento = rd.IsDBNull(3) ? "" : rd.GetString(3),
                    Referencia = rd.IsDBNull(4) ? null : rd.GetString(4),
                    Debe = rd.IsDBNull(5) ? 0m : rd.GetDecimal(5),
                    Haber = rd.IsDBNull(6) ? 0m : rd.GetDecimal(6),
                    Monto = rd.IsDBNull(7) ? 0m : rd.GetDecimal(7),
                    FechaVencimiento = rd.GetDateTime(8),
                    OrigenId = Convert.ToInt64(rd.GetValue(9)),
                    Descripcion = rd.IsDBNull(10) ? null : rd.GetString(10)
                });
            }

            decimal balance = 0m;
            foreach (var item in list.OrderBy(x => x.Fecha).ThenBy(x => x.Documento).ThenBy(x => x.OrigenId))
            {
                balance += item.Debe - item.Haber;
                item.BalanceAcumulado = balance;

                if (item.TipoMovimiento == "FACTURA" &&
                    item.FechaVencimiento.Date < DateTime.Today &&
                    item.Debe > 0)
                {
                    item.DiasVencidos = (DateTime.Today - item.FechaVencimiento.Date).Days;
                }
                else
                {
                    item.DiasVencidos = 0;
                }
            }

            return list;
        }
    }
}