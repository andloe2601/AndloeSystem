using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using Andloe.Entidad;

namespace Andloe.Data
{
    public sealed class MunicipioRepository
    {
        public List<Municipio> ListarPorProvincia(string codigoProvincia)
        {
            var list = new List<Municipio>();

            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
SELECT MunicipioId, CodigoMunicipio, CodigoProvincia, Nombre, ProvinciaId, Estado
FROM dbo.Municipio
WHERE CodigoProvincia = @CodigoProvincia
  AND Estado = 1
ORDER BY Nombre;", cn);

            cmd.Parameters.AddWithValue("@CodigoProvincia", codigoProvincia);

            using var rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                list.Add(new Municipio
                {
                    MunicipioId = rd.GetInt32(0),
                    CodigoMunicipio = rd.GetString(1),
                    CodigoProvincia = rd.GetString(2),
                    Nombre = rd.GetString(3),
                    ProvinciaId = rd.GetInt32(4),
                    Estado = rd.GetBoolean(5)
                });
            }

            return list;
        }

        public (int ProvinciaId, string Provincia, string Municipio)? ObtenerUbicacion(int CodigoMunicipio)
        {
            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
SELECT p.ProvinciaId, p.Nombre AS Provincia, m.Nombre AS Municipio
FROM dbo.Municipio m
JOIN dbo.Provincia p ON m.ProvinciaId = p.ProvinciaId
WHERE m.CodigoMunicipo = @CodigoMunicipo;", cn);

            cmd.Parameters.AddWithValue("@CodigoMunicipo", CodigoMunicipio);

            using var rd = cmd.ExecuteReader();
            if (!rd.Read()) return null;

            return (rd.GetInt32(0), rd.GetString(1), rd.GetString(2));
        }
    }
}