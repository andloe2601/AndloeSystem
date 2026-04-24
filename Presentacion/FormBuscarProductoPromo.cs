using System;
using System.Linq;
using System.Windows.Forms;
using Andloe.Data;

namespace Andloe.Presentacion
{
    public partial class FormBuscarProductoPromo : Form
    {
        private readonly ProductoRepository _prodRepo = new();

        public string? ProductoCodigoSeleccionado { get; private set; }
        public string? ProductoNombreSeleccionado { get; private set; }

        public FormBuscarProductoPromo()
        {
            InitializeComponent();
        }

        private void FormBuscarProductoPromo_Load(object sender, EventArgs e)
        {
            CargarDatos();
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            CargarDatos();
        }

        private void CargarDatos()
        {
            try
            {
                var filtro = txtFiltro.Text.Trim();
                var lista = _prodRepo.ListarParaPromo(filtro);

                dgvProductos.Rows.Clear();
                foreach (var p in lista)
                {
                    dgvProductos.Rows.Add(p.Codigo, p.Descripcion, p.PrecioVenta);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al listar productos: " + ex.Message,
                    "Productos", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SeleccionarActual()
        {
            if (dgvProductos.CurrentRow == null) return;

            ProductoCodigoSeleccionado =
                Convert.ToString(dgvProductos.CurrentRow.Cells["colCodigo"].Value);
            ProductoNombreSeleccionado =
                Convert.ToString(dgvProductos.CurrentRow.Cells["colDescripcion"].Value);

            if (!string.IsNullOrWhiteSpace(ProductoCodigoSeleccionado))
            {
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private void dgvProductos_DoubleClick(object sender, EventArgs e)
        {
            SeleccionarActual();
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            SeleccionarActual();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
