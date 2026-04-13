using Andloe.Data;
using Andloe.Entidad;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Andloe.Presentacion
{
    public partial class FormBuscarCliente : Form
    {
        private readonly ClienteRepository _repo = new();
        private List<Cliente> _data = new();
        private string? _filtroInicial;

        public ClienteDto? ClienteSeleccionado { get; private set; }

        public FormBuscarCliente()
        {
            InitializeComponent();
            WireEvents();
        }

        public void SetFiltroInicial(string? filtro)
        {
            _filtroInicial = (filtro ?? "").Trim();
        }

        private void WireEvents()
        {
            Shown += (_, __) =>
            {
                if (!string.IsNullOrWhiteSpace(_filtroInicial))
                    txtBuscar.Text = _filtroInicial;

                txtBuscar.Focus();
                txtBuscar.SelectionStart = txtBuscar.TextLength;
                Cargar();
            };

            btnBuscar.Click += (_, __) => Cargar();

            txtBuscar.KeyDown += (_, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    Cargar();
                }
            };

            grid.DoubleClick += (_, __) => SeleccionarActual();

            grid.KeyDown += (_, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    SeleccionarActual();
                }
            };

            btnAceptar.Click += (_, __) => SeleccionarActual();

            btnCancelar.Click += (_, __) =>
            {
                DialogResult = DialogResult.Cancel;
                Close();
            };
        }

        private void Cargar()
        {
            try
            {
                var filtro = (txtBuscar.Text ?? "").Trim();
                _data = _repo.Listar(filtro, 300);

                grid.Rows.Clear();

                foreach (var c in _data)
                {
                    grid.Rows.Add(
                        c.Codigo ?? "",
                        c.Nombre ?? "",
                        c.RncCedula ?? "",
                        c.Telefono ?? "",
                        c.Estado == 1 ? "Activo" : "Inactivo"
                    );
                }

                lblTotal.Text = $"Total: {_data.Count}";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Buscar cliente", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SeleccionarActual()
        {
            try
            {
                if (grid.CurrentRow == null) return;

                var codigo = Convert.ToString(grid.CurrentRow.Cells["colCodigo"].Value)?.Trim();
                if (string.IsNullOrWhiteSpace(codigo)) return;

                var dto = _repo.BuscarPorCodigoORnc(codigo);
                if (dto == null || dto.ClienteId <= 0)
                {
                    MessageBox.Show(
                        "No se pudo cargar el cliente seleccionado.",
                        "Buscar cliente",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                ClienteSeleccionado = dto;
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Seleccionar cliente", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}