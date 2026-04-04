using System;
using System.Windows.Forms;
using Andloe.Logica;

namespace Andloe.Presentacion
{
    public partial class FormCxCCobros : Form
    {
        private readonly CxCCobroService _service = new();

        public FormCxCCobros()
        {
            InitializeComponent();
            btnBuscar.Click += (_, __) => Cargar();
            btnNuevo.Click += (_, __) => Nuevo();
            txtBuscar.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) { Cargar(); e.SuppressKeyPress = true; } };
            Shown += (_, __) => Cargar();
        }

        private void Cargar()
        {
            try
            {
                var data = _service.Listar(txtBuscar.Text.Trim(), 300);
                grid.Rows.Clear();
                foreach (var x in data)
                {
                    grid.Rows.Add(false, x.NoRecibo, x.ClienteNombre, x.Detalles, x.Fecha.ToString("dd/MM/yyyy"), x.CuentaMostrar, x.Estado, x.Monto.ToString("N2"));
                }
                lblTotal.Text = $"Total registros: {data.Count}";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Pagos recibidos", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Nuevo()
        {
            using var frm = new FormCxCCobroEdit();
            if (frm.ShowDialog(this) == DialogResult.OK)
                Cargar();
        }
    }
}
