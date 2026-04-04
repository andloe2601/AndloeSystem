using System;
using System.Windows.Forms;
using Andloe.Data;
using Andloe.Entidad;

namespace Andloe.Presentacion
{
    public partial class FormVendedorEdit : Form
    {
        private readonly VendedorRepository _repo = new();
        private readonly string? _codigo;

        public FormVendedorEdit(string? codigo)
        {
            _codigo = codigo;
            InitializeComponent();

            AcceptButton = btnGuardar;
            CancelButton = btnCancelar;

            btnGuardar.Click += (_, __) => Guardar();
            btnCancelar.Click += (_, __) => { DialogResult = DialogResult.Cancel; Close(); };
            btnBorrar.Click += (_, __) => Borrar();

            Load += (_, __) =>
            {
                if (_codigo == null) PrepNuevo();
                else Cargar();
            };
        }

        private void PrepNuevo()
        {
            Text = "Nuevo Vendedor";
            chkActivo.Checked = true;
            btnBorrar.Visible = false;
            txtCodigo.ReadOnly = false;
            txtCodigo.Focus();
        }

        private void Cargar()
        {
            var v = _repo.ObtenerPorCodigo(_codigo!);
            if (v == null) throw new InvalidOperationException("Vendedor no encontrado.");

            Text = $"Editar {v.Codigo}";
            txtCodigo.Text = v.Codigo;
            txtNombre.Text = v.Nombre;
            txtEmail.Text = v.Email ?? "";
            txtTel.Text = v.Telefono ?? "";
            chkActivo.Checked = v.Estado;

            // Regla recomendada: no cambiar Código en edición (referenciado por Cliente/Proveedor)
            txtCodigo.ReadOnly = true;
            btnBorrar.Visible = true;
        }

        private void Guardar()
        {
            try
            {
                var codigo = txtCodigo.Text.Trim();
                var nombre = txtNombre.Text.Trim();

                if (string.IsNullOrWhiteSpace(codigo))
                {
                    MessageBox.Show("Código requerido.", "Vendedor", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtCodigo.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(nombre))
                {
                    MessageBox.Show("Nombre requerido.", "Vendedor", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtNombre.Focus();
                    return;
                }

                var email = string.IsNullOrWhiteSpace(txtEmail.Text) ? null : txtEmail.Text.Trim();
                var tel = string.IsNullOrWhiteSpace(txtTel.Text) ? null : txtTel.Text.Trim();
                var estado = chkActivo.Checked;

                var v = new Vendedor
                {
                    Codigo = codigo,
                    Nombre = nombre,
                    Email = email,
                    Telefono = tel,
                    Estado = estado
                };

                if (_codigo == null)
                {
                    _repo.Crear(v);
                }
                else
                {
                    // Se mantiene el código original (readonly)
                    v.Codigo = _codigo;
                    _repo.Actualizar(v);
                }

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Guardar", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Borrar()
        {
            if (_codigo == null) return;

            if (MessageBox.Show($"¿Inactivar vendedor {_codigo}?",
                    "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            try
            {
                _repo.Eliminar(_codigo);
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Borrar", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
