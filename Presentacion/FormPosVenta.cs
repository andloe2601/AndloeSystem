using Andloe.Data;
using Andloe.Entidad;
using Andloe.Logica;
using Andloe.Presentacion;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Andloe.Presentation
{
    public partial class FormPosVenta : Form
    {

        private readonly PosService _pos = new();
        private readonly ProductoRepository _productoRepo = new();
        private readonly CategoriaRepository _categoriaRepo = new();
        private readonly ClienteRepository _clienteRepo = new();
        private readonly PosPagoRepository _posPagoRepo = new();
        private readonly Andloe.Data.DGII.ECFDocumentoRepository _ecfDocRepo = new();
        private int? _ultimaFacturaId;
        private Button btnToggleSidebar = null!;
        private bool _sidebarExpandido = false;
        private const int SidebarExpandedWidth = 190;
        private const int SidebarCollapsedWidth = 60;


        private readonly PosTipoComprobanteConfigRepository _tipoCompRepo = new();
        private List<PosTipoComprobanteConfigDto> _tiposComprobante = new();
        private ComboBox cbTipoComprobanteFiscal = null!;
        private PosTipoComprobanteConfigDto? _tipoComprobanteActual;

        private readonly string _usuarioPos;
        private readonly int _cajaId;
        private readonly string _cajaNumero;
        private readonly bool _puedeCerrarCaja;

        private readonly List<Producto> _productosCache = new();
        private readonly Dictionary<int, string> _categorias = new();
        private readonly Dictionary<string, PosFormaPagoUiDto> _mediosPagoPorClave = new(StringComparer.OrdinalIgnoreCase);
        private readonly System.Windows.Forms.Timer _searchTimer = new();

        private string _filtroTexto = string.Empty;
        private int? _categoriaSeleccionadaId;
        private ClienteDto? _clienteActual;
        private long? _ultimaVentaId;
        private int _ventasSesion;
        private decimal _montoSesion;

        // Top
        private Panel pnlTop = null!;
        private Label lblTitulo = null!;
        private TextBox txtBuscar = null!;
        private Label lblEstado = null!;
        private Label lblUsuario = null!;
        private Label lblCaja = null!;

        // Sidebar
        private Panel pnlSidebar = null!;
        private FlowLayoutPanel pnlMenu = null!;
        private Button btnMenuVenta = null!;
        private Button btnMenuProductos = null!;
        private Button btnMenuClientes = null!;
        private Button btnMenuInventario = null!;
        private Button btnMenuCaja = null!;
        private Button btnMenuCompras = null!;
        private Button btnMenuDevoluciones = null!;
        private Button btnMenuReportes = null!;
        private Button btnMenuFacturacion = null!;
        private Button btnMenuConfig = null!;

        // Main
        private Panel pnlMain = null!;
        private FlowLayoutPanel pnlCategorias = null!;
        private FlowLayoutPanel pnlProductos = null!;
        private FlowLayoutPanel pnlAccesos = null!;
        private FlowLayoutPanel pnlResumen = null!;

        // Right
        private Panel pnlVenta = null!;
        private TextBox txtCliente = null!;
        private Button btnBuscarCliente = null!;
        private FlowLayoutPanel pnlCarrito = null!;
        private Label lblSubtotal = null!;
        private Label lblDescuento = null!;
        private Label lblImpuestos = null!;
        private Label lblTotal = null!;
        private Button btnPagoEfectivo = null!;
        private Button btnPagoTarjeta = null!;
        private Button btnPagoTransferencia = null!;
        private Button btnPagoOtro = null!;
        private Button btnCobrar = null!;
        private Button btnPendiente = null!;
        private Button btnImprimir = null!;
        private Button btnMas = null!;
        private Label lblClienteSecundario = null!;

        // Footer summary
        private Label lblVentasDia = null!;
        private Label lblTransacciones = null!;
        private Label lblTicketPromedio = null!;

        public FormPosVenta(string usuarioPos, int cajaId, string cajaNumero, bool puedeCerrarCaja)
        {
            _usuarioPos = usuarioPos ?? string.Empty;
            _cajaId = cajaId;
            _cajaNumero = cajaNumero ?? string.Empty;
            _puedeCerrarCaja = puedeCerrarCaja;

            InitializeUi();
            WireEvents();
        }

        private void InitializeUi()
        {
            SuspendLayout();

            Text = $"POS - Venta rápida | {_usuarioPos} | Caja {_cajaNumero}";
            WindowState = FormWindowState.Maximized;
            MinimumSize = new Size(1360, 820);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.FromArgb(242, 245, 248);
            Font = new Font("Segoe UI", 9.5f, FontStyle.Regular);
            KeyPreview = true;

            pnlSidebar = new Panel
            {
                Dock = DockStyle.Left,
                Width = SidebarCollapsedWidth,
                BackColor = Color.White,
                Padding = new Padding(8, 16, 8, 16)
            };

            btnToggleSidebar = new Button
            {
                Dock = DockStyle.Top,
                Height = 50,
                Text = "☰",
                Font = new Font("Segoe UI", 20f, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White,
                ForeColor = Color.FromArgb(14, 37, 64),
                TextAlign = ContentAlignment.MiddleCenter
            };
            btnToggleSidebar.FlatAppearance.BorderSize = 0;
            pnlSidebar.Controls.Add(btnToggleSidebar);

            pnlMenu = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoScroll = true,
                Padding = new Padding(0, 18, 0, 12)
            };
            pnlSidebar.Controls.Add(pnlMenu);

            btnMenuVenta = CreateSideButton("Venta", true);
            btnMenuProductos = CreateSideButton("Productos");
            btnMenuClientes = CreateSideButton("Clientes");
            btnMenuInventario = CreateSideButton("Inventario");
            btnMenuCaja = CreateSideButton("Caja");
            btnMenuCompras = CreateSideButton("Compras");
            btnMenuDevoluciones = CreateSideButton("Devoluciones");
            btnMenuReportes = CreateSideButton("Reportes");
            btnMenuFacturacion = CreateSideButton("Facturación");
            btnMenuConfig = CreateSideButton("Configuración");

            pnlMenu.Controls.AddRange(new Control[]
            {
        btnMenuVenta, btnMenuProductos, btnMenuClientes, btnMenuInventario,
        btnMenuCaja, btnMenuCompras, btnMenuDevoluciones, btnMenuReportes,
        btnMenuFacturacion
            });

            pnlSidebar.Controls.Add(btnMenuConfig);
            btnMenuConfig.Dock = DockStyle.Bottom;

            pnlTop = new Panel
            {
                Dock = DockStyle.Top,
                Height = 74,
                BackColor = Color.FromArgb(245, 247, 250),
                Padding = new Padding(18, 16, 18, 10)
            };

            lblTitulo = new Label
            {
                AutoSize = true,
                Text = "Venta actual",
                Font = new Font("Segoe UI", 16f, FontStyle.Bold),
                ForeColor = Color.FromArgb(14, 37, 64),
                Location = new Point(18, 20)
            };
            pnlTop.Controls.Add(lblTitulo);

            txtBuscar = new TextBox
            {
                PlaceholderText = "Buscar productos (F2)...",
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Segoe UI", 12f),
                Size = new Size(430, 36),
                Location = new Point(220, 16)
            };
            pnlTop.Controls.Add(txtBuscar);

            lblEstado = CreatePillLabel("● Abierta", Color.FromArgb(0, 161, 118), Color.FromArgb(235, 250, 246));
            lblEstado.Location = new Point(680, 19);
            pnlTop.Controls.Add(lblEstado);

            lblUsuario = new Label
            {
                AutoSize = true,
                Text = _usuarioPos,
                Font = new Font("Segoe UI", 10.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 41, 59),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Location = new Point(930, 18)
            };
            pnlTop.Controls.Add(lblUsuario);

            lblCaja = new Label
            {
                AutoSize = true,
                Text = $"Caja {_cajaNumero}",
                Font = new Font("Segoe UI", 10.5f),
                ForeColor = Color.FromArgb(76, 91, 106),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Location = new Point(930, 40)
            };
            pnlTop.Controls.Add(lblCaja);

            pnlMain = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(18, 10, 18, 18),
                BackColor = Color.FromArgb(242, 245, 248)
            };

            pnlVenta = CreateCardPanel();
            pnlVenta.Dock = DockStyle.Right;
            pnlVenta.Width = 420;
            pnlVenta.Padding = new Padding(16);

            BuildRightSalePanel();

            var centerWrap = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(0, 0, 18, 0)
            };

            var centerStack = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 4,
                BackColor = Color.Transparent
            };
            centerStack.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            centerStack.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
            centerStack.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            centerStack.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            centerWrap.Controls.Add(centerStack);

            pnlCategorias = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Height = 42,
                Margin = new Padding(0, 0, 0, 12)
            };
            centerStack.Controls.Add(pnlCategorias, 0, 0);

            var pnlProductosCard = CreateCardPanel();
            pnlProductosCard.Dock = DockStyle.Fill;
            pnlProductosCard.Padding = new Padding(16);
            centerStack.Controls.Add(pnlProductosCard, 0, 1);

            var lblProductos = new Label
            {
                Text = "Productos",
                Dock = DockStyle.Top,
                Height = 30,
                Font = new Font("Segoe UI", 15f, FontStyle.Bold),
                ForeColor = Color.FromArgb(17, 24, 39)
            };
            pnlProductosCard.Controls.Add(lblProductos);

            pnlProductos = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                Padding = new Padding(0, 6, 0, 0)
            };
            pnlProductosCard.Controls.Add(pnlProductos);
            pnlProductosCard.Controls.SetChildIndex(lblProductos, 1);

            var accesosWrap = CreateCardPanel();
            accesosWrap.Dock = DockStyle.Top;
            accesosWrap.Height = 126;
            accesosWrap.Padding = new Padding(16, 12, 16, 12);
            centerStack.Controls.Add(accesosWrap, 0, 2);

            var lblAccesos = new Label
            {
                Text = "Accesos rápidos",
                Dock = DockStyle.Top,
                Height = 28,
                Font = new Font("Segoe UI", 12.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(17, 24, 39)
            };
            accesosWrap.Controls.Add(lblAccesos);

            pnlAccesos = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(0, 4, 0, 0)
            };
            accesosWrap.Controls.Add(pnlAccesos);
            accesosWrap.Controls.SetChildIndex(lblAccesos, 1);

            var resumenWrap = CreateCardPanel();
            resumenWrap.Dock = DockStyle.Top;
            resumenWrap.Height = 126;
            resumenWrap.Padding = new Padding(16, 12, 16, 12);
            centerStack.Controls.Add(resumenWrap, 0, 3);

            var lblResumen = new Label
            {
                Text = "Resumen del día",
                Dock = DockStyle.Top,
                Height = 28,
                Font = new Font("Segoe UI", 12.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(17, 24, 39)
            };
            resumenWrap.Controls.Add(lblResumen);

            pnlResumen = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(0, 4, 0, 0)
            };
            resumenWrap.Controls.Add(pnlResumen);
            resumenWrap.Controls.SetChildIndex(lblResumen, 1);

            pnlMain.Controls.Add(centerWrap);
            pnlMain.Controls.Add(pnlVenta);

            Controls.Add(pnlMain);
            Controls.Add(pnlTop);
            Controls.Add(pnlSidebar);

            _searchTimer.Interval = 280;
            ResumeLayout();
        }

        private void BuildRightSalePanel()
        {
            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 7,
                BackColor = Color.Transparent
            };
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            pnlVenta.Controls.Add(layout);

            var lblHeader = new Label
            {
                Text = "Venta actual",
                Dock = DockStyle.Top,
                Height = 34,
                Font = new Font("Segoe UI", 16f, FontStyle.Bold),
                ForeColor = Color.FromArgb(17, 24, 39)
            };
            layout.Controls.Add(lblHeader, 0, 0);

            var clientWrap = new Panel { Dock = DockStyle.Top, Height = 110 };
            layout.Controls.Add(clientWrap, 0, 1);

            txtCliente = new TextBox
            {
                PlaceholderText = "Cliente (opcional)",
                ReadOnly = true,
                BorderStyle = BorderStyle.FixedSingle,
                Location = new Point(0, 4),
                Width = 335,
                Font = new Font("Segoe UI", 11f)
            };
            clientWrap.Controls.Add(txtCliente);

            btnBuscarCliente = new Button
            {
                Text = "+",
                Width = 42,
                Height = 36,
                Location = new Point(343, 3),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White,
                ForeColor = Color.FromArgb(16, 185, 129),
                Font = new Font("Segoe UI", 18f, FontStyle.Regular)
            };
            btnBuscarCliente.FlatAppearance.BorderColor = Color.FromArgb(220, 228, 236);
            clientWrap.Controls.Add(btnBuscarCliente);

            lblClienteSecundario = new Label
            {
                Text = "Consumidor final",
                AutoSize = false,
                Width = 390,
                Height = 22,
                Location = new Point(2, 43),
                ForeColor = Color.FromArgb(100, 116, 139)
            };
            clientWrap.Controls.Add(lblClienteSecundario);

            var lblTipoComprobante = new Label
            {
                Text = "Comprobante fiscal",
                AutoSize = true,
                Location = new Point(2, 68),
                ForeColor = Color.FromArgb(100, 116, 139)
            };
            clientWrap.Controls.Add(lblTipoComprobante);

            cbTipoComprobanteFiscal = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Width = 390,
                Location = new Point(0, 86),
                Font = new Font("Segoe UI", 10f)
            };
            clientWrap.Controls.Add(cbTipoComprobanteFiscal);

            pnlCarrito = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoScroll = true,
                Padding = new Padding(0, 4, 0, 4)
            };
            layout.Controls.Add(pnlCarrito, 0, 2);

            var btnDescuento = new Button
            {
                Text = "+ Agregar descuento",
                Height = 38,
                Width = 180,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White,
                ForeColor = Color.FromArgb(15, 23, 42),
                Margin = new Padding(0, 8, 0, 10)
            };
            btnDescuento.FlatAppearance.BorderColor = Color.FromArgb(220, 228, 236);
            btnDescuento.Click += (_, __) => AplicarDescuentoALineaSeleccionada();
            layout.Controls.Add(btnDescuento, 0, 3);

            var totalsWrap = new Panel { Dock = DockStyle.Top, Height = 120 };
            layout.Controls.Add(totalsWrap, 0, 4);
            lblSubtotal = CreateValueLine(totalsWrap, "Subtotal", 0);
            lblDescuento = CreateValueLine(totalsWrap, "Descuento", 28);
            lblImpuestos = CreateValueLine(totalsWrap, "Impuestos (DGII 18%)", 56);

            var totalTitle = new Label
            {
                Text = "Total",
                AutoSize = true,
                Font = new Font("Segoe UI", 17f, FontStyle.Bold),
                ForeColor = Color.FromArgb(17, 24, 39),
                Location = new Point(0, 86)
            };
            totalsWrap.Controls.Add(totalTitle);

            lblTotal = new Label
            {
                Text = "RD$ 0.00",
                AutoSize = true,
                Font = new Font("Segoe UI", 20f, FontStyle.Bold),
                ForeColor = Color.FromArgb(14, 37, 64),
                Location = new Point(245, 82)
            };
            totalsWrap.Controls.Add(lblTotal);

            btnCobrar = new Button
            {
                Text = "Cobrar (F9)",
                Dock = DockStyle.Top,
                Height = 54,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(13, 177, 146),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 15f, FontStyle.Bold),
                Margin = new Padding(0, 12, 0, 10)
            };
            btnCobrar.FlatAppearance.BorderSize = 0;
            layout.Controls.Add(btnCobrar, 0, 5);

            var footerActions = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 68,
                FlowDirection = FlowDirection.LeftToRight,
                Padding = new Padding(0, 8, 0, 0)
            };
            layout.Controls.Add(footerActions, 0, 6);

            btnPendiente = CreateSmallActionButton("Pendiente (F7)");
            btnImprimir = CreateSmallActionButton("Imprimir (F8)");
            btnMas = CreateSmallActionButton("Más");
            footerActions.Controls.AddRange(new Control[] { btnPendiente, btnImprimir, btnMas });
        }

        private void WireEvents()
        {
            Load += FormPosVenta_Load;
            KeyDown += FormPosVenta_KeyDown;

            txtBuscar.TextChanged += (_, __) =>
            {
                _searchTimer.Stop();
                _searchTimer.Start();
            };

            txtBuscar.KeyDown += (_, e) =>
            {
                if (e.KeyCode != Keys.Enter) return;

                e.SuppressKeyPress = true;

                var valor = (txtBuscar.Text ?? "").Trim();
                if (string.IsNullOrWhiteSpace(valor)) return;

                AgregarProductoPorCodigoOBarras(valor);
            };

            _searchTimer.Tick += (_, __) =>
            {
                _searchTimer.Stop();
                _filtroTexto = (txtBuscar.Text ?? string.Empty).Trim();
                RenderProductos();
            };

            cbTipoComprobanteFiscal.SelectedIndexChanged += (_, __) =>
            {
                _tipoComprobanteActual = cbTipoComprobanteFiscal.SelectedItem as PosTipoComprobanteConfigDto;

                if (_tipoComprobanteActual?.RequiereCliente == true && _clienteActual == null)
                {
                    MessageBox.Show(
                        "Este tipo de comprobante requiere cliente.",
                        "POS",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    BuscarCliente();
                }
            };

            btnBuscarCliente.Click += (_, __) => BuscarCliente();
            btnCobrar.Click += (_, __) => CobrarConPantallaPago();
            btnPendiente.Click += (_, __) => MessageBox.Show("Pendientes aún no implementado en este nuevo form.", "POS", MessageBoxButtons.OK, MessageBoxIcon.Information);
            btnImprimir.Click += (_, __) => ReimprimirUltimaVenta();
            btnMas.Click += (_, __) => MostrarMenuExtra();
            btnToggleSidebar.Click += (_, __) => AlternarSidebar();

            btnMenuProductos.Click += (_, __) => AbrirBusquedaProducto();
            btnMenuClientes.Click += (_, __) => BuscarCliente();
            btnMenuCaja.Click += (_, __) => AbrirCaja();
            btnMenuReportes.Click += (_, __) => MessageBox.Show("Aquí puedes enlazar tus reportes POS o gerenciales.", "POS", MessageBoxButtons.OK, MessageBoxIcon.Information);
            btnMenuFacturacion.Click += (_, __) => MessageBox.Show("Aquí puedes abrir FormFacturaV o el módulo e-CF.", "POS", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private int? ObtenerSucursalActual()
        {
            return 1;
        }

        private void AgregarProductoPorCodigoOBarras(string valor)
        {
            try
            {
                var prod = _productoRepo.ObtenerPorCodigoOBarras(valor);

                if (prod == null || string.IsNullOrWhiteSpace(prod.Codigo))
                {
                    MessageBox.Show(
                        $"No se encontró producto con código/barra: {valor}",
                        "POS",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                AgregarProducto(prod.Codigo);

                txtBuscar.Clear();
                _filtroTexto = string.Empty;
                RenderProductos();
                txtBuscar.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error buscando producto por código/barra.\n{ex.Message}", "POS",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string ConstruirResumenFiscal(int facturaId)
        {
            var ecf = _ecfDocRepo.ObtenerDocumentoPorFactura(facturaId);
            if (ecf == null)
                return "ECF no encontrado.";

            var encf = string.IsNullOrWhiteSpace(ecf.ENCF) ? "Pendiente" : ecf.ENCF;
            var trackId = string.IsNullOrWhiteSpace(ecf.TrackId) ? "Pendiente" : ecf.TrackId;
            var estadoDgii = string.IsNullOrWhiteSpace(ecf.EstadoDGII) ? "N/D" : ecf.EstadoDGII;
            var estadoProceso = string.IsNullOrWhiteSpace(ecf.EstadoProceso) ? "N/D" : ecf.EstadoProceso;

            return $"FacturaId={facturaId} | e-NCF={encf} | DGII={estadoDgii} | Proceso={estadoProceso} | TrackId={trackId}";
        }

        private void AlternarSidebar()
        {
            _sidebarExpandido = !_sidebarExpandido;

            pnlSidebar.Width = _sidebarExpandido ? SidebarExpandedWidth : SidebarCollapsedWidth;
            pnlSidebar.Padding = _sidebarExpandido
                ? new Padding(14, 16, 14, 16)
                : new Padding(8, 16, 8, 16);

            foreach (var btn in pnlMenu.Controls.OfType<Button>())
            {
                btn.TextAlign = _sidebarExpandido ? ContentAlignment.MiddleLeft : ContentAlignment.MiddleCenter;
                btn.Padding = _sidebarExpandido ? new Padding(14, 0, 0, 0) : new Padding(0);
                btn.Width = _sidebarExpandido ? 150 : 42;
                btn.Text = _sidebarExpandido ? (btn.Tag?.ToString() ?? btn.Text) : "•";
            }

            btnMenuConfig.TextAlign = _sidebarExpandido ? ContentAlignment.MiddleLeft : ContentAlignment.MiddleCenter;
            btnMenuConfig.Padding = _sidebarExpandido ? new Padding(14, 0, 0, 0) : new Padding(0);
            btnMenuConfig.Width = _sidebarExpandido ? 150 : 42;
            btnMenuConfig.Text = _sidebarExpandido ? (btnMenuConfig.Tag?.ToString() ?? btnMenuConfig.Text) : "⚙";
        }



        private void CargarTiposComprobanteFiscal()
        {
            var sucursalId = ObtenerSucursalActual();
            _tiposComprobante = _tipoCompRepo.ListarActivos(_cajaId, sucursalId);

            cbTipoComprobanteFiscal.DataSource = null;

            if (_tiposComprobante == null || _tiposComprobante.Count == 0)
                throw new InvalidOperationException(
                    "No hay tipos de comprobante POS configurados. Debe alimentar la tabla POS_TipoComprobanteConfig.");

            cbTipoComprobanteFiscal.DisplayMember = "NombreMostrar";
            cbTipoComprobanteFiscal.ValueMember = "PosTipoComprobanteConfigId";
            cbTipoComprobanteFiscal.DataSource = _tiposComprobante;

            AplicarReglaTipoComprobanteSegunCliente();
        }

        private void AplicarReglaTipoComprobanteSegunCliente()
        {
            if (_tiposComprobante == null || _tiposComprobante.Count == 0)
                throw new InvalidOperationException("No hay comprobantes fiscales configurados en POS.");

            var tieneDocumento = !string.IsNullOrWhiteSpace(_clienteActual?.RncCedula);
            PosTipoComprobanteConfigDto? elegido;

            if (!tieneDocumento)
            {
                // Consumidor final => debe ir con comprobante de consumo
                elegido = _tiposComprobante.FirstOrDefault(x =>
                    x.Activo &&
                    x.PermiteEnPOS &&
                    x.TipoECFId == 2); // E32 / consumo

                if (elegido == null)
                    throw new InvalidOperationException(
                        "No existe configuración activa en POS_TipoComprobanteConfig para consumo final (TipoECFId=2).");

                cbTipoComprobanteFiscal.Enabled = false;
            }
            else
            {
                // Cliente con RNC/Cédula => por defecto crédito fiscal, pero puede quedar habilitado
                elegido = _tiposComprobante.FirstOrDefault(x =>
                    x.Activo &&
                    x.PermiteEnPOS &&
                    x.TipoECFId == 1); // E31 / crédito fiscal

                if (elegido == null)
                {
                    elegido = _tiposComprobante.FirstOrDefault(x =>
                        x.Activo &&
                        x.PermiteEnPOS &&
                        x.EsDefault);
                }

                if (elegido == null)
                    throw new InvalidOperationException(
                        "No existe configuración activa en POS_TipoComprobanteConfig para cliente con documento fiscal.");

                cbTipoComprobanteFiscal.Enabled = true;
            }

            cbTipoComprobanteFiscal.SelectedValue = elegido.PosTipoComprobanteConfigId;
            _tipoComprobanteActual = elegido;
        }


        private void ValidarAntesDeCobrar()
        {
            if (_tipoComprobanteActual == null)
                throw new InvalidOperationException("Debe seleccionar un tipo de comprobante fiscal.");

            if (_tipoComprobanteActual.RequiereCliente && _clienteActual == null)
                throw new InvalidOperationException("Este tipo de comprobante requiere cliente.");

            if (_tipoComprobanteActual.RequiereDocumentoCliente &&
                string.IsNullOrWhiteSpace(_clienteActual?.RncCedula))
                throw new InvalidOperationException("Este tipo de comprobante requiere documento del cliente.");
        }


        private void FormPosVenta_Load(object? sender, EventArgs e)
        {
            try
            {
                CargarClientePorDefecto();
                CargarCategorias();
                CargarMediosPago();
                CargarTiposComprobanteFiscal();
                CargarProductos();
                ConstruirAccesosRapidos();
                ConstruirResumen();
                RefrescarVenta();
                AlternarSidebar();
                AlternarSidebar();
                txtBuscar.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"No se pudo iniciar el POS nuevo.\n{ex.Message}", "POS", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FormPosVenta_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F2)
            {
                txtBuscar.Focus();
                txtBuscar.SelectAll();
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.F9)
            {
                CobrarConPantallaPago();
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.F8)
            {
                ReimprimirUltimaVenta();
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.F7)
            {
                MessageBox.Show("Pendiente aún no implementado en este diseño.", "POS", MessageBoxButtons.OK, MessageBoxIcon.Information);
                e.Handled = true;
            }
        }

        private void CargarClientePorDefecto()
        {
            _clienteActual = null;
            RefrescarClienteVisual();
        }

        private void CargarCategorias()
        {
            pnlCategorias.Controls.Clear();
            _categorias.Clear();

            pnlCategorias.Controls.Add(CreateCategoryChip("Todos", null, true));
            foreach (var categoria in _categoriaRepo.ListarActivas())
            {
                _categorias[categoria.Id] = categoria.Nombre;
                pnlCategorias.Controls.Add(CreateCategoryChip(categoria.Nombre, categoria.Id, false));
            }
        }

        private void CargarMediosPago()
        {
            _mediosPagoPorClave.Clear();

            var dt = SqlHelper.ExecuteDataTable(@"
SELECT FormaPagoCodigo, Descripcion
FROM dbo.ECFFormaPagoCatalogo
WHERE Activo = 1
ORDER BY TRY_CONVERT(INT, FormaPagoCodigo);");

            foreach (DataRow row in dt.Rows)
            {
                var codigo = Convert.ToString(row["FormaPagoCodigo"]) ?? "";
                var descripcion = Convert.ToString(row["Descripcion"]) ?? "";

                if (string.IsNullOrWhiteSpace(codigo))
                    continue;

                var dto = new PosFormaPagoUiDto
                {
                    FormaPagoCodigo = codigo,
                    Nombre = descripcion
                };

                switch (codigo)
                {
                    case "1":
                        _mediosPagoPorClave["efectivo"] = dto;
                        break;

                    case "2":
                        _mediosPagoPorClave["transfer"] = dto;
                        _mediosPagoPorClave["transferencia"] = dto;
                        _mediosPagoPorClave["cheque"] = dto;
                        break;

                    case "3":
                        _mediosPagoPorClave["tarjeta"] = dto;
                        break;

                    case "4":
                        _mediosPagoPorClave["credito"] = dto;
                        break;
                }
            }
        }

        private void CargarProductos()
        {
            _productosCache.Clear();
            var items = _productoRepo.Listar(_filtroTexto, 120)
                .Where(x => x.Estado == 1)
                .ToList();

            foreach (var item in items)
            {
                try
                {
                    var detalle = _productoRepo.ObtenerPorCodigo(item.Codigo);
                    if (detalle != null)
                    {
                        detalle.StockActual = item.StockActual;
                        detalle.PrecioVenta = item.PrecioVenta;
                        _productosCache.Add(detalle);
                    }
                    else
                    {
                        _productosCache.Add(item);
                    }
                }
                catch
                {
                    _productosCache.Add(item);
                }
            }

            RenderProductos();
        }

        private void RenderProductos()
        {
            pnlProductos.SuspendLayout();
            pnlProductos.Controls.Clear();

            IEnumerable<Producto> query = _productosCache;

            if (!string.IsNullOrWhiteSpace(_filtroTexto))
            {
                query = query.Where(p =>
                    (p.Codigo ?? string.Empty).Contains(_filtroTexto, StringComparison.OrdinalIgnoreCase) ||
                    (p.Descripcion ?? string.Empty).Contains(_filtroTexto, StringComparison.OrdinalIgnoreCase));
            }

            if (_categoriaSeleccionadaId.HasValue)
                query = query.Where(p => p.CategoriaId == _categoriaSeleccionadaId.Value);

            foreach (var producto in query.Take(120))
            {
                pnlProductos.Controls.Add(CreateProductCard(producto));
            }

            if (pnlProductos.Controls.Count == 0)
            {
                var empty = new Label
                {
                    Text = "No hay productos para mostrar.",
                    ForeColor = Color.FromArgb(100, 116, 139),
                    AutoSize = true,
                    Margin = new Padding(12)
                };
                pnlProductos.Controls.Add(empty);
            }

            pnlProductos.ResumeLayout();
        }

        private Control CreateProductCard(Producto producto)
        {
            var card = CreateCardPanel(Color.White, false);
            card.Width = 138;
            card.Height = 168;
            card.Margin = new Padding(0, 0, 12, 12);
            card.Padding = new Padding(10);
            card.Cursor = Cursors.Hand;

            var imageBox = new Panel
            {
                Dock = DockStyle.Top,
                Height = 68,
                BackColor = Color.FromArgb(245, 247, 250)
            };
            card.Controls.Add(imageBox);

            var imgLabel = new Label
            {
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Text = ObtenerIniciales(producto.Descripcion),
                Font = new Font("Segoe UI", 18f, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 118, 110)
            };
            imageBox.Controls.Add(imgLabel);

            var lblDesc = new Label
            {
                Dock = DockStyle.Top,
                Height = 44,
                Text = producto.Descripcion,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                ForeColor = Color.FromArgb(31, 41, 55),
                Padding = new Padding(0, 8, 0, 0)
            };
            card.Controls.Add(lblDesc);

            var lblPrecio = new Label
            {
                Dock = DockStyle.Bottom,
                Height = 24,
                Text = $"RD$ {producto.PrecioVenta:N2}",
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                ForeColor = Color.FromArgb(13, 148, 136)
            };
            card.Controls.Add(lblPrecio);

            card.Click += (_, __) => AgregarProducto(producto.Codigo);
            foreach (Control ctrl in card.Controls)
                ctrl.Click += (_, __) => AgregarProducto(producto.Codigo);

            return card;
        }

        private void AgregarProducto(string codigo)
        {
            try
            {
                var item = _pos.AgregarPorCodigo(codigo, 1m, _clienteActual?.Codigo);
                if (item == null)
                {
                    MessageBox.Show("No se pudo agregar el producto. Verifica stock o configuración.", "POS", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                RefrescarVenta();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error agregando producto.\n{ex.Message}", "POS", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RefrescarVenta()
        {
            RenderCarrito();
            RefrescarTotales();
            ActualizarResumenSesion();
        }

        private void RenderCarrito()
        {
            pnlCarrito.SuspendLayout();
            pnlCarrito.Controls.Clear();

            if (_pos.Carrito.Count == 0)
            {
                var empty = new Label
                {
                    Text = "No hay productos agregados.",
                    ForeColor = Color.FromArgb(100, 116, 139),
                    AutoSize = true,
                    Margin = new Padding(4, 8, 4, 8)
                };
                pnlCarrito.Controls.Add(empty);
                pnlCarrito.ResumeLayout();
                return;
            }

            foreach (var item in _pos.Carrito)
                pnlCarrito.Controls.Add(CreateCartItem(item));

            pnlCarrito.ResumeLayout();
        }

        private Control CreateCartItem(ItemCarrito item)
        {
            var row = new Panel
            {
                Width = 360,
                Height = 72,
                BackColor = Color.White,
                Margin = new Padding(0, 0, 0, 8),
                Padding = new Padding(8)
            };

            var pic = new Panel
            {
                Width = 42,
                Height = 42,
                Location = new Point(0, 10),
                BackColor = Color.FromArgb(245, 247, 250)
            };
            row.Controls.Add(pic);

            var initials = new Label
            {
                Dock = DockStyle.Fill,
                Text = ObtenerIniciales(item.Descripcion),
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.FromArgb(13, 148, 136),
                Font = new Font("Segoe UI", 10f, FontStyle.Bold)
            };
            pic.Controls.Add(initials);

            var lblDesc = new Label
            {
                Text = item.Descripcion,
                Location = new Point(52, 6),
                Size = new Size(185, 22),
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(17, 24, 39)
            };
            row.Controls.Add(lblDesc);

            var lblDetalle = new Label
            {
                Text = $"RD$ {item.PrecioUnit:N2}   ×   {item.Cantidad:N0}",
                Location = new Point(52, 30),
                Size = new Size(170, 22),
                ForeColor = Color.FromArgb(100, 116, 139)
            };
            row.Controls.Add(lblDetalle);

            var lblTotalLinea = new Label
            {
                Text = $"RD$ {item.Total:N2}",
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleRight,
                Location = new Point(230, 8),
                Size = new Size(94, 22),
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(17, 24, 39)
            };
            row.Controls.Add(lblTotalLinea);

            var btnMenos = CreateQtyButton("−", new Point(228, 36), (_, __) => CambiarCantidad(item.ProductoCodigo, -1m));
            var btnMas = CreateQtyButton("+", new Point(262, 36), (_, __) => CambiarCantidad(item.ProductoCodigo, 1m));
            var btnDelete = CreateQtyButton("🗑", new Point(300, 36), (_, __) => EliminarItem(item.ProductoCodigo));
            row.Controls.AddRange(new Control[] { btnMenos, btnMas, btnDelete });

            return row;
        }

        private Button CreateQtyButton(string text, Point location, EventHandler onClick)
        {
            var btn = new Button
            {
                Text = text,
                Width = 28,
                Height = 24,
                Location = location,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White,
                ForeColor = Color.FromArgb(15, 23, 42),
                Font = new Font("Segoe UI", 9f, FontStyle.Bold)
            };
            btn.FlatAppearance.BorderColor = Color.FromArgb(220, 228, 236);
            btn.Click += onClick;
            return btn;
        }

        private void CambiarCantidad(string productoCodigo, decimal delta)
        {
            var item = _pos.Carrito.FirstOrDefault(x => x.ProductoCodigo == productoCodigo);
            if (item == null) return;

            if (delta > 0)
            {
                var agregado = _pos.AgregarPorCodigo(productoCodigo, 1m, _clienteActual?.Codigo);
                if (agregado == null)
                {
                    MessageBox.Show("No se pudo aumentar la cantidad. Verifica stock.", "POS", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
            else
            {
                item.Cantidad += delta;
                if (item.Cantidad <= 0)
                    _pos.Quitar(productoCodigo);
                else
                    ReaplicarPromos();
            }

            RefrescarVenta();
        }

        private void EliminarItem(string productoCodigo)
        {
            _pos.Quitar(productoCodigo);
            RefrescarVenta();
        }

        private void ReaplicarPromos()
        {
            var clienteCodigo = _clienteActual?.Codigo;
            _pos.RecalcularPromosCarrito(clienteCodigo);
        }

        private void RefrescarTotales()
        {
            var total = _pos.Totales();
            var descuento = _pos.Carrito.Sum(x => x.DescuentoMonto);

            lblSubtotal.Text = $"RD$ {total.Subtotal:N2}";
            lblDescuento.Text = $"RD$ {descuento:N2}";
            lblImpuestos.Text = $"RD$ {total.Itbis:N2}";
            lblTotal.Text = $"RD$ {total.Total:N2}";
        }

        private void BuscarCliente()
        {
            using var frm = new FormBuscarCliente();
            if (frm.ShowDialog(this) != DialogResult.OK || frm.ClienteSeleccionado == null)
                return;

            _clienteActual = frm.ClienteSeleccionado;
            RefrescarClienteVisual();
            ReaplicarPromos();
            RefrescarVenta();
        }

        private void RefrescarClienteVisual()
        {
            var nombre = _clienteActual?.Nombre;
            var codigo = _clienteActual?.Codigo;
            var rnc = _clienteActual?.RncCedula;

            txtCliente.Text = string.IsNullOrWhiteSpace(nombre) ? string.Empty : nombre;
            lblClienteSecundario.Text = string.IsNullOrWhiteSpace(codigo)
                ? "Consumidor final"
                : $"{codigo} · {rnc}";

            if (cbTipoComprobanteFiscal != null && _tiposComprobante != null && _tiposComprobante.Count > 0)
                AplicarReglaTipoComprobanteSegunCliente();
        }

        private void AplicarDescuentoALineaSeleccionada()
        {
            if (_pos.Carrito.Count == 0)
            {
                MessageBox.Show("No hay productos en la venta.", "POS", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var item = _pos.Carrito.Last();
            if (!ConfigService.DescuentoHabilitado)
            {
                MessageBox.Show("Los descuentos están deshabilitados en configuración.", "POS", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var maxPermitido = _pos.CalcularMaxDescuentoPct(_clienteActual?.Codigo, item.ProductoCodigo);
            if (maxPermitido <= 0)
            {
                MessageBox.Show("Esta línea no permite descuento.", "POS", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var nuevoPct = PedirPorcentaje(maxPermitido, item.DescuentoPct);
            if (!nuevoPct.HasValue) return;

            item.DescuentoPct = nuevoPct.Value;
            item.DescuentoMonto = Math.Round(item.SubtotalBruto * (item.DescuentoPct / 100m), 2);
            RefrescarVenta();
        }

        private decimal? PedirPorcentaje(decimal maxPermitido, decimal actual)
        {
            using var frm = new Form
            {
                Text = "Descuento",
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                ShowInTaskbar = false,
                MaximizeBox = false,
                MinimizeBox = false,
                ClientSize = new Size(280, 146)
            };

            var lbl = new Label { Text = $"% descuento (0 - {maxPermitido:N2})", AutoSize = true, Location = new Point(14, 18) };
            var num = new NumericUpDown
            {
                Location = new Point(14, 50),
                Width = 120,
                DecimalPlaces = 2,
                Minimum = 0,
                Maximum = maxPermitido,
                Value = Math.Min(actual, maxPermitido)
            };
            var ok = new Button { Text = "Aceptar", DialogResult = DialogResult.OK, Location = new Point(40, 96), Width = 84 };
            var cancel = new Button { Text = "Cancelar", DialogResult = DialogResult.Cancel, Location = new Point(150, 96), Width = 84 };
            frm.Controls.AddRange(new Control[] { lbl, num, ok, cancel });
            frm.AcceptButton = ok;
            frm.CancelButton = cancel;
            return frm.ShowDialog(this) == DialogResult.OK ? num.Value : null;
        }

        private void CobrarConPantallaPago()
        {
            try
            {
                ValidarAntesDeCobrar();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Validación",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            var total = _pos.Totales().Total;
            if (total <= 0)
            {
                MessageBox.Show(
                    "No hay nada para cobrar.",
                    "POS",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            using var frm = new FormSeleccionPago(total, ConfigService.MonedaDefecto);

            if (frm.ShowDialog(this) != DialogResult.OK || frm.Result == null)
                return;

            ProcesarCobro(frm.Result);
        }

        private void CobrarRapido(string clave)
        {
            try
            {
                ValidarAntesDeCobrar();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var total = _pos.Totales().Total;
            if (total <= 0)
            {
                MessageBox.Show("No hay nada para cobrar.", "POS", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (!_mediosPagoPorClave.TryGetValue(clave, out var medio))
            {
                MessageBox.Show(
                    $"No existe configuración de forma de pago para '{clave}'.",
                    "POS",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            decimal montoMoneda = total;
            if (clave.Equals("efectivo", StringComparison.OrdinalIgnoreCase))
            {
                var recibido = PedirMontoRecibido(total);
                if (!recibido.HasValue) return;
                montoMoneda = recibido.Value;
            }

            var result = new SeleccionPagoResult { TotalBase = total };
            result.Pagos.Add(new PagoLineaResult
            {
                FormaPagoCodigo = medio.FormaPagoCodigo,
                NombreMedio = medio.Nombre,
                MonedaCodigo = ConfigService.MonedaDefecto,
                TasaCambio = 1m,
                MontoMoneda = montoMoneda
            });

            ProcesarCobro(result);
        }

        private decimal? PedirMontoRecibido(decimal total)
        {
            using var frm = new Form
            {
                Text = "Efectivo recibido",
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                ShowInTaskbar = false,
                MaximizeBox = false,
                MinimizeBox = false,
                ClientSize = new Size(320, 160)
            };

            var lbl = new Label { Text = $"Total a cobrar: RD$ {total:N2}", AutoSize = true, Location = new Point(16, 18) };
            var txt = new TextBox { Text = total.ToString("N2"), Location = new Point(16, 52), Width = 130 };
            var ok = new Button { Text = "Aceptar", DialogResult = DialogResult.OK, Location = new Point(50, 104), Width = 88 };
            var cancel = new Button { Text = "Cancelar", DialogResult = DialogResult.Cancel, Location = new Point(162, 104), Width = 88 };
            frm.Controls.AddRange(new Control[] { lbl, txt, ok, cancel });
            frm.AcceptButton = ok;
            frm.CancelButton = cancel;

            if (frm.ShowDialog(this) != DialogResult.OK)
                return null;

            return decimal.TryParse(txt.Text, out var monto) ? monto : null;
        }

        private void ProcesarCobro(SeleccionPagoResult result)
        {
            try
            {
                if (_pos.Carrito.Count == 0)
                    throw new InvalidOperationException("Carrito vacío.");

                if (result.Pagos.Count == 0)
                    throw new InvalidOperationException("No hay pagos seleccionados.");

                if (result.DiferenciaBase > 0.01m)
                    throw new InvalidOperationException($"Falta por cobrar RD$ {result.DiferenciaBase:N2}.");

                if (_tipoComprobanteActual == null)
                    throw new InvalidOperationException("No hay un tipo de comprobante fiscal seleccionado.");

                ValidarAntesDeCobrar();

                var primerPago = result.Pagos[0];
                var totalesAntes = _pos.Totales();

                string? clienteCodigo = _clienteActual?.Codigo;
                if (string.IsNullOrWhiteSpace(clienteCodigo))
                    throw new InvalidOperationException("Debe seleccionar un cliente válido.");

                var tipoECFId = _tipoComprobanteActual.TipoECFId;

                // 🔥 VALIDACIÓN CLAVE
                ValidarComprobanteContraCliente(tipoECFId);

                var formaPagoFiscal = primerPago.FormaPagoCodigo;
                if (string.IsNullOrWhiteSpace(formaPagoFiscal))
                    throw new InvalidOperationException("Forma de pago fiscal inválida.");

                var tipoPagoECFId = ResolverTipoPagoECFId(formaPagoFiscal);

                var ventaId = _pos.CerrarVenta(
                    usuario: _usuarioPos,
                    clienteCodigo: clienteCodigo,
                    medioPagoId: 0,
                    montoRecibido: result.PagadoBase,
                    moneda: ConfigService.MonedaDefecto,
                    terminoPagoId: 1,
                    posCajaNumero: _cajaNumero,
                    cajaId: _cajaId,
                    tipoECFId: tipoECFId,
                    tipoPagoECFId: tipoPagoECFId,
                    formaPagoFiscal: formaPagoFiscal);

                _pos.GuardarPagosPOS(
                    ventaId,
                    result.Pagos,
                    _usuarioPos,
                    _cajaId,
                    _cajaNumero);

                var fiscal = new PosFiscalService();
                var fiscalResult = fiscal.GenerarFacturaFiscalDesdeVenta(
                    ventaId,
                    tipoECFId,
                    tipoPagoECFId,
                    formaPagoFiscal,
                    _usuarioPos);

                if (fiscalResult == null || fiscalResult.FacturaId <= 0)
                    throw new InvalidOperationException("No se pudo generar la factura fiscal.");

                var ecf = _ecfDocRepo.ObtenerDocumentoPorFactura(fiscalResult.FacturaId);

                _ultimaVentaId = ventaId;
                _ultimaFacturaId = fiscalResult.FacturaId;
                _ventasSesion++;
                _montoSesion += totalesAntes.Total;

                LimpiarVentaInterna();

                MessageBox.Show(
                    $"Venta procesada correctamente.\n" +
                    $"Venta ID: {ventaId}\n" +
                    $"Factura ID: {fiscalResult.FacturaId}\n" +
                    $"e-NCF: {ecf?.ENCF ?? "Pendiente"}",
                    "POS",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"No se pudo cobrar la venta.\n{ex.Message}",
                    "POS",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
                

        private void ValidarComprobanteContraCliente(int tipoECFId)
        {
            var rnc = (_clienteActual?.RncCedula ?? "").Trim();

            var esConsumidorFinal =
                string.IsNullOrWhiteSpace(rnc) ||
                rnc == "000000000" ||
                rnc == "00000000000";

            // E31 - Crédito fiscal
            if (tipoECFId == 1 && esConsumidorFinal)
                throw new InvalidOperationException(
                    "Crédito fiscal (E31) requiere un cliente con RNC/Cédula válido.");

            // E45 - Gubernamental
            if (tipoECFId == 8 && esConsumidorFinal)
                throw new InvalidOperationException(
                    "Comprobante gubernamental (E45) requiere un RNC válido.");

            // E32 - Consumidor final → OK siempre
        }

        private readonly ECFTipoPagoRepository _ecfTipoPagoRepo = new();



        private int ResolverTipoPagoECFId(string formaPagoCodigo)
        {
            if (string.IsNullOrWhiteSpace(formaPagoCodigo))
                throw new InvalidOperationException("La forma de pago fiscal está vacía.");

            var tipos = _ecfTipoPagoRepo.ListarActivos();

            var item = tipos.FirstOrDefault(x =>
                string.Equals(x.CodigoDGII, formaPagoCodigo, StringComparison.OrdinalIgnoreCase));

            if (item == null || item.TipoPagoECFId <= 0)
                throw new InvalidOperationException(
                    $"No existe configuración en ECFTipoPago para la forma de pago fiscal '{formaPagoCodigo}'.");

            return item.TipoPagoECFId;
        }

        private void LimpiarVentaInterna()
        {
            _pos.Carrito.Clear();
            RefrescarVenta();
            txtBuscar.Clear();
            txtBuscar.Focus();
        }

        private void AbrirBusquedaProducto()
        {
            using var frm = new FormBuscarProducto();
            frm.SetFiltroInicial(txtBuscar.Text);
            if (frm.ShowDialog(this) != DialogResult.OK || frm.ProductoSeleccionado == null)
                return;

            AgregarProducto(frm.ProductoSeleccionado.Codigo);
        }

        private void AbrirCaja()
        {
            if (!_puedeCerrarCaja)
            {
                MessageBox.Show("Este usuario no tiene permiso para cierre de caja.", "POS", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            MessageBox.Show("Desde aquí puedes enlazar apertura/cierre usando tus forms actuales de caja.", "POS", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ReimprimirUltimaVenta()
        {
            try
            {
                if (_ultimaFacturaId.HasValue && _ultimaFacturaId.Value > 0)
                {
                    var ecf = _ecfDocRepo.ObtenerDocumentoPorFactura(_ultimaFacturaId.Value);

                    var encf = string.IsNullOrWhiteSpace(ecf?.ENCF) ? "Pendiente" : ecf!.ENCF!;
                    var trackId = string.IsNullOrWhiteSpace(ecf?.TrackId) ? "Pendiente" : ecf!.TrackId!;
                    var estadoDgii = string.IsNullOrWhiteSpace(ecf?.EstadoDGII) ? "N/D" : ecf!.EstadoDGII!;
                    var estadoProceso = string.IsNullOrWhiteSpace(ecf?.EstadoProceso) ? "N/D" : ecf!.EstadoProceso!;

                    MessageBox.Show(
                        $"Reimpresión fiscal preparada.\n" +
                        $"Factura ID: {_ultimaFacturaId.Value}\n" +
                        $"e-NCF: {encf}\n" +
                        $"Estado DGII: {estadoDgii}\n" +
                        $"Estado Proceso: {estadoProceso}\n" +
                        $"TrackId: {trackId}",
                        "POS",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    return;
                }

                if (_ultimaVentaId.HasValue && _ultimaVentaId.Value > 0)
                {
                    MessageBox.Show(
                        $"Reimpresión POS preparada.\nVenta ID: {_ultimaVentaId.Value}",
                        "POS",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    return;
                }

                MessageBox.Show(
                    "No hay una venta o factura reciente para reimprimir.",
                    "POS",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"No se pudo preparar la reimpresión.\n{ex.Message}",
                    "POS",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void MostrarMenuExtra()
        {
            var menu = new ContextMenuStrip();
            menu.Items.Add("Buscar producto", null, (_, __) => AbrirBusquedaProducto());
            menu.Items.Add("Buscar cliente", null, (_, __) => BuscarCliente());
            menu.Items.Add("Limpiar venta", null, (_, __) => LimpiarVentaInterna());
            menu.Items.Add("Abrir facturación", null, (_, __) => MessageBox.Show("Conecta aquí FormFacturaV.", "POS"));
            menu.Show(btnMas, new Point(0, btnMas.Height));
        }

        private void ConstruirAccesosRapidos()
        {
            pnlAccesos.Controls.Clear();
            pnlAccesos.Controls.Add(CreateActionCard("Buscar cliente", "F6", BuscarCliente));
            pnlAccesos.Controls.Add(CreateActionCard("Buscar producto", "F3", AbrirBusquedaProducto));
            pnlAccesos.Controls.Add(CreateActionCard("Pago mixto", "F9", CobrarConPantallaPago));
            pnlAccesos.Controls.Add(CreateActionCard("Limpiar venta", "ESC", LimpiarVentaInterna));
        }

        private void ConstruirResumen()
        {
            pnlResumen.Controls.Clear();
            lblVentasDia = CreateMetricCard("Ventas", "RD$ 0.00");
            lblTransacciones = CreateMetricCard("Transacciones", "0");
            lblTicketPromedio = CreateMetricCard("Ticket promedio", "RD$ 0.00");
            pnlResumen.Controls.Add(lblVentasDia.Parent!);
            pnlResumen.Controls.Add(lblTransacciones.Parent!);
            pnlResumen.Controls.Add(lblTicketPromedio.Parent!);
        }

        private void ActualizarResumenSesion()
        {
            if (lblVentasDia == null || lblTransacciones == null || lblTicketPromedio == null)
                return;

            var actual = _pos.Totales();

            lblVentasDia.Text = $"RD$ {_montoSesion + actual.Total:N2}";
            lblTransacciones.Text = _ventasSesion.ToString("N0");

            var promedio = _ventasSesion <= 0 ? 0m : _montoSesion / _ventasSesion;
            lblTicketPromedio.Text = $"RD$ {promedio:N2}";
        }

        private Label CreateMetricCard(string title, string value)
        {
            var card = CreateCardPanel(Color.FromArgb(248, 250, 252), false);
            card.Width = 182;
            card.Height = 72;
            card.Margin = new Padding(0, 0, 12, 0);
            card.Padding = new Padding(12);

            var lblTitle = new Label
            {
                Text = title,
                Dock = DockStyle.Top,
                Height = 22,
                ForeColor = Color.FromArgb(100, 116, 139)
            };
            card.Controls.Add(lblTitle);

            var lblValue = new Label
            {
                Text = value,
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 12.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42)
            };
            card.Controls.Add(lblValue);
            card.Controls.SetChildIndex(lblTitle, 1);
            return lblValue;
        }

        private Control CreateActionCard(string title, string shortcut, Action onClick)
        {
            var card = CreateCardPanel(Color.FromArgb(248, 250, 252), false);
            card.Width = 154;
            card.Height = 74;
            card.Margin = new Padding(0, 0, 12, 0);
            card.Padding = new Padding(12);
            card.Cursor = Cursors.Hand;

            var lblTitle = new Label
            {
                Text = title,
                Dock = DockStyle.Top,
                Height = 26,
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42)
            };
            var lblShort = new Label
            {
                Text = shortcut,
                Dock = DockStyle.Bottom,
                Height = 20,
                ForeColor = Color.FromArgb(100, 116, 139)
            };
            card.Controls.Add(lblTitle);
            card.Controls.Add(lblShort);
            card.Click += (_, __) => onClick();
            foreach (Control child in card.Controls)
                child.Click += (_, __) => onClick();
            return card;
        }

        private Label CreateValueLine(Control parent, string title, int top)
        {
            var lblTitle = new Label
            {
                Text = title,
                AutoSize = true,
                Location = new Point(0, top),
                ForeColor = Color.FromArgb(100, 116, 139)
            };
            parent.Controls.Add(lblTitle);

            var lblValue = new Label
            {
                Text = "RD$ 0.00",
                AutoSize = true,
                Location = new Point(288, top),
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(17, 24, 39)
            };
            parent.Controls.Add(lblValue);
            return lblValue;
        }

        private Label CreatePillLabel(string text, Color foreColor, Color backColor)
        {
            return new Label
            {
                AutoSize = true,
                Text = text,
                ForeColor = foreColor,
                BackColor = backColor,
                Padding = new Padding(12, 6, 12, 6),
                Font = new Font("Segoe UI", 10f, FontStyle.Bold)
            };
        }

        private Button CreateCategoryChip(string text, int? categoriaId, bool active)
        {
            var btn = new Button
            {
                Text = text,
                Tag = categoriaId,
                Height = 34,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                FlatStyle = FlatStyle.Flat,
                BackColor = active ? Color.FromArgb(13, 177, 146) : Color.White,
                ForeColor = active ? Color.White : Color.FromArgb(30, 41, 59),
                Padding = new Padding(14, 0, 14, 0),
                Margin = new Padding(0, 0, 8, 8)
            };
            btn.FlatAppearance.BorderColor = active ? Color.FromArgb(13, 177, 146) : Color.FromArgb(219, 228, 235);
            btn.Click += (_, __) => SeleccionarCategoria(btn, categoriaId);
            return btn;
        }

        private void SeleccionarCategoria(Button clicked, int? categoriaId)
        {
            _categoriaSeleccionadaId = categoriaId;
            foreach (Button btn in pnlCategorias.Controls.OfType<Button>())
            {
                var active = ReferenceEquals(btn, clicked);
                btn.BackColor = active ? Color.FromArgb(13, 177, 146) : Color.White;
                btn.ForeColor = active ? Color.White : Color.FromArgb(30, 41, 59);
                btn.FlatAppearance.BorderColor = active ? Color.FromArgb(13, 177, 146) : Color.FromArgb(219, 228, 235);
            }
            RenderProductos();
        }

        private Button CreateSideButton(string text, bool active = false)
        {
            var btn = new Button
            {
                Text = text,
                Tag = text,
                Width = 150,
                Height = 42,
                FlatStyle = FlatStyle.Flat,
                BackColor = active ? Color.FromArgb(13, 177, 146) : Color.White,
                ForeColor = active ? Color.White : Color.FromArgb(71, 85, 105),
                Font = new Font("Segoe UI", 10.5f, active ? FontStyle.Bold : FontStyle.Regular),
                Margin = new Padding(0, 0, 0, 10),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(14, 0, 0, 0)
            };
            btn.FlatAppearance.BorderColor = active ? Color.FromArgb(13, 177, 146) : Color.FromArgb(233, 237, 242);
            return btn;
        }

        private Panel CreateCardPanel(Color? backColor = null, bool border = true)
        {
            var pnl = new Panel
            {
                BackColor = backColor ?? Color.White,
                Margin = new Padding(0),
                BorderStyle = border ? BorderStyle.FixedSingle : BorderStyle.None
            };
            return pnl;
        }

        private Button CreatePayButton(string text)
        {
            var btn = new Button
            {
                Text = text,
                Width = 86,
                Height = 42,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White,
                ForeColor = Color.FromArgb(17, 24, 39),
                Margin = new Padding(0, 0, 8, 8)
            };
            btn.FlatAppearance.BorderColor = Color.FromArgb(180, 225, 214);
            return btn;
        }

        private Button CreateSmallActionButton(string text)
        {
            var btn = new Button
            {
                Text = text,
                Width = 118,
                Height = 46,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White,
                ForeColor = Color.FromArgb(17, 24, 39),
                Margin = new Padding(0, 0, 10, 0),
                Font = new Font("Segoe UI", 10f, FontStyle.Bold)
            };
            btn.FlatAppearance.BorderColor = Color.FromArgb(220, 228, 236);
            return btn;
        }

        private static string ObtenerIniciales(string? texto)
        {
            var parts = (texto ?? string.Empty)
                .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                .Take(2)
                .Select(x => char.ToUpperInvariant(x[0]));
            var value = string.Concat(parts);
            return string.IsNullOrWhiteSpace(value) ? "P" : value;
        }
    }
}
