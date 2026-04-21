using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Andloe.Presentacion
{
    partial class FormFacturaV
    {
        private System.ComponentModel.IContainer components = null;

        // ====== ROOT ======
        private Panel pnlRoot;
        private Panel pnlHeader;
        private Label lblTitle;
        private Label lblSubTitle;

        private Panel pnlTop;
        private Panel pnlMid;
        private Panel pnlBottom;

        private Panel cardTop;
        private Panel cardMid;
        private Panel cardBottom;

        private TableLayoutPanel tblTop;
        private TableLayoutPanel tblLeft;
        private TableLayoutPanel tblRightWrap;
        private TableLayoutPanel tblTopRight;
        private TableLayoutPanel tblRight;

        // ====== LEFT (cliente) ======
        private Label lblClienteN;
        public TextBox txtClienteBuscar;
        public Button btnBuscarCliente;

        // ✅ REQUERIDO por FormFacturaV.cs
        public TextBox txtClienteCodigo;
        private Label lblClienteCodigo;

        private Label lblCliNombre;
        public TextBox txtClienteNombre;

        private Label lblCliDireccion;
        public TextBox txtClienteDireccion;

        private Label lblCliRnc;
        public TextBox txtClienteRnc;

        // ✅ REQUERIDO por FormFacturaV.cs → AplicarEstadoFiscalVisual()
        public TextBox txtEstadoFiscal;
        private Label lblEstadoFiscal;

        // ====== HIDDEN INFO ======
        private Label lblInfoFacturaId;
        public TextBox txtFacturaIdInfo;
        private Label lblInfoEstado;
        public TextBox txtEstadoInfo;
        private Label lblInfoTipo;
        public TextBox txtTipoInfo;

        // ====== RIGHT TOP ======
        private Label lblNumeroFacturaTop;
        public TextBox txtNumeroFacturaTop;

        private Label lblFechaEmision;
        public DateTimePicker dtpFechaDoc;

        private Label lblTotSubtotal;
        public TextBox txtSubtotal;

        private Label lblTotDescuento;
        public TextBox txtDescuentoTotal;

        private Label lblTotItbis;
        public TextBox txtItbisTotal;

        private Label lblTotTotal;
        public TextBox txtTotalGeneral;

        // ====== RIGHT BODY ======
        private Label lblFechaRegistro;
        public DateTimePicker dtpFechaRegistro;

        private Label lblTipoDoc;
        public ComboBox cboTipoDoc;

        private Label lblCredito;
        public CheckBox chkCredito;

        private Label lblTerminoPago;
        public ComboBox cboTerminoPago;

        private Label lblDiasCredito;
        public TextBox txtDiasCredito;

        // ✅ REQUERIDO por FormFacturaV.cs → InitCombos(), ActualizarEstadoComprobante(), etc.
        private Label lblTipoComprobante;
        public ComboBox cboTipoComprobante;

        // ✅ REQUERIDO por FormFacturaV.cs → InitCombos(), GetCodVendedorUI(), SetVendedorUI(), etc.
        private Label lblVendedor;
        public ComboBox cboVendedor;

        // ====== GRID ======
        public DataGridView grid;
        private DataGridViewTextBoxColumn colDetId;
        private DataGridViewTextBoxColumn colImpuestoId;
        private DataGridViewTextBoxColumn colProductoCodigo;
        private DataGridViewTextBoxColumn colCodBarra;
        private DataGridViewTextBoxColumn colDescripcion;
        private DataGridViewTextBoxColumn colUnidad;
        private DataGridViewTextBoxColumn colCantidad;
        private DataGridViewTextBoxColumn colPrecio;
        private DataGridViewTextBoxColumn colDescuentoPct;
        private DataGridViewTextBoxColumn colDescuentoMonto;
        private DataGridViewTextBoxColumn colItbisPct;
        private DataGridViewTextBoxColumn colItbisMonto;
        private DataGridViewTextBoxColumn colTotalLinea;

        // ====== BOTTOM ======
        private FlowLayoutPanel flowBottom;
        public Button btnImprimir;
        public Button btnNuevo;
        public Button btnAgregar;
        public Button btnGuardar;
        public Button btnFinalizar;
        public Button btnEliminarLinea;
        public Button btnCerrar;
        public Button btnRegistrarFactura;
        public Button btnAnular;

        // compat labels (no se usan visualmente)
        public Label lblFacturaId;
        public Label lblNumero;
        public Label lblEstado;
        public Label lblTipoActual;
        public Label lblClienteActual;
        public Label lblSubtotal;
        public Label lblDescuentoTotal;
        public Label lblItbis;
        public Label lblTotal;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();

            // ===== Paleta de colores =====
            var cBg = Color.FromArgb(242, 244, 248);
            var cCard = Color.White;
            var cLine = Color.FromArgb(220, 226, 235);
            var cText = Color.FromArgb(22, 28, 45);
            var cMuted = Color.FromArgb(115, 125, 145);
            var cAccent = Color.FromArgb(37, 99, 235);
            var cAccentLight = Color.FromArgb(219, 234, 254);
            var cHeaderBg1 = Color.FromArgb(23, 37, 84);
            var cHeaderBg2 = Color.FromArgb(37, 99, 235);
            var cGridHeader = Color.FromArgb(248, 250, 252);
            var cGridSel = Color.FromArgb(219, 234, 254);
            var cGridAlt = Color.FromArgb(250, 251, 255);

            // ===== Fuentes =====
            var fTitle = new Font("Segoe UI", 18F, FontStyle.Bold);
            var fSub = new Font("Segoe UI", 9.5F, FontStyle.Regular);
            var fLabel = new Font("Segoe UI", 8F, FontStyle.Bold);
            var fInput = new Font("Segoe UI", 9.5F, FontStyle.Regular);
            var fTotLbl = new Font("Segoe UI", 7.5F, FontStyle.Bold);
            var fTotVal = new Font("Segoe UI", 11F, FontStyle.Bold);
            var fTotMain = new Font("Segoe UI", 13F, FontStyle.Bold);

            // ===== Instancias =====
            pnlRoot = new Panel();
            pnlHeader = new Panel();
            lblTitle = new Label();
            lblSubTitle = new Label();
            pnlTop = new Panel();
            pnlMid = new Panel();
            pnlBottom = new Panel();
            cardTop = new Panel();
            cardMid = new Panel();
            cardBottom = new Panel();
            tblTop = new TableLayoutPanel();
            tblLeft = new TableLayoutPanel();
            tblRightWrap = new TableLayoutPanel();
            tblTopRight = new TableLayoutPanel();
            tblRight = new TableLayoutPanel();

            lblClienteN = new Label();
            txtClienteBuscar = new TextBox();
            btnBuscarCliente = new Button();
            lblClienteCodigo = new Label();
            txtClienteCodigo = new TextBox();
            lblCliNombre = new Label();
            txtClienteNombre = new TextBox();
            lblCliDireccion = new Label();
            txtClienteDireccion = new TextBox();
            lblCliRnc = new Label();
            txtClienteRnc = new TextBox();
            lblEstadoFiscal = new Label();
            txtEstadoFiscal = new TextBox();

            lblInfoFacturaId = new Label();
            txtFacturaIdInfo = new TextBox();
            lblInfoEstado = new Label();
            txtEstadoInfo = new TextBox();
            lblInfoTipo = new Label();
            txtTipoInfo = new TextBox();

            lblNumeroFacturaTop = new Label();
            txtNumeroFacturaTop = new TextBox();
            lblFechaEmision = new Label();
            dtpFechaDoc = new DateTimePicker();
            lblTotSubtotal = new Label();
            txtSubtotal = new TextBox();
            lblTotDescuento = new Label();
            txtDescuentoTotal = new TextBox();
            lblTotItbis = new Label();
            txtItbisTotal = new TextBox();
            lblTotTotal = new Label();
            txtTotalGeneral = new TextBox();

            lblFechaRegistro = new Label();
            dtpFechaRegistro = new DateTimePicker();
            lblTipoDoc = new Label();
            cboTipoDoc = new ComboBox();
            lblCredito = new Label();
            chkCredito = new CheckBox();
            lblTerminoPago = new Label();
            cboTerminoPago = new ComboBox();
            lblDiasCredito = new Label();
            txtDiasCredito = new TextBox();
            lblTipoComprobante = new Label();
            cboTipoComprobante = new ComboBox();
            lblVendedor = new Label();
            cboVendedor = new ComboBox();

            grid = new DataGridView();
            colDetId = new DataGridViewTextBoxColumn();
            colImpuestoId = new DataGridViewTextBoxColumn();
            colProductoCodigo = new DataGridViewTextBoxColumn();
            colCodBarra = new DataGridViewTextBoxColumn();
            colDescripcion = new DataGridViewTextBoxColumn();
            colUnidad = new DataGridViewTextBoxColumn();
            colCantidad = new DataGridViewTextBoxColumn();
            colPrecio = new DataGridViewTextBoxColumn();
            colDescuentoPct = new DataGridViewTextBoxColumn();
            colDescuentoMonto = new DataGridViewTextBoxColumn();
            colItbisPct = new DataGridViewTextBoxColumn();
            colItbisMonto = new DataGridViewTextBoxColumn();
            colTotalLinea = new DataGridViewTextBoxColumn();

            flowBottom = new FlowLayoutPanel();
            btnCerrar = new Button();
            btnEliminarLinea = new Button();
            btnFinalizar = new Button();
            btnGuardar = new Button();
            btnAgregar = new Button();
            btnNuevo = new Button();
            btnRegistrarFactura = new Button();
            btnImprimir = new Button();
            btnAnular = new Button();

            lblFacturaId = new Label();
            lblNumero = new Label();
            lblEstado = new Label();
            lblTipoActual = new Label();
            lblClienteActual = new Label();
            lblSubtotal = new Label();
            lblDescuentoTotal = new Label();
            lblItbis = new Label();
            lblTotal = new Label();

            SuspendLayout();

            // ============================================================
            // FORM
            // ============================================================
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = cBg;
            ClientSize = new Size(1300, 790);
            Font = new Font("Segoe UI", 9F);
            KeyPreview = true;
            MinimumSize = new Size(1060, 680);
            Name = "FormFacturaV";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Facturación — Andloe";

            // ============================================================
            // ROOT
            // ============================================================
            pnlRoot.Dock = DockStyle.Fill;
            pnlRoot.BackColor = cBg;
            Controls.Add(pnlRoot);

            // ============================================================
            // HEADER — degradado marino → azul
            // ============================================================
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Height = 72;
            pnlHeader.BackColor = cHeaderBg1;
            pnlHeader.Paint += (_, e) =>
            {
                using var br = new LinearGradientBrush(
                    pnlHeader.ClientRectangle, cHeaderBg1, cHeaderBg2,
                    LinearGradientMode.Horizontal);
                e.Graphics.FillRectangle(br, pnlHeader.ClientRectangle);
                using var shadowBr = new LinearGradientBrush(
                    new Rectangle(0, pnlHeader.Height - 4, pnlHeader.Width, 4),
                    Color.FromArgb(60, 0, 0, 0), Color.Transparent,
                    LinearGradientMode.Vertical);
                e.Graphics.FillRectangle(shadowBr,
                    new Rectangle(0, pnlHeader.Height - 4, pnlHeader.Width, 4));
            };

            var lblIcon = new Label
            {
                Text = "🧾",
                Font = new Font("Segoe UI Emoji", 20F),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(20, 14)
            };
            pnlHeader.Controls.Add(lblIcon);

            lblTitle.AutoSize = true;
            lblTitle.Text = "Facturación";
            lblTitle.Font = fTitle;
            lblTitle.ForeColor = Color.White;
            lblTitle.Location = new Point(64, 10);

            lblSubTitle.AutoSize = true;
            lblSubTitle.Text = "Cotización  ·  Proforma  ·  Factura (RI)";
            lblSubTitle.Font = fSub;
            lblSubTitle.ForeColor = Color.FromArgb(180, 210, 255);
            lblSubTitle.Location = new Point(66, 42);

            pnlHeader.Controls.Add(lblTitle);
            pnlHeader.Controls.Add(lblSubTitle);

            // ============================================================
            // CONTENT WRAPPER (debajo del header)
            // ============================================================
            var pnlContent = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(14, 12, 14, 0),
                BackColor = cBg
            };
            pnlRoot.Controls.Add(pnlContent);
            pnlRoot.Controls.Add(pnlHeader);  // header al final para que quede arriba

            // ============================================================
            // BOTTOM — barra de botones
            // ============================================================
            pnlBottom = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 70,
                BackColor = cBg,
                Padding = new Padding(0, 6, 0, 0)
            };

            cardBottom = MakeCard(cCard, cLine);
            cardBottom.Padding = new Padding(10, 0, 10, 0);

            flowBottom.Dock = DockStyle.Fill;
            flowBottom.FlowDirection = FlowDirection.RightToLeft;
            flowBottom.WrapContents = false;

            StyleBtnPrimary(btnFinalizar, "✔  Finalizar (F12)", cAccent);
            StyleBtnPrimary(btnRegistrarFactura, "📋  Registrar Factura", cAccent);
            btnFinalizar.Width = 160;
            btnRegistrarFactura.Width = 175;

            StyleBtnDefault(btnImprimir, "🖨  Imprimir", cText, cCard, cLine);
            StyleBtnDefault(btnNuevo, "➕  Nuevo (Ctrl+N)", cText, cCard, cLine);
            StyleBtnDefault(btnAgregar, "🔖  Agregar", cText, cCard, cLine);
            StyleBtnDefault(btnGuardar, "💾  Guardar (Ctrl+S)", cText, cCard, cLine);
            StyleBtnDefault(btnCerrar, "✖  Cerrar", cMuted, cCard, cLine);

            StyleBtnDanger(btnEliminarLinea, "🗑  Eliminar línea", Color.FromArgb(220, 38, 38));
            StyleBtnDanger(btnAnular, "⛔  Anular", Color.FromArgb(220, 38, 38));
            btnAnular.Width = 120;
            btnEliminarLinea.Width = 148;

            flowBottom.Controls.Add(btnCerrar);
            flowBottom.Controls.Add(btnEliminarLinea);
            flowBottom.Controls.Add(btnAnular);
            flowBottom.Controls.Add(btnFinalizar);
            flowBottom.Controls.Add(btnGuardar);
            flowBottom.Controls.Add(btnAgregar);
            flowBottom.Controls.Add(btnNuevo);
            flowBottom.Controls.Add(btnRegistrarFactura);
            flowBottom.Controls.Add(btnImprimir);

            cardBottom.Controls.Add(flowBottom);
            pnlBottom.Controls.Add(cardBottom);

            // ============================================================
            // TOP — cliente + datos doc (altura ampliada = 7 filas left)
            // ============================================================
            pnlTop = new Panel
            {
                Dock = DockStyle.Top,
                Height = 320,
                BackColor = cBg,
                Padding = new Padding(0, 0, 0, 10)
            };

            cardTop = MakeCard(cCard, cLine);
            cardTop.Padding = new Padding(16, 12, 16, 12);
            pnlTop.Controls.Add(cardTop);

            tblTop.Dock = DockStyle.Fill;
            tblTop.ColumnCount = 2;
            tblTop.RowCount = 1;
            tblTop.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 54F));
            tblTop.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 46F));
            tblTop.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            cardTop.Controls.Add(tblTop);

            // ============================================================
            // LEFT — 7 filas: buscar, código, nombre, dirección, RNC,
            //                  tipo comprobante, estado fiscal
            // ============================================================
            var pnlLeftWrap = new Panel { Dock = DockStyle.Fill, Padding = new Padding(0, 0, 16, 0) };
            var lblSectionCliente = MakeSectionHeader("👤  Datos del Cliente", cAccent, cAccentLight);
            pnlLeftWrap.Controls.Add(lblSectionCliente);

            tblLeft.Dock = DockStyle.Fill;
            tblLeft.ColumnCount = 3;
            tblLeft.RowCount = 7;
            tblLeft.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 155F));
            tblLeft.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tblLeft.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 44F));
            tblLeft.Padding = new Padding(0, 30, 0, 0);
            for (int i = 0; i < 7; i++)
                tblLeft.RowStyles.Add(new RowStyle(SizeType.Absolute, 36F));

            // Labels
            SetFieldLabel(lblClienteN, "Buscar cliente", fLabel, cMuted);
            SetFieldLabel(lblClienteCodigo, "Código cliente", fLabel, cMuted);
            SetFieldLabel(lblCliNombre, "Nombre cliente", fLabel, cMuted);
            SetFieldLabel(lblCliDireccion, "Dirección", fLabel, cMuted);
            SetFieldLabel(lblCliRnc, "RNC / Cédula", fLabel, cMuted);
            SetFieldLabel(lblTipoComprobante, "Tipo comprobante", fLabel, cMuted);
            SetFieldLabel(lblEstadoFiscal, "Estado fiscal", fLabel, cMuted);

            // Inputs
            SetInput(txtClienteBuscar, fInput, cText);
            SetInput(txtClienteCodigo, fInput, cText, readOnly: true, bg: Color.FromArgb(248, 250, 253));
            SetInput(txtClienteNombre, fInput, cText, readOnly: true, bg: Color.FromArgb(248, 250, 253));
            SetInput(txtClienteDireccion, fInput, cText);
            SetInput(txtClienteRnc, fInput, cText, readOnly: true, bg: Color.FromArgb(248, 250, 253));

            // txtEstadoFiscal — lectura con color semántico
            SetInput(txtEstadoFiscal, new Font("Segoe UI", 8.5F, FontStyle.Bold), Color.FromArgb(22, 163, 74),
                readOnly: true, bg: Color.FromArgb(240, 253, 244));

            // Botón buscar
            StyleIconBtn(btnBuscarCliente, "🔍", cAccent);

            // cboTipoComprobante
            cboTipoComprobante.Dock = DockStyle.Fill;
            cboTipoComprobante.DropDownStyle = ComboBoxStyle.DropDownList;
            cboTipoComprobante.Font = fInput;
            cboTipoComprobante.Margin = new Padding(3, 4, 3, 4);

            // Agregar controles — fila 0: buscar
            tblLeft.Controls.Add(lblClienteN, 0, 0);
            tblLeft.Controls.Add(txtClienteBuscar, 1, 0);
            tblLeft.Controls.Add(btnBuscarCliente, 2, 0);

            // fila 1: código (span 2)
            tblLeft.Controls.Add(lblClienteCodigo, 0, 1);
            tblLeft.Controls.Add(txtClienteCodigo, 1, 1);
            tblLeft.SetColumnSpan(txtClienteCodigo, 2);

            // fila 2: nombre (span 2)
            tblLeft.Controls.Add(lblCliNombre, 0, 2);
            tblLeft.Controls.Add(txtClienteNombre, 1, 2);
            tblLeft.SetColumnSpan(txtClienteNombre, 2);

            // fila 3: dirección (span 2)
            tblLeft.Controls.Add(lblCliDireccion, 0, 3);
            tblLeft.Controls.Add(txtClienteDireccion, 1, 3);
            tblLeft.SetColumnSpan(txtClienteDireccion, 2);

            // fila 4: RNC (span 2)
            tblLeft.Controls.Add(lblCliRnc, 0, 4);
            tblLeft.Controls.Add(txtClienteRnc, 1, 4);
            tblLeft.SetColumnSpan(txtClienteRnc, 2);

            // fila 5: tipo comprobante (span 2)
            tblLeft.Controls.Add(lblTipoComprobante, 0, 5);
            tblLeft.Controls.Add(cboTipoComprobante, 1, 5);
            tblLeft.SetColumnSpan(cboTipoComprobante, 2);

            // fila 6: estado fiscal (span 2)
            tblLeft.Controls.Add(lblEstadoFiscal, 0, 6);
            tblLeft.Controls.Add(txtEstadoFiscal, 1, 6);
            tblLeft.SetColumnSpan(txtEstadoFiscal, 2);

            pnlLeftWrap.Controls.Add(tblLeft);
            tblTop.Controls.Add(pnlLeftWrap, 0, 0);

            // ============================================================
            // RIGHT WRAP (top: totales, bottom: datos doc)
            // ============================================================
            tblRightWrap.Dock = DockStyle.Fill;
            tblRightWrap.ColumnCount = 1;
            tblRightWrap.RowCount = 2;
            tblRightWrap.RowStyles.Add(new RowStyle(SizeType.Absolute, 100F));
            tblRightWrap.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tblRightWrap.Padding = new Padding(16, 0, 0, 0);

            // ---- Totales card ----
            var pnlTotCard = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(249, 250, 255),
                Padding = new Padding(10, 8, 10, 8)
            };
            pnlTotCard.Paint += (_, e) =>
            {
                using var pen = new Pen(cLine);
                var r = pnlTotCard.ClientRectangle;
                r.Width--; r.Height--;
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                DrawRoundRect(e.Graphics, pen, r, 8);
            };

            tblTopRight.Dock = DockStyle.Fill;
            tblTopRight.ColumnCount = 12;
            tblTopRight.RowCount = 2;
            tblTopRight.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tblTopRight.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tblTopRight.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 78F));
            tblTopRight.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 135F));
            tblTopRight.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 66F));
            tblTopRight.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 115F));
            tblTopRight.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 42F));
            tblTopRight.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 92F));
            tblTopRight.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 42F));
            tblTopRight.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 92F));
            tblTopRight.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 50F));
            tblTopRight.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 92F));
            tblTopRight.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 54F));
            tblTopRight.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            SetFieldLabel(lblNumeroFacturaTop, "Factura #", fTotLbl, cMuted);
            SetInput(txtNumeroFacturaTop, new Font("Segoe UI", 11F, FontStyle.Bold), cAccent,
                readOnly: true, bg: Color.White);
            txtNumeroFacturaTop.TextAlign = HorizontalAlignment.Center;

            SetFieldLabel(lblFechaEmision, "Emisión", fTotLbl, cMuted);
            dtpFechaDoc.Format = DateTimePickerFormat.Short;
            dtpFechaDoc.Dock = DockStyle.Fill;
            dtpFechaDoc.Font = fInput;
            dtpFechaDoc.Margin = new Padding(3, 4, 3, 4);

            SetTotLabel(lblTotSubtotal, "SUB", fTotLbl, cMuted);
            SetTotLabel(lblTotDescuento, "DESC", fTotLbl, cMuted);
            SetTotLabel(lblTotItbis, "ITBIS", fTotLbl, cMuted);
            SetTotLabel(lblTotTotal, "TOTAL", fTotLbl, cAccent);

            SetTotBox(txtSubtotal, fTotVal, cText);
            SetTotBox(txtDescuentoTotal, fTotVal, cText);
            SetTotBox(txtItbisTotal, fTotVal, cText);
            SetTotBox(txtTotalGeneral, fTotMain, cAccent);
            txtTotalGeneral.BackColor = cAccentLight;

            tblTopRight.Controls.Add(lblNumeroFacturaTop, 0, 0);
            tblTopRight.Controls.Add(txtNumeroFacturaTop, 1, 0);
            tblTopRight.SetRowSpan(txtNumeroFacturaTop, 2);
            tblTopRight.Controls.Add(lblFechaEmision, 2, 0);
            tblTopRight.Controls.Add(dtpFechaDoc, 3, 0);
            tblTopRight.SetRowSpan(dtpFechaDoc, 2);
            tblTopRight.Controls.Add(lblTotSubtotal, 4, 0);
            tblTopRight.Controls.Add(txtSubtotal, 5, 0);
            tblTopRight.Controls.Add(lblTotDescuento, 6, 0);
            tblTopRight.Controls.Add(txtDescuentoTotal, 7, 0);
            tblTopRight.Controls.Add(lblTotItbis, 8, 0);
            tblTopRight.Controls.Add(txtItbisTotal, 9, 0);
            tblTopRight.Controls.Add(lblTotTotal, 10, 0);
            tblTopRight.Controls.Add(txtTotalGeneral, 11, 0);
            tblTopRight.SetRowSpan(txtTotalGeneral, 2);

            pnlTotCard.Controls.Add(tblTopRight);
            tblRightWrap.Controls.Add(pnlTotCard, 0, 0);

            // ---- Datos del documento ----
            var pnlRightBody = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(0, 10, 0, 0),
                BackColor = Color.Transparent
            };
            var lblSectionDoc = MakeSectionHeader("📄  Datos del Documento", cAccent, cAccentLight);
            pnlRightBody.Controls.Add(lblSectionDoc);

            // 6 filas: fecha registro, tipo doc, crédito, término pago, días crédito, vendedor
            tblRight.Dock = DockStyle.Fill;
            tblRight.ColumnCount = 2;
            tblRight.RowCount = 6;
            tblRight.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 148F));
            tblRight.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tblRight.Padding = new Padding(0, 30, 0, 0);
            for (int i = 0; i < 6; i++)
                tblRight.RowStyles.Add(new RowStyle(SizeType.Absolute, 36F));

            SetFieldLabel(lblFechaRegistro, "Fecha registro", fLabel, cMuted);
            dtpFechaRegistro.Format = DateTimePickerFormat.Short;
            dtpFechaRegistro.Dock = DockStyle.Fill;
            dtpFechaRegistro.Enabled = false;
            dtpFechaRegistro.Font = fInput;
            dtpFechaRegistro.Margin = new Padding(3, 4, 3, 4);

            SetFieldLabel(lblTipoDoc, "Tipo documento", fLabel, cMuted);
            cboTipoDoc.Dock = DockStyle.Fill;
            cboTipoDoc.DropDownStyle = ComboBoxStyle.DropDownList;
            cboTipoDoc.Font = fInput;
            cboTipoDoc.Margin = new Padding(3, 4, 3, 4);

            SetFieldLabel(lblCredito, "Crédito", fLabel, cMuted);
            chkCredito.Dock = DockStyle.Left;
            chkCredito.Margin = new Padding(3, 8, 0, 4);

            SetFieldLabel(lblTerminoPago, "Término pago", fLabel, cMuted);
            cboTerminoPago.Dock = DockStyle.Fill;
            cboTerminoPago.DropDownStyle = ComboBoxStyle.DropDownList;
            cboTerminoPago.Font = fInput;
            cboTerminoPago.Margin = new Padding(3, 4, 3, 4);

            SetFieldLabel(lblDiasCredito, "Días crédito", fLabel, cMuted);
            SetInput(txtDiasCredito, fInput, cText);

            SetFieldLabel(lblVendedor, "Vendedor", fLabel, cMuted);
            cboVendedor.Dock = DockStyle.Fill;
            cboVendedor.DropDownStyle = ComboBoxStyle.DropDownList;
            cboVendedor.Font = fInput;
            cboVendedor.Margin = new Padding(3, 4, 3, 4);

            tblRight.Controls.Add(lblFechaRegistro, 0, 0);
            tblRight.Controls.Add(dtpFechaRegistro, 1, 0);
            tblRight.Controls.Add(lblTipoDoc, 0, 1);
            tblRight.Controls.Add(cboTipoDoc, 1, 1);
            tblRight.Controls.Add(lblCredito, 0, 2);
            tblRight.Controls.Add(chkCredito, 1, 2);
            tblRight.Controls.Add(lblTerminoPago, 0, 3);
            tblRight.Controls.Add(cboTerminoPago, 1, 3);
            tblRight.Controls.Add(lblDiasCredito, 0, 4);
            tblRight.Controls.Add(txtDiasCredito, 1, 4);
            tblRight.Controls.Add(lblVendedor, 0, 5);
            tblRight.Controls.Add(cboVendedor, 1, 5);

            pnlRightBody.Controls.Add(tblRight);
            tblRightWrap.Controls.Add(pnlRightBody, 0, 1);
            tblTop.Controls.Add(tblRightWrap, 1, 0);

            // ============================================================
            // MID — DataGridView
            // ============================================================
            pnlMid = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = cBg,
                Padding = new Padding(0, 0, 0, 10)
            };

            cardMid = MakeCard(cCard, cLine);
            cardMid.Padding = new Padding(0);

            var pnlGridHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 38,
                BackColor = Color.FromArgb(248, 250, 252)
            };
            pnlGridHeader.Paint += (_, e) =>
            {
                using var pen = new Pen(cLine);
                e.Graphics.DrawLine(pen, 0, pnlGridHeader.Height - 1,
                    pnlGridHeader.Width, pnlGridHeader.Height - 1);
            };
            var lblGridTitle = new Label
            {
                Text = "📦  Detalle de Productos / Servicios",
                AutoSize = false,
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = cText,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(12, 0, 0, 0)
            };
            pnlGridHeader.Controls.Add(lblGridTitle);

            grid.Dock = DockStyle.Fill;
            grid.BackgroundColor = Color.White;
            grid.BorderStyle = BorderStyle.None;
            grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            grid.GridColor = Color.FromArgb(232, 236, 242);
            grid.AllowUserToResizeRows = false;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.EditMode = DataGridViewEditMode.EditOnEnter;
            grid.MultiSelect = false;
            grid.SelectionMode = DataGridViewSelectionMode.CellSelect;
            grid.RowHeadersWidth = 24;
            grid.RowHeadersDefaultCellStyle.BackColor = cGridHeader;
            grid.RowHeadersDefaultCellStyle.SelectionBackColor = cAccentLight;
            grid.EnableHeadersVisualStyles = false;
            grid.ColumnHeadersDefaultCellStyle.BackColor = cGridHeader;
            grid.ColumnHeadersDefaultCellStyle.ForeColor = cText;
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            grid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            grid.ColumnHeadersHeight = 36;
            grid.DefaultCellStyle.Font = new Font("Segoe UI", 9F);
            grid.DefaultCellStyle.SelectionBackColor = cGridSel;
            grid.DefaultCellStyle.SelectionForeColor = cText;
            grid.DefaultCellStyle.Padding = new Padding(4, 0, 4, 0);
            grid.RowTemplate.Height = 30;
            grid.AlternatingRowsDefaultCellStyle.BackColor = cGridAlt;

            // Columnas ocultas
            colDetId.HeaderText = "DetId"; colDetId.Name = "colDetId"; colDetId.Visible = false;
            colImpuestoId.HeaderText = "ImpId"; colImpuestoId.Name = "colImpuestoId"; colImpuestoId.Visible = false;

            // Columnas visibles
            colProductoCodigo.FillWeight = 85F; colProductoCodigo.HeaderText = "Código"; colProductoCodigo.Name = "colProductoCodigo";
            colCodBarra.FillWeight = 110F; colCodBarra.HeaderText = "Cód. Barra"; colCodBarra.Name = "colCodBarra";
            colDescripcion.FillWeight = 260F; colDescripcion.HeaderText = "Descripción"; colDescripcion.Name = "colDescripcion";

            colUnidad.FillWeight = 65F; colUnidad.HeaderText = "Unidad"; colUnidad.Name = "colUnidad";
            colUnidad.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            colCantidad.FillWeight = 75F; colCantidad.HeaderText = "Cantidad"; colCantidad.Name = "colCantidad";
            colCantidad.DefaultCellStyle.Format = "N2";
            colCantidad.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            colPrecio.FillWeight = 90F; colPrecio.HeaderText = "Precio"; colPrecio.Name = "colPrecio";
            colPrecio.DefaultCellStyle.Format = "N2";
            colPrecio.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            colDescuentoPct.FillWeight = 65F; colDescuentoPct.HeaderText = "% Dto."; colDescuentoPct.Name = "colDescuentoPct";
            colDescuentoPct.DefaultCellStyle.Format = "N2";
            colDescuentoPct.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            colDescuentoMonto.FillWeight = 88F; colDescuentoMonto.HeaderText = "Desc. Monto"; colDescuentoMonto.Name = "colDescuentoMonto";
            colDescuentoMonto.ReadOnly = true;
            colDescuentoMonto.DefaultCellStyle.Format = "N2";
            colDescuentoMonto.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            colItbisPct.FillWeight = 65F; colItbisPct.HeaderText = "ITBIS %"; colItbisPct.Name = "colItbisPct";
            colItbisPct.DefaultCellStyle.Format = "N2";
            colItbisPct.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            colItbisMonto.FillWeight = 90F; colItbisMonto.HeaderText = "ITBIS"; colItbisMonto.Name = "colItbisMonto";
            colItbisMonto.ReadOnly = true;
            colItbisMonto.DefaultCellStyle.Format = "N2";
            colItbisMonto.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            colTotalLinea.FillWeight = 92F; colTotalLinea.HeaderText = "Importe"; colTotalLinea.Name = "colTotalLinea";
            colTotalLinea.ReadOnly = true;
            colTotalLinea.DefaultCellStyle.Format = "N2";
            colTotalLinea.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            colTotalLinea.DefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            colTotalLinea.DefaultCellStyle.ForeColor = cAccent;

            grid.Columns.AddRange(new DataGridViewColumn[]
            {
                colDetId, colImpuestoId,
                colProductoCodigo, colCodBarra, colDescripcion, colUnidad,
                colCantidad, colPrecio,
                colDescuentoPct, colDescuentoMonto,
                colItbisPct, colItbisMonto,
                colTotalLinea
            });

            cardMid.Controls.Add(grid);
            cardMid.Controls.Add(pnlGridHeader);
            pnlMid.Controls.Add(cardMid);

            // ============================================================
            // Hidden info
            // ============================================================
            lblInfoFacturaId.Visible = false; txtFacturaIdInfo.Visible = false;
            lblInfoTipo.Visible = false; txtTipoInfo.Visible = false;
            lblInfoEstado.Visible = false; txtEstadoInfo.Visible = false;

            // ============================================================
            // Compose
            // ============================================================
            pnlContent.Controls.Add(pnlMid);
            pnlContent.Controls.Add(pnlBottom);
            pnlContent.Controls.Add(pnlTop);

            ResumeLayout(false);
        }

        // ================================================================
        // HELPERS DE DISEÑO (también usados desde FormFacturaV.cs)
        // ================================================================

        private Panel MakeCard(Color bg, Color border)
        {
            var p = new Panel { Dock = DockStyle.Fill, BackColor = bg };
            p.Paint += (_, e) =>
            {
                var r = p.ClientRectangle;
                r.Width--; r.Height--;
                using var pen = new Pen(border);
                e.Graphics.DrawRectangle(pen, r);
            };
            return p;
        }

        private Label MakeSectionHeader(string text, Color fg, Color bgChip)
        {
            return new Label
            {
                Text = text,
                AutoSize = false,
                Height = 26,
                Dock = DockStyle.Top,
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                ForeColor = fg,
                BackColor = bgChip,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(8, 0, 0, 0)
            };
        }

        private void SetFieldLabel(Label lbl, string text, Font f, Color fg)
        {
            lbl.AutoSize = true;
            lbl.Text = text;
            lbl.Font = f;
            lbl.ForeColor = fg;
            lbl.Padding = new Padding(0, 10, 0, 0);
        }

        private void SetInput(TextBox txt, Font f, Color fg,
            bool readOnly = false, Color? bg = null)
        {
            txt.Dock = DockStyle.Fill;
            txt.BorderStyle = BorderStyle.FixedSingle;
            txt.Font = f;
            txt.ForeColor = fg;
            txt.BackColor = bg ?? Color.White;
            txt.ReadOnly = readOnly;
            if (readOnly) txt.TabStop = false;
            txt.Margin = new Padding(3, 4, 3, 4);
        }

        private void StyleIconBtn(Button b, string icon, Color accent)
        {
            b.Text = icon;
            b.Dock = DockStyle.Fill;
            b.FlatStyle = FlatStyle.Flat;
            b.FlatAppearance.BorderColor = accent;
            b.FlatAppearance.BorderSize = 1;
            b.BackColor = Color.FromArgb(219, 234, 254);
            b.ForeColor = accent;
            b.Font = new Font("Segoe UI Emoji", 12F);
            b.Margin = new Padding(4, 4, 0, 4);
            b.Cursor = Cursors.Hand;
        }

        private void SetTotLabel(Label lbl, string text, Font f, Color fg)
        {
            lbl.AutoSize = true;
            lbl.Text = text;
            lbl.Font = f;
            lbl.ForeColor = fg;
            lbl.TextAlign = ContentAlignment.BottomRight;
            lbl.Padding = new Padding(0, 0, 4, 0);
            lbl.Dock = DockStyle.Fill;
        }

        private void SetTotBox(TextBox txt, Font f, Color fg)
        {
            txt.BorderStyle = BorderStyle.FixedSingle;
            txt.ReadOnly = true;
            txt.TabStop = false;
            txt.TextAlign = HorizontalAlignment.Right;
            txt.Dock = DockStyle.Fill;
            txt.BackColor = Color.White;
            txt.Font = f;
            txt.ForeColor = fg;
            txt.Margin = new Padding(3, 6, 3, 6);
        }

        /// <summary>
        /// ✅ Requerido por FormFacturaV.cs → AplicarEstiloModernoFormulario().
        /// Aplica estilo visual consistente a cualquier ComboBox.
        /// </summary>
        private void StyleCombo(ComboBox cbo)
        {
            if (cbo == null) return;
            cbo.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular);
            cbo.FlatStyle = FlatStyle.Flat;
            cbo.BackColor = Color.White;
            cbo.ForeColor = Color.FromArgb(22, 28, 45);
            cbo.Cursor = Cursors.Hand;
            if (cbo.Margin == Padding.Empty)
                cbo.Margin = new Padding(3, 4, 3, 4);
        }

        private void BaseBtn(Button b)
        {
            b.Height = 42;
            b.Width = 130;
            b.Margin = new Padding(8, 0, 0, 0);
            b.Cursor = Cursors.Hand;
            b.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            b.FlatStyle = FlatStyle.Flat;
            b.FlatAppearance.BorderSize = 1;
        }

        private void StyleBtnDefault(Button b, string text, Color fg, Color bg, Color border)
        {
            BaseBtn(b);
            b.Text = text;
            b.BackColor = bg;
            b.ForeColor = fg;
            b.FlatAppearance.BorderColor = border;
        }

        private void StyleBtnPrimary(Button b, string text, Color accent)
        {
            BaseBtn(b);
            b.Text = text;
            b.BackColor = accent;
            b.ForeColor = Color.White;
            b.FlatAppearance.BorderColor = accent;
        }

        private void StyleBtnDanger(Button b, string text, Color danger)
        {
            BaseBtn(b);
            b.Text = text;
            b.BackColor = Color.White;
            b.ForeColor = danger;
            b.FlatAppearance.BorderColor = Color.FromArgb(252, 205, 205);
        }

        private void DrawRoundRect(Graphics g, Pen pen, Rectangle r, int radius)
        {
            using var path = GetRoundRectPath(r, radius);
            g.DrawPath(pen, path);
        }

        private System.Drawing.Drawing2D.GraphicsPath GetRoundRectPath(Rectangle r, int radius)
        {
            var path = new System.Drawing.Drawing2D.GraphicsPath();
            int d = radius * 2;
            path.AddArc(r.X, r.Y, d, d, 180, 90);
            path.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            path.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            path.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }
    }
}