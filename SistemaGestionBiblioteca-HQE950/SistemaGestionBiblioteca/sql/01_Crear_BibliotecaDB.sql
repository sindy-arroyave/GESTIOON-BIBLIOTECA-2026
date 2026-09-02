/*
  Sistema de Gestión de Biblioteca
  Motor: Microsoft SQL Server 2012 o superior
  Ejecución: abrir en SQL Server Management Studio y ejecutar el archivo completo.
*/

SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

IF DB_ID(N'BibliotecaDB') IS NULL
BEGIN
    CREATE DATABASE BibliotecaDB;
END;
GO

USE BibliotecaDB;
GO

IF OBJECT_ID(N'dbo.Autores', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Autores
    (
        AutorId INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Autores PRIMARY KEY,
        Codigo VARCHAR(20) NOT NULL CONSTRAINT UQ_Autores_Codigo UNIQUE,
        Nombre NVARCHAR(80) NOT NULL,
        Apellidos NVARCHAR(100) NOT NULL,
        Nacionalidad NVARCHAR(80) NULL,
        FechaNacimiento DATE NOT NULL,
        FechaRegistro DATETIME2(0) NOT NULL CONSTRAINT DF_Autores_FechaRegistro DEFAULT SYSDATETIME(),
        CONSTRAINT CK_Autores_Codigo CHECK (LEN(LTRIM(RTRIM(Codigo))) > 0),
        CONSTRAINT CK_Autores_FechaNacimiento CHECK (FechaNacimiento <= CONVERT(date, GETDATE()))
    );
END;
GO

IF OBJECT_ID(N'dbo.Categorias', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Categorias
    (
        CategoriaId INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Categorias PRIMARY KEY,
        Nombre NVARCHAR(80) NOT NULL CONSTRAINT UQ_Categorias_Nombre UNIQUE,
        Descripcion NVARCHAR(250) NULL,
        Activa BIT NOT NULL CONSTRAINT DF_Categorias_Activa DEFAULT 1,
        CONSTRAINT CK_Categorias_Nombre CHECK (LEN(LTRIM(RTRIM(Nombre))) > 0)
    );
END;
GO

IF OBJECT_ID(N'dbo.Usuarios', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Usuarios
    (
        UsuarioId INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Usuarios PRIMARY KEY,
        Documento VARCHAR(25) NOT NULL CONSTRAINT UQ_Usuarios_Documento UNIQUE,
        Nombre NVARCHAR(80) NOT NULL,
        Apellidos NVARCHAR(100) NOT NULL,
        Telefono VARCHAR(20) NOT NULL,
        Correo VARCHAR(150) NOT NULL CONSTRAINT UQ_Usuarios_Correo UNIQUE,
        ProgramaAcademico NVARCHAR(120) NOT NULL,
        Activo BIT NOT NULL CONSTRAINT DF_Usuarios_Activo DEFAULT 1,
        FechaRegistro DATETIME2(0) NOT NULL CONSTRAINT DF_Usuarios_FechaRegistro DEFAULT SYSDATETIME(),
        CONSTRAINT CK_Usuarios_Documento CHECK (LEN(LTRIM(RTRIM(Documento))) > 0),
        CONSTRAINT CK_Usuarios_Telefono CHECK (LEN(Telefono) BETWEEN 7 AND 20),
        CONSTRAINT CK_Usuarios_Correo CHECK (Correo LIKE '%_@_%._%')
    );
END;
GO

IF OBJECT_ID(N'dbo.Libros', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Libros
    (
        LibroId INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Libros PRIMARY KEY,
        Codigo VARCHAR(20) NOT NULL CONSTRAINT UQ_Libros_Codigo UNIQUE,
        ISBN VARCHAR(20) NOT NULL CONSTRAINT UQ_Libros_ISBN UNIQUE,
        Titulo NVARCHAR(200) NOT NULL,
        AutorId INT NOT NULL,
        CategoriaId INT NOT NULL,
        CantidadTotal INT NOT NULL,
        CantidadDisponible INT NOT NULL,
        Activo BIT NOT NULL CONSTRAINT DF_Libros_Activo DEFAULT 1,
        FechaRegistro DATETIME2(0) NOT NULL CONSTRAINT DF_Libros_FechaRegistro DEFAULT SYSDATETIME(),
        CONSTRAINT FK_Libros_Autores FOREIGN KEY (AutorId) REFERENCES dbo.Autores(AutorId),
        CONSTRAINT FK_Libros_Categorias FOREIGN KEY (CategoriaId) REFERENCES dbo.Categorias(CategoriaId),
        CONSTRAINT CK_Libros_Codigo CHECK (LEN(LTRIM(RTRIM(Codigo))) > 0),
        CONSTRAINT CK_Libros_ISBN CHECK (LEN(REPLACE(ISBN, '-', '')) IN (10, 13)),
        CONSTRAINT CK_Libros_Titulo CHECK (LEN(LTRIM(RTRIM(Titulo))) > 0),
        CONSTRAINT CK_Libros_CantidadTotal CHECK (CantidadTotal > 0),
        CONSTRAINT CK_Libros_CantidadDisponible CHECK (CantidadDisponible BETWEEN 0 AND CantidadTotal)
    );
END;
GO

IF OBJECT_ID(N'dbo.Prestamos', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Prestamos
    (
        PrestamoId INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Prestamos PRIMARY KEY,
        UsuarioId INT NOT NULL,
        FechaPrestamo DATETIME2(0) NOT NULL CONSTRAINT DF_Prestamos_FechaPrestamo DEFAULT SYSDATETIME(),
        FechaDevolucionEsperada DATE NOT NULL,
        FechaDevolucionReal DATETIME2(0) NULL,
        Estado VARCHAR(12) NOT NULL CONSTRAINT DF_Prestamos_Estado DEFAULT 'ACTIVO',
        CONSTRAINT FK_Prestamos_Usuarios FOREIGN KEY (UsuarioId) REFERENCES dbo.Usuarios(UsuarioId),
        CONSTRAINT CK_Prestamos_Estado CHECK (Estado IN ('ACTIVO', 'DEVUELTO', 'VENCIDO')),
        CONSTRAINT CK_Prestamos_FechaEsperada CHECK (FechaDevolucionEsperada >= CONVERT(date, FechaPrestamo)),
        CONSTRAINT CK_Prestamos_FechaReal CHECK (FechaDevolucionReal IS NULL OR FechaDevolucionReal >= FechaPrestamo)
    );
END;
GO

IF OBJECT_ID(N'dbo.DetallePrestamos', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.DetallePrestamos
    (
        DetallePrestamoId INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_DetallePrestamos PRIMARY KEY,
        PrestamoId INT NOT NULL,
        LibroId INT NOT NULL,
        Cantidad INT NOT NULL CONSTRAINT DF_DetallePrestamos_Cantidad DEFAULT 1,
        CONSTRAINT FK_DetallePrestamos_Prestamos FOREIGN KEY (PrestamoId) REFERENCES dbo.Prestamos(PrestamoId),
        CONSTRAINT FK_DetallePrestamos_Libros FOREIGN KEY (LibroId) REFERENCES dbo.Libros(LibroId),
        CONSTRAINT UQ_DetallePrestamos_PrestamoLibro UNIQUE (PrestamoId, LibroId),
        CONSTRAINT CK_DetallePrestamos_Cantidad CHECK (Cantidad > 0)
    );
END;
GO

IF OBJECT_ID(N'dbo.Devoluciones', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Devoluciones
    (
        DevolucionId INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Devoluciones PRIMARY KEY,
        PrestamoId INT NOT NULL CONSTRAINT UQ_Devoluciones_Prestamo UNIQUE,
        FechaDevolucion DATETIME2(0) NOT NULL CONSTRAINT DF_Devoluciones_Fecha DEFAULT SYSDATETIME(),
        Observacion NVARCHAR(300) NULL,
        CONSTRAINT FK_Devoluciones_Prestamos FOREIGN KEY (PrestamoId) REFERENCES dbo.Prestamos(PrestamoId)
    );
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Libros_Titulo' AND object_id = OBJECT_ID(N'dbo.Libros')) CREATE INDEX IX_Libros_Titulo ON dbo.Libros(Titulo);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Libros_AutorId' AND object_id = OBJECT_ID(N'dbo.Libros')) CREATE INDEX IX_Libros_AutorId ON dbo.Libros(AutorId);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Libros_CategoriaId' AND object_id = OBJECT_ID(N'dbo.Libros')) CREATE INDEX IX_Libros_CategoriaId ON dbo.Libros(CategoriaId);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Prestamos_Estado' AND object_id = OBJECT_ID(N'dbo.Prestamos')) CREATE INDEX IX_Prestamos_Estado ON dbo.Prestamos(Estado, FechaDevolucionEsperada);
GO

IF OBJECT_ID(N'dbo.vw_LibrosDetalle', N'V') IS NOT NULL DROP VIEW dbo.vw_LibrosDetalle;
GO
CREATE VIEW dbo.vw_LibrosDetalle
AS
SELECT l.LibroId, l.Codigo, l.ISBN, l.Titulo, a.Nombre + N' ' + a.Apellidos AS Autor,
       c.Nombre AS Categoria, l.CantidadTotal, l.CantidadDisponible,
       CASE WHEN l.CantidadDisponible > 0 THEN N'Disponible' ELSE N'Prestado' END AS Disponibilidad
FROM dbo.Libros l
INNER JOIN dbo.Autores a ON a.AutorId = l.AutorId
INNER JOIN dbo.Categorias c ON c.CategoriaId = l.CategoriaId;
GO

IF OBJECT_ID(N'dbo.vw_HistorialPrestamos', N'V') IS NOT NULL DROP VIEW dbo.vw_HistorialPrestamos;
GO
CREATE VIEW dbo.vw_HistorialPrestamos
AS
SELECT p.PrestamoId, u.Documento, u.Nombre + N' ' + u.Apellidos AS Usuario, u.Correo, u.ProgramaAcademico,
       l.Codigo, l.Titulo, d.Cantidad, p.FechaPrestamo, p.FechaDevolucionEsperada,
       p.FechaDevolucionReal, p.Estado
FROM dbo.Prestamos p
INNER JOIN dbo.Usuarios u ON u.UsuarioId = p.UsuarioId
INNER JOIN dbo.DetallePrestamos d ON d.PrestamoId = p.PrestamoId
INNER JOIN dbo.Libros l ON l.LibroId = d.LibroId;
GO

IF OBJECT_ID(N'dbo.sp_RegistrarPrestamo', N'P') IS NOT NULL DROP PROCEDURE dbo.sp_RegistrarPrestamo;
GO
CREATE PROCEDURE dbo.sp_RegistrarPrestamo
    @UsuarioId INT,
    @LibroId INT,
    @FechaDevolucionEsperada DATE
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        IF NOT EXISTS (SELECT 1 FROM dbo.Usuarios WITH (UPDLOCK, HOLDLOCK) WHERE UsuarioId = @UsuarioId AND Activo = 1)
            THROW 50001, 'El usuario no existe o está inactivo.', 1;

        IF NOT EXISTS (SELECT 1 FROM dbo.Libros WITH (UPDLOCK, HOLDLOCK) WHERE LibroId = @LibroId AND Activo = 1)
            THROW 50002, 'El libro no existe o está inactivo.', 1;

        IF (SELECT CantidadDisponible FROM dbo.Libros WITH (UPDLOCK, HOLDLOCK) WHERE LibroId = @LibroId) <= 0
            THROW 50003, 'El libro no tiene existencias disponibles.', 1;

        IF @FechaDevolucionEsperada < CONVERT(date, GETDATE())
            THROW 50004, 'La fecha de devolución esperada no puede ser anterior a hoy.', 1;

        INSERT dbo.Prestamos(UsuarioId, FechaDevolucionEsperada, Estado)
        VALUES(@UsuarioId, @FechaDevolucionEsperada, 'ACTIVO');

        DECLARE @PrestamoId INT = SCOPE_IDENTITY();
        INSERT dbo.DetallePrestamos(PrestamoId, LibroId, Cantidad) VALUES(@PrestamoId, @LibroId, 1);
        UPDATE dbo.Libros SET CantidadDisponible = CantidadDisponible - 1 WHERE LibroId = @LibroId;

        COMMIT TRANSACTION;
        SELECT @PrestamoId AS PrestamoId;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END;
GO

IF OBJECT_ID(N'dbo.sp_RegistrarDevolucion', N'P') IS NOT NULL DROP PROCEDURE dbo.sp_RegistrarDevolucion;
GO
CREATE PROCEDURE dbo.sp_RegistrarDevolucion
    @PrestamoId INT,
    @Observacion NVARCHAR(300) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        IF NOT EXISTS (SELECT 1 FROM dbo.Prestamos WITH (UPDLOCK, HOLDLOCK) WHERE PrestamoId = @PrestamoId AND Estado = 'ACTIVO')
            THROW 50005, 'El préstamo no existe o ya fue devuelto.', 1;

        UPDATE l
           SET l.CantidadDisponible = l.CantidadDisponible + d.Cantidad
        FROM dbo.Libros l
        INNER JOIN dbo.DetallePrestamos d ON d.LibroId = l.LibroId
        WHERE d.PrestamoId = @PrestamoId;

        UPDATE dbo.Prestamos SET Estado = 'DEVUELTO', FechaDevolucionReal = SYSDATETIME() WHERE PrestamoId = @PrestamoId;
        INSERT dbo.Devoluciones(PrestamoId, Observacion) VALUES(@PrestamoId, @Observacion);

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END;
GO

/* Datos de prueba idempotentes */
IF NOT EXISTS (SELECT 1 FROM dbo.Categorias WHERE Nombre = N'Programación') INSERT dbo.Categorias(Nombre,Descripcion) VALUES(N'Programación',N'Lenguajes, algoritmos y desarrollo de software');
IF NOT EXISTS (SELECT 1 FROM dbo.Categorias WHERE Nombre = N'Bases de datos') INSERT dbo.Categorias(Nombre,Descripcion) VALUES(N'Bases de datos',N'Modelado, SQL y administración de datos');
IF NOT EXISTS (SELECT 1 FROM dbo.Categorias WHERE Nombre = N'Redes') INSERT dbo.Categorias(Nombre,Descripcion) VALUES(N'Redes',N'Comunicación y redes de computadores');
IF NOT EXISTS (SELECT 1 FROM dbo.Categorias WHERE Nombre = N'Matemáticas') INSERT dbo.Categorias(Nombre,Descripcion) VALUES(N'Matemáticas',N'Fundamentos y aplicaciones matemáticas');
IF NOT EXISTS (SELECT 1 FROM dbo.Categorias WHERE Nombre = N'Electrónica') INSERT dbo.Categorias(Nombre,Descripcion) VALUES(N'Electrónica',N'Circuitos y sistemas electrónicos');
IF NOT EXISTS (SELECT 1 FROM dbo.Categorias WHERE Nombre = N'Inteligencia Artificial') INSERT dbo.Categorias(Nombre,Descripcion) VALUES(N'Inteligencia Artificial',N'Aprendizaje automático y sistemas inteligentes');
IF NOT EXISTS (SELECT 1 FROM dbo.Categorias WHERE Nombre = N'Otros') INSERT dbo.Categorias(Nombre,Descripcion) VALUES(N'Otros',N'Otras áreas del conocimiento');

IF NOT EXISTS (SELECT 1 FROM dbo.Autores WHERE Codigo = 'AUT-001') INSERT dbo.Autores(Codigo,Nombre,Apellidos,Nacionalidad,FechaNacimiento) VALUES('AUT-001',N'Robert',N'Martin',N'Estadounidense','1952-12-05');
IF NOT EXISTS (SELECT 1 FROM dbo.Autores WHERE Codigo = 'AUT-002') INSERT dbo.Autores(Codigo,Nombre,Apellidos,Nacionalidad,FechaNacimiento) VALUES('AUT-002',N'Andrew',N'Tanenbaum',N'Estadounidense','1944-03-16');
IF NOT EXISTS (SELECT 1 FROM dbo.Autores WHERE Codigo = 'AUT-003') INSERT dbo.Autores(Codigo,Nombre,Apellidos,Nacionalidad,FechaNacimiento) VALUES('AUT-003',N'Ian',N'Sommerville',N'Británico','1951-02-23');

IF NOT EXISTS (SELECT 1 FROM dbo.Usuarios WHERE Documento = '1032456789') INSERT dbo.Usuarios(Documento,Nombre,Apellidos,Telefono,Correo,ProgramaAcademico) VALUES('1032456789',N'Laura',N'Gómez','3005551101','laura.gomez@institucion.edu.co',N'Ingeniería de Sistemas');
IF NOT EXISTS (SELECT 1 FROM dbo.Usuarios WHERE Documento = '1019988776') INSERT dbo.Usuarios(Documento,Nombre,Apellidos,Telefono,Correo,ProgramaAcademico) VALUES('1019988776',N'Carlos',N'Ramírez','3155552410','carlos.ramirez@institucion.edu.co',N'Ingeniería Electrónica');
IF NOT EXISTS (SELECT 1 FROM dbo.Usuarios WHERE Documento = '1001234567') INSERT dbo.Usuarios(Documento,Nombre,Apellidos,Telefono,Correo,ProgramaAcademico) VALUES('1001234567',N'Valentina',N'Torres','3205558741','valentina.torres@institucion.edu.co',N'Matemáticas');

DECLARE @AutorMartin INT = (SELECT AutorId FROM dbo.Autores WHERE Codigo='AUT-001');
DECLARE @AutorTanenbaum INT = (SELECT AutorId FROM dbo.Autores WHERE Codigo='AUT-002');
DECLARE @AutorSommerville INT = (SELECT AutorId FROM dbo.Autores WHERE Codigo='AUT-003');
DECLARE @CatProgramacion INT = (SELECT CategoriaId FROM dbo.Categorias WHERE Nombre=N'Programación');
DECLARE @CatDatos INT = (SELECT CategoriaId FROM dbo.Categorias WHERE Nombre=N'Bases de datos');
DECLARE @CatRedes INT = (SELECT CategoriaId FROM dbo.Categorias WHERE Nombre=N'Redes');

IF NOT EXISTS (SELECT 1 FROM dbo.Libros WHERE ISBN='9780132350884') INSERT dbo.Libros(Codigo,ISBN,Titulo,AutorId,CategoriaId,CantidadTotal,CantidadDisponible) VALUES('LIB-001','9780132350884',N'Código limpio',@AutorMartin,@CatProgramacion,5,5);
IF NOT EXISTS (SELECT 1 FROM dbo.Libros WHERE ISBN='9780132126953') INSERT dbo.Libros(Codigo,ISBN,Titulo,AutorId,CategoriaId,CantidadTotal,CantidadDisponible) VALUES('LIB-002','9780132126953',N'Redes de computadoras',@AutorTanenbaum,@CatRedes,3,3);
IF NOT EXISTS (SELECT 1 FROM dbo.Libros WHERE ISBN='9780137035151') INSERT dbo.Libros(Codigo,ISBN,Titulo,AutorId,CategoriaId,CantidadTotal,CantidadDisponible) VALUES('LIB-003','9780137035151',N'Ingeniería de software',@AutorSommerville,@CatProgramacion,4,4);
IF NOT EXISTS (SELECT 1 FROM dbo.Libros WHERE ISBN='9780321197849') INSERT dbo.Libros(Codigo,ISBN,Titulo,AutorId,CategoriaId,CantidadTotal,CantidadDisponible) VALUES('LIB-004','9780321197849',N'Fundamentos de bases de datos',@AutorTanenbaum,@CatDatos,2,2);
GO

/* Préstamos de demostración idempotentes */
DECLARE @UsuarioLaura INT = (SELECT UsuarioId FROM dbo.Usuarios WHERE Documento='1032456789');
DECLARE @UsuarioValentina INT = (SELECT UsuarioId FROM dbo.Usuarios WHERE Documento='1001234567');
DECLARE @LibroLimpio INT = (SELECT LibroId FROM dbo.Libros WHERE Codigo='LIB-001');
DECLARE @LibroSoftware INT = (SELECT LibroId FROM dbo.Libros WHERE Codigo='LIB-003');
DECLARE @FechaEsperada DATE = DATEADD(DAY, 10, CONVERT(date, GETDATE()));

IF NOT EXISTS
(
    SELECT 1
    FROM dbo.Prestamos p
    INNER JOIN dbo.DetallePrestamos d ON d.PrestamoId = p.PrestamoId
    WHERE p.UsuarioId = @UsuarioLaura AND d.LibroId = @LibroLimpio AND p.Estado = 'ACTIVO'
)
    EXEC dbo.sp_RegistrarPrestamo @UsuarioLaura, @LibroLimpio, @FechaEsperada;

IF NOT EXISTS
(
    SELECT 1
    FROM dbo.Prestamos p
    INNER JOIN dbo.DetallePrestamos d ON d.PrestamoId = p.PrestamoId
    WHERE p.UsuarioId = @UsuarioValentina AND d.LibroId = @LibroSoftware AND p.Estado = 'ACTIVO'
)
    EXEC dbo.sp_RegistrarPrestamo @UsuarioValentina, @LibroSoftware, @FechaEsperada;
GO

PRINT N'BibliotecaDB creada y cargada correctamente.';
GO
