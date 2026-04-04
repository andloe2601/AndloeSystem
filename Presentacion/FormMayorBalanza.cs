#nullable enable
using Andloe.Data;
using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Andloe.Presentacion
{
    public partial class FormMayorBalanza : Form
    {
        public FormMayorBalanza()
        {
            InitializeComponent();
            grid.AutoGenerateColumns = true;
        }

        private void btnBuscar_Click(object? sender, EventArgs e)
        {
            try
            {
                var desde = dtDesde.Value.Date;
                var hasta = dtHasta.Value.Date;

                using var cn = Db.GetOpenConnection();
                using var cmd = new SqlCommand("dbo.sp_Conta_Mayor_TotalesPorCuenta", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@Desde", SqlDbType.Date).Value = desde;
                cmd.Parameters.Add("@Hasta", SqlDbType.Date).Value = hasta;
                cmd.Parameters.Add("@CuentaId", SqlDbType.Int).Value = DBNull.Value;

                var dt = new DataTable();
                using var da = new SqlDataAdapter(cmd);
                da.Fill(dt);

                grid.DataSource = dt;

                // Totales generales en pantalla:
                decimal deb = 0, cred = 0, debB = 0, credB = 0;
                foreach (DataRow r in dt.Rows)
                {
                    deb += Convert.ToDecimal(r["DebitoMoneda"]);
                    cred += Convert.ToDecimal(r["CreditoMoneda"]);
                    debB += Convert.ToDecimal(r["DebitoBase"]);
                    credB += Convert.ToDecimal(r["CreditoBase"]);
                }

                lblTot.Text = $"Totales -> Deb: {deb:N2} | Cred: {cred:N2} | DebBase: {debB:N2} | CredBase: {credB:N2}";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Mayor/Balanza", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
#nullable restore
