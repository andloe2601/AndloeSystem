using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.Data.SqlClient;
using Andloe.Data;
using Andloe.Entidad;

namespace Andloe.Logica
{
    public class PromoService
    {
        private readonly PromoRepository _repo = new();

        /// <summary>
        /// Calcula la mejor promo aplicable a UNA línea individual.
        /// NO maneja combos de varios productos; eso se hace en
        /// AplicarPromosCarrito.
        /// 
        /// Soporta:
        ///   - DESCUENTO_PCT
        ///   - DESCUENTO_MONTO
        ///   - PRECIO_FIJO
        ///   - PACK (mismo producto, Cant.pack &gt; 1)
        ///   - PACK_PRODUCTO (mismo producto, Cant.pack &gt; 1)
        /// </summary>
        public PromoAplicadaResult? CalcularMejorPromoLinea(
            string? clienteCodigo,
            string productoCodigo,
            int? categoriaId,
            int? subcategoriaId,
            decimal cantidad,
            decimal precioUnit)
        {
            if (string.IsNullOrWhiteSpace(productoCodigo))
                return null;

            var hoy = DateTime.Now.Date;

            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
SELECT TOP(1)
       c.PromoId,           -- 0
       c.Codigo,            -- 1
       c.Nombre,            -- 2
       c.TipoPromo,         -- 3
       c.Prioridad,         -- 4
       r.ReglaId,           -- 5
       r.TipoRegla,         -- 6
       r.DescuentoPct,      -- 7
       r.DescuentoMonto,    -- 8
       r.PrecioFijo,        -- 9
       r.MinCantidad,       --10
       r.Pack_BuyProducto,  --11
       r.Pack_BuyCant,      --12
       r.Pack_Precio        --13
FROM dbo.PromoCab        c
JOIN dbo.PromoRegla      r   ON r.PromoId = c.PromoId
JOIN dbo.PromoAlcance    a   ON a.PromoId = c.PromoId
JOIN dbo.PromoCalendario cal ON cal.PromoId = c.PromoId
WHERE c.Estado       = 'ACTIVA'
  AND c.TipoPromo    = 'DESCUENTO'
  AND a.TipoObjetivo = 'PRODUCTO'
  AND a.NoProducto   = @ProductoCodigo
  AND @Hoy >= CONVERT(date, cal.FechaInicio)
  AND @Hoy <= CONVERT(date, cal.FechaFin)
  AND r.TipoRegla IN ('DESCUENTO_PCT',
                      'DESCUENTO_MONTO',
                      'PRECIO_FIJO',
                      'PACK',
                      'PACK_PRODUCTO')
ORDER BY c.Prioridad DESC, c.PromoId DESC;", cn);

            cmd.Parameters.Add("@ProductoCodigo", SqlDbType.VarChar, 20).Value = productoCodigo;
            cmd.Parameters.Add("@Hoy", SqlDbType.Date).Value = hoy;

            using var rd = cmd.ExecuteReader();
            if (!rd.Read())
                return null;

            var promo = new PromoAplicadaResult
            {
                PromoId = rd.GetInt32(0),
                CodigoPromo = rd.GetString(1),
                NombrePromo = rd.GetString(2),
                TipoPromo = rd.GetString(3),
                Prioridad = rd.GetInt32(4),
                ReglaId = rd.IsDBNull(5) ? (int?)null : rd.GetInt32(5),
                TipoRegla = rd.GetString(6),
                DescuentoPct = rd.IsDBNull(7) ? 0m : rd.GetDecimal(7),
                DescuentoMonto = rd.IsDBNull(8) ? 0m : rd.GetDecimal(8),
                PrecioFijo = rd.IsDBNull(9) ? 0m : rd.GetDecimal(9),
                MinCantidad = rd.IsDBNull(10) ? (decimal?)null : rd.GetDecimal(10),
                PackBuyProducto = rd.IsDBNull(11) ? null : rd.GetString(11),
                PackBuyCant = rd.IsDBNull(12) ? 0m : rd.GetDecimal(12),
                PackPrecio = rd.IsDBNull(13) ? 0m : rd.GetDecimal(13),
            };

            var subtotalBruto = cantidad * precioUnit;
            decimal montoDesc = 0m;

            switch (promo.TipoRegla)
            {
                case "DESCUENTO_PCT":
                    if (promo.DescuentoPct > 0)
                        montoDesc = Math.Round(subtotalBruto * (promo.DescuentoPct / 100m), 2);
                    break;

                case "DESCUENTO_MONTO":
                    if (promo.DescuentoMonto > 0)
                    {
                        montoDesc = promo.DescuentoMonto;
                        if (montoDesc > subtotalBruto)
                            montoDesc = subtotalBruto;
                        montoDesc = Math.Round(montoDesc, 2);
                    }
                    break;

                case "PRECIO_FIJO":
                    if (promo.PrecioFijo > 0)
                    {
                        var totalPromo = promo.PrecioFijo * cantidad;
                        montoDesc = subtotalBruto - totalPromo;
                        if (montoDesc < 0) montoDesc = 0;
                        if (montoDesc > subtotalBruto) montoDesc = subtotalBruto;
                        montoDesc = Math.Round(montoDesc, 2);
                    }
                    break;

                case "PACK":
                case "PACK_PRODUCTO":
                    {
                        // IMPORTANTE:
                        // Aquí SOLO manejamos packs de un MISMO producto
                        // donde Cant.pack > 1. Cuando Cant.pack = 1,
                        // lo consideramos combo multi-producto y se maneja
                        // en AplicarPromosCarrito().
                        if (promo.PackBuyCant <= 1)
                            break;

                        const decimal ITBIS_POR_DEFECTO = 18m;

                        // Si viene Pack_BuyProducto, debe ser el mismo producto
                        if (!string.IsNullOrEmpty(promo.PackBuyProducto) &&
                            !string.Equals(promo.PackBuyProducto, productoCodigo, StringComparison.OrdinalIgnoreCase))
                        {
                            break;
                        }

                        var packCant = promo.PackBuyCant;
                        var packPrecioTotal = promo.PackPrecio > 0
                            ? promo.PackPrecio
                            : promo.PrecioFijo;

                        if (packCant <= 0 || packPrecioTotal <= 0)
                            break;

                        if (cantidad < packCant)
                            break;

                        var packs = (int)Math.Floor(cantidad / packCant);
                        if (packs <= 0)
                            break;

                        var cantidadPack = packs * packCant;

                        var subtotalNormalPacks = cantidadPack * precioUnit;
                        var totalNormalPacks = subtotalNormalPacks * (1 + ITBIS_POR_DEFECTO / 100m);

                        var totalPromoPacks = packs * packPrecioTotal;

                        var descTotalConItbis = totalNormalPacks - totalPromoPacks;
                        if (descTotalConItbis <= 0)
                            break;

                        var descBase = descTotalConItbis / (1 + ITBIS_POR_DEFECTO / 100m);
                        if (descBase > subtotalBruto)
                            descBase = subtotalBruto;

                        montoDesc = Math.Round(descBase, 2);
                        break;
                    }
            }

            promo.MontoDescuentoCalculado = montoDesc;
            promo.PrecioUnitFinal = precioUnit;

            return promo;
        }

        /// <summary>
        /// Aplica TODAS las promos al carrito completo:
        /// 1) Combos PACK_PRODUCTO de varios productos (Cant.pack = 1).
        /// 2) Promos individuales por línea.
        /// Devuelve la lista de promos aplicadas para poder registrarlas.
        /// </summary>
        public List<(ItemCarrito Item, PromoAplicadaResult Promo)> AplicarPromosCarrito(
            List<ItemCarrito> carrito,
            string? clienteCodigo)
        {
            var resultado = new List<(ItemCarrito, PromoAplicadaResult)>();

            if (carrito == null || carrito.Count == 0)
                return resultado;

            // Limpiamos descuentos previos
            foreach (var it in carrito)
            {
                it.DescuentoMonto = 0m;
                it.DescuentoPct = 0m;
            }

            // 1) Combos multi-producto con PACK_PRODUCTO y Cant.pack = 1
            var combos = CalcularCombosPackProducto(carrito);
            foreach (var par in combos)
            {
                var it = par.Item;
                var promo = par.Promo;

                // Sumamos el descuento combo a la línea
                it.DescuentoMonto += par.Promo.MontoDescuentoCalculado;

                if (it.SubtotalBruto > 0)
                    it.DescuentoPct = Math.Round((it.DescuentoMonto * 100m) / it.SubtotalBruto, 2);

                resultado.Add((it, promo));
            }

            // 2) Promos por línea (evitando volver a tratar combos)
            foreach (var it in carrito)
            {
                var promo = CalcularMejorPromoLinea(
                    clienteCodigo,
                    it.ProductoCodigo,
                    null,
                    null,
                    it.Cantidad,
                    it.PrecioUnit);

                if (promo == null || promo.MontoDescuentoCalculado <= 0)
                    continue;

                // Si es PACK_PRODUCTO con Cant.pack = 1 ya lo tratamos como combo.
                if (promo.TipoRegla == "PACK_PRODUCTO" && promo.PackBuyCant <= 1)
                    continue;

                it.DescuentoMonto += promo.MontoDescuentoCalculado;

                if (promo.DescuentoPct > 0)
                {
                    it.DescuentoPct = promo.DescuentoPct;
                }
                else if (it.SubtotalBruto > 0)
                {
                    it.DescuentoPct = Math.Round((it.DescuentoMonto * 100m) / it.SubtotalBruto, 2);
                }

                resultado.Add((it, promo));
            }

            return resultado;
        }

        /// <summary>
        /// Calcula combos PACK_PRODUCTO donde:
        ///   - TipoRegla = PACK_PRODUCTO
        ///   - PackBuyCant = 1
        ///   - Misma PromoId para varios productos distintos
        ///   - El precio pack (Pack_Precio) es el precio final del combo (con ITBIS).
        /// </summary>
        private List<(ItemCarrito Item, PromoAplicadaResult Promo)> CalcularCombosPackProducto(
            List<ItemCarrito> carrito)
        {
            var resultado = new List<(ItemCarrito, PromoAplicadaResult)>();
            if (carrito == null || carrito.Count == 0)
                return resultado;

            // 1) Buscar la promo PACK_PRODUCTO / Cant=1 de cada línea
            var promosPorItem = new Dictionary<ItemCarrito, PromoAplicadaResult>();

            foreach (var it in carrito)
            {
                var promo = CalcularMejorPromoLinea(
                    clienteCodigo: null,
                    productoCodigo: it.ProductoCodigo,
                    categoriaId: null,
                    subcategoriaId: null,
                    cantidad: it.Cantidad,
                    precioUnit: it.PrecioUnit);

                if (promo == null)
                    continue;

                if (!string.Equals(promo.TipoRegla, "PACK_PRODUCTO", StringComparison.OrdinalIgnoreCase))
                    continue;

                if (promo.PackBuyCant != 1)
                    continue;

                if (promo.PackPrecio <= 0)
                    continue;

                promosPorItem[it] = promo;
            }

            if (promosPorItem.Count == 0)
                return resultado;

            const decimal ITBIS_POR_DEFECTO = 18m;

            // 2) Agrupar por PromoId (cada grupo = combo)
            foreach (var grupo in promosPorItem.GroupBy(x => x.Value.PromoId))
            {
                var itemsGrupo = grupo.Select(g => g.Key).ToList();
                var promoBase = grupo.First().Value;

                // Para que sea combo, al menos 2 productos distintos
                if (itemsGrupo.Count < 2)
                    continue;

                // Nº de combos: mínimo de cantidades disponibles de cada producto
                var combos = (int)itemsGrupo
                    .Select(it => (int)Math.Floor(it.Cantidad / promoBase.PackBuyCant))
                    .Min();

                if (combos <= 0)
                    continue;

                // Total normal (con ITBIS) para la parte combinable
                decimal totalNormalConItbis = 0m;
                decimal totalBaseCombos = 0m;
                var basePorItem = new Dictionary<ItemCarrito, decimal>();

                foreach (var it in itemsGrupo)
                {
                    var cantCombo = combos * promoBase.PackBuyCant; // aquí = combos * 1
                    var baseItem = cantCombo * it.PrecioUnit;       // sin ITBIS

                    basePorItem[it] = baseItem;
                    totalBaseCombos += baseItem;

                    totalNormalConItbis += baseItem * (1 + ITBIS_POR_DEFECTO / 100m);
                }

                if (totalBaseCombos <= 0)
                    continue;

                // Precio final del combo
                var totalPromoConItbis = combos * promoBase.PackPrecio;

                var descTotalConItbis = totalNormalConItbis - totalPromoConItbis;
                if (descTotalConItbis <= 0)
                    continue;

                var descBase = descTotalConItbis / (1 + ITBIS_POR_DEFECTO / 100m);

                // 3) Repartir el descuento entre las líneas del combo
                decimal acumulado = 0m;
                int index = 0;

                foreach (var it in itemsGrupo)
                {
                    index++;
                    decimal descItem;

                    if (index < itemsGrupo.Count)
                    {
                        var propor = basePorItem[it] / totalBaseCombos;
                        descItem = Math.Round(descBase * propor, 2);
                        acumulado += descItem;
                    }
                    else
                    {
                        // Último: ajustar para cuadrar con el total
                        descItem = Math.Round(descBase - acumulado, 2);
                    }

                    var promoItem = new PromoAplicadaResult
                    {
                        PromoId = promoBase.PromoId,
                        CodigoPromo = promoBase.CodigoPromo,
                        NombrePromo = promoBase.NombrePromo,
                        TipoPromo = promoBase.TipoPromo,
                        Prioridad = promoBase.Prioridad,
                        ReglaId = promoBase.ReglaId,
                        TipoRegla = promoBase.TipoRegla,
                        DescuentoPct = 0m,
                        DescuentoMonto = 0m,
                        PrecioFijo = promoBase.PrecioFijo,
                        MinCantidad = promoBase.MinCantidad,
                        PackBuyProducto = promoBase.PackBuyProducto,
                        PackBuyCant = promoBase.PackBuyCant,
                        PackPrecio = promoBase.PackPrecio,
                        MontoDescuentoCalculado = descItem,
                        PrecioUnitFinal = it.PrecioUnit
                    };

                    resultado.Add((it, promoItem));
                }
            }

            return resultado;
        }

        /// <summary>
        /// Registra el uso de una promo (cuando implementes PromoLog/PromoTope).
        /// </summary>
        public void RegistrarUsoPromoLinea(
            PromoAplicadaResult promo,
            long ventaId,
            string productoCodigo,
            decimal cantidad,
            string usuario)
        {
            if (promo == null || promo.MontoDescuentoCalculado <= 0)
                return;

            _repo.RegistrarUsoPromo(
                promo.PromoId,
                ventaId,
                productoCodigo,
                cantidad,
                promo.MontoDescuentoCalculado,
                usuario);
        }
    }
}
