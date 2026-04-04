namespace Andloe.Presentacion
{
    partial class FormPostulacionDGII
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.Panel panelHeader;
        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.Label lblSubtitulo;
        private System.Windows.Forms.Label lblEstadoOperacion;

        private System.Windows.Forms.Panel panelFiltros;
        private System.Windows.Forms.Label lblFiltroEstado;
        private System.Windows.Forms.ComboBox cboEstadoFiltro;
        private System.Windows.Forms.Label lblPostulacionId;
        private System.Windows.Forms.NumericUpDown nudPostulacionId;
        private System.Windows.Forms.Label lblUsuario;
        private System.Windows.Forms.TextBox txtUsuario;
        private System.Windows.Forms.Button btnRefrescar;
        private System.Windows.Forms.Button btnVerDetalle;

        private System.Windows.Forms.SplitContainer splitMain;

        private System.Windows.Forms.Panel panelLeft;
        private System.Windows.Forms.Label lblResumen;
        private System.Windows.Forms.DataGridView grid;

        private System.Windows.Forms.Panel panelRight;
        private System.Windows.Forms.TableLayoutPanel layoutRight;

        private System.Windows.Forms.GroupBox grpAcciones;
        private System.Windows.Forms.Label lblPfxPath;
        private System.Windows.Forms.TextBox txtPfxPath;
        private System.Windows.Forms.Button btnBuscarPfx;
        private System.Windows.Forms.Button btnAbrirCarpetaPfx;
        private System.Windows.Forms.Label lblPfxPassword;
        private System.Windows.Forms.TextBox txtPfxPassword;
        private System.Windows.Forms.Button btnValidar;
        private System.Windows.Forms.Button btnGenerarXml;
        private System.Windows.Forms.Button btnFirmar;

        private System.Windows.Forms.GroupBox grpCabecera;
        private System.Windows.Forms.TableLayoutPanel layoutCabecera;
        private System.Windows.Forms.TextBox txtPostulacionGuid;
        private System.Windows.Forms.TextBox txtTipoRegistro;
        private System.Windows.Forms.TextBox txtGrupoComprobante;
        private System.Windows.Forms.TextBox txtEstado;
        private System.Windows.Forms.TextBox txtFechaSolicitud;
        private System.Windows.Forms.TextBox txtFechaGenerado;
        private System.Windows.Forms.TextBox txtFechaFirma;
        private System.Windows.Forms.TextBox txtFechaEnviado;

        private System.Windows.Forms.GroupBox grpFirma;
        private System.Windows.Forms.TableLayoutPanel layoutFirma;
        private System.Windows.Forms.TextBox txtDigestValue;
        private System.Windows.Forms.TextBox txtSignatureValue;
        private System.Windows.Forms.TextBox txtCanonicalization;
        private System.Windows.Forms.TextBox txtSignatureMethod;
        private System.Windows.Forms.TextBox txtThumbprint;
        private System.Windows.Forms.TextBox txtSerial;
        private System.Windows.Forms.TextBox txtCertIssuer;
        private System.Windows.Forms.TextBox txtCertSubject;
        private System.Windows.Forms.TextBox txtHash;

        private System.Windows.Forms.GroupBox grpValidaciones;
        private System.Windows.Forms.Label lblValidacionResumen;
        private System.Windows.Forms.DataGridView dgvValidaciones;

        private System.Windows.Forms.GroupBox grpHistorial;
        private System.Windows.Forms.DataGridView dgvHistorial;

        private System.Windows.Forms.Panel panelXmlActions;
        private System.Windows.Forms.Button btnGuardarXmlSinFirmar;
        private System.Windows.Forms.Button btnCopiarXmlSinFirmar;
        private System.Windows.Forms.Button btnGuardarXmlFirmado;
        private System.Windows.Forms.Button btnCopiarXmlFirmado;

        private System.Windows.Forms.TabControl tabXml;
        private System.Windows.Forms.TabPage tabXmlSinFirmar;
        private System.Windows.Forms.TabPage tabXmlFirmado;
        private System.Windows.Forms.RichTextBox txtXmlSinFirmar;
        private System.Windows.Forms.RichTextBox txtXmlFirmado;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.panelHeader = new System.Windows.Forms.Panel();
            this.lblTitulo = new System.Windows.Forms.Label();
            this.lblSubtitulo = new System.Windows.Forms.Label();
            this.lblEstadoOperacion = new System.Windows.Forms.Label();

            this.panelFiltros = new System.Windows.Forms.Panel();
            this.lblFiltroEstado = new System.Windows.Forms.Label();
            this.cboEstadoFiltro = new System.Windows.Forms.ComboBox();
            this.lblPostulacionId = new System.Windows.Forms.Label();
            this.nudPostulacionId = new System.Windows.Forms.NumericUpDown();
            this.lblUsuario = new System.Windows.Forms.Label();
            this.txtUsuario = new System.Windows.Forms.TextBox();
            this.btnRefrescar = new System.Windows.Forms.Button();
            this.btnVerDetalle = new System.Windows.Forms.Button();

            this.splitMain = new System.Windows.Forms.SplitContainer();

            this.panelLeft = new System.Windows.Forms.Panel();
            this.lblResumen = new System.Windows.Forms.Label();
            this.grid = new System.Windows.Forms.DataGridView();

            this.panelRight = new System.Windows.Forms.Panel();
            this.layoutRight = new System.Windows.Forms.TableLayoutPanel();

            this.grpAcciones = new System.Windows.Forms.GroupBox();
            this.lblPfxPath = new System.Windows.Forms.Label();
            this.txtPfxPath = new System.Windows.Forms.TextBox();
            this.btnBuscarPfx = new System.Windows.Forms.Button();
            this.btnAbrirCarpetaPfx = new System.Windows.Forms.Button();
            this.lblPfxPassword = new System.Windows.Forms.Label();
            this.txtPfxPassword = new System.Windows.Forms.TextBox();
            this.btnValidar = new System.Windows.Forms.Button();
            this.btnGenerarXml = new System.Windows.Forms.Button();
            this.btnFirmar = new System.Windows.Forms.Button();

            this.grpCabecera = new System.Windows.Forms.GroupBox();
            this.layoutCabecera = new System.Windows.Forms.TableLayoutPanel();
            this.txtPostulacionGuid = new System.Windows.Forms.TextBox();
            this.txtTipoRegistro = new System.Windows.Forms.TextBox();
            this.txtGrupoComprobante = new System.Windows.Forms.TextBox();
            this.txtEstado = new System.Windows.Forms.TextBox();
            this.txtFechaSolicitud = new System.Windows.Forms.TextBox();
            this.txtFechaGenerado = new System.Windows.Forms.TextBox();
            this.txtFechaFirma = new System.Windows.Forms.TextBox();
            this.txtFechaEnviado = new System.Windows.Forms.TextBox();

            this.grpFirma = new System.Windows.Forms.GroupBox();
            this.layoutFirma = new System.Windows.Forms.TableLayoutPanel();
            this.txtDigestValue = new System.Windows.Forms.TextBox();
            this.txtSignatureValue = new System.Windows.Forms.TextBox();
            this.txtCanonicalization = new System.Windows.Forms.TextBox();
            this.txtSignatureMethod = new System.Windows.Forms.TextBox();
            this.txtThumbprint = new System.Windows.Forms.TextBox();
            this.txtSerial = new System.Windows.Forms.TextBox();
            this.txtCertIssuer = new System.Windows.Forms.TextBox();
            this.txtCertSubject = new System.Windows.Forms.TextBox();
            this.txtHash = new System.Windows.Forms.TextBox();

            this.grpValidaciones = new System.Windows.Forms.GroupBox();
            this.lblValidacionResumen = new System.Windows.Forms.Label();
            this.dgvValidaciones = new System.Windows.Forms.DataGridView();

            this.grpHistorial = new System.Windows.Forms.GroupBox();
            this.dgvHistorial = new System.Windows.Forms.DataGridView();

            this.panelXmlActions = new System.Windows.Forms.Panel();
            this.btnGuardarXmlSinFirmar = new System.Windows.Forms.Button();
            this.btnCopiarXmlSinFirmar = new System.Windows.Forms.Button();
            this.btnGuardarXmlFirmado = new System.Windows.Forms.Button();
            this.btnCopiarXmlFirmado = new System.Windows.Forms.Button();

            this.tabXml = new System.Windows.Forms.TabControl();
            this.tabXmlSinFirmar = new System.Windows.Forms.TabPage();
            this.tabXmlFirmado = new System.Windows.Forms.TabPage();
            this.txtXmlSinFirmar = new System.Windows.Forms.RichTextBox();
            this.txtXmlFirmado = new System.Windows.Forms.RichTextBox();

            this.panelHeader.SuspendLayout();
            this.panelFiltros.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudPostulacionId)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitMain)).BeginInit();
            this.splitMain.Panel1.SuspendLayout();
            this.splitMain.Panel2.SuspendLayout();
            this.splitMain.SuspendLayout();
            this.panelLeft.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grid)).BeginInit();
            this.panelRight.SuspendLayout();
            this.layoutRight.SuspendLayout();
            this.grpAcciones.SuspendLayout();
            this.grpCabecera.SuspendLayout();
            this.layoutCabecera.SuspendLayout();
            this.grpFirma.SuspendLayout();
            this.layoutFirma.SuspendLayout();
            this.grpValidaciones.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvValidaciones)).BeginInit();
            this.grpHistorial.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvHistorial)).BeginInit();
            this.panelXmlActions.SuspendLayout();
            this.tabXml.SuspendLayout();
            this.tabXmlSinFirmar.SuspendLayout();
            this.tabXmlFirmado.SuspendLayout();
            this.SuspendLayout();

            // panelHeader
            this.panelHeader.BackColor = System.Drawing.Color.FromArgb(17, 24, 39);
            this.panelHeader.Controls.Add(this.lblTitulo);
            this.panelHeader.Controls.Add(this.lblSubtitulo);
            this.panelHeader.Controls.Add(this.lblEstadoOperacion);
            this.panelHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelHeader.Location = new System.Drawing.Point(0, 0);
            this.panelHeader.Name = "panelHeader";
            this.panelHeader.Padding = new System.Windows.Forms.Padding(18, 16, 18, 16);
            this.panelHeader.Size = new System.Drawing.Size(1700, 88);
            this.panelHeader.TabIndex = 0;

            // lblTitulo
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI Semibold", 18F, System.Drawing.FontStyle.Bold);
            this.lblTitulo.ForeColor = System.Drawing.Color.White;
            this.lblTitulo.Location = new System.Drawing.Point(18, 12);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(240, 32);
            this.lblTitulo.TabIndex = 0;
            this.lblTitulo.Text = "Postulación DGII e-CF";

            // lblSubtitulo
            this.lblSubtitulo.AutoSize = true;
            this.lblSubtitulo.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.lblSubtitulo.ForeColor = System.Drawing.Color.Gainsboro;
            this.lblSubtitulo.Location = new System.Drawing.Point(20, 50);
            this.lblSubtitulo.Name = "lblSubtitulo";
            this.lblSubtitulo.Size = new System.Drawing.Size(677, 17);
            this.lblSubtitulo.TabIndex = 1;
            this.lblSubtitulo.Text = "Validación, generación, firmado y trazabilidad de la postulación DGII basada en el XML oficial.";

            // lblEstadoOperacion
            this.lblEstadoOperacion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblEstadoOperacion.BackColor = System.Drawing.Color.FromArgb(30, 41, 59);
            this.lblEstadoOperacion.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.lblEstadoOperacion.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.lblEstadoOperacion.Location = new System.Drawing.Point(1420, 26);
            this.lblEstadoOperacion.Name = "lblEstadoOperacion";
            this.lblEstadoOperacion.Padding = new System.Windows.Forms.Padding(12, 8, 12, 8);
            this.lblEstadoOperacion.Size = new System.Drawing.Size(250, 34);
            this.lblEstadoOperacion.TabIndex = 2;
            this.lblEstadoOperacion.Text = "Listo";
            this.lblEstadoOperacion.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            // panelFiltros
            this.panelFiltros.BackColor = System.Drawing.Color.White;
            this.panelFiltros.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelFiltros.Controls.Add(this.lblFiltroEstado);
            this.panelFiltros.Controls.Add(this.cboEstadoFiltro);
            this.panelFiltros.Controls.Add(this.lblPostulacionId);
            this.panelFiltros.Controls.Add(this.nudPostulacionId);
            this.panelFiltros.Controls.Add(this.lblUsuario);
            this.panelFiltros.Controls.Add(this.txtUsuario);
            this.panelFiltros.Controls.Add(this.btnRefrescar);
            this.panelFiltros.Controls.Add(this.btnVerDetalle);
            this.panelFiltros.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelFiltros.Location = new System.Drawing.Point(0, 88);
            this.panelFiltros.Name = "panelFiltros";
            this.panelFiltros.Padding = new System.Windows.Forms.Padding(14, 12, 14, 12);
            this.panelFiltros.Size = new System.Drawing.Size(1700, 72);
            this.panelFiltros.TabIndex = 1;

            // lblFiltroEstado
            this.lblFiltroEstado.AutoSize = true;
            this.lblFiltroEstado.Location = new System.Drawing.Point(18, 8);
            this.lblFiltroEstado.Name = "lblFiltroEstado";
            this.lblFiltroEstado.Size = new System.Drawing.Size(45, 17);
            this.lblFiltroEstado.TabIndex = 0;
            this.lblFiltroEstado.Text = "Estado";

            // cboEstadoFiltro
            this.cboEstadoFiltro.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboEstadoFiltro.FormattingEnabled = true;
            this.cboEstadoFiltro.Location = new System.Drawing.Point(18, 28);
            this.cboEstadoFiltro.Name = "cboEstadoFiltro";
            this.cboEstadoFiltro.Size = new System.Drawing.Size(180, 25);
            this.cboEstadoFiltro.TabIndex = 1;

            // lblPostulacionId
            this.lblPostulacionId.AutoSize = true;
            this.lblPostulacionId.Location = new System.Drawing.Point(214, 8);
            this.lblPostulacionId.Name = "lblPostulacionId";
            this.lblPostulacionId.Size = new System.Drawing.Size(84, 17);
            this.lblPostulacionId.TabIndex = 2;
            this.lblPostulacionId.Text = "PostulaciónId";

            // nudPostulacionId
            this.nudPostulacionId.Location = new System.Drawing.Point(214, 28);
            this.nudPostulacionId.Maximum = new decimal(new int[] { 999999999, 0, 0, 0 });
            this.nudPostulacionId.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            this.nudPostulacionId.Name = "nudPostulacionId";
            this.nudPostulacionId.Size = new System.Drawing.Size(150, 25);
            this.nudPostulacionId.TabIndex = 3;
            this.nudPostulacionId.Value = new decimal(new int[] { 1, 0, 0, 0 });

            // lblUsuario
            this.lblUsuario.AutoSize = true;
            this.lblUsuario.Location = new System.Drawing.Point(380, 8);
            this.lblUsuario.Name = "lblUsuario";
            this.lblUsuario.Size = new System.Drawing.Size(49, 17);
            this.lblUsuario.TabIndex = 4;
            this.lblUsuario.Text = "Usuario";

            // txtUsuario
            this.txtUsuario.Location = new System.Drawing.Point(380, 28);
            this.txtUsuario.Name = "txtUsuario";
            this.txtUsuario.Size = new System.Drawing.Size(150, 25);
            this.txtUsuario.TabIndex = 5;

            // btnRefrescar
            this.btnRefrescar.BackColor = System.Drawing.Color.FromArgb(13, 110, 253);
            this.btnRefrescar.FlatAppearance.BorderSize = 0;
            this.btnRefrescar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRefrescar.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnRefrescar.ForeColor = System.Drawing.Color.White;
            this.btnRefrescar.Location = new System.Drawing.Point(548, 23);
            this.btnRefrescar.Name = "btnRefrescar";
            this.btnRefrescar.Size = new System.Drawing.Size(110, 34);
            this.btnRefrescar.TabIndex = 6;
            this.btnRefrescar.Text = "Refrescar";
            this.btnRefrescar.UseVisualStyleBackColor = false;

            // btnVerDetalle
            this.btnVerDetalle.BackColor = System.Drawing.Color.FromArgb(108, 117, 125);
            this.btnVerDetalle.FlatAppearance.BorderSize = 0;
            this.btnVerDetalle.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnVerDetalle.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnVerDetalle.ForeColor = System.Drawing.Color.White;
            this.btnVerDetalle.Location = new System.Drawing.Point(668, 23);
            this.btnVerDetalle.Name = "btnVerDetalle";
            this.btnVerDetalle.Size = new System.Drawing.Size(130, 34);
            this.btnVerDetalle.TabIndex = 7;
            this.btnVerDetalle.Text = "Ver Detalle";
            this.btnVerDetalle.UseVisualStyleBackColor = false;

            // splitMain
            this.splitMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitMain.Location = new System.Drawing.Point(0, 160);
            this.splitMain.Name = "splitMain";
            this.splitMain.Panel1.Controls.Add(this.panelLeft);
            this.splitMain.Panel2.Controls.Add(this.panelRight);
            this.splitMain.Size = new System.Drawing.Size(1700, 860);
            this.splitMain.SplitterDistance = 560;
            this.splitMain.TabIndex = 2;

            // panelLeft
            this.panelLeft.BackColor = System.Drawing.Color.White;
            this.panelLeft.Controls.Add(this.grid);
            this.panelLeft.Controls.Add(this.lblResumen);
            this.panelLeft.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelLeft.Padding = new System.Windows.Forms.Padding(14);
            this.panelLeft.Name = "panelLeft";
            this.panelLeft.TabIndex = 0;

            // lblResumen
            this.lblResumen.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblResumen.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.lblResumen.ForeColor = System.Drawing.Color.FromArgb(52, 58, 64);
            this.lblResumen.Location = new System.Drawing.Point(14, 14);
            this.lblResumen.Name = "lblResumen";
            this.lblResumen.Padding = new System.Windows.Forms.Padding(0, 0, 0, 8);
            this.lblResumen.Size = new System.Drawing.Size(532, 28);
            this.lblResumen.TabIndex = 0;
            this.lblResumen.Text = "Registros: 0";

            // grid
            this.grid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grid.Location = new System.Drawing.Point(14, 42);
            this.grid.Name = "grid";
            this.grid.Size = new System.Drawing.Size(532, 804);
            this.grid.TabIndex = 1;

            // panelRight
            this.panelRight.BackColor = System.Drawing.Color.FromArgb(248, 249, 250);
            this.panelRight.Controls.Add(this.layoutRight);
            this.panelRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelRight.Padding = new System.Windows.Forms.Padding(12);
            this.panelRight.Name = "panelRight";
            this.panelRight.TabIndex = 0;

            // layoutRight
            this.layoutRight.ColumnCount = 1;
            this.layoutRight.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.layoutRight.Controls.Add(this.grpAcciones, 0, 0);
            this.layoutRight.Controls.Add(this.grpCabecera, 0, 1);
            this.layoutRight.Controls.Add(this.grpFirma, 0, 2);
            this.layoutRight.Controls.Add(this.grpValidaciones, 0, 3);
            this.layoutRight.Controls.Add(this.grpHistorial, 0, 4);
            this.layoutRight.Controls.Add(this.panelXmlActions, 0, 5);
            this.layoutRight.Controls.Add(this.tabXml, 0, 6);
            this.layoutRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutRight.Location = new System.Drawing.Point(12, 12);
            this.layoutRight.Name = "layoutRight";
            this.layoutRight.RowCount = 7;
            this.layoutRight.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 130F));
            this.layoutRight.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 190F));
            this.layoutRight.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 220F));
            this.layoutRight.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 160F));
            this.layoutRight.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 180F));
            this.layoutRight.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 44F));
            this.layoutRight.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.layoutRight.Size = new System.Drawing.Size(1112, 836);
            this.layoutRight.TabIndex = 0;

            // grpAcciones
            this.grpAcciones.Controls.Add(this.lblPfxPath);
            this.grpAcciones.Controls.Add(this.txtPfxPath);
            this.grpAcciones.Controls.Add(this.btnBuscarPfx);
            this.grpAcciones.Controls.Add(this.btnAbrirCarpetaPfx);
            this.grpAcciones.Controls.Add(this.lblPfxPassword);
            this.grpAcciones.Controls.Add(this.txtPfxPassword);
            this.grpAcciones.Controls.Add(this.btnValidar);
            this.grpAcciones.Controls.Add(this.btnGenerarXml);
            this.grpAcciones.Controls.Add(this.btnFirmar);
            this.grpAcciones.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpAcciones.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.grpAcciones.Location = new System.Drawing.Point(3, 3);
            this.grpAcciones.Name = "grpAcciones";
            this.grpAcciones.Size = new System.Drawing.Size(1106, 124);
            this.grpAcciones.TabIndex = 0;
            this.grpAcciones.TabStop = false;
            this.grpAcciones.Text = "Acciones de validación, generación y firmado";

            // lblPfxPath
            this.lblPfxPath.AutoSize = true;
            this.lblPfxPath.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblPfxPath.Location = new System.Drawing.Point(16, 28);
            this.lblPfxPath.Name = "lblPfxPath";
            this.lblPfxPath.Size = new System.Drawing.Size(115, 15);
            this.lblPfxPath.TabIndex = 0;
            this.lblPfxPath.Text = "Ruta del certificado .pfx";

            // txtPfxPath
            this.txtPfxPath.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.txtPfxPath.Location = new System.Drawing.Point(16, 48);
            this.txtPfxPath.Name = "txtPfxPath";
            this.txtPfxPath.Size = new System.Drawing.Size(640, 24);
            this.txtPfxPath.TabIndex = 1;

            // btnBuscarPfx
            this.btnBuscarPfx.BackColor = System.Drawing.Color.White;
            this.btnBuscarPfx.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBuscarPfx.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnBuscarPfx.Location = new System.Drawing.Point(666, 46);
            this.btnBuscarPfx.Name = "btnBuscarPfx";
            this.btnBuscarPfx.Size = new System.Drawing.Size(90, 28);
            this.btnBuscarPfx.TabIndex = 2;
            this.btnBuscarPfx.Text = "Buscar";
            this.btnBuscarPfx.UseVisualStyleBackColor = false;

            // btnAbrirCarpetaPfx
            this.btnAbrirCarpetaPfx.BackColor = System.Drawing.Color.White;
            this.btnAbrirCarpetaPfx.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAbrirCarpetaPfx.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnAbrirCarpetaPfx.Location = new System.Drawing.Point(764, 46);
            this.btnAbrirCarpetaPfx.Name = "btnAbrirCarpetaPfx";
            this.btnAbrirCarpetaPfx.Size = new System.Drawing.Size(120, 28);
            this.btnAbrirCarpetaPfx.TabIndex = 3;
            this.btnAbrirCarpetaPfx.Text = "Abrir carpeta";
            this.btnAbrirCarpetaPfx.UseVisualStyleBackColor = false;

            // lblPfxPassword
            this.lblPfxPassword.AutoSize = true;
            this.lblPfxPassword.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblPfxPassword.Location = new System.Drawing.Point(16, 82);
            this.lblPfxPassword.Name = "lblPfxPassword";
            this.lblPfxPassword.Size = new System.Drawing.Size(93, 15);
            this.lblPfxPassword.TabIndex = 4;
            this.lblPfxPassword.Text = "Clave del archivo";

            // txtPfxPassword
            this.txtPfxPassword.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.txtPfxPassword.Location = new System.Drawing.Point(16, 100);
            this.txtPfxPassword.Name = "txtPfxPassword";
            this.txtPfxPassword.PasswordChar = '*';
            this.txtPfxPassword.Size = new System.Drawing.Size(220, 24);
            this.txtPfxPassword.TabIndex = 5;

            // btnValidar
            this.btnValidar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnValidar.BackColor = System.Drawing.Color.FromArgb(255, 193, 7);
            this.btnValidar.FlatAppearance.BorderSize = 0;
            this.btnValidar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnValidar.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnValidar.ForeColor = System.Drawing.Color.Black;
            this.btnValidar.Location = new System.Drawing.Point(760, 90);
            this.btnValidar.Name = "btnValidar";
            this.btnValidar.Size = new System.Drawing.Size(110, 30);
            this.btnValidar.TabIndex = 6;
            this.btnValidar.Text = "Validar";
            this.btnValidar.UseVisualStyleBackColor = false;

            // btnGenerarXml
            this.btnGenerarXml.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGenerarXml.BackColor = System.Drawing.Color.FromArgb(13, 110, 253);
            this.btnGenerarXml.FlatAppearance.BorderSize = 0;
            this.btnGenerarXml.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGenerarXml.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnGenerarXml.ForeColor = System.Drawing.Color.White;
            this.btnGenerarXml.Location = new System.Drawing.Point(878, 90);
            this.btnGenerarXml.Name = "btnGenerarXml";
            this.btnGenerarXml.Size = new System.Drawing.Size(110, 30);
            this.btnGenerarXml.TabIndex = 7;
            this.btnGenerarXml.Text = "Generar XML";
            this.btnGenerarXml.UseVisualStyleBackColor = false;

            // btnFirmar
            this.btnFirmar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFirmar.BackColor = System.Drawing.Color.FromArgb(25, 135, 84);
            this.btnFirmar.FlatAppearance.BorderSize = 0;
            this.btnFirmar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFirmar.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnFirmar.ForeColor = System.Drawing.Color.White;
            this.btnFirmar.Location = new System.Drawing.Point(994, 90);
            this.btnFirmar.Name = "btnFirmar";
            this.btnFirmar.Size = new System.Drawing.Size(100, 30);
            this.btnFirmar.TabIndex = 8;
            this.btnFirmar.Text = "Firmar XML";
            this.btnFirmar.UseVisualStyleBackColor = false;

            // grpCabecera
            this.grpCabecera.Controls.Add(this.layoutCabecera);
            this.grpCabecera.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpCabecera.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.grpCabecera.Location = new System.Drawing.Point(3, 133);
            this.grpCabecera.Name = "grpCabecera";
            this.grpCabecera.Size = new System.Drawing.Size(1106, 184);
            this.grpCabecera.TabIndex = 1;
            this.grpCabecera.TabStop = false;
            this.grpCabecera.Text = "Datos generales de la postulación";

            // layoutCabecera
            this.layoutCabecera.ColumnCount = 4;
            this.layoutCabecera.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 150F));
            this.layoutCabecera.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.layoutCabecera.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 150F));
            this.layoutCabecera.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.layoutCabecera.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutCabecera.Location = new System.Drawing.Point(3, 20);
            this.layoutCabecera.Name = "layoutCabecera";
            this.layoutCabecera.RowCount = 4;
            this.layoutCabecera.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 38F));
            this.layoutCabecera.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 38F));
            this.layoutCabecera.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 38F));
            this.layoutCabecera.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 38F));
            this.layoutCabecera.Size = new System.Drawing.Size(1100, 161);
            this.layoutCabecera.TabIndex = 0;

            AddLabel(this.layoutCabecera, "Postulación GUID", 0, 0);
            this.layoutCabecera.Controls.Add(this.txtPostulacionGuid, 1, 0);
            AddLabel(this.layoutCabecera, "Estado", 2, 0);
            this.layoutCabecera.Controls.Add(this.txtEstado, 3, 0);

            AddLabel(this.layoutCabecera, "Tipo Registro", 0, 1);
            this.layoutCabecera.Controls.Add(this.txtTipoRegistro, 1, 1);
            AddLabel(this.layoutCabecera, "Grupo Comprobante", 2, 1);
            this.layoutCabecera.Controls.Add(this.txtGrupoComprobante, 3, 1);

            AddLabel(this.layoutCabecera, "Fecha Solicitud", 0, 2);
            this.layoutCabecera.Controls.Add(this.txtFechaSolicitud, 1, 2);
            AddLabel(this.layoutCabecera, "Fecha Generado", 2, 2);
            this.layoutCabecera.Controls.Add(this.txtFechaGenerado, 3, 2);

            AddLabel(this.layoutCabecera, "Fecha Firmado", 0, 3);
            this.layoutCabecera.Controls.Add(this.txtFechaFirma, 1, 3);
            AddLabel(this.layoutCabecera, "Fecha Enviado", 2, 3);
            this.layoutCabecera.Controls.Add(this.txtFechaEnviado, 3, 3);

            // grpFirma
            this.grpFirma.Controls.Add(this.layoutFirma);
            this.grpFirma.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpFirma.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.grpFirma.Location = new System.Drawing.Point(3, 323);
            this.grpFirma.Name = "grpFirma";
            this.grpFirma.Size = new System.Drawing.Size(1106, 214);
            this.grpFirma.TabIndex = 2;
            this.grpFirma.TabStop = false;
            this.grpFirma.Text = "Metadatos de firma digital";

            // layoutFirma
            this.layoutFirma.ColumnCount = 4;
            this.layoutFirma.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 160F));
            this.layoutFirma.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.layoutFirma.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 160F));
            this.layoutFirma.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.layoutFirma.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutFirma.Location = new System.Drawing.Point(3, 20);
            this.layoutFirma.Name = "layoutFirma";
            this.layoutFirma.RowCount = 5;
            this.layoutFirma.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 36F));
            this.layoutFirma.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 36F));
            this.layoutFirma.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 36F));
            this.layoutFirma.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 36F));
            this.layoutFirma.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 36F));
            this.layoutFirma.Size = new System.Drawing.Size(1100, 191);
            this.layoutFirma.TabIndex = 0;

            AddLabel(this.layoutFirma, "DigestValue", 0, 0);
            this.layoutFirma.Controls.Add(this.txtDigestValue, 1, 0);
            AddLabel(this.layoutFirma, "SignatureValue", 2, 0);
            this.layoutFirma.Controls.Add(this.txtSignatureValue, 3, 0);

            AddLabel(this.layoutFirma, "Canonicalization", 0, 1);
            this.layoutFirma.Controls.Add(this.txtCanonicalization, 1, 1);
            AddLabel(this.layoutFirma, "SignatureMethod", 2, 1);
            this.layoutFirma.Controls.Add(this.txtSignatureMethod, 3, 1);

            AddLabel(this.layoutFirma, "Thumbprint", 0, 2);
            this.layoutFirma.Controls.Add(this.txtThumbprint, 1, 2);
            AddLabel(this.layoutFirma, "Serial", 2, 2);
            this.layoutFirma.Controls.Add(this.txtSerial, 3, 2);

            AddLabel(this.layoutFirma, "CertIssuer", 0, 3);
            this.layoutFirma.Controls.Add(this.txtCertIssuer, 1, 3);
            AddLabel(this.layoutFirma, "CertSubject", 2, 3);
            this.layoutFirma.Controls.Add(this.txtCertSubject, 3, 3);

            AddLabel(this.layoutFirma, "HashDocumento", 0, 4);
            this.layoutFirma.Controls.Add(this.txtHash, 1, 4);

            // grpValidaciones
            this.grpValidaciones.Controls.Add(this.dgvValidaciones);
            this.grpValidaciones.Controls.Add(this.lblValidacionResumen);
            this.grpValidaciones.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpValidaciones.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.grpValidaciones.Location = new System.Drawing.Point(3, 543);
            this.grpValidaciones.Name = "grpValidaciones";
            this.grpValidaciones.Size = new System.Drawing.Size(1106, 154);
            this.grpValidaciones.TabIndex = 3;
            this.grpValidaciones.TabStop = false;
            this.grpValidaciones.Text = "Validaciones";

            // lblValidacionResumen
            this.lblValidacionResumen.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblValidacionResumen.Font = new System.Drawing.Font("Segoe UI", 9.25F, System.Drawing.FontStyle.Bold);
            this.lblValidacionResumen.ForeColor = System.Drawing.Color.FromArgb(25, 135, 84);
            this.lblValidacionResumen.Location = new System.Drawing.Point(3, 20);
            this.lblValidacionResumen.Name = "lblValidacionResumen";
            this.lblValidacionResumen.Padding = new System.Windows.Forms.Padding(8, 0, 0, 6);
            this.lblValidacionResumen.Size = new System.Drawing.Size(1100, 28);
            this.lblValidacionResumen.TabIndex = 0;
            this.lblValidacionResumen.Text = "Validación: pendiente";

            // dgvValidaciones
            this.dgvValidaciones.AllowUserToAddRows = false;
            this.dgvValidaciones.AllowUserToDeleteRows = false;
            this.dgvValidaciones.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvValidaciones.BackgroundColor = System.Drawing.Color.White;
            this.dgvValidaciones.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvValidaciones.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvValidaciones.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvValidaciones.Location = new System.Drawing.Point(3, 48);
            this.dgvValidaciones.MultiSelect = false;
            this.dgvValidaciones.Name = "dgvValidaciones";
            this.dgvValidaciones.ReadOnly = true;
            this.dgvValidaciones.RowHeadersVisible = false;
            this.dgvValidaciones.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvValidaciones.Size = new System.Drawing.Size(1100, 103);
            this.dgvValidaciones.TabIndex = 1;

            // grpHistorial
            this.grpHistorial.Controls.Add(this.dgvHistorial);
            this.grpHistorial.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpHistorial.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.grpHistorial.Location = new System.Drawing.Point(3, 703);
            this.grpHistorial.Name = "grpHistorial";
            this.grpHistorial.Size = new System.Drawing.Size(1106, 174);
            this.grpHistorial.TabIndex = 4;
            this.grpHistorial.TabStop = false;
            this.grpHistorial.Text = "Historial / bitácora";

            // dgvHistorial
            this.dgvHistorial.AllowUserToAddRows = false;
            this.dgvHistorial.AllowUserToDeleteRows = false;
            this.dgvHistorial.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells;
            this.dgvHistorial.BackgroundColor = System.Drawing.Color.White;
            this.dgvHistorial.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvHistorial.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvHistorial.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvHistorial.Location = new System.Drawing.Point(3, 20);
            this.dgvHistorial.MultiSelect = false;
            this.dgvHistorial.Name = "dgvHistorial";
            this.dgvHistorial.ReadOnly = true;
            this.dgvHistorial.RowHeadersVisible = false;
            this.dgvHistorial.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvHistorial.Size = new System.Drawing.Size(1100, 151);
            this.dgvHistorial.TabIndex = 0;

            // panelXmlActions
            this.panelXmlActions.Controls.Add(this.btnGuardarXmlSinFirmar);
            this.panelXmlActions.Controls.Add(this.btnCopiarXmlSinFirmar);
            this.panelXmlActions.Controls.Add(this.btnGuardarXmlFirmado);
            this.panelXmlActions.Controls.Add(this.btnCopiarXmlFirmado);
            this.panelXmlActions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelXmlActions.Location = new System.Drawing.Point(3, 883);
            this.panelXmlActions.Name = "panelXmlActions";
            this.panelXmlActions.Size = new System.Drawing.Size(1106, 38);
            this.panelXmlActions.TabIndex = 5;

            // btnGuardarXmlSinFirmar
            this.btnGuardarXmlSinFirmar.BackColor = System.Drawing.Color.White;
            this.btnGuardarXmlSinFirmar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGuardarXmlSinFirmar.Location = new System.Drawing.Point(0, 4);
            this.btnGuardarXmlSinFirmar.Name = "btnGuardarXmlSinFirmar";
            this.btnGuardarXmlSinFirmar.Size = new System.Drawing.Size(170, 30);
            this.btnGuardarXmlSinFirmar.TabIndex = 0;
            this.btnGuardarXmlSinFirmar.Text = "Guardar XML sin firmar";
            this.btnGuardarXmlSinFirmar.UseVisualStyleBackColor = false;

            // btnCopiarXmlSinFirmar
            this.btnCopiarXmlSinFirmar.BackColor = System.Drawing.Color.White;
            this.btnCopiarXmlSinFirmar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCopiarXmlSinFirmar.Location = new System.Drawing.Point(176, 4);
            this.btnCopiarXmlSinFirmar.Name = "btnCopiarXmlSinFirmar";
            this.btnCopiarXmlSinFirmar.Size = new System.Drawing.Size(160, 30);
            this.btnCopiarXmlSinFirmar.TabIndex = 1;
            this.btnCopiarXmlSinFirmar.Text = "Copiar XML sin firmar";
            this.btnCopiarXmlSinFirmar.UseVisualStyleBackColor = false;

            // btnGuardarXmlFirmado
            this.btnGuardarXmlFirmado.BackColor = System.Drawing.Color.White;
            this.btnGuardarXmlFirmado.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGuardarXmlFirmado.Location = new System.Drawing.Point(380, 4);
            this.btnGuardarXmlFirmado.Name = "btnGuardarXmlFirmado";
            this.btnGuardarXmlFirmado.Size = new System.Drawing.Size(170, 30);
            this.btnGuardarXmlFirmado.TabIndex = 2;
            this.btnGuardarXmlFirmado.Text = "Guardar XML firmado";
            this.btnGuardarXmlFirmado.UseVisualStyleBackColor = false;

            // btnCopiarXmlFirmado
            this.btnCopiarXmlFirmado.BackColor = System.Drawing.Color.White;
            this.btnCopiarXmlFirmado.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCopiarXmlFirmado.Location = new System.Drawing.Point(556, 4);
            this.btnCopiarXmlFirmado.Name = "btnCopiarXmlFirmado";
            this.btnCopiarXmlFirmado.Size = new System.Drawing.Size(160, 30);
            this.btnCopiarXmlFirmado.TabIndex = 3;
            this.btnCopiarXmlFirmado.Text = "Copiar XML firmado";
            this.btnCopiarXmlFirmado.UseVisualStyleBackColor = false;

            // tabXml
            this.tabXml.Controls.Add(this.tabXmlSinFirmar);
            this.tabXml.Controls.Add(this.tabXmlFirmado);
            this.tabXml.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabXml.Location = new System.Drawing.Point(3, 927);
            this.tabXml.Name = "tabXml";
            this.tabXml.SelectedIndex = 0;
            this.tabXml.Size = new System.Drawing.Size(1106, 1);
            this.tabXml.TabIndex = 6;

            // tabXmlSinFirmar
            this.tabXmlSinFirmar.Controls.Add(this.txtXmlSinFirmar);
            this.tabXmlSinFirmar.Location = new System.Drawing.Point(4, 26);
            this.tabXmlSinFirmar.Name = "tabXmlSinFirmar";
            this.tabXmlSinFirmar.Padding = new System.Windows.Forms.Padding(3);
            this.tabXmlSinFirmar.Size = new System.Drawing.Size(1098, 0);
            this.tabXmlSinFirmar.TabIndex = 0;
            this.tabXmlSinFirmar.Text = "XML sin firmar";
            this.tabXmlSinFirmar.UseVisualStyleBackColor = true;

            // tabXmlFirmado
            this.tabXmlFirmado.Controls.Add(this.txtXmlFirmado);
            this.tabXmlFirmado.Location = new System.Drawing.Point(4, 26);
            this.tabXmlFirmado.Name = "tabXmlFirmado";
            this.tabXmlFirmado.Padding = new System.Windows.Forms.Padding(3);
            this.tabXmlFirmado.Size = new System.Drawing.Size(1098, 0);
            this.tabXmlFirmado.TabIndex = 1;
            this.tabXmlFirmado.Text = "XML firmado";
            this.tabXmlFirmado.UseVisualStyleBackColor = true;

            // txtXmlSinFirmar
            this.txtXmlSinFirmar.BackColor = System.Drawing.Color.FromArgb(248, 249, 250);
            this.txtXmlSinFirmar.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtXmlSinFirmar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtXmlSinFirmar.Font = new System.Drawing.Font("Consolas", 9F);
            this.txtXmlSinFirmar.Location = new System.Drawing.Point(3, 3);
            this.txtXmlSinFirmar.Name = "txtXmlSinFirmar";
            this.txtXmlSinFirmar.Size = new System.Drawing.Size(1092, 0);
            this.txtXmlSinFirmar.TabIndex = 0;
            this.txtXmlSinFirmar.Text = "";

            // txtXmlFirmado
            this.txtXmlFirmado.BackColor = System.Drawing.Color.FromArgb(248, 249, 250);
            this.txtXmlFirmado.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtXmlFirmado.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtXmlFirmado.Font = new System.Drawing.Font("Consolas", 9F);
            this.txtXmlFirmado.Location = new System.Drawing.Point(3, 3);
            this.txtXmlFirmado.Name = "txtXmlFirmado";
            this.txtXmlFirmado.Size = new System.Drawing.Size(1092, 0);
            this.txtXmlFirmado.TabIndex = 0;
            this.txtXmlFirmado.Text = "";

            // Apply textbox style
            SetReadOnlyTextBox(this.txtPostulacionGuid);
            SetReadOnlyTextBox(this.txtTipoRegistro);
            SetReadOnlyTextBox(this.txtGrupoComprobante);
            SetReadOnlyTextBox(this.txtEstado);
            SetReadOnlyTextBox(this.txtFechaSolicitud);
            SetReadOnlyTextBox(this.txtFechaGenerado);
            SetReadOnlyTextBox(this.txtFechaFirma);
            SetReadOnlyTextBox(this.txtFechaEnviado);

            SetReadOnlyTextBox(this.txtDigestValue);
            SetReadOnlyTextBox(this.txtSignatureValue);
            SetReadOnlyTextBox(this.txtCanonicalization);
            SetReadOnlyTextBox(this.txtSignatureMethod);
            SetReadOnlyTextBox(this.txtThumbprint);
            SetReadOnlyTextBox(this.txtSerial);
            SetReadOnlyTextBox(this.txtCertIssuer);
            SetReadOnlyTextBox(this.txtCertSubject);
            SetReadOnlyTextBox(this.txtHash);

            // FormPostulacionDGII
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(248, 249, 250);
            this.ClientSize = new System.Drawing.Size(1700, 1020);
            this.Controls.Add(this.splitMain);
            this.Controls.Add(this.panelFiltros);
            this.Controls.Add(this.panelHeader);
            this.Name = "FormPostulacionDGII";
            this.Text = "Postulación DGII - e-CF";

            this.panelHeader.ResumeLayout(false);
            this.panelHeader.PerformLayout();
            this.panelFiltros.ResumeLayout(false);
            this.panelFiltros.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudPostulacionId)).EndInit();
            this.splitMain.Panel1.ResumeLayout(false);
            this.splitMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitMain)).EndInit();
            this.splitMain.ResumeLayout(false);
            this.panelLeft.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grid)).EndInit();
            this.panelRight.ResumeLayout(false);
            this.layoutRight.ResumeLayout(false);
            this.grpAcciones.ResumeLayout(false);
            this.grpAcciones.PerformLayout();
            this.grpCabecera.ResumeLayout(false);
            this.layoutCabecera.ResumeLayout(false);
            this.layoutCabecera.PerformLayout();
            this.grpFirma.ResumeLayout(false);
            this.layoutFirma.ResumeLayout(false);
            this.layoutFirma.PerformLayout();
            this.grpValidaciones.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvValidaciones)).EndInit();
            this.grpHistorial.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvHistorial)).EndInit();
            this.panelXmlActions.ResumeLayout(false);
            this.tabXml.ResumeLayout(false);
            this.tabXmlSinFirmar.ResumeLayout(false);
            this.tabXmlFirmado.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        private static void SetReadOnlyTextBox(System.Windows.Forms.TextBox txt)
        {
            txt.ReadOnly = true;
            txt.Dock = System.Windows.Forms.DockStyle.Fill;
            txt.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            txt.BackColor = System.Drawing.Color.White;
            txt.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            txt.Font = new System.Drawing.Font("Segoe UI", 9.5F);
        }

        private static void AddLabel(System.Windows.Forms.TableLayoutPanel layout, string text, int col, int row)
        {
            var lbl = new System.Windows.Forms.Label();
            lbl.Text = text;
            lbl.Dock = System.Windows.Forms.DockStyle.Fill;
            lbl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            lbl.Font = new System.Drawing.Font("Segoe UI", 9F);
            lbl.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            layout.Controls.Add(lbl, col, row);
        }
    }
}