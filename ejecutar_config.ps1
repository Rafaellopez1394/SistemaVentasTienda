Write-Host "Ejecutando configuracion PADE..." -ForegroundColor Cyan

$servers = @("localhost", "(localdb)\MSSQLLocalDB", "localhost\SQLEXPRESS", ".\SQLEXPRESS")

foreach ($srv in $servers) {
    Write-Host "Probando: $srv" -ForegroundColor Yellow
    $out = sqlcmd -S $srv -d DB_TIENDA -E -i "CONFIGURAR_PADE_PRODIGIA.sql" 2>&1
    if ($LASTEXITCODE -eq 0) {
        Write-Host "EXITOSO con $srv" -ForegroundColor Green
        Write-Host $out
        Write-Host ""
        Write-Host "COMPLETADO - Actualiza tus credenciales:" -ForegroundColor Green
        Write-Host "UPDATE ConfiguracionProdigia SET Usuario='xxx', Password='xxx', Contrato='xxx' WHERE ConfiguracionID=1"
        exit 0
    }
}

Write-Host "No se pudo conectar. Ejecuta manualmente en SQL Server Management Studio:" -ForegroundColor Red
Write-Host "Abre: CONFIGURAR_PADE_PRODIGIA.sql"
