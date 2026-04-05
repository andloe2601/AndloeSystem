using Andloe.Entidad;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace Andloe.Data
{
    public sealed class EmpresaRepository
    {
        public List<Empresa> Listar()
        {
            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
SELECT
    EmpresaId, RazonSocial, RNC, MonedaBaseCodigo, Pais, Provincia, Ciudad,
    Direccion, Telefono, Email, Estado, FechaCreacion, Logo, MunicipioId
FROM dbo.Empresa
ORDER BY RazonSocial;", cn);

            using var rd = cmd.ExecuteReader();
            var list = new List<Empresa>();

            while (rd.Read())
            {
                list.Add(Map(rd));
            }

            return list;
        }

        public Empresa? ObtenerPorId(int empresaId)
        {
            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
SELECT
    EmpresaId, RazonSocial, RNC, MonedaBaseCodigo, Pais, Provincia, Ciudad,
    Direccion, Telefono, Email, Estado, FechaCreacion, Logo, MunicipioId
FROM dbo.Empresa
WHERE EmpresaId = @EmpresaId;", cn);

            cmd.Parameters.Add("@EmpresaId", SqlDbType.Int).Value = empresaId;

            using var rd = cmd.ExecuteReader();
            return rd.Read() ? Map(rd) : null;
        }

        public int Insertar(Empresa e)
        {
            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
INSERT INTO dbo.Empresa
(
    RazonSocial, RNC, MonedaBaseCodigo, Pais, Provincia, Ciudad,
    Direccion, Telefono, Email, Estado, FechaCreacion, Logo, MunicipioId
)
OUTPUT INSERTED.EmpresaId
VALUES
(
    @RazonSocial, @RNC, @MonedaBaseCodigo, @Pais, @Provincia, @Ciudad,
    @Direccion, @Telefono, @Email, @Estado, SYSDATETIME(), @Logo, @MunicipioId
);", cn);

            AddParameters(cmd, e);
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public void Actualizar(Empresa e)
        {
            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
UPDATE dbo.Empresa
SET
    RazonSocial = @RazonSocial,
    RNC = @RNC,
    MonedaBaseCodigo = @MonedaBaseCodigo,
    Pais = @Pais,
    Provincia = @Provincia,
    Ciudad = @Ciudad,
    Direccion = @Direccion,
    Telefono = @Telefono,
    Email = @Email,
    Estado = @Estado,
    Logo = @Logo,
    MunicipioId = @MunicipioId
WHERE EmpresaId = @EmpresaId;", cn);

            cmd.Parameters.Add("@EmpresaId", SqlDbType.Int).Value = e.EmpresaId;
            AddParameters(cmd, e);
            cmd.ExecuteNonQuery();
        }

        public void Eliminar(int empresaId)
        {
            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand("DELETE FROM dbo.Empresa WHERE EmpresaId = @EmpresaId;", cn);
            cmd.Parameters.Add("@EmpresaId", SqlDbType.Int).Value = empresaId;
            cmd.ExecuteNonQuery();
        }

        private static void AddParameters(SqlCommand cmd, Empresa e)
        {
            cmd.Parameters.Add("@RazonSocial", SqlDbType.NVarChar, 200).Value = e.RazonSocial.Trim();
            cmd.Parameters.Add("@RNC", SqlDbType.VarChar, 15).Value = e.RNC.Trim();
            cmd.Parameters.Add("@MonedaBaseCodigo", SqlDbType.VarChar, 3).Value =
                string.IsNullOrWhiteSpace(e.MonedaBaseCodigo) ? "DOP" : e.MonedaBaseCodigo.Trim();
            cmd.Parameters.Add("@Pais", SqlDbType.VarChar, 60).Value = (object?)e.Pais ?? DBNull.Value;
            cmd.Parameters.Add("@Provincia", SqlDbType.VarChar, 60).Value = (object?)e.Provincia ?? DBNull.Value;
            cmd.Parameters.Add("@Ciudad", SqlDbType.NVarChar, 80).Value = (object?)e.Ciudad ?? DBNull.Value;
            cmd.Parameters.Add("@Direccion", SqlDbType.NVarChar, 200).Value = (object?)e.Direccion ?? DBNull.Value;
            cmd.Parameters.Add("@Telefono", SqlDbType.VarChar, 20).Value = (object?)e.Telefono ?? DBNull.Value;
            cmd.Parameters.Add("@Email", SqlDbType.VarChar, 100).Value = (object?)e.Email ?? DBNull.Value;
            cmd.Parameters.Add("@Estado", SqlDbType.Bit).Value = e.Estado;
            cmd.Parameters.Add("@Logo", SqlDbType.VarBinary, -1).Value = (object?)e.Logo ?? DBNull.Value;
            cmd.Parameters.Add("@MunicipioId", SqlDbType.Int).Value = (object?)e.MunicipioId ?? DBNull.Value;
        }

        private static Empresa Map(SqlDataReader rd)
        {
            return new Empresa
            {
                EmpresaId = rd.GetInt32(rd.GetOrdinal("EmpresaId")),
                RazonSocial = rd["RazonSocial"]?.ToString() ?? "",
                RNC = rd["RNC"]?.ToString() ?? "",
                MonedaBaseCodigo = rd["MonedaBaseCodigo"]?.ToString() ?? "DOP",
                Pais = rd["Pais"] as string,
                Provincia = rd["Provincia"] as string,
                Ciudad = rd["Ciudad"] as string,
                Direccion = rd["Direccion"] as string,
                Telefono = rd["Telefono"] as string,
                Email = rd["Email"] as string,
                Estado = Convert.ToBoolean(rd["Estado"]),
                FechaCreacion = rd["FechaCreacion"] == DBNull.Value ? null : Convert.ToDateTime(rd["FechaCreacion"]),
                Logo = rd["Logo"] == DBNull.Value ? null : (byte[])rd["Logo"],
                MunicipioId = rd["MunicipioId"] == DBNull.Value ? null : Convert.ToInt32(rd["MunicipioId"])
            };
        }
    }
}