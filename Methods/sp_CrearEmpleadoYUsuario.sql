ALTER PROCEDURE [dbo].[sp_CrearEmpleadoYUsuario]
    @inNombre VARCHAR(100),
    @inIdTipoDocumento INT,
    @inValorDocumento VARCHAR(64),
    @inFechaNacimiento DATE,
    @inIdPuesto INT,
    @inIdDepartamento INT,

    @inUsername VARCHAR(64),
    @inPassword VARCHAR(100),

    @inIdUsuarioEjecutor INT,
    @inUserIP VARCHAR(64),

    @outResultCode INT OUTPUT,
    @outResultMessage VARCHAR(529) OUTPUT,
	@outIdEmpleado INT OUTPUT,
    @outIdUsuario INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET @outResultCode = 0;
    SET @outResultMessage = 'Operación exitosa';

	SET @outIdEmpleado = -1;
    SET @outIdUsuario = -1;
    
    DECLARE @IdTipoEvento INT = 5;
    DECLARE @EstadoNuevo VARCHAR(MAX);
    DECLARE @TipoUsuarioEmpleado INT = 2;
    
    BEGIN TRY
        BEGIN TRANSACTION;
        
     
        IF EXISTS (SELECT 1 FROM Empleado WHERE ValorDocumento = @inValorDocumento 
		AND IdTipoDocumento = @inIdTipoDocumento
		AND Activo = 1)
        BEGIN
            SET @outResultCode = 50004;
            SET @outResultMessage = 'Ya existe un empleado activo con este documento';
            
            INSERT INTO BitacoraEventos (
                IdUsuario,
                IdTipoEvento,
                FechaHora,
                IPUser,
                Descripcion
            )
            VALUES (
                @inIdUsuarioEjecutor,
                @IdTipoEvento,
                GETDATE(),
                @inUserIP,
                JSON_OBJECT(
					'error': @outResultMessage,
					'numeroError': @outResultCode
				)
            );
            
            COMMIT TRANSACTION;
            RETURN;
        END;
        
        -- 2. Validar que el username no exista
        IF EXISTS (SELECT 1 FROM Usuario WHERE Username = @inUsername)
        BEGIN
            SET @outResultCode = 50005;
            SET @outResultMessage = 'El nombre de usuario ya está en uso';
            
            -- Registrar intento fallido en bitácora
            INSERT INTO BitacoraEventos (
                IdUsuario,
                IdTipoEvento,
                FechaHora,
                IPUser,
                Descripcion
            )
            VALUES (
                @inIdUsuarioEjecutor,
                @IdTipoEvento,
                GETDATE(),
                @inUserIP,
                JSON_OBJECT(
					'error': @outResultMessage,
					'numeroError': @outResultCode
				)
            );
            
            COMMIT TRANSACTION;
            RETURN;
        END;
        
        -- 3. Crear el usuario primero
        INSERT INTO Usuario (
            Username,
            Password,
            Tipo
        )
        VALUES (
            @inUsername,
            @inPassword,
            @TipoUsuarioEmpleado
        );
		SET @outIdUsuario = SCOPE_IDENTITY();
                
        -- 4. Crear el empleado
        INSERT INTO Empleado (
            Nombre,
            IdTipoDocumento,
            ValorDocumento,
            FechaNacimiento,
            IdPuesto,
            IdDepartamento,
            IdUsuario,
            Activo
        )
        VALUES (
            @inNombre,
            @inIdTipoDocumento,
            @inValorDocumento,
            @inFechaNacimiento,
            @inIdPuesto,
            @inIdDepartamento,
            @outIdUsuario,
            1
        );
		SET @outIdEmpleado = SCOPE_IDENTITY();
           
        -- 5. Preparar datos para bitácora
        SET @EstadoNuevo = JSON_OBJECT(
            'Nombre': @inNombre,
            'TipoDocumento': @inIdTipoDocumento,
            'ValorDocumento': @inValorDocumento,
            'FechaNacimiento': CONVERT(VARCHAR, @inFechaNacimiento, 120),
            'Puesto': @inIdPuesto,
            'Departamento': @inIdDepartamento,
            'Username': @inUsername,
            'TipoUsuario': @TipoUsuarioEmpleado
        );
        
        -- 6. Registrar en bitácora
        INSERT INTO BitacoraEventos (
            IdUsuario,
            IdTipoEvento,
            FechaHora,
            IPUser,
            Descripcion
        )
        VALUES (
            @inIdUsuarioEjecutor,
            @IdTipoEvento,
            GETDATE(),
            @inUserIP,
            @EstadoNuevo
        );
        
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        
        SET @outResultCode = ERROR_NUMBER();
        SET @outResultMessage = ERROR_MESSAGE();
        
        -- Registrar error técnico en bitácora
        INSERT INTO BitacoraEventos (
            IdUsuario,
            IdTipoEvento,
            FechaHora,
            IPUser,
            Descripcion
        )
        VALUES (
            @inIdUsuarioEjecutor,
            @IdTipoEvento,
            GETDATE(),
            @inUserIP,
            JSON_OBJECT(
                'Error': ERROR_MESSAGE(),
                'NumeroError': ERROR_NUMBER(),
                'LineaError': ERROR_LINE(),
                'Empleado': JSON_QUERY(@EstadoNuevo)
            )
        );
    END CATCH;
END;