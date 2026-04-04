using Andloe.Data;
using Andloe.Entidad;
using ClosedXML.Excel;
using ClosedXML.Excel.Drawings;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Andloe.Presentacion
{
    public partial class FormVentasPorVendedor : Form
    {
        private readonly ReporteVentasVendedorRepository _repo = new();
        private readonly VendedorRepository _vendRepo = new();

        public FormVentasPorVendedor()
        {
            InitializeComponent();
            Wire();
            Init();
        }

        private void Wire()
        {
            Load += (_, __) => Refrescar();
            btnBuscar.Click += (_, __) => Refrescar();
            btnVerDetalle.Click += (_, __) => VerDetalle();
            btnExportarExcel.Click += (_, __) => ExportarXlsxNivelDios();
        }

        private void Init()
        {
            Text = "Ventas por Vendedor";
            dtpDesde.Value = DateTime.Today.AddDays(-30);
            dtpHasta.Value = DateTime.Today;

            var vendedores = _vendRepo.Listar(null, top: 500, incluirInactivos: false);
            var ds = vendedores
                .Select(v => new { Text = $"{v.Codigo} - {v.Nombre}", Value = v.Codigo })
                .ToList();
            ds.Insert(0, new { Text = "(Todos)", Value = "" });

            cboVendedor.DisplayMember = "Text";
            cboVendedor.ValueMember = "Value";
            cboVendedor.DataSource = ds;
            cboVendedor.SelectedValue = "";

            grid.AutoGenerateColumns = false;
            grid.ReadOnly = true;
            grid.AllowUserToAddRows = false;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.MultiSelect = false;

            grid.Columns.Clear();
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "CodVendedor", HeaderText = "Código", Width = 90, Name = "colCod" });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Vendedor", HeaderText = "Vendedor", Width = 220, Name = "colVen" });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "CantFacturas", HeaderText = "Facturas", Width = 80, Name = "colFact" });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "SubTotal", HeaderText = "SubTotal", Width = 110, Name = "colSub", DefaultCellStyle = { Format = "N2" } });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Descuento", HeaderText = "Descuento", Width = 110, Name = "colDesc", DefaultCellStyle = { Format = "N2" } });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Itbis", HeaderText = "ITBIS", Width = 110, Name = "colItbis", DefaultCellStyle = { Format = "N2" } });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "TotalGeneral", HeaderText = "Total", Width = 120, Name = "colTot", DefaultCellStyle = { Format = "N2" } });
        }

        private string? GetCodVend()
        {
            var v = Convert.ToString(cboVendedor.SelectedValue) ?? "";
            v = v.Trim();
            return string.IsNullOrWhiteSpace(v) ? null : v;
        }

        private void Refrescar()
        {
            try
            {
                var desde = dtpDesde.Value.Date;
                var hasta = dtpHasta.Value.Date;
                var codVend = GetCodVend();

                var data = _repo.Resumen(desde, hasta, codVend);
                grid.DataSource = data;

                txtTotFacturas.Text = data.Sum(x => x.CantFacturas).ToString("N0");
                txtTotMonto.Text = data.Sum(x => x.TotalGeneral).ToString("N2");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ventas por vendedor", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ============================================================
        // ✅ EXPORT XLSX NIVEL DIOS (estable, sin corrupción)
        // ============================================================
        private void ExportarXlsxNivelDios()
        {
            try
            {
                if (grid.Rows.Count <= 0)
                {
                    MessageBox.Show("No hay datos para exportar.", "Exportar XLSX", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                using var sfd = new SaveFileDialog
                {
                    Title = "Exportar a Excel (Nivel Dios)",
                    Filter = "Excel (*.xlsx)|*.xlsx",
                    FileName = $"VentasPorVendedor_{dtpDesde.Value:yyyyMMdd}_{dtpHasta.Value:yyyyMMdd}.xlsx",
                    OverwritePrompt = true
                };

                if (sfd.ShowDialog(this) != DialogResult.OK)
                    return;

                var desde = dtpDesde.Value.Date;
                var hasta = dtpHasta.Value.Date;
                var codVend = GetCodVend();

                var resumen = grid.DataSource as List<VentasPorVendedorRowDto>;
                if (resumen == null)
                {
                    resumen = new List<VentasPorVendedorRowDto>();
                    foreach (DataGridViewRow r in grid.Rows)
                    {
                        if (r.IsNewRow) continue;
                        resumen.Add(new VentasPorVendedorRowDto
                        {
                            CodVendedor = Convert.ToString(r.Cells["colCod"].Value) ?? "",
                            Vendedor = Convert.ToString(r.Cells["colVen"].Value) ?? "",
                            CantFacturas = SafeInt(r.Cells["colFact"].Value),
                            SubTotal = SafeDec(r.Cells["colSub"].Value),
                            Descuento = SafeDec(r.Cells["colDesc"].Value),
                            Itbis = SafeDec(r.Cells["colItbis"].Value),
                            TotalGeneral = SafeDec(r.Cells["colTot"].Value),
                        });
                    }
                }

                var detalle = _repo.Detalle(desde, hasta, codVend);

                var top10 = resumen
                    .OrderByDescending(x => x.TotalGeneral)
                    .Take(10)
                    .ToList();

                // Generar imagen del chart (TOP 10)
                var chartPng = BuildTop10ChartPng(top10, $"TOP 10 - Total vendido ({desde:yyyy-MM-dd} a {hasta:yyyy-MM-dd})");

                using (var wb = new XLWorkbook())
                {
                    // =====================
                    // HOJA RESUMEN
                    // =====================
                    var ws = wb.Worksheets.Add("Resumen");
                    ws.Style.Font.FontName = "Segoe UI";
                    ws.Style.Font.FontSize = 10;

                    ws.Cell("A1").Value = "Ventas por Vendedor";
                    ws.Range("A1:G1").Merge();
                    ws.Cell("A1").Style.Font.Bold = true;
                    ws.Cell("A1").Style.Font.FontSize = 16;

                    ws.Cell("A2").Value = $"Desde: {desde:yyyy-MM-dd}   Hasta: {hasta:yyyy-MM-dd}   |   Vendedor: {(string.IsNullOrWhiteSpace(codVend) ? "Todos" : codVend)}";
                    ws.Range("A2:G2").Merge();
                    ws.Cell("A2").Style.Font.FontColor = XLColor.Gray;

                    int hdrRow = 4;
                    string[] headers = { "Código", "Vendedor", "Facturas", "SubTotal", "Descuento", "ITBIS", "Total" };
                    for (int i = 0; i < headers.Length; i++) ws.Cell(hdrRow, i + 1).Value = headers[i];

                    var hdr = ws.Range(hdrRow, 1, hdrRow, 7);
                    hdr.Style.Font.Bold = true;
                    hdr.Style.Fill.BackgroundColor = XLColor.FromHtml("#F5F6F8");
                    hdr.Style.Border.BottomBorder = XLBorderStyleValues.Thin;

                    int row = hdrRow + 1;
                    foreach (var x in resumen)
                    {
                        ws.Cell(row, 1).Value = x.CodVendedor;
                        ws.Cell(row, 2).Value = x.Vendedor;
                        ws.Cell(row, 3).Value = x.CantFacturas;
                        ws.Cell(row, 4).Value = x.SubTotal;
                        ws.Cell(row, 5).Value = x.Descuento;
                        ws.Cell(row, 6).Value = x.Itbis;
                        ws.Cell(row, 7).Value = x.TotalGeneral;
                        row++;
                    }

                    ws.Column(3).Style.NumberFormat.Format = "0";
                    ws.Columns(4, 7).Style.NumberFormat.Format = "#,##0.00";
                    ws.Columns(4, 7).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                    int totalRow = row + 1;
                    ws.Cell(totalRow, 2).Value = "TOTALES";
                    ws.Cell(totalRow, 2).Style.Font.Bold = true;

                    ws.Cell(totalRow, 3).FormulaA1 = $"=SUM(C{hdrRow + 1}:C{row - 1})";
                    ws.Cell(totalRow, 4).FormulaA1 = $"=SUM(D{hdrRow + 1}:D{row - 1})";
                    ws.Cell(totalRow, 5).FormulaA1 = $"=SUM(E{hdrRow + 1}:E{row - 1})";
                    ws.Cell(totalRow, 6).FormulaA1 = $"=SUM(F{hdrRow + 1}:F{row - 1})";
                    ws.Cell(totalRow, 7).FormulaA1 = $"=SUM(G{hdrRow + 1}:G{row - 1})";

                    ws.Range(totalRow, 2, totalRow, 7).Style.Fill.BackgroundColor = XLColor.FromHtml("#EEF5FF");
                    ws.Range(totalRow, 2, totalRow, 7).Style.Font.Bold = true;
                    ws.Range(totalRow, 2, totalRow, 7).Style.Border.TopBorder = XLBorderStyleValues.Thin;

                    // TOP 10 tabla (I-K)
                    ws.Cell("I1").Value = "TOP 10 Vendedores";
                    ws.Range("I1:K1").Merge();
                    ws.Cell("I1").Style.Font.Bold = true;
                    ws.Cell("I1").Style.Font.FontSize = 12;

                    ws.Cell("I3").Value = "Vendedor";
                    ws.Cell("J3").Value = "Código";
                    ws.Cell("K3").Value = "Total";
                    ws.Range("I3:K3").Style.Font.Bold = true;
                    ws.Range("I3:K3").Style.Fill.BackgroundColor = XLColor.FromHtml("#F5F6F8");

                    int tRow = 4;
                    foreach (var t in top10)
                    {
                        ws.Cell(tRow, 9).Value = t.Vendedor;
                        ws.Cell(tRow, 10).Value = t.CodVendedor;
                        ws.Cell(tRow, 11).Value = t.TotalGeneral;
                        tRow++;
                    }
                    ws.Range($"K4:K{tRow - 1}").Style.NumberFormat.Format = "#,##0.00";
                    ws.Range($"K4:K{tRow - 1}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                    ws.SheetView.FreezeRows(hdrRow);
                    ws.Columns(1, 7).AdjustToContents();
                    ws.Columns(9, 11).AdjustToContents();

                    // ✅ INSERTAR CHART COMO IMAGEN (cero corrupción)
                    if (!string.IsNullOrWhiteSpace(chartPng) && File.Exists(chartPng))
                    {
                        var pic = ws.AddPicture(chartPng)
                            .MoveTo(ws.Cell("M3"))
                            .WithSize(620, 360);

                        // borde suave (look moderno)
                        pic.Placement = XLPicturePlacement.FreeFloating;
                    }

                    // =====================
                    // HOJA DETALLE
                    // =====================
                    var ws2 = wb.Worksheets.Add("Detalle");
                    ws2.Style.Font.FontName = "Segoe UI";
                    ws2.Style.Font.FontSize = 10;

                    ws2.Cell("A1").Value = "Detalle de Facturas (FAC FINALIZADA)";
                    ws2.Range("A1:G1").Merge();
                    ws2.Cell("A1").Style.Font.Bold = true;
                    ws2.Cell("A1").Style.Font.FontSize = 14;

                    ws2.Cell("A2").Value = $"Desde: {desde:yyyy-MM-dd}   Hasta: {hasta:yyyy-MM-dd}   |   Vendedor: {(string.IsNullOrWhiteSpace(codVend) ? "Todos" : codVend)}";
                    ws2.Range("A2:G2").Merge();
                    ws2.Cell("A2").Style.Font.FontColor = XLColor.Gray;

                    int dHdr = 4;
                    string[] dh = { "FacturaId", "Número", "Fecha", "Vendedor", "Cliente", "Total", "Estado" };
                    for (int i = 0; i < dh.Length; i++) ws2.Cell(dHdr, i + 1).Value = dh[i];

                    var dhead = ws2.Range(dHdr, 1, dHdr, 7);
                    dhead.Style.Font.Bold = true;
                    dhead.Style.Fill.BackgroundColor = XLColor.FromHtml("#F5F6F8");
                    dhead.Style.Border.BottomBorder = XLBorderStyleValues.Thin;

                    int dr = dHdr + 1;
                    foreach (var d in detalle)
                    {
                        ws2.Cell(dr, 1).Value = d.FacturaId;
                        ws2.Cell(dr, 2).Value = d.NumeroDocumento;
                        ws2.Cell(dr, 3).Value = d.FechaDocumento;
                        ws2.Cell(dr, 4).Value = string.IsNullOrWhiteSpace(d.Vendedor) ? d.CodVendedor : d.Vendedor;
                        ws2.Cell(dr, 5).Value = d.Cliente;
                        ws2.Cell(dr, 6).Value = d.TotalGeneral;
                        ws2.Cell(dr, 7).Value = d.Estado;
                        dr++;
                    }

                    ws2.Column(3).Style.DateFormat.Format = "yyyy-MM-dd";
                    ws2.Column(6).Style.NumberFormat.Format = "#,##0.00";
                    ws2.Column(6).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                    ws2.SheetView.FreezeRows(dHdr);
                    ws2.Columns(1, 7).AdjustToContents();

                    wb.SaveAs(sfd.FileName);
                }

                // borrar temp png
                try { if (!string.IsNullOrWhiteSpace(chartPng) && File.Exists(chartPng)) File.Delete(chartPng); } catch { }

                MessageBox.Show("Exportado XLSX (Nivel Dios) ✅📈", "Exportar XLSX", MessageBoxButtons.OK, MessageBoxIcon.Information);

                try
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = sfd.FileName,
                        UseShellExecute = true
                    });
                }
                catch { }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Exportar XLSX", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ============================================================
        // ✅ Chart PNG (TOP10) -> estable 100%
        // ============================================================
        private static string? BuildTop10ChartPng(List<VentasPorVendedorRowDto> top10, string title)
        {
            try
            {
                if (top10 == null || top10.Count == 0) return null;

                var tmp = Path.Combine(Path.GetTempPath(), $"Top10Ventas_{Guid.NewGuid():N}.png");

                using var chart = new Chart();
                chart.Width = 1000;
                chart.Height = 520;

                var area = new ChartArea("A");
                area.AxisX.Interval = 1;
                area.AxisX.MajorGrid.Enabled = false;
                area.AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Dash;
                area.AxisY.LabelStyle.Format = "N2";
                area.BackColor = Color.White;
                chart.ChartAreas.Add(area);

                chart.Titles.Add(title);

                var series = new Series("Total")
                {
                    ChartType = SeriesChartType.Column,
                    XValueType = ChartValueType.String,
                    YValueType = ChartValueType.Double,
                    IsValueShownAsLabel = true,
                    LabelFormat = "N2"
                };

                foreach (var r in top10)
                {
                    var name = string.IsNullOrWhiteSpace(r.Vendedor) ? r.CodVendedor : r.Vendedor;
                    var p = series.Points.AddY((double)r.TotalGeneral);
                    series.Points[p].AxisLabel = Trunc(name, 18);
                    series.Points[p].ToolTip = $"{name} - {r.TotalGeneral:N2}";
                }

                chart.Series.Add(series);

                var legend = new Legend();
                legend.Enabled = false;
                chart.Legends.Add(legend);

                chart.SaveImage(tmp, ChartImageFormat.Png);
                return tmp;
            }
            catch
            {
                return null;
            }
        }

        private static string Trunc(string s, int max)
        {
            s ??= "";
            s = s.Trim();
            return s.Length <= max ? s : s.Substring(0, max - 1) + "…";
        }

        private static int SafeInt(object? v)
        {
            try { return Convert.ToInt32(v); } catch { return 0; }
        }

        private static decimal SafeDec(object? v)
        {
            try { return Convert.ToDecimal(v); } catch { return 0m; }
        }

        // ============================================================
        // DETALLE
        // ============================================================
        private void VerDetalle()
        {
            try
            {
                var desde = dtpDesde.Value.Date;
                var hasta = dtpHasta.Value.Date;

                string? codVend = null;
                if (grid.CurrentRow?.DataBoundItem is VentasPorVendedorRowDto row && !string.IsNullOrWhiteSpace(row.CodVendedor))
                    codVend = row.CodVendedor;
                else
                    codVend = GetCodVend();

                var det = _repo.Detalle(desde, hasta, codVend);

                using var f = new Form
                {
                    Text = $"Detalle ventas {(string.IsNullOrWhiteSpace(codVend) ? "(Todos)" : codVend)}",
                    Width = 1050,
                    Height = 650,
                    StartPosition = FormStartPosition.CenterParent
                };

                var g = new DataGridView
                {
                    Dock = DockStyle.Fill,
                    ReadOnly = true,
                    AllowUserToAddRows = false,
                    AutoGenerateColumns = false,
                    SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                    MultiSelect = false
                };

                g.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "FacturaId", HeaderText = "FacturaId", Width = 80 });
                g.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "NumeroDocumento", HeaderText = "Número", Width = 140 });
                g.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "FechaDocumento", HeaderText = "Fecha", Width = 120, DefaultCellStyle = { Format = "yyyy-MM-dd" } });
                g.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Cliente", HeaderText = "Cliente", Width = 260 });
                g.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "TotalGeneral", HeaderText = "Total", Width = 120, DefaultCellStyle = { Format = "N2" } });
                g.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Estado", HeaderText = "Estado", Width = 110 });

                g.DataSource = det;

                var bottom = new Panel { Dock = DockStyle.Bottom, Height = 42 };
                var btnAbrir = new Button { Text = "Abrir Factura", Left = 10, Top = 8, Width = 140 };
                btnAbrir.Click += (_, __) =>
                {
                    if (g.CurrentRow?.DataBoundItem is VentasPorVendedorDetalleDto d)
                    {
                        using var ff = new FormFacturaV(d.FacturaId);
                        ff.ShowDialog(f);
                    }
                };
                bottom.Controls.Add(btnAbrir);

                f.Controls.Add(g);
                f.Controls.Add(bottom);
                f.ShowDialog(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Detalle", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}