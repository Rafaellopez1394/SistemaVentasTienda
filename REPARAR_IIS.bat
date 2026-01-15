@echo off
:: ============================================================================
:: REPARAR CONFIGURACION IIS - Sistema Ventas
:: ============================================================================
echo.
echo ========================================
echo   REPARAR IIS - SISTEMA VENTAS
echo ========================================
echo.

:: Verificar que estamos ejecutando como Administrador
net session >nul 2>&1
if %errorLevel% neq 0 (
    echo ERROR: Debe ejecutar como Administrador
    pause
    exit /b 1
)

echo [1/5] Deteniendo IIS...
iisreset /stop >nul 2>&1
echo IIS detenido
echo.

echo [2/5] Eliminando sitios y pools existentes...
powershell.exe -ExecutionPolicy Bypass -Command ^
    "Import-Module WebAdministration; ^
     Get-Website | Where-Object { $_.Name -ne 'Default Web Site' } | ForEach-Object { Remove-Website -Name $_.Name -ErrorAction SilentlyContinue }; ^
     if (Test-Path 'IIS:\Sites\SistemaVentas') { Remove-Website -Name 'SistemaVentas' -ErrorAction SilentlyContinue }; ^
     if (Test-Path 'IIS:\AppPools\VentasWebPool') { Remove-WebAppPool -Name 'VentasWebPool' -ErrorAction SilentlyContinue }; ^
     Write-Host 'Limpieza completada'"

timeout /t 3 /nobreak >nul
echo Limpieza completada
echo.

echo [3/5] Deteniendo sitio por defecto...
powershell.exe -ExecutionPolicy Bypass -Command ^
    "Import-Module WebAdministration; ^
     if (Test-Path 'IIS:\Sites\Default Web Site') { Stop-Website -Name 'Default Web Site' -ErrorAction SilentlyContinue }; ^
     Write-Host 'Default Web Site detenido'"
echo.

echo [4/5] Creando nuevo sitio en C:\SistemaVentas...
powershell.exe -ExecutionPolicy Bypass -Command ^
    "$pool = New-WebAppPool -Name 'VentasWebPool'; ^
     Set-ItemProperty 'IIS:\AppPools\VentasWebPool' managedRuntimeVersion 'v4.0'; ^
     Set-ItemProperty 'IIS:\AppPools\VentasWebPool' managedPipelineMode 'Integrated'; ^
     $site = New-Website -Name 'SistemaVentas' -PhysicalPath 'C:\SistemaVentas' -ApplicationPool 'VentasWebPool' -Port 80 -Force; ^
     Start-Website -Name 'SistemaVentas'; ^
     Write-Host 'Sitio creado y iniciado'"

if %errorLevel% neq 0 (
    echo ERROR: No se pudo crear el sitio
    pause
    exit /b 1
)
echo Sitio creado correctamente
echo.

echo [5/5] Reiniciando IIS...
iisreset /start >nul 2>&1
echo IIS iniciado
echo.

echo ========================================
echo   REPARACION COMPLETADA
echo ========================================
echo.
echo Sitio configurado en: C:\SistemaVentas
echo URL: http://localhost
echo.
echo Verificando configuracion...
echo.

powershell.exe -ExecutionPolicy Bypass -Command ^
    "Import-Module WebAdministration; ^
     $site = Get-Website -Name 'SistemaVentas'; ^
     Write-Host 'Nombre:' $site.Name; ^
     Write-Host 'Estado:' $site.State; ^
     Write-Host 'Ruta:' $site.PhysicalPath; ^
     Write-Host 'Puerto:' $site.bindings.Collection[0].bindingInformation"

echo.
echo Abra http://localhost en su navegador
echo.
pause
