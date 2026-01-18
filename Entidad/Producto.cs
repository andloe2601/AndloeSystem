namespace Andloe.Entidad
{
    public class Producto
    {
        public string Codigo { get; set; } = null!;
        public string? Descripcion { get; set; }
        public string? Clase { get; set; }

        public string? UnidadBase { get; set; }            // [Unidad medida base]
        public string? UnidadMedidaCodigo { get; set; }    // cat. de unidad (si usas tabla UnidadMedida)

        public decimal PrecioVenta { get; set; }           // NOT NULL (default 0 en DB)
        public decimal? PrecioCoste { get; set; }
        public decimal? PrecioCompraPromedio { get; set; }

        public decimal StockActual { get; set; }           // si lo usas como lectura
        public decimal? StockMinimo { get; set; }
        public decimal? StockMaximo { get; set; }

        public string? CodReferencia { get; set; }         // [Cod_ Referencia]
        public string? Referencia { get; set; }            // [Descripción alias]

        public decimal? PrecioMayor { get; set; }               // Precio x mayor
        public decimal? UltimoPrecioCompra { get; set; }        // Ultimo precio compra

        public byte[]? Imagen { get; set; }
        public decimal? PorcFijoBeneficio { get; set; }    // [% fijo beneficio]

        public int Estado { get; set; } = 1;               // 1=Activo,0=Inactivo
        public DateTime? FechaCreacion { get; set; }
        public int? CategoriaId { get; set; }
        public int? SubcategoriaId { get; set; }

        public int? ImpuestoId { get; set; }          
        public decimal ItbisPct { get; set; }         

        public decimal? DescuentoPctMax { get; set; }  // ej. 10.00m

        /// <summary>
        /// Rellena defaults seguros para columnas problemáticas.
        /// </summary>
        public void NormalizeDefaults()
        {
            if (string.IsNullOrWhiteSpace(UnidadBase))
                UnidadBase = "UND";

            if (string.IsNullOrWhiteSpace(UnidadMedidaCodigo))
                UnidadMedidaCodigo = "UND";

            if (string.IsNullOrWhiteSpace(CodReferencia))
                CodReferencia = "N/A";

            if (string.IsNullOrWhiteSpace(Referencia))
                Referencia = "N/A";

            if (Estado != 0 && Estado != 1)
                Estado = 1;
        }

        /// <summary>
        /// Valida reglas de negocio antes de guardar.
        /// Lanza excepción con mensaje claro si algo no cuadra.
        /// </summary>
        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(Descripcion))
                throw new ArgumentException("La descripción es obligatoria.");

            if (PrecioVenta < 0)
                throw new ArgumentException("El precio de venta no puede ser negativo.");

            if (StockMinimo is < 0) throw new ArgumentException("Stock mínimo no puede ser negativo.");
            if (StockMaximo is < 0) throw new ArgumentException("Stock máximo no puede ser negativo.");
            if (StockMinimo.HasValue && StockMaximo.HasValue && StockMinimo > StockMaximo)
                throw new ArgumentException("El stock mínimo no puede ser mayor que el stock máximo.");
        }
    }
}
