using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using Andloe.Entidad;

namespace Andloe.Data
{
    public class TerminoPagoRepository
    {
        public List<TerminoPago> ListarTodos(bool soloActivos = false)
        {
            var list = new List<TerminoPago>();

            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
SELECT
    TerminoPagoId,
    Codigo,
    Descripcion,
    DiasPlazo,
    TieneDescuento,
    PorcDescuento,
    DiasDescuento,
    Estado,
    FechaCreacion,
    FechaActualizacion,
    Usuario,
    CantCuotas,
    FrecuenciaDias,
    UnidadTiempo,
    CantidadTiempo,
    TextoECF,
    TipoPagoECF
FROM dbo.TerminoPago
WHERE (@soloActivos = 0 OR Estado = 1)
ORDER BY Codigo;", cn);

            cmd.Parameters.Add("@soloActivos", SqlDbType.Bit).Value = soloActivos;

            using var rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                list.Add(Map(rd));
            }

            return list;
        }

        public List<TerminoPago> ListarActivos()
        {
            return ListarTodos(true);
        }

        public TerminoPago? ObtenerPorId(int terminoPagoId)
        {
            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
SELECT
    TerminoPagoId,
    Codigo,
    Descripcion,
    DiasPlazo,
    TieneDescuento,
    PorcDescuento,
    DiasDescuento,
    Estado,
    FechaCreacion,
    FechaActualizacion,
    Usuario,
    CantCuotas,
    FrecuenciaDias,
    UnidadTiempo,
    CantidadTiempo,
    TextoECF,
    TipoPagoECF
FROM dbo.TerminoPago
WHERE TerminoPagoId = @id;", cn);

            cmd.Parameters.Add("@id", SqlDbType.Int).Value = terminoPagoId;

            using var rd = cmd.ExecuteReader();
            if (!rd.Read()) return null;

            return Map(rd);
        }

        public bool ExisteCodigo(string codigo, int? excluirId = null)
        {
            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
SELECT COUNT(1)
FROM dbo.TerminoPago
WHERE Codigo = @codigo
  AND (@excluirId IS NULL OR TerminoPagoId <> @excluirId);", cn);

            cmd.Parameters.Add("@codigo", SqlDbType.VarChar, 20).Value = codigo.Trim();
            cmd.Parameters.Add("@excluirId", SqlDbType.Int).Value = (object?)excluirId ?? DBNull.Value;

            return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
        }

        public int Insertar(TerminoPago t)
        {
            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
INSERT INTO dbo.TerminoPago
(
    Codigo,
    Descripcion,
    DiasPlazo,
    TieneDescuento,
    PorcDescuento,
    DiasDescuento,
    Estado,
    FechaCreacion,
    FechaActualizacion,
    Usuario,
    CantCuotas,
    FrecuenciaDias,
    UnidadTiempo,
    CantidadTiempo,
    TextoECF,
    TipoPagoECF
)
VALUES
(
    @Codigo,
    @Descripcion,
    @DiasPlazo,
    @TieneDescuento,
    @PorcDescuento,
    @DiasDescuento,
    @Estado,
    GETDATE(),
    GETDATE(),
    @Usuario,
    @CantCuotas,
    @FrecuenciaDias,
    @UnidadTiempo,
    @CantidadTiempo,
    @TextoECF,
    @TipoPagoECF
);

SELECT CAST(SCOPE_IDENTITY() AS INT);", cn);

            CargarParametros(cmd, t, incluirId: false);

            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public void Actualizar(TerminoPago t)
        {
            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
UPDATE dbo.TerminoPago
SET
    Codigo = @Codigo,
    Descripcion = @Descripcion,
    DiasPlazo = @DiasPlazo,
    TieneDescuento = @TieneDescuento,
    PorcDescuento = @PorcDescuento,
    DiasDescuento = @DiasDescuento,
    Estado = @Estado,
    FechaActualizacion = GETDATE(),
    Usuario = @Usuario,
    CantCuotas = @CantCuotas,
    FrecuenciaDias = @FrecuenciaDias,
    UnidadTiempo = @UnidadTiempo,
    CantidadTiempo = @CantidadTiempo,
    TextoECF = @TextoECF,
    TipoPagoECF = @TipoPagoECF
WHERE TerminoPagoId = @TerminoPagoId;", cn);

            CargarParametros(cmd, t, incluirId: true);
            cmd.ExecuteNonQuery();
        }

        public void CambiarEstado(int terminoPagoId, byte estado)
        {
            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
UPDATE dbo.TerminoPago
SET
    Estado = @Estado,
    FechaActualizacion = GETDATE()
WHERE TerminoPagoId = @Id;", cn);

            cmd.Parameters.Add("@Estado", SqlDbType.TinyInt).Value = estado;
            cmd.Parameters.Add("@Id", SqlDbType.Int).Value = terminoPagoId;

            cmd.ExecuteNonQuery();
        }

        private static void CargarParametros(SqlCommand cmd, TerminoPago t, bool incluirId)
        {
            if (incluirId)
                cmd.Parameters.Add("@TerminoPagoId", SqlDbType.Int).Value = t.TerminoPagoId;

            cmd.Parameters.Add("@Codigo", SqlDbType.VarChar, 20).Value = t.Codigo;
            cmd.Parameters.Add("@Descripcion", SqlDbType.VarChar, 150).Value = t.Descripcion;
            cmd.Parameters.Add("@DiasPlazo", SqlDbType.Int).Value = t.DiasPlazo;
            cmd.Parameters.Add("@TieneDescuento", SqlDbType.Bit).Value = t.TieneDescuento;

            var pPorc = cmd.Parameters.Add("@PorcDescuento", SqlDbType.Decimal);
            pPorc.Precision = 18;
            pPorc.Scale = 2;
            pPorc.Value = (object?)t.PorcDescuento ?? DBNull.Value;

            cmd.Parameters.Add("@DiasDescuento", SqlDbType.Int).Value = (object?)t.DiasDescuento ?? DBNull.Value;
            cmd.Parameters.Add("@Estado", SqlDbType.TinyInt).Value = t.Estado;
            cmd.Parameters.Add("@Usuario", SqlDbType.VarChar, 50).Value = (object?)t.Usuario ?? DBNull.Value;
            cmd.Parameters.Add("@CantCuotas", SqlDbType.Int).Value = (object?)t.CantCuotas ?? DBNull.Value;
            cmd.Parameters.Add("@FrecuenciaDias", SqlDbType.Int).Value = (object?)t.FrecuenciaDias ?? DBNull.Value;
            cmd.Parameters.Add("@UnidadTiempo", SqlDbType.VarChar, 20).Value = (object?)t.UnidadTiempo ?? DBNull.Value;
            cmd.Parameters.Add("@CantidadTiempo", SqlDbType.Int).Value = (object?)t.CantidadTiempo ?? DBNull.Value;
            cmd.Parameters.Add("@TextoECF", SqlDbType.VarChar, 100).Value = (object?)t.TextoECF ?? DBNull.Value;
            cmd.Parameters.Add("@TipoPagoECF", SqlDbType.Int).Value = (object?)t.TipoPagoECF ?? DBNull.Value;
        }

        private static TerminoPago Map(SqlDataReader rd)
        {
            return new TerminoPago
            {
                TerminoPagoId = rd.IsDBNull(0) ? 0 : Convert.ToInt32(rd.GetValue(0)),
                Codigo = rd.IsDBNull(1) ? "" : Convert.ToString(rd.GetValue(1)) ?? "",
                Descripcion = rd.IsDBNull(2) ? "" : Convert.ToString(rd.GetValue(2)) ?? "",
                DiasPlazo = rd.IsDBNull(3) ? 0 : Convert.ToInt32(rd.GetValue(3)),
                TieneDescuento = !rd.IsDBNull(4) && Convert.ToBoolean(rd.GetValue(4)),
                PorcDescuento = rd.IsDBNull(5) ? null : Convert.ToDecimal(rd.GetValue(5)),
                DiasDescuento = rd.IsDBNull(6) ? null : Convert.ToInt32(rd.GetValue(6)),
                Estado = rd.IsDBNull(7) ? (byte)1 : Convert.ToByte(rd.GetValue(7)),
                FechaCreacion = rd.IsDBNull(8) ? null : Convert.ToDateTime(rd.GetValue(8)),
                FechaActualizacion = rd.IsDBNull(9) ? null : Convert.ToDateTime(rd.GetValue(9)),
                Usuario = rd.IsDBNull(10) ? null : Convert.ToString(rd.GetValue(10)),
                CantCuotas = rd.IsDBNull(11) ? null : Convert.ToInt32(rd.GetValue(11)),
                FrecuenciaDias = rd.IsDBNull(12) ? null : Convert.ToInt32(rd.GetValue(12)),
                UnidadTiempo = rd.IsDBNull(13) ? null : Convert.ToString(rd.GetValue(13)),
                CantidadTiempo = rd.IsDBNull(14) ? null : Convert.ToInt32(rd.GetValue(14)),
                TextoECF = rd.IsDBNull(15) ? null : Convert.ToString(rd.GetValue(15)),
                TipoPagoECF = rd.IsDBNull(16) ? null : Convert.ToInt32(rd.GetValue(16))
            };
        }
    }
}