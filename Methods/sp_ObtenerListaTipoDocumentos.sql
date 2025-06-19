ALTER PROCEDURE [dbo].[sp_ObtenerListaTipoDocumentos]
    @outResultCode INT OUTPUT,
	@outResultDescription NVARCHAR (529) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY  
        SELECT 
            Id,
            Nombre
        FROM 
            dbo.TiposDocumentoIdentidad
        ORDER BY 
            Nombre ASC;
        SET @outResultCode = 0;
		SET @outResultDescription = '';
    END TRY
    BEGIN CATCH
        SET @outResultCode = ERROR_NUMBER();
		SET @outResultDescription = ERROR_MESSAGE();
    END CATCH;
    SET NOCOUNT OFF;
END;