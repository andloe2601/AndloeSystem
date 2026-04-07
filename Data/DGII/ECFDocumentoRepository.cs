using Microsoft.Data.SqlClient;
using System;
using System.Data;

namespace Andloe.Data.DGII
{
    public sealed class ECFDocumentoRepository
    {
        public const string EST_PENDIENTE = "PENDIENTE";
        public const string EST_FIRMADO = "FIRMADO";
        public const string EST_ENVIADO = "ENVIADO";
        public const string EST_RESPONDIDO = "RESPONDIDO";
        public const string EST_ACEPTADO = "ACEPTADO";
        public const string EST_RECHAZADO = "RECHAZADO";
        public const string EST_ERROR = "ERROR";

        public void UpsertPendiente(int facturaId, int tipoEcf, string encf, string xmlSinFirmar)
        {
            if (facturaId <= 0) throw new ArgumentException("FacturaId inválido.", nameof(facturaId));
            if (tipoEcf <= 0) throw new ArgumentException("TipoECF inválido.", nameof(tipoEcf));
            if (string.IsNullOrWhiteSpace(encf)) throw new ArgumentException("ENCF inválido.", nameof(encf));

            encf = encf.Trim();
            xmlSinFirmar ??= "";

            const string sql = @"
IF EXISTS (SELECT 1 FROM dbo.ECFDocumento WHERE FacturaId = @FacturaId)
BEGIN
    UPDATE dbo.ECFDocumento
       SET TipoECF       = @TipoECF,
           ENCF          = @ENCF,
           EstadoDGII    = @Estado,
           TrackId       = NULL,
           XmlSinFirmar  = @XmlSinFirmar,
           XmlFirmado    = NULL,
           RespuestaDGII = NULL,
           FechaGenerado = SYSUTCDATETIME(),
           FechaFirmado  = NULL,
           FechaEnviado  = NULL,
           IntentosEnvio = 0,
           UltimoError   = NULL
     WHERE FacturaId = @FacturaId;
END
ELSE
BEGIN
    INSERT INTO dbo.ECFDocumento
    (
        FacturaId, TipoECF, ENCF, EstadoDGII, TrackId,
        XmlSinFirmar, XmlFirmado, RespuestaDGII,
        FechaGenerado, FechaFirmado, FechaEnviado,
        IntentosEnvio, UltimoError
    )
    VALUES
    (
        @FacturaId, @TipoECF, @ENCF, @Estado, NULL,
        @XmlSinFirmar, NULL, NULL,
        SYSUTCDATETIME(), NULL, NULL,
        0, NULL
    );
END
";
            using var cn = Db.GetOpenConnection();
            using var cmd = Db.CreateCommand(cn, sql);
            cmd.Parameters.Add("@FacturaId", SqlDbType.Int).Value = facturaId;
            cmd.Parameters.Add("@TipoECF", SqlDbType.Int).Value = tipoEcf;
            cmd.Parameters.Add("@ENCF", SqlDbType.VarChar, 20).Value = encf;
            cmd.Parameters.Add("@Estado", SqlDbType.VarChar, 20).Value = EST_PENDIENTE;
            cmd.Parameters.Add("@XmlSinFirmar", SqlDbType.NVarChar).Value =
                string.IsNullOrWhiteSpace(xmlSinFirmar) ? (object)DBNull.Value : xmlSinFirmar;

            cmd.ExecuteNonQuery();
        }

        public DataTable Listar(
            DateTime? desde = null,
            DateTime? hasta = null,
            string? estado = null,
            string? encfLike = null,
            int? facturaId = null,
            int top = 500)
        {
            if (top <= 0) top = 500;

            estado = string.IsNullOrWhiteSpace(estado) ? null : estado.Trim();
            encfLike = string.IsNullOrWhiteSpace(encfLike) ? null : encfLike.Trim();

            var d1 = desde?.Date;
            var d2 = hasta?.Date;

            const string sql = @"
SELECT TOP (@Top)
    e.ECFDocumentoId,
    e.FacturaId,
    e.TipoECF,
    e.ENCF,
    e.EstadoDGII,
    e.TrackId,
    e.FechaGenerado,
    e.FechaFirmado,
    e.FechaEnviado,
    e.IntentosEnvio,
    e.UltimoError,
    f.NumeroDocumento,
    f.FechaDocumento,
    f.NombreCliente AS Cliente,
    f.Estado AS EstadoFactura
FROM dbo.ECFDocumento e
LEFT JOIN dbo.FacturaCab f ON f.FacturaId = e.FacturaId
WHERE 1=1
  AND (@FacturaId IS NULL OR e.FacturaId = @FacturaId)
  AND (@Estado IS NULL OR e.EstadoDGII = @Estado)
  AND (@ENCFLike IS NULL OR e.ENCF LIKE @ENCFLike)
  AND (@Desde IS NULL OR e.FechaGenerado >= @Desde)
  AND (@Hasta IS NULL OR e.FechaGenerado < DATEADD(DAY, 1, @Hasta))
ORDER BY e.ECFDocumentoId DESC;
";
            using var cn = Db.GetOpenConnection();
            using var cmd = Db.CreateCommand(cn, sql);

            cmd.Parameters.Add("@Top", SqlDbType.Int).Value = top;
            cmd.Parameters.Add("@FacturaId", SqlDbType.Int).Value = (object?)facturaId ?? DBNull.Value;
            cmd.Parameters.Add("@Estado", SqlDbType.VarChar, 20).Value = (object?)estado ?? DBNull.Value;
            cmd.Parameters.Add("@ENCFLike", SqlDbType.VarChar, 30).Value =
                (object?)(encfLike == null ? null : $"%{encfLike}%") ?? DBNull.Value;
            cmd.Parameters.Add("@Desde", SqlDbType.Date).Value = (object?)d1 ?? DBNull.Value;
            cmd.Parameters.Add("@Hasta", SqlDbType.Date).Value = (object?)d2 ?? DBNull.Value;

            using var da = new SqlDataAdapter(cmd);
            var dt = new DataTable("ECFDocumento");
            da.Fill(dt);
            return dt;
        }

        public string? ObtenerXmlSinFirmar(int facturaId)
        {
            if (facturaId <= 0) return null;

            const string sql = @"SELECT XmlSinFirmar FROM dbo.ECFDocumento WHERE FacturaId = @FacturaId;";
            using var cn = Db.GetOpenConnection();
            using var cmd = Db.CreateCommand(cn, sql);
            cmd.Parameters.Add("@FacturaId", SqlDbType.Int).Value = facturaId;

            var v = cmd.ExecuteScalar();
            return v == null || v == DBNull.Value ? null : Convert.ToString(v);
        }

        public string? ObtenerXmlFirmado(int facturaId)
        {
            if (facturaId <= 0) return null;

            const string sql = @"SELECT XmlFirmado FROM dbo.ECFDocumento WHERE FacturaId = @FacturaId;";
            using var cn = Db.GetOpenConnection();
            using var cmd = Db.CreateCommand(cn, sql);
            cmd.Parameters.Add("@FacturaId", SqlDbType.Int).Value = facturaId;

            var v = cmd.ExecuteScalar();
            return v == null || v == DBNull.Value ? null : Convert.ToString(v);
        }

        public string? ObtenerTrackId(int facturaId)
        {
            if (facturaId <= 0) return null;

            const string sql = @"SELECT TrackId FROM dbo.ECFDocumento WHERE FacturaId = @FacturaId;";
            using var cn = Db.GetOpenConnection();
            using var cmd = Db.CreateCommand(cn, sql);
            cmd.Parameters.Add("@FacturaId", SqlDbType.Int).Value = facturaId;

            var v = cmd.ExecuteScalar();
            return v == null || v == DBNull.Value ? null : Convert.ToString(v);
        }

        public void GuardarXmlFirmadoPorFactura(int facturaId, string xmlFirmado)
        {
            if (facturaId <= 0) throw new ArgumentException("FacturaId inválido.", nameof(facturaId));
            xmlFirmado ??= "";

            const string sql = @"
UPDATE dbo.ECFDocumento
   SET XmlFirmado = @XmlFirmado,
       FechaFirmado = SYSUTCDATETIME(),
       EstadoDGII = @Estado,
       UltimoError = NULL
 WHERE FacturaId = @FacturaId;
";
            using var cn = Db.GetOpenConnection();
            using var cmd = Db.CreateCommand(cn, sql);
            cmd.Parameters.Add("@XmlFirmado", SqlDbType.NVarChar).Value =
                string.IsNullOrWhiteSpace(xmlFirmado) ? (object)DBNull.Value : xmlFirmado;
            cmd.Parameters.Add("@Estado", SqlDbType.VarChar, 20).Value = EST_FIRMADO;
            cmd.Parameters.Add("@FacturaId", SqlDbType.Int).Value = facturaId;
            cmd.ExecuteNonQuery();
        }

        public void MarcarPendientePorFactura(int facturaId)
        {
            if (facturaId <= 0) return;

            const string sql = @"
UPDATE dbo.ECFDocumento
   SET EstadoDGII = @Estado,
       UltimoError = NULL
 WHERE FacturaId = @FacturaId;";
            using var cn = Db.GetOpenConnection();
            using var cmd = Db.CreateCommand(cn, sql);
            cmd.Parameters.Add("@Estado", SqlDbType.VarChar, 20).Value = EST_PENDIENTE;
            cmd.Parameters.Add("@FacturaId", SqlDbType.Int).Value = facturaId;
            cmd.ExecuteNonQuery();
        }

        

        public void MarcarErrorPorFactura(int facturaId, string? error)
        {
            if (facturaId <= 0) return;

            error = (error ?? "").Trim();
            if (error.Length > 4000) error = error.Substring(0, 4000);

            const string sql = @"
UPDATE dbo.ECFDocumento
   SET EstadoDGII = @Estado,
       UltimoError = @Err,
       IntentosEnvio = ISNULL(IntentosEnvio,0) + 1
 WHERE FacturaId = @FacturaId;";
            using var cn = Db.GetOpenConnection();
            using var cmd = Db.CreateCommand(cn, sql);
            cmd.Parameters.Add("@Estado", SqlDbType.VarChar, 20).Value = EST_ERROR;
            cmd.Parameters.Add("@Err", SqlDbType.NVarChar, 4000).Value =
                string.IsNullOrWhiteSpace(error) ? (object)DBNull.Value : error;
            cmd.Parameters.Add("@FacturaId", SqlDbType.Int).Value = facturaId;
            cmd.ExecuteNonQuery();
        }

        public void GuardarRespuesta(int facturaId, string estado, string? respuesta, string? error)
        {
            if (facturaId <= 0) return;

            estado = (estado ?? "").Trim().ToUpperInvariant();
            if (string.IsNullOrWhiteSpace(estado)) estado = EST_RESPONDIDO;

            error = (error ?? "").Trim();
            if (error.Length > 4000) error = error.Substring(0, 4000);

            const string sql = @"
UPDATE dbo.ECFDocumento
   SET EstadoDGII = @Estado,
       RespuestaDGII = @Resp,
       UltimoError = @Err
 WHERE FacturaId = @FacturaId;";
            using var cn = Db.GetOpenConnection();
            using var cmd = Db.CreateCommand(cn, sql);
            cmd.Parameters.Add("@Estado", SqlDbType.VarChar, 20).Value = estado;
            cmd.Parameters.Add("@Resp", SqlDbType.NVarChar).Value =
                string.IsNullOrWhiteSpace(respuesta) ? (object)DBNull.Value : respuesta!;
            cmd.Parameters.Add("@Err", SqlDbType.NVarChar, 4000).Value =
                string.IsNullOrWhiteSpace(error) ? (object)DBNull.Value : error!;
            cmd.Parameters.Add("@FacturaId", SqlDbType.Int).Value = facturaId;
            cmd.ExecuteNonQuery();
        }

        public void UpdateEstado(long ecfDocumentoId, string estado, string? ultimoError = null, string? trackId = null)
        {
            if (ecfDocumentoId <= 0) throw new ArgumentException("ECFDocumentoId inválido.", nameof(ecfDocumentoId));

            estado = (estado ?? "").Trim().ToUpperInvariant();
            if (string.IsNullOrWhiteSpace(estado))
                throw new ArgumentException("Estado inválido.", nameof(estado));

            ultimoError = (ultimoError ?? "").Trim();
            if (ultimoError.Length > 4000) ultimoError = ultimoError.Substring(0, 4000);

            trackId = (trackId ?? "").Trim();
            if (trackId.Length > 80) trackId = trackId.Substring(0, 80);

            const string sql = @"
UPDATE dbo.ECFDocumento
   SET EstadoDGII = @Estado,
       UltimoError = @Err,
       TrackId = CASE WHEN @TrackId IS NULL OR LTRIM(RTRIM(@TrackId)) = '' THEN TrackId ELSE @TrackId END
 WHERE ECFDocumentoId = @Id;";
            using var cn = Db.GetOpenConnection();
            using var cmd = Db.CreateCommand(cn, sql);
            cmd.Parameters.Add("@Estado", SqlDbType.VarChar, 20).Value = estado;
            cmd.Parameters.Add("@Err", SqlDbType.NVarChar, 4000).Value =
                string.IsNullOrWhiteSpace(ultimoError) ? (object)DBNull.Value : ultimoError;
            cmd.Parameters.Add("@TrackId", SqlDbType.VarChar, 80).Value =
                string.IsNullOrWhiteSpace(trackId) ? (object)DBNull.Value : trackId;
            cmd.Parameters.Add("@Id", SqlDbType.BigInt).Value = ecfDocumentoId;
            cmd.ExecuteNonQuery();
        }
    
    public void GuardarRespuestaAlanubePorFactura(
    int facturaId,
    string? trackId,
    string? status,
    string? legalStatus,
    string? codigo,
    string? mensaje,
    string? rawJson)
        {
            if (facturaId <= 0) return;

            trackId = (trackId ?? "").Trim();
            if (trackId.Length > 80) trackId = trackId.Substring(0, 80);

            status = (status ?? "").Trim();
            if (status.Length > 20) status = status.Substring(0, 20);

            legalStatus = (legalStatus ?? "").Trim();
            if (legalStatus.Length > 20) legalStatus = legalStatus.Substring(0, 20);

            codigo = (codigo ?? "").Trim();
            if (codigo.Length > 50) codigo = codigo.Substring(0, 50);

            mensaje = (mensaje ?? "").Trim();
            if (mensaje.Length > 2000) mensaje = mensaje.Substring(0, 2000);

            const string sql = @"
UPDATE dbo.ECFDocumento
   SET TrackId = CASE WHEN @TrackId IS NULL OR LTRIM(RTRIM(@TrackId)) = '' THEN TrackId ELSE @TrackId END,
       EstadoDGII = CASE WHEN @LegalStatus IS NULL OR LTRIM(RTRIM(@LegalStatus)) = '' THEN EstadoDGII ELSE @LegalStatus END,
       EstadoProceso = CASE
                           WHEN UPPER(ISNULL(@Status,'')) IN ('ACCEPTED','ACEPTADO') THEN 'ACEPTADO'
                           WHEN UPPER(ISNULL(@Status,'')) IN ('REJECTED','RECHAZADO') THEN 'RECHAZADO'
                           WHEN UPPER(ISNULL(@Status,'')) IN ('SENT','ENVIADO','PENDING','PENDIENTE') THEN 'ENVIADO'
                           ELSE ISNULL(EstadoProceso, 'ENVIADO')
                       END,
       CodigoRespuestaDGII = CASE WHEN @Codigo IS NULL OR LTRIM(RTRIM(@Codigo)) = '' THEN CodigoRespuestaDGII ELSE @Codigo END,
       RespuestaDGIITexto = CASE WHEN @Mensaje IS NULL OR LTRIM(RTRIM(@Mensaje)) = '' THEN RespuestaDGIITexto ELSE @Mensaje END,
       RespuestaDGII = @RawJson,
       XmlRespuesta = @RawJson,
       FechaEnviado = CASE WHEN FechaEnviado IS NULL THEN SYSUTCDATETIME() ELSE FechaEnviado END,
       UltimoError = NULL
 WHERE FacturaId = @FacturaId;";

            using var cn = Db.GetOpenConnection();
            using var cmd = Db.CreateCommand(cn, sql);
            cmd.Parameters.Add("@FacturaId", SqlDbType.Int).Value = facturaId;
            cmd.Parameters.Add("@TrackId", SqlDbType.VarChar, 80).Value =
                string.IsNullOrWhiteSpace(trackId) ? (object)DBNull.Value : trackId;
            cmd.Parameters.Add("@Status", SqlDbType.VarChar, 20).Value =
                string.IsNullOrWhiteSpace(status) ? (object)DBNull.Value : status;
            cmd.Parameters.Add("@LegalStatus", SqlDbType.VarChar, 20).Value =
                string.IsNullOrWhiteSpace(legalStatus) ? (object)DBNull.Value : legalStatus;
            cmd.Parameters.Add("@Codigo", SqlDbType.VarChar, 50).Value =
                string.IsNullOrWhiteSpace(codigo) ? (object)DBNull.Value : codigo;
            cmd.Parameters.Add("@Mensaje", SqlDbType.NVarChar, 2000).Value =
                string.IsNullOrWhiteSpace(mensaje) ? (object)DBNull.Value : mensaje;
            cmd.Parameters.Add("@RawJson", SqlDbType.NVarChar).Value =
                string.IsNullOrWhiteSpace(rawJson) ? (object)DBNull.Value : rawJson;

            cmd.ExecuteNonQuery();
        }

        public void RegistrarConsultaAlanubePorFactura(
            int facturaId,
            string? status,
            string? legalStatus,
            string? codigo,
            string? mensaje,
            string? rawJson)
        {
            if (facturaId <= 0) return;

            status = (status ?? "").Trim();
            if (status.Length > 20) status = status.Substring(0, 20);

            legalStatus = (legalStatus ?? "").Trim();
            if (legalStatus.Length > 20) legalStatus = legalStatus.Substring(0, 20);

            codigo = (codigo ?? "").Trim();
            if (codigo.Length > 50) codigo = codigo.Substring(0, 50);

            mensaje = (mensaje ?? "").Trim();
            if (mensaje.Length > 2000) mensaje = mensaje.Substring(0, 2000);

            const string sql = @"
UPDATE dbo.ECFDocumento
   SET FechaUltimaConsulta = SYSUTCDATETIME(),
       IntentosConsulta = ISNULL(IntentosConsulta, 0) + 1,
       EstadoDGII = CASE WHEN @LegalStatus IS NULL OR LTRIM(RTRIM(@LegalStatus)) = '' THEN EstadoDGII ELSE @LegalStatus END,
       EstadoProceso = CASE
                           WHEN UPPER(ISNULL(@Status,'')) IN ('ACCEPTED','ACEPTADO') THEN 'ACEPTADO'
                           WHEN UPPER(ISNULL(@Status,'')) IN ('REJECTED','RECHAZADO') THEN 'RECHAZADO'
                           WHEN UPPER(ISNULL(@Status,'')) IN ('SENT','ENVIADO','PENDING','PENDIENTE') THEN 'ENVIADO'
                           ELSE ISNULL(EstadoProceso, EstadoDGII)
                       END,
       CodigoRespuestaDGII = CASE WHEN @Codigo IS NULL OR LTRIM(RTRIM(@Codigo)) = '' THEN CodigoRespuestaDGII ELSE @Codigo END,
       RespuestaDGIITexto = CASE WHEN @Mensaje IS NULL OR LTRIM(RTRIM(@Mensaje)) = '' THEN RespuestaDGIITexto ELSE @Mensaje END,
       RespuestaDGII = @RawJson,
       XmlRespuesta = @RawJson,
       FechaAceptado = CASE
                           WHEN UPPER(ISNULL(@Status,'')) IN ('ACCEPTED','ACEPTADO')
                                AND FechaAceptado IS NULL
                           THEN SYSUTCDATETIME()
                           ELSE FechaAceptado
                       END,
       FechaRechazado = CASE
                           WHEN UPPER(ISNULL(@Status,'')) IN ('REJECTED','RECHAZADO')
                                AND FechaRechazado IS NULL
                           THEN SYSUTCDATETIME()
                           ELSE FechaRechazado
                       END,
       UltimoError = CASE
                         WHEN UPPER(ISNULL(@Status,'')) IN ('REJECTED','RECHAZADO','ERROR')
                         THEN @Mensaje
                         ELSE NULL
                     END
 WHERE FacturaId = @FacturaId;";

            using var cn = Db.GetOpenConnection();
            using var cmd = Db.CreateCommand(cn, sql);
            cmd.Parameters.Add("@FacturaId", SqlDbType.Int).Value = facturaId;
            cmd.Parameters.Add("@Status", SqlDbType.VarChar, 20).Value =
                string.IsNullOrWhiteSpace(status) ? (object)DBNull.Value : status;
            cmd.Parameters.Add("@LegalStatus", SqlDbType.VarChar, 20).Value =
                string.IsNullOrWhiteSpace(legalStatus) ? (object)DBNull.Value : legalStatus;
            cmd.Parameters.Add("@Codigo", SqlDbType.VarChar, 50).Value =
                string.IsNullOrWhiteSpace(codigo) ? (object)DBNull.Value : codigo;
            cmd.Parameters.Add("@Mensaje", SqlDbType.NVarChar, 2000).Value =
                string.IsNullOrWhiteSpace(mensaje) ? (object)DBNull.Value : mensaje;
            cmd.Parameters.Add("@RawJson", SqlDbType.NVarChar).Value =
                string.IsNullOrWhiteSpace(rawJson) ? (object)DBNull.Value : rawJson;

            cmd.ExecuteNonQuery();
        }
    

    public void MarcarFirmadoPorFactura(int facturaId)
        {
            if (facturaId <= 0) return;

            const string sql = @"
UPDATE dbo.ECFDocumento
   SET EstadoDGII = 'FIRMADO',
       EstadoProceso = 'FIRMADO',
       FechaFirmado = ISNULL(FechaFirmado, SYSUTCDATETIME()),
       UltimoError = NULL
 WHERE FacturaId = @FacturaId;";

            using var cn = Db.GetOpenConnection();
            using var cmd = Db.CreateCommand(cn, sql);
            cmd.Parameters.Add("@FacturaId", SqlDbType.Int).Value = facturaId;
            cmd.ExecuteNonQuery();
        }

        public void MarcarEnviadoPorFactura(int facturaId, string? trackId)
        {
            if (facturaId <= 0) return;

            const string sql = @"
UPDATE dbo.ECFDocumento
   SET EstadoDGII = 'ENVIADO',
       EstadoProceso = 'ENVIADO',
       TrackId = CASE WHEN @TrackId IS NULL OR LTRIM(RTRIM(@TrackId)) = '' THEN TrackId ELSE @TrackId END,
       FechaEnviado = ISNULL(FechaEnviado, SYSUTCDATETIME()),
       UltimoError = NULL
 WHERE FacturaId = @FacturaId;";

            using var cn = Db.GetOpenConnection();
            using var cmd = Db.CreateCommand(cn, sql);
            cmd.Parameters.Add("@FacturaId", SqlDbType.Int).Value = facturaId;
            cmd.Parameters.Add("@TrackId", SqlDbType.VarChar, 80).Value =
                string.IsNullOrWhiteSpace(trackId) ? (object)DBNull.Value : trackId.Trim();
            cmd.ExecuteNonQuery();
        }

       

    public void ActualizarTrackingYRespuesta(int facturaId, string? trackId, string? estado, string? respuesta)
        {
            if (facturaId <= 0) return;

            const string sql = @"
UPDATE dbo.ECFDocumento
   SET TrackId = CASE WHEN @TrackId IS NULL OR LTRIM(RTRIM(@TrackId)) = '' THEN TrackId ELSE @TrackId END,
       EstadoDGII = CASE WHEN @Estado IS NULL OR LTRIM(RTRIM(@Estado)) = '' THEN EstadoDGII ELSE @Estado END,
       EstadoProceso = CASE WHEN @Estado IS NULL OR LTRIM(RTRIM(@Estado)) = '' THEN EstadoProceso ELSE @Estado END,
       RespuestaDGII = @Respuesta,
       XmlRespuesta = @Respuesta,
       UltimoError = NULL
 WHERE FacturaId = @FacturaId;";

            using var cn = Db.GetOpenConnection();
            using var cmd = Db.CreateCommand(cn, sql);
            cmd.Parameters.Add("@FacturaId", SqlDbType.Int).Value = facturaId;
            cmd.Parameters.Add("@TrackId", SqlDbType.VarChar, 80).Value =
                string.IsNullOrWhiteSpace(trackId) ? (object)DBNull.Value : trackId.Trim();
            cmd.Parameters.Add("@Estado", SqlDbType.VarChar, 20).Value =
                string.IsNullOrWhiteSpace(estado) ? (object)DBNull.Value : estado.Trim();
            cmd.Parameters.Add("@Respuesta", SqlDbType.NVarChar).Value =
                string.IsNullOrWhiteSpace(respuesta) ? (object)DBNull.Value : respuesta;
            cmd.ExecuteNonQuery();
        }

        public void ActualizarEstadoPorFactura(int facturaId, string? estado, string? mensaje)
        {
            if (facturaId <= 0) return;

            const string sql = @"
UPDATE dbo.ECFDocumento
   SET EstadoDGII = CASE WHEN @Estado IS NULL OR LTRIM(RTRIM(@Estado)) = '' THEN EstadoDGII ELSE @Estado END,
       EstadoProceso = CASE WHEN @Estado IS NULL OR LTRIM(RTRIM(@Estado)) = '' THEN EstadoProceso ELSE @Estado END,
       RespuestaDGIITexto = @Mensaje,
       FechaUltimaConsulta = SYSUTCDATETIME(),
       IntentosConsulta = ISNULL(IntentosConsulta, 0) + 1,
       UltimoError = CASE WHEN UPPER(ISNULL(@Estado,'')) IN ('ERROR','RECHAZADO') THEN @Mensaje ELSE NULL END
 WHERE FacturaId = @FacturaId;";

            using var cn = Db.GetOpenConnection();
            using var cmd = Db.CreateCommand(cn, sql);
            cmd.Parameters.Add("@FacturaId", SqlDbType.Int).Value = facturaId;
            cmd.Parameters.Add("@Estado", SqlDbType.VarChar, 20).Value =
                string.IsNullOrWhiteSpace(estado) ? (object)DBNull.Value : estado.Trim();
            cmd.Parameters.Add("@Mensaje", SqlDbType.NVarChar, 2000).Value =
                string.IsNullOrWhiteSpace(mensaje) ? (object)DBNull.Value : mensaje.Trim();
            cmd.ExecuteNonQuery();
        }

        public ECFDocumento? ObtenerDocumentoPorFactura(long facturaId)
        {
            using var cn = Db.GetOpenConnection();

            using var cmd = new SqlCommand(@"
        SELECT TOP 1 *
        FROM ECFDocumento
        WHERE FacturaId = @FacturaId
        ORDER BY ECFDocumentoId DESC", cn);

            cmd.Parameters.AddWithValue("@FacturaId", facturaId);

            using var rd = cmd.ExecuteReader();

            if (!rd.Read()) return null;

            return new ECFDocumento
            {
                ECFDocumentoId = Convert.ToInt64(rd["ECFDocumentoId"]),
                FacturaId = Convert.ToInt64(rd["FacturaId"]),
                ENCF = rd["ENCF"]?.ToString(),
                TrackId = rd["TrackId"]?.ToString(),
                XmlRespuesta = rd["XmlRespuesta"]?.ToString(),
                XmlEnviado = rd["XmlEnviado"]?.ToString(),
                EstadoDGII = rd["EstadoDGII"]?.ToString()
            };
        }

    }
}