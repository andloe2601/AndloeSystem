#nullable enable
using System;
using System.Data;
using Microsoft.Data.SqlClient;
using Andloe.Data;

namespace Andloe.Logica
{
    public sealed class SesionActual
    {
        public int UsuarioId { get; init; }
        public string Usuario { get; init; } = "";
        public int EmpresaId { get; init; }
        public int SucursalId { get; init; }
        public int AlmacenId { get; init; }

        public int? CajaId { get; private set; }

        public bool EstaCompletaBase()
            => UsuarioId > 0 && EmpresaId > 0 && SucursalId > 0 && AlmacenId > 0;

        public void SetCaja(int cajaId) => CajaId = (cajaId > 0) ? cajaId : null;
        public void ClearCaja() => CajaId = null;
    }

    public static class SesionService
    {
        private static SesionActual? _current;

        /// <summary>
        /// Úsalo cuando quieras exigir sesión completa (lanza si no está lista).
        /// </summary>
        public static SesionActual Current
        {
            get
            {
                var s = _current;
                if (s is null || !s.EstaCompletaBase())
                    throw new InvalidOperationException("Sesión no inicializada: debe iniciar sesión y tener Empresa/Sucursal/Almacén.");
                return s;
            }
        }

        /// <summary>
        /// Devuelve null si no hay sesión (NO lanza).
        /// FormPrincipal lo usa.
        /// </summary>
        public static SesionActual? TryGet() => _current;

        public static void Cerrar() => _current = null;

        /// <summary>
        /// Inicia sesión base:
        /// - Usuario + Empresa seleccionada
        /// - Resuelve Sucursal/Almacén así:
        ///   (1) UsuarioContexto (SP dbo.UsuarioContexto_Get) si coincide Empresa
        ///   (2) Defaults del sistema (SistemaConfig SUCURSAL_DEFECTO_ID / ALMACEN_DEFECTO_ID) si son válidos
        ///   (3) Fallback: primera Sucursal/Almacén activos de la empresa
        /// </summary>
        public static void Iniciar(int usuarioId, string usuario, int empresaId)
        {
            if (usuarioId <= 0) throw new ArgumentException("UsuarioId inválido.", nameof(usuarioId));
            if (empresaId <= 0) throw new ArgumentException("EmpresaId inválido.", nameof(empresaId));

            usuario = (usuario ?? "").Trim();
            if (usuario.Length == 0) usuario = $"USR{usuarioId}";

            var (sucDefault, almDefault) = TryGetDefaultsFromConfig();

            // ✅ OJO: aquí VA usuarioId también (ya no da error)
            var (sucursalId, almacenId) = ResolverSucursalAlmacenParaEmpresa(usuarioId, empresaId, sucDefault, almDefault);

            _current = new SesionActual
            {
                UsuarioId = usuarioId,
                Usuario = usuario,
                EmpresaId = empresaId,
                SucursalId = sucursalId,
                AlmacenId = almacenId
            };
        }

        public static void SetCaja(int cajaId)
        {
            if (_current is null) throw new InvalidOperationException("No hay sesión iniciada.");
            _current.SetCaja(cajaId);
        }

        public static void ClearCaja()
        {
            if (_current is null) return;
            _current.ClearCaja();
        }

        // =========================
        // Helpers internos
        // =========================

        private static (int sucursalId, int almacenId) TryGetDefaultsFromConfig()
        {
            try
            {
                var cfg = new SistemaConfigRepository();
                var sucTxt = cfg.GetValor("SUCURSAL_DEFECTO_ID");
                var almTxt = cfg.GetValor("ALMACEN_DEFECTO_ID");

                int.TryParse(sucTxt, out var sucId);
                int.TryParse(almTxt, out var almId);

                return (sucId, almId);
            }
            catch
            {
                return (0, 0);
            }
        }

        private static (int sucursalId, int almacenId) ResolverSucursalAlmacenParaEmpresa(
            int usuarioId,
            int empresaId,
            int sucDefault,
            int almDefault)
        {
            // 1) UsuarioContexto (SP) si coincide Empresa
            var ctx = TryGetUsuarioContexto(usuarioId);
            if (ctx != null &&
                ctx.EmpresaId == empresaId &&
                ctx.SucursalId > 0 &&
                ctx.AlmacenId > 0)
            {
                return (ctx.SucursalId, ctx.AlmacenId);
            }

            using var cn = Db.GetOpenConnection();

            // 2) Defaults del sistema (si son válidos para esa empresa)
            if (sucDefault > 0 && almDefault > 0)
            {
                using var cmdVal = new SqlCommand(@"
IF EXISTS (
    SELECT 1
    FROM dbo.Sucursal s
    JOIN dbo.Almacen a ON a.SucursalId = s.SucursalId AND a.EmpresaId = s.EmpresaId
    WHERE s.EmpresaId = @emp
      AND s.SucursalId = @suc AND s.Estado = 1
      AND a.AlmacenId  = @alm AND a.Estado = 1
)
    SELECT 1
ELSE
    SELECT 0;", cn);

                cmdVal.Parameters.Add("@emp", SqlDbType.Int).Value = empresaId;
                cmdVal.Parameters.Add("@suc", SqlDbType.Int).Value = sucDefault;
                cmdVal.Parameters.Add("@alm", SqlDbType.Int).Value = almDefault;

                var ok = Convert.ToInt32(cmdVal.ExecuteScalar() ?? 0) == 1;
                if (ok)
                    return (sucDefault, almDefault);
            }

            // 3) Fallback: primera sucursal + primer almacén activos
            using var cmd = new SqlCommand(@"
;WITH S AS (
    SELECT TOP(1) SucursalId
    FROM dbo.Sucursal
    WHERE EmpresaId = @emp AND Estado = 1
    ORDER BY Codigo, Nombre, SucursalId
),
A AS (
    SELECT TOP(1) a.AlmacenId, a.SucursalId
    FROM dbo.Almacen a
    JOIN S ON S.SucursalId = a.SucursalId
    WHERE a.EmpresaId = @emp AND a.Estado = 1
    ORDER BY a.Codigo, a.Nombre, a.AlmacenId
)
SELECT
    (SELECT TOP(1) SucursalId FROM S)  AS SucursalId,
    (SELECT TOP(1) AlmacenId  FROM A)  AS AlmacenId;", cn);

            cmd.Parameters.Add("@emp", SqlDbType.Int).Value = empresaId;

            using var rd = cmd.ExecuteReader();
            if (!rd.Read())
                throw new InvalidOperationException("No se pudo resolver Sucursal/Almacén para la empresa seleccionada.");

            var sucursalId = rd.IsDBNull(0) ? 0 : rd.GetInt32(0);
            var almacenId = rd.IsDBNull(1) ? 0 : rd.GetInt32(1);

            if (sucursalId <= 0)
                throw new InvalidOperationException("La empresa seleccionada no tiene sucursales activas (Estado=1).");

            if (almacenId <= 0)
                throw new InvalidOperationException("La sucursal seleccionada no tiene almacenes activos (Estado=1).");

            return (sucursalId, almacenId);
        }

        private sealed class UsuarioContextoDto
        {
            public int UsuarioId { get; set; }
            public int EmpresaId { get; set; }
            public int SucursalId { get; set; }
            public int AlmacenId { get; set; }
        }

        private static UsuarioContextoDto? TryGetUsuarioContexto(int usuarioId)
        {
            try
            {
                using var cn = Db.GetOpenConnection();
                using var cmd = new SqlCommand("dbo.UsuarioContexto_Get", cn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.Add("@UsuarioId", SqlDbType.Int).Value = usuarioId;

                using var rd = cmd.ExecuteReader();
                if (!rd.Read()) return null;

                // Tu SP:
                // 0 uc.UsuarioId
                // 1 uc.EmpresaId
                // 2 e.RazonSocial
                // 3 uc.SucursalId
                // 4 s.Codigo
                // 5 s.Nombre
                // 6 uc.AlmacenId
                // 7 a.Codigo
                // 8 a.Nombre
                return new UsuarioContextoDto
                {
                    UsuarioId = rd.GetInt32(0),
                    EmpresaId = rd.GetInt32(1),
                    SucursalId = rd.GetInt32(3),
                    AlmacenId = rd.GetInt32(6),
                };
            }
            catch
            {
                return null;
            }
        }
    }
}
#nullable restore
