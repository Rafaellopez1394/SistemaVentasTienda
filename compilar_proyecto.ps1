Write-Host "=========================================" -ForegroundColor Cyan
Write-Host "COMPILANDO PROYECTO - FISCALAPI" -ForegroundColor Cyan
Write-Host "=========================================" -ForegroundColor Cyan
Write-Host ""

# Buscar archivo .sln
$solutionFile = Get-ChildItem -Filter "*.sln" | Select-Object -First 1

if (-not $solutionFile) {
    Write-Host "ERROR: No se encontro archivo .sln" -ForegroundColor Red
    exit 1
}

Write-Host "Archivo de solucion: $($solutionFile.Name)" -ForegroundColor Green
Write-Host ""

# Buscar MSBuild
$msbuildPaths = @(
    "C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe",
    "C:\Program Files\Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin\MSBuild.exe",
    "C:\Program Files\Microsoft Visual Studio\2022\Enterprise\MSBuild\Current\Bin\MSBuild.exe",
    "C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild.exe",
    "C:\Program Files (x86)\Microsoft Visual Studio\2019\Professional\MSBuild\Current\Bin\MSBuild.exe"
)

$msbuild = $null
foreach ($path in $msbuildPaths) {
    if (Test-Path $path) {
        $msbuild = $path
        break
    }
}

if (-not $msbuild) {
    Write-Host "ADVERTENCIA: MSBuild no encontrado" -ForegroundColor Yellow
    Write-Host "Opciones:" -ForegroundColor Yellow
    Write-Host "1. Abre Visual Studio" -ForegroundColor White
    Write-Host "2. Abre la solucion: $($solutionFile.Name)" -ForegroundColor White
    Write-Host "3. Build -> Rebuild Solution (Ctrl+Shift+B)" -ForegroundColor White
    Write-Host ""
    Write-Host "O instala Visual Studio Build Tools desde:" -ForegroundColor White
    Write-Host "https://visualstudio.microsoft.com/downloads/" -ForegroundColor White
    exit 1
}

Write-Host "MSBuild encontrado: $msbuild" -ForegroundColor Green
Write-Host ""
Write-Host "Compilando..." -ForegroundColor Yellow

& $msbuild $solutionFile.FullName /t:Rebuild /p:Configuration=Release /v:minimal

if ($LASTEXITCODE -eq 0) {
    Write-Host ""
    Write-Host "=========================================" -ForegroundColor Green
    Write-Host "COMPILACION EXITOSA" -ForegroundColor Green
    Write-Host "=========================================" -ForegroundColor Green
    Write-Host ""
    Write-Host "Sistema actualizado con:" -ForegroundColor Cyan
    Write-Host "- FiscalAPI como unico PAC" -ForegroundColor White
    Write-Host "- Reportes Avanzados de Rentabilidad" -ForegroundColor White
    Write-Host "- Analisis de utilidad por producto" -ForegroundColor White
    Write-Host "- Estado de Resultados (P&L)" -ForegroundColor White
    Write-Host "- Recuperacion de credito y cartera" -ForegroundColor White
    Write-Host ""
    Write-Host "Listo para facturar y analizar rentabilidad!" -ForegroundColor Green
} else {
    Write-Host ""
    Write-Host "ERROR EN COMPILACION" -ForegroundColor Red
    Write-Host "Revisa los errores arriba" -ForegroundColor Yellow
}
