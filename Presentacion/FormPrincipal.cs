using Andloe.Logica;
using Presentacion;
using Presentation;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Andloe.Presentacion
{
    public partial class FormPrincipal : Form
    {
        public static FormPrincipal? Instancia { get; private set; }

        private readonly string _usuario;
        private readonly string[] _roles;
        private readonly AuthorizationService _auth;

        private Form? _activeForm;
        private Button? _activeButton;

        private static readonly Color ColSidebar = Color.FromArgb(24, 30, 54);
        private static readonly Color ColSidebarActive = Color.FromArgb(46, 51, 73);
        private static readonly Color ColTop = Color.FromArgb(46, 51, 73);
        private static readonly Color ColText = Color.White;
        private static readonly Color ColMuted = Color.FromArgb(170, 175, 190);
        private static readonly Color ColContent = Color.White;

        public FormPrincipal(string usuario, string[] roles)
        {
            Instancia = this;

            _usuario = usuario;
            _roles = roles ?? Array.Empty<string>();
            _auth = new AuthorizationService(_roles);

            InitializeComponent();
            ApplyTheme();
            ApplyPermissions();
            CollapseAllGroups();

            lblTitle.Text = $"Bienvenido, {_usuario}";

            btnGrpDashboard.PerformClick();
            if (btnDashboard.Visible)
                btnDashboard.PerformClick();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            if (ReferenceEquals(Instancia, this))
                Instancia = null;

            base.OnFormClosed(e);
        }

        private void ApplyTheme()
        {
            panelSidebar.BackColor = ColSidebar;
            panelTop.BackColor = ColTop;
            panelContent.BackColor = ColContent;

            lblBrand.ForeColor = ColText;
            lblTitle.ForeColor = ColText;

            foreach (var b in panelSidebar.Controls.OfType<Button>())
            {
                b.ForeColor = ColText;
                b.FlatAppearance.BorderSize = 0;
                b.FlatStyle = FlatStyle.Flat;
                b.Cursor = Cursors.Hand;
                b.BackColor = ColSidebar;
                b.Padding = new Padding(12, 0, 0, 0);
                b.TextAlign = ContentAlignment.MiddleLeft;
            }

            foreach (var p in panelSidebar.Controls.OfType<Panel>())
            {
                foreach (var b in p.Controls.OfType<Button>())
                {
                    b.ForeColor = ColMuted;
                    b.FlatAppearance.BorderSize = 0;
                    b.FlatStyle = FlatStyle.Flat;
                    b.Cursor = Cursors.Hand;
                    b.BackColor = ColSidebar;
                    b.Padding = new Padding(32, 0, 0, 0);
                    b.TextAlign = ContentAlignment.MiddleLeft;
                    b.Height = 36;
                }
            }

            MinimumSize = new Size(1100, 700);
            DoubleBuffered = true;
        }

        private void ApplyPermissions()
        {
            bool canUsuarios = _auth.Can(Permisos.VerUsuarios);
            bool canConfig = _auth.Can(Permisos.Configurar);

            btnDashboard.Visible = _auth.Can(Permisos.VerDashboard);
            btnUsuarios.Visible = canUsuarios;
            btnPOS.Visible = _auth.Can(Permisos.VerPOS);
            btnConfigSistema.Visible = canConfig;

            // Opcionales
            bool canContable = true;
            bool canConexion = true;

            btnConfiguracionContable.Visible = canContable;
            btnConexion.Visible = canConexion;

            // 🔥 Grupo basado en permisos reales
            btnGrpConfiguracion.Visible =
                canUsuarios ||
                canConfig ||
                canContable ||
                canConexion;
        }

        private void CollapseAllGroups()
        {
            pnlDashboard.Visible = false;
            pnlContabilidad.Visible = false;
            pnlVenta.Visible = false;
            pnlCompra.Visible = false;
            pnlProducto.Visible = false;
            pnlInventario.Visible = false;
            pnlNomina.Visible = false;
            pnlConfiguracion.Visible = false;
        }

        private void ToggleGroup(Panel groupPanel)
        {
            bool willShow = !groupPanel.Visible;

            CollapseAllGroups();
            groupPanel.Visible = willShow;

            if (willShow)
                panelSidebar.ScrollControlIntoView(groupPanel);
        }

        private void ActivateButton(Button btn)
        {
            if (_activeButton == btn)
                return;

            if (_activeButton != null)
                _activeButton.BackColor = ColSidebar;

            _activeButton = btn;
            _activeButton.BackColor = ColSidebarActive;
        }

        public void OpenChild(Form child)
        {
            try
            {
                panelContent.SuspendLayout();

                if (_activeForm != null)
                {
                    _activeForm.Close();
                    _activeForm.Dispose();
                    _activeForm = null;
                }

                panelContent.Controls.Clear();

                _activeForm = child;
                child.TopLevel = false;
                child.FormBorderStyle = FormBorderStyle.None;
                child.Dock = DockStyle.Fill;

                panelContent.Controls.Add(child);
                panelContent.Tag = child;

                child.Show();
                child.BringToFront();

                lblTitle.Text = string.IsNullOrWhiteSpace(child.Text) ? "ERP" : child.Text;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Error al abrir formulario",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                panelContent.ResumeLayout();
            }
        }

        private void btnGrpDashboard_Click(object sender, EventArgs e) => ToggleGroup(pnlDashboard);
        private void btnGrpContabilidad_Click(object sender, EventArgs e) => ToggleGroup(pnlContabilidad);
        private void btnGrpVenta_Click(object sender, EventArgs e) => ToggleGroup(pnlVenta);
        private void btnGrpCompra_Click(object sender, EventArgs e) => ToggleGroup(pnlCompra);
        private void btnGrpProducto_Click(object sender, EventArgs e) => ToggleGroup(pnlProducto);
        private void btnGrpInventario_Click(object sender, EventArgs e) => ToggleGroup(pnlInventario);
        private void btnGrpNomina_Click(object sender, EventArgs e) => ToggleGroup(pnlNomina);
        private void btnGrpConfiguracion_Click(object sender, EventArgs e) => ToggleGroup(pnlConfiguracion);

        private void btnDashboard_Click(object sender, EventArgs e)
        {
            ActivateButton(btnDashboard);
            OpenChild(new FormDashboard(OpenChild, _auth));
        }

        private void btnPOS_Click(object sender, EventArgs e)
        {
            ActivateButton(btnPOS);

            using (var frmLogin = new FormLoginPosClave())
            {
                var drLogin = frmLogin.ShowDialog(this);
                if (drLogin != DialogResult.OK)
                    return;

                var cajaId = frmLogin.CajaIdSeleccionada;
                var cajaNumero = frmLogin.CajaNumeroSeleccionada;
                var usuario = frmLogin.UsuarioLogueado;

                if (cajaId <= 0 || string.IsNullOrWhiteSpace(cajaNumero))
                {
                    MessageBox.Show(
                        "No se obtuvo información válida de caja desde el login.",
                        "POS",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }

                bool puedeCerrarCaja =
                    usuario.Equals("ADMIN", StringComparison.OrdinalIgnoreCase) ||
                    usuario.Equals("SUPERVISOR", StringComparison.OrdinalIgnoreCase);

                using (var frmPos = new FormPosVenta(usuario, cajaId, cajaNumero, puedeCerrarCaja))
                {
                    frmPos.ShowDialog(this);
                }
            }
        }

        private void btnClientes_Click(object sender, EventArgs e)
        {
            ActivateButton(btnClientes);
            OpenChild(new FormClientes());
        }

        private void btnVendedores_Click(object sender, EventArgs e)
        {
            ActivateButton(btnVendedores);
            OpenChild(new FormVendedores());
        }

        private void btnCajas_Click(object sender, EventArgs e)
        {
            ActivateButton(btnCajas);
            OpenChild(new FormFacturaHistorial());
        }

        private void btnCierresHistorico_Click(object sender, EventArgs e)
        {
            ActivateButton(btnCierresHistorico);
            OpenChild(new FormCierresHistorico());
        }

        private void btnProductos_Click(object sender, EventArgs e)
        {
            ActivateButton(btnProductos);
            OpenChild(new FormProductos());
        }

        private void btnPromosProducto_Click(object sender, EventArgs e)
        {
            ActivateButton(btnPromosProducto);
            OpenChild(new FormPromoHistorico(_usuario));
        }

        private void BtnEtiquetasBarras_Click(object sender, EventArgs e)
        {
            ActivateButton(BtnEtiquetasBarras);
            MessageBox.Show("Módulo pendiente.");
        }

        private void btnKardex_Click(object sender, EventArgs e)
        {
            ActivateButton(btnKardex);
            OpenChild(new FormKardex());
        }

        private void btnInvMov_Click(object sender, EventArgs e)
        {
            ActivateButton(btnInvMov);
            MessageBox.Show("Movimiento inventario.");
        }

        private void btnUsuarios_Click(object sender, EventArgs e)
        {
            ActivateButton(btnUsuarios);
            OpenChild(new FormUsuarios(_auth));
        }

        private void btnConfigSistema_Click(object sender, EventArgs e)
        {
            ActivateButton(btnConfigSistema);
            OpenChild(new FormConfiguracion());
        }

        private void btnConexion_Click(object sender, EventArgs e)
        {
            ActivateButton(btnConexion);
            OpenChild(new FormConexion());
        }

        private void btnContabilidadMovimiento_Click(object sender, EventArgs e)
        {
            ActivateButton(btnContabilidadMovimiento);
            OpenChild(new FormContabilidadMovimiento());
        }

        private void btnCatalogoCuentas_Click(object sender, EventArgs e)
        {
            ActivateButton(btnCatalogoCuentas);
            OpenChild(new FormCatalogoCuentas());
        }

        private void btnConfiguracionContable_Click(object sender, EventArgs e)
        {
            ActivateButton(btnConfiguracionContable);
            OpenChild(new FormConfiguracionContable());
        }

        private void btnContabilidad_Click(object sender, EventArgs e)
        {
            ActivateButton(btnContabilidad);
            MessageBox.Show("Módulo pendiente.");
        }

        private void btnCompra_Click(object sender, EventArgs e)
        {
            ActivateButton(btnCompra);
            MessageBox.Show("Módulo pendiente.");
        }

        private void btnNomina_Click(object sender, EventArgs e)
        {
            ActivateButton(btnNomina);
            MessageBox.Show("Módulo pendiente.");
        }

        private void btnECN_Click(object sender, EventArgs e)
        {
            ActivateButton(btnECN);
            OpenChild(new FormAlanubeMonitor());
        }

        private void BtnNc_Click(object sender, EventArgs e)
        {
            ActivateButton(BtnNc);
            OpenChild(new FormNotaCreditoVentaECF(1, _usuario, null));
        }

        private void BtnRecibo_Click(object sender, EventArgs e)
        {
            ActivateButton(BtnRecibo);
            OpenChild(new FormCxCCobros());
        }

        private void BtnReporte_Click(object sender, EventArgs e)
        {
            ActivateButton(BtnReporte);
            OpenChild(new FormCxCReportes());
        }

        private void BtnConfiguracionVentas_Click(object sender, EventArgs e)
        {
            ActivateButton(BtnConfiguracionVentas);
            MessageBox.Show("Módulo pendiente.");
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}