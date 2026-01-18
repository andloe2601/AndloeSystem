using System;

namespace Andloe.Entidad
{
    /// <summary>
    /// Línea de Kardex (movimiento de inventario) para un producto.
    /// </summary>
    public class KardexItemDto
    {
        public DateTime Fecha { get; set; }

        /// <summary>
        /// Tipo de movimiento en cabecera: ENTRADA, SALIDA, TRASLADO.
        /// </summary>
        public string Tipo { get; set; } = "";

        /// <summary>
        /// Texto de origen: COMPRA, VENTA, AJUSTE, etc.
        /// </summary>
        public string Origen { get; set; } = "";

        public long? OrigenId { get; set; }

        public int? AlmacenOrigenId { get; set; }
        public int? AlmacenDestinoId { get; set; }

        /// <summary>
        /// Cantidad que entra por esta línea (si aplica).
        /// </summary>
        public decimal Entrada { get; set; }

        /// <summary>
        /// Cantidad que sale por esta línea (si aplica).
        /// </summary>
        public decimal Salida { get; set; }

        /// <summary>
        /// Existencia acumulada del producto después de aplicar esta línea.
        /// </summary>
        public decimal Existencia { get; set; }

        /// <summary>
        /// Costo unitario asociado a la línea (para valorización rápida).
        /// </summary>
        public decimal CostoUnitario { get; set; }

        /// <summary>
        /// Costo total = (Entrada - Salida) * CostoUnitario,
        /// normalmente CostoUnitario * CantidadMovimiento en valor absoluto.
        /// </summary>
        public decimal CostoTotal { get; set; }

        public string Usuario { get; set; } = "";
        public string Observacion { get; set; } = "";
    }
}
