# Pre-flight Checklist para Despliegue en Producción
# Ejecutar antes de DESPLEGAR_PRODUCCION.ps1

$ErrorActionPreference = "Stop"

# ========================================
# COLORES Y FUNCIONES
# ========================================
function Print-Header {
    param([string]$message)
    Write-Host "`n========================================" -ForegroundColor Cyan
    Write-Host "   $message" -ForegroundColor Cyan
    Write-Host "========================================" -ForegroundColor Cyan
}

function Print-Check {
    param([string]$message, [bool]$pass)
    if ($pass) {
        Write-Host "✓ $message" -ForegroundColor Green
    } else {
        Write-Host "✗ $message" -ForegroundColor Red
    }
    return $pass
}

function Check-Administrator {
    Print-Header "1. VERIFICAR PERMISOS"
    
    $currentUser = [Security.Principal.WindowsIdentity]::GetCurrent()
    $principal = New-Object Security.Principal.WindowsPrincipal($currentUser)
    $isAdmin = $principal.IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)
    
    return Print-Check "Ejecutando como Administrador" $isAdmin
}

function Check-DotNetFramework {
    Print-Header "2. VERIFICAR .NET FRAMEWORK"
    
    $registry = Get-Item "HKLM:\SOFTWARE\Microsoft\NET Framework Setup\NDP\v4" -ErrorAction SilentlyContinue
    $hasNet46 = $null -ne $registry
    
    if ($hasNet46) {
        $version = $registry.GetValue("Version")
        Write-Host "✓ .NET Framework detectado: $version" -ForegroundColor Green
    } else {
        Print-Check ".NET Framework 4.6+" $false
    }
    
    return $hasNet46
}

function Check-MSBuild {
    Print-Header "3. VERIFICAR MSBUILD"
    
    $msbuild = "C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe"
    $exists = Test-Path $msbuild
    
    Print-Check "MSBuild encontrado" $exists
    if ($exists) {
        $version = (Get-Item $msbuild).VersionInfo.FileVersion
        Write-Host "  Versión: $version" -ForegroundColor Gray
    }
    
    return $exists
}

function Check-IIS {
    Print-Header "4. VERIFICAR IIS"
    
    # Verificar si IIS está instalado
    $iisFeature = Get-WindowsFeature -Name Web-Server -ErrorAction SilentlyContinue
    $iisInstalled = $null -ne $iisFeature -and $iisFeature.Installed
    
    Print-Check "IIS instalado" $iisInstalled
    
    if ($iisInstalled) {
        # Verificar WebAdministration
        $webAdminModule = Get-Module -ListAvailable WebAdministration -ErrorAction SilentlyContinue
        Print-Check "WebAdministration module" $null -ne $webAdminModule
        
        # Importar módulo
        if ($null -ne $webAdminModule) {
            Import-Module WebAdministration
        }
    }
    
    return $iisInstalled
}

function Check-SourceFiles {
    Print-Header "5. VERIFICAR ARCHIVOS FUENTE"
    
    $projectPath = "C:\Users\Rafael Lopez\Documents\SistemaVentasTienda"
    
    $files = @(
        @{Path = "$projectPath\VentasWeb.sln"; Name = "Solución VentasWeb.sln"},
        @{Path = "$projectPath\VentasWeb\VentasWeb.csproj"; Name = "Proyecto VentasWeb"},
        @{Path = "$projectPath\VentasWeb\Web.config"; Name = "Web.config"},
        @{Path = "$projectPath\VentasWeb\Global.asax"; Name = "Global.asax"}
    )
    
    $allExist = $true
    foreach ($file in $files) {
        $exists = Test-Path $file.Path
        Print-Check "$($file.Name)" $exists
        $allExist = $allExist -and $exists
    }
    
    return $allExist
}

function Check-Database {
    Print-Header "6. VERIFICAR BASE DE DATOS"
    
    try {
        $connectionString = "Server=localhost;Database=master;Integrated Security=true;"
        $connection = New-Object System.Data.SqlClient.SqlConnection($connectionString)
        $connection.Open()
        
        Print-Check "SQL Server accesible" $true
        
        # Verificar DB_TIENDA
        $cmd = $connection.CreateCommand()
        $cmd.CommandText = "SELECT DB_ID('DB_TIENDA') as dbid"
        $result = $cmd.ExecuteScalar()
        $dbExists = $null -ne $result
        
        Print-Check "Base de datos DB_TIENDA existe" $dbExists
        
        if ($dbExists) {
            # Verificar tablas críticas
            $cmd.CommandText = @"
SELECT 
    CASE WHEN OBJECT_ID('dbo.ConfiguracionPAC') IS NOT NULL THEN 1 ELSE 0 END as ConfigPAC,
    CASE WHEN OBJECT_ID('dbo.Ventas') IS NOT NULL THEN 1 ELSE 0 END as Ventas,
    CASE WHEN OBJECT_ID('dbo.Clientes') IS NOT NULL THEN 1 ELSE 0 END as Clientes
FROM DB_TIENDA.sys.databases
WHERE name = 'DB_TIENDA'
"@
            $cmd.CommandText = "USE DB_TIENDA; SELECT COUNT(*) FROM ConfiguracionPAC"
            $configCount = $cmd.ExecuteScalar()
            
            Print-Check "Tabla ConfiguracionPAC existe" ($configCount -gt 0)
        }
        
        $connection.Close()
        return $dbExists
    } catch {
        Print-Check "SQL Server accesible" $false
        Write-Host "  Error: $_" -ForegroundColor Yellow
        return $false
    }
}

function Check-FiscalAPIConfig {
    Print-Header "7. VERIFICAR CONFIGURACION FISCALAPI"
    
    try {
        $connectionString = "Server=localhost;Database=DB_TIENDA;Integrated Security=true;"
        $connection = New-Object System.Data.SqlClient.SqlConnection($connectionString)
        $connection.Open()
        
        $cmd = $connection.CreateCommand()
        $cmd.CommandText = @"
SELECT 
    ConfigPACID,
    EsProduccion,
    CASE WHEN EsProduccion = 1 THEN 'PRODUCCION' ELSE 'TEST' END as Ambiente,
    LEFT(ApiKey, 20) as ApiKey_Partial,
    Tenant
FROM ConfiguracionPAC
WHERE ConfigPACID = 1
"@
        
        $adapter = New-Object System.Data.SqlClient.SqlDataAdapter($cmd)
        $dataSet = New-Object System.Data.DataSet
        $adapter.Fill($dataSet) | Out-Null
        
        if ($dataSet.Tables[0].Rows.Count -gt 0) {
            $row = $dataSet.Tables[0].Rows[0]
            $esProduccion = $row["EsProduccion"]
            $apiKey = $row["ApiKey_Partial"]
            $tenant = $row["Tenant"]
            
            Print-Check "Configuración FiscalAPI existe" $true
            
            if ($esProduccion -eq 1) {
                Write-Host "  ✓ Ambiente: PRODUCCIÓN" -ForegroundColor Green
            } else {
                Write-Host "  ✗ Ambiente: TEST (Cambiar a PRODUCCIÓN)" -ForegroundColor Yellow
            }
            
            Write-Host "  - API Key: $($apiKey)..." -ForegroundColor Gray
            Write-Host "  - Tenant: $($tenant)" -ForegroundColor Gray
            
            return $esProduccion -eq 1
        } else {
            Print-Check "Configuración FiscalAPI" $false
            return $false
        }
        
        $connection.Close()
    } catch {
        Print-Check "Acceso a configuración FiscalAPI" $false
        Write-Host "  Error: $_" -ForegroundColor Yellow
        return $false
    }
}

function Check-WebConfig {
    Print-Header "8. VERIFICAR WEB.CONFIG"
    
    $webConfigPath = "C:\Users\Rafael Lopez\Documents\SistemaVentasTienda\VentasWeb\Web.config"
    
    if (-not (Test-Path $webConfigPath)) {
        Print-Check "Web.config existe" $false
        return $false
    }
    
    [xml]$config = Get-Content $webConfigPath
    
    # Verificar debug="false"
    $debugAttribute = $config.configuration.'system.web'.compilation.GetAttribute("debug")
    $isProduction = $debugAttribute -eq "false"
    
    if ($isProduction) {
        Print-Check "Debug deshabilitado (debug=false)" $true
    } else {
        Print-Check "Debug habilitado (CAMBIAR A FALSE)" $false
        Write-Host "  ⚠ En Web.config, línea de compilation debe tener: debug=`"false`"" -ForegroundColor Yellow
    }
    
    return $isProduction
}

function Check-Ports {
    Print-Header "9. VERIFICAR PUERTOS"
    
    $port = 80
    $protocol = "tcp"
    
    $listening = Get-NetTCPConnection -LocalPort $port -State Listen -ErrorAction SilentlyContinue
    
    if ($listening.Count -gt 0) {
        Write-Host "⚠ Puerto 80 ya está en uso" -ForegroundColor Yellow
        foreach ($conn in $listening) {
            Write-Host "  - PID: $($conn.OwningProcess), Estado: $($conn.State)" -ForegroundColor Gray
        }
        return $false
    } else {
        Print-Check "Puerto 80 disponible" $true
        return $true
    }
}

function Check-Disk {
    Print-Header "10. VERIFICAR ESPACIO EN DISCO"
    
    $driveC = Get-PSDrive -Name C
    $freeSpace = $driveC.Free / 1GB
    $totalSpace = $driveC.Used + $driveC.Free
    $percentFree = ($driveC.Free / $totalSpace) * 100
    
    $isOk = $freeSpace -gt 2
    
    Print-Check "Espacio en disco >= 2 GB" $isOk
    Write-Host "  - Espacio libre: $($freeSpace:F2) GB" -ForegroundColor Gray
    Write-Host "  - Uso: $($percentFree:F1)%" -ForegroundColor Gray
    
    return $isOk
}

function Print-Summary {
    param([hashtable]$results)
    
    Print-Header "RESUMEN DE VERIFICACIÓN"
    
    $passed = ($results.Values | Where-Object { $_ -eq $true }).Count
    $total = $results.Count
    
    Write-Host "`nResultados: $passed/$total validaciones pasadas" -ForegroundColor $(if ($passed -eq $total) { "Green" } else { "Yellow" })
    
    if ($passed -eq $total) {
        Write-Host "`n✓ LISTO PARA DESPLEGAR" -ForegroundColor Green
        Write-Host "`nEjecutar: .\DESPLEGAR_PRODUCCION.ps1" -ForegroundColor Cyan
        return $true
    } else {
        Write-Host "`n✗ FALTAN AJUSTES ANTES DE DESPLEGAR" -ForegroundColor Red
        Write-Host "`nRESUELVE LOS SIGUIENTES PROBLEMAS:" -ForegroundColor Yellow
        foreach ($check in $results.GetEnumerator()) {
            if (-not $check.Value) {
                Write-Host "  - $($check.Key)" -ForegroundColor White
            }
        }
        return $false
    }
}

# ========================================
# EJECUCIÓN PRINCIPAL
# ========================================
$results = @{
    "Administrador" = Check-Administrator
    ".NET Framework" = Check-DotNetFramework
    "MSBuild" = Check-MSBuild
    "IIS" = Check-IIS
    "Archivos Fuente" = Check-SourceFiles
    "Base de Datos" = Check-Database
    "FiscalAPI (Producción)" = Check-FiscalAPIConfig
    "Web.config (debug=false)" = Check-WebConfig
    "Puertos Disponibles" = Check-Ports
    "Espacio en Disco" = Check-Disk
}

$canDeploy = Print-Summary $results

Write-Host "`nPresiona Enter para cerrar..." -ForegroundColor Gray
Read-Host

exit $(if ($canDeploy) { 0 } else { 1 })
