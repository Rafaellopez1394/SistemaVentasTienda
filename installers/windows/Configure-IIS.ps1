param(
    [string]$SiteName = "SistemaVentasTienda",
    [string]$AppPoolName = "SistemaVentasTiendaPool",
    [string]$PhysicalPath = "C:\\inetpub\\SistemaVentasTienda",
    [int]$Port = 8080
)

$ErrorActionPreference = "Stop"

# Preparar carpeta de logs y comenzar transcripción
$logDir = Join-Path $PSScriptRoot "logs"
if (-not (Test-Path $logDir)) { New-Item -ItemType Directory -Path $logDir | Out-Null }
$logFile = Join-Path $logDir ("Configure-IIS_" + (Get-Date -Format "yyyyMMdd_HHmmss") + ".log")
Start-Transcript -Path $logFile -Append | Out-Null

Write-Host "Enabling IIS features (requires admin)..."
# Enable IIS core features
& dism /Online /Enable-Feature /FeatureName:IIS-WebServer /All /NoRestart | Out-Null
& dism /Online /Enable-Feature /FeatureName:IIS-DefaultDocument /NoRestart | Out-Null
& dism /Online /Enable-Feature /FeatureName:IIS-StaticContent /NoRestart | Out-Null
& dism /Online /Enable-Feature /FeatureName:IIS-ASPNET45 /NoRestart | Out-Null
& dism /Online /Enable-Feature /FeatureName:IIS-ISAPIExtensions /NoRestart | Out-Null
& dism /Online /Enable-Feature /FeatureName:IIS-ISAPIFilter /NoRestart | Out-Null
& dism /Online /Enable-Feature /FeatureName:IIS-ManagementConsole /NoRestart | Out-Null

Import-Module WebAdministration

# Create folder
if (-not (Test-Path $PhysicalPath)) { New-Item -ItemType Directory -Path $PhysicalPath | Out-Null }

# Stop and remove existing site/app pool if present
if (Test-Path IIS:\AppPools\$AppPoolName) { Remove-WebAppPool -Name $AppPoolName }
New-WebAppPool -Name $AppPoolName
Set-ItemProperty IIS:\AppPools\$AppPoolName -Name managedRuntimeVersion -Value "v4.0"
Set-ItemProperty IIS:\AppPools\$AppPoolName -Name processModel.identityType -Value ApplicationPoolIdentity

if (Test-Path IIS:\Sites\$SiteName) { Remove-Website -Name $SiteName }
New-Website -Name $SiteName -Port $Port -PhysicalPath $PhysicalPath -ApplicationPool $AppPoolName

Write-Host "IIS site '$SiteName' created at $PhysicalPath on port $Port."

Write-Host "Logs: $logFile" -ForegroundColor Green
Stop-Transcript | Out-Null