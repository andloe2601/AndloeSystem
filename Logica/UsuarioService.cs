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

        private static readonly HashSet<string> EstadosValidos =
            new(StringComparer.OrdinalIgnoreCase)
            {
                "Activo",
                "Inactivo",
                "Bloqueado"
            };

        public List<Usuario> Listar(string? filtro = null, int top = 200)
            => _repo.Listar(filtro, top);

        public Usuario? Obtener(int id)
        {
            if (id <= 0) throw new InvalidOperationException("Id inválido.");
            return _repo.ObtenerPorId(id);
        }

        public int Crear(string usuario, string? email, string estado, string passwordPlano)
        {
            usuario = (usuario ?? "").Trim();
            email = string.IsNullOrWhiteSpace(email) ? null : email.Trim();
            estado = string.IsNullOrWhiteSpace(estado) ? "Activo" : estado.Trim();

            if (usuario.Length == 0) throw new InvalidOperationException("Usuario requerido.");
            if (!EstadosValidos.Contains(estado)) throw new InvalidOperationException("Estado inválido.");
            if (_repo.ExisteUsuario(usuario)) throw new InvalidOperationException("El usuario ya existe.");
            if (!string.IsNullOrEmpty(email) && _repo.ExisteEmail(email)) throw new InvalidOperationException("El email ya existe.");
            if (string.IsNullOrWhiteSpace(passwordPlano) || passwordPlano.Length < 8)
                throw new InvalidOperationException("La contraseña debe tener al menos 8 caracteres.");

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

        public void Actualizar(int id, string usuario, string? email, string estado)
        {
            if (id <= 0) throw new InvalidOperationException("Id inválido.");

            usuario = (usuario ?? "").Trim();
            email = string.IsNullOrWhiteSpace(email) ? null : email.Trim();
            estado = string.IsNullOrWhiteSpace(estado) ? "Activo" : estado.Trim();

            if (usuario.Length == 0) throw new InvalidOperationException("Usuario requerido.");
            if (!EstadosValidos.Contains(estado)) throw new InvalidOperationException("Estado inválido.");
            if (_repo.ExisteUsuario(usuario, id)) throw new InvalidOperationException("El usuario ya existe.");
            if (!string.IsNullOrEmpty(email) && _repo.ExisteEmail(email, id)) throw new InvalidOperationException("El email ya existe.");

            var u = _repo.ObtenerPorId(id) ?? throw new InvalidOperationException("Usuario no encontrado.");
            u.UsuarioNombre = usuario;
            u.Email = email ?? "";
            u.Estado = estado;

            _repo.Actualizar(u);
        }

        public void CambiarPassword(int id, string nuevaClave)
        {
            if (id <= 0) throw new InvalidOperationException("Id inválido.");
            if (string.IsNullOrWhiteSpace(nuevaClave) || nuevaClave.Length < 8)
                throw new InvalidOperationException("La contraseña debe tener al menos 8 caracteres.");

            var (hash, salt) = HashWithSalt(nuevaClave);
            _repo.CambiarPassword(id, hash, salt);
        }

        public void Eliminar(int id)
        {
            if (id <= 0) throw new InvalidOperationException("Id inválido.");
            _repo.Eliminar(id);
        }

        private static (byte[] hash, byte[] salt) HashWithSalt(string password)
        {
            var salt = RandomNumberGenerator.GetBytes(32);
            var passBytes = Encoding.Unicode.GetBytes(password);
            var toHash = new byte[salt.Length + passBytes.Length];

            Buffer.BlockCopy(salt, 0, toHash, 0, salt.Length);
            Buffer.BlockCopy(passBytes, 0, toHash, salt.Length, passBytes.Length);

            var hash = SHA256.HashData(toHash);
            return (hash, salt);
        }

        public List<Rol> ListarRolesSistema()
            => _repo.ListarTodosLosRoles();

        public List<Rol> ObtenerRolesDeUsuario(int usuarioId)
        {
            if (usuarioId <= 0) throw new InvalidOperationException("Id inválido.");
            return _repo.ObtenerRoles(usuarioId);
        }

        public void GuardarRolesDeUsuario(int usuarioId, IEnumerable<int> rolIds)
        {
            if (usuarioId <= 0) throw new InvalidOperationException("Id inválido.");
            _repo.ReemplazarRolesUsuario(usuarioId, rolIds);
        }

        public List<(int EmpresaId, string Nombre)> ListarEmpresas()
    => _repo.ListarEmpresas();

        public int? ObtenerEmpresaDeUsuario(int usuarioId)
        {
            if (usuarioId <= 0) throw new InvalidOperationException("Id inválido.");
            return _repo.ObtenerEmpresaDeUsuario(usuarioId);
        }

        public void GuardarEmpresaDeUsuario(int usuarioId, int empresaId, string? rol = null)
        {
            if (usuarioId <= 0) throw new InvalidOperationException("Id inválido.");
            if (empresaId <= 0) throw new InvalidOperationException("Empresa inválida.");

            _repo.GuardarEmpresaDeUsuario(usuarioId, empresaId, rol);
        }
    }
}