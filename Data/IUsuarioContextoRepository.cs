using Andloe.Entidad;

namespace Andloe.Data
{
    public interface IUsuarioContextoRepository
    {
        UsuarioContextoDto ObtenerPorUsuario(string usuario);
        UsuarioContextoDto ObtenerPorUsuarioId(int usuarioId);

        // ✅ alias para no romper código viejo
        UsuarioContextoDto ObtenerContexto(int usuarioId);

        bool UsuarioTieneAccesoAEmpresa(int usuarioId, int empresaId);
        void UpsertContextoDefault(int usuarioId, int empresaId);
    }
}
