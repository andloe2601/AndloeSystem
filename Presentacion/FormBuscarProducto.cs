using Andloe.Data;
using Andloe.Entidad;
using System;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Andloe.Presentacion
{
    public partial class FormBuscarProducto : Form
    {
        private readonly ProductoRepository _repo = new();
        private readonly FacturaRepository _facRepo = new();
        private string? _filtroInicial;

        public Producto? ProductoSeleccionado { get; private set; }

        public FormBuscarProducto()
        {
            InitializeComponent();
            WireEvents();
        }

        public void SetFiltroInicial(string? filtro)
        {
            _filtroInicial = (filtro ?? "").Trim();
        }

        private void WireEvents()
        {
            Shown += (_, __) =>
            {
                if (!string.IsNullOrWhiteSpace(_filtroInicial))
                    txtBuscar.Text = _filtroInicial;

                txtBuscar.Focus();
                txtBuscar.SelectionStart = txtBuscar.TextLength;
                Cargar();
            };

            btnBuscar.Click += (_, __) => Cargar();

            txtBuscar.KeyDown += (_, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    Cargar();
                }
            };

            grid.DoubleClick += (_, __) => SeleccionarActual();

            grid.KeyDown += (_, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    SeleccionarActual();
                }
            };

            btnAceptar.Click += (_, __) => SeleccionarActual();

            btnCancelar.Click += (_, __) =>
            {
                DialogResult = DialogResult.Cancel;
                Close();
            };
        }

        private void Cargar()
        {
            try
            {
                var filtro = (txtBuscar.Text ?? "").Trim();
                var data = _repo.Listar(filtro, 300);

                grid.Rows.Clear();

                foreach (var p in data)
                {
                    var codigo = (p.Codigo ?? "").Trim();

                    var codBarra = "";
                    try
                    {
                        if (!string.IsNullOrWhiteSpace(codigo))
                            codBarra = _facRepo.ObtenerPrimerCodigoBarra(codigo) ?? "";
                    }
                    catch { }

                    var unidad = (p.UnidadMedidaCodigo ?? "").Trim();
                    if (string.IsNullOrWhiteSpace(unidad))
                        unidad = (p.UnidadBase ?? "").Trim();
                    if (string.IsNullOrWhiteSpace(unidad))
                        unidad = _repo.ObtenerUnidadPorCodigo(codigo, "UND") ?? "UND";

                    grid.Rows.Add(
                        codigo,
                        codBarra,
                        p.Descripcion ?? "",
                        unidad,
                        p.PrecioVenta
                    );
                }

                lblTotal.Text = $"Total: {data.Count}";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Buscar producto", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SeleccionarActual()
        {
            try
            {
                if (grid.CurrentRow == null) return;

                var codigo = Convert.ToString(grid.CurrentRow.Cells["colCodigo"].Value)?.Trim();
                var barra = Convert.ToString(grid.CurrentRow.Cells["colCodBarra"].Value)?.Trim();

                var lookup = !string.IsNullOrWhiteSpace(codigo) ? codigo : barra;
                if (string.IsNullOrWhiteSpace(lookup)) return;

                var prod = _repo.ObtenerPorCodigoOBarras(lookup);
                if (prod == null)
                {
                    MessageBox.Show(
                        "No se pudo cargar el producto seleccionado.",
                        "Buscar producto",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                ProductoSeleccionado = prod;
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Seleccionar producto", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}