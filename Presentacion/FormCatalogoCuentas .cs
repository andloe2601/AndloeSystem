using Andloe.Data;
using Andloe.Logica;
using System;
using System.Linq;
using System.Windows.Forms;

namespace Andloe.Presentacion
{
    public partial class FormCatalogoCuentas : Form
    {
        private readonly ContabilidadCatalogoCuentaService _svc = new();
        private CtaRow? _seleccion;
        private int? _padreId;

        public FormCatalogoCuentas()
        {
            InitializeComponent();
            ConfigurarGrid();
        }

        private void FormCatalogoCuentas_Load(object sender, EventArgs e)
        {
            cboTipo.Items.Clear();
            cboTipo.Items.AddRange(new object[] { "ACTIVO", "PASIVO", "CAPITAL", "INGRESO", "GASTO" });
            cboTipo.SelectedIndex = 0;

            CargarTodo();
        }

        private void ConfigurarGrid()
        {
            gridCuentas.AutoGenerateColumns = true;
            gridCuentas.ReadOnly = true;
            gridCuentas.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }

        private void CargarTodo()
        {
            var data = _svc.Listar(txtBuscar.Text.Trim());
            gridCuentas.DataSource = data;

            CargarTree(data);
            LimpiarForm();
        }

        private void CargarTree(System.Collections.Generic.List<CtaRow> data)
        {
            treeCuentas.Nodes.Clear();

            var raiz = data.Where(x => x.PadreId == null).OrderBy(x => x.Codigo).ToList();
            foreach (var r in raiz)
            {
                var node = new TreeNode($"{r.Codigo} - {r.Descripcion}") { Tag = r };
                treeCuentas.Nodes.Add(node);
                CargarHijos(node, data);
            }

            treeCuentas.ExpandAll();
        }

        private void CargarHijos(TreeNode parent, System.Collections.Generic.List<CtaRow> data)
        {
            var p = (CtaRow)parent.Tag;
            var hijos = data.Where(x => x.PadreId == p.CuentaId).OrderBy(x => x.Codigo).ToList();

            foreach (var h in hijos)
            {
                var node = new TreeNode($"{h.Codigo} - {h.Descripcion}") { Tag = h };
                parent.Nodes.Add(node);
                CargarHijos(node, data);
            }
        }

        private void LimpiarForm()
        {
            _seleccion = null;
            txtCodigo.Text = "";
            txtDescripcion.Text = "";
            cboTipo.SelectedIndex = 0;
            chkActivo.Checked = true;
            _padreId = null;
            lblPadre.Text = "Padre: (Raíz)";
        }

        private void btnNuevo_Click(object sender, EventArgs e) => LimpiarForm();

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                var c = _seleccion ?? new CtaRow();

                c.Codigo = txtCodigo.Text.Trim();
                c.Descripcion = txtDescripcion.Text.Trim();
                c.Tipo = cboTipo.SelectedItem?.ToString() ?? "ACTIVO";
                c.Estado = chkActivo.Checked ? "ACTIVO" : "INACTIVO";
                c.PadreId = _padreId;

                var id = _svc.Guardar(c);
                MessageBox.Show($"Guardado OK. CuentaId={id}", "Contabilidad",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                CargarTodo();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Contabilidad",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            try
            {
                if (_seleccion == null)
                {
                    MessageBox.Show("Seleccione una cuenta.", "Contabilidad",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                _svc.Eliminar(_seleccion.CuentaId);
                MessageBox.Show("Eliminado OK.", "Contabilidad",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                CargarTodo();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Contabilidad",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnEstado_Click(object sender, EventArgs e)
        {
            try
            {
                if (_seleccion == null)
                {
                    MessageBox.Show("Seleccione una cuenta.", "Contabilidad",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var nuevo = !_seleccion.Estado.Equals("ACTIVO", StringComparison.OrdinalIgnoreCase);
                _svc.SetEstado(_seleccion.CuentaId, nuevo);

                MessageBox.Show("Estado actualizado.", "Contabilidad",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                CargarTodo();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Contabilidad",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtBuscar_TextChanged(object sender, EventArgs e)
        {
            var data = _svc.Listar(txtBuscar.Text.Trim());
            gridCuentas.DataSource = data;
            CargarTree(data);
        }

        private void gridCuentas_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var row = gridCuentas.Rows[e.RowIndex].DataBoundItem as CtaRow;
            if (row == null) return;

            _seleccion = row;

            txtCodigo.Text = row.Codigo;
            txtDescripcion.Text = row.Descripcion;
            cboTipo.SelectedItem = row.Tipo;
            chkActivo.Checked = row.Estado.Equals("ACTIVO", StringComparison.OrdinalIgnoreCase);

            _padreId = row.PadreId;
            lblPadre.Text = _padreId.HasValue ? $"PadreId: {_padreId}" : "Padre: (Raíz)";
        }

        private void treeCuentas_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node?.Tag is not CtaRow sel) return;

            // Seleccionar cuenta del tree como "Padre"
            _padreId = sel.CuentaId;
            lblPadre.Text = $"Padre: {sel.Codigo} - {sel.Descripcion}";
        }
    }
}
