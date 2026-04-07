namespace Andloe.Presentacion
{
    partial class FormAlanubeMonitor
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();

            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            panelTop = new System.Windows.Forms.Panel();
            lblEstado = new System.Windows.Forms.Label();
            cboFiltroEstado = new System.Windows.Forms.ComboBox();
            btnRefrescar = new System.Windows.Forms.Button();
            btnEnviar = new System.Windows.Forms.Button();
            btnConsultar = new System.Windows.Forms.Button();
            btnAbrirFactura = new System.Windows.Forms.Button();
            btnCopiarEncf = new System.Windows.Forms.Button();
            btnCopiarTrackId = new System.Windows.Forms.Button();
            btnVerJson = new System.Windows.Forms.Button();
            grid = new System.Windows.Forms.DataGridView();
            panelTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)grid).BeginInit();
            SuspendLayout();
            // 
            // panelTop
            // 
            panelTop.Controls.Add(lblEstado);
            panelTop.Controls.Add(cboFiltroEstado);
            panelTop.Controls.Add(btnRefrescar);
            panelTop.Controls.Add(btnEnviar);
            panelTop.Controls.Add(btnConsultar);
            panelTop.Controls.Add(btnAbrirFactura);
            panelTop.Controls.Add(btnCopiarEncf);
            panelTop.Controls.Add(btnCopiarTrackId);
            panelTop.Controls.Add(btnVerJson);
            panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            panelTop.Location = new System.Drawing.Point(0, 0);
            panelTop.Name = "panelTop";
            panelTop.Size = new System.Drawing.Size(1284, 74);
            panelTop.TabIndex = 0;
            // 
            // lblEstado
            // 
            lblEstado.AutoSize = true;
            lblEstado.Location = new System.Drawing.Point(14, 15);
            lblEstado.Name = "lblEstado";
            lblEstado.Size = new System.Drawing.Size(98, 23);
            lblEstado.TabIndex = 0;
            lblEstado.Text = "Filtrar estado";
            // 
            // cboFiltroEstado
            // 
            cboFiltroEstado.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cboFiltroEstado.FormattingEnabled = true;
            cboFiltroEstado.Location = new System.Drawing.Point(118, 12);
            cboFiltroEstado.Name = "cboFiltroEstado";
            cboFiltroEstado.Size = new System.Drawing.Size(169, 31);
            cboFiltroEstado.TabIndex = 1;
            // 
            // btnRefrescar
            // 
            btnRefrescar.Location = new System.Drawing.Point(303, 10);
            btnRefrescar.Name = "btnRefrescar";
            btnRefrescar.Size = new System.Drawing.Size(117, 36);
            btnRefrescar.TabIndex = 2;
            btnRefrescar.Text = "Refrescar";
            btnRefrescar.UseVisualStyleBackColor = true;
            // 
            // btnEnviar
            // 
            btnEnviar.Location = new System.Drawing.Point(437, 10);
            btnEnviar.Name = "btnEnviar";
            btnEnviar.Size = new System.Drawing.Size(133, 36);
            btnEnviar.TabIndex = 3;
            btnEnviar.Text = "Enviar Alanube";
            btnEnviar.UseVisualStyleBackColor = true;
            // 
            // btnConsultar
            // 
            btnConsultar.Location = new System.Drawing.Point(586, 10);
            btnConsultar.Name = "btnConsultar";
            btnConsultar.Size = new System.Drawing.Size(178, 36);
            btnConsultar.TabIndex = 4;
            btnConsultar.Text = "Consultar Estado";
            btnConsultar.UseVisualStyleBackColor = true;
            // 
            // btnAbrirFactura
            // 
            btnAbrirFactura.Location = new System.Drawing.Point(780, 10);
            btnAbrirFactura.Name = "btnAbrirFactura";
            btnAbrirFactura.Size = new System.Drawing.Size(122, 36);
            btnAbrirFactura.TabIndex = 5;
            btnAbrirFactura.Text = "Abrir Factura";
            btnAbrirFactura.UseVisualStyleBackColor = true;
            // 
            // btnCopiarEncf
            // 
            btnCopiarEncf.Location = new System.Drawing.Point(918, 10);
            btnCopiarEncf.Name = "btnCopiarEncf";
            btnCopiarEncf.Size = new System.Drawing.Size(110, 36);
            btnCopiarEncf.TabIndex = 6;
            btnCopiarEncf.Text = "Copiar eNCF";
            btnCopiarEncf.UseVisualStyleBackColor = true;
            // 
            // btnCopiarTrackId
            // 
            btnCopiarTrackId.Location = new System.Drawing.Point(1042, 10);
            btnCopiarTrackId.Name = "btnCopiarTrackId";
            btnCopiarTrackId.Size = new System.Drawing.Size(129, 36);
            btnCopiarTrackId.TabIndex = 7;
            btnCopiarTrackId.Text = "Copiar TrackId";
            btnCopiarTrackId.UseVisualStyleBackColor = true;
            // 
            // btnVerJson
            // 
            btnVerJson.Location = new System.Drawing.Point(1180, 10);
            btnVerJson.Name = "btnVerJson";
            btnVerJson.Size = new System.Drawing.Size(90, 36);
            btnVerJson.TabIndex = 8;
            btnVerJson.Text = "Ver JSON";
            btnVerJson.UseVisualStyleBackColor = true;
            // 
            // grid
            // 
            grid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            grid.Dock = System.Windows.Forms.DockStyle.Fill;
            grid.Location = new System.Drawing.Point(0, 74);
            grid.Name = "grid";
            grid.RowHeadersWidth = 51;
            grid.RowTemplate.Height = 29;
            grid.Size = new System.Drawing.Size(1284, 587);
            grid.TabIndex = 1;
            // 
            // FormAlanubeMonitor
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(9F, 23F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1284, 661);
            Controls.Add(grid);
            Controls.Add(panelTop);
            Name = "FormAlanubeMonitor";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "Monitor Alanube API";
            panelTop.ResumeLayout(false);
            panelTop.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)grid).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Label lblEstado;
        private System.Windows.Forms.ComboBox cboFiltroEstado;
        private System.Windows.Forms.Button btnRefrescar;
        private System.Windows.Forms.Button btnEnviar;
        private System.Windows.Forms.Button btnConsultar;
        private System.Windows.Forms.Button btnAbrirFactura;
        private System.Windows.Forms.Button btnCopiarEncf;
        private System.Windows.Forms.Button btnCopiarTrackId;
        private System.Windows.Forms.Button btnVerJson;
        private System.Windows.Forms.DataGridView grid;
    }
}