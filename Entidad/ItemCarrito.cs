namespace Andloe.Entidad
{
    public class ItemCarrito
    {
        public string ProductoCodigo { get; set; } = "";
        public string Descripcion { get; set; } = "";
        public decimal Cantidad { get; set; }
        public decimal PrecioUnit { get; set; }
        public decimal ItbisPct { get; set; }



        // =====================================
        //  NUEVOS CAMPOS PARA DESCUENTOS
        // =====================================

        // *** NUEVO DESCUENTO
        public decimal DescuentoPct { get; set; } = 0m;

        // *** NUEVO DESCUENTO
        public decimal DescuentoMonto { get; set; } = 0m;

        // =====================================
        //  CAMPOS CALCULADOS
        // =====================================

        // *** NUEVO DESCUENTO
        public decimal SubtotalBruto =>
            Cantidad * PrecioUnit;

        // *** NUEVO DESCUENTO
        public decimal SubtotalNeto =>
            SubtotalBruto - DescuentoMonto;

        public decimal ItbisMonto =>
            Math.Round(SubtotalNeto * (ItbisPct / 100m), 2);

        public decimal Total =>
            Math.Round(SubtotalNeto + ItbisMonto, 2);

        // Compatibilidad con lo existente:
        public decimal Importe =>
            SubtotalNeto; // Igual que antes, pero ahora con descuento aplicado
    }
}
