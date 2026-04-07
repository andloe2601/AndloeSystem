using System.Collections.Generic;

namespace Andloe.Entidad
{
    public class AuthResult
    {
        public bool Exitoso { get; set; }
        public string Mensaje { get; set; } = "";
        public Usuario? Usuario { get; set; }

        // Para autorización/UI necesitamos nombres de roles
        public List<string> Roles { get; set; } = new();
    }
}