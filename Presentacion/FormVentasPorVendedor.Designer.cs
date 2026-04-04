namespace Andloe.Presentacion
{
    partial class FormVentasPorVendedor
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.TableLayoutPanel layoutRoot;
        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblSubtitle;
        private System.Windows.Forms.Button btnExportarExcel;

        private System.Windows.Forms.Panel pnlFilters;
        private System.Windows.Forms.TableLayoutPanel layoutFilters;

        private System.Windows.Forms.Label lblDesde;
        private System.Windows.Forms.DateTimePicker dtpDesde;

        private System.Windows.Forms.Label lblHasta;
        private System.Windows.Forms.DateTimePicker dtpHasta;

        private System.Windows.Forms.Label lblVendedor;
        private System.Windows.Forms.ComboBox cboVendedor;

        private System.Windows.Forms.Button btnBuscar;
        private System.Windows.Forms.Button btnVerDetalle;

        private System.Windows.Forms.Panel pnlGrid;
        private System.Windows.Forms.DataGridView grid;

        private System.Windows.Forms.Panel pnlFooter;
        private System.Windows.Forms.TableLayoutPanel layoutFooter;
        private System.Windows.Forms.Panel cardFacturas;
        private System.Windows.Forms.Panel cardMonto;

        private System.Windows.Forms.Label lblTotFacturasTitle;
        private System.Windows.Forms.TextBox txtTotFacturas;

        private System.Windows.Forms.Label lblTotMontoTitle;
        private System.Windows.Forms.TextBox txtTotMonto;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            layoutRoot = new TableLayoutPanel();
            pnlHeader = new Panel();
            lblTitle = new Label();
            lblSubtitle = new Label();
            pnlFilters = new Panel();
            layoutFilters = new TableLayoutPanel();
            lblDesde = new Label();
            lblHasta = new Label();
            lblVendedor = new Label();
            dtpDesde = new DateTimePicker();
            dtpHasta = new DateTimePicker();
            cboVendedor = new ComboBox();
            btnBuscar = new Button();
            btnVerDetalle = new Button();
            btnExportarExcel = new Button();
            pnlGrid = new Panel();
            grid = new DataGridView();
            pnlFooter = new Panel();
            layoutFooter = new TableLayoutPanel();
            cardFacturas = new Panel();
            lblTotFacturasTitle = new Label();
            txtTotFacturas = new TextBox();
            cardMonto = new Panel();
            lblTotMontoTitle = new Label();
            txtTotMonto = new TextBox();
            layoutRoot.SuspendLayout();
            pnlHeader.SuspendLayout();
            pnlFilters.SuspendLayout();
            layoutFilters.SuspendLayout();
            pnlGrid.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)grid).BeginInit();
            pnlFooter.SuspendLayout();
            layoutFooter.SuspendLayout();
            cardFacturas.SuspendLayout();
            cardMonto.SuspendLayout();
            SuspendLayout();
            // 
            // layoutRoot
            // 
            layoutRoot.ColumnCount = 1;
            layoutRoot.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20F));
            layoutRoot.Controls.Add(pnlHeader, 0, 0);
            layoutRoot.Controls.Add(pnlFilters, 0, 1);
            layoutRoot.Controls.Add(pnlGrid, 0, 2);
            layoutRoot.Controls.Add(pnlFooter, 0, 3);
            layoutRoot.Dock = DockStyle.Fill;
            layoutRoot.Location = new Point(0, 0);
            layoutRoot.Name = "layoutRoot";
            layoutRoot.RowCount = 4;
            layoutRoot.RowStyles.Add(new RowStyle(SizeType.Absolute, 92F));
            layoutRoot.RowStyles.Add(new RowStyle(SizeType.Absolute, 123F));
            layoutRoot.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            layoutRoot.RowStyles.Add(new RowStyle(SizeType.Absolute, 96F));
            layoutRoot.Size = new Size(1200, 720);
            layoutRoot.TabIndex = 0;
            // 
            // pnlHeader
            // 
            pnlHeader.BackColor = Color.White;
            pnlHeader.Controls.Add(lblTitle);
            pnlHeader.Controls.Add(lblSubtitle);
            pnlHeader.Dock = DockStyle.Fill;
            pnlHeader.Location = new Point(16, 16);
            pnlHeader.Margin = new Padding(16, 16, 16, 8);
            pnlHeader.Name = "pnlHeader";
            pnlHeader.Padding = new Padding(18, 14, 18, 14);
            pnlHeader.Size = new Size(1168, 68);
            pnlHeader.TabIndex = 0;
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe UI Semibold", 16F, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(20, 20, 20);
            lblTitle.Location = new Point(18, 12);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(222, 30);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "Ventas por Vendedor";
            // 
            // lblSubtitle
            // 
            lblSubtitle.AutoSize = true;
            lblSubtitle.Font = new Font("Segoe UI", 9.5F);
            lblSubtitle.ForeColor = Color.FromArgb(110, 110, 110);
            lblSubtitle.Location = new Point(20, 48);
            lblSubtitle.Name = "lblSubtitle";
            lblSubtitle.Size = new Size(554, 17);
            lblSubtitle.TabIndex = 1;
            lblSubtitle.Text = "Filtra por rango de fecha y vendedor. Verás resumen y detalle de facturas FINALIZADAS (FAC).";
            // 
            // pnlFilters
            // 
            pnlFilters.BackColor = Color.White;
            pnlFilters.Controls.Add(layoutFilters);
            pnlFilters.Dock = DockStyle.Fill;
            pnlFilters.Location = new Point(16, 100);
            pnlFilters.Margin = new Padding(16, 8, 16, 8);
            pnlFilters.Name = "pnlFilters";
            pnlFilters.Padding = new Padding(14);
            pnlFilters.Size = new Size(1168, 107);
            pnlFilters.TabIndex = 1;
            // 
            // layoutFilters
            // 
            layoutFilters.ColumnCount = 11;
            layoutFilters.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 90F));
            layoutFilters.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 170F));
            layoutFilters.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20F));
            layoutFilters.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 90F));
            layoutFilters.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 170F));
            layoutFilters.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20F));
            layoutFilters.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 90F));
            layoutFilters.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            layoutFilters.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 128F));
            layoutFilters.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 127F));
            layoutFilters.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 84F));
            layoutFilters.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150F));
            layoutFilters.Controls.Add(lblDesde, 0, 0);
            layoutFilters.Controls.Add(lblHasta, 3, 0);
            layoutFilters.Controls.Add(lblVendedor, 6, 0);
            layoutFilters.Controls.Add(dtpDesde, 1, 1);
            layoutFilters.Controls.Add(dtpHasta, 4, 1);
            layoutFilters.Controls.Add(cboVendedor, 7, 1);
            layoutFilters.Controls.Add(btnBuscar, 8, 1);
            layoutFilters.Controls.Add(btnVerDetalle, 9, 1);
            layoutFilters.Controls.Add(btnExportarExcel, 10, 1);
            layoutFilters.Dock = DockStyle.Fill;
            layoutFilters.Location = new Point(14, 14);
            layoutFilters.Name = "layoutFilters";
            layoutFilters.RowCount = 2;
            layoutFilters.RowStyles.Add(new RowStyle(SizeType.Absolute, 23F));
            layoutFilters.RowStyles.Add(new RowStyle(SizeType.Absolute, 56F));
            layoutFilters.Size = new Size(1140, 79);
            layoutFilters.TabIndex = 0;
            // 
            // lblDesde
            // 
            lblDesde.AutoSize = true;
            lblDesde.Dock = DockStyle.Fill;
            lblDesde.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            lblDesde.ForeColor = Color.FromArgb(90, 90, 90);
            lblDesde.Location = new Point(3, 0);
            lblDesde.Name = "lblDesde";
            lblDesde.Size = new Size(84, 23);
            lblDesde.TabIndex = 0;
            lblDesde.Text = "Desde";
            lblDesde.TextAlign = ContentAlignment.BottomLeft;
            // 
            // lblHasta
            // 
            lblHasta.AutoSize = true;
            lblHasta.Dock = DockStyle.Fill;
            lblHasta.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            lblHasta.ForeColor = Color.FromArgb(90, 90, 90);
            lblHasta.Location = new Point(283, 0);
            lblHasta.Name = "lblHasta";
            lblHasta.Size = new Size(84, 23);
            lblHasta.TabIndex = 1;
            lblHasta.Text = "Hasta";
            lblHasta.TextAlign = ContentAlignment.BottomLeft;
            // 
            // lblVendedor
            // 
            lblVendedor.AutoSize = true;
            lblVendedor.Dock = DockStyle.Fill;
            lblVendedor.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            lblVendedor.ForeColor = Color.FromArgb(90, 90, 90);
            lblVendedor.Location = new Point(563, 0);
            lblVendedor.Name = "lblVendedor";
            lblVendedor.Size = new Size(84, 23);
            lblVendedor.TabIndex = 2;
            lblVendedor.Text = "Vendedor";
            lblVendedor.TextAlign = ContentAlignment.BottomLeft;
            // 
            // dtpDesde
            // 
            dtpDesde.Dock = DockStyle.Fill;
            dtpDesde.Font = new Font("Segoe UI", 10F);
            dtpDesde.Format = DateTimePickerFormat.Short;
            dtpDesde.Location = new Point(93, 26);
            dtpDesde.Name = "dtpDesde";
            dtpDesde.Size = new Size(164, 25);
            dtpDesde.TabIndex = 3;
            // 
            // dtpHasta
            // 
            dtpHasta.Dock = DockStyle.Fill;
            dtpHasta.Font = new Font("Segoe UI", 10F);
            dtpHasta.Format = DateTimePickerFormat.Short;
            dtpHasta.Location = new Point(373, 26);
            dtpHasta.Name = "dtpHasta";
            dtpHasta.Size = new Size(164, 25);
            dtpHasta.TabIndex = 4;
            // 
            // cboVendedor
            // 
            cboVendedor.Dock = DockStyle.Fill;
            cboVendedor.DropDownStyle = ComboBoxStyle.DropDownList;
            cboVendedor.Font = new Font("Segoe UI", 10F);
            cboVendedor.Location = new Point(653, 26);
            cboVendedor.Name = "cboVendedor";
            cboVendedor.Size = new Size(145, 25);
            cboVendedor.TabIndex = 5;
            // 
            // btnBuscar
            // 
            btnBuscar.BackColor = Color.FromArgb(44, 110, 203);
            btnBuscar.Dock = DockStyle.Fill;
            btnBuscar.FlatAppearance.BorderSize = 0;
            btnBuscar.FlatStyle = FlatStyle.Flat;
            btnBuscar.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            btnBuscar.ForeColor = Color.White;
            btnBuscar.Location = new Point(811, 45);
            btnBuscar.Margin = new Padding(10, 22, 6, 6);
            btnBuscar.Name = "btnBuscar";
            btnBuscar.Size = new Size(112, 28);
            btnBuscar.TabIndex = 6;
            btnBuscar.Text = "Buscar";
            btnBuscar.UseVisualStyleBackColor = false;
            // 
            // btnVerDetalle
            // 
            btnVerDetalle.BackColor = Color.White;
            btnVerDetalle.Dock = DockStyle.Fill;
            btnVerDetalle.FlatAppearance.BorderColor = Color.FromArgb(210, 210, 210);
            btnVerDetalle.FlatStyle = FlatStyle.Flat;
            btnVerDetalle.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            btnVerDetalle.ForeColor = Color.FromArgb(30, 30, 30);
            btnVerDetalle.Location = new Point(935, 45);
            btnVerDetalle.Margin = new Padding(6, 22, 0, 6);
            btnVerDetalle.Name = "btnVerDetalle";
            btnVerDetalle.Size = new Size(121, 28);
            btnVerDetalle.TabIndex = 7;
            btnVerDetalle.Text = "Ver detalle";
            btnVerDetalle.UseVisualStyleBackColor = false;
            // 
            // btnExportarExcel
            // 
            btnExportarExcel.BackColor = Color.White;
            btnExportarExcel.Dock = DockStyle.Fill;
            btnExportarExcel.FlatAppearance.BorderColor = Color.FromArgb(210, 210, 210);
            btnExportarExcel.FlatStyle = FlatStyle.Flat;
            btnExportarExcel.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            btnExportarExcel.ForeColor = Color.FromArgb(30, 30, 30);
            btnExportarExcel.Location = new Point(1062, 45);
            btnExportarExcel.Margin = new Padding(6, 22, 0, 6);
            btnExportarExcel.Name = "btnExportarExcel";
            btnExportarExcel.Size = new Size(78, 28);
            btnExportarExcel.TabIndex = 0;
            btnExportarExcel.Text = "Exportar Excel";
            btnExportarExcel.UseVisualStyleBackColor = false;
            // 
            // pnlGrid
            // 
            pnlGrid.BackColor = Color.White;
            pnlGrid.Controls.Add(grid);
            pnlGrid.Dock = DockStyle.Fill;
            pnlGrid.Location = new Point(16, 223);
            pnlGrid.Margin = new Padding(16, 8, 16, 8);
            pnlGrid.Name = "pnlGrid";
            pnlGrid.Padding = new Padding(12);
            pnlGrid.Size = new Size(1168, 393);
            pnlGrid.TabIndex = 2;
            // 
            // grid
            // 
            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.AllowUserToResizeRows = false;
            grid.BackgroundColor = Color.White;
            grid.BorderStyle = BorderStyle.None;
            grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = Color.FromArgb(245, 246, 248);
            dataGridViewCellStyle1.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            dataGridViewCellStyle1.ForeColor = Color.FromArgb(55, 55, 55);
            dataGridViewCellStyle1.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = DataGridViewTriState.True;
            grid.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            grid.ColumnHeadersHeight = 40;
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = SystemColors.Window;
            dataGridViewCellStyle2.Font = new Font("Segoe UI", 10F);
            dataGridViewCellStyle2.ForeColor = Color.FromArgb(35, 35, 35);
            dataGridViewCellStyle2.SelectionBackColor = Color.FromArgb(220, 235, 255);
            dataGridViewCellStyle2.SelectionForeColor = Color.FromArgb(15, 15, 15);
            dataGridViewCellStyle2.WrapMode = DataGridViewTriState.False;
            grid.DefaultCellStyle = dataGridViewCellStyle2;
            grid.Dock = DockStyle.Fill;
            grid.EnableHeadersVisualStyles = false;
            grid.GridColor = Color.FromArgb(235, 235, 235);
            grid.Location = new Point(12, 12);
            grid.MultiSelect = false;
            grid.Name = "grid";
            grid.ReadOnly = true;
            grid.RowHeadersVisible = false;
            grid.RowTemplate.Height = 34;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.Size = new Size(1144, 369);
            grid.TabIndex = 0;
            // 
            // pnlFooter
            // 
            pnlFooter.BackColor = Color.Transparent;
            pnlFooter.Controls.Add(layoutFooter);
            pnlFooter.Dock = DockStyle.Fill;
            pnlFooter.Location = new Point(16, 632);
            pnlFooter.Margin = new Padding(16, 8, 16, 16);
            pnlFooter.Name = "pnlFooter";
            pnlFooter.Size = new Size(1168, 72);
            pnlFooter.TabIndex = 3;
            // 
            // layoutFooter
            // 
            layoutFooter.ColumnCount = 2;
            layoutFooter.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            layoutFooter.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            layoutFooter.Controls.Add(cardFacturas, 0, 0);
            layoutFooter.Controls.Add(cardMonto, 1, 0);
            layoutFooter.Dock = DockStyle.Fill;
            layoutFooter.Location = new Point(0, 0);
            layoutFooter.Name = "layoutFooter";
            layoutFooter.RowCount = 1;
            layoutFooter.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            layoutFooter.Size = new Size(1168, 72);
            layoutFooter.TabIndex = 0;
            // 
            // cardFacturas
            // 
            cardFacturas.BackColor = Color.White;
            cardFacturas.Controls.Add(lblTotFacturasTitle);
            cardFacturas.Controls.Add(txtTotFacturas);
            cardFacturas.Dock = DockStyle.Fill;
            cardFacturas.Location = new Point(0, 0);
            cardFacturas.Margin = new Padding(0, 0, 8, 0);
            cardFacturas.Name = "cardFacturas";
            cardFacturas.Padding = new Padding(16);
            cardFacturas.Size = new Size(576, 72);
            cardFacturas.TabIndex = 0;
            // 
            // lblTotFacturasTitle
            // 
            lblTotFacturasTitle.AutoSize = true;
            lblTotFacturasTitle.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            lblTotFacturasTitle.ForeColor = Color.FromArgb(90, 90, 90);
            lblTotFacturasTitle.Location = new Point(16, 12);
            lblTotFacturasTitle.Name = "lblTotFacturasTitle";
            lblTotFacturasTitle.Size = new Size(113, 19);
            lblTotFacturasTitle.TabIndex = 0;
            lblTotFacturasTitle.Text = "Total de facturas";
            // 
            // txtTotFacturas
            // 
            txtTotFacturas.BackColor = Color.White;
            txtTotFacturas.BorderStyle = BorderStyle.None;
            txtTotFacturas.Font = new Font("Segoe UI Semibold", 22F, FontStyle.Bold);
            txtTotFacturas.ForeColor = Color.FromArgb(20, 20, 20);
            txtTotFacturas.Location = new Point(16, 40);
            txtTotFacturas.Name = "txtTotFacturas";
            txtTotFacturas.ReadOnly = true;
            txtTotFacturas.Size = new Size(400, 40);
            txtTotFacturas.TabIndex = 1;
            txtTotFacturas.Text = "0";
            // 
            // cardMonto
            // 
            cardMonto.BackColor = Color.White;
            cardMonto.Controls.Add(lblTotMontoTitle);
            cardMonto.Controls.Add(txtTotMonto);
            cardMonto.Dock = DockStyle.Fill;
            cardMonto.Location = new Point(592, 0);
            cardMonto.Margin = new Padding(8, 0, 0, 0);
            cardMonto.Name = "cardMonto";
            cardMonto.Padding = new Padding(16);
            cardMonto.Size = new Size(576, 72);
            cardMonto.TabIndex = 1;
            // 
            // lblTotMontoTitle
            // 
            lblTotMontoTitle.AutoSize = true;
            lblTotMontoTitle.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            lblTotMontoTitle.ForeColor = Color.FromArgb(90, 90, 90);
            lblTotMontoTitle.Location = new Point(16, 12);
            lblTotMontoTitle.Name = "lblTotMontoTitle";
            lblTotMontoTitle.Size = new Size(94, 19);
            lblTotMontoTitle.TabIndex = 0;
            lblTotMontoTitle.Text = "Total vendido";
            // 
            // txtTotMonto
            // 
            txtTotMonto.BackColor = Color.White;
            txtTotMonto.BorderStyle = BorderStyle.None;
            txtTotMonto.Font = new Font("Segoe UI Semibold", 22F, FontStyle.Bold);
            txtTotMonto.ForeColor = Color.FromArgb(20, 20, 20);
            txtTotMonto.Location = new Point(16, 40);
            txtTotMonto.Name = "txtTotMonto";
            txtTotMonto.ReadOnly = true;
            txtTotMonto.Size = new Size(500, 40);
            txtTotMonto.TabIndex = 1;
            txtTotMonto.Text = "0.00";
            // 
            // FormVentasPorVendedor
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(245, 246, 248);
            ClientSize = new Size(1200, 720);
            Controls.Add(layoutRoot);
            Font = new Font("Segoe UI", 9F);
            MinimumSize = new Size(1050, 650);
            Name = "FormVentasPorVendedor";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Ventas por Vendedor";
            layoutRoot.ResumeLayout(false);
            pnlHeader.ResumeLayout(false);
            pnlHeader.PerformLayout();
            pnlFilters.ResumeLayout(false);
            layoutFilters.ResumeLayout(false);
            layoutFilters.PerformLayout();
            pnlGrid.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)grid).EndInit();
            pnlFooter.ResumeLayout(false);
            layoutFooter.ResumeLayout(false);
            cardFacturas.ResumeLayout(false);
            cardFacturas.PerformLayout();
            cardMonto.ResumeLayout(false);
            cardMonto.PerformLayout();
            ResumeLayout(false);
        }
    }
}