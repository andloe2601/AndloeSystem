#nullable enable
using Andloe.Data;
using Microsoft.Reporting.WinForms;
using QRCoder;
using System;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Windows.Forms;

namespace Andloe.Presentacion
{
    public partial class FormReporteFacturaRI : Form
    {
        private readonly int _facturaId;
        private readonly ReporteFacturacionRepository _repo = new();

        // Cambia según ambiente
        private const string DGII_HOST_PROD = "https://ecf.dgii.gov.do/ecf/consultatimbre";
        private const string DGII_HOST_CERT = "https://ecf.dgii.gov.do/certecf/consultatimbre";
        private const string DGII_HOST_TEST = "https://ecf.dgii.gov.do/testecf/consultatimbre";

        private const string DGII_HOST_ACTUAL = DGII_HOST_PROD;

        // ✅ ponlo true para ver el nombre exacto del recurso la primera vez
        private const bool DEBUG = false;

        public FormReporteFacturaRI(int facturaId)
        {
            _facturaId = facturaId;
            InitializeComponent();
            Shown += (_, __) => Cargar();
        }

        private void Cargar()
        {
            try
            {
                // ===========================
                // 1) DATA
                // ===========================
                var ds = _repo.GetFacturaRI(_facturaId);

                if (!ds.Tables.Contains("Cab") || ds.Tables["Cab"].Rows.Count == 0)
                    throw new InvalidOperationException("La factura no devolvió Cab (cabecera).");

                if (!ds.Tables.Contains("Det"))
                    throw new InvalidOperationException("La factura no devolvió Det (detalle).");

                // ✅ Garantiza Totales SIEMPRE (RDLC lo requiere)
                AsegurarTotales(ds);

                var cab = ds.Tables["Cab"];
                var det = ds.Tables["Det"];
                var tot = ds.Tables["Totales"];

                // QR en cab (RDLC espera QRImage)
                AsegurarColumnaQrImage(cab);
                PrepararQrEnCab(cab.Rows[0]);

                // ===========================
                // 2) RDLC embebido
                // ===========================
                reportViewer1.Reset();
                reportViewer1.ProcessingMode = ProcessingMode.Local;

                // ✅ Usa el assembly donde está el Form (correcto para recursos embebidos)
                var asm = typeof(FormReporteFacturaRI).Assembly;
                var resources = asm.GetManifestResourceNames();

                if (DEBUG)
                {
                    MessageBox.Show(
                        "Assembly: " + asm.FullName + "\n\nRecursos embebidos:\n- " + string.Join("\n- ", resources),
                        "DEBUG RDLC",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }

                // ✅ Buscar EXACTO por la ruta de tu carpeta:
                // Presentacion/Reportes/Facturacion/FacturaRI.rdlc
                // El embedded suele quedar ...Reportes.Facturacion.FacturaRI.rdlc
                var rdlcName = resources.FirstOrDefault(n =>
                    n.EndsWith(".Reportes.Facturacion.FacturaRI.rdlc", StringComparison.OrdinalIgnoreCase));

                // Fallback: cualquier cosa que termine en FacturaRI.rdlc (por si el root namespace cambió)
                rdlcName ??= resources.FirstOrDefault(n =>
                    n.EndsWith(".FacturaRI.rdlc", StringComparison.OrdinalIgnoreCase));

                if (rdlcName == null)
                {
                    throw new System.IO.FileNotFoundException(
                        "No se encontró el RDLC embebido (FacturaRI.rdlc). " +
                        "Verifica: Acción de compilación = Recurso incrustado.\n\n" +
                        "Recursos encontrados:\n- " + string.Join("\n- ", resources)
                    );
                }

                using var stream = asm.GetManifestResourceStream(rdlcName);
                if (stream == null)
                    throw new InvalidOperationException("Se encontró el nombre del RDLC pero no se pudo abrir el stream: " + rdlcName);

                reportViewer1.LocalReport.LoadReportDefinition(stream);

                // ===========================
                // 3) DataSources (EXACTOS del RDLC)
                // ===========================
                reportViewer1.LocalReport.DataSources.Clear();

                // OJO: en tu RDLC el dataset de cabecera está como "cab" (minúscula)
                reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("cab", cab));
                reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("Det", det));
                reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("Totales", tot));

                reportViewer1.RefreshReport();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.ToString(),
                    "Imprimir - Error real",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        // ===========================
        // Totales: siempre presente
        // ===========================
        private static void AsegurarTotales(DataSet ds)
        {
            if (!ds.Tables.Contains("Totales"))
            {
                var t = CrearTablaTotales();
                t.Rows.Add(0m, 0m, 0m, 0m, 0m, 0m);
                ds.Tables.Add(t);
                return;
            }

            var tot = ds.Tables["Totales"];

            AsegurarColumnaDecimal(tot, "TotalCantidad");
            AsegurarColumnaDecimal(tot, "Bruto");
            AsegurarColumnaDecimal(tot, "Descuento");
            AsegurarColumnaDecimal(tot, "ITBIS");
            AsegurarColumnaDecimal(tot, "Impuesto");
            AsegurarColumnaDecimal(tot, "TotalLineas");

            if (tot.Rows.Count == 0)
                tot.Rows.Add(0m, 0m, 0m, 0m, 0m, 0m);
        }

        private static DataTable CrearTablaTotales()
        {
            var t = new DataTable("Totales");
            t.Columns.Add("TotalCantidad", typeof(decimal));
            t.Columns.Add("Bruto", typeof(decimal));
            t.Columns.Add("Descuento", typeof(decimal));
            t.Columns.Add("ITBIS", typeof(decimal));
            t.Columns.Add("Impuesto", typeof(decimal));
            t.Columns.Add("TotalLineas", typeof(decimal));
            return t;
        }

        private static void AsegurarColumnaDecimal(DataTable t, string name)
        {
            if (!t.Columns.Contains(name))
                t.Columns.Add(name, typeof(decimal));
        }

        // ===========================
        // QR -> QRImage (byte[])
        // ===========================
        private static void AsegurarColumnaQrImage(DataTable cab)
        {
            if (!cab.Columns.Contains("QRImage"))
                cab.Columns.Add("QRImage", typeof(byte[]));
        }

        private static void PrepararQrEnCab(DataRow row)
        {
            string GetS(string c) =>
                row.Table.Columns.Contains(c) && row[c] != DBNull.Value
                    ? (Convert.ToString(row[c]) ?? "").Trim()
                    : "";

            DateTime GetDt(string c)
            {
                if (!row.Table.Columns.Contains(c) || row[c] == DBNull.Value) return DateTime.Now;
                if (row[c] is DateTime d) return d;
                return DateTime.TryParse(Convert.ToString(row[c]), out var x) ? x : DateTime.Now;
            }

            decimal GetDec(string c)
            {
                if (!row.Table.Columns.Contains(c) || row[c] == DBNull.Value) return 0m;
                try { return Convert.ToDecimal(row[c], CultureInfo.InvariantCulture); }
                catch { return 0m; }
            }

            var rncEmisor = SoloDigitos(GetS("EmpresaRNC"));
            var rncComprador = SoloDigitos(GetS("DocumentoCliente"));
            var encf = GetS("eNCF");
            var codigoSeg = GetS("CodigoSeguridad");

            var fechaEmision = GetDt("FechaDocumento").Date;
            var fechaFirma = GetDt("FechaVencimiento");
            var montoTotal = GetDec("TotalGeneral");

            var qs = HttpUtility.ParseQueryString(string.Empty);
            qs["rncemisor"] = rncEmisor;
            if (!string.IsNullOrWhiteSpace(rncComprador)) qs["rnccomprador"] = rncComprador;
            qs["encf"] = encf;
            qs["fechaemision"] = fechaEmision.ToString("dd-MM-yyyy", CultureInfo.InvariantCulture);
            qs["montototal"] = montoTotal.ToString("0.00", CultureInfo.InvariantCulture);
            qs["fechafirma"] = fechaFirma.ToString("dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture);
            qs["codigoseguridad"] = codigoSeg;

            var qrUrl = DGII_HOST_ACTUAL + "?" + qs.ToString();
            row["QRImage"] = GenerarQrPng(qrUrl);
        }

        private static string SoloDigitos(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return "";
            var buf = new StringBuilder(s.Length);
            foreach (var ch in s)
                if (char.IsDigit(ch)) buf.Append(ch);
            return buf.ToString();
        }

        private static byte[] GenerarQrPng(string texto)
        {
            using var gen = new QRCodeGenerator();
            using var data = gen.CreateQrCode(texto, QRCodeGenerator.ECCLevel.Q);
            var qr = new PngByteQRCode(data);
            return qr.GetGraphic(8);
        }
    }
}
#nullable restore
