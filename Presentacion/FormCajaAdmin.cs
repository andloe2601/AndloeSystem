using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Andloe.Logica;
using Andloe.Data;

namespace Andloe.Presentacion
{
    public partial class FormCajaAdmin : Form
    {
        private readonly CajaAdminService _srv = new();
        private readonly SucursalRepository _sucRepo = new();

        private List<CajaDto> _listaActual = new();

        // Para saber si estamos editando o creando nueva
        private int? _cajaIdActual = null;

        public FormCajaAdmin()
        {
            InitializeComponent();
        }

        private void FormCajaAdmin_Load(object sender, EventArgs e)
        {
            try
            {
               // CargarSucursales();
                ConfigurarComboEstado();
                CargarCajasSucursalActual();
                LimpiarFormulario();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar formulario de cajas: " + ex.Message,
                    "Cajas", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ==============================
        //   CARGA DE COMBOS / GRILLA
        // ==============================

      

        private void ConfigurarComboEstado()
        {
            cbEstado.Items.Clear();
            cbEstado.Items.Add("ACTIVA");
            cbEstado.Items.Add("INACTIVA");
            cbEstado.SelectedIndex = 0;
        }

        private void CargarCajasSucursalActual()
        {
            if (cbSucursal.SelectedValue is not int sucId || sucId <= 0)
            {
                _listaActual = new List<CajaDto>();
                gridCajas.DataSource = null;
                return;
            }

            // Usa el servicio nuevo (sin parámetro soloActivas)
            _listaActual = _srv.ListarPorSucursal(sucId) ?? new List<CajaDto>();

            gridCajas.AutoGenerateColumns = false;
            gridCajas.DataSource = null;
            gridCajas.DataSource = _listaActual;
        }

        /// <summary>
        /// Selecciona en el grid la fila que tenga ese CajaId.
        /// </summary>
        private void SeleccionarCajaEnGrid(int cajaId)
        {
            if (cajaId <= 0 || gridCajas.Rows.Count == 0)
                return;

            foreach (DataGridViewRow row in gridCajas.Rows)
            {
                if (row.DataBoundItem is CajaDto dto && dto.CajaId == cajaId)
                {
                    row.Selected = true;
                    gridCajas.CurrentCell = row.Cells[0];
                    break;
                }
            }
        }

        // ==============================
        //   FORMULARIO (CRUD)
        // ==============================

        private void LimpiarFormulario()
        {
            _cajaIdActual = null;
            txtCajaId.Text = "";
            txtCajaNumero.Clear();
            txtDescripcion.Clear();

            // <-- proteger el SelectedIndex
            if (cbEstado.Items.Count > 0)
                cbEstado.SelectedIndex = 0;

            txtCajaNumero.Focus();
        }

        private void LlenarFormularioDesdeGrid()
        {
            if (gridCajas.CurrentRow == null)
                return;

            var row = gridCajas.CurrentRow;

            if (row.DataBoundItem is CajaDto dto)
            {
                _cajaIdActual = dto.CajaId;
                txtCajaId.Text = dto.CajaId.ToString();
                txtCajaNumero.Text = dto.CajaNumero;
                txtDescripcion.Text = dto.Descripcion ?? "";

                var estado = dto.Estado ?? "ACTIVA";
                var idx = cbEstado.FindStringExact(estado.ToUpper());

                if (idx >= 0)
                    cbEstado.SelectedIndex = idx;
                else if (cbEstado.Items.Count > 0)
                    cbEstado.SelectedIndex = 0;
            }
            else
            {
                _cajaIdActual = Convert.ToInt32(row.Cells["colCajaId"].Value);
                txtCajaId.Text = _cajaIdActual.ToString();

                txtCajaNumero.Text = row.Cells["colCajaNumero"].Value?.ToString() ?? "";
                txtDescripcion.Text = row.Cells["colDescripcion"].Value?.ToString() ?? "";
                var estado = row.Cells["colEstado"].Value?.ToString() ?? "ACTIVA";

                var idx = cbEstado.FindStringExact(estado.ToUpper());
                if (idx >= 0)
                    cbEstado.SelectedIndex = idx;
                else if (cbEstado.Items.Count > 0)
                    cbEstado.SelectedIndex = 0;
            }
        }

        // ==============================
        //   EVENTOS UI
        // ==============================

        private void cbSucursal_SelectedIndexChanged(object sender, EventArgs e)
        {
            CargarCajasSucursalActual();
            LimpiarFormulario();
        }

        private void gridCajas_SelectionChanged(object sender, EventArgs e)
        {
            if (gridCajas.Focused)
            {
                LlenarFormularioDesdeGrid();
            }
        }

        private void gridCajas_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                gridCajas.CurrentCell = gridCajas.Rows[e.RowIndex].Cells[e.ColumnIndex];
                LlenarFormularioDesdeGrid();
            }
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            LimpiarFormulario();
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                if (cbSucursal.SelectedValue is not int sucursalId || sucursalId <= 0)
                {
                    MessageBox.Show("Seleccione una sucursal válida.", "Cajas",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    cbSucursal.Focus();
                    return;
                }

                var cajaNumero = txtCajaNumero.Text.Trim();
                var descripcion = txtDescripcion.Text.Trim();
                var estado = cbEstado.SelectedItem?.ToString() ?? "ACTIVA";

                if (string.IsNullOrWhiteSpace(cajaNumero))
                {
                    MessageBox.Show("Debe digitar el número de caja.", "Cajas",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtCajaNumero.Focus();
                    return;
                }

                int id;

                // NUEVA CAJA
                if (_cajaIdActual == null)
                {
                    id = _srv.CrearCaja(
                        sucursalId: sucursalId,
                        cajaNumero: cajaNumero,
                        descripcion: descripcion,
                        estado: estado);

                    MessageBox.Show($"Caja creada correctamente. ID: {id}",
                        "Cajas", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    // EDICIÓN
                    id = _cajaIdActual.Value;

                    _srv.ActualizarCaja(
                        cajaId: id,
                        sucursalId: sucursalId,
                        cajaNumero: cajaNumero,
                        descripcion: descripcion,
                        estado: estado);

                    MessageBox.Show("Caja actualizada correctamente.",
                        "Cajas", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                // 🔄 Recargar grid y seleccionar la caja recién creada/actualizada
                CargarCajasSucursalActual();
                SeleccionarCajaEnGrid(id);
                _cajaIdActual = id; // seguimos en modo edición de esa caja
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
                if (_cajaIdActual == null || _cajaIdActual <= 0)
                {
                    MessageBox.Show("Seleccione una caja para eliminar.", "Cajas",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var r = MessageBox.Show(
                    "¿Seguro que desea eliminar esta caja?\n" +
                    "Asegúrese de que no tenga movimientos relacionados.",
                    "Cajas", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (r != DialogResult.Yes)
                    return;

                _srv.EliminarCaja(_cajaIdActual.Value);

                MessageBox.Show("Caja eliminada correctamente.",
                    "Cajas", MessageBoxButtons.OK, MessageBoxIcon.Information);

                CargarCajasSucursalActual();
                LimpiarFormulario();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar caja: " + ex.Message,
                    "Cajas", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
