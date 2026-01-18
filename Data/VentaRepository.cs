using Andloe.Entidad;
using Microsoft.Data.SqlClient;
using System;
using System.Data;

namespace Andloe.Data
{
    public class VentaRepository
    {
        public long InsertCabecera(Venta v, SqlTransaction tx)
        {
            if (tx == null)
                throw new ArgumentNullException(nameof(tx));

            var cn = tx.Connection
                     ?? throw new InvalidOperationException("Transacción sin conexión asociada.");

            using var cmd = new SqlCommand(@"
INSERT INTO dbo.VentaCab(
    NoDocumento,
    Fecha,
    ClienteCodigo,
    ClienteId,
    NombreCliente,
    EmailCliente,
    TelefonoCliente,
    MonedaCodigo,
    TasaCambio,
    Subtotal,
    DescuentoTotal,
    ImpuestoTotal,
    Total,
    Estado,
    Usuario,
    Observacion,
    FechaCreacion,
    MedioPagoId,
    MontoPago,
    MontoCambio,
    SubTotalMoneda,
    ItbisMoneda,
    TotalMoneda,
    TerminoPagoId,
    POS_CajaNumero
)
VALUES(
    @NoDocumento,
    @Fecha,
    @ClienteCodigo,
    (SELECT TOP(1) ClienteId FROM dbo.Cliente WHERE [Código] = @ClienteCodigo),
    (SELECT TOP(1) Nombre    FROM dbo.Cliente WHERE [Código] = @ClienteCodigo),
    (SELECT TOP(1) Email     FROM dbo.Cliente WHERE [Código] = @ClienteCodigo),
    (SELECT TOP(1) Telefono  FROM dbo.Cliente WHERE [Código] = @ClienteCodigo),
    @MonedaCodigo,
    @TasaCambio,
    @Subtotal,
    @DescuentoTotal,
    @ImpuestoTotal,
    @Total,
    @Estado,
    @Usuario,
    @Observacion,
    @FechaCreacion,
    @MedioPagoId,
    @MontoPago,
    @MontoCambio,
    @SubTotalMoneda,
    @ItbisMoneda,
    @TotalMoneda,
    @TerminoPagoId,
    @POS_CajaNumero
);
SELECT CAST(SCOPE_IDENTITY() AS BIGINT);", cn, tx);

            // NoDocumento
            cmd.Parameters.Add("@NoDocumento", SqlDbType.VarChar, 20).Value = v.NoDocumento;

            // Fecha
            cmd.Parameters.Add("@Fecha", SqlDbType.DateTime).Value = v.Fecha;

            // ClienteCodigo
            if (string.IsNullOrWhiteSpace(v.ClienteCodigo))
                cmd.Parameters.Add("@ClienteCodigo", SqlDbType.VarChar, 20).Value = DBNull.Value;
            else
                cmd.Parameters.Add("@ClienteCodigo", SqlDbType.VarChar, 20).Value = v.ClienteCodigo;

            // Moneda
            cmd.Parameters.Add("@MonedaCodigo", SqlDbType.VarChar, 3).Value = v.MonedaCodigo ?? "DOP";

            // 🔹 TerminoPagoId
            cmd.Parameters.Add("@TerminoPagoId", SqlDbType.Int).Value = v.TerminoPagoId;

            // TasaCambio
            var pTasa = cmd.Parameters.Add("@TasaCambio", SqlDbType.Decimal);
            pTasa.Precision = 18;
            pTasa.Scale = 6;
            pTasa.Value = v.TasaCambio;

            // Subtotal
            var pSub = cmd.Parameters.Add("@Subtotal", SqlDbType.Decimal);
            pSub.Precision = 18;
            pSub.Scale = 2;
            pSub.Value = v.Subtotal;

            // DescuentoTotal
            var pDesc = cmd.Parameters.Add("@DescuentoTotal", SqlDbType.Decimal);
            pDesc.Precision = 18;
            pDesc.Scale = 2;
            pDesc.Value = v.DescuentoTotal;

            // ImpuestoTotal
            var pImp = cmd.Parameters.Add("@ImpuestoTotal", SqlDbType.Decimal);
            pImp.Precision = 18;
            pImp.Scale = 2;
            pImp.Value = v.ImpuestoTotal;

            // Total
            var pTot = cmd.Parameters.Add("@Total", SqlDbType.Decimal);
            pTot.Precision = 18;
            pTot.Scale = 2;
            pTot.Value = v.Total;

            // Estado
            cmd.Parameters.Add("@Estado", SqlDbType.VarChar, 12)
               .Value = v.Estado ?? "FACTURADA";

            // Usuario
            cmd.Parameters.Add("@Usuario", SqlDbType.VarChar, 50)
               .Value = (object?)v.Usuario ?? DBNull.Value;

            // Observacion
            cmd.Parameters.Add("@Observacion", SqlDbType.VarChar, 200)
               .Value = (object?)v.Observacion ?? DBNull.Value;

            // FechaCreacion
            cmd.Parameters.Add("@FechaCreacion", SqlDbType.DateTime)
               .Value = v.FechaCreacion == default ? DateTime.Now : v.FechaCreacion;

            // MedioPagoId
            if (v.MedioPagoId.HasValue)
                cmd.Parameters.Add("@MedioPagoId", SqlDbType.Int).Value = v.MedioPagoId.Value;
            else
                cmd.Parameters.Add("@MedioPagoId", SqlDbType.Int).Value = DBNull.Value;

            // MontoPago
            var pPago = cmd.Parameters.Add("@MontoPago", SqlDbType.Decimal);
            pPago.Precision = 18;
            pPago.Scale = 2;
            pPago.Value = v.MontoPago;

            // MontoCambio
            var pCambio = cmd.Parameters.Add("@MontoCambio", SqlDbType.Decimal);
            pCambio.Precision = 18;
            pCambio.Scale = 2;
            pCambio.Value = v.MontoCambio;

            // Totales en moneda
            var pSubMon = cmd.Parameters.Add("@SubTotalMoneda", SqlDbType.Decimal);
            pSubMon.Precision = 18;
            pSubMon.Scale = 2;
            pSubMon.Value = v.SubTotalMoneda == 0 ? v.Subtotal : v.SubTotalMoneda;

            var pItbMon = cmd.Parameters.Add("@ItbisMoneda", SqlDbType.Decimal);
            pItbMon.Precision = 18;
            pItbMon.Scale = 2;
            pItbMon.Value = v.ItbisMoneda == 0 ? v.ImpuestoTotal : v.ItbisMoneda;

            var pTotMon = cmd.Parameters.Add("@TotalMoneda", SqlDbType.Decimal);
            pTotMon.Precision = 18;
            pTotMon.Scale = 2;
            pTotMon.Value = v.TotalMoneda == 0 ? v.Total : v.TotalMoneda;

            // 🔹 POS_CajaNumero
            if (string.IsNullOrWhiteSpace(v.POS_CajaNumero))
                cmd.Parameters.Add("@POS_CajaNumero", SqlDbType.NVarChar, 10).Value = DBNull.Value;
            else
                cmd.Parameters.Add("@POS_CajaNumero", SqlDbType.NVarChar, 10).Value = v.POS_CajaNumero;

            var obj = cmd.ExecuteScalar();
            return (obj == null || obj == DBNull.Value) ? 0L : Convert.ToInt64(obj);
        }
    }
}
