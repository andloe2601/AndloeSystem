using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using Andloe.Data;
using Andloe.Entidad;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Presentation
{
    public partial class FormSeleccionPago : Form
    {
        private readonly decimal _totalBase;
        private readonly string _monedaBase;
        private Button? _btnRapidoActivo;
        private string _montoBuffer = "0";

        // Lista interna de pagos que el usuario va agregando
        private readonly List<PagoLineaResult> _pagos = new();

        // Resultado que se devolverá al POS
        public SeleccionPagoResult? Result { get; private set; }

        public FormSeleccionPago(decimal totalBase, string monedaBase = "DOP")
        {
            _totalBase = totalBase;
            _monedaBase = monedaBase;
            InitializeComponent();
        }

        private void FormSeleccionPago_Load(object sender, EventArgs e)
        {
            lblTotalBase.Text = $"{_totalBase:N2} {_monedaBase}";

            CargarMonedas();
            CargarMediosPago();
            ConfigurarGrids();
            AplicarEstilosVisuales();
            AplicarBotonesRedondeados();
            ConfigurarBotonesRapidos();

            txtMontoMoneda.TextChanged += (_, __) => ActualizarPreviewCobro();
            txtMontoMoneda.KeyDown += TxtMontoMoneda_KeyDown;

            ActualizarResumenMonedas();
            ActualizarGrillaPagos();

            // 🔥 iniciar monto en 0.00 correctamente
            SetMontoDesdeBuffer("0");

            ActualizarTotales();

            txtMontoMoneda.Focus();
            txtMontoMoneda.SelectAll();
        }

        private void SetMontoDesdeBuffer(string buffer)
        {
            if (string.IsNullOrWhiteSpace(buffer))
                buffer = "0";

            _montoBuffer = buffer;

            if (!decimal.TryParse(_montoBuffer, out var valor))
                valor = 0m;

            txtMontoMoneda.Text = valor.ToString("N2");
            txtMontoMoneda.SelectionStart = txtMontoMoneda.Text.Length;
        }

        private void ConfigurarGrids()
        {
            gridMonedas.AutoGenerateColumns = true;
            gridPagos.AutoGenerateColumns = true;

            gridMonedas.DefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            gridPagos.DefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 9.5F);

            gridMonedas.ColumnHeadersHeight = 34;
            gridPagos.ColumnHeadersHeight = 34;

            gridMonedas.RowTemplate.Height = 28;
            gridPagos.RowTemplate.Height = 28;
        }

        private void SetBotonActivo(Button btn)
        {
            // Reset todos primero
            ResetBotonesRapidos();

            _btnRapidoActivo = btn;

            // Colores según tipo
            var clave = btn.Tag?.ToString()?.ToLower() ?? "";

            if (clave == "efectivo")
                btn.BackColor = Color.FromArgb(34, 197, 94); // verde
            else if (clave == "tarjeta")
                btn.BackColor = Color.FromArgb(59, 130, 246); // azul
            else if (clave == "transferencia" || clave == "transfer")
                btn.BackColor = Color.FromArgb(168, 85, 247); // morado

            btn.ForeColor = Color.White;
            btn.FlatAppearance.BorderSize = 0;
        }

        private void ResetBotonesRapidos()
        {
            var botones = new[]
            {
        btnRapidoEfectivo,
        btnRapidoTarjeta,
        btnRapidoTransferencia
    };

            foreach (var b in botones)
            {
                b.BackColor = Color.White;
                b.ForeColor = Color.FromArgb(17, 24, 39);
                b.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
                b.FlatAppearance.BorderSize = 1;
            }
        }

        private void TxtMontoMoneda_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;

                if (btnAceptar.Enabled)
                    btnAceptar.PerformClick();
                else
                    btnAgregarLinea.PerformClick();
            }
        }

        private void AplicarEstilosVisuales()
        {
            txtMontoMoneda.BackColor = System.Drawing.Color.White;
            txtMontoMoneda.ForeColor = System.Drawing.Color.FromArgb(17, 24, 39);

            cbMoneda.BackColor = System.Drawing.Color.White;
            cbMedioPago.BackColor = System.Drawing.Color.White;

            if (string.Equals(_monedaBase, "DOP", StringComparison.OrdinalIgnoreCase))
            {
                for (int i = 0; i < cbMoneda.Items.Count; i++)
                {
                    var drv = cbMoneda.Items[i] as DataRowView;
                    if (drv != null && string.Equals(
                            drv["MonedaCodigo"]?.ToString(), "DOP", StringComparison.OrdinalIgnoreCase))
                    {
                        cbMoneda.SelectedIndex = i;
                        break;
                    }
                }
            }
        }

        // ===================== MONEDAS =====================

        private void CargarMonedas()
        {
            var dt = SqlHelper.ExecuteDataTable(@"
SELECT MonedaCodigo, Nombre
FROM dbo.Moneda
WHERE Estado = 1
ORDER BY Nombre;");

            cbMoneda.DisplayMember = "Nombre";
            cbMoneda.ValueMember = "MonedaCodigo";
            cbMoneda.DataSource = dt;

            // Selecciona por defecto la moneda base si existe
            for (int i = 0; i < cbMoneda.Items.Count; i++)
            {
                var drv = cbMoneda.Items[i] as DataRowView;
                if (drv != null && string.Equals(
                        drv["MonedaCodigo"].ToString(), _monedaBase, StringComparison.OrdinalIgnoreCase))
                {
                    cbMoneda.SelectedIndex = i;
                    break;
                }
            }
        }

        private void ActualizarResumenMonedas()
        {
            var dt = SqlHelper.ExecuteDataTable(@"
SELECT MonedaCodigo, Nombre
FROM dbo.Moneda
WHERE Estado = 1
ORDER BY Nombre;");

            var tabla = new DataTable();
            tabla.Columns.Add("Código", typeof(string));
            tabla.Columns.Add("Moneda", typeof(string));
            tabla.Columns.Add("Tasa", typeof(decimal));
            tabla.Columns.Add("Total", typeof(decimal));
            tabla.Columns.Add("Pendiente", typeof(decimal));

            decimal pagadoBase = _pagos.Sum(p => p.MontoBase);
            decimal pendienteBase = _totalBase - pagadoBase;
            if (pendienteBase < 0)
                pendienteBase = 0;

            foreach (DataRow row in dt.Rows)
            {
                string codigo = row["MonedaCodigo"].ToString() ?? "";
                string nombre = row["Nombre"].ToString() ?? "";

                decimal tasa = 1m;
                if (!string.Equals(codigo, _monedaBase, StringComparison.OrdinalIgnoreCase))
                {
                    var sql = @"
SELECT TOP(1) TasaVenta
FROM dbo.TipoCambio
WHERE MonedaCodigo = @m
ORDER BY Fecha DESC;";
                    using var cn = Db.GetOpenConnection();
                    using var cmd = new Microsoft.Data.SqlClient.SqlCommand(sql, cn);
                    cmd.Parameters.AddWithValue("@m", codigo);
                    var valor = cmd.ExecuteScalar();
                    if (valor != null && valor != DBNull.Value)
                        tasa = Convert.ToDecimal(valor);
                }

                if (tasa <= 0)
                    tasa = 1m;

                decimal totalMoneda = Math.Round(_totalBase / tasa, 2);
                decimal pendienteMoneda = Math.Round(pendienteBase / tasa, 2);

                tabla.Rows.Add(codigo, nombre, tasa, totalMoneda, pendienteMoneda);
            }

            gridMonedas.DataSource = tabla;

            if (gridMonedas.Columns["Tasa"] != null)
                gridMonedas.Columns["Tasa"]!.DefaultCellStyle.Format = "N4";

            if (gridMonedas.Columns["Total"] != null)
                gridMonedas.Columns["Total"]!.DefaultCellStyle.Format = "N2";

            if (gridMonedas.Columns["Pendiente"] != null)
                gridMonedas.Columns["Pendiente"]!.DefaultCellStyle.Format = "N2";
        }

        // ===================== MEDIOS DE PAGO =====================



        private void btnCompletarPendiente_Click(object sender, EventArgs e)
        {
            if (cbMoneda.SelectedValue == null)
                return;

            string monedaCodigo = cbMoneda.SelectedValue.ToString() ?? _monedaBase;
            decimal tasa = ObtenerTasaMonedaSeleccionada(monedaCodigo);

            decimal pagadoBase = _pagos.Sum(p => p.MontoBase);
            decimal pendienteBase = _totalBase - pagadoBase;

            if (pendienteBase <= 0)
            {
                SetMontoDesdeBuffer("0");
                ActualizarPreviewCobro();
                return;
            }

            decimal pendienteMoneda = Math.Round(pendienteBase / tasa, 2);

            _montoBuffer = pendienteMoneda.ToString("0.##", System.Globalization.CultureInfo.InvariantCulture);
            SetMontoDesdeBuffer(_montoBuffer);

            txtMontoMoneda.Focus();
            txtMontoMoneda.SelectAll();
            ActualizarPreviewCobro();
        }



        private void CargarMediosPago()
        {
            var dt = SqlHelper.ExecuteDataTable(@"
SELECT FormaPagoCodigo, Descripcion
FROM dbo.ECFFormaPagoCatalogo
WHERE Activo = 1
ORDER BY TRY_CONVERT(INT, FormaPagoCodigo);");

            cbMedioPago.DisplayMember = "Descripcion";
            cbMedioPago.ValueMember = "FormaPagoCodigo";
            cbMedioPago.DataSource = dt;

            // Por defecto: Efectivo = 1
            for (int i = 0; i < cbMedioPago.Items.Count; i++)
            {
                var drv = cbMedioPago.Items[i] as DataRowView;
                if (drv != null &&
                    string.Equals(Convert.ToString(drv["FormaPagoCodigo"]), "1", StringComparison.OrdinalIgnoreCase))
                {
                    cbMedioPago.SelectedIndex = i;
                    break;
                }
            }
        }

        // ===================== KEYPAD NUMÉRICO =====================

        private void AppendNumero(string digito)
        {
            if (string.IsNullOrWhiteSpace(digito))
                return;

            if (_montoBuffer == "0")
                _montoBuffer = digito;
            else
                _montoBuffer += digito;

            SetMontoDesdeBuffer(_montoBuffer);
            ActualizarPreviewCobro();
        }

        private void btnNum_Click(object sender, EventArgs e)
        {
            if (sender is Button btn && !string.IsNullOrEmpty(btn.Tag?.ToString()))
            {
                AppendNumero(btn.Tag.ToString()!);
            }
        }

        private void btnBorrarMonto_Click(object sender, EventArgs e)
        {
            SetMontoDesdeBuffer("0");
            ActualizarPreviewCobro();
        }

        // ===================== PAGOS =====================

        private void btnAgregarLinea_Click(object sender, EventArgs e)
        {
            if (cbMoneda.SelectedValue == null || cbMedioPago.SelectedValue == null)
            {
                MessageBox.Show("Seleccione moneda y medio de pago.",
                    "Pago", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!decimal.TryParse(txtMontoMoneda.Text.Replace(",", ""), out var montoMoneda) ||
                montoMoneda <= 0)
            {
                MessageBox.Show("Monto inválido.",
                    "Pago", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string monedaCodigo = cbMoneda.SelectedValue.ToString() ?? _monedaBase;
            string formaPagoCodigo = cbMedioPago.SelectedValue?.ToString() ?? "";
            string nombreMedio = cbMedioPago.Text;

            decimal tasa = ObtenerTasaMonedaSeleccionada(monedaCodigo);
            if (tasa <= 0) tasa = 1m;

            _pagos.Add(new PagoLineaResult
            {
                FormaPagoCodigo = formaPagoCodigo,
                NombreMedio = nombreMedio,
                MonedaCodigo = monedaCodigo,
                TasaCambio = tasa,
                MontoMoneda = montoMoneda
            });

            SetMontoDesdeBuffer("0");
            ActualizarGrillaPagos();
            ActualizarTotales();
            ActualizarResumenMonedas();
            ResetBotonesRapidos();

            txtMontoMoneda.Focus();
            txtMontoMoneda.SelectAll();
        }

        private void btnQuitarLinea_Click(object sender, EventArgs e)
        {
            if (gridPagos.CurrentRow == null || gridPagos.CurrentRow.Index < 0)
                return;

            int idx = gridPagos.CurrentRow.Index;
            if (idx >= 0 && idx < _pagos.Count)
            {
                _pagos.RemoveAt(idx);
                SetMontoDesdeBuffer("0");
                ActualizarGrillaPagos();
                ActualizarTotales();
                ActualizarResumenMonedas();
            }
        }

        private void ActualizarGrillaPagos()
        {
            gridPagos.DataSource = null;

            var lista = _pagos
                .Select((p, i) => new
                {
                    Linea = i + 1,
                    MedioPago = p.NombreMedio,
                    Moneda = p.MonedaCodigo,
                    Tasa = p.TasaCambio,
                    Monto = p.MontoMoneda,
                    EquivalenteDOP = p.MontoBase
                })
                .ToList();

            gridPagos.DataSource = lista;

            if (gridPagos.Columns["Linea"] != null)
                gridPagos.Columns["Linea"]!.Width = 60;

            if (gridPagos.Columns["MedioPago"] != null)
                gridPagos.Columns["MedioPago"]!.HeaderText = "Medio de pago";

            if (gridPagos.Columns["Moneda"] != null)
                gridPagos.Columns["Moneda"]!.HeaderText = "Moneda";

            if (gridPagos.Columns["Tasa"] != null)
            {
                gridPagos.Columns["Tasa"]!.HeaderText = "Tasa";
                gridPagos.Columns["Tasa"]!.DefaultCellStyle.Format = "N4";
            }

            if (gridPagos.Columns["Monto"] != null)
            {
                gridPagos.Columns["Monto"]!.HeaderText = "Monto";
                gridPagos.Columns["Monto"]!.DefaultCellStyle.Format = "N2";
            }

            if (gridPagos.Columns["EquivalenteDOP"] != null)
            {
                gridPagos.Columns["EquivalenteDOP"]!.HeaderText = "Equivalente DOP";
                gridPagos.Columns["EquivalenteDOP"]!.DefaultCellStyle.Format = "N2";
            }
        }

        private void ActualizarTotales()
        {
            var pagadoRegistradoBase = _pagos.Sum(p => p.MontoBase);
            var montoDigitadoBase = ObtenerMontoDigitadoBase();

            // Preview visual = pagos registrados + monto digitado
            var pagadoPreviewBase = pagadoRegistradoBase + montoDigitadoBase;
            var diferenciaPreview = pagadoPreviewBase - _totalBase;

            decimal pendientePreviewBase = diferenciaPreview < 0 ? Math.Abs(diferenciaPreview) : 0m;
            decimal cambioPreviewBase = diferenciaPreview > 0 ? diferenciaPreview : 0m;

            lblPagadoBase.Text = pagadoPreviewBase.ToString("N2");
            lblPendienteBase.Text = pendientePreviewBase.ToString("N2");
            lblCambioBase.Text = cambioPreviewBase.ToString("N2");

            lblPagadoBase.ForeColor = pagadoPreviewBase > 0
                ? Color.FromArgb(13, 148, 136)
                : Color.FromArgb(17, 24, 39);

            lblPendienteBase.ForeColor = pendientePreviewBase <= 0.01m
                ? Color.FromArgb(22, 163, 74)
                : Color.FromArgb(220, 38, 38);

            lblCambioBase.ForeColor = cambioPreviewBase > 0
                ? Color.FromArgb(37, 99, 235)
                : Color.FromArgb(100, 116, 139);

            // 🔥 Estado real: solo pagos ya agregados
            var diferenciaReal = pagadoRegistradoBase - _totalBase;
            decimal pendienteRealBase = diferenciaReal < 0 ? Math.Abs(diferenciaReal) : 0m;

            btnAceptar.Enabled = diferenciaReal >= -0.01m && _pagos.Count > 0;

            if (btnAceptar.Enabled)
            {
                btnAceptar.BackColor = Color.FromArgb(22, 163, 74);
                btnAceptar.ForeColor = Color.White;
                btnAceptar.Text = "Finalizar cobro";
            }
            else
            {
                btnAceptar.BackColor = Color.FromArgb(13, 177, 146);
                btnAceptar.ForeColor = Color.White;
                btnAceptar.Text = "Finalizar";
            }

            // Highlight visual puede seguir usando preview
            ActualizarEstadoPagoVisual(pendientePreviewBase, cambioPreviewBase);

            // 🔥 Bloqueo solo con pago real, no preview
            ActualizarBloqueoCobro(pendienteRealBase);
        }
        private decimal ObtenerTasaMonedaSeleccionada(string? monedaCodigo = null)
        {
            monedaCodigo ??= cbMoneda.SelectedValue?.ToString() ?? _monedaBase;

            decimal tasa = 1m;
            if (!string.Equals(monedaCodigo, _monedaBase, StringComparison.OrdinalIgnoreCase))
            {
                var sql = @"
SELECT TOP(1) TasaVenta
FROM dbo.TipoCambio
WHERE MonedaCodigo = @m
ORDER BY Fecha DESC;";
                using var cn = Db.GetOpenConnection();
                using var cmd = new Microsoft.Data.SqlClient.SqlCommand(sql, cn);
                cmd.Parameters.AddWithValue("@m", monedaCodigo);
                var valor = cmd.ExecuteScalar();
                if (valor != null && valor != DBNull.Value)
                    tasa = Convert.ToDecimal(valor);
            }

            return tasa <= 0 ? 1m : tasa;
        }

        private decimal ObtenerMontoDigitadoBase()
        {
            if (!decimal.TryParse(txtMontoMoneda.Text.Replace(",", ""), out var montoMoneda) || montoMoneda <= 0)
                return 0m;

            var monedaCodigo = cbMoneda.SelectedValue?.ToString() ?? _monedaBase;
            var tasa = ObtenerTasaMonedaSeleccionada(monedaCodigo);

            return Math.Round(montoMoneda * tasa, 2);
        }

        private void ActualizarPreviewCobro()
        {
            ActualizarTotales();
        }

        private void ActualizarEstadoPagoVisual(decimal pendienteBase, decimal cambioBase)
        {
            if (pendienteBase <= 0.01m)
            {
                lblTotalBase.BackColor = Color.FromArgb(220, 252, 231);
                lblTotalBase.Padding = new Padding(5);
                lblTotalBase.ForeColor = Color.FromArgb(22, 163, 74);
            }
            else
            {
                lblTotalBase.BackColor = Color.Transparent;
                lblTotalBase.Padding = new Padding(0);
                lblTotalBase.ForeColor = Color.FromArgb(13, 148, 136);
            }

            lblCambioBase.ForeColor = cambioBase > 0
                ? Color.FromArgb(37, 99, 235)
                : Color.FromArgb(100, 116, 139);
        }

        private void AplicarBotonesRedondeados()
        {
            RedondearBoton(btnAgregarLinea, 12);
            RedondearBoton(btnQuitarLinea, 12);
            RedondearBoton(btnCompletarPendiente, 12);
            RedondearBoton(btnRapidoEfectivo, 10);
            RedondearBoton(btnRapidoTarjeta, 10);
            RedondearBoton(btnRapidoTransferencia, 10);
            RedondearBoton(btnAceptar, 12);
            RedondearBoton(btnCancelar, 12);

            RedondearBoton(btnNum0, 10);
            RedondearBoton(btnNum1, 10);
            RedondearBoton(btnNum2, 10);
            RedondearBoton(btnNum3, 10);
            RedondearBoton(btnNum4, 10);
            RedondearBoton(btnNum5, 10);
            RedondearBoton(btnNum6, 10);
            RedondearBoton(btnNum7, 10);
            RedondearBoton(btnNum8, 10);
            RedondearBoton(btnNum9, 10);
            RedondearBoton(btnBorrarMonto, 10);
        }

        private void RedondearBoton(Button btn, int radius)
        {
            btn.FlatAppearance.BorderSize = 0;
            btn.Region = new Region(GetRoundedPath(new Rectangle(0, 0, btn.Width, btn.Height), radius));
        }

        private GraphicsPath GetRoundedPath(Rectangle rect, int radius)
        {
            int d = radius * 2;
            var path = new GraphicsPath();

            path.StartFigure();
            path.AddArc(rect.X, rect.Y, d, d, 180, 90);
            path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
            path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
            path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
            path.CloseFigure();

            return path;
        }
        // ===================== ACEPTAR / CANCELAR =====================

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            var pagadoBase = _pagos.Sum(p => p.MontoBase);
            var pendienteBase = _totalBase - pagadoBase;

            if (pendienteBase > 0.01m)
            {
                MessageBox.Show(
                    $"Aún falta por cobrar: {pendienteBase:N2} {_monedaBase}",
                    "Pago", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_pagos.Count == 0)
            {
                MessageBox.Show("No hay pagos registrados.", "Pago",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var res = new SeleccionPagoResult
            {
                TotalBase = _totalBase
            };
            res.Pagos.AddRange(_pagos);

            Result = res;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void ConfigurarBotonesRapidos()
        {
            btnRapidoEfectivo.Click += (_, __) => SeleccionarMedioRapido("1");
            btnRapidoTransferencia.Click += (_, __) => SeleccionarMedioRapido("2");
            btnRapidoTarjeta.Click += (_, __) => SeleccionarMedioRapido("3");
        }

        private void SeleccionarMedioRapido(string formaPagoCodigo)
        {
            for (int i = 0; i < cbMedioPago.Items.Count; i++)
            {
                if (cbMedioPago.Items[i] is DataRowView drv)
                {
                    var codigo = Convert.ToString(drv["FormaPagoCodigo"]) ?? "";
                    if (codigo == formaPagoCodigo)
                    {
                        cbMedioPago.SelectedIndex = i;
                        break;
                    }
                }
            }

            if (formaPagoCodigo == "1")
                SetBotonActivo(btnRapidoEfectivo);
            else if (formaPagoCodigo == "2")
                SetBotonActivo(btnRapidoTransferencia);
            else if (formaPagoCodigo == "3")
                SetBotonActivo(btnRapidoTarjeta);

            txtMontoMoneda.Focus();
            txtMontoMoneda.SelectAll();

            if (decimal.TryParse(txtMontoMoneda.Text.Replace(",", ""), out var monto) && monto > 0)
                btnAgregarLinea.PerformClick();
        }

        private void ActualizarBloqueoCobro(decimal pendienteBase)
        {
            bool completo = pendienteBase <= 0.01m;

            btnAgregarLinea.Enabled = !completo;
            btnCompletarPendiente.Enabled = !completo;

            btnNum0.Enabled = !completo;
            btnNum1.Enabled = !completo;
            btnNum2.Enabled = !completo;
            btnNum3.Enabled = !completo;
            btnNum4.Enabled = !completo;
            btnNum5.Enabled = !completo;
            btnNum6.Enabled = !completo;
            btnNum7.Enabled = !completo;
            btnNum8.Enabled = !completo;
            btnNum9.Enabled = !completo;
            btnBorrarMonto.Enabled = !completo;

            btnRapidoEfectivo.Enabled = !completo;
            btnRapidoTarjeta.Enabled = !completo;
            btnRapidoTransferencia.Enabled = !completo;

            txtMontoMoneda.ReadOnly = completo;

            if (completo)
            {
                txtMontoMoneda.BackColor = Color.FromArgb(241, 245, 249);
                txtMontoMoneda.ForeColor = Color.FromArgb(100, 116, 139);
            }
            else
            {
                txtMontoMoneda.BackColor = Color.White;
                txtMontoMoneda.ForeColor = Color.FromArgb(17, 24, 39);
            }
        }

        private void cbMoneda_SelectedIndexChanged(object sender, EventArgs e)
        {
            ActualizarPreviewCobro();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            // No devolvemos resultado
            Result = null;
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
