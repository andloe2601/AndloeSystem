using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Andloe.Data;
using Andloe.Entidad;
using System.Text.Json;
using Andloe.Logica;

namespace Andloe.Presentacion
{
    public partial class FormProductoEdit : Form
    {
        private string? _codigoActual;

        private readonly ProductoRepository _repo = new();
        private readonly ImpuestoRepository _impRepo = new();
        private readonly NumeradorRepository _numRepo = new();
        private readonly CategoriaRepository _catRepo = new();
        private readonly SubcategoriaRepository _subRepo = new();

        // ✅ NUEVO
        private readonly SistemaConfigRepository _cfgRepo = new();

        private readonly BindingSource _bsBarras = new();
        private List<ProductoRepository.CodigoBarra> _barras = new();

        private List<string> _listaCodigos = new();
        private int _idx = -1;

        // ✅ claves config (las mismas que ya usamos en FacturaRepository)
        private const string CFG_EMPRESA_DEFECTO_ID = "EMPRESA_DEFECTO_ID";
        private const string CFG_SUCURSAL_DEFECTO_ID = "SUCURSAL_DEFECTO_ID";
        private const string CFG_ALMACEN_DEFECTO_ID = "ALMACEN_DEFECTO_ID";

        public FormProductoEdit(string? codigo)
        {
            _codigoActual = string.IsNullOrWhiteSpace(codigo) ? null : codigo.Trim();
            InitializeComponent();
            Inicializar();
        }

        private void Inicializar()
        {
            btnGuardar.Click += (_, __) => Guardar();
            btnFinalizar.Click += (_, __) => Finalizar();
            btnCerrar.Click += (_, __) => Close();

            btnAgregarBarraManual.Click += (_, __) => AgregarBarraManual();
            btnAgregarBarraAuto.Click += (_, __) => AgregarBarraAuto();
            btnEliminarBarra.Click += (_, __) => EliminarBarraSeleccionada();

            btnAtras.Click += (_, __) => IrAtras();
            btnSiguiente.Click += (_, __) => IrSiguiente();

            _bsBarras.DataSource = _barras;
            gridBarras.AutoGenerateColumns = false;
            gridBarras.DataSource = _bsBarras;

            txtExistencia.ReadOnly = true;
            txtExistencia.TabStop = false;

            txtUltimoPrecioCompra.ReadOnly = true;
            txtUltimoPrecioCompra.TabStop = false;

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

                CargarProducto(_codigoActual);
                CargarBarras(_codigoActual);

                // ✅ cargar flag negativo
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

                chkActivo.Checked = true;

                // ✅ por defecto: NO bloquear (puedes cambiarlo si quieres)
                if (chkBloqNegativo != null) chkBloqNegativo.Checked = false;

                txtPrecioVenta.Text = "0.00";
                txtPrecioCosto.Text = "0.00";
                txtPrecioMayor.Text = "0.00";
                txtPorcBeneficio.Text = "0.00";
                txtExistencia.Text = "0.00";
                txtUltimoPrecioCompra.Text = "0.00";

                HabilitarZonaBarras(false);

                btnFinalizar.Visible = true;
                btnFinalizar.Enabled = false;
            }

            FormClosing += (_, __) =>
            {
                // aquí no hacemos nada: cerrar con X no debe refrescar FormProductos
            };
        }

        private void HabilitarZonaBarras(bool enabled)
        {
            gridBarras.Enabled = enabled;
            txtBarraManual.Enabled = enabled;
            btnAgregarBarraManual.Enabled = enabled;
            btnAgregarBarraAuto.Enabled = enabled;
            btnEliminarBarra.Enabled = enabled;
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
        }

        private void CargarUnidades()
        {
            var unidades = _repo.ListarUnidadesActivas()
                .Select(x => new { Codigo = x.Codigo, Nombre = x.Nombre })
                .ToList();

            cboUnidad.DisplayMember = "Nombre";
            cboUnidad.ValueMember = "Codigo";
            cboUnidad.DataSource = unidades;

            var und = unidades.FirstOrDefault(x => x.Codigo == "UND");
            if (und != null)
                cboUnidad.SelectedValue = und.Codigo;
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

                int catId;
                if (cboCategoria.SelectedValue is int v) catId = v;
                else catId = Convert.ToInt32(cboCategoria.SelectedValue);

                CargarSubcategorias(catId);
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
            txtDescripcion.Text = p.Descripcion ?? "";
            txtReferencia.Text = p.Referencia ?? "";
            txtPrecioVenta.Text = p.PrecioVenta.ToString("N2");
            chkActivo.Checked = p.Estado == 1;

            if (p.ImpuestoId.HasValue)
                cboImpuesto.SelectedValue = p.ImpuestoId.Value;

            if (!string.IsNullOrWhiteSpace(p.UnidadBase))
                cboUnidad.SelectedValue = p.UnidadBase;

            MostrarExtrasProducto(p);

            // ✅ cargar flag negativo
            CargarBloqueoNegativoUI(p.Codigo);
        }

        private void MostrarExtrasProducto(Producto p)
        {
            txtExistencia.Text = p.StockActual.ToString("N2");
            txtUltimoPrecioCompra.Text = (p.UltimoPrecioCompra ?? 0m).ToString("N2");
            txtPorcBeneficio.Text = (p.PorcFijoBeneficio ?? 0m).ToString("N2");
            txtPrecioCosto.Text = (p.PrecioCoste ?? 0m).ToString("N2");
            txtPrecioMayor.Text = (p.PrecioMayor ?? 0m).ToString("N2");

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

        // ============================================================
        // ✅ BLOQUEO NEGATIVO (POR PRODUCTO) usando SistemaConfig
        // ============================================================
        private (int empresaId, int sucursalId, int almacenId) GetContextoDefaults()
        {
            // tú tienes 1/1/5, pero lo leemos de SistemaConfig (más pro)
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

            // existe y vale "1" => bloqueado
            chkBloqNegativo.Checked = string.Equals((val ?? "").Trim(), "1", StringComparison.OrdinalIgnoreCase);
        }

        private void GuardarBloqueoNegativo(string productoCodigo)
        {
            if (chkBloqNegativo == null) return;
            if (string.IsNullOrWhiteSpace(productoCodigo)) return;

            var clave = BuildClaveBloqNegativo(productoCodigo);
            var usuario = string.IsNullOrWhiteSpace(Environment.UserName) ? "SYSTEM" : Environment.UserName.Trim();

            if (chkBloqNegativo.Checked)
            {
                // ✅ checked = bloquear => upsert Valor=1
                _cfgRepo.SetValorSimple(clave, "1", usuario);
            }
            else
            {
                // ✅ unchecked = permitir => borrar clave si existe
                if (_cfgRepo.ExisteClave(clave))
                    _cfgRepo.DeleteClave(clave);
            }
        }

        private void Guardar()
        {
            try
            {
                var codigo = txtCodigo.Text.Trim();
                var desc = txtDescripcion.Text.Trim();

                if (string.IsNullOrWhiteSpace(codigo))
                    throw new Exception("El código es obligatorio.");
                if (string.IsNullOrWhiteSpace(desc))
                    throw new Exception("La descripción es obligatoria.");

                if (!decimal.TryParse(txtPrecioVenta.Text, out var precioVenta))
                    throw new Exception("Precio venta inválido.");

                decimal.TryParse(txtPrecioCosto.Text, out var precioCosto);
                decimal.TryParse(txtPrecioMayor.Text, out var precioMayor);
                decimal.TryParse(txtPorcBeneficio.Text, out var porcBeneficio);

                var impuestoId = cboImpuesto.SelectedValue == null ? (int?)null : Convert.ToInt32(cboImpuesto.SelectedValue);
                var categoriaId = cboCategoria.SelectedValue == null ? (int?)null : Convert.ToInt32(cboCategoria.SelectedValue);
                var subcategoriaId = cboSubcategoria.SelectedValue == null ? (int?)null : Convert.ToInt32(cboSubcategoria.SelectedValue);

                var unidadBase = cboUnidad.SelectedValue?.ToString();

                if (_codigoActual == null)
                {
                    var existe = _repo.ObtenerPorCodigo(codigo);
                    if (existe != null)
                        throw new Exception("Ese código ya existe. Revisa NumeradorSecuencia.");

                    var nuevo = new Producto
                    {
                        Codigo = codigo,
                        Descripcion = desc,
                        Referencia = txtReferencia.Text.Trim(),
                        UnidadBase = unidadBase,

                        PrecioVenta = precioVenta,
                        PrecioCoste = precioCosto,
                        PrecioMayor = precioMayor,
                        PorcFijoBeneficio = porcBeneficio,

                        CategoriaId = categoriaId,
                        SubcategoriaId = subcategoriaId,
                        ImpuestoId = impuestoId,

                        Estado = chkActivo.Checked ? 1 : 0
                    };

                    _repo.InsertBasico(nuevo);

                    _codigoActual = codigo;

                    // ✅ guardar bloqueo negativo luego de crear
                    GuardarBloqueoNegativo(_codigoActual);

                    Text = $"Editar Producto - {_codigoActual}";
                    txtCodigo.ReadOnly = true;

                    HabilitarZonaBarras(true);
                    btnFinalizar.Enabled = true;

                    CargarBarras(_codigoActual);

                    MessageBox.Show("Producto creado. Ahora agregue los códigos de barra y presione Finalizar.",
                        "Producto", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    return;
                }

                var p = _repo.ObtenerPorCodigo(_codigoActual)!;

                p.Descripcion = desc;
                p.Referencia = txtReferencia.Text.Trim();
                p.UnidadBase = unidadBase;

                p.PrecioVenta = precioVenta;
                p.PrecioCoste = precioCosto;
                p.PrecioMayor = precioMayor;
                p.PorcFijoBeneficio = porcBeneficio;

                p.CategoriaId = categoriaId;
                p.SubcategoriaId = subcategoriaId;
                p.ImpuestoId = impuestoId;

                p.Estado = chkActivo.Checked ? 1 : 0;

                var antes = _repo.ObtenerPorCodigo(p.Codigo);
                var antesJson = antes != null ? JsonSerializer.Serialize(antes) : null;

                _repo.ActualizarBasico(p);

                // ✅ Después
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

                // ✅ guardar bloqueo negativo al editar
                GuardarBloqueoNegativo(p.Codigo);

                MessageBox.Show("Cambios guardados.", "Producto",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                MostrarExtrasProducto(p);
                CargarBarras(_codigoActual);
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

                var usuario = string.IsNullOrWhiteSpace(Environment.UserName) ? "SYSTEM" : Environment.UserName;

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

                var usuario = string.IsNullOrWhiteSpace(Environment.UserName) ? "SYSTEM" : Environment.UserName;

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

                if (MessageBox.Show($"¿Eliminar código {item.CodigoBarras}?", "Confirmar",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
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

            // ✅ refrescar flag por producto
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

            // ✅ refrescar flag por producto
            CargarBloqueoNegativoUI(cod);

            HabilitarZonaBarras(true);
            btnFinalizar.Enabled = true;
        }
    }
}
