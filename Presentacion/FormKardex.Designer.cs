namespace Presentation
{
    partial class FormKardex
    {
        /// <summary>
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador.
        /// No modificar el contenido con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.panelFiltros = new System.Windows.Forms.Panel();
            this.lblProducto = new System.Windows.Forms.Label();
            this.txtProductoCodigo = new System.Windows.Forms.TextBox();
            this.txtProductoDescripcion = new System.Windows.Forms.TextBox();
            this.btnBuscarProducto = new System.Windows.Forms.Button();
            this.lblDesde = new System.Windows.Forms.Label();
            this.dtDesde = new System.Windows.Forms.DateTimePicker();
            this.lblHasta = new System.Windows.Forms.Label();
            this.dtHasta = new System.Windows.Forms.DateTimePicker();
            this.lblAlmacen = new System.Windows.Forms.Label();
            this.cboAlmacen = new System.Windows.Forms.ComboBox();
            this.btnBuscar = new System.Windows.Forms.Button();
            this.gridKardex = new System.Windows.Forms.DataGridView();
            this.panelInferior = new System.Windows.Forms.Panel();
            this.lblExistenciaFinalTitulo = new System.Windows.Forms.Label();
            this.lblExistenciaFinal = new System.Windows.Forms.Label();
            this.btnCerrar = new System.Windows.Forms.Button();
            this.panelFiltros.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridKardex)).BeginInit();
            this.panelInferior.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelFiltros
            // 
            this.panelFiltros.BackColor = System.Drawing.SystemColors.ControlLight;
            this.panelFiltros.Controls.Add(this.lblProducto);
            this.panelFiltros.Controls.Add(this.txtProductoCodigo);
            this.panelFiltros.Controls.Add(this.txtProductoDescripcion);
            this.panelFiltros.Controls.Add(this.btnBuscarProducto);
            this.panelFiltros.Controls.Add(this.lblDesde);
            this.panelFiltros.Controls.Add(this.dtDesde);
            this.panelFiltros.Controls.Add(this.lblHasta);
            this.panelFiltros.Controls.Add(this.dtHasta);
            this.panelFiltros.Controls.Add(this.lblAlmacen);
            this.panelFiltros.Controls.Add(this.cboAlmacen);
            this.panelFiltros.Controls.Add(this.btnBuscar);
            this.panelFiltros.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelFiltros.Location = new System.Drawing.Point(0, 0);
            this.panelFiltros.Name = "panelFiltros";
            this.panelFiltros.Padding = new System.Windows.Forms.Padding(8);
            this.panelFiltros.Size = new System.Drawing.Size(800, 95);
            this.panelFiltros.TabIndex = 0;
            // 
            // lblProducto
            // 
            this.lblProducto.AutoSize = true;
            this.lblProducto.Location = new System.Drawing.Point(15, 15);
            this.lblProducto.Name = "lblProducto";
            this.lblProducto.Size = new System.Drawing.Size(59, 15);
            this.lblProducto.TabIndex = 0;
            this.lblProducto.Text = "Producto:";
            // 
            // txtProductoCodigo
            // 
            this.txtProductoCodigo.Location = new System.Drawing.Point(80, 11);
            this.txtProductoCodigo.Name = "txtProductoCodigo";
            this.txtProductoCodigo.Size = new System.Drawing.Size(90, 23);
            this.txtProductoCodigo.TabIndex = 1;
            this.txtProductoCodigo.Leave += new System.EventHandler(this.txtProductoCodigo_Leave);
            // 
            // txtProductoDescripcion
            // 
            this.txtProductoDescripcion.Location = new System.Drawing.Point(176, 11);
            this.txtProductoDescripcion.Name = "txtProductoDescripcion";
            this.txtProductoDescripcion.ReadOnly = true;
            this.txtProductoDescripcion.Size = new System.Drawing.Size(260, 23);
            this.txtProductoDescripcion.TabIndex = 2;
            // 
            // btnBuscarProducto
            // 
            this.btnBuscarProducto.Location = new System.Drawing.Point(442, 10);
            this.btnBuscarProducto.Name = "btnBuscarProducto";
            this.btnBuscarProducto.Size = new System.Drawing.Size(35, 25);
            this.btnBuscarProducto.TabIndex = 3;
            this.btnBuscarProducto.Text = "...";
            this.btnBuscarProducto.UseVisualStyleBackColor = true;
            this.btnBuscarProducto.Click += new System.EventHandler(this.btnBuscarProducto_Click);
            // 
            // lblDesde
            // 
            this.lblDesde.AutoSize = true;
            this.lblDesde.Location = new System.Drawing.Point(15, 50);
            this.lblDesde.Name = "lblDesde";
            this.lblDesde.Size = new System.Drawing.Size(44, 15);
            this.lblDesde.TabIndex = 4;
            this.lblDesde.Text = "Desde:";
            // 
            // dtDesde
            // 
            this.dtDesde.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtDesde.Location = new System.Drawing.Point(80, 46);
            this.dtDesde.Name = "dtDesde";
            this.dtDesde.Size = new System.Drawing.Size(95, 23);
            this.dtDesde.TabIndex = 5;
            // 
            // lblHasta
            // 
            this.lblHasta.AutoSize = true;
            this.lblHasta.Location = new System.Drawing.Point(185, 50);
            this.lblHasta.Name = "lblHasta";
            this.lblHasta.Size = new System.Drawing.Size(42, 15);
            this.lblHasta.TabIndex = 6;
            this.lblHasta.Text = "Hasta:";
            // 
            // dtHasta
            // 
            this.dtHasta.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtHasta.Location = new System.Drawing.Point(235, 46);
            this.dtHasta.Name = "dtHasta";
            this.dtHasta.Size = new System.Drawing.Size(95, 23);
            this.dtHasta.TabIndex = 7;
            // 
            // lblAlmacen
            // 
            this.lblAlmacen.AutoSize = true;
            this.lblAlmacen.Location = new System.Drawing.Point(345, 50);
            this.lblAlmacen.Name = "lblAlmacen";
            this.lblAlmacen.Size = new System.Drawing.Size(57, 15);
            this.lblAlmacen.TabIndex = 8;
            this.lblAlmacen.Text = "Almacén:";
            // 
            // cboAlmacen
            // 
            this.cboAlmacen.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboAlmacen.FormattingEnabled = true;
            this.cboAlmacen.Location = new System.Drawing.Point(408, 46);
            this.cboAlmacen.Name = "cboAlmacen";
            this.cboAlmacen.Size = new System.Drawing.Size(160, 23);
            this.cboAlmacen.TabIndex = 9;
            // 
            // btnBuscar
            // 
            this.btnBuscar.Location = new System.Drawing.Point(590, 44);
            this.btnBuscar.Name = "btnBuscar";
            this.btnBuscar.Size = new System.Drawing.Size(85, 28);
            this.btnBuscar.TabIndex = 10;
            this.btnBuscar.Text = "Buscar";
            this.btnBuscar.UseVisualStyleBackColor = true;
            this.btnBuscar.Click += new System.EventHandler(this.btnBuscar_Click);
            // 
            // gridKardex
            // 
            this.gridKardex.AllowUserToAddRows = false;
            this.gridKardex.AllowUserToDeleteRows = false;
            this.gridKardex.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                                                                         | System.Windows.Forms.AnchorStyles.Left)
                                                                         | System.Windows.Forms.AnchorStyles.Right)));
            this.gridKardex.BackgroundColor = System.Drawing.SystemColors.Window;
            this.gridKardex.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridKardex.Location = new System.Drawing.Point(10, 100);
            this.gridKardex.Name = "gridKardex";
            this.gridKardex.ReadOnly = true;
            this.gridKardex.RowHeadersVisible = false;
            this.gridKardex.RowTemplate.Height = 25;
            this.gridKardex.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridKardex.Size = new System.Drawing.Size(780, 330);
            this.gridKardex.TabIndex = 11;
            // 
            // panelInferior
            // 
            this.panelInferior.BackColor = System.Drawing.SystemColors.ControlLight;
            this.panelInferior.Controls.Add(this.lblExistenciaFinalTitulo);
            this.panelInferior.Controls.Add(this.lblExistenciaFinal);
            this.panelInferior.Controls.Add(this.btnCerrar);
            this.panelInferior.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelInferior.Location = new System.Drawing.Point(0, 435);
            this.panelInferior.Name = "panelInferior";
            this.panelInferior.Padding = new System.Windows.Forms.Padding(8);
            this.panelInferior.Size = new System.Drawing.Size(800, 45);
            this.panelInferior.TabIndex = 12;
            // 
            // lblExistenciaFinalTitulo
            // 
            this.lblExistenciaFinalTitulo.AutoSize = true;
            this.lblExistenciaFinalTitulo.Location = new System.Drawing.Point(15, 15);
            this.lblExistenciaFinalTitulo.Name = "lblExistenciaFinalTitulo";
            this.lblExistenciaFinalTitulo.Size = new System.Drawing.Size(95, 15);
            this.lblExistenciaFinalTitulo.TabIndex = 0;
            this.lblExistenciaFinalTitulo.Text = "Existencia Final:";
            // 
            // lblExistenciaFinal
            // 
            this.lblExistenciaFinal.AutoSize = true;
            this.lblExistenciaFinal.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblExistenciaFinal.Location = new System.Drawing.Point(120, 15);
            this.lblExistenciaFinal.Name = "lblExistenciaFinal";
            this.lblExistenciaFinal.Size = new System.Drawing.Size(35, 15);
            this.lblExistenciaFinal.TabIndex = 1;
            this.lblExistenciaFinal.Text = "0.00";
            // 
            // btnCerrar
            // 
            this.btnCerrar.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnCerrar.Location = new System.Drawing.Point(700, 10);
            this.btnCerrar.Name = "btnCerrar";
            this.btnCerrar.Size = new System.Drawing.Size(85, 25);
            this.btnCerrar.TabIndex = 2;
            this.btnCerrar.Text = "Cerrar";
            this.btnCerrar.UseVisualStyleBackColor = true;
            this.btnCerrar.Click += new System.EventHandler(this.btnCerrar_Click);
            // 
            // FormKardex
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.ClientSize = new System.Drawing.Size(800, 480);
            this.Controls.Add(this.gridKardex);
            this.Controls.Add(this.panelInferior);
            this.Controls.Add(this.panelFiltros);
            this.Name = "FormKardex";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Kardex por Producto";
            this.Load += new System.EventHandler(this.FormKardex_Load);
            this.panelFiltros.ResumeLayout(false);
            this.panelFiltros.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridKardex)).EndInit();
            this.panelInferior.ResumeLayout(false);
            this.panelInferior.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelFiltros;
        private System.Windows.Forms.Label lblProducto;
        private System.Windows.Forms.TextBox txtProductoCodigo;
        private System.Windows.Forms.TextBox txtProductoDescripcion;
        private System.Windows.Forms.Button btnBuscarProducto;
        private System.Windows.Forms.Label lblDesde;
        private System.Windows.Forms.DateTimePicker dtDesde;
        private System.Windows.Forms.Label lblHasta;
        private System.Windows.Forms.DateTimePicker dtHasta;
        private System.Windows.Forms.Label lblAlmacen;
        private System.Windows.Forms.ComboBox cboAlmacen;
        private System.Windows.Forms.Button btnBuscar;
        private System.Windows.Forms.DataGridView gridKardex;
        private System.Windows.Forms.Panel panelInferior;
        private System.Windows.Forms.Label lblExistenciaFinalTitulo;
        private System.Windows.Forms.Label lblExistenciaFinal;
        private System.Windows.Forms.Button btnCerrar;
    }
}
