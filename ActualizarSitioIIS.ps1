# Script para actualizar la configuracion del sitio IIS
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "   ACTUALIZANDO SITIO EN IIS" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

$siteName = "SistemaVentas"
$newPath = "C:\Publish\SistemaVentas"
$appPoolName = "VentasWebPool"

# Verificar que la carpeta existe
if (-not (Test-Path $newPath)) {
    Write-Host "ERROR: La carpeta $newPath no existe" -ForegroundColor Red
    Write-Host "Ejecuta primero PublicarCarpeta.ps1" -ForegroundColor Yellow
    Read-Host "Presiona Enter para salir"
    exit 1
}

Write-Host "[1/4] Deteniendo sitio..." -ForegroundColor Yellow
Stop-Process -Name "w3wp" -Force -ErrorAction SilentlyContinue
Start-Sleep -Seconds 2
Write-Host "Sitio detenido" -ForegroundColor Green

Write-Host "[2/4] Actualizando ruta del sitio..." -ForegroundColor Yellow
try {
    Import-Module WebAdministration -ErrorAction Stop
    
    # Actualizar la ruta fisica del sitio
    Set-ItemProperty "IIS:\Sites\$siteName" -Name physicalPath -Value $newPath
    Write-Host "Ruta actualizada a: $newPath" -ForegroundColor Green
    
} catch {
    Write-Host "ERROR: No se pudo actualizar con WebAdministration" -ForegroundColor Red
    Write-Host "Intentando con appcmd..." -ForegroundColor Yellow
    
    $appcmd = "$env:SystemRoot\System32\inetsrv\appcmd.exe"
    & $appcmd set site "$siteName" "/[path='/'].physicalPath:$newPath"
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "Ruta actualizada con appcmd" -ForegroundColor Green
    } else {
        Write-Host "ERROR: Fallo la actualizacion" -ForegroundColor Red
        Read-Host "Presiona Enter para salir"
        exit 1
    }
}

Write-Host "[3/4] Verificando Application Pool..." -ForegroundColor Yellow
try {
    $pool = Get-Item "IIS:\AppPools\$appPoolName" -ErrorAction SilentlyContinue
    if ($pool) {
        Write-Host "Application Pool: $appPoolName" -ForegroundColor White
        Write-Host "  .NET CLR Version: $($pool.managedRuntimeVersion)" -ForegroundColor Gray
        Write-Host "  Pipeline Mode: $($pool.managedPipelineMode)" -ForegroundColor Gray
    } else {
        Write-Host "ADVERTENCIA: Application Pool no encontrado" -ForegroundColor Yellow
    }
} catch {
    Write-Host "No se pudo verificar el Application Pool" -ForegroundColor Yellow
}

Write-Host "[4/4] Iniciando sitio..." -ForegroundColor Yellow
try {
    Start-Website -Name $siteName -ErrorAction Stop
    Write-Host "Sitio iniciado" -ForegroundColor Green
} catch {
    $appcmd = "$env:SystemRoot\System32\inetsrv\appcmd.exe"
    & $appcmd start site "$siteName"
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "Sitio iniciado con appcmd" -ForegroundColor Green
    } else {
        Write-Host "ERROR: No se pudo iniciar el sitio" -ForegroundColor Red
    }
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Green
Write-Host "   CONFIGURACION ACTUALIZADA" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host "Sitio: $siteName" -ForegroundColor White
Write-Host "Ruta: $newPath" -ForegroundColor Cyan
Write-Host "URL: http://localhost" -ForegroundColor Cyan
Write-Host ""
Write-Host "Abre el navegador en: http://localhost" -ForegroundColor Yellow
Write-Host ""
Read-Host "Presiona Enter para salir"
