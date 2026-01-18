using System;
using System.Collections.Generic;

namespace Andloe.Entidad.Facturacion
{
    public class FacturaCabDto
    {
        public int FacturaId { get; set; }
        public string TipoDocumento { get; set; } = "";
        public string? NumeroDocumento { get; set; }
        public DateTime FechaDocumento { get; set; }
        public DateTime? FechaVencimiento { get; set; }

        public string? DireccionCliente { get; set; }

        public int? ClienteId { get; set; }
        public string NombreCliente { get; set; } = "";
        public string? DocumentoCliente { get; set; }

        public decimal SubTotal { get; set; }
        public decimal TotalDescuento { get; set; }
        public decimal TotalImpuesto { get; set; }
        public decimal TotalGeneral { get; set; }

        public string TipoPago { get; set; } = "";
        public int? TerminoPagoId { get; set; }
        public int? DiasCredito { get; set; }

        public string Estado { get; set; } = "";
        public string? Observacion { get; set; }
        public string? ENCF { get; set; }
        public string? TrackId { get; set; }
        public string? CodigoSeguridad { get; set; }
    }

    public class FacturaDetDto
    {
        public int FacturaDetId { get; set; }
        public int FacturaId { get; set; }
        public string? ProductoCodigo { get; set; }
        public string? CodBarra { get; set; }
        public string Descripcion { get; set; } = "";
        public string Unidad { get; set; } = "";
        public decimal Cantidad { get; set; }
        public decimal Precio { get; set; }
        public decimal ItbisPct { get; set; }
        public decimal ItbisMonto { get; set; }
        public decimal TotalLinea { get; set; }
        public int? ImpuestoId { get; set; }

        public decimal DescuentoPct { get; set; }
        public decimal DescuentoMonto { get; set; }
    }

    public class FacturaHistorialDto
    {
        public int FacturaId { get; set; }
        public string TipoDocumento { get; set; } = "";
        public string? NumeroDocumento { get; set; }
        public DateTime FechaDocumento { get; set; }
        public DateTime? FechaVencimiento { get; set; }
        public string Estado { get; set; } = "";
        public string TipoPago { get; set; } = "";
        public string NombreCliente { get; set; } = "";
        public string? DocumentoCliente { get; set; }
        public decimal SubTotal { get; set; }
        public decimal TotalImpuesto { get; set; }
        public decimal TotalGeneral { get; set; }
    }

    public class FacturaCompletaDto
    {
        public FacturaCabDto Cab { get; set; } = new FacturaCabDto();
        public List<FacturaDetDto> Det { get; set; } = new List<FacturaDetDto>();
    }
}
