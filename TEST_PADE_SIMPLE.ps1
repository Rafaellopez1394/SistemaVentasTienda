Write-Host "========================================="-ForegroundColor Green
Write-Host "PRUEBA DE CONEXION CON PADE" -ForegroundColor Green
Write-Host "=========================================" -ForegroundColor Green
Write-Host ""

$usuario = "rafaellopez941113@gmail.com"
$password = "Rl19941113@"
$contrato = "f33e2e53-3bcd-49d5-a7c6-5f5cd4dd8c18"
$url = "https://pruebas.pade.mx/servicio/rest/timbrado40/timbrarCfdi"

Write-Host "Usuario: $usuario"
Write-Host "Contrato: $contrato"
Write-Host ""

$credenciales = "${usuario}:${password}"
$credencialesBase64 = [Convert]::ToBase64String([Text.Encoding]::UTF8.GetBytes($credenciales))

$xmlTest = '<?xml version="1.0"?><cfdi:Comprobante xmlns:cfdi="http://www.sat.gob.mx/cfd/4" Version="4.0" Serie="A" Folio="1" Fecha="2026-01-15T00:00:00" FormaPago="01" SubTotal="100.00" Moneda="MXN" Total="116.00" TipoDeComprobante="I" Exportacion="01" MetodoPago="PUE" LugarExpedicion="81048"><cfdi:Emisor Rfc="GAMA6111156JA" Nombre="ALMA ROSA GAXIOLA MONTOYA" RegimenFiscal="612"/><cfdi:Receptor Rfc="XAXX010101000" Nombre="PUBLICO EN GENERAL" DomicilioFiscalReceptor="81048" RegimenFiscalReceptor="616" UsoCFDI="S01"/><cfdi:Conceptos><cfdi:Concepto ClaveProdServ="01010101" Cantidad="1" ClaveUnidad="H87" Descripcion="Producto" ValorUnitario="100" Importe="100" ObjetoImp="02"/></cfdi:Conceptos><cfdi:Impuestos TotalImpuestosTrasladados="16"><cfdi:Traslados><cfdi:Traslado Base="100" Impuesto="002" TipoFactor="Tasa" TasaOCuota="0.16" Importe="16"/></cfdi:Traslados></cfdi:Impuestos></cfdi:Comprobante>'
$xmlBase64 = [Convert]::ToBase64String([Text.Encoding]::UTF8.GetBytes($xmlTest))

$body = @{
    xmlBase64 = $xmlBase64
    contrato = $contrato
    prueba = $true
    opciones = @("CERT_DEFAULT","CALCULAR_SELLO","ESTABLECER_NO_CERTIFICADO")
} | ConvertTo-Json

$headers = @{
    "Authorization" = "Basic $credencialesBase64"
    "Content-Type" = "application/json"
}

$urlConParams = "$url`?contrato=$contrato"

Write-Host "Enviando peticion..." -ForegroundColor Yellow

try {
    $response = Invoke-WebRequest -Uri $urlConParams -Method Post -Headers $headers -Body $body -ErrorAction Stop
    
    Write-Host ""
    Write-Host "RESPUESTA:" -ForegroundColor Green
    Write-Host "Status: $($response.StatusCode)"
    Write-Host "Content-Type: $($response.Headers['Content-Type'])"
    Write-Host ""
    Write-Host "Body:"
    Write-Host $response.Content
} catch {
    Write-Host ""
    Write-Host "ERROR:" -ForegroundColor Red
    Write-Host $_.Exception.Message
    
    if ($_.Exception.Response) {
        $reader = New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream())
        $responseBody = $reader.ReadToEnd()
        Write-Host "Response Body:"
        Write-Host $responseBody
    }
}

Write-Host ""
Write-Host "Fin de prueba"
