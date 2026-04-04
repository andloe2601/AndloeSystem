
namespace Andloe.Presentacion
{
    partial class FormConfiguracionContable
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabGeneral;
        private System.Windows.Forms.CheckBox chkBloqCrear;
        private System.Windows.Forms.CheckBox chkBloqLinea;
        private System.Windows.Forms.CheckBox chkBloqPlantilla;
        private System.Windows.Forms.CheckBox chkBloqCerrar;
        private System.Windows.Forms.Button btnGuardarGeneral;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabGeneral = new System.Windows.Forms.TabPage();
            this.chkBloqCrear = new System.Windows.Forms.CheckBox();
            this.chkBloqLinea = new System.Windows.Forms.CheckBox();
            this.chkBloqPlantilla = new System.Windows.Forms.CheckBox();
            this.chkBloqCerrar = new System.Windows.Forms.CheckBox();
            this.btnGuardarGeneral = new System.Windows.Forms.Button();

            this.tabControl1.SuspendLayout();
            this.tabGeneral.SuspendLayout();
            this.SuspendLayout();

            this.tabControl1.Controls.Add(this.tabGeneral);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;

            this.tabGeneral.Text = "General";

            this.chkBloqCrear.Text = "Bloquear Crear Asiento";
            this.chkBloqCrear.Top = 20;
            this.chkBloqCrear.Left = 20;

            this.chkBloqLinea.Text = "Bloquear Agregar Línea";
            this.chkBloqLinea.Top = 50;
            this.chkBloqLinea.Left = 20;

            this.chkBloqPlantilla.Text = "Bloquear Aplicar Plantilla";
            this.chkBloqPlantilla.Top = 80;
            this.chkBloqPlantilla.Left = 20;

            this.chkBloqCerrar.Text = "Bloquear Cerrar Asiento";
            this.chkBloqCerrar.Top = 110;
            this.chkBloqCerrar.Left = 20;

            this.btnGuardarGeneral.Text = "Guardar";
            this.btnGuardarGeneral.Top = 150;
            this.btnGuardarGeneral.Left = 20;
            this.btnGuardarGeneral.Click += new System.EventHandler(this.btnGuardarGeneral_Click);

            this.tabGeneral.Controls.Add(this.chkBloqCrear);
            this.tabGeneral.Controls.Add(this.chkBloqLinea);
            this.tabGeneral.Controls.Add(this.chkBloqPlantilla);
            this.tabGeneral.Controls.Add(this.chkBloqCerrar);
            this.tabGeneral.Controls.Add(this.btnGuardarGeneral);

            this.Controls.Add(this.tabControl1);
            this.Load += new System.EventHandler(this.FormConfiguracionContable_Load);
            this.Text = "Configuración Contable";
            this.ClientSize = new System.Drawing.Size(420, 260);

            this.tabControl1.ResumeLayout(false);
            this.tabGeneral.ResumeLayout(false);
            this.ResumeLayout(false);
        }
    }
}
