public sealed class PosFiscalResult
{
    public int FacturaId { get; set; }
    public string NumeroDocumento { get; set; } = "";
    public string? ENCF { get; set; }
    public string? TrackId { get; set; }
    public string? EstadoECF { get; set; }
    public bool EcfDocumentoCreado { get; set; }
}