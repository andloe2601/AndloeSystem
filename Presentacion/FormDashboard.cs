using System;
using System.Drawing;
using System.Windows.Forms;
using Presentation;
namespace Andloe.Presentacion
{
    public partial class FormDashboard : Form
    {
        private readonly Action<Form> _openChild;

        // ✔ Constructor principal: usa el callback para abrir dentro del panel del FormPrincipal
        public FormDashboard(Action<Form> openChild)
        {
            _openChild = openChild ?? throw new ArgumentNullException(nameof(openChild));
            InitializeComponent();
            ApplyTheme();
            WireEvents();
        }

        // ✔ Constructor sin parámetros: útil para el diseñador.
        // En runtime, si se usa este por error, abrirá el form hijo normal (no embebido).
        public FormDashboard() : this(f =>
        {
            // fallback seguro (no embebido). Úsalo solo si te olvidaste del callback.
            f.TopLevel = true;
            f.Show();
        })
        { }

        private void ApplyTheme()
        {
            BackColor = Color.White;
            lblTitulo.ForeColor = Color.FromArgb(46, 51, 73);

            foreach (Control c in panelKPIs.Controls)
                if (c is Panel p) p.BackColor = Color.White;

            foreach (Control c in gridBotones.Controls)
                if (c is Button b)
                {
                    b.FlatStyle = FlatStyle.Flat;
                    b.FlatAppearance.BorderSize = 0;
                    b.BackColor = Color.FromArgb(46, 51, 73);
                    b.ForeColor = Color.White;
                    b.Cursor = Cursors.Hand;
                }
        }

        private void WireEvents()
        {
            btnUsuarios.Click += (_, __) => _openChild(new FormUsuarios());
            btnProductos.Click += (_, __) => _openChild(new FormProductos());
            btnClientes.Click += (_, __) => _openChild(new FormClientes());
           //btnProveedores.Click += (_, __) => _openChild(new FormProveedores());
            //btnPOS.Click += (_, __) => _openChild(new FormPOS());
            btnConfiguracion.Click += (_, __) => _openChild(new FormConfiguracion());

            btnRefrescar.Click += (_, __) => RefrescarKPIs();
            Shown += (_, __) => RefrescarKPIs();
        }

        private void RefrescarKPIs()
        {
            try
            {
                lblKpiUsuarios.Text = "—";
                lblKpiProductos.Text = "—";
                lblKpiClientes.Text = "—";
                lblKpiProveedores.Text = "—";
            }
            catch { /* no romper el dashboard */ }
        }
    }
}
