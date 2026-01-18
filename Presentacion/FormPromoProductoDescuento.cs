using System;
using System.Linq;
using System.Windows.Forms;
using Andloe.Data;

namespace Presentation
{
    public partial class FormPromoProductoDescuento : Form
    {
        private readonly PromoRepository _promoRepo = new();
        private readonly ProductoRepository _prodRepo = new();

        private readonly string _usuarioActual;
        private int? _promoIdEdicion;

        private bool EsEdicion => _promoIdEdicion.HasValue;

        // ===========================
        //   CONSTRUCTORES
        // ===========================
        public FormPromoProductoDescuento(string usuarioActual)
        {
            _usuarioActual = usuarioActual;
            InitializeComponent();

            dtpDesde.Value = DateTime.Today;
            dtpHasta.Value = DateTime.Today.AddMonths(1);

            CargarCodigoSugerido();

            if (cmbTipo.Items.Count > 0)
                cmbTipo.SelectedIndex = 0;

            chkTodosDias.Checked = true;

            // evento para habilitar/deshabilitar controles
            cmbTipo.SelectedIndexChanged += cmbTipo_SelectedIndexChanged;
            ActualizarControlesSegunTipo();
        }

        // EDICIÓN
        public FormPromoProductoDescuento(string usuarioActual, int promoIdEditar)
            : this(usuarioActual)
        {
            _promoIdEdicion = promoIdEditar;
            CargarPromoParaEditar(promoIdEditar);
        }

        // ===========================
        //   CAMBIO DE TIPO
        // ===========================
        private void cmbTipo_SelectedIndexChanged(object? sender, EventArgs e)
        {
            ActualizarControlesSegunTipo();
        }

        private void ActualizarControlesSegunTipo()
        {
            var tipo = cmbTipo.SelectedItem?.ToString() ?? string.Empty;

            nudDescuentoPct.Enabled = false;
            nudPrecioPromo.Enabled = false;
            nudCantPack.Enabled = false;

            lblAyudaTipo.Text = string.Empty;

            switch (tipo)
            {
                case "Descuento %":
                    nudDescuentoPct.Enabled = true;
                    nudCantPack.Enabled = true; // cantidad mínima del grupo
                    lblAyudaTipo.Text =
                        "Si el cliente compra al menos N unidades (Cant. pack) de los productos " +
                        "del listado, se aplica el % de descuento indicado.";
                    break;

                case "Precio fijo":
                    nudPrecioPromo.Enabled = true;
                    lblAyudaTipo.Text =
                        "El producto se vende al precio promocional indicado, sin mínimo de cantidad.";
                    break;

                case "Pack (N por $)":
                    nudCantPack.Enabled = true;
                    nudPrecioPromo.Enabled = true;
                    lblAyudaTipo.Text =
                        "Si el cliente compra N unidades (Cant. pack), paga solo el precio total del pack.";
                    break;
            }
        }

        // ===========================
        //   CÓDIGO SUGERIDO
        // ===========================
        private void CargarCodigoSugerido()
        {
            try
            {
                var cod = _promoRepo.ObtenerProximoCodigoPromo(); // peek
                txtCodigoPromo.Text = cod;
            }
            catch
            {
                txtCodigoPromo.Text = string.Empty;
            }
        }

        // ===========================
        //   BÚSQUEDA DE PRODUCTO
        // ===========================
        private void btnBuscarProducto_Click(object? sender, EventArgs e)
        {
            try
            {
                using (var frm = new FormBuscarProductoPromo())
                {
                    var dr = frm.ShowDialog(this);
                    if (dr == DialogResult.OK &&
                        !string.IsNullOrWhiteSpace(frm.ProductoCodigoSeleccionado))
                    {
                        txtProductoCodigo.Text = frm.ProductoCodigoSeleccionado;

                        if (nudDescuentoPct.Value == 0)
                            nudDescuentoPct.Value = 5;

                        btnAgregarLinea_Click(sender, e);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al buscar producto: " + ex.Message,
                    "Promo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ===========================
        //   AGREGAR LÍNEA
        // ===========================
        private void btnAgregarLinea_Click(object? sender, EventArgs e)
        {
            try
            {
                var prodCod = txtProductoCodigo.Text.Trim();

                if (string.IsNullOrWhiteSpace(prodCod))
                {
                    MessageBox.Show("Indique un código de producto.", "Promo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var prodInfo = _prodRepo.ObtenerPorCodigo(prodCod);
                if (prodInfo == null)
                {
                    MessageBox.Show("El producto indicado no existe.", "Promo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                foreach (DataGridViewRow r in dgvLineas.Rows)
                {
                    if (!r.IsNewRow &&
                        string.Equals(Convert.ToString(r.Cells["colProdCodigo"].Value),
                                      prodInfo.Codigo,
                                      StringComparison.OrdinalIgnoreCase))
                    {
                        MessageBox.Show("El producto ya está en la lista.", "Promo",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                }

                int idx = dgvLineas.Rows.Add();
                var row = dgvLineas.Rows[idx];
                row.Cells["colProdCodigo"].Value = prodInfo.Codigo;
                row.Cells["colProdNombre"].Value = prodInfo.Descripcion;
                row.Cells["colPrecioVenta"].Value = prodInfo.PrecioVenta;
                row.Cells["colCosto"].Value = prodInfo.PrecioCoste;
                row.Cells["colDescPct"].Value = nudDescuentoPct.Value;

                txtProductoCodigo.Clear();
                txtProductoCodigo.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al agregar línea: " + ex.Message,
                    "Promo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ===========================
        //   ELIMINAR LÍNEA
        // ===========================
        private void btnEliminarLinea_Click(object? sender, EventArgs e)
        {
            try
            {
                if (dgvLineas.CurrentRow == null || dgvLineas.CurrentRow.IsNewRow)
                    return;

                dgvLineas.Rows.RemoveAt(dgvLineas.CurrentRow.Index);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al borrar línea: " + ex.Message,
                    "Promo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ===========================
        //   GUARDAR PROMO
        // ===========================
        private void btnGuardar_Click(object? sender, EventArgs e)
        {
            try
            {
                if (EsEdicion)
                {
                    MessageBox.Show(
                        "La edición de promociones todavía no está implementada.\n" +
                        "Por ahora, use 'Nuevo' para crear una nueva promoción.",
                        "Promo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var nombre = txtNombrePromo.Text.Trim();
                var fDesde = dtpDesde.Value.Date;
                var fHasta = dtpHasta.Value.Date;
                var activa = chkActiva.Checked;

                decimal descPctGlobal = nudDescuentoPct.Value;
                decimal precioPromoGlobal = nudPrecioPromo.Value;
                decimal cantPackGlobal = nudCantPack.Value;

                if (string.IsNullOrWhiteSpace(nombre))
                {
                    MessageBox.Show("Debe indicar un nombre para la promoción.", "Promo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (_promoRepo.ExisteNombrePromo(nombre))
                {
                    MessageBox.Show("Ya existe una promoción con ese nombre.", "Promo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (fHasta < fDesde)
                {
                    MessageBox.Show("La fecha fin no puede ser menor que la fecha inicio.", "Promo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (dgvLineas.Rows.Count == 0 ||
                    dgvLineas.Rows.Cast<DataGridViewRow>().All(r => r.IsNewRow))
                {
                    MessageBox.Show("Agregue al menos un producto en el listado.", "Promo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // ----- Tipo de regla -----
                var tipoReglaLinea = "DESCUENTO_PCT";
                var tipoTexto = cmbTipo.SelectedItem?.ToString() ?? string.Empty;

                switch (tipoTexto)
                {
                    case "Descuento %":
                        tipoReglaLinea = "DESCUENTO_PCT";
                        if (descPctGlobal <= 0)
                        {
                            MessageBox.Show("Indique un % de descuento mayor a 0.",
                                "Promo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                        break;

                    case "Precio fijo":
                        tipoReglaLinea = "PRECIO_FIJO";
                        if (precioPromoGlobal <= 0)
                        {
                            MessageBox.Show("Indique un precio promocional mayor a 0.",
                                "Promo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                        break;

                    case "Pack (N por $)":
                        tipoReglaLinea = "PACK_PRODUCTO";
                        if (cantPackGlobal <= 0 || precioPromoGlobal <= 0)
                        {
                            MessageBox.Show("Para pack indique cantidad y precio total.",
                                "Promo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                        break;

                    default:
                        tipoReglaLinea = "DESCUENTO_PCT";
                        if (descPctGlobal <= 0)
                        {
                            MessageBox.Show("Indique un % de descuento mayor a 0 o seleccione otro tipo.",
                                "Promo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                        break;
                }

                // ----- Días -----
                bool lunes = chkLunes.Checked;
                bool martes = chkMartes.Checked;
                bool miercoles = chkMiercoles.Checked;
                bool jueves = chkJueves.Checked;
                bool viernes = chkViernes.Checked;
                bool sabado = chkSabado.Checked;
                bool domingo = chkDomingo.Checked;

                if (chkTodosDias.Checked)
                {
                    lunes = martes = miercoles =
                        jueves = viernes = sabado = domingo = true;
                }

                // ----- Productos -----
                var codigosProductos = dgvLineas.Rows
                    .Cast<DataGridViewRow>()
                    .Where(r => !r.IsNewRow)
                    .Select(r => Convert.ToString(r.Cells["colProdCodigo"].Value))
                    .Where(c => !string.IsNullOrWhiteSpace(c))
                    .Select(c => c!.Trim())
                    .ToList();

                if (!codigosProductos.Any())
                {
                    MessageBox.Show("No hay productos válidos en la lista.", "Promo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var primerProducto = codigosProductos.First();

                // ----- Parámetros según regla -----
                decimal? descPct = null;
                decimal? precioPromo = null;
                decimal? cantPack = null;
                decimal? packPrecioTotal = null;

                // cantidad mínima para que se aplique la promo al grupo
                decimal? cantidadMinima = null;

                if (tipoReglaLinea == "DESCUENTO_PCT")
                {
                    descPct = descPctGlobal;
                    if (nudCantPack.Value > 0)
                        cantidadMinima = nudCantPack.Value;
                }
                else if (tipoReglaLinea == "PRECIO_FIJO")
                {
                    precioPromo = precioPromoGlobal;
                    if (nudCantPack.Value > 0)
                        cantidadMinima = nudCantPack.Value;
                }
                else if (tipoReglaLinea == "PACK_PRODUCTO")
                {
                    cantPack = cantPackGlobal;
                    packPrecioTotal = precioPromoGlobal;
                    cantidadMinima = cantPackGlobal; // coincide con el pack
                }

                // ----- Crear UNA sola promo -----
                int promoId = _promoRepo.CrearPromoDescuentoProducto(
                    codigoPromo: string.Empty,   // <- no null
                    nombre: nombre,
                    productoCodigo: primerProducto,
                    tipoRegla: tipoReglaLinea,
                    descuentoPct: descPct,
                    precioFijoPromo: precioPromo,
                    packCant: cantPack,
                    packPrecioTotal: packPrecioTotal,
                    fechaDesde: fDesde,
                    fechaHasta: fHasta,
                    activa: activa,
                    usuario: _usuarioActual,
                    lunes: lunes,
                    martes: martes,
                    miercoles: miercoles,
                    jueves: jueves,
                    viernes: viernes,
                    sabado: sabado,
                    domingo: domingo,
                    cantidadMinima: cantidadMinima
                );

                // ----- Reemplazar alcance con TODOS los productos -----
                _promoRepo.ReemplazarProductosPromo(promoId, codigosProductos);

                var codigoReal = _promoRepo.ObtenerCodigoPromoPorId(promoId);
                if (!string.IsNullOrWhiteSpace(codigoReal))
                    txtCodigoPromo.Text = codigoReal;

                MessageBox.Show("Promoción creada correctamente.",
                    "Promo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar promoción: " + ex.Message,
                    "Promo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ===========================
        //   CANCELAR
        // ===========================
        private void btnCancelar_Click(object? sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        // ===========================
        //   TODOS LOS DÍAS
        // ===========================
        private void chkTodosDias_CheckedChanged(object? sender, EventArgs e)
        {
            bool v = chkTodosDias.Checked;
            chkLunes.Checked = v;
            chkMartes.Checked = v;
            chkMiercoles.Checked = v;
            chkJueves.Checked = v;
            chkViernes.Checked = v;
            chkSabado.Checked = v;
            chkDomingo.Checked = v;
        }

        // ===========================
        //   CARGAR PROMO PARA EDICIÓN
        // ===========================
        private void CargarPromoParaEditar(int promoId)
        {
            try
            {
                var dto = _promoRepo.ObtenerPromoProductoPorId(promoId);
                if (dto == null)
                {
                    MessageBox.Show("No se encontró la promoción seleccionada.",
                        "Promo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                txtCodigoPromo.Text = dto.Codigo;
                txtNombrePromo.Text = dto.Nombre;
                chkActiva.Checked = dto.Activa;

                dtpDesde.Value = dto.FechaInicio;
                dtpHasta.Value = dto.FechaFin;

                chkLunes.Checked = dto.Lunes;
                chkMartes.Checked = dto.Martes;
                chkMiercoles.Checked = dto.Miercoles;
                chkJueves.Checked = dto.Jueves;
                chkViernes.Checked = dto.Viernes;
                chkSabado.Checked = dto.Sabado;
                chkDomingo.Checked = dto.Domingo;

                chkTodosDias.Checked =
                    dto.Lunes && dto.Martes && dto.Miercoles &&
                    dto.Jueves && dto.Viernes && dto.Sabado && dto.Domingo;

                if (dto.DescuentoPct > 0)
                {
                    cmbTipo.SelectedItem = "Descuento %";
                    nudDescuentoPct.Value = dto.DescuentoPct;
                    nudPrecioPromo.Value = 0;
                    nudCantPack.Value = 0;
                }
                else if (dto.PrecioFijo > 0)
                {
                    cmbTipo.SelectedItem = "Precio fijo";
                    nudDescuentoPct.Value = 0;
                    nudPrecioPromo.Value = dto.PrecioFijo;
                    nudCantPack.Value = 0;
                }
                else if (dto.EsPack && dto.PackCantidad > 0 && dto.PackPrecioTotal > 0)
                {
                    cmbTipo.SelectedItem = "Pack (N por $)";
                    nudDescuentoPct.Value = 0;
                    nudPrecioPromo.Value = dto.PackPrecioTotal;
                    nudCantPack.Value = dto.PackCantidad;
                }
                else
                {
                    nudDescuentoPct.Value = 0;
                    nudPrecioPromo.Value = 0;
                    nudCantPack.Value = 0;
                }

                // refrescar habilitación de controles y mensaje de ayuda
                ActualizarControlesSegunTipo();

                dgvLineas.Rows.Clear();
                foreach (var p in dto.Productos)
                {
                    int idx = dgvLineas.Rows.Add();
                    var row = dgvLineas.Rows[idx];
                    row.Cells["colProdCodigo"].Value = p.Codigo;
                    row.Cells["colProdNombre"].Value = p.Nombre;
                    row.Cells["colPrecioVenta"].Value = p.PrecioVenta;
                    row.Cells["colCosto"].Value = p.PrecioCoste;
                    row.Cells["colDescPct"].Value = dto.DescuentoPct;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar promoción para edición: " + ex.Message,
                    "Promo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
