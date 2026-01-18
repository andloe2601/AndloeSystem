#nullable enable
using System;
using System.Data;
using Microsoft.Extensions.Logging;
using Andloe.Data;

namespace Andloe.Logica.Facturacion
{
    public sealed class FacturaReportService
    {
        private readonly ReporteService _reporteService;
        private readonly ReporteFacturacionRepository _reporteFacturacionRepo;
        private readonly IPrintPreviewService _preview;
        private readonly ILogger<FacturaReportService> _logger;

        public FacturaReportService(
            ReporteService reporteService,
            ReporteFacturacionRepository reporteFacturacionRepo,
            IPrintPreviewService preview,
            ILogger<FacturaReportService> logger)
        {
            _reporteService = reporteService ?? throw new ArgumentNullException(nameof(reporteService));
            _reporteFacturacionRepo = reporteFacturacionRepo ?? throw new ArgumentNullException(nameof(reporteFacturacionRepo));
            _preview = preview ?? throw new ArgumentNullException(nameof(preview));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void ImprimirFactura(string modulo, string codigoReporte, int facturaId)
        {
            if (string.IsNullOrWhiteSpace(modulo)) throw new ArgumentException("módulo requerido", nameof(modulo));
            if (facturaId <= 0) throw new ArgumentException("facturaId inválido", nameof(facturaId));

            var mod = modulo.Trim();

            // ✅ si viene vacío o viene "FAC/COT/PF", lo resolvemos por TipoDocumento desde DB
            var cod = ResolveCodigoReporte(codigoReporte, facturaId);

            // 1) Resolver reporte
            var rep = _reporteService.ObtenerReporteParaImprimir(mod, cod);
            if (rep == null)
                throw new InvalidOperationException($"No se encontró definición de reporte: Modulo={mod}, Codigo={cod}.");

            _logger.LogInformation("ImprimirFactura => Modulo={Modulo}, Codigo={Codigo}, Motor={Motor}, Ruta={Ruta}",
                mod, cod, rep.Motor, rep.RutaArchivo);

            if (!string.Equals(rep.Motor, "RDLC", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException($"Motor no soportado: {rep.Motor}. Este servicio solo imprime RDLC.");

            // 2) Dataset (siempre con @FacturaId)
            var ds = ObtenerDatasetReporte(cod, facturaId);

            // 3) Validar / normalizar (Cab/Det/Totales + 1 fila en Totales)
            NormalizarDataset(ds);

            // 4) Debug útil
            _logger.LogInformation("RDLC => CabRows={CabRows} DetRows={DetRows} TotRows={TotRows}",
                ds.Tables["Cab"]!.Rows.Count,
                ds.Tables["Det"]!.Rows.Count,
                ds.Tables["Totales"]!.Rows.Count);

            // 5) Preview
            _preview.Preview(
                rep.RutaArchivo,
                ds.Tables["Cab"]!,
                ds.Tables["Det"]!,
                ds.Tables["Totales"]!,
                string.IsNullOrWhiteSpace(rep.Nombre) ? "Imprimir" : rep.Nombre
            );
        }

        // =========================================================
        // ✅ RESOLVER CÓDIGO (FACTURA_RI / COTIZACION_RI / PROFORMA_RI)
        // =========================================================
        private string ResolveCodigoReporte(string? codigoReporte, int facturaId)
        {
            var cod = (codigoReporte ?? "").Trim().ToUpperInvariant();

            // Si ya viene en el formato correcto, no inventamos nada
            if (cod is "FACTURA_RI" or "COTIZACION_RI" or "PROFORMA_RI")
                return cod;

            // Soporta llaves cortas: FAC/COT/PF
            if (cod is "FAC") return "FACTURA_RI";
            if (cod is "COT") return "COTIZACION_RI";
            if (cod is "PF") return "PROFORMA_RI";

            // Si no vino nada (o vino raro), lo resolvemos leyendo TipoDocumento del registro
            if (string.IsNullOrWhiteSpace(cod))
            {
                var tipo = GetTipoDocumentoFromFacturaId(facturaId);

                return tipo switch
                {
                    "FAC" => "FACTURA_RI",
                    "COT" => "COTIZACION_RI",
                    "PF" => "PROFORMA_RI",
                    _ => "FACTURA_RI" // fallback seguro
                };
            }

            // Si vino algo no esperado, lo dejamos caer con error claro
            throw new InvalidOperationException($"Código de reporte no soportado: '{cod}'. Usa FACTURA_RI / COTIZACION_RI / PROFORMA_RI o FAC/COT/PF.");
        }

        // =========================================================
        // ✅ Obtiene TipoDocumento (FAC/COT/PF) usando FacturaId
        // =========================================================
        private string GetTipoDocumentoFromFacturaId(int facturaId)
        {
            // Usamos el SP de factura (ya existe) para leer Cab y sacar TipoDocumento sin crear SQL nuevo.
            var ds = _reporteFacturacionRepo.GetFacturaRI(facturaId);

            if (ds.Tables.Contains("Cab") && ds.Tables["Cab"] != null && ds.Tables["Cab"]!.Rows.Count > 0)
            {
                var row = ds.Tables["Cab"]!.Rows[0];

                // Nombre de columna según tu SP: f.TipoDocumento
                var tipoObj = row.Table.Columns.Contains("TipoDocumento") ? row["TipoDocumento"] : null;

                var tipo = (tipoObj == null || tipoObj == DBNull.Value)
                    ? ""
                    : Convert.ToString(tipoObj)?.Trim().ToUpperInvariant() ?? "";

                return tipo;
            }

            return "";
        }

        // =========================================================
        // DATASET BUILDER
        // =========================================================
        private DataSet ObtenerDatasetReporte(string codigoReporte, int facturaId)
        {
            var cod = (codigoReporte ?? "").Trim().ToUpperInvariant();

            return cod switch
            {
                // ✅ YA FUNCIONA → NO SE TOCA
                "FACTURA_RI" => _reporteFacturacionRepo.GetFacturaRI(facturaId),

                // ✅ NUEVOS (mismo FacturaId / mismo parámetro @FacturaId)
                "COTIZACION_RI" => _reporteFacturacionRepo.GetCotizacionRI(facturaId),
                "PROFORMA_RI" => _reporteFacturacionRepo.GetProformaRI(facturaId),

                _ => throw new InvalidOperationException($"No existe DataSet builder para el reporte: {cod}")
            };
        }

        // =========================================================
        // NORMALIZACIÓN BASE
        // =========================================================
        private static void NormalizarDataset(DataSet ds)
        {
            if (ds == null) throw new InvalidOperationException("El repositorio devolvió DataSet null.");

            if (!ds.Tables.Contains("Cab") || ds.Tables["Cab"] == null || ds.Tables["Cab"]!.Rows.Count == 0)
                throw new InvalidOperationException("El DataSet no devolvió Cab (o vino vacío).");

            if (!ds.Tables.Contains("Det") || ds.Tables["Det"] == null)
                ds.Tables.Add(new DataTable("Det"));

            if (!ds.Tables.Contains("Totales") || ds.Tables["Totales"] == null)
                ds.Tables.Add(CreateTotalesFallback());

            // ✅ fuerza nombres correctos
            ds.Tables["Cab"]!.TableName = "Cab";
            ds.Tables["Det"]!.TableName = "Det";
            ds.Tables["Totales"]!.TableName = "Totales";

            EnsureTotalesHasAtLeastOneRow(ds.Tables["Totales"]!);
        }

        private static DataTable CreateTotalesFallback()
        {
            var t = new DataTable("Totales");
            t.Columns.Add("TotalCantidad", typeof(decimal));
            t.Columns.Add("Bruto", typeof(decimal));
            t.Columns.Add("Descuento", typeof(decimal));
            t.Columns.Add("ITBIS", typeof(decimal));
            t.Columns.Add("Impuesto", typeof(decimal));
            t.Columns.Add("TotalLineas", typeof(decimal));
            t.Rows.Add(0m, 0m, 0m, 0m, 0m, 0m);
            return t;
        }

        private static void EnsureTotalesHasAtLeastOneRow(DataTable totales)
        {
            if (totales.TableName != "Totales") totales.TableName = "Totales";

            EnsureDecimalColumn(totales, "TotalCantidad");
            EnsureDecimalColumn(totales, "Bruto");
            EnsureDecimalColumn(totales, "Descuento");
            EnsureDecimalColumn(totales, "ITBIS");
            EnsureDecimalColumn(totales, "Impuesto");
            EnsureDecimalColumn(totales, "TotalLineas");

            if (totales.Rows.Count == 0)
                totales.Rows.Add(0m, 0m, 0m, 0m, 0m, 0m);
        }

        private static void EnsureDecimalColumn(DataTable t, string name)
        {
            if (!t.Columns.Contains(name))
                t.Columns.Add(name, typeof(decimal));
        }
    }
}
#nullable restore
