# Diagnostico y limpieza completa de IIS
# EJECUTAR COMO ADMINISTRADOR

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "   DIAGNOSTICO Y LIMPIEZA IIS" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

Import-Module WebAdministration

Write-Host "`n[DIAGNOSTICO] Sitios actuales:" -ForegroundColor Yellow
Get-Website | Format-Table Name, ID, State, PhysicalPath, @{Name="Bindings";Expression={$_.bindings.Collection.bindingInformation}} -AutoSize

Write-Host "`n[DIAGNOSTICO] Aplicaciones virtuales:" -ForegroundColor Yellow
Get-WebApplication | Format-Table Site, Path, PhysicalPath -AutoSize

Write-Host "`n[LIMPIEZA] Eliminando aplicacion virtual /SistemaVentas si existe..." -ForegroundColor Yellow
$apps = Get-WebApplication -Site "*" | Where-Object { $_.Path -eq "/SistemaVentas" }
if ($apps) {
    foreach ($app in $apps) {
        Write-Host "  Eliminando aplicacion: $($app.Path) del sitio: $($app.ItemXPath.Split("'")[1])" -ForegroundColor Red
        Remove-WebApplication -Name "SistemaVentas" -Site $app.ItemXPath.Split("'")[1] -ErrorAction SilentlyContinue
    }
    Write-Host "Aplicaciones eliminadas" -ForegroundColor Green
} else {
    Write-Host "No hay aplicaciones virtuales /SistemaVentas" -ForegroundColor Gray
}

Write-Host "`n[LIMPIEZA] Eliminando sitio SistemaVentas antiguo..." -ForegroundColor Yellow
$site = Get-Website -Name "SistemaVentas" -ErrorAction SilentlyContinue
if ($site) {
    Stop-Website -Name "SistemaVentas" -ErrorAction SilentlyContinue
    Remove-Website -Name "SistemaVentas"
    Write-Host "Sitio eliminado" -ForegroundColor Green
} else {
    Write-Host "No habia sitio anterior" -ForegroundColor Gray
}

Write-Host "`n[LIMPIEZA] Deteniendo Default Web Site..." -ForegroundColor Yellow
$defaultSite = Get-Website -Name "Default Web Site" -ErrorAction SilentlyContinue
if ($defaultSite) {
    Stop-Website -Name "Default Web Site" -ErrorAction SilentlyContinue
    Write-Host "Default Web Site detenido" -ForegroundColor Green
}

Write-Host "`n[CONFIGURACION] Verificando Application Pool..." -ForegroundColor Yellow
$appPoolName = "VentasWebPool"
$pool = Get-WebAppPoolState -Name $appPoolName -ErrorAction SilentlyContinue
if (-not $pool) {
    Write-Host "Creando Application Pool..." -ForegroundColor Yellow
    New-WebAppPool -Name $appPoolName
    Set-ItemProperty "IIS:\AppPools\$appPoolName" -Name managedRuntimeVersion -Value "v4.0"
    Set-ItemProperty "IIS:\AppPools\$appPoolName" -Name managedPipelineMode -Value "Integrated"
    Write-Host "Application Pool creado" -ForegroundColor Green
} else {
    Write-Host "Application Pool existe" -ForegroundColor Gray
}

Write-Host "`n[CONFIGURACION] Creando sitio nuevo..." -ForegroundColor Yellow
$publishPath = "C:\Publish\SistemaVentas"

if (-not (Test-Path $publishPath)) {
    Write-Host "ERROR: No existe $publishPath" -ForegroundColor Red
    Write-Host "Ejecuta primero PublicarCarpeta.ps1" -ForegroundColor Yellow
    pause
    exit 1
}

New-Website -Name "SistemaVentas" `
    -PhysicalPath $publishPath `
    -ApplicationPool $appPoolName `
    -Port 80 `
    -Force

Start-Website -Name "SistemaVentas"

Write-Host "`n========================================" -ForegroundColor Green
Write-Host "   CONFIGURACION COMPLETADA" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green

Write-Host "`nSitios activos:" -ForegroundColor Yellow
Get-Website | Where-Object { $_.State -eq "Started" } | Format-Table Name, State, PhysicalPath, @{Name="Bindings";Expression={$_.bindings.Collection.bindingInformation}} -AutoSize

Write-Host "`nAccede al sitio en:" -ForegroundColor Cyan
Write-Host "  http://localhost" -ForegroundColor Green
Write-Host "  http://localhost/Login" -ForegroundColor Green
Write-Host ""
Write-Host "Si aun ves el error, presiona Ctrl+F5 en el navegador para limpiar cache" -ForegroundColor Yellow
Write-Host ""
pause
