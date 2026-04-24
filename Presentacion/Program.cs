using Andloe.Data;
using Andloe.Logica;
using Andloe.Presentacion;

namespace Andloe.Presentacion;

internal static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();

        if (!ConfigManager.ConfigExists())
        {
            using var cfg = new FormConexion();
            if (cfg.ShowDialog() != DialogResult.OK)
                return;
        }

        var cs = ConfigManager.GetConnectionString();
        Db.Init(cs);

        Application.Run(new FormLogin());
    }
}