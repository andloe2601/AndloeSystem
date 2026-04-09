using System.Drawing;
using System.Windows.Forms;

namespace Andloe.Presentacion
{
    partial class FormClienteEdit
    {
        private System.ComponentModel.IContainer components = null;

        private TextBox txtCodigo;
        private TextBox txtNombre;
        private TextBox txtRnc;
        private TextBox txtTel;
        private TextBox txtEmail;
        private TextBox txtDir;
        private ComboBox cboTipo;
        private CheckBox chkActivo;
        private Button btnGuardar;
        private Button btnCancelar;
        private NumericUpDown numCreditoMaximo;
        private TextBox txtDivisa;
        private TextBox txtTermino;
        private TextBox txtVendedor;
        private TextBox txtAlmacen;
        private NumericUpDown numDescuentoMaximo;
        private ComboBox cboTerminoPago;
        private ComboBox cboMoneda;
        private ComboBox cboVendedor;
        private ComboBox cboAlmacen;
        private TextBox txtRazonFiscal;
        private TextBox txtNombreComercialFiscal;
        private ComboBox cboTipoIdentificacion;
        private TextBox txtProvinciaCodigo;
        private TextBox txtMunicipioCodigo;
        private TextBox txtPaisCodigo;
        private TextBox txtCorreoFiscal;
        private CheckBox chkEsContribuyente;
        private ComboBox cboTipoClienteFiscal;
        private CheckBox chkValidadoDgii;
        private DateTimePicker dtpFechaValidacion;
        private TextBox txtEstadoRncDgii;
        private TextBox txtIdentificadorExtranjero;
        private CheckBox chkEsExtranjero;
        private Button btnValidarDgii;
        private TextBox txtEstadoFiscal;
        private ComboBox cboProvincia;
        private ComboBox cboMunicipio;
        private ComboBox cboPais;

        private TabControl tabs;
        private TabPage tabComercial;
        private TabPage tabFiscal;
        private TableLayoutPanel pnlRoot;
        private TableLayoutPanel pnlEstado;
        private FlowLayoutPanel flow;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            txtCodigo = new TextBox();
            txtNombre = new TextBox();
            txtRnc = new TextBox();
            txtTel = new TextBox();
            txtEmail = new TextBox();
            txtDir = new TextBox();
            cboTipo = new ComboBox();
            chkActivo = new CheckBox();
            btnGuardar = new Button();
            btnCancelar = new Button();
            numCreditoMaximo = new NumericUpDown();
            txtDivisa = new TextBox();
            txtTermino = new TextBox();
            txtVendedor = new TextBox();
            txtAlmacen = new TextBox();
            numDescuentoMaximo = new NumericUpDown();
            cboTerminoPago = new ComboBox();
            cboProvincia = new ComboBox();
            cboMunicipio = new ComboBox();
            cboMoneda = new ComboBox();
            cboPais = new ComboBox();
            cboVendedor = new ComboBox();
            cboAlmacen = new ComboBox();
            txtRazonFiscal = new TextBox();
            txtNombreComercialFiscal = new TextBox();
            cboTipoIdentificacion = new ComboBox();
            txtProvinciaCodigo = new TextBox();
            txtMunicipioCodigo = new TextBox();
            txtPaisCodigo = new TextBox();
            txtCorreoFiscal = new TextBox();
            chkEsContribuyente = new CheckBox();
            cboTipoClienteFiscal = new ComboBox();
            chkValidadoDgii = new CheckBox();
            dtpFechaValidacion = new DateTimePicker();
            txtEstadoRncDgii = new TextBox();
            txtIdentificadorExtranjero = new TextBox();
            chkEsExtranjero = new CheckBox();
            btnValidarDgii = new Button();
            txtEstadoFiscal = new TextBox();
            tabs = new TabControl();
            tabComercial = new TabPage();
            tabFiscal = new TabPage();
            pnlRoot = new TableLayoutPanel();
            pnlEstado = new TableLayoutPanel();
            lblEstadoFiscal = new Label();
            flow = new FlowLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)numCreditoMaximo).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numDescuentoMaximo).BeginInit();
            tabs.SuspendLayout();
            pnlRoot.SuspendLayout();
            pnlEstado.SuspendLayout();
            flow.SuspendLayout();
            SuspendLayout();

            // === TAB COMERCIAL ===
            var comercial = CrearGridBase();
            int rowC = 0;

            txtCodigo.ReadOnly = true;
            AgregarFila(comercial, ref rowC, "Código", txtCodigo, readOnly: true);
            AgregarFila(comercial, ref rowC, "Nombre / Razón comercial *", txtNombre);
            AgregarFila(comercial, ref rowC, "RNC / Cédula *", txtRnc);
            AgregarFila(comercial, ref rowC, "Teléfono", txtTel);
            AgregarFila(comercial, ref rowC, "Email", txtEmail);
            AgregarFila(comercial, ref rowC, "Dirección", txtDir, multiline: true);

            cboTipo.DropDownStyle = ComboBoxStyle.DropDownList;
            AgregarFila(comercial, ref rowC, "Tipo cliente", cboTipo);

            chkActivo.Text = "Cliente activo";
            AgregarFila(comercial, ref rowC, "Estado", chkActivo);

            numCreditoMaximo.DecimalPlaces = 2;
            numCreditoMaximo.Maximum = 999999999;
            numCreditoMaximo.ThousandsSeparator = true;
            AgregarFila(comercial, ref rowC, "Crédito máximo", numCreditoMaximo);

            cboMoneda.DropDownStyle = ComboBoxStyle.DropDownList;
            AgregarFila(comercial, ref rowC, "Divisa *", cboMoneda);

            cboTerminoPago.DropDownStyle = ComboBoxStyle.DropDownList;
            AgregarFila(comercial, ref rowC, "Término de pago *", cboTerminoPago);

            cboVendedor.DropDownStyle = ComboBoxStyle.DropDownList;
            AgregarFila(comercial, ref rowC, "Vendedor", cboVendedor);

            cboAlmacen.DropDownStyle = ComboBoxStyle.DropDownList;
            AgregarFila(comercial, ref rowC, "Almacén", cboAlmacen);

            numDescuentoMaximo.DecimalPlaces = 2;
            numDescuentoMaximo.Maximum = 100;
            numDescuentoMaximo.ThousandsSeparator = true;
            AgregarFila(comercial, ref rowC, "Desc. máximo %", numDescuentoMaximo);

            tabComercial.Controls.Add(comercial);

            // === TAB FISCAL ===
            var fiscal = CrearGridBase();
            int rowF = 0;

            AgregarFila(fiscal, ref rowF, "Razón social fiscal *", txtRazonFiscal);
            AgregarFila(fiscal, ref rowF, "Nombre comercial fiscal", txtNombreComercialFiscal);

            cboTipoIdentificacion.DropDownStyle = ComboBoxStyle.DropDownList;
            AgregarFila(fiscal, ref rowF, "Tipo identificación fiscal *", cboTipoIdentificacion);

            cboProvincia.DropDownStyle = ComboBoxStyle.DropDownList;
            AgregarFila(fiscal, ref rowF, "Provincia *", cboProvincia);

            cboMunicipio.DropDownStyle = ComboBoxStyle.DropDownList;
            AgregarFila(fiscal, ref rowF, "Municipio *", cboMunicipio);

            cboPais.DropDownStyle = ComboBoxStyle.DropDownList;
            AgregarFila(fiscal, ref rowF, "País *", cboPais);
            AgregarFila(fiscal, ref rowF, "Correo fiscal", txtCorreoFiscal);

            chkEsContribuyente.Text = "Es contribuyente";
            AgregarFila(fiscal, ref rowF, "Contribuyente", chkEsContribuyente);

            cboTipoClienteFiscal.DropDownStyle = ComboBoxStyle.DropDownList;
            AgregarFila(fiscal, ref rowF, "Tipo cliente fiscal", cboTipoClienteFiscal);

            chkValidadoDgii.Text = "Validado DGII";
            AgregarFila(fiscal, ref rowF, "DGII validado *", chkValidadoDgii);

            dtpFechaValidacion.Format = DateTimePickerFormat.Short;
            dtpFechaValidacion.ShowCheckBox = true;
            AgregarFila(fiscal, ref rowF, "Fecha validación", dtpFechaValidacion);

            AgregarFila(fiscal, ref rowF, "Estado RNC DGII", txtEstadoRncDgii);

            chkEsExtranjero.Text = "Es extranjero";
            AgregarFila(fiscal, ref rowF, "Extranjero", chkEsExtranjero);

            AgregarFila(fiscal, ref rowF, "Identificador extranjero", txtIdentificadorExtranjero);

            btnValidarDgii.Text = "Validar DGII";
            AgregarFila(fiscal, ref rowF, "Consulta DGII", btnValidarDgii);

            tabFiscal.Controls.Add(fiscal);

            // 
            // txtCodigo
            // 
            txtCodigo.Location = new Point(0, 0);
            txtCodigo.Name = "txtCodigo";
            txtCodigo.ReadOnly = true;
            txtCodigo.Size = new Size(100, 23);
            txtCodigo.TabIndex = 0;
            // 
            // txtNombre
            // 
            txtNombre.Location = new Point(0, 0);
            txtNombre.Name = "txtNombre";
            txtNombre.Size = new Size(100, 23);
            txtNombre.TabIndex = 0;
            // 
            // txtRnc
            // 
            txtRnc.Location = new Point(0, 0);
            txtRnc.Name = "txtRnc";
            txtRnc.Size = new Size(100, 23);
            txtRnc.TabIndex = 0;
            // 
            // txtTel
            // 
            txtTel.Location = new Point(0, 0);
            txtTel.Name = "txtTel";
            txtTel.Size = new Size(100, 23);
            txtTel.TabIndex = 0;
            // 
            // txtEmail
            // 
            txtEmail.Location = new Point(0, 0);
            txtEmail.Name = "txtEmail";
            txtEmail.Size = new Size(100, 23);
            txtEmail.TabIndex = 0;
            // 
            // txtDir
            // 
            txtDir.Location = new Point(0, 0);
            txtDir.Name = "txtDir";
            txtDir.TabIndex = 0;
            // 
            // cboTipo
            // 
            cboTipo.DropDownStyle = ComboBoxStyle.DropDownList;
            cboTipo.Location = new Point(0, 0);
            cboTipo.Name = "cboTipo";
            cboTipo.Size = new Size(121, 23);
            cboTipo.TabIndex = 0;
            // 
            // chkActivo
            // 
            chkActivo.Location = new Point(0, 0);
            chkActivo.Name = "chkActivo";
            chkActivo.Size = new Size(104, 24);
            chkActivo.TabIndex = 0;
            chkActivo.Text = "Cliente activo";
            // 
            // btnGuardar
            // 
            btnGuardar.Location = new Point(665, 13);
            btnGuardar.Name = "btnGuardar";
            btnGuardar.Size = new Size(100, 30);
            btnGuardar.TabIndex = 1;
            btnGuardar.Text = "Guardar";
            // 
            // btnCancelar
            // 
            btnCancelar.Location = new Point(771, 13);
            btnCancelar.Name = "btnCancelar";
            btnCancelar.Size = new Size(100, 30);
            btnCancelar.TabIndex = 0;
            btnCancelar.Text = "Cancelar";
            // 
            // numCreditoMaximo
            // 
            numCreditoMaximo.DecimalPlaces = 2;
            numCreditoMaximo.Location = new Point(0, 0);
            numCreditoMaximo.Maximum = new decimal(new int[] { 999999999, 0, 0, 0 });
            numCreditoMaximo.Name = "numCreditoMaximo";
            numCreditoMaximo.Size = new Size(120, 23);
            numCreditoMaximo.TabIndex = 0;
            numCreditoMaximo.ThousandsSeparator = true;
            // 
            // txtDivisa
            // 
            txtDivisa.Location = new Point(0, 0);
            txtDivisa.Name = "txtDivisa";
            txtDivisa.Size = new Size(100, 23);
            txtDivisa.TabIndex = 0;
            // 
            // txtTermino
            // 
            txtTermino.Location = new Point(0, 0);
            txtTermino.Name = "txtTermino";
            txtTermino.Size = new Size(100, 23);
            txtTermino.TabIndex = 0;
            // 
            // txtVendedor
            // 
            txtVendedor.Location = new Point(0, 0);
            txtVendedor.Name = "txtVendedor";
            txtVendedor.Size = new Size(100, 23);
            txtVendedor.TabIndex = 0;
            // 
            // txtAlmacen
            // 
            txtAlmacen.Location = new Point(0, 0);
            txtAlmacen.Name = "txtAlmacen";
            txtAlmacen.Size = new Size(100, 23);
            txtAlmacen.TabIndex = 0;
            // 
            // numDescuentoMaximo
            // 
            numDescuentoMaximo.DecimalPlaces = 2;
            numDescuentoMaximo.Location = new Point(0, 0);
            numDescuentoMaximo.Name = "numDescuentoMaximo";
            numDescuentoMaximo.Size = new Size(120, 23);
            numDescuentoMaximo.TabIndex = 0;
            numDescuentoMaximo.ThousandsSeparator = true;
            // 
            // cboTerminoPago
            // 
            cboTerminoPago.DropDownStyle = ComboBoxStyle.DropDownList;
            cboTerminoPago.Location = new Point(0, 0);
            cboTerminoPago.Name = "cboTerminoPago";
            cboTerminoPago.Size = new Size(121, 23);
            cboTerminoPago.TabIndex = 0;
            // 
            // txtRazonFiscal
            // 
            txtRazonFiscal.Location = new Point(0, 0);
            txtRazonFiscal.Name = "txtRazonFiscal";
            txtRazonFiscal.Size = new Size(100, 23);
            txtRazonFiscal.TabIndex = 0;
            // 
            // txtNombreComercialFiscal
            // 
            txtNombreComercialFiscal.Location = new Point(0, 0);
            txtNombreComercialFiscal.Name = "txtNombreComercialFiscal";
            txtNombreComercialFiscal.Size = new Size(100, 23);
            txtNombreComercialFiscal.TabIndex = 0;
            // 
            // cboTipoIdentificacion
            // 
            cboTipoIdentificacion.DropDownStyle = ComboBoxStyle.DropDownList;
            cboTipoIdentificacion.Location = new Point(0, 0);
            cboTipoIdentificacion.Name = "cboTipoIdentificacion";
            cboTipoIdentificacion.Size = new Size(121, 23);
            cboTipoIdentificacion.TabIndex = 0;
            // 
            // txtProvinciaCodigo
            // 
            txtProvinciaCodigo.Location = new Point(0, 0);
            txtProvinciaCodigo.Name = "txtProvinciaCodigo";
            txtProvinciaCodigo.Size = new Size(100, 23);
            txtProvinciaCodigo.TabIndex = 0;
            // 
            // txtMunicipioCodigo
            // 
            txtMunicipioCodigo.Location = new Point(0, 0);
            txtMunicipioCodigo.Name = "txtMunicipioCodigo";
            txtMunicipioCodigo.Size = new Size(100, 23);
            txtMunicipioCodigo.TabIndex = 0;
            // 
            // txtPaisCodigo
            // 
            txtPaisCodigo.Location = new Point(0, 0);
            txtPaisCodigo.Name = "txtPaisCodigo";
            txtPaisCodigo.Size = new Size(100, 23);
            txtPaisCodigo.TabIndex = 0;
            // 
            // txtCorreoFiscal
            // 
            txtCorreoFiscal.Location = new Point(0, 0);
            txtCorreoFiscal.Name = "txtCorreoFiscal";
            txtCorreoFiscal.Size = new Size(100, 23);
            txtCorreoFiscal.TabIndex = 0;
            // 
            // chkEsContribuyente
            // 
            chkEsContribuyente.Location = new Point(0, 0);
            chkEsContribuyente.Name = "chkEsContribuyente";
            chkEsContribuyente.Size = new Size(104, 24);
            chkEsContribuyente.TabIndex = 0;
            chkEsContribuyente.Text = "Es contribuyente";
            // 
            // cboTipoClienteFiscal
            // 
            cboTipoClienteFiscal.DropDownStyle = ComboBoxStyle.DropDownList;
            cboTipoClienteFiscal.Location = new Point(0, 0);
            cboTipoClienteFiscal.Name = "cboTipoClienteFiscal";
            cboTipoClienteFiscal.Size = new Size(121, 23);
            cboTipoClienteFiscal.TabIndex = 0;
            // 
            // chkValidadoDgii
            // 
            chkValidadoDgii.Location = new Point(0, 0);
            chkValidadoDgii.Name = "chkValidadoDgii";
            chkValidadoDgii.Size = new Size(104, 24);
            chkValidadoDgii.TabIndex = 0;
            chkValidadoDgii.Text = "Validado DGII";
            // 
            // dtpFechaValidacion
            // 
            dtpFechaValidacion.Format = DateTimePickerFormat.Short;
            dtpFechaValidacion.Location = new Point(0, 0);
            dtpFechaValidacion.Name = "dtpFechaValidacion";
            dtpFechaValidacion.ShowCheckBox = true;
            dtpFechaValidacion.Size = new Size(200, 23);
            dtpFechaValidacion.TabIndex = 0;
            // 
            // txtEstadoRncDgii
            // 
            txtEstadoRncDgii.Location = new Point(0, 0);
            txtEstadoRncDgii.Name = "txtEstadoRncDgii";
            txtEstadoRncDgii.Size = new Size(100, 23);
            txtEstadoRncDgii.TabIndex = 0;
            // 
            // txtIdentificadorExtranjero
            // 
            txtIdentificadorExtranjero.Location = new Point(0, 0);
            txtIdentificadorExtranjero.Name = "txtIdentificadorExtranjero";
            txtIdentificadorExtranjero.Size = new Size(100, 23);
            txtIdentificadorExtranjero.TabIndex = 0;
            // 
            // chkEsExtranjero
            // 
            chkEsExtranjero.Location = new Point(0, 0);
            chkEsExtranjero.Name = "chkEsExtranjero";
            chkEsExtranjero.Size = new Size(104, 24);
            chkEsExtranjero.TabIndex = 0;
            chkEsExtranjero.Text = "Es extranjero";
            // 
            // btnValidarDgii
            // 
            btnValidarDgii.Location = new Point(0, 0);
            btnValidarDgii.Name = "btnValidarDgii";
            btnValidarDgii.Size = new Size(120, 23);
            btnValidarDgii.TabIndex = 0;
            btnValidarDgii.Text = "Validar DGII";
            // 
            // txtEstadoFiscal
            // 
            txtEstadoFiscal.Dock = DockStyle.Fill;
            txtEstadoFiscal.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            txtEstadoFiscal.Location = new Point(173, 11);
            txtEstadoFiscal.Name = "txtEstadoFiscal";
            txtEstadoFiscal.ReadOnly = true;
            txtEstadoFiscal.BorderStyle = BorderStyle.FixedSingle;
            txtEstadoFiscal.Size = new Size(708, 23);
            txtEstadoFiscal.TabIndex = 1;
            // 
            // tabs
            // 
            tabs.Controls.Add(tabComercial);
            tabs.Controls.Add(tabFiscal);
            tabs.Dock = DockStyle.Fill;
            tabs.Location = new Point(3, 48);
            tabs.Name = "tabs";
            tabs.SelectedIndex = 0;
            tabs.Size = new Size(894, 594);
            tabs.TabIndex = 1;
            // 
            // tabComercial
            // 
            tabComercial.Location = new Point(4, 24);
            tabComercial.Name = "tabComercial";
            tabComercial.Size = new Size(886, 566);
            tabComercial.TabIndex = 0;
            tabComercial.Text = "Comercial";
            // 
            // tabFiscal
            // 
            tabFiscal.Location = new Point(4, 24);
            tabFiscal.Name = "tabFiscal";
            tabFiscal.Size = new Size(886, 566);
            tabFiscal.TabIndex = 1;
            tabFiscal.Text = "Fiscal / E-CF";
            // 
            // pnlRoot
            // 
            pnlRoot.ColumnCount = 1;
            pnlRoot.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            pnlRoot.Controls.Add(pnlEstado, 0, 0);
            pnlRoot.Controls.Add(tabs, 0, 1);
            pnlRoot.Controls.Add(flow, 0, 2);
            pnlRoot.Dock = DockStyle.Fill;
            pnlRoot.Location = new Point(0, 0);
            pnlRoot.Name = "pnlRoot";
            pnlRoot.RowCount = 3;
            pnlRoot.RowStyles.Add(new RowStyle(SizeType.Absolute, 45F));
            pnlRoot.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            pnlRoot.RowStyles.Add(new RowStyle(SizeType.Absolute, 55F));
            pnlRoot.Size = new Size(900, 700);
            pnlRoot.TabIndex = 0;
            // 
            // pnlEstado
            // 
            pnlEstado.ColumnCount = 2;
            pnlEstado.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 160F));
            pnlEstado.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            pnlEstado.Controls.Add(lblEstadoFiscal, 0, 0);
            pnlEstado.Controls.Add(txtEstadoFiscal, 1, 0);
            pnlEstado.Dock = DockStyle.Fill;
            pnlEstado.Location = new Point(3, 3);
            pnlEstado.Name = "pnlEstado";
            pnlEstado.Padding = new Padding(10, 8, 10, 8);
            pnlEstado.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            pnlEstado.Size = new Size(894, 39);
            pnlEstado.TabIndex = 0;
            // 
            // lblEstadoFiscal
            // 
            lblEstadoFiscal.Location = new Point(13, 8);
            lblEstadoFiscal.Name = "lblEstadoFiscal";
            lblEstadoFiscal.Size = new Size(100, 23);
            lblEstadoFiscal.TabIndex = 0;
            lblEstadoFiscal.Text = "Estado fiscal";
            lblEstadoFiscal.TextAlign = ContentAlignment.MiddleRight;
            // 
            // flow
            // 
            flow.Controls.Add(btnCancelar);
            flow.Controls.Add(btnGuardar);
            flow.Dock = DockStyle.Fill;
            flow.FlowDirection = FlowDirection.RightToLeft;
            flow.Location = new Point(3, 648);
            flow.Name = "flow";
            flow.Padding = new Padding(10);
            flow.Size = new Size(894, 49);
            flow.TabIndex = 2;
            flow.WrapContents = false;
            // 
            // FormClienteEdit
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(900, 700);
            Controls.Add(pnlRoot);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FormClienteEdit";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Cliente";
            ((System.ComponentModel.ISupportInitialize)numCreditoMaximo).EndInit();
            ((System.ComponentModel.ISupportInitialize)numDescuentoMaximo).EndInit();
            tabs.ResumeLayout(false);
            pnlRoot.ResumeLayout(false);
            pnlEstado.ResumeLayout(false);
            pnlEstado.PerformLayout();
            flow.ResumeLayout(false);
            ResumeLayout(false);
        }

        private static TableLayoutPanel CrearGridBase()
        {
            var table = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(12),
                ColumnCount = 2,
                AutoScroll = true,
                RowCount = 0
            };
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 190));
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            return table;
        }

        private static void AgregarFila(TableLayoutPanel table, ref int row, string label, Control control, bool readOnly = false, bool multiline = false)
        {
            table.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            var lbl = new Label
            {
                Text = label,
                TextAlign = ContentAlignment.MiddleRight,
                Dock = DockStyle.Fill,
                AutoSize = true,
                Padding = new Padding(0, 6, 0, 0)
            };

            control.Dock = DockStyle.Fill;

            if (control is TextBox tb)
            {
                tb.ReadOnly = readOnly;
                if (multiline)
                {
                    tb.Multiline = true;
                    tb.Height = 70;
                }
            }

            table.Controls.Add(lbl, 0, row);
            table.Controls.Add(control, 1, row);
            row++;
        }
        private Label lblEstadoFiscal;
    }
}