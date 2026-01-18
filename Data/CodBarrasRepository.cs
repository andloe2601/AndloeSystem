using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using Andloe.Entidad;

namespace Andloe.Data
{
    public class CodBarrasRepository
    {
        public List<CodBarra> ListarPorProducto(string codigoProducto)
        {
            var list = new List<CodBarra>();
            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
SELECT [Cód_ barras], [Nº producto], [Tipo], [Usuario], [Ultima fecha utilización]
FROM dbo.CodBarras
WHERE [Nº producto]=@p;", cn);
            cmd.Parameters.Add("@p", SqlDbType.VarChar, 20).Value = codigoProducto;

            using var rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                list.Add(new CodBarra
                {
                    CodigoBarras = rd.GetString(0),
                    NoProducto = rd.GetString(1),
                    Tipo = rd.IsDBNull(2) ? 0 : rd.GetInt32(2),
                    Usuario = rd.IsDBNull(3) ? "" : rd.GetString(3),
                    UltimaFechaUtilizacion = rd.IsDBNull(4) ? (System.DateTime?)null : rd.GetDateTime(4)
                });
            }
            return list;
        }

        public string CrearAuto(string codigoProducto, string usuario = "system")
        {
            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand("dbo.sp_CodBarras_CrearAuto", cn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.Add("@NoProducto", SqlDbType.VarChar, 20).Value = codigoProducto;
            cmd.Parameters.Add("@Usuario", SqlDbType.VarChar, 30).Value = usuario;

            var pOut = cmd.Parameters.Add("@CodigoBarras", SqlDbType.VarChar, 22);
            pOut.Direction = ParameterDirection.Output;

            cmd.ExecuteNonQuery();
            return (string)pOut.Value;
        }

        public void Eliminar(string codigoBarras)
        {
            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"DELETE FROM dbo.CodBarras WHERE [Cód_ barras]=@cb;", cn);
            cmd.Parameters.Add("@cb", SqlDbType.VarChar, 22).Value = codigoBarras;
            cmd.ExecuteNonQuery();
        }
    }
}
