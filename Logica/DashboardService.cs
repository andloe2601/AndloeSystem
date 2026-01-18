using System.Data;
using Andloe.Data;

namespace Andloe.Logica;

public class DashboardService
{
    public (int Total, int Activos, int Bloqueados, DateTime? UltimoAcceso) GetUsuariosResumen()
    {
        // funciona con la vista; si no existiera, cae al SELECT directo
        try
        {
            var dt = SqlHelper.ExecuteDataTable("SELECT * FROM dbo.vw_Dashboard_Usuarios;");
            if (dt.Rows.Count == 0) return (0, 0, 0, null);
            var r = dt.Rows[0];
            return (Convert.ToInt32(r["Total"]),
                    Convert.ToInt32(r["Activos"]),
                    Convert.ToInt32(r["Bloqueados"]),
                    r["UltimoAcceso"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(r["UltimoAcceso"]));
        }
        catch
        {
            var total = SqlHelper.ExecuteScalar<int>("SELECT COUNT(*) FROM dbo.Usuario;");
            var activos = SqlHelper.ExecuteScalar<int>("SELECT COUNT(*) FROM dbo.Usuario WHERE Estado = N'Activo';");
            var bloqueados = SqlHelper.ExecuteScalar<int>("SELECT COUNT(*) FROM dbo.Usuario WHERE Estado = N'Bloqueado';");
            var ultimo = SqlHelper.ExecuteScalar<DateTime?>("SELECT MAX(UltimoAcceso) FROM dbo.Usuario;");
            return (total, activos, bloqueados, ultimo);
        }
    }

    public DataTable GetUsuariosPorRol()
        => SqlHelper.ExecuteDataTable("SELECT Rol, Cantidad FROM dbo.vw_Dashboard_UsuariosPorRol ORDER BY Cantidad DESC;");

    // Placeholders para POS (ya funcionan aunque devuelvan ceros)
    public DataTable GetVentas7Dias()
        => SqlHelper.ExecuteDataTable("SELECT Fecha, Monto FROM dbo.vw_Dashboard_Ventas7Dias ORDER BY Fecha;");

    public DataTable GetVentasPorCategoria()
        => SqlHelper.ExecuteDataTable("SELECT Categoria, Monto FROM dbo.vw_Dashboard_VentasPorCategoria ORDER BY Monto DESC;");
}
