param(
    [string]$InstanceName = "SQLEXPRESS",
    [string]$CustomUrl = ""
)

$ErrorActionPreference = "Stop"

# Logging
$logDir = Join-Path $PSScriptRoot "logs"
if (-not (Test-Path $logDir)) { New-Item -ItemType Directory -Path $logDir | Out-Null }
$logFile = Join-Path $logDir ("Install-SQLExpress_" + (Get-Date -Format "yyyyMMdd_HHmmss") + ".log")
Start-Transcript -Path $logFile -Append | Out-Null

function HasSqlCmd {
    return (Get-Command sqlcmd -ErrorAction SilentlyContinue) -ne $null
}

function HasSqlService {
    try {
        $svc = Get-Service -Name ("MSSQL$" + $InstanceName) -ErrorAction Stop
        return $true
    } catch {
        return $false
    }
}

if (HasSqlCmd -and HasSqlService) {
    Write-Host "SQL Server Express ya está instalado y 'sqlcmd' disponible."
    Write-Host "Logs: $logFile" -ForegroundColor Green
    Stop-Transcript | Out-Null
    exit 0
}

if ([string]::IsNullOrWhiteSpace($CustomUrl) -eq $false) {
    Write-Host "Intentando instalar SQL Server Express desde URL personalizada..."
    $installerPath = Join-Path $env:TEMP "SQLEXPRESS_Setup_Custom.exe"
    try {
        # Validación previa HEAD
        try {
            $head = Invoke-WebRequest -Uri $CustomUrl -Method Head -UseBasicParsing
            $len = 0
            if ($head.Headers.ContainsKey('Content-Length')) { [int]$len = $head.Headers['Content-Length'] }
            if ($head.StatusCode -lt 200 -or $head.StatusCode -ge 400 -or $len -lt 1000000) {
                Write-Host "La URL personalizada no es válida o el tamaño del archivo es muy pequeño." -ForegroundColor Yellow
            } else {
                Invoke-WebRequest -Uri $CustomUrl -OutFile $installerPath -UseBasicParsing
            }
        } catch {
            Write-Host "Fallo al consultar cabeceras de la URL personalizada." -ForegroundColor Yellow
        }
        if ((Test-Path $installerPath) -and ((Get-Item $installerPath).Length -gt 1024)) {
            $args = "/Q /IACCEPTSQLSERVERLICENSETERMS /ACTION=Install /FEATURES=SQLEngine /INSTANCENAME=$InstanceName"
            $p = Start-Process -FilePath $installerPath -ArgumentList $args -PassThru -Wait
            Write-Host ("Instalador personalizado terminó con código: {0}" -f $p.ExitCode)
        }
    } catch {
        Write-Host "Fallo al instalar desde URL personalizada." -ForegroundColor Yellow
    }
}

Write-Host "Intentando instalar SQL Server Express mediante winget..."
$winget = Get-Command winget -ErrorAction SilentlyContinue
if ($winget) {
    $ids = @('Microsoft.SQLServer.2022.Express','Microsoft.SQLServerExpress','Microsoft.SQL Server 2019 Express')
    $installed = $false
    foreach ($id in $ids) {
        try {
            & winget install --id $id -e --silent --accept-package-agreements --accept-source-agreements
            $installed = $true
            break
        } catch {
            Write-Host "Falló instalar con id: $id" -ForegroundColor Yellow
        }
    }
    if (-not $installed) { Write-Host "No se pudo instalar SQL Express via winget." -ForegroundColor Yellow }
} else {
    Write-Host "winget no está disponible; se omite instalación automática." -ForegroundColor Yellow
}

# Intento de descarga directa desde enlaces oficiales (fallback)
if (-not (HasSqlService) -or -not (HasSqlCmd)) {
    Write-Host "Intentando descarga directa de SQL Server Express..."
    $urls = @(
        # Microsoft fwlinks (pueden cambiar con el tiempo)
        "https://go.microsoft.com/fwlink/?linkid=866658",   # SQL Server 2019 Express (bootstrapper)
        "https://go.microsoft.com/fwlink/?linkid=2191013",  # SQL Server 2022 Express (posible enlace)
        "https://download.microsoft.com/download/1/5/1/151f7e7c-6c25-42d3-8f68-9e5ddb3b3f2a/SQLEXPR_x64_ENU.exe" # ejemplo directo (puede expirar)
    )
    $installerPath = Join-Path $env:TEMP "SQLEXPRESS_Setup.exe"
    $downloaded = $false
    foreach ($u in $urls) {
        try {
            Write-Host "Descargando: $u"
            Invoke-WebRequest -Uri $u -OutFile $installerPath -UseBasicParsing
            if ((Test-Path $installerPath) -and ((Get-Item $installerPath).Length -gt 1024)) { $downloaded = $true; break }
        } catch {
            Write-Host "Fallo al descargar: $u" -ForegroundColor Yellow
        }
    }
    if ($downloaded) {
        Write-Host "Instalando SQL Express en modo silencioso..."
        $args = "/Q /IACCEPTSQLSERVERLICENSETERMS /ACTION=Install /FEATURES=SQLEngine /INSTANCENAME=$InstanceName"
        try {
            $p = Start-Process -FilePath $installerPath -ArgumentList $args -PassThru -Wait
            Write-Host ("Instalador terminó con código: {0}" -f $p.ExitCode)
        } catch {
            Write-Host "Fallo al ejecutar el instalador de SQL Express." -ForegroundColor Yellow
        }
    } else {
        Write-Host "No fue posible descargar el instalador de SQL Express." -ForegroundColor Yellow
    }
}

# Verificar e iniciar servicio
if (HasSqlService) {
    $svc = Get-Service -Name ("MSSQL$" + $InstanceName)
    if ($svc.Status -ne 'Running') { Start-Service -Name $svc.Name }
}

# Confirmar sqlcmd
if (-not (HasSqlCmd)) {
    Write-Host "'sqlcmd' no disponible tras la instalación. Asegure instalar utilidades de línea de comandos de SQL Server." -ForegroundColor Yellow
}

Write-Host "Instalación de SQL Server Express finalizada (si fue posible)." -ForegroundColor Green
Write-Host "Logs: $logFile" -ForegroundColor Green
Stop-Transcript | Out-Null
