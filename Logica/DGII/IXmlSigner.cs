using System.Threading;
using System.Threading.Tasks;

namespace Andloe.Logica.DGII
{
    public interface IXmlSigner
    {
        Task<string> FirmarAsync(string xmlSinFirmar, CancellationToken ct = default);
    }
}