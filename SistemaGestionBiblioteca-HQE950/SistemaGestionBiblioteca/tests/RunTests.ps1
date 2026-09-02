$ErrorActionPreference = 'Stop'
$projectRoot = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
$compiler = 'C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe'
$appRoot = Join-Path $projectRoot 'build\app'
$testRoot = Join-Path $projectRoot 'build\tests'

& (Join-Path $projectRoot 'tools\BuildAndCapture.ps1') -SkipCapture
New-Item -ItemType Directory -Force -Path $testRoot | Out-Null
$testExe = Join-Path $testRoot 'DomainSmokeTests.exe'
$entitiesDll = Join-Path $appRoot 'Biblioteca.Entidades.dll'
$dataDll = Join-Path $appRoot 'Biblioteca.Datos.dll'
$businessDll = Join-Path $appRoot 'Biblioteca.Negocio.dll'
& $compiler /nologo /target:exe /out:$testExe /reference:System.Core.dll /reference:System.Data.dll /reference:$entitiesDll /reference:$dataDll /reference:$businessDll (Join-Path $PSScriptRoot 'DomainSmokeTests.cs')
if ($LASTEXITCODE -ne 0) { throw 'No fue posible compilar las pruebas.' }
Copy-Item -LiteralPath (Join-Path $appRoot 'Biblioteca.Entidades.dll') -Destination $testRoot -Force
Copy-Item -LiteralPath (Join-Path $appRoot 'Biblioteca.Datos.dll') -Destination $testRoot -Force
Copy-Item -LiteralPath (Join-Path $appRoot 'Biblioteca.Negocio.dll') -Destination $testRoot -Force
& $testExe
if ($LASTEXITCODE -ne 0) { throw 'Fallaron pruebas de dominio.' }

$sql = Get-Content -Raw -Encoding UTF8 (Join-Path $projectRoot 'sql\01_Crear_BibliotecaDB.sql')
$requiredSql = @('CREATE TABLE dbo.Autores','CREATE TABLE dbo.Categorias','CREATE TABLE dbo.Usuarios','CREATE TABLE dbo.Libros','CREATE TABLE dbo.Prestamos','CREATE TABLE dbo.DetallePrestamos','CREATE PROCEDURE dbo.sp_RegistrarPrestamo','CREATE PROCEDURE dbo.sp_RegistrarDevolucion','BEGIN TRANSACTION','ROLLBACK TRANSACTION')
foreach ($token in $requiredSql) {
    if (-not $sql.Contains($token)) { throw "Falta elemento SQL requerido: $token" }
    Write-Host "PASS SQL: $token"
}

$captures = Get-ChildItem -LiteralPath (Join-Path $projectRoot 'docs\capturas') -Filter '*.png'
if ($captures.Count -lt 5) { throw 'Se esperaban al menos cinco capturas.' }
Write-Host 'PASS capturas:' $captures.Count
Write-Host 'Todas las pruebas finalizaron correctamente.'
