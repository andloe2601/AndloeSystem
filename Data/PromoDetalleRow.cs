namespace Andloe.Data
//History of promotional details 
{
    public class PromoDetalleRow
    {
        public string CodigoProducto { get; set; } = "";
        public string NombreProducto { get; set; } = "";
        public string TipoRegla { get; set; } = "";
        public decimal DescuentoPct { get; set; }
        public decimal PrecioFijo { get; set; }
        public decimal PackCantidad { get; set; }
        public decimal PackPrecio { get; set; }
    }
}
