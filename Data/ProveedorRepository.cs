using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using Andloe.Entidad;

namespace Andloe.Data
{
    public class ProveedorRepository
    {
        public List<Proveedor> Listar(string? filtro = null, int top = 200)
        {
            var list = new List<Proveedor>();
            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
SELECT TOP(@top)
    [Código], [Nombre], [RNC], [Telefono], [Email], [Direccion],
    [Estado], [FechaCreacion],
    CreditoMaximo, CodDivisas, CodTerminoPagos, CodVendedor, CodAlmacen
FROM dbo.Proveedor
WHERE (@filtro IS NULL OR [Código] LIKE @like OR [Nombre] LIKE @like OR [RNC] LIKE @like)
ORDER BY [FechaCreacion] DESC, [Código] DESC;", cn);

            cmd.Parameters.Add("@top", SqlDbType.Int).Value = top;
            if (string.IsNullOrWhiteSpace(filtro))
            {
                cmd.Parameters.Add("@filtro", SqlDbType.NVarChar, 100).Value = DBNull.Value;
                cmd.Parameters.Add("@like", SqlDbType.NVarChar, 100).Value = DBNull.Value;
            }
            else
            {
                cmd.Parameters.Add("@filtro", SqlDbType.NVarChar, 100).Value = filtro;
                cmd.Parameters.Add("@like", SqlDbType.NVarChar, 100).Value = "%" + filtro + "%";
            }

            using var rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                list.Add(new Proveedor
                {
                    Codigo = rd.GetString(0),
                    Nombre = rd.GetString(1),
                    Rnc = rd.IsDBNull(2) ? null : rd.GetString(2),
                    Telefono = rd.IsDBNull(3) ? null : rd.GetString(3),
                    Email = rd.IsDBNull(4) ? null : rd.GetString(4),
                    Direccion = rd.IsDBNull(5) ? null : rd.GetString(5),
                    Estado = rd.IsDBNull(6) ? (byte)1 : rd.GetByte(6),
                    FechaCreacion = rd.IsDBNull(7) ? (DateTime?)null : rd.GetDateTime(7),
                    CreditoMaximo = rd.IsDBNull(8) ? (decimal?)null : rd.GetDecimal(8),
                    CodDivisas = rd.IsDBNull(9) ? null : rd.GetString(9),
                    CodTerminoPagos = rd.IsDBNull(10) ? null : rd.GetString(10),
                    CodVendedor = rd.IsDBNull(11) ? null : rd.GetString(11),
                    CodAlmacen = rd.IsDBNull(12) ? null : rd.GetString(12)
                });
            }
            return list;
        }

        public Proveedor? ObtenerPorCodigo(string codigo)
        {
            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
SELECT [Código], [Nombre], [RNC], [Telefono], [Email], [Direccion],
       [Estado], [FechaCreacion],
       CreditoMaximo, CodDivisas, CodTerminoPagos, CodVendedor, CodAlmacen
FROM dbo.Proveedor
WHERE [Código]=@c;", cn);
            cmd.Parameters.Add("@c", SqlDbType.VarChar, 20).Value = codigo;

            using var rd = cmd.ExecuteReader();
            if (!rd.Read()) return null;

            return new Proveedor
            {
                Codigo = rd.GetString(0),
                Nombre = rd.GetString(1),
                Rnc = rd.IsDBNull(2) ? null : rd.GetString(2),
                Telefono = rd.IsDBNull(3) ? null : rd.GetString(3),
                Email = rd.IsDBNull(4) ? null : rd.GetString(4),
                Direccion = rd.IsDBNull(5) ? null : rd.GetString(5),
                Estado = rd.IsDBNull(6) ? (byte)1 : rd.GetByte(6),
                FechaCreacion = rd.IsDBNull(7) ? (DateTime?)null : rd.GetDateTime(7),
                CreditoMaximo = rd.IsDBNull(8) ? (decimal?)null : rd.GetDecimal(8),
                CodDivisas = rd.IsDBNull(9) ? null : rd.GetString(9),
                CodTerminoPagos = rd.IsDBNull(10) ? null : rd.GetString(10),
                CodVendedor = rd.IsDBNull(11) ? null : rd.GetString(11),
                CodAlmacen = rd.IsDBNull(12) ? null : rd.GetString(12)
            };
        }

        // Usa sp_Proveedor_CrearAuto (con params extendidos)
        public string CrearAuto(
            string nombre, string? rnc = null, string? telefono = null, string? email = null, string? direccion = null,
            decimal? creditoMaximo = null, string? codDivisas = null, string? codTerminoPagos = null,
            string? codVendedor = null, string? codAlmacen = null)
        {
            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand("dbo.sp_Proveedor_CrearAuto", cn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.Add("@Nombre", SqlDbType.NVarChar, 120).Value = nombre;
            cmd.Parameters.Add("@RNC", SqlDbType.VarChar, 20).Value = (object?)rnc ?? DBNull.Value;
            cmd.Parameters.Add("@Telefono", SqlDbType.VarChar, 20).Value = (object?)telefono ?? DBNull.Value;
            cmd.Parameters.Add("@Email", SqlDbType.VarChar, 100).Value = (object?)email ?? DBNull.Value;
            cmd.Parameters.Add("@Direccion", SqlDbType.NVarChar, 200).Value = (object?)direccion ?? DBNull.Value;

            var pCm = cmd.Parameters.Add("@CreditoMaximo", SqlDbType.Decimal);
            pCm.Precision = 18; pCm.Scale = 2;
            pCm.Value = (object?)creditoMaximo ?? DBNull.Value;

            cmd.Parameters.Add("@CodDivisas", SqlDbType.VarChar, 10).Value = (object?)codDivisas ?? DBNull.Value;
            cmd.Parameters.Add("@CodTerminoPagos", SqlDbType.VarChar, 20).Value = (object?)codTerminoPagos ?? DBNull.Value;
            cmd.Parameters.Add("@CodVendedor", SqlDbType.VarChar, 20).Value = (object?)codVendedor ?? DBNull.Value;
            cmd.Parameters.Add("@CodAlmacen", SqlDbType.VarChar, 20).Value = (object?)codAlmacen ?? DBNull.Value;

            var pOut = cmd.Parameters.Add("@CodigoGenerado", SqlDbType.VarChar, 20);
            pOut.Direction = ParameterDirection.Output;

            cmd.ExecuteNonQuery();
            return (string)pOut.Value;
        }

        public void Actualizar(Proveedor p)
        {
            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
UPDATE dbo.Proveedor
   SET [Nombre]=@n, [RNC]=@r, [Telefono]=@t, [Email]=@e, [Direccion]=@d,
       [Estado]=@es,
       CreditoMaximo=@cm, CodDivisas=@div, CodTerminoPagos=@ter, CodVendedor=@ven, CodAlmacen=@alm
 WHERE [Código]=@c;", cn);

            cmd.Parameters.Add("@n", SqlDbType.NVarChar, 120).Value = p.Nombre;
            cmd.Parameters.Add("@r", SqlDbType.VarChar, 20).Value = (object?)p.Rnc ?? DBNull.Value;
            cmd.Parameters.Add("@t", SqlDbType.VarChar, 20).Value = (object?)p.Telefono ?? DBNull.Value;
            cmd.Parameters.Add("@e", SqlDbType.VarChar, 100).Value = (object?)p.Email ?? DBNull.Value;
            cmd.Parameters.Add("@d", SqlDbType.NVarChar, 200).Value = (object?)p.Direccion ?? DBNull.Value;
            cmd.Parameters.Add("@es", SqlDbType.TinyInt).Value = p.Estado;

            var pCm = cmd.Parameters.Add("@cm", SqlDbType.Decimal);
            pCm.Precision = 18; pCm.Scale = 2;
            pCm.Value = (object?)p.CreditoMaximo ?? DBNull.Value;

            cmd.Parameters.Add("@div", SqlDbType.VarChar, 10).Value = (object?)p.CodDivisas ?? DBNull.Value;
            cmd.Parameters.Add("@ter", SqlDbType.VarChar, 20).Value = (object?)p.CodTerminoPagos ?? DBNull.Value;
            cmd.Parameters.Add("@ven", SqlDbType.VarChar, 20).Value = (object?)p.CodVendedor ?? DBNull.Value;
            cmd.Parameters.Add("@alm", SqlDbType.VarChar, 20).Value = (object?)p.CodAlmacen ?? DBNull.Value;

            cmd.Parameters.Add("@c", SqlDbType.VarChar, 20).Value = p.Codigo;

            cmd.ExecuteNonQuery();
        }

        public void Eliminar(string codigo)
        {
            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"DELETE FROM dbo.Proveedor WHERE [Código]=@c;", cn);
            cmd.Parameters.Add("@c", SqlDbType.VarChar, 20).Value = codigo;
            cmd.ExecuteNonQuery();
        }
    }
}
