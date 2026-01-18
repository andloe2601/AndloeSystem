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

        // General
        private Label labelMonedaBase;
        private ComboBox cbMonedaBase;
        private Label labelMedioPagoDefecto;
        private ComboBox cbMedioPagoDefecto;
        private Label labelClienteDefecto;
        private TextBox txtClienteDefecto;
        private Label labelStockNegativo;
        private CheckBox chkPermitirStockNegativo;

        // Contexto
        private Label labelEmpresa;
        private ComboBox cbEmpresa;
        private Label labelSucursal;
        private ComboBox cbSucursal;
        private Label labelAlmacenDefecto;
        private ComboBox cbAlmacenDefecto;

        // POS almacenes
        private Label labelAlmacenOrigenPos;
        private ComboBox cbAlmacenOrigenPos;
        private Label labelAlmacenDestinoPos;
        private ComboBox cbAlmacenDestinoPos;

        private Button btnActualizarDgii;

        // Numeradores
        private Label labelTituloProducto;
        private Label labelProdPrefijo;
        private TextBox txtNumProdPrefijo;
        private Label labelProdLong;
        private TextBox txtNumProdLongitud;
        private Label lblPreviewProd;

        private Label labelTituloCliente;
        private Label labelCliPrefijo;
        private TextBox txtNumCliPrefijo;
        private Label labelCliLong;
        private TextBox txtNumCliLongitud;
        private Label lblPreviewCli;

        private Label labelTituloProveedor;
        private Label labelProvPrefijo;
        private TextBox txtNumProvPrefijo;
        private Label labelProvLong;
        private TextBox txtNumProvLongitud;
        private Label lblPreviewProv;

        private Label labelTituloFactVen;
        private Label labelFactVenPrefijo;
        private TextBox txtNumFactVenPrefijo;
        private Label labelFactVenLong;
        private TextBox txtNumFactVenLongitud;
        private Label lblPreviewFactVen;

        private Label labelTituloFactCom;
        private Label labelFactComPrefijo;
        private TextBox txtNumFactComPrefijo;
        private Label labelFactComLong;
        private TextBox txtNumFactComLongitud;
        private Label lblPreviewFactCom;

        private Button btnGuardar;
        private Button btnCerrar;

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
            tpNumeradores = new TabPage();
            tpReportes = new TabPage();

            // General
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

            // Numeradores
            labelTituloProducto = new Label();
            labelProdPrefijo = new Label();
            txtNumProdPrefijo = new TextBox();
            labelProdLong = new Label();
            txtNumProdLongitud = new TextBox();
            lblPreviewProd = new Label();

            labelTituloCliente = new Label();
            labelCliPrefijo = new Label();
            txtNumCliPrefijo = new TextBox();
            labelCliLong = new Label();
            txtNumCliLongitud = new TextBox();
            lblPreviewCli = new Label();

            labelTituloProveedor = new Label();
            labelProvPrefijo = new Label();
            txtNumProvPrefijo = new TextBox();
            labelProvLong = new Label();
            txtNumProvLongitud = new TextBox();
            lblPreviewProv = new Label();

            labelTituloFactVen = new Label();
            labelFactVenPrefijo = new Label();
            txtNumFactVenPrefijo = new TextBox();
            labelFactVenLong = new Label();
            txtNumFactVenLongitud = new TextBox();
            lblPreviewFactVen = new Label();

            labelTituloFactCom = new Label();
            labelFactComPrefijo = new Label();
            txtNumFactComPrefijo = new TextBox();
            labelFactComLong = new Label();
            txtNumFactComLongitud = new TextBox();
            lblPreviewFactCom = new Label();

            btnGuardar = new Button();
            btnCerrar = new Button();

            tabMain.SuspendLayout();
            tpGeneral.SuspendLayout();
            tpNumeradores.SuspendLayout();
            SuspendLayout();

            // 
            // tabMain
            // 
            tabMain.Controls.Add(tpGeneral);
            tabMain.Controls.Add(tpNumeradores);
            tabMain.Controls.Add(tpReportes);
            tabMain.Location = new Point(12, 12);
            tabMain.Name = "tabMain";
            tabMain.SelectedIndex = 0;
            tabMain.Size = new Size(734, 540);
            tabMain.TabIndex = 0;
            tabMain.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            // 
            // tpGeneral
            // 
            tpGeneral.Name = "tpGeneral";
            tpGeneral.Text = "General";
            tpGeneral.Padding = new Padding(8);
            tpGeneral.UseVisualStyleBackColor = true;

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

            // 
            // labelMonedaBase
            // 
            labelMonedaBase.AutoSize = true;
            labelMonedaBase.Location = new Point(24, 26);
            labelMonedaBase.Name = "labelMonedaBase";
            labelMonedaBase.Size = new Size(81, 15);
            labelMonedaBase.Text = "Moneda base:";

            // 
            // cbMonedaBase
            // 
            cbMonedaBase.DropDownStyle = ComboBoxStyle.DropDownList;
            cbMonedaBase.Location = new Point(190, 22);
            cbMonedaBase.Name = "cbMonedaBase";
            cbMonedaBase.Size = new Size(260, 23);

            // 
            // labelMedioPagoDefecto
            // 
            labelMedioPagoDefecto.AutoSize = true;
            labelMedioPagoDefecto.Location = new Point(24, 66);
            labelMedioPagoDefecto.Name = "labelMedioPagoDefecto";
            labelMedioPagoDefecto.Size = new Size(133, 15);
            labelMedioPagoDefecto.Text = "Medio de pago defecto:";

            // 
            // cbMedioPagoDefecto
            // 
            cbMedioPagoDefecto.DropDownStyle = ComboBoxStyle.DropDownList;
            cbMedioPagoDefecto.Location = new Point(190, 62);
            cbMedioPagoDefecto.Name = "cbMedioPagoDefecto";
            cbMedioPagoDefecto.Size = new Size(260, 23);

            // 
            // labelClienteDefecto
            // 
            labelClienteDefecto.AutoSize = true;
            labelClienteDefecto.Location = new Point(24, 106);
            labelClienteDefecto.Name = "labelClienteDefecto";
            labelClienteDefecto.Size = new Size(90, 15);
            labelClienteDefecto.Text = "Cliente defecto:";

            // 
            // txtClienteDefecto
            // 
            txtClienteDefecto.Location = new Point(190, 102);
            txtClienteDefecto.Name = "txtClienteDefecto";
            txtClienteDefecto.Size = new Size(180, 23);

            // 
            // labelStockNegativo
            // 
            labelStockNegativo.AutoSize = true;
            labelStockNegativo.Location = new Point(24, 146);
            labelStockNegativo.Name = "labelStockNegativo";
            labelStockNegativo.Size = new Size(132, 15);
            labelStockNegativo.Text = "Permitir stock negativo:";

            // 
            // chkPermitirStockNegativo
            // 
            chkPermitirStockNegativo.AutoSize = true;
            chkPermitirStockNegativo.Location = new Point(190, 146);
            chkPermitirStockNegativo.Name = "chkPermitirStockNegativo";
            chkPermitirStockNegativo.Size = new Size(15, 14);

            // 
            // labelEmpresa
            // 
            labelEmpresa.AutoSize = true;
            labelEmpresa.Location = new Point(24, 186);
            labelEmpresa.Name = "labelEmpresa";
            labelEmpresa.Size = new Size(55, 15);
            labelEmpresa.Text = "Empresa:";

            // 
            // cbEmpresa
            // 
            cbEmpresa.DropDownStyle = ComboBoxStyle.DropDownList;
            cbEmpresa.Location = new Point(190, 182);
            cbEmpresa.Name = "cbEmpresa";
            cbEmpresa.Size = new Size(380, 23);

            // 
            // labelSucursal
            // 
            labelSucursal.AutoSize = true;
            labelSucursal.Location = new Point(24, 226);
            labelSucursal.Name = "labelSucursal";
            labelSucursal.Size = new Size(54, 15);
            labelSucursal.Text = "Sucursal:";

            // 
            // cbSucursal
            // 
            cbSucursal.DropDownStyle = ComboBoxStyle.DropDownList;
            cbSucursal.Location = new Point(190, 222);
            cbSucursal.Name = "cbSucursal";
            cbSucursal.Size = new Size(380, 23);

            // 
            // labelAlmacenDefecto
            // 
            labelAlmacenDefecto.AutoSize = true;
            labelAlmacenDefecto.Location = new Point(24, 266);
            labelAlmacenDefecto.Name = "labelAlmacenDefecto";
            labelAlmacenDefecto.Size = new Size(121, 15);
            labelAlmacenDefecto.Text = "Almacén por defecto:";

            // 
            // cbAlmacenDefecto
            // 
            cbAlmacenDefecto.DropDownStyle = ComboBoxStyle.DropDownList;
            cbAlmacenDefecto.Location = new Point(190, 262);
            cbAlmacenDefecto.Name = "cbAlmacenDefecto";
            cbAlmacenDefecto.Size = new Size(380, 23);

            // 
            // labelAlmacenOrigenPos
            // 
            labelAlmacenOrigenPos.AutoSize = true;
            labelAlmacenOrigenPos.Location = new Point(24, 306);
            labelAlmacenOrigenPos.Name = "labelAlmacenOrigenPos";
            labelAlmacenOrigenPos.Size = new Size(127, 15);
            labelAlmacenOrigenPos.Text = "Almacén POS (origen):";

            // 
            // cbAlmacenOrigenPos
            // 
            cbAlmacenOrigenPos.DropDownStyle = ComboBoxStyle.DropDownList;
            cbAlmacenOrigenPos.Location = new Point(190, 302);
            cbAlmacenOrigenPos.Name = "cbAlmacenOrigenPos";
            cbAlmacenOrigenPos.Size = new Size(380, 23);

            // 
            // labelAlmacenDestinoPos
            // 
            labelAlmacenDestinoPos.AutoSize = true;
            labelAlmacenDestinoPos.Location = new Point(24, 336);
            labelAlmacenDestinoPos.Name = "labelAlmacenDestinoPos";
            labelAlmacenDestinoPos.Size = new Size(132, 15);
            labelAlmacenDestinoPos.Text = "Almacén POS (destino):";

            // 
            // cbAlmacenDestinoPos
            // 
            cbAlmacenDestinoPos.DropDownStyle = ComboBoxStyle.DropDownList;
            cbAlmacenDestinoPos.Location = new Point(190, 332);
            cbAlmacenDestinoPos.Name = "cbAlmacenDestinoPos";
            cbAlmacenDestinoPos.Size = new Size(380, 23);

            // 
            // btnActualizarDgii
            // 
            btnActualizarDgii.Location = new Point(590, 302);
            btnActualizarDgii.Name = "btnActualizarDgii";
            btnActualizarDgii.Size = new Size(120, 53);
            btnActualizarDgii.Text = "Actualizar DGII";
            btnActualizarDgii.UseVisualStyleBackColor = true;

            // 
            // tpNumeradores
            // 
            tpNumeradores.Name = "tpNumeradores";
            tpNumeradores.Text = "Numeradores";
            tpNumeradores.Padding = new Padding(8);
            tpNumeradores.UseVisualStyleBackColor = true;

            tpNumeradores.Controls.Add(labelTituloProducto);
            tpNumeradores.Controls.Add(labelProdPrefijo);
            tpNumeradores.Controls.Add(txtNumProdPrefijo);
            tpNumeradores.Controls.Add(labelProdLong);
            tpNumeradores.Controls.Add(txtNumProdLongitud);
            tpNumeradores.Controls.Add(lblPreviewProd);

            tpNumeradores.Controls.Add(labelTituloCliente);
            tpNumeradores.Controls.Add(labelCliPrefijo);
            tpNumeradores.Controls.Add(txtNumCliPrefijo);
            tpNumeradores.Controls.Add(labelCliLong);
            tpNumeradores.Controls.Add(txtNumCliLongitud);
            tpNumeradores.Controls.Add(lblPreviewCli);

            tpNumeradores.Controls.Add(labelTituloProveedor);
            tpNumeradores.Controls.Add(labelProvPrefijo);
            tpNumeradores.Controls.Add(txtNumProvPrefijo);
            tpNumeradores.Controls.Add(labelProvLong);
            tpNumeradores.Controls.Add(txtNumProvLongitud);
            tpNumeradores.Controls.Add(lblPreviewProv);

            tpNumeradores.Controls.Add(labelTituloFactVen);
            tpNumeradores.Controls.Add(labelFactVenPrefijo);
            tpNumeradores.Controls.Add(txtNumFactVenPrefijo);
            tpNumeradores.Controls.Add(labelFactVenLong);
            tpNumeradores.Controls.Add(txtNumFactVenLongitud);
            tpNumeradores.Controls.Add(lblPreviewFactVen);

            tpNumeradores.Controls.Add(labelTituloFactCom);
            tpNumeradores.Controls.Add(labelFactComPrefijo);
            tpNumeradores.Controls.Add(txtNumFactComPrefijo);
            tpNumeradores.Controls.Add(labelFactComLong);
            tpNumeradores.Controls.Add(txtNumFactComLongitud);
            tpNumeradores.Controls.Add(lblPreviewFactCom);

            // -- Numeradores layout (igual a tu versión) --
            labelTituloProducto.AutoSize = true;
            labelTituloProducto.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            labelTituloProducto.Location = new Point(16, 16);
            labelTituloProducto.Name = "labelTituloProducto";
            labelTituloProducto.Text = "PRODUCTO";

            labelProdPrefijo.AutoSize = true;
            labelProdPrefijo.Location = new Point(32, 42);
            labelProdPrefijo.Name = "labelProdPrefijo";
            labelProdPrefijo.Text = "Prefijo:";

            txtNumProdPrefijo.Location = new Point(90, 38);
            txtNumProdPrefijo.Name = "txtNumProdPrefijo";
            txtNumProdPrefijo.Size = new Size(80, 23);

            labelProdLong.AutoSize = true;
            labelProdLong.Location = new Point(190, 42);
            labelProdLong.Name = "labelProdLong";
            labelProdLong.Text = "Longitud:";

            txtNumProdLongitud.Location = new Point(255, 38);
            txtNumProdLongitud.Name = "txtNumProdLongitud";
            txtNumProdLongitud.Size = new Size(50, 23);

            lblPreviewProd.AutoSize = true;
            lblPreviewProd.Location = new Point(330, 42);
            lblPreviewProd.Name = "lblPreviewProd";
            lblPreviewProd.Text = "Ej: P-000001";

            labelTituloCliente.AutoSize = true;
            labelTituloCliente.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            labelTituloCliente.Location = new Point(16, 80);
            labelTituloCliente.Name = "labelTituloCliente";
            labelTituloCliente.Text = "CLIENTE";

            labelCliPrefijo.AutoSize = true;
            labelCliPrefijo.Location = new Point(32, 106);
            labelCliPrefijo.Name = "labelCliPrefijo";
            labelCliPrefijo.Text = "Prefijo:";

            txtNumCliPrefijo.Location = new Point(90, 102);
            txtNumCliPrefijo.Name = "txtNumCliPrefijo";
            txtNumCliPrefijo.Size = new Size(80, 23);

            labelCliLong.AutoSize = true;
            labelCliLong.Location = new Point(190, 106);
            labelCliLong.Name = "labelCliLong";
            labelCliLong.Text = "Longitud:";

            txtNumCliLongitud.Location = new Point(255, 102);
            txtNumCliLongitud.Name = "txtNumCliLongitud";
            txtNumCliLongitud.Size = new Size(50, 23);

            lblPreviewCli.AutoSize = true;
            lblPreviewCli.Location = new Point(330, 106);
            lblPreviewCli.Name = "lblPreviewCli";
            lblPreviewCli.Text = "Ej: C-000001";

            labelTituloProveedor.AutoSize = true;
            labelTituloProveedor.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            labelTituloProveedor.Location = new Point(16, 144);
            labelTituloProveedor.Name = "labelTituloProveedor";
            labelTituloProveedor.Text = "PROVEEDOR";

            labelProvPrefijo.AutoSize = true;
            labelProvPrefijo.Location = new Point(32, 170);
            labelProvPrefijo.Name = "labelProvPrefijo";
            labelProvPrefijo.Text = "Prefijo:";

            txtNumProvPrefijo.Location = new Point(90, 166);
            txtNumProvPrefijo.Name = "txtNumProvPrefijo";
            txtNumProvPrefijo.Size = new Size(80, 23);

            labelProvLong.AutoSize = true;
            labelProvLong.Location = new Point(190, 170);
            labelProvLong.Name = "labelProvLong";
            labelProvLong.Text = "Longitud:";

            txtNumProvLongitud.Location = new Point(255, 166);
            txtNumProvLongitud.Name = "txtNumProvLongitud";
            txtNumProvLongitud.Size = new Size(50, 23);

            lblPreviewProv.AutoSize = true;
            lblPreviewProv.Location = new Point(330, 170);
            lblPreviewProv.Name = "lblPreviewProv";
            lblPreviewProv.Text = "Ej: PR-000001";

            labelTituloFactVen.AutoSize = true;
            labelTituloFactVen.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            labelTituloFactVen.Location = new Point(16, 208);
            labelTituloFactVen.Name = "labelTituloFactVen";
            labelTituloFactVen.Text = "FACTURA VENTA";

            labelFactVenPrefijo.AutoSize = true;
            labelFactVenPrefijo.Location = new Point(32, 234);
            labelFactVenPrefijo.Name = "labelFactVenPrefijo";
            labelFactVenPrefijo.Text = "Prefijo:";

            txtNumFactVenPrefijo.Location = new Point(90, 230);
            txtNumFactVenPrefijo.Name = "txtNumFactVenPrefijo";
            txtNumFactVenPrefijo.Size = new Size(80, 23);

            labelFactVenLong.AutoSize = true;
            labelFactVenLong.Location = new Point(190, 234);
            labelFactVenLong.Name = "labelFactVenLong";
            labelFactVenLong.Text = "Longitud:";

            txtNumFactVenLongitud.Location = new Point(255, 230);
            txtNumFactVenLongitud.Name = "txtNumFactVenLongitud";
            txtNumFactVenLongitud.Size = new Size(50, 23);

            lblPreviewFactVen.AutoSize = true;
            lblPreviewFactVen.Location = new Point(330, 234);
            lblPreviewFactVen.Name = "lblPreviewFactVen";
            lblPreviewFactVen.Text = "Ej: V00000001";

            labelTituloFactCom.AutoSize = true;
            labelTituloFactCom.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            labelTituloFactCom.Location = new Point(16, 268);
            labelTituloFactCom.Name = "labelTituloFactCom";
            labelTituloFactCom.Text = "FACTURA COMPRA";

            labelFactComPrefijo.AutoSize = true;
            labelFactComPrefijo.Location = new Point(32, 294);
            labelFactComPrefijo.Name = "labelFactComPrefijo";
            labelFactComPrefijo.Text = "Prefijo:";

            txtNumFactComPrefijo.Location = new Point(90, 290);
            txtNumFactComPrefijo.Name = "txtNumFactComPrefijo";
            txtNumFactComPrefijo.Size = new Size(80, 23);

            labelFactComLong.AutoSize = true;
            labelFactComLong.Location = new Point(190, 294);
            labelFactComLong.Name = "labelFactComLong";
            labelFactComLong.Text = "Longitud:";

            txtNumFactComLongitud.Location = new Point(255, 290);
            txtNumFactComLongitud.Name = "txtNumFactComLongitud";
            txtNumFactComLongitud.Size = new Size(50, 23);

            lblPreviewFactCom.AutoSize = true;
            lblPreviewFactCom.Location = new Point(330, 294);
            lblPreviewFactCom.Name = "lblPreviewFactCom";
            lblPreviewFactCom.Text = "Ej: FC-00000001";

            // 
            // tpReportes
            // 
            tpReportes.Name = "tpReportes";
            tpReportes.Text = "Reportes";
            tpReportes.Padding = new Padding(8);
            tpReportes.UseVisualStyleBackColor = true;

            // 
            // btnGuardar
            // 
            btnGuardar.Location = new Point(552, 560);
            btnGuardar.Name = "btnGuardar";
            btnGuardar.Size = new Size(95, 32);
            btnGuardar.TabIndex = 1;
            btnGuardar.Text = "Guardar";
            btnGuardar.UseVisualStyleBackColor = true;
            btnGuardar.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            // 
            // btnCerrar
            // 
            btnCerrar.Location = new Point(653, 560);
            btnCerrar.Name = "btnCerrar";
            btnCerrar.Size = new Size(95, 32);
            btnCerrar.TabIndex = 2;
            btnCerrar.Text = "Cerrar";
            btnCerrar.UseVisualStyleBackColor = true;
            btnCerrar.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            // 
            // FormConfiguracion
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(760, 604);
            Controls.Add(tabMain);
            Controls.Add(btnGuardar);
            Controls.Add(btnCerrar);
            Name = "FormConfiguracion";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Configuración del Sistema";

            tabMain.ResumeLayout(false);
            tpGeneral.ResumeLayout(false);
            tpGeneral.PerformLayout();
            tpNumeradores.ResumeLayout(false);
            tpNumeradores.PerformLayout();
            ResumeLayout(false);
        }
    }
}
#nullable restore
