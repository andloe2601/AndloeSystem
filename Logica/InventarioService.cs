using Andloe.Data;
using Andloe.Entidad;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace Andloe.Logica
{
    /// <summary>
    /// Servicio central para registrar entradas/salidas de inventario
    /// y consultar Kardex de un producto.
    /// </summary>
    public class InventarioService
    {
        private readonly InvMovimientoCabRepository _cabRepo = new();
        private readonly InvMovimientoLinRepository _linRepo = new();
        private readonly ProductoRepository _prodRepo = new();

        /// <summary>
        /// Registra una ENTRADA de inventario.
        /// Ej: compra, ajuste positivo, devolución de cliente, etc.
        /// </summary>
        /// <param name="usuario">Usuario que realiza el movimiento.</param>
        /// <param name="almacenId">Almacén donde entra la mercancía.</param>
        /// <param name="origen">Texto de origen (COMPRA, AJUSTE, DEVOLUCION, etc.).</param>
        /// <param name="origenId">Id del documento origen (opcional).</param>
        /// <param name="observacion">Descripción libre.</param>
        /// <param name="lineas">
        /// Lista de tuplas (ProductoCodigo, Cantidad, CostoUnitario).
        /// </param>
        public long RegistrarEntrada(
            string usuario,
            int almacenId,
            string origen,
            long? origenId,
            string observacion,
            IEnumerable<(string ProductoCodigo, decimal Cantidad, decimal CostoUnitario)> lineas)
        {
            if (almacenId <= 0)
                throw new ArgumentException("Debe especificar un almacén válido para la entrada.", nameof(almacenId));

            if (lineas == null)
                throw new ArgumentNullException(nameof(lineas));

            var lista = new List<(string ProductoCodigo, decimal Cantidad, decimal CostoUnitario)>(lineas);
            if (lista.Count == 0)
                throw new InvalidOperationException("No hay líneas de inventario para registrar.");

            var ahora = DateTime.Now;

            using var cn = Db.GetOpenConnection();
            using var tx = cn.BeginTransaction(IsolationLevel.ReadCommitted);

            try
            {
                var cab = new InvMovimientoCab
                {
                    Fecha = ahora,
                    Tipo = "ENTRADA",
                    Origen = origen,
                    OrigenId = origenId,
                    AlmacenIdOrigen = null,
                    AlmacenIdDestino = almacenId,
                    Usuario = usuario,
                    Observacion = string.IsNullOrWhiteSpace(observacion) ? "Entrada inventario" : observacion,
                    Estado = "APLICADO"
                };

                var movId = _cabRepo.InsertCabecera(cab, tx);

                int linea = 1;
                foreach (var l in lista)
                {
                    if (l.Cantidad <= 0) continue;

                    // Actualizamos stock total del producto
                    _prodRepo.SumarStock(l.ProductoCodigo, l.Cantidad, cn, tx);

                    var lin = new InvMovimientoLin
                    {
                        InvMovId = movId,
                        Linea = linea,
                        ProductoCodigo = l.ProductoCodigo,
                        Cantidad = l.Cantidad,
                        CostoUnitario = l.CostoUnitario
                    };

                    _linRepo.InsertLinea(lin, tx);
                    linea++;
                }

                tx.Commit();
                return movId;
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }

        /// <summary>
        /// Registra una SALIDA de inventario.
        /// Ej: ajuste negativo, consumo interno, merma, etc.
        /// </summary>
        public long RegistrarSalida(
            string usuario,
            int almacenId,
            string origen,
            long? origenId,
            string observacion,
            IEnumerable<(string ProductoCodigo, decimal Cantidad, decimal CostoUnitario)> lineas,
            bool permitirStockNegativo = false)
        {
            if (almacenId <= 0)
                throw new ArgumentException("Debe especificar un almacén válido para la salida.", nameof(almacenId));

            if (lineas == null)
                throw new ArgumentNullException(nameof(lineas));

            var lista = new List<(string ProductoCodigo, decimal Cantidad, decimal CostoUnitario)>(lineas);
            if (lista.Count == 0)
                throw new InvalidOperationException("No hay líneas de inventario para registrar.");

            var ahora = DateTime.Now;

            using var cn = Db.GetOpenConnection();
            using var tx = cn.BeginTransaction(IsolationLevel.ReadCommitted);

            try
            {
                var cab = new InvMovimientoCab
                {
                    Fecha = ahora,
                    Tipo = "SALIDA",
                    Origen = origen,
                    OrigenId = origenId,
                    AlmacenIdOrigen = almacenId,
                    AlmacenIdDestino = null,
                    Usuario = usuario,
                    Observacion = string.IsNullOrWhiteSpace(observacion) ? "Salida inventario" : observacion,
                    Estado = "APLICADO"
                };

                var movId = _cabRepo.InsertCabecera(cab, tx);

                int linea = 1;
                foreach (var l in lista)
                {
                    if (l.Cantidad <= 0) continue;

                    // Descontamos stock (según flag permitirStockNegativo)
                    _prodRepo.RestarStock(
                        l.ProductoCodigo,
                        l.Cantidad,
                        cn,
                        tx,
                        permitirNegativo: permitirStockNegativo);

                    var lin = new InvMovimientoLin
                    {
                        InvMovId = movId,
                        Linea = linea,
                        ProductoCodigo = l.ProductoCodigo,
                        Cantidad = l.Cantidad,
                        CostoUnitario = l.CostoUnitario
                    };

                    _linRepo.InsertLinea(lin, tx);
                    linea++;
                }

                tx.Commit();
                return movId;
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }

        /// <summary>
        /// Registra un TRASLADO entre almacenes.
        /// Suma en destino, resta en origen.
        /// </summary>
        public long RegistrarTraslado(
            string usuario,
            int almacenOrigenId,
            int almacenDestinoId,
            string origen,
            long? origenId,
            string observacion,
            IEnumerable<(string ProductoCodigo, decimal Cantidad, decimal CostoUnitario)> lineas,
            bool permitirStockNegativo = false)
        {
            if (almacenOrigenId <= 0 || almacenDestinoId <= 0)
                throw new ArgumentException("Debe especificar almacenes válidos para traslado.");

            if (almacenOrigenId == almacenDestinoId)
                throw new ArgumentException("El almacén origen y destino no pueden ser el mismo.");

            if (lineas == null)
                throw new ArgumentNullException(nameof(lineas));

            var lista = new List<(string ProductoCodigo, decimal Cantidad, decimal CostoUnitario)>(lineas);
            if (lista.Count == 0)
                throw new InvalidOperationException("No hay líneas de inventario para registrar.");

            var ahora = DateTime.Now;

            using var cn = Db.GetOpenConnection();
            using var tx = cn.BeginTransaction(IsolationLevel.ReadCommitted);

            try
            {
                var cab = new InvMovimientoCab
                {
                    Fecha = ahora,
                    Tipo = "TRASLADO",
                    Origen = origen,
                    OrigenId = origenId,
                    AlmacenIdOrigen = almacenOrigenId,
                    AlmacenIdDestino = almacenDestinoId,
                    Usuario = usuario,
                    Observacion = string.IsNullOrWhiteSpace(observacion) ? "Traslado de inventario" : observacion,
                    Estado = "APLICADO"
                };

                var movId = _cabRepo.InsertCabecera(cab, tx);

                int linea = 1;
                foreach (var l in lista)
                {
                    if (l.Cantidad <= 0) continue;

                    // Restamos del origen
                    _prodRepo.RestarStock(
                        l.ProductoCodigo,
                        l.Cantidad,
                        cn,
                        tx,
                        permitirNegativo: permitirStockNegativo);

                    // Sumamos al destino (a nivel total de producto)
                    _prodRepo.SumarStock(l.ProductoCodigo, l.Cantidad, cn, tx);

                    var lin = new InvMovimientoLin
                    {
                        InvMovId = movId,
                        Linea = linea,
                        ProductoCodigo = l.ProductoCodigo,
                        Cantidad = l.Cantidad,
                        CostoUnitario = l.CostoUnitario
                    };

                    _linRepo.InsertLinea(lin, tx);
                    linea++;
                }

                tx.Commit();
                return movId;
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }
    }
}
