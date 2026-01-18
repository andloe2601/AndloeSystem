using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using Andloe.Data;
using Andloe.Entidad;

namespace Presentation
{
    public partial class FormSeleccionPago : Form
    {
        private readonly decimal _totalBase;
        private readonly string _monedaBase;

        // Lista interna de pagos que el usuario va agregando
        private readonly List<PagoLineaResult> _pagos = new();

        // Resultado que se devolverá al POS
        public SeleccionPagoResult? Result { get; private set; }

        public FormSeleccionPago(decimal totalBase, string monedaBase = "DOP")
        {
            _totalBase = totalBase;
            _monedaBase = monedaBase;
            InitializeComponent();
        }

        private void FormSeleccionPago_Load(object sender, EventArgs e)
        {
            lblTotalBase.Text = $"{_totalBase:N2} {_monedaBase}";

            CargarMonedas();
            CargarMediosPago();
            ActualizarResumenMonedas();
            ActualizarGrillaPagos();
            ActualizarTotales();
        }

        // ===================== MONEDAS =====================

        private void CargarMonedas()
        {
            var dt = SqlHelper.ExecuteDataTable(@"
SELECT MonedaCodigo, Nombre
FROM dbo.Moneda
WHERE Estado = 1
ORDER BY Nombre;");

            cbMoneda.DisplayMember = "Nombre";
            cbMoneda.ValueMember = "MonedaCodigo";
            cbMoneda.DataSource = dt;

            // Selecciona por defecto la moneda base si existe
            for (int i = 0; i < cbMoneda.Items.Count; i++)
            {
                var drv = cbMoneda.Items[i] as DataRowView;
                if (drv != null && string.Equals(
                        drv["MonedaCodigo"].ToString(), _monedaBase, StringComparison.OrdinalIgnoreCase))
                {
                    cbMoneda.SelectedIndex = i;
                    break;
                }
            }
        }

        private void ActualizarResumenMonedas()
        {
            // Obtenemos todas las monedas activas otra vez
            var dt = SqlHelper.ExecuteDataTable(@"
SELECT MonedaCodigo, Nombre
FROM dbo.Moneda
WHERE Estado = 1
ORDER BY Nombre;");

            var tabla = new DataTable();
            tabla.Columns.Add("MonedaCodigo", typeof(string));
            tabla.Columns.Add("Nombre", typeof(string));
            tabla.Columns.Add("Tasa", typeof(decimal));
            tabla.Columns.Add("TotalMoneda", typeof(decimal));
            tabla.Columns.Add("PendienteMoneda", typeof(decimal));

            // Diferencia pendiente en base (DOP)
            decimal pagadoBase = _pagos.Sum(p => p.MontoBase);
            decimal pendienteBase = _totalBase - pagadoBase;

            foreach (DataRow row in dt.Rows)
            {
                string codigo = row["MonedaCodigo"].ToString() ?? "";
                string nombre = row["Nombre"].ToString() ?? "";

                decimal tasa = 1m;
                if (!string.Equals(codigo, _monedaBase, StringComparison.OrdinalIgnoreCase))
                {
                    var sql = $@"
SELECT TOP(1) TasaVenta
FROM dbo.TipoCambio
WHERE MonedaCodigo = @m
ORDER BY Fecha DESC;";
                    using var cn = Db.GetOpenConnection();
                    using var cmd = new Microsoft.Data.SqlClient.SqlCommand(sql, cn);
                    cmd.Parameters.AddWithValue("@m", codigo);
                    var valor = cmd.ExecuteScalar();
                    if (valor != null && valor != DBNull.Value)
                        tasa = Convert.ToDecimal(valor);
                }

                if (tasa <= 0) tasa = 1m;

                decimal totalMoneda = Math.Round(_totalBase / tasa, 2);
                decimal pendienteMoneda = Math.Round(pendienteBase / tasa, 2);

                tabla.Rows.Add(codigo, nombre, tasa, totalMoneda, pendienteMoneda);
            }

            gridMonedas.DataSource = tabla;

            // Formatos con variable local para evitar warnings de null
            var colTotalMoneda = gridMonedas.Columns["TotalMoneda"];
            if (colTotalMoneda != null)
                colTotalMoneda.DefaultCellStyle.Format = "N2";

            var colPendienteMoneda = gridMonedas.Columns["PendienteMoneda"];
            if (colPendienteMoneda != null)
                colPendienteMoneda.DefaultCellStyle.Format = "N2";

            var colTasa = gridMonedas.Columns["Tasa"];
            if (colTasa != null)
                colTasa.DefaultCellStyle.Format = "N4";
        }
        private void cbMoneda_SelectedIndexChanged(object sender, EventArgs e)
        {
            // No necesitamos recalcular nada aquí; la moneda se usa al agregar una línea
        }

        // ===================== MEDIOS DE PAGO =====================

        private void CargarMediosPago()
        {
            var dt = SqlHelper.ExecuteDataTable(@"
SELECT MedioPagoId, Nombre
FROM dbo.MedioPago
WHERE Estado = 1
ORDER BY Nombre;");

            cbMedioPago.DisplayMember = "Nombre";
            cbMedioPago.ValueMember = "MedioPagoId";
            cbMedioPago.DataSource = dt;

            // Por defecto, medio pago 1 si existe
            for (int i = 0; i < cbMedioPago.Items.Count; i++)
            {
                var drv = cbMedioPago.Items[i] as DataRowView;
                if (drv != null && drv["MedioPagoId"] != DBNull.Value &&
                    Convert.ToInt32(drv["MedioPagoId"]) == 1)
                {
                    cbMedioPago.SelectedIndex = i;
                    break;
                }
            }
        }

        // ===================== KEYPAD NUMÉRICO =====================

        private void AppendNumero(string digito)
        {
            // Sencillo: concatenamos al final del textbox de monto
            txtMontoMoneda.Text += digito;
            txtMontoMoneda.SelectionStart = txtMontoMoneda.Text.Length;
        }

        private void btnNum_Click(object sender, EventArgs e)
        {
            if (sender is Button btn && !string.IsNullOrEmpty(btn.Tag?.ToString()))
            {
                AppendNumero(btn.Tag.ToString()!);
            }
        }

        private void btnBorrarMonto_Click(object sender, EventArgs e)
        {
            txtMontoMoneda.Clear();
        }

        // ===================== PAGOS =====================

        private void btnAgregarLinea_Click(object sender, EventArgs e)
        {
            if (cbMoneda.SelectedValue == null || cbMedioPago.SelectedValue == null)
            {
                MessageBox.Show("Seleccione moneda y medio de pago.",
                    "Pago", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!decimal.TryParse(txtMontoMoneda.Text.Replace(",", ""), out var montoMoneda) ||
                montoMoneda <= 0)
            {
                MessageBox.Show("Monto inválido.",
                    "Pago", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string monedaCodigo = cbMoneda.SelectedValue.ToString() ?? _monedaBase;
            int medioId = Convert.ToInt32(cbMedioPago.SelectedValue);
            string nombreMedio = cbMedioPago.Text;

            // Obtener tasa
            decimal tasa = 1m;
            if (!string.Equals(monedaCodigo, _monedaBase, StringComparison.OrdinalIgnoreCase))
            {
                var sql = @"
SELECT TOP(1) TasaVenta
FROM dbo.TipoCambio
WHERE MonedaCodigo = @m
ORDER BY Fecha DESC;";
                using var cn = Db.GetOpenConnection();
                using var cmd = new Microsoft.Data.SqlClient.SqlCommand(sql, cn);
                cmd.Parameters.AddWithValue("@m", monedaCodigo);
                var valor = cmd.ExecuteScalar();
                if (valor != null && valor != DBNull.Value)
                    tasa = Convert.ToDecimal(valor);
            }
            if (tasa <= 0) tasa = 1m;

            // Agregamos a la lista interna
            _pagos.Add(new PagoLineaResult
            {
                MedioPagoId = medioId,
                NombreMedio = nombreMedio,
                MonedaCodigo = monedaCodigo,
                TasaCambio = tasa,
                MontoMoneda = montoMoneda
            });

            txtMontoMoneda.Clear();
            ActualizarGrillaPagos();
            ActualizarTotales();
            ActualizarResumenMonedas();
        }

        private void btnQuitarLinea_Click(object sender, EventArgs e)
        {
            if (gridPagos.CurrentRow == null || gridPagos.CurrentRow.Index < 0)
                return;

            int idx = gridPagos.CurrentRow.Index;
            if (idx >= 0 && idx < _pagos.Count)
            {
                _pagos.RemoveAt(idx);
                ActualizarGrillaPagos();
                ActualizarTotales();
                ActualizarResumenMonedas();
            }
        }

        private void ActualizarGrillaPagos()
        {
            gridPagos.DataSource = null;
            gridPagos.DataSource = _pagos
                .Select(p => new
                {
                    p.NombreMedio,
                    p.MonedaCodigo,
                    p.TasaCambio,
                    p.MontoMoneda,
                    MontoBase = p.MontoBase
                })
                .ToList();

            // Usar variables locales para evitar la "desreferencia posiblemente NULL"
            var colMontoMoneda = gridPagos.Columns["MontoMoneda"];
            if (colMontoMoneda != null)
                colMontoMoneda.DefaultCellStyle.Format = "N2";

            var colMontoBase = gridPagos.Columns["MontoBase"];
            if (colMontoBase != null)
                colMontoBase.DefaultCellStyle.Format = "N2";

            var colTasaCambio = gridPagos.Columns["TasaCambio"];
            if (colTasaCambio != null)
                colTasaCambio.DefaultCellStyle.Format = "N4";
        }

        private void ActualizarTotales()
        {
            var pagadoBase = _pagos.Sum(p => p.MontoBase);
            var pendienteBase = _totalBase - pagadoBase;

            lblPagadoBase.Text = pagadoBase.ToString("N2");
            lblPendienteBase.Text = pendienteBase.ToString("N2");
        }

        // ===================== ACEPTAR / CANCELAR =====================

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            var pagadoBase = _pagos.Sum(p => p.MontoBase);
            var pendienteBase = _totalBase - pagadoBase;

            if (pendienteBase > 0.01m)
            {
                MessageBox.Show(
                    $"Aún falta por cobrar: {pendienteBase:N2} {_monedaBase}",
                    "Pago", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_pagos.Count == 0)
            {
                MessageBox.Show("No hay pagos registrados.", "Pago",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var res = new SeleccionPagoResult
            {
                TotalBase = _totalBase
            };
            res.Pagos.AddRange(_pagos);

            Result = res;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            // No devolvemos resultado
            Result = null;
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
