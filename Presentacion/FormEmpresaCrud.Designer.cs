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
            if (disposing && (components != null)) components.Dispose();
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

            dgvEmpresas.Location = new Point(12, 12);
            dgvEmpresas.Size = new Size(520, 520);
            dgvEmpresas.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvEmpresas.MultiSelect = false;
            dgvEmpresas.ReadOnly = true;
            dgvEmpresas.AllowUserToAddRows = false;
            dgvEmpresas.AllowUserToDeleteRows = false;
            dgvEmpresas.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvEmpresas.RowHeadersVisible = false;
            dgvEmpresas.Columns.Add(new DataGridViewTextBoxColumn { Name = "colEmpresaId", DataPropertyName = "EmpresaId", HeaderText = "Id", Width = 60 });
            dgvEmpresas.Columns.Add(new DataGridViewTextBoxColumn { Name = "colRazonSocial", DataPropertyName = "RazonSocial", HeaderText = "Razón Social" });
            dgvEmpresas.Columns.Add(new DataGridViewTextBoxColumn { Name = "colRNC", DataPropertyName = "RNC", HeaderText = "RNC" });
            dgvEmpresas.Columns.Add(new DataGridViewTextBoxColumn { Name = "colEstado", DataPropertyName = "Estado", HeaderText = "Estado", Width = 80 });

            int x1 = 560, y = 20, w = 300, h = 23, gap = 34;

            lblEmpresaId.Text = "Id:";
            lblEmpresaId.Location = new Point(x1, y);
            txtEmpresaId.Location = new Point(x1 + 110, y - 3);
            txtEmpresaId.Size = new Size(80, h);
            txtEmpresaId.ReadOnly = true;

            y += gap;
            lblRazonSocial.Text = "Razón Social:";
            lblRazonSocial.Location = new Point(x1, y);
            txtRazonSocial.Location = new Point(x1 + 110, y - 3);
            txtRazonSocial.Size = new Size(w, h);

            y += gap;
            lblRNC.Text = "RNC:";
            lblRNC.Location = new Point(x1, y);
            txtRNC.Location = new Point(x1 + 110, y - 3);
            txtRNC.Size = new Size(180, h);

            y += gap;
            lblMonedaBase.Text = "Moneda:";
            lblMonedaBase.Location = new Point(x1, y);
            txtMonedaBase.Location = new Point(x1 + 110, y - 3);
            txtMonedaBase.Size = new Size(80, h);

            y += gap;
            lblPais.Text = "País:";
            lblPais.Location = new Point(x1, y);
            txtPais.Location = new Point(x1 + 110, y - 3);
            txtPais.Size = new Size(80, h);

            y += gap;
            lblProvincia.Text = "Provincia:";
            lblProvincia.Location = new Point(x1, y);
            cbProvincia.Location = new Point(x1 + 110, y - 3);
            cbProvincia.Size = new Size(w, h);
            cbProvincia.DropDownStyle = ComboBoxStyle.DropDownList;

            y += gap;
            lblMunicipio.Text = "Municipio:";
            lblMunicipio.Location = new Point(x1, y);
            cbMunicipio.Location = new Point(x1 + 110, y - 3);
            cbMunicipio.Size = new Size(w, h);
            cbMunicipio.DropDownStyle = ComboBoxStyle.DropDownList;

            y += gap;
            lblDireccion.Text = "Dirección:";
            lblDireccion.Location = new Point(x1, y);
            txtDireccion.Location = new Point(x1 + 110, y - 3);
            txtDireccion.Size = new Size(w, h);

            y += gap;
            lblTelefono.Text = "Teléfono:";
            lblTelefono.Location = new Point(x1, y);
            txtTelefono.Location = new Point(x1 + 110, y - 3);
            txtTelefono.Size = new Size(180, h);

            y += gap;
            lblEmail.Text = "Email:";
            lblEmail.Location = new Point(x1, y);
            txtEmail.Location = new Point(x1 + 110, y - 3);
            txtEmail.Size = new Size(w, h);

            y += gap;
            chkEstado.Text = "Activo";
            chkEstado.Location = new Point(x1 + 110, y - 2);

            y += gap;
            lblLogo.Text = "Logo:";
            lblLogo.Location = new Point(x1, y);
            picLogo.Location = new Point(x1 + 110, y);
            picLogo.Size = new Size(180, 120);
            picLogo.BorderStyle = BorderStyle.FixedSingle;
            picLogo.SizeMode = PictureBoxSizeMode.Zoom;

            btnCargarLogo.Text = "Cargar";
            btnCargarLogo.Location = new Point(x1 + 300, y);
            btnCargarLogo.Size = new Size(80, 28);

            btnQuitarLogo.Text = "Quitar";
            btnQuitarLogo.Location = new Point(x1 + 300, y + 36);
            btnQuitarLogo.Size = new Size(80, 28);

            btnNuevo.Text = "Nuevo";
            btnNuevo.Location = new Point(560, 490);
            btnNuevo.Size = new Size(80, 32);

            btnGuardar.Text = "Guardar";
            btnGuardar.Location = new Point(650, 490);
            btnGuardar.Size = new Size(80, 32);

            btnEliminar.Text = "Eliminar";
            btnEliminar.Location = new Point(740, 490);
            btnEliminar.Size = new Size(80, 32);

            btnCerrar.Text = "Cerrar";
            btnCerrar.Location = new Point(830, 490);
            btnCerrar.Size = new Size(80, 32);

            ClientSize = new Size(930, 550);
            Controls.AddRange(new Control[]
            {
                dgvEmpresas,
                lblEmpresaId, txtEmpresaId,
                lblRazonSocial, txtRazonSocial,
                lblRNC, txtRNC,
                lblMonedaBase, txtMonedaBase,
                lblPais, txtPais,
                lblProvincia, cbProvincia,
                lblMunicipio, cbMunicipio,
                lblDireccion, txtDireccion,
                lblTelefono, txtTelefono,
                lblEmail, txtEmail,
                chkEstado,
                lblLogo, picLogo, btnCargarLogo, btnQuitarLogo,
                btnNuevo, btnGuardar, btnEliminar, btnCerrar
            });

            StartPosition = FormStartPosition.CenterScreen;
            Text = "Empresas";
            ((System.ComponentModel.ISupportInitialize)dgvEmpresas).EndInit();
            ((System.ComponentModel.ISupportInitialize)picLogo).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }
    }
}