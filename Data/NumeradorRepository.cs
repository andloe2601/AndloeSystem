using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace Andloe.Data
{
    /// <summary>
    /// Numeración oficial en tu BD:
    /// - dbo.NumeradorSecuencia (Codigo, Prefijo, Longitud, Actual)
    /// - dbo.sp_Numerador_Siguiente
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
            if (lenFallback > 20) lenFallback = 20;

            using var cn = Db.GetOpenConnection();

            using var cmd = new SqlCommand("dbo.sp_Numerador_Siguiente", cn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@Codigo", SqlDbType.VarChar, 20).Value = codigo;
            cmd.Parameters.Add("@Prefijo", SqlDbType.VarChar, 10).Value = prefijoFallback;
            cmd.Parameters.Add("@Longitud", SqlDbType.Int).Value = lenFallback;

            var pOut = new SqlParameter("@Siguiente", SqlDbType.VarChar, 30)
            {
                Direction = ParameterDirection.Output
            };
            cmd.Parameters.Add(pOut);

            cmd.ExecuteNonQuery();

            var siguiente = pOut.Value == DBNull.Value ? null : Convert.ToString(pOut.Value);

            if (string.IsNullOrWhiteSpace(siguiente))
                throw new InvalidOperationException($"No se pudo generar numerador para '{codigo}' (SP devolvió vacío).");

            return siguiente!;
        }
    }
}
