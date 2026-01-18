using Microsoft.Data.SqlClient;
using System.Data;

namespace Andloe.Data;

public static class Db
{
    private static string? _cs;
    private static readonly object _lock = new();

    public static void Init(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new ArgumentException("connectionString inválido.", nameof(connectionString));

        lock (_lock)
        {
            _cs = connectionString;
        }
    }

    public static SqlConnection GetOpenConnection()
    {
        string? cs;
        lock (_lock) cs = _cs;

        if (string.IsNullOrWhiteSpace(cs))
            throw new InvalidOperationException("Db.Init no fue llamado con el connection string.");

        var cn = new SqlConnection(cs);
        cn.Open();
        return cn;
    }

    public static SqlCommand CreateCommand(SqlConnection cn, string sql, CommandType type = CommandType.Text, int timeoutSeconds = 30)
    {
        var cmd = cn.CreateCommand();
        cmd.CommandText = sql;
        cmd.CommandType = type;
        cmd.CommandTimeout = timeoutSeconds;
        return cmd;
    }
}
