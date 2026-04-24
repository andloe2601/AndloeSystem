using System.Windows.Forms;

namespace Andloe.Presentacion
{
    partial class FormPromoProductoDescuento
    {
        private System.ComponentModel.IContainer components = null;

        private Label lblCodigoPromo;
        private TextBox txtCodigoPromo;
        private Label lblNombrePromo;
        private TextBox txtNombrePromo;
        private Label lblProducto;
        private TextBox txtProductoCodigo;
        private Button btnBuscarProducto;
        private Button btnAgregarLinea;

        private Label lblTipo;
        private ComboBox cmbTipo;
        private Label lblDescuentoPct;
        private NumericUpDown nudDescuentoPct;
        private Label lblPrecioPromo;
        private NumericUpDown nudPrecioPromo;
        private Label lblCantPack;
        private NumericUpDown nudCantPack;

        // 🔹 NUEVO: mensaje de ayuda
        private Label lblAyudaTipo;

        private Label lblDesde;
        private DateTimePicker dtpDesde;
        private Label lblHasta;
        private DateTimePicker dtpHasta;
        private CheckBox chkActiva;

        private GroupBox grpDias;
        private CheckBox chkTodosDias;
        private CheckBox chkLunes;
        private CheckBox chkMartes;
        private CheckBox chkMiercoles;
        private CheckBox chkJueves;
        private CheckBox chkViernes;
        private CheckBox chkSabado;
        private CheckBox chkDomingo;

        private DataGridView dgvLineas;
        private DataGridViewTextBoxColumn colProdCodigo;
        private DataGridViewTextBoxColumn colProdNombre;
        private DataGridViewTextBoxColumn colPrecioVenta;
        private DataGridViewTextBoxColumn colCosto;
        private DataGridViewTextBoxColumn colDescPct;

        private Button btnBorrarLinea;
        private Button btnGuardar;
        private Button btnCancelar;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.lblCodigoPromo = new Label();
            this.txtCodigoPromo = new TextBox();
            this.lblNombrePromo = new Label();
            this.txtNombrePromo = new TextBox();
            this.lblProducto = new Label();
            this.txtProductoCodigo = new TextBox();
            this.btnBuscarProducto = new Button();
            this.btnAgregarLinea = new Button();

            this.lblTipo = new Label();
            this.cmbTipo = new ComboBox();
            this.lblDescuentoPct = new Label();
            this.nudDescuentoPct = new NumericUpDown();
            this.lblPrecioPromo = new Label();
            this.nudPrecioPromo = new NumericUpDown();
            this.lblCantPack = new Label();
            this.nudCantPack = new NumericUpDown();

            // 🔹 NUEVO
            this.lblAyudaTipo = new Label();

            this.lblDesde = new Label();
            this.dtpDesde = new DateTimePicker();
            this.lblHasta = new Label();
            this.dtpHasta = new DateTimePicker();
            this.chkActiva = new CheckBox();

            this.grpDias = new GroupBox();
            this.chkTodosDias = new CheckBox();
            this.chkLunes = new CheckBox();
            this.chkMartes = new CheckBox();
            this.chkMiercoles = new CheckBox();
            this.chkJueves = new CheckBox();
            this.chkViernes = new CheckBox();
            this.chkSabado = new CheckBox();
            this.chkDomingo = new CheckBox();

            this.dgvLineas = new DataGridView();
            this.colProdCodigo = new DataGridViewTextBoxColumn();
            this.colProdNombre = new DataGridViewTextBoxColumn();
            this.colPrecioVenta = new DataGridViewTextBoxColumn();
            this.colCosto = new DataGridViewTextBoxColumn();
            this.colDescPct = new DataGridViewTextBoxColumn();

            this.btnBorrarLinea = new Button();
            this.btnGuardar = new Button();
            this.btnCancelar = new Button();

            ((System.ComponentModel.ISupportInitialize)(this.nudDescuentoPct)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudPrecioPromo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCantPack)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLineas)).BeginInit();
            this.grpDias.SuspendLayout();
            this.SuspendLayout();

            // lblCodigoPromo
            this.lblCodigoPromo.AutoSize = true;
            this.lblCodigoPromo.Location = new System.Drawing.Point(20, 20);
            this.lblCodigoPromo.Name = "lblCodigoPromo";
            this.lblCodigoPromo.Size = new System.Drawing.Size(46, 15);
            this.lblCodigoPromo.Text = "Código:";

            // txtCodigoPromo
            this.txtCodigoPromo.Location = new System.Drawing.Point(90, 17);
            this.txtCodigoPromo.MaxLength = 20;
            this.txtCodigoPromo.Name = "txtCodigoPromo";
            this.txtCodigoPromo.ReadOnly = true;
            this.txtCodigoPromo.Size = new System.Drawing.Size(150, 23);

            // lblNombrePromo
            this.lblNombrePromo.AutoSize = true;
            this.lblNombrePromo.Location = new System.Drawing.Point(20, 55);
            this.lblNombrePromo.Name = "lblNombrePromo";
            this.lblNombrePromo.Size = new System.Drawing.Size(54, 15);
            this.lblNombrePromo.Text = "Nombre:";

            // txtNombrePromo
            this.txtNombrePromo.Location = new System.Drawing.Point(90, 52);
            this.txtNombrePromo.MaxLength = 100;
            this.txtNombrePromo.Name = "txtNombrePromo";
            this.txtNombrePromo.Size = new System.Drawing.Size(550, 23);

            // lblProducto
            this.lblProducto.AutoSize = true;
            this.lblProducto.Location = new System.Drawing.Point(20, 90);
            this.lblProducto.Name = "lblProducto";
            this.lblProducto.Size = new System.Drawing.Size(60, 15);
            this.lblProducto.Text = "Producto:";

            // txtProductoCodigo
            this.txtProductoCodigo.Location = new System.Drawing.Point(90, 87);
            this.txtProductoCodigo.MaxLength = 20;
            this.txtProductoCodigo.Name = "txtProductoCodigo";
            this.txtProductoCodigo.Size = new System.Drawing.Size(150, 23);

            // btnBuscarProducto
            this.btnBuscarProducto.Location = new System.Drawing.Point(250, 86);
            this.btnBuscarProducto.Name = "btnBuscarProducto";
            this.btnBuscarProducto.Size = new System.Drawing.Size(80, 25);
            this.btnBuscarProducto.Text = "Buscar...";
            this.btnBuscarProducto.UseVisualStyleBackColor = true;
            this.btnBuscarProducto.Click += new System.EventHandler(this.btnBuscarProducto_Click);

            // btnAgregarLinea
            this.btnAgregarLinea.Location = new System.Drawing.Point(336, 86);
            this.btnAgregarLinea.Name = "btnAgregarLinea";
            this.btnAgregarLinea.Size = new System.Drawing.Size(114, 25);
            this.btnAgregarLinea.Text = "Agregar línea";
            this.btnAgregarLinea.UseVisualStyleBackColor = true;
            this.btnAgregarLinea.Click += new System.EventHandler(this.btnAgregarLinea_Click);

            // lblTipo
            this.lblTipo.AutoSize = true;
            this.lblTipo.Location = new System.Drawing.Point(20, 125);
            this.lblTipo.Name = "lblTipo";
            this.lblTipo.Size = new System.Drawing.Size(33, 15);
            this.lblTipo.Text = "Tipo:";

            // cmbTipo
            this.cmbTipo.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbTipo.Items.AddRange(new object[] {
                "Descuento %",
                "Precio fijo",
                "Pack (N por $)"});
            this.cmbTipo.Location = new System.Drawing.Point(90, 122);
            this.cmbTipo.Name = "cmbTipo";
            this.cmbTipo.Size = new System.Drawing.Size(120, 23);
            this.cmbTipo.SelectedIndex = 0;

            // lblDescuentoPct
            this.lblDescuentoPct.AutoSize = true;
            this.lblDescuentoPct.Location = new System.Drawing.Point(220, 125);
            this.lblDescuentoPct.Name = "lblDescuentoPct";
            this.lblDescuentoPct.Size = new System.Drawing.Size(57, 15);
            this.lblDescuentoPct.Text = "% Desc.:";

            // nudDescuentoPct
            this.nudDescuentoPct.DecimalPlaces = 2;
            this.nudDescuentoPct.Location = new System.Drawing.Point(283, 123);
            this.nudDescuentoPct.Maximum = 100;
            this.nudDescuentoPct.Name = "nudDescuentoPct";
            this.nudDescuentoPct.Size = new System.Drawing.Size(80, 23);

            // lblPrecioPromo
            this.lblPrecioPromo.AutoSize = true;
            this.lblPrecioPromo.Location = new System.Drawing.Point(380, 125);
            this.lblPrecioPromo.Name = "lblPrecioPromo";
            this.lblPrecioPromo.Size = new System.Drawing.Size(104, 15);
            this.lblPrecioPromo.Text = "Precio promo/pack:";

            // nudPrecioPromo
            this.nudPrecioPromo.DecimalPlaces = 2;
            this.nudPrecioPromo.Maximum = 1000000;
            this.nudPrecioPromo.Location = new System.Drawing.Point(490, 123);
            this.nudPrecioPromo.Name = "nudPrecioPromo";
            this.nudPrecioPromo.Size = new System.Drawing.Size(90, 23);

            // lblCantPack
            this.lblCantPack.AutoSize = true;
            this.lblCantPack.Location = new System.Drawing.Point(585, 125);
            this.lblCantPack.Name = "lblCantPack";
            this.lblCantPack.Size = new System.Drawing.Size(64, 15);
            this.lblCantPack.Text = "Cant. pack:";

            // nudCantPack
            this.nudCantPack.DecimalPlaces = 0;
            this.nudCantPack.Minimum = 1;
            this.nudCantPack.Maximum = 999;
            this.nudCantPack.Location = new System.Drawing.Point(655, 123);
            this.nudCantPack.Name = "nudCantPack";
            this.nudCantPack.Size = new System.Drawing.Size(60, 23);
            this.nudCantPack.Value = 3;

            // 🔹 lblAyudaTipo
            this.lblAyudaTipo.AutoSize = true;
            this.lblAyudaTipo.Location = new System.Drawing.Point(20, 155);
            this.lblAyudaTipo.Name = "lblAyudaTipo";
            this.lblAyudaTipo.Size = new System.Drawing.Size(0, 15);
            this.lblAyudaTipo.Text = "";

            // lblDesde
            this.lblDesde.AutoSize = true;
            this.lblDesde.Location = new System.Drawing.Point(20, 185);
            this.lblDesde.Name = "lblDesde";
            this.lblDesde.Size = new System.Drawing.Size(43, 15);
            this.lblDesde.Text = "Desde:";

            // dtpDesde
            this.dtpDesde.Format = DateTimePickerFormat.Short;
            this.dtpDesde.Location = new System.Drawing.Point(69, 183);
            this.dtpDesde.Name = "dtpDesde";
            this.dtpDesde.Size = new System.Drawing.Size(102, 23);

            // lblHasta
            this.lblHasta.AutoSize = true;
            this.lblHasta.Location = new System.Drawing.Point(180, 185);
            this.lblHasta.Name = "lblHasta";
            this.lblHasta.Size = new System.Drawing.Size(41, 15);
            this.lblHasta.Text = "Hasta:";

            // dtpHasta
            this.dtpHasta.Format = DateTimePickerFormat.Short;
            this.dtpHasta.Location = new System.Drawing.Point(227, 183);
            this.dtpHasta.Name = "dtpHasta";
            this.dtpHasta.Size = new System.Drawing.Size(102, 23);

            // chkActiva
            this.chkActiva.AutoSize = true;
            this.chkActiva.Checked = true;
            this.chkActiva.CheckState = CheckState.Checked;
            this.chkActiva.Location = new System.Drawing.Point(350, 185);
            this.chkActiva.Name = "chkActiva";
            this.chkActiva.Size = new System.Drawing.Size(58, 19);
            this.chkActiva.Text = "Activa";
            this.chkActiva.UseVisualStyleBackColor = true;

            // grpDias
            this.grpDias.Controls.Add(this.chkTodosDias);
            this.grpDias.Controls.Add(this.chkLunes);
            this.grpDias.Controls.Add(this.chkMartes);
            this.grpDias.Controls.Add(this.chkMiercoles);
            this.grpDias.Controls.Add(this.chkJueves);
            this.grpDias.Controls.Add(this.chkViernes);
            this.grpDias.Controls.Add(this.chkSabado);
            this.grpDias.Controls.Add(this.chkDomingo);
            this.grpDias.Location = new System.Drawing.Point(430, 175);
            this.grpDias.Name = "grpDias";
            this.grpDias.Size = new System.Drawing.Size(340, 55);
            this.grpDias.Text = "Días";

            // chkTodosDias
            this.chkTodosDias.AutoSize = true;
            this.chkTodosDias.Location = new System.Drawing.Point(10, 22);
            this.chkTodosDias.Name = "chkTodosDias";
            this.chkTodosDias.Size = new System.Drawing.Size(57, 19);
            this.chkTodosDias.Text = "Todos";
            this.chkTodosDias.Checked = true;
            this.chkTodosDias.CheckedChanged += new System.EventHandler(this.chkTodosDias_CheckedChanged);

            // chkLunes..Domingo
            int x = 75;
            string[] etiquetas = { "L", "Ma", "Mi", "J", "V", "S", "D" };
            CheckBox[] chks = { chkLunes, chkMartes, chkMiercoles, chkJueves, chkViernes, chkSabado, chkDomingo };

            for (int i = 0; i < chks.Length; i++)
            {
                chks[i].AutoSize = true;
                chks[i].Location = new System.Drawing.Point(x, 22);
                chks[i].Name = "chk" + etiquetas[i];
                chks[i].Text = etiquetas[i];
                chks[i].Checked = true;
                this.grpDias.Controls.Add(chks[i]);
                x += 35;
            }

            // dgvLineas
            this.dgvLineas.AllowUserToAddRows = false;
            this.dgvLineas.AllowUserToDeleteRows = false;
            this.dgvLineas.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this.dgvLineas.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvLineas.Columns.AddRange(new DataGridViewColumn[] {
                this.colProdCodigo,
                this.colProdNombre,
                this.colPrecioVenta,
                this.colCosto,
                this.colDescPct});
            this.dgvLineas.Location = new System.Drawing.Point(20, 240);
            this.dgvLineas.MultiSelect = false;
            this.dgvLineas.Name = "dgvLineas";
            this.dgvLineas.ReadOnly = true;
            this.dgvLineas.RowHeadersVisible = false;
            this.dgvLineas.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dgvLineas.Size = new System.Drawing.Size(750, 260);

            // colProdCodigo
            this.colProdCodigo.HeaderText = "Código";
            this.colProdCodigo.Name = "colProdCodigo";
            this.colProdCodigo.ReadOnly = true;
            this.colProdCodigo.Width = 90;

            // colProdNombre
            this.colProdNombre.HeaderText = "Producto";
            this.colProdNombre.Name = "colProdNombre";
            this.colProdNombre.ReadOnly = true;
            this.colProdNombre.Width = 260;

            // colPrecioVenta
            this.colPrecioVenta.HeaderText = "Precio venta";
            this.colPrecioVenta.Name = "colPrecioVenta";
            this.colPrecioVenta.ReadOnly = true;
            this.colPrecioVenta.Width = 90;

            // colCosto
            this.colCosto.HeaderText = "Costo";
            this.colCosto.Name = "colCosto";
            this.colCosto.ReadOnly = true;
            this.colCosto.Width = 90;

            // colDescPct
            this.colDescPct.HeaderText = "% Desc.";
            this.colDescPct.Name = "colDescPct";
            this.colDescPct.ReadOnly = true;
            this.colDescPct.Width = 70;

            // btnBorrarLinea
            this.btnBorrarLinea.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            this.btnBorrarLinea.Location = new System.Drawing.Point(20, 510);
            this.btnBorrarLinea.Name = "btnBorrarLinea";
            this.btnBorrarLinea.Size = new System.Drawing.Size(100, 28);
            this.btnBorrarLinea.Text = "Borrar línea";
            this.btnBorrarLinea.UseVisualStyleBackColor = true;
            this.btnBorrarLinea.Click += new System.EventHandler(this.btnEliminarLinea_Click);

            // btnGuardar
            this.btnGuardar.Anchor = AnchorStyles.Bottom;
            this.btnGuardar.Location = new System.Drawing.Point(300, 510);
            this.btnGuardar.Name = "btnGuardar";
            this.btnGuardar.Size = new System.Drawing.Size(90, 28);
            this.btnGuardar.Text = "Guardar";
            this.btnGuardar.UseVisualStyleBackColor = true;
            this.btnGuardar.Click += new System.EventHandler(this.btnGuardar_Click);

            // btnCancelar
            this.btnCancelar.Anchor = AnchorStyles.Bottom;
            this.btnCancelar.Location = new System.Drawing.Point(410, 510);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(90, 28);
            this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.UseVisualStyleBackColor = true;
            this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);

            // FormPromoProductoDescuento
            this.ClientSize = new System.Drawing.Size(800, 550);
            this.Controls.Add(this.btnCancelar);
            this.Controls.Add(this.btnGuardar);
            this.Controls.Add(this.btnBorrarLinea);
            this.Controls.Add(this.dgvLineas);
            this.Controls.Add(this.grpDias);
            this.Controls.Add(this.chkActiva);
            this.Controls.Add(this.dtpHasta);
            this.Controls.Add(this.lblHasta);
            this.Controls.Add(this.dtpDesde);
            this.Controls.Add(this.lblDesde);
            this.Controls.Add(this.lblAyudaTipo);
            this.Controls.Add(this.nudCantPack);
            this.Controls.Add(this.lblCantPack);
            this.Controls.Add(this.nudPrecioPromo);
            this.Controls.Add(this.lblPrecioPromo);
            this.Controls.Add(this.nudDescuentoPct);
            this.Controls.Add(this.lblDescuentoPct);
            this.Controls.Add(this.cmbTipo);
            this.Controls.Add(this.lblTipo);
            this.Controls.Add(this.btnAgregarLinea);
            this.Controls.Add(this.btnBuscarProducto);
            this.Controls.Add(this.txtProductoCodigo);
            this.Controls.Add(this.lblProducto);
            this.Controls.Add(this.txtNombrePromo);
            this.Controls.Add(this.lblNombrePromo);
            this.Controls.Add(this.txtCodigoPromo);
            this.Controls.Add(this.lblCodigoPromo);
            this.Name = "FormPromoProductoDescuento";
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Promo descuento / pack por producto";

            ((System.ComponentModel.ISupportInitialize)(this.nudDescuentoPct)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudPrecioPromo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCantPack)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLineas)).EndInit();
            this.grpDias.ResumeLayout(false);
            this.grpDias.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
