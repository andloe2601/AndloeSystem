// Proyecto: Andloe.Entidad
namespace Andloe.Entidad
{
    public class TerminoPago
    {
        public int TerminoPagoId { get; set; }
        public string Codigo { get; set; } = string.Empty;        // NET30, CONTADO, etc.
        public string Descripcion { get; set; } = string.Empty;   // Pago a 30 días
        public int DiasPlazo { get; set; }                        // 0, 15, 30...

        // ✅ NUEVO: cuotas (si DiasPlazo = 0 y CantCuotas > 0)
        public int? CantCuotas { get; set; }                      // 3, 6, 12...
        public int? FrecuenciaDias { get; set; }                  // 7, 15, 30...

        public bool TieneDescuento { get; set; }
        public decimal? PorcDescuento { get; set; }
        public int? DiasDescuento { get; set; }

        public bool Estado { get; set; } = true;

        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaActualizacion { get; set; }
        public string? Usuario { get; set; }
    }
}
