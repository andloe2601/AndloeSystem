using System;
using System.Collections.Generic;
using System.Data;
using Andloe.Data;
using Andloe.Entidad;
using Microsoft.Data.SqlClient;

namespace Andloe.Logica
{
    public class MovimientoInventarioService
    {
        private readonly InvMovimientoCabRepository _cabRepo = new();
        private readonly InvMovimientoLinRepository _linRepo = new();
        private readonly ProductoRepository _productoRepo = new();
        private readonly AuditoriaService _audit = new();

        /// <summary>
        /// Crea un movimiento de inventario de tipo ENTRADA o SALIDA
        /// y actualiza el stock del producto.
        /// </summary>
        public int CrearMovimiento(
            DateTime fecha,
            string tipo,
            int almacenIdOrigen,
            int? almacenIdDestino,
            string usuario,
            string? observacion,
            List<ItemMovimientoDto> lineas)
        {
            if (string.IsNullOrWhiteSpace(usuario))
                throw new InvalidOperationException("Usuario requerido para registrar movimiento.");

            if (lineas == null || lineas.Count == 0)
                throw new InvalidOperationException("Debe agregar al menos una línea.");

            tipo = (tipo ?? "").Trim().ToUpperInvariant();
            if (tipo != "ENTRADA" && tipo != "SALIDA")
                throw new ArgumentException("Tipo inválido. Use ENTRADA o SALIDA.", nameof(tipo));

            using var cn = Db.GetOpenConnection();
            using var tx = cn.BeginTransaction(IsolationLevel.ReadCommitted);

            long invMovId = 0;

            try
            {
                var cab = new InvMovimientoCab
                {
                    Fecha = fecha,
                    Tipo = tipo,
                    Origen = "MANUAL",
                    OrigenId = null,
                    AlmacenIdOrigen = almacenIdOrigen,
                    AlmacenIdDestino = almacenIdDestino,
                    Usuario = usuario,
                    Observacion = observacion ?? "",
                    Estado = "APLICADO",
                };

                invMovId = _cabRepo.InsertCabecera(cab, tx);

                bool permitirNegativo = ConfigService.PermitirStockNegativo;
                int linea = 1;

                foreach (var it in lineas)
                {
                    if (it.Cantidad <= 0)
                        throw new InvalidOperationException($"Cantidad inválida en {it.ProductoCodigo}.");

                    var lin = new InvMovimientoLin
                    {
                        InvMovId = invMovId,
                        Linea = linea,
                        ProductoCodigo = it.ProductoCodigo,
                        Cantidad = it.Cantidad,
                        CostoUnitario = it.CostoUnitario,
                        Usuario = usuario
                    };

                    _linRepo.InsertLinea(lin, tx);

                    if (tipo == "ENTRADA")
                        _productoRepo.SumarStock(it.ProductoCodigo, it.Cantidad, cn, tx);
                    else
                        _productoRepo.RestarStock(it.ProductoCodigo, it.Cantidad, cn, tx, permitirNegativo);

                    linea++;
                }

                tx.Commit();

                _audit.Log(
                    modulo: "INVENTARIO",
                    accion: "MOV_MANUAL",
                    entidad: "InvMovimientoCab",
                    entidadId: invMovId.ToString(),
                    detalle: $"Movimiento manual {tipo}. Lineas={lineas.Count}"
                );

                return (int)invMovId;
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }

        /// <summary>
        /// Revierte (deshace) un movimiento MANUAL aplicado.
        /// Crea un movimiento espejo (Tipo contrario) y ajusta stock.
        /// </summary>
        public long ReversarMovimientoManual(long invMovId, string usuario, string motivo)
        {
            if (invMovId <= 0) throw new ArgumentException("invMovId inválido.");
            if (string.IsNullOrWhiteSpace(usuario)) throw new InvalidOperationException("Usuario requerido.");
            motivo = (motivo ?? "").Trim();

            using var cn = Db.GetOpenConnection();
            using var tx = cn.BeginTransaction(IsolationLevel.Serializable);

            try
            {
                // 1) Leer cabecera original
                string tipoOrig;
                string origen;
                string estado;

                using (var cmd = new SqlCommand(@"
SELECT Tipo, Origen, Estado
FROM dbo.InvMovimientoCab
WHERE InvMovId = @id;", cn, tx))
                {
                    cmd.Parameters.Add("@id", SqlDbType.BigInt).Value = invMovId;
                    using var rd = cmd.ExecuteReader();
                    if (!rd.Read())
                        throw new InvalidOperationException("Movimiento no existe.");

                    tipoOrig = (rd.IsDBNull(0) ? "" : rd.GetString(0)).Trim().ToUpperInvariant();
                    origen = (rd.IsDBNull(1) ? "" : rd.GetString(1)).Trim().ToUpperInvariant();
                    estado = (rd.IsDBNull(2) ? "" : rd.GetString(2)).Trim().ToUpperInvariant();
                }

                if (origen != "MANUAL")
                    throw new InvalidOperationException("Solo se puede reversar movimientos con Origen=MANUAL.");

                if (estado != "APLICADO")
                    throw new InvalidOperationException("Solo se puede reversar movimientos en estado APLICADO.");

                if (tipoOrig != "ENTRADA" && tipoOrig != "SALIDA")
                    throw new InvalidOperationException("Tipo de movimiento inválido para reverso.");

                // 2) Leer líneas originales
                var lineas = new List<(string prod, decimal cant, decimal cost)>();

                using (var cmd = new SqlCommand(@"
SELECT ProductoCodigo, Cantidad, ISNULL(CostoUnitario,0)
FROM dbo.InvMovimientoLin
WHERE InvMovId = @id
ORDER BY Linea;", cn, tx))
                {
                    cmd.Parameters.Add("@id", SqlDbType.BigInt).Value = invMovId;
                    using var rd = cmd.ExecuteReader();
                    while (rd.Read())
                    {
                        var prod = (rd.IsDBNull(0) ? "" : rd.GetString(0)).Trim();
                        var cant = rd.IsDBNull(1) ? 0m : rd.GetDecimal(1);
                        var cost = rd.IsDBNull(2) ? 0m : rd.GetDecimal(2);

                        if (!string.IsNullOrWhiteSpace(prod) && cant > 0)
                            lineas.Add((prod, cant, cost));
                    }
                }

                if (lineas.Count == 0)
                    throw new InvalidOperationException("Movimiento sin líneas, no se puede reversar.");

                // 3) Crear movimiento espejo
                var tipoRev = (tipoOrig == "ENTRADA") ? "SALIDA" : "ENTRADA";

                long invMovRevId;
                using (var cmd = new SqlCommand(@"
INSERT INTO dbo.InvMovimientoCab
(Fecha, Tipo, Origen, OrigenId, AlmacenIdOrigen, AlmacenIdDestino, Usuario, Observacion, Estado, FechaCreacion, UsuarioCreacion)
OUTPUT INSERTED.InvMovId
SELECT
    GETDATE(),
    @Tipo,
    'REVERSO',
    @OrigenId,
    AlmacenIdOrigen,
    AlmacenIdDestino,
    @Usr,
    CONCAT('Reverso de Mov ', @OrigenId, '. Motivo: ', @Motivo),
    'APLICADO',
    GETDATE(),
    @Usr
FROM dbo.InvMovimientoCab
WHERE InvMovId=@OrigenId;", cn, tx))
                {
                    cmd.Parameters.Add("@Tipo", SqlDbType.VarChar, 10).Value = tipoRev;
                    cmd.Parameters.Add("@OrigenId", SqlDbType.BigInt).Value = invMovId;
                    cmd.Parameters.Add("@Usr", SqlDbType.VarChar, 30).Value = usuario.Length > 30 ? usuario.Substring(0, 30) : usuario;
                    cmd.Parameters.Add("@Motivo", SqlDbType.NVarChar, 200).Value = motivo;

                    invMovRevId = Convert.ToInt64(cmd.ExecuteScalar() ?? 0L);
                    if (invMovRevId <= 0) throw new InvalidOperationException("No se pudo crear el reverso.");
                }

                bool permitirNegativo = ConfigService.PermitirStockNegativo;

                int lineaN = 1;
                foreach (var it in lineas)
                {
                    using (var cmd = new SqlCommand(@"
INSERT INTO dbo.InvMovimientoLin
(InvMovId, Linea, ProductoCodigo, Cantidad, CostoUnitario, Usuario)
VALUES
(@mov, @lin, @prod, @cant, @cost, @usr);", cn, tx))
                    {
                        cmd.Parameters.Add("@mov", SqlDbType.BigInt).Value = invMovRevId;
                        cmd.Parameters.Add("@lin", SqlDbType.Int).Value = lineaN;
                        cmd.Parameters.Add("@prod", SqlDbType.VarChar, 20).Value = it.prod;

                        var pCant = cmd.Parameters.Add("@cant", SqlDbType.Decimal);
                        pCant.Precision = 18; pCant.Scale = 4; pCant.Value = it.cant;

                        var pCost = cmd.Parameters.Add("@cost", SqlDbType.Decimal);
                        pCost.Precision = 18; pCost.Scale = 6; pCost.Value = it.cost;

                        cmd.Parameters.Add("@usr", SqlDbType.VarChar, 30).Value = usuario.Length > 30 ? usuario.Substring(0, 30) : usuario;

                        cmd.ExecuteNonQuery();
                    }

                    // Ajuste stock contrario al original
                    if (tipoOrig == "ENTRADA")
                        _productoRepo.RestarStock(it.prod, it.cant, cn, tx, permitirNegativo);
                    else
                        _productoRepo.SumarStock(it.prod, it.cant, cn, tx);

                    lineaN++;
                }

                // 4) Marcar original como REVERSADO
                using (var cmd = new SqlCommand(@"
UPDATE dbo.InvMovimientoCab
SET Estado='REVERSADO',
    Observacion = CASE 
        WHEN Observacion IS NULL OR LTRIM(RTRIM(Observacion))='' THEN CONCAT('REVERSADO por ', @usr, '. Motivo: ', @mot)
        ELSE Observacion + CHAR(10) + CONCAT('REVERSADO por ', @usr, '. Motivo: ', @mot)
    END
WHERE InvMovId=@id;", cn, tx))
                {
                    cmd.Parameters.Add("@id", SqlDbType.BigInt).Value = invMovId;
                    cmd.Parameters.Add("@usr", SqlDbType.VarChar, 30).Value = usuario.Length > 30 ? usuario.Substring(0, 30) : usuario;
                    cmd.Parameters.Add("@mot", SqlDbType.NVarChar, 200).Value = motivo;
                    cmd.ExecuteNonQuery();
                }

                tx.Commit();

                _audit.Log(
                    modulo: "INVENTARIO",
                    accion: "REVERSO_MANUAL",
                    entidad: "InvMovimientoCab",
                    entidadId: invMovId.ToString(),
                    detalle: $"Reverso creado: {invMovRevId}. Motivo: {motivo}"
                );

                return invMovRevId;
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }
    }
}
