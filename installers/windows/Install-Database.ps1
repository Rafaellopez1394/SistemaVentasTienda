param(
    [string]$ServerInstance = ".\\SQLEXPRESS",
    [string]$DatabaseName = "DB_TIENDA",
    [ValidateSet("Windows","SQL")] [string]$AuthType = "Windows",
    [string]$SqlUser = "",
    [string]$SqlPassword = "",
    [string]$ProductosFile = "productos_actuales.txt"
)

$ErrorActionPreference = "Stop"

# Validar que sqlcmd esté disponible
$sqlcmdPath = Get-Command sqlcmd -ErrorAction SilentlyContinue
if (-not $sqlcmdPath) {
    Write-Host "ERROR: No se encontró 'sqlcmd'. Instale SQL Server (o la herramienta de línea de comandos) y reintente." -ForegroundColor Red
    throw "sqlcmd no disponible"
}

# Preparar carpeta de logs y comenzar transcripción
$logDir = Join-Path $PSScriptRoot "logs"
if (-not (Test-Path $logDir)) { New-Item -ItemType Directory -Path $logDir | Out-Null }
$logFile = Join-Path $logDir ("Install-Database_" + (Get-Date -Format "yyyyMMdd_HHmmss") + ".log")
Start-Transcript -Path $logFile -Append | Out-Null

function Invoke-Sql {
    param([string]$Sql, [string]$Db = "master")
    $authArgs = @()
    if ($AuthType -eq "SQL") { $authArgs += @("-U", $SqlUser, "-P", $SqlPassword) }
    $args = @("-S", $ServerInstance, "-d", $Db, "-b", "-I", "-Q", $Sql) + $authArgs
    & sqlcmd @args | Out-String | Write-Host
}

function Invoke-SqlFile {
    param([string]$FilePath, [string]$Db = $DatabaseName)
    Write-Host "Running SQL file: $FilePath"
    $authArgs = @()
    if ($AuthType -eq "SQL") { $authArgs += @("-U", $SqlUser, "-P", $SqlPassword) }
    $args = @("-S", $ServerInstance, "-d", $Db, "-b", "-I", "-i", $FilePath) + $authArgs
    & sqlcmd @args | Out-String | Write-Host
}

function Test-Table {
    param([string]$Table)
    $sql = "IF OBJECT_ID('$Table','U') IS NOT NULL SELECT 1 ELSE SELECT 0"
    $res = & sqlcmd -S $ServerInstance -d $DatabaseName -b -I -Q $sql
    return ($res -match "1")
}

function Invoke-SqlScalar {
    param([string]$Sql, [string]$Db = $DatabaseName)
    $authArgs = @()
    if ($AuthType -eq "SQL") { $authArgs += @("-U", $SqlUser, "-P", $SqlPassword) }
    $args = @("-S", $ServerInstance, "-d", $Db, "-b", "-I", "-Q", $Sql) + $authArgs
    $out = & sqlcmd @args
    $num = ($out | Where-Object { $_ -match "^\s*\d+\s*$" } | Select-Object -First 1)
    if ($null -eq $num) { return 0 }
    return [int]$num.Trim()
}

# 1) Ensure database exists
Write-Host "Ensuring database '$DatabaseName' exists on $ServerInstance"
Invoke-Sql "IF DB_ID('$DatabaseName') IS NULL CREATE DATABASE [$DatabaseName]"

# 2) Ejecutar todos los SQL en orden alfabético
# Resolver ubicación de scripts para escenarios de instalación
$possibleSqlRoots = @(
    (Join-Path $PSScriptRoot "SQL"),
    (Join-Path $PSScriptRoot "Utilidad\SQL Server"),
    (Join-Path (Split-Path -Parent (Split-Path -Parent $PSScriptRoot)) "Utilidad\SQL Server")
)
$sqlRoot = $null
foreach ($p in $possibleSqlRoots) { if (Test-Path $p) { $sqlRoot = $p; break } }
if (-not $sqlRoot) { throw "No se encontró la carpeta de scripts SQL en rutas conocidas." }

$files = Get-ChildItem $sqlRoot -Filter "*.sql" -File | Sort-Object Name
foreach ($f in $files) {
    Invoke-SqlFile -FilePath $f.FullName
}

# 3) Siembra de catálogos y productos (existencia 0)
#    Intentar leer productos_actuales.txt si existe
$possibleProductosPaths = @(
    (Join-Path $PSScriptRoot $ProductosFile),
    (Join-Path (Split-Path -Parent (Split-Path -Parent $PSScriptRoot)) $ProductosFile)
)
$productosPath = $possibleProductosPaths | Where-Object { Test-Path $_ } | Select-Object -First 1
if (-not $productosPath) { $productosPath = $null }
$defaultCategoriaId = 1

# Ensure a default category exists
if (Test-Table "CATEGORIA") {
    Invoke-Sql "IF NOT EXISTS(SELECT 1 FROM CATEGORIA WHERE Descripcion='General') INSERT INTO CATEGORIA(Descripcion,Activo) VALUES('General',1)" -Db $DatabaseName
    $defaultCategoriaId = (& sqlcmd -S $ServerInstance -d $DatabaseName -b -I -Q "SELECT TOP 1 CategoriaID FROM CATEGORIA WHERE Descripcion='General' ORDER BY CategoriaID" | Where-Object { $_ -match "^\s*\d+\s*$" } | Select-Object -First 1).Trim()
}

function Insert-ProductoProductoTable {
    param($codigo,$nombre,$descripcion)
    $codigoSafe = $codigo
    if ([string]::IsNullOrWhiteSpace($codigoSafe)) { $codigoSafe = $null }
    $sql = "IF NOT EXISTS(SELECT 1 FROM PRODUCTO WHERE Nombre=@n) INSERT INTO PRODUCTO(Codigo, ValorCodigo, Nombre, Descripcion, CategoriaID, Activo) VALUES(@c, NULL, @n, @d, @cat, 1)"
    $authArgs = @()
    if ($AuthType -eq "SQL") { $authArgs += @("-U", $SqlUser, "-P", $SqlPassword) }
    $args = @("-S", $ServerInstance, "-d", $DatabaseName, "-b", "-I", "-Q", $sql, "-v", "n=$nombre", "d=$descripcion", "c=$codigoSafe", "cat=$defaultCategoriaId") + $authArgs
    & sqlcmd @args | Out-String | Write-Host
}

function Insert-ProductoModern {
    param($codigo,$nombre,$descripcion)
    $codigoSafe = $codigo
    if ([string]::IsNullOrWhiteSpace($codigoSafe)) { $codigoSafe = $null }
    $sql = @"
IF OBJECT_ID('Productos','U') IS NOT NULL
BEGIN
    IF NOT EXISTS(SELECT 1 FROM Productos WHERE Nombre=@n)
    BEGIN
        INSERT INTO Productos(Nombre, Descripcion, CategoriaID, CodigoInterno, Estatus, FechaAlta)
        VALUES(@n, @d, @cat, @c, 1, GETDATE())
    END
END
"@
    $authArgs = @()
    if ($AuthType -eq "SQL") { $authArgs += @("-U", $SqlUser, "-P", $SqlPassword) }
    $args = @("-S", $ServerInstance, "-d", $DatabaseName, "-b", "-I", "-Q", $sql, "-v", "n=$nombre", "d=$descripcion", "c=$codigoSafe", "cat=$defaultCategoriaId") + $authArgs
    & sqlcmd @args | Out-String | Write-Host
}

if ($productosPath -and (Test-Path $productosPath)) {
    Write-Host "Seeding products from $productosPath"
    $lines = Get-Content $productosPath | Where-Object { $_.Trim().Length -gt 0 }
    # Skip header and dashed line
    $dataLines = $lines | Where-Object { $_ -notmatch "^-+" -and $_ -notmatch "^CodigoInterno" -and $_ -notmatch "\(\d+ rows affected\)" }
    foreach ($line in $dataLines) {
        $parts = ($line -replace "\s{2,}", "|" ).Split('|')
        if ($parts.Count -ge 3) {
            $codigo = $parts[0].Trim()
            $nombre = $parts[1].Trim()
            $descripcion = $parts[2].Trim()
            if ([string]::IsNullOrWhiteSpace($nombre)) { continue }
            if (Test-Table "PRODUCTO") { Insert-ProductoProductoTable -codigo $codigo -nombre $nombre -descripcion $descripcion }
            Insert-ProductoModern -codigo $codigo -nombre $nombre -descripcion $descripcion
        }
    }
}

# 4) Verificación y resumen
$catCount = Invoke-SqlScalar "IF OBJECT_ID('CATEGORIA','U') IS NOT NULL SELECT COUNT(*) FROM CATEGORIA ELSE SELECT 0"
$prodLegacyCount = Invoke-SqlScalar "IF OBJECT_ID('PRODUCTO','U') IS NOT NULL SELECT COUNT(*) FROM PRODUCTO ELSE SELECT 0"
$prodModernCount = Invoke-SqlScalar "IF OBJECT_ID('Productos','U') IS NOT NULL SELECT COUNT(*) FROM Productos ELSE SELECT 0"

Write-Host "Resumen instalación BD:" -ForegroundColor Cyan
Write-Host (" - Instancia: {0}" -f $ServerInstance)
Write-Host (" - Base de datos: {0}" -f $DatabaseName)
Write-Host (" - Categorías: {0}" -f $catCount)
Write-Host (" - Productos (PRODUCTO): {0}" -f $prodLegacyCount)
Write-Host (" - Productos (Productos): {0}" -f $prodModernCount)

$summaryPath = Join-Path $logDir "Install-Database-Summary.txt"
@(
    "Instancia: $ServerInstance",
    "Base de datos: $DatabaseName",
    "Categorias: $catCount",
    "Productos (PRODUCTO): $prodLegacyCount",
    "Productos (Productos): $prodModernCount"
) | Set-Content -Path $summaryPath

Write-Host "Database setup complete. Logs: $logFile, Summary: $summaryPath" -ForegroundColor Green

Stop-Transcript | Out-Null