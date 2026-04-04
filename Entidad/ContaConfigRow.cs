#nullable enable
using System;

namespace Andloe.Entidad
{
    public sealed class ContaConfigRow
    {
        public int ConfigId { get; set; }

        public string Modulo { get; set; } = "";
        public string Evento { get; set; } = "";
        public string Rol { get; set; } = "";

        public string Naturaleza { get; set; } = "DEBITO"; // DEBITO / CREDITO
        public int Orden { get; set; } = 1;

        public int? CuentaId { get; set; }
        public string? CuentaCodigo { get; set; }
        public string? CuentaNombre { get; set; }

        public bool Activo { get; set; } = true;
        public DateTime FechaCreacion { get; set; }
    }

    public sealed class CuentaLookupRow
    {
        public int CuentaId { get; set; }
        public string Codigo { get; set; } = "";
        public string Nombre { get; set; } = "";
        public string Display => $"{Codigo} - {Nombre}";
    }
}
#nullable restore
