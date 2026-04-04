#nullable disable
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Presentacion
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
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges1 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges2 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            lblMsg = new Label();
            chkMostrar = new CheckBox();
            txtPassword = new TextBox();
            txtUsuario = new TextBox();
            cbEmpresa = new ComboBox();
            lblEmpresa = new Label();
            lblTitulo = new Label();
            btnEntrar = new Guna.UI2.WinForms.Guna2Button();
            pictureBox1 = new PictureBox();
            pictureBox3 = new PictureBox();
            pictureBox2 = new PictureBox();
            pictureBox4 = new PictureBox();
            pictureBox5 = new PictureBox();
            pictureBox7 = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox4).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox5).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox7).BeginInit();
            SuspendLayout();
            // 
            // lblMsg
            // 
            lblMsg.AutoSize = true;
            lblMsg.Dock = DockStyle.Top;
            lblMsg.ForeColor = Color.Firebrick;
            lblMsg.Location = new Point(0, 0);
            lblMsg.Name = "lblMsg";
            lblMsg.Padding = new Padding(12, 4, 12, 8);
            lblMsg.Size = new Size(24, 27);
            lblMsg.TabIndex = 0;
            // 
            // chkMostrar
            // 
            chkMostrar.AutoSize = true;
            chkMostrar.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            chkMostrar.ForeColor = Color.White;
            chkMostrar.Location = new Point(572, 405);
            chkMostrar.Name = "chkMostrar";
            chkMostrar.Size = new Size(164, 25);
            chkMostrar.TabIndex = 7;
            chkMostrar.Text = "Mostrar contraseña";
            chkMostrar.CheckedChanged += chkMostrar_CheckedChanged;
            // 
            // txtPassword
            // 
            txtPassword.BackColor = Color.FromArgb(15, 15, 15);
            txtPassword.BorderStyle = BorderStyle.None;
            txtPassword.Font = new Font("Segoe UI Emoji", 15.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            txtPassword.ForeColor = Color.DimGray;
            txtPassword.Location = new Point(625, 358);
            txtPassword.Name = "txtPassword";
            txtPassword.PlaceholderText = "Contraseña";
            txtPassword.Size = new Size(502, 28);
            txtPassword.TabIndex = 6;
            txtPassword.UseSystemPasswordChar = true;
            // 
            // txtUsuario
            // 
            txtUsuario.BackColor = Color.FromArgb(15, 15, 15);
            txtUsuario.BorderStyle = BorderStyle.None;
            txtUsuario.Font = new Font("Segoe UI Emoji", 15.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            txtUsuario.ForeColor = Color.White;
            txtUsuario.Location = new Point(625, 286);
            txtUsuario.Name = "txtUsuario";
            txtUsuario.PlaceholderText = "Usuario";
            txtUsuario.Size = new Size(502, 28);
            txtUsuario.TabIndex = 4;
            // 
            // cbEmpresa
            // 
            cbEmpresa.BackColor = Color.DimGray;
            cbEmpresa.DropDownStyle = ComboBoxStyle.DropDownList;
            cbEmpresa.Font = new Font("Segoe UI Emoji", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            cbEmpresa.FormattingEnabled = true;
            cbEmpresa.Location = new Point(573, 216);
            cbEmpresa.Name = "cbEmpresa";
            cbEmpresa.Size = new Size(568, 34);
            cbEmpresa.TabIndex = 2;
            // 
            // lblEmpresa
            // 
            lblEmpresa.Font = new Font("Verdana", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblEmpresa.ForeColor = Color.White;
            lblEmpresa.Location = new Point(792, 177);
            lblEmpresa.Name = "lblEmpresa";
            lblEmpresa.Size = new Size(118, 36);
            lblEmpresa.TabIndex = 1;
            lblEmpresa.Text = "Empresa:";
            lblEmpresa.TextAlign = ContentAlignment.MiddleRight;
            // 
            // lblTitulo
            // 
            lblTitulo.Font = new Font("Segoe UI", 20.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblTitulo.ForeColor = Color.White;
            lblTitulo.Location = new Point(748, 114);
            lblTitulo.Name = "lblTitulo";
            lblTitulo.Size = new Size(229, 47);
            lblTitulo.TabIndex = 0;
            lblTitulo.Text = "Inicio de Sesión";
            lblTitulo.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // btnEntrar
            // 
            btnEntrar.BackColor = Color.FromArgb(40, 40, 40);
            btnEntrar.BorderColor = Color.Aquamarine;
            btnEntrar.BorderRadius = 10;
            btnEntrar.CustomizableEdges = customizableEdges1;
            btnEntrar.DisabledState.BorderColor = Color.DarkGray;
            btnEntrar.DisabledState.CustomBorderColor = Color.DarkGray;
            btnEntrar.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnEntrar.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnEntrar.FillColor = SystemColors.HotTrack;
            btnEntrar.Font = new Font("Segoe UI Emoji", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            btnEntrar.ForeColor = Color.White;
            btnEntrar.Location = new Point(748, 456);
            btnEntrar.Name = "btnEntrar";
            btnEntrar.PressedColor = Color.DimGray;
            btnEntrar.ShadowDecoration.CustomizableEdges = customizableEdges2;
            btnEntrar.Size = new Size(256, 45);
            btnEntrar.TabIndex = 8;
            btnEntrar.Text = "Entrar";
            // 
            // pictureBox1
            // 
            pictureBox1.BackColor = Color.Black;
            pictureBox1.Image = Properties.Resources.Logo_Erp;
            pictureBox1.Location = new Point(0, 0);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(567, 551);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.TabIndex = 10;
            pictureBox1.TabStop = false;
            // 
            // pictureBox3
            // 
            pictureBox3.Image = Properties.Resources.sdd;
            pictureBox3.Location = new Point(573, 358);
            pictureBox3.Name = "pictureBox3";
            pictureBox3.Size = new Size(46, 34);
            pictureBox3.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox3.TabIndex = 12;
            pictureBox3.TabStop = false;
            pictureBox3.Click += pictureBox3_Click;
            // 
            // pictureBox2
            // 
            pictureBox2.Image = Properties.Resources.Email;
            pictureBox2.Location = new Point(572, 276);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(46, 50);
            pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox2.TabIndex = 13;
            pictureBox2.TabStop = false;
            // 
            // pictureBox4
            // 
            pictureBox4.Image = Properties.Resources.bs;
            pictureBox4.Location = new Point(792, 12);
            pictureBox4.Name = "pictureBox4";
            pictureBox4.Size = new Size(128, 99);
            pictureBox4.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox4.TabIndex = 17;
            pictureBox4.TabStop = false;
            // 
            // pictureBox5
            // 
            pictureBox5.Image = Properties.Resources.Linea;
            pictureBox5.Location = new Point(525, 332);
            pictureBox5.Name = "pictureBox5";
            pictureBox5.Size = new Size(675, 13);
            pictureBox5.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox5.TabIndex = 18;
            pictureBox5.TabStop = false;
            // 
            // pictureBox7
            // 
            pictureBox7.Image = Properties.Resources.Linea1;
            pictureBox7.Location = new Point(525, 392);
            pictureBox7.Name = "pictureBox7";
            pictureBox7.Size = new Size(675, 13);
            pictureBox7.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox7.TabIndex = 20;
            pictureBox7.TabStop = false;
            // 
            // FormLogin
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Black;
            ClientSize = new Size(1149, 552);
            Controls.Add(pictureBox7);
            Controls.Add(pictureBox5);
            Controls.Add(pictureBox4);
            Controls.Add(pictureBox2);
            Controls.Add(pictureBox3);
            Controls.Add(pictureBox1);
            Controls.Add(btnEntrar);
            Controls.Add(lblTitulo);
            Controls.Add(chkMostrar);
            Controls.Add(lblEmpresa);
            Controls.Add(cbEmpresa);
            Controls.Add(txtUsuario);
            Controls.Add(lblMsg);
            Controls.Add(txtPassword);
            FormBorderStyle = FormBorderStyle.None;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FormLogin";
            StartPosition = FormStartPosition.CenterScreen;
            Text = " ";
            Load += FormLogin_Load_1;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox4).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox5).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox7).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        private Label lblMsg;
        private CheckBox chkMostrar;
        private TextBox txtPassword;
        private TextBox txtUsuario;
        private ComboBox cbEmpresa;
        private Label lblEmpresa;
        private Label lblTitulo;
        private Guna.UI2.WinForms.Guna2Button btnEntrar;
        private PictureBox pictureBox1;
        private PictureBox pictureBox3;
        private PictureBox pictureBox2;
        private PictureBox pictureBox4;
        private PictureBox pictureBox5;
        private PictureBox pictureBox7;
    }
}
#nullable restore
