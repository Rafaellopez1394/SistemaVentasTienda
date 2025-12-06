# Guía de Inicio Rápido - Testing

**Objetivo:** Verificar que el sistema de polizas con IVA desglose funciona correctamente

**Tiempo estimado:** 15-20 minutos

---

## Paso 1: Verificar Compilación ✅

```powershell
cd "C:\Users\Rafael Lopez\Documents\SistemaVentasTienda"

# Build
& "C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe" `
    VentasWeb.sln /t:Build /p:Configuration=Debug

# Resultado esperado: "Compilación correcta. 0 Errores"
```

---

## Paso 2: Verificar Base de Datos 🗄️

Abrir **SQL Server Management Studio** y ejecutar:

```sql
-- 1. Verificar tablas existen
SELECT COUNT(*) AS [Cuentas] FROM CatalogoContable;
-- Debe mostrar: 15

SELECT COUNT(*) AS [Mapeos IVA] FROM MapeoContableIVA;
-- Debe mostrar: 4

-- 2. Ver estructura de cuentas
SELECT SubTipo, NombreCuenta, CodigoCuenta 
FROM CatalogoContable 
ORDER BY CodigoCuenta;

-- 3. Ver mapeo de IVA
SELECT TasaIVA, Exento, CuentaDeudora, CuentaAcreedora, Descripcion
FROM MapeoContableIVA
ORDER BY TasaIVA;
```

**✅ Si todas las queries retornan datos → BD OK**

---

## Paso 3: Verificar Datos de Producto 📦

```sql
-- Verificar que los productos tienen TasaIVAPorcentaje configurada
SELECT TOP 5
    ProductoID,
    Nombre,
    iva.Porcentaje AS TasaIVAPorcentaje,
    p.Exento
FROM Productos p
INNER JOIN CatTasaIVA iva ON p.TasaIVAID = iva.TasaIVAID
WHERE p.Estatus = 1;

-- Si retorna datos → OK
-- Si no retorna: Necesitas insertar productos con IVA antes de testear
```

---

## Paso 4: Test Case 1 - Venta Simple (16% IVA)

### Escenario
Vender 1 unidad de un producto a 100 pesos con 16% IVA

### Paso 1: Obtener datos
```sql
-- Obtener un cliente de prueba
SELECT TOP 1 ClienteID, RazonSocial FROM Clientes;

-- Obtener un producto con 16% IVA
SELECT TOP 1 ProductoID, Nombre FROM Productos 
WHERE (SELECT Porcentaje FROM CatTasaIVA WHERE TasaIVAID = Productos.TasaIVAID) = 16.00;

-- Obtener un lote disponible
SELECT TOP 1 LoteID, ProductoID FROM Lotes WHERE CantidadDisponible > 0;
```

### Paso 2: Crear venta (opción A - Manual desde UI)
1. Ir a `http://localhost:PUERTO/Venta/Index`
2. Buscar cliente
3. Agregar producto (1 unidad, precio 100)
4. Registrar venta

### Paso 2: Crear venta (opción B - Postman/JavaScript)
```javascript
const venta = {
    clienteID: "CLIENT-GUID",
    total: 116,
    estatus: "CREDITO",
    detalle: [
        {
            productoID: 1,
            loteID: 1,
            cantidad: 1,
            precioVenta: 100,
            precioCompra: 60,
            tasaIVAPorcentaje: 16,
            exento: false
        }
    ]
};

fetch('/Venta/RegistrarVenta', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(venta)
})
.then(r => r.json())
.then(d => console.log('Resultado:', d));
```

### Paso 3: Validar en BD
```sql
-- Obtener última póliza
DECLARE @PolizaID INT = (SELECT TOP 1 PolizaID 
                         FROM PolizasContables 
                         ORDER BY FechaPoliza DESC);

-- Ver estructura
SELECT 
    c.SubTipo,
    c.NombreCuenta,
    pd.Debe,
    pd.Haber,
    pd.Concepto
FROM PolizasDetalle pd
INNER JOIN CatalogoContable c ON pd.CuentaID = c.CuentaID
WHERE pd.PolizaID = @PolizaID
ORDER BY c.SubTipo;

-- Validar balance
SELECT 
    SUM(Debe) AS TotalDebe,
    SUM(Haber) AS TotalHaber
FROM PolizasDetalle
WHERE PolizaID = @PolizaID;
```

**✅ Debe == Haber → TEST PASSED**

---

## Paso 5: Test Case 2 - Venta Multi-Tasa (CRÍTICO)

### Escenario
Venta con 3 productos diferentes:
- Producto A: 100 pesos @ 16% IVA
- Producto B: 50 pesos @ 8% IVA
- Producto C: 30 pesos @ 0% (exento)

### Paso 1: Preparar datos
```sql
-- Modificar 3 productos para tener diferentes IVAs
UPDATE Productos SET TasaIVAID = 1 WHERE ProductoID = 1;  -- 16%
UPDATE Productos SET TasaIVAID = 2 WHERE ProductoID = 2;  -- 8%
UPDATE Productos SET Exento = 1 WHERE ProductoID = 3;     -- Exento
```

### Paso 2: Crear venta
```javascript
const venta = {
    clienteID: "CLIENT-GUID",
    total: 200,  // 100 + 16 + 50 + 4 + 30 + 0
    estatus: "CREDITO",
    detalle: [
        {
            productoID: 1,
            loteID: 1,
            cantidad: 1,
            precioVenta: 100,
            precioCompra: 60,
            tasaIVAPorcentaje: 16,
            exento: false
        },
        {
            productoID: 2,
            loteID: 2,
            cantidad: 1,
            precioVenta: 50,
            precioCompra: 30,
            tasaIVAPorcentaje: 8,
            exento: false
        },
        {
            productoID: 3,
            loteID: 3,
            cantidad: 1,
            precioVenta: 30,
            precioCompra: 20,
            tasaIVAPorcentaje: 0,
            exento: true
        }
    ]
};

fetch('/Venta/RegistrarVenta', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(venta)
})
.then(r => r.json())
.then(d => console.log(d));
```

### Paso 3: Validar en BD
```sql
DECLARE @PolizaID INT = (SELECT TOP 1 PolizaID 
                         FROM PolizasContables 
                         ORDER BY FechaPoliza DESC);

-- Debe tener 7 líneas:
-- 1. Débito Clientes/Caja (200)
-- 2. Débito COGS (60 + 30 + 20 = 110)
-- 3. Crédito Ventas (100 + 50 + 30 = 180)
-- 4. Crédito IVA 16% (16)
-- 5. Crédito IVA 8% (4)
-- 6. Crédito IVA 0% (0) - puede no aparecer
-- 7. Crédito Inventario (110)

SELECT COUNT(*) AS NumeroLineas FROM PolizasDetalle WHERE PolizaID = @PolizaID;
-- Debe mostrar: 7 (o 6 si IVA 0% no se registra)

-- Ver detalles
SELECT * FROM PolizasDetalle WHERE PolizaID = @PolizaID ORDER BY Debe DESC;

-- Validar balance
SELECT 
    SUM(Debe) AS TotalDebe,
    SUM(Haber) AS TotalHaber,
    CASE WHEN ABS(SUM(Debe) - SUM(Haber)) < 0.01 THEN 'BALANCEADO ✅' 
         ELSE 'DESBALANCEADO ❌' END AS Status
FROM PolizasDetalle
WHERE PolizaID = @PolizaID;
```

**✅ 7 líneas + Balance OK → TEST PASSED**

---

## Paso 6: Test Case 3 - Auto-población IVA

### Objetivo
Verificar que IVA se auto-popula desde el producto, no desde la UI

### Pasos
1. Crear venta enviando `tasaIVAPorcentaje: 0` en la UI (fuerza un valor incorrecto)
2. Verificar que la póliza refleja el IVA correcto del producto (16%)

```javascript
const venta = {
    clienteID: "CLIENT-GUID",
    total: 116,  // 100 * 1.16
    estatus: "CREDITO",
    detalle: [
        {
            productoID: 1,  // Producto con TasaIVA = 16%
            loteID: 1,
            cantidad: 1,
            precioVenta: 100,
            precioCompra: 60,
            tasaIVAPorcentaje: 0,   // ❌ Intencionalmente INCORRECTO
            exento: false
        }
    ]
};

// El sistema debe OVERRIDE con el valor correcto (16%)
// Resultado: póliza con IVA 16%, NO 0%
```

Validar en BD:
```sql
-- Última póliza debe tener línea de "IVA 16%"
SELECT * FROM PolizasDetalle pd
INNER JOIN CatalogoContable c ON pd.CuentaID = c.CuentaID
WHERE pd.PolizaID = (SELECT TOP 1 PolizaID FROM PolizasContables ORDER BY FechaPoliza DESC)
AND c.SubTipo LIKE 'IVA%';

-- Debe mostrar: IVA_COBRADO_16 (no 0)
```

**✅ IVA es 16% (no lo que se envió en la UI) → AUTO-POBLACIÓN FUNCIONA**

---

## Paso 7: Verificar Todo Está Balanceado

```sql
-- Encontrar pólizas desbalanceadas (no debe haber ninguna)
SELECT 
    p.PolizaID,
    p.FechaPoliza,
    p.Concepto,
    SUM(pd.Debe) AS Debe,
    SUM(pd.Haber) AS Haber
FROM PolizasContables p
LEFT JOIN PolizasDetalle pd ON p.PolizaID = pd.PolizaID
GROUP BY p.PolizaID, p.FechaPoliza, p.Concepto
HAVING ABS(SUM(pd.Debe) - SUM(pd.Haber)) > 0.01;

-- Si no retorna nada → TODO ESTÁ BALANCEADO ✅
```

---

## Resumen de Tests

| Test | Caso | Validación | Status |
|------|------|-----------|--------|
| 1 | Venta simple 16% | 5 líneas, balance OK | ✅/❌ |
| 2 | Venta multi-tasa | 7 líneas, IVA desglosado | ✅/❌ |
| 3 | Auto-población IVA | IVA correcta (DB, no UI) | ✅/❌ |
| 4 | Balance total | Sin pólizas desbalanceadas | ✅/❌ |

---

## Troubleshooting Rápido

### Error: "Falta cuenta CLIENTE"
```sql
-- Verificar que existe
SELECT * FROM CatalogoContable WHERE SubTipo = 'CLIENTE';

-- Si no existe, insertar:
INSERT INTO CatalogoContable 
(CodigoCuenta, NombreCuenta, TipoCuenta, SubTipo)
VALUES ('1100', 'Clientes', 'ACTIVO', 'CLIENTE');
```

### Error: "No se encuentra producto"
```sql
-- Verificar que ProductoID existe
SELECT * FROM Productos WHERE ProductoID = 1;

-- Si no existe, crear uno con:
INSERT INTO Productos (Nombre, CategoriaID, TasaIVAID, ...)
VALUES ('Test Product', 1, 3, ...);  -- TasaIVAID 3 = 16%
```

### Póliza no genera desglose IVA
```sql
-- Verificar que MapeoContableIVA tiene datos
SELECT * FROM MapeoContableIVA WHERE Activo = 1;

-- Si está vacío:
INSERT INTO MapeoContableIVA (TasaIVA, Exento, CuentaDeudora, CuentaAcreedora, Descripcion)
VALUES 
(0.00, 0, 2050, 2050, 'IVA 0%'),
(8.00, 0, 2051, 2051, 'IVA 8%'),
(16.00, 0, 2052, 2052, 'IVA 16%'),
(0.00, 1, 2053, 2053, 'Exento');
```

---

## Checklist Final ✅

- [ ] Compilación sin errores
- [ ] BD tiene CatalogoContable (15 registros)
- [ ] BD tiene MapeoContableIVA (4 registros)
- [ ] Test 1: Venta simple crea póliza balanceada
- [ ] Test 2: Venta multi-tasa desglosada correctamente
- [ ] Test 3: Auto-población IVA funciona
- [ ] Test 4: Sin pólizas desbalanceadas

---

**Tiempo total:** ~20 minutos  
**Resultado esperado:** ✅ TODOS LOS TESTS PASAN

Si tienes dudas o algún test falla, revisar el archivo `MANUAL_DE_PRUEBAS.md` para análisis más detallado.

