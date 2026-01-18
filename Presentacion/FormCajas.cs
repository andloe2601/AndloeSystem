// Proyecto: Presentation
using Andloe.Data;
using Andloe.Entidad;
using Andloe.Logica;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Presentation
{
    public partial class FormCajas : Form
    {
        private readonly CajaAdminService _srv = new();

        public FormCajas()
        {
            InitializeComponent();
        }

        private void FormCajas_Load(object sender, EventArgs e)
        {
            try
            {
                CargarEstadosCombo();
                CargarSucursales();
                CargarCajas();
                LimpiarFormulario();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al iniciar formulario de cajas: " + ex.Message,
                    "Cajas", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarEstadosCombo()
        {
            // Estados fijos: ACTIVA / INACTIVA
            cbEstado.Items.Clear();
            cbEstado.Items.Add("ACTIVA");
            cbEstado.Items.Add("INACTIVA");
            cbEstado.SelectedIndex = 0;
        }

        // TODO: reemplaza esto por tu propio repositorio de sucursales
        private class SucursalItem
        {
            public int SucursalId { get; set; }
            public string Nombre { get; set; } = string.Empty;
        }

        private void CargarSucursales()
        {
            // 🔹 Ejemplo HARDCODEADO, cámbialo por consulta a tu tabla Sucursal.
            var sucursales = new List<SucursalItem>
            {
                new SucursalItem { SucursalId = 1, Nombre = "Sucursal 1" },
                new SucursalItem { SucursalId = 2, Nombre = "Sucursal 2" }
            };

            // Combo filtro
            cbSucursalFiltro.DisplayMember = "Nombre";
            cbSucursalFiltro.ValueMember = "SucursalId";
            cbSucursalFiltro.DataSource = sucursales.ToList();

            // Combo edición
            cbSucursal.DisplayMember = "Nombre";
            cbSucursal.ValueMember = "SucursalId";
            cbSucursal.DataSource = sucursales.ToList();
        }

        private void CargarCajas()
        {
            int? sucursalId = null;
            if (cbSucursalFiltro.SelectedValue is int id && id > 0)
                sucursalId = id;

            // Corregido: ListarPorSucursal espera un int, no un int?
            var cajas = _srv.ListarPorSucursal(sucursalId ?? 0);

            gridCajas.DataSource = null;
            gridCajas.DataSource = cajas;

            // Ocultar columnas si hace falta
            gridCajas.Columns[nameof(CajaDto.Descripcion)].HeaderText = "Descripción";
            gridCajas.Columns[nameof(CajaDto.CajaNumero)].HeaderText = "Caja";
            gridCajas.Columns[nameof(CajaDto.Estado)].HeaderText = "Estado";
        }

        private void LimpiarFormulario()
        {
            txtCajaId.Text = "";
            txtCajaNumero.Text = "";
            txtDescripcion.Text = "";
            cbEstado.SelectedIndex = 0;

            if (cbSucursalFiltro.SelectedValue is int sucId && sucId > 0)
                cbSucursal.SelectedValue = sucId;
        }

        private void cbSucursalFiltro_SelectedIndexChanged(object sender, EventArgs e)
        {
            CargarCajas();
        }

        private void gridCajas_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var row = gridCajas.Rows[e.RowIndex];
            if (row.DataBoundItem is CajaDto dto)
            {
                txtCajaId.Text = dto.CajaId.ToString();
                txtCajaNumero.Text = dto.CajaNumero;
                txtDescripcion.Text = dto.Descripcion;
                cbEstado.SelectedItem = dto.Estado;

                cbSucursal.SelectedValue = dto.SucursalId;
            }
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            LimpiarFormulario();
            txtCajaNumero.Focus();
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                if (!(cbSucursal.SelectedValue is int sucursalId) || sucursalId <= 0)
                {
                    MessageBox.Show("Seleccione una sucursal.", "Cajas",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var cajaNumero = txtCajaNumero.Text.Trim();
                var descripcion = txtDescripcion.Text.Trim();
                var estado = cbEstado.SelectedItem?.ToString() ?? "ACTIVA";

                if (string.IsNullOrWhiteSpace(cajaNumero))
                {
                    MessageBox.Show("El número de caja es obligatorio.", "Cajas",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtCajaNumero.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtCajaId.Text))
                {
                    // Crear
                    var idNuevo = _srv.CrearCaja(sucursalId, cajaNumero, descripcion, estado);
                    MessageBox.Show($"Caja creada con Id {idNuevo}.", "Cajas",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    // Actualizar
                    int cajaId = int.Parse(txtCajaId.Text);
                    _srv.ActualizarCaja(cajaId, sucursalId, cajaNumero, descripcion, estado);
                    MessageBox.Show("Caja actualizada correctamente.", "Cajas",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                CargarCajas();
                LimpiarFormulario();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar caja: " + ex.Message,
                    "Cajas", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtCajaId.Text))
                {
                    MessageBox.Show("Seleccione una caja primero.", "Cajas",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int cajaId = int.Parse(txtCajaId.Text);

                var r = MessageBox.Show("¿Seguro que desea eliminar esta caja?",
                    "Cajas", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (r != DialogResult.Yes) return;

                _srv.EliminarCaja(cajaId);

                MessageBox.Show("Caja eliminada correctamente.", "Cajas",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                CargarCajas();
                LimpiarFormulario();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar caja: " + ex.Message,
                    "Cajas", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            LimpiarFormulario();
        }
    }
}
