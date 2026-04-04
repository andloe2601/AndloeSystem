using Andloe.Entidad;
using Andloe.Logica;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace Andloe.Presentacion
{
    public partial class FormNotaCreditoVentaECF : Form
    {
        private readonly NotaCreditoVentaService _svc = new();
        private readonly BindingList<VentaOrigenDto> _ventas = new();
        private readonly BindingList<NotaCreditoVentaLinDto> _lineas = new();

        private readonly int _usuarioId;
        private readonly string _usuario;
        private readonly int? _cajaId;

        private long _ventaIdSeleccionada;
        private long _ncId;

        public FormNotaCreditoVentaECF(int usuarioId, string usuario, int? cajaId = null)
        {
            _usuarioId = usuarioId;
            _usuario = (usuario ?? "").Trim();
            _cajaId = cajaId;

            InitializeComponent();
            BuildUi();
            Wire();
            ConfigGrids();
            CargarTiposENcf();
            CargarPreviewNoNC();
        }

        private void BuildUi()
        {
            Text = "Nota de crédito venta / e-NCF";
            Font = new Font("Segoe UI", 10f);
            StartPosition = FormStartPosition.CenterScreen;
            KeyPreview = true;

            txtFecha.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
            txtUsuario.Text = _usuario;
            txtNCId.Text = "0";
            txtEstado.Text = "NUEVA";
            txtMoneda.Text = "DOP";
            txtTasaCambio.Text = "1.000000";

            RefrescarTotales();
        }

        private void Wire()
        {
            Load += (_, __) => BuscarVentas();

            btnBuscarVentas.Click += (_, __) => BuscarVentas();
            btnBuscarPorId.Click += (_, __) => BuscarPorVentaId();
            btnTomarVenta.Click += (_, __) => TomarVentaSeleccionada();
            btnEmitirNc.Click += (_, __) => EmitirNotaCredito();
            btnGenerarEncf.Click += (_, __) => GenerarENcf();
            btnCerrar.Click += (_, __) => Close();
            btnLimpiar.Click += (_, __) => Limpiar();

            gridVentas.DoubleClick += (_, __) => TomarVentaSeleccionada();
            gridLineas.CellEndEdit += (_, __) => RefrescarTotales();

            gridLineas.CurrentCellDirtyStateChanged += (_, __) =>
            {
                if (gridLineas.IsCurrentCellDirty)
                    gridLineas.CommitEdit(DataGridViewDataErrorContexts.Commit);
            };
        }

        private void ConfigGrids()
        {
            gridVentas.AutoGenerateColumns = false;
            gridVentas.AllowUserToAddRows = false;
            gridVentas.AllowUserToDeleteRows = false;
            gridVentas.MultiSelect = false;
            gridVentas.ReadOnly = true;
            gridVentas.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            gridVentas.DataSource = _ventas;
            gridVentas.Columns.Clear();

            gridVentas.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "VentaId",
                DataPropertyName = nameof(VentaOrigenDto.VentaId),
                Width = 80
            });
            gridVentas.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Documento",
                DataPropertyName = nameof(VentaOrigenDto.NoDocumento),
                Width = 120
            });
            gridVentas.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Fecha",
                DataPropertyName = nameof(VentaOrigenDto.Fecha),
                Width = 110,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy" }
            });
            gridVentas.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Cliente",
                DataPropertyName = nameof(VentaOrigenDto.ClienteNombre),
                Width = 190
            });
            gridVentas.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Total",
                DataPropertyName = nameof(VentaOrigenDto.TotalMoneda),
                Width = 90,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Format = "N2",
                    Alignment = DataGridViewContentAlignment.MiddleRight
                }
            });

            gridLineas.AutoGenerateColumns = false;
            gridLineas.AllowUserToAddRows = false;
            gridLineas.AllowUserToDeleteRows = false;
            gridLineas.MultiSelect = false;
            gridLineas.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            gridLineas.DataSource = _lineas;
            gridLineas.Columns.Clear();

            gridLineas.Columns.Add(new DataGridViewCheckBoxColumn
            {
                HeaderText = "Sel",
                DataPropertyName = nameof(NotaCreditoVentaLinDto.Seleccionado),
                Width = 40
            });
            gridLineas.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Línea",
                DataPropertyName = nameof(NotaCreditoVentaLinDto.Linea),
                Width = 50,
                ReadOnly = true
            });
            gridLineas.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Código",
                DataPropertyName = nameof(NotaCreditoVentaLinDto.ProductoCodigo),
                Width = 90,
                ReadOnly = true
            });
            gridLineas.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Descripción",
                DataPropertyName = nameof(NotaCreditoVentaLinDto.Descripcion),
                Width = 220,
                ReadOnly = true
            });
            gridLineas.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Cant. Fact",
                DataPropertyName = nameof(NotaCreditoVentaLinDto.CantidadOriginal),
                Width = 80,
                ReadOnly = true,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Format = "N2",
                    Alignment = DataGridViewContentAlignment.MiddleRight
                }
            });
            gridLineas.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Cant. NC",
                DataPropertyName = nameof(NotaCreditoVentaLinDto.CantidadNC),
                Width = 80,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Format = "N2",
                    Alignment = DataGridViewContentAlignment.MiddleRight
                }
            });
            gridLineas.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Precio",
                DataPropertyName = nameof(NotaCreditoVentaLinDto.PrecioUnit),
                Width = 80,
                ReadOnly = true,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Format = "N2",
                    Alignment = DataGridViewContentAlignment.MiddleRight
                }
            });
            gridLineas.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Total NC",
                DataPropertyName = nameof(NotaCreditoVentaLinDto.TotalNcCalculado),
                Width = 90,
                ReadOnly = true,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Format = "N2",
                    Alignment = DataGridViewContentAlignment.MiddleRight
                }
            });
        }

        private void CargarPreviewNoNC()
        {
            try
            {
                if (_ncId > 0)
                    return;

                txtNoNC.Text = _svc.ObtenerProximoNoDocumentoNcPreview();
            }
            catch
            {
                txtNoNC.Text = "";
            }
        }

        private void CargarTiposENcf()
        {
            var tipos = _svc.ListarTiposENcf();

            cboTipoENcf.DisplayMember = nameof(NcfTipoDto.Descripcion);
            cboTipoENcf.ValueMember = nameof(NcfTipoDto.TipoId);
            cboTipoENcf.DataSource = tipos;

            var preferido = tipos.FirstOrDefault(x => x.Codigo.Equals("E34", StringComparison.OrdinalIgnoreCase))
                         ?? tipos.FirstOrDefault();

            if (preferido != null)
                cboTipoENcf.SelectedValue = preferido.TipoId;
        }

        private void BuscarVentas()
        {
            try
            {
                _ventas.Clear();

                foreach (var v in _svc.BuscarVentas(txtBuscarVenta.Text, 150))
                    _ventas.Add(v);

                lblStatus.Text = $"{_ventas.Count} venta(s) encontradas";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Nota de crédito",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BuscarPorVentaId()
        {
            try
            {
                if (!long.TryParse(txtFacturaId.Text.Trim(), out var ventaId) || ventaId <= 0)
                {
                    MessageBox.Show("Digite un VentaId válido.", "Nota de crédito",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var dto = _svc.CargarVenta(ventaId, _usuarioId);
                if (dto == null)
                {
                    MessageBox.Show("No se encontró la venta origen.", "Nota de crédito",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                CargarVentaEnPantalla(dto);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Nota de crédito",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void TomarVentaSeleccionada()
        {
            try
            {
                var venta = gridVentas.CurrentRow?.DataBoundItem as VentaOrigenDto;
                if (venta == null)
                    return;

                var dto = _svc.CargarVenta(venta.VentaId, _usuarioId);
                if (dto == null)
                    return;

                CargarVentaEnPantalla(dto);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Nota de crédito",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarVentaEnPantalla(NotaCreditoVentaDto dto)
        {
            _ventaIdSeleccionada = dto.Cab.VentaIdOrigen ?? 0;
            _ncId = 0;

            txtNCId.Text = "0";
            txtEstado.Text = "NUEVA";
            txtENcf.Text = "";

            txtVentaId.Text = _ventaIdSeleccionada.ToString(CultureInfo.InvariantCulture);
            txtFacturaId.Text = txtVentaId.Text;
            txtNoVenta.Text = dto.Cab.NoDocumentoOrigen ?? "";
            txtClienteCodigo.Text = dto.Cab.ClienteCodigo;
            txtClienteNombre.Text = dto.Cab.ClienteNombre;
            txtClienteDoc.Text = dto.Cab.DocumentoCliente ?? "";
            txtMoneda.Text = dto.Cab.MonedaCodigo;
            txtTasaCambio.Text = dto.Cab.TasaCambio.ToString("N6");
            txtENcfOrigen.Text = dto.Cab.ENcfOrigen ?? "";

            _lineas.Clear();
            foreach (var ln in dto.Lineas)
            {
                ln.Seleccionado = true;
                ln.CantidadNC = ln.CantidadOriginal;
                _lineas.Add(ln);
            }

            RefrescarTotales();
            CargarPreviewNoNC();
            lblStatus.Text = $"Venta {_ventaIdSeleccionada} cargada";
        }

        private NotaCreditoVentaDto ConstruirDtoDesdePantalla()
        {
            if (_ventaIdSeleccionada <= 0)
                throw new InvalidOperationException("Debe seleccionar una venta origen.");

            var dto = new NotaCreditoVentaDto
            {
                Cab = new NotaCreditoVentaCabDto
                {
                    NCId = _ncId,
                    Fecha = DateTime.Now,
                    VentaIdOrigen = _ventaIdSeleccionada,
                    ClienteCodigo = txtClienteCodigo.Text.Trim(),
                    ClienteNombre = txtClienteNombre.Text.Trim(),
                    DocumentoCliente = txtClienteDoc.Text.Trim(),
                    MonedaCodigo = txtMoneda.Text.Trim(),
                    TasaCambio = ParseDecimal(txtTasaCambio.Text, 1m),
                    Motivo = txtMotivo.Text.Trim(),
                    ENCF = txtENcf.Text.Trim(),
                    NoDocumentoOrigen = txtNoVenta.Text.Trim(),
                    ENcfOrigen = txtENcfOrigen.Text.Trim()
                }
            };

            foreach (var ln in _lineas)
            {
                dto.Lineas.Add(new NotaCreditoVentaLinDto
                {
                    Seleccionado = ln.Seleccionado,
                    Linea = ln.Linea,
                    ProductoCodigo = ln.ProductoCodigo,
                    Descripcion = ln.Descripcion,
                    CantidadOriginal = ln.CantidadOriginal,
                    CantidadNC = ln.CantidadNC,
                    PrecioUnit = ln.PrecioUnit,
                    ItbisPct = ln.ItbisPct,
                    DescuentoMoneda = ln.DescuentoMoneda,
                    SubTotalMoneda = ln.SubTotalMoneda,
                    ItbisMoneda = ln.ItbisMoneda,
                    TotalMoneda = ln.TotalMoneda
                });
            }

            return dto;
        }

        private void EmitirNotaCredito()
        {
            try
            {
                var dto = ConstruirDtoDesdePantalla();
                var guardada = _svc.EmitirDesdeVenta(dto, _usuario, _usuarioId, _cajaId);

                _ncId = guardada.Cab.NCId;
                txtNCId.Text = guardada.Cab.NCId.ToString(CultureInfo.InvariantCulture);
                txtEstado.Text = guardada.Cab.Estado;
                txtNoNC.Text = guardada.Cab.NoDocumento;
                txtENcf.Text = guardada.Cab.ENCF ?? txtENcf.Text;

                lblStatus.Text = $"NC {_ncId} guardada y mercancía repuesta";

                MessageBox.Show(
                    $"Nota de crédito creada correctamente.\nNo. NC: {guardada.Cab.NoDocumento}",
                    "Nota de crédito",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Nota de crédito",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void GenerarENcf()
        {
            try
            {
                if (_ncId <= 0)
                    throw new InvalidOperationException("Primero guarde la nota de crédito.");

                if (cboTipoENcf.SelectedValue == null)
                    throw new InvalidOperationException("Seleccione el tipo de e-NCF.");

                var tipoId = Convert.ToInt32(cboTipoENcf.SelectedValue, CultureInfo.InvariantCulture);
                var encf = _svc.GenerarENcf(_ncId, tipoId, _usuarioId, _cajaId);

                txtENcf.Text = encf;
                txtEstado.Text = "EMITIDO";
                lblStatus.Text = $"e-NCF generado: {encf}";

                MessageBox.Show(
                    $"e-NCF generado correctamente:\n{encf}",
                    "Nota de crédito",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Nota de crédito",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void RefrescarTotales()
        {
            decimal sub = 0m, itb = 0m, tot = 0m;

            foreach (var ln in _lineas)
            {
                if (ln.CantidadNC < 0)
                    ln.CantidadNC = 0;

                if (ln.CantidadNC > ln.CantidadOriginal)
                    ln.CantidadNC = ln.CantidadOriginal;

                var factor = ln.CantidadOriginal <= 0 ? 0m : (ln.CantidadNC / ln.CantidadOriginal);

                ln.SubTotalNcCalculado = Math.Round(ln.SubTotalMoneda * factor, 2, MidpointRounding.AwayFromZero);
                ln.ItbisNcCalculado = Math.Round(ln.ItbisMoneda * factor, 2, MidpointRounding.AwayFromZero);
                ln.TotalNcCalculado = Math.Round(ln.SubTotalNcCalculado + ln.ItbisNcCalculado, 2, MidpointRounding.AwayFromZero);

                if (!ln.Seleccionado || ln.CantidadNC <= 0)
                    continue;

                sub += ln.SubTotalNcCalculado;
                itb += ln.ItbisNcCalculado;
                tot += ln.TotalNcCalculado;
            }

            txtSubTotal.Text = sub.ToString("N2");
            txtItbis.Text = itb.ToString("N2");
            txtTotal.Text = tot.ToString("N2");

            gridLineas.Refresh();
        }

        private void Limpiar()
        {
            _ncId = 0;
            _ventaIdSeleccionada = 0;

            txtNCId.Text = "0";
            txtNoNC.Text = "";
            txtEstado.Text = "NUEVA";
            txtVentaId.Text = "";
            txtFacturaId.Text = "";
            txtNoVenta.Text = "";
            txtClienteCodigo.Text = "";
            txtClienteNombre.Text = "";
            txtClienteDoc.Text = "";
            txtMoneda.Text = "DOP";
            txtTasaCambio.Text = "1.000000";
            txtENcf.Text = "";
            txtENcfOrigen.Text = "";
            txtMotivo.Text = "";
            txtSubTotal.Text = "0.00";
            txtItbis.Text = "0.00";
            txtTotal.Text = "0.00";

            _lineas.Clear();

            lblStatus.Text = "Formulario limpio";
            CargarPreviewNoNC();
        }

        private static decimal ParseDecimal(string text, decimal fallback)
        {
            if (decimal.TryParse(text, NumberStyles.Any, CultureInfo.CurrentCulture, out var v))
                return v;

            if (decimal.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out v))
                return v;

            return fallback;
        }
    }
}