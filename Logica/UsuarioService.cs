using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Andloe.Data;
using Andloe.Entidad;

namespace Andloe.Logica
{
    public class UsuarioService
    {
        private readonly UsuarioRepository _repo = new();

        /// <summary>Lista usuarios con filtro opcional (usuario/email) y TOP configurable.</summary>
        public List<Usuario> Listar(string? filtro = null, int top = 200)
            => _repo.Listar(filtro, top);

        /// <summary>Obtiene un usuario por Id.</summary>
        public Usuario? Obtener(int id)
            => _repo.ObtenerPorId(id);

        /// <summary>Crea un usuario con contraseña (hash + salt).</summary>
        public int Crear(string usuario, string? email, string estado, string passwordPlano)
        {
            usuario = (usuario ?? "").Trim();
            email = string.IsNullOrWhiteSpace(email) ? null : email!.Trim();
            estado = string.IsNullOrWhiteSpace(estado) ? "Activo" : estado.Trim();

            if (usuario.Length == 0) throw new InvalidOperationException("Usuario requerido.");
            if (_repo.ExisteUsuario(usuario)) throw new InvalidOperationException("El usuario ya existe.");
            if (!string.IsNullOrEmpty(email) && _repo.ExisteEmail(email)) throw new InvalidOperationException("El email ya existe.");
            if (string.IsNullOrEmpty(passwordPlano)) throw new InvalidOperationException("Contraseña requerida.");

            var (hash, salt) = HashWithSalt(passwordPlano);

            var u = new Usuario
            {
                UsuarioNombre = usuario,
                Email = email ?? "",
                Estado = estado,
                HashPassword = hash,
                Salt = salt,
                IntentosFallidos = 0,
                BloqueadoHasta = null
            };

            return _repo.Crear(u);
        }

        /// <summary>Actualiza datos básicos (sin cambiar contraseña).</summary>
        public void Actualizar(int id, string usuario, string? email, string estado)
        {
            usuario = (usuario ?? "").Trim();
            email = string.IsNullOrWhiteSpace(email) ? null : email!.Trim();
            estado = string.IsNullOrWhiteSpace(estado) ? "Activo" : estado.Trim();

            if (usuario.Length == 0) throw new InvalidOperationException("Usuario requerido.");
            if (_repo.ExisteUsuario(usuario, id)) throw new InvalidOperationException("El usuario ya existe.");
            if (!string.IsNullOrEmpty(email) && _repo.ExisteEmail(email, id)) throw new InvalidOperationException("El email ya existe.");

            var u = _repo.ObtenerPorId(id) ?? throw new InvalidOperationException("Usuario no encontrado.");
            u.UsuarioNombre = usuario;
            u.Email = email ?? "";
            u.Estado = estado;

            _repo.Actualizar(u);
        }

        /// <summary>Cambia la contraseña del usuario (hash + salt).</summary>
        public void CambiarPassword(int id, string nuevaClave)
        {
            if (string.IsNullOrEmpty(nuevaClave)) throw new InvalidOperationException("Contraseña requerida.");
            var (hash, salt) = HashWithSalt(nuevaClave);
            _repo.CambiarPassword(id, hash, salt);
        }

        /// <summary>Elimina el usuario (y sus roles relacionados).</summary>
        public void Eliminar(int id)
            => _repo.Eliminar(id);

        /// <summary>
        /// Genera hash SHA-256 compatible con SQL: HASHBYTES('SHA2_256', Salt + CAST(@pass AS VARBINARY(NV)))
        /// NVARCHAR usa UTF-16 LE → Encoding.Unicode.
        /// </summary>
        private static (byte[] hash, byte[] salt) HashWithSalt(string password)
        {
            var salt = RandomNumberGenerator.GetBytes(32);
            var passBytes = Encoding.Unicode.GetBytes(password);
            var toHash = new byte[salt.Length + passBytes.Length];
            Buffer.BlockCopy(salt, 0, toHash, 0, salt.Length);
            Buffer.BlockCopy(passBytes, 0, toHash, salt.Length, passBytes.Length);
            using var sha = SHA256.Create();
            var hash = sha.ComputeHash(toHash);
            return (hash, salt);
        }
         
        // === ROLES ===
        public List<Rol> ListarRolesSistema()
            => _repo.ListarTodosLosRoles();

        public List<Rol> ObtenerRolesDeUsuario(int usuarioId)
            => _repo.ObtenerRoles(usuarioId);

        public void GuardarRolesDeUsuario(int usuarioId, IEnumerable<int> rolIds)
            => _repo.ReemplazarRolesUsuario(usuarioId, rolIds);
    }
}
