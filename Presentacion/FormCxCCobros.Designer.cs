using System.Drawing;
using System.Windows.Forms;

namespace Andloe.Presentacion
{
    partial class FormCxCCobros
    {
        private System.ComponentModel.IContainer components = null;
        private Label lblTitulo;
        private Label lblSubtitulo;
        private TextBox txtBuscar;
        private Button btnBuscar;
        private Button btnNuevo;
        private DataGridView grid;
        private Label lblTotal;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            lblTitulo = new Label();
            lblSubtitulo = new Label();
            txtBuscar = new TextBox();
            btnBuscar = new Button();
            btnNuevo = new Button();
            grid = new DataGridView();
            lblTotal = new Label();

            SuspendLayout();

            lblTitulo.AutoSize = true;
            lblTitulo.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            lblTitulo.Location = new Point(24, 18);
            lblTitulo.Text = "Pagos recibidos";

            lblSubtitulo.AutoSize = true;
            lblSubtitulo.Location = new Point(26, 52);
            lblSubtitulo.Text = "Registra y organiza todos los pagos que recibes en tu empresa.";

            txtBuscar.Location = new Point(28, 92);
            txtBuscar.Size = new Size(420, 23);

            btnBuscar.Location = new Point(456, 90);
            btnBuscar.Size = new Size(90, 27);
            btnBuscar.Text = "Filtrar";

            btnNuevo.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnNuevo.Location = new Point(930, 28);
            btnNuevo.Size = new Size(180, 40);
            btnNuevo.Text = "+ Nuevo pago recibido";
            btnNuevo.BackColor = Color.FromArgb(33, 179, 164);
            btnNuevo.ForeColor = Color.White;
            btnNuevo.FlatStyle = FlatStyle.Flat;
            btnNuevo.FlatAppearance.BorderSize = 0;

            grid.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            grid.Location = new Point(28, 132);
            grid.Size = new Size(1082, 500);
            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.ReadOnly = true;
            grid.MultiSelect = false;
            grid.RowHeadersVisible = false;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.BackgroundColor = Color.White;
            grid.BorderStyle = BorderStyle.FixedSingle;
            grid.Columns.Add(new DataGridViewCheckBoxColumn { Name = "colSel", HeaderText = "", FillWeight = 6 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "colNumero", HeaderText = "Número", FillWeight = 12 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "colCliente", HeaderText = "Cliente", FillWeight = 18 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "colDetalles", HeaderText = "Detalles", FillWeight = 22 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "colFecha", HeaderText = "Creación", FillWeight = 12 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "colCuenta", HeaderText = "Cuenta bancaria", FillWeight = 16 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "colEstado", HeaderText = "Estado", FillWeight = 14 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "colMonto", HeaderText = "Monto", FillWeight = 12 });

            lblTotal.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            lblTotal.Location = new Point(28, 640);
            lblTotal.Size = new Size(250, 24);
            lblTotal.Text = "Total registros: 0";

            AutoScaleDimensions = new SizeF(7F, 15F);
            ClientSize = new Size(1140, 680);
            Controls.Add(lblTitulo);
            Controls.Add(lblSubtitulo);
            Controls.Add(txtBuscar);
            Controls.Add(btnBuscar);
            Controls.Add(btnNuevo);
            Controls.Add(grid);
            Controls.Add(lblTotal);
            Name = "FormCxCCobros";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Pagos recibidos";
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
