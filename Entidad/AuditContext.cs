using System;

namespace Andloe.Logica
{
    public static class AuditContext
    {
        public static int? UsuarioId { get; private set; }
        public static string? Usuario { get; private set; }

        public static string Maquina { get; private set; } = Environment.MachineName;
        public static string? Ip { get; private set; } = null;

        public static void SetUser(int? usuarioId, string? usuario)
        {
            UsuarioId = usuarioId;
            Usuario = usuario;
        }

        public static void ClearUser()
        {
            UsuarioId = null;
            Usuario = null;
        }

        public static void SetIp(string? ip)
        {
            Ip = ip;
        }
    }
}
