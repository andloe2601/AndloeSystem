using System;

namespace Andloe.Entidad
{
    public class SesionActual
    {
        public int UsuarioId { get; set; }
        public string Usuario { get; set; } = "";

        public int EmpresaId { get; set; }
        public int SucursalId { get; set; }
        public int AlmacenId { get; set; }

        // No está en UsuarioContexto en tu BD -> se maneja en memoria
        public int? CajaId { get; set; }

        public bool EstaCompletaBase()
            => UsuarioId > 0 && EmpresaId > 0 && SucursalId > 0 && AlmacenId > 0;

        public void ValidarCompletaBase()
        {
            if (!EstaCompletaBase())
                throw new InvalidOperationException("Sesión incompleta: falta Empresa/Sucursal/Almacén o UsuarioId.");
        }
    }
}
