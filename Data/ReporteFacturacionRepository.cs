#nullable enable
using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Linq;

namespace Andloe.Data
{
    public sealed class ReporteFacturacionRepository
    {
        public DataSet GetFacturaRI(int facturaId)
        {
            if (facturaId <= 0) throw new ArgumentException("facturaId inválido.", nameof(facturaId));

            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand("dbo.Reporte_FacturaRI_Get", cn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.Add(new SqlParameter("@FacturaId", SqlDbType.Int) { Value = facturaId });

            using var da = new SqlDataAdapter(cmd);

            var ds = new DataSet("FacturaRIDataSet");
            da.Fill(ds);

            NormalizeTablesBySignature(ds);

            EnsureCommonTables(ds, facturaId, "Reporte_FacturaRI_Get");
            return ds;
        }

        public DataSet GetCotizacionRI(int facturaId)
        {
            if (facturaId <= 0) throw new ArgumentException("facturaId inválido.", nameof(facturaId));

            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand("dbo.Reporte_CotizacionRI_Get", cn) { CommandType = CommandType.StoredProcedure };

            // SP pide @FacturaId (como te salió en el error)
            cmd.Parameters.Add(new SqlParameter("@FacturaId", SqlDbType.Int) { Value = facturaId });

            using var da = new SqlDataAdapter(cmd);

            // Forzar nombres consistentes
            da.TableMappings.Add("Table", "Cab");
            da.TableMappings.Add("Table1", "Det");
            da.TableMappings.Add("Table2", "Totales");

            var ds = new DataSet("CotizacionRIDataSet");
            da.Fill(ds);

            NormalizeTablesBySignature(ds);
            EnsureCommonTables(ds, facturaId, "Reporte_CotizacionRI_Get");
            return ds;
        }

        public DataSet GetProformaRI(int facturaId)
        {
            if (facturaId <= 0) throw new ArgumentException("facturaId inválido.", nameof(facturaId));

            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand("dbo.Reporte_ProformaRI_Get", cn) { CommandType = CommandType.StoredProcedure };

            // Si tu SP también usa FacturaId, dejamos igual.
            // Si de verdad usa @ProformaId, cámbialo en el SP o aquí, pero tú pediste mantener FacturaId.
            cmd.Parameters.Add(new SqlParameter("@FacturaId", SqlDbType.Int) { Value = facturaId });

            using var da = new SqlDataAdapter(cmd);

            da.TableMappings.Add("Table", "Cab");
            da.TableMappings.Add("Table1", "Det");
            da.TableMappings.Add("Table2", "Totales");

            var ds = new DataSet("ProformaRIDataSet");
            da.Fill(ds);

            NormalizeTablesBySignature(ds);
            EnsureCommonTables(ds, facturaId, "Reporte_ProformaRI_Get");
            return ds;
        }

        private static void EnsureCommonTables(DataSet ds, int id, string spName)
        {
            if (!ds.Tables.Contains("Cab") || ds.Tables["Cab"] == null || ds.Tables["Cab"]!.Rows.Count == 0)
                throw new DataException($"No hay cabecera para Id={id} ({spName}).");

            if (!ds.Tables.Contains("Det") || ds.Tables["Det"] == null)
                ds.Tables.Add(new DataTable("Det"));

            if (!ds.Tables.Contains("Totales") || ds.Tables["Totales"] == null)
                ds.Tables.Add(CreateTotalesFallback());

            EnsureTotalesHasAtLeastOneRow(ds.Tables["Totales"]!);

            // Asegurar nombres
            ds.Tables["Cab"]!.TableName = "Cab";
            ds.Tables["Det"]!.TableName = "Det";
            ds.Tables["Totales"]!.TableName = "Totales";
        }

        private static void NormalizeTablesBySignature(DataSet ds)
        {
            if (ds.Tables.Count == 0) return;

            if (ds.Tables.Contains("Cab") && ds.Tables.Contains("Det") && ds.Tables.Contains("Totales"))
                return;

            DataTable? cab = null;
            DataTable? det = null;
            DataTable? tot = null;

            foreach (DataTable t in ds.Tables)
            {
                var cols = t.Columns.Cast<DataColumn>()
                    .Select(c => c.ColumnName)
                    .ToHashSet(StringComparer.OrdinalIgnoreCase);

                if (det == null && (cols.Contains("ProductoCodigo") || cols.Contains("Descripcion")) && cols.Contains("Cantidad"))
                    det = t;

                if (tot == null && (cols.Contains("Bruto") || cols.Contains("Descuento") || cols.Contains("ITBIS") || cols.Contains("TotalLineas"))
                    && !cols.Contains("ProductoCodigo") && !cols.Contains("Descripcion"))
                    tot = t;

                if (cab == null && (cols.Contains("FacturaId") || cols.Contains("NumeroDocumento") || cols.Contains("FechaDocumento") || cols.Contains("NombreCliente"))
                    && !cols.Contains("ProductoCodigo"))
                    cab = t;
            }

            if (cab != null) cab.TableName = "Cab";
            if (det != null) det.TableName = "Det";
            if (tot != null) tot.TableName = "Totales";
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
