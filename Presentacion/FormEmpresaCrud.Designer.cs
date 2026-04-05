#nullable disable
using System.Drawing;
using System.Windows.Forms;

namespace Andloe.Presentacion
{
    partial class FormEmpresaCrud
    {
        private System.ComponentModel.IContainer components = null;

        private DataGridView dgvEmpresas;
        private TextBox txtEmpresaId;
        private TextBox txtRazonSocial;
        private TextBox txtRNC;
        private TextBox txtMonedaBase;
        private TextBox txtPais;
        private ComboBox cbProvincia;
        private ComboBox cbMunicipio;
        private TextBox txtDireccion;
        private TextBox txtTelefono;
        private TextBox txtEmail;
        private CheckBox chkEstado;
        private PictureBox picLogo;
        private Button btnCargarLogo;
        private Button btnQuitarLogo;
        private Button btnNuevo;
        private Button btnGuardar;
        private Button btnEliminar;
        private Button btnCerrar;

        private Label lblEmpresaId;
        private Label lblRazonSocial;
        private Label lblRNC;
        private Label lblMonedaBase;
        private Label lblPais;
        private Label lblProvincia;
        private Label lblMunicipio;
        private Label lblDireccion;
        private Label lblTelefono;
        private Label lblEmail;
        private Label lblLogo;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            dgvEmpresas = new DataGridView();
            txtEmpresaId = new TextBox();
            txtRazonSocial = new TextBox();
            txtRNC = new TextBox();
            txtMonedaBase = new TextBox();
            txtPais = new TextBox();
            cbProvincia = new ComboBox();
            cbMunicipio = new ComboBox();
            txtDireccion = new TextBox();
            txtTelefono = new TextBox();
            txtEmail = new TextBox();
            chkEstado = new CheckBox();
            picLogo = new PictureBox();
            btnCargarLogo = new Button();
            btnQuitarLogo = new Button();
            btnNuevo = new Button();
            btnGuardar = new Button();
            btnEliminar = new Button();
            btnCerrar = new Button();

            lblEmpresaId = new Label();
            lblRazonSocial = new Label();
            lblRNC = new Label();
            lblMonedaBase = new Label();
            lblPais = new Label();
            lblProvincia = new Label();
            lblMunicipio = new Label();
            lblDireccion = new Label();
            lblTelefono = new Label();
            lblEmail = new Label();
            lblLogo = new Label();

            ((System.ComponentModel.ISupportInitialize)dgvEmpresas).BeginInit();
            ((System.ComponentModel.ISupportInitialize)picLogo).BeginInit();
            SuspendLayout();

            // dgvEmpresas
            dgvEmpresas.AllowUserToAddRows = false;
            dgvEmpresas.AllowUserToDeleteRows = false;
            dgvEmpresas.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            dgvEmpresas.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvEmpresas.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvEmpresas.Location = new Point(12, 12);
            dgvEmpresas.MultiSelect = false;
            dgvEmpresas.Name = "dgvEmpresas";
            dgvEmpresas.ReadOnly = true;
            dgvEmpresas.RowHeadersVisible = false;
            dgvEmpresas.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvEmpresas.Size = new Size(520, 526);
            dgvEmpresas.TabIndex = 0;

            dgvEmpresas.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colEmpresaId",
                DataPropertyName = "EmpresaId",
                HeaderText = "Id",
                Width = 60
            });

            dgvEmpresas.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colRazonSocial",
                DataPropertyName = "RazonSocial",
                HeaderText = "Razón Social"
            });

            dgvEmpresas.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colRNC",
                DataPropertyName = "RNC",
                HeaderText = "RNC"
            });

            dgvEmpresas.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colEstado",
                DataPropertyName = "Estado",
                HeaderText = "Estado",
                Width = 80
            });

            int x = 555;
            int y = 18;
            int labelW = 95;
            int txtX = x + 105;
            int w = 250;
            int h = 23;
            int gap = 34;

            // lblEmpresaId
            lblEmpresaId.AutoSize = true;
            lblEmpresaId.Location = new Point(x, y + 3);
            lblEmpresaId.Name = "lblEmpresaId";
            lblEmpresaId.Size = new Size(20, 15);
            lblEmpresaId.Text = "Id:";

            txtEmpresaId.Location = new Point(txtX, y);
            txtEmpresaId.Name = "txtEmpresaId";
            txtEmpresaId.ReadOnly = true;
            txtEmpresaId.Size = new Size(80, h);

            y += gap;

            lblRazonSocial.AutoSize = true;
            lblRazonSocial.Location = new Point(x, y + 3);
            lblRazonSocial.Text = "Razón Social:";
            txtRazonSocial.Location = new Point(txtX, y);
            txtRazonSocial.Size = new Size(w, h);

            y += gap;

            lblRNC.AutoSize = true;
            lblRNC.Location = new Point(x, y + 3);
            lblRNC.Text = "RNC:";
            txtRNC.Location = new Point(txtX, y);
            txtRNC.Size = new Size(160, h);

            y += gap;

            lblMonedaBase.AutoSize = true;
            lblMonedaBase.Location = new Point(x, y + 3);
            lblMonedaBase.Text = "Moneda:";
            txtMonedaBase.Location = new Point(txtX, y);
            txtMonedaBase.Size = new Size(70, h);

            y += gap;

            lblPais.AutoSize = true;
            lblPais.Location = new Point(x, y + 3);
            lblPais.Text = "País:";
            txtPais.Location = new Point(txtX, y);
            txtPais.Size = new Size(70, h);

            y += gap;

            lblProvincia.AutoSize = true;
            lblProvincia.Location = new Point(x, y + 3);
            lblProvincia.Text = "Provincia:";
            cbProvincia.DropDownStyle = ComboBoxStyle.DropDownList;
            cbProvincia.Location = new Point(txtX, y);
            cbProvincia.Size = new Size(w, h);

            y += gap;

            lblMunicipio.AutoSize = true;
            lblMunicipio.Location = new Point(x, y + 3);
            lblMunicipio.Text = "Municipio:";
            cbMunicipio.DropDownStyle = ComboBoxStyle.DropDownList;
            cbMunicipio.Location = new Point(txtX, y);
            cbMunicipio.Size = new Size(w, h);

            y += gap;

            lblDireccion.AutoSize = true;
            lblDireccion.Location = new Point(x, y + 3);
            lblDireccion.Text = "Dirección:";
            txtDireccion.Location = new Point(txtX, y);
            txtDireccion.Size = new Size(320, h);

            y += gap;

            lblTelefono.AutoSize = true;
            lblTelefono.Location = new Point(x, y + 3);
            lblTelefono.Text = "Teléfono:";
            txtTelefono.Location = new Point(txtX, y);
            txtTelefono.Size = new Size(180, h);

            y += gap;

            lblEmail.AutoSize = true;
            lblEmail.Location = new Point(x, y + 3);
            lblEmail.Text = "Email:";
            txtEmail.Location = new Point(txtX, y);
            txtEmail.Size = new Size(320, h);

            y += gap;

            chkEstado.AutoSize = true;
            chkEstado.Location = new Point(txtX, y + 3);
            chkEstado.Text = "Activo";

            y += gap;

            lblLogo.AutoSize = true;
            lblLogo.Location = new Point(x, y + 3);
            lblLogo.Text = "Logo:";

            picLogo.BorderStyle = BorderStyle.FixedSingle;
            picLogo.Location = new Point(txtX, y);
            picLogo.Name = "picLogo";
            picLogo.Size = new Size(180, 120);
            picLogo.SizeMode = PictureBoxSizeMode.Zoom;

            btnCargarLogo.Location = new Point(txtX + 190, y);
            btnCargarLogo.Name = "btnCargarLogo";
            btnCargarLogo.Size = new Size(90, 30);
            btnCargarLogo.Text = "Cargar";

            btnQuitarLogo.Location = new Point(txtX + 190, y + 40);
            btnQuitarLogo.Name = "btnQuitarLogo";
            btnQuitarLogo.Size = new Size(90, 30);
            btnQuitarLogo.Text = "Quitar";

            btnNuevo.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnNuevo.Location = new Point(555, 506);
            btnNuevo.Name = "btnNuevo";
            btnNuevo.Size = new Size(85, 32);
            btnNuevo.Text = "Nuevo";

            btnGuardar.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnGuardar.Location = new Point(650, 506);
            btnGuardar.Name = "btnGuardar";
            btnGuardar.Size = new Size(85, 32);
            btnGuardar.Text = "Guardar";

            btnEliminar.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnEliminar.Location = new Point(745, 506);
            btnEliminar.Name = "btnEliminar";
            btnEliminar.Size = new Size(85, 32);
            btnEliminar.Text = "Eliminar";

            btnCerrar.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnCerrar.Location = new Point(840, 506);
            btnCerrar.Name = "btnCerrar";
            btnCerrar.Size = new Size(85, 32);
            btnCerrar.Text = "Cerrar";

            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(940, 550);
            Controls.Add(dgvEmpresas);
            Controls.Add(lblEmpresaId);
            Controls.Add(txtEmpresaId);
            Controls.Add(lblRazonSocial);
            Controls.Add(txtRazonSocial);
            Controls.Add(lblRNC);
            Controls.Add(txtRNC);
            Controls.Add(lblMonedaBase);
            Controls.Add(txtMonedaBase);
            Controls.Add(lblPais);
            Controls.Add(txtPais);
            Controls.Add(lblProvincia);
            Controls.Add(cbProvincia);
            Controls.Add(lblMunicipio);
            Controls.Add(cbMunicipio);
            Controls.Add(lblDireccion);
            Controls.Add(txtDireccion);
            Controls.Add(lblTelefono);
            Controls.Add(txtTelefono);
            Controls.Add(lblEmail);
            Controls.Add(txtEmail);
            Controls.Add(chkEstado);
            Controls.Add(lblLogo);
            Controls.Add(picLogo);
            Controls.Add(btnCargarLogo);
            Controls.Add(btnQuitarLogo);
            Controls.Add(btnNuevo);
            Controls.Add(btnGuardar);
            Controls.Add(btnEliminar);
            Controls.Add(btnCerrar);
            MinimumSize = new Size(956, 589);
            Name = "FormEmpresaCrud";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Empresas";

            ((System.ComponentModel.ISupportInitialize)dgvEmpresas).EndInit();
            ((System.ComponentModel.ISupportInitialize)picLogo).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
#nullable restore