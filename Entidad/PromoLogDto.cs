public sealed class PromoLogDto
{
    public int PromoId { get; set; }

    public string Origen { get; set; } = "VENTA"; // VENTA|DEVOLUCION|AJUSTE...
    public long OrigenId { get; set; }            // ej: VentaId

    public int? Linea { get; set; }               // opcional: #linea o VentaDetId

    public decimal MontoDesc { get; set; }        // (18,2)
    public string? MonedaCodigo { get; set; }     // (3)
    public decimal? TasaCambio { get; set; }      // (18,6)

    public string? Usuario { get; set; }          // (50)
    public DateTime FechaAplic { get; set; } = DateTime.Now;
    public DateTime Fecha { get; set; } = DateTime.Now; // si no hay default en DB
}

public sealed class PromoTopeDeltaDto
{
    public int PromoId { get; set; }

    public string PeriodoTipo { get; set; } = "DIARIO"; // GENERAL|DIARIO|MENSUAL
    public string PeriodoClave { get; set; } = DateTime.Now.ToString("yyyyMMdd");

    public string? ClienteCodigo { get; set; }          // null = global
    public string? ProductoCodigo { get; set; }         // null = global

    public int UsosDelta { get; set; } = 1;
    public decimal MontoDelta { get; set; }

    public DateTime UltimaFecha { get; set; } = DateTime.Now;
}
