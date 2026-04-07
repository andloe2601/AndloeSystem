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
using System.Data;
using System.Diagnostics;
using System.Drawing;
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

        private bool _autoNuevo = false;
        private string _autoTipo = FacturaRepository.TIPO_COT;

        private int _facturaId = 0;
        private string _tipoDoc = FacturaRepository.TIPO_COT;

        // ✅ _finalizada = bloquea edición (FINALIZADA o ANULADA)
        private bool _finalizada = false;

        private bool _gridInternalUpdate = false;
        private bool _isEmbedded = false;

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
        // 🔥 NUEVO: Siempre TRUE (solo ComboBox, sin botón)
        private bool _esComprobanteFiscal = true;
        private bool _warningHeaderOnce = false;

        // ✅ Consumidor Final por defecto (C-000001)
        private const string CLIENTE_CONSUMIDOR_FINAL_CODIGO = "C-000001";

        public FormFacturaV()
        {
            InitializeComponent();

            KeyPreview = true;
            Load += FormFacturaV_Load;
            KeyDown += FormFacturaV_KeyDown;

            EnsureGridColumns();
            ConfigureGridColumns();

            WireEvents();
            InitCombos();

            // ✅ aplica estado inicial (E32 sin doc / E31 con doc)
            ActualizarEstadoComprobante();

            SetupAutoSave();

            AplicarModoEdicion();
            AplicarReglasUI();

            _lastTipoPermitido = GetTipoDocUI();
            RefrescarEstadoBotonesAccion();
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

        // ============================================================
        // AUTO SAVE
        // ============================================================
        private void SetupAutoSave()
        {
            _autoSaveTimer.Interval = 900;
            _autoSaveTimer.Tick += AutoSaveTimer_Tick;

            cboTipoDoc.SelectedValueChanged += (_, __) =>
            {
                if (_cargando) return;
                AplicarReglasUI();
                RefrescarEstadoBotonesAccion();
            };

            chkCredito.CheckedChanged += (_, __) => { AplicarReglasUI(); ScheduleAutoSave(); };
            cboTerminoPago.SelectedValueChanged += (_, __) => ScheduleAutoSave();
            txtDiasCredito.TextChanged += (_, __) => ScheduleAutoSave();
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

            txtClienteNombre.TextChanged += (_, __) => { _warningHeaderOnce = false; };

            // ✅ Regla final: E32 por defecto, E31 si hay RNC, vuelve a E32 si se borra
            txtClienteRnc.TextChanged += (_, __) =>
            {
                if (_cargando) return;
                _warningHeaderOnce = false;

                ActualizarEstadoComprobante();
                ScheduleAutoSave();
            };

            txtClienteDireccion.TextChanged += (_, __) => { _warningHeaderOnce = false; };
            cboTipoComprobante.SelectedValueChanged += (_, __) => { _warningHeaderOnce = false; };
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

            var prefijo = (Convert.ToString(cboTipoComprobante.SelectedValue) ?? "")
                .Trim().ToUpperInvariant();

            if (string.IsNullOrWhiteSpace(prefijo))
                throw new InvalidOperationException("Debe seleccionar el tipo de comprobante (E31/E32/E34).");

            // Consumidor Final => SOLO E32
            if (EsConsumidorFinal())
            {
                if (prefijo != "E32")
                    throw new InvalidOperationException("Consumidor final debe facturarse con e-Factura de Consumo (E32).");
                return; // E32 permite sin RNC
            }

            // Cliente con doc => validar doc si el tipo lo requiere
            if (prefijo == "E31" || prefijo == "E34" || prefijo == "E45")
            {
                if (!DocValidoRncOCedula(txtClienteRnc.Text))
                    throw new InvalidOperationException("RNC/Cédula inválido. Debe tener 9 (RNC) o 11 (Cédula) dígitos.");
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

                if (string.IsNullOrWhiteSpace(prefijo))
                    throw new InvalidOperationException("Debe seleccionar el tipo de comprobante fiscal.");

                // Hoy AlanubeService solo soporta 31 y 32
                if (prefijo == "E34")
                    throw new InvalidOperationException("Alanube Sandbox todavía no está habilitado en este flujo para E34. Usa E31 o E32.");

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
                var requiereDoc = (prefijo == "E31" || prefijo == "E34");

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

            btnRegistrarFactura.Click += (_, __) => RegistrarFactura();

            btnBuscarCliente.Click += (_, __) => BuscarYSetCliente();
            txtClienteBuscar.KeyDown += (_, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    BuscarYSetCliente();
                }
            };

            btnGuardar.Click += (_, __) => GuardarBorrador();
            btnFinalizar.Click += (_, __) => FinalizarDocumento(true);

            btnAnular.Click += (_, __) => AnularDocumento();

            btnEliminarLinea.Click += (_, __) => EliminarLineaSeleccionada();
            btnImprimir.Click += (_, __) => ImprimirCOTPF();
            btnCerrar.Click += (_, __) => Close();

            chkCredito.CheckedChanged += (_, __) =>
            {
                if (_finalizada) return;
                cboTerminoPago.Enabled = chkCredito.Checked;
                txtDiasCredito.Enabled = chkCredito.Checked;
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
            return (txtEstadoInfo?.Text ?? "").Trim().ToUpperInvariant();
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

            if (_finalizada && tipoActual == FacturaRepository.TIPO_FAC)
            {
                ImprimirFacturaRI();
                return;
            }

            try
            {
                _suppressReload = true;
                try { GuardarBorrador(); }
                finally { _suppressReload = false; }

                if (tipoActual != FacturaRepository.TIPO_FAC)
                {
                    var nuevoNumero = _facRepo.ConvertirCotPfAFac_GenerandoNuevoNumero(_facturaId, Environment.UserName);

                    _allowFacProgrammatic = true;
                    _cargando = true;
                    try { cboTipoDoc.SelectedValue = FacturaRepository.TIPO_FAC; }
                    finally
                    {
                        _cargando = false;
                        _allowFacProgrammatic = false;
                    }

                    _tipoDoc = FacturaRepository.TIPO_FAC;
                    txtNumeroFacturaTop.Text = string.IsNullOrWhiteSpace(nuevoNumero) ? txtNumeroFacturaTop.Text : nuevoNumero;
                }

                FinalizarDocumento();

                // 1) generar eNCF si aplica
                GuardarCabeceraFiscalBorrador();

                try
                {
                    ProcesarEcfSiAplica(_facturaId);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        $"Factura registrada, pero no se pudo completar el proceso e-CF.\n{ex.Message}",
                        "e-CF",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );

                    ImprimirFacturaRI();
                    return;
                }

                try
                {
                    EnviarAlanubeSiAplica(_facturaId);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        $"Factura registrada y XML generado, pero falló el envío a Alanube.\n{ex.Message}",
                        "Alanube Sandbox",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );

                    ImprimirFacturaRI();
                    return;
                }

                ImprimirFacturaRI();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Registrar", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            cboTerminoPago.DisplayMember = "Descripcion";
            cboTerminoPago.ValueMember = "TerminoPagoId";
            cboTerminoPago.DataSource = tps;

            cboTerminoPago.Enabled = false;
            txtDiasCredito.Enabled = false;

            // ✅ Tipos de comprobante basados en tu BD (31,32,34)
            cboTipoComprobante.DisplayMember = "Text";
            cboTipoComprobante.ValueMember = "Value";
            cboTipoComprobante.DataSource = new[]
            {
                new { Text = "e-Factura de Crédito Fiscal", Value = "E31" },
                new { Text = "e-Factura de Consumo",        Value = "E32" },
                new { Text = "e-Comprobante Gubernamental", Value = "E45" },

            }.ToList();

            // 🔥 Por defecto E32
            cboTipoComprobante.SelectedValue = "E32";
            cboTipoComprobante.Enabled = false;

            // ✅ Vendedores (Catálogo)
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
                    _facRepo.SetCliente(_facturaId, cf.ClienteId, cf.Nombre ?? "CONSUMIDOR FINAL", null);

                    txtClienteNombre.Text = cf.Nombre ?? "CONSUMIDOR FINAL";
                    txtClienteRnc.Text = "";
                    txtClienteDireccion.Text = "";
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Consumidor Final", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    _facRepo.SetCliente(_facturaId, null, "CONSUMIDOR FINAL", null);
                    txtClienteNombre.Text = "CONSUMIDOR FINAL";
                    txtClienteRnc.Text = "";
                    txtClienteDireccion.Text = "";
                }

                // ✅ Reaplica regla
                ActualizarEstadoComprobante();

                CargarDocumentoDesdeBD(_facturaId);

                txtClienteBuscar.Clear();

                _warningHeaderOnce = false;

                grid.Focus();

                _lastTipoPermitido = GetTipoDocUI();
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

                txtEstadoInfo.Text = dto.Cab.Estado ?? "";
                txtTipoInfo.Text = _tipoDoc ?? "";

                txtClienteNombre.Text = dto.Cab.NombreCliente ?? "";
                txtClienteDireccion.Text = dto.Cab.DireccionCliente ?? "";
                txtClienteRnc.Text = dto.Cab.DocumentoCliente ?? "";

                var f = dto.Cab.FechaDocumento;
                if (f < dtpFechaDoc.MinDate) f = DateTime.Today;
                if (f > dtpFechaDoc.MaxDate) f = DateTime.Today;
                dtpFechaDoc.Value = f;

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
                    AsignarClienteAFactura(cf.ClienteId, cf.Nombre ?? "CONSUMIDOR FINAL", null);

                    txtClienteNombre.Text = cf.Nombre ?? "CONSUMIDOR FINAL";
                    txtClienteDireccion.Text = "";
                    txtClienteRnc.Text = "";
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Consumidor Final", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    _facRepo.SetCliente(_facturaId, null, "CONSUMIDOR FINAL", null);
                    txtClienteNombre.Text = "CONSUMIDOR FINAL";
                    txtClienteDireccion.Text = "";
                    txtClienteRnc.Text = "";
                }

                _facRepo.SetDireccionCliente(_facturaId, null);
                SetVendedorUI(null);
                try { _facRepo.SetCodVendedorBorrador(_facturaId, null); } catch { }

                txtClienteBuscar.Clear();

                // ✅ Regla final: vuelve a E32
                ActualizarEstadoComprobante();

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

                    _facRepo.SetDireccionCliente(_facturaId, txtClienteDireccion.Text);

                    // ✅ si la factura no tiene vendedor, usar el del cliente por defecto
                    SetVendedorSiVacio(cli.CodVendedor);

                    ActualizarEstadoComprobante();

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

                txtClienteNombre.Text = dg.NombreComercial ?? dg.Nombre ?? "";
                txtClienteRnc.Text = rnc;
                _warningHeaderOnce = false;
                _facRepo.SetDireccionCliente(_facturaId, txtClienteDireccion.Text);

                ActualizarEstadoComprobante();

                ScheduleAutoSave();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Cliente/DGII", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

            // ✅ NO lo fuerces a false aquí; usa la regla central
            ActualizarEstadoComprobante();

            if (grid.Columns.Contains("colItbisMonto")) grid.Columns["colItbisMonto"].ReadOnly = true;
            if (grid.Columns.Contains("colTotalLinea")) grid.Columns["colTotalLinea"].ReadOnly = true;
            if (grid.Columns.Contains("colDescuentoMonto")) grid.Columns["colDescuentoMonto"].ReadOnly = true;
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

            var rncComprador = (txtClienteRnc.Text ?? "").Trim();
            var razonSocial = (txtClienteNombre.Text ?? "").Trim();
            var direccion = (txtClienteDireccion.Text ?? "").Trim();

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
                correoCompradorSnapshot: null,
                direccionCompradorSnapshot: string.IsNullOrWhiteSpace(direccion) ? null : direccion,
                municipioCompradorSnapshot: null,
                provinciaCompradorSnapshot: null,
                montoGravadoTotal: resumen.montoGravado,
                montoExentoTotal: resumen.montoExento,
                totalItbisRetenido: 0m,
                totalIsrRetencion: 0m,
                totalOtrosImpuestos: 0m,
                estadoEcf: "PENDIENTE",
                tipoPagoEcfHeader: tipoPagoEcf,
                fechaLimitePago: chkCredito.Checked ? dtpFechaDoc.Value.Date.AddDays(int.TryParse(txtDiasCredito.Text, out var d) ? d : 0) : (DateTime?)null,
                identificadorExtranjeroSnapshot: null
            );
        }
                
    }
}
