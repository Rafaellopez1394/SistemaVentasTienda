# Script de verificacion del estado del sistema
Write-Host "============================================" -ForegroundColor Cyan
Write-Host "VERIFICACION DE ESTADO DEL SISTEMA POS" -ForegroundColor Cyan
Write-Host "============================================" -ForegroundColor Cyan
Write-Host ""

$errores = 0

# 1. Web.config
Write-Host "[1] Verificando Web.config..." -ForegroundColor Yellow
$webConfig = "c:\Users\Rafael Lopez\Documents\SistemaVentasTienda\VentasWeb\Web.config"
if (Test-Path $webConfig) {
    $content = Get-Content $webConfig -Raw
    
    if ($content -match 'debug="true"') {
        Write-Host "    ADVERTENCIA: debug=true (deberia ser false para produccion)" -ForegroundColor Yellow
    } else {
        Write-Host "    OK: debug=false" -ForegroundColor Green
    }
    
    if ($content -match 'targetFramework="4\.6\.2"') {
        Write-Host "    OK: targetFramework=4.6.2" -ForegroundColor Green
    } else {
        Write-Host "    ADVERTENCIA: targetFramework no es 4.6.2" -ForegroundColor Yellow
    }
} else {
    Write-Host "    ERROR: Web.config no encontrado" -ForegroundColor Red
    $errores++
}

# 2. DLL Compilado
Write-Host ""
Write-Host "[2] Verificando DLL compilado..." -ForegroundColor Yellow
$dll = "c:\Users\Rafael Lopez\Documents\SistemaVentasTienda\VentasWeb\bin\VentasWeb.dll"
if (Test-Path $dll) {
    $dllInfo = Get-Item $dll
    $tamanoKB = [math]::Round($dllInfo.Length / 1KB, 2)
    Write-Host "    OK: VentasWeb.dll encontrado" -ForegroundColor Green
    Write-Host "    Tamano: $tamanoKB KB" -ForegroundColor Gray
    Write-Host "    Fecha: $($dllInfo.LastWriteTime)" -ForegroundColor Gray
} else {
    Write-Host "    ERROR: VentasWeb.dll no encontrado" -ForegroundColor Red
    Write-Host "    Necesitas compilar el proyecto" -ForegroundColor Yellow
    $errores++
}

# 3. EPPlus
Write-Host ""
Write-Host "[3] Verificando EPPlus..." -ForegroundColor Yellow
$epplus = "c:\Users\Rafael Lopez\Documents\SistemaVentasTienda\packages\EPPlus.7.0.0\lib\net462\EPPlus.dll"
if (Test-Path $epplus) {
    Write-Host "    OK: EPPlus.dll encontrado" -ForegroundColor Green
} else {
    Write-Host "    ERROR: EPPlus.dll no encontrado" -ForegroundColor Red
    $errores++
}

# 4. Base de datos
Write-Host ""
Write-Host "[4] Verificando base de datos..." -ForegroundColor Yellow
try {
    $testDB = sqlcmd -S "." -Q "SELECT DB_ID('DB_TIENDA')" -h -1 2>&1
    if ($testDB -match "[0-9]+") {
        Write-Host "    OK: DB_TIENDA existe" -ForegroundColor Green
    } else {
        Write-Host "    ERROR: DB_TIENDA no existe" -ForegroundColor Red
        $errores++
    }
} catch {
    Write-Host "    ADVERTENCIA: No se pudo conectar a SQL Server" -ForegroundColor Yellow
}

# 5. Stored Procedure
Write-Host ""
Write-Host "[5] Verificando sp_ReporteUtilidadDiaria..." -ForegroundColor Yellow
try {
    $testSP = sqlcmd -S "." -d "DB_TIENDA" -Q "SELECT OBJECT_ID('sp_ReporteUtilidadDiaria', 'P')" -h -1 2>&1
    if ($testSP -match "[0-9]+") {
        Write-Host "    OK: sp_ReporteUtilidadDiaria existe" -ForegroundColor Green
    } else {
        Write-Host "    ERROR: sp_ReporteUtilidadDiaria no existe" -ForegroundColor Red
        Write-Host "    Ejecuta: sqlcmd -i CREAR_SP_REPORTE_UTILIDAD_DIARIA.sql" -ForegroundColor Yellow
        $errores++
    }
} catch {
    Write-Host "    ADVERTENCIA: No se pudo verificar" -ForegroundColor Yellow
}

# Resumen
Write-Host ""
Write-Host "============================================" -ForegroundColor Cyan
Write-Host "RESUMEN" -ForegroundColor Cyan
Write-Host "============================================" -ForegroundColor Cyan
Write-Host ""

if ($errores -eq 0) {
    Write-Host "ESTADO: LISTO PARA PRODUCCION" -ForegroundColor Green
    Write-Host ""
    Write-Host "Pasos siguientes:" -ForegroundColor Cyan
    Write-Host "  1. Actualiza ConnectionString en Web.config" -ForegroundColor Gray
    Write-Host "  2. Actualiza SMTP_Username y SMTP_Password" -ForegroundColor Gray
    Write-Host "  3. Despliega en IIS" -ForegroundColor Gray
} else {
    Write-Host "ESTADO: CONFIGURACION INCOMPLETA" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "Total de problemas: $errores" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "Revisa los errores arriba y corrigelos." -ForegroundColor Cyan
}

Write-Host ""
Write-Host "Ver documentacion: ANALISIS_COMPLETITUD_SISTEMA.md" -ForegroundColor Gray
Write-Host ""
