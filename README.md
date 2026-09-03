[README.md](https://github.com/user-attachments/files/31812438/README.md)
# Sistema de Gestión de Biblioteca

Aplicación de escritorio en C# y SQL Server para administrar libros, autores, categorías, usuarios, préstamos, devoluciones y consultas de una biblioteca académica. El proyecto aplica programación orientada a objetos, arquitectura por capas, operaciones CRUD, validaciones, manejo de excepciones y control de versiones con Git.

## Entregables incluidos

- `docs/INFORME.md`: documento académico completo solicitado.
- `docs/INFORME.pdf`: versión lista para imprimir con numeración de páginas.
- `SistemaGestionBiblioteca.sln`: solución de Visual Studio.
- `src/`: código fuente dividido en cuatro capas.
- `sql/01_Crear_BibliotecaDB.sql`: creación de base de datos, tablas, relaciones, vistas, procedimientos y datos de prueba.
- `sql/02_Pruebas_Integridad.sql`: pruebas transaccionales y consultas de verificación.
- `docs/capturas/`: evidencias visuales generadas desde la aplicación compilada.
- `docs/diagramas/`: arquitectura, diagrama de clases y modelo entidad-relación.
- `tests/`: pruebas automáticas de dominio y verificación estructural.
- `tools/`: compilación, capturas, diagramas y configuración de SQL Server.

## Funcionalidades

- CRUD de libros con búsqueda por código, título, autor o categoría.
- CRUD de autores, categorías y usuarios.
- Registro de préstamos con verificación transaccional de usuario, libro y existencias.
- Registro de devoluciones con restauración automática de disponibilidad.
- Prevención de códigos, ISBN, documentos, correos y categorías duplicados.
- Validación de campos obligatorios, correo, teléfono, ISBN, fechas y cantidades.
- Consultas de libros disponibles, libros prestados, usuarios con préstamos, historial, libros por categoría y cantidad prestada.
- Mensajes claros ante validaciones, duplicados, conflictos de integridad y errores de conexión.

## Arquitectura

```text
Biblioteca.Presentacion  -> Windows Forms y experiencia de usuario
Biblioteca.Negocio       -> servicios, reglas y validaciones
Biblioteca.Datos         -> repositorios, ADO.NET y consultas parametrizadas
Biblioteca.Entidades     -> clases de dominio y encapsulamiento
BibliotecaDB             -> SQL Server, integridad y transacciones
```

La presentación nunca ejecuta SQL directamente. Los formularios llaman servicios de negocio; los servicios validan entidades y delegan a repositorios; los repositorios acceden a SQL Server mediante `System.Data.SqlClient`.

## Requisitos

- Windows 10 u 11.
- SQL Server 2012 o superior, SQL Server Express o LocalDB.
- SQL Server Management Studio recomendado.
- Visual Studio con la carga de trabajo Desarrollo de escritorio de .NET y .NET Framework 4.8.
- PowerShell 5.1 o superior para los scripts auxiliares.

## Instalación de la base de datos

1. Abra `sql/01_Crear_BibliotecaDB.sql` en SQL Server Management Studio.
2. Conéctese a la instancia que utilizará la aplicación.
3. Ejecute el script completo. Es idempotente para los datos de catálogo incluidos.
4. Confirme el mensaje `BibliotecaDB creada y cargada correctamente`.
5. Opcionalmente ejecute `sql/02_Pruebas_Integridad.sql`. Las operaciones de prueba usan `ROLLBACK`, por lo que no dejan datos residuales.

Si tiene `sqlcmd`, también puede ejecutar:

```powershell
.\tools\ConfigurarBaseDatos.ps1 -Servidor 'localhost\SQLEXPRESS'
```

## Configuración de conexión

La cadena está en `src/Biblioteca.Presentacion/App.config`:

```xml
<add name="BibliotecaDb"
     providerName="System.Data.SqlClient"
     connectionString="Data Source=localhost\SQLEXPRESS;Initial Catalog=BibliotecaDB;Integrated Security=True;Connect Timeout=15;TrustServerCertificate=True" />
```

El proyecto está configurado para SQL Server Express:

```text
Data Source=localhost\SQLEXPRESS;Initial Catalog=BibliotecaDB;Integrated Security=True;TrustServerCertificate=True
```

No se almacenan contraseñas en el repositorio.

## Compilar y ejecutar

### Visual Studio

1. Abra `SistemaGestionBiblioteca.sln`.
2. Si Visual Studio ofrece cambiar el framework de destino, seleccione una versión de .NET Framework instalada y aplique el cambio a los cuatro proyectos.
3. Establezca `Biblioteca.Presentacion` como proyecto de inicio.
4. Compile la solución.
5. Ejecute el proyecto y verifique la conexión a `BibliotecaDB`.

### PowerShell

El proyecto incluye un proceso reproducible que compila las cuatro capas y genera las capturas:

```powershell
.\tools\BuildAndCapture.ps1
```

El ejecutable resultante queda en:

```text
build\app\Biblioteca.Presentacion.exe
```

## Pruebas

Ejecute:

```powershell
.\tests\RunTests.ps1
```

La prueba automatizada verifica:

- compilación de las cuatro capas;
- correos, teléfonos e ISBN válidos e inválidos;
- reglas obligatorias de libros y préstamos;
- presencia de las seis tablas mínimas;
- procedimientos de préstamo y devolución;
- transacciones y reversión ante error;
- existencia de al menos cinco capturas.

Para validar contra SQL Server, ejecute además `sql/02_Pruebas_Integridad.sql`.

## Estructura

```text
SistemaGestionBiblioteca/
|-- SistemaGestionBiblioteca.sln
|-- README.md
|-- docs/
|   |-- INFORME.md
|   |-- INFORME.pdf
|   |-- capturas/
|   `-- diagramas/
|-- sql/
|   |-- 01_Crear_BibliotecaDB.sql
|   `-- 02_Pruebas_Integridad.sql
|-- src/
|   |-- Biblioteca.Entidades/
|   |-- Biblioteca.Datos/
|   |-- Biblioteca.Negocio/
|   `-- Biblioteca.Presentacion/
|-- tests/
`-- tools/
```

## Publicación en GitHub

El repositorio local ya contiene historial de commits y evidencia. Para publicarlo en una cuenta de GitHub:

```powershell
git remote add origin https://github.com/USUARIO/SistemaGestionBiblioteca.git
git branch -M main
git push -u origin main
```

Reemplace `USUARIO` y la URL por el repositorio creado en su cuenta. La publicación requiere las credenciales del propietario; no deben incluirse tokens en archivos o commits.

## Decisiones de alcance

- No se implementan autenticación por roles ni pagos porque fueron excluidos explícitamente.
- Cada préstamo de la interfaz registra un ejemplar por operación; el modelo `DetallePrestamos` permite ampliar la solución a varios títulos y cantidades.
- Las operaciones críticas se ejecutan mediante procedimientos almacenados con transacciones, bloqueos de fila y reversión automática.
- La aplicación incluye un modo interno de demostración usado solo por `BuildAndCapture.ps1`; la ejecución normal siempre utiliza SQL Server.

## Estado

- Compilación: aprobada.
- Pruebas de dominio y estructura: aprobadas.
- Revisión visual: aprobada.
- Base de datos real: el script queda listo para ejecutar en la instancia SQL Server del evaluador.
