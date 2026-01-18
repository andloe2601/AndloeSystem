using System;
using System.Drawing;
using System.Windows.Forms;
using Andloe.Data;

namespace Andloe.Presentacion
{
    public partial class FormSelectorDocumento : Form
    {
        public string TipoSeleccionado { get; private set; } = FacturaRepository.TIPO_COT;

        public FormSelectorDocumento()
        {
            InitializeComponent();
            ApplyStyle();
        }

        private void ApplyStyle()
        {
            Font = new Font("Segoe UI", 9F);
            BackColor = Color.White;

            void Style(Button b, Color bg, Color fg)
            {
                b.FlatStyle = FlatStyle.Flat;
                b.FlatAppearance.BorderSize = 0;
                b.BackColor = bg;
                b.ForeColor = fg;
                b.Cursor = Cursors.Hand;
            }

            Style(btnCot, Color.FromArgb(33, 150, 243), Color.White);
            Style(btnProforma, Color.FromArgb(33, 150, 243), Color.White);
            Style(btnFactura, Color.FromArgb(33, 150, 243), Color.White);
            Style(btnCancelar, Color.FromArgb(240, 240, 240), Color.Black);
        }

        private void btnCot_Click(object sender, EventArgs e)
        {
            TipoSeleccionado = FacturaRepository.TIPO_COT;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnProforma_Click(object sender, EventArgs e)
        {
            TipoSeleccionado = FacturaRepository.TIPO_PF;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnFactura_Click(object sender, EventArgs e)
        {
            TipoSeleccionado = FacturaRepository.TIPO_FAC;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
