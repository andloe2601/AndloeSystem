using System;

namespace Andloe.Logica.DGII
{
    public sealed class DGIIClientMock : IDgiiClient
    {
        private static readonly Random _rnd = new Random();

        public DgiiSendResult Enviar(string xmlFirmado, string encf, int tipoEcf)
        {
            // Simula TrackId
            var track = $"MOCK-{DateTime.UtcNow:yyyyMMddHHmmss}-{_rnd.Next(1000, 9999)}";
            return new DgiiSendResult
            {
                TrackId = track,
                RawResponse = $"{{\"ok\":true,\"trackId\":\"{track}\",\"encf\":\"{encf}\",\"tipo\":{tipoEcf}}}"
            };
        }

        public DgiiStatusResult Consultar(string trackId)
        {
            // Simula estados
            var p = _rnd.Next(0, 100);
            var estado = p < 70 ? "ACEPTADO" : (p < 90 ? "RECHAZADO" : "EN_PROCESO");

            return new DgiiStatusResult
            {
                Estado = estado,
                RawResponse = $"{{\"trackId\":\"{trackId}\",\"estado\":\"{estado}\",\"ts\":\"{DateTime.UtcNow:o}\"}}"
            };
        }
    }
}