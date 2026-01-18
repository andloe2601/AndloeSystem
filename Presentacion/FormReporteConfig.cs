using Andloe.Data;
using Andloe.Entidad;
using System;
using System.Data;
using System.Windows.Forms;

namespace Andloe.Presentacion
{
    public partial class FormReporteConfig : Form
    {
        private readonly ReporteMantenimientoRepository _repo = new();

        public FormReporteConfig()
        {
            InitializeComponent();
            Load += FormReporteConfig_Load;
            btnRecargar.Click += (_, __) => RecargarTodo();
            cboModulo.SelectedValueChanged += (_, __) => CargarActividades();
            cboActividad.SelectedValueChanged += (_, __) => RecargarTodo();

            rbEmpresa.CheckedChanged += (_, __) => RecargarAsignaciones();
            rbSucursal.CheckedChanged += (_, __) => RecargarAsignaciones();
            rbUsuario.CheckedChanged += (_, __) => RecargarAsignaciones();

            btnGuardarAsignacion.Click += (_, __) => GuardarAsignacion();
        }

        private void FormReporteConfig_Load(object? sender, EventArgs e)
        {
            // Módulos (puedes crecer después)
            cboModulo.Items.Clear();
            cboModulo.Items.Add("VENTA");
            cboModulo.SelectedIndex = 0;

            rbEmpresa.Checked = true;

            CargarActividades();
            RecargarTodo();
        }

        private void CargarActividades()
        {
            var modulo = (cboModulo.Text ?? "").Trim();
            if (string.IsNullOrWhiteSpace(modulo)) return;

            var dt = _repo.ListarActividades(modulo);

            cboActividad.DisplayMember = "Actividad";
            cboActividad.ValueMember = "Actividad";
            cboActividad.DataSource = dt;

            if (cboActividad.Items.Count > 0)
                cboActividad.SelectedIndex = 0;
        }

        private void RecargarTodo()
        {
            RecargarDef();
            RecargarAsignaciones();
        }

        private void RecargarDef()
        {
            var modulo = (cboModulo.Text ?? "").Trim();
            var actividad = (cboActividad.Text ?? "").Trim();
            if (string.IsNullOrWhiteSpace(modulo) || string.IsNullOrWhiteSpace(actividad)) return;

            var dt = _repo.ListarDef(modulo, actividad);
            gridDef.DataSource = dt;

            // columnas friendly
            if (gridDef.Columns.Contains("RutaArchivo")) gridDef.Columns["RutaArchivo"].Width = 260;
            if (gridDef.Columns.Contains("Nombre")) gridDef.Columns["Nombre"].Width = 220;
        }

        private void RecargarAsignaciones()
        {
            var modulo = (cboModulo.Text ?? "").Trim();
            var actividad = (cboActividad.Text ?? "").Trim();
            if (string.IsNullOrWhiteSpace(modulo) || string.IsNullOrWhiteSpace(actividad)) return;

            var s = Andloe.Logica.SesionService.Current;

            int empresaId = s.EmpresaId;
            int? sucursalId = null;
            int? usuarioId = null;

            if (rbSucursal.Checked) sucursalId = s.SucursalId;
            if (rbUsuario.Checked) usuarioId = s.UsuarioId;

            // Empresa: ambos null
            if (rbEmpresa.Checked) { sucursalId = null; usuarioId = null; }
            if (rbSucursal.Checked) { usuarioId = null; } // sucursal-only
            if (rbUsuario.Checked) { sucursalId = null; } // user-only (si tú quieres user dentro de sucursal, lo cambiamos)

            var dt = _repo.ListarAsignaciones(modulo, actividad, empresaId, sucursalId, usuarioId);
            gridAsignaciones.DataSource = dt;

            if (gridAsignaciones.Columns.Contains("RutaArchivo")) gridAsignaciones.Columns["RutaArchivo"].Width = 240;
            if (gridAsignaciones.Columns.Contains("Nombre")) gridAsignaciones.Columns["Nombre"].Width = 220;
        }

        private void GuardarAsignacion()
        {
            if (gridDef.CurrentRow == null)
            {
                MessageBox.Show("Selecciona un reporte disponible (izquierda).");
                return;
            }

            var modulo = (cboModulo.Text ?? "").Trim();
            var actividad = (cboActividad.Text ?? "").Trim();
            if (string.IsNullOrWhiteSpace(modulo) || string.IsNullOrWhiteSpace(actividad)) return;

            int reporteId = Convert.ToInt32(gridDef.CurrentRow.Cells["ReporteId"].Value);

            var s = Andloe.Logica.SesionService.Current;
            int empresaId = s.EmpresaId;
            int? sucursalId = null;
            int? usuarioId = null;

            if (rbSucursal.Checked) sucursalId = s.SucursalId;
            if (rbUsuario.Checked) usuarioId = s.UsuarioId;

            if (rbEmpresa.Checked) { sucursalId = null; usuarioId = null; }
            if (rbSucursal.Checked) { usuarioId = null; }
            if (rbUsuario.Checked) { sucursalId = null; }

            bool esActivo = chkActivo.Checked;
            bool esDefault = chkDefault.Checked;

            int orden = (int)numOrden.Value;
            int prioridad = (int)numPrioridad.Value;

            _repo.UpsertAsignacion(empresaId, sucursalId, usuarioId, modulo, actividad,
                reporteId, esActivo, orden, esDefault, prioridad);

            RecargarAsignaciones();
            MessageBox.Show("Asignación guardada.");
        }
    }
}
