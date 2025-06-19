ALTER PROCEDURE [dbo].[sp_RegistrarEvento]
    @inIdUsuario INT,
    @inIdTipoEvento INT,
    @inIPUser VARCHAR(50),
    @inDescripcion VARCHAR(MAX),

	@outResultCode INT OUT,
	@outResultDescription VARCHAR(529)
AS
BEGIN
    SET NOCOUNT ON;
	BEGIN TRY
		INSERT INTO BitacoraEventos (IdUsuario, IdTipoEvento, FechaHora, IPUser, Descripcion)
		VALUES (@inIdUsuario, @inIdTipoEvento, GETDATE(), @inIPUser, @inDescripcion);
	
		SET @outResultCode = 0;
		SET @outResultDescription = '';
	END TRY
	BEGIN CATCH
		SET @outResultCode = ERROR_NUMBER();
		SET @outResultDescription = ERROR_MESSAGE();
	END CATCH
END;
