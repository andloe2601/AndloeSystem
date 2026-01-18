using System;
using System.Drawing;
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

        private TableLayoutPanel tblTop;         // 2 cols
        private TableLayoutPanel tblLeft;        // izquierda
        private TableLayoutPanel tblRightWrap;   // derecha: fila top + datos abajo
        private TableLayoutPanel tblTopRight;    // fila top derecha: Numero + Emision + Totales
        private TableLayoutPanel tblRight;       // datos doc abajo

        // ====== LEFT (cliente) ======
        private Label lblClienteN;
        public TextBox txtClienteBuscar;
        public Button btnBuscarCliente;

        private Label lblCliNombre;
        public TextBox txtClienteNombre;

        private Label lblCliDireccion;
        public TextBox txtClienteDireccion;

        private Label lblCliRnc;
        public TextBox txtClienteRnc;

        private Label lblTipoComprobante;
        public Button btnComprobanteFiscal;
        public TextBox txtTipoComprobante;

        // ====== HIDDEN INFO (compat / lógica) ======
        private Label lblInfoFacturaId;
        public TextBox txtFacturaIdInfo;

        private Label lblInfoEstado;
        public TextBox txtEstadoInfo;

        private Label lblInfoTipo;
        public TextBox txtTipoInfo;

        // ====== RIGHT ======
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

        // ✅ NUEVO: botón anular
        public Button btnAnular;

        // (compat: no se usan)
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

            // ===== Theme =====
            var cAppBg = Color.WhiteSmoke;
            var cCard = Color.White;
            var cLine = Color.FromArgb(225, 225, 225);
            var cText = Color.FromArgb(25, 25, 25);
            var cMuted = Color.FromArgb(110, 110, 110);
            var cAccent = Color.FromArgb(45, 125, 255);

            // ===== Root =====
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

            // LEFT
            lblClienteN = new Label();
            txtClienteBuscar = new TextBox();
            btnBuscarCliente = new Button();

            lblCliNombre = new Label();
            txtClienteNombre = new TextBox();

            lblCliDireccion = new Label();
            txtClienteDireccion = new TextBox();

            lblCliRnc = new Label();
            txtClienteRnc = new TextBox();

            lblTipoComprobante = new Label();
            btnComprobanteFiscal = new Button();
            txtTipoComprobante = new TextBox();

            // Hidden
            lblInfoFacturaId = new Label();
            txtFacturaIdInfo = new TextBox();

            lblInfoEstado = new Label();
            txtEstadoInfo = new TextBox();

            lblInfoTipo = new Label();
            txtTipoInfo = new TextBox();

            // Right top
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

            // Right body
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

            // Grid
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

            // Bottom
            flowBottom = new FlowLayoutPanel();
            btnCerrar = new Button();
            btnEliminarLinea = new Button();
            btnFinalizar = new Button();
            btnGuardar = new Button();
            btnAgregar = new Button();
            btnNuevo = new Button();
            btnRegistrarFactura = new Button();
            btnImprimir = new Button();

            // ✅ nuevo
            btnAnular = new Button();

            // compat labels (unused)
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
            BackColor = cAppBg;
            ClientSize = new Size(1180, 720);
            Font = new Font("Segoe UI", 9F);
            KeyPreview = true;
            MinimumSize = new Size(980, 620);
            Name = "FormFacturaV";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Facturación - Andloe";

            // ============================================================
            // ROOT PANEL
            // ============================================================
            pnlRoot.Dock = DockStyle.Fill;
            pnlRoot.BackColor = cAppBg;
            pnlRoot.Padding = new Padding(12);
            Controls.Add(pnlRoot);

            // ============================================================
            // HEADER
            // ============================================================
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Height = 58;
            pnlHeader.BackColor = cAppBg;

            lblTitle.AutoSize = true;
            lblTitle.Text = "Facturación";
            lblTitle.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            lblTitle.ForeColor = cText;
            lblTitle.Location = new Point(2, 2);

            lblSubTitle.AutoSize = true;
            lblSubTitle.Text = "Cotización · Proforma · Factura (RI)";
            lblSubTitle.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular);
            lblSubTitle.ForeColor = cMuted;
            lblSubTitle.Location = new Point(4, 32);

            pnlHeader.Controls.Add(lblTitle);
            pnlHeader.Controls.Add(lblSubTitle);

            // ============================================================
            // pnlTop / cardTop
            // ============================================================
            pnlTop.Dock = DockStyle.Top;
            pnlTop.Height = 245;
            pnlTop.BackColor = cAppBg;
            pnlTop.Padding = new Padding(0, 8, 0, 10);

            cardTop.Dock = DockStyle.Fill;
            cardTop.BackColor = cCard;
            cardTop.Padding = new Padding(12);
            cardTop.Paint += (_, e) =>
            {
                using var pen = new Pen(cLine);
                var r = cardTop.ClientRectangle;
                r.Width -= 1; r.Height -= 1;
                e.Graphics.DrawRectangle(pen, r);
            };

            pnlTop.Controls.Add(cardTop);

            // ============================================================
            // tblTop (2 cols)
            // ============================================================
            tblTop.Dock = DockStyle.Fill;
            tblTop.ColumnCount = 2;
            tblTop.RowCount = 1;
            tblTop.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 52F));
            tblTop.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 48F));
            tblTop.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            cardTop.Controls.Add(tblTop);

            // ============================================================
            // LEFT table (cliente)
            // ============================================================
            tblLeft.Dock = DockStyle.Fill;
            tblLeft.ColumnCount = 3;
            tblLeft.RowCount = 5;

            tblLeft.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 180F));
            tblLeft.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tblLeft.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 44F));

            for (int i = 0; i < 5; i++)
                tblLeft.RowStyles.Add(new RowStyle(SizeType.Absolute, 34F));

            ConfigFieldLabel(lblClienteN, "Cliente (Código/RNC)", cMuted);
            ConfigFieldLabel(lblCliNombre, "Nombre cliente", cMuted);
            ConfigFieldLabel(lblCliDireccion, "Dirección", cMuted);
            ConfigFieldLabel(lblCliRnc, "RNC", cMuted);
            ConfigFieldLabel(lblTipoComprobante, "Comprobante fiscal", cMuted);

            ConfigInput(txtClienteBuscar);
            ConfigInfo(txtClienteNombre, readOnly: true);
            ConfigInput(txtClienteDireccion); // editable
            ConfigInfo(txtClienteRnc, readOnly: true);

            // botón buscar cliente
            ConfigMiniIcon(btnBuscarCliente, "🔎", cAccent);

            // CF button + tipo
            ConfigChipButton(btnComprobanteFiscal, "CF: No", cLine, cText);
            ConfigInput(txtTipoComprobante);
            txtTipoComprobante.ReadOnly = true;

            // add controls left
            tblLeft.Controls.Add(lblClienteN, 0, 0);
            tblLeft.Controls.Add(txtClienteBuscar, 1, 0);
            tblLeft.Controls.Add(btnBuscarCliente, 2, 0);

            tblLeft.Controls.Add(lblCliNombre, 0, 1);
            tblLeft.Controls.Add(txtClienteNombre, 1, 1);
            tblLeft.SetColumnSpan(txtClienteNombre, 2);

            tblLeft.Controls.Add(lblCliDireccion, 0, 2);
            tblLeft.Controls.Add(txtClienteDireccion, 1, 2);
            tblLeft.SetColumnSpan(txtClienteDireccion, 2);

            tblLeft.Controls.Add(lblCliRnc, 0, 3);
            tblLeft.Controls.Add(txtClienteRnc, 1, 3);
            tblLeft.SetColumnSpan(txtClienteRnc, 2);

            tblLeft.Controls.Add(lblTipoComprobante, 0, 4);
            tblLeft.Controls.Add(btnComprobanteFiscal, 1, 4);
            tblLeft.Controls.Add(txtTipoComprobante, 2, 4);

            // Ajuste: textbox tipo comprobante ocupa más
            tblLeft.ColumnStyles[2].Width = 44F;
            tblLeft.SetColumnSpan(txtTipoComprobante, 1);
            txtTipoComprobante.Dock = DockStyle.Fill;
            txtTipoComprobante.Margin = new Padding(10, 6, 0, 6);

            // hidden info fields (tu lógica los usa internamente)
            lblInfoFacturaId.Visible = false;
            txtFacturaIdInfo.Visible = false;
            lblInfoTipo.Visible = false;
            txtTipoInfo.Visible = false;

            // estado hidden
            lblInfoEstado.Visible = false;
            txtEstadoInfo.Visible = false;

            // ============================================================
            // RIGHT wrap
            // ============================================================
            tblRightWrap.Dock = DockStyle.Fill;
            tblRightWrap.ColumnCount = 1;
            tblRightWrap.RowCount = 2;
            tblRightWrap.RowStyles.Add(new RowStyle(SizeType.Absolute, 78F));
            tblRightWrap.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            // ============================================================
            // TOP RIGHT: Numero + Emision + Totales
            // ============================================================
            tblTopRight.Dock = DockStyle.Fill;
            tblTopRight.ColumnCount = 12;
            tblTopRight.RowCount = 2;
            tblTopRight.RowStyles.Add(new RowStyle(SizeType.Absolute, 34F));
            tblTopRight.RowStyles.Add(new RowStyle(SizeType.Absolute, 34F));

            // Numero
            tblTopRight.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 82F));   // lbl numero
            tblTopRight.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150F));  // txt numero

            // Emision
            tblTopRight.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 70F));   // lbl emision
            tblTopRight.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120F));  // dtp

            // Totales
            tblTopRight.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 46F));   // SUB lbl
            tblTopRight.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 98F));   // SUB txt
            tblTopRight.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 46F));   // DTO lbl
            tblTopRight.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 98F));   // DTO txt
            tblTopRight.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 52F));   // ITBIS lbl
            tblTopRight.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 98F));   // ITBIS txt
            tblTopRight.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 54F));   // TOTAL lbl
            tblTopRight.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));   // TOTAL txt

            ConfigFieldLabel(lblNumeroFacturaTop, "Factura #", cMuted);
            ConfigInfo(txtNumeroFacturaTop, readOnly: true);

            ConfigFieldLabel(lblFechaEmision, "Emisión", cMuted);
            dtpFechaDoc.Format = DateTimePickerFormat.Short;
            dtpFechaDoc.Dock = DockStyle.Fill;

            ConfigTotLabel(lblTotSubtotal, "SUB", cMuted, bold: true);
            ConfigTotLabel(lblTotDescuento, "DTO", cMuted, bold: true);
            ConfigTotLabel(lblTotItbis, "ITBIS", cMuted, bold: true);
            ConfigTotLabel(lblTotTotal, "TOTAL", cAccent, bold: true);

            ConfigTotBox(txtSubtotal, bold: true);
            ConfigTotBox(txtDescuentoTotal, bold: true);
            ConfigTotBox(txtItbisTotal, bold: true);
            ConfigTotBox(txtTotalGeneral, bold: true, accent: true);

            // Place TopRight
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

            // Divider
            var div = new Panel
            {
                Dock = DockStyle.Top,
                Height = 1,
                BackColor = cLine,
                Margin = new Padding(0, 6, 0, 6)
            };

            // ============================================================
            // RIGHT body table
            // ============================================================
            tblRight.Dock = DockStyle.Fill;
            tblRight.ColumnCount = 2;
            tblRight.RowCount = 5;
            tblRight.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 180F));
            tblRight.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            for (int i = 0; i < 5; i++)
                tblRight.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F));

            ConfigFieldLabel(lblFechaRegistro, "Fecha registro", cMuted);
            dtpFechaRegistro.Format = DateTimePickerFormat.Short;
            dtpFechaRegistro.Dock = DockStyle.Fill;
            dtpFechaRegistro.Enabled = false;

            ConfigFieldLabel(lblTipoDoc, "Tipo documento", cMuted);
            cboTipoDoc.Dock = DockStyle.Fill;
            cboTipoDoc.DropDownStyle = ComboBoxStyle.DropDownList;

            ConfigFieldLabel(lblCredito, "Crédito", cMuted);
            chkCredito.Dock = DockStyle.Left;

            ConfigFieldLabel(lblTerminoPago, "Término pago", cMuted);
            cboTerminoPago.Dock = DockStyle.Fill;
            cboTerminoPago.DropDownStyle = ComboBoxStyle.DropDownList;

            ConfigFieldLabel(lblDiasCredito, "Días crédito", cMuted);
            ConfigInput(txtDiasCredito);

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

            // Compose right wrap
            var rightBody = new Panel { Dock = DockStyle.Fill, Padding = new Padding(0, 8, 0, 0), BackColor = cCard };
            rightBody.Controls.Add(tblRight);
            rightBody.Controls.Add(div);

            tblRightWrap.Controls.Add(tblTopRight, 0, 0);
            tblRightWrap.Controls.Add(rightBody, 0, 1);

            // Add both sides
            tblTop.Controls.Add(tblLeft, 0, 0);
            tblTop.Controls.Add(tblRightWrap, 1, 0);

            // ============================================================
            // pnlMid / cardMid
            // ============================================================
            pnlMid.Dock = DockStyle.Fill;
            pnlMid.BackColor = cAppBg;
            pnlMid.Padding = new Padding(0, 0, 0, 10);

            cardMid.Dock = DockStyle.Fill;
            cardMid.BackColor = cCard;
            cardMid.Padding = new Padding(12);
            cardMid.Paint += (_, e) =>
            {
                using var pen = new Pen(cLine);
                var r = cardMid.ClientRectangle;
                r.Width -= 1; r.Height -= 1;
                e.Graphics.DrawRectangle(pen, r);
            };

            pnlMid.Controls.Add(cardMid);

            // Grid style "premium"
            grid.Dock = DockStyle.Fill;
            grid.BackgroundColor = Color.White;
            grid.BorderStyle = BorderStyle.None;
            grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            grid.GridColor = Color.FromArgb(238, 238, 238);

            grid.AllowUserToResizeRows = false;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.EditMode = DataGridViewEditMode.EditOnEnter;
            grid.MultiSelect = false;
            grid.SelectionMode = DataGridViewSelectionMode.CellSelect;
            grid.RowHeadersWidth = 26;

            grid.EnableHeadersVisualStyles = false;
            grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(246, 248, 252);
            grid.ColumnHeadersDefaultCellStyle.ForeColor = cText;
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            grid.ColumnHeadersHeight = 34;

            grid.DefaultCellStyle.Font = new Font("Segoe UI", 9F);
            grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(200, 225, 255);
            grid.DefaultCellStyle.SelectionForeColor = cText;

            grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(252, 252, 252);

            // Columns
            colDetId.HeaderText = "DetId";
            colDetId.Name = "colDetId";
            colDetId.Visible = false;

            colImpuestoId.HeaderText = "ImpId";
            colImpuestoId.Name = "colImpuestoId";
            colImpuestoId.Visible = false;

            colProductoCodigo.FillWeight = 90F;
            colProductoCodigo.HeaderText = "Código";
            colProductoCodigo.Name = "colProductoCodigo";

            colCodBarra.FillWeight = 120F;
            colCodBarra.HeaderText = "Cod. Barra";
            colCodBarra.Name = "colCodBarra";

            colDescripcion.FillWeight = 240F;
            colDescripcion.HeaderText = "Descripción";
            colDescripcion.Name = "colDescripcion";

            colUnidad.FillWeight = 70F;
            colUnidad.HeaderText = "Unidad";
            colUnidad.Name = "colUnidad";

            colCantidad.FillWeight = 80F;
            colCantidad.HeaderText = "Cantidad";
            colCantidad.Name = "colCantidad";
            colCantidad.DefaultCellStyle.Format = "N2";

            colPrecio.FillWeight = 90F;
            colPrecio.HeaderText = "Precio";
            colPrecio.Name = "colPrecio";
            colPrecio.DefaultCellStyle.Format = "N2";

            colDescuentoPct.FillWeight = 70F;
            colDescuentoPct.HeaderText = "% Dto.";
            colDescuentoPct.Name = "colDescuentoPct";
            colDescuentoPct.DefaultCellStyle.Format = "N2";

            colDescuentoMonto.FillWeight = 90F;
            colDescuentoMonto.HeaderText = "Desc. Monto";
            colDescuentoMonto.Name = "colDescuentoMonto";
            colDescuentoMonto.ReadOnly = true;
            colDescuentoMonto.DefaultCellStyle.Format = "N2";

            colItbisPct.FillWeight = 70F;
            colItbisPct.HeaderText = "ITBIS %";
            colItbisPct.Name = "colItbisPct";
            colItbisPct.DefaultCellStyle.Format = "N2";

            colItbisMonto.FillWeight = 95F;
            colItbisMonto.HeaderText = "ITBIS";
            colItbisMonto.Name = "colItbisMonto";
            colItbisMonto.ReadOnly = true;
            colItbisMonto.DefaultCellStyle.Format = "N2";

            colTotalLinea.FillWeight = 95F;
            colTotalLinea.HeaderText = "Importe";
            colTotalLinea.Name = "colTotalLinea";
            colTotalLinea.ReadOnly = true;
            colTotalLinea.DefaultCellStyle.Format = "N2";

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

            // ============================================================
            // pnlBottom / cardBottom
            // ============================================================
            pnlBottom.Dock = DockStyle.Bottom;
            pnlBottom.Height = 74;
            pnlBottom.BackColor = cAppBg;

            cardBottom.Dock = DockStyle.Fill;
            cardBottom.BackColor = cCard;
            cardBottom.Padding = new Padding(12);
            cardBottom.Paint += (_, e) =>
            {
                using var pen = new Pen(cLine);
                var r = cardBottom.ClientRectangle;
                r.Width -= 1; r.Height -= 1;
                e.Graphics.DrawRectangle(pen, r);
            };
            pnlBottom.Controls.Add(cardBottom);

            flowBottom.Dock = DockStyle.Fill;
            flowBottom.FlowDirection = FlowDirection.RightToLeft;
            flowBottom.WrapContents = false;
            flowBottom.Padding = new Padding(0);
            flowBottom.Margin = new Padding(0);

            // Premium buttons
            ConfigBtnPrimary(btnFinalizar, "Finalizar (F12)", cAccent);
            ConfigBtnPrimary(btnRegistrarFactura, "Registrar Factura", cAccent);

            ConfigBtnDefault(btnImprimir, "Imprimir");
            ConfigBtnDefault(btnNuevo, "Nuevo (Ctrl+N)");
            ConfigBtnDefault(btnAgregar, "Agregar");
            ConfigBtnDefault(btnGuardar, "Guardar (Ctrl+S)");
            ConfigBtnDanger(btnEliminarLinea, "Eliminar línea");
            ConfigBtnDefault(btnCerrar, "Cerrar");

            // ✅ Nuevo botón ANULAR (mismo estilo danger)
            ConfigBtnDanger(btnAnular, "Anular");
            btnAnular.Width = 120;

            btnRegistrarFactura.Width = 160;
            btnFinalizar.Width = 150;

            flowBottom.Controls.Add(btnCerrar);
            flowBottom.Controls.Add(btnEliminarLinea);

            // ✅ Orden: con RightToLeft, estos quedan visualmente cerca de Finalizar
            flowBottom.Controls.Add(btnAnular);

            flowBottom.Controls.Add(btnFinalizar);
            flowBottom.Controls.Add(btnGuardar);
            flowBottom.Controls.Add(btnAgregar);
            flowBottom.Controls.Add(btnNuevo);
            flowBottom.Controls.Add(btnRegistrarFactura);
            flowBottom.Controls.Add(btnImprimir);

            cardBottom.Controls.Add(flowBottom);

            // ============================================================
            // Compose root
            // ============================================================
            pnlRoot.Controls.Add(pnlMid);
            pnlRoot.Controls.Add(pnlBottom);
            pnlRoot.Controls.Add(pnlTop);
            pnlRoot.Controls.Add(pnlHeader);

            ResumeLayout(false);

            // ======== helpers =========
            void ConfigFieldLabel(Label lbl, string text, Color color)
            {
                lbl.AutoSize = true;
                lbl.Text = text;
                lbl.ForeColor = color;
                lbl.Font = new Font("Segoe UI", 8.5F, FontStyle.Regular);
                lbl.Padding = new Padding(0, 8, 0, 0);
            }

            void ConfigInput(TextBox txt)
            {
                txt.Dock = DockStyle.Fill;
                txt.BorderStyle = BorderStyle.FixedSingle;
                txt.BackColor = Color.White;
                txt.Margin = new Padding(3, 6, 0, 6);
            }

            void ConfigInfo(TextBox txt, bool readOnly)
            {
                txt.Dock = DockStyle.Fill;
                txt.BorderStyle = BorderStyle.FixedSingle;
                txt.ReadOnly = readOnly;
                txt.TabStop = false;
                txt.BackColor = Color.White;
                txt.Margin = new Padding(3, 6, 0, 6);
            }

            void ConfigMiniIcon(Button b, string text, Color accent)
            {
                b.Text = text;
                b.Dock = DockStyle.Fill;
                b.FlatStyle = FlatStyle.Flat;
                b.FlatAppearance.BorderColor = cLine;
                b.FlatAppearance.BorderSize = 1;
                b.BackColor = Color.White;
                b.ForeColor = accent;
                b.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
                b.Margin = new Padding(8, 6, 0, 6);
                b.Cursor = Cursors.Hand;
            }

            void ConfigChipButton(Button b, string text, Color border, Color fg)
            {
                b.Text = text;
                b.Height = 26;
                b.Dock = DockStyle.Left;
                b.FlatStyle = FlatStyle.Flat;
                b.FlatAppearance.BorderSize = 1;
                b.FlatAppearance.BorderColor = border;
                b.BackColor = Color.White;
                b.ForeColor = fg;
                b.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
                b.Margin = new Padding(3, 6, 0, 6);
                b.Width = 90;
                b.Cursor = Cursors.Hand;
            }

            void ConfigTotLabel(Label lbl, string text, Color color, bool bold)
            {
                lbl.AutoSize = true;
                lbl.Text = text;
                lbl.ForeColor = color;
                lbl.Font = new Font("Segoe UI", 8.5F, bold ? FontStyle.Bold : FontStyle.Regular);
                lbl.Padding = new Padding(0, 8, 6, 0);
            }

            void ConfigTotBox(TextBox txt, bool bold, bool accent = false)
            {
                txt.BorderStyle = BorderStyle.FixedSingle;
                txt.ReadOnly = true;
                txt.TabStop = false;
                txt.TextAlign = HorizontalAlignment.Right;
                txt.Dock = DockStyle.Fill;
                txt.BackColor = Color.White;
                txt.Font = new Font("Segoe UI", 10F, bold ? FontStyle.Bold : FontStyle.Regular);
                txt.Margin = new Padding(3, 6, 0, 6);

                if (accent)
                    txt.ForeColor = cAccent;
            }

            void BaseBtn(Button b)
            {
                b.Height = 38;
                b.Width = 130;
                b.Margin = new Padding(10, 0, 0, 0);
                b.Cursor = Cursors.Hand;
                b.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
                b.FlatStyle = FlatStyle.Flat;
                b.FlatAppearance.BorderSize = 1;
                b.FlatAppearance.BorderColor = cLine;
            }

            void ConfigBtnDefault(Button b, string text)
            {
                BaseBtn(b);
                b.Text = text;
                b.BackColor = Color.White;
                b.ForeColor = cText;
            }

            void ConfigBtnDanger(Button b, string text)
            {
                BaseBtn(b);
                b.Text = text;
                b.BackColor = Color.White;
                b.ForeColor = Color.FromArgb(190, 45, 45);
            }

            void ConfigBtnPrimary(Button b, string text, Color accent)
            {
                BaseBtn(b);
                b.Text = text;
                b.BackColor = accent;
                b.ForeColor = Color.White;
                b.FlatAppearance.BorderColor = accent;
            }
        }
    }
}
