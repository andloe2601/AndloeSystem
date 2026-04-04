namespace Andloe.Entidad
{
    public sealed class ECFDocumentoDto
    {
        public long ECFDocumentoId { get; set; }
        public long FacturaId { get; set; }
        public int TipoECF { get; set; }
        public string ENCF { get; set; } = "";
        public string EstadoDGII { get; set; } = "PENDIENTE";
        public string? TrackId { get; set; }

        public string? XmlSinFirmar { get; set; }
        public string? XmlFirmado { get; set; }

        public string? RespuestaDGII { get; set; }

        public System.DateTime FechaGenerado { get; set; }
        public System.DateTime? FechaFirmado { get; set; }
        public System.DateTime? FechaEnviado { get; set; }

        public int IntentosEnvio { get; set; }
        public string? UltimoError { get; set; }
    }
}