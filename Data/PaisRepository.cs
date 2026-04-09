using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using Andloe.Entidad;

namespace Andloe.Data
{
    public class PaisRepository
    {
        public List<Pais> ListarActivos()
        {
            var list = new List<Pais>();

            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
SELECT PaisId, CodigoIso, Nombre, Estado, FechaCreacion
FROM dbo.Pais
WHERE Estado = 1
ORDER BY Nombre;", cn);

            using var rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                list.Add(new Pais
                {
                    PaisId = rd.GetInt32(0),
                    CodigoIso = rd.GetString(1),
                    Nombre = rd.GetString(2),
                    Estado = rd.GetBoolean(3),
                    FechaCreacion = rd.GetDateTime(4)
                });
            }

            return list;
        }
    }
}