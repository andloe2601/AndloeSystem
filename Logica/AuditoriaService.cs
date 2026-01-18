using Andloe.Data;
using Andloe.Entidad;
using System;

namespace Andloe.Logica
{
    public class AuditoriaService
    {
        private readonly AuditoriaRepository _repo = new();

        public void Log(
            string modulo,
            string accion,
            string? entidad = null,
            string? entidadId = null,
            string? detalle = null,
            string? antesJson = null,
            string? despuesJson = null)
        {
            try
            {
                var log = new AuditoriaLog
                {
                    Fecha = DateTime.Now,
                    UsuarioId = AuditContext.UsuarioId,
                    Usuario = AuditContext.Usuario,

                    Modulo = (modulo ?? "").Trim(),
                    Accion = (accion ?? "").Trim(),

                    Entidad = entidad,
                    EntidadId = entidadId,

                    Detalle = detalle,
                    AntesJson = antesJson,
                    DespuesJson = despuesJson,

                    Maquina = AuditContext.Maquina,
                    Ip = AuditContext.Ip
                };

                _repo.Insert(log);
            }
            catch
            {
                // Auditoría nunca debe tumbar la app.
            }
        }
    }
}
