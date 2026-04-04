using System;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Security.Cryptography.X509Certificates;
using System.Xml;

namespace Andloe.Logica.DGII
{
    public sealed class XmlSignerSignedXml
    {
        private readonly X509Certificate2 _certificate;

        public XmlSignerSignedXml(X509Certificate2 certificate)
        {
            _certificate = certificate ?? throw new ArgumentNullException(nameof(certificate));
        }

        public string FirmarEnveloped(string xml)
        {
            var doc = new XmlDocument();
            doc.PreserveWhitespace = true;
            doc.LoadXml(xml);

            var signedXml = new SignedXml(doc)
            {
                SigningKey = _certificate.GetRSAPrivateKey()
            };

            signedXml.SignedInfo.CanonicalizationMethod =
                SignedXml.XmlDsigExcC14NTransformUrl;

            signedXml.SignedInfo.SignatureMethod =
                SignedXml.XmlDsigRSASHA256Url;

            var reference = new Reference();
            reference.Uri = "";

            reference.AddTransform(new XmlDsigEnvelopedSignatureTransform());
            reference.AddTransform(new XmlDsigExcC14NTransform());

            reference.DigestMethod = SignedXml.XmlDsigSHA256Url;

            signedXml.AddReference(reference);

            var keyInfo = new KeyInfo();
            keyInfo.AddClause(new KeyInfoX509Data(_certificate));

            signedXml.KeyInfo = keyInfo;

            signedXml.ComputeSignature();

            var xmlSignature = signedXml.GetXml();

            doc.DocumentElement!.AppendChild(
                doc.ImportNode(xmlSignature, true)
            );

            return doc.OuterXml;
        }
    }
}