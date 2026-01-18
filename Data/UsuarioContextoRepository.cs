using System;
using System.Data;
using Microsoft.Data.SqlClient;
using Andloe.Entidad;

namespace Andloe.Data
{
    public class UsuarioContextoRepository : IUsuarioContextoRepository
    {
        public UsuarioContextoDto ObtenerPorUsuario(string usuario)
        {
            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
SELECT TOP 1 u.UsuarioId
FROM dbo.Usuario u
WHERE u.Usuario = @u;", cn);

            cmd.Parameters.Add("@u", SqlDbType.NVarChar, 50).Value = usuario;

            var idObj = cmd.ExecuteScalar();
            if (idObj == null) throw new Exception("Usuario no encontrado.");

            return ObtenerPorUsuarioId(Convert.ToInt32(idObj));
        }

        public UsuarioContextoDto ObtenerPorUsuarioId(int usuarioId)
        {
            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
SELECT TOP 1
    UsuarioId,
    EmpresaId,
    SucursalId,
    AlmacenId
FROM dbo.UsuarioContexto
WHERE UsuarioId = @u;", cn);

            cmd.Parameters.Add("@u", SqlDbType.Int).Value = usuarioId;

            using var rd = cmd.ExecuteReader();
            if (!rd.Read())
                throw new Exception("El usuario no tiene contexto asignado (UsuarioContexto).");

            return new UsuarioContextoDto
            {
                UsuarioId = rd.GetInt32(0),
                EmpresaId = rd.GetInt32(1),
                SucursalId = rd.GetInt32(2),
                AlmacenId = rd.GetInt32(3)
            };
        }

        // ✅ Método que te faltaba en el repo (para el error: no contiene ObtenerContexto)
        public UsuarioContextoDto ObtenerContexto(int usuarioId) => ObtenerPorUsuarioId(usuarioId);

        public bool UsuarioTieneAccesoAEmpresa(int usuarioId, int empresaId)
        {
            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
SELECT 1
FROM dbo.UsuarioEmpresa
WHERE UsuarioId = @u AND EmpresaId = @e AND Activo = 1;", cn);

            cmd.Parameters.Add("@u", SqlDbType.Int).Value = usuarioId;
            cmd.Parameters.Add("@e", SqlDbType.Int).Value = empresaId;

            return cmd.ExecuteScalar() != null;
        }

        public void UpsertContextoDefault(int usuarioId, int empresaId)
        {
            using var cn = Db.GetOpenConnection();

            // 1) Sucursal default (primera activa)
            int sucursalId;
            using (var cmdSuc = new SqlCommand(@"
SELECT TOP 1 SucursalId
FROM dbo.Sucursal
WHERE EmpresaId = @e AND Estado = 1
ORDER BY SucursalId;", cn))
            {
                cmdSuc.Parameters.Add("@e", SqlDbType.Int).Value = empresaId;
                var sObj = cmdSuc.ExecuteScalar();
                if (sObj == null) throw new Exception("No hay sucursales activas para esta empresa.");
                sucursalId = Convert.ToInt32(sObj);
            }

            // 2) Almacén default (primero activo para la sucursal)
            int almacenId;
            using (var cmdAlm = new SqlCommand(@"
SELECT TOP 1 AlmacenId
FROM dbo.Almacen
WHERE EmpresaId = @e AND SucursalId = @s AND Estado = 1
ORDER BY AlmacenId;", cn))
            {
                cmdAlm.Parameters.Add("@e", SqlDbType.Int).Value = empresaId;
                cmdAlm.Parameters.Add("@s", SqlDbType.Int).Value = sucursalId;
                var aObj = cmdAlm.ExecuteScalar();
                if (aObj == null) throw new Exception("No hay almacenes activos para esta sucursal.");
                almacenId = Convert.ToInt32(aObj);
            }

            // 3) Upsert en UsuarioContexto
            using var cmd = new SqlCommand(@"
IF EXISTS (SELECT 1 FROM dbo.UsuarioContexto WHERE UsuarioId = @u)
BEGIN
    UPDATE dbo.UsuarioContexto
       SET EmpresaId = @e,
           SucursalId = @s,
           AlmacenId = @a,
           FechaCambio = SYSDATETIME(),
           Host = HOST_NAME(),
           Ip = NULL
     WHERE UsuarioId = @u;
END
ELSE
BEGIN
    INSERT INTO dbo.UsuarioContexto (UsuarioId, EmpresaId, SucursalId, AlmacenId, FechaCambio, Host, Ip)
    VALUES (@u, @e, @s, @a, SYSDATETIME(), HOST_NAME(), NULL);
END;", cn);

            cmd.Parameters.Add("@u", SqlDbType.Int).Value = usuarioId;
            cmd.Parameters.Add("@e", SqlDbType.Int).Value = empresaId;
            cmd.Parameters.Add("@s", SqlDbType.Int).Value = sucursalId;
            cmd.Parameters.Add("@a", SqlDbType.Int).Value = almacenId;

            cmd.ExecuteNonQuery();
        }
    }
}
