using System.Drawing;
using System.Windows.Forms;

namespace Andloe.Presentation
{
    partial class FormCierresHistorico
    {
        private System.ComponentModel.IContainer components = null;

        private Label lblTitulo;
        private Label lblDesde;
        private Label lblHasta;
        private DateTimePicker dtDesde;
        private DateTimePicker dtHasta;
        private Label lblCaja;
        private TextBox txtCajaNumero;
        private Label lblUsuario;
        private TextBox txtUsuario;
        private Label lblEstado;
        private ComboBox cboEstado;
        private Button btnBuscar;
        private Button btnCerrar;
        private DataGridView dgvCierres;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            lblTitulo = new Label();
            lblDesde = new Label();
            lblHasta = new Label();
            dtDesde = new DateTimePicker();
            dtHasta = new DateTimePicker();
            lblCaja = new Label();
            txtCajaNumero = new TextBox();
            lblUsuario = new Label();
            txtUsuario = new TextBox();
            lblEstado = new Label();
            cboEstado = new ComboBox();
            btnBuscar = new Button();
            btnCerrar = new Button();
            dgvCierres = new DataGridView();
            ((System.ComponentModel.ISupportInitialize)dgvCierres).BeginInit();
            SuspendLayout();
            // 
            // lblTitulo
            // 
            lblTitulo.AutoSize = true;
            lblTitulo.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            lblTitulo.Location = new Point(20, 15);
            lblTitulo.Name = "lblTitulo";
            lblTitulo.Size = new Size(255, 25);
            lblTitulo.TabIndex = 0;
            lblTitulo.Text = "Histórico de Cierres de Caja";
            // 
            // lblDesde
            // 
            lblDesde.AutoSize = true;
            lblDesde.Location = new Point(20, 55);
            lblDesde.Name = "lblDesde";
            lblDesde.Size = new Size(42, 15);
            lblDesde.TabIndex = 1;
            lblDesde.Text = "Desde:";
            // 
            // lblHasta
            // 
            lblHasta.AutoSize = true;
            lblHasta.Location = new Point(200, 55);
            lblHasta.Name = "lblHasta";
            lblHasta.Size = new Size(40, 15);
            lblHasta.TabIndex = 3;
            lblHasta.Text = "Hasta:";
            // 
            // dtDesde
            // 
            dtDesde.CustomFormat = "dd/MM/yyyy";
            dtDesde.Format = DateTimePickerFormat.Custom;
            dtDesde.Location = new Point(70, 51);
            dtDesde.Name = "dtDesde";
            dtDesde.ShowCheckBox = true;
            dtDesde.Size = new Size(110, 23);
            dtDesde.TabIndex = 2;
            // 
            // dtHasta
            // 
            dtHasta.CustomFormat = "dd/MM/yyyy";
            dtHasta.Format = DateTimePickerFormat.Custom;
            dtHasta.Location = new Point(250, 51);
            dtHasta.Name = "dtHasta";
            dtHasta.ShowCheckBox = true;
            dtHasta.Size = new Size(110, 23);
            dtHasta.TabIndex = 4;
            // 
            // lblCaja
            // 
            lblCaja.AutoSize = true;
            lblCaja.Location = new Point(380, 55);
            lblCaja.Name = "lblCaja";
            lblCaja.Size = new Size(33, 15);
            lblCaja.TabIndex = 5;
            lblCaja.Text = "Caja:";
            // 
            // txtCajaNumero
            // 
            txtCajaNumero.Location = new Point(420, 51);
            txtCajaNumero.Name = "txtCajaNumero";
            txtCajaNumero.Size = new Size(80, 23);
            txtCajaNumero.TabIndex = 6;
            // 
            // lblUsuario
            // 
            lblUsuario.AutoSize = true;
            lblUsuario.Location = new Point(520, 55);
            lblUsuario.Name = "lblUsuario";
            lblUsuario.Size = new Size(50, 15);
            lblUsuario.TabIndex = 7;
            lblUsuario.Text = "Usuario:";
            // 
            // txtUsuario
            // 
            txtUsuario.Location = new Point(580, 51);
            txtUsuario.Name = "txtUsuario";
            txtUsuario.Size = new Size(120, 23);
            txtUsuario.TabIndex = 8;
            // 
            // lblEstado
            // 
            lblEstado.AutoSize = true;
            lblEstado.Location = new Point(720, 55);
            lblEstado.Name = "lblEstado";
            lblEstado.Size = new Size(45, 15);
            lblEstado.TabIndex = 9;
            lblEstado.Text = "Estado:";
            // 
            // cboEstado
            // 
            cboEstado.DropDownStyle = ComboBoxStyle.DropDownList;
            cboEstado.Location = new Point(775, 51);
            cboEstado.Name = "cboEstado";
            cboEstado.Size = new Size(100, 23);
            cboEstado.TabIndex = 10;
            // 
            // btnBuscar
            // 
            btnBuscar.Location = new Point(890, 48);
            btnBuscar.Name = "btnBuscar";
            btnBuscar.Size = new Size(80, 26);
            btnBuscar.TabIndex = 11;
            btnBuscar.Text = "Buscar";
            btnBuscar.Click += btnBuscar_Click;
            // 
            // btnCerrar
            // 
            btnCerrar.Location = new Point(890, 12);
            btnCerrar.Name = "btnCerrar";
            btnCerrar.Size = new Size(80, 26);
            btnCerrar.TabIndex = 13;
            btnCerrar.Text = "Cerrar";
            btnCerrar.Click += btnCerrar_Click;
            // 
            // dgvCierres
            // 
            dgvCierres.AllowUserToAddRows = false;
            dgvCierres.AllowUserToDeleteRows = false;
            dgvCierres.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgvCierres.Location = new Point(20, 90);
            dgvCierres.MultiSelect = false;
            dgvCierres.Name = "dgvCierres";
            dgvCierres.ReadOnly = true;
            dgvCierres.RowHeadersVisible = false;
            dgvCierres.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvCierres.Size = new Size(950, 370);
            dgvCierres.TabIndex = 12;
            dgvCierres.CellDoubleClick += dgvCierres_CellDoubleClick;
            // 
            // FormCierresHistorico
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.WhiteSmoke;
            ClientSize = new Size(1000, 520);
            Controls.Add(lblTitulo);
            Controls.Add(lblDesde);
            Controls.Add(dtDesde);
            Controls.Add(lblHasta);
            Controls.Add(dtHasta);
            Controls.Add(lblCaja);
            Controls.Add(txtCajaNumero);
            Controls.Add(lblUsuario);
            Controls.Add(txtUsuario);
            Controls.Add(lblEstado);
            Controls.Add(cboEstado);
            Controls.Add(btnBuscar);
            Controls.Add(dgvCierres);
            Controls.Add(btnCerrar);
            Name = "FormCierresHistorico";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Histórico de Cierres";
            Load += FormCierresHistorico_Load;
            ((System.ComponentModel.ISupportInitialize)dgvCierres).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
    }
}
