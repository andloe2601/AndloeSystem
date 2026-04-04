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
            if (v == null) throw new ArgumentNullException(nameof(v));
            if (tx == null) throw new ArgumentNullException(nameof(tx));

            var cn = tx.Connection
                     ?? throw new InvalidOperationException("Transacción sin conexión asociada.");

            // Reglas seguras (no asumimos, solo blindamos)
            var moneda = string.IsNullOrWhiteSpace(v.MonedaCodigo) ? "DOP" : v.MonedaCodigo.Trim();
            var estado = string.IsNullOrWhiteSpace(v.Estado) ? "FACTURADA" : v.Estado.Trim();
            var fechaCreacion = v.FechaCreacion == default ? DateTime.Now : v.FechaCreacion;
            var fecha = v.Fecha == default ? DateTime.Now : v.Fecha;

            // IMPORTANTE: en tu BD TerminoPagoId es NOT NULL con DEFAULT (1),
            // pero aquí lo garantizamos si viene 0 o negativo.
            var terminoPagoId = v.TerminoPagoId <= 0 ? 1 : v.TerminoPagoId;

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
SELECT
    @NoDocumento,
    @Fecha,
    @ClienteCodigo,
    COALESCE(C.ClienteId, @ClienteIdManual),
    COALESCE(C.Nombre,    @NombreClienteManual),
    COALESCE(C.Email,     @EmailClienteManual),
    COALESCE(C.Telefono,  @TelefonoClienteManual),
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
FROM (SELECT 1 AS X) D
OUTER APPLY (
    SELECT TOP(1) ClienteId, Nombre, Email, Telefono
    FROM dbo.Cliente
    WHERE [Código] = @ClienteCodigo
) C;

SELECT CAST(SCOPE_IDENTITY() AS BIGINT);", cn, tx);

            // NoDocumento
            cmd.Parameters.Add("@NoDocumento", SqlDbType.VarChar, 20).Value = v.NoDocumento?.Trim() ?? "";

            // Fecha
            cmd.Parameters.Add("@Fecha", SqlDbType.DateTime).Value = fecha;

            // ClienteCodigo
            if (string.IsNullOrWhiteSpace(v.ClienteCodigo))
                cmd.Parameters.Add("@ClienteCodigo", SqlDbType.VarChar, 20).Value = DBNull.Value;
            else
                cmd.Parameters.Add("@ClienteCodigo", SqlDbType.VarChar, 20).Value = v.ClienteCodigo.Trim();

            // Datos manuales (por si el cliente no existe en tabla Cliente)
            // No forzamos nada: si tú no los llenas, quedan NULL y ya.
            cmd.Parameters.Add("@ClienteIdManual", SqlDbType.Int).Value =
                v.ClienteId.HasValue ? v.ClienteId.Value : DBNull.Value;

            cmd.Parameters.Add("@NombreClienteManual", SqlDbType.VarChar, 120).Value =
                string.IsNullOrWhiteSpace(v.NombreCliente) ? DBNull.Value : v.NombreCliente.Trim();

            cmd.Parameters.Add("@EmailClienteManual", SqlDbType.VarChar, 120).Value =
                string.IsNullOrWhiteSpace(v.EmailCliente) ? DBNull.Value : v.EmailCliente.Trim();

            cmd.Parameters.Add("@TelefonoClienteManual", SqlDbType.VarChar, 40).Value =
                string.IsNullOrWhiteSpace(v.TelefonoCliente) ? DBNull.Value : v.TelefonoCliente.Trim();

            // Moneda
            cmd.Parameters.Add("@MonedaCodigo", SqlDbType.VarChar, 3).Value = moneda;

            // TerminoPagoId (ya blindado)
            cmd.Parameters.Add("@TerminoPagoId", SqlDbType.Int).Value = terminoPagoId;

            // TasaCambio
            var pTasa = cmd.Parameters.Add("@TasaCambio", SqlDbType.Decimal);
            pTasa.Precision = 18;
            pTasa.Scale = 6;
            pTasa.Value = v.TasaCambio <= 0 ? 1m : v.TasaCambio;

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
            cmd.Parameters.Add("@Estado", SqlDbType.VarChar, 12).Value = estado;

            // Usuario
            cmd.Parameters.Add("@Usuario", SqlDbType.VarChar, 50).Value =
                string.IsNullOrWhiteSpace(v.Usuario) ? DBNull.Value : v.Usuario.Trim();

            // Observacion
            cmd.Parameters.Add("@Observacion", SqlDbType.VarChar, 200).Value =
                string.IsNullOrWhiteSpace(v.Observacion) ? DBNull.Value : v.Observacion.Trim();

            // FechaCreacion
            cmd.Parameters.Add("@FechaCreacion", SqlDbType.DateTime).Value = fechaCreacion;

            // MedioPagoId
            cmd.Parameters.Add("@MedioPagoId", SqlDbType.Int).Value =
                v.MedioPagoId.HasValue ? v.MedioPagoId.Value : DBNull.Value;

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

            // Totales en moneda (si vienen 0, caen al valor base)
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

            // POS_CajaNumero
            cmd.Parameters.Add("@POS_CajaNumero", SqlDbType.NVarChar, 10).Value =
                string.IsNullOrWhiteSpace(v.POS_CajaNumero) ? DBNull.Value : v.POS_CajaNumero.Trim();

            var obj = cmd.ExecuteScalar();
            return (obj == null || obj == DBNull.Value) ? 0L : Convert.ToInt64(obj);
        }
    }
}
