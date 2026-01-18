#nullable enable
using System;
using Andloe.Data;
using Andloe.Entidad;
using Microsoft.Extensions.Logging;
using SesionSvc = Andloe.Logica.SesionService;

namespace Andloe.Logica
{
    public sealed class ReporteService
    {
        private readonly ReporteRepository _repo;
        private readonly ILogger<ReporteService> _logger;

        public ReporteService(ReporteRepository repo, ILogger<ReporteService> logger)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // ✅ recomendado: modulo + actividad + codigo
        public ReporteDefDto ObtenerReporteParaImprimir(string modulo, string actividad, string codigo)
        {
            var s = SesionSvc.Current;

            var rep = _repo.ResolverReporte(
                modulo: modulo,
                actividad: actividad,
                codigo: codigo,
                empresaId: s.EmpresaId,
                sucursalId: s.SucursalId,
                usuarioId: s.UsuarioId
            );

            if (rep == null)
                throw new InvalidOperationException($"No hay reporte activo configurado para {modulo}/{actividad}/{codigo}");

            if (string.IsNullOrWhiteSpace(rep.Motor))
                throw new InvalidOperationException("El reporte no tiene Motor definido.");

            if (string.IsNullOrWhiteSpace(rep.RutaArchivo))
                throw new InvalidOperationException("El reporte no tiene RutaArchivo definido.");

            _logger.LogInformation("Reporte obtenido: {Identificador}, Motor: {Motor}", codigo, rep.Motor);

            return rep;
        }

        // ✅ atajo: modulo + codigo (si no quieres actividad)
        public ReporteDefDto ObtenerReporteParaImprimir(string modulo, string codigo)
        {
            var s = SesionSvc.Current;

            var rep = _repo.ResolverReportePorCodigo(
                modulo: modulo,
                codigo: codigo,
                empresaId: s.EmpresaId,
                sucursalId: s.SucursalId,
                usuarioId: s.UsuarioId
            );

            if (rep == null)
                throw new InvalidOperationException($"No hay reporte activo configurado para {modulo}/{codigo}");

            if (string.IsNullOrWhiteSpace(rep.Motor))
                throw new InvalidOperationException("El reporte no tiene Motor definido.");

            if (string.IsNullOrWhiteSpace(rep.RutaArchivo))
                throw new InvalidOperationException("El reporte no tiene RutaArchivo definido.");

            _logger.LogInformation("Reporte obtenido: {Identificador}, Motor: {Motor}", codigo, rep.Motor);

            return rep;
        }
    }
}
#nullable restore
