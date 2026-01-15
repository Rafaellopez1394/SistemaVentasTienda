# Script para publicar Sistema de Ventas en IIS
$ErrorActionPreference = "Stop"
$projectPath = "C:\Users\Rafael Lopez\Documents\SistemaVentasTienda\VentasWeb\VentasWeb.csproj"
$solutionPath = "C:\Users\Rafael Lopez\Documents\SistemaVentasTienda\VentasWeb.sln"
$publishPath = "C:\inetpub\wwwroot\SistemaVentas"
$siteName = "SistemaVentas"
$appPoolName = "VentasWebPool"
$port = 80

Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "   PUBLICACION SISTEMA DE VENTAS" -ForegroundColor Cyan
Write-Host "========================================`n" -ForegroundColor Cyan

Write-Host "[1/6] Compilando proyecto en modo Release..." -ForegroundColor Yellow
& "C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe" $solutionPath /t:Clean,Build /p:Configuration=Release /p:DeployOnBuild=false /v:minimal /nologo

if ($LASTEXITCODE -ne 0) {
    Write-Host "Error en la compilacion!" -ForegroundColor Red
    exit 1
}
Write-Host "Compilacion exitosa`n" -ForegroundColor Green

Write-Host "[2/6] Preparando carpeta de publicacion..." -ForegroundColor Yellow
if (Test-Path $publishPath) {
    # Remove all contents but be more aggressive
    Get-ChildItem "$publishPath\*" -Recurse -Force -ErrorAction SilentlyContinue | Remove-Item -Force -Recurse -ErrorAction SilentlyContinue
    Start-Sleep -Milliseconds 500
} else {
    New-Item -ItemType Directory -Path $publishPath -Force | Out-Null
}

Write-Host "[3/6] Copiando archivos..." -ForegroundColor Yellow
$sourcePath = "C:\Users\Rafael Lopez\Documents\SistemaVentasTienda\VentasWeb"

# Copy directories and files separately to avoid permission issues
$null = New-Item -ItemType Directory -Path "$publishPath\bin" -Force -ErrorAction SilentlyContinue
Copy-Item "$sourcePath\bin\*" -Destination "$publishPath\bin" -Recurse -Force
# Copy directories and files separately to avoid permission issues
$null = New-Item -ItemType Directory -Path "$publishPath\bin" -Force -ErrorAction SilentlyContinue
Copy-Item "$sourcePath\bin\*" -Destination "$publishPath\bin" -Recurse -Force

$null = New-Item -ItemType Directory -Path "$publishPath\Content" -Force -ErrorAction SilentlyContinue
Copy-Item "$sourcePath\Content\*" -Destination "$publishPath\Content" -Recurse -Force

$null = New-Item -ItemType Directory -Path "$publishPath\Scripts" -Force -ErrorAction SilentlyContinue
Copy-Item "$sourcePath\Scripts\*" -Destination "$publishPath\Scripts" -Recurse -Force

$null = New-Item -ItemType Directory -Path "$publishPath\Views" -Force -ErrorAction SilentlyContinue
Copy-Item "$sourcePath\Views\*" -Destination "$publishPath\Views" -Recurse -Force

if (Test-Path "$sourcePath\fonts") {
    $null = New-Item -ItemType Directory -Path "$publishPath\fonts" -Force -ErrorAction SilentlyContinue
    Copy-Item "$sourcePath\fonts\*" -Destination "$publishPath\fonts" -Recurse -Force -ErrorAction SilentlyContinue
}

Copy-Item "$sourcePath\Web.config" -Destination $publishPath -Force
Copy-Item "$sourcePath\Global.asax" -Destination $publishPath -Force
Write-Host "Archivos copiados`n" -ForegroundColor Green

Write-Host "[4/6] Configurando IIS..." -ForegroundColor Yellow
Import-Module WebAdministration
if (Get-Website -Name $siteName -ErrorAction SilentlyContinue) {
    Stop-Website -Name $siteName -ErrorAction SilentlyContinue
    Start-Sleep -Seconds 2
    Remove-Website -Name $siteName
}
if (Test-Path "IIS:\AppPools\$appPoolName") {
    Remove-WebAppPool -Name $appPoolName
}
New-WebAppPool -Name $appPoolName
Set-ItemProperty "IIS:\AppPools\$appPoolName" -Name managedRuntimeVersion -Value "v4.0"
New-Website -Name $siteName -PhysicalPath $publishPath -ApplicationPool $appPoolName -Port $port -Force
Write-Host "IIS configurado`n" -ForegroundColor Green

Write-Host "[5/6] Configurando permisos..." -ForegroundColor Yellow
$acl = Get-Acl $publishPath
$identity = "IIS AppPool\$appPoolName"
$accessRule = New-Object System.Security.AccessControl.FileSystemAccessRule($identity, "Modify", "ContainerInherit,ObjectInherit", "None", "Allow")
$acl.AddAccessRule($accessRule)
Set-Acl $publishPath $acl
Write-Host "Permisos configurados`n" -ForegroundColor Green

Write-Host "[6/6] Iniciando sitio..." -ForegroundColor Yellow
Start-Website -Name $siteName
Start-Sleep -Seconds 2
Write-Host "Sitio iniciado`n" -ForegroundColor Green

Write-Host "`n========================================"  -ForegroundColor Cyan
Write-Host "      PUBLICACION COMPLETADA" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "`nSitio disponible en: http://localhost:$port" -ForegroundColor Yellow
Write-Host "Carpeta: $publishPath`n" -ForegroundColor Gray
