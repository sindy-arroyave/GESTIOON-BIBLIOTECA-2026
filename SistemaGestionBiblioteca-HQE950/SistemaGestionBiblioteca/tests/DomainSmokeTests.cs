using System;
using System.Collections.Generic;
using Biblioteca.Entidades;
using Biblioteca.Negocio;

internal static class DomainSmokeTests
{
    private static int _failures;

    private static void Check(bool condition, string name)
    {
        Console.WriteLine((condition ? "PASS " : "FAIL ") + name);
        if (!condition) _failures++;
    }

    private static int Main()
    {
        Check(Validador.ValidarCorreo("ana@institucion.edu.co") == string.Empty, "correo válido");
        Check(Validador.ValidarCorreo("correo-invalido") != string.Empty, "correo inválido");
        Check(Validador.ValidarTelefono("300-555-1212") == string.Empty, "teléfono válido");
        Check(Validador.ValidarTelefono("12") != string.Empty, "teléfono inválido");
        Check(Validador.ValidarIsbn("978-0132350884") == string.Empty, "ISBN de 13 dígitos");
        Check(Validador.ValidarIsbn("123") != string.Empty, "ISBN inválido");

        Libro libro = new Libro { Codigo = "", Isbn = "123", Titulo = "", AutorId = 0, CategoriaId = 0, CantidadTotal = 0, CantidadDisponible = 0 };
        Check(libro.Validar().Count >= 5, "validaciones obligatorias de libro");

        Prestamo prestamo = new Prestamo { UsuarioId = 0, FechaDevolucionEsperada = DateTime.Today.AddDays(-1) };
        Check(prestamo.Validar().Count >= 3, "validaciones obligatorias de préstamo");

        return _failures == 0 ? 0 : 1;
    }
}

