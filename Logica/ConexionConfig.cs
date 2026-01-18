namespace Andloe.Logica;

public class ConexionConfig
{
    public string DataSource { get; set; } = ".";
    public string InitialCatalog { get; set; } = "AndloeV1.1";
    public bool IntegratedSecurity { get; set; } = true;
    public string? UserID { get; set; }
    public string? Password { get; set; }
    public bool Encrypt { get; set; } = false;
    public bool TrustServerCertificate { get; set; } = true;

    public string BuildConnectionString()
    {
        var parts = new List<string>
        {
            $"Data Source={DataSource}",
            $"Initial Catalog={InitialCatalog}",
            $"Integrated Security={(IntegratedSecurity ? "True" : "False")}",
            $"Encrypt={(Encrypt ? "True" : "False")}",
            $"TrustServerCertificate={(TrustServerCertificate ? "True" : "False")}"
        };

        if (!IntegratedSecurity)
        {
            parts.Add($"User ID={UserID ?? ""}");
            parts.Add($"Password={Password ?? ""}");
        }

        return string.Join(";", parts);
    }
}
