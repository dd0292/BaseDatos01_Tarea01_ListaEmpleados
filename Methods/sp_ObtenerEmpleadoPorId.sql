ALTER PROCEDURE [dbo].[sp_ObtenerEmpleadoPorId]
    @inId INT,
    @outResultCode INT OUTPUT,
    @outResultDescription VARCHAR(529) OUTPUT
AS
BEGIN
    BEGIN TRY
        SET NOCOUNT ON;
        
        SELECT 
            e.Id,
            e.Nombre,
            td.Id AS IdTipoDocumento,
            td.Nombre AS TipoDocumento,
            e.ValorDocumento,
            p.Id AS IdPuesto,
            p.Nombre AS NombrePuesto,
            p.SalarioXHora AS SalarioPorHora,
            d.Id AS IdDepartamento,
            d.Nombre AS Departamento,
            e.FechaNacimiento,
            u.Id AS IdUsuario,
            u.Username AS Username,
            u.Password AS Passphrase,
            e.Activo AS Estado
        FROM 
            Empleado e
            INNER JOIN TiposDocumentoIdentidad td ON e.IdTipoDocumento = td.Id
            INNER JOIN Puesto p ON e.IdPuesto = p.Id
            INNER JOIN Departamento d ON e.IdDepartamento = d.Id
            INNER JOIN Usuario u ON e.IdUsuario = u.Id
        WHERE 
            e.Id = @inId;
        
        IF @@ROWCOUNT = 0
        BEGIN
            SET @outResultCode = -1;
            SET @outResultDescription = 'Empleado no encontrado';
        END
        ELSE
        BEGIN
            SET @outResultCode = 0;
            SET @outResultDescription = 'Empleado encontrado correctamente';
        END
    END TRY
    BEGIN CATCH
        SET @outResultCode = ERROR_NUMBER();
        SET @outResultDescription = ERROR_MESSAGE();
    END CATCH
END