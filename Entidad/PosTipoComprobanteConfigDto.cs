namespace Andloe.Entidad
{
    public class PosTipoComprobanteConfigDto
    {
        public int PosTipoComprobanteConfigId { get; set; }
        public int? CajaId { get; set; }
        public int? SucursalId { get; set; }
        public int TipoECFId { get; set; }
        public string CodigoInterno { get; set; } = string.Empty;
        public string NombreMostrar { get; set; } = string.Empty;
        public bool Activo { get; set; }
        public bool EsDefault { get; set; }
        public int Orden { get; set; }
        public bool RequiereCliente { get; set; }
        public bool RequiereDocumentoCliente { get; set; }
        public bool PermiteEnPOS { get; set; }
    }
}