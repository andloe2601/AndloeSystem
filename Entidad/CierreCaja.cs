using System;

namespace Andloe.Entidad
{
    public class CierreCaja
    {
        public long CierreId { get; set; }

        public int CajaId { get; set; }
        public string POS_CajaNumero { get; set; } = string.Empty;

        public DateTime FechaDesde { get; set; }
        public DateTime FechaHasta { get; set; }

        public decimal FondoInicial { get; set; }
        public decimal TotalVentas { get; set; }
        public decimal TotalPagos { get; set; }
        public decimal EfectivoTeorico { get; set; }
        public decimal EfectivoDeclarado { get; set; }
        public decimal Diferencia { get; set; }

        public string UsuarioCierre { get; set; } = string.Empty;
        public DateTime FechaCierre { get; set; }
        public string Estado { get; set; } = "CERRADO";
    }

    public class CierreCajaResumen
    {
        public decimal TotalVentas { get; set; }
        public decimal TotalPagos { get; set; }
        public decimal EfectivoTeorico { get; set; }


    }
}
