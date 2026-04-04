using System;
using System.Collections.Generic;

namespace Andloe.Entidad.CxC
{
    public sealed class CxCCobroListadoDto
    {
        public long CobroId { get; set; }
        public string NoRecibo { get; set; } = "";
        public string ClienteNombre { get; set; } = "";
        public string Detalles { get; set; } = "";
        public DateTime Fecha { get; set; }
        public string CuentaMostrar { get; set; } = "";
        public string Estado { get; set; } = "";
        public decimal Monto { get; set; }
    }

    public sealed class CxCFacturaPendienteDto
    {
        public int FacturaId { get; set; }
        public string NumeroDocumento { get; set; } = "";
        public string? ENcf { get; set; }
        public DateTime FechaDocumento { get; set; }
        public DateTime? FechaVencimiento { get; set; }
        public decimal TotalFactura { get; set; }
        public decimal TotalCobrado { get; set; }
        public decimal BalancePendiente { get; set; }
        public decimal Retencion { get; set; }
        public decimal MontoRecibido { get; set; }
    }

    public sealed class CxCCuentaDestinoDto
    {
        public string TipoCuenta { get; set; } = "BANCO"; // BANCO / CAJA
        public int CuentaId { get; set; }
        public string NombreMostrar { get; set; } = "";
        public string MonedaCodigo { get; set; } = "DOP";
    }

    public sealed class CxCCentroCostoDto
    {
        public int CentroCostoId { get; set; }
        public string Codigo { get; set; } = "";
        public string Nombre { get; set; } = "";
        public override string ToString() => string.IsNullOrWhiteSpace(Codigo) ? Nombre : $"{Codigo} - {Nombre}";
    }

    public sealed class CxCClienteLookupDto
    {
        public int ClienteId { get; set; }
        public string Codigo { get; set; } = "";
        public string Nombre { get; set; } = "";
        public string? Documento { get; set; }
        public override string ToString() => string.IsNullOrWhiteSpace(Codigo) ? Nombre : $"{Codigo} - {Nombre}";
    }

    public sealed class CxCCobroAplicacionDto
    {
        public int FacturaId { get; set; }
        public decimal MontoAplicado { get; set; }
        public decimal Retencion { get; set; }
        public string? Nota { get; set; }
    }

    public sealed class CxCCobroCrearDto
    {
        public int EmpresaId { get; set; }
        public int? SucursalId { get; set; }
        public int? ClienteId { get; set; }
        public string? ClienteCodigo { get; set; }
        public string ClienteNombre { get; set; } = "";
        public string? ClienteDoc { get; set; }
        public DateTime Fecha { get; set; } = DateTime.Today;
        public string MonedaCodigo { get; set; } = "DOP";
        public decimal TasaCambio { get; set; } = 1m;
        public string TipoMedio { get; set; } = "EFECTIVO";
        public decimal MontoMedio { get; set; }
        public int? BancoId { get; set; }
        public int? CuentaBancoId { get; set; }
        public string? Referencia { get; set; }
        public string? Observacion { get; set; }
        public int? CentroCostoId { get; set; }
        public string? TipoIngreso { get; set; }
        public bool Postear { get; set; } = true;
        public List<CxCCobroAplicacionDto> Aplicaciones { get; set; } = new();
        public string? NumeroCheque { get; set; }
    }

    public sealed class CxCCobroCrearResultDto
    {
        public long CobroId { get; set; }
        public string NoRecibo { get; set; } = "";
        public string Estado { get; set; } = "";
    }
}
