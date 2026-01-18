using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace Andloe.Data
{
    public class SucursalDto
    {
        public int SucursalId { get; set; }
        public string Nombre { get; set; } = string.Empty;

        public override string ToString() => Nombre;
    }

    public class SucursalRepository
    {
        public string? ObtenerNombre(int sucursalId)
        {
            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
SELECT TOP 1 Nombre
FROM dbo.Sucursal
WHERE SucursalId = @id;", cn);

            cmd.Parameters.Add("@id", SqlDbType.Int).Value = sucursalId;

            var val = cmd.ExecuteScalar();
            if (val == null || val == DBNull.Value) return null;

            return Convert.ToString(val);
        }
    }
}
