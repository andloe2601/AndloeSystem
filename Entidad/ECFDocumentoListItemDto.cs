using System;

namespace Andloe.Entidad
{
    public sealed class ECFDocumentoListItemDto
    {
        public long ECFDocumentoId { get; set; }
        public int FacturaId { get; set; }
        public int TipoECF { get; set; }
        public string ENCF { get; set; } = "";
        public string EstadoDGII { get; set; } = "";
        public string? TrackId { get; set; }
        public DateTime FechaGenerado { get; set; }
        public int IntentosEnvio { get; set; }
        public string? UltimoError { get; set; }

        public string? NumeroDocumento { get; set; }
        public string? Cliente { get; set; }
        public decimal? TotalGeneral { get; set; }
    }
}