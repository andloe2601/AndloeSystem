using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using Andloe.Entidad;

namespace Andloe.Data
{
    public class PosTipoComprobanteConfigRepository
    {
        public List<PosTipoComprobanteConfigDto> ListarActivos(int cajaId, int? sucursalId)
        {
            var lista = new List<PosTipoComprobanteConfigDto>();

            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
SELECT
    PosTipoComprobanteConfigId,
    CajaId,
    SucursalId,
    TipoECFId,
    CodigoInterno,
    NombreMostrar,
    Activo,
    EsDefault,
    Orden,
    RequiereCliente,
    RequiereDocumentoCliente,
    PermiteEnPOS
FROM dbo.POS_TipoComprobanteConfig
WHERE Activo = 1
  AND PermiteEnPOS = 1
  AND (CajaId IS NULL OR CajaId = @CajaId)
  AND (SucursalId IS NULL OR SucursalId = @SucursalId)
ORDER BY
    CASE WHEN EsDefault = 1 THEN 0 ELSE 1 END,
    Orden,
    NombreMostrar;", cn);

            cmd.Parameters.Add("@CajaId", SqlDbType.Int).Value = cajaId;
            cmd.Parameters.Add("@SucursalId", SqlDbType.Int).Value = (object?)sucursalId ?? DBNull.Value;

            using var rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                lista.Add(new PosTipoComprobanteConfigDto
                {
                    PosTipoComprobanteConfigId = rd.GetInt32(0),
                    CajaId = rd.IsDBNull(1) ? null : rd.GetInt32(1),
                    SucursalId = rd.IsDBNull(2) ? null : rd.GetInt32(2),
                    TipoECFId = rd.GetInt32(3),
                    CodigoInterno = rd.GetString(4),
                    NombreMostrar = rd.GetString(5),
                    Activo = rd.GetBoolean(6),
                    EsDefault = rd.GetBoolean(7),
                    Orden = rd.GetInt32(8),
                    RequiereCliente = rd.GetBoolean(9),
                    RequiereDocumentoCliente = rd.GetBoolean(10),
                    PermiteEnPOS = rd.GetBoolean(11)
                });
            }

            return lista;
        }
    }
}