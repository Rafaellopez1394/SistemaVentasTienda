# Manual de Pruebas - Sistema POS con IVA Desglose

## Fecha: 2025-12-03

### Status Actual
✅ Compilación exitosa  
✅ Bloqueador de IVA resuelta - Ahora se auto-pobla desde producto  
✅ Database schema creado  
✅ Poliza generation implementada

---

## Casos de Prueba

### Test 1: Venta Simple (Single Tax Rate)

**Objetivo:** Verificar que una venta con un solo rango de IVA genera poliza correctamente

**Pasos:**
1. Crear un cliente de prueba (si no existe)
2. Crear una venta con 3 productos diferentes todos al 16% IVA
3. Calcular manualmente:
   - Total base: suma de (precio × cantidad) sin IVA
   - Total IVA: total base × 16%
   - Total: base + IVA

**Validación en SQL:**
```sql
-- Verificar que se creó la poliza
SELECT TOP 1 * FROM PolizasContables 
WHERE Concepto LIKE 'Venta%' 
ORDER BY FechaPoliza DESC;

-- Verificar detalles de poliza
SELECT * FROM PolizasDetalle 
WHERE PolizaID = (SELECT TOP 1 PolizaID FROM PolizasContables 
                  WHERE Concepto LIKE 'Venta%' 
                  ORDER BY FechaPoliza DESC)
ORDER BY Debe DESC, Haber DESC;

-- Validar balance: Debe == Haber
SELECT 
    SUM(Debe) AS TotalDebe,
    SUM(Haber) AS TotalHaber,
    CASE WHEN SUM(Debe) = SUM(Haber) THEN 'BALANCEADO ✅' ELSE 'DESBALANCEADO ❌' END AS Status
FROM PolizasDetalle 
WHERE PolizaID = (SELECT TOP 1 PolizaID FROM PolizasContables 
                  WHERE Concepto LIKE 'Venta%' 
                  ORDER BY FechaPoliza DESC);
```

**Resultado Esperado:**
- Poliza con 5 líneas:
  1. Débito Clientes (total con IVA)
  2. Débito COGS
  3. Crédito Ventas (base)
  4. Crédito IVA 16%
  5. Crédito Inventario

---

### Test 2: Venta Multi-Tasa (Mixed Tax Rates) 🔑

**Objetivo:** Verificar desglose de IVA por múltiples tasas (0%, 8%, 16%)

**Pasos:**
1. Crear/modificar productos:
   - Producto A: 100 pesos @ 16% IVA
   - Producto B: 50 pesos @ 8% IVA
   - Producto C: 30 pesos @ 0% IVA (Exento)

2. Crear venta con 1 unidad de cada producto

3. Cálculos esperados:
   ```
   Base A: 100 × 1 = 100, IVA: 100 × 16% = 16
   Base B: 50 × 1 = 50, IVA: 50 × 8% = 4
   Base C: 30 × 1 = 30, IVA: 0
   
   Total Base: 180
   Total IVA: 20
   Total Venta: 200
   ```

**Validación en SQL:**
```sql
-- Obtener poliza reciente
DECLARE @PolizaID INT = (SELECT TOP 1 PolizaID FROM PolizasContables 
                         WHERE Concepto LIKE 'Venta%' 
                         ORDER BY FechaPoliza DESC);

-- Ver estructura de poliza
SELECT 
    pd.PolizaDetalleID,
    c.SubTipo AS CuentaTipo,
    c.NombreCuenta,
    pd.Debe,
    pd.Haber,
    pd.Concepto
FROM PolizasDetalle pd
INNER JOIN CatalogoContable c ON pd.CuentaID = c.CuentaID
WHERE pd.PolizaID = @PolizaID
ORDER BY pd.Debe DESC, pd.Haber DESC;

-- Contar líneas (debe ser 7: 1 débito cliente, 1 COGS, 1 ventas, 3 IVAs, 1 inventario)
SELECT COUNT(*) AS NumeroLineas FROM PolizasDetalle WHERE PolizaID = @PolizaID;
```

**Resultado Esperado:**
- Poliza con 7 líneas (no 5):
  1. Débito Clientes: 200
  2. Débito COGS: (costo total)
  3. Crédito Ventas: 180 (base)
  4. Crédito IVA 16%: 16
  5. Crédito IVA 8%: 4
  6. Crédito IVA 0%: 0 (puede no aparecer si es 0)
  7. Crédito Inventario: (costo)
  
- **Balance:** Debe == 200 + COGS, Haber == 180 + 20 + COGS ✅

---

### Test 3: Auto-población de IVA desde Producto

**Objetivo:** Verificar que se auto-popula TasaIVAPorcentaje y Exento

**Pasos:**

1. En el navegador, abrir DevTools (F12)
2. Ir a la página de ventas
3. Hacer POST a `/Venta/RegistrarVenta` con payload:
```json
{
  "ClienteID": "00000000-0000-0000-0000-000000000001",
  "Total": 200,
  "Estatus": "CREDITO",
  "Detalle": [
    {
      "ProductoID": 1,
      "Cantidad": 2,
      "PrecioVenta": 50,
      "PrecioCompra": 30,
      "LoteID": 1,
      "TasaIVAPorcentaje": 0,
      "Exento": false
    }
  ]
}
```

4. Inspeccionar respuesta - debe incluir mensaje de éxito

**Validación JavaScript en DevTools:**
```javascript
// Verificar que los datos se auto-poblaron correctamente
fetch('/Poliza/Obtener?top=1')
  .then(r => r.json())
  .then(d => {
    console.log('Última póliza:', d.data[0]);
    console.log('Líneas de póliza:', d.data[0].TotalDebe, 'Debe -', d.data[0].TotalHaber, 'Haber');
  });
```

---

### Test 4: Validación de Balance (Debe == Haber)

**Objetivo:** Verificar que TODAS las pólizas están balanceadas

**Query SQL (ejecutar en SSMS):**
```sql
-- Encontrar pólizas desbalanceadas (si existen)
SELECT 
    p.PolizaID,
    p.FechaPoliza,
    p.Concepto,
    SUM(pd.Debe) AS SumaDebe,
    SUM(pd.Haber) AS SumaHaber,
    ABS(SUM(pd.Debe) - SUM(pd.Haber)) AS Diferencia
FROM PolizasContables p
LEFT JOIN PolizasDetalle pd ON p.PolizaID = pd.PolizaID
GROUP BY p.PolizaID, p.FechaPoliza, p.Concepto
HAVING ABS(SUM(pd.Debe) - SUM(pd.Haber)) > 0.01
ORDER BY p.FechaPoliza DESC;

-- Si esta query no retorna resultados = TODO ESTÁ BIEN ✅
```

---

### Test 5: Validar Mapeo de Cuentas desde Base de Datos

**Objetivo:** Verificar que las cuentas se resuelven correctamente desde CatalogoContable

**Pasos:**

1. **Verificar que existen todas las cuentas:**
```sql
-- Contar cuentas
SELECT COUNT(*) AS TotalCuentas FROM CatalogoContable;
-- Debe mostrar: 15

-- Ver estructura
SELECT SubTipo, COUNT(*) FROM CatalogoContable 
GROUP BY SubTipo 
ORDER BY SubTipo;

-- Resultado esperado:
-- CAJA, CLIENTE, COSTO_VENTAS, INVENTARIO, IVA_COBRADO_0, 
-- IVA_COBRADO_16, IVA_COBRADO_8, IVA_PAGADO_0, IVA_PAGADO_16, 
-- IVA_PAGADO_8, PROVEEDOR, VENTAS
```

2. **Verificar que MapeoIVA está configurado:**
```sql
SELECT * FROM MapeoContableIVA WHERE Activo = 1;

-- Debe mostrar 4 filas:
-- TasaIVA | Exento | CuentaDeudora | CuentaAcreedora | Descripcion
--   0.00  |   0    |     2050      |      2050       | IVA 0%
--   8.00  |   0    |     2051      |      2051       | IVA 8%
--  16.00  |   0    |     2052      |      2052       | IVA 16%
--   0.00  |   1    |     2053      |      2053       | Exento
```

3. **Verificar uso en póliza:**
```sql
SELECT DISTINCT c.SubTipo, c.CuentaID, c.NombreCuenta, COUNT(*) as UsosEnPolizas
FROM PolizasDetalle pd
INNER JOIN CatalogoContable c ON pd.CuentaID = c.CuentaID
GROUP BY c.SubTipo, c.CuentaID, c.NombreCuenta
ORDER BY c.SubTipo;
```

---

## Checklist de Validación

### ✅ Bloqueador Resuelto - Auto-población IVA

- [x] `CD_Producto.ObtenerDatosFiscales()` creado
- [x] `VentaController.RegistrarVenta()` ahora popula `TasaIVAPorcentaje` y `Exento`
- [x] Los datos se cargan desde la BD antes de guardar
- [x] Si el producto no existe, usa defaults (16%, no exento)

### 📊 Database Layer

- [x] `CatalogoContable` con 15 cuentas
- [x] `MapeoContableIVA` con 4 tasas
- [x] `CD_CatalogoContable` con métodos de lookup
- [x] `CD_MapeoIVA` con métodos de lookup

### 💾 Data Access Layer

- [x] `CD_Venta.RegistrarVentaCredito()` - Con desglose IVA y transacción atómica
- [x] `CD_Compra.RegistrarCompraConLotes()` - Con desglose IVA
- [x] `CD_Poliza.CrearPoliza()` - Transactional overload
- [x] `CD_Producto.ObtenerDatosFiscales()` - NEW para auto-población

### 🎮 Controller Layer

- [x] `VentaController.RegistrarVenta()` - Popula IVA antes de guardar
- [x] `PolizaController.Crear()` - Manual poliza entry
- [x] `PolizaController.Obtener()` - Listing con totales

### 📋 Views Layer

- [x] `Poliza/Index.cshtml` - Form manual entry
- [x] `Poliza/List.cshtml` - List view con AJAX

### ✅ Compilation

- [x] 0 Errors
- [x] 24 Warnings (pre-existing unused variables)
- [x] All DLLs generated successfully

---

## Próximos Pasos Recomendados

### Inmediato (Hoy)
1. **Ejecutar Test 2 (Multi-Tasa)** - El más crítico
2. **Validar balance de pólizas** con Test 4
3. **Verificar queries SQL** de Test 5

### Corto Plazo (Esta semana)
1. **Gestión de Clientes:** CRUD completo
2. **Sistema de Créditos:** Implantación de 3 tipos
3. **UI de Ventas (POS):** Interfaz de entrada rápida

### Mediano Plazo
1. **Compras y Proveedores**
2. **Pagos y Cobranza**
3. **Reportes básicos**

---

## Troubleshooting

### Si la poliza no se genera:
1. Verificar que `CatalogoContable` tiene todas las cuentas necesarias
2. Verificar que `MapeoContableIVA` está poblada
3. Revisar el log de la aplicación en `~/bin/` o Event Viewer

### Si la TasaIVAPorcentaje es 0:
1. Verificar que el producto existe en BD
2. Verificar que tiene una `TasaIVAID` válida en `CatTasaIVA`
3. Ejecutar: `SELECT TasaIVAPorcentaje FROM CatalogoContable WHERE ProductoID = ?`

### Si Debe ≠ Haber:
1. Revisar la lógica de cálculo en `CD_Venta.RegistrarVentaCredito()`
2. Verificar que todas las líneas se crearon correctamente
3. Ejecutar SQL de validación de balance

### Si no se auto-popula IVA:
1. Verificar que `CD_Producto.ObtenerDatosFiscales()` retorna datos
2. Revisar la tabla `CatTasaIVA` - debe tener las tasas: 0, 8, 16
3. Verificar que el detalle de venta es `not null` antes de guardar

---

## Comandos Útiles SQL

```sql
-- Última venta con poliza
SELECT TOP 1 v.VentaID, v.Total, p.PolizaID, p.Concepto
FROM Ventas v
LEFT JOIN PolizasContables p ON v.VentaID = p.ReferenciaTipo
ORDER BY v.FechaVenta DESC;

-- Desglose IVA en última poliza
SELECT DISTINCT 
    c.SubTipo,
    COUNT(*) AS NumeroLineas,
    SUM(CASE WHEN pd.Debe > 0 THEN pd.Debe ELSE 0 END) AS Debito,
    SUM(CASE WHEN pd.Haber > 0 THEN pd.Haber ELSE 0 END) AS Credito
FROM PolizasDetalle pd
INNER JOIN CatalogoContable c ON pd.CuentaID = c.CuentaID
WHERE pd.PolizaID = (SELECT TOP 1 PolizaID FROM PolizasContables ORDER BY FechaPoliza DESC)
GROUP BY c.SubTipo;

-- Listar últimas 10 pólizas
SELECT TOP 10 
    PolizaID,
    FechaPoliza,
    TipoPoliza,
    Concepto,
    (SELECT SUM(Debe) FROM PolizasDetalle WHERE PolizaID = PolizasContables.PolizaID) AS TotalDebe,
    (SELECT SUM(Haber) FROM PolizasDetalle WHERE PolizaID = PolizasContables.PolizaID) AS TotalHaber
FROM PolizasContables
ORDER BY FechaPoliza DESC;
```

---

**Nota:** Todos los tests asumen que la base de datos `DB_TIENDA` está actualizada con las tablas creadas hoy (MapeoContableIVA y CatalogoContable).

**Generado:** 2025-12-03  
**Estado:** Ready for Testing  
**Próxima Revisión:** Después de ejecutar los tests
