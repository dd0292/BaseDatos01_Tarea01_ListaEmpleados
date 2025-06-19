ALTER PROCEDURE [dbo].[sub_PrepararNuevaSemana]
    @FechaOperacion DATE
AS
BEGIN
    SET NOCOUNT ON;

    -- 1. Calcular inicio y fin de la nueva semana
    DECLARE @FechaInicioNuevaSemana DATE = DATEADD(DAY, 1, @FechaOperacion);
    DECLARE @FechaFinNuevaSemana DATE = DATEADD(DAY, 6, @FechaInicioNuevaSemana);

    -- 2. Determinar el mes asociado a esta nueva semana
    DECLARE @IdMes INT;

    SELECT TOP 1 @IdMes = Id
    FROM MesesPlanilla
    WHERE @FechaInicioNuevaSemana BETWEEN FechaInicio AND FechaFin;

    IF @IdMes IS NULL
    BEGIN
        -- Si no existe un mes que cubra esa semana, abortar
        RETURN;
    END

    -- 3. Crear nueva semana si no existe
    IF NOT EXISTS (
        SELECT 1 FROM SemanasPlanilla
        WHERE FechaInicio = @FechaInicioNuevaSemana
    )
    BEGIN
        INSERT INTO SemanasPlanilla (IdMes, NumeroSemana, FechaInicio, FechaFin, Cerrada)
        VALUES (
            @IdMes,
            (SELECT COUNT(*) + 1 FROM SemanasPlanilla WHERE IdMes = @IdMes),
            @FechaInicioNuevaSemana,
            @FechaFinNuevaSemana,
            0
        );
    END

    DECLARE @IdSemana INT;
    SELECT @IdSemana = Id
    FROM SemanasPlanilla
    WHERE FechaInicio = @FechaInicioNuevaSemana;

    -- 4. Crear encabezado de planilla semanal para todos los empleados activos
    INSERT INTO PlanillaSemanalEmpleado (IdEmpleado, IdSemana)
    SELECT e.Id, @IdSemana
    FROM Empleado e
    WHERE e.Activo = 1
    AND NOT EXISTS (
        SELECT 1 FROM PlanillaSemanalEmpleado p
        WHERE p.IdEmpleado = e.Id AND p.IdSemana = @IdSemana
    );

END;