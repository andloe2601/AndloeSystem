using Microsoft.Data.SqlClient;
using System;
using System.Data;

namespace Andloe.Data
{
    public class MovMedioPagoRepository
    {
        public void InsertDesdePOS(
            SqlTransaction tx,
            long ventaId,
            int medioPagoId,
            decimal monto,
            string usuario,
            int cajaId,
            string? cajaNumero)
        {
            if (tx == null)
                throw new ArgumentNullException(nameof(tx));

            var cn = tx.Connection ?? throw new InvalidOperationException("Transacción sin conexión.");

            using var cmd = new SqlCommand(@"
INSERT INTO dbo.MovMedioPago
    (Fecha,
     VentaId,
     MedioPagoId,
     Monto,
     Usuario,
     CajaId,
     POS_CajaNumero,
     Estado)
VALUES
    (@Fecha,
     @VentaId,
     @MedioPagoId,
     @Monto,
     @Usuario,
     @CajaId,
     @POS_CajaNumero,
     @Estado);", cn, tx);

            cmd.Parameters.Add("@Fecha", SqlDbType.DateTime).Value = DateTime.Now;
            cmd.Parameters.Add("@VentaId", SqlDbType.BigInt).Value = ventaId;
            cmd.Parameters.Add("@MedioPagoId", SqlDbType.Int).Value = medioPagoId;

            var pMonto = cmd.Parameters.Add("@Monto", SqlDbType.Decimal);
            pMonto.Precision = 18;
            pMonto.Scale = 2;
            pMonto.Value = monto;

            cmd.Parameters.Add("@Usuario", SqlDbType.VarChar, 30).Value = usuario;
            cmd.Parameters.Add("@CajaId", SqlDbType.Int).Value = cajaId;

            if (string.IsNullOrWhiteSpace(cajaNumero))
                cmd.Parameters.Add("@POS_CajaNumero", SqlDbType.NVarChar, 10).Value = DBNull.Value;
            else
                cmd.Parameters.Add("@POS_CajaNumero", SqlDbType.NVarChar, 10).Value = cajaNumero.Trim();

            cmd.Parameters.Add("@Estado", SqlDbType.VarChar, 20).Value = "APLICADO";

            cmd.ExecuteNonQuery();
        }
    }
}
