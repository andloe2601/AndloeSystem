using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;

namespace Andloe.Data
{
    public class CategoriaRepository
    {
        public List<(int Id, string Nombre)> ListarActivas()
        {
            var list = new List<(int, string)>();
            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
SELECT CategoriaId, Nombre
FROM dbo.Categoria
WHERE Estado=1
ORDER BY Nombre;", cn);

            using var rd = cmd.ExecuteReader();
            while (rd.Read())
                list.Add((rd.GetInt32(0), rd.GetString(1)));

            return list;
        }
    }
}
