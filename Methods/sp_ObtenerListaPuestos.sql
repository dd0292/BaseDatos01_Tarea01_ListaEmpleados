ALTER PROCEDURE [dbo].[sp_ObtenerListaPuestos]
    @outResultCode INT OUTPUT,
	@outResultDescription Nvarchar(529) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY  
        SELECT 
            Id,
            Nombre,
            SalarioXHora
        FROM 
            dbo.Puesto
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