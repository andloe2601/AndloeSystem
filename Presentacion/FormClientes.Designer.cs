using System.Windows.Forms;

namespace Andloe.Presentacion
{
    partial class FormClientes
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
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "colCodigo", HeaderText = "Código", FillWeight = 15 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "colNombre", HeaderText = "Nombre", FillWeight = 35 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "colRNC", HeaderText = "RNC/Céd.", FillWeight = 15 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "colTel", HeaderText = "Teléfono", FillWeight = 15 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "colEmail", HeaderText = "Email", FillWeight = 20 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "colEstado", HeaderText = "Estado", FillWeight = 10 });

            var top = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 44, Padding = new Padding(8) };
            txtBuscar.Width = 320;
            btnBuscar.Text = "Buscar";
            btnNuevo.Text = "Nuevo";
            btnEditar.Text = "Editar";
            btnEliminar.Text = "Eliminar";
            top.Controls.AddRange(new Control[] { txtBuscar, btnBuscar, btnNuevo, btnEditar, btnEliminar });

            lblTotal.Dock = DockStyle.Bottom;
            lblTotal.Padding = new Padding(8);
            lblTotal.Text = "Total: 0";

            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            ClientSize = new System.Drawing.Size(900, 560);
            Controls.Add(grid);
            Controls.Add(top);
            Controls.Add(lblTotal);
            Text = "Clientes";
            Name = "FormClientes";
            StartPosition = FormStartPosition.CenterScreen;
        }
    }
}
