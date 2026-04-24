using System;
using System.Collections.Generic;
using System.Linq;

namespace Andloe.Entidad
{
    public class PagoLineaResult
    {
        public string FormaPagoCodigo { get; set; } = "";
        public string NombreMedio { get; set; } = "";
        public string MonedaCodigo { get; set; } = "";
        public decimal TasaCambio { get; set; }
        public decimal MontoMoneda { get; set; }

        public decimal MontoBase => Math.Round(MontoMoneda * TasaCambio, 2);
    }

    public class SeleccionPagoResult
    {
        public decimal TotalBase { get; set; }

        public List<PagoLineaResult> Pagos { get; } = new();

        public decimal PagadoBase => Math.Round(Pagos.Sum(p => p.MontoBase), 2);

        public decimal DiferenciaBase => Math.Round(TotalBase - PagadoBase, 2);

        public bool IncluidoEnCierre { get; set; }
    }

    public class PagoPOS
    {
        public DateTime Fecha { get; set; }
        public string? MonedaCodigo { get; set; }
        public decimal TasaCambio { get; set; }
        public decimal Monto { get; set; }
        public decimal MontoBase { get; set; }
        public string FormaPagoCodigo { get; set; } = "";
        public string? Referencia { get; set; }
        public string? Entidad { get; set; }
        public string? Observacion { get; set; }
        public string? Usuario { get; set; }
        public int? CajaId { get; set; }
        public long VentaId { get; set; }
        public string Estado { get; set; } = "APLICADO";
        public string? POS_CajaNumero { get; set; }
    }
}