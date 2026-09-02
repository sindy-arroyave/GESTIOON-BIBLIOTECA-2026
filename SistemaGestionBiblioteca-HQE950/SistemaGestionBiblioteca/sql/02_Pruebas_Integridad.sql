USE BibliotecaDB;
GO

/* Estas pruebas no dejan cambios porque se revierten al final de cada bloque. */
BEGIN TRANSACTION;
BEGIN TRY
    DECLARE @UsuarioId INT = (SELECT TOP (1) UsuarioId FROM dbo.Usuarios ORDER BY UsuarioId);
    DECLARE @LibroId INT = (SELECT TOP (1) LibroId FROM dbo.Libros WHERE CantidadDisponible > 0 ORDER BY LibroId);
    DECLARE @Antes INT = (SELECT CantidadDisponible FROM dbo.Libros WHERE LibroId = @LibroId);
    DECLARE @Prestamo TABLE (PrestamoId INT);
    INSERT @Prestamo EXEC dbo.sp_RegistrarPrestamo @UsuarioId, @LibroId, DATEADD(day, 15, CONVERT(date, GETDATE()));
    DECLARE @PrestamoId INT = (SELECT PrestamoId FROM @Prestamo);
    IF (SELECT CantidadDisponible FROM dbo.Libros WHERE LibroId = @LibroId) <> @Antes - 1 THROW 51000, 'Falló la reducción de disponibilidad.', 1;
    EXEC dbo.sp_RegistrarDevolucion @PrestamoId, N'Prueba automática';
    IF (SELECT CantidadDisponible FROM dbo.Libros WHERE LibroId = @LibroId) <> @Antes THROW 51001, 'Falló la restauración de disponibilidad.', 1;
    PRINT N'OK: préstamo y devolución actualizan existencias correctamente.';
    ROLLBACK TRANSACTION;
END TRY
BEGIN CATCH
    IF XACT_STATE() <> 0 ROLLBACK TRANSACTION;
    THROW;
END CATCH;
GO

SELECT * FROM dbo.vw_LibrosDetalle ORDER BY Titulo;
SELECT * FROM dbo.vw_HistorialPrestamos ORDER BY FechaPrestamo DESC;
SELECT c.Nombre AS Categoria, COUNT(l.LibroId) AS Titulos, SUM(ISNULL(l.CantidadTotal,0)) AS Ejemplares
FROM dbo.Categorias c LEFT JOIN dbo.Libros l ON l.CategoriaId=c.CategoriaId
GROUP BY c.Nombre ORDER BY c.Nombre;
GO
