namespace Andloe.Entidad
{
    public class KardexLineaDto
    {
        public DateTime Fecha { get; set; }
        public string Tipo { get; set; } = "";
        public string? Origen { get; set; }
        public long? OrigenId { get; set; }
        public string ProductoCodigo { get; set; } = "";
        public decimal CantidadEntrada { get; set; }
        public decimal CantidadSalida { get; set; }
        public decimal Saldo { get; set; }        // saldo acumulado calculado
        public string Usuario { get; set; } = "";
        public string? Observacion { get; set; }
    }
}
