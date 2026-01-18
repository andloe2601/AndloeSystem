namespace Andloe.Entidad
{
    public class Usuario
    {
        public int UsuarioId { get; set; }
        public string UsuarioNombre { get; set; } = "";
        public string Email { get; set; } = "";
        public string Estado { get; set; } = "Activo";
        public DateTime? UltimoAcceso { get; set; }

        // Para login seguro:
        public int IntentosFallidos { get; set; }          // NUEVO
        public DateTime? BloqueadoHasta { get; set; }      // NUEVO

        public byte[] HashPassword { get; set; } = Array.Empty<byte>();
        public byte[] Salt { get; set; } = Array.Empty<byte>();
    }
}
