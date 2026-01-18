namespace Andloe.Entidad
{
    public class FondoCaja
    {
        public long FondoId { get; set; }
        public int CajaId { get; set; }
        public string POS_CajaNumero { get; set; } = string.Empty;

        public DateTime FechaApertura { get; set; }
        public string UsuarioApertura { get; set; } = string.Empty;

        public decimal MontoFondo { get; set; }
        public string? Observacion { get; set; }

        public string Estado { get; set; } = "ABIERTO"; // ABIERTO / CERRADO

        public long? CierreId { get; set; }
        public DateTime? FechaCierre { get; set; }
    }
}
