using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;

namespace Andloe.Data.DGII
{
    public class DgiiImportService
    {
        private readonly string _zipUrl;

        public DgiiImportService(string zipUrl)
        {
            _zipUrl = zipUrl ?? throw new ArgumentNullException(nameof(zipUrl));
        }

        // ===============================
        // A) IMPORTAR POR ZIP
        // ===============================
        public int ImportarDataset(bool skipIfSameSha = true)
        {
            var workDir = Path.Combine(AppContext.BaseDirectory, "_dgii");
            Directory.CreateDirectory(workDir);

            var zipPath = Path.Combine(workDir, "DGII_RNC.zip");
            var txtPath = Path.Combine(workDir, "DGII_RNC.TXT");

            DownloadFile(_zipUrl, zipPath).GetAwaiter().GetResult();

            var sha = ComputeSha256(zipPath);
            var zipBytes = new FileInfo(zipPath).Length;

            ExtractTxtFromZip(zipPath, txtPath);
            var txtBytes = new FileInfo(txtPath).Length;

            return ImportarCore(
                txtPath: txtPath,
                fuente: _zipUrl,
                sha256: sha,
                zipBytes: zipBytes,
                txtBytes: txtBytes,
                skipIfSameSha: skipIfSameSha
            );
        }

        // ===============================
        // B) IMPORTAR DESDE TXT DIRECTO
        // ===============================
        public int ImportarDesdeTxt(string txtPath, bool skipIfSameSha = true)
        {
            if (string.IsNullOrWhiteSpace(txtPath))
                throw new ArgumentException("txtPath requerido.");

            if (!File.Exists(txtPath))
                throw new FileNotFoundException("No existe el TXT.", txtPath);

            var sha = ComputeSha256(txtPath);
            var txtBytes = new FileInfo(txtPath).Length;

            return ImportarCore(
                txtPath: txtPath,
                fuente: txtPath,
                sha256: sha,
                zipBytes: 0,
                txtBytes: txtBytes,
                skipIfSameSha: skipIfSameSha
            );
        }

        // ===============================
        // CORE (común)
        // ===============================
        private int ImportarCore(string txtPath, string fuente, byte[] sha256, long zipBytes, long txtBytes, bool skipIfSameSha)
        {
            using var cn = Db.GetOpenConnection();
            using var tx = cn.BeginTransaction(IsolationLevel.Serializable);

            try
            {
                EnsureStageTable(cn, tx);

                if (skipIfSameSha)
                {
                    var activo = GetActivoDataset(cn, tx);
                    // ✅ aquí estaba el bug: antes comparabas ZipSha256 con txt-sha pero OK,
                    // solo que hay que comparar siempre contra el hash guardado, sea zip o txt.
                    if (activo != null && activo.Value.HashSha256 != null && activo.Value.HashSha256.SequenceEqual(sha256))
                    {
                        tx.Commit();
                        return activo.Value.DatasetId;
                    }
                }

                var datasetId = InsertDataset(cn, tx, fuente, sha256, zipBytes, txtBytes);

                ClearStageForDataset(cn, tx, datasetId);

                BulkToStage(cn, tx, txtPath, datasetId);

                var stageCount = GetStageCount(cn, tx, datasetId);
                if (stageCount <= 0)
                    throw new Exception("DGII: Stage quedó en 0 filas. El TXT no se está parseando correctamente.");

                MoveStageToFinal(cn, tx, datasetId);

                SwitchActivoDataset(cn, tx, datasetId);

                tx.Commit();
                return datasetId;
            }
            catch (Exception ex)
            {
                try { MarkDatasetError(cn, tx, ex.Message); } catch { }
                tx.Rollback();
                throw;
            }
        }

        // -----------------------------
        // SQL
        // -----------------------------
        private static void EnsureStageTable(SqlConnection cn, SqlTransaction tx)
        {
            using var cmd = new SqlCommand(@"
IF OBJECT_ID('dbo.DgiiRncEntryStage') IS NULL
BEGIN
    CREATE TABLE dbo.DgiiRncEntryStage(
        StageId BIGINT IDENTITY(1,1) PRIMARY KEY,
        DatasetId INT NOT NULL,
        Rnc VARCHAR(20) NOT NULL,
        Nombre NVARCHAR(300) NOT NULL,
        NombreComercial NVARCHAR(300) NULL,
        Categoria NVARCHAR(120) NULL,
        Estado NVARCHAR(40) NULL,
        Actividad NVARCHAR(300) NULL,
        FechaRegistro DATE NULL,
        Condicion NVARCHAR(40) NULL
    );
    CREATE INDEX IX_DgiiRncEntryStage_DatasetId ON dbo.DgiiRncEntryStage(DatasetId);
END", cn, tx);

            cmd.ExecuteNonQuery();
        }

        // ✅ renombré ZipSha256 a HashSha256 en retorno (es el mismo campo en DB, pero semántica correcta)
        private static (int DatasetId, byte[]? HashSha256)? GetActivoDataset(SqlConnection cn, SqlTransaction tx)
        {
            using var cmd = new SqlCommand(@"
SELECT TOP(1) DatasetId, ZipSha256
FROM dbo.DgiiRncDataset
WHERE Estado='ACTIVO'
ORDER BY DatasetId DESC;", cn, tx);

            using var rd = cmd.ExecuteReader();
            if (!rd.Read()) return null;

            return (rd.GetInt32(0), rd.IsDBNull(1) ? null : (byte[])rd[1]);
        }

        private static int InsertDataset(SqlConnection cn, SqlTransaction tx, string fuenteUrl, byte[] sha256, long zipBytes, long txtBytes)
        {
            using var cmd = new SqlCommand(@"
INSERT INTO dbo.DgiiRncDataset(FuenteUrl, ZipSha256, ZipBytes, TxtBytes, FechaDescarga, Estado, Observacion)
OUTPUT INSERTED.DatasetId
VALUES(@url, @sha, @zb, @tb, GETDATE(), 'CARGANDO', 'Import iniciado');", cn, tx);

            cmd.Parameters.Add("@url", SqlDbType.NVarChar, 600).Value = (object)fuenteUrl ?? DBNull.Value;
            cmd.Parameters.Add("@sha", SqlDbType.VarBinary, 32).Value = sha256;
            cmd.Parameters.Add("@zb", SqlDbType.BigInt).Value = zipBytes;
            cmd.Parameters.Add("@tb", SqlDbType.BigInt).Value = txtBytes;

            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        private static void MarkDatasetError(SqlConnection cn, SqlTransaction tx, string msg)
        {
            using var cmd = new SqlCommand(@"
UPDATE TOP(1) dbo.DgiiRncDataset
SET Estado='ERROR', Observacion=LEFT(@m, 600)
WHERE Estado='CARGANDO'
ORDER BY DatasetId DESC;", cn, tx);

            cmd.Parameters.Add("@m", SqlDbType.NVarChar, 600).Value = msg ?? "ERROR";
            cmd.ExecuteNonQuery();
        }

        private static void SwitchActivoDataset(SqlConnection cn, SqlTransaction tx, int newId)
        {
            using (var cmd = new SqlCommand(@"UPDATE dbo.DgiiRncDataset SET Estado='HISTORICO' WHERE Estado='ACTIVO';", cn, tx))
                cmd.ExecuteNonQuery();

            using (var cmd = new SqlCommand(@"UPDATE dbo.DgiiRncDataset SET Estado='ACTIVO', Observacion='Import OK' WHERE DatasetId=@id;", cn, tx))
            {
                cmd.Parameters.Add("@id", SqlDbType.Int).Value = newId;
                cmd.ExecuteNonQuery();
            }
        }

        private static void ClearStageForDataset(SqlConnection cn, SqlTransaction tx, int datasetId)
        {
            using var cmd = new SqlCommand(@"DELETE FROM dbo.DgiiRncEntryStage WHERE DatasetId=@id;", cn, tx);
            cmd.Parameters.Add("@id", SqlDbType.Int).Value = datasetId;
            cmd.ExecuteNonQuery();
        }

        private static int GetStageCount(SqlConnection cn, SqlTransaction tx, int datasetId)
        {
            using var cmd = new SqlCommand(@"SELECT COUNT(1) FROM dbo.DgiiRncEntryStage WHERE DatasetId=@id;", cn, tx);
            cmd.Parameters.Add("@id", SqlDbType.Int).Value = datasetId;
            return Convert.ToInt32(cmd.ExecuteScalar() ?? 0);
        }

        private static void MoveStageToFinal(SqlConnection cn, SqlTransaction tx, int datasetId)
        {
            // 1) borra lo anterior del mismo dataset
            using (var cmdDel = new SqlCommand(@"
DELETE FROM dbo.DgiiRncEntry
WHERE DatasetId = @id;", cn, tx))
            {
                cmdDel.Parameters.Add("@id", SqlDbType.Int).Value = datasetId;
                cmdDel.ExecuteNonQuery();
            }

            // 2) inserta deduplicado (una fila por RNC) + filtra basura como 'SRL'
            using (var cmdIns = new SqlCommand(@"
INSERT INTO dbo.DgiiRncEntry
(
    DatasetId,
    Rnc,
    Nombre,
    NombreComercial,
    Categoria,
    Estado,
    Actividad,
    FechaRegistro,
    Condicion
)
SELECT
    x.DatasetId,
    x.Rnc,
    x.Nombre,
    x.NombreComercial,
    x.Categoria,
    x.Estado,
    x.Actividad,
    x.FechaRegistro,
    x.Condicion
FROM
(
    SELECT
        s.DatasetId,
        s.Rnc,
        s.Nombre,
        s.NombreComercial,
        s.Categoria,
        s.Estado,
        s.Actividad,
        s.FechaRegistro,
        s.Condicion,
        ROW_NUMBER() OVER(
            PARTITION BY s.DatasetId, s.Rnc
            ORDER BY s.StageId DESC
        ) AS rn
    FROM dbo.DgiiRncEntryStage s
    WHERE s.DatasetId = @id
      AND s.Rnc IS NOT NULL
      AND LEN(LTRIM(RTRIM(s.Rnc))) >= 9  -- evita basura: 'SRL', etc
) x
WHERE x.rn = 1;", cn, tx))
            {
                cmdIns.Parameters.Add("@id", SqlDbType.Int).Value = datasetId;
                cmdIns.ExecuteNonQuery();
            }
        }

        // -----------------------------
        // BULK (✅ FIX COLID + TRUNC)
        // -----------------------------
        private static void BulkToStage(SqlConnection cn, SqlTransaction tx, string txtPath, int datasetId)
        {
            var delim = DetectDelimiter(txtPath);

            var dt = new DataTable();
            dt.Columns.Add("DatasetId", typeof(int));
            dt.Columns.Add("Rnc", typeof(string));
            dt.Columns.Add("Nombre", typeof(string));
            dt.Columns.Add("NombreComercial", typeof(string));
            dt.Columns.Add("Categoria", typeof(string));
            dt.Columns.Add("Estado", typeof(string));
            dt.Columns.Add("Actividad", typeof(string));
            dt.Columns.Add("FechaRegistro", typeof(DateTime));
            dt.Columns.Add("Condicion", typeof(string));

            using var bulk = new SqlBulkCopy(cn, SqlBulkCopyOptions.Default, tx)
            {
                DestinationTableName = "dbo.DgiiRncEntryStage",
                BatchSize = 5000,
                BulkCopyTimeout = 0
            };

            // ✅ mapeo por nombre
            bulk.ColumnMappings.Add("DatasetId", "DatasetId");
            bulk.ColumnMappings.Add("Rnc", "Rnc");
            bulk.ColumnMappings.Add("Nombre", "Nombre");
            bulk.ColumnMappings.Add("NombreComercial", "NombreComercial");
            bulk.ColumnMappings.Add("Categoria", "Categoria");
            bulk.ColumnMappings.Add("Estado", "Estado");
            bulk.ColumnMappings.Add("Actividad", "Actividad");
            bulk.ColumnMappings.Add("FechaRegistro", "FechaRegistro");
            bulk.ColumnMappings.Add("Condicion", "Condicion");

            foreach (var row in ReadDgiiRows(txtPath, delim))
            {
                // 0 RNC | 1 Nombre | 2 NombreComercial | 3 Actividad | ... | 8 Fecha | 9 Estado | 10 Condicion

                var rnc = (Safe(row, 0) ?? "").Trim();
                var nombre = (Safe(row, 1) ?? "").Trim();

                if (string.IsNullOrWhiteSpace(rnc) || string.IsNullOrWhiteSpace(nombre))
                    continue;

                var nombreCom = (Safe(row, 2) ?? "").Trim();
                var actividad = (Safe(row, 3) ?? "").Trim();
                var sFecha = (Safe(row, 8) ?? "").Trim();
                var estado = (Safe(row, 9) ?? "").Trim();
                var condicion = (Safe(row, 10) ?? "").Trim();

                // ✅ TRUNC fuerte según tus columnas
                var rncOk = Trunc(rnc, 20);
                var nombreOk = Trunc(nombre, 300);
                var nombreComOk = NullIfEmpty(Trunc(nombreCom, 300));

                var actividadOk = NullIfEmpty(Trunc(actividad, 300));

                // Categoria: si no hay, usamos actividad (recortada)
                var categoria = Trunc(actividad, 120);
                var categoriaOk = NullIfEmpty(categoria);

                var estadoOk = NullIfEmpty(Trunc(estado, 40));
                var condicionOk = NullIfEmpty(Trunc(condicion, 40));

                object fechaRegistro = DBNull.Value;
                if (!string.IsNullOrWhiteSpace(sFecha) &&
                    DateTime.TryParseExact(sFecha, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var fd))
                {
                    fechaRegistro = fd.Date;
                }

                dt.Rows.Add(
                    datasetId,
                    rncOk,
                    nombreOk,
                    nombreComOk,
                    categoriaOk,
                    estadoOk,
                    actividadOk,
                    fechaRegistro,
                    condicionOk
                );

                if (dt.Rows.Count >= 5000)
                {
                    bulk.WriteToServer(dt);
                    dt.Clear();
                }
            }

            if (dt.Rows.Count > 0)
            {
                bulk.WriteToServer(dt);
                dt.Clear();
            }
        }

        // -----------------------------
        // TXT
        // -----------------------------
        private static char DetectDelimiter(string txtPath)
        {
            using var sr = OpenTextSmart(txtPath);

            for (int i = 0; i < 10; i++)
            {
                var line = sr.ReadLine();
                if (string.IsNullOrWhiteSpace(line)) continue;

                if (line.Contains('|')) return '|';
                if (line.Contains('\t')) return '\t';
                if (line.Contains(';')) return ';';
                if (line.Contains(',')) return ',';
            }
            return '|';
        }

        private static System.Collections.Generic.IEnumerable<string[]> ReadDgiiRows(string txtPath, char delim)
        {
            // DGII usa Windows-1252 (ANSI latino)
            using var sr = new StreamReader(
                txtPath,
                Encoding.GetEncoding(1252),
                detectEncodingFromByteOrderMarks: false
            );

            string? line;
            while ((line = sr.ReadLine()) != null)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;

                var up = line.Trim().ToUpperInvariant();
                if (up.StartsWith("RNC") || up.StartsWith("ID") ||
                    (up.Contains("NOMBRE") && up.Contains("COMERCIAL")))
                    continue;

                var parts = line.Split(delim);
                for (int i = 0; i < parts.Length; i++)
                    parts[i] = Normalize(parts[i]);

                yield return parts;
            }
        }

        private static string Normalize(string? input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return "";

            var normalized = input.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder(normalized.Length);

            foreach (var ch in normalized)
            {
                var uc = Char.GetUnicodeCategory(ch);
                if (uc != UnicodeCategory.NonSpacingMark)
                    sb.Append(ch);
            }

            return sb
                .ToString()
                .Normalize(NormalizationForm.FormC)
                .Replace("�", "")   // limpieza final por seguridad
                .Trim();
        }
        private static StreamReader OpenTextSmart(string path)
        {
            // ✅ DGII a veces viene en Windows-1252 (acentos/ñ)
            try
            {
                return new StreamReader(path, Encoding.UTF8, detectEncodingFromByteOrderMarks: true);
            }
            catch
            {
                return new StreamReader(path, Encoding.GetEncoding(1252), detectEncodingFromByteOrderMarks: false);
            }
        }

        private static string Safe(string[] arr, int idx) => (idx >= 0 && idx < arr.Length) ? (arr[idx] ?? "") : "";

        private static object NullIfEmpty(string s) => string.IsNullOrWhiteSpace(s) ? DBNull.Value : s.Trim();

        private static string Trunc(string s, int max)
        {
            if (string.IsNullOrEmpty(s)) return "";
            s = s.Trim();
            return s.Length <= max ? s : s.Substring(0, max);
        }

        // -----------------------------
        // ZIP / HTTP / SHA
        // -----------------------------
        private static async System.Threading.Tasks.Task DownloadFile(string url, string path)
        {
            using var http = new HttpClient();
            using var resp = await http.GetAsync(url);
            resp.EnsureSuccessStatusCode();

            await using var fs = File.Create(path);
            await resp.Content.CopyToAsync(fs);
        }

        private static void ExtractTxtFromZip(string zipPath, string txtOutPath)
        {
            if (File.Exists(txtOutPath)) File.Delete(txtOutPath);

            using var zip = ZipFile.OpenRead(zipPath);
            var entry = zip.Entries.FirstOrDefault(e => e.Name.Equals("DGII_RNC.TXT", StringComparison.OrdinalIgnoreCase))
                        ?? zip.Entries.FirstOrDefault(e => e.Name.EndsWith(".TXT", StringComparison.OrdinalIgnoreCase));

            if (entry == null)
                throw new Exception("No se encontró DGII_RNC.TXT dentro del ZIP.");

            entry.ExtractToFile(txtOutPath, overwrite: true);
        }

        private static byte[] ComputeSha256(string filePath)
        {
            using var sha = SHA256.Create();
            using var fs = File.OpenRead(filePath);
            return sha.ComputeHash(fs);
        }
    }
}
