@echo off
:: ============================================================================
:: DIAGNOSTICO IIS - Sistema Ventas
:: ============================================================================
echo.
echo ========================================
echo   DIAGNOSTICO IIS
echo ========================================
echo.

echo [1] Verificando archivos en C:\SistemaVentas...
if exist "C:\SistemaVentas\Web.config" (
    echo    OK - Web.config existe
) else (
    echo    ERROR - No se encuentra Web.config
)

if exist "C:\SistemaVentas\bin\VentasWeb.dll" (
    echo    OK - VentasWeb.dll existe
) else (
    echo    ERROR - No se encuentra VentasWeb.dll
)
echo.

echo [2] Listando sitios en IIS...
powershell.exe -ExecutionPolicy Bypass -Command "Import-Module WebAdministration; Get-Website | Format-Table Name, ID, State, PhysicalPath, @{Label='Bindings';Expression={$_.bindings.Collection.bindingInformation}} -AutoSize"
echo.

echo [3] Listando Application Pools...
powershell.exe -ExecutionPolicy Bypass -Command "Import-Module WebAdministration; Get-WebAppPoolState * | Format-Table"
echo.

echo [4] Verificando sitio SistemaVentas...
powershell.exe -ExecutionPolicy Bypass -Command "Import-Module WebAdministration; if (Test-Path 'IIS:\Sites\SistemaVentas') { $site = Get-Website -Name 'SistemaVentas'; Write-Host 'Sitio existe:'; Write-Host '  Nombre:' $site.Name; Write-Host '  Estado:' $site.State; Write-Host '  Ruta fisica:' $site.PhysicalPath; Write-Host '  Puerto:' $site.bindings.Collection[0].bindingInformation } else { Write-Host 'ERROR: Sitio SistemaVentas NO existe' }"
echo.

echo ========================================
echo   DIAGNOSTICO COMPLETADO
echo ========================================
echo.
pause
