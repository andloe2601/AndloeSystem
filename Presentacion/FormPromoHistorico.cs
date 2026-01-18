using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Andloe.Data;

namespace Presentation
{
    public partial class FormPromoHistorico : Form
    {
        private readonly PromoRepository _promoRepo = new();
        private readonly string _usuarioActual;


        // Estados válidos en PromoCab.Estado
        private const string ESTADO_ACTIVO = "ACTIVA";
        private const string ESTADO_INACTIVO = "PAUSADA";

        // GRID de detalle creado por código
        private DataGridView dgvDetalles;

        public FormPromoHistorico(string usuarioActual)
        {
            _usuarioActual = usuarioActual;
            InitializeComponent();

            // cuando cambia la selección del grid principal
            dgvPromos.SelectionChanged += dgvPromos_SelectionChanged;

            CrearYConfigurarGridDetalle();
            AplicarEstiloModerno();
        }

        private void FormPromoHistorico_Load(object sender, EventArgs e)
        {
            CargarPromos();
        }

        // ==========================
        //  GRID DE DETALLE
        // ==========================
        private void CrearYConfigurarGridDetalle()
        {
            dgvDetalles = new DataGridView();
            dgvDetalles.Name = "dgvDetalles";
            dgvDetalles.ReadOnly = true;
            dgvDetalles.AllowUserToAddRows = false;
            dgvDetalles.AllowUserToDeleteRows = false;
            dgvDetalles.RowHeadersVisible = false;
            dgvDetalles.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvDetalles.MultiSelect = false;
            dgvDetalles.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // Lo ponemos justo debajo de dgvPromos
            dgvDetalles.Location = new Point(
                dgvPromos.Left,
                dgvPromos.Bottom + 6
            );

            dgvDetalles.Size = new Size(
                dgvPromos.Width,
                this.ClientSize.Height - (dgvPromos.Bottom + 12)
            );

            dgvDetalles.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom | AnchorStyles.Top;

            // Columnas
            dgvDetalles.AutoGenerateColumns = false;
            dgvDetalles.Columns.Clear();

            dgvDetalles.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colDetCodigo",
                HeaderText = "Código",
                DataPropertyName = "CodigoProducto",
                Width = 80
            });

            dgvDetalles.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colDetNombre",
                HeaderText = "Producto",
                DataPropertyName = "NombreProducto",
                Width = 200
            });

            dgvDetalles.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colDetTipoRegla",
                HeaderText = "Tipo regla",
                DataPropertyName = "TipoRegla",
                Width = 100
            });

            dgvDetalles.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colDetDescPct",
                HeaderText = "% Desc.",
                DataPropertyName = "DescuentoPct",
                DefaultCellStyle = { Format = "N2" },
                Width = 80
            });

            dgvDetalles.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colDetPrecioFijo",
                HeaderText = "Precio fijo",
                DataPropertyName = "PrecioFijo",
                DefaultCellStyle = { Format = "N2" },
                Width = 90
            });

            dgvDetalles.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colDetPackCantidad",
                HeaderText = "Cant. pack",
                DataPropertyName = "PackCantidad",
                DefaultCellStyle = { Format = "N0" },
                Width = 80
            });

            dgvDetalles.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colDetPackPrecio",
                HeaderText = "Precio pack",
                DataPropertyName = "PackPrecio",
                DefaultCellStyle = { Format = "N2" },
                Width = 90
            });

            this.Controls.Add(dgvDetalles);
            dgvDetalles.BringToFront();
        }

        private void AplicarEstiloModerno()
        {
            // Estilo para el grid de cabecera
            dgvPromos.EnableHeadersVisualStyles = false;
            dgvPromos.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(45, 45, 48);
            dgvPromos.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvPromos.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);

            dgvPromos.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 248, 255);
            dgvPromos.DefaultCellStyle.SelectionBackColor = Color.FromArgb(51, 153, 255);
            dgvPromos.DefaultCellStyle.SelectionForeColor = Color.White;

            // Estilo para el grid de detalle
            dgvDetalles.EnableHeadersVisualStyles = false;
            dgvDetalles.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(28, 28, 28);
            dgvDetalles.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvDetalles.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);

            dgvDetalles.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(250, 250, 252);
            dgvDetalles.DefaultCellStyle.SelectionBackColor = Color.FromArgb(76, 175, 80);
            dgvDetalles.DefaultCellStyle.SelectionForeColor = Color.White;
        }

        // ==========================
        //  CARGA DE PROMOS
        // ==========================
        private void CargarPromos()
        {
            try
            {
                var texto = txtBuscar.Text.Trim();
                bool soloActivas = chkSoloActivas.Checked;

                var lista = _promoRepo.ListarHistoricoPromos(texto, soloActivas);

                dgvPromos.AutoGenerateColumns = false;
                dgvPromos.DataSource = null;
                dgvPromos.DataSource = lista;

                dgvPromos.Columns["colFechaInicio"].DefaultCellStyle.Format = "dd/MM/yyyy";
                dgvPromos.Columns["colFechaFin"].DefaultCellStyle.Format = "dd/MM/yyyy";
                dgvPromos.Columns["colFechaCreacion"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";

                ActualizarBotonDesactivar();
                CargarDetalleSeleccionado();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar promociones: " + ex.Message,
                    "Promos", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarDetalleSeleccionado()
        {
            var sel = ObtenerSeleccionado();
            if (sel == null)
            {
                dgvDetalles.DataSource = null;
                return;
            }

            try
            {
                var detalle = _promoRepo.ListarDetallePromo(sel.PromoId);
                dgvDetalles.DataSource = null;
                dgvDetalles.DataSource = detalle;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar detalle de la promoción: " + ex.Message,
                    "Promos", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private PromoHistoricoRow? ObtenerSeleccionado()
        {
            if (dgvPromos.CurrentRow == null)
                return null;

            return dgvPromos.CurrentRow.DataBoundItem as PromoHistoricoRow;
        }

        // ==========================
        //  BUSCAR / FILTROS
        // ==========================
        private void btnBuscar_Click(object sender, EventArgs e)
        {
            CargarPromos();
        }

        private void txtBuscar_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                CargarPromos();
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void chkSoloActivas_CheckedChanged(object sender, EventArgs e)
        {
            CargarPromos();
        }

        // ==========================
        //  NUEVO
        // ==========================
        private void btnNuevo_Click(object sender, EventArgs e)
        {
            try
            {
                using (var frm = new FormPromoProductoDescuento(_usuarioActual))
                {
                    var dr = frm.ShowDialog(this);
                    if (dr == DialogResult.OK)
                    {
                        CargarPromos();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al crear promoción: " + ex.Message,
                    "Promos", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ==========================
        //  EDITAR
        // ==========================
        private void btnEditar_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvPromos.CurrentRow == null)
                {
                    MessageBox.Show("Seleccione una promoción primero.",
                        "Promos", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var promoIdObj = dgvPromos.CurrentRow.Cells["colPromoId"].Value;
                if (promoIdObj == null || promoIdObj == DBNull.Value)
                {
                    MessageBox.Show("No se pudo obtener el Id de la promoción.",
                        "Promos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int promoId = Convert.ToInt32(promoIdObj);

                using (var frm = new FormPromoProductoDescuento(_usuarioActual, promoId))
                {
                    var dr = frm.ShowDialog(this);

                    if (dr == DialogResult.OK)
                    {
                        CargarPromos();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al editar promoción: " + ex.Message,
                    "Promos", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvPromos_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
                btnEditar_Click(sender, EventArgs.Empty);
        }

        // ==========================
        //  ACTIVAR / DESACTIVAR
        // ==========================
        private void btnDesactivar_Click(object sender, EventArgs e)
        {
            var sel = ObtenerSeleccionado();
            if (sel == null)
            {
                MessageBox.Show("Seleccione una promoción.", "Promos",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            bool estaActiva = sel.Estado.Equals(ESTADO_ACTIVO, StringComparison.OrdinalIgnoreCase);
            string nuevoEstado = estaActiva ? ESTADO_INACTIVO : ESTADO_ACTIVO;
            string accion = estaActiva ? "desactivar" : "activar";

            var r = MessageBox.Show(
                $"¿Seguro que deseas {accion} la promoción?\n\n" +
                $"{sel.Codigo} - {sel.Nombre}",
                $"Confirmar {accion}",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (r != DialogResult.Yes)
                return;

            try
            {
                _promoRepo.CambiarEstadoPromo(sel.PromoId, nuevoEstado);
                CargarPromos();
                ActualizarBotonDesactivar();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al {accion} promoción: " + ex.Message,
                    "Promos", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ActualizarBotonDesactivar()
        {
            var sel = ObtenerSeleccionado();

            if (sel == null)
            {
                btnDesactivar.Enabled = false;
                btnDesactivar.Text = "Desactivar";
                btnDesactivar.BackColor = SystemColors.Control;
                btnDesactivar.ForeColor = SystemColors.ControlText;
                return;
            }

            btnDesactivar.Enabled = true;

            bool estaActiva = sel.Estado.Equals(ESTADO_ACTIVO, StringComparison.OrdinalIgnoreCase);

            if (estaActiva)
            {
                btnDesactivar.Text = "Desactivar";
                btnDesactivar.BackColor = Color.Green;
                btnDesactivar.ForeColor = Color.White;
            }
            else
            {
                btnDesactivar.Text = "Activar";
                btnDesactivar.BackColor = Color.Red;
                btnDesactivar.ForeColor = Color.White;
            }
        }

        private void dgvPromos_SelectionChanged(object sender, EventArgs e)
        {
            ActualizarBotonDesactivar();
            CargarDetalleSeleccionado();
        }

        // ==========================
        //  CERRAR
        // ==========================
        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
