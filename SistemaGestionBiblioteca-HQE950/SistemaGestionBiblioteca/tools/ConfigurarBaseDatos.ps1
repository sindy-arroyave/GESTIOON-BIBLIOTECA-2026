param(
    [string]$Servidor = 'localhost\SQLEXPRESS'
)

$ErrorActionPreference = 'Stop'
$projectRoot = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
$scriptPath = Join-Path $projectRoot 'sql\01_Crear_BibliotecaDB.sql'
$sqlcmd = Get-Command sqlcmd -ErrorAction SilentlyContinue
if (-not $sqlcmd) {
    throw 'sqlcmd no está instalado. Ejecute el archivo sql\01_Crear_BibliotecaDB.sql desde SQL Server Management Studio.'
}

& $sqlcmd.Source -S $Servidor -E -b -i $scriptPath
if ($LASTEXITCODE -ne 0) { throw 'No se pudo configurar BibliotecaDB.' }
Write-Host 'BibliotecaDB configurada correctamente en' $Servidor
