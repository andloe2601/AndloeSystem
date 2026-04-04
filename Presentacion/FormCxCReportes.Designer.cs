using System.Drawing;
using System.Windows.Forms;

namespace Andloe.Presentacion
{
    partial class FormCxCReportes
    {
        private System.ComponentModel.IContainer components = null;

        private Label lblBreadcrumb;
        private Label lblTitulo;
        private Label lblSubtitulo;

        private Panel cardEstadoCuenta;
        private Panel cardReportesGerenciales;

        private Label lblEstadoCuentaTitulo;
        private Label lblEstadoCuentaDesc;
        private Button btnEstadoCuenta;

        private Label lblGerencialesTitulo;
        private Label lblGerencialesDesc;
        private Button btnReportesGerenciales;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
                components.Dispose();

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            lblBreadcrumb = new Label();
            lblTitulo = new Label();
            lblSubtitulo = new Label();
            cardEstadoCuenta = new Panel();
            lblEstadoCuentaTitulo = new Label();
            lblEstadoCuentaDesc = new Label();
            btnEstadoCuenta = new Button();
            cardReportesGerenciales = new Panel();
            lblGerencialesTitulo = new Label();
            lblGerencialesDesc = new Label();
            btnReportesGerenciales = new Button();
            cardVendedores = new Panel();
            label1 = new Label();
            label2 = new Label();
            BtnVendedor = new Button();
            cardEstadoCuenta.SuspendLayout();
            cardReportesGerenciales.SuspendLayout();
            cardVendedores.SuspendLayout();
            SuspendLayout();
            // 
            // lblBreadcrumb
            // 
            lblBreadcrumb.AutoSize = true;
            lblBreadcrumb.Font = new Font("Segoe UI", 9F);
            lblBreadcrumb.ForeColor = Color.FromArgb(90, 100, 120);
            lblBreadcrumb.Location = new Point(20, 18);
            lblBreadcrumb.Name = "lblBreadcrumb";
            lblBreadcrumb.Size = new Size(168, 15);
            lblBreadcrumb.TabIndex = 0;
            lblBreadcrumb.Text = "Reportes > Cuentas por cobrar";
            // 
            // lblTitulo
            // 
            lblTitulo.AutoSize = true;
            lblTitulo.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            lblTitulo.ForeColor = Color.FromArgb(25, 35, 55);
            lblTitulo.Location = new Point(20, 45);
            lblTitulo.Name = "lblTitulo";
            lblTitulo.Size = new Size(234, 32);
            lblTitulo.TabIndex = 1;
            lblTitulo.Text = "Cuentas por cobrar";
            // 
            // lblSubtitulo
            // 
            lblSubtitulo.AutoSize = true;
            lblSubtitulo.Font = new Font("Segoe UI", 10F);
            lblSubtitulo.ForeColor = Color.FromArgb(90, 100, 120);
            lblSubtitulo.Location = new Point(20, 84);
            lblSubtitulo.Name = "lblSubtitulo";
            lblSubtitulo.Size = new Size(383, 19);
            lblSubtitulo.TabIndex = 2;
            lblSubtitulo.Text = "Consulta estados de cuenta y reportes gerenciales de cartera.";
            // 
            // cardEstadoCuenta
            // 
            cardEstadoCuenta.BackColor = Color.White;
            cardEstadoCuenta.BorderStyle = BorderStyle.FixedSingle;
            cardEstadoCuenta.Controls.Add(lblEstadoCuentaTitulo);
            cardEstadoCuenta.Controls.Add(lblEstadoCuentaDesc);
            cardEstadoCuenta.Controls.Add(btnEstadoCuenta);
            cardEstadoCuenta.Location = new Point(20, 130);
            cardEstadoCuenta.Name = "cardEstadoCuenta";
            cardEstadoCuenta.Size = new Size(330, 150);
            cardEstadoCuenta.TabIndex = 3;
            // 
            // lblEstadoCuentaTitulo
            // 
            lblEstadoCuentaTitulo.AutoSize = true;
            lblEstadoCuentaTitulo.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            lblEstadoCuentaTitulo.ForeColor = Color.FromArgb(25, 35, 55);
            lblEstadoCuentaTitulo.Location = new Point(16, 18);
            lblEstadoCuentaTitulo.Name = "lblEstadoCuentaTitulo";
            lblEstadoCuentaTitulo.Size = new Size(206, 20);
            lblEstadoCuentaTitulo.TabIndex = 0;
            lblEstadoCuentaTitulo.Text = "Estado de cuenta por cliente";
            // 
            // lblEstadoCuentaDesc
            // 
            lblEstadoCuentaDesc.Font = new Font("Segoe UI", 9.5F);
            lblEstadoCuentaDesc.ForeColor = Color.FromArgb(90, 100, 120);
            lblEstadoCuentaDesc.Location = new Point(16, 52);
            lblEstadoCuentaDesc.Name = "lblEstadoCuentaDesc";
            lblEstadoCuentaDesc.Size = new Size(290, 50);
            lblEstadoCuentaDesc.TabIndex = 1;
            lblEstadoCuentaDesc.Text = "Revisa el detalle de facturas, cobros, balance acumulado y vencimientos por cliente.";
            // 
            // btnEstadoCuenta
            // 
            btnEstadoCuenta.BackColor = Color.FromArgb(33, 179, 164);
            btnEstadoCuenta.FlatAppearance.BorderSize = 0;
            btnEstadoCuenta.FlatStyle = FlatStyle.Flat;
            btnEstadoCuenta.ForeColor = Color.White;
            btnEstadoCuenta.Location = new Point(16, 104);
            btnEstadoCuenta.Name = "btnEstadoCuenta";
            btnEstadoCuenta.Size = new Size(100, 32);
            btnEstadoCuenta.TabIndex = 2;
            btnEstadoCuenta.Text = "Abrir";
            btnEstadoCuenta.UseVisualStyleBackColor = false;
            // 
            // cardReportesGerenciales
            // 
            cardReportesGerenciales.BackColor = Color.White;
            cardReportesGerenciales.BorderStyle = BorderStyle.FixedSingle;
            cardReportesGerenciales.Controls.Add(lblGerencialesTitulo);
            cardReportesGerenciales.Controls.Add(lblGerencialesDesc);
            cardReportesGerenciales.Controls.Add(btnReportesGerenciales);
            cardReportesGerenciales.Location = new Point(370, 130);
            cardReportesGerenciales.Name = "cardReportesGerenciales";
            cardReportesGerenciales.Size = new Size(330, 150);
            cardReportesGerenciales.TabIndex = 4;
            // 
            // lblGerencialesTitulo
            // 
            lblGerencialesTitulo.AutoSize = true;
            lblGerencialesTitulo.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            lblGerencialesTitulo.ForeColor = Color.FromArgb(25, 35, 55);
            lblGerencialesTitulo.Location = new Point(16, 18);
            lblGerencialesTitulo.Name = "lblGerencialesTitulo";
            lblGerencialesTitulo.Size = new Size(184, 20);
            lblGerencialesTitulo.TabIndex = 0;
            lblGerencialesTitulo.Text = "Reportes gerenciales CxC";
            // 
            // lblGerencialesDesc
            // 
            lblGerencialesDesc.Font = new Font("Segoe UI", 9.5F);
            lblGerencialesDesc.ForeColor = Color.FromArgb(90, 100, 120);
            lblGerencialesDesc.Location = new Point(16, 52);
            lblGerencialesDesc.Name = "lblGerencialesDesc";
            lblGerencialesDesc.Size = new Size(290, 50);
            lblGerencialesDesc.TabIndex = 1;
            lblGerencialesDesc.Text = "Consulta indicadores de cartera, antigüedad de saldos y resumen gerencial de cobros.";
            // 
            // btnReportesGerenciales
            // 
            btnReportesGerenciales.BackColor = Color.FromArgb(33, 179, 164);
            btnReportesGerenciales.FlatAppearance.BorderSize = 0;
            btnReportesGerenciales.FlatStyle = FlatStyle.Flat;
            btnReportesGerenciales.ForeColor = Color.White;
            btnReportesGerenciales.Location = new Point(16, 104);
            btnReportesGerenciales.Name = "btnReportesGerenciales";
            btnReportesGerenciales.Size = new Size(100, 32);
            btnReportesGerenciales.TabIndex = 2;
            btnReportesGerenciales.Text = "Abrir";
            btnReportesGerenciales.UseVisualStyleBackColor = false;
            // 
            // cardVendedores
            // 
            cardVendedores.BackColor = Color.White;
            cardVendedores.BorderStyle = BorderStyle.FixedSingle;
            cardVendedores.Controls.Add(label1);
            cardVendedores.Controls.Add(label2);
            cardVendedores.Controls.Add(BtnVendedor);
            cardVendedores.Location = new Point(733, 130);
            cardVendedores.Name = "cardVendedores";
            cardVendedores.Size = new Size(330, 150);
            cardVendedores.TabIndex = 5;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            label1.ForeColor = Color.FromArgb(25, 35, 55);
            label1.Location = new Point(16, 18);
            label1.Name = "label1";
            label1.Size = new Size(158, 20);
            label1.TabIndex = 0;
            label1.Text = "Reportes Vendedores";
            // 
            // label2
            // 
            label2.Font = new Font("Segoe UI", 9.5F);
            label2.ForeColor = Color.FromArgb(90, 100, 120);
            label2.Location = new Point(16, 52);
            label2.Name = "label2";
            label2.Size = new Size(290, 50);
            label2.TabIndex = 1;
            label2.Text = "Consulta Venta Vendedores.";
            // 
            // BtnVendedor
            // 
            BtnVendedor.BackColor = Color.FromArgb(33, 179, 164);
            BtnVendedor.FlatAppearance.BorderSize = 0;
            BtnVendedor.FlatStyle = FlatStyle.Flat;
            BtnVendedor.ForeColor = Color.White;
            BtnVendedor.Location = new Point(16, 104);
            BtnVendedor.Name = "BtnVendedor";
            BtnVendedor.Size = new Size(100, 32);
            BtnVendedor.TabIndex = 2;
            BtnVendedor.Text = "Abrir";
            BtnVendedor.UseVisualStyleBackColor = false;
            // 
            // FormCxCReportes
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(248, 249, 252);
            ClientSize = new Size(1180, 720);
            Controls.Add(cardVendedores);
            Controls.Add(lblBreadcrumb);
            Controls.Add(lblTitulo);
            Controls.Add(lblSubtitulo);
            Controls.Add(cardEstadoCuenta);
            Controls.Add(cardReportesGerenciales);
            FormBorderStyle = FormBorderStyle.None;
            Name = "FormCxCReportes";
            Text = "Reportes CxC";
            cardEstadoCuenta.ResumeLayout(false);
            cardEstadoCuenta.PerformLayout();
            cardReportesGerenciales.ResumeLayout(false);
            cardReportesGerenciales.PerformLayout();
            cardVendedores.ResumeLayout(false);
            cardVendedores.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }
        private Panel cardVendedores;
        private Label label1;
        private Label label2;
        private Button BtnVendedor;
    }
}