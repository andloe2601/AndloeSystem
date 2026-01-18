using System.Data;
using Microsoft.Data.SqlClient;
using Andloe.Entidad;

namespace Andloe.Data
{
    public class InvMovimientoLinRepository
    {
        public void InsertLinea(InvMovimientoLin lin, SqlTransaction tx)
        {
            var cn = tx.Connection ?? throw new InvalidOperationException("Transacción sin conexión.");

            using var cmd = new SqlCommand(@"
INSERT INTO dbo.InvMovimientoLin
(InvMovId, Linea, ProductoCodigo, Cantidad, CostoUnitario)
VALUES
(@InvMovId, @Linea, @ProductoCodigo, @Cantidad, @CostoUnitario);", cn, tx);

            cmd.Parameters.Add("@InvMovId", SqlDbType.BigInt).Value = lin.InvMovId;
            cmd.Parameters.Add("@Linea", SqlDbType.Int).Value = lin.Linea;
            cmd.Parameters.Add("@ProductoCodigo", SqlDbType.VarChar, 20).Value = lin.ProductoCodigo;

            var pCant = cmd.Parameters.Add("@Cantidad", SqlDbType.Decimal);
            pCant.Precision = 18; pCant.Scale = 4; pCant.Value = lin.Cantidad;

            var pCosto = cmd.Parameters.Add("@CostoUnitario", SqlDbType.Decimal);
            pCosto.Precision = 18; pCosto.Scale = 6; pCosto.Value = lin.CostoUnitario;

            cmd.ExecuteNonQuery();
        }
    }
}
