using System;
using System.Collections.Generic;

namespace Biblioteca.Entidades
{
    public interface IValidable
    {
        IList<string> Validar();
    }

    public abstract class EntidadBase : IValidable
    {
        public int Id { get; set; }
        public abstract IList<string> Validar();
    }

    public abstract class Persona : EntidadBase
    {
        public string Nombre { get; set; }
        public string Apellidos { get; set; }
        public string NombreCompleto { get { return (Nombre + " " + Apellidos).Trim(); } }

        protected void ValidarNombre(IList<string> errores)
        {
            if (string.IsNullOrWhiteSpace(Nombre)) errores.Add("El nombre es obligatorio.");
            if (string.IsNullOrWhiteSpace(Apellidos)) errores.Add("Los apellidos son obligatorios.");
        }
    }

    public sealed class Autor : Persona
    {
        public string Codigo { get; set; }
        public string Nacionalidad { get; set; }
        public DateTime FechaNacimiento { get; set; }

        public override IList<string> Validar()
        {
            IList<string> errores = new List<string>();
            if (string.IsNullOrWhiteSpace(Codigo)) errores.Add("El código del autor es obligatorio.");
            ValidarNombre(errores);
            if (FechaNacimiento.Date > DateTime.Today) errores.Add("La fecha de nacimiento no puede estar en el futuro.");
            return errores;
        }
    }

    public sealed class Categoria : EntidadBase
    {
        public string Nombre { get; set; }
        public string Descripcion { get; set; }

        public override IList<string> Validar()
        {
            IList<string> errores = new List<string>();
            if (string.IsNullOrWhiteSpace(Nombre)) errores.Add("El nombre de la categoría es obligatorio.");
            return errores;
        }
    }

    public sealed class Usuario : Persona
    {
        public string Documento { get; set; }
        public string Telefono { get; set; }
        public string Correo { get; set; }
        public string ProgramaAcademico { get; set; }

        public override IList<string> Validar()
        {
            IList<string> errores = new List<string>();
            if (string.IsNullOrWhiteSpace(Documento)) errores.Add("El documento es obligatorio.");
            ValidarNombre(errores);
            if (string.IsNullOrWhiteSpace(ProgramaAcademico)) errores.Add("El programa académico es obligatorio.");
            return errores;
        }
    }

    public sealed class Libro : EntidadBase
    {
        public string Codigo { get; set; }
        public string Isbn { get; set; }
        public string Titulo { get; set; }
        public int AutorId { get; set; }
        public int CategoriaId { get; set; }
        public int CantidadTotal { get; set; }
        public int CantidadDisponible { get; set; }

        public bool Disponible { get { return CantidadDisponible > 0; } }

        public override IList<string> Validar()
        {
            IList<string> errores = new List<string>();
            if (string.IsNullOrWhiteSpace(Codigo)) errores.Add("El código del libro es obligatorio.");
            if (string.IsNullOrWhiteSpace(Isbn)) errores.Add("El ISBN es obligatorio.");
            if (string.IsNullOrWhiteSpace(Titulo)) errores.Add("El título es obligatorio.");
            if (AutorId <= 0) errores.Add("Debe seleccionar un autor.");
            if (CategoriaId <= 0) errores.Add("Debe seleccionar una categoría.");
            if (CantidadTotal <= 0) errores.Add("La cantidad debe ser mayor que cero.");
            if (CantidadDisponible < 0 || CantidadDisponible > CantidadTotal) errores.Add("La disponibilidad es inválida.");
            return errores;
        }
    }

    public enum EstadoPrestamo
    {
        Activo,
        Devuelto,
        Vencido
    }

    public sealed class DetallePrestamo
    {
        public int Id { get; set; }
        public int PrestamoId { get; set; }
        public int LibroId { get; set; }
        public int Cantidad { get; set; }
    }

    public sealed class Prestamo : EntidadBase
    {
        public int UsuarioId { get; set; }
        public DateTime FechaPrestamo { get; set; }
        public DateTime FechaDevolucionEsperada { get; set; }
        public DateTime? FechaDevolucionReal { get; set; }
        public EstadoPrestamo Estado { get; set; }
        public IList<DetallePrestamo> Detalles { get; private set; }

        public Prestamo()
        {
            FechaPrestamo = DateTime.Now;
            Estado = EstadoPrestamo.Activo;
            Detalles = new List<DetallePrestamo>();
        }

        public override IList<string> Validar()
        {
            IList<string> errores = new List<string>();
            if (UsuarioId <= 0) errores.Add("Debe seleccionar un usuario existente.");
            if (FechaDevolucionEsperada.Date < FechaPrestamo.Date) errores.Add("La devolución esperada no puede ser anterior al préstamo.");
            if (Detalles.Count == 0) errores.Add("El préstamo debe contener al menos un libro.");
            foreach (DetallePrestamo detalle in Detalles)
                if (detalle.LibroId <= 0 || detalle.Cantidad <= 0) errores.Add("El detalle del préstamo es inválido.");
            return errores;
        }
    }
}

