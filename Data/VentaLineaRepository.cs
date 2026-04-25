using Andloe.Entidad;
using Microsoft.Data.SqlClient;
using System;
using System.Data;

namespace Andloe.Data
{
    public class VentaLineaRepository
    {
        /// <summary>
        /// Inserta una línea de venta en la tabla VentaLin.
        /// Usa la transacción abierta desde PosService.
        /// </summary>
        public void InsertLinea(VentaLinea l, SqlTransaction tx)
        {
            if (tx == null)
                throw new ArgumentNullException(nameof(tx));

            var cn = tx.Connection
                     ?? throw new InvalidOperationException("Transacción sin conexión asociada.");

            using var cmd = new SqlCommand(@"
                INSERT INTO dbo.VentaLin(
                    VentaId,
                    Linea,
                    NoProducto,
                    Descripcion,
                    Cantidad,
                    PrecioUnitario,
                    DescuentoLinea,
                    ItbisPorc,
                    ItbisMonto,
                    Importe,
                    ProductoCodigo,
                    PrecioUnit,
PrecioIncluyeITBIS,
                    DescuentoMoneda,
                    ImporteMoneda,
                    ItbisMoneda,
                    TotalMoneda
                )
                VALUES(
                    @VentaId,
                    @Linea,
                    @NoProducto,
                    @Descripcion,
                    @Cantidad,
                    @PrecioUnitario,
                    @DescuentoLinea,
                    @ItbisPorc,
                    @ItbisMonto,
                    @Importe,
                    @ProductoCodigo,
                    @PrecioUnit,
                    @PrecioIncluyeITBIS,
                    @DescuentoMoneda,
                    @ImporteMoneda,
                    @ItbisMoneda,
                    @TotalMoneda
                );", cn, tx);

            cmd.Parameters.Add("@VentaId", SqlDbType.BigInt).Value = l.VentaId;
            cmd.Parameters.Add("@Linea", SqlDbType.Int).Value = l.Linea;

            // NoProducto = ProductoCodigo
            cmd.Parameters.Add("@NoProducto", SqlDbType.VarChar, 20)
                .Value = l.ProductoCodigo;

            cmd.Parameters.Add("@Descripcion", SqlDbType.VarChar, 100)
                .Value = l.Descripcion ?? string.Empty;

            // Cantidad
            var pCant = cmd.Parameters.Add("@Cantidad", SqlDbType.Decimal);
            pCant.Precision = 18; pCant.Scale = 3; pCant.Value = l.Cantidad;

            // PrecioUnitario (compatibilidad)
            var pPU1 = cmd.Parameters.Add("@PrecioUnitario", SqlDbType.Decimal);
            pPU1.Precision = 18; pPU1.Scale = 2; pPU1.Value = l.PrecioUnit;

            // DescuentoLinea (sin descuentos en POS)
            var pDescLin = cmd.Parameters.Add("@DescuentoLinea", SqlDbType.Decimal);
            pDescLin.Precision = 18; pDescLin.Scale = 2; pDescLin.Value = 0m;

            // ItbisPorc
            var pItbPct = cmd.Parameters.Add("@ItbisPorc", SqlDbType.Decimal);
            pItbPct.Precision = 9; pItbPct.Scale = 4; pItbPct.Value = l.ItbisPct;

            // ItbisMonto
            var pItbMonto = cmd.Parameters.Add("@ItbisMonto", SqlDbType.Decimal);
            pItbMonto.Precision = 18; pItbMonto.Scale = 2; pItbMonto.Value = l.ItbisMonto;

            // Importe (subtotal sin ITBIS)
            var pImp = cmd.Parameters.Add("@Importe", SqlDbType.Decimal);
            pImp.Precision = 18; pImp.Scale = 2; pImp.Value = l.Importe;

            // ProductoCodigo
            cmd.Parameters.Add("@ProductoCodigo", SqlDbType.VarChar, 20)
                .Value = l.ProductoCodigo;

            // PrecioUnit (segunda columna)
            var pPU2 = cmd.Parameters.Add("@PrecioUnit", SqlDbType.Decimal);
            pPU2.Precision = 18; pPU2.Scale = 2; pPU2.Value = l.PrecioUnit;

            // Precio incluye ITBIS
            cmd.Parameters.Add("@PrecioIncluyeITBIS", SqlDbType.Bit)
                .Value = l.PrecioIncluyeITBIS;

            // Moneda (igual a base en POS)
            cmd.Parameters.Add("@DescuentoMoneda", SqlDbType.Decimal).Value = 0m;

            var pImpMon = cmd.Parameters.Add("@ImporteMoneda", SqlDbType.Decimal);
            pImpMon.Precision = 18; pImpMon.Scale = 2; pImpMon.Value = l.Importe;

            var pItbMon = cmd.Parameters.Add("@ItbisMoneda", SqlDbType.Decimal);
            pItbMon.Precision = 18; pItbMon.Scale = 2; pItbMon.Value = l.ItbisMonto;

            var pTotMon = cmd.Parameters.Add("@TotalMoneda", SqlDbType.Decimal);
            pTotMon.Precision = 18; pTotMon.Scale = 2; pTotMon.Value = l.TotalLinea;

            cmd.ExecuteNonQuery();
        }
    }
}
