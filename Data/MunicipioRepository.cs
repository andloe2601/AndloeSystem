using Microsoft.Data.SqlClient;
using System.Data;

namespace Andloe.Data
{
    public sealed class MunicipioRepository
    {
        public DataTable ListarPorProvincia(int provinciaId)
        {
            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
SELECT MunicipioId, Nombre
FROM dbo.Municipio
WHERE ProvinciaId = @ProvinciaId
ORDER BY Nombre;", cn);

            cmd.Parameters.Add("@ProvinciaId", SqlDbType.Int).Value = provinciaId;

            var dt = new DataTable();
            dt.Load(cmd.ExecuteReader());
            return dt;
        }

        public (int ProvinciaId, string Provincia, string Municipio)? ObtenerUbicacion(int municipioId)
        {
            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
SELECT TOP 1
    p.ProvinciaId,
    p.Nombre AS Provincia,
    m.Nombre AS Municipio
FROM dbo.Municipio m
INNER JOIN dbo.Provincia p
    ON p.ProvinciaId = m.ProvinciaId
WHERE m.MunicipioId = @MunicipioId;", cn);

            cmd.Parameters.Add("@MunicipioId", SqlDbType.Int).Value = municipioId;

            using var rd = cmd.ExecuteReader();
            if (!rd.Read()) return null;

            return (
                Convert.ToInt32(rd["ProvinciaId"]),
                rd["Provincia"]?.ToString() ?? "",
                rd["Municipio"]?.ToString() ?? ""
            );
        }
    }
}