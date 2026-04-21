public class ECFDocumento
{
    public long ECFDocumentoId { get; set; }
    public int FacturaId { get; set; }
    public string? ENCF { get; set; }
    public string? TrackId { get; set; }
    public string? XmlRespuesta { get; set; }
    public string? XmlEnviado { get; set; }
    public string? EstadoDGII { get; set; }
    public int TipoECF { get; set; }
    public string? XmlSinFirmar { get; set; }
    public string? XmlFirmado { get; set; }
    public DateTime FechaGenerado { get; set; }
    public int IntentosEnvio { get; set; }
    public string? UltimoError { get; set; }
    public decimal? MontoTotalDocumento { get; set; }
    public string? EstadoProceso { get; set; }
    public bool Activo { get; set; }
}