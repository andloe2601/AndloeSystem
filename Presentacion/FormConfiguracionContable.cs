
using System;
using System.Data;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using Andloe.Data;

namespace Andloe.Presentacion
{
    public partial class FormConfiguracionContable : Form
    {
        public FormConfiguracionContable()
        {
            InitializeComponent();
        }

        private void FormConfiguracionContable_Load(object sender, EventArgs e)
        {
            CargarConfig();
        }

        private void CargarConfig()
        {
            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand("dbo.sp_Conta_ConfigGeneral_Get", cn);
            using var rd = cmd.ExecuteReader();
            if (!rd.Read()) return;

            chkBloqCrear.Checked = rd.GetBoolean(rd.GetOrdinal("BloquearCrearAsiento"));
            chkBloqLinea.Checked = rd.GetBoolean(rd.GetOrdinal("BloquearAgregarLinea"));
            chkBloqPlantilla.Checked = rd.GetBoolean(rd.GetOrdinal("BloquearAplicarPlantilla"));
            chkBloqCerrar.Checked = rd.GetBoolean(rd.GetOrdinal("BloquearCerrarAsiento"));
        }

        private void btnGuardarGeneral_Click(object sender, EventArgs e)
        {
            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand("dbo.sp_Conta_ConfigGeneral_Save", cn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@BloquearCrearAsiento", chkBloqCrear.Checked);
            cmd.Parameters.AddWithValue("@BloquearAgregarLinea", chkBloqLinea.Checked);
            cmd.Parameters.AddWithValue("@BloquearAplicarPlantilla", chkBloqPlantilla.Checked);
            cmd.Parameters.AddWithValue("@BloquearCerrarAsiento", chkBloqCerrar.Checked);

            cmd.ExecuteNonQuery();

            MessageBox.Show("Configuración guardada correctamente.",
                "Configuración contable", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
