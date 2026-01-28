# =============================================
# SCRIPT DE CONFIGURACIÓN RÁPIDA
# Preparar sistema para producción
# =============================================

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "CONFIGURACIÓN RÁPIDA - SISTEMA POS" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

$baseDir = "c:\Users\Rafael Lopez\Documents\SistemaVentasTienda"
$errores = 0

# =============================================
# 1. ACTUALIZAR WEB.CONFIG
# =============================================
Write-Host "1. Actualizando Web.config..." -ForegroundColor Yellow

$webConfig = Join-Path $baseDir "VentasWeb\Web.config"
if (Test-Path $webConfig) {
    $content = Get-Content $webConfig -Raw
    
    # Cambiar debug a false
    $content = $content -replace '<compilation debug="true"', '<compilation debug="false"'
    
    # Cambiar targetFramework a 4.6.2
    $content = $content -replace 'targetFramework="4\.6"', 'targetFramework="4.6.2"'
    
    Set-Content $webConfig $content -Encoding UTF8
    Write-Host "   OK Web.config actualizado" -ForegroundColor Green
} else {
    Write-Host "   FALTA No se encontro Web.config" -ForegroundColor Red
    $errores++
}

# =============================================
# 2. VERIFICAR BASE DE DATOS
# =============================================
Write-Host ""
Write-Host "2. Verificando base de datos..." -ForegroundColor Yellow

try {
    $testConn = "SELECT CASE WHEN DB_ID('DB_TIENDA') IS NOT NULL THEN 'EXISTE' ELSE 'NO_EXISTE' END AS Estado"
    
    $resultado = sqlcmd -S "." -Q $testConn -h -1 2>&1
    
    if ($resultado -like "*EXISTE*") {
        Write-Host "   OK Base de datos DB_TIENDA existe" -ForegroundColor Green
    } else {
        Write-Host "   FALTA Base de datos DB_TIENDA no existe" -ForegroundColor Yellow
        Write-Host "   Ejecuta: sqlcmd -Q CREATE DATABASE DB_TIENDA" -ForegroundColor Gray
        $errores++
    }
} catch {
    Write-Host "   ADVERTENCIA No se pudo conectar a SQL Server" -ForegroundColor Yellow
    Write-Host "   Verifica que SQL Server este corriendo" -ForegroundColor Gray
}

# =============================================
# 3. VERIFICAR STORED PROCEDURE CRÍTICO
# =============================================
Write-Host ""
Write-Host "3. Verificando sp_ReporteUtilidadDiaria..." -ForegroundColor Yellow

try {
    $testSP = "USE DB_TIENDA; SELECT CASE WHEN OBJECT_ID('sp_ReporteUtilidadDiaria', 'P') IS NOT NULL THEN 'EXISTE' ELSE 'NO_EXISTE' END AS Estado"
    
    $resultado = sqlcmd -S "." -Q $testSP -h -1 2>&1
    
    if ($resultado -like "*EXISTE*") {
        Write-Host "   OK sp_ReporteUtilidadDiaria existe" -ForegroundColor Green
    } else {
        Write-Host "   FALTA sp_ReporteUtilidadDiaria NO EXISTE" -ForegroundColor Red
        Write-Host "   EJECUTA: sqlcmd -i CREAR_SP_REPORTE_UTILIDAD_DIARIA.sql" -ForegroundColor Yellow
        $errores++
    }
} catch {
    Write-Host "   ADVERTENCIA No se pudo verificar" -ForegroundColor Yellow
}

# =============================================
# 4. VERIFICAR COMPILACIÓN
# =============================================
Write-Host ""
Write-Host "4. Verificando DLL compilado..." -ForegroundColor Yellow

$dll = Join-Path $baseDir "VentasWeb\bin\VentasWeb.dll"
if (Test-Path $dll) {
    $dllInfo = Get-Item $dll
    $tamano = [math]::Round($dllInfo.Length / 1KB, 2)
    Write-Host "   OK VentasWeb.dll existe ($tamano KB)" -ForegroundColor Green
    Write-Host "   Ultima compilacion: $($dllInfo.LastWriteTime)" -ForegroundColor Gray
} else {
    Write-Host "   FALTA VentasWeb.dll NO existe" -ForegroundColor Red
    Write-Host "   EJECUTA: .\DESPLEGAR_PRODUCCION.ps1" -ForegroundColor Yellow
    $errores++
}

# =============================================
# 5. VERIFICAR DEPENDENCIAS EPPLUS
# =============================================
Write-Host ""
Write-Host "5. Verificando dependencias EPPlus..." -ForegroundColor Yellow

$epplus = Join-Path $baseDir "packages\EPPlus.7.0.0\lib\net462\EPPlus.dll"
if (Test-Path $epplus) {
    Write-Host "   OK EPPlus.dll encontrado" -ForegroundColor Green
} else {
    Write-Host "   FALTA EPPlus.dll NO encontrado" -ForegroundColor Red
    $errores++
}

# =============================================
# RESUMEN
# =============================================
Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "RESUMEN DE CONFIGURACIÓN" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

if ($errores -eq 0) {
    Write-Host ""
    Write-Host "*** SISTEMA LISTO PARA PRODUCCION ***" -ForegroundColor Green
    Write-Host ""
    Write-Host "Pasos siguientes:" -ForegroundColor Cyan
    Write-Host "1. Actualiza ConnectionString en Web.config con tus datos" -ForegroundColor Gray
    Write-Host "2. Actualiza SMTP_Username y SMTP_Password en Web.config" -ForegroundColor Gray
    Write-Host "3. Ejecuta: .\DESPLEGAR_PRODUCCION.ps1" -ForegroundColor Gray
    Write-Host "4. Configura IIS y prueba: http://localhost/VentasWeb" -ForegroundColor Gray
} else {
    Write-Host ""
    $mensajeError = "*** CONFIGURACION INCOMPLETA (" + $errores + " errores) ***"
    Write-Host $mensajeError -ForegroundColor Yellow
    Write-Host ""
    Write-Host "Revisa los errores arriba y:" -ForegroundColor Cyan
    Write-Host "1. Crea la base de datos si no existe" -ForegroundColor Gray
    Write-Host "2. Ejecuta CREAR_SP_REPORTE_UTILIDAD_DIARIA.sql" -ForegroundColor Gray
    Write-Host "3. Compila el proyecto" -ForegroundColor Gray
}

Write-Host ""
Write-Host "Documentacion completa: ANALISIS_COMPLETITUD_SISTEMA.md" -ForegroundColor Gray
Write-Host ""
