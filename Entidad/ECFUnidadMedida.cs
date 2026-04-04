using System;

namespace Andloe.Entidad
{
    public class ECFUnidadMedida
    {
        public int UnidadMedidaECFId { get; set; }
        public string CodigoDGII { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string UnidadInternaCodigo { get; set; } = string.Empty;
        public bool Activo { get; set; }

        public override string ToString()
            => $"{CodigoDGII} - {Descripcion}";
    }
}