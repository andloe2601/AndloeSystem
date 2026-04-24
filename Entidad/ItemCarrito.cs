namespace Andloe.Entidad
{
    public class ItemCarrito
    {
        public string ProductoCodigo { get; set; } = "";
        public string Descripcion { get; set; } = "";
        public decimal Cantidad { get; set; }
        public decimal PrecioUnit { get; set; }
        public decimal ItbisPct { get; set; }
        public bool PrecioIncluyeITBIS { get; set; }

        public decimal DescuentoPct { get; set; } = 0m;
        public decimal DescuentoMonto { get; set; } = 0m;

        public decimal SubtotalBruto => Math.Round(Cantidad * PrecioUnit, 2);

        public decimal SubtotalNeto
        {
            get
            {
                var neto = SubtotalBruto - DescuentoMonto;
                return neto < 0m ? 0m : Math.Round(neto, 2);
            }
        }

        public decimal Importe
        {
            get
            {
                if (!PrecioIncluyeITBIS || ItbisPct <= 0m)
                    return SubtotalNeto;

                return Math.Round(SubtotalNeto / (1 + (ItbisPct / 100m)), 2);
            }
        }

        public decimal ItbisMonto
        {
            get
            {
                if (ItbisPct <= 0m)
                    return 0m;

                if (PrecioIncluyeITBIS)
                    return Math.Round(SubtotalNeto - Importe, 2);

                return Math.Round(Importe * (ItbisPct / 100m), 2);
            }
        }

        public decimal Total => Math.Round(Importe + ItbisMonto, 2);
    }
}