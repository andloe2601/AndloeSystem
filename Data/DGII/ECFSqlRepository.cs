using Andloe.Data;
using Entidad;
using Microsoft.Data.SqlClient;
using System;
using System.Data;

namespace Data
{
    public sealed class ECFSqlRepository
    {
        public string GenerarXmlFacturaV3(int facturaId, string? usuario = null)
        {
            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand("dbo.sp_ECF_GenerarXmlFactura_V3", cn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.Add("@FacturaId", SqlDbType.Int).Value = facturaId;
            cmd.Parameters.Add("@Usuario", SqlDbType.NVarChar, 100).Value = (object?)usuario ?? DBNull.Value;

            var xmlOut = new SqlParameter("@XmlOut", SqlDbType.NVarChar, -1)
            {
                Direction = ParameterDirection.Output
            };
            cmd.Parameters.Add(xmlOut);

            cmd.ExecuteNonQuery();

            return Convert.ToString(xmlOut.Value) ?? string.Empty;
        }

        public string ObtenerXmlSinFirmar(int facturaId)
        {
            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
SELECT TOP (1) XmlSinFirmar
FROM dbo.ECFDocumento
WHERE FacturaId = @FacturaId;", cn);

            cmd.Parameters.Add("@FacturaId", SqlDbType.Int).Value = facturaId;

            var value = cmd.ExecuteScalar();
            return value == null || value == DBNull.Value ? string.Empty : Convert.ToString(value)!;
        }

        public EcfDocumentoDto? ObtenerDocumentoPorFactura(int facturaId)
        {
            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
SELECT TOP (1)
    ECFDocumentoId,
    FacturaId,
    TipoECF,
    ENCF,
    EstadoDGII,
    XmlSinFirmar,
    XmlFirmado,
    FechaGenerado,
    FechaFirmado,
    FechaHoraFirma,
    DigestValue,
    SignatureValue,
    CanonicalizationMethod,
    SignatureMethod,
    CertThumbprint,
    CertSerialNumber,
    CertIssuer,
    CertSubject,
    HashDocumento,
    TrackId,
    CodigoSeguridad,
    XmlEnviado,
    XmlRespuesta,
    RespuestaDGII
FROM dbo.ECFDocumento
WHERE FacturaId = @FacturaId;", cn);

            cmd.Parameters.Add("@FacturaId", SqlDbType.Int).Value = facturaId;

            using var rd = cmd.ExecuteReader();
            if (!rd.Read()) return null;

            return new EcfDocumentoDto
            {
                ECFDocumentoId = Convert.ToInt64(rd["ECFDocumentoId"]),
                FacturaId = Convert.ToInt32(rd["FacturaId"]),
                TipoECF = rd["TipoECF"] as string,
                ENCF = rd["ENCF"] as string,
                EstadoDGII = rd["EstadoDGII"] as string,
                XmlSinFirmar = rd["XmlSinFirmar"] as string,
                XmlFirmado = rd["XmlFirmado"] as string,
                FechaGenerado = rd["FechaGenerado"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(rd["FechaGenerado"]),
                FechaFirmado = rd["FechaFirmado"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(rd["FechaFirmado"]),
                FechaHoraFirma = rd["FechaHoraFirma"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(rd["FechaHoraFirma"]),
                DigestValue = rd["DigestValue"] as string,
                SignatureValue = rd["SignatureValue"] as string,
                CanonicalizationMethod = rd["CanonicalizationMethod"] as string,
                SignatureMethod = rd["SignatureMethod"] as string,
                CertThumbprint = rd["CertThumbprint"] as string,
                CertSerialNumber = rd["CertSerialNumber"] as string,
                CertIssuer = rd["CertIssuer"] as string,
                CertSubject = rd["CertSubject"] as string,
                HashDocumento = rd["HashDocumento"] as string,
                TrackId = rd["TrackId"] as string,
                CodigoSeguridad = rd["CodigoSeguridad"] as string,
                XmlEnviado = rd["XmlEnviado"] as string,
                XmlRespuesta = rd["XmlRespuesta"] as string,
                RespuestaDGII = rd["RespuestaDGII"] as string
            };
        }

        public void RegistrarXmlFirmado(EcfFirmaResult firma)
        {
            if (firma == null) throw new ArgumentNullException(nameof(firma));
            if (firma.FacturaId <= 0) throw new ArgumentOutOfRangeException(nameof(firma.FacturaId));
            if (string.IsNullOrWhiteSpace(firma.XmlFirmado))
                throw new ArgumentException("XmlFirmado es requerido.", nameof(firma));

            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand("dbo.sp_ECF_FirmarXml", cn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.Add("@FacturaId", SqlDbType.Int).Value = firma.FacturaId;
            cmd.Parameters.Add("@XmlFirmado", SqlDbType.NVarChar, -1).Value = firma.XmlFirmado;
            cmd.Parameters.Add("@FechaHoraFirma", SqlDbType.DateTime2).Value = firma.FechaHoraFirma;
            cmd.Parameters.Add("@DigestValue", SqlDbType.NVarChar, 300).Value = (object?)firma.DigestValue ?? DBNull.Value;
            cmd.Parameters.Add("@SignatureValue", SqlDbType.NVarChar, -1).Value = (object?)firma.SignatureValue ?? DBNull.Value;
            cmd.Parameters.Add("@CanonicalizationMethod", SqlDbType.NVarChar, 200).Value = (object?)firma.CanonicalizationMethod ?? DBNull.Value;
            cmd.Parameters.Add("@SignatureMethod", SqlDbType.NVarChar, 200).Value = (object?)firma.SignatureMethod ?? DBNull.Value;
            cmd.Parameters.Add("@CertThumbprint", SqlDbType.VarChar, 200).Value = (object?)firma.CertThumbprint ?? DBNull.Value;
            cmd.Parameters.Add("@CertSerialNumber", SqlDbType.VarChar, 200).Value = (object?)firma.CertSerialNumber ?? DBNull.Value;
            cmd.Parameters.Add("@CertIssuer", SqlDbType.NVarChar, 500).Value = (object?)firma.CertIssuer ?? DBNull.Value;
            cmd.Parameters.Add("@CertSubject", SqlDbType.NVarChar, 500).Value = (object?)firma.CertSubject ?? DBNull.Value;
            cmd.Parameters.Add("@HashDocumento", SqlDbType.VarChar, 128).Value = (object?)firma.HashDocumento ?? DBNull.Value;
            cmd.Parameters.Add("@Usuario", SqlDbType.NVarChar, 100).Value = (object?)firma.Usuario ?? DBNull.Value;

            cmd.ExecuteNonQuery();
        }
    

    public string GenerarENcf(
    int empresaId,
    int sucursalId,
    int cajaId,
    int facturaId,
    int tipoEcf,
    string prefijo)
        {
            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand("sp_Factura_GenerarENcf", cn);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@EmpresaId", empresaId);
            cmd.Parameters.AddWithValue("@SucursalId", sucursalId);
            cmd.Parameters.AddWithValue("@CajaId", cajaId);
            cmd.Parameters.AddWithValue("@FacturaId", facturaId);
            cmd.Parameters.AddWithValue("@TipoId", tipoEcf);
            cmd.Parameters.AddWithValue("@Prefijo", prefijo);
            cmd.Parameters.AddWithValue("@TrackId", DBNull.Value);

            var outParam = new SqlParameter("@ENcfOut", SqlDbType.VarChar, 20)
            {
                Direction = ParameterDirection.Output
            };

            cmd.Parameters.Add(outParam);

            cmd.ExecuteNonQuery();

            return outParam.Value?.ToString() ?? "";
        }
    }
}