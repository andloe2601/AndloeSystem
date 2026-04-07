using System;
using System.Windows.Forms;
using Andloe.Logica;
using Andloe.Entidad;

namespace Andloe.Presentacion
{
    public partial class FormUsuarios : Form
    {
        private readonly AuthorizationService _auth;
        private readonly UsuarioService _svc = new();

        private int? _usuarioIdSeleccionado = null;

        public FormUsuarios(AuthorizationService auth)
        {
            InitializeComponent();
            _auth = auth;

            ConfigurarPermisos();
            WireEvents();
            CargarUsuarios();
        }

        private void ConfigurarPermisos()
        {
            bool canEdit = _auth.Can(Permisos.EditUsuarios);

            btnNuevo.Enabled = canEdit;
            btnEditar.Enabled = canEdit;
            btnEliminar.Enabled = canEdit;
            btnEditarForm.Enabled = canEdit;

            btnDetalle.Enabled = true;

            ctxMenuNuevo.Enabled = canEdit;
            ctxMenuEditar.Enabled = canEdit;
            ctxMenuEliminar.Enabled = canEdit;
        }

        private void WireEvents()
        {
            dgvUsuarios.SelectionChanged += (_, __) => Seleccionar();

            btnRefrescar.Click += (_, __) => CargarUsuarios();
            btnNuevo.Click += (_, __) => Nuevo();
            btnEditar.Click += (_, __) => Editar();
            btnEliminar.Click += (_, __) => Eliminar();

            btnDetalle.Click += (_, __) => AbrirDetalle();
            btnEditarForm.Click += (_, __) => AbrirEditarForm();

            btnEmptyNew.Click += (_, __) => Nuevo();

            ctxMenuNuevo.Click += (_, __) => Nuevo();
            ctxMenuEditar.Click += (_, __) => Editar();
            ctxMenuEliminar.Click += (_, __) => Eliminar();

            btnLimpiarBusqueda.Click += (_, __) =>
            {
                txtBuscar.Clear();
                CargarUsuarios();
            };

            dgvUsuarios.DataError += (s, e) =>
            {
                e.ThrowException = false;
            };

            txtBuscar.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    CargarUsuarios();
                }
            };
        }

        private void CargarUsuarios()
        {
            try
            {
                toolProgress.Visible = true;
                lblInfo.Text = "Cargando usuarios...";

                var lista = _svc.Listar(txtBuscar.Text);
                dgvUsuarios.DataSource = lista;

                emptyPanel.Visible = lista.Count == 0;
                lblInfo.Text = $"Registros: {lista.Count}";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Usuarios", MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblInfo.Text = "Error al cargar";
            }
            finally
            {
                toolProgress.Visible = false;
            }
        }

        // 🔥 FIX DEFINITIVO AQUÍ
        private void Seleccionar()
        {
            if (dgvUsuarios.CurrentRow == null)
            {
                _usuarioIdSeleccionado = null;
                return;
            }

            var value = dgvUsuarios.CurrentRow.Cells[0].Value;

            if (value == null || value == DBNull.Value)
            {
                _usuarioIdSeleccionado = null;
                return;
            }

            _usuarioIdSeleccionado = Convert.ToInt32(value);
        }

        private void Nuevo()
        {
            using var frm = new FormUsuarioDetalle();
            if (frm.ShowDialog(this) == DialogResult.OK && frm.GuardadoOk)
                CargarUsuarios();
        }

        private void Editar()
        {
            if (_usuarioIdSeleccionado == null)
            {
                MessageBox.Show("Seleccione un usuario.", "Usuarios", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using var frm = new FormUsuarioDetalle(_usuarioIdSeleccionado.Value);
            if (frm.ShowDialog(this) == DialogResult.OK && frm.GuardadoOk)
                CargarUsuarios();
        }

        private void AbrirDetalle()
        {
            if (_usuarioIdSeleccionado == null)
            {
                MessageBox.Show("Seleccione un usuario.", "Usuarios", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using var frm = new FormUsuarioDetalle(_usuarioIdSeleccionado.Value);
            frm.ShowDialog(this);
        }

        private void AbrirEditarForm()
        {
            if (_usuarioIdSeleccionado == null)
            {
                MessageBox.Show("Seleccione un usuario.", "Usuarios", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using var frm = new FormUsuarioEdit(_usuarioIdSeleccionado.Value);
            if (frm.ShowDialog(this) == DialogResult.OK)
                CargarUsuarios();
        }

        private void Eliminar()
        {
            if (_usuarioIdSeleccionado == null)
            {
                MessageBox.Show("Seleccione un usuario.", "Usuarios", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var r = MessageBox.Show(
                "¿Eliminar usuario?",
                "Confirmar",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (r != DialogResult.Yes)
                return;

            try
            {
                _svc.Eliminar(_usuarioIdSeleccionado.Value);
                _usuarioIdSeleccionado = null;
                CargarUsuarios();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Usuarios", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}