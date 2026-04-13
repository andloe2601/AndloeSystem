using Andloe.Data;
using Andloe.Data.DGII;
using Andloe.Entidad;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Windows.Forms;
using EntidadCliente = Andloe.Entidad.Cliente;
using System.Data;


namespace Andloe.Presentacion
{
    public partial class FormClienteEdit : Form
    {
        private readonly ClienteRepository _repo = new();
        private readonly DgiiRncRepository _dgiiRepo = new();
        private readonly string? _codigo;
        private readonly string? _filtroRetorno;
        private readonly TerminoPagoRepository _terminoRepo = new();
        private List<TerminoPago> _terminosPago = new();

        private readonly ProvinciaRepository _provinciaRepo = new();
        private readonly MunicipioRepository _municipioRepo = new();

        private List<Provincia> _provincias = new();
        private List<Municipio> _municipios = new();

        private readonly MonedaRepository _monedaRepo = new();
        private readonly VendedorRepository _vendedorRepo = new();
        private readonly AlmacenRepository _almacenRepo = new();
        private readonly PaisRepository _paisRepo = new();

        private DataTable _monedas = new();
        private List<Vendedor> _vendedores = new();
        private List<Almacen> _almacenes = new();
        private List<Pais> _paises = new();

        private bool _actualizandoUi;

        public FormClienteEdit(string? codigo, string? filtroRetorno = null)
        {
            _codigo = codigo;
            _filtroRetorno = filtroRetorno;
            InitializeComponent();

            AcceptButton = btnGuardar;
            CancelButton = btnCancelar;

            btnGuardar.Click += (_, __) => Guardar();
            btnCancelar.Click += (_, __) => VolverAListadoClientes();
            btnValidarDgii.Click += (_, __) => ValidarConDgii();
            chkEsExtranjero.CheckedChanged += (_, __) =>
            {
                AplicarReglasSegunTipoCliente();
                ActualizarEstadoFiscalUi();
            };
            chkValidadoDgii.CheckedChanged += (_, __) => ActualizarEstadoFiscalUi();
            cboTipoIdentificacion.SelectedIndexChanged += (_, __) => ActualizarEstadoFiscalUi();
            cboProvincia.SelectedIndexChanged += (_, __) => CargarMunicipiosSegunProvincia();
            cboMoneda.SelectedIndexChanged += (_, __) => SincronizarMonedaSeleccionada();
            cboVendedor.SelectedIndexChanged += (_, __) => SincronizarVendedorSeleccionado();
            cboAlmacen.SelectedIndexChanged += (_, __) => SincronizarAlmacenSeleccionado();
            cboPais.SelectedIndexChanged += (_, __) => SincronizarPaisSeleccionado();
            cboMunicipio.SelectedIndexChanged += (_, __) => SincronizarMunicipioSeleccionado();

            txtRnc.TextChanged += (_, __) =>
            {
                txtRnc.Text = SoloDigitos(txtRnc.Text);
                txtRnc.SelectionStart = txtRnc.Text.Length;
                ActualizarEstadoFiscalUi();
            };

            cboTerminoPago.SelectedIndexChanged += (_, __) => SincronizarTerminoSeleccionado();

            txtRazonFiscal.TextChanged += (_, __) => ActualizarEstadoFiscalUi();
            txtEstadoRncDgii.TextChanged += (_, __) => ActualizarEstadoFiscalUi();

            Load += (_, __) =>
            {
                CargarCatalogos();
                if (_codigo == null) PrepNuevo();
                else Cargar();
            };
        }

        private void CargarCatalogos()
        {
            cboTipo.Items.Clear();
            cboTipo.Items.AddRange(new object[] { "Normal", "Mayorista", "VIP" });
            cboTipo.SelectedIndex = 0;

            CargarProvincias();
            CargarTerminosPago();
            CargarMonedas();
            CargarVendedores();
            CargarAlmacenes();
            CargarPaises();

            cboTipoIdentificacion.Items.Clear();
            cboTipoIdentificacion.Items.AddRange(new object[]
            {
        new ComboItem<byte>(1, "RNC"),
        new ComboItem<byte>(2, "Cédula"),
        new ComboItem<byte>(3, "Identificador extranjero")
            });
            cboTipoIdentificacion.SelectedIndex = 0;

            cboTipoClienteFiscal.Items.Clear();
            cboTipoClienteFiscal.Items.AddRange(new object[]
            {
        new ComboItem<byte>(0, "No definido"),
        new ComboItem<byte>(1, "Contribuyente"),
        new ComboItem<byte>(2, "Consumidor final"),
        new ComboItem<byte>(3, "Gubernamental"),
        new ComboItem<byte>(4, "Especial")
            });
            cboTipoClienteFiscal.SelectedIndex = 0;
        }

        private void CargarProvincias()
        {
            _provincias = _provinciaRepo.Listar();

            cboProvincia.DataSource = null;
            cboProvincia.DisplayMember = "Texto";
            cboProvincia.ValueMember = "CodigoProvincia";
            cboProvincia.DataSource = _provincias
                .Select(x => new
                {
                    x.CodigoProvincia,
                    Texto = $"{x.CodigoProvincia} - {x.Nombre}"
                })
                .ToList();

            cboProvincia.SelectedIndex = -1;
        }

        private void CargarMunicipiosSegunProvincia()
        {
            var codigoProvincia = cboProvincia.SelectedValue?.ToString();

            cboMunicipio.DataSource = null;
            txtMunicipioCodigo.Text = "";

            if (string.IsNullOrWhiteSpace(codigoProvincia))
            {
                txtProvinciaCodigo.Text = "";
                return;
            }

            txtProvinciaCodigo.Text = codigoProvincia;

            _municipios = _municipioRepo.ListarPorProvincia(codigoProvincia);

            cboMunicipio.DisplayMember = "Texto";
            cboMunicipio.ValueMember = "CodigoMunicipio";
            cboMunicipio.DataSource = _municipios
                .Select(x => new
                {
                    x.CodigoMunicipio,
                    Texto = $"{x.CodigoMunicipio} - {x.Nombre}"
                })
                .ToList();

            cboMunicipio.SelectedIndex = -1;
        }

        private void VolverAListadoClientes()
        {
            var principal = FormPrincipal.Instancia;
            if (principal == null) return;

            var frm = new FormClientes(_filtroRetorno);
            principal.OpenChild(frm);
            frm.Cargar();
        }

        private void CargarPaises()
        {
            _paises = _paisRepo.ListarActivos();

            cboPais.DataSource = null;
            cboPais.DisplayMember = "Texto";
            cboPais.ValueMember = "CodigoIso";
            cboPais.DataSource = _paises
                .Select(x => new
                {
                    x.CodigoIso,
                    Texto = $"{x.CodigoIso} - {x.Nombre}"
                })
                .ToList();

            cboPais.SelectedIndex = -1;
        }

        private void SincronizarPaisSeleccionado()
        {
            txtPaisCodigo.Text = cboPais.SelectedValue?.ToString() ?? "";
        }

        private void SeleccionarPais(string? codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo))
            {
                cboPais.SelectedIndex = -1;
                txtPaisCodigo.Text = "";
                return;
            }

            cboPais.SelectedValue = codigo.Trim();
            txtPaisCodigo.Text = codigo.Trim();
        }

        private void AplicarReglasSegunTipoCliente()
        {
            if (_actualizandoUi) return;

            _actualizandoUi = true;
            try
            {
                var esExtranjero = chkEsExtranjero.Checked;

                txtIdentificadorExtranjero.Enabled = esExtranjero;
                btnValidarDgii.Enabled = !esExtranjero;

                if (esExtranjero)
                {
                    cboPais.Enabled = true;

                    if (chkValidadoDgii.Checked)
                        chkValidadoDgii.Checked = false;

                    txtEstadoRncDgii.Text = "";
                }
                else
                {
                    SeleccionarPais("DO");
                    cboPais.Enabled = false;
                    txtIdentificadorExtranjero.Text = "";
                }
            }
            finally
            {
                _actualizandoUi = false;
            }
        }

        private void SincronizarMunicipioSeleccionado()
        {
            txtMunicipioCodigo.Text = cboMunicipio.SelectedValue?.ToString() ?? "";
        }

        private void SincronizarMonedaSeleccionada()
        {
            txtDivisa.Text = cboMoneda.SelectedValue?.ToString() ?? "";
        }

        private void SincronizarVendedorSeleccionado()
        {
            txtVendedor.Text = cboVendedor.SelectedValue?.ToString() ?? "";
        }

        private void SincronizarAlmacenSeleccionado()
        {
            txtAlmacen.Text = cboAlmacen.SelectedValue?.ToString() ?? "";
        }

        private void SeleccionarProvinciaMunicipio(string? provinciaCodigo, string? municipioCodigo)
        {
            if (string.IsNullOrWhiteSpace(provinciaCodigo))
            {
                cboProvincia.SelectedIndex = -1;
                cboMunicipio.DataSource = null;
                txtProvinciaCodigo.Text = "";
                txtMunicipioCodigo.Text = "";
                return;
            }

            cboProvincia.SelectedValue = provinciaCodigo.Trim();
            txtProvinciaCodigo.Text = provinciaCodigo.Trim();

            CargarMunicipiosSegunProvincia();

            if (!string.IsNullOrWhiteSpace(municipioCodigo))
            {
                cboMunicipio.SelectedValue = municipioCodigo.Trim();
                txtMunicipioCodigo.Text = municipioCodigo.Trim();
            }
        }

        private void CargarTerminosPago()
        {
            _terminosPago = _terminoRepo.ListarActivos();

            cboTerminoPago.DataSource = null;
            cboTerminoPago.DisplayMember = "Texto";
            cboTerminoPago.ValueMember = "Codigo";
            cboTerminoPago.DataSource = _terminosPago
                .Select(x => new
                {
                    Codigo = x.Codigo,
                    Texto = $"{x.Codigo} - {x.Descripcion}"
                })
                .ToList();

            cboTerminoPago.SelectedIndex = -1;
        }

        private void CargarMonedas()
        {
            _monedas = _monedaRepo.ListarMonedas();

            cboMoneda.DataSource = null;
            cboMoneda.DisplayMember = "Descripcion";
            cboMoneda.ValueMember = "Codigo";
            cboMoneda.DataSource = _monedas;

            cboMoneda.SelectedIndex = -1;
        }

        private void SeleccionarMoneda(string? codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo))
            {
                cboMoneda.SelectedIndex = -1;
                txtDivisa.Text = "";
                return;
            }

            cboMoneda.SelectedValue = codigo.Trim();
            txtDivisa.Text = codigo.Trim();
        }

        private void CargarVendedores()
        {
            _vendedores = _vendedorRepo.ListarActivos();

            cboVendedor.DataSource = null;
            cboVendedor.DisplayMember = "Texto";
            cboVendedor.ValueMember = "Codigo";
            cboVendedor.DataSource = _vendedores
                .Select(x => new
                {
                    x.Codigo,
                    Texto = $"{x.Codigo} - {x.Nombre}"
                })
                .ToList();

            cboVendedor.SelectedIndex = -1;
        }

        private void SeleccionarVendedor(string? codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo))
            {
                cboVendedor.SelectedIndex = -1;
                txtVendedor.Text = "";
                return;
            }

            cboVendedor.SelectedValue = codigo.Trim();
            txtVendedor.Text = codigo.Trim();
        }

        private void CargarAlmacenes()
        {
            _almacenes = _almacenRepo.ListarActivos();

            cboAlmacen.DataSource = null;
            cboAlmacen.DisplayMember = "Texto";
            cboAlmacen.ValueMember = "Codigo";
            cboAlmacen.DataSource = _almacenes
                .Select(x => new
                {
                    x.Codigo,
                    Texto = $"{x.Codigo} - {x.Nombre}"
                })
                .ToList();

            cboAlmacen.SelectedIndex = -1;
        }

        private void SeleccionarAlmacen(string? codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo))
            {
                cboAlmacen.SelectedIndex = -1;
                txtAlmacen.Text = "";
                return;
            }

            cboAlmacen.SelectedValue = codigo.Trim();
            txtAlmacen.Text = codigo.Trim();
        }

        private void SincronizarTerminoSeleccionado()
        {
            if (cboTerminoPago.SelectedValue == null)
            {
                txtTermino.Text = "";
                return;
            }

            txtTermino.Text = cboTerminoPago.SelectedValue.ToString() ?? "";
        }

        private void SeleccionarTerminoPago(string? codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo))
            {
                cboTerminoPago.SelectedIndex = -1;
                txtTermino.Text = "";
                return;
            }

            cboTerminoPago.SelectedValue = codigo.Trim();
            txtTermino.Text = codigo.Trim();
        }


        private void PrepNuevo()
        {
            Text = "Nuevo Cliente";
            txtCodigo.Text = "(auto)";
            cboTipo.SelectedIndex = 0;
            chkActivo.Checked = true;
            chkEsContribuyente.Checked = true;
            SeleccionarPais("DO");
            txtEstadoRncDgii.Text = "PENDIENTE";
            dtpFechaValidacion.Checked = false;
            SeleccionarTerminoPago("NET30");
            SeleccionarMoneda("DOP");
            AplicarReglasSegunTipoCliente();
            ActualizarEstadoFiscalUi();
            txtNombre.Focus();
        }

        private void Cargar()
        {
            var c = _repo.ObtenerPorCodigo(_codigo!);
            if (c == null) throw new InvalidOperationException("Cliente no encontrado.");

            Text = $"Editar {c.Codigo}";
            txtCodigo.Text = c.Codigo;
            txtNombre.Text = c.Nombre;
            txtRnc.Text = c.RncCedula ?? "";
            txtTel.Text = c.Telefono ?? "";
            txtEmail.Text = c.Email ?? "";
            txtDir.Text = c.Direccion ?? "";
            cboTipo.SelectedIndex = c.Tipo >= 0 && c.Tipo < cboTipo.Items.Count ? c.Tipo : 0;
            chkActivo.Checked = c.Estado == 1;



            numCreditoMaximo.Value = c.CreditoMaximo ?? 0;
            SeleccionarMoneda(c.CodDivisas);
            SeleccionarTerminoPago(c.CodTerminoPagos);
            SeleccionarVendedor(c.CodVendedor);
            SeleccionarAlmacen(c.CodAlmacen);
            numDescuentoMaximo.Value = c.DescuentoPctMax ?? 0;

            txtRazonFiscal.Text = c.RazonSocialFiscal ?? c.Nombre;
            txtNombreComercialFiscal.Text = c.NombreComercialFiscal ?? c.Nombre;
            SeleccionarComboPorValor<byte>(cboTipoIdentificacion, c.TipoIdentificacionFiscal ?? InferirTipoDocumento(c.RncCedula));
            SeleccionarProvinciaMunicipio(c.ProvinciaCodigo, c.MunicipioCodigo);
            SeleccionarPais(c.PaisCodigo ?? "DO");
            txtCorreoFiscal.Text = c.CorreoFiscal ?? c.Email ?? "";
            chkEsContribuyente.Checked = c.EsContribuyente;
            SeleccionarComboPorValor<byte>(cboTipoClienteFiscal, c.TipoClienteFiscal ?? (byte)1);
            chkValidadoDgii.Checked = c.ValidadoDGII;
            if (c.FechaValidacionDGII.HasValue)
            {
                dtpFechaValidacion.Checked = true;
                dtpFechaValidacion.Value = c.FechaValidacionDGII.Value;
            }
            else
            {
                dtpFechaValidacion.Checked = false;
            }
            txtEstadoRncDgii.Text = c.EstadoRncDGII ?? "";
            txtIdentificadorExtranjero.Text = c.IdentificadorExtranjero ?? "";
            chkEsExtranjero.Checked = c.EsExtranjero;

            AplicarReglasSegunTipoCliente();
            ActualizarEstadoFiscalUi();
        }

        private void Guardar()
        {
            try
            {
                var c = ConstruirDesdePantalla();
                var errores = ValidarCliente(c);
                if (errores.Length > 0)
                {
                    MessageBox.Show(
    "Corrige estos datos antes de guardar:\n\n" + errores,
    "Cliente incompleto",
    MessageBoxButtons.OK,
    MessageBoxIcon.Warning);
                    return;
                }

                if (_codigo == null)
                {
                    var nuevo = _repo.CrearAuto(
                        c.Nombre,
                        c.RncCedula,
                        c.Telefono,
                        c.Email,
                        c.Direccion,
                        c.CreditoMaximo,
                        c.CodDivisas,
                        c.CodTerminoPagos,
                        c.CodVendedor,
                        c.CodAlmacen);

                    c.Codigo = nuevo;
                    _repo.Actualizar(c);

                    MessageBox.Show(
    $"Cliente guardado correctamente: {nuevo}",
    "Cliente",
    MessageBoxButtons.OK,
    MessageBoxIcon.Information);

                    VolverAListadoClientes();
                }
                else
                {
                    c.Codigo = _codigo;
                    _repo.Actualizar(c);

                    MessageBox.Show(
                        $"Cliente actualizado correctamente: {_codigo}",
                        "Cliente",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    VolverAListadoClientes();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Guardar", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private EntidadCliente ConstruirDesdePantalla()
        {
            var nombre = (txtNombre.Text ?? "").Trim();
            var rnc = SoloDigitos(txtRnc.Text);
            var esExtranjero = chkEsExtranjero.Checked;
            var tipoDoc = ObtenerValorCombo<byte>(cboTipoIdentificacion);
            var validado = chkValidadoDgii.Checked;

            return new EntidadCliente
            {
                Codigo = _codigo ?? "",
                Nombre = nombre,
                RncCedula = NullIfWhite(rnc),
                Telefono = NullIfWhite(txtTel.Text),
                Email = NormalizarEmail(txtEmail.Text),
                Direccion = NullIfWhite(txtDir.Text),
                Tipo = (byte)Math.Max(cboTipo.SelectedIndex, 0),
                Estado = chkActivo.Checked ? (byte)1 : (byte)0,
                CreditoMaximo = numCreditoMaximo.Value == 0 ? null : numCreditoMaximo.Value,
                CodDivisas = cboMoneda.SelectedValue?.ToString() ?? NullIfWhite(txtDivisa.Text),
                CodVendedor = cboVendedor.SelectedValue?.ToString() ?? NullIfWhite(txtVendedor.Text),
                CodAlmacen = cboAlmacen.SelectedValue?.ToString() ?? NullIfWhite(txtAlmacen.Text),
                CodTerminoPagos = cboTerminoPago.SelectedValue?.ToString() ?? NullIfWhite(txtTermino.Text),
                DescuentoPctMax = numDescuentoMaximo.Value == 0 ? null : numDescuentoMaximo.Value,

                RazonSocialFiscal = NullIfWhite(txtRazonFiscal.Text) ?? nombre,
                NombreComercialFiscal = NullIfWhite(txtNombreComercialFiscal.Text),
                TipoIdentificacionFiscal = tipoDoc,
                ProvinciaCodigo = cboProvincia.SelectedValue?.ToString() ?? NullIfWhite(txtProvinciaCodigo.Text),
                MunicipioCodigo = cboMunicipio.SelectedValue?.ToString() ?? NullIfWhite(txtMunicipioCodigo.Text),
                PaisCodigo = cboPais.SelectedValue?.ToString() ?? NullIfWhite(txtPaisCodigo.Text) ?? "DO",
                CorreoFiscal = NormalizarEmail(txtCorreoFiscal.Text) ?? NormalizarEmail(txtEmail.Text),
                EsContribuyente = chkEsContribuyente.Checked,
                TipoClienteFiscal = ObtenerValorCombo<byte>(cboTipoClienteFiscal),
                ValidadoDGII = validado,
                FechaValidacionDGII = validado && dtpFechaValidacion.Checked ? dtpFechaValidacion.Value : validado ? DateTime.Now : null,
                EstadoRncDGII = NullIfWhite(txtEstadoRncDgii.Text),
                IdentificadorExtranjero = esExtranjero ? NullIfWhite(txtIdentificadorExtranjero.Text) : null,
                EsExtranjero = esExtranjero
            };
        }

        private StringBuilder ValidarCliente(EntidadCliente c)
        {
            var sb = new StringBuilder();

            if (string.IsNullOrWhiteSpace(c.Nombre))
                sb.AppendLine("- El nombre es obligatorio.");

            if (!string.IsNullOrWhiteSpace(c.Email) && !EsEmailValido(c.Email))
                sb.AppendLine("- El email comercial no es válido.");

            if (!string.IsNullOrWhiteSpace(c.CorreoFiscal) && !EsEmailValido(c.CorreoFiscal))
                sb.AppendLine("- El correo fiscal no es válido.");

            if (string.IsNullOrWhiteSpace(c.CodDivisas))
                sb.AppendLine("- Debes seleccionar una divisa.");

            if (string.IsNullOrWhiteSpace(c.CodTerminoPagos))
                sb.AppendLine("- Debes seleccionar un término de pago.");

            if (!c.EsExtranjero)
            {
                if (string.IsNullOrWhiteSpace(c.RncCedula))
                {
                    sb.AppendLine("- Para cliente local debes registrar RNC/Cédula.");
                }
                else
                {
                    if (c.TipoIdentificacionFiscal == 1 && c.RncCedula!.Length != 9)
                        sb.AppendLine("- Si el tipo de identificación es RNC, debe tener 9 dígitos.");

                    if (c.TipoIdentificacionFiscal == 2 && c.RncCedula!.Length != 11)
                        sb.AppendLine("- Si el tipo de identificación es Cédula, debe tener 11 dígitos.");

                    if (c.TipoIdentificacionFiscal is null or 0 && c.RncCedula!.Length is not (9 or 11))
                        sb.AppendLine("- El RNC/Cédula debe tener 9 u 11 dígitos.");
                }

                if (string.IsNullOrWhiteSpace(c.RazonSocialFiscal))
                    sb.AppendLine("- La razón social fiscal es obligatoria.");

                if (c.TipoIdentificacionFiscal is null or 0)
                    sb.AppendLine("- Debes indicar el tipo de identificación fiscal.");

                if (string.IsNullOrWhiteSpace(c.ProvinciaCodigo))
                    sb.AppendLine("- Debes seleccionar la provincia.");

                if (string.IsNullOrWhiteSpace(c.MunicipioCodigo))
                    sb.AppendLine("- Debes seleccionar el municipio.");

                if (string.IsNullOrWhiteSpace(c.PaisCodigo))
                    sb.AppendLine("- Debes indicar el país.");

                if (!c.ValidadoDGII)
                    sb.AppendLine("- Para cliente local debes validar el RNC en DGII antes de facturar e-NCF.");

                if (c.TipoIdentificacionFiscal == 3)
                    sb.AppendLine("- Para cliente local no uses tipo de identificación extranjero.");
            }
            else
            {
                if (string.IsNullOrWhiteSpace(c.IdentificadorExtranjero))
                    sb.AppendLine("- Si el cliente es extranjero debes registrar identificador extranjero.");

                if (string.IsNullOrWhiteSpace(c.PaisCodigo))
                    sb.AppendLine("- Para cliente extranjero debes indicar el país.");
            }

            if (c.ValidadoDGII)
            {
                if (c.EsExtranjero)
                    sb.AppendLine("- No debes marcar validado DGII para un cliente extranjero sin consulta local.");

                if (string.IsNullOrWhiteSpace(c.EstadoRncDGII))
                    sb.AppendLine("- Si marcas validado DGII debes guardar el estado del RNC.");
            }

            return sb;
        }

        private void ValidarConDgii()
        {
            try
            {
                if (chkEsExtranjero.Checked)
                {
                    MessageBox.Show("La validación local DGII no aplica para clientes extranjeros.", "DGII", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var documento = SoloDigitos(txtRnc.Text);
                var tipoDoc = ObtenerValorCombo<byte>(cboTipoIdentificacion);

                if (string.IsNullOrWhiteSpace(documento))
                {
                    MessageBox.Show("Debes indicar RNC o cédula para consultar la tabla local DGII.", "DGII", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtRnc.Focus();
                    return;
                }

                if (tipoDoc == 1 && documento.Length != 9)
                {
                    MessageBox.Show("Si el tipo de identificación es RNC, el documento debe tener 9 dígitos.", "DGII", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtRnc.Focus();
                    return;
                }

                if (tipoDoc == 2 && documento.Length != 11)
                {
                    MessageBox.Show("Si el tipo de identificación es Cédula, el documento debe tener 11 dígitos.", "DGII", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtRnc.Focus();
                    return;
                }

                if (tipoDoc == 3)
                {
                    MessageBox.Show("La consulta DGII no aplica para identificador extranjero.", "DGII", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                if (documento.Length is not (9 or 11))
                {
                    MessageBox.Show("El documento debe tener 9 dígitos (RNC) o 11 dígitos (Cédula).", "DGII", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtRnc.Focus();
                    return;
                }

                var dto = _dgiiRepo.BuscarActivoPorRnc(documento) ?? _dgiiRepo.BuscarPorRnc(documento)?.ToDto();
                if (dto == null)
                {
                    chkValidadoDgii.Checked = false;
                    txtEstadoRncDgii.Text = "NO ENCONTRADO";
                    dtpFechaValidacion.Checked = false;
                    ActualizarEstadoFiscalUi();
                    MessageBox.Show("No encontré ese documento en la tabla local DGII.", "DGII", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                txtRnc.Text = dto.Rnc;

                if (string.IsNullOrWhiteSpace(txtRazonFiscal.Text))
                    txtRazonFiscal.Text = dto.Nombre;

                if (string.IsNullOrWhiteSpace(txtNombre.Text))
                    txtNombre.Text = dto.Nombre;

                if (string.IsNullOrWhiteSpace(txtNombreComercialFiscal.Text))
                    txtNombreComercialFiscal.Text = dto.NombreComercial ?? dto.Nombre;

                chkValidadoDgii.Checked = true;
                txtEstadoRncDgii.Text = dto.Estado ?? "ACTIVO";
                dtpFechaValidacion.Checked = true;
                dtpFechaValidacion.Value = DateTime.Now;
                chkEsContribuyente.Checked = true;

                if (documento.Length == 9)
                    SeleccionarComboPorValor<byte>(cboTipoIdentificacion, (byte)1);
                else if (documento.Length == 11)
                    SeleccionarComboPorValor<byte>(cboTipoIdentificacion, (byte)2);

                ActualizarEstadoFiscalUi();

                MessageBox.Show("Cliente validado con la tabla local DGII.", "DGII", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "DGII", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AplicarEstadoVisualFiscal(string mensaje, System.Drawing.Color backColor, System.Drawing.Color foreColor)
        {
            txtEstadoFiscal.Text = mensaje;
            txtEstadoFiscal.BackColor = backColor;
            txtEstadoFiscal.ForeColor = foreColor;
        }

        private List<string> ObtenerFaltantesFiscales(EntidadCliente c)
        {
            var faltan = new List<string>();

            if (string.IsNullOrWhiteSpace(c.Nombre))
                faltan.Add("Nombre");

            if (string.IsNullOrWhiteSpace(c.CodDivisas))
                faltan.Add("Divisa");

            if (string.IsNullOrWhiteSpace(c.CodTerminoPagos))
                faltan.Add("Término");

            if (!c.EsExtranjero)
            {
                if (string.IsNullOrWhiteSpace(c.RncCedula))
                    faltan.Add("RNC/Cédula");

                if (c.TipoIdentificacionFiscal is null or 0)
                    faltan.Add("Tipo identificación");

                if (string.IsNullOrWhiteSpace(c.RazonSocialFiscal))
                    faltan.Add("Razón social");

                if (string.IsNullOrWhiteSpace(c.ProvinciaCodigo))
                    faltan.Add("Provincia");

                if (string.IsNullOrWhiteSpace(c.MunicipioCodigo))
                    faltan.Add("Municipio");

                if (string.IsNullOrWhiteSpace(c.PaisCodigo))
                    faltan.Add("País");

                if (!c.ValidadoDGII)
                    faltan.Add("DGII");
            }
            else
            {
                if (string.IsNullOrWhiteSpace(c.IdentificadorExtranjero))
                    faltan.Add("Identificador extranjero");

                if (string.IsNullOrWhiteSpace(c.PaisCodigo))
                    faltan.Add("País");
            }

            return faltan;
        }

        private void MarcarControl(Control control, bool invalido)
        {
            control.BackColor = invalido ? System.Drawing.Color.MistyRose : System.Drawing.SystemColors.Window;
        }

        private void MarcarCombo(ComboBox combo, bool invalido)
        {
            combo.BackColor = invalido ? System.Drawing.Color.MistyRose : System.Drawing.SystemColors.Window;
        }

        private void ResaltarCamposObligatorios(EntidadCliente c)
        {
            MarcarControl(txtNombre, string.IsNullOrWhiteSpace(c.Nombre));
            MarcarControl(txtRnc, !c.EsExtranjero && string.IsNullOrWhiteSpace(c.RncCedula));
            MarcarControl(txtRazonFiscal, !c.EsExtranjero && string.IsNullOrWhiteSpace(c.RazonSocialFiscal));
            MarcarControl(txtIdentificadorExtranjero, c.EsExtranjero && string.IsNullOrWhiteSpace(c.IdentificadorExtranjero));
            MarcarControl(txtCorreoFiscal, false);

            MarcarCombo(cboMoneda, string.IsNullOrWhiteSpace(c.CodDivisas));
            MarcarCombo(cboTerminoPago, string.IsNullOrWhiteSpace(c.CodTerminoPagos));
            MarcarCombo(cboTipoIdentificacion, !c.EsExtranjero && (c.TipoIdentificacionFiscal is null or 0));
            MarcarCombo(cboProvincia, !c.EsExtranjero && string.IsNullOrWhiteSpace(c.ProvinciaCodigo));
            MarcarCombo(cboMunicipio, !c.EsExtranjero && string.IsNullOrWhiteSpace(c.MunicipioCodigo));
            MarcarCombo(cboPais, string.IsNullOrWhiteSpace(c.PaisCodigo));

            chkValidadoDgii.BackColor = (!c.EsExtranjero && !c.ValidadoDGII)
                ? System.Drawing.Color.MistyRose
                : System.Drawing.SystemColors.Control;
        }

        private void ActualizarEstadoFiscalUi()
        {
            if (_actualizandoUi) return;

            var cliente = ConstruirDesdePantalla();

            AplicarReglasSegunTipoCliente();
            ResaltarCamposObligatorios(cliente);

            var faltantes = ObtenerFaltantesFiscales(cliente);

            if (cliente.AptoParaE31)
            {
                AplicarEstadoVisualFiscal(
                    "APTO PARA E31 / FACTURACIÓN ELECTRÓNICA",
                    System.Drawing.Color.Honeydew,
                    System.Drawing.Color.DarkGreen);
                return;
            }

            if (cliente.EsExtranjero)
            {
                var mensaje = faltantes.Count == 0
                    ? "CLIENTE EXTRANJERO / VALIDAR IDENTIFICADOR Y PAÍS"
                    : "EXTRANJERO / FALTA: " + string.Join(", ", faltantes);

                AplicarEstadoVisualFiscal(
                    mensaje,
                    System.Drawing.Color.LemonChiffon,
                    System.Drawing.Color.DarkOrange);
                return;
            }

            var textoFaltantes = faltantes.Count == 0
                ? "INCOMPLETO PARA E-NCF / REVISAR DATOS FISCALES"
                : "FALTA: " + string.Join(", ", faltantes);

            AplicarEstadoVisualFiscal(
                textoFaltantes,
                System.Drawing.Color.MistyRose,
                System.Drawing.Color.DarkRed);
        }

        private static string? NullIfWhite(string? value)
            => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

        private static string SoloDigitos(string? texto)
        {
            if (string.IsNullOrWhiteSpace(texto)) return "";
            var sb = new StringBuilder(texto.Length);
            foreach (var ch in texto)
                if (char.IsDigit(ch)) sb.Append(ch);
            return sb.ToString();
        }

        private static bool EsEmailValido(string email)
        {
            try
            {
                _ = new MailAddress(email);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static string? NormalizarEmail(string? email)
        {
            if (string.IsNullOrWhiteSpace(email)) return null;
            return email.Trim();
        }

        private static byte InferirTipoDocumento(string? documento)
        {
            var digitos = SoloDigitos(documento);
            return digitos.Length == 11 ? (byte)2 : (byte)1;
        }

        private static void SeleccionarComboPorValor<T>(ComboBox combo, T? valor) where T : struct
        {
            if (valor == null) return;
            for (var i = 0; i < combo.Items.Count; i++)
            {
                if (combo.Items[i] is ComboItem<T> item && EqualityComparer<T>.Default.Equals(item.Value, valor.Value))
                {
                    combo.SelectedIndex = i;
                    return;
                }
            }
        }

        private static T? ObtenerValorCombo<T>(ComboBox combo) where T : struct
        {
            if (combo.SelectedItem is ComboItem<T> item)
                return item.Value;
            return null;
        }

        private sealed class ComboItem<T>
        {
            public T Value { get; }
            public string Text { get; }

            public ComboItem(T value, string text)
            {
                Value = value;
                Text = text;
            }

            public override string ToString() => Text;
        }
    }

    internal static class DgiiRncEntryExtensions
    {
        public static DgiiRncEntryDto ToDto(this DgiiRncEntry entry)
        {
            return new DgiiRncEntryDto
            {
                Rnc = entry.Rnc,
                Nombre = entry.Nombre,
                NombreComercial = entry.NombreComercial,
                Estado = entry.Estado,
                Condicion = entry.Condicion
            };
        }
    }
}
