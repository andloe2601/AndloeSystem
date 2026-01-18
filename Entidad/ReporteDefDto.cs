namespace Andloe.Entidad
{
    public sealed class ReporteDefDto
    {
        public int ReporteId { get; set; }
        public string Modulo { get; set; } = "";
        public string Actividad { get; set; } = "";
        public string Codigo { get; set; } = "";
        public string Nombre { get; set; } = "";
        public string Motor { get; set; } = "";
        public string RutaArchivo { get; set; } = "";

        public int Orden { get; set; }
    }
}
