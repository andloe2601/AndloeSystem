using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Andloe.Data;
using Andloe.Entidad;
using Andloe.Logica;

namespace Andloe.Presentacion
{
    public partial class FormInvMovimiento : Form
    {
        private readonly string _usuario;
        private readonly MovimientoInventarioService _movService = new();
        private readonly ProductoRepository _productoRepo = new();

        private readonly BindingSource _bsLineas = new();
        private readonly List<ItemMovimientoDto> _lineas = new();

        // Cache del producto cargado (para no volver a consultar)
        private Producto? _productoActual;

        public FormInvMovimiento(string usuario)
        {
            _usuario = string.IsNullOrWhiteSpace(usuario) ? "SYSTEM" : usuario.Trim();
            InitializeComponent();
            Inicializar();
        }

        private void Inicializar()
        {
            Text = "Entrada / Salida de Inventario";
            lblUsuario.Text = $"Usuario: {_usuario}";

            cboTipo.Items.Clear();
            cboTipo.Items.Add("ENTRADA");
            cboTipo.Items.Add("SALIDA");
            cboTipo.SelectedIndex = 0;

            dtFecha.Value = DateTime.Now;

            // Existencia deshabilitada
            txtExistencia.ReadOnly = true;
            txtExistencia.TabStop = false;

            // Descripción/costo readonly
            txtProductoDescripcion.ReadOnly = true;
            txtCosto.ReadOnly = true;

            ConfigurarGrid();

            _bsLineas.DataSource = _lineas;
            gridLineas.DataSource = _bsLineas;

            CargarAlmacenesDemo();
            cboAlmacenDestino.Enabled = chkUsarDestino.Checked;
        }

        private void CargarAlmacenesDemo()
        {
            var almacenes = new[]
            {
                new { AlmacenId = ConfigService.AlmacenPosOrigenId > 0 ? ConfigService.AlmacenPosOrigenId : 1, Nombre = "Almacén Principal" },
                new { AlmacenId = ConfigService.AlmacenPosDestinoId > 0 ? ConfigService.AlmacenPosDestinoId : 2, Nombre = "Almacén Secundario" }
            }.ToList();

            cboAlmacenOrigen.DisplayMember = "Nombre";
            cboAlmacenOrigen.ValueMember = "AlmacenId";
            cboAlmacenOrigen.DataSource = almacenes.ToList();

            cboAlmacenDestino.DisplayMember = "Nombre";
            cboAlmacenDestino.ValueMember = "AlmacenId";
            cboAlmacenDestino.DataSource = almacenes.ToList();

            cboAlmacenOrigen.SelectedIndex = 0;
            cboAlmacenDestino.SelectedIndex = almacenes.Count > 1 ? 1 : 0;
        }

        private void ConfigurarGrid()
        {
            gridLineas.AutoGenerateColumns = false;
            gridLineas.AllowUserToAddRows = false;
            gridLineas.AllowUserToDeleteRows = false;
            gridLineas.ReadOnly = true;
            gridLineas.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            gridLineas.MultiSelect = false;

            colCodigo.DataPropertyName = nameof(ItemMovimientoDto.ProductoCodigo);
            colDescripcion.DataPropertyName = nameof(ItemMovimientoDto.Descripcion);
            colCantidad.DataPropertyName = nameof(ItemMovimientoDto.Cantidad);
            colCosto.DataPropertyName = nameof(ItemMovimientoDto.CostoUnitario);

            colCantidad.DefaultCellStyle.Format = "N2";
            colCosto.DefaultCellStyle.Format = "N2";
            colCantidad.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            colCosto.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        }

        // =========================
        //  PRODUCTO: CARGA POR CÓDIGO / BARRAS
        // =========================

        private void txtProductoCodigo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;
            e.SuppressKeyPress = true;

            CargarProducto(txtProductoCodigo.Text.Trim());
        }

        private void btnBuscarProducto_Click(object sender, EventArgs e)
        {
            txtProductoCodigo.Focus();
            txtProductoCodigo.SelectAll();
        }

        private void CargarProducto(string entrada)
        {
            if (string.IsNullOrWhiteSpace(entrada))
                return;

            try
            {
                // ✅ IMPORTANTE: aquí usamos OBARRAS (código, referencia o código de barra)
                var p = _productoRepo.ObtenerPorCodigoOBarras(entrada);
                if (p == null)
                {
                    MessageBox.Show("Producto no encontrado.", "Inventario",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    LimpiarProducto();
                    return;
                }

                _productoActual = p;

                txtProductoCodigo.Text = p.Codigo ?? entrada;
                txtProductoDescripcion.Text = GetDescripcionProducto(p);

                // EXISTENCIA
                var stock = _productoRepo.ObtenerStockActual(p.Codigo!);
                txtExistencia.Text = stock.ToString("N2");

                // COSTO desde producto
                var costo = p.PrecioCoste;
                if (costo <= 0m) costo = p.PrecioCompraPromedio;
                if (costo <= 0m) costo = 0m;
                txtCosto.Text = costo.ToString("N2");

                numCantidad.Value = 1;
                numCantidad.Focus();
                numCantidad.Select(0, numCantidad.Text.Length);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error cargando producto: " + ex.Message, "Inventario",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                LimpiarProducto();
            }
        }

        private static string GetDescripcionProducto(Producto p)
        {
            // ✅ Si viene vacío/espacios, cae al siguiente
            if (!string.IsNullOrWhiteSpace(p.Referencia))
                return p.Referencia!.Trim();

            if (!string.IsNullOrWhiteSpace(p.Descripcion))
                return p.Descripcion!.Trim();

            return p.Codigo ?? "Producto";
        }

        private void LimpiarProducto()
        {
            _productoActual = null;
            txtProductoDescripcion.Text = "";
            txtExistencia.Text = "0.00";
            txtCosto.Text = "0.00";
            numCantidad.Value = 1;
        }

        // =========================
        //  LÍNEAS
        // =========================

        private void btnAgregarLinea_Click(object sender, EventArgs e)
        {
            try
            {
                var entrada = txtProductoCodigo.Text.Trim();
                if (string.IsNullOrWhiteSpace(entrada))
                {
                    MessageBox.Show("Digite o escanee un código de producto.", "Inventario",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txtProductoCodigo.Focus();
                    return;
                }

                // ✅ Reusar producto cargado si coincide, sino recargar por barras
                Producto? p = _productoActual;

                if (p == null || string.IsNullOrWhiteSpace(p.Codigo) ||
                    (!entrada.Equals(p.Codigo, StringComparison.OrdinalIgnoreCase)
                     && !entrada.Equals(p.CodReferencia ?? "", StringComparison.OrdinalIgnoreCase)))
                {
                    p = _productoRepo.ObtenerPorCodigoOBarras(entrada);
                }

                if (p == null)
                {
                    MessageBox.Show("Producto no encontrado.", "Inventario",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtProductoCodigo.Focus();
                    txtProductoCodigo.SelectAll();
                    return;
                }

                var cantidad = (decimal)numCantidad.Value;
                if (cantidad <= 0)
                {
                    MessageBox.Show("Cantidad inválida.", "Inventario",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var costo = p.PrecioCoste;
                if (costo <= 0m) costo = p.PrecioCompraPromedio;
                if (costo <= 0m) costo = 0m;
                var desc = GetDescripcionProducto(p);

                // Si ya existe en líneas, sumamos
                var existente = _lineas.FirstOrDefault(x => x.ProductoCodigo == p.Codigo);
                if (existente != null)
                {
                    existente.Cantidad += cantidad;
                    existente.CostoUnitario = costo;
                    if (string.IsNullOrWhiteSpace(existente.Descripcion))
                        existente.Descripcion = desc;
                }
                else
                {
                    _lineas.Add(new ItemMovimientoDto
                    {
                        ProductoCodigo = p.Codigo!,
                        Descripcion = desc,
                        Cantidad = cantidad,
                        CostoUnitario = costo
                    });
                }

                _bsLineas.ResetBindings(false);

                // limpiar para siguiente scan
                txtProductoCodigo.Clear();
                txtProductoDescripcion.Clear();
                txtExistencia.Text = "0.00";
                txtCosto.Text = "0.00";
                numCantidad.Value = 1;
                _productoActual = null;

                txtProductoCodigo.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error agregando línea: " + ex.Message, "Inventario",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnQuitarLinea_Click(object sender, EventArgs e)
        {
            if (gridLineas.CurrentRow == null) return;

            if (gridLineas.CurrentRow.DataBoundItem is not ItemMovimientoDto it) return;

            _lineas.Remove(it);
            _bsLineas.ResetBindings(false);
        }

        // =========================
        //  GUARDAR MOVIMIENTO
        // =========================

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                if (_lineas.Count == 0)
                {
                    MessageBox.Show("Agregue al menos una línea.", "Inventario",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var tipo = (cboTipo.SelectedItem?.ToString() ?? "ENTRADA").Trim().ToUpperInvariant();

                var almacenOrigen = Convert.ToInt32(cboAlmacenOrigen.SelectedValue);

                int? almacenDestino = null;
                if (chkUsarDestino.Checked)
                    almacenDestino = Convert.ToInt32(cboAlmacenDestino.SelectedValue);

                var invMovId = _movService.CrearMovimiento(
                    fecha: dtFecha.Value,
                    tipo: tipo,
                    almacenIdOrigen: almacenOrigen,
                    almacenIdDestino: almacenDestino,
                    usuario: _usuario,
                    observacion: txtObservacion.Text.Trim(),
                    lineas: _lineas.ToList()
                );

                MessageBox.Show($"Movimiento guardado.\nID: {invMovId}", "Inventario",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                LimpiarFormulario();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar: " + ex.Message, "Inventario",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LimpiarFormulario()
        {
            _lineas.Clear();
            _bsLineas.ResetBindings(false);

            txtProductoCodigo.Clear();
            txtProductoDescripcion.Clear();
            txtExistencia.Text = "0.00";
            txtCosto.Text = "0.00";
            numCantidad.Value = 1;

            txtObservacion.Clear();
            dtFecha.Value = DateTime.Now;

            _productoActual = null;
            txtProductoCodigo.Focus();
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void chkUsarDestino_CheckedChanged(object sender, EventArgs e)
        {
            cboAlmacenDestino.Enabled = chkUsarDestino.Checked;
        }
    }
}
