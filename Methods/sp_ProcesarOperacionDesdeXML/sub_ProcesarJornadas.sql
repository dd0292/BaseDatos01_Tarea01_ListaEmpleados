ALTER PROCEDURE [dbo].[sub_ProcesarJornadas]
    @XmlOperacion XML,
    @FechaOperacion DATE,
    @inIdUsuario INT,
    @inUserIP VARCHAR(64)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @InicioSemana DATE = DATEADD(DAY, (8 - DATEPART(WEEKDAY, @FechaOperacion)) % 7, @FechaOperacion);
    DECLARE @IdSemana INT;

    SELECT TOP 1 @IdSemana = sp.Id
    FROM SemanasPlanilla sp
    WHERE sp.FechaInicio = @InicioSemana;

    IF @IdSemana IS NULL RETURN;

    CREATE TABLE #Jornadas (
        ValorDocumento VARCHAR(64),
        IdTipoJornada INT
    );

    INSERT INTO #Jornadas (ValorDocumento, IdTipoJornada)
    SELECT 
        j.value('@ValorTipoDocumento', 'VARCHAR(64)'),
        j.value('@IdTipoJornada', 'INT')
    FROM @XmlOperacion.nodes('/Operacion/FechaOperacion[@Fecha=sql:variable("@FechaOperacion")]/JornadasProximaSemana/TipoJornadaProximaSemana') AS T(j);

    -- Insertar jornadas
    INSERT INTO JornadasEmpleados (IdEmpleado, IdSemana, IdTipoJornada)
    SELECT 
        e.Id,
        @IdSemana,
        j.IdTipoJornada
    FROM #Jornadas j
    JOIN Empleado e ON e.ValorDocumento = j.ValorDocumento
    WHERE NOT EXISTS (
        SELECT 1 FROM JornadasEmpleados je
        WHERE je.IdEmpleado = e.Id AND je.IdSemana = @IdSemana
    );

    -- Registrar en bitácora
    INSERT INTO BitacoraEventos (IdUsuario, IdTipoEvento, FechaHora, IPUser, Descripcion)
    SELECT 
        @inIdUsuario,
        15,
        GETDATE(),
        @inUserIP,
		JSON_OBJECT(
			'EmpleadoId':  e.Id,
            'TipoJornadaId': j.IdTipoJornada,
            'SemanaId': @IdSemana	
		)
    FROM #Jornadas j
    JOIN Empleado e ON e.ValorDocumento = j.ValorDocumento
    WHERE NOT EXISTS (
        SELECT 1 FROM JornadasEmpleados je
        WHERE je.IdEmpleado = e.Id AND je.IdSemana = @IdSemana
    );

    DROP TABLE #Jornadas;
END;
