using System;
using System.Collections.Generic;
using System.Linq;
using Andloe.Data;
using Andloe.Entidad.CxC;

namespace Andloe.Logica
{
    public sealed class CxCEstadoCuentaService
    {
        private readonly CxCEstadoCuentaRepository _repo = new();
        private readonly CxCCobroService _clienteHelper = new();

        public List<CxCClienteLookupDto> BuscarClientes(string? filtro, int top = 20)
            => _clienteHelper.BuscarClientes(filtro, top);

        public List<CxCEstadoCuentaDto> ListarEstadoCuentaCliente(int clienteId, DateTime? desde = null, DateTime? hasta = null)
        {
            if (clienteId <= 0)
                return new List<CxCEstadoCuentaDto>();

            return _repo.ListarEstadoCuentaCliente(clienteId, desde, hasta);
        }

        public (decimal totalFacturado, decimal totalCobrado, decimal balance) CalcularTotales(List<CxCEstadoCuentaDto> items)
        {
            items ??= new List<CxCEstadoCuentaDto>();

            decimal totalFacturado = items.Sum(x => x.Debe);
            decimal totalCobrado = items.Sum(x => x.Haber);
            decimal balance = totalFacturado - totalCobrado;

            return (totalFacturado, totalCobrado, balance);
        }
    }
}