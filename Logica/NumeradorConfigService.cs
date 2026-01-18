using System;
using Andloe.Data;

namespace Andloe.Logica
{
    /// <summary>
    /// Servicio para generar códigos usando NumeradorRepository
    /// pero tomando prefijo y longitud desde SistemaConfig.
    /// </summary>
    public static class NumeradorConfigService
    {
        private static readonly SistemaConfigRepository _cfgRepo = new();
        private static readonly NumeradorRepository _numRepo = new();
      

        /// <summary>
        /// Lee prefijo y longitud desde SistemaConfig.
        /// Claves usadas:
        ///   {baseClave}_PREFIJO
        ///   {baseClave}_LONGITUD
        /// </summary>
        private static (string Prefijo, int Longitud) GetFormato(
            string baseClave,
            string prefijoDefault,
            int longitudDefault)
        {
            // Prefijo
            var prefijo = _cfgRepo.GetValor(baseClave + "_PREFIJO") ?? prefijoDefault;
            if (string.IsNullOrWhiteSpace(prefijo))
                prefijo = prefijoDefault;

            // Longitud
            var txtLen = _cfgRepo.GetValor(baseClave + "_LONGITUD");
            int longitud;
            if (!int.TryParse(txtLen, out longitud) || longitud <= 0 || longitud > 20)
                longitud = longitudDefault;

            return (prefijo, longitud);
        }

        // =======================
        //   NUMERADORES PÚBLICOS
        // =======================

        /// <summary>Próximo número de FACTURA DE VENTA.</summary>
        public static string NextFacturaVenta()
        {
            // En tu sistema la cabecera de venta usa tipo "VC"
            var (pref, len) = GetFormato("NUM_FACTVENTA", "V", 8);
            return _numRepo.Next("VC", pref, len);
        }

        /// <summary>Próximo código de PRODUCTO.</summary>
        public static string NextProducto()
        {
            var (pref, len) = GetFormato("NUM_PRODUCTO", "P-", 6);
            return _numRepo.Next("PROD", pref, len);
        }

        /// <summary>Próximo código de CLIENTE.</summary>
        public static string NextCliente()
        {
            var (pref, len) = GetFormato("NUM_CLIENTE", "C-", 6);
            return _numRepo.Next("CLI", pref, len);
        }

        /// <summary>Próximo código de PROVEEDOR.</summary>
        public static string NextProveedor()
        {
            var (pref, len) = GetFormato("NUM_PROVEEDOR", "PR-", 6);
            return _numRepo.Next("PROV", pref, len);
        }

        /// <summary>Próximo número de FACTURA DE COMPRA.</summary>
        public static string NextFacturaCompra()
        {
            var (pref, len) = GetFormato("NUM_FACTCOMPRA", "FC-", 8);
            return _numRepo.Next("CC", pref, len);
        }
    
    // public static string NextPromoProductoDescuento()
     //   {
            // Debe coincidir con NumeradorSecuencia.Codigo
           // return _numRepo.GetNext("PROMOPROD");
        }

    }
//}
 
 

