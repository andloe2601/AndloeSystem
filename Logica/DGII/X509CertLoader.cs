using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace Andloe.Logica.DGII
{
    public static class X509CertLoader
    {
        public static X509Certificate2 LoadPkcs12FromFile(string path, string? password)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("Ruta del certificado vacía.");

            if (!File.Exists(path))
                throw new FileNotFoundException("No se encontró el certificado.", path);

            return new X509Certificate2(
                path,
                password,
                X509KeyStorageFlags.MachineKeySet |
                X509KeyStorageFlags.PersistKeySet |
                X509KeyStorageFlags.Exportable
            );
        }
    }
}