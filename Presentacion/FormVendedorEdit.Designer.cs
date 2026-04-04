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
            components = new System.ComponentModel.Container();

            txtCodigo = new TextBox();
            txtNombre = new TextBox();
            txtEmail = new TextBox();
            txtTel = new TextBox();
            chkActivo = new CheckBox();

            btnGuardar = new Button();
            btnCancelar = new Button();
            btnBorrar = new Button();

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 7,
                Padding = new Padding(12)
            };

            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 28));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            layout.Controls.Add(new Label { Text = "Código", AutoSize = true, Anchor = AnchorStyles.Left }, 0, 1);
            layout.Controls.Add(txtCodigo, 1, 1);
            layout.Controls.Add(new Label { Text = "Nombre", AutoSize = true, Anchor = AnchorStyles.Left }, 0, 2);
            layout.Controls.Add(txtNombre, 1, 2);
            layout.Controls.Add(new Label { Text = "Email", AutoSize = true, Anchor = AnchorStyles.Left }, 0, 3);
            layout.Controls.Add(txtEmail, 1, 3);
            layout.Controls.Add(new Label { Text = "Teléfono", AutoSize = true, Anchor = AnchorStyles.Left }, 0, 4);
            layout.Controls.Add(txtTel, 1, 4);

            chkActivo.Text = "Activo";
            chkActivo.AutoSize = true;
            layout.Controls.Add(chkActivo, 1, 5);

            txtCodigo.Dock = DockStyle.Fill;
            txtNombre.Dock = DockStyle.Fill;
            txtEmail.Dock = DockStyle.Fill;
            txtTel.Dock = DockStyle.Fill;

            var buttons = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 44, FlowDirection = FlowDirection.RightToLeft, Padding = new Padding(8) };
            btnGuardar.Text = "Guardar";
            btnCancelar.Text = "Cancelar";
            btnBorrar.Text = "Borrar";
            buttons.Controls.AddRange(new Control[] { btnGuardar, btnCancelar, btnBorrar });

            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            ClientSize = new System.Drawing.Size(520, 260);
            Controls.Add(layout);
            Controls.Add(buttons);

            Name = "FormVendedorEdit";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Vendedor";
        }
    }
}
