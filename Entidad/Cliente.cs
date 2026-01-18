namespace Andloe.Entidad
{
    public class Cliente
    {
        public int ClienteId { get; set; }
        public string Codigo { get; set; } = "";
        public string Nombre { get; set; } = "";
        public string? RncCedula { get; set; }
        public string? Telefono { get; set; }
        public string? Email { get; set; }
        public string? Direccion { get; set; }
        public byte Tipo { get; set; } = 0;
        public byte Estado { get; set; } = 1;
        public System.DateTime? FechaCreacion { get; set; }

        // Nuevos
        public decimal? CreditoMaximo { get; set; }
        public string? CodDivisas { get; set; }
        public string? CodTerminoPagos { get; set; }
        public string? CodVendedor { get; set; }
        public string? CodAlmacen { get; set; }

        public decimal? DescuentoPctMax { get; set; }  // ej. 5.00m
    }
}
