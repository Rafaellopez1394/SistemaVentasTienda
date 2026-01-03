# Script para configurar tasas de IVA en la base de datos
# Ejecutar desde PowerShell

Write-Host "================================" -ForegroundColor Cyan
Write-Host "CONFIGURACIÓN DE TASAS DE IVA" -ForegroundColor Cyan
Write-Host "================================" -ForegroundColor Cyan
Write-Host ""

# Ruta del script SQL
$scriptPath = "c:\Users\Rafael Lopez\Documents\SistemaVentasTienda\Utilidad\SQL Server\021_CONFIGURAR_TASAS_IVA.sql"

# Verificar que existe el archivo
if (-not (Test-Path $scriptPath)) {
    Write-Host "❌ ERROR: No se encuentra el archivo SQL" -ForegroundColor Red
    Write-Host "   Ruta esperada: $scriptPath" -ForegroundColor Yellow
    exit 1
}

# Solicitar datos de conexión
Write-Host "Ingrese los datos de conexión a SQL Server:" -ForegroundColor Yellow
Write-Host ""

$servidor = Read-Host "Servidor (por defecto: localhost)"
if ([string]::IsNullOrWhiteSpace($servidor)) {
    $servidor = "localhost"
}

$baseDatos = Read-Host "Base de datos (por defecto: DBVENTAS_WEB)"
if ([string]::IsNullOrWhiteSpace($baseDatos)) {
    $baseDatos = "DBVENTAS_WEB"
}

Write-Host ""
Write-Host "Tipo de autenticación:" -ForegroundColor Yellow
Write-Host "1. Autenticación de Windows (recomendado)"
Write-Host "2. Usuario y contraseña de SQL Server"
$tipoAuth = Read-Host "Seleccione (1 o 2)"

if ($tipoAuth -eq "2") {
    $usuario = Read-Host "Usuario SQL"
    $password = Read-Host "Contraseña" -AsSecureString
    $passwordPlain = [Runtime.InteropServices.Marshal]::PtrToStringAuto([Runtime.InteropServices.Marshal]::SecureStringToBSTR($password))
    $connectionString = "Server=$servidor;Database=$baseDatos;User Id=$usuario;Password=$passwordPlain;"
} else {
    $connectionString = "Server=$servidor;Database=$baseDatos;Integrated Security=True;"
}

Write-Host ""
Write-Host "Ejecutando script de configuración..." -ForegroundColor Yellow

try {
    # Usar sqlcmd para ejecutar el script
    $sqlcmdPath = "sqlcmd.exe"
    
    # Verificar si sqlcmd está disponible
    if (-not (Get-Command $sqlcmdPath -ErrorAction SilentlyContinue)) {
        Write-Host "❌ ERROR: sqlcmd.exe no está instalado o no está en el PATH" -ForegroundColor Red
        Write-Host ""
        Write-Host "Alternativa: Ejecute el script manualmente en SQL Server Management Studio" -ForegroundColor Yellow
        Write-Host "Ubicación: $scriptPath" -ForegroundColor Cyan
        exit 1
    }
    
    # Construir comando
    if ($tipoAuth -eq "2") {
        $result = & $sqlcmdPath -S $servidor -d $baseDatos -U $usuario -P $passwordPlain -i $scriptPath
    } else {
        $result = & $sqlcmdPath -S $servidor -d $baseDatos -E -i $scriptPath
    }
    
    # Mostrar resultado
    Write-Host ""
    Write-Host $result
    Write-Host ""
    
    Write-Host "✅ Script ejecutado exitosamente" -ForegroundColor Green
    Write-Host ""
    Write-Host "Próximos pasos:" -ForegroundColor Cyan
    Write-Host "1. Revisar cada producto en el sistema" -ForegroundColor White
    Write-Host "2. Asignar la tasa de IVA correcta según el tipo:" -ForegroundColor White
    Write-Host "   - IVA 16%: Productos generales (refrescos, dulces, etc.)" -ForegroundColor Gray
    Write-Host "   - IVA 0%: Alimentos básicos (pan, leche, frutas, etc.)" -ForegroundColor Gray
    Write-Host "   - Exento: Libros y revistas" -ForegroundColor Gray
    Write-Host "3. Verificar cálculos en las ventas" -ForegroundColor White
    Write-Host ""
    Write-Host "📚 Ver documentación completa en:" -ForegroundColor Yellow
    Write-Host "   CONFIGURACION_TASAS_IVA.md" -ForegroundColor Cyan
    
} catch {
    Write-Host ""
    Write-Host "❌ ERROR al ejecutar el script:" -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Red
    Write-Host ""
    Write-Host "Alternativa: Ejecute el script manualmente:" -ForegroundColor Yellow
    Write-Host "1. Abra SQL Server Management Studio" -ForegroundColor White
    Write-Host "2. Conéctese a su servidor" -ForegroundColor White
    Write-Host "3. Abra el archivo: $scriptPath" -ForegroundColor Cyan
    Write-Host "4. Ejecute el script (F5)" -ForegroundColor White
    exit 1
}

Write-Host ""
Read-Host "Presione Enter para continuar"
