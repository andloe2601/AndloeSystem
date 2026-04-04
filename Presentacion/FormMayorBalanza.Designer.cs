#nullable disable
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Andloe.Presentacion
{
    partial class FormMayorBalanza
    {
        private IContainer components = null;

        private Panel panelTop;
        private Panel panelBottom;

        private Label lblDesde;
        private DateTimePicker dtDesde;

        private Label lblHasta;
        private DateTimePicker dtHasta;

        private Button btnBuscar;

        private DataGridView grid;
        private Label lblTot;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new Container();

            panelTop = new Panel();
            panelBottom = new Panel();

            lblDesde = new Label();
            dtDesde = new DateTimePicker();

            lblHasta = new Label();
            dtHasta = new DateTimePicker();

            btnBuscar = new Button();

            grid = new DataGridView();
            lblTot = new Label();

            ((ISupportInitialize)grid).BeginInit();
            SuspendLayout();

            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1100, 650);
            Name = "FormMayorBalanza";
            Text = "Contabilidad - Mayor/Balanza";
            StartPosition = FormStartPosition.CenterScreen;

            panelTop.Dock = DockStyle.Top;
            panelTop.Height = 60;
            panelTop.Padding = new Padding(10);
            panelTop.BackColor = Color.White;

            lblDesde.AutoSize = true;
            lblDesde.Location = new Point(10, 20);
            lblDesde.Text = "Desde:";

            dtDesde.Location = new Point(60, 16);
            dtDesde.Size = new Size(220, 23);
            dtDesde.Format = DateTimePickerFormat.Short;
            dtDesde.Value = DateTime.Today.AddMonths(-1);

            lblHasta.AutoSize = true;
            lblHasta.Location = new Point(300, 20);
            lblHasta.Text = "Hasta:";

            dtHasta.Location = new Point(345, 16);
            dtHasta.Size = new Size(220, 23);
            dtHasta.Format = DateTimePickerFormat.Short;
            dtHasta.Value = DateTime.Today;

            btnBuscar.Location = new Point(590, 15);
            btnBuscar.Size = new Size(120, 27);
            btnBuscar.Text = "Buscar";
            btnBuscar.Click += btnBuscar_Click;

            panelTop.Controls.Add(lblDesde);
            panelTop.Controls.Add(dtDesde);
            panelTop.Controls.Add(lblHasta);
            panelTop.Controls.Add(dtHasta);
            panelTop.Controls.Add(btnBuscar);

            panelBottom.Dock = DockStyle.Fill;
            panelBottom.Padding = new Padding(10);
            panelBottom.BackColor = Color.White;

            lblTot.Dock = DockStyle.Bottom;
            lblTot.Height = 28;
            lblTot.TextAlign = ContentAlignment.MiddleLeft;
            lblTot.Text = "Totales -> Deb: 0.00 | Cred: 0.00 | DebBase: 0.00 | CredBase: 0.00";

            grid.Dock = DockStyle.Fill;
            grid.ReadOnly = true;
            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.MultiSelect = false;

            panelBottom.Controls.Add(grid);
            panelBottom.Controls.Add(lblTot);

            Controls.Add(panelBottom);
            Controls.Add(panelTop);

            ((ISupportInitialize)grid).EndInit();
            ResumeLayout(false);
        }
    }
}
#nullable restore
