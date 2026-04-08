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


namespace Andloe.Presentacion
{
    public partial class FormClienteEdit : Form
    {
        private readonly ClienteRepository _repo = new();
        private readonly DgiiRncRepository _dgiiRepo = new();
        private readonly string? _codigo;
        private readonly TerminoPagoRepository _terminoRepo = new();
        private List<TerminoPago> _terminosPago = new();




        public FormClienteEdit(string? codigo)
        {
            _codigo = codigo;
            InitializeComponent();

            AcceptButton = btnGuardar;
            CancelButton = btnCancelar;

            btnGuardar.Click += (_, __) => Guardar();
            btnCancelar.Click += (_, __) => { DialogResult = DialogResult.Cancel; Close(); };
            btnValidarDgii.Click += (_, __) => ValidarConDgii();
            chkEsExtranjero.CheckedChanged += (_, __) => ActualizarEstadoFiscalUi();
            chkValidadoDgii.CheckedChanged += (_, __) => ActualizarEstadoFiscalUi();
            cboTipoIdentificacion.SelectedIndexChanged += (_, __) => ActualizarEstadoFiscalUi();
            txtRnc.TextChanged += (_, __) =>
            {
                txtRnc.Text = SoloDigitos(txtRnc.Text);
                txtRnc.SelectionStart = txtRnc.Text.Length;
                ActualizarEstadoFiscalUi();
            };

            cboTerminoPago.SelectedIndexChanged += (_, __) => SincronizarTerminoSeleccionado();

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

            CargarTerminosPago();

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
            txtPaisCodigo.Text = "DO";
            txtEstadoRncDgii.Text = "PENDIENTE";
            dtpFechaValidacion.Checked = false;
            SeleccionarTerminoPago("NET30");
            ActualizarEstadoFiscalUi();
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
            txtDivisa.Text = c.CodDivisas ?? "";
            SeleccionarTerminoPago(c.CodTerminoPagos);
            txtVendedor.Text = c.CodVendedor ?? "";
            txtAlmacen.Text = c.CodAlmacen ?? "";
            numDescuentoMaximo.Value = c.DescuentoPctMax ?? 0;

            txtRazonFiscal.Text = c.RazonSocialFiscal ?? c.Nombre;
            txtNombreComercialFiscal.Text = c.NombreComercialFiscal ?? c.Nombre;
            SeleccionarComboPorValor<byte>(cboTipoIdentificacion, c.TipoIdentificacionFiscal ?? InferirTipoDocumento(c.RncCedula));
            txtProvinciaCodigo.Text = c.ProvinciaCodigo ?? "";
            txtMunicipioCodigo.Text = c.MunicipioCodigo ?? "";
            txtPaisCodigo.Text = c.PaisCodigo ?? "DO";
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
                    MessageBox.Show(errores.ToString(), "Validación cliente", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

                    MessageBox.Show($"Creado: {nuevo}", "Cliente", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    DialogResult = DialogResult.OK;
                    Close();
                }
                else
                {
                    c.Codigo = _codigo;
                    _repo.Actualizar(c);
                    DialogResult = DialogResult.OK;
                    Close();
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
                CodDivisas = NullIfWhite(txtDivisa.Text),
                CodTerminoPagos = cboTerminoPago.SelectedValue?.ToString() ?? NullIfWhite(txtTermino.Text),
                CodVendedor = NullIfWhite(txtVendedor.Text),
                CodAlmacen = NullIfWhite(txtAlmacen.Text),
                DescuentoPctMax = numDescuentoMaximo.Value == 0 ? null : numDescuentoMaximo.Value,

                RazonSocialFiscal = NullIfWhite(txtRazonFiscal.Text) ?? nombre,
                NombreComercialFiscal = NullIfWhite(txtNombreComercialFiscal.Text),
                TipoIdentificacionFiscal = tipoDoc,
                ProvinciaCodigo = NullIfWhite(txtProvinciaCodigo.Text),
                MunicipioCodigo = NullIfWhite(txtMunicipioCodigo.Text),
                PaisCodigo = NullIfWhite(txtPaisCodigo.Text) ?? "DO",
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

            if (!c.EsExtranjero)
            {
                if (string.IsNullOrWhiteSpace(c.RncCedula))
                    sb.AppendLine("- Para cliente local debes registrar RNC/Cédula.");
                else if (c.RncCedula!.Length is not (9 or 11))
                    sb.AppendLine("- El RNC/Cédula debe tener 9 u 11 dígitos.");

                if (string.IsNullOrWhiteSpace(c.RazonSocialFiscal))
                    sb.AppendLine("- La razón social fiscal es obligatoria.");

                if (c.TipoIdentificacionFiscal is null or 0)
                    sb.AppendLine("- Debes indicar el tipo de identificación fiscal.");

                if (c.TipoIdentificacionFiscal == 3)
                    sb.AppendLine("- Para cliente local no uses tipo de identificación extranjero.");
            }
            else
            {
                if (string.IsNullOrWhiteSpace(c.IdentificadorExtranjero))
                    sb.AppendLine("- Si el cliente es extranjero debes registrar identificador extranjero.");
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
                    MessageBox.Show("La validación local DGII aplica para clientes locales con RNC.", "DGII", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var rnc = SoloDigitos(txtRnc.Text);
                if (string.IsNullOrWhiteSpace(rnc) || rnc.Length != 9)
                {
                    MessageBox.Show("Para consultar DGII local debes indicar un RNC de 9 dígitos.", "DGII", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtRnc.Focus();
                    return;
                }

                var dto = _dgiiRepo.BuscarActivoPorRnc(rnc) ?? _dgiiRepo.BuscarPorRnc(rnc)?.ToDto();
                if (dto == null)
                {
                    chkValidadoDgii.Checked = false;
                    txtEstadoRncDgii.Text = "NO ENCONTRADO";
                    dtpFechaValidacion.Checked = false;
                    ActualizarEstadoFiscalUi();
                    MessageBox.Show("No encontré ese RNC en la tabla local DGII activa.", "DGII", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                txtRnc.Text = dto.Rnc;
                if (string.IsNullOrWhiteSpace(txtRazonFiscal.Text)) txtRazonFiscal.Text = dto.Nombre;
                if (string.IsNullOrWhiteSpace(txtNombre.Text)) txtNombre.Text = dto.Nombre;
                if (string.IsNullOrWhiteSpace(txtNombreComercialFiscal.Text)) txtNombreComercialFiscal.Text = dto.NombreComercial ?? dto.Nombre;
                chkValidadoDgii.Checked = true;
                txtEstadoRncDgii.Text = dto.Estado ?? "ACTIVO";
                dtpFechaValidacion.Checked = true;
                dtpFechaValidacion.Value = DateTime.Now;
                chkEsContribuyente.Checked = true;
                if (cboTipoIdentificacion.SelectedIndex < 0)
                    SeleccionarComboPorValor<byte>(cboTipoIdentificacion, (byte)1);
                ActualizarEstadoFiscalUi();

                MessageBox.Show("Cliente validado con la tabla local DGII.", "DGII", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "DGII", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ActualizarEstadoFiscalUi()
        {
            var cliente = ConstruirDesdePantalla();

            txtIdentificadorExtranjero.Enabled = cliente.EsExtranjero;
            btnValidarDgii.Enabled = !cliente.EsExtranjero;
            txtEstadoFiscal.Text = cliente.AptoParaE31
                ? "APTO PARA E31"
                : cliente.EsExtranjero
                    ? "CLIENTE EXTRANJERO / REQUIERE REVISIÓN FISCAL"
                    : "PENDIENTE O NO APTO PARA E31";
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
