// Andloe.Entidad / CierreCajaDto.cs
namespace Andloe.Entidad
{
    public class CierreCajaDto
    {
        public int CajaId { get; set; }
        public string CajaNumero { get; set; } = string.Empty;
        public DateTime FechaDesde { get; set; }
        public DateTime FechaHasta { get; set; }
        public decimal FondoInicial { get; set; }
        public decimal EfectivoDeclarado { get; set; }
        public string UsuarioCierre { get; set; } = string.Empty;
    }
}
