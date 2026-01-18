using Andloe.Data;
using System;
using System.Globalization;

namespace Andloe.Logica
{
    /// <summary>
    /// Servicio estático para leer configuraciones (clave/valor) de uso general.
    /// </summary>
    public static class ConfigService
    {
        private static readonly SistemaConfigRepository _repo = new();

        // =======================
        //      LECTURAS
        // =======================

        public static string ClienteDefecto =>
            _repo.GetValor("CLIENTE_DEFECTO") ?? "C-000001";

        public static string MonedaDefecto =>
            _repo.GetValor("MONEDA_DEFECTO") ?? "DOP";

        public static int MedioPagoDefectoId
        {
            get
            {
                var txt = _repo.GetValor("MEDIO_PAGO_DEFECTO");
                return int.TryParse(txt, out var id) ? id : 1;
            }
        }

        public static decimal TasaItbis =>
            _repo.GetNumero("TASA_ITBIS", 18m);

        /// <summary>Límite global de descuento (%). Si no existe, devuelve 0.</summary>
        public static decimal DescuentoMaxGlobal =>
            _repo.GetNumero("DESCUENTO_MAX_GLOBAL", 0m);

        /// <summary>Indica si el sistema permite aplicar descuentos en el POS.</summary>
        public static bool DescuentoHabilitado
        {
            get
            {
                var v = _repo.GetValor("DESCUENTO_HABILITADO");
                if (string.IsNullOrWhiteSpace(v)) return true; // por defecto: sí
                return v.Equals("true", StringComparison.OrdinalIgnoreCase) || v == "1";
            }
        }

        /// <summary>
        /// Permitir vender con stock negativo (1 = sí, 0 = no).
        /// </summary>
        public static bool PermitirStockNegativo
        {
            get
            {
                var v = _repo.GetValor("PERMITIR_STOCK_NEGATIVO");
                if (string.IsNullOrWhiteSpace(v)) return false;
                return v == "1"
                    || v.Equals("true", StringComparison.OrdinalIgnoreCase)
                    || v.Equals("si", StringComparison.OrdinalIgnoreCase);
            }
        }

        /// <summary>Almacén de ORIGEN por defecto para POS.</summary>
        public static int AlmacenPosOrigenId
        {
            get
            {
                var txt = _repo.GetValor("ALMACEN_POS_ORIGEN_ID");
                return int.TryParse(txt, out var id) ? id : 0;
            }
        }

        /// <summary>Almacén de DESTINO por defecto para POS (0 = ninguno).</summary>
        public static int AlmacenPosDestinoId
        {
            get
            {
                var txt = _repo.GetValor("ALMACEN_POS_DESTINO_ID");
                return int.TryParse(txt, out var id) ? id : 0;
            }
        }

        // =======================
        //      ESCRITURAS
        // =======================

        public static void SetClienteDefecto(string codigo) =>
            _repo.SetValor("CLIENTE_DEFECTO", codigo,
                "Cliente por defecto", "POS", "admin");

        public static void SetMonedaDefecto(string moneda) =>
            _repo.SetValor("MONEDA_DEFECTO", moneda,
                "Moneda por defecto", "POS", "admin");

        public static void SetMedioPagoDefecto(int medioId) =>
            _repo.SetValor("MEDIO_PAGO_DEFECTO", medioId.ToString(),
                "Medio de pago por defecto", "POS", "admin");

        public static void SetTasaItbis(decimal itbis) =>
            _repo.SetValor("TASA_ITBIS", itbis.ToString("0.##", CultureInfo.InvariantCulture),
                "Tasa ITBIS por defecto", "GENERAL", "admin");

        public static void SetDescuentoMaxGlobal(decimal porciento) =>
            _repo.SetValor("DESCUENTO_MAX_GLOBAL", porciento.ToString("0.##"),
                "Descuento máximo total permitido en %", "POS", "admin");

        public static void SetDescuentoHabilitado(bool habilitado) =>
            _repo.SetValor("DESCUENTO_HABILITADO", habilitado ? "true" : "false",
                "Habilitar o no descuentos en POS", "POS", "admin");
    }

    /// <summary>
    /// Servicio instanciable para leer configuraciones numéricas / booleanas,
    /// usado dentro de servicios como PosService.
    /// </summary>
    public class ConfigCalcService
    {
        private readonly SistemaConfigRepository _repo;

        public ConfigCalcService(SistemaConfigRepository repo)
        {
            _repo = repo;
        }

        public decimal GetDecimal(string clave, decimal defaultValue = 0m)
        {
            var valor = _repo.GetValor(clave);
            if (decimal.TryParse(valor, NumberStyles.Any, CultureInfo.InvariantCulture, out var dec))
                return dec;

            return defaultValue;
        }

        public bool GetBool(string clave, bool defaultValue = false)
        {
            var valor = _repo.GetValor(clave);
            if (string.IsNullOrWhiteSpace(valor))
                return defaultValue;

            return valor.Equals("true", StringComparison.OrdinalIgnoreCase)
                || valor.Equals("1");
        }
    }
}
