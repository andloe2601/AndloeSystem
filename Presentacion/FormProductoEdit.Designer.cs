using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using WinCheckBox = System.Windows.Forms.CheckBox;
using WinColor = System.Drawing.Color;

namespace Andloe.Presentacion
{
    partial class FormProductoEdit
    {
        private IContainer components = null;

        private TextBox txtCodigo;
        private TextBox txtDescripcion;
        private TextBox txtDescripcionFiscal;
        private TextBox txtReferencia;
        private TextBox txtCodigoItemFiscal;

        private ComboBox cboUnidad;
        private ComboBox cboImpuesto;
        private ComboBox cboCategoria;
        private ComboBox cboSubcategoria;
        private ComboBox cboTipoProducto;

        private TextBox txtPrecioVenta;
        private TextBox txtPrecioCosto;
        private TextBox txtPrecioMayor;

        private TextBox txtExistencia;
        private TextBox txtUltimoPrecioCompra;
        private TextBox txtPorcBeneficio;

        private WinCheckBox chkActivo;
        private WinCheckBox chkBloqNegativo;
        private WinCheckBox chkPrecioIncluyeITBIS;

        private PictureBox picImagenProducto;
        private Button btnCargarImagen;
        private Button btnQuitarImagen;

        private Button btnAtras;
        private Button btnSiguiente;
        private Button btnGuardar;
        private Button btnFinalizar;

        private DataGridView gridBarras;
        private TextBox txtBarraManual;
        private Button btnAgregarBarraManual;
        private Button btnAgregarBarraAuto;
        private Button btnEliminarBarra;

        private DataGridViewTextBoxColumn colBarraCodigo;
        private DataGridViewTextBoxColumn colBarraTipo;
        private DataGridViewTextBoxColumn colBarraUsuario;
        private DataGridViewTextBoxColumn colBarraUltUso;

        private TableLayoutPanel root;
        private TableLayoutPanel tlMain;
        private GroupBox grpGeneral;
        private GroupBox grpFiscal;
        private GroupBox grpPrecios;
        private GroupBox grpInventario;
        private GroupBox grpImagen;
        private GroupBox grpBarras;
        private TableLayoutPanel tlGeneral;
        private TableLayoutPanel tlFiscal;
        private TableLayoutPanel tlPrecios;
        private TableLayoutPanel tlInventario;
        private TableLayoutPanel tlImagen;
        private TableLayoutPanel tlBarras;
        private FlowLayoutPanel flChecks;
        private FlowLayoutPanel flImagenButtons;
        private FlowLayoutPanel flBarrasTop;
        private FlowLayoutPanel flBottom;
        private Panel panelTop;
        private Panel panelBottom;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();

            base.Dispose(disposing);
        }

        private Label MakeLabel(string text)
        {
            return new Label
            {
                Text = text,
                AutoSize = true,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Margin = new Padding(3, 6, 3, 6)
            };
        }

        private void InitializeComponent()
        {
            components = new Container();

            root = new TableLayoutPanel();
            panelTop = new Panel();
            tlMain = new TableLayoutPanel();

            grpGeneral = new GroupBox();
            grpFiscal = new GroupBox();
            grpPrecios = new GroupBox();
            grpInventario = new GroupBox();
            grpImagen = new GroupBox();
            grpBarras = new GroupBox();

            tlGeneral = new TableLayoutPanel();
            tlFiscal = new TableLayoutPanel();
            tlPrecios = new TableLayoutPanel();
            tlInventario = new TableLayoutPanel();
            tlImagen = new TableLayoutPanel();
            tlBarras = new TableLayoutPanel();

            flChecks = new FlowLayoutPanel();
            flImagenButtons = new FlowLayoutPanel();
            flBarrasTop = new FlowLayoutPanel();
            flBottom = new FlowLayoutPanel();
            panelBottom = new Panel();

            txtCodigo = new TextBox();
            txtDescripcion = new TextBox();
            txtDescripcionFiscal = new TextBox();
            txtReferencia = new TextBox();
            txtCodigoItemFiscal = new TextBox();

            cboUnidad = new ComboBox();
            cboImpuesto = new ComboBox();
            cboCategoria = new ComboBox();
            cboSubcategoria = new ComboBox();
            cboTipoProducto = new ComboBox();

            txtPrecioVenta = new TextBox();
            txtPrecioCosto = new TextBox();
            txtPrecioMayor = new TextBox();

            txtExistencia = new TextBox();
            txtUltimoPrecioCompra = new TextBox();
            txtPorcBeneficio = new TextBox();

            chkActivo = new WinCheckBox();
            chkBloqNegativo = new WinCheckBox();
            chkPrecioIncluyeITBIS = new WinCheckBox();

            picImagenProducto = new PictureBox();
            btnCargarImagen = new Button();
            btnQuitarImagen = new Button();

            btnAtras = new Button();
            btnSiguiente = new Button();
            btnGuardar = new Button();
            btnFinalizar = new Button();

            gridBarras = new DataGridView();
            txtBarraManual = new TextBox();
            btnAgregarBarraManual = new Button();
            btnAgregarBarraAuto = new Button();
            btnEliminarBarra = new Button();

            colBarraCodigo = new DataGridViewTextBoxColumn();
            colBarraTipo = new DataGridViewTextBoxColumn();
            colBarraUsuario = new DataGridViewTextBoxColumn();
            colBarraUltUso = new DataGridViewTextBoxColumn();

            ((ISupportInitialize)(picImagenProducto)).BeginInit();
            ((ISupportInitialize)(gridBarras)).BeginInit();
            SuspendLayout();

            // root
            root.ColumnCount = 1;
            root.RowCount = 2;
            root.Dock = DockStyle.Fill;
            root.Padding = new Padding(12);
            root.BackColor = WinColor.FromArgb(245, 246, 248);
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 68F));
            root.Controls.Add(panelTop, 0, 0);
            root.Controls.Add(panelBottom, 0, 1);

            // panelTop
            panelTop.Dock = DockStyle.Fill;
            panelTop.Controls.Add(tlMain);

            // tlMain
            tlMain.ColumnCount = 3;
            tlMain.RowCount = 3;
            tlMain.Dock = DockStyle.Fill;
            tlMain.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 42F));
            tlMain.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33F));
            tlMain.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tlMain.RowStyles.Add(new RowStyle(SizeType.Absolute, 320F));
            tlMain.RowStyles.Add(new RowStyle(SizeType.Absolute, 235F));
            tlMain.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tlMain.Controls.Add(grpGeneral, 0, 0);
            tlMain.Controls.Add(grpFiscal, 1, 0);
            tlMain.Controls.Add(grpImagen, 2, 0);
            tlMain.Controls.Add(grpPrecios, 0, 1);
            tlMain.Controls.Add(grpInventario, 1, 1);
            tlMain.Controls.Add(flChecks, 2, 1);
            tlMain.Controls.Add(grpBarras, 0, 2);
            tlMain.SetColumnSpan(grpBarras, 3);

            // grpGeneral
            grpGeneral.Text = "Datos generales";
            grpGeneral.Dock = DockStyle.Fill;
            grpGeneral.Padding = new Padding(12);
            grpGeneral.Controls.Add(tlGeneral);

            // tlGeneral
            tlGeneral.ColumnCount = 2;
            tlGeneral.RowCount = 6;
            tlGeneral.Dock = DockStyle.Fill;
            tlGeneral.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120F));
            tlGeneral.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tlGeneral.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));
            tlGeneral.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));
            tlGeneral.RowStyles.Add(new RowStyle(SizeType.Absolute, 125F));
            tlGeneral.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));
            tlGeneral.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));
            tlGeneral.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            tlGeneral.Controls.Add(MakeLabel("Código"), 0, 0);
            tlGeneral.Controls.Add(txtCodigo, 1, 0);

            tlGeneral.Controls.Add(MakeLabel("Referencia"), 0, 1);
            tlGeneral.Controls.Add(txtReferencia, 1, 1);

            tlGeneral.Controls.Add(MakeLabel("Descripción"), 0, 2);
            tlGeneral.Controls.Add(txtDescripcion, 1, 2);

            tlGeneral.Controls.Add(MakeLabel("Categoría"), 0, 3);
            tlGeneral.Controls.Add(cboCategoria, 1, 3);

            tlGeneral.Controls.Add(MakeLabel("Subcategoría"), 0, 4);
            tlGeneral.Controls.Add(cboSubcategoria, 1, 4);

            // grpFiscal
            grpFiscal.Text = "Datos fiscales / e-CF";
            grpFiscal.Dock = DockStyle.Fill;
            grpFiscal.Padding = new Padding(12);
            grpFiscal.Controls.Add(tlFiscal);

            // tlFiscal
            tlFiscal.ColumnCount = 2;
            tlFiscal.RowCount = 6;
            tlFiscal.Dock = DockStyle.Fill;
            tlFiscal.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 135F));
            tlFiscal.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tlFiscal.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));
            tlFiscal.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));
            tlFiscal.RowStyles.Add(new RowStyle(SizeType.Absolute, 125F));
            tlFiscal.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));
            tlFiscal.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));
            tlFiscal.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            tlFiscal.Controls.Add(MakeLabel("Impuesto"), 0, 0);
            tlFiscal.Controls.Add(cboImpuesto, 1, 0);

            tlFiscal.Controls.Add(MakeLabel("Tipo producto"), 0, 1);
            tlFiscal.Controls.Add(cboTipoProducto, 1, 1);

            tlFiscal.Controls.Add(MakeLabel("Descripción fiscal"), 0, 2);
            tlFiscal.Controls.Add(txtDescripcionFiscal, 1, 2);

            tlFiscal.Controls.Add(MakeLabel("Código item fiscal"), 0, 3);
            tlFiscal.Controls.Add(txtCodigoItemFiscal, 1, 3);

            tlFiscal.Controls.Add(MakeLabel("Unidad e-CF"), 0, 4);
            tlFiscal.Controls.Add(cboUnidad, 1, 4);

            // grpImagen
            grpImagen.Text = "Imagen del producto";
            grpImagen.Dock = DockStyle.Fill;
            grpImagen.Padding = new Padding(12);
            grpImagen.Controls.Add(tlImagen);

            // tlImagen
            tlImagen.ColumnCount = 1;
            tlImagen.RowCount = 2;
            tlImagen.Dock = DockStyle.Fill;
            tlImagen.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tlImagen.RowStyles.Add(new RowStyle(SizeType.Absolute, 42F));
            tlImagen.Controls.Add(picImagenProducto, 0, 0);
            tlImagen.Controls.Add(flImagenButtons, 0, 1);

            // picImagenProducto
            picImagenProducto.Dock = DockStyle.Fill;
            picImagenProducto.BorderStyle = BorderStyle.FixedSingle;
            picImagenProducto.BackColor = WinColor.White;
            picImagenProducto.SizeMode = PictureBoxSizeMode.Zoom;
            picImagenProducto.Margin = new Padding(3);
            picImagenProducto.Name = "picImagenProducto";

            // flImagenButtons
            flImagenButtons.Dock = DockStyle.Fill;
            flImagenButtons.FlowDirection = FlowDirection.LeftToRight;
            flImagenButtons.WrapContents = false;
            flImagenButtons.Controls.Add(btnCargarImagen);
            flImagenButtons.Controls.Add(btnQuitarImagen);

            btnCargarImagen.Text = "Cargar imagen";
            btnCargarImagen.AutoSize = true;
            btnCargarImagen.Padding = new Padding(8, 4, 8, 4);
            btnCargarImagen.Name = "btnCargarImagen";

            btnQuitarImagen.Text = "Quitar imagen";
            btnQuitarImagen.AutoSize = true;
            btnQuitarImagen.Padding = new Padding(8, 4, 8, 4);
            btnQuitarImagen.Name = "btnQuitarImagen";

            // grpPrecios
            grpPrecios.Text = "Precios";
            grpPrecios.Dock = DockStyle.Fill;
            grpPrecios.Padding = new Padding(12);
            grpPrecios.Controls.Add(tlPrecios);

            // tlPrecios
            tlPrecios.ColumnCount = 2;
            tlPrecios.RowCount = 5;
            tlPrecios.Dock = DockStyle.Fill;
            tlPrecios.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 140F));
            tlPrecios.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tlPrecios.RowStyles.Add(new RowStyle(SizeType.Absolute, 42F));
            tlPrecios.RowStyles.Add(new RowStyle(SizeType.Absolute, 42F));
            tlPrecios.RowStyles.Add(new RowStyle(SizeType.Absolute, 42F));
            tlPrecios.RowStyles.Add(new RowStyle(SizeType.Absolute, 42F));
            tlPrecios.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            tlPrecios.Controls.Add(MakeLabel("Precio venta"), 0, 0);
            tlPrecios.Controls.Add(txtPrecioVenta, 1, 0);

            tlPrecios.Controls.Add(MakeLabel("Precio costo"), 0, 1);
            tlPrecios.Controls.Add(txtPrecioCosto, 1, 1);

            tlPrecios.Controls.Add(MakeLabel("Precio mayor"), 0, 2);
            tlPrecios.Controls.Add(txtPrecioMayor, 1, 2);

            // grpInventario
            grpInventario.Text = "Inventario / márgenes";
            grpInventario.Dock = DockStyle.Fill;
            grpInventario.Padding = new Padding(12);
            grpInventario.Controls.Add(tlInventario);

            // tlInventario
            tlInventario.ColumnCount = 2;
            tlInventario.RowCount = 5;
            tlInventario.Dock = DockStyle.Fill;
            tlInventario.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 145F));
            tlInventario.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tlInventario.RowStyles.Add(new RowStyle(SizeType.Absolute, 42F));
            tlInventario.RowStyles.Add(new RowStyle(SizeType.Absolute, 42F));
            tlInventario.RowStyles.Add(new RowStyle(SizeType.Absolute, 42F));
            tlInventario.RowStyles.Add(new RowStyle(SizeType.Absolute, 42F));
            tlInventario.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            tlInventario.Controls.Add(MakeLabel("Existencia"), 0, 0);
            tlInventario.Controls.Add(txtExistencia, 1, 0);

            tlInventario.Controls.Add(MakeLabel("Último precio compra"), 0, 1);
            tlInventario.Controls.Add(txtUltimoPrecioCompra, 1, 1);

            tlInventario.Controls.Add(MakeLabel("% beneficio"), 0, 2);
            tlInventario.Controls.Add(txtPorcBeneficio, 1, 2);

            // flChecks
            flChecks.Dock = DockStyle.Fill;
            flChecks.FlowDirection = FlowDirection.TopDown;
            flChecks.WrapContents = false;
            flChecks.Padding = new Padding(40, 18, 12, 12);
            flChecks.Controls.Add(chkActivo);
            flChecks.Controls.Add(chkPrecioIncluyeITBIS);
            flChecks.Controls.Add(chkBloqNegativo);

            chkActivo.Text = "Producto activo";
            chkActivo.AutoSize = true;
            chkActivo.Margin = new Padding(3, 3, 3, 8);
            chkActivo.Name = "chkActivo";

            chkPrecioIncluyeITBIS.Text = "Precio incluye ITBIS";
            chkPrecioIncluyeITBIS.AutoSize = true;
            chkPrecioIncluyeITBIS.Margin = new Padding(24, 0, 3, 8);
            chkPrecioIncluyeITBIS.Name = "chkPrecioIncluyeITBIS";
            chkPrecioIncluyeITBIS.Enabled = false;
            chkPrecioIncluyeITBIS.AutoCheck = false;

            chkBloqNegativo.Text = "Bloquear stock negativo";
            chkBloqNegativo.AutoSize = true;
            chkBloqNegativo.Margin = new Padding(3, 0, 3, 3);
            chkBloqNegativo.Name = "chkBloqNegativo";

            // grpBarras
            grpBarras.Text = "Códigos de barra";
            grpBarras.Dock = DockStyle.Fill;
            grpBarras.Padding = new Padding(12);
            grpBarras.Controls.Add(tlBarras);

            // tlBarras
            tlBarras.ColumnCount = 1;
            tlBarras.RowCount = 2;
            tlBarras.Dock = DockStyle.Fill;
            tlBarras.RowStyles.Add(new RowStyle(SizeType.Absolute, 46F));
            tlBarras.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tlBarras.Controls.Add(flBarrasTop, 0, 0);
            tlBarras.Controls.Add(gridBarras, 0, 1);

            // flBarrasTop
            flBarrasTop.Dock = DockStyle.Fill;
            flBarrasTop.FlowDirection = FlowDirection.LeftToRight;
            flBarrasTop.WrapContents = false;
            flBarrasTop.Controls.Add(txtBarraManual);
            flBarrasTop.Controls.Add(btnAgregarBarraManual);
            flBarrasTop.Controls.Add(btnAgregarBarraAuto);
            flBarrasTop.Controls.Add(btnEliminarBarra);

            txtBarraManual.Width = 220;
            txtBarraManual.Name = "txtBarraManual";

            btnAgregarBarraManual.Text = "Agregar manual";
            btnAgregarBarraManual.AutoSize = true;
            btnAgregarBarraManual.Padding = new Padding(8, 4, 8, 4);
            btnAgregarBarraManual.Name = "btnAgregarBarraManual";

            btnAgregarBarraAuto.Text = "Generar auto";
            btnAgregarBarraAuto.AutoSize = true;
            btnAgregarBarraAuto.Padding = new Padding(8, 4, 8, 4);
            btnAgregarBarraAuto.Name = "btnAgregarBarraAuto";

            btnEliminarBarra.Text = "Eliminar";
            btnEliminarBarra.AutoSize = true;
            btnEliminarBarra.Padding = new Padding(8, 4, 8, 4);
            btnEliminarBarra.Name = "btnEliminarBarra";

            // gridBarras
            gridBarras.Dock = DockStyle.Fill;
            gridBarras.AllowUserToAddRows = false;
            gridBarras.AllowUserToDeleteRows = false;
            gridBarras.AllowUserToResizeRows = false;
            gridBarras.ReadOnly = true;
            gridBarras.MultiSelect = false;
            gridBarras.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            gridBarras.AutoGenerateColumns = false;
            gridBarras.RowHeadersVisible = false;
            gridBarras.BackgroundColor = WinColor.White;
            gridBarras.BorderStyle = BorderStyle.FixedSingle;
            gridBarras.Name = "gridBarras";
            gridBarras.Columns.AddRange(new DataGridViewColumn[]
            {
                colBarraCodigo,
                colBarraTipo,
                colBarraUsuario,
                colBarraUltUso
            });

            colBarraCodigo.DataPropertyName = "CodigoBarras";
            colBarraCodigo.HeaderText = "Código";
            colBarraCodigo.Name = "colBarraCodigo";
            colBarraCodigo.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            colBarraCodigo.FillWeight = 35F;

            colBarraTipo.DataPropertyName = "Tipo";
            colBarraTipo.HeaderText = "Tipo";
            colBarraTipo.Name = "colBarraTipo";
            colBarraTipo.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            colBarraTipo.FillWeight = 15F;

            colBarraUsuario.DataPropertyName = "Usuario";
            colBarraUsuario.HeaderText = "Usuario";
            colBarraUsuario.Name = "colBarraUsuario";
            colBarraUsuario.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            colBarraUsuario.FillWeight = 20F;

            colBarraUltUso.DataPropertyName = "UltimaFechaUtilizacion";
            colBarraUltUso.HeaderText = "Último uso";
            colBarraUltUso.Name = "colBarraUltUso";
            colBarraUltUso.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            colBarraUltUso.FillWeight = 30F;

            // panelBottom
            panelBottom.Dock = DockStyle.Fill;
            panelBottom.Controls.Add(flBottom);

            // flBottom
            flBottom.Dock = DockStyle.Fill;
            flBottom.FlowDirection = FlowDirection.LeftToRight;
            flBottom.WrapContents = false;
            flBottom.Padding = new Padding(0, 10, 0, 10);
            flBottom.AutoScroll = false;
            flBottom.Controls.Add(btnAtras);
            flBottom.Controls.Add(btnSiguiente);
            flBottom.Controls.Add(btnGuardar);
            flBottom.Controls.Add(btnFinalizar);

            btnAtras.Text = "Atrás";
            btnAtras.AutoSize = true;
            btnAtras.Padding = new Padding(12, 5, 12, 5);
            btnAtras.Margin = new Padding(0, 0, 10, 0);
            btnAtras.Name = "btnAtras";

            btnSiguiente.Text = "Siguiente";
            btnSiguiente.AutoSize = true;
            btnSiguiente.Padding = new Padding(12, 5, 12, 5);
            btnSiguiente.Margin = new Padding(0, 0, 10, 0);
            btnSiguiente.Name = "btnSiguiente";

            btnGuardar.Text = "Guardar";
            btnGuardar.AutoSize = true;
            btnGuardar.Padding = new Padding(12, 5, 12, 5);
            btnGuardar.Margin = new Padding(0, 0, 10, 0);
            btnGuardar.Name = "btnGuardar";

            btnFinalizar.Text = "Finalizar";
            btnFinalizar.AutoSize = true;
            btnFinalizar.Padding = new Padding(12, 5, 12, 5);
            btnFinalizar.Margin = new Padding(0);
            btnFinalizar.Name = "btnFinalizar";

            // combos
            cboUnidad.DropDownStyle = ComboBoxStyle.DropDownList;
            cboImpuesto.DropDownStyle = ComboBoxStyle.DropDownList;
            cboCategoria.DropDownStyle = ComboBoxStyle.DropDownList;
            cboSubcategoria.DropDownStyle = ComboBoxStyle.DropDownList;
            cboTipoProducto.DropDownStyle = ComboBoxStyle.DropDownList;
            cboTipoProducto.Items.AddRange(new object[] { "Bien", "Servicio" });

            // textboxes
            txtCodigo.Name = "txtCodigo";

            txtDescripcion.Name = "txtDescripcion";
            txtDescripcion.Multiline = true;
            txtDescripcion.ScrollBars = ScrollBars.Vertical;
            txtDescripcion.Dock = DockStyle.Fill;

            txtDescripcionFiscal.Name = "txtDescripcionFiscal";
            txtDescripcionFiscal.Multiline = true;
            txtDescripcionFiscal.ScrollBars = ScrollBars.Vertical;
            txtDescripcionFiscal.Dock = DockStyle.Fill;

            txtReferencia.Name = "txtReferencia";
            txtCodigoItemFiscal.Name = "txtCodigoItemFiscal";
            txtPrecioVenta.Name = "txtPrecioVenta";
            txtPrecioCosto.Name = "txtPrecioCosto";
            txtPrecioMayor.Name = "txtPrecioMayor";
            txtExistencia.Name = "txtExistencia";
            txtUltimoPrecioCompra.Name = "txtUltimoPrecioCompra";
            txtPorcBeneficio.Name = "txtPorcBeneficio";

            // form
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1180, 820);
            Controls.Add(root);
            MinimumSize = new Size(1100, 760);
            Name = "FormProductoEdit";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Producto";

            ((ISupportInitialize)(picImagenProducto)).EndInit();
            ((ISupportInitialize)(gridBarras)).EndInit();
            ResumeLayout(false);
        }
    }
}