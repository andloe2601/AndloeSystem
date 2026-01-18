using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace Andloe.Data
{
    public class EmpresaRepository
    {
        public string? ObtenerRazonSocial(int empresaId)
        {
            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
SELECT TOP 1 RazonSocial
FROM dbo.Empresa
WHERE EmpresaId = @id;", cn);

            cmd.Parameters.Add("@id", SqlDbType.Int).Value = empresaId;

            var val = cmd.ExecuteScalar();
            if (val == null || val == DBNull.Value) return null;

            return Convert.ToString(val);
        }
    }
}
