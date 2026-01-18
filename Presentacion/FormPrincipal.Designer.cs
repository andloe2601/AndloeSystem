using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace Andloe.Presentacion
{
    partial class FormPrincipal
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code
        private void InitializeComponent()
        {
            panelSidebar = new Panel();
            btnSalir = new Button();
            pnlConfiguracion = new Panel();
            btnConexion = new Button();
            btnConfigSistema = new Button();
            btnUsuarios = new Button();
            btnGrpConfiguracion = new Button();
            pnlNomina = new Panel();
            btnNomina = new Button();
            btnGrpNomina = new Button();
            pnlInventario = new Panel();
            btnInvMov = new Button();
            btnKardex = new Button();
            btnGrpInventario = new Button();
            pnlProducto = new Panel();
            btnPromosProducto = new Button();
            btnProductos = new Button();
            btnGrpProducto = new Button();
            pnlCompra = new Panel();
            btnCompra = new Button();
            btnGrpCompra = new Button();
            pnlVenta = new Panel();
            btnCierresHistorico = new Button();
            btnCierreCaja = new Button();
            menuFacturacion = new Button();
            btnClientes = new Button();
            btnPOS = new Button();
            btnGrpVenta = new Button();
            pnlContabilidad = new Panel();
            btnContabilidad = new Button();
            btnGrpContabilidad = new Button();
            pnlDashboard = new Panel();
            btnDashboard = new Button();
            btnGrpDashboard = new Button();
            lblBrand = new Label();
            panelTop = new Panel();
            lblEmpresaConectada = new Label();
            lblTitle = new Label();
            panelContent = new Panel();
            panelSidebar.SuspendLayout();
            pnlConfiguracion.SuspendLayout();
            pnlNomina.SuspendLayout();
            pnlInventario.SuspendLayout();
            pnlProducto.SuspendLayout();
            pnlCompra.SuspendLayout();
            pnlVenta.SuspendLayout();
            pnlContabilidad.SuspendLayout();
            pnlDashboard.SuspendLayout();
            panelTop.SuspendLayout();
            SuspendLayout();
            // 
            // panelSidebar
            // 
            panelSidebar.AutoScroll = true;
            panelSidebar.Controls.Add(btnSalir);
            panelSidebar.Controls.Add(pnlConfiguracion);
            panelSidebar.Controls.Add(btnGrpConfiguracion);
            panelSidebar.Controls.Add(pnlNomina);
            panelSidebar.Controls.Add(btnGrpNomina);
            panelSidebar.Controls.Add(pnlInventario);
            panelSidebar.Controls.Add(btnGrpInventario);
            panelSidebar.Controls.Add(pnlProducto);
            panelSidebar.Controls.Add(btnGrpProducto);
            panelSidebar.Controls.Add(pnlCompra);
            panelSidebar.Controls.Add(btnGrpCompra);
            panelSidebar.Controls.Add(pnlVenta);
            panelSidebar.Controls.Add(btnGrpVenta);
            panelSidebar.Controls.Add(pnlContabilidad);
            panelSidebar.Controls.Add(btnGrpContabilidad);
            panelSidebar.Controls.Add(pnlDashboard);
            panelSidebar.Controls.Add(btnGrpDashboard);
            panelSidebar.Controls.Add(lblBrand);
            panelSidebar.Dock = DockStyle.Left;
            panelSidebar.Location = new Point(0, 0);
            panelSidebar.Name = "panelSidebar";
            panelSidebar.Padding = new Padding(12, 16, 12, 12);
            panelSidebar.Size = new Size(260, 720);
            panelSidebar.TabIndex = 0;
            // 
            // btnSalir
            // 
            btnSalir.Dock = DockStyle.Bottom;
            btnSalir.Location = new Point(12, 990);
            btnSalir.Name = "btnSalir";
            btnSalir.Size = new Size(219, 40);
            btnSalir.TabIndex = 99;
            btnSalir.Text = "Salir";
            btnSalir.UseVisualStyleBackColor = true;
            btnSalir.Click += btnSalir_Click;
            // 
            // pnlConfiguracion
            // 
            pnlConfiguracion.Controls.Add(btnConexion);
            pnlConfiguracion.Controls.Add(btnConfigSistema);
            pnlConfiguracion.Controls.Add(btnUsuarios);
            pnlConfiguracion.Dock = DockStyle.Top;
            pnlConfiguracion.Location = new Point(12, 890);
            pnlConfiguracion.Name = "pnlConfiguracion";
            pnlConfiguracion.Size = new Size(219, 112);
            pnlConfiguracion.TabIndex = 16;
            pnlConfiguracion.Visible = false;
            // 
            // btnConexion
            // 
            btnConexion.Dock = DockStyle.Top;
            btnConexion.Location = new Point(0, 72);
            btnConexion.Name = "btnConexion";
            btnConexion.Size = new Size(219, 36);
            btnConexion.TabIndex = 2;
            btnConexion.Text = "Conexión";
            btnConexion.UseVisualStyleBackColor = true;
            btnConexion.Click += btnConexion_Click;
            // 
            // btnConfigSistema
            // 
            btnConfigSistema.Dock = DockStyle.Top;
            btnConfigSistema.Location = new Point(0, 36);
            btnConfigSistema.Name = "btnConfigSistema";
            btnConfigSistema.Size = new Size(219, 36);
            btnConfigSistema.TabIndex = 1;
            btnConfigSistema.Text = "Sistema";
            btnConfigSistema.UseVisualStyleBackColor = true;
            btnConfigSistema.Click += btnConfigSistema_Click;
            // 
            // btnUsuarios
            // 
            btnUsuarios.Dock = DockStyle.Top;
            btnUsuarios.Location = new Point(0, 0);
            btnUsuarios.Name = "btnUsuarios";
            btnUsuarios.Size = new Size(219, 36);
            btnUsuarios.TabIndex = 0;
            btnUsuarios.Text = "Usuarios";
            btnUsuarios.UseVisualStyleBackColor = true;
            btnUsuarios.Click += btnUsuarios_Click;
            // 
            // btnGrpConfiguracion
            // 
            btnGrpConfiguracion.Dock = DockStyle.Top;
            btnGrpConfiguracion.Location = new Point(12, 848);
            btnGrpConfiguracion.Name = "btnGrpConfiguracion";
            btnGrpConfiguracion.Size = new Size(219, 42);
            btnGrpConfiguracion.TabIndex = 15;
            btnGrpConfiguracion.Text = "Configuración";
            btnGrpConfiguracion.UseVisualStyleBackColor = true;
            btnGrpConfiguracion.Click += btnGrpConfiguracion_Click;
            // 
            // pnlNomina
            // 
            pnlNomina.Controls.Add(btnNomina);
            pnlNomina.Dock = DockStyle.Top;
            pnlNomina.Location = new Point(12, 810);
            pnlNomina.Name = "pnlNomina";
            pnlNomina.Size = new Size(219, 38);
            pnlNomina.TabIndex = 14;
            pnlNomina.Visible = false;
            // 
            // btnNomina
            // 
            btnNomina.Dock = DockStyle.Top;
            btnNomina.Location = new Point(0, 0);
            btnNomina.Name = "btnNomina";
            btnNomina.Size = new Size(219, 36);
            btnNomina.TabIndex = 0;
            btnNomina.Text = "Empleados / Nómina (pendiente)";
            btnNomina.UseVisualStyleBackColor = true;
            btnNomina.Click += btnNomina_Click;
            // 
            // btnGrpNomina
            // 
            btnGrpNomina.Dock = DockStyle.Top;
            btnGrpNomina.Location = new Point(12, 768);
            btnGrpNomina.Name = "btnGrpNomina";
            btnGrpNomina.Size = new Size(219, 42);
            btnGrpNomina.TabIndex = 13;
            btnGrpNomina.Text = "Nómina";
            btnGrpNomina.UseVisualStyleBackColor = true;
            btnGrpNomina.Click += btnGrpNomina_Click;
            // 
            // pnlInventario
            // 
            pnlInventario.Controls.Add(btnInvMov);
            pnlInventario.Controls.Add(btnKardex);
            pnlInventario.Dock = DockStyle.Top;
            pnlInventario.Location = new Point(12, 692);
            pnlInventario.Name = "pnlInventario";
            pnlInventario.Size = new Size(219, 76);
            pnlInventario.TabIndex = 12;
            pnlInventario.Visible = false;
            // 
            // btnInvMov
            // 
            btnInvMov.Dock = DockStyle.Top;
            btnInvMov.Location = new Point(0, 36);
            btnInvMov.Name = "btnInvMov";
            btnInvMov.Size = new Size(219, 36);
            btnInvMov.TabIndex = 1;
            btnInvMov.Text = "Entrada / Salida";
            btnInvMov.UseVisualStyleBackColor = true;
            btnInvMov.Click += btnInvMov_Click;
            // 
            // btnKardex
            // 
            btnKardex.Dock = DockStyle.Top;
            btnKardex.Location = new Point(0, 0);
            btnKardex.Name = "btnKardex";
            btnKardex.Size = new Size(219, 36);
            btnKardex.TabIndex = 0;
            btnKardex.Text = "Kardex";
            btnKardex.UseVisualStyleBackColor = true;
            btnKardex.Click += btnKardex_Click;
            // 
            // btnGrpInventario
            // 
            btnGrpInventario.Dock = DockStyle.Top;
            btnGrpInventario.Location = new Point(12, 650);
            btnGrpInventario.Name = "btnGrpInventario";
            btnGrpInventario.Size = new Size(219, 42);
            btnGrpInventario.TabIndex = 11;
            btnGrpInventario.Text = "Inventario";
            btnGrpInventario.UseVisualStyleBackColor = true;
            btnGrpInventario.Click += btnGrpInventario_Click;
            // 
            // pnlProducto
            // 
            pnlProducto.Controls.Add(btnPromosProducto);
            pnlProducto.Controls.Add(btnProductos);
            pnlProducto.Dock = DockStyle.Top;
            pnlProducto.Location = new Point(12, 574);
            pnlProducto.Name = "pnlProducto";
            pnlProducto.Size = new Size(219, 76);
            pnlProducto.TabIndex = 10;
            pnlProducto.Visible = false;
            // 
            // btnPromosProducto
            // 
            btnPromosProducto.Dock = DockStyle.Top;
            btnPromosProducto.Location = new Point(0, 36);
            btnPromosProducto.Name = "btnPromosProducto";
            btnPromosProducto.Size = new Size(219, 36);
            btnPromosProducto.TabIndex = 1;
            btnPromosProducto.Text = "Promos / Descuentos";
            btnPromosProducto.UseVisualStyleBackColor = true;
            btnPromosProducto.Click += btnPromosProducto_Click;
            // 
            // btnProductos
            // 
            btnProductos.Dock = DockStyle.Top;
            btnProductos.Location = new Point(0, 0);
            btnProductos.Name = "btnProductos";
            btnProductos.Size = new Size(219, 36);
            btnProductos.TabIndex = 0;
            btnProductos.Text = "Productos";
            btnProductos.UseVisualStyleBackColor = true;
            btnProductos.Click += btnProductos_Click;
            // 
            // btnGrpProducto
            // 
            btnGrpProducto.Dock = DockStyle.Top;
            btnGrpProducto.Location = new Point(12, 532);
            btnGrpProducto.Name = "btnGrpProducto";
            btnGrpProducto.Size = new Size(219, 42);
            btnGrpProducto.TabIndex = 9;
            btnGrpProducto.Text = "Producto";
            btnGrpProducto.UseVisualStyleBackColor = true;
            btnGrpProducto.Click += btnGrpProducto_Click;
            // 
            // pnlCompra
            // 
            pnlCompra.Controls.Add(btnCompra);
            pnlCompra.Dock = DockStyle.Top;
            pnlCompra.Location = new Point(12, 494);
            pnlCompra.Name = "pnlCompra";
            pnlCompra.Size = new Size(219, 38);
            pnlCompra.TabIndex = 8;
            pnlCompra.Visible = false;
            // 
            // btnCompra
            // 
            btnCompra.Dock = DockStyle.Top;
            btnCompra.Location = new Point(0, 0);
            btnCompra.Name = "btnCompra";
            btnCompra.Size = new Size(219, 36);
            btnCompra.TabIndex = 0;
            btnCompra.Text = "Órdenes / Facturas (pendiente)";
            btnCompra.UseVisualStyleBackColor = true;
            btnCompra.Click += btnCompra_Click;
            // 
            // btnGrpCompra
            // 
            btnGrpCompra.Dock = DockStyle.Top;
            btnGrpCompra.Location = new Point(12, 452);
            btnGrpCompra.Name = "btnGrpCompra";
            btnGrpCompra.Size = new Size(219, 42);
            btnGrpCompra.TabIndex = 7;
            btnGrpCompra.Text = "Compra";
            btnGrpCompra.UseVisualStyleBackColor = true;
            btnGrpCompra.Click += btnGrpCompra_Click;
            // 
            // pnlVenta
            // 
            pnlVenta.Controls.Add(btnCierresHistorico);
            pnlVenta.Controls.Add(btnCierreCaja);
            pnlVenta.Controls.Add(menuFacturacion);
            pnlVenta.Controls.Add(btnClientes);
            pnlVenta.Controls.Add(btnPOS);
            pnlVenta.Dock = DockStyle.Top;
            pnlVenta.Location = new Point(12, 262);
            pnlVenta.Name = "pnlVenta";
            pnlVenta.Size = new Size(219, 190);
            pnlVenta.TabIndex = 6;
            pnlVenta.Visible = false;
            // 
            // btnCierresHistorico
            // 
            btnCierresHistorico.Dock = DockStyle.Top;
            btnCierresHistorico.Location = new Point(0, 144);
            btnCierresHistorico.Name = "btnCierresHistorico";
            btnCierresHistorico.Size = new Size(219, 36);
            btnCierresHistorico.TabIndex = 4;
            btnCierresHistorico.Text = "Histórico de Cierres";
            btnCierresHistorico.UseVisualStyleBackColor = true;
            btnCierresHistorico.Click += btnCierresHistorico_Click;
            // 
            // btnCierreCaja
            // 
            btnCierreCaja.Dock = DockStyle.Top;
            btnCierreCaja.Location = new Point(0, 108);
            btnCierreCaja.Name = "btnCierreCaja";
            btnCierreCaja.Size = new Size(219, 36);
            btnCierreCaja.TabIndex = 3;
            btnCierreCaja.Text = "Cierre de Caja";
            btnCierreCaja.UseVisualStyleBackColor = true;
            // 
            // menuFacturacion
            // 
            menuFacturacion.Dock = DockStyle.Top;
            menuFacturacion.Location = new Point(0, 72);
            menuFacturacion.Name = "menuFacturacion";
            menuFacturacion.Size = new Size(219, 36);
            menuFacturacion.TabIndex = 2;
            menuFacturacion.Text = "Factura/Cotizacion/Pro-Forma";
            menuFacturacion.UseVisualStyleBackColor = true;
            menuFacturacion.Click += menuFacturacion_Click;
            // 
            // btnClientes
            // 
            btnClientes.Dock = DockStyle.Top;
            btnClientes.Location = new Point(0, 36);
            btnClientes.Name = "btnClientes";
            btnClientes.Size = new Size(219, 36);
            btnClientes.TabIndex = 1;
            btnClientes.Text = "Clientes";
            btnClientes.UseVisualStyleBackColor = true;
            btnClientes.Click += btnClientes_Click;
            // 
            // btnPOS
            // 
            btnPOS.Dock = DockStyle.Top;
            btnPOS.Location = new Point(0, 0);
            btnPOS.Name = "btnPOS";
            btnPOS.Size = new Size(219, 36);
            btnPOS.TabIndex = 0;
            btnPOS.Text = "POS";
            btnPOS.UseVisualStyleBackColor = true;
            btnPOS.Click += btnPOS_Click;
            // 
            // btnGrpVenta
            // 
            btnGrpVenta.Dock = DockStyle.Top;
            btnGrpVenta.Location = new Point(12, 220);
            btnGrpVenta.Name = "btnGrpVenta";
            btnGrpVenta.Size = new Size(219, 42);
            btnGrpVenta.TabIndex = 5;
            btnGrpVenta.Text = "Venta";
            btnGrpVenta.UseVisualStyleBackColor = true;
            btnGrpVenta.Click += btnGrpVenta_Click;
            // 
            // pnlContabilidad
            // 
            pnlContabilidad.Controls.Add(btnContabilidad);
            pnlContabilidad.Dock = DockStyle.Top;
            pnlContabilidad.Location = new Point(12, 182);
            pnlContabilidad.Name = "pnlContabilidad";
            pnlContabilidad.Size = new Size(219, 38);
            pnlContabilidad.TabIndex = 4;
            pnlContabilidad.Visible = false;
            // 
            // btnContabilidad
            // 
            btnContabilidad.Dock = DockStyle.Top;
            btnContabilidad.Location = new Point(0, 0);
            btnContabilidad.Name = "btnContabilidad";
            btnContabilidad.Size = new Size(219, 36);
            btnContabilidad.TabIndex = 0;
            btnContabilidad.Text = "Asientos / Reportes (pendiente)";
            btnContabilidad.UseVisualStyleBackColor = true;
            btnContabilidad.Click += btnContabilidad_Click;
            // 
            // btnGrpContabilidad
            // 
            btnGrpContabilidad.Dock = DockStyle.Top;
            btnGrpContabilidad.Location = new Point(12, 140);
            btnGrpContabilidad.Name = "btnGrpContabilidad";
            btnGrpContabilidad.Size = new Size(219, 42);
            btnGrpContabilidad.TabIndex = 3;
            btnGrpContabilidad.Text = "Contabilidad";
            btnGrpContabilidad.UseVisualStyleBackColor = true;
            btnGrpContabilidad.Click += btnGrpContabilidad_Click;
            // 
            // pnlDashboard
            // 
            pnlDashboard.Controls.Add(btnDashboard);
            pnlDashboard.Dock = DockStyle.Top;
            pnlDashboard.Location = new Point(12, 102);
            pnlDashboard.Name = "pnlDashboard";
            pnlDashboard.Size = new Size(219, 38);
            pnlDashboard.TabIndex = 2;
            pnlDashboard.Visible = false;
            // 
            // btnDashboard
            // 
            btnDashboard.Dock = DockStyle.Top;
            btnDashboard.Location = new Point(0, 0);
            btnDashboard.Name = "btnDashboard";
            btnDashboard.Size = new Size(219, 36);
            btnDashboard.TabIndex = 0;
            btnDashboard.Text = "Inicio / KPIs";
            btnDashboard.UseVisualStyleBackColor = true;
            btnDashboard.Click += btnDashboard_Click;
            // 
            // btnGrpDashboard
            // 
            btnGrpDashboard.Dock = DockStyle.Top;
            btnGrpDashboard.Location = new Point(12, 60);
            btnGrpDashboard.Name = "btnGrpDashboard";
            btnGrpDashboard.Size = new Size(219, 42);
            btnGrpDashboard.TabIndex = 1;
            btnGrpDashboard.Text = "Dashboard";
            btnGrpDashboard.UseVisualStyleBackColor = true;
            btnGrpDashboard.Click += btnGrpDashboard_Click;
            // 
            // lblBrand
            // 
            lblBrand.Dock = DockStyle.Top;
            lblBrand.Location = new Point(12, 16);
            lblBrand.Name = "lblBrand";
            lblBrand.Padding = new Padding(0, 0, 0, 12);
            lblBrand.Size = new Size(219, 44);
            lblBrand.TabIndex = 0;
            lblBrand.Text = "ANDLOE ERP";
            lblBrand.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // panelTop
            // 
            panelTop.Controls.Add(lblEmpresaConectada);
            panelTop.Controls.Add(lblTitle);
            panelTop.Dock = DockStyle.Top;
            panelTop.Location = new Point(260, 0);
            panelTop.Name = "panelTop";
            panelTop.Size = new Size(940, 52);
            panelTop.TabIndex = 1;
            // 
            // lblEmpresaConectada
            // 
            lblEmpresaConectada.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblEmpresaConectada.Location = new Point(430, 18);
            lblEmpresaConectada.Name = "lblEmpresaConectada";
            lblEmpresaConectada.Size = new Size(490, 15);
            lblEmpresaConectada.TabIndex = 1;
            lblEmpresaConectada.Text = "Empresa: (cargando...)";
            lblEmpresaConectada.TextAlign = ContentAlignment.MiddleRight;
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(16, 18);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(36, 15);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "Inicio";
            // 
            // panelContent
            // 
            panelContent.Dock = DockStyle.Fill;
            panelContent.Location = new Point(260, 52);
            panelContent.Name = "panelContent";
            panelContent.Size = new Size(940, 668);
            panelContent.TabIndex = 2;
            // 
            // FormPrincipal
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1200, 720);
            Controls.Add(panelContent);
            Controls.Add(panelTop);
            Controls.Add(panelSidebar);
            Name = "FormPrincipal";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Andloe ERP";
            panelSidebar.ResumeLayout(false);
            pnlConfiguracion.ResumeLayout(false);
            pnlNomina.ResumeLayout(false);
            pnlInventario.ResumeLayout(false);
            pnlProducto.ResumeLayout(false);
            pnlCompra.ResumeLayout(false);
            pnlVenta.ResumeLayout(false);
            pnlContabilidad.ResumeLayout(false);
            pnlDashboard.ResumeLayout(false);
            panelTop.ResumeLayout(false);
            panelTop.PerformLayout();
            ResumeLayout(false);
        }
        #endregion

        private System.Windows.Forms.Panel panelSidebar;
        private System.Windows.Forms.Label lblBrand;

        private System.Windows.Forms.Button btnGrpDashboard;
        private System.Windows.Forms.Panel pnlDashboard;
        private System.Windows.Forms.Button btnDashboard;

        private System.Windows.Forms.Button btnGrpContabilidad;
        private System.Windows.Forms.Panel pnlContabilidad;
        private System.Windows.Forms.Button btnContabilidad;

        private System.Windows.Forms.Button btnGrpVenta;
        private System.Windows.Forms.Panel pnlVenta;
        private System.Windows.Forms.Button btnPOS;
        private System.Windows.Forms.Button btnClientes;
        private System.Windows.Forms.Button menuFacturacion;
        private System.Windows.Forms.Button btnCierreCaja;
        private System.Windows.Forms.Button btnCierresHistorico;

        private System.Windows.Forms.Button btnGrpCompra;
        private System.Windows.Forms.Panel pnlCompra;
        private System.Windows.Forms.Button btnCompra;

        private System.Windows.Forms.Button btnGrpProducto;
        private System.Windows.Forms.Panel pnlProducto;
        private System.Windows.Forms.Button btnProductos;
        private System.Windows.Forms.Button btnPromosProducto;

        private System.Windows.Forms.Button btnGrpInventario;
        private System.Windows.Forms.Panel pnlInventario;
        private System.Windows.Forms.Button btnKardex;
        private System.Windows.Forms.Button btnInvMov;

        private System.Windows.Forms.Button btnGrpNomina;
        private System.Windows.Forms.Panel pnlNomina;
        private System.Windows.Forms.Button btnNomina;

        private System.Windows.Forms.Button btnGrpConfiguracion;
        private System.Windows.Forms.Panel pnlConfiguracion;
        private System.Windows.Forms.Button btnUsuarios;
        private System.Windows.Forms.Button btnConfigSistema;
        private System.Windows.Forms.Button btnConexion;

        private System.Windows.Forms.Button btnSalir;

        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblEmpresaConectada;

        private System.Windows.Forms.Panel panelContent;
    }
}
