using System;
using System.Windows.Forms;
using Andloe.Logica;
using Andloe.Entidad;

namespace Presentation
{
    public partial class FormTerminoPagoEdicion : Form
    {
        private readonly TerminoPagoService _service = new();
        private readonly int _id;
        private readonly bool _esNuevo;

        public FormTerminoPagoEdicion()
        {
            _esNuevo = true;
            InitializeComponent();
        }

        public FormTerminoPagoEdicion(int terminoPagoId)
        {
            _id = terminoPagoId;
            _esNuevo = false;
            InitializeComponent();
        }

        private void FormTerminoPagoEdicion_Load(object sender, EventArgs e)
        {
            if (!_esNuevo)
                CargarDatos();
        }

        private void CargarDatos()
        {
            var t = _service.ObtenerPorId(_id);
            if (t == null)
            {
                MessageBox.Show("No se encontró el término de pago.", "Términos de Pago",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                DialogResult = DialogResult.Cancel;
                Close();
                return;
            }

            txtCodigo.Text = t.Codigo;
            txtDescripcion.Text = t.Descripcion;
            nudDiasPlazo.Value = t.DiasPlazo;

            chkTieneDescuento.Checked = t.TieneDescuento;
            if (t.PorcDescuento.HasValue)
                nudPorcDescuento.Value = t.PorcDescuento.Value;
            if (t.DiasDescuento.HasValue)
                nudDiasDescuento.Value = t.DiasDescuento.Value;

            chkActivo.Checked = t.Estado;
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                var t = new TerminoPago
                {
                    TerminoPagoId = _id,
                    Codigo = txtCodigo.Text,
                    Descripcion = txtDescripcion.Text,
                    DiasPlazo = (int)nudDiasPlazo.Value,
                    TieneDescuento = chkTieneDescuento.Checked,
                    PorcDescuento = chkTieneDescuento.Checked ? nudPorcDescuento.Value : (decimal?)null,
                    DiasDescuento = chkTieneDescuento.Checked ? (int?)nudDiasDescuento.Value : null,
                    Estado = chkActivo.Checked,
                    Usuario = Environment.UserName  // o tu usuario de sesión
                };

                if (_esNuevo)
                    _service.Crear(t);
                else
                    _service.Actualizar(t);

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar: " + ex.Message,
                    "Términos de Pago", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void chkTieneDescuento_CheckedChanged(object sender, EventArgs e)
        {
            nudPorcDescuento.Enabled = chkTieneDescuento.Checked;
            nudDiasDescuento.Enabled = chkTieneDescuento.Checked;
        }
    }
}
