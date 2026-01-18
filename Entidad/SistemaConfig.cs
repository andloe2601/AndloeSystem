using System;

namespace Andloe.Entidad
{
   
    public class SistemaConfig
    {
        public string Clave { get; set; } = string.Empty;
        public string? Valor { get; set; }
        public string? Descripcion { get; set; }
        public string? Tipo { get; set; }
        public DateTime? FechaMod { get; set; }
        public string? UsuarioMod { get; set; }
    }
}
