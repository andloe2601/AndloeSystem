using System.Windows.Forms;

namespace Andloe.Presentacion
{
    partial class FormVendedores
    {
        private System.ComponentModel.IContainer components = null;
        private TextBox txtBuscar;
        private Button btnBuscar;
        private Button btnNuevo;
        private Button btnEditar;
        private Button btnEliminar;
        private DataGridView grid;
        private Label lblTotal;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            txtBuscar = new TextBox();
            btnBuscar = new Button();
            btnNuevo = new Button();
            btnEditar = new Button();
            btnEliminar = new Button();
            grid = new DataGridView();
            lblTotal = new Label();

            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.ReadOnly = true;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.MultiSelect = false;
            grid.Dock = DockStyle.Fill;
            grid.RowHeadersVisible = false;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "colCodigo", HeaderText = "Código", FillWeight = 18 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "colNombre", HeaderText = "Nombre", FillWeight = 34 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "colEmail", HeaderText = "Email", FillWeight = 22 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "colTel", HeaderText = "Teléfono", FillWeight = 16 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "colEstado", HeaderText = "Estado", FillWeight = 10 });

            var top = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 44, Padding = new Padding(8) };
            txtBuscar.Width = 320;
            btnBuscar.Text = "Buscar";
            btnNuevo.Text = "Crear";
            btnEditar.Text = "Editar";
            btnEliminar.Text = "Borrar";
            top.Controls.AddRange(new Control[] { txtBuscar, btnBuscar, btnNuevo, btnEditar, btnEliminar });

            lblTotal.Dock = DockStyle.Bottom;
            lblTotal.Padding = new Padding(8);
            lblTotal.Text = "Total: 0";

            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            ClientSize = new System.Drawing.Size(900, 560);
            Controls.Add(grid);
            Controls.Add(top);
            Controls.Add(lblTotal);

            Text = "Catálogo de Vendedores";
            Name = "FormVendedores";
            StartPosition = FormStartPosition.CenterScreen;
        }
    }
}
