namespace Andloe.Entidad
{
    public class Vendedor
    {
        public int VendedorId { get; set; }
        public string Codigo { get; set; } = "";
        public string Nombre { get; set; } = "";
        public string? Email { get; set; }
        public string? Telefono { get; set; }
        public bool Estado { get; set; } = true;
    }
}
