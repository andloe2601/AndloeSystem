using System.Collections.Concurrent;

namespace Andloe.Logica;

public class AuthorizationService
{
    private readonly HashSet<string> _roles;
    private readonly HashSet<string> _perms;

    // Mapa simple de rol -> permisos
    private static readonly ConcurrentDictionary<string, string[]> RolePerms = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Administrador"] = new[]
        {
            Permisos.VerDashboard, Permisos.VerUsuarios, Permisos.EditUsuarios,
            Permisos.VerPOS, Permisos.Configurar
        },
        ["CajeroPOS"] = new[] { Permisos.VerDashboard, Permisos.VerPOS },
        ["Vendedor"] = new[] { Permisos.VerDashboard, Permisos.VerPOS }
    };

    public AuthorizationService(IEnumerable<string> roles)
    {
        _roles = new HashSet<string>(roles ?? Array.Empty<string>(), StringComparer.OrdinalIgnoreCase);
        _perms = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var r in _roles)
            if (RolePerms.TryGetValue(r, out var list))
                foreach (var p in list) _perms.Add(p);
    }

    public bool HasRole(string role) => _roles.Contains(role);
    public bool Can(string perm) => _perms.Contains(perm);
}
