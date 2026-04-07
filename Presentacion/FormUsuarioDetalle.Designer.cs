namespace Andloe.Presentacion
{
    partial class FormUsuarioDetalle
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();

            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            lblUsuario = new Label();
            txtUsuario = new TextBox();
            lblEmail = new Label();
            txtEmail = new TextBox();
            lblEstado = new Label();
            cmbEstado = new ComboBox();
            lblRoles = new Label();
            clbRoles = new CheckedListBox();
            chkCambiarPassword = new CheckBox();
            lblPassword = new Label();
            txtPassword = new TextBox();
            lblConfirmar = new Label();
            txtConfirmar = new TextBox();
            btnGuardar = new Button();
            btnCancelar = new Button();
            lblEmpresa = new Label();
            cmbEmpresa = new ComboBox();
            SuspendLayout();
            // 
            // lblUsuario
            // 
            lblUsuario.AutoSize = true;
            lblUsuario.Location = new Point(24, 20);
            lblUsuario.Name = "lblUsuario";
            lblUsuario.Size = new Size(50, 15);
            lblUsuario.TabIndex = 0;
            lblUsuario.Text = "Usuario";
            // 
            // txtUsuario
            // 
            txtUsuario.Location = new Point(24, 38);
            txtUsuario.Name = "txtUsuario";
            txtUsuario.Size = new Size(336, 23);
            txtUsuario.TabIndex = 1;
            // 
            // lblEmail
            // 
            lblEmail.AutoSize = true;
            lblEmail.Location = new Point(24, 74);
            lblEmail.Name = "lblEmail";
            lblEmail.Size = new Size(36, 15);
            lblEmail.TabIndex = 2;
            lblEmail.Text = "Email";
            // 
            // txtEmail
            // 

            // lblEmpresa
            lblEmpresa.AutoSize = true;
            lblEmpresa.Location = new Point(24, 128);
            lblEmpresa.Name = "lblEmpresa";
            lblEmpresa.Size = new Size(52, 15);
            lblEmpresa.TabIndex = 4;
            lblEmpresa.Text = "Empresa";

            // cmbEmpresa
            cmbEmpresa.FormattingEnabled = true;
            cmbEmpresa.Location = new Point(24, 146);
            cmbEmpresa.Name = "cmbEmpresa";
            cmbEmpresa.Size = new Size(336, 23);
            cmbEmpresa.TabIndex = 5;

            // txtEmail

            txtEmail.Location = new Point(24, 92);
            txtEmail.Name = "txtEmail";
            txtEmail.Size = new Size(336, 23);
            txtEmail.TabIndex = 3;
            // 
            // lblEstado
            // 
            lblEstado.AutoSize = true;
            lblEstado.Location = new Point(24, 128);
            lblEstado.Name = "lblEstado";
            lblEstado.Size = new Size(42, 15);
            lblEstado.TabIndex = 4;
            lblEstado.Text = "Estado";
            // 
            // cmbEstado
            // 
            cmbEstado.FormattingEnabled = true;
            cmbEstado.Location = new Point(24, 146);
            cmbEstado.Name = "cmbEstado";
            cmbEstado.Size = new Size(180, 23);
            cmbEstado.TabIndex = 5;
            // 
            // lblRoles
            // 
            lblRoles.AutoSize = true;
            lblRoles.Location = new Point(24, 182);
            lblRoles.Name = "lblRoles";
            lblRoles.Size = new Size(33, 15);
            lblRoles.TabIndex = 6;
            lblRoles.Text = "Roles";
            // 
            // clbRoles
            // 
            clbRoles.CheckOnClick = true;
            clbRoles.FormattingEnabled = true;
            clbRoles.Location = new Point(24, 200);
            clbRoles.Name = "clbRoles";
            clbRoles.Size = new Size(336, 94);
            clbRoles.TabIndex = 7;
            // 
            // chkCambiarPassword
            // 
            chkCambiarPassword.AutoSize = true;
            chkCambiarPassword.Location = new Point(24, 309);
            chkCambiarPassword.Name = "chkCambiarPassword";
            chkCambiarPassword.Size = new Size(127, 19);
            chkCambiarPassword.TabIndex = 8;
            chkCambiarPassword.Text = "Cambiar contraseña";
            chkCambiarPassword.UseVisualStyleBackColor = true;
            chkCambiarPassword.CheckedChanged += chkCambiarPassword_CheckedChanged;
            // 
            // lblPassword
            // 
            lblPassword.AutoSize = true;
            lblPassword.Location = new Point(24, 340);
            lblPassword.Name = "lblPassword";
            lblPassword.Size = new Size(67, 15);
            lblPassword.TabIndex = 9;
            lblPassword.Text = "Contraseña";
            // 
            // txtPassword
            // 
            txtPassword.Location = new Point(24, 358);
            txtPassword.Name = "txtPassword";
            txtPassword.Size = new Size(336, 23);
            txtPassword.TabIndex = 10;
            txtPassword.UseSystemPasswordChar = true;
            // 
            // lblConfirmar
            // 
            lblConfirmar.AutoSize = true;
            lblConfirmar.Location = new Point(24, 394);
            lblConfirmar.Name = "lblConfirmar";
            lblConfirmar.Size = new Size(118, 15);
            lblConfirmar.TabIndex = 11;
            lblConfirmar.Text = "Confirmar contraseña";
            // 
            // txtConfirmar
            // 
            txtConfirmar.Location = new Point(24, 412);
            txtConfirmar.Name = "txtConfirmar";
            txtConfirmar.Size = new Size(336, 23);
            txtConfirmar.TabIndex = 12;
            txtConfirmar.UseSystemPasswordChar = true;
            // 
            // btnGuardar
            // 
            btnGuardar.Location = new Point(204, 455);
            btnGuardar.Name = "btnGuardar";
            btnGuardar.Size = new Size(75, 30);
            btnGuardar.TabIndex = 13;
            btnGuardar.Text = "Guardar";
            btnGuardar.UseVisualStyleBackColor = true;
            btnGuardar.Click += btnGuardar_Click;
            // 
            // btnCancelar
            // 
            btnCancelar.Location = new Point(285, 455);
            btnCancelar.Name = "btnCancelar";
            btnCancelar.Size = new Size(75, 30);
            btnCancelar.TabIndex = 14;
            btnCancelar.Text = "Cancelar";
            btnCancelar.UseVisualStyleBackColor = true;
            btnCancelar.Click += btnCancelar_Click;
            // 
            // FormUsuarioDetalle
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(392, 505);
            Controls.Add(btnCancelar);
            Controls.Add(btnGuardar);
            Controls.Add(txtConfirmar);
            Controls.Add(lblConfirmar);
            Controls.Add(txtPassword);
            Controls.Add(lblPassword);
            Controls.Add(chkCambiarPassword);
            Controls.Add(clbRoles);
            Controls.Add(lblRoles);
            Controls.Add(cmbEstado);
            Controls.Add(lblEstado);
            Controls.Add(txtEmail);
            Controls.Add(lblEmail);
            Controls.Add(txtUsuario);
            Controls.Add(lblUsuario);
            Controls.Add(cmbEmpresa);
            Controls.Add(lblEmpresa);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FormUsuarioDetalle";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Usuario";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblUsuario;
        private TextBox txtUsuario;
        private Label lblEmail;
        private TextBox txtEmail;
        private Label lblEstado;
        private ComboBox cmbEstado;
        private Label lblRoles;
        private CheckedListBox clbRoles;
        private CheckBox chkCambiarPassword;
        private Label lblPassword;
        private TextBox txtPassword;
        private Label lblConfirmar;
        private TextBox txtConfirmar;
        private Button btnGuardar;
        private Button btnCancelar;
        private Label lblEmpresa;
        private ComboBox cmbEmpresa;

    }
}