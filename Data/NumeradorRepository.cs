using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace Andloe.Data
{
    /// <summary>
    /// Genera numeraciones usando:
    /// - dbo.Numerador (config/master)
    /// - dbo.NumeradorSecuencia (contador runtime)
    /// </summary>
    public sealed class NumeradorRepository
    {
        public string Next(string codigo, string prefijoFallback, int lenFallback)
        {
            if (string.IsNullOrWhiteSpace(codigo))
                throw new ArgumentException("Código requerido.", nameof(codigo));

            codigo = codigo.Trim().ToUpperInvariant();
            prefijoFallback ??= "";
            if (lenFallback <= 0) lenFallback = 6;

            using var cn = Db.GetOpenConnection();
            using var tx = cn.BeginTransaction(IsolationLevel.Serializable);

            try
            {
                // 1) Leer config desde dbo.Numerador (si existe)
                string prefCfg = prefijoFallback;
                int lenCfg = lenFallback;

                using (var cmd = new SqlCommand(@"
SELECT TOP(1) Prefijo, Longitud
FROM dbo.Numerador WITH (NOLOCK)
WHERE Codigo = @cod;", cn, tx))
                {
                    cmd.Parameters.Add("@cod", SqlDbType.VarChar, 20).Value = codigo;
                    using var rd = cmd.ExecuteReader();
                    if (rd.Read())
                    {
                        if (!rd.IsDBNull(0)) prefCfg = rd.GetString(0) ?? prefijoFallback;
                        if (!rd.IsDBNull(1)) lenCfg = rd.GetInt32(1);
                    }
                }

                if (lenCfg <= 0) lenCfg = lenFallback;
                if (lenCfg > 20) lenCfg = 20;

                // 2) Crear fila runtime si no existe, tomando prefijo/longitud del master (o fallback)
                using (var cmd = new SqlCommand(@"
IF NOT EXISTS (SELECT 1 FROM dbo.NumeradorSecuencia WITH (UPDLOCK, HOLDLOCK) WHERE Codigo=@cod)
BEGIN
    INSERT INTO dbo.NumeradorSecuencia(Codigo, Prefijo, Longitud, Actual)
    VALUES (@cod, @pref, @len, 0);
END
ELSE
BEGIN
    -- si cambiaste el master, puedes sincronizar prefijo/longitud aquí (opcional)
    UPDATE dbo.NumeradorSecuencia
    SET Prefijo=@pref, Longitud=@len
    WHERE Codigo=@cod;
END
", cn, tx))
                {
                    cmd.Parameters.Add("@cod", SqlDbType.VarChar, 20).Value = codigo;
                    cmd.Parameters.Add("@pref", SqlDbType.VarChar, 10).Value = prefCfg ?? "";
                    cmd.Parameters.Add("@len", SqlDbType.Int).Value = lenCfg;
                    cmd.ExecuteNonQuery();
                }

                // 3) Incrementar y devolver
                string prefijo;
                int longitud;
                int actual;

                using (var cmd = new SqlCommand(@"
UPDATE dbo.NumeradorSecuencia
SET Actual = Actual + 1
OUTPUT INSERTED.Prefijo, INSERTED.Longitud, INSERTED.Actual
WHERE Codigo = @cod;
", cn, tx))
                {
                    cmd.Parameters.Add("@cod", SqlDbType.VarChar, 20).Value = codigo;

                    using var rd = cmd.ExecuteReader();
                    if (!rd.Read())
                        throw new InvalidOperationException($"No se pudo generar numerador para '{codigo}'.");

                    prefijo = rd.IsDBNull(0) ? "" : rd.GetString(0);
                    longitud = rd.IsDBNull(1) ? lenCfg : rd.GetInt32(1);
                    actual = rd.IsDBNull(2) ? 0 : rd.GetInt32(2);
                }

                tx.Commit();

                var numero = actual.ToString().PadLeft(longitud <= 0 ? lenCfg : longitud, '0');
                return (prefijo ?? "") + numero;
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }
    }
}
