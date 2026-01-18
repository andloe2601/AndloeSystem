using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using Andloe.Entidad;

namespace Andloe.Data
{
    public class ImpuestoRepository
    {
        public List<Impuesto> ListarActivos()
        {
            var list = new List<Impuesto>();

            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
SELECT ImpuestoId, Codigo, Nombre, Porcentaje, Estado
FROM dbo.Impuesto
WHERE Estado = 1
ORDER BY Nombre;", cn);

            using var rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                list.Add(new Impuesto
                {
                    ImpuestoId = rd.GetInt32(0),
                    Codigo = rd.GetString(1),
                    Nombre = rd.GetString(2),
                    Porcentaje = rd.GetDecimal(3),
                    Estado = rd.GetBoolean(4)
                });
            }

            return list;
        }
    }
}
