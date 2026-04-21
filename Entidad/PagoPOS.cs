using Andloe.Entidad;
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
        /// <summary>Total a cobrar en moneda base (DOP).</summary>
        public decimal TotalBase { get; set; }

        /// <summary>Lista de líneas de pago (efectivo, tarjeta, etc.).</summary>
        public List<PagoLineaResult> Pagos { get; } = new();

        /// <summary>Suma de todos los pagos en base (DOP).</summary>
        public decimal PagadoBase => Math.Round(Pagos.Sum(p => p.MontoBase), 2);

        /// <summary>Diferencia = TotalBase - PagadoBase. Positivo = falta por cobrar; negativo = cambio.</summary>
        public decimal DiferenciaBase => Math.Round(TotalBase - PagadoBase, 2);

        public bool IncluidoEnCierre { get; set; }
    }
}
namespace Entidad
{
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
        public int VentaId { get; set; }
        public string Estado { get; set; } = "APLICADO";
        public string? POS_CajaNumero { get; set; }
    }
}