using System;
using System.Windows.Forms;
using Andloe.Data;
using Andloe.Entidad;

namespace Andloe.Presentacion
{
    public partial class FormProductos : Form
    {
        private readonly ProductoRepository _repo = new();

        public FormProductos()
        {
            InitializeComponent();

            btnBuscar.Click += (_, __) => Cargar();
            txtBuscar.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    Cargar();
                    e.SuppressKeyPress = true;
                }
            };

            btnNuevo.Click += (_, __) => Crear();
            btnEditar.Click += (_, __) => EditarActual();
            btnEliminar.Click += (_, __) => EliminarActual();
            grid.DoubleClick += (_, __) => EditarActual();

            Shown += (_, __) => Cargar();
        }

        private void Cargar()
        {
            try
            {
                var data = _repo.Listar(txtBuscar.Text.Trim(), 200);

                grid.Rows.Clear();

                foreach (var p in data)
                {
                    // OJO: Listar() trae básico; si StockActual / PrecioCoste no vienen,
                    // el grid mostrará 0.00 hasta que los incluyas en el SELECT de Listar().
                    var existencia = p.StockActual;                 // decimal (si viene)
                    var precioCoste = p.PrecioCoste;          // nullable decimal

                    grid.Rows.Add(new object[]
                    {
                        (object)(p.Codigo       ?? string.Empty),               // colCodigo
                        (object)(p.Descripcion  ?? string.Empty),               // colDescripcion
                        (object)(p.UnidadBase   ?? string.Empty),               // colUnidad
                        (object)p.PrecioVenta,                                  // colPrecioVenta
                        (object)precioCoste,                                     // colExistencia 
                        (object)existencia,                                    // colPrecioCoste 
                        (object)((p.Estado == 1) ? "Activo" : "Inactivo")       // colEstado
                    });
                }

                lblTotal.Text = $"Total: {data.Count}";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Productos", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private Producto? GetSeleccionado()
        {
            if (grid.CurrentRow == null) return null;

            var cod = Convert.ToString(grid.CurrentRow.Cells["colCodigo"].Value) ?? "";
            if (string.IsNullOrWhiteSpace(cod)) return null;

            return _repo.ObtenerPorCodigo(cod);
        }

        private void Crear()
        {
            using var frm = new FormProductoEdit(null);

            // ✅ solo refresca si le dio Finalizar (DialogResult.OK)
            if (frm.ShowDialog(this) == DialogResult.OK)
                Cargar();
        }

        private void EditarActual()
        {
            var p = GetSeleccionado();
            if (p == null) return;

            using var frm = new FormProductoEdit(p.Codigo);

            // si decides que al editar también cierre con OK, refresca igual
            if (frm.ShowDialog(this) == DialogResult.OK)
                Cargar();
        }

        private void EliminarActual()
        {
            var p = GetSeleccionado();
            if (p == null) return;

            if (MessageBox.Show($"¿Eliminar producto {p.Codigo}?", "Confirmar",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    _repo.Eliminar(p.Codigo);
                    Cargar();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Eliminar", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
