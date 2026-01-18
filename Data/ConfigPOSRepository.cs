using Andloe.Entidad;

namespace Andloe.Data
{
    public class ConfigPOSRepository
    {
        private readonly SistemaConfigRepository _repo = new();

        private const string ClaveMoneda = "POS.MonedaBase";
        private const string ClaveCliente = "POS.ClienteDefectoCodigo";
        private const string ClaveMedio = "POS.MedioPagoDefectoId";

        public ConfigPOS Obtener()
        {
            var cfg = new ConfigPOS();

            var moneda = _repo.GetValor(ClaveMoneda);
            if (!string.IsNullOrWhiteSpace(moneda))
                cfg.MonedaCodigo = moneda;

            var cli = _repo.GetValor(ClaveCliente);
            if (!string.IsNullOrWhiteSpace(cli))
                cfg.ClienteDefectoCodigo = cli;

            var medio = _repo.GetValor(ClaveMedio);
            if (int.TryParse(medio, out var medioId))
                cfg.MedioPagoDefectoId = medioId;

            return cfg;
        }

        public void Guardar(ConfigPOS cfg, string usuario)
        {
            _repo.SetValor(ClaveMoneda, cfg.MonedaCodigo, "Moneda base POS", "string", usuario);
            _repo.SetValor(ClaveCliente, cfg.ClienteDefectoCodigo, "Cliente por defecto POS", "string", usuario);
            _repo.SetValor(ClaveMedio, cfg.MedioPagoDefectoId?.ToString(), "Medio pago def.", "int?", usuario);
        }
    }
}
