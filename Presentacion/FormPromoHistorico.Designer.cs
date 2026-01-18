using System.Windows.Forms;

namespace Presentation
{
    partial class FormPromoHistorico
    {
        private System.ComponentModel.IContainer components = null;
        private TextBox txtBuscar;
        private Label lblBuscar;
        private CheckBox chkSoloActivas;
        private Button btnBuscar;
        private Button btnNuevo;
        private Button btnEditar;
        private Button btnDesactivar;
        private Button btnCerrar;
        private DataGridView dgvPromos;

        private DataGridViewTextBoxColumn colPromoId;
        private DataGridViewTextBoxColumn colCodigo;
        private DataGridViewTextBoxColumn colNombre;
        private DataGridViewTextBoxColumn colTipo;
        private DataGridViewTextBoxColumn colEstado;
        private DataGridViewTextBoxColumn colFechaInicio;
        private DataGridViewTextBoxColumn colFechaFin;
        private DataGridViewTextBoxColumn colUsuarioCreacion;
        private DataGridViewTextBoxColumn colFechaCreacion;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            txtBuscar = new TextBox();
            lblBuscar = new Label();
            chkSoloActivas = new CheckBox();
            btnBuscar = new Button();
            btnNuevo = new Button();
            btnEditar = new Button();
            btnDesactivar = new Button();
            btnCerrar = new Button();
            dgvPromos = new DataGridView();
            colPromoId = new DataGridViewTextBoxColumn();
            colCodigo = new DataGridViewTextBoxColumn();
            colNombre = new DataGridViewTextBoxColumn();
            colTipo = new DataGridViewTextBoxColumn();
            colEstado = new DataGridViewTextBoxColumn();
            colFechaInicio = new DataGridViewTextBoxColumn();
            colFechaFin = new DataGridViewTextBoxColumn();
            colUsuarioCreacion = new DataGridViewTextBoxColumn();
            colFechaCreacion = new DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)dgvPromos).BeginInit();
            SuspendLayout();
            // 
            // txtBuscar
            // 
            txtBuscar.Location = new Point(64, 12);
            txtBuscar.Name = "txtBuscar";
            txtBuscar.Size = new Size(260, 23);
            txtBuscar.TabIndex = 1;
            txtBuscar.KeyDown += txtBuscar_KeyDown;
            // 
            // lblBuscar
            // 
            lblBuscar.AutoSize = true;
            lblBuscar.Location = new Point(12, 15);
            lblBuscar.Name = "lblBuscar";
            lblBuscar.Size = new Size(45, 15);
            lblBuscar.TabIndex = 0;
            lblBuscar.Text = "Buscar:";
            // 
            // chkSoloActivas
            // 
            chkSoloActivas.AutoSize = true;
            chkSoloActivas.Checked = true;
            chkSoloActivas.CheckState = CheckState.Checked;
            chkSoloActivas.Location = new Point(330, 14);
            chkSoloActivas.Name = "chkSoloActivas";
            chkSoloActivas.Size = new Size(88, 19);
            chkSoloActivas.TabIndex = 2;
            chkSoloActivas.Text = "Solo activas";
            chkSoloActivas.UseVisualStyleBackColor = true;
            chkSoloActivas.CheckedChanged += chkSoloActivas_CheckedChanged;
            // 
            // btnBuscar
            // 
            btnBuscar.Location = new Point(440, 11);
            btnBuscar.Name = "btnBuscar";
            btnBuscar.Size = new Size(75, 25);
            btnBuscar.TabIndex = 3;
            btnBuscar.Text = "Buscar";
            btnBuscar.UseVisualStyleBackColor = true;
            btnBuscar.Click += btnBuscar_Click;
            // 
            // btnNuevo
            // 
            btnNuevo.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnNuevo.Location = new Point(540, 11);
            btnNuevo.Name = "btnNuevo";
            btnNuevo.Size = new Size(75, 25);
            btnNuevo.TabIndex = 4;
            btnNuevo.Text = "Nuevo";
            btnNuevo.UseVisualStyleBackColor = true;
            btnNuevo.Click += btnNuevo_Click;
            // 
            // btnEditar
            // 
            btnEditar.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnEditar.Location = new Point(621, 11);
            btnEditar.Name = "btnEditar";
            btnEditar.Size = new Size(75, 25);
            btnEditar.TabIndex = 5;
            btnEditar.Text = "Editar";
            btnEditar.UseVisualStyleBackColor = true;
            btnEditar.Click += btnEditar_Click;
            // 
            // btnDesactivar
            // 
            btnDesactivar.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnDesactivar.Location = new Point(702, 11);
            btnDesactivar.Name = "btnDesactivar";
            btnDesactivar.Size = new Size(85, 25);
            btnDesactivar.TabIndex = 6;
            btnDesactivar.Text = "Desactivar";
            btnDesactivar.UseVisualStyleBackColor = true;
            btnDesactivar.Click += btnDesactivar_Click;
            // 
            // btnCerrar
            // 
            btnCerrar.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnCerrar.Location = new Point(793, 11);
            btnCerrar.Name = "btnCerrar";
            btnCerrar.Size = new Size(75, 25);
            btnCerrar.TabIndex = 7;
            btnCerrar.Text = "Cerrar";
            btnCerrar.UseVisualStyleBackColor = true;
            btnCerrar.Click += btnCerrar_Click;
            // 
            // dgvPromos
            // 
            dgvPromos.AllowUserToAddRows = false;
            dgvPromos.AllowUserToDeleteRows = false;
            dgvPromos.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgvPromos.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvPromos.Columns.AddRange(new DataGridViewColumn[] { colPromoId, colCodigo, colNombre, colTipo, colEstado, colFechaInicio, colFechaFin, colUsuarioCreacion, colFechaCreacion });
            dgvPromos.Location = new Point(12, 50);
            dgvPromos.MultiSelect = false;
            dgvPromos.Name = "dgvPromos";
            dgvPromos.ReadOnly = true;
            dgvPromos.RowHeadersVisible = false;
            dgvPromos.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvPromos.Size = new Size(856, 439);
            dgvPromos.TabIndex = 8;
            dgvPromos.CellDoubleClick += dgvPromos_CellDoubleClick;
            // 
            // colPromoId
            // 
            colPromoId.DataPropertyName = "PromoId";
            colPromoId.HeaderText = "PromoId";
            colPromoId.Name = "colPromoId";
            colPromoId.ReadOnly = true;
            colPromoId.Visible = false;
            // 
            // colCodigo
            // 
            colCodigo.DataPropertyName = "Codigo";
            colCodigo.HeaderText = "Código";
            colCodigo.Name = "colCodigo";
            colCodigo.ReadOnly = true;
            colCodigo.Width = 90;
            // 
            // colNombre
            // 
            colNombre.DataPropertyName = "Nombre";
            colNombre.HeaderText = "Nombre";
            colNombre.Name = "colNombre";
            colNombre.ReadOnly = true;
            colNombre.Width = 220;
            // 
            // colTipo
            // 
            colTipo.DataPropertyName = "TipoPromo";
            colTipo.HeaderText = "Tipo";
            colTipo.Name = "colTipo";
            colTipo.ReadOnly = true;
            colTipo.Width = 90;
            // 
            // colEstado
            // 
            colEstado.DataPropertyName = "Estado";
            colEstado.HeaderText = "Estado";
            colEstado.Name = "colEstado";
            colEstado.ReadOnly = true;
            colEstado.Width = 80;
            // 
            // colFechaInicio
            // 
            colFechaInicio.DataPropertyName = "FechaInicio";
            colFechaInicio.HeaderText = "Fecha inicio";
            colFechaInicio.Name = "colFechaInicio";
            colFechaInicio.ReadOnly = true;
            colFechaInicio.Width = 90;
            // 
            // colFechaFin
            // 
            colFechaFin.DataPropertyName = "FechaFin";
            colFechaFin.HeaderText = "Fecha fin";
            colFechaFin.Name = "colFechaFin";
            colFechaFin.ReadOnly = true;
            colFechaFin.Width = 90;
            // 
            // colUsuarioCreacion
            // 
            colUsuarioCreacion.DataPropertyName = "UsuarioCreacion";
            colUsuarioCreacion.HeaderText = "Creado/Usuario";
            colUsuarioCreacion.Name = "colUsuarioCreacion";
            colUsuarioCreacion.ReadOnly = true;
            colUsuarioCreacion.Width = 120;
            // 
            // colFechaCreacion
            // 
            colFechaCreacion.DataPropertyName = "FechaCreacion";
            colFechaCreacion.HeaderText = "Fecha creación";
            colFechaCreacion.Name = "colFechaCreacion";
            colFechaCreacion.ReadOnly = true;
            colFechaCreacion.Width = 120;
            // 
            // FormPromoHistorico
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(880, 501);
            Controls.Add(dgvPromos);
            Controls.Add(btnCerrar);
            Controls.Add(btnDesactivar);
            Controls.Add(btnEditar);
            Controls.Add(btnNuevo);
            Controls.Add(btnBuscar);
            Controls.Add(chkSoloActivas);
            Controls.Add(txtBuscar);
            Controls.Add(lblBuscar);
            Name = "FormPromoHistorico";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Histórico de promociones";
            Load += FormPromoHistorico_Load;
            ((System.ComponentModel.ISupportInitialize)dgvPromos).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
