using System;

namespace Entidad
{
    public sealed class EcfFirmaResult
    {
        public int FacturaId { get; set; }
        public string XmlFirmado { get; set; } = string.Empty;
        public DateTime FechaHoraFirma { get; set; }

        public string? DigestValue { get; set; }
        public string? SignatureValue { get; set; }
        public string? CanonicalizationMethod { get; set; }
        public string? SignatureMethod { get; set; }

        public string? CertThumbprint { get; set; }
        public string? CertSerialNumber { get; set; }
        public string? CertIssuer { get; set; }
        public string? CertSubject { get; set; }

        public string? HashDocumento { get; set; }
        public string? Usuario { get; set; }
    }
}