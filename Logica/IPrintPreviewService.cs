#nullable enable
using System.Data;

namespace Andloe.Logica
{
    /// <summary>
    /// Contrato de vista previa de impresión (UI). La lógica solo conoce este contrato.
    /// </summary>
    public interface IPrintPreviewService
    {
        void Preview(
            string rdlcRelativePath,
            DataTable cab,
            DataTable det,
            DataTable totales,
            string windowTitle
        );
    }


}
#nullable restore
