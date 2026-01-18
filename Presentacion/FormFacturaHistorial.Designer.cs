using System.Windows.Forms;
using System.Drawing;

namespace Andloe.Presentacion
{
    partial class FormFacturaHistorial
    {
        private System.ComponentModel.IContainer components = null;

        private Panel pnlTop;
        private Label lblTitulo;

        private CheckBox chkRangoFechas;
        private DateTimePicker dtDesde;
        private DateTimePicker dtHasta;

        private ComboBox cboTipo;
        private CheckBox chkSoloAbiertas;
        private CheckBox chkSoloRegistradas;

        private TextBox txtBuscar;
        private Button btnBuscar;

        private DataGridView grid;

        private Panel pnlBottom;
        private Label lblCount;

        private Button btnNuevoDoc;   // ✅ NUEVO
        private Button btnAbrir;
        private Button btnCerrar;

        private DataGridViewTextBoxColumn colFacturaId;
        private DataGridViewTextBoxColumn colTipo;
        private DataGridViewTextBoxColumn colNumero;
        private DataGridViewTextBoxColumn colFecha;
        private DataGridViewTextBoxColumn colEstado;
        private DataGridViewTextBoxColumn colPago;
        private DataGridViewTextBoxColumn colCliente;
        private DataGridViewTextBoxColumn colDoc;
        private DataGridViewTextBoxColumn colTotal;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            pnlTop = new Panel();
            lblTitulo = new Label();
            chkRangoFechas = new CheckBox();
            dtDesde = new DateTimePicker();
            dtHasta = new DateTimePicker();
            cboTipo = new ComboBox();
            chkSoloRegistradas = new CheckBox();
            chkSoloAbiertas = new CheckBox();
            txtBuscar = new TextBox();
            btnBuscar = new Button();
            grid = new DataGridView();
            colFacturaId = new DataGridViewTextBoxColumn();
            colTipo = new DataGridViewTextBoxColumn();
            colNumero = new DataGridViewTextBoxColumn();
            colFecha = new DataGridViewTextBoxColumn();
            colEstado = new DataGridViewTextBoxColumn();
            colPago = new DataGridViewTextBoxColumn();
            colCliente = new DataGridViewTextBoxColumn();
            colDoc = new DataGridViewTextBoxColumn();
            colTotal = new DataGridViewTextBoxColumn();
            pnlBottom = new Panel();
            lblCount = new Label();
            btnNuevoDoc = new Button();
            btnAbrir = new Button();
            btnCerrar = new Button();
            btnAnular = new Button();
            pnlTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)grid).BeginInit();
            pnlBottom.SuspendLayout();
            SuspendLayout();
            // 
            // pnlTop
            // 
            pnlTop.BackColor = Color.White;
            pnlTop.Controls.Add(lblTitulo);
            pnlTop.Controls.Add(chkRangoFechas);
            pnlTop.Controls.Add(dtDesde);
            pnlTop.Controls.Add(dtHasta);
            pnlTop.Controls.Add(cboTipo);
            pnlTop.Controls.Add(chkSoloAbiertas);
            pnlTop.Controls.Add(chkSoloRegistradas);
            pnlTop.Controls.Add(txtBuscar);
            pnlTop.Controls.Add(btnBuscar);
            pnlTop.Dock = DockStyle.Top;
            pnlTop.Location = new Point(0, 0);
            pnlTop.Name = "pnlTop";
            pnlTop.Padding = new Padding(14);
            pnlTop.Size = new Size(1120, 90);
            pnlTop.TabIndex = 2;
            // 
            // lblTitulo
            // 
            lblTitulo.AutoSize = true;
            lblTitulo.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblTitulo.Location = new Point(14, 12);
            lblTitulo.Name = "lblTitulo";
            lblTitulo.Size = new Size(197, 21);
            lblTitulo.TabIndex = 0;
            lblTitulo.Text = "Historial de documentos";
            // 
            // chkRangoFechas
            // 
            chkRangoFechas.AutoSize = true;
            chkRangoFechas.Checked = true;
            chkRangoFechas.CheckState = CheckState.Checked;
            chkRangoFechas.Location = new Point(16, 48);
            chkRangoFechas.Name = "chkRangoFechas";
            chkRangoFechas.Size = new Size(97, 19);
            chkRangoFechas.TabIndex = 1;
            chkRangoFechas.Text = "Rango fechas";
            // 
            // dtDesde
            // 
            dtDesde.Format = DateTimePickerFormat.Short;
            dtDesde.Location = new Point(120, 45);
            dtDesde.Name = "dtDesde";
            dtDesde.Size = new Size(110, 23);
            dtDesde.TabIndex = 2;
            // 
            // dtHasta
            // 
            dtHasta.Format = DateTimePickerFormat.Short;
            dtHasta.Location = new Point(240, 45);
            dtHasta.Name = "dtHasta";
            dtHasta.Size = new Size(110, 23);
            dtHasta.TabIndex = 3;
            // 
            // cboTipo
            // 
            cboTipo.DropDownStyle = ComboBoxStyle.DropDownList;
            cboTipo.Location = new Point(365, 45);
            cboTipo.Name = "cboTipo";
            cboTipo.Size = new Size(180, 23);
            cboTipo.TabIndex = 4;
            // 
            // chkSoloAbiertas
            // 
            chkSoloAbiertas.AutoSize = true;
            chkSoloAbiertas.Location = new Point(555, 48);
            chkSoloAbiertas.Name = "chkSoloAbiertas";
            chkSoloAbiertas.Size = new Size(166, 19);
            chkSoloAbiertas.TabIndex = 5;
            chkSoloAbiertas.Text = "Solo abiertas (BORRADOR)";

            chkSoloRegistradas.AutoSize = true;
            chkSoloRegistradas.Text = "Solo registradas (FINALIZADA)";
            chkSoloRegistradas.Location = new Point(555, 68);
            // 
            // txtBuscar
            // 
            txtBuscar.Location = new Point(745, 45);
            txtBuscar.Name = "txtBuscar";
            txtBuscar.PlaceholderText = "Número / Cliente / Documento";
            txtBuscar.Size = new Size(240, 23);
            txtBuscar.TabIndex = 6;
            // 
            // btnBuscar
            // 
            btnBuscar.BackColor = Color.FromArgb(33, 150, 243);
            btnBuscar.ForeColor = Color.White;
            btnBuscar.Location = new Point(995, 44);
            btnBuscar.Name = "btnBuscar";
            btnBuscar.Size = new Size(90, 28);
            btnBuscar.TabIndex = 7;
            btnBuscar.Text = "Buscar";
            btnBuscar.UseVisualStyleBackColor = false;
            // 
            // grid
            // 
            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.BackgroundColor = Color.White;
            grid.BorderStyle = BorderStyle.None;
            grid.Columns.AddRange(new DataGridViewColumn[] { colFacturaId, colTipo, colNumero, colFecha, colEstado, colPago, colCliente, colDoc, colTotal });
            grid.Dock = DockStyle.Fill;
            grid.Location = new Point(0, 90);
            grid.MultiSelect = false;
            grid.Name = "grid";
            grid.ReadOnly = true;
            grid.RowHeadersVisible = false;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.Size = new Size(1120, 538);
            grid.TabIndex = 0;
            // 
            // colFacturaId
            // 
            colFacturaId.HeaderText = "Id";
            colFacturaId.Name = "colFacturaId";
            colFacturaId.ReadOnly = true;
            colFacturaId.Width = 55;
            // 
            // colTipo
            // 
            colTipo.HeaderText = "Tipo";
            colTipo.Name = "colTipo";
            colTipo.ReadOnly = true;
            colTipo.Width = 60;
            // 
            // colNumero
            // 
            colNumero.HeaderText = "Número";
            colNumero.Name = "colNumero";
            colNumero.ReadOnly = true;
            colNumero.Width = 120;
            // 
            // colFecha
            // 
            colFecha.HeaderText = "Fecha";
            colFecha.Name = "colFecha";
            colFecha.ReadOnly = true;
            colFecha.Width = 95;
            // 
            // colEstado
            // 
            colEstado.HeaderText = "Estado";
            colEstado.Name = "colEstado";
            colEstado.ReadOnly = true;
            colEstado.Width = 95;
            // 
            // colPago
            // 
            colPago.HeaderText = "Pago";
            colPago.Name = "colPago";
            colPago.ReadOnly = true;
            colPago.Width = 85;
            // 
            // colCliente
            // 
            colCliente.HeaderText = "Cliente";
            colCliente.Name = "colCliente";
            colCliente.ReadOnly = true;
            colCliente.Width = 300;
            // 
            // colDoc
            // 
            colDoc.HeaderText = "Documento";
            colDoc.Name = "colDoc";
            colDoc.ReadOnly = true;
            colDoc.Width = 120;
            // 
            // colTotal
            // 
            colTotal.HeaderText = "Total";
            colTotal.Name = "colTotal";
            colTotal.ReadOnly = true;
            colTotal.Width = 110;
            // 
            // pnlBottom
            // 
            pnlBottom.BackColor = Color.White;
            pnlBottom.Controls.Add(btnAnular);
            pnlBottom.Controls.Add(lblCount);
            pnlBottom.Controls.Add(btnNuevoDoc);
            pnlBottom.Controls.Add(btnAbrir);
            pnlBottom.Controls.Add(btnCerrar);
            pnlBottom.Dock = DockStyle.Bottom;
            pnlBottom.Location = new Point(0, 628);
            pnlBottom.Name = "pnlBottom";
            pnlBottom.Padding = new Padding(14);
            pnlBottom.Size = new Size(1120, 52);
            pnlBottom.TabIndex = 1;
            // 
            // lblCount
            // 
            lblCount.AutoSize = true;
            lblCount.Location = new Point(16, 18);
            lblCount.Name = "lblCount";
            lblCount.Size = new Size(91, 15);
            lblCount.TabIndex = 0;
            lblCount.Text = "0 documento(s)";
            // 
            // btnNuevoDoc
            // 
            btnNuevoDoc.BackColor = Color.FromArgb(33, 150, 243);
            btnNuevoDoc.ForeColor = Color.White;
            btnNuevoDoc.Location = new Point(598, 12);
            btnNuevoDoc.Name = "btnNuevoDoc";
            btnNuevoDoc.Size = new Size(130, 30);
            btnNuevoDoc.TabIndex = 1;
            btnNuevoDoc.Text = "Nuevo Doc";
            btnNuevoDoc.UseVisualStyleBackColor = false;
            // 
            // btnAbrir
            // 
            btnAbrir.BackColor = Color.FromArgb(76, 175, 80);
            btnAbrir.ForeColor = Color.White;
            btnAbrir.Location = new Point(870, 12);
            btnAbrir.Name = "btnAbrir";
            btnAbrir.Size = new Size(110, 30);
            btnAbrir.TabIndex = 2;
            btnAbrir.Text = "Abrir";
            btnAbrir.UseVisualStyleBackColor = false;
            // 
            // btnCerrar
            // 
            btnCerrar.BackColor = Color.FromArgb(158, 158, 158);
            btnCerrar.ForeColor = Color.White;
            btnCerrar.Location = new Point(990, 12);
            btnCerrar.Name = "btnCerrar";
            btnCerrar.Size = new Size(110, 30);
            btnCerrar.TabIndex = 3;
            btnCerrar.Text = "Cerrar";
            btnCerrar.UseVisualStyleBackColor = false;
            // 
            // btnAnular
            // 
            btnAnular.BackColor = Color.FromArgb(33, 150, 243);
            btnAnular.ForeColor = Color.White;
            btnAnular.Location = new Point(734, 12);
            btnAnular.Name = "btnAnular";
            btnAnular.Size = new Size(130, 30);
            btnAnular.TabIndex = 4;
            btnAnular.Text = "Anular";
            btnAnular.UseVisualStyleBackColor = false;
            // 
            // FormFacturaHistorial
            // 
            ClientSize = new Size(1120, 680);
            Controls.Add(grid);
            Controls.Add(pnlBottom);
            Controls.Add(pnlTop);
            Name = "FormFacturaHistorial";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Historial - Facturación";
            pnlTop.ResumeLayout(false);
            pnlTop.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)grid).EndInit();
            pnlBottom.ResumeLayout(false);
            pnlBottom.PerformLayout();
            ResumeLayout(false);
        }
        private Button btnAnular;
    }
}
