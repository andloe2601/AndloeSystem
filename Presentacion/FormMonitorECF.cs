using Andloe.Data.DGII;
using Andloe.Entidad;
using Andloe.Logica;
using Andloe.Logica.DGII;
using Andloe.Presentacion;
using Logica;
using Microsoft.VisualBasic;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace Andloe.Presentacion
{
    public partial class FormMonitorECF : Form
    {
        private readonly ECFDocumentoRepository _repo = new();
        private readonly ECFService _svc = new();
        private readonly ECFAlanubeFacade _alanube = new();

        private bool _cargando = false;
        private bool _working = false;

        public FormMonitorECF()
        {
            InitializeComponent();
            KeyPreview = true;

            BuildUi();
            Wire();
            ConfigGrid();
        }

        private void BuildUi()
        {
            Text = "Monitor e-CF (DGII)";
            Font = new Font("Segoe UI", 10f);

            try
            {
                if (cboEstado.Items.Count == 0)
                {
                    cboEstado.Items.AddRange(new object[]
                    {
                        "PENDIENTE",
                        "FIRMADO",
                        "ENVIADO",
                        "ACEPTADO",
                        "RECHAZADO",
                        "ERROR",
                        "TODOS"
                    });
                }

                if (cboEstado.SelectedItem == null)
                    cboEstado.SelectedItem = "PENDIENTE";
            }
            catch { }
        }

        private void Wire()
        {
            Load += (_, __) => Refrescar();

            btnRefrescar.Click += (_, __) => Refrescar();
            btnVerXml.Click += (_, __) => VerXml();
            btnRegenerarXml.Click += (_, __) => RegenerarXml();
            btnCopiarEncf.Click += (_, __) => CopiarEncf();
            btnAbrirFactura.Click += (_, __) => AbrirFactura();

            btnFirmar.Click += (_, __) => Firmar();
            btnEnviar.Click += (_, __) => Enviar();
            btnConsultar.Click += (_, __) => Consultar();

            btnEnviarAlanube.Click += (_, __) => EnviarAlanube();
            btnConsultarAlanube.Click += (_, __) => ConsultarAlanube();

            grid.DoubleClick += (_, __) => VerXml();
            grid.SelectionChanged += (_, __) => SyncButtons();
            grid.CellFormatting += Grid_CellFormatting;

            cboEstado.SelectedIndexChanged += (_, __) =>
            {
                if (!IsHandleCreated) return;
                if (_cargando) return;
                Refrescar();
            };

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
            cboEstado.Enabled = !working;
            btnEnviarAlanube.Enabled = !working;
            btnConsultarAlanube.Enabled = !working;

            SyncButtons();
        }

        private void SyncButtons()
        {
            var hasRow = grid.CurrentRow != null && !grid.CurrentRow.IsNewRow;

            btnVerXml.Enabled = hasRow && !_working;
            btnRegenerarXml.Enabled = hasRow && !_working;
            btnCopiarEncf.Enabled = hasRow && !_working;
            btnAbrirFactura.Enabled = hasRow && !_working;

            btnFirmar.Enabled = false;
            btnEnviar.Enabled = false;
            btnConsultar.Enabled = false;
            btnEnviarAlanube.Enabled = false;
            btnConsultarAlanube.Enabled = false;

            if (!hasRow || _working) return;

            var facturaId = SafeInt(GetCell("FacturaId"));
            var tipo = SafeInt(GetCell("TipoECF"));
            var encf = SafeStr(GetCell("ENCF")) ?? "";
            var trackId = SafeStr(GetCell("TrackId")) ?? "";
            var estado = (SafeStr(GetCell("EstadoDGII")) ?? "").Trim().ToUpperInvariant();

            if (facturaId <= 0 || tipo <= 0) return;

            try
            {
                var xmlSinFirmar = _repo.ObtenerXmlSinFirmar(facturaId);
                var xmlFirmado = _repo.ObtenerXmlFirmado(facturaId);

                btnFirmar.Enabled = !string.IsNullOrWhiteSpace(xmlSinFirmar);
                btnEnviar.Enabled = !string.IsNullOrWhiteSpace(xmlFirmado);
                btnConsultar.Enabled = !string.IsNullOrWhiteSpace(trackId);

                btnEnviarAlanube.Enabled =
                    !string.IsNullOrWhiteSpace(encf) &&
                    !string.IsNullOrWhiteSpace(xmlFirmado) &&
                    estado != "ACEPTADO";

                btnConsultarAlanube.Enabled =
                    !string.IsNullOrWhiteSpace(trackId);
            }
            catch
            {
                btnFirmar.Enabled = false;
                btnEnviar.Enabled = false;
                btnConsultar.Enabled = false;
                btnEnviarAlanube.Enabled = false;
                btnConsultarAlanube.Enabled = false;
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

                if (tipo <= 0)
                    throw new Exception("TipoECF inválido.");

                if (string.IsNullOrWhiteSpace(encf))
                    throw new Exception("La factura no tiene eNCF.");

                SetWorking(true);

                var resp = _alanube.EnviarFactura(facturaId);

                var track = resp?.GetTrackOrId() ?? "";
                var estado = resp?.Status ?? resp?.LegalStatus ?? "OK";

                _repo.GuardarRespuestaAlanubePorFactura(
                    facturaId,
                    track,
                    resp?.Status,
                    resp?.LegalStatus,
                    null,
                    resp?.Message,
                    resp?.RawJson
                );

                MessageBox.Show(
                    $"Enviado a Alanube ✅\n\nTrack/Id: {track}\nEstado: {estado}",
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

            try
            {
                if (facturaId <= 0)
                    throw new Exception("FacturaId inválido.");

                if (tipo <= 0)
                    throw new Exception("TipoECF inválido.");

                if (string.IsNullOrWhiteSpace(trackId))
                    throw new Exception("No hay TrackId. Primero envía a Alanube.");

                SetWorking(true);

                var resp = _alanube.ConsultarFactura(facturaId);

                _repo.RegistrarConsultaAlanubePorFactura(
                    facturaId,
                    resp?.Status,
                    resp?.LegalStatus,
                    resp?.Code,
                    resp?.Message,
                    resp?.RawJson
                );

                var estado = resp?.Status ?? "";
                var legal = resp?.LegalStatus ?? "";
                var codigo = resp?.Code ?? "";
                var mensaje = resp?.Message ?? "";

                MessageBox.Show(
                    $"Consulta Alanube ✅\n\nStatus: {estado}\nLegalStatus: {legal}\nCode: {codigo}\nMensaje: {mensaje}",
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
                    "Consultar Alanube",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                SetWorking(false);
            }
        }

        private string? GetEstadoFiltro()
        {
            var s = Convert.ToString(cboEstado.SelectedItem) ?? "PENDIENTE";
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

                var dt = _repo.Listar(
                    desde: null,
                    hasta: null,
                    estado: estado,
                    encfLike: null,
                    facturaId: null,
                    top: 500
                );

                dt.DefaultView.Sort = "FechaGenerado DESC, ECFDocumentoId DESC";
                grid.DataSource = dt.DefaultView;

                try
                {
                    if (grid.Columns.Contains("RespuestaDGII"))
                        grid.Columns["RespuestaDGII"].Visible = false;

                    if (grid.Columns.Contains("XmlSinFirmar"))
                        grid.Columns["XmlSinFirmar"].Visible = false;

                    if (grid.Columns.Contains("XmlFirmado"))
                        grid.Columns["XmlFirmado"].Visible = false;

                    if (grid.Columns.Contains("UltimoError"))
                        grid.Columns["UltimoError"].Width = 260;

                    if (grid.Columns.Contains("ENCF"))
                        grid.Columns["ENCF"].Width = 150;

                    if (grid.Columns.Contains("TrackId"))
                        grid.Columns["TrackId"].Width = 170;
                }
                catch { }

                SyncButtons();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Monitor e-CF", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                UseWaitCursor = false;
                _cargando = false;
            }
        }

        private void VerXml()
        {
            if (grid.CurrentRow == null) return;

            try
            {
                var facturaId = SafeInt(GetCell("FacturaId"));
                if (facturaId <= 0)
                {
                    MessageBox.Show("FacturaId inválido.", "XML", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var xml = _repo.ObtenerXmlSinFirmar(facturaId);
                if (string.IsNullOrWhiteSpace(xml))
                {
                    MessageBox.Show("Esta factura no tiene XML guardado todavía.", "XML", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                using var f = new Form
                {
                    Text = $"XML e-CF - FacturaId {facturaId}",
                    Width = 980,
                    Height = 700,
                    StartPosition = FormStartPosition.CenterParent
                };

                var txt = new TextBox
                {
                    Dock = DockStyle.Fill,
                    Multiline = true,
                    ScrollBars = ScrollBars.Both,
                    WordWrap = false,
                    Font = new Font("Consolas", 10f),
                    Text = xml
                };

                var btnCopy = new Button
                {
                    Text = "Copiar XML",
                    Dock = DockStyle.Bottom,
                    Height = 44
                };
                btnCopy.Click += (_, __) =>
                {
                    try { Clipboard.SetText(xml); } catch { }
                    MessageBox.Show("XML copiado ✅", "XML", MessageBoxButtons.OK, MessageBoxIcon.Information);
                };

                f.Controls.Add(txt);
                f.Controls.Add(btnCopy);
                f.ShowDialog(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "XML", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RegenerarXml()
        {
            if (grid.CurrentRow == null) return;

            var facturaId = SafeInt(GetCell("FacturaId"));
            var tipo = SafeInt(GetCell("TipoECF"));
            var encf = SafeStr(GetCell("ENCF")) ?? "";

            try
            {
                if (facturaId <= 0 || tipo <= 0 || string.IsNullOrWhiteSpace(encf))
                    throw new Exception("Fila inválida (FacturaId/TipoECF/ENCF).");

                _svc.GenerarXmlPendiente(facturaId, tipo, encf);
                _repo.MarcarPendientePorFactura(facturaId);

                MessageBox.Show("XML regenerado y marcado como PENDIENTE ✅", "e-CF", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Refrescar();
            }
            catch (Exception ex)
            {
                try { _repo.MarcarErrorPorFactura(facturaId, ex.Message); } catch { }
                MessageBox.Show(ex.Message, "Regenerar XML", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CopiarEncf()
        {
            var encf = SafeStr(GetCell("ENCF"));
            if (string.IsNullOrWhiteSpace(encf)) return;

            try
            {
                Clipboard.SetText(encf);
                MessageBox.Show("eNCF copiado ✅", "Copiar", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch
            {
                MessageBox.Show("No se pudo copiar.", "Copiar", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

        private void Firmar()
        {
            if (grid.CurrentRow == null) return;

            var facturaId = SafeInt(GetCell("FacturaId"));

            try
            {
                if (facturaId <= 0)
                    throw new Exception("FacturaId inválido.");

                using var ofd = new OpenFileDialog
                {
                    Title = "Selecciona certificado PFX/P12",
                    Filter = "Certificados PKCS#12 (*.pfx;*.p12)|*.pfx;*.p12|Certificado PFX (*.pfx)|*.pfx|Certificado P12 (*.p12)|*.p12|Todos los archivos (*.*)|*.*",
                    Multiselect = false
                };

                if (ofd.ShowDialog(this) != DialogResult.OK)
                    return;

                var pass = Interaction.InputBox("Password del PFX (si aplica):", "PFX Password", "");

                SetWorking(true);

                _svc.FirmarFactura(facturaId, ofd.FileName, string.IsNullOrWhiteSpace(pass) ? null : pass);
                _repo.MarcarFirmadoPorFactura(facturaId);

                MessageBox.Show("XML firmado ✅", "Firmar", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Refrescar();
            }
            catch (Exception ex)
            {
                try { _repo.MarcarErrorPorFactura(facturaId, ex.Message); } catch { }
                MessageBox.Show(ex.Message, "Firmar", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SetWorking(false);
            }
        }

        private void Enviar()
        {
            if (grid.CurrentRow == null) return;

            var ecfId = SafeLong(GetCell("ECFDocumentoId"));
            var facturaId = SafeInt(GetCell("FacturaId"));
            var tipo = SafeInt(GetCell("TipoECF"));
            var encf = SafeStr(GetCell("ENCF")) ?? "";

            try
            {
                if (ecfId <= 0 || facturaId <= 0 || tipo <= 0 || string.IsNullOrWhiteSpace(encf))
                    throw new Exception("Fila inválida para enviar.");

                SetWorking(true);

                _svc.EnviarFactura(ecfId, facturaId, tipo, encf);

                MessageBox.Show("Enviado ✅", "DGII", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Refrescar();
            }
            catch (Exception ex)
            {
                try { _repo.MarcarErrorPorFactura(facturaId, ex.Message); } catch { }
                MessageBox.Show(ex.Message, "Enviar", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SetWorking(false);
            }
        }

        private void Consultar()
        {
            if (grid.CurrentRow == null) return;

            var ecfId = SafeLong(GetCell("ECFDocumentoId"));
            var facturaId = SafeInt(GetCell("FacturaId"));
            var trackId = SafeStr(GetCell("TrackId")) ?? "";

            try
            {
                if (ecfId <= 0 || facturaId <= 0)
                    throw new Exception("Fila inválida.");

                if (string.IsNullOrWhiteSpace(trackId))
                    throw new Exception("No hay TrackId. Primero envía.");

                SetWorking(true);

                _svc.Consultar(ecfId, facturaId, trackId);

                MessageBox.Show("Consulta realizada ✅", "DGII", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Refrescar();
            }
            catch (Exception ex)
            {
                try { _repo.MarcarErrorPorFactura(facturaId, ex.Message); } catch { }
                MessageBox.Show(ex.Message, "Consultar", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SetWorking(false);
            }
        }

        private void Grid_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
        {
            try
            {
                if (grid.Columns[e.ColumnIndex].Name != "EstadoDGII" &&
                    grid.Columns[e.ColumnIndex].DataPropertyName != "EstadoDGII")
                    return;

                var estado = Convert.ToString(e.Value)?.Trim().ToUpperInvariant() ?? "";
                if (string.IsNullOrWhiteSpace(estado)) return;

                switch (estado)
                {
                    case "PENDIENTE":
                        e.CellStyle.BackColor = Color.FromArgb(255, 248, 225);
                        e.CellStyle.ForeColor = Color.FromArgb(140, 90, 0);
                        e.CellStyle.Font = new Font(grid.Font, FontStyle.Bold);
                        break;

                    case "FIRMADO":
                        e.CellStyle.BackColor = Color.FromArgb(232, 245, 255);
                        e.CellStyle.ForeColor = Color.FromArgb(0, 88, 140);
                        e.CellStyle.Font = new Font(grid.Font, FontStyle.Bold);
                        break;

                    case "ENVIADO":
                        e.CellStyle.BackColor = Color.FromArgb(227, 242, 253);
                        e.CellStyle.ForeColor = Color.FromArgb(13, 71, 161);
                        e.CellStyle.Font = new Font(grid.Font, FontStyle.Bold);
                        break;

                    case "ACEPTADO":
                        e.CellStyle.BackColor = Color.FromArgb(232, 245, 233);
                        e.CellStyle.ForeColor = Color.FromArgb(27, 94, 32);
                        e.CellStyle.Font = new Font(grid.Font, FontStyle.Bold);
                        break;

                    case "RECHAZADO":
                        e.CellStyle.BackColor = Color.FromArgb(255, 235, 238);
                        e.CellStyle.ForeColor = Color.FromArgb(183, 28, 28);
                        e.CellStyle.Font = new Font(grid.Font, FontStyle.Bold);
                        break;

                    case "ERROR":
                        e.CellStyle.BackColor = Color.FromArgb(255, 243, 224);
                        e.CellStyle.ForeColor = Color.FromArgb(191, 54, 12);
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

        private static long SafeLong(object? v)
        {
            try { return Convert.ToInt64(v); } catch { return 0L; }
        }
    }
}