ALTER PROCEDURE [dbo].[sub_RealizarCierreSemanal]
    @FechaOperacion DATE
AS
BEGIN
    SET NOCOUNT ON;

    -- Obtener la semana y mes activos
    DECLARE @IdSemana INT, @IdMes INT, @SemanasEnMes INT;

    SELECT TOP 1 
        @IdSemana = sp.Id,
        @IdMes = sp.IdMes
    FROM SemanasPlanilla sp
    WHERE @FechaOperacion BETWEEN sp.FechaInicio AND sp.FechaFin;

    IF @IdSemana IS NULL RETURN;

    SELECT @SemanasEnMes = COUNT(*) FROM SemanasPlanilla WHERE IdMes = @IdMes;

    -- ===========================
    -- 1. Calcular deducciones
    -- ===========================
    -- Porcentuales
    SELECT 
        pse.IdEmpleado,
        td.Id AS IdTipoDeduccion,
        td.Nombre AS CategoriaMovimiento,
        ROUND(pse.SalarioBruto * td.Valor, 2) AS Monto,
        'Porcentual' AS Tipo
    INTO #Deducciones
    FROM PlanillaSemanalEmpleado pse
    JOIN EmpleadosDeduccion ed ON pse.IdEmpleado = ed.IdEmpleado
    JOIN TiposDeduccion td ON td.Id = ed.IdTipoDeduccion
    WHERE pse.IdSemana = @IdSemana
      AND td.Porcentual = 1
      AND ed.FechaDesasociacion IS NULL;

    -- Fijas no obligatorias
    INSERT INTO #Deducciones (IdEmpleado, IdTipoDeduccion, CategoriaMovimiento, Monto, Tipo)
    SELECT 
        pse.IdEmpleado,
        td.Id,
        td.Nombre,
        ROUND(ed.MontoFijo / @SemanasEnMes, 2),
        'Fija'
    FROM PlanillaSemanalEmpleado pse
    JOIN EmpleadosDeduccion ed ON pse.IdEmpleado = ed.IdEmpleado
    JOIN TiposDeduccion td ON td.Id = ed.IdTipoDeduccion
    WHERE pse.IdSemana = @IdSemana
      AND td.Porcentual = 0 AND td.Obligatorio = 0
      AND ed.FechaDesasociacion IS NULL;

    -- ===========================
    -- 2. Insertar movimientos de deducción
    -- ===========================
    INSERT INTO Movimiento (IdEmpleado, IdSemana, Fecha, TipoMovimiento, CategoriaMovimiento, Monto, Fuente)
    SELECT 
        d.IdEmpleado,
        @IdSemana,
        @FechaOperacion,
        'Deducción',
        d.CategoriaMovimiento,
        d.Monto,
        'Deducción'
    FROM #Deducciones d;

    -- ===========================
    -- 3. Actualizar planilla semanal
    -- ===========================
    UPDATE pse
    SET 
        pse.TotalDeducciones = d.TotalDeducciones,
        pse.SalarioNeto = pse.SalarioBruto - d.TotalDeducciones
    FROM PlanillaSemanalEmpleado pse
    JOIN (
        SELECT IdEmpleado, SUM(Monto) AS TotalDeducciones
        FROM #Deducciones
        GROUP BY IdEmpleado
    ) d ON pse.IdEmpleado = d.IdEmpleado
    WHERE pse.IdSemana = @IdSemana;

    -- ===========================
    -- 4. Actualizar planilla mensual
    -- ===========================
    MERGE PlanillaMensualEmpleado AS pme
    USING (
        SELECT 
            pse.IdEmpleado,
            pse.SalarioBruto,
            pse.TotalDeducciones,
            pse.SalarioBruto - pse.TotalDeducciones AS SalarioNeto
        FROM PlanillaSemanalEmpleado pse
        WHERE pse.IdSemana = @IdSemana
    ) src
    ON pme.IdEmpleado = src.IdEmpleado AND pme.IdMes = @IdMes
    WHEN MATCHED THEN
        UPDATE SET 
            SalarioBruto = pme.SalarioBruto + src.SalarioBruto,
            TotalDeducciones = pme.TotalDeducciones + src.TotalDeducciones,
            SalarioNeto = pme.SalarioNeto + src.SalarioNeto
    WHEN NOT MATCHED THEN
        INSERT (IdEmpleado, IdMes, SalarioBruto, TotalDeducciones, SalarioNeto)
        VALUES (src.IdEmpleado, @IdMes, src.SalarioBruto, src.TotalDeducciones, src.SalarioNeto);

    -- ===========================
    -- 5. Insertar deducciones mensuales
    -- ===========================
    INSERT INTO DeduccionesMensualesEmpleado (IdPlanillaMensual, IdTipoDeduccion, Monto)
    SELECT 
        pme.Id,
        d.IdTipoDeduccion,
        d.Monto
    FROM #Deducciones d
    JOIN PlanillaMensualEmpleado pme 
        ON pme.IdEmpleado = d.IdEmpleado AND pme.IdMes = @IdMes
    WHERE d.Tipo = 'Fija';

    -- ===========================
    -- 6. Cerrar semana
    -- ===========================
    UPDATE SemanasPlanilla SET Cerrada = 1 WHERE Id = @IdSemana;

    DROP TABLE #Deducciones;
END;
