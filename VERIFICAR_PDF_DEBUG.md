# Verificación de PDF - Pasos de Diagnóstico

## Situación
El PDF se descarga pero está dañado ("Invalid PDF structure")

## Diagnóstico Necesario

### 1. Verificar Logs de Debug

En **Visual Studio**:
1. Ve a: **View → Output** (o presiona `Ctrl+Alt+O`)
2. En el dropdown de "Show output from:", selecciona **"Debug"**
3. Recarga la página y descarga el PDF de nuevo
4. Copia TODOS los logs que aparezcan

**Busca específicamente:**
```
=== DescargarPDF INICIO ===
FacturaID: ...
Factura cargada: F-2
UUID: 013416dd-b424-454d-89be-91a62f9a1da7
FiscalAPIInvoiceId: 8eb2120d-3361-4b61-acf5-191f42ee9a74  ← ¿Es NULL o tiene valor?
✅ Usando FiscalAPI PDF (oficial)  ← ¿Cuál aparece?
⚠️ Usando generador local (iTextSharp)
=== DESCARGA PDF FISCALAPI ===
Response Status: OK  ← ¿Qué status code?
✅ PDF descargado: XXXXX bytes  ← ¿Cuántos bytes?
```

### 2. Verificar InvoiceId en Base de Datos

Ejecuta en **SQL Server Management Studio**:
```sql
SELECT 
    FacturaID,
    Serie,
    Folio,
    UUID,
    FiscalAPIInvoiceId,
    LEN(FiscalAPIInvoiceId) as LongitudInvoiceId
FROM Facturas 
WHERE Serie = 'F' AND Folio = '2'
```

**Debe mostrar:**
- FiscalAPIInvoiceId: `8eb2120d-3361-4b61-acf5-191f42ee9a74`
- LongitudInvoiceId: `36`

### 3. Prueba Manual de API (PowerShell)

Si el InvoiceId está OK, prueba directamente la API:

```powershell
# Test directo a FiscalAPI
$headers = @{
    "X-API-KEY" = "sk_test_47126aed_6c71_4060_b05b_932c4423dd00"
    "X-TENANT-KEY" = "e0a0d1de-d225-46de-b95f-55d04f2787ff"
    "X-TIME-ZONE" = "America/Mexico_City"
    "Content-Type" = "application/json"
}

$body = @{
    invoiceId = "8eb2120d-3361-4b61-acf5-191f42ee9a74"
} | ConvertTo-Json

Write-Host "Llamando a FiscalAPI..."
try {
    $response = Invoke-RestMethod -Uri "https://test.fiscalapi.com/api/v4/invoices/pdf" `
                                  -Method POST `
                                  -Headers $headers `
                                  -Body $body `
                                  -OutFile "C:\Users\Rafael Lopez\Desktop\TEST_FISCAL_PDF.pdf"
    
    Write-Host "✅ PDF descargado en Desktop: TEST_FISCAL_PDF.pdf"
    
    $archivo = Get-Item "C:\Users\Rafael Lopez\Desktop\TEST_FISCAL_PDF.pdf"
    Write-Host "Tamaño: $($archivo.Length) bytes"
    
} catch {
    Write-Host "❌ Error: $_"
    Write-Host $_.Exception.Response
}
```

### 4. Posibles Causas del Error

| Síntoma | Causa | Solución |
|---------|-------|----------|
| InvoiceId es NULL en logs | ObtenerPorId no lee la columna | Verificar que DLL se haya actualizado |
| InvoiceId existe pero API error 404 | InvoiceId incorrecto en BD | Verificar query SQL |
| API devuelve JSON error | Credenciales incorrectas | Verificar X-API-KEY, X-TENANT-KEY |
| PDF muy pequeño (<1000 bytes) | Es un error JSON, no PDF | Revisar logs de errorBody |

## Próximos Pasos

1. **Ejecuta el paso 1** (Debug Output) primero
2. Copia y pega los logs completos
3. Si InvoiceId es NULL, el DLL no se actualizó correctamente
4. Si InvoiceId existe, ejecuta **paso 3** (prueba manual PowerShell)
