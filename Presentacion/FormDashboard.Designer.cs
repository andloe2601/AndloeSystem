using System.Drawing;
using System.Windows.Forms;

namespace Andloe.Presentacion
{
    partial class FormDashboard
    {
        private System.ComponentModel.IContainer components = null;

        private Label lblTitulo;
        private Panel panelKPIs;
        private Panel cardUsuarios;
        private Panel cardProductos;
        private Panel cardClientes;
        private Panel cardProveedores;
        private Label lblKpiUsuarios;
        private Label lblKpiProductos;
        private Label lblKpiClientes;
        private Label lblKpiProveedores;
        private Button btnRefrescar;

        private TableLayoutPanel gridBotones;
        private Button btnUsuarios;
        private Button btnProductos;
        private Button btnClientes;
        private Button btnProveedores;
        private Button btnPOS;
        private Button btnConfiguracion;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private Panel MakeCard(string titulo, out Label lblValor)
        {
            var card = new Panel
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(8),
                Padding = new Padding(12),
                BackColor = Color.White
            };

            var lblT = new Label
            {
                Text = titulo,
                Dock = DockStyle.Top,
                Height = 20,
                ForeColor = Color.FromArgb(90, 90, 90)
            };
            lblValor = new Label
            {
                Text = "—",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 20f, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft,
                ForeColor = Color.FromArgb(46, 51, 73)
            };

            card.Controls.Add(lblValor);
            card.Controls.Add(lblT);
            return card;
        }

        private Button MakeTile(string text)
        {
            return new Button
            {
                Text = text,
                Dock = DockStyle.Fill,
                Height = 90,
                Margin = new Padding(8),
                FlatStyle = FlatStyle.Flat
            };
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();

            SuspendLayout();
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            ClientSize = new Size(980, 620);
            FormBorderStyle = FormBorderStyle.None;
            Text = "Dashboard";

            // ===== Título + refrescar =====
            lblTitulo = new Label
            {
                Text = "Dashboard",
                Dock = DockStyle.Top,
                Height = 44,
                Font = new Font("Segoe UI", 16f, FontStyle.Bold),
                Padding = new Padding(12, 6, 12, 0),
                ForeColor = Color.FromArgb(46, 51, 73)
            };

            btnRefrescar = new Button
            {
                Text = "Refrescar",
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Width = 110,
                Height = 32,
                Top = 8,
                Left = 0 // lo reubicamos después
            };

            var header = new Panel
            {
                Dock = DockStyle.Top,
                Height = 54
            };
            header.Controls.Add(lblTitulo);
            header.Controls.Add(btnRefrescar);
            Controls.Add(header);

            // posicionar btnRefrescar a la derecha del header
            header.Resize += (_, __) =>
            {
                btnRefrescar.Left = header.Width - btnRefrescar.Width - 12;
                btnRefrescar.Top = (header.Height - btnRefrescar.Height) / 2;
            };

            // ===== KPIs (4 tarjetas) =====
            panelKPIs = new Panel
            {
                Dock = DockStyle.Top,
                Height = 110,
                Padding = new Padding(8),
                BackColor = Color.White
            };
            Controls.Add(panelKPIs);

            var gridKpi = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 4,
                RowCount = 1
            };
            gridKpi.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
            gridKpi.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
            gridKpi.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
            gridKpi.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
            panelKPIs.Controls.Add(gridKpi);

            cardUsuarios = MakeCard("Usuarios", out lblKpiUsuarios);
            cardProductos = MakeCard("Productos", out lblKpiProductos);
            cardClientes = MakeCard("Clientes", out lblKpiClientes);
            cardProveedores = MakeCard("Proveedores", out lblKpiProveedores);

            gridKpi.Controls.Add(cardUsuarios, 0, 0);
            gridKpi.Controls.Add(cardProductos, 1, 0);
            gridKpi.Controls.Add(cardClientes, 2, 0);
            gridKpi.Controls.Add(cardProveedores, 3, 0);

            // ===== Grid de botones (tiles) =====
            gridBotones = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(12),
                ColumnCount = 3,
                RowCount = 2
            };
            gridBotones.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33f));
            gridBotones.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33f));
            gridBotones.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33f));
            gridBotones.RowStyles.Add(new RowStyle(SizeType.Percent, 50f));
            gridBotones.RowStyles.Add(new RowStyle(SizeType.Percent, 50f));
            Controls.Add(gridBotones);

            btnUsuarios = MakeTile("Usuarios");
            btnProductos = MakeTile("Productos");
            btnClientes = MakeTile("Clientes");
            btnProveedores = MakeTile("Proveedores");
            btnPOS = MakeTile("Punto de Venta");
            btnConfiguracion = MakeTile("Configuración");

            gridBotones.Controls.Add(btnUsuarios, 0, 0);
            gridBotones.Controls.Add(btnProductos, 1, 0);
            gridBotones.Controls.Add(btnClientes, 2, 0);
            gridBotones.Controls.Add(btnProveedores, 0, 1);
            gridBotones.Controls.Add(btnPOS, 1, 1);
            gridBotones.Controls.Add(btnConfiguracion, 2, 1);

            ResumeLayout(false);
        }
    }
}
