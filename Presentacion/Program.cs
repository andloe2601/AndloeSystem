using Andloe.Data;
using Andloe.Logica;
using Presentacion;
using Presentation;

namespace Andloe.Presentacion;

internal static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();

        // Si no hay archivo, mostrar FormConexion antes de todo
        if (!ConfigManager.ConfigExists())
        {
            using var cfg = new FormConexion();
            if (cfg.ShowDialog() != DialogResult.OK)
                return; // usuario canceló
        }

        // Inicializar DB con lo leído del archivo
        var cs = ConfigManager.GetConnectionString();
        Db.Init(cs);

        Application.Run(new FormLogin());


    }
}
