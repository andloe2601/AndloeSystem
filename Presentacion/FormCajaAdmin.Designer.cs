namespace Andloe.Presentacion
{
    partial class FormCajaAdmin
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblSucursal = new System.Windows.Forms.Label();
            this.cbSucursal = new System.Windows.Forms.ComboBox();
            this.groupDatos = new System.Windows.Forms.GroupBox();
            this.cbEstado = new System.Windows.Forms.ComboBox();
            this.lblEstado = new System.Windows.Forms.Label();
            this.txtDescripcion = new System.Windows.Forms.TextBox();
            this.lblDescripcion = new System.Windows.Forms.Label();
            this.txtCajaNumero = new System.Windows.Forms.TextBox();
            this.lblCajaNumero = new System.Windows.Forms.Label();
            this.txtCajaId = new System.Windows.Forms.TextBox();
            this.lblCajaId = new System.Windows.Forms.Label();
            this.gridCajas = new System.Windows.Forms.DataGridView();
            this.colCajaId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colSucursalId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCajaNumero = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDescripcion = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colEstado = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnNuevo = new System.Windows.Forms.Button();
            this.btnGuardar = new System.Windows.Forms.Button();
            this.btnEliminar = new System.Windows.Forms.Button();
            this.btnCerrar = new System.Windows.Forms.Button();
            this.groupDatos.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridCajas)).BeginInit();
            this.SuspendLayout();
            // 
            // lblSucursal
            // 
            this.lblSucursal.AutoSize = true;
            this.lblSucursal.Location = new System.Drawing.Point(12, 15);
            this.lblSucursal.Name = "lblSucursal";
            this.lblSucursal.Size = new System.Drawing.Size(54, 15);
            this.lblSucursal.TabIndex = 0;
            this.lblSucursal.Text = "Sucursal:";
            // 
            // cbSucursal
            // 
            this.cbSucursal.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbSucursal.FormattingEnabled = true;
            this.cbSucursal.Location = new System.Drawing.Point(72, 12);
            this.cbSucursal.Name = "cbSucursal";
            this.cbSucursal.Size = new System.Drawing.Size(220, 23);
            this.cbSucursal.TabIndex = 1;
            this.cbSucursal.SelectedIndexChanged += new System.EventHandler(this.cbSucursal_SelectedIndexChanged);
            // 
            // groupDatos
            // 
            this.groupDatos.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupDatos.Controls.Add(this.cbEstado);
            this.groupDatos.Controls.Add(this.lblEstado);
            this.groupDatos.Controls.Add(this.txtDescripcion);
            this.groupDatos.Controls.Add(this.lblDescripcion);
            this.groupDatos.Controls.Add(this.txtCajaNumero);
            this.groupDatos.Controls.Add(this.lblCajaNumero);
            this.groupDatos.Controls.Add(this.txtCajaId);
            this.groupDatos.Controls.Add(this.lblCajaId);
            this.groupDatos.Location = new System.Drawing.Point(12, 45);
            this.groupDatos.Name = "groupDatos";
            this.groupDatos.Size = new System.Drawing.Size(660, 110);
            this.groupDatos.TabIndex = 2;
            this.groupDatos.TabStop = false;
            this.groupDatos.Text = "Datos de Caja";
            // 
            // cbEstado
            // 
            this.cbEstado.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbEstado.FormattingEnabled = true;
            this.cbEstado.Location = new System.Drawing.Point(419, 60);
            this.cbEstado.Name = "cbEstado";
            this.cbEstado.Size = new System.Drawing.Size(140, 23);
            this.cbEstado.TabIndex = 7;
            // 
            // lblEstado
            // 
            this.lblEstado.AutoSize = true;
            this.lblEstado.Location = new System.Drawing.Point(371, 63);
            this.lblEstado.Name = "lblEstado";
            this.lblEstado.Size = new System.Drawing.Size(45, 15);
            this.lblEstado.TabIndex = 6;
            this.lblEstado.Text = "Estado:";
            // 
            // txtDescripcion
            // 
            this.txtDescripcion.Location = new System.Drawing.Point(92, 60);
            this.txtDescripcion.MaxLength = 100;
            this.txtDescripcion.Name = "txtDescripcion";
            this.txtDescripcion.Size = new System.Drawing.Size(260, 23);
            this.txtDescripcion.TabIndex = 5;
            // 
            // lblDescripcion
            // 
            this.lblDescripcion.AutoSize = true;
            this.lblDescripcion.Location = new System.Drawing.Point(16, 63);
            this.lblDescripcion.Name = "lblDescripcion";
            this.lblDescripcion.Size = new System.Drawing.Size(73, 15);
            this.lblDescripcion.TabIndex = 4;
            this.lblDescripcion.Text = "Descripción:";
            // 
            // txtCajaNumero
            // 
            this.txtCajaNumero.Location = new System.Drawing.Point(419, 26);
            this.txtCajaNumero.MaxLength = 10;
            this.txtCajaNumero.Name = "txtCajaNumero";
            this.txtCajaNumero.Size = new System.Drawing.Size(140, 23);
            this.txtCajaNumero.TabIndex = 3;
            // 
            // lblCajaNumero
            // 
            this.lblCajaNumero.AutoSize = true;
            this.lblCajaNumero.Location = new System.Drawing.Point(337, 29);
            this.lblCajaNumero.Name = "lblCajaNumero";
            this.lblCajaNumero.Size = new System.Drawing.Size(79, 15);
            this.lblCajaNumero.TabIndex = 2;
            this.lblCajaNumero.Text = "Núm. Caja *:";
            // 
            // txtCajaId
            // 
            this.txtCajaId.Location = new System.Drawing.Point(92, 26);
            this.txtCajaId.Name = "txtCajaId";
            this.txtCajaId.ReadOnly = true;
            this.txtCajaId.Size = new System.Drawing.Size(100, 23);
            this.txtCajaId.TabIndex = 1;
            this.txtCajaId.TabStop = false;
            // 
            // lblCajaId
            // 
            this.lblCajaId.AutoSize = true;
            this.lblCajaId.Location = new System.Drawing.Point(16, 29);
            this.lblCajaId.Name = "lblCajaId";
            this.lblCajaId.Size = new System.Drawing.Size(49, 15);
            this.lblCajaId.TabIndex = 0;
            this.lblCajaId.Text = "CajaId:";
            // 
            // gridCajas
            // 
            this.gridCajas.AllowUserToAddRows = false;
            this.gridCajas.AllowUserToDeleteRows = false;
            this.gridCajas.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gridCajas.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridCajas.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colCajaId,
            this.colSucursalId,
            this.colCajaNumero,
            this.colDescripcion,
            this.colEstado});
            this.gridCajas.Location = new System.Drawing.Point(12, 161);
            this.gridCajas.MultiSelect = false;
            this.gridCajas.Name = "gridCajas";
            this.gridCajas.ReadOnly = true;
            this.gridCajas.RowHeadersVisible = false;
            this.gridCajas.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridCajas.Size = new System.Drawing.Size(660, 240);
            this.gridCajas.TabIndex = 3;
            this.gridCajas.SelectionChanged += new System.EventHandler(this.gridCajas_SelectionChanged);
            this.gridCajas.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridCajas_CellDoubleClick);
            // 
            // colCajaId
            // 
            this.colCajaId.DataPropertyName = "CajaId";
            this.colCajaId.HeaderText = "CajaId";
            this.colCajaId.Name = "colCajaId";
            this.colCajaId.ReadOnly = true;
            this.colCajaId.Width = 60;
            // 
            // colSucursalId
            // 
            this.colSucursalId.DataPropertyName = "SucursalId";
            this.colSucursalId.HeaderText = "SucursalId";
            this.colSucursalId.Name = "colSucursalId";
            this.colSucursalId.ReadOnly = true;
            this.colSucursalId.Visible = false;
            // 
            // colCajaNumero
            // 
            this.colCajaNumero.DataPropertyName = "CajaNumero";
            this.colCajaNumero.HeaderText = "Número";
            this.colCajaNumero.Name = "colCajaNumero";
            this.colCajaNumero.ReadOnly = true;
            this.colCajaNumero.Width = 120;
            // 
            // colDescripcion
            // 
            this.colDescripcion.DataPropertyName = "Descripcion";
            this.colDescripcion.HeaderText = "Descripción";
            this.colDescripcion.Name = "colDescripcion";
            this.colDescripcion.ReadOnly = true;
            this.colDescripcion.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            // 
            // colEstado
            // 
            this.colEstado.DataPropertyName = "Estado";
            this.colEstado.HeaderText = "Estado";
            this.colEstado.Name = "colEstado";
            this.colEstado.ReadOnly = true;
            this.colEstado.Width = 80;
            // 
            // btnNuevo
            // 
            this.btnNuevo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnNuevo.Location = new System.Drawing.Point(12, 410);
            this.btnNuevo.Name = "btnNuevo";
            this.btnNuevo.Size = new System.Drawing.Size(80, 27);
            this.btnNuevo.TabIndex = 4;
            this.btnNuevo.Text = "&Nuevo";
            this.btnNuevo.UseVisualStyleBackColor = true;
            this.btnNuevo.Click += new System.EventHandler(this.btnNuevo_Click);
            // 
            // btnGuardar
            // 
            this.btnGuardar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnGuardar.Location = new System.Drawing.Point(98, 410);
            this.btnGuardar.Name = "btnGuardar";
            this.btnGuardar.Size = new System.Drawing.Size(80, 27);
            this.btnGuardar.TabIndex = 5;
            this.btnGuardar.Text = "&Guardar";
            this.btnGuardar.UseVisualStyleBackColor = true;
            this.btnGuardar.Click += new System.EventHandler(this.btnGuardar_Click);
            // 
            // btnEliminar
            // 
            this.btnEliminar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnEliminar.Location = new System.Drawing.Point(184, 410);
            this.btnEliminar.Name = "btnEliminar";
            this.btnEliminar.Size = new System.Drawing.Size(80, 27);
            this.btnEliminar.TabIndex = 6;
            this.btnEliminar.Text = "&Eliminar";
            this.btnEliminar.UseVisualStyleBackColor = true;
            this.btnEliminar.Click += new System.EventHandler(this.btnEliminar_Click);
            // 
            // btnCerrar
            // 
            this.btnCerrar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCerrar.Location = new System.Drawing.Point(592, 410);
            this.btnCerrar.Name = "btnCerrar";
            this.btnCerrar.Size = new System.Drawing.Size(80, 27);
            this.btnCerrar.TabIndex = 7;
            this.btnCerrar.Text = "&Cerrar";
            this.btnCerrar.UseVisualStyleBackColor = true;
            this.btnCerrar.Click += new System.EventHandler(this.btnCerrar_Click);
            // 
            // FormCajaAdmin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(684, 451);
            this.Controls.Add(this.btnCerrar);
            this.Controls.Add(this.btnEliminar);
            this.Controls.Add(this.btnGuardar);
            this.Controls.Add(this.btnNuevo);
            this.Controls.Add(this.gridCajas);
            this.Controls.Add(this.groupDatos);
            this.Controls.Add(this.cbSucursal);
            this.Controls.Add(this.lblSucursal);
            this.Name = "FormCajaAdmin";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Administración de Cajas";
            this.Load += new System.EventHandler(this.FormCajaAdmin_Load);
            this.groupDatos.ResumeLayout(false);
            this.groupDatos.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridCajas)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblSucursal;
        private System.Windows.Forms.ComboBox cbSucursal;
        private System.Windows.Forms.GroupBox groupDatos;
        private System.Windows.Forms.TextBox txtCajaId;
        private System.Windows.Forms.Label lblCajaId;
        private System.Windows.Forms.TextBox txtCajaNumero;
        private System.Windows.Forms.Label lblCajaNumero;
        private System.Windows.Forms.TextBox txtDescripcion;
        private System.Windows.Forms.Label lblDescripcion;
        private System.Windows.Forms.ComboBox cbEstado;
        private System.Windows.Forms.Label lblEstado;
        private System.Windows.Forms.DataGridView gridCajas;
        private System.Windows.Forms.Button btnNuevo;
        private System.Windows.Forms.Button btnGuardar;
        private System.Windows.Forms.Button btnEliminar;
        private System.Windows.Forms.Button btnCerrar;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCajaId;
        private System.Windows.Forms.DataGridViewTextBoxColumn colSucursalId;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCajaNumero;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDescripcion;
        private System.Windows.Forms.DataGridViewTextBoxColumn colEstado;
    }
}
