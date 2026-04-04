using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Andloe.Entidad.CxC;
using Andloe.Logica;

namespace Andloe.Presentacion
{
    public partial class FormCxCEstadoCuenta : Form
    {
        private readonly CxCEstadoCuentaService _service = new();

        private List<CxCClienteLookupDto> _clientes = new();
        private List<CxCEstadoCuentaDto> _items = new();

        private readonly PrintDocument _printDocument = new();
        private readonly PrintPreviewDialog _printPreview = new();

        private int _printRowIndex;
        private string _clienteActual = "";

        private int _printPageNumber;
        private string _empresaNombre = "ANDLOE SYSTEM";

        public FormCxCEstadoCuenta()
        {
            InitializeComponent();

            Load += FormLoad;
            btnBuscarCliente.Click += (_, __) => BuscarClientes();
            btnConsultar.Click += (_, __) => Consultar();
            btnExportarExcel.Click += (_, __) => ExportarExcel();
            btnImprimir.Click += (_, __) => Imprimir();

            _printDocument.DefaultPageSettings.Landscape = true;
            _printDocument.DefaultPageSettings.Margins = new Margins(30, 30, 35, 35);
            _printDocument.OriginAtMargins = true;

            _printDocument.BeginPrint += PrintDocument_BeginPrint;
            _printDocument.PrintPage += PrintDocument_PrintPage;

            _printPreview.Document = _printDocument;
            _printPreview.Width = 1200;
            _printPreview.Height = 850;
        }

        private void FormLoad(object? sender, EventArgs e)
        {
            dtpDesde.Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            dtpHasta.Value = DateTime.Today;

            lblTotalFacturadoValor.Text = FormatearMoneda(0m);
            lblTotalCobradoValor.Text = FormatearMoneda(0m);
            lblBalanceValor.Text = FormatearMoneda(0m);

            ConfigurarGrid();
            BuscarClientes();
        }

        private void ConfigurarGrid()
        {
            grid.AutoGenerateColumns = false;
            grid.EnableHeadersVisualStyles = false;
            grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(245, 247, 250);
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);

            grid.DefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(230, 240, 255);
            grid.DefaultCellStyle.SelectionForeColor = Color.Black;

            grid.RowTemplate.Height = 28;
            grid.AllowUserToResizeRows = false;
            grid.MultiSelect = false;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(250, 250, 250);

            grid.Columns["colFecha"].Width = 95;
            grid.Columns["colTipo"].Width = 90;
            grid.Columns["colDocumento"].Width = 130;
            grid.Columns["colReferencia"].Width = 150;
            grid.Columns["colDebe"].Width = 110;
            grid.Columns["colHaber"].Width = 110;
            grid.Columns["colBalance"].Width = 110;
            grid.Columns["colDias"].Width = 95;
            grid.Columns["colDesc"].Width = 340;

            grid.Columns["colDesc"].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            grid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
        }

        private void BuscarClientes()
        {
            try
            {
                _clientes = _service.BuscarClientes(txtBuscarCliente.Text.Trim(), 30);

                cboCliente.DataSource = null;
                cboCliente.DataSource = _clientes;
                cboCliente.DisplayMember = nameof(CxCClienteLookupDto.Nombre);
                cboCliente.ValueMember = nameof(CxCClienteLookupDto.ClienteId);

                if (_clientes.Count > 0)
                    cboCliente.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Estado de cuenta", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Consultar()
        {
            try
            {
                var cli = cboCliente.SelectedItem as CxCClienteLookupDto;
                if (cli == null)
                    throw new InvalidOperationException("Debes seleccionar un cliente.");

                _clienteActual = cli.Nombre;

                _items = _service.ListarEstadoCuentaCliente(
                    cli.ClienteId,
                    dtpDesde.Value.Date,
                    dtpHasta.Value.Date);

                grid.Rows.Clear();

                foreach (var item in _items)
                {
                    int row = grid.Rows.Add(
                        item.Fecha.ToString("dd/MM/yyyy"),
                        item.TipoMovimiento,
                        item.Documento,
                        item.Referencia ?? "",
                        FormatearMoneda(item.Debe),
                        FormatearMoneda(item.Haber),
                        FormatearMoneda(item.BalanceAcumulado),
                        item.DiasVencidos <= 0 ? "" : item.DiasVencidos.ToString(),
                        item.Descripcion ?? ""
                    );

                    if (item.TipoMovimiento == "FACTURA" && item.DiasVencidos > 0)
                    {
                        grid.Rows[row].DefaultCellStyle.BackColor = Color.FromArgb(255, 244, 244);
                        grid.Rows[row].Cells["colDias"].Style.ForeColor = Color.Firebrick;
                        grid.Rows[row].Cells["colDias"].Style.Font = new Font(grid.Font, FontStyle.Bold);
                    }

                    if (item.TipoMovimiento == "COBRO")
                    {
                        grid.Rows[row].DefaultCellStyle.BackColor = Color.FromArgb(245, 250, 245);
                    }
                }

                var totales = _service.CalcularTotales(_items);
                lblTotalFacturadoValor.Text = FormatearMoneda(totales.totalFacturado);
                lblTotalCobradoValor.Text = FormatearMoneda(totales.totalCobrado);
                lblBalanceValor.Text = FormatearMoneda(totales.balance);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Estado de cuenta", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ExportarExcel()
        {
            try
            {
                if (_items == null || _items.Count == 0)
                    throw new InvalidOperationException("No hay datos para exportar.");

                using var sfd = new SaveFileDialog
                {
                    Filter = "Excel 97-2003 (*.xls)|*.xls",
                    FileName = $"EstadoCuenta_{SanitizarNombreArchivo(_clienteActual)}_{DateTime.Now:yyyyMMdd_HHmmss}.xls",
                    Title = "Exportar estado de cuenta"
                };

                if (sfd.ShowDialog(this) != DialogResult.OK)
                    return;

                var html = ConstruirHtmlExcel();
                File.WriteAllText(sfd.FileName, html, Encoding.UTF8);

                MessageBox.Show(
                    "Archivo exportado correctamente.",
                    "Estado de cuenta",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Exportar Excel", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string ConstruirHtmlExcel()
        {
            var sb = new StringBuilder();

            sb.AppendLine("<html>");
            sb.AppendLine("<head>");
            sb.AppendLine("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\" />");
            sb.AppendLine("<style>");
            sb.AppendLine("body{font-family:Segoe UI,Arial;font-size:12px;}");
            sb.AppendLine("table{border-collapse:collapse;width:100%;}");
            sb.AppendLine("th,td{border:1px solid #999;padding:6px;}");
            sb.AppendLine("th{background:#eef2f7;font-weight:bold;}");
            sb.AppendLine(".num{text-align:right;}");
            sb.AppendLine(".title{font-size:18px;font-weight:bold;margin-bottom:8px;}");
            sb.AppendLine(".sub{margin-bottom:12px;color:#444;}");
            sb.AppendLine(".tot{font-weight:bold;background:#f8f8f8;}");
            sb.AppendLine("</style>");
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");

            sb.AppendLine($"<div class='title'>Estado de Cuenta por Cliente</div>");
            sb.AppendLine($"<div class='sub'>Cliente: {System.Net.WebUtility.HtmlEncode(_clienteActual)}<br/>");
            sb.AppendLine($"Desde: {dtpDesde.Value:dd/MM/yyyy} &nbsp;&nbsp; Hasta: {dtpHasta.Value:dd/MM/yyyy}</div>");

            sb.AppendLine("<table>");
            sb.AppendLine("<tr>");
            sb.AppendLine("<th>Fecha</th>");
            sb.AppendLine("<th>Tipo</th>");
            sb.AppendLine("<th>Documento</th>");
            sb.AppendLine("<th>Referencia</th>");
            sb.AppendLine("<th>Debe</th>");
            sb.AppendLine("<th>Haber</th>");
            sb.AppendLine("<th>Balance</th>");
            sb.AppendLine("<th>Días vencidos</th>");
            sb.AppendLine("<th>Descripción</th>");
            sb.AppendLine("</tr>");

            foreach (var item in _items)
            {
                sb.AppendLine("<tr>");
                sb.AppendLine($"<td>{item.Fecha:dd/MM/yyyy}</td>");
                sb.AppendLine($"<td>{System.Net.WebUtility.HtmlEncode(item.TipoMovimiento)}</td>");
                sb.AppendLine($"<td>{System.Net.WebUtility.HtmlEncode(item.Documento)}</td>");
                sb.AppendLine($"<td>{System.Net.WebUtility.HtmlEncode(item.Referencia ?? "")}</td>");
                sb.AppendLine($"<td class='num'>{item.Debe:N2}</td>");
                sb.AppendLine($"<td class='num'>{item.Haber:N2}</td>");
                sb.AppendLine($"<td class='num'>{item.BalanceAcumulado:N2}</td>");
                sb.AppendLine($"<td class='num'>{(item.DiasVencidos <= 0 ? "" : item.DiasVencidos.ToString())}</td>");
                sb.AppendLine($"<td>{System.Net.WebUtility.HtmlEncode(item.Descripcion ?? "")}</td>");
                sb.AppendLine("</tr>");
            }

            sb.AppendLine("</table>");
            sb.AppendLine("<br/>");
            sb.AppendLine("<table style='width:420px'>");
            sb.AppendLine("<tr class='tot'><td>Total facturado</td><td class='num'>" + _items.Sum(x => x.Debe).ToString("N2") + "</td></tr>");
            sb.AppendLine("<tr class='tot'><td>Total cobrado</td><td class='num'>" + _items.Sum(x => x.Haber).ToString("N2") + "</td></tr>");
            sb.AppendLine("<tr class='tot'><td>Balance actual</td><td class='num'>" + (_items.Sum(x => x.Debe) - _items.Sum(x => x.Haber)).ToString("N2") + "</td></tr>");
            sb.AppendLine("</table>");

            sb.AppendLine("</body>");
            sb.AppendLine("</html>");

            return sb.ToString();
        }

        private void Imprimir()
        {
            try
            {
                if (_items == null || _items.Count == 0)
                    throw new InvalidOperationException("No hay datos para imprimir.");

                _printDocument.DefaultPageSettings.Landscape = true;
                _printDocument.DefaultPageSettings.Margins = new Margins(30, 30, 35, 35);

                _printPreview.Document = _printDocument;
                _printPreview.ShowDialog(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Imprimir", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PrintDocument_BeginPrint(object? sender, PrintEventArgs e)
        {
            _printRowIndex = 0;
            _printPageNumber = 1;
        }

        private void PrintDocument_PrintPage(object? sender, PrintPageEventArgs e)
        {
            var g = e.Graphics;
            var marginLeft = e.MarginBounds.Left;
            var marginTop = e.MarginBounds.Top;
            var pageWidth = e.MarginBounds.Width;
            var pageHeight = e.MarginBounds.Height;

            using var fontEmpresa = new Font("Segoe UI", 14, FontStyle.Bold);
            using var fontTitulo = new Font("Segoe UI", 12, FontStyle.Bold);
            using var fontSub = new Font("Segoe UI", 9, FontStyle.Regular);
            using var fontHeader = new Font("Segoe UI", 8.5f, FontStyle.Bold);
            using var fontBody = new Font("Segoe UI", 8.2f, FontStyle.Regular);
            using var fontFooter = new Font("Segoe UI", 8, FontStyle.Regular);

            int y = marginTop;

            // ===== ENCABEZADO =====
            g.DrawString(_empresaNombre, fontEmpresa, Brushes.Black, marginLeft, y);
            y += 24;

            g.DrawString("ESTADO DE CUENTA POR CLIENTE", fontTitulo, Brushes.Black, marginLeft, y);
            y += 22;

            g.DrawString(
                $"Cliente: {_clienteActual}",
                fontSub,
                Brushes.Black,
                marginLeft,
                y);

            g.DrawString(
                $"Desde: {dtpDesde.Value:dd/MM/yyyy}    Hasta: {dtpHasta.Value:dd/MM/yyyy}",
                fontSub,
                Brushes.Black,
                marginLeft + 350,
                y);

            g.DrawString(
                $"Impreso: {DateTime.Now:dd/MM/yyyy HH:mm}",
                fontSub,
                Brushes.Black,
                marginLeft + 690,
                y);

            y += 24;

            // Línea separadora encabezado
            g.DrawLine(Pens.Black, marginLeft, y, marginLeft + pageWidth, y);
            y += 8;

            // ===== ANCHOS DE COLUMNAS =====
            int[] widths =
            {
        78,   // Fecha
        78,   // Tipo
        110,  // Documento
        120,  // Referencia
        88,   // Debe
        88,   // Haber
        92,   // Balance
        72,   // Días
        pageWidth - (78 + 78 + 110 + 120 + 88 + 88 + 92 + 72) // Descripción
    };

            string[] headers =
            {
        "Fecha", "Tipo", "Documento", "Referencia", "Debe", "Haber", "Balance", "Días", "Descripción"
    };

            // ===== HEADERS =====
            int x = marginLeft;
            for (int i = 0; i < headers.Length; i++)
            {
                g.FillRectangle(Brushes.LightGray, x, y, widths[i], 24);
                g.DrawRectangle(Pens.Black, x, y, widths[i], 24);

                var sfHeader = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };

                g.DrawString(headers[i], fontHeader, Brushes.Black,
                    new RectangleF(x + 2, y + 2, widths[i] - 4, 20), sfHeader);

                x += widths[i];
            }

            y += 24;

            // ===== DETALLE =====
            while (_printRowIndex < _items.Count)
            {
                var item = _items[_printRowIndex];

                string[] vals =
                {
            item.Fecha.ToString("dd/MM/yyyy"),
            item.TipoMovimiento,
            item.Documento,
            item.Referencia ?? "",
            item.Debe.ToString("N2"),
            item.Haber.ToString("N2"),
            item.BalanceAcumulado.ToString("N2"),
            item.DiasVencidos <= 0 ? "" : item.DiasVencidos.ToString(),
            item.Descripcion ?? ""
        };

                int rowHeight = 30;

                if (y + rowHeight > marginTop + pageHeight - 80)
                {
                    DibujarFooterImpresion(g, fontFooter, marginLeft, marginTop, pageWidth, pageHeight);
                    e.HasMorePages = true;
                    _printPageNumber++;
                    return;
                }

                x = marginLeft;

                for (int i = 0; i < vals.Length; i++)
                {
                    g.DrawRectangle(Pens.Black, x, y, widths[i], rowHeight);

                    var sf = new StringFormat
                    {
                        Alignment = (i >= 4 && i <= 7) ? StringAlignment.Far : StringAlignment.Near,
                        LineAlignment = StringAlignment.Center,
                        Trimming = StringTrimming.EllipsisCharacter
                    };

                    RectangleF rect = new RectangleF(x + 3, y + 2, widths[i] - 6, rowHeight - 4);

                    Brush brush = Brushes.Black;
                    if (i == 7 && item.DiasVencidos > 0)
                        brush = Brushes.Firebrick;

                    g.DrawString(vals[i], fontBody, brush, rect, sf);

                    x += widths[i];
                }

                y += rowHeight;
                _printRowIndex++;
            }

            // ===== TOTALES =====
            y += 10;

            decimal totalFacturado = _items.Sum(xi => xi.Debe);
            decimal totalCobrado = _items.Sum(xi => xi.Haber);
            decimal balance = totalFacturado - totalCobrado;

            int totX = marginLeft + pageWidth - 320;
            int totW1 = 170;
            int totW2 = 150;
            int totH = 24;

            DibujarFilaTotal(g, fontHeader, totX, y, totW1, totW2, totH, "Total facturado", FormatearMoneda(totalFacturado));
            y += totH;
            DibujarFilaTotal(g, fontHeader, totX, y, totW1, totW2, totH, "Total cobrado", FormatearMoneda(totalCobrado));
            y += totH;
            DibujarFilaTotal(g, fontHeader, totX, y, totW1, totW2, totH, "Balance actual", FormatearMoneda(balance));

            // ===== FOOTER =====
            DibujarFooterImpresion(g, fontFooter, marginLeft, marginTop, pageWidth, pageHeight);

            e.HasMorePages = false;
        }

        private static string SanitizarNombreArchivo(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                return "Cliente";

            foreach (char c in Path.GetInvalidFileNameChars())
                nombre = nombre.Replace(c, '_');

            return nombre.Replace(" ", "_").Trim();
        }

        private static string FormatearMoneda(decimal valor)
            => $"RD$ {valor:N2}";
    

private void DibujarFilaTotal(Graphics g, Font font, int x, int y, int w1, int w2, int h, string texto, string valor)
        {
            g.FillRectangle(Brushes.Gainsboro, x, y, w1, h);
            g.FillRectangle(Brushes.WhiteSmoke, x + w1, y, w2, h);

            g.DrawRectangle(Pens.Black, x, y, w1, h);
            g.DrawRectangle(Pens.Black, x + w1, y, w2, h);

            var sfLeft = new StringFormat
            {
                Alignment = StringAlignment.Near,
                LineAlignment = StringAlignment.Center
            };

            var sfRight = new StringFormat
            {
                Alignment = StringAlignment.Far,
                LineAlignment = StringAlignment.Center
            };

            g.DrawString(texto, font, Brushes.Black, new RectangleF(x + 6, y + 2, w1 - 12, h - 4), sfLeft);
            g.DrawString(valor, font, Brushes.Black, new RectangleF(x + w1 + 6, y + 2, w2 - 12, h - 4), sfRight);
        }

        private void DibujarFooterImpresion(Graphics g, Font fontFooter, int marginLeft, int marginTop, int pageWidth, int pageHeight)
        {
            int footerY = marginTop + pageHeight + 8;

            g.DrawLine(Pens.Gray, marginLeft, footerY - 4, marginLeft + pageWidth, footerY - 4);

            g.DrawString(
                $"Página {_printPageNumber}",
                fontFooter,
                Brushes.Black,
                marginLeft,
                footerY);

            g.DrawString(
                "Documento generado por Andloe ERP",
                fontFooter,
                Brushes.Gray,
                marginLeft + pageWidth - 180,
                footerY);
        }
    }
}