using System.Drawing;
using System.Windows.Forms;

namespace Presentation
{
    partial class FormPOS
    {
        private System.ComponentModel.IContainer components = null;

        private TextBox txtCodigo;
        private Label lblCodigoHint;
        private Label lblClienteCodigoTitle;
        private TextBox txtClienteCodigo;
        private Label lblRncTitle;
        private TextBox txtRNC;
        private Label lblNombreClienteTitle;
        private Label lblNombreCliente;
        private Label lblDireccionTitle;
        private Label lblDireccionCliente;
        private Label lblTelefonoTitle;
        private Label lblTelefonoCliente;
        private Label lblTipoTitle;
        private Label lblTipoCliente;
        private DataGridView grid;

        private Button btnEliminar;
        private Button btnLimpiar;
        private Button btnSeleccionPago;
        private Button btnCobrar;

        private Label labelSubtotalTitle;
        private Label lblSubtotal;
        private Label labelItbisTitle;
        private Label lblItbis;
        private Label labelTotalTitle;
        private Label lblTotal;
        private Label labelPagoTitle;
        private Label lblPagoSeleccionado;
        private Label labelCobradoTitle;
        private Label lblMontoCobrado;
        private Label labelDiferenciaTitle;
        private Label lblMontoDiferencia;

        // FACTURA
        private Label lblFacturaTitle;
        private TextBox txtNoFactura;

        // Usuario POS y Caja actual
        private Label lblUsuarioPOSTitle;
        private TextBox txtUsuarioPOS;
        private Label lblCajaTitle;
        private TextBox txtCajaActual;

        // BOTONES INFERIORES
        private Button btnNotaCredito;
        private Button btnTicketEspera;
        private Button btnDescuento;
        private Button btnCierreTurno;

        // NUEVOS BOTONES LATERALES
        private Button btnBuscarProducto;
        private Button btnBuscarCliente;
        private Button btnReimprimirTicket;

        private ComboBox cboTerminoPago;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();

            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();

            txtCodigo = new TextBox();
            lblCodigoHint = new Label();
            lblClienteCodigoTitle = new Label();
            txtClienteCodigo = new TextBox();
            lblRncTitle = new Label();
            txtRNC = new TextBox();
            lblNombreClienteTitle = new Label();
            lblNombreCliente = new Label();
            lblDireccionTitle = new Label();
            lblDireccionCliente = new Label();
            lblTelefonoTitle = new Label();
            lblTelefonoCliente = new Label();
            lblTipoTitle = new Label();
            lblTipoCliente = new Label();
            grid = new DataGridView();
            colCodigo = new DataGridViewTextBoxColumn();
            colDescripcion = new DataGridViewTextBoxColumn();
            colCantidad = new DataGridViewTextBoxColumn();
            colPrecio = new DataGridViewTextBoxColumn();
            colSubtotal = new DataGridViewTextBoxColumn();
            colItbis = new DataGridViewTextBoxColumn();
            colTotal = new DataGridViewTextBoxColumn();
            colDescPct = new DataGridViewTextBoxColumn();
            colDescMonto = new DataGridViewTextBoxColumn();


            btnEliminar = new Button();
            btnLimpiar = new Button();
            btnSeleccionPago = new Button();
            btnCobrar = new Button();

            labelSubtotalTitle = new Label();
            lblSubtotal = new Label();
            labelItbisTitle = new Label();
            lblItbis = new Label();
            labelTotalTitle = new Label();
            lblTotal = new Label();
            labelPagoTitle = new Label();
            lblPagoSeleccionado = new Label();
            labelCobradoTitle = new Label();
            lblMontoCobrado = new Label();
            labelDiferenciaTitle = new Label();
            lblMontoDiferencia = new Label();

            lblFacturaTitle = new Label();
            txtNoFactura = new TextBox();
            lblUsuarioPOSTitle = new Label();
            txtUsuarioPOS = new TextBox();
            lblCajaTitle = new Label();
            txtCajaActual = new TextBox();

            btnNotaCredito = new Button();
            btnTicketEspera = new Button();
            btnDescuento = new Button();
            btnCierreTurno = new Button();

            btnBuscarProducto = new Button();
            btnBuscarCliente = new Button();
            btnReimprimirTicket = new Button();

            cboTerminoPago = new ComboBox();

            ((System.ComponentModel.ISupportInitialize)grid).BeginInit();
            SuspendLayout();

            // ========= COLORES BASE FORM =========
            BackColor = Color.FromArgb(245, 246, 250);
            Font = new Font("Segoe UI", 9F);
            ForeColor = Color.FromArgb(44, 62, 80);

            // txtCodigo
            txtCodigo.Font = new Font("Segoe UI", 14F);
            txtCodigo.Location = new Point(20, 20);
            txtCodigo.Name = "txtCodigo";
            txtCodigo.Size = new Size(460, 32);
            txtCodigo.TabIndex = 0;
            txtCodigo.KeyDown += txtCodigo_KeyDown;

            // lblCodigoHint
            lblCodigoHint.AutoSize = true;
            lblCodigoHint.Font = new Font("Segoe UI", 9F);
            lblCodigoHint.ForeColor = Color.FromArgb(127, 140, 141);
            lblCodigoHint.Location = new Point(22, 5);
            lblCodigoHint.Name = "lblCodigoHint";
            lblCodigoHint.Size = new Size(182, 15);
            lblCodigoHint.TabIndex = 1;
            lblCodigoHint.Text = "Escanee o escriba código / barras";

            // lblClienteCodigoTitle
            lblClienteCodigoTitle.AutoSize = true;
            lblClienteCodigoTitle.Font = new Font("Segoe UI", 9F);
            lblClienteCodigoTitle.Location = new Point(620, 5);
            lblClienteCodigoTitle.Name = "lblClienteCodigoTitle";
            lblClienteCodigoTitle.Size = new Size(47, 15);
            lblClienteCodigoTitle.TabIndex = 2;
            lblClienteCodigoTitle.Text = "Cliente:";

            // txtClienteCodigo
            txtClienteCodigo.Enabled = false;
            txtClienteCodigo.Font = new Font("Segoe UI", 10F);
            txtClienteCodigo.Location = new Point(620, 23);
            txtClienteCodigo.Name = "txtClienteCodigo";
            txtClienteCodigo.Size = new Size(110, 25);
            txtClienteCodigo.TabIndex = 3;
            txtClienteCodigo.Leave += txtClienteCodigo_Leave;

            // lblRncTitle
            lblRncTitle.AutoSize = true;
            lblRncTitle.Font = new Font("Segoe UI", 9F);
            lblRncTitle.Location = new Point(740, 5);
            lblRncTitle.Name = "lblRncTitle";
            lblRncTitle.Size = new Size(82, 15);
            lblRncTitle.TabIndex = 4;
            lblRncTitle.Text = "RNC / Cédula:";

            // txtRNC
            txtRNC.Font = new Font("Segoe UI", 10F);
            txtRNC.Location = new Point(740, 23);
            txtRNC.Name = "txtRNC";
            txtRNC.Size = new Size(140, 25);
            txtRNC.TabIndex = 5;
            txtRNC.TextChanged += txtRNC_TextChanged;

            // lblNombreClienteTitle
            lblNombreClienteTitle.AutoSize = true;
            lblNombreClienteTitle.Font = new Font("Segoe UI", 9F);
            lblNombreClienteTitle.Location = new Point(900, 5);
            lblNombreClienteTitle.Name = "lblNombreClienteTitle";
            lblNombreClienteTitle.Size = new Size(54, 15);
            lblNombreClienteTitle.TabIndex = 6;
            lblNombreClienteTitle.Text = "Nombre:";

            // lblNombreCliente
            lblNombreCliente.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblNombreCliente.Location = new Point(900, 23);
            lblNombreCliente.Name = "lblNombreCliente";
            lblNombreCliente.Size = new Size(380, 25);
            lblNombreCliente.TabIndex = 7;
            lblNombreCliente.Text = "Cliente Contado";

            // lblDireccionTitle
            lblDireccionTitle.AutoSize = true;
            lblDireccionTitle.Location = new Point(620, 55);
            lblDireccionTitle.Name = "lblDireccionTitle";
            lblDireccionTitle.Size = new Size(60, 15);
            lblDireccionTitle.TabIndex = 8;
            lblDireccionTitle.Text = "Dirección:";

            // lblDireccionCliente
            lblDireccionCliente.Location = new Point(680, 55);
            lblDireccionCliente.Name = "lblDireccionCliente";
            lblDireccionCliente.Size = new Size(350, 20);
            lblDireccionCliente.TabIndex = 9;

            // lblTelefonoTitle
            lblTelefonoTitle.AutoSize = true;
            lblTelefonoTitle.Location = new Point(1040, 55);
            lblTelefonoTitle.Name = "lblTelefonoTitle";
            lblTelefonoTitle.Size = new Size(28, 15);
            lblTelefonoTitle.TabIndex = 10;
            lblTelefonoTitle.Text = "Tel.:";

            // lblTelefonoCliente
            lblTelefonoCliente.Location = new Point(1070, 55);
            lblTelefonoCliente.Name = "lblTelefonoCliente";
            lblTelefonoCliente.Size = new Size(100, 20);
            lblTelefonoCliente.TabIndex = 11;

            // lblTipoTitle
            lblTipoTitle.AutoSize = true;
            lblTipoTitle.Location = new Point(1180, 55);
            lblTipoTitle.Name = "lblTipoTitle";
            lblTipoTitle.Size = new Size(34, 15);
            lblTipoTitle.TabIndex = 12;
            lblTipoTitle.Text = "Tipo:";

            // lblTipoCliente
            lblTipoCliente.Location = new Point(1215, 55);
            lblTipoCliente.Name = "lblTipoCliente";
            lblTipoCliente.Size = new Size(80, 20);
            lblTipoCliente.TabIndex = 13;

            // ============ GRID (ESTILO MODERNO) ============
            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.BackgroundColor = Color.White;
            grid.BorderStyle = BorderStyle.None;
            grid.CellBorderStyle = DataGridViewCellBorderStyle.None;
            grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            grid.EnableHeadersVisualStyles = false;
            grid.GridColor = Color.LightGray;
            grid.RowHeadersVisible = false;
            grid.RowTemplate.Height = 32;

            grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(52, 73, 94);
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);

            grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(236, 240, 241);
            grid.DefaultCellStyle.SelectionForeColor = Color.Black;
            grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(249, 252, 255);

            grid.Columns.AddRange(new DataGridViewColumn[]
            {
    colCodigo,
    colDescripcion,
    colCantidad,
    colPrecio,
    colDescPct,
    colDescMonto,
    colSubtotal,
    colItbis,
    colTotal
});

            grid.Location = new Point(180, 90);
            grid.MultiSelect = false;
            grid.Name = "grid";
            grid.ReadOnly = true;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.Size = new Size(1120, 440);
            grid.TabIndex = 3;

            // colCodigo
            colCodigo.DataPropertyName = "ProductoCodigo";
            colCodigo.HeaderText = "Código";
            colCodigo.Name = "colCodigo";
            colCodigo.ReadOnly = true;

            // colDescripcion
            colDescripcion.DataPropertyName = "Descripcion";
            colDescripcion.HeaderText = "Descripción";
            colDescripcion.Name = "colDescripcion";
            colDescripcion.ReadOnly = true;

            // colCantidad
            colCantidad.DataPropertyName = "Cantidad";
            colCantidad.HeaderText = "Cant.";
            colCantidad.Name = "colCantidad";
            colCantidad.ReadOnly = true;

            // colPrecio
            colPrecio.DataPropertyName = "Precio";
            colPrecio.HeaderText = "Precio";
            colPrecio.Name = "colPrecio";
            colPrecio.ReadOnly = true;

            // *** NUEVO DESCUENTO
            colDescPct.DataPropertyName = "DescuentoPct";
            colDescPct.HeaderText = "% Desc.";
            colDescPct.Name = "colDescPct";
            colDescPct.ReadOnly = true;

            // *** NUEVO DESCUENTO
            colDescMonto.DataPropertyName = "DescuentoMonto";
            colDescMonto.HeaderText = "Desc. Monto";
            colDescMonto.Name = "colDescMonto";
            colDescMonto.ReadOnly = true;

            // colSubtotal
            colSubtotal.DataPropertyName = "Subtotal";
            colSubtotal.HeaderText = "Subt.";
            colSubtotal.Name = "colSubtotal";
            colSubtotal.ReadOnly = true;

            // colItbis
            colItbis.DataPropertyName = "ITBIS";
            colItbis.HeaderText = "ITBIS";
            colItbis.Name = "colItbis";
            colItbis.ReadOnly = true;

            // colTotal
            colTotal.DataPropertyName = "Total";
            colTotal.HeaderText = "Total";
            colTotal.Name = "colTotal";
            colTotal.ReadOnly = true;

            // ========= BOTONES LATERALES (ESTILO PANEL) =========
            int leftButtonsX = 20;
            int firstButtonsY = 90;
            int buttonWidth = 140;
            int buttonHeight = 36;
            int buttonGap = 8;

            void StyleSideButton(Button b, Color back, Color fore, bool bold = false)
            {
                b.BackColor = back;
                b.ForeColor = fore;
                b.FlatStyle = FlatStyle.Flat;
                b.FlatAppearance.BorderSize = 0;
                b.Font = new Font("Segoe UI", 9F, bold ? FontStyle.Bold : FontStyle.Regular);
                b.Size = new Size(buttonWidth, buttonHeight);
            }

            // btnEliminar
            btnEliminar.Name = "btnEliminar";
            btnEliminar.Text = "Eliminar línea";
            StyleSideButton(btnEliminar, Color.White, Color.FromArgb(44, 62, 80));
            btnEliminar.Location = new Point(leftButtonsX, firstButtonsY);
            btnEliminar.TabIndex = 14;
            btnEliminar.Click += btnEliminar_Click;

            // btnLimpiar
            btnLimpiar.Name = "btnLimpiar";
            btnLimpiar.Text = "Limpiar carrito";
            StyleSideButton(btnLimpiar, Color.White, Color.FromArgb(44, 62, 80));
            btnLimpiar.Location = new Point(leftButtonsX, firstButtonsY + (buttonHeight + buttonGap));
            btnLimpiar.TabIndex = 15;
            btnLimpiar.Click += btnLimpiar_Click;

            // btnBuscarProducto
            btnBuscarProducto = new Button();
            btnBuscarProducto.Name = "btnBuscarProducto";
            btnBuscarProducto.Text = "Buscar producto";
            StyleSideButton(btnBuscarProducto, Color.White, Color.FromArgb(44, 62, 80));
            btnBuscarProducto.Location = new Point(leftButtonsX, firstButtonsY + 2 * (buttonHeight + buttonGap));
            btnBuscarProducto.TabIndex = 16;
            btnBuscarProducto.Click += btnBuscarProducto_Click;

            // btnBuscarCliente
            btnBuscarCliente = new Button();
            btnBuscarCliente.Name = "btnBuscarCliente";
            btnBuscarCliente.Text = "Buscar cliente";
            StyleSideButton(btnBuscarCliente, Color.White, Color.FromArgb(44, 62, 80));
            btnBuscarCliente.Location = new Point(leftButtonsX, firstButtonsY + 3 * (buttonHeight + buttonGap));
            btnBuscarCliente.TabIndex = 17;
            btnBuscarCliente.Click += btnBuscarCliente_Click;

            // btnReimprimirTicket
            btnReimprimirTicket = new Button();
            btnReimprimirTicket.Name = "btnReimprimirTicket";
            btnReimprimirTicket.Text = "Reimprimir ticket";
            StyleSideButton(btnReimprimirTicket, Color.White, Color.FromArgb(44, 62, 80));
            btnReimprimirTicket.Location = new Point(leftButtonsX, firstButtonsY + 4 * (buttonHeight + buttonGap));
            btnReimprimirTicket.TabIndex = 18;
            btnReimprimirTicket.Click += btnReimprimirTicket_Click;

            // btnSeleccionPago
            btnSeleccionPago.Name = "btnSeleccionPago";
            btnSeleccionPago.Text = "Seleccionar pagos";
            StyleSideButton(btnSeleccionPago, Color.FromArgb(52, 152, 219), Color.White, true);
            btnSeleccionPago.Location = new Point(leftButtonsX, firstButtonsY + 5 * (buttonHeight + buttonGap) + 10);
            btnSeleccionPago.TabIndex = 22;
            btnSeleccionPago.Click += btnSeleccionPago_Click;

            // ========= RESUMEN TOTALES (PARTE DERECHA) =========
            labelSubtotalTitle.AutoSize = true;
            labelSubtotalTitle.Location = new Point(788, 547);
            labelSubtotalTitle.Name = "labelSubtotalTitle";
            labelSubtotalTitle.Size = new Size(54, 15);
            labelSubtotalTitle.TabIndex = 16;
            labelSubtotalTitle.Text = "Subtotal:";

            lblSubtotal.AutoSize = true;
            lblSubtotal.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblSubtotal.Location = new Point(838, 547);
            lblSubtotal.Name = "lblSubtotal";
            lblSubtotal.Size = new Size(31, 15);
            lblSubtotal.TabIndex = 17;
            lblSubtotal.Text = "0.00";

            labelItbisTitle.AutoSize = true;
            labelItbisTitle.Location = new Point(888, 547);
            labelItbisTitle.Name = "labelItbisTitle";
            labelItbisTitle.Size = new Size(36, 15);
            labelItbisTitle.TabIndex = 18;
            labelItbisTitle.Text = "ITBIS:";

            lblItbis.AutoSize = true;
            lblItbis.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblItbis.Location = new Point(923, 547);
            lblItbis.Name = "lblItbis";
            lblItbis.Size = new Size(31, 15);
            lblItbis.TabIndex = 19;
            lblItbis.Text = "0.00";

            labelTotalTitle.AutoSize = true;
            labelTotalTitle.Location = new Point(1002, 547);
            labelTotalTitle.Name = "labelTotalTitle";
            labelTotalTitle.Size = new Size(44, 15);
            labelTotalTitle.TabIndex = 20;
            labelTotalTitle.Text = "TOTAL:";

            lblTotal.AutoSize = true;
            lblTotal.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            lblTotal.Location = new Point(1057, 544);
            lblTotal.Name = "lblTotal";
            lblTotal.Size = new Size(40, 20);
            lblTotal.TabIndex = 21;
            lblTotal.Text = "0.00";

            labelPagoTitle.AutoSize = true;
            labelPagoTitle.Location = new Point(190, 588);
            labelPagoTitle.Name = "labelPagoTitle";
            labelPagoTitle.Size = new Size(37, 15);
            labelPagoTitle.TabIndex = 23;
            labelPagoTitle.Text = "Pago:";

            lblPagoSeleccionado.AutoSize = true;
            lblPagoSeleccionado.Location = new Point(230, 588);
            lblPagoSeleccionado.Name = "lblPagoSeleccionado";
            lblPagoSeleccionado.Size = new Size(123, 15);
            lblPagoSeleccionado.TabIndex = 24;
            lblPagoSeleccionado.Text = "Pago no seleccionado";

            labelCobradoTitle.AutoSize = true;
            labelCobradoTitle.Location = new Point(822, 583);
            labelCobradoTitle.Name = "labelCobradoTitle";
            labelCobradoTitle.Size = new Size(93, 15);
            labelCobradoTitle.TabIndex = 25;
            labelCobradoTitle.Text = "Monto cobrado:";

            lblMontoCobrado.AutoSize = true;
            lblMontoCobrado.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblMontoCobrado.Location = new Point(921, 583);
            lblMontoCobrado.Name = "lblMontoCobrado";
            lblMontoCobrado.Size = new Size(31, 15);
            lblMontoCobrado.TabIndex = 26;
            lblMontoCobrado.Text = "0.00";

            labelDiferenciaTitle.AutoSize = true;
            labelDiferenciaTitle.Location = new Point(967, 583);
            labelDiferenciaTitle.Name = "labelDiferenciaTitle";
            labelDiferenciaTitle.Size = new Size(101, 15);
            labelDiferenciaTitle.TabIndex = 27;
            labelDiferenciaTitle.Text = "Monto diferencia:";

            lblMontoDiferencia.AutoSize = true;
            lblMontoDiferencia.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblMontoDiferencia.Location = new Point(1072, 583);
            lblMontoDiferencia.Name = "lblMontoDiferencia";
            lblMontoDiferencia.Size = new Size(31, 15);
            lblMontoDiferencia.TabIndex = 28;
            lblMontoDiferencia.Text = "0.00";

            // btnCobrar
            btnCobrar.BackColor = Color.FromArgb(46, 204, 113);
            btnCobrar.FlatStyle = FlatStyle.Flat;
            btnCobrar.FlatAppearance.BorderSize = 0;
            btnCobrar.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            btnCobrar.ForeColor = Color.White;
            btnCobrar.Location = new Point(1120, 570);
            btnCobrar.Name = "btnCobrar";
            btnCobrar.Size = new Size(180, 45);
            btnCobrar.TabIndex = 29;
            btnCobrar.Text = "COBRAR (F12)";
            btnCobrar.UseVisualStyleBackColor = false;
            btnCobrar.Click += btnCobrar_Click;

            // FACTURA / USUARIO / CAJA
            lblFacturaTitle.AutoSize = true;
            lblFacturaTitle.Font = new Font("Segoe UI", 9F);
            lblFacturaTitle.Location = new Point(20, 55);
            lblFacturaTitle.Name = "lblFacturaTitle";
            lblFacturaTitle.Size = new Size(66, 15);
            lblFacturaTitle.TabIndex = 40;
            lblFacturaTitle.Text = "Factura N°:";

            txtNoFactura.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            txtNoFactura.Location = new Point(90, 50);
            txtNoFactura.Name = "txtNoFactura";
            txtNoFactura.ReadOnly = true;
            txtNoFactura.Size = new Size(160, 25);
            txtNoFactura.TabIndex = 41;
            txtNoFactura.TabStop = false;
            txtNoFactura.Text = "Pendiente";

            lblUsuarioPOSTitle.AutoSize = true;
            lblUsuarioPOSTitle.Font = new Font("Segoe UI", 9F);
            lblUsuarioPOSTitle.Location = new Point(270, 55);
            lblUsuarioPOSTitle.Name = "lblUsuarioPOSTitle";
            lblUsuarioPOSTitle.Size = new Size(50, 15);
            lblUsuarioPOSTitle.TabIndex = 42;
            lblUsuarioPOSTitle.Text = "Usuario:";

            txtUsuarioPOS.Font = new Font("Segoe UI", 9F);
            txtUsuarioPOS.Location = new Point(325, 50);
            txtUsuarioPOS.Name = "txtUsuarioPOS";
            txtUsuarioPOS.ReadOnly = true;
            txtUsuarioPOS.Size = new Size(150, 23);
            txtUsuarioPOS.TabIndex = 43;
            txtUsuarioPOS.TabStop = false;

            lblCajaTitle.AutoSize = true;
            lblCajaTitle.Font = new Font("Segoe UI", 9F);
            lblCajaTitle.Location = new Point(490, 55);
            lblCajaTitle.Name = "lblCajaTitle";
            lblCajaTitle.Size = new Size(33, 15);
            lblCajaTitle.TabIndex = 44;
            lblCajaTitle.Text = "Caja:";

            txtCajaActual.Font = new Font("Segoe UI", 9F);
            txtCajaActual.Location = new Point(525, 50);
            txtCajaActual.Name = "txtCajaActual";
            txtCajaActual.ReadOnly = true;
            txtCajaActual.Size = new Size(80, 23);
            txtCajaActual.TabIndex = 45;
            txtCajaActual.TabStop = false;

            // BOTONES INFERIORES
            void StyleBottomButton(Button b)
            {
                b.BackColor = Color.WhiteSmoke;
                b.FlatStyle = FlatStyle.Flat;
                b.FlatAppearance.BorderSize = 0;
                b.Font = new Font("Segoe UI", 9F);
                b.Size = new Size(120, 32);
            }

            btnNotaCredito.Name = "btnNotaCredito";
            btnNotaCredito.Text = "Nota de Crédito";
            StyleBottomButton(btnNotaCredito);
            btnNotaCredito.Location = new Point(380, 580);
            btnNotaCredito.TabIndex = 30;
            btnNotaCredito.Click += btnNotaCredito_Click;

            btnTicketEspera.Name = "btnTicketEspera";
            btnTicketEspera.Text = "Ticket en espera";
            StyleBottomButton(btnTicketEspera);
            btnTicketEspera.Location = new Point(510, 580);
            btnTicketEspera.TabIndex = 31;
            btnTicketEspera.Click += btnTicketEspera_Click;

            btnDescuento.Name = "btnDescuento";
            btnDescuento.Text = "Descuento";
            StyleBottomButton(btnDescuento);
            btnDescuento.Location = new Point(640, 580);
            btnDescuento.TabIndex = 32;
            btnDescuento.Click += btnDescuento_Click;

            btnCierreTurno.Name = "btnCierreTurno";
            btnCierreTurno.Text = "Cierre Turno";
            btnCierreTurno.BackColor = Color.FromArgb(241, 196, 15);
            btnCierreTurno.FlatStyle = FlatStyle.Flat;
            btnCierreTurno.FlatAppearance.BorderSize = 0;
            btnCierreTurno.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnCierreTurno.Location = new Point(770, 580);
            btnCierreTurno.Size = new Size(120, 32);
            btnCierreTurno.TabIndex = 33;
            btnCierreTurno.Click += btnCierreTurno_Click;

            // cboTerminoPago
            cboTerminoPago.FormattingEnabled = true;
            cboTerminoPago.Location = new Point(1150, 25);
            cboTerminoPago.Name = "cboTerminoPago";
            cboTerminoPago.Size = new Size(121, 23);
            cboTerminoPago.TabIndex = 46;

            // FormPOS
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1320, 720);
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Punto de Venta";

            Controls.Add(cboTerminoPago);
            Controls.Add(txtCodigo);
            Controls.Add(lblCodigoHint);
            Controls.Add(lblFacturaTitle);
            Controls.Add(txtNoFactura);
            Controls.Add(lblUsuarioPOSTitle);
            Controls.Add(txtUsuarioPOS);
            Controls.Add(lblCajaTitle);
            Controls.Add(txtCajaActual);

            Controls.Add(lblClienteCodigoTitle);
            Controls.Add(txtClienteCodigo);
            Controls.Add(lblRncTitle);
            Controls.Add(txtRNC);
            Controls.Add(lblNombreClienteTitle);
            Controls.Add(lblNombreCliente);
            Controls.Add(lblDireccionTitle);
            Controls.Add(lblDireccionCliente);
            Controls.Add(lblTelefonoTitle);
            Controls.Add(lblTelefonoCliente);
            Controls.Add(lblTipoTitle);
            Controls.Add(lblTipoCliente);

            Controls.Add(grid);

            Controls.Add(btnEliminar);
            Controls.Add(btnLimpiar);
            Controls.Add(btnBuscarProducto);
            Controls.Add(btnBuscarCliente);
            Controls.Add(btnReimprimirTicket);
            Controls.Add(btnSeleccionPago);

            Controls.Add(labelSubtotalTitle);
            Controls.Add(lblSubtotal);
            Controls.Add(labelItbisTitle);
            Controls.Add(lblItbis);
            Controls.Add(labelTotalTitle);
            Controls.Add(lblTotal);
            Controls.Add(labelPagoTitle);
            Controls.Add(lblPagoSeleccionado);
            Controls.Add(labelCobradoTitle);
            Controls.Add(lblMontoCobrado);
            Controls.Add(labelDiferenciaTitle);
            Controls.Add(lblMontoDiferencia);

            Controls.Add(btnCobrar);
            Controls.Add(btnNotaCredito);
            Controls.Add(btnTicketEspera);
            Controls.Add(btnDescuento);
            Controls.Add(btnCierreTurno);

            Load += FormPOS_Load;

            Name = "FormPOS";

            ((System.ComponentModel.ISupportInitialize)grid).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DataGridViewTextBoxColumn colCodigo;
        private DataGridViewTextBoxColumn colDescripcion;
        private DataGridViewTextBoxColumn colCantidad;
        private DataGridViewTextBoxColumn colPrecio;
        private DataGridViewTextBoxColumn colSubtotal;
        private DataGridViewTextBoxColumn colItbis;
        private DataGridViewTextBoxColumn colTotal;
        private DataGridViewTextBoxColumn colDescPct;
        private DataGridViewTextBoxColumn colDescMonto;

    }
}
