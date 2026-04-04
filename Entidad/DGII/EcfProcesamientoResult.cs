namespace Entidad
{
    public sealed class EcfProcesamientoResult
    {
        public int FacturaId { get; set; }
        public string? ENCF { get; set; }
        public string? EstadoDGII { get; set; }
        public bool XmlGenerado { get; set; }
        public bool XmlFirmado { get; set; }
        public string? Mensaje { get; set; }
    }
}