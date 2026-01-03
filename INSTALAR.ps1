# ============================================================================
# SCRIPT DE INSTALACIÓN AUTOMÁTICA
# Venta por Gramaje y Descomposición de Productos
# ============================================================================

Write-Host "=============================================" -ForegroundColor Cyan
Write-Host "INSTALACIÓN: GRAMAJE Y DESCOMPOSICIÓN" -ForegroundColor Cyan
Write-Host "=============================================" -ForegroundColor Cyan
Write-Host ""

$ErrorActionPreference = "Stop"
$projectPath = "C:\Users\Rafael Lopez\Documents\SistemaVentasTienda"

# ============================================================================
# PASO 1: VERIFICAR ARCHIVOS
# ============================================================================
Write-Host "PASO 1: Verificando archivos..." -ForegroundColor Yellow

$archivosRequeridos = @(
    "Utilidad\SQL Server\024_VENTA_POR_GRAMAJE_Y_DESCOMPOSICION.sql",
    "Utilidad\SQL Server\024b_ACTUALIZAR_SP_BUSCAR_PRODUCTO_POS.sql",
    "CapaModelo\DescomposicionProducto.cs",
    "CapaDatos\CD_DescomposicionProducto.cs",
    "VentasWeb\Controllers\DescomposicionProductoController.cs",
    "VentasWeb\Views\DescomposicionProducto\Index.cshtml",
    "VentasWeb\Scripts\Views\VentaPOS_Gramaje.js",
    "VentasWeb\Scripts\descomposicion-producto.js"
)

$faltantes = @()
foreach ($archivo in $archivosRequeridos) {
    $rutaCompleta = Join-Path $projectPath $archivo
    if (Test-Path $rutaCompleta) {
        Write-Host "  ✓ $archivo" -ForegroundColor Green
    } else {
        Write-Host "  ✗ $archivo" -ForegroundColor Red
        $faltantes += $archivo
    }
}

if ($faltantes.Count -gt 0) {
    Write-Host ""
    Write-Host "ERROR: Faltan archivos requeridos" -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "✓ Todos los archivos están presentes" -ForegroundColor Green
Write-Host ""

# ============================================================================
# PASO 2: GENERAR SCRIPT SQL CONSOLIDADO
# ============================================================================
Write-Host "PASO 2: Generando script SQL consolidado..." -ForegroundColor Yellow

$sqlConsolidado = @"
-- ============================================================================
-- SCRIPT CONSOLIDADO DE INSTALACIÓN
-- Ejecutar en SQL Server Management Studio
-- Base de datos: DBVENTAS_WEB
-- ============================================================================

USE DBVENTAS_WEB
GO

PRINT '============================================='
PRINT 'INSTALACIÓN: GRAMAJE Y DESCOMPOSICIÓN'
PRINT '============================================='
PRINT ''

-- Leer script 1
:r "$projectPath\Utilidad\SQL Server\024_VENTA_POR_GRAMAJE_Y_DESCOMPOSICION.sql"

PRINT ''
PRINT '============================================='
PRINT 'SCRIPT 1/2 COMPLETADO'
PRINT '============================================='
PRINT ''

-- Leer script 2
:r "$projectPath\Utilidad\SQL Server\024b_ACTUALIZAR_SP_BUSCAR_PRODUCTO_POS.sql"

PRINT ''
PRINT '============================================='
PRINT 'SCRIPT 2/2 COMPLETADO'
PRINT '============================================='
PRINT ''
PRINT '✓ INSTALACIÓN SQL COMPLETADA'
PRINT ''
PRINT 'Siguiente paso: Compilar proyecto en Visual Studio'
PRINT ''
GO
"@

$rutaSqlConsolidado = Join-Path $projectPath "INSTALAR_SQL.sql"
$sqlConsolidado | Out-File -FilePath $rutaSqlConsolidado -Encoding UTF8
Write-Host "  ✓ Script SQL consolidado creado: INSTALAR_SQL.sql" -ForegroundColor Green
Write-Host ""

# ============================================================================
# PASO 3: VERIFICAR VISUAL STUDIO
# ============================================================================
Write-Host "PASO 3: Buscando Visual Studio..." -ForegroundColor Yellow

$vsPath = "C:\Program Files\Microsoft Visual Studio"
if (Test-Path $vsPath) {
    Write-Host "  ✓ Visual Studio encontrado" -ForegroundColor Green
} else {
    Write-Host "  ⚠ Visual Studio no encontrado en ruta predeterminada" -ForegroundColor Yellow
}
Write-Host ""

# ============================================================================
# PASO 4: VERIFICAR SQL SERVER
# ============================================================================
Write-Host "PASO 4: Verificando SQL Server..." -ForegroundColor Yellow

$servicioSQL = Get-Service -Name "MSSQL*" -ErrorAction SilentlyContinue | Select-Object -First 1
if ($servicioSQL) {
    if ($servicioSQL.Status -eq "Running") {
        Write-Host "  ✓ SQL Server está en ejecución" -ForegroundColor Green
    } else {
        Write-Host "  ⚠ SQL Server está detenido" -ForegroundColor Yellow
        Write-Host "    Iniciando SQL Server..." -ForegroundColor Yellow
        try {
            Start-Service $servicioSQL.Name
            Write-Host "  ✓ SQL Server iniciado correctamente" -ForegroundColor Green
        } catch {
            Write-Host "  ✗ No se pudo iniciar SQL Server automáticamente" -ForegroundColor Red
            Write-Host "    Debe iniciar SQL Server manualmente" -ForegroundColor Yellow
        }
    }
} else {
    Write-Host "  ⚠ No se detectó servicio de SQL Server" -ForegroundColor Yellow
}
Write-Host ""

# ============================================================================
# RESUMEN E INSTRUCCIONES
# ============================================================================
Write-Host "=============================================" -ForegroundColor Cyan
Write-Host "PREPARACIÓN COMPLETADA" -ForegroundColor Cyan
Write-Host "=============================================" -ForegroundColor Cyan
Write-Host ""

Write-Host "SIGUIENTE: Ejecute estos comandos:" -ForegroundColor Yellow
Write-Host ""

Write-Host "1. INSTALAR SQL (En SQL Server Management Studio):" -ForegroundColor White
Write-Host "   Abrir: SQL Server Management Studio" -ForegroundColor Gray
Write-Host "   Conectar a: (local) o su servidor" -ForegroundColor Gray
Write-Host "   Archivo - Abrir - Archivo..." -ForegroundColor Gray
Write-Host "   Seleccionar: $rutaSqlConsolidado" -ForegroundColor Cyan
Write-Host "   Presionar: F5 o clic en Ejecutar" -ForegroundColor Gray
Write-Host ""

Write-Host "   O ejecutar scripts individuales en orden:" -ForegroundColor Gray
Write-Host "   Script a: 024_VENTA_POR_GRAMAJE_Y_DESCOMPOSICION.sql" -ForegroundColor Gray
Write-Host "   Script b: 024b_ACTUALIZAR_SP_BUSCAR_PRODUCTO_POS.sql" -ForegroundColor Gray
Write-Host ""

Write-Host "2. COMPILAR PROYECTO (En Visual Studio):" -ForegroundColor White
Write-Host "   Abrir: VentasWeb.sln" -ForegroundColor Gray
Write-Host "   Presionar: Ctrl + Shift + B" -ForegroundColor Cyan
Write-Host "   O: Menu Compilar - Recompilar solucion" -ForegroundColor Gray
Write-Host ""

Write-Host "3. PROBAR FUNCIONALIDADES:" -ForegroundColor White
Write-Host "   Paso a: Configurar un producto para gramaje" -ForegroundColor Gray
Write-Host "   Paso b: Ir al POS y buscar el producto" -ForegroundColor Gray
Write-Host "   Paso c: Probar venta por gramaje" -ForegroundColor Gray
Write-Host "   Paso d: Ir a Descomposicion de Productos" -ForegroundColor Gray
Write-Host "   Paso e: Probar descomposicion" -ForegroundColor Gray
Write-Host ""

Write-Host "=============================================" -ForegroundColor Cyan
Write-Host "ARCHIVOS GENERADOS:" -ForegroundColor Cyan
Write-Host "=============================================" -ForegroundColor Cyan
Write-Host "  - INSTALAR_SQL.sql (Script consolidado)" -ForegroundColor Green
Write-Host "  - CONFIGURAR_PRODUCTOS.sql (Configuracion)" -ForegroundColor Green
Write-Host "  - VERIFICAR_INSTALACION.sql (Verificacion)" -ForegroundColor Green
Write-Host ""

Write-Host "Documentacion disponible:" -ForegroundColor Yellow
Write-Host "  - README_GRAMAJE_DESCOMPOSICION.md" -ForegroundColor Gray
Write-Host "  - IMPLEMENTACION_RAPIDA.md" -ForegroundColor Gray
Write-Host "  - GUIA_VENTA_GRAMAJE_Y_DESCOMPOSICION.md" -ForegroundColor Gray
Write-Host ""

Write-Host "Listo para instalar!" -ForegroundColor Green
Write-Host ""
