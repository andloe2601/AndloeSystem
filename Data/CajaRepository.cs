using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace Andloe.Data
{
    public class CajaDto
    {
        public int CajaId { get; set; }
        public int SucursalId { get; set; }
        public string CajaNumero { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public string Estado { get; set; } = "ACTIVA";

        // Texto que se muestra en combos / grids
        public string Display =>
            string.IsNullOrWhiteSpace(Descripcion)
                ? CajaNumero
                : $"{CajaNumero} - {Descripcion}";
    }

    public class CajaRepository
    {
        // ==========================
        // LISTADOS
        // ==========================

        /// <summary>
        /// Lista todas las cajas activas (todas las sucursales).
        /// </summary>
        public List<CajaDto> ListarActivas()
        {
            var lista = new List<CajaDto>();

            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
SELECT 
    CajaId,
    SucursalId,
    CajaNumero,
    Descripcion,
    Estado
FROM dbo.Caja
WHERE Estado IN ('ACTIVA','ACTIVO')
ORDER BY SucursalId, CajaNumero;", cn);

            using var rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                lista.Add(new CajaDto
                {
                    CajaId = rd.GetInt32(0),
                    SucursalId = rd.GetInt32(1),
                    CajaNumero = rd.GetString(2),
                    Descripcion = rd.IsDBNull(3) ? null : rd.GetString(3),
                    Estado = rd.GetString(4)
                });
            }

            return lista;
        }

        /// <summary>
        /// Lista las cajas de una sucursal específica (con opción de incluir inactivas).
        /// </summary>
        public List<CajaDto> ListarPorSucursal(int sucursalId, bool soloActivas = false)
        {
            var lista = new List<CajaDto>();

            using var cn = Db.GetOpenConnection();

            string sql = @"
SELECT 
    CajaId,
    SucursalId,
    CajaNumero,
    Descripcion,
    Estado
FROM dbo.Caja
WHERE SucursalId = @SucursalId";

            if (soloActivas)
                sql += " AND Estado IN ('ACTIVA','ACTIVO')";

            sql += " ORDER BY CajaNumero;";

            using var cmd = new SqlCommand(sql, cn);
            cmd.Parameters.Add("@SucursalId", SqlDbType.Int).Value = sucursalId;

            using var rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                lista.Add(new CajaDto
                {
                    CajaId = rd.GetInt32(0),
                    SucursalId = rd.GetInt32(1),
                    CajaNumero = rd.GetString(2),
                    Descripcion = rd.IsDBNull(3) ? null : rd.GetString(3),
                    Estado = rd.GetString(4)
                });
            }

            return lista;
        }

        /// <summary>
        /// Obtiene una caja por su ID.
        /// </summary>
        public CajaDto? ObtenerPorId(int cajaId)
        {
            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
SELECT 
    CajaId,
    SucursalId,
    CajaNumero,
    Descripcion,
    Estado
FROM dbo.Caja
WHERE CajaId = @CajaId;", cn);

            cmd.Parameters.Add("@CajaId", SqlDbType.Int).Value = cajaId;

            using var rd = cmd.ExecuteReader();
            if (!rd.Read())
                return null;

            return new CajaDto
            {
                CajaId = rd.GetInt32(0),
                SucursalId = rd.GetInt32(1),
                CajaNumero = rd.GetString(2),
                Descripcion = rd.IsDBNull(3) ? null : rd.GetString(3),
                Estado = rd.GetString(4)
            };
        }

        // ==========================
        // VALIDACIONES
        // ==========================

        /// <summary>
        /// Valida si ya existe un CajaNumero en una sucursal.
        /// Opcionalmente excluye una caja específica (para edición).
        /// </summary>
        public bool ExisteNumeroEnSucursal(int sucursalId, string cajaNumero, int? excluirCajaId = null)
        {
            using var cn = Db.GetOpenConnection();

            string sql = @"
SELECT COUNT(1)
FROM dbo.Caja
WHERE SucursalId = @SucursalId
  AND CajaNumero = @CajaNumero";

            if (excluirCajaId.HasValue)
                sql += " AND CajaId <> @ExcluirCajaId;";

            using var cmd = new SqlCommand(sql, cn);
            cmd.Parameters.Add("@SucursalId", SqlDbType.Int).Value = sucursalId;
            cmd.Parameters.Add("@CajaNumero", SqlDbType.NVarChar, 10).Value = cajaNumero.Trim();

            if (excluirCajaId.HasValue)
                cmd.Parameters.Add("@ExcluirCajaId", SqlDbType.Int).Value = excluirCajaId.Value;

            var obj = cmd.ExecuteScalar();
            var count = (obj == null || obj == DBNull.Value) ? 0 : Convert.ToInt32(obj);
            return count > 0;
        }

        // ==========================
        // INSERT / UPDATE / DELETE
        // ==========================

        /// <summary>
        /// Inserta una caja nueva.
        /// (Versión genérica)
        /// </summary>
        public int Insertar(CajaDto caja)
        {
            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
INSERT INTO dbo.Caja(
    SucursalId,
    CajaNumero,
    Descripcion,
    Estado
)
VALUES(
    @SucursalId,
    @CajaNumero,
    @Descripcion,
    @Estado
);

SELECT CAST(SCOPE_IDENTITY() AS INT);", cn);

            // OJO: NO se envía CajaId, lo genera el IDENTITY
            cmd.Parameters.Add("@SucursalId", SqlDbType.Int).Value = caja.SucursalId;
            cmd.Parameters.Add("@CajaNumero", SqlDbType.NVarChar, 10).Value = caja.CajaNumero;
            cmd.Parameters.Add("@Descripcion", SqlDbType.NVarChar, 100).Value =
                (object?)caja.Descripcion ?? DBNull.Value;
            cmd.Parameters.Add("@Estado", SqlDbType.VarChar, 12).Value = caja.Estado;

            var obj = cmd.ExecuteScalar();
            return (obj == null || obj == DBNull.Value) ? 0 : Convert.ToInt32(obj);
        }

        // Nombre usado por el formulario
        public int InsertarCaja(CajaDto caja)
        {
            return Insertar(caja);  // devuelve el nuevo CajaId
        }

        /// <summary>
        /// Actualiza una caja existente.
        /// (Versión genérica)
        /// </summary>
        public int Actualizar(CajaDto caja)
        {
            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
UPDATE dbo.Caja
SET
    SucursalId = @SucursalId,
    CajaNumero = @CajaNumero,
    Descripcion = @Descripcion,
    Estado = @Estado
WHERE CajaId = @CajaId;", cn);

            cmd.Parameters.Add("@CajaId", SqlDbType.Int).Value = caja.CajaId;
            cmd.Parameters.Add("@SucursalId", SqlDbType.Int).Value = caja.SucursalId;
            cmd.Parameters.Add("@CajaNumero", SqlDbType.NVarChar, 10).Value = caja.CajaNumero;
            cmd.Parameters.Add("@Descripcion", SqlDbType.NVarChar, 100).Value =
                (object?)caja.Descripcion ?? DBNull.Value;
            cmd.Parameters.Add("@Estado", SqlDbType.VarChar, 12).Value = caja.Estado;

            return cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Actualiza una caja existente.
        /// (Nombre usado por el formulario: ActualizarCaja)
        /// </summary>
        public int ActualizarCaja(CajaDto caja)
        {
            return Actualizar(caja);
        }

        /// <summary>
        /// Elimina una caja por ID.
        /// (Versión genérica)
        /// </summary>
        public int Eliminar(int cajaId)
        {
            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
DELETE FROM dbo.Caja
WHERE CajaId = @CajaId;", cn);

            cmd.Parameters.Add("@CajaId", SqlDbType.Int).Value = cajaId;
            return cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Elimina una caja por ID.
        /// (Nombre usado por el formulario: EliminarCaja)
        /// </summary>
        public int EliminarCaja(int cajaId)
        {
            return Eliminar(cajaId);
        }

        public int ObtenerSiguienteCajaId()
        {
            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(
                "SELECT ISNULL(MAX(CajaId), 0) + 1 FROM dbo.Caja;",
                cn);

            var obj = cmd.ExecuteScalar();
            if (obj == null || obj == DBNull.Value)
                return 1;

            return Convert.ToInt32(obj);
        }
    
    public string? ObtenerCajaNumero(int cajaId)
        {
            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
SELECT TOP 1 CajaNumero
FROM dbo.Caja
WHERE CajaId = @id;", cn);

            cmd.Parameters.Add("@id", SqlDbType.Int).Value = cajaId;

            var val = cmd.ExecuteScalar();
            if (val == null || val == DBNull.Value) return null;

            return Convert.ToString(val);
        }

        public string? ObtenerDescripcion(int cajaId)
        {
            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
SELECT TOP 1 Descripcion
FROM dbo.Caja
WHERE CajaId = @id;", cn);

            cmd.Parameters.Add("@id", SqlDbType.Int).Value = cajaId;

            var val = cmd.ExecuteScalar();
            if (val == null || val == DBNull.Value) return null;

            return Convert.ToString(val);
        }
    }
}
