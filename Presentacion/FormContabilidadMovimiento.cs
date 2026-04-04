#nullable enable
using Andloe.Data;
using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Text.Json;
using System.Collections.Generic;

namespace Andloe.Presentacion
{
    public partial class FormContabilidadMovimiento : Form
    {
        private long _movimientoId = 0;
        private string _noAsiento = "(sin crear)";

        // Plantillas Contables (dbo.ContabilidadConfig)
        private DataTable? _plantillaDt;
        private bool _plantillaReady = false;


        public FormContabilidadMovimiento()
        {
            InitializeComponent();
            InitUi();
        }

        private void InitUi()
        {
            // Plantillas (ContabilidadConfig)
            InitPlantillaUi();

            // Monedas básicas (ajusta si quieres cargar desde tabla)
            cboMoneda.Items.Clear();
            cboMoneda.Items.Add("DOP");
            cboMoneda.Items.Add("USD");
            cboMoneda.SelectedIndex = 0;

            nudTC.Value = 1;

            ConfigGrid();
            PintarTotales(0, 0, 0, 0);
        }


        // =====================
        // Plantillas contables (ContabilidadConfig)
        // =====================
        private void InitPlantillaUi()
        {
            // Grid plantilla
            if (gridPlantilla != null)
            {
                gridPlantilla.AutoGenerateColumns = false;
                gridPlantilla.Columns.Clear();
                gridPlantilla.AllowUserToAddRows = false;
                gridPlantilla.AllowUserToDeleteRows = false;
                gridPlantilla.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                gridPlantilla.MultiSelect = false;

                gridPlantilla.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "pOrden",
                    HeaderText = "Orden",
                    DataPropertyName = "Orden",
                    Width = 60,
                    ReadOnly = true
                });

                gridPlantilla.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "pNaturaleza",
                    HeaderText = "Naturaleza",
                    DataPropertyName = "Naturaleza",
                    Width = 90,
                    ReadOnly = true
                });

                gridPlantilla.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "pCodigo",
                    HeaderText = "Cuenta",
                    DataPropertyName = "CuentaCodigo",
                    Width = 120,
                    ReadOnly = true
                });

                gridPlantilla.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "pNombre",
                    HeaderText = "Nombre Cuenta",
                    DataPropertyName = "CuentaNombre",
                    Width = 260,
                    ReadOnly = true
                });

                // Conceptos soportados (VENTA/COBRO/NC). Se filtran visualmente por evento.
                gridPlantilla.Columns.Add(new DataGridViewComboBoxColumn
                {
                    Name = "pConcepto",
                    HeaderText = "Concepto",
                    DataPropertyName = "Concepto",
                    Width = 150,
                    FlatStyle = FlatStyle.Flat,
                    DataSource = new string[]
                    {
                        "MANUAL",
                        // VENTA
                        "TOTAL", "BASE", "ITBIS", "DESCUENTO",
                        // COBRO
                        "MONTO_COBRO", "CXC", "CAJA", "BANCO", "EFECTIVO", "TARJETA", "TRANSFERENCIA",
                        // NC
                        "TOTAL_NC", "BASE_NC", "ITBIS_NC"
                    }
                });

                gridPlantilla.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "pMonto",
                    HeaderText = "Monto",
                    DataPropertyName = "Monto",
                    Width = 110,
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "N2" }
                });

                gridPlantilla.CellEndEdit += gridPlantilla_CellEndEdit;
            }

            // Eventos
            if (cboPlantillaModulo != null)
            {
                cboPlantillaModulo.DropDownStyle = ComboBoxStyle.DropDownList;
                cboPlantillaEvento.DropDownStyle = ComboBoxStyle.DropDownList;
                cboPlantillaRol.DropDownStyle = ComboBoxStyle.DropDownList;

                cboPlantillaModulo.SelectedIndexChanged += cboPlantillaModulo_SelectedIndexChanged;
                cboPlantillaEvento.SelectedIndexChanged += cboPlantillaEvento_SelectedIndexChanged;

                btnPlantillaCargar.Click += btnPlantillaCargar_Click;
                btnPlantillaAplicar.Click += btnPlantillaAplicar_Click;
                btnVentaAutoCalcular.Click += btnVentaAutoCalcular_Click;
                btnCobroAutoCalcular.Click += btnCobroAutoCalcular_Click;
                btnNcAutoCalcular.Click += btnNcAutoCalcular_Click;

                // Cargar combos al iniciar
                LoadPlantillaLookups();
                ToggleEventoUi();
            }
        }

        private void LoadPlantillaLookups()
        {
            try
            {
                using var cn = Db.GetOpenConnection();

                // Modulos
                using (var cmd = new SqlCommand(@"
SELECT DISTINCT Modulo
FROM dbo.ContabilidadConfig
WHERE Activo = 1 AND Modulo IS NOT NULL AND LTRIM(RTRIM(Modulo)) <> ''
ORDER BY Modulo;", cn))
                {
                    var dt = new DataTable();
                    new SqlDataAdapter(cmd).Fill(dt);
                    cboPlantillaModulo.Items.Clear();
                    foreach (DataRow r in dt.Rows){
                        var s = r[0]?.ToString();
                        if (!string.IsNullOrWhiteSpace(s))
                            cboPlantillaModulo.Items.Add(s);
                    }
}

                // Seleccion inicial
                if (cboPlantillaModulo.Items.Count > 0)
                    cboPlantillaModulo.SelectedIndex = 0;

                _plantillaReady = true;
            }
            catch (Exception ex)
            {
                _plantillaReady = false;
                MessageBox.Show(ex.Message, "Plantillas - Cargar lookups", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void cboPlantillaModulo_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (!_plantillaReady) return;

            try
            {
                var modulo = cboPlantillaModulo.SelectedItem?.ToString();
                if (string.IsNullOrWhiteSpace(modulo)) return;

                using var cn = Db.GetOpenConnection();

                // Eventos por modulo
                using (var cmd = new SqlCommand(@"
SELECT DISTINCT Evento
FROM dbo.ContabilidadConfig
WHERE Activo = 1 AND Modulo = @Modulo
  AND Evento IS NOT NULL AND LTRIM(RTRIM(Evento)) <> ''
ORDER BY Evento;", cn))
                {
                    cmd.Parameters.Add("@Modulo", SqlDbType.VarChar, 30).Value = modulo;
                    var dt = new DataTable();
                    new SqlDataAdapter(cmd).Fill(dt);

                    cboPlantillaEvento.Items.Clear();
                    foreach (DataRow r in dt.Rows){
                        var s = r[0]?.ToString();
                        if (!string.IsNullOrWhiteSpace(s))
                            cboPlantillaEvento.Items.Add(s);
                    }
}

                if (cboPlantillaEvento.Items.Count > 0)
                    cboPlantillaEvento.SelectedIndex = 0;
                else
                    cboPlantillaRol.Items.Clear();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Plantillas - Eventos", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void cboPlantillaEvento_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (!_plantillaReady) return;

            try
            {
                var modulo = cboPlantillaModulo.SelectedItem?.ToString();
                var evento = cboPlantillaEvento.SelectedItem?.ToString();
                if (string.IsNullOrWhiteSpace(modulo) || string.IsNullOrWhiteSpace(evento)) return;

                using var cn = Db.GetOpenConnection();

                // Roles por modulo+evento
                using (var cmd = new SqlCommand(@"
SELECT DISTINCT Rol
FROM dbo.ContabilidadConfig
WHERE Activo = 1 AND Modulo = @Modulo AND Evento = @Evento
  AND Rol IS NOT NULL AND LTRIM(RTRIM(Rol)) <> ''
ORDER BY Rol;", cn))
                {
                    cmd.Parameters.Add("@Modulo", SqlDbType.VarChar, 30).Value = modulo;
                    cmd.Parameters.Add("@Evento", SqlDbType.VarChar, 40).Value = evento;
                    var dt = new DataTable();
                    new SqlDataAdapter(cmd).Fill(dt);

                    cboPlantillaRol.Items.Clear();
                    foreach (DataRow r in dt.Rows){
                        var s = r[0]?.ToString();
                        if (!string.IsNullOrWhiteSpace(s))
                            cboPlantillaRol.Items.Add(s);
                    }
}

                if (cboPlantillaRol.Items.Count > 0)
                    cboPlantillaRol.SelectedIndex = 0;

                ToggleEventoUi();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Plantillas - Roles", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnPlantillaCargar_Click(object? sender, EventArgs e)
        {
            try
            {
                var modulo = cboPlantillaModulo.SelectedItem?.ToString();
                var evento = cboPlantillaEvento.SelectedItem?.ToString();
                var rol = cboPlantillaRol.SelectedItem?.ToString();

                if (string.IsNullOrWhiteSpace(modulo) || string.IsNullOrWhiteSpace(evento) || string.IsNullOrWhiteSpace(rol))
                {
                    MessageBox.Show("Seleccione Módulo, Evento y Rol.", "Plantilla", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                using var cn = Db.GetOpenConnection();
                using var cmd = new SqlCommand(@"
SELECT
    cc.Orden,
    cc.Naturaleza,
    COALESCE(NULLIF(cc.CuentaCodigo,''), c.Codigo) AS CuentaCodigo,
    c.Nombre AS CuentaNombre,
    UPPER(LTRIM(RTRIM(cc.Concepto))) AS Concepto,
    CAST(0 AS decimal(18,2)) AS Monto
FROM dbo.ContabilidadConfig cc
LEFT JOIN dbo.ContabilidadCatalogoCuenta c ON c.CuentaId = cc.CuentaId
WHERE cc.Activo = 1
  AND cc.Modulo = @Modulo
  AND cc.Evento = @Evento
  AND cc.Rol = @Rol
ORDER BY cc.Orden;", cn);

                cmd.Parameters.Add("@Modulo", SqlDbType.VarChar, 30).Value = modulo;
                cmd.Parameters.Add("@Evento", SqlDbType.VarChar, 40).Value = evento;
                cmd.Parameters.Add("@Rol", SqlDbType.VarChar, 60).Value = rol;

                var dt = new DataTable();
                new SqlDataAdapter(cmd).Fill(dt);

                EnsurePlantillaConceptoColumn(dt);

                _plantillaDt = dt;
                gridPlantilla.DataSource = dt;
                if (gridPlantilla.Columns.Contains("pConcepto"))
                    gridPlantilla.Columns["pConcepto"].ReadOnly = true;

                // UX: sugerir descripcion en cabecera si está vacía
                if (string.IsNullOrWhiteSpace(txtDescripcion.Text))
                    txtDescripcion.Text = $"{modulo} - {evento}";

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Cargar plantilla", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnPlantillaAplicar_Click(object? sender, EventArgs e)
        {
            try
            {
                if (_plantillaDt == null || _plantillaDt.Rows.Count == 0)
                {
                    MessageBox.Show("Primero cargue una plantilla.", "Plantilla", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (_movimientoId <= 0)
                {
                    MessageBox.Show("Primero cree el movimiento (cabecera).", "Plantilla", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }


                var modulo = cboPlantillaModulo.SelectedItem?.ToString() ?? "";
                var evento = cboPlantillaEvento.SelectedItem?.ToString() ?? "";
                var rol = cboPlantillaRol.SelectedItem?.ToString() ?? "";
                modulo = modulo.Trim();
                evento = evento.Trim();
                rol = rol.Trim();

                // Validar período contable antes de aplicar plantilla
                if (!ValidarPeriodoAbierto(DateTime.Now, "No se puede APLICAR la plantilla.")) return;

                // Aplicar: por cada fila con monto > 0, agregar linea
                using var cn = Db.GetOpenConnection();

                foreach (DataRow r in _plantillaDt.Rows)
                {
                    var monto = r["Monto"] == DBNull.Value ? 0m : Convert.ToDecimal(r["Monto"]);
                    if (monto <= 0) continue;

                    var naturaleza = (r["Naturaleza"]?.ToString() ?? "").Trim().ToUpperInvariant();
                    var cuentaCodigo = (r["CuentaCodigo"]?.ToString() ?? "").Trim();

                    if (string.IsNullOrWhiteSpace(cuentaCodigo))
                        continue;

                    var deb = 0m;
                    var cred = 0m;

                    // Naturaleza: admite varias formas (DEBITO, DEBE, D / CREDITO, HABER, C)
                    if (naturaleza.StartsWith("D") || naturaleza.Contains("DEB") || naturaleza.Contains("DEBE"))
                        deb = monto;
                    else
                        cred = monto;

                    using var cmd = new SqlCommand("dbo.sp_Conta_Mov_AddLineaPorCodigo", cn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@MovimientoId", SqlDbType.BigInt).Value = _movimientoId;
                    cmd.Parameters.Add("@CuentaCodigo", SqlDbType.VarChar, 15).Value = cuentaCodigo;
                    var descCab = (txtDescripcion.Text ?? "").Trim();
                    var concepto = (r["Concepto"]?.ToString() ?? "").Trim();
                    var descLineaFinal = string.IsNullOrWhiteSpace(concepto) ? descCab : $"{descCab} | {concepto}";
                    if (string.IsNullOrWhiteSpace(descLineaFinal))
                        cmd.Parameters.Add("@Descripcion", SqlDbType.NVarChar, 300).Value = DBNull.Value;
                    else
                        cmd.Parameters.Add("@Descripcion", SqlDbType.NVarChar, 300).Value = descLineaFinal;

                    var pDeb = cmd.Parameters.Add("@DebitoMoneda", SqlDbType.Decimal);
                    pDeb.Precision = 18; pDeb.Scale = 2; pDeb.Value = deb;

                    var pCred = cmd.Parameters.Add("@CreditoMoneda", SqlDbType.Decimal);
                    pCred.Precision = 18; pCred.Scale = 2; pCred.Value = cred;

                    cmd.ExecuteNonQuery();
                }

                CargarLineas();
                CargarTotales();

                // Auditoría JSON automática
                AuditoriaPlantilla_Aplicada(modulo, evento, rol);

                MessageBox.Show("Plantilla aplicada al movimiento.", "Plantilla", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Aplicar plantilla", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void gridPlantilla_CellEndEdit(object? sender, DataGridViewCellEventArgs e)
        {
            // Normalizar monto (si el usuario borra -> 0)
            try
            {
                if (_plantillaDt == null) return;
                if (e.RowIndex < 0) return;

                var cell = gridPlantilla.Rows[e.RowIndex].Cells["pMonto"];
                if (cell?.Value == null) return;

                var s = cell.Value.ToString();
                if (string.IsNullOrWhiteSpace(s))
                    cell.Value = 0m;
            }
            catch { /* silencioso */ }
        }


        private void ToggleEventoUi()
        {
            var evento = (cboPlantillaEvento.SelectedItem?.ToString() ?? "").Trim();

            var isVenta = string.Equals(evento, "VENTA", StringComparison.OrdinalIgnoreCase);
            var isCobro = string.Equals(evento, "COBRO", StringComparison.OrdinalIgnoreCase);
            var isNc = string.Equals(evento, "NC", StringComparison.OrdinalIgnoreCase)
                      || string.Equals(evento, "NOTA_CREDITO", StringComparison.OrdinalIgnoreCase)
                      || string.Equals(evento, "NOTA DE CREDITO", StringComparison.OrdinalIgnoreCase)
                      || string.Equals(evento, "DEVOLUCION", StringComparison.OrdinalIgnoreCase)
                      || string.Equals(evento, "DEVOLUCIÓN", StringComparison.OrdinalIgnoreCase);

            // VENTA
            lblVentaSubtotal.Visible = isVenta;
            nudVentaSubtotal.Visible = isVenta;
            lblVentaItbis.Visible = isVenta;
            nudVentaItbis.Visible = isVenta;
            lblVentaDescuento.Visible = isVenta;
            nudVentaDescuento.Visible = isVenta;
            lblVentaTotal.Visible = isVenta;
            nudVentaTotal.Visible = isVenta;
            btnVentaAutoCalcular.Visible = isVenta;

            // COBRO
            lblCobroMonto.Visible = isCobro;
            nudCobroMonto.Visible = isCobro;
            lblCobroEfectivo.Visible = isCobro;
            nudCobroEfectivo.Visible = isCobro;
            lblCobroTarjeta.Visible = isCobro;
            nudCobroTarjeta.Visible = isCobro;
            lblCobroTransfer.Visible = isCobro;
            nudCobroTransfer.Visible = isCobro;
            lblCobroTotal.Visible = isCobro;
            nudCobroTotal.Visible = isCobro;
            btnCobroAutoCalcular.Visible = isCobro;

            // NC / Devolución (Nota de crédito)
            lblNcBase.Visible = isNc;
            nudNcBase.Visible = isNc;
            lblNcItbis.Visible = isNc;
            nudNcItbis.Visible = isNc;
            lblNcTotal.Visible = isNc;
            nudNcTotal.Visible = isNc;
            btnNcAutoCalcular.Visible = isNc;

            // UX: total cobro siempre calculado
            if (isCobro)
            {
                nudCobroTotal.Enabled = false;
                nudCobroTotal.Value = 0;
            }

            if (isNc)
            {
                nudNcTotal.Enabled = false;
                nudNcTotal.Value = 0;
            }
        }

        private static void EnsurePlantillaConceptoColumn(DataTable dt)
        {
            if (!dt.Columns.Contains("Concepto"))
            {
                var col = new DataColumn("Concepto", typeof(string)) { DefaultValue = "MANUAL" };
                dt.Columns.Add(col);
            }

            if (!dt.Columns.Contains("Monto"))
            {
                var col = new DataColumn("Monto", typeof(decimal)) { DefaultValue = 0m };
                dt.Columns.Add(col);
            }

            foreach (DataRow r in dt.Rows)
            {
                var c = (r["Concepto"] == DBNull.Value ? "MANUAL" : Convert.ToString(r["Concepto"]) ?? "MANUAL").Trim();
                if (string.IsNullOrWhiteSpace(c)) c = "MANUAL";
                r["Concepto"] = c.ToUpperInvariant();

                if (r["Monto"] == DBNull.Value)
                    r["Monto"] = 0m;
            }
        }

        private void btnVentaAutoCalcular_Click(object? sender, EventArgs e)
        {
            try
            {
                if (_plantillaDt == null || _plantillaDt.Rows.Count == 0)
                {
                    MessageBox.Show("Primero cargue una plantilla.", "VENTA", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                EnsurePlantillaConceptoColumn(_plantillaDt);

                var subtotal = Convert.ToDecimal(nudVentaSubtotal.Value);
                var itbis = Convert.ToDecimal(nudVentaItbis.Value);
                var descuento = Convert.ToDecimal(nudVentaDescuento.Value);

                // Total calculado: Base - Descuento + ITBIS
                var total = subtotal - descuento + itbis;
                if (total < 0) total = 0m;

                // NumericUpDown bounds
                var totalToSet = total;
                if (totalToSet > nudVentaTotal.Maximum) totalToSet = nudVentaTotal.Maximum;
                nudVentaTotal.Value = totalToSet;

                void distribuir(string concepto, decimal valor)
                {
                    var rows = _plantillaDt!.Select($"Concepto = '{concepto}'");
                    if (rows.Length == 0) return;

                    var each = rows.Length == 0 ? 0m : Math.Round(valor / rows.Length, 2, MidpointRounding.AwayFromZero);
                    for (int i = 0; i < rows.Length; i++)
                    {
                        rows[i]["Monto"] = each;
                    }

                    // Ajuste por redondeo en la última fila
                    var sum = each * rows.Length;
                    var diff = Math.Round(valor - sum, 2, MidpointRounding.AwayFromZero);
                    if (rows.Length > 0 && diff != 0m)
                    {
                        var last = Convert.ToDecimal(rows[^1]["Monto"]);
                        rows[^1]["Monto"] = last + diff;
                    }
                }

                distribuir("TOTAL", total);
                distribuir("BASE", subtotal);
                distribuir("ITBIS", itbis);
                distribuir("DESCUENTO", descuento);

                gridPlantilla.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "VENTA - Auto-calcular", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCobroAutoCalcular_Click(object? sender, EventArgs e)
        {
            try
            {
                if (_plantillaDt == null || _plantillaDt.Rows.Count == 0)
                {
                    MessageBox.Show("Primero cargue una plantilla.", "COBRO", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                EnsurePlantillaConceptoColumn(_plantillaDt);

                var monto = Convert.ToDecimal(nudCobroMonto.Value);
                var ef = Convert.ToDecimal(nudCobroEfectivo.Value);
                var tc = Convert.ToDecimal(nudCobroTarjeta.Value);
                var tr = Convert.ToDecimal(nudCobroTransfer.Value);

                var sumSplit = ef + tc + tr;
                var totalCobro = sumSplit > 0 ? sumSplit : monto;
                if (totalCobro < 0) totalCobro = 0m;

                // Reflect in UI
                if (totalCobro > nudCobroTotal.Maximum) totalCobro = nudCobroTotal.Maximum;
                nudCobroTotal.Value = totalCobro;

                // Si el usuario usa split, reflejar el monto (total) para consistencia
                if (sumSplit > 0)
                {
                    var toSet = sumSplit;
                    if (toSet > nudCobroMonto.Maximum) toSet = nudCobroMonto.Maximum;
                    nudCobroMonto.Value = toSet;
                }

                void distribuir(string concepto, decimal valor)
                {
                    var rows = _plantillaDt!.Select($"Concepto = '{concepto}'");
                    if (rows.Length == 0) return;

                    var each = Math.Round(valor / rows.Length, 2, MidpointRounding.AwayFromZero);
                    for (int i = 0; i < rows.Length; i++)
                        rows[i]["Monto"] = each;

                    var sum = each * rows.Length;
                    var diff = Math.Round(valor - sum, 2, MidpointRounding.AwayFromZero);
                    if (diff != 0m)
                    {
                        var last = Convert.ToDecimal(rows[^1]["Monto"]);
                        rows[^1]["Monto"] = last + diff;
                    }
                }

                // Total (CxC vs Caja/Banco): soporta varias etiquetas para que la plantilla sea flexible
                distribuir("MONTO_COBRO", totalCobro);
                distribuir("CXC", totalCobro);
                distribuir("TOTAL", totalCobro);

                // Medios / destinos (AMBOS)
                distribuir("EFECTIVO", ef);
                distribuir("TARJETA", tc);
                distribuir("TRANSFERENCIA", tr);

                // Atajos: si la plantilla usa CAJA/BANCO directamente
                distribuir("CAJA", ef);
                distribuir("BANCO", tr);

                gridPlantilla.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "COBRO - Auto-calcular", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnNcAutoCalcular_Click(object? sender, EventArgs e)
        {
            try
            {
                if (_plantillaDt == null || _plantillaDt.Rows.Count == 0)
                {
                    MessageBox.Show("Primero cargue una plantilla.", "NC", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                EnsurePlantillaConceptoColumn(_plantillaDt);

                var baseNc = Convert.ToDecimal(nudNcBase.Value);
                var itbisNc = Convert.ToDecimal(nudNcItbis.Value);

                // Total NC: Base + ITBIS (monto positivo; la naturaleza D/C de la plantilla define el sentido)
                var totalNc = baseNc + itbisNc;
                if (totalNc < 0) totalNc = 0m;

                if (totalNc > nudNcTotal.Maximum) totalNc = nudNcTotal.Maximum;
                nudNcTotal.Value = totalNc;

                void distribuir(string concepto, decimal valor)
                {
                    var rows = _plantillaDt!.Select($"Concepto = '{concepto}'");
                    if (rows.Length == 0) return;

                    var each = Math.Round(valor / rows.Length, 2, MidpointRounding.AwayFromZero);
                    for (int i = 0; i < rows.Length; i++)
                        rows[i]["Monto"] = each;

                    var sum = each * rows.Length;
                    var diff = Math.Round(valor - sum, 2, MidpointRounding.AwayFromZero);
                    if (diff != 0m)
                    {
                        var last = Convert.ToDecimal(rows[^1]["Monto"]);
                        rows[^1]["Monto"] = last + diff;
                    }
                }

                distribuir("TOTAL_NC", totalNc);
                distribuir("BASE_NC", baseNc);
                distribuir("ITBIS_NC", itbisNc);

                // Compatibilidad: si el usuario usa TOTAL/BASE/ITBIS en NC por error, también lo llenamos
                distribuir("TOTAL", totalNc);
                distribuir("BASE", baseNc);
                distribuir("ITBIS", itbisNc);

                gridPlantilla.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "NC - Auto-calcular", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ConfigGrid()
        {
            grid.AutoGenerateColumns = false;
            grid.Columns.Clear();

            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colLinea",
                HeaderText = "Linea",
                DataPropertyName = "Linea",
                Width = 60
            });

            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colCodigo",
                HeaderText = "Codigo",
                DataPropertyName = "Codigo",
                Width = 110
            });

            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colCuenta",
                HeaderText = "Cuenta",
                DataPropertyName = "Cuenta",
                Width = 260
            });

            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colDesc",
                HeaderText = "Descripcion",
                DataPropertyName = "Descripcion",
                Width = 320
            });

            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colDeb",
                HeaderText = "Debito",
                DataPropertyName = "DebitoMoneda",
                Width = 110,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "N2" }
            });

            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colCred",
                HeaderText = "Credito",
                DataPropertyName = "CreditoMoneda",
                Width = 110,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "N2" }
            });

            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colDebBase",
                HeaderText = "DebitoBase",
                DataPropertyName = "DebitoBase",
                Width = 110,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "N2" }
            });

            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colCredBase",
                HeaderText = "CreditoBase",
                DataPropertyName = "CreditoBase",
                Width = 110,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "N2" }
            });
        }

        private void btnCrear_Click(object? sender, EventArgs e)
        {
            try
            {
                var desc = txtDescripcion.Text?.Trim();
                if (string.IsNullOrWhiteSpace(desc))
                {
                    MessageBox.Show("Digite la descripción del asiento.", "Contabilidad",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var origen = string.IsNullOrWhiteSpace(txtOrigen.Text) ? "MANUAL" : txtOrigen.Text.Trim();
                var origenId = Convert.ToInt64(nudOrigenId.Value);
                var moneda = cboMoneda.SelectedItem?.ToString() ?? "DOP";
                var tc = Convert.ToDecimal(nudTC.Value);

                var fecha = DateTime.Now;
                if (!ValidarPeriodoAbierto(fecha, "No se puede CREAR el asiento.")) return;

                using var cn = Db.GetOpenConnection();
                using var cmd = new SqlCommand("dbo.sp_Conta_Mov_Crear_wrap", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@Fecha", SqlDbType.DateTime2).Value = fecha;
                cmd.Parameters.Add("@Descripcion", SqlDbType.NVarChar, 150).Value = desc;
                cmd.Parameters.Add("@Origen", SqlDbType.VarChar, 20).Value = origen;
                cmd.Parameters.Add("@OrigenId", SqlDbType.BigInt).Value = origenId;
                cmd.Parameters.Add("@Usuario", SqlDbType.NVarChar, 60).Value = Environment.UserName;
                cmd.Parameters.Add("@MonedaCodigo", SqlDbType.VarChar, 3).Value = moneda;

                var pTc = cmd.Parameters.Add("@TasaCambio", SqlDbType.Decimal);
                pTc.Precision = 18; pTc.Scale = 6; pTc.Value = tc;

                var pMovId = cmd.Parameters.Add("@MovimientoId", SqlDbType.BigInt);
                pMovId.Direction = ParameterDirection.Output;

                var pNo = cmd.Parameters.Add("@NoAsiento", SqlDbType.VarChar, 30);
                pNo.Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();

                _movimientoId = Convert.ToInt64(pMovId.Value);
                _noAsiento = Convert.ToString(pNo.Value) ?? "(sin numero)";

                lblNoAsiento.Text = $"NoAsiento: {_noAsiento} | MovId: {_movimientoId}";

                CargarCabecera();
                CargarLineas();
                CargarTotales();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Crear asiento", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnAddLinea_Click(object? sender, EventArgs e)
        {
            try
            {
                if (_movimientoId <= 0)
                {
                    MessageBox.Show("Primero cree el movimiento.", "Contabilidad",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var codigo = txtCuentaCodigo.Text?.Trim();
                if (string.IsNullOrWhiteSpace(codigo))
                {
                    MessageBox.Show("Digite el código de la cuenta (ej: 1.1.01).", "Contabilidad",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var descLinea = txtLineaDesc.Text?.Trim();
                var deb = Convert.ToDecimal(nudDeb.Value);
                var cred = Convert.ToDecimal(nudCred.Value);

                if ((deb > 0 && cred > 0) || (deb <= 0 && cred <= 0))
                {
                    MessageBox.Show("Debe venir SOLO débito o SOLO crédito.", "Contabilidad",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                using var cn = Db.GetOpenConnection();
                using var cmd = new SqlCommand("dbo.sp_Conta_Mov_AddLineaPorCodigo", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@MovimientoId", SqlDbType.BigInt).Value = _movimientoId;
                cmd.Parameters.Add("@CuentaCodigo", SqlDbType.VarChar, 15).Value = codigo;

                // Descripción de línea:
                // - Si el usuario escribió una descripción en la línea, se usa esa.
                // - Si no, se usa la descripción de cabecera (txtDescripcion) como fallback.
                var descCab = (txtDescripcion.Text ?? "").Trim();
                var descLineaFinal = string.IsNullOrWhiteSpace(descLinea) ? descCab : descLinea;

                if (string.IsNullOrWhiteSpace(descLineaFinal))
                    cmd.Parameters.Add("@Descripcion", SqlDbType.NVarChar, 300).Value = DBNull.Value;
                else
                    cmd.Parameters.Add("@Descripcion", SqlDbType.NVarChar, 300).Value = descLineaFinal;

var pDeb = cmd.Parameters.Add("@DebitoMoneda", SqlDbType.Decimal);
                pDeb.Precision = 18; pDeb.Scale = 2; pDeb.Value = deb;

                var pCred = cmd.Parameters.Add("@CreditoMoneda", SqlDbType.Decimal);
                pCred.Precision = 18; pCred.Scale = 2; pCred.Value = cred;

                cmd.ExecuteNonQuery();

                // reset
                txtCuentaCodigo.Clear();
                txtLineaDesc.Clear();
                nudDeb.Value = 0;
                nudCred.Value = 0;

                CargarLineas();
                CargarTotales();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Agregar línea", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCerrar_Click(object? sender, EventArgs e)
        {
            try
            {
                if (_movimientoId <= 0)
                {
                    MessageBox.Show("No hay movimiento para cerrar.", "Contabilidad",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!ValidarPeriodoAbierto(DateTime.Now, "No se puede CERRAR el asiento.")) return;

                using var cn = Db.GetOpenConnection();
                using var cmd = new SqlCommand("dbo.sp_Conta_Mov_Cerrar", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@MovimientoId", SqlDbType.BigInt).Value = _movimientoId;

                cmd.ExecuteNonQuery();

                MessageBox.Show("Movimiento cerrado correctamente.", "Contabilidad",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                CargarTotales();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Cerrar movimiento", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        
        
        // =====================
        // Auditoría JSON automática (snapshot plantilla aplicada)
        // =====================
        
        // =====================
        // Bloqueo por período contable (ABIERTO/CERRADO)
        // =====================
        private bool ValidarPeriodoAbierto(DateTime fecha, string accion)
        {
            try
            {
                using var cn = Db.GetOpenConnection();
                using var cmd = new SqlCommand("dbo.sp_Conta_Periodo_EstaAbierto", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@Fecha", SqlDbType.DateTime2).Value = fecha;

                var pOk = cmd.Parameters.Add("@EstaAbierto", SqlDbType.Bit);
                pOk.Direction = ParameterDirection.Output;

                var pPeriodoId = cmd.Parameters.Add("@PeriodoId", SqlDbType.Int);
                pPeriodoId.Direction = ParameterDirection.Output;

                var pMsg = cmd.Parameters.Add("@Mensaje", SqlDbType.NVarChar, 200);
                pMsg.Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();

                var ok = (pOk.Value != DBNull.Value) && Convert.ToBoolean(pOk.Value);
                if (!ok)
                {
                    var msg = Convert.ToString(pMsg.Value) ?? "Período contable cerrado.";
                    MessageBox.Show($"{accion}\n\n{msg}", "Período contable", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                return true;
            }
            catch (SqlException ex)
            {
                // Si el SP aún no está creado, mostramos mensaje claro
                MessageBox.Show($"{accion}\n\nNo existe el SP dbo.sp_Conta_Periodo_EstaAbierto en la BD o falló su ejecución.\n\nDetalle: {ex.Message}",
                    "Período contable", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{accion}\n\n{ex.Message}", "Período contable", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
private void AuditoriaPlantilla_Aplicada(string modulo, string evento, string rol)
        {
            try
            {
                if (_movimientoId <= 0) return;
                if (_plantillaDt == null || _plantillaDt.Rows.Count == 0) return;

                // Snapshot de plantilla aplicada (solo filas con monto > 0)
                var lineas = new List<Dictionary<string, object?>>();
                foreach (DataRow r in _plantillaDt.Rows)
                {
                    var monto = r["Monto"] == DBNull.Value ? 0m : Convert.ToDecimal(r["Monto"]);
                    if (monto <= 0) continue;

                    lineas.Add(new Dictionary<string, object?>
                    {
                        ["Orden"] = r["Orden"],
                        ["Naturaleza"] = (r["Naturaleza"]?.ToString() ?? "").Trim(),
                        ["CuentaCodigo"] = (r["CuentaCodigo"]?.ToString() ?? "").Trim(),
                        ["CuentaNombre"] = (r["CuentaNombre"]?.ToString() ?? "").Trim(),
                        ["Concepto"] = (r["Concepto"]?.ToString() ?? "").Trim(),
                        ["Monto"] = monto
                    });
                }

                var snapshot = new Dictionary<string, object?>
                {
                    ["MovimientoId"] = _movimientoId,
                    ["NoAsiento"] = _noAsiento,
                    ["Modulo"] = modulo,
                    ["Evento"] = evento,
                    ["Rol"] = rol,
                    ["FechaApp"] = DateTime.Now,
                    ["UsuarioApp"] = Environment.UserName,
                    ["MonedaCodigo"] = cboMoneda.SelectedItem?.ToString(),
                    ["TasaCambio"] = Convert.ToDecimal(nudTC.Value),
                    ["DescripcionCab"] = (txtDescripcion.Text ?? "").Trim(),
                    ["Origen"] = (txtOrigen.Text ?? "").Trim(),
                    ["OrigenId"] = Convert.ToInt64(nudOrigenId.Value),
                    ["Lineas"] = lineas
                };

                var valoresEvento = BuildValoresEvento(evento);

                var configSnapshotJson = JsonSerializer.Serialize(snapshot, new JsonSerializerOptions { WriteIndented = false });
                var valoresEventoJson = JsonSerializer.Serialize(valoresEvento, new JsonSerializerOptions { WriteIndented = false });

                using var cn = Db.GetOpenConnection();
                using var cmd = new SqlCommand("dbo.sp_Conta_Auditoria_Insertar", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@MovimientoId", SqlDbType.BigInt).Value = _movimientoId;
                cmd.Parameters.Add("@Modulo", SqlDbType.VarChar, 30).Value = modulo;
                cmd.Parameters.Add("@Evento", SqlDbType.VarChar, 40).Value = evento;
                cmd.Parameters.Add("@Rol", SqlDbType.VarChar, 60).Value = rol;
                cmd.Parameters.Add("@Usuario", SqlDbType.VarChar, 50).Value = Environment.UserName;

                cmd.Parameters.Add("@ConfigSnapshotJson", SqlDbType.NVarChar).Value = configSnapshotJson;
                cmd.Parameters.Add("@ValoresEventoJson", SqlDbType.NVarChar).Value = valoresEventoJson;

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                // No bloquea la operación principal por auditoría
                MessageBox.Show(ex.Message, "Auditoría - Plantilla aplicada", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private Dictionary<string, object?> BuildValoresEvento(string evento)
        {
            var e = (evento ?? "").Trim().ToUpperInvariant();

            if (e == "VENTA")
            {
                var subtotal = Convert.ToDecimal(nudVentaSubtotal.Value);
                var itbis = Convert.ToDecimal(nudVentaItbis.Value);
                var descuento = Convert.ToDecimal(nudVentaDescuento.Value);
                var total = Convert.ToDecimal(nudVentaTotal.Value);

                return new Dictionary<string, object?>
                {
                    ["VENTA_SUBTOTAL"] = subtotal,
                    ["VENTA_ITBIS"] = itbis,
                    ["VENTA_DESCUENTO"] = descuento,
                    ["VENTA_TOTAL"] = total
                };
            }

            if (e == "COBRO")
            {
                var monto = Convert.ToDecimal(nudCobroMonto.Value);
                var ef = Convert.ToDecimal(nudCobroEfectivo.Value);
                var tc = Convert.ToDecimal(nudCobroTarjeta.Value);
                var tr = Convert.ToDecimal(nudCobroTransfer.Value);
                var total = Convert.ToDecimal(nudCobroTotal.Value);

                return new Dictionary<string, object?>
                {
                    ["COBRO_MONTO"] = monto,
                    ["COBRO_EFECTIVO"] = ef,
                    ["COBRO_TARJETA"] = tc,
                    ["COBRO_TRANSFERENCIA"] = tr,
                    ["COBRO_TOTAL"] = total
                };
            }

            // NC / Nota de crédito / Devolución
            if (e == "NC" || e == "NOTA_CREDITO" || e == "NOTA DE CREDITO" || e == "DEVOLUCION" || e == "DEVOLUCIÓN")
            {
                var baseNc = Convert.ToDecimal(nudNcBase.Value);
                var itbisNc = Convert.ToDecimal(nudNcItbis.Value);
                var totalNc = Convert.ToDecimal(nudNcTotal.Value);

                return new Dictionary<string, object?>
                {
                    ["NC_BASE"] = baseNc,
                    ["NC_ITBIS"] = itbisNc,
                    ["NC_TOTAL"] = totalNc
                };
            }

            // Default
            return new Dictionary<string, object?>
            {
                ["EVENTO"] = evento
            };
        }
private void CargarCabecera()
        {
            if (_movimientoId <= 0) return;

            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
SELECT NoAsiento, Fecha, Descripcion, Origen, OrigenId, Usuario, Estado, MonedaCodigo, TasaCambio
FROM dbo.ContabilidadMovimientoCab
WHERE MovimientoId = @Id;", cn);

            cmd.Parameters.Add("@Id", SqlDbType.BigInt).Value = _movimientoId;

            using var rd = cmd.ExecuteReader();
            if (!rd.Read()) return;

            _noAsiento = rd["NoAsiento"]?.ToString() ?? _noAsiento;
            txtDescripcion.Text = rd["Descripcion"]?.ToString() ?? "";
            txtOrigen.Text = rd["Origen"]?.ToString() ?? "";
            nudOrigenId.Value = rd["OrigenId"] == DBNull.Value ? 0 : Convert.ToDecimal(rd["OrigenId"]);
            cboMoneda.SelectedItem = rd["MonedaCodigo"]?.ToString() ?? "DOP";
            nudTC.Value = rd["TasaCambio"] == DBNull.Value ? 1 : Convert.ToDecimal(rd["TasaCambio"]);

            lblNoAsiento.Text = $"NoAsiento: {_noAsiento} | MovId: {_movimientoId}";
        }
private void CargarLineas()
        {
            if (_movimientoId <= 0)
            {
                grid.DataSource = null;
                return;
            }

            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
SELECT
    l.Linea,
    c.Codigo,
    c.Nombre AS Cuenta,
    l.Descripcion,
    l.DebitoMoneda,
    l.CreditoMoneda,
    l.DebitoBase,
    l.CreditoBase
FROM dbo.ContabilidadMovimientoLin l
INNER JOIN dbo.ContabilidadCatalogoCuenta c ON c.CuentaId = l.CuentaId
WHERE l.MovimientoId = @Id
ORDER BY l.Linea;", cn);

            cmd.Parameters.Add("@Id", SqlDbType.BigInt).Value = _movimientoId;

            var dt = new DataTable();
            using var da = new SqlDataAdapter(cmd);
            da.Fill(dt);

            grid.DataSource = dt;
        }

        private void CargarTotales()
        {
            if (_movimientoId <= 0)
            {
                PintarTotales(0, 0, 0, 0);
                return;
            }

            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand("dbo.sp_Conta_Mov_Totales", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@MovimientoId", SqlDbType.BigInt).Value = _movimientoId;

            using var rd = cmd.ExecuteReader();
            if (!rd.Read())
            {
                PintarTotales(0, 0, 0, 0);
                return;
            }

            var debMon = rd.IsDBNull(0) ? 0m : rd.GetDecimal(0);
            var credMon = rd.IsDBNull(1) ? 0m : rd.GetDecimal(1);
            var debBase = rd.IsDBNull(2) ? 0m : rd.GetDecimal(2);
            var credBase = rd.IsDBNull(3) ? 0m : rd.GetDecimal(3);

            PintarTotales(debMon, credMon, debBase, credBase);
        }

        private void PintarTotales(decimal debMon, decimal credMon, decimal debBase, decimal credBase)
        {
            lblTotDebMon.Text = $"Total Débito: {debMon:N2}";
            lblTotCredMon.Text = $"Total Crédito: {credMon:N2}";
            lblTotDebBase.Text = $"Total DébitoBase: {debBase:N2}";
            lblTotCredBase.Text = $"Total CréditoBase: {credBase:N2}";

            var dif = Math.Round(debBase - credBase, 2);
            lblDifBase.Text = $"DifBase: {dif:N2}";
            lblDifBase.ForeColor = dif == 0 ? Color.Green : Color.Red;
        }
    }
}
#nullable restore
