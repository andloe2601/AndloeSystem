namespace Andloe.Entidad
{
    public class VentaLinea
    {
        public long VentaId { get; set; }
        public int Linea { get; set; }
        public string ProductoCodigo { get; set; } = "";
        public string? Descripcion { get; set; }
        public decimal Cantidad { get; set; }
        public decimal PrecioUnit { get; set; }
        public decimal ItbisPct { get; set; }
        public decimal ItbisMonto { get; set; }
        public decimal Importe { get; set; }
        public decimal TotalLinea { get; set; }

        public decimal DescuentoPct { get; set; }      // % aplicado
        public decimal DescuentoMonto { get; set; }    // monto rebajado
    }
}
