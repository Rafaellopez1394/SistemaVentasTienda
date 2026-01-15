@echo off
:: ============================================================================
:: REGISTRAR ASP.NET EN IIS
:: ============================================================================
echo.
echo ========================================
echo   REGISTRAR ASP.NET EN IIS
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
iisreset /stop
echo.

echo [2/5] Registrando ASP.NET 4.x en IIS...
%windir%\Microsoft.NET\Framework64\v4.0.30319\aspnet_regiis.exe -i
echo.

echo [3/5] Desbloqueando configuracion IIS...
%systemroot%\system32\inetsrv\appcmd.exe unlock config -section:system.webServer/modules
%systemroot%\system32\inetsrv\appcmd.exe unlock config -section:system.webServer/handlers
echo.

echo [4/5] Configurando Application Pool...
%systemroot%\system32\inetsrv\appcmd.exe set apppool "VentasWebPool" /managedRuntimeVersion:v4.0
%systemroot%\system32\inetsrv\appcmd.exe set apppool "VentasWebPool" /managedPipelineMode:Integrated
echo.

echo [5/5] Reiniciando IIS...
iisreset /start
echo.

echo ========================================
echo   REGISTRO COMPLETADO
echo ========================================
echo.
echo ASP.NET registrado correctamente
echo.
echo Abra: http://localhost
echo.
pause
