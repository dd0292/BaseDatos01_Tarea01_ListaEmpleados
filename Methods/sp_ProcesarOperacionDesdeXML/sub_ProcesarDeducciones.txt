CREATE PROCEDURE sub_ProcesarDeducciones
    @XmlOperacion XML,
    @FechaOperacion DATE
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @InicioSemana DATE = DATEADD(DAY, (8 - DATEPART(WEEKDAY, @FechaOperacion)) % 7, @FechaOperacion);

    -- ========================
    -- 1. Asociaciones nuevas
    -- ========================
    WITH DeduccionesNuevas AS (
        SELECT 
            d.value('@ValorTipoDocumento', 'VARCHAR(64)') AS ValorDocumento,
            d.value('@IdTipoDeduccion', 'INT') AS IdTipoDeduccion,
            d.value('@Monto', 'DECIMAL(10,2)') AS Monto
        FROM @XmlOperacion.nodes('/Operacion/FechaOperacion[@Fecha=sql:variable("@FechaOperacion")]/AsociacionEmpleadoDeducciones/AsociacionEmpleadoConDeduccion') AS T(d)
    )
    INSERT INTO EmpleadosDeduccion (IdEmpleado, IdTipoDeduccion, MontoFijo, FechaAsociacion)
    SELECT 
        e.Id,
        dn.IdTipoDeduccion,
        CASE WHEN td.Obligatorio = 0 THEN dn.Monto ELSE NULL END,
        @InicioSemana
    FROM DeduccionesNuevas dn
    JOIN Empleado e ON e.ValorDocumento = dn.ValorDocumento
    JOIN TiposDeduccion td ON td.Id = dn.IdTipoDeduccion
    WHERE NOT EXISTS (
        SELECT 1
        FROM EmpleadosDeduccion ed
        WHERE ed.IdEmpleado = e.Id AND ed.IdTipoDeduccion = dn.IdTipoDeduccion
          AND ed.FechaDesasociacion IS NULL
    );

    -- ========================
    -- 2. Desasociaciones
    -- ========================
    WITH DeduccionesAEliminar AS (
        SELECT 
            d.value('@ValorTipoDocumento', 'VARCHAR(64)') AS ValorDocumento,
            d.value('@IdTipoDeduccion', 'INT') AS IdTipoDeduccion
        FROM @XmlOperacion.nodes('/Operacion/FechaOperacion[@Fecha=sql:variable("@FechaOperacion")]/DesasociacionEmpleadoDeducciones/DesasociacionEmpleadoConDeduccion') AS T(d)
    )
    UPDATE ed
    SET FechaDesasociacion = @InicioSemana
    FROM EmpleadosDeduccion ed
    JOIN Empleado e ON e.Id = ed.IdEmpleado
    JOIN DeduccionesAEliminar da ON da.ValorDocumento = e.ValorDocumento AND da.IdTipoDeduccion = ed.IdTipoDeduccion
    WHERE ed.FechaDesasociacion IS NULL;

END;
