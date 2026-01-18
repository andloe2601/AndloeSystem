#nullable enable
using Andloe.Logica;
using Andloe.Data; // ✅ nuevo
using Presentation;
using Andloe.Presentacion;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Andloe.Presentacion
{
    public partial class FormPrincipal : Form
    {
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

        // ✅ repos para nombres
        private readonly EmpresaRepository _empresaRepo = new();
        private readonly SucursalRepository _sucursalRepo = new();
        private readonly AlmacenRepository _almacenRepo = new();
        private readonly CajaRepository _cajaRepo = new();

        // ✅ cache simple para evitar golpear DB cada click
        private readonly Dictionary<int, string> _empresaCache = new();
        private readonly Dictionary<int, string> _sucursalCache = new();
        private readonly Dictionary<int, string> _almacenCache = new();
        private readonly Dictionary<int, string> _cajaCache = new();

        public FormPrincipal(string usuario, string[] roles)
        {
            _usuario = usuario;
            _roles = roles ?? Array.Empty<string>();
            _auth = new AuthorizationService(_roles);

            InitializeComponent();
            ApplyTheme();
            ApplyPermissions();

            lblTitle.Text = $"Bienvenido, {_usuario}";

            // ✅ Mostrar empresa/sucursal/almacén conectado (NOMBRES)
            UpdateEmpresaConectadaLabel();

            btnGrpDashboard.PerformClick();
            btnDashboard.PerformClick();
        }

        private void ApplyTheme()
        {
            panelSidebar.BackColor = ColSidebar;
            panelTop.BackColor = ColTop;
            panelContent.BackColor = ColContent;

            lblBrand.ForeColor = ColText;

            lblTitle.ForeColor = ColText;

            // ✅ label de contexto
            lblEmpresaConectada.ForeColor = ColMuted;

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
            btnDashboard.Visible = _auth.Can(Permisos.VerDashboard);
            btnUsuarios.Visible = _auth.Can(Permisos.VerUsuarios);
            btnPOS.Visible = _auth.Can(Permisos.VerPOS);
            btnConfigSistema.Visible = _auth.Can(Permisos.Configurar);
        }

        private void ActivateButton(Button btn)
        {
            if (_activeButton == btn) return;

            if (_activeButton != null)
                _activeButton.BackColor = ColSidebar;

            _activeButton = btn;
            _activeButton.BackColor = ColSidebarActive;
        }

        /// <summary>Abre un Form dentro de panelContent (NO MDI)</summary>
        private void OpenChild(Form child)
        {
            try
            {
                if (_activeForm != null)
                {
                    panelContent.Controls.Remove(_activeForm);

                    _activeForm.Close();
                    _activeForm.Dispose();
                    _activeForm = null;
                }

                _activeForm = child;

                child.TopLevel = false;
                child.FormBorderStyle = FormBorderStyle.None;
                child.Dock = DockStyle.Fill;

                panelContent.Controls.Clear();
                panelContent.Controls.Add(child);

                child.Show();
                child.BringToFront();

                lblTitle.Text = child.Text;

                // ✅ refresca por si cambió el contexto
                UpdateEmpresaConectadaLabel();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "OpenChild", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ================== CONTEXTO (NOMBRES) ==================
        private string GetEmpresaNombre(int empresaId)
        {
            if (empresaId <= 0) return "(sin empresa)";
            if (_empresaCache.TryGetValue(empresaId, out var n)) return n;

            var nombre = _empresaRepo.ObtenerRazonSocial(empresaId);
            n = string.IsNullOrWhiteSpace(nombre) ? $"Empresa {empresaId}" : nombre.Trim();
            _empresaCache[empresaId] = n;
            return n;
        }

        private string GetSucursalNombre(int sucursalId)
        {
            if (sucursalId <= 0) return "(sin sucursal)";
            if (_sucursalCache.TryGetValue(sucursalId, out var n)) return n;

            var nombre = _sucursalRepo.ObtenerNombre(sucursalId);
            n = string.IsNullOrWhiteSpace(nombre) ? $"Sucursal {sucursalId}" : nombre.Trim();
            _sucursalCache[sucursalId] = n;
            return n;
        }

        private string GetAlmacenNombre(int almacenId)
        {
            if (almacenId <= 0) return "(sin almacén)";
            if (_almacenCache.TryGetValue(almacenId, out var n)) return n;

            var nombre = _almacenRepo.ObtenerNombre(almacenId);
            n = string.IsNullOrWhiteSpace(nombre) ? $"Almacén {almacenId}" : nombre.Trim();
            _almacenCache[almacenId] = n;
            return n;
        }

        private string? GetCajaNumero(int cajaId)
        {
            if (cajaId <= 0) return null;
            if (_cajaCache.TryGetValue(cajaId, out var n)) return n;

            var numero = _cajaRepo.ObtenerCajaNumero(cajaId);
            n = string.IsNullOrWhiteSpace(numero) ? $"Caja {cajaId}" : numero.Trim();
            _cajaCache[cajaId] = n;
            return n;
        }

        private void UpdateEmpresaConectadaLabel()
        {
            try
            {
                var s = SesionService.TryGet();
                if (s == null)
                {
                    lblEmpresaConectada.Text = "Empresa: (sin sesión)";
                    return;
                }

                var emp = GetEmpresaNombre(s.EmpresaId);
                var suc = GetSucursalNombre(s.SucursalId);
                var alm = GetAlmacenNombre(s.AlmacenId);

                var cajaTxt = "";
                if (s.CajaId.HasValue)
                {
                    var caja = GetCajaNumero(s.CajaId.Value);
                    if (!string.IsNullOrWhiteSpace(caja))
                        cajaTxt = $" | Caja: {caja}";
                }

                lblEmpresaConectada.Text = $"Empresa: {emp} | Sucursal: {suc} | Almacén: {alm}{cajaTxt}";
            }
            catch
            {
                lblEmpresaConectada.Text = "Empresa: (no disponible)";
            }
        }

        // ===================== COLLAPSE GROUPS =====================
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

        private void btnGrpDashboard_Click(object? sender, EventArgs e) => ToggleGroup(pnlDashboard);
        private void btnGrpContabilidad_Click(object? sender, EventArgs e) => ToggleGroup(pnlContabilidad);
        private void btnGrpVenta_Click(object? sender, EventArgs e) => ToggleGroup(pnlVenta);
        private void btnGrpCompra_Click(object? sender, EventArgs e) => ToggleGroup(pnlCompra);
        private void btnGrpProducto_Click(object? sender, EventArgs e) => ToggleGroup(pnlProducto);
        private void btnGrpInventario_Click(object? sender, EventArgs e) => ToggleGroup(pnlInventario);
        private void btnGrpNomina_Click(object? sender, EventArgs e) => ToggleGroup(pnlNomina);
        private void btnGrpConfiguracion_Click(object? sender, EventArgs e) => ToggleGroup(pnlConfiguracion);

        private void btnDashboard_Click(object? sender, EventArgs e)
        {
            ActivateButton(btnDashboard);
            OpenChild(new FormDashboard());
        }

        private void btnPOS_Click(object? sender, EventArgs e)
        {
            ActivateButton(btnPOS);

            using (var frmLogin = new FormLoginPosClave())
            {
                var drLogin = frmLogin.ShowDialog(this);
                if (drLogin != DialogResult.OK) return;

                var cajaId = frmLogin.CajaIdSeleccionada;
                var cajaNumero = frmLogin.CajaNumeroSeleccionada;
                var usuario = frmLogin.UsuarioLogueado;

                if (cajaId <= 0 || string.IsNullOrWhiteSpace(cajaNumero))
                {
                    MessageBox.Show("No se obtuvo información válida de caja desde el login.",
                        "POS", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // ✅ guarda caja en sesión y refresca label
              
              SesionService.SetCaja(cajaId);
                UpdateEmpresaConectadaLabel();

                using (var frmFondo = new FormFondoCaja(cajaId, cajaNumero, usuario))
                {
                    var drFondo = frmFondo.ShowDialog(this);
                    if (drFondo != DialogResult.OK) return;
                }

                bool puedeCerrarCaja =
                    usuario.Equals("ADMIN", StringComparison.OrdinalIgnoreCase) ||
                    usuario.Equals("SUPERVISOR", StringComparison.OrdinalIgnoreCase);

                using (var frmPos = new FormPOS(usuario, cajaId, cajaNumero, puedeCerrarCaja))
                {
                    frmPos.ShowDialog(this);
                }
            }
        }

        private void btnConfigSistema_Click(object? sender, EventArgs e)
        {
            ActivateButton(btnConfigSistema);
            OpenChild(new FormConfiguracion());
        }

        private void btnClientes_Click(object? sender, EventArgs e)
        {
            ActivateButton(btnClientes);
            OpenChild(new FormClientes());
        }

        private void menuFacturacion_Click(object? sender, EventArgs e)
        {
            OpenChild(new FormFacturaHistorial());
        }

        private void btnCierresHistorico_Click(object? sender, EventArgs e)
        {
            ActivateButton(btnCierresHistorico);
            OpenChild(new FormCierresHistorico());
        }

        private void btnProductos_Click(object? sender, EventArgs e)
        {
            ActivateButton(btnProductos);
            OpenChild(new FormProductos());
        }

        private void btnPromosProducto_Click(object? sender, EventArgs e)
        {
            ActivateButton(btnPromosProducto);
            OpenChild(new FormPromoHistorico(_usuario));
        }

        private void btnKardex_Click(object? sender, EventArgs e)
        {
            ActivateButton(btnKardex);
            OpenChild(new FormKardex());
        }

        private void btnInvMov_Click(object? sender, EventArgs e)
        {
            using (var frm = new Presentation.FormInvMovimiento(_usuario))
                frm.ShowDialog(this);
        }

        private void btnUsuarios_Click(object? sender, EventArgs e)
        {
            ActivateButton(btnUsuarios);
            OpenChild(new FormUsuarios(_auth));
        }

        private void btnConexion_Click(object? sender, EventArgs e)
        {
            ActivateButton(btnConexion);
            OpenChild(new FormConexion());
        }

        private void btnContabilidad_Click(object? sender, EventArgs e)
        {
            ActivateButton(btnContabilidad);
            MessageBox.Show("Módulo Contabilidad pendiente de implementación.", "ERP",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnCompra_Click(object? sender, EventArgs e)
        {
            ActivateButton(btnCompra);
            MessageBox.Show("Módulo Compra pendiente de implementación.", "ERP",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnNomina_Click(object? sender, EventArgs e)
        {
            ActivateButton(btnNomina);
            MessageBox.Show("Módulo Nómina pendiente de implementación.", "ERP",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnSalir_Click(object? sender, EventArgs e) => Close();
    }
}
#nullable restore
