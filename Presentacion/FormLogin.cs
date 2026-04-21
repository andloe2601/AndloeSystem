using Andloe.Data;
using Andloe.Logica;
using Andloe.Presentacion;
using Guna.UI2.WinForms;
using Microsoft.Data.SqlClient;
using Presentation;
using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Presentacion
{
    public partial class FormLogin : Form
    {
        private readonly AuthService _auth = new();

        public FormLogin()
        {
            InitializeComponent();
        }

        private void FormLogin_Load(object sender, EventArgs e)
        {
            try
            {
                ConfigurarFormulario();
                CargarEmpresas();
            }
            catch (Exception ex)
            {
                MostrarMensaje("Error cargando empresas: " + ex.Message);
                CargarPlaceholderEmpresa();
            }
        }

        private void FormLogin_Shown(object? sender, EventArgs e)
        {
            ActiveControl = txtUsuario;
            txtUsuario.Focus();
        }

        private void ConfigurarFormulario()
        {
            lblMsg.Text = "";
            lblMsg.Visible = false;
            lblMsg.AutoSize = false;
            lblMsg.TextAlign = ContentAlignment.MiddleCenter;

            txtPassword.UseSystemPasswordChar = true;
            chkMostrar.Checked = false;

            cbEmpresa.DropDownStyle = ComboBoxStyle.DropDownList;
            cbEmpresa.DrawMode = DrawMode.OwnerDrawFixed;
            cbEmpresa.ItemHeight = 30;

            cbEmpresa.BackColor = Color.Transparent;
            cbEmpresa.FillColor = Color.White;
            cbEmpresa.ForeColor = Color.Black;
            cbEmpresa.Font = new Font("Segoe UI", 10F);

            cbEmpresa.BorderColor = Color.LightGray;
            cbEmpresa.BorderThickness = 1;
            cbEmpresa.BorderRadius = 8;

            cbEmpresa.FocusedState.BorderColor = Color.DodgerBlue;
            cbEmpresa.HoverState.BorderColor = Color.DodgerBlue;

            cbEmpresa.ItemsAppearance.BackColor = Color.White;
            cbEmpresa.ItemsAppearance.ForeColor = Color.Black;
            cbEmpresa.ItemsAppearance.SelectedBackColor = Color.FromArgb(230, 230, 230);
            cbEmpresa.ItemsAppearance.SelectedForeColor = Color.Black;

            cbEmpresa.StartIndex = 0;
        }

        private void OnEnterKey(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                DoLogin();
            }
        }

        private void CargarEmpresas()
        {
            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
                SELECT EmpresaId, RazonSocial
                FROM dbo.Empresa
                WHERE Estado = 1
                ORDER BY RazonSocial;", cn);

            var dt = new DataTable();
            dt.Load(cmd.ExecuteReader());

            var fila = dt.NewRow();
            fila["EmpresaId"] = 0;
            fila["RazonSocial"] = "Seleccione una empresa";
            dt.Rows.InsertAt(fila, 0);

            cbEmpresa.DataSource = null;
            cbEmpresa.DisplayMember = "RazonSocial";
            cbEmpresa.ValueMember = "EmpresaId";
            cbEmpresa.DataSource = dt;

            cbEmpresa.SelectedIndex = 0;
        }

        private void CargarPlaceholderEmpresa()
        {
            var dt = new DataTable();
            dt.Columns.Add("EmpresaId", typeof(int));
            dt.Columns.Add("RazonSocial", typeof(string));

            dt.Rows.Add(0, "Seleccione una empresa");

            cbEmpresa.DataSource = null;
            cbEmpresa.DisplayMember = "RazonSocial";
            cbEmpresa.ValueMember = "EmpresaId";
            cbEmpresa.DataSource = dt;

            cbEmpresa.SelectedIndex = 0;
        }

        private void DoLogin()
        {
            LimpiarMensaje();

            var usuario = txtUsuario.Text.Trim();
            var pass = txtPassword.Text;

            if (string.IsNullOrWhiteSpace(usuario))
            {
                MostrarMensaje("Tiene que ingresar un usuario");
                txtUsuario.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(pass))
            {
                MostrarMensaje("Tiene que ingresar una contraseña");
                txtPassword.Focus();
                return;
            }

            if (cbEmpresa.SelectedValue == null ||
                !int.TryParse(cbEmpresa.SelectedValue.ToString(), out var empresaId) ||
                empresaId <= 0)
            {
                MostrarMensaje("Seleccione una empresa");
                cbEmpresa.Focus();
                return;
            }

            try
            {
                var res = _auth.Login(usuario, pass);

                if (!res.Exitoso)
                {
                    MostrarMensaje(res.Mensaje);
                    txtPassword.SelectAll();
                    txtPassword.Focus();
                    return;
                }

                var usuarioId = res.Usuario?.UsuarioId ?? 0;
                var displayUser = res.Usuario?.UsuarioNombre ?? usuario;
                var roles = res.Roles?.ToArray() ?? Array.Empty<string>();

                if (usuarioId <= 0)
                {
                    MostrarMensaje("Error al obtener el usuario.");
                    return;
                }

                // Debug temporal si quieres validar roles:
                // MessageBox.Show(string.Join(", ", roles), "Roles cargados");

                SesionService.Iniciar(usuarioId, displayUser, empresaId);
                AuditContext.SetUser(usuarioId, displayUser);

                Hide();

                if (EsAccesoPosDirecto(roles))
                {
                    AbrirPosDesdeLogin();
                }
                else
                {
                    using (var main = new FormPrincipal(displayUser, roles))
                    {
                        main.StartPosition = FormStartPosition.CenterScreen;
                        main.ShowDialog(this);
                    }
                }

                SesionService.Cerrar();
                AuditContext.ClearUser();

                Show();
                txtPassword.Clear();
                cbEmpresa.SelectedIndex = 0;
                txtUsuario.Focus();
            }
            catch (Exception ex)
            {
                MostrarMensaje("Error al iniciar sesión: " + ex.Message);
            }
        }


        private bool EsAccesoPosDirecto(string[] roles)
        {
            if (roles == null || roles.Length == 0) return false;

            return roles.Any(r =>
                !string.IsNullOrWhiteSpace(r) &&
                (r.Contains("POS", StringComparison.OrdinalIgnoreCase) ||
                 r.Contains("CAJA", StringComparison.OrdinalIgnoreCase) ||
                 r.Contains("CAJERO", StringComparison.OrdinalIgnoreCase)));
        }

        private void AbrirPosDesdeLogin()
        {
            using var frmLoginPos = new FormLoginPosClave();
            var dr = frmLoginPos.ShowDialog(this);
            if (dr != DialogResult.OK)
                return;

            var cajaId = frmLoginPos.CajaIdSeleccionada;
            var cajaNumero = frmLoginPos.CajaNumeroSeleccionada;
            var usuarioPos = frmLoginPos.UsuarioLogueado;

            if (cajaId <= 0 || string.IsNullOrWhiteSpace(cajaNumero))
            {
                MostrarMensaje("No se obtuvo una caja válida para abrir el POS.");
                return;
            }

            bool puedeCerrarCaja =
                usuarioPos.Equals("ADMIN", StringComparison.OrdinalIgnoreCase) ||
                usuarioPos.Equals("SUPERVISOR", StringComparison.OrdinalIgnoreCase);

            using var frmPos = new FormPosVenta(usuarioPos, cajaId, cajaNumero, puedeCerrarCaja);
            frmPos.StartPosition = FormStartPosition.CenterScreen;
            frmPos.ShowDialog(this);
        }

        private void MostrarMensaje(string mensaje)
        {
            lblMsg.Text = mensaje;
            lblMsg.Visible = true;
        }

        private void LimpiarMensaje()
        {
            lblMsg.Text = "";
            lblMsg.Visible = false;
        }

        private void btnEntrar_Click(object sender, EventArgs e)
        {
            DoLogin();
        }

        private void chkMostrar_CheckedChanged(object sender, EventArgs e)
        {
            txtPassword.UseSystemPasswordChar = !chkMostrar.Checked;
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            txtPassword.UseSystemPasswordChar = !txtPassword.UseSystemPasswordChar;
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void panelDerecho_Paint(object sender, PaintEventArgs e)
        {
        }

        private void lblTitulo_Click(object sender, EventArgs e)
        {
        }

        private void lblMsg_Click(object sender, EventArgs e)
        {
        }

        private void cbEmpresa_SelectedIndexChanged_2(object sender, EventArgs e)
        {
        }
    }
}