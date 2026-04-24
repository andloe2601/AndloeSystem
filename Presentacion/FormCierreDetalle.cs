using Andloe.Entidad;
using Andloe.Logica;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Windows.Forms;


namespace Andloe.Presentacion
{
    public partial class FormCierreDetalle : Form
    {
        private readonly long _cierreId;
        private readonly bool _isDesigner;
        private readonly CierreCajaService _service = new();
        private CierreCaja? _cierre;  // encabezado del cierre
        private PrintDocument? _printDoc;
        private Image? _logoEmpresa;

        // Para el diseñador (no usar manualmente)
        public FormCierreDetalle()
        {
            _isDesigner = true;
            InitializeComponent();
            ConfigurarGridVentas();
            ConfigurarGridPagos();
            dgvPagos.CellFormatting += dgvPagos_CellFormatting;
        }

        // Constructor real que se llama desde FormCierresHistorico
        public FormCierreDetalle(long cierreId)
        {
            _cierreId = cierreId;
            _isDesigner = false;
            InitializeComponent();
            Load += FormCierreDetalle_Load;
            ConfigurarGridVentas();
            ConfigurarGridPagos();
            dgvPagos.CellFormatting += dgvPagos_CellFormatting;

            try
            {
                var ruta = @"C:\Andloe\logo.png"; // o desde ConfigService
                if (System.IO.File.Exists(ruta))
                {
                    _logoEmpresa = Image.FromFile(ruta);
                }
            }
            catch
            {
                _logoEmpresa = null;
            }
        }
        // ---------------------------------------------------
        //  CONFIGURACIÓN GRID VENTAS
        // ---------------------------------------------------
        private void ConfigurarGridVentas()
        {
            // Dejamos AutoGenerateColumns = true para que se cree desde el DataTable
            dgvVentas.AutoGenerateColumns = true;
            dgvVentas.ReadOnly = true;
            dgvVentas.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvVentas.MultiSelect = false;
            dgvVentas.RowHeadersVisible = false;
        }

        // ---------------------------------------------------
        //  CONFIGURACIÓN GRID PAGOS (Icono + columnas)
        // ---------------------------------------------------
        private void ConfigurarGridPagos()
        {
            dgvPagos.AutoGenerateColumns = false;
            dgvPagos.Columns.Clear();

            // Columna de icono NO enlazada
            var colIcono = new DataGridViewImageColumn
            {
                Name = "Icono",
                HeaderText = "",
                Width = 32,
                ImageLayout = DataGridViewImageCellLayout.Zoom
            };
            dgvPagos.Columns.Add(colIcono);

            dgvPagos.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "FormaPagoCodigo",
                DataPropertyName = "FormaPagoCodigo",
                HeaderText = "Código"
            });

            dgvPagos.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "MedioPago",
                DataPropertyName = "MedioPago",
                HeaderText = "Medio de Pago",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            });

            dgvPagos.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Total",
                DataPropertyName = "Total",
                HeaderText = "Total",
                DefaultCellStyle =
                {
                    Alignment = DataGridViewContentAlignment.MiddleRight,
                    Format = "N2"
                },
                Width = 110
            });

            dgvPagos.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Cantidad",
                DataPropertyName = "Cantidad",
                HeaderText = "Transacciones",
                Width = 110
            });

            dgvPagos.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Porcentaje",
                DataPropertyName = "Porcentaje",
                HeaderText = "%",
                DefaultCellStyle =
                {
                    Alignment = DataGridViewContentAlignment.MiddleRight,
                    Format = "N2"
                },
                Width = 80
            });

            dgvPagos.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvPagos.MultiSelect = false;
            dgvPagos.ReadOnly = true;
            dgvPagos.RowHeadersVisible = false;
        }

        // ---------------------------------------------------
        //  LOAD
        // ---------------------------------------------------
        private void FormCierreDetalle_Load(object? sender, EventArgs e)
        {
            if (_isDesigner)
                return;

            try
            {
                CargarResumenCierre();  // por CierreId

                if (_cierre == null)
                {
                    MessageBox.Show("No se encontró información del cierre.",
                        "Detalle de Cierre", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    Close();
                    return;
                }

                CargarVentas();   // VentaCab por rango y caja del cierre
                CargarPagos();    // POS_Pago WHERE CierreId = @CierreId (agrupado por medio)
                CargarFondo();    // POS_FondoCaja WHERE CierreId = @CierreId
                CargarTotales();  // Totales desde CierreCaja
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar el detalle del cierre: " + ex.Message,
                    "Detalle de Cierre", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ---------------------------------------------------
        //  RESUMEN / VENTAS
        // ---------------------------------------------------
        private void CargarResumenCierre()
        {
            var lista = _service.BuscarCierres(null, null, null, null, null);
            _cierre = lista.FirstOrDefault(c => c.CierreId == _cierreId);

            if (_cierre == null)
                return;

            lblCierreIdValor.Text = _cierre.CierreId.ToString();
            lblCajaValor.Text = _cierre.POS_CajaNumero ?? "";
            lblUsuarioValor.Text = _cierre.UsuarioCierre ?? "";
            lblEstadoValor.Text = _cierre.Estado ?? "";
            lblRangoFechasValor.Text =
                $"{_cierre.FechaDesde:dd/MM/yyyy HH:mm} - {_cierre.FechaHasta:dd/MM/yyyy HH:mm}";
        }

        private void CargarVentas()
        {
            DataTable dt = _service.ObtenerVentasPorCierre(_cierreId); // usa CierreId en el repo
            dgvVentas.DataSource = dt;

            if (dgvVentas.Columns.Contains("TerminoPago"))
                dgvVentas.Columns["TerminoPago"].HeaderText = "Término de pago";

            if (dt == null)
                return;

            // Ajustar encabezados básicos
            if (dgvVentas.Columns["VentaId"] is not null)
                dgvVentas.Columns["VentaId"].HeaderText = "Venta Id";

            if (dgvVentas.Columns["NoDocumento"] is not null)
                dgvVentas.Columns["NoDocumento"].HeaderText = "No. Documento";

            if (dgvVentas.Columns["Fecha"] is not null)
                dgvVentas.Columns["Fecha"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";

            if (dgvVentas.Columns["SubTotalMoneda"] is not null)
            {
                dgvVentas.Columns["SubTotalMoneda"].HeaderText = "SubTotal";
                dgvVentas.Columns["SubTotalMoneda"].DefaultCellStyle.Format = "N2";
                dgvVentas.Columns["SubTotalMoneda"].DefaultCellStyle.Alignment =
                    DataGridViewContentAlignment.MiddleRight;
            }

            if (dgvVentas.Columns["ItbisMoneda"] is not null)
            {
                dgvVentas.Columns["ItbisMoneda"].HeaderText = "Impuesto";
                dgvVentas.Columns["ItbisMoneda"].DefaultCellStyle.Format = "N2";
                dgvVentas.Columns["ItbisMoneda"].DefaultCellStyle.Alignment =
                    DataGridViewContentAlignment.MiddleRight;
            }

            if (dgvVentas.Columns["TotalMoneda"] is not null)
            {
                dgvVentas.Columns["TotalMoneda"].HeaderText = "Total";
                dgvVentas.Columns["TotalMoneda"].DefaultCellStyle.Format = "N2";
                dgvVentas.Columns["TotalMoneda"].DefaultCellStyle.Alignment =
                    DataGridViewContentAlignment.MiddleRight;
            }

            // --- MEDIO DE PAGO AFECTADO ---
            if (dgvVentas.Columns["MedioPago"] is not null)
            {
                dgvVentas.Columns["MedioPago"].HeaderText = "Medio de Pago";
            }
            else if (dgvVentas.Columns["MedioPagoId"] is not null)
            {
                dgvVentas.Columns["MedioPagoId"].HeaderText = "Medio Pago";
            }

            if (dgvVentas.Columns["MontoPago"] is not null)
            {
                dgvVentas.Columns["MontoPago"].HeaderText = "Monto Pago";
                dgvVentas.Columns["MontoPago"].DefaultCellStyle.Format = "N2";
                dgvVentas.Columns["MontoPago"].DefaultCellStyle.Alignment =
                    DataGridViewContentAlignment.MiddleRight;
            }
        }

        // ---------------------------------------------------
        //  PAGOS / MEDIOS (AGRUPADO)
        // ---------------------------------------------------
        private void CargarPagos()
        {
            var lista = _service.ListarPagosPosPorCierre(_cierreId);

            if (lista == null || lista.Count == 0)
            {
                dgvPagos.DataSource = null;
                return;
            }

            // 1) AGRUPAR PAGOS POR FORMA DE PAGO
            var grupos = lista
                .GroupBy(x => new { x.FormaPagoCodigo, x.MedioPagoNombre })
                .Select(g => new
                {
                    FormaPagoCodigo = g.Key.FormaPagoCodigo,
                    MedioPago = g.Key.MedioPagoNombre,
                    Total = g.Sum(x => x.Monto),
                    Cantidad = g.Count()
                })
                .OrderByDescending(x => x.Total)
                .ToList();

            decimal totalGeneral = grupos.Sum(x => x.Total);

            // 2) DATA PARA EL GRID
            var dt = new DataTable();
            dt.Columns.Add("FormaPagoCodigo", typeof(string));
            dt.Columns.Add("MedioPago", typeof(string));
            dt.Columns.Add("Total", typeof(decimal));
            dt.Columns.Add("Cantidad", typeof(int));
            dt.Columns.Add("Porcentaje", typeof(decimal));
            dt.Columns.Add("EsTotal", typeof(bool));

            foreach (var g in grupos)
            {
                decimal porcentaje = totalGeneral > 0 ? (g.Total * 100m / totalGeneral) : 0m;

                dt.Rows.Add(
                    g.FormaPagoCodigo,
                    g.MedioPago,
                    g.Total,
                    g.Cantidad,
                    porcentaje,
                    false
                );
            }

            // 3) FILA TOTAL GENERAL
            if (grupos.Count > 0)
            {
                dt.Rows.Add(
                    DBNull.Value,
                    "TOTAL GENERAL",
                    totalGeneral,
                    grupos.Sum(x => x.Cantidad),
                    100m,
                    true
                );
            }

            dgvPagos.DataSource = dt;
        }
        // ---------------------------------------------------
        //  FONDO / TOTALES
        // ---------------------------------------------------
        private void CargarFondo()
        {
            DataTable dt = _service.ObtenerFondoPorCierre(_cierreId);  // POS_FondoCaja.CierreId
            dgvFondo.DataSource = dt;
        }

        private void CargarTotales()
        {
            if (_cierre == null)
                return;

            lblTotalVentasValor.Text = _cierre.TotalVentas.ToString("N2");
            lblTotalPagosValor.Text = _cierre.TotalPagos.ToString("N2");
            lblEfectivoTeoricoValor.Text = _cierre.EfectivoTeorico.ToString("N2");
            lblEfectivoDeclaradoValor.Text = _cierre.EfectivoDeclarado.ToString("N2");
            lblDiferenciaValor.Text = _cierre.Diferencia.ToString("N2");
        }

        // ---------------------------------------------------
        //  ICONO POR MEDIO (para la columna no enlazada)
        // ---------------------------------------------------
        private Image ObtenerIconoPorMedio(string? medioPagoNombre)
        {
            string nombre = (medioPagoNombre ?? string.Empty).ToUpperInvariant();

            if (nombre.Contains("EFECTIVO"))
                return SystemIcons.Shield.ToBitmap();        // efectivo

            if (nombre.Contains("TARJETA") || nombre.Contains("CRÉDITO") || nombre.Contains("CREDITO"))
                return SystemIcons.Information.ToBitmap();   // tarjeta

            if (nombre.Contains("TRANSFER"))
                return SystemIcons.Warning.ToBitmap();       // transferencia

            if (nombre.Contains("CHEQUE"))
                return SystemIcons.Question.ToBitmap();      // cheque

            // genérico
            return SystemIcons.Application.ToBitmap();
        }

        // ---------------------------------------------------
        //  FORMATEO (COLORES, ICONO, %)
        // ---------------------------------------------------
        private void dgvPagos_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (dgvPagos.Rows[e.RowIndex].DataBoundItem is not DataRowView drv) return;

            bool esTotal = drv.Row.Field<bool>("EsTotal");
            string? medio = drv.Row.Field<string>("MedioPago");
            string medioUpper = (medio ?? string.Empty).ToUpperInvariant();
            var row = dgvPagos.Rows[e.RowIndex];

            // ----- ICONO -----
            if (dgvPagos.Columns[e.ColumnIndex].Name == "Icono" && !esTotal)
            {
                e.Value = ObtenerIconoPorMedio(medio);
                e.FormattingApplied = true;
            }

            // ----- FILA TOTAL GENERAL -----
            if (esTotal)
            {
                row.DefaultCellStyle.BackColor = Color.FromArgb(30, 144, 255);
                row.DefaultCellStyle.ForeColor = Color.White;
                row.DefaultCellStyle.Font = new Font(
                    row.InheritedStyle.Font ?? dgvPagos.Font,
                    FontStyle.Bold
                );

                if (dgvPagos.Columns[e.ColumnIndex].Name == "Porcentaje" && e.Value is decimal decTotal)
                {
                    e.Value = decTotal.ToString("N2") + " %";
                    e.FormattingApplied = true;
                }

                return;
            }

            // ----- COLORES POR MEDIO -----
            if (medioUpper.Contains("EFECTIVO"))
            {
                row.DefaultCellStyle.BackColor = Color.FromArgb(232, 245, 233);
                row.DefaultCellStyle.ForeColor = Color.FromArgb(27, 94, 32);
            }
            else if (medioUpper.Contains("TARJETA") || medioUpper.Contains("CRÉDITO") || medioUpper.Contains("CREDITO"))
            {
                row.DefaultCellStyle.BackColor = Color.FromArgb(227, 242, 253);
                row.DefaultCellStyle.ForeColor = Color.FromArgb(21, 101, 192);
            }
            else if (medioUpper.Contains("TRANSFER"))
            {
                row.DefaultCellStyle.BackColor = Color.FromArgb(248, 249, 229);
                row.DefaultCellStyle.ForeColor = Color.FromArgb(130, 119, 23);
            }
            else
            {
                row.DefaultCellStyle.BackColor = Color.White;
                row.DefaultCellStyle.ForeColor = Color.Black;
            }

            // Formato del porcentaje: "xx.xx %"
            if (dgvPagos.Columns[e.ColumnIndex].Name == "Porcentaje" && e.Value is decimal dec)
            {
                e.Value = dec.ToString("N2") + " %";
                e.FormattingApplied = true;
            }
        }

        private void PrintDoc_PrintPage(object? sender, PrintPageEventArgs e)
        {
            if (_cierre == null)
            {
                e.HasMorePages = false;
                return;
            }

            var g = e.Graphics;
            float x = e.MarginBounds.Left;
            float y = e.MarginBounds.Top;

            using var fontTitulo = new Font("Segoe UI", 14, FontStyle.Bold);
            using var fontSeccion = new Font("Segoe UI", 10, FontStyle.Bold);
            using var fontTexto = new Font("Segoe UI", 9, FontStyle.Regular);
            using var penLinea = new Pen(Color.Gray, 1);

            // ====== TÍTULO ======
            g.DrawString("CIERRE DE CAJA", fontTitulo, Brushes.Black, x, y);
            y += fontTitulo.GetHeight(g) + 10;

            // ====== ENCABEZADO ======
            g.DrawString($"Cierre ID: {_cierre.CierreId}", fontTexto, Brushes.Black, x, y);
            y += fontTexto.GetHeight(g) + 2;

            g.DrawString($"Caja: {_cierre.POS_CajaNumero}", fontTexto, Brushes.Black, x, y);
            y += fontTexto.GetHeight(g) + 2;

            g.DrawString($"Usuario cierre: {_cierre.UsuarioCierre}", fontTexto, Brushes.Black, x, y);
            y += fontTexto.GetHeight(g) + 2;

            g.DrawString(
                $"Rango: {_cierre.FechaDesde:dd/MM/yyyy HH:mm} - {_cierre.FechaHasta:dd/MM/yyyy HH:mm}",
                fontTexto,
                Brushes.Black,
                x,
                y);
            y += fontTexto.GetHeight(g) + 2;

            g.DrawString($"Fecha cierre: {_cierre.FechaCierre:dd/MM/yyyy HH:mm}", fontTexto, Brushes.Black, x, y);
            y += fontTexto.GetHeight(g) + 8;

            g.DrawLine(penLinea, x, y, e.MarginBounds.Right, y);
            y += 8;

            // ====== TOTALES GENERALES ======
            g.DrawString("Totales generales", fontSeccion, Brushes.Black, x, y);
            y += fontSeccion.GetHeight(g) + 4;

            g.DrawString($"Total ventas      : {_cierre.TotalVentas:N2}", fontTexto, Brushes.Black, x, y);
            y += fontTexto.GetHeight(g) + 2;

            g.DrawString($"Total pagos       : {_cierre.TotalPagos:N2}", fontTexto, Brushes.Black, x, y);
            y += fontTexto.GetHeight(g) + 2;

            g.DrawString($"Fondo inicial     : {_cierre.FondoInicial:N2}", fontTexto, Brushes.Black, x, y);
            y += fontTexto.GetHeight(g) + 2;

            g.DrawString($"Efectivo teórico  : {_cierre.EfectivoTeorico:N2}", fontTexto, Brushes.Black, x, y);
            y += fontTexto.GetHeight(g) + 2;

            g.DrawString($"Efectivo declarado: {_cierre.EfectivoDeclarado:N2}", fontTexto, Brushes.Black, x, y);
            y += fontTexto.GetHeight(g) + 2;

            g.DrawString($"Diferencia        : {_cierre.Diferencia:N2}", fontTexto, Brushes.Black, x, y);
            y += fontTexto.GetHeight(g) + 8;

            g.DrawLine(penLinea, x, y, e.MarginBounds.Right, y);
            y += 8;

            // ====== RESUMEN POR MEDIO DE PAGO ======
            g.DrawString("Resumen por medio de pago", fontSeccion, Brushes.Black, x, y);
            y += fontSeccion.GetHeight(g) + 4;

            var pagos = _service.ListarPagosPosPorCierre(_cierreId) ?? new System.Collections.Generic.List<CierrePagoPosDetalleDto>();
            decimal totalGeneralPagos = pagos.Sum(p => p.Monto);

            // Encabezado de “tabla”
            float colMedio = x;
            float colMonto = x + 260;
            float colPorc = x + 380;

            g.DrawString("Medio de pago", fontTexto, Brushes.Black, colMedio, y);
            g.DrawString("Monto", fontTexto, Brushes.Black, colMonto, y);
            g.DrawString("%", fontTexto, Brushes.Black, colPorc, y);
            y += fontTexto.GetHeight(g) + 4;

            g.DrawLine(penLinea, x, y, e.MarginBounds.Right, y);
            y += 4;

            foreach (var grp in pagos.GroupBy(p => p.MedioPagoNombre ?? "N/D"))
            {
                decimal total = grp.Sum(p => p.Monto);
                decimal pct = totalGeneralPagos != 0 ? (total / totalGeneralPagos) * 100m : 0m;

                if (y > e.MarginBounds.Bottom - 40)
                {
                    // Si se llena la página, puedes paginar
                    e.HasMorePages = true;
                    return;
                }

                g.DrawString(grp.Key, fontTexto, Brushes.Black, colMedio, y);
                g.DrawString(total.ToString("N2"), fontTexto, Brushes.Black, colMonto, y);
                g.DrawString(pct.ToString("N1") + " %", fontTexto, Brushes.Black, colPorc, y);
                y += fontTexto.GetHeight(g) + 2;
            }

            y += 6;
            g.DrawLine(penLinea, x, y, e.MarginBounds.Right, y);
            y += 8;

            // Total general de pagos
            g.DrawString($"Total pagos (todas las formas): {totalGeneralPagos:N2}",
                fontTexto, Brushes.Black, x, y);

            e.HasMorePages = false;
        }


       


        // ---------------------------------------------------
        //  IMPRIMIR
        // ---------------------------------------------------
        private void btnImprimir_Click(object? sender, EventArgs e)
        {
            try
            {
                if (_cierre == null)
                {
                    MessageBox.Show(
                        "No hay información de cierre cargada para imprimir.",
                        "Imprimir cierre",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    return;
                }

                _printDoc = new PrintDocument();
                _printDoc.DocumentName = $"Cierre_{_cierre.CierreId}";

                // 👉 FORMATO TICKET (vertical, márgenes pequeños)
                _printDoc.DefaultPageSettings.Landscape = false;
                _printDoc.DefaultPageSettings.Margins = new Margins(5, 5, 5, 5);

                // OPCIONAL: si tu impresora térmica soporta tamaño personalizado,
                // puedes descomentar esto (80mm aprox):
                /*
                _printDoc.DefaultPageSettings.PaperSize = new PaperSize(
                    "Ticket80mm",
                    300,   // ancho (centésimas de pulgada)
                    1100   // alto aprox. (ajustable)
                );
                */

                // 🔹 Aquí enganchamos el método de ticket
                _printDoc.PrintPage += PrintDoc_PrintPage_Ticket;

                using (var preview = new PrintPreviewDialog())
                {
                    preview.Document = _printDoc;
                    preview.WindowState = FormWindowState.Maximized;
                    preview.ShowIcon = false;
                    preview.Text = $"Vista previa - Cierre {_cierre.CierreId}";
                    preview.ShowDialog(this);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al preparar la impresión del cierre: " + ex.Message,
                    "Imprimir cierre",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void PrintDoc_PrintPage_Ticket(object? sender, PrintPageEventArgs e)
        {
            if (_cierre == null)
            {
                e.HasMorePages = false;
                return;
            }

            var g = e.Graphics;
            float x = e.MarginBounds.Left;
            float y = e.MarginBounds.Top;
            float ancho = e.MarginBounds.Width;

            using var fontTitulo = new Font("Segoe UI", 11, FontStyle.Bold);
            using var fontNormal = new Font("Segoe UI", 8.5f, FontStyle.Regular);
            using var fontNegrita = new Font("Segoe UI", 8.5f, FontStyle.Bold);
            using var penLinea = new Pen(Color.Black, 1);

            // ===== Helpers =====
            void DrawCentered(string texto, Font font)
            {
                var size = g.MeasureString(texto, font);
                float xx = x + (ancho - size.Width) / 2;
                g.DrawString(texto, font, Brushes.Black, xx, y);
                y += size.Height;
            }

            void DrawLine()
            {
                g.DrawLine(penLinea, x, y, x + ancho, y);
                y += 3;
            }

            void DrawLabelValue(string label, string value, bool bold = false)
            {
                var font = bold ? fontNegrita : fontNormal;
                string texto = $"{label} {value}";
                g.DrawString(texto, font, Brushes.Black, x, y);
                y += font.GetHeight(g);
            }

            // ====== LOGO (si lo cargaste en _logoEmpresa) ======
            if (_logoEmpresa != null)
            {
                float logoAncho = Math.Min(ancho * 0.8f, _logoEmpresa.Width);
                float escala = logoAncho / _logoEmpresa.Width;
                float logoAlto = _logoEmpresa.Height * escala;

                float lx = x + (ancho - logoAncho) / 2f;
                g.DrawImage(_logoEmpresa, lx, y, logoAncho, logoAlto);
                y += logoAlto + 4;
            }

            // ====== CABECERA / ENCABEZADO ======
            DrawCentered("CIERRE DE CAJA", fontTitulo);
            y += 2;

            DrawCentered($"Cierre: {_cierre.CierreId}", fontNormal);
            DrawCentered($"Caja:   {_cierre.POS_CajaNumero}", fontNormal);
            DrawCentered($"Usuario: {_cierre.UsuarioCierre}", fontNormal);

            y += 2;
            DrawCentered($"{_cierre.FechaCierre:dd/MM/yyyy HH:mm}", fontNormal);

            y += 4;
            DrawLine();

            // ====== RANGO DE OPERACIÓN ======
            DrawLabelValue("Desde:", $"{_cierre.FechaDesde:dd/MM/yyyy HH:mm}");
            DrawLabelValue("Hasta:", $"{_cierre.FechaHasta:dd/MM/yyyy HH:mm}");

            y += 2;
            DrawLine();

            // ====== TOTALES GENERALES ======
            DrawCentered("RESUMEN GENERAL", fontNegrita);
            y += 2;

            DrawLabelValue("Ventas  :", _cierre.TotalVentas.ToString("N2"), true);
            DrawLabelValue("Pagos   :", _cierre.TotalPagos.ToString("N2"));
            DrawLabelValue("Fondo   :", _cierre.FondoInicial.ToString("N2"));
            DrawLabelValue("Teórico :", _cierre.EfectivoTeorico.ToString("N2"));
            DrawLabelValue("Declarad:", _cierre.EfectivoDeclarado.ToString("N2"));
            DrawLabelValue("Dif.    :", _cierre.Diferencia.ToString("N2"), true);

            y += 3;
            DrawLine();

            // ====== RESUMEN POR MEDIO DE PAGO ======
            DrawCentered("MEDIOS DE PAGO", fontNegrita);
            y += 2;

            var pagos = _service.ListarPagosPosPorCierre(_cierreId)
                       ?? new List<CierrePagoPosDetalleDto>();

            decimal totalGeneralPagos = pagos.Sum(p => p.Monto);

            // Encabezado “tabla” simple
            string header = "Medio        Monto     %";
            g.DrawString(header, fontNegrita, Brushes.Black, x, y);
            y += fontNegrita.GetHeight(g);
            DrawLine();

            foreach (var grp in pagos.GroupBy(p => p.MedioPagoNombre ?? "N/D"))
            {
                decimal total = grp.Sum(p => p.Monto);
                decimal pct = totalGeneralPagos != 0
                    ? (total / totalGeneralPagos) * 100m
                    : 0m;

                // Dejamos espacio para firmas al final
                if (y > e.MarginBounds.Bottom - 60)
                {
                    e.HasMorePages = true;
                    return;
                }

                // "EFECTIVO    123.45   80.0%"
                string medio = grp.Key;
                if (medio.Length > 10)
                    medio = medio.Substring(0, 10);

                string montoStr = total.ToString("N2");
                string pctStr = pct.ToString("N1") + "%";

                string linea = medio.PadRight(11);
                linea += montoStr.PadLeft(8);
                linea += pctStr.PadLeft(6);

                g.DrawString(linea, fontNormal, Brushes.Black, x, y);
                y += fontNormal.GetHeight(g);
            }

            y += 3;
            DrawLine();

            DrawLabelValue("Total pagos:", totalGeneralPagos.ToString("N2"), true);

            y += 6;
            DrawCentered("GRACIAS", fontNegrita);
            y += 10;

            DrawLine();
            y += 10;

            // ====== ÁREA DE FIRMAS ======

            // Firma Cajero
            g.DrawLine(penLinea, x, y, x + ancho, y);
            y += 2;
            DrawCentered("Firma Cajero", fontNormal);
            y += 12;

            // Firma Supervisor
            g.DrawLine(penLinea, x, y, x + ancho, y);
            y += 2;
            DrawCentered("Firma Supervisor", fontNormal);
            y += 8;

            e.HasMorePages = false;
        }

    }
}

