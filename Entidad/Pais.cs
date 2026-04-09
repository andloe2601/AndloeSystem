using System;

namespace Andloe.Entidad
{
    public class Pais
    {
        public int PaisId { get; set; }
        public string CodigoIso { get; set; } = "";
        public string Nombre { get; set; } = "";
        public bool Estado { get; set; }
        public DateTime FechaCreacion { get; set; }
    }
}