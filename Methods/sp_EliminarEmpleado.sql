ALTER PROCEDURE [dbo].[sp_EliminarEmpleado]
    @inIdEmpleado INT,
    @inIdUsuario INT,
    @inUserIP VARCHAR(64),
    @outResultCode INT OUTPUT,
    @outResultMessage VARCHAR(529) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        SET @outResultCode = 0;
        SET @outResultMessage = 'Operación exitosa';

        IF NOT EXISTS (SELECT 1 FROM Empleado WHERE Id = @inIdEmpleado AND Activo = 1)
        BEGIN
            SET @outResultCode = 50001;
            SET @outResultMessage = 'El empleado no existe o ya está inactivo';
            RETURN;
        END;
       
        UPDATE Empleado 
        SET 
            Activo = 0
        WHERE 
            Id = @inIdEmpleado;
            
        INSERT INTO BitacoraEventos (
            IdUsuario,
            IdTipoEvento,
            FechaHora,
            IPUser,
            Descripcion
        )
        VALUES (
            @inIdUsuario,
            6,
            GETDATE(),
            @inUserIP,
            JSON_OBJECT('IdEmpleado': @inIdEmpleado)
        );
        
    END TRY
    BEGIN CATCH
        SET @outResultCode = ERROR_NUMBER();
        SET @outResultMessage = ERROR_MESSAGE();
        
		INSERT INTO BitacoraEventos (
            IdUsuario,
            IdTipoEvento,
            FechaHora,
            IPUser,
            Descripcion
        )
        VALUES (
            @inIdUsuario,
            6,
            GETDATE(),
            @inUserIP,
            JSON_OBJECT(
                'error': ERROR_MESSAGE(),
                'IdEmpleado': @inIdEmpleado,
                'numeroError': ERROR_NUMBER(),
                'lineaError': ERROR_LINE()
            )
        );

    END CATCH;
    
    SET NOCOUNT OFF;
END;