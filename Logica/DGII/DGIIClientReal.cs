using System;

namespace Andloe.Logica.DGII
{
    public sealed class DGIIClientReal : IDgiiClient
    {
        public DgiiSendResult Enviar(string xmlFirmado, string encf, int tipoEcf)
        {
            throw new NotImplementedException("DGIIClientReal aún no está implementado.");
        }

        public DgiiStatusResult Consultar(string trackId)
        {
            throw new NotImplementedException("DGIIClientReal aún no está implementado.");
        }
    }
}