using System;

namespace Andloe.Data
{
    public class PromoHistoricoRow
    {
        public int PromoId { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string TipoPromo { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;

        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }

        public string UsuarioCreacion { get; set; } = string.Empty;
        public DateTime FechaCreacion { get; set; }

        public string UsuarioMod { get; set; } = string.Empty;
        public DateTime? FechaMod { get; set; }
    }
}

public class PromoDetalleRow
{
    public string CodigoProducto { get; set; } = "";
    public string NombreProducto { get; set; } = "";
    public string TipoRegla { get; set; } = "";
    public decimal DescuentoPct { get; set; }
    public decimal PrecioFijo { get; set; }
    public decimal PackCantidad { get; set; }
    public decimal PackPrecio { get; set; }
}
