using System;
using System.Collections.Generic;
using System.Data;
using Andloe.Entidad;
using Microsoft.Data.SqlClient;

namespace Andloe.Data
{
    public class TerminoPagoRepository
    {
        private const string SQL_BASE = @"
SELECT
    TerminoPagoId,
    Codigo,
    Descripcion,
    DiasPlazo,
    CantCuotas,
    FrecuenciaDias,
    TieneDescuento,
    PorcDescuento,
    DiasDescuento,
    Estado,
    FechaCreacion,
    FechaActualizacion,
    Usuario
FROM dbo.TerminoPago ";

        public List<TerminoPago> ListarTodos()
        {
            var lista = new List<TerminoPago>();
            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(SQL_BASE + " ORDER BY Codigo;", cn);

            using var dr = cmd.ExecuteReader();
            while (dr.Read()) lista.Add(Map(dr));
            return lista;
        }

        public List<TerminoPago> ListarActivos()
        {
            var lista = new List<TerminoPago>();
            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(SQL_BASE + " WHERE Estado = 1 ORDER BY Codigo;", cn);

            using var dr = cmd.ExecuteReader();
            while (dr.Read()) lista.Add(Map(dr));
            return lista;
        }

        public TerminoPago? ObtenerPorId(int id)
        {
            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(SQL_BASE + " WHERE TerminoPagoId = @Id;", cn);
            cmd.Parameters.Add("@Id", SqlDbType.Int).Value = id;

            using var dr = cmd.ExecuteReader();
            if (!dr.Read()) return null;
            return Map(dr);
        }

        public bool ExisteCodigo(string codigo, int? excluirId = null)
        {
            using var cn = Db.GetOpenConnection();

            var sql = @"
SELECT COUNT(1)
FROM dbo.TerminoPago
WHERE Codigo = @Codigo " + (excluirId.HasValue ? "AND TerminoPagoId <> @Id" : "") + ";";

            using var cmd = new SqlCommand(sql, cn);
            cmd.Parameters.Add("@Codigo", SqlDbType.VarChar, 20).Value = codigo.Trim();
            if (excluirId.HasValue) cmd.Parameters.Add("@Id", SqlDbType.Int).Value = excluirId.Value;

            var obj = cmd.ExecuteScalar();
            var count = (obj == null || obj == DBNull.Value) ? 0 : Convert.ToInt32(obj);
            return count > 0;
        }

        public int Insertar(TerminoPago t)
        {
            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
INSERT INTO dbo.TerminoPago
(
    Codigo, Descripcion, DiasPlazo, CantCuotas, FrecuenciaDias,
    TieneDescuento, PorcDescuento, DiasDescuento,
    Estado, FechaCreacion, Usuario
)
VALUES
(
    @Codigo, @Descripcion, @DiasPlazo, @CantCuotas, @FrecuenciaDias,
    @TieneDescuento, @PorcDescuento, @DiasDescuento,
    @Estado, GETDATE(), @Usuario
);
SELECT CAST(SCOPE_IDENTITY() AS INT);", cn);

            cmd.Parameters.Add("@Codigo", SqlDbType.VarChar, 20).Value = t.Codigo.Trim();
            cmd.Parameters.Add("@Descripcion", SqlDbType.VarChar, 100).Value = t.Descripcion.Trim();
            cmd.Parameters.Add("@DiasPlazo", SqlDbType.Int).Value = t.DiasPlazo;

            cmd.Parameters.Add("@CantCuotas", SqlDbType.Int).Value = (object?)t.CantCuotas ?? DBNull.Value;
            cmd.Parameters.Add("@FrecuenciaDias", SqlDbType.Int).Value = (object?)t.FrecuenciaDias ?? DBNull.Value;

            cmd.Parameters.Add("@TieneDescuento", SqlDbType.Bit).Value = t.TieneDescuento;
            cmd.Parameters.Add("@PorcDescuento", SqlDbType.Decimal).Value =
                (t.TieneDescuento && t.PorcDescuento.HasValue) ? t.PorcDescuento.Value : (object)DBNull.Value;
            cmd.Parameters.Add("@DiasDescuento", SqlDbType.Int).Value =
                (t.TieneDescuento && t.DiasDescuento.HasValue) ? t.DiasDescuento.Value : (object)DBNull.Value;

            cmd.Parameters.Add("@Estado", SqlDbType.Bit).Value = t.Estado;
            cmd.Parameters.Add("@Usuario", SqlDbType.VarChar, 30).Value = (object?)t.Usuario ?? DBNull.Value;

            var obj = cmd.ExecuteScalar();
            return (obj == null || obj == DBNull.Value) ? 0 : Convert.ToInt32(obj);
        }

        public void Actualizar(TerminoPago t)
        {
            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
UPDATE dbo.TerminoPago
SET
    Codigo            = @Codigo,
    Descripcion       = @Descripcion,
    DiasPlazo         = @DiasPlazo,
    CantCuotas        = @CantCuotas,
    FrecuenciaDias    = @FrecuenciaDias,
    TieneDescuento    = @TieneDescuento,
    PorcDescuento     = @PorcDescuento,
    DiasDescuento     = @DiasDescuento,
    Estado            = @Estado,
    FechaActualizacion = GETDATE(),
    Usuario           = @Usuario
WHERE TerminoPagoId = @Id;", cn);

            cmd.Parameters.Add("@Id", SqlDbType.Int).Value = t.TerminoPagoId;
            cmd.Parameters.Add("@Codigo", SqlDbType.VarChar, 20).Value = t.Codigo.Trim();
            cmd.Parameters.Add("@Descripcion", SqlDbType.VarChar, 100).Value = t.Descripcion.Trim();
            cmd.Parameters.Add("@DiasPlazo", SqlDbType.Int).Value = t.DiasPlazo;

            cmd.Parameters.Add("@CantCuotas", SqlDbType.Int).Value = (object?)t.CantCuotas ?? DBNull.Value;
            cmd.Parameters.Add("@FrecuenciaDias", SqlDbType.Int).Value = (object?)t.FrecuenciaDias ?? DBNull.Value;

            cmd.Parameters.Add("@TieneDescuento", SqlDbType.Bit).Value = t.TieneDescuento;
            cmd.Parameters.Add("@PorcDescuento", SqlDbType.Decimal).Value =
                (t.TieneDescuento && t.PorcDescuento.HasValue) ? t.PorcDescuento.Value : (object)DBNull.Value;
            cmd.Parameters.Add("@DiasDescuento", SqlDbType.Int).Value =
                (t.TieneDescuento && t.DiasDescuento.HasValue) ? t.DiasDescuento.Value : (object)DBNull.Value;

            cmd.Parameters.Add("@Estado", SqlDbType.Bit).Value = t.Estado;
            cmd.Parameters.Add("@Usuario", SqlDbType.VarChar, 30).Value = (object?)t.Usuario ?? DBNull.Value;

            cmd.ExecuteNonQuery();
        }

        public void CambiarEstado(int terminoPagoId, bool activo)
        {
            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
UPDATE dbo.TerminoPago
SET Estado = @Estado,
    FechaActualizacion = GETDATE()
WHERE TerminoPagoId = @Id;", cn);

            cmd.Parameters.Add("@Id", SqlDbType.Int).Value = terminoPagoId;
            cmd.Parameters.Add("@Estado", SqlDbType.Bit).Value = activo;
            cmd.ExecuteNonQuery();
        }

        private static TerminoPago Map(SqlDataReader dr)
        {
            int O(string n) => dr.GetOrdinal(n);

            return new TerminoPago
            {
                TerminoPagoId = dr.GetInt32(O("TerminoPagoId")),
                Codigo = dr.GetString(O("Codigo")),
                Descripcion = dr.GetString(O("Descripcion")),
                DiasPlazo = dr.GetInt32(O("DiasPlazo")),

                CantCuotas = dr.IsDBNull(O("CantCuotas")) ? null : dr.GetInt32(O("CantCuotas")),
                FrecuenciaDias = dr.IsDBNull(O("FrecuenciaDias")) ? null : dr.GetInt32(O("FrecuenciaDias")),

                TieneDescuento = dr.GetBoolean(O("TieneDescuento")),
                PorcDescuento = dr.IsDBNull(O("PorcDescuento")) ? null : dr.GetDecimal(O("PorcDescuento")),
                DiasDescuento = dr.IsDBNull(O("DiasDescuento")) ? null : dr.GetInt32(O("DiasDescuento")),

                Estado = dr.GetBoolean(O("Estado")),
                FechaCreacion = dr.GetDateTime(O("FechaCreacion")),
                FechaActualizacion = dr.IsDBNull(O("FechaActualizacion")) ? null : dr.GetDateTime(O("FechaActualizacion")),
                Usuario = dr["Usuario"] as string
            };
        }
    }
}
