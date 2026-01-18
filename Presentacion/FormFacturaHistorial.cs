using Andloe.Data;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Andloe.Entidad.Facturacion;

namespace Andloe.Presentacion
{
    public partial class FormFacturaHistorial : Form
    {
        private readonly FacturaRepository _facRepo = new();
        private int _ultimaFacturaAvisada = -1;

        public FormFacturaHistorial()
        {
            InitializeComponent();
            EstilosModernos();
            InitCombos();
            WireEvents();
            CargarDefault();
        }

        private void EstilosModernos()
        {
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);

            void Flat(Button b)
            {
                b.FlatStyle = FlatStyle.Flat;
                b.FlatAppearance.BorderSize = 0;
                b.Cursor = Cursors.Hand;
            }

            Flat(btnBuscar);
            Flat(btnAbrir);
            Flat(btnCerrar);
            if (btnNuevoDoc != null) Flat(btnNuevoDoc);

            grid.EnableHeadersVisualStyles = false;
            grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(250, 250, 250);

            grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(220, 235, 252);
            grid.DefaultCellStyle.SelectionForeColor = Color.Black;
        }

        private void InitCombos()
        {
            cboTipo.DisplayMember = "Text";
            cboTipo.ValueMember = "Value";
            cboTipo.DataSource = new[]
            {
                new { Text = "Todos", Value = "" },
                new { Text = "Cotización (COT)", Value = FacturaRepository.TIPO_COT },
                new { Text = "Pro-Forma (PF)", Value = FacturaRepository.TIPO_PF },
                new { Text = "Factura (FAC)", Value = FacturaRepository.TIPO_FAC },

            }.ToList();

            cboTipo.SelectedIndex = 0;

            chkSoloAbiertas.Checked = true;
            if (chkSoloRegistradas != null)
                chkSoloRegistradas.Checked = false;

            var hoy = DateTime.Today;
            dtDesde.Value = new DateTime(hoy.Year, hoy.Month, 1);
            dtHasta.Value = hoy;
        }

        private void WireEvents()
        {
            btnBuscar.Click += (_, __) => Buscar();

            txtBuscar.KeyDown += (_, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    Buscar();
                }
            };

            chkSoloAbiertas.CheckedChanged += (_, __) =>
            {
                if (chkSoloAbiertas.Checked && chkSoloRegistradas != null)
                    chkSoloRegistradas.Checked = false;
            };

            if (chkSoloRegistradas != null)
            {
                chkSoloRegistradas.CheckedChanged += (_, __) =>
                {
                    if (chkSoloRegistradas.Checked)
                        chkSoloAbiertas.Checked = false;
                };
            }

            // ✅ Mensaje cuando selecciona FINALIZADA
            grid.SelectionChanged += Grid_SelectionChanged;

            var btnAnular = new Button
            {
                Text = "Anular",
                Width = 90,
                Height = btnAbrir.Height,
                Left = btnAbrir.Left + btnAbrir.Width + 10,
                Top = btnAbrir.Top
            };
            btnAnular.FlatStyle = FlatStyle.Flat;
            btnAnular.FlatAppearance.BorderSize = 0;
            btnAnular.Cursor = Cursors.Hand;
            Controls.Add(btnAnular);

            btnAnular.Click += (_, __) => AnularSeleccion();

            grid.CellDoubleClick += (_, e) =>
            {
                if (e.RowIndex >= 0) AbrirSeleccion();
            };

            btnAbrir.Click += (_, __) => AbrirSeleccion();

            if (btnNuevoDoc != null)
                btnNuevoDoc.Click += (_, __) => NuevoDoc();

            btnCerrar.Click += (_, __) => Close();
        }


        private void CargarDefault() => Buscar();

        private void Buscar()
        {
            try
            {
                var desde = chkRangoFechas.Checked ? dtDesde.Value.Date : (DateTime?)null;
                var hasta = chkRangoFechas.Checked ? dtHasta.Value.Date : (DateTime?)null;

                var tipo = Convert.ToString(cboTipo.SelectedValue);
                if (string.IsNullOrWhiteSpace(tipo)) tipo = null;

                var txt = (txtBuscar.Text ?? "").Trim();

                // ✅ Estado: null = todos, BORRADOR = abiertas, FINALIZADA = registradas
                string? estado = null;

                if (chkSoloAbiertas.Checked)
                    estado = FacturaRepository.EST_BORRADOR;
                else if (chkSoloRegistradas != null && chkSoloRegistradas.Checked)
                    estado = FacturaRepository.EST_FINALIZADA;

                var data = _facRepo.ListarHistorial(desde, hasta, tipo, estado, txt);

                BindGrid(data);
                lblCount.Text = $"{data.Count} documento(s)";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Historial", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Grid_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                if (grid.CurrentRow == null) return;

                var estadoObj = grid.CurrentRow.Cells["colEstado"].Value;
                var facturaIdObj = grid.CurrentRow.Cells["colFacturaId"].Value;
                var numeroObj = grid.CurrentRow.Cells["colNumero"].Value;

                if (estadoObj == null || facturaIdObj == null) return;

                var estado = (estadoObj.ToString() ?? "").Trim().ToUpperInvariant();
                if (!int.TryParse(facturaIdObj.ToString(), out var facturaId)) return;

                if (estado == "FINALIZADA" && facturaId > 0 && _ultimaFacturaAvisada != facturaId)
                {
                    _ultimaFacturaAvisada = facturaId;

                    var numero = (numeroObj?.ToString() ?? "").Trim();
                    MessageBox.Show(
                        string.IsNullOrWhiteSpace(numero)
                            ? "Esta es una factura registrada (FINALIZADA)."
                            : $"Esta es una factura registrada (FINALIZADA).\nNúmero: {numero}",
                        "Factura registrada",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }
            }
            catch
            {
                // No romper UX por un aviso.
            }
        }

        private void AnularSeleccion()
        {
            try
            {
                if (grid.CurrentRow == null) return;
                var facturaIdObj = grid.CurrentRow.Cells["colFacturaId"].Value;
                if (facturaIdObj == null) return;

                var facturaId = Convert.ToInt32(facturaIdObj);
                if (facturaId <= 0) return;

                var motivo = Microsoft.VisualBasic.Interaction.InputBox(
                    "Motivo de anulación:", "Anular factura", "Error de facturación");
                if (string.IsNullOrWhiteSpace(motivo)) return;

                // Antes (para auditoría)
                var cabAntes = _facRepo.ObtenerCab(facturaId); // si no tienes este método, me lo dices y lo agrego.
                var antesJson = cabAntes != null ? System.Text.Json.JsonSerializer.Serialize(cabAntes) : null;

                _facRepo.AnularFactura(facturaId, Environment.UserName, motivo);

                // Después
                var cabDespues = _facRepo.ObtenerCab(facturaId);
                var despuesJson = cabDespues != null ? System.Text.Json.JsonSerializer.Serialize(cabDespues) : null;

                new Andloe.Logica.AuditoriaService().Log(
                    modulo: "FACTURACION",
                    accion: "ANULAR",
                    entidad: "FacturaCab",
                    entidadId: facturaId.ToString(),
                    detalle: $"Factura anulada. Motivo: {motivo}",
                    antesJson: antesJson,
                    despuesJson: despuesJson
                );

                MessageBox.Show("Factura anulada y reverso aplicado.", "OK", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Buscar();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Anular", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ✅ CORREGIDO: ya no es FacturaRepository.FacturaHistorialDto
        private void BindGrid(List<FacturaHistorialDto> data)
        {
            grid.Rows.Clear();

            foreach (var x in data)
            {
                var idx = grid.Rows.Add();
                var r = grid.Rows[idx];

                r.Cells["colFacturaId"].Value = x.FacturaId;
                r.Cells["colTipo"].Value = x.TipoDocumento;
                r.Cells["colNumero"].Value = string.IsNullOrWhiteSpace(x.NumeroDocumento) ? "(sin asignar)" : x.NumeroDocumento;
                r.Cells["colFecha"].Value = x.FechaDocumento;
                r.Cells["colEstado"].Value = x.Estado;
                r.Cells["colPago"].Value = x.TipoPago;
                r.Cells["colCliente"].Value = x.NombreCliente;
                r.Cells["colDoc"].Value = x.DocumentoCliente ?? "";
                r.Cells["colTotal"].Value = x.TotalGeneral;
            }
        }

        private void NuevoDoc()
        {
            try
            {
                using var sel = new FormSelectorDocumento();
                var dr = sel.ShowDialog(this);

                if (dr != DialogResult.OK)
                    return;

                var tipo = sel.TipoSeleccionado;
                if (string.IsNullOrWhiteSpace(tipo))
                    tipo = FacturaRepository.TIPO_COT;

                var fv = new FormFacturaV();

                fv.SetAutoNuevoDocumento(tipo);

                AbrirComoHijoEnPrincipal(fv);
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Nuevo Doc", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AbrirSeleccion()
        {
            try
            {
                if (grid.CurrentRow == null) return;

                var facturaIdObj = grid.CurrentRow.Cells["colFacturaId"].Value;
                if (facturaIdObj == null) return;

                var facturaId = Convert.ToInt32(facturaIdObj);
                if (facturaId <= 0) return;

                var fv = new FormFacturaV(facturaId);
                AbrirComoHijoEnPrincipal(fv);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Abrir", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AbrirComoHijoEnPrincipal(Form child)
        {
            Form? principal = null;

            try { principal = FindForm(); } catch { }

            if (principal == null || principal.GetType().Name != "FormPrincipal")
            {
                principal = Application.OpenForms
                    .Cast<Form>()
                    .FirstOrDefault(f => f.GetType().Name == "FormPrincipal");
            }

            if (principal == null)
            {
                child.Show();
                return;
            }

            var mi = principal.GetType().GetMethod("OpenChild", BindingFlags.Instance | BindingFlags.NonPublic);
            if (mi != null)
            {
                mi.Invoke(principal, new object[] { child });
                return;
            }

            child.Show();
        }
    }
}
