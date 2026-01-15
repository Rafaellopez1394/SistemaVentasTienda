@echo off
:: ============================================================================
:: INSTALAR ASP.NET 4.x EN IIS
:: ============================================================================
echo.
echo ========================================
echo   INSTALAR ASP.NET 4.x EN IIS
echo ========================================
echo.

:: Verificar que estamos ejecutando como Administrador
net session >nul 2>&1
if %errorLevel% neq 0 (
    echo ERROR: Debe ejecutar como Administrador
    pause
    exit /b 1
)

echo [1/6] Deteniendo IIS...
iisreset /stop
echo.

echo [2/6] Instalando caracteristicas de IIS y ASP.NET 4.x...
echo (Esto puede tomar varios minutos)
echo.

DISM /Online /Enable-Feature /FeatureName:IIS-WebServerRole
DISM /Online /Enable-Feature /FeatureName:IIS-WebServer
DISM /Online /Enable-Feature /FeatureName:IIS-CommonHttpFeatures
DISM /Online /Enable-Feature /FeatureName:IIS-HttpErrors
DISM /Online /Enable-Feature /FeatureName:IIS-ApplicationDevelopment
DISM /Online /Enable-Feature /FeatureName:IIS-NetFxExtensibility45
DISM /Online /Enable-Feature /FeatureName:IIS-HealthAndDiagnostics
DISM /Online /Enable-Feature /FeatureName:IIS-HttpLogging
DISM /Online /Enable-Feature /FeatureName:IIS-Security
DISM /Online /Enable-Feature /FeatureName:IIS-RequestFiltering
DISM /Online /Enable-Feature /FeatureName:IIS-Performance
DISM /Online /Enable-Feature /FeatureName:IIS-WebServerManagementTools
DISM /Online /Enable-Feature /FeatureName:IIS-StaticContent
DISM /Online /Enable-Feature /FeatureName:IIS-DefaultDocument
DISM /Online /Enable-Feature /FeatureName:IIS-DirectoryBrowsing
DISM /Online /Enable-Feature /FeatureName:IIS-ASPNET45
DISM /Online /Enable-Feature /FeatureName:IIS-ISAPIExtensions
DISM /Online /Enable-Feature /FeatureName:IIS-ISAPIFilter
DISM /Online /Enable-Feature /FeatureName:IIS-HttpCompressionStatic

echo.

echo [3/6] Registrando ASP.NET 4.x...
%windir%\Microsoft.NET\Framework64\v4.0.30319\aspnet_regiis.exe -i
echo.

echo [4/6] Desbloqueando configuracion IIS...
%systemroot%\system32\inetsrv\appcmd.exe unlock config -section:system.webServer/modules
%systemroot%\system32\inetsrv\appcmd.exe unlock config -section:system.webServer/handlers
echo.

echo [5/6] Configurando Application Pool...
%systemroot%\system32\inetsrv\appcmd.exe set apppool "VentasWebPool" /managedRuntimeVersion:v4.0
%systemroot%\system32\inetsrv\appcmd.exe set apppool "VentasWebPool" /managedPipelineMode:Integrated
echo.

echo [6/6] Reiniciando IIS...
iisreset /start
echo.

echo ========================================
echo   INSTALACION COMPLETADA
echo ========================================
echo.
echo ASP.NET 4.x instalado y configurado
echo.
echo Abra: http://localhost
echo.
pause
