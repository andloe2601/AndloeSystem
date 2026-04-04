#nullable enable
using Andloe.Entidad;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace Andloe.Data
{
    public sealed class ContabilidadConfigRepository
    {
        public List<ContaConfigRow> ListarPorModulo(string modulo)
        {
            var list = new List<ContaConfigRow>();

            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
SELECT 
    cfg.ConfigId,
    cfg.Modulo,
    cfg.Evento,
    cfg.Rol,
    cfg.Naturaleza,
    cfg.Orden,
    cfg.CuentaId,
    cfg.CuentaCodigo,
    c.Nombre AS CuentaNombre,
    cfg.Activo,
    cfg.FechaCreacion
FROM dbo.ContabilidadConfig cfg
LEFT JOIN dbo.ContabilidadCatalogoCuenta c
    ON c.CuentaId = cfg.CuentaId
WHERE cfg.Modulo = @Modulo
ORDER BY cfg.Evento, cfg.Orden, cfg.Rol;", cn);

            cmd.Parameters.Add("@Modulo", SqlDbType.VarChar, 30).Value = modulo.Trim().ToUpperInvariant();

            using var rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                list.Add(new ContaConfigRow
                {
                    ConfigId = rd.GetInt32(0),
                    Modulo = rd.GetString(1),
                    Evento = rd.GetString(2),
                    Rol = rd.GetString(3),
                    Naturaleza = rd.GetString(4),
                    Orden = rd.GetInt32(5),
                    CuentaId = rd.IsDBNull(6) ? (int?)null : rd.GetInt32(6),
                    CuentaCodigo = rd.IsDBNull(7) ? null : rd.GetString(7),
                    CuentaNombre = rd.IsDBNull(8) ? null : rd.GetString(8),
                    Activo = rd.GetBoolean(9),
                    FechaCreacion = rd.GetDateTime(10)
                });
            }

            return list;
        }

        public List<CuentaLookupRow> ListarCuentasActivas()
        {
            var list = new List<CuentaLookupRow>();

            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
SELECT CuentaId, Codigo, Nombre
FROM dbo.ContabilidadCatalogoCuenta
WHERE Estado = 1
ORDER BY Codigo;", cn);

            using var rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                list.Add(new CuentaLookupRow
                {
                    CuentaId = rd.GetInt32(0),
                    Codigo = rd.GetString(1),
                    Nombre = rd.GetString(2)
                });
            }

            return list;
        }

        /// <summary>
        /// Inserta o actualiza por (Modulo, Evento, Rol) (único).
        /// Resuelve CuentaId por CuentaCodigo (si viene).
        /// </summary>
        public int Upsert(ContaConfigRow r)
        {
            if (r == null) throw new ArgumentNullException(nameof(r));
            if (string.IsNullOrWhiteSpace(r.Modulo)) throw new ArgumentException("Modulo requerido.");
            if (string.IsNullOrWhiteSpace(r.Evento)) throw new ArgumentException("Evento requerido.");
            if (string.IsNullOrWhiteSpace(r.Rol)) throw new ArgumentException("Rol requerido.");

            var modulo = r.Modulo.Trim().ToUpperInvariant();
            var evento = r.Evento.Trim().ToUpperInvariant();
            var rol = r.Rol.Trim().ToUpperInvariant();
            var nat = (r.Naturaleza ?? "DEBITO").Trim().ToUpperInvariant();

            if (nat != "DEBITO" && nat != "CREDITO")
                throw new ArgumentException("Naturaleza debe ser DEBITO o CREDITO.");

            using var cn = Db.GetOpenConnection();
            using var tx = cn.BeginTransaction();

            try
            {
                // Resolver CuentaId por Codigo si viene
                int? cuentaId = null;
                string? cuentaCodigo = null;

                if (!string.IsNullOrWhiteSpace(r.CuentaCodigo))
                {
                    cuentaCodigo = r.CuentaCodigo!.Trim();

                    using var cmdCta = new SqlCommand(@"
SELECT TOP(1) CuentaId
FROM dbo.ContabilidadCatalogoCuenta
WHERE Codigo = @Codigo AND Estado = 1;", cn, tx);

                    cmdCta.Parameters.Add("@Codigo", SqlDbType.VarChar, 20).Value = cuentaCodigo;

                    var obj = cmdCta.ExecuteScalar();
                    cuentaId = (obj == null || obj == DBNull.Value) ? (int?)null : Convert.ToInt32(obj);

                    if (!cuentaId.HasValue)
                        throw new InvalidOperationException($"CuentaCódigo '{cuentaCodigo}' no existe o está inactiva.");
                }

                using var cmd = new SqlCommand(@"
MERGE dbo.ContabilidadConfig AS T
USING (SELECT @Modulo AS Modulo, @Evento AS Evento, @Rol AS Rol) AS S
ON (T.Modulo = S.Modulo AND T.Evento = S.Evento AND T.Rol = S.Rol)
WHEN MATCHED THEN
    UPDATE SET
        Naturaleza   = @Naturaleza,
        Orden        = @Orden,
        CuentaId     = @CuentaId,
        CuentaCodigo = @CuentaCodigo,
        Activo       = @Activo
WHEN NOT MATCHED THEN
    INSERT (Modulo, Evento, Rol, Naturaleza, Orden, CuentaId, CuentaCodigo, Activo, FechaCreacion)
    VALUES (@Modulo, @Evento, @Rol, @Naturaleza, @Orden, @CuentaId, @CuentaCodigo, @Activo, GETDATE())
OUTPUT inserted.ConfigId;", cn, tx);

                cmd.Parameters.Add("@Modulo", SqlDbType.VarChar, 30).Value = modulo;
                cmd.Parameters.Add("@Evento", SqlDbType.VarChar, 40).Value = evento;
                cmd.Parameters.Add("@Rol", SqlDbType.VarChar, 60).Value = rol;
                cmd.Parameters.Add("@Naturaleza", SqlDbType.VarChar, 10).Value = nat;

                cmd.Parameters.Add("@Orden", SqlDbType.Int).Value = r.Orden <= 0 ? 1 : r.Orden;

                cmd.Parameters.Add("@CuentaId", SqlDbType.Int).Value = cuentaId.HasValue ? cuentaId.Value : DBNull.Value;
                cmd.Parameters.Add("@CuentaCodigo", SqlDbType.VarChar, 20).Value = (object?)cuentaCodigo ?? DBNull.Value;

                cmd.Parameters.Add("@Activo", SqlDbType.Bit).Value = r.Activo;

                var configIdObj = cmd.ExecuteScalar();
                var configId = Convert.ToInt32(configIdObj);

                tx.Commit();
                return configId;
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }

        public void Eliminar(int configId)
        {
            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"DELETE FROM dbo.ContabilidadConfig WHERE ConfigId=@Id;", cn);
            cmd.Parameters.Add("@Id", SqlDbType.Int).Value = configId;
            cmd.ExecuteNonQuery();
        }
    }
}
#nullable restore
