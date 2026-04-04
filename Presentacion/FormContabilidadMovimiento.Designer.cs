#nullable disable
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Andloe.Presentacion
{
    partial class FormContabilidadMovimiento
    {
        private IContainer components = null;

        private Panel panelTop;
        private Panel panelMid;
        private Panel panelBottom;

        private Panel panelPlantilla;


        private Label lblNoAsiento;

        private Label lblDescCab;
        private TextBox txtDescripcion;

        private Label lblOrigen;
        private TextBox txtOrigen;

        private Label lblOrigenId;
        private NumericUpDown nudOrigenId;

        private Label lblMoneda;
        private ComboBox cboMoneda;

        private Label lblTC;
        private NumericUpDown nudTC;

        private Button btnCrear;
        private Button btnCerrar;

        private Label lblCuentaCodigo;
        private TextBox txtCuentaCodigo;

        private Label lblLineaDesc;
        private TextBox txtLineaDesc;

        private Label lblDeb;
        private NumericUpDown nudDeb;

        private Label lblCred;
        private NumericUpDown nudCred;

        private Button btnAddLinea;

        private Label lblPlantilla;

        private Label lblPlantillaModulo;
        private ComboBox cboPlantillaModulo;
        private Label lblPlantillaEvento;
        private ComboBox cboPlantillaEvento;
        private Label lblPlantillaRol;
        private ComboBox cboPlantillaRol;
        private Button btnPlantillaCargar;
        private Button btnPlantillaAplicar;
        private Button btnVentaAutoCalcular;
        private Button btnCobroAutoCalcular;
        private Button btnNcAutoCalcular;

        private Label lblVentaSubtotal;
        private NumericUpDown nudVentaSubtotal;
        private Label lblVentaItbis;
        private NumericUpDown nudVentaItbis;
        private Label lblVentaDescuento;
        private NumericUpDown nudVentaDescuento;
        private Label lblVentaTotal;
        private NumericUpDown nudVentaTotal;

        private Label lblCobroMonto;
        private NumericUpDown nudCobroMonto;
        private Label lblCobroEfectivo;
        private NumericUpDown nudCobroEfectivo;
        private Label lblCobroTarjeta;
        private NumericUpDown nudCobroTarjeta;
        private Label lblCobroTransfer;
        private NumericUpDown nudCobroTransfer;
        private Label lblCobroTotal;
        private NumericUpDown nudCobroTotal;

        private Label lblNcBase;
        private NumericUpDown nudNcBase;
        private Label lblNcItbis;
        private NumericUpDown nudNcItbis;
        private Label lblNcTotal;
        private NumericUpDown nudNcTotal;

        private DataGridView gridPlantilla;

        private DataGridView grid;

        private FlowLayoutPanel footerTotals;
        private Label lblTotDebMon;
        private Label lblTotCredMon;
        private Label lblTotDebBase;
        private Label lblTotCredBase;
        private Label lblDifBase;

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
            panelMid = new Panel();
            panelBottom = new Panel();

            panelPlantilla = new Panel();


            lblNoAsiento = new Label();

            lblDescCab = new Label();
            txtDescripcion = new TextBox();

            lblOrigen = new Label();
            txtOrigen = new TextBox();

            lblOrigenId = new Label();
            nudOrigenId = new NumericUpDown();

            lblMoneda = new Label();
            cboMoneda = new ComboBox();

            lblTC = new Label();
            nudTC = new NumericUpDown();

            btnCrear = new Button();
            btnCerrar = new Button();

            lblCuentaCodigo = new Label();
            txtCuentaCodigo = new TextBox();

            lblLineaDesc = new Label();
            txtLineaDesc = new TextBox();

            lblDeb = new Label();
            nudDeb = new NumericUpDown();

            lblCred = new Label();
            nudCred = new NumericUpDown();

            btnAddLinea = new Button();

            lblPlantilla = new Label();
            lblPlantillaModulo = new Label();
            cboPlantillaModulo = new ComboBox();
            lblPlantillaEvento = new Label();
            cboPlantillaEvento = new ComboBox();
            lblPlantillaRol = new Label();
            cboPlantillaRol = new ComboBox();
            btnPlantillaCargar = new Button();
            btnPlantillaAplicar = new Button();
            btnVentaAutoCalcular = new Button();
            btnCobroAutoCalcular = new Button();
            btnNcAutoCalcular = new Button();

            lblVentaSubtotal = new Label();
            nudVentaSubtotal = new NumericUpDown();
            lblVentaItbis = new Label();
            nudVentaItbis = new NumericUpDown();
            lblVentaDescuento = new Label();
            nudVentaDescuento = new NumericUpDown();
            lblVentaTotal = new Label();
            nudVentaTotal = new NumericUpDown();

            lblCobroMonto = new Label();
            nudCobroMonto = new NumericUpDown();
            lblCobroEfectivo = new Label();
            nudCobroEfectivo = new NumericUpDown();
            lblCobroTarjeta = new Label();
            nudCobroTarjeta = new NumericUpDown();
            lblCobroTransfer = new Label();
            nudCobroTransfer = new NumericUpDown();
            lblCobroTotal = new Label();
            nudCobroTotal = new NumericUpDown();

            lblNcBase = new Label();
            nudNcBase = new NumericUpDown();
            lblNcItbis = new Label();
            nudNcItbis = new NumericUpDown();
            lblNcTotal = new Label();
            nudNcTotal = new NumericUpDown();

            gridPlantilla = new DataGridView();

            
            grid = new DataGridView();

            footerTotals = new FlowLayoutPanel();
            lblTotDebMon = new Label();
            lblTotCredMon = new Label();
            lblTotDebBase = new Label();
            lblTotCredBase = new Label();
            lblDifBase = new Label();

            ((ISupportInitialize)nudOrigenId).BeginInit();
            ((ISupportInitialize)nudTC).BeginInit();
            ((ISupportInitialize)nudDeb).BeginInit();
            ((ISupportInitialize)nudCred).BeginInit();
            ((ISupportInitialize)nudVentaSubtotal).BeginInit();
            ((ISupportInitialize)nudVentaItbis).BeginInit();
            ((ISupportInitialize)nudVentaDescuento).BeginInit();
            ((ISupportInitialize)nudVentaTotal).BeginInit();
            ((ISupportInitialize)nudCobroMonto).BeginInit();
            ((ISupportInitialize)nudCobroEfectivo).BeginInit();
            ((ISupportInitialize)nudCobroTarjeta).BeginInit();
            ((ISupportInitialize)nudCobroTransfer).BeginInit();
            ((ISupportInitialize)nudCobroTotal).BeginInit();
            ((ISupportInitialize)nudNcBase).BeginInit();
            ((ISupportInitialize)nudNcItbis).BeginInit();
            ((ISupportInitialize)nudNcTotal).BeginInit();
            ((ISupportInitialize)gridPlantilla).BeginInit();
            ((ISupportInitialize)grid).BeginInit();
            SuspendLayout();

            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1200, 750);
            Name = "FormContabilidadMovimiento";
            Text = "Contabilidad - Movimiento";
            StartPosition = FormStartPosition.CenterScreen;

            // panelTop
            panelTop.Dock = DockStyle.Top;
            panelTop.Height = 140;
            panelTop.Padding = new Padding(10);
            panelTop.BackColor = Color.White;

            lblNoAsiento.AutoSize = true;
            lblNoAsiento.Font = new Font("Segoe UI", 10F, FontStyle.Bold, GraphicsUnit.Point);
            lblNoAsiento.Location = new Point(10, 10);
            lblNoAsiento.Text = "NoAsiento: (sin crear)";

            lblDescCab.AutoSize = true;
            lblDescCab.Location = new Point(10, 50);
            lblDescCab.Text = "Descripción:";

            txtDescripcion.Location = new Point(90, 46);
            txtDescripcion.Size = new Size(420, 23);

            lblOrigen.AutoSize = true;
            lblOrigen.Location = new Point(525, 50);
            lblOrigen.Text = "Origen:";

            txtOrigen.Location = new Point(575, 46);
            txtOrigen.Size = new Size(120, 23);
            txtOrigen.Text = "MANUAL";

            lblOrigenId.AutoSize = true;
            lblOrigenId.Location = new Point(710, 50);
            lblOrigenId.Text = "OrigenId:";

            nudOrigenId.Location = new Point(772, 46);
            nudOrigenId.Size = new Size(120, 23);
            nudOrigenId.Maximum = 999999999;

            lblMoneda.AutoSize = true;
            lblMoneda.Location = new Point(10, 90);
            lblMoneda.Text = "Moneda:";

            cboMoneda.DropDownStyle = ComboBoxStyle.DropDownList;
            cboMoneda.Location = new Point(90, 86);
            cboMoneda.Size = new Size(90, 23);

            lblTC.AutoSize = true;
            lblTC.Location = new Point(200, 90);
            lblTC.Text = "TC:";

            nudTC.Location = new Point(230, 86);
            nudTC.Size = new Size(120, 23);
            nudTC.DecimalPlaces = 6;
            nudTC.Maximum = 9999999;
            nudTC.Value = 1;

            btnCrear.Location = new Point(375, 84);
            btnCrear.Size = new Size(120, 27);
            btnCrear.Text = "Crear";
            btnCrear.Click += btnCrear_Click;

            btnCerrar.Location = new Point(505, 84);
            btnCerrar.Size = new Size(120, 27);
            btnCerrar.Text = "Cerrar";
            btnCerrar.Click += btnCerrar_Click;

            panelTop.Controls.Add(lblNoAsiento);
            panelTop.Controls.Add(lblDescCab);
            panelTop.Controls.Add(txtDescripcion);
            panelTop.Controls.Add(lblOrigen);
            panelTop.Controls.Add(txtOrigen);
            panelTop.Controls.Add(lblOrigenId);
            panelTop.Controls.Add(nudOrigenId);
            panelTop.Controls.Add(lblMoneda);
            panelTop.Controls.Add(cboMoneda);
            panelTop.Controls.Add(lblTC);
            panelTop.Controls.Add(nudTC);
            panelTop.Controls.Add(btnCrear);
            panelTop.Controls.Add(btnCerrar);

            // panelPlantilla
            panelPlantilla.Dock = DockStyle.Top;
            panelPlantilla.Height = 340;
            panelPlantilla.Padding = new Padding(10);
            panelPlantilla.BackColor = Color.White;

            lblPlantilla.AutoSize = true;
            lblPlantilla.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            lblPlantilla.Location = new Point(10, 10);
            lblPlantilla.Text = "Plantillas contables (ContabilidadConfig)";

            lblPlantillaModulo.AutoSize = true;
            lblPlantillaModulo.Location = new Point(10, 40);
            lblPlantillaModulo.Text = "Módulo:";

            cboPlantillaModulo.Location = new Point(70, 36);
            cboPlantillaModulo.Size = new Size(160, 23);
            cboPlantillaModulo.DropDownStyle = ComboBoxStyle.DropDownList;

            lblPlantillaEvento.AutoSize = true;
            lblPlantillaEvento.Location = new Point(245, 40);
            lblPlantillaEvento.Text = "Evento:";

            cboPlantillaEvento.Location = new Point(295, 36);
            cboPlantillaEvento.Size = new Size(220, 23);
            cboPlantillaEvento.DropDownStyle = ComboBoxStyle.DropDownList;

            lblPlantillaRol.AutoSize = true;
            lblPlantillaRol.Location = new Point(530, 40);
            lblPlantillaRol.Text = "Rol:";

            cboPlantillaRol.Location = new Point(565, 36);
            cboPlantillaRol.Size = new Size(180, 23);
            cboPlantillaRol.DropDownStyle = ComboBoxStyle.DropDownList;

            btnPlantillaCargar.Location = new Point(760, 34);
            btnPlantillaCargar.Size = new Size(120, 27);
            btnPlantillaCargar.Text = "Cargar";

            btnPlantillaAplicar.Location = new Point(890, 34);
            btnPlantillaAplicar.Size = new Size(120, 27);
            btnPlantillaAplicar.Text = "Aplicar";

            // VENTA - Valores para auto-cálculo
            lblVentaSubtotal.AutoSize = true;
            lblVentaSubtotal.Location = new Point(10, 75);
            lblVentaSubtotal.Text = "Subtotal:";

            nudVentaSubtotal.Location = new Point(70, 71);
            nudVentaSubtotal.Size = new Size(120, 23);
            nudVentaSubtotal.DecimalPlaces = 2;
            nudVentaSubtotal.Maximum = 999999999;

            lblVentaItbis.AutoSize = true;
            lblVentaItbis.Location = new Point(205, 75);
            lblVentaItbis.Text = "ITBIS:";

            nudVentaItbis.Location = new Point(255, 71);
            nudVentaItbis.Size = new Size(120, 23);
            nudVentaItbis.DecimalPlaces = 2;
            nudVentaItbis.Maximum = 999999999;

            lblVentaDescuento.AutoSize = true;
            lblVentaDescuento.Location = new Point(390, 75);
            lblVentaDescuento.Text = "Desc:";

            nudVentaDescuento.Location = new Point(435, 71);
            nudVentaDescuento.Size = new Size(120, 23);
            nudVentaDescuento.DecimalPlaces = 2;
            nudVentaDescuento.Maximum = 999999999;

            lblVentaTotal.AutoSize = true;
            lblVentaTotal.Location = new Point(570, 75);
            lblVentaTotal.Text = "Total:";

            nudVentaTotal.Location = new Point(615, 71);
            nudVentaTotal.Size = new Size(120, 23);
            nudVentaTotal.DecimalPlaces = 2;
            nudVentaTotal.Maximum = 999999999;
            nudVentaTotal.Enabled = false;

            btnVentaAutoCalcular.Location = new Point(760, 69);
            btnVentaAutoCalcular.Size = new Size(250, 27);
            btnVentaAutoCalcular.Text = "VENTA - Auto-calcular plantilla";

            // COBRO - Valores para auto-cálculo (AMBOS: Caja/Banco)
            lblCobroMonto.AutoSize = true;
            lblCobroMonto.Location = new Point(10, 105);
            lblCobroMonto.Text = "Monto:";

            nudCobroMonto.Location = new Point(70, 101);
            nudCobroMonto.Size = new Size(120, 23);
            nudCobroMonto.DecimalPlaces = 2;
            nudCobroMonto.Maximum = 999999999;

            lblCobroEfectivo.AutoSize = true;
            lblCobroEfectivo.Location = new Point(205, 105);
            lblCobroEfectivo.Text = "Efectivo:";

            nudCobroEfectivo.Location = new Point(265, 101);
            nudCobroEfectivo.Size = new Size(110, 23);
            nudCobroEfectivo.DecimalPlaces = 2;
            nudCobroEfectivo.Maximum = 999999999;

            lblCobroTarjeta.AutoSize = true;
            lblCobroTarjeta.Location = new Point(390, 105);
            lblCobroTarjeta.Text = "Tarjeta:";

            nudCobroTarjeta.Location = new Point(445, 101);
            nudCobroTarjeta.Size = new Size(110, 23);
            nudCobroTarjeta.DecimalPlaces = 2;
            nudCobroTarjeta.Maximum = 999999999;

            lblCobroTransfer.AutoSize = true;
            lblCobroTransfer.Location = new Point(570, 105);
            lblCobroTransfer.Text = "Transfer:";

            nudCobroTransfer.Location = new Point(635, 101);
            nudCobroTransfer.Size = new Size(100, 23);
            nudCobroTransfer.DecimalPlaces = 2;
            nudCobroTransfer.Maximum = 999999999;

            lblCobroTotal.AutoSize = true;
            lblCobroTotal.Location = new Point(750, 105);
            lblCobroTotal.Text = "Total:";

            nudCobroTotal.Location = new Point(795, 101);
            nudCobroTotal.Size = new Size(120, 23);
            nudCobroTotal.DecimalPlaces = 2;
            nudCobroTotal.Maximum = 999999999;
            nudCobroTotal.Enabled = false;

            btnCobroAutoCalcular.Location = new Point(930, 99);
            btnCobroAutoCalcular.Size = new Size(240, 27);
            btnCobroAutoCalcular.Text = "COBRO - Auto-calcular plantilla";

            // NC / Devolución - Valores para auto-cálculo
            lblNcBase.AutoSize = true;
            lblNcBase.Location = new Point(10, 135);
            lblNcBase.Text = "Base NC:";

            nudNcBase.Location = new Point(70, 131);
            nudNcBase.Size = new Size(120, 23);
            nudNcBase.DecimalPlaces = 2;
            nudNcBase.Maximum = 999999999;

            lblNcItbis.AutoSize = true;
            lblNcItbis.Location = new Point(205, 135);
            lblNcItbis.Text = "ITBIS NC:";

            nudNcItbis.Location = new Point(275, 131);
            nudNcItbis.Size = new Size(120, 23);
            nudNcItbis.DecimalPlaces = 2;
            nudNcItbis.Maximum = 999999999;

            lblNcTotal.AutoSize = true;
            lblNcTotal.Location = new Point(410, 135);
            lblNcTotal.Text = "Total NC:";

            nudNcTotal.Location = new Point(475, 131);
            nudNcTotal.Size = new Size(120, 23);
            nudNcTotal.DecimalPlaces = 2;
            nudNcTotal.Maximum = 999999999;
            nudNcTotal.Enabled = false;

            btnNcAutoCalcular.Location = new Point(610, 129);
            btnNcAutoCalcular.Size = new Size(250, 27);
            btnNcAutoCalcular.Text = "NC - Auto-calcular plantilla";

            gridPlantilla.Location = new Point(10, 175);
            gridPlantilla.Size = new Size(1160, 155);
            gridPlantilla.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            gridPlantilla.ReadOnly = false;
            gridPlantilla.AllowUserToAddRows = false;
            gridPlantilla.AllowUserToDeleteRows = false;

            panelPlantilla.Controls.Add(lblPlantilla);
            panelPlantilla.Controls.Add(lblPlantillaModulo);
            panelPlantilla.Controls.Add(cboPlantillaModulo);
            panelPlantilla.Controls.Add(lblPlantillaEvento);
            panelPlantilla.Controls.Add(cboPlantillaEvento);
            panelPlantilla.Controls.Add(lblPlantillaRol);
            panelPlantilla.Controls.Add(cboPlantillaRol);
            panelPlantilla.Controls.Add(btnPlantillaCargar);
            panelPlantilla.Controls.Add(btnPlantillaAplicar);
            panelPlantilla.Controls.Add(lblVentaSubtotal);
            panelPlantilla.Controls.Add(nudVentaSubtotal);
            panelPlantilla.Controls.Add(lblVentaItbis);
            panelPlantilla.Controls.Add(nudVentaItbis);
            panelPlantilla.Controls.Add(lblVentaDescuento);
            panelPlantilla.Controls.Add(nudVentaDescuento);
            panelPlantilla.Controls.Add(lblVentaTotal);
            panelPlantilla.Controls.Add(nudVentaTotal);
            panelPlantilla.Controls.Add(btnVentaAutoCalcular);
            panelPlantilla.Controls.Add(lblCobroMonto);
            panelPlantilla.Controls.Add(nudCobroMonto);
            panelPlantilla.Controls.Add(lblCobroEfectivo);
            panelPlantilla.Controls.Add(nudCobroEfectivo);
            panelPlantilla.Controls.Add(lblCobroTarjeta);
            panelPlantilla.Controls.Add(nudCobroTarjeta);
            panelPlantilla.Controls.Add(lblCobroTransfer);
            panelPlantilla.Controls.Add(nudCobroTransfer);
            panelPlantilla.Controls.Add(lblCobroTotal);
            panelPlantilla.Controls.Add(nudCobroTotal);
            panelPlantilla.Controls.Add(btnCobroAutoCalcular);
            panelPlantilla.Controls.Add(lblNcBase);
            panelPlantilla.Controls.Add(nudNcBase);
            panelPlantilla.Controls.Add(lblNcItbis);
            panelPlantilla.Controls.Add(nudNcItbis);
            panelPlantilla.Controls.Add(lblNcTotal);
            panelPlantilla.Controls.Add(nudNcTotal);
            panelPlantilla.Controls.Add(btnNcAutoCalcular);
            panelPlantilla.Controls.Add(gridPlantilla);

            
            // panelMid
            panelMid.Dock = DockStyle.Top;
            panelMid.Height = 90;
            panelMid.Padding = new Padding(10);
            panelMid.BackColor = Color.White;

            lblCuentaCodigo.AutoSize = true;
            lblCuentaCodigo.Location = new Point(10, 15);
            lblCuentaCodigo.Text = "Cuenta:";

            txtCuentaCodigo.Location = new Point(62, 11);
            txtCuentaCodigo.Size = new Size(110, 23);

            lblLineaDesc.AutoSize = true;
            lblLineaDesc.Location = new Point(190, 15);
            lblLineaDesc.Text = "Desc:";

            txtLineaDesc.Location = new Point(231, 11);
            txtLineaDesc.Size = new Size(360, 23);

            lblDeb.AutoSize = true;
            lblDeb.Location = new Point(610, 15);
            lblDeb.Text = "Débito:";

            nudDeb.Location = new Point(658, 11);
            nudDeb.Size = new Size(120, 23);
            nudDeb.DecimalPlaces = 2;
            nudDeb.Maximum = 999999999;

            lblCred.AutoSize = true;
            lblCred.Location = new Point(800, 15);
            lblCred.Text = "Crédito:";

            nudCred.Location = new Point(853, 11);
            nudCred.Size = new Size(120, 23);
            nudCred.DecimalPlaces = 2;
            nudCred.Maximum = 999999999;

            btnAddLinea.Location = new Point(990, 10);
            btnAddLinea.Size = new Size(140, 27);
            btnAddLinea.Text = "Agregar Línea";
            btnAddLinea.Click += btnAddLinea_Click;

            panelMid.Controls.Add(lblCuentaCodigo);
            panelMid.Controls.Add(txtCuentaCodigo);
            panelMid.Controls.Add(lblLineaDesc);
            panelMid.Controls.Add(txtLineaDesc);
            panelMid.Controls.Add(lblDeb);
            panelMid.Controls.Add(nudDeb);
            panelMid.Controls.Add(lblCred);
            panelMid.Controls.Add(nudCred);
            panelMid.Controls.Add(btnAddLinea);

            // panelBottom
            panelBottom.Dock = DockStyle.Fill;
            panelBottom.Padding = new Padding(10);
            panelBottom.BackColor = Color.White;

            grid.Dock = DockStyle.Fill;
            grid.ReadOnly = true;
            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.MultiSelect = false;

            footerTotals.Dock = DockStyle.Bottom;
            footerTotals.Height = 34;
            footerTotals.Padding = new Padding(2);
            footerTotals.FlowDirection = FlowDirection.LeftToRight;
            footerTotals.WrapContents = false;

            lblTotDebMon.AutoSize = true;
            lblTotDebMon.Text = "Total Débito: 0.00";

            lblTotCredMon.AutoSize = true;
            lblTotCredMon.Text = "Total Crédito: 0.00";

            lblTotDebBase.AutoSize = true;
            lblTotDebBase.Text = "Total DébitoBase: 0.00";

            lblTotCredBase.AutoSize = true;
            lblTotCredBase.Text = "Total CréditoBase: 0.00";

            lblDifBase.AutoSize = true;
            lblDifBase.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            lblDifBase.Text = "DifBase: 0.00";

            footerTotals.Controls.Add(lblTotDebMon);
            footerTotals.Controls.Add(new Label { AutoSize = true, Text = "   " });
            footerTotals.Controls.Add(lblTotCredMon);
            footerTotals.Controls.Add(new Label { AutoSize = true, Text = "   " });
            footerTotals.Controls.Add(lblTotDebBase);
            footerTotals.Controls.Add(new Label { AutoSize = true, Text = "   " });
            footerTotals.Controls.Add(lblTotCredBase);
            footerTotals.Controls.Add(new Label { AutoSize = true, Text = "   " });
            footerTotals.Controls.Add(lblDifBase);

            panelBottom.Controls.Add(grid);
            panelBottom.Controls.Add(footerTotals);

            Controls.Add(panelBottom);
            Controls.Add(panelMid);
            Controls.Add(panelPlantilla);
            Controls.Add(panelTop);

            ((ISupportInitialize)nudOrigenId).EndInit();
            ((ISupportInitialize)nudTC).EndInit();
            ((ISupportInitialize)nudDeb).EndInit();
            ((ISupportInitialize)nudCred).EndInit();
            ((ISupportInitialize)nudVentaSubtotal).EndInit();
            ((ISupportInitialize)nudVentaItbis).EndInit();
            ((ISupportInitialize)nudVentaDescuento).EndInit();
            ((ISupportInitialize)nudVentaTotal).EndInit();
            ((ISupportInitialize)nudCobroMonto).EndInit();
            ((ISupportInitialize)nudCobroEfectivo).EndInit();
            ((ISupportInitialize)nudCobroTarjeta).EndInit();
            ((ISupportInitialize)nudCobroTransfer).EndInit();
            ((ISupportInitialize)nudCobroTotal).EndInit();
            ((ISupportInitialize)nudNcBase).EndInit();
            ((ISupportInitialize)nudNcItbis).EndInit();
            ((ISupportInitialize)nudNcTotal).EndInit();
            ((ISupportInitialize)gridPlantilla).EndInit();
            ((ISupportInitialize)grid).EndInit();

            ResumeLayout(false);
        }
    }
}
#nullable restore
