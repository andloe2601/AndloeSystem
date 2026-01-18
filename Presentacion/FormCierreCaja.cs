using System;
using System.Globalization;
using System.Windows.Forms;
using Andloe.Logica;
using Andloe.Data;
using Andloe.Entidad;

namespace Presentation
{
    public partial class FormCierreCaja : Form
    {
        private readonly int _cajaId;
        private readonly string _cajaNumero;
        private readonly string _usuario;

        private readonly CierreCajaService _cierreSrv = new();
        private readonly FondoCajaRepository _fondoRepo = new();   // 🔹 AQUÍ: instancia del repo

        private decimal _fondoInicial = 0m;
        private CierreCajaResumen? _resumenActual;

        // 🔹 Constructor principal: POS debe llamar este
        public FormCierreCaja(int cajaId, string cajaNumero, string usuario)
        {
            _cajaId = cajaId;
            _cajaNumero = cajaNumero ?? string.Empty;
            _usuario = usuario ?? string.Empty;

            InitializeComponent();
        }

        // 🔹 Constructor sin parámetros (por si el diseñador lo requiere)
        public FormCierreCaja() : this(0, string.Empty, string.Empty)
        {
        }

        private void FormCierreCaja_Load(object sender, EventArgs e)
        {
            try
            {
                lblCajaValor.Text = _cajaNumero;
                lblUsuarioValor.Text = _usuario;

                // Rango por defecto: hoy
                var hoy = DateTime.Today;
                dtDesde.Value = hoy;
                dtHasta.Value = hoy.AddDays(1).AddSeconds(-1);

                // 🔹 Buscar fondo de caja abierto para hoy (USANDO LA INSTANCIA)
                var fondo = _fondoRepo.ObtenerFondoAbierto(_cajaId, hoy);
                if (fondo != null)
                {
                    _fondoInicial = fondo.MontoFondo;
                    txtFondoInicial.Text = _fondoInicial.ToString("N2");
                    lblInfoFondo.Text = $"Fondo abierto hoy: {_fondoInicial:N2} por {fondo.UsuarioApertura}";
                }
                else
                {
                    _fondoInicial = 0m;
                    txtFondoInicial.Text = "0.00";
                    lblInfoFondo.Text = "No se encontró fondo de caja abierto para hoy.";
                }

                RecalcularResumen();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar pantalla de cierre: " + ex.Message,
                    "Cierre de Caja", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ========================
        //   REGLAS DE CÁLCULO
        // ========================

        private void RecalcularResumen()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(_cajaNumero))
                {
                    MessageBox.Show("No se ha definido el número de caja.",
                        "Cierre de Caja", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var desde = dtDesde.Value;
                var hasta = dtHasta.Value;

                _resumenActual = _cierreSrv.CalcularResumen(_cajaNumero, desde, hasta);

                txtTotalVentas.Text = _resumenActual.TotalVentas.ToString("N2");
                txtTotalPagos.Text = _resumenActual.TotalPagos.ToString("N2");

                // Efectivo teórico por movimientos (ventas/pagos)
                txtEfectivoMovimientos.Text = _resumenActual.EfectivoTeorico.ToString("N2");

                // Efectivo teórico final = movimientos + fondo inicial
                var efectivoTeoricoTotal = _resumenActual.EfectivoTeorico + _fondoInicial;
                txtEfectivoTeorico.Text = efectivoTeoricoTotal.ToString("N2");

                // Si no han digitado declarado, ponemos 0
                if (string.IsNullOrWhiteSpace(txtEfectivoDeclarado.Text))
                    txtEfectivoDeclarado.Text = "0.00";

                RecalcularDiferencia();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al calcular resumen: " + ex.Message,
                    "Cierre de Caja", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RecalcularDiferencia()
        {
            try
            {
                if (!decimal.TryParse(
                        txtEfectivoTeorico.Text.Replace(",", ""),
                        NumberStyles.Any,
                        CultureInfo.InvariantCulture,
                        out var teorico))
                    teorico = 0m;

                if (!decimal.TryParse(
                        txtEfectivoDeclarado.Text.Replace(",", ""),
                        NumberStyles.Any,
                        CultureInfo.InvariantCulture,
                        out var declarado))
                    declarado = 0m;

                var diferencia = declarado - teorico;
                txtDiferencia.Text = diferencia.ToString("N2");
            }
            catch
            {
                txtDiferencia.Text = "0.00";
            }
        }

        // ========================
        //   EVENTOS DE CONTROLES
        // ========================

        private void dtDesde_ValueChanged(object sender, EventArgs e)
        {
            RecalcularResumen();
        }

        private void dtHasta_ValueChanged(object sender, EventArgs e)
        {
            RecalcularResumen();
        }

        private void txtFondoInicial_Leave(object sender, EventArgs e)
        {
            if (!decimal.TryParse(
                    txtFondoInicial.Text.Replace(",", ""),
                    NumberStyles.Any,
                    CultureInfo.InvariantCulture,
                    out var fondo))
            {
                fondo = 0m;
            }

            _fondoInicial = fondo;
            txtFondoInicial.Text = _fondoInicial.ToString("N2");

            // Recalcular resumen para actualizar el teórico final
            if (_resumenActual != null)
            {
                var efectivoTeoricoTotal = _resumenActual.EfectivoTeorico + _fondoInicial;
                txtEfectivoTeorico.Text = efectivoTeoricoTotal.ToString("N2");
                RecalcularDiferencia();
            }
        }

        private void txtEfectivoDeclarado_Leave(object sender, EventArgs e)
        {
            RecalcularDiferencia();
        }

        private void btnRecalcular_Click(object sender, EventArgs e)
        {
            RecalcularResumen();
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                if (_cajaId <= 0 || string.IsNullOrWhiteSpace(_cajaNumero))
                {
                    MessageBox.Show("Caja no definida correctamente.", "Cierre de Caja",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (_resumenActual == null)
                {
                    MessageBox.Show("No se ha calculado el resumen.", "Cierre de Caja",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!decimal.TryParse(
                        txtFondoInicial.Text.Replace(",", ""),
                        NumberStyles.Any,
                        CultureInfo.InvariantCulture,
                        out var fondoInicial))
                {
                    MessageBox.Show("Fondo inicial inválido.", "Cierre de Caja",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtFondoInicial.Focus();
                    return;
                }

                if (!decimal.TryParse(
                        txtEfectivoDeclarado.Text.Replace(",", ""),
                        NumberStyles.Any,
                        CultureInfo.InvariantCulture,
                        out var efectivoDeclarado))
                {
                    MessageBox.Show("Efectivo declarado inválido.", "Cierre de Caja",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtEfectivoDeclarado.Focus();
                    return;
                }

                var desde = dtDesde.Value;
                var hasta = dtHasta.Value;

                var cierreId = _cierreSrv.GuardarCierre(
                    cajaId: _cajaId,
                    cajaNumero: _cajaNumero,
                    desde: desde,
                    hasta: hasta,
                    fondoInicial: fondoInicial,
                    efectivoDeclarado: efectivoDeclarado,
                    usuarioCierre: _usuario);

                MessageBox.Show($"Cierre de caja registrado correctamente.\nID: {cierreId}",
                    "Cierre de Caja", MessageBoxButtons.OK, MessageBoxIcon.Information);

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar cierre de caja: " + ex.Message,
                    "Cierre de Caja", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void btnHistorial_Click(object sender, EventArgs e)
        {
            using (var frm = new FormCierresHistorico())
            {
                frm.ShowDialog(this);
            }
        }
    }
}
