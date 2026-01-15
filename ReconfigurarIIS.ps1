# Script para reconfigurar completamente el sitio en IIS
# EJECUTAR COMO ADMINISTRADOR

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "   RECONFIGURANDO SITIO EN IIS" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

$siteName = "SistemaVentas"
$publishPath = "C:\Publish\SistemaVentas"
$appPoolName = "VentasWebPool"
$puerto = 80

# Verificar que existe la carpeta
if (-not (Test-Path $publishPath)) {
    Write-Host "ERROR: No existe la carpeta $publishPath" -ForegroundColor Red
    Write-Host "Ejecuta primero PublicarCarpeta.ps1" -ForegroundColor Yellow
    pause
    exit 1
}

Write-Host "[1/5] Deteniendo sitios y procesos IIS..." -ForegroundColor Yellow
Stop-Process -Name "w3wp" -Force -ErrorAction SilentlyContinue
Start-Sleep -Seconds 2
Write-Host "Procesos detenidos" -ForegroundColor Green

Write-Host "[2/5] Importando modulo WebAdministration..." -ForegroundColor Yellow
Import-Module WebAdministration

Write-Host "[3/5] Eliminando sitio anterior si existe..." -ForegroundColor Yellow
$siteExists = Get-Website -Name $siteName -ErrorAction SilentlyContinue
if ($siteExists) {
    Remove-Website -Name $siteName
    Write-Host "Sitio anterior eliminado" -ForegroundColor Green
} else {
    Write-Host "No habia sitio anterior" -ForegroundColor Gray
}

Write-Host "[4/5] Verificando Application Pool..." -ForegroundColor Yellow
$poolExists = Get-WebAppPoolState -Name $appPoolName -ErrorAction SilentlyContinue
if (-not $poolExists) {
    Write-Host "Creando Application Pool: $appPoolName" -ForegroundColor Yellow
    New-WebAppPool -Name $appPoolName
    Set-ItemProperty "IIS:\AppPools\$appPoolName" -Name managedRuntimeVersion -Value "v4.0"
    Set-ItemProperty "IIS:\AppPools\$appPoolName" -Name managedPipelineMode -Value "Integrated"
    Write-Host "Application Pool creado" -ForegroundColor Green
} else {
    Write-Host "Application Pool ya existe" -ForegroundColor Gray
}

Write-Host "[5/5] Creando sitio nuevo..." -ForegroundColor Yellow
New-Website -Name $siteName `
    -PhysicalPath $publishPath `
    -ApplicationPool $appPoolName `
    -Port $puerto `
    -Force

if ($?) {
    Write-Host "Sitio creado exitosamente" -ForegroundColor Green
} else {
    Write-Host "ERROR: No se pudo crear el sitio" -ForegroundColor Red
    pause
    exit 1
}

Write-Host ""
Write-Host "Iniciando sitio..." -ForegroundColor Yellow
Start-Website -Name $siteName

# Detener Default Web Site si existe
$defaultSite = Get-Website -Name "Default Web Site" -ErrorAction SilentlyContinue
if ($defaultSite -and $defaultSite.State -eq "Started") {
    Write-Host "Deteniendo Default Web Site..." -ForegroundColor Yellow
    Stop-Website -Name "Default Web Site"
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Green
Write-Host "   SITIO CONFIGURADO EXITOSAMENTE" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host "Nombre: $siteName" -ForegroundColor White
Write-Host "Ruta: $publishPath" -ForegroundColor Cyan
Write-Host "Puerto: $puerto" -ForegroundColor White
Write-Host "Application Pool: $appPoolName (.NET CLR v4.0)" -ForegroundColor White
Write-Host ""
Write-Host "Accede al sitio en:" -ForegroundColor Yellow
Write-Host "  http://localhost" -ForegroundColor Cyan
Write-Host "  http://localhost/Login" -ForegroundColor Cyan
Write-Host ""
pause
