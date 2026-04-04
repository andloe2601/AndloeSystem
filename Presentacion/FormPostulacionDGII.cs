using Andloe.Data;
using Andloe.Data.DGII;
using Andloe.Logica;
using Andloe.Logica.DGII;
using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Andloe.Presentacion
{
    public partial class FormPostulacionDGII : Form
    {
        private readonly string _connectionString;
        private readonly DGIIPostulacionSqlRepository _repo;
        private readonly DGIIPostulacionService _service;

        private bool _cargando = false;
        private bool _working = false;

        public FormPostulacionDGII()
        {
            InitializeComponent();

            _connectionString = ConfigManager.GetConnectionString();
            Db.Init(_connectionString);

            _repo = new DGIIPostulacionSqlRepository(_connectionString);
            _service = new DGIIPostulacionService(_repo);

            BuildUi();
            WireEvents();
        }

        private void BuildUi()
        {
            Text = "Postulación DGII - e-CF";
            KeyPreview = true;
            StartPosition = FormStartPosition.CenterScreen;
            WindowState = FormWindowState.Maximized;
            Font = new Font("Segoe UI", 9.75f);

            cboEstadoFiltro.Items.Clear();
            cboEstadoFiltro.Items.AddRange(new object[]
            {
                "TODOS",
                "BORRADOR",
                "VALIDADO",
                "GENERADO",
                "FIRMADO",
                "ENVIADO",
                "ACEPTADO",
                "RECHAZADO",
                "ERROR"
            });
            cboEstadoFiltro.SelectedIndex = 0;

            txtPfxPath.Text = @"C:\Certificados\empresa.pfx";
            txtUsuario.Text = "admin";

            ConfigGridPrincipal();
            ConfigGridValidaciones();
            ConfigGridHistorial();

            Load += (_, __) => Refrescar();
        }

        private void WireEvents()
        {
            btnRefrescar.Click += (_, __) => Refrescar();
            btnGenerarXml.Click += (_, __) => GenerarXml();
            btnFirmar.Click += (_, __) => FirmarXml();
            btnVerDetalle.Click += (_, __) => CargarDetalleSeleccionado();
            btnGuardarXmlSinFirmar.Click += (_, __) => GuardarXmlActual(txtXmlSinFirmar.Text, "postulacion-sin-firmar.xml");
            btnGuardarXmlFirmado.Click += (_, __) => GuardarXmlActual(txtXmlFirmado.Text, "postulacion-firmado.xml");
            btnCopiarXmlSinFirmar.Click += (_, __) => CopiarTexto(txtXmlSinFirmar.Text, "XML sin firmar copiado.");
            btnCopiarXmlFirmado.Click += (_, __) => CopiarTexto(txtXmlFirmado.Text, "XML firmado copiado.");
            btnBuscarPfx.Click += (_, __) => BuscarPfx();
            btnAbrirCarpetaPfx.Click += (_, __) => AbrirCarpetaPfx();

            if (btnValidar != null)
                btnValidar.Click += (_, __) => ValidarPostulacion();

            grid.SelectionChanged += (_, __) =>
            {
                if (_cargando) return;
                CargarDetalleSeleccionado();
            };

            cboEstadoFiltro.SelectedIndexChanged += (_, __) =>
            {
                if (!IsHandleCreated || _cargando) return;
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

        private void ConfigGridPrincipal()
        {
            ConfigurarGridBase(grid, false);

            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
            grid.RowTemplate.Height = 30;
        }

        private void ConfigGridValidaciones()
        {
            if (dgvValidaciones == null) return;

            ConfigurarGridBase(dgvValidaciones, true);
            dgvValidaciones.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvValidaciones.RowTemplate.Height = 26;
        }

        private void ConfigGridHistorial()
        {
            if (dgvHistorial == null) return;

            ConfigurarGridBase(dgvHistorial, false);
            dgvHistorial.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
            dgvHistorial.RowTemplate.Height = 26;
        }

        private static void ConfigurarGridBase(DataGridView dgv, bool fillMode)
        {
            if (dgv == null) return;

            dgv.AutoGenerateColumns = true;
            dgv.ReadOnly = true;
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.MultiSelect = false;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.RowHeadersVisible = false;
            dgv.BackgroundColor = Color.White;
            dgv.BorderStyle = BorderStyle.None;
            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(33, 37, 41);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            dgv.ColumnHeadersHeight = 34;
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 249, 250);
            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(207, 226, 255);
            dgv.DefaultCellStyle.SelectionForeColor = Color.Black;
            dgv.DefaultCellStyle.Font = new Font("Segoe UI", 9F);

            dgv.AutoSizeColumnsMode = fillMode
                ? DataGridViewAutoSizeColumnsMode.Fill
                : DataGridViewAutoSizeColumnsMode.DisplayedCells;
        }

        private void AplicarEstadoVisual(string estado)
        {
            estado = (estado ?? "").Trim().ToUpperInvariant();

            bool puedeGenerar = estado is "" or "BORRADOR" or "VALIDADO" or "ERROR" or "GENERADO";
            bool puedeFirmar = estado == "GENERADO";

            btnGenerarXml.Enabled = !_working && puedeGenerar;
            btnFirmar.Enabled = !_working && puedeFirmar;

            if (btnValidar != null)
                btnValidar.Enabled = !_working;
        }

        private void SetWorking(bool working, string? message = null)
        {
            _working = working;
            UseWaitCursor = working;

            btnRefrescar.Enabled = !working;
            btnGenerarXml.Enabled = !working;
            btnFirmar.Enabled = !working;
            btnVerDetalle.Enabled = !working;
            btnBuscarPfx.Enabled = !working;
            btnGuardarXmlSinFirmar.Enabled = !working;
            btnGuardarXmlFirmado.Enabled = !working;
            btnCopiarXmlSinFirmar.Enabled = !working;
            btnCopiarXmlFirmado.Enabled = !working;
            cboEstadoFiltro.Enabled = !working;
            nudPostulacionId.Enabled = !working;
            txtPfxPath.Enabled = !working;
            txtPfxPassword.Enabled = !working;
            txtUsuario.Enabled = !working;

            if (btnValidar != null)
                btnValidar.Enabled = !working;

            lblEstadoOperacion.Text = message ?? (working ? "Procesando..." : "Listo");
            Application.DoEvents();
        }

        private void Refrescar()
        {
            try
            {
                _cargando = true;
                SetWorking(true, "Cargando postulaciones...");

                using var cn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand(@"
SELECT
    DGIIPostulacionId,
    EmpresaId,
    PostulacionId,
    TipoRegistro,
    GrupoComprobante,
    FechaSolicitud,
    Estado,
    FechaCreacion,
    FechaFirmado,
    FechaEnviado,
    CertIssuer,
    CertSubject
FROM dbo.DGIIPostulacion
WHERE (@Estado IS NULL OR Estado = @Estado)
ORDER BY DGIIPostulacionId DESC;", cn);

                string estado = Convert.ToString(cboEstadoFiltro.SelectedItem) ?? "TODOS";
                object estadoParam = estado == "TODOS" ? DBNull.Value : estado;

                cmd.Parameters.Add("@Estado", SqlDbType.VarChar, 20).Value = estadoParam;

                var dt = new DataTable();
                using var da = new SqlDataAdapter(cmd);
                da.Fill(dt);

                grid.DataSource = dt;
                FormatearGridPrincipal();

                if (dt.Rows.Count > 0 && grid.Rows.Count > 0)
                    grid.Rows[0].Selected = true;

                lblResumen.Text = $"Registros: {dt.Rows.Count}";
                CargarDetalleSeleccionado();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Postulación DGII", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _cargando = false;
                SetWorking(false, "Listo");
            }
        }

        private void ValidarPostulacion()
        {
            try
            {
                long postulacionId = ObtenerPostulacionIdActual();
                if (postulacionId <= 0)
                {
                    MessageBox.Show("Seleccione una postulación válida.", "Postulación DGII", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                SetWorking(true, "Validando postulación...");

                var dt = _service.ValidarYRegistrar(postulacionId, txtUsuario.Text);

                CargarValidaciones(postulacionId);
                CargarHistorial(postulacionId);
                CargarDetalle(postulacionId);

                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("La postulación pasó validación correctamente.", "Postulación DGII", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    var sb = new StringBuilder();
                    sb.AppendLine("Se detectaron observaciones:");
                    sb.AppendLine();

                    foreach (DataRow row in dt.Rows)
                    {
                        if (dt.Columns.Contains("Mensaje"))
                            sb.AppendLine("• " + Convert.ToString(row["Mensaje"]));
                    }

                    MessageBox.Show(sb.ToString(), "Validación de Postulación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                RegistrarLogErrorOperacion("VALIDAR", ex);
                MessageBox.Show(ex.ToString(), "Postulación DGII", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SetWorking(false, "Listo");
            }
        }

        private void GenerarXml()
        {
            try
            {
                long postulacionId = ObtenerPostulacionIdActual();
                if (postulacionId <= 0)
                {
                    MessageBox.Show("Seleccione una postulación válida.", "Postulación DGII", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                SetWorking(true, "Generando XML de postulación...");

                string xml = _service.GenerarXml(postulacionId, txtUsuario.Text);

                txtXmlSinFirmar.Text = xml;
                txtXmlFirmado.Clear();

                CargarDetalle(postulacionId);
                CargarValidaciones(postulacionId);
                CargarHistorial(postulacionId);

                MessageBox.Show("XML de postulación generado correctamente.", "Postulación DGII", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                RegistrarLogErrorOperacion("GENERAR_XML", ex);
                MessageBox.Show(ex.ToString(), "Postulación DGII", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SetWorking(false, "Listo");
            }
        }

        private void FirmarXml()
        {
            try
            {
                long postulacionId = ObtenerPostulacionIdActual();
                if (postulacionId <= 0)
                {
                    MessageBox.Show("Seleccione una postulación válida.", "Postulación DGII", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtPfxPath.Text))
                {
                    MessageBox.Show("Indique la ruta del archivo .pfx.", "Postulación DGII", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!File.Exists(txtPfxPath.Text.Trim()))
                {
                    MessageBox.Show("El archivo .pfx indicado no existe.", "Postulación DGII", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                SetWorking(true, "Firmando XML de postulación...");

                var result = _service.FirmarPostulacion(
                    postulacionId,
                    txtPfxPath.Text.Trim(),
                    txtPfxPassword.Text,
                    txtUsuario.Text
                );

                txtXmlSinFirmar.Text = result.XmlSinFirmar;
                txtXmlFirmado.Text = result.XmlFirmado;

                txtDigestValue.Text = result.DigestValue ?? "";
                txtSignatureValue.Text = result.SignatureValue ?? "";
                txtCertIssuer.Text = result.CertIssuer ?? "";
                txtCertSubject.Text = result.CertSubject ?? "";
                txtThumbprint.Text = result.CertThumbprint ?? "";
                txtSerial.Text = result.CertSerialNumber ?? "";
                txtHash.Text = result.HashDocumento ?? "";
                txtFechaFirma.Text = result.FechaHoraFirma.ToString("dd-MM-yyyy HH:mm:ss");

                CargarDetalle(postulacionId);
                CargarValidaciones(postulacionId);
                CargarHistorial(postulacionId);

                MessageBox.Show("XML de postulación firmado correctamente.", "Postulación DGII", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                RegistrarLogErrorOperacion("FIRMAR_XML", ex);
                MessageBox.Show(ex.ToString(), "Postulación DGII", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SetWorking(false, "Listo");
            }
        }

        private void CargarDetalleSeleccionado()
        {
            long id = ObtenerPostulacionIdActual();
            if (id > 0)
                CargarDetalle(id);
        }

        private void CargarDetalle(long postulacionId)
        {
            try
            {
                if (postulacionId <= 0) return;

                if (postulacionId <= nudPostulacionId.Maximum)
                    nudPostulacionId.Value = postulacionId;

                using var cn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand(@"
SELECT TOP (1)
    DGIIPostulacionId,
    EmpresaId,
    PostulacionId,
    TipoRegistro,
    GrupoComprobante,
    FechaSolicitud,
    Estado,
    XmlSinFirmar,
    XmlFirmado,
    DigestValue,
    SignatureValue,
    CanonicalizationMethod,
    SignatureMethod,
    CertThumbprint,
    CertSerialNumber,
    CertIssuer,
    CertSubject,
    HashDocumento,
    FechaCreacion,
    FechaFirmado,
    FechaEnviado
FROM dbo.DGIIPostulacion
WHERE DGIIPostulacionId = @Id;", cn);

                cmd.Parameters.Add("@Id", SqlDbType.BigInt).Value = postulacionId;

                cn.Open();
                using var rd = cmd.ExecuteReader();

                if (!rd.Read())
                {
                    LimpiarDetalle();
                    return;
                }

                txtPostulacionGuid.Text = Valor(rd, "PostulacionId");
                txtTipoRegistro.Text = Valor(rd, "TipoRegistro");
                txtGrupoComprobante.Text = Valor(rd, "GrupoComprobante");
                txtEstado.Text = Valor(rd, "Estado");
                txtFechaSolicitud.Text = Valor(rd, "FechaSolicitud");
                txtFechaGenerado.Text = Valor(rd, "FechaCreacion");
                txtFechaFirma.Text = Valor(rd, "FechaFirmado");
                txtFechaEnviado.Text = Valor(rd, "FechaEnviado");

                txtDigestValue.Text = Valor(rd, "DigestValue");
                txtSignatureValue.Text = Valor(rd, "SignatureValue");
                txtCanonicalization.Text = Valor(rd, "CanonicalizationMethod");
                txtSignatureMethod.Text = Valor(rd, "SignatureMethod");
                txtThumbprint.Text = Valor(rd, "CertThumbprint");
                txtSerial.Text = Valor(rd, "CertSerialNumber");
                txtCertIssuer.Text = Valor(rd, "CertIssuer");
                txtCertSubject.Text = Valor(rd, "CertSubject");
                txtHash.Text = Valor(rd, "HashDocumento");

                txtXmlSinFirmar.Text = Valor(rd, "XmlSinFirmar");
                txtXmlFirmado.Text = Valor(rd, "XmlFirmado");

                AplicarEstadoVisual(txtEstado.Text);
                CargarValidaciones(postulacionId);
                CargarHistorial(postulacionId);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Postulación DGII", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarValidaciones(long postulacionId)
        {
            try
            {
                if (dgvValidaciones == null) return;

                var dt = _repo.ValidarPostulacion(postulacionId);
                dgvValidaciones.DataSource = dt;
                FormatearGridValidaciones();

                if (dt.Rows.Count == 0)
                {
                    lblValidacionResumen.Text = "Validación: OK";
                    lblValidacionResumen.ForeColor = Color.FromArgb(25, 135, 84);
                    grpValidaciones.ForeColor = Color.FromArgb(25, 135, 84);
                }
                else
                {
                    lblValidacionResumen.Text = $"Validación: {dt.Rows.Count} observación(es)";
                    lblValidacionResumen.ForeColor = Color.FromArgb(220, 53, 69);
                    grpValidaciones.ForeColor = Color.FromArgb(220, 53, 69);
                }
            }
            catch
            {
                if (lblValidacionResumen != null)
                {
                    lblValidacionResumen.Text = "Validación: error";
                    lblValidacionResumen.ForeColor = Color.FromArgb(220, 53, 69);
                    grpValidaciones.ForeColor = Color.FromArgb(220, 53, 69);
                }
            }
        }

        private void CargarHistorial(long postulacionId)
        {
            try
            {
                if (dgvHistorial == null) return;

                var dt = _repo.ObtenerLog(postulacionId);
                dgvHistorial.DataSource = dt;
                FormatearGridHistorial();
            }
            catch
            {
            }
        }

        private void FormatearGridPrincipal()
        {
            if (grid?.Columns == null) return;

            OcultarColumnaSiExiste(grid, "EmpresaId");
            AjustarAnchoSiExiste(grid, "DGIIPostulacionId", 90);
            AjustarAnchoSiExiste(grid, "Estado", 110);
            AjustarAnchoSiExiste(grid, "TipoRegistro", 110);
            AjustarAnchoSiExiste(grid, "GrupoComprobante", 130);
        }

        private void FormatearGridValidaciones()
        {
            if (dgvValidaciones?.Columns == null) return;

            dgvValidaciones.ClearSelection();

            foreach (DataGridViewColumn col in dgvValidaciones.Columns)
            {
                if (col.Name.Equals("Mensaje", StringComparison.OrdinalIgnoreCase))
                    col.HeaderText = "Observación";
            }
        }

        private void FormatearGridHistorial()
        {
            if (dgvHistorial?.Columns == null) return;

            dgvHistorial.ClearSelection();

            AjustarAnchoSiExiste(dgvHistorial, "DGIIPostulacionLogId", 70);
            AjustarAnchoSiExiste(dgvHistorial, "Evento", 110);
            AjustarAnchoSiExiste(dgvHistorial, "EstadoAnterior", 110);
            AjustarAnchoSiExiste(dgvHistorial, "EstadoNuevo", 110);
            AjustarAnchoSiExiste(dgvHistorial, "FechaEvento", 140);
            AjustarAnchoSiExiste(dgvHistorial, "Usuario", 100);
            AjustarAnchoSiExiste(dgvHistorial, "Origen", 180);
        }

        private static void OcultarColumnaSiExiste(DataGridView dgv, string nombre)
        {
            if (dgv.Columns.Contains(nombre))
                dgv.Columns[nombre].Visible = false;
        }

        private static void AjustarAnchoSiExiste(DataGridView dgv, string nombre, int ancho)
        {
            if (dgv.Columns.Contains(nombre))
                dgv.Columns[nombre].Width = ancho;
        }

        private void RegistrarLogErrorOperacion(string evento, Exception ex)
        {
            try
            {
                long postulacionId = ObtenerPostulacionIdActual();
                if (postulacionId <= 0) return;

                string? estadoActual = string.IsNullOrWhiteSpace(txtEstado.Text) ? null : txtEstado.Text.Trim();

                _repo.InsertarLog(
                    postulacionId,
                    evento,
                    estadoActual,
                    "ERROR",
                    ex.Message,
                    txtUsuario.Text,
                    "FormPostulacionDGII"
                );
            }
            catch
            {
                // evitar romper la operación por falla en log
            }
        }

        private void LimpiarDetalle()
        {
            txtPostulacionGuid.Clear();
            txtTipoRegistro.Clear();
            txtGrupoComprobante.Clear();
            txtEstado.Clear();
            txtFechaSolicitud.Clear();
            txtFechaGenerado.Clear();
            txtFechaFirma.Clear();
            txtFechaEnviado.Clear();

            txtDigestValue.Clear();
            txtSignatureValue.Clear();
            txtCanonicalization.Clear();
            txtSignatureMethod.Clear();
            txtThumbprint.Clear();
            txtSerial.Clear();
            txtCertIssuer.Clear();
            txtCertSubject.Clear();
            txtHash.Clear();

            txtXmlSinFirmar.Clear();
            txtXmlFirmado.Clear();

            if (dgvValidaciones != null)
                dgvValidaciones.DataSource = null;

            if (dgvHistorial != null)
                dgvHistorial.DataSource = null;

            if (lblValidacionResumen != null)
            {
                lblValidacionResumen.Text = "Validación: pendiente";
                lblValidacionResumen.ForeColor = Color.FromArgb(108, 117, 125);
            }

            AplicarEstadoVisual("");
        }

        private long ObtenerPostulacionIdActual()
        {
            if (grid.CurrentRow != null && grid.CurrentRow.Cells["DGIIPostulacionId"]?.Value != null)
            {
                if (long.TryParse(Convert.ToString(grid.CurrentRow.Cells["DGIIPostulacionId"].Value), out long fromGrid))
                    return fromGrid;
            }

            return Convert.ToInt64(nudPostulacionId.Value);
        }

        private void BuscarPfx()
        {
            using var ofd = new OpenFileDialog
            {
                Title = "Seleccionar certificado PFX/P12",
                Filter = "Certificados PKCS#12 (*.pfx;*.p12)|*.pfx;*.p12|Certificado PFX (*.pfx)|*.pfx|Certificado P12 (*.p12)|*.p12|Todos los archivos (*.*)|*.*",
                CheckFileExists = true
            };

            if (ofd.ShowDialog(this) == DialogResult.OK)
                txtPfxPath.Text = ofd.FileName;
        }

        private void AbrirCarpetaPfx()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtPfxPath.Text))
                    return;

                var path = txtPfxPath.Text.Trim();
                var dir = File.Exists(path) ? Path.GetDirectoryName(path) : path;

                if (!string.IsNullOrWhiteSpace(dir) && Directory.Exists(dir))
                    Process.Start("explorer.exe", dir);
            }
            catch
            {
            }
        }

        private void GuardarXmlActual(string xml, string defaultName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(xml))
                {
                    MessageBox.Show("No hay XML para guardar.", "Postulación DGII", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                using var sfd = new SaveFileDialog
                {
                    Title = "Guardar XML",
                    FileName = defaultName,
                    Filter = "Archivo XML (*.xml)|*.xml|Todos los archivos (*.*)|*.*"
                };

                if (sfd.ShowDialog(this) != DialogResult.OK)
                    return;

                File.WriteAllText(sfd.FileName, xml);
                MessageBox.Show("Archivo guardado correctamente.", "Postulación DGII", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Postulación DGII", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CopiarTexto(string text, string okMessage)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(text))
                {
                    MessageBox.Show("No hay contenido para copiar.", "Postulación DGII", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                Clipboard.SetText(text);
                MessageBox.Show(okMessage, "Postulación DGII", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Postulación DGII", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static string Valor(IDataRecord rd, string field)
        {
            int ord = rd.GetOrdinal(field);
            if (rd.IsDBNull(ord)) return "";
            return Convert.ToString(rd.GetValue(ord)) ?? "";
        }
    }
}