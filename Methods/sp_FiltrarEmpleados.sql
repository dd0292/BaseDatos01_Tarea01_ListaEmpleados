ALTER PROCEDURE [dbo].[sp_FiltrarEmpleados]
    @inFiltro VARCHAR(100) = NULL,
    @inIdUsuario INT,
    @inUserIP VARCHAR(64),
    @outResultCode INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @IdTipoEvento INT;
    DECLARE @DescripcionEventoParaBitacora VARCHAR(MAX);
    
    IF @inFiltro IS NULL OR @inFiltro = ''
    BEGIN
        SET @IdTipoEvento = 3; 
        SET @DescripcionEventoParaBitacora = '';
    END
    ELSE
    BEGIN
        SET @IdTipoEvento = 4;
        SET @DescripcionEventoParaBitacora = JSON_OBJECT('filtro': @inFiltro);
    END;

    BEGIN TRY
        SELECT 
            e.Id AS IdEmpleado,
            e.Nombre AS NombreEmpleado,
            
            tdi.Id AS IdTipoDocumento,
            tdi.Nombre AS TipoDocumento,
            
            e.ValorDocumento AS DocumentoEmpleado,
            
            p.Id AS IdPuesto,
            p.Nombre AS NombrePuesto,
            p.SalarioXHora AS SalarioPorHora,
            
            d.Id AS IdDepartamento,
            d.Nombre AS Departamento,
            
            e.FechaNacimiento AS FechaNacimiento,
            e.IdUsuario AS IdUsuario,
            u.Username AS Usuario,
			u.Password AS Passphrase,
            
            CASE WHEN e.Activo = 1 THEN 'Activo' ELSE 'Inactivo' END AS Estado
            
        FROM dbo.Empleado AS e
        INNER JOIN dbo.Puesto AS p ON p.Id = e.IdPuesto
        INNER JOIN dbo.Departamento AS d ON d.Id = e.IdDepartamento
        INNER JOIN dbo.TiposDocumentoIdentidad AS tdi ON tdi.Id = e.IdTipoDocumento
        INNER JOIN dbo.Usuario AS u ON u.Id = e.IdUsuario
        WHERE 
            e.Activo = 1
            AND (
                @inFiltro IS NULL 
                OR @inFiltro = ''
                OR e.Nombre LIKE '%' + @inFiltro + '%'
                OR e.ValorDocumento LIKE '%' + @inFiltro + '%'
                OR p.Nombre LIKE '%' + @inFiltro + '%'
                OR d.Nombre LIKE '%' + @inFiltro + '%'
                OR tdi.Nombre LIKE '%' + @inFiltro + '%'
            )
        ORDER BY 
            e.Nombre ASC;
        
        SET @outResultCode = 0;
        
        INSERT INTO dbo.BitacoraEventos (
            IdUsuario,
            IdTipoEvento,
            FechaHora,
            IPUser,
            Descripcion
        )
        VALUES (
            @inIdUsuario,
            @IdTipoEvento,
            GETDATE(),
            @inUserIP,
            @DescripcionEventoParaBitacora
        );
    END TRY
    BEGIN CATCH
        SET @outResultCode = ERROR_NUMBER();

        INSERT INTO dbo.BitacoraEventos (
            IdUsuario,
            IdTipoEvento,
            FechaHora,
            IPUser,
            Descripcion
        )
        VALUES (
            @inIdUsuario,
            @IdTipoEvento,
            GETDATE(),
            @inUserIP,
            JSON_OBJECT(
                'error': ERROR_MESSAGE(),
                'filtro': @inFiltro,
                'numeroError': ERROR_NUMBER(),
                'lineaError': ERROR_LINE()
            )
        );
    END CATCH;
    
    SET NOCOUNT OFF;
END;