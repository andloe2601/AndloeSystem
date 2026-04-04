using Andloe.Entidad.DGII;
using Microsoft.Data.SqlClient;
using System;
using System.Data;

namespace Andloe.Data.DGII
{
    public sealed class DGIIPostulacionSqlRepository
    {
        private readonly string _connectionString;

        public DGIIPostulacionSqlRepository(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException("Connection string vacía.", nameof(connectionString));

            _connectionString = connectionString;
        }

        public string GenerarXmlPostulacion(long postulacionId, string? usuario = null)
        {
            using var cn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("dbo.sp_DGII_GenerarXmlPostulacion", cn);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@DGIIPostulacionId", SqlDbType.BigInt).Value = postulacionId;
            cmd.Parameters.Add("@Usuario", SqlDbType.NVarChar, 100).Value =
                string.IsNullOrWhiteSpace(usuario) ? (object)DBNull.Value : usuario!.Trim();

            var pXml = cmd.Parameters.Add("@XmlOut", SqlDbType.NVarChar, -1);
            pXml.Direction = ParameterDirection.Output;

            cn.Open();
            cmd.ExecuteNonQuery();

            return Convert.ToString(pXml.Value) ?? "";
        }

        public string ObtenerXmlSinFirmar(long postulacionId)
        {
            using var cn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(
                "SELECT XmlSinFirmar FROM dbo.DGIIPostulacion WHERE DGIIPostulacionId = @id",
                cn);

            cmd.Parameters.Add("@id", SqlDbType.BigInt).Value = postulacionId;

            cn.Open();

            return Convert.ToString(cmd.ExecuteScalar()) ?? "";
        }

        public void RegistrarXmlFirmado(SignedPostulacionResult result, string? usuario = null)
        {
            using var cn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("dbo.sp_DGII_FirmarXmlPostulacion", cn);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@DGIIPostulacionId", SqlDbType.BigInt).Value = result.DGIIPostulacionId;
            cmd.Parameters.Add("@XmlFirmado", SqlDbType.NVarChar, -1).Value = result.XmlFirmado;
            cmd.Parameters.Add("@FechaHoraFirma", SqlDbType.DateTime2).Value = result.FechaHoraFirma;

            cmd.Parameters.Add("@DigestValue", SqlDbType.NVarChar, 300).Value =
                string.IsNullOrWhiteSpace(result.DigestValue) ? (object)DBNull.Value : result.DigestValue!;

            cmd.Parameters.Add("@SignatureValue", SqlDbType.NVarChar, -1).Value =
                string.IsNullOrWhiteSpace(result.SignatureValue) ? (object)DBNull.Value : result.SignatureValue!;

            cmd.Parameters.Add("@CanonicalizationMethod", SqlDbType.NVarChar, 200).Value =
                string.IsNullOrWhiteSpace(result.CanonicalizationMethod) ? (object)DBNull.Value : result.CanonicalizationMethod!;

            cmd.Parameters.Add("@SignatureMethod", SqlDbType.NVarChar, 200).Value =
                string.IsNullOrWhiteSpace(result.SignatureMethod) ? (object)DBNull.Value : result.SignatureMethod!;

            cmd.Parameters.Add("@CertThumbprint", SqlDbType.VarChar, 200).Value =
                string.IsNullOrWhiteSpace(result.CertThumbprint) ? (object)DBNull.Value : result.CertThumbprint!;

            cmd.Parameters.Add("@CertSerialNumber", SqlDbType.VarChar, 200).Value =
                string.IsNullOrWhiteSpace(result.CertSerialNumber) ? (object)DBNull.Value : result.CertSerialNumber!;

            cmd.Parameters.Add("@CertIssuer", SqlDbType.NVarChar, 500).Value =
                string.IsNullOrWhiteSpace(result.CertIssuer) ? (object)DBNull.Value : result.CertIssuer!;

            cmd.Parameters.Add("@CertSubject", SqlDbType.NVarChar, 500).Value =
                string.IsNullOrWhiteSpace(result.CertSubject) ? (object)DBNull.Value : result.CertSubject!;

            cmd.Parameters.Add("@HashDocumento", SqlDbType.VarChar, 128).Value =
                string.IsNullOrWhiteSpace(result.HashDocumento) ? (object)DBNull.Value : result.HashDocumento!;

            cn.Open();
            cmd.ExecuteNonQuery();
        }

        public DataTable ObtenerLog(long postulacionId)
        {
            using var cn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(@"
SELECT
    DGIIPostulacionLogId,
    Evento,
    EstadoAnterior,
    EstadoNuevo,
    Mensaje,
    FechaEvento,
    Usuario,
    Origen
FROM dbo.DGIIPostulacionLog
WHERE DGIIPostulacionId = @Id
ORDER BY FechaEvento DESC;", cn);

            cmd.Parameters.Add("@Id", SqlDbType.BigInt).Value = postulacionId;

            var dt = new DataTable();
            using var da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            return dt;
        }

        public void InsertarLog(
            long postulacionId,
            string evento,
            string? estadoAnterior,
            string? estadoNuevo,
            string? mensaje,
            string? usuario,
            string? origen)
        {
            using var cn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("dbo.sp_DGII_PostulacionLog_Insertar", cn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@DGIIPostulacionId", SqlDbType.BigInt).Value = postulacionId;
            cmd.Parameters.Add("@Evento", SqlDbType.VarChar, 50).Value = evento;
            cmd.Parameters.Add("@EstadoAnterior", SqlDbType.VarChar, 20).Value =
                string.IsNullOrWhiteSpace(estadoAnterior) ? (object)DBNull.Value : estadoAnterior!;
            cmd.Parameters.Add("@EstadoNuevo", SqlDbType.VarChar, 20).Value =
                string.IsNullOrWhiteSpace(estadoNuevo) ? (object)DBNull.Value : estadoNuevo!;
            cmd.Parameters.Add("@Mensaje", SqlDbType.NVarChar, 1000).Value =
                string.IsNullOrWhiteSpace(mensaje) ? (object)DBNull.Value : mensaje!;
            cmd.Parameters.Add("@Usuario", SqlDbType.NVarChar, 100).Value =
                string.IsNullOrWhiteSpace(usuario) ? (object)DBNull.Value : usuario!;
            cmd.Parameters.Add("@Origen", SqlDbType.NVarChar, 100).Value =
                string.IsNullOrWhiteSpace(origen) ? (object)DBNull.Value : origen!;

            cn.Open();
            cmd.ExecuteNonQuery();
        }


        public DataTable ValidarPostulacion(long postulacionId)
        {
            using var cn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("dbo.sp_DGII_ValidarPostulacion", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@DGIIPostulacionId", SqlDbType.BigInt).Value = postulacionId;

            var dt = new DataTable();
            using var da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            return dt;
        }
    }
}