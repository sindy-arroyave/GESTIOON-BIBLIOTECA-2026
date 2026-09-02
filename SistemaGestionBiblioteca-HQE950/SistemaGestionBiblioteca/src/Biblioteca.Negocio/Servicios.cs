using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Mail;
using System.Text.RegularExpressions;
using Biblioteca.Datos;
using Biblioteca.Entidades;

namespace Biblioteca.Negocio
{
    public sealed class ValidacionException : ApplicationException
    {
        public IList<string> Errores { get; private set; }
        public ValidacionException(IEnumerable<string> errores) : base(string.Join(Environment.NewLine, errores))
        {
            Errores = errores.ToList();
        }
    }

    public static class Validador
    {
        public static string Texto(string value) { return (value ?? string.Empty).Trim(); }
        public static void LanzarSiHayErrores(IValidable validable, params string[] adicionales)
        {
            IList<string> errores = validable.Validar();
            foreach (string error in adicionales) if (!string.IsNullOrWhiteSpace(error)) errores.Add(error);
            if (errores.Count > 0) throw new ValidacionException(errores);
        }
        public static string ValidarCorreo(string correo)
        {
            try { new MailAddress(correo); return string.Empty; }
            catch { return "El correo electrónico no tiene un formato válido."; }
        }
        public static string ValidarTelefono(string telefono)
        {
            return Regex.IsMatch(telefono ?? string.Empty, @"^[0-9+()\-\s]{7,20}$") ? string.Empty : "El teléfono debe contener entre 7 y 20 caracteres numéricos válidos.";
        }
        public static string ValidarIsbn(string isbn)
        {
            string limpio = Regex.Replace(isbn ?? string.Empty, "[- ]", string.Empty);
            return Regex.IsMatch(limpio, @"^(\d{10}|\d{13})$") ? string.Empty : "El ISBN debe contener 10 o 13 dígitos.";
        }
    }

    public abstract class ServicioBase
    {
        protected readonly SqlDatabase Db;
        protected ServicioBase() { Db = new SqlDatabase(); }
    }

    public sealed class AutorServicio : ServicioBase
    {
        private readonly AutorRepositorio _repo;
        public AutorServicio() { _repo = new AutorRepositorio(Db); }
        public DataTable Listar() { return _repo.Listar(); }
        public void Guardar(int id, string codigo, string nombre, string apellidos, string nacionalidad, DateTime nacimiento)
        {
            Autor a = new Autor { Id = id, Codigo = Validador.Texto(codigo), Nombre = Validador.Texto(nombre), Apellidos = Validador.Texto(apellidos), Nacionalidad = Validador.Texto(nacionalidad), FechaNacimiento = nacimiento };
            Validador.LanzarSiHayErrores(a);
            _repo.Guardar(a);
        }
        public void Eliminar(int id) { if (id <= 0) throw new ValidacionException(new[] { "Seleccione un autor." }); _repo.Eliminar(id); }
    }

    public sealed class CategoriaServicio : ServicioBase
    {
        private readonly CategoriaRepositorio _repo;
        public CategoriaServicio() { _repo = new CategoriaRepositorio(Db); }
        public DataTable Listar() { return _repo.Listar(); }
        public void Guardar(int id, string nombre, string descripcion)
        {
            Categoria c = new Categoria { Id = id, Nombre = Validador.Texto(nombre), Descripcion = Validador.Texto(descripcion) };
            Validador.LanzarSiHayErrores(c);
            _repo.Guardar(c);
        }
        public void Eliminar(int id) { if (id <= 0) throw new ValidacionException(new[] { "Seleccione una categoría." }); _repo.Eliminar(id); }
    }

    public sealed class UsuarioServicio : ServicioBase
    {
        private readonly UsuarioRepositorio _repo;
        public UsuarioServicio() { _repo = new UsuarioRepositorio(Db); }
        public DataTable Listar() { return _repo.Listar(); }
        public void Guardar(int id, string documento, string nombre, string apellidos, string telefono, string correo, string programa)
        {
            Usuario u = new Usuario { Id = id, Documento = Validador.Texto(documento), Nombre = Validador.Texto(nombre), Apellidos = Validador.Texto(apellidos), Telefono = Validador.Texto(telefono), Correo = Validador.Texto(correo), ProgramaAcademico = Validador.Texto(programa) };
            Validador.LanzarSiHayErrores(u, Validador.ValidarCorreo(u.Correo), Validador.ValidarTelefono(u.Telefono));
            _repo.Guardar(u);
        }
        public void Eliminar(int id) { if (id <= 0) throw new ValidacionException(new[] { "Seleccione un usuario." }); _repo.Eliminar(id); }
    }

    public sealed class LibroServicio : ServicioBase
    {
        private readonly LibroRepositorio _repo;
        public LibroServicio() { _repo = new LibroRepositorio(Db); }
        public DataTable Listar(string filtro) { return _repo.Listar(Validador.Texto(filtro)); }
        public void Guardar(int id, string codigo, string isbn, string titulo, int autorId, int categoriaId, int cantidad)
        {
            Libro l = new Libro { Id = id, Codigo = Validador.Texto(codigo), Isbn = Validador.Texto(isbn), Titulo = Validador.Texto(titulo), AutorId = autorId, CategoriaId = categoriaId, CantidadTotal = cantidad, CantidadDisponible = cantidad };
            Validador.LanzarSiHayErrores(l, Validador.ValidarIsbn(l.Isbn));
            int rows = _repo.Guardar(l);
            if (rows == 0) throw new ValidacionException(new[] { "La cantidad total no puede ser menor que el número de ejemplares actualmente prestados." });
        }
        public void Eliminar(int id) { if (id <= 0) throw new ValidacionException(new[] { "Seleccione un libro." }); _repo.Eliminar(id); }
    }

    public sealed class PrestamoServicio : ServicioBase
    {
        private readonly PrestamoRepositorio _repo;
        public PrestamoServicio() { _repo = new PrestamoRepositorio(Db); }
        public DataTable Activos() { return _repo.Activos(); }
        public int Registrar(int usuarioId, int libroId, DateTime fechaEsperada)
        {
            Prestamo p = new Prestamo { UsuarioId = usuarioId, FechaDevolucionEsperada = fechaEsperada };
            p.Detalles.Add(new DetallePrestamo { LibroId = libroId, Cantidad = 1 });
            Validador.LanzarSiHayErrores(p);
            return _repo.Registrar(usuarioId, libroId, fechaEsperada);
        }
        public void Devolver(int prestamoId)
        {
            if (prestamoId <= 0) throw new ValidacionException(new[] { "Seleccione un préstamo activo." });
            _repo.RegistrarDevolucion(prestamoId);
        }
    }

    public enum TipoConsulta { LibrosDisponibles, LibrosPrestados, UsuariosConPrestamos, HistorialPrestamos, LibrosPorCategoria, CantidadPrestados }

    public sealed class ConsultaServicio : ServicioBase
    {
        private readonly ConsultaRepositorio _repo;
        public ConsultaServicio() { _repo = new ConsultaRepositorio(Db); }
        public DataTable Ejecutar(TipoConsulta tipo)
        {
            switch (tipo)
            {
                case TipoConsulta.LibrosDisponibles: return _repo.LibrosDisponibles();
                case TipoConsulta.LibrosPrestados: return _repo.LibrosPrestados();
                case TipoConsulta.UsuariosConPrestamos: return _repo.UsuariosConPrestamos();
                case TipoConsulta.HistorialPrestamos: return _repo.Historial();
                case TipoConsulta.LibrosPorCategoria: return _repo.LibrosPorCategoria();
                default: return _repo.CantidadPrestados();
            }
        }
        public DataTable Resumen() { return _repo.Resumen(); }
    }
}
