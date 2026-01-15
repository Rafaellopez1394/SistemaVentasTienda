@echo off
:: ============================================================================
:: DESBLOQUEAR CONFIGURACION IIS
:: ============================================================================
echo.
echo ========================================
echo   DESBLOQUEAR CONFIGURACION IIS
echo ========================================
echo.

:: Verificar que estamos ejecutando como Administrador
net session >nul 2>&1
if %errorLevel% neq 0 (
    echo ERROR: Debe ejecutar como Administrador
    pause
    exit /b 1
)

echo [1/3] Desbloqueando seccion modules...
%systemroot%\system32\inetsrv\appcmd.exe unlock config -section:system.webServer/modules
echo.

echo [2/3] Desbloqueando seccion handlers...
%systemroot%\system32\inetsrv\appcmd.exe unlock config -section:system.webServer/handlers
echo.

echo [3/3] Reiniciando IIS...
iisreset /restart
echo.

echo ========================================
echo   CONFIGURACION DESBLOQUEADA
echo ========================================
echo.
echo Ahora puede acceder a: http://localhost
echo.
pause
