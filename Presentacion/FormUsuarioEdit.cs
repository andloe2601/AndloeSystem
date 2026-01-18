using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Andloe.Logica;
using Andloe.Entidad;

namespace Andloe.Presentacion
{
    public partial class FormUsuarioEdit : Form
    {
        private readonly UsuarioService _svc = new();
        private readonly int? _usuarioId;              // null = crear
        private List<Rol> _rolesSistema = new();

        public FormUsuarioEdit(int? usuarioId = null)
        {
            _usuarioId = usuarioId;
            InitializeComponent();

            // eventos
            chkCambiarClave.CheckedChanged += (_, __) => TogglePasswordFields();
            btnCancelar.Click += (_, __) => { DialogResult = DialogResult.Cancel; Close(); };
            btnGuardar.Click += (_, __) => Guardar();

            // cargar roles y usuario (si aplica)
            CargarRolesSistema();

            if (_usuarioId.HasValue)
            {
                CargarUsuario();
            }
            else
            {
                Text = "Crear Usuario";
                chkCambiarClave.Checked = true; // en creación, obligar contraseña
                TogglePasswordFields();
            }
        }

        private void TogglePasswordFields()
        {
            var enabled = chkCambiarClave.Checked;
            txtClave.Enabled = enabled;
            txtConfirmar.Enabled = enabled;
        }

        private void CargarRolesSistema()
        {
            _rolesSistema = _svc.ListarRolesSistema();
            clbRoles.Items.Clear();
            foreach (var r in _rolesSistema)
                clbRoles.Items.Add(r.Nombre); // índice coincide con _rolesSistema
        }

        private void MarcarRolesDeUsuario(int usuarioId)
        {
            var rolesUser = _svc.ObtenerRolesDeUsuario(usuarioId).Select(r => r.RolId).ToHashSet();
            for (int i = 0; i < _rolesSistema.Count; i++)
                clbRoles.SetItemChecked(i, rolesUser.Contains(_rolesSistema[i].RolId));
        }

        private void CargarUsuario()
        {
            var u = _svc.Obtener(_usuarioId!.Value) ?? throw new InvalidOperationException("Usuario no encontrado.");
            Text = $"Editar Usuario #{u.UsuarioId}";
            txtUsuario.Text = u.UsuarioNombre;
            txtEmail.Text = u.Email;
            cboEstado.SelectedItem = string.IsNullOrWhiteSpace(u.Estado) ? "Activo" : u.Estado;

            chkCambiarClave.Checked = false;
            TogglePasswordFields();

            MarcarRolesDeUsuario(u.UsuarioId);
        }

        private IEnumerable<int> RolesSeleccionados()
        {
            var ids = new List<int>();
            for (int i = 0; i < clbRoles.Items.Count; i++)
                if (clbRoles.GetItemChecked(i))
                    ids.Add(_rolesSistema[i].RolId);
            return ids;
        }

        private void Guardar()
        {
            try
            {
                lblMsg.Text = "";
                var usuario = txtUsuario.Text.Trim();
                var email = txtEmail.Text.Trim();
                var estado = cboEstado.SelectedItem?.ToString() ?? "Activo";

                if (_usuarioId is null)
                {
                    // crear
                    if (!chkCambiarClave.Checked) { lblMsg.Text = "Defina una contraseña."; return; }
                    if (txtClave.Text != txtConfirmar.Text) { lblMsg.Text = "Las contraseñas no coinciden."; return; }

                    var nuevoId = _svc.Crear(usuario,
                                             string.IsNullOrWhiteSpace(email) ? null : email,
                                             estado,
                                             txtClave.Text);

                    _svc.GuardarRolesDeUsuario(nuevoId, RolesSeleccionados());
                }
                else
                {
                    // editar
                    _svc.Actualizar(_usuarioId.Value,
                                    usuario,
                                    string.IsNullOrWhiteSpace(email) ? null : email,
                                    estado);

                    if (chkCambiarClave.Checked)
                    {
                        if (txtClave.Text != txtConfirmar.Text) { lblMsg.Text = "Las contraseñas no coinciden."; return; }
                        _svc.CambiarPassword(_usuarioId.Value, txtClave.Text);
                    }

                    _svc.GuardarRolesDeUsuario(_usuarioId.Value, RolesSeleccionados());
                }

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                lblMsg.Text = ex.Message;
            }
        }
    }
}
