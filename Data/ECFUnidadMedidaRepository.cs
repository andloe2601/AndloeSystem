using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using Andloe.Entidad;

namespace Andloe.Data
{
    public class ECFUnidadMedidaRepository
    {
        public List<ECFUnidadMedida> ListarActivas()
        {
            var lista = new List<ECFUnidadMedida>();

            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
SELECT
    UnidadMedidaECFId,
    CodigoDGII,
    Descripcion,
    UnidadInternaCodigo,
    Activo
FROM dbo.ECFUnidadMedida
WHERE Activo = 1
ORDER BY Descripcion;", cn);

            using var rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                lista.Add(new ECFUnidadMedida
                {
                    UnidadMedidaECFId = rd.IsDBNull(0) ? 0 : rd.GetInt32(0),
                    CodigoDGII = rd.IsDBNull(1) ? "" : rd.GetString(1),
                    Descripcion = rd.IsDBNull(2) ? "" : rd.GetString(2),
                    UnidadInternaCodigo = rd.IsDBNull(3) ? "" : rd.GetString(3),
                    Activo = !rd.IsDBNull(4) && rd.GetBoolean(4)
                });
            }

            return lista;
        }

        public ECFUnidadMedida? ObtenerPorId(int unidadMedidaECFId)
        {
            if (unidadMedidaECFId <= 0) return null;

            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
SELECT TOP (1)
    UnidadMedidaECFId,
    CodigoDGII,
    Descripcion,
    UnidadInternaCodigo,
    Activo
FROM dbo.ECFUnidadMedida
WHERE UnidadMedidaECFId = @id;", cn);

            cmd.Parameters.Add("@id", System.Data.SqlDbType.Int).Value = unidadMedidaECFId;

            using var rd = cmd.ExecuteReader();
            if (!rd.Read()) return null;

            return new ECFUnidadMedida
            {
                UnidadMedidaECFId = rd.IsDBNull(0) ? 0 : rd.GetInt32(0),
                CodigoDGII = rd.IsDBNull(1) ? "" : rd.GetString(1),
                Descripcion = rd.IsDBNull(2) ? "" : rd.GetString(2),
                UnidadInternaCodigo = rd.IsDBNull(3) ? "" : rd.GetString(3),
                Activo = !rd.IsDBNull(4) && rd.GetBoolean(4)
            };
        }

        public ECFUnidadMedida? ObtenerPorCodigoDGII(string codigoDGII)
        {
            if (string.IsNullOrWhiteSpace(codigoDGII)) return null;
            codigoDGII = codigoDGII.Trim().ToUpperInvariant();

            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
SELECT TOP (1)
    UnidadMedidaECFId,
    CodigoDGII,
    Descripcion,
    UnidadInternaCodigo,
    Activo
FROM dbo.ECFUnidadMedida
WHERE Activo = 1
  AND UPPER(LTRIM(RTRIM(CodigoDGII))) = @codigo;", cn);

            cmd.Parameters.Add("@codigo", System.Data.SqlDbType.VarChar, 10).Value = codigoDGII;

            using var rd = cmd.ExecuteReader();
            if (!rd.Read()) return null;

            return new ECFUnidadMedida
            {
                UnidadMedidaECFId = rd.IsDBNull(0) ? 0 : rd.GetInt32(0),
                CodigoDGII = rd.IsDBNull(1) ? "" : rd.GetString(1),
                Descripcion = rd.IsDBNull(2) ? "" : rd.GetString(2),
                UnidadInternaCodigo = rd.IsDBNull(3) ? "" : rd.GetString(3),
                Activo = !rd.IsDBNull(4) && rd.GetBoolean(4)
            };
        }
    }
}