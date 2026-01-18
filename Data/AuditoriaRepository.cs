using Andloe.Entidad;
using Andloe.Data;
using Microsoft.Data.SqlClient;
using System;
using System.Data;

namespace Andloe.Data
{
    public class AuditoriaRepository
    {
        public void Insert(AuditoriaLog log)
        {
            using var cn = Db.GetOpenConnection();

            using var cmd = new SqlCommand(@"
INSERT INTO dbo.AuditoriaLog
(UsuarioId, Usuario, Modulo, Accion, Entidad, EntidadId, Detalle, AntesJson, DespuesJson, Maquina, Ip)
VALUES
(@UsuarioId, @Usuario, @Modulo, @Accion, @Entidad, @EntidadId, @Detalle, @AntesJson, @DespuesJson, @Maquina, @Ip);", cn);

            cmd.Parameters.Add("@UsuarioId", SqlDbType.Int).Value = (object?)log.UsuarioId ?? DBNull.Value;
            cmd.Parameters.Add("@Usuario", SqlDbType.VarChar, 50).Value = (object?)log.Usuario ?? DBNull.Value;

            cmd.Parameters.Add("@Modulo", SqlDbType.VarChar, 50).Value = log.Modulo ?? "";
            cmd.Parameters.Add("@Accion", SqlDbType.VarChar, 30).Value = log.Accion ?? "";

            cmd.Parameters.Add("@Entidad", SqlDbType.VarChar, 80).Value = (object?)log.Entidad ?? DBNull.Value;
            cmd.Parameters.Add("@EntidadId", SqlDbType.VarChar, 50).Value = (object?)log.EntidadId ?? DBNull.Value;

            cmd.Parameters.Add("@Detalle", SqlDbType.NVarChar, 400).Value = (object?)log.Detalle ?? DBNull.Value;

            cmd.Parameters.Add("@AntesJson", SqlDbType.NVarChar).Value = (object?)log.AntesJson ?? DBNull.Value;
            cmd.Parameters.Add("@DespuesJson", SqlDbType.NVarChar).Value = (object?)log.DespuesJson ?? DBNull.Value;

            cmd.Parameters.Add("@Maquina", SqlDbType.VarChar, 100).Value = (object?)log.Maquina ?? DBNull.Value;
            cmd.Parameters.Add("@Ip", SqlDbType.VarChar, 45).Value = (object?)log.Ip ?? DBNull.Value;

            cmd.ExecuteNonQuery();
        }
    }
}
