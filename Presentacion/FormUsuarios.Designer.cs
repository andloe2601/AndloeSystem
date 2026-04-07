namespace Andloe.Presentacion
{
    partial class FormUsuarios
    {
        private System.ComponentModel.IContainer components = null;

        #region Windows Form Designer generated code
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            headerPanel = new Panel();
            actionFlow = new FlowLayoutPanel();
            btnRefrescar = new Button();
            btnNuevo = new Button();
            btnEditar = new Button();
            btnEliminar = new Button();
            btnDetalle = new Button();
            btnEditarForm = new Button();
            searchContainer = new Panel();
            picSearch = new PictureBox();
            txtBuscar = new TextBox();
            btnLimpiarBusqueda = new Button();
            lblTitulo = new Label();
            mainPadding = new Panel();
            cardPanel = new Panel();
            dgvUsuarios = new DataGridView();
            ctxMenu = new ContextMenuStrip(components);
            ctxMenuNuevo = new ToolStripMenuItem();
            ctxMenuEditar = new ToolStripMenuItem();
            ctxMenuEliminar = new ToolStripMenuItem();
            emptyPanel = new Panel();
            lblEmptyTitle = new Label();
            lblEmptyDesc = new Label();
            btnEmptyNew = new Button();
            statusStrip = new StatusStrip();
            lblInfo = new ToolStripStatusLabel();
            toolProgress = new ToolStripProgressBar();
            toolTip = new ToolTip(components);
            headerPanel.SuspendLayout();
            actionFlow.SuspendLayout();
            searchContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picSearch).BeginInit();
            mainPadding.SuspendLayout();
            cardPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvUsuarios).BeginInit();
            ctxMenu.SuspendLayout();
            emptyPanel.SuspendLayout();
            statusStrip.SuspendLayout();
            SuspendLayout();
            // 
            // headerPanel
            // 
            headerPanel.BackColor = Color.White;
            headerPanel.Controls.Add(actionFlow);
            headerPanel.Controls.Add(searchContainer);
            headerPanel.Controls.Add(lblTitulo);
            headerPanel.Dock = DockStyle.Top;
            headerPanel.Location = new Point(0, 0);
            headerPanel.Name = "headerPanel";
            headerPanel.Padding = new Padding(20, 18, 20, 12);
            headerPanel.Size = new Size(1000, 96);
            headerPanel.TabIndex = 2;
            actionFlow.Controls.Add(btnRefrescar);
            actionFlow.Controls.Add(btnNuevo);
            actionFlow.Controls.Add(btnEditar);
            actionFlow.Controls.Add(btnEliminar);
            actionFlow.Controls.Add(btnDetalle);
            actionFlow.Controls.Add(btnEditarForm);
            // 

            // searchContainer
            searchContainer.BackColor = Color.FromArgb(245, 247, 250);
            searchContainer.Controls.Add(picSearch);
            searchContainer.Controls.Add(txtBuscar);
            searchContainer.Controls.Add(btnLimpiarBusqueda);
            searchContainer.Location = new Point(200, 30);
            searchContainer.Name = "searchContainer";
            searchContainer.Padding = new Padding(12, 8, 8, 8);
            searchContainer.Size = new Size(380, 36);
            searchContainer.TabIndex = 1;
            // 
            // btnRefrescar
            // 
            btnRefrescar.BackColor = Color.FromArgb(245, 247, 250);
            btnRefrescar.FlatAppearance.BorderSize = 0;
            btnRefrescar.FlatStyle = FlatStyle.Flat;
            btnRefrescar.ForeColor = Color.FromArgb(36, 41, 45);
            btnRefrescar.Location = new Point(572, 12);
            btnRefrescar.Margin = new Padding(6, 6, 0, 6);
            btnRefrescar.Name = "btnRefrescar";
            btnRefrescar.Size = new Size(88, 32);
            btnRefrescar.TabIndex = 0;
            btnRefrescar.Text = "Refrescar";
            btnRefrescar.UseVisualStyleBackColor = true;
            // 
            // btnNuevo
            // 
            btnNuevo.BackColor = Color.FromArgb(0, 123, 255);
            btnNuevo.FlatAppearance.BorderSize = 0;
            btnNuevo.FlatStyle = FlatStyle.Flat;
            btnNuevo.ForeColor = Color.White;
            btnNuevo.Location = new Point(476, 12);
            btnNuevo.Margin = new Padding(6, 6, 0, 6);
            btnNuevo.Name = "btnNuevo";
            btnNuevo.Size = new Size(75, 32);
            btnNuevo.TabIndex = 1;
            btnNuevo.Text = "Nuevo";
            btnNuevo.UseVisualStyleBackColor = true;
            // 
            // btnEditar
            // 
            btnEditar.BackColor = Color.FromArgb(40, 167, 69);
            btnEditar.FlatAppearance.BorderSize = 0;
            btnEditar.FlatStyle = FlatStyle.Flat;
            btnEditar.ForeColor = Color.White;
            btnEditar.Location = new Point(380, 12);
            btnEditar.Margin = new Padding(6, 6, 0, 6);
            btnEditar.Name = "btnEditar";
            btnEditar.Size = new Size(75, 32);
            btnEditar.TabIndex = 2;
            btnEditar.Text = "Editar";
            btnEditar.UseVisualStyleBackColor = true;
            // 
            // btnEliminar
            // 
            btnEliminar.BackColor = Color.FromArgb(220, 53, 69);
            btnEliminar.FlatAppearance.BorderSize = 0;
            btnEliminar.FlatStyle = FlatStyle.Flat;
            btnEliminar.ForeColor = Color.White;
            btnEliminar.Location = new Point(284, 12);
            btnEliminar.Margin = new Padding(6, 6, 0, 6);
            btnEliminar.Name = "btnEliminar";
            btnEliminar.Size = new Size(80, 32);
            btnEliminar.TabIndex = 3;
            btnEliminar.Text = "Eliminar";
            btnEliminar.UseVisualStyleBackColor = true;
            // 
            // btnDetalle
            // 
            btnDetalle.BackColor = Color.FromArgb(108, 117, 125);
            btnDetalle.FlatAppearance.BorderSize = 0;
            btnDetalle.FlatStyle = FlatStyle.Flat;
            btnDetalle.ForeColor = Color.White;
            btnDetalle.Location = new Point(188, 12);
            btnDetalle.Margin = new Padding(6, 6, 0, 6);
            btnDetalle.Name = "btnDetalle";
            btnDetalle.Size = new Size(75, 32);
            btnDetalle.TabIndex = 4;
            btnDetalle.Text = "Detalle";
            btnDetalle.UseVisualStyleBackColor = true;
            // 
            // btnEditarForm
            // 
            btnEditarForm.BackColor = Color.FromArgb(111, 66, 193);
            btnEditarForm.FlatAppearance.BorderSize = 0;
            btnEditarForm.FlatStyle = FlatStyle.Flat;
            btnEditarForm.ForeColor = Color.White;
            btnEditarForm.Location = new Point(84, 12);
            btnEditarForm.Margin = new Padding(6, 6, 0, 6);
            btnEditarForm.Name = "btnEditarForm";
            btnEditarForm.Size = new Size(90, 32);
            btnEditarForm.TabIndex = 5;
            btnEditarForm.Text = "Edit Form";
            btnEditarForm.UseVisualStyleBackColor = true;
            // 
            // searchContainer
            // 
            searchContainer.BackColor = Color.FromArgb(245, 247, 250);
            searchContainer.Controls.Add(picSearch);
            searchContainer.Controls.Add(txtBuscar);
            searchContainer.Controls.Add(btnLimpiarBusqueda);
            searchContainer.Location = new Point(200, 30);
            searchContainer.Name = "searchContainer";
            searchContainer.Padding = new Padding(12, 8, 8, 8);
            searchContainer.Size = new Size(380, 36);
            searchContainer.TabIndex = 1;
            // 
            // picSearch
            // 
            picSearch.Location = new Point(12, 8);
            picSearch.Name = "picSearch";
            picSearch.Size = new Size(20, 20);
            picSearch.SizeMode = PictureBoxSizeMode.CenterImage;
            picSearch.TabIndex = 0;
            picSearch.TabStop = false;
            // 
            // txtBuscar
            // 
            txtBuscar.BackColor = Color.FromArgb(245, 247, 250);
            txtBuscar.BorderStyle = BorderStyle.None;
            txtBuscar.Font = new Font("Segoe UI", 9F);
            txtBuscar.Location = new Point(44, 10);
            txtBuscar.Name = "txtBuscar";
            txtBuscar.PlaceholderText = "Buscar por usuario o correo (Enter para buscar)";
            txtBuscar.Size = new Size(340, 16);
            txtBuscar.TabIndex = 1;
            // 
            // btnLimpiarBusqueda
            // 
            btnLimpiarBusqueda.BackColor = Color.Transparent;
            btnLimpiarBusqueda.Cursor = Cursors.Hand;
            btnLimpiarBusqueda.FlatAppearance.BorderSize = 0;
            btnLimpiarBusqueda.FlatStyle = FlatStyle.Flat;
            btnLimpiarBusqueda.Location = new Point(412, 7);
            btnLimpiarBusqueda.Name = "btnLimpiarBusqueda";
            btnLimpiarBusqueda.Size = new Size(22, 22);
            btnLimpiarBusqueda.TabIndex = 2;
            btnLimpiarBusqueda.Text = "✕";
            toolTip.SetToolTip(btnLimpiarBusqueda, "Limpiar búsqueda");
            btnLimpiarBusqueda.UseVisualStyleBackColor = true;
            // 
            // lblTitulo
            // 
            lblTitulo.AutoSize = true;
            lblTitulo.Font = new Font("Segoe UI Semibold", 18F, FontStyle.Bold);
            lblTitulo.ForeColor = Color.FromArgb(34, 40, 49);
            lblTitulo.Location = new Point(20, 26);
            lblTitulo.Name = "lblTitulo";
            lblTitulo.Size = new Size(107, 32);
            lblTitulo.TabIndex = 2;
            lblTitulo.Text = "Usuarios";
            // 
            // mainPadding
            // 
            mainPadding.BackColor = Color.FromArgb(248, 249, 250);
            mainPadding.Controls.Add(cardPanel);
            mainPadding.Dock = DockStyle.Fill;
            mainPadding.Location = new Point(0, 96);
            mainPadding.Name = "mainPadding";
            mainPadding.Padding = new Padding(20);
            mainPadding.Size = new Size(1000, 522);
            mainPadding.TabIndex = 1;
            // 
            // cardPanel
            // 
            cardPanel.BackColor = Color.White;
            cardPanel.BorderStyle = BorderStyle.FixedSingle;
            cardPanel.Controls.Add(dgvUsuarios);
            cardPanel.Controls.Add(emptyPanel);
            cardPanel.Dock = DockStyle.Fill;
            cardPanel.Location = new Point(20, 20);
            cardPanel.Name = "cardPanel";
            cardPanel.Padding = new Padding(14);
            cardPanel.Size = new Size(960, 482);
            cardPanel.TabIndex = 0;
            // 
            // dgvUsuarios
            // 
            dgvUsuarios.AllowUserToAddRows = false;
            dgvUsuarios.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.BackColor = Color.FromArgb(249, 250, 252);
            dgvUsuarios.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            dgvUsuarios.BackgroundColor = Color.White;
            dgvUsuarios.BorderStyle = BorderStyle.None;
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = Color.FromArgb(38, 50, 56);
            dataGridViewCellStyle2.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            dataGridViewCellStyle2.ForeColor = Color.White;
            dataGridViewCellStyle2.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = DataGridViewTriState.True;
            dgvUsuarios.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            dgvUsuarios.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvUsuarios.ContextMenuStrip = ctxMenu;
            dgvUsuarios.Dock = DockStyle.Fill;
            dgvUsuarios.EnableHeadersVisualStyles = false;
            dgvUsuarios.Font = new Font("Segoe UI", 9F);
            dgvUsuarios.GridColor = Color.FromArgb(235, 236, 240);
            dgvUsuarios.Location = new Point(14, 14);
            dgvUsuarios.MultiSelect = false;
            dgvUsuarios.Name = "dgvUsuarios";
            dgvUsuarios.ReadOnly = true;
            dgvUsuarios.RowHeadersVisible = false;
            dgvUsuarios.RowTemplate.Height = 28;
            dgvUsuarios.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvUsuarios.Size = new Size(930, 452);
            dgvUsuarios.TabIndex = 0;
            // 
            // ctxMenu
            // 
            ctxMenu.ImageScalingSize = new Size(18, 18);
            ctxMenu.Items.AddRange(new ToolStripItem[] { ctxMenuNuevo, ctxMenuEditar, ctxMenuEliminar });
            ctxMenu.Name = "ctxMenu";
            ctxMenu.Size = new Size(118, 70);
            // 
            // ctxMenuNuevo
            // 
            ctxMenuNuevo.Name = "ctxMenuNuevo";
            ctxMenuNuevo.Size = new Size(117, 22);
            ctxMenuNuevo.Text = "Nuevo";
            // 
            // ctxMenuEditar
            // 
            ctxMenuEditar.Name = "ctxMenuEditar";
            ctxMenuEditar.Size = new Size(117, 22);
            ctxMenuEditar.Text = "Editar";
            // 
            // ctxMenuEliminar
            // 
            ctxMenuEliminar.Name = "ctxMenuEliminar";
            ctxMenuEliminar.Size = new Size(117, 22);
            ctxMenuEliminar.Text = "Eliminar";
            // 
            // emptyPanel
            // 
            emptyPanel.BackColor = Color.Transparent;
            emptyPanel.Controls.Add(lblEmptyTitle);
            emptyPanel.Controls.Add(lblEmptyDesc);
            emptyPanel.Controls.Add(btnEmptyNew);
            emptyPanel.Dock = DockStyle.Fill;
            emptyPanel.Location = new Point(14, 14);
            emptyPanel.Name = "emptyPanel";
            emptyPanel.Size = new Size(930, 452);
            emptyPanel.TabIndex = 1;
            emptyPanel.Visible = false;
            // 
            // lblEmptyTitle
            // 
            lblEmptyTitle.AutoSize = true;
            lblEmptyTitle.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            lblEmptyTitle.ForeColor = Color.FromArgb(54, 62, 71);
            lblEmptyTitle.Location = new Point(40, 80);
            lblEmptyTitle.Name = "lblEmptyTitle";
            lblEmptyTitle.Size = new Size(155, 25);
            lblEmptyTitle.TabIndex = 0;
            lblEmptyTitle.Text = "No hay usuarios";
            // 
            // lblEmptyDesc
            // 
            lblEmptyDesc.AutoSize = true;
            lblEmptyDesc.Font = new Font("Segoe UI", 10F);
            lblEmptyDesc.ForeColor = Color.Gray;
            lblEmptyDesc.Location = new Point(40, 120);
            lblEmptyDesc.MaximumSize = new Size(520, 0);
            lblEmptyDesc.Name = "lblEmptyDesc";
            lblEmptyDesc.Size = new Size(519, 38);
            lblEmptyDesc.TabIndex = 1;
            lblEmptyDesc.Text = "Aún no hay registros. Usa el botón \"Nuevo\" para crear tu primer usuario o importa usuarios desde un archivo.";
            // 
            // btnEmptyNew
            // 
            btnEmptyNew.BackColor = Color.FromArgb(0, 123, 255);
            btnEmptyNew.FlatStyle = FlatStyle.Flat;
            btnEmptyNew.ForeColor = Color.White;
            btnEmptyNew.Location = new Point(40, 170);
            btnEmptyNew.Name = "btnEmptyNew";
            btnEmptyNew.Size = new Size(140, 36);
            btnEmptyNew.TabIndex = 2;
            btnEmptyNew.Text = "Nuevo usuario";
            btnEmptyNew.UseVisualStyleBackColor = true;
            // 
            // statusStrip
            // 
            statusStrip.BackColor = Color.WhiteSmoke;
            statusStrip.Items.AddRange(new ToolStripItem[] { lblInfo, toolProgress });
            statusStrip.Location = new Point(0, 618);
            statusStrip.Name = "statusStrip";
            statusStrip.Size = new Size(1000, 22);
            statusStrip.TabIndex = 3;
            // 
            // lblInfo
            // 
            lblInfo.Name = "lblInfo";
            lblInfo.Size = new Size(32, 17);
            lblInfo.Text = "Listo";
            // 
            // toolProgress
            // 
            toolProgress.Name = "toolProgress";
            toolProgress.Size = new Size(120, 16);
            toolProgress.Style = ProgressBarStyle.Marquee;
            toolProgress.Visible = false;
            // 
            // toolTip
            // 
            toolTip.AutoPopDelay = 5000;
            toolTip.InitialDelay = 600;
            toolTip.ReshowDelay = 120;
            // 
            // FormUsuarios
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1000, 640);
            Controls.Add(mainPadding);
            Controls.Add(headerPanel);
            Controls.Add(statusStrip);
            MinimumSize = new Size(960, 520);
            Name = "FormUsuarios";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Usuarios";
            headerPanel.ResumeLayout(false);
            headerPanel.PerformLayout();
            actionFlow.ResumeLayout(false);
            searchContainer.ResumeLayout(false);
            searchContainer.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)picSearch).EndInit();
            mainPadding.ResumeLayout(false);
            cardPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvUsuarios).EndInit();
            ctxMenu.ResumeLayout(false);
            emptyPanel.ResumeLayout(false);
            emptyPanel.PerformLayout();
            statusStrip.ResumeLayout(false);
            statusStrip.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
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
        private System.Windows.Forms.Button btnDetalle;
        private System.Windows.Forms.Button btnEditarForm;

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