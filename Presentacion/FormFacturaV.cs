using Andloe.Data;
using Andloe.Data.DGII;
using Andloe.Entidad;
using Andloe.Logica;
using Andloe.Logica.DGII;
using Data;
using Logica;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualBasic;
using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;



namespace Andloe.Presentacion
{
    public partial class FormFacturaV : Form
    {
        private readonly FacturaRepository _facRepo = new();
        private readonly ProductoRepository _prodRepo = new();
        private readonly TerminoPagoRepository _tpRepo = new();
        private readonly DgiiRncRepository _dgiiRepo = new();
        private readonly ClienteRepository _cliRepo = new();
        private readonly VendedorRepository _vendRepo = new();
        private ClienteDto? _clienteActual;
        private string _estadoDoc = "";

        private bool _autoNuevo = false;
        private string _autoTipo = FacturaRepository.TIPO_COT;

        private int _facturaId = 0;
        private string _tipoDoc = FacturaRepository.TIPO_COT;

        // ✅ _finalizada = bloquea edición (FINALIZADA o ANULADA)
        private bool _finalizada = false;

        private bool _gridInternalUpdate = false;
        private bool _isEmbedded = false;

        private bool _warningHeaderOnce = false;
        private readonly System.Windows.Forms.Timer _clienteSearchTimer = new System.Windows.Forms.Timer();
        private bool _openingClienteSearch = false;

        private readonly System.Windows.Forms.Timer _productoSearchTimer = new System.Windows.Forms.Timer();
        private bool _openingProductoSearch = false;

        private readonly System.Windows.Forms.Timer _autoSaveTimer = new System.Windows.Forms.Timer();
        private bool _pendingAutoSave = false;
        private bool _savingNow = false;

        private bool _cargando = false;
        private bool _suppressReload = false;

        // ✅ bloqueo de FAC manual
        private bool _allowFacProgrammatic = false;
        private string _lastTipoPermitido = FacturaRepository.TIPO_COT;

        // ✅ “pro”: evita spam de logs en DataError
        private DateTime _lastGridErrorLog = DateTime.MinValue;

        // ✅ Cabecera / Comprobante Fiscal
        private bool _esComprobanteFiscal = true;

        // ✅ Consumidor Final por defecto (C-000001)
        private const string CLIENTE_CONSUMIDOR_FINAL_CODIGO = "C-000001";

        public FormFacturaV()
        {
            InitializeComponent();

            AplicarEstiloModernoFormulario();

            KeyPreview = true;
            Load += FormFacturaV_Load;
            KeyDown += FormFacturaV_KeyDown;

            EnsureGridColumns();
            ConfigureGridColumns();

            WireEvents();
            InitCombos();

            ActualizarEstadoComprobante();
            SetupAutoSave();
            AplicarModoEdicion();
            AplicarReglasUI();

            _lastTipoPermitido = GetTipoDocUI();
        }

        public FormFacturaV(int facturaId) : this()
        {
            if (facturaId > 0)
                CargarDocumentoDesdeBD(facturaId);
        }

        public void SetAutoNuevoDocumento(string tipoDocumento)
        {
            _autoNuevo = true;
            _autoTipo = (tipoDocumento ?? FacturaRepository.TIPO_COT).Trim().ToUpperInvariant();
        }

        private void FormFacturaV_Load(object? sender, EventArgs e)
        {
            _isEmbedded = !TopLevel;

            if (_facturaId <= 0 && !_finalizada && !_autoNuevo)
                NuevoDocumento(GetTipoDocUI());

            if (_autoNuevo && _facturaId <= 0)
            {
                _autoNuevo = false;
                NuevoDocumento(_autoTipo);
            }

            _lastTipoPermitido = GetTipoDocUI();

            // ✅ asegura que quede bien apenas abre
            ActualizarEstadoComprobante();
            AplicarEstadoFiscalVisual();

            grid.Focus();
        }


        protected override void OnParentChanged(EventArgs e)
        {
            base.OnParentChanged(EventArgs.Empty);
            _isEmbedded = !TopLevel;
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            try
            {
                _autoSaveTimer.Stop();
                _autoSaveTimer.Tick -= AutoSaveTimer_Tick;
            }
            catch { }

            base.OnFormClosed(e);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Enter)
            {
                if (ResolverEnterProductoEnGrid())
                    return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }


        // ============================================================
        // AUTO SAVE
        // ============================================================
        private void SetupAutoSave()
        {
            _autoSaveTimer.Interval = 900;
            _autoSaveTimer.Tick += AutoSaveTimer_Tick;

           

          
            txtDiasCredito.TextChanged += (_, __) =>
            {
                ScheduleAutoSave();
            };
            dtpFechaDoc.ValueChanged += (_, __) => ScheduleAutoSave();

            cboVendedor.SelectedValueChanged += (_, __) =>
            {
                if (_cargando) return;
                if (_finalizada) return;
                if (_facturaId <= 0) return;

                try
                {
                    _facRepo.SetCodVendedorBorrador(_facturaId, GetCodVendedorUI());
                }
                catch { }

                ScheduleAutoSave();
            };



            txtClienteNombre.TextChanged += (_, __) =>
            {
                _warningHeaderOnce = false;
                AplicarEstadoFiscalVisual();
            };

            // ✅ Regla final: E32 por defecto, E31 si hay RNC, vuelve a E32 si se borra
            txtClienteRnc.TextChanged += (_, __) =>
            {
                if (_cargando) return;
                _warningHeaderOnce = false;

                ActualizarEstadoComprobante();
                AplicarEstadoFiscalVisual();
                ScheduleAutoSave();
            };

            txtClienteDireccion.TextChanged += (_, __) =>
            {
                _warningHeaderOnce = false;
                AplicarEstadoFiscalVisual();
            };

            cboTipoComprobante.SelectedValueChanged += (_, __) =>
            {
                _warningHeaderOnce = false;
                AplicarEstadoFiscalVisual();
            };
        }
    


private void AutoSaveTimer_Tick(object? sender, EventArgs e)
        {
            _autoSaveTimer.Stop();
            if (_pendingAutoSave) AutoSaveNow();
        }

        private void ScheduleAutoSave()
        {
            if (_cargando) return;
            if (_gridInternalUpdate) return;
            if (_finalizada) return;
            if (_facturaId <= 0) return;
            if (grid.IsCurrentCellInEditMode || grid.EditingControl != null) return;

            _pendingAutoSave = true;
            _autoSaveTimer.Stop();
            _autoSaveTimer.Start();
        }

        private void AutoSaveNow()
        {
            if (_savingNow) return;
            if (_cargando) return;
            if (_gridInternalUpdate) return;
            if (_finalizada) return;
            if (_facturaId <= 0) return;
            if (grid.IsCurrentCellInEditMode || grid.EditingControl != null) return;

            try
            {
                _savingNow = true;
                _pendingAutoSave = false;

                var tipo = GetTipoDocUI();
                _facRepo.SetTipoDocumentoBorrador(_facturaId, tipo);
                _facRepo.SetFechaDocumentoBorrador(_facturaId, dtpFechaDoc.Value);

                int? terminoPagoId = null;
                int? diasCredito = null;

                if (chkCredito.Checked)
                {
                    if (cboTerminoPago.SelectedValue != null)
                        terminoPagoId = Convert.ToInt32(cboTerminoPago.SelectedValue);

                    if (int.TryParse((txtDiasCredito.Text ?? "").Trim(), out var d) && d >= 0)
                        diasCredito = d;
                }

                _facRepo.SetCreditoBorrador(_facturaId, chkCredito.Checked, terminoPagoId, diasCredito);
                _facRepo.SetFechaVencimientoBorrador(_facturaId, CalcularFechaLimitePagoUI());
                _facRepo.SetDireccionCliente(_facturaId, txtClienteDireccion.Text);

                InsertarFilasNuevasSiExisten();

                if (!_suppressReload)
                {
                    _gridInternalUpdate = true;
                    try { CargarDocumentoDesdeBD(_facturaId); }
                    finally { _gridInternalUpdate = false; }
                }
            }
            catch (Exception ex)
            {
                SafeDebugLog("AutoSaveNow", ex);
            }
            finally { _savingNow = false; }
        }

        // ============================================================
        // GRID COLUMNS
        // ============================================================
        private void EnsureGridColumns()
        {
            EnsureTextCol("colDetId", "DetId", 60, readOnly: true, visible: false, valueType: typeof(int));
            EnsureTextCol("colImpuestoId", "ImpId", 60, readOnly: true, visible: false, valueType: typeof(int));

            EnsureTextCol("colProductoCodigo", "Código", 90, valueType: typeof(string));
            EnsureTextCol("colCodBarra", "Cod. Barra", 110, valueType: typeof(string));
            EnsureTextCol("colDescripcion", "Descripción", 260, valueType: typeof(string));
            EnsureTextCol("colUnidad", "Unidad", 70, valueType: typeof(string));

            EnsureTextCol("colCantidad", "Cant.", 70, valueType: typeof(decimal));
            EnsureTextCol("colPrecio", "Precio", 90, valueType: typeof(decimal));

            EnsureTextCol("colDescuentoPct", "% Dto.", 70, valueType: typeof(decimal));
            EnsureTextCol("colDescuentoMonto", "Desc. Monto", 90, readOnly: true, valueType: typeof(decimal));

            EnsureTextCol("colItbisPct", "% ITBIS", 70, valueType: typeof(decimal));
            EnsureTextCol("colItbisMonto", "ITBIS", 90, readOnly: true, valueType: typeof(decimal));

            EnsureTextCol("colTotalLinea", "Total", 100, readOnly: true, valueType: typeof(decimal));
        }

        private void EnsureTextCol(string name, string header, int width, bool readOnly = false, bool visible = true, Type? valueType = null)
        {
            if (grid.Columns.Contains(name)) return;

            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = name,
                HeaderText = header,
                Width = width,
                ReadOnly = readOnly,
                Visible = visible,
                ValueType = valueType
            });
        }

        private void ConfigureGridColumns()
        {
            TrySetNumCol("colCantidad", "N2");
            TrySetNumCol("colPrecio", "N2");
            TrySetNumCol("colDescuentoPct", "N2");
            TrySetNumCol("colDescuentoMonto", "N2");
            TrySetNumCol("colItbisPct", "N2");
            TrySetNumCol("colItbisMonto", "N2");
            TrySetNumCol("colTotalLinea", "N2");

            grid.AllowUserToAddRows = true;
            grid.AllowUserToDeleteRows = true;
            grid.MultiSelect = false;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
        }

        private void TrySetNumCol(string colName, string format)
        {
            if (!grid.Columns.Contains(colName)) return;
            grid.Columns[colName].DefaultCellStyle.Format = format;
            grid.Columns[colName].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        }

        // ============================================================
        // ✅ DGII PRO HELPERS / VALIDATION
        // ============================================================

           private static string SoloDigitos(string? s)
        {
            if (string.IsNullOrWhiteSpace(s)) return "";
            return new string(s.Where(char.IsDigit).ToArray());
        }

        private static bool DocValidoRncOCedula(string? doc)
        {
            var d = SoloDigitos(doc);
            return d.Length == 9 || d.Length == 11; // RNC 9 / Cédula 11
        }

        // ✅ Consumidor final: sin doc
        private bool EsConsumidorFinal()
        {
            var rnc = SoloDigitos(txtClienteRnc.Text);
            return string.IsNullOrWhiteSpace(rnc);
        }

        private void ValidarDgiiProAntesDeGenerar()
        {
            if (!_esComprobanteFiscal) return;

            AplicarEstadoFiscalVisual();

            var prefijo = (Convert.ToString(cboTipoComprobante.SelectedValue) ?? "")
                .Trim().ToUpperInvariant();

            if (string.IsNullOrWhiteSpace(prefijo))
                throw new InvalidOperationException("Debe seleccionar el tipo de comprobante (E31/E32/E45).");

            if (EsConsumidorFinal())
            {
                if (prefijo != "E32")
                    throw new InvalidOperationException("Consumidor final debe facturarse con e-Factura de Consumo (E32).");
                return;
            }

            if (_clienteActual == null)
                throw new InvalidOperationException("Debe seleccionar un cliente válido antes de generar el e-CF.");

            if (prefijo == "E31" || prefijo == "E45")
            {
                if (string.IsNullOrWhiteSpace(_clienteActual.RncCedula))
                    throw new InvalidOperationException("El cliente no tiene RNC/Cédula.");

                var doc = SoloDigitos(_clienteActual.RncCedula);
                if (doc.Length is not (9 or 11))
                    throw new InvalidOperationException("El RNC/Cédula del cliente debe tener 9 o 11 dígitos.");

                if (string.IsNullOrWhiteSpace(_clienteActual.RazonSocialFiscal))
                    throw new InvalidOperationException("El cliente no tiene razón social fiscal.");

                if (!_clienteActual.EsExtranjero)
                {
                    if (string.IsNullOrWhiteSpace(_clienteActual.ProvinciaCodigo))
                        throw new InvalidOperationException("El cliente no tiene provincia fiscal.");

                    if (string.IsNullOrWhiteSpace(_clienteActual.MunicipioCodigo))
                        throw new InvalidOperationException("El cliente no tiene municipio fiscal.");

                    if (string.IsNullOrWhiteSpace(_clienteActual.PaisCodigo))
                        throw new InvalidOperationException("El cliente no tiene país fiscal.");

                    if (!_clienteActual.ValidadoDGII)
                        throw new InvalidOperationException("El cliente no está validado en DGII.");
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(_clienteActual.IdentificadorExtranjero))
                        throw new InvalidOperationException("El cliente extranjero no tiene identificador extranjero.");

                    if (string.IsNullOrWhiteSpace(_clienteActual.PaisCodigo))
                        throw new InvalidOperationException("El cliente extranjero no tiene país.");
                }

            }
        }

        private void ProcesarEcfSiAplica(int facturaId)
        {
            if (facturaId <= 0) return;
            if (!_esComprobanteFiscal) return;

            try
            {
                var cab = _facRepo.ObtenerCab(facturaId);
                if (cab == null)
                    throw new InvalidOperationException("No se pudo cargar la cabecera de la factura.");

                var prefijo = (Convert.ToString(cboTipoComprobante.SelectedValue) ?? "")
                    .Trim()
                    .ToUpperInvariant();

                if (string.IsNullOrWhiteSpace(prefijo))
                    throw new InvalidOperationException("Debe seleccionar el tipo de comprobante fiscal.");

                var tipoEcf = int.Parse(prefijo.Substring(1, 2));

                var encf = string.IsNullOrWhiteSpace(cab.ENCF)
                    ? GenerarENcfSiAplica()
                    : cab.ENCF;

                if (string.IsNullOrWhiteSpace(encf))
                    throw new InvalidOperationException("No se pudo generar el eNCF.");

                var ecfSvc = new ECFService();
                ecfSvc.GenerarXmlPendiente(facturaId, tipoEcf, encf);

                MessageBox.Show(
                    "XML e-CF generado y dejado pendiente correctamente.",
                    "e-CF",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error procesando e-CF: " + ex.Message, ex);
            }
        }

        private void EnviarAlanubeSiAplica(int facturaId)
        {
            if (facturaId <= 0) return;
            if (!_esComprobanteFiscal) return;

            try
            {
                var cab = _facRepo.ObtenerCab(facturaId);
                if (cab == null)
                    throw new InvalidOperationException("No se pudo cargar la cabecera de la factura.");

                var prefijo = (Convert.ToString(cboTipoComprobante.SelectedValue) ?? "")
                    .Trim()
                    .ToUpperInvariant();

                if (prefijo == "E34")
                    throw new InvalidOperationException("Alanube Sandbox en este flujo solo está habilitado para E31, E32 y E45. E34 no está soportado por el endpoint actual.");

                if (string.IsNullOrWhiteSpace(prefijo))
                    throw new InvalidOperationException("Debe seleccionar el tipo de comprobante fiscal.");

                if (prefijo != "E31" && prefijo != "E32" && prefijo != "E45")
                    throw new InvalidOperationException("Alanube Sandbox solo está habilitado en este flujo para E31, E32 y E45.");

                // ✅ Regla Alanube crédito fiscal
                if (prefijo == "E31")
                {
                    var esCredito = false;

                    try
                    {
                        esCredito =
                            string.Equals(cab.TipoPago, FacturaRepository.PAGO_CREDITO, StringComparison.OrdinalIgnoreCase)
                            || cab.TipoPagoECFHeader == 2;
                    }
                    catch { }

                    if (esCredito)
                    {
                        if (!cab.FechaLimitePago.HasValue)
                            throw new InvalidOperationException("La factura a crédito no tiene FechaLimitePago.");

                        if (!cab.FechaVencimientoSecuencia.HasValue)
                        {
                            // fallback defensivo: usar la fecha límite de pago
                            _facRepo.SetFechaVencimientoSecuenciaAlanube(facturaId, cab.FechaLimitePago.Value.Date);
                            cab = _facRepo.ObtenerCab(facturaId);

                            if (cab == null || !cab.FechaVencimientoSecuencia.HasValue)
                                throw new InvalidOperationException("No se pudo establecer FechaVencimientoSecuencia para Alanube.");
                        }
                    }
                }

                var alanube = new ECFAlanubeService();
                var resp = alanube.EnviarFactura(facturaId, Environment.UserName);

                var track = resp?.GetTrackOrId() ?? "";
                var status = resp?.Status ?? "";
                var legalStatus = resp?.LegalStatus ?? "";
                var mensaje = resp?.Message ?? "";

                MessageBox.Show(
                    "Factura enviada a Alanube Sandbox correctamente." +
                    Environment.NewLine +
                    $"TrackId: {track}" +
                    Environment.NewLine +
                    $"Status: {status}" +
                    Environment.NewLine +
                    $"LegalStatus: {legalStatus}" +
                    (string.IsNullOrWhiteSpace(mensaje) ? "" : Environment.NewLine + $"Mensaje: {mensaje}"),
                    "Alanube Sandbox",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error enviando a Alanube Sandbox: " + ex.Message, ex);
            }
        }

        // ============================================================
        // CABECERA VALIDATION (bloquea entrada al GRID)
        // ============================================================
        private bool HeaderOkParaProductos()
        {
            var nombre = (txtClienteNombre.Text ?? "").Trim();
            var dir = (txtClienteDireccion.Text ?? "").Trim();

            if (string.IsNullOrWhiteSpace(nombre)) return false;
            if (string.IsNullOrWhiteSpace(dir)) return false;

            if (_esComprobanteFiscal)
            {
                var prefijo = (Convert.ToString(cboTipoComprobante.SelectedValue) ?? "")
                    .Trim().ToUpperInvariant();

                if (string.IsNullOrWhiteSpace(prefijo)) return false;

                // E31/E34 requieren doc, E32 no
                var requiereDoc = (prefijo == "E31" || prefijo == "E34" || prefijo == "E45");
                if (requiereDoc && string.IsNullOrWhiteSpace(SoloDigitos(txtClienteRnc.Text)))
                    return false;
            }

            return true;
        }

        private void EnsureHeaderOkOrWarn()
        {
            if (HeaderOkParaProductos()) return;

            if (!_warningHeaderOnce)
                _warningHeaderOnce = true;

            if (string.IsNullOrWhiteSpace((txtClienteNombre.Text ?? "").Trim())) { txtClienteNombre.Focus(); return; }
            if (string.IsNullOrWhiteSpace((txtClienteDireccion.Text ?? "").Trim())) { txtClienteDireccion.Focus(); return; }

            if (_esComprobanteFiscal)
            {
                var prefijo = (Convert.ToString(cboTipoComprobante.SelectedValue) ?? "").Trim().ToUpperInvariant();
                var requiereDoc = (prefijo == "E31" || prefijo == "E34" || prefijo == "E45");

                if (requiereDoc && string.IsNullOrWhiteSpace(SoloDigitos(txtClienteRnc.Text)))
                {
                    txtClienteRnc.Focus();
                    return;
                }

                if (cboTipoComprobante.SelectedValue == null || string.IsNullOrWhiteSpace(Convert.ToString(cboTipoComprobante.SelectedValue)))
                {
                    cboTipoComprobante.Focus();
                    return;
                }
            }
        }

        // ============================================================
        // EVENTS
        // ============================================================
        private void WireEvents()
        {
            cboTipoDoc.SelectionChangeCommitted += (_, __) =>
            {
                if (_cargando) return;
                if (_finalizada) return;
                if (_facturaId <= 0) return;

                var seleccionado = GetTipoDocUI();

                if (seleccionado == FacturaRepository.TIPO_FAC && !_allowFacProgrammatic)
                {
                    _cargando = true;
                    try { cboTipoDoc.SelectedValue = _lastTipoPermitido; }
                    finally { _cargando = false; }

                    MessageBox.Show("No puedes cambiar a FACTURA (FAC) manualmente. Usa 'Registrar Factura'.",
                        "Validación", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    AplicarReglasUI();
                    AplicarEstadoFiscalVisual();
                    RefrescarEstadoBotonesAccion();
                    return;
                }

                if (seleccionado == FacturaRepository.TIPO_COT || seleccionado == FacturaRepository.TIPO_PF)
                {
                    _lastTipoPermitido = seleccionado;

                    try
                    {
                        _suppressReload = true;
                        GuardarBorrador();
                    }
                    finally
                    {
                        _suppressReload = false;
                    }

                    CargarDocumentoDesdeBD(_facturaId);

                    _pendingAutoSave = false;
                    _autoSaveTimer.Stop();
                }






                AplicarReglasUI();
                AplicarEstadoFiscalVisual();
                RefrescarEstadoBotonesAccion();
            };

            txtClienteDireccion.Leave += (_, __) =>
            {
                if (_finalizada) return;
                if (_cargando) return;
                if (_facturaId <= 0) return;

                try
                {
                    _facRepo.SetDireccionCliente(_facturaId, txtClienteDireccion.Text);
                    ScheduleAutoSave();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Dirección", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            btnNuevo.Click += (_, __) => NuevoDocumento(GetTipoDocUI());

            btnAgregar.Click += (_, __) =>
            {
                if (_finalizada) return;
                if (!HeaderOkParaProductos())
                {
                    EnsureHeaderOkOrWarn();
                    return;
                }

                grid.Focus();
                if (grid.Rows.Count > 0)
                {
                    var idx = grid.NewRowIndex;
                    if (idx >= 0 && idx < grid.Rows.Count)
                        grid.CurrentCell = grid.Rows[idx].Cells["colProductoCodigo"];
                }
            };

            btnBuscarCliente.Click += (_, __) =>
            {
                var filtro = (txtClienteBuscar.Text ?? "").Trim();

                if (string.IsNullOrWhiteSpace(filtro))
                {
                    BuscarClienteDesdeVentana(null);
                    return;
                }

                var cli = _cliRepo.BuscarPorCodigoORnc(filtro);
                if (cli != null && cli.ClienteId > 0)
                {
                    BuscarYSetCliente();
                    return;
                }

                BuscarClienteDesdeVentana(filtro);
            };

            txtClienteBuscar.KeyDown += (_, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;

                    var filtro = (txtClienteBuscar.Text ?? "").Trim();

                    if (string.IsNullOrWhiteSpace(filtro))
                    {
                        BuscarClienteDesdeVentana(null);
                        return;
                    }

                    var cli = _cliRepo.BuscarPorCodigoORnc(filtro);
                    if (cli != null && cli.ClienteId > 0)
                    {
                        BuscarYSetCliente();
                        return;
                    }

                    BuscarClienteDesdeVentana(filtro);
                }
            };

            _clienteSearchTimer.Interval = 500;
            _clienteSearchTimer.Tick += (_, __) =>
            {
                _clienteSearchTimer.Stop();

                if (_cargando || _finalizada) return;
                if (_openingClienteSearch) return;
                if (ActiveControl != txtClienteBuscar) return;

                var filtro = (txtClienteBuscar.Text ?? "").Trim();
                if (filtro.Length < 3) return;

                var cli = _cliRepo.BuscarPorCodigoORnc(filtro);
                if (cli != null && cli.ClienteId > 0) return;

                BuscarClienteDesdeVentana(filtro);
            };

            txtClienteBuscar.TextChanged += (_, __) =>
            {
                if (_cargando || _finalizada) return;
                if (_openingClienteSearch) return;

                var filtro = (txtClienteBuscar.Text ?? "").Trim();

                _clienteSearchTimer.Stop();

                if (string.IsNullOrWhiteSpace(filtro) || filtro.Length < 3)
                    return;

                _clienteSearchTimer.Start();
            };

            if (txtClienteCodigo != null)
            {
                txtClienteCodigo.KeyDown += (_, e) =>
                {
                    if (e.KeyCode == Keys.Enter)
                    {
                        e.SuppressKeyPress = true;
                        BuscarClientePorCodigoDesdeTextbox();
                    }
                };
            }
                                     
               
         

            cboTerminoPago.SelectionChangeCommitted += (_, __) =>
            {
                if (_finalizada) return;
                if (!chkCredito.Checked) return;

                AplicarDiasCreditoDesdeTerminoPago();
                ScheduleAutoSave();
            };

            btnGuardar.Click += (_, __) => GuardarBorrador();
            btnFinalizar.Click += (_, __) => FinalizarDocumento(true);
            btnRegistrarFactura.Click += (_, __) => RegistrarFactura();

            btnAnular.Click += (_, __) => AnularDocumento();

            btnEliminarLinea.Click += (_, __) => EliminarLineaSeleccionada();
            btnImprimir.Click += (_, __) => ImprimirCOTPF();
            btnCerrar.Click += (_, __) => Close();

            chkCredito.CheckedChanged += (_, __) =>
            {
                if (_finalizada) return;

                cboTerminoPago.Enabled = chkCredito.Checked;
                txtDiasCredito.Enabled = chkCredito.Checked;

                if (chkCredito.Checked)
                {
                    if (cboTerminoPago.Items.Count > 0 && cboTerminoPago.SelectedIndex < 0)
                        cboTerminoPago.SelectedIndex = 0;

                    AplicarDiasCreditoDesdeTerminoPago();

                    if (!int.TryParse((txtDiasCredito.Text ?? "").Trim(), out var dias) || dias <= 0)
                        txtDiasCredito.Text = "1";
                }
                else
                {
                    txtDiasCredito.Text = "";
                }

                AplicarReglasUI();
                ScheduleAutoSave();
            };

            txtDiasCredito.Leave += (_, __) =>
            {
                if (_finalizada) return;
                if (!chkCredito.Checked) return;

                if (!int.TryParse((txtDiasCredito.Text ?? "").Trim(), out var dias) || dias <= 0)
                {
                    MessageBox.Show(
                        "Los días de crédito deben ser mayores que cero.",
                        "Crédito",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    AplicarDiasCreditoDesdeTerminoPago();
                    txtDiasCredito.Focus();
                    txtDiasCredito.SelectAll();
                }
            };

            grid.CurrentCellDirtyStateChanged += (_, __) =>
            {
                if (grid.IsCurrentCellDirty)
                    grid.CommitEdit(DataGridViewDataErrorContexts.Commit);
            };

                      
            grid.CellBeginEdit += (_, e) =>
            {
                if (_finalizada) { e.Cancel = true; return; }
                if (e.RowIndex < 0) return;

                if (!HeaderOkParaProductos())
                {
                    e.Cancel = true;
                    EnsureHeaderOkOrWarn();
                    return;
                }

                var r = grid.Rows[e.RowIndex];
                if (r.IsNewRow) return;

                var detId = GetDetId(e.RowIndex);
                var colName = grid.Columns[e.ColumnIndex].Name;

                if ((colName == "colProductoCodigo" || colName == "colCodBarra") && detId > 0)
                {
                    e.Cancel = true;
                    return;
                }

                if (colName == "colItbisMonto" || colName == "colTotalLinea" || colName == "colDescuentoMonto")
                {
                    e.Cancel = true;
                    return;
                }
            };

            grid.CellEndEdit += (_, e) =>
            {
                if (_finalizada) return;
                if (_gridInternalUpdate) return;
                if (_cargando) return;
                if (e.RowIndex < 0) return;
                if (grid.Rows[e.RowIndex].IsNewRow) return;

                try
                {
                    HandleRowEdited(e.RowIndex);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Editar", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            grid.UserDeletingRow += (_, e) =>
            {
                var detId = GetDetId(e.Row.Index);
                if (detId > 0)
                {
                    try
                    {
                        _facRepo.DeleteLinea(detId);

                        try
                        {
                            _suppressReload = true;
                            ScheduleAutoSave();
                        }
                        finally { _suppressReload = false; }

                        CargarDocumentoDesdeBD(_facturaId);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Eliminar", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        e.Cancel = true;
                    }
                }
            };

            grid.DataError += (_, e) =>
            {
                if ((DateTime.Now - _lastGridErrorLog).TotalSeconds > 2)
                {
                    _lastGridErrorLog = DateTime.Now;
                    SafeDebugLog("Grid.DataError", e.Exception);
                }
                e.ThrowException = false;
            };
        }

        private void HandleRowEdited(int rowIndex)
        {
            if (_finalizada) return;
            if (_gridInternalUpdate) return;
            if (_cargando) return;

            _suppressReload = true;
            try
            {
                TryResolveProductoEnFilaNueva(rowIndex);
                ValidateRowValues(rowIndex);
                RecalcularFilaUI(rowIndex);

                if (GetDetId(rowIndex) > 0)
                    UpsertFilaBD(rowIndex);

                RecalcularTotalesDesdeGridUI();
                ScheduleAutoSave();
            }
            finally
            {
                _suppressReload = false;
            }
        }

        

        private void ValidateRowValues(int rowIndex)
        {
            if (rowIndex < 0 || rowIndex >= grid.Rows.Count) return;
            var r = grid.Rows[rowIndex];
            if (r == null || r.IsNewRow) return;

            var cant = GetCellDecimal(r, "colCantidad");
            var precio = GetCellDecimal(r, "colPrecio");

            if (cant < 0m) throw new Exception("La cantidad no puede ser negativa.");
            if (precio < 0m) throw new Exception("El precio no puede ser negativo.");

            var itbisPct = GetCellDecimal(r, "colItbisPct");
            var descPct = GetCellDecimal(r, "colDescuentoPct");

            if (itbisPct < 0m) r.Cells["colItbisPct"].Value = 0m;
            if (descPct < 0m) r.Cells["colDescuentoPct"].Value = 0m;
        }

        // ✅ FIX CENTRAL (FINAL): usa el controlador central
        private void AplicarTipoComprobanteSegunDocumento()
        {
            if (!_esComprobanteFiscal) return;
            ActualizarEstadoComprobante();
        }

        private void SafeDebugLog(string area, Exception? ex)
        {
            try
            {
                var msg = ex == null ? "(sin excepción)" : ex.ToString();
                Debug.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {area}: {msg}");
            }
            catch { }
        }

        private System.Collections.Generic.List<string> ObtenerFaltantesFiscalesFactura()
        {
            var faltan = new System.Collections.Generic.List<string>();

            var prefijo = (Convert.ToString(cboTipoComprobante.SelectedValue) ?? "").Trim().ToUpperInvariant();
            var doc = SoloDigitos(txtClienteRnc.Text);

            if (string.IsNullOrWhiteSpace((txtClienteNombre.Text ?? "").Trim()))
                faltan.Add("Cliente");

            if (string.IsNullOrWhiteSpace((txtClienteDireccion.Text ?? "").Trim()))
                faltan.Add("Dirección");

            if (string.IsNullOrWhiteSpace(prefijo))
                faltan.Add("Tipo comprobante");

            if (EsConsumidorFinal())
            {
                if (prefijo != "E32")
                    faltan.Add("E32");
                return faltan;
            }

            if (_clienteActual == null)
            {
                faltan.Add("Cliente fiscal");
                return faltan;
            }

            if (doc.Length is not (9 or 11))
                faltan.Add("RNC/Cédula");

            if (string.IsNullOrWhiteSpace(_clienteActual.RazonSocialFiscal))
                faltan.Add("Razón social");

            if (!_clienteActual.EsExtranjero)
            {
                if (string.IsNullOrWhiteSpace(_clienteActual.ProvinciaCodigo))
                    faltan.Add("Provincia");

                if (string.IsNullOrWhiteSpace(_clienteActual.MunicipioCodigo))
                    faltan.Add("Municipio");

                if (string.IsNullOrWhiteSpace(_clienteActual.PaisCodigo))
                    faltan.Add("País");

                if (!_clienteActual.ValidadoDGII)
                    faltan.Add("DGII");
            }
            else
            {
                if (string.IsNullOrWhiteSpace(_clienteActual.IdentificadorExtranjero))
                    faltan.Add("Identificador extranjero");

                if (string.IsNullOrWhiteSpace(_clienteActual.PaisCodigo))
                    faltan.Add("País");
            }

            return faltan;
        }

        private bool ClienteAptoParaEcf()
        {
            return ObtenerFaltantesFiscalesFactura().Count == 0;
        }

        private void AplicarEstadoFiscalVisual()
        {
            if (txtEstadoFiscal == null) return;

            var faltan = ObtenerFaltantesFiscalesFactura();
            var prefijo = (Convert.ToString(cboTipoComprobante.SelectedValue) ?? "").Trim().ToUpperInvariant();

            if (EsConsumidorFinal())
            {
                txtEstadoFiscal.Text = prefijo == "E32"
                    ? "CONSUMIDOR FINAL / APTO PARA E32"
                    : "CONSUMIDOR FINAL / DEBE USAR E32";

                txtEstadoFiscal.BackColor = prefijo == "E32"
                    ? Color.LemonChiffon
                    : Color.MistyRose;

                txtEstadoFiscal.ForeColor = prefijo == "E32"
                    ? Color.DarkOrange
                    : Color.DarkRed;

                return;
            }

            if (faltan.Count == 0)
            {
                txtEstadoFiscal.Text = $"CLIENTE APTO PARA {prefijo}";
                txtEstadoFiscal.BackColor = Color.Honeydew;
                txtEstadoFiscal.ForeColor = Color.DarkGreen;
                return;
            }

            txtEstadoFiscal.Text = "FALTA: " + string.Join(", ", faltan);
            txtEstadoFiscal.BackColor = Color.MistyRose;
            txtEstadoFiscal.ForeColor = Color.DarkRed;
        }

        // ============================================================
        // UI RULES
        // ============================================================
        private string GetTipoDocUI()
        {
            var tipo = Convert.ToString(cboTipoDoc.SelectedValue) ?? FacturaRepository.TIPO_COT;
            return tipo.Trim().ToUpperInvariant();
        }

        private string GetEstadoUI()
        {
            return (_estadoDoc ?? "").Trim().ToUpperInvariant();
        }


        // ============================================================
        // VENDEDOR UI
        // ============================================================
        private string? GetCodVendedorUI()
        {
            try
            {
                var v = Convert.ToString(cboVendedor?.SelectedValue) ?? "";
                v = v.Trim();
                return string.IsNullOrWhiteSpace(v) ? null : v;
            }
            catch { return null; }
        }

        private void SetVendedorUI(string? codVendedor)
        {
            if (cboVendedor == null) return;

            var cod = (codVendedor ?? "").Trim();
            if (string.IsNullOrWhiteSpace(cod))
            {
                try { cboVendedor.SelectedValue = ""; } catch { }
                return;
            }

            try
            {
                cboVendedor.SelectedValue = cod;
            }
            catch
            {
                try { cboVendedor.SelectedValue = ""; } catch { }
            }
        }

        private void SetVendedorSiVacio(string? codVendedor)
        {
            if (_cargando) return;
            var actual = GetCodVendedorUI();
            if (!string.IsNullOrWhiteSpace(actual)) return;
            SetVendedorUI(codVendedor);
        }

        private void SetClienteCodigoUI(string? codigo)
        {
            if (txtClienteCodigo == null) return;
            txtClienteCodigo.Text = (codigo ?? "").Trim();
        }

        private void BuscarClientePorCodigoDesdeTextbox()
        {
            var codigo = (txtClienteCodigo.Text ?? "").Trim();
            if (string.IsNullOrWhiteSpace(codigo)) return;

            txtClienteBuscar.Text = codigo;
            BuscarYSetCliente();
        }



        private void AplicarReglasUI()
        {
            if (btnRegistrarFactura == null || btnImprimir == null || cboTipoDoc == null) return;

            var tipo = GetTipoDocUI();

            cboTipoDoc.Enabled = !_finalizada;

            btnImprimir.Enabled = (_facturaId > 0) && (tipo == FacturaRepository.TIPO_COT || tipo == FacturaRepository.TIPO_PF);

            btnRegistrarFactura.Enabled = (_facturaId > 0) && (!_finalizada || tipo == FacturaRepository.TIPO_FAC);

            btnRegistrarFactura.Text = (_finalizada && tipo == FacturaRepository.TIPO_FAC)
                ? "Imprimir Factura"
                : "Registrar Factura";

            RefrescarEstadoBotonesAccion();
        }

        private void AplicarDiasCreditoDesdeTerminoPago()
        {
            if (cboTerminoPago == null) return;
            if (cboTerminoPago.SelectedItem == null) return;

            try
            {
                var propDias = cboTerminoPago.SelectedItem.GetType().GetProperty("Dias");
                if (propDias == null) return;

                var valor = propDias.GetValue(cboTerminoPago.SelectedItem, null);
                if (valor == null) return;

                var dias = Convert.ToInt32(valor);
                txtDiasCredito.Text = dias.ToString();
            }
            catch
            {
                // no romper flujo si el datasource cambia
            }
        }

        private void AplicarCondicionPagoDesdeCliente(ClienteDto? cliente)
        {
            if (cliente == null || !cliente.TerminoPagoId.HasValue || cliente.TerminoPagoId.Value <= 0)
            {
                chkCredito.Checked = false;
                cboTerminoPago.SelectedIndex = -1;
                cboTerminoPago.Enabled = false;
                txtDiasCredito.Enabled = false;
                txtDiasCredito.Text = "";
                return;
            }

            var terminoPagoId = cliente.TerminoPagoId.Value;

            var itemExiste = false;
            foreach (var item in cboTerminoPago.Items)
            {
                var prop = item.GetType().GetProperty("Value");
                if (prop == null) continue;

                var val = prop.GetValue(item, null);
                if (val != null && Convert.ToInt32(val) == terminoPagoId)
                {
                    itemExiste = true;
                    break;
                }
            }

            if (!itemExiste)
            {
                chkCredito.Checked = false;
                cboTerminoPago.SelectedIndex = -1;
                cboTerminoPago.Enabled = false;
                txtDiasCredito.Enabled = false;
                txtDiasCredito.Text = "";
                return;
            }

            cboTerminoPago.SelectedValue = terminoPagoId;
            chkCredito.Checked = true;
            cboTerminoPago.Enabled = !_finalizada;
            txtDiasCredito.Enabled = !_finalizada;

            AplicarDiasCreditoDesdeTerminoPago();

            if (string.IsNullOrWhiteSpace(txtDiasCredito.Text) || txtDiasCredito.Text == "0")
                txtDiasCredito.Text = "1";
        }

        private DateTime? CalcularFechaLimitePagoUI()
        {
            if (!chkCredito.Checked) return null;

            if (!int.TryParse((txtDiasCredito.Text ?? "").Trim(), out var dias))
                dias = 0;

            return dtpFechaDoc.Value.Date.AddDays(dias);
        }

        private void ValidarCreditoAntesDeGuardar()
        {
            if (!chkCredito.Checked) return;

            if (cboTerminoPago.SelectedValue == null)
                throw new InvalidOperationException("Debe seleccionar un término de pago para facturas a crédito.");

            if (!int.TryParse((txtDiasCredito.Text ?? "").Trim(), out var dias))
                throw new InvalidOperationException("Debe indicar los días de crédito.");

            if (dias <= 0)
                throw new InvalidOperationException("Los días de crédito deben ser mayores que cero.");

            var fechaDoc = dtpFechaDoc.Value.Date;
            var fechaLimite = fechaDoc.AddDays(dias);

            if (fechaLimite.Date < fechaDoc)
                throw new InvalidOperationException("La fecha límite de pago no puede ser menor que la fecha del documento.");
        }

        private void RefrescarEstadoBotonesAccion()
        {
            try
            {
                var tipo = GetTipoDocUI();
                var estado = GetEstadoUI();

                var habilitarAnular =
                    _facturaId > 0
                    && tipo == FacturaRepository.TIPO_FAC
                    && string.Equals(estado, FacturaRepository.EST_FINALIZADA, StringComparison.OrdinalIgnoreCase);

                btnAnular.Enabled = habilitarAnular;
            }
            catch
            {
                btnAnular.Enabled = false;
            }
        }

        // ============================================================
        // REPORT SERVICE
        // ============================================================
        private Andloe.Logica.Facturacion.FacturaReportService BuildFacturaReportService()
        {
            var repLogger = NullLogger<Andloe.Logica.ReporteService>.Instance;
            var repSvc = new Andloe.Logica.ReporteService(new Andloe.Data.ReporteRepository(), repLogger);

            var dsRepo = new Andloe.Data.ReporteFacturacionRepository();
            Andloe.Logica.IPrintPreviewService preview = new Andloe.Presentacion.Impresion.RdlcPrintService();

            var logFactura = NullLogger<Andloe.Logica.Facturacion.FacturaReportService>.Instance;
            return new Andloe.Logica.Facturacion.FacturaReportService(repSvc, dsRepo, preview, logFactura);
        }

        private static string GetReporteCodigoPorTipo(string tipoDoc)
        {
            tipoDoc = (tipoDoc ?? FacturaRepository.TIPO_COT).Trim().ToUpperInvariant();

            return tipoDoc switch
            {
                "COT" => "COTIZACION_RI",
                "PF" => "PROFORMA_RI",
                "FAC" => "FACTURA_RI",
                _ => "COTIZACION_RI"
            };
        }

        // ============================================================
        // IMPRESION
        // ============================================================
        private void ImprimirCOTPF()
        {
            if (_facturaId <= 0)
            {
                MessageBox.Show("No hay documento para imprimir.", "Imprimir", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var tipo = GetTipoDocUI();
            if (tipo == FacturaRepository.TIPO_FAC)
            {
                MessageBox.Show("Para Factura usa el botón REGISTRAR FACTURA.", "Imprimir", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var codigoReporte = GetReporteCodigoPorTipo(tipo);

            try
            {
                var svc = BuildFacturaReportService();
                svc.ImprimirFactura("VENTA", codigoReporte, _facturaId);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Imprimir", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ImprimirFacturaRI()
        {
            try
            {
                var svc = BuildFacturaReportService();
                svc.ImprimirFactura("VENTA", "FACTURA_RI", _facturaId);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Imprimir Factura", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ============================================================
        // ✅ REGISTRAR FACTURA
        // ============================================================
        private void RegistrarFactura()
        {
            if (_facturaId <= 0)
            {
                MessageBox.Show("Primero crea/abre un documento.", "Registrar", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var tipoActual = GetTipoDocUI();

            // 🖨️ Si ya es factura finalizada → solo imprimir
            if (_finalizada && tipoActual == FacturaRepository.TIPO_FAC)
            {
                ImprimirFacturaRI();
                return;
            }

            try
            {
               
                // 🔹 1. Validar crédito y guardar cambios pendientes
                ValidarCreditoAntesDeGuardar();

                _suppressReload = true;
                try { GuardarBorrador(); }
                finally { _suppressReload = false; }

                // 🔹 2. VALIDACIÓN DGII ANTES DE CONVERTIR
                try
                {
                    ValidarDgiiProAntesDeGenerar();
                }
                catch (Exception ex)
                {
                    AplicarEstadoFiscalVisual();

                    MessageBox.Show(
                        "No se puede registrar la factura.\n\n" + ex.Message,
                        "Validación fiscal",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    return;
                }

                // 🔹 3. Convertir a FACTURA (FAC)
                if (tipoActual != FacturaRepository.TIPO_FAC)
                {
                    var nuevoNumero = _facRepo.ConvertirCotPfAFac_GenerandoNuevoNumero(
                        _facturaId,
                        Environment.UserName);

                    _allowFacProgrammatic = true;
                    _cargando = true;
                    try { cboTipoDoc.SelectedValue = FacturaRepository.TIPO_FAC; }
                    finally
                    {
                        _cargando = false;
                        _allowFacProgrammatic = false;
                    }

                    _tipoDoc = FacturaRepository.TIPO_FAC;

                    if (!string.IsNullOrWhiteSpace(nuevoNumero))
                        txtNumeroFacturaTop.Text = nuevoNumero;
                }

                // 🔹 4. Finalizar documento
                FinalizarDocumento(false);

                // 🔹 5. Guardar cabecera fiscal
                GuardarCabeceraFiscalBorrador();

                // 🔹 6. Generar XML e-CF
                try
                {
                    ProcesarEcfSiAplica(_facturaId);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        "Factura registrada, pero falló la generación del e-CF.\n\n" + ex.Message,
                        "e-CF",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    ImprimirFacturaRI();
                    return;
                }

                // 🔹 7. Enviar a Alanube
                try
                {
                    EnviarAlanubeSiAplica(_facturaId);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        "Factura registrada y XML generado, pero falló el envío a Alanube.\n\n" + ex.Message,
                        "Alanube",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    ImprimirFacturaRI();
                    return;
                }

                // 🔹 8. Éxito total
                MessageBox.Show(
                    "Factura electrónica generada y enviada correctamente.",
                    "Factura",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                ImprimirFacturaRI();
            }
            catch (Exception ex)
            {
                AplicarEstadoFiscalVisual();

                MessageBox.Show(
                    "Error general al registrar la factura.\n\n" + ex.Message,
                    "Factura",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
        private string? GenerarENcfSiAplica()
        {
            if (!_esComprobanteFiscal) return null;

            var tipoDoc = GetTipoDocUI();
            if (!string.Equals(tipoDoc, FacturaRepository.TIPO_FAC, StringComparison.OrdinalIgnoreCase))
                return null;

            ValidarDgiiProAntesDeGenerar();

            var prefijo = (Convert.ToString(cboTipoComprobante.SelectedValue) ?? "")
                .Trim()
                .ToUpperInvariant();

            if (prefijo.Length != 3 || prefijo[0] != 'E' || !char.IsDigit(prefijo[1]) || !char.IsDigit(prefijo[2]))
                throw new InvalidOperationException("Tipo de comprobante inválido. Formato esperado: E31/E32/E34.");

            var tipoId = int.Parse(prefijo.Substring(1, 2));
            var s = SesionService.Current;
            var cajaId = s.CajaId ?? 0;

            var repo = new ECFSqlRepository();

            return repo.GenerarENcf(
                empresaId: s.EmpresaId,
                sucursalId: s.SucursalId,
                cajaId: cajaId,
                facturaId: _facturaId,
                tipoEcf: tipoId,
                prefijo: prefijo
            );
        }

        // ============================================================
        // ✅ ANULAR FACTURA (FAC FINALIZADA)
        // ============================================================
        private void AnularDocumento()
        {
            try
            {
                if (_facturaId <= 0)
                {
                    MessageBox.Show("No hay factura cargada.", "Anular", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var tipo = GetTipoDocUI();
                if (tipo != FacturaRepository.TIPO_FAC)
                {
                    MessageBox.Show("Solo se puede anular una FACTURA (FAC).", "Anular", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var estado = GetEstadoUI();
                if (!string.Equals(estado, FacturaRepository.EST_FINALIZADA, StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show("Solo se puede anular una factura FINALIZADA.", "Anular", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var motivo = Interaction.InputBox("Motivo de anulación:", "Anular factura", "Error de facturación");
                if (string.IsNullOrWhiteSpace(motivo))
                    return;

                var ok = MessageBox.Show(
                    "¿Seguro que deseas ANULAR esta factura?\nEsto hará reverso de inventario.",
                    "Confirmar anulación",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (ok != DialogResult.Yes) return;

                _facRepo.AnularFactura(_facturaId, Environment.UserName, motivo);

                try
                {
                    new AuditoriaService().Log(
                        modulo: "FACTURACION",
                        accion: "ANULAR",
                        entidad: "FacturaCab",
                        entidadId: _facturaId.ToString(),
                        detalle: $"Factura anulada desde FormFacturaV. Motivo: {motivo}"
                    );
                }
                catch { }

                MessageBox.Show("Factura ANULADA correctamente.", "Anular", MessageBoxButtons.OK, MessageBoxIcon.Information);

                CargarDocumentoDesdeBD(_facturaId);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Anular", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ============================================================
        // COMBOS
        // ============================================================
        private void InitCombos()
        {
            cboTipoDoc.DisplayMember = "Text";
            cboTipoDoc.ValueMember = "Value";
            cboTipoDoc.DataSource = new[]
            {
        new { Text = "Cotización",        Value = FacturaRepository.TIPO_COT },
        new { Text = "Factura Pro-Forma", Value = FacturaRepository.TIPO_PF  },
        new { Text = "Factura",           Value = FacturaRepository.TIPO_FAC }
    }.ToList();

            cboTipoDoc.SelectedValue = FacturaRepository.TIPO_COT;

            var tps = _tpRepo.ListarActivos();

            var lista = tps
                .Select(x => new
                {
                    Text = x.Descripcion,
                    Value = x.TerminoPagoId,
                    Dias = x.DiasPlazo
                })
                .ToList();

            cboTerminoPago.DisplayMember = "Text";
            cboTerminoPago.ValueMember = "Value";
            cboTerminoPago.DataSource = lista;

            if (lista.Count > 0)
                cboTerminoPago.SelectedIndex = 0;

            cboTerminoPago.Enabled = false;
            txtDiasCredito.Enabled = false;

            // ✅ Tipos de comprobante basados en tu BD
            cboTipoComprobante.DisplayMember = "Text";
            cboTipoComprobante.ValueMember = "Value";
            cboTipoComprobante.DataSource = new[]
            {
        new { Text = "e-Factura de Crédito Fiscal", Value = "E31" },
        new { Text = "e-Factura de Consumo",        Value = "E32" },
        new { Text = "e-Comprobante Gubernamental", Value = "E45" },
    }.ToList();

            cboTipoComprobante.SelectedValue = "E32";
            cboTipoComprobante.Enabled = false;

            var vendedores = _vendRepo.Listar(null, top: 500, incluirInactivos: false);
            var venDs = vendedores
                .Select(v => new { Text = $"{v.Codigo} - {v.Nombre}", Value = v.Codigo })
                .ToList();

            venDs.Insert(0, new { Text = "(Sin vendedor)", Value = "" });

            cboVendedor.DisplayMember = "Text";
            cboVendedor.ValueMember = "Value";
            cboVendedor.DataSource = venDs;
            cboVendedor.SelectedValue = "";
            cboVendedor.Enabled = true;
        }

        // ✅ CONTROL CENTRAL: habilita/deshabilita combo según RNC
        private void ActualizarEstadoComprobante()
        {
            if (_cargando) return;

            var doc = SoloDigitos(txtClienteRnc.Text);

            // Sin doc => Consumidor Final (E32) bloqueado
            if (string.IsNullOrWhiteSpace(doc))
            {
                if (cboTipoComprobante.SelectedValue == null ||
                    !string.Equals(Convert.ToString(cboTipoComprobante.SelectedValue), "E32", StringComparison.OrdinalIgnoreCase))
                {
                    cboTipoComprobante.SelectedValue = "E32";
                }

                cboTipoComprobante.Enabled = false;
                return;
            }

            // Con doc válido => habilitar combo + por defecto E31
            if (doc.Length == 9 || doc.Length == 11)
            {
                if (cboTipoComprobante.SelectedValue == null ||
                    string.Equals(Convert.ToString(cboTipoComprobante.SelectedValue), "E32", StringComparison.OrdinalIgnoreCase))
                {
                    cboTipoComprobante.SelectedValue = "E31";
                }

                cboTipoComprobante.Enabled = !_finalizada; // ✅ AQUÍ está la habilitación real
                return;
            }

            // Doc inválido => no habilitar
            cboTipoComprobante.Enabled = false;
        }

        // ============================================================
        // NUEVO
        // ============================================================
        public void NuevoDocumento(string tipoDocumento)
        {
            try
            {
                _finalizada = false;
                _tipoDoc = (tipoDocumento ?? FacturaRepository.TIPO_COT).Trim().ToUpperInvariant();

                var s = SesionService.Current;

                _facturaId = _facRepo.CrearBorrador(
                    tipoDocumento: _tipoDoc,
                    usuarioCreacion: Environment.UserName,
                    empresaId: s.EmpresaId,
                    sucursalId: s.SucursalId,
                    almacenId: s.AlmacenId
                );

                // ✅ Consumidor Final por defecto SIEMPRE con ClienteId real
                try
                {
                    var cf = _cliRepo.ObtenerConsumidorFinal();
                    _clienteActual = cf;

                    _facRepo.SetCliente(_facturaId, cf.ClienteId, cf.Nombre ?? "CONSUMIDOR FINAL", null);

                    txtClienteNombre.Text = cf.Nombre ?? "CONSUMIDOR FINAL";
                    txtClienteRnc.Text = "";
                    txtClienteDireccion.Text = "";
                    SetClienteCodigoUI(cf.Codigo);

                    chkCredito.Checked = false;
                    cboTerminoPago.Enabled = false;
                    txtDiasCredito.Enabled = false;
                    txtDiasCredito.Text = "";
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Consumidor Final", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    _clienteActual = null;

                    _facRepo.SetCliente(_facturaId, null, "CONSUMIDOR FINAL", null);
                    txtClienteNombre.Text = "CONSUMIDOR FINAL";
                    txtClienteRnc.Text = "";
                    txtClienteDireccion.Text = "";
                    SetClienteCodigoUI(CLIENTE_CONSUMIDOR_FINAL_CODIGO);

                    chkCredito.Checked = false;
                    cboTerminoPago.Enabled = false;
                    txtDiasCredito.Enabled = false;
                    txtDiasCredito.Text = "";
                }

                // ✅ Reaplica regla
                ActualizarEstadoComprobante();

                CargarDocumentoDesdeBD(_facturaId);

                txtClienteBuscar.Clear();

                _warningHeaderOnce = false;

                grid.Focus();

                _lastTipoPermitido = GetTipoDocUI();
                AplicarEstadoFiscalVisual();
                ScheduleAutoSave();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Nuevo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ============================================================
        // CARGAR
        // ============================================================
        private void CargarDocumentoDesdeBD(int facturaId)
        {
                      
           
            if (facturaId <= 0) return;

            var dto = _facRepo.ObtenerCompleta(facturaId);
            if (dto == null) throw new Exception("No se pudo cargar el documento.");

            _cargando = true;
            try
            {
                _facturaId = dto.Cab.FacturaId;
                _tipoDoc = (dto.Cab.TipoDocumento ?? FacturaRepository.TIPO_COT).Trim().ToUpperInvariant();

                txtFacturaIdInfo.Text = _facturaId.ToString();

                txtNumeroFacturaTop.Text = string.IsNullOrWhiteSpace(dto.Cab.NumeroDocumento) ? "(sin asignar)" : dto.Cab.NumeroDocumento;

                _estadoDoc = dto.Cab.Estado ?? "";
                txtEstadoInfo.Text = _estadoDoc;
                txtTipoInfo.Text = _tipoDoc ?? "";

                txtClienteNombre.Text = dto.Cab.NombreCliente ?? "";
                txtClienteDireccion.Text = dto.Cab.DireccionCliente ?? "";
                txtClienteRnc.Text = dto.Cab.DocumentoCliente ?? "";



                
                _clienteActual = null;
                SetClienteCodigoUI(null);

                var docCliente = (dto.Cab.DocumentoCliente ?? "").Trim();
                if (!string.IsNullOrWhiteSpace(docCliente))
                {
                    try
                    {
                        _clienteActual = _cliRepo.BuscarPorCodigoORnc(docCliente);
                        SetClienteCodigoUI(_clienteActual?.Codigo);
                    }
                    catch { }
                }

                var f = dto.Cab.FechaDocumento;
                if (f < dtpFechaDoc.MinDate) f = DateTime.Today;
                if (f > dtpFechaDoc.MaxDate) f = DateTime.Today;
                dtpFechaDoc.Value = f;

                try
                {
                    var fr = dto.Cab.FechaDocumento;

                    if (fr < dtpFechaRegistro.MinDate)
                        fr = dtpFechaRegistro.MinDate;

                    if (fr > dtpFechaRegistro.MaxDate)
                        fr = dtpFechaRegistro.MaxDate;

                    dtpFechaRegistro.Value = fr;
                }
                catch
                {
                    try
                    {
                        dtpFechaRegistro.Value = DateTime.Today;
                    }
                    catch { }
                }





                _finalizada =
                    string.Equals(dto.Cab.Estado, FacturaRepository.EST_FINALIZADA, StringComparison.OrdinalIgnoreCase)
                    || string.Equals(dto.Cab.Estado, "ANULADA", StringComparison.OrdinalIgnoreCase);

                _allowFacProgrammatic = true;
                try { cboTipoDoc.SelectedValue = _tipoDoc; }
                finally { _allowFacProgrammatic = false; }

                if (_tipoDoc != FacturaRepository.TIPO_FAC)
                    _lastTipoPermitido = _tipoDoc;

                SetTotales(dto.Cab.SubTotal, dto.Cab.TotalDescuento, dto.Cab.TotalImpuesto, dto.Cab.TotalGeneral);

                var esCredito = string.Equals(dto.Cab.TipoPago, FacturaRepository.PAGO_CREDITO, StringComparison.OrdinalIgnoreCase);
                chkCredito.Checked = esCredito;

                cboTerminoPago.Enabled = !_finalizada && esCredito;
                txtDiasCredito.Enabled = !_finalizada && esCredito;

                if (dto.Cab.TerminoPagoId.HasValue)
                    cboTerminoPago.SelectedValue = dto.Cab.TerminoPagoId.Value;

                txtDiasCredito.Text = dto.Cab.DiasCredito?.ToString() ?? "";

                // ✅ vendedor asignado
                SetVendedorUI(dto.Cab.CodVendedor);
                cboVendedor.Enabled = !_finalizada;

                // ✅ regla final aplicada al cargar (HABILITA si hay doc)
                ActualizarEstadoComprobante();

                _gridInternalUpdate = true;
                try
                {
                    grid.Rows.Clear();

                    foreach (var d in dto.Det)
                    {
                        var idx = grid.Rows.Add();
                        var r = grid.Rows[idx];

                        r.Cells["colDetId"].Value = d.FacturaDetId;
                        r.Cells["colImpuestoId"].Value = d.ImpuestoId;

                        r.Cells["colProductoCodigo"].Value = d.ProductoCodigo ?? "";
                        r.Cells["colCodBarra"].Value = d.CodBarra ?? "";
                        r.Cells["colDescripcion"].Value = d.Descripcion ?? "";
                        r.Cells["colUnidad"].Value = d.Unidad ?? "";

                        r.Cells["colCantidad"].Value = Convert.ToDecimal(d.Cantidad);
                        r.Cells["colPrecio"].Value = Convert.ToDecimal(d.Precio);

                        r.Cells["colDescuentoPct"].Value = Convert.ToDecimal(d.DescuentoPct);
                        r.Cells["colDescuentoMonto"].Value = Convert.ToDecimal(d.DescuentoMonto);

                        r.Cells["colItbisPct"].Value = Convert.ToDecimal(d.ItbisPct);
                        r.Cells["colItbisMonto"].Value = Convert.ToDecimal(d.ItbisMonto);
                        r.Cells["colTotalLinea"].Value = Convert.ToDecimal(d.TotalLinea);
                    }
                }
                finally
                {
                    _gridInternalUpdate = false;
                }

                RecalcularTotalesDesdeGridUI();
                AplicarModoEdicion();
                AplicarReglasUI();
                AplicarEstadoFiscalVisual();
                RefrescarEstadoBotonesAccion();
            }
            finally
            {
                _cargando = false;
            }
        }

        // ============================================================
        // GUARDAR / LINEAS
        // ============================================================
        private void GuardarBorrador()
        {
            if (_finalizada) return;
            if (_facturaId <= 0) return;

            ValidarCreditoAntesDeGuardar();

            var tipo = GetTipoDocUI();
            _facRepo.SetTipoDocumentoBorrador(_facturaId, tipo);
            _facRepo.SetFechaDocumentoBorrador(_facturaId, dtpFechaDoc.Value);

            int? terminoPagoId = null;
            int? diasCredito = null;

            if (chkCredito.Checked)
            {
                if (cboTerminoPago.SelectedValue != null)
                    terminoPagoId = Convert.ToInt32(cboTerminoPago.SelectedValue);

                if (int.TryParse((txtDiasCredito.Text ?? "").Trim(), out var d) && d >= 0)
                    diasCredito = d;
            }

            _facRepo.SetCreditoBorrador(_facturaId, chkCredito.Checked, terminoPagoId, diasCredito);
            _facRepo.SetFechaVencimientoBorrador(_facturaId, CalcularFechaLimitePagoUI());
            _facRepo.SetDireccionCliente(_facturaId, txtClienteDireccion.Text);
            _facRepo.SetCodVendedorBorrador(_facturaId, GetCodVendedorUI());

            InsertarFilasNuevasSiExisten();

            if (_esComprobanteFiscal)
                GuardarCabeceraFiscalBorrador();
        }

        private void InsertarFilasNuevasSiExisten()
        {
            if (_gridInternalUpdate) return;
            if (_finalizada) return;
            if (_facturaId <= 0) return;

            foreach (DataGridViewRow r in grid.Rows)
            {
                if (r.IsNewRow) continue;

                var detId = 0;
                try { detId = Convert.ToInt32(r.Cells["colDetId"].Value ?? 0); } catch { detId = 0; }
                if (detId > 0) continue;

                var prodCod = (Convert.ToString(r.Cells["colProductoCodigo"].Value) ?? "").Trim();
                var codBarra = (Convert.ToString(r.Cells["colCodBarra"].Value) ?? "").Trim();

                if (string.IsNullOrWhiteSpace(prodCod) && !string.IsNullOrWhiteSpace(codBarra))
                {
                    var p = _prodRepo.ObtenerPorCodigoOBarras(codBarra);
                    prodCod = (p?.Codigo ?? "").Trim();
                }

                if (string.IsNullOrWhiteSpace(prodCod)) continue;

                var desc = (Convert.ToString(r.Cells["colDescripcion"].Value) ?? "").Trim();
                if (string.IsNullOrWhiteSpace(desc)) desc = prodCod;

                var unidad = (Convert.ToString(r.Cells["colUnidad"].Value) ?? "").Trim();
                if (string.IsNullOrWhiteSpace(unidad))
                    unidad = _prodRepo.ObtenerUnidadPorCodigo(prodCod, "UND") ?? "UND";

                var cant = GetCellDecimal(r, "colCantidad");
                if (cant <= 0m) cant = 1m;

                var descPct = NormalizarPct(GetCellDecimal(r, "colDescuentoPct"));

                var precio = GetCellDecimal(r, "colPrecio");
                var itbisPct = NormalizarPct(GetCellDecimal(r, "colItbisPct"));
                if (itbisPct <= 0m) itbisPct = 18m;

                int? impuestoId = null;
                try
                {
                    var vImp = r.Cells["colImpuestoId"].Value;
                    if (vImp != null && vImp != DBNull.Value)
                        impuestoId = Convert.ToInt32(vImp);
                }
                catch { }

                if (string.IsNullOrWhiteSpace(codBarra))
                    codBarra = _facRepo.ObtenerPrimerCodigoBarra(prodCod);

                var newDetId = _facRepo.AddLineaConUnidad(
                    facturaId: _facturaId,
                    productoCodigo: prodCod,
                    codBarra: string.IsNullOrWhiteSpace(codBarra) ? null : codBarra,
                    descripcion: desc,
                    unidad: unidad,
                    impuestoId: impuestoId,
                    cantidad: cant,
                    precioUnitario: precio,
                    impuestoPctFallback: itbisPct,
                    descuentoPct: descPct
                );

                _gridInternalUpdate = true;
                try { r.Cells["colDetId"].Value = newDetId; }
                finally { _gridInternalUpdate = false; }
            }
        }

        private void EliminarLineaSeleccionada()
        {
            if (_finalizada) return;
            if (grid.CurrentRow == null) return;

            var detId = GetDetId(grid.CurrentRow.Index);
            if (detId <= 0) return;

            _facRepo.DeleteLinea(detId);
            ScheduleAutoSave();
            CargarDocumentoDesdeBD(_facturaId);
        }

        // ============================================================
        // CLIENTE
        // ============================================================
        private void BuscarYSetCliente()
        {
            if (_finalizada) return;

            if (_facturaId <= 0)
            {
                MessageBox.Show("Primero crea/abre un documento.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var txt = (txtClienteBuscar.Text ?? "").Trim();

            // ✅ vacío => Consumidor Final por defecto (C-000001)
            if (string.IsNullOrWhiteSpace(txt))
            {
                try
                {
                    var cf = _cliRepo.ObtenerConsumidorFinal();
                    _clienteActual = cf;

                    AsignarClienteAFactura(cf.ClienteId, cf.Nombre ?? "CONSUMIDOR FINAL", null);

                    txtClienteNombre.Text = cf.Nombre ?? "CONSUMIDOR FINAL";
                    txtClienteDireccion.Text = "";
                    txtClienteRnc.Text = "";
                    SetClienteCodigoUI(cf.Codigo);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Consumidor Final", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    _clienteActual = null;

                    _facRepo.SetCliente(_facturaId, null, "CONSUMIDOR FINAL", null);
                    txtClienteNombre.Text = "CONSUMIDOR FINAL";
                    txtClienteDireccion.Text = "";
                    txtClienteRnc.Text = "";
                    SetClienteCodigoUI(CLIENTE_CONSUMIDOR_FINAL_CODIGO);
                }

                _facRepo.SetDireccionCliente(_facturaId, null);
                SetVendedorUI(null);
                try { _facRepo.SetCodVendedorBorrador(_facturaId, null); } catch { }

                txtClienteBuscar.Clear();

                chkCredito.Checked = false;
                cboTerminoPago.Enabled = false;
                txtDiasCredito.Enabled = false;
                txtDiasCredito.Text = "";

                // ✅ Regla final: vuelve a E32
                ActualizarEstadoComprobante();
                AplicarEstadoFiscalVisual();

                ScheduleAutoSave();
                CargarDocumentoDesdeBD(_facturaId);
                return;
            }

            try
            {
                var cli = _cliRepo.BuscarPorCodigoORnc(txt);
                if (cli != null && cli.ClienteId > 0)
                {
                    AsignarClienteAFactura(cli.ClienteId, cli.Nombre, cli.RncCedula);

                    txtClienteNombre.Text = cli.Nombre ?? "";
                    txtClienteRnc.Text = cli.RncCedula ?? "";
                    txtClienteDireccion.Text = cli.Direccion ?? "";
                    SetClienteCodigoUI(cli.Codigo);
                    _clienteActual = cli;

                    AplicarCondicionPagoDesdeCliente(cli);

                    _facRepo.SetDireccionCliente(_facturaId, txtClienteDireccion.Text);

                    try
                    {
                        GuardarBorrador();
                    }
                    catch { }
                                       

                    // ✅ usar vendedor del cliente solo si la factura aún no tiene uno
                    SetVendedorSiVacio(cli.CodVendedor);

                    // ✅ guardar el vendedor actual de la factura
                    try
                    {
                        _facRepo.SetCodVendedorBorrador(_facturaId, GetCodVendedorUI());
                    }
                    catch { }

                    ActualizarEstadoComprobante();
                    AplicarEstadoFiscalVisual();

                    _warningHeaderOnce = false;

                    ScheduleAutoSave();
                    return;
                }

                var rnc = new string(txt.Where(char.IsDigit).ToArray());
                if (string.IsNullOrWhiteSpace(rnc))
                {
                    MessageBox.Show("Cliente no encontrado. Escribe Código o RNC/Cédula.", "Cliente", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var dg = BuscarEnDgii(rnc);
                if (dg == null)
                {
                    MessageBox.Show("No existe en clientes ni en DGII (dataset ACTIVO).", "DGII", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var nuevoId = _cliRepo.CrearDesdeDgii(
    rnc: rnc,
    nombre: dg.Nombre ?? "",
    nombreComercial: dg.NombreComercial,
    usuario: Environment.UserName
);

                AsignarClienteAFactura(nuevoId, dg.NombreComercial ?? dg.Nombre, rnc);

                var cliNuevo = _cliRepo.BuscarPorCodigoORnc(rnc);
                _clienteActual = cliNuevo;

                txtClienteNombre.Text = cliNuevo?.Nombre ?? dg.NombreComercial ?? dg.Nombre ?? "";
                txtClienteRnc.Text = cliNuevo?.RncCedula ?? rnc;
                txtClienteDireccion.Text = cliNuevo?.Direccion ?? "";
                SetClienteCodigoUI(cliNuevo?.Codigo);
                _warningHeaderOnce = false;

                AplicarCondicionPagoDesdeCliente(cliNuevo);

                _facRepo.SetDireccionCliente(_facturaId, txtClienteDireccion.Text);

                // ✅ si el cliente nuevo tiene vendedor, usarlo solo si la factura está vacía
                SetVendedorSiVacio(cliNuevo?.CodVendedor);

                // ✅ guardar el vendedor actual de la factura
                try
                {
                    _facRepo.SetCodVendedorBorrador(_facturaId, GetCodVendedorUI());
                }
                catch { }

                ActualizarEstadoComprobante();
                AplicarEstadoFiscalVisual();

                ScheduleAutoSave();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Cliente/DGII", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BuscarClienteDesdeVentana()
        {
            BuscarClienteDesdeVentana((txtClienteBuscar.Text ?? "").Trim());
        }

        private void BuscarClienteDesdeVentana(string? filtroInicial)
        {
            if (_finalizada) return;
            if (_facturaId <= 0)
            {
                MessageBox.Show(
                    "Primero crea/abre un documento.",
                    "Cliente",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            if (_openingClienteSearch) return;

            try
            {
                _openingClienteSearch = true;

                using var frm = new FormBuscarCliente();

                if (!string.IsNullOrWhiteSpace(filtroInicial))
                    frm.SetFiltroInicial(filtroInicial);

                if (frm.ShowDialog(this) != DialogResult.OK) return;

                var cli = frm.ClienteSeleccionado;
                if (cli == null || cli.ClienteId <= 0) return;

                AsignarClienteAFactura(cli.ClienteId, cli.Nombre, cli.RncCedula);

                txtClienteNombre.Text = cli.Nombre ?? "";
                txtClienteRnc.Text = cli.RncCedula ?? "";
                txtClienteDireccion.Text = cli.Direccion ?? "";
                SetClienteCodigoUI(cli.Codigo);
                _clienteActual = cli;

                AplicarCondicionPagoDesdeCliente(cli);

                _facRepo.SetDireccionCliente(_facturaId, txtClienteDireccion.Text);

                SetVendedorSiVacio(cli.CodVendedor);

                try
                {
                    _facRepo.SetCodVendedorBorrador(_facturaId, GetCodVendedorUI());
                }
                catch { }

                try
                {
                    GuardarBorrador();
                }
                catch { }

                ActualizarEstadoComprobante();
                AplicarEstadoFiscalVisual();
                _warningHeaderOnce = false;
                ScheduleAutoSave();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Cliente", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _openingClienteSearch = false;
            }
        }

        private void AsignarClienteAFactura(int clienteId, string? nombre, string? doc)
        {
            var nom = string.IsNullOrWhiteSpace(nombre) ? "CLIENTE" : nombre.Trim();
            var documento = string.IsNullOrWhiteSpace(doc) ? null : doc.Trim();

            _facRepo.SetCliente(_facturaId, clienteId, nom, documento);

            txtClienteBuscar.Clear();

            ActualizarEstadoComprobante();
            CargarDocumentoDesdeBD(_facturaId);
        }

        private DgiiInfo? BuscarEnDgii(string rnc)
        {
            var dto = _dgiiRepo.BuscarActivoPorRnc(rnc);
            if (dto != null)
                return new DgiiInfo { Nombre = dto.Nombre, NombreComercial = dto.NombreComercial };

            var ent = _dgiiRepo.BuscarPorRnc(rnc);
            if (ent != null)
                return new DgiiInfo { Nombre = ent.Nombre, NombreComercial = ent.NombreComercial };

            return null;
        }

        private sealed class DgiiInfo
        {
            public string? Nombre { get; set; }
            public string? NombreComercial { get; set; }
        }

        // ============================================================
        // FILAS
        // ============================================================
        private void UpsertFilaBD(int rowIndex)
        {
            if (_finalizada) return;
            if (_facturaId <= 0) return;
            if (rowIndex < 0) return;

            var r = grid.Rows[rowIndex];
            if (r.IsNewRow) return;

            var detId = GetDetId(rowIndex);
            if (detId <= 0) return;

            var desc = GetCellString(r, "colDescripcion") ?? "";
            var unidad = GetCellString(r, "colUnidad") ?? "";
            var cant = GetCellDecimal(r, "colCantidad");
            var precio = GetCellDecimal(r, "colPrecio");
            var itbisPct = GetCellDecimal(r, "colItbisPct");
            var descPct = GetCellDecimal(r, "colDescuentoPct");

            _facRepo.UpdateLineaConUnidad(detId, desc, unidad, cant, precio, itbisPct, descPct);
        }

        private void TryResolveProductoEnFilaNueva(int rowIndex)
        {
            if (_facturaId <= 0) return;
            if (rowIndex < 0) return;
            if (rowIndex >= grid.Rows.Count) return;

            var r = grid.Rows[rowIndex];
            if (r == null || r.IsNewRow) return;

            var detId = GetDetId(rowIndex);
            if (detId > 0) return;

            var codigo = (Convert.ToString(r.Cells["colProductoCodigo"]?.Value) ?? "").Trim();
            var barra = (Convert.ToString(r.Cells["colCodBarra"]?.Value) ?? "").Trim();

            var lookup = !string.IsNullOrWhiteSpace(codigo) ? codigo : barra;
            if (string.IsNullOrWhiteSpace(lookup)) return;

            var prod = _prodRepo.ObtenerPorCodigoOBarras(lookup);
            if (prod == null) return;

            var productoCodigo = (prod.Codigo ?? "").Trim();
            if (string.IsNullOrWhiteSpace(productoCodigo)) return;

            r.Cells["colProductoCodigo"].Value = productoCodigo;

            if (string.IsNullOrWhiteSpace(barra))
            {
                var cb = _facRepo.ObtenerPrimerCodigoBarra(productoCodigo);
                if (!string.IsNullOrWhiteSpace(cb))
                    r.Cells["colCodBarra"].Value = cb;
            }

            var descripcion = (prod.Referencia ?? "").Trim();
            if (string.IsNullOrWhiteSpace(descripcion))
                descripcion = (prod.Descripcion ?? "").Trim();
            if (string.IsNullOrWhiteSpace(descripcion))
                descripcion = productoCodigo;

            var unidad = (prod.UnidadMedidaCodigo ?? "").Trim();
            if (string.IsNullOrWhiteSpace(unidad)) unidad = (prod.UnidadBase ?? "").Trim();
            if (string.IsNullOrWhiteSpace(unidad)) unidad = _prodRepo.ObtenerUnidadPorCodigo(productoCodigo, "UND") ?? "UND";

            decimal precio = prod.PrecioVenta;
            if (precio <= 0m) precio = prod.PrecioCoste;
            if (precio <= 0m) precio = prod.PrecioCompraPromedio;
            if (precio <= 0m) precio = prod.UltimoPrecioCompra;

            r.Cells["colDescripcion"].Value = descripcion;
            r.Cells["colUnidad"].Value = unidad;

            if (GetCellDecimal(r, "colCantidad") <= 0m) r.Cells["colCantidad"].Value = 1m;
            if (GetCellDecimal(r, "colPrecio") <= 0m) r.Cells["colPrecio"].Value = precio;
            if (GetCellDecimal(r, "colItbisPct") <= 0m) r.Cells["colItbisPct"].Value = 18m;
            if (GetCellDecimal(r, "colDescuentoPct") < 0m) r.Cells["colDescuentoPct"].Value = 0m;

            try { r.Cells["colImpuestoId"].Value = prod.ImpuestoId; } catch { }

            RecalcularFilaUI(rowIndex);
        }

        private bool ResolverEnterProductoEnGrid()
        {
            if (_finalizada) return false;
            if (grid.CurrentCell == null) return false;

            var rowIndex = grid.CurrentCell.RowIndex;
            if (rowIndex < 0 || rowIndex >= grid.Rows.Count) return false;

            var colName = grid.Columns[grid.CurrentCell.ColumnIndex].Name;
            if (colName != "colProductoCodigo" && colName != "colCodBarra")
                return false;

            string valor = "";

            try
            {
                if (grid.IsCurrentCellInEditMode && grid.EditingControl is TextBox tb)
                    valor = (tb.Text ?? "").Trim();
                else
                    valor = (Convert.ToString(grid.CurrentCell.Value) ?? "").Trim();
            }
            catch { }

            try
            {
                grid.EndEdit();
            }
            catch { }

            BeginInvoke(new Action(() =>
            {
                if (_finalizada) return;
                if (rowIndex < 0 || rowIndex >= grid.Rows.Count) return;

                if (string.IsNullOrWhiteSpace(valor))
                {
                    BuscarProductoDesdeVentana(rowIndex, null);
                    return;
                }

                var prod = _prodRepo.ObtenerPorCodigoOBarras(valor);
                if (prod != null)
                {
                    AplicarProductoEnFila(rowIndex, prod);
                    return;
                }

                BuscarProductoDesdeVentana(rowIndex, valor);
            }));

            return true;
        }

        private void BuscarProductoDesdeVentana(int rowIndex, string? filtroInicial = null)
        {
            if (_finalizada) return;
            if (_facturaId <= 0) return;
            if (rowIndex < 0 || rowIndex >= grid.Rows.Count) return;
            if (_openingProductoSearch) return;

            try
            {
                _openingProductoSearch = true;

                using var frm = new FormBuscarProducto();

                if (!string.IsNullOrWhiteSpace(filtroInicial))
                    frm.SetFiltroInicial(filtroInicial);

                if (frm.ShowDialog(this) != DialogResult.OK) return;

                var prod = frm.ProductoSeleccionado;
                if (prod == null) return;

                AplicarProductoEnFila(rowIndex, prod);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Producto", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _openingProductoSearch = false;
            }
        }

        private void AplicarProductoEnFila(int rowIndex, Producto prod)
        {
            if (rowIndex < 0) return;

            if (rowIndex >= grid.Rows.Count)
                rowIndex = grid.Rows.Count - 1;

            if (rowIndex < 0) return;

            var r = grid.Rows[rowIndex];
            if (r == null) return;

            if (r.IsNewRow)
            {
                rowIndex = grid.Rows.Add();
                r = grid.Rows[rowIndex];
            }

            var productoCodigo = (prod.Codigo ?? "").Trim();
            if (string.IsNullOrWhiteSpace(productoCodigo)) return;

            var codBarra = "";
            try
            {
                codBarra = _facRepo.ObtenerPrimerCodigoBarra(productoCodigo) ?? "";
            }
            catch { }

            codBarra = codBarra.Trim();

            var descripcion = (prod.Referencia ?? "").Trim();
            if (string.IsNullOrWhiteSpace(descripcion))
                descripcion = (prod.Descripcion ?? "").Trim();
            if (string.IsNullOrWhiteSpace(descripcion))
                descripcion = productoCodigo;

            var unidad = (prod.UnidadMedidaCodigo ?? "").Trim();
            if (string.IsNullOrWhiteSpace(unidad))
                unidad = (prod.UnidadBase ?? "").Trim();
            if (string.IsNullOrWhiteSpace(unidad))
                unidad = _prodRepo.ObtenerUnidadPorCodigo(productoCodigo, "UND") ?? "UND";

            decimal precio = prod.PrecioVenta;
            if (precio <= 0m) precio = prod.PrecioCoste;
            if (precio <= 0m) precio = prod.PrecioCompraPromedio;
            if (precio <= 0m) precio = prod.UltimoPrecioCompra;

            _gridInternalUpdate = true;
            try
            {
                r.Cells["colProductoCodigo"].Value = productoCodigo;
                r.Cells["colCodBarra"].Value = codBarra;
                r.Cells["colDescripcion"].Value = descripcion;
                r.Cells["colUnidad"].Value = unidad;

                if (GetCellDecimal(r, "colCantidad") <= 0m)
                    r.Cells["colCantidad"].Value = 1m;

                r.Cells["colPrecio"].Value = precio;

                if (GetCellDecimal(r, "colItbisPct") <= 0m)
                    r.Cells["colItbisPct"].Value = 18m;

                if (GetCellDecimal(r, "colDescuentoPct") < 0m)
                    r.Cells["colDescuentoPct"].Value = 0m;

                try { r.Cells["colImpuestoId"].Value = prod.ImpuestoId; } catch { }
            }
            finally
            {
                _gridInternalUpdate = false;
            }

            RecalcularFilaUI(rowIndex);
            RecalcularTotalesDesdeGridUI();

            try
            {
                if (GetDetId(rowIndex) <= 0)
                    InsertarFilasNuevasSiExisten();
                else
                    UpsertFilaBD(rowIndex);
            }
            catch { }

            ScheduleAutoSave();

            if (grid.Columns.Contains("colCantidad"))
                grid.CurrentCell = r.Cells["colCantidad"];

            if (grid.EditingControl is TextBox tb)
            {
                tb.SelectAll();
            }
        }

        private void RecalcularFilaUI(int rowIndex)
        {
            var r = grid.Rows[rowIndex];
            if (r.IsNewRow) return;

            var cant = GetCellDecimal(r, "colCantidad");
            var precio = GetCellDecimal(r, "colPrecio");

            var itbisPct = NormalizarPct(GetCellDecimal(r, "colItbisPct"));
            var descPct = NormalizarPct(GetCellDecimal(r, "colDescuentoPct"));

            var baseLinea = cant * precio;
            var descMonto = Math.Round(baseLinea * (descPct / 100m), 2, MidpointRounding.AwayFromZero);

            var baseNeta = baseLinea - descMonto;
            if (baseNeta < 0m) baseNeta = 0m;

            var itbisMonto = Math.Round(baseNeta * (itbisPct / 100m), 2, MidpointRounding.AwayFromZero);
            var totalLinea = Math.Round(baseNeta + itbisMonto, 2, MidpointRounding.AwayFromZero);

            _gridInternalUpdate = true;
            try
            {
                r.Cells["colDescuentoMonto"].Value = descMonto;
                r.Cells["colItbisMonto"].Value = itbisMonto;
                r.Cells["colTotalLinea"].Value = totalLinea;

                r.Cells["colItbisPct"].Value = itbisPct;
                r.Cells["colDescuentoPct"].Value = descPct;
            }
            finally
            {
                _gridInternalUpdate = false;
            }
        }

        // ============================================================
        // FINALIZAR
        // ============================================================
        private void FinalizarDocumento(bool cerrarAlFinal = true)
        {
            try
            {
                if (_facturaId <= 0) throw new Exception("FacturaId inválido.");

                ValidarCreditoAntesDeGuardar();

                GuardarBorrador();

                int? terminoPagoId = null;
                int? diasCredito = null;

                if (chkCredito.Checked)
                {
                    if (cboTerminoPago.SelectedValue != null)
                        terminoPagoId = Convert.ToInt32(cboTerminoPago.SelectedValue);

                    if (int.TryParse((txtDiasCredito.Text ?? "").Trim(), out var d) && d >= 0)
                        diasCredito = d;
                }

                _ = _facRepo.Finalizar(_facturaId, chkCredito.Checked, terminoPagoId, diasCredito, Environment.UserName);

                MessageBox.Show("Documento finalizado.", "Finalizar", MessageBoxButtons.OK, MessageBoxIcon.Information);

                CargarDocumentoDesdeBD(_facturaId);

                if (cerrarAlFinal && !_isEmbedded)
                {
                    DialogResult = DialogResult.OK;
                    Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Finalizar", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }
        }

        private void AplicarModoEdicion()
        {
            grid.ReadOnly = _finalizada;

            btnAgregar.Enabled = !_finalizada;
            btnGuardar.Enabled = !_finalizada;
            btnFinalizar.Enabled = !_finalizada;
            btnEliminarLinea.Enabled = !_finalizada;

            chkCredito.Enabled = !_finalizada;
            cboTerminoPago.Enabled = !_finalizada && chkCredito.Checked;
            txtDiasCredito.Enabled = !_finalizada && chkCredito.Checked;

            dtpFechaDoc.Enabled = !_finalizada;

            txtClienteBuscar.Enabled = !_finalizada;

            if (txtClienteCodigo != null)
                txtClienteCodigo.ReadOnly = _finalizada;

            btnBuscarCliente.Enabled = !_finalizada;
            txtClienteNombre.ReadOnly = _finalizada;
            txtClienteDireccion.ReadOnly = _finalizada;
            txtClienteRnc.ReadOnly = _finalizada;

            cboVendedor.Enabled = !_finalizada;
            cboTipoDoc.Enabled = !_finalizada;

            ActualizarEstadoComprobante();
            AplicarEstadoFiscalVisual();

            if (grid.Columns.Contains("colItbisMonto"))
                grid.Columns["colItbisMonto"].ReadOnly = true;

            if (grid.Columns.Contains("colTotalLinea"))
                grid.Columns["colTotalLinea"].ReadOnly = true;

            if (grid.Columns.Contains("colDescuentoMonto"))
                grid.Columns["colDescuentoMonto"].ReadOnly = true;
        }

        // ============================================================
        // HELPERS
        // ============================================================
        private int GetDetId(int rowIndex)
        {
            try
            {
                var r = grid.Rows[rowIndex];
                var v = r.Cells["colDetId"].Value;
                if (v == null || v == DBNull.Value) return 0;
                return Convert.ToInt32(v);
            }
            catch { return 0; }
        }

        private static decimal NormalizarPct(decimal pct)
        {
            if (pct < 0m) pct = 0m;
            if (pct > 100m) pct = 100m;
            return Math.Round(pct, 4, MidpointRounding.AwayFromZero);
        }

        private static string? GetCellString(DataGridViewRow r, string colName)
        {
            var v = r.Cells[colName].Value;
            return v == null || v == DBNull.Value ? null : Convert.ToString(v);
        }

        private static decimal GetCellDecimal(DataGridViewRow r, string colName)
        {
            var v = r.Cells[colName].Value;
            if (v == null || v == DBNull.Value) return 0m;

            try
            {
                return Convert.ToDecimal(v, CultureInfo.InvariantCulture);
            }
            catch { }

            var s = Convert.ToString(v) ?? "0";
            if (decimal.TryParse(s, NumberStyles.Any, CultureInfo.CurrentCulture, out var d)) return d;
            if (decimal.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out d)) return d;

            return 0m;
        }

        private void RecalcularTotalesDesdeGridUI()
        {
            if (grid == null) return;

            decimal subtotal = 0m;
            decimal descuento = 0m;
            decimal itbis = 0m;
            decimal total = 0m;

            foreach (DataGridViewRow r in grid.Rows)
            {
                if (r == null || r.IsNewRow) continue;

                var cant = GetCellDecimal(r, "colCantidad");
                var precio = GetCellDecimal(r, "colPrecio");
                var itbisPct = NormalizarPct(GetCellDecimal(r, "colItbisPct"));
                var descPct = NormalizarPct(GetCellDecimal(r, "colDescuentoPct"));

                var baseLinea = cant * precio;
                var descMonto = Math.Round(baseLinea * (descPct / 100m), 2, MidpointRounding.AwayFromZero);

                var baseNeta = baseLinea - descMonto;
                if (baseNeta < 0m) baseNeta = 0m;

                var itbisMonto = Math.Round(baseNeta * (itbisPct / 100m), 2, MidpointRounding.AwayFromZero);
                var totalLinea = Math.Round(baseNeta + itbisMonto, 2, MidpointRounding.AwayFromZero);

                subtotal += baseLinea;
                descuento += descMonto;
                itbis += itbisMonto;
                total += totalLinea;
            }

            SetTotales(subtotal, descuento, itbis, total);
        }

        private void SetTotales(decimal subtotal, decimal descuento, decimal itbis, decimal total)
        {
            if (txtSubtotal != null) txtSubtotal.Text = subtotal.ToString("N2");
            if (txtDescuentoTotal != null) txtDescuentoTotal.Text = descuento.ToString("N2");
            if (txtItbisTotal != null) txtItbisTotal.Text = itbis.ToString("N2");
            if (txtTotalGeneral != null) txtTotalGeneral.Text = total.ToString("N2");
        }

        private void FormFacturaV_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F2)
            {
                e.SuppressKeyPress = true;
                BuscarClienteDesdeVentana();
                return;
            }

            if (e.Control && e.KeyCode == Keys.N)
            {
                e.SuppressKeyPress = true;
                btnNuevo.PerformClick();
                return;
            }

            if (e.Control && e.KeyCode == Keys.S)
            {
                e.SuppressKeyPress = true;
                btnGuardar.PerformClick();
                return;
            }

            if (e.KeyCode == Keys.F12)
            {
                e.SuppressKeyPress = true;
                if (!_finalizada) btnFinalizar.PerformClick();
                return;
            }

            if (e.KeyCode == Keys.Delete)
            {
                if (!_finalizada && grid.Focused)
                {
                    e.SuppressKeyPress = true;
                    btnEliminarLinea.PerformClick();
                }
            }
        }

        private int ResolverTipoEcfIdDesdePrefijo(string prefijo)
        {
            prefijo = (prefijo ?? "").Trim().ToUpperInvariant();

            return prefijo switch
            {
                "E31" => 1,
                "E32" => 2,
                "E33" => 3,
                "E34" => 4,
                "E41" => 5,
                "E43" => 6,
                "E44" => 7,
                "E45" => 8,
                "E46" => 9,
                "E47" => 10,
                _ => throw new InvalidOperationException("Tipo de comprobante e-CF no reconocido.")
            };
        }

        private int ResolverTipoPagoEcf()
        {
            return chkCredito.Checked ? 2 : 1; // 1 contado, 2 crédito
        }

        private (decimal montoGravado, decimal montoExento, decimal totalItbis) CalcularResumenFiscalDesdeGrid()
        {
            decimal gravado = 0m;
            decimal exento = 0m;
            decimal itbis = 0m;

            foreach (DataGridViewRow r in grid.Rows)
            {
                if (r == null || r.IsNewRow) continue;

                var cant = GetCellDecimal(r, "colCantidad");
                var precio = GetCellDecimal(r, "colPrecio");
                var descPct = NormalizarPct(GetCellDecimal(r, "colDescuentoPct"));
                var itbisPct = NormalizarPct(GetCellDecimal(r, "colItbisPct"));
                var itbisMonto = GetCellDecimal(r, "colItbisMonto");

                var bruto = cant * precio;
                var descMonto = Math.Round(bruto * (descPct / 100m), 2, MidpointRounding.AwayFromZero);
                var neto = bruto - descMonto;
                if (neto < 0m) neto = 0m;

                if (itbisPct > 0m)
                    gravado += neto;
                else
                    exento += neto;

                itbis += itbisMonto;
            }

            gravado = Math.Round(gravado, 2, MidpointRounding.AwayFromZero);
            exento = Math.Round(exento, 2, MidpointRounding.AwayFromZero);
            itbis = Math.Round(itbis, 2, MidpointRounding.AwayFromZero);

            return (gravado, exento, itbis);
        }

        private void GuardarCabeceraFiscalBorrador()
        {
            if (_facturaId <= 0) return;
            if (!_esComprobanteFiscal) return;
            AplicarEstadoFiscalVisual();

            var prefijo = (Convert.ToString(cboTipoComprobante.SelectedValue) ?? "").Trim().ToUpperInvariant();
            if (string.IsNullOrWhiteSpace(prefijo))
                throw new InvalidOperationException("Debe seleccionar tipo de comprobante fiscal.");

            var tipoEcfId = ResolverTipoEcfIdDesdePrefijo(prefijo);
            var tipoPagoEcf = ResolverTipoPagoEcf();

            var tipoId = int.Parse(prefijo.Substring(1, 2));

            var fechaVencimiento = _facRepo.ObtenerFechaVencimientoSecuencia(
                empresaId: 1,
                sucursalId: 1,
                cajaId: null,
                tipoId: tipoId,
                prefijo: prefijo
            );

            if (!fechaVencimiento.HasValue)
                throw new InvalidOperationException("No existe rango NCF válido para este tipo de comprobante.");

            var resumen = CalcularResumenFiscalDesdeGrid();

            var cliente = _clienteActual;

            var rncComprador = (cliente?.RncCedula ?? txtClienteRnc.Text ?? "").Trim();
            var razonSocial = (
                cliente?.RazonSocialFiscal
                ?? cliente?.Nombre
                ?? txtClienteNombre.Text
                ?? ""
            ).Trim();

            var correoFiscal = (cliente?.CorreoFiscal ?? "").Trim();
            var direccionFiscal = (
                cliente?.Direccion
                ?? txtClienteDireccion.Text
                ?? ""
            ).Trim();

            var municipio = (cliente?.MunicipioCodigo ?? "").Trim();
            var provincia = (cliente?.ProvinciaCodigo ?? "").Trim();
            var identificadorExtranjero = (cliente?.IdentificadorExtranjero ?? "").Trim();

            _facRepo.SetDatosFiscalesBorrador(
                facturaId: _facturaId,
                tipoEcfId: tipoEcfId,
                tipoIngresoId: 1,
                tipoPagoEcfId: tipoPagoEcf,
                esElectronica: true,
                encf: null,
                fechaVencimientoSecuencia: fechaVencimiento,
                indicadorMontoGravado: resumen.montoGravado > 0m ? 1 : (int?)null,
                rncCompradorSnapshot: string.IsNullOrWhiteSpace(rncComprador) ? null : rncComprador,
                razonSocialCompradorSnapshot: string.IsNullOrWhiteSpace(razonSocial) ? null : razonSocial,
                correoCompradorSnapshot: string.IsNullOrWhiteSpace(correoFiscal) ? null : correoFiscal,
                direccionCompradorSnapshot: string.IsNullOrWhiteSpace(direccionFiscal) ? null : direccionFiscal,
                municipioCompradorSnapshot: string.IsNullOrWhiteSpace(municipio) ? null : municipio,
                provinciaCompradorSnapshot: string.IsNullOrWhiteSpace(provincia) ? null : provincia,
                montoGravadoTotal: resumen.montoGravado,
                montoExentoTotal: resumen.montoExento,
                totalItbisRetenido: 0m,
                totalIsrRetencion: 0m,
                totalOtrosImpuestos: 0m,
                estadoEcf: "PENDIENTE",
                tipoPagoEcfHeader: tipoPagoEcf,
                fechaLimitePago: CalcularFechaLimitePagoUI(),
                identificadorExtranjeroSnapshot: string.IsNullOrWhiteSpace(identificadorExtranjero) ? null : identificadorExtranjero
            );
        }

        private RoundedPanel WrapRoundedTextBox(TextBox txt, int height = 34)
        {
            var parent = txt.Parent;
            if (parent == null) return null!;

            var index = parent.Controls.GetChildIndex(txt);
            var bounds = txt.Bounds;
            var dock = txt.Dock;
            var margin = txt.Margin;

            parent.Controls.Remove(txt);

            var wrap = new RoundedPanel
            {
                Name = "wrap_" + txt.Name,
                Margin = margin,
                Dock = dock,
                Size = new Size(bounds.Width, height),
                MinimumSize = new Size(80, height),
                BorderRadius = 10,
                BorderSize = 1,
                BorderColor = Color.FromArgb(220, 224, 230),
                BackColor = Color.White
            };

            txt.Parent = wrap;
            txt.BorderStyle = BorderStyle.None;
            txt.BackColor = Color.White;
            txt.ForeColor = Color.FromArgb(25, 25, 25);
            txt.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular);
            txt.Location = new Point(8, 8);
            txt.Width = wrap.Width - 16;
            txt.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            wrap.Controls.Add(txt);
            wrap.Resize += (_, __) =>
            {
                txt.Width = wrap.ClientSize.Width - 16;
                txt.Top = (wrap.ClientSize.Height - txt.Height) / 2;
            };

            parent.Controls.Add(wrap);
            parent.Controls.SetChildIndex(wrap, index);

            return wrap;
        }

       

        private void StyleCheck(CheckBox chk)
        {
            chk.AutoSize = false;
            chk.Height = 28;
            chk.Width = 120;
            chk.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            chk.Text = "A crédito";
        }

        private void AplicarEstiloModernoFormulario()
        {
            SuspendLayout();

            try
            {
                BackColor = Color.WhiteSmoke;

                // TextBox principales
                WrapRoundedTextBox(txtClienteBuscar);
                WrapRoundedTextBox(txtClienteCodigo);
                WrapRoundedTextBox(txtClienteNombre);
                WrapRoundedTextBox(txtClienteDireccion);
                WrapRoundedTextBox(txtClienteRnc);
                WrapRoundedTextBox(txtDiasCredito);
                WrapRoundedTextBox(txtNumeroFacturaTop);
                WrapRoundedTextBox(txtSubtotal);
                WrapRoundedTextBox(txtDescuentoTotal);
                WrapRoundedTextBox(txtItbisTotal);
                WrapRoundedTextBox(txtTotalGeneral);
                WrapRoundedTextBox(txtEstadoFiscal);

                // Combos
                StyleCombo(cboTipoComprobante);
                StyleCombo(cboTipoDoc);
                StyleCombo(cboVendedor);
                StyleCombo(cboTerminoPago);

                // Check
                StyleCheck(chkCredito);

                // Botón buscar más limpio
                btnBuscarCliente.Height = 34;
                btnBuscarCliente.Width = 100;
                btnBuscarCliente.FlatStyle = FlatStyle.Flat;
                btnBuscarCliente.FlatAppearance.BorderSize = 0;
                btnBuscarCliente.BackColor = Color.FromArgb(45, 125, 255);
                btnBuscarCliente.ForeColor = Color.White;
                btnBuscarCliente.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
                btnBuscarCliente.Cursor = Cursors.Hand;

                // Totales sólo lectura más elegantes
                txtSubtotal.TextAlign = HorizontalAlignment.Right;
                txtDescuentoTotal.TextAlign = HorizontalAlignment.Right;
                txtItbisTotal.TextAlign = HorizontalAlignment.Right;
                txtTotalGeneral.TextAlign = HorizontalAlignment.Right;

                txtSubtotal.ReadOnly = true;
                txtDescuentoTotal.ReadOnly = true;
                txtItbisTotal.ReadOnly = true;
                txtTotalGeneral.ReadOnly = true;
                txtEstadoFiscal.ReadOnly = true;
                txtNumeroFacturaTop.ReadOnly = true;
            }
            finally
            {
                ResumeLayout();
            }
        }

        public class RoundedPanel : Panel
        {
            private int _borderRadius = 8;
            private int _borderSize = 1;
            private Color _borderColor = Color.FromArgb(220, 224, 230);

            [DefaultValue(8)]
            public int BorderRadius
            {
                get => _borderRadius;
                set
                {
                    _borderRadius = value < 1 ? 1 : value;
                    Invalidate();
                }
            }

            [DefaultValue(1)]
            public int BorderSize
            {
                get => _borderSize;
                set
                {
                    _borderSize = value < 1 ? 1 : value;
                    Invalidate();
                }
            }

            [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
            public Color BorderColor
            {
                get => _borderColor;
                set
                {
                    _borderColor = value;
                    Invalidate();
                }
            }

            public RoundedPanel()
            {
                DoubleBuffered = true;
                BackColor = Color.White;
                ResizeRedraw = true;
                Padding = new Padding(8, 4, 8, 4);
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                base.OnPaint(e);

                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

                using var path = GetPath(ClientRectangle, BorderRadius);
                using var pen = new Pen(BorderColor, BorderSize);

                Region = new Region(path);
                e.Graphics.DrawPath(pen, path);
            }

            private GraphicsPath GetPath(Rectangle rect, int radius)
            {
                var path = new GraphicsPath();
                int d = radius * 2;

                path.StartFigure();
                path.AddArc(rect.X, rect.Y, d, d, 180, 90);
                path.AddArc(rect.Right - d - 1, rect.Y, d, d, 270, 90);
                path.AddArc(rect.Right - d - 1, rect.Bottom - d - 1, d, d, 0, 90);
                path.AddArc(rect.X, rect.Bottom - d - 1, d, d, 90, 90);
                path.CloseFigure();

                return path;
            }
        }
    }
}