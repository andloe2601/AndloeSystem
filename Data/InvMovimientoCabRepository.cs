using System;
using System.Data;
using Microsoft.Data.SqlClient;
using Andloe.Entidad;

namespace Andloe.Data
{
    public class InvMovimientoCabRepository
    {
        public long InsertCabecera(InvMovimientoCab cab, SqlTransaction tx)
        {
            if (cab == null) throw new ArgumentNullException(nameof(cab));
            if (tx == null) throw new ArgumentNullException(nameof(tx));

            var cn = (SqlConnection)tx.Connection!;

            using var cmd = new SqlCommand(@"
INSERT INTO dbo.InvMovimientoCab
(
    Fecha,
    Tipo,
    Origen,
    OrigenId,
    AlmacenIdOrigen,
    AlmacenIdDestino,
    Usuario,
    Observacion,
    Estado,
    FechaCreacion,
    UsuarioCreacion
)
OUTPUT INSERTED.InvMovId
VALUES
(
    @Fecha,
    @Tipo,
    @Origen,
    @OrigenId,
    @AlmacenIdOrigen,
    @AlmacenIdDestino,
    @Usuario,
    @Observacion,
    @Estado,
    @FechaCreacion,
    @UsuarioCreacion
);", cn, tx);

            // ====== PARAMS (UNA SOLA VEZ CADA UNO) ======
            cmd.Parameters.Add("@Fecha", SqlDbType.DateTime).Value = cab.Fecha;

            cmd.Parameters.Add("@Tipo", SqlDbType.VarChar, 10).Value =
                (object?)cab.Tipo ?? DBNull.Value;

            cmd.Parameters.Add("@Origen", SqlDbType.VarChar, 20).Value =
                (object?)cab.Origen ?? DBNull.Value;

            cmd.Parameters.Add("@OrigenId", SqlDbType.BigInt).Value =
                cab.OrigenId.HasValue ? cab.OrigenId.Value : (object)DBNull.Value;

            cmd.Parameters.Add("@AlmacenIdOrigen", SqlDbType.Int).Value = cab.AlmacenIdOrigen;

            cmd.Parameters.Add("@AlmacenIdDestino", SqlDbType.Int).Value =
                cab.AlmacenIdDestino.HasValue ? cab.AlmacenIdDestino.Value : (object)DBNull.Value;

            // ⚠️ IMPORTANTE: SOLO UNA VEZ @Usuario
            cmd.Parameters.Add("@Usuario", SqlDbType.VarChar, 30).Value =
                (object?)cab.Usuario ?? DBNull.Value;

            cmd.Parameters.Add("@Observacion", SqlDbType.NVarChar, 200).Value =
                (object?)cab.Observacion ?? "";

            cmd.Parameters.Add("@Estado", SqlDbType.VarChar, 20).Value =
                (object?)cab.Estado ?? "APLICADO";

            // ✅ Estos eran los que faltaban
            cmd.Parameters.Add("@FechaCreacion", SqlDbType.DateTime).Value = DateTime.Now;

            cmd.Parameters.Add("@UsuarioCreacion", SqlDbType.VarChar, 30).Value =
                (object?)cab.Usuario ?? "SYSTEM";

            var id = cmd.ExecuteScalar();
            return Convert.ToInt64(id);
        }
    }
}
