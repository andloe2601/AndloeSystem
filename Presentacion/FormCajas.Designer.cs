namespace Andloe.Presentacion
{
    partial class FormCajas
    {
        private System.ComponentModel.IContainer components = null;

        private DataGridView gridCajas;
        private ComboBox cbSucursalFiltro;
        private Label lblSucursalFiltro;
        private TextBox txtCajaId;
        private Label lblCajaId;
        private ComboBox cbSucursal;
        private Label lblSucursal;
        private TextBox txtCajaNumero;
        private Label lblCajaNumero;
        private TextBox txtDescripcion;
        private Label lblDescripcion;
        private ComboBox cbEstado;
        private Label lblEstado;
        private Button btnNuevo;
        private Button btnGuardar;
        private Button btnEliminar;
        private Button btnCancelar;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();

            gridCajas = new DataGridView();
            cbSucursalFiltro = new ComboBox();
            lblSucursalFiltro = new Label();
            txtCajaId = new TextBox();
            lblCajaId = new Label();
            cbSucursal = new ComboBox();
            lblSucursal = new Label();
            txtCajaNumero = new TextBox();
            lblCajaNumero = new Label();
            txtDescripcion = new TextBox();
            lblDescripcion = new Label();
            cbEstado = new ComboBox();
            lblEstado = new Label();
            btnNuevo = new Button();
            btnGuardar = new Button();
            btnEliminar = new Button();
            btnCancelar = new Button();

            ((System.ComponentModel.ISupportInitialize)gridCajas).BeginInit();
            SuspendLayout();

            // gridCajas
            gridCajas.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            gridCajas.Location = new System.Drawing.Point(12, 60);
            gridCajas.Name = "gridCajas";
            gridCajas.Size = new System.Drawing.Size(550, 350);
            gridCajas.ReadOnly = true;
            gridCajas.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            gridCajas.MultiSelect = false;
            gridCajas.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            gridCajas.RowHeadersVisible = false;
            gridCajas.CellClick += gridCajas_CellClick;

            // lblSucursalFiltro
            lblSucursalFiltro.AutoSize = true;
            lblSucursalFiltro.Location = new System.Drawing.Point(12, 15);
            lblSucursalFiltro.Name = "lblSucursalFiltro";
            lblSucursalFiltro.Size = new System.Drawing.Size(52, 15);
            lblSucursalFiltro.Text = "Sucursal:";

            // cbSucursalFiltro
            cbSucursalFiltro.DropDownStyle = ComboBoxStyle.DropDownList;
            cbSucursalFiltro.Location = new System.Drawing.Point(70, 12);
            cbSucursalFiltro.Name = "cbSucursalFiltro";
            cbSucursalFiltro.Size = new System.Drawing.Size(200, 23);
            cbSucursalFiltro.SelectedIndexChanged += cbSucursalFiltro_SelectedIndexChanged;

            // lblCajaId
            lblCajaId.AutoSize = true;
            lblCajaId.Location = new System.Drawing.Point(580, 20);
            lblCajaId.Name = "lblCajaId";
            lblCajaId.Size = new System.Drawing.Size(44, 15);
            lblCajaId.Text = "Caja Id:";

            // txtCajaId
            txtCajaId.Location = new System.Drawing.Point(650, 17);
            txtCajaId.Name = "txtCajaId";
            txtCajaId.ReadOnly = true;
            txtCajaId.Size = new System.Drawing.Size(80, 23);

            // lblSucursal
            lblSucursal.AutoSize = true;
            lblSucursal.Location = new System.Drawing.Point(580, 55);
            lblSucursal.Name = "lblSucursal";
            lblSucursal.Size = new System.Drawing.Size(52, 15);
            lblSucursal.Text = "Sucursal:";

            // cbSucursal
            cbSucursal.DropDownStyle = ComboBoxStyle.DropDownList;
            cbSucursal.Location = new System.Drawing.Point(650, 52);
            cbSucursal.Name = "cbSucursal";
            cbSucursal.Size = new System.Drawing.Size(200, 23);

            // lblCajaNumero
            lblCajaNumero.AutoSize = true;
            lblCajaNumero.Location = new System.Drawing.Point(580, 90);
            lblCajaNumero.Name = "lblCajaNumero";
            lblCajaNumero.Size = new System.Drawing.Size(63, 15);
            lblCajaNumero.Text = "Nro. Caja:";

            // txtCajaNumero
            txtCajaNumero.Location = new System.Drawing.Point(650, 87);
            txtCajaNumero.Name = "txtCajaNumero";
            txtCajaNumero.Size = new System.Drawing.Size(120, 23);

            // lblDescripcion
            lblDescripcion.AutoSize = true;
            lblDescripcion.Location = new System.Drawing.Point(580, 125);
            lblDescripcion.Name = "lblDescripcion";
            lblDescripcion.Size = new System.Drawing.Size(69, 15);
            lblDescripcion.Text = "Descripción:";

            // txtDescripcion
            txtDescripcion.Location = new System.Drawing.Point(650, 122);
            txtDescripcion.Name = "txtDescripcion";
            txtDescripcion.Size = new System.Drawing.Size(250, 23);

            // lblEstado
            lblEstado.AutoSize = true;
            lblEstado.Location = new System.Drawing.Point(580, 160);
            lblEstado.Name = "lblEstado";
            lblEstado.Size = new System.Drawing.Size(45, 15);
            lblEstado.Text = "Estado:";

            // cbEstado
            cbEstado.DropDownStyle = ComboBoxStyle.DropDownList;
            cbEstado.Location = new System.Drawing.Point(650, 157);
            cbEstado.Name = "cbEstado";
            cbEstado.Size = new System.Drawing.Size(120, 23);

            // btnNuevo
            btnNuevo.Location = new System.Drawing.Point(580, 210);
            btnNuevo.Name = "btnNuevo";
            btnNuevo.Size = new System.Drawing.Size(80, 30);
            btnNuevo.Text = "Nuevo";
            btnNuevo.Click += btnNuevo_Click;

            // btnGuardar
            btnGuardar.Location = new System.Drawing.Point(670, 210);
            btnGuardar.Name = "btnGuardar";
            btnGuardar.Size = new System.Drawing.Size(80, 30);
            btnGuardar.Text = "Guardar";
            btnGuardar.Click += btnGuardar_Click;

            // btnEliminar
            btnEliminar.Location = new System.Drawing.Point(760, 210);
            btnEliminar.Name = "btnEliminar";
            btnEliminar.Size = new System.Drawing.Size(80, 30);
            btnEliminar.Text = "Eliminar";
            btnEliminar.Click += btnEliminar_Click;

            // btnCancelar
            btnCancelar.Location = new System.Drawing.Point(850, 210);
            btnCancelar.Name = "btnCancelar";
            btnCancelar.Size = new System.Drawing.Size(80, 30);
            btnCancelar.Text = "Cancelar";
            btnCancelar.Click += btnCancelar_Click;

            // FormCajas
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(950, 430);
            Controls.Add(gridCajas);
            Controls.Add(lblSucursalFiltro);
            Controls.Add(cbSucursalFiltro);
            Controls.Add(lblCajaId);
            Controls.Add(txtCajaId);
            Controls.Add(lblSucursal);
            Controls.Add(cbSucursal);
            Controls.Add(lblCajaNumero);
            Controls.Add(txtCajaNumero);
            Controls.Add(lblDescripcion);
            Controls.Add(txtDescripcion);
            Controls.Add(lblEstado);
            Controls.Add(cbEstado);
            Controls.Add(btnNuevo);
            Controls.Add(btnGuardar);
            Controls.Add(btnEliminar);
            Controls.Add(btnCancelar);
            Name = "FormCajas";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Mantenimiento de Cajas";
            Load += FormCajas_Load;

            ((System.ComponentModel.ISupportInitialize)gridCajas).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
