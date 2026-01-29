# ========================================================================
# SCRIPT FINAL - PREPARAR SISTEMA PARA PRODUCCION
# Aplica todos los ajustes necesarios automáticamente
# ========================================================================

Write-Host ""
Write-Host "========================================================================" -ForegroundColor Cyan
Write-Host "               PREPARACION FINAL PARA PRODUCCION" -ForegroundColor Cyan
Write-Host "========================================================================" -ForegroundColor Cyan
Write-Host ""

$baseDir = "c:\Users\Rafael Lopez\Documents\SistemaVentasTienda"

# ========================================================================
# PASO 1: ACTUALIZAR WEB.CONFIG
# ========================================================================

Write-Host "[PASO 1/3] Actualizando Web.config para produccion..." -ForegroundColor Yellow
Write-Host ""

$webConfigPath = Join-Path $baseDir "VentasWeb\Web.config"

if (Test-Path $webConfigPath) {
    # Hacer backup primero
    $backupPath = $webConfigPath + ".backup." + (Get-Date -Format "yyyyMMdd_HHmmss")
    Copy-Item $webConfigPath $backupPath
    Write-Host "  Backup creado: $backupPath" -ForegroundColor Gray
    
    # Leer contenido
    $content = Get-Content $webConfigPath -Raw -Encoding UTF8
    
    # Aplicar cambios
    $cambios = 0
    
    # 1. Cambiar debug a false
    if ($content -match 'debug="true"') {
        $content = $content -replace 'debug="true"', 'debug="false"'
        Write-Host "  OK: debug cambiado a false" -ForegroundColor Green
        $cambios++
    } else {
        Write-Host "  INFO: debug ya estaba en false" -ForegroundColor Gray
    }
    
    # 2. Cambiar targetFramework a 4.6.2
    if ($content -match 'targetFramework="4\.6"[^\.2]') {
        $content = $content -replace 'targetFramework="4\.6"', 'targetFramework="4.6.2"'
        Write-Host "  OK: targetFramework cambiado a 4.6.2" -ForegroundColor Green
        $cambios++
    } else {
        Write-Host "  INFO: targetFramework ya estaba en 4.6.2" -ForegroundColor Gray
    }
    
    # Guardar cambios
    if ($cambios -gt 0) {
        Set-Content $webConfigPath $content -Encoding UTF8 -NoNewline
        Write-Host ""
        Write-Host "  COMPLETADO: $cambios cambios aplicados" -ForegroundColor Green
    } else {
        Write-Host ""
        Write-Host "  INFO: No se necesitaron cambios" -ForegroundColor Gray
    }
} else {
    Write-Host "  ERROR: Web.config no encontrado en: $webConfigPath" -ForegroundColor Red
    exit 1
}

# ========================================================================
# PASO 2: VERIFICAR CONFIGURACIONES CRITICAS
# ========================================================================

Write-Host ""
Write-Host "[PASO 2/3] Verificando configuraciones criticas..." -ForegroundColor Yellow
Write-Host ""

$warnings = 0

# Verificar ConnectionString
if ($content -match 'connectionString="([^"]+)"') {
    $connString = $matches[1]
    Write-Host "  ConnectionString encontrado:" -ForegroundColor Gray
    
    if ($connString -match 'Data Source=([^;]+)') {
        Write-Host "    Servidor: $($matches[1])" -ForegroundColor Gray
    }
    
    if ($connString -match 'Initial Catalog=([^;]+)') {
        Write-Host "    Base de datos: $($matches[1])" -ForegroundColor Gray
    }
    
    if ($connString -match 'User ID=([^;]+)') {
        Write-Host "    Usuario: $($matches[1])" -ForegroundColor Gray
    }
    
    # Probar conexión
    Write-Host ""
    Write-Host "  Probando conexion a SQL Server..." -ForegroundColor Gray
    
    if ($connString -match 'Data Source=([^;]+)') {
        $servidor = $matches[1]
        if ($connString -match 'User ID=([^;]+)') {
            $usuario = $matches[1]
            if ($connString -match 'Password=([^;]+)') {
                $password = $matches[1]
                
                try {
                    $null = sqlcmd -S $servidor -U $usuario -P $password -Q "SELECT @@VERSION" -h -1 2>&1
                    if ($LASTEXITCODE -eq 0) {
                        Write-Host "  OK: Conexion a SQL Server exitosa" -ForegroundColor Green
                    } else {
                        Write-Host "  ADVERTENCIA: No se pudo conectar a SQL Server" -ForegroundColor Yellow
                        Write-Host "  Verifica las credenciales en Web.config" -ForegroundColor Yellow
                        $warnings++
                    }
                } catch {
                    Write-Host "  ADVERTENCIA: Error al probar conexion" -ForegroundColor Yellow
                    $warnings++
                }
            }
        }
    }
} else {
    Write-Host "  ADVERTENCIA: ConnectionString no encontrado" -ForegroundColor Yellow
    $warnings++
}

# Verificar SMTP
Write-Host ""
if ($content -match 'SMTP_Username.*value="([^"]+)"') {
    $smtpUser = $matches[1]
    if ($smtpUser -like "*@*") {
        Write-Host "  OK: SMTP configurado ($smtpUser)" -ForegroundColor Green
    } else {
        Write-Host "  ADVERTENCIA: SMTP no configurado (correos no funcionaran)" -ForegroundColor Yellow
        Write-Host "    Si necesitas enviar correos, configura SMTP_Username y SMTP_Password" -ForegroundColor Gray
        $warnings++
    }
} else {
    Write-Host "  INFO: SMTP no configurado (opcional)" -ForegroundColor Gray
}

# ========================================================================
# PASO 3: VERIFICAR COMPILACION
# ========================================================================

Write-Host ""
Write-Host "[PASO 3/3] Verificando compilacion..." -ForegroundColor Yellow
Write-Host ""

$dllPath = Join-Path $baseDir "VentasWeb\bin\VentasWeb.dll"

if (Test-Path $dllPath) {
    $dllInfo = Get-Item $dllPath
    $tamanoKB = [math]::Round($dllInfo.Length / 1KB, 2)
    
    Write-Host "  OK: VentasWeb.dll encontrado" -ForegroundColor Green
    Write-Host "    Tamano: $tamanoKB KB" -ForegroundColor Gray
    Write-Host "    Fecha: $($dllInfo.LastWriteTime)" -ForegroundColor Gray
    
    # Verificar si es reciente (menos de 1 día)
    $horasDesdeCompilacion = (Get-Date) - $dllInfo.LastWriteTime
    if ($horasDesdeCompilacion.TotalHours -gt 24) {
        Write-Host ""
        Write-Host "  ADVERTENCIA: La compilacion tiene mas de 1 dia" -ForegroundColor Yellow
        Write-Host "  Considera recompilar con: .\DESPLEGAR_PRODUCCION.ps1" -ForegroundColor Gray
        $warnings++
    }
} else {
    Write-Host "  ERROR: VentasWeb.dll no encontrado" -ForegroundColor Red
    Write-Host "  Ejecuta: .\DESPLEGAR_PRODUCCION.ps1" -ForegroundColor Yellow
    exit 1
}

# ========================================================================
# RESUMEN FINAL
# ========================================================================

Write-Host ""
Write-Host "========================================================================" -ForegroundColor Cyan
Write-Host "                           RESUMEN FINAL" -ForegroundColor Cyan
Write-Host "========================================================================" -ForegroundColor Cyan
Write-Host ""

if ($warnings -eq 0) {
    Write-Host "                   *** SISTEMA LISTO PARA PRODUCCION ***" -ForegroundColor Green
    Write-Host ""
    Write-Host "  El sistema ha sido configurado correctamente y esta listo para usarse." -ForegroundColor White
    Write-Host ""
    Write-Host "  PROXIMOS PASOS:" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "  1. Desplegar en IIS:" -ForegroundColor White
    Write-Host "     .\DESPLEGAR_PRODUCCION.ps1" -ForegroundColor Gray
    Write-Host ""
    Write-Host "  2. Abrir en navegador:" -ForegroundColor White
    Write-Host "     http://localhost/VentasWeb" -ForegroundColor Gray
    Write-Host ""
    Write-Host "  3. Probar funcionalidades:" -ForegroundColor White
    Write-Host "     - Login" -ForegroundColor Gray
    Write-Host "     - Crear venta" -ForegroundColor Gray
    Write-Host "     - Generar reporte" -ForegroundColor Gray
    Write-Host ""
} else {
    Write-Host "                SISTEMA CONFIGURADO CON ADVERTENCIAS" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "  Total de advertencias: $warnings" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "  El sistema funcionara, pero revisa las advertencias arriba." -ForegroundColor White
    Write-Host ""
    Write-Host "  PROXIMOS PASOS:" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "  1. Revisa las advertencias y corrige si es necesario" -ForegroundColor Gray
    Write-Host "  2. Despliega en IIS: .\DESPLEGAR_PRODUCCION.ps1" -ForegroundColor Gray
    Write-Host "  3. Prueba el sistema: http://localhost/VentasWeb" -ForegroundColor Gray
    Write-Host ""
}

Write-Host "========================================================================" -ForegroundColor Cyan
Write-Host ""

# Mostrar backup info
if (Test-Path $backupPath) {
    Write-Host "NOTA: Backup de Web.config guardado en:" -ForegroundColor Gray
    Write-Host "  $backupPath" -ForegroundColor Gray
    Write-Host ""
}

Write-Host "Para mas informacion: RESUMEN_FINAL.md" -ForegroundColor Gray
Write-Host ""
