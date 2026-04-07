#nullable enable
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using Andloe.Entidad;

namespace Andloe.Data
{
    public class UsuarioRepository
    {
        public List<Usuario> Listar(string? filtro = null, int top = 200)
        {
            var list = new List<Usuario>();

            var f = string.IsNullOrWhiteSpace(filtro) ? null : filtro.Trim();
            var like = f == null ? null : "%" + f + "%";

            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
SELECT TOP(@top)
    UsuarioId,
    Usuario AS UsuarioNombre,
    Email,
    Estado,
    UltimoAcceso,
    ISNULL(IntentosFallidos,0) AS IntentosFallidos,
    BloqueadoHasta
FROM dbo.Usuario
WHERE (@filtro IS NULL OR Usuario LIKE @like OR Email LIKE @like)
ORDER BY UsuarioId DESC;", cn);

            cmd.Parameters.Add("@top", SqlDbType.Int).Value = top;
            cmd.Parameters.Add("@filtro", SqlDbType.NVarChar, 100).Value = (object?)f ?? DBNull.Value;
            cmd.Parameters.Add("@like", SqlDbType.NVarChar, 100).Value = (object?)like ?? DBNull.Value;

            using var rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                list.Add(new Usuario
                {
                    UsuarioId = rd.GetInt32(0),
                    UsuarioNombre = rd.GetString(1),
                    Email = rd.IsDBNull(2) ? "" : rd.GetString(2),
                    Estado = rd.IsDBNull(3) ? "" : rd.GetString(3),
                    UltimoAcceso = rd.IsDBNull(4) ? (DateTime?)null : rd.GetDateTime(4),
                    IntentosFallidos = rd.IsDBNull(5) ? 0 : rd.GetInt32(5),
                    BloqueadoHasta = rd.IsDBNull(6) ? (DateTime?)null : rd.GetDateTime(6)
                });
            }

            return list;
        }

        public Usuario? ObtenerPorId(int id)
        {
            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
SELECT
    UsuarioId,
    Usuario AS UsuarioNombre,
    Email,
    Estado,
    UltimoAcceso,
    HashPassword,
    Salt,
    ISNULL(IntentosFallidos,0) AS IntentosFallidos,
    BloqueadoHasta
FROM dbo.Usuario
WHERE UsuarioId = @id;", cn);

            cmd.Parameters.Add("@id", SqlDbType.Int).Value = id;

            using var rd = cmd.ExecuteReader();
            if (!rd.Read()) return null;

            return new Usuario
            {
                UsuarioId = rd.GetInt32(0),
                UsuarioNombre = rd.GetString(1),
                Email = rd.IsDBNull(2) ? "" : rd.GetString(2),
                Estado = rd.IsDBNull(3) ? "" : rd.GetString(3),
                UltimoAcceso = rd.IsDBNull(4) ? (DateTime?)null : rd.GetDateTime(4),
                HashPassword = rd.IsDBNull(5) ? Array.Empty<byte>() : (byte[])rd[5],
                Salt = rd.IsDBNull(6) ? Array.Empty<byte>() : (byte[])rd[6],
                IntentosFallidos = rd.IsDBNull(7) ? 0 : rd.GetInt32(7),
                BloqueadoHasta = rd.IsDBNull(8) ? (DateTime?)null : rd.GetDateTime(8)
            };
        }

        public Usuario? ObtenerPorUsuario(string usuario)
        {
            if (string.IsNullOrWhiteSpace(usuario))
                throw new ArgumentException("usuario requerido", nameof(usuario));

            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
SELECT TOP 1
    UsuarioId,
    Usuario AS UsuarioNombre,
    Email,
    Estado,
    UltimoAcceso,
    HashPassword,
    Salt,
    ISNULL(IntentosFallidos,0) AS IntentosFallidos,
    BloqueadoHasta
FROM dbo.Usuario
WHERE Usuario = @u;", cn);

            cmd.Parameters.Add("@u", SqlDbType.NVarChar, 100).Value = usuario.Trim();

            using var rd = cmd.ExecuteReader();
            if (!rd.Read()) return null;

            return new Usuario
            {
                UsuarioId = rd.GetInt32(0),
                UsuarioNombre = rd.GetString(1),
                Email = rd.IsDBNull(2) ? "" : rd.GetString(2),
                Estado = rd.IsDBNull(3) ? "" : rd.GetString(3),
                UltimoAcceso = rd.IsDBNull(4) ? (DateTime?)null : rd.GetDateTime(4),
                HashPassword = rd.IsDBNull(5) ? Array.Empty<byte>() : (byte[])rd[5],
                Salt = rd.IsDBNull(6) ? Array.Empty<byte>() : (byte[])rd[6],
                IntentosFallidos = rd.IsDBNull(7) ? 0 : rd.GetInt32(7),
                BloqueadoHasta = rd.IsDBNull(8) ? (DateTime?)null : rd.GetDateTime(8)
            };
        }

        public int ObtenerUsuarioIdPorNombre(string usuario)
        {
            if (string.IsNullOrWhiteSpace(usuario)) return 0;

            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
SELECT TOP(1) UsuarioId
FROM dbo.Usuario
WHERE Usuario = @u;", cn);

            cmd.Parameters.Add("@u", SqlDbType.NVarChar, 100).Value = usuario.Trim();

            var v = cmd.ExecuteScalar();
            if (v == null || v == DBNull.Value) return 0;
            return Convert.ToInt32(v);
        }

        public bool ExisteUsuario(string usuario, int? excluirId = null)
        {
            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
SELECT TOP(1) 1
FROM dbo.Usuario
WHERE Usuario = @u
  AND (@excluir IS NULL OR UsuarioId <> @excluir);", cn);

            cmd.Parameters.Add("@u", SqlDbType.NVarChar, 100).Value = usuario.Trim();
            cmd.Parameters.Add("@excluir", SqlDbType.Int).Value = (object?)excluirId ?? DBNull.Value;

            var v = cmd.ExecuteScalar();
            return v != null && v != DBNull.Value;
        }

        public bool ExisteEmail(string email, int? excluirId = null)
        {
            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
SELECT TOP(1) 1
FROM dbo.Usuario
WHERE Email = @e
  AND (@excluir IS NULL OR UsuarioId <> @excluir);", cn);

            cmd.Parameters.Add("@e", SqlDbType.NVarChar, 512).Value = email.Trim();
            cmd.Parameters.Add("@excluir", SqlDbType.Int).Value = (object?)excluirId ?? DBNull.Value;

            var v = cmd.ExecuteScalar();
            return v != null && v != DBNull.Value;
        }

        public int Crear(Usuario u)
        {
            if (u == null) throw new ArgumentNullException(nameof(u));
            if (string.IsNullOrWhiteSpace(u.UsuarioNombre))
                throw new ArgumentException("UsuarioNombre requerido", nameof(u));

            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
INSERT INTO dbo.Usuario (Usuario, Email, HashPassword, Salt, Estado, IntentosFallidos, BloqueadoHasta)
VALUES (@Usuario, @Email, @Hash, @Salt, @Estado, @Intentos, @Bloq);
SELECT CAST(SCOPE_IDENTITY() AS int);", cn);

            cmd.Parameters.Add("@Usuario", SqlDbType.NVarChar, 100).Value = u.UsuarioNombre.Trim();
            cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 512).Value =
                (object?)(string.IsNullOrWhiteSpace(u.Email) ? null : u.Email.Trim()) ?? DBNull.Value;
            cmd.Parameters.Add("@Hash", SqlDbType.VarBinary, 32).Value =
                (u.HashPassword != null && u.HashPassword.Length > 0) ? u.HashPassword : (object)DBNull.Value;
            cmd.Parameters.Add("@Salt", SqlDbType.VarBinary, 32).Value =
                (u.Salt != null && u.Salt.Length > 0) ? u.Salt : (object)DBNull.Value;
            cmd.Parameters.Add("@Estado", SqlDbType.NVarChar, 40).Value =
                string.IsNullOrWhiteSpace(u.Estado) ? "Activo" : u.Estado.Trim();
            cmd.Parameters.Add("@Intentos", SqlDbType.Int).Value = u.IntentosFallidos;
            cmd.Parameters.Add("@Bloq", SqlDbType.DateTime2).Value = (object?)u.BloqueadoHasta ?? DBNull.Value;

            return (int)cmd.ExecuteScalar()!;
        }

        public void Actualizar(Usuario u)
        {
            if (u == null) throw new ArgumentNullException(nameof(u));
            if (u.UsuarioId <= 0) throw new ArgumentException("UsuarioId inválido", nameof(u));
            if (string.IsNullOrWhiteSpace(u.UsuarioNombre))
                throw new ArgumentException("UsuarioNombre requerido", nameof(u));

            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
UPDATE dbo.Usuario
   SET Usuario = @Usuario,
       Email   = @Email,
       Estado  = @Estado
 WHERE UsuarioId = @Id;", cn);

            cmd.Parameters.Add("@Usuario", SqlDbType.NVarChar, 100).Value = u.UsuarioNombre.Trim();
            cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 512).Value =
                (object?)(string.IsNullOrWhiteSpace(u.Email) ? null : u.Email.Trim()) ?? DBNull.Value;
            cmd.Parameters.Add("@Estado", SqlDbType.NVarChar, 40).Value =
                string.IsNullOrWhiteSpace(u.Estado) ? "Activo" : u.Estado.Trim();
            cmd.Parameters.Add("@Id", SqlDbType.Int).Value = u.UsuarioId;

            cmd.ExecuteNonQuery();
        }

        public void CambiarPassword(int usuarioId, byte[] hash, byte[] salt)
        {
            if (usuarioId <= 0) throw new ArgumentException("usuarioId inválido", nameof(usuarioId));
            if (hash == null || hash.Length == 0) throw new ArgumentException("hash requerido", nameof(hash));
            if (salt == null || salt.Length == 0) throw new ArgumentException("salt requerido", nameof(salt));

            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
UPDATE dbo.Usuario
   SET HashPassword     = @Hash,
       Salt             = @Salt,
       IntentosFallidos = 0,
       BloqueadoHasta   = NULL
 WHERE UsuarioId = @Id;", cn);

            cmd.Parameters.Add("@Hash", SqlDbType.VarBinary, 32).Value = hash;
            cmd.Parameters.Add("@Salt", SqlDbType.VarBinary, 32).Value = salt;
            cmd.Parameters.Add("@Id", SqlDbType.Int).Value = usuarioId;

            cmd.ExecuteNonQuery();
        }

        public void Eliminar(int usuarioId)
        {
            using var cn = Db.GetOpenConnection();
            using var tx = cn.BeginTransaction();

            try
            {
                using (var cmd1 = new SqlCommand("DELETE FROM dbo.UsuarioRol WHERE UsuarioId = @Id;", cn, tx))
                {
                    cmd1.Parameters.Add("@Id", SqlDbType.Int).Value = usuarioId;
                    cmd1.ExecuteNonQuery();
                }

                using (var cmd2 = new SqlCommand("DELETE FROM dbo.Usuario WHERE UsuarioId = @Id;", cn, tx))
                {
                    cmd2.Parameters.Add("@Id", SqlDbType.Int).Value = usuarioId;
                    cmd2.ExecuteNonQuery();
                }

                tx.Commit();
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }

        public List<Rol> ObtenerRoles(int usuarioId)
        {
            var roles = new List<Rol>();

            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
SELECT r.RolId, r.Nombre
FROM dbo.UsuarioRol ur
INNER JOIN dbo.Rol r ON r.RolId = ur.RolId
WHERE ur.UsuarioId = @id
  AND ISNULL(r.Activo, 1) = 1
ORDER BY r.Nombre;", cn);

            cmd.Parameters.Add("@id", SqlDbType.Int).Value = usuarioId;

            using var rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                roles.Add(new Rol
                {
                    RolId = rd.GetInt32(0),
                    Nombre = rd.GetString(1)
                });
            }

            return roles;
        }

        public List<Rol> ListarTodosLosRoles()
        {
            var roles = new List<Rol>();

            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
SELECT RolId, Nombre
FROM dbo.Rol
WHERE ISNULL(Activo, 1) = 1
ORDER BY Nombre;", cn);

            using var rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                roles.Add(new Rol
                {
                    RolId = rd.GetInt32(0),
                    Nombre = rd.GetString(1)
                });
            }

            return roles;
        }

        public void ReemplazarRolesUsuario(int usuarioId, IEnumerable<int>? rolIds)
        {
            rolIds ??= Array.Empty<int>();

            using var cn = Db.GetOpenConnection();
            using var tx = cn.BeginTransaction();

            try
            {
                using (var del = new SqlCommand("DELETE FROM dbo.UsuarioRol WHERE UsuarioId = @u;", cn, tx))
                {
                    del.Parameters.Add("@u", SqlDbType.Int).Value = usuarioId;
                    del.ExecuteNonQuery();
                }

                foreach (var rid in rolIds)
                {
                    using var ins = new SqlCommand(@"
INSERT INTO dbo.UsuarioRol(UsuarioId, RolId)
VALUES (@u, @r);", cn, tx);

                    ins.Parameters.Add("@u", SqlDbType.Int).Value = usuarioId;
                    ins.Parameters.Add("@r", SqlDbType.Int).Value = rid;
                    ins.ExecuteNonQuery();
                }

                tx.Commit();
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }

        public void RegistrarAccesoExitoso(int usuarioId)
        {
            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
UPDATE dbo.Usuario
   SET UltimoAcceso      = SYSDATETIME(),
       IntentosFallidos  = 0,
       BloqueadoHasta    = NULL
 WHERE UsuarioId = @id;", cn);

            cmd.Parameters.Add("@id", SqlDbType.Int).Value = usuarioId;
            cmd.ExecuteNonQuery();
        }

        public void RegistrarAccesoFallido(int usuarioId, int nuevosIntentos, DateTime? bloqueadoHastaUtc)
        {
            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
UPDATE dbo.Usuario
   SET IntentosFallidos = @intentos,
       BloqueadoHasta   = @bloq
 WHERE UsuarioId = @id;", cn);

            cmd.Parameters.Add("@id", SqlDbType.Int).Value = usuarioId;
            cmd.Parameters.Add("@intentos", SqlDbType.Int).Value = nuevosIntentos;
            cmd.Parameters.Add("@bloq", SqlDbType.DateTime2).Value = (object?)bloqueadoHastaUtc ?? DBNull.Value;

            cmd.ExecuteNonQuery();
        }

        public (int EmpresaId, int SucursalId, int AlmacenId)? ObtenerContexto(int usuarioId)
        {
            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
SELECT TOP(1) EmpresaId, SucursalId, AlmacenId
FROM dbo.UsuarioContexto
WHERE UsuarioId = @u;", cn);

            cmd.Parameters.Add("@u", SqlDbType.Int).Value = usuarioId;

            using var rd = cmd.ExecuteReader();
            if (!rd.Read()) return null;

            return (rd.GetInt32(0), rd.GetInt32(1), rd.GetInt32(2));
        }

        public void UpsertContexto(int usuarioId, int empresaId, int sucursalId, int almacenId)
        {
            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
IF EXISTS (SELECT 1 FROM dbo.UsuarioContexto WHERE UsuarioId = @u)
BEGIN
    UPDATE dbo.UsuarioContexto
       SET EmpresaId   = @e,
           SucursalId  = @s,
           AlmacenId   = @a,
           FechaCambio = SYSDATETIME()
     WHERE UsuarioId   = @u;
END
ELSE
BEGIN
    INSERT INTO dbo.UsuarioContexto(UsuarioId, EmpresaId, SucursalId, AlmacenId, FechaCambio)
    VALUES(@u, @e, @s, @a, SYSDATETIME());
END", cn);

            cmd.Parameters.Add("@u", SqlDbType.Int).Value = usuarioId;
            cmd.Parameters.Add("@e", SqlDbType.Int).Value = empresaId;
            cmd.Parameters.Add("@s", SqlDbType.Int).Value = sucursalId;
            cmd.Parameters.Add("@a", SqlDbType.Int).Value = almacenId;

            cmd.ExecuteNonQuery();
        }
    

    public List<(int EmpresaId, string Nombre)> ListarEmpresas()
        {
            var list = new List<(int EmpresaId, string Nombre)>();

            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
SELECT EmpresaId, RazonSocial
FROM dbo.Empresa
WHERE Estado = 1
ORDER BY RazonSocial;", cn);

            using var rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                list.Add((rd.GetInt32(0), rd.GetString(1)));
            }

            return list;
        }

        public int? ObtenerEmpresaDeUsuario(int usuarioId)
        {
            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
SELECT TOP(1) EmpresaId
FROM dbo.UsuarioEmpresa
WHERE UsuarioId = @u
  AND Activo = 1;", cn);

            cmd.Parameters.Add("@u", SqlDbType.Int).Value = usuarioId;

            var v = cmd.ExecuteScalar();
            if (v == null || v == DBNull.Value)
                return null;

            return Convert.ToInt32(v);
        }

        public void GuardarEmpresaDeUsuario(int usuarioId, int empresaId, string? rol = null)
        {
            using var cn = Db.GetOpenConnection();
            using var tx = cn.BeginTransaction();

            try
            {
                using (var des = new SqlCommand(@"
UPDATE dbo.UsuarioEmpresa
SET Activo = 0
WHERE UsuarioId = @u;", cn, tx))
                {
                    des.Parameters.Add("@u", SqlDbType.Int).Value = usuarioId;
                    des.ExecuteNonQuery();
                }

                using (var cmd = new SqlCommand(@"
IF EXISTS (
    SELECT 1
    FROM dbo.UsuarioEmpresa
    WHERE UsuarioId = @u AND EmpresaId = @e
)
BEGIN
    UPDATE dbo.UsuarioEmpresa
       SET Activo = 1,
           Rol = @rol,
           FechaAlta = SYSDATETIME()
     WHERE UsuarioId = @u
       AND EmpresaId = @e;
END
ELSE
BEGIN
    INSERT INTO dbo.UsuarioEmpresa (UsuarioId, EmpresaId, Rol, Activo, FechaAlta)
    VALUES (@u, @e, @rol, 1, SYSDATETIME());
END", cn, tx))
                {
                    cmd.Parameters.Add("@u", SqlDbType.Int).Value = usuarioId;
                    cmd.Parameters.Add("@e", SqlDbType.Int).Value = empresaId;
                    cmd.Parameters.Add("@rol", SqlDbType.VarChar, 20).Value =
                        (object?)rol ?? DBNull.Value;

                    cmd.ExecuteNonQuery();
                }

                tx.Commit();
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }

    }
}
#nullable restore