using System;

namespace Andloe.Entidad.DGII
{
    public sealed class SignedPostulacionResult
    {
        public long DGIIPostulacionId { get; set; }

        public string XmlSinFirmar { get; set; } = "";
        public string XmlFirmado { get; set; } = "";

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
    }
}