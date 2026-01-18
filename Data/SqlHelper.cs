using Microsoft.Data.SqlClient;
using System;
using System.Data;

namespace Andloe.Data
{
    public static class SqlHelper
    {
        public static T? ExecuteScalar<T>(string sql)
        {
            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(sql, cn);
            var val = cmd.ExecuteScalar();

            // Si viene NULL de SQL => default(T)
            if (val == null || val == DBNull.Value)
                return default;

            var targetType = typeof(T);

            // Si ya es del tipo exacto, devolvemos directo
            if (val is T tDirect)
                return tDirect;

            // Si T es Nullable<X>, convertimos al tipo subyacente
            var underlying = Nullable.GetUnderlyingType(targetType);
            if (underlying != null)
            {
                var converted = Convert.ChangeType(val, underlying); // decimal, int, etc.
                return (T)converted;
            }

            // Tipo normal (no nullable)
            return (T)Convert.ChangeType(val, targetType);
        }

        public static DataTable ExecuteDataTable(string sql)
        {
            using var cn = Db.GetOpenConnection();
            using var da = new SqlDataAdapter(sql, cn);
            var dt = new DataTable();
            da.Fill(dt);
            return dt;
        }
    }
}
