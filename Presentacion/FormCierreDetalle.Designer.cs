using System.Drawing;
using System.Windows.Forms;

namespace Presentation
{
    partial class FormCierreDetalle
    {
        private System.ComponentModel.IContainer components = null;

        private Panel panelHeader;
        private Label lblTitulo;
        private Label lblSubtitulo;

        private GroupBox grpResumen;
        private Label lblCierreId;
        private Label lblCierreIdValor;
        private Label lblCaja;
        private Label lblCajaValor;
        private Label lblUsuario;
        private Label lblUsuarioValor;
        private Label lblRangoFechas;
        private Label lblRangoFechasValor;
        private Label lblEstado;
        private Label lblEstadoValor;

        private TabControl tabControlDetalle;
        private TabPage tabVentas;
        private TabPage tabPagos;
        private TabPage tabFondo;

        private DataGridView dgvVentas;
        private DataGridView dgvPagos;
        private DataGridView dgvFondo;

        private GroupBox grpTotales;
        private Label lblTotalVentas;
        private Label lblTotalVentasValor;
        private Label lblTotalPagos;
        private Label lblTotalPagosValor;
        private Label lblEfectivoTeorico;
        private Label lblEfectivoTeoricoValor;
        private Label lblEfectivoDeclarado;
        private Label lblEfectivoDeclaradoValor;
        private Label lblDiferencia;
        private Label lblDiferenciaValor;

        private Button btnCerrar;
        private Button btnImprimir; 

        // NUEVOS CONTROLES PARA RESUMEN DE PAGOS (si los usas luego)
        private Label lblResumenMedios;
        private DataGridView dgvPagosResumen;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();

            panelHeader = new Panel();
            lblTitulo = new Label();
            lblSubtitulo = new Label();

            grpResumen = new GroupBox();
            lblCierreId = new Label();
            lblCierreIdValor = new Label();
            lblCaja = new Label();
            lblCajaValor = new Label();
            lblUsuario = new Label();
            lblUsuarioValor = new Label();
            lblRangoFechas = new Label();
            lblRangoFechasValor = new Label();
            lblEstado = new Label();
            lblEstadoValor = new Label();

            tabControlDetalle = new TabControl();
            tabVentas = new TabPage();
            tabPagos = new TabPage();
            tabFondo = new TabPage();

            dgvVentas = new DataGridView();
            dgvPagos = new DataGridView();
            dgvFondo = new DataGridView();

            grpTotales = new GroupBox();
            lblTotalVentas = new Label();
            lblTotalVentasValor = new Label();
            lblTotalPagos = new Label();
            lblTotalPagosValor = new Label();
            lblEfectivoTeorico = new Label();
            lblEfectivoTeoricoValor = new Label();
            lblEfectivoDeclarado = new Label();
            lblEfectivoDeclaradoValor = new Label();
            lblDiferencia = new Label();
            lblDiferenciaValor = new Label();

            btnCerrar = new Button();
            btnImprimir = new Button();      // nuevo botón

            lblResumenMedios = new Label();
            dgvPagosResumen = new DataGridView();

            SuspendLayout();

            // ============== HEADER ==================
            panelHeader.Dock = DockStyle.Top;
            panelHeader.Height = 60;
            panelHeader.BackColor = Color.FromArgb(30, 144, 255);
            panelHeader.Padding = new Padding(20, 10, 20, 10);

            lblTitulo.Text = "Detalle de Cierre de Caja";
            lblTitulo.ForeColor = Color.White;
            lblTitulo.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            lblTitulo.AutoSize = true;
            lblTitulo.Location = new Point(20, 10);

            lblSubtitulo.Text = "Información detallada del cierre seleccionado";
            lblSubtitulo.ForeColor = Color.WhiteSmoke;
            lblSubtitulo.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            lblSubtitulo.AutoSize = true;
            lblSubtitulo.Location = new Point(22, 35);

            panelHeader.Controls.Add(lblTitulo);
            panelHeader.Controls.Add(lblSubtitulo);

            // ============== GRUPO RESUMEN ==============
            grpResumen.Text = "Resumen del Cierre";
            grpResumen.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            grpResumen.BackColor = Color.White;
            grpResumen.Location = new Point(15, 70);
            grpResumen.Size = new Size(760, 90);

            int xLabel = 15;
            int xValor = 120;
            int yRow1 = 25;
            int yRow2 = 50;
            int sepX = 380;

            // CierreId
            lblCierreId.Text = "Cierre ID:";
            lblCierreId.Location = new Point(xLabel, yRow1);
            lblCierreId.AutoSize = true;

            lblCierreIdValor.Text = "---";
            lblCierreIdValor.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblCierreIdValor.Location = new Point(xValor, yRow1);
            lblCierreIdValor.AutoSize = true;

            // Caja
            lblCaja.Text = "Caja:";
            lblCaja.Location = new Point(xLabel, yRow2);
            lblCaja.AutoSize = true;

            lblCajaValor.Text = "---";
            lblCajaValor.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblCajaValor.Location = new Point(xValor, yRow2);
            lblCajaValor.AutoSize = true;

            // Usuario
            lblUsuario.Text = "Usuario:";
            lblUsuario.Location = new Point(xLabel + sepX, yRow1);
            lblUsuario.AutoSize = true;

            lblUsuarioValor.Text = "---";
            lblUsuarioValor.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblUsuarioValor.Location = new Point(xValor + sepX, yRow1);
            lblUsuarioValor.AutoSize = true;

            // Estado
            lblEstado.Text = "Estado:";
            lblEstado.Location = new Point(xLabel + sepX, yRow2);
            lblEstado.AutoSize = true;

            lblEstadoValor.Text = "---";
            lblEstadoValor.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblEstadoValor.Location = new Point(xValor + sepX, yRow2);
            lblEstadoValor.AutoSize = true;

            // Rango Fechas
            lblRangoFechas.Text = "Rango:";
            lblRangoFechas.Location = new Point(15, 70);
            lblRangoFechas.AutoSize = true;

            lblRangoFechasValor.Text = "---";
            lblRangoFechasValor.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblRangoFechasValor.Location = new Point(70, 70);
            lblRangoFechasValor.AutoSize = true;

            grpResumen.Controls.Add(lblCierreId);
            grpResumen.Controls.Add(lblCierreIdValor);
            grpResumen.Controls.Add(lblCaja);
            grpResumen.Controls.Add(lblCajaValor);
            grpResumen.Controls.Add(lblUsuario);
            grpResumen.Controls.Add(lblUsuarioValor);
            grpResumen.Controls.Add(lblEstado);
            grpResumen.Controls.Add(lblEstadoValor);
            grpResumen.Controls.Add(lblRangoFechas);
            grpResumen.Controls.Add(lblRangoFechasValor);

            // ============== TABCONTROL DETALLE ==============
            tabControlDetalle.Location = new Point(15, 170);
            tabControlDetalle.Size = new Size(760, 260);
            tabControlDetalle.Font = new Font("Segoe UI", 9F, FontStyle.Regular);

            tabVentas.Text = "Ventas";
            tabPagos.Text = "Pagos / Medios";
            tabFondo.Text = "Fondo y Movimientos";

            // helper grids
            void ConfigGrid(DataGridView dgv)
            {
                dgv.AllowUserToAddRows = false;
                dgv.AllowUserToDeleteRows = false;
                dgv.ReadOnly = true;
                dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dgv.MultiSelect = false;
                dgv.BackgroundColor = Color.White;
                dgv.BorderStyle = BorderStyle.None;
                dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dgv.RowHeadersVisible = false;
            }

            // VENTAS
            ConfigGrid(dgvVentas);
            dgvVentas.Dock = DockStyle.Fill;
            tabVentas.Controls.Add(dgvVentas);

            // PAGOS (detalle arriba)
            ConfigGrid(dgvPagos);
            dgvPagos.Dock = DockStyle.Top;
            dgvPagos.Height = 170;

            // LABEL resumen medios
            lblResumenMedios.Text = "Totales por medio de pago:";
            lblResumenMedios.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            lblResumenMedios.AutoSize = true;
            lblResumenMedios.Location = new Point(10, 180);

            // GRID resumen medios
            ConfigGrid(dgvPagosResumen);
            dgvPagosResumen.Location = new Point(10, 200);
            dgvPagosResumen.Size = new Size(730, 55);
            dgvPagosResumen.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            dgvPagosResumen.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            tabPagos.Controls.Add(dgvPagosResumen);
            tabPagos.Controls.Add(lblResumenMedios);
            tabPagos.Controls.Add(dgvPagos);

            // FONDO
            ConfigGrid(dgvFondo);
            dgvFondo.Dock = DockStyle.Fill;
            tabFondo.Controls.Add(dgvFondo);

            tabControlDetalle.TabPages.Add(tabVentas);
            tabControlDetalle.TabPages.Add(tabPagos);
            tabControlDetalle.TabPages.Add(tabFondo);

            // ============== GRUPO TOTALES ==============
            grpTotales.Text = "Totales del Cierre";
            grpTotales.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            grpTotales.Location = new Point(15, 435);
            grpTotales.Size = new Size(760, 80);
            grpTotales.BackColor = Color.White;

            int yTot1 = 25;
            int yTot2 = 50;
            int xCol1 = 15;
            int xCol2 = 265;
            int xCol3 = 515;

            lblTotalVentas.Text = "Total ventas:";
            lblTotalVentas.Location = new Point(xCol1, yTot1);
            lblTotalVentas.AutoSize = true;

            lblTotalVentasValor.Text = "0.00";
            lblTotalVentasValor.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblTotalVentasValor.Location = new Point(xCol1 + 95, yTot1);
            lblTotalVentasValor.AutoSize = true;

            lblTotalPagos.Text = "Total pagos:";
            lblTotalPagos.Location = new Point(xCol1, yTot2);
            lblTotalPagos.AutoSize = true;

            lblTotalPagosValor.Text = "0.00";
            lblTotalPagosValor.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblTotalPagosValor.Location = new Point(xCol1 + 95, yTot2);
            lblTotalPagosValor.AutoSize = true;

            lblEfectivoTeorico.Text = "Efectivo teórico:";
            lblEfectivoTeorico.Location = new Point(xCol2, yTot1);
            lblEfectivoTeorico.AutoSize = true;

            lblEfectivoTeoricoValor.Text = "0.00";
            lblEfectivoTeoricoValor.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblEfectivoTeoricoValor.Location = new Point(xCol2 + 115, yTot1);
            lblEfectivoTeoricoValor.AutoSize = true;

            lblEfectivoDeclarado.Text = "Efectivo declarado:";
            lblEfectivoDeclarado.Location = new Point(xCol2, yTot2);
            lblEfectivoDeclarado.AutoSize = true;

            lblEfectivoDeclaradoValor.Text = "0.00";
            lblEfectivoDeclaradoValor.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblEfectivoDeclaradoValor.Location = new Point(xCol2 + 115, yTot2);
            lblEfectivoDeclaradoValor.AutoSize = true;

            lblDiferencia.Text = "Diferencia:";
            lblDiferencia.Location = new Point(xCol3, yTot1);
            lblDiferencia.AutoSize = true;

            lblDiferenciaValor.Text = "0.00";
            lblDiferenciaValor.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblDiferenciaValor.Location = new Point(xCol3 + 85, yTot1);
            lblDiferenciaValor.AutoSize = true;
            lblDiferenciaValor.ForeColor = Color.FromArgb(192, 0, 0);

            grpTotales.Controls.AddRange(new Control[]
            {
                lblTotalVentas, lblTotalVentasValor,
                lblTotalPagos, lblTotalPagosValor,
                lblEfectivoTeorico, lblEfectivoTeoricoValor,
                lblEfectivoDeclarado, lblEfectivoDeclaradoValor,
                lblDiferencia, lblDiferenciaValor
            });

            // ============== BOTONES ==============
            // Imprimir
            btnImprimir.Text = "Imprimir";
            btnImprimir.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnImprimir.Size = new Size(90, 30);
            btnImprimir.Location = new Point(585, 525);
            btnImprimir.BackColor = Color.White;
            btnImprimir.FlatStyle = FlatStyle.Flat;
            btnImprimir.FlatAppearance.BorderColor = Color.Silver;
            btnImprimir.Click += btnImprimir_Click;

            // Cerrar
            btnCerrar.Text = "Cerrar";
            btnCerrar.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnCerrar.Size = new Size(90, 30);
            btnCerrar.Location = new Point(685, 525);
            btnCerrar.BackColor = Color.White;
            btnCerrar.FlatStyle = FlatStyle.Flat;
            btnCerrar.FlatAppearance.BorderColor = Color.Silver;
            btnCerrar.Click += (s, e) => Close();

            // ============== FORM ==================
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.WhiteSmoke;
            ClientSize = new Size(790, 570);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Detalle de Cierre";

            Controls.Add(panelHeader);
            Controls.Add(grpResumen);
            Controls.Add(tabControlDetalle);
            Controls.Add(grpTotales);
            Controls.Add(btnImprimir);
            Controls.Add(btnCerrar);

            ResumeLayout(false);
        }

        #endregion
    }
}
