# Test compilation script
Set-Location "C:\Users\Rafael Lopez\Documents\SistemaVentasTienda"

$msbuild = "C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe"

Write-Host "Compilando VentasWeb..." -ForegroundColor Cyan

& $msbuild ".\VentasWeb\VentasWeb.csproj" /p:Configuration=Release /t:Rebuild /v:minimal /nologo

Write-Host "`nCompilación completada. Código de salida: $LASTEXITCODE" -ForegroundColor $(if($LASTEXITCODE -eq 0){"Green"}else{"Red"})
