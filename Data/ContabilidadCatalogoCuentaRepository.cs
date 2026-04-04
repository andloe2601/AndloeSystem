using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace Andloe.Data
{
    public sealed class ContabilidadCatalogoCuentaRepository
    {
        public List<CtaRow> Listar(string? filtro = null)
        {
            var list = new List<CtaRow>();

            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
SELECT
    CuentaId,
    Codigo,
    Nombre AS Descripcion,  -- ✅ en BD es Nombre
    Tipo,
    Nivel,
    PadreId,
    CASE WHEN Estado = 1 THEN 'ACTIVO' ELSE 'INACTIVO' END AS Estado, -- ✅ Estado es bit
    CAST(NULL AS datetime) AS FechaCreacion                           -- ✅ no existe en BD
FROM dbo.ContabilidadCatalogoCuenta
WHERE (
    @filtro IS NULL
    OR Codigo LIKE '%' + @filtro + '%'
    OR Nombre LIKE '%' + @filtro + '%'
)
ORDER BY Codigo;", cn);

            cmd.Parameters.Add("@filtro", SqlDbType.VarChar, 120).Value =
                string.IsNullOrWhiteSpace(filtro) ? DBNull.Value : filtro.Trim();

            using var rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                list.Add(new CtaRow
                {
                    CuentaId = rd.GetInt32(0),
                    Codigo = rd.IsDBNull(1) ? "" : rd.GetString(1),
                    Descripcion = rd.IsDBNull(2) ? "" : rd.GetString(2),
                    Tipo = rd.IsDBNull(3) ? "" : rd.GetString(3),
                    Nivel = rd.IsDBNull(4) ? 0 : rd.GetInt32(4),
                    PadreId = rd.IsDBNull(5) ? (int?)null : rd.GetInt32(5),
                    Estado = rd.IsDBNull(6) ? "INACTIVO" : rd.GetString(6),
                    FechaCreacion = rd.IsDBNull(7) ? (DateTime?)null : rd.GetDateTime(7)
                });
            }

            return list;
        }

        public bool ExisteCodigo(string codigo, int? excluirCuentaId = null)
        {
            if (string.IsNullOrWhiteSpace(codigo))
                return false;

            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
SELECT TOP(1) 1
FROM dbo.ContabilidadCatalogoCuenta
WHERE Codigo = @Codigo
AND (@Excluir IS NULL OR CuentaId <> @Excluir);", cn);

            cmd.Parameters.Add("@Codigo", SqlDbType.VarChar, 30).Value = codigo.Trim();
            cmd.Parameters.Add("@Excluir", SqlDbType.Int).Value =
                excluirCuentaId.HasValue ? excluirCuentaId.Value : DBNull.Value;

            return cmd.ExecuteScalar() != null;
        }

        public bool TieneHijos(int cuentaId)
        {
            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
SELECT TOP(1) 1
FROM dbo.ContabilidadCatalogoCuenta
WHERE PadreId = @Id;", cn);

            cmd.Parameters.Add("@Id", SqlDbType.Int).Value = cuentaId;
            return cmd.ExecuteScalar() != null;
        }

        public int Insertar(CtaRow c)
        {
            if (c == null) throw new ArgumentNullException(nameof(c));
            if (string.IsNullOrWhiteSpace(c.Codigo)) throw new InvalidOperationException("Código requerido.");
            if (string.IsNullOrWhiteSpace(c.Descripcion)) throw new InvalidOperationException("Nombre/Descripción requerida.");

            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
INSERT INTO dbo.ContabilidadCatalogoCuenta
(Codigo, Nombre, Tipo, Nivel, PadreId, Estado)
VALUES
(@Codigo, @Nombre, @Tipo, @Nivel, @PadreId, @Estado);

SELECT CAST(SCOPE_IDENTITY() AS INT);", cn);

            cmd.Parameters.Add("@Codigo", SqlDbType.VarChar, 30).Value = c.Codigo.Trim();
            cmd.Parameters.Add("@Nombre", SqlDbType.VarChar, 200).Value = c.Descripcion.Trim();
            cmd.Parameters.Add("@Tipo", SqlDbType.VarChar, 20).Value = (c.Tipo ?? "").Trim();
            cmd.Parameters.Add("@Nivel", SqlDbType.Int).Value = c.Nivel;
            cmd.Parameters.Add("@PadreId", SqlDbType.Int).Value = c.PadreId.HasValue ? c.PadreId.Value : DBNull.Value;

            // ✅ Estado en BD es bit
            cmd.Parameters.Add("@Estado", SqlDbType.Bit).Value = EstadoToBit(c.Estado);

            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public void Actualizar(CtaRow c)
        {
            if (c == null) throw new ArgumentNullException(nameof(c));
            if (c.CuentaId <= 0) throw new InvalidOperationException("CuentaId inválido.");
            if (string.IsNullOrWhiteSpace(c.Codigo)) throw new InvalidOperationException("Código requerido.");
            if (string.IsNullOrWhiteSpace(c.Descripcion)) throw new InvalidOperationException("Nombre/Descripción requerida.");

            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
UPDATE dbo.ContabilidadCatalogoCuenta
SET Codigo  = @Codigo,
    Nombre  = @Nombre,
    Tipo    = @Tipo,
    Nivel   = @Nivel,
    PadreId = @PadreId,
    Estado  = @Estado
WHERE CuentaId = @CuentaId;", cn);

            cmd.Parameters.Add("@CuentaId", SqlDbType.Int).Value = c.CuentaId;
            cmd.Parameters.Add("@Codigo", SqlDbType.VarChar, 30).Value = c.Codigo.Trim();
            cmd.Parameters.Add("@Nombre", SqlDbType.VarChar, 200).Value = c.Descripcion.Trim();
            cmd.Parameters.Add("@Tipo", SqlDbType.VarChar, 20).Value = (c.Tipo ?? "").Trim();
            cmd.Parameters.Add("@Nivel", SqlDbType.Int).Value = c.Nivel;
            cmd.Parameters.Add("@PadreId", SqlDbType.Int).Value = c.PadreId.HasValue ? c.PadreId.Value : DBNull.Value;

            // ✅ Estado en BD es bit
            cmd.Parameters.Add("@Estado", SqlDbType.Bit).Value = EstadoToBit(c.Estado);

            cmd.ExecuteNonQuery();
        }

        public void Eliminar(int cuentaId)
        {
            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
DELETE FROM dbo.ContabilidadCatalogoCuenta
WHERE CuentaId = @Id;", cn);

            cmd.Parameters.Add("@Id", SqlDbType.Int).Value = cuentaId;
            cmd.ExecuteNonQuery();
        }

        public void SetEstado(int cuentaId, string estado)
        {
            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
UPDATE dbo.ContabilidadCatalogoCuenta
SET Estado = @Estado
WHERE CuentaId = @Id;", cn);

            cmd.Parameters.Add("@Id", SqlDbType.Int).Value = cuentaId;
            cmd.Parameters.Add("@Estado", SqlDbType.Bit).Value = EstadoToBit(estado);

            cmd.ExecuteNonQuery();
        }

        private static bool EstadoToBit(string? estado)
        {
            // Acepta: "ACTIVO"/"INACTIVO", "1"/"0", "TRUE"/"FALSE"
            if (string.IsNullOrWhiteSpace(estado))
                return true;

            var s = estado.Trim().ToUpperInvariant();

            if (s == "ACTIVO" || s == "A" || s == "1" || s == "TRUE" || s == "SI" || s == "S")
                return true;

            if (s == "INACTIVO" || s == "I" || s == "0" || s == "FALSE" || s == "NO" || s == "N")
                return false;

            // por defecto, activo
            return true;
        }
    }

    public sealed class CtaRow
    {
        public int CuentaId { get; set; }
        public string Codigo { get; set; } = "";

        // ⚠️ Se llama Descripcion porque tu UI/Forms así lo usan,
        // pero en BD esto se guarda en la columna "Nombre".
        public string Descripcion { get; set; } = "";

        public string Tipo { get; set; } = "";
        public int Nivel { get; set; }
        public int? PadreId { get; set; }

        // Se mantiene como string para no tocar tu UI (ACTIVO/INACTIVO)
        public string Estado { get; set; } = "ACTIVO";

        // No existe en BD, se devuelve NULL en Listar()
        public DateTime? FechaCreacion { get; set; }
    }
}
