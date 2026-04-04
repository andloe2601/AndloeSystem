using System;
using System.Collections.Generic;
using System.Data;
using Andloe.Entidad.CxC;
using Microsoft.Data.SqlClient;

namespace Andloe.Data
{
    public sealed class CxCCobroRepository
    {
        public List<CxCCobroListadoDto> Listar(string? filtro = null, int top = 200)
        {
            var list = new List<CxCCobroListadoDto>();

            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
SELECT TOP(@top)
    CobroId,
    NoRecibo,
    ClienteNombre,
    Detalles,
    Fecha,
    CuentaMostrar,
    Estado,
    Monto
FROM dbo.vw_CxCCobroListado
WHERE (@filtro IS NULL OR NoRecibo LIKE @like OR ClienteNombre LIKE @like OR Detalles LIKE @like)
ORDER BY Fecha DESC, CobroId DESC;", cn);

            cmd.Parameters.Add("@top", SqlDbType.Int).Value = top;

            if (string.IsNullOrWhiteSpace(filtro))
            {
                cmd.Parameters.Add("@filtro", SqlDbType.NVarChar, 100).Value = DBNull.Value;
                cmd.Parameters.Add("@like", SqlDbType.NVarChar, 120).Value = DBNull.Value;
            }
            else
            {
                var value = filtro.Trim();
                cmd.Parameters.Add("@filtro", SqlDbType.NVarChar, 100).Value = value;
                cmd.Parameters.Add("@like", SqlDbType.NVarChar, 120).Value = "%" + value + "%";
            }

            using var rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                list.Add(new CxCCobroListadoDto
                {
                    CobroId = rd.GetInt64(0),
                    NoRecibo = rd.IsDBNull(1) ? "" : rd.GetString(1),
                    ClienteNombre = rd.IsDBNull(2) ? "" : rd.GetString(2),
                    Detalles = rd.IsDBNull(3) ? "" : rd.GetString(3),
                    Fecha = rd.GetDateTime(4),
                    CuentaMostrar = rd.IsDBNull(5) ? "" : rd.GetString(5),
                    Estado = rd.IsDBNull(6) ? "" : rd.GetString(6),
                    Monto = rd.IsDBNull(7) ? 0m : rd.GetDecimal(7)
                });
            }

            return list;
        }

        public List<CxCClienteLookupDto> BuscarClientes(string? filtro, int top = 20)
        {
            var list = new List<CxCClienteLookupDto>();

            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
SELECT TOP(@top)
    ClienteId,
    [Código] AS Codigo,
    [Nombre],
    [RNC_Cedula]
FROM dbo.Cliente
WHERE Estado = 1
  AND (@filtro IS NULL OR [Código] LIKE @like OR [Nombre] LIKE @like OR [RNC_Cedula] LIKE @like)
ORDER BY [Nombre];", cn);

            cmd.Parameters.Add("@top", SqlDbType.Int).Value = top;

            if (string.IsNullOrWhiteSpace(filtro))
            {
                cmd.Parameters.Add("@filtro", SqlDbType.NVarChar, 100).Value = DBNull.Value;
                cmd.Parameters.Add("@like", SqlDbType.NVarChar, 120).Value = DBNull.Value;
            }
            else
            {
                var value = filtro.Trim();
                cmd.Parameters.Add("@filtro", SqlDbType.NVarChar, 100).Value = value;
                cmd.Parameters.Add("@like", SqlDbType.NVarChar, 120).Value = "%" + value + "%";
            }

            using var rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                list.Add(new CxCClienteLookupDto
                {
                    ClienteId = rd.GetInt32(0),
                    Codigo = rd.IsDBNull(1) ? "" : rd.GetString(1),
                    Nombre = rd.IsDBNull(2) ? "" : rd.GetString(2),
                    Documento = rd.IsDBNull(3) ? null : rd.GetString(3)
                });
            }

            return list;
        }

        public List<CxCCuentaDestinoDto> ListarCuentasDestino()
        {
            var list = new List<CxCCuentaDestinoDto>();

            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
SELECT 
    'BANCO' AS TipoCuenta,
    BancoCuentaId AS CuentaId,
    COALESCE(NULLIF(NombreMostrar,''), Banco + ' - ' + NoCuenta) AS NombreMostrar,
    MonedaCodigo
FROM dbo.BancoCuenta
WHERE Estado = 1

UNION ALL

SELECT 
    'CAJA' AS TipoCuenta,
    CajaId AS CuentaId,
    COALESCE(Descripcion, CajaNumero) AS NombreMostrar,
    'DOP' AS MonedaCodigo
FROM dbo.Caja
WHERE Estado = 'ACTIVA'

ORDER BY TipoCuenta, NombreMostrar;", cn);

            using var rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                list.Add(new CxCCuentaDestinoDto
                {
                    TipoCuenta = rd.IsDBNull(0) ? "" : rd.GetString(0),
                    CuentaId = rd.GetInt32(1),
                    NombreMostrar = rd.IsDBNull(2) ? "" : rd.GetString(2),
                    MonedaCodigo = rd.IsDBNull(3) ? "DOP" : rd.GetString(3)
                });
            }

            return list;
        }

        public List<CxCCentroCostoDto> ListarCentrosCosto(int empresaId)
        {
            var list = new List<CxCCentroCostoDto>();

            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
SELECT CentroCostoId, Codigo, Nombre
FROM dbo.ContabilidadCentroCosto
WHERE EmpresaId = @emp
  AND Estado = 1
ORDER BY Codigo, Nombre;", cn);

            cmd.Parameters.Add("@emp", SqlDbType.Int).Value = empresaId;

            using var rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                list.Add(new CxCCentroCostoDto
                {
                    CentroCostoId = rd.GetInt32(0),
                    Codigo = rd.IsDBNull(1) ? "" : rd.GetString(1),
                    Nombre = rd.IsDBNull(2) ? "" : rd.GetString(2)
                });
            }

            return list;
        }

        public List<CxCFacturaPendienteDto> ListarFacturasPendientes(int clienteId)
        {
            var list = new List<CxCFacturaPendienteDto>();

            using var cn = Db.GetOpenConnection();
            using var cmd = new SqlCommand(@"
SELECT
    FacturaId,
    NumeroDocumento,
    eNCF,
    FechaDocumento,
    FechaVencimiento,
    TotalFactura,
    TotalCobrado,
    BalancePendiente
FROM dbo.vw_CxCFacturasPendientes
WHERE ClienteId = @cli
  AND BalancePendiente > 0
ORDER BY FechaVencimiento, FechaDocumento, FacturaId;", cn);

            cmd.Parameters.Add("@cli", SqlDbType.Int).Value = clienteId;

            using var rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                list.Add(new CxCFacturaPendienteDto
                {
                    FacturaId = rd.GetInt32(0),
                    NumeroDocumento = rd.IsDBNull(1) ? "" : rd.GetString(1),
                    ENcf = rd.IsDBNull(2) ? null : rd.GetString(2),
                    FechaDocumento = rd.GetDateTime(3),
                    FechaVencimiento = rd.IsDBNull(4) ? null : rd.GetDateTime(4),
                    TotalFactura = rd.IsDBNull(5) ? 0m : rd.GetDecimal(5),
                    TotalCobrado = rd.IsDBNull(6) ? 0m : rd.GetDecimal(6),
                    BalancePendiente = rd.IsDBNull(7) ? 0m : rd.GetDecimal(7),
                    Retencion = 0m,
                    MontoRecibido = 0m
                });
            }

            return list;
        }

        public CxCCobroCrearResultDto Crear(CxCCobroCrearDto dto, string usuario)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            if (dto.Aplicaciones == null || dto.Aplicaciones.Count == 0)
                throw new InvalidOperationException("Debe indicar al menos una factura con monto aplicado.");

            if (string.IsNullOrWhiteSpace(dto.ClienteNombre))
                throw new InvalidOperationException("El cliente es obligatorio.");

            if (string.IsNullOrWhiteSpace(dto.MonedaCodigo))
                throw new InvalidOperationException("La moneda es obligatoria.");

            if (string.IsNullOrWhiteSpace(dto.TipoMedio))
                throw new InvalidOperationException("El tipo de medio es obligatorio.");

            decimal totalAplicado = 0m;
            foreach (var item in dto.Aplicaciones)
            {
                if (item.MontoAplicado > 0)
                    totalAplicado += Math.Round(item.MontoAplicado, 2);
            }

            totalAplicado = Math.Round(totalAplicado, 2);
            var montoMedio = Math.Round(dto.MontoMedio, 2);

            if (totalAplicado <= 0)
                throw new InvalidOperationException("El monto total aplicado debe ser mayor que cero.");

            if (montoMedio <= 0)
                throw new InvalidOperationException("El monto del medio de pago debe ser mayor que cero.");

            if (totalAplicado != montoMedio)
                throw new InvalidOperationException("El total aplicado debe coincidir con el monto del medio.");

            long cobroId = 0;
            string noRecibo = "";
            string paso = "INICIO";

            try
            {
                using (var cn = Db.GetOpenConnection())
                using (var tx = cn.BeginTransaction())
                {
                    try
                    {
                        paso = "CREAR_BORRADOR";

                        using (var cmd = new SqlCommand("dbo.sp_CxC_Cobro_CrearBorrador", cn, tx))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;

                            cmd.Parameters.Add("@EmpresaId", SqlDbType.Int).Value = dto.EmpresaId;
                            cmd.Parameters.Add("@SucursalId", SqlDbType.Int).Value = (object?)dto.SucursalId ?? DBNull.Value;
                            cmd.Parameters.Add("@ClienteId", SqlDbType.Int).Value = (object?)dto.ClienteId ?? DBNull.Value;
                            cmd.Parameters.Add("@ClienteCodigo", SqlDbType.VarChar, 30).Value = (object?)dto.ClienteCodigo ?? DBNull.Value;
                            cmd.Parameters.Add("@ClienteNombre", SqlDbType.NVarChar, 200).Value = dto.ClienteNombre;
                            cmd.Parameters.Add("@ClienteDoc", SqlDbType.VarChar, 30).Value = (object?)dto.ClienteDoc ?? DBNull.Value;
                            cmd.Parameters.Add("@Fecha", SqlDbType.Date).Value = dto.Fecha.Date;
                            cmd.Parameters.Add("@MonedaCodigo", SqlDbType.VarChar, 3).Value = dto.MonedaCodigo;

                            var pTcCab = cmd.Parameters.Add("@TasaCambio", SqlDbType.Decimal);
                            pTcCab.Precision = 18;
                            pTcCab.Scale = 6;
                            pTcCab.Value = dto.TasaCambio;

                            cmd.Parameters.Add("@Observacion", SqlDbType.NVarChar, 300).Value = (object?)dto.Observacion ?? DBNull.Value;

                            var pOut = cmd.Parameters.Add("@CobroId", SqlDbType.BigInt);
                            pOut.Direction = ParameterDirection.Output;

                            using (var rd = cmd.ExecuteReader())
                            {
                                if (rd.Read())
                                {
                                    cobroId = rd.IsDBNull(0) ? 0L : Convert.ToInt64(rd.GetValue(0));
                                    noRecibo = rd.IsDBNull(1) ? "" : rd.GetString(1);
                                }
                            }

                            if (cobroId <= 0 && pOut.Value != DBNull.Value)
                                cobroId = Convert.ToInt64(pOut.Value);

                            if (cobroId <= 0)
                                throw new InvalidOperationException("No se pudo generar el recibo de cobro.");
                        }

                        for (int i = 0; i < dto.Aplicaciones.Count; i++)
                        {
                            var item = dto.Aplicaciones[i];
                            var montoAplicado = Math.Round(item.MontoAplicado, 2);

                            if (montoAplicado <= 0)
                                continue;

                            paso = $"APLICAR_FACTURA_{i + 1}_FACTURAID_{item.FacturaId}";

                            using var cmdDet = new SqlCommand("dbo.sp_CxC_Cobro_AplicarFactura", cn, tx);
                            cmdDet.CommandType = CommandType.StoredProcedure;
                            cmdDet.Parameters.Add("@CobroId", SqlDbType.BigInt).Value = cobroId;
                            cmdDet.Parameters.Add("@FacturaId", SqlDbType.Int).Value = item.FacturaId;
                            AddMoney(cmdDet, "@MontoAplicadoMoneda", montoAplicado);
                            AddMoney(cmdDet, "@MontoAplicadoBase", montoAplicado);
                            cmdDet.Parameters.Add("@Nota", SqlDbType.NVarChar, 200).Value = (object?)item.Nota ?? DBNull.Value;
                            cmdDet.ExecuteNonQuery();
                        }

                        paso = "AGREGAR_MEDIO";

                        using (var cmdMedio = new SqlCommand("dbo.sp_CxC_Cobro_AgregarMedio", cn, tx))
                        {
                            cmdMedio.CommandType = CommandType.StoredProcedure;
                            cmdMedio.Parameters.Add("@CobroId", SqlDbType.BigInt).Value = cobroId;
                            cmdMedio.Parameters.Add("@TipoMedio", SqlDbType.VarChar, 20).Value = dto.TipoMedio.Trim().ToUpperInvariant();

                            AddMoney(cmdMedio, "@MontoMoneda", montoMedio);
                            cmdMedio.Parameters.Add("@MonedaCodigo", SqlDbType.VarChar, 3).Value = dto.MonedaCodigo;

                            var pTc = cmdMedio.Parameters.Add("@TasaCambio", SqlDbType.Decimal);
                            pTc.Precision = 18;
                            pTc.Scale = 6;
                            pTc.Value = dto.TasaCambio;

                            cmdMedio.Parameters.Add("@BancoId", SqlDbType.Int).Value = (object?)dto.BancoId ?? DBNull.Value;
                            cmdMedio.Parameters.Add("@CuentaBancoId", SqlDbType.Int).Value = (object?)dto.CuentaBancoId ?? DBNull.Value;
                            cmdMedio.Parameters.Add("@NumeroCheque", SqlDbType.VarChar, 30).Value =
    string.IsNullOrWhiteSpace(dto.NumeroCheque)
        ? DBNull.Value
        : dto.NumeroCheque.Trim();
                            cmdMedio.Parameters.Add("@Referencia", SqlDbType.VarChar, 80).Value = (object?)dto.Referencia ?? DBNull.Value;
                            cmdMedio.Parameters.Add("@Observacion", SqlDbType.NVarChar, 200).Value = (object?)dto.Observacion ?? DBNull.Value;
                            cmdMedio.ExecuteNonQuery();
                        }

                        if (dto.CentroCostoId.HasValue && dto.CentroCostoId.Value > 0)
                        {
                            paso = "ACTUALIZAR_OBSERVACION_CC";

                            using var cmdObs = new SqlCommand(@"
UPDATE dbo.CxCCobroCab
SET Observacion = CONCAT(
        ISNULL(Observacion,''),
        CASE WHEN ISNULL(Observacion,'') = '' THEN '' ELSE ' | ' END,
        'CC=', @CentroCostoId,
        CASE 
            WHEN @TipoIngreso IS NULL OR LTRIM(RTRIM(@TipoIngreso)) = '' 
                THEN '' 
            ELSE ' | TI=' + @TipoIngreso 
        END
    )
WHERE CobroId = @CobroId;", cn, tx);

                            cmdObs.Parameters.Add("@CobroId", SqlDbType.BigInt).Value = cobroId;
                            cmdObs.Parameters.Add("@CentroCostoId", SqlDbType.Int).Value = dto.CentroCostoId.Value;
                            cmdObs.Parameters.Add("@TipoIngreso", SqlDbType.NVarChar, 50).Value = (object?)dto.TipoIngreso ?? DBNull.Value;
                            cmdObs.ExecuteNonQuery();
                        }

                        paso = "VALIDAR_INSERTS";

                        using (var cmdVal = new SqlCommand(@"
SELECT
    DetCount = (SELECT COUNT(*) FROM dbo.CxCCobroDet WHERE CobroId = @CobroId),
    MedCount = (SELECT COUNT(*) FROM dbo.CxCCobroMedio WHERE CobroId = @CobroId),
    TotalMoneda = (SELECT ISNULL(TotalMoneda,0) FROM dbo.CxCCobroCab WHERE CobroId = @CobroId);", cn, tx))
                        {
                            cmdVal.Parameters.Add("@CobroId", SqlDbType.BigInt).Value = cobroId;

                            using var rdVal = cmdVal.ExecuteReader();
                            if (rdVal.Read())
                            {
                                var detCount = rdVal.IsDBNull(0) ? 0 : rdVal.GetInt32(0);
                                var medCount = rdVal.IsDBNull(1) ? 0 : rdVal.GetInt32(1);
                                var totalMonedaCab = rdVal.IsDBNull(2) ? 0m : rdVal.GetDecimal(2);

                                if (detCount <= 0)
                                    throw new InvalidOperationException("El cobro fue creado pero no tiene facturas aplicadas.");

                                if (medCount <= 0)
                                    throw new InvalidOperationException("El cobro fue creado pero no tiene medios de pago.");

                                if (totalMonedaCab <= 0)
                                    throw new InvalidOperationException("El cobro fue creado pero quedó con total en cero.");
                            }
                            else
                            {
                                throw new InvalidOperationException("No fue posible validar el recibo generado.");
                            }
                        }

                        paso = "COMMIT_CREACION";
                        tx.Commit();
                    }
                    catch
                    {
                        try
                        {
                            if (tx.Connection != null)
                                tx.Rollback();
                        }
                        catch
                        {
                        }

                        throw;
                    }
                }

                if (dto.Postear)
                {
                    paso = "POSTEAR_COBRO";

                    using var cn2 = Db.GetOpenConnection();
                    using var cmdPost = new SqlCommand("dbo.sp_CxC_Cobro_Postear", cn2);
                    cmdPost.CommandType = CommandType.StoredProcedure;
                    cmdPost.Parameters.Add("@CobroId", SqlDbType.BigInt).Value = cobroId;
                    cmdPost.Parameters.Add("@Usuario", SqlDbType.NVarChar, 60).Value =
                        string.IsNullOrWhiteSpace(usuario) ? Environment.UserName : usuario;

                    using var rdPost = cmdPost.ExecuteReader();
                    if (rdPost.Read())
                    {
                        // consumido intencionalmente
                    }
                }

                return new CxCCobroCrearResultDto
                {
                    CobroId = cobroId,
                    NoRecibo = noRecibo,
                    Estado = dto.Postear ? "POSTEADO" : "BORRADOR"
                };
            }
            catch (SqlException ex)
            {
                var detalles = string.Join(Environment.NewLine, ex.Errors.Cast<SqlError>()
                    .Select(e => $"SQL {e.Number} | Línea {e.LineNumber} | {e.Message}"));

                throw new Exception(
                    $"Error en paso: {paso}{Environment.NewLine}" +
                    $"CobroId: {cobroId}{Environment.NewLine}" +
                    detalles, ex);
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $"Error en paso: {paso}{Environment.NewLine}" +
                    $"CobroId: {cobroId}{Environment.NewLine}" +
                    ex.Message, ex);
            }
        }

        private static void AddMoney(SqlCommand cmd, string name, decimal value)
        {
            var p = cmd.Parameters.Add(name, SqlDbType.Decimal);
            p.Precision = 18;
            p.Scale = 2;
            p.Value = Math.Round(value, 2);
        }
    }
}