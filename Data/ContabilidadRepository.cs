using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace Andloe.Data
{
    public sealed class ContabilidadRepository
    {
        public (long MovimientoId, string NoAsiento) AsentarVentaPOS(
            long ventaId,
            int cajaId,
            string usuario,
            SqlConnection cn,
            SqlTransaction tx)
        {
            if (ventaId <= 0) throw new ArgumentException("ventaId inválido.");
            if (cajaId <= 0) throw new ArgumentException("cajaId inválido.");
            if (string.IsNullOrWhiteSpace(usuario)) usuario = "SYSTEM";

            // 1) Leer totales + moneda + tasa + documento
            string noDoc;
            DateTime fecha;
            string moneda;
            decimal tasa;
            decimal subTotal;
            decimal itbis;
            decimal total;
            int terminoPagoId;

            using (var cmd = new SqlCommand(@"
SELECT 
    NoDocumento, Fecha, MonedaCodigo, ISNULL(TasaCambio,1),
    ISNULL(SubTotalMoneda,0), ISNULL(ItbisMoneda,0), ISNULL(TotalMoneda,0),
    TerminoPagoId
FROM dbo.VentaCab
WHERE VentaId=@id;", cn, tx))
            {
                cmd.Parameters.Add("@id", SqlDbType.BigInt).Value = ventaId;

                using var rd = cmd.ExecuteReader();
                if (!rd.Read())
                    throw new InvalidOperationException("No existe la venta para asentar.");

                noDoc = rd.GetString(0);
                fecha = rd.GetDateTime(1);
                moneda = rd.IsDBNull(2) ? "DOP" : rd.GetString(2);
                tasa = rd.IsDBNull(3) ? 1m : rd.GetDecimal(3);
                subTotal = rd.GetDecimal(4);
                itbis = rd.GetDecimal(5);
                total = rd.GetDecimal(6);
                terminoPagoId = rd.GetInt32(7);
            }

            // 2) Resolver cuentas por Caja/Moneda (CajaContaConfig)
            int? ctaCaja = null, ctaCliente = null, ctaIngreso = null, ctaItbis = null;

            using (var cmd = new SqlCommand(@"
SELECT CtaCajaId, CtaClienteId, CtaIngresoId, CtaITBISId
FROM dbo.CajaContaConfig
WHERE CajaId=@caja AND MonedaCodigo=@moneda;", cn, tx))
            {
                cmd.Parameters.Add("@caja", SqlDbType.Int).Value = cajaId;
                cmd.Parameters.Add("@moneda", SqlDbType.VarChar, 3).Value = moneda;

                using var rd = cmd.ExecuteReader();
                if (rd.Read())
                {
                    ctaCaja = rd.IsDBNull(0) ? null : rd.GetInt32(0);
                    ctaCliente = rd.IsDBNull(1) ? null : rd.GetInt32(1);
                    ctaIngreso = rd.IsDBNull(2) ? null : rd.GetInt32(2);
                    ctaItbis = rd.IsDBNull(3) ? null : rd.GetInt32(3);
                }
            }

            if (!ctaIngreso.HasValue) throw new InvalidOperationException("Falta CtaIngresoId en CajaContaConfig.");
            if (!ctaItbis.HasValue) throw new InvalidOperationException("Falta CtaITBISId en CajaContaConfig.");
            if (!ctaCaja.HasValue) throw new InvalidOperationException("Falta CtaCajaId en CajaContaConfig.");
            if (!ctaCliente.HasValue) throw new InvalidOperationException("Falta CtaClienteId en CajaContaConfig.");

            // 3) Determinar si es contado o crédito (regla práctica)
            bool esContado = true;
            using (var cmd = new SqlCommand(@"
SELECT DiasPlazo, CantCuotas
FROM dbo.TerminoPago
WHERE TerminoPagoId=@id;", cn, tx))
            {
                cmd.Parameters.Add("@id", SqlDbType.Int).Value = terminoPagoId;
                using var rd = cmd.ExecuteReader();
                if (rd.Read())
                {
                    int dias = rd.GetInt32(0);
                    int? cuotas = rd.IsDBNull(1) ? null : rd.GetInt32(1);

                    // Si hay plazo > 0 o cuotas > 1 => crédito
                    esContado = !(dias > 0 || (cuotas.HasValue && cuotas.Value > 1));
                }
            }

            int cuentaDebe = esContado ? ctaCaja.Value : ctaCliente.Value;

            // 4) Crear cabecera contable (SP)
            long movId;
            string noAsiento;

            using (var cmd = new SqlCommand("dbo.sp_Conta_Mov_Crear", cn, tx))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Fecha", fecha);
                cmd.Parameters.AddWithValue("@Descripcion", $"Venta POS {noDoc}");
                cmd.Parameters.AddWithValue("@Origen", "VENTA");
                cmd.Parameters.AddWithValue("@OrigenId", ventaId);
                cmd.Parameters.AddWithValue("@Usuario", usuario);
                cmd.Parameters.AddWithValue("@MonedaCodigo", moneda);
                cmd.Parameters.AddWithValue("@TasaCambio", tasa <= 0 ? 1m : tasa);

                var pMov = new SqlParameter("@MovimientoId", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
                var pNo = new SqlParameter("@NoAsiento", SqlDbType.VarChar, 30) { Direction = ParameterDirection.Output };
                cmd.Parameters.Add(pMov);
                cmd.Parameters.Add(pNo);

                cmd.ExecuteNonQuery();

                movId = (long)pMov.Value;
                noAsiento = (string)pNo.Value;
            }

            // 5) Líneas (SP)
            AddLinea(movId, cuentaDebe, $"Debe venta {noDoc}", total, 0, cn, tx);
            AddLinea(movId, ctaIngreso.Value, $"Ingreso venta {noDoc}", 0, subTotal, cn, tx);
            if (itbis != 0) AddLinea(movId, ctaItbis.Value, $"ITBIS venta {noDoc}", 0, itbis, cn, tx);

            // 6) Cerrar/contabilizar
            using (var cmd = new SqlCommand("dbo.sp_Conta_Mov_Cerrar", cn, tx))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@MovimientoId", movId);
                cmd.ExecuteNonQuery();
            }

            // 7) Amarrar el movimiento a la venta
            using (var cmd = new SqlCommand(@"
UPDATE dbo.VentaCab
SET MovimientoIdConta=@mov
WHERE VentaId=@id;", cn, tx))
            {
                cmd.Parameters.Add("@mov", SqlDbType.BigInt).Value = movId;
                cmd.Parameters.Add("@id", SqlDbType.BigInt).Value = ventaId;
                cmd.ExecuteNonQuery();
            }

            return (movId, noAsiento);
        }

        private static void AddLinea(long movId, int cuentaId, string desc, decimal debito, decimal credito, SqlConnection cn, SqlTransaction tx)
        {
            using var cmd = new SqlCommand("dbo.sp_Conta_Mov_AddLinea", cn, tx);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@MovimientoId", movId);
            cmd.Parameters.AddWithValue("@CuentaId", cuentaId);
            cmd.Parameters.AddWithValue("@Descripcion", desc);
            cmd.Parameters.AddWithValue("@DebitoMoneda", debito);
            cmd.Parameters.AddWithValue("@CreditoMoneda", credito);

            cmd.ExecuteNonQuery();
        }
    }
}
