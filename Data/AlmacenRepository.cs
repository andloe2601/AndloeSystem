using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;

namespace Andloe.Data
{
    public class AlmacenRepository
    {
        public class AlmacenSimple
        {
            public int AlmacenId { get; set; }
            public string Nombre { get; set; } = "";
        }

        public List<AlmacenSimple> ListarActivos()
        {
            var lista = new List<AlmacenSimple>();

            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
SELECT AlmacenId, Nombre
FROM dbo.Almacen
WHERE Estado = 1
ORDER BY Nombre;", cn);

            using var rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                lista.Add(new AlmacenSimple
                {
                    AlmacenId = rd.GetInt32(0),
                    Nombre = rd.GetString(1)
                });
            }

            return lista;
        }
    
     public string? ObtenerNombre(int almacenId)
        {
            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
SELECT TOP 1 Nombre
FROM dbo.Almacen
WHERE AlmacenId = @id;", cn);

            cmd.Parameters.Add("@id", SqlDbType.Int).Value = almacenId;

            var val = cmd.ExecuteScalar();
            if (val == null || val == DBNull.Value) return null;

            return Convert.ToString(val);
        }
    }
}
