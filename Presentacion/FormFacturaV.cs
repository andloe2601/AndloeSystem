using Andloe.Data;
using Andloe.Data.DGII;
using Andloe.Entidad;
using Andloe.Logica;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Microsoft.VisualBasic;

namespace Andloe.Presentacion
{
    public partial class FormFacturaV : Form
    {
        private readonly FacturaRepository _facRepo = new();
        private readonly ProductoRepository _prodRepo = new();
        private readonly TerminoPagoRepository _tpRepo = new();
        private readonly DgiiRncRepository _dgiiRepo = new();
        private readonly ClienteRepository _cliRepo = new();

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
        private bool _esComprobanteFiscal = false;
        private bool _warningHeaderOnce = false;

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
            SetupAutoSave();

            AplicarModoEdicion();
            AplicarReglasUI();

            _lastTipoPermitido = GetTipoDocUI();

            // ✅ estado inicial de botón anular
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

            // ✅ ya no existe txtEntrada: enfoca grid
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

            // ✅ cabecera nueva: validación + autosave ligero
            txtClienteNombre.TextChanged += (_, __) => { _warningHeaderOnce = false; };
            txtClienteRnc.TextChanged += (_, __) => { _warningHeaderOnce = false; };
            txtClienteDireccion.TextChanged += (_, __) => { _warningHeaderOnce = false; };
            txtTipoComprobante.TextChanged += (_, __) => { _warningHeaderOnce = false; };
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

                // ✅ evita duplicados: inserta y marca detId en grid
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
            // ✅ ValueType para evitar errores de casteo y DataError por int/decimal mezclado
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
        // CABECERA VALIDATION (bloquea entrada al GRID)
        // ============================================================
        private bool HeaderOkParaProductos()
        {
            var nombre = (txtClienteNombre.Text ?? "").Trim();
            var rnc = (txtClienteRnc.Text ?? "").Trim();
            var dir = (txtClienteDireccion.Text ?? "").Trim();

            if (string.IsNullOrWhiteSpace(nombre)) return false;
            if (string.IsNullOrWhiteSpace(dir)) return false;
            if (string.IsNullOrWhiteSpace(rnc)) return false;

            if (_esComprobanteFiscal)
            {
                var tipoComp = (txtTipoComprobante.Text ?? "").Trim();
                if (string.IsNullOrWhiteSpace(tipoComp)) return false;
            }

            return true;
        }

        private void EnsureHeaderOkOrWarn()
        {
            if (HeaderOkParaProductos()) return;

            if (!_warningHeaderOnce)
            {
                _warningHeaderOnce = true;
                // mensaje apagado por ti (lo respeté)
            }

            // enfoque inteligente
            if (string.IsNullOrWhiteSpace((txtClienteNombre.Text ?? "").Trim())) { txtClienteNombre.Focus(); return; }
            if (string.IsNullOrWhiteSpace((txtClienteDireccion.Text ?? "").Trim())) { txtClienteDireccion.Focus(); return; }
            if (string.IsNullOrWhiteSpace((txtClienteRnc.Text ?? "").Trim())) { txtClienteRnc.Focus(); return; }
            if (_esComprobanteFiscal && string.IsNullOrWhiteSpace((txtTipoComprobante.Text ?? "").Trim()))
            {
                txtTipoComprobante.Focus();
                return;
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

            // ✅ ya no se agregan productos por textbox; el botón "Agregar" solo enfoca el grid y crea fila
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

            // ✅ nuevo: botón CF
            btnComprobanteFiscal.Click += (_, __) =>
            {
                if (_finalizada) return;

                _esComprobanteFiscal = !_esComprobanteFiscal;

                btnComprobanteFiscal.Text = _esComprobanteFiscal ? "CF: Sí" : "CF: No";
                btnComprobanteFiscal.BackColor = _esComprobanteFiscal ? System.Drawing.Color.FromArgb(45, 125, 255) : System.Drawing.Color.White;
                btnComprobanteFiscal.ForeColor = _esComprobanteFiscal ? System.Drawing.Color.White : System.Drawing.Color.FromArgb(25, 25, 25);

                txtTipoComprobante.ReadOnly = !_esComprobanteFiscal;
                if (!_esComprobanteFiscal)
                    txtTipoComprobante.Text = "";

                _warningHeaderOnce = false;
                if (_esComprobanteFiscal) txtTipoComprobante.Focus();
            };

            btnGuardar.Click += (_, __) => GuardarBorrador();
            btnFinalizar.Click += (_, __) => FinalizarDocumento();

            // ✅ ANULAR (nuevo)
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

                // ✅ Bloqueo por cabecera
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

            // ✅ anular depende de tipo + estado
            RefrescarEstadoBotonesAccion();
        }

        private void RefrescarEstadoBotonesAccion()
        {
            try
            {
                // ✅ Solo anular FAC FINALIZADA
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
                ImprimirFacturaRI();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Registrar", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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

                // ✅ Repo (debe existir)
                _facRepo.AnularFactura(_facturaId, Environment.UserName, motivo);

                // ✅ Auditoría (si ya lo tienes implementado)
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

                // recargar
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

                var s = Andloe.Logica.SesionService.Current;

                _facturaId = _facRepo.CrearBorrador(
                    tipoDocumento: _tipoDoc,
                    usuarioCreacion: Environment.UserName,
                    empresaId: s.EmpresaId,
                    sucursalId: s.SucursalId,
                    almacenId: s.AlmacenId
                );

                CargarDocumentoDesdeBD(_facturaId);

                txtClienteBuscar.Clear();

                // cabecera cliente nueva
                txtClienteNombre.Text = "";
                txtClienteDireccion.Text = "";
                txtClienteRnc.Text = "";
                _esComprobanteFiscal = false;
                btnComprobanteFiscal.Text = "CF: No";
                txtTipoComprobante.Text = "";
                txtTipoComprobante.ReadOnly = true;

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

                // número arriba (factura)
                txtNumeroFacturaTop.Text = string.IsNullOrWhiteSpace(dto.Cab.NumeroDocumento) ? "(sin asignar)" : dto.Cab.NumeroDocumento;

                txtEstadoInfo.Text = dto.Cab.Estado ?? "";
                txtTipoInfo.Text = _tipoDoc ?? "";

                // Cliente cabecera nueva
                txtClienteNombre.Text = dto.Cab.NombreCliente ?? "";
                txtClienteDireccion.Text = dto.Cab.DireccionCliente ?? "";

                // ✅ FIX: NO inventar el RNC desde el buscador.
                txtClienteRnc.Text = dto.Cab.DocumentoCliente ?? "";

                var f = dto.Cab.FechaDocumento;
                if (f < dtpFechaDoc.MinDate) f = DateTime.Today;
                if (f > dtpFechaDoc.MaxDate) f = DateTime.Today;
                dtpFechaDoc.Value = f;

                // ✅ Bloquea edición si FINALIZADA o ANULADA (sin depender de constante nueva)
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

                        // ✅ asegurar decimal en el grid (aunque venga int desde BD)
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

            InsertarFilasNuevasSiExisten();

            if (!_suppressReload)
                CargarDocumentoDesdeBD(_facturaId);
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

            if (string.IsNullOrWhiteSpace(txt))
            {
                _facRepo.SetCliente(_facturaId, null, "CONSUMIDOR FINAL", null);
                _facRepo.SetDireccionCliente(_facturaId, null);

                txtClienteBuscar.Clear();

                txtClienteNombre.Text = "Entrar Datos CLiente";
                txtClienteDireccion.Text = "";
                txtClienteRnc.Text = "";
                _warningHeaderOnce = false;

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

                    // refleja en cabecera nueva
                    txtClienteNombre.Text = cli.Nombre ?? "";
                    txtClienteRnc.Text = cli.RncCedula ?? "";
                    txtClienteDireccion.Text = cli.Direccion ?? cli.Direccion ?? "";

                    _facRepo.SetDireccionCliente(_facturaId, txtClienteDireccion.Text);

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

                // refleja cabecera nueva
                txtClienteNombre.Text = dg.NombreComercial ?? dg.Nombre ?? "";
                txtClienteRnc.Text = rnc;
                _warningHeaderOnce = false;
                _facRepo.SetDireccionCliente(_facturaId, txtClienteDireccion.Text);

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
            if (precio <= 0m) precio = prod.PrecioCoste ?? 0m;
            if (precio <= 0m) precio = prod.PrecioCompraPromedio ?? 0m;
            if (precio <= 0m) precio = prod.UltimoPrecioCompra ?? 0m;

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
        private void FinalizarDocumento()
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

                if (!_isEmbedded)
                {
                    DialogResult = DialogResult.OK;
                    Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Finalizar", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AplicarModoEdicion()
        {
            grid.ReadOnly = _finalizada;

            btnAgregar.Enabled = !_finalizada;
            btnGuardar.Enabled = !_finalizada;
            btnFinalizar.Enabled = !_finalizada;
            btnEliminarLinea.Enabled = !_finalizada;

            // ✅ Anular se maneja por regla (tipo+estado), no por _finalizada
            // Aquí no lo tocamos; lo decide RefrescarEstadoBotonesAccion()

            chkCredito.Enabled = !_finalizada;
            cboTerminoPago.Enabled = !_finalizada && chkCredito.Checked;
            txtDiasCredito.Enabled = !_finalizada && chkCredito.Checked;

            dtpFechaDoc.Enabled = !_finalizada;

            btnComprobanteFiscal.Enabled = !_finalizada;
            txtTipoComprobante.Enabled = !_finalizada;

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

            // ✅ FIX fuerte: si viene Int32, Double, Decimal, etc.
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
    }
}
