using System;

namespace Andloe.Entidad
{
    public class KardexMovimientoDto
    {
        public DateTime Fecha { get; set; }
        public string Tipo { get; set; } = "";
        public string Origen { get; set; } = "";
        public string NumeroDocumento { get; set; } = "";
        public string Almacen { get; set; } = "";
        public decimal Entrada { get; set; }
        public decimal Salida { get; set; }
        public decimal Existencia { get; set; }
    }
}
