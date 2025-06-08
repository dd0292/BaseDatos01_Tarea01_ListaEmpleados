ALTER PROCEDURE [dbo].[sp_EditarEmpleado]
    @inIdEmpleado INT,
    @inNombre VARCHAR(100),
    @inIdTipoDocumento INT,
    @inValorDocumento VARCHAR(50),
    @inFechaNacimiento DATE,
    @inIdPuesto INT,
    @inIdDepartamento INT,
    @inIdUsuario INT,
    @inUserIP VARCHAR(64),
    @outResultCode INT OUTPUT,
    @outResultMessage VARCHAR(529) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
	BEGIN TRANSACTION;
        SET @outResultCode = 0;
        SET @outResultMessage = 'Operación exitosa';
        
        DECLARE @EstadoOriginal VARCHAR(MAX);
        
        
        IF NOT EXISTS (SELECT 1 FROM Empleado WHERE Id = @inIdEmpleado AND Activo = 1)
        BEGIN
            SET @outResultCode = 50002;
            SET @outResultMessage = 'El empleado no existe o está inactivo';
            RETURN;
        END;
        
        SELECT @EstadoOriginal = (
            SELECT 
                Nombre,
                IdTipoDocumento,
                ValorDocumento,
                FechaNacimiento,
                IdPuesto,
                IdDepartamento
            FROM Empleado
            WHERE Id = @inIdEmpleado
            FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
        );

        IF EXISTS (
            SELECT 1 
            FROM Empleado 
            WHERE ValorDocumento = @inValorDocumento 
			  AND IdTipoDocumento = @inIdTipoDocumento
              AND Id <> @inIdEmpleado
              AND Activo = 1
        )
        BEGIN
            SET @outResultCode = 50003;
            SET @outResultMessage = 'Ya existe otro empleado activo con este documento';
            RETURN;
        END;
        
        UPDATE Empleado 
        SET 
            Nombre = @inNombre,
            IdTipoDocumento = @inIdTipoDocumento,
            ValorDocumento = @inValorDocumento,
            FechaNacimiento = @inFechaNacimiento,
            IdPuesto = @inIdPuesto,
            IdDepartamento = @inIdDepartamento
        WHERE 
            Id = @inIdEmpleado;
            
        
        DECLARE @EstadoActualizado VARCHAR(MAX);
        SELECT @EstadoActualizado = (
            SELECT 
                @inNombre AS Nombre,
                @inIdTipoDocumento AS IdTipoDocumento,
                @inValorDocumento AS ValorDocumento,
                @inFechaNacimiento AS FechaNacimiento,
                @inIdPuesto AS IdPuesto,
                @inIdDepartamento AS IdDepartamento
            FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
        );
        
        INSERT INTO BitacoraEventos (
            IdUsuario,
            IdTipoEvento,
            FechaHora,
            IPUser,
            Descripcion
        )
        VALUES (
            @inIdUsuario,
            7, 
            GETDATE(),
            @inUserIP,
            JSON_OBJECT(
                'IdEmpleado': @inIdEmpleado,
                'Antes': JSON_QUERY(@EstadoOriginal),
                'Despues': JSON_QUERY(@EstadoActualizado)
            )
        );
    COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
	IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
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
            7, 
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