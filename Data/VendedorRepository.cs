using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using Andloe.Entidad;

namespace Andloe.Data
{
    public class VendedorRepository
    {
        public List<Vendedor> Listar(string? filtro = null, int top = 200, bool incluirInactivos = true)
        {
            var list = new List<Vendedor>();
            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
SELECT TOP(@top)
    VendedorId, Codigo, Nombre, Email, Telefono, Estado
FROM dbo.Vendedor
WHERE (@soloActivos = 0 OR Estado = 1)
  AND (@filtro IS NULL OR Codigo LIKE @like OR Nombre LIKE @like)
ORDER BY Nombre;", cn);

            cmd.Parameters.Add("@top", SqlDbType.Int).Value = top;
            cmd.Parameters.Add("@soloActivos", SqlDbType.Bit).Value = incluirInactivos ? 0 : 1;

            if (string.IsNullOrWhiteSpace(filtro))
            {
                cmd.Parameters.Add("@filtro", SqlDbType.NVarChar, 100).Value = DBNull.Value;
                cmd.Parameters.Add("@like", SqlDbType.NVarChar, 100).Value = DBNull.Value;
            }
            else
            {
                cmd.Parameters.Add("@filtro", SqlDbType.NVarChar, 100).Value = filtro.Trim();
                cmd.Parameters.Add("@like", SqlDbType.NVarChar, 100).Value = "%" + filtro.Trim() + "%";
            }

            using var rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                list.Add(new Vendedor
                {
                    VendedorId = rd.IsDBNull(0) ? 0 : rd.GetInt32(0),
                    Codigo = rd.IsDBNull(1) ? "" : rd.GetString(1),
                    Nombre = rd.IsDBNull(2) ? "" : rd.GetString(2),
                    Email = rd.IsDBNull(3) ? null : rd.GetString(3),
                    Telefono = rd.IsDBNull(4) ? null : rd.GetString(4),
                    Estado = !rd.IsDBNull(5) && rd.GetBoolean(5)
                });
            }

            return list;
        }

        public Vendedor? ObtenerPorCodigo(string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo)) return null;

            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
SELECT TOP(1)
    VendedorId, Codigo, Nombre, Email, Telefono, Estado
FROM dbo.Vendedor
WHERE Codigo = @c;", cn);

            cmd.Parameters.Add("@c", SqlDbType.VarChar, 20).Value = codigo.Trim();

            using var rd = cmd.ExecuteReader();
            if (!rd.Read()) return null;

            return new Vendedor
            {
                VendedorId = rd.IsDBNull(0) ? 0 : rd.GetInt32(0),
                Codigo = rd.IsDBNull(1) ? "" : rd.GetString(1),
                Nombre = rd.IsDBNull(2) ? "" : rd.GetString(2),
                Email = rd.IsDBNull(3) ? null : rd.GetString(3),
                Telefono = rd.IsDBNull(4) ? null : rd.GetString(4),
                Estado = !rd.IsDBNull(5) && rd.GetBoolean(5)
            };
        }

        public void Crear(Vendedor v)
        {
            if (v == null) throw new ArgumentNullException(nameof(v));
            v.Codigo = (v.Codigo ?? "").Trim();
            v.Nombre = (v.Nombre ?? "").Trim();

            if (string.IsNullOrWhiteSpace(v.Codigo))
                throw new Exception("Código requerido.");
            if (string.IsNullOrWhiteSpace(v.Nombre))
                throw new Exception("Nombre requerido.");

            using var cn = Db.GetOpenConnection();

            // Validar duplicado
            using (var cmdDup = new SqlCommand("SELECT COUNT(1) FROM dbo.Vendedor WHERE Codigo=@c;", cn))
            {
                cmdDup.Parameters.Add("@c", SqlDbType.VarChar, 20).Value = v.Codigo;
                var n = Convert.ToInt32(cmdDup.ExecuteScalar() ?? 0);
                if (n > 0) throw new Exception($"Ya existe un vendedor con Código '{v.Codigo}'.");
            }

            using var cmd = new SqlCommand(@"
INSERT dbo.Vendedor (Codigo, Nombre, Email, Telefono, Estado)
VALUES (@Codigo, @Nombre, @Email, @Telefono, @Estado);", cn);

            cmd.Parameters.Add("@Codigo", SqlDbType.VarChar, 20).Value = v.Codigo;
            cmd.Parameters.Add("@Nombre", SqlDbType.NVarChar, 120).Value = v.Nombre;
            cmd.Parameters.Add("@Email", SqlDbType.VarChar, 100).Value = (object?)v.Email ?? DBNull.Value;
            cmd.Parameters.Add("@Telefono", SqlDbType.VarChar, 20).Value = (object?)v.Telefono ?? DBNull.Value;
            cmd.Parameters.Add("@Estado", SqlDbType.Bit).Value = v.Estado;

            cmd.ExecuteNonQuery();
        }

        public void Actualizar(Vendedor v)
        {
            if (v == null) throw new ArgumentNullException(nameof(v));
            v.Codigo = (v.Codigo ?? "").Trim();
            v.Nombre = (v.Nombre ?? "").Trim();

            if (string.IsNullOrWhiteSpace(v.Codigo))
                throw new Exception("Código requerido.");
            if (string.IsNullOrWhiteSpace(v.Nombre))
                throw new Exception("Nombre requerido.");

            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
UPDATE dbo.Vendedor
SET Nombre = @Nombre,
    Email = @Email,
    Telefono = @Telefono,
    Estado = @Estado
WHERE Codigo = @Codigo;", cn);

            cmd.Parameters.Add("@Codigo", SqlDbType.VarChar, 20).Value = v.Codigo;
            cmd.Parameters.Add("@Nombre", SqlDbType.NVarChar, 120).Value = v.Nombre;
            cmd.Parameters.Add("@Email", SqlDbType.VarChar, 100).Value = (object?)v.Email ?? DBNull.Value;
            cmd.Parameters.Add("@Telefono", SqlDbType.VarChar, 20).Value = (object?)v.Telefono ?? DBNull.Value;
            cmd.Parameters.Add("@Estado", SqlDbType.Bit).Value = v.Estado;

            var rows = cmd.ExecuteNonQuery();
            if (rows <= 0) throw new Exception("No se actualizó (Código no encontrado).");
        }

        // Importante: por el modelo actual (Cliente/Proveedor usan CodVendedor),
        // este "Eliminar" se implementa como INACTIVAR (Estado=0) para no romper histórico.
        public void Eliminar(string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo))
                throw new Exception("Código requerido.");

            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
UPDATE dbo.Vendedor
SET Estado = 0
WHERE Codigo = @c;", cn);

            cmd.Parameters.Add("@c", SqlDbType.VarChar, 20).Value = codigo.Trim();
            cmd.ExecuteNonQuery();
        }
    }
}
