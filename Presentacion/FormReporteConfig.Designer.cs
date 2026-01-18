namespace Andloe.Presentacion
{
    partial class FormReporteConfig
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
            this.pnlTop = new System.Windows.Forms.Panel();
            this.btnRecargar = new System.Windows.Forms.Button();
            this.gbScope = new System.Windows.Forms.GroupBox();
            this.rbUsuario = new System.Windows.Forms.RadioButton();
            this.rbSucursal = new System.Windows.Forms.RadioButton();
            this.rbEmpresa = new System.Windows.Forms.RadioButton();
            this.lblActividad = new System.Windows.Forms.Label();
            this.cboActividad = new System.Windows.Forms.ComboBox();
            this.lblModulo = new System.Windows.Forms.Label();
            this.cboModulo = new System.Windows.Forms.ComboBox();
            this.splitMain = new System.Windows.Forms.SplitContainer();
            this.gbDef = new System.Windows.Forms.GroupBox();
            this.gridDef = new System.Windows.Forms.DataGridView();
            this.gbAsig = new System.Windows.Forms.GroupBox();
            this.gridAsignaciones = new System.Windows.Forms.DataGridView();
            this.pnlBottom = new System.Windows.Forms.Panel();
            this.btnGuardarAsignacion = new System.Windows.Forms.Button();
            this.numPrioridad = new System.Windows.Forms.NumericUpDown();
            this.lblPrioridad = new System.Windows.Forms.Label();
            this.numOrden = new System.Windows.Forms.NumericUpDown();
            this.lblOrden = new System.Windows.Forms.Label();
            this.chkDefault = new System.Windows.Forms.CheckBox();
            this.chkActivo = new System.Windows.Forms.CheckBox();
            this.pnlTop.SuspendLayout();
            this.gbScope.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitMain)).BeginInit();
            this.splitMain.Panel1.SuspendLayout();
            this.splitMain.Panel2.SuspendLayout();
            this.splitMain.SuspendLayout();
            this.gbDef.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridDef)).BeginInit();
            this.gbAsig.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridAsignaciones)).BeginInit();
            this.pnlBottom.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numPrioridad)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numOrden)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlTop
            // 
            this.pnlTop.Controls.Add(this.btnRecargar);
            this.pnlTop.Controls.Add(this.gbScope);
            this.pnlTop.Controls.Add(this.lblActividad);
            this.pnlTop.Controls.Add(this.cboActividad);
            this.pnlTop.Controls.Add(this.lblModulo);
            this.pnlTop.Controls.Add(this.cboModulo);
            this.pnlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTop.Location = new System.Drawing.Point(0, 0);
            this.pnlTop.Name = "pnlTop";
            this.pnlTop.Padding = new System.Windows.Forms.Padding(10);
            this.pnlTop.Size = new System.Drawing.Size(1240, 86);
            this.pnlTop.TabIndex = 0;
            // 
            // btnRecargar
            // 
            this.btnRecargar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRecargar.Location = new System.Drawing.Point(1134, 26);
            this.btnRecargar.Name = "btnRecargar";
            this.btnRecargar.Size = new System.Drawing.Size(94, 34);
            this.btnRecargar.TabIndex = 5;
            this.btnRecargar.Text = "Recargar";
            this.btnRecargar.UseVisualStyleBackColor = true;
            // 
            // gbScope
            // 
            this.gbScope.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.gbScope.Controls.Add(this.rbUsuario);
            this.gbScope.Controls.Add(this.rbSucursal);
            this.gbScope.Controls.Add(this.rbEmpresa);
            this.gbScope.Location = new System.Drawing.Point(740, 10);
            this.gbScope.Name = "gbScope";
            this.gbScope.Size = new System.Drawing.Size(380, 66);
            this.gbScope.TabIndex = 4;
            this.gbScope.TabStop = false;
            this.gbScope.Text = "Alcance";
            // 
            // rbUsuario
            // 
            this.rbUsuario.AutoSize = true;
            this.rbUsuario.Location = new System.Drawing.Point(260, 29);
            this.rbUsuario.Name = "rbUsuario";
            this.rbUsuario.Size = new System.Drawing.Size(72, 19);
            this.rbUsuario.TabIndex = 2;
            this.rbUsuario.TabStop = true;
            this.rbUsuario.Text = "Usuario";
            this.rbUsuario.UseVisualStyleBackColor = true;
            // 
            // rbSucursal
            // 
            this.rbSucursal.AutoSize = true;
            this.rbSucursal.Location = new System.Drawing.Point(145, 29);
            this.rbSucursal.Name = "rbSucursal";
            this.rbSucursal.Size = new System.Drawing.Size(70, 19);
            this.rbSucursal.TabIndex = 1;
            this.rbSucursal.TabStop = true;
            this.rbSucursal.Text = "Sucursal";
            this.rbSucursal.UseVisualStyleBackColor = true;
            // 
            // rbEmpresa
            // 
            this.rbEmpresa.AutoSize = true;
            this.rbEmpresa.Location = new System.Drawing.Point(30, 29);
            this.rbEmpresa.Name = "rbEmpresa";
            this.rbEmpresa.Size = new System.Drawing.Size(70, 19);
            this.rbEmpresa.TabIndex = 0;
            this.rbEmpresa.TabStop = true;
            this.rbEmpresa.Text = "Empresa";
            this.rbEmpresa.UseVisualStyleBackColor = true;
            // 
            // lblActividad
            // 
            this.lblActividad.AutoSize = true;
            this.lblActividad.Location = new System.Drawing.Point(256, 16);
            this.lblActividad.Name = "lblActividad";
            this.lblActividad.Size = new System.Drawing.Size(58, 15);
            this.lblActividad.TabIndex = 2;
            this.lblActividad.Text = "Actividad";
            // 
            // cboActividad
            // 
            this.cboActividad.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboActividad.FormattingEnabled = true;
            this.cboActividad.Location = new System.Drawing.Point(259, 36);
            this.cboActividad.Name = "cboActividad";
            this.cboActividad.Size = new System.Drawing.Size(320, 23);
            this.cboActividad.TabIndex = 3;
            // 
            // lblModulo
            // 
            this.lblModulo.AutoSize = true;
            this.lblModulo.Location = new System.Drawing.Point(10, 16);
            this.lblModulo.Name = "lblModulo";
            this.lblModulo.Size = new System.Drawing.Size(50, 15);
            this.lblModulo.TabIndex = 0;
            this.lblModulo.Text = "Módulo";
            // 
            // cboModulo
            // 
            this.cboModulo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboModulo.FormattingEnabled = true;
            this.cboModulo.Location = new System.Drawing.Point(13, 36);
            this.cboModulo.Name = "cboModulo";
            this.cboModulo.Size = new System.Drawing.Size(230, 23);
            this.cboModulo.TabIndex = 1;
            // 
            // splitMain
            // 
            this.splitMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitMain.Location = new System.Drawing.Point(0, 86);
            this.splitMain.Name = "splitMain";
            // 
            // splitMain.Panel1
            // 
            this.splitMain.Panel1.Controls.Add(this.gbDef);
            // 
            // splitMain.Panel2
            // 
            this.splitMain.Panel2.Controls.Add(this.gbAsig);
            this.splitMain.Size = new System.Drawing.Size(1240, 514);
            this.splitMain.SplitterDistance = 610;
            this.splitMain.TabIndex = 1;
            // 
            // gbDef
            // 
            this.gbDef.Controls.Add(this.gridDef);
            this.gbDef.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbDef.Location = new System.Drawing.Point(0, 0);
            this.gbDef.Name = "gbDef";
            this.gbDef.Padding = new System.Windows.Forms.Padding(10);
            this.gbDef.Size = new System.Drawing.Size(610, 514);
            this.gbDef.TabIndex = 0;
            this.gbDef.TabStop = false;
            this.gbDef.Text = "Reportes disponibles (ReporteDef)";
            // 
            // gridDef
            // 
            this.gridDef.AllowUserToAddRows = false;
            this.gridDef.AllowUserToDeleteRows = false;
            this.gridDef.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.gridDef.BackgroundColor = System.Drawing.SystemColors.Window;
            this.gridDef.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridDef.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridDef.Location = new System.Drawing.Point(10, 26);
            this.gridDef.MultiSelect = false;
            this.gridDef.Name = "gridDef";
            this.gridDef.ReadOnly = true;
            this.gridDef.RowHeadersVisible = false;
            this.gridDef.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridDef.Size = new System.Drawing.Size(590, 478);
            this.gridDef.TabIndex = 0;
            // 
            // gbAsig
            // 
            this.gbAsig.Controls.Add(this.gridAsignaciones);
            this.gbAsig.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbAsig.Location = new System.Drawing.Point(0, 0);
            this.gbAsig.Name = "gbAsig";
            this.gbAsig.Padding = new System.Windows.Forms.Padding(10);
            this.gbAsig.Size = new System.Drawing.Size(626, 514);
            this.gbAsig.TabIndex = 0;
            this.gbAsig.TabStop = false;
            this.gbAsig.Text = "Asignaciones (ReporteAsignacion)";
            // 
            // gridAsignaciones
            // 
            this.gridAsignaciones.AllowUserToAddRows = false;
            this.gridAsignaciones.AllowUserToDeleteRows = false;
            this.gridAsignaciones.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.gridAsignaciones.BackgroundColor = System.Drawing.SystemColors.Window;
            this.gridAsignaciones.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridAsignaciones.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridAsignaciones.Location = new System.Drawing.Point(10, 26);
            this.gridAsignaciones.MultiSelect = false;
            this.gridAsignaciones.Name = "gridAsignaciones";
            this.gridAsignaciones.ReadOnly = true;
            this.gridAsignaciones.RowHeadersVisible = false;
            this.gridAsignaciones.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridAsignaciones.Size = new System.Drawing.Size(606, 478);
            this.gridAsignaciones.TabIndex = 0;
            // 
            // pnlBottom
            // 
            this.pnlBottom.Controls.Add(this.btnGuardarAsignacion);
            this.pnlBottom.Controls.Add(this.numPrioridad);
            this.pnlBottom.Controls.Add(this.lblPrioridad);
            this.pnlBottom.Controls.Add(this.numOrden);
            this.pnlBottom.Controls.Add(this.lblOrden);
            this.pnlBottom.Controls.Add(this.chkDefault);
            this.pnlBottom.Controls.Add(this.chkActivo);
            this.pnlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlBottom.Location = new System.Drawing.Point(0, 600);
            this.pnlBottom.Name = "pnlBottom";
            this.pnlBottom.Padding = new System.Windows.Forms.Padding(10);
            this.pnlBottom.Size = new System.Drawing.Size(1240, 70);
            this.pnlBottom.TabIndex = 2;
            // 
            // btnGuardarAsignacion
            // 
            this.btnGuardarAsignacion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGuardarAsignacion.Location = new System.Drawing.Point(1078, 17);
            this.btnGuardarAsignacion.Name = "btnGuardarAsignacion";
            this.btnGuardarAsignacion.Size = new System.Drawing.Size(150, 36);
            this.btnGuardarAsignacion.TabIndex = 6;
            this.btnGuardarAsignacion.Text = "Guardar asignación";
            this.btnGuardarAsignacion.UseVisualStyleBackColor = true;
            // 
            // numPrioridad
            // 
            this.numPrioridad.Location = new System.Drawing.Point(675, 24);
            this.numPrioridad.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.numPrioridad.Name = "numPrioridad";
            this.numPrioridad.Size = new System.Drawing.Size(90, 23);
            this.numPrioridad.TabIndex = 5;
            this.numPrioridad.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // lblPrioridad
            // 
            this.lblPrioridad.AutoSize = true;
            this.lblPrioridad.Location = new System.Drawing.Point(614, 27);
            this.lblPrioridad.Name = "lblPrioridad";
            this.lblPrioridad.Size = new System.Drawing.Size(55, 15);
            this.lblPrioridad.TabIndex = 4;
            this.lblPrioridad.Text = "Prioridad";
            // 
            // numOrden
            // 
            this.numOrden.Location = new System.Drawing.Point(506, 24);
            this.numOrden.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.numOrden.Name = "numOrden";
            this.numOrden.Size = new System.Drawing.Size(90, 23);
            this.numOrden.TabIndex = 3;
            this.numOrden.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
            // 
            // lblOrden
            // 
            this.lblOrden.AutoSize = true;
            this.lblOrden.Location = new System.Drawing.Point(462, 27);
            this.lblOrden.Name = "lblOrden";
            this.lblOrden.Size = new System.Drawing.Size(38, 15);
            this.lblOrden.TabIndex = 2;
            this.lblOrden.Text = "Orden";
            // 
            // chkDefault
            // 
            this.chkDefault.AutoSize = true;
            this.chkDefault.Location = new System.Drawing.Point(224, 26);
            this.chkDefault.Name = "chkDefault";
            this.chkDefault.Size = new System.Drawing.Size(64, 19);
            this.chkDefault.TabIndex = 1;
            this.chkDefault.Text = "Default";
            this.chkDefault.UseVisualStyleBackColor = true;
            // 
            // chkActivo
            // 
            this.chkActivo.AutoSize = true;
            this.chkActivo.Checked = true;
            this.chkActivo.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkActivo.Location = new System.Drawing.Point(13, 26);
            this.chkActivo.Name = "chkActivo";
            this.chkActivo.Size = new System.Drawing.Size(58, 19);
            this.chkActivo.TabIndex = 0;
            this.chkActivo.Text = "Activo";
            this.chkActivo.UseVisualStyleBackColor = true;
            // 
            // FormReporteConfig
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1240, 670);
            this.Controls.Add(this.splitMain);
            this.Controls.Add(this.pnlBottom);
            this.Controls.Add(this.pnlTop);
            this.MinimumSize = new System.Drawing.Size(980, 600);
            this.Name = "FormReporteConfig";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Configuración de Reportes por Actividad";
            this.pnlTop.ResumeLayout(false);
            this.pnlTop.PerformLayout();
            this.gbScope.ResumeLayout(false);
            this.gbScope.PerformLayout();
            this.splitMain.Panel1.ResumeLayout(false);
            this.splitMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitMain)).EndInit();
            this.splitMain.ResumeLayout(false);
            this.gbDef.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridDef)).EndInit();
            this.gbAsig.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridAsignaciones)).EndInit();
            this.pnlBottom.ResumeLayout(false);
            this.pnlBottom.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numPrioridad)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numOrden)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlTop;
        private System.Windows.Forms.Label lblModulo;
        public System.Windows.Forms.ComboBox cboModulo;
        private System.Windows.Forms.Label lblActividad;
        public System.Windows.Forms.ComboBox cboActividad;
        private System.Windows.Forms.GroupBox gbScope;
        public System.Windows.Forms.RadioButton rbEmpresa;
        public System.Windows.Forms.RadioButton rbSucursal;
        public System.Windows.Forms.RadioButton rbUsuario;
        public System.Windows.Forms.Button btnRecargar;

        private System.Windows.Forms.SplitContainer splitMain;
        private System.Windows.Forms.GroupBox gbDef;
        public System.Windows.Forms.DataGridView gridDef;
        private System.Windows.Forms.GroupBox gbAsig;
        public System.Windows.Forms.DataGridView gridAsignaciones;

        private System.Windows.Forms.Panel pnlBottom;
        public System.Windows.Forms.CheckBox chkActivo;
        public System.Windows.Forms.CheckBox chkDefault;
        private System.Windows.Forms.Label lblOrden;
        public System.Windows.Forms.NumericUpDown numOrden;
        private System.Windows.Forms.Label lblPrioridad;
        public System.Windows.Forms.NumericUpDown numPrioridad;
        public System.Windows.Forms.Button btnGuardarAsignacion;

    }
}
