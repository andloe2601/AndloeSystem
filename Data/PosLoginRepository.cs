using Microsoft.Data.SqlClient;
using System;
using System.Data;

namespace Andloe.Data
{
    public class PosUsuarioCajaDto
    {
        public bool Ok { get; set; }
        public int PosUsuarioId { get; set; }
        public string Usuario { get; set; } = string.Empty;
        public int? CajaId { get; set; }
        public bool PuedeTodasCajas { get; set; }
        public string? Estado { get; set; }
    }

    public class PosLoginRepository
    {
        /// <summary>
        /// Valida SOLO por PIN y devuelve los datos del usuario POS.
        /// </summary>
        public PosUsuarioCajaDto? ValidarPorPin(string pin)
        {
            if (string.IsNullOrWhiteSpace(pin))
                throw new ArgumentException("pin requerido", nameof(pin));

            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
SELECT TOP(1)
    PosUsuarioId,
    Usuario,
    CajaId,
    PuedeTodasCajas,
    Estado
FROM dbo.PosUsuario
WHERE ClavePIN = @Pin
ORDER BY PosUsuarioId DESC;", cn);

            cmd.Parameters.Add("@Pin", SqlDbType.NVarChar, 20).Value = pin;

            using var dr = cmd.ExecuteReader();
            if (!dr.Read())
                return null; // PIN no existe

            var dto = new PosUsuarioCajaDto
            {
                PosUsuarioId = dr.GetInt32(dr.GetOrdinal("PosUsuarioId")),
                Usuario = dr.GetString(dr.GetOrdinal("Usuario")),
                CajaId = dr.IsDBNull(dr.GetOrdinal("CajaId"))
                               ? (int?)null
                               : dr.GetInt32(dr.GetOrdinal("CajaId")),
                PuedeTodasCajas = dr.GetBoolean(dr.GetOrdinal("PuedeTodasCajas"))
            };

            string? estado = dr.IsDBNull(dr.GetOrdinal("Estado"))
                ? null
                : dr.GetString(dr.GetOrdinal("Estado"));

            estado = estado?.Trim().ToUpperInvariant();
            dto.Estado = estado;

            // Lo marcamos OK solo si está ACTIVO/ACTIVA o sin estado
            dto.Ok = string.IsNullOrEmpty(estado) ||
                     estado == "ACTIVO" ||
                     estado == "ACTIVA";

            return dto;
        }

        // Opcional: dejas el viejo método para no romper otros lados
        public PosUsuarioCajaDto? ValidarUsuario(string usuario, string pin)
            => ValidarPorPin(pin);
    }
}
