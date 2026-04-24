using System;
using System.Drawing;
using System.Windows.Forms;
using Andloe.Logica;
using Andloe.Presentation;

namespace Andloe.Presentacion
{
    public partial class FormDashboard : Form
    {
        private readonly Action<Form> _openChild;
        private readonly AuthorizationService _auth;

        // Constructor principal
        public FormDashboard(Action<Form> openChild, AuthorizationService auth)
        {
            _openChild = openChild ?? throw new ArgumentNullException(nameof(openChild));
            _auth = auth ?? throw new ArgumentNullException(nameof(auth));

            InitializeComponent();
            ApplyTheme();
            WireEvents();
        }

        // Constructor para diseñador
        public FormDashboard() : this(
            f =>
            {
                f.TopLevel = true;
                f.Show();
            },
            new AuthorizationService(Array.Empty<string>()))
        { }

        private void ApplyTheme()
        {
            BackColor = Color.White;
            lblTitulo.ForeColor = Color.FromArgb(46, 51, 73);

            foreach (Control c in panelKPIs.Controls)
            {
                if (c is Panel p)
                    p.BackColor = Color.White;
            }

            foreach (Control c in gridBotones.Controls)
            {
                if (c is Button b)
                {
                    b.FlatStyle = FlatStyle.Flat;
                    b.FlatAppearance.BorderSize = 0;
                    b.BackColor = Color.FromArgb(46, 51, 73);
                    b.ForeColor = Color.White;
                    b.Cursor = Cursors.Hand;
                }
            }
        }

        private void WireEvents()
        {
            btnUsuarios.Click += (_, __) => _openChild(new FormUsuarios(_auth));
            btnProductos.Click += (_, __) => _openChild(new FormProductos());
            btnClientes.Click += (_, __) => _openChild(new FormClientes());
            // btnProveedores.Click += (_, __) => _openChild(new FormProveedores());
            // btnPOS.Click += (_, __) => _openChild(new FormPOS());
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
            catch
            {
                // no romper el dashboard
            }
        }
    }
}