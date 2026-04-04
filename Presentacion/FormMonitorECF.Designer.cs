namespace Andloe.Presentacion
{
    partial class FormMonitorECF
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.ComboBox cboEstado;
        private System.Windows.Forms.Button btnRefrescar;
        private System.Windows.Forms.Button btnVerXml;
        private System.Windows.Forms.Button btnRegenerarXml;
        private System.Windows.Forms.Button btnCopiarEncf;
        private System.Windows.Forms.Button btnAbrirFactura;
        private System.Windows.Forms.Button btnFirmar;
        private System.Windows.Forms.Button btnEnviar;
        private System.Windows.Forms.Button btnConsultar;
        private System.Windows.Forms.DataGridView grid;
        private System.Windows.Forms.Button btnEnviarAlanube;
        private System.Windows.Forms.Button btnConsultarAlanube;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.panelTop = new System.Windows.Forms.Panel();
            this.cboEstado = new System.Windows.Forms.ComboBox();
            this.btnRefrescar = new System.Windows.Forms.Button();
            this.btnVerXml = new System.Windows.Forms.Button();
            this.btnRegenerarXml = new System.Windows.Forms.Button();
            this.btnCopiarEncf = new System.Windows.Forms.Button();
            this.btnAbrirFactura = new System.Windows.Forms.Button();
            this.btnFirmar = new System.Windows.Forms.Button();
            this.btnEnviar = new System.Windows.Forms.Button();
            this.btnConsultar = new System.Windows.Forms.Button();
            this.btnEnviarAlanube = new System.Windows.Forms.Button();
            this.btnConsultarAlanube = new System.Windows.Forms.Button();
            this.grid = new System.Windows.Forms.DataGridView();

            this.panelTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grid)).BeginInit();
            this.SuspendLayout();

            // panelTop
            this.panelTop.BackColor = System.Drawing.Color.FromArgb(250, 250, 252);
            this.panelTop.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Height = 64;
            this.panelTop.Padding = new System.Windows.Forms.Padding(10, 8, 10, 8);

            // cboEstado
            this.cboEstado.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboEstado.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cboEstado.FormattingEnabled = true;
            this.cboEstado.Items.AddRange(new object[]
            {
                "PENDIENTE",
                "FIRMADO",
                "ENVIADO",
                "ACEPTADO",
                "RECHAZADO",
                "ERROR",
                "TODOS"
            });
            this.cboEstado.Left = 12;
            this.cboEstado.Top = 16;
            this.cboEstado.Width = 165;
            this.cboEstado.Name = "cboEstado";

            // btnRefrescar
            this.btnRefrescar.Text = "Refrescar";
            this.btnRefrescar.Name = "btnRefrescar";
            this.btnRefrescar.Left = 188;
            this.btnRefrescar.Top = 14;
            this.btnRefrescar.Width = 105;
            this.btnRefrescar.Height = 34;
            this.btnRefrescar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRefrescar.BackColor = System.Drawing.Color.White;
            this.btnRefrescar.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(210, 210, 210);
            this.btnRefrescar.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnRefrescar.UseVisualStyleBackColor = false;

            // btnVerXml
            this.btnVerXml.Text = "Ver XML";
            this.btnVerXml.Name = "btnVerXml";
            this.btnVerXml.Left = 300;
            this.btnVerXml.Top = 14;
            this.btnVerXml.Width = 105;
            this.btnVerXml.Height = 34;
            this.btnVerXml.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnVerXml.BackColor = System.Drawing.Color.White;
            this.btnVerXml.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(210, 210, 210);
            this.btnVerXml.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnVerXml.UseVisualStyleBackColor = false;

            // btnRegenerarXml
            this.btnRegenerarXml.Text = "Regenerar XML";
            this.btnRegenerarXml.Name = "btnRegenerarXml";
            this.btnRegenerarXml.Left = 412;
            this.btnRegenerarXml.Top = 14;
            this.btnRegenerarXml.Width = 138;
            this.btnRegenerarXml.Height = 34;
            this.btnRegenerarXml.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRegenerarXml.BackColor = System.Drawing.Color.White;
            this.btnRegenerarXml.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(210, 210, 210);
            this.btnRegenerarXml.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnRegenerarXml.UseVisualStyleBackColor = false;

            // btnCopiarEncf
            this.btnCopiarEncf.Text = "Copiar eNCF";
            this.btnCopiarEncf.Name = "btnCopiarEncf";
            this.btnCopiarEncf.Left = 557;
            this.btnCopiarEncf.Top = 14;
            this.btnCopiarEncf.Width = 128;
            this.btnCopiarEncf.Height = 34;
            this.btnCopiarEncf.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCopiarEncf.BackColor = System.Drawing.Color.White;
            this.btnCopiarEncf.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(210, 210, 210);
            this.btnCopiarEncf.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnCopiarEncf.UseVisualStyleBackColor = false;

            // btnAbrirFactura
            this.btnAbrirFactura.Text = "Abrir Factura";
            this.btnAbrirFactura.Name = "btnAbrirFactura";
            this.btnAbrirFactura.Left = 692;
            this.btnAbrirFactura.Top = 14;
            this.btnAbrirFactura.Width = 128;
            this.btnAbrirFactura.Height = 34;
            this.btnAbrirFactura.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAbrirFactura.BackColor = System.Drawing.Color.White;
            this.btnAbrirFactura.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(210, 210, 210);
            this.btnAbrirFactura.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnAbrirFactura.UseVisualStyleBackColor = false;

            // btnFirmar
            this.btnFirmar.Text = "Firmar";
            this.btnFirmar.Name = "btnFirmar";
            this.btnFirmar.Left = 827;
            this.btnFirmar.Top = 14;
            this.btnFirmar.Width = 105;
            this.btnFirmar.Height = 34;
            this.btnFirmar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFirmar.BackColor = System.Drawing.Color.White;
            this.btnFirmar.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(210, 210, 210);
            this.btnFirmar.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnFirmar.UseVisualStyleBackColor = false;

            // btnEnviar
            this.btnEnviar.Text = "Enviar";
            this.btnEnviar.Name = "btnEnviar";
            this.btnEnviar.Left = 939;
            this.btnEnviar.Top = 14;
            this.btnEnviar.Width = 105;
            this.btnEnviar.Height = 34;
            this.btnEnviar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEnviar.BackColor = System.Drawing.Color.White;
            this.btnEnviar.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(210, 210, 210);
            this.btnEnviar.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnEnviar.UseVisualStyleBackColor = false;

            // btnConsultar
            this.btnConsultar.Text = "Consultar";
            this.btnConsultar.Name = "btnConsultar";
            this.btnConsultar.Left = 1051;
            this.btnConsultar.Top = 14;
            this.btnConsultar.Width = 110;
            this.btnConsultar.Height = 34;
            this.btnConsultar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnConsultar.BackColor = System.Drawing.Color.White;
            this.btnConsultar.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(210, 210, 210);
            this.btnConsultar.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnConsultar.UseVisualStyleBackColor = false;

            // btnEnviarAlanube
            this.btnEnviarAlanube.Text = "Enviar Alanube";
            this.btnEnviarAlanube.Name = "btnEnviarAlanube";
            this.btnEnviarAlanube.Left = 1168;
            this.btnEnviarAlanube.Top = 14;
            this.btnEnviarAlanube.Width = 130;
            this.btnEnviarAlanube.Height = 34;
            this.btnEnviarAlanube.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEnviarAlanube.BackColor = System.Drawing.Color.White;
            this.btnEnviarAlanube.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(210, 210, 210);
            this.btnEnviarAlanube.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnEnviarAlanube.UseVisualStyleBackColor = false;

            // btnConsultarAlanube
            this.btnConsultarAlanube.Text = "Consultar Alanube";
            this.btnConsultarAlanube.Name = "btnConsultarAlanube";
            this.btnConsultarAlanube.Left = 1305;
            this.btnConsultarAlanube.Top = 14;
            this.btnConsultarAlanube.Width = 145;
            this.btnConsultarAlanube.Height = 34;
            this.btnConsultarAlanube.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnConsultarAlanube.BackColor = System.Drawing.Color.White;
            this.btnConsultarAlanube.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(210, 210, 210);
            this.btnConsultarAlanube.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnConsultarAlanube.UseVisualStyleBackColor = false;

            // panelTop controls
            this.panelTop.Controls.Add(this.cboEstado);
            this.panelTop.Controls.Add(this.btnRefrescar);
            this.panelTop.Controls.Add(this.btnVerXml);
            this.panelTop.Controls.Add(this.btnRegenerarXml);
            this.panelTop.Controls.Add(this.btnCopiarEncf);
            this.panelTop.Controls.Add(this.btnAbrirFactura);
            this.panelTop.Controls.Add(this.btnFirmar);
            this.panelTop.Controls.Add(this.btnEnviar);
            this.panelTop.Controls.Add(this.btnConsultar);
            this.panelTop.Controls.Add(this.btnEnviarAlanube);
            this.panelTop.Controls.Add(this.btnConsultarAlanube);

            // grid
            this.grid.AllowUserToAddRows = false;
            this.grid.AllowUserToDeleteRows = false;
            this.grid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells;
            this.grid.BackgroundColor = System.Drawing.Color.White;
            this.grid.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.grid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grid.Location = new System.Drawing.Point(0, 64);
            this.grid.MultiSelect = false;
            this.grid.Name = "grid";
            this.grid.ReadOnly = true;
            this.grid.RowHeadersVisible = false;
            this.grid.RowTemplate.Height = 30;
            this.grid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.grid.TabIndex = 1;

            // FormMonitorECF
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1470, 650);
            this.Controls.Add(this.grid);
            this.Controls.Add(this.panelTop);
            this.Name = "FormMonitorECF";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Monitor e-CF (DGII) - Cola";

            this.panelTop.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grid)).EndInit();
            this.ResumeLayout(false);
        }
    }
}