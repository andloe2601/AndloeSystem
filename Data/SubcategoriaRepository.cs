using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;

namespace Andloe.Data
{
    public class SubcategoriaRepository
    {
        public List<(int Id, string Nombre)> ListarPorCategoria(int categoriaId)
        {
            var list = new List<(int, string)>();
            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
SELECT SubcategoriaId, Nombre
FROM dbo.Subcategoria
WHERE Estado=1 AND CategoriaId=@cat
ORDER BY Nombre;", cn);

            cmd.Parameters.Add("@cat", SqlDbType.Int).Value = categoriaId;

            using var rd = cmd.ExecuteReader();
            while (rd.Read())
                list.Add((rd.GetInt32(0), rd.GetString(1)));

            return list;
        }
    }
}
