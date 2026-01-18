namespace Presentation
{
    partial class FormInvMovimiento
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.panelTop = new System.Windows.Forms.Panel();
            this.lblUsuario = new System.Windows.Forms.Label();
            this.lblTitulo = new System.Windows.Forms.Label();
            this.panelMain = new System.Windows.Forms.Panel();
            this.groupCabecera = new System.Windows.Forms.GroupBox();
            this.chkUsarDestino = new System.Windows.Forms.CheckBox();
            this.cboAlmacenDestino = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.cboAlmacenOrigen = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.cboTipo = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.dtFecha = new System.Windows.Forms.DateTimePicker();
            this.label3 = new System.Windows.Forms.Label();
            this.txtObservacion = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.groupProducto = new System.Windows.Forms.GroupBox();
            this.btnAgregarLinea = new System.Windows.Forms.Button();
            this.btnBuscarProducto = new System.Windows.Forms.Button();
            this.numCantidad = new System.Windows.Forms.NumericUpDown();
            this.label10 = new System.Windows.Forms.Label();
            this.txtCosto = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.txtExistencia = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.txtProductoDescripcion = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtProductoCodigo = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.gridLineas = new System.Windows.Forms.DataGridView();
            this.colCodigo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDescripcion = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCantidad = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCosto = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panelBottom = new System.Windows.Forms.Panel();
            this.btnQuitarLinea = new System.Windows.Forms.Button();
            this.btnGuardar = new System.Windows.Forms.Button();
            this.btnCerrar = new System.Windows.Forms.Button();
            this.panelTop.SuspendLayout();
            this.panelMain.SuspendLayout();
            this.groupCabecera.SuspendLayout();
            this.groupProducto.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numCantidad)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLineas)).BeginInit();
            this.panelBottom.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelTop
            // 
            this.panelTop.Controls.Add(this.lblUsuario);
            this.panelTop.Controls.Add(this.lblTitulo);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Name = "panelTop";
            this.panelTop.Padding = new System.Windows.Forms.Padding(12, 10, 12, 10);
            this.panelTop.Size = new System.Drawing.Size(1100, 54);
            this.panelTop.TabIndex = 0;
            // 
            // lblUsuario
            // 
            this.lblUsuario.Dock = System.Windows.Forms.DockStyle.Right;
            this.lblUsuario.Location = new System.Drawing.Point(780, 10);
            this.lblUsuario.Name = "lblUsuario";
            this.lblUsuario.Size = new System.Drawing.Size(308, 34);
            this.lblUsuario.TabIndex = 1;
            this.lblUsuario.Text = "Usuario: -";
            this.lblUsuario.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblTitulo
            // 
            this.lblTitulo.Dock = System.Windows.Forms.DockStyle.Left;
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblTitulo.Location = new System.Drawing.Point(12, 10);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(450, 34);
            this.lblTitulo.TabIndex = 0;
            this.lblTitulo.Text = "Entrada / Salida de Inventario";
            this.lblTitulo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panelMain
            // 
            this.panelMain.Controls.Add(this.gridLineas);
            this.panelMain.Controls.Add(this.groupProducto);
            this.panelMain.Controls.Add(this.groupCabecera);
            this.panelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMain.Location = new System.Drawing.Point(0, 54);
            this.panelMain.Name = "panelMain";
            this.panelMain.Padding = new System.Windows.Forms.Padding(12);
            this.panelMain.Size = new System.Drawing.Size(1100, 606);
            this.panelMain.TabIndex = 1;
            // 
            // groupCabecera
            // 
            this.groupCabecera.Controls.Add(this.chkUsarDestino);
            this.groupCabecera.Controls.Add(this.cboAlmacenDestino);
            this.groupCabecera.Controls.Add(this.label6);
            this.groupCabecera.Controls.Add(this.cboAlmacenOrigen);
            this.groupCabecera.Controls.Add(this.label5);
            this.groupCabecera.Controls.Add(this.cboTipo);
            this.groupCabecera.Controls.Add(this.label4);
            this.groupCabecera.Controls.Add(this.dtFecha);
            this.groupCabecera.Controls.Add(this.label3);
            this.groupCabecera.Controls.Add(this.txtObservacion);
            this.groupCabecera.Controls.Add(this.label2);
            this.groupCabecera.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupCabecera.Location = new System.Drawing.Point(12, 12);
            this.groupCabecera.Name = "groupCabecera";
            this.groupCabecera.Padding = new System.Windows.Forms.Padding(10);
            this.groupCabecera.Size = new System.Drawing.Size(1076, 132);
            this.groupCabecera.TabIndex = 0;
            this.groupCabecera.TabStop = false;
            this.groupCabecera.Text = "Cabecera";
            // 
            // chkUsarDestino
            // 
            this.chkUsarDestino.AutoSize = true;
            this.chkUsarDestino.Location = new System.Drawing.Point(708, 32);
            this.chkUsarDestino.Name = "chkUsarDestino";
            this.chkUsarDestino.Size = new System.Drawing.Size(129, 19);
            this.chkUsarDestino.TabIndex = 6;
            this.chkUsarDestino.Text = "Usar almacén destino";
            this.chkUsarDestino.UseVisualStyleBackColor = true;
            this.chkUsarDestino.CheckedChanged += new System.EventHandler(this.chkUsarDestino_CheckedChanged);
            // 
            // cboAlmacenDestino
            // 
            this.cboAlmacenDestino.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboAlmacenDestino.Enabled = false;
            this.cboAlmacenDestino.FormattingEnabled = true;
            this.cboAlmacenDestino.Location = new System.Drawing.Point(708, 78);
            this.cboAlmacenDestino.Name = "cboAlmacenDestino";
            this.cboAlmacenDestino.Size = new System.Drawing.Size(350, 23);
            this.cboAlmacenDestino.TabIndex = 8;
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(708, 57);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(132, 18);
            this.label6.TabIndex = 7;
            this.label6.Text = "Almacén Destino";
            // 
            // cboAlmacenOrigen
            // 
            this.cboAlmacenOrigen.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboAlmacenOrigen.FormattingEnabled = true;
            this.cboAlmacenOrigen.Location = new System.Drawing.Point(345, 78);
            this.cboAlmacenOrigen.Name = "cboAlmacenOrigen";
            this.cboAlmacenOrigen.Size = new System.Drawing.Size(350, 23);
            this.cboAlmacenOrigen.TabIndex = 5;
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(345, 57);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(132, 18);
            this.label5.TabIndex = 4;
            this.label5.Text = "Almacén Origen";
            // 
            // cboTipo
            // 
            this.cboTipo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboTipo.FormattingEnabled = true;
            this.cboTipo.Location = new System.Drawing.Point(13, 78);
            this.cboTipo.Name = "cboTipo";
            this.cboTipo.Size = new System.Drawing.Size(150, 23);
            this.cboTipo.TabIndex = 1;
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(13, 57);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(76, 18);
            this.label4.TabIndex = 0;
            this.label4.Text = "Tipo";
            // 
            // dtFecha
            // 
            this.dtFecha.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtFecha.CustomFormat = "dd/MM/yyyy HH:mm";
            this.dtFecha.Location = new System.Drawing.Point(170, 78);
            this.dtFecha.Name = "dtFecha";
            this.dtFecha.Size = new System.Drawing.Size(165, 23);
            this.dtFecha.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(170, 57);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(76, 18);
            this.label3.TabIndex = 2;
            this.label3.Text = "Fecha";
            // 
            // txtObservacion
            // 
            this.txtObservacion.Location = new System.Drawing.Point(13, 32);
            this.txtObservacion.Name = "txtObservacion";
            this.txtObservacion.Size = new System.Drawing.Size(682, 23);
            this.txtObservacion.TabIndex = 9;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(13, 14);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(120, 18);
            this.label2.TabIndex = 10;
            this.label2.Text = "Observación";
            // 
            // groupProducto
            // 
            this.groupProducto.Controls.Add(this.btnAgregarLinea);
            this.groupProducto.Controls.Add(this.btnBuscarProducto);
            this.groupProducto.Controls.Add(this.numCantidad);
            this.groupProducto.Controls.Add(this.label10);
            this.groupProducto.Controls.Add(this.txtCosto);
            this.groupProducto.Controls.Add(this.label9);
            this.groupProducto.Controls.Add(this.txtExistencia);
            this.groupProducto.Controls.Add(this.label8);
            this.groupProducto.Controls.Add(this.txtProductoDescripcion);
            this.groupProducto.Controls.Add(this.label7);
            this.groupProducto.Controls.Add(this.txtProductoCodigo);
            this.groupProducto.Controls.Add(this.label1);
            this.groupProducto.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupProducto.Location = new System.Drawing.Point(12, 144);
            this.groupProducto.Name = "groupProducto";
            this.groupProducto.Padding = new System.Windows.Forms.Padding(10);
            this.groupProducto.Size = new System.Drawing.Size(1076, 110);
            this.groupProducto.TabIndex = 1;
            this.groupProducto.TabStop = false;
            this.groupProducto.Text = "Producto";
            // 
            // btnAgregarLinea
            // 
            this.btnAgregarLinea.Location = new System.Drawing.Point(930, 62);
            this.btnAgregarLinea.Name = "btnAgregarLinea";
            this.btnAgregarLinea.Size = new System.Drawing.Size(128, 28);
            this.btnAgregarLinea.TabIndex = 11;
            this.btnAgregarLinea.Text = "Agregar línea";
            this.btnAgregarLinea.UseVisualStyleBackColor = true;
            this.btnAgregarLinea.Click += new System.EventHandler(this.btnAgregarLinea_Click);
            // 
            // btnBuscarProducto
            // 
            this.btnBuscarProducto.Location = new System.Drawing.Point(263, 31);
            this.btnBuscarProducto.Name = "btnBuscarProducto";
            this.btnBuscarProducto.Size = new System.Drawing.Size(38, 23);
            this.btnBuscarProducto.TabIndex = 2;
            this.btnBuscarProducto.Text = "...";
            this.btnBuscarProducto.UseVisualStyleBackColor = true;
            this.btnBuscarProducto.Click += new System.EventHandler(this.btnBuscarProducto_Click);
            // 
            // numCantidad
            // 
            this.numCantidad.DecimalPlaces = 2;
            this.numCantidad.Location = new System.Drawing.Point(708, 66);
            this.numCantidad.Maximum = new decimal(new int[] { 1000000, 0, 0, 0 });
            this.numCantidad.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            this.numCantidad.Name = "numCantidad";
            this.numCantidad.Size = new System.Drawing.Size(100, 23);
            this.numCantidad.TabIndex = 9;
            this.numCantidad.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // label10
            // 
            this.label10.Location = new System.Drawing.Point(708, 48);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(76, 18);
            this.label10.TabIndex = 8;
            this.label10.Text = "Cantidad";
            // 
            // txtCosto
            // 
            this.txtCosto.Location = new System.Drawing.Point(590, 66);
            this.txtCosto.Name = "txtCosto";
            this.txtCosto.Size = new System.Drawing.Size(110, 23);
            this.txtCosto.TabIndex = 7;
            // 
            // label9
            // 
            this.label9.Location = new System.Drawing.Point(590, 48);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(76, 18);
            this.label9.TabIndex = 6;
            this.label9.Text = "Costo";
            // 
            // txtExistencia
            // 
            this.txtExistencia.Location = new System.Drawing.Point(472, 66);
            this.txtExistencia.Name = "txtExistencia";
            this.txtExistencia.Size = new System.Drawing.Size(110, 23);
            this.txtExistencia.TabIndex = 5;
            // 
            // label8
            // 
            this.label8.Location = new System.Drawing.Point(472, 48);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(76, 18);
            this.label8.TabIndex = 4;
            this.label8.Text = "Existencia";
            // 
            // txtProductoDescripcion
            // 
            this.txtProductoDescripcion.Location = new System.Drawing.Point(307, 66);
            this.txtProductoDescripcion.Name = "txtProductoDescripcion";
            this.txtProductoDescripcion.Size = new System.Drawing.Size(457, 23);
            this.txtProductoDescripcion.TabIndex = 3;
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(307, 48);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(120, 18);
            this.label7.TabIndex = 2;
            this.label7.Text = "Descripción";
            // 
            // txtProductoCodigo
            // 
            this.txtProductoCodigo.Location = new System.Drawing.Point(13, 31);
            this.txtProductoCodigo.Name = "txtProductoCodigo";
            this.txtProductoCodigo.Size = new System.Drawing.Size(244, 23);
            this.txtProductoCodigo.TabIndex = 1;
            this.txtProductoCodigo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtProductoCodigo_KeyDown);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(140, 18);
            this.label1.TabIndex = 0;
            this.label1.Text = "Código Producto";
            // 
            // gridLineas
            // 
            this.gridLineas.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridLineas.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colCodigo,
            this.colDescripcion,
            this.colCantidad,
            this.colCosto});
            this.gridLineas.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridLineas.Location = new System.Drawing.Point(12, 254);
            this.gridLineas.Name = "gridLineas";
            this.gridLineas.RowTemplate.Height = 25;
            this.gridLineas.Size = new System.Drawing.Size(1076, 352);
            this.gridLineas.TabIndex = 2;
            // 
            // colCodigo
            // 
            this.colCodigo.HeaderText = "Código";
            this.colCodigo.Name = "colCodigo";
            this.colCodigo.Width = 140;
            // 
            // colDescripcion
            // 
            this.colDescripcion.HeaderText = "Descripción";
            this.colDescripcion.Name = "colDescripcion";
            this.colDescripcion.Width = 520;
            // 
            // colCantidad
            // 
            this.colCantidad.HeaderText = "Cantidad";
            this.colCantidad.Name = "colCantidad";
            this.colCantidad.Width = 120;
            // 
            // colCosto
            // 
            this.colCosto.HeaderText = "Costo";
            this.colCosto.Name = "colCosto";
            this.colCosto.Width = 120;
            // 
            // panelBottom
            // 
            this.panelBottom.Controls.Add(this.btnQuitarLinea);
            this.panelBottom.Controls.Add(this.btnGuardar);
            this.panelBottom.Controls.Add(this.btnCerrar);
            this.panelBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelBottom.Location = new System.Drawing.Point(0, 660);
            this.panelBottom.Name = "panelBottom";
            this.panelBottom.Padding = new System.Windows.Forms.Padding(12, 10, 12, 10);
            this.panelBottom.Size = new System.Drawing.Size(1100, 60);
            this.panelBottom.TabIndex = 2;
            // 
            // btnQuitarLinea
            // 
            this.btnQuitarLinea.Location = new System.Drawing.Point(12, 16);
            this.btnQuitarLinea.Name = "btnQuitarLinea";
            this.btnQuitarLinea.Size = new System.Drawing.Size(120, 30);
            this.btnQuitarLinea.TabIndex = 0;
            this.btnQuitarLinea.Text = "Quitar línea";
            this.btnQuitarLinea.UseVisualStyleBackColor = true;
            this.btnQuitarLinea.Click += new System.EventHandler(this.btnQuitarLinea_Click);
            // 
            // btnGuardar
            // 
            this.btnGuardar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGuardar.Location = new System.Drawing.Point(844, 16);
            this.btnGuardar.Name = "btnGuardar";
            this.btnGuardar.Size = new System.Drawing.Size(120, 30);
            this.btnGuardar.TabIndex = 1;
            this.btnGuardar.Text = "Guardar";
            this.btnGuardar.UseVisualStyleBackColor = true;
            this.btnGuardar.Click += new System.EventHandler(this.btnGuardar_Click);
            // 
            // btnCerrar
            // 
            this.btnCerrar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCerrar.Location = new System.Drawing.Point(968, 16);
            this.btnCerrar.Name = "btnCerrar";
            this.btnCerrar.Size = new System.Drawing.Size(120, 30);
            this.btnCerrar.TabIndex = 2;
            this.btnCerrar.Text = "Cerrar";
            this.btnCerrar.UseVisualStyleBackColor = true;
            this.btnCerrar.Click += new System.EventHandler(this.btnCerrar_Click);
            // 
            // FormInvMovimiento
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1100, 720);
            this.Controls.Add(this.panelMain);
            this.Controls.Add(this.panelBottom);
            this.Controls.Add(this.panelTop);
            this.Name = "FormInvMovimiento";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Entrada / Salida de Inventario";
            this.panelTop.ResumeLayout(false);
            this.panelMain.ResumeLayout(false);
            this.groupCabecera.ResumeLayout(false);
            this.groupCabecera.PerformLayout();
            this.groupProducto.ResumeLayout(false);
            this.groupProducto.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numCantidad)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLineas)).EndInit();
            this.panelBottom.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Label lblUsuario;
        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.Panel panelMain;
        private System.Windows.Forms.GroupBox groupCabecera;
        private System.Windows.Forms.TextBox txtObservacion;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DateTimePicker dtFecha;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cboTipo;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cboAlmacenOrigen;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckBox chkUsarDestino;
        private System.Windows.Forms.ComboBox cboAlmacenDestino;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.GroupBox groupProducto;
        private System.Windows.Forms.TextBox txtProductoCodigo;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnBuscarProducto;
        private System.Windows.Forms.TextBox txtProductoDescripcion;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtExistencia;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtCosto;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.NumericUpDown numCantidad;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Button btnAgregarLinea;
        private System.Windows.Forms.DataGridView gridLineas;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCodigo;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDescripcion;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCantidad;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCosto;
        private System.Windows.Forms.Panel panelBottom;
        private System.Windows.Forms.Button btnQuitarLinea;
        private System.Windows.Forms.Button btnGuardar;
        private System.Windows.Forms.Button btnCerrar;
    }
}
