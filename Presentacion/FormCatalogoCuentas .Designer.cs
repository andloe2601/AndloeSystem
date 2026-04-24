namespace Andloe.Presentacion
{
    partial class FormCatalogoCuentas
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.SplitContainer splitMain;
        private System.Windows.Forms.GroupBox grpTree;
        private System.Windows.Forms.TreeView treeCuentas;

        private System.Windows.Forms.GroupBox grpListado;
        private System.Windows.Forms.DataGridView gridCuentas;
        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Label lblBuscar;
        private System.Windows.Forms.TextBox txtBuscar;

        private System.Windows.Forms.GroupBox grpEdicion;
        private System.Windows.Forms.TableLayoutPanel tableEdicion;
        private System.Windows.Forms.Label lblCodigo;
        private System.Windows.Forms.TextBox txtCodigo;
        private System.Windows.Forms.Label lblDescripcion;
        private System.Windows.Forms.TextBox txtDescripcion;
        private System.Windows.Forms.Label lblTipo;
        private System.Windows.Forms.ComboBox cboTipo;
        private System.Windows.Forms.Label lblPadreTitulo;
        private System.Windows.Forms.Label lblPadre;
        private System.Windows.Forms.CheckBox chkActivo;

        private System.Windows.Forms.FlowLayoutPanel panelBotones;
        private System.Windows.Forms.Button btnNuevo;
        private System.Windows.Forms.Button btnGuardar;
        private System.Windows.Forms.Button btnEliminar;
        private System.Windows.Forms.Button btnEstado;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            splitMain = new SplitContainer();
            grpTree = new GroupBox();
            treeCuentas = new TreeView();
            grpListado = new GroupBox();
            gridCuentas = new DataGridView();
            panelTop = new Panel();
            txtBuscar = new TextBox();
            lblBuscar = new Label();
            grpEdicion = new GroupBox();
            tableEdicion = new TableLayoutPanel();
            lblCodigo = new Label();
            txtCodigo = new TextBox();
            lblTipo = new Label();
            cboTipo = new ComboBox();
            lblDescripcion = new Label();
            txtDescripcion = new TextBox();
            lblPadreTitulo = new Label();
            lblPadre = new Label();
            chkActivo = new CheckBox();
            panelBotones = new FlowLayoutPanel();
            btnNuevo = new Button();
            btnGuardar = new Button();
            btnEliminar = new Button();
            btnEstado = new Button();
            ((System.ComponentModel.ISupportInitialize)splitMain).BeginInit();
            splitMain.Panel1.SuspendLayout();
            splitMain.Panel2.SuspendLayout();
            splitMain.SuspendLayout();
            grpTree.SuspendLayout();
            grpListado.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)gridCuentas).BeginInit();
            panelTop.SuspendLayout();
            grpEdicion.SuspendLayout();
            tableEdicion.SuspendLayout();
            panelBotones.SuspendLayout();
            SuspendLayout();
            // 
            // splitMain
            // 
            splitMain.Dock = DockStyle.Fill;
            splitMain.FixedPanel = FixedPanel.Panel1;
            splitMain.Location = new Point(12, 12);
            splitMain.Name = "splitMain";
            // 
            // splitMain.Panel1
            // 
            splitMain.Panel1.Controls.Add(grpTree);
            // 
            // splitMain.Panel2
            // 
            splitMain.Panel2.Controls.Add(grpListado);
            splitMain.Panel2.Controls.Add(grpEdicion);
            splitMain.Size = new Size(1060, 640);
            splitMain.SplitterDistance = 330;
            splitMain.TabIndex = 0;
            // 
            // grpTree
            // 
            grpTree.Controls.Add(treeCuentas);
            grpTree.Dock = DockStyle.Fill;
            grpTree.Location = new Point(0, 0);
            grpTree.Name = "grpTree";
            grpTree.Padding = new Padding(10);
            grpTree.Size = new Size(330, 640);
            grpTree.TabIndex = 0;
            grpTree.TabStop = false;
            grpTree.Text = "Estructura (Árbol)";
            // 
            // treeCuentas
            // 
            treeCuentas.Dock = DockStyle.Fill;
            treeCuentas.HideSelection = false;
            treeCuentas.Location = new Point(10, 26);
            treeCuentas.Name = "treeCuentas";
            treeCuentas.Size = new Size(310, 604);
            treeCuentas.TabIndex = 0;
            treeCuentas.AfterSelect += treeCuentas_AfterSelect;
            // 
            // grpListado
            // 
            grpListado.Controls.Add(gridCuentas);
            grpListado.Controls.Add(panelTop);
            grpListado.Dock = DockStyle.Fill;
            grpListado.Location = new Point(0, 0);
            grpListado.Name = "grpListado";
            grpListado.Padding = new Padding(10);
            grpListado.Size = new Size(726, 430);
            grpListado.TabIndex = 1;
            grpListado.TabStop = false;
            grpListado.Text = "Listado";
            // 
            // gridCuentas
            // 
            gridCuentas.AllowUserToAddRows = false;
            gridCuentas.AllowUserToDeleteRows = false;
            gridCuentas.AllowUserToResizeRows = false;
            gridCuentas.BackgroundColor = Color.White;
            gridCuentas.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            gridCuentas.Dock = DockStyle.Fill;
            gridCuentas.Location = new Point(10, 66);
            gridCuentas.MultiSelect = false;
            gridCuentas.Name = "gridCuentas";
            gridCuentas.ReadOnly = true;
            gridCuentas.RowHeadersVisible = false;
            gridCuentas.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            gridCuentas.Size = new Size(706, 354);
            gridCuentas.TabIndex = 1;
            gridCuentas.CellClick += gridCuentas_CellClick;
            // 
            // panelTop
            // 
            panelTop.Controls.Add(txtBuscar);
            panelTop.Controls.Add(lblBuscar);
            panelTop.Dock = DockStyle.Top;
            panelTop.Location = new Point(10, 26);
            panelTop.Name = "panelTop";
            panelTop.Size = new Size(706, 40);
            panelTop.TabIndex = 0;
            // 
            // txtBuscar
            // 
            txtBuscar.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtBuscar.Location = new Point(125, 8);
            txtBuscar.Name = "txtBuscar";
            txtBuscar.Size = new Size(570, 23);
            txtBuscar.TabIndex = 1;
            txtBuscar.TextChanged += txtBuscar_TextChanged;
            // 
            // lblBuscar
            // 
            lblBuscar.AutoSize = true;
            lblBuscar.Location = new Point(3, 11);
            lblBuscar.Name = "lblBuscar";
            lblBuscar.Size = new Size(122, 15);
            lblBuscar.TabIndex = 0;
            lblBuscar.Text = "Buscar (código/desc):";
            // 
            // grpEdicion
            // 
            grpEdicion.Controls.Add(tableEdicion);
            grpEdicion.Controls.Add(panelBotones);
            grpEdicion.Dock = DockStyle.Bottom;
            grpEdicion.Location = new Point(0, 430);
            grpEdicion.Name = "grpEdicion";
            grpEdicion.Padding = new Padding(10);
            grpEdicion.Size = new Size(726, 210);
            grpEdicion.TabIndex = 0;
            grpEdicion.TabStop = false;
            grpEdicion.Text = "Edición / Registro";
            // 
            // tableEdicion
            // 
            tableEdicion.ColumnCount = 4;
            tableEdicion.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 90F));
            tableEdicion.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableEdicion.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 90F));
            tableEdicion.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableEdicion.Controls.Add(lblCodigo, 0, 0);
            tableEdicion.Controls.Add(txtCodigo, 1, 0);
            tableEdicion.Controls.Add(lblTipo, 2, 0);
            tableEdicion.Controls.Add(cboTipo, 3, 0);
            tableEdicion.Controls.Add(lblDescripcion, 0, 1);
            tableEdicion.Controls.Add(txtDescripcion, 1, 1);
            tableEdicion.Controls.Add(lblPadreTitulo, 0, 2);
            tableEdicion.Controls.Add(lblPadre, 1, 2);
            tableEdicion.Controls.Add(chkActivo, 0, 3);
            tableEdicion.Dock = DockStyle.Fill;
            tableEdicion.Location = new Point(10, 26);
            tableEdicion.Name = "tableEdicion";
            tableEdicion.RowCount = 4;
            tableEdicion.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F));
            tableEdicion.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            tableEdicion.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
            tableEdicion.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
            tableEdicion.Size = new Size(706, 124);
            tableEdicion.TabIndex = 0;
            // 
            // lblCodigo
            // 
            lblCodigo.AutoSize = true;
            lblCodigo.Dock = DockStyle.Fill;
            lblCodigo.Location = new Point(3, 0);
            lblCodigo.Name = "lblCodigo";
            lblCodigo.Size = new Size(84, 32);
            lblCodigo.TabIndex = 0;
            lblCodigo.Text = "Código:";
            lblCodigo.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // txtCodigo
            // 
            txtCodigo.Dock = DockStyle.Fill;
            txtCodigo.Location = new Point(93, 6);
            txtCodigo.Margin = new Padding(3, 6, 10, 3);
            txtCodigo.Name = "txtCodigo";
            txtCodigo.Size = new Size(250, 23);
            txtCodigo.TabIndex = 1;
            // 
            // lblTipo
            // 
            lblTipo.AutoSize = true;
            lblTipo.Dock = DockStyle.Fill;
            lblTipo.Location = new Point(356, 0);
            lblTipo.Name = "lblTipo";
            lblTipo.Size = new Size(84, 32);
            lblTipo.TabIndex = 2;
            lblTipo.Text = "Tipo:";
            lblTipo.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // cboTipo
            // 
            cboTipo.Dock = DockStyle.Fill;
            cboTipo.DropDownStyle = ComboBoxStyle.DropDownList;
            cboTipo.FormattingEnabled = true;
            cboTipo.Location = new Point(446, 6);
            cboTipo.Margin = new Padding(3, 6, 3, 3);
            cboTipo.Name = "cboTipo";
            cboTipo.Size = new Size(257, 23);
            cboTipo.TabIndex = 3;
            // 
            // lblDescripcion
            // 
            lblDescripcion.AutoSize = true;
            lblDescripcion.Dock = DockStyle.Fill;
            lblDescripcion.Location = new Point(3, 32);
            lblDescripcion.Name = "lblDescripcion";
            lblDescripcion.Size = new Size(84, 40);
            lblDescripcion.TabIndex = 4;
            lblDescripcion.Text = "Descripción:";
            lblDescripcion.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // txtDescripcion
            // 
            tableEdicion.SetColumnSpan(txtDescripcion, 3);
            txtDescripcion.Dock = DockStyle.Fill;
            txtDescripcion.Location = new Point(93, 38);
            txtDescripcion.Margin = new Padding(3, 6, 3, 3);
            txtDescripcion.Name = "txtDescripcion";
            txtDescripcion.Size = new Size(610, 23);
            txtDescripcion.TabIndex = 5;
            // 
            // lblPadreTitulo
            // 
            lblPadreTitulo.AutoSize = true;
            lblPadreTitulo.Dock = DockStyle.Fill;
            lblPadreTitulo.Location = new Point(3, 72);
            lblPadreTitulo.Name = "lblPadreTitulo";
            lblPadreTitulo.Size = new Size(84, 30);
            lblPadreTitulo.TabIndex = 6;
            lblPadreTitulo.Text = "Padre:";
            lblPadreTitulo.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblPadre
            // 
            lblPadre.AutoSize = true;
            tableEdicion.SetColumnSpan(lblPadre, 3);
            lblPadre.Dock = DockStyle.Fill;
            lblPadre.Location = new Point(93, 72);
            lblPadre.Name = "lblPadre";
            lblPadre.Size = new Size(610, 30);
            lblPadre.TabIndex = 7;
            lblPadre.Text = "Padre: (Raíz)";
            lblPadre.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // chkActivo
            // 
            chkActivo.AutoSize = true;
            chkActivo.Checked = true;
            chkActivo.CheckState = CheckState.Checked;
            tableEdicion.SetColumnSpan(chkActivo, 4);
            chkActivo.Location = new Point(3, 105);
            chkActivo.Name = "chkActivo";
            chkActivo.Size = new Size(98, 19);
            chkActivo.TabIndex = 8;
            chkActivo.Text = "Cuenta activa";
            chkActivo.UseVisualStyleBackColor = true;
            // 
            // panelBotones
            // 
            panelBotones.Controls.Add(btnNuevo);
            panelBotones.Controls.Add(btnGuardar);
            panelBotones.Controls.Add(btnEliminar);
            panelBotones.Controls.Add(btnEstado);
            panelBotones.Dock = DockStyle.Bottom;
            panelBotones.FlowDirection = FlowDirection.RightToLeft;
            panelBotones.Location = new Point(10, 150);
            panelBotones.Name = "panelBotones";
            panelBotones.Padding = new Padding(0, 8, 0, 0);
            panelBotones.Size = new Size(706, 50);
            panelBotones.TabIndex = 1;
            // 
            // btnNuevo
            // 
            btnNuevo.Location = new Point(628, 11);
            btnNuevo.Name = "btnNuevo";
            btnNuevo.Size = new Size(75, 27);
            btnNuevo.TabIndex = 0;
            btnNuevo.Text = "Nuevo";
            btnNuevo.UseVisualStyleBackColor = true;
            btnNuevo.Click += btnNuevo_Click;
            // 
            // btnGuardar
            // 
            btnGuardar.Location = new Point(547, 11);
            btnGuardar.Name = "btnGuardar";
            btnGuardar.Size = new Size(75, 27);
            btnGuardar.TabIndex = 1;
            btnGuardar.Text = "Guardar";
            btnGuardar.UseVisualStyleBackColor = true;
            btnGuardar.Click += btnGuardar_Click;
            // 
            // btnEliminar
            // 
            btnEliminar.Location = new Point(466, 11);
            btnEliminar.Name = "btnEliminar";
            btnEliminar.Size = new Size(75, 27);
            btnEliminar.TabIndex = 2;
            btnEliminar.Text = "Eliminar";
            btnEliminar.UseVisualStyleBackColor = true;
            btnEliminar.Click += btnEliminar_Click;
            // 
            // btnEstado
            // 
            btnEstado.Location = new Point(356, 11);
            btnEstado.Name = "btnEstado";
            btnEstado.Size = new Size(104, 27);
            btnEstado.TabIndex = 3;
            btnEstado.Text = "Activar/Inactivar";
            btnEstado.UseVisualStyleBackColor = true;
            btnEstado.Click += btnEstado_Click;
            // 
            // FormCatalogoCuentas
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1084, 664);
            Controls.Add(splitMain);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Name = "FormCatalogoCuentas";
            Padding = new Padding(12);
            StartPosition = FormStartPosition.CenterParent;
            Text = "Contabilidad - Catálogo de Cuentas";
            Load += FormCatalogoCuentas_Load;
            splitMain.Panel1.ResumeLayout(false);
            splitMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitMain).EndInit();
            splitMain.ResumeLayout(false);
            grpTree.ResumeLayout(false);
            grpListado.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)gridCuentas).EndInit();
            panelTop.ResumeLayout(false);
            panelTop.PerformLayout();
            grpEdicion.ResumeLayout(false);
            tableEdicion.ResumeLayout(false);
            tableEdicion.PerformLayout();
            panelBotones.ResumeLayout(false);
            ResumeLayout(false);

        }
    }
}
