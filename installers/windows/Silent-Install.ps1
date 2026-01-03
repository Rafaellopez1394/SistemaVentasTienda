param(
    [string]$InstallerPath = "installers\windows\SistemaVentasTienda_Installer.exe",
    [string]$SQLInstance = ".\SQLEXPRESS",
    [string]$DatabaseName = "DB_TIENDA",
    [ValidateSet("Windows","SQL")]
    [string]$AuthType = "Windows",
    [string]$SqlUser = "",
    [string]$SqlPassword = "",
    [string]$SqlUrl = "https://go.microsoft.com/fwlink/?linkid=866658",
    [int]$SitePort = 8080,
    [string]$SiteName = "SistemaVentasTienda",
    [string]$AppPoolName = "SistemaVentasTiendaPool",
    [string]$LoadInf = ""
)

# Helper: Build argument list for silent install
function New-InstallerArgs {
    param(
        [string]$SQLInstance,
        [string]$DatabaseName,
        [string]$AuthType,
        [string]$SqlUser,
        [string]$SqlPassword,
        [string]$SqlUrl,
        [int]$SitePort,
        [string]$SiteName,
        [string]$AppPoolName,
        [string]$LoadInf
    )
    $args = New-Object System.Collections.Generic.List[string]
    $args.Add('/VERYSILENT')
    if (![string]::IsNullOrWhiteSpace($LoadInf)) {
        $args.Add("/LOADINF=$LoadInf")
        return $args.ToArray()
    }
    $args.Add("/SQLInstance=$SQLInstance")
    $args.Add("/DatabaseName=$DatabaseName")
    $args.Add("/AuthType=$AuthType")
    if ($AuthType -eq 'SQL') {
        $args.Add("/SqlUser=$SqlUser")
        $args.Add("/SqlPassword=$SqlPassword")
    }
    if (![string]::IsNullOrWhiteSpace($SqlUrl)) {
        $args.Add("/SqlUrl=$SqlUrl")
    }
    $args.Add("/SitePort=$SitePort")
    $args.Add("/SiteName=$SiteName")
    $args.Add("/AppPoolName=$AppPoolName")
    return $args.ToArray()
}

# Validate installer path
if (-not (Test-Path -Path $InstallerPath)) {
    Write-Error "Installer not found at path: $InstallerPath"
    exit 1
}

# Build args
$argList = New-InstallerArgs -SQLInstance $SQLInstance -DatabaseName $DatabaseName -AuthType $AuthType -SqlUser $SqlUser -SqlPassword $SqlPassword -SqlUrl $SqlUrl -SitePort $SitePort -SiteName $SiteName -AppPoolName $AppPoolName -LoadInf $LoadInf

Write-Host "Starting installer (elevated) with silent parameters..." -ForegroundColor Cyan
Write-Host "Path: $InstallerPath" -ForegroundColor DarkGray
Write-Host "Args: $($argList -join ' ')" -ForegroundColor DarkGray

# Run elevated
$proc = Start-Process -FilePath $InstallerPath -ArgumentList $argList -Verb RunAs -PassThru -WindowStyle Hidden
$proc.WaitForExit()

if ($proc.ExitCode -eq 0) {
    Write-Host "Silent installation completed successfully." -ForegroundColor Green
    exit 0
} else {
    Write-Error "Installer exited with code $($proc.ExitCode). Check logs in the installed app directory (logs) and Windows Event Viewer."
    exit $proc.ExitCode
}