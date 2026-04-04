using System;
using System.Collections.Generic;
using System.Linq;
using Andloe.Data;
using Andloe.Entidad.CxC;

namespace Andloe.Logica
{
    public sealed class CxCCobroService
    {
        private readonly CxCCobroRepository _repo = new();

        public List<CxCCobroListadoDto> Listar(string? filtro = null, int top = 200)
            => _repo.Listar(filtro, top);

        public List<CxCClienteLookupDto> BuscarClientes(string? filtro, int top = 20)
            => _repo.BuscarClientes(filtro, top);

        public List<CxCCuentaDestinoDto> ListarCuentasDestino()
            => _repo.ListarCuentasDestino();

        public List<CxCCentroCostoDto> ListarCentrosCosto()
        {
            var sesion = SesionService.Current;
            return _repo.ListarCentrosCosto(sesion.EmpresaId);
        }

        public List<CxCFacturaPendienteDto> ListarFacturasPendientes(int clienteId)
        {
            if (clienteId <= 0)
                return new List<CxCFacturaPendienteDto>();

            return _repo.ListarFacturasPendientes(clienteId);
        }

        public void AutoAplicar(List<CxCFacturaPendienteDto> facturas, decimal montoDisponible)
        {
            if (facturas == null || facturas.Count == 0)
                return;

            if (montoDisponible < 0)
                montoDisponible = 0;

            foreach (var f in facturas)
            {
                f.MontoRecibido = 0;
                f.Retencion = 0;
            }

            foreach (var item in facturas
                .OrderBy(x => x.FechaVencimiento ?? x.FechaDocumento)
                .ThenBy(x => x.FacturaId))
            {
                if (montoDisponible <= 0)
                    break;

                var balance = Math.Round(item.BalancePendiente, 2);
                if (balance <= 0)
                    continue;

                var aplicar = Math.Min(balance, montoDisponible);
                aplicar = Math.Round(aplicar, 2);

                item.MontoRecibido = aplicar;
                montoDisponible = Math.Round(montoDisponible - aplicar, 2);
            }
        }

        public CxCCobroCrearResultDto Crear(CxCCobroCrearDto dto)
        {
            Validar(dto);

            var sesion = SesionService.Current;
            dto.EmpresaId = sesion.EmpresaId;
            dto.SucursalId = sesion.SucursalId;

            return _repo.Crear(dto, sesion.Usuario);
        }

        private static void Validar(CxCCobroCrearDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            if (dto.ClienteId.GetValueOrDefault() <= 0)
                throw new InvalidOperationException("Debes seleccionar un cliente.");

            if (string.IsNullOrWhiteSpace(dto.ClienteNombre))
                throw new InvalidOperationException("El nombre del cliente es obligatorio.");

            if (string.IsNullOrWhiteSpace(dto.MonedaCodigo))
                throw new InvalidOperationException("La moneda es obligatoria.");

            if (string.IsNullOrWhiteSpace(dto.TipoMedio))
                throw new InvalidOperationException("La forma de pago es obligatoria.");

            if (dto.MontoMedio <= 0)
                throw new InvalidOperationException("El monto del pago debe ser mayor que cero.");

            if (dto.Aplicaciones == null || dto.Aplicaciones.Count == 0)
                throw new InvalidOperationException("Debes aplicar el pago a por lo menos una factura.");

            dto.Aplicaciones = dto.Aplicaciones
                .Where(x => x != null && x.FacturaId > 0 && Math.Round(x.MontoAplicado, 2) > 0)
                .Select(x =>
                {
                    x.MontoAplicado = Math.Round(x.MontoAplicado, 2);
                    x.Retencion = Math.Round(x.Retencion, 2);
                    return x;
                })
                .ToList();

            if (dto.Aplicaciones.Count == 0)
                throw new InvalidOperationException("No hay facturas válidas con monto aplicado.");

            var totalAplicado = Math.Round(dto.Aplicaciones.Sum(x => x.MontoAplicado), 2);
            var montoMedio = Math.Round(dto.MontoMedio, 2);

            if (totalAplicado <= 0)
                throw new InvalidOperationException("El total aplicado debe ser mayor que cero.");

            if (totalAplicado != montoMedio)
                throw new InvalidOperationException("El total aplicado debe coincidir con el monto del pago.");

            dto.TipoMedio = dto.TipoMedio.Trim().ToUpperInvariant();
            dto.MontoMedio = montoMedio;
            dto.TasaCambio = dto.TasaCambio <= 0 ? 1m : dto.TasaCambio;
        }
    }
}