using Andloe.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Andloe.Logica
{
    public sealed class ContabilidadCatalogoCuentaService
    {
        private readonly ContabilidadCatalogoCuentaRepository _repo = new();

        public List<CtaRow> Listar(string? filtro = null) => _repo.Listar(filtro);

        public int Guardar(CtaRow c)
        {
            if (string.IsNullOrWhiteSpace(c.Codigo))
                throw new InvalidOperationException("Código requerido.");
            if (string.IsNullOrWhiteSpace(c.Descripcion))
                throw new InvalidOperationException("Descripción requerida.");
            if (string.IsNullOrWhiteSpace(c.Tipo))
                throw new InvalidOperationException("Tipo requerido.");

            c.Codigo = c.Codigo.Trim();
            c.Descripcion = c.Descripcion.Trim();
            c.Tipo = c.Tipo.Trim().ToUpperInvariant();
            c.Estado = string.IsNullOrWhiteSpace(c.Estado) ? "ACTIVO" : c.Estado.Trim().ToUpperInvariant();

            // Nivel: si tiene padre => padre.Nivel + 1
            if (c.PadreId.HasValue)
            {
                var padre = _repo.Listar().FirstOrDefault(x => x.CuentaId == c.PadreId.Value);
                if (padre == null) throw new InvalidOperationException("Cuenta padre inválida.");
                c.Nivel = padre.Nivel + 1;
            }
            else
            {
                c.Nivel = 1;
            }

            // Unicidad de código
            if (_repo.ExisteCodigo(c.Codigo, c.CuentaId == 0 ? null : c.CuentaId))
                throw new InvalidOperationException($"Ya existe una cuenta con el código '{c.Codigo}'.");

            if (c.CuentaId == 0)
                return _repo.Insertar(c);

            _repo.Actualizar(c);
            return c.CuentaId;
        }

        public void Eliminar(int cuentaId)
        {
            if (_repo.TieneHijos(cuentaId))
                throw new InvalidOperationException("No se puede eliminar: esta cuenta tiene hijos.");

            _repo.Eliminar(cuentaId);
        }

        public void SetEstado(int cuentaId, bool activo)
        {
            _repo.SetEstado(cuentaId, activo ? "ACTIVO" : "INACTIVO");
        }
    }
}
