using System.Drawing;
using System.Windows.Forms;

namespace Presentation
{
    partial class FormCierreCaja
    {
        private System.ComponentModel.IContainer components = null;

        // NUEVO: panel de encabezado para un look más moderno
        private Panel panelHeader;

        private Label lblTitulo;
        private Label lblCaja;
        private Label lblCajaValor;
        private Label lblUsuario;
        private Label lblUsuarioValor;
        private Label lblDesde;
        private Label lblHasta;
        private DateTimePicker dtDesde;
        private DateTimePicker dtHasta;
        private GroupBox grpTotales;
        private Label lblTotalVentas;
        private TextBox txtTotalVentas;
        private Label lblTotalPagos;
        private TextBox txtTotalPagos;
        private Label lblFondoInicial;
        private TextBox txtFondoInicial;
        private Label lblEfectivoMovimientos;
        private TextBox txtEfectivoMovimientos;
        private Label lblEfectivoTeorico;
        private TextBox txtEfectivoTeorico;
        private Label lblEfectivoDeclarado;
        private TextBox txtEfectivoDeclarado;
        private Label lblDiferencia;
        private TextBox txtDiferencia;
        private Label lblInfoFondo;
        private Button btnRecalcular;
        private Button btnGuardar;
        private Button btnCancelar;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();

            this.panelHeader = new Panel();
            this.lblTitulo = new Label();
            this.lblCaja = new Label();
            this.lblCajaValor = new Label();
            this.lblUsuario = new Label();
            this.lblUsuarioValor = new Label();
            this.lblDesde = new Label();
            this.lblHasta = new Label();
            this.dtDesde = new DateTimePicker();
            this.dtHasta = new DateTimePicker();
            this.btnRecalcular = new Button();
            this.grpTotales = new GroupBox();
            this.lblTotalVentas = new Label();
            this.txtTotalVentas = new TextBox();
            this.lblTotalPagos = new Label();
            this.txtTotalPagos = new TextBox();
            this.lblFondoInicial = new Label();
            this.txtFondoInicial = new TextBox();
            this.lblEfectivoMovimientos = new Label();
            this.txtEfectivoMovimientos = new TextBox();
            this.lblEfectivoTeorico = new Label();
            this.txtEfectivoTeorico = new TextBox();
            this.lblEfectivoDeclarado = new Label();
            this.txtEfectivoDeclarado = new TextBox();
            this.lblDiferencia = new Label();
            this.txtDiferencia = new TextBox();
            this.lblInfoFondo = new Label();
            this.btnGuardar = new Button();
            this.btnCancelar = new Button();

            this.grpTotales.SuspendLayout();
            this.SuspendLayout();

            // 
            // FormCierreCaja
            // 
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.BackColor = Color.White;
            this.ClientSize = new Size(620, 460);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormCierreCaja";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Cierre de Caja";
            this.Load += FormCierreCaja_Load;

            // ========= PANEL HEADER =========
            this.panelHeader.Dock = DockStyle.Top;
            this.panelHeader.Height = 60;
            this.panelHeader.BackColor = Color.FromArgb(30, 144, 255); // DodgerBlue
            this.panelHeader.Padding = new Padding(16, 10, 16, 10);
            this.panelHeader.Controls.Add(this.lblTitulo);

            // 
            // lblTitulo
            // 
            this.lblTitulo.AutoSize = false;
            this.lblTitulo.Dock = DockStyle.Fill;
            this.lblTitulo.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            this.lblTitulo.ForeColor = Color.White;
            this.lblTitulo.TextAlign = ContentAlignment.MiddleLeft;
            this.lblTitulo.Text = "Cierre de Caja";

            // ========= LABELS CAJA / USUARIO =========
            // lblCaja
            this.lblCaja.AutoSize = true;
            this.lblCaja.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            this.lblCaja.Location = new Point(20, 75);
            this.lblCaja.Name = "lblCaja";
            this.lblCaja.Size = new Size(35, 15);
            this.lblCaja.Text = "Caja:";

            // lblCajaValor
            this.lblCajaValor.AutoSize = true;
            this.lblCajaValor.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this.lblCajaValor.Location = new Point(60, 75);
            this.lblCajaValor.Name = "lblCajaValor";
            this.lblCajaValor.Size = new Size(24, 15);
            this.lblCajaValor.Text = "---";

            // lblUsuario
            this.lblUsuario.AutoSize = true;
            this.lblUsuario.Font = new Font("Segoe UI", 9F);
            this.lblUsuario.Location = new Point(200, 75);
            this.lblUsuario.Name = "lblUsuario";
            this.lblUsuario.Size = new Size(52, 15);
            this.lblUsuario.Text = "Usuario:";

            // lblUsuarioValor
            this.lblUsuarioValor.AutoSize = true;
            this.lblUsuarioValor.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this.lblUsuarioValor.Location = new Point(255, 75);
            this.lblUsuarioValor.Name = "lblUsuarioValor";
            this.lblUsuarioValor.Size = new Size(24, 15);
            this.lblUsuarioValor.Text = "---";

            // ========= FECHAS Y BOTÓN RE-CALCULAR =========
            // lblDesde
            this.lblDesde.AutoSize = true;
            this.lblDesde.Font = new Font("Segoe UI", 9F);
            this.lblDesde.Location = new Point(20, 105);
            this.lblDesde.Name = "lblDesde";
            this.lblDesde.Size = new Size(42, 15);
            this.lblDesde.Text = "Desde:";

            // dtDesde
            this.dtDesde.Format = DateTimePickerFormat.Custom;
            this.dtDesde.CustomFormat = "dd/MM/yyyy HH:mm";
            this.dtDesde.Location = new Point(70, 101);
            this.dtDesde.Name = "dtDesde";
            this.dtDesde.Size = new Size(160, 23);
            this.dtDesde.ValueChanged += dtDesde_ValueChanged;

            // lblHasta
            this.lblHasta.AutoSize = true;
            this.lblHasta.Font = new Font("Segoe UI", 9F);
            this.lblHasta.Location = new Point(250, 105);
            this.lblHasta.Name = "lblHasta";
            this.lblHasta.Size = new Size(40, 15);
            this.lblHasta.Text = "Hasta:";

            // dtHasta
            this.dtHasta.Format = DateTimePickerFormat.Custom;
            this.dtHasta.CustomFormat = "dd/MM/yyyy HH:mm";
            this.dtHasta.Location = new Point(300, 101);
            this.dtHasta.Name = "dtHasta";
            this.dtHasta.Size = new Size(160, 23);
            this.dtHasta.ValueChanged += dtHasta_ValueChanged;

            // btnRecalcular
            this.btnRecalcular.BackColor = Color.FromArgb(0, 122, 204);
            this.btnRecalcular.FlatStyle = FlatStyle.Flat;
            this.btnRecalcular.FlatAppearance.BorderSize = 0;
            this.btnRecalcular.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this.btnRecalcular.ForeColor = Color.White;
            this.btnRecalcular.Location = new Point(480, 100);
            this.btnRecalcular.Name = "btnRecalcular";
            this.btnRecalcular.Size = new Size(100, 26);
            this.btnRecalcular.Text = "Recalcular";
            this.btnRecalcular.UseVisualStyleBackColor = false;
            this.btnRecalcular.Click += btnRecalcular_Click;

            // ========= GRUPO TOTALES =========
            this.grpTotales.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            this.grpTotales.Location = new Point(20, 140);
            this.grpTotales.Name = "grpTotales";
            this.grpTotales.Size = new Size(560, 210);
            this.grpTotales.TabStop = true;
            this.grpTotales.Text = "Totales de la caja";

            // lblTotalVentas
            this.lblTotalVentas.AutoSize = true;
            this.lblTotalVentas.Location = new Point(20, 30);
            this.lblTotalVentas.Name = "lblTotalVentas";
            this.lblTotalVentas.Size = new Size(71, 15);
            this.lblTotalVentas.Text = "Total ventas:";

            // txtTotalVentas
            this.txtTotalVentas.Location = new Point(140, 27);
            this.txtTotalVentas.Name = "txtTotalVentas";
            this.txtTotalVentas.ReadOnly = true;
            this.txtTotalVentas.TextAlign = HorizontalAlignment.Right;
            this.txtTotalVentas.Size = new Size(120, 23);

            // lblTotalPagos
            this.lblTotalPagos.AutoSize = true;
            this.lblTotalPagos.Location = new Point(290, 30);
            this.lblTotalPagos.Name = "lblTotalPagos";
            this.lblTotalPagos.Size = new Size(69, 15);
            this.lblTotalPagos.Text = "Total pagos:";

            // txtTotalPagos
            this.txtTotalPagos.Location = new Point(380, 27);
            this.txtTotalPagos.Name = "txtTotalPagos";
            this.txtTotalPagos.ReadOnly = true;
            this.txtTotalPagos.TextAlign = HorizontalAlignment.Right;
            this.txtTotalPagos.Size = new Size(120, 23);

            // lblFondoInicial
            this.lblFondoInicial.AutoSize = true;
            this.lblFondoInicial.Location = new Point(20, 70);
            this.lblFondoInicial.Name = "lblFondoInicial";
            this.lblFondoInicial.Size = new Size(76, 15);
            this.lblFondoInicial.Text = "Fondo inicial:";

            // txtFondoInicial
            this.txtFondoInicial.Location = new Point(140, 67);
            this.txtFondoInicial.Name = "txtFondoInicial";
            this.txtFondoInicial.TextAlign = HorizontalAlignment.Right;
            this.txtFondoInicial.Size = new Size(120, 23);
            this.txtFondoInicial.Leave += txtFondoInicial_Leave;

            // lblEfectivoMovimientos
            this.lblEfectivoMovimientos.AutoSize = true;
            this.lblEfectivoMovimientos.Location = new Point(290, 70);
            this.lblEfectivoMovimientos.Name = "lblEfectivoMovimientos";
            this.lblEfectivoMovimientos.Size = new Size(108, 15);
            this.lblEfectivoMovimientos.Text = "Efectivo movim.:";

            // txtEfectivoMovimientos
            this.txtEfectivoMovimientos.Location = new Point(410, 67);
            this.txtEfectivoMovimientos.Name = "txtEfectivoMovimientos";
            this.txtEfectivoMovimientos.ReadOnly = true;
            this.txtEfectivoMovimientos.TextAlign = HorizontalAlignment.Right;
            this.txtEfectivoMovimientos.Size = new Size(90, 23);

            // lblEfectivoTeorico
            this.lblEfectivoTeorico.AutoSize = true;
            this.lblEfectivoTeorico.Location = new Point(20, 110);
            this.lblEfectivoTeorico.Name = "lblEfectivoTeorico";
            this.lblEfectivoTeorico.Size = new Size(92, 15);
            this.lblEfectivoTeorico.Text = "Efectivo teórico:";

            // txtEfectivoTeorico
            this.txtEfectivoTeorico.Location = new Point(140, 107);
            this.txtEfectivoTeorico.Name = "txtEfectivoTeorico";
            this.txtEfectivoTeorico.ReadOnly = true;
            this.txtEfectivoTeorico.TextAlign = HorizontalAlignment.Right;
            this.txtEfectivoTeorico.Size = new Size(120, 23);

            // lblEfectivoDeclarado
            this.lblEfectivoDeclarado.AutoSize = true;
            this.lblEfectivoDeclarado.Location = new Point(290, 110);
            this.lblEfectivoDeclarado.Name = "lblEfectivoDeclarado";
            this.lblEfectivoDeclarado.Size = new Size(106, 15);
            this.lblEfectivoDeclarado.Text = "Efectivo declarado:";

            // txtEfectivoDeclarado
            this.txtEfectivoDeclarado.Location = new Point(410, 107);
            this.txtEfectivoDeclarado.Name = "txtEfectivoDeclarado";
            this.txtEfectivoDeclarado.TextAlign = HorizontalAlignment.Right;
            this.txtEfectivoDeclarado.Size = new Size(90, 23);
            this.txtEfectivoDeclarado.Leave += txtEfectivoDeclarado_Leave;

            // lblDiferencia
            this.lblDiferencia.AutoSize = true;
            this.lblDiferencia.Location = new Point(20, 150);
            this.lblDiferencia.Name = "lblDiferencia";
            this.lblDiferencia.Size = new Size(65, 15);
            this.lblDiferencia.Text = "Diferencia:";

            // txtDiferencia
            this.txtDiferencia.Location = new Point(140, 147);
            this.txtDiferencia.Name = "txtDiferencia";
            this.txtDiferencia.ReadOnly = true;
            this.txtDiferencia.TextAlign = HorizontalAlignment.Right;
            this.txtDiferencia.Size = new Size(120, 23);

            // lblInfoFondo
            this.lblInfoFondo.AutoSize = true;
            this.lblInfoFondo.Font = new Font("Segoe UI", 8.5F, FontStyle.Italic);
            this.lblInfoFondo.ForeColor = Color.DimGray;
            this.lblInfoFondo.Location = new Point(20, 365);
            this.lblInfoFondo.Name = "lblInfoFondo";
            this.lblInfoFondo.Size = new Size(12, 15);
            this.lblInfoFondo.Text = "-";

            // ========= BOTONES GUARDAR / CANCELAR =========
            // btnGuardar
            this.btnGuardar.BackColor = Color.FromArgb(46, 204, 113);
            this.btnGuardar.FlatStyle = FlatStyle.Flat;
            this.btnGuardar.FlatAppearance.BorderSize = 0;
            this.btnGuardar.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this.btnGuardar.ForeColor = Color.White;
            this.btnGuardar.Location = new Point(380, 365);
            this.btnGuardar.Name = "btnGuardar";
            this.btnGuardar.Size = new Size(100, 30);
            this.btnGuardar.Text = "Guardar";
            this.btnGuardar.UseVisualStyleBackColor = false;
            this.btnGuardar.Click += btnGuardar_Click;

            // btnCancelar
            this.btnCancelar.BackColor = Color.White;
            this.btnCancelar.FlatStyle = FlatStyle.Flat;
            this.btnCancelar.FlatAppearance.BorderColor = Color.Silver;
            this.btnCancelar.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            this.btnCancelar.ForeColor = Color.Black;
            this.btnCancelar.Location = new Point(490, 365);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new Size(90, 30);
            this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.UseVisualStyleBackColor = false;
            this.btnCancelar.Click += btnCancelar_Click;

            // ========= ADD CONTROLS =========
            this.grpTotales.Controls.Add(this.lblTotalVentas);
            this.grpTotales.Controls.Add(this.txtTotalVentas);
            this.grpTotales.Controls.Add(this.lblTotalPagos);
            this.grpTotales.Controls.Add(this.txtTotalPagos);
            this.grpTotales.Controls.Add(this.lblFondoInicial);
            this.grpTotales.Controls.Add(this.txtFondoInicial);
            this.grpTotales.Controls.Add(this.lblEfectivoMovimientos);
            this.grpTotales.Controls.Add(this.txtEfectivoMovimientos);
            this.grpTotales.Controls.Add(this.lblEfectivoTeorico);
            this.grpTotales.Controls.Add(this.txtEfectivoTeorico);
            this.grpTotales.Controls.Add(this.lblEfectivoDeclarado);
            this.grpTotales.Controls.Add(this.txtEfectivoDeclarado);
            this.grpTotales.Controls.Add(this.lblDiferencia);
            this.grpTotales.Controls.Add(this.txtDiferencia);

            this.Controls.Add(this.panelHeader);
            this.Controls.Add(this.lblCaja);
            this.Controls.Add(this.lblCajaValor);
            this.Controls.Add(this.lblUsuario);
            this.Controls.Add(this.lblUsuarioValor);
            this.Controls.Add(this.lblDesde);
            this.Controls.Add(this.dtDesde);
            this.Controls.Add(this.lblHasta);
            this.Controls.Add(this.dtHasta);
            this.Controls.Add(this.btnRecalcular);
            this.Controls.Add(this.grpTotales);
            this.Controls.Add(this.lblInfoFondo);
            this.Controls.Add(this.btnGuardar);
            this.Controls.Add(this.btnCancelar);

            this.grpTotales.ResumeLayout(false);
            this.grpTotales.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion
    }
}
