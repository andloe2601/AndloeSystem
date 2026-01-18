using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Andloe.Logica
{
    public static class ConfigManager
    {
        // ===============================
        // RUTAS SEGURAS (AppData)
        // ===============================
        private static readonly string ConfigDir =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Andloe");

        private static readonly string ConfigFile =
            Path.Combine(ConfigDir, "conexion.cfg");

        // ===============================
        // RUTA LEGACY (migración)
        // ===============================
        private const string LegacyDir = @"C:\Andloe";
        private static readonly string LegacyFile =
            Path.Combine(LegacyDir, "conexion.txt");

        // ===============================
        // SEGURIDAD
        // ===============================
        private const string PasswordPrefix = "dpapi:";
        private static readonly byte[] Entropy = Encoding.UTF8.GetBytes("Andloe.Config.v1");

        // ===============================
        // API PUBLICA
        // ===============================
        public static bool ConfigExists()
        {
            MigrateLegacyIfNeeded();
            return File.Exists(ConfigFile);
        }

        public static ConexionConfig Load()
        {
            MigrateLegacyIfNeeded();

            var cfg = new ConexionConfig();
            if (!File.Exists(ConfigFile))
                return cfg;

            foreach (var raw in File.ReadAllLines(ConfigFile))
            {
                var line = raw.Trim();
                if (line.Length == 0 || line.StartsWith("#"))
                    continue;

                var idx = line.IndexOf('=');
                if (idx <= 0)
                    continue;

                var key = line[..idx].Trim().ToLowerInvariant();
                var val = line[(idx + 1)..].Trim();

                switch (key)
                {
                    case "datasource": cfg.DataSource = val; break;
                    case "initialcatalog": cfg.InitialCatalog = val; break;
                    case "integratedsecurity": cfg.IntegratedSecurity = val.Equals("true", StringComparison.OrdinalIgnoreCase); break;
                    case "userid": cfg.UserID = string.IsNullOrWhiteSpace(val) ? null : val; break;
                    case "password": cfg.Password = DecryptPassword(val); break;
                    case "encrypt": cfg.Encrypt = val.Equals("true", StringComparison.OrdinalIgnoreCase); break;
                    case "trustservercertificate": cfg.TrustServerCertificate = val.Equals("true", StringComparison.OrdinalIgnoreCase); break;
                }
            }

            return cfg;
        }

        public static void Save(ConexionConfig cfg)
        {
            Directory.CreateDirectory(ConfigDir);

            var lines = new[]
            {
                "# Configuracion de conexion Andloe",
                "# Password protegida con DPAPI (CurrentUser)",
                $"DataSource={cfg.DataSource}",
                $"InitialCatalog={cfg.InitialCatalog}",
                $"IntegratedSecurity={(cfg.IntegratedSecurity ? "true" : "false")}",
                $"UserID={cfg.UserID ?? ""}",
                $"Password={EncryptPassword(cfg.Password)}",
                $"Encrypt={(cfg.Encrypt ? "true" : "false")}",
                $"TrustServerCertificate={(cfg.TrustServerCertificate ? "true" : "false")}"
            };

            File.WriteAllLines(ConfigFile, lines);
        }

        public static string GetConnectionString()
        {
            return Load().BuildConnectionString();
        }

        public static string GetConfigFilePath()
        {
            return ConfigFile;
        }

        // ===============================
        // MIGRACION AUTOMATICA
        // ===============================
        private static void MigrateLegacyIfNeeded()
        {
            try
            {
                if (File.Exists(ConfigFile))
                    return;

                if (!File.Exists(LegacyFile))
                    return;

                var legacy = LoadLegacy(LegacyFile);
                Save(legacy);

                var bak = LegacyFile + ".migrated.bak";
                if (!File.Exists(bak))
                    File.Move(LegacyFile, bak);
            }
            catch
            {
                // No romper la app por config
            }
        }

        private static ConexionConfig LoadLegacy(string file)
        {
            var cfg = new ConexionConfig();

            foreach (var raw in File.ReadAllLines(file))
            {
                var line = raw.Trim();
                if (line.Length == 0 || line.StartsWith("#"))
                    continue;

                var idx = line.IndexOf('=');
                if (idx <= 0)
                    continue;

                var key = line[..idx].Trim().ToLowerInvariant();
                var val = line[(idx + 1)..].Trim();

                switch (key)
                {
                    case "datasource": cfg.DataSource = val; break;
                    case "initialcatalog": cfg.InitialCatalog = val; break;
                    case "integratedsecurity": cfg.IntegratedSecurity = val.Equals("true", StringComparison.OrdinalIgnoreCase); break;
                    case "userid": cfg.UserID = val; break;
                    case "password":
                        try
                        {
                            cfg.Password = Encoding.UTF8.GetString(Convert.FromBase64String(val));
                        }
                        catch
                        {
                            cfg.Password = val;
                        }
                        break;
                    case "encrypt": cfg.Encrypt = val.Equals("true", StringComparison.OrdinalIgnoreCase); break;
                    case "trustservercertificate": cfg.TrustServerCertificate = val.Equals("true", StringComparison.OrdinalIgnoreCase); break;
                }
            }

            return cfg;
        }

        // ===============================
        // DPAPI
        // ===============================
        private static string EncryptPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
                return "";

            var bytes = Encoding.UTF8.GetBytes(password);
            var protectedBytes = ProtectedData.Protect(bytes, Entropy, DataProtectionScope.CurrentUser);
            return PasswordPrefix + Convert.ToBase64String(protectedBytes);
        }

        private static string DecryptPassword(string stored)
        {
            if (string.IsNullOrWhiteSpace(stored))
                return null;

            if (stored.StartsWith(PasswordPrefix, StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    var protectedBytes = Convert.FromBase64String(stored.Substring(PasswordPrefix.Length));
                    var bytes = ProtectedData.Unprotect(protectedBytes, Entropy, DataProtectionScope.CurrentUser);
                    return Encoding.UTF8.GetString(bytes);
                }
                catch
                {
                    return null;
                }
            }

            // fallback: base64 o texto plano
            try
            {
                var bytes = Convert.FromBase64String(stored);
                return Encoding.UTF8.GetString(bytes);
            }
            catch
            {
                return stored;
            }
        }
    }
}
