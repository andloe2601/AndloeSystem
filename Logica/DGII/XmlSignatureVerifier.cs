using System;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Xml;

namespace Andloe.Logica.DGII
{
    public static class XmlSignatureVerifier
    {
        public static bool Verificar(string xml)
        {
            if (string.IsNullOrWhiteSpace(xml)) return false;

            var doc = new XmlDocument { PreserveWhitespace = true };
            doc.LoadXml(xml);

            var nsm = new XmlNamespaceManager(doc.NameTable);
            nsm.AddNamespace("ds", SignedXml.XmlDsigNamespaceUrl);

            var sigNode = doc.SelectSingleNode("//ds:Signature", nsm) as XmlElement;
            if (sigNode == null) return false;

            var signedXml = new SignedXml(doc);
            signedXml.LoadXml(sigNode);

            return signedXml.CheckSignature();
        }
    }
}