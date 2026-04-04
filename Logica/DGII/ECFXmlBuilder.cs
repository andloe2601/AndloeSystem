using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Security;
using System.Text;

namespace Andloe.Logica.DGII
{
    /// <summary>
    /// Builder XML e-CF (base) SIN depender del tipo DTO (evita conflictos de ensamblado y tipos ambiguos).
    /// Soporta DTOs que tengan:
    ///  dto.Cab.{FacturaId, NumeroDocumento, FechaDocumento, NombreCliente, DocumentoCliente, DireccionCliente,
    ///           SubTotal, TotalDescuento, TotalImpuesto, TotalGeneral}
    ///  dto.Det = IEnumerable con items {ProductoCodigo, CodBarra, Descripcion, Unidad,
    ///                                  Cantidad, Precio, DescuentoPct, DescuentoMonto, ItbisPct, ItbisMonto, TotalLinea}
    /// </summary>
    public sealed class ECFXmlBuilder
    {
        public string BuildXml(object dto, int tipoEcf, string encf)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            if (tipoEcf <= 0) throw new ArgumentException("TipoECF inválido.", nameof(tipoEcf));

            encf = (encf ?? "").Trim();
            if (string.IsNullOrWhiteSpace(encf)) throw new ArgumentException("ENCF inválido.", nameof(encf));

            // ========= CAB =========
            var cab = GetProp(dto, "Cab");
            if (cab == null) throw new InvalidOperationException("dto.Cab no existe o es null.");

            var facturaId = GetInt(cab, "FacturaId");
            var numero = Nz(GetString(cab, "NumeroDocumento"), "(sin-numero)");
            var fecha = GetDate(cab, "FechaDocumento") ?? DateTime.Today;

            var clienteNombre = Nz(GetString(cab, "NombreCliente"), "CONSUMIDOR FINAL");
            var clienteDoc = SoloDigitos(GetString(cab, "DocumentoCliente"));
            var clienteDir = Nz(GetString(cab, "DireccionCliente"), "");

            var subTotal = GetDec(cab, "SubTotal");
            var descuento = GetDec(cab, "TotalDescuento");
            var itbis = GetDec(cab, "TotalImpuesto");
            var total = GetDec(cab, "TotalGeneral");

            // ========= DET =========
            var detObj = GetProp(dto, "Det");
            var det = ToEnumerable(detObj);

            // Backup: si total viene 0, recalcula con detalle
            if (total <= 0m && det.Any())
            {
                var calc = CalcularTotales(det);
                subTotal = calc.subTotal;
                descuento = calc.desc;
                itbis = calc.itbis;
                total = calc.total;
            }

            // ========= XML =========
            var sb = new StringBuilder(64 * 1024);

            sb.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            sb.Append("<ECFDocumento>");
            sb.Append("<Encabezado>");

            sb.Append("<TipoECF>").Append(tipoEcf).Append("</TipoECF>");
            sb.Append("<ENCF>").Append(X(encf)).Append("</ENCF>");

            sb.Append("<FacturaId>").Append(facturaId).Append("</FacturaId>");
            sb.Append("<NumeroDocumento>").Append(X(numero)).Append("</NumeroDocumento>");
            sb.Append("<FechaDocumento>").Append(fecha.ToString("yyyy-MM-dd")).Append("</FechaDocumento>");

            sb.Append("<Comprador>");
            sb.Append("<Nombre>").Append(X(clienteNombre)).Append("</Nombre>");
            sb.Append("<Documento>").Append(X(clienteDoc)).Append("</Documento>");
            sb.Append("<Direccion>").Append(X(clienteDir)).Append("</Direccion>");
            sb.Append("</Comprador>");

            sb.Append("<Totales>");
            sb.Append("<SubTotal>").Append(F2(subTotal)).Append("</SubTotal>");
            sb.Append("<Descuento>").Append(F2(descuento)).Append("</Descuento>");
            sb.Append("<ITBIS>").Append(F2(itbis)).Append("</ITBIS>");
            sb.Append("<Total>").Append(F2(total)).Append("</Total>");
            sb.Append("</Totales>");

            sb.Append("</Encabezado>");

            sb.Append("<Detalle>");
            int n = 0;
            foreach (var d in det)
            {
                n++;
                sb.Append("<Item>");
                sb.Append("<Linea>").Append(n).Append("</Linea>");

                sb.Append("<Codigo>").Append(X(Nz(GetString(d, "ProductoCodigo"), ""))).Append("</Codigo>");
                sb.Append("<CodBarra>").Append(X(Nz(GetString(d, "CodBarra"), ""))).Append("</CodBarra>");
                sb.Append("<Descripcion>").Append(X(Nz(GetString(d, "Descripcion"), ""))).Append("</Descripcion>");
                sb.Append("<Unidad>").Append(X(Nz(GetString(d, "Unidad"), "UND"))).Append("</Unidad>");

                sb.Append("<Cantidad>").Append(F4(GetDec(d, "Cantidad"))).Append("</Cantidad>");
                sb.Append("<Precio>").Append(F4(GetDec(d, "Precio"))).Append("</Precio>");

                sb.Append("<DescuentoPct>").Append(F4(GetDec(d, "DescuentoPct"))).Append("</DescuentoPct>");
                sb.Append("<DescuentoMonto>").Append(F2(GetDec(d, "DescuentoMonto"))).Append("</DescuentoMonto>");

                sb.Append("<ITBISPct>").Append(F4(GetDec(d, "ItbisPct"))).Append("</ITBISPct>");
                sb.Append("<ITBISMonto>").Append(F2(GetDec(d, "ItbisMonto"))).Append("</ITBISMonto>");

                sb.Append("<TotalLinea>").Append(F2(GetDec(d, "TotalLinea"))).Append("</TotalLinea>");
                sb.Append("</Item>");
            }
            sb.Append("</Detalle>");

            sb.Append("</ECFDocumento>");

            return sb.ToString();
        }

        // ============================================================
        // Helpers reflexión + conversiones
        // ============================================================

        private static object? GetProp(object obj, string name)
        {
            if (obj == null) return null;
            var t = obj.GetType();
            var p = t.GetProperty(name);
            return p?.GetValue(obj);
        }

        private static string? GetString(object obj, string name)
        {
            var v = GetProp(obj, name);
            if (v == null) return null;
            return Convert.ToString(v);
        }

        private static int GetInt(object obj, string name)
        {
            var v = GetProp(obj, name);
            if (v == null || v == DBNull.Value) return 0;
            try { return Convert.ToInt32(v, CultureInfo.InvariantCulture); }
            catch
            {
                var s = Convert.ToString(v) ?? "0";
                return int.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out var i) ? i : 0;
            }
        }

        private static decimal GetDec(object obj, string name)
        {
            var v = GetProp(obj, name);
            if (v == null || v == DBNull.Value) return 0m;

            try { return Convert.ToDecimal(v, CultureInfo.InvariantCulture); }
            catch
            {
                var s = Convert.ToString(v) ?? "0";
                if (decimal.TryParse(s, NumberStyles.Any, CultureInfo.CurrentCulture, out var d)) return d;
                if (decimal.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out d)) return d;
                return 0m;
            }
        }

        private static DateTime? GetDate(object obj, string name)
        {
            var v = GetProp(obj, name);
            if (v == null || v == DBNull.Value) return null;
            if (v is DateTime dt) return dt;

            var s = Convert.ToString(v);
            if (DateTime.TryParse(s, CultureInfo.CurrentCulture, DateTimeStyles.None, out var d1)) return d1;
            if (DateTime.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.None, out var d2)) return d2;
            return null;
        }

        private static object[] ToEnumerable(object? detObj)
        {
            if (detObj == null) return Array.Empty<object>();
            if (detObj is IEnumerable en)
                return en.Cast<object>().Where(x => x != null).ToArray();
            return Array.Empty<object>();
        }

        private static (decimal subTotal, decimal desc, decimal itbis, decimal total) CalcularTotales(object[] det)
        {
            decimal st = 0m, ds = 0m, it = 0m, tt = 0m;

            foreach (var d in det)
            {
                var cant = GetDec(d, "Cantidad");
                var precio = GetDec(d, "Precio");
                var descMonto = GetDec(d, "DescuentoMonto");
                var itbisMonto = GetDec(d, "ItbisMonto");

                var baseLinea = cant * precio;
                st += baseLinea;
                ds += descMonto;

                var neta = baseLinea - descMonto;
                if (neta < 0m) neta = 0m;

                it += itbisMonto;
                tt += (neta + itbisMonto);
            }

            return (st, ds, it, tt);
        }

        private static string Nz(string? s, string fallback) => string.IsNullOrWhiteSpace(s) ? fallback : s.Trim();

        private static string SoloDigitos(string? s)
        {
            if (string.IsNullOrWhiteSpace(s)) return "";
            return new string(s.Where(char.IsDigit).ToArray());
        }

        private static string F2(decimal v) => v.ToString("0.00", CultureInfo.InvariantCulture);
        private static string F4(decimal v) => v.ToString("0.####", CultureInfo.InvariantCulture);

        private static string X(string? s)
        {
            s ??= "";
            return SecurityElement.Escape(s) ?? "";
        }
    }
}