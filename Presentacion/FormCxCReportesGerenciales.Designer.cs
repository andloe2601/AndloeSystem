using System.Drawing;
using System.Windows.Forms;

namespace Andloe.Presentacion
{
    partial class FormCxCReportesGerenciales
    {
        private System.ComponentModel.IContainer components = null;

        private Label lblTitulo;
        private Label lblSubtitulo;

        private Panel pnlResumen1;
        private Panel pnlResumen2;
        private Panel pnlResumen3;

        private Label lblBalanceTitulo;
        private Label lblBalanceValor;

        private Label lblVencidoTitulo;
        private Label lblVencidoValor;

        private Label lblCobradoTitulo;
        private Label lblCobradoValor;

        private DataGridView gridResumen;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
                components.Dispose();

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();

            lblTitulo = new Label();
            lblSubtitulo = new Label();

            pnlResumen1 = new Panel();
            pnlResumen2 = new Panel();
            pnlResumen3 = new Panel();

            lblBalanceTitulo = new Label();
            lblBalanceValor = new Label();

            lblVencidoTitulo = new Label();
            lblVencidoValor = new Label();

            lblCobradoTitulo = new Label();
            lblCobradoValor = new Label();

            gridResumen = new DataGridView();

            ((System.ComponentModel.ISupportInitialize)gridResumen).BeginInit();
            SuspendLayout();

            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            ClientSize = new Size(1180, 720);
            FormBorderStyle = FormBorderStyle.None;
            Name = "FormCxCReportesGerenciales";
            Text = "Reportes gerenciales CxC";

            lblTitulo.AutoSize = true;
            lblTitulo.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            lblTitulo.ForeColor = Color.FromArgb(25, 35, 55);
            lblTitulo.Location = new Point(20, 20);
            lblTitulo.Text = "Reportes gerenciales CxC";

            lblSubtitulo.AutoSize = true;
            lblSubtitulo.Font = new Font("Segoe UI", 10F);
            lblSubtitulo.ForeColor = Color.FromArgb(90, 100, 120);
            lblSubtitulo.Location = new Point(20, 58);
            lblSubtitulo.Text = "Resumen ejecutivo de cartera y cobranza.";

            pnlResumen1.BackColor = Color.FromArgb(245, 250, 255);
            pnlResumen1.BorderStyle = BorderStyle.FixedSingle;
            pnlResumen1.Location = new Point(20, 100);
            pnlResumen1.Size = new Size(220, 90);

            lblBalanceTitulo.AutoSize = true;
            lblBalanceTitulo.Text = "Balance actual";
            lblBalanceTitulo.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblBalanceTitulo.Location = new Point(14, 14);

            lblBalanceValor.AutoSize = true;
            lblBalanceValor.Text = "RD$ 0.00";
            lblBalanceValor.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            lblBalanceValor.Location = new Point(14, 38);

            pnlResumen1.Controls.Add(lblBalanceTitulo);
            pnlResumen1.Controls.Add(lblBalanceValor);

            pnlResumen2.BackColor = Color.FromArgb(255, 248, 248);
            pnlResumen2.BorderStyle = BorderStyle.FixedSingle;
            pnlResumen2.Location = new Point(255, 100);
            pnlResumen2.Size = new Size(220, 90);

            lblVencidoTitulo.AutoSize = true;
            lblVencidoTitulo.Text = "Balance vencido";
            lblVencidoTitulo.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblVencidoTitulo.Location = new Point(14, 14);

            lblVencidoValor.AutoSize = true;
            lblVencidoValor.Text = "RD$ 0.00";
            lblVencidoValor.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            lblVencidoValor.Location = new Point(14, 38);

            pnlResumen2.Controls.Add(lblVencidoTitulo);
            pnlResumen2.Controls.Add(lblVencidoValor);

            pnlResumen3.BackColor = Color.FromArgb(245, 255, 245);
            pnlResumen3.BorderStyle = BorderStyle.FixedSingle;
            pnlResumen3.Location = new Point(490, 100);
            pnlResumen3.Size = new Size(220, 90);

            lblCobradoTitulo.AutoSize = true;
            lblCobradoTitulo.Text = "Total cobrado";
            lblCobradoTitulo.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblCobradoTitulo.Location = new Point(14, 14);

            lblCobradoValor.AutoSize = true;
            lblCobradoValor.Text = "RD$ 0.00";
            lblCobradoValor.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            lblCobradoValor.Location = new Point(14, 38);

            pnlResumen3.Controls.Add(lblCobradoTitulo);
            pnlResumen3.Controls.Add(lblCobradoValor);

            gridResumen.Location = new Point(20, 220);
            gridResumen.Size = new Size(1140, 450);
            gridResumen.AllowUserToAddRows = false;
            gridResumen.AllowUserToDeleteRows = false;
            gridResumen.RowHeadersVisible = false;
            gridResumen.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            gridResumen.BackgroundColor = Color.White;

            gridResumen.Columns.Add(new DataGridViewTextBoxColumn { Name = "colCliente", HeaderText = "Cliente" });
            gridResumen.Columns.Add(new DataGridViewTextBoxColumn { Name = "colBalance", HeaderText = "Balance" });
            gridResumen.Columns.Add(new DataGridViewTextBoxColumn { Name = "colVencido", HeaderText = "Vencido" });
            gridResumen.Columns.Add(new DataGridViewTextBoxColumn { Name = "colNoVencido", HeaderText = "No vencido" });
            gridResumen.Columns.Add(new DataGridViewTextBoxColumn { Name = "colUltimoCobro", HeaderText = "Último cobro" });

            Controls.Add(lblTitulo);
            Controls.Add(lblSubtitulo);
            Controls.Add(pnlResumen1);
            Controls.Add(pnlResumen2);
            Controls.Add(pnlResumen3);
            Controls.Add(gridResumen);

            ((System.ComponentModel.ISupportInitialize)gridResumen).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }
    }
}