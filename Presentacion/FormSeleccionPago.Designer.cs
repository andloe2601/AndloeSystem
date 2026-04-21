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

        private Button btnRapidoEfectivo;
        private Button btnRapidoTarjeta;
        private Button btnRapidoTransferencia;

        private Button btnAgregarLinea;
        private Button btnQuitarLinea;
        private Button btnCompletarPendiente;

        private DataGridView gridPagos;

        private Label label4;
        private Label lblPagadoBase;
        private Label label5;
        private Label lblPendienteBase;
        private Label label6;
        private Label lblCambioBase;

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

            btnRapidoEfectivo = new Button();
            btnRapidoTarjeta = new Button();
            btnRapidoTransferencia = new Button();

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
            btnCompletarPendiente = new Button();

            gridPagos = new DataGridView();

            label4 = new Label();
            lblPagadoBase = new Label();
            label5 = new Label();
            lblPendienteBase = new Label();
            label6 = new Label();
            lblCambioBase = new Label();

            btnAceptar = new Button();
            btnCancelar = new Button();

            ((System.ComponentModel.ISupportInitialize)gridMonedas).BeginInit();
            ((System.ComponentModel.ISupportInitialize)gridPagos).BeginInit();
            SuspendLayout();

            AutoScaleMode = AutoScaleMode.Font;
            BackColor = System.Drawing.Color.FromArgb(245, 247, 250);
            ClientSize = new System.Drawing.Size(980, 760);
            Font = new System.Drawing.Font("Segoe UI", 9.5F);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Seleccionar pagos";
            Load += FormSeleccionPago_Load;

            label1.AutoSize = true;
            label1.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            label1.ForeColor = System.Drawing.Color.FromArgb(31, 41, 55);
            label1.Location = new System.Drawing.Point(20, 18);
            label1.Text = "Total a cobrar:";

            lblTotalBase.AutoSize = true;
            lblTotalBase.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            lblTotalBase.ForeColor = System.Drawing.Color.FromArgb(13, 148, 136);
            lblTotalBase.Location = new System.Drawing.Point(155, 12);
            lblTotalBase.Text = "0.00 DOP";

            gridMonedas.Location = new System.Drawing.Point(20, 60);
            gridMonedas.Name = "gridMonedas";
            gridMonedas.ReadOnly = true;
            gridMonedas.AllowUserToAddRows = false;
            gridMonedas.AllowUserToDeleteRows = false;
            gridMonedas.AllowUserToResizeRows = false;
            gridMonedas.AllowUserToResizeColumns = false;
            gridMonedas.RowHeadersVisible = false;
            gridMonedas.MultiSelect = false;
            gridMonedas.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            gridMonedas.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            gridMonedas.BackgroundColor = System.Drawing.Color.White;
            gridMonedas.BorderStyle = BorderStyle.FixedSingle;
            gridMonedas.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.FromArgb(220, 252, 231);
            gridMonedas.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.FromArgb(17, 24, 39);
            gridMonedas.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(241, 245, 249);
            gridMonedas.ColumnHeadersDefaultCellStyle.ForeColor = System.Drawing.Color.FromArgb(17, 24, 39);
            gridMonedas.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            gridMonedas.EnableHeadersVisualStyles = false;
            gridMonedas.Size = new System.Drawing.Size(940, 150);

            label2.AutoSize = true;
            label2.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            label2.ForeColor = System.Drawing.Color.FromArgb(51, 65, 85);
            label2.Location = new System.Drawing.Point(20, 230);
            label2.Text = "Moneda";

            cbMoneda.DropDownStyle = ComboBoxStyle.DropDownList;
            cbMoneda.FlatStyle = FlatStyle.Flat;
            cbMoneda.Location = new System.Drawing.Point(20, 255);
            cbMoneda.Name = "cbMoneda";
            cbMoneda.Size = new System.Drawing.Size(220, 28);
            cbMoneda.SelectedIndexChanged += cbMoneda_SelectedIndexChanged;

            label7.AutoSize = true;
            label7.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            label7.ForeColor = System.Drawing.Color.FromArgb(51, 65, 85);
            label7.Location = new System.Drawing.Point(260, 230);
            label7.Text = "Medio de pago";

            cbMedioPago.DropDownStyle = ComboBoxStyle.DropDownList;
            cbMedioPago.FlatStyle = FlatStyle.Flat;
            cbMedioPago.Location = new System.Drawing.Point(260, 255);
            cbMedioPago.Name = "cbMedioPago";
            cbMedioPago.Size = new System.Drawing.Size(240, 28);

            label3.AutoSize = true;
            label3.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            label3.ForeColor = System.Drawing.Color.FromArgb(51, 65, 85);
            label3.Location = new System.Drawing.Point(520, 230);
            label3.Text = "Monto";

            txtMontoMoneda.Location = new System.Drawing.Point(520, 255);
            txtMontoMoneda.Name = "txtMontoMoneda";
            txtMontoMoneda.Size = new System.Drawing.Size(200, 28);
            txtMontoMoneda.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            txtMontoMoneda.TextAlign = HorizontalAlignment.Right;

            btnRapidoEfectivo.Location = new System.Drawing.Point(740, 245);
            btnRapidoEfectivo.Size = new System.Drawing.Size(100, 36);
            btnRapidoEfectivo.Text = "Efectivo";
            btnRapidoEfectivo.Tag = "efectivo";
            btnRapidoEfectivo.FlatStyle = FlatStyle.Flat;
            btnRapidoEfectivo.BackColor = System.Drawing.Color.White;
            btnRapidoEfectivo.ForeColor = System.Drawing.Color.FromArgb(17, 24, 39);

            btnRapidoTarjeta.Location = new System.Drawing.Point(850, 245);
            btnRapidoTarjeta.Size = new System.Drawing.Size(100, 36);
            btnRapidoTarjeta.Text = "Tarjeta";
            btnRapidoTarjeta.Tag = "tarjeta";
            btnRapidoTarjeta.FlatStyle = FlatStyle.Flat;
            btnRapidoTarjeta.BackColor = System.Drawing.Color.White;
            btnRapidoTarjeta.ForeColor = System.Drawing.Color.FromArgb(17, 24, 39);

            btnRapidoTransferencia.Location = new System.Drawing.Point(740, 287);
            btnRapidoTransferencia.Size = new System.Drawing.Size(210, 36);
            btnRapidoTransferencia.Text = "Transferencia";
            btnRapidoTransferencia.Tag = "transferencia";
            btnRapidoTransferencia.FlatStyle = FlatStyle.Flat;
            btnRapidoTransferencia.BackColor = System.Drawing.Color.White;
            btnRapidoTransferencia.ForeColor = System.Drawing.Color.FromArgb(17, 24, 39);

            int baseX = 20;
            int baseY = 305;
            int w = 82;
            int h = 42;
            int sep = 8;

            btnNum7.Location = new System.Drawing.Point(baseX, baseY);
            btnNum7.Size = new System.Drawing.Size(w, h);
            btnNum7.Text = "7";
            btnNum7.Tag = "7";

            btnNum8.Location = new System.Drawing.Point(baseX + (w + sep), baseY);
            btnNum8.Size = new System.Drawing.Size(w, h);
            btnNum8.Text = "8";
            btnNum8.Tag = "8";

            btnNum9.Location = new System.Drawing.Point(baseX + 2 * (w + sep), baseY);
            btnNum9.Size = new System.Drawing.Size(w, h);
            btnNum9.Text = "9";
            btnNum9.Tag = "9";

            btnNum4.Location = new System.Drawing.Point(baseX, baseY + h + sep);
            btnNum4.Size = new System.Drawing.Size(w, h);
            btnNum4.Text = "4";
            btnNum4.Tag = "4";

            btnNum5.Location = new System.Drawing.Point(baseX + (w + sep), baseY + h + sep);
            btnNum5.Size = new System.Drawing.Size(w, h);
            btnNum5.Text = "5";
            btnNum5.Tag = "5";

            btnNum6.Location = new System.Drawing.Point(baseX + 2 * (w + sep), baseY + h + sep);
            btnNum6.Size = new System.Drawing.Size(w, h);
            btnNum6.Text = "6";
            btnNum6.Tag = "6";

            btnNum1.Location = new System.Drawing.Point(baseX, baseY + 2 * (h + sep));
            btnNum1.Size = new System.Drawing.Size(w, h);
            btnNum1.Text = "1";
            btnNum1.Tag = "1";

            btnNum2.Location = new System.Drawing.Point(baseX + (w + sep), baseY + 2 * (h + sep));
            btnNum2.Size = new System.Drawing.Size(w, h);
            btnNum2.Text = "2";
            btnNum2.Tag = "2";

            btnNum3.Location = new System.Drawing.Point(baseX + 2 * (w + sep), baseY + 2 * (h + sep));
            btnNum3.Size = new System.Drawing.Size(w, h);
            btnNum3.Text = "3";
            btnNum3.Tag = "3";

            btnNum0.Location = new System.Drawing.Point(baseX, baseY + 3 * (h + sep));
            btnNum0.Size = new System.Drawing.Size(w * 2 + sep, h);
            btnNum0.Text = "0";
            btnNum0.Tag = "0";

            btnBorrarMonto.Location = new System.Drawing.Point(baseX + 2 * (w + sep), baseY + 3 * (h + sep));
            btnBorrarMonto.Size = new System.Drawing.Size(w, h);
            btnBorrarMonto.Text = "C";

            var keypadButtons = new Button[]
            {
        btnNum0, btnNum1, btnNum2, btnNum3, btnNum4, btnNum5,
        btnNum6, btnNum7, btnNum8, btnNum9, btnBorrarMonto
            };

            foreach (var btn in keypadButtons)
            {
                btn.FlatStyle = FlatStyle.Flat;
                btn.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(203, 213, 225);
                btn.BackColor = System.Drawing.Color.White;
                btn.ForeColor = System.Drawing.Color.FromArgb(17, 24, 39);
                btn.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            }

            btnNum0.Click += btnNum_Click;
            btnNum1.Click += btnNum_Click;
            btnNum2.Click += btnNum_Click;
            btnNum3.Click += btnNum_Click;
            btnNum4.Click += btnNum_Click;
            btnNum5.Click += btnNum_Click;
            btnNum6.Click += btnNum_Click;
            btnNum7.Click += btnNum_Click;
            btnNum8.Click += btnNum_Click;
            btnNum9.Click += btnNum_Click;
            btnBorrarMonto.Click += btnBorrarMonto_Click;

            btnAgregarLinea.Location = new System.Drawing.Point(520, 305);
            btnAgregarLinea.Size = new System.Drawing.Size(170, 42);
            btnAgregarLinea.Text = "Agregar pago";
            btnAgregarLinea.FlatStyle = FlatStyle.Flat;
            btnAgregarLinea.FlatAppearance.BorderSize = 0;
            btnAgregarLinea.BackColor = System.Drawing.Color.FromArgb(13, 177, 146);
            btnAgregarLinea.ForeColor = System.Drawing.Color.White;
            btnAgregarLinea.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            btnAgregarLinea.Click += btnAgregarLinea_Click;

            btnQuitarLinea.Location = new System.Drawing.Point(520, 365);
            btnQuitarLinea.Size = new System.Drawing.Size(170, 42);
            btnQuitarLinea.FlatStyle = FlatStyle.Flat;
            btnQuitarLinea.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(226, 232, 240);
            btnQuitarLinea.BackColor = System.Drawing.Color.White;
            btnQuitarLinea.ForeColor = System.Drawing.Color.FromArgb(17, 24, 39);
            btnQuitarLinea.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            btnQuitarLinea.Text = "Quitar pago";
            btnQuitarLinea.Click += btnQuitarLinea_Click;

            btnCompletarPendiente.Location = new System.Drawing.Point(520, 405);
            btnCompletarPendiente.Size = new System.Drawing.Size(170, 42);
            btnCompletarPendiente.Text = "Pagar exacto";
            btnCompletarPendiente.FlatStyle = FlatStyle.Flat;
            btnCompletarPendiente.FlatAppearance.BorderSize = 0;
            btnCompletarPendiente.BackColor = System.Drawing.Color.FromArgb(59, 130, 246);
            btnCompletarPendiente.ForeColor = System.Drawing.Color.White;
            btnCompletarPendiente.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            btnCompletarPendiente.Click += btnCompletarPendiente_Click;

            gridPagos.Location = new System.Drawing.Point(20, 520);
            gridPagos.Name = "gridPagos";
            gridPagos.ReadOnly = true;
            gridPagos.AllowUserToAddRows = false;
            gridPagos.AllowUserToDeleteRows = false;
            gridPagos.AllowUserToResizeRows = false;
            gridPagos.RowHeadersVisible = false;
            gridPagos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            gridPagos.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            gridPagos.MultiSelect = false;
            gridPagos.BackgroundColor = System.Drawing.Color.White;
            gridPagos.BorderStyle = BorderStyle.FixedSingle;
            gridPagos.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.FromArgb(219, 234, 254);
            gridPagos.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.FromArgb(17, 24, 39);
            gridPagos.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(241, 245, 249);
            gridPagos.ColumnHeadersDefaultCellStyle.ForeColor = System.Drawing.Color.FromArgb(17, 24, 39);
            gridPagos.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            gridPagos.EnableHeadersVisualStyles = false;
            gridPagos.Size = new System.Drawing.Size(940, 130);

            label4.AutoSize = true;
            label4.Font = new System.Drawing.Font("Segoe UI", 10.5F, System.Drawing.FontStyle.Bold);
            label4.ForeColor = System.Drawing.Color.FromArgb(51, 65, 85);
            label4.Location = new System.Drawing.Point(20, 650);
            label4.Text = "Pagado (DOP):";

            lblPagadoBase.AutoSize = true;
            lblPagadoBase.Font = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Bold);
            lblPagadoBase.ForeColor = System.Drawing.Color.FromArgb(13, 148, 136);
            lblPagadoBase.Location = new System.Drawing.Point(135, 647);
            lblPagadoBase.Text = "0.00";

            label5.AutoSize = true;
            label5.Font = new System.Drawing.Font("Segoe UI", 10.5F, System.Drawing.FontStyle.Bold);
            label5.ForeColor = System.Drawing.Color.FromArgb(51, 65, 85);
            label5.Location = new System.Drawing.Point(280, 650);
            label5.Text = "Pendiente (DOP):";

            lblPendienteBase.AutoSize = true;
            lblPendienteBase.Font = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Bold);
            lblPendienteBase.ForeColor = System.Drawing.Color.FromArgb(220, 38, 38);
            lblPendienteBase.Location = new System.Drawing.Point(425, 647);
            lblPendienteBase.Text = "0.00";

            label6.AutoSize = true;
            label6.Font = new System.Drawing.Font("Segoe UI", 10.5F, System.Drawing.FontStyle.Bold);
            label6.ForeColor = System.Drawing.Color.FromArgb(51, 65, 85);
            label6.Location = new System.Drawing.Point(20, 683);
            label6.Text = "Cambio (DOP):";

            lblCambioBase.AutoSize = true;
            lblCambioBase.Font = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Bold);
            lblCambioBase.ForeColor = System.Drawing.Color.FromArgb(37, 99, 235);
            lblCambioBase.Location = new System.Drawing.Point(135, 680);
            lblCambioBase.Text = "0.00";

            btnAceptar.Location = new System.Drawing.Point(740, 665);
            btnAceptar.Size = new System.Drawing.Size(110, 42);
            btnAceptar.Text = "Finalizar";
            btnAceptar.FlatStyle = FlatStyle.Flat;
            btnAceptar.FlatAppearance.BorderSize = 0;
            btnAceptar.BackColor = System.Drawing.Color.FromArgb(13, 177, 146);
            btnAceptar.ForeColor = System.Drawing.Color.White;
            btnAceptar.Font = new System.Drawing.Font("Segoe UI", 10.5F, System.Drawing.FontStyle.Bold);
            btnAceptar.Click += btnAceptar_Click;

            btnCancelar.Location = new System.Drawing.Point(850, 665);
            btnCancelar.Size = new System.Drawing.Size(110, 42);
            btnCancelar.Text = "Cancelar";
            btnCancelar.FlatStyle = FlatStyle.Flat;
            btnCancelar.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(203, 213, 225);
            btnCancelar.BackColor = System.Drawing.Color.White;
            btnCancelar.ForeColor = System.Drawing.Color.FromArgb(17, 24, 39);
            btnCancelar.Font = new System.Drawing.Font("Segoe UI", 10.5F, System.Drawing.FontStyle.Bold);
            btnCancelar.Click += btnCancelar_Click;

            Controls.Add(label1);
            Controls.Add(lblTotalBase);
            Controls.Add(gridMonedas);

            Controls.Add(label2);
            Controls.Add(cbMoneda);
            Controls.Add(label7);
            Controls.Add(cbMedioPago);
            Controls.Add(label3);
            Controls.Add(txtMontoMoneda);

            Controls.Add(btnRapidoEfectivo);
            Controls.Add(btnRapidoTarjeta);
            Controls.Add(btnRapidoTransferencia);

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
            Controls.Add(btnCompletarPendiente);

            Controls.Add(gridPagos);

            Controls.Add(label4);
            Controls.Add(lblPagadoBase);
            Controls.Add(label5);
            Controls.Add(lblPendienteBase);
            Controls.Add(label6);
            Controls.Add(lblCambioBase);
            Controls.Add(btnAceptar);
            Controls.Add(btnCancelar);

            ResumeLayout(false);
            PerformLayout();
        }
    }
}