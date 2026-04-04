using Andloe.Data;
using Microsoft.Data.SqlClient;
using System;
using System.Data;

namespace Data
{
    public sealed class SnapshotFiscalRepository
    {
        public void SincronizarCompradorFactura(int facturaId)
        {
            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
UPDATE FC
   SET FC.RncCompradorSnapshot = COALESCE(NULLIF(C.RNC_Cedula, ''), FC.DocumentoCliente),
       FC.RazonSocialCompradorSnapshot = COALESCE(NULLIF(C.RazonSocialFiscal, ''), FC.NombreCliente),
       FC.CorreoCompradorSnapshot = COALESCE(NULLIF(C.CorreoFiscal, ''), C.Email),
       FC.DireccionCompradorSnapshot = COALESCE(NULLIF(C.Direccion, ''), FC.DireccionCliente),
       FC.MunicipioCompradorSnapshot = C.MunicipioCodigo,
       FC.ProvinciaCompradorSnapshot = C.ProvinciaCodigo,
       FC.IdentificadorExtranjeroSnapshot = C.IdentificadorExtranjero
FROM dbo.FacturaCab FC
LEFT JOIN dbo.Cliente C
       ON C.ClienteId = FC.ClienteId
WHERE FC.FacturaId = @FacturaId;", cn);

            cmd.Parameters.Add("@FacturaId", SqlDbType.Int).Value = facturaId;
            cmd.ExecuteNonQuery();
        }
    }
}