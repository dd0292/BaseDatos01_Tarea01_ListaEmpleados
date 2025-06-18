CREATE PROCEDURE [dbo].[sp_CargarCatalogoXML]
	@inXmlCat XML,
	@outResultCode INT OUTPUT,
	@outResultDescription Nvarchar(529) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
	BEGIN TRY
		-- 1. Cargar Tipos de Documento de Identidad
        INSERT INTO TiposDocumentoIdentidad (Id, Nombre)
        SELECT
            T.Item.value('@Id', 'INT'),
            T.Item.value('@Nombre', 'VARCHAR(64)')
        FROM @inXmlCat.nodes('/Catalogo/TiposdeDocumentodeIdentidad/TipoDocuIdentidad') AS T(Item);
        -- 2. Cargar Tipos de Jornada
        INSERT INTO TiposJornada (Id, Nombre, HoraInicio, HoraFin)
        SELECT
            T.Item.value('@Id', 'INT'),
            T.Item.value('@Nombre', 'VARCHAR(64)'),
            T.Item.value('@HoraInicio', 'TIME'),
            T.Item.value('@HoraFin', 'TIME')
        FROM @inXmlCat.nodes('/Catalogo/TiposDeJornada/TipoDeJornada') AS T(Item)
        -- 3. Cargar Puestos
        INSERT INTO Puesto (Nombre, SalarioXHora)
        SELECT
            T.Item.value('@Nombre', 'VARCHAR(100)'),
            T.Item.value('@SalarioXHora', 'DECIMAL(10,2)')
        FROM @inXmlCat.nodes('/Catalogo/Puestos/Puesto') AS T(Item);
        -- 4. Cargar Departamentos
        INSERT INTO Departamento (Id, Nombre)
        SELECT
            T.Item.value('@Id', 'INT'),
            T.Item.value('@Nombre', 'VARCHAR(100)')
        FROM @inXmlCat.nodes('/Catalogo/Departamentos/Departamento') AS T(Item);
        -- 5. Cargar Feriados
        INSERT INTO Feriado (Id, Nombre, Fecha)
        SELECT
            T.Item.value('@Id', 'INT'),
            T.Item.value('@Nombre', 'VARCHAR(100)'),
            CONVERT(DATE, T.Item.value('@Fecha', 'VARCHAR(8)'), 112)
        FROM @inXmlCat.nodes('/Catalogo/Feriados/Feriado') AS T(Item);
        -- 6. Cargar Tipos de Movimiento
        INSERT INTO TiposMovimiento (Id, Nombre)
        SELECT
            T.Item.value('@Id', 'INT'),
            T.Item.value('@Nombre', 'VARCHAR(100)')
        FROM @inXmlCat.nodes('/Catalogo/TiposDeMovimiento/TipoDeMovimiento') AS T(Item);
        -- 7. Cargar Tipos de Deducción
        INSERT INTO TiposDeduccion (Id, Nombre, Obligatorio, Porcentual, Valor)
        SELECT
            T.Item.value('@Id', 'INT'),
            T.Item.value('@Nombre', 'VARCHAR(100)'),
            CASE WHEN T.Item.value('@Obligatorio', 'VARCHAR(2)') = 'Si' THEN 1 ELSE 0 END,
            CASE WHEN T.Item.value('@Porcentual', 'VARCHAR(2)') = 'Si' THEN 1 ELSE 0 END,
            T.Item.value('@Valor', 'DECIMAL(10,4)')
        FROM @inXmlCat.nodes('/Catalogo/TiposDeDeduccion/TipoDeDeduccion') AS T(Item);
        -- 8. Cargar Errores (si existe tabla para esto)
        INSERT INTO Errores (Id,Codigo, Descripcion)
        SELECT
			T.Item.value('@Codigo', 'INT'),
            T.Item.value('@Codigo', 'INT'),
            T.Item.value('@Descripcion', 'VARCHAR(255)')
        FROM @inXmlCat.nodes('/Catalogo/Errores/Error') AS T(Item);
        -- 9. Cargar Usuarios
        INSERT INTO Usuario (Id, Username, Password, Tipo)
        SELECT
            T.Item.value('@Id', 'INT'),
            T.Item.value('@Username', 'VARCHAR(50)'),
            T.Item.value('@Password', 'VARCHAR(100)'),
            T.Item.value('@Tipo', 'INT')
        FROM @inXmlCat.nodes('/Catalogo/Usuarios/Usuario') AS T(Item)
		WHERE T.Item.value('@Tipo', 'INT') IN (1, 2); 
        -- 11. Cargar Tipos de Evento
        INSERT INTO TiposEvento (Id, Nombre)
        SELECT
            T.Item.value('@Id', 'INT'),
            T.Item.value('@Nombre', 'VARCHAR(100)')
        FROM @inXmlCat.nodes('/Catalogo/TiposdeEvento/TipoEvento') AS T(Item);
        -- 12. Cargar Empleados 
        INSERT INTO Empleado (
            Nombre, 
            IdTipoDocumento, 
            ValorDocumento, 
            FechaNacimiento, 
            IdDepartamento, 
            IdPuesto, 
            IdUsuario, 
            Activo
        )
        SELECT
            T.Item.value('@Nombre', 'VARCHAR(100)'),
            T.Item.value('@IdTipoDocumento', 'INT'),
            T.Item.value('@ValorDocumento', 'VARCHAR(50)'),
            T.Item.value('@FechaNacimiento', 'DATE'),
            T.Item.value('@IdDepartamento', 'INT'),
            P.Id, -- Obtenemos el Id del puesto basado en el nombre
            T.Item.value('@IdUsuario', 'INT'),
            T.Item.value('@Activo', 'BIT')
        FROM @inXmlCat.nodes('/Catalogo/Empleados/Empleado') AS T(Item)
        INNER JOIN Puesto P ON P.Nombre = T.Item.value('@NombrePuesto', 'VARCHAR(100)');
		SET @outResultCode = 0;
		SET @outResultDescription = '';
	END TRY
    BEGIN CATCH
        SET @outResultCode = ERROR_NUMBER();
		SET @outResultDescription = ERROR_MESSAGE();
    END CATCH;
    SET NOCOUNT OFF;
END;