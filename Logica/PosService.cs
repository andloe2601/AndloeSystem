using Andloe.Data;
using Andloe.Entidad;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Andloe.Logica
{
    public class PosService
    {
        private readonly NumeradorRepository _numRepo = new();
        private readonly VentaRepository _ventaRepo = new();
        private readonly VentaLineaRepository _lineRepo = new();
        private readonly PosPagoRepository _pagoRepo = new();
        private readonly ProductoRepository _prodRepo = new();
        private readonly ClienteRepository _cliRepo = new();
        private readonly InvMovimientoCabRepository _invCabRepo = new();
        private readonly InvMovimientoLinRepository _invLinRepo = new();
        private readonly AuditoriaService _audit = new();

        // DESCUENTO / CONFIG
        private readonly ConfigCalcService _configService;
        private readonly ClienteRepository _clienteRepo;
        private readonly ProductoRepository _productoRepo;

        // PROMOS
        private readonly PromoService _promoService = new();

        // ✅ NUEVO: para registrar promos dentro del mismo TX del cierre
        private readonly PromoRepository _promoRepo = new();

        public PosService()
        {
            var repoConfig = new SistemaConfigRepository();
            _configService = new ConfigCalcService(repoConfig);

            _clienteRepo = _cliRepo;
            _productoRepo = _prodRepo;
        }

        public PosService(
            ConfigCalcService configService,
            ClienteRepository clienteRepo,
            ProductoRepository productoRepo)
        {
            _configService = configService;
            _clienteRepo = clienteRepo;
            _productoRepo = productoRepo;
        }

        // ================== DESCUENTOS ==================

        public decimal CalcularMaxDescuentoPct(string clienteCodigo, string productoCodigo)
        {
            var maxGlobal = _configService.GetDecimal("DESCUENTO_MAX_GLOBAL", 0m);

            var cliente = string.IsNullOrWhiteSpace(clienteCodigo)
                ? null
                : _clienteRepo.ObtenerPorCodigo(clienteCodigo);

            var producto = _productoRepo.ObtenerPorCodigo(productoCodigo);

            var descCliente = cliente?.DescuentoPctMax ?? 0m;
            var descProducto = producto?.DescuentoPctMax ?? 0m;

            var maxLocal = Math.Max(descCliente, descProducto);

            return Math.Min(maxGlobal, maxLocal);
        }

        public void AplicarDescuento(ItemCarrito item, string clienteCodigo)
        {
            var maxPermitido = CalcularMaxDescuentoPct(clienteCodigo, item.ProductoCodigo);

            item.DescuentoPct = maxPermitido;
            item.DescuentoMonto = Math.Round(item.SubtotalBruto * (item.DescuentoPct / 100m), 2);
        }

        // ================== PROMOS ==================

        public PromoAplicadaResult? CalcularPromoParaItem(
            string? clienteCodigo,
            ItemCarrito item,
            int? categoriaId = null,
            int? subcategoriaId = null)
        {
            if (item == null) return null;

            return _promoService.CalcularMejorPromoLinea(
                clienteCodigo: clienteCodigo,
                productoCodigo: item.ProductoCodigo,
                categoriaId: categoriaId,
                subcategoriaId: subcategoriaId,
                cantidad: item.Cantidad,
                precioUnit: item.PrecioUnit
            );
        }

        public void AplicarPromoEnItem(
            string? clienteCodigo,
            ItemCarrito item,
            int? categoriaId = null,
            int? subcategoriaId = null)
        {
            if (item == null) return;

            var promo = _promoService.CalcularMejorPromoLinea(
                clienteCodigo: clienteCodigo,
                productoCodigo: item.ProductoCodigo,
                categoriaId: categoriaId,
                subcategoriaId: subcategoriaId,
                cantidad: item.Cantidad,
                precioUnit: item.PrecioUnit
            );

            if (promo == null)
                return;

            if (promo.DescuentoPct > 0)
                item.DescuentoPct = promo.DescuentoPct;

            if (promo.MontoDescuentoCalculado > 0)
                item.DescuentoMonto = promo.MontoDescuentoCalculado;
        }

        // ================== CARRITO ==================

        public List<ItemCarrito> Carrito { get; } = new();

        public ItemCarrito? AgregarPorCodigo(string codigo, decimal cantidad = 1, string? clienteCodigo = null)
        {
            var itemInfo = _prodRepo.GetItemPOS(codigo);
            if (itemInfo == null)
                return null;

            var (codigoProd, desc, precio, itbis) = itemInfo.Value;

            if (!ConfigService.PermitirStockNegativo)
            {
                try
                {
                    var stockActual = _productoRepo.ObtenerStockActual(codigoProd);
                    var enCarrito = Carrito
                        .Where(x => x.ProductoCodigo == codigoProd)
                        .Sum(x => x.Cantidad);

                    var totalSolicitado = enCarrito + cantidad;

                    if (stockActual < totalSolicitado)
                        return null;
                }
                catch
                {
                    return null;
                }
            }

            var prodFull = _productoRepo.ObtenerPorCodigo(codigoProd);
            int? categoriaId = prodFull?.CategoriaId;
            int? subcategoriaId = prodFull?.SubcategoriaId;

            ItemCarrito item;

            var existente = Carrito.FirstOrDefault(x => x.ProductoCodigo == codigoProd);
            if (existente != null)
            {
                existente.Cantidad += cantidad;
                item = existente;
            }
            else
            {
                var nuevo = new ItemCarrito
                {
                    ProductoCodigo = codigoProd,
                    Descripcion = desc,
                    Cantidad = cantidad,
                    PrecioUnit = precio,
                    ItbisPct = itbis
                };

                Carrito.Add(nuevo);
                item = nuevo;
            }

            AplicarPromoEnItem(
                clienteCodigo: clienteCodigo,
                item: item,
                categoriaId: categoriaId,
                subcategoriaId: subcategoriaId
            );

            if (!string.IsNullOrWhiteSpace(clienteCodigo))
            {
                _promoService.AplicarPromosCarrito(Carrito, clienteCodigo);
            }

            return item;
        }

        public void Quitar(string productoCodigo)
        {
            Carrito.RemoveAll(x => x.ProductoCodigo == productoCodigo);
        }

        public (decimal Subtotal, decimal Itbis, decimal Total) Totales()
        {
            decimal s = 0, i = 0, t = 0;

            foreach (var it in Carrito)
            {
                s += it.Importe;
                i += it.ItbisMonto;
                t += it.Total;
            }

            return (Math.Round(s, 2), Math.Round(i, 2), Math.Round(t, 2));
        }

        // ================== CERRAR VENTA ==================

        public long CerrarVenta(
    string usuario,
    string? clienteCodigo,
    int medioPagoId,
    decimal montoRecibido,
    string? moneda = null,
    int terminoPagoId = 1,
    string? posCajaNumero = null,
    int cajaId = 0 
)

        {
            if (Carrito.Count == 0)
                throw new InvalidOperationException("Carrito vacío.");

            if (string.IsNullOrWhiteSpace(clienteCodigo))
                clienteCodigo = ConfigService.ClienteDefecto;

            if (string.IsNullOrWhiteSpace(moneda))
                moneda = ConfigService.MonedaDefecto;

            var ahora = DateTime.Now;

            // 1) Aplicar TODAS las promos al carrito (combos + individuales)
            var promosAplicadas = _promoService.AplicarPromosCarrito(Carrito, clienteCodigo);

            // 2) Totales ya con descuentos
            var totales = Totales();
            var descuentoTotal = Carrito.Sum(x => x.DescuentoMonto);

            var venta = new Venta
            {
                NoDocumento = NumeradorConfigService.NextFacturaVenta(),
                ClienteCodigo = clienteCodigo,
                MonedaCodigo = moneda!,
                TasaCambio = 1,
                Subtotal = totales.Subtotal,
                ImpuestoTotal = totales.Itbis,
                Total = totales.Total,
                DescuentoTotal = descuentoTotal,
                Usuario = usuario,
                Observacion = "POS",
                Estado = "FACTURADA",
                MedioPagoId = medioPagoId,
                TerminoPagoId = terminoPagoId,
                MontoPago = montoRecibido,
                MontoCambio = Math.Max(0, montoRecibido - totales.Total),
                Fecha = ahora,
                FechaCreacion = ahora,
                SubTotalMoneda = totales.Subtotal,
                ItbisMoneda = totales.Itbis,
                TotalMoneda = totales.Total,
                POS_CajaNumero = posCajaNumero
            };

            using var cn = Db.GetOpenConnection();
            using var tx = cn.BeginTransaction(IsolationLevel.ReadCommitted);

            long ventaId;

            try
            {
                // 3) Cabecera de venta
                ventaId = _ventaRepo.InsertCabecera(venta, tx);

                // 4) Movimiento de inventario
                var almOrigenId = ConfigService.AlmacenPosOrigenId;
                var almDestinoId = ConfigService.AlmacenPosDestinoId;

                if (almOrigenId <= 0)
                    throw new InvalidOperationException(
                        "No está configurado el almacén de origen para POS. " +
                        "Vaya a Configuración > General y seleccione un almacén.");

                var movCab = new InvMovimientoCab
                {
                    Fecha = ahora,
                    Tipo = "SALIDA",
                    Origen = "VENTA",
                    OrigenId = ventaId,
                    AlmacenIdOrigen = almOrigenId,
                    AlmacenIdDestino = almDestinoId > 0 ? almDestinoId : (int?)null,
                    Usuario = usuario,
                    Observacion = "Salida por venta POS",
                    Estado = "APLICADO"
                };

                var invMovId = _invCabRepo.InsertCabecera(movCab, tx);

                var permitirNegativo = ConfigService.PermitirStockNegativo;

                // ✅ Indexar promos por item (una sola vez)
                var promosPorItem = promosAplicadas
                    .GroupBy(p => p.Item)
                    .ToDictionary(g => g.Key, g => g.Select(x => x.Promo).ToList());

                // ✅ Acumular por (PromoId, ProductoCodigo) una sola vez en todo el cierre
                var acumuladoPromoProd = new Dictionary<(int PromoId, string ProductoCodigo), (int Linea, decimal MontoTotal)>();

                // ✅ Periodo calculado una vez
                var (periodoTipo, periodoClave) = PromoRepository.PeriodoDiario(ahora);

                // 5) Líneas
                int linea = 1;
                foreach (var it in Carrito)
                {
                    int lineaActual = linea;

                    var lin = new VentaLinea
                    {
                        VentaId = ventaId,
                        Linea = lineaActual,
                        ProductoCodigo = it.ProductoCodigo,
                        Descripcion = it.Descripcion,
                        Cantidad = it.Cantidad,
                        PrecioUnit = it.PrecioUnit,
                        ItbisPct = it.ItbisPct,
                        ItbisMonto = it.ItbisMonto,
                        Importe = it.Importe,
                        TotalLinea = it.Total,
                        DescuentoPct = it.DescuentoPct,
                        DescuentoMonto = it.DescuentoMonto
                    };

                    _lineRepo.InsertLinea(lin, tx);

                    _productoRepo.RestarStock(
                        it.ProductoCodigo,
                        it.Cantidad,
                        cn,
                        tx,
                        permitirNegativo);

                    var movLin = new InvMovimientoLin
                    {
                        InvMovId = invMovId,
                        Linea = lineaActual,
                        ProductoCodigo = it.ProductoCodigo,
                        Cantidad = it.Cantidad,
                        CostoUnitario = it.PrecioUnit
                    };

                    _invLinRepo.InsertLinea(movLin, tx);

                    // ✅ Acumular promos por (PromoId, ProductoCodigo)
                    if (promosPorItem.TryGetValue(it, out var promosItem))
                    {
                        foreach (var promo in promosItem)
                        {
                            if (promo == null || promo.MontoDescuentoCalculado <= 0)
                                continue;

                            var key = (promo.PromoId, it.ProductoCodigo);

                            if (acumuladoPromoProd.TryGetValue(key, out var cur))
                            {
                                acumuladoPromoProd[key] = (cur.Linea, cur.MontoTotal + promo.MontoDescuentoCalculado);
                            }
                            else
                            {
                                acumuladoPromoProd[key] = (lineaActual, promo.MontoDescuentoCalculado);
                            }
                        }
                    }

                    linea++;
                }


                // ✅ Registrar PromoLog + PromoTope UNA sola vez por (PromoId, ProductoCodigo)
                foreach (var kv in acumuladoPromoProd)
                {
                    var promoId = kv.Key.PromoId;
                    var productoCodigo = kv.Key.ProductoCodigo;
                    var lineaPromo = kv.Value.Linea;
                    var montoTotal = Math.Round(kv.Value.MontoTotal, 2);

                    if (montoTotal <= 0) continue;

                    _promoRepo.RegistrarUsoPromoTx(
                        promoId: promoId,
                        origen: "VENTA",
                        origenId: ventaId,
                        linea: lineaPromo,
                        montoDesc: montoTotal,
                        monedaCodigo: moneda,
                        tasaCambio: venta.TasaCambio,
                        usuario: usuario,
                        fechaAplic: ahora,
                        periodoTipo: periodoTipo,
                        periodoClave: periodoClave,
                        clienteCodigo: clienteCodigo,
                        productoCodigo: productoCodigo,
                        cn: cn,
                        tx: tx
                    );
                }

                // ✅ A4: Asiento automático al facturar (dentro del TX)
                _ = AsentarVentaPOS(
                    cn: cn,
                    tx: tx,
                    ventaId: ventaId,
                    fecha: ahora,
                    usuario: usuario,
                    monedaCodigo: venta.MonedaCodigo,
                    tasaCambio: venta.TasaCambio,
                    subtotalMoneda: venta.SubTotalMoneda,
                    itbisMoneda: venta.ItbisMoneda,
                    cajaId: cajaId,
                    terminoPagoId: venta.TerminoPagoId
                );


                tx.Commit();
            }
            catch
            {
                tx?.Rollback();
                throw;
            }

            // ✅ IMPORTANTE: ya NO se registra fuera del TX (evita duplicar)
            return ventaId;
        }

        // ================== PAGOS POS ==================

        public void GuardarPagosPOS(
            long ventaId,
            IEnumerable<PagoLineaResult> pagos,
            string usuario,
            int cajaId,
            string? cajaNumero)
        {
            var lista = pagos?.ToList() ?? new();
            if (lista.Count == 0) return;

            using var cn = Db.GetOpenConnection();
            using var tx = cn.BeginTransaction();

            try
            {
                foreach (var p in lista)
                {
                    using var cmd = new SqlCommand(@"
INSERT INTO dbo.POS_Pago
(Fecha, MonedaCodigo, TasaCambio, Monto, MedioPagoId, Referencia, Entidad,
 Observacion, Usuario, CajaId, POS_CajaNumero, VentaId, Estado)
VALUES
(@Fecha, @MonedaCodigo, @TasaCambio, @Monto, @MedioPagoId, @Referencia, @Entidad,
 @Observacion, @Usuario, @CajaId, @POS_CajaNumero, @VentaId, @Estado);", cn, tx);

                    cmd.Parameters.Add("@Fecha", SqlDbType.DateTime).Value = DateTime.Now;
                    cmd.Parameters.Add("@MonedaCodigo", SqlDbType.VarChar, 3).Value = p.MonedaCodigo;

                    var tc = cmd.Parameters.Add("@TasaCambio", SqlDbType.Decimal);
                    tc.Precision = 18; tc.Scale = 6; tc.Value = p.TasaCambio;

                    var monto = cmd.Parameters.Add("@Monto", SqlDbType.Decimal);
                    monto.Precision = 18; monto.Scale = 2; monto.Value = p.MontoBase;

                    cmd.Parameters.Add("@MedioPagoId", SqlDbType.Int).Value = p.MedioPagoId;
                    cmd.Parameters.Add("@Referencia", SqlDbType.VarChar, 50).Value = "POS";
                    cmd.Parameters.Add("@Entidad", SqlDbType.VarChar, 80).Value = "CAJA POS";
                    cmd.Parameters.Add("@Observacion", SqlDbType.VarChar, 200).Value = "Pago POS";
                    cmd.Parameters.Add("@Usuario", SqlDbType.VarChar, 30).Value = usuario;
                    cmd.Parameters.Add("@CajaId", SqlDbType.Int).Value = cajaId;

                    if (string.IsNullOrWhiteSpace(cajaNumero))
                        cmd.Parameters.Add("@POS_CajaNumero", SqlDbType.NVarChar, 10).Value = DBNull.Value;
                    else
                        cmd.Parameters.Add("@POS_CajaNumero", SqlDbType.NVarChar, 10).Value = cajaNumero;

                    cmd.Parameters.Add("@VentaId", SqlDbType.BigInt).Value = ventaId;
                    cmd.Parameters.Add("@Estado", SqlDbType.VarChar, 20).Value = "APLICADO";

                    cmd.ExecuteNonQuery();
                }

                tx.Commit();
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }

        private sealed class CajaContaCtas
        {
            public int? CtaCajaId { get; set; }
            public int? CtaClienteId { get; set; }
            public int? CtaIngresoId { get; set; }
            public int? CtaItbisId { get; set; }
        }

        private CajaContaCtas LeerConfigCajaConta(SqlConnection cn, SqlTransaction tx, int cajaId, string monedaCodigo)
        {
            using var cmd = new SqlCommand(@"
SELECT TOP(1)
    COALESCE(CtaCajaId, CuentaCajaId)   AS CtaCajaId,
    CtaClienteId,
    CtaIngresoId,
    CtaITBISId                          AS CtaItbisId
FROM dbo.CajaContaConfig
WHERE CajaId = @CajaId AND MonedaCodigo = @MonedaCodigo;", cn, tx);

            cmd.Parameters.Add("@CajaId", SqlDbType.Int).Value = cajaId;
            cmd.Parameters.Add("@MonedaCodigo", SqlDbType.VarChar, 3).Value = monedaCodigo;

            using var rd = cmd.ExecuteReader();
            if (!rd.Read())
                throw new InvalidOperationException(
                    $"No hay configuración contable en CajaContaConfig para CajaId={cajaId} y Moneda={monedaCodigo}.");

            return new CajaContaCtas
            {
                CtaCajaId = rd["CtaCajaId"] == DBNull.Value ? null : Convert.ToInt32(rd["CtaCajaId"]),
                CtaClienteId = rd["CtaClienteId"] == DBNull.Value ? null : Convert.ToInt32(rd["CtaClienteId"]),
                CtaIngresoId = rd["CtaIngresoId"] == DBNull.Value ? null : Convert.ToInt32(rd["CtaIngresoId"]),
                CtaItbisId = rd["CtaItbisId"] == DBNull.Value ? null : Convert.ToInt32(rd["CtaItbisId"]),
            };
        }

        private long CrearAsientoVentaPos(
            SqlConnection cn,
            SqlTransaction tx,
            DateTime fecha,
            long ventaId,
            string usuario,
            string monedaCodigo,
            decimal tasaCambio,
            decimal subtotalMoneda,
            decimal itbisMoneda,
            int cajaId,
            int terminoPagoId
        )
        {
            if (cajaId <= 0)
                throw new InvalidOperationException("CajaId inválido. No se puede contabilizar la venta POS sin CajaId.");

            var ctas = LeerConfigCajaConta(cn, tx, cajaId, monedaCodigo);

            // Regla mínima (sin asumir cosas raras):
            // - Contado (terminoPagoId == 1): Debe Caja
            // - Crédito: Debe Cliente (CxC)
            bool esContado = (terminoPagoId == 1);

            int? cuentaDebe = esContado ? ctas.CtaCajaId : ctas.CtaClienteId;
            if (cuentaDebe is null || cuentaDebe <= 0)
                throw new InvalidOperationException(esContado
                    ? "Falta CtaCajaId/CuentaCajaId en CajaContaConfig."
                    : "Falta CtaClienteId en CajaContaConfig.");

            if (ctas.CtaIngresoId is null || ctas.CtaIngresoId <= 0)
                throw new InvalidOperationException("Falta CtaIngresoId en CajaContaConfig.");

            // ITBIS puede ser 0, pero si hay ITBIS y no hay cuenta, fallo:
            if (itbisMoneda != 0 && (ctas.CtaItbisId is null || ctas.CtaItbisId <= 0))
                throw new InvalidOperationException("La venta tiene ITBIS pero falta CtaITBISId en CajaContaConfig.");

            long movId;
            string noAsiento;

            // 1) Crear cabecera
            using (var cmd = new SqlCommand("dbo.sp_Conta_Mov_Crear", cn, tx))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@Fecha", SqlDbType.DateTime2).Value = fecha;
                cmd.Parameters.Add("@Descripcion", SqlDbType.VarChar, 200).Value = $"Venta POS {ventaId}";
                cmd.Parameters.Add("@Origen", SqlDbType.VarChar, 20).Value = "VENTA";
                cmd.Parameters.Add("@OrigenId", SqlDbType.BigInt).Value = ventaId;
                cmd.Parameters.Add("@Usuario", SqlDbType.NVarChar, 120).Value = usuario;
                cmd.Parameters.Add("@MonedaCodigo", SqlDbType.VarChar, 3).Value = monedaCodigo;

                var pTc = cmd.Parameters.Add("@TasaCambio", SqlDbType.Decimal);
                pTc.Precision = 18; pTc.Scale = 6; pTc.Value = tasaCambio <= 0 ? 1 : tasaCambio;

                var pMov = cmd.Parameters.Add("@MovimientoId", SqlDbType.BigInt);
                pMov.Direction = ParameterDirection.Output;

                var pNo = cmd.Parameters.Add("@NoAsiento", SqlDbType.VarChar, 30);
                pNo.Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();

                movId = Convert.ToInt64(pMov.Value);
                noAsiento = Convert.ToString(pNo.Value) ?? "";
            }

            decimal totalMoneda = Math.Round(subtotalMoneda + itbisMoneda, 2);

            // 2) Líneas (por CuentaId)
            void AddLinea(int cuentaId, string desc, decimal debMon, decimal credMon)
            {
                using var cmd = new SqlCommand("dbo.sp_Conta_Mov_AddLinea", cn, tx);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@MovimientoId", SqlDbType.BigInt).Value = movId;
                cmd.Parameters.Add("@CuentaId", SqlDbType.Int).Value = cuentaId;
                cmd.Parameters.Add("@Descripcion", SqlDbType.VarChar, 100).Value = desc;

                var pDeb = cmd.Parameters.Add("@DebitoMoneda", SqlDbType.Decimal);
                pDeb.Precision = 18; pDeb.Scale = 2; pDeb.Value = debMon;

                var pCred = cmd.Parameters.Add("@CreditoMoneda", SqlDbType.Decimal);
                pCred.Precision = 18; pCred.Scale = 2; pCred.Value = credMon;

                cmd.ExecuteNonQuery();
            }

            AddLinea(cuentaDebe.Value, esContado ? "Caja venta POS" : "CxC venta POS", totalMoneda, 0m);
            AddLinea(ctas.CtaIngresoId.Value, "Ingresos por venta", 0m, Math.Round(subtotalMoneda, 2));

            if (itbisMoneda != 0)
                AddLinea(ctas.CtaItbisId!.Value, "ITBIS por pagar", 0m, Math.Round(itbisMoneda, 2));

            // 3) Cerrar
            using (var cmd = new SqlCommand("dbo.sp_Conta_Mov_Cerrar", cn, tx))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@MovimientoId", SqlDbType.BigInt).Value = movId;
                cmd.ExecuteNonQuery();
            }

            // 4) Dejarlo CONTABILIZADO (para poder reversar luego si lo necesitas)
            using (var cmd = new SqlCommand(@"
UPDATE dbo.ContabilidadMovimientoCab
SET Estado = 'CONTABILIZADO'
WHERE MovimientoId = @MovimientoId;", cn, tx))
            {
                cmd.Parameters.Add("@MovimientoId", SqlDbType.BigInt).Value = movId;
                cmd.ExecuteNonQuery();
            }

            // 5) Amarrar la venta con asiento + caja
            using (var cmd = new SqlCommand(@"
UPDATE dbo.VentaCab
SET MovimientoIdConta = @MovimientoIdConta,
    CajaId = @CajaId
WHERE VentaId = @VentaId;", cn, tx))
            {
                cmd.Parameters.Add("@MovimientoIdConta", SqlDbType.BigInt).Value = movId;
                cmd.Parameters.Add("@CajaId", SqlDbType.Int).Value = cajaId;
                cmd.Parameters.Add("@VentaId", SqlDbType.BigInt).Value = ventaId;
                cmd.ExecuteNonQuery();
            }

            return movId;
        }

        private sealed class CuentasCajaConta
        {
            public int CuentaCajaId { get; set; }
            public int? CtaClienteId { get; set; }
            public int? CtaIngresoId { get; set; }
            public int? CtaITBISId { get; set; }
        }

        private CuentasCajaConta LeerCajaContaConfig(SqlConnection cn, SqlTransaction tx, int cajaId, string monedaCodigo)
        {
            using var cmd = new SqlCommand(@"
SELECT TOP(1)
    CuentaCajaId,
    CtaClienteId,
    CtaIngresoId,
    CtaITBISId
FROM dbo.CajaContaConfig
WHERE CajaId = @CajaId AND MonedaCodigo = @Moneda;", cn, tx);

            cmd.Parameters.Add("@CajaId", SqlDbType.Int).Value = cajaId;
            cmd.Parameters.Add("@Moneda", SqlDbType.VarChar, 3).Value = monedaCodigo;

            using var rd = cmd.ExecuteReader();
            if (!rd.Read())
                throw new InvalidOperationException(
                    $"No existe CajaContaConfig para CajaId={cajaId} y Moneda={monedaCodigo}.");

            return new CuentasCajaConta
            {
                CuentaCajaId = Convert.ToInt32(rd["CuentaCajaId"]),
                CtaClienteId = rd["CtaClienteId"] == DBNull.Value ? null : Convert.ToInt32(rd["CtaClienteId"]),
                CtaIngresoId = rd["CtaIngresoId"] == DBNull.Value ? null : Convert.ToInt32(rd["CtaIngresoId"]),
                CtaITBISId = rd["CtaITBISId"] == DBNull.Value ? null : Convert.ToInt32(rd["CtaITBISId"]),
            };
        }

        private string GetCodigoCuenta(SqlConnection cn, SqlTransaction tx, int cuentaId)
        {
            using var cmd = new SqlCommand(@"
SELECT Codigo
FROM dbo.ContabilidadCatalogoCuenta
WHERE CuentaId = @CuentaId;", cn, tx);

            cmd.Parameters.Add("@CuentaId", SqlDbType.Int).Value = cuentaId;

            var obj = cmd.ExecuteScalar();
            var codigo = obj == null || obj == DBNull.Value ? null : Convert.ToString(obj);

            if (string.IsNullOrWhiteSpace(codigo))
                throw new InvalidOperationException($"No se encontró Codigo en ContabilidadCatalogoCuenta para CuentaId={cuentaId}.");

            codigo = codigo.Trim();

            // sp_Conta_AsientoVenta usa varchar(15) para las cuentas (verificado en script).
            if (codigo.Length > 15)
                throw new InvalidOperationException($"El código de cuenta '{codigo}' excede 15 caracteres; ajusta el código o el SP.");

            return codigo;
        }

        private long AsentarVentaPOS(
            SqlConnection cn,
            SqlTransaction tx,
            long ventaId,
            DateTime fecha,
            string usuario,
            string monedaCodigo,
            decimal tasaCambio,
            decimal subtotalMoneda,
            decimal itbisMoneda,
            int cajaId,
            int terminoPagoId
        )
        {
            // Regla mínima (sin inventar otra): si terminoPagoId == 1 => contado.
            // Si luego tú manejas crédito real, aquí cambiamos la regla con tu tabla TerminoPago.
            bool esContado = (terminoPagoId == 1);

            var cfg = LeerCajaContaConfig(cn, tx, cajaId, monedaCodigo);

            if (cfg.CtaIngresoId is null)
                throw new InvalidOperationException("CajaContaConfig: falta CtaIngresoId.");
            if (itbisMoneda != 0 && cfg.CtaITBISId is null)
                throw new InvalidOperationException("CajaContaConfig: falta CtaITBISId y la venta tiene ITBIS.");
            if (!esContado && cfg.CtaClienteId is null)
                throw new InvalidOperationException("CajaContaConfig: falta CtaClienteId para ventas a crédito.");

            var cuentaDebeId = esContado ? cfg.CuentaCajaId : cfg.CtaClienteId!.Value;

            var cuentaDebeCodigo = GetCodigoCuenta(cn, tx, cuentaDebeId);
            var cuentaIngresoCodigo = GetCodigoCuenta(cn, tx, cfg.CtaIngresoId.Value);
            var cuentaItbisCodigo = (itbisMoneda != 0) ? GetCodigoCuenta(cn, tx, cfg.CtaITBISId!.Value) : "1.1.01"; // no se usa si itbisMoneda==0

            long movId;
            string noAsiento;

            using (var cmd = new SqlCommand("dbo.sp_Conta_AsientoVenta", cn, tx))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@Fecha", SqlDbType.DateTime2).Value = fecha;
                cmd.Parameters.Add("@Origen", SqlDbType.VarChar, 20).Value = "VENTA";
                cmd.Parameters.Add("@OrigenId", SqlDbType.BigInt).Value = ventaId;
                cmd.Parameters.Add("@Usuario", SqlDbType.NVarChar, 120).Value = usuario;
                cmd.Parameters.Add("@MonedaCodigo", SqlDbType.VarChar, 3).Value = monedaCodigo;

                var pTc = cmd.Parameters.Add("@TasaCambio", SqlDbType.Decimal);
                pTc.Precision = 18; pTc.Scale = 6; pTc.Value = (tasaCambio <= 0 ? 1m : tasaCambio);

                var pSub = cmd.Parameters.Add("@SubtotalMoneda", SqlDbType.Decimal);
                pSub.Precision = 18; pSub.Scale = 2; pSub.Value = subtotalMoneda;

                var pItb = cmd.Parameters.Add("@ItbisMoneda", SqlDbType.Decimal);
                pItb.Precision = 18; pItb.Scale = 2; pItb.Value = itbisMoneda;

                // NOTA: el SP llama este parámetro @CuentaCxC, pero lo usamos como "cuenta debe"
                // (caja si contado, cliente si crédito).
                cmd.Parameters.Add("@CuentaCxC", SqlDbType.VarChar, 15).Value = cuentaDebeCodigo;
                cmd.Parameters.Add("@CuentaIngreso", SqlDbType.VarChar, 15).Value = cuentaIngresoCodigo;
                cmd.Parameters.Add("@CuentaITBIS", SqlDbType.VarChar, 15).Value = cuentaItbisCodigo;

                var pMov = cmd.Parameters.Add("@MovimientoId", SqlDbType.BigInt);
                pMov.Direction = ParameterDirection.Output;

                var pNo = cmd.Parameters.Add("@NoAsiento", SqlDbType.VarChar, 30);
                pNo.Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();

                movId = Convert.ToInt64(pMov.Value);
                noAsiento = Convert.ToString(pNo.Value) ?? "";
            }

            // Amarrar asiento + caja en VentaCab (verificado que existen estas columnas y FK).
            using (var cmd = new SqlCommand(@"
UPDATE dbo.VentaCab
SET MovimientoIdConta = @MovId,
    CajaId = @CajaId
WHERE VentaId = @VentaId;", cn, tx))
            {
                cmd.Parameters.Add("@MovId", SqlDbType.BigInt).Value = movId;
                cmd.Parameters.Add("@CajaId", SqlDbType.Int).Value = cajaId;
                cmd.Parameters.Add("@VentaId", SqlDbType.BigInt).Value = ventaId;
                cmd.ExecuteNonQuery();
            }

            return movId;
        }



        // ================== CLIENTE ==================

        public ClienteDto? BuscarClientePorRncOCodigo(string valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
                return null;

            return _cliRepo.BuscarPorCodigoORnc(valor.Trim());
        }

        public void RecalcularPromosCarrito(string? clienteCodigo = null)
        {
            _promoService.AplicarPromosCarrito(Carrito, clienteCodigo);
        }
    }
}
