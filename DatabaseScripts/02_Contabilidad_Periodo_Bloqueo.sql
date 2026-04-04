/* ============================
   02 - Bloqueo por período contable (IDEMPOTENTE)
   ============================ */

-- Tabla de períodos
IF OBJECT_ID('dbo.ContabilidadPeriodo', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.ContabilidadPeriodo
    (
        PeriodoId     INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_ContabilidadPeriodo PRIMARY KEY,
        Anio          INT NOT NULL,
        Mes           INT NOT NULL,
        FechaInicio   DATE NOT NULL,
        FechaFin      DATE NOT NULL,
        Estado        VARCHAR(20) NOT NULL CONSTRAINT DF_ContabilidadPeriodo_Estado DEFAULT ('ABIERTO'), -- ABIERTO/CERRADO
        UsuarioCierre VARCHAR(60) NULL,
        FechaCierre   DATETIME NULL,
        FechaCreacion DATETIME NOT NULL CONSTRAINT DF_ContabilidadPeriodo_FechaCreacion DEFAULT (GETDATE())
    );

    CREATE UNIQUE INDEX UX_ContabilidadPeriodo_AnioMes ON dbo.ContabilidadPeriodo(Anio, Mes);
END
GO

-- SP: crea período si no existe
CREATE OR ALTER PROCEDURE dbo.sp_Conta_Periodo_Ensure
    @Fecha DATETIME2(0)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Anio INT = YEAR(@Fecha);
    DECLARE @Mes  INT = MONTH(@Fecha);

    IF NOT EXISTS (SELECT 1 FROM dbo.ContabilidadPeriodo WHERE Anio=@Anio AND Mes=@Mes)
    BEGIN
        DECLARE @Ini DATE = DATEFROMPARTS(@Anio, @Mes, 1);
        DECLARE @Fin DATE = EOMONTH(@Ini);

        INSERT INTO dbo.ContabilidadPeriodo(Anio, Mes, FechaInicio, FechaFin, Estado)
        VALUES (@Anio, @Mes, @Ini, @Fin, 'ABIERTO');
    END
END
GO

-- SP: valida si el período está abierto (para bloquear movimientos en período cerrado)
CREATE OR ALTER PROCEDURE dbo.sp_Conta_Periodo_EstaAbierto
    @Fecha       DATETIME2(0),
    @EstaAbierto BIT OUTPUT,
    @PeriodoId   INT OUTPUT,
    @Mensaje     NVARCHAR(200) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    -- Asegura que exista registro del período
    EXEC dbo.sp_Conta_Periodo_Ensure @Fecha=@Fecha;

    DECLARE @Anio INT = YEAR(@Fecha);
    DECLARE @Mes  INT = MONTH(@Fecha);

    SELECT TOP (1)
        @PeriodoId = PeriodoId,
        @EstaAbierto = CASE WHEN UPPER(Estado) = 'ABIERTO' THEN 1 ELSE 0 END
    FROM dbo.ContabilidadPeriodo
    WHERE Anio=@Anio AND Mes=@Mes;

    IF @EstaAbierto = 1
        SET @Mensaje = N'Período contable ABIERTO.';
    ELSE
        SET @Mensaje = N'Período contable CERRADO: no se permiten asientos en ' + CAST(@Mes AS NVARCHAR(2)) + N'/' + CAST(@Anio AS NVARCHAR(4)) + N'.';
END
GO

-- SP: cerrar período (opcional)
CREATE OR ALTER PROCEDURE dbo.sp_Conta_Periodo_Cerrar
    @Anio INT,
    @Mes  INT,
    @Usuario VARCHAR(60)
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM dbo.ContabilidadPeriodo WHERE Anio=@Anio AND Mes=@Mes)
    BEGIN
        DECLARE @Ini DATE = DATEFROMPARTS(@Anio, @Mes, 1);
        DECLARE @Fin DATE = EOMONTH(@Ini);
        INSERT INTO dbo.ContabilidadPeriodo(Anio, Mes, FechaInicio, FechaFin, Estado)
        VALUES (@Anio, @Mes, @Ini, @Fin, 'ABIERTO');
    END

    UPDATE dbo.ContabilidadPeriodo
    SET Estado='CERRADO', UsuarioCierre=@Usuario, FechaCierre=GETDATE()
    WHERE Anio=@Anio AND Mes=@Mes;
END
GO

-- SP: abrir período (opcional)
CREATE OR ALTER PROCEDURE dbo.sp_Conta_Periodo_Abrir
    @Anio INT,
    @Mes  INT,
    @Usuario VARCHAR(60) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM dbo.ContabilidadPeriodo WHERE Anio=@Anio AND Mes=@Mes)
    BEGIN
        DECLARE @Ini DATE = DATEFROMPARTS(@Anio, @Mes, 1);
        DECLARE @Fin DATE = EOMONTH(@Ini);
        INSERT INTO dbo.ContabilidadPeriodo(Anio, Mes, FechaInicio, FechaFin, Estado)
        VALUES (@Anio, @Mes, @Ini, @Fin, 'ABIERTO');
    END
    ELSE
    BEGIN
        UPDATE dbo.ContabilidadPeriodo
        SET Estado='ABIERTO', UsuarioCierre=NULL, FechaCierre=NULL
        WHERE Anio=@Anio AND Mes=@Mes;
    END
END
GO
