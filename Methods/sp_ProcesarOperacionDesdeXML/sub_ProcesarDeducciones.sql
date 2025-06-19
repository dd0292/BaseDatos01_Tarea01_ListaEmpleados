ALTER PROCEDURE [dbo].[sub_ProcesarDeducciones]
    @XmlOperacion XML,
    @FechaOperacion DATE,

    @inIdUsuario INT,
    @inUserIP VARCHAR(64)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @InicioSemana DATE = DATEADD(DAY, (8 - DATEPART(WEEKDAY, @FechaOperacion)) % 7, @FechaOperacion);

    -- ========================
    -- 1. Asociaciones nuevas
    -- ========================

    -- Tabla temporal para nuevas asociaciones
    CREATE TABLE #DeduccionesNuevas (
        ValorDocumento VARCHAR(64),
        IdTipoDeduccion INT,
        Monto DECIMAL(25,2)
    );

    INSERT INTO #DeduccionesNuevas (ValorDocumento, IdTipoDeduccion, Monto)
    SELECT 
        d.value('@ValorTipoDocumento', 'VARCHAR(64)'),
        d.value('@IdTipoDeduccion', 'INT'),
        d.value('@Monto', 'DECIMAL(25,2)')
    FROM @XmlOperacion.nodes('/Operacion/FechaOperacion[@Fecha=sql:variable("@FechaOperacion")]/AsociacionEmpleadoDeducciones/AsociacionEmpleadoConDeduccion') AS T(d);

    -- Insertar deducciones
    INSERT INTO EmpleadosDeduccion (IdEmpleado, IdTipoDeduccion, MontoFijo, FechaAsociacion)
    SELECT 
        e.Id,
        dn.IdTipoDeduccion,
        CASE WHEN td.Obligatorio = 0 THEN dn.Monto ELSE NULL END,
        @InicioSemana
    FROM #DeduccionesNuevas dn
    JOIN Empleado e ON e.ValorDocumento = dn.ValorDocumento
    JOIN TiposDeduccion td ON td.Id = dn.IdTipoDeduccion
    WHERE NOT EXISTS (
        SELECT 1 FROM EmpleadosDeduccion ed
        WHERE ed.IdEmpleado = e.Id AND ed.IdTipoDeduccion = dn.IdTipoDeduccion AND ed.FechaDesasociacion IS NULL
    );

    -- Registrar bitácora para cada asociación
    INSERT INTO BitacoraEventos (IdUsuario, IdTipoEvento, FechaHora, IPUser, Descripcion)
    SELECT 
        @inIdUsuario,
        8,
        GETDATE(),
        @inUserIP,
		JSON_OBJECT(
			'EmpleadoId': e.Id,
            'TipoDeduccionId': td.Id,
            'Porcentual': td.Porcentual,
            'MontoFijo': ISNULL(dn.Monto, 0)	
		)
    FROM #DeduccionesNuevas dn
    JOIN Empleado e ON e.ValorDocumento = dn.ValorDocumento
    JOIN TiposDeduccion td ON td.Id = dn.IdTipoDeduccion
    WHERE NOT EXISTS (
        SELECT 1 FROM EmpleadosDeduccion ed
        WHERE ed.IdEmpleado = e.Id AND ed.IdTipoDeduccion = dn.IdTipoDeduccion AND ed.FechaDesasociacion IS NULL
    );

    -- ========================
    -- 2. Desasociaciones
    -- ========================

    -- Tabla temporal para desasociaciones
    CREATE TABLE #DeduccionesAEliminar (
        ValorDocumento VARCHAR(64),
        IdTipoDeduccion INT
    );

    INSERT INTO #DeduccionesAEliminar (ValorDocumento, IdTipoDeduccion)
    SELECT 
        d.value('@ValorTipoDocumento', 'VARCHAR(64)'),
        d.value('@IdTipoDeduccion', 'INT')
    FROM @XmlOperacion.nodes('/Operacion/FechaOperacion[@Fecha=sql:variable("@FechaOperacion")]/DesasociacionEmpleadoDeducciones/DesasociacionEmpleadoConDeduccion') AS T(d);

    -- Bitácora antes de desasociar
    INSERT INTO BitacoraEventos (IdUsuario, IdTipoEvento, FechaHora, IPUser, Descripcion)
    SELECT 
        @inIdUsuario,
        9,
        GETDATE(),
        @inUserIP,
		JSON_OBJECT(
			'EmpleadoId': e.Id,
            'TipoDeduccionId': da.IdTipoDeduccion	
		)
    FROM #DeduccionesAEliminar AS da
    JOIN Empleado AS e ON e.ValorDocumento = da.ValorDocumento
    JOIN EmpleadosDeduccion AS ed ON ed.IdEmpleado = e.Id AND ed.IdTipoDeduccion = da.IdTipoDeduccion
    WHERE ed.FechaDesasociacion IS NULL;

    -- Desasociar
    UPDATE ed
    SET FechaDesasociacion = @InicioSemana
    FROM EmpleadosDeduccion AS ed
    JOIN Empleado AS e ON e.Id = ed.IdEmpleado
    JOIN #DeduccionesAEliminar da ON da.ValorDocumento = e.ValorDocumento AND da.IdTipoDeduccion = ed.IdTipoDeduccion
    WHERE ed.FechaDesasociacion IS NULL;

    DROP TABLE #DeduccionesNuevas;
    DROP TABLE #DeduccionesAEliminar;
END;
