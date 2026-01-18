using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Andloe.Entidad;
using Microsoft.Data.SqlClient;

namespace Andloe.Data
{
    public class CierreCajaRepository
    {
        // =====================================
        //  RESUMEN PARA CIERRE (NO INCLUIDOS)
        // =====================================
        public CierreCajaResumen CalcularResumen(string cajaNumero, DateTime desde, DateTime hasta)
        {
            if (string.IsNullOrWhiteSpace(cajaNumero))
                throw new ArgumentException("El número de caja es obligatorio.", nameof(cajaNumero));

            using var cn = Db.GetOpenConnection();
            decimal totalVentas = 0m;
            decimal totalPagos = 0m;

            // FACTURAS NO INCLUIDAS EN CIERRE
            using (var cmd = new SqlCommand(@"
                SELECT ISNULL(SUM(Total), 0)
                FROM dbo.VentaCab
                WHERE Fecha        >= @Desde
                  AND Fecha        <= @Hasta
                  AND POS_CajaNumero = @CajaNumero
                  AND Estado       = 'FACTURADA'
                  AND IncluidaEnCierre = 0;", cn))
            {
                cmd.Parameters.Add("@Desde", SqlDbType.DateTime).Value = desde;
                cmd.Parameters.Add("@Hasta", SqlDbType.DateTime).Value = hasta;
                cmd.Parameters.Add("@CajaNumero", SqlDbType.NVarChar, 10).Value = cajaNumero.Trim();

                var obj = cmd.ExecuteScalar();
                if (obj != null && obj != DBNull.Value)
                    totalVentas = Convert.ToDecimal(obj);
            }

            // PAGOS NO INCLUIDOS EN CIERRE
            using (var cmd = new SqlCommand(@"
                SELECT ISNULL(SUM(Monto), 0)
                FROM dbo.POS_Pago
                WHERE Fecha            >= @Desde
                  AND Fecha            <= @Hasta
                  AND (POS_CajaNumero  = @CajaNumero OR ISNULL(POS_CajaNumero,'') = '')
                  AND Estado           = 'APLICADO'
                  AND (IncluidoEnCierre = 0 OR IncluidoEnCierre IS NULL);", cn))
            {
                cmd.Parameters.Add("@Desde", SqlDbType.DateTime).Value = desde;
                cmd.Parameters.Add("@Hasta", SqlDbType.DateTime).Value = hasta;
                cmd.Parameters.Add("@CajaNumero", SqlDbType.NVarChar, 10).Value = cajaNumero.Trim();

                var obj = cmd.ExecuteScalar();
                if (obj != null && obj != DBNull.Value)
                    totalPagos = Convert.ToDecimal(obj);
            }

            return new CierreCajaResumen
            {
                TotalVentas = totalVentas,
                TotalPagos = totalPagos,
                EfectivoTeorico = totalPagos
            };
        }

        // =====================================
        //  CONSULTAS POR CIERREID (DETALLE)
        // =====================================

        // Trae ventas por caja+rango del cierre, con término de pago
        public DataTable ListarVentasPorCierre(long cierreId)
        {
            using var cn = Db.GetOpenConnection();

            using var cmd = new SqlCommand(@"
                SELECT
                    v.VentaId,
                    v.NoDocumento,
                    v.Fecha,
                    v.ClienteCodigo,
                    v.MonedaCodigo,
                    v.SubTotalMoneda,
                    v.ItbisMoneda,
                    v.TotalMoneda,
                    v.Estado,
                    v.MedioPagoId,
                    mp.Nombre       AS MedioPago,
                    v.MontoPago,
                    v.TerminoPagoId,
                    tp.Descripcion  AS TerminoPago
                FROM dbo.CajaCierreCab c
                INNER JOIN dbo.VentaCab v
                    ON  v.POS_CajaNumero = c.CajaNumero
                    AND v.CajaId         = c.CajaId
                    AND v.Fecha         >= c.FechaDesde
                    AND v.Fecha         <= c.FechaHasta
                LEFT JOIN dbo.MedioPago mp
                    ON mp.MedioPagoId = v.MedioPagoId
                LEFT JOIN dbo.TerminoPago tp
                    ON tp.TerminoPagoId = v.TerminoPagoId
                WHERE c.CierreId = @CierreId
                  AND v.Estado   = 'FACTURADA'
                ORDER BY v.Fecha;", cn);

            cmd.Parameters.Add("@CierreId", SqlDbType.BigInt).Value = cierreId;

            using var da = new SqlDataAdapter(cmd);
            var dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        public DataTable ListarPagosPorCierre(long cierreId)
        {
            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
                SELECT
                    p.Fecha,
                    mp.Nombre        AS MedioPago,
                    p.MonedaCodigo,
                    p.Monto,
                    p.MontoBase,
                    p.TasaCambio,
                    p.Referencia,
                    p.Entidad,
                    p.Observacion,
                    p.Usuario
                FROM dbo.POS_Pago p
                LEFT JOIN dbo.MedioPago mp ON mp.MedioPagoId = p.MedioPagoId
                WHERE p.CierreId = @CierreId
                ORDER BY p.Fecha;", cn);

            cmd.Parameters.Add("@CierreId", SqlDbType.BigInt).Value = cierreId;

            using var da = new SqlDataAdapter(cmd);
            var dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        public DataTable ListarFondoPorCierre(long cierreId)
        {
            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
                SELECT
                    f.FondoId,
                    f.POS_CajaNumero,
                    f.FechaApertura,
                    f.UsuarioApertura,
                    f.MontoFondo,
                    f.Observacion,
                    f.Estado,
                    f.FechaCierre
                FROM dbo.POS_FondoCaja f
                WHERE f.CierreId = @CierreId
                ORDER BY f.FechaApertura;", cn);

            cmd.Parameters.Add("@CierreId", SqlDbType.BigInt).Value = cierreId;

            using var da = new SqlDataAdapter(cmd);
            var dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        // =====================================
        //  INSERTAR CIERRE + MARCAR VENTAS/PAGOS
        // =====================================
        public long InsertarCierre(CierreCaja c)
        {
            if (c == null) throw new ArgumentNullException(nameof(c));
            if (string.IsNullOrWhiteSpace(c.POS_CajaNumero))
                throw new InvalidOperationException("No se recibió el número de caja para el cierre.");

            using var cn = Db.GetOpenConnection();
            using var tx = cn.BeginTransaction(IsolationLevel.ReadCommitted);

            try
            {
                long cierreId;

                // INSERT en la cabecera de cierre (CajaCierreCab)
                using (var cmd = new SqlCommand(@"
                    INSERT INTO dbo.CajaCierreCab
                    (
                        CajaId,
                        CajaNumero,
                        FechaDesde,
                        FechaHasta,
                        FondoInicial,
                        TotalVentas,
                        TotalPagos,
                        EfectivoTeorico,
                        EfectivoDeclarado,
                        Diferencia,
                        UsuarioCierre,
                        FechaCierre,
                        Estado
                    )
                    VALUES
                    (
                        @CajaId,
                        @CajaNumero,
                        @FechaDesde,
                        @FechaHasta,
                        @FondoInicial,
                        @TotalVentas,
                        @TotalPagos,
                        @EfectivoTeorico,
                        @EfectivoDeclarado,
                        @Diferencia,
                        @UsuarioCierre,
                        @FechaCierre,
                        @Estado
                    );
                    SELECT CAST(SCOPE_IDENTITY() AS BIGINT);", cn, tx))
                {
                    cmd.Parameters.Add("@CajaId", SqlDbType.Int).Value = c.CajaId;
                    cmd.Parameters.Add("@CajaNumero", SqlDbType.NVarChar, 10).Value = c.POS_CajaNumero!.Trim();
                    cmd.Parameters.Add("@FechaDesde", SqlDbType.DateTime).Value = c.FechaDesde;
                    cmd.Parameters.Add("@FechaHasta", SqlDbType.DateTime).Value = c.FechaHasta;

                    var pFondo = cmd.Parameters.Add("@FondoInicial", SqlDbType.Decimal);
                    pFondo.Precision = 18; pFondo.Scale = 2; pFondo.Value = c.FondoInicial;

                    var pTotV = cmd.Parameters.Add("@TotalVentas", SqlDbType.Decimal);
                    pTotV.Precision = 18; pTotV.Scale = 2; pTotV.Value = c.TotalVentas;

                    var pTotP = cmd.Parameters.Add("@TotalPagos", SqlDbType.Decimal);
                    pTotP.Precision = 18; pTotP.Scale = 2; pTotP.Value = c.TotalPagos;

                    var pEfT = cmd.Parameters.Add("@EfectivoTeorico", SqlDbType.Decimal);
                    pEfT.Precision = 18; pEfT.Scale = 2; pEfT.Value = c.EfectivoTeorico;

                    var pEfD = cmd.Parameters.Add("@EfectivoDeclarado", SqlDbType.Decimal);
                    pEfD.Precision = 18; pEfD.Scale = 2; pEfD.Value = c.EfectivoDeclarado;

                    var pDif = cmd.Parameters.Add("@Diferencia", SqlDbType.Decimal);
                    pDif.Precision = 18; pDif.Scale = 2; pDif.Value = c.Diferencia;

                    cmd.Parameters.Add("@UsuarioCierre", SqlDbType.VarChar, 50).Value = c.UsuarioCierre ?? "";
                    cmd.Parameters.Add("@FechaCierre", SqlDbType.DateTime).Value = c.FechaCierre;
                    cmd.Parameters.Add("@Estado", SqlDbType.VarChar, 12).Value = c.Estado ?? "CERRADO";

                    var obj = cmd.ExecuteScalar();
                    cierreId = (obj == null || obj == DBNull.Value) ? 0L : Convert.ToInt64(obj);
                }

                // === VENTAS: marcar en cierre (por caja específica) ===
                using (var cmdUpd = new SqlCommand(@"
                    UPDATE v
                    SET  v.CierreId         = @CierreId,
                         v.IncluidaEnCierre = 1
                    FROM dbo.VentaCab v
                    WHERE v.POS_CajaNumero   = @CajaNumero
                      AND v.CajaId           = @CajaId
                      AND v.Fecha            >= @Desde
                      AND v.Fecha            <= @Hasta
                      AND v.Estado           = 'FACTURADA'
                      AND (v.IncluidaEnCierre = 0 OR v.IncluidaEnCierre IS NULL)
                      AND (v.CierreId IS NULL OR v.CierreId = 0);", cn, tx))
                {
                    cmdUpd.Parameters.Add("@CierreId", SqlDbType.BigInt).Value = cierreId;
                    cmdUpd.Parameters.Add("@CajaNumero", SqlDbType.NVarChar, 10).Value = c.POS_CajaNumero!.Trim();
                    cmdUpd.Parameters.Add("@CajaId", SqlDbType.Int).Value = c.CajaId;
                    cmdUpd.Parameters.Add("@Desde", SqlDbType.DateTime).Value = c.FechaDesde;
                    cmdUpd.Parameters.Add("@Hasta", SqlDbType.DateTime).Value = c.FechaHasta;
                    cmdUpd.ExecuteNonQuery();
                }

                // === PAGOS: marcar en cierre (por caja específica) ===
                using (var cmdUpdPago = new SqlCommand(@"
                    UPDATE p
                    SET
                        p.CierreId         = @CierreId,
                        p.IncluidoEnCierre = 1,
                        p.POS_CajaNumero   = CASE 
                                                WHEN ISNULL(p.POS_CajaNumero,'') = '' 
                                                THEN @CajaNumero 
                                                ELSE p.POS_CajaNumero 
                                             END
                    FROM dbo.POS_Pago p
                    WHERE (p.POS_CajaNumero = @CajaNumero OR ISNULL(p.POS_CajaNumero,'') = '')
                      AND p.CajaId = @CajaId
                      AND p.Fecha  >= @Desde
                      AND p.Fecha  <= @Hasta
                      AND p.Estado = 'APLICADO'
                      AND (p.CierreId IS NULL OR p.CierreId = 0);", cn, tx))
                {
                    cmdUpdPago.Parameters.Add("@CierreId", SqlDbType.BigInt).Value = cierreId;
                    cmdUpdPago.Parameters.Add("@CajaNumero", SqlDbType.NVarChar, 10).Value = c.POS_CajaNumero!.Trim();
                    cmdUpdPago.Parameters.Add("@CajaId", SqlDbType.Int).Value = c.CajaId;
                    cmdUpdPago.Parameters.Add("@Desde", SqlDbType.DateTime).Value = c.FechaDesde;
                    cmdUpdPago.Parameters.Add("@Hasta", SqlDbType.DateTime).Value = c.FechaHasta;
                    cmdUpdPago.ExecuteNonQuery();
                }

                tx.Commit();
                return cierreId;
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }

        // =====================================
        //  LISTAR CIERRES (HISTÓRICO)
        // =====================================
        public List<CierreCaja> ListarCierres(
            DateTime? desde,
            DateTime? hasta,
            string? cajaNumero,
            string? usuarioCierre,
            string? estado)
        {
            var lista = new List<CierreCaja>();
            using var cn = Db.GetOpenConnection();

            var sb = new StringBuilder(@"
                SELECT
                    CierreId,
                    CajaId,
                    CajaNumero,
                    FechaDesde,
                    FechaHasta,
                    FondoInicial,
                    TotalVentas,
                    TotalPagos,
                    EfectivoTeorico,
                    EfectivoDeclarado,
                    Diferencia,
                    UsuarioCierre,
                    FechaCierre,
                    Estado
                FROM dbo.CajaCierreCab
                WHERE 1 = 1");

            using var cmd = new SqlCommand();
            cmd.Connection = cn;

            if (desde.HasValue)
            {
                sb.AppendLine("  AND FechaCierre >= @Desde");
                cmd.Parameters.Add("@Desde", SqlDbType.DateTime).Value = desde.Value;
            }

            if (hasta.HasValue)
            {
                sb.AppendLine("  AND FechaCierre <= @Hasta");
                cmd.Parameters.Add("@Hasta", SqlDbType.DateTime).Value = hasta.Value;
            }

            if (!string.IsNullOrWhiteSpace(cajaNumero))
            {
                sb.AppendLine("  AND CajaNumero = @CajaNumero");
                cmd.Parameters.Add("@CajaNumero", SqlDbType.NVarChar, 10).Value = cajaNumero.Trim();
            }

            if (!string.IsNullOrWhiteSpace(usuarioCierre))
            {
                sb.AppendLine("  AND UsuarioCierre LIKE @UsuarioCierre");
                cmd.Parameters.Add("@UsuarioCierre", SqlDbType.VarChar, 50)
                    .Value = "%" + usuarioCierre.Trim() + "%";
            }

            if (!string.IsNullOrWhiteSpace(estado) &&
                !estado.Equals("TODOS", StringComparison.OrdinalIgnoreCase))
            {
                sb.AppendLine("  AND Estado = @Estado");
                cmd.Parameters.Add("@Estado", SqlDbType.VarChar, 12).Value = estado.Trim();
            }

            sb.AppendLine("ORDER BY FechaCierre DESC;");
            cmd.CommandText = sb.ToString();

            using var dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                var c = new CierreCaja
                {
                    CierreId = dr.GetInt64(dr.GetOrdinal("CierreId")),
                    CajaId = dr.GetInt32(dr.GetOrdinal("CajaId")),
                    POS_CajaNumero = dr.GetString(dr.GetOrdinal("CajaNumero")),
                    FechaDesde = dr.GetDateTime(dr.GetOrdinal("FechaDesde")),
                    FechaHasta = dr.GetDateTime(dr.GetOrdinal("FechaHasta")),
                    FondoInicial = dr.GetDecimal(dr.GetOrdinal("FondoInicial")),
                    TotalVentas = dr.GetDecimal(dr.GetOrdinal("TotalVentas")),
                    TotalPagos = dr.GetDecimal(dr.GetOrdinal("TotalPagos")),
                    EfectivoTeorico = dr.GetDecimal(dr.GetOrdinal("EfectivoTeorico")),
                    EfectivoDeclarado = dr.GetDecimal(dr.GetOrdinal("EfectivoDeclarado")),
                    Diferencia = dr.GetDecimal(dr.GetOrdinal("Diferencia")),
                    UsuarioCierre = dr.GetString(dr.GetOrdinal("UsuarioCierre")),
                    FechaCierre = dr.GetDateTime(dr.GetOrdinal("FechaCierre")),
                    Estado = dr.GetString(dr.GetOrdinal("Estado"))
                };
                lista.Add(c);
            }

            return lista;
        }

        public DataTable ListarPagosPorCajaYRango(string cajaNumero, DateTime fechaDesde, DateTime fechaHasta)
        {
            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
                SELECT
                    p.Fecha,                 -- fecha del pago
                    p.POS_CajaNumero,        -- número de caja
                    mp.Nombre      AS Medio, -- nombre del medio de pago
                    p.MonedaCodigo,
                    p.Monto,
                    p.MontoBase,
                    p.TasaCambio,
                    p.Referencia,
                    p.Entidad,
                    p.Observacion,
                    p.Usuario
                FROM dbo.POS_Pago p
                LEFT JOIN dbo.MedioPago mp ON mp.MedioPagoId = p.MedioPagoId
                WHERE p.POS_CajaNumero = @CajaNumero
                  AND p.Fecha >= @Desde
                  AND p.Fecha <= @Hasta
                ORDER BY p.Fecha;", cn);

            cmd.Parameters.Add("@CajaNumero", SqlDbType.VarChar, 20).Value = cajaNumero;
            cmd.Parameters.Add("@Desde", SqlDbType.DateTime).Value = fechaDesde;
            cmd.Parameters.Add("@Hasta", SqlDbType.DateTime).Value = fechaHasta;

            using var da = new SqlDataAdapter(cmd);
            var dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        public DataTable ListarFondoPorCajaYRango(string cajaNumero, DateTime fechaDesde, DateTime fechaHasta)
        {
            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
                SELECT
                    f.FondoId,
                    f.POS_CajaNumero,
                    f.FechaApertura,
                    f.UsuarioApertura,
                    f.MontoFondo,
                    f.Observacion,
                    f.Estado,
                    f.FechaCierre
                FROM dbo.POS_FondoCaja f
                WHERE f.POS_CajaNumero = @CajaNumero
                  AND f.FechaApertura >= @Desde
                  AND f.FechaApertura <= @Hasta
                ORDER BY f.FechaApertura;", cn);

            cmd.Parameters.Add("@CajaNumero", SqlDbType.VarChar, 20).Value = cajaNumero;
            cmd.Parameters.Add("@Desde", SqlDbType.DateTime).Value = fechaDesde;
            cmd.Parameters.Add("@Hasta", SqlDbType.DateTime).Value = fechaHasta;

            using var da = new SqlDataAdapter(cmd);
            var dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        // Cierre pagos datagrid view cierre detalles
        public List<CierrePagoPosDetalleDto> ListarPagosPosPorCierre(long cierreId)
        {
            var lista = new List<CierrePagoPosDetalleDto>();
            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
                SELECT
                    p.PagoId,
                    p.Fecha,
                    p.POS_CajaNumero,
                    p.MonedaCodigo,
                    p.TasaCambio,
                    p.Monto,
                    p.MedioPagoId,
                    mp.Nombre AS MedioPagoNombre,
                    p.Referencia,
                    p.Entidad,
                    p.Observacion,
                    p.Usuario
                FROM dbo.POS_Pago p
                LEFT JOIN dbo.MedioPago mp ON p.MedioPagoId = mp.MedioPagoId
                WHERE p.CierreId = @CierreId
                  AND p.Estado = 'APLICADO'
                ORDER BY p.Fecha;", cn);

            cmd.Parameters.Add("@CierreId", SqlDbType.BigInt).Value = cierreId;

            using var dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                var it = new CierrePagoPosDetalleDto
                {
                    PagoId = dr.GetInt64(dr.GetOrdinal("PagoId")),
                    Fecha = dr.GetDateTime(dr.GetOrdinal("Fecha")),
                    POS_CajaNumero = dr["POS_CajaNumero"] as string,
                    MonedaCodigo = dr["MonedaCodigo"] as string,
                    TasaCambio = dr.GetDecimal(dr.GetOrdinal("TasaCambio")),
                    Monto = dr.GetDecimal(dr.GetOrdinal("Monto")),
                    MedioPagoId = dr.GetInt32(dr.GetOrdinal("MedioPagoId")),
                    MedioPagoNombre = dr["MedioPagoNombre"] as string,
                    Referencia = dr["Referencia"] as string,
                    Entidad = dr["Entidad"] as string,
                    Observacion = dr["Observacion"] as string,
                    Usuario = dr["Usuario"] as string
                };
                lista.Add(it);
            }

            return lista;
        }

        // actualizar pagos pos con cierre id (usado desde el service con cierreId)
        public void ActualizarPagosPosConCierreId(long cierreId, string cajaNumero, DateTime fechaDesde, DateTime fechaHasta)
        {
            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
                UPDATE p
                SET 
                    p.CierreId         = @CierreId,
                    p.IncluidoEnCierre = 1,
                    p.POS_CajaNumero   = CASE 
                                            WHEN ISNULL(p.POS_CajaNumero,'') = '' 
                                            THEN @CajaNumero 
                                            ELSE p.POS_CajaNumero 
                                         END
                FROM dbo.POS_Pago p
                WHERE (p.POS_CajaNumero = @CajaNumero OR ISNULL(p.POS_CajaNumero,'') = '')
                  AND p.CajaId = (
                        SELECT TOP 1 CajaId 
                        FROM dbo.CajaCierreCab 
                        WHERE CierreId = @CierreId
                    )
                  AND p.Fecha  >= @Desde
                  AND p.Fecha  <= @Hasta
                  AND p.Estado = 'APLICADO'
                  AND (p.CierreId IS NULL OR p.CierreId = 0);", cn);

            cmd.Parameters.Add("@CierreId", SqlDbType.BigInt).Value = cierreId;
            cmd.Parameters.Add("@CajaNumero", SqlDbType.NVarChar, 10).Value = cajaNumero.Trim();
            cmd.Parameters.Add("@Desde", SqlDbType.DateTime).Value = fechaDesde;
            cmd.Parameters.Add("@Hasta", SqlDbType.DateTime).Value = fechaHasta;

            cmd.ExecuteNonQuery();
        }

        // nuevo método para insertar cierre desde DTO
        public long InsertarCierre(CierreCajaDto dto)
        {
            var entidad = new CierreCaja
            {
                CajaId = dto.CajaId,
                POS_CajaNumero = dto.CajaNumero,
                FechaDesde = dto.FechaDesde,
                FechaHasta = dto.FechaHasta,
                FondoInicial = dto.FondoInicial,
                UsuarioCierre = dto.UsuarioCierre,
                FechaCierre = DateTime.Now,
                Estado = "CERRADO"
            };

            return InsertarCierre(entidad); // reutiliza el método existente
        }

        public void ActualizarVentasConCierreId(long cierreId, string cajaNumero, DateTime desde, DateTime hasta)
        {
            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
                UPDATE v
                SET 
                    v.CierreId         = @CierreId,
                    v.IncluidaEnCierre = 1
                FROM dbo.VentaCab v
                WHERE v.POS_CajaNumero = @CajaNumero
                  AND v.CajaId = (
                        SELECT TOP 1 CajaId 
                        FROM dbo.CajaCierreCab 
                        WHERE CierreId = @CierreId
                    )
                  AND v.Fecha >= @Desde
                  AND v.Fecha <= @Hasta
                  AND (v.IncluidaEnCierre = 0 OR v.IncluidaEnCierre IS NULL)
                  AND (v.CierreId IS NULL OR v.CierreId = 0);", cn);

            cmd.Parameters.Add("@CierreId", SqlDbType.BigInt).Value = cierreId;
            cmd.Parameters.Add("@CajaNumero", SqlDbType.NVarChar, 10).Value = cajaNumero;
            cmd.Parameters.Add("@Desde", SqlDbType.DateTime).Value = desde;
            cmd.Parameters.Add("@Hasta", SqlDbType.DateTime).Value = hasta;

            cmd.ExecuteNonQuery();
        }
    }
}
