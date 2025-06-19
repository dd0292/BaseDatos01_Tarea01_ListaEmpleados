ALTER PROCEDURE [dbo].[sub_ProcesarAsistencias]
    @XmlOperacion XML,
    @FechaOperacion DATE,

    @inIdUsuario INT,
    @inUserIP VARCHAR(64)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @EsFeriado BIT = CASE WHEN EXISTS (
        SELECT 1 FROM Feriado WHERE Fecha = @FechaOperacion
    ) THEN 1 ELSE 0 END;

    DECLARE @EsDomingo BIT = CASE WHEN DATENAME(WEEKDAY, @FechaOperacion) = 'Sunday' THEN 1 ELSE 0 END;

    -- Crear tabla temporal para asistencias procesadas
    CREATE TABLE #AsistenciasProcesadas (
        IdEmpleado INT,
        HoraEntrada DATETIME,
        HoraSalida DATETIME,
        SalarioXHora DECIMAL(10,2),
        HoraInicio TIME,
        HoraFin TIME,
        IdSemana INT,
        FechaAsistencia DATE
    );

    -- Cargar asistencias desde XML
    INSERT INTO #AsistenciasProcesadas
    SELECT 
        e.Id,
        ax.HoraEntrada,
        ax.HoraSalida,
        p.SalarioXHora,
        tj.HoraInicio,
        tj.HoraFin,
        sj.Id,
        CAST(ax.HoraEntrada AS DATE)
    FROM (
        SELECT 
            x.value('@ValorTipoDocumento', 'VARCHAR(64)') AS ValorDocumento,
            x.value('@HoraEntrada', 'DATETIME') AS HoraEntrada,
            x.value('@HoraSalida', 'DATETIME') AS HoraSalida
        FROM @XmlOperacion.nodes('/Operacion/FechaOperacion[@Fecha=sql:variable("@FechaOperacion")]/MarcasAsistencia/MarcaDeAsistencia') AS T(x)
    ) ax
    JOIN Empleado e ON e.ValorDocumento = ax.ValorDocumento
    JOIN JornadasEmpleados je ON je.IdEmpleado = e.Id
    JOIN SemanasPlanilla sj ON sj.Id = je.IdSemana AND @FechaOperacion BETWEEN sj.FechaInicio AND sj.FechaFin
    JOIN TiposJornada tj ON tj.Id = je.IdTipoJornada
    JOIN Puesto p ON p.Id = e.IdPuesto;

    -- Insertar asistencias
    INSERT INTO Asistencias (IdEmpleado, Fecha, HoraEntrada, HoraSalida)
    SELECT IdEmpleado, FechaAsistencia, HoraEntrada, HoraSalida
    FROM #AsistenciasProcesadas;

    -- Registrar bitácora
    INSERT INTO BitacoraEventos (IdUsuario, IdTipoEvento, FechaHora, IPUser, Descripcion)
    SELECT 
        @inIdUsuario,
        14, 
        GETDATE(),
        @inUserIP,
		JSON_OBJECT(
			'EmpleadoId':  ap.IdEmpleado,
            'HoraEntrada': CONVERT(varchar, ap.HoraEntrada, 126),
            'HoraSalida': CONVERT(varchar, ap.HoraSalida, 126),
			'Fecha': CONVERT(varchar, ap.FechaAsistencia, 126)
		)
    FROM #AsistenciasProcesadas ap;

    -- Calcular horas y clasificar
    SELECT
        ap.IdEmpleado,
        ap.IdSemana,
        ap.FechaAsistencia AS Fecha,
        ap.SalarioXHora,
        DATEDIFF(MINUTE, 
            CASE WHEN ap.HoraEntrada < CAST(ap.FechaAsistencia AS DATETIME) + CAST(ap.HoraInicio AS DATETIME)
                 THEN CAST(ap.FechaAsistencia AS DATETIME) + CAST(ap.HoraInicio AS DATETIME)
                 ELSE ap.HoraEntrada END,
            CASE WHEN ap.HoraSalida > 
                    DATEADD(DAY, CASE WHEN ap.HoraFin < ap.HoraInicio THEN 1 ELSE 0 END,
                        CAST(ap.FechaAsistencia AS DATETIME)) + CAST(ap.HoraFin AS DATETIME)
                 THEN DATEADD(DAY, CASE WHEN ap.HoraFin < ap.HoraInicio THEN 1 ELSE 0 END,
                        CAST(ap.FechaAsistencia AS DATETIME)) + CAST(ap.HoraFin AS DATETIME)
                 ELSE ap.HoraSalida END
        ) / 60 AS HorasOrdinarias,
        CASE 
            WHEN ap.HoraSalida > 
                DATEADD(DAY, CASE WHEN ap.HoraFin < ap.HoraInicio THEN 1 ELSE 0 END,
                    CAST(ap.FechaAsistencia AS DATETIME)) + CAST(ap.HoraFin AS DATETIME)
            THEN DATEDIFF(MINUTE, 
                    DATEADD(DAY, CASE WHEN ap.HoraFin < ap.HoraInicio THEN 1 ELSE 0 END,
                        CAST(ap.FechaAsistencia AS DATETIME)) + CAST(ap.HoraFin AS DATETIME),
                    ap.HoraSalida
                 ) / 60
            ELSE 0
        END AS HorasExtrasCrudas
	INTO #Clasificadas
    FROM #AsistenciasProcesadas AS ap;

    -- Añadir columnas de horas clasificadas
    ALTER TABLE #Clasificadas
    ADD HorasExtrasNormales INT, HorasExtrasDobles INT;

    UPDATE #Clasificadas
	SET HorasExtrasNormales = CASE WHEN @EsFeriado = 1 OR @EsDomingo = 1 THEN 0 ELSE HorasExtrasCrudas END,
        HorasExtrasDobles = CASE WHEN @EsFeriado = 1 OR @EsDomingo = 1 THEN HorasExtrasCrudas ELSE 0 END;

    -- Insertar movimientos
    INSERT INTO Movimiento (IdEmpleado, IdSemana, Fecha, TipoMovimiento, CategoriaMovimiento, Monto, CantidadHoras, Fuente)
    SELECT IdEmpleado, IdSemana, Fecha, 'Devengado', 'Horas Ordinarias', SalarioXHora * HorasOrdinarias, HorasOrdinarias, 'Asistencia'
    FROM #Clasificadas WHERE HorasOrdinarias > 0;

    INSERT INTO Movimiento (IdEmpleado, IdSemana, Fecha, TipoMovimiento, CategoriaMovimiento, Monto, CantidadHoras, Fuente)
    SELECT IdEmpleado, IdSemana, Fecha, 'Devengado', 'Horas Extra Normales', SalarioXHora * 1.5 * HorasExtrasNormales, HorasExtrasNormales, 'Asistencia'
    FROM #Clasificadas WHERE HorasExtrasNormales > 0;

    INSERT INTO Movimiento (IdEmpleado, IdSemana, Fecha, TipoMovimiento, CategoriaMovimiento, Monto, CantidadHoras, Fuente)
    SELECT IdEmpleado, IdSemana, Fecha, 'Devengado', 'Horas Extra Dobles', SalarioXHora * 2 * HorasExtrasDobles, HorasExtrasDobles, 'Asistencia'
    FROM #Clasificadas WHERE HorasExtrasDobles > 0;

    -- Actualizar planilla semanal
    MERGE PlanillaSemanalEmpleado tgt
    USING (
        SELECT 
            IdEmpleado,
            IdSemana,
            SUM(SalarioXHora * HorasOrdinarias + SalarioXHora * 1.5 * HorasExtrasNormales + SalarioXHora * 2 * HorasExtrasDobles) AS SalarioBruto,
            SUM(HorasOrdinarias) AS HorasOrdinarias,
            SUM(HorasExtrasNormales) AS HorasExtrasNormales,
            SUM(HorasExtrasDobles) AS HorasExtrasDobles
        FROM #Clasificadas
        GROUP BY IdEmpleado, IdSemana
    ) src ON src.IdEmpleado = tgt.IdEmpleado AND src.IdSemana = tgt.IdSemana
    WHEN MATCHED THEN
        UPDATE SET 
            SalarioBruto = tgt.SalarioBruto + src.SalarioBruto,
            HorasOrdinarias = tgt.HorasOrdinarias + src.HorasOrdinarias,
            HorasExtrasNormales = tgt.HorasExtrasNormales + src.HorasExtrasNormales,
            HorasExtrasDobles = tgt.HorasExtrasDobles + src.HorasExtrasDobles
    WHEN NOT MATCHED THEN
        INSERT (IdEmpleado, IdSemana, SalarioBruto, HorasOrdinarias, HorasExtrasNormales, HorasExtrasDobles)
        VALUES (src.IdEmpleado, src.IdSemana, src.SalarioBruto, src.HorasOrdinarias, src.HorasExtrasNormales, src.HorasExtrasDobles);

    DROP TABLE #AsistenciasProcesadas;
    DROP TABLE #Clasificadas;
END;