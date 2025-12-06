# Script para ejecutar los scripts SQL de MapeoIVA y CatalogoContable
# Requisitos: SQL Server Management Tools (sqlcmd) instalado

$connectionString = "Data Source=.;Initial Catalog=DB_TIENDA;Integrated Security=True"
$sqlScriptsPath = "C:\Users\Rafael Lopez\Documents\SistemaVentasTienda\Utilidad\SQL Server"

# Array de scripts a ejecutar en orden
$scripts = @(
    "01_CrearTablaMapeoIVA.sql",
    "02_CrearCatalogoContable.sql"
)

Write-Host "Iniciando ejecucion de scripts SQL..." -ForegroundColor Green
Write-Host "Conexion: $connectionString" -ForegroundColor Cyan

foreach ($script in $scripts) {
    $scriptPath = Join-Path $sqlScriptsPath $script
    
    if (Test-Path $scriptPath) {
        Write-Host "Ejecutando: $script" -ForegroundColor Yellow
        
        try {
            sqlcmd -S . -d DB_TIENDA -i "$scriptPath" -E
            Write-Host "Ejecutado exitosamente: $script" -ForegroundColor Green
        }
        catch {
            Write-Host "Error ejecutando $script : $_" -ForegroundColor Red
        }
    }
    else {
        Write-Host "Archivo no encontrado: $scriptPath" -ForegroundColor Red
    }
}

Write-Host "Proceso completado." -ForegroundColor Green
