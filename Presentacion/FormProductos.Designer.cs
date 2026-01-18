using System.Windows.Forms;

namespace Andloe.Presentacion
{
    partial class FormProductos
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

            // grid
            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.ReadOnly = true;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.MultiSelect = false;
            grid.Dock = DockStyle.Fill;
            grid.RowHeadersVisible = false;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "colCodigo", HeaderText = "Código", FillWeight = 20 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "colDescripcion", HeaderText = "Descripción", FillWeight = 40 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "colUnidad", HeaderText = "Unidad", FillWeight = 10 });

            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colPrecio",
                HeaderText = "Precio",
                FillWeight = 15,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "N2" }
            });

            // ✅ NUEVO: Precio Coste
            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colPrecioCoste",
                HeaderText = "Precio Coste",
                FillWeight = 15,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "N2" }
            });

            // ✅ NUEVO: Existencia
            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colExistencia",
                HeaderText = "Existencia",
                FillWeight = 15,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "N2" }
            });

            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "colEstado", HeaderText = "Estado", FillWeight = 15 });

            // top panel
            var top = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 44,
                Padding = new Padding(8),
                FlowDirection = FlowDirection.LeftToRight
            };

            txtBuscar.Width = 320;
            btnBuscar.Text = "Buscar";
            btnNuevo.Text = "Nuevo";
            btnEditar.Text = "Editar";
            btnEliminar.Text = "Eliminar";
            top.Controls.AddRange(new Control[] { txtBuscar, btnBuscar, btnNuevo, btnEditar, btnEliminar });

            // bottom
            var bottom = new StatusStrip();
            var tsi = new ToolStripStatusLabel();
            bottom.Items.Add(tsi);

            lblTotal.Dock = DockStyle.Bottom;
            lblTotal.Padding = new Padding(8);
            lblTotal.Text = "Total: 0";

            // form
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            ClientSize = new System.Drawing.Size(900, 560);
            Controls.Add(grid);
            Controls.Add(top);
            Controls.Add(lblTotal);
            Text = "Productos";
            Name = "FormProductos";
            StartPosition = FormStartPosition.CenterScreen;
        }
    }
}
