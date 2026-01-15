@echo off
:: ============================================================================
:: INSTALADOR SIMPLE - Sistema Ventas
:: Ejecuta el script PowerShell de instalacion
:: ============================================================================

:: Verificar que estamos ejecutando como Administrador
net session >nul 2>&1
if %errorLevel% neq 0 (
    echo.
    echo ERROR: Debe ejecutar como Administrador
    echo.
    echo Haga clic derecho en este archivo y seleccione "Ejecutar como administrador"
    echo.
    pause
    exit /b 1
)

:: Ejecutar instalador PowerShell
powershell.exe -ExecutionPolicy Bypass -File "%~dp0INSTALAR_COMPLETO.ps1"

pause
