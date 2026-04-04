#nullable disable
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Andloe.Presentacion
{
    partial class FormConfiguracion
    {
        private System.ComponentModel.IContainer components = null;

        private TabControl tabMain;
        private TabPage tpGeneral;
        private TabPage tpNumeradores;
        private TabPage tpReportes;

        private Label labelMonedaBase;
        private ComboBox cbMonedaBase;
        private Label labelMedioPagoDefecto;
        private ComboBox cbMedioPagoDefecto;
        private Label labelClienteDefecto;
        private TextBox txtClienteDefecto;
        private Label labelStockNegativo;
        private CheckBox chkPermitirStockNegativo;

        private Label labelEmpresa;
        private ComboBox cbEmpresa;
        private Label labelSucursal;
        private ComboBox cbSucursal;
        private Label labelAlmacenDefecto;
        private ComboBox cbAlmacenDefecto;

        private Label labelAlmacenOrigenPos;
        private ComboBox cbAlmacenOrigenPos;
        private Label labelAlmacenDestinoPos;
        private ComboBox cbAlmacenDestinoPos;

        private Button btnActualizarDgii;

        private Label labelTituloProducto;
        private Label labelProdPrefijo;
        private TextBox txtNumProdPrefijo;
        private Label labelProdLong;
        private TextBox txtNumProdLongitud;
        private Label labelProdActual;
        private TextBox txtNumProdActual;
        private Label lblPreviewProd;

        private Label labelTituloCliente;
        private Label labelCliPrefijo;
        private TextBox txtNumCliPrefijo;
        private Label labelCliLong;
        private TextBox txtNumCliLongitud;
        private Label labelCliActual;
        private TextBox txtNumCliActual;
        private Label lblPreviewCli;

        private Label labelTituloProveedor;
        private Label labelProvPrefijo;
        private TextBox txtNumProvPrefijo;
        private Label labelProvLong;
        private TextBox txtNumProvLongitud;
        private Label labelProvActual;
        private TextBox txtNumProvActual;
        private Label lblPreviewProv;

        private Label labelTituloFactVen;
        private Label labelFactVenPrefijo;
        private TextBox txtNumFactVenPrefijo;
        private Label labelFactVenLong;
        private TextBox txtNumFactVenLongitud;
        private Label labelFactVenActual;
        private TextBox txtNumFactVenActual;
        private Label lblPreviewFactVen;

        private Label labelTituloFactCom;
        private Label labelFactComPrefijo;
        private TextBox txtNumFactComPrefijo;
        private Label labelFactComLong;
        private TextBox txtNumFactComLongitud;
        private Label labelFactComActual;
        private TextBox txtNumFactComActual;
        private Label lblPreviewFactCom;

        private Label labelTituloNcVen;
        private Label labelNcVenPrefijo;
        private TextBox txtNumNcVenPrefijo;
        private Label labelNcVenLong;
        private TextBox txtNumNcVenLongitud;
        private Label labelNcVenActual;
        private TextBox txtNumNcVenActual;
        private Label lblPreviewNcVen;

        private Button btnGuardar;
        private Button btnCerrar;

        private Label lblAlanubeTitulo;
        private Label lblAlanubeBaseUrl;
        private TextBox txtAlanubeBaseUrl;
        private Label lblAlanubeToken;
        private TextBox txtAlanubeToken;
        private Label lblAlanubeAmbiente;
        private ComboBox cbAlanubeAmbiente;
        private Label lblAlanubeTimeout;
        private TextBox txtAlanubeTimeout;
        private Label lblAlanubeIdCompany;
        private TextBox txtAlanubeIdCompany;
        private Label lblAlanubeRetryNumber;
        private TextBox txtAlanubeRetryNumber;
        private Button btnProbarAlanube;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            tabMain = new TabControl();
            tpGeneral = new TabPage();
            labelMonedaBase = new Label();
            cbMonedaBase = new ComboBox();
            labelMedioPagoDefecto = new Label();
            cbMedioPagoDefecto = new ComboBox();
            labelClienteDefecto = new Label();
            txtClienteDefecto = new TextBox();
            labelStockNegativo = new Label();
            chkPermitirStockNegativo = new CheckBox();
            labelEmpresa = new Label();
            cbEmpresa = new ComboBox();
            labelSucursal = new Label();
            cbSucursal = new ComboBox();
            labelAlmacenDefecto = new Label();
            cbAlmacenDefecto = new ComboBox();
            labelAlmacenOrigenPos = new Label();
            cbAlmacenOrigenPos = new ComboBox();
            labelAlmacenDestinoPos = new Label();
            cbAlmacenDestinoPos = new ComboBox();
            btnActualizarDgii = new Button();
            tpNumeradores = new TabPage();
            labelTituloProducto = new Label();
            labelProdPrefijo = new Label();
            txtNumProdPrefijo = new TextBox();
            labelProdLong = new Label();
            txtNumProdLongitud = new TextBox();
            labelProdActual = new Label();
            txtNumProdActual = new TextBox();
            lblPreviewProd = new Label();
            labelTituloCliente = new Label();
            labelCliPrefijo = new Label();
            txtNumCliPrefijo = new TextBox();
            labelCliLong = new Label();
            txtNumCliLongitud = new TextBox();
            labelCliActual = new Label();
            txtNumCliActual = new TextBox();
            lblPreviewCli = new Label();
            labelTituloProveedor = new Label();
            labelProvPrefijo = new Label();
            txtNumProvPrefijo = new TextBox();
            labelProvLong = new Label();
            txtNumProvLongitud = new TextBox();
            labelProvActual = new Label();
            txtNumProvActual = new TextBox();
            lblPreviewProv = new Label();
            labelTituloFactVen = new Label();
            labelFactVenPrefijo = new Label();
            txtNumFactVenPrefijo = new TextBox();
            labelFactVenLong = new Label();
            txtNumFactVenLongitud = new TextBox();
            labelFactVenActual = new Label();
            txtNumFactVenActual = new TextBox();
            lblPreviewFactVen = new Label();
            labelTituloFactCom = new Label();
            labelFactComPrefijo = new Label();
            txtNumFactComPrefijo = new TextBox();
            labelFactComLong = new Label();
            txtNumFactComLongitud = new TextBox();
            labelFactComActual = new Label();
            txtNumFactComActual = new TextBox();
            lblPreviewFactCom = new Label();
            labelTituloNcVen = new Label();
            labelNcVenPrefijo = new Label();
            txtNumNcVenPrefijo = new TextBox();
            labelNcVenLong = new Label();
            txtNumNcVenLongitud = new TextBox();
            labelNcVenActual = new Label();
            txtNumNcVenActual = new TextBox();
            lblPreviewNcVen = new Label();
            tpReportes = new TabPage();
            btnGuardar = new Button();
            btnCerrar = new Button();
            BtnPostulacion = new Button();
            lblAlanubeTitulo = new Label();
            lblAlanubeBaseUrl = new Label();
            txtAlanubeBaseUrl = new TextBox();
            lblAlanubeToken = new Label();
            txtAlanubeToken = new TextBox();
            lblAlanubeAmbiente = new Label();
            cbAlanubeAmbiente = new ComboBox();
            lblAlanubeTimeout = new Label();
            txtAlanubeTimeout = new TextBox();
            lblAlanubeIdCompany = new Label();
            txtAlanubeIdCompany = new TextBox();
            lblAlanubeRetryNumber = new Label();
            txtAlanubeRetryNumber = new TextBox();
            btnProbarAlanube = new Button();

            tabMain.SuspendLayout();
            tpGeneral.SuspendLayout();
            tpNumeradores.SuspendLayout();
            SuspendLayout();
            // 
            // tabMain
            // 
            tabMain.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tabMain.Controls.Add(tpGeneral);
            tabMain.Controls.Add(tpNumeradores);
            tabMain.Controls.Add(tpReportes);
            tabMain.Location = new Point(12, 12);
            tabMain.Name = "tabMain";
            tabMain.SelectedIndex = 0;
            tabMain.Size = new Size(900, 660);
            tabMain.TabIndex = 0;
            // 
            // tpGeneral
            // 
            tpGeneral.Controls.Add(BtnPostulacion);
            tpGeneral.Controls.Add(labelMonedaBase);
            tpGeneral.Controls.Add(cbMonedaBase);
            tpGeneral.Controls.Add(labelMedioPagoDefecto);
            tpGeneral.Controls.Add(cbMedioPagoDefecto);
            tpGeneral.Controls.Add(labelClienteDefecto);
            tpGeneral.Controls.Add(txtClienteDefecto);
            tpGeneral.Controls.Add(labelStockNegativo);
            tpGeneral.Controls.Add(chkPermitirStockNegativo);
            tpGeneral.Controls.Add(labelEmpresa);
            tpGeneral.Controls.Add(cbEmpresa);
            tpGeneral.Controls.Add(labelSucursal);
            tpGeneral.Controls.Add(cbSucursal);
            tpGeneral.Controls.Add(labelAlmacenDefecto);
            tpGeneral.Controls.Add(cbAlmacenDefecto);
            tpGeneral.Controls.Add(labelAlmacenOrigenPos);
            tpGeneral.Controls.Add(cbAlmacenOrigenPos);
            tpGeneral.Controls.Add(labelAlmacenDestinoPos);
            tpGeneral.Controls.Add(cbAlmacenDestinoPos);
            tpGeneral.Controls.Add(btnActualizarDgii);
            tpGeneral.Location = new Point(4, 24);
            tpGeneral.Name = "tpGeneral";
            tpGeneral.Padding = new Padding(8);
            tpGeneral.Size = new Size(892, 632);
            tpGeneral.TabIndex = 0;
            tpGeneral.Text = "General";
            tpGeneral.UseVisualStyleBackColor = true;
            // 
            // labelMonedaBase
            // 
            labelMonedaBase.AutoSize = true;
            labelMonedaBase.Location = new Point(24, 26);
            labelMonedaBase.Name = "labelMonedaBase";
            labelMonedaBase.Size = new Size(81, 15);
            labelMonedaBase.TabIndex = 0;
            labelMonedaBase.Text = "Moneda base:";
            // 
            // cbMonedaBase
            // 
            cbMonedaBase.DropDownStyle = ComboBoxStyle.DropDownList;
            cbMonedaBase.Location = new Point(190, 22);
            cbMonedaBase.Name = "cbMonedaBase";
            cbMonedaBase.Size = new Size(260, 23);
            cbMonedaBase.TabIndex = 1;
            // 
            // labelMedioPagoDefecto
            // 
            labelMedioPagoDefecto.AutoSize = true;
            labelMedioPagoDefecto.Location = new Point(24, 66);
            labelMedioPagoDefecto.Name = "labelMedioPagoDefecto";
            labelMedioPagoDefecto.Size = new Size(133, 15);
            labelMedioPagoDefecto.TabIndex = 2;
            labelMedioPagoDefecto.Text = "Medio de pago defecto:";
            // 
            // cbMedioPagoDefecto
            // 
            cbMedioPagoDefecto.DropDownStyle = ComboBoxStyle.DropDownList;
            cbMedioPagoDefecto.Location = new Point(190, 62);
            cbMedioPagoDefecto.Name = "cbMedioPagoDefecto";
            cbMedioPagoDefecto.Size = new Size(260, 23);
            cbMedioPagoDefecto.TabIndex = 3;
            // 
            // labelClienteDefecto
            // 
            labelClienteDefecto.AutoSize = true;
            labelClienteDefecto.Location = new Point(24, 106);
            labelClienteDefecto.Name = "labelClienteDefecto";
            labelClienteDefecto.Size = new Size(90, 15);
            labelClienteDefecto.TabIndex = 4;
            labelClienteDefecto.Text = "Cliente defecto:";
            // 
            // txtClienteDefecto
            // 
            txtClienteDefecto.Location = new Point(190, 102);
            txtClienteDefecto.Name = "txtClienteDefecto";
            txtClienteDefecto.Size = new Size(180, 23);
            txtClienteDefecto.TabIndex = 5;
            // 
            // labelStockNegativo
            // 
            labelStockNegativo.AutoSize = true;
            labelStockNegativo.Location = new Point(24, 146);
            labelStockNegativo.Name = "labelStockNegativo";
            labelStockNegativo.Size = new Size(132, 15);
            labelStockNegativo.TabIndex = 6;
            labelStockNegativo.Text = "Permitir stock negativo:";
            // 
            // chkPermitirStockNegativo
            // 
            chkPermitirStockNegativo.AutoSize = true;
            chkPermitirStockNegativo.Location = new Point(190, 146);
            chkPermitirStockNegativo.Name = "chkPermitirStockNegativo";
            chkPermitirStockNegativo.Size = new Size(15, 14);
            chkPermitirStockNegativo.TabIndex = 7;
            // 
            // labelEmpresa
            // 
            labelEmpresa.AutoSize = true;
            labelEmpresa.Location = new Point(24, 186);
            labelEmpresa.Name = "labelEmpresa";
            labelEmpresa.Size = new Size(55, 15);
            labelEmpresa.TabIndex = 8;
            labelEmpresa.Text = "Empresa:";
            // 
            // cbEmpresa
            // 
            cbEmpresa.DropDownStyle = ComboBoxStyle.DropDownList;
            cbEmpresa.Location = new Point(190, 182);
            cbEmpresa.Name = "cbEmpresa";
            cbEmpresa.Size = new Size(380, 23);
            cbEmpresa.TabIndex = 9;
            // 
            // labelSucursal
            // 
            labelSucursal.AutoSize = true;
            labelSucursal.Location = new Point(24, 226);
            labelSucursal.Name = "labelSucursal";
            labelSucursal.Size = new Size(54, 15);
            labelSucursal.TabIndex = 10;
            labelSucursal.Text = "Sucursal:";
            // 
            // cbSucursal
            // 
            cbSucursal.DropDownStyle = ComboBoxStyle.DropDownList;
            cbSucursal.Location = new Point(190, 222);
            cbSucursal.Name = "cbSucursal";
            cbSucursal.Size = new Size(380, 23);
            cbSucursal.TabIndex = 11;
            // 
            // labelAlmacenDefecto
            // 
            labelAlmacenDefecto.AutoSize = true;
            labelAlmacenDefecto.Location = new Point(24, 266);
            labelAlmacenDefecto.Name = "labelAlmacenDefecto";
            labelAlmacenDefecto.Size = new Size(121, 15);
            labelAlmacenDefecto.TabIndex = 12;
            labelAlmacenDefecto.Text = "Almacén por defecto:";
            // 
            // cbAlmacenDefecto
            // 
            cbAlmacenDefecto.DropDownStyle = ComboBoxStyle.DropDownList;
            cbAlmacenDefecto.Location = new Point(190, 262);
            cbAlmacenDefecto.Name = "cbAlmacenDefecto";
            cbAlmacenDefecto.Size = new Size(380, 23);
            cbAlmacenDefecto.TabIndex = 13;
            // 
            // labelAlmacenOrigenPos
            // 
            labelAlmacenOrigenPos.AutoSize = true;
            labelAlmacenOrigenPos.Location = new Point(24, 306);
            labelAlmacenOrigenPos.Name = "labelAlmacenOrigenPos";
            labelAlmacenOrigenPos.Size = new Size(127, 15);
            labelAlmacenOrigenPos.TabIndex = 14;
            labelAlmacenOrigenPos.Text = "Almacén POS (origen):";
            // 
            // cbAlmacenOrigenPos
            // 
            cbAlmacenOrigenPos.DropDownStyle = ComboBoxStyle.DropDownList;
            cbAlmacenOrigenPos.Location = new Point(190, 302);
            cbAlmacenOrigenPos.Name = "cbAlmacenOrigenPos";
            cbAlmacenOrigenPos.Size = new Size(380, 23);
            cbAlmacenOrigenPos.TabIndex = 15;
            // 
            // labelAlmacenDestinoPos
            // 
            labelAlmacenDestinoPos.AutoSize = true;
            labelAlmacenDestinoPos.Location = new Point(24, 336);
            labelAlmacenDestinoPos.Name = "labelAlmacenDestinoPos";
            labelAlmacenDestinoPos.Size = new Size(132, 15);
            labelAlmacenDestinoPos.TabIndex = 16;
            labelAlmacenDestinoPos.Text = "Almacén POS (destino):";
            // 
            // cbAlmacenDestinoPos
            // 
            cbAlmacenDestinoPos.DropDownStyle = ComboBoxStyle.DropDownList;
            cbAlmacenDestinoPos.Location = new Point(190, 332);
            cbAlmacenDestinoPos.Name = "cbAlmacenDestinoPos";
            cbAlmacenDestinoPos.Size = new Size(380, 23);
            cbAlmacenDestinoPos.TabIndex = 17;

            // =========================
            //   ALANUBE
            // =========================
            lblAlanubeTitulo.AutoSize = true;
            lblAlanubeTitulo.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblAlanubeTitulo.Location = new Point(24, 470);
            lblAlanubeTitulo.Name = "lblAlanubeTitulo";
            lblAlanubeTitulo.Size = new Size(148, 15);
            lblAlanubeTitulo.Text = "Integración Alanube (RD)";

            lblAlanubeBaseUrl.AutoSize = true;
            lblAlanubeBaseUrl.Location = new Point(24, 500);
            lblAlanubeBaseUrl.Name = "lblAlanubeBaseUrl";
            lblAlanubeBaseUrl.Size = new Size(57, 15);
            lblAlanubeBaseUrl.Text = "Base URL:";

            txtAlanubeBaseUrl.Location = new Point(120, 496);
            txtAlanubeBaseUrl.Name = "txtAlanubeBaseUrl";
            txtAlanubeBaseUrl.Size = new Size(380, 23);
            txtAlanubeBaseUrl.TabIndex = 200;

            lblAlanubeToken.AutoSize = true;
            lblAlanubeToken.Location = new Point(24, 532);
            lblAlanubeToken.Name = "lblAlanubeToken";
            lblAlanubeToken.Size = new Size(43, 15);
            lblAlanubeToken.Text = "Token:";

            txtAlanubeToken.Location = new Point(120, 528);
            txtAlanubeToken.Name = "txtAlanubeToken";
            txtAlanubeToken.Size = new Size(380, 23);
            txtAlanubeToken.TabIndex = 201;
            txtAlanubeToken.UseSystemPasswordChar = true;

            lblAlanubeAmbiente.AutoSize = true;
            lblAlanubeAmbiente.Location = new Point(520, 500);
            lblAlanubeAmbiente.Name = "lblAlanubeAmbiente";
            lblAlanubeAmbiente.Size = new Size(61, 15);
            lblAlanubeAmbiente.Text = "Ambiente:";

            cbAlanubeAmbiente.DropDownStyle = ComboBoxStyle.DropDownList;
            cbAlanubeAmbiente.Location = new Point(600, 496);
            cbAlanubeAmbiente.Name = "cbAlanubeAmbiente";
            cbAlanubeAmbiente.Size = new Size(140, 23);
            cbAlanubeAmbiente.TabIndex = 202;

            lblAlanubeTimeout.AutoSize = true;
            lblAlanubeTimeout.Location = new Point(520, 532);
            lblAlanubeTimeout.Name = "lblAlanubeTimeout";
            lblAlanubeTimeout.Size = new Size(54, 15);
            lblAlanubeTimeout.Text = "Timeout:";

            txtAlanubeTimeout.Location = new Point(600, 528);
            txtAlanubeTimeout.Name = "txtAlanubeTimeout";
            txtAlanubeTimeout.Size = new Size(80, 23);
            txtAlanubeTimeout.TabIndex = 203;

            lblAlanubeIdCompany.AutoSize = true;
            lblAlanubeIdCompany.Location = new Point(24, 564);
            lblAlanubeIdCompany.Name = "lblAlanubeIdCompany";
            lblAlanubeIdCompany.Size = new Size(70, 15);
            lblAlanubeIdCompany.Text = "Id Company:";

            txtAlanubeIdCompany.Location = new Point(120, 560);
            txtAlanubeIdCompany.Name = "txtAlanubeIdCompany";
            txtAlanubeIdCompany.Size = new Size(380, 23);
            txtAlanubeIdCompany.TabIndex = 204;

            lblAlanubeRetryNumber.AutoSize = true;
            lblAlanubeRetryNumber.Location = new Point(520, 564);
            lblAlanubeRetryNumber.Name = "lblAlanubeRetryNumber";
            lblAlanubeRetryNumber.Size = new Size(79, 15);
            lblAlanubeRetryNumber.Text = "Retry Number:";

            txtAlanubeRetryNumber.Location = new Point(600, 560);
            txtAlanubeRetryNumber.Name = "txtAlanubeRetryNumber";
            txtAlanubeRetryNumber.Size = new Size(80, 23);
            txtAlanubeRetryNumber.TabIndex = 205;

            btnProbarAlanube.Location = new Point(700, 558);
            btnProbarAlanube.Name = "btnProbarAlanube";
            btnProbarAlanube.Size = new Size(120, 27);
            btnProbarAlanube.TabIndex = 206;
            btnProbarAlanube.Text = "Probar Alanube";
            btnProbarAlanube.UseVisualStyleBackColor = true;

            // 
            // btnActualizarDgii
            // 
            btnActualizarDgii.Location = new Point(590, 302);
            btnActualizarDgii.Name = "btnActualizarDgii";
            btnActualizarDgii.Size = new Size(120, 53);
            btnActualizarDgii.TabIndex = 18;
            btnActualizarDgii.Text = "Actualizar DGII";
            // 
            // tpNumeradores
            // 
            tpNumeradores.Controls.Add(labelTituloProducto);
            tpNumeradores.Controls.Add(labelProdPrefijo);
            tpNumeradores.Controls.Add(txtNumProdPrefijo);
            tpNumeradores.Controls.Add(labelProdLong);
            tpNumeradores.Controls.Add(txtNumProdLongitud);
            tpNumeradores.Controls.Add(labelProdActual);
            tpNumeradores.Controls.Add(txtNumProdActual);
            tpNumeradores.Controls.Add(lblPreviewProd);
            tpNumeradores.Controls.Add(labelTituloCliente);
            tpNumeradores.Controls.Add(labelCliPrefijo);
            tpNumeradores.Controls.Add(txtNumCliPrefijo);
            tpNumeradores.Controls.Add(labelCliLong);
            tpNumeradores.Controls.Add(txtNumCliLongitud);
            tpNumeradores.Controls.Add(labelCliActual);
            tpNumeradores.Controls.Add(txtNumCliActual);
            tpNumeradores.Controls.Add(lblPreviewCli);
            tpNumeradores.Controls.Add(labelTituloProveedor);
            tpNumeradores.Controls.Add(labelProvPrefijo);
            tpNumeradores.Controls.Add(txtNumProvPrefijo);
            tpNumeradores.Controls.Add(labelProvLong);
            tpNumeradores.Controls.Add(txtNumProvLongitud);
            tpNumeradores.Controls.Add(labelProvActual);
            tpNumeradores.Controls.Add(txtNumProvActual);
            tpNumeradores.Controls.Add(lblPreviewProv);
            tpNumeradores.Controls.Add(labelTituloFactVen);
            tpNumeradores.Controls.Add(labelFactVenPrefijo);
            tpNumeradores.Controls.Add(txtNumFactVenPrefijo);
            tpNumeradores.Controls.Add(labelFactVenLong);
            tpNumeradores.Controls.Add(txtNumFactVenLongitud);
            tpNumeradores.Controls.Add(labelFactVenActual);
            tpNumeradores.Controls.Add(txtNumFactVenActual);
            tpNumeradores.Controls.Add(lblPreviewFactVen);
            tpNumeradores.Controls.Add(labelTituloFactCom);
            tpNumeradores.Controls.Add(labelFactComPrefijo);
            tpNumeradores.Controls.Add(txtNumFactComPrefijo);
            tpNumeradores.Controls.Add(labelFactComLong);
            tpNumeradores.Controls.Add(txtNumFactComLongitud);
            tpNumeradores.Controls.Add(labelFactComActual);
            tpNumeradores.Controls.Add(txtNumFactComActual);
            tpNumeradores.Controls.Add(lblPreviewFactCom);
            tpNumeradores.Controls.Add(labelTituloNcVen);
            tpNumeradores.Controls.Add(labelNcVenPrefijo);
            tpNumeradores.Controls.Add(txtNumNcVenPrefijo);
            tpNumeradores.Controls.Add(labelNcVenLong);
            tpNumeradores.Controls.Add(txtNumNcVenLongitud);
            tpNumeradores.Controls.Add(labelNcVenActual);
            tpNumeradores.Controls.Add(txtNumNcVenActual);
            tpNumeradores.Controls.Add(lblPreviewNcVen);
            tpNumeradores.Location = new Point(4, 24);
            tpNumeradores.Name = "tpNumeradores";
            tpNumeradores.Padding = new Padding(8);
            tpNumeradores.Size = new Size(892, 632);
            tpNumeradores.TabIndex = 1;
            tpNumeradores.Text = "Numeradores";
            tpNumeradores.UseVisualStyleBackColor = true;
            // 
            // labelTituloProducto
            // 
            labelTituloProducto.Location = new Point(0, 0);
            labelTituloProducto.Name = "labelTituloProducto";
            labelTituloProducto.Size = new Size(100, 23);
            labelTituloProducto.TabIndex = 0;
            // 
            // labelProdPrefijo
            // 
            labelProdPrefijo.Location = new Point(0, 0);
            labelProdPrefijo.Name = "labelProdPrefijo";
            labelProdPrefijo.Size = new Size(100, 23);
            labelProdPrefijo.TabIndex = 1;
            // 
            // txtNumProdPrefijo
            // 
            txtNumProdPrefijo.Location = new Point(0, 0);
            txtNumProdPrefijo.Name = "txtNumProdPrefijo";
            txtNumProdPrefijo.Size = new Size(100, 23);
            txtNumProdPrefijo.TabIndex = 2;
            // 
            // labelProdLong
            // 
            labelProdLong.Location = new Point(0, 0);
            labelProdLong.Name = "labelProdLong";
            labelProdLong.Size = new Size(100, 23);
            labelProdLong.TabIndex = 3;
            // 
            // txtNumProdLongitud
            // 
            txtNumProdLongitud.Location = new Point(0, 0);
            txtNumProdLongitud.Name = "txtNumProdLongitud";
            txtNumProdLongitud.Size = new Size(100, 23);
            txtNumProdLongitud.TabIndex = 4;
            // 
            // labelProdActual
            // 
            labelProdActual.Location = new Point(0, 0);
            labelProdActual.Name = "labelProdActual";
            labelProdActual.Size = new Size(100, 23);
            labelProdActual.TabIndex = 5;
            // 
            // txtNumProdActual
            // 
            txtNumProdActual.Location = new Point(0, 0);
            txtNumProdActual.Name = "txtNumProdActual";
            txtNumProdActual.Size = new Size(100, 23);
            txtNumProdActual.TabIndex = 6;
            // 
            // lblPreviewProd
            // 
            lblPreviewProd.Location = new Point(0, 0);
            lblPreviewProd.Name = "lblPreviewProd";
            lblPreviewProd.Size = new Size(100, 23);
            lblPreviewProd.TabIndex = 7;
            // 
            // labelTituloCliente
            // 
            labelTituloCliente.Location = new Point(0, 0);
            labelTituloCliente.Name = "labelTituloCliente";
            labelTituloCliente.Size = new Size(100, 23);
            labelTituloCliente.TabIndex = 8;
            // 
            // labelCliPrefijo
            // 
            labelCliPrefijo.Location = new Point(0, 0);
            labelCliPrefijo.Name = "labelCliPrefijo";
            labelCliPrefijo.Size = new Size(100, 23);
            labelCliPrefijo.TabIndex = 9;
            // 
            // txtNumCliPrefijo
            // 
            txtNumCliPrefijo.Location = new Point(0, 0);
            txtNumCliPrefijo.Name = "txtNumCliPrefijo";
            txtNumCliPrefijo.Size = new Size(100, 23);
            txtNumCliPrefijo.TabIndex = 10;
            // 
            // labelCliLong
            // 
            labelCliLong.Location = new Point(0, 0);
            labelCliLong.Name = "labelCliLong";
            labelCliLong.Size = new Size(100, 23);
            labelCliLong.TabIndex = 11;
            // 
            // txtNumCliLongitud
            // 
            txtNumCliLongitud.Location = new Point(0, 0);
            txtNumCliLongitud.Name = "txtNumCliLongitud";
            txtNumCliLongitud.Size = new Size(100, 23);
            txtNumCliLongitud.TabIndex = 12;
            // 
            // labelCliActual
            // 
            labelCliActual.Location = new Point(0, 0);
            labelCliActual.Name = "labelCliActual";
            labelCliActual.Size = new Size(100, 23);
            labelCliActual.TabIndex = 13;
            // 
            // txtNumCliActual
            // 
            txtNumCliActual.Location = new Point(0, 0);
            txtNumCliActual.Name = "txtNumCliActual";
            txtNumCliActual.Size = new Size(100, 23);
            txtNumCliActual.TabIndex = 14;
            // 
            // lblPreviewCli
            // 
            lblPreviewCli.Location = new Point(0, 0);
            lblPreviewCli.Name = "lblPreviewCli";
            lblPreviewCli.Size = new Size(100, 23);
            lblPreviewCli.TabIndex = 15;
            // 
            // labelTituloProveedor
            // 
            labelTituloProveedor.Location = new Point(0, 0);
            labelTituloProveedor.Name = "labelTituloProveedor";
            labelTituloProveedor.Size = new Size(100, 23);
            labelTituloProveedor.TabIndex = 16;
            // 
            // labelProvPrefijo
            // 
            labelProvPrefijo.Location = new Point(0, 0);
            labelProvPrefijo.Name = "labelProvPrefijo";
            labelProvPrefijo.Size = new Size(100, 23);
            labelProvPrefijo.TabIndex = 17;
            // 
            // txtNumProvPrefijo
            // 
            txtNumProvPrefijo.Location = new Point(0, 0);
            txtNumProvPrefijo.Name = "txtNumProvPrefijo";
            txtNumProvPrefijo.Size = new Size(100, 23);
            txtNumProvPrefijo.TabIndex = 18;
            // 
            // labelProvLong
            // 
            labelProvLong.Location = new Point(0, 0);
            labelProvLong.Name = "labelProvLong";
            labelProvLong.Size = new Size(100, 23);
            labelProvLong.TabIndex = 19;
            // 
            // txtNumProvLongitud
            // 
            txtNumProvLongitud.Location = new Point(0, 0);
            txtNumProvLongitud.Name = "txtNumProvLongitud";
            txtNumProvLongitud.Size = new Size(100, 23);
            txtNumProvLongitud.TabIndex = 20;
            // 
            // labelProvActual
            // 
            labelProvActual.Location = new Point(0, 0);
            labelProvActual.Name = "labelProvActual";
            labelProvActual.Size = new Size(100, 23);
            labelProvActual.TabIndex = 21;
            // 
            // txtNumProvActual
            // 
            txtNumProvActual.Location = new Point(0, 0);
            txtNumProvActual.Name = "txtNumProvActual";
            txtNumProvActual.Size = new Size(100, 23);
            txtNumProvActual.TabIndex = 22;
            // 
            // lblPreviewProv
            // 
            lblPreviewProv.Location = new Point(0, 0);
            lblPreviewProv.Name = "lblPreviewProv";
            lblPreviewProv.Size = new Size(100, 23);
            lblPreviewProv.TabIndex = 23;
            // 
            // labelTituloFactVen
            // 
            labelTituloFactVen.Location = new Point(0, 0);
            labelTituloFactVen.Name = "labelTituloFactVen";
            labelTituloFactVen.Size = new Size(100, 23);
            labelTituloFactVen.TabIndex = 24;
            // 
            // labelFactVenPrefijo
            // 
            labelFactVenPrefijo.Location = new Point(0, 0);
            labelFactVenPrefijo.Name = "labelFactVenPrefijo";
            labelFactVenPrefijo.Size = new Size(100, 23);
            labelFactVenPrefijo.TabIndex = 25;
            // 
            // txtNumFactVenPrefijo
            // 
            txtNumFactVenPrefijo.Location = new Point(0, 0);
            txtNumFactVenPrefijo.Name = "txtNumFactVenPrefijo";
            txtNumFactVenPrefijo.Size = new Size(100, 23);
            txtNumFactVenPrefijo.TabIndex = 26;
            // 
            // labelFactVenLong
            // 
            labelFactVenLong.Location = new Point(0, 0);
            labelFactVenLong.Name = "labelFactVenLong";
            labelFactVenLong.Size = new Size(100, 23);
            labelFactVenLong.TabIndex = 27;
            // 
            // txtNumFactVenLongitud
            // 
            txtNumFactVenLongitud.Location = new Point(0, 0);
            txtNumFactVenLongitud.Name = "txtNumFactVenLongitud";
            txtNumFactVenLongitud.Size = new Size(100, 23);
            txtNumFactVenLongitud.TabIndex = 28;
            // 
            // labelFactVenActual
            // 
            labelFactVenActual.Location = new Point(0, 0);
            labelFactVenActual.Name = "labelFactVenActual";
            labelFactVenActual.Size = new Size(100, 23);
            labelFactVenActual.TabIndex = 29;
            // 
            // txtNumFactVenActual
            // 
            txtNumFactVenActual.Location = new Point(0, 0);
            txtNumFactVenActual.Name = "txtNumFactVenActual";
            txtNumFactVenActual.Size = new Size(100, 23);
            txtNumFactVenActual.TabIndex = 30;
            // 
            // lblPreviewFactVen
            // 
            lblPreviewFactVen.Location = new Point(0, 0);
            lblPreviewFactVen.Name = "lblPreviewFactVen";
            lblPreviewFactVen.Size = new Size(100, 23);
            lblPreviewFactVen.TabIndex = 31;
            // 
            // labelTituloFactCom
            // 
            labelTituloFactCom.Location = new Point(0, 0);
            labelTituloFactCom.Name = "labelTituloFactCom";
            labelTituloFactCom.Size = new Size(100, 23);
            labelTituloFactCom.TabIndex = 32;
            // 
            // labelFactComPrefijo
            // 
            labelFactComPrefijo.Location = new Point(0, 0);
            labelFactComPrefijo.Name = "labelFactComPrefijo";
            labelFactComPrefijo.Size = new Size(100, 23);
            labelFactComPrefijo.TabIndex = 33;
            // 
            // txtNumFactComPrefijo
            // 
            txtNumFactComPrefijo.Location = new Point(0, 0);
            txtNumFactComPrefijo.Name = "txtNumFactComPrefijo";
            txtNumFactComPrefijo.Size = new Size(100, 23);
            txtNumFactComPrefijo.TabIndex = 34;
            // 
            // labelFactComLong
            // 
            labelFactComLong.Location = new Point(0, 0);
            labelFactComLong.Name = "labelFactComLong";
            labelFactComLong.Size = new Size(100, 23);
            labelFactComLong.TabIndex = 35;
            // 
            // txtNumFactComLongitud
            // 
            txtNumFactComLongitud.Location = new Point(0, 0);
            txtNumFactComLongitud.Name = "txtNumFactComLongitud";
            txtNumFactComLongitud.Size = new Size(100, 23);
            txtNumFactComLongitud.TabIndex = 36;
            // 
            // labelFactComActual
            // 
            labelFactComActual.Location = new Point(0, 0);
            labelFactComActual.Name = "labelFactComActual";
            labelFactComActual.Size = new Size(100, 23);
            labelFactComActual.TabIndex = 37;
            // 
            // txtNumFactComActual
            // 
            txtNumFactComActual.Location = new Point(0, 0);
            txtNumFactComActual.Name = "txtNumFactComActual";
            txtNumFactComActual.Size = new Size(100, 23);
            txtNumFactComActual.TabIndex = 38;
            // 
            // lblPreviewFactCom
            // 
            lblPreviewFactCom.Location = new Point(0, 0);
            lblPreviewFactCom.Name = "lblPreviewFactCom";
            lblPreviewFactCom.Size = new Size(100, 23);
            lblPreviewFactCom.TabIndex = 39;
            // 
            // labelTituloNcVen
            // 
            labelTituloNcVen.Location = new Point(0, 0);
            labelTituloNcVen.Name = "labelTituloNcVen";
            labelTituloNcVen.Size = new Size(100, 23);
            labelTituloNcVen.TabIndex = 40;
            // 
            // labelNcVenPrefijo
            // 
            labelNcVenPrefijo.Location = new Point(0, 0);
            labelNcVenPrefijo.Name = "labelNcVenPrefijo";
            labelNcVenPrefijo.Size = new Size(100, 23);
            labelNcVenPrefijo.TabIndex = 41;
            // 
            // txtNumNcVenPrefijo
            // 
            txtNumNcVenPrefijo.Location = new Point(0, 0);
            txtNumNcVenPrefijo.Name = "txtNumNcVenPrefijo";
            txtNumNcVenPrefijo.Size = new Size(100, 23);
            txtNumNcVenPrefijo.TabIndex = 42;
            // 
            // labelNcVenLong
            // 
            labelNcVenLong.Location = new Point(0, 0);
            labelNcVenLong.Name = "labelNcVenLong";
            labelNcVenLong.Size = new Size(100, 23);
            labelNcVenLong.TabIndex = 43;
            // 
            // txtNumNcVenLongitud
            // 
            txtNumNcVenLongitud.Location = new Point(0, 0);
            txtNumNcVenLongitud.Name = "txtNumNcVenLongitud";
            txtNumNcVenLongitud.Size = new Size(100, 23);
            txtNumNcVenLongitud.TabIndex = 44;
            // 
            // labelNcVenActual
            // 
            labelNcVenActual.Location = new Point(0, 0);
            labelNcVenActual.Name = "labelNcVenActual";
            labelNcVenActual.Size = new Size(100, 23);
            labelNcVenActual.TabIndex = 45;
            // 
            // txtNumNcVenActual
            // 
            txtNumNcVenActual.Location = new Point(0, 0);
            txtNumNcVenActual.Name = "txtNumNcVenActual";
            txtNumNcVenActual.Size = new Size(100, 23);
            txtNumNcVenActual.TabIndex = 46;
            // 
            // lblPreviewNcVen
            // 
            lblPreviewNcVen.Location = new Point(0, 0);
            lblPreviewNcVen.Name = "lblPreviewNcVen";
            lblPreviewNcVen.Size = new Size(100, 23);
            lblPreviewNcVen.TabIndex = 47;
            // 
            // tpReportes
            // 
            tpReportes.Location = new Point(4, 24);
            tpReportes.Name = "tpReportes";
            tpReportes.Padding = new Padding(8);
            tpReportes.Size = new Size(892, 632);
            tpReportes.TabIndex = 2;
            tpReportes.Text = "Reportes";
            tpReportes.UseVisualStyleBackColor = true;
            // 
            // btnGuardar
            // 
            btnGuardar.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnGuardar.Location = new Point(716, 684);
            btnGuardar.Name = "btnGuardar";
            btnGuardar.Size = new Size(90, 30);
            btnGuardar.TabIndex = 1;
            btnGuardar.Text = "Guardar";
            // 
            // btnCerrar
            // 
            btnCerrar.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnCerrar.Location = new Point(814, 684);
            btnCerrar.Name = "btnCerrar";
            btnCerrar.Size = new Size(90, 30);
            btnCerrar.TabIndex = 2;
            btnCerrar.Text = "Cerrar";
            // 
            // BtnPostulacion
            // 
            BtnPostulacion.Location = new Point(591, 411);
            BtnPostulacion.Name = "BtnPostulacion";
            BtnPostulacion.Size = new Size(119, 41);
            BtnPostulacion.TabIndex = 19;
            BtnPostulacion.Text = "Postulacion DGGI";
            BtnPostulacion.UseVisualStyleBackColor = true;
            BtnPostulacion.Click += BtnPostulacion_Click;
            // 
            // FormConfiguracion
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(924, 726);
            Controls.Add(tabMain);
            Controls.Add(btnGuardar);
            Controls.Add(btnCerrar);
            Name = "FormConfiguracion";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Configuración del Sistema";
            tabMain.ResumeLayout(false);
            tpGeneral.ResumeLayout(false);
            tpGeneral.PerformLayout();
            tpGeneral.Controls.Add(lblAlanubeTitulo);
            tpGeneral.Controls.Add(lblAlanubeBaseUrl);
            tpGeneral.Controls.Add(txtAlanubeBaseUrl);
            tpGeneral.Controls.Add(lblAlanubeToken);
            tpGeneral.Controls.Add(txtAlanubeToken);
            tpGeneral.Controls.Add(lblAlanubeAmbiente);
            tpGeneral.Controls.Add(cbAlanubeAmbiente);
            tpGeneral.Controls.Add(lblAlanubeTimeout);
            tpGeneral.Controls.Add(txtAlanubeTimeout);
            tpGeneral.Controls.Add(lblAlanubeIdCompany);
            tpGeneral.Controls.Add(txtAlanubeIdCompany);
            tpGeneral.Controls.Add(lblAlanubeRetryNumber);
            tpGeneral.Controls.Add(txtAlanubeRetryNumber);
            tpGeneral.Controls.Add(btnProbarAlanube);
            tpNumeradores.ResumeLayout(false);
            tpNumeradores.PerformLayout();
            ResumeLayout(false);
        }

        private void SetupBlock(
            Label lblTitulo,
            Label lblPrefijo,
            TextBox txtPrefijo,
            Label lblLong,
            TextBox txtLong,
            Label lblActual,
            TextBox txtActual,
            Label lblPreview,
            string titulo,
            string preview,
            int left1,
            int left2,
            int top)
        {
            lblTitulo.AutoSize = true;
            lblTitulo.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblTitulo.Location = new Point(left1, top);
            lblTitulo.Text = titulo;

            lblPrefijo.AutoSize = true;
            lblPrefijo.Location = new Point(left2, top + 26);
            lblPrefijo.Text = "Prefijo:";

            txtPrefijo.Location = new Point(90, top + 22);
            txtPrefijo.Size = new Size(80, 23);

            lblLong.AutoSize = true;
            lblLong.Location = new Point(190, top + 26);
            lblLong.Text = "Longitud:";

            txtLong.Location = new Point(255, top + 22);
            txtLong.Size = new Size(50, 23);

            lblActual.AutoSize = true;
            lblActual.Location = new Point(325, top + 26);
            lblActual.Text = "Actual:";

            txtActual.Location = new Point(376, top + 22);
            txtActual.Size = new Size(60, 23);

            lblPreview.AutoSize = true;
            lblPreview.Location = new Point(460, top + 26);
            lblPreview.Text = preview;
        }
        private Button BtnPostulacion;
    }
}
#nullable restore