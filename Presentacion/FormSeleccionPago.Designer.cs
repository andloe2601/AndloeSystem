using System.Windows.Forms;

namespace Presentation
{
    partial class FormSeleccionPago
    {
        private System.ComponentModel.IContainer components = null;

        private Label label1;
        private Label lblTotalBase;

        private DataGridView gridMonedas;

        private Label label2;
        private ComboBox cbMoneda;

        private Label label7;
        private ComboBox cbMedioPago;

        private Label label3;
        private TextBox txtMontoMoneda;

        private Button btnNum0;
        private Button btnNum1;
        private Button btnNum2;
        private Button btnNum3;
        private Button btnNum4;
        private Button btnNum5;
        private Button btnNum6;
        private Button btnNum7;
        private Button btnNum8;
        private Button btnNum9;
        private Button btnBorrarMonto;

        private Button btnAgregarLinea;
        private Button btnQuitarLinea;

        private DataGridView gridPagos;

        private Label label4;
        private Label lblPagadoBase;
        private Label label5;
        private Label lblPendienteBase;

        private Button btnAceptar;
        private Button btnCancelar;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();

            label1 = new Label();
            lblTotalBase = new Label();

            gridMonedas = new DataGridView();

            label2 = new Label();
            cbMoneda = new ComboBox();

            label7 = new Label();
            cbMedioPago = new ComboBox();

            label3 = new Label();
            txtMontoMoneda = new TextBox();

            btnNum0 = new Button();
            btnNum1 = new Button();
            btnNum2 = new Button();
            btnNum3 = new Button();
            btnNum4 = new Button();
            btnNum5 = new Button();
            btnNum6 = new Button();
            btnNum7 = new Button();
            btnNum8 = new Button();
            btnNum9 = new Button();
            btnBorrarMonto = new Button();

            btnAgregarLinea = new Button();
            btnQuitarLinea = new Button();

            gridPagos = new DataGridView();

            label4 = new Label();
            lblPagadoBase = new Label();
            label5 = new Label();
            lblPendienteBase = new Label();

            btnAceptar = new Button();
            btnCancelar = new Button();

            ((System.ComponentModel.ISupportInitialize)gridMonedas).BeginInit();
            ((System.ComponentModel.ISupportInitialize)gridPagos).BeginInit();
            SuspendLayout();

            // Form
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(700, 520);
            StartPosition = FormStartPosition.CenterParent;
            Text = "Seleccionar pagos";
            Load += FormSeleccionPago_Load;

            // label1 - Total base
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(12, 9);
            label1.Text = "Total (base):";

            // lblTotalBase
            lblTotalBase.AutoSize = true;
            lblTotalBase.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            lblTotalBase.Location = new System.Drawing.Point(100, 7);
            lblTotalBase.Text = "0.00 DOP";

            // gridMonedas
            gridMonedas.Location = new System.Drawing.Point(12, 32);
            gridMonedas.Name = "gridMonedas";
            gridMonedas.ReadOnly = true;
            gridMonedas.AllowUserToAddRows = false;
            gridMonedas.AllowUserToDeleteRows = false;
            gridMonedas.RowHeadersVisible = false;
            gridMonedas.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            gridMonedas.Size = new System.Drawing.Size(670, 120);
            gridMonedas.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            // label2 - Moneda
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(12, 165);
            label2.Text = "Moneda:";

            // cbMoneda
            cbMoneda.DropDownStyle = ComboBoxStyle.DropDownList;
            cbMoneda.Location = new System.Drawing.Point(70, 161);
            cbMoneda.Name = "cbMoneda";
            cbMoneda.Size = new System.Drawing.Size(120, 23);
            cbMoneda.SelectedIndexChanged += cbMoneda_SelectedIndexChanged;

            // label7 - Medio pago
            label7.AutoSize = true;
            label7.Location = new System.Drawing.Point(210, 165);
            label7.Text = "Medio pago:";

            // cbMedioPago
            cbMedioPago.DropDownStyle = ComboBoxStyle.DropDownList;
            cbMedioPago.Location = new System.Drawing.Point(285, 161);
            cbMedioPago.Name = "cbMedioPago";
            cbMedioPago.Size = new System.Drawing.Size(160, 23);

            // label3 - Monto
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(12, 198);
            label3.Text = "Monto:";

            // txtMontoMoneda
            txtMontoMoneda.Location = new System.Drawing.Point(70, 194);
            txtMontoMoneda.Name = "txtMontoMoneda";
            txtMontoMoneda.Size = new System.Drawing.Size(160, 23);

            // Keypad numérico
            int baseX = 250;
            int baseY = 194;
            int w = 45;
            int h = 35;
            int sep = 5;

            // btnNum7
            btnNum7.Location = new System.Drawing.Point(baseX, baseY);
            btnNum7.Size = new System.Drawing.Size(w, h);
            btnNum7.Text = "7";
            btnNum7.Tag = "7";
            btnNum7.Click += btnNum_Click;

            // btnNum8
            btnNum8.Location = new System.Drawing.Point(baseX + (w + sep), baseY);
            btnNum8.Size = new System.Drawing.Size(w, h);
            btnNum8.Text = "8";
            btnNum8.Tag = "8";
            btnNum8.Click += btnNum_Click;

            // btnNum9
            btnNum9.Location = new System.Drawing.Point(baseX + 2 * (w + sep), baseY);
            btnNum9.Size = new System.Drawing.Size(w, h);
            btnNum9.Text = "9";
            btnNum9.Tag = "9";
            btnNum9.Click += btnNum_Click;

            // fila 2: 4,5,6
            btnNum4.Location = new System.Drawing.Point(baseX, baseY + h + sep);
            btnNum4.Size = new System.Drawing.Size(w, h);
            btnNum4.Text = "4";
            btnNum4.Tag = "4";
            btnNum4.Click += btnNum_Click;

            btnNum5.Location = new System.Drawing.Point(baseX + (w + sep), baseY + h + sep);
            btnNum5.Size = new System.Drawing.Size(w, h);
            btnNum5.Text = "5";
            btnNum5.Tag = "5";
            btnNum5.Click += btnNum_Click;

            btnNum6.Location = new System.Drawing.Point(baseX + 2 * (w + sep), baseY + h + sep);
            btnNum6.Size = new System.Drawing.Size(w, h);
            btnNum6.Text = "6";
            btnNum6.Tag = "6";
            btnNum6.Click += btnNum_Click;

            // fila 3: 1,2,3
            btnNum1.Location = new System.Drawing.Point(baseX, baseY + 2 * (h + sep));
            btnNum1.Size = new System.Drawing.Size(w, h);
            btnNum1.Text = "1";
            btnNum1.Tag = "1";
            btnNum1.Click += btnNum_Click;

            btnNum2.Location = new System.Drawing.Point(baseX + (w + sep), baseY + 2 * (h + sep));
            btnNum2.Size = new System.Drawing.Size(w, h);
            btnNum2.Text = "2";
            btnNum2.Tag = "2";
            btnNum2.Click += btnNum_Click;

            btnNum3.Location = new System.Drawing.Point(baseX + 2 * (w + sep), baseY + 2 * (h + sep));
            btnNum3.Size = new System.Drawing.Size(w, h);
            btnNum3.Text = "3";
            btnNum3.Tag = "3";
            btnNum3.Click += btnNum_Click;

            // fila 4: 0 y borrar
            btnNum0.Location = new System.Drawing.Point(baseX, baseY + 3 * (h + sep));
            btnNum0.Size = new System.Drawing.Size(w * 2 + sep, h);
            btnNum0.Text = "0";
            btnNum0.Tag = "0";
            btnNum0.Click += btnNum_Click;

            btnBorrarMonto.Location = new System.Drawing.Point(baseX + 2 * (w + sep), baseY + 3 * (h + sep));
            btnBorrarMonto.Size = new System.Drawing.Size(w, h);
            btnBorrarMonto.Text = "C";
            btnBorrarMonto.Click += btnBorrarMonto_Click;

            // btnAgregarLinea
            btnAgregarLinea.Location = new System.Drawing.Point(450, 192);
            btnAgregarLinea.Size = new System.Drawing.Size(100, 30);
            btnAgregarLinea.Text = "Agregar pago";
            btnAgregarLinea.Click += btnAgregarLinea_Click;

            // btnQuitarLinea
            btnQuitarLinea.Location = new System.Drawing.Point(560, 192);
            btnQuitarLinea.Size = new System.Drawing.Size(100, 30);
            btnQuitarLinea.Text = "Quitar pago";
            btnQuitarLinea.Click += btnQuitarLinea_Click;

            // gridPagos
            gridPagos.Location = new System.Drawing.Point(12, 250);
            gridPagos.Name = "gridPagos";
            gridPagos.ReadOnly = true;
            gridPagos.AllowUserToAddRows = false;
            gridPagos.AllowUserToDeleteRows = false;
            gridPagos.RowHeadersVisible = false;
            gridPagos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            gridPagos.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            gridPagos.Size = new System.Drawing.Size(670, 180);

            // label4 - Pagado
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(12, 440);
            label4.Text = "Pagado (DOP):";

            lblPagadoBase.AutoSize = true;
            lblPagadoBase.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            lblPagadoBase.Location = new System.Drawing.Point(110, 440);
            lblPagadoBase.Text = "0.00";

            // label5 - Pendiente
            label5.AutoSize = true;
            label5.Location = new System.Drawing.Point(220, 440);
            label5.Text = "Pendiente (DOP):";

            lblPendienteBase.AutoSize = true;
            lblPendienteBase.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            lblPendienteBase.Location = new System.Drawing.Point(320, 440);
            lblPendienteBase.Text = "0.00";

            // btnAceptar
            btnAceptar.Location = new System.Drawing.Point(490, 435);
            btnAceptar.Size = new System.Drawing.Size(90, 30);
            btnAceptar.Text = "Aceptar";
            btnAceptar.Click += btnAceptar_Click;

            // btnCancelar
            btnCancelar.Location = new System.Drawing.Point(592, 435);
            btnCancelar.Size = new System.Drawing.Size(90, 30);
            btnCancelar.Text = "Cancelar";
            btnCancelar.Click += btnCancelar_Click;

            // Add controls
            Controls.Add(label1);
            Controls.Add(lblTotalBase);
            Controls.Add(gridMonedas);

            Controls.Add(label2);
            Controls.Add(cbMoneda);
            Controls.Add(label7);
            Controls.Add(cbMedioPago);
            Controls.Add(label3);
            Controls.Add(txtMontoMoneda);

            Controls.Add(btnNum7);
            Controls.Add(btnNum8);
            Controls.Add(btnNum9);
            Controls.Add(btnNum4);
            Controls.Add(btnNum5);
            Controls.Add(btnNum6);
            Controls.Add(btnNum1);
            Controls.Add(btnNum2);
            Controls.Add(btnNum3);
            Controls.Add(btnNum0);
            Controls.Add(btnBorrarMonto);

            Controls.Add(btnAgregarLinea);
            Controls.Add(btnQuitarLinea);
            Controls.Add(gridPagos);

            Controls.Add(label4);
            Controls.Add(lblPagadoBase);
            Controls.Add(label5);
            Controls.Add(lblPendienteBase);
            Controls.Add(btnAceptar);
            Controls.Add(btnCancelar);

            ResumeLayout(false);
            PerformLayout();
        }
    }
}
