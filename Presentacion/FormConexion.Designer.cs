namespace Andloe.Presentacion
{
    partial class FormConexion
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
            table = new TableLayoutPanel();
            label1 = new Label();
            txtServidor = new TextBox();
            label2 = new Label();
            txtBase = new TextBox();
            chkIntegrated = new CheckBox();
            label3 = new Label();
            txtUsuario = new TextBox();
            label4 = new Label();
            txtPassword = new TextBox();
            chkEncrypt = new CheckBox();
            chkTrust = new CheckBox();
            flowButtons = new FlowLayoutPanel();
            btnProbar = new Button();
            btnGuardar = new Button();
            btnCancelar = new Button();
            lblEstado = new Label();
            table.SuspendLayout();
            flowButtons.SuspendLayout();
            SuspendLayout();
            // 
            // table
            // 
            table.ColumnCount = 2;
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 140F));
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            table.Controls.Add(label1, 0, 0);
            table.Controls.Add(txtServidor, 1, 0);
            table.Controls.Add(label2, 0, 1);
            table.Controls.Add(txtBase, 1, 1);
            table.Controls.Add(chkIntegrated, 1, 2);
            table.Controls.Add(label3, 0, 3);
            table.Controls.Add(txtUsuario, 1, 3);
            table.Controls.Add(label4, 0, 4);
            table.Controls.Add(txtPassword, 1, 4);
            table.Controls.Add(chkEncrypt, 1, 5);
            table.Controls.Add(chkTrust, 1, 6);
            table.Controls.Add(flowButtons, 1, 7);
            table.Controls.Add(lblEstado, 1, 8);
            table.Dock = DockStyle.Fill;
            table.Location = new Point(16, 16);
            table.Name = "table";
            table.RowCount = 9;
            table.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
            table.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
            table.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
            table.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
            table.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
            table.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
            table.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
            table.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            table.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            table.Size = new Size(508, 296);
            table.TabIndex = 0;
            // 
            // label1
            // 
            label1.Dock = DockStyle.Fill;
            label1.Location = new Point(3, 0);
            label1.Name = "label1";
            label1.Size = new Size(134, 30);
            label1.TabIndex = 0;
            label1.Text = "Servidor:";
            label1.TextAlign = ContentAlignment.MiddleRight;
            // 
            // txtServidor
            // 
            txtServidor.Dock = DockStyle.Fill;
            txtServidor.Location = new Point(143, 3);
            txtServidor.Name = "txtServidor";
            txtServidor.PlaceholderText = ".";
            txtServidor.Size = new Size(362, 23);
            txtServidor.TabIndex = 1;
            // 
            // label2
            // 
            label2.Dock = DockStyle.Fill;
            label2.Location = new Point(3, 30);
            label2.Name = "label2";
            label2.Size = new Size(134, 30);
            label2.TabIndex = 2;
            label2.Text = "Base de datos:";
            label2.TextAlign = ContentAlignment.MiddleRight;
            // 
            // txtBase
            // 
            txtBase.Dock = DockStyle.Fill;
            txtBase.Location = new Point(143, 33);
            txtBase.Name = "txtBase";
            txtBase.PlaceholderText = "AndloeV1.1";
            txtBase.Size = new Size(362, 23);
            txtBase.TabIndex = 3;
            // 
            // chkIntegrated
            // 
            chkIntegrated.AutoSize = true;
            chkIntegrated.Checked = true;
            chkIntegrated.CheckState = CheckState.Checked;
            chkIntegrated.Location = new Point(143, 63);
            chkIntegrated.Name = "chkIntegrated";
            chkIntegrated.Size = new Size(278, 19);
            chkIntegrated.TabIndex = 4;
            chkIntegrated.Text = "Autenticación de Windows (Integrated Security)";
            chkIntegrated.CheckedChanged += chkIntegrated_CheckedChanged;
            // 
            // label3
            // 
            label3.Dock = DockStyle.Fill;
            label3.Location = new Point(3, 90);
            label3.Name = "label3";
            label3.Size = new Size(134, 30);
            label3.TabIndex = 5;
            label3.Text = "Usuario (SQL):";
            label3.TextAlign = ContentAlignment.MiddleRight;
            // 
            // txtUsuario
            // 
            txtUsuario.Dock = DockStyle.Fill;
            txtUsuario.Location = new Point(143, 93);
            txtUsuario.Name = "txtUsuario";
            txtUsuario.Size = new Size(362, 23);
            txtUsuario.TabIndex = 6;
            // 
            // label4
            // 
            label4.Dock = DockStyle.Fill;
            label4.Location = new Point(3, 120);
            label4.Name = "label4";
            label4.Size = new Size(134, 30);
            label4.TabIndex = 7;
            label4.Text = "Contraseña:";
            label4.TextAlign = ContentAlignment.MiddleRight;
            // 
            // txtPassword
            // 
            txtPassword.Dock = DockStyle.Fill;
            txtPassword.Location = new Point(143, 123);
            txtPassword.Name = "txtPassword";
            txtPassword.Size = new Size(362, 23);
            txtPassword.TabIndex = 8;
            txtPassword.UseSystemPasswordChar = true;
            // 
            // chkEncrypt
            // 
            chkEncrypt.AutoSize = true;
            chkEncrypt.Location = new Point(143, 153);
            chkEncrypt.Name = "chkEncrypt";
            chkEncrypt.Size = new Size(198, 19);
            chkEncrypt.TabIndex = 9;
            chkEncrypt.Text = "Encrypt (recomendado en redes)";
            // 
            // chkTrust
            // 
            chkTrust.AutoSize = true;
            chkTrust.Checked = true;
            chkTrust.CheckState = CheckState.Checked;
            chkTrust.Location = new Point(143, 183);
            chkTrust.Name = "chkTrust";
            chkTrust.Size = new Size(138, 19);
            chkTrust.TabIndex = 10;
            chkTrust.Text = "TrustServerCertificate";
            // 
            // flowButtons
            // 
            flowButtons.AutoSize = true;
            flowButtons.Controls.Add(btnProbar);
            flowButtons.Controls.Add(btnGuardar);
            flowButtons.Controls.Add(btnCancelar);
            flowButtons.Dock = DockStyle.Left;
            flowButtons.Location = new Point(143, 213);
            flowButtons.Name = "flowButtons";
            flowButtons.Size = new Size(243, 34);
            flowButtons.TabIndex = 11;
            // 
            // btnProbar
            // 
            btnProbar.Location = new Point(3, 3);
            btnProbar.Name = "btnProbar";
            btnProbar.Size = new Size(75, 23);
            btnProbar.TabIndex = 0;
            btnProbar.Text = "Probar";
            btnProbar.Click += btnProbar_Click;
            // 
            // btnGuardar
            // 
            btnGuardar.Location = new Point(84, 3);
            btnGuardar.Name = "btnGuardar";
            btnGuardar.Size = new Size(75, 23);
            btnGuardar.TabIndex = 1;
            btnGuardar.Text = "Guardar";
            btnGuardar.Click += btnGuardar_Click;
            // 
            // btnCancelar
            // 
            btnCancelar.Location = new Point(165, 3);
            btnCancelar.Name = "btnCancelar";
            btnCancelar.Size = new Size(75, 23);
            btnCancelar.TabIndex = 2;
            btnCancelar.Text = "Cancelar";
            btnCancelar.Click += btnCancelar_Click;
            // 
            // lblEstado
            // 
            lblEstado.AutoSize = true;
            lblEstado.Dock = DockStyle.Fill;
            lblEstado.ForeColor = Color.Firebrick;
            lblEstado.Location = new Point(143, 250);
            lblEstado.Name = "lblEstado";
            lblEstado.Size = new Size(362, 46);
            lblEstado.TabIndex = 12;
            // 
            // FormConexion
            // 
            AcceptButton = btnGuardar;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(540, 328);
            Controls.Add(table);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FormConexion";
            Padding = new Padding(16);
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Configuración de Conexión";
            Load += FormConexion_Load;
            table.ResumeLayout(false);
            table.PerformLayout();
            flowButtons.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel table;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtServidor;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtBase;
        private System.Windows.Forms.CheckBox chkIntegrated;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtUsuario;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.CheckBox chkEncrypt;
        private System.Windows.Forms.CheckBox chkTrust;
        private System.Windows.Forms.FlowLayoutPanel flowButtons;
        private System.Windows.Forms.Button btnProbar;
        private System.Windows.Forms.Button btnGuardar;
        private System.Windows.Forms.Button btnCancelar;
        private System.Windows.Forms.Label lblEstado;
    }
}
