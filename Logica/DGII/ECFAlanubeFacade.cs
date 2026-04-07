using Andloe.Data.Fiscal;

namespace Andloe.Logica.DGII
{
    public sealed class ECFAlanubeFacade
    {
        private readonly ECFAlanubeService _service = new();

        public AlanubeEmitResponseDto EnviarFactura(int facturaId, string? usuario = null)
        {
            return _service.EnviarFactura(facturaId, usuario);
        }

        public AlanubeStatusResponseDto ConsultarFactura(int facturaId)
        {
            return _service.ConsultarFactura(facturaId);
        }
    }
}