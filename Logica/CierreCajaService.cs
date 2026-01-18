using Andloe.Data;
using Andloe.Entidad;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Andloe.Logica
{
    public class CierreCajaService
    {
        private readonly CierreCajaRepository _repo = new();
        private readonly FondoCajaRepository _fondoRepo = new();
        private readonly AuditoriaService _audit = new();

        public List<CierrePagoPosDetalleDto> ListarPagosPosPorCierre(long cierreId)
            => _repo.ListarPagosPosPorCierre(cierreId);

        public CierreCajaResumen CalcularResumen(string cajaNumero, DateTime desde, DateTime hasta)
        {
            return _repo.CalcularResumen(cajaNumero, desde, hasta);
        }

        public long GuardarCierre(
            int cajaId,
            string cajaNumero,
            DateTime desde,
            DateTime hasta,
            decimal fondoInicial,
            decimal efectivoDeclarado,
            string usuarioCierre)
        {
            if (string.IsNullOrWhiteSpace(cajaNumero))
                throw new ArgumentException("El número de caja es obligatorio.", nameof(cajaNumero));

            // Recalcular resumen para ese rango y caja
            var resumen = _repo.CalcularResumen(cajaNumero, desde, hasta);
            var ahora = DateTime.Now;
            var efectivoTeoricoTotal = resumen.EfectivoTeorico + fondoInicial;
            var diferencia = efectivoDeclarado - efectivoTeoricoTotal;

            var cierre = new CierreCaja
            {
                CajaId = cajaId,
                POS_CajaNumero = cajaNumero,
                FechaDesde = desde,
                FechaHasta = hasta,
                FondoInicial = fondoInicial,
                TotalVentas = resumen.TotalVentas,
                TotalPagos = resumen.TotalPagos,
                EfectivoTeorico = efectivoTeoricoTotal,
                EfectivoDeclarado = efectivoDeclarado,
                Diferencia = diferencia,
                UsuarioCierre = usuarioCierre,
                FechaCierre = ahora,
                Estado = "CERRADO"
            };

            // 1) Graba cabecera de cierre
            var cierreId = _repo.InsertarCierre(cierre);

            // 1.1) Actualizar POS_Pago con el cierre (CierreId + IncluidoEnCierre = 1)
            _repo.ActualizarPagosPosConCierreId(
                cierreId,
                cajaNumero,
                desde,
                hasta
            );

            // 1.2) Actualizar VentaCab con el cierre (CierreId + IncluidaEnCierre = 1)
            _repo.ActualizarVentasConCierreId(
                cierreId,
                cajaNumero,
                desde,
                hasta
            );

            // 2) Marca el fondo de esa caja como CERRADO
            _fondoRepo.CerrarFondosAbiertos(cajaId);

            // ✅ Auditoría
            _audit.Log(
                modulo: "CAJA",
                accion: "CIERRE",
                entidad: "CajaCierreCab",
                entidadId: cierreId.ToString(),
                detalle: $"CajaId={cajaId} CajaNumero={cajaNumero} Desde={desde:yyyy-MM-dd HH:mm} Hasta={hasta:yyyy-MM-dd HH:mm} Fondo={fondoInicial:0.00} Declarado={efectivoDeclarado:0.00} Teorico={efectivoTeoricoTotal:0.00} Dif={diferencia:0.00}"
            );

            return cierreId;
        }

        // Histórico de cierres
        public List<CierreCaja> BuscarCierres(
            DateTime? desde,
            DateTime? hasta,
            string? cajaNumero,
            string? usuarioCierre,
            string? estado)
        {
            return _repo.ListarCierres(desde, hasta, cajaNumero, usuarioCierre, estado);
        }

        public DataTable ObtenerVentasPorCierre(long cierreId)
        {
            return _repo.ListarVentasPorCierre(cierreId);
        }

        public DataTable ObtenerPagosPorCierre(long cierreId)
        {
            // Buscamos el cierre para obtener caja y rango de fechas
            var cierre = BuscarCierres(null, null, null, null, null)
                .FirstOrDefault(c => c.CierreId == cierreId);

            if (cierre == null)
                return new DataTable();

            return _repo.ListarPagosPorCajaYRango(
                cierre.POS_CajaNumero,
                cierre.FechaDesde,
                cierre.FechaHasta
            );
        }

        public DataTable ObtenerFondoPorCierre(long cierreId)
        {
            var cierre = BuscarCierres(null, null, null, null, null)
                .FirstOrDefault(c => c.CierreId == cierreId);

            if (cierre == null)
                return new DataTable();

            return _repo.ListarFondoPorCajaYRango(
                cierre.POS_CajaNumero,
                cierre.FechaDesde,
                cierre.FechaHasta
            );
        }

        public long GuardarCierre(CierreCajaDto dto)
        {
            // 1. Guardas el encabezado de cierre y obtienes el ID
            long cierreId = _repo.InsertarCierre(dto);

            // 2. Actualizas POS_Pago con ese cierre (CierreId + IncluidoEnCierre)
            _repo.ActualizarPagosPosConCierreId(
                cierreId,
                dto.CajaNumero,
                dto.FechaDesde,
                dto.FechaHasta
            );

            // 2.1) Actualizar VentaCab con ese cierre
            _repo.ActualizarVentasConCierreId(
                cierreId,
                dto.CajaNumero,
                dto.FechaDesde,
                dto.FechaHasta
            );

            // ✅ Auditoría
            _audit.Log(
                modulo: "CAJA",
                accion: "CIERRE",
                entidad: "CajaCierreCab",
                entidadId: cierreId.ToString(),
                detalle: $"Cierre (DTO). CajaNumero={dto.CajaNumero} Desde={dto.FechaDesde:yyyy-MM-dd HH:mm} Hasta={dto.FechaHasta:yyyy-MM-dd HH:mm}"
            );

            return cierreId;
        }
    }
}
