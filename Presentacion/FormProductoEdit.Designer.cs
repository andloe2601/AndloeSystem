using System.Windows.Forms;

namespace Andloe.Presentacion
{
    partial class FormProductoEdit
    {
        private System.ComponentModel.IContainer components = null;

        private TextBox txtCodigo;
        private TextBox txtDescripcion;
        private TextBox txtReferencia;
        private Button btnFinalizar;

        private ComboBox cboUnidad;
        private ComboBox cboImpuesto;
        private ComboBox cboCategoria;
        private ComboBox cboSubcategoria;

        private TextBox txtPrecioVenta;
        private TextBox txtPrecioCosto;
        private TextBox txtPrecioMayor;

        private TextBox txtExistencia;
        private TextBox txtUltimoPrecioCompra;
        private TextBox txtPorcBeneficio;

        private CheckBox chkActivo;

        // ✅ NUEVO
        private CheckBox chkBloqNegativo;

        private Button btnAtras;
        private Button btnSiguiente;
        private Button btnGuardar;
        private Button btnCerrar;

        private DataGridView gridBarras;
        private TextBox txtBarraManual;
        private Button btnAgregarBarraManual;
        private Button btnAgregarBarraAuto;
        private Button btnEliminarBarra;

        private DataGridViewTextBoxColumn colBarraCodigo;
        private DataGridViewTextBoxColumn colBarraTipo;
        private DataGridViewTextBoxColumn colBarraUsuario;
        private DataGridViewTextBoxColumn colBarraUltUso;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        // ✅ helper clásico (NO local function) para que el diseñador no se rompa
        private static Label MakeLabel(string text)
        {
            var l = new Label();
            l.Text = text;
            l.Dock = DockStyle.Fill;
            l.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            return l;
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();

            // =========================
            // ROOT
            // =========================
            var root = new TableLayoutPanel();
            root.Dock = DockStyle.Fill;
            root.Padding = new Padding(12);
            root.ColumnCount = 1;
            root.RowCount = 3;
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 270));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 56));

            // =========================
            // TOP CARD (FICHA)
            // =========================
            var card = new Panel { Dock = DockStyle.Fill };

            var grid = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 6,
                RowCount = 6,
                Padding = new Padding(8)
            };

            // ✅ SIN for (para que abra el diseñador)
            grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16.666F));
            grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16.666F));
            grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16.666F));
            grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16.666F));
            grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16.666F));
            grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16.666F));

            // 6 filas
            grid.RowStyles.Add(new RowStyle(SizeType.Absolute, 30)); // 0
            grid.RowStyles.Add(new RowStyle(SizeType.Absolute, 30)); // 1
            grid.RowStyles.Add(new RowStyle(SizeType.Absolute, 30)); // 2
            grid.RowStyles.Add(new RowStyle(SizeType.Absolute, 30)); // 3
            grid.RowStyles.Add(new RowStyle(SizeType.Absolute, 30)); // 4
            grid.RowStyles.Add(new RowStyle(SizeType.Absolute, 30)); // 5

            // =========================
            // CONTROLES
            // =========================
            txtCodigo = new TextBox { Dock = DockStyle.Fill };
            txtDescripcion = new TextBox { Dock = DockStyle.Fill };
            txtReferencia = new TextBox { Dock = DockStyle.Fill };

            cboUnidad = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };
            cboImpuesto = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };
            cboCategoria = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };
            cboSubcategoria = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };

            txtPrecioVenta = new TextBox { Dock = DockStyle.Fill };
            txtPrecioCosto = new TextBox { Dock = DockStyle.Fill };
            txtPrecioMayor = new TextBox { Dock = DockStyle.Fill };

            txtExistencia = new TextBox { Dock = DockStyle.Fill, ReadOnly = true, TabStop = false };
            txtUltimoPrecioCompra = new TextBox { Dock = DockStyle.Fill, ReadOnly = true, TabStop = false };
            txtPorcBeneficio = new TextBox { Dock = DockStyle.Fill };

            chkActivo = new CheckBox { Text = "Activo", AutoSize = true };
            chkBloqNegativo = new CheckBox { Text = "Bloquear venta en negativo", AutoSize = true };

            // =========================
            // GRID LAYOUT (FICHA)
            // =========================

            // Row 0
            grid.Controls.Add(MakeLabel("Código"), 0, 0);
            grid.Controls.Add(txtCodigo, 1, 0);
            grid.SetColumnSpan(txtCodigo, 2);

            grid.Controls.Add(MakeLabel("Impuesto (ITBIS)"), 3, 0);
            grid.Controls.Add(cboImpuesto, 4, 0);
            grid.SetColumnSpan(cboImpuesto, 2);

            // Row 1
            grid.Controls.Add(MakeLabel("Descripción"), 0, 1);
            grid.Controls.Add(txtDescripcion, 1, 1);
            grid.SetColumnSpan(txtDescripcion, 5);

            // Row 2
            grid.Controls.Add(MakeLabel("Referencia"), 0, 2);
            grid.Controls.Add(txtReferencia, 1, 2);
            grid.SetColumnSpan(txtReferencia, 2);

            grid.Controls.Add(MakeLabel("Unidad"), 3, 2);
            grid.Controls.Add(cboUnidad, 4, 2);
            grid.SetColumnSpan(cboUnidad, 2);

            // Row 3
            grid.Controls.Add(MakeLabel("Categoría"), 0, 3);
            grid.Controls.Add(cboCategoria, 1, 3);
            grid.SetColumnSpan(cboCategoria, 2);

            grid.Controls.Add(MakeLabel("Subcategoría"), 3, 3);
            grid.Controls.Add(cboSubcategoria, 4, 3);
            grid.SetColumnSpan(cboSubcategoria, 2);

            // Row 4
            grid.Controls.Add(MakeLabel("Precio Venta"), 0, 4);
            grid.Controls.Add(txtPrecioVenta, 1, 4);

            grid.Controls.Add(MakeLabel("Precio Costo"), 2, 4);
            grid.Controls.Add(txtPrecioCosto, 3, 4);

            grid.Controls.Add(MakeLabel("Precio x Mayor"), 4, 4);
            grid.Controls.Add(txtPrecioMayor, 5, 4);

            // Row 5
            grid.Controls.Add(MakeLabel("Existencia"), 0, 5);
            grid.Controls.Add(txtExistencia, 1, 5);

            grid.Controls.Add(MakeLabel("Ult. Precio Compra"), 2, 5);
            grid.Controls.Add(txtUltimoPrecioCompra, 3, 5);

            grid.Controls.Add(MakeLabel("% Bfº bruto"), 4, 5);
            grid.Controls.Add(txtPorcBeneficio, 5, 5);

            // Flow activo (abajo del card)
            var flow = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                Height = 44,
                FlowDirection = FlowDirection.LeftToRight,
                Padding = new Padding(8, 0, 8, 8),
                WrapContents = false,
                AutoScroll = true
            };

            flow.Controls.Add(chkActivo);
            flow.Controls.Add(chkBloqNegativo);

            card.Controls.Add(grid);
            card.Controls.Add(flow);

            // =========================
            // MID (BARRAS)
            // =========================
            var panelBarras = new Panel { Dock = DockStyle.Fill, Padding = new Padding(0, 8, 0, 0) };

            var topBarras = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 40,
                FlowDirection = FlowDirection.LeftToRight,
                Padding = new Padding(0, 0, 0, 8)
            };

            txtBarraManual = new TextBox { Width = 240 };
            btnAgregarBarraManual = new Button { Text = "Agregar Manual", Width = 120 };
            btnAgregarBarraAuto = new Button { Text = "Generar Auto", Width = 120 };
            btnEliminarBarra = new Button { Text = "Eliminar", Width = 90 };
            btnFinalizar = new Button { Text = "Finalizar", Width = 110, Height = 34 };

            topBarras.Controls.AddRange(new Control[]
            {
                new Label { Text = "Código Barra:", AutoSize = true, TextAlign = System.Drawing.ContentAlignment.MiddleLeft },
                txtBarraManual,
                btnAgregarBarraManual,
                btnAgregarBarraAuto,
                btnEliminarBarra
            });

            gridBarras = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoGenerateColumns = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            colBarraCodigo = new DataGridViewTextBoxColumn
            {
                HeaderText = "Código",
                DataPropertyName = "CodigoBarras",
                FillWeight = 35,
                Name = "colBarraCodigo"
            };
            colBarraTipo = new DataGridViewTextBoxColumn
            {
                HeaderText = "Tipo",
                DataPropertyName = "Tipo",
                FillWeight = 10,
                Name = "colBarraTipo"
            };
            colBarraUsuario = new DataGridViewTextBoxColumn
            {
                HeaderText = "Usuario",
                DataPropertyName = "Usuario",
                FillWeight = 20,
                Name = "colBarraUsuario"
            };
            colBarraUltUso = new DataGridViewTextBoxColumn
            {
                HeaderText = "Último uso",
                DataPropertyName = "UltimaFechaUtilizacion",
                FillWeight = 20,
                Name = "colBarraUltUso",
                DefaultCellStyle = new DataGridViewCellStyle { Format = "g" }
            };

            gridBarras.Columns.AddRange(new DataGridViewColumn[]
            {
                colBarraCodigo,
                colBarraTipo,
                colBarraUsuario,
                colBarraUltUso
            });

            panelBarras.Controls.Add(gridBarras);
            panelBarras.Controls.Add(topBarras);

            // =========================
            // BOTTOM BOTONES
            // =========================
            var bottom = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.RightToLeft,
                Padding = new Padding(0, 8, 0, 0)
            };

            btnCerrar = new Button { Text = "Cerrar", Width = 110, Height = 34 };
            btnGuardar = new Button { Text = "Guardar", Width = 110, Height = 34 };
            btnSiguiente = new Button { Text = "Siguiente >", Width = 110, Height = 34 };
            btnAtras = new Button { Text = "< Atrás", Width = 110, Height = 34 };

            bottom.Controls.AddRange(new Control[] { btnCerrar, btnGuardar, btnSiguiente, btnAtras });

            // =========================
            // ROOT ADD
            // =========================
            root.Controls.Add(card, 0, 0);
            root.Controls.Add(panelBarras, 0, 1);
            root.Controls.Add(bottom, 0, 2);

            // =========================
            // FORM
            // =========================
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(980, 680);
            Controls.Add(root);
            Name = "FormProductoEdit";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Producto";
        }
    }
}
