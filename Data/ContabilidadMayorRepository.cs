#nullable enable
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace Andloe.Data
{
    public sealed class ContabilidadMayorRepository
    {
        public sealed class MayorRow
        {
            public int CuentaId { get; set; }
            public string Codigo { get; set; } = "";
            public string Nombre { get; set; } = "";
            public decimal DebitoMoneda { get; set; }
            public decimal CreditoMoneda { get; set; }
            public decimal DebitoBase { get; set; }
            public decimal CreditoBase { get; set; }
        }

        public List<MayorRow> ListarTotalesPorCuenta(DateTime desde, DateTime hasta, int? cuentaId = null)
        {
            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand("dbo.sp_Conta_Mayor_TotalesPorCuenta", cn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@Desde", SqlDbType.Date).Value = desde.Date;
            cmd.Parameters.Add("@Hasta", SqlDbType.Date).Value = hasta.Date;
            cmd.Parameters.Add("@CuentaId", SqlDbType.Int).Value = cuentaId.HasValue ? cuentaId.Value : DBNull.Value;

            using var rd = cmd.ExecuteReader();
            var list = new List<MayorRow>();

            while (rd.Read())
            {
                list.Add(new MayorRow
                {
                    CuentaId = rd.GetInt32(0),
                    Codigo = rd.IsDBNull(1) ? "" : rd.GetString(1),
                    Nombre = rd.IsDBNull(2) ? "" : rd.GetString(2),
                    DebitoMoneda = rd.IsDBNull(3) ? 0m : rd.GetDecimal(3),
                    CreditoMoneda = rd.IsDBNull(4) ? 0m : rd.GetDecimal(4),
                    DebitoBase = rd.IsDBNull(5) ? 0m : rd.GetDecimal(5),
                    CreditoBase = rd.IsDBNull(6) ? 0m : rd.GetDecimal(6)
                });
            }

            return list;
        }
    }
}
#nullable restore
