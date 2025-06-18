CREATE PROCEDURE sp_CargarOperacionesXML
    @XmlOperacion XML
AS
BEGIN
    SET NOCOUNT ON;

    -- 1. Extraer fechas ordenadas del XML
    DECLARE @FechasOperacion TABLE (RowNum INT IDENTITY(1,1), Fecha DATE);

    INSERT INTO @FechasOperacion (Fecha)
    SELECT DISTINCT
        x.value('@Fecha', 'DATE') AS Fecha
    FROM 
        @XmlOperacion.nodes('/Operacion/FechaOperacion') AS T(x)
    ORDER BY Fecha;

    DECLARE @TotalFechas INT = (SELECT COUNT(*) FROM @FechasOperacion);
    DECLARE @Indice INT = 1;
    DECLARE @FechaActual DATE;

    WHILE @Indice <= @TotalFechas
    BEGIN
        SELECT @FechaActual = Fecha FROM @FechasOperacion WHERE RowNum = @Indice;

        -- Procesar: Nuevos empleados
        EXEC sub_ProcesarNuevosEmpleados @XmlOperacion, @FechaActual;

        -- Procesar: Asociar y desasociar deducciones
        EXEC sub_ProcesarDeducciones @XmlOperacion, @FechaActual;

        -- Procesar: Jornadas futuras
        EXEC sub_ProcesarJornadas @XmlOperacion, @FechaActual;

        -- Procesar: Asistencias
        EXEC sub_ProcesarAsistencias @XmlOperacion, @FechaActual;

        -- Cierre de semana si es jueves
        IF DATENAME(WEEKDAY, @FechaActual) = 'Thursday'
        BEGIN
            EXEC sub_RealizarCierreSemanal @FechaActual;
            EXEC sub_PrepararNuevaSemana @FechaActual;

            -- Verifica si mañana es el primer viernes del mes
            IF DATEPART(DAY, DATEADD(DAY, 1, @FechaActual)) BETWEEN 1 AND 7
               AND DATENAME(WEEKDAY, DATEADD(DAY, 1, @FechaActual)) = 'Friday'
            BEGIN
                EXEC sub_AbrirNuevoMes @FechaActual;
            END
        END

        SET @Indice += 1;
    END
END;
