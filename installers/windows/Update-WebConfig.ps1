param(
    [Parameter(Mandatory=$true)][string]$WebConfigPath,
    [Parameter(Mandatory=$true)][string]$ServerInstance,
    [Parameter(Mandatory=$true)][string]$DatabaseName,
    [ValidateSet("Windows","SQL")][string]$AuthType = "Windows",
    [string]$SqlUser = "",
    [string]$SqlPassword = ""
)

$ErrorActionPreference = "Stop"

# Preparar carpeta de logs y comenzar transcripción
$logDir = Join-Path $PSScriptRoot "logs"
if (-not (Test-Path $logDir)) { New-Item -ItemType Directory -Path $logDir | Out-Null }
$logFile = Join-Path $logDir ("Update-WebConfig_" + (Get-Date -Format "yyyyMMdd_HHmmss") + ".log")
Start-Transcript -Path $logFile -Append | Out-Null

if (-not (Test-Path $WebConfigPath)) {
    Write-Host "No se encontró Web.config en: $WebConfigPath" -ForegroundColor Yellow
    return
}

[xml]$xml = Get-Content -Path $WebConfigPath
$changed = $false

function Parse-ConnStr {
    param([string]$s)
    $map = @{}
    $parts = $s.Split(';') | Where-Object { $_.Trim().Length -gt 0 }
    foreach ($p in $parts) {
        $kv = $p.Split('=')
        if ($kv.Count -ge 2) {
            $key = $kv[0].Trim()
            $val = ($kv[1..($kv.Count-1)] -join '=').Trim()
            $map[$key] = $val
        }
    }
    return $map
}

function Build-ConnStr {
    param([hashtable]$map)
    $order = @('Data Source','Initial Catalog','Integrated Security','User ID','Password')
    $sb = New-Object System.Collections.Generic.List[string]
    foreach ($k in $order) { if ($map.ContainsKey($k)) { $sb.Add("$k=${map[$k]}") } }
    # Append remaining keys preserving them
    foreach ($k in $map.Keys | Where-Object { $order -notcontains $_ }) {
        $sb.Add("$k=${map[$k]}")
    }
    return ($sb -join ';') + ';'
}

$nodes = @()
if ($xml.configuration.connectionStrings -and $xml.configuration.connectionStrings.add) {
    $nodes = @($xml.configuration.connectionStrings.add)
}

if ($nodes.Count -eq 0) {
    Write-Host "No se encontraron entradas en <connectionStrings>." -ForegroundColor Yellow
} else {
    foreach ($n in $nodes) {
        if (-not $n.connectionString) { continue }
        $map = Parse-ConnStr -s $n.connectionString
        $map['Data Source'] = $ServerInstance
        $map['Initial Catalog'] = $DatabaseName
        if ($AuthType -eq 'Windows') {
            $map['Integrated Security'] = 'True'
            $map.Remove('User ID') | Out-Null
            $map.Remove('Password') | Out-Null
        } else {
            $map['Integrated Security'] = 'False'
            $map['User ID'] = $SqlUser
            $map['Password'] = $SqlPassword
        }
        $n.connectionString = Build-ConnStr -map $map
        $changed = $true
    }
}

if ($changed) {
    $xml.Save($WebConfigPath)
    Write-Host "Web.config actualizado con la instancia '$ServerInstance' y BD '$DatabaseName' (Auth: $AuthType)." -ForegroundColor Green
} else {
    Write-Host "No se realizaron cambios en Web.config." -ForegroundColor Yellow
}

Write-Host "Logs: $logFile" -ForegroundColor Green
Stop-Transcript | Out-Null
