# Script de publicación mejorado para IIS
Write-Host ""
Write-Host "========================================"  -ForegroundColor Cyan
Write-Host "   PUBLICACION SISTEMA DE VENTAS" -ForegroundColor Cyan
Write-Host "========================================"  -ForegroundColor Cyan
Write-Host ""

# 1. Compilación
Write-Host "[1/6] Compilando proyecto en modo Release..." -ForegroundColor Yellow
$msbuild = "C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe"
$projectPath = "C:\Users\Rafael Lopez\Documents\SistemaVentasTienda\VentasWeb.sln"

& $msbuild $projectPath /p:Configuration=Release /t:Rebuild /v:minimal /nologo

if ($LASTEXITCODE -ne 0) {
    Write-Host "ERROR: Falló la compilación" -ForegroundColor Red
    exit 1
}
Write-Host "Compilacion exitosa`n" -ForegroundColor Green

# 2. Preparar carpeta
Write-Host "[2/6] Preparando carpeta de publicacion..." -ForegroundColor Yellow
$publishPath = "C:\inetpub\wwwroot\SistemaVentas"
if (Test-Path $publishPath) {
    # Remove folders completely using robocopy empty folder trick
    $emptyPath = Join-Path $env:TEMP "EmptyFolder_$(Get-Random)"
    New-Item -ItemType Directory -Path $emptyPath -Force | Out-Null
    robocopy $emptyPath $publishPath /MIR /NFL /NDL /NJH /NJS /NP | Out-Null
    Remove-Item $emptyPath -Force
} else {
    New-Item -ItemType Directory -Path $publishPath -Force | Out-Null
}

# 3. Copiar archivos
Write-Host "[3/6] Copiando archivos..." -ForegroundColor Yellow
$sourcePath = "C:\Users\Rafael Lopez\Documents\SistemaVentasTienda\VentasWeb"

# robocopy returns codes: 0-7 are success, 8+ are errors
robocopy "$sourcePath\bin" "$publishPath\bin" /E /IS /IT /R:2 /W:1 /XO | Out-Null
if ($LASTEXITCODE -ge 8) { Write-Host "Warning: bin copy had issues" -ForegroundColor Yellow }

robocopy "$sourcePath\Content" "$publishPath\Content" /E /IS /IT /R:2 /W:1 /XO | Out-Null
robocopy "$sourcePath\Scripts" "$publishPath\Scripts" /E /IS /IT /R:2 /W:1 /XO | Out-Null  
robocopy "$sourcePath\Views" "$publishPath\Views" /E /IS /IT /R:2 /W:1 /XO | Out-Null

if (Test-Path "$sourcePath\fonts") {
    robocopy "$sourcePath\fonts" "$publishPath\fonts" /E /IS /IT /R:2 /W:1 /XO | Out-Null
}

Copy-Item "$sourcePath\Web.config" -Destination $publishPath -Force
Copy-Item "$sourcePath\Global.asax" -Destination $publishPath -Force
Write-Host "Archivos copiados`n" -ForegroundColor Green

# 4. Configurar IIS
Write-Host "[4/6] Configurando IIS..." -ForegroundColor Yellow
Import-Module WebAdministration

$poolName = "VentasWebPool"
$siteName = "SistemaVentas"

# Crear AppPool si no existe
if (!(Test-Path "IIS:\AppPools\$poolName")) {
    New-WebAppPool -Name $poolName
    Set-ItemProperty "IIS:\AppPools\$poolName" managedRuntimeVersion v4.0
    Set-ItemProperty "IIS:\AppPools\$poolName" managedPipelineMode Integrated
}

# Crear sitio si no existe
if (!(Test-Path "IIS:\Sites\$siteName")) {
    New-Website -Name $siteName -PhysicalPath $publishPath -ApplicationPool $poolName -Port 80 -Force
} else {
    Set-ItemProperty "IIS:\Sites\$siteName" -Name physicalPath -Value $publishPath
    Set-ItemProperty "IIS:\Sites\$siteName" -Name applicationPool -Value $poolName
}

Write-Host "IIS configurado`n" -ForegroundColor Green

# 5. Permisos
Write-Host "[5/6] Configurando permisos..." -ForegroundColor Yellow
$acl = Get-Acl $publishPath
$accessRule = New-Object System.Security.AccessControl.FileSystemAccessRule("IIS_IUSRS", "FullControl", "ContainerInherit,ObjectInherit", "None", "Allow")
$acl.SetAccessRule($accessRule)
$accessRule2 = New-Object System.Security.AccessControl.FileSystemAccessRule("IUSR", "ReadAndExecute", "ContainerInherit,ObjectInherit", "None", "Allow")
$acl.SetAccessRule($accessRule2)
Set-Acl $publishPath $acl
Write-Host "Permisos configurados`n" -ForegroundColor Green

# 6. Iniciar sitio
Write-Host "[6/6] Iniciando sitio..." -ForegroundColor Yellow
Stop-Website -Name "Default Web Site" -ErrorAction SilentlyContinue
Start-WebAppPool -Name $poolName -ErrorAction SilentlyContinue
Start-Website -Name $siteName -ErrorAction SilentlyContinue
Start-Sleep -Seconds 2

Write-Host ""
Write-Host "========================================" -ForegroundColor Green
Write-Host "SITIO PUBLICADO EXITOSAMENTE" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host "Sitio disponible en:" -ForegroundColor Cyan
Write-Host "  http://localhost" -ForegroundColor White
Write-Host "  http://127.0.0.1" -ForegroundColor White
Write-Host ""
