# ============================================================================
# SCRIPT DE INSTALACION AUTOMATICA
# Venta por Gramaje y Descomposicion de Productos
# ============================================================================

Write-Host "=============================================" -ForegroundColor Cyan
Write-Host "INSTALACION: GRAMAJE Y DESCOMPOSICION" -ForegroundColor Cyan
Write-Host "=============================================" -ForegroundColor Cyan
Write-Host ""

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
    "VentasWeb\Controllers\DescomposicionProductoController.cs"
)

$todosPresentes = $true
foreach ($archivo in $archivosRequeridos) {
    $rutaCompleta = Join-Path $projectPath $archivo
    if (Test-Path $rutaCompleta) {
        Write-Host "  OK: $archivo" -ForegroundColor Green
    } else {
        Write-Host "  FALTA: $archivo" -ForegroundColor Red
        $todosPresentes = $false
    }
}

Write-Host ""
if ($todosPresentes) {
    Write-Host "TODOS LOS ARCHIVOS ESTAN PRESENTES" -ForegroundColor Green
} else {
    Write-Host "FALTAN ARCHIVOS - Revise la instalacion" -ForegroundColor Red
}
Write-Host ""

# ============================================================================
# PASO 2: MOSTRAR INSTRUCCIONES SQL
# ============================================================================
Write-Host "=============================================" -ForegroundColor Cyan
Write-Host "SIGUIENTE PASO: EJECUTAR SCRIPTS SQL" -ForegroundColor Cyan
Write-Host "=============================================" -ForegroundColor Cyan
Write-Host ""

Write-Host "Abra SQL Server Management Studio y ejecute en orden:" -ForegroundColor Yellow
Write-Host ""
Write-Host "1. Archivo ubicado en:" -ForegroundColor White
Write-Host "   $projectPath\Utilidad\SQL Server\" -ForegroundColor Gray
Write-Host "   Archivo: 024_VENTA_POR_GRAMAJE_Y_DESCOMPOSICION.sql" -ForegroundColor Cyan
Write-Host ""
Write-Host "2. Luego ejecute:" -ForegroundColor White
Write-Host "   Archivo: 024b_ACTUALIZAR_SP_BUSCAR_PRODUCTO_POS.sql" -ForegroundColor Cyan
Write-Host ""
Write-Host "3. (Opcional) Para datos de ejemplo:" -ForegroundColor White
Write-Host "   Archivo: DATOS_EJEMPLO_GRAMAJE_Y_DESCOMPOSICION.sql" -ForegroundColor Cyan
Write-Host ""

# ============================================================================
# PASO 3: MOSTRAR INSTRUCCIONES VISUAL STUDIO
# ============================================================================
Write-Host "=============================================" -ForegroundColor Cyan
Write-Host "DESPUES: COMPILAR EN VISUAL STUDIO" -ForegroundColor Cyan
Write-Host "=============================================" -ForegroundColor Cyan
Write-Host ""

Write-Host "1. Abrir: VentasWeb.sln" -ForegroundColor Yellow
Write-Host "2. Presionar: Ctrl + Shift + B" -ForegroundColor Yellow
Write-Host "   O Menu: Compilar -> Recompilar solucion" -ForegroundColor Gray
Write-Host ""

# ============================================================================
# PASO 4: VERIFICACION
# ============================================================================
Write-Host "=============================================" -ForegroundColor Cyan
Write-Host "FINALMENTE: VERIFICAR INSTALACION" -ForegroundColor Cyan
Write-Host "=============================================" -ForegroundColor Cyan
Write-Host ""

Write-Host "Ejecute en SQL Server:" -ForegroundColor Yellow
Write-Host "   Archivo: VERIFICAR_INSTALACION.sql" -ForegroundColor Cyan
Write-Host ""
Write-Host "Este script verifica que todo este instalado correctamente" -ForegroundColor Gray
Write-Host ""

# ============================================================================
# RESUMEN
# ============================================================================
Write-Host "=============================================" -ForegroundColor Cyan
Write-Host "RESUMEN DE PASOS" -ForegroundColor Cyan
Write-Host "=============================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "1. Ejecutar scripts SQL (3 archivos)" -ForegroundColor White
Write-Host "2. Compilar proyecto en Visual Studio" -ForegroundColor White
Write-Host "3. Verificar instalacion con script SQL" -ForegroundColor White
Write-Host "4. Configurar productos y probar" -ForegroundColor White
Write-Host ""
Write-Host "Documentacion: README_GRAMAJE_DESCOMPOSICION.md" -ForegroundColor Gray
Write-Host ""
Write-Host "LISTO PARA INSTALAR" -ForegroundColor Green
Write-Host ""
