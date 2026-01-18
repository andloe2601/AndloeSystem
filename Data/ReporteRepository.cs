#nullable enable
using System;
using System.Data;
using Microsoft.Data.SqlClient;
using Andloe.Entidad;

namespace Andloe.Data
{
    public sealed class ReporteRepository
    {
        // =========================================================
        // ✅ Resolver por (Modulo + Actividad + Codigo) => recomendado
        // =========================================================
        public ReporteDefDto? ResolverReporte(
            string modulo,
            string actividad,
            string codigo,
            int empresaId,
            int? sucursalId,
            int? usuarioId)
        {
            if (string.IsNullOrWhiteSpace(modulo))
                throw new ArgumentException("módulo requerido", nameof(modulo));
            if (string.IsNullOrWhiteSpace(actividad))
                throw new ArgumentException("actividad requerida", nameof(actividad));
            if (string.IsNullOrWhiteSpace(codigo))
                throw new ArgumentException("codigo requerido", nameof(codigo));
            if (empresaId <= 0)
                throw new ArgumentException("empresaId inválido", nameof(empresaId));

            modulo = modulo.Trim().ToUpperInvariant();
            actividad = actividad.Trim().ToUpperInvariant();
            codigo = codigo.Trim().ToUpperInvariant();

            using var cn = Db.GetOpenConnection();

            // ✅ Directo a tabla (tu captura demuestra que existe y funciona)
            using var cmd = new SqlCommand(@"
SELECT TOP(1)
    ReporteId, Modulo, Actividad, Codigo, Nombre, Motor, RutaArchivo, Orden
FROM dbo.ReporteDef
WHERE EsActivo = 1
  AND Modulo   = @m
  AND Actividad = @a
  AND Codigo   = @c
ORDER BY ISNULL(Orden, 999999), ReporteId;", cn);

            cmd.Parameters.Add("@m", SqlDbType.VarChar, 50).Value = modulo;
            cmd.Parameters.Add("@a", SqlDbType.VarChar, 50).Value = actividad;
            cmd.Parameters.Add("@c", SqlDbType.VarChar, 80).Value = codigo;

            using var rd = cmd.ExecuteReader(CommandBehavior.SingleRow);
            if (!rd.Read()) return null;

            return Map(rd);
        }

        // =========================================================
        // ✅ Resolver por (Modulo + Codigo) => si no quieres actividad
        // =========================================================
        public ReporteDefDto? ResolverReportePorCodigo(
            string modulo,
            string codigo,
            int empresaId,
            int? sucursalId,
            int? usuarioId)
        {
            if (string.IsNullOrWhiteSpace(modulo))
                throw new ArgumentException("módulo requerido", nameof(modulo));
            if (string.IsNullOrWhiteSpace(codigo))
                throw new ArgumentException("codigo requerido", nameof(codigo));
            if (empresaId <= 0)
                throw new ArgumentException("empresaId inválido", nameof(empresaId));

            modulo = modulo.Trim().ToUpperInvariant();
            codigo = codigo.Trim().ToUpperInvariant();

            using var cn = Db.GetOpenConnection();

            using var cmd = new SqlCommand(@"
SELECT TOP(1)
    ReporteId, Modulo, Actividad, Codigo, Nombre, Motor, RutaArchivo, Orden
FROM dbo.ReporteDef
WHERE EsActivo = 1
  AND Modulo = @m
  AND Codigo = @c
ORDER BY ISNULL(Orden, 999999), ReporteId;", cn);

            cmd.Parameters.Add("@m", SqlDbType.VarChar, 50).Value = modulo;
            cmd.Parameters.Add("@c", SqlDbType.VarChar, 80).Value = codigo;

            using var rd = cmd.ExecuteReader(CommandBehavior.SingleRow);
            if (!rd.Read()) return null;

            return Map(rd);
        }

        // =========================================================
        // Helpers
        // =========================================================
        private static ReporteDefDto Map(SqlDataReader rd)
        {
            return new ReporteDefDto
            {
                ReporteId = SafeGetInt(rd, "ReporteId"),
                Modulo = SafeGetString(rd, "Modulo"),
                Actividad = SafeGetString(rd, "Actividad"),
                Codigo = SafeGetString(rd, "Codigo"),
                Nombre = SafeGetString(rd, "Nombre"),
                Motor = SafeGetString(rd, "Motor"),
                RutaArchivo = SafeGetString(rd, "RutaArchivo"),
                Orden = SafeGetInt(rd, "Orden")
            };
        }

        private static string SafeGetString(SqlDataReader rd, string col)
        {
            int i;
            try { i = rd.GetOrdinal(col); }
            catch { return ""; }

            return rd.IsDBNull(i) ? "" : (rd.GetString(i) ?? "");
        }

        private static int SafeGetInt(SqlDataReader rd, string col)
        {
            int i;
            try { i = rd.GetOrdinal(col); }
            catch { return 0; }

            return rd.IsDBNull(i) ? 0 : rd.GetInt32(i);
        }
    }
}
#nullable restore
