using System;

namespace Entidad
{
    public sealed class EcfDocumentoDto
    {
        public long ECFDocumentoId { get; set; }
        public int FacturaId { get; set; }

        // OJO: tu repo viejo lo está leyendo como string,
        // aunque en SQL la columna es int. Lo dejamos así por compatibilidad inmediata.
        public string? TipoECF { get; set; }

        public string? ENCF { get; set; }
        public string? EstadoDGII { get; set; }
        public string? EstadoProceso { get; set; }

        public string? XmlSinFirmar { get; set; }
        public string? XmlFirmado { get; set; }
        public string? XmlEnviado { get; set; }
        public string? XmlRespuesta { get; set; }
        public string? RespuestaDGII { get; set; }
        public string? RespuestaDGIITexto { get; set; }

        public DateTime? FechaGenerado { get; set; }
        public DateTime? FechaFirmado { get; set; }
        public DateTime? FechaEnviado { get; set; }
        public DateTime? FechaHoraFirma { get; set; }
        public DateTime? FechaAceptado { get; set; }
        public DateTime? FechaRechazado { get; set; }
        public DateTime? FechaUltimaConsulta { get; set; }

        public string? DigestValue { get; set; }
        public string? SignatureValue { get; set; }
        public string? CanonicalizationMethod { get; set; }
        public string? SignatureMethod { get; set; }

        public string? CertThumbprint { get; set; }
        public string? CertSerialNumber { get; set; }
        public string? CertIssuer { get; set; }
        public string? CertSubject { get; set; }

        public string? HashDocumento { get; set; }
        public string? TrackId { get; set; }
        public string? CodigoSeguridad { get; set; }
        public string? CodigoRespuestaDGII { get; set; }
        public string? UltimoError { get; set; }
        public decimal? MontoTotalDocumento { get; set; }
        public int IntentosEnvio { get; set; }
        public int IntentosConsulta { get; set; }
        public string? OrigenEmision { get; set; }
        public bool Activo { get; set; }
    }
}