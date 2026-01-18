namespace Andloe.Entidad
{
    public class Impuesto
    {
        public int ImpuestoId { get; set; }
        public string Codigo { get; set; } = "";
        public string Nombre { get; set; } = "";
        public decimal Porcentaje { get; set; }
        public bool Estado { get; set; }
    }
}
