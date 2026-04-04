using Andloe.Data.DGII;
using Data;
using Entidad;
using Logica.DGII;
using System;
using System.Security.Cryptography.X509Certificates;

namespace Logica
{
    public sealed class ECFService
    {
        private readonly SnapshotFiscalRepository _snapshotRepository;
        private readonly ECFSqlRepository _ecfSqlRepository;
        private readonly ECFFirmaService _firmaService;
        private readonly ECFDocumentoRepository _ecfDocumentoRepository;

        public ECFService()
        {
            _snapshotRepository = new SnapshotFiscalRepository();
            _ecfSqlRepository = new ECFSqlRepository();
            _firmaService = new ECFFirmaService();
            _ecfDocumentoRepository = new ECFDocumentoRepository();
        }

        public string GenerarXml(int facturaId, string usuario)
        {
            _snapshotRepository.SincronizarCompradorFactura(facturaId);
            return _ecfSqlRepository.GenerarXmlFacturaV3(facturaId, usuario);
        }

        public EcfFirmaResult FirmarDocumento(int facturaId, X509Certificate2 cert, string usuario)
        {
            var xmlSinFirmar = _ecfSqlRepository.ObtenerXmlSinFirmar(facturaId);

            if (string.IsNullOrWhiteSpace(xmlSinFirmar))
                throw new InvalidOperationException("No existe XmlSinFirmar para la factura indicada.");

            var result = _firmaService.FirmarXml(facturaId, xmlSinFirmar, cert, usuario);
            _ecfSqlRepository.RegistrarXmlFirmado(result);

            return result;
        }

        public EcfProcesamientoResult ProcesarFacturaFiscalCompleta(
    int facturaId,
    int empresaId,
    int sucursalId,
    int cajaId,
    int tipoEcf,
    string prefijo,
    X509Certificate2 cert,
    string usuario)
        {
            if (facturaId <= 0)
                throw new ArgumentException("FacturaId inválido.");

            // 1. Generar eNCF
            string encf = _ecfSqlRepository.GenerarENcf(
                empresaId,
                sucursalId,
                cajaId,
                facturaId,
                tipoEcf,
                prefijo);

            if (string.IsNullOrWhiteSpace(encf))
                throw new Exception("No se pudo generar el eNCF.");

            // 2. Generar XML (con snapshot)
            var xml = GenerarXml(facturaId, usuario);

            if (string.IsNullOrWhiteSpace(xml))
                throw new Exception("No se pudo generar el XML.");

            // 3. Firmar XML
            var firma = FirmarDocumento(facturaId, cert, usuario);

            if (string.IsNullOrWhiteSpace(firma.XmlFirmado))
                throw new Exception("Error al firmar XML.");

            // 4. Obtener documento final
            var doc = _ecfSqlRepository.ObtenerDocumentoPorFactura(facturaId);

            return new EcfProcesamientoResult
            {
                FacturaId = facturaId,
                ENCF = encf,
                EstadoDGII = doc?.EstadoDGII,
                XmlGenerado = true,
                XmlFirmado = true,
                Mensaje = "Factura e-CF generada y firmada correctamente."
            };
        }

        public void GenerarXmlPendiente(int facturaId, int tipoEcf, string encf)
        {
            if (facturaId <= 0)
                throw new ArgumentException("FacturaId inválido.", nameof(facturaId));

            var usuario = Environment.UserName;
            var xml = GenerarXml(facturaId, usuario);

            if (string.IsNullOrWhiteSpace(xml))
                throw new InvalidOperationException("No se pudo generar el XML pendiente.");
        }

        public void FirmarFactura(int facturaId, string pfxPath, string? password)
        {
            if (facturaId <= 0)
                throw new ArgumentException("FacturaId inválido.", nameof(facturaId));

            if (string.IsNullOrWhiteSpace(pfxPath))
                throw new ArgumentException("Debes indicar el certificado PFX.", nameof(pfxPath));

#pragma warning disable SYSLIB0057
            using var cert = string.IsNullOrWhiteSpace(password)
                ? new X509Certificate2(pfxPath)
                : new X509Certificate2(pfxPath, password,
    X509KeyStorageFlags.Exportable | X509KeyStorageFlags.UserKeySet | X509KeyStorageFlags.PersistKeySet);
#pragma warning restore SYSLIB0057

            var usuario = Environment.UserName;
            var result = FirmarDocumento(facturaId, cert, usuario);

            if (result == null || string.IsNullOrWhiteSpace(result.XmlFirmado))
                throw new InvalidOperationException("No se pudo firmar el XML.");

            _ecfDocumentoRepository.MarcarFirmadoPorFactura(facturaId);
        }

        public void EnviarFactura(long ecfDocumentoId, int facturaId, int tipoEcf, string encf)
        {
            if (facturaId <= 0)
                throw new ArgumentException("FacturaId inválido.", nameof(facturaId));

            var doc = _ecfSqlRepository.ObtenerDocumentoPorFactura(facturaId);
            if (doc == null || string.IsNullOrWhiteSpace(doc.XmlFirmado))
                throw new InvalidOperationException("La factura no tiene XML firmado.");

            var trackSimulado = $"LOCAL-{facturaId}-{DateTime.Now:yyyyMMddHHmmss}";

            _ecfDocumentoRepository.ActualizarTrackingYRespuesta(
                facturaId,
                trackSimulado,
                "ENVIADO",
                "Documento marcado como enviado desde monitor local.");

            _ecfDocumentoRepository.MarcarEnviadoPorFactura(facturaId, trackSimulado);
        }

        public void Consultar(long ecfDocumentoId, int facturaId, string trackId)
        {
            if (facturaId <= 0)
                throw new ArgumentException("FacturaId inválido.", nameof(facturaId));

            if (string.IsNullOrWhiteSpace(trackId))
                throw new InvalidOperationException("No hay TrackId para consultar.");

            _ecfDocumentoRepository.ActualizarEstadoPorFactura(
                facturaId,
                "ENVIADO",
                $"Consulta local OK. TrackId: {trackId}");
        }
    }
}
