using System;

namespace Andloe.Entidad.CxC
{
    public sealed class CxCEstadoCuentaDto
    {
        public int ClienteId { get; set; }
        public DateTime Fecha { get; set; }
        public string TipoMovimiento { get; set; } = "";
        public string Documento { get; set; } = "";
        public string? Referencia { get; set; }
        public decimal Debe { get; set; }
        public decimal Haber { get; set; }
        public decimal Monto { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public long OrigenId { get; set; }
        public string? Descripcion { get; set; }

        public decimal BalanceAcumulado { get; set; }
        public int DiasVencidos { get; set; }
    }
}