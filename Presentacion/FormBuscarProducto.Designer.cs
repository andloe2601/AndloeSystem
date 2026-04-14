using System.Drawing;
using System.Windows.Forms;

namespace Andloe.Presentacion
{
    partial class FormBuscarProducto
    {
        private System.ComponentModel.IContainer components = null;

        private Panel pnlTop;
        private Panel pnlBottom;
        private Panel pnlFill;
        private TextBox txtBuscar;
        private Button btnBuscar;
        private Button btnAceptar;
        private Button btnCancelar;
        private Label lblBuscar;
        private Label lblTotal;
        private DataGridView grid;

        private DataGridViewTextBoxColumn colCodigo;
        private DataGridViewTextBoxColumn colCodBarra;
        private DataGridViewTextBoxColumn colDescripcion;
        private DataGridViewTextBoxColumn colUnidad;
        private DataGridViewTextBoxColumn colPrecio;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            pnlTop = new Panel();
            lblBuscar = new Label();
            txtBuscar = new TextBox();
            btnBuscar = new Button();

            pnlFill = new Panel();
            grid = new DataGridView();
            colCodigo = new DataGridViewTextBoxColumn();
            colCodBarra = new DataGridViewTextBoxColumn();
            colDescripcion = new DataGridViewTextBoxColumn();
            colUnidad = new DataGridViewTextBoxColumn();
            colPrecio = new DataGridViewTextBoxColumn();

            pnlBottom = new Panel();
            lblTotal = new Label();
            btnAceptar = new Button();
            btnCancelar = new Button();

            pnlTop.SuspendLayout();
            pnlFill.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)grid).BeginInit();
            pnlBottom.SuspendLayout();
            SuspendLayout();

            pnlTop.Controls.Add(lblBuscar);
            pnlTop.Controls.Add(txtBuscar);
            pnlTop.Controls.Add(btnBuscar);
            pnlTop.Dock = DockStyle.Top;
            pnlTop.Height = 52;
            pnlTop.Padding = new Padding(10);

            lblBuscar.AutoSize = true;
            lblBuscar.Location = new Point(10, 16);
            lblBuscar.Text = "Código / Barra / Descripción";

            txtBuscar.Location = new Point(170, 13);
            txtBuscar.Size = new Size(420, 23);

            btnBuscar.Location = new Point(600, 12);
            btnBuscar.Size = new Size(90, 25);
            btnBuscar.Text = "Buscar";

            pnlFill.Controls.Add(grid);
            pnlFill.Dock = DockStyle.Fill;
            pnlFill.Padding = new Padding(10, 0, 10, 0);

            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.AllowUserToResizeRows = false;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.BackgroundColor = Color.White;
            grid.Dock = DockStyle.Fill;
            grid.MultiSelect = false;
            grid.ReadOnly = true;
            grid.RowHeadersVisible = false;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.Columns.AddRange(new DataGridViewColumn[]
            {
                colCodigo, colCodBarra, colDescripcion, colUnidad, colPrecio
            });

            colCodigo.Name = "colCodigo";
            colCodigo.HeaderText = "Código";
            colCodigo.FillWeight = 90;

            colCodBarra.Name = "colCodBarra";
            colCodBarra.HeaderText = "Cod. Barra";
            colCodBarra.FillWeight = 120;

            colDescripcion.Name = "colDescripcion";
            colDescripcion.HeaderText = "Descripción";
            colDescripcion.FillWeight = 240;

            colUnidad.Name = "colUnidad";
            colUnidad.HeaderText = "Unidad";
            colUnidad.FillWeight = 70;

            colPrecio.Name = "colPrecio";
            colPrecio.HeaderText = "Precio";
            colPrecio.FillWeight = 80;

            pnlBottom.Controls.Add(lblTotal);
            pnlBottom.Controls.Add(btnAceptar);
            pnlBottom.Controls.Add(btnCancelar);
            pnlBottom.Dock = DockStyle.Bottom;
            pnlBottom.Height = 52;
            pnlBottom.Padding = new Padding(10);

            lblTotal.AutoSize = true;
            lblTotal.Location = new Point(10, 18);
            lblTotal.Text = "Total: 0";

            btnAceptar.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnAceptar.Location = new Point(530, 13);
            btnAceptar.Size = new Size(80, 26);
            btnAceptar.Text = "Aceptar";

            btnCancelar.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnCancelar.Location = new Point(620, 13);
            btnCancelar.Size = new Size(80, 26);
            btnCancelar.Text = "Cancelar";

            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(714, 430);
            Controls.Add(pnlFill);
            Controls.Add(pnlBottom);
            Controls.Add(pnlTop);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FormBuscarProducto";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Buscar producto";

            pnlTop.ResumeLayout(false);
            pnlTop.PerformLayout();
            pnlFill.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)grid).EndInit();
            pnlBottom.ResumeLayout(false);
            pnlBottom.PerformLayout();
            ResumeLayout(false);
        }
    }
}