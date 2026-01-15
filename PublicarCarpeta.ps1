Write-Host "========================================" -ForegroundColor Cyan
Write-Host "   PUBLICACION MANUAL A CARPETA" -ForegroundColor Cyan  
Write-Host "========================================" -ForegroundColor Cyan

$sourcePath = "C:\Users\Rafael Lopez\Documents\SistemaVentasTienda\VentasWeb"
$publishPath = "C:\Publish\SistemaVentas"
$msbuild = "C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe"

Write-Host "[1/4] Compilando proyecto..." -ForegroundColor Yellow
Set-Location "C:\Users\Rafael Lopez\Documents\SistemaVentasTienda"
& $msbuild "VentasWeb\VentasWeb.csproj" /p:Configuration=Release /t:Rebuild /v:minimal /nologo

if ($LASTEXITCODE -ne 0) {
    Write-Host "ERROR: Fallo la compilacion" -ForegroundColor Red
    exit 1
}
Write-Host "Compilacion exitosa" -ForegroundColor Green

Write-Host "[2/4] Preparando carpeta de publicacion..." -ForegroundColor Yellow
if (Test-Path $publishPath) {
    Remove-Item "$publishPath\*" -Recurse -Force -ErrorAction SilentlyContinue
    Start-Sleep -Milliseconds 300
} else {
    New-Item -ItemType Directory -Path $publishPath -Force | Out-Null
}
Write-Host "Carpeta preparada" -ForegroundColor Green

Write-Host "[3/4] Copiando archivos..." -ForegroundColor Yellow
Write-Host "  - Copiando bin..." -ForegroundColor Gray
Copy-Item "$sourcePath\bin" -Destination "$publishPath\bin" -Recurse -Force

Write-Host "  - Copiando Content..." -ForegroundColor Gray
Copy-Item "$sourcePath\Content" -Destination "$publishPath\Content" -Recurse -Force

Write-Host "  - Copiando Scripts..." -ForegroundColor Gray
Copy-Item "$sourcePath\Scripts" -Destination "$publishPath\Scripts" -Recurse -Force

Write-Host "  - Copiando Views..." -ForegroundColor Gray
Copy-Item "$sourcePath\Views" -Destination "$publishPath\Views" -Recurse -Force

if (Test-Path "$sourcePath\fonts") {
    Write-Host "  - Copiando fonts..." -ForegroundColor Gray
    Copy-Item "$sourcePath\fonts" -Destination "$publishPath\fonts" -Recurse -Force
}

Write-Host "  - Copiando archivos raiz..." -ForegroundColor Gray
Copy-Item "$sourcePath\Web.config" -Destination $publishPath -Force
Copy-Item "$sourcePath\Global.asax" -Destination $publishPath -Force

if (Test-Path "$sourcePath\favicon.ico") {
    Copy-Item "$sourcePath\favicon.ico" -Destination $publishPath -Force
}

Write-Host "Archivos copiados" -ForegroundColor Green

Write-Host "[4/4] Verificando archivos criticos..." -ForegroundColor Yellow
$criticalFiles = @(
    "$publishPath\bin\VentasWeb.dll",
    "$publishPath\bin\roslyn\csc.exe",
    "$publishPath\Web.config",
    "$publishPath\Global.asax"
)

foreach ($file in $criticalFiles) {
    if (Test-Path $file) {
        Write-Host "  OK $(Split-Path $file -Leaf)" -ForegroundColor Green
    } else {
        Write-Host "  FALTA $(Split-Path $file -Leaf)" -ForegroundColor Red
    }
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Green
Write-Host "   PUBLICACION COMPLETADA" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host "Carpeta: $publishPath" -ForegroundColor White
Write-Host ""
Write-Host "PASOS PARA CONFIGURAR EN IIS:" -ForegroundColor Yellow
Write-Host "1. Abrir Administrador de IIS (ejecutar: inetmgr)" -ForegroundColor White
Write-Host "2. Click derecho en Sitios -> Agregar sitio web" -ForegroundColor White
Write-Host "3. Nombre: SistemaVentas" -ForegroundColor White
Write-Host "4. Ruta fisica: $publishPath" -ForegroundColor Cyan
Write-Host "5. Puerto: 80" -ForegroundColor White
Write-Host "6. Application Pool: .NET CLR v4.0, Integrated" -ForegroundColor White
Write-Host ""
Write-Host "Presiona Enter para salir..." -ForegroundColor Gray
Read-Host
