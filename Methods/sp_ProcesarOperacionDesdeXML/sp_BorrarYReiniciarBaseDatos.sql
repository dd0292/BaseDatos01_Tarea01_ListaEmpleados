ALTER PROCEDURE [dbo].[sp_BorrarYReiniciarBaseDatos]
	@outResultCode INT OUTPUT,
	@outResultDescription Nvarchar(529) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        BEGIN TRANSACTION;
        
        -- Borrar datos de tablas con dependencias en orden inverso
        DELETE FROM BitacoraEventos;
        DELETE FROM Asistencias;
        DELETE FROM Movimiento;
        DELETE FROM DeduccionesMensualesEmpleado;
        DELETE FROM PlanillaMensualEmpleado;
        DELETE FROM PlanillaSemanalEmpleado;
        DELETE FROM JornadasEmpleados;
        DELETE FROM SemanasPlanilla;
        DELETE FROM MesesPlanilla;
        DELETE FROM EmpleadosDeduccion;
        DELETE FROM Empleado;
        DELETE FROM Usuario;
        
        -- Borrar datos de tablas maestras
        DELETE FROM TiposEvento;
        DELETE FROM TiposDeduccion;
        DELETE FROM TiposMovimiento;
        DELETE FROM Feriado;
        DELETE FROM Puesto;
        DELETE FROM Departamento;
        DELETE FROM TiposJornada;
        DELETE FROM TiposDocumentoIdentidad;
        
        -- Reiniciar los valores IDENTITY en las tablas que lo requieren
        DBCC CHECKIDENT ('BitacoraEventos', RESEED, 0);
        DBCC CHECKIDENT ('Asistencias', RESEED, 0);
        DBCC CHECKIDENT ('Movimiento', RESEED, 0);
        DBCC CHECKIDENT ('DeduccionesMensualesEmpleado', RESEED, 0);
        DBCC CHECKIDENT ('PlanillaMensualEmpleado', RESEED, 0);
        DBCC CHECKIDENT ('PlanillaSemanalEmpleado', RESEED, 0);
        DBCC CHECKIDENT ('JornadasEmpleados', RESEED, 0);
        DBCC CHECKIDENT ('SemanasPlanilla', RESEED, 0);
        DBCC CHECKIDENT ('MesesPlanilla', RESEED, 0);
        DBCC CHECKIDENT ('EmpleadosDeduccion', RESEED, 0);
        DBCC CHECKIDENT ('Empleado', RESEED, 0);
        DBCC CHECKIDENT ('Puesto', RESEED, 0);
        
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