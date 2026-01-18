#nullable disable
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Andloe.Presentacion
{
    partial class FormLogin
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            btnCancelar = new Button();
            btnEntrar = new Button();
            chkMostrar = new CheckBox();
            txtPassword = new TextBox();
            lblPassword = new Label();
            txtUsuario = new TextBox();
            lblUsuario = new Label();
            cbEmpresa = new ComboBox();
            lblEmpresa = new Label();
            lblTitulo = new Label();
            panel = new TableLayoutPanel();
            flowButtons = new FlowLayoutPanel();
            lblMsg = new Label();
            pictureBox1 = new PictureBox();
            panel.SuspendLayout();
            flowButtons.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // btnCancelar
            // 
            btnCancelar.Location = new Point(434, 12);
            btnCancelar.Margin = new Padding(8, 4, 0, 4);
            btnCancelar.Name = "btnCancelar";
            btnCancelar.Size = new Size(100, 32);
            btnCancelar.TabIndex = 1;
            btnCancelar.Text = "Cancelar";
            btnCancelar.UseVisualStyleBackColor = true;
            // 
            // btnEntrar
            // 
            btnEntrar.Location = new Point(542, 12);
            btnEntrar.Margin = new Padding(8, 4, 0, 4);
            btnEntrar.Name = "btnEntrar";
            btnEntrar.Size = new Size(100, 32);
            btnEntrar.TabIndex = 0;
            btnEntrar.Text = "Entrar";
            btnEntrar.UseVisualStyleBackColor = true;
            // 
            // chkMostrar
            // 
            chkMostrar.AutoSize = true;
            chkMostrar.Dock = DockStyle.Left;
            chkMostrar.Location = new Point(135, 171);
            chkMostrar.Name = "chkMostrar";
            chkMostrar.Size = new Size(128, 22);
            chkMostrar.TabIndex = 7;
            chkMostrar.Text = "Mostrar contraseña";
            // 
            // txtPassword
            // 
            txtPassword.Dock = DockStyle.Fill;
            txtPassword.Location = new Point(135, 135);
            txtPassword.Name = "txtPassword";
            txtPassword.Size = new Size(516, 23);
            txtPassword.TabIndex = 6;
            txtPassword.UseSystemPasswordChar = true;
            // 
            // lblPassword
            // 
            lblPassword.Dock = DockStyle.Fill;
            lblPassword.Location = new Point(15, 132);
            lblPassword.Name = "lblPassword";
            lblPassword.Size = new Size(114, 36);
            lblPassword.TabIndex = 5;
            lblPassword.Text = "Contraseña:";
            lblPassword.TextAlign = ContentAlignment.MiddleRight;
            // 
            // txtUsuario
            // 
            txtUsuario.Dock = DockStyle.Fill;
            txtUsuario.Location = new Point(135, 99);
            txtUsuario.Name = "txtUsuario";
            txtUsuario.PlaceholderText = "nombre de usuario";
            txtUsuario.Size = new Size(516, 23);
            txtUsuario.TabIndex = 4;
            // 
            // lblUsuario
            // 
            lblUsuario.Dock = DockStyle.Fill;
            lblUsuario.Location = new Point(15, 96);
            lblUsuario.Name = "lblUsuario";
            lblUsuario.Size = new Size(114, 36);
            lblUsuario.TabIndex = 3;
            lblUsuario.Text = "Usuario:";
            lblUsuario.TextAlign = ContentAlignment.MiddleRight;
            // 
            // cbEmpresa
            // 
            cbEmpresa.Dock = DockStyle.Fill;
            cbEmpresa.DropDownStyle = ComboBoxStyle.DropDownList;
            cbEmpresa.FormattingEnabled = true;
            cbEmpresa.Location = new Point(135, 63);
            cbEmpresa.Name = "cbEmpresa";
            cbEmpresa.Size = new Size(516, 23);
            cbEmpresa.TabIndex = 2;
            // 
            // lblEmpresa
            // 
            lblEmpresa.Dock = DockStyle.Fill;
            lblEmpresa.Location = new Point(15, 60);
            lblEmpresa.Name = "lblEmpresa";
            lblEmpresa.Size = new Size(114, 36);
            lblEmpresa.TabIndex = 1;
            lblEmpresa.Text = "Empresa:";
            lblEmpresa.TextAlign = ContentAlignment.MiddleRight;
            // 
            // lblTitulo
            // 
            panel.SetColumnSpan(lblTitulo, 2);
            lblTitulo.Dock = DockStyle.Fill;
            lblTitulo.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            lblTitulo.Location = new Point(15, 12);
            lblTitulo.Name = "lblTitulo";
            lblTitulo.Size = new Size(636, 48);
            lblTitulo.TabIndex = 0;
            lblTitulo.Text = "Inicio de Sesión";
            lblTitulo.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // panel
            // 
            panel.ColumnCount = 2;
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120F));
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            panel.Controls.Add(lblTitulo, 0, 0);
            panel.Controls.Add(lblEmpresa, 0, 1);
            panel.Controls.Add(cbEmpresa, 1, 1);
            panel.Controls.Add(lblUsuario, 0, 2);
            panel.Controls.Add(txtUsuario, 1, 2);
            panel.Controls.Add(lblPassword, 0, 3);
            panel.Controls.Add(txtPassword, 1, 3);
            panel.Controls.Add(chkMostrar, 1, 4);
            panel.Dock = DockStyle.Top;
            panel.Location = new Point(0, 0);
            panel.Name = "panel";
            panel.Padding = new Padding(12, 12, 12, 0);
            panel.RowCount = 6;
            panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F));
            panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 36F));
            panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 36F));
            panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 36F));
            panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 28F));
            panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            panel.Size = new Size(666, 224);
            panel.TabIndex = 2;
            // 
            // flowButtons
            // 
            flowButtons.Controls.Add(btnEntrar);
            flowButtons.Controls.Add(btnCancelar);
            flowButtons.Dock = DockStyle.Top;
            flowButtons.FlowDirection = FlowDirection.RightToLeft;
            flowButtons.Location = new Point(0, 224);
            flowButtons.Name = "flowButtons";
            flowButtons.Padding = new Padding(12, 8, 12, 0);
            flowButtons.Size = new Size(666, 44);
            flowButtons.TabIndex = 1;
            // 
            // lblMsg
            // 
            lblMsg.AutoSize = true;
            lblMsg.Dock = DockStyle.Top;
            lblMsg.ForeColor = Color.Firebrick;
            lblMsg.Location = new Point(0, 268);
            lblMsg.Name = "lblMsg";
            lblMsg.Padding = new Padding(12, 4, 12, 8);
            lblMsg.Size = new Size(24, 27);
            lblMsg.TabIndex = 0;
            // 
            // pictureBox1
            // 
            pictureBox1.Location = new Point(0, 0);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(666, 50);
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            // 
            // FormLogin
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(666, 552);
            Controls.Add(lblMsg);
            Controls.Add(flowButtons);
            Controls.Add(panel);
            Controls.Add(pictureBox1);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FormLogin";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "ANDLOE ERP - Login";
            panel.ResumeLayout(false);
            panel.PerformLayout();
            flowButtons.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }
        private Button btnCancelar;
        private Button btnEntrar;
        private CheckBox chkMostrar;
        private TextBox txtPassword;
        private Label lblPassword;
        private TextBox txtUsuario;
        private Label lblUsuario;
        private ComboBox cbEmpresa;
        private Label lblEmpresa;
        private Label lblTitulo;
        private TableLayoutPanel panel;
        private FlowLayoutPanel flowButtons;
        private Label lblMsg;
        private PictureBox pictureBox1;
    }
}
#nullable restore
