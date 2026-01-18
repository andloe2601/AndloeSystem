namespace Andloe.Entidad
{
    public class CodBarra
    {
        // Mapea a dbo.CodBarras
        public string CodigoBarras { get; set; } = ""; // [Cód_ barras]
        public string NoProducto { get; set; } = "";   // [Nº producto]
        public int Tipo { get; set; }                  // [Tipo]
        public string Usuario { get; set; } = "system";
        public System.DateTime? UltimaFechaUtilizacion { get; set; }
    }
}
