using Andloe.Data;
using Andloe.Entidad;
using Microsoft.Data.SqlClient;
using System;
using System.Data;

namespace Andloe.Logica
{
    public class cajaCierreService
    {
        private readonly CierreCajaRepository _repo = new();
        private readonly FondoCajaRepository _fondoRepo = new();

        public List<CierrePagoPosDetalleDto> ListarPagosPosPorCierre(long cierreId)
            => _repo.ListarPagosPosPorCierre(cierreId);

        public CierreCajaResumen CalcularResumen(string cajaNumero, DateTime desde, DateTime hasta)
        {
            return _repo.CalcularResumen(cajaNumero, desde, hasta);
        }
        public long CerrarTurno(
            DateTime fechaDesde,
            DateTime fechaHasta,
            int cajaId,
            string cajaNumero,
            string usuarioCierre,
            decimal? conteoEfectivoBase,
            string? observacion)
        {
            using var cn = Db.GetOpenConnection();

            using var cmd = new SqlCommand("sp_Caja_CerrarTurno", cn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.Add("@FechaDesde", SqlDbType.DateTime2).Value = fechaDesde;
            cmd.Parameters.Add("@FechaHasta", SqlDbType.DateTime2).Value = fechaHasta;
            cmd.Parameters.Add("@CajaId", SqlDbType.Int).Value = cajaId;
            cmd.Parameters.Add("@CajaNumero", SqlDbType.NVarChar, 10).Value = cajaNumero;
            cmd.Parameters.Add("@UsuarioCierre", SqlDbType.NVarChar, 100).Value = usuarioCierre;

            if (conteoEfectivoBase.HasValue)
                cmd.Parameters.Add("@ConteoEfectivoBase", SqlDbType.Decimal).Value = conteoEfectivoBase.Value;
            else
                cmd.Parameters.Add("@ConteoEfectivoBase", SqlDbType.Decimal).Value = DBNull.Value;

            cmd.Parameters.Add("@Observacion", SqlDbType.NVarChar, 200).Value =
                (object?)observacion ?? DBNull.Value;

            var pOut = cmd.Parameters.Add("@CierreId", SqlDbType.BigInt);
            pOut.Direction = ParameterDirection.Output;

            cmd.ExecuteNonQuery();

            return Convert.ToInt64(pOut.Value);
        }

        public void ActualizarPagosPosConCierreId(long cierreId, string cajaNumero, DateTime desde, DateTime hasta)
        {
            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
UPDATE dbo.POS_Pago
SET 
    CierreId = @CierreId,
    IncluidoEnCierre = 1
WHERE 
    POS_CajaNumero = @CajaNumero
    AND Fecha >= @Desde
    AND Fecha <= @Hasta
    AND IncluidoEnCierre = 0;", cn);

            cmd.Parameters.Add("@CierreId", SqlDbType.BigInt).Value = cierreId;
            cmd.Parameters.Add("@CajaNumero", SqlDbType.NVarChar, 10).Value = cajaNumero;
            cmd.Parameters.Add("@Desde", SqlDbType.DateTime).Value = desde;
            cmd.Parameters.Add("@Hasta", SqlDbType.DateTime).Value = hasta;

            cmd.ExecuteNonQuery();
        }
    

    public long GuardarCierre(
    int cajaId,
    string cajaNumero,
    DateTime desde,
    DateTime hasta,
    decimal fondoInicial,
    decimal efectivoDeclarado,
    string usuarioCierre)
        {
            var resumen = _repo.CalcularResumen(cajaNumero, desde, hasta);
            var ahora = DateTime.Now;

            var cierre = new CierreCaja
            {
                CajaId = cajaId,
                POS_CajaNumero = cajaNumero,
                FechaDesde = desde,
                FechaHasta = hasta,
                FondoInicial = fondoInicial,
                TotalVentas = resumen.TotalVentas,
                TotalPagos = resumen.TotalPagos,
                EfectivoTeorico = resumen.EfectivoTeorico + fondoInicial,
                EfectivoDeclarado = efectivoDeclarado,
                Diferencia = efectivoDeclarado - (resumen.EfectivoTeorico + fondoInicial),
                UsuarioCierre = usuarioCierre,
                FechaCierre = ahora,
                Estado = "CERRADO"
            };

            long cierreId = _repo.InsertarCierre(cierre);

            // 🔥 ACTUALIZAR VENTAS
            _repo.ActualizarVentasConCierreId(
                cierreId,
                cajaNumero,
                desde,
                hasta
            );

            // 🔥 ACTUALIZAR PAGOS POS
            _repo.ActualizarPagosPosConCierreId(
                cierreId,
                cajaNumero,
                desde,
                hasta
            );

            // 🔥 Cerrar fondos de caja
            _fondoRepo.CerrarFondosAbiertos(cajaId);

            return cierreId;
        }
    }
}




