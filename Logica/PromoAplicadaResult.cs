namespace Andloe.Logica
{
    // Resultado estándar que el POS usará después de evaluar promociones.
    public class PromoAplicadaResult
    {
        public int PromoId { get; set; }
        public string CodigoPromo { get; set; } = string.Empty;
        public string NombrePromo { get; set; } = string.Empty;
        public string TipoPromo { get; set; } = string.Empty;
        public int Prioridad { get; set; }

        public int? ReglaId { get; set; }
        public string TipoRegla { get; set; } = string.Empty;

        public decimal DescuentoPct { get; set; }
        public decimal DescuentoMonto { get; set; }
        public decimal PrecioFijo { get; set; }

        // ---- CAMPOS EXTRA PARA PACK / CONDICIONES ----
        public decimal? MinCantidad { get; set; }          // r.MinCantidad
        public string? PackBuyProducto { get; set; }       // r.Pack_BuyProducto
        public decimal PackBuyCant { get; set; }           // r.Pack_BuyCant
        public decimal PackPrecio { get; set; }            // r.Pack_Precio

        // Resultado final
        public decimal MontoDescuentoCalculado { get; set; }
        public decimal PrecioUnitFinal { get; set; } = 0m;
    }
}
