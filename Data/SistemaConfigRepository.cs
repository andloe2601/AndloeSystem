using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace Andloe.Data
{
    /// <summary>
    /// Acceso a la tabla dbo.SistemaConfig
    /// Columnas: Clave, Valor, Descripcion, Tipo, FechaMod, UsuarioMod
    /// </summary>
    public sealed class SistemaConfigRepository
    {
        public string? GetValor(string clave)
        {
            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
SELECT TOP(1) Valor
FROM dbo.SistemaConfig
WHERE Clave = @Clave
ORDER BY FechaMod DESC;", cn);

            cmd.Parameters.Add("@Clave", SqlDbType.VarChar, 200).Value = (clave ?? "").Trim();

            var obj = cmd.ExecuteScalar();
            return obj == null || obj == DBNull.Value
                ? null
                : Convert.ToString(obj);
        }

        public decimal GetNumero(string clave, decimal valorPorDefecto)
        {
            var txt = GetValor(clave);
            if (decimal.TryParse(txt, out var num))
                return num;

            return valorPorDefecto;
        }

        // ✅ NUEVO: Entero (para EMPRESA/SUCURSAL/ALMACEN)
        public int? GetEntero(string clave)
        {
            var txt = GetValor(clave);
            if (string.IsNullOrWhiteSpace(txt)) return null;

            return int.TryParse(txt.Trim(), out var n) ? n : (int?)null;
        }

        // ✅ NUEVO: Existe clave
        public bool ExisteClave(string clave)
        {
            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
SELECT TOP(1) 1
FROM dbo.SistemaConfig
WHERE Clave = @Clave;", cn);

            cmd.Parameters.Add("@Clave", SqlDbType.VarChar, 200).Value = (clave ?? "").Trim();
            var v = cmd.ExecuteScalar();
            return v != null && v != DBNull.Value;
        }

        // ✅ NUEVO: borrar clave (cuando unchecked)
        public void DeleteClave(string clave)
        {
            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
DELETE FROM dbo.SistemaConfig
WHERE Clave = @Clave;", cn);

            cmd.Parameters.Add("@Clave", SqlDbType.VarChar, 200).Value = (clave ?? "").Trim();
            cmd.ExecuteNonQuery();
        }

        public void SetValor(string clave, string? valor, string? descripcion, string? tipo, string usuario)
        {
            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
MERGE dbo.SistemaConfig AS T
USING (SELECT @Clave AS Clave) AS S
   ON T.Clave = S.Clave
WHEN MATCHED THEN
    UPDATE SET 
        Valor      = @Valor,
        Descripcion= @Descripcion,
        Tipo       = @Tipo,
        FechaMod   = SYSDATETIME(),
        UsuarioMod = @Usuario
WHEN NOT MATCHED THEN
    INSERT (Clave, Valor, Descripcion, Tipo, FechaMod, UsuarioMod)
    VALUES (@Clave, @Valor, @Descripcion, @Tipo, SYSDATETIME(), @Usuario);", cn);

            cmd.Parameters.Add("@Clave", SqlDbType.VarChar, 200).Value = (clave ?? "").Trim();
            cmd.Parameters.Add("@Valor", SqlDbType.NVarChar, -1).Value =
    (object?)valor ?? DBNull.Value;
            cmd.Parameters.Add("@Descripcion", SqlDbType.VarChar, 200).Value = (object?)descripcion ?? DBNull.Value;
            cmd.Parameters.Add("@Tipo", SqlDbType.VarChar, 50).Value = (object?)tipo ?? DBNull.Value;
            cmd.Parameters.Add("@Usuario", SqlDbType.VarChar, 50).Value = string.IsNullOrWhiteSpace(usuario) ? "SYSTEM" : usuario.Trim();

            cmd.ExecuteNonQuery();
        }

        // ✅ NUEVO: atajo simple para flags
        public void SetValorSimple(string clave, string valor, string usuario)
        {
            SetValor(clave, valor, descripcion: null, tipo: "FLAG", usuario: usuario);
        }
    }



}
