using System.Windows.Forms;

namespace Andloe.Presentacion
{
    partial class FormVendedorEdit
    {
        private System.ComponentModel.IContainer components = null;

        private TextBox txtCodigo;
        private TextBox txtNombre;
        private TextBox txtEmail;
        private TextBox txtTel;
        private CheckBox chkActivo;

        private Button btnGuardar;
        private Button btnCancelar;
        private Button btnBorrar;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            txtCodigo = new TextBox();
            txtNombre = new TextBox();
            txtEmail = new TextBox();
            txtTel = new TextBox();
            chkActivo = new CheckBox();
            btnGuardar = new Button();
            btnCancelar = new Button();
            btnBorrar = new Button();
            layout = new TableLayoutPanel();
            buttons = new FlowLayoutPanel();
            layout.SuspendLayout();
            buttons.SuspendLayout();
            SuspendLayout();
            // 
            // txtCodigo
            // 
            txtCodigo.Dock = DockStyle.Fill;
            txtCodigo.Location = new Point(123, 31);
            txtCodigo.Name = "txtCodigo";
            txtCodigo.Size = new Size(74, 23);
            txtCodigo.TabIndex = 1;
            // 
            // txtNombre
            // 
            txtNombre.Dock = DockStyle.Fill;
            txtNombre.Location = new Point(123, 65);
            txtNombre.Name = "txtNombre";
            txtNombre.Size = new Size(74, 23);
            txtNombre.TabIndex = 3;
            // 
            // txtEmail
            // 
            txtEmail.Dock = DockStyle.Fill;
            txtEmail.Location = new Point(123, 99);
            txtEmail.Name = "txtEmail";
            txtEmail.Size = new Size(74, 23);
            txtEmail.TabIndex = 5;
            // 
            // txtTel
            // 
            txtTel.Dock = DockStyle.Fill;
            txtTel.Location = new Point(123, 133);
            txtTel.Name = "txtTel";
            txtTel.Size = new Size(74, 23);
            txtTel.TabIndex = 7;
            // 
            // chkActivo
            // 
            chkActivo.AutoSize = true;
            chkActivo.Location = new Point(123, 167);
            chkActivo.Name = "chkActivo";
            chkActivo.Size = new Size(60, 19);
            chkActivo.TabIndex = 8;
            chkActivo.Text = "Activo";
            // 
            // btnGuardar
            // 
            btnGuardar.Location = new Point(3, 3);
            btnGuardar.Name = "btnGuardar";
            btnGuardar.Size = new Size(75, 23);
            btnGuardar.TabIndex = 0;
            btnGuardar.Text = "Guardar";
            // 
            // btnCancelar
            // 
            btnCancelar.Location = new Point(84, 3);
            btnCancelar.Name = "btnCancelar";
            btnCancelar.Size = new Size(75, 23);
            btnCancelar.TabIndex = 1;
            btnCancelar.Text = "Cancelar";
            // 
            // btnBorrar
            // 
            btnBorrar.Location = new Point(3, 32);
            btnBorrar.Name = "btnBorrar";
            btnBorrar.Size = new Size(75, 23);
            btnBorrar.TabIndex = 2;
            btnBorrar.Text = "Borrar";
            // 
            // layout
            // 
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120F));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            layout.Controls.Add(txtCodigo, 1, 1);
            layout.Controls.Add(txtNombre, 1, 2);
            layout.Controls.Add(txtEmail, 1, 3);
            layout.Controls.Add(txtTel, 1, 4);
            layout.Controls.Add(chkActivo, 1, 5);
            layout.Location = new Point(0, 0);
            layout.Name = "layout";
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 28F));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 34F));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 34F));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 34F));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 34F));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 34F));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            layout.Size = new Size(200, 100);
            layout.TabIndex = 0;
            // 
            // buttons
            // 
            buttons.Controls.Add(btnGuardar);
            buttons.Controls.Add(btnCancelar);
            buttons.Controls.Add(btnBorrar);
            buttons.Location = new Point(0, 0);
            buttons.Name = "buttons";
            buttons.Size = new Size(200, 100);
            buttons.TabIndex = 1;
            // 
            // FormVendedorEdit
            // 
            ClientSize = new Size(520, 260);
            Controls.Add(layout);
            Controls.Add(buttons);
            Name = "FormVendedorEdit";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Vendedor";
            layout.ResumeLayout(false);
            layout.PerformLayout();
            buttons.ResumeLayout(false);
            ResumeLayout(false);
        }
        private TableLayoutPanel layout;
        private FlowLayoutPanel buttons;
    }
}
