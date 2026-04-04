using System;

namespace Andloe.Logica.DGII
{
    public interface IDgiiClient
    {
        // Enviar xml firmado (o sin firmar si estás mockeando)
        DgiiSendResult Enviar(string xmlFirmado, string encf, int tipoEcf);

        // Consultar estado por TrackId
        DgiiStatusResult Consultar(string trackId);
    }

    public sealed class DgiiSendResult
    {
        public string TrackId { get; set; } = "";
        public string RawResponse { get; set; } = "";
    }

    public sealed class DgiiStatusResult
    {
        public string Estado { get; set; } = "";       // ACEPTADO / RECHAZADO / EN_PROCESO / ERROR ...
        public string RawResponse { get; set; } = "";  // JSON/XML/texto
    }
}