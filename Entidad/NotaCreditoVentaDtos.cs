using System;
using System.Collections.Generic;

namespace Andloe.Entidad
{
    public sealed class NotaCreditoVentaCabDto
    {
        public long NCId { get; set; }
        public string NoDocumento { get; set; } = "";
        public DateTime Fecha { get; set; } = DateTime.Now;
        public long? VentaIdOrigen { get; set; }
        public int EmpresaId { get; set; }
        public int SucursalId { get; set; }
        public int AlmacenId { get; set; }
        public string ClienteCodigo { get; set; } = "";
        public string ClienteNombre { get; set; } = "";
        public string? DocumentoCliente { get; set; }
        public string MonedaCodigo { get; set; } = "DOP";
        public decimal TasaCambio { get; set; } = 1m;
        public decimal SubTotalMoneda { get; set; }
        public decimal ItbisMoneda { get; set; }
        public decimal TotalMoneda { get; set; }
        public string Estado { get; set; } = "EMITIDA";
        public string? Usuario { get; set; }
        public string? Motivo { get; set; }
        public string? ENCF { get; set; }
        public string? ENcfOrigen { get; set; }
        public string? NoDocumentoOrigen { get; set; }
    }

    public sealed class NotaCreditoVentaLinDto
    {
        public bool Seleccionado { get; set; } = true;
        public int Linea { get; set; }
        public string ProductoCodigo { get; set; } = "";
        public string Descripcion { get; set; } = "";
        public decimal CantidadOriginal { get; set; }
        public decimal CantidadNC { get; set; }
        public decimal PrecioUnit { get; set; }
        public decimal ItbisPct { get; set; }
        public decimal DescuentoMoneda { get; set; }
        public decimal SubTotalMoneda { get; set; }
        public decimal ItbisMoneda { get; set; }
        public decimal TotalMoneda { get; set; }
        public decimal SubTotalNcCalculado { get; set; }
        public decimal ItbisNcCalculado { get; set; }
        public decimal TotalNcCalculado { get; set; }
    }

    public sealed class NotaCreditoVentaDto
    {
        public NotaCreditoVentaCabDto Cab { get; set; } = new();
        public List<NotaCreditoVentaLinDto> Lineas { get; set; } = new();
    }

    public sealed class VentaOrigenDto
    {
        public long VentaId { get; set; }
        public string NoDocumento { get; set; } = "";
        public DateTime Fecha { get; set; }
        public string ClienteCodigo { get; set; } = "";
        public string ClienteNombre { get; set; } = "";
        public string? DocumentoCliente { get; set; }
        public string MonedaCodigo { get; set; } = "DOP";
        public decimal TasaCambio { get; set; } = 1m;
        public decimal TotalMoneda { get; set; }
        public string Estado { get; set; } = "";
    }

    public sealed class NcfTipoDto
    {
        public int TipoId { get; set; }
        public string Codigo { get; set; } = "";
        public string Descripcion { get; set; } = "";
        public override string ToString() => string.IsNullOrWhiteSpace(Descripcion) ? Codigo : $"{Codigo} - {Descripcion}";
    }
}
