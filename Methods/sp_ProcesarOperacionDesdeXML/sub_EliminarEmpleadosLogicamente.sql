ALTER PROCEDURE [dbo].[sub_EliminarEmpleadosLogicamente]
    @XmlOperacion XML,
    @FechaOperacion DATE,

    @inIdUsuario INT,
    @inUserIP VARCHAR(64)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Now DATETIME = GETDATE();

    -- Tabla temporal con los documentos a eliminar
    CREATE TABLE #EmpleadosEliminar (
        ValorDocumento VARCHAR(64)
    );

    INSERT INTO #EmpleadosEliminar (ValorDocumento)
    SELECT x.value('@ValorTipoDocumento', 'VARCHAR(64)')
    FROM @XmlOperacion.nodes('/Operacion/FechaOperacion[@Fecha=sql:variable("@FechaOperacion")]/EliminarEmpleados/EliminarEmpleado') AS T(x);

    -- Eliminar lógicamente
    UPDATE Empleado
    SET Activo = 0
    WHERE ValorDocumento IN (SELECT ValorDocumento FROM #EmpleadosEliminar);

    -- Bitácora por cada eliminación
    INSERT INTO BitacoraEventos (IdUsuario, IdTipoEvento, FechaHora, IPUser, Descripcion)
    SELECT 
        @inIdUsuario,
        6, 
        GETDATE(),
        @inUserIP,
        JSON_OBJECT('IdEmpleado': e.ValorDocumento)
    FROM #EmpleadosEliminar AS e;

    DROP TABLE #EmpleadosEliminar;
END;

