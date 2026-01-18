// Lista para mostrar productos con precio de venta e ITBIS
namespace Andloe.Entidad
{
    public class ProductoListaDto
    {
        public string Codigo { get; set; } = "";
        public string Descripcion { get; set; } = "";
        public decimal PrecioVenta { get; set; }
        public decimal ItbisPct { get; set; }
    }
}
