// Andloe.Presentacion/FormUsuarioEdit.Designer.cs
using System.Windows.Forms;
using System.Drawing;

namespace Andloe.Presentacion
{
    partial class FormUsuarioEdit
    {
        private System.ComponentModel.IContainer components = null;

        // Controles que usa el .cs
        private Label lblUsuario;
        private TextBox txtUsuario;
        private Label lblEmail;
        private TextBox txtEmail;
        private Label lblEstado;
        private ComboBox cboEstado;

        private CheckBox chkCambiarClave;
        private Label lblClave;
        private TextBox txtClave;
        private Label lblConfirmar;
        private TextBox txtConfirmar;

        private Label lblRoles;
        private CheckedListBox clbRoles;

        private Label lblMsg;
        private Button btnGuardar;
        private Button btnCancelar;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();

            // --- Form ---
            this.SuspendLayout();
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(720, 520);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Text = "Usuario";

            // Paleta básica
            var pad = 12;
            var lblW = 110;
            var ctrlW = 260;
            var rowH = 28;

            // === Panel principal con 2 columnas (datos a la izquierda, roles a la derecha) ===
            var main = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(pad),
                ColumnCount = 2,
                RowCount = 1,
            };
            main.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 420));
            main.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            this.Controls.Add(main);

            // === Panel izquierda (datos y contraseña) ===
            var left = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                Padding = new Padding(0),
            };
            left.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, lblW));
            left.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, ctrlW));
            main.Controls.Add(left, 0, 0);

            int r = 0;

            // Usuario
            lblUsuario = new Label
            {
                Text = "Usuario:",
                TextAlign = ContentAlignment.MiddleRight,
                Dock = DockStyle.Fill,
                Height = rowH
            };
            txtUsuario = new TextBox
            {
                Dock = DockStyle.Fill,
                MaxLength = 30
            };
            left.RowStyles.Add(new RowStyle(SizeType.Absolute, rowH));
            left.Controls.Add(lblUsuario, 0, r);
            left.Controls.Add(txtUsuario, 1, r++);

            // Email
            lblEmail = new Label
            {
                Text = "Email:",
                TextAlign = ContentAlignment.MiddleRight,
                Dock = DockStyle.Fill,
                Height = rowH
            };
            txtEmail = new TextBox
            {
                Dock = DockStyle.Fill,
                MaxLength = 120
            };
            left.RowStyles.Add(new RowStyle(SizeType.Absolute, rowH));
            left.Controls.Add(lblEmail, 0, r);
            left.Controls.Add(txtEmail, 1, r++);

            // Estado
            lblEstado = new Label
            {
                Text = "Estado:",
                TextAlign = ContentAlignment.MiddleRight,
                Dock = DockStyle.Fill,
                Height = rowH
            };
            cboEstado = new ComboBox
            {
                Dock = DockStyle.Left,
                Width = 140,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cboEstado.Items.AddRange(new object[] { "Activo", "Inactivo" });
            cboEstado.SelectedIndex = 0;

            left.RowStyles.Add(new RowStyle(SizeType.Absolute, rowH));
            left.Controls.Add(lblEstado, 0, r);
            left.Controls.Add(cboEstado, 1, r++);

            // Separador pequeño
            left.RowStyles.Add(new RowStyle(SizeType.Absolute, 8));
            left.Controls.Add(new Panel { Height = 8, Dock = DockStyle.Fill }, 0, r);
            left.SetColumnSpan(left.GetControlFromPosition(0, r), 2);
            r++;

            // Cambiar clave (check)
            chkCambiarClave = new CheckBox
            {
                Text = "Cambiar contraseña",
                Dock = DockStyle.Left,
                AutoSize = true
            };
            left.RowStyles.Add(new RowStyle(SizeType.Absolute, rowH));
            left.Controls.Add(new Label { Dock = DockStyle.Fill }, 0, r);
            left.Controls.Add(chkCambiarClave, 1, r++);

            // Clave
            lblClave = new Label
            {
                Text = "Contraseña:",
                TextAlign = ContentAlignment.MiddleRight,
                Dock = DockStyle.Fill,
                Height = rowH
            };
            txtClave = new TextBox
            {
                Dock = DockStyle.Fill,
                UseSystemPasswordChar = true,
                Enabled = false,
                MaxLength = 128
            };
            left.RowStyles.Add(new RowStyle(SizeType.Absolute, rowH));
            left.Controls.Add(lblClave, 0, r);
            left.Controls.Add(txtClave, 1, r++);

            // Confirmar
            lblConfirmar = new Label
            {
                Text = "Confirmar:",
                TextAlign = ContentAlignment.MiddleRight,
                Dock = DockStyle.Fill,
                Height = rowH
            };
            txtConfirmar = new TextBox
            {
                Dock = DockStyle.Fill,
                UseSystemPasswordChar = true,
                Enabled = false,
                MaxLength = 128
            };
            left.RowStyles.Add(new RowStyle(SizeType.Absolute, rowH));
            left.Controls.Add(lblConfirmar, 0, r);
            left.Controls.Add(txtConfirmar, 1, r++);

            // Mensaje (errores)
            lblMsg = new Label
            {
                Text = "",
                Dock = DockStyle.Fill,
                ForeColor = Color.Firebrick,
                AutoEllipsis = true
            };
            left.RowStyles.Add(new RowStyle(SizeType.Absolute, 36));
            left.Controls.Add(lblMsg, 0, r);
            left.SetColumnSpan(lblMsg, 2);
            r++;

            // Botonera (Guardar / Cancelar)
            var flow = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                FlowDirection = FlowDirection.RightToLeft,
                Height = 42,
                Padding = new Padding(0, 6, 0, 0)
            };
            btnGuardar = new Button
            {
                Text = "Guardar",
                Width = 110
            };
            btnCancelar = new Button
            {
                Text = "Cancelar",
                Width = 110
            };
            flow.Controls.Add(btnGuardar);
            flow.Controls.Add(btnCancelar);

            left.RowStyles.Add(new RowStyle(SizeType.Absolute, 48));
            left.Controls.Add(flow, 0, r);
            left.SetColumnSpan(flow, 2);
            r++;

            // Estira el resto si el form crece
            left.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            left.Controls.Add(new Panel { Dock = DockStyle.Fill }, 0, r);
            left.SetColumnSpan(left.GetControlFromPosition(0, r), 2);

            // === Panel derecha (roles) ===
            var right = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3
            };
            right.RowStyles.Add(new RowStyle(SizeType.Absolute, rowH));
            right.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            right.RowStyles.Add(new RowStyle(SizeType.Absolute, 0));
            main.Controls.Add(right, 1, 0);

            lblRoles = new Label
            {
                Text = "Roles:",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            };
            right.Controls.Add(lblRoles, 0, 0);

            clbRoles = new CheckedListBox
            {
                Dock = DockStyle.Fill,
                CheckOnClick = true
            };
            right.Controls.Add(clbRoles, 0, 1);

            this.ResumeLayout(false);
        }
    }
}
