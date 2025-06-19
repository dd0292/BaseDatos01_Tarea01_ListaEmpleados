ALTER PROCEDURE [dbo].[sub_AbrirNuevoMes]
    @FechaOperacion DATE
AS
BEGIN
    SET NOCOUNT ON;

    -- Día siguiente (viernes)
    DECLARE @FechaInicioMes DATE = DATEADD(DAY, 1, @FechaOperacion);
    DECLARE @Anio INT = YEAR(@FechaInicioMes);
    DECLARE @Mes INT = MONTH(@FechaInicioMes);

    -- Verificar si ya existe el mes
    IF EXISTS (
        SELECT 1 FROM MesesPlanilla
        WHERE Anio = @Anio AND Mes = @Mes
    )
        RETURN;

    -- Calcular fecha fin del mes planilla (último jueves antes del primer viernes del próximo mes)
    DECLARE @FechaFinMes DATE;

    SET @FechaFinMes = (
        SELECT MAX(FechaFin)
        FROM (
            SELECT DATEADD(DAY, 6, Fecha) AS FechaFin
            FROM (
                -- Generamos todos los viernes del mes
                SELECT DATEADD(DAY, v.n * 7, @FechaInicioMes) AS Fecha
                FROM (
                    SELECT TOP 5 ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) - 1 AS n
                    FROM sys.all_objects
                ) v
                WHERE DATENAME(WEEKDAY, DATEADD(DAY, v.n * 7, @FechaInicioMes)) = 'Friday'
                    AND MONTH(DATEADD(DAY, v.n * 7, @FechaInicioMes)) = @Mes
            ) Viernes
        ) Fechas
    );

    -- 1. Insertar encabezado de mes
    INSERT INTO MesesPlanilla (Anio, Mes, FechaInicio, FechaFin, Cerrado)
    VALUES (@Anio, @Mes, @FechaInicioMes, @FechaFinMes, 0);

    DECLARE @IdMes INT = SCOPE_IDENTITY();

    -- 2. Crear encabezado mensual para todos los empleados activos
    INSERT INTO PlanillaMensualEmpleado (IdEmpleado, IdMes)
    SELECT Id, @IdMes
    FROM Empleado
    WHERE Activo = 1;
END;
