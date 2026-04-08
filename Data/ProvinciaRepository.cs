using Microsoft.Data.SqlClient;
using System.Data;

namespace Andloe.Data
{
    public sealed class ProvinciaRepository
    {
        public DataTable Listar()
        {
            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
SELECT ProvinciaId, Nombre
FROM dbo.Provincia
ORDER BY Nombre;", cn);

            var dt = new DataTable();
            dt.Load(cmd.ExecuteReader());
            return dt;
        }


    }
}