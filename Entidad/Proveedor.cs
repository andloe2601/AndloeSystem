namespace Andloe.Entidad
{
    public class Proveedor
    {
        public string Codigo { get; set; } = "";
        public string Nombre { get; set; } = "";
        public string? Rnc { get; set; }
        public string? Telefono { get; set; }
        public string? Email { get; set; }
        public string? Direccion { get; set; }
        public byte Estado { get; set; } = 1;
        public System.DateTime? FechaCreacion { get; set; }

        // Nuevos
        public decimal? CreditoMaximo { get; set; }
        public string? CodDivisas { get; set; }
        public string? CodTerminoPagos { get; set; }
        public string? CodVendedor { get; set; }
        public string? CodAlmacen { get; set; }
    }
}
