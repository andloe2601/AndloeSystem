namespace Andloe.Entidad
{
    public class Almacen
    {
        public int AlmacenId { get; set; }
        public int SucursalId { get; set; }
        public string Codigo { get; set; } = "";
        public string Nombre { get; set; } = "";
        public bool Estado { get; set; }
        public int EmpresaId { get; set; }
    }
}