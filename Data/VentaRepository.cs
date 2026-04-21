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

            var moneda = string.IsNullOrWhiteSpace(v.MonedaCodigo) ? "DOP" : v.MonedaCodigo.Trim();
            var estado = string.IsNullOrWhiteSpace(v.Estado) ? "FACTURADA" : v.Estado.Trim();
            var fechaCreacion = v.FechaCreacion == default ? DateTime.Now : v.FechaCreacion;
            var fecha = v.Fecha == default ? DateTime.Now : v.Fecha;
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
    MontoPago,
    MontoCambio,
    SubTotalMoneda,
    ItbisMoneda,
    TotalMoneda,
    TerminoPagoId,
    POS_CajaNumero,
    CajaId
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
    @MontoPago,
    @MontoCambio,
    @SubTotalMoneda,
    @ItbisMoneda,
    @TotalMoneda,
    @TerminoPagoId,
    @POS_CajaNumero,
    @CajaId
FROM (SELECT 1 AS X) D
OUTER APPLY (
    SELECT TOP(1) ClienteId, Nombre, Email, Telefono
    FROM dbo.Cliente
    WHERE [Código] = @ClienteCodigo
) C;

SELECT CAST(SCOPE_IDENTITY() AS BIGINT);", cn, tx);

            cmd.Parameters.Add("@NoDocumento", SqlDbType.VarChar, 20).Value = v.NoDocumento?.Trim() ?? "";
            cmd.Parameters.Add("@Fecha", SqlDbType.DateTime).Value = fecha;

            if (string.IsNullOrWhiteSpace(v.ClienteCodigo))
                cmd.Parameters.Add("@ClienteCodigo", SqlDbType.VarChar, 20).Value = DBNull.Value;
            else
                cmd.Parameters.Add("@ClienteCodigo", SqlDbType.VarChar, 20).Value = v.ClienteCodigo.Trim();

            cmd.Parameters.Add("@ClienteIdManual", SqlDbType.Int).Value = v.ClienteId > 0 ? v.ClienteId : DBNull.Value;
            cmd.Parameters.Add("@NombreClienteManual", SqlDbType.NVarChar, 240).Value =
                string.IsNullOrWhiteSpace(v.NombreCliente) ? DBNull.Value : v.NombreCliente.Trim();
            cmd.Parameters.Add("@EmailClienteManual", SqlDbType.NVarChar, 200).Value =
                string.IsNullOrWhiteSpace(v.EmailCliente) ? DBNull.Value : v.EmailCliente.Trim();
            cmd.Parameters.Add("@TelefonoClienteManual", SqlDbType.VarChar, 20).Value =
                string.IsNullOrWhiteSpace(v.TelefonoCliente) ? DBNull.Value : v.TelefonoCliente.Trim();

            cmd.Parameters.Add("@MonedaCodigo", SqlDbType.VarChar, 3).Value = moneda;

            var pTc = cmd.Parameters.Add("@TasaCambio", SqlDbType.Decimal);
            pTc.Precision = 18;
            pTc.Scale = 6;
            pTc.Value = v.TasaCambio <= 0 ? 1m : v.TasaCambio;

            var pSub = cmd.Parameters.Add("@Subtotal", SqlDbType.Decimal);
            pSub.Precision = 18;
            pSub.Scale = 2;
            pSub.Value = v.Subtotal;

            var pDto = cmd.Parameters.Add("@DescuentoTotal", SqlDbType.Decimal);
            pDto.Precision = 18;
            pDto.Scale = 2;
            pDto.Value = v.DescuentoTotal;

            var pItb = cmd.Parameters.Add("@ImpuestoTotal", SqlDbType.Decimal);
            pItb.Precision = 18;
            pItb.Scale = 2;
            pItb.Value = v.ImpuestoTotal;

            var pTot = cmd.Parameters.Add("@Total", SqlDbType.Decimal);
            pTot.Precision = 18;
            pTot.Scale = 2;
            pTot.Value = v.Total;

            cmd.Parameters.Add("@Estado", SqlDbType.VarChar, 12).Value = estado;
            cmd.Parameters.Add("@Usuario", SqlDbType.VarChar, 50).Value =
                string.IsNullOrWhiteSpace(v.Usuario) ? DBNull.Value : v.Usuario.Trim();
            cmd.Parameters.Add("@Observacion", SqlDbType.VarChar, 200).Value =
                string.IsNullOrWhiteSpace(v.Observacion) ? DBNull.Value : v.Observacion.Trim();
            cmd.Parameters.Add("@FechaCreacion", SqlDbType.DateTime).Value = fechaCreacion;

            var pMontoPago = cmd.Parameters.Add("@MontoPago", SqlDbType.Decimal);
            pMontoPago.Precision = 18;
            pMontoPago.Scale = 2;
            pMontoPago.Value = v.MontoPago;

            var pMontoCambio = cmd.Parameters.Add("@MontoCambio", SqlDbType.Decimal);
            pMontoCambio.Precision = 18;
            pMontoCambio.Scale = 2;
            pMontoCambio.Value = v.MontoCambio;

            var pSubMon = cmd.Parameters.Add("@SubTotalMoneda", SqlDbType.Decimal);
            pSubMon.Precision = 18;
            pSubMon.Scale = 2;
            pSubMon.Value = v.SubTotalMoneda;

            var pItbMon = cmd.Parameters.Add("@ItbisMoneda", SqlDbType.Decimal);
            pItbMon.Precision = 18;
            pItbMon.Scale = 2;
            pItbMon.Value = v.ItbisMoneda;

            var pTotMon = cmd.Parameters.Add("@TotalMoneda", SqlDbType.Decimal);
            pTotMon.Precision = 18;
            pTotMon.Scale = 2;
            pTotMon.Value = v.TotalMoneda;

            cmd.Parameters.Add("@TerminoPagoId", SqlDbType.Int).Value = terminoPagoId;

            if (string.IsNullOrWhiteSpace(v.POS_CajaNumero))
                cmd.Parameters.Add("@POS_CajaNumero", SqlDbType.NVarChar, 20).Value = DBNull.Value;
            else
                cmd.Parameters.Add("@POS_CajaNumero", SqlDbType.NVarChar, 20).Value = v.POS_CajaNumero.Trim();

            if (v.CajaId > 0)
                cmd.Parameters.Add("@CajaId", SqlDbType.Int).Value = v.CajaId;
            else
                cmd.Parameters.Add("@CajaId", SqlDbType.Int).Value = DBNull.Value;

            var obj = cmd.ExecuteScalar();
            return obj == null || obj == DBNull.Value ? 0L : Convert.ToInt64(obj);
        }
    }
}
