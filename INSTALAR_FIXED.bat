@echo off
:: ============================================================================
:: INSTALADOR SISTEMA VENTAS - Version Corregida
:: ============================================================================
echo.
echo ========================================
echo   INSTALADOR SISTEMA VENTAS
echo ========================================
echo.

:: Verificar que estamos ejecutando como Administrador
net session >nul 2>&1
if %errorLevel% neq 0 (
    echo ERROR: Debe ejecutar como Administrador
    echo.
    echo Haga clic derecho en INSTALAR.bat y seleccione "Ejecutar como administrador"
    pause
    exit /b 1
)

:: Verificar que existe la carpeta Web
if not exist "%~dp0Web" (
    echo ERROR: No se encuentra la carpeta Web
    echo Asegurese de que INSTALAR.bat este en la misma carpeta que Web\
    pause
    exit /b 1
)

echo Verificando archivos...
echo Instalando sistema...
echo.

:: ============================================================================
:: PASO 1: COPIAR ARCHIVOS
:: ============================================================================
echo [1/4] Copiando archivos a C:\SistemaVentas...

if exist "C:\SistemaVentas" (
    rmdir /S /Q "C:\SistemaVentas" 2>nul
)

xcopy /E /I /Y /Q "%~dp0Web" "C:\SistemaVentas"
if %errorLevel% neq 0 (
    echo ERROR: No se pudieron copiar los archivos
    pause
    exit /b 1
)
echo Archivos copiados OK
echo.

:: ============================================================================
:: PASO 2: PERMISOS
:: ============================================================================
echo [2/4] Configurando permisos...

icacls "C:\SistemaVentas" /grant "IIS_IUSRS:(OI)(CI)F" /T >nul 2>&1
icacls "C:\SistemaVentas" /grant "IUSR:(OI)(CI)F" /T >nul 2>&1
icacls "C:\SistemaVentas" /grant "IIS APPPOOL\VentasWebPool:(OI)(CI)F" /T >nul 2>&1

echo Permisos configurados OK
echo.

:: ============================================================================
:: PASO 3: CONFIGURAR IIS CON POWERSHELL
:: ============================================================================
echo [3/4] Configurando IIS...

powershell.exe -ExecutionPolicy Bypass -Command ^
    "Import-Module WebAdministration; ^
     if (Test-Path 'IIS:\AppPools\VentasWebPool') { Remove-WebAppPool -Name 'VentasWebPool' -ErrorAction SilentlyContinue }; ^
     if (Test-Path 'IIS:\Sites\SistemaVentas') { Remove-Website -Name 'SistemaVentas' -ErrorAction SilentlyContinue }; ^
     Start-Sleep -Seconds 2; ^
     $pool = New-WebAppPool -Name 'VentasWebPool'; ^
     Set-ItemProperty IIS:\AppPools\VentasWebPool managedRuntimeVersion 'v4.0'; ^
     Set-ItemProperty IIS:\AppPools\VentasWebPool managedPipelineMode 'Integrated'; ^
     $site = New-Website -Name 'SistemaVentas' -PhysicalPath 'C:\SistemaVentas' -ApplicationPool 'VentasWebPool' -Port 80; ^
     Start-Website -Name 'SistemaVentas'; ^
     Write-Host 'IIS configurado OK'"

if %errorLevel% neq 0 (
    echo ERROR: No se pudo configurar IIS
    echo.
    echo Verifique que IIS este instalado correctamente
    pause
    exit /b 1
)
echo.

:: ============================================================================
:: PASO 4: REINICIAR IIS
:: ============================================================================
echo [4/4] Reiniciando IIS...

iisreset /restart >nul 2>&1

echo IIS reiniciado OK
echo.

:: ============================================================================
:: INSTALACION COMPLETADA
:: ============================================================================
echo ========================================
echo   INSTALACION COMPLETADA
echo ========================================
echo.
echo Sistema instalado en: C:\SistemaVentas
echo Sitio web: http://localhost
echo.
echo ========================================
echo   CONFIGURACION PENDIENTE
echo ========================================
echo.
echo IMPORTANTE: Debe configurar la cadena de conexion en Web.config
echo.
echo 1. Abra el archivo: C:\SistemaVentas\Web.config
echo.
echo 2. Busque la seccion ^<connectionStrings^>
echo.
echo 3. Actualice los datos de su servidor SQL:
echo    - Data Source: [NOMBRE_SERVIDOR]
echo    - Initial Catalog: DB_TIENDA
echo    - User ID: [USUARIO]
echo    - Password: [CONTRASENA]
echo.
echo 4. Guarde el archivo
echo.
echo 5. Ejecute: iisreset
echo.
echo 6. Abra en navegador: http://localhost
echo.
echo ========================================
echo.
pause
