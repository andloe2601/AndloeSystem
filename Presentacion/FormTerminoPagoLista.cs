using System;
using System.Windows.Forms;
using Andloe.Logica;
using Andloe.Entidad;

namespace Presentation
{
    public partial class FormTerminoPagoLista : Form
    {
        private readonly TerminoPagoService _service = new();

        public FormTerminoPagoLista()
        {
            InitializeComponent();
        }

        private void FormTerminoPagoLista_Load(object sender, EventArgs e)
        {
            CargarDatos();
        }

        private void CargarDatos()
        {
            try
            {
                var lista = _service.ListarTodos();
                dgvTerminos.DataSource = lista;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar términos de pago: " + ex.Message,
                    "Términos de Pago", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private TerminoPago? ObtenerSeleccionado()
        {
            if (dgvTerminos.CurrentRow == null)
                return null;

            return dgvTerminos.CurrentRow.DataBoundItem as TerminoPago;
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            var frm = new FormTerminoPagoEdicion();
            if (frm.ShowDialog(this) == DialogResult.OK)
            {
                CargarDatos();
            }
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            var seleccionado = ObtenerSeleccionado();
            if (seleccionado == null)
            {
                MessageBox.Show("Seleccione un registro.", "Términos de Pago",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var frm = new FormTerminoPagoEdicion(seleccionado.TerminoPagoId);
            if (frm.ShowDialog(this) == DialogResult.OK)
            {
                CargarDatos();
            }
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
