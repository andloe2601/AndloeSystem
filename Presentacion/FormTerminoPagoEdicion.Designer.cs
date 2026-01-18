using System.Drawing;
using System.Windows.Forms;

namespace Presentation
{
    partial class FormTerminoPagoEdicion
    {
        private System.ComponentModel.IContainer components = null;
        private Label lblCodigo;
        private TextBox txtCodigo;
        private Label lblDescripcion;
        private TextBox txtDescripcion;
        private Label lblDiasPlazo;
        private NumericUpDown nudDiasPlazo;
        private CheckBox chkTieneDescuento;
        private Label lblPorcDescuento;
        private NumericUpDown nudPorcDescuento;
        private Label lblDiasDescuento;
        private NumericUpDown nudDiasDescuento;
        private CheckBox chkActivo;
        private Button btnGuardar;
        private Button btnCancelar;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();

            lblCodigo = new Label();
            txtCodigo = new TextBox();
            lblDescripcion = new Label();
            txtDescripcion = new TextBox();
            lblDiasPlazo = new Label();
            nudDiasPlazo = new NumericUpDown();
            chkTieneDescuento = new CheckBox();
            lblPorcDescuento = new Label();
            nudPorcDescuento = new NumericUpDown();
            lblDiasDescuento = new Label();
            nudDiasDescuento = new NumericUpDown();
            chkActivo = new CheckBox();
            btnGuardar = new Button();
            btnCancelar = new Button();

            ((System.ComponentModel.ISupportInitialize)nudDiasPlazo).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudPorcDescuento).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudDiasDescuento).BeginInit();

            SuspendLayout();

            // lblCodigo
            lblCodigo.Text = "Código:";
            lblCodigo.Location = new Point(15, 15);
            lblCodigo.AutoSize = true;

            // txtCodigo
            txtCodigo.Location = new Point(120, 12);
            txtCodigo.Size = new Size(150, 23);
            txtCodigo.MaxLength = 20;

            // lblDescripcion
            lblDescripcion.Text = "Descripción:";
            lblDescripcion.Location = new Point(15, 45);
            lblDescripcion.AutoSize = true;

            // txtDescripcion
            txtDescripcion.Location = new Point(120, 42);
            txtDescripcion.Size = new Size(260, 23);
            txtDescripcion.MaxLength = 100;

            // lblDiasPlazo
            lblDiasPlazo.Text = "Días de plazo:";
            lblDiasPlazo.Location = new Point(15, 75);
            lblDiasPlazo.AutoSize = true;

            // nudDiasPlazo
            nudDiasPlazo.Location = new Point(120, 72);
            nudDiasPlazo.Maximum = 365;
            nudDiasPlazo.Minimum = 0;

            // chkTieneDescuento
            chkTieneDescuento.Text = "Tiene descuento por pronto pago";
            chkTieneDescuento.Location = new Point(15, 105);
            chkTieneDescuento.AutoSize = true;
            chkTieneDescuento.CheckedChanged += chkTieneDescuento_CheckedChanged;

            // lblPorcDescuento
            lblPorcDescuento.Text = "% Descuento:";
            lblPorcDescuento.Location = new Point(35, 135);
            lblPorcDescuento.AutoSize = true;

            // nudPorcDescuento
            nudPorcDescuento.Location = new Point(120, 132);
            nudPorcDescuento.DecimalPlaces = 2;
            nudPorcDescuento.Maximum = 100;
            nudPorcDescuento.Enabled = false;

            // lblDiasDescuento
            lblDiasDescuento.Text = "Días descuento:";
            lblDiasDescuento.Location = new Point(200, 135);
            lblDiasDescuento.AutoSize = true;

            // nudDiasDescuento
            nudDiasDescuento.Location = new Point(305, 132);
            nudDiasDescuento.Maximum = 365;
            nudDiasDescuento.Enabled = false;

            // chkActivo
            chkActivo.Text = "Activo";
            chkActivo.Location = new Point(15, 170);
            chkActivo.AutoSize = true;
            chkActivo.Checked = true;

            // btnGuardar
            btnGuardar.Text = "Guardar";
            btnGuardar.Location = new Point(220, 200);
            btnGuardar.Size = new Size(80, 30);
            btnGuardar.Click += btnGuardar_Click;

            // btnCancelar
            btnCancelar.Text = "Cancelar";
            btnCancelar.Location = new Point(305, 200);
            btnCancelar.Size = new Size(80, 30);
            btnCancelar.Click += btnCancelar_Click;

            // FormTerminoPagoEdicion
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(400, 245);
            Controls.Add(lblCodigo);
            Controls.Add(txtCodigo);
            Controls.Add(lblDescripcion);
            Controls.Add(txtDescripcion);
            Controls.Add(lblDiasPlazo);
            Controls.Add(nudDiasPlazo);
            Controls.Add(chkTieneDescuento);
            Controls.Add(lblPorcDescuento);
            Controls.Add(nudPorcDescuento);
            Controls.Add(lblDiasDescuento);
            Controls.Add(nudDiasDescuento);
            Controls.Add(chkActivo);
            Controls.Add(btnGuardar);
            Controls.Add(btnCancelar);
            Text = "Término de Pago";
            StartPosition = FormStartPosition.CenterParent;
            Load += FormTerminoPagoEdicion_Load;

            ((System.ComponentModel.ISupportInitialize)nudDiasPlazo).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudPorcDescuento).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudDiasDescuento).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
