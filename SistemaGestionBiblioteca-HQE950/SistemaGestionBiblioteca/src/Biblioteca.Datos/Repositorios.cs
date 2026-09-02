using System;
using System.Data;
using Biblioteca.Entidades;

namespace Biblioteca.Datos
{
    public abstract class RepositorioBase
    {
        protected readonly SqlDatabase Db;
        protected RepositorioBase(SqlDatabase db) { Db = db; }
    }

    public sealed class AutorRepositorio : RepositorioBase
    {
        public AutorRepositorio(SqlDatabase db) : base(db) { }
        public DataTable Listar() { return Db.Query("SELECT AutorId, Codigo, Nombre, Apellidos, Nacionalidad, FechaNacimiento FROM dbo.Autores ORDER BY Apellidos, Nombre", CommandType.Text); }
        public int Guardar(Autor a)
        {
            if (a.Id == 0)
                return Db.Execute("INSERT dbo.Autores(Codigo,Nombre,Apellidos,Nacionalidad,FechaNacimiento) VALUES(@Codigo,@Nombre,@Apellidos,@Nacionalidad,@Fecha)", CommandType.Text, SqlDatabase.Param("@Codigo", a.Codigo), SqlDatabase.Param("@Nombre", a.Nombre), SqlDatabase.Param("@Apellidos", a.Apellidos), SqlDatabase.Param("@Nacionalidad", a.Nacionalidad), SqlDatabase.Param("@Fecha", a.FechaNacimiento.Date));
            return Db.Execute("UPDATE dbo.Autores SET Codigo=@Codigo,Nombre=@Nombre,Apellidos=@Apellidos,Nacionalidad=@Nacionalidad,FechaNacimiento=@Fecha WHERE AutorId=@Id", CommandType.Text, SqlDatabase.Param("@Codigo", a.Codigo), SqlDatabase.Param("@Nombre", a.Nombre), SqlDatabase.Param("@Apellidos", a.Apellidos), SqlDatabase.Param("@Nacionalidad", a.Nacionalidad), SqlDatabase.Param("@Fecha", a.FechaNacimiento.Date), SqlDatabase.Param("@Id", a.Id));
        }
        public int Eliminar(int id) { return Db.Execute("DELETE dbo.Autores WHERE AutorId=@Id", CommandType.Text, SqlDatabase.Param("@Id", id)); }
    }

    public sealed class CategoriaRepositorio : RepositorioBase
    {
        public CategoriaRepositorio(SqlDatabase db) : base(db) { }
        public DataTable Listar() { return Db.Query("SELECT CategoriaId, Nombre, Descripcion FROM dbo.Categorias ORDER BY Nombre", CommandType.Text); }
        public int Guardar(Categoria c)
        {
            if (c.Id == 0) return Db.Execute("INSERT dbo.Categorias(Nombre,Descripcion) VALUES(@Nombre,@Descripcion)", CommandType.Text, SqlDatabase.Param("@Nombre", c.Nombre), SqlDatabase.Param("@Descripcion", c.Descripcion));
            return Db.Execute("UPDATE dbo.Categorias SET Nombre=@Nombre,Descripcion=@Descripcion WHERE CategoriaId=@Id", CommandType.Text, SqlDatabase.Param("@Nombre", c.Nombre), SqlDatabase.Param("@Descripcion", c.Descripcion), SqlDatabase.Param("@Id", c.Id));
        }
        public int Eliminar(int id) { return Db.Execute("DELETE dbo.Categorias WHERE CategoriaId=@Id", CommandType.Text, SqlDatabase.Param("@Id", id)); }
    }

    public sealed class UsuarioRepositorio : RepositorioBase
    {
        public UsuarioRepositorio(SqlDatabase db) : base(db) { }
        public DataTable Listar() { return Db.Query("SELECT UsuarioId, Documento, Nombre, Apellidos, Telefono, Correo, ProgramaAcademico FROM dbo.Usuarios ORDER BY Apellidos, Nombre", CommandType.Text); }
        public int Guardar(Usuario u)
        {
            if (u.Id == 0)
                return Db.Execute("INSERT dbo.Usuarios(Documento,Nombre,Apellidos,Telefono,Correo,ProgramaAcademico) VALUES(@Documento,@Nombre,@Apellidos,@Telefono,@Correo,@Programa)", CommandType.Text, SqlDatabase.Param("@Documento", u.Documento), SqlDatabase.Param("@Nombre", u.Nombre), SqlDatabase.Param("@Apellidos", u.Apellidos), SqlDatabase.Param("@Telefono", u.Telefono), SqlDatabase.Param("@Correo", u.Correo), SqlDatabase.Param("@Programa", u.ProgramaAcademico));
            return Db.Execute("UPDATE dbo.Usuarios SET Documento=@Documento,Nombre=@Nombre,Apellidos=@Apellidos,Telefono=@Telefono,Correo=@Correo,ProgramaAcademico=@Programa WHERE UsuarioId=@Id", CommandType.Text, SqlDatabase.Param("@Documento", u.Documento), SqlDatabase.Param("@Nombre", u.Nombre), SqlDatabase.Param("@Apellidos", u.Apellidos), SqlDatabase.Param("@Telefono", u.Telefono), SqlDatabase.Param("@Correo", u.Correo), SqlDatabase.Param("@Programa", u.ProgramaAcademico), SqlDatabase.Param("@Id", u.Id));
        }
        public int Eliminar(int id) { return Db.Execute("DELETE dbo.Usuarios WHERE UsuarioId=@Id", CommandType.Text, SqlDatabase.Param("@Id", id)); }
    }

    public sealed class LibroRepositorio : RepositorioBase
    {
        public LibroRepositorio(SqlDatabase db) : base(db) { }
        public DataTable Listar(string filtro)
        {
            const string sql = @"SELECT l.LibroId,l.Codigo,l.ISBN,l.Titulo,l.AutorId,a.Nombre+' '+a.Apellidos AS Autor,l.CategoriaId,c.Nombre AS Categoria,l.CantidadTotal,l.CantidadDisponible,CASE WHEN l.CantidadDisponible>0 THEN 'Disponible' ELSE 'Prestado' END AS Disponibilidad FROM dbo.Libros l INNER JOIN dbo.Autores a ON a.AutorId=l.AutorId INNER JOIN dbo.Categorias c ON c.CategoriaId=l.CategoriaId WHERE @Filtro='' OR l.Codigo LIKE '%'+@Filtro+'%' OR l.Titulo LIKE '%'+@Filtro+'%' OR a.Nombre+' '+a.Apellidos LIKE '%'+@Filtro+'%' OR c.Nombre LIKE '%'+@Filtro+'%' ORDER BY l.Titulo";
            return Db.Query(sql, CommandType.Text, SqlDatabase.Param("@Filtro", filtro ?? string.Empty));
        }
        public int Guardar(Libro l)
        {
            if (l.Id == 0)
                return Db.Execute("INSERT dbo.Libros(Codigo,ISBN,Titulo,AutorId,CategoriaId,CantidadTotal,CantidadDisponible) VALUES(@Codigo,@ISBN,@Titulo,@AutorId,@CategoriaId,@Total,@Disponible)", CommandType.Text, SqlDatabase.Param("@Codigo", l.Codigo), SqlDatabase.Param("@ISBN", l.Isbn), SqlDatabase.Param("@Titulo", l.Titulo), SqlDatabase.Param("@AutorId", l.AutorId), SqlDatabase.Param("@CategoriaId", l.CategoriaId), SqlDatabase.Param("@Total", l.CantidadTotal), SqlDatabase.Param("@Disponible", l.CantidadDisponible));
            const string update = @"UPDATE dbo.Libros SET Codigo=@Codigo,ISBN=@ISBN,Titulo=@Titulo,AutorId=@AutorId,CategoriaId=@CategoriaId,CantidadDisponible=CantidadDisponible+(@Total-CantidadTotal),CantidadTotal=@Total WHERE LibroId=@Id AND CantidadDisponible+(@Total-CantidadTotal)>=0";
            return Db.Execute(update, CommandType.Text, SqlDatabase.Param("@Codigo", l.Codigo), SqlDatabase.Param("@ISBN", l.Isbn), SqlDatabase.Param("@Titulo", l.Titulo), SqlDatabase.Param("@AutorId", l.AutorId), SqlDatabase.Param("@CategoriaId", l.CategoriaId), SqlDatabase.Param("@Total", l.CantidadTotal), SqlDatabase.Param("@Id", l.Id));
        }
        public int Eliminar(int id) { return Db.Execute("DELETE dbo.Libros WHERE LibroId=@Id", CommandType.Text, SqlDatabase.Param("@Id", id)); }
    }

    public sealed class PrestamoRepositorio : RepositorioBase
    {
        public PrestamoRepositorio(SqlDatabase db) : base(db) { }
        public int Registrar(int usuarioId, int libroId, DateTime fechaEsperada)
        {
            object result = Db.Scalar("dbo.sp_RegistrarPrestamo", CommandType.StoredProcedure, SqlDatabase.Param("@UsuarioId", usuarioId), SqlDatabase.Param("@LibroId", libroId), SqlDatabase.Param("@FechaDevolucionEsperada", fechaEsperada.Date));
            return Convert.ToInt32(result);
        }
        public void RegistrarDevolucion(int prestamoId)
        {
            Db.Execute("dbo.sp_RegistrarDevolucion", CommandType.StoredProcedure, SqlDatabase.Param("@PrestamoId", prestamoId));
        }
        public DataTable Activos()
        {
            return Db.Query(@"SELECT p.PrestamoId,u.Documento,u.Nombre+' '+u.Apellidos AS Usuario,l.Codigo,l.Titulo,p.FechaPrestamo,p.FechaDevolucionEsperada,p.Estado FROM dbo.Prestamos p INNER JOIN dbo.Usuarios u ON u.UsuarioId=p.UsuarioId INNER JOIN dbo.DetallePrestamos d ON d.PrestamoId=p.PrestamoId INNER JOIN dbo.Libros l ON l.LibroId=d.LibroId WHERE p.Estado='ACTIVO' ORDER BY p.FechaDevolucionEsperada", CommandType.Text);
        }
    }

    public sealed class ConsultaRepositorio : RepositorioBase
    {
        public ConsultaRepositorio(SqlDatabase db) : base(db) { }
        public DataTable LibrosDisponibles() { return Db.Query("SELECT Codigo,ISBN,Titulo,Autor,Categoria,CantidadDisponible FROM dbo.vw_LibrosDetalle WHERE CantidadDisponible>0 ORDER BY Titulo", CommandType.Text); }
        public DataTable LibrosPrestados() { return Db.Query("SELECT PrestamoId,Documento,Usuario,Codigo,Titulo,FechaPrestamo,FechaDevolucionEsperada,Estado FROM dbo.vw_HistorialPrestamos WHERE Estado='ACTIVO' ORDER BY FechaDevolucionEsperada", CommandType.Text); }
        public DataTable UsuariosConPrestamos() { return Db.Query("SELECT DISTINCT Documento,Usuario,Correo,ProgramaAcademico FROM dbo.vw_HistorialPrestamos WHERE Estado='ACTIVO' ORDER BY Usuario", CommandType.Text); }
        public DataTable Historial() { return Db.Query("SELECT * FROM dbo.vw_HistorialPrestamos ORDER BY FechaPrestamo DESC", CommandType.Text); }
        public DataTable LibrosPorCategoria() { return Db.Query("SELECT c.Nombre AS Categoria,COUNT(l.LibroId) AS Titulos,SUM(ISNULL(l.CantidadTotal,0)) AS Ejemplares FROM dbo.Categorias c LEFT JOIN dbo.Libros l ON l.CategoriaId=c.CategoriaId GROUP BY c.Nombre ORDER BY c.Nombre", CommandType.Text); }
        public DataTable CantidadPrestados() { return Db.Query("SELECT COUNT(*) AS PrestamosActivos,ISNULL(SUM(d.Cantidad),0) AS LibrosPrestados FROM dbo.Prestamos p INNER JOIN dbo.DetallePrestamos d ON d.PrestamoId=p.PrestamoId WHERE p.Estado='ACTIVO'", CommandType.Text); }
        public DataTable Resumen() { return Db.Query("SELECT (SELECT COUNT(*) FROM dbo.Libros) AS Titulos,(SELECT ISNULL(SUM(CantidadDisponible),0) FROM dbo.Libros) AS Disponibles,(SELECT COUNT(*) FROM dbo.Usuarios) AS Usuarios,(SELECT COUNT(*) FROM dbo.Prestamos WHERE Estado='ACTIVO') AS PrestamosActivos", CommandType.Text); }
    }
}
