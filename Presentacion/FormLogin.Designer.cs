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
            if (disposing && (components != null))
                components.Dispose();

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges1 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges2 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges3 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges4 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges5 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges6 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormLogin));
            panelDerecho = new Panel();
            guna2PictureBox1 = new Guna.UI2.WinForms.Guna2PictureBox();
            cbEmpresa = new Guna.UI2.WinForms.Guna2ComboBox();
            lblMsg = new Label();
            btnEntrar = new Guna.UI2.WinForms.Guna2Button();
            chkMostrar = new CheckBox();
            txtPassword = new TextBox();
            pictureBox3 = new PictureBox();
            pictureBox2 = new PictureBox();
            txtUsuario = new TextBox();
            lblTitulo = new Label();
            pictureBox1 = new PictureBox();
            btnCerrar = new Label();
            pictureBox5 = new PictureBox();
            pictureBox7 = new PictureBox();
            guna2BorderlessForm1 = new Guna.UI2.WinForms.Guna2BorderlessForm(components);
            panelDerecho.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)guna2PictureBox1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox5).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox7).BeginInit();
            SuspendLayout();
            // 
            // panelDerecho
            // 
            panelDerecho.BackColor = Color.Black;
            panelDerecho.Controls.Add(guna2PictureBox1);
            panelDerecho.Controls.Add(cbEmpresa);
            panelDerecho.Controls.Add(lblMsg);
            panelDerecho.Controls.Add(btnEntrar);
            panelDerecho.Controls.Add(chkMostrar);
            panelDerecho.Controls.Add(txtPassword);
            panelDerecho.Controls.Add(pictureBox3);
            panelDerecho.Controls.Add(pictureBox2);
            panelDerecho.Controls.Add(txtUsuario);
            panelDerecho.Controls.Add(lblTitulo);
            panelDerecho.Controls.Add(pictureBox1);
            panelDerecho.Controls.Add(btnCerrar);
            panelDerecho.Controls.Add(pictureBox5);
            panelDerecho.Controls.Add(pictureBox7);
            panelDerecho.Location = new Point(24, 12);
            panelDerecho.Name = "panelDerecho";
            panelDerecho.Size = new Size(500, 547);
            panelDerecho.TabIndex = 1;
            panelDerecho.Paint += panelDerecho_Paint;
            // 
            // guna2PictureBox1
            // 
            guna2PictureBox1.CustomizableEdges = customizableEdges1;
            guna2PictureBox1.ImageRotate = 0F;
            guna2PictureBox1.Location = new Point(20, 720);
            guna2PictureBox1.Name = "guna2PictureBox1";
            guna2PictureBox1.ShadowDecoration.CustomizableEdges = customizableEdges2;
            guna2PictureBox1.Size = new Size(300, 200);
            guna2PictureBox1.TabIndex = 17;
            guna2PictureBox1.TabStop = false;
            // 
            // cbEmpresa
            // 
            cbEmpresa.BackColor = Color.Transparent;
            cbEmpresa.BorderRadius = 10;
            cbEmpresa.CustomizableEdges = customizableEdges3;
            cbEmpresa.DrawMode = DrawMode.OwnerDrawFixed;
            cbEmpresa.DropDownStyle = ComboBoxStyle.DropDownList;
            cbEmpresa.FocusedColor = Color.FromArgb(94, 148, 255);
            cbEmpresa.FocusedState.BorderColor = Color.FromArgb(94, 148, 255);
            cbEmpresa.Font = new Font("Segoe UI", 10F);
            cbEmpresa.ForeColor = Color.FromArgb(68, 88, 112);
            cbEmpresa.ItemHeight = 30;
            cbEmpresa.Location = new Point(70, 176);
            cbEmpresa.Name = "cbEmpresa";
            cbEmpresa.ShadowDecoration.CustomizableEdges = customizableEdges4;
            cbEmpresa.Size = new Size(320, 36);
            cbEmpresa.TabIndex = 16;
            cbEmpresa.SelectedIndexChanged += cbEmpresa_SelectedIndexChanged_2;
            // 
            // lblMsg
            // 
            lblMsg.BackColor = Color.Transparent;
            lblMsg.BorderStyle = BorderStyle.FixedSingle;
            lblMsg.Font = new Font("Segoe UI", 10F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblMsg.ForeColor = Color.FromArgb(255, 140, 140);
            lblMsg.Location = new Point(119, 463);
            lblMsg.Name = "lblMsg";
            lblMsg.Padding = new Padding(6, 0, 6, 0);
            lblMsg.Size = new Size(224, 34);
            lblMsg.TabIndex = 9;
            lblMsg.TextAlign = ContentAlignment.MiddleCenter;
            lblMsg.Visible = false;
            lblMsg.Click += lblMsg_Click;
            // 
            // btnEntrar
            // 
            btnEntrar.BorderRadius = 10;
            btnEntrar.CustomizableEdges = customizableEdges5;
            btnEntrar.DisabledState.BorderColor = Color.DarkGray;
            btnEntrar.DisabledState.CustomBorderColor = Color.DarkGray;
            btnEntrar.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnEntrar.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnEntrar.FillColor = Color.RoyalBlue;
            btnEntrar.Font = new Font("Segoe UI Symbol", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnEntrar.ForeColor = Color.White;
            btnEntrar.Location = new Point(119, 400);
            btnEntrar.Name = "btnEntrar";
            btnEntrar.ShadowDecoration.CustomizableEdges = customizableEdges6;
            btnEntrar.Size = new Size(224, 45);
            btnEntrar.TabIndex = 8;
            btnEntrar.Text = "Entrar";
            btnEntrar.Click += btnEntrar_Click;
            // 
            // chkMostrar
            // 
            chkMostrar.AutoSize = true;
            chkMostrar.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point, 0);
            chkMostrar.ForeColor = Color.White;
            chkMostrar.Location = new Point(70, 361);
            chkMostrar.Name = "chkMostrar";
            chkMostrar.Size = new Size(148, 23);
            chkMostrar.TabIndex = 7;
            chkMostrar.Text = "Mostrar contraseña";
            chkMostrar.UseVisualStyleBackColor = true;
            chkMostrar.CheckedChanged += chkMostrar_CheckedChanged;
            // 
            // txtPassword
            // 
            txtPassword.BackColor = Color.Black;
            txtPassword.BorderStyle = BorderStyle.None;
            txtPassword.Font = new Font("Segoe UI", 14F, FontStyle.Regular, GraphicsUnit.Point, 0);
            txtPassword.ForeColor = Color.White;
            txtPassword.Location = new Point(116, 314);
            txtPassword.Name = "txtPassword";
            txtPassword.PlaceholderText = "Contraseña";
            txtPassword.Size = new Size(274, 25);
            txtPassword.TabIndex = 6;
            txtPassword.UseSystemPasswordChar = true;
            // 
            // pictureBox3
            // 
            pictureBox3.Image = Properties.Resources.Candado;
            pictureBox3.Location = new Point(70, 311);
            pictureBox3.Name = "pictureBox3";
            pictureBox3.Size = new Size(40, 28);
            pictureBox3.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox3.TabIndex = 6;
            pictureBox3.TabStop = false;
            pictureBox3.Click += pictureBox3_Click;
            // 
            // pictureBox2
            // 
            pictureBox2.Image = (Image)resources.GetObject("pictureBox2.Image");
            pictureBox2.Location = new Point(70, 235);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(40, 33);
            pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox2.TabIndex = 3;
            pictureBox2.TabStop = false;
            // 
            // txtUsuario
            // 
            txtUsuario.BackColor = Color.Black;
            txtUsuario.BorderStyle = BorderStyle.None;
            txtUsuario.Font = new Font("Segoe UI", 14F, FontStyle.Regular, GraphicsUnit.Point, 0);
            txtUsuario.ForeColor = Color.White;
            txtUsuario.Location = new Point(116, 243);
            txtUsuario.Name = "txtUsuario";
            txtUsuario.PlaceholderText = "Usuario";
            txtUsuario.Size = new Size(274, 25);
            txtUsuario.TabIndex = 4;
            // 
            // lblTitulo
            // 
            lblTitulo.BackColor = Color.Black;
            lblTitulo.Font = new Font("Sitka Small", 21.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblTitulo.ForeColor = Color.White;
            lblTitulo.Location = new Point(107, 118);
            lblTitulo.Name = "lblTitulo";
            lblTitulo.Size = new Size(253, 45);
            lblTitulo.TabIndex = 0;
            lblTitulo.Text = "Inicio de Sesión";
            lblTitulo.TextAlign = ContentAlignment.MiddleLeft;
            lblTitulo.Click += lblTitulo_Click;
            // 
            // pictureBox1
            // 
            pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.Location = new Point(13, -39);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(446, 251);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.TabIndex = 13;
            pictureBox1.TabStop = false;
            // 
            // btnCerrar
            // 
            btnCerrar.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnCerrar.Cursor = Cursors.Hand;
            btnCerrar.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            btnCerrar.ForeColor = Color.White;
            btnCerrar.Location = new Point(465, 5);
            btnCerrar.Name = "btnCerrar";
            btnCerrar.Size = new Size(30, 30);
            btnCerrar.TabIndex = 12;
            btnCerrar.Text = "X";
            btnCerrar.TextAlign = ContentAlignment.MiddleCenter;
            btnCerrar.Click += btnCerrar_Click;
            // 
            // pictureBox5
            // 
            pictureBox5.Image = Properties.Resources.Linea;
            pictureBox5.Location = new Point(70, 274);
            pictureBox5.Name = "pictureBox5";
            pictureBox5.Size = new Size(320, 10);
            pictureBox5.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox5.TabIndex = 5;
            pictureBox5.TabStop = false;
            // 
            // pictureBox7
            // 
            pictureBox7.Image = Properties.Resources.Linea1;
            pictureBox7.Location = new Point(70, 345);
            pictureBox7.Name = "pictureBox7";
            pictureBox7.Size = new Size(320, 10);
            pictureBox7.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox7.TabIndex = 7;
            pictureBox7.TabStop = false;
            // 
            // guna2BorderlessForm1
            // 
            guna2BorderlessForm1.ContainerControl = this;
            guna2BorderlessForm1.DockIndicatorTransparencyValue = 0.6D;
            guna2BorderlessForm1.TransparentWhileDrag = true;
            // 
            // FormLogin
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Black;
            ClientSize = new Size(539, 560);
            Controls.Add(panelDerecho);
            FormBorderStyle = FormBorderStyle.None;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FormLogin";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Login";
            Load += FormLogin_Load;
            panelDerecho.ResumeLayout(false);
            panelDerecho.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)guna2PictureBox1).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox5).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox7).EndInit();
            ResumeLayout(false);
        }

        private Panel panelDerecho;
        private Label btnCerrar;
        private Label lblTitulo;
        private PictureBox pictureBox2;
        private TextBox txtUsuario;
        private PictureBox pictureBox5;
        private PictureBox pictureBox3;
        private TextBox txtPassword;
        private PictureBox pictureBox7;
        private CheckBox chkMostrar;
        private Guna.UI2.WinForms.Guna2Button btnEntrar;
        private PictureBox pictureBox1;
        private Label lblMsg;
        private Guna.UI2.WinForms.Guna2BorderlessForm guna2BorderlessForm1;
        private Guna.UI2.WinForms.Guna2ComboBox cbEmpresa;
        private Guna.UI2.WinForms.Guna2PictureBox guna2PictureBox1;
    }
}
#nullable restore