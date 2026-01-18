using Andloe.Entidad;
using Andloe.Logica;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;

namespace Andloe.Presentacion
{
    public partial class FormTerminoPago : Form
    {
        private readonly TerminoPagoService _svc = new();
        private List<TerminoPago> _data = new();

        public FormTerminoPago()
        {
            InitializeComponent();
            ConfigGrid();
        }

        private void FormTerminoPago_Load(object sender, EventArgs e)
        {
            Cargar();
        }

        private void ConfigGrid()
        {
            dgv.AutoGenerateColumns = false;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.MultiSelect = false;
            dgv.ReadOnly = true;

            dgv.Columns.Clear();
            dgv.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "TerminoPagoId", HeaderText = "ID", Width = 60 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Codigo", HeaderText = "Código", Width = 90 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Descripcion", HeaderText = "Descripción", Width = 220 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "DiasPlazo", HeaderText = "Días", Width = 70 });
            dgv.Columns.Add(new DataGridViewCheckBoxColumn { DataPropertyName = "TieneDescuento", HeaderText = "Desc.", Width = 60 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "PorcDescuento", HeaderText = "%Desc", Width = 70 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "DiasDescuento", HeaderText = "DíasDesc", Width = 80 });
            dgv.Columns.Add(new DataGridViewCheckBoxColumn { DataPropertyName = "Estado", HeaderText = "Activo", Width = 60 });
        }

        private void Cargar()
        {
            _data = chkSoloActivos.Checked ? _svc.ListarActivos() : _svc.ListarTodos();
            AplicarFiltro();
        }

        private void AplicarFiltro()
        {
            var f = (txtBuscar.Text ?? "").Trim().ToUpperInvariant();
            var view = string.IsNullOrWhiteSpace(f)
                ? _data
                : _data.Where(x =>
                       (x.Codigo ?? "").ToUpperInvariant().Contains(f) ||
                       (x.Descripcion ?? "").ToUpperInvariant().Contains(f))
                   .ToList();

            dgv.DataSource = null;
            dgv.DataSource = view;
        }

        private TerminoPago? GetSelected()
        {
            if (dgv.CurrentRow == null) return null;
            return dgv.CurrentRow.DataBoundItem as TerminoPago;
        }

        private void btnRefrescar_Click(object sender, EventArgs e) => Cargar();

        private void txtBuscar_TextChanged(object sender, EventArgs e) => AplicarFiltro();

        private void chkSoloActivos_CheckedChanged(object sender, EventArgs e) => Cargar();

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            LimpiarEditor();
            tabControl1.SelectedTab = tabEditor;
            txtCodigo.Focus();
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            var sel = GetSelected();
            if (sel == null) return;

            CargarEditor(sel);
            tabControl1.SelectedTab = tabEditor;
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                var id = int.TryParse(txtId.Text, out var v) ? v : 0;

                var t = new TerminoPago
                {
                    TerminoPagoId = id,
                    Codigo = (txtCodigo.Text ?? "").Trim(),
                    Descripcion = (txtDescripcion.Text ?? "").Trim(),
                    DiasPlazo = (int)numDias.Value,
                    TieneDescuento = chkDescuento.Checked,
                    PorcDescuento = chkDescuento.Checked ? (decimal?)numPorcDesc.Value : null,
                    DiasDescuento = chkDescuento.Checked ? (int?)numDiasDesc.Value : null,
                    Estado = chkActivo.Checked,
                    Usuario = (txtUsuario.Text ?? "").Trim()
                };

                if (id <= 0)
                {
                    var newId = _svc.Crear(t);
                    MessageBox.Show($"Creado ID {newId}", "OK", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    _svc.Actualizar(t);
                    MessageBox.Show("Actualizado", "OK", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                tabControl1.SelectedTab = tabLista;
                Cargar();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabLista;
        }

        private void btnToggle_Click(object sender, EventArgs e)
        {
            var sel = GetSelected();
            if (sel == null) return;

            try
            {
                var nuevo = !sel.Estado;
                _svc.CambiarEstado(sel.TerminoPagoId, nuevo);
                Cargar();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void chkDescuento_CheckedChanged(object sender, EventArgs e)
        {
            var on = chkDescuento.Checked;
            numPorcDesc.Enabled = on;
            numDiasDesc.Enabled = on;
        }

        private void LimpiarEditor()
        {
            txtId.Text = "0";
            txtCodigo.Text = "";
            txtDescripcion.Text = "";
            numDias.Value = 0;
            chkDescuento.Checked = false;
            numPorcDesc.Value = 0;
            numDiasDesc.Value = 0;
            chkActivo.Checked = true;
            txtUsuario.Text = "";
        }

        private void CargarEditor(TerminoPago t)
        {
            txtId.Text = t.TerminoPagoId.ToString();
            txtCodigo.Text = t.Codigo;
            txtDescripcion.Text = t.Descripcion;
            numDias.Value = t.DiasPlazo;
            chkDescuento.Checked = t.TieneDescuento;
            numPorcDesc.Value = t.PorcDescuento ?? 0m;
            numDiasDesc.Value = t.DiasDescuento ?? 0;
            chkActivo.Checked = t.Estado;
            txtUsuario.Text = t.Usuario ?? "";
        }
    }
}
