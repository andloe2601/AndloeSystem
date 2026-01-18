using System.Drawing;
using System.Windows.Forms;

namespace Presentation
{
    partial class FormTerminoPagoLista
    {
        private System.ComponentModel.IContainer components = null;
        private DataGridView dgvTerminos;
        private Button btnNuevo;
        private Button btnEditar;
        private Button btnCerrar;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            dgvTerminos = new DataGridView();
            btnNuevo = new Button();
            btnEditar = new Button();
            btnCerrar = new Button();

            SuspendLayout();

            // dgvTerminos
            dgvTerminos.AllowUserToAddRows = false;
            dgvTerminos.AllowUserToDeleteRows = false;
            dgvTerminos.ReadOnly = true;
            dgvTerminos.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvTerminos.MultiSelect = false;
            dgvTerminos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvTerminos.Location = new Point(12, 12);
            dgvTerminos.Size = new Size(560, 280);
            dgvTerminos.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;

            // btnNuevo
            btnNuevo.Text = "Nuevo";
            btnNuevo.Location = new Point(12, 305);
            btnNuevo.Size = new Size(80, 30);
            btnNuevo.Click += btnNuevo_Click;

            // btnEditar
            btnEditar.Text = "Editar";
            btnEditar.Location = new Point(100, 305);
            btnEditar.Size = new Size(80, 30);
            btnEditar.Click += btnEditar_Click;

            // btnCerrar
            btnCerrar.Text = "Cerrar";
            btnCerrar.Location = new Point(492, 305);
            btnCerrar.Size = new Size(80, 30);
            btnCerrar.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnCerrar.Click += btnCerrar_Click;

            // FormTerminoPagoLista
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(584, 351);
            Controls.Add(dgvTerminos);
            Controls.Add(btnNuevo);
            Controls.Add(btnEditar);
            Controls.Add(btnCerrar);
            Text = "Términos de Pago";
            StartPosition = FormStartPosition.CenterParent;
            Load += FormTerminoPagoLista_Load;

            ResumeLayout(false);
        }
    }
}
