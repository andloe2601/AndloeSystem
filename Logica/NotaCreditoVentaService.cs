using Andloe.Data;
using Andloe.Entidad;
using System;
using System.Collections.Generic;

namespace Andloe.Logica
{
    public sealed class NotaCreditoVentaService
    {
        private readonly NotaCreditoVentaRepository _repo = new();
        private readonly UsuarioContextoRepository _ctxRepo = new();

        public List<VentaOrigenDto> BuscarVentas(string? filtro, int top = 100)
            => _repo.BuscarVentas(filtro, top);

        public NotaCreditoVentaDto? CargarVenta(long ventaId, int usuarioId)
        {
            var dto = _repo.CargarVentaOrigen(ventaId);
            if (dto == null) return null;

            var ctx = _ctxRepo.ObtenerPorUsuarioId(usuarioId);

            if (dto.Cab.EmpresaId <= 0) dto.Cab.EmpresaId = ctx.EmpresaId;
            if (dto.Cab.SucursalId <= 0) dto.Cab.SucursalId = ctx.SucursalId;
            if (dto.Cab.AlmacenId <= 0) dto.Cab.AlmacenId = ctx.AlmacenId;

            return dto;
        }

        public List<NcfTipoDto> ListarTiposENcf()
            => _repo.ListarTiposNcfNotaCredito();

        public string ObtenerProximoNoDocumentoNcPreview()
            => _repo.ObtenerProximoNoDocumentoNcPreview();

        public NotaCreditoVentaDto EmitirDesdeVenta(NotaCreditoVentaDto dto, string usuario, int usuarioId, int? cajaId)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            if (usuarioId <= 0) throw new InvalidOperationException("UsuarioId requerido.");

            var ctx = _ctxRepo.ObtenerPorUsuarioId(usuarioId);

            if (dto.Cab.EmpresaId <= 0) dto.Cab.EmpresaId = ctx.EmpresaId;
            if (dto.Cab.SucursalId <= 0) dto.Cab.SucursalId = ctx.SucursalId;
            if (dto.Cab.AlmacenId <= 0) dto.Cab.AlmacenId = ctx.AlmacenId;

            var ncId = _repo.CrearNotaCredito(dto, usuario);

            var guardada = _repo.Obtener(ncId)
                ?? throw new InvalidOperationException("No se pudo recargar la NC.");

            guardada.Cab.EmpresaId = dto.Cab.EmpresaId;
            guardada.Cab.SucursalId = dto.Cab.SucursalId;
            guardada.Cab.AlmacenId = dto.Cab.AlmacenId;

            return guardada;
        }

        public string GenerarENcf(long ncId, int tipoId, int usuarioId, int? cajaId)
        {
            if (usuarioId <= 0) throw new InvalidOperationException("UsuarioId requerido.");

            var ctx = _ctxRepo.ObtenerPorUsuarioId(usuarioId);

            return _repo.GenerarENcf(
                ncId,
                tipoId,
                ctx.EmpresaId,
                ctx.SucursalId,
                cajaId,
                string.Empty
            );
        }
    }
}