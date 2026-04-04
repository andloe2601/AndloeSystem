using System.Drawing;
using System.Windows.Forms;

namespace Andloe.Presentacion
{
    partial class FormCxCEstadoCuenta
    {
        private System.ComponentModel.IContainer components = null;

        private TextBox txtBuscarCliente;
        private Button btnBuscarCliente;
        private ComboBox cboCliente;
        private DateTimePicker dtpDesde;
        private DateTimePicker dtpHasta;
        private Button btnConsultar;
        private Button btnExportarExcel;
        private Button btnImprimir;
        private DataGridView grid;

        private Label lblTotalFacturadoValor;
        private Label lblTotalCobradoValor;
        private Label lblBalanceValor;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
                components.Dispose();

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();

            txtBuscarCliente = new TextBox();
            btnBuscarCliente = new Button();
            cboCliente = new ComboBox();
            dtpDesde = new DateTimePicker();
            dtpHasta = new DateTimePicker();
            btnConsultar = new Button();
            btnExportarExcel = new Button();
            btnImprimir = new Button();
            grid = new DataGridView();

            lblTotalFacturadoValor = new Label();
            lblTotalCobradoValor = new Label();
            lblBalanceValor = new Label();

            ((System.ComponentModel.ISupportInitialize)grid).BeginInit();
            SuspendLayout();

            ClientSize = new Size(1280, 760);
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Estado de Cuenta de Cliente";
            BackColor = Color.White;

            Controls.Add(new Label
            {
                Text = "Cliente",
                Location = new Point(16, 18),
                AutoSize = true,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            });

            txtBuscarCliente.Location = new Point(16, 42);
            txtBuscarCliente.Size = new Size(210, 23);

            btnBuscarCliente.Location = new Point(232, 41);
            btnBuscarCliente.Size = new Size(78, 25);
            btnBuscarCliente.Text = "Buscar";

            cboCliente.Location = new Point(16, 74);
            cboCliente.Size = new Size(360, 23);
            cboCliente.DropDownStyle = ComboBoxStyle.DropDownList;

            Controls.Add(new Label
            {
                Text = "Desde",
                Location = new Point(410, 18),
                AutoSize = true,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            });

            dtpDesde.Location = new Point(410, 42);
            dtpDesde.Size = new Size(150, 23);
            dtpDesde.Format = DateTimePickerFormat.Short;

            Controls.Add(new Label
            {
                Text = "Hasta",
                Location = new Point(580, 18),
                AutoSize = true,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            });

            dtpHasta.Location = new Point(580, 42);
            dtpHasta.Size = new Size(150, 23);
            dtpHasta.Format = DateTimePickerFormat.Short;

            btnConsultar.Location = new Point(760, 40);
            btnConsultar.Size = new Size(100, 28);
            btnConsultar.Text = "Consultar";
            btnConsultar.BackColor = Color.FromArgb(33, 179, 164);
            btnConsultar.ForeColor = Color.White;
            btnConsultar.FlatStyle = FlatStyle.Flat;
            btnConsultar.FlatAppearance.BorderSize = 0;

            btnExportarExcel.Location = new Point(870, 40);
            btnExportarExcel.Size = new Size(130, 28);
            btnExportarExcel.Text = "Exportar Excel";
            btnExportarExcel.BackColor = Color.FromArgb(50, 120, 70);
            btnExportarExcel.ForeColor = Color.White;
            btnExportarExcel.FlatStyle = FlatStyle.Flat;
            btnExportarExcel.FlatAppearance.BorderSize = 0;

            btnImprimir.Location = new Point(1010, 40);
            btnImprimir.Size = new Size(100, 28);
            btnImprimir.Text = "Imprimir";
            btnImprimir.BackColor = Color.FromArgb(70, 90, 130);
            btnImprimir.ForeColor = Color.White;
            btnImprimir.FlatStyle = FlatStyle.Flat;
            btnImprimir.FlatAppearance.BorderSize = 0;

            grid.Location = new Point(16, 120);
            grid.Size = new Size(1248, 520);
            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.RowHeadersVisible = false;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            grid.BackgroundColor = Color.White;
            grid.BorderStyle = BorderStyle.FixedSingle;

            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colFecha",
                HeaderText = "Fecha",
                Width = 95,
                ReadOnly = true
            });

            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colTipo",
                HeaderText = "Tipo",
                Width = 95,
                ReadOnly = true
            });

            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colDocumento",
                HeaderText = "Documento",
                Width = 130,
                ReadOnly = true
            });

            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colReferencia",
                HeaderText = "Referencia",
                Width = 150,
                ReadOnly = true
            });

            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colDebe",
                HeaderText = "Debe",
                Width = 115,
                ReadOnly = true
            });

            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colHaber",
                HeaderText = "Haber",
                Width = 115,
                ReadOnly = true
            });

            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colBalance",
                HeaderText = "Balance",
                Width = 115,
                ReadOnly = true
            });

            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colDias",
                HeaderText = "Días vencidos",
                Width = 100,
                ReadOnly = true
            });

            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colDesc",
                HeaderText = "Descripción",
                Width = 400,
                ReadOnly = true,
                DefaultCellStyle = new DataGridViewCellStyle { WrapMode = DataGridViewTriState.True }
            });

            Controls.Add(new Label
            {
                Text = "Total facturado",
                Location = new Point(16, 665),
                AutoSize = true,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold)
            });

            lblTotalFacturadoValor.Location = new Point(16, 692);
            lblTotalFacturadoValor.Size = new Size(210, 30);
            lblTotalFacturadoValor.Font = new Font("Segoe UI", 13F, FontStyle.Bold);

            Controls.Add(new Label
            {
                Text = "Total cobrado",
                Location = new Point(280, 665),
                AutoSize = true,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold)
            });

            lblTotalCobradoValor.Location = new Point(280, 692);
            lblTotalCobradoValor.Size = new Size(210, 30);
            lblTotalCobradoValor.Font = new Font("Segoe UI", 13F, FontStyle.Bold);

            Controls.Add(new Label
            {
                Text = "Balance actual",
                Location = new Point(560, 665),
                AutoSize = true,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold)
            });

            lblBalanceValor.Location = new Point(560, 692);
            lblBalanceValor.Size = new Size(210, 30);
            lblBalanceValor.Font = new Font("Segoe UI", 13F, FontStyle.Bold);

            Controls.Add(txtBuscarCliente);
            Controls.Add(btnBuscarCliente);
            Controls.Add(cboCliente);
            Controls.Add(dtpDesde);
            Controls.Add(dtpHasta);
            Controls.Add(btnConsultar);
            Controls.Add(btnExportarExcel);
            Controls.Add(btnImprimir);
            Controls.Add(grid);
            Controls.Add(lblTotalFacturadoValor);
            Controls.Add(lblTotalCobradoValor);
            Controls.Add(lblBalanceValor);

            ((System.ComponentModel.ISupportInitialize)grid).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }
    }
}