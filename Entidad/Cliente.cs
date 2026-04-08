using System;

namespace Andloe.Entidad
{
    public class Cliente
    {
        // =========================
        // Básico / Comercial
        // =========================
        public string Codigo { get; set; } = null!;
        public string Nombre { get; set; } = null!;
        public string? RncCedula { get; set; }
        public string? Telefono { get; set; }
        public string? Email { get; set; }
        public string? Direccion { get; set; }
        public byte Tipo { get; set; }
        public byte Estado { get; set; }
        public DateTime? FechaCreacion { get; set; }

        // =========================
        // Comercial extendido
        // =========================
        public decimal? CreditoMaximo { get; set; }
        public string? CodDivisas { get; set; }
        public string? CodTerminoPagos { get; set; }
        public string? CodVendedor { get; set; }
        public string? CodAlmacen { get; set; }
        public int? ClienteId { get; set; }
        public decimal? DescuentoPctMax { get; set; }

        // =========================
        // Fiscal (DGII / e-CF)
        // =========================
        public string? RazonSocialFiscal { get; set; }
        public string? NombreComercialFiscal { get; set; }
        public byte? TipoIdentificacionFiscal { get; set; } // 1=RNC, 2=Cédula, 3=Extranjero
        public string? MunicipioCodigo { get; set; }
        public string? ProvinciaCodigo { get; set; }
        public string? PaisCodigo { get; set; }
        public string? CorreoFiscal { get; set; }

        public bool EsContribuyente { get; set; }
        public byte? TipoClienteFiscal { get; set; }

        public bool ValidadoDGII { get; set; }
        public DateTime? FechaValidacionDGII { get; set; }
        public string? EstadoRncDGII { get; set; }

        public string? IdentificadorExtranjero { get; set; }
        public bool EsExtranjero { get; set; }

        // =========================
        // Lógica de negocio
        // =========================

        /// <summary>
        /// Determina si el cliente está listo para facturación E31
        /// </summary>
        public bool AptoParaE31
        {
            get
            {
                if (EsExtranjero)
                    return false;

                if (Estado != 1)
                    return false;

                if (!ValidadoDGII)
                    return false;

                if (string.IsNullOrWhiteSpace(RncCedula))
                    return false;

                if (string.IsNullOrWhiteSpace(RazonSocialFiscal))
                    return false;

                if (TipoIdentificacionFiscal is null or 0)
                    return false;

                if (TipoIdentificacionFiscal == 3)
                    return false;

                return true;
            }
        }

        /// <summary>
        /// Nombre que se usará en XML (fallback seguro)
        /// </summary>
        public string NombreFiscalFinal
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(RazonSocialFiscal))
                    return RazonSocialFiscal!;

                return Nombre;
            }
        }

        /// <summary>
        /// Documento fiscal limpio
        /// </summary>
        public string? DocumentoFiscalLimpio
        {
            get
            {
                if (string.IsNullOrWhiteSpace(RncCedula))
                    return null;

                var texto = RncCedula.Trim();
                var limpio = "";

                foreach (var c in texto)
                {
                    if (char.IsDigit(c))
                        limpio += c;
                }

                return limpio;
            }
        }
    }
}