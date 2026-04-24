using System;
using System.Data;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using Andloe.Data;

namespace Andloe.Presentacion
{
    public partial class FormKardex : Form
    {
        // Repos para leer producto y BD
        private readonly ProductoRepository _productoRepo = new();

        // Para llevar control del producto actual y existencia
        private string? _productoCodigoActual;
        private decimal _existenciaFinal = 0m;

        public FormKardex()
        {
            InitializeComponent();
        }

        // ================== LOAD ==================
        private void FormKardex_Load(object sender, EventArgs e)
        {
            try
            {
                // Rango de fechas por defecto: último mes
                dtDesde.Value = DateTime.Today.AddMonths(-1);
                dtHasta.Value = DateTime.Today;

                CargarAlmacenes();

                // Diseño básico del grid
                ConfigurarGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al iniciar Kardex: " + ex.Message,
                    "Kardex", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ================== PRODUCTO ==================

        private void txtProductoCodigo_Leave(object sender, EventArgs e)
        {
            var cod = txtProductoCodigo.Text.Trim();

            if (string.IsNullOrEmpty(cod))
            {
                _productoCodigoActual = null;
                txtProductoDescripcion.Text = string.Empty;
                return;
            }

            try
            {
                // Usamos el mismo método que POS: código, ref o código de barras
                var prod = _productoRepo.ObtenerPorCodigoOBarras(cod);
                if (prod == null)
                {
                    _productoCodigoActual = null;
                    txtProductoDescripcion.Text = string.Empty;
                    MessageBox.Show("Producto no encontrado.",
                        "Kardex", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                _productoCodigoActual = prod.Codigo;
                txtProductoCodigo.Text = prod.Codigo; // normalizamos código real
                txtProductoDescripcion.Text =
                    !string.IsNullOrWhiteSpace(prod.Referencia)
                        ? prod.Referencia
                        : prod.Descripcion ?? "";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al buscar producto: " + ex.Message,
                    "Kardex", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnBuscarProducto_Click(object sender, EventArgs e)
        {
            // Si ya tienes un FormBuscarProducto, llama algo como:
            // using (var frm = new FormBuscarProducto())
            // {
            //     if (frm.ShowDialog(this) == DialogResult.OK)
            //     {
            //         txtProductoCodigo.Text = frm.CodigoSeleccionado;
            //         txtProductoCodigo_Leave(sender, e);
            //     }
            // }
            //
            // Por ahora dejamos un mensaje:
            MessageBox.Show("Búsqueda de producto pendiente de implementar.",
                "Kardex", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // ================== ALMACÉN ==================

        private void CargarAlmacenes()
        {
            // Si ya tienes un AlmacenRepository úsalo. Para no romper nada,
            // aquí hago una lectura directa sencilla de dbo.Almacen.

            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
SELECT AlmacenId, Nombre
FROM dbo.Almacen
ORDER BY Nombre;", cn);

            using var da = new SqlDataAdapter(cmd);
            var dt = new DataTable();
            da.Fill(dt);

            // Agregamos opción "Todos"
            var dtCombo = dt.Clone();
            dtCombo.Columns["AlmacenId"].AllowDBNull = true;

            var rowTodos = dtCombo.NewRow();
            rowTodos["AlmacenId"] = DBNull.Value;
            rowTodos["Nombre"] = "(Todos)";
            dtCombo.Rows.Add(rowTodos);

            foreach (DataRow r in dt.Rows)
                dtCombo.ImportRow(r);

            cboAlmacen.DisplayMember = "Nombre";
            cboAlmacen.ValueMember = "AlmacenId";
            cboAlmacen.DataSource = dtCombo;
            cboAlmacen.SelectedIndex = 0;
        }

        // ================== GRID ==================

        private void ConfigurarGrid()
        {
            gridKardex.AutoGenerateColumns = false;
            gridKardex.Columns.Clear();

            gridKardex.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colFecha",
                HeaderText = "Fecha",
                DataPropertyName = "Fecha",
                Width = 90,
                DefaultCellStyle = { Format = "dd/MM/yyyy" }
            });

            gridKardex.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colDocumento",
                HeaderText = "Documento",
                DataPropertyName = "Documento",
                Width = 130
            });

            gridKardex.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colTipo",
                HeaderText = "Tipo",
                DataPropertyName = "Tipo",
                Width = 80
            });

            gridKardex.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colOrigen",
                HeaderText = "Origen",
                DataPropertyName = "Origen",
                Width = 100
            });

            gridKardex.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colEntrada",
                HeaderText = "Entrada",
                DataPropertyName = "Entrada",
                Width = 80,
                DefaultCellStyle =
                {
                    Alignment = DataGridViewContentAlignment.MiddleRight,
                    Format = "N2"
                }
            });

                        gridKardex.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colSalida",
                HeaderText = "Salida",
                DataPropertyName = "Salida",
                Width = 80,
                DefaultCellStyle =
                {
                    Alignment = DataGridViewContentAlignment.MiddleRight,
                    Format = "N2"
                }
            });

            gridKardex.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colExistencia",
                HeaderText = "Existencia",
                DataPropertyName = "Existencia",
                Width = 90,
                DefaultCellStyle =
                {
                    Alignment = DataGridViewContentAlignment.MiddleRight,
                    Format = "N2"
                }
            });

            gridKardex.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colCostoUnit",
                HeaderText = "Costo Unit.",
                DataPropertyName = "CostoUnit",
                Width = 90,
                DefaultCellStyle =
                {
                    Alignment = DataGridViewContentAlignment.MiddleRight,
                    Format = "N2"
                }
            });

            gridKardex.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colCostoTotal",
                HeaderText = "Costo Total",
                DataPropertyName = "CostoTotal",
                Width = 100,
                DefaultCellStyle =
                {
                    Alignment = DataGridViewContentAlignment.MiddleRight,
                    Format = "N2"
                }
            });
        }

        // ================== BOTÓN BUSCAR ==================

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(_productoCodigoActual))
                {
                    MessageBox.Show("Debe seleccionar un producto.",
                        "Kardex", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtProductoCodigo.Focus();
                    return;
                }

                var desde = dtDesde.Value.Date;
                var hasta = dtHasta.Value.Date;

                if (hasta < desde)
                {
                    MessageBox.Show("La fecha 'Hasta' no puede ser menor que 'Desde'.",
                        "Kardex", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int? almacenId = null;
                if (cboAlmacen.SelectedValue != null && cboAlmacen.SelectedValue != DBNull.Value)
                {
                    almacenId = Convert.ToInt32(cboAlmacen.SelectedValue);
                }

                CargarKardex(_productoCodigoActual, desde, hasta, almacenId);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al buscar Kardex: " + ex.Message,
                    "Kardex", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ================== CONSULTA KARDEX ==================

        private void CargarKardex(string productoCodigo, DateTime desde, DateTime hasta, int? almacenId)
        {
            using var cn = Db.GetOpenConnection();

            // 1) ================= EXISTENCIA INICIAL (antes de 'desde') =================
            decimal existenciaInicial = 0m;

            using (var cmdIni = new SqlCommand(@"
SELECT 
    c.Tipo,
    SUM(l.Cantidad) AS Cantidad
FROM dbo.InvMovimientoLin l
JOIN dbo.InvMovimientoCab c ON c.InvMovId = l.InvMovId
WHERE l.ProductoCodigo = @ProductoCodigo
  AND CONVERT(date, c.Fecha) < @Desde
  AND (@AlmacenId IS NULL 
       OR c.AlmacenIdOrigen = @AlmacenId 
       OR c.AlmacenIdDestino = @AlmacenId)
GROUP BY c.Tipo;", cn))
            {
                cmdIni.Parameters.Add("@ProductoCodigo", System.Data.SqlDbType.VarChar, 20).Value = productoCodigo;
                cmdIni.Parameters.Add("@Desde", System.Data.SqlDbType.Date).Value = desde;
                if (almacenId.HasValue)
                    cmdIni.Parameters.Add("@AlmacenId", System.Data.SqlDbType.Int).Value = almacenId.Value;
                else
                    cmdIni.Parameters.Add("@AlmacenId", System.Data.SqlDbType.Int).Value = DBNull.Value;

                using var rdIni = cmdIni.ExecuteReader();
                while (rdIni.Read())
                {
                    var tipoMov = (rdIni["Tipo"] as string ?? "").Trim().ToUpperInvariant();
                    var cant = rdIni.GetDecimal(1);

                    if (tipoMov == "ENTRADA")
                        existenciaInicial += cant;
                    else
                        existenciaInicial -= cant;   // SALIDA u otros tipos restan
                }
            }

            // 2) ================= MOVIMIENTOS EN RANGO [desde, hasta] =================
            using var cmd = new SqlCommand(@"
SELECT 
    c.InvMovId,
    c.Fecha,
    c.Tipo,
    c.Origen,
    c.OrigenId,
    c.AlmacenIdOrigen,
    c.AlmacenIdDestino,
    l.Linea,
    l.Cantidad,
    l.CostoUnitario
FROM dbo.InvMovimientoLin l
JOIN dbo.InvMovimientoCab c ON c.InvMovId = l.InvMovId
WHERE l.ProductoCodigo = @ProductoCodigo
  AND CONVERT(date, c.Fecha) >= @Desde
  AND CONVERT(date, c.Fecha) <= @Hasta
  AND (@AlmacenId IS NULL 
       OR c.AlmacenIdOrigen = @AlmacenId 
       OR c.AlmacenIdDestino = @AlmacenId)
ORDER BY c.Fecha, c.InvMovId, l.Linea;", cn);

            cmd.Parameters.Add("@ProductoCodigo", System.Data.SqlDbType.VarChar, 20).Value = productoCodigo;
            cmd.Parameters.Add("@Desde", System.Data.SqlDbType.Date).Value = desde;
            cmd.Parameters.Add("@Hasta", System.Data.SqlDbType.Date).Value = hasta;
            if (almacenId.HasValue)
                cmd.Parameters.Add("@AlmacenId", System.Data.SqlDbType.Int).Value = almacenId.Value;
            else
                cmd.Parameters.Add("@AlmacenId", System.Data.SqlDbType.Int).Value = DBNull.Value;

            using var da = new SqlDataAdapter(cmd);
            var dtMov = new DataTable();
            da.Fill(dtMov);

            // 3) ================= ARMAR TABLA PARA LA GRILLA =================
            var dtView = new DataTable();
            dtView.Columns.Add("Fecha", typeof(DateTime));
            dtView.Columns.Add("Documento", typeof(string));
            dtView.Columns.Add("Tipo", typeof(string));
            dtView.Columns.Add("Origen", typeof(string));
            dtView.Columns.Add("Entrada", typeof(decimal));
            dtView.Columns.Add("Salida", typeof(decimal));
            dtView.Columns.Add("Existencia", typeof(decimal));
            dtView.Columns.Add("CostoUnit", typeof(decimal));
            dtView.Columns.Add("CostoTotal", typeof(decimal));

            // 3.1 Fila de SALDO INICIAL
            if (existenciaInicial != 0m || dtMov.Rows.Count > 0)
            {
                var rowIni = dtView.NewRow();
                rowIni["Fecha"] = desde;              // puedes usar (desde.AddDays(-1)) si prefieres
                rowIni["Documento"] = "Saldo inicial";
                rowIni["Tipo"] = "";
                rowIni["Origen"] = "";
                rowIni["Entrada"] = 0m;
                rowIni["Salida"] = 0m;
                rowIni["Existencia"] = existenciaInicial;
                rowIni["CostoUnit"] = 0m;
                rowIni["CostoTotal"] = 0m;
                dtView.Rows.Add(rowIni);
            }

            // 3.2 Recorrer los movimientos y acumular existencia a partir del saldo inicial
            decimal existencia = existenciaInicial;

            foreach (DataRow r in dtMov.Rows)
            {
                var fecha = r.Field<DateTime>("Fecha");
                var tipo = (r["Tipo"] as string ?? "").Trim().ToUpperInvariant();
                var origen = r["Origen"] as string ?? "";
                var origenId = r["OrigenId"]?.ToString() ?? "";
                var cant = r.Field<decimal>("Cantidad");
                var costoUnit = r.Field<decimal>("CostoUnitario");

                decimal entrada = 0m;
                decimal salida = 0m;

                if (tipo == "ENTRADA")
                {
                    entrada = cant;
                    existencia += cant;
                }
                else
                {
                    // Todo lo que no sea ENTRADA lo tratamos como SALIDA (SALIDA, AJUSTE_SALIDA, etc.)
                    salida = cant;
                    existencia -= cant;
                }

                var costoTotal = cant * costoUnit;

                var rowView = dtView.NewRow();
                rowView["Fecha"] = fecha;
                rowView["Documento"] = $"{origen} {origenId}";
                rowView["Tipo"] = tipo;
                rowView["Origen"] = origen;
                rowView["Entrada"] = entrada;
                rowView["Salida"] = salida;
                rowView["Existencia"] = existencia;
                rowView["CostoUnit"] = costoUnit;
                rowView["CostoTotal"] = costoTotal;

                dtView.Rows.Add(rowView);
            }

            gridKardex.DataSource = dtView;

            _existenciaFinal = existencia;
            lblExistenciaFinal.Text = _existenciaFinal.ToString("N2");
        }

    
    private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}