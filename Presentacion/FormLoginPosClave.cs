using Andloe.Data;
using Andloe.Entidad;
using System;
using System.Windows.Forms;

namespace Presentation
{
    public partial class FormLoginPosClave : Form
    {
        private readonly PosLoginRepository _loginRepo = new();
        private readonly CajaRepository _cajaRepo = new();
        private readonly FondoCajaRepository _fondoRepo = new();

        public event Action<int, string, string>? OnAccesoCorrecto;

        // Datos que usará el POS
        public string UsuarioLogueado { get; private set; } = string.Empty;
        public int CajaIdSeleccionada { get; private set; }
        public string CajaNumeroSeleccionada { get; private set; } = string.Empty;

        public FormLoginPosClave()
        {
            InitializeComponent();
        }

        private void FormLoginPosClave_Load(object sender, EventArgs e)
        {
            try
            {
                lblUsuarioValor.Text = "";   // aún no sabemos cuál es, depende del PIN

                var cajas = _cajaRepo.ListarActivas();
                if (cajas.Count == 0)
                {
                    MessageBox.Show("No hay cajas activas configuradas en la base de datos.",
                        "POS", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                cbCaja.DisplayMember = "Display";  // propiedad de CajaDto
                cbCaja.ValueMember = "CajaId";
                cbCaja.DataSource = cajas;

                if (cajas.Count > 0)
                    cbCaja.SelectedIndex = 0;

                txtClave.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar cajas: " + ex.Message,
                    "POS", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ========================
        //   TECLADO NUMÉRICO
        // ========================

        private const int MAX_LONGITUD_PIN = 6;

        private void AgregarDigito(string digito)
        {
            if (txtClave.Text.Length >= MAX_LONGITUD_PIN)
                return;

            txtClave.Text += digito;
        }

        private void btn0_Click(object sender, EventArgs e) => AgregarDigito("0");
        private void btn1_Click(object sender, EventArgs e) => AgregarDigito("1");
        private void btn2_Click(object sender, EventArgs e) => AgregarDigito("2");
        private void btn3_Click(object sender, EventArgs e) => AgregarDigito("3");
        private void btn4_Click(object sender, EventArgs e) => AgregarDigito("4");
        private void btn5_Click(object sender, EventArgs e) => AgregarDigito("5");
        private void btn6_Click(object sender, EventArgs e) => AgregarDigito("6");
        private void btn7_Click(object sender, EventArgs e) => AgregarDigito("7");
        private void btn8_Click(object sender, EventArgs e) => AgregarDigito("8");
        private void btn9_Click(object sender, EventArgs e) => AgregarDigito("9");

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtClave.Clear();
            txtClave.Focus();
        }

        private void btnBackspace_Click(object sender, EventArgs e)
        {
            var txt = txtClave.Text;
            if (string.IsNullOrEmpty(txt)) return;

            txtClave.Text = txt[..^1];
            txtClave.SelectionStart = txtClave.Text.Length;
            txtClave.Focus();
        }

        // ========================
        //   VALIDAR LOGIN POS
        // ========================

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            // 1) Validar selección de caja
            if (cbCaja.SelectedItem == null)
            {
                MessageBox.Show("Seleccione una caja.", "POS",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cbCaja.Focus();
                return;
            }

            // 2) Validar PIN
            var pin = txtClave.Text.Trim();
            if (string.IsNullOrWhiteSpace(pin))
            {
                MessageBox.Show("Ingrese la clave/PIN.", "POS",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtClave.Focus();
                return;
            }

            try
            {
                // 3) Validar SOLO PIN en PosUsuario
                var result = _loginRepo.ValidarPorPin(pin);
                if (result == null || !result.Ok)
                {
                    MessageBox.Show("Clave incorrecta o usuario inactivo.",
                        "POS", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtClave.Clear();
                    txtClave.Focus();
                    return;
                }

                // Usuario viene de la tabla PosUsuario
                UsuarioLogueado = result.Usuario;
                lblUsuarioValor.Text = UsuarioLogueado;

                // 4) Validar caja seleccionada
                if (cbCaja.SelectedValue is not int cajaIdSel)
                {
                    MessageBox.Show("Caja seleccionada inválida.", "POS",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string cajaNumSel;
                if (cbCaja.SelectedItem is CajaDto dto)
                    cajaNumSel = dto.CajaNumero;
                else
                    cajaNumSel = cbCaja.Text;

                // 5) Validar permisos de la caja según PosUsuario
                if (!result.PuedeTodasCajas)
                {
                    if (!result.CajaId.HasValue)
                    {
                        MessageBox.Show("El usuario no tiene una caja asignada.",
                            "POS", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    if (result.CajaId.Value != cajaIdSel)
                    {
                        MessageBox.Show(
                            $"Este usuario solo puede usar la caja {result.CajaId.Value}.",
                            "POS", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }

                // 6) Asignar propiedades seleccionadas
                CajaIdSeleccionada = cajaIdSel;
                CajaNumeroSeleccionada = cajaNumSel;

                // 7) 🔹 Verificar / Solicitar Fondo de Caja (solo una vez por día y caja)
                var hoy = DateTime.Today;
                var fondo = _fondoRepo.ObtenerFondoAbierto(cajaIdSel, hoy);

                if (fondo == null)
                {
                    // No existe fondo abierto hoy → pedirlo
                    using (var frmFondo = new FormFondoCaja(cajaIdSel, cajaNumSel, UsuarioLogueado))
                    {
                        var dr = frmFondo.ShowDialog(this);
                        if (dr != DialogResult.OK)
                        {
                            // Si cancela, no entra al POS
                            return;
                        }

                        // Si quieres usar el monto:
                        // var montoFondo = frmFondo.MontoFondoRegistrado;
                    }
                }
                // Si ya hay fondo, simplemente continúa al POS.

                // 8) Notificar acceso correcto al POS
                OnAccesoCorrecto?.Invoke(
                    CajaIdSeleccionada,
                    CajaNumeroSeleccionada,
                    UsuarioLogueado);

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al validar acceso POS: " + ex.Message,
                    "POS", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
