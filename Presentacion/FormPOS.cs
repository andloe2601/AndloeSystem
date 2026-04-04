using System;
using System.Linq;
using System.Windows.Forms;
using Andloe.Logica;
using Andloe.Entidad;
using System.Drawing;
using System.Drawing.Printing;

namespace Presentation
{
    public partial class FormPOS : Form
    {
        private readonly PosService _pos = new();
        private readonly TerminoPagoService _terminoPagoService = new();
        private readonly bool _puedeCerrarCaja;

        // Datos de contexto POS
        private readonly string _usuarioPos;
        private readonly int _cajaId;
        private readonly string _cajaNumero;

        // Ticket
        private PrintDocument? _printDocTicket;
        private long _ticketVentaId;
        private string _ticketClienteNombre = "";
        private string _ticketClienteCodigo = "";
        private string _ticketUsuario = "";
        private string _ticketCaja = "";
        private decimal _ticketSubtotal;
        private decimal _ticketItbis;
        private decimal _ticketTotal;
        private decimal _ticketPagado;
        private decimal _ticketCambio;
        // Copia de las líneas del carrito al momento de la venta
        private List<dynamic> _ticketLineas = new();

        // Resultado de la pantalla de selección de pagos
        private SeleccionPagoResult? _pagoSeleccionado;

        // Id de la venta en curso (última venta realizada en esta sesión)
        private long? _ventaIdActual;

        // 🔹 Constructor recibiendo usuario y caja
        public FormPOS(string usuarioPos, int cajaId, string cajaNumero, bool puedeCerrarCaja)
        {
            _usuarioPos = usuarioPos ?? throw new ArgumentNullException(nameof(usuarioPos));
            _cajaId = cajaId;
            _cajaNumero = cajaNumero ?? string.Empty;
            _puedeCerrarCaja = puedeCerrarCaja;

            InitializeComponent();
            ConfigurarGrid();

            // Para que F12 funcione en todo el formulario
            KeyPreview = true;
            KeyDown += FormPOS_KeyDown;

            // Mostrar datos de usuario y caja
            txtUsuarioPOS.Text = _usuarioPos;
            txtCajaActual.Text = _cajaNumero;

            // Opcional: título de la ventana
            Text = $"Punto de Venta - {_usuarioPos} (Caja: {_cajaNumero})";
        }

        // F12 = COBRAR
        private void FormPOS_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F12)
            {
                btnCobrar.PerformClick();
                e.Handled = true;
            }
        }

        private void FormPOS_Load(object sender, EventArgs e)
        {
            CargarTerminosPago();
            cboTerminoPago.Enabled = false;
            cboTerminoPago.SelectedValue = 1; // Pago inmediato por defecto

            try
            {
                // Cliente por defecto desde configuración
                txtClienteCodigo.Text = ConfigService.ClienteDefecto;
                CargarClientePorCodigoORnc(txtClienteCodigo.Text.Trim());

                RefrescarGrid();
                txtCodigo.Focus();

                lblPagoSeleccionado.Text = "Pago no seleccionado";
                lblMontoCobrado.Text = 0m.ToString("N2");
                lblMontoDiferencia.Text = 0m.ToString("N2");

                // Factura en curso (todavía no generada)
                _ventaIdActual = null;
                txtNoFactura.Text = "Pendiente";

                // Refrescar visual
                txtUsuarioPOS.Text = _usuarioPos;
                txtCajaActual.Text = _cajaNumero;

                btnCierreTurno.Enabled = _puedeCerrarCaja;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al iniciar POS: " + ex.Message,
                    "POS", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ================= CLIENTE (RNC / CÓDIGO) =================

        private void txtRNC_TextChanged(object sender, EventArgs e)
        {
            var rnc = txtRNC.Text.Trim();
            if (rnc.Length < 3) return;
            CargarClientePorCodigoORnc(rnc);
        }

        private void txtClienteCodigo_Leave(object sender, EventArgs e)
        {
            var cod = txtClienteCodigo.Text.Trim();
            if (string.IsNullOrEmpty(cod)) return;
            CargarClientePorCodigoORnc(cod);
        }

        private void CargarClientePorCodigoORnc(string valor)
        {
            try
            {
                var cli = _pos.BuscarClientePorRncOCodigo(valor);
                if (cli == null)
                {
                    lblNombreCliente.Text = "Cliente no encontrado";
                    lblDireccionCliente.Text = "";
                    lblTelefonoCliente.Text = "";
                    lblTipoCliente.Text = "";
                    return;
                }

                txtClienteCodigo.Text = cli.Codigo ?? "";
                lblNombreCliente.Text = cli.Nombre ?? "";
                lblDireccionCliente.Text = cli.Direccion ?? "";
                lblTelefonoCliente.Text = cli.Telefono ?? "";

                string tipoDesc = cli.Tipo switch
                {
                    1 => "Crédito",
                    2 => "Mayorista",
                    _ => "Contado"
                };
                lblTipoCliente.Text = tipoDesc;

                // Si ya hay productos, recalculamos promos según el nuevo cliente
                if (_pos.Carrito.Count > 0)
                    RecalcularPromosSegunCliente();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar cliente: " + ex.Message,
                    "POS", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ================= PRODUCTOS / CARRITO =================

        private void txtCodigo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;

            var cod = txtCodigo.Text.Trim();
            if (string.IsNullOrEmpty(cod)) return;

            var clienteCodigo = string.IsNullOrWhiteSpace(txtClienteCodigo.Text)
                ? ConfigService.ClienteDefecto
                : txtClienteCodigo.Text.Trim();

            // 👉 Ahora el cliente se pasa al servicio POS
            var item = _pos.AgregarPorCodigo(cod, 1, clienteCodigo);
            if (item == null)
            {
                System.Media.SystemSounds.Exclamation.Play();
                MessageBox.Show("Producto no encontrado o sin existencia suficiente.", "POS",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtCodigo.SelectAll();
                return;
            }

            txtCodigo.Clear();
            RefrescarGrid();
            e.SuppressKeyPress = true; // evita beep
        }

        private void RefrescarGrid()
        {
            grid.DataSource = null;

            grid.DataSource = _pos.Carrito.Select(x => new
            {
                x.ProductoCodigo,
                x.Descripcion,
                x.Cantidad,
                Precio = x.PrecioUnit,

                // % Desc. real calculado a partir del monto de descuento
                DescuentoPct = (x.Cantidad * x.PrecioUnit) == 0
                    ? 0m
                    : Math.Round(
                        (x.DescuentoMonto / (x.Cantidad * x.PrecioUnit)) * 100m,
                        2
                      ),

                DescuentoMonto = x.DescuentoMonto,
                Subtotal = x.Importe,
                ITBIS = x.ItbisMonto,
                Total = x.Total
            }).ToList();

            // Formato 2 decimales a las columnas numéricas
            foreach (DataGridViewColumn c in grid.Columns)
            {
                if (c.Name == "colPrecio" ||
                    c.Name == "colSubtotal" ||
                    c.Name == "colItbis" ||
                    c.Name == "colTotal" ||
                    c.Name == "colDescPct" ||
                    c.Name == "colDescMonto")
                {
                    c.DefaultCellStyle.Format = "N2";
                    c.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                }
            }

            var totales = _pos.Totales();
            decimal s = totales.Subtotal;
            decimal i = totales.Itbis;
            decimal t = totales.Total;

            lblSubtotal.Text = s.ToString("N2");
            lblItbis.Text = i.ToString("N2");
            lblTotal.Text = t.ToString("N2");

            ActualizarMontosDesdePagos();
        }

        private void ConfigurarGrid()
        {
            grid.AutoGenerateColumns = false;

            colCodigo.DataPropertyName = "ProductoCodigo";
            colDescripcion.DataPropertyName = "Descripcion";
            colCantidad.DataPropertyName = "Cantidad";
            colPrecio.DataPropertyName = "Precio";
            colDescPct.DataPropertyName = "DescuentoPct";
            colDescMonto.DataPropertyName = "DescuentoMonto";
            colSubtotal.DataPropertyName = "Subtotal";
            colItbis.DataPropertyName = "ITBIS";
            colTotal.DataPropertyName = "Total";
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (grid.CurrentRow == null) return;
            var cod = grid.CurrentRow.Cells["colCodigo"].Value?.ToString();
            if (string.IsNullOrEmpty(cod)) return;

            _pos.Quitar(cod);
            RefrescarGrid();
        }

        private void RecalcularPromosSegunCliente()
        {
            var clienteCodigo = string.IsNullOrWhiteSpace(txtClienteCodigo.Text)
                ? ConfigService.ClienteDefecto
                : txtClienteCodigo.Text.Trim();

            _pos.RecalcularPromosCarrito(clienteCodigo);
            RefrescarGrid();
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            _pos.Carrito.Clear();
            _pagoSeleccionado = null;
            lblPagoSeleccionado.Text = "Pago no seleccionado";
            lblMontoCobrado.Text = 0m.ToString("N2");
            lblMontoDiferencia.Text = 0m.ToString("N2");
            RefrescarGrid();
            txtCodigo.Clear();
            txtCodigo.Focus();

            // Reiniciar factura en curso
            _ventaIdActual = null;
            txtNoFactura.Text = "Pendiente";
        }

        // ================= PAGOS / MULTIMONEDA =================

        private void btnSeleccionPago_Click(object sender, EventArgs e)
        {
            if (!decimal.TryParse(lblTotal.Text.Replace(",", ""), out var totalBase))
                totalBase = 0m;

            if (totalBase <= 0)
            {
                MessageBox.Show("No hay total a cobrar. Agregue productos primero.",
                    "POS", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var monedaBase = ConfigService.MonedaDefecto;

            using (var frm = new FormSeleccionPago(totalBase, monedaBase))
            {
                var dr = frm.ShowDialog(this);
                if (dr == DialogResult.OK && frm.Result != null)
                {
                    _pagoSeleccionado = frm.Result;
                    ActualizarMontosDesdePagos();
                }
            }
        }

        private void ActualizarMontosDesdePagos()
        {
            if (_pagoSeleccionado == null)
            {
                lblPagoSeleccionado.Text = "Pago no seleccionado";
                lblMontoCobrado.Text = 0m.ToString("N2");
                lblMontoDiferencia.Text = lblTotal.Text;
                return;
            }

            if (_pagoSeleccionado.Pagos.Count == 1)
            {
                var p = _pagoSeleccionado.Pagos[0];
                lblPagoSeleccionado.Text =
                    $"{p.NombreMedio} - {p.MontoMoneda:N2} {p.MonedaCodigo} (Tasa {p.TasaCambio:N4})";
            }
            else
            {
                lblPagoSeleccionado.Text =
                    $"Múltiples pagos ({_pagoSeleccionado.Pagos.Count}) - Pagado: {_pagoSeleccionado.PagadoBase:N2} DOP";
            }

            lblMontoCobrado.Text = _pagoSeleccionado.PagadoBase.ToString("N2");

            var diff = _pagoSeleccionado.DiferenciaBase;
            if (diff < 0) diff = 0;
            lblMontoDiferencia.Text = diff.ToString("N2");
        }

        private void btnCobrar_Click(object sender, EventArgs e)
        {
            try
            {
                if (_pos.Carrito.Count == 0)
                {
                    MessageBox.Show("Carrito vacío.", "POS",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (_pagoSeleccionado == null)
                {
                    MessageBox.Show("Debe seleccionar los pagos antes de cobrar.",
                        "POS", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (_pagoSeleccionado.DiferenciaBase > 0.01m)
                {
                    MessageBox.Show(
                        $"Falta por cobrar: {_pagoSeleccionado.DiferenciaBase:N2} DOP.",
                        "POS", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var clienteCodigo = string.IsNullOrWhiteSpace(txtClienteCodigo.Text)
                    ? ConfigService.ClienteDefecto
                    : txtClienteCodigo.Text.Trim();

                // Tomamos el primer medio de pago
                var primerPago = _pagoSeleccionado.Pagos[0];
                var medioPagoId = primerPago.MedioPagoId;
                var montoRecibidoBase = _pagoSeleccionado.PagadoBase;

                // Cerrar venta (esto ahora también registra promos en BD desde PosService)
                var ventaId = _pos.CerrarVenta(
    usuario: _usuarioPos,
    clienteCodigo: clienteCodigo,
    medioPagoId: medioPagoId,
    montoRecibido: montoRecibidoBase,
    cajaId: _cajaId,
    moneda: ConfigService.MonedaDefecto,
    terminoPagoId: 1,
    posCajaNumero: _cajaNumero
 );

                // Guardar también los pagos amarrados a la caja
                _pos.GuardarPagosPOS(
                    ventaId,
                    _pagoSeleccionado.Pagos,
                    _usuarioPos,
                    _cajaId,
                    _cajaNumero);

                // ====================== PREPARAR TICKET ======================
                _ticketVentaId = ventaId;
                _ticketUsuario = _usuarioPos;
                _ticketCaja = _cajaNumero;
                _ticketClienteCodigo = clienteCodigo;
                _ticketClienteNombre = lblNombreCliente.Text ?? "";

                // Totales actuales del POS
                var totales = _pos.Totales();
                _ticketSubtotal = totales.Subtotal;
                _ticketItbis = totales.Itbis;
                _ticketTotal = totales.Total;

                // Pagado y cambio
                _ticketPagado = _pagoSeleccionado.PagadoBase;
                _ticketCambio = _ticketPagado - _ticketTotal;
                if (_ticketCambio < 0) _ticketCambio = 0;

                // Copia de las líneas del carrito (antes de limpiar)
                _ticketLineas = _pos.Carrito
                    .Select(x => new
                    {
                        x.ProductoCodigo,
                        x.Descripcion,
                        x.Cantidad,
                        x.PrecioUnit,
                        x.Total
                    })
                    .Cast<dynamic>()
                    .ToList();

                // Imprimir ticket 80mm
                ImprimirTicket80mm();

                _ventaIdActual = ventaId;
                txtNoFactura.Text = ventaId.ToString();

                MessageBox.Show($"Venta realizada.\nID: {ventaId}", "POS",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                _pos.Carrito.Clear();
                _pagoSeleccionado = null;
                lblPagoSeleccionado.Text = "Pago no seleccionado";
                lblMontoCobrado.Text = 0m.ToString("N2");
                lblMontoDiferencia.Text = 0m.ToString("N2");
                RefrescarGrid();
                txtCodigo.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cobrar: " + ex.Message,
                    "POS", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ================= BOTONES INFERIORES EXTRA =================

        private void btnNotaCredito_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Funcionalidad de Nota de Crédito (pendiente de implementación).",
                "POS", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnTicketEspera_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Funcionalidad de Ticket en espera (pendiente de implementación).",
                "POS", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnDescuento_Click(object sender, EventArgs e)
        {
            try
            {
                if (_pos.Carrito.Count == 0)
                {
                    MessageBox.Show("No hay productos en el carrito.", "POS",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                if (grid.CurrentRow == null)
                {
                    MessageBox.Show("Debe seleccionar una línea para aplicar descuento.", "POS",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var cod = grid.CurrentRow.Cells["colCodigo"].Value?.ToString();
                if (string.IsNullOrWhiteSpace(cod))
                {
                    MessageBox.Show("No se pudo obtener el código de producto de la línea seleccionada.", "POS",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Buscar el item real en el carrito
                var item = _pos.Carrito.FirstOrDefault(x => x.ProductoCodigo == cod);
                if (item == null)
                {
                    MessageBox.Show("No se encontró el producto en el carrito.", "POS",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Validar si el sistema permite descuentos
                if (!ConfigService.DescuentoHabilitado)
                {
                    MessageBox.Show("Los descuentos están deshabilitados en la configuración del sistema.",
                        "POS", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Cliente actual
                var clienteCodigo = string.IsNullOrWhiteSpace(txtClienteCodigo.Text)
                    ? ConfigService.ClienteDefecto
                    : txtClienteCodigo.Text.Trim();

                // Calcular el máximo permitido de descuento para este cliente/producto
                var maxPermitido = _pos.CalcularMaxDescuentoPct(clienteCodigo, cod);
                if (maxPermitido <= 0)
                {
                    MessageBox.Show("Este cliente/producto no permite descuento.",
                        "POS", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Pedir el % de descuento al usuario
                var nuevoPct = PedirPorcentajeDescuento(maxPermitido, item.DescuentoPct);
                if (nuevoPct == null)
                    return; // Canceló

                // Aplicar el descuento a la línea
                item.DescuentoPct = nuevoPct.Value;
                item.DescuentoMonto = Math.Round(item.SubtotalBruto * (item.DescuentoPct / 100m), 2);

                // Refrescar grilla y totales
                RefrescarGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al aplicar descuento: " + ex.Message,
                    "POS", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Form para pedir % descuento
        private decimal? PedirPorcentajeDescuento(decimal maxPermitido, decimal actualPct)
        {
            using (var frm = new Form())
            {
                frm.Text = "Descuento en %";
                frm.StartPosition = FormStartPosition.CenterParent;
                frm.FormBorderStyle = FormBorderStyle.FixedDialog;
                frm.MinimizeBox = false;
                frm.MaximizeBox = false;
                frm.ShowInTaskbar = false;
                frm.ClientSize = new Size(280, 140);

                var lbl = new Label
                {
                    AutoSize = true,
                    Text = $"Descuento (0 - {maxPermitido:N2} %):",
                    Location = new Point(10, 15)
                };

                var num = new NumericUpDown
                {
                    DecimalPlaces = 2,
                    Minimum = 0,
                    Maximum = maxPermitido > 0 ? maxPermitido : 0,
                    Value = (actualPct >= 0 && actualPct <= maxPermitido)
                        ? actualPct
                        : 0,
                    Location = new Point(10, 40),
                    Width = 120
                };

                var btnOk = new Button
                {
                    Text = "Aceptar",
                    DialogResult = DialogResult.OK,
                    Location = new Point(40, 80),
                    Width = 80
                };

                var btnCancel = new Button
                {
                    Text = "Cancelar",
                    DialogResult = DialogResult.Cancel,
                    Location = new Point(150, 80),
                    Width = 80
                };

                frm.Controls.Add(lbl);
                frm.Controls.Add(num);
                frm.Controls.Add(btnOk);
                frm.Controls.Add(btnCancel);

                frm.AcceptButton = btnOk;
                frm.CancelButton = btnCancel;

                var dr = frm.ShowDialog(this);
                if (dr == DialogResult.OK)
                    return num.Value;

                return null;
            }
        }

        private void btnCierreTurno_Click(object sender, EventArgs e)
        {
            // 🔐 Seguridad extra
            if (!_puedeCerrarCaja)
            {
                MessageBox.Show(
                    "No tiene permisos para cerrar la caja.\n" +
                    "Contacte al administrador o supervisor.",
                    "Permiso denegado",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }
            using (var frm = new FormCierreCaja(_cajaId, _cajaNumero, _usuarioPos))
            {
                var dr = frm.ShowDialog(this);
                if (dr == DialogResult.OK)
                {
                    MessageBox.Show(
                        "Cierre de caja realizado.\nLa sesión del cajero se cerrará.",
                        "POS",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    this.Close();
                }
            }
        }

        private void btnBuscarProducto_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Búsqueda de producto (pendiente de implementación).",
                "POS", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnBuscarCliente_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Búsqueda de cliente (pendiente de implementación).",
                "POS", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnReimprimirTicket_Click(object sender, EventArgs e)
        {
            if (_ticketVentaId <= 0)
            {
                MessageBox.Show("No hay ticket reciente para reimprimir.",
                    "POS", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            ImprimirTicket80mm();
        }

        private void CargarTerminosPago()
        {
            try
            {
                var lista = _terminoPagoService.ListarActivos();
                cboTerminoPago.DisplayMember = "Descripcion";
                cboTerminoPago.ValueMember = "TerminoPagoId";
                cboTerminoPago.DataSource = lista;

                cboTerminoPago.SelectedValue = 1;
                cboTerminoPago.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar términos de pago: " + ex.Message,
                    "Venta", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // =====================================================
        //  IMPRESIÓN TICKET 80mm
        // =====================================================

        private void ImprimirTicket80mm()
        {
            try
            {
                _printDocTicket = new PrintDocument();
                _printDocTicket.PrintPage += PrintDocTicket_PrintPage;

                _printDocTicket.DefaultPageSettings.PaperSize =
                    new PaperSize("Ticket80mm", 300, 1000);

                _printDocTicket.DefaultPageSettings.Margins =
                    new Margins(5, 5, 5, 5);

                _printDocTicket.Print();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al imprimir ticket: " + ex.Message,
                    "POS", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PrintDocTicket_PrintPage(object? sender, PrintPageEventArgs e)
        {
            float y = e.MarginBounds.Top;
            float left = e.MarginBounds.Left;
            float width = e.MarginBounds.Width;

            using var fontTitulo = new Font("Consolas", 10, FontStyle.Bold);
            using var fontNormal = new Font("Consolas", 8, FontStyle.Regular);
            using var fontNegrita = new Font("Consolas", 8, FontStyle.Bold);

            var g = e.Graphics;

            string centro(string texto)
            {
                SizeF sz = g.MeasureString(texto, fontTitulo);
                float x = left + (width - sz.Width) / 2;
                g.DrawString(texto, fontTitulo, Brushes.Black, x, y);
                y += sz.Height;
                return texto;
            }

            string linea = new string('-', 40);

            // ENCABEZADO
            centro("MI TIENDA");
            centro("RNC: 000000000");
            y += 5;

            g.DrawString($"Fecha: {DateTime.Now:dd/MM/yyyy HH:mm}",
                fontNormal, Brushes.Black, left, y);
            y += fontNormal.GetHeight(g);

            g.DrawString($"Caja : {_ticketCaja}", fontNormal, Brushes.Black, left, y);
            y += fontNormal.GetHeight(g);

            g.DrawString($"Cajero: {_ticketUsuario}", fontNormal, Brushes.Black, left, y);
            y += fontNormal.GetHeight(g);

            g.DrawString($"Venta: {_ticketVentaId}", fontNormal, Brushes.Black, left, y);
            y += fontNormal.GetHeight(g);

            if (!string.IsNullOrWhiteSpace(_ticketClienteCodigo))
            {
                g.DrawString($"Cliente: {_ticketClienteCodigo}",
                    fontNormal, Brushes.Black, left, y);
                y += fontNormal.GetHeight(g);
            }

            if (!string.IsNullOrWhiteSpace(_ticketClienteNombre))
            {
                g.DrawString(_ticketClienteNombre,
                    fontNormal, Brushes.Black, left, y);
                y += fontNormal.GetHeight(g);
            }

            y += 5;
            g.DrawString(linea, fontNormal, Brushes.Black, left, y);
            y += fontNormal.GetHeight(g);

            // DETALLE
            foreach (var item in _ticketLineas)
            {
                string desc = (item.Descripcion ?? "").ToString();
                if (desc.Length > 30)
                    desc = desc.Substring(0, 30);

                g.DrawString(desc, fontNormal, Brushes.Black, left, y);
                y += fontNormal.GetHeight(g);

                string lineaDetalle =
                    $"{item.Cantidad:N0} x {item.PrecioUnit,8:N2}  {item.Total,8:N2}";
                g.DrawString(lineaDetalle, fontNormal, Brushes.Black, left, y);
                y += fontNormal.GetHeight(g);

                y += 2;
            }

            g.DrawString(linea, fontNormal, Brushes.Black, left, y);
            y += fontNormal.GetHeight(g);

            // TOTALES
            g.DrawString($"SUBTOTAL: {_ticketSubtotal,10:N2}",
                fontNegrita, Brushes.Black, left, y);
            y += fontNegrita.GetHeight(g);

            g.DrawString($"ITBIS   : {_ticketItbis,10:N2}",
                fontNegrita, Brushes.Black, left, y);
            y += fontNegrita.GetHeight(g);

            g.DrawString($"TOTAL   : {_ticketTotal,10:N2}",
                fontNegrita, Brushes.Black, left, y);
            y += fontNegrita.GetHeight(g);

            g.DrawString($"PAGADO  : {_ticketPagado,10:N2}",
                fontNegrita, Brushes.Black, left, y);
            y += fontNegrita.GetHeight(g);

            g.DrawString($"CAMBIO  : {_ticketCambio,10:N2}",
                fontNegrita, Brushes.Black, left, y);
            y += fontNegrita.GetHeight(g);

            y += 5;
            g.DrawString("Gracias por su compra",
                fontNormal, Brushes.Black, left, y);
            y += fontNormal.GetHeight(g);

            e.HasMorePages = false;
        }
    }
}
