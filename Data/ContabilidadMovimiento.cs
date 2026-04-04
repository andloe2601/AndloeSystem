#nullable enable
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace Andloe.Data
{
    public sealed class ContabilidadMovimientoRepository
    {
        public sealed class MovLineaRow
        {
            public int Linea { get; set; }
            public int CuentaId { get; set; }
            public string Codigo { get; set; } = "";
            public string CuentaNombre { get; set; } = "";
            public string Descripcion { get; set; } = "";
            public decimal DebitoMoneda { get; set; }
            public decimal CreditoMoneda { get; set; }
            public decimal DebitoBase { get; set; }
            public decimal CreditoBase { get; set; }
        }

        public (long MovimientoId, string NoAsiento) Crear(
            DateTime? fecha,
            string descripcion,
            string origen,
            long origenId,
            string usuario,
            string monedaCodigo,
            decimal? tasaCambio)
        {
            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand("dbo.sp_Conta_Mov_Crear_wrap", cn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@Fecha", SqlDbType.DateTime2).Value = (object?)fecha ?? DBNull.Value;
            cmd.Parameters.Add("@Descripcion", SqlDbType.NVarChar, 150).Value = descripcion.Trim();
            cmd.Parameters.Add("@Origen", SqlDbType.VarChar, 20).Value = origen.Trim();
            cmd.Parameters.Add("@OrigenId", SqlDbType.BigInt).Value = origenId;
            cmd.Parameters.Add("@Usuario", SqlDbType.NVarChar, 60).Value = usuario.Trim();
            cmd.Parameters.Add("@MonedaCodigo", SqlDbType.VarChar, 3).Value = monedaCodigo.Trim();
            cmd.Parameters.Add("@TasaCambio", SqlDbType.Decimal).Value = (object?)tasaCambio ?? DBNull.Value;

            var pMovId = cmd.Parameters.Add("@MovimientoId", SqlDbType.BigInt);
            pMovId.Direction = ParameterDirection.Output;

            var pNo = cmd.Parameters.Add("@NoAsiento", SqlDbType.VarChar, 30);
            pNo.Direction = ParameterDirection.Output;

            cmd.ExecuteNonQuery();

            var movId = (pMovId.Value == DBNull.Value) ? 0L : Convert.ToInt64(pMovId.Value);
            var no = (pNo.Value == DBNull.Value) ? "" : Convert.ToString(pNo.Value) ?? "";

            return (movId, no);
        }

        public void AddLineaPorCodigo(long movimientoId, string cuentaCodigo, string descripcion, decimal debitoMoneda, decimal creditoMoneda)
        {
            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand("dbo.sp_Conta_Mov_AddLineaPorCodigo", cn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@MovimientoId", SqlDbType.BigInt).Value = movimientoId;
            cmd.Parameters.Add("@CuentaCodigo", SqlDbType.VarChar, 15).Value = cuentaCodigo.Trim();
            cmd.Parameters.Add("@Descripcion", SqlDbType.NVarChar, 300).Value = string.IsNullOrWhiteSpace(descripcion) ? DBNull.Value : descripcion.Trim();
            cmd.Parameters.Add("@DebitoMoneda", SqlDbType.Decimal).Value = debitoMoneda;
            cmd.Parameters.Add("@CreditoMoneda", SqlDbType.Decimal).Value = creditoMoneda;

            cmd.ExecuteNonQuery();
        }

        public void Cerrar(long movimientoId)
        {
            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand("dbo.sp_Conta_Mov_Cerrar", cn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@MovimientoId", SqlDbType.BigInt).Value = movimientoId;
            cmd.ExecuteNonQuery();
        }

        public (decimal DebMon, decimal CredMon, decimal DebBase, decimal CredBase) GetTotales(long movimientoId)
        {
            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand("dbo.sp_Conta_Mov_Totales", cn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@MovimientoId", SqlDbType.BigInt).Value = movimientoId;

            using var rd = cmd.ExecuteReader();
            if (!rd.Read()) return (0m, 0m, 0m, 0m);

            return (
                rd.IsDBNull(0) ? 0m : rd.GetDecimal(0),
                rd.IsDBNull(1) ? 0m : rd.GetDecimal(1),
                rd.IsDBNull(2) ? 0m : rd.GetDecimal(2),
                rd.IsDBNull(3) ? 0m : rd.GetDecimal(3)
            );
        }

        public List<MovLineaRow> ListarLineas(long movimientoId)
        {
            using var cn = Db.GetOpenConnection();

            // ✅ basado en tus tablas reales: ContabilidadMovimientoLin y ContabilidadCatalogoCuenta(Nombre)
            using var cmd = new SqlCommand(@"
SELECT
    l.Linea,
    l.CuentaId,
    c.Codigo,
    c.Nombre AS CuentaNombre,
    l.Descripcion,
    l.DebitoMoneda,
    l.CreditoMoneda,
    l.DebitoBase,
    l.CreditoBase
FROM dbo.ContabilidadMovimientoLin l WITH (NOLOCK)
JOIN dbo.ContabilidadCatalogoCuenta c WITH (NOLOCK) ON c.CuentaId = l.CuentaId
WHERE l.MovimientoId = @MovimientoId
ORDER BY l.Linea;", cn);

            cmd.Parameters.Add("@MovimientoId", SqlDbType.BigInt).Value = movimientoId;

            using var rd = cmd.ExecuteReader();
            var list = new List<MovLineaRow>();
            while (rd.Read())
            {
                list.Add(new MovLineaRow
                {
                    Linea = rd.GetInt32(0),
                    CuentaId = rd.GetInt32(1),
                    Codigo = rd.IsDBNull(2) ? "" : rd.GetString(2),
                    CuentaNombre = rd.IsDBNull(3) ? "" : rd.GetString(3),
                    Descripcion = rd.IsDBNull(4) ? "" : rd.GetString(4),
                    DebitoMoneda = rd.IsDBNull(5) ? 0m : rd.GetDecimal(5),
                    CreditoMoneda = rd.IsDBNull(6) ? 0m : rd.GetDecimal(6),
                    DebitoBase = rd.IsDBNull(7) ? 0m : rd.GetDecimal(7),
                    CreditoBase = rd.IsDBNull(8) ? 0m : rd.GetDecimal(8)
                });
            }

            return list;
        }
    }
}
#nullable restore
