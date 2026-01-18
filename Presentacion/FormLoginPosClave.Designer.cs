using System.Drawing;
using System.Windows.Forms;
using Andloe.Data;

namespace Presentation
{
    partial class FormLoginPosClave
    {
        private System.ComponentModel.IContainer components = null;

        private Label lblTitulo;
        private Label lblUsuario;
        private Label lblUsuarioValor;

        private Label lblCaja;
        private ComboBox cbCaja;

        private Label lblClave;
        private TextBox txtClave;

        private Button btn0;
        private Button btn1;
        private Button btn2;
        private Button btn3;
        private Button btn4;
        private Button btn5;
        private Button btn6;
        private Button btn7;
        private Button btn8;
        private Button btn9;

        private Button btnClear;       // Borrar todo
        private Button btnBackspace;   // Borrar uno
        private Button btnAceptar;
        private Button btnCancelar;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();

            lblTitulo = new Label();
            lblUsuario = new Label();
            lblUsuarioValor = new Label();

            lblCaja = new Label();
            cbCaja = new ComboBox();

            lblClave = new Label();
            txtClave = new TextBox();

            btn0 = new Button();
            btn1 = new Button();
            btn2 = new Button();
            btn3 = new Button();
            btn4 = new Button();
            btn5 = new Button();
            btn6 = new Button();
            btn7 = new Button();
            btn8 = new Button();
            btn9 = new Button();

            btnClear = new Button();
            btnBackspace = new Button();
            btnAceptar = new Button();
            btnCancelar = new Button();

            SuspendLayout();
            // 
            // FormLoginPosClave
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(245, 246, 250);
            ClientSize = new Size(360, 500);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FormLoginPosClave";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Acceso POS";
            Load += FormLoginPosClave_Load;
            // 
            // lblTitulo
            // 
            lblTitulo.AutoSize = true;
            lblTitulo.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            lblTitulo.Location = new Point(70, 10);
            lblTitulo.Name = "lblTitulo";
            lblTitulo.Size = new Size(220, 20);
            lblTitulo.Text = "Seleccione caja e ingrese PIN";
            // 
            // lblUsuario
            // 
            lblUsuario.AutoSize = true;
            lblUsuario.Font = new Font("Segoe UI", 9F);
            lblUsuario.Location = new Point(25, 40);
            lblUsuario.Name = "lblUsuario";
            lblUsuario.Size = new Size(50, 15);
            lblUsuario.Text = "Usuario:";
            // 
            // lblUsuarioValor
            // 
            lblUsuarioValor.AutoSize = true;
            lblUsuarioValor.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblUsuarioValor.Location = new Point(80, 40);
            lblUsuarioValor.Name = "lblUsuarioValor";
            lblUsuarioValor.Size = new Size(70, 15);
            lblUsuarioValor.Text = "userActual";
            // 
            // lblCaja
            // 
            lblCaja.AutoSize = true;
            lblCaja.Font = new Font("Segoe UI", 9F);
            lblCaja.Location = new Point(25, 65);
            lblCaja.Name = "lblCaja";
            lblCaja.Size = new Size(33, 15);
            lblCaja.Text = "Caja:";
            // 
            // cbCaja
            // 
            cbCaja.DropDownStyle = ComboBoxStyle.DropDownList;
            cbCaja.Font = new Font("Segoe UI", 10F);
            cbCaja.Location = new Point(25, 83);
            cbCaja.Name = "cbCaja";
            cbCaja.Size = new Size(300, 25);
            // 
            // lblClave
            // 
            lblClave.AutoSize = true;
            lblClave.Font = new Font("Segoe UI", 9F);
            lblClave.Location = new Point(25, 120);
            lblClave.Name = "lblClave";
            lblClave.Size = new Size(74, 15);
            lblClave.Text = "Clave / PIN:";
            // 
            // txtClave
            // 
            txtClave.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            txtClave.Location = new Point(25, 138);
            txtClave.Name = "txtClave";
            txtClave.ReadOnly = true;
            txtClave.PasswordChar = '●';
            txtClave.Size = new Size(300, 36);
            txtClave.TextAlign = HorizontalAlignment.Center;

            // Tamaño estándar botones
            Size sizeBtn = new Size(60, 45);
            int startX = 50;
            int startY = 190;
            int sepX = 70;
            int sepY = 55;

            // fila 1: 1 2 3
            btn1.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            btn1.Location = new Point(startX, startY);
            btn1.Name = "btn1";
            btn1.Size = sizeBtn;
            btn1.Text = "1";
            btn1.UseVisualStyleBackColor = true;
            btn1.Click += btn1_Click;

            btn2.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            btn2.Location = new Point(startX + sepX, startY);
            btn2.Name = "btn2";
            btn2.Size = sizeBtn;
            btn2.Text = "2";
            btn2.UseVisualStyleBackColor = true;
            btn2.Click += btn2_Click;

            btn3.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            btn3.Location = new Point(startX + sepX * 2, startY);
            btn3.Name = "btn3";
            btn3.Size = sizeBtn;
            btn3.Text = "3";
            btn3.UseVisualStyleBackColor = true;
            btn3.Click += btn3_Click;

            // fila 2: 4 5 6
            btn4.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            btn4.Location = new Point(startX, startY + sepY);
            btn4.Name = "btn4";
            btn4.Size = sizeBtn;
            btn4.Text = "4";
            btn4.UseVisualStyleBackColor = true;
            btn4.Click += btn4_Click;

            btn5.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            btn5.Location = new Point(startX + sepX, startY + sepY);
            btn5.Name = "btn5";
            btn5.Size = sizeBtn;
            btn5.Text = "5";
            btn5.UseVisualStyleBackColor = true;
            btn5.Click += btn5_Click;

            btn6.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            btn6.Location = new Point(startX + sepX * 2, startY + sepY);
            btn6.Name = "btn6";
            btn6.Size = sizeBtn;
            btn6.Text = "6";
            btn6.UseVisualStyleBackColor = true;
            btn6.Click += btn6_Click;

            // fila 3: 7 8 9
            btn7.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            btn7.Location = new Point(startX, startY + sepY * 2);
            btn7.Name = "btn7";
            btn7.Size = sizeBtn;
            btn7.Text = "7";
            btn7.UseVisualStyleBackColor = true;
            btn7.Click += btn7_Click;

            btn8.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            btn8.Location = new Point(startX + sepX, startY + sepY * 2);
            btn8.Name = "btn8";
            btn8.Size = sizeBtn;
            btn8.Text = "8";
            btn8.UseVisualStyleBackColor = true;
            btn8.Click += btn8_Click;

            btn9.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            btn9.Location = new Point(startX + sepX * 2, startY + sepY * 2);
            btn9.Name = "btn9";
            btn9.Size = sizeBtn;
            btn9.Text = "9";
            btn9.UseVisualStyleBackColor = true;
            btn9.Click += btn9_Click;

            // fila 4: Clear, 0, Backspace
            btnClear.Font = new Font("Segoe UI", 9F);
            btnClear.BackColor = Color.WhiteSmoke;
            btnClear.Location = new Point(startX, startY + sepY * 3);
            btnClear.Name = "btnClear";
            btnClear.Size = sizeBtn;
            btnClear.Text = "C / Todo";
            btnClear.UseVisualStyleBackColor = false;
            btnClear.Click += btnClear_Click;

            btn0.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            btn0.Location = new Point(startX + sepX, startY + sepY * 3);
            btn0.Name = "btn0";
            btn0.Size = sizeBtn;
            btn0.Text = "0";
            btn0.UseVisualStyleBackColor = true;
            btn0.Click += btn0_Click;

            btnBackspace.Font = new Font("Segoe UI", 9F);
            btnBackspace.BackColor = Color.WhiteSmoke;
            btnBackspace.Location = new Point(startX + sepX * 2, startY + sepY * 3);
            btnBackspace.Name = "btnBackspace";
            btnBackspace.Size = sizeBtn;
            btnBackspace.Text = "← Borrar";
            btnBackspace.UseVisualStyleBackColor = false;
            btnBackspace.Click += btnBackspace_Click;

            // 
            // btnAceptar
            // 
            btnAceptar.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnAceptar.BackColor = Color.FromArgb(46, 204, 113);
            btnAceptar.ForeColor = Color.White;
            btnAceptar.Location = new Point(50, 420);
            btnAceptar.Name = "btnAceptar";
            btnAceptar.Size = new Size(110, 40);
            btnAceptar.Text = "Aceptar";
            btnAceptar.UseVisualStyleBackColor = false;
            btnAceptar.Click += btnAceptar_Click;
            // 
            // btnCancelar
            // 
            btnCancelar.Font = new Font("Segoe UI", 10F);
            btnCancelar.BackColor = Color.FromArgb(231, 76, 60);
            btnCancelar.ForeColor = Color.White;
            btnCancelar.Location = new Point(190, 420);
            btnCancelar.Name = "btnCancelar";
            btnCancelar.Size = new Size(110, 40);
            btnCancelar.Text = "Cancelar";
            btnCancelar.UseVisualStyleBackColor = false;
            btnCancelar.Click += btnCancelar_Click;

            // Add controls
            Controls.Add(lblTitulo);
            Controls.Add(lblUsuario);
            Controls.Add(lblUsuarioValor);
            Controls.Add(lblCaja);
            Controls.Add(cbCaja);
            Controls.Add(lblClave);
            Controls.Add(txtClave);

            Controls.Add(btn1);
            Controls.Add(btn2);
            Controls.Add(btn3);
            Controls.Add(btn4);
            Controls.Add(btn5);
            Controls.Add(btn6);
            Controls.Add(btn7);
            Controls.Add(btn8);
            Controls.Add(btn9);
            Controls.Add(btn0);

            Controls.Add(btnClear);
            Controls.Add(btnBackspace);
            Controls.Add(btnAceptar);
            Controls.Add(btnCancelar);

            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
    }
}
