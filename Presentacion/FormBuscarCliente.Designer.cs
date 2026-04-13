using System.Drawing;
using System.Windows.Forms;

namespace Andloe.Presentacion
{
    partial class FormBuscarCliente
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
        private DataGridViewTextBoxColumn colNombre;
        private DataGridViewTextBoxColumn colRnc;
        private DataGridViewTextBoxColumn colTelefono;
        private DataGridViewTextBoxColumn colEstado;

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
            colNombre = new DataGridViewTextBoxColumn();
            colRnc = new DataGridViewTextBoxColumn();
            colTelefono = new DataGridViewTextBoxColumn();
            colEstado = new DataGridViewTextBoxColumn();

            pnlBottom = new Panel();
            lblTotal = new Label();
            btnAceptar = new Button();
            btnCancelar = new Button();

            pnlTop.SuspendLayout();
            pnlFill.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)grid).BeginInit();
            pnlBottom.SuspendLayout();
            SuspendLayout();

            // pnlTop
            pnlTop.Controls.Add(lblBuscar);
            pnlTop.Controls.Add(txtBuscar);
            pnlTop.Controls.Add(btnBuscar);
            pnlTop.Dock = DockStyle.Top;
            pnlTop.Height = 52;
            pnlTop.Padding = new Padding(10);

            // lblBuscar
            lblBuscar.AutoSize = true;
            lblBuscar.Location = new Point(10, 16);
            lblBuscar.Name = "lblBuscar";
            lblBuscar.Size = new Size(122, 15);
            lblBuscar.Text = "Código / Nombre / RNC";

            // txtBuscar
            txtBuscar.Location = new Point(140, 13);
            txtBuscar.Name = "txtBuscar";
            txtBuscar.Size = new Size(420, 23);
            txtBuscar.TabIndex = 0;

            // btnBuscar
            btnBuscar.Location = new Point(570, 12);
            btnBuscar.Name = "btnBuscar";
            btnBuscar.Size = new Size(90, 25);
            btnBuscar.TabIndex = 1;
            btnBuscar.Text = "Buscar";
            btnBuscar.UseVisualStyleBackColor = true;

            // pnlFill
            pnlFill.Controls.Add(grid);
            pnlFill.Dock = DockStyle.Fill;
            pnlFill.Padding = new Padding(10, 0, 10, 0);

            // grid
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
            grid.Name = "grid";
            grid.TabIndex = 2;
            grid.Columns.AddRange(new DataGridViewColumn[]
            {
                colCodigo, colNombre, colRnc, colTelefono, colEstado
            });

            // columnas
            colCodigo.Name = "colCodigo";
            colCodigo.HeaderText = "Código";
            colCodigo.FillWeight = 90;

            colNombre.Name = "colNombre";
            colNombre.HeaderText = "Nombre";
            colNombre.FillWeight = 220;

            colRnc.Name = "colRnc";
            colRnc.HeaderText = "RNC / Cédula";
            colRnc.FillWeight = 120;

            colTelefono.Name = "colTelefono";
            colTelefono.HeaderText = "Teléfono";
            colTelefono.FillWeight = 110;

            colEstado.Name = "colEstado";
            colEstado.HeaderText = "Estado";
            colEstado.FillWeight = 70;

            // pnlBottom
            pnlBottom.Controls.Add(lblTotal);
            pnlBottom.Controls.Add(btnAceptar);
            pnlBottom.Controls.Add(btnCancelar);
            pnlBottom.Dock = DockStyle.Bottom;
            pnlBottom.Height = 52;
            pnlBottom.Padding = new Padding(10);

            // lblTotal
            lblTotal.AutoSize = true;
            lblTotal.Location = new Point(10, 18);
            lblTotal.Name = "lblTotal";
            lblTotal.Size = new Size(45, 15);
            lblTotal.Text = "Total: 0";

            // btnAceptar
            btnAceptar.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnAceptar.Location = new Point(500, 13);
            btnAceptar.Name = "btnAceptar";
            btnAceptar.Size = new Size(90, 26);
            btnAceptar.TabIndex = 3;
            btnAceptar.Text = "Aceptar";
            btnAceptar.UseVisualStyleBackColor = true;

            // btnCancelar
            btnCancelar.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnCancelar.Location = new Point(600, 13);
            btnCancelar.Name = "btnCancelar";
            btnCancelar.Size = new Size(90, 26);
            btnCancelar.TabIndex = 4;
            btnCancelar.Text = "Cancelar";
            btnCancelar.UseVisualStyleBackColor = true;

            // FormBuscarCliente
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(704, 420);
            Controls.Add(pnlFill);
            Controls.Add(pnlBottom);
            Controls.Add(pnlTop);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FormBuscarCliente";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Buscar cliente";

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