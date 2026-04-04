using System;

namespace Andloe.Entidad
{
    public class Producto
    {
        public int ProductoId { get; set; }

        // =========================
        // Compatibilidad con tabla Producto
        // =========================
        public string Codigo { get; set; } = string.Empty;
        public string CodReferencia { get; set; } = string.Empty;
        public string Referencia { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string UnidadBase { get; set; } = string.Empty;

        // Legacy 0/1
        public int Estado { get; set; } = 1;

        public decimal PrecioVenta { get; set; }
        public decimal PrecioCoste { get; set; }
        public decimal PrecioMayor { get; set; }
        public decimal PorcFijoBeneficio { get; set; }
        public decimal PrecioCompraPromedio { get; set; }
        public decimal UltimoPrecioCompra { get; set; }
        public decimal StockActual { get; set; }
        public decimal DescuentoPctMax { get; set; }

        public decimal StockMinimo { get; set; }
        public decimal StockMaximo { get; set; }

        public int? CategoriaId { get; set; }
        public int? SubcategoriaId { get; set; }
        public int? ImpuestoId { get; set; }

        public DateTime? FechaCreacion { get; set; }

        // =========================
        // e-CF / Fiscal
        // =========================
        public string DescripcionFiscal { get; set; } = string.Empty;
        public bool PrecioIncluyeITBIS { get; set; }
        public string CodigoItemFiscal { get; set; } = string.Empty;

        /// <summary>
        /// 1 = Bien, 2 = Servicio
        /// </summary>
        public int TipoProducto { get; set; } = 1;

        public int? UnidadMedidaECFId { get; set; }
        public string UnidadMedidaCodigo { get; set; } = string.Empty;

        /// <summary>
        /// NO viene de la tabla Producto.
        /// Se llena desde ECFUnidadMedida por JOIN o lookup.
        /// </summary>
        public string UnidadMedidaDGII { get; set; } = string.Empty;

        public bool EsExento { get; set; }

        // =========================
        // Operación
        // =========================
        public bool PermiteVenta { get; set; } = true;
        public bool PermiteCompra { get; set; } = true;
        public bool ManejaInventario { get; set; } = true;
        public bool Activo { get; set; } = true;
        public bool RequiereLote { get; set; }
        public bool RequiereVencimiento { get; set; }

        // =========================
        // Helpers
        // =========================
        public bool EstadoActivo => Estado == 1;
        public bool EsBien => TipoProducto == 1;
        public bool EsServicio => TipoProducto == 2;

        public void NormalizeDefaults()
        {
            Codigo = (Codigo ?? string.Empty).Trim();
            CodReferencia = (CodReferencia ?? string.Empty).Trim();
            Referencia = (Referencia ?? string.Empty).Trim();
            Descripcion = (Descripcion ?? string.Empty).Trim();
            UnidadBase = (UnidadBase ?? string.Empty).Trim();
            DescripcionFiscal = (DescripcionFiscal ?? string.Empty).Trim();
            CodigoItemFiscal = (CodigoItemFiscal ?? string.Empty).Trim();
            UnidadMedidaCodigo = (UnidadMedidaCodigo ?? string.Empty).Trim();
            UnidadMedidaDGII = (UnidadMedidaDGII ?? string.Empty).Trim();

            if (string.IsNullOrWhiteSpace(DescripcionFiscal))
                DescripcionFiscal = Descripcion;

            if (string.IsNullOrWhiteSpace(CodReferencia))
                CodReferencia = Referencia;

            if (string.IsNullOrWhiteSpace(Referencia))
                Referencia = CodReferencia;

            if (Estado != 0 && Estado != 1)
                Estado = 1;

            if (TipoProducto != 1 && TipoProducto != 2)
                TipoProducto = 1;

            if (PrecioVenta < 0) PrecioVenta = 0m;
            if (PrecioCoste < 0) PrecioCoste = 0m;
            if (PrecioMayor < 0) PrecioMayor = 0m;
            if (PorcFijoBeneficio < 0) PorcFijoBeneficio = 0m;
            if (PrecioCompraPromedio < 0) PrecioCompraPromedio = 0m;
            if (UltimoPrecioCompra < 0) UltimoPrecioCompra = 0m;
            if (StockActual < 0) StockActual = 0m;
            if (StockMinimo < 0) StockMinimo = 0m;
            if (StockMaximo < 0) StockMaximo = 0m;
            if (DescuentoPctMax < 0) DescuentoPctMax = 0m;

            if (string.IsNullOrWhiteSpace(UnidadBase) && !string.IsNullOrWhiteSpace(UnidadMedidaCodigo))
                UnidadBase = UnidadMedidaCodigo;

            if (string.IsNullOrWhiteSpace(UnidadMedidaCodigo) && !string.IsNullOrWhiteSpace(UnidadBase))
                UnidadMedidaCodigo = UnidadBase;

            // Este campo NO es persistido en Producto; si no viene cargado desde JOIN,
            // usamos fallback temporal para no romper validaciones internas.
            if (string.IsNullOrWhiteSpace(UnidadMedidaDGII) && !string.IsNullOrWhiteSpace(UnidadMedidaCodigo))
                UnidadMedidaDGII = UnidadMedidaCodigo;

            if (EsExento)
                ImpuestoId = null;

            // Mantener sincronía básica con campo legacy Estado
            Activo = Estado == 1;
        }

        public void ApplyEcFDefaults()
        {
            NormalizeDefaults();

            if (string.IsNullOrWhiteSpace(DescripcionFiscal))
                DescripcionFiscal = Descripcion;

            if (TipoProducto != 1 && TipoProducto != 2)
                TipoProducto = 1;

            if (string.IsNullOrWhiteSpace(UnidadMedidaCodigo))
                UnidadMedidaCodigo = "UND";

            if (string.IsNullOrWhiteSpace(UnidadBase))
                UnidadBase = UnidadMedidaCodigo;

            if (string.IsNullOrWhiteSpace(UnidadMedidaDGII))
                UnidadMedidaDGII = UnidadMedidaCodigo;

            if (!UnidadMedidaECFId.HasValue || UnidadMedidaECFId.Value <= 0)
                UnidadMedidaECFId = 43; // UND temporal por defecto
        }

        public void Validate()
        {
            NormalizeDefaults();

            if (string.IsNullOrWhiteSpace(Codigo))
                throw new InvalidOperationException("El código del producto es obligatorio.");

            if (string.IsNullOrWhiteSpace(Descripcion))
                throw new InvalidOperationException("La descripción del producto es obligatoria.");

            if (string.IsNullOrWhiteSpace(DescripcionFiscal))
                throw new InvalidOperationException("La descripción fiscal del producto es obligatoria.");

            if (string.IsNullOrWhiteSpace(UnidadBase))
                throw new InvalidOperationException("La unidad base del producto es obligatoria.");

            if (string.IsNullOrWhiteSpace(UnidadMedidaCodigo))
                throw new InvalidOperationException("La unidad de medida del producto es obligatoria.");

            if (!UnidadMedidaECFId.HasValue || UnidadMedidaECFId.Value <= 0)
                throw new InvalidOperationException("La UnidadMedidaECFId del producto es obligatoria.");

            if (string.IsNullOrWhiteSpace(CodigoItemFiscal))
                throw new InvalidOperationException("El CódigoItemFiscal del producto es obligatorio.");

            if (TipoProducto != 1 && TipoProducto != 2)
                throw new InvalidOperationException("El TipoProducto debe ser 1 = Bien o 2 = Servicio.");

            if (PrecioVenta < 0)
                throw new InvalidOperationException("El precio de venta no puede ser negativo.");

            if (PrecioCoste < 0)
                throw new InvalidOperationException("El precio de costo no puede ser negativo.");

            if (PrecioMayor < 0)
                throw new InvalidOperationException("El precio mayor no puede ser negativo.");

            if (DescuentoPctMax < 0)
                throw new InvalidOperationException("El descuento máximo no puede ser negativo.");

            if (EsExento && ImpuestoId.HasValue)
                throw new InvalidOperationException("Un producto exento no debe tener impuesto asignado.");

            if (!EsExento && !ImpuestoId.HasValue)
                throw new InvalidOperationException("Debe indicar un impuesto o marcar el producto como exento.");
        }
    }
}