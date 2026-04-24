using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using Andloe.Entidad;
using Entidad;

namespace Andloe.Data
{
    public class PosPagoRepository
    {
        public void Insert(PagoPOS pago, SqlTransaction tx)
        {
            using var cmd = new SqlCommand(@"
INSERT INTO dbo.POS_Pago
(
    Fecha,
    MonedaCodigo,
    TasaCambio,
    Monto,
    MontoBase,
    FormaPagoCodigo,
    Referencia,
    Entidad,
    Observacion,
    Usuario,
    CajaId,
    VentaId,
    Estado,
    POS_CajaNumero,
    CierreId,
    IncluidoEnCierre
)
VALUES
(
    @Fecha,
    @MonedaCodigo,
    @TasaCambio,
    @Monto,
    @MontoBase,
    @FormaPagoCodigo,
    @Referencia,
    @Entidad,
    @Observacion,
    @Usuario,
    @CajaId,
    @VentaId,
    @Estado,
    @POS_CajaNumero,
    NULL,
    0
);", tx.Connection, tx);

            cmd.Parameters.Add("@Fecha", SqlDbType.DateTime).Value = pago.Fecha;
            cmd.Parameters.Add("@MonedaCodigo", SqlDbType.VarChar, 3).Value =
                string.IsNullOrWhiteSpace(pago.MonedaCodigo) ? "DOP" : pago.MonedaCodigo;

            var pTc = cmd.Parameters.Add("@TasaCambio", SqlDbType.Decimal);
            pTc.Precision = 18;
            pTc.Scale = 6;
            pTc.Value = pago.TasaCambio <= 0 ? 1m : pago.TasaCambio;

            var pMonto = cmd.Parameters.Add("@Monto", SqlDbType.Decimal);
            pMonto.Precision = 18;
            pMonto.Scale = 2;
            pMonto.Value = pago.Monto;

            var pMontoBase = cmd.Parameters.Add("@MontoBase", SqlDbType.Decimal);
            pMontoBase.Precision = 18;
            pMontoBase.Scale = 2;
            pMontoBase.Value = pago.MontoBase;

            cmd.Parameters.Add("@FormaPagoCodigo", SqlDbType.VarChar, 2).Value = pago.FormaPagoCodigo;
            cmd.Parameters.Add("@Referencia", SqlDbType.VarChar, 60).Value = pago.Referencia ?? "";
            cmd.Parameters.Add("@Entidad", SqlDbType.VarChar, 80).Value = pago.Entidad ?? "";
            cmd.Parameters.Add("@Observacion", SqlDbType.VarChar, 200).Value = pago.Observacion ?? "";
            cmd.Parameters.Add("@Usuario", SqlDbType.VarChar, 50).Value = pago.Usuario ?? "";
            cmd.Parameters.Add("@CajaId", SqlDbType.Int).Value = pago.CajaId;
            cmd.Parameters.Add("@VentaId", SqlDbType.BigInt).Value = pago.VentaId;
            cmd.Parameters.Add("@Estado", SqlDbType.VarChar, 12).Value =
                string.IsNullOrWhiteSpace(pago.Estado) ? "APLICADO" : pago.Estado;

            if (string.IsNullOrWhiteSpace(pago.POS_CajaNumero))
                cmd.Parameters.Add("@POS_CajaNumero", SqlDbType.NVarChar, 20).Value = DBNull.Value;
            else
                cmd.Parameters.Add("@POS_CajaNumero", SqlDbType.NVarChar, 20).Value = pago.POS_CajaNumero.Trim();

            cmd.ExecuteNonQuery();
        }

        public List<MedioPagoDto> ListarMediosPago()
        {
            var list = new List<MedioPagoDto>();

            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
SELECT FormaPagoCodigo, Descripcion
FROM dbo.ECFFormaPagoCatalogo
WHERE Activo = 1
ORDER BY TRY_CONVERT(INT, FormaPagoCodigo);", cn);

            using var rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                list.Add(new MedioPagoDto
                {
                    MedioPagoId = 0,
                    Nombre = rd.GetString(1)
                });
            }

            return list;
        }
    }
}