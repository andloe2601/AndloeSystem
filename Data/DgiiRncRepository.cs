using Microsoft.Data.SqlClient;
using System;
using System.Data;

namespace Andloe.Data.DGII
{
    public class DgiiRncRepository
    {
        public DgiiRncEntry? BuscarPorRnc(string rnc)
        {
            if (string.IsNullOrWhiteSpace(rnc)) return null;

            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
SELECT TOP 1
    Rnc,
    Nombre,
    NombreComercial,
    Estado,
    Condicion
FROM dbo.DgiiRncEntry
WHERE Rnc = @rnc
AND DatasetId = (
    SELECT TOP 1 DatasetId
    FROM dbo.DgiiRncDataset
    WHERE Estado = 'ACTIVO'
)", cn);

            cmd.Parameters.Add("@rnc", SqlDbType.VarChar, 20).Value = rnc.Trim();

            using var rd = cmd.ExecuteReader();
            if (!rd.Read()) return null;

            return new DgiiRncEntry
            {
                Rnc = rd.GetString(0),
                Nombre = rd.GetString(1),
                NombreComercial = rd.IsDBNull(2) ? null : rd.GetString(2),
                Estado = rd.IsDBNull(3) ? null : rd.GetString(3),
                Condicion = rd.IsDBNull(4) ? null : rd.GetString(4)
            };
        }

        public DgiiRncEntryDto? BuscarActivoPorRnc(string rnc)
        {
            if (string.IsNullOrWhiteSpace(rnc)) return null;

            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
SELECT TOP(1)
    e.Rnc,
    e.Nombre,
    e.NombreComercial,
    e.Actividad,
    e.Estado,
    e.Condicion,
    e.FechaRegistro
FROM dbo.DgiiRncEntry e
INNER JOIN dbo.DgiiRncDataset d
    ON d.DatasetId = e.DatasetId
WHERE d.Estado = 'ACTIVO'
  AND e.Rnc = @rnc
ORDER BY e.EntryId DESC;", cn);

            cmd.Parameters.Add("@rnc", SqlDbType.VarChar, 20).Value = rnc.Trim();

            using var rd = cmd.ExecuteReader();
            if (!rd.Read()) return null;

            return new DgiiRncEntryDto
            {
                Rnc = rd.GetString(0),
                Nombre = rd.GetString(1),
                NombreComercial = rd.IsDBNull(2) ? null : rd.GetString(2),
                Actividad = rd.IsDBNull(3) ? null : rd.GetString(3),
                Estado = rd.IsDBNull(4) ? null : rd.GetString(4),
                Condicion = rd.IsDBNull(5) ? null : rd.GetString(5),
                FechaRegistro = rd.IsDBNull(6) ? (DateTime?)null : rd.GetDateTime(6),
            };
        }
    }

    // ✅ Esta clase te faltaba (por eso “no se encontró DgiiRncEntry”)
    public class DgiiRncEntry
    {
        public string Rnc { get; set; } = "";
        public string Nombre { get; set; } = "";
        public string? NombreComercial { get; set; }
        public string? Estado { get; set; }
        public string? Condicion { get; set; }
    }

    public class DgiiRncEntryDto
    {
        public string Rnc { get; set; } = "";
        public string Nombre { get; set; } = "";
        public string? NombreComercial { get; set; }
        public string? Actividad { get; set; }
        public string? Estado { get; set; }
        public string? Condicion { get; set; }
        public DateTime? FechaRegistro { get; set; }
    }
}
