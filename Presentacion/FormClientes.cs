using System;
using System.Windows.Forms;
using Andloe.Data;

namespace Andloe.Presentacion
{
    public partial class FormClientes : Form
    {
        private readonly ClienteRepository _repo = new();
        private readonly string? _filtroInicial;

        public FormClientes(string? filtroInicial = null)
        {
            _filtroInicial = filtroInicial;
            InitializeComponent();

            btnBuscar.Click += (_, __) => Cargar();
            txtBuscar.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) { Cargar(); e.SuppressKeyPress = true; } };
            btnNuevo.Click += (_, __) => Crear();
            btnEditar.Click += (_, __) => EditarActual();
            btnEliminar.Click += (_, __) => EliminarActual();
            grid.DoubleClick += (_, __) => EditarActual();

            Shown += (_, __) =>
            {
                if (!string.IsNullOrWhiteSpace(_filtroInicial))
                    txtBuscar.Text = _filtroInicial;

                Cargar();
            };
        }

        public void Cargar()
        {
            try
            {
                var data = _repo.Listar(txtBuscar.Text.Trim(), 200);
                grid.Rows.Clear();
                foreach (var c in data)
                {
                    grid.Rows.Add(new object[] {
                        (object)(c.Codigo    ?? string.Empty),
                        (object)(c.Nombre    ?? string.Empty),
                        (object)(c.RncCedula ?? string.Empty),
                        (object)(c.Telefono  ?? string.Empty),
                        (object)(c.Email     ?? string.Empty),
                        (object)((c.Estado == 1) ? "Activo" : "Inactivo")
                    });
                }
                lblTotal.Text = $"Total: {data.Count}";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Clientes", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AbrirEditor(string? codigo)
        {
            var principal = FormPrincipal.Instancia;
            if (principal == null)
            {
                MessageBox.Show(
                    "No se encontró la ventana principal del sistema.",
                    "Clientes",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            principal.OpenChild(new FormClienteEdit(codigo, txtBuscar.Text.Trim()));
        }

        private string? GetCodigoActual()
        {
            if (grid.CurrentRow == null) return null;
            return Convert.ToString(grid.CurrentRow.Cells["colCodigo"].Value);
        }

        private void Crear()
        {
            AbrirEditor(null);
        }

        private void EditarActual()
        {
            var cod = GetCodigoActual();
            if (string.IsNullOrWhiteSpace(cod)) return;

            AbrirEditor(cod);
        }

  

        private void EliminarActual()
        {
            var cod = GetCodigoActual();
            if (string.IsNullOrWhiteSpace(cod)) return;
            if (MessageBox.Show($"¿Eliminar cliente {cod}?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
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
}
