using System.ComponentModel;
using System.Windows.Forms;

namespace Andloe.Presentacion
{
    [DesignerCategory("Code")]
    public class ReadOnlyHintTextBox : TextBox
    {
        public ReadOnlyHintTextBox()
        {
            ReadOnly = true;
            Enabled = false;   // gris
            TabStop = false;
        }

        // Propiedad auxiliar para mostrar texto "legado" (solo lectura)
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string LegacyText
        {
            get => Text ?? string.Empty;
            set => Text = value ?? string.Empty;
        }

        public bool ShouldSerializeLegacyText() => false;
        public void ResetLegacyText() => Text = string.Empty;
    }
}
