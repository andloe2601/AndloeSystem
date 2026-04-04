using System;
using System.Windows.Forms;

namespace Andloe.Presentacion
{
    public partial class FormCxCReportes : Form
    {
        public FormCxCReportes()
        {
            InitializeComponent();

            btnEstadoCuenta.Click += (_, __) => AbrirFormulario(new FormCxCEstadoCuenta());
            btnReportesGerenciales.Click += (_, __) => AbrirFormulario(new FormCxCReportesGerenciales());
            BtnVendedor.Click += (_, __) => AbrirFormulario(new FormVentasPorVendedor());
        }

        private void AbrirFormulario(Form form)
        {
            if (FormPrincipal.Instancia != null)
            {
                FormPrincipal.Instancia.OpenChild(form);
            }
            else
            {
                form.ShowDialog(this);
            }
        }
    }
}