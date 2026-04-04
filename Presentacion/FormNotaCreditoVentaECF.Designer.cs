namespace Andloe.Presentacion
{
    partial class FormNotaCreditoVentaECF
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.panelTop = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.panelLeft = new System.Windows.Forms.Panel();
            this.gridVentas = new System.Windows.Forms.DataGridView();
            this.panelVentaSearch = new System.Windows.Forms.Panel();
            this.btnTomarVenta = new System.Windows.Forms.Button();
            this.btnBuscarPorId = new System.Windows.Forms.Button();
            this.txtFacturaId = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.btnBuscarVentas = new System.Windows.Forms.Button();
            this.txtBuscarVenta = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.panelCenter = new System.Windows.Forms.Panel();
            this.gridLineas = new System.Windows.Forms.DataGridView();
            this.panelCab = new System.Windows.Forms.Panel();
            this.txtMotivo = new System.Windows.Forms.TextBox();
            this.label18 = new System.Windows.Forms.Label();
            this.txtENcfOrigen = new System.Windows.Forms.TextBox();
            this.label17 = new System.Windows.Forms.Label();
            this.txtNoVenta = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.txtTasaCambio = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.txtMoneda = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.txtClienteDoc = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.txtClienteNombre = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.txtClienteCodigo = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.txtVentaId = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.txtFecha = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtUsuario = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtENcf = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtEstado = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtNoNC = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtNCId = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.panelBottom = new System.Windows.Forms.Panel();
            this.lblStatus = new System.Windows.Forms.Label();
            this.btnCerrar = new System.Windows.Forms.Button();
            this.btnLimpiar = new System.Windows.Forms.Button();
            this.btnGenerarEncf = new System.Windows.Forms.Button();
            this.cboTipoENcf = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnEmitirNc = new System.Windows.Forms.Button();
            this.txtTotal = new System.Windows.Forms.TextBox();
            this.txtItbis = new System.Windows.Forms.TextBox();
            this.txtSubTotal = new System.Windows.Forms.TextBox();
            this.label19 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();

            this.panelTop.SuspendLayout();
            this.panelLeft.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridVentas)).BeginInit();
            this.panelVentaSearch.SuspendLayout();
            this.panelCenter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridLineas)).BeginInit();
            this.panelCab.SuspendLayout();
            this.panelBottom.SuspendLayout();
            this.SuspendLayout();

            // panelTop
            this.panelTop.BackColor = System.Drawing.Color.FromArgb(46, 51, 73);
            this.panelTop.Controls.Add(this.lblTitle);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(1460, 52);

            // lblTitle
            this.lblTitle.AutoSize = true;
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI Semibold", 14F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Location = new System.Drawing.Point(14, 12);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(272, 25);
            this.lblTitle.Text = "Nota de crédito venta e-NCF";

            // panelLeft
            this.panelLeft.Controls.Add(this.gridVentas);
            this.panelLeft.Controls.Add(this.panelVentaSearch);
            this.panelLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelLeft.Padding = new System.Windows.Forms.Padding(8);
            this.panelLeft.Location = new System.Drawing.Point(0, 52);
            this.panelLeft.Name = "panelLeft";
            this.panelLeft.Size = new System.Drawing.Size(430, 748);

            // gridVentas
            this.gridVentas.BackgroundColor = System.Drawing.Color.White;
            this.gridVentas.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridVentas.Location = new System.Drawing.Point(8, 98);
            this.gridVentas.Name = "gridVentas";
            this.gridVentas.Size = new System.Drawing.Size(414, 642);

            // panelVentaSearch
            this.panelVentaSearch.Controls.Add(this.btnTomarVenta);
            this.panelVentaSearch.Controls.Add(this.btnBuscarPorId);
            this.panelVentaSearch.Controls.Add(this.txtFacturaId);
            this.panelVentaSearch.Controls.Add(this.label15);
            this.panelVentaSearch.Controls.Add(this.btnBuscarVentas);
            this.panelVentaSearch.Controls.Add(this.txtBuscarVenta);
            this.panelVentaSearch.Controls.Add(this.label14);
            this.panelVentaSearch.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelVentaSearch.Location = new System.Drawing.Point(8, 8);
            this.panelVentaSearch.Name = "panelVentaSearch";
            this.panelVentaSearch.Size = new System.Drawing.Size(414, 90);

            // btnTomarVenta
            this.btnTomarVenta.Location = new System.Drawing.Point(300, 54);
            this.btnTomarVenta.Name = "btnTomarVenta";
            this.btnTomarVenta.Size = new System.Drawing.Size(104, 28);
            this.btnTomarVenta.Text = "Tomar venta";

            // btnBuscarPorId
            this.btnBuscarPorId.Location = new System.Drawing.Point(190, 54);
            this.btnBuscarPorId.Name = "btnBuscarPorId";
            this.btnBuscarPorId.Size = new System.Drawing.Size(104, 28);
            this.btnBuscarPorId.Text = "Cargar por Id";

            // txtFacturaId
            this.txtFacturaId.Location = new System.Drawing.Point(82, 56);
            this.txtFacturaId.Name = "txtFacturaId";
            this.txtFacturaId.Size = new System.Drawing.Size(102, 23);

            // label15
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(9, 59);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(49, 15);
            this.label15.Text = "Venta Id";

            // btnBuscarVentas
            this.btnBuscarVentas.Location = new System.Drawing.Point(300, 16);
            this.btnBuscarVentas.Name = "btnBuscarVentas";
            this.btnBuscarVentas.Size = new System.Drawing.Size(104, 28);
            this.btnBuscarVentas.Text = "Buscar";

            // txtBuscarVenta
            this.txtBuscarVenta.Location = new System.Drawing.Point(82, 19);
            this.txtBuscarVenta.Name = "txtBuscarVenta";
            this.txtBuscarVenta.Size = new System.Drawing.Size(212, 23);

            // label14
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(9, 22);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(40, 15);
            this.label14.Text = "Buscar";

            // panelCenter
            this.panelCenter.Controls.Add(this.gridLineas);
            this.panelCenter.Controls.Add(this.panelCab);
            this.panelCenter.Controls.Add(this.panelBottom);
            this.panelCenter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelCenter.Padding = new System.Windows.Forms.Padding(8);
            this.panelCenter.Location = new System.Drawing.Point(430, 52);
            this.panelCenter.Name = "panelCenter";
            this.panelCenter.Size = new System.Drawing.Size(1030, 748);

            // gridLineas
            this.gridLineas.BackgroundColor = System.Drawing.Color.White;
            this.gridLineas.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridLineas.Location = new System.Drawing.Point(8, 183);
            this.gridLineas.Name = "gridLineas";
            this.gridLineas.Size = new System.Drawing.Size(1014, 457);

            // panelCab
            this.panelCab.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelCab.Location = new System.Drawing.Point(8, 8);
            this.panelCab.Name = "panelCab";
            this.panelCab.Size = new System.Drawing.Size(1014, 175);

            this.panelCab.Controls.AddRange(new System.Windows.Forms.Control[]
            {
                txtMotivo,label18,txtENcfOrigen,label17,txtNoVenta,label16,txtTasaCambio,label13,txtMoneda,label12,
                txtClienteDoc,label11,txtClienteNombre,label10,txtClienteCodigo,label9,txtVentaId,label8,txtFecha,label7,
                txtUsuario,label6,txtENcf,label5,txtEstado,label4,txtNoNC,label3,txtNCId,label2
            });

            int x1 = 20, x2 = 85, y = 26, dy = 29, w1 = 110;
            int x4 = 312, w2 = 164;
            int x5 = 527, x6 = 589, w3 = 195;
            int x7 = 793, x8 = 844, w4 = 150;

            // fila 1
            this.txtNCId.Location = new System.Drawing.Point(x2, y);
            this.txtNCId.ReadOnly = true;
            this.txtNCId.Size = new System.Drawing.Size(w1, 23);
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(x1, y + 3);
            this.label2.Text = "NC Id";

            this.txtNoNC.Location = new System.Drawing.Point(x4, y);
            this.txtNoNC.ReadOnly = true;
            this.txtNoNC.Size = new System.Drawing.Size(w2, 23);
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(263, y + 3);
            this.label3.Text = "No NC";

            this.txtUsuario.Location = new System.Drawing.Point(x6, y);
            this.txtUsuario.ReadOnly = true;
            this.txtUsuario.Size = new System.Drawing.Size(134, 23);
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(x5, y + 3);
            this.label6.Text = "Usuario";

            this.txtFecha.Location = new System.Drawing.Point(x8, y);
            this.txtFecha.ReadOnly = true;
            this.txtFecha.Size = new System.Drawing.Size(w4, 23);
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(x7, y + 3);
            this.label7.Text = "Fecha";

            // fila 2
            y += dy;
            this.txtClienteCodigo.Location = new System.Drawing.Point(x4, y);
            this.txtClienteCodigo.ReadOnly = true;
            this.txtClienteCodigo.Size = new System.Drawing.Size(w2, 23);
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(218, y + 3);
            this.label9.Text = "Código cliente";

            this.txtClienteNombre.Location = new System.Drawing.Point(x6, y);
            this.txtClienteNombre.ReadOnly = true;
            this.txtClienteNombre.Size = new System.Drawing.Size(355, 23);
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(x5, y + 3);
            this.label10.Text = "Cliente";

            // fila 3
            y += dy;
            this.txtClienteDoc.Location = new System.Drawing.Point(x4, y);
            this.txtClienteDoc.ReadOnly = true;
            this.txtClienteDoc.Size = new System.Drawing.Size(w2, 23);
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(232, y + 3);
            this.label11.Text = "RNC/Cédula";

            this.txtMoneda.Location = new System.Drawing.Point(x6, y);
            this.txtMoneda.ReadOnly = true;
            this.txtMoneda.Size = new System.Drawing.Size(99, 23);
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(x5, y + 3);
            this.label12.Text = "Moneda";

            this.txtTasaCambio.Location = new System.Drawing.Point(886, y);
            this.txtTasaCambio.ReadOnly = true;
            this.txtTasaCambio.Size = new System.Drawing.Size(108, 23);
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(806, y + 3);
            this.label13.Text = "Tasa cambio";

            // fila 4
            y += dy;
            this.txtVentaId.Location = new System.Drawing.Point(x2, y);
            this.txtVentaId.ReadOnly = true;
            this.txtVentaId.Size = new System.Drawing.Size(w1, 23);
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(x1, y + 3);
            this.label8.Text = "Venta Id";

            this.txtNoVenta.Location = new System.Drawing.Point(x4, y);
            this.txtNoVenta.ReadOnly = true;
            this.txtNoVenta.Size = new System.Drawing.Size(w2, 23);
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(249, y + 3);
            this.label16.Text = "No. venta";

            this.txtENcfOrigen.Location = new System.Drawing.Point(x6, y);
            this.txtENcfOrigen.ReadOnly = true;
            this.txtENcfOrigen.Size = new System.Drawing.Size(195, 23);
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(512, y + 3);
            this.label17.Text = "e-NCF orig";

            // fila 5
            y += dy;
            this.txtEstado.Location = new System.Drawing.Point(x2, y);
            this.txtEstado.ReadOnly = true;
            this.txtEstado.Size = new System.Drawing.Size(w1, 23);
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(x1, y + 3);
            this.label4.Text = "Estado";

            this.txtENcf.Location = new System.Drawing.Point(x4, y);
            this.txtENcf.ReadOnly = true;
            this.txtENcf.Size = new System.Drawing.Size(w2, 23);
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(263, y + 3);
            this.label5.Text = "e-NCF";

            this.txtMotivo.Location = new System.Drawing.Point(x6, y);
            this.txtMotivo.Size = new System.Drawing.Size(355, 23);
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(528, y + 3);
            this.label18.Text = "Motivo";

            // panelBottom
            this.panelBottom.Controls.AddRange(new System.Windows.Forms.Control[]
            {
                lblStatus,btnCerrar,btnLimpiar,btnGenerarEncf,cboTipoENcf,label1,btnEmitirNc,
                txtTotal,txtItbis,txtSubTotal,label19,label20,label21
            });
            this.panelBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelBottom.Location = new System.Drawing.Point(8, 640);
            this.panelBottom.Name = "panelBottom";
            this.panelBottom.Size = new System.Drawing.Size(1014, 100);

            this.lblStatus.AutoSize = true;
            this.lblStatus.ForeColor = System.Drawing.Color.DimGray;
            this.lblStatus.Location = new System.Drawing.Point(18, 74);
            this.lblStatus.Text = "Listo...";

            this.btnCerrar.Location = new System.Drawing.Point(894, 12);
            this.btnCerrar.Size = new System.Drawing.Size(104, 32);
            this.btnCerrar.Text = "Cerrar";

            this.btnLimpiar.Location = new System.Drawing.Point(784, 12);
            this.btnLimpiar.Size = new System.Drawing.Size(104, 32);
            this.btnLimpiar.Text = "Limpiar";

            this.btnGenerarEncf.Location = new System.Drawing.Point(894, 50);
            this.btnGenerarEncf.Size = new System.Drawing.Size(104, 32);
            this.btnGenerarEncf.Text = "Generar e-NCF";

            this.cboTipoENcf.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboTipoENcf.Location = new System.Drawing.Point(665, 17);
            this.cboTipoENcf.Size = new System.Drawing.Size(113, 23);
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(590, 20);
            this.label1.Text = "Tipo e-NCF";

            this.btnEmitirNc.Location = new System.Drawing.Point(784, 50);
            this.btnEmitirNc.Size = new System.Drawing.Size(104, 32);
            this.btnEmitirNc.Text = "Guardar NC";

            this.txtSubTotal.Location = new System.Drawing.Point(76, 16);
            this.txtSubTotal.ReadOnly = true;
            this.txtSubTotal.Size = new System.Drawing.Size(118, 23);
            this.txtSubTotal.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(18, 19);
            this.label21.Text = "SubTotal";

            this.txtItbis.Location = new System.Drawing.Point(258, 16);
            this.txtItbis.ReadOnly = true;
            this.txtItbis.Size = new System.Drawing.Size(118, 23);
            this.txtItbis.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(218, 19);
            this.label20.Text = "ITBIS";

            this.txtTotal.Location = new System.Drawing.Point(449, 16);
            this.txtTotal.ReadOnly = true;
            this.txtTotal.Size = new System.Drawing.Size(118, 23);
            this.txtTotal.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(408, 19);
            this.label19.Text = "Total";

            // Form
            this.ClientSize = new System.Drawing.Size(1460, 800);
            this.Controls.Add(this.panelCenter);
            this.Controls.Add(this.panelLeft);
            this.Controls.Add(this.panelTop);
            this.Name = "FormNotaCreditoVentaECF";
            this.Text = "Nota de crédito venta / e-NCF";

            this.panelTop.ResumeLayout(false);
            this.panelTop.PerformLayout();
            this.panelLeft.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridVentas)).EndInit();
            this.panelVentaSearch.ResumeLayout(false);
            this.panelVentaSearch.PerformLayout();
            this.panelCenter.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridLineas)).EndInit();
            this.panelCab.ResumeLayout(false);
            this.panelCab.PerformLayout();
            this.panelBottom.ResumeLayout(false);
            this.panelBottom.PerformLayout();
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Panel panelLeft;
        private System.Windows.Forms.DataGridView gridVentas;
        private System.Windows.Forms.Panel panelVentaSearch;
        private System.Windows.Forms.Button btnTomarVenta;
        private System.Windows.Forms.Button btnBuscarPorId;
        private System.Windows.Forms.TextBox txtFacturaId;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Button btnBuscarVentas;
        private System.Windows.Forms.TextBox txtBuscarVenta;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Panel panelCenter;
        private System.Windows.Forms.DataGridView gridLineas;
        private System.Windows.Forms.Panel panelCab;
        private System.Windows.Forms.TextBox txtMotivo;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.TextBox txtENcfOrigen;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.TextBox txtNoVenta;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.TextBox txtTasaCambio;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox txtMoneda;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox txtClienteDoc;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox txtClienteNombre;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox txtClienteCodigo;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txtVentaId;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtFecha;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtUsuario;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtENcf;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtEstado;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtNoNC;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtNCId;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panelBottom;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Button btnCerrar;
        private System.Windows.Forms.Button btnLimpiar;
        private System.Windows.Forms.Button btnGenerarEncf;
        private System.Windows.Forms.ComboBox cboTipoENcf;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnEmitirNc;
        private System.Windows.Forms.TextBox txtTotal;
        private System.Windows.Forms.TextBox txtItbis;
        private System.Windows.Forms.TextBox txtSubTotal;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Label label21;
    }
}