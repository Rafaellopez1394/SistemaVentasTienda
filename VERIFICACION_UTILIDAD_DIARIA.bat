@echo off
REM Verificación de implementación del Reporte de Utilidad Diaria

echo.
echo ======================================
echo VERIFICACION DE IMPLEMENTACION
echo Reporte de Utilidad Diaria
echo ======================================
echo.

echo [1/4] Verificando Stored Procedure SQL...
IF EXIST "Utilidad\SQL Server\050_REPORTE_UTILIDAD_DIARIA.sql" (
    echo     [OK] SQL Procedure encontrado
) ELSE (
    echo     [ERROR] Falta SQL Procedure
)

echo.
echo [2/4] Verificando Modelos C#...
IF EXIST "CapaModelo\ReporteUtilidadDiaria.cs" (
    echo     [OK] Modelo principal encontrado
) ELSE (
    echo     [ERROR] Falta modelo principal
)

IF EXIST "CapaDatos\CD_ReporteUtilidadDiaria.cs" (
    echo     [OK] Capa de datos encontrada
) ELSE (
    echo     [ERROR] Falta capa de datos
)

echo.
echo [3/4] Verificando Controlador...
IF EXIST "VentasWeb\Controllers\ReporteController.cs" (
    echo     [OK] Controlador encontrado
    findstr /M "ObtenerPreviewUtilidadDiaria" "VentasWeb\Controllers\ReporteController.cs" >nul
    IF %ERRORLEVEL% EQU 0 (
        echo     [OK] Accion Preview implementada
    ) ELSE (
        echo     [ERROR] Accion Preview no encontrada
    )
    findstr /M "ExportarUtilidadDiaria" "VentasWeb\Controllers\ReporteController.cs" >nul
    IF %ERRORLEVEL% EQU 0 (
        echo     [OK] Accion Export implementada
    ) ELSE (
        echo     [ERROR] Accion Export no encontrada
    )
) ELSE (
    echo     [ERROR] Controlador no encontrado
)

echo.
echo [4/4] Verificando Vista Razor...
IF EXIST "VentasWeb\Views\Reporte\UtilidadDiaria.cshtml" (
    echo     [OK] Vista encontrada
) ELSE (
    echo     [ERROR] Falta vista
)

echo.
echo ======================================
echo CHECKLIST DE PASOS SIGUIENTES:
echo ======================================
echo.
echo 1. CREAR STORED PROCEDURE EN BD:
echo    - Abrir SQL Server Management Studio
echo    - Conectar a DB_TIENDA
echo    - Ejecutar: Utilidad\SQL Server\050_REPORTE_UTILIDAD_DIARIA.sql
echo.
echo 2. COMPILAR PROYECTO:
echo    - Abrir Visual Studio
echo    - Cargar VentasWeb.sln
echo    - Build ^> Rebuild Solution
echo    - Verificar que no hay errores
echo.
echo 3. EJECUTAR APLICACION:
echo    - Presionar F5 para debug
echo    - Navegar a: http://localhost:[PORT]/Reporte/UtilidadDiaria
echo    - Verificar que la página se carga correctamente
echo.
echo 4. PROBAR PREVIEW:
echo    - Seleccionar una fecha (default: hoy)
echo    - Hacer clic en "Ver Preview"
echo    - Verificar que aparecen datos en las secciones
echo.
echo 5. PROBAR EXCEL:
echo    - Hacer clic en "Descargar Excel"
echo    - Abrir archivo descargado (UtilidadDiaria_YYYYMMDD.xlsx)
echo    - Verificar formato y datos
echo.
echo ======================================
echo.
pause
