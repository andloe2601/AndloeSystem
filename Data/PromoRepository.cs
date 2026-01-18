using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;

namespace Andloe.Data
{
    public class PromoRepository
    {
        // =========================================
        //   CREAR PROMO DESCUENTO / PACK / PRECIO
        // =========================================
        public int CrearPromoDescuentoProducto(
            string codigoPromo,              // se ignora, SQL genera el real
            string nombre,
            string productoCodigo,
            string tipoRegla,
            decimal? descuentoPct,
            decimal? precioFijoPromo,
            decimal? packCant,
            decimal? packPrecioTotal,
            DateTime fechaDesde,
            DateTime fechaHasta,
            bool activa,
            string usuario,
            bool lunes,
            bool martes,
            bool miercoles,
            bool jueves,
            bool viernes,
            bool sabado,
            bool domingo,
            decimal? cantidadMinima
        )
        {
            using var cn = Db.GetOpenConnection();
            using var tx = cn.BeginTransaction();

            try
            {
                // 1) SQL GENERA EL CÓDIGO REAL
                string codigoReal = GenerarCodigoPromo(cn, tx);

                // 2) CABECERA
                int promoId;
                using (var cmdCab = new SqlCommand(@"
INSERT INTO dbo.PromoCab
    (Codigo, Nombre, TipoPromo, Estado, Prioridad, Acumulable, RequiereCupon, Usuario, FechaCreacion)
VALUES
    (@Codigo, @Nombre, @TipoPromo, @Estado, @Prioridad, @Acumulable, @RequiereCupon, @Usuario, GETDATE());
SELECT CAST(SCOPE_IDENTITY() AS int);", cn, tx))
                {
                    cmdCab.Parameters.Add("@Codigo", SqlDbType.VarChar, 20).Value = codigoReal;
                    cmdCab.Parameters.Add("@Nombre", SqlDbType.VarChar, 100).Value = nombre ?? string.Empty;
                    cmdCab.Parameters.Add("@TipoPromo", SqlDbType.VarChar, 20).Value = "DESCUENTO";
                    cmdCab.Parameters.Add("@Estado", SqlDbType.VarChar, 12).Value = activa ? "ACTIVA" : "PAUSADA";
                    cmdCab.Parameters.Add("@Prioridad", SqlDbType.Int).Value = 100;
                    cmdCab.Parameters.Add("@Acumulable", SqlDbType.Bit).Value = 0;
                    cmdCab.Parameters.Add("@RequiereCupon", SqlDbType.Bit).Value = 0;
                    cmdCab.Parameters.Add("@Usuario", SqlDbType.VarChar, 50).Value = usuario ?? string.Empty;

                    promoId = (int)cmdCab.ExecuteScalar();
                }

                // 3) CALENDARIO
                using (var cmdCal = new SqlCommand(@"
INSERT INTO dbo.PromoCalendario
    (PromoId, FechaInicio, FechaFin,
     Lunes, Martes, Miercoles, Jueves, Viernes, Sabado, Domingo,
     HoraInicio, HoraFin)
VALUES
    (@PromoId, @FechaInicio, @FechaFin,
     @Lunes, @Martes, @Miercoles, @Jueves, @Viernes, @Sabado, @Domingo,
     NULL, NULL);", cn, tx))
                {
                    cmdCal.Parameters.Add("@PromoId", SqlDbType.Int).Value = promoId;
                    cmdCal.Parameters.Add("@FechaInicio", SqlDbType.DateTime).Value = fechaDesde.Date;
                    cmdCal.Parameters.Add("@FechaFin", SqlDbType.DateTime).Value = fechaHasta.Date;

                    cmdCal.Parameters.Add("@Lunes", SqlDbType.Bit).Value = lunes;
                    cmdCal.Parameters.Add("@Martes", SqlDbType.Bit).Value = martes;
                    cmdCal.Parameters.Add("@Miercoles", SqlDbType.Bit).Value = miercoles;
                    cmdCal.Parameters.Add("@Jueves", SqlDbType.Bit).Value = jueves;
                    cmdCal.Parameters.Add("@Viernes", SqlDbType.Bit).Value = viernes;
                    cmdCal.Parameters.Add("@Sabado", SqlDbType.Bit).Value = sabado;
                    cmdCal.Parameters.Add("@Domingo", SqlDbType.Bit).Value = domingo;

                    cmdCal.ExecuteNonQuery();
                }

                // 4) ALCANCE (producto inicial)
                using (var cmdAlc = new SqlCommand(@"
INSERT INTO dbo.PromoAlcance
    (PromoId, TipoObjetivo, NoProducto, CategoriaId, SubcategoriaId, ClienteCodigo)
VALUES
    (@PromoId, @TipoObjetivo, @NoProducto, NULL, NULL, NULL);", cn, tx))
                {
                    cmdAlc.Parameters.Add("@PromoId", SqlDbType.Int).Value = promoId;
                    cmdAlc.Parameters.Add("@TipoObjetivo", SqlDbType.VarChar, 20).Value = "PRODUCTO";
                    cmdAlc.Parameters.Add("@NoProducto", SqlDbType.VarChar, 20).Value = productoCodigo ?? string.Empty;
                    cmdAlc.ExecuteNonQuery();
                }

                // 5) REGLA
                using (var cmdReg = new SqlCommand(@"
INSERT INTO dbo.PromoRegla
    (PromoId, TipoRegla,
     MinCantidad, MaxCantidad,
     DescuentoPct, DescuentoMonto, PrecioFijo,
     Pack_BuyProducto, Pack_BuyCant,
     Pack_GetProducto, Pack_GetCant,
     Pack_Precio)
VALUES
    (@PromoId, @TipoRegla,
     @MinCantidad, @MaxCantidad,
     @DescuentoPct, @DescuentoMonto, @PrecioFijo,
     @Pack_BuyProducto, @Pack_BuyCant,
     NULL, NULL,
     @Pack_Precio);", cn, tx))
                {
                    cmdReg.Parameters.Add("@PromoId", SqlDbType.Int).Value = promoId;
                    cmdReg.Parameters.Add("@TipoRegla", SqlDbType.VarChar, 20).Value = tipoRegla ?? string.Empty;

                    var pMin = cmdReg.Parameters.Add("@MinCantidad", SqlDbType.Decimal);
                    pMin.Precision = 18;
                    pMin.Scale = 3;
                    pMin.Value = (object?)cantidadMinima ?? 1m;

                    var pMax = cmdReg.Parameters.Add("@MaxCantidad", SqlDbType.Decimal);
                    pMax.Precision = 18;
                    pMax.Scale = 3;
                    pMax.Value = DBNull.Value;

                    var pDesc = cmdReg.Parameters.Add("@DescuentoPct", SqlDbType.Decimal);
                    pDesc.Precision = 9;
                    pDesc.Scale = 4;
                    pDesc.Value = (object?)descuentoPct ?? DBNull.Value;

                    var pDescMonto = cmdReg.Parameters.Add("@DescuentoMonto", SqlDbType.Decimal);
                    pDescMonto.Precision = 18;
                    pDescMonto.Scale = 2;
                    pDescMonto.Value = DBNull.Value;

                    var pPrecioFijo = cmdReg.Parameters.Add("@PrecioFijo", SqlDbType.Decimal);
                    pPrecioFijo.Precision = 18;
                    pPrecioFijo.Scale = 2;
                    pPrecioFijo.Value = (object?)precioFijoPromo ?? DBNull.Value;

                    cmdReg.Parameters.Add("@Pack_BuyProducto", SqlDbType.VarChar, 20).Value =
                        (tipoRegla == "PACK_PRODUCTO")
                            ? (object)(productoCodigo ?? string.Empty)
                            : DBNull.Value;

                    var pPackCant = cmdReg.Parameters.Add("@Pack_BuyCant", SqlDbType.Decimal);
                    pPackCant.Precision = 18;
                    pPackCant.Scale = 3;
                    pPackCant.Value = (tipoRegla == "PACK_PRODUCTO")
                        ? (object?)packCant ?? DBNull.Value
                        : DBNull.Value;

                    var pPackPrecio = cmdReg.Parameters.Add("@Pack_Precio", SqlDbType.Decimal);
                    pPackPrecio.Precision = 18;
                    pPackPrecio.Scale = 2;
                    pPackPrecio.Value = (object?)packPrecioTotal ?? DBNull.Value;

                    cmdReg.ExecuteNonQuery();
                }

                tx.Commit();
                return promoId;
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }

        // =========================================
        //   CREAR PROMO PARA GRUPO DE PRODUCTOS
        // =========================================
        public int CrearPromoDescuentoProductoMultiple(
            string codigoPromo,
            IEnumerable<string> codigosProductos,
            string tipoRegla,
            decimal? descuentoPct,
            decimal? precioFijoPromo,
            decimal? packCant,
            decimal? packPrecioTotal,
            decimal? cantidadMinimaGrupo,
            DateTime fechaDesde,
            DateTime fechaHasta,
            bool activa,
            string usuario,
            bool lunes,
            bool martes,
            bool miercoles,
            bool jueves,
            bool viernes,
            bool sabado,
            bool domingo)
        {
            var lista = new List<string>(codigosProductos ?? Array.Empty<string>());
            if (lista.Count == 0)
                throw new InvalidOperationException("No se recibieron productos para la promoción.");

            var primerProducto = lista[0];

            int promoId = CrearPromoDescuentoProducto(
                codigoPromo,
                nombre: "",
                productoCodigo: primerProducto,
                tipoRegla: tipoRegla,
                descuentoPct: descuentoPct,
                precioFijoPromo: precioFijoPromo,
                packCant: packCant,
                packPrecioTotal: packPrecioTotal,
                fechaDesde: fechaDesde,
                fechaHasta: fechaHasta,
                activa: activa,
                usuario: usuario,
                lunes: lunes,
                martes: martes,
                miercoles: miercoles,
                jueves: jueves,
                viernes: viernes,
                sabado: sabado,
                domingo: domingo,
                cantidadMinima: cantidadMinimaGrupo
            );

            ReemplazarProductosPromo(promoId, lista);

            return promoId;
        }

        // ================== LOG/TOPE ==================

        // ✅ Mantengo tu firma EXACTA para no romper nada
        public void RegistrarUsoPromo(
            int promoId,
            long ventaId,
            string productoCodigo,
            decimal cantidad,
            decimal montoDescuento,
            string usuario)
        {
            using var cn = Db.GetOpenConnection();
            using var tx = cn.BeginTransaction();

            try
            {
                var now = DateTime.Now;
                var (tipo, clave) = PeriodoDiario(now);

                RegistrarUsoPromoTx(
                    promoId: promoId,
                    origen: "VENTA",
                    origenId: ventaId,
                    linea: null,
                    montoDesc: montoDescuento,
                    monedaCodigo: null,
                    tasaCambio: null,
                    usuario: usuario,
                    fechaAplic: now,
                    periodoTipo: tipo,
                    periodoClave: clave,
                    clienteCodigo: null,
                    productoCodigo: string.IsNullOrWhiteSpace(productoCodigo) ? null : productoCodigo.Trim(),
                    cn: cn,
                    tx: tx
                );

                tx.Commit();
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }

        /// <summary>
        /// ✅ Método PRO: usar dentro del mismo SqlTransaction del cierre.
        /// Inserta PromoLog + acumula PromoTope en una sola operación atómica.
        /// </summary>
        public void RegistrarUsoPromoTx(
            int promoId,
            string origen,
            long origenId,
            int? linea,
            decimal montoDesc,
            string? monedaCodigo,
            decimal? tasaCambio,
            string? usuario,
            DateTime fechaAplic,
            string periodoTipo,
            string periodoClave,
            string? clienteCodigo,
            string? productoCodigo,
            SqlConnection cn,
            SqlTransaction tx)
        {
            if (promoId <= 0) return;

            montoDesc = Math.Round(montoDesc, 2);
            if (montoDesc <= 0) return;

            var prod = string.IsNullOrWhiteSpace(productoCodigo) ? null : productoCodigo.Trim();
            var cli = string.IsNullOrWhiteSpace(clienteCodigo) ? null : clienteCodigo.Trim();

            InsertPromoLogTx(
                promoId: promoId,
                origen: origen,
                origenId: origenId,
                linea: linea,
                montoDesc: montoDesc,
                monedaCodigo: monedaCodigo,
                tasaCambio: tasaCambio,
                usuario: usuario,
                fechaAplic: fechaAplic,
                cn: cn,
                tx: tx
            );

            // ✅ En tu flujo actual: acumular por (PromoId, ProductoCodigo)
            // Cliente se deja opcional (si viene, entra en la llave; si no, queda null)
            UpsertPromoTopeTx(
                promoId: promoId,
                periodoTipo: periodoTipo,
                periodoClave: periodoClave,
                clienteCodigo: cli,
                productoCodigo: prod,
                usosDelta: 1,
                montoDelta: montoDesc,
                ultimaFecha: fechaAplic,
                cn: cn,
                tx: tx
            );
        }

        /// <summary>
        /// ✅ NUEVO: para cuando YA tú acumulaste en memoria y quieres registrar 1 vez por (PromoId, ProductoCodigo).
        /// Úsalo desde PosService después de acumular.
        /// </summary>
        public void RegistrarUsoPromoAcumuladoTx(
            int promoId,
            long ventaId,
            int? linea,
            string? clienteCodigo,
            string productoCodigo,
            decimal montoTotal,
            string? monedaCodigo,
            decimal? tasaCambio,
            string usuario,
            DateTime fechaAplic,
            SqlConnection cn,
            SqlTransaction tx)
        {
            var (tipo, clave) = PeriodoDiario(fechaAplic);

            RegistrarUsoPromoTx(
                promoId: promoId,
                origen: "VENTA",
                origenId: ventaId,
                linea: linea,
                montoDesc: montoTotal,
                monedaCodigo: monedaCodigo,
                tasaCambio: tasaCambio,
                usuario: usuario,
                fechaAplic: fechaAplic,
                periodoTipo: tipo,
                periodoClave: clave,
                clienteCodigo: clienteCodigo,
                productoCodigo: productoCodigo,
                cn: cn,
                tx: tx
            );
        }

        private long InsertPromoLogTx(
            int promoId,
            string origen,
            long origenId,
            int? linea,
            decimal montoDesc,
            string? monedaCodigo,
            decimal? tasaCambio,
            string? usuario,
            DateTime fechaAplic,
            SqlConnection cn,
            SqlTransaction tx)
        {
            const string sql = @"
INSERT INTO dbo.PromoLog
(
    PromoId, Origen, OrigenId, Linea,
    MontoDesc, MonedaCodigo, TasaCambio,
    Usuario, FechaAplic, Fecha
)
VALUES
(
    @PromoId, @Origen, @OrigenId, @Linea,
    @MontoDesc, @MonedaCodigo, @TasaCambio,
    @Usuario, @FechaAplic, SYSDATETIME()
);
SELECT CAST(SCOPE_IDENTITY() AS bigint);";

            using var cmd = new SqlCommand(sql, cn, tx);

            cmd.Parameters.Add("@PromoId", SqlDbType.Int).Value = promoId;
            cmd.Parameters.Add("@Origen", SqlDbType.VarChar, 20).Value = origen ?? "VENTA";
            cmd.Parameters.Add("@OrigenId", SqlDbType.BigInt).Value = origenId;
            cmd.Parameters.Add("@Linea", SqlDbType.Int).Value = (object?)linea ?? DBNull.Value;

            var pMonto = cmd.Parameters.Add("@MontoDesc", SqlDbType.Decimal);
            pMonto.Precision = 18; pMonto.Scale = 2; pMonto.Value = montoDesc;

            cmd.Parameters.Add("@MonedaCodigo", SqlDbType.VarChar, 3)
                .Value = string.IsNullOrWhiteSpace(monedaCodigo) ? (object)DBNull.Value : monedaCodigo!.Trim();

            var pTc = cmd.Parameters.Add("@TasaCambio", SqlDbType.Decimal);
            pTc.Precision = 18; pTc.Scale = 6;
            pTc.Value = tasaCambio.HasValue ? tasaCambio.Value : (object)DBNull.Value;

            cmd.Parameters.Add("@Usuario", SqlDbType.VarChar, 50)
                .Value = string.IsNullOrWhiteSpace(usuario) ? (object)DBNull.Value : usuario!.Trim();

            cmd.Parameters.Add("@FechaAplic", SqlDbType.DateTime).Value = fechaAplic;

            return (long)cmd.ExecuteScalar();
        }

        private void UpsertPromoTopeTx(
            int promoId,
            string periodoTipo,
            string periodoClave,
            string? clienteCodigo,
            string? productoCodigo,
            int usosDelta,
            decimal montoDelta,
            DateTime ultimaFecha,
            SqlConnection cn,
            SqlTransaction tx)
        {
            // ✅ "amarrado": por defecto el match cae por PromoId + Periodo + Producto (y Cliente si viene)
            const string sql = @"
MERGE dbo.PromoTope AS t
USING (SELECT
        @PromoId AS PromoId,
        @PeriodoTipo AS PeriodoTipo,
        @PeriodoClave AS PeriodoClave,
        @ClienteCodigo AS ClienteCodigo,
        @ProductoCodigo AS ProductoCodigo
) AS s
ON  t.PromoId = s.PromoId
AND t.PeriodoTipo = s.PeriodoTipo
AND t.PeriodoClave = s.PeriodoClave
AND ISNULL(t.ProductoCodigo,'') = ISNULL(s.ProductoCodigo,'')
AND ISNULL(t.ClienteCodigo,'') = ISNULL(s.ClienteCodigo,'')

WHEN MATCHED THEN
    UPDATE SET
        t.Usos = t.Usos + @UsosDelta,
        t.MontoAcum = t.MontoAcum + @MontoDelta,
        t.UltimaFecha = @UltimaFecha

WHEN NOT MATCHED THEN
    INSERT (PromoId, PeriodoTipo, PeriodoClave, ClienteCodigo, ProductoCodigo, Usos, MontoAcum, UltimaFecha)
    VALUES (s.PromoId, s.PeriodoTipo, s.PeriodoClave, s.ClienteCodigo, s.ProductoCodigo, @UsosDelta, @MontoDelta, @UltimaFecha);";

            using var cmd = new SqlCommand(sql, cn, tx);

            cmd.Parameters.Add("@PromoId", SqlDbType.Int).Value = promoId;
            cmd.Parameters.Add("@PeriodoTipo", SqlDbType.VarChar, 10).Value = periodoTipo;
            cmd.Parameters.Add("@PeriodoClave", SqlDbType.VarChar, 10).Value = periodoClave;

            cmd.Parameters.Add("@ClienteCodigo", SqlDbType.VarChar, 30)
                .Value = string.IsNullOrWhiteSpace(clienteCodigo) ? (object)DBNull.Value : clienteCodigo!.Trim();

            cmd.Parameters.Add("@ProductoCodigo", SqlDbType.VarChar, 30)
                .Value = string.IsNullOrWhiteSpace(productoCodigo) ? (object)DBNull.Value : productoCodigo!.Trim();

            cmd.Parameters.Add("@UsosDelta", SqlDbType.Int).Value = usosDelta;

            var pMonto = cmd.Parameters.Add("@MontoDelta", SqlDbType.Decimal);
            pMonto.Precision = 18; pMonto.Scale = 2; pMonto.Value = montoDelta;

            cmd.Parameters.Add("@UltimaFecha", SqlDbType.DateTime2).Value = ultimaFecha;

            cmd.ExecuteNonQuery();
        }

        // Helpers de periodo (para tope)
        public static (string tipo, string clave) PeriodoDiario(DateTime f) => ("DIARIO", f.ToString("yyyyMMdd"));
        public static (string tipo, string clave) PeriodoMensual(DateTime f) => ("MENSUAL", f.ToString("yyyyMM"));
        public static (string tipo, string clave) PeriodoGeneral() => ("GENERAL", "GENERAL");

        // =========================================
        //   HELPER PRIVADO: NUMERADOR PROMOPROD
        // =========================================
        private string GenerarCodigoPromo(SqlConnection cn, SqlTransaction tx)
        {
            using var cmdSel = new SqlCommand(@"
SELECT Prefijo, Longitud, Actual
FROM dbo.NumeradorSecuencia WITH (UPDLOCK, ROWLOCK)
WHERE Codigo = @Codigo;", cn, tx);

            cmdSel.Parameters.Add("@Codigo", SqlDbType.VarChar, 20).Value = "PROMOPROD";

            string prefijo;
            int longitud;
            int actual;

            using (var rd = cmdSel.ExecuteReader())
            {
                if (!rd.Read())
                    throw new InvalidOperationException("No se encontró numerador 'PROMOPROD' en NumeradorSecuencia.");

                prefijo = rd.IsDBNull(0) ? "" : rd.GetString(0);
                longitud = rd.GetInt32(1);
                actual = rd.GetInt32(2);
            }

            int nuevoValor = actual + 1;

            using (var cmdUpd = new SqlCommand(@"
UPDATE dbo.NumeradorSecuencia
SET Actual = @Nuevo
WHERE Codigo = @Codigo;", cn, tx))
            {
                cmdUpd.Parameters.Add("@Nuevo", SqlDbType.Int).Value = nuevoValor;
                cmdUpd.Parameters.Add("@Codigo", SqlDbType.VarChar, 20).Value = "PROMOPROD";
                cmdUpd.ExecuteNonQuery();
            }

            string numeroStr = nuevoValor.ToString().PadLeft(longitud, '0');
            return (prefijo ?? "") + numeroStr;
        }

        // =========================================
        //   SOLO PARA MOSTRAR (NO incrementa Actual)
        // =========================================
        public string PeekProximoCodigoPromo()
        {
            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
SELECT Prefijo, Longitud, Actual
FROM dbo.NumeradorSecuencia
WHERE Codigo = @Codigo;", cn);

            cmd.Parameters.Add("@Codigo", SqlDbType.VarChar, 20).Value = "PROMOPROD";

            string prefijo;
            int longitud;
            int actual;

            using (var rd = cmd.ExecuteReader())
            {
                if (!rd.Read())
                    throw new InvalidOperationException("No se encontró numerador 'PROMOPROD' en NumeradorSecuencia.");

                prefijo = rd.IsDBNull(0) ? "" : rd.GetString(0);
                longitud = rd.GetInt32(1);
                actual = rd.GetInt32(2);
            }

            int siguiente = actual + 1;
            string numeroStr = siguiente.ToString().PadLeft(longitud, '0');
            return (prefijo ?? "") + numeroStr;
        }

        public string ObtenerProximoCodigoPromo()
            => PeekProximoCodigoPromo();

        // =========================================
        //   HISTÓRICO (PARA FormPromoHistorico)
        // =========================================
        public List<PromoHistoricoRow> ListarHistoricoPromos(string texto, bool soloActivas)
        {
            var lista = new List<PromoHistoricoRow>();

            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
SELECT  c.PromoId,
        c.Codigo,
        c.Nombre,
        c.TipoPromo,
        c.Estado,
        cal.FechaInicio,
        cal.FechaFin,
        c.Usuario,
        c.FechaCreacion,
        ''  AS UsuarioMod,
        NULL AS FechaMod
FROM dbo.PromoCab c
LEFT JOIN dbo.PromoCalendario cal ON cal.PromoId = c.PromoId
WHERE (@Texto = '' 
        OR c.Codigo LIKE '%' + @Texto + '%' 
        OR c.Nombre LIKE '%' + @Texto + '%')
  AND (@SoloActivas = 0 OR c.Estado = 'ACTIVA')
ORDER BY c.FechaCreacion DESC;", cn);

            cmd.Parameters.Add("@Texto", SqlDbType.VarChar, 100).Value = texto ?? string.Empty;
            cmd.Parameters.Add("@SoloActivas", SqlDbType.Bit).Value = soloActivas;

            using var rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                var row = new PromoHistoricoRow
                {
                    PromoId = rd.GetInt32(0),
                    Codigo = rd.GetString(1),
                    Nombre = rd.GetString(2),
                    TipoPromo = rd.GetString(3),
                    Estado = rd.GetString(4),
                    FechaInicio = rd.IsDBNull(5) ? DateTime.MinValue : rd.GetDateTime(5),
                    FechaFin = rd.IsDBNull(6) ? DateTime.MinValue : rd.GetDateTime(6),
                    UsuarioCreacion = rd.IsDBNull(7) ? string.Empty : rd.GetString(7),
                    FechaCreacion = rd.IsDBNull(8) ? DateTime.MinValue : rd.GetDateTime(8),
                    UsuarioMod = rd.IsDBNull(9) ? string.Empty : rd.GetString(9),
                    FechaMod = rd.IsDBNull(10) ? (DateTime?)null : rd.GetDateTime(10)
                };

                lista.Add(row);
            }

            return lista;
        }

        public void CambiarEstadoPromo(int promoId, string nuevoEstado)
        {
            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
UPDATE dbo.PromoCab
SET Estado = @Estado
WHERE PromoId = @PromoId;", cn);

            cmd.Parameters.Add("@Estado", SqlDbType.VarChar, 12).Value = nuevoEstado;
            cmd.Parameters.Add("@PromoId", SqlDbType.Int).Value = promoId;

            cmd.ExecuteNonQuery();
        }

        public bool ExisteNombrePromo(string nombre, int? excluirPromoId = null)
        {
            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
SELECT COUNT(1)
FROM dbo.PromoCab
WHERE Nombre = @Nombre
  AND (@ExcluirId IS NULL OR PromoId <> @ExcluirId);", cn);

            cmd.Parameters.Add("@Nombre", SqlDbType.VarChar, 100).Value = nombre;

            var pExc = cmd.Parameters.Add("@ExcluirId", SqlDbType.Int);
            pExc.Value = excluirPromoId.HasValue ? excluirPromoId.Value : (object)DBNull.Value;

            var count = (int)cmd.ExecuteScalar();
            return count > 0;
        }

        public string? ObtenerCodigoPromoPorId(int promoId)
        {
            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
SELECT Codigo
FROM dbo.PromoCab
WHERE PromoId = @PromoId;", cn);

            cmd.Parameters.Add("@PromoId", SqlDbType.Int).Value = promoId;

            var obj = cmd.ExecuteScalar();
            if (obj == null || obj == DBNull.Value)
                return null;

            return Convert.ToString(obj);
        }

        // =========================================
        //   DETALLE DE PRODUCTOS DE UNA PROMO
        // =========================================
        public List<PromoDetalleRow> ListarDetallePromo(int promoId)
        {
            var lista = new List<PromoDetalleRow>();

            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
SELECT  
       a.NoProducto,
       ISNULL(p.Descripcion, '') AS NombreProducto,
       ISNULL(r.TipoRegla, '')   AS TipoRegla,
       ISNULL(r.DescuentoPct, 0) AS DescuentoPct,
       ISNULL(r.PrecioFijo, 0)   AS PrecioFijo,
       ISNULL(r.Pack_BuyCant, 0) AS PackCantidad,
       ISNULL(r.Pack_Precio, 0)  AS PackPrecio
FROM dbo.PromoAlcance a
LEFT JOIN dbo.PromoRegla  r ON r.PromoId = a.PromoId
LEFT JOIN dbo.Producto    p ON p.Nº = a.NoProducto
WHERE a.PromoId = @PromoId;", cn);

            cmd.Parameters.Add("@PromoId", SqlDbType.Int).Value = promoId;

            using var rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                var row = new PromoDetalleRow
                {
                    CodigoProducto = rd.IsDBNull(0) ? "" : rd.GetString(0),
                    NombreProducto = rd.IsDBNull(1) ? "" : rd.GetString(1),
                    TipoRegla = rd.IsDBNull(2) ? "" : rd.GetString(2),
                    DescuentoPct = rd.IsDBNull(3) ? 0m : rd.GetDecimal(3),
                    PrecioFijo = rd.IsDBNull(4) ? 0m : rd.GetDecimal(4),
                    PackCantidad = rd.IsDBNull(5) ? 0m : rd.GetDecimal(5),
                    PackPrecio = rd.IsDBNull(6) ? 0m : rd.GetDecimal(6)
                };

                lista.Add(row);
            }

            return lista;
        }

        // =========================================
        //   OBTENER PROMO PARA EDICIÓN
        // =========================================
        public PromoProductoDetalle? ObtenerPromoProductoPorId(int promoId)
        {
            using var cn = Db.GetOpenConnection();

            using (var cmd = new SqlCommand(@"
SELECT TOP(1)
       c.PromoId,
       c.Codigo,
       c.Nombre,
       c.Estado,
       cal.FechaInicio,
       cal.FechaFin,
       cal.Lunes,
       cal.Martes,
       cal.Miercoles,
       cal.Jueves,
       cal.Viernes,
       cal.Sabado,
       cal.Domingo,
       r.TipoRegla,
       r.DescuentoPct,
       r.PrecioFijo,
       r.Pack_BuyCant,
       r.Pack_Precio
FROM dbo.PromoCab c
LEFT JOIN dbo.PromoCalendario cal ON cal.PromoId = c.PromoId
LEFT JOIN dbo.PromoRegla r ON r.PromoId = c.PromoId
WHERE c.PromoId = @PromoId;", cn))
            {
                cmd.Parameters.Add("@PromoId", SqlDbType.Int).Value = promoId;

                using var rd = cmd.ExecuteReader();
                if (!rd.Read())
                    return null;

                var dto = new PromoProductoDetalle
                {
                    PromoId = rd.GetInt32(0),
                    Codigo = rd.GetString(1),
                    Nombre = rd.GetString(2),
                    Activa = string.Equals(rd.GetString(3), "ACTIVA", StringComparison.OrdinalIgnoreCase),
                    FechaInicio = rd.IsDBNull(4) ? DateTime.Today : rd.GetDateTime(4),
                    FechaFin = rd.IsDBNull(5) ? DateTime.Today : rd.GetDateTime(5),
                    Lunes = !rd.IsDBNull(6) && rd.GetBoolean(6),
                    Martes = !rd.IsDBNull(7) && rd.GetBoolean(7),
                    Miercoles = !rd.IsDBNull(8) && rd.GetBoolean(8),
                    Jueves = !rd.IsDBNull(9) && rd.GetBoolean(9),
                    Viernes = !rd.IsDBNull(10) && rd.GetBoolean(10),
                    Sabado = !rd.IsDBNull(11) && rd.GetBoolean(11),
                    Domingo = !rd.IsDBNull(12) && rd.GetBoolean(12),
                    DescuentoPct = rd.IsDBNull(14) ? 0m : rd.GetDecimal(14),
                    PrecioFijo = rd.IsDBNull(15) ? 0m : rd.GetDecimal(15),
                    PackCantidad = rd.IsDBNull(16) ? 0m : rd.GetDecimal(16),
                    PackPrecioTotal = rd.IsDBNull(17) ? 0m : rd.GetDecimal(17)
                };

                var tipoRegla = rd.IsDBNull(13) ? "" : rd.GetString(13);
                dto.EsPack = tipoRegla == "PACK_PRODUCTO";

                rd.Close();

                using var cmdProd = new SqlCommand(@"
SELECT  a.NoProducto,
        ISNULL(p.Descripcion, '') AS Descripcion,
        0 AS PrecioVenta,
        0 AS PrecioCoste
FROM dbo.PromoAlcance a
LEFT JOIN dbo.Producto p ON p.Nº = a.NoProducto
WHERE a.PromoId = @PromoId;", cn);

                cmdProd.Parameters.Add("@PromoId", SqlDbType.Int).Value = promoId;

                using var rd2 = cmdProd.ExecuteReader();
                while (rd2.Read())
                {
                    var prod = new PromoProductoDetalleProducto
                    {
                        Codigo = rd2.IsDBNull(0) ? "" : rd2.GetString(0),
                        Nombre = rd2.IsDBNull(1) ? "" : rd2.GetString(1),
                        PrecioVenta = rd2.IsDBNull(2) ? 0m : rd2.GetDecimal(2),
                        PrecioCoste = rd2.IsDBNull(3) ? 0m : rd2.GetDecimal(3)
                    };
                    dto.Productos.Add(prod);
                }

                return dto;
            }
        }

        // =========================================
        //   REEMPLAZAR PRODUCTOS EN ALCANCE
        // =========================================
        public void ReemplazarProductosPromo(int promoId, IEnumerable<string> codigosProductos)
        {
            using var cn = Db.GetOpenConnection();
            using var tx = cn.BeginTransaction();

            try
            {
                using (var cmdDel = new SqlCommand(@"
DELETE FROM dbo.PromoAlcance
WHERE PromoId = @PromoId;", cn, tx))
                {
                    cmdDel.Parameters.Add("@PromoId", SqlDbType.Int).Value = promoId;
                    cmdDel.ExecuteNonQuery();
                }

                using (var cmdIns = new SqlCommand(@"
INSERT INTO dbo.PromoAlcance
    (PromoId, TipoObjetivo, NoProducto, CategoriaId, SubcategoriaId, ClienteCodigo)
VALUES
    (@PromoId, 'PRODUCTO', @NoProducto, NULL, NULL, NULL);", cn, tx))
                {
                    cmdIns.Parameters.Add("@PromoId", SqlDbType.Int).Value = promoId;
                    var pNoProd = cmdIns.Parameters.Add("@NoProducto", SqlDbType.VarChar, 20);

                    foreach (var cod in codigosProductos)
                    {
                        if (string.IsNullOrWhiteSpace(cod))
                            continue;

                        pNoProd.Value = cod.Trim();
                        cmdIns.ExecuteNonQuery();
                    }
                }

                tx.Commit();
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }
    }
}
