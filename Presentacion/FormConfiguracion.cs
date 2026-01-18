#nullable enable
using Andloe.Data;
using Andloe.Data.DGII;
using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;

namespace Andloe.Presentacion
{
    public partial class FormConfiguracion : Form
    {
        private readonly MonedaRepository _monRepo = new();
        private readonly PosPagoRepository _posPagoRepo = new();
        private readonly SistemaConfigRepository _cfgRepo = new();
        private FormReporteConfig? _frmReporteConfig;

        private const int MAX_LONGITUD = 15;

        // evita que SelectedIndexChanged dispare lógica mientras cargas combos
        private bool _loading = false;

        public FormConfiguracion()
        {
            InitializeComponent();

            // ✅ No uses eventos duplicados (Designer + aquí). Aquí lo amarramos todo por código.
            this.Load += FormConfiguracion_Load;

            cbEmpresa.SelectedIndexChanged += cbEmpresa_SelectedIndexChanged;
            cbSucursal.SelectedIndexChanged += cbSucursal_SelectedIndexChanged;

            // Numeradores
            txtNumProdPrefijo.TextChanged += txtNumerador_TextChanged;
            txtNumProdLongitud.TextChanged += txtNumerador_TextChanged;
            txtNumProdLongitud.KeyPress += txtLongitud_KeyPress;

            txtNumCliPrefijo.TextChanged += txtNumerador_TextChanged;
            txtNumCliLongitud.TextChanged += txtNumerador_TextChanged;
            txtNumCliLongitud.KeyPress += txtLongitud_KeyPress;

            txtNumProvPrefijo.TextChanged += txtNumerador_TextChanged;
            txtNumProvLongitud.TextChanged += txtNumerador_TextChanged;
            txtNumProvLongitud.KeyPress += txtLongitud_KeyPress;

            txtNumFactVenPrefijo.TextChanged += txtNumerador_TextChanged;
            txtNumFactVenLongitud.TextChanged += txtNumerador_TextChanged;
            txtNumFactVenLongitud.KeyPress += txtLongitud_KeyPress;

            txtNumFactComPrefijo.TextChanged += txtNumerador_TextChanged;
            txtNumFactComLongitud.TextChanged += txtNumerador_TextChanged;
            txtNumFactComLongitud.KeyPress += txtLongitud_KeyPress;

            btnGuardar.Click += btnGuardar_Click;
            btnCerrar.Click += btnCerrar_Click;
            btnActualizarDgii.Click += btnActualizarDgii_Click;
        }

        private void FormConfiguracion_Load(object? sender, EventArgs e)
        {
            try
            {
                _loading = true;

                CargarCombos();
                CargarConfigGeneral();
                CargarNumeradores();

                // ✅ monta la pestaña de reportes
                MontarTabReportes();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar configuración: " + ex.Message,
                    "Configuración", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _loading = false;
            }
        }

        // =========================
        //   COMBOS GENERALES
        // =========================
        private void CargarCombos()
        {
            // Monedas
            var monedas = _monRepo.ListarMonedas();
            cbMonedaBase.DisplayMember = "Descripcion";
            cbMonedaBase.ValueMember = "Codigo";
            cbMonedaBase.DataSource = monedas;

            // Medios de pago
            var medios = _posPagoRepo.ListarMediosPago();
            cbMedioPagoDefecto.DisplayMember = "Nombre";
            cbMedioPagoDefecto.ValueMember = "MedioPagoId";
            cbMedioPagoDefecto.DataSource = medios;

            // Contexto (Empresa/Sucursal/Almacén)
            CargarEmpresas();

            // limpia cascada
            cbSucursal.DataSource = null;
            cbAlmacenDefecto.DataSource = null;
            cbAlmacenOrigenPos.DataSource = null;
            cbAlmacenDestinoPos.DataSource = null;
        }

        private void CargarEmpresas()
        {
            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
SELECT EmpresaId, RazonSocial
FROM dbo.Empresa
WHERE Estado = 1
ORDER BY RazonSocial;", cn);

            var dt = new DataTable();
            dt.Load(cmd.ExecuteReader());

            cbEmpresa.DisplayMember = "RazonSocial";
            cbEmpresa.ValueMember = "EmpresaId";
            cbEmpresa.DataSource = dt;
        }

        private void CargarSucursales(int empresaId)
        {
            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
SELECT SucursalId, (Codigo + ' - ' + Nombre) AS NombreMostrar
FROM dbo.Sucursal
WHERE EmpresaId = @emp AND Estado = 1
ORDER BY Codigo, Nombre;", cn);

            cmd.Parameters.Add("@emp", SqlDbType.Int).Value = empresaId;

            var dt = new DataTable();
            dt.Load(cmd.ExecuteReader());

            cbSucursal.DisplayMember = "NombreMostrar";
            cbSucursal.ValueMember = "SucursalId";
            cbSucursal.DataSource = dt;
        }

        private void CargarAlmacenes(int empresaId, int sucursalId)
        {
            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
SELECT AlmacenId, (Codigo + ' - ' + Nombre) AS NombreMostrar
FROM dbo.Almacen
WHERE EmpresaId = @emp AND SucursalId = @suc AND Estado = 1
ORDER BY Codigo, Nombre;", cn);

            cmd.Parameters.Add("@emp", SqlDbType.Int).Value = empresaId;
            cmd.Parameters.Add("@suc", SqlDbType.Int).Value = sucursalId;

            var dt = new DataTable();
            dt.Load(cmd.ExecuteReader());

            cbAlmacenDefecto.DisplayMember = "NombreMostrar";
            cbAlmacenDefecto.ValueMember = "AlmacenId";
            cbAlmacenDefecto.DataSource = dt;

            // dt.Copy() para que cada combo tenga su propia fuente
            cbAlmacenOrigenPos.DisplayMember = "NombreMostrar";
            cbAlmacenOrigenPos.ValueMember = "AlmacenId";
            cbAlmacenOrigenPos.DataSource = dt.Copy();

            cbAlmacenDestinoPos.DisplayMember = "NombreMostrar";
            cbAlmacenDestinoPos.ValueMember = "AlmacenId";
            cbAlmacenDestinoPos.DataSource = dt.Copy();
        }

        private void cbEmpresa_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (_loading) return;

            try
            {
                if (cbEmpresa.SelectedValue == null) return;
                if (!int.TryParse(cbEmpresa.SelectedValue.ToString(), out var empresaId)) return;

                _loading = true;

                cbSucursal.DataSource = null;
                cbAlmacenDefecto.DataSource = null;
                cbAlmacenOrigenPos.DataSource = null;
                cbAlmacenDestinoPos.DataSource = null;

                CargarSucursales(empresaId);
            }
            catch
            {
                // silencioso
            }
            finally
            {
                _loading = false;
            }
        }

        private void cbSucursal_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (_loading) return;

            try
            {
                if (cbEmpresa.SelectedValue == null || cbSucursal.SelectedValue == null) return;
                if (!int.TryParse(cbEmpresa.SelectedValue.ToString(), out var empresaId)) return;
                if (!int.TryParse(cbSucursal.SelectedValue.ToString(), out var sucursalId)) return;

                _loading = true;
                CargarAlmacenes(empresaId, sucursalId);
            }
            catch
            {
                // silencioso
            }
            finally
            {
                _loading = false;
            }
        }

        // =========================
        //   CONFIG GENERAL
        // =========================
        private void CargarConfigGeneral()
        {
            var monedaDef = _cfgRepo.GetValor("MONEDA_DEFECTO") ?? "DOP";
            var clienteDef = _cfgRepo.GetValor("CLIENTE_DEFECTO") ?? "C-000001";
            var medioDef = _cfgRepo.GetValor("MEDIO_PAGO_DEFECTO");
            var stockNegVal = _cfgRepo.GetValor("PERMITIR_STOCK_NEGATIVO") ?? "0";

            var empDef = _cfgRepo.GetValor("EMPRESA_DEFECTO_ID");
            var sucDef = _cfgRepo.GetValor("SUCURSAL_DEFECTO_ID");
            var almDef = _cfgRepo.GetValor("ALMACEN_DEFECTO_ID");

            var almOri = _cfgRepo.GetValor("ALMACEN_POS_ORIGEN_ID");
            var almDes = _cfgRepo.GetValor("ALMACEN_POS_DESTINO_ID");

            txtClienteDefecto.Text = clienteDef;
            cbMonedaBase.SelectedValue = monedaDef;

            if (int.TryParse(medioDef, out var medioId))
            {
                try { cbMedioPagoDefecto.SelectedValue = medioId; } catch { }
            }

            chkPermitirStockNegativo.Checked =
                stockNegVal == "1" ||
                stockNegVal.Equals("true", StringComparison.OrdinalIgnoreCase) ||
                stockNegVal.Equals("si", StringComparison.OrdinalIgnoreCase);

            // Contexto en cascada (sin disparar eventos)
            _loading = true;
            try
            {
                if (int.TryParse(empDef, out var empresaId) && empresaId > 0)
                {
                    try { cbEmpresa.SelectedValue = empresaId; } catch { }

                    CargarSucursales(empresaId);

                    if (int.TryParse(sucDef, out var sucursalId) && sucursalId > 0)
                    {
                        try { cbSucursal.SelectedValue = sucursalId; } catch { }

                        CargarAlmacenes(empresaId, sucursalId);

                        if (int.TryParse(almDef, out var almacenId) && almacenId > 0)
                        {
                            try { cbAlmacenDefecto.SelectedValue = almacenId; } catch { }
                        }
                    }
                }

                if (int.TryParse(almOri, out var idOri))
                {
                    try { cbAlmacenOrigenPos.SelectedValue = idOri; } catch { }
                }
                if (int.TryParse(almDes, out var idDes) && idDes > 0)
                {
                    try { cbAlmacenDestinoPos.SelectedValue = idDes; } catch { }
                }
            }
            finally
            {
                _loading = false;
            }
        }

        private void GuardarConfigGeneral()
        {
            var usuarioWindows = Environment.UserName;

            var moneda = cbMonedaBase.SelectedValue?.ToString() ?? "DOP";
            var cliente = string.IsNullOrWhiteSpace(txtClienteDefecto.Text) ? "C-000001" : txtClienteDefecto.Text.Trim();
            var medioId = cbMedioPagoDefecto.SelectedValue?.ToString() ?? "1";

            var empresaIdTxt = cbEmpresa.SelectedValue?.ToString() ?? "0";
            var sucursalIdTxt = cbSucursal.SelectedValue?.ToString() ?? "0";
            var almacenIdTxt = cbAlmacenDefecto.SelectedValue?.ToString() ?? "0";

            var almOri = cbAlmacenOrigenPos.SelectedValue?.ToString() ?? "0";
            var almDes = cbAlmacenDestinoPos.SelectedValue?.ToString() ?? "0";

            _cfgRepo.SetValor("MONEDA_DEFECTO", moneda, "Moneda por defecto para ventas POS", "GENERAL", usuarioWindows);
            _cfgRepo.SetValor("CLIENTE_DEFECTO", cliente, "Cliente por defecto para POS", "GENERAL", usuarioWindows);
            _cfgRepo.SetValor("MEDIO_PAGO_DEFECTO", medioId, "Medio de pago por defecto para POS", "GENERAL", usuarioWindows);

            _cfgRepo.SetValor("EMPRESA_DEFECTO_ID", empresaIdTxt, "Empresa por defecto del sistema", "GENERAL", usuarioWindows);
            _cfgRepo.SetValor("SUCURSAL_DEFECTO_ID", sucursalIdTxt, "Sucursal por defecto del sistema", "GENERAL", usuarioWindows);
            _cfgRepo.SetValor("ALMACEN_DEFECTO_ID", almacenIdTxt, "Almacén por defecto del sistema", "GENERAL", usuarioWindows);

            _cfgRepo.SetValor("ALMACEN_POS_ORIGEN_ID", almOri, "Almacén de ORIGEN por defecto para POS", "GENERAL", usuarioWindows);
            _cfgRepo.SetValor("ALMACEN_POS_DESTINO_ID", almDes, "Almacén de DESTINO por defecto para POS (0 = ninguno)", "GENERAL", usuarioWindows);

            var permitirNegativo = chkPermitirStockNegativo.Checked ? "1" : "0";
            _cfgRepo.SetValor("PERMITIR_STOCK_NEGATIVO", permitirNegativo,
                "1 = permite vender con stock negativo, 0 = bloquea si no hay existencia suficiente.",
                "BOOL", usuarioWindows);

            // UsuarioContexto
            if (int.TryParse(empresaIdTxt, out var empId) &&
                int.TryParse(sucursalIdTxt, out var sucId) &&
                int.TryParse(almacenIdTxt, out var almId) &&
                empId > 0 && sucId > 0 && almId > 0)
            {
                var usuarioId = ObtenerUsuarioIdActual(usuarioWindows);
                if (usuarioId > 0)
                {
                    UpsertUsuarioContexto(usuarioId, empId, sucId, almId);
                }
            }
        }

        private static int ObtenerUsuarioIdActual(string usuarioWindows)
        {
            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
SELECT TOP(1) UsuarioId
FROM dbo.Usuario
WHERE Usuario = @u;", cn);

            cmd.Parameters.Add("@u", SqlDbType.NVarChar, 50).Value = usuarioWindows;

            var v = cmd.ExecuteScalar();
            if (v == null || v == DBNull.Value) return 0;
            return Convert.ToInt32(v);
        }

        private static void UpsertUsuarioContexto(int usuarioId, int empresaId, int sucursalId, int almacenId)
        {
            using var cn = Db.GetOpenConnection();
            using var tx = cn.BeginTransaction(IsolationLevel.ReadCommitted);

            try
            {
                using (var cmdVal = new SqlCommand(@"
IF NOT EXISTS (
    SELECT 1
    FROM dbo.Almacen
    WHERE AlmacenId = @alm AND EmpresaId = @emp AND SucursalId = @suc AND Estado = 1
)
    THROW 50001, 'El almacén seleccionado no pertenece a la empresa/sucursal seleccionada.', 1;", cn, tx))
                {
                    cmdVal.Parameters.Add("@alm", SqlDbType.Int).Value = almacenId;
                    cmdVal.Parameters.Add("@emp", SqlDbType.Int).Value = empresaId;
                    cmdVal.Parameters.Add("@suc", SqlDbType.Int).Value = sucursalId;
                    cmdVal.ExecuteNonQuery();
                }

                var host = Environment.MachineName;
                var ip = GetLocalIp();
                object ipDb = string.IsNullOrWhiteSpace(ip) ? DBNull.Value : ip;

                using (var cmd = new SqlCommand(@"
IF EXISTS (SELECT 1 FROM dbo.UsuarioContexto WHERE UsuarioId = @uid)
BEGIN
    UPDATE dbo.UsuarioContexto
    SET EmpresaId = @emp,
        SucursalId = @suc,
        AlmacenId = @alm,
        FechaCambio = SYSDATETIME(),
        Host = @host,
        Ip = @ip
    WHERE UsuarioId = @uid;
END
ELSE
BEGIN
    INSERT INTO dbo.UsuarioContexto (UsuarioId, EmpresaId, SucursalId, AlmacenId, FechaCambio, Host, Ip)
    VALUES (@uid, @emp, @suc, @alm, SYSDATETIME(), @host, @ip);
END;", cn, tx))
                {
                    cmd.Parameters.Add("@uid", SqlDbType.Int).Value = usuarioId;
                    cmd.Parameters.Add("@emp", SqlDbType.Int).Value = empresaId;
                    cmd.Parameters.Add("@suc", SqlDbType.Int).Value = sucursalId;
                    cmd.Parameters.Add("@alm", SqlDbType.Int).Value = almacenId;
                    cmd.Parameters.Add("@host", SqlDbType.NVarChar, 80).Value = host;
                    cmd.Parameters.Add("@ip", SqlDbType.VarChar, 45).Value = ipDb;

                    cmd.ExecuteNonQuery();
                }

                tx.Commit();
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }

        private static string GetLocalIp()
        {
            try
            {
                var host = Dns.GetHostName();
                var ips = Dns.GetHostAddresses(host);
                var ipv4 = ips.FirstOrDefault(x => x.AddressFamily == AddressFamily.InterNetwork);
                return ipv4?.ToString() ?? "";
            }
            catch
            {
                return "";
            }
        }

        // =========================
        // NUMERADORES
        // =========================
        private void CargarNumeradores()
        {
            txtNumProdPrefijo.Text = _cfgRepo.GetValor("NUM_PRODUCTO_PREFIJO") ?? "P-";
            txtNumProdLongitud.Text = _cfgRepo.GetValor("NUM_PRODUCTO_LONGITUD") ?? "6";

            txtNumCliPrefijo.Text = _cfgRepo.GetValor("NUM_CLIENTE_PREFIJO") ?? "C-";
            txtNumCliLongitud.Text = _cfgRepo.GetValor("NUM_CLIENTE_LONGITUD") ?? "6";

            txtNumProvPrefijo.Text = _cfgRepo.GetValor("NUM_PROVEEDOR_PREFIJO") ?? "PR-";
            txtNumProvLongitud.Text = _cfgRepo.GetValor("NUM_PROVEEDOR_LONGITUD") ?? "6";

            txtNumFactVenPrefijo.Text = _cfgRepo.GetValor("NUM_FACTVENTA_PREFIJO") ?? "V";
            txtNumFactVenLongitud.Text = _cfgRepo.GetValor("NUM_FACTVENTA_LONGITUD") ?? "8";

            txtNumFactComPrefijo.Text = _cfgRepo.GetValor("NUM_FACTCOMPRA_PREFIJO") ?? "FC-";
            txtNumFactComLongitud.Text = _cfgRepo.GetValor("NUM_FACTCOMPRA_LONGITUD") ?? "8";

            ActualizarPreviewsNumeradores();
        }

        private bool ValidarNumeradores()
        {
            bool ValidarLongitud(TextBox txt, string etiqueta)
            {
                if (!int.TryParse(txt.Text.Trim(), out var valor))
                {
                    MessageBox.Show($"La longitud de '{etiqueta}' debe ser un número entero.",
                        "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txt.Focus(); txt.SelectAll();
                    return false;
                }

                if (valor < 1 || valor > MAX_LONGITUD)
                {
                    MessageBox.Show($"La longitud de '{etiqueta}' debe estar entre 1 y {MAX_LONGITUD}.",
                        "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txt.Focus(); txt.SelectAll();
                    return false;
                }
                return true;
            }

            if (!ValidarLongitud(txtNumProdLongitud, "Producto")) return false;
            if (!ValidarLongitud(txtNumCliLongitud, "Cliente")) return false;
            if (!ValidarLongitud(txtNumProvLongitud, "Proveedor")) return false;
            if (!ValidarLongitud(txtNumFactVenLongitud, "Factura de Venta")) return false;
            if (!ValidarLongitud(txtNumFactComLongitud, "Factura de Compra")) return false;

            return true;
        }

        private void GuardarNumeradores()
        {
            var usuario = Environment.UserName;
            const string tipo = "NUMERADOR";

            static string P(string? txt) => (txt ?? "").Trim();

            _cfgRepo.SetValor("NUM_PRODUCTO_PREFIJO", P(txtNumProdPrefijo.Text), "Prefijo numerador de Producto", tipo, usuario);
            _cfgRepo.SetValor("NUM_PRODUCTO_LONGITUD", P(txtNumProdLongitud.Text), "Longitud numerador de Producto", tipo, usuario);

            _cfgRepo.SetValor("NUM_CLIENTE_PREFIJO", P(txtNumCliPrefijo.Text), "Prefijo numerador de Cliente", tipo, usuario);
            _cfgRepo.SetValor("NUM_CLIENTE_LONGITUD", P(txtNumCliLongitud.Text), "Longitud numerador de Cliente", tipo, usuario);

            _cfgRepo.SetValor("NUM_PROVEEDOR_PREFIJO", P(txtNumProvPrefijo.Text), "Prefijo numerador de Proveedor", tipo, usuario);
            _cfgRepo.SetValor("NUM_PROVEEDOR_LONGITUD", P(txtNumProvLongitud.Text), "Longitud numerador de Proveedor", tipo, usuario);

            _cfgRepo.SetValor("NUM_FACTVENTA_PREFIJO", P(txtNumFactVenPrefijo.Text), "Prefijo numerador Factura de Venta", tipo, usuario);
            _cfgRepo.SetValor("NUM_FACTVENTA_LONGITUD", P(txtNumFactVenLongitud.Text), "Longitud numerador Factura de Venta", tipo, usuario);

            _cfgRepo.SetValor("NUM_FACTCOMPRA_PREFIJO", P(txtNumFactComPrefijo.Text), "Prefijo numerador Factura de Compra", tipo, usuario);
            _cfgRepo.SetValor("NUM_FACTCOMPRA_LONGITUD", P(txtNumFactComLongitud.Text), "Longitud numerador Factura de Compra", tipo, usuario);
        }

        private void ActualizarPreviewsNumeradores()
        {
            lblPreviewProd.Text = CrearPreview(txtNumProdPrefijo, txtNumProdLongitud, "P-", 6);
            lblPreviewCli.Text = CrearPreview(txtNumCliPrefijo, txtNumCliLongitud, "C-", 6);
            lblPreviewProv.Text = CrearPreview(txtNumProvPrefijo, txtNumProvLongitud, "PR-", 6);
            lblPreviewFactVen.Text = CrearPreview(txtNumFactVenPrefijo, txtNumFactVenLongitud, "V", 8);
            lblPreviewFactCom.Text = CrearPreview(txtNumFactComPrefijo, txtNumFactComLongitud, "FC-", 8);
        }

        private static string CrearPreview(TextBox txtPrefijo, TextBox txtLongitud, string prefijoFallback, int longitudFallback)
        {
            var pref = (txtPrefijo.Text ?? "").Trim();
            if (string.IsNullOrEmpty(pref)) pref = prefijoFallback;

            if (!int.TryParse((txtLongitud.Text ?? "").Trim(), out var len) || len < 1)
                len = longitudFallback;

            if (len > MAX_LONGITUD) len = MAX_LONGITUD;

            var num = 1.ToString().PadLeft(len, '0');
            return $"Ej: {pref}{num}";
        }

        // =========================
        // TAB REPORTES
        // =========================
        private void MontarTabReportes()
        {
            // ✅ ya existe por designer: tabMain + tpReportes
            if (_frmReporteConfig != null && !_frmReporteConfig.IsDisposed)
                return;

            _frmReporteConfig = new FormReporteConfig
            {
                TopLevel = false,
                FormBorderStyle = FormBorderStyle.None,
                Dock = DockStyle.Fill
            };

            tpReportes.Controls.Clear();
            tpReportes.Controls.Add(_frmReporteConfig);
            _frmReporteConfig.Show();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            try
            {
                if (_frmReporteConfig != null && !_frmReporteConfig.IsDisposed)
                    _frmReporteConfig.Dispose();
            }
            catch { }

            base.OnFormClosed(e);
        }

        private void txtNumerador_TextChanged(object? sender, EventArgs e) => ActualizarPreviewsNumeradores();

        private void txtLongitud_KeyPress(object? sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar)) return;
            if (!char.IsDigit(e.KeyChar)) e.Handled = true;
        }

        // =========================
        // BOTONES
        // =========================
        private void btnGuardar_Click(object? sender, EventArgs e)
        {
            try
            {
                if (!ValidarNumeradores()) return;

                GuardarConfigGeneral();
                GuardarNumeradores();

                MessageBox.Show("Configuración guardada correctamente.",
                    "Configuración", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar configuración: " + ex.Message,
                    "Configuración", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCerrar_Click(object? sender, EventArgs e) => Close();

        private void btnActualizarDgii_Click(object? sender, EventArgs e)
        {
            try
            {
                using var ofd = new OpenFileDialog
                {
                    Title = "Selecciona DGII_RNC.TXT",
                    Filter = "TXT (*.txt)|*.txt|Todos (*.*)|*.*",
                    Multiselect = false
                };

                if (ofd.ShowDialog() != DialogResult.OK) return;

                var svc = new DgiiImportService("IGNORADO");
                var datasetId = svc.ImportarDesdeTxt(ofd.FileName, skipIfSameSha: true);

                MessageBox.Show($"DGII cargado OK. DatasetId={datasetId}", "DGII",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "DGII ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
#nullable restore
