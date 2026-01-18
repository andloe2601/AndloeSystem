#nullable enable
using Microsoft.Reporting.WinForms;
using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace Andloe.Logica
{
    public sealed class PrintPreviewService : IPrintPreviewService
    {
        private readonly Assembly _reportAssembly;

        public PrintPreviewService(Assembly reportAssembly)
        {
            if (reportAssembly == null) throw new ArgumentNullException(nameof(reportAssembly));
            _reportAssembly = reportAssembly;
        }

        public void Preview(string rdlcRelativePath, DataTable cab, DataTable det, DataTable totales, string windowTitle)
        {
            if (string.IsNullOrWhiteSpace(rdlcRelativePath))
                throw new ArgumentException("rdlcRelativePath requerido", nameof(rdlcRelativePath));
            if (cab == null) throw new ArgumentNullException(nameof(cab));
            if (det == null) throw new ArgumentNullException(nameof(det));

            if (totales == null) totales = CreateTotalesFallback();
            EnsureTotalesHasAtLeastOneRow(totales);

            // Asegura nombres (muchos RDLC dependen de esto)
            cab.TableName = "Cab";
            det.TableName = "Det";
            totales.TableName = "Totales";

            using var f = new ReportPreviewForm();
            f.Text = string.IsNullOrWhiteSpace(windowTitle) ? "Imprimir" : windowTitle;

            f.Viewer.Reset();
            f.Viewer.ProcessingMode = ProcessingMode.Local;

            // 1) Disco (ruta BD)
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var normalized = NormalizePath(rdlcRelativePath);
            normalized = StripLeadingFolder(normalized, "Presentacion");

            var fullPath = Path.GetFullPath(Path.Combine(baseDir, normalized));

            if (File.Exists(fullPath))
            {
                f.Viewer.LocalReport.ReportPath = fullPath;
            }
            else
            {
                // 2) EmbeddedResource
                var resName = FindEmbeddedRdlcResourceName(_reportAssembly, rdlcRelativePath);

                if (resName == null)
                {
                    var disponibles = string.Join("\n- ", _reportAssembly.GetManifestResourceNames());
                    throw new FileNotFoundException(
                        "No se encontró el RDLC.\n\n" +
                        $"Ruta BD: {rdlcRelativePath}\n" +
                        $"Ruta intentada en bin: {fullPath}\n\n" +
                        "Recursos embebidos disponibles:\n- " + disponibles,
                        rdlcRelativePath
                    );
                }

                using var stream = _reportAssembly.GetManifestResourceStream(resName);
                if (stream == null)
                    throw new InvalidOperationException("Se encontró el recurso pero el stream devolvió null: " + resName);

                f.Viewer.LocalReport.LoadReportDefinition(stream);
            }

            // DataSources (una sola vez)
            f.Viewer.LocalReport.DataSources.Clear();
            f.Viewer.LocalReport.DataSources.Add(new ReportDataSource("Cab", cab));
            f.Viewer.LocalReport.DataSources.Add(new ReportDataSource("Det", det));
            f.Viewer.LocalReport.DataSources.Add(new ReportDataSource("Totales", totales));

            f.Viewer.RefreshReport();
            f.ShowDialog();
        }

        private static string NormalizePath(string p)
        {
            p = p.Trim();
            p = p.Replace('/', Path.DirectorySeparatorChar);
            p = p.Replace('\\', Path.DirectorySeparatorChar);
            return p.TrimStart(Path.DirectorySeparatorChar);
        }

        private static string StripLeadingFolder(string path, string folder)
        {
            var prefix = folder.TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;
            if (path.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                return path.Substring(prefix.Length);
            return path;
        }

        private static string? FindEmbeddedRdlcResourceName(Assembly asm, string rdlcRelativePath)
        {
            var fileName = Path.GetFileName(rdlcRelativePath);
            if (string.IsNullOrWhiteSpace(fileName)) return null;

            var all = asm.GetManifestResourceNames();

            // match por carpeta
            var found = all.FirstOrDefault(n =>
                n.EndsWith(".Reportes.Facturacion." + fileName, StringComparison.OrdinalIgnoreCase));
            if (found != null) return found;

            // fallback por filename
            return all.FirstOrDefault(n =>
                n.EndsWith("." + fileName, StringComparison.OrdinalIgnoreCase));
        }

        private static DataTable CreateTotalesFallback()
        {
            var t = new DataTable("Totales");
            t.Columns.Add("TotalCantidad", typeof(decimal));
            t.Columns.Add("Bruto", typeof(decimal));
            t.Columns.Add("Descuento", typeof(decimal));
            t.Columns.Add("ITBIS", typeof(decimal));
            t.Columns.Add("Impuesto", typeof(decimal));
            t.Columns.Add("TotalLineas", typeof(decimal));
            t.Rows.Add(0m, 0m, 0m, 0m, 0m, 0m);
            return t;
        }

        private static void EnsureTotalesHasAtLeastOneRow(DataTable totales)
        {
            if (totales.TableName != "Totales") totales.TableName = "Totales";

            EnsureDecimalColumn(totales, "TotalCantidad");
            EnsureDecimalColumn(totales, "Bruto");
            EnsureDecimalColumn(totales, "Descuento");
            EnsureDecimalColumn(totales, "ITBIS");
            EnsureDecimalColumn(totales, "Impuesto");
            EnsureDecimalColumn(totales, "TotalLineas");

            if (totales.Rows.Count == 0)
                totales.Rows.Add(0m, 0m, 0m, 0m, 0m, 0m);
        }

        private static void EnsureDecimalColumn(DataTable t, string name)
        {
            if (!t.Columns.Contains(name))
                t.Columns.Add(name, typeof(decimal));
        }
    }

    internal sealed class ReportPreviewForm : Form
    {
        public ReportViewer Viewer { get; }

        public ReportPreviewForm()
        {
            Width = 1100;
            Height = 800;
            StartPosition = FormStartPosition.CenterScreen;

            Viewer = new ReportViewer { Dock = DockStyle.Fill };
            Controls.Add(Viewer);
        }
    }
}
#nullable restore
