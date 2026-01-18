using System.Collections.Generic;

namespace Andloe.Entidad
{
    public class AuthResult
    {
        public bool Exitoso { get; set; }
        public string Mensaje { get; set; } = "";
        public Usuario? Usuario { get; set; }
        public List<Rol> Roles { get; set; } = new();
    }
}
