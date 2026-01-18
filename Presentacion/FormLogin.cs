using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using Andloe.Data;
using Andloe.Logica;

namespace Andloe.Presentacion
{
    public partial class FormLogin : Form
    {
        private readonly AuthService _auth = new();

        public FormLogin()
        {
            InitializeComponent();

            Load += FormLogin_Load;

            txtUsuario.KeyDown += OnEnterKey;
            txtPassword.KeyDown += OnEnterKey;

            btnEntrar.Click += (_, __) => DoLogin();
            btnCancelar.Click += (_, __) => Close();

            chkMostrar.CheckedChanged += (_, __) =>
            {
                txtPassword.UseSystemPasswordChar = !chkMostrar.Checked;
            };
        }

        private void FormLogin_Load(object? sender, EventArgs e)
        {
            try
            {
                CargarEmpresas();
                txtUsuario.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error cargando empresas", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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

            cbEmpresa.DisplayMember = "RazonSocial";
            cbEmpresa.ValueMember = "EmpresaId";
            cbEmpresa.DataSource = dt;

            if (cbEmpresa.Items.Count > 0)
                cbEmpresa.SelectedIndex = 0;
        }

        private void DoLogin()
        {
            lblMsg.Text = "";

            var usuario = txtUsuario.Text.Trim();
            var pass = txtPassword.Text;

            if (usuario.Length == 0) { lblMsg.Text = "Ingrese usuario."; txtUsuario.Focus(); return; }
            if (pass.Length == 0) { lblMsg.Text = "Ingrese contraseña."; txtPassword.Focus(); return; }

            if (cbEmpresa.SelectedValue == null ||
                !int.TryParse(cbEmpresa.SelectedValue.ToString(), out var empresaId) || empresaId <= 0)
            {
                lblMsg.Text = "Seleccione una empresa.";
                cbEmpresa.Focus();
                return;
            }

            try
            {
                var res = _auth.Login(usuario, pass);
                if (!res.Exitoso)
                {
                    lblMsg.Text = res.Mensaje;
                    txtPassword.SelectAll();
                    txtPassword.Focus();
                    return;
                }

                var usuarioId = res.Usuario?.UsuarioId ?? 0;
                var displayUser = res.Usuario?.UsuarioNombre ?? usuario;

                if (usuarioId <= 0)
                {
                    lblMsg.Text = "Login OK pero UsuarioId inválido. Revisa AuthResult.Usuario.";
                    return;
                }

                // ✅ Sesión
                SesionService.Iniciar(usuarioId, displayUser, empresaId);

                // ✅ Auditoría: set contexto y log de login
                AuditContext.SetUser(usuarioId, displayUser);
                new AuditoriaService().Log(
                    modulo: "SEGURIDAD",
                    accion: "LOGIN",
                    entidad: "Usuario",
                    entidadId: usuarioId.ToString(),
                    detalle: $"Login exitoso. EmpresaId={empresaId}"
                );

                // Roles para el principal (como ya lo tienes)
                var roles = (res.Roles ?? new()).Select(r => r.Nombre).ToArray();

                Hide();
                using (var main = new FormPrincipal(displayUser, roles))
                {
                    main.StartPosition = FormStartPosition.CenterScreen;
                    main.ShowDialog(this);
                }

                // ✅ Auditoría: logout (cuando se cierra el principal)
                new AuditoriaService().Log(
                    modulo: "SEGURIDAD",
                    accion: "LOGOUT",
                    entidad: "Usuario",
                    entidadId: usuarioId.ToString(),
                    detalle: "Salida del sistema"
                );

                // limpiar sesión
                SesionService.Cerrar();
                AuditContext.ClearUser();

                Show();
                txtPassword.Clear();
                txtPassword.Focus();
            }
            catch (Exception ex)
            {
                lblMsg.Text = ex.Message;
            }
        }
    }
}
