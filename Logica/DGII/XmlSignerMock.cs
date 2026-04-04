using System.Threading;
using System.Threading.Tasks;

namespace Andloe.Logica.DGII
{
    public sealed class XmlSignerMock : IXmlSigner
    {
        public Task<string> FirmarAsync(string xmlSinFirmar, CancellationToken ct = default)
        {
            // Mock: no cambia nada (o si quieres, agrega un tag)
            xmlSinFirmar ??= "";
            return Task.FromResult(xmlSinFirmar);
        }
    }
}