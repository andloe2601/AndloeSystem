using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace Andloe.Logica.Reportes
{
    public static class RdlcReportDataBuilder
    {
        private static readonly ConcurrentDictionary<Type, PropertyInfo[]> PropertyCache = new ConcurrentDictionary<Type, PropertyInfo[]>();

        public static DataTable BuildTable<T>(string tableName, T data)
        {
            if (string.IsNullOrWhiteSpace(tableName))
                throw new ArgumentException("El nombre de la tabla no puede ser nulo o vacío.", nameof(tableName));

            if (data == null)
                throw new ArgumentNullException(nameof(data));

            var dt = new DataTable(tableName);
            dt.Locale = System.Globalization.CultureInfo.InvariantCulture;

            var props = GetCachedProperties<T>();

            foreach (var p in props)
            {
                var t = Nullable.GetUnderlyingType(p.PropertyType) ?? p.PropertyType;
                
                // Validar que el tipo sea compatible con DataTable
                if (!IsValidDataTableType(t))
                {
                    // Usar string como fallback para tipos complejos
                    dt.Columns.Add(p.Name, typeof(string));
                }
                else
                {
                    dt.Columns.Add(p.Name, t);
                }
            }

            var row = dt.NewRow();
            foreach (var p in props)
            {
                try
                {
                    var val = p.GetValue(data, null);
                    
                    if (val == null)
                    {
                        row[p.Name] = DBNull.Value;
                    }
                    else if (!IsValidDataTableType(val.GetType()))
                    {
                        // Convertir tipos complejos a string
                        row[p.Name] = val.ToString();
                    }
                    else
                    {
                        row[p.Name] = val;
                    }
                }
                catch (TargetInvocationException)
                {
                    // Si falla la obtención del valor, asignar DBNull
                    row[p.Name] = DBNull.Value;
                }
            }
            dt.Rows.Add(row);

            return dt;
        }

        public static DataTable BuildTableList<T>(string tableName, IEnumerable<T> list)
        {
            if (string.IsNullOrWhiteSpace(tableName))
                throw new ArgumentException("El nombre de la tabla no puede ser nulo o vacío.", nameof(tableName));

            if (list == null)
                throw new ArgumentNullException(nameof(list));

            var dt = new DataTable(tableName);
            dt.Locale = System.Globalization.CultureInfo.InvariantCulture;

            var props = GetCachedProperties<T>();

            foreach (var p in props)
            {
                var t = Nullable.GetUnderlyingType(p.PropertyType) ?? p.PropertyType;
                
                if (!IsValidDataTableType(t))
                {
                    dt.Columns.Add(p.Name, typeof(string));
                }
                else
                {
                    dt.Columns.Add(p.Name, t);
                }
            }

            foreach (var item in list)
            {
                if (item == null) continue; // Saltar elementos nulos en la lista

                var row = dt.NewRow();
                foreach (var p in props)
                {
                    try
                    {
                        var val = p.GetValue(item, null);
                        
                        if (val == null)
                        {
                            row[p.Name] = DBNull.Value;
                        }
                        else if (!IsValidDataTableType(val.GetType()))
                        {
                            row[p.Name] = val.ToString();
                        }
                        else
                        {
                            row[p.Name] = val;
                        }
                    }
                    catch (TargetInvocationException)
                    {
                        row[p.Name] = DBNull.Value;
                    }
                }
                dt.Rows.Add(row);
            }

            return dt;
        }

        private static PropertyInfo[] GetCachedProperties<T>()
        {
            return PropertyCache.GetOrAdd(typeof(T), type =>
                type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(p => p.CanRead && p.GetIndexParameters().Length == 0) // Excluir propiedades indexadas
                    .ToArray()
            );
        }

        private static bool IsValidDataTableType(Type type)
        {
            // DataTable soporta tipos primitivos, string, DateTime, Decimal, Guid, TimeSpan, y byte[]
            return type.IsPrimitive
                || type == typeof(string)
                || type == typeof(DateTime)
                || type == typeof(decimal)
                || type == typeof(Guid)
                || type == typeof(TimeSpan)
                || type == typeof(byte[])
                || type == typeof(DateTimeOffset)
                || type.IsEnum;
        }

        /// <summary>
        /// Limpia la caché de propiedades. Útil para pruebas o escenarios de carga dinámica de tipos.
        /// </summary>
        public static void ClearCache()
        {
            PropertyCache.Clear();
        }
    }
}
