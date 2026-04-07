using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Andloe.Data;
using Andloe.Entidad;

namespace Andloe.Logica
{
    public class AuthService
    {
        private readonly UsuarioRepository _repo = new();
        private const int MAX_INTENTOS = 5;
        private static readonly TimeSpan BLOQUEO = TimeSpan.FromMinutes(15);

        public AuthResult Login(string usuario, string password)
        {
            usuario = (usuario ?? string.Empty).Trim();

            var u = _repo.ObtenerPorUsuario(usuario);
            if (u is null)
                return Fail("No fue posible iniciar sesión con este usuario.");

            if (!string.Equals(u.Estado, "Activo", StringComparison.OrdinalIgnoreCase))
                return Fail($"Usuario en estado '{u.Estado}'.");

            if (u.BloqueadoHasta is not null && u.BloqueadoHasta > DateTime.UtcNow)
            {
                var mins = (int)Math.Ceiling((u.BloqueadoHasta.Value - DateTime.UtcNow).TotalMinutes);
                return Fail($"Usuario bloqueado. Intente en {Math.Max(mins, 1)} minuto(s).");
            }

            var passBytes = Encoding.Unicode.GetBytes(password ?? string.Empty);
            var toHash = new byte[u.Salt.Length + passBytes.Length];
            Buffer.BlockCopy(u.Salt, 0, toHash, 0, u.Salt.Length);
            Buffer.BlockCopy(passBytes, 0, toHash, u.Salt.Length, passBytes.Length);

            var hash = SHA256.HashData(toHash);

            if (!FixedTimeEquals(hash, u.HashPassword))
            {
                var intentos = u.IntentosFallidos + 1;
                DateTime? bloqueado = null;

                if (intentos >= MAX_INTENTOS)
                {
                    bloqueado = DateTime.UtcNow.Add(BLOQUEO);
                    intentos = 0;
                }

                _repo.RegistrarAccesoFallido(u.UsuarioId, intentos, bloqueado);

                return Fail(bloqueado is null
                    ? $"Credenciales inválidas. Intentos: {intentos}/{MAX_INTENTOS}."
                    : $"Demasiados intentos. Usuario bloqueado {BLOQUEO.TotalMinutes:0} minutos.");
            }

            _repo.RegistrarAccesoExitoso(u.UsuarioId);

            var roles = _repo.ObtenerRoles(u.UsuarioId)
                .Select(r => r.Nombre)
                .Where(n => !string.IsNullOrWhiteSpace(n))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            u.HashPassword = Array.Empty<byte>();
            u.Salt = Array.Empty<byte>();

            return new AuthResult
            {
                Exitoso = true,
                Mensaje = "Login OK.",
                Usuario = u,
                Roles = roles
            };

            static AuthResult Fail(string msg) => new() { Exitoso = false, Mensaje = msg };
        }

        private static bool FixedTimeEquals(byte[] a, byte[] b)
        {
            if (a is null || b is null || a.Length != b.Length)
                return false;

            var diff = 0;
            for (int i = 0; i < a.Length; i++)
                diff |= a[i] ^ b[i];

            return diff == 0;
        }
    }
}