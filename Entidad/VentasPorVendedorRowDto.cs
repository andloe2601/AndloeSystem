using System;

namespace Andloe.Entidad
{
    public sealed class VentasPorVendedorRowDto
    {
        public string CodVendedor { get; set; } = "";
        public string Vendedor { get; set; } = "";
        public int CantFacturas { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Descuento { get; set; }
        public decimal Itbis { get; set; }
        public decimal TotalGeneral { get; set; }
    }

    public sealed class VentasPorVendedorDetalleDto
    {
        public int FacturaId { get; set; }
        public string NumeroDocumento { get; set; } = "";
        public DateTime FechaDocumento { get; set; }
        public string CodVendedor { get; set; } = "";
        public string Vendedor { get; set; } = "";
        public string Cliente { get; set; } = "";
        public decimal TotalGeneral { get; set; }
        public string Estado { get; set; } = "";
    }
}