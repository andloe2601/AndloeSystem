using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Guna.UI2.WinForms;
using Guna.UI2.WinForms.Suite;

namespace Presentation
{
    partial class FormLoginPosClave
    {
        private IContainer components = null;

        private Label lblTitulo;
        private Label lblUsuario;
        private Label lblUsuarioValor;
        private Label lblCaja;
        private Label lblClave;

        private Guna2ComboBox cbCaja;
        private Guna2TextBox txtClave;

        // Teclado numérico (TODO Guna)
        private Guna2Button btn1;
        private Guna2Button btn2;
        private Guna2Button btn3;
        private Guna2Button btn4;
        private Guna2Button btn5;
        private Guna2Button btn6;
        private Guna2Button btn7;
        private Guna2Button btn8;
        private Guna2Button btn9;
        private Guna2Button btn0;

        private Guna2Button btnClear;
        private Guna2Button btnBackspace;

        private Guna2Button btnAceptar;
        private Guna2Button btnCancelar;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            CustomizableEdges customizableEdges1 = new CustomizableEdges();
            CustomizableEdges customizableEdges2 = new CustomizableEdges();
            CustomizableEdges customizableEdges3 = new CustomizableEdges();
            CustomizableEdges customizableEdges4 = new CustomizableEdges();
            CustomizableEdges customizableEdges5 = new CustomizableEdges();
            CustomizableEdges customizableEdges6 = new CustomizableEdges();
            CustomizableEdges customizableEdges7 = new CustomizableEdges();
            CustomizableEdges customizableEdges8 = new CustomizableEdges();
            CustomizableEdges customizableEdges9 = new CustomizableEdges();
            CustomizableEdges customizableEdges10 = new CustomizableEdges();
            CustomizableEdges customizableEdges11 = new CustomizableEdges();
            CustomizableEdges customizableEdges12 = new CustomizableEdges();
            CustomizableEdges customizableEdges13 = new CustomizableEdges();
            CustomizableEdges customizableEdges14 = new CustomizableEdges();
            CustomizableEdges customizableEdges15 = new CustomizableEdges();
            CustomizableEdges customizableEdges16 = new CustomizableEdges();
            CustomizableEdges customizableEdges17 = new CustomizableEdges();
            CustomizableEdges customizableEdges18 = new CustomizableEdges();
            CustomizableEdges customizableEdges19 = new CustomizableEdges();
            CustomizableEdges customizableEdges20 = new CustomizableEdges();
            CustomizableEdges customizableEdges21 = new CustomizableEdges();
            CustomizableEdges customizableEdges22 = new CustomizableEdges();
            CustomizableEdges customizableEdges23 = new CustomizableEdges();
            CustomizableEdges customizableEdges24 = new CustomizableEdges();
            CustomizableEdges customizableEdges25 = new CustomizableEdges();
            CustomizableEdges customizableEdges26 = new CustomizableEdges();
            CustomizableEdges customizableEdges27 = new CustomizableEdges();
            CustomizableEdges customizableEdges28 = new CustomizableEdges();
            CustomizableEdges customizableEdges29 = new CustomizableEdges();
            CustomizableEdges customizableEdges30 = new CustomizableEdges();
            CustomizableEdges customizableEdges31 = new CustomizableEdges();
            CustomizableEdges customizableEdges32 = new CustomizableEdges();
            lblTitulo = new Label();
            lblUsuario = new Label();
            lblUsuarioValor = new Label();
            lblCaja = new Label();
            lblClave = new Label();
            cbCaja = new Guna2ComboBox();
            txtClave = new Guna2TextBox();
            btnAceptar = new Guna2Button();
            btnCancelar = new Guna2Button();
            btn1 = new Guna2Button();
            btn2 = new Guna2Button();
            btn3 = new Guna2Button();
            btn4 = new Guna2Button();
            btn5 = new Guna2Button();
            btn6 = new Guna2Button();
            btn7 = new Guna2Button();
            btn8 = new Guna2Button();
            btn9 = new Guna2Button();
            btnClear = new Guna2Button();
            btn0 = new Guna2Button();
            btnBackspace = new Guna2Button();
            SuspendLayout();
            // 
            // lblTitulo
            // 
            lblTitulo.AutoSize = true;
            lblTitulo.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            lblTitulo.ForeColor = Color.White;
            lblTitulo.Location = new Point(24, 24);
            lblTitulo.Name = "lblTitulo";
            lblTitulo.Size = new Size(312, 30);
            lblTitulo.TabIndex = 0;
            lblTitulo.Text = "Seleccione caja e ingrese PIN";
            // 
            // lblUsuario
            // 
            lblUsuario.AutoSize = true;
            lblUsuario.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            lblUsuario.ForeColor = Color.White;
            lblUsuario.Location = new Point(24, 70);
            lblUsuario.Name = "lblUsuario";
            lblUsuario.Size = new Size(86, 25);
            lblUsuario.TabIndex = 1;
            lblUsuario.Text = "Usuario:";
            // 
            // lblUsuarioValor
            // 
            lblUsuarioValor.AutoSize = true;
            lblUsuarioValor.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            lblUsuarioValor.ForeColor = Color.White;
            lblUsuarioValor.Location = new Point(120, 70);
            lblUsuarioValor.Name = "lblUsuarioValor";
            lblUsuarioValor.Size = new Size(0, 25);
            lblUsuarioValor.TabIndex = 2;
            // 
            // lblCaja
            // 
            lblCaja.AutoSize = true;
            lblCaja.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            lblCaja.ForeColor = Color.White;
            lblCaja.Location = new Point(24, 104);
            lblCaja.Name = "lblCaja";
            lblCaja.Size = new Size(54, 25);
            lblCaja.TabIndex = 3;
            lblCaja.Text = "Caja:";
            lblCaja.Click += lblCaja_Click;
            // 
            // lblClave
            // 
            lblClave.AutoSize = true;
            lblClave.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            lblClave.ForeColor = Color.White;
            lblClave.Location = new Point(24, 171);
            lblClave.Name = "lblClave";
            lblClave.Size = new Size(115, 25);
            lblClave.TabIndex = 4;
            lblClave.Text = "Clave / PIN:";
            // 
            // cbCaja
            // 
            cbCaja.BackColor = Color.Transparent;
            cbCaja.BorderRadius = 20;
            cbCaja.CustomizableEdges = customizableEdges1;
            cbCaja.DrawMode = DrawMode.OwnerDrawFixed;
            cbCaja.DropDownStyle = ComboBoxStyle.DropDownList;
            cbCaja.FocusedColor = Color.Empty;
            cbCaja.Font = new Font("Segoe UI", 10F);
            cbCaja.ForeColor = Color.FromArgb(68, 88, 112);
            cbCaja.ItemHeight = 30;
            cbCaja.Location = new Point(24, 132);
            cbCaja.Name = "cbCaja";
            cbCaja.ShadowDecoration.CustomizableEdges = customizableEdges2;
            cbCaja.Size = new Size(397, 36);
            cbCaja.TabIndex = 5;
            // 
            // txtClave
            // 
            txtClave.BorderRadius = 20;
            txtClave.CustomizableEdges = customizableEdges3;
            txtClave.DefaultText = "";
            txtClave.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            txtClave.Location = new Point(24, 199);
            txtClave.MaxLength = 6;
            txtClave.Name = "txtClave";
            txtClave.PasswordChar = '●';
            txtClave.PlaceholderText = "";
            txtClave.SelectedText = "";
            txtClave.ShadowDecoration.CustomizableEdges = customizableEdges4;
            txtClave.Size = new Size(397, 36);
            txtClave.TabIndex = 6;
            txtClave.TextAlign = HorizontalAlignment.Center;
            // 
            // btnAceptar
            // 
            btnAceptar.BorderRadius = 22;
            btnAceptar.CustomizableEdges = customizableEdges5;
            btnAceptar.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            btnAceptar.ForeColor = Color.White;
            btnAceptar.Location = new Point(114, 518);
            btnAceptar.Name = "btnAceptar";
            btnAceptar.ShadowDecoration.CustomizableEdges = customizableEdges6;
            btnAceptar.Size = new Size(115, 40);
            btnAceptar.TabIndex = 7;
            btnAceptar.Text = "Entrar";
            btnAceptar.Click += btnAceptar_Click;
            // 
            // btnCancelar
            // 
            btnCancelar.BorderRadius = 22;
            btnCancelar.CustomizableEdges = customizableEdges7;
            btnCancelar.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            btnCancelar.ForeColor = Color.White;
            btnCancelar.Location = new Point(235, 518);
            btnCancelar.Name = "btnCancelar";
            btnCancelar.ShadowDecoration.CustomizableEdges = customizableEdges8;
            btnCancelar.Size = new Size(109, 40);
            btnCancelar.TabIndex = 8;
            btnCancelar.Text = "Cancelar";
            btnCancelar.Click += btnCancelar_Click;
            // 
            // btn1
            // 
            btn1.BorderRadius = 22;
            btn1.CustomizableEdges = customizableEdges9;
            btn1.Font = new Font("Segoe UI", 9F);
            btn1.ForeColor = Color.White;
            btn1.Location = new Point(114, 258);
            btn1.Name = "btn1";
            btn1.ShadowDecoration.CustomizableEdges = customizableEdges10;
            btn1.Size = new Size(69, 45);
            btn1.TabIndex = 9;
            btn1.Text = "1";
            btn1.Click += btn1_Click;
            // 
            // btn2
            // 
            btn2.BorderRadius = 22;
            btn2.CustomizableEdges = customizableEdges11;
            btn2.Font = new Font("Segoe UI", 9F);
            btn2.ForeColor = Color.White;
            btn2.Location = new Point(197, 258);
            btn2.Name = "btn2";
            btn2.ShadowDecoration.CustomizableEdges = customizableEdges12;
            btn2.Size = new Size(63, 45);
            btn2.TabIndex = 10;
            btn2.Text = "2";
            btn2.Click += btn2_Click;
            // 
            // btn3
            // 
            btn3.BorderRadius = 22;
            btn3.CustomizableEdges = customizableEdges13;
            btn3.Font = new Font("Segoe UI", 9F);
            btn3.ForeColor = Color.White;
            btn3.Location = new Point(270, 258);
            btn3.Name = "btn3";
            btn3.ShadowDecoration.CustomizableEdges = customizableEdges14;
            btn3.Size = new Size(66, 45);
            btn3.TabIndex = 11;
            btn3.Text = "3";
            btn3.Click += btn3_Click;
            // 
            // btn4
            // 
            btn4.BorderRadius = 22;
            btn4.CustomizableEdges = customizableEdges15;
            btn4.Font = new Font("Segoe UI", 9F);
            btn4.ForeColor = Color.White;
            btn4.Location = new Point(114, 326);
            btn4.Name = "btn4";
            btn4.ShadowDecoration.CustomizableEdges = customizableEdges16;
            btn4.Size = new Size(69, 45);
            btn4.TabIndex = 12;
            btn4.Text = "4";
            btn4.Click += btn4_Click;
            // 
            // btn5
            // 
            btn5.BorderRadius = 22;
            btn5.CustomizableEdges = customizableEdges17;
            btn5.Font = new Font("Segoe UI", 9F);
            btn5.ForeColor = Color.White;
            btn5.Location = new Point(197, 326);
            btn5.Name = "btn5";
            btn5.ShadowDecoration.CustomizableEdges = customizableEdges18;
            btn5.Size = new Size(63, 45);
            btn5.TabIndex = 13;
            btn5.Text = "5";
            btn5.Click += btn5_Click;
            // 
            // btn6
            // 
            btn6.BorderRadius = 22;
            btn6.CustomizableEdges = customizableEdges19;
            btn6.Font = new Font("Segoe UI", 9F);
            btn6.ForeColor = Color.White;
            btn6.Location = new Point(270, 326);
            btn6.Name = "btn6";
            btn6.ShadowDecoration.CustomizableEdges = customizableEdges20;
            btn6.Size = new Size(66, 45);
            btn6.TabIndex = 14;
            btn6.Text = "6";
            btn6.Click += btn6_Click;
            // 
            // btn7
            // 
            btn7.BorderRadius = 22;
            btn7.CustomizableEdges = customizableEdges21;
            btn7.Font = new Font("Segoe UI", 9F);
            btn7.ForeColor = Color.White;
            btn7.Location = new Point(114, 392);
            btn7.Name = "btn7";
            btn7.ShadowDecoration.CustomizableEdges = customizableEdges22;
            btn7.Size = new Size(69, 45);
            btn7.TabIndex = 15;
            btn7.Text = "7";
            btn7.Click += btn7_Click;
            // 
            // btn8
            // 
            btn8.BorderRadius = 22;
            btn8.CustomizableEdges = customizableEdges23;
            btn8.Font = new Font("Segoe UI", 9F);
            btn8.ForeColor = Color.White;
            btn8.Location = new Point(197, 392);
            btn8.Name = "btn8";
            btn8.ShadowDecoration.CustomizableEdges = customizableEdges24;
            btn8.Size = new Size(63, 45);
            btn8.TabIndex = 16;
            btn8.Text = "8";
            btn8.Click += btn8_Click;
            // 
            // btn9
            // 
            btn9.BorderRadius = 22;
            btn9.CustomizableEdges = customizableEdges25;
            btn9.Font = new Font("Segoe UI", 9F);
            btn9.ForeColor = Color.White;
            btn9.Location = new Point(270, 392);
            btn9.Name = "btn9";
            btn9.ShadowDecoration.CustomizableEdges = customizableEdges26;
            btn9.Size = new Size(66, 45);
            btn9.TabIndex = 17;
            btn9.Text = "9";
            btn9.Click += btn9_Click;
            // 
            // btnClear
            // 
            btnClear.BorderRadius = 22;
            btnClear.CustomizableEdges = customizableEdges27;
            btnClear.Font = new Font("Segoe UI", 9F);
            btnClear.ForeColor = Color.White;
            btnClear.Location = new Point(114, 457);
            btnClear.Name = "btnClear";
            btnClear.ShadowDecoration.CustomizableEdges = customizableEdges28;
            btnClear.Size = new Size(69, 45);
            btnClear.TabIndex = 18;
            btnClear.Text = "C / Todo";
            btnClear.Click += btnClear_Click;
            // 
            // btn0
            // 
            btn0.BorderRadius = 22;
            btn0.CustomizableEdges = customizableEdges29;
            btn0.Font = new Font("Segoe UI", 9F);
            btn0.ForeColor = Color.White;
            btn0.Location = new Point(197, 457);
            btn0.Name = "btn0";
            btn0.ShadowDecoration.CustomizableEdges = customizableEdges30;
            btn0.Size = new Size(63, 45);
            btn0.TabIndex = 19;
            btn0.Text = "0";
            btn0.Click += btn0_Click;
            // 
            // btnBackspace
            // 
            btnBackspace.BorderRadius = 22;
            btnBackspace.CustomizableEdges = customizableEdges31;
            btnBackspace.Font = new Font("Segoe UI", 9F);
            btnBackspace.ForeColor = Color.White;
            btnBackspace.Location = new Point(270, 457);
            btnBackspace.Name = "btnBackspace";
            btnBackspace.ShadowDecoration.CustomizableEdges = customizableEdges32;
            btnBackspace.Size = new Size(66, 45);
            btnBackspace.TabIndex = 20;
            btnBackspace.Text = "← Borrar";
            btnBackspace.Click += btnBackspace_Click;
            // 
            // FormLoginPosClave
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(30, 30, 60);
            ClientSize = new Size(466, 580);
            Controls.Add(btnBackspace);
            Controls.Add(btn0);
            Controls.Add(btnClear);
            Controls.Add(btn9);
            Controls.Add(btn8);
            Controls.Add(btn7);
            Controls.Add(btn6);
            Controls.Add(btn5);
            Controls.Add(btn4);
            Controls.Add(btn3);
            Controls.Add(btn2);
            Controls.Add(btn1);
            Controls.Add(lblTitulo);
            Controls.Add(lblUsuario);
            Controls.Add(lblUsuarioValor);
            Controls.Add(lblCaja);
            Controls.Add(lblClave);
            Controls.Add(cbCaja);
            Controls.Add(txtClave);
            Controls.Add(btnAceptar);
            Controls.Add(btnCancelar);
            FormBorderStyle = FormBorderStyle.None;
            MaximizeBox = false;
            Name = "FormLoginPosClave";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Acceso POS";
            Load += FormLoginPosClave_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
    }
}
