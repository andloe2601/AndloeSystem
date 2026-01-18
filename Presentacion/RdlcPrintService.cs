#nullable enable
using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Microsoft.Reporting.WinForms;
using Andloe.Logica;

namespace Andloe.Presentacion.Impresion
{
    public sealed class RdlcPrintService : IPrintPreviewService
    {
        private static readonly Assembly ReportAssembly = typeof(RdlcPrintService).Assembly;

        public void Preview(string rdlcRelativePath, DataTable cab, DataTable det, DataTable totales, string windowTitle)
        {
            if (string.IsNullOrWhiteSpace(rdlcRelativePath))
                throw new ArgumentNullException(nameof(rdlcRelativePath));

            if (cab == null) throw new ArgumentNullException(nameof(cab));
            if (det == null) throw new ArgumentNullException(nameof(det));

            totales ??= CreateTotalesFallback();
            EnsureTotalesHasAtLeastOneRow(totales);

            using var f = new Form
            {
                Text = string.IsNullOrWhiteSpace(windowTitle) ? "Vista previa" : windowTitle,
                StartPosition = FormStartPosition.CenterScreen,
                Width = 1100,
                Height = 750
            };

            var viewer = new ReportViewer { Dock = DockStyle.Fill };
            viewer.ProcessingMode = ProcessingMode.Local;

            viewer.Reset();
            viewer.LocalReport.DataSources.Clear();

            LoadRdlcDefinition(viewer, rdlcRelativePath);

            // Estos nombres DEBEN coincidir con el RDLC:
            viewer.LocalReport.DataSources.Add(new ReportDataSource("Cab", cab));
            viewer.LocalReport.DataSources.Add(new ReportDataSource("Det", det));
            viewer.LocalReport.DataSources.Add(new ReportDataSource("Totales", totales));

            viewer.RefreshReport();

            f.Controls.Add(viewer);
            f.ShowDialog();
        }

        private static void LoadRdlcDefinition(ReportViewer viewer, string rdlcRelativePath)
        {
            var rdlcPath = ResolveRdlcPathFromDb(rdlcRelativePath);

            if (File.Exists(rdlcPath))
            {
                viewer.LocalReport.ReportPath = rdlcPath;
                return;
            }

            var resName = FindEmbeddedRdlcResourceName(ReportAssembly, rdlcRelativePath);

            if (resName == null)
            {
                var disponibles = string.Join("\n- ", ReportAssembly.GetManifestResourceNames());
                throw new FileNotFoundException(
                    "No se encontró el RDLC.\n\n" +
                    $"Ruta BD: {rdlcRelativePath}\n" +
                    $"Ruta intentada en bin: {rdlcPath}\n\n" +
                    "Recursos embebidos disponibles:\n- " + disponibles
                );
            }

            using var stream = ReportAssembly.GetManifestResourceStream(resName);
            if (stream == null)
                throw new InvalidOperationException("Se encontró el recurso pero el stream devolvió null: " + resName);

            viewer.LocalReport.LoadReportDefinition(stream);
        }

        private static string ResolveRdlcPathFromDb(string relativeFromDb)
        {
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var normalized = NormalizePath(relativeFromDb);
            normalized = StripLeadingFolder(normalized, "Presentacion");
            return Path.GetFullPath(Path.Combine(baseDir, normalized));
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

            // intenta el patrón Reportes.Facturacion.<file>.rdlc
            var found = all.FirstOrDefault(n =>
                n.EndsWith(".Reportes.Facturacion." + fileName, StringComparison.OrdinalIgnoreCase));

            if (found != null) return found;

            // fallback: cualquier recurso que termine con el filename
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
            if (totales.TableName != "Totales")
                totales.TableName = "Totales";

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
}
#nullable restore
