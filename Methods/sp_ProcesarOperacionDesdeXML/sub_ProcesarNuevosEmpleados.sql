ALTER PROCEDURE [dbo].[sub_ProcesarNuevosEmpleados]
    @XmlOperacion XML,
    @FechaOperacion DATE,

	@inIdUsuario INT,
	@inUserIP VARCHAR(64)
AS
BEGIN
    SET NOCOUNT ON;
	 DECLARE @NuevoEmpleado VARCHAR(MAX);

	-- Crear tabla temporal para almacenar empleados nuevos
	CREATE TABLE #EmpleadosNuevos (
		Nombre VARCHAR(100),
		IdTipoDocumento INT,
		ValorDocumento VARCHAR(64),
		IdDepartamento INT,
		NombrePuesto VARCHAR(100),
		Usuario VARCHAR(64),
		Password VARCHAR(100)
	);

	-- Extraer empleados de la fecha actual a la tabla temporal
	INSERT INTO #EmpleadosNuevos (
		Nombre, 
		IdTipoDocumento, 
		ValorDocumento, 
		IdDepartamento, 
		NombrePuesto, 
		Usuario, 
		Password
	)
	SELECT 
		e.value('@Nombre', 'VARCHAR(100)') AS Nombre,
		e.value('@IdTipoDocumento', 'INT') AS IdTipoDocumento,
		e.value('@ValorTipoDocumento', 'VARCHAR(64)') AS ValorDocumento,
		e.value('@IdDepartamento', 'INT') AS IdDepartamento,
		e.value('@NombrePuesto', 'VARCHAR(100)') AS NombrePuesto,
		e.value('@Usuario', 'VARCHAR(64)') AS Usuario,
		e.value('@Password', 'VARCHAR(100)') AS Password
	FROM @XmlOperacion.nodes('OperacionFechaOperacion[@Fecha=sql:variable("@FechaOperacion")]/NuevosEmpleados/NuevoEmpleado') AS T(e);

	-- Inserta los usuarios
	INSERT INTO Usuario (Username, Password, Tipo)
	SELECT en.Usuario, en.Password, 2
	FROM #EmpleadosNuevos AS en
	WHERE NOT EXISTS (
		SELECT 1 FROM Usuario AS u WHERE u.Username = en.Usuario
	);

	-- Inserta empleados
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
		en.Nombre,
		en.IdTipoDocumento,
		en.ValorDocumento,
		'1990-01-01', -- FechaNacimiento por defecto
		en.IdDepartamento,
		p.Id,
		u.Id,
		1
	FROM #EmpleadosNuevos AS en
	INNER JOIN Puesto p ON p.Nombre = en.NombrePuesto
	INNER JOIN Usuario u ON u.Username = en.Usuario
	WHERE NOT EXISTS (
		SELECT 1 FROM Empleado emp WHERE emp.ValorDocumento = en.ValorDocumento
	);

	-- Inserta en bitacora
    INSERT INTO BitacoraEventos (IdUsuario, IdTipoEvento, FechaHora, IPUser, Descripcion)
    SELECT 
        @inIdUsuario,
        5, 
        GETDATE(),
        @inUserIP,
		JSON_OBJECT(
			'Nombre': en.Nombre,
            'TipoDocumento': en.IdTipoDocumento,
            'ValorDocumento': en.ValorDocumento,
            'FechaNacimiento': '1990-01-01',
            'Puesto': p.Id,
            'Departamento': en.IdDepartamento,
            'Username': en.Usuario,
            'TipoUsuario': 2		
		)
		FROM #EmpleadosNuevos AS en
		INNER JOIN Puesto p ON p.Nombre = en.NombrePuesto;

	-- Eliminar la tabla temporal
	DROP TABLE #EmpleadosNuevos;
END;