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
        // ✅ Inserta el pago en POS_Pago
        //    Ahora también guarda POS_CajaNumero
        //    y deja CierreId = 0, IncluidoEnCierre = 0
        public void Insert(PagoPOS pago, SqlTransaction tx)
        {
            using var cmd = new SqlCommand(@"
                INSERT INTO dbo.POS_Pago
                (
                    Fecha,
                    MonedaCodigo,
                    TasaCambio,
                    Monto,
                    MedioPagoId,
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
                    @MedioPagoId,
                    @Referencia,
                    @Entidad,
                    @Observacion,
                    @Usuario,
                    @CajaId,
                    @VentaId,
                    @Estado,
                    @POS_CajaNumero,
                    0,     -- CierreId (sin cierre aún)
                    0      -- IncluidoEnCierre = FALSE
                );",
                tx.Connection, tx);

            cmd.Parameters.Add("@Fecha", SqlDbType.DateTime).Value = pago.Fecha;
            cmd.Parameters.Add("@MonedaCodigo", SqlDbType.VarChar, 3).Value = pago.MonedaCodigo;

            var pTc = cmd.Parameters.Add("@TasaCambio", SqlDbType.Decimal);
            pTc.Precision = 18;
            pTc.Scale = 6;
            pTc.Value = pago.TasaCambio;

            var pMonto = cmd.Parameters.Add("@Monto", SqlDbType.Decimal);
            pMonto.Precision = 18;
            pMonto.Scale = 2;
            pMonto.Value = pago.Monto;

            cmd.Parameters.Add("@MedioPagoId", SqlDbType.Int).Value = pago.MedioPagoId;
            cmd.Parameters.Add("@Referencia", SqlDbType.VarChar, 50).Value = pago.Referencia ?? "";
            cmd.Parameters.Add("@Entidad", SqlDbType.VarChar, 80).Value = pago.Entidad ?? "";
            cmd.Parameters.Add("@Observacion", SqlDbType.VarChar, 200).Value = pago.Observacion ?? "";
            cmd.Parameters.Add("@Usuario", SqlDbType.VarChar, 30).Value = pago.Usuario ?? "";
            cmd.Parameters.Add("@CajaId", SqlDbType.Int).Value = pago.CajaId;
            cmd.Parameters.Add("@VentaId", SqlDbType.BigInt).Value = pago.VentaId;
            cmd.Parameters.Add("@Estado", SqlDbType.VarChar, 20).Value = pago.Estado ?? "APLICADO";

            // 👉 IMPORTANTE: número de caja POS
            cmd.Parameters.Add("@POS_CajaNumero", SqlDbType.NVarChar, 10).Value =
                (pago.POS_CajaNumero ?? "").Trim();

            cmd.ExecuteNonQuery();
        }

        // ✅ Listado de medios de pago para combos / configuración
        public List<MedioPagoDto> ListarMediosPago()
        {
            var list = new List<MedioPagoDto>();

            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
                SELECT MedioPagoId, Nombre
                FROM dbo.MedioPago
                WHERE Estado = 1
                ORDER BY Nombre;", cn);

            using var rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                list.Add(new MedioPagoDto
                {
                    MedioPagoId = rd.GetInt32(0),
                    Nombre = rd.GetString(1)
                });
            }

            return list;
        }
    }
}
