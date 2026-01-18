namespace Andloe.Presentacion
{
    partial class FormTerminoPago
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabLista = new System.Windows.Forms.TabPage();
            this.btnToggle = new System.Windows.Forms.Button();
            this.btnEditar = new System.Windows.Forms.Button();
            this.btnNuevo = new System.Windows.Forms.Button();
            this.btnRefrescar = new System.Windows.Forms.Button();
            this.chkSoloActivos = new System.Windows.Forms.CheckBox();
            this.txtBuscar = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.dgv = new System.Windows.Forms.DataGridView();
            this.tabEditor = new System.Windows.Forms.TabPage();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.btnGuardar = new System.Windows.Forms.Button();
            this.txtUsuario = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.chkActivo = new System.Windows.Forms.CheckBox();
            this.numDiasDesc = new System.Windows.Forms.NumericUpDown();
            this.numPorcDesc = new System.Windows.Forms.NumericUpDown();
            this.chkDescuento = new System.Windows.Forms.CheckBox();
            this.numDias = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.txtDescripcion = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtCodigo = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtId = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tabControl1.SuspendLayout();
            this.tabLista.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).BeginInit();
            this.tabEditor.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numDiasDesc)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPorcDesc)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numDias)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabLista);
            this.tabControl1.Controls.Add(this.tabEditor);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(920, 520);
            this.tabControl1.TabIndex = 0;
            // 
            // tabLista
            // 
            this.tabLista.Controls.Add(this.btnToggle);
            this.tabLista.Controls.Add(this.btnEditar);
            this.tabLista.Controls.Add(this.btnNuevo);
            this.tabLista.Controls.Add(this.btnRefrescar);
            this.tabLista.Controls.Add(this.chkSoloActivos);
            this.tabLista.Controls.Add(this.txtBuscar);
            this.tabLista.Controls.Add(this.label1);
            this.tabLista.Controls.Add(this.dgv);
            this.tabLista.Location = new System.Drawing.Point(4, 24);
            this.tabLista.Name = "tabLista";
            this.tabLista.Padding = new System.Windows.Forms.Padding(3);
            this.tabLista.Size = new System.Drawing.Size(912, 492);
            this.tabLista.TabIndex = 0;
            this.tabLista.Text = "Listado";
            this.tabLista.UseVisualStyleBackColor = true;
            // 
            // btnToggle
            // 
            this.btnToggle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnToggle.Location = new System.Drawing.Point(785, 44);
            this.btnToggle.Name = "btnToggle";
            this.btnToggle.Size = new System.Drawing.Size(119, 27);
            this.btnToggle.TabIndex = 7;
            this.btnToggle.Text = "Activar/Inactivar";
            this.btnToggle.UseVisualStyleBackColor = true;
            this.btnToggle.Click += new System.EventHandler(this.btnToggle_Click);
            // 
            // btnEditar
            // 
            this.btnEditar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnEditar.Location = new System.Drawing.Point(785, 11);
            this.btnEditar.Name = "btnEditar";
            this.btnEditar.Size = new System.Drawing.Size(119, 27);
            this.btnEditar.TabIndex = 6;
            this.btnEditar.Text = "Editar";
            this.btnEditar.UseVisualStyleBackColor = true;
            this.btnEditar.Click += new System.EventHandler(this.btnEditar_Click);
            // 
            // btnNuevo
            // 
            this.btnNuevo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNuevo.Location = new System.Drawing.Point(660, 11);
            this.btnNuevo.Name = "btnNuevo";
            this.btnNuevo.Size = new System.Drawing.Size(119, 27);
            this.btnNuevo.TabIndex = 5;
            this.btnNuevo.Text = "Nuevo";
            this.btnNuevo.UseVisualStyleBackColor = true;
            this.btnNuevo.Click += new System.EventHandler(this.btnNuevo_Click);
            // 
            // btnRefrescar
            // 
            this.btnRefrescar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRefrescar.Location = new System.Drawing.Point(660, 44);
            this.btnRefrescar.Name = "btnRefrescar";
            this.btnRefrescar.Size = new System.Drawing.Size(119, 27);
            this.btnRefrescar.TabIndex = 4;
            this.btnRefrescar.Text = "Refrescar";
            this.btnRefrescar.UseVisualStyleBackColor = true;
            this.btnRefrescar.Click += new System.EventHandler(this.btnRefrescar_Click);
            // 
            // chkSoloActivos
            // 
            this.chkSoloActivos.AutoSize = true;
            this.chkSoloActivos.Location = new System.Drawing.Point(89, 46);
            this.chkSoloActivos.Name = "chkSoloActivos";
            this.chkSoloActivos.Size = new System.Drawing.Size(93, 19);
            this.chkSoloActivos.TabIndex = 3;
            this.chkSoloActivos.Text = "Solo activos";
            this.chkSoloActivos.UseVisualStyleBackColor = true;
            this.chkSoloActivos.CheckedChanged += new System.EventHandler(this.chkSoloActivos_CheckedChanged);
            // 
            // txtBuscar
            // 
            this.txtBuscar.Location = new System.Drawing.Point(89, 13);
            this.txtBuscar.Name = "txtBuscar";
            this.txtBuscar.Size = new System.Drawing.Size(382, 23);
            this.txtBuscar.TabIndex = 2;
            this.txtBuscar.TextChanged += new System.EventHandler(this.txtBuscar_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(23, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(45, 15);
            this.label1.TabIndex = 1;
            this.label1.Text = "Buscar:";
            // 
            // dgv
            // 
            this.dgv.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgv.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv.Location = new System.Drawing.Point(8, 81);
            this.dgv.Name = "dgv";
            this.dgv.Size = new System.Drawing.Size(896, 403);
            this.dgv.TabIndex = 0;
            // 
            // tabEditor
            // 
            this.tabEditor.Controls.Add(this.btnCancelar);
            this.tabEditor.Controls.Add(this.btnGuardar);
            this.tabEditor.Controls.Add(this.txtUsuario);
            this.tabEditor.Controls.Add(this.label8);
            this.tabEditor.Controls.Add(this.chkActivo);
            this.tabEditor.Controls.Add(this.numDiasDesc);
            this.tabEditor.Controls.Add(this.numPorcDesc);
            this.tabEditor.Controls.Add(this.chkDescuento);
            this.tabEditor.Controls.Add(this.numDias);
            this.tabEditor.Controls.Add(this.label6);
            this.tabEditor.Controls.Add(this.txtDescripcion);
            this.tabEditor.Controls.Add(this.label5);
            this.tabEditor.Controls.Add(this.txtCodigo);
            this.tabEditor.Controls.Add(this.label4);
            this.tabEditor.Controls.Add(this.txtId);
            this.tabEditor.Controls.Add(this.label3);
            this.tabEditor.Controls.Add(this.label2);
            this.tabEditor.Location = new System.Drawing.Point(4, 24);
            this.tabEditor.Name = "tabEditor";
            this.tabEditor.Padding = new System.Windows.Forms.Padding(3);
            this.tabEditor.Size = new System.Drawing.Size(912, 492);
            this.tabEditor.TabIndex = 1;
            this.tabEditor.Text = "Editor";
            this.tabEditor.UseVisualStyleBackColor = true;
            // 
            // btnCancelar
            // 
            this.btnCancelar.Location = new System.Drawing.Point(214, 383);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(130, 32);
            this.btnCancelar.TabIndex = 16;
            this.btnCancelar.Text = "Volver";
            this.btnCancelar.UseVisualStyleBackColor = true;
            this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);
            // 
            // btnGuardar
            // 
            this.btnGuardar.Location = new System.Drawing.Point(78, 383);
            this.btnGuardar.Name = "btnGuardar";
            this.btnGuardar.Size = new System.Drawing.Size(130, 32);
            this.btnGuardar.TabIndex = 15;
            this.btnGuardar.Text = "Guardar";
            this.btnGuardar.UseVisualStyleBackColor = true;
            this.btnGuardar.Click += new System.EventHandler(this.btnGuardar_Click);
            // 
            // txtUsuario
            // 
            this.txtUsuario.Location = new System.Drawing.Point(78, 327);
            this.txtUsuario.Name = "txtUsuario";
            this.txtUsuario.Size = new System.Drawing.Size(266, 23);
            this.txtUsuario.TabIndex = 14;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(23, 330);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(50, 15);
            this.label8.TabIndex = 13;
            this.label8.Text = "Usuario:";
            // 
            // chkActivo
            // 
            this.chkActivo.AutoSize = true;
            this.chkActivo.Location = new System.Drawing.Point(78, 291);
            this.chkActivo.Name = "chkActivo";
            this.chkActivo.Size = new System.Drawing.Size(60, 19);
            this.chkActivo.TabIndex = 12;
            this.chkActivo.Text = "Activo";
            this.chkActivo.UseVisualStyleBackColor = true;
            // 
            // numDiasDesc
            // 
            this.numDiasDesc.Enabled = false;
            this.numDiasDesc.Location = new System.Drawing.Point(214, 252);
            this.numDiasDesc.Maximum = new decimal(new int[] {
            3650,
            0,
            0,
            0});
            this.numDiasDesc.Name = "numDiasDesc";
            this.numDiasDesc.Size = new System.Drawing.Size(130, 23);
            this.numDiasDesc.TabIndex = 11;
            // 
            // numPorcDesc
            // 
            this.numPorcDesc.DecimalPlaces = 2;
            this.numPorcDesc.Enabled = false;
            this.numPorcDesc.Location = new System.Drawing.Point(78, 252);
            this.numPorcDesc.Maximum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numPorcDesc.Name = "numPorcDesc";
            this.numPorcDesc.Size = new System.Drawing.Size(130, 23);
            this.numPorcDesc.TabIndex = 10;
            // 
            // chkDescuento
            // 
            this.chkDescuento.AutoSize = true;
            this.chkDescuento.Location = new System.Drawing.Point(78, 219);
            this.chkDescuento.Name = "chkDescuento";
            this.chkDescuento.Size = new System.Drawing.Size(174, 19);
            this.chkDescuento.TabIndex = 9;
            this.chkDescuento.Text = "Tiene descuento (% y días)";
            this.chkDescuento.UseVisualStyleBackColor = true;
            this.chkDescuento.CheckedChanged += new System.EventHandler(this.chkDescuento_CheckedChanged);
            // 
            // numDias
            // 
            this.numDias.Location = new System.Drawing.Point(78, 181);
            this.numDias.Maximum = new decimal(new int[] {
            3650,
            0,
            0,
            0});
            this.numDias.Name = "numDias";
            this.numDias.Size = new System.Drawing.Size(130, 23);
            this.numDias.TabIndex = 8;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(23, 183);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(33, 15);
            this.label6.TabIndex = 7;
            this.label6.Text = "Días:";
            // 
            // txtDescripcion
            // 
            this.txtDescripcion.Location = new System.Drawing.Point(78, 141);
            this.txtDescripcion.Name = "txtDescripcion";
            this.txtDescripcion.Size = new System.Drawing.Size(480, 23);
            this.txtDescripcion.TabIndex = 6;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(23, 144);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(72, 15);
            this.label5.TabIndex = 5;
            this.label5.Text = "Descripción:";
            // 
            // txtCodigo
            // 
            this.txtCodigo.Location = new System.Drawing.Point(78, 103);
            this.txtCodigo.Name = "txtCodigo";
            this.txtCodigo.Size = new System.Drawing.Size(266, 23);
            this.txtCodigo.TabIndex = 4;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(23, 106);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(47, 15);
            this.label4.TabIndex = 3;
            this.label4.Text = "Código:";
            // 
            // txtId
            // 
            this.txtId.Location = new System.Drawing.Point(78, 64);
            this.txtId.Name = "txtId";
            this.txtId.ReadOnly = true;
            this.txtId.Size = new System.Drawing.Size(130, 23);
            this.txtId.TabIndex = 2;
            this.txtId.Text = "0";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(23, 67);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(21, 15);
            this.label3.TabIndex = 1;
            this.label3.Text = "ID:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.label2.Location = new System.Drawing.Point(23, 20);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(148, 21);
            this.label2.TabIndex = 0;
            this.label2.Text = "Editar Término Pago";
            // 
            // FormTerminoPago
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(920, 520);
            this.Controls.Add(this.tabControl1);
            this.Name = "FormTerminoPago";
            this.Text = "Términos de Pago";
            this.Load += new System.EventHandler(this.FormTerminoPago_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabLista.ResumeLayout(false);
            this.tabLista.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).EndInit();
            this.tabEditor.ResumeLayout(false);
            this.tabEditor.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numDiasDesc)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPorcDesc)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numDias)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabLista;
        private System.Windows.Forms.TabPage tabEditor;
        private System.Windows.Forms.DataGridView dgv;
        private System.Windows.Forms.TextBox txtBuscar;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox chkSoloActivos;
        private System.Windows.Forms.Button btnRefrescar;
        private System.Windows.Forms.Button btnNuevo;
        private System.Windows.Forms.Button btnEditar;
        private System.Windows.Forms.Button btnToggle;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtId;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtCodigo;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtDescripcion;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown numDias;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.CheckBox chkDescuento;
        private System.Windows.Forms.NumericUpDown numPorcDesc;
        private System.Windows.Forms.NumericUpDown numDiasDesc;
        private System.Windows.Forms.CheckBox chkActivo;
        private System.Windows.Forms.TextBox txtUsuario;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button btnGuardar;
        private System.Windows.Forms.Button btnCancelar;
    }
}
