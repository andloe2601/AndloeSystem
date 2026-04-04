using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Andloe.Entidad.CxC;
using Andloe.Logica;

namespace Andloe.Presentacion
{
    public partial class FormCxCCobroEdit : Form
    {
        private readonly CxCCobroService _service = new();

        private List<CxCClienteLookupDto> _clientes = new();
        private List<CxCCuentaDestinoDto> _cuentas = new();
        private List<CxCCentroCostoDto> _centros = new();
        private List<CxCFacturaPendienteDto> _facturas = new();

        private bool _guardando;

        public FormCxCCobroEdit()
        {
            InitializeComponent();

            Load += FormLoad;

            btnBuscarCliente.Click += (_, __) => BuscarClientes();
            cboCliente.SelectionChangeCommitted += (_, __) => CargarFacturasCliente();

            cboFormaPago.SelectedIndexChanged += (_, __) =>
            {
                FiltrarCuentas();
                ActualizarCamposSegunMedio();
                ValidarPantalla();
            };

            cboCuenta.SelectedIndexChanged += (_, __) => ValidarPantalla();
            cboCentroCosto.SelectedIndexChanged += (_, __) => ValidarPantalla();
            cboTipoIngreso.SelectedIndexChanged += (_, __) => ValidarPantalla();

            txtReferencia.TextChanged += (_, __) => ValidarPantalla();
            txtNumeroCheque.TextChanged += (_, __) => ValidarPantalla();
            txtObservacion.TextChanged += (_, __) => ValidarPantalla();

            nudMontoPago.ValueChanged += (_, __) => ValidarPantalla();

            btnAutoAplicar.Click += (_, __) => AutoAplicar();
            btnGuardar.Click += (_, __) => Guardar(postear: true);
            btnGuardarBorrador.Click += (_, __) => Guardar(postear: false);

            gridFacturas.CellEndEdit += GridFacturas_CellEndEdit;
            gridFacturas.CurrentCellDirtyStateChanged += (_, __) =>
            {
                if (gridFacturas.IsCurrentCellDirty)
                    gridFacturas.CommitEdit(DataGridViewDataErrorContexts.Commit);
            };

            gridFacturas.CellFormatting += GridFacturas_CellFormatting;
            gridFacturas.CellDoubleClick += GridFacturas_CellDoubleClick;
            gridFacturas.CellContentClick += GridFacturas_CellContentClick;
            gridFacturas.EditingControlShowing += GridFacturas_EditingControlShowing;
            gridFacturas.KeyDown += GridFacturas_KeyDown;
        }

        private void FormLoad(object? sender, EventArgs e)
        {
            try
            {
                Cursor = Cursors.WaitCursor;

                _cuentas = _service.ListarCuentasDestino();
                _centros = _service.ListarCentrosCosto();

                cboCentroCosto.DataSource = null;
                cboCentroCosto.DataSource = _centros;
                cboCentroCosto.DisplayMember = nameof(CxCCentroCostoDto.Nombre);
                cboCentroCosto.ValueMember = nameof(CxCCentroCostoDto.CentroCostoId);
                cboCentroCosto.SelectedIndex = -1;

                cboFormaPago.Items.Clear();
                cboFormaPago.Items.AddRange(new object[] { "EFECTIVO", "TRANSFERENCIA", "CHEQUE", "TARJETA" });
                cboFormaPago.SelectedIndex = 0;

                cboTipoIngreso.Items.Clear();
                cboTipoIngreso.Items.AddRange(new object[] { "Pago a factura de cliente", "Otros ingresos" });
                cboTipoIngreso.SelectedIndex = 0;

                dtpFecha.Value = DateTime.Today;

                nudMontoPago.Minimum = 0;
                nudMontoPago.DecimalPlaces = 2;
                nudMontoPago.Maximum = 999999999m;

                lblTotalValor.Text = FormatearMoneda(0m);
                lblEstadoValidacion.Text = string.Empty;

                ConfigurarGridFacturas();
                FiltrarCuentas();
                ActualizarCamposSegunMedio();
                BuscarClientes();
                ValidarPantalla();
            }
            catch (Exception ex)
            {
                MostrarError(ex);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void ConfigurarGridFacturas()
        {
            gridFacturas.AutoGenerateColumns = false;
            gridFacturas.EnableHeadersVisualStyles = false;
            gridFacturas.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(245, 247, 250);
            gridFacturas.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
            gridFacturas.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            gridFacturas.DefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            gridFacturas.DefaultCellStyle.SelectionBackColor = Color.FromArgb(230, 240, 255);
            gridFacturas.DefaultCellStyle.SelectionForeColor = Color.Black;
            gridFacturas.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(250, 250, 250);
            gridFacturas.RowTemplate.Height = 28;
        }

        private void BuscarClientes()
        {
            try
            {
                Cursor = Cursors.WaitCursor;

                _clientes = _service.BuscarClientes(txtBuscarCliente.Text.Trim(), 30);

                cboCliente.DataSource = null;
                cboCliente.DataSource = _clientes;
                cboCliente.DisplayMember = nameof(CxCClienteLookupDto.Nombre);
                cboCliente.ValueMember = nameof(CxCClienteLookupDto.ClienteId);

                if (_clientes.Count > 0)
                    cboCliente.SelectedIndex = 0;

                CargarFacturasCliente();
            }
            catch (Exception ex)
            {
                MostrarError(ex);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void CargarFacturasCliente()
        {
            var cli = cboCliente.SelectedItem as CxCClienteLookupDto;

            _facturas = cli == null
                ? new List<CxCFacturaPendienteDto>()
                : _service.ListarFacturasPendientes(cli.ClienteId);

            gridFacturas.Rows.Clear();

            foreach (var f in _facturas)
            {
                int diasVencidos = 0;
                if (f.FechaVencimiento.HasValue && f.FechaVencimiento.Value.Date < DateTime.Today)
                    diasVencidos = (DateTime.Today - f.FechaVencimiento.Value.Date).Days;

                decimal saldoLuego = Math.Round(f.BalancePendiente - f.MontoRecibido, 2);
                if (saldoLuego < 0) saldoLuego = 0;

                int rowIndex = gridFacturas.Rows.Add(
                    f.NumeroDocumento,
                    f.FechaVencimiento?.ToString("dd/MM/yyyy") ?? "",
                    diasVencidos,
                    FormatearMoneda(f.TotalFactura),
                    FormatearMoneda(f.Retencion),
                    FormatearMoneda(f.BalancePendiente),
                    FormatearMoneda(f.MontoRecibido),
                    FormatearMoneda(saldoLuego),
                    "Aplicar todo"
                );

                gridFacturas.Rows[rowIndex].Tag = f;
            }

            RecalcularTotal();
            AplicarEstilosFacturas();
            ValidarPantalla();
        }

        private void FiltrarCuentas()
        {
            var tipo = ObtenerTipoMedio();

            List<CxCCuentaDestinoDto> cuentasFiltradas;

            if (tipo == "EFECTIVO")
            {
                cuentasFiltradas = _cuentas
                    .Where(x => string.Equals(x.TipoCuenta, "CAJA", StringComparison.OrdinalIgnoreCase))
                    .OrderBy(x => x.NombreMostrar)
                    .ToList();
            }
            else
            {
                cuentasFiltradas = _cuentas
                    .Where(x => string.Equals(x.TipoCuenta, "BANCO", StringComparison.OrdinalIgnoreCase))
                    .OrderBy(x => x.NombreMostrar)
                    .ToList();
            }

            cboCuenta.DataSource = null;
            cboCuenta.DisplayMember = nameof(CxCCuentaDestinoDto.NombreMostrar);
            cboCuenta.ValueMember = nameof(CxCCuentaDestinoDto.CuentaId);
            cboCuenta.DataSource = cuentasFiltradas;

            if (cuentasFiltradas.Count > 0)
            {
                cboCuenta.SelectedIndex = 0;
                cboCuenta.Enabled = true;
            }
            else
            {
                cboCuenta.Enabled = false;
            }
        }

        private void ActualizarCamposSegunMedio()
        {
            var tipo = ObtenerTipoMedio();

            bool esCheque = tipo == "CHEQUE";
            bool usaBanco = tipo == "TRANSFERENCIA" || tipo == "CHEQUE" || tipo == "TARJETA";
            bool esEfectivo = tipo == "EFECTIVO";

            lblCuenta.Text = usaBanco ? "Cuenta bancaria *" : "Caja *";

            lblNumeroCheque.Visible = esCheque;
            txtNumeroCheque.Visible = esCheque;

            lblReferencia.Visible = !esCheque;
            txtReferencia.Visible = !esCheque;

            if (esCheque)
            {
                txtReferencia.Text = string.Empty;
                txtNumeroCheque.Enabled = true;
            }
            else
            {
                txtNumeroCheque.Text = string.Empty;
                txtNumeroCheque.Enabled = false;
            }

            if (tipo == "TRANSFERENCIA")
            {
                lblReferencia.Text = "Referencia *";
                txtReferencia.PlaceholderText = "No. transferencia / referencia";
            }
            else if (tipo == "TARJETA")
            {
                lblReferencia.Text = "Referencia";
                txtReferencia.PlaceholderText = "Autorización / referencia";
            }
            else
            {
                lblReferencia.Text = "Referencia";
                txtReferencia.PlaceholderText = "Opcional";
            }

            txtNumeroCheque.PlaceholderText = "Número de cheque";
        }

        private void AutoAplicar()
        {
            ConfirmarEdicionPantalla();

            var monto = nudMontoPago.Value;
            if (monto <= 0)
            {
                MostrarInfo("Primero indica el monto del pago.");
                nudMontoPago.Focus();
                return;
            }

            _service.AutoAplicar(_facturas, monto);

            for (var i = 0; i < _facturas.Count; i++)
            {
                gridFacturas.Rows[i].Cells["colRet"].Value = FormatearMoneda(_facturas[i].Retencion);
                gridFacturas.Rows[i].Cells["colRec"].Value = FormatearMoneda(_facturas[i].MontoRecibido);

                decimal saldoLuego = Math.Round(_facturas[i].BalancePendiente - _facturas[i].MontoRecibido, 2);
                if (saldoLuego < 0) saldoLuego = 0;
                gridFacturas.Rows[i].Cells["colSaldoLuego"].Value = FormatearMoneda(saldoLuego);
            }

            RecalcularTotal();
            AplicarEstilosFacturas();
            ValidarPantalla();
        }

        private void GridFacturas_CellEndEdit(object? sender, DataGridViewCellEventArgs e)
        {
            RecalcularTotal();
            AplicarEstilosFacturas();
            ValidarPantalla();
        }

        private void GridFacturas_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            var colName = gridFacturas.Columns[e.ColumnIndex].Name;
            if (colName is "colPend" or "colRec" or "colNumero" or "colSaldoLuego")
                AplicarTodoFila(e.RowIndex);
        }

        private void GridFacturas_CellContentClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            if (gridFacturas.Columns[e.ColumnIndex].Name == "colAplicar")
                AplicarTodoFila(e.RowIndex);
        }

        private void GridFacturas_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && gridFacturas.CurrentCell != null)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;

                int row = gridFacturas.CurrentCell.RowIndex;
                int col = gridFacturas.CurrentCell.ColumnIndex;

                if (gridFacturas.Columns[col].Name == "colRec")
                {
                    if (row < gridFacturas.Rows.Count - 1)
                    {
                        gridFacturas.CurrentCell = gridFacturas.Rows[row + 1].Cells["colRec"];
                        gridFacturas.BeginEdit(true);
                    }
                }
            }
        }

        private void GridFacturas_EditingControlShowing(object? sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (gridFacturas.CurrentCell == null)
                return;

            string colName = gridFacturas.Columns[gridFacturas.CurrentCell.ColumnIndex].Name;

            if (colName == "colRec" || colName == "colRet")
            {
                if (e.Control is TextBox tb)
                {
                    tb.KeyPress -= DecimalTextBox_KeyPress;
                    tb.KeyPress += DecimalTextBox_KeyPress;
                }
            }
        }

        private void DecimalTextBox_KeyPress(object? sender, KeyPressEventArgs e)
        {
            char decimalSeparator = Convert.ToChar(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);

            if (char.IsControl(e.KeyChar))
                return;

            if (char.IsDigit(e.KeyChar))
                return;

            if (e.KeyChar == decimalSeparator)
            {
                if (sender is TextBox tb && !tb.Text.Contains(decimalSeparator))
                    return;
            }

            e.Handled = true;
        }

        private void AplicarTodoFila(int rowIndex)
        {
            if (rowIndex < 0 || rowIndex >= _facturas.Count)
                return;

            var factura = _facturas[rowIndex];
            factura.MontoRecibido = Math.Round(factura.BalancePendiente, 2);
            factura.Retencion = 0m;

            gridFacturas.Rows[rowIndex].Cells["colRet"].Value = FormatearMoneda(factura.Retencion);
            gridFacturas.Rows[rowIndex].Cells["colRec"].Value = FormatearMoneda(factura.MontoRecibido);
            gridFacturas.Rows[rowIndex].Cells["colSaldoLuego"].Value = FormatearMoneda(0m);

            RecalcularTotal();
            AplicarEstilosFacturas();
            ValidarPantalla();
        }

        private void RecalcularTotal()
        {
            decimal total = 0m;

            for (var i = 0; i < _facturas.Count; i++)
            {
                var ret = ParseCellDecimal(gridFacturas.Rows[i].Cells["colRet"].Value);
                var monto = ParseCellDecimal(gridFacturas.Rows[i].Cells["colRec"].Value);

                if (ret < 0) ret = 0;
                if (monto < 0) monto = 0;

                if (monto > _facturas[i].BalancePendiente)
                    monto = _facturas[i].BalancePendiente;

                _facturas[i].Retencion = Math.Round(ret, 2);
                _facturas[i].MontoRecibido = Math.Round(monto, 2);

                decimal saldoLuego = Math.Round(_facturas[i].BalancePendiente - _facturas[i].MontoRecibido, 2);
                if (saldoLuego < 0) saldoLuego = 0;

                gridFacturas.Rows[i].Cells["colRet"].Value = FormatearMoneda(_facturas[i].Retencion);
                gridFacturas.Rows[i].Cells["colRec"].Value = FormatearMoneda(_facturas[i].MontoRecibido);
                gridFacturas.Rows[i].Cells["colSaldoLuego"].Value = FormatearMoneda(saldoLuego);

                total += _facturas[i].MontoRecibido;
            }

            total = Math.Round(total, 2);
            lblTotalValor.Text = FormatearMoneda(total);

            if (total <= nudMontoPago.Maximum)
                nudMontoPago.Value = total;
            else
                nudMontoPago.Value = nudMontoPago.Maximum;
        }

        private void AplicarEstilosFacturas()
        {
            for (int i = 0; i < _facturas.Count; i++)
            {
                var row = gridFacturas.Rows[i];
                var factura = _facturas[i];

                row.DefaultCellStyle.BackColor = Color.White;
                row.DefaultCellStyle.ForeColor = Color.Black;

                bool vencida = factura.FechaVencimiento.HasValue && factura.FechaVencimiento.Value.Date < DateTime.Today;
                bool aplicada = factura.MontoRecibido > 0;

                row.Cells["colDiasVencidos"].Style.ForeColor = Color.Black;
                row.Cells["colDiasVencidos"].Style.Font = new Font(gridFacturas.Font, FontStyle.Regular);

                if (vencida)
                {
                    row.DefaultCellStyle.BackColor = Color.FromArgb(255, 244, 244);
                    row.Cells["colVencimiento"].Style.ForeColor = Color.Firebrick;
                    row.Cells["colVencimiento"].Style.Font = new Font(gridFacturas.Font, FontStyle.Bold);
                    row.Cells["colDiasVencidos"].Style.ForeColor = Color.Firebrick;
                    row.Cells["colDiasVencidos"].Style.Font = new Font(gridFacturas.Font, FontStyle.Bold);
                }
                else
                {
                    row.Cells["colVencimiento"].Style.ForeColor = Color.Black;
                    row.Cells["colVencimiento"].Style.Font = new Font(gridFacturas.Font, FontStyle.Regular);
                }

                if (aplicada)
                {
                    row.DefaultCellStyle.BackColor = vencida
                        ? Color.FromArgb(236, 248, 236)
                        : Color.FromArgb(240, 249, 240);

                    row.Cells["colRec"].Style.BackColor = Color.FromArgb(215, 240, 215);
                    row.Cells["colRec"].Style.Font = new Font(gridFacturas.Font, FontStyle.Bold);
                    row.Cells["colSaldoLuego"].Style.BackColor = Color.FromArgb(246, 250, 230);
                }
                else
                {
                    row.Cells["colRec"].Style.BackColor = Color.White;
                    row.Cells["colRec"].Style.Font = new Font(gridFacturas.Font, FontStyle.Regular);
                    row.Cells["colSaldoLuego"].Style.BackColor = Color.White;
                }
            }
        }

        private void GridFacturas_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            var columnName = gridFacturas.Columns[e.ColumnIndex].Name;
            if (columnName is "colTotal" or "colRet" or "colPend" or "colRec" or "colSaldoLuego")
            {
                if (e.Value != null)
                {
                    decimal valor = ParseCellDecimal(e.Value);
                    e.Value = FormatearMoneda(valor);
                    e.FormattingApplied = true;
                }
            }
        }

        private void ConfirmarEdicionPantalla()
        {
            Validate();

            if (gridFacturas.IsCurrentCellInEditMode)
                gridFacturas.EndEdit();

            if (gridFacturas.CurrentCell != null)
                gridFacturas.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }

        private void Guardar(bool postear)
        {
            if (_guardando)
                return;

            try
            {
                _guardando = true;
                Cursor = Cursors.WaitCursor;

                ConfirmarEdicionPantalla();
                RecalcularTotal();

                var error = ObtenerErrorValidacion();
                if (!string.IsNullOrWhiteSpace(error))
                    throw new InvalidOperationException(error);

                var cli = cboCliente.SelectedItem as CxCClienteLookupDto;
                var cuenta = cboCuenta.SelectedItem as CxCCuentaDestinoDto;
                var tipoMedio = ObtenerTipoMedio();

                var aplicaciones = _facturas
                    .Where(x => x.MontoRecibido > 0)
                    .Select(x => new CxCCobroAplicacionDto
                    {
                        FacturaId = x.FacturaId,
                        MontoAplicado = Math.Round(x.MontoRecibido, 2),
                        Retencion = Math.Round(x.Retencion, 2),
                        Nota = x.Retencion > 0 ? $"Retención: {x.Retencion:N2}" : null
                    })
                    .ToList();

                decimal totalAplicado = Math.Round(aplicaciones.Sum(x => x.MontoAplicado), 2);
                decimal montoMedio = Math.Round(nudMontoPago.Value, 2);

                int? centroCostoId = null;
                if (cboCentroCosto.SelectedValue != null &&
                    int.TryParse(cboCentroCosto.SelectedValue.ToString(), out var ccId) &&
                    ccId > 0)
                {
                    centroCostoId = ccId;
                }

                var dto = new CxCCobroCrearDto
                {
                    ClienteId = cli!.ClienteId,
                    ClienteCodigo = cli.Codigo,
                    ClienteNombre = cli.Nombre,
                    ClienteDoc = cli.Documento,
                    Fecha = dtpFecha.Value.Date,
                    MonedaCodigo = string.IsNullOrWhiteSpace(cuenta!.MonedaCodigo) ? "DOP" : cuenta.MonedaCodigo,
                    TasaCambio = 1m,
                    TipoMedio = tipoMedio,
                    MontoMedio = montoMedio,
                    BancoId = null,
                    CuentaBancoId = string.Equals(cuenta.TipoCuenta, "BANCO", StringComparison.OrdinalIgnoreCase)
                        ? cuenta.CuentaId
                        : null,
                    NumeroCheque = string.IsNullOrWhiteSpace(txtNumeroCheque.Text) ? null : txtNumeroCheque.Text.Trim(),
                    Referencia = string.IsNullOrWhiteSpace(txtReferencia.Text) ? null : txtReferencia.Text.Trim(),
                    Observacion = txtObservacion.Text.Trim(),
                    CentroCostoId = centroCostoId,
                    TipoIngreso = Convert.ToString(cboTipoIngreso.SelectedItem),
                    Postear = postear,
                    Aplicaciones = aplicaciones
                };

                var result = _service.Crear(dto);

                MessageBox.Show(
                    $"Recibo {result.NoRecibo} generado correctamente en estado {result.Estado}.",
                    "Recibo de pago",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MostrarError(ex);
            }
            finally
            {
                _guardando = false;
                Cursor = Cursors.Default;
                ValidarPantalla();
            }
        }

        private void ValidarPantalla()
        {
            var error = ObtenerErrorValidacion();

            if (string.IsNullOrWhiteSpace(error))
            {
                lblEstadoValidacion.ForeColor = Color.SeaGreen;
                lblEstadoValidacion.Text = "Listo para guardar.";
                btnGuardar.Enabled = true;
                btnGuardarBorrador.Enabled = true;
            }
            else
            {
                lblEstadoValidacion.ForeColor = Color.Firebrick;
                lblEstadoValidacion.Text = error;
                btnGuardar.Enabled = false;
                btnGuardarBorrador.Enabled = false;
            }
        }

        private string ObtenerErrorValidacion()
        {
            var cli = cboCliente.SelectedItem as CxCClienteLookupDto;
            if (cli == null)
                return "Debes seleccionar un cliente.";

            var tipoMedio = ObtenerTipoMedio();

            if (cboCuenta.SelectedItem == null)
            {
                if (tipoMedio == "EFECTIVO")
                    return "Debes seleccionar una caja.";
                return "Debes seleccionar una cuenta bancaria.";
            }

            var cuenta = cboCuenta.SelectedItem as CxCCuentaDestinoDto;
            if (cuenta == null)
                return "Debes seleccionar una cuenta válida.";

            if (tipoMedio == "EFECTIVO" &&
                !string.Equals(cuenta.TipoCuenta, "CAJA", StringComparison.OrdinalIgnoreCase))
                return "Para EFECTIVO debes seleccionar una caja.";

            if ((tipoMedio == "TRANSFERENCIA" || tipoMedio == "CHEQUE" || tipoMedio == "TARJETA") &&
                !string.Equals(cuenta.TipoCuenta, "BANCO", StringComparison.OrdinalIgnoreCase))
                return $"Para {tipoMedio} debes seleccionar una cuenta bancaria.";

            decimal montoMedio = Math.Round(nudMontoPago.Value, 2);
            if (montoMedio <= 0)
                return "El monto del pago debe ser mayor que cero.";

            var aplicaciones = _facturas.Where(x => x.MontoRecibido > 0).ToList();
            if (aplicaciones.Count == 0)
                return "Debes aplicar el pago a por lo menos una factura.";

            decimal totalAplicado = Math.Round(aplicaciones.Sum(x => x.MontoRecibido), 2);
            if (totalAplicado <= 0)
                return "El total aplicado debe ser mayor que cero.";

            if (totalAplicado != montoMedio)
                return "El total aplicado debe coincidir con el monto del pago.";

            if (tipoMedio == "TRANSFERENCIA" && string.IsNullOrWhiteSpace(txtReferencia.Text))
                return "Para TRANSFERENCIA debes indicar la referencia.";

            if (tipoMedio == "CHEQUE" && string.IsNullOrWhiteSpace(txtNumeroCheque.Text))
                return "Para CHEQUE debes indicar el número de cheque.";

            return string.Empty;
        }

        private string ObtenerTipoMedio()
        {
            return (Convert.ToString(cboFormaPago.SelectedItem) ?? "EFECTIVO")
                .Trim()
                .ToUpperInvariant();
        }

        private static string FormatearMoneda(decimal valor)
            => $"RD$ {valor:N2}";

        private void MostrarInfo(string mensaje)
        {
            MessageBox.Show(mensaje, "Recibo de pago", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void MostrarError(Exception ex)
        {
            var msg = ex.Message;
            if (string.IsNullOrWhiteSpace(msg))
                msg = ex.ToString();

            if (msg.Length > 3500)
                msg = msg.Substring(0, 3500);

            MessageBox.Show(msg, "Recibo de pago", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private static decimal ParseCellDecimal(object? value)
        {
            if (value == null)
                return 0m;

            var s = Convert.ToString(value) ?? "0";
            s = s.Replace("RD$", "", StringComparison.OrdinalIgnoreCase).Trim();

            if (decimal.TryParse(s, NumberStyles.Any, CultureInfo.CurrentCulture, out var n))
                return n;

            if (decimal.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out n))
                return n;

            return 0m;
        }
    }
}