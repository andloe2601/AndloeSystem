using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using Andloe.Entidad;

namespace Andloe.Data
{
    public class AlmacenRepository
    {
        public List<Almacen> ListarActivos()
        {
            var lista = new List<Almacen>();

            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
SELECT AlmacenId, SucursalId, Codigo, Nombre, Estado, EmpresaId
FROM dbo.Almacen
WHERE Estado = 1
ORDER BY Nombre;", cn);

            using var rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                lista.Add(new Almacen
                {
                    AlmacenId = rd.GetInt32(0),
                    SucursalId = rd.GetInt32(1),
                    Codigo = rd.GetString(2),
                    Nombre = rd.GetString(3),
                    Estado = rd.GetBoolean(4),
                    EmpresaId = rd.GetInt32(5)
                });
            }

            return lista;
        }

        public string? ObtenerNombrePorCodigo(string codigo)
        {
            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
SELECT TOP 1 Nombre
FROM dbo.Almacen
WHERE Codigo = @codigo;", cn);

            cmd.Parameters.Add("@codigo", SqlDbType.VarChar, 20).Value = codigo;

            var val = cmd.ExecuteScalar();
            if (val == null || val == DBNull.Value) return null;

            return Convert.ToString(val);
        }
    }
}