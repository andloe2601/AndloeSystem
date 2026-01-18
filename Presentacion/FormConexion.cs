using System;
using System.Windows.Forms;
using Andloe.Logica;
using Microsoft.Data.SqlClient; // NuGet en Presentacion

namespace Andloe.Presentacion
{
    public partial class FormConexion : Form
    {
        public FormConexion()
        {
            InitializeComponent();
        }

        private void FormConexion_Load(object sender, EventArgs e)
        {
            var cfg = ConfigManager.Load();
            txtServidor.Text = string.IsNullOrWhiteSpace(cfg.DataSource) ? "." : cfg.DataSource;
            txtBase.Text = string.IsNullOrWhiteSpace(cfg.InitialCatalog) ? "AndloeV1.1" : cfg.InitialCatalog;
            chkIntegrated.Checked = cfg.IntegratedSecurity;
            txtUsuario.Text = cfg.UserID ?? "";
            txtPassword.Text = cfg.Password ?? "";
            chkEncrypt.Checked = cfg.Encrypt;
            chkTrust.Checked = cfg.TrustServerCertificate;

            ToggleSqlFields();
        }

        private void chkIntegrated_CheckedChanged(object sender, EventArgs e) => ToggleSqlFields();

        private void ToggleSqlFields()
        {
            var sqlAuth = !chkIntegrated.Checked;
            txtUsuario.Enabled = sqlAuth;
            txtPassword.Enabled = sqlAuth;
            label3.Enabled = sqlAuth;
            label4.Enabled = sqlAuth;
        }

        private ConexionConfig ReadForm()
        {
            if (string.IsNullOrWhiteSpace(txtServidor.Text))
                throw new InvalidOperationException("Servidor requerido.");
            if (string.IsNullOrWhiteSpace(txtBase.Text))
                throw new InvalidOperationException("Base de datos requerida.");
            if (!chkIntegrated.Checked)
            {
                if (string.IsNullOrWhiteSpace(txtUsuario.Text))
                    throw new InvalidOperationException("Usuario (SQL) requerido.");
                // contraseña puede estar vacía si así lo maneja su SQL
            }

            return new ConexionConfig
            {
                DataSource = txtServidor.Text.Trim(),
                InitialCatalog = txtBase.Text.Trim(),
                IntegratedSecurity = chkIntegrated.Checked,
                UserID = chkIntegrated.Checked ? null : txtUsuario.Text.Trim(),
                Password = chkIntegrated.Checked ? null : txtPassword.Text,
                Encrypt = chkEncrypt.Checked,
                TrustServerCertificate = chkTrust.Checked
            };
        }

        private string BuildCs(ConexionConfig cfg) => cfg.BuildConnectionString();

        private void btnProbar_Click(object sender, EventArgs e)
        {
            try
            {
                var cfg = ReadForm();
                var cs = BuildCs(cfg);

                using var cn = new SqlConnection(cs);
                cn.Open(); // si no lanza -> OK
                lblEstado.ForeColor = System.Drawing.Color.ForestGreen;
                lblEstado.Text = "Conexión exitosa.";
            }
            catch (Exception ex)
            {
                lblEstado.ForeColor = System.Drawing.Color.Firebrick;
                lblEstado.Text = "Error: " + ex.Message;
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                var cfg = ReadForm();
                ConfigManager.Save(cfg);

                // Inicialize Db para la sesión actual
                var cs = BuildCs(cfg);
                Andloe.Data.Db.Init(cs);

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                lblEstado.ForeColor = System.Drawing.Color.Firebrick;
                lblEstado.Text = "Error: " + ex.Message;
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
