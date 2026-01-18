using System.Data;
using Microsoft.Data.SqlClient;

namespace Andloe.Data
{
    public class MonedaRepository
    {
        /// <summary>
        /// Lista las monedas activas para combos.
        /// Devuelve columnas: Codigo, Descripcion
        /// </summary>
        public DataTable ListarMonedas()
        {
            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
SELECT 
    MonedaCodigo AS Codigo,
    Nombre       AS Descripcion
FROM dbo.Moneda
WHERE Estado = 1
ORDER BY Nombre;", cn);

            using var da = new SqlDataAdapter(cmd);
            var dt = new DataTable();
            da.Fill(dt);
            return dt;
        }
    }
}
