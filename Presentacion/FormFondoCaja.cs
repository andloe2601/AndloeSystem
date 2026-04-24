using System;
using System.Globalization;
using System.Windows.Forms;
using Andloe.Data;
using Andloe.Entidad;

namespace Andloe.Presentacion
{
    public partial class FormFondoCaja : Form
    {
        private readonly int _cajaId;
        private readonly string _cajaNumero;
        private readonly string _usuario;
        private readonly FondoCajaRepository _repo = new();

        public decimal MontoFondoRegistrado { get; private set; }
        public bool FondoExistente { get; private set; }

        public FormFondoCaja(int cajaId, string cajaNumero, string usuario)
        {
            _cajaId = cajaId;
            _cajaNumero = cajaNumero ?? string.Empty;
            _usuario = usuario ?? string.Empty;
            InitializeComponent();
        }

        private void FormFondoCaja_Load(object sender, EventArgs e)
        {
            txtCaja.Text = _cajaNumero;
            txtUsuario.Text = _usuario;

            var hoy = DateTime.Today;
            var fondo = _repo.ObtenerFondoAbierto(_cajaId, hoy);

            if (fondo != null)
            {
                FondoExistente = true;
                MontoFondoRegistrado = fondo.MontoFondo;

                txtMonto.Text = fondo.MontoFondo.ToString("N2");
                txtObservacion.Text = fondo.Observacion ?? "";

                txtMonto.Enabled = false;
                txtObservacion.Enabled = false;

                lblInfo.Text = $"Ya existe un fondo de caja para hoy ({fondo.MontoFondo:N2}).";
                btnAceptar.Text = "Continuar";
            }
            else
            {
                FondoExistente = false;
                MontoFondoRegistrado = 0m;
                lblInfo.Text = "Digite el fondo inicial de caja para este turno.";
                txtMonto.Focus();
            }
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            if (FondoExistente)
            {
                DialogResult = DialogResult.OK;
                Close();
                return;
            }

            if (!decimal.TryParse(
                    txtMonto.Text.Replace(",", ""),
                    NumberStyles.Any,
                    CultureInfo.InvariantCulture,
                    out var monto) || monto < 0)
            {
                MessageBox.Show("Monto de fondo inválido.",
                    "Fondo de Caja", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtMonto.Focus();
                txtMonto.SelectAll();
                return;
            }

            try
            {
                var f = new FondoCaja
                {
                    CajaId = _cajaId,
                    POS_CajaNumero = _cajaNumero,
                    FechaApertura = DateTime.Now,
                    UsuarioApertura = _usuario,
                    MontoFondo = monto,
                    Observacion = string.IsNullOrWhiteSpace(txtObservacion.Text)
                        ? null
                        : txtObservacion.Text.Trim(),
                    Estado = "ABIERTO"
                };

                var id = _repo.InsertarFondo(f);
                if (id <= 0)
                {
                    MessageBox.Show("No se pudo registrar el fondo de caja.",
                        "Fondo de Caja", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                MontoFondoRegistrado = monto;

                MessageBox.Show("Fondo de caja registrado correctamente.",
                    "Fondo de Caja", MessageBoxButtons.OK, MessageBoxIcon.Information);

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al registrar el fondo de caja: " + ex.Message,
                    "Fondo de Caja", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
