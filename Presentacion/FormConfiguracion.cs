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
using Andloe.Entidad;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace Andloe.Presentacion
{
    public partial class FormConfiguracion : Form
    {
        private readonly MonedaRepository _monRepo = new();
        private readonly PosPagoRepository _posPagoRepo = new();
        private readonly SistemaConfigRepository _cfgRepo = new();
        private FormReporteConfig? _frmReporteConfig;

        private const int MAX_LONGITUD = 15;
        private bool _loading = false;

        public FormConfiguracion()
        {
            InitializeComponent();

            CargarComboAlanubeAmbiente();

            Load += FormConfiguracion_Load;

            cbEmpresa.SelectedIndexChanged += cbEmpresa_SelectedIndexChanged;
            cbSucursal.SelectedIndexChanged += cbSucursal_SelectedIndexChanged;

            HookNumerador(txtNumProdPrefijo, txtNumProdLongitud, txtNumProdActual);
            HookNumerador(txtNumCliPrefijo, txtNumCliLongitud, txtNumCliActual);
            HookNumerador(txtNumProvPrefijo, txtNumProvLongitud, txtNumProvActual);
            HookNumerador(txtNumFactVenPrefijo, txtNumFactVenLongitud, txtNumFactVenActual);
            HookNumerador(txtNumFactComPrefijo, txtNumFactComLongitud, txtNumFactComActual);
            HookNumerador(txtNumNcVenPrefijo, txtNumNcVenLongitud, txtNumNcVenActual);

            btnGuardar.Click += btnGuardar_Click;
            btnCerrar.Click += btnCerrar_Click;
            btnActualizarDgii.Click += btnActualizarDgii_Click;
            btnProbarAlanube.Click += btnProbarAlanube_Click;
        }

        private void HookNumerador(TextBox txtPrefijo, TextBox txtLongitud, TextBox txtActual)
        {
            txtPrefijo.TextChanged += txtNumerador_TextChanged;
            txtLongitud.TextChanged += txtNumerador_TextChanged;
            txtActual.TextChanged += txtNumerador_TextChanged;

            txtLongitud.KeyPress += txtSoloNumeros_KeyPress;
            txtActual.KeyPress += txtSoloNumeros_KeyPress;
        }

        private void FormConfiguracion_Load(object? sender, EventArgs e)
        {
            try
            {
                _loading = true;

                CargarCombos();
                CargarConfigGeneral();
                CargarNumeradores();
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

        private void CargarComboAlanubeAmbiente()
        {
            cbAlanubeAmbiente.BeginUpdate();
            try
            {
                cbAlanubeAmbiente.Items.Clear();
                cbAlanubeAmbiente.Items.Add("sandbox");
                cbAlanubeAmbiente.Items.Add("production");

                cbAlanubeAmbiente.SelectedIndex = cbAlanubeAmbiente.Items.Count > 0 ? 0 : -1;
            }
            finally
            {
                cbAlanubeAmbiente.EndUpdate();
            }
        }

        // =========================
        //   COMBOS GENERALES
        // =========================
        private void CargarCombos()
        {
            var monedas = _monRepo.ListarMonedas();
            cbMonedaBase.DisplayMember = "Descripcion";
            cbMonedaBase.ValueMember = "Codigo";
            cbMonedaBase.DataSource = monedas;

            var medios = _posPagoRepo.ListarMediosPago();
            cbMedioPagoDefecto.DisplayMember = "Nombre";
            cbMedioPagoDefecto.ValueMember = "MedioPagoId";
            cbMedioPagoDefecto.DataSource = medios;

            CargarEmpresas();

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

            var alanubeBaseUrl = _cfgRepo.GetValor("ALANUBE_BASE_URL") ?? "";
            var alanubeToken = (_cfgRepo.GetValor("ALANUBE_TOKEN") ?? "").Trim();

            if (alanubeToken.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                alanubeToken = alanubeToken.Substring(7).Trim();
            var alanubeAmbiente = (_cfgRepo.GetValor("ALANUBE_AMBIENTE") ?? "sandbox").Trim().ToLowerInvariant();
            var alanubeTimeout = _cfgRepo.GetValor("ALANUBE_TIMEOUT_SEGUNDOS") ?? "60";
            var alanubeIdCompany = _cfgRepo.GetValor("ALANUBE_ID_COMPANY") ?? "";
            var alanubeRetryNumber = _cfgRepo.GetValor("ALANUBE_RETRY_NUMBER") ?? "1";

            txtAlanubeBaseUrl.Text = alanubeBaseUrl;
            txtAlanubeToken.Text = alanubeToken;
            txtAlanubeTimeout.Text = alanubeTimeout;
            txtAlanubeIdCompany.Text = alanubeIdCompany;
            txtAlanubeRetryNumber.Text = alanubeRetryNumber;

            if (cbAlanubeAmbiente.Items.Count == 0)
                CargarComboAlanubeAmbiente();

            if (cbAlanubeAmbiente.Items.Count > 0)
            {
                var idx = cbAlanubeAmbiente.FindStringExact(
                    alanubeAmbiente == "production" ? "production" : "sandbox");

                cbAlanubeAmbiente.SelectedIndex = idx >= 0 ? idx : 0;
            }
            else
            {
                cbAlanubeAmbiente.SelectedIndex = -1;
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

            var alanubeBaseUrl = (txtAlanubeBaseUrl.Text ?? "").Trim();
            var alanubeToken = (txtAlanubeToken.Text ?? "").Trim();

            if (alanubeToken.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                alanubeToken = alanubeToken.Substring(7).Trim();

            var alanubeAmbiente = "sandbox";
            if (cbAlanubeAmbiente.Items.Count > 0 && cbAlanubeAmbiente.SelectedItem != null)
                alanubeAmbiente = (cbAlanubeAmbiente.SelectedItem.ToString() ?? "sandbox").Trim().ToLowerInvariant();

            var alanubeTimeout = (txtAlanubeTimeout.Text ?? "").Trim();

            var alanubeIdCompany = (txtAlanubeIdCompany.Text ?? "").Trim();
            if (!string.IsNullOrWhiteSpace(alanubeBaseUrl) &&
                string.IsNullOrWhiteSpace(alanubeIdCompany))
            {
                throw new Exception("Debes indicar el Id Company de Alanube.");
            }

            var alanubeRetryNumber = (txtAlanubeRetryNumber.Text ?? "").Trim();
            if (!string.IsNullOrWhiteSpace(alanubeRetryNumber) &&
                (!int.TryParse(alanubeRetryNumber, out var retry) || retry <= 0))
            {
                throw new Exception("Retry Number debe ser un entero mayor que 0.");
            }


            if (!string.IsNullOrWhiteSpace(alanubeBaseUrl) &&
                !Uri.TryCreate(alanubeBaseUrl, UriKind.Absolute, out _))
            {
                throw new Exception("La Base URL de Alanube no es válida.");
            }

            if (!string.IsNullOrWhiteSpace(alanubeTimeout) &&
                (!int.TryParse(alanubeTimeout, out var timeoutSeg) || timeoutSeg <= 0 || timeoutSeg > 300))
            {
                throw new Exception("El timeout de Alanube debe ser un número entre 1 y 300.");
            }

            if (!string.IsNullOrWhiteSpace(alanubeBaseUrl) &&
                string.IsNullOrWhiteSpace(alanubeToken))
            {
                throw new Exception("Debes indicar el token de Alanube.");
            }

            if (alanubeAmbiente != "sandbox" && alanubeAmbiente != "production")
                alanubeAmbiente = "sandbox";

            _cfgRepo.SetValor("MONEDA_DEFECTO", moneda, "Moneda por defecto para ventas POS", "GENERAL", usuarioWindows);
            _cfgRepo.SetValor("CLIENTE_DEFECTO", cliente, "Cliente por defecto para POS", "GENERAL", usuarioWindows);
            _cfgRepo.SetValor("MEDIO_PAGO_DEFECTO", medioId, "Medio de pago por defecto para POS", "GENERAL", usuarioWindows);

            _cfgRepo.SetValor("EMPRESA_DEFECTO_ID", empresaIdTxt, "Empresa por defecto del sistema", "GENERAL", usuarioWindows);
            _cfgRepo.SetValor("SUCURSAL_DEFECTO_ID", sucursalIdTxt, "Sucursal por defecto del sistema", "GENERAL", usuarioWindows);
            _cfgRepo.SetValor("ALMACEN_DEFECTO_ID", almacenIdTxt, "Almacén por defecto del sistema", "GENERAL", usuarioWindows);

            _cfgRepo.SetValor("ALMACEN_POS_ORIGEN_ID", almOri, "Almacén de ORIGEN por defecto para POS", "GENERAL", usuarioWindows);
            _cfgRepo.SetValor("ALMACEN_POS_DESTINO_ID", almDes, "Almacén de DESTINO por defecto para POS (0 = ninguno)", "GENERAL", usuarioWindows);

            _cfgRepo.SetValor("ALANUBE_BASE_URL", alanubeBaseUrl, "Base URL Alanube DOM", "GENERAL", usuarioWindows);
            _cfgRepo.SetValor("ALANUBE_TOKEN", alanubeToken, "Token Bearer Alanube", "GENERAL", usuarioWindows);
            _cfgRepo.SetValor("ALANUBE_AMBIENTE", alanubeAmbiente, "Ambiente Alanube", "GENERAL", usuarioWindows);
            _cfgRepo.SetValor(
                "ALANUBE_TIMEOUT_SEGUNDOS",
                string.IsNullOrWhiteSpace(alanubeTimeout) ? "60" : alanubeTimeout,
                "Timeout HTTP Alanube",
                "GENERAL",
                usuarioWindows);
            _cfgRepo.SetValor(
                "ALANUBE_ID_COMPANY",
                alanubeIdCompany,
                "Id Company Alanube",
                "GENERAL",
                usuarioWindows);
            _cfgRepo.SetValor(
                "ALANUBE_RETRY_NUMBER",
                string.IsNullOrWhiteSpace(alanubeRetryNumber) ? "1" : alanubeRetryNumber,
                "Retry Number Alanube Sandbox",
                "GENERAL",
                usuarioWindows);

            _cfgRepo.SetValor("ALANUBE_ID_COMPANY", alanubeIdCompany, "Id Company Alanube", "GENERAL", usuarioWindows);
            _cfgRepo.SetValor(
                "ALANUBE_RETRY_NUMBER",
                string.IsNullOrWhiteSpace(alanubeRetryNumber) ? "1" : alanubeRetryNumber,
                "Retry Number Alanube Sandbox",
                "GENERAL",
                usuarioWindows);

            var permitirNegativo = chkPermitirStockNegativo.Checked ? "1" : "0";
            _cfgRepo.SetValor("PERMITIR_STOCK_NEGATIVO", permitirNegativo,
                "1 = permite vender con stock negativo, 0 = bloquea si no hay existencia suficiente.",
                "BOOL", usuarioWindows);
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
            CargarNumeradorDesdeFuente(new[] { "PROD", "PRODUCTO" },
                txtNumProdPrefijo, txtNumProdLongitud, txtNumProdActual,
                "NUM_PRODUCTO_PREFIJO", "NUM_PRODUCTO_LONGITUD",
                "P-", "6");

            CargarNumeradorDesdeFuente(new[] { "CLI", "CLIENTE" },
                txtNumCliPrefijo, txtNumCliLongitud, txtNumCliActual,
                "NUM_CLIENTE_PREFIJO", "NUM_CLIENTE_LONGITUD",
                "C", "6");

            CargarNumeradorDesdeFuente(new[] { "PROV", "PROVEEDOR" },
                txtNumProvPrefijo, txtNumProvLongitud, txtNumProvActual,
                "NUM_PROVEEDOR_PREFIJO", "NUM_PROVEEDOR_LONGITUD",
                "PR", "6");

            CargarNumeradorDesdeFuente(new[] { "VENTA", "VC", "FACTVENTA" },
                txtNumFactVenPrefijo, txtNumFactVenLongitud, txtNumFactVenActual,
                "NUM_FACTVENTA_PREFIJO", "NUM_FACTVENTA_LONGITUD",
                "V", "8");

            CargarNumeradorDesdeFuente(new[] { "FAC", "FC", "FACTCOMPRA" },
                txtNumFactComPrefijo, txtNumFactComLongitud, txtNumFactComActual,
                "NUM_FACTCOMPRA_PREFIJO", "NUM_FACTCOMPRA_LONGITUD",
                "F-", "6");

            CargarNumeradorDesdeFuente(new[] { "NCV", "NC", "NOTACREDITO", "NCR" },
                txtNumNcVenPrefijo, txtNumNcVenLongitud, txtNumNcVenActual,
                "NUM_NCV_PREFIJO", "NUM_NCV_LONGITUD",
                "NC", "8");

            ActualizarPreviewsNumeradores();
        }

        private void CargarNumeradorDesdeFuente(
            string[] codigos,
            TextBox txtPrefijo,
            TextBox txtLongitud,
            TextBox txtActual,
            string keyPrefijoCfg,
            string keyLongitudCfg,
            string prefijoDefault,
            string longitudDefault)
        {
            var tupla = ObtenerNumeradorSecuencia(codigos);

            if (tupla.HasValue)
            {
                txtPrefijo.Text = string.IsNullOrWhiteSpace(tupla.Value.Prefijo) ? prefijoDefault : tupla.Value.Prefijo;
                txtLongitud.Text = tupla.Value.Longitud <= 0 ? longitudDefault : tupla.Value.Longitud.ToString();
                txtActual.Text = tupla.Value.Actual < 0 ? "0" : tupla.Value.Actual.ToString();
                return;
            }

            txtPrefijo.Text = _cfgRepo.GetValor(keyPrefijoCfg) ?? prefijoDefault;
            txtLongitud.Text = _cfgRepo.GetValor(keyLongitudCfg) ?? longitudDefault;
            txtActual.Text = "0";
        }

        private (string Prefijo, int Longitud, int Actual, string Codigo)? ObtenerNumeradorSecuencia(string[] codigos)
        {
            using var cn = Db.GetOpenConnection();

            var inSql = string.Join(",", codigos.Select((_, i) => "@c" + i));
            var orderCase = string.Join(" ", codigos.Select((c, i) => $"WHEN @c{i} THEN {i + 1}"));

            using var cmd = new SqlCommand($@"
IF OBJECT_ID('dbo.NumeradorSecuencia') IS NULL
BEGIN
    SELECT CAST(NULL AS varchar(20)) AS Codigo,
           CAST(NULL AS varchar(20)) AS Prefijo,
           CAST(NULL AS int) AS Longitud,
           CAST(NULL AS int) AS Actual
    WHERE 1 = 0;
    RETURN;
END

SELECT TOP(1)
    Codigo,
    ISNULL(Prefijo,'') AS Prefijo,
    ISNULL(Longitud,0) AS Longitud,
    ISNULL(Actual,0) AS Actual
FROM dbo.NumeradorSecuencia
WHERE Codigo IN ({inSql})
ORDER BY CASE Codigo {orderCase} ELSE 999 END;", cn);

            for (int i = 0; i < codigos.Length; i++)
                cmd.Parameters.Add("@c" + i, SqlDbType.VarChar, 20).Value = codigos[i];

            using var rd = cmd.ExecuteReader();
            if (!rd.Read()) return null;

            return (
                rd.IsDBNull(1) ? "" : rd.GetString(1),
                rd.IsDBNull(2) ? 0 : Convert.ToInt32(rd.GetValue(2)),
                rd.IsDBNull(3) ? 0 : Convert.ToInt32(rd.GetValue(3)),
                rd.IsDBNull(0) ? "" : rd.GetString(0)
            );
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

            bool ValidarActual(TextBox txt, string etiqueta)
            {
                if (!int.TryParse(txt.Text.Trim(), out var valor))
                {
                    MessageBox.Show($"El valor actual de '{etiqueta}' debe ser un número entero.",
                        "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txt.Focus(); txt.SelectAll();
                    return false;
                }

                if (valor < 0)
                {
                    MessageBox.Show($"El valor actual de '{etiqueta}' no puede ser negativo.",
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
            if (!ValidarLongitud(txtNumNcVenLongitud, "Nota de Crédito Venta")) return false;

            if (!ValidarActual(txtNumProdActual, "Producto")) return false;
            if (!ValidarActual(txtNumCliActual, "Cliente")) return false;
            if (!ValidarActual(txtNumProvActual, "Proveedor")) return false;
            if (!ValidarActual(txtNumFactVenActual, "Factura de Venta")) return false;
            if (!ValidarActual(txtNumFactComActual, "Factura de Compra")) return false;
            if (!ValidarActual(txtNumNcVenActual, "Nota de Crédito Venta")) return false;

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

            _cfgRepo.SetValor("NUM_NCV_PREFIJO", P(txtNumNcVenPrefijo.Text), "Prefijo numerador Nota de Crédito Venta", tipo, usuario);
            _cfgRepo.SetValor("NUM_NCV_LONGITUD", P(txtNumNcVenLongitud.Text), "Longitud numerador Nota de Crédito Venta", tipo, usuario);

            UpsertNumeradorSecuencia(new[] { "PROD", "PRODUCTO" }, P(txtNumProdPrefijo.Text), ParseInt(txtNumProdLongitud.Text, 6), ParseInt(txtNumProdActual.Text, 0));
            UpsertNumeradorSecuencia(new[] { "CLI", "CLIENTE" }, P(txtNumCliPrefijo.Text), ParseInt(txtNumCliLongitud.Text, 6), ParseInt(txtNumCliActual.Text, 0));
            UpsertNumeradorSecuencia(new[] { "PROV", "PROVEEDOR" }, P(txtNumProvPrefijo.Text), ParseInt(txtNumProvLongitud.Text, 6), ParseInt(txtNumProvActual.Text, 0));
            UpsertNumeradorSecuencia(new[] { "VENTA", "VC", "FACTVENTA" }, P(txtNumFactVenPrefijo.Text), ParseInt(txtNumFactVenLongitud.Text, 8), ParseInt(txtNumFactVenActual.Text, 0));
            UpsertNumeradorSecuencia(new[] { "FAC", "FC", "FACTCOMPRA" }, P(txtNumFactComPrefijo.Text), ParseInt(txtNumFactComLongitud.Text, 6), ParseInt(txtNumFactComActual.Text, 0));
            UpsertNumeradorSecuencia(new[] { "NCV", "NC", "NOTACREDITO", "NCR" }, P(txtNumNcVenPrefijo.Text), ParseInt(txtNumNcVenLongitud.Text, 8), ParseInt(txtNumNcVenActual.Text, 0));
        }

        private static int ParseInt(string? text, int fallback)
        {
            return int.TryParse((text ?? "").Trim(), out var n) && n >= 0 ? n : fallback;
        }

        private void UpsertNumeradorSecuencia(string[] codigos, string prefijo, int longitud, int actual)
        {
            using var cn = Db.GetOpenConnection();

            var existente = ObtenerNumeradorSecuencia(codigos);
            var codigoGuardar = existente?.Codigo ?? codigos[0];

            using var cmd = new SqlCommand(@"
IF OBJECT_ID('dbo.NumeradorSecuencia') IS NULL
    RETURN;

IF EXISTS (SELECT 1 FROM dbo.NumeradorSecuencia WHERE Codigo = @Codigo)
BEGIN
    UPDATE dbo.NumeradorSecuencia
       SET Prefijo = @Prefijo,
           Longitud = @Longitud,
           Actual = @Actual
     WHERE Codigo = @Codigo;
END
ELSE
BEGIN
    INSERT INTO dbo.NumeradorSecuencia (Codigo, Prefijo, Longitud, Actual)
    VALUES (@Codigo, @Prefijo, @Longitud, @Actual);
END;", cn);

            cmd.Parameters.Add("@Codigo", SqlDbType.VarChar, 20).Value = codigoGuardar;
            cmd.Parameters.Add("@Prefijo", SqlDbType.VarChar, 20).Value = prefijo;
            cmd.Parameters.Add("@Longitud", SqlDbType.Int).Value = longitud;
            cmd.Parameters.Add("@Actual", SqlDbType.Int).Value = actual;

            cmd.ExecuteNonQuery();
        }

        private void ActualizarPreviewsNumeradores()
        {
            lblPreviewProd.Text = CrearPreview(txtNumProdPrefijo, txtNumProdLongitud, txtNumProdActual, "P-", 6);
            lblPreviewCli.Text = CrearPreview(txtNumCliPrefijo, txtNumCliLongitud, txtNumCliActual, "C", 6);
            lblPreviewProv.Text = CrearPreview(txtNumProvPrefijo, txtNumProvLongitud, txtNumProvActual, "PR", 6);
            lblPreviewFactVen.Text = CrearPreview(txtNumFactVenPrefijo, txtNumFactVenLongitud, txtNumFactVenActual, "V", 8);
            lblPreviewFactCom.Text = CrearPreview(txtNumFactComPrefijo, txtNumFactComLongitud, txtNumFactComActual, "F-", 6);
            lblPreviewNcVen.Text = CrearPreview(txtNumNcVenPrefijo, txtNumNcVenLongitud, txtNumNcVenActual, "NC", 8);
        }

        private static string CrearPreview(TextBox txtPrefijo, TextBox txtLongitud, TextBox txtActual, string prefijoFallback, int longitudFallback)
        {
            var pref = (txtPrefijo.Text ?? "").Trim();
            if (string.IsNullOrEmpty(pref)) pref = prefijoFallback;

            if (!int.TryParse((txtLongitud.Text ?? "").Trim(), out var len) || len < 1)
                len = longitudFallback;

            if (len > MAX_LONGITUD) len = MAX_LONGITUD;

            if (!int.TryParse((txtActual.Text ?? "").Trim(), out var actual) || actual < 0)
                actual = 0;

            var siguiente = actual + 1;
            var num = siguiente.ToString().PadLeft(len, '0');
            return $"Próximo: {pref}{num}";
        }

        // =========================
        // TAB REPORTES
        // =========================
        private void MontarTabReportes()
        {
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

        private void txtSoloNumeros_KeyPress(object? sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar)) return;
            if (!char.IsDigit(e.KeyChar)) e.Handled = true;
        }

        // =========================
        // BOTONES
        // =========================

        private async void btnProbarAlanube_Click(object? sender, EventArgs e)
        {
            try
            {
                var baseUrl = (txtAlanubeBaseUrl.Text ?? "").Trim().TrimEnd('/');
                var token = (txtAlanubeToken.Text ?? "").Trim();

                if (token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                    token = token.Substring(7).Trim();
                var idCompany = (txtAlanubeIdCompany.Text ?? "").Trim();
                var retryTxt = (txtAlanubeRetryNumber.Text ?? "").Trim();

                if (string.IsNullOrWhiteSpace(baseUrl))
                    throw new Exception("Debes indicar la Base URL de Alanube.");

                if (!Uri.TryCreate(baseUrl, UriKind.Absolute, out _))
                    throw new Exception("La Base URL de Alanube no es válida.");

                if (string.IsNullOrWhiteSpace(token))
                    throw new Exception("Debes indicar el Bearer Token de Alanube.");

                if (string.IsNullOrWhiteSpace(idCompany))
                    throw new Exception("Debes indicar el Id Company de Alanube.");

                if (!int.TryParse(string.IsNullOrWhiteSpace(retryTxt) ? "1" : retryTxt, out var retryNumber) || retryNumber <= 0)
                    throw new Exception("Retry Number debe ser un entero mayor que 0.");

                btnProbarAlanube.Enabled = false;
                Cursor = Cursors.WaitCursor;

                var req = new AlanubeSetTestsRequestDto
                {
                    IdCompany = idCompany,
                    RetryNumber = retryNumber,
                    ItemExample = new AlanubeSetTestsItemDto
                    {
                        BillingIndicator = 1,
                        GoodServiceIndicator = 1,
                        ItemName = "caja de madera",
                        ItemDescription = "Fabricado con madera de arce canadience",
                        UnitPriceItem = 250m
                    }
                };

                var json = JsonSerializer.Serialize(req, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                using var http = new HttpClient
                {
                    Timeout = TimeSpan.FromSeconds(
                        int.TryParse((txtAlanubeTimeout.Text ?? "").Trim(), out var timeoutSeg) && timeoutSeg > 0
                            ? timeoutSeg
                            : 60)
                };

                http.DefaultRequestHeaders.Clear();
                http.DefaultRequestHeaders.Add("accept", "application/json");
                http.DefaultRequestHeaders.Add("authorization", "Bearer " + token);

                var url = baseUrl + "/set-tests";
                using var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await http.PostAsync(url, content);
                var raw = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                    throw new Exception($"Alanube devolvió {(int)response.StatusCode}: {raw}");

                var parsed = JsonSerializer.Deserialize<AlanubeSetTestsResponseDto>(
                    raw,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                var mensaje =
                    "Prueba Alanube ejecutada correctamente." + Environment.NewLine + Environment.NewLine +
                    $"Id: {parsed?.Id ?? ""}" + Environment.NewLine +
                    $"Status: {parsed?.Status ?? ""}" + Environment.NewLine +
                    $"IdCompany: {parsed?.IdCompany ?? ""}" + Environment.NewLine +
                    $"CompanyIdentification: {parsed?.CompanyIdentification ?? ""}" + Environment.NewLine +
                    $"CreationDate: {parsed?.CreationDate ?? ""}";

                MessageBox.Show(
                    mensaje,
                    "Alanube Sandbox",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error probando Alanube: " + ex.Message,
                    "Alanube Sandbox",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
                btnProbarAlanube.Enabled = true;
            }
        }


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

        private void BtnPostulacion_Click(object sender, EventArgs e)
        {
            FormPostulacionDGII myForm = new FormPostulacionDGII();
            this.Hide();
            myForm.ShowDialog();
            this.Close();
        }
    }

    internal sealed class AlanubeSetTestsRequestDto
    {
        public string IdCompany { get; set; } = "";
        public int RetryNumber { get; set; } = 1;
        public AlanubeSetTestsItemDto ItemExample { get; set; } = new();
    }

    internal sealed class AlanubeSetTestsItemDto
    {
        public int BillingIndicator { get; set; }
        public int GoodServiceIndicator { get; set; }
        public string ItemName { get; set; } = "";
        public string ItemDescription { get; set; } = "";
        public decimal UnitPriceItem { get; set; }
    }

    internal sealed class AlanubeSetTestsResponseDto
    {
        public string? Id { get; set; }
        public string? Status { get; set; }
        public string? IdCompany { get; set; }
        public string? CompanyIdentification { get; set; }
        public string? CreationDate { get; set; }
    }

}
#nullable restore