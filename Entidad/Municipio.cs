namespace Andloe.Entidad
{
    public class Municipio
    {
        public int MunicipioId { get; set; }
        public string CodigoMunicipio { get; set; } = "";
        public string CodigoProvincia { get; set; } = "";
        public string Nombre { get; set; } = "";
        public int ProvinciaId { get; set; }
        public bool Estado { get; set; }
    }
}