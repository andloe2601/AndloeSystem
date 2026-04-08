using System;

namespace Andloe.Entidad
{
    public class Moneda
    {
        public string MonedaCodigo { get; set; } = "";
        public string Nombre { get; set; } = "";
        public string? Simbolo { get; set; }
        public byte Decimales { get; set; }
        public bool Estado { get; set; }
        public DateTime FechaCreacion { get; set; }
    }
}