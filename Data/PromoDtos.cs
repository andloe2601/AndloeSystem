using System;
using System.Collections.Generic;

namespace Andloe.Data
{
    public class PromoProductoDetalle
    {
        public int PromoId { get; set; }
        public string Codigo { get; set; } = "";
        public string Nombre { get; set; } = "";
        public bool Activa { get; set; }

        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }

        public bool Lunes { get; set; }
        public bool Martes { get; set; }
        public bool Miercoles { get; set; }
        public bool Jueves { get; set; }
        public bool Viernes { get; set; }
        public bool Sabado { get; set; }
        public bool Domingo { get; set; }

        public decimal DescuentoPct { get; set; }
        public decimal PrecioFijo { get; set; }

        public decimal CantidadMinimaGrupo { get; set; } = 1m;
        // Pack
        public bool EsPack { get; set; }
        public decimal PackCantidad { get; set; }
        public decimal PackPrecioTotal { get; set; }

        public List<PromoProductoDetalleProducto> Productos { get; set; } = new();
    }

    public class PromoProductoDetalleProducto
    {
        public string Codigo { get; set; } = "";
        public string Nombre { get; set; } = "";
        public decimal PrecioVenta { get; set; }
        public decimal PrecioCoste { get; set; }
    }
}
