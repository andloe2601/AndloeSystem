using System;
using System.Collections.Generic;
using Andloe.Data;
using Andloe.Entidad;

namespace Andloe.Logica
{
    public class TerminoPagoService
    {
        private readonly TerminoPagoRepository _repo = new();

        public List<TerminoPago> ListarTodos() => _repo.ListarTodos();
        public List<TerminoPago> ListarActivos() => _repo.ListarActivos();
        public TerminoPago? ObtenerPorId(int id) => _repo.ObtenerPorId(id);

        public int Crear(TerminoPago t)
        {
            Validar(t, esNuevo: true);
            return _repo.Insertar(t);
        }

        public void Actualizar(TerminoPago t)
        {
            if (t.TerminoPagoId <= 0)
                throw new ArgumentException("ID inválido para actualizar.", nameof(t.TerminoPagoId));

            Validar(t, esNuevo: false);
            _repo.Actualizar(t);
        }

        private void Validar(TerminoPago t, bool esNuevo)
        {
            if (string.IsNullOrWhiteSpace(t.Codigo))
                throw new InvalidOperationException("El código del término de pago es obligatorio.");

            if (string.IsNullOrWhiteSpace(t.Descripcion))
                throw new InvalidOperationException("La descripción es obligatoria.");

            if (t.DiasPlazo < 0)
                throw new InvalidOperationException("Los días de plazo no pueden ser negativos.");

            if (t.DiasPlazo > 0)
            {
                t.CantCuotas = null;
                t.FrecuenciaDias = null;
            }
            else
            {
                if (!t.CantCuotas.HasValue || t.CantCuotas.Value <= 0)
                    throw new InvalidOperationException("Si DiasPlazo es 0, debes indicar CantCuotas (> 0) para plan de pago.");

                if (!t.FrecuenciaDias.HasValue || t.FrecuenciaDias.Value <= 0)
                    t.FrecuenciaDias = 30;
            }

            if (t.TieneDescuento)
            {
                if (!t.PorcDescuento.HasValue || t.PorcDescuento.Value <= 0)
                    throw new InvalidOperationException("Debe indicar un porcentaje de descuento mayor a 0.");

                if (!t.DiasDescuento.HasValue || t.DiasDescuento.Value <= 0)
                    throw new InvalidOperationException("Debe indicar los días de descuento mayores a 0.");
            }
            else
            {
                t.PorcDescuento = null;
                t.DiasDescuento = null;
            }

            int? excluirId = esNuevo ? null : t.TerminoPagoId;
            if (_repo.ExisteCodigo(t.Codigo, excluirId))
                throw new InvalidOperationException("Ya existe un término de pago con ese código.");
        }
    }
}