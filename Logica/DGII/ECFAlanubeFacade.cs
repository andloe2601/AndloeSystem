using Andloe.Entidad;

namespace Andloe.Logica.DGII
{
    public sealed class ECFAlanubeFacade
    {
        private readonly ECFAlanubeService _alanubeService;

        public ECFAlanubeFacade()
        {
            _alanubeService = new ECFAlanubeService();
        }

        public AlanubeEmitResponseDto EnviarFactura(int facturaId)
        {
            return _alanubeService.EnviarFactura(facturaId);
        }

        public AlanubeStatusResponseDto ConsultarFactura(int facturaId)
        {
            return _alanubeService.ConsultarFactura(facturaId);
        }

        public string GenerarPayloadJson(int facturaId)
        {
            return _alanubeService.GenerarPayloadJson(facturaId);
        }
    }
}