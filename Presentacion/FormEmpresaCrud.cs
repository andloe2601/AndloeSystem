using Andloe.Data;
using Andloe.Entidad;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Andloe.Presentacion
{
    public partial class FormEmpresaCrud : Form
    {
        private readonly EmpresaRepository _repo = new();
        private readonly ProvinciaRepository _provRepo = new();
        private readonly MunicipioRepository _munRepo = new();

        private List<Empresa> _empresas = new();
        private byte[]? _logoBytes;
        private bool _loading = false;

        public FormEmpresaCrud()
        {
            InitializeComponent();

            AcceptButton = btnGuardar;
            CancelButton = btnCerrar;

            Load += FormEmpresaCrud_Load;
            dgvEmpresas.SelectionChanged += dgvEmpresas_SelectionChanged;
            cbProvincia.SelectedIndexChanged += cbProvincia_SelectedIndexChanged;

            btnNuevo.Click += btnNuevo_Click;
            btnGuardar.Click += btnGuardar_Click;
            btnEliminar.Click += btnEliminar_Click;
            btnCerrar.Click += btnCerrar_Click;
            btnCargarLogo.Click += btnCargarLogo_Click;
            btnQuitarLogo.Click += btnQuitarLogo_Click;
        }

        private void FormEmpresaCrud_Load(object? sender, EventArgs e)
        {
            CargarProvincias();
            CargarEmpresas();
            LimpiarFormulario();
        }

        private void CargarEmpresas()
        {
            _empresas = _repo.Listar();

            dgvEmpresas.AutoGenerateColumns = false;
            dgvEmpresas.DataSource = null;
            dgvEmpresas.DataSource = _empresas
                .Select(x => new
                {
                    x.EmpresaId,
                    x.RazonSocial,
                    x.RNC,
                    x.Telefono,
                    x.Email,
                    Estado = x.Estado ? "Activo" : "Inactivo"
                })
                .ToList();
        }

        private void CargarProvincias()
        {
            _loading = true;
            try
            {
                cbProvincia.DisplayMember = "Nombre";
                cbProvincia.ValueMember = "ProvinciaId";
                cbProvincia.DataSource = _provRepo.Listar();
                cbMunicipio.DataSource = null;
            }
            finally
            {
                _loading = false;
            }
        }

        private void CargarMunicipios(int provinciaId)
        {
            cbMunicipio.DisplayMember = "Nombre";
            cbMunicipio.ValueMember = "MunicipioId";
            cbMunicipio.DataSource = _munRepo.ListarPorProvincia(provinciaId.ToString());
        }

        private void cbProvincia_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (_loading) return;
            if (cbProvincia.SelectedValue == null) return;
            if (!int.TryParse(cbProvincia.SelectedValue.ToString(), out var provinciaId)) return;

            _loading = true;
            try
            {
                CargarMunicipios(provinciaId);
            }
            finally
            {
                _loading = false;
            }
        }

        private void dgvEmpresas_SelectionChanged(object? sender, EventArgs e)
        {
            if (_loading) return;
            if (dgvEmpresas.CurrentRow == null) return;
            if (dgvEmpresas.CurrentRow.Cells["colEmpresaId"].Value == null) return;

            var empresaId = Convert.ToInt32(dgvEmpresas.CurrentRow.Cells["colEmpresaId"].Value);
            var empresa = _repo.ObtenerPorId(empresaId);
            if (empresa == null) return;

            CargarFormulario(empresa);
        }

        private void CargarFormulario(Empresa e)
        {
            _loading = true;
            try
            {
                txtEmpresaId.Text = e.EmpresaId.ToString();
                txtRazonSocial.Text = e.RazonSocial;
                txtRNC.Text = e.RNC;
                txtMonedaBase.Text = string.IsNullOrWhiteSpace(e.MonedaBaseCodigo) ? "DOP" : e.MonedaBaseCodigo;
                txtPais.Text = string.IsNullOrWhiteSpace(e.Pais) ? "DO" : e.Pais!;
                txtDireccion.Text = e.Direccion ?? "";
                txtTelefono.Text = e.Telefono ?? "";
                txtEmail.Text = e.Email ?? "";
                chkEstado.Checked = e.Estado;

                _logoBytes = e.Logo;
                CargarLogoEnPreview(_logoBytes);

                if (e.MunicipioId.HasValue && e.MunicipioId.Value > 0)
                {
                    var ub = _munRepo.ObtenerUbicacion(e.MunicipioId.Value);
                    if (ub.HasValue)
                    {
                        cbProvincia.SelectedValue = ub.Value.ProvinciaId;
                        CargarMunicipios(ub.Value.ProvinciaId);
                        cbMunicipio.SelectedValue = e.MunicipioId.Value;
                    }
                }
                else
                {
                    if (cbProvincia.SelectedValue != null && int.TryParse(cbProvincia.SelectedValue.ToString(), out var provinciaId))
                        CargarMunicipios(provinciaId);
                }
            }
            finally
            {
                _loading = false;
            }
        }

        private void LimpiarFormulario()
        {
            _loading = true;
            try
            {
                txtEmpresaId.Text = "";
                txtRazonSocial.Text = "";
                txtRNC.Text = "";
                txtMonedaBase.Text = "DOP";
                txtPais.Text = "DO";
                txtDireccion.Text = "";
                txtTelefono.Text = "";
                txtEmail.Text = "";
                chkEstado.Checked = true;

                _logoBytes = null;
                CargarLogoEnPreview(null);

                if (cbProvincia.Items.Count > 0)
                    cbProvincia.SelectedIndex = 0;

                if (cbProvincia.SelectedValue != null && int.TryParse(cbProvincia.SelectedValue.ToString(), out var provinciaId))
                    CargarMunicipios(provinciaId);
                else
                    cbMunicipio.DataSource = null;
            }
            finally
            {
                _loading = false;
            }
        }

        private Empresa ConstruirEntidadDesdeUI()
        {
            if (string.IsNullOrWhiteSpace(txtRazonSocial.Text))
                throw new Exception("La Razón Social es obligatoria.");

            if (string.IsNullOrWhiteSpace(txtRNC.Text))
                throw new Exception("El RNC es obligatorio.");

            if (cbMunicipio.SelectedValue == null || !int.TryParse(cbMunicipio.SelectedValue.ToString(), out var municipioId))
                throw new Exception("Debes seleccionar un municipio.");

            var ub = _munRepo.ObtenerUbicacion(municipioId);
            if (!ub.HasValue)
                throw new Exception("No se pudo resolver la ubicación seleccionada.");

            return new Empresa
            {
                EmpresaId = int.TryParse(txtEmpresaId.Text, out var id) ? id : 0,
                RazonSocial = txtRazonSocial.Text.Trim(),
                RNC = txtRNC.Text.Trim(),
                MonedaBaseCodigo = string.IsNullOrWhiteSpace(txtMonedaBase.Text) ? "DOP" : txtMonedaBase.Text.Trim(),
                Pais = string.IsNullOrWhiteSpace(txtPais.Text) ? "DO" : txtPais.Text.Trim(),
                Direccion = string.IsNullOrWhiteSpace(txtDireccion.Text) ? null : txtDireccion.Text.Trim(),
                Telefono = string.IsNullOrWhiteSpace(txtTelefono.Text) ? null : txtTelefono.Text.Trim(),
                Email = string.IsNullOrWhiteSpace(txtEmail.Text) ? null : txtEmail.Text.Trim(),
                Estado = chkEstado.Checked,
                Logo = _logoBytes,
                MunicipioId = municipioId,
                Provincia = ub.Value.Provincia,
                Ciudad = ub.Value.Municipio
            };
        }

        private void btnNuevo_Click(object? sender, EventArgs e)
        {
            LimpiarFormulario();
        }

        private void btnGuardar_Click(object? sender, EventArgs e)
        {
            try
            {
                var empresa = ConstruirEntidadDesdeUI();

                if (empresa.EmpresaId <= 0)
                    empresa.EmpresaId = _repo.Insertar(empresa);
                else
                    _repo.Actualizar(empresa);

                MessageBox.Show(
                    "Empresa guardada correctamente.",
                    "Empresas",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                CargarEmpresas();
                SeleccionarFila(empresa.EmpresaId);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error guardando empresa: " + ex.Message,
                    "Empresas",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void btnEliminar_Click(object? sender, EventArgs e)
        {
            try
            {
                if (!int.TryParse(txtEmpresaId.Text, out var empresaId) || empresaId <= 0)
                {
                    MessageBox.Show(
                        "Debes seleccionar una empresa.",
                        "Empresas",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                var r = MessageBox.Show(
                    "¿Eliminar esta empresa?",
                    "Empresas",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (r != DialogResult.Yes) return;

                _repo.Eliminar(empresaId);

                MessageBox.Show(
                    "Empresa eliminada.",
                    "Empresas",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                CargarEmpresas();
                LimpiarFormulario();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error eliminando empresa: " + ex.Message,
                    "Empresas",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void btnCerrar_Click(object? sender, EventArgs e)
        {
            Close();
        }

        private void btnCargarLogo_Click(object? sender, EventArgs e)
        {
            using var ofd = new OpenFileDialog
            {
                Title = "Selecciona logo",
                Filter = "Imágenes (*.png;*.jpg;*.jpeg;*.bmp)|*.png;*.jpg;*.jpeg;*.bmp",
                Multiselect = false
            };

            if (ofd.ShowDialog() != DialogResult.OK) return;

            _logoBytes = File.ReadAllBytes(ofd.FileName);
            CargarLogoEnPreview(_logoBytes);
        }

        private void btnQuitarLogo_Click(object? sender, EventArgs e)
        {
            _logoBytes = null;
            CargarLogoEnPreview(null);
        }

        private void CargarLogoEnPreview(byte[]? bytes)
        {
            var old = picLogo.Image;
            picLogo.Image = null;
            old?.Dispose();

            if (bytes == null || bytes.Length == 0) return;

            using var ms = new MemoryStream(bytes);
            using var img = Image.FromStream(ms);
            picLogo.Image = new Bitmap(img);
            picLogo.SizeMode = PictureBoxSizeMode.Zoom;
        }

        private void SeleccionarFila(int empresaId)
        {
            foreach (DataGridViewRow row in dgvEmpresas.Rows)
            {
                if (row.Cells["colEmpresaId"].Value == null) continue;

                if (Convert.ToInt32(row.Cells["colEmpresaId"].Value) == empresaId)
                {
                    row.Selected = true;
                    dgvEmpresas.CurrentCell = row.Cells["colRazonSocial"];
                    break;
                }
            }
        }
    }
}