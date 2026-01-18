namespace Andloe.Entidad
{
    public class ClienteDto
    {
        public int ClienteId { get; set; }
        public string Codigo { get; set; } = "";
        public string Nombre { get; set; } = "";
        public string? RncCedula { get; set; }
        public string? Direccion { get; set; }
        public string? Telefono { get; set; }

        // IMPORTANTE: byte, no string
        public byte Tipo { get; set; }


    }
}
