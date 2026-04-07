using System;
using System.Linq;
using System.Windows.Forms;
using Andloe.Logica;

namespace Andloe.Presentacion
{
    public partial class FormUsuarioDetalle : Form
    {
        private readonly UsuarioService _svc = new();
        private readonly int? _usuarioId;

        public bool GuardadoOk { get; private set; }

        public FormUsuarioDetalle()
        {
            InitializeComponent();
            CargarEstados();
            CargarRoles();
            CargarEmpresas();

            Text = "Nuevo usuario";
            cmbEstado.SelectedItem = "Activo";

            chkCambiarPassword.Checked = true;
            chkCambiarPassword.Enabled = false;
            txtPassword.Enabled = true;
            txtConfirmar.Enabled = true;
        }

        public FormUsuarioDetalle(int usuarioId) : this()
        {
            _usuarioId = usuarioId;
            Text = "Editar usuario";
            chkCambiarPassword.Enabled = true;
            CargarUsuario(usuarioId);
        }

        private void CargarEstados()
        {
            cmbEstado.Items.Clear();
            cmbEstado.Items.Add("Activo");
            cmbEstado.Items.Add("Inactivo");
            cmbEstado.Items.Add("Bloqueado");
            cmbEstado.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        private void CargarRoles()
        {
            clbRoles.Items.Clear();

            var roles = _svc.ListarRolesSistema();
            foreach (var rol in roles)
            {
                clbRoles.Items.Add(new RolItem(rol.RolId, rol.Nombre), false);
            }
        }

        private void CargarEmpresas()
        {
            var empresas = _svc.ListarEmpresas();

            cmbEmpresa.DataSource = empresas
                .Select(x => new EmpresaItem(x.EmpresaId, x.Nombre))
                .ToList();

            cmbEmpresa.DisplayMember = "Nombre";
            cmbEmpresa.ValueMember = "EmpresaId";
            cmbEmpresa.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbEmpresa.SelectedIndex = -1;
        }

        private void MarcarRolesDelUsuario(int usuarioId)
        {
            var rolesUsuario = _svc.ObtenerRolesDeUsuario(usuarioId)
                .Select(r => r.RolId)
                .ToHashSet();

            for (int i = 0; i < clbRoles.Items.Count; i++)
            {
                if (clbRoles.Items[i] is RolItem item)
                    clbRoles.SetItemChecked(i, rolesUsuario.Contains(item.RolId));
            }
        }

        private void CargarEmpresaDelUsuario(int usuarioId)
        {
            var empresaId = _svc.ObtenerEmpresaDeUsuario(usuarioId);
            if (empresaId.HasValue)
                cmbEmpresa.SelectedValue = empresaId.Value;
        }

        private void CargarUsuario(int usuarioId)
        {
            var u = _svc.Obtener(usuarioId);
            if (u == null)
            {
                MessageBox.Show("Usuario no encontrado.", "Usuarios", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                DialogResult = DialogResult.Cancel;
                Close();
                return;
            }

            txtUsuario.Text = u.UsuarioNombre;
            txtEmail.Text = u.Email;
            cmbEstado.SelectedItem = string.IsNullOrWhiteSpace(u.Estado) ? "Activo" : u.Estado;

            txtPassword.Clear();
            txtConfirmar.Clear();
            chkCambiarPassword.Checked = false;
            txtPassword.Enabled = false;
            txtConfirmar.Enabled = false;

            MarcarRolesDelUsuario(usuarioId);
            CargarEmpresaDelUsuario(usuarioId);
        }

        private void chkCambiarPassword_CheckedChanged(object sender, EventArgs e)
        {
            bool enabled = _usuarioId == null || chkCambiarPassword.Checked;
            txtPassword.Enabled = enabled;
            txtConfirmar.Enabled = enabled;

            if (!enabled)
            {
                txtPassword.Clear();
                txtConfirmar.Clear();
            }
        }

        private int[] ObtenerRolesSeleccionados()
        {
            return clbRoles.CheckedItems
                .OfType<RolItem>()
                .Select(x => x.RolId)
                .ToArray();
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                var usuario = txtUsuario.Text.Trim();
                var email = string.IsNullOrWhiteSpace(txtEmail.Text) ? null : txtEmail.Text.Trim();
                var estado = cmbEstado.SelectedItem?.ToString() ?? "Activo";
                var password = txtPassword.Text;
                var confirmar = txtConfirmar.Text;
                var rolesSeleccionados = ObtenerRolesSeleccionados();

                if (cmbEmpresa.SelectedItem is not EmpresaItem empresaSeleccionada)
                    throw new InvalidOperationException("Debe seleccionar una empresa.");

                if (string.IsNullOrWhiteSpace(usuario))
                    throw new InvalidOperationException("Debe indicar el usuario.");

                if (rolesSeleccionados.Length == 0)
                    throw new InvalidOperationException("Debe seleccionar al menos un rol.");

                bool requierePassword = _usuarioId == null || chkCambiarPassword.Checked;

                if (requierePassword)
                {
                    if (string.IsNullOrWhiteSpace(password))
                        throw new InvalidOperationException("Debe indicar la contraseña.");

                    if (password != confirmar)
                        throw new InvalidOperationException("La confirmación de contraseña no coincide.");
                }

                if (_usuarioId == null)
                {
                    int nuevoId = _svc.Crear(usuario, email, estado, password);
                    _svc.GuardarRolesDeUsuario(nuevoId, rolesSeleccionados);
                    _svc.GuardarEmpresaDeUsuario(nuevoId, empresaSeleccionada.EmpresaId);
                }
                else
                {
                    _svc.Actualizar(_usuarioId.Value, usuario, email, estado);

                    if (chkCambiarPassword.Checked)
                        _svc.CambiarPassword(_usuarioId.Value, password);

                    _svc.GuardarRolesDeUsuario(_usuarioId.Value, rolesSeleccionados);
                    _svc.GuardarEmpresaDeUsuario(_usuarioId.Value, empresaSeleccionada.EmpresaId);
                }

                GuardadoOk = true;
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Usuarios", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private sealed class RolItem
        {
            public int RolId { get; }
            public string Nombre { get; }

            public RolItem(int rolId, string nombre)
            {
                RolId = rolId;
                Nombre = nombre ?? string.Empty;
            }

            public override string ToString() => Nombre;
        }

        private sealed class EmpresaItem
        {
            public int EmpresaId { get; }
            public string Nombre { get; }

            public EmpresaItem(int empresaId, string nombre)
            {
                EmpresaId = empresaId;
                Nombre = nombre ?? string.Empty;
            }

            public override string ToString() => Nombre;
        }
    }
}