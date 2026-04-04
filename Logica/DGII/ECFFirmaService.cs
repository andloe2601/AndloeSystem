using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Xml;
using Entidad;

namespace Logica.DGII
{
    public sealed class ECFFirmaService
    {
        public EcfFirmaResult FirmarXml(int facturaId, string xmlSinFirmar, X509Certificate2 cert, string? usuario = null)
        {
            if (string.IsNullOrWhiteSpace(xmlSinFirmar))
                throw new ArgumentException("XML sin firmar vacío.", nameof(xmlSinFirmar));

            if (cert == null)
                throw new ArgumentNullException(nameof(cert));

            var xmlDoc = new XmlDocument { PreserveWhitespace = true };
            xmlDoc.LoadXml(xmlSinFirmar);

            var signedXml = new SignedXml(xmlDoc)
            {
                SigningKey = cert.GetRSAPrivateKey()
            };

            signedXml.SignedInfo.CanonicalizationMethod = SignedXml.XmlDsigCanonicalizationUrl;
            signedXml.SignedInfo.SignatureMethod = SignedXml.XmlDsigRSASHA256Url;

            var reference = new Reference(string.Empty);
            reference.DigestMethod = SignedXml.XmlDsigSHA256Url;
            reference.AddTransform(new XmlDsigEnvelopedSignatureTransform());
            signedXml.AddReference(reference);

            var keyInfo = new KeyInfo();
            keyInfo.AddClause(new KeyInfoX509Data(cert));
            signedXml.KeyInfo = keyInfo;

            signedXml.ComputeSignature();

            var xmlSignature = signedXml.GetXml();
            xmlDoc.DocumentElement?.AppendChild(xmlDoc.ImportNode(xmlSignature, true));

            var xmlFirmado = xmlDoc.OuterXml;

            string? digestValue = null;
            if (signedXml.SignedInfo.References.Count > 0)
            {
                var firstRef = (Reference)signedXml.SignedInfo.References[0];
                if (firstRef.DigestValue != null)
                    digestValue = Convert.ToBase64String(firstRef.DigestValue);
            }

            string? signatureValue = null;
            if (signedXml.Signature?.SignatureValue != null)
                signatureValue = Convert.ToBase64String(signedXml.Signature.SignatureValue);

            using var sha256 = SHA256.Create();
            var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(xmlFirmado));
            var hashDocumento = Convert.ToHexString(hashBytes);

            return new EcfFirmaResult
            {
                FacturaId = facturaId,
                XmlFirmado = xmlFirmado,
                FechaHoraFirma = DateTime.Now,
                DigestValue = digestValue,
                SignatureValue = signatureValue,
                CanonicalizationMethod = signedXml.SignedInfo.CanonicalizationMethod,
                SignatureMethod = signedXml.SignedInfo.SignatureMethod,
                CertThumbprint = cert.Thumbprint,
                CertSerialNumber = cert.SerialNumber,
                CertIssuer = cert.Issuer,
                CertSubject = cert.Subject,
                HashDocumento = hashDocumento,
                Usuario = usuario
            };
        }
    }
}