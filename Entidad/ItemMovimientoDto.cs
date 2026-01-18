using System;

namespace Andloe.Entidad
{
    public class ItemMovimientoDto
    {
        public string ProductoCodigo { get; set; } = "";
        public string Descripcion { get; set; } = "";
        public decimal Cantidad { get; set; }
        public decimal CostoUnitario { get; set; }

        public decimal Importe => Math.Round(Cantidad * CostoUnitario, 2);
    }
}
