using Andloe.Data.DGII;
using Andloe.Entidad;
using System;

namespace Andloe.Logica
{
    public sealed class ECFEmisionService
    {
        private readonly ECFDocumentoRepository _ecfRepo = new();

        public ECFDocumento? EmitirDesdeFactura(int facturaId, string usuario)
        {
            if (facturaId <= 0)
                throw new InvalidOperationException("FacturaId inválido.");

            if (string.IsNullOrWhiteSpace(usuario))
                throw new InvalidOperationException("Usuario inválido.");

            // 1. Leer factura
            // 2. Generar XML
            // 3. UpsertPendiente(...)
            // 4. Firmar
            // 5. GuardarXmlFirmadoPorFactura(...)
            // 6. Enviar Alanube
            // 7. MarcarEnviado / GuardarRespuestaAlanubePorFactura(...)
            // 8. Retornar ECFDocumento actualizado

            return _ecfRepo.ObtenerDocumentoPorFactura(facturaId);
        }
    }
}