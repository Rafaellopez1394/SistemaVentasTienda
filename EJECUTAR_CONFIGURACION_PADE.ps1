# =====================================================
# Script: Ejecutar Configuración PADE/Prodigia
# Descripción: Ejecuta el SQL de configuración
# =====================================================

Write-Host "=========================================" -ForegroundColor Cyan
Write-Host "EJECUTANDO CONFIGURACIÓN PADE/PRODIGIA" -ForegroundColor Cyan
Write-Host "=========================================" -ForegroundColor Cyan
Write-Host ""

$scriptPath = Join-Path $PSScriptRoot "CONFIGURAR_PADE_PRODIGIA.sql"

if (-not (Test-Path $scriptPath)) {
    Write-Host "❌ ERROR: No se encontró el archivo $scriptPath" -ForegroundColor Red
    exit 1
}

Write-Host "✅ Script SQL encontrado: $scriptPath" -ForegroundColor Green
Write-Host ""

# Intentar diferentes conexiones
$connectionStrings = @(
    "localhost",
    "(localdb)\MSSQLLocalDB",
    "localhost\SQLEXPRESS",
    ".\SQLEXPRESS",
    "(local)",
    "."
)

$success = $false

foreach ($server in $connectionStrings) {
    Write-Host "🔍 Intentando conectar a: $server" -ForegroundColor Yellow
    
    try {
        $result = sqlcmd -S $server -d DB_TIENDA -E -i $scriptPath -b 2>&1
        
        if ($LASTEXITCODE -eq 0) {
            Write-Host "✅ CONEXIÓN EXITOSA a $server" -ForegroundColor Green
            Write-Host ""
            Write-Host "=========================================" -ForegroundColor Green
            Write-Host "RESULTADO DE LA EJECUCIÓN" -ForegroundColor Green
            Write-Host "=========================================" -ForegroundColor Green
            Write-Host $result
            $success = $true
            break
        }
    }
    catch {
        Write-Host "❌ Falló conexión a $server" -ForegroundColor Red
    }
}

if (-not $success) {
    Write-Host ""
    Write-Host "=========================================" -ForegroundColor Red
    Write-Host "NO SE PUDO CONECTAR A SQL SERVER" -ForegroundColor Red
    Write-Host "=========================================" -ForegroundColor Red
    Write-Host ""
    Write-Host "Opciones:" -ForegroundColor Yellow
    Write-Host "1. Abre SQL Server Management Studio (SSMS)" -ForegroundColor White
    Write-Host "2. Conéctate a tu servidor" -ForegroundColor White
    Write-Host "3. Abre el archivo: CONFIGURAR_PADE_PRODIGIA.sql" -ForegroundColor White
    Write-Host "4. Ejecuta el script manualmente (F5)" -ForegroundColor White
    Write-Host ""
    Write-Host "O verifica que SQL Server esté en ejecución:" -ForegroundColor Yellow
    Write-Host "   - Busca 'Servicios' en Windows" -ForegroundColor White
    Write-Host "   - Busca 'SQL Server (MSSQLSERVER)' o 'SQL Server (SQLEXPRESS)'" -ForegroundColor White
    Write-Host "   - Asegúrate que esté en estado 'En ejecución'" -ForegroundColor White
    Write-Host ""
}
else {
    Write-Host ""
    Write-Host "=========================================" -ForegroundColor Green
    Write-Host "✅ CONFIGURACIÓN COMPLETADA" -ForegroundColor Green
    Write-Host "=========================================" -ForegroundColor Green
    Write-Host ""
    Write-Host "Próximos pasos:" -ForegroundColor Cyan
    Write-Host "1. Contacta a soporte@pade.mx para obtener credenciales de prueba" -ForegroundColor White
    Write-Host "2. Accede a https://pruebas.pade.mx para subir certificados CSD" -ForegroundColor White
    Write-Host "3. Actualiza las credenciales en la base de datos:" -ForegroundColor White
    Write-Host ""
    Write-Host "   UPDATE ConfiguracionProdigia" -ForegroundColor Gray
    Write-Host "   SET Usuario = 'TU_USUARIO', Password = 'TU_PASSWORD', Contrato = 'TU_CONTRATO'" -ForegroundColor Gray
    Write-Host "   WHERE ConfiguracionID = 1" -ForegroundColor Gray
    Write-Host ""
    Write-Host "Presiona cualquier tecla para continuar..." -ForegroundColor Yellow
    $null = $Host.UI.RawUI.ReadKey('NoEcho,IncludeKeyDown')
}

Write-Host "Presiona cualquier tecla para salir..." -ForegroundColor Yellow
$null = $Host.UI.RawUI.ReadKey('NoEcho,IncludeKeyDown')
