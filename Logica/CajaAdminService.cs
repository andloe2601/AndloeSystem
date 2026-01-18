using Andloe.Data;
using System;
using System.Collections.Generic;

namespace Andloe.Logica
{
    /// <summary>
    /// Servicio para administrar cajas (crear, editar, eliminar)
    /// validando por sucursal que no se repitan números de caja.
    /// </summary>
    public class CajaAdminService
    {
        private readonly CajaRepository _repo = new();

        // ==========================
        //     LISTADOS / CONSULTAS
        // ==========================

        /// <summary>
        /// Lista todas las cajas activas (todas las sucursales).
        /// </summary>
        public List<CajaDto> ListarActivas()
            => _repo.ListarActivas();

        /// <summary>
        /// Lista las cajas de una sucursal específica.
        /// </summary>
        public List<CajaDto> ListarPorSucursal(int sucursalId, bool soloActivas = false)
        {
            if (sucursalId <= 0)
                throw new ArgumentException("SucursalId inválido.", nameof(sucursalId));

            return _repo.ListarPorSucursal(sucursalId, soloActivas);
        }

        /// <summary>
        /// Busca una caja por su ID.
        /// </summary>
        public CajaDto? ObtenerPorId(int cajaId)
        {
            if (cajaId <= 0)
                return null;

            return _repo.ObtenerPorId(cajaId);
        }

        // ==========================
        //       CREAR / EDITAR
        // ==========================

        /// <summary>
        /// Crea una caja nueva.
        /// El CajaId se genera automáticamente con MAX(CajaId) + 1.
        /// </summary>
        public int CrearCaja(int sucursalId, string cajaNumero, string? descripcion, string estado)
        {
            if (sucursalId <= 0)
                throw new ArgumentException("Debe seleccionar una sucursal válida.", nameof(sucursalId));
            if (string.IsNullOrWhiteSpace(cajaNumero))
                throw new ArgumentException("El número de caja es obligatorio.", nameof(cajaNumero));

            if (_repo.ExisteNumeroEnSucursal(sucursalId, cajaNumero))
                throw new InvalidOperationException($"Ya existe la caja {cajaNumero} en esa sucursal.");

            var caja = new CajaDto
            {
                SucursalId = sucursalId,
                CajaNumero = cajaNumero.Trim(),
                Descripcion = string.IsNullOrWhiteSpace(descripcion) ? null : descripcion.Trim(),
                Estado = string.IsNullOrWhiteSpace(estado) ? "ACTIVA" : estado.Trim().ToUpper()
            };

            // 👉 aquí recibes el nuevo ID
            int nuevoId = _repo.InsertarCaja(caja);
            return nuevoId;
        }


        /// <summary>
        /// Actualiza una caja existente.
        /// </summary>
        public void ActualizarCaja(int cajaId, int sucursalId, string cajaNumero, string? descripcion, string estado)
        {
            if (cajaId <= 0)
                throw new ArgumentException("CajaId inválido.", nameof(cajaId));

            if (sucursalId <= 0)
                throw new ArgumentException("Debe seleccionar una sucursal válida.", nameof(sucursalId));

            if (string.IsNullOrWhiteSpace(cajaNumero))
                throw new ArgumentException("El número de caja es obligatorio.", nameof(cajaNumero));

            // Validar que no se repita el número en la sucursal (excluyendo esta misma caja)
            if (_repo.ExisteNumeroEnSucursal(sucursalId, cajaNumero, cajaId))
                throw new InvalidOperationException($"Ya existe la caja {cajaNumero} en esa sucursal.");

            var caja = new CajaDto
            {
                CajaId = cajaId,
                SucursalId = sucursalId,
                CajaNumero = cajaNumero.Trim(),
                Descripcion = string.IsNullOrWhiteSpace(descripcion) ? null : descripcion.Trim(),
                Estado = string.IsNullOrWhiteSpace(estado) ? "ACTIVA" : estado.Trim().ToUpper()
            };

            _repo.ActualizarCaja(caja);
        }

        // ==========================
        //          ELIMINAR
        // ==========================

        /// <summary>
        /// Elimina una caja por ID.
        /// </summary>
        public void EliminarCaja(int cajaId)
        {
            if (cajaId <= 0)
                throw new ArgumentException("CajaId inválido.", nameof(cajaId));

            _repo.EliminarCaja(cajaId);
        }


    }
}
