using System.Drawing;
using System.Windows.Forms;

namespace Andloe.Presentacion
{
    partial class FormCxCCobroEdit
    {
        private System.ComponentModel.IContainer components = null;

        private ComboBox cboCliente;
        private TextBox txtBuscarCliente;
        private Button btnBuscarCliente;

        private Label lblCuenta;
        private ComboBox cboCuenta;
        private DateTimePicker dtpFecha;

        private ComboBox cboFormaPago;
        private ComboBox cboCentroCosto;
        private ComboBox cboTipoIngreso;
        private NumericUpDown nudMontoPago;

        private Label lblReferencia;
        private TextBox txtReferencia;

        private Label lblNumeroCheque;
        private TextBox txtNumeroCheque;

        private TextBox txtObservacion;

        private DataGridView gridFacturas;
        private Button btnAutoAplicar;
        private Button btnGuardar;
        private Button btnGuardarBorrador;
        private Label lblTotalValor;
        private Label lblEstadoValidacion;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
                components.Dispose();

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();

            cboCliente = new ComboBox();
            txtBuscarCliente = new TextBox();
            btnBuscarCliente = new Button();

            lblCuenta = new Label();
            cboCuenta = new ComboBox();
            dtpFecha = new DateTimePicker();

            cboFormaPago = new ComboBox();
            cboCentroCosto = new ComboBox();
            cboTipoIngreso = new ComboBox();
            nudMontoPago = new NumericUpDown();

            lblReferencia = new Label();
            txtReferencia = new TextBox();

            lblNumeroCheque = new Label();
            txtNumeroCheque = new TextBox();

            txtObservacion = new TextBox();

            gridFacturas = new DataGridView();
            btnAutoAplicar = new Button();
            btnGuardar = new Button();
            btnGuardarBorrador = new Button();
            lblTotalValor = new Label();
            lblEstadoValidacion = new Label();

            ((System.ComponentModel.ISupportInitialize)nudMontoPago).BeginInit();
            ((System.ComponentModel.ISupportInitialize)gridFacturas).BeginInit();
            SuspendLayout();

            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            ClientSize = new Size(1120, 720);
            Name = "FormCxCCobroEdit";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Recibo de pago";

            Controls.Add(new Label
            {
                Text = "Cliente",
                Location = new Point(16, 18),
                AutoSize = true,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            });

            txtBuscarCliente.Location = new Point(16, 42);
            txtBuscarCliente.Size = new Size(190, 23);

            btnBuscarCliente.Location = new Point(210, 41);
            btnBuscarCliente.Size = new Size(72, 25);
            btnBuscarCliente.Text = "Buscar";

            cboCliente.Location = new Point(16, 74);
            cboCliente.Size = new Size(266, 23);
            cboCliente.DropDownStyle = ComboBoxStyle.DropDownList;

            lblCuenta.Text = "Cuenta bancaria *";
            lblCuenta.Location = new Point(320, 18);
            lblCuenta.AutoSize = true;
            lblCuenta.Font = new Font("Segoe UI", 9F, FontStyle.Bold);

            cboCuenta.Location = new Point(320, 42);
            cboCuenta.Size = new Size(260, 23);
            cboCuenta.DropDownStyle = ComboBoxStyle.DropDownList;

            Controls.Add(new Label
            {
                Text = "Fecha de pago *",
                Location = new Point(610, 18),
                AutoSize = true,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            });

            dtpFecha.Location = new Point(610, 42);
            dtpFecha.Size = new Size(170, 23);
            dtpFecha.Format = DateTimePickerFormat.Short;

            Controls.Add(new Label
            {
                Text = "Forma de pago *",
                Location = new Point(16, 110),
                AutoSize = true,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            });

            cboFormaPago.Location = new Point(16, 134);
            cboFormaPago.Size = new Size(200, 23);
            cboFormaPago.DropDownStyle = ComboBoxStyle.DropDownList;

            Controls.Add(new Label
            {
                Text = "Centro de costo",
                Location = new Point(250, 110),
                AutoSize = true,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            });

            cboCentroCosto.Location = new Point(250, 134);
            cboCentroCosto.Size = new Size(250, 23);
            cboCentroCosto.DropDownStyle = ComboBoxStyle.DropDownList;

            Controls.Add(new Label
            {
                Text = "Monto del pago",
                Location = new Point(530, 110),
                AutoSize = true,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            });

            nudMontoPago.Location = new Point(530, 134);
            nudMontoPago.Size = new Size(130, 23);
            nudMontoPago.DecimalPlaces = 2;
            nudMontoPago.Maximum = 999999999;

            Controls.Add(new Label
            {
                Text = "Tipo de ingreso",
                Location = new Point(16, 176),
                AutoSize = true,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            });

            cboTipoIngreso.Location = new Point(16, 200);
            cboTipoIngreso.Size = new Size(300, 23);
            cboTipoIngreso.DropDownStyle = ComboBoxStyle.DropDownList;

            lblReferencia.Text = "Referencia";
            lblReferencia.Location = new Point(340, 176);
            lblReferencia.AutoSize = true;
            lblReferencia.Font = new Font("Segoe UI", 9F, FontStyle.Bold);

            txtReferencia.Location = new Point(340, 200);
            txtReferencia.Size = new Size(210, 23);

            lblNumeroCheque.Text = "Número de cheque *";
            lblNumeroCheque.Location = new Point(340, 176);
            lblNumeroCheque.AutoSize = true;
            lblNumeroCheque.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblNumeroCheque.Visible = false;

            txtNumeroCheque.Location = new Point(340, 200);
            txtNumeroCheque.Size = new Size(210, 23);
            txtNumeroCheque.Visible = false;

            Controls.Add(new Label
            {
                Text = "Observación",
                Location = new Point(570, 176),
                AutoSize = true,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            });

            txtObservacion.Location = new Point(570, 200);
            txtObservacion.Size = new Size(280, 23);

            btnAutoAplicar.Location = new Point(868, 198);
            btnAutoAplicar.Size = new Size(120, 27);
            btnAutoAplicar.Text = "Aplicar automático";

            Controls.Add(new Label
            {
                Text = "Facturas por cobrar",
                Location = new Point(16, 246),
                AutoSize = true,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold)
            });

            Controls.Add(new Label
            {
                Text = "Agrega el monto recibido a las facturas relacionadas con este ingreso.",
                Location = new Point(16, 270),
                AutoSize = true
            });

            gridFacturas.Location = new Point(16, 302);
            gridFacturas.Size = new Size(1088, 260);
            gridFacturas.AllowUserToAddRows = false;
            gridFacturas.AllowUserToDeleteRows = false;
            gridFacturas.RowHeadersVisible = false;
            gridFacturas.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            gridFacturas.SelectionMode = DataGridViewSelectionMode.CellSelect;
            gridFacturas.MultiSelect = false;
            gridFacturas.BackgroundColor = Color.White;
            gridFacturas.BorderStyle = BorderStyle.FixedSingle;

            gridFacturas.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colNumero",
                HeaderText = "Número",
                FillWeight = 18,
                ReadOnly = true
            });

            gridFacturas.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colVencimiento",
                HeaderText = "Vencimiento",
                FillWeight = 14,
                ReadOnly = true
            });

            gridFacturas.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colDiasVencidos",
                HeaderText = "Días vencidos",
                FillWeight = 12,
                ReadOnly = true
            });

            gridFacturas.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colTotal",
                HeaderText = "Total",
                FillWeight = 13,
                ReadOnly = true
            });

            gridFacturas.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colRet",
                HeaderText = "Retenciones",
                FillWeight = 13
            });

            gridFacturas.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colPend",
                HeaderText = "Por cobrar",
                FillWeight = 13,
                ReadOnly = true
            });

            gridFacturas.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colRec",
                HeaderText = "Monto recibido",
                FillWeight = 14
            });

            gridFacturas.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colSaldoLuego",
                HeaderText = "Saldo luego",
                FillWeight = 14,
                ReadOnly = true
            });

            gridFacturas.Columns.Add(new DataGridViewButtonColumn
            {
                Name = "colAplicar",
                HeaderText = "Acción",
                FillWeight = 12,
                Text = "Aplicar todo",
                UseColumnTextForButtonValue = true
            });

            lblEstadoValidacion.Location = new Point(16, 575);
            lblEstadoValidacion.Size = new Size(760, 22);
            lblEstadoValidacion.Text = "";
            lblEstadoValidacion.Font = new Font("Segoe UI", 9F, FontStyle.Bold);

            Controls.Add(new Label
            {
                Text = "Total",
                Location = new Point(920, 582),
                AutoSize = true,
                Font = new Font("Segoe UI", 16F, FontStyle.Bold)
            });

            lblTotalValor.Location = new Point(988, 582);
            lblTotalValor.Size = new Size(116, 30);
            lblTotalValor.Text = "RD$ 0.00";
            lblTotalValor.Font = new Font("Segoe UI", 16F, FontStyle.Bold);

            btnGuardarBorrador.Location = new Point(794, 632);
            btnGuardarBorrador.Size = new Size(150, 34);
            btnGuardarBorrador.Text = "Guardar borrador";

            btnGuardar.Location = new Point(954, 632);
            btnGuardar.Size = new Size(150, 34);
            btnGuardar.Text = "Guardar y postear";
            btnGuardar.BackColor = Color.FromArgb(33, 179, 164);
            btnGuardar.ForeColor = Color.White;
            btnGuardar.FlatStyle = FlatStyle.Flat;
            btnGuardar.FlatAppearance.BorderSize = 0;

            Controls.Add(cboCliente);
            Controls.Add(txtBuscarCliente);
            Controls.Add(btnBuscarCliente);
            Controls.Add(lblCuenta);
            Controls.Add(cboCuenta);
            Controls.Add(dtpFecha);
            Controls.Add(cboFormaPago);
            Controls.Add(cboCentroCosto);
            Controls.Add(cboTipoIngreso);
            Controls.Add(nudMontoPago);
            Controls.Add(lblReferencia);
            Controls.Add(txtReferencia);
            Controls.Add(lblNumeroCheque);
            Controls.Add(txtNumeroCheque);
            Controls.Add(txtObservacion);
            Controls.Add(gridFacturas);
            Controls.Add(btnAutoAplicar);
            Controls.Add(lblTotalValor);
            Controls.Add(lblEstadoValidacion);
            Controls.Add(btnGuardar);
            Controls.Add(btnGuardarBorrador);

            ((System.ComponentModel.ISupportInitialize)nudMontoPago).EndInit();
            ((System.ComponentModel.ISupportInitialize)gridFacturas).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }
    }
}