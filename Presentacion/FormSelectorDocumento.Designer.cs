using System.Windows.Forms;

namespace Andloe.Presentacion
{
    partial class FormSelectorDocumento
    {
        private System.ComponentModel.IContainer components = null;

        private Label lblTitulo;
        private Button btnCot;
        private Button btnProforma;
        private Button btnFactura;
        private Button btnCancelar;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();

            lblTitulo = new Label();
            btnCot = new Button();
            btnProforma = new Button();
            btnFactura = new Button();
            btnCancelar = new Button();

            // Form
            Text = "Nuevo documento";
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            ClientSize = new System.Drawing.Size(440, 190);
            Name = "FormSelectorDocumento";

            // titulo
            lblTitulo.AutoSize = true;
            lblTitulo.Text = "Seleccione el tipo de documento:";
            lblTitulo.Left = 18;
            lblTitulo.Top = 15;
            lblTitulo.Name = "lblTitulo";

            // botones
            btnCot.Text = "Cotización";
            btnCot.Width = 130;
            btnCot.Height = 42;
            btnCot.Left = 18;
            btnCot.Top = 55;
            btnCot.Name = "btnCot";
            btnCot.Click += btnCot_Click;

            btnProforma.Text = "Pro-Forma";
            btnProforma.Width = 130;
            btnProforma.Height = 42;
            btnProforma.Left = 155;
            btnProforma.Top = 55;
            btnProforma.Name = "btnProforma";
            btnProforma.Click += btnProforma_Click;

            btnFactura.Text = "Factura";
            btnFactura.Width = 130;
            btnFactura.Height = 42;
            btnFactura.Left = 292;
            btnFactura.Top = 55;
            btnFactura.Name = "btnFactura";
            btnFactura.Click += btnFactura_Click;

            btnCancelar.Text = "Cancelar";
            btnCancelar.Width = 110;
            btnCancelar.Height = 32;
            btnCancelar.Left = 312;
            btnCancelar.Top = 125;
            btnCancelar.Name = "btnCancelar";
            btnCancelar.Click += btnCancelar_Click;

            Controls.Add(lblTitulo);
            Controls.Add(btnCot);
            Controls.Add(btnProforma);
            Controls.Add(btnFactura);
            Controls.Add(btnCancelar);
        }
    }
}
