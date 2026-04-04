using System;
using System.Security.Cryptography.X509Certificates;

namespace Andloe.Logica.DGII
{
    public sealed class XmlSignerPfx
    {
        private readonly X509Certificate2 _cert;

        public XmlSignerPfx(string pfxPath, string? password)
        {
            if (string.IsNullOrWhiteSpace(pfxPath))
                throw new ArgumentException("Ruta PFX inválida.", nameof(pfxPath));

            // ✅ Compatible con .NET 7 / 8 sin named parameters
            _cert = X509CertificateLoader.LoadPkcs12FromFile(
                pfxPath,
                password,
                X509KeyStorageFlags.MachineKeySet
                | X509KeyStorageFlags.EphemeralKeySet
                | X509KeyStorageFlags.Exportable
            );
        }

        public X509Certificate2 Certificate => _cert;
    }
}