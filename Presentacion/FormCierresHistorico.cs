using System;
using System.Windows.Forms;
using Andloe.Logica;
using Andloe.Entidad;

namespace Presentation
{
    public partial class FormCierresHistorico : Form
    {
        private readonly CierreCajaService _service = new();

        public FormCierresHistorico()
        {
            InitializeComponent();
        }

        private void FormCierresHistorico_Load(object? sender, EventArgs e)
        {
            try
            {
                // Valores por defecto (solo para que se vean en la UI)
                dtDesde.Value = DateTime.Today.AddDays(-7);
                dtHasta.Value = DateTime.Today;

                // ❗ Clave: NO aplicar filtro de fecha al inicio
                dtDesde.Checked = false;
                dtHasta.Checked = false;

                // Estado: vacío = todos
                cboEstado.Items.Clear();
                cboEstado.Items.Add(""); // Todos
                cboEstado.Items.Add("ABIERTO");
                cboEstado.Items.Add("CERRADO");
                cboEstado.Items.Add("ANULADO");
                cboEstado.SelectedIndex = 0;

                ConfigurarGrid();
                CargarCierres();   // 👉 esto ahora llama al service con fechas = null
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar histórico de cierres: " + ex.Message,
                    "Histórico de Cierres", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvCierres_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
        {
           
            try
            {
                if (e.RowIndex < 0) return; // encabezado

                var row = dgvCierres.Rows[e.RowIndex];

                // Tomamos el valor de la columna CierreId
                var valor = row.Cells["CierreId"].Value;
                //MessageBox.Show("Valor CierreId = " + (valor?.ToString() ?? "NULL"));

                if (valor == null || valor == DBNull.Value)
                {
                    MessageBox.Show("No se pudo obtener el ID del cierre.",
                        "Histórico de Cierres", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!long.TryParse(valor.ToString(), out long cierreId))
                {
                    MessageBox.Show("ID de cierre inválido.",
                        "Histórico de Cierres", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Abrimos el detalle
                using (var frm = new FormCierreDetalle(cierreId))
                {
                    frm.ShowDialog(this);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al abrir el detalle del cierre: " + ex.Message,
                    "Histórico de Cierres", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ConfigurarGrid()
        {
            dgvCierres.AutoGenerateColumns = false;
            dgvCierres.Columns.Clear();
            dgvCierres.ReadOnly = true;
            dgvCierres.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvCierres.MultiSelect = false;
            dgvCierres.AllowUserToAddRows = false;
            dgvCierres.AllowUserToDeleteRows = false;

            // IMPORTANTE: Name = "CierreId" para usar row.Cells["CierreId"]
            dgvCierres.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "CierreId",
                DataPropertyName = "CierreId",
                HeaderText = "ID",
                Width = 60
            });

            dgvCierres.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "POS_CajaNumero",
                HeaderText = "Caja",
                Width = 80
            });

            dgvCierres.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "FechaDesde",
                HeaderText = "Desde",
                Width = 120,
                DefaultCellStyle = { Format = "dd/MM/yyyy HH:mm" }
            });

            dgvCierres.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "FechaHasta",
                HeaderText = "Hasta",
                Width = 120,
                DefaultCellStyle = { Format = "dd/MM/yyyy HH:mm" }
            });

            dgvCierres.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "FondoInicial",
                HeaderText = "Fondo",
                Width = 90,
                DefaultCellStyle = { Format = "N2", Alignment = DataGridViewContentAlignment.MiddleRight }
            });

            dgvCierres.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "TotalVentas",
                HeaderText = "Ventas",
                Width = 90,
                DefaultCellStyle = { Format = "N2", Alignment = DataGridViewContentAlignment.MiddleRight }
            });

            dgvCierres.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "TotalPagos",
                HeaderText = "Pagos",
                Width = 90,
                DefaultCellStyle = { Format = "N2", Alignment = DataGridViewContentAlignment.MiddleRight }
            });

            dgvCierres.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "EfectivoTeorico",
                HeaderText = "Efectivo Teórico",
                Width = 110,
                DefaultCellStyle = { Format = "N2", Alignment = DataGridViewContentAlignment.MiddleRight }
            });

            dgvCierres.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "EfectivoDeclarado",
                HeaderText = "Efectivo Declarado",
                Width = 120,
                DefaultCellStyle = { Format = "N2", Alignment = DataGridViewContentAlignment.MiddleRight }
            });

            dgvCierres.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Diferencia",
                HeaderText = "Diferencia",
                Width = 90,
                DefaultCellStyle = { Format = "N2", Alignment = DataGridViewContentAlignment.MiddleRight }
            });

            dgvCierres.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "UsuarioCierre",
                HeaderText = "Usuario",
                Width = 100
            });

            dgvCierres.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "FechaCierre",
                HeaderText = "Fecha Cierre",
                Width = 130,
                DefaultCellStyle = { Format = "dd/MM/yyyy HH:mm" }
            });

            dgvCierres.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Estado",
                HeaderText = "Estado",
                Width = 80
            });
        }

        private void CargarCierres()
        {
            try
            {
                DateTime? desde = dtDesde.Checked ? dtDesde.Value : (DateTime?)null;
                DateTime? hasta = dtHasta.Checked ? dtHasta.Value : (DateTime?)null;

                string? cajaNumero = string.IsNullOrWhiteSpace(txtCajaNumero.Text)
                    ? null
                    : txtCajaNumero.Text.Trim();

                string? usuario = string.IsNullOrWhiteSpace(txtUsuario.Text)
                    ? null
                    : txtUsuario.Text.Trim();

                string? estado = string.IsNullOrWhiteSpace(cboEstado.Text)
                    ? null
                    : cboEstado.Text.Trim();

                var lista = _service.BuscarCierres(desde, hasta, cajaNumero, usuario, estado);
                dgvCierres.DataSource = lista;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al buscar cierres: " + ex.Message,
                    "Histórico de Cierres", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnBuscar_Click(object? sender, EventArgs e)
        {
            CargarCierres();
        }

        private void btnCerrar_Click(object? sender, EventArgs e)
        {
            Close();
        }
    }
}
