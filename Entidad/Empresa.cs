namespace Andloe.Entidad
{
    public sealed class Empresa
    {
        public int EmpresaId { get; set; }
        public string RazonSocial { get; set; } = "";
        public string RNC { get; set; } = "";
        public string MonedaBaseCodigo { get; set; } = "DOP";
        public string? Pais { get; set; }
        public string? Provincia { get; set; }
        public string? Ciudad { get; set; }
        public string? Direccion { get; set; }
        public string? Telefono { get; set; }
        public string? Email { get; set; }
        public bool Estado { get; set; } = true;
        public DateTime? FechaCreacion { get; set; }
        public byte[]? Logo { get; set; }
        public int? MunicipioId { get; set; }
    }
}