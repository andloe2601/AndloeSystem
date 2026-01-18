using System;
using System.Windows.Forms;
using Andloe.Data;
using Andloe.Entidad;

namespace Andloe.Presentacion
{
    public partial class FormClienteEdit : Form
    {
        private readonly ClienteRepository _repo = new();
        private readonly string? _codigo;

        public FormClienteEdit(string? codigo)
        {
            _codigo = codigo;
            InitializeComponent();

            AcceptButton = btnGuardar;
            CancelButton = btnCancelar;

            btnGuardar.Click += (_, __) => Guardar();
            btnCancelar.Click += (_, __) => { DialogResult = DialogResult.Cancel; Close(); };

            Load += (_, __) =>
            {
                if (_codigo == null) PrepNuevo();
                else Cargar();
            };
        }

        private void PrepNuevo()
        {
            Text = "Nuevo Cliente";
            txtCodigo.Text = "(auto)";
            cboTipo.SelectedIndex = 0;
            chkActivo.Checked = true;
            numCreditoMaximo.DecimalPlaces = 2;
            numCreditoMaximo.Maximum = 999999999;
            numCreditoMaximo.ThousandsSeparator = true;
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
            cboTipo.SelectedIndex = c.Tipo;
            chkActivo.Checked = c.Estado == 1;

            numCreditoMaximo.Value = c.CreditoMaximo ?? 0;
            txtDivisa.Text = c.CodDivisas ?? "";
            txtTermino.Text = c.CodTerminoPagos ?? "";
            txtVendedor.Text = c.CodVendedor ?? "";
            txtAlmacen.Text = c.CodAlmacen ?? "";
        }

        private void Guardar()
        {
            try
            {
                var nombre = txtNombre.Text.Trim();
                if (string.IsNullOrWhiteSpace(nombre))
                {
                    MessageBox.Show("Nombre requerido.", "Cliente", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtNombre.Focus();
                    return;
                }

                var rnc = string.IsNullOrWhiteSpace(txtRnc.Text) ? null : txtRnc.Text.Trim();
                var tel = string.IsNullOrWhiteSpace(txtTel.Text) ? null : txtTel.Text.Trim();
                var email = string.IsNullOrWhiteSpace(txtEmail.Text) ? null : txtEmail.Text.Trim();
                var dir = string.IsNullOrWhiteSpace(txtDir.Text) ? null : txtDir.Text.Trim();
                var tipo = (byte)cboTipo.SelectedIndex;
                var estado = chkActivo.Checked ? (byte)1 : (byte)0;

                decimal? credito = numCreditoMaximo.Value == 0 ? null : numCreditoMaximo.Value;
                string? div = string.IsNullOrWhiteSpace(txtDivisa.Text) ? null : txtDivisa.Text.Trim();
                string? ter = string.IsNullOrWhiteSpace(txtTermino.Text) ? null : txtTermino.Text.Trim();
                string? ven = string.IsNullOrWhiteSpace(txtVendedor.Text) ? null : txtVendedor.Text.Trim();
                string? alm = string.IsNullOrWhiteSpace(txtAlmacen.Text) ? null : txtAlmacen.Text.Trim();

                if (_codigo == null)
                {
                    var nuevo = _repo.CrearAuto(nombre, rnc, tel, email, dir, credito, div, ter, ven, alm);
                    MessageBox.Show($"Creado: {nuevo}", "Cliente", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    DialogResult = DialogResult.OK;
                    Close();
                }
                else
                {
                    var c = new Cliente
                    {
                        Codigo = _codigo,
                        Nombre = nombre,
                        RncCedula = rnc,
                        Telefono = tel,
                        Email = email,
                        Direccion = dir,
                        Tipo = tipo,
                        Estado = estado,
                        CreditoMaximo = credito,
                        CodDivisas = div,
                        CodTerminoPagos = ter,
                        CodVendedor = ven,
                        CodAlmacen = alm
                    };
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
    }
}
