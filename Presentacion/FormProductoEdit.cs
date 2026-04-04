using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows.Forms;
using Andloe.Data;
using Andloe.Entidad;
using Andloe.Logica;

namespace Andloe.Presentacion
{
    public partial class FormProductoEdit : Form
    {
        private string? _codigoActual;
        private bool _cargandoDescripcionFiscal = false;
        private string _ultimaDescripcionBase = string.Empty;

        private readonly ECFUnidadMedidaRepository _ecfUnidadRepo = new();
        private readonly ProductoRepository _repo = new();
        private readonly ImpuestoRepository _impRepo = new();
        private readonly NumeradorRepository _numRepo = new();
        private readonly CategoriaRepository _catRepo = new();
        private readonly SubcategoriaRepository _subRepo = new();
        private readonly SistemaConfigRepository _cfgRepo = new();

        private readonly BindingSource _bsBarras = new();
        private List<ProductoRepository.CodigoBarra> _barras = new();

        private List<string> _listaCodigos = new();
        private int _idx = -1;

        private const string CFG_EMPRESA_DEFECTO_ID = "EMPRESA_DEFECTO_ID";
        private const string CFG_SUCURSAL_DEFECTO_ID = "SUCURSAL_DEFECTO_ID";
        private const string CFG_ALMACEN_DEFECTO_ID = "ALMACEN_DEFECTO_ID";

        private const string RUTA_IMAGENES_PRODUCTO = @"C:\Andloe\AndloeSystem\Imagen_Producto";
        private byte[]? _imagenProductoBytes;
        private string? _imagenProductoExtension;
        private bool _quitarImagenPendiente = false;

        private readonly ErrorProvider _ep = new();
        private readonly ToolTip _tt = new();

        private static readonly Color ColorError = Color.MistyRose;
        private static readonly Color ColorOk = Color.White;
        private static readonly Color ColorReadOnly = Color.FromArgb(245, 245, 245);

        public FormProductoEdit(string? codigo)
        {
            _codigoActual = string.IsNullOrWhiteSpace(codigo) ? null : codigo.Trim();
            InitializeComponent();
            Inicializar();
        }

        private void Inicializar()
        {
            ConfigurarUIPro();
            AplicarEstiloVisualPro();
            ConectarEventos();

            _bsBarras.DataSource = _barras;
            gridBarras.AutoGenerateColumns = false;
            gridBarras.DataSource = _bsBarras;

            CargarCategorias();
            CargarImpuestos();
            CargarUnidades();

            if (_codigoActual != null)
            {
                _listaCodigos = _repo.Listar(null, 5000).Select(x => x.Codigo).ToList();
                _idx = _listaCodigos.FindIndex(x => x == _codigoActual);
            }

            if (_codigoActual != null)
            {
                Text = $"Editar Producto - {_codigoActual}";
                txtCodigo.ReadOnly = true;
                txtCodigo.BackColor = ColorReadOnly;

                CargarProducto(_codigoActual);
                CargarBarras(_codigoActual);
                CargarBloqueoNegativoUI(_codigoActual);

                HabilitarZonaBarras(true);
                btnFinalizar.Visible = true;
                btnFinalizar.Enabled = true;
            }
            else
            {
                Text = "Crear Producto";

                txtCodigo.Text = _numRepo.Next("PROD", "P-", 6);
                txtCodigo.ReadOnly = true;
                txtCodigo.BackColor = ColorReadOnly;

                chkActivo.Checked = true;

                if (chkBloqNegativo != null)
                    chkBloqNegativo.Checked = false;

                txtDescripcionFiscal.Text = "";
                txtCodigoItemFiscal.Text = txtCodigo.Text.Trim();
                cboTipoProducto.SelectedIndex = 0;

                txtPrecioVenta.Text = "0.00";
                txtPrecioCosto.Text = "0.00";
                txtPrecioMayor.Text = "0.00";
                txtPorcBeneficio.Text = "0.00";
                txtExistencia.Text = "0.00";
                txtUltimoPrecioCompra.Text = "0.00";

                HabilitarZonaBarras(false);

                btnFinalizar.Visible = true;
                btnFinalizar.Enabled = false;

                SincronizarPrecioIncluyeITBISDesdeImpuesto();
                LimpiarImagenUI();
            }

            ValidarPantalla();
        }

        private void ConfigurarUIPro()
        {
            BackColor = Color.FromArgb(245, 246, 248);

            _ep.BlinkStyle = ErrorBlinkStyle.NeverBlink;
            _ep.ContainerControl = this;

            _tt.IsBalloon = false;
            _tt.AutoPopDelay = 5000;
            _tt.InitialDelay = 300;
            _tt.ReshowDelay = 150;

            ConfigurarTextBox(txtCodigo, true);
            ConfigurarTextBox(txtDescripcion, false);
            ConfigurarTextBox(txtDescripcionFiscal, false);
            ConfigurarTextBox(txtReferencia, false);
            ConfigurarTextBox(txtCodigoItemFiscal, false);

            ConfigurarTextBox(txtPrecioVenta, false, true);
            ConfigurarTextBox(txtPrecioCosto, false, true);
            ConfigurarTextBox(txtPrecioMayor, false, true);

            ConfigurarTextBox(txtExistencia, true, true);
            ConfigurarTextBox(txtUltimoPrecioCompra, true, true);
            ConfigurarTextBox(txtPorcBeneficio, false, true);

            ConfigurarCombo(cboUnidad);
            ConfigurarCombo(cboImpuesto);
            ConfigurarCombo(cboCategoria);
            ConfigurarCombo(cboSubcategoria);
            ConfigurarCombo(cboTipoProducto);

            chkPrecioIncluyeITBIS.Enabled = false;
            chkPrecioIncluyeITBIS.AutoCheck = false;

            txtExistencia.ReadOnly = true;
            txtExistencia.TabStop = false;
            txtExistencia.BackColor = ColorReadOnly;

            txtUltimoPrecioCompra.ReadOnly = true;
            txtUltimoPrecioCompra.TabStop = false;
            txtUltimoPrecioCompra.BackColor = ColorReadOnly;

            txtCodigo.ReadOnly = true;
            txtCodigo.BackColor = ColorReadOnly;

            ConfigurarGridBarras();

            _tt.SetToolTip(txtCodigo, "Código interno del producto");
            _tt.SetToolTip(txtDescripcion, "Descripción comercial del producto");
            _tt.SetToolTip(txtDescripcionFiscal, "Texto que se usará para la factura fiscal / e-CF");
            _tt.SetToolTip(txtCodigoItemFiscal, "Si queda vacío, se toma automáticamente el código del producto");
            _tt.SetToolTip(chkPrecioIncluyeITBIS, "Se calcula automáticamente según el impuesto seleccionado");
            _tt.SetToolTip(btnCargarImagen, "Seleccionar imagen del producto");
            _tt.SetToolTip(btnQuitarImagen, "Quitar imagen");
        }

        private void AplicarEstiloVisualPro()
        {
            BackColor = Color.FromArgb(245, 246, 248);

            AplicarEstiloGroupBox(grpGeneral);
            AplicarEstiloGroupBox(grpFiscal);
            AplicarEstiloGroupBox(grpPrecios);
            AplicarEstiloGroupBox(grpInventario);
            AplicarEstiloGroupBox(grpImagen);
            AplicarEstiloGroupBox(grpBarras);

            AplicarEstiloBotonPrimario(btnGuardar);
            AplicarEstiloBotonSecundario(btnAtras);
            AplicarEstiloBotonSecundario(btnSiguiente);
            AplicarEstiloBotonPrimario(btnFinalizar);

            AplicarEstiloBotonSecundario(btnCargarImagen);
            AplicarEstiloBotonSecundario(btnQuitarImagen);
            AplicarEstiloBotonSecundario(btnAgregarBarraManual);
            AplicarEstiloBotonSecundario(btnAgregarBarraAuto);
            AplicarEstiloBotonSecundario(btnEliminarBarra);

            picImagenProducto.BackColor = Color.White;
            picImagenProducto.BorderStyle = BorderStyle.FixedSingle;

            gridBarras.BackgroundColor = Color.White;
            gridBarras.BorderStyle = BorderStyle.FixedSingle;
            gridBarras.GridColor = Color.FromArgb(220, 224, 230);
            gridBarras.ColumnHeadersHeight = 36;
            gridBarras.EnableHeadersVisualStyles = false;
            gridBarras.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(230, 233, 238);
            gridBarras.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
            gridBarras.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(230, 233, 238);
            gridBarras.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.Black;
        }

        private void AplicarEstiloGroupBox(GroupBox gb)
        {
            gb.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            gb.ForeColor = Color.FromArgb(35, 35, 35);
            gb.BackColor = Color.White;
            gb.Padding = new Padding(12);
        }

        private void AplicarEstiloBotonPrimario(Button btn)
        {
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.BackColor = Color.FromArgb(0, 120, 215);
            btn.ForeColor = Color.White;
            btn.Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold);
            btn.Height = 36;
            btn.Cursor = Cursors.Hand;
        }

        private void AplicarEstiloBotonSecundario(Button btn)
        {
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderColor = Color.FromArgb(200, 205, 210);
            btn.FlatAppearance.BorderSize = 1;
            btn.BackColor = Color.White;
            btn.ForeColor = Color.FromArgb(35, 35, 35);
            btn.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular);
            btn.Height = 36;
            btn.Cursor = Cursors.Hand;
        }

        private void ConfigurarTextBox(TextBox txt, bool readOnly, bool alignRight = false)
        {
            txt.BorderStyle = BorderStyle.FixedSingle;
            txt.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
            txt.ReadOnly = readOnly;
            txt.BackColor = readOnly ? ColorReadOnly : ColorOk;
            txt.TextAlign = alignRight ? HorizontalAlignment.Right : HorizontalAlignment.Left;

            if (txt.Multiline)
            {
                txt.Margin = new Padding(3, 3, 3, 6);
                txt.Dock = DockStyle.Fill;
            }
            else
            {
                txt.Margin = new Padding(3);
                txt.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            }
        }

        private void ConfigurarCombo(ComboBox cbo)
        {
            cbo.DropDownStyle = ComboBoxStyle.DropDownList;
            cbo.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
            cbo.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        }

        private void ConfigurarGridBarras()
        {
            gridBarras.EnableHeadersVisualStyles = false;
            gridBarras.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(230, 233, 238);
            gridBarras.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
            gridBarras.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            gridBarras.DefaultCellStyle.Font = new Font("Segoe UI", 9.5F);
            gridBarras.DefaultCellStyle.SelectionBackColor = Color.FromArgb(0, 120, 215);
            gridBarras.DefaultCellStyle.SelectionForeColor = Color.White;
            gridBarras.RowTemplate.Height = 30;
            gridBarras.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
        }

        private void ConectarEventos()
        {
            btnGuardar.Click += (_, __) => Guardar();
            btnFinalizar.Click += (_, __) => Finalizar();

            btnAgregarBarraManual.Click += (_, __) => AgregarBarraManual();
            btnAgregarBarraAuto.Click += (_, __) => AgregarBarraAuto();
            btnEliminarBarra.Click += (_, __) => EliminarBarraSeleccionada();

            btnAtras.Click += (_, __) => IrAtras();
            btnSiguiente.Click += (_, __) => IrSiguiente();

            btnCargarImagen.Click += (_, __) => CargarImagen();
            btnQuitarImagen.Click += (_, __) => QuitarImagen();

            txtCodigo.TextChanged += (_, __) =>
            {
                var cod = txtCodigo.Text.Trim();

                if (string.IsNullOrWhiteSpace(txtCodigoItemFiscal.Text) ||
                    string.Equals(txtCodigoItemFiscal.Text.Trim(), _codigoActual ?? "", StringComparison.OrdinalIgnoreCase))
                {
                    txtCodigoItemFiscal.Text = cod;
                }

                ValidarPantalla();
            };

            txtDescripcion.TextChanged += (_, __) =>
            {
                if (_cargandoDescripcionFiscal) return;

                if (string.IsNullOrWhiteSpace(txtDescripcionFiscal.Text) ||
                    txtDescripcionFiscal.Text == _ultimaDescripcionBase)
                {
                    txtDescripcionFiscal.Text = txtDescripcion.Text;
                }

                _ultimaDescripcionBase = txtDescripcion.Text;
                ValidarPantalla();
            };

            txtDescripcionFiscal.TextChanged += (_, __) =>
            {
                if (!_cargandoDescripcionFiscal && string.IsNullOrWhiteSpace(txtDescripcionFiscal.Text))
                    txtDescripcionFiscal.Text = txtDescripcion.Text;

                ValidarPantalla();
            };

            txtReferencia.TextChanged += (_, __) => ValidarPantalla();
            txtCodigoItemFiscal.TextChanged += (_, __) => ValidarPantalla();

            cboImpuesto.SelectedIndexChanged += CboImpuesto_SelectedIndexChanged;
            cboUnidad.SelectedIndexChanged += (_, __) => ValidarPantalla();
            cboCategoria.SelectedIndexChanged += (_, __) => ValidarPantalla();
            cboSubcategoria.SelectedIndexChanged += (_, __) => ValidarPantalla();
            cboTipoProducto.SelectedIndexChanged += (_, __) => ValidarPantalla();

            txtPrecioVenta.Enter += (_, __) => SeleccionarTodo(txtPrecioVenta);
            txtPrecioCosto.Enter += (_, __) => SeleccionarTodo(txtPrecioCosto);
            txtPrecioMayor.Enter += (_, __) => SeleccionarTodo(txtPrecioMayor);
            txtPorcBeneficio.Enter += (_, __) => SeleccionarTodo(txtPorcBeneficio);

            txtPrecioVenta.Leave += (_, __) => FormatearDecimalTextbox(txtPrecioVenta);
            txtPrecioCosto.Leave += (_, __) => FormatearDecimalTextbox(txtPrecioCosto);
            txtPrecioMayor.Leave += (_, __) => FormatearDecimalTextbox(txtPrecioMayor);
            txtPorcBeneficio.Leave += (_, __) => FormatearDecimalTextbox(txtPorcBeneficio);

            txtPrecioVenta.TextChanged += (_, __) => ValidarPantalla();
            txtPrecioCosto.TextChanged += (_, __) => ValidarPantalla();
            txtPrecioMayor.TextChanged += (_, __) => ValidarPantalla();
            txtPorcBeneficio.TextChanged += (_, __) => ValidarPantalla();
        }

        private void HabilitarZonaBarras(bool enabled)
        {
            gridBarras.Enabled = enabled;
            txtBarraManual.Enabled = enabled;
            btnAgregarBarraManual.Enabled = enabled;
            btnAgregarBarraAuto.Enabled = enabled;
            btnEliminarBarra.Enabled = enabled;
            btnFinalizar.Enabled = enabled;
        }

        private void SeleccionarTodo(TextBox txt)
        {
            BeginInvoke(new Action(() => txt.SelectAll()));
        }

        private void FormatearDecimalTextbox(TextBox txt)
        {
            if (decimal.TryParse(txt.Text, NumberStyles.Any, CultureInfo.CurrentCulture, out var val) ||
                decimal.TryParse(txt.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out val))
            {
                txt.Text = val.ToString("N2");
            }
            else if (string.IsNullOrWhiteSpace(txt.Text))
            {
                txt.Text = "0.00";
            }
        }

        private void CargarImpuestos()
        {
            var impuestos = _impRepo.ListarActivos();

            cboImpuesto.DisplayMember = nameof(Impuesto.Nombre);
            cboImpuesto.ValueMember = nameof(Impuesto.ImpuestoId);
            cboImpuesto.DataSource = impuestos;

            var ex = impuestos.FirstOrDefault(x => x.Codigo == "EXENTO");
            if (ex != null)
                cboImpuesto.SelectedValue = ex.ImpuestoId;

            SincronizarPrecioIncluyeITBISDesdeImpuesto();
        }

        private void CboImpuesto_SelectedIndexChanged(object? sender, EventArgs e)
        {
            SincronizarPrecioIncluyeITBISDesdeImpuesto();
            ValidarPantalla();
        }

        private void SincronizarPrecioIncluyeITBISDesdeImpuesto()
        {
            var impuestoSel = cboImpuesto.SelectedItem as Impuesto;
            if (impuestoSel == null)
            {
                chkPrecioIncluyeITBIS.Checked = false;
                return;
            }

            var esExento =
                string.Equals((impuestoSel.Codigo ?? "").Trim(), "EXENTO", StringComparison.OrdinalIgnoreCase) ||
                impuestoSel.Porcentaje <= 0;

            chkPrecioIncluyeITBIS.Checked = !esExento;
        }

        private void CargarUnidades()
        {
            var unidades = _ecfUnidadRepo.ListarActivas();

            cboUnidad.DisplayMember = "Descripcion";
            cboUnidad.ValueMember = "UnidadMedidaECFId";
            cboUnidad.DataSource = unidades;

            var und = unidades.FirstOrDefault(x =>
                string.Equals(Convert.ToString(x.UnidadInternaCodigo)?.Trim(), "UND", StringComparison.OrdinalIgnoreCase));

            if (und != null)
                cboUnidad.SelectedValue = und.UnidadMedidaECFId;
        }

        private void CargarCategorias()
        {
            var cats = _catRepo.ListarActivas()
                .Select(x => new { x.Id, x.Nombre })
                .ToList();

            cboCategoria.DisplayMember = "Nombre";
            cboCategoria.ValueMember = "Id";
            cboCategoria.DataSource = cats;

            cboCategoria.SelectedIndexChanged += (_, __) =>
            {
                if (cboCategoria.SelectedValue == null) return;

                int catId = cboCategoria.SelectedValue is int v
                    ? v
                    : Convert.ToInt32(cboCategoria.SelectedValue);

                CargarSubcategorias(catId);
                ValidarPantalla();
            };
        }

        private void CargarSubcategorias(int catId)
        {
            var subs = _subRepo.ListarPorCategoria(catId)
                .Select(x => new { x.Id, x.Nombre })
                .ToList();

            cboSubcategoria.DisplayMember = "Nombre";
            cboSubcategoria.ValueMember = "Id";
            cboSubcategoria.DataSource = subs;
        }

        private void CargarProducto(string codigo)
        {
            var p = _repo.ObtenerPorCodigo(codigo);
            if (p == null)
            {
                MessageBox.Show("Producto no encontrado.", "Producto",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Close();
                return;
            }

            _codigoActual = p.Codigo;
            Text = $"Editar Producto - {_codigoActual}";

            txtCodigo.Text = p.Codigo;

            _cargandoDescripcionFiscal = true;
            txtDescripcion.Text = p.Descripcion ?? "";
            txtDescripcionFiscal.Text = string.IsNullOrWhiteSpace(p.DescripcionFiscal)
                ? (p.Descripcion ?? "")
                : p.DescripcionFiscal;
            _ultimaDescripcionBase = txtDescripcion.Text;
            _cargandoDescripcionFiscal = false;

            txtReferencia.Text = p.Referencia ?? "";
            txtPrecioVenta.Text = p.PrecioVenta.ToString("N2");
            txtPrecioCosto.Text = p.PrecioCoste.ToString("N2");
            txtPrecioMayor.Text = p.PrecioMayor.ToString("N2");
            txtPorcBeneficio.Text = p.PorcFijoBeneficio.ToString("N2");
            txtExistencia.Text = p.StockActual.ToString("N2");
            txtUltimoPrecioCompra.Text = p.UltimoPrecioCompra.ToString("N2");

            txtCodigoItemFiscal.Text = string.IsNullOrWhiteSpace(p.CodigoItemFiscal)
                ? p.Codigo
                : p.CodigoItemFiscal;

            cboTipoProducto.SelectedIndex = p.TipoProducto == 2 ? 1 : 0;
            chkActivo.Checked = p.Estado == 1;

            if (p.ImpuestoId.HasValue)
                cboImpuesto.SelectedValue = p.ImpuestoId.Value;

            if (p.UnidadMedidaECFId.HasValue && p.UnidadMedidaECFId.Value > 0)
                cboUnidad.SelectedValue = p.UnidadMedidaECFId.Value;

            MostrarExtrasProducto(p);
            CargarBloqueoNegativoUI(p.Codigo);
            SincronizarPrecioIncluyeITBISDesdeImpuesto();
            CargarImagenDesdePersistencia(p.Codigo);
            ValidarPantalla();
        }

        private void MostrarExtrasProducto(Producto p)
        {
            txtExistencia.Text = p.StockActual.ToString("N2");
            txtUltimoPrecioCompra.Text = p.UltimoPrecioCompra.ToString("N2");
            txtPorcBeneficio.Text = p.PorcFijoBeneficio.ToString("N2");
            txtPrecioCosto.Text = p.PrecioCoste.ToString("N2");
            txtPrecioMayor.Text = p.PrecioMayor.ToString("N2");

            if (p.CategoriaId.HasValue)
            {
                cboCategoria.SelectedValue = p.CategoriaId.Value;
                CargarSubcategorias(p.CategoriaId.Value);

                if (p.SubcategoriaId.HasValue)
                    cboSubcategoria.SelectedValue = p.SubcategoriaId.Value;
            }
            else
            {
                cboSubcategoria.DataSource = null;
            }
        }

        private void CargarBarras(string codigoProd)
        {
            _barras = _repo.ListarCodigosBarras(codigoProd);
            _bsBarras.DataSource = _barras;
            _bsBarras.ResetBindings(false);
        }

        private (int empresaId, int sucursalId, int almacenId) GetContextoDefaults()
        {
            var emp = _cfgRepo.GetEntero(CFG_EMPRESA_DEFECTO_ID) ?? 1;
            var suc = _cfgRepo.GetEntero(CFG_SUCURSAL_DEFECTO_ID) ?? 1;
            var alm = _cfgRepo.GetEntero(CFG_ALMACEN_DEFECTO_ID) ?? 1;

            if (emp <= 0) emp = 1;
            if (suc <= 0) suc = 1;
            if (alm <= 0) alm = 1;

            return (emp, suc, alm);
        }

        private string BuildClaveBloqNegativo(string productoCodigo)
        {
            var ctx = GetContextoDefaults();
            return
                "STOCK_NEGATIVO_BLOQ_PROD_EMP_" + ctx.empresaId +
                "_SUC_" + ctx.sucursalId +
                "_ALM_" + ctx.almacenId +
                "_" + (productoCodigo ?? "").Trim();
        }

        private void CargarBloqueoNegativoUI(string productoCodigo)
        {
            if (chkBloqNegativo == null) return;

            if (string.IsNullOrWhiteSpace(productoCodigo))
            {
                chkBloqNegativo.Checked = false;
                return;
            }

            var clave = BuildClaveBloqNegativo(productoCodigo);
            var val = _cfgRepo.GetValor(clave);

            chkBloqNegativo.Checked =
                string.Equals((val ?? "").Trim(), "1", StringComparison.OrdinalIgnoreCase);
        }

        private void GuardarBloqueoNegativo(string productoCodigo)
        {
            if (chkBloqNegativo == null) return;
            if (string.IsNullOrWhiteSpace(productoCodigo)) return;

            var clave = BuildClaveBloqNegativo(productoCodigo);
            var usuario = string.IsNullOrWhiteSpace(Environment.UserName)
                ? "SYSTEM"
                : Environment.UserName.Trim();

            if (chkBloqNegativo.Checked)
                _cfgRepo.SetValorSimple(clave, "1", usuario);
            else if (_cfgRepo.ExisteClave(clave))
                _cfgRepo.DeleteClave(clave);
        }

        private bool ValidarPantalla()
        {
            var ok = true;

            LimpiarError(txtDescripcion);
            LimpiarError(txtDescripcionFiscal);
            LimpiarError(txtCodigoItemFiscal);
            LimpiarError(cboImpuesto);
            LimpiarError(cboUnidad);
            LimpiarError(txtPrecioVenta);

            if (string.IsNullOrWhiteSpace(txtDescripcion.Text))
            {
                PonerError(txtDescripcion, "La descripción es obligatoria.");
                ok = false;
            }

            if (string.IsNullOrWhiteSpace(txtDescripcionFiscal.Text))
            {
                PonerError(txtDescripcionFiscal, "La descripción fiscal es obligatoria.");
                ok = false;
            }

            if (string.IsNullOrWhiteSpace(txtCodigoItemFiscal.Text) &&
                string.IsNullOrWhiteSpace(txtCodigo.Text))
            {
                PonerError(txtCodigoItemFiscal, "Debe existir código fiscal o código de producto.");
                ok = false;
            }

            if (cboImpuesto.SelectedItem == null)
            {
                PonerError(cboImpuesto, "Debe seleccionar un impuesto.");
                ok = false;
            }

            if (cboUnidad.SelectedItem == null)
            {
                PonerError(cboUnidad, "Debe seleccionar una unidad.");
                ok = false;
            }

            if (!decimal.TryParse(txtPrecioVenta.Text, NumberStyles.Any, CultureInfo.CurrentCulture, out var pv) &&
                !decimal.TryParse(txtPrecioVenta.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out pv))
            {
                PonerError(txtPrecioVenta, "Precio venta inválido.");
                ok = false;
            }
            else if (pv < 0)
            {
                PonerError(txtPrecioVenta, "El precio no puede ser negativo.");
                ok = false;
            }

            btnGuardar.Enabled = ok;
            return ok;
        }

        private void PonerError(Control c, string msg)
        {
            _ep.SetError(c, msg);
            c.BackColor = ColorError;
        }

        private void LimpiarError(Control c)
        {
            _ep.SetError(c, "");
            if (c is TextBox txt)
                c.BackColor = txt.ReadOnly ? ColorReadOnly : ColorOk;
            else
                c.BackColor = ColorOk;
        }

        private void Guardar()
        {
            try
            {
                if (!ValidarPantalla())
                    throw new Exception("Hay campos obligatorios o inválidos. Revise los marcados.");

                var codigo = txtCodigo.Text.Trim();

                var descBase = txtDescripcion.Text.Trim();
                var referenciaTexto = txtReferencia.Text.Trim();

                if (!string.IsNullOrWhiteSpace(referenciaTexto) &&
                    !descBase.EndsWith(" " + referenciaTexto, StringComparison.OrdinalIgnoreCase))
                {
                    descBase = $"{descBase} {referenciaTexto}";
                }

                var desc = descBase;
                txtDescripcion.Text = desc;

                if (!decimal.TryParse(txtPrecioVenta.Text, NumberStyles.Any, CultureInfo.CurrentCulture, out var precioVenta) &&
                    !decimal.TryParse(txtPrecioVenta.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out precioVenta))
                    throw new Exception("Precio venta inválido.");

                decimal.TryParse(txtPrecioCosto.Text, NumberStyles.Any, CultureInfo.CurrentCulture, out var precioCosto);
                decimal.TryParse(txtPrecioMayor.Text, NumberStyles.Any, CultureInfo.CurrentCulture, out var precioMayor);
                decimal.TryParse(txtPorcBeneficio.Text, NumberStyles.Any, CultureInfo.CurrentCulture, out var porcBeneficio);

                var impuestoId = cboImpuesto.SelectedValue == null ? (int?)null : Convert.ToInt32(cboImpuesto.SelectedValue);
                var categoriaId = cboCategoria.SelectedValue == null ? (int?)null : Convert.ToInt32(cboCategoria.SelectedValue);
                var subcategoriaId = cboSubcategoria.SelectedValue == null ? (int?)null : Convert.ToInt32(cboSubcategoria.SelectedValue);

                var impuestoSel = cboImpuesto.SelectedItem as Impuesto;
                var esExento = impuestoSel != null &&
                               (impuestoSel.Codigo?.Trim().ToUpperInvariant() == "EXENTO" || impuestoSel.Porcentaje <= 0);

                var unidadSel = cboUnidad.SelectedItem as ECFUnidadMedida;
                if (unidadSel == null)
                    throw new Exception("Debe seleccionar una unidad de medida e-CF.");

                var unidadInterna = Convert.ToString(unidadSel.UnidadInternaCodigo)?.Trim().ToUpperInvariant() ?? "";
                var codigoDgii = Convert.ToString(unidadSel.CodigoDGII)?.Trim() ?? "";

                if (string.IsNullOrWhiteSpace(unidadInterna))
                    throw new Exception("La unidad seleccionada no tiene UnidadInternaCodigo válido.");

                if (string.IsNullOrWhiteSpace(codigoDgii))
                    throw new Exception("La unidad seleccionada no tiene CodigoDGII válido.");

                var unidadBase = unidadInterna;

                var descripcionFiscal = string.IsNullOrWhiteSpace(txtDescripcionFiscal.Text)
                    ? desc
                    : txtDescripcionFiscal.Text.Trim();

                var referenciaFinal = txtReferencia.Text.Trim();

                var codigoItemFiscal = string.IsNullOrWhiteSpace(txtCodigoItemFiscal.Text)
                    ? codigo
                    : txtCodigoItemFiscal.Text.Trim();

                txtCodigoItemFiscal.Text = codigoItemFiscal;

                var tipoProducto = cboTipoProducto.SelectedIndex == 1 ? 2 : 1;

                if (_codigoActual == null)
                {
                    var existe = _repo.ObtenerPorCodigo(codigo);
                    if (existe != null)
                        throw new Exception("Ese código ya existe. Revisa NumeradorSecuencia.");

                    var nuevo = new Producto
                    {
                        Codigo = codigo,
                        Descripcion = desc,
                        Referencia = referenciaFinal,
                        CodReferencia = referenciaFinal,
                        DescripcionFiscal = descripcionFiscal,
                        PrecioIncluyeITBIS = chkPrecioIncluyeITBIS.Checked,

                        UnidadBase = unidadBase,
                        UnidadMedidaCodigo = unidadInterna,
                        UnidadMedidaDGII = codigoDgii,
                        UnidadMedidaECFId = unidadSel.UnidadMedidaECFId,

                        CodigoItemFiscal = codigoItemFiscal,
                        TipoProducto = tipoProducto,
                        EsExento = esExento,

                        PrecioVenta = precioVenta,
                        PrecioCoste = precioCosto,
                        PrecioMayor = precioMayor,
                        PorcFijoBeneficio = porcBeneficio,

                        CategoriaId = categoriaId == 0 ? (int?)null : categoriaId,
                        SubcategoriaId = subcategoriaId == 0 ? (int?)null : subcategoriaId,
                        ImpuestoId = esExento ? null : (impuestoId == 0 ? (int?)null : impuestoId),

                        Estado = chkActivo.Checked ? 1 : 0
                    };

                    _repo.InsertBasico(nuevo);

                    _codigoActual = codigo;
                    GuardarBloqueoNegativo(_codigoActual);
                    GuardarImagenDoblePersistencia(_codigoActual);

                    Text = $"Editar Producto - {_codigoActual}";
                    txtCodigo.ReadOnly = true;
                    txtCodigo.BackColor = ColorReadOnly;

                    HabilitarZonaBarras(true);
                    btnFinalizar.Enabled = true;

                    CargarBarras(_codigoActual);

                    MessageBox.Show(
                        "Producto creado correctamente.",
                        "Producto",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    return;
                }

                var p = _repo.ObtenerPorCodigo(_codigoActual)!;

                p.Descripcion = desc;
                p.Referencia = referenciaFinal;
                p.CodReferencia = referenciaFinal;
                p.DescripcionFiscal = descripcionFiscal;

                p.UnidadBase = unidadBase;
                p.UnidadMedidaCodigo = unidadInterna;
                p.UnidadMedidaDGII = codigoDgii;
                p.UnidadMedidaECFId = unidadSel.UnidadMedidaECFId;

                p.PrecioIncluyeITBIS = chkPrecioIncluyeITBIS.Checked;
                p.CodigoItemFiscal = codigoItemFiscal;
                p.TipoProducto = tipoProducto;
                p.EsExento = esExento;

                p.PrecioVenta = precioVenta;
                p.PrecioCoste = precioCosto;
                p.PrecioMayor = precioMayor;
                p.PorcFijoBeneficio = porcBeneficio;

                p.CategoriaId = categoriaId;
                p.SubcategoriaId = subcategoriaId;
                p.ImpuestoId = esExento ? null : impuestoId;
                p.Estado = chkActivo.Checked ? 1 : 0;

                var antes = _repo.ObtenerPorCodigo(p.Codigo);
                var antesJson = antes != null ? JsonSerializer.Serialize(antes) : null;

                _repo.ActualizarBasico(p);

                var despues = _repo.ObtenerPorCodigo(p.Codigo);
                var despuesJson = despues != null ? JsonSerializer.Serialize(despues) : null;

                new AuditoriaService().Log(
                    modulo: "PRODUCTOS",
                    accion: "EDITAR",
                    entidad: "Producto",
                    entidadId: p.Codigo,
                    detalle: $"Producto editado: {p.Codigo} - {p.Descripcion}",
                    antesJson: antesJson,
                    despuesJson: despuesJson
                );

                GuardarBloqueoNegativo(p.Codigo);
                GuardarImagenDoblePersistencia(p.Codigo);

                MessageBox.Show("Cambios guardados.", "Producto",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                CargarProducto(p.Codigo);
                CargarBarras(p.Codigo);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Guardar",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Finalizar()
        {
            if (string.IsNullOrWhiteSpace(_codigoActual))
            {
                MessageBox.Show("Primero debe guardar el producto.", "Finalizar",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private void AgregarBarraManual()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(_codigoActual))
                    throw new Exception("Primero guarde el producto para agregar códigos de barra.");

                var barra = txtBarraManual.Text.Trim();
                if (string.IsNullOrWhiteSpace(barra))
                    throw new Exception("Digite un código de barra.");

                var usuario = string.IsNullOrWhiteSpace(Environment.UserName)
                    ? "SYSTEM"
                    : Environment.UserName;

                _repo.CrearCodigoBarraManual(barra, _codigoActual, usuario);

                txtBarraManual.Clear();
                CargarBarras(_codigoActual);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Códigos de barra",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AgregarBarraAuto()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(_codigoActual))
                    throw new Exception("Primero guarde el producto para generar código de barra.");

                var usuario = string.IsNullOrWhiteSpace(Environment.UserName)
                    ? "SYSTEM"
                    : Environment.UserName;

                var gen = _repo.CrearCodigoBarraAuto(_codigoActual, usuario);
                CargarBarras(_codigoActual);

                MessageBox.Show($"Código generado: {gen}", "Códigos de barra",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Códigos de barra",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void EliminarBarraSeleccionada()
        {
            try
            {
                if (gridBarras.CurrentRow?.DataBoundItem is not ProductoRepository.CodigoBarra item)
                    return;

                if (string.IsNullOrWhiteSpace(item.CodigoBarras))
                    return;

                if (MessageBox.Show(
                        $"¿Eliminar código {item.CodigoBarras}?",
                        "Confirmar",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question) != DialogResult.Yes)
                    return;

                _repo.EliminarCodigoBarra(item.CodigoBarras);

                if (!string.IsNullOrWhiteSpace(_codigoActual))
                    CargarBarras(_codigoActual);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Códigos de barra",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void IrSiguiente()
        {
            if (_listaCodigos.Count == 0) return;
            if (_idx < _listaCodigos.Count - 1) _idx++;

            var cod = _listaCodigos[_idx];
            _codigoActual = cod;

            CargarProducto(cod);
            CargarBarras(cod);
            CargarBloqueoNegativoUI(cod);

            HabilitarZonaBarras(true);
            btnFinalizar.Enabled = true;
        }

        private void IrAtras()
        {
            if (_listaCodigos.Count == 0) return;
            if (_idx > 0) _idx--;

            var cod = _listaCodigos[_idx];
            _codigoActual = cod;

            CargarProducto(cod);
            CargarBarras(cod);
            CargarBloqueoNegativoUI(cod);

            HabilitarZonaBarras(true);
            btnFinalizar.Enabled = true;
        }

        private void CargarImagen()
        {
            try
            {
                using var ofd = new OpenFileDialog
                {
                    Title = "Seleccionar imagen del producto",
                    Filter = "Imágenes|*.jpg;*.jpeg;*.png;*.bmp;*.webp",
                    Multiselect = false
                };

                if (ofd.ShowDialog() != DialogResult.OK)
                    return;

                var bytes = File.ReadAllBytes(ofd.FileName);
                using var ms = new MemoryStream(bytes);
                using var imgTemp = Image.FromStream(ms);

                _imagenProductoBytes = bytes;
                _imagenProductoExtension = Path.GetExtension(ofd.FileName)?.ToLowerInvariant();
                _quitarImagenPendiente = false;

                var old = picImagenProducto.Image;
                picImagenProducto.Image = new Bitmap(imgTemp);
                old?.Dispose();
                picImagenProducto.SizeMode = PictureBoxSizeMode.Zoom;
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo cargar la imagen.\n" + ex.Message,
                    "Imagen", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void QuitarImagen()
        {
            _imagenProductoBytes = null;
            _imagenProductoExtension = null;
            _quitarImagenPendiente = true;
            LimpiarImagenUI();

            if (!string.IsNullOrWhiteSpace(_codigoActual))
            {
                EliminarImagenDeDisco(_codigoActual);
                _repo.QuitarImagen(_codigoActual);
            }
        }

        private void GuardarImagenDoblePersistencia(string codigoProducto)
        {
            if (string.IsNullOrWhiteSpace(codigoProducto))
                return;

            if (_quitarImagenPendiente)
            {
                EliminarImagenDeDisco(codigoProducto);
                _repo.QuitarImagen(codigoProducto);
                _quitarImagenPendiente = false;
                return;
            }

            if (_imagenProductoBytes == null || _imagenProductoBytes.Length == 0)
                return;

            GuardarImagenEnDisco(codigoProducto);
            _repo.GuardarImagen(codigoProducto, _imagenProductoBytes);
        }

        private void GuardarImagenEnDisco(string codigoProducto)
        {
            Directory.CreateDirectory(RUTA_IMAGENES_PRODUCTO);

            var ext = string.IsNullOrWhiteSpace(_imagenProductoExtension) ? ".jpg" : _imagenProductoExtension;
            if (!ext.StartsWith(".")) ext = "." + ext;

            EliminarImagenDeDisco(codigoProducto);

            var ruta = Path.Combine(RUTA_IMAGENES_PRODUCTO, codigoProducto + ext);
            File.WriteAllBytes(ruta, _imagenProductoBytes!);
        }

        private void CargarImagenDesdePersistencia(string codigoProducto)
        {
            _imagenProductoBytes = null;
            _imagenProductoExtension = null;
            _quitarImagenPendiente = false;

            LimpiarImagenUI();

            var archivo = BuscarArchivoImagenProducto(codigoProducto);
            if (!string.IsNullOrWhiteSpace(archivo) && File.Exists(archivo))
            {
                var bytes = File.ReadAllBytes(archivo);
                _imagenProductoBytes = bytes;
                _imagenProductoExtension = Path.GetExtension(archivo)?.ToLowerInvariant();
                CargarImagenEnUIDesdeBytes(bytes);
                return;
            }

            var bytesSql = _repo.ObtenerImagen(codigoProducto);
            if (bytesSql != null && bytesSql.Length > 0)
            {
                _imagenProductoBytes = bytesSql;
                _imagenProductoExtension = ".jpg";
                CargarImagenEnUIDesdeBytes(bytesSql);
            }
        }

        private void CargarImagenEnUIDesdeBytes(byte[] bytes)
        {
            using var ms = new MemoryStream(bytes);
            using var imgTemp = Image.FromStream(ms);

            var old = picImagenProducto.Image;
            picImagenProducto.Image = new Bitmap(imgTemp);
            old?.Dispose();
            picImagenProducto.SizeMode = PictureBoxSizeMode.Zoom;
        }

        private void EliminarImagenDeDisco(string codigoProducto)
        {
            Directory.CreateDirectory(RUTA_IMAGENES_PRODUCTO);

            var extensiones = new[] { ".jpg", ".jpeg", ".png", ".bmp", ".webp" };
            foreach (var ext in extensiones)
            {
                var ruta = Path.Combine(RUTA_IMAGENES_PRODUCTO, codigoProducto + ext);
                if (File.Exists(ruta))
                    File.Delete(ruta);
            }
        }

        private string? BuscarArchivoImagenProducto(string codigoProducto)
        {
            var extensiones = new[] { ".jpg", ".jpeg", ".png", ".bmp", ".webp" };
            foreach (var ext in extensiones)
            {
                var ruta = Path.Combine(RUTA_IMAGENES_PRODUCTO, codigoProducto + ext);
                if (File.Exists(ruta))
                    return ruta;
            }

            return null;
        }

        private void LimpiarImagenUI()
        {
            var old = picImagenProducto.Image;
            picImagenProducto.Image = null;
            old?.Dispose();
        }
    }
}