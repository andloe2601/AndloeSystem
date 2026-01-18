namespace Andloe.Entidad
{
    public class Categoria
    {
        public int CategoriaId { get; set; }
        public string Nombre { get; set; } = "";
        public string Codigo { get; set; } = "";
        public byte Estado { get; set; } = 1;
    }
}
