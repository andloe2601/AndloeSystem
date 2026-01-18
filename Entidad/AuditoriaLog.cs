using System;

namespace Andloe.Entidad
{
    public class AuditoriaLog
    {
        public long AuditoriaId { get; set; }
        public DateTime Fecha { get; set; }

        public int? UsuarioId { get; set; }
        public string? Usuario { get; set; }

        public string Modulo { get; set; } = "";
        public string Accion { get; set; } = "";

        public string? Entidad { get; set; }
        public string? EntidadId { get; set; }

        public string? Detalle { get; set; }

        public string? AntesJson { get; set; }
        public string? DespuesJson { get; set; }

        public string? Maquina { get; set; }
        public string? Ip { get; set; }
    }
}
