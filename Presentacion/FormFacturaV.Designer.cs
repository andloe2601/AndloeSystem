// =====================================
// FormFacturaV.Designer.cs  (COMPLETO)
// ✅ SIN funciones locales dentro de InitializeComponent
// ✅ SIN lambdas en eventos (Designer-friendly)
// ✅ SIN btnComprobanteFiscal (solo ComboBox fijo)
// =====================================
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Andloe.Presentacion
{
    partial class FormFacturaV
    {
        private System.ComponentModel.IContainer components = null;

        // 🎨 Paleta (Designer-friendly)
        private readonly Color cAppBg = Color.WhiteSmoke;
        private readonly Color cCard = Color.White;
        private readonly Color cLine = Color.FromArgb(225, 225, 225);
        private readonly Color cText = Color.FromArgb(25, 25, 25);
        private readonly Color cMuted = Color.FromArgb(110, 110, 110);
        private readonly Color cAccent = Color.FromArgb(45, 125, 255);

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
        public ComboBox cboTipoComprobante;

        private Label lblInfoFacturaId;
        public TextBox txtFacturaIdInfo;

        private Label lblInfoEstado;
        public TextBox txtEstadoInfo;

        private Label lblInfoTipo;
        public TextBox txtTipoInfo;

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

        private Label lblVendedor;
        public ComboBox cboVendedor;

        private Label lblCredito;
        public CheckBox chkCredito;

        private Label lblTerminoPago;
        public ComboBox cboTerminoPago;

        private Label lblDiasCredito;
        public TextBox txtDiasCredito;

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
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle10 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle3 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle4 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle5 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle6 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle7 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle8 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle9 = new DataGridViewCellStyle();
            pnlRoot = new Panel();
            pnlMid = new Panel();
            cardMid = new Panel();
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
            pnlBottom = new Panel();
            cardBottom = new Panel();
            flowBottom = new FlowLayoutPanel();
            btnCerrar = new Button();
            btnEliminarLinea = new Button();
            btnAnular = new Button();
            btnFinalizar = new Button();
            btnGuardar = new Button();
            btnAgregar = new Button();
            btnNuevo = new Button();
            btnRegistrarFactura = new Button();
            btnImprimir = new Button();
            pnlTop = new Panel();
            cardTop = new Panel();
            tblTop = new TableLayoutPanel();
            tblLeft = new TableLayoutPanel();
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
            cboTipoComprobante = new ComboBox();
            tblRightWrap = new TableLayoutPanel();
            rightBody = new Panel();
            tblRight = new TableLayoutPanel();
            lblFechaRegistro = new Label();
            dtpFechaRegistro = new DateTimePicker();
            lblTipoDoc = new Label();
            cboTipoDoc = new ComboBox();
            lblVendedor = new Label();
            cboVendedor = new ComboBox();
            lblCredito = new Label();
            chkCredito = new CheckBox();
            lblTerminoPago = new Label();
            cboTerminoPago = new ComboBox();
            lblDiasCredito = new Label();
            txtDiasCredito = new TextBox();
            div = new Panel();
            tblTopRight = new TableLayoutPanel();
            lblNumeroFacturaTop = new Label();
            txtNumeroFacturaTop = new TextBox();
            dtpFechaDoc = new DateTimePicker();
            lblTotSubtotal = new Label();
            txtSubtotal = new TextBox();
            lblFechaEmision = new Label();
            txtDescuentoTotal = new TextBox();
            lblTotDescuento = new Label();
            lblTotItbis = new Label();
            txtItbisTotal = new TextBox();
            lblTotTotal = new Label();
            txtTotalGeneral = new TextBox();
            pnlHeader = new Panel();
            lblTitle = new Label();
            lblSubTitle = new Label();
            lblInfoFacturaId = new Label();
            txtFacturaIdInfo = new TextBox();
            lblInfoEstado = new Label();
            txtEstadoInfo = new TextBox();
            lblInfoTipo = new Label();
            txtTipoInfo = new TextBox();
            lblFacturaId = new Label();
            lblNumero = new Label();
            lblEstado = new Label();
            lblTipoActual = new Label();
            lblClienteActual = new Label();
            lblSubtotal = new Label();
            lblDescuentoTotal = new Label();
            lblItbis = new Label();
            lblTotal = new Label();
            pnlRoot.SuspendLayout();
            pnlMid.SuspendLayout();
            cardMid.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)grid).BeginInit();
            pnlBottom.SuspendLayout();
            cardBottom.SuspendLayout();
            flowBottom.SuspendLayout();
            pnlTop.SuspendLayout();
            cardTop.SuspendLayout();
            tblTop.SuspendLayout();
            tblLeft.SuspendLayout();
            tblRightWrap.SuspendLayout();
            rightBody.SuspendLayout();
            tblRight.SuspendLayout();
            tblTopRight.SuspendLayout();
            pnlHeader.SuspendLayout();
            SuspendLayout();
            // 
            // pnlRoot
            // 
            pnlRoot.Controls.Add(pnlMid);
            pnlRoot.Controls.Add(pnlBottom);
            pnlRoot.Controls.Add(pnlTop);
            pnlRoot.Controls.Add(pnlHeader);
            pnlRoot.Dock = DockStyle.Fill;
            pnlRoot.Location = new Point(0, 0);
            pnlRoot.Name = "pnlRoot";
            pnlRoot.Padding = new Padding(12);
            pnlRoot.Size = new Size(1563, 777);
            pnlRoot.TabIndex = 0;
            // 
            // pnlMid
            // 
            pnlMid.Controls.Add(cardMid);
            pnlMid.Dock = DockStyle.Fill;
            pnlMid.Location = new Point(12, 414);
            pnlMid.Name = "pnlMid";
            pnlMid.Padding = new Padding(0, 0, 0, 10);
            pnlMid.Size = new Size(1539, 277);
            pnlMid.TabIndex = 0;
            // 
            // cardMid
            // 
            cardMid.Controls.Add(grid);
            cardMid.Dock = DockStyle.Fill;
            cardMid.Location = new Point(0, 0);
            cardMid.Name = "cardMid";
            cardMid.Padding = new Padding(12);
            cardMid.Size = new Size(1539, 267);
            cardMid.TabIndex = 0;
            cardMid.Paint += CardMid_Paint;
            // 
            // grid
            // 
            grid.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = Color.FromArgb(252, 252, 252);
            grid.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.BackgroundColor = Color.White;
            grid.BorderStyle = BorderStyle.None;
            grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = Color.FromArgb(246, 248, 252);
            dataGridViewCellStyle2.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            dataGridViewCellStyle2.ForeColor = SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = DataGridViewTriState.True;
            grid.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            grid.ColumnHeadersHeight = 34;
            grid.Columns.AddRange(new DataGridViewColumn[] { colDetId, colImpuestoId, colProductoCodigo, colCodBarra, colDescripcion, colUnidad, colCantidad, colPrecio, colDescuentoPct, colDescuentoMonto, colItbisPct, colItbisMonto, colTotalLinea });
            dataGridViewCellStyle10.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle10.BackColor = SystemColors.Window;
            dataGridViewCellStyle10.Font = new Font("Segoe UI", 9F);
            dataGridViewCellStyle10.ForeColor = SystemColors.ControlText;
            dataGridViewCellStyle10.SelectionBackColor = Color.FromArgb(200, 225, 255);
            dataGridViewCellStyle10.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle10.WrapMode = DataGridViewTriState.False;
            grid.DefaultCellStyle = dataGridViewCellStyle10;
            grid.Dock = DockStyle.Fill;
            grid.EditMode = DataGridViewEditMode.EditOnEnter;
            grid.EnableHeadersVisualStyles = false;
            grid.GridColor = Color.FromArgb(238, 238, 238);
            grid.Location = new Point(12, 12);
            grid.MultiSelect = false;
            grid.Name = "grid";
            grid.RowHeadersWidth = 26;
            grid.SelectionMode = DataGridViewSelectionMode.CellSelect;
            grid.Size = new Size(1515, 243);
            grid.TabIndex = 0;
            // 
            // colDetId
            // 
            colDetId.HeaderText = "DetId";
            colDetId.Name = "colDetId";
            colDetId.Visible = false;
            // 
            // colImpuestoId
            // 
            colImpuestoId.HeaderText = "ImpId";
            colImpuestoId.Name = "colImpuestoId";
            colImpuestoId.Visible = false;
            // 
            // colProductoCodigo
            // 
            colProductoCodigo.FillWeight = 90F;
            colProductoCodigo.HeaderText = "Código";
            colProductoCodigo.Name = "colProductoCodigo";
            // 
            // colCodBarra
            // 
            colCodBarra.FillWeight = 120F;
            colCodBarra.HeaderText = "Cod. Barra";
            colCodBarra.Name = "colCodBarra";
            // 
            // colDescripcion
            // 
            colDescripcion.FillWeight = 240F;
            colDescripcion.HeaderText = "Descripción";
            colDescripcion.Name = "colDescripcion";
            // 
            // colUnidad
            // 
            colUnidad.FillWeight = 70F;
            colUnidad.HeaderText = "Unidad";
            colUnidad.Name = "colUnidad";
            // 
            // colCantidad
            // 
            dataGridViewCellStyle3.Format = "N2";
            colCantidad.DefaultCellStyle = dataGridViewCellStyle3;
            colCantidad.FillWeight = 80F;
            colCantidad.HeaderText = "Cantidad";
            colCantidad.Name = "colCantidad";
            // 
            // colPrecio
            // 
            dataGridViewCellStyle4.Format = "N2";
            colPrecio.DefaultCellStyle = dataGridViewCellStyle4;
            colPrecio.FillWeight = 90F;
            colPrecio.HeaderText = "Precio";
            colPrecio.Name = "colPrecio";
            // 
            // colDescuentoPct
            // 
            dataGridViewCellStyle5.Format = "N2";
            colDescuentoPct.DefaultCellStyle = dataGridViewCellStyle5;
            colDescuentoPct.FillWeight = 70F;
            colDescuentoPct.HeaderText = "% Dto.";
            colDescuentoPct.Name = "colDescuentoPct";
            // 
            // colDescuentoMonto
            // 
            dataGridViewCellStyle6.Format = "N2";
            colDescuentoMonto.DefaultCellStyle = dataGridViewCellStyle6;
            colDescuentoMonto.FillWeight = 90F;
            colDescuentoMonto.HeaderText = "Desc. Monto";
            colDescuentoMonto.Name = "colDescuentoMonto";
            colDescuentoMonto.ReadOnly = true;
            // 
            // colItbisPct
            // 
            dataGridViewCellStyle7.Format = "N2";
            colItbisPct.DefaultCellStyle = dataGridViewCellStyle7;
            colItbisPct.FillWeight = 70F;
            colItbisPct.HeaderText = "ITBIS %";
            colItbisPct.Name = "colItbisPct";
            // 
            // colItbisMonto
            // 
            dataGridViewCellStyle8.Format = "N2";
            colItbisMonto.DefaultCellStyle = dataGridViewCellStyle8;
            colItbisMonto.FillWeight = 95F;
            colItbisMonto.HeaderText = "ITBIS";
            colItbisMonto.Name = "colItbisMonto";
            colItbisMonto.ReadOnly = true;
            // 
            // colTotalLinea
            // 
            dataGridViewCellStyle9.Format = "N2";
            colTotalLinea.DefaultCellStyle = dataGridViewCellStyle9;
            colTotalLinea.FillWeight = 95F;
            colTotalLinea.HeaderText = "Importe";
            colTotalLinea.Name = "colTotalLinea";
            colTotalLinea.ReadOnly = true;
            // 
            // pnlBottom
            // 
            pnlBottom.Controls.Add(cardBottom);
            pnlBottom.Dock = DockStyle.Bottom;
            pnlBottom.Location = new Point(12, 691);
            pnlBottom.Name = "pnlBottom";
            pnlBottom.Size = new Size(1539, 74);
            pnlBottom.TabIndex = 1;
            // 
            // cardBottom
            // 
            cardBottom.Controls.Add(flowBottom);
            cardBottom.Dock = DockStyle.Fill;
            cardBottom.Location = new Point(0, 0);
            cardBottom.Name = "cardBottom";
            cardBottom.Padding = new Padding(12);
            cardBottom.Size = new Size(1539, 74);
            cardBottom.TabIndex = 0;
            cardBottom.Paint += CardBottom_Paint;
            // 
            // flowBottom
            // 
            flowBottom.Controls.Add(btnCerrar);
            flowBottom.Controls.Add(btnEliminarLinea);
            flowBottom.Controls.Add(btnAnular);
            flowBottom.Controls.Add(btnFinalizar);
            flowBottom.Controls.Add(btnGuardar);
            flowBottom.Controls.Add(btnAgregar);
            flowBottom.Controls.Add(btnNuevo);
            flowBottom.Controls.Add(btnRegistrarFactura);
            flowBottom.Controls.Add(btnImprimir);
            flowBottom.Dock = DockStyle.Fill;
            flowBottom.FlowDirection = FlowDirection.RightToLeft;
            flowBottom.Location = new Point(12, 12);
            flowBottom.Margin = new Padding(0);
            flowBottom.Name = "flowBottom";
            flowBottom.Size = new Size(1515, 50);
            flowBottom.TabIndex = 0;
            flowBottom.WrapContents = false;
            // 
            // btnCerrar
            // 
            btnCerrar.Location = new Point(1437, 3);
            btnCerrar.Name = "btnCerrar";
            btnCerrar.Size = new Size(75, 23);
            btnCerrar.TabIndex = 0;
            // 
            // btnEliminarLinea
            // 
            btnEliminarLinea.Location = new Point(1356, 3);
            btnEliminarLinea.Name = "btnEliminarLinea";
            btnEliminarLinea.Size = new Size(75, 23);
            btnEliminarLinea.TabIndex = 1;
            // 
            // btnAnular
            // 
            btnAnular.Location = new Point(1230, 3);
            btnAnular.Name = "btnAnular";
            btnAnular.Size = new Size(120, 23);
            btnAnular.TabIndex = 2;
            // 
            // btnFinalizar
            // 
            btnFinalizar.Location = new Point(1074, 3);
            btnFinalizar.Name = "btnFinalizar";
            btnFinalizar.Size = new Size(150, 23);
            btnFinalizar.TabIndex = 3;
            // 
            // btnGuardar
            // 
            btnGuardar.Location = new Point(993, 3);
            btnGuardar.Name = "btnGuardar";
            btnGuardar.Size = new Size(75, 23);
            btnGuardar.TabIndex = 4;
            // 
            // btnAgregar
            // 
            btnAgregar.Location = new Point(912, 3);
            btnAgregar.Name = "btnAgregar";
            btnAgregar.Size = new Size(75, 23);
            btnAgregar.TabIndex = 5;
            // 
            // btnNuevo
            // 
            btnNuevo.Location = new Point(831, 3);
            btnNuevo.Name = "btnNuevo";
            btnNuevo.Size = new Size(75, 23);
            btnNuevo.TabIndex = 6;
            // 
            // btnRegistrarFactura
            // 
            btnRegistrarFactura.Location = new Point(665, 3);
            btnRegistrarFactura.Name = "btnRegistrarFactura";
            btnRegistrarFactura.Size = new Size(160, 23);
            btnRegistrarFactura.TabIndex = 7;
            //btnRegistrarFactura.Click += btnRegistrarFactura_Click;
            // 
            // btnImprimir
            // 
            btnImprimir.Location = new Point(584, 3);
            btnImprimir.Name = "btnImprimir";
            btnImprimir.Size = new Size(75, 23);
            btnImprimir.TabIndex = 8;
            // 
            // pnlTop
            // 
            pnlTop.Controls.Add(cardTop);
            pnlTop.Dock = DockStyle.Top;
            pnlTop.Location = new Point(12, 70);
            pnlTop.Name = "pnlTop";
            pnlTop.Padding = new Padding(0, 8, 0, 10);
            pnlTop.Size = new Size(1539, 344);
            pnlTop.TabIndex = 2;
            // 
            // cardTop
            // 
            cardTop.Controls.Add(tblTop);
            cardTop.Dock = DockStyle.Fill;
            cardTop.Location = new Point(0, 8);
            cardTop.Name = "cardTop";
            cardTop.Padding = new Padding(12);
            cardTop.Size = new Size(1539, 326);
            cardTop.TabIndex = 0;
            cardTop.Paint += CardTop_Paint;
            // 
            // tblTop
            // 
            tblTop.ColumnCount = 2;
            tblTop.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 52F));
            tblTop.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 48F));
            tblTop.Controls.Add(tblLeft, 0, 0);
            tblTop.Controls.Add(tblRightWrap, 1, 0);
            tblTop.Dock = DockStyle.Fill;
            tblTop.Location = new Point(12, 12);
            tblTop.Name = "tblTop";
            tblTop.RowCount = 1;
            tblTop.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tblTop.Size = new Size(1515, 302);
            tblTop.TabIndex = 0;
            // 
            // tblLeft
            // 
            tblLeft.ColumnCount = 3;
            tblLeft.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 180F));
            tblLeft.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tblLeft.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120F));
            tblLeft.Controls.Add(lblClienteN, 0, 0);
            tblLeft.Controls.Add(txtClienteBuscar, 1, 0);
            tblLeft.Controls.Add(btnBuscarCliente, 2, 0);
            tblLeft.Controls.Add(lblCliNombre, 0, 1);
            tblLeft.Controls.Add(txtClienteNombre, 1, 1);
            tblLeft.Controls.Add(lblCliDireccion, 0, 2);
            tblLeft.Controls.Add(txtClienteDireccion, 1, 2);
            tblLeft.Controls.Add(lblCliRnc, 0, 3);
            tblLeft.Controls.Add(txtClienteRnc, 1, 3);
            tblLeft.Controls.Add(lblTipoComprobante, 0, 4);
            tblLeft.Controls.Add(cboTipoComprobante, 1, 4);
            tblLeft.Dock = DockStyle.Fill;
            tblLeft.Location = new Point(3, 3);
            tblLeft.Name = "tblLeft";
            tblLeft.RowCount = 5;
            tblLeft.RowStyles.Add(new RowStyle(SizeType.Absolute, 34F));
            tblLeft.RowStyles.Add(new RowStyle(SizeType.Absolute, 34F));
            tblLeft.RowStyles.Add(new RowStyle(SizeType.Absolute, 34F));
            tblLeft.RowStyles.Add(new RowStyle(SizeType.Absolute, 34F));
            tblLeft.RowStyles.Add(new RowStyle(SizeType.Absolute, 34F));
            tblLeft.Size = new Size(781, 296);
            tblLeft.TabIndex = 0;
            // 
            // lblClienteN
            // 
            lblClienteN.Font = new Font("Segoe UI Emoji", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblClienteN.Location = new Point(3, 0);
            lblClienteN.Name = "lblClienteN";
            lblClienteN.Size = new Size(100, 23);
            lblClienteN.TabIndex = 0;
            lblClienteN.Text = "RNC";
            // 
            // txtClienteBuscar
            // 
            txtClienteBuscar.Location = new Point(183, 3);
            txtClienteBuscar.Name = "txtClienteBuscar";
            txtClienteBuscar.Size = new Size(256, 23);
            txtClienteBuscar.TabIndex = 1;
            // 
            // btnBuscarCliente
            // 
            btnBuscarCliente.Location = new Point(664, 3);
            btnBuscarCliente.Name = "btnBuscarCliente";
            btnBuscarCliente.Size = new Size(75, 23);
            btnBuscarCliente.TabIndex = 2;
            // 
            // lblCliNombre
            // 
            lblCliNombre.Location = new Point(3, 34);
            lblCliNombre.Name = "lblCliNombre";
            lblCliNombre.Size = new Size(100, 23);
            lblCliNombre.TabIndex = 3;
            lblCliNombre.Text = "Nombre";
            // 
            // txtClienteNombre
            // 
            tblLeft.SetColumnSpan(txtClienteNombre, 2);
            txtClienteNombre.Location = new Point(183, 37);
            txtClienteNombre.Name = "txtClienteNombre";
            txtClienteNombre.Size = new Size(256, 23);
            txtClienteNombre.TabIndex = 4;
            // 
            // lblCliDireccion
            // 
            lblCliDireccion.Location = new Point(3, 68);
            lblCliDireccion.Name = "lblCliDireccion";
            lblCliDireccion.Size = new Size(100, 23);
            lblCliDireccion.TabIndex = 5;
            // 
            // txtClienteDireccion
            // 
            tblLeft.SetColumnSpan(txtClienteDireccion, 2);
            txtClienteDireccion.Location = new Point(183, 71);
            txtClienteDireccion.Name = "txtClienteDireccion";
            txtClienteDireccion.Size = new Size(256, 23);
            txtClienteDireccion.TabIndex = 6;
            // 
            // lblCliRnc
            // 
            lblCliRnc.Location = new Point(3, 102);
            lblCliRnc.Name = "lblCliRnc";
            lblCliRnc.Size = new Size(100, 23);
            lblCliRnc.TabIndex = 7;
            // 
            // txtClienteRnc
            // 
            tblLeft.SetColumnSpan(txtClienteRnc, 2);
            txtClienteRnc.Location = new Point(183, 105);
            txtClienteRnc.Name = "txtClienteRnc";
            txtClienteRnc.Size = new Size(256, 23);
            txtClienteRnc.TabIndex = 8;
            // 
            // lblTipoComprobante
            // 
            lblTipoComprobante.Location = new Point(3, 136);
            lblTipoComprobante.Name = "lblTipoComprobante";
            lblTipoComprobante.Size = new Size(100, 23);
            lblTipoComprobante.TabIndex = 9;
            // 
            // cboTipoComprobante
            // 
            tblLeft.SetColumnSpan(cboTipoComprobante, 2);
            cboTipoComprobante.Dock = DockStyle.Fill;
            cboTipoComprobante.DropDownStyle = ComboBoxStyle.DropDownList;
            cboTipoComprobante.Location = new Point(183, 142);
            cboTipoComprobante.Margin = new Padding(3, 6, 0, 6);
            cboTipoComprobante.Name = "cboTipoComprobante";
            cboTipoComprobante.Size = new Size(598, 23);
            cboTipoComprobante.TabIndex = 10;
            // 
            // tblRightWrap
            // 
            tblRightWrap.ColumnCount = 1;
            tblRightWrap.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20F));
            tblRightWrap.Controls.Add(rightBody, 0, 1);
            tblRightWrap.Controls.Add(tblTopRight, 0, 0);
            tblRightWrap.Dock = DockStyle.Fill;
            tblRightWrap.Location = new Point(790, 3);
            tblRightWrap.Name = "tblRightWrap";
            tblRightWrap.RowCount = 2;
            tblRightWrap.RowStyles.Add(new RowStyle(SizeType.Absolute, 71F));
            tblRightWrap.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tblRightWrap.Size = new Size(722, 296);
            tblRightWrap.TabIndex = 1;
            // 
            // rightBody
            // 
            rightBody.Controls.Add(tblRight);
            rightBody.Controls.Add(div);
            rightBody.Dock = DockStyle.Fill;
            rightBody.Location = new Point(3, 74);
            rightBody.Name = "rightBody";
            rightBody.Padding = new Padding(0, 8, 0, 0);
            rightBody.Size = new Size(716, 219);
            rightBody.TabIndex = 1;
            // 
            // tblRight
            // 
            tblRight.ColumnCount = 2;
            tblRight.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 180F));
            tblRight.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tblRight.Controls.Add(lblFechaRegistro, 0, 0);
            tblRight.Controls.Add(dtpFechaRegistro, 1, 0);
            tblRight.Controls.Add(lblTipoDoc, 0, 1);
            tblRight.Controls.Add(cboTipoDoc, 1, 1);
            tblRight.Controls.Add(lblVendedor, 0, 2);
            tblRight.Controls.Add(cboVendedor, 1, 2);
            tblRight.Controls.Add(lblCredito, 0, 3);
            tblRight.Controls.Add(chkCredito, 1, 3);
            tblRight.Controls.Add(lblTerminoPago, 0, 4);
            tblRight.Controls.Add(cboTerminoPago, 1, 4);
            tblRight.Controls.Add(lblDiasCredito, 0, 5);
            tblRight.Controls.Add(txtDiasCredito, 1, 5);
            tblRight.Dock = DockStyle.Fill;
            tblRight.Location = new Point(0, 9);
            tblRight.Name = "tblRight";
            tblRight.RowCount = 6;
            tblRight.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F));
            tblRight.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F));
            tblRight.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F));
            tblRight.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F));
            tblRight.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F));
            tblRight.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F));
            tblRight.Size = new Size(716, 210);
            tblRight.TabIndex = 0;
            // 
            // lblFechaRegistro
            // 
            lblFechaRegistro.Location = new Point(3, 0);
            lblFechaRegistro.Name = "lblFechaRegistro";
            lblFechaRegistro.Size = new Size(100, 23);
            lblFechaRegistro.TabIndex = 0;
            lblFechaRegistro.Text = "Fecha Registro";
            // 
            // dtpFechaRegistro
            // 
            dtpFechaRegistro.Dock = DockStyle.Fill;
            dtpFechaRegistro.Enabled = false;
            dtpFechaRegistro.Format = DateTimePickerFormat.Short;
            dtpFechaRegistro.Location = new Point(183, 3);
            dtpFechaRegistro.Name = "dtpFechaRegistro";
            dtpFechaRegistro.Size = new Size(530, 23);
            dtpFechaRegistro.TabIndex = 1;
            // 
            // lblTipoDoc
            // 
            lblTipoDoc.Location = new Point(3, 32);
            lblTipoDoc.Name = "lblTipoDoc";
            lblTipoDoc.Size = new Size(100, 23);
            lblTipoDoc.TabIndex = 2;
            lblTipoDoc.Text = "Tipo Doc";
            // 
            // cboTipoDoc
            // 
            cboTipoDoc.Dock = DockStyle.Fill;
            cboTipoDoc.DropDownStyle = ComboBoxStyle.DropDownList;
            cboTipoDoc.Location = new Point(183, 35);
            cboTipoDoc.Name = "cboTipoDoc";
            cboTipoDoc.Size = new Size(530, 23);
            cboTipoDoc.TabIndex = 3;
            // 
            // lblVendedor
            // 
            lblVendedor.Location = new Point(3, 64);
            lblVendedor.Name = "lblVendedor";
            lblVendedor.Size = new Size(100, 23);
            lblVendedor.TabIndex = 4;
            lblVendedor.Text = "Vendedor";
            // 
            // cboVendedor
            // 
            cboVendedor.Dock = DockStyle.Fill;
            cboVendedor.DropDownStyle = ComboBoxStyle.DropDownList;
            cboVendedor.Location = new Point(183, 67);
            cboVendedor.Name = "cboVendedor";
            cboVendedor.Size = new Size(530, 23);
            cboVendedor.TabIndex = 5;
            // 
            // lblCredito
            // 
            lblCredito.Location = new Point(3, 96);
            lblCredito.Name = "lblCredito";
            lblCredito.Size = new Size(100, 23);
            lblCredito.TabIndex = 4;
            lblCredito.Text = "Credito";
            // 
            // chkCredito
            // 
            chkCredito.Dock = DockStyle.Left;
            chkCredito.Location = new Point(183, 99);
            chkCredito.Name = "chkCredito";
            chkCredito.Size = new Size(104, 26);
            chkCredito.TabIndex = 5;
            // 
            // lblTerminoPago
            // 
            lblTerminoPago.Location = new Point(3, 128);
            lblTerminoPago.Name = "lblTerminoPago";
            lblTerminoPago.Size = new Size(100, 23);
            lblTerminoPago.TabIndex = 6;
            lblTerminoPago.Text = "Termino Pago";
            // 
            // cboTerminoPago
            // 
            cboTerminoPago.Dock = DockStyle.Fill;
            cboTerminoPago.DropDownStyle = ComboBoxStyle.DropDownList;
            cboTerminoPago.Location = new Point(183, 131);
            cboTerminoPago.Name = "cboTerminoPago";
            cboTerminoPago.Size = new Size(530, 23);
            cboTerminoPago.TabIndex = 7;
            // 
            // lblDiasCredito
            // 
            lblDiasCredito.Location = new Point(3, 160);
            lblDiasCredito.Name = "lblDiasCredito";
            lblDiasCredito.Size = new Size(100, 23);
            lblDiasCredito.TabIndex = 8;
            lblDiasCredito.Text = "Dia Credito";
            // 
            // txtDiasCredito
            // 
            txtDiasCredito.Location = new Point(183, 163);
            txtDiasCredito.Name = "txtDiasCredito";
            txtDiasCredito.Size = new Size(100, 23);
            txtDiasCredito.TabIndex = 9;
            // 
            // div
            // 
            div.Dock = DockStyle.Top;
            div.Location = new Point(0, 8);
            div.Margin = new Padding(0, 6, 0, 6);
            div.Name = "div";
            div.Size = new Size(716, 1);
            div.TabIndex = 1;
            // 
            // tblTopRight
            // 
            tblTopRight.ColumnCount = 10;
            tblTopRight.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 82F));
            tblTopRight.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 108F));
            tblTopRight.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 96F));
            tblTopRight.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 107F));
            tblTopRight.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 84F));
            tblTopRight.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80F));
            tblTopRight.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 73F));
            tblTopRight.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80F));
            tblTopRight.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 52F));
            tblTopRight.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 98F));
            tblTopRight.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 54F));
            tblTopRight.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tblTopRight.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20F));
            tblTopRight.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20F));
            tblTopRight.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20F));
            tblTopRight.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20F));
            tblTopRight.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20F));
            tblTopRight.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20F));
            tblTopRight.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20F));
            tblTopRight.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20F));
            tblTopRight.Controls.Add(lblNumeroFacturaTop, 0, 0);
            tblTopRight.Controls.Add(txtNumeroFacturaTop, 1, 0);
            tblTopRight.Controls.Add(dtpFechaDoc, 3, 0);
            tblTopRight.Controls.Add(lblTotSubtotal, 4, 0);
            tblTopRight.Controls.Add(txtSubtotal, 5, 0);
            tblTopRight.Controls.Add(lblFechaEmision, 2, 0);
            tblTopRight.Controls.Add(txtDescuentoTotal, 5, 1);
            tblTopRight.Controls.Add(lblTotDescuento, 4, 1);
            tblTopRight.Controls.Add(lblTotItbis, 6, 0);
            tblTopRight.Controls.Add(txtItbisTotal, 7, 0);
            tblTopRight.Controls.Add(lblTotTotal, 6, 1);
            tblTopRight.Controls.Add(txtTotalGeneral, 7, 1);
            tblTopRight.Dock = DockStyle.Fill;
            tblTopRight.Location = new Point(3, 3);
            tblTopRight.Name = "tblTopRight";
            tblTopRight.RowCount = 2;
            tblTopRight.RowStyles.Add(new RowStyle(SizeType.Absolute, 33F));
            tblTopRight.RowStyles.Add(new RowStyle(SizeType.Absolute, 41F));
            tblTopRight.RowStyles.Add(new RowStyle(SizeType.Absolute, 14F));
            tblTopRight.Size = new Size(716, 65);
            tblTopRight.TabIndex = 0;
            // 
            // lblNumeroFacturaTop
            // 
            lblNumeroFacturaTop.Location = new Point(3, 0);
            lblNumeroFacturaTop.Name = "lblNumeroFacturaTop";
            lblNumeroFacturaTop.Size = new Size(76, 33);
            lblNumeroFacturaTop.TabIndex = 0;
            lblNumeroFacturaTop.Text = "Numero Factura";
            // 
            // txtNumeroFacturaTop
            // 
            txtNumeroFacturaTop.Location = new Point(85, 3);
            txtNumeroFacturaTop.Name = "txtNumeroFacturaTop";
            tblTopRight.SetRowSpan(txtNumeroFacturaTop, 2);
            txtNumeroFacturaTop.Size = new Size(100, 23);
            txtNumeroFacturaTop.TabIndex = 1;
            // 
            // dtpFechaDoc
            // 
            dtpFechaDoc.Dock = DockStyle.Fill;
            dtpFechaDoc.Format = DateTimePickerFormat.Short;
            dtpFechaDoc.Location = new Point(289, 3);
            dtpFechaDoc.Name = "dtpFechaDoc";
            tblTopRight.SetRowSpan(dtpFechaDoc, 2);
            dtpFechaDoc.Size = new Size(101, 23);
            dtpFechaDoc.TabIndex = 3;
            // 
            // lblTotSubtotal
            // 
            lblTotSubtotal.Location = new Point(396, 0);
            lblTotSubtotal.Name = "lblTotSubtotal";
            lblTotSubtotal.Size = new Size(60, 33);
            lblTotSubtotal.TabIndex = 4;
            lblTotSubtotal.Text = "Subtotal";
            // 
            // txtSubtotal
            // 
            txtSubtotal.Location = new Point(480, 3);
            txtSubtotal.Name = "txtSubtotal";
            txtSubtotal.Size = new Size(72, 23);
            txtSubtotal.TabIndex = 5;
            // 
            // lblFechaEmision
            // 
            lblFechaEmision.Location = new Point(193, 0);
            lblFechaEmision.Name = "lblFechaEmision";
            lblFechaEmision.Size = new Size(90, 33);
            lblFechaEmision.TabIndex = 2;
            lblFechaEmision.Text = "Fecha Emision";
            // 
            // txtDescuentoTotal
            // 
            txtDescuentoTotal.Location = new Point(480, 36);
            txtDescuentoTotal.Name = "txtDescuentoTotal";
            txtDescuentoTotal.Size = new Size(72, 23);
            txtDescuentoTotal.TabIndex = 7;
            // 
            // lblTotDescuento
            // 
            lblTotDescuento.Location = new Point(396, 33);
            lblTotDescuento.Name = "lblTotDescuento";
            lblTotDescuento.Size = new Size(78, 23);
            lblTotDescuento.TabIndex = 6;
            lblTotDescuento.Text = "Descuento";
            // 
            // lblTotItbis
            // 
            lblTotItbis.Location = new Point(560, 0);
            lblTotItbis.Name = "lblTotItbis";
            lblTotItbis.Size = new Size(67, 23);
            lblTotItbis.TabIndex = 8;
            lblTotItbis.Text = "Impuesto";
            // 
            // txtItbisTotal
            // 
            txtItbisTotal.Location = new Point(633, 3);
            txtItbisTotal.Name = "txtItbisTotal";
            txtItbisTotal.Size = new Size(74, 23);
            txtItbisTotal.TabIndex = 9;
            // 
            // lblTotTotal
            // 
            lblTotTotal.Location = new Point(560, 33);
            lblTotTotal.Name = "lblTotTotal";
            lblTotTotal.Size = new Size(46, 23);
            lblTotTotal.TabIndex = 10;
            lblTotTotal.Text = "Total";
            // 
            // txtTotalGeneral
            // 
            txtTotalGeneral.Location = new Point(633, 36);
            txtTotalGeneral.Name = "txtTotalGeneral";
            tblTopRight.SetRowSpan(txtTotalGeneral, 2);
            txtTotalGeneral.Size = new Size(74, 23);
            txtTotalGeneral.TabIndex = 11;
            // 
            // pnlHeader
            // 
            pnlHeader.Controls.Add(lblTitle);
            pnlHeader.Controls.Add(lblSubTitle);
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Location = new Point(12, 12);
            pnlHeader.Name = "pnlHeader";
            pnlHeader.Size = new Size(1539, 58);
            pnlHeader.TabIndex = 3;
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe UI Semibold", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblTitle.Location = new Point(2, 2);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(123, 30);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "Facturación";
            // 
            // lblSubTitle
            // 
            lblSubTitle.AutoSize = true;
            lblSubTitle.Font = new Font("Segoe UI Semibold", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblSubTitle.Location = new Point(4, 32);
            lblSubTitle.Name = "lblSubTitle";
            lblSubTitle.Size = new Size(246, 20);
            lblSubTitle.TabIndex = 1;
            lblSubTitle.Text = "Cotización · Proforma · Factura (RI)";
            // 
            // lblInfoFacturaId
            // 
            lblInfoFacturaId.Location = new Point(0, 0);
            lblInfoFacturaId.Name = "lblInfoFacturaId";
            lblInfoFacturaId.Size = new Size(100, 23);
            lblInfoFacturaId.TabIndex = 0;
            lblInfoFacturaId.Visible = false;
            // 
            // txtFacturaIdInfo
            // 
            txtFacturaIdInfo.Location = new Point(0, 0);
            txtFacturaIdInfo.Name = "txtFacturaIdInfo";
            txtFacturaIdInfo.Size = new Size(100, 23);
            txtFacturaIdInfo.TabIndex = 0;
            txtFacturaIdInfo.Visible = false;
            // 
            // lblInfoEstado
            // 
            lblInfoEstado.Location = new Point(0, 0);
            lblInfoEstado.Name = "lblInfoEstado";
            lblInfoEstado.Size = new Size(100, 23);
            lblInfoEstado.TabIndex = 0;
            lblInfoEstado.Visible = false;
            // 
            // txtEstadoInfo
            // 
            txtEstadoInfo.Location = new Point(0, 0);
            txtEstadoInfo.Name = "txtEstadoInfo";
            txtEstadoInfo.Size = new Size(100, 23);
            txtEstadoInfo.TabIndex = 0;
            txtEstadoInfo.Visible = false;
            // 
            // lblInfoTipo
            // 
            lblInfoTipo.Location = new Point(0, 0);
            lblInfoTipo.Name = "lblInfoTipo";
            lblInfoTipo.Size = new Size(100, 23);
            lblInfoTipo.TabIndex = 0;
            lblInfoTipo.Visible = false;
            // 
            // txtTipoInfo
            // 
            txtTipoInfo.Location = new Point(0, 0);
            txtTipoInfo.Name = "txtTipoInfo";
            txtTipoInfo.Size = new Size(100, 23);
            txtTipoInfo.TabIndex = 0;
            txtTipoInfo.Visible = false;
            // 
            // lblFacturaId
            // 
            lblFacturaId.Location = new Point(0, 0);
            lblFacturaId.Name = "lblFacturaId";
            lblFacturaId.Size = new Size(100, 23);
            lblFacturaId.TabIndex = 0;
            // 
            // lblNumero
            // 
            lblNumero.Location = new Point(0, 0);
            lblNumero.Name = "lblNumero";
            lblNumero.Size = new Size(100, 23);
            lblNumero.TabIndex = 0;
            // 
            // lblEstado
            // 
            lblEstado.Location = new Point(0, 0);
            lblEstado.Name = "lblEstado";
            lblEstado.Size = new Size(100, 23);
            lblEstado.TabIndex = 0;
            // 
            // lblTipoActual
            // 
            lblTipoActual.Location = new Point(0, 0);
            lblTipoActual.Name = "lblTipoActual";
            lblTipoActual.Size = new Size(100, 23);
            lblTipoActual.TabIndex = 0;
            // 
            // lblClienteActual
            // 
            lblClienteActual.Location = new Point(0, 0);
            lblClienteActual.Name = "lblClienteActual";
            lblClienteActual.Size = new Size(100, 23);
            lblClienteActual.TabIndex = 0;
            // 
            // lblSubtotal
            // 
            lblSubtotal.Location = new Point(0, 0);
            lblSubtotal.Name = "lblSubtotal";
            lblSubtotal.Size = new Size(100, 23);
            lblSubtotal.TabIndex = 0;
            // 
            // lblDescuentoTotal
            // 
            lblDescuentoTotal.Location = new Point(0, 0);
            lblDescuentoTotal.Name = "lblDescuentoTotal";
            lblDescuentoTotal.Size = new Size(100, 23);
            lblDescuentoTotal.TabIndex = 0;
            // 
            // lblItbis
            // 
            lblItbis.Location = new Point(0, 0);
            lblItbis.Name = "lblItbis";
            lblItbis.Size = new Size(100, 23);
            lblItbis.TabIndex = 0;
            // 
            // lblTotal
            // 
            lblTotal.Location = new Point(0, 0);
            lblTotal.Name = "lblTotal";
            lblTotal.Size = new Size(100, 23);
            lblTotal.TabIndex = 0;
            // 
            // FormFacturaV
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1563, 777);
            Controls.Add(pnlRoot);
            Font = new Font("Segoe UI", 9F);
            KeyPreview = true;
            MinimumSize = new Size(980, 620);
            Name = "FormFacturaV";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Facturación - Andloe";
            pnlRoot.ResumeLayout(false);
            pnlMid.ResumeLayout(false);
            cardMid.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)grid).EndInit();
            pnlBottom.ResumeLayout(false);
            cardBottom.ResumeLayout(false);
            flowBottom.ResumeLayout(false);
            pnlTop.ResumeLayout(false);
            cardTop.ResumeLayout(false);
            tblTop.ResumeLayout(false);
            tblLeft.ResumeLayout(false);
            tblLeft.PerformLayout();
            tblRightWrap.ResumeLayout(false);
            rightBody.ResumeLayout(false);
            tblRight.ResumeLayout(false);
            tblRight.PerformLayout();
            tblTopRight.ResumeLayout(false);
            tblTopRight.PerformLayout();
            pnlHeader.ResumeLayout(false);
            pnlHeader.PerformLayout();
            ResumeLayout(false);
        }

        // ============================
        // Paint handlers (Designer-safe)
        // ============================
        private void CardTop_Paint(object sender, PaintEventArgs e) => DrawCardBorder(sender, e);
        private void CardMid_Paint(object sender, PaintEventArgs e) => DrawCardBorder(sender, e);
        private void CardBottom_Paint(object sender, PaintEventArgs e) => DrawCardBorder(sender, e);

        private void DrawCardBorder(object sender, PaintEventArgs e)
        {
            Control c = sender as Control;
            if (c == null) return;

            Rectangle r = c.ClientRectangle;
            r.Width -= 1;
            r.Height -= 1;

            using (Pen pen = new Pen(cLine))
            {
                e.Graphics.DrawRectangle(pen, r);
            }
        }

        // ============================
        // Helper methods (NO locales)
        // ============================
        private void ConfigFieldLabel(Label lbl, string text, Color color)
        {
            lbl.AutoSize = true;
            lbl.Text = text;
            lbl.ForeColor = color;
            lbl.Font = new Font("Segoe UI", 8.5F, FontStyle.Regular);
            lbl.Padding = new Padding(0, 8, 0, 0);
        }

        private void ConfigInput(TextBox txt)
        {
            txt.Dock = DockStyle.Fill;
            txt.BorderStyle = BorderStyle.FixedSingle;
            txt.BackColor = Color.White;
            txt.Margin = new Padding(3, 6, 0, 6);
        }

        private void ConfigInfo(TextBox txt, bool readOnly)
        {
            txt.Dock = DockStyle.Fill;
            txt.BorderStyle = BorderStyle.FixedSingle;
            txt.ReadOnly = readOnly;
            txt.TabStop = false;
            txt.BackColor = Color.White;
            txt.Margin = new Padding(3, 6, 0, 6);
        }

        private void ConfigMiniIcon(Button b, string text, Color accent)
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

        private void ConfigTotLabel(Label lbl, string text, Color color, bool bold)
        {
            lbl.AutoSize = true;
            lbl.Text = text;
            lbl.ForeColor = color;
            lbl.Font = new Font("Segoe UI", 8.5F, bold ? FontStyle.Bold : FontStyle.Regular);
            lbl.Padding = new Padding(0, 8, 6, 0);
        }

        private void ConfigTotBox(TextBox txt, bool bold, bool accent = false)
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

        private void BaseBtn(Button b)
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

        private void ConfigBtnDefault(Button b, string text)
        {
            BaseBtn(b);
            b.Text = text;
            b.BackColor = Color.White;
            b.ForeColor = cText;
        }

        private void ConfigBtnDanger(Button b, string text)
        {
            BaseBtn(b);
            b.Text = text;
            b.BackColor = Color.White;
            b.ForeColor = Color.FromArgb(190, 45, 45);
        }

        private void ConfigBtnPrimary(Button b, string text, Color accent)
        {
            BaseBtn(b);
            b.Text = text;
            b.BackColor = accent;
            b.ForeColor = Color.White;
            b.FlatAppearance.BorderColor = accent;
        }
        private Panel rightBody;
        private Panel div;
    }
}