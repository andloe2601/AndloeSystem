using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace Andloe.Data
{
    public sealed class ReporteMantenimientoRepository
    {
        public DataTable ListarActividades(string modulo)
        {
            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand("dbo.Reporte_ListarActividades", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@pModulo", modulo));

            using var da = new SqlDataAdapter(cmd);
            var dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        public DataTable ListarDef(string modulo, string actividad)
        {
            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand("dbo.ReporteDef_Listar", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@pModulo", modulo));
            cmd.Parameters.Add(new SqlParameter("@pActividad", actividad));

            using var da = new SqlDataAdapter(cmd);
            var dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        public DataTable ListarAsignaciones(string modulo, string actividad, int empresaId, int? sucursalId, int? usuarioId)
        {
            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand("dbo.ReporteAsignacion_Listar", cn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add(new SqlParameter("@pModulo", modulo));
            cmd.Parameters.Add(new SqlParameter("@pActividad", actividad));
            cmd.Parameters.Add(new SqlParameter("@pEmpresaId", empresaId));
            cmd.Parameters.Add(new SqlParameter("@pSucursalId", (object?)sucursalId ?? DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@pUsuarioId", (object?)usuarioId ?? DBNull.Value));

            using var da = new SqlDataAdapter(cmd);
            var dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        public void UpsertAsignacion(int empresaId, int? sucursalId, int? usuarioId,
            string modulo, string actividad, int reporteId,
            bool esActivo, int orden, bool esDefault, int prioridad)
        {
            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand("dbo.ReporteAsignacion_Upsert", cn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add(new SqlParameter("@pEmpresaId", empresaId));
            cmd.Parameters.Add(new SqlParameter("@pSucursalId", (object?)sucursalId ?? DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@pUsuarioId", (object?)usuarioId ?? DBNull.Value));

            cmd.Parameters.Add(new SqlParameter("@pModulo", modulo));
            cmd.Parameters.Add(new SqlParameter("@pActividad", actividad));
            cmd.Parameters.Add(new SqlParameter("@pReporteId", reporteId));

            cmd.Parameters.Add(new SqlParameter("@pEsActivo", esActivo));
            cmd.Parameters.Add(new SqlParameter("@pOrden", orden));
            cmd.Parameters.Add(new SqlParameter("@pEsDefault", esDefault));
            cmd.Parameters.Add(new SqlParameter("@pPrioridad", prioridad));

            cmd.ExecuteNonQuery();
        }
    }
}
