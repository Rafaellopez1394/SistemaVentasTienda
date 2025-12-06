Add-Type -Path 'c:\Users\Rafael Lopez\Documents\SistemaVentasTienda\CapaModelo\bin\Debug\CapaModelo.dll'
Add-Type -Path 'c:\Users\Rafael Lopez\Documents\SistemaVentasTienda\CapaDatos\bin\Debug\CapaDatos.dll'
$inst = [CapaDatos.CD_TipoCredito]::Instancia
$list = $inst.ObtenerTodos()
Write-Output "Count = $($list.Count)"
foreach($t in $list) {
    Write-Output "$($t.TipoCreditoID) | $($t.Codigo) | $($t.Nombre) | FechaCreacion=$($t.FechaCreacion) | UltimaAct=$($t.UltimaAct)"
}
