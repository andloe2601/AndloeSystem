using System.Drawing;
using System.Windows.Forms;

namespace Presentation
{
    partial class FormFondoCaja
    {
        private System.ComponentModel.IContainer components = null;

        private Label lblTitulo;
        private Label lblCaja;
        private TextBox txtCaja;
        private Label lblUsuario;
        private TextBox txtUsuario;
        private Label lblMonto;
        private TextBox txtMonto;
        private Label lblObservacion;
        private TextBox txtObservacion;
        private Label lblInfo;
        private Button btnAceptar;
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

            lblTitulo = new Label();
            lblCaja = new Label();
            txtCaja = new TextBox();
            lblUsuario = new Label();
            txtUsuario = new TextBox();
            lblMonto = new Label();
            txtMonto = new TextBox();
            lblObservacion = new Label();
            txtObservacion = new TextBox();
            lblInfo = new Label();
            btnAceptar = new Button();
            btnCancelar = new Button();

            SuspendLayout();
            // 
            // lblTitulo
            // 
            lblTitulo.AutoSize = true;
            lblTitulo.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point);
            lblTitulo.Location = new Point(20, 15);
            lblTitulo.Name = "lblTitulo";
            lblTitulo.Size = new Size(135, 21);
            lblTitulo.Text = "Fondo de Caja";
            // 
            // lblCaja
            // 
            lblCaja.AutoSize = true;
            lblCaja.Location = new Point(20, 55);
            lblCaja.Name = "lblCaja";
            lblCaja.Size = new Size(33, 15);
            lblCaja.Text = "Caja:";
            // 
            // txtCaja
            // 
            txtCaja.Location = new Point(90, 52);
            txtCaja.Name = "txtCaja";
            txtCaja.ReadOnly = true;
            txtCaja.Size = new Size(120, 23);
            // 
            // lblUsuario
            // 
            lblUsuario.AutoSize = true;
            lblUsuario.Location = new Point(230, 55);
            lblUsuario.Name = "lblUsuario";
            lblUsuario.Size = new Size(53, 15);
            lblUsuario.Text = "Usuario:";
            // 
            // txtUsuario
            // 
            txtUsuario.Location = new Point(290, 52);
            txtUsuario.Name = "txtUsuario";
            txtUsuario.ReadOnly = true;
            txtUsuario.Size = new Size(180, 23);
            // 
            // lblMonto
            // 
            lblMonto.AutoSize = true;
            lblMonto.Location = new Point(20, 95);
            lblMonto.Name = "lblMonto";
            lblMonto.Size = new Size(96, 15);
            lblMonto.Text = "Monto de fondo:";
            // 
            // txtMonto
            // 
            txtMonto.Location = new Point(130, 92);
            txtMonto.Name = "txtMonto";
            txtMonto.Size = new Size(120, 23);
            txtMonto.TextAlign = HorizontalAlignment.Right;
            // 
            // lblObservacion
            // 
            lblObservacion.AutoSize = true;
            lblObservacion.Location = new Point(20, 130);
            lblObservacion.Name = "lblObservacion";
            lblObservacion.Size = new Size(77, 15);
            lblObservacion.Text = "Observación:";
            // 
            // txtObservacion
            // 
            txtObservacion.Location = new Point(20, 148);
            txtObservacion.Multiline = true;
            txtObservacion.Name = "txtObservacion";
            txtObservacion.Size = new Size(450, 60);
            // 
            // lblInfo
            // 
            lblInfo.AutoSize = true;
            lblInfo.ForeColor = Color.DimGray;
            lblInfo.Location = new Point(20, 220);
            lblInfo.Name = "lblInfo";
            lblInfo.Size = new Size(38, 15);
            lblInfo.Text = "Info...";
            // 
            // btnAceptar
            // 
            btnAceptar.BackColor = Color.FromArgb(46, 204, 113);
            btnAceptar.FlatStyle = FlatStyle.Flat;
            btnAceptar.ForeColor = Color.White;
            btnAceptar.Location = new Point(290, 250);
            btnAceptar.Name = "btnAceptar";
            btnAceptar.Size = new Size(85, 30);
            btnAceptar.Text = "Aceptar";
            btnAceptar.UseVisualStyleBackColor = false;
            btnAceptar.Click += btnAceptar_Click;
            // 
            // btnCancelar
            // 
            btnCancelar.BackColor = Color.LightGray;
            btnCancelar.FlatStyle = FlatStyle.Flat;
            btnCancelar.Location = new Point(385, 250);
            btnCancelar.Name = "btnCancelar";
            btnCancelar.Size = new Size(85, 30);
            btnCancelar.Text = "Cancelar";
            btnCancelar.UseVisualStyleBackColor = false;
            btnCancelar.Click += btnCancelar_Click;
            // 
            // FormFondoCaja
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(500, 300);
            Controls.Add(lblTitulo);
            Controls.Add(lblCaja);
            Controls.Add(txtCaja);
            Controls.Add(lblUsuario);
            Controls.Add(txtUsuario);
            Controls.Add(lblMonto);
            Controls.Add(txtMonto);
            Controls.Add(lblObservacion);
            Controls.Add(txtObservacion);
            Controls.Add(lblInfo);
            Controls.Add(btnAceptar);
            Controls.Add(btnCancelar);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FormFondoCaja";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Fondo de Caja";
            Load += FormFondoCaja_Load;
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
