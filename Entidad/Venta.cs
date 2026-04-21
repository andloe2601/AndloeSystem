namespace Andloe.Entidad
{
    public class Venta
    {
        public long VentaId { get; set; }
        public string NoDocumento { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
        public int? CajaId { get; set; }

        public string? ClienteCodigo { get; set; }
        public int? ClienteId { get; set; }

        public string? NombreCliente { get; set; }
        public string? EmailCliente { get; set; }
        public string? TelefonoCliente { get; set; }

        public string MonedaCodigo { get; set; } = "DOP";
        public decimal TasaCambio { get; set; }

        public decimal Subtotal { get; set; }
        public decimal DescuentoTotal { get; set; }
        public decimal ImpuestoTotal { get; set; }
        public decimal Total { get; set; }

        public string? Estado { get; set; }
        public string? Usuario { get; set; }
        public string? Observacion { get; set; }

        public DateTime FechaCreacion { get; set; }

        //public int? MedioPagoId { get; set; }
        public decimal MontoPago { get; set; }
        public decimal MontoCambio { get; set; }

        public decimal SubTotalMoneda { get; set; }
        public decimal ItbisMoneda { get; set; }
        public decimal TotalMoneda { get; set; }

        // 🔹 Campo POS para la caja
        public string? POS_CajaNumero { get; set; }

        public int TerminoPagoId { get; set; } = 1;

        public bool IncluidaEnCierre { get; set; }
    }
}
