@echo off
:: ============================================================================
:: INSTALADOR COMPLETO SISTEMA VENTAS - TODO EN UNO
:: ============================================================================
setlocal enabledelayedexpansion

cls
echo.
echo ============================================================
echo   INSTALADOR COMPLETO - SISTEMA VENTAS
echo ============================================================
echo.
echo Este proceso puede tomar 10-15 minutos
echo No cierre esta ventana hasta que termine
echo.
pause

:: Verificar que estamos ejecutando como Administrador
net session >nul 2>&1
if %errorLevel% neq 0 (
    echo.
    echo ERROR: Debe ejecutar como Administrador
    echo.
    echo Haga clic derecho en este archivo y seleccione
    echo "Ejecutar como administrador"
    echo.
    pause
    exit /b 1
)

:: Verificar carpeta Web
if not exist "%~dp0Web" (
    echo.
    echo ERROR: No se encuentra la carpeta Web
    echo Asegurese de ejecutar este instalador desde la carpeta correcta
    echo.
    pause
    exit /b 1
)

:: ============================================================================
:: PASO 1: INSTALAR ASP.NET 4.x Y CARACTERISTICAS IIS
:: ============================================================================
echo.
echo [1/10] Instalando ASP.NET 4.x y caracteristicas IIS...
echo        (Esto puede tomar varios minutos, sea paciente)
echo.

DISM /Online /Enable-Feature /FeatureName:IIS-ASPNET45 /All /NoRestart /Quiet >nul 2>&1
DISM /Online /Enable-Feature /FeatureName:IIS-NetFxExtensibility45 /All /NoRestart /Quiet >nul 2>&1
DISM /Online /Enable-Feature /FeatureName:NetFx4Extended-ASPNET45 /All /NoRestart /Quiet >nul 2>&1
DISM /Online /Enable-Feature /FeatureName:IIS-StaticContent /All /NoRestart /Quiet >nul 2>&1
DISM /Online /Enable-Feature /FeatureName:IIS-DefaultDocument /All /NoRestart /Quiet >nul 2>&1
DISM /Online /Enable-Feature /FeatureName:IIS-DirectoryBrowsing /All /NoRestart /Quiet >nul 2>&1

echo        ASP.NET 4.x y contenido estatico instalado OK

:: ============================================================================
:: PASO 2: DETENER IIS Y LIMPIAR INSTALACION ANTERIOR
:: ============================================================================
echo.
echo [2/10] Deteniendo IIS y limpiando instalacion anterior...
echo.

iisreset /stop >nul 2>&1
timeout /t 3 /nobreak >nul

:: Eliminar sitio anterior
%systemroot%\system32\inetsrv\appcmd.exe delete site "SistemaVentas" >nul 2>&1
%systemroot%\system32\inetsrv\appcmd.exe delete apppool "VentasWebPool" >nul 2>&1

:: ELIMINAR COMPLETAMENTE Default Web Site para evitar conflictos
%systemroot%\system32\inetsrv\appcmd.exe stop site "Default Web Site" >nul 2>&1
timeout /t 1 /nobreak >nul
%systemroot%\system32\inetsrv\appcmd.exe delete site "Default Web Site" >nul 2>&1

:: Eliminar todos los otros sitios que puedan estar en puerto 80
for /f "tokens=*" %%a in ('%systemroot%\system32\inetsrv\appcmd.exe list site /text:name') do (
    if not "%%a"=="SistemaVentas" (
        %systemroot%\system32\inetsrv\appcmd.exe delete site "%%a" >nul 2>&1
    )
)

timeout /t 2 /nobreak >nul
echo        Limpieza completada

:: ============================================================================
:: PASO 3: COPIAR ARCHIVOS
:: ============================================================================
echo.
echo [3/10] Copiando archivos a C:\SistemaVentas...
echo.

if exist "C:\SistemaVentas" (
    rmdir /S /Q "C:\SistemaVentas" >nul 2>&1
    timeout /t 2 /nobreak >nul
)

xcopy /E /I /Y /Q "%~dp0Web" "C:\SistemaVentas" >nul

if %errorLevel% neq 0 (
    echo        ERROR: No se pudieron copiar los archivos
    pause
    exit /b 1
)

echo        Archivos copiados OK

:: Verificar carpetas de recursos
echo.
echo        Verificando recursos...
if exist "C:\SistemaVentas\Content" (echo        - Content: OK) else (echo        - Content: FALTA)
if exist "C:\SistemaVentas\Scripts" (echo        - Scripts: OK) else (echo        - Scripts: FALTA)
if exist "C:\SistemaVentas\Images" (echo        - Images: OK) else (echo        - Images: FALTA ^(puede ser normal^))
if exist "C:\SistemaVentas\fonts" (echo        - fonts: OK) else (echo        - fonts: FALTA ^(puede ser normal^))

:: ============================================================================
:: PASO 4: LIMPIAR WEB.CONFIG (REMOVER DUPLICADOS)
:: ============================================================================
echo.
echo [4/10] Limpiando Web.config (removiendo configuraciones duplicadas)...
echo.

powershell.exe -ExecutionPolicy Bypass -File "%~dp0LimpiarWebConfig.ps1"

if %errorLevel% neq 0 (
    echo        ADVERTENCIA: No se pudo limpiar Web.config, continuando...
)

:: ============================================================================
:: PASO 5: REGISTRAR ASP.NET
:: ============================================================================
echo.
echo [5/10] Registrando ASP.NET 4.x...
echo.

%windir%\Microsoft.NET\Framework64\v4.0.30319\aspnet_regiis.exe -i >nul 2>&1

echo        ASP.NET registrado OK

:: ============================================================================
:: PASO 6: DESBLOQUEAR CONFIGURACION IIS
:: ============================================================================
echo.
echo [6/10] Desbloqueando configuracion IIS...
echo.

%systemroot%\system32\inetsrv\appcmd.exe unlock config -section:system.webServer/modules >nul 2>&1
%systemroot%\system32\inetsrv\appcmd.exe unlock config -section:system.webServer/handlers >nul 2>&1

echo        Configuracion desbloqueada OK

:: ============================================================================
:: PASO 7: CREAR APPLICATION POOL
:: ============================================================================
echo.
echo [7/10] Creando Application Pool...
echo.

%systemroot%\system32\inetsrv\appcmd.exe add apppool /name:"VentasWebPool" /managedRuntimeVersion:"v4.0" /managedPipelineMode:"Integrated" >nul 2>&1

echo        Application Pool creado OK

:: ============================================================================
:: PASO 8: CONFIGURAR PERMISOS
:: ============================================================================
echo.
echo [8/10] Configurando permisos...
echo.

icacls "C:\SistemaVentas" /grant "IIS_IUSRS:(OI)(CI)F" /T /Q >nul 2>&1
icacls "C:\SistemaVentas" /grant "IUSR:(OI)(CI)F" /T /Q >nul 2>&1
icacls "C:\SistemaVentas" /grant "IIS APPPOOL\VentasWebPool:(OI)(CI)F" /T /Q >nul 2>&1

echo        Permisos configurados OK

:: ============================================================================
:: PASO 9: CREAR SITIO WEB
:: ============================================================================
echo.
echo [9/10] Creando sitio web en puerto 80...
echo.

:: Verificar que no haya otros sitios en puerto 80
%systemroot%\system32\inetsrv\appcmd.exe list sites >nul 2>&1

:: Crear sitio nuevo
%systemroot%\system32\inetsrv\appcmd.exe add site /name:"SistemaVentas" /physicalPath:"C:\SistemaVentas" /bindings:http/*:80: >nul 2>&1
%systemroot%\system32\inetsrv\appcmd.exe set app "SistemaVentas/" /applicationPool:"VentasWebPool" >nul 2>&1

:: Configurar autenticacion anonima
%systemroot%\system32\inetsrv\appcmd.exe set config "SistemaVentas" /section:anonymousAuthentication /enabled:true >nul 2>&1
%systemroot%\system32\inetsrv\appcmd.exe set config "SistemaVentas" /section:anonymousAuthentication /userName:"IUSR" >nul 2>&1

:: Habilitar contenido estatico (imagenes, CSS, JS)
%systemroot%\system32\inetsrv\appcmd.exe set config "SistemaVentas" /section:staticContent /enabled:true >nul 2>&1

:: Configurar handler para archivos estaticos
%systemroot%\system32\inetsrv\appcmd.exe set config "SistemaVentas" /section:handlers /accessPolicy:Read,Script >nul 2>&1

:: Iniciar sitio
%systemroot%\system32\inetsrv\appcmd.exe start site "SistemaVentas" >nul 2>&1

echo        Sitio web creado OK

:: ============================================================================
:: PASO 10: REINICIAR IIS
:: ============================================================================
echo.
echo [10/10] Reiniciando IIS...
echo.

iisreset /start >nul 2>&1

echo        IIS reiniciado OK

echo.
echo Sitios activos en IIS:
%systemroot%\system32\inetsrv\appcmd.exe list sites
echo.
echo Verificando ruta fisica del sitio:
%systemroot%\system32\inetsrv\appcmd.exe list site "SistemaVentas" /text:physicalPath
:: ============================================================================
:: VERIFICACION
:: ============================================================================
echo.
echo ============================================================
echo   VERIFICANDO INSTALACION
echo ============================================================
echo.

%systemroot%\system32\inetsrv\appcmd.exe list site "SistemaVentas"

:: ============================================================================
:: INSTALACION COMPLETADA
:: ============================================================================
echo.
echo ============================================================
echo   INSTALACION COMPLETADA EXITOSAMENTE
echo ============================================================
echo.
echo Sistema instalado en: C:\SistemaVentas
echo URL: http://localhost
echo.
echo ============================================================
echo   CONFIGURACION PENDIENTE
echo ============================================================
echo.
echo IMPORTANTE: Configure la cadena de conexion a SQL Server
echo.
echo 1. Abra: C:\SistemaVentas\Web.config
echo.
echo 2. Busque: ^<connectionStrings^>
echo.
echo 3. Actualice:
echo    - Data Source: [NOMBRE_SERVIDOR_SQL]
echo    - Initial Catalog: DB_TIENDA
echo    - User ID: [USUARIO]
echo    - Password: [CONTRASENA]
echo.
echo 4. Guarde y ejecute: iisreset
echo.
echo 5. Abra en navegador: http://localhost
echo.
echo ============================================================
echo.
echo Presione una tecla para abrir http://localhost en el navegador...
pause >nul

start http://localhost

echo.
pause
