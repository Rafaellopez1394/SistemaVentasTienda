param(
    [Parameter(Mandatory=$true)][string]$ServerInstance,
    [Parameter(Mandatory=$true)][string]$DatabaseName,
    [ValidateSet("Windows","SQL")][string]$AuthType = "Windows",
    [string]$SqlUser = "",
    [string]$SqlPassword = ""
)

$ErrorActionPreference = "Stop"

# Logging
$logDir = Join-Path $PSScriptRoot "logs"
if (-not (Test-Path $logDir)) { New-Item -ItemType Directory -Path $logDir | Out-Null }
$logFile = Join-Path $logDir ("Ensure-Database_" + (Get-Date -Format "yyyyMMdd_HHmmss") + ".log")
Start-Transcript -Path $logFile -Append | Out-Null

# Validar sqlcmd
$sqlcmdPath = Get-Command sqlcmd -ErrorAction SilentlyContinue
if (-not $sqlcmdPath) {
    Write-Host "ERROR: No se encontró 'sqlcmd'. Omite creación temprana de BD." -ForegroundColor Yellow
    Write-Host "Logs: $logFile" -ForegroundColor Green
    Stop-Transcript | Out-Null
    exit 0
}

Write-Host "Asegurando existencia de base de datos '$DatabaseName' en $ServerInstance"
$authArgs = @()
if ($AuthType -eq "SQL") { $authArgs += @("-U", $SqlUser, "-P", $SqlPassword) }
$args = @("-S", $ServerInstance, "-d", "master", "-b", "-I", "-Q", "IF DB_ID('" + $DatabaseName + "') IS NULL CREATE DATABASE [" + $DatabaseName + "]") + $authArgs
& sqlcmd @args | Out-String | Write-Host

Write-Host "Creación temprana de BD completada (si era necesario)." -ForegroundColor Green
Write-Host "Logs: $logFile" -ForegroundColor Green
Stop-Transcript | Out-Null
