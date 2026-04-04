using System;
using System.Windows.Forms;
using Andloe.Data;

namespace Andloe.Presentacion
{
    public partial class FormVendedores : Form
    {
        private readonly VendedorRepository _repo = new();

        public FormVendedores()
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
                var data = _repo.Listar(txtBuscar.Text.Trim(), 200, incluirInactivos: true);

                grid.Rows.Clear();
                foreach (var v in data)
                {
                    grid.Rows.Add(new object[]
                    {
                        (object)(v.Codigo ?? string.Empty),
                        (object)(v.Nombre ?? string.Empty),
                        (object)(v.Email ?? string.Empty),
                        (object)(v.Telefono ?? string.Empty),
                        (object)(v.Estado ? "Activo" : "Inactivo")
                    });
                }

                lblTotal.Text = $"Total: {data.Count}";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Catálogo de Vendedores", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string? GetCodigoActual()
        {
            if (grid.CurrentRow == null) return null;
            return Convert.ToString(grid.CurrentRow.Cells["colCodigo"].Value);
        }

        private void Crear()
        {
            using var frm = new FormVendedorEdit(null);
            if (frm.ShowDialog(this) == DialogResult.OK)
                Cargar();
        }

        private void EditarActual()
        {
            var cod = GetCodigoActual();
            if (string.IsNullOrWhiteSpace(cod)) return;

            using var frm = new FormVendedorEdit(cod);
            if (frm.ShowDialog(this) == DialogResult.OK)
                Cargar();
        }

        private void EliminarActual()
        {
            var cod = GetCodigoActual();
            if (string.IsNullOrWhiteSpace(cod)) return;

            if (MessageBox.Show($"¿Inactivar vendedor {cod}?",
                    "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            try
            {
                _repo.Eliminar(cod);
                Cargar();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Eliminar", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
