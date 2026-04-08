using System;

namespace Andloe.Entidad
{
    public class TerminoPago
    {
        public int TerminoPagoId { get; set; }

        public string Codigo { get; set; } = "";
        public string Descripcion { get; set; } = "";

        public int DiasPlazo { get; set; }

        public bool TieneDescuento { get; set; }
        public decimal? PorcDescuento { get; set; }
        public int? DiasDescuento { get; set; }

        public byte Estado { get; set; }

        public DateTime? FechaCreacion { get; set; }
        public DateTime? FechaActualizacion { get; set; }

        public string? Usuario { get; set; }

        public int? CantCuotas { get; set; }
        public int? FrecuenciaDias { get; set; }

        public string? UnidadTiempo { get; set; }
        public int? CantidadTiempo { get; set; }

        public string? TextoECF { get; set; }
        public int? TipoPagoECF { get; set; }
    }
}