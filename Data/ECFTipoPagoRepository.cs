using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using Andloe.Entidad;

namespace Andloe.Data
{
    public class ECFTipoPagoRepository
    {
        public List<ECFTipoPago> ListarActivos()
        {
            var list = new List<ECFTipoPago>();

            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
SELECT TipoPagoECFId, CodigoDGII, Descripcion, Activo
FROM dbo.ECFTipoPago
WHERE Activo = 1
ORDER BY TipoPagoECFId;", cn);

            using var rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                list.Add(new ECFTipoPago
                {
                    TipoPagoECFId = rd.GetInt32(0),
                    CodigoDGII = rd.GetString(1),
                    Descripcion = rd.GetString(2),
                    Activo = rd.GetBoolean(3)
                });
            }

            return list;
        }
    }
}