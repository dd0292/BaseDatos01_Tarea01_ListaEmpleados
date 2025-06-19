ALTER PROCEDURE [dbo].[sp_CargarOperacionesXML]
    @XmlOperacion XML,
	@inIdUsuario INT,
	@inUserIP VARCHAR(64),

	@outResultCode INT OUTPUT,
    @outResultDescription VARCHAR(529) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

	BEGIN TRY
		BEGIN TRANSACTION;
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

				-- Procesar: Nuevos empleados [y bitacora]
				EXEC sub_ProcesarNuevosEmpleados @XmlOperacion, @FechaActual, @inIdUsuario, @inUserIP;

				-- Procesar: Eliminar empleados [y bitacora]
				EXEC sub_EliminarEmpleadosLogicamente @XmlOperacion, @FechaActual, @inIdUsuario, @inUserIP;

				-- Procesar: Asociar y desasociar deducciones [y bitacora]
				EXEC sub_ProcesarDeducciones @XmlOperacion, @FechaActual, @inIdUsuario, @inUserIP;

				-- Procesar: Jornadas futuras [y bitacora]
				EXEC sub_ProcesarJornadas @XmlOperacion, @FechaActual, @inIdUsuario, @inUserIP;

				-- Procesar: Asistencias [y bitacora]
				EXEC sub_ProcesarAsistencias @XmlOperacion, @FechaActual, @inIdUsuario, @inUserIP;

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
			END;
		COMMIT TRANSACTION;

		SET @outResultCode = 0;
        SET @outResultDescription = '';
	END TRY

	BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        
        SET @outResultCode = ERROR_NUMBER();
        SET @outResultDescription = ERROR_MESSAGE();
	END CATCH;
END;