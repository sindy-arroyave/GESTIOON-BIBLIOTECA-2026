param(
    [switch]$SkipCapture
)

$ErrorActionPreference = 'Stop'
$projectRoot = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
$sourceRoot = Join-Path $projectRoot 'src'
$buildRoot = Join-Path $projectRoot 'build'
$appRoot = Join-Path $buildRoot 'app'
$captureRoot = Join-Path $projectRoot 'docs\capturas'
$compiler = 'C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe'

if (-not (Test-Path -LiteralPath $compiler)) {
    throw 'No se encontró el compilador de .NET Framework 4.0.'
}

if (Test-Path -LiteralPath $buildRoot) {
    $resolvedBuild = (Resolve-Path -LiteralPath $buildRoot).Path
    if (-not $resolvedBuild.StartsWith($projectRoot, [System.StringComparison]::OrdinalIgnoreCase)) {
        throw 'La carpeta de compilación no pertenece al proyecto.'
    }
    Remove-Item -LiteralPath $resolvedBuild -Recurse -Force
}
New-Item -ItemType Directory -Force -Path $appRoot | Out-Null

$entitiesDll = Join-Path $appRoot 'Biblioteca.Entidades.dll'
$dataDll = Join-Path $appRoot 'Biblioteca.Datos.dll'
$businessDll = Join-Path $appRoot 'Biblioteca.Negocio.dll'
$appExe = Join-Path $appRoot 'Biblioteca.Presentacion.exe'

& $compiler /nologo /target:library /debug+ /out:$entitiesDll /reference:System.Core.dll (Join-Path $sourceRoot 'Biblioteca.Entidades\Entidades.cs')
if ($LASTEXITCODE -ne 0) { throw 'Falló la compilación de Entidades.' }

& $compiler /nologo /target:library /debug+ /out:$dataDll /reference:System.Core.dll /reference:System.Data.dll /reference:System.Configuration.dll /reference:$entitiesDll (Join-Path $sourceRoot 'Biblioteca.Datos\SqlDatabase.cs') (Join-Path $sourceRoot 'Biblioteca.Datos\Repositorios.cs')
if ($LASTEXITCODE -ne 0) { throw 'Falló la compilación de Datos.' }

& $compiler /nologo /target:library /debug+ /out:$businessDll /reference:System.Core.dll /reference:System.Data.dll /reference:$entitiesDll /reference:$dataDll (Join-Path $sourceRoot 'Biblioteca.Negocio\Servicios.cs')
if ($LASTEXITCODE -ne 0) { throw 'Falló la compilación de Negocio.' }

& $compiler /nologo /target:winexe /debug+ /out:$appExe /reference:System.Core.dll /reference:System.Data.dll /reference:System.Drawing.dll /reference:System.Windows.Forms.dll /reference:System.Configuration.dll /reference:$entitiesDll /reference:$dataDll /reference:$businessDll (Join-Path $sourceRoot 'Biblioteca.Presentacion\Program.cs') (Join-Path $sourceRoot 'Biblioteca.Presentacion\UiHelpers.cs') (Join-Path $sourceRoot 'Biblioteca.Presentacion\CrudForms.cs') (Join-Path $sourceRoot 'Biblioteca.Presentacion\OperationalForms.cs') (Join-Path $sourceRoot 'Biblioteca.Presentacion\MainForm.cs')
if ($LASTEXITCODE -ne 0) { throw 'Falló la compilación de Presentación.' }

Copy-Item -LiteralPath (Join-Path $sourceRoot 'Biblioteca.Presentacion\App.config') -Destination ($appExe + '.config')

if (-not $SkipCapture) {
    New-Item -ItemType Directory -Force -Path $captureRoot | Out-Null
    & $appExe --capture $captureRoot
    if ($LASTEXITCODE -ne 0) { throw 'Falló la generación de capturas.' }
}

Write-Host 'Compilación completada:' $appExe

