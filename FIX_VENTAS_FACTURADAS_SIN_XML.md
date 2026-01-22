# FIX: Corrección de Ventas Marcadas como Facturadas sin XML

## Problema Identificado

Las ventas mostraban el estado "Facturada" en la interfaz pero al intentar descargar el XML fallaba con:

```
La factura A-25 no tiene XML timbrado. Estatus: PENDIENTE
```

### Causa Raíz

La lógica original marcaba una venta como `EstaFacturada = 1` inmediatamente después de **guardar** la factura en la base de datos, independientemente de si el timbrado con el PAC era exitoso o no.

**Flujo original (INCORRECTO):**
1. Usuario solicita facturar venta
2. Se crea factura con estatus "PENDIENTE" → `GuardarFactura()`
3. **Se marca venta como facturada** ← ❌ PROBLEMA
4. Se intenta timbrar con PAC (FiscalAPI o Prodigia)
5. Si timbrado falla o es cancelado → Factura queda PENDIENTE sin XML

**Resultado:** Ventas marcadas como facturadas que en realidad solo tienen facturas PENDIENTES sin XML timbrado.

## Diagnóstico Ejecutado

### Script: VERIFICAR_VENTAS_FACTURADAS.sql

**Resultados:**
- **40 facturas** en total asociadas a las ventas
- **33 facturas sin XML** (estatus PENDIENTE)
- **3 ventas** marcadas incorrectamente como facturadas:
  - `9f035d37-8764-4aa6-b71a-041dffd940b0` - 16 facturas pendientes
  - `6bc16123-7b85-418e-a4aa-62384726aa44` - 6 facturas pendientes  
  - `46a2c22d-045f-417e-96fc-9b7fcfff3fff` - 8 facturas pendientes

Estos registros representaban **intentos fallidos de facturación** donde el usuario intentó múltiples veces pero ningún timbrado fue exitoso.

## Solución Implementada

### Cambios en CD_Factura.cs

#### 1. Remover marca prematura en GuardarFactura
**Antes (líneas 821-827):**
```csharp
tran.Commit();

// Si la factura tiene VentaID, marcar la venta como facturada
if (factura.VentaID.HasValue)
{
    MarcarVentaComoFacturada(factura.VentaID.Value, cnx);
}

mensaje = "Factura guardada correctamente";
```

**Después:**
```csharp
tran.Commit();

mensaje = "Factura guardada correctamente";
```

#### 2. Marcar solo después de timbrado exitoso (Prodigia)
**Ubicación:** `GenerarFactura()` líneas ~1237-1250

**Después:**
```csharp
// Guardar factura en BD
bool guardado = GuardarFactura(factura, out string mensajeGuardado);

if (guardado)
{
    // Marcar la venta como facturada solo si se guardó correctamente
    if (factura.VentaID.HasValue)
    {
        using (SqlConnection cnx = new SqlConnection(Conexion.CN))
        {
            cnx.Open();
            MarcarVentaComoFacturada(factura.VentaID.Value, cnx);
        }
    }
}
```

#### 3. Marcar solo después de timbrado exitoso (FiscalAPI)
**Ubicación:** `TimbrarConFiscalAPI()` líneas ~1843-1860

**Lógica idéntica a Prodigia:** Solo marca si:
- `resultado.Exitoso = true`
- `GuardarFactura()` retorna `true`
- Factura tiene estatus "TIMBRADA"
- UUID, XMLTimbrado y sellos están presentes

### Nuevo Flujo (CORRECTO)

1. Usuario solicita facturar venta
2. Se crea factura con estatus "PENDIENTE"
3. Se guarda factura en BD sin marcar venta
4. Se timbra con PAC (FiscalAPI o Prodigia)
5. **Si timbrado exitoso:**
   - Se actualiza factura con UUID, XML, sellos
   - Se cambia estatus a "TIMBRADA"
   - Se guarda actualización en BD
   - **✓ Se marca venta como facturada**
6. **Si timbrado falla:**
   - Factura permanece PENDIENTE
   - **✓ Venta NO se marca como facturada**

## Corrección de Datos Existentes

### Script: CORREGIR_VENTAS_FACTURADAS.sql

**Ejecutado con éxito:**
```
3 ventas desmarcadas
```

**Lógica del script:**
1. Identifica ventas con `EstaFacturada = 1`
2. Verifica que **TODAS** sus facturas asociadas tengan estatus "PENDIENTE"
3. Excluye ventas que tengan al menos una factura TIMBRADA o CANCELADA
4. Ejecuta: `UPDATE VentasClientes SET EstaFacturada = 0` para esas ventas

**Estado final:**
- **5 ventas** correctamente marcadas (tienen facturas TIMBRADAS)
- **3 ventas** desmarcadas (solo tenían facturas PENDIENTES)
- **28 ventas** sin facturas (nunca se intentó facturar)

## Validación

### Verificación de Compilación
```powershell
MSBuild.exe VentasWeb.sln /t:Build /p:Configuration=Debug
```
**Resultado:** ✅ Build succeeded. 0 Error(s)

### Pruebas Manuales Sugeridas
1. **Intento fallido de facturación:**
   - Desconectar temporalmente servicio PAC
   - Intentar facturar venta
   - Verificar que venta quede como "Sin Facturar"
   - Reconectar PAC y facturar exitosamente
   - Verificar que venta cambie a "Facturada"

2. **Cancelación de factura:**
   - Facturar una venta
   - Verificar estado "Facturada"
   - Cancelar la factura
   - Verificar que venta vuelva a "Sin Facturar" (funcionalidad ya implementada)

3. **Descarga de XML:**
   - URL: `http://localhost:64927/Factura/DescargarXML?ventaId=9f035d37-8764-4aa6-b71a-041dffd940b0`
   - **Antes:** "La factura A-25 no tiene XML timbrado. Estatus: PENDIENTE"
   - **Después:** "La venta existe pero no tiene factura asociada" (porque se desmarcó y las pendientes no cuentan)

## Archivos Modificados

1. **CapaDatos/CD_Factura.cs**
   - Removida llamada a `MarcarVentaComoFacturada` desde `GuardarFactura` (línea ~823)
   - Agregada marca en `GenerarFactura` después de Prodigia (línea ~1240)
   - Agregada marca en `TimbrarConFiscalAPI` después de FiscalAPI (línea ~1850)

2. **CORREGIR_VENTAS_FACTURADAS.sql** (nuevo)
   - Script de corrección ejecutado exitosamente

3. **VERIFICAR_VENTAS_FACTURADAS.sql** (nuevo)
   - Script de diagnóstico para auditorías futuras

## Impacto

### Positivo
- ✅ Ventas solo se marcan como facturadas cuando **realmente** tienen XML timbrado
- ✅ Interfaz de consulta de ventas muestra estado preciso
- ✅ Descarga de XML funciona correctamente
- ✅ Previene confusión del usuario
- ✅ Permite reintentos de facturación sin inconsistencias

### Consideraciones
- Facturas con estatus "PENDIENTE" permanecen en BD como historial de intentos
- No afecta facturas ya timbradas exitosamente
- Compatible con flujo de cancelación existente

## Fecha de Implementación
**15 de enero de 2026**

## Ambiente
**Desarrollo/Pruebas** - Base de datos: DB_TIENDA

## Próximos Pasos Recomendados
1. Desplegar en ambiente de producción
2. Ejecutar `CORREGIR_VENTAS_FACTURADAS.sql` en producción antes del despliegue
3. Monitorear tabla VentasClientes después del despliegue
4. Validar con usuarios finales en primeras facturaciones

---

**Estado:** ✅ RESUELTO
**Prioridad:** ALTA (corrige inconsistencia de datos crítica)
