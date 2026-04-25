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

        private void ValidarTipoComprobanteCliente(string clienteCodigo, int tipoECFId)
        {
            var cli = string.IsNullOrWhiteSpace(clienteCodigo)
                ? null
                : _clienteRepo.BuscarPorCodigoORnc(clienteCodigo);

            var rnc = (cli?.RncCedula ?? "").Trim();

            var esConsumidorFinal =
                string.IsNullOrWhiteSpace(rnc) ||
                rnc == "000000000" ||
                rnc == "00000000000";

            if (tipoECFId == 1 && esConsumidorFinal)
                throw new InvalidOperationException(
                    "No se puede emitir crédito fiscal (E31) a consumidor final.");

            if (tipoECFId == 8 && esConsumidorFinal)
                throw new InvalidOperationException(
                    "No se puede emitir comprobante gubernamental (E45) a consumidor final.");
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
            if (string.IsNullOrWhiteSpace(codigo))
                return null;

            var prod = _prodRepo.ObtenerPorCodigoOBarras(codigo);
            if (prod == null || string.IsNullOrWhiteSpace(prod.Codigo))
                return null;

            prod.NormalizeDefaults();

            var codigoProd = prod.Codigo.Trim();
            var desc = !string.IsNullOrWhiteSpace(prod.Referencia)
                ? prod.Referencia.Trim()
                : !string.IsNullOrWhiteSpace(prod.Descripcion)
                    ? prod.Descripcion.Trim()
                    : codigoProd;

            var precio = prod.PrecioVenta;
            if (precio <= 0m) precio = prod.PrecioCoste;
            if (precio <= 0m) precio = prod.PrecioCompraPromedio;
            if (precio <= 0m) precio = prod.UltimoPrecioCompra;

            var itbis = ObtenerItbisPctProducto(prod);

            if (!ConfigService.PermitirStockNegativo)
            {
                try
                {
                    var stockActual = _productoRepo.ObtenerStockActual(codigoProd);
                    var enCarrito = Carrito
                        .Where(x => x.ProductoCodigo == codigoProd)
                        .Sum(x => x.Cantidad);

                    if (stockActual < enCarrito + cantidad)
                        return null;
                }
                catch
                {
                    return null;
                }
            }

            var existente = Carrito.FirstOrDefault(x => x.ProductoCodigo == codigoProd);
            ItemCarrito item;

            if (existente != null)
            {
                existente.Cantidad += cantidad;
                item = existente;
            }
            else
            {
                item = new ItemCarrito
                {
                    ProductoCodigo = codigoProd,
                    Descripcion = desc,
                    Cantidad = cantidad,
                    PrecioUnit = precio,
                    ItbisPct = itbis,
                    PrecioIncluyeITBIS = prod.PrecioIncluyeITBIS
                };

                Carrito.Add(item);
            }

            AplicarPromoEnItem(
                clienteCodigo: clienteCodigo,
                item: item,
                categoriaId: prod.CategoriaId,
                subcategoriaId: prod.SubcategoriaId
            );

            if (!string.IsNullOrWhiteSpace(clienteCodigo))
                _promoService.AplicarPromosCarrito(Carrito, clienteCodigo);

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

        private decimal ObtenerItbisPctProducto(Producto prod)
        {
            if (prod == null)
                return 0m;

            if (prod.EsExento)
                return 0m;

            if (!prod.ImpuestoId.HasValue || prod.ImpuestoId.Value <= 0)
                return 0m;

            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
SELECT TOP (1)
    ISNULL(Porcentaje, 0)
FROM dbo.Impuesto
WHERE ImpuestoId = @ImpuestoId;", cn);

            cmd.Parameters.Add("@ImpuestoId", SqlDbType.Int).Value = prod.ImpuestoId.Value;

            var value = cmd.ExecuteScalar();

            return value == null || value == DBNull.Value
                ? 0m
                : Convert.ToDecimal(value);
        }


        // ================== CERRAR VENTA ==================

        public long CerrarVenta(
    string usuario,
    string clienteCodigo,
    int medioPagoId,
    decimal montoRecibido,
    string moneda,
    int terminoPagoId,
    string posCajaNumero,
    int cajaId,
    int tipoECFId,
    int tipoPagoECFId,
    string formaPagoFiscal)
        {
            if (Carrito.Count == 0)
                throw new InvalidOperationException("Carrito vacío.");

            if (tipoECFId <= 0)
                throw new InvalidOperationException("Debe indicar un tipo de comprobante fiscal válido.");

            if (tipoPagoECFId <= 0)
                throw new InvalidOperationException("Debe indicar un tipo de pago ECF válido.");

            if (string.IsNullOrWhiteSpace(formaPagoFiscal))
                throw new InvalidOperationException("Debe indicar una forma de pago fiscal válida.");

            if (string.IsNullOrWhiteSpace(clienteCodigo))
                clienteCodigo = ConfigService.ClienteDefecto;

            if (string.IsNullOrWhiteSpace(moneda))
                moneda = ConfigService.MonedaDefecto;

            var ahora = DateTime.Now;

            var promosAplicadas = _promoService.AplicarPromosCarrito(Carrito, clienteCodigo);

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
                Observacion = $"POS | TipoECFId={tipoECFId} | TipoPagoECFId={tipoPagoECFId} | FormaPagoFiscal={formaPagoFiscal}",
                Estado = "FACTURADA",
                TerminoPagoId = terminoPagoId,
                MontoPago = montoRecibido,
                MontoCambio = Math.Max(0, montoRecibido - totales.Total),
                Fecha = ahora,
                FechaCreacion = ahora,
                SubTotalMoneda = totales.Subtotal,
                ItbisMoneda = totales.Itbis,
                TotalMoneda = totales.Total,
                POS_CajaNumero = posCajaNumero,
                CajaId = cajaId
            };

            using var cn = Db.GetOpenConnection();
            using var tx = cn.BeginTransaction(IsolationLevel.ReadCommitted);

            long ventaId;

            try
            {
                ventaId = _ventaRepo.InsertCabecera(venta, tx);

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

                var promosPorItem = promosAplicadas
                    .GroupBy(p => p.Item)
                    .ToDictionary(g => g.Key, g => g.Select(x => x.Promo).ToList());

                var acumuladoPromoProd = new Dictionary<(int PromoId, string ProductoCodigo), (int Linea, decimal MontoTotal)>();
                var (periodoTipo, periodoClave) = PromoRepository.PeriodoDiario(ahora);

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
                        DescuentoMonto = it.DescuentoMonto,
                        PrecioIncluyeITBIS = it.PrecioIncluyeITBIS
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

                    if (promosPorItem.TryGetValue(it, out var promosItem))
                    {
                        foreach (var promo in promosItem)
                        {
                            if (promo == null || promo.MontoDescuentoCalculado <= 0)
                                continue;

                            var key = (promo.PromoId, it.ProductoCodigo);

                            if (acumuladoPromoProd.TryGetValue(key, out var cur))
                                acumuladoPromoProd[key] = (cur.Linea, cur.MontoTotal + promo.MontoDescuentoCalculado);
                            else
                                acumuladoPromoProd[key] = (lineaActual, promo.MontoDescuentoCalculado);
                        }
                    }

                    linea++;
                }

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
            if (lista.Count == 0)
                return;

            using var cn = Db.GetOpenConnection();
            using var tx = cn.BeginTransaction();

            try
            {
                foreach (var p in lista)
                {
                    if (string.IsNullOrWhiteSpace(p.FormaPagoCodigo))
                        throw new InvalidOperationException(
                            "El pago no tiene FormaPagoCodigo. No se puede registrar en POS_Pago.");

                    using var cmd = new SqlCommand(@"
INSERT INTO dbo.POS_Pago
(
    Fecha,
    MonedaCodigo,
    TasaCambio,
    Monto,
    MontoBase,
    Referencia,
    Entidad,
    Observacion,
    Usuario,
    CajaId,
    VentaId,
    Estado,
    POS_CajaNumero,
    FormaPagoCodigo
)
VALUES
(
    @Fecha,
    @MonedaCodigo,
    @TasaCambio,
    @Monto,
    @MontoBase,
    @Referencia,
    @Entidad,
    @Observacion,
    @Usuario,
    @CajaId,
    @VentaId,
    @Estado,
    @POS_CajaNumero,
    @FormaPagoCodigo
);", cn, tx);

                    cmd.Parameters.Add("@Fecha", SqlDbType.DateTime).Value = DateTime.Now;
                    cmd.Parameters.Add("@MonedaCodigo", SqlDbType.VarChar, 3).Value = p.MonedaCodigo;

                    var tasa = cmd.Parameters.Add("@TasaCambio", SqlDbType.Decimal);
                    tasa.Precision = 18;
                    tasa.Scale = 6;
                    tasa.Value = p.TasaCambio <= 0 ? 1m : p.TasaCambio;

                    var monto = cmd.Parameters.Add("@Monto", SqlDbType.Decimal);
                    monto.Precision = 18;
                    monto.Scale = 2;
                    monto.Value = p.MontoMoneda;

                    var montoBase = cmd.Parameters.Add("@MontoBase", SqlDbType.Decimal);
                    montoBase.Precision = 18;
                    montoBase.Scale = 2;
                    montoBase.Value = p.MontoBase;

                    cmd.Parameters.Add("@Referencia", SqlDbType.VarChar, 60).Value = "POS";
                    cmd.Parameters.Add("@Entidad", SqlDbType.VarChar, 80).Value = "CAJA POS";

                    cmd.Parameters.Add("@Observacion", SqlDbType.VarChar, 200).Value =
                        string.IsNullOrWhiteSpace(p.NombreMedio)
                            ? (object)DBNull.Value
                            : $"Pago POS {p.NombreMedio}".Trim();

                    cmd.Parameters.Add("@Usuario", SqlDbType.VarChar, 50).Value =
                        string.IsNullOrWhiteSpace(usuario) ? (object)DBNull.Value : usuario.Trim();

                    cmd.Parameters.Add("@CajaId", SqlDbType.Int).Value = cajaId;
                    cmd.Parameters.Add("@VentaId", SqlDbType.BigInt).Value = ventaId;
                    cmd.Parameters.Add("@Estado", SqlDbType.VarChar, 12).Value = "APLICADO";
                    cmd.Parameters.Add("@POS_CajaNumero", SqlDbType.NVarChar, 20).Value =
                        string.IsNullOrWhiteSpace(cajaNumero) ? (object)DBNull.Value : cajaNumero.Trim();

                    cmd.Parameters.Add("@FormaPagoCodigo", SqlDbType.VarChar, 2).Value = p.FormaPagoCodigo.Trim();

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
