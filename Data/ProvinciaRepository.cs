using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using Andloe.Entidad;

namespace Andloe.Data
{
    public sealed class ProvinciaRepository
    {
        public List<Provincia> Listar()
        {
            var list = new List<Provincia>();

            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
SELECT ProvinciaId, CodigoProvincia, Nombre
FROM dbo.Provincia
ORDER BY Nombre;", cn);

            using var rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                list.Add(new Provincia
                {
                    ProvinciaId = rd.GetInt32(0),
                    CodigoProvincia = rd.GetString(1),
                    Nombre = rd.GetString(2)
                });
            }

            return list;
        }
    }
}