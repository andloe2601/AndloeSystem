namespace Andloe.Entidad

/// Carga los pagos asociados al cierre y los muestra en el DataGridView.
{
    public class CierrePagoPosDetalleDto
    {
        public long PagoId { get; set; }
        public DateTime Fecha { get; set; }

        public string? POS_CajaNumero { get; set; }

        public string? MonedaCodigo { get; set; }
        public decimal TasaCambio { get; set; }

        public string? FormaPagoCodigo { get; set; }
        public decimal MontoBase { get; set; }
        public string? MedioPagoNombre { get; set; }

        public decimal Monto { get; set; }

        public string? Referencia { get; set; }
        public string? Entidad { get; set; }
        public string? Observacion { get; set; }

        public string? Usuario { get; set; }
    }
}
