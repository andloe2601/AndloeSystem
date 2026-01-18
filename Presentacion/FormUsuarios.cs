using System;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Andloe.Data;
using Andloe.Entidad;
using Andloe.Logica;

namespace Andloe.Presentacion
{
    public partial class FormUsuarios : Form
    {
        private readonly AuthorizationService _auth;
        private readonly UsuarioRepository _repo = new();
        private readonly UsuarioService _svc = new();

        private readonly BindingSource _binding = new();
        private CancellationTokenSource? _loadCts;

        public FormUsuarios(AuthorizationService auth)
        {
            _auth = auth ?? throw new ArgumentNullException(nameof(auth));
            InitializeComponent();

            // UI tweaks and event hookups
            DoubleBuffered = true;
            SetupGridStyle();
            WireEvents();
            ApplyPermissions();

            // Bind the grid to the BindingSource
            dgvUsuarios.DataSource = _binding;

            // react to data changes to show/hide the empty panel
            _binding.ListChanged += Binding_ListChanged;
        }

        // Designer fallback
        public FormUsuarios() : this(new AuthorizationService(Array.Empty<string>())) { }

        private void SetupGridStyle()
        {
            // Set selection colors safely
            var sel = new DataGridViewCellStyle
            {
                SelectionBackColor = Color.FromArgb(0, 120, 215),
                SelectionForeColor = Color.White
            };
            dgvUsuarios.DefaultCellStyle.SelectionBackColor = sel.SelectionBackColor;
            dgvUsuarios.DefaultCellStyle.SelectionForeColor = sel.SelectionForeColor;

            // Make header visually prominent
            dgvUsuarios.EnableHeadersVisualStyles = false;

            // Optional: better row height and font
            dgvUsuarios.RowTemplate.Height = 30;
            dgvUsuarios.Font = new Font("Segoe UI", 9F);
        }

        private void WireEvents()
        {
            // header paint for gradient (modern look)
            headerPanel.Paint += HeaderPanel_Paint;

            // Search actions
            txtBuscar.KeyDown += TxtBuscar_KeyDown;
            btnLimpiarBusqueda.Click += (_, __) => { txtBuscar.Clear(); _ = CargarAsync(); };
            btnRefrescar.Click += async (_, __) => await CargarAsync(txtBuscar.Text.Trim());
            btnNuevo.Click += (_, __) => NewUsuario();
            btnEditar.Click += (_, __) => EditSelectedUsuario();
            btnEliminar.Click += async (_, __) => await DeleteSelectedUsuarioAsync();

            // context menu
            ctxMenuNuevo.Click += (_, __) => NewUsuario();
            ctxMenuEditar.Click += (_, __) => EditSelectedUsuario();
            ctxMenuEliminar.Click += async (_, __) => await DeleteSelectedUsuarioAsync();

            // grid interactions
            dgvUsuarios.CellDoubleClick += (s, e) =>
            {
                if (e.RowIndex >= 0) EditSelectedUsuario();
            };

            // empty panel CTA
            btnEmptyNew.Click += (_, __) => NewUsuario();

            // keyboard shortcuts
            KeyPreview = true;
            KeyDown += async (s, ev) =>
            {
                if (ev.KeyCode == Keys.F5) { ev.Handled = true; await CargarAsync(txtBuscar.Text.Trim()); }
                else if (ev.Control && ev.KeyCode == Keys.N) { ev.Handled = true; NewUsuario(); }
                else if (ev.Control && ev.KeyCode == Keys.E) { ev.Handled = true; EditSelectedUsuario(); }
                else if (ev.KeyCode == Keys.Delete) { ev.Handled = true; await DeleteSelectedUsuarioAsync(); }
            };

            // show/hide empty overlay initially
            emptyPanel.Visible = false;
        }

        private void Binding_ListChanged(object? sender, ListChangedEventArgs e)
        {
            // Show emptyPanel when no items; hide otherwise
            var count = _binding.Count;
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() =>
                {
                    emptyPanel.Visible = (count == 0);
                    lblInfo.Text = $"Registros: {count}";
                }));
            }
            else
            {
                emptyPanel.Visible = (count == 0);
                lblInfo.Text = $"Registros: {count}";
            }
        }

        private void ApplyPermissions()
        {
            var canEdit = _auth.Can(Permisos.EditUsuarios);
            btnNuevo.Enabled = canEdit;
            btnEditar.Enabled = canEdit;
            btnEliminar.Enabled = canEdit;

            ctxMenuEditar.Enabled = canEdit;
            ctxMenuEliminar.Enabled = canEdit;
            ctxMenuNuevo.Enabled = canEdit;
            btnEmptyNew.Enabled = canEdit;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            // initial load
            _ = CargarAsync();
        }

        private void HeaderPanel_Paint(object? sender, PaintEventArgs e)
        {
            // Draw a soft horizontal gradient
            var g = e.Graphics;
            var rect = headerPanel.ClientRectangle;
            using var brush = new System.Drawing.Drawing2D.LinearGradientBrush(
                rect,
                Color.FromArgb(250, 250, 251), // light top
                Color.FromArgb(240, 246, 255), // slightly bluish bottom
                System.Drawing.Drawing2D.LinearGradientMode.Vertical);
            g.FillRectangle(brush, rect);

            // subtle bottom separator
            using var pen = new Pen(Color.FromArgb(220, 225, 235));
            g.DrawLine(pen, rect.Left, rect.Bottom - 1, rect.Right, rect.Bottom - 1);
        }

        private async void TxtBuscar_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                await CargarAsync(txtBuscar.Text.Trim());
            }
        }

        private int? GetSelectedId()
        {
            if (dgvUsuarios.CurrentRow == null) return null;
            if (dgvUsuarios.CurrentRow.Cells["colId"].Value is int id) return id;
            // sometimes datasource returns long or string; try parse
            var v = dgvUsuarios.CurrentRow.Cells["colId"].Value;
            if (v != null && int.TryParse(v.ToString(), out var pid)) return pid;
            return null;
        }

        private async Task CargarAsync(string? filtro = null)
        {
            // Cancel previous load
            _loadCts?.Cancel();
            _loadCts?.Dispose();
            _loadCts = new CancellationTokenSource();
            var ct = _loadCts.Token;

            try
            {
                SetLoading(true, "Cargando...");
                var list = await Task.Run(() => _repo.Listar(filtro), ct).ConfigureAwait(false);
                if (ct.IsCancellationRequested) return;

                // Marshal back to UI
                if (InvokeRequired)
                {
                    BeginInvoke(new Action(() =>
                    {
                        _binding.DataSource = new BindingList<Usuario>(list);
                        // Binding_ListChanged will update lblInfo & emptyPanel
                        SetLoading(false, "Listo");
                    }));
                }
                else
                {
                    _binding.DataSource = new BindingList<Usuario>(list);
                    SetLoading(false, "Listo");
                }
            }
            catch (OperationCanceledException)
            {
                SetLoading(false, "Cancelado");
            }
            catch (Exception ex)
            {
                SetLoading(false, "Error");
                MessageBox.Show(this, $"Error al cargar usuarios: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetLoading(bool loading, string statusText)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => SetLoading(loading, statusText)));
                return;
            }

            toolProgress.Visible = loading;
            lblInfo.Text = statusText;
            // disable controls briefly while loading
            btnRefrescar.Enabled = !loading;
            btnNuevo.Enabled = !loading && _auth.Can(Permisos.EditUsuarios);
            btnEditar.Enabled = !loading && _auth.Can(Permisos.EditUsuarios);
            btnEliminar.Enabled = !loading && _auth.Can(Permisos.EditUsuarios);
            txtBuscar.Enabled = !loading;
        }

        private void NewUsuario()
        {
            if (!_auth.Can(Permisos.EditUsuarios)) { MessageBox.Show("No autorizado."); return; }
            using var f = new FormUsuarioEdit(null);
            if (f.ShowDialog(this) == DialogResult.OK) _ = CargarAsync(txtBuscar.Text.Trim());
        }

        private void EditSelectedUsuario()
        {
            if (!_auth.Can(Permisos.EditUsuarios)) { MessageBox.Show("No autorizado."); return; }
            var id = GetSelectedId();
            if (id == null) { MessageBox.Show("Seleccione un usuario."); return; }

            using var f = new FormUsuarioEdit(id.Value);
            if (f.ShowDialog(this) == DialogResult.OK) _ = CargarAsync(txtBuscar.Text.Trim());
        }

        private async Task DeleteSelectedUsuarioAsync()
        {
            if (!_auth.Can(Permisos.EditUsuarios)) { MessageBox.Show("No autorizado."); return; }
            var id = GetSelectedId();
            if (id == null) { MessageBox.Show("Seleccione un usuario."); return; }

            var r = MessageBox.Show(this, "¿Eliminar usuario seleccionado?", "Confirmar",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (r != DialogResult.Yes) return;

            try
            {
                SetLoading(true, "Eliminando...");
                await Task.Run(() => _svc.Eliminar(id.Value));
                await CargarAsync(txtBuscar.Text.Trim());
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"Error al eliminar: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                SetLoading(false, "Error");
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                components?.Dispose();
                _loadCts?.Cancel();
                _loadCts?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}