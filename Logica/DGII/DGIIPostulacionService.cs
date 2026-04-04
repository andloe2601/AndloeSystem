using Andloe.Data.DGII;
using Andloe.Entidad.DGII;
using System;
using System.Data;
using System.Globalization;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Xml;

namespace Andloe.Logica.DGII
{
    public sealed class DGIIPostulacionService
    {
        private readonly DGIIPostulacionSqlRepository _repo;

        public DGIIPostulacionService(DGIIPostulacionSqlRepository repo)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
        }

        public string GenerarXml(long postulacionId, string? usuario = null)
        {
            if (postulacionId <= 0)
                throw new ArgumentException("PostulacionId inválido.", nameof(postulacionId));

            var validacion = _repo.ValidarPostulacion(postulacionId);
            if (validacion.Rows.Count > 0)
                throw ConstruirErrorValidacion(validacion);

            return _repo.GenerarXmlPostulacion(postulacionId, usuario);
        }

        public DataTable Validar(long postulacionId)
        {
            if (postulacionId <= 0)
                throw new ArgumentException("PostulacionId inválido.", nameof(postulacionId));

            return _repo.ValidarPostulacion(postulacionId);
        }

        public SignedPostulacionResult FirmarPostulacion(
            long postulacionId,
            string pfxPath,
            string? pfxPassword,
            string? usuario = null)
        {
            if (postulacionId <= 0)
                throw new ArgumentException("PostulacionId inválido.", nameof(postulacionId));

            if (string.IsNullOrWhiteSpace(pfxPath))
                throw new ArgumentException("Debe indicar la ruta del certificado .pfx.", nameof(pfxPath));

            var validacion = _repo.ValidarPostulacion(postulacionId);
            if (validacion.Rows.Count > 0)
                throw ConstruirErrorValidacion(validacion);

            var xml = _repo.ObtenerXmlSinFirmar(postulacionId);

            if (string.IsNullOrWhiteSpace(xml))
                throw new InvalidOperationException("XmlSinFirmar vacío. Debe generar el XML antes de firmar.");

            var fecha = DateTime.Now;
            var xmlConFecha = InsertarFechaHoraFirma(xml, fecha);

            var cert = X509CertLoader.LoadPkcs12FromFile(pfxPath, pfxPassword);
            var signer = new XmlSignerSignedXml(cert);

            var xmlFirmado = signer.FirmarEnveloped(xmlConFecha);

            var result = ConstruirResultado(postulacionId, xmlConFecha, xmlFirmado, fecha, cert);

            _repo.RegistrarXmlFirmado(result, usuario);

            return result;
        }

        private static string InsertarFechaHoraFirma(string xml, DateTime fecha)
        {
            var doc = new XmlDocument { PreserveWhitespace = true };
            doc.LoadXml(xml);

            var existente = doc.DocumentElement?.SelectSingleNode("FechaHoraFirma");
            if (existente != null)
                doc.DocumentElement!.RemoveChild(existente);

            var node = doc.CreateElement("FechaHoraFirma");
            node.InnerText = fecha.ToString("dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture);

            doc.DocumentElement!.AppendChild(node);

            return doc.OuterXml;
        }

        public DataTable ValidarYRegistrar(long postulacionId, string? usuario = null)
        {
            var dt = Validar(postulacionId);

            if (dt.Rows.Count == 0)
            {
                _repo.InsertarLog(
                    postulacionId,
                    "VALIDAR",
                    null,
                    "VALIDADO",
                    "La postulación pasó validación correctamente.",
                    usuario,
                    "DGIIPostulacionService.ValidarYRegistrar"
                );
            }
            else
            {
                var sb = new System.Text.StringBuilder();
                foreach (DataRow row in dt.Rows)
                {
                    if (dt.Columns.Contains("Mensaje"))
                        sb.AppendLine(Convert.ToString(row["Mensaje"]));
                }

                _repo.InsertarLog(
                    postulacionId,
                    "VALIDAR",
                    null,
                    "ERROR",
                    sb.ToString(),
                    usuario,
                    "DGIIPostulacionService.ValidarYRegistrar"
                );
            }

            return dt;
        }

        private static SignedPostulacionResult ConstruirResultado(
            long postulacionId,
            string xmlSinFirmar,
            string xmlFirmado,
            DateTime fecha,
            X509Certificate2 cert)
        {
            var result = new SignedPostulacionResult
            {
                DGIIPostulacionId = postulacionId,
                XmlSinFirmar = xmlSinFirmar,
                XmlFirmado = xmlFirmado,
                FechaHoraFirma = fecha,
                CertThumbprint = cert.Thumbprint,
                CertSerialNumber = cert.SerialNumber,
                CertIssuer = cert.Issuer,
                CertSubject = cert.Subject,
                HashDocumento = CalcularSha256(xmlFirmado)
            };

            var doc = new XmlDocument { PreserveWhitespace = true };
            doc.LoadXml(xmlFirmado);

            var ns = new XmlNamespaceManager(doc.NameTable);
            ns.AddNamespace("ds", "http://www.w3.org/2000/09/xmldsig#");

            result.DigestValue = doc.SelectSingleNode("//ds:DigestValue", ns)?.InnerText;
            result.SignatureValue = doc.SelectSingleNode("//ds:SignatureValue", ns)?.InnerText;

            var canonicalizationMethod = doc.SelectSingleNode("//ds:SignedInfo/ds:CanonicalizationMethod", ns);
            if (canonicalizationMethod?.Attributes?["Algorithm"] != null)
                result.CanonicalizationMethod = canonicalizationMethod.Attributes["Algorithm"]!.Value;

            var signatureMethod = doc.SelectSingleNode("//ds:SignedInfo/ds:SignatureMethod", ns);
            if (signatureMethod?.Attributes?["Algorithm"] != null)
                result.SignatureMethod = signatureMethod.Attributes["Algorithm"]!.Value;

            return result;
        }

        private static string CalcularSha256(string content)
        {
            using var sha = SHA256.Create();
            var bytes = System.Text.Encoding.UTF8.GetBytes(content ?? "");
            var hash = sha.ComputeHash(bytes);
            return Convert.ToHexString(hash);
        }

        private static Exception ConstruirErrorValidacion(DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0)
                return new InvalidOperationException("La postulación no pasó validación.");

            var mensajes = new System.Text.StringBuilder();
            mensajes.AppendLine("La postulación no pasó validación:");

            foreach (DataRow row in dt.Rows)
            {
                if (dt.Columns.Contains("Mensaje"))
                    mensajes.AppendLine("- " + Convert.ToString(row["Mensaje"]));
                else
                    mensajes.AppendLine("- Error de validación detectado.");
            }

            return new InvalidOperationException(mensajes.ToString());
        }
    }
}