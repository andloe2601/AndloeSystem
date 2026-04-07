using Andloe.Data.DGII;
using Andloe.Logica.DGII;
using Andloe.Entidad;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Andloe.Presentacion
{
    public partial class FormAlanubeMonitor : Form
    {
        private readonly ECFDocumentoRepository _repo = new();
        private readonly ECFAlanubeFacade _alanube = new();

        private bool _cargando = false;
        private bool _working = false;

        public FormAlanubeMonitor()
        {
            InitializeComponent();
            KeyPreview = true;

            BuildUi();
            Wire();
            ConfigGrid();
        }

        private void BuildUi()
        {
            Text = "Monitor Alanube API";
            Font = new Font("Segoe UI", 10f);

            try
            {
                if (cboFiltroEstado.Items.Count == 0)
                {
                    cboFiltroEstado.Items.AddRange(new object[]
                    {
                        "TODOS",
                        "PENDIENTE",
                        "REGISTERED",
                        "ACCEPTED",
                        "REJECTED",
                        "ERROR"
                    });
                }

                if (cboFiltroEstado.SelectedItem == null)
                    cboFiltroEstado.SelectedItem = "TODOS";
            }
            catch { }
        }

        private void Wire()
        {
            Load += (_, __) => Refrescar();

            btnRefrescar.Click += (_, __) => Refrescar();
            btnEnviar.Click += (_, __) => EnviarAlanube();
            btnConsultar.Click += (_, __) => ConsultarAlanube();
            btnAbrirFactura.Click += (_, __) => AbrirFactura();
            btnCopiarEncf.Click += (_, __) => CopiarTextoColumna("ENCF", "eNCF copiado ✅");
            btnCopiarTrackId.Click += (_, __) => CopiarTextoColumna("TrackId", "TrackId copiado ✅");
            btnVerJson.Click += (_, __) => VerJson();

            cboFiltroEstado.SelectedIndexChanged += (_, __) =>
            {
                if (_cargando) return;
                if (!IsHandleCreated) return;
                Refrescar();
            };

            grid.SelectionChanged += (_, __) => SyncButtons();
            grid.CellFormatting += Grid_CellFormatting;
            grid.DoubleClick += (_, __) => ConsultarAlanube();

            KeyDown += (_, e) =>
            {
                if (e.KeyCode == Keys.F5)
                {
                    e.SuppressKeyPress = true;
                    Refrescar();
                }
            };
        }

        private void ConfigGrid()
        {
            grid.AutoGenerateColumns = true;
            grid.ReadOnly = true;
            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.MultiSelect = false;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            try
            {
                grid.BackgroundColor = Color.White;
                grid.BorderStyle = BorderStyle.FixedSingle;
                grid.EnableHeadersVisualStyles = false;
                grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(245, 246, 248);
                grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(25, 25, 25);
                grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10f, FontStyle.Bold);
                grid.ColumnHeadersHeight = 36;
                grid.RowHeadersVisible = false;
                grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(250, 250, 250);
                grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(220, 235, 255);
                grid.DefaultCellStyle.SelectionForeColor = Color.FromArgb(10, 10, 10);
                grid.RowTemplate.Height = 30;
            }
            catch { }

            SyncButtons();
        }

        private void SetWorking(bool working)
        {
            _working = working;
            UseWaitCursor = working;

            btnRefrescar.Enabled = !working;
            cboFiltroEstado.Enabled = !working;

            SyncButtons();
        }

        private void SyncButtons()
        {
            var hasRow = grid.CurrentRow != null && !grid.CurrentRow.IsNewRow;

            btnEnviar.Enabled = false;
            btnConsultar.Enabled = false;
            btnAbrirFactura.Enabled = false;
            btnCopiarEncf.Enabled = false;
            btnCopiarTrackId.Enabled = false;
            btnVerJson.Enabled = false;

            if (!hasRow || _working) return;

            var facturaId = SafeInt(GetCell("FacturaId"));
            if (facturaId <= 0) return;

            var encf = SafeStr(GetCell("ENCF")) ?? "";
            var trackId = SafeStr(GetCell("TrackId")) ?? "";

            btnEnviar.Enabled = !string.IsNullOrWhiteSpace(encf);
            btnConsultar.Enabled = !string.IsNullOrWhiteSpace(trackId);
            btnAbrirFactura.Enabled = true;
            btnCopiarEncf.Enabled = !string.IsNullOrWhiteSpace(encf);
            btnCopiarTrackId.Enabled = !string.IsNullOrWhiteSpace(trackId);
            btnVerJson.Enabled = true;
        }

        private string? GetEstadoFiltro()
        {
            var s = Convert.ToString(cboFiltroEstado.SelectedItem) ?? "TODOS";
            s = s.Trim().ToUpperInvariant();
            return s == "TODOS" ? null : s;
        }

        private void Refrescar()
        {
            if (_cargando) return;

            try
            {
                _cargando = true;
                UseWaitCursor = true;

                var estado = GetEstadoFiltro();

                // Reutiliza el mismo repo del monitor actual
                var dt = _repo.Listar(
                    desde: null,
                    hasta: null,
                    estado: null,
                    encfLike: null,
                    facturaId: null,
                    top: 500
                );

                // Filtro local por estado Alanube / DGII visible
                if (!string.IsNullOrWhiteSpace(estado))
                {
                    var dv = dt.DefaultView;
                    dv.RowFilter =
                        $"Convert(EstadoDGII, 'System.String') = '{estado.Replace("'", "''")}' " +
                        $"OR Convert(UltimoError, 'System.String') LIKE '%{estado.Replace("'", "''")}%'";
                    dv.Sort = "FechaGenerado DESC, ECFDocumentoId DESC";
                    grid.DataSource = dv;
                }
                else
                {
                    dt.DefaultView.Sort = "FechaGenerado DESC, ECFDocumentoId DESC";
                    grid.DataSource = dt.DefaultView;
                }

                try
                {
                    if (grid.Columns.Contains("XmlSinFirmar"))
                        grid.Columns["XmlSinFirmar"].Visible = false;

                    if (grid.Columns.Contains("XmlFirmado"))
                        grid.Columns["XmlFirmado"].Visible = false;

                    if (grid.Columns.Contains("XmlEnviado"))
                        grid.Columns["XmlEnviado"].Visible = false;

                    if (grid.Columns.Contains("XmlRespuesta"))
                        grid.Columns["XmlRespuesta"].Visible = false;

                    if (grid.Columns.Contains("RespuestaDGII"))
                        grid.Columns["RespuestaDGII"].Visible = false;

                    if (grid.Columns.Contains("UltimoError"))
                        grid.Columns["UltimoError"].Width = 260;

                    if (grid.Columns.Contains("ENCF"))
                        grid.Columns["ENCF"].Width = 150;

                    if (grid.Columns.Contains("TrackId"))
                        grid.Columns["TrackId"].Width = 190;

                    if (grid.Columns.Contains("EstadoDGII"))
                        grid.Columns["EstadoDGII"].Width = 110;
                }
                catch { }

                SyncButtons();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Alanube", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                UseWaitCursor = false;
                _cargando = false;
            }
        }

        private void EnviarAlanube()
        {
            if (grid.CurrentRow == null) return;

            var facturaId = SafeInt(GetCell("FacturaId"));
            var tipo = SafeInt(GetCell("TipoECF"));
            var encf = SafeStr(GetCell("ENCF")) ?? "";

            try
            {
                if (facturaId <= 0)
                    throw new Exception("FacturaId inválido.");

                if (tipo is not 31 and not 32)
                    throw new Exception("Alanube por ahora solo está preparado para E31 y E32.");

                if (string.IsNullOrWhiteSpace(encf))
                    throw new Exception("La factura no tiene eNCF.");

                SetWorking(true);

                var resp = _alanube.EnviarFactura(facturaId);

                var track = resp?.GetTrackOrId() ?? "";
                var status = resp?.Status ?? "";
                var legal = resp?.LegalStatus ?? "";
                var mensaje = resp?.Message ?? "";

                MessageBox.Show(
                    $"Enviado a Alanube ✅\n\n" +
                    $"FacturaId: {facturaId}\n" +
                    $"eNCF: {encf}\n" +
                    $"TrackId: {track}\n\n" +
                    $"Status: {status}\n" +
                    $"LegalStatus: {legal}\n" +
                    (string.IsNullOrWhiteSpace(mensaje) ? "" : $"\nMensaje: {mensaje}"),
                    "Alanube",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                Refrescar();
            }
            catch (Exception ex)
            {
                try { _repo.MarcarErrorPorFactura(facturaId, ex.Message); } catch { }

                MessageBox.Show(
                    ex.Message,
                    "Enviar Alanube",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                SetWorking(false);
            }
        }

        private void ConsultarAlanube()
        {
            if (grid.CurrentRow == null) return;

            var facturaId = SafeInt(GetCell("FacturaId"));
            var tipo = SafeInt(GetCell("TipoECF"));
            var trackId = SafeStr(GetCell("TrackId")) ?? "";
            var encf = SafeStr(GetCell("ENCF")) ?? "";

            try
            {
                if (facturaId <= 0)
                    throw new Exception("FacturaId inválido.");

                if (tipo is not 31 and not 32)
                    throw new Exception("Alanube por ahora solo está preparado para E31 y E32.");

                if (string.IsNullOrWhiteSpace(trackId))
                    throw new Exception("No hay TrackId. Primero envía a Alanube.");

                SetWorking(true);

                var resp = _alanube.ConsultarFactura(facturaId);

                var estado = resp?.Status ?? "";
                var legal = resp?.LegalStatus ?? "";
                var codigo = resp?.Code ?? "";
                var mensaje = resp?.Message ?? "";
                var raw = resp?.RawJson ?? "";

                MessageBox.Show(
    $"Consulta Alanube ✅\n\n" +
    $"FacturaId: {facturaId}\n" +
    $"eNCF: {encf}\n" +
    $"TrackId: {trackId}\n\n" +
    $"Status: {estado}\n" +
    $"LegalStatus: {legal}\n" +
    $"Code: {codigo}\n\n" +
    $"Mensaje:\n{mensaje}" +
    (string.IsNullOrWhiteSpace(raw) ? "" : $"\n\nRawJson:\n{raw}"),
    "Alanube",
    MessageBoxButtons.OK,
    MessageBoxIcon.Information
);


                Refrescar();
            }
            catch (Exception ex)
            {
                try { _repo.MarcarErrorPorFactura(facturaId, ex.Message); } catch { }

                MessageBox.Show(
                    ex.Message,
                    "Consultar Alanube",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                SetWorking(false);
            }
        }

        private void AbrirFactura()
        {
            var facturaId = SafeInt(GetCell("FacturaId"));
            if (facturaId <= 0) return;

            try
            {
                using var f = new FormFacturaV(facturaId);
                f.ShowDialog(this);
                Refrescar();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Abrir Factura", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CopiarTextoColumna(string colName, string mensajeOk)
        {
            var txt = SafeStr(GetCell(colName));
            if (string.IsNullOrWhiteSpace(txt)) return;

            try
            {
                Clipboard.SetText(txt);
                MessageBox.Show(mensajeOk, "Copiar", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch
            {
                MessageBox.Show("No se pudo copiar.", "Copiar", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void VerJson()
        {
            if (grid.CurrentRow == null) return;

            try
            {
                var facturaId = SafeInt(GetCell("FacturaId"));
                if (facturaId <= 0)
                    throw new Exception("FacturaId inválido.");

                var json = SafeStr(GetCell("UltimoError")) ?? "(Sin respuesta almacenada)";

                if (string.IsNullOrWhiteSpace(json))
                    json = SafeStr(GetCell("UltimoError")) ?? "(Sin respuesta almacenada)";

                using var f = new Form
                {
                    Text = $"Respuesta Alanube - FacturaId {facturaId}",
                    Width = 980,
                    Height = 680,
                    StartPosition = FormStartPosition.CenterParent
                };

                var txt = new TextBox
                {
                    Dock = DockStyle.Fill,
                    Multiline = true,
                    ScrollBars = ScrollBars.Both,
                    WordWrap = false,
                    Font = new Font("Consolas", 10f),
                    Text = json
                };

                var btnCopy = new Button
                {
                    Text = "Copiar",
                    Dock = DockStyle.Bottom,
                    Height = 42
                };
                btnCopy.Click += (_, __) =>
                {
                    try { Clipboard.SetText(txt.Text); } catch { }
                    MessageBox.Show("Texto copiado ✅", "Alanube", MessageBoxButtons.OK, MessageBoxIcon.Information);
                };

                f.Controls.Add(txt);
                f.Controls.Add(btnCopy);
                f.ShowDialog(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ver Respuesta", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Grid_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
        {
            try
            {
                var row = grid.Rows[e.RowIndex];

                var estadoDgii = Convert.ToString(row.Cells["EstadoDGII"]?.Value)?.Trim().ToUpperInvariant() ?? "";
                var ultimoError = Convert.ToString(row.Cells["UltimoError"]?.Value)?.Trim().ToUpperInvariant() ?? "";

                if (ultimoError.Contains("REJECTED"))
                {
                    e.CellStyle.BackColor = Color.FromArgb(255, 235, 238);
                    e.CellStyle.ForeColor = Color.FromArgb(183, 28, 28);
                    e.CellStyle.Font = new Font(grid.Font, FontStyle.Bold);
                    return;
                }

                if (ultimoError.Contains("REGISTERED"))
                {
                    e.CellStyle.BackColor = Color.FromArgb(232, 245, 255);
                    e.CellStyle.ForeColor = Color.FromArgb(0, 88, 140);
                    e.CellStyle.Font = new Font(grid.Font, FontStyle.Bold);
                    return;
                }

                switch (estadoDgii)
                {
                    case "PENDIENTE":
                        e.CellStyle.BackColor = Color.FromArgb(255, 248, 225);
                        e.CellStyle.ForeColor = Color.FromArgb(140, 90, 0);
                        e.CellStyle.Font = new Font(grid.Font, FontStyle.Bold);
                        break;

                    case "ACEPTADO":
                        e.CellStyle.BackColor = Color.FromArgb(232, 245, 233);
                        e.CellStyle.ForeColor = Color.FromArgb(27, 94, 32);
                        e.CellStyle.Font = new Font(grid.Font, FontStyle.Bold);
                        break;

                    case "RECHAZADO":
                    case "ERROR":
                        e.CellStyle.BackColor = Color.FromArgb(255, 235, 238);
                        e.CellStyle.ForeColor = Color.FromArgb(183, 28, 28);
                        e.CellStyle.Font = new Font(grid.Font, FontStyle.Bold);
                        break;
                }
            }
            catch
            {
            }
        }

        private object? GetCell(string colName)
        {
            try
            {
                if (grid.CurrentRow == null) return null;
                return grid.CurrentRow.Cells[colName]?.Value;
            }
            catch { return null; }
        }

        private static string? SafeStr(object? v)
        {
            var s = Convert.ToString(v);
            return string.IsNullOrWhiteSpace(s) ? null : s.Trim();
        }

        private static int SafeInt(object? v)
        {
            try { return Convert.ToInt32(v); } catch { return 0; }
        }
    }
}