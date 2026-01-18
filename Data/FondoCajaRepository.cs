using Andloe.Entidad;
using Microsoft.Data.SqlClient;
using System;
using System.Data;

namespace Andloe.Data
{
    public class FondoCajaRepository
    {
        /// <summary>
        /// Devuelve el fondo ABIERTO para una caja y día (si existe).
        /// </summary>
        public FondoCaja? ObtenerFondoAbierto(int cajaId, DateTime fecha)
        {
            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
SELECT TOP(1)
    FondoId,
    CajaId,
    POS_CajaNumero,
    FechaApertura,
    UsuarioApertura,
    MontoFondo,
    Observacion,
    Estado,
    CierreId,
    FechaCierre
FROM dbo.POS_FondoCaja
WHERE CajaId = @CajaId
  AND CONVERT(date, FechaApertura) = @Fecha
  AND Estado = 'ABIERTO'
ORDER BY FechaApertura DESC;", cn);

            cmd.Parameters.Add("@CajaId", SqlDbType.Int).Value = cajaId;
            cmd.Parameters.Add("@Fecha", SqlDbType.Date).Value = fecha.Date;

            using var dr = cmd.ExecuteReader();
            if (!dr.Read())
                return null;

            return new FondoCaja
            {
                FondoId = dr.GetInt64(dr.GetOrdinal("FondoId")),
                CajaId = dr.GetInt32(dr.GetOrdinal("CajaId")),
                POS_CajaNumero = dr.GetString(dr.GetOrdinal("POS_CajaNumero")),
                FechaApertura = dr.GetDateTime(dr.GetOrdinal("FechaApertura")),
                UsuarioApertura = dr.GetString(dr.GetOrdinal("UsuarioApertura")),
                MontoFondo = dr.GetDecimal(dr.GetOrdinal("MontoFondo")),
                Observacion = dr.IsDBNull(dr.GetOrdinal("Observacion"))
                    ? null
                    : dr.GetString(dr.GetOrdinal("Observacion")),
                Estado = dr.GetString(dr.GetOrdinal("Estado")),
                CierreId = dr.IsDBNull(dr.GetOrdinal("CierreId"))
                    ? (long?)null
                    : dr.GetInt64(dr.GetOrdinal("CierreId")),
                FechaCierre = dr.IsDBNull(dr.GetOrdinal("FechaCierre"))
                    ? (DateTime?)null
                    : dr.GetDateTime(dr.GetOrdinal("FechaCierre"))
            };
        }

        /// <summary>
        /// Inserta un nuevo fondo en POS_FondoCaja y devuelve el ID.
        /// </summary>
        public long InsertarFondo(FondoCaja f)
        {
            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
INSERT INTO dbo.POS_FondoCaja
(
    CajaId,
    POS_CajaNumero,
    FechaApertura,
    UsuarioApertura,
    MontoFondo,
    Observacion,
    Estado,
    CierreId,
    FechaCierre
)
VALUES
(
    @CajaId,
    @POS_CajaNumero,
    @FechaApertura,
    @UsuarioApertura,
    @MontoFondo,
    @Observacion,
    @Estado,
    NULL,
    NULL
);

SELECT CAST(SCOPE_IDENTITY() AS BIGINT);", cn);

            cmd.Parameters.Add("@CajaId", SqlDbType.Int).Value = f.CajaId;
            cmd.Parameters.Add("@POS_CajaNumero", SqlDbType.NVarChar, 10).Value = f.POS_CajaNumero;
            cmd.Parameters.Add("@FechaApertura", SqlDbType.DateTime).Value = f.FechaApertura;
            cmd.Parameters.Add("@UsuarioApertura", SqlDbType.VarChar, 50).Value = f.UsuarioApertura;

            var pMonto = cmd.Parameters.Add("@MontoFondo", SqlDbType.Decimal);
            pMonto.Precision = 18; pMonto.Scale = 2; pMonto.Value = f.MontoFondo;

            cmd.Parameters.Add("@Observacion", SqlDbType.VarChar, 200)
               .Value = (object?)f.Observacion ?? DBNull.Value;

            cmd.Parameters.Add("@Estado", SqlDbType.VarChar, 12).Value = f.Estado ?? "ABIERTO";

            var obj = cmd.ExecuteScalar();
            return (obj == null || obj == DBNull.Value) ? 0L : Convert.ToInt64(obj);
        }

        /// <summary>
        /// Cierra todos los fondos ABIERTO de una caja/cajaNumero (normalmente 1) para este cierre.
        /// </summary>
        public void CerrarFondosPorCierre(
            SqlConnection cn,
            SqlTransaction tx,
            int cajaId,
            string posCajaNumero,
            long cierreId,
            DateTime fechaCierre)
        {
            using var cmd = new SqlCommand(@"
UPDATE dbo.POS_FondoCaja
SET Estado = 'CERRADO',
    FechaCierre = @FechaCierre,
    CierreId = @CierreId
WHERE CajaId = @CajaId
  AND POS_CajaNumero = @CajaNumero
  AND Estado = 'ABIERTO';", cn, tx);

            cmd.Parameters.Add("@CajaId", SqlDbType.Int).Value = cajaId;
            cmd.Parameters.Add("@CajaNumero", SqlDbType.NVarChar, 10).Value = posCajaNumero;
            cmd.Parameters.Add("@FechaCierre", SqlDbType.DateTime).Value = fechaCierre;
            cmd.Parameters.Add("@CierreId", SqlDbType.BigInt).Value = cierreId;

            cmd.ExecuteNonQuery();
        }

        public void CerrarFondosAbiertos(int cajaId)
        {
            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
UPDATE dbo.POS_FondoCaja
SET Estado = 'CERRADO'
WHERE CajaId = @CajaId
  AND Estado = 'ABIERTO';", cn);

            cmd.Parameters.Add("@CajaId", SqlDbType.Int).Value = cajaId;
            cmd.ExecuteNonQuery();
        }
    }
}

