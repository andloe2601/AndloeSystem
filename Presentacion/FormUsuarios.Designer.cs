namespace Andloe.Presentacion
{
    partial class FormUsuarios
    {
        private System.ComponentModel.IContainer components = null;

        #region Windows Form Designer generated code
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();

            // Top gradient header (Paint handler added in code-behind)
            this.headerPanel = new System.Windows.Forms.Panel();
            this.lblTitulo = new System.Windows.Forms.Label();

            // Search box with icon and clear button
            this.searchContainer = new System.Windows.Forms.Panel();
            this.picSearch = new System.Windows.Forms.PictureBox();
            this.txtBuscar = new System.Windows.Forms.TextBox();
            this.btnLimpiarBusqueda = new System.Windows.Forms.Button();

            // Right-side actions (modern buttons)
            this.actionFlow = new System.Windows.Forms.FlowLayoutPanel();
            this.btnRefrescar = new System.Windows.Forms.Button();
            this.btnNuevo = new System.Windows.Forms.Button();
            this.btnEditar = new System.Windows.Forms.Button();
            this.btnEliminar = new System.Windows.Forms.Button();

            // Main content area with "card" look
            this.mainPadding = new System.Windows.Forms.Panel();
            this.cardPanel = new System.Windows.Forms.Panel();
            this.dgvUsuarios = new System.Windows.Forms.DataGridView();

            // Columns (names preserved)
            this.colId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colUsuario = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colEmail = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colEstado = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colUltimoAcceso = new System.Windows.Forms.DataGridViewTextBoxColumn();

            // Empty-state friendly overlay
            this.emptyPanel = new System.Windows.Forms.Panel();
            this.lblEmptyTitle = new System.Windows.Forms.Label();
            this.lblEmptyDesc = new System.Windows.Forms.Label();
            this.btnEmptyNew = new System.Windows.Forms.Button();

            // Status strip with info and progress
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.lblInfo = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolProgress = new System.Windows.Forms.ToolStripProgressBar();

            // Context menu for grid
            this.ctxMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ctxMenuNuevo = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxMenuEditar = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxMenuEliminar = new System.Windows.Forms.ToolStripMenuItem();

            // Tooltips
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);

            // Begin layout
            this.headerPanel.SuspendLayout();
            this.searchContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picSearch)).BeginInit();
            this.actionFlow.SuspendLayout();
            this.mainPadding.SuspendLayout();
            this.cardPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvUsuarios)).BeginInit();
            this.emptyPanel.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.ctxMenu.SuspendLayout();
            this.SuspendLayout();

            // headerPanel - modern gradient header
            this.headerPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.headerPanel.Height = 96;
            this.headerPanel.Padding = new System.Windows.Forms.Padding(20, 18, 20, 12);
            this.headerPanel.BackColor = System.Drawing.Color.White;
            this.headerPanel.Controls.Add(this.actionFlow);
            this.headerPanel.Controls.Add(this.searchContainer);
            this.headerPanel.Controls.Add(this.lblTitulo);
            this.headerPanel.Name = "headerPanel";

            // lblTitulo - title
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI Semibold", 18F, System.Drawing.FontStyle.Bold);
            this.lblTitulo.ForeColor = System.Drawing.Color.FromArgb(34, 40, 49);
            this.lblTitulo.Location = new System.Drawing.Point(20, 26);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Text = "Usuarios";

            // searchContainer - rounded search appearance
            this.searchContainer.BackColor = System.Drawing.Color.FromArgb(245, 247, 250);
            this.searchContainer.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.searchContainer.Padding = new System.Windows.Forms.Padding(12, 8, 8, 8);
            this.searchContainer.Location = new System.Drawing.Point(200, 30);
            this.searchContainer.Name = "searchContainer";
            this.searchContainer.Size = new System.Drawing.Size(460, 36);
            this.searchContainer.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left;

            // picSearch - icon placeholder
            this.picSearch.Location = new System.Drawing.Point(12, 8);
            this.picSearch.Size = new System.Drawing.Size(20, 20);
            this.picSearch.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.picSearch.Name = "picSearch";
            this.picSearch.TabStop = false;
            // Optionally set an image in code-behind: picSearch.Image = Properties.Resources.icon_search;

            // txtBuscar
            this.txtBuscar.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtBuscar.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtBuscar.BackColor = System.Drawing.Color.FromArgb(245, 247, 250);
            this.txtBuscar.Location = new System.Drawing.Point(44, 10);
            this.txtBuscar.Name = "txtBuscar";
            this.txtBuscar.PlaceholderText = "Buscar por usuario o correo (Enter para buscar)";
            this.txtBuscar.Width = 340;

            // btnLimpiarBusqueda
            this.btnLimpiarBusqueda.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLimpiarBusqueda.FlatAppearance.BorderSize = 0;
            this.btnLimpiarBusqueda.BackColor = System.Drawing.Color.Transparent;
            this.btnLimpiarBusqueda.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnLimpiarBusqueda.Size = new System.Drawing.Size(22, 22);
            this.btnLimpiarBusqueda.Location = new System.Drawing.Point(412, 7);
            this.btnLimpiarBusqueda.Name = "btnLimpiarBusqueda";
            this.btnLimpiarBusqueda.Text = "✕";
            this.btnLimpiarBusqueda.UseVisualStyleBackColor = true;
            this.toolTip.SetToolTip(this.btnLimpiarBusqueda, "Limpiar búsqueda");

            // Add search children
            this.searchContainer.Controls.Add(this.picSearch);
            this.searchContainer.Controls.Add(this.txtBuscar);
            this.searchContainer.Controls.Add(this.btnLimpiarBusqueda);

            // actionFlow - right-aligned modern buttons
            this.actionFlow.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            this.actionFlow.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.actionFlow.Location = new System.Drawing.Point(680, 24);
            this.actionFlow.Name = "actionFlow";
            this.actionFlow.Size = new System.Drawing.Size(300, 48);
            this.actionFlow.Padding = new System.Windows.Forms.Padding(0, 6, 0, 0);

            // btnRefrescar
            this.btnRefrescar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRefrescar.FlatAppearance.BorderSize = 0;
            this.btnRefrescar.BackColor = System.Drawing.Color.FromArgb(245, 247, 250);
            this.btnRefrescar.ForeColor = System.Drawing.Color.FromArgb(36, 41, 45);
            this.btnRefrescar.Margin = new System.Windows.Forms.Padding(8, 6, 0, 6);
            this.btnRefrescar.Name = "btnRefrescar";
            this.btnRefrescar.Size = new System.Drawing.Size(88, 36);
            this.btnRefrescar.Text = "Refrescar";
            this.btnRefrescar.UseVisualStyleBackColor = true;

            // btnNuevo
            this.btnNuevo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNuevo.FlatAppearance.BorderSize = 0;
            this.btnNuevo.BackColor = System.Drawing.Color.FromArgb(0, 123, 255);
            this.btnNuevo.ForeColor = System.Drawing.Color.White;
            this.btnNuevo.Margin = new System.Windows.Forms.Padding(8, 6, 0, 6);
            this.btnNuevo.Name = "btnNuevo";
            this.btnNuevo.Size = new System.Drawing.Size(88, 36);
            this.btnNuevo.Text = "Nuevo";
            this.btnNuevo.UseVisualStyleBackColor = true;

            // btnEditar
            this.btnEditar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEditar.FlatAppearance.BorderSize = 0;
            this.btnEditar.BackColor = System.Drawing.Color.FromArgb(40, 167, 69);
            this.btnEditar.ForeColor = System.Drawing.Color.White;
            this.btnEditar.Margin = new System.Windows.Forms.Padding(8, 6, 0, 6);
            this.btnEditar.Name = "btnEditar";
            this.btnEditar.Size = new System.Drawing.Size(88, 36);
            this.btnEditar.Text = "Editar";
            this.btnEditar.UseVisualStyleBackColor = true;

            // btnEliminar
            this.btnEliminar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEliminar.FlatAppearance.BorderSize = 0;
            this.btnEliminar.BackColor = System.Drawing.Color.FromArgb(220, 53, 69);
            this.btnEliminar.ForeColor = System.Drawing.Color.White;
            this.btnEliminar.Margin = new System.Windows.Forms.Padding(8, 6, 0, 6);
            this.btnEliminar.Name = "btnEliminar";
            this.btnEliminar.Size = new System.Drawing.Size(88, 36);
            this.btnEliminar.Text = "Eliminar";
            this.btnEliminar.UseVisualStyleBackColor = true;

            // Add buttons to flow
            this.actionFlow.Controls.Add(this.btnRefrescar);
            this.actionFlow.Controls.Add(this.btnNuevo);
            this.actionFlow.Controls.Add(this.btnEditar);
            this.actionFlow.Controls.Add(this.btnEliminar);

            // mainPadding
            this.mainPadding.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainPadding.Padding = new System.Windows.Forms.Padding(20);
            this.mainPadding.BackColor = System.Drawing.Color.FromArgb(248, 249, 250);
            this.mainPadding.Controls.Add(this.cardPanel);
            this.mainPadding.Name = "mainPadding";

            // cardPanel
            this.cardPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cardPanel.BackColor = System.Drawing.Color.White;
            this.cardPanel.Padding = new System.Windows.Forms.Padding(14);
            this.cardPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.cardPanel.Controls.Add(this.dgvUsuarios);
            this.cardPanel.Controls.Add(this.emptyPanel);
            this.cardPanel.Name = "cardPanel";

            // dgvUsuarios
            this.dgvUsuarios.AllowUserToAddRows = false;
            this.dgvUsuarios.AllowUserToDeleteRows = false;
            this.dgvUsuarios.AutoGenerateColumns = false;
            this.dgvUsuarios.BackgroundColor = System.Drawing.Color.White;
            this.dgvUsuarios.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvUsuarios.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvUsuarios.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
                this.colId, this.colUsuario, this.colEmail, this.colEstado, this.colUltimoAcceso
            });
            this.dgvUsuarios.ContextMenuStrip = this.ctxMenu;
            this.dgvUsuarios.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvUsuarios.Location = new System.Drawing.Point(14, 14);
            this.dgvUsuarios.MultiSelect = false;
            this.dgvUsuarios.Name = "dgvUsuarios";
            this.dgvUsuarios.ReadOnly = true;
            this.dgvUsuarios.RowHeadersVisible = false;
            this.dgvUsuarios.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvUsuarios.Size = new System.Drawing.Size(860, 420);
            this.dgvUsuarios.EnableHeadersVisualStyles = false;
            this.dgvUsuarios.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(38, 50, 56);
            this.dgvUsuarios.ColumnHeadersDefaultCellStyle.ForeColor = System.Drawing.Color.White;
            this.dgvUsuarios.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.dgvUsuarios.AlternatingRowsDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(249, 250, 252);
            this.dgvUsuarios.RowTemplate.Height = 28;
            this.dgvUsuarios.GridColor = System.Drawing.Color.FromArgb(235, 236, 240);
            this.dgvUsuarios.Font = new System.Drawing.Font("Segoe UI", 9F);

            // Default selection style will be applied in code-behind for cross-version safety.

            // Columns definition
            this.colId.DataPropertyName = "UsuarioId";
            this.colId.HeaderText = "ID";
            this.colId.Name = "colId";
            this.colId.ReadOnly = true;
            this.colId.Width = 70;
            this.colId.DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;

            this.colUsuario.DataPropertyName = "UsuarioNombre";
            this.colUsuario.HeaderText = "Usuario";
            this.colUsuario.Name = "colUsuario";
            this.colUsuario.ReadOnly = true;
            this.colUsuario.Width = 240;

            this.colEmail.DataPropertyName = "Email";
            this.colEmail.HeaderText = "Email";
            this.colEmail.Name = "colEmail";
            this.colEmail.ReadOnly = true;
            this.colEmail.Width = 300;

            this.colEstado.DataPropertyName = "Estado";
            this.colEstado.HeaderText = "Estado";
            this.colEstado.Name = "colEstado";
            this.colEstado.ReadOnly = true;
            this.colEstado.Width = 120;
            this.colEstado.DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;

            this.colUltimoAcceso.DataPropertyName = "UltimoAcceso";
            this.colUltimoAcceso.HeaderText = "Último acceso";
            this.colUltimoAcceso.Name = "colUltimoAcceso";
            this.colUltimoAcceso.ReadOnly = true;
            this.colUltimoAcceso.Width = 200;
            this.colUltimoAcceso.DefaultCellStyle.Format = "g";

            // emptyPanel
            this.emptyPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.emptyPanel.BackColor = System.Drawing.Color.Transparent;
            this.emptyPanel.Controls.Add(this.lblEmptyTitle);
            this.emptyPanel.Controls.Add(this.lblEmptyDesc);
            this.emptyPanel.Controls.Add(this.btnEmptyNew);
            this.emptyPanel.Name = "emptyPanel";
            this.emptyPanel.Visible = false;

            this.lblEmptyTitle.AutoSize = true;
            this.lblEmptyTitle.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblEmptyTitle.ForeColor = System.Drawing.Color.FromArgb(54, 62, 71);
            this.lblEmptyTitle.Location = new System.Drawing.Point(40, 80);
            this.lblEmptyTitle.Text = "No hay usuarios";

            this.lblEmptyDesc.AutoSize = true;
            this.lblEmptyDesc.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblEmptyDesc.ForeColor = System.Drawing.Color.Gray;
            this.lblEmptyDesc.Location = new System.Drawing.Point(40, 120);
            this.lblEmptyDesc.MaximumSize = new System.Drawing.Size(520, 0);
            this.lblEmptyDesc.Text = "Aún no hay registros. Usa el botón \"Nuevo\" para crear tu primer usuario o importa usuarios desde un archivo.";

            this.btnEmptyNew.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEmptyNew.BackColor = System.Drawing.Color.FromArgb(0, 123, 255);
            this.btnEmptyNew.ForeColor = System.Drawing.Color.White;
            this.btnEmptyNew.Location = new System.Drawing.Point(40, 170);
            this.btnEmptyNew.Size = new System.Drawing.Size(140, 36);
            this.btnEmptyNew.Text = "Nuevo usuario";
            this.btnEmptyNew.UseVisualStyleBackColor = true;

            // statusStrip
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.lblInfo,
                this.toolProgress
            });
            this.statusStrip.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.statusStrip.BackColor = System.Drawing.Color.WhiteSmoke;
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(1000, 26);

            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Text = "Listo";

            this.toolProgress.Name = "toolProgress";
            this.toolProgress.Size = new System.Drawing.Size(120, 16);
            this.toolProgress.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.toolProgress.Visible = false;

            // ctxMenu
            this.ctxMenu.ImageScalingSize = new System.Drawing.Size(18, 18);
            this.ctxMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.ctxMenuNuevo,
                this.ctxMenuEditar,
                this.ctxMenuEliminar
            });
            this.ctxMenu.Name = "ctxMenu";

            this.ctxMenuNuevo.Name = "ctxMenuNuevo";
            this.ctxMenuNuevo.Text = "Nuevo";

            this.ctxMenuEditar.Name = "ctxMenuEditar";
            this.ctxMenuEditar.Text = "Editar";

            this.ctxMenuEliminar.Name = "ctxMenuEliminar";
            this.ctxMenuEliminar.Text = "Eliminar";

            // Tooltips defaults
            this.toolTip.AutoPopDelay = 5000;
            this.toolTip.InitialDelay = 600;
            this.toolTip.ReshowDelay = 120;

            // FormUsuarios properties
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1000, 640);
            this.Controls.Add(this.mainPadding);
            this.Controls.Add(this.headerPanel);
            this.Controls.Add(this.statusStrip);
            this.MinimumSize = new System.Drawing.Size(960, 520);
            this.Name = "FormUsuarios";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Usuarios";

            // complete layout
            this.headerPanel.ResumeLayout(false);
            this.headerPanel.PerformLayout();
            this.searchContainer.ResumeLayout(false);
            this.searchContainer.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picSearch)).EndInit();
            this.actionFlow.ResumeLayout(false);
            this.mainPadding.ResumeLayout(false);
            this.cardPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvUsuarios)).EndInit();
            this.emptyPanel.ResumeLayout(false);
            this.emptyPanel.PerformLayout();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ctxMenu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();
        }
        #endregion

        // Controls (names preserved)
        private System.Windows.Forms.Panel headerPanel;
        private System.Windows.Forms.Label lblTitulo;

        private System.Windows.Forms.Panel searchContainer;
        private System.Windows.Forms.PictureBox picSearch;
        private System.Windows.Forms.TextBox txtBuscar;
        private System.Windows.Forms.Button btnLimpiarBusqueda;

        private System.Windows.Forms.FlowLayoutPanel actionFlow;
        private System.Windows.Forms.Button btnRefrescar;
        private System.Windows.Forms.Button btnNuevo;
        private System.Windows.Forms.Button btnEditar;
        private System.Windows.Forms.Button btnEliminar;

        private System.Windows.Forms.Panel mainPadding;
        private System.Windows.Forms.Panel cardPanel;
        private System.Windows.Forms.DataGridView dgvUsuarios;

        private System.Windows.Forms.DataGridViewTextBoxColumn colId;
        private System.Windows.Forms.DataGridViewTextBoxColumn colUsuario;
        private System.Windows.Forms.DataGridViewTextBoxColumn colEmail;
        private System.Windows.Forms.DataGridViewTextBoxColumn colEstado;
        private System.Windows.Forms.DataGridViewTextBoxColumn colUltimoAcceso;

        private System.Windows.Forms.Panel emptyPanel;
        private System.Windows.Forms.Label lblEmptyTitle;
        private System.Windows.Forms.Label lblEmptyDesc;
        private System.Windows.Forms.Button btnEmptyNew;

        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel lblInfo;
        private System.Windows.Forms.ToolStripProgressBar toolProgress;

        private System.Windows.Forms.ContextMenuStrip ctxMenu;
        private System.Windows.Forms.ToolStripMenuItem ctxMenuNuevo;
        private System.Windows.Forms.ToolStripMenuItem ctxMenuEditar;
        private System.Windows.Forms.ToolStripMenuItem ctxMenuEliminar;

        private System.Windows.Forms.ToolTip toolTip;
    }
}