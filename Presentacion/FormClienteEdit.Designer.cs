using System.Windows.Forms;

namespace Andloe.Presentacion
{
    partial class FormClienteEdit
    {
        private System.ComponentModel.IContainer components = null;
        private TextBox txtCodigo;
        private TextBox txtNombre;
        private TextBox txtRnc;
        private TextBox txtTel;
        private TextBox txtEmail;
        private TextBox txtDir;
        private ComboBox cboTipo;
        private CheckBox chkActivo;
        private Button btnGuardar;
        private Button btnCancelar;
        private NumericUpDown numCreditoMaximo;
        private TextBox txtDivisa;
        private TextBox txtTermino;
        private TextBox txtVendedor;
        private TextBox txtAlmacen;

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
            txtRnc = new TextBox();
            txtTel = new TextBox();
            txtEmail = new TextBox();
            txtDir = new TextBox();
            cboTipo = new ComboBox();
            chkActivo = new CheckBox();
            btnGuardar = new Button();
            btnCancelar = new Button();
            numCreditoMaximo = new NumericUpDown();
            txtDivisa = new TextBox();
            txtTermino = new TextBox();
            txtVendedor = new TextBox();
            txtAlmacen = new TextBox();

            // === Layout (grid 2 columnas, 16 filas) ===
            var table = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                Padding = new Padding(12),
                ColumnCount = 2,
                AutoSize = true
            };
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 170));
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            for (int i = 0; i < 16; i++)
                table.RowStyles.Add(new RowStyle(SizeType.Absolute, 32));

            // Campos
            var lblCodigo = new Label { Text = "Código:", TextAlign = System.Drawing.ContentAlignment.MiddleRight, Dock = DockStyle.Fill };
            txtCodigo.ReadOnly = true; txtCodigo.Dock = DockStyle.Fill;

            var lblNombre = new Label { Text = "Nombre:", TextAlign = System.Drawing.ContentAlignment.MiddleRight, Dock = DockStyle.Fill };
            txtNombre.Dock = DockStyle.Fill; txtNombre.MaxLength = 120;

            var lblRnc = new Label { Text = "RNC/Cédula:", TextAlign = System.Drawing.ContentAlignment.MiddleRight, Dock = DockStyle.Fill };
            txtRnc.Width = 160;

            var lblTel = new Label { Text = "Teléfono:", TextAlign = System.Drawing.ContentAlignment.MiddleRight, Dock = DockStyle.Fill };
            txtTel.Width = 160;

            var lblEmail = new Label { Text = "Email:", TextAlign = System.Drawing.ContentAlignment.MiddleRight, Dock = DockStyle.Fill };

            var lblDir = new Label { Text = "Dirección:", TextAlign = System.Drawing.ContentAlignment.MiddleRight, Dock = DockStyle.Fill };
            txtDir.Multiline = true; txtDir.Height = 60; txtDir.Dock = DockStyle.Fill;

            var lblTipo = new Label { Text = "Tipo:", TextAlign = System.Drawing.ContentAlignment.MiddleRight, Dock = DockStyle.Fill };
            cboTipo.DropDownStyle = ComboBoxStyle.DropDownList;
            cboTipo.Items.AddRange(new object[] { "Normal", "Mayorista", "VIP" });
            cboTipo.SelectedIndex = 0;

            chkActivo.Text = "Activo";

            var lblCredito = new Label { Text = "Crédito Máximo:", TextAlign = System.Drawing.ContentAlignment.MiddleRight, Dock = DockStyle.Fill };
            numCreditoMaximo.DecimalPlaces = 2; numCreditoMaximo.Maximum = 999999999; numCreditoMaximo.ThousandsSeparator = true; numCreditoMaximo.Width = 160;

            var lblDivisa = new Label { Text = "Cod. Divisas:", TextAlign = System.Drawing.ContentAlignment.MiddleRight, Dock = DockStyle.Fill };
            txtDivisa.MaxLength = 10; txtDivisa.Width = 120;

            var lblTermino = new Label { Text = "Cod. Término Pagos:", TextAlign = System.Drawing.ContentAlignment.MiddleRight, Dock = DockStyle.Fill };
            txtTermino.MaxLength = 20; txtTermino.Width = 160;

            var lblVendedor = new Label { Text = "Cod. Vendedor:", TextAlign = System.Drawing.ContentAlignment.MiddleRight, Dock = DockStyle.Fill };
            txtVendedor.MaxLength = 20; txtVendedor.Width = 160;

            var lblAlmacen = new Label { Text = "Cod. Almacén:", TextAlign = System.Drawing.ContentAlignment.MiddleRight, Dock = DockStyle.Fill };
            txtAlmacen.MaxLength = 20; txtAlmacen.Width = 160;

            int r = 0;
            table.Controls.Add(lblCodigo, 0, r); table.Controls.Add(txtCodigo, 1, r++);
            table.Controls.Add(lblNombre, 0, r); table.Controls.Add(txtNombre, 1, r++);
            table.Controls.Add(lblRnc, 0, r); table.Controls.Add(txtRnc, 1, r++);
            table.Controls.Add(lblTel, 0, r); table.Controls.Add(txtTel, 1, r++);
            table.Controls.Add(lblEmail, 0, r); table.Controls.Add(txtEmail, 1, r++);
            table.Controls.Add(lblDir, 0, r); table.Controls.Add(txtDir, 1, r++);

            table.Controls.Add(lblTipo, 0, r);
            var tipoPanel = new FlowLayoutPanel { FlowDirection = FlowDirection.LeftToRight, Dock = DockStyle.Fill, Height = 32 };
            tipoPanel.Controls.Add(cboTipo);
            tipoPanel.Controls.Add(chkActivo);
            table.Controls.Add(tipoPanel, 1, r++);

            table.Controls.Add(lblCredito, 0, r); table.Controls.Add(numCreditoMaximo, 1, r++);
            table.Controls.Add(lblDivisa, 0, r); table.Controls.Add(txtDivisa, 1, r++);
            table.Controls.Add(lblTermino, 0, r); table.Controls.Add(txtTermino, 1, r++);
            table.Controls.Add(lblVendedor, 0, r); table.Controls.Add(txtVendedor, 1, r++);
            table.Controls.Add(lblAlmacen, 0, r); table.Controls.Add(txtAlmacen, 1, r++);

            // Botonera
            var flow = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Padding = new Padding(12),
                FlowDirection = FlowDirection.RightToLeft,
                Height = 46
            };
            btnGuardar.Text = "Guardar"; btnGuardar.Width = 100;
            btnCancelar.Text = "Cancelar"; btnCancelar.Width = 100;
            flow.Controls.Add(btnGuardar); flow.Controls.Add(btnCancelar);

            // Form
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            ClientSize = new System.Drawing.Size(760, 600);
            Controls.Add(flow);
            Controls.Add(table);
            Text = "Cliente";
            Name = "FormClienteEdit";
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false; MinimizeBox = false;
        }
    }
}
