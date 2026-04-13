public class ClienteDto
{
    public int ClienteId { get; set; }
    public string Codigo { get; set; } = "";
    public string Nombre { get; set; } = "";
    public string? CodVendedor { get; set; }
    public string? RncCedula { get; set; }
    public string? Direccion { get; set; }
    public string? Telefono { get; set; }
    public int? TerminoPagoId { get; set; }
    public byte Tipo { get; set; }

    public string? RazonSocialFiscal { get; set; }
    public string? CorreoFiscal { get; set; }
    public string? ProvinciaCodigo { get; set; }
    public string? MunicipioCodigo { get; set; }
    public string? PaisCodigo { get; set; }
    public string? IdentificadorExtranjero { get; set; }
    public bool EsExtranjero { get; set; }
    public bool ValidadoDGII { get; set; }
}