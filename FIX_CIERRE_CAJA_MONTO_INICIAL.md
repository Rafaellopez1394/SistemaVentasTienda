# Fix: Cierre de Caja - Monto Inicial

## Problema Identificado
El módulo de Cierre de Caja no estaba considerando el **monto inicial** registrado en la apertura de caja al calcular el efectivo disponible.

**Cálculo anterior (incorrecto):**
```
Efectivo en Caja = Ventas Efectivo - Gastos Efectivo - Retiros
```

**Cálculo correcto (actualizado):**
```
Efectivo en Caja = Monto Inicial + Ventas Efectivo - Gastos Efectivo - Retiros
```

---

## Solución Implementada

### 1. **Base de Datos - Stored Procedure**
**Archivo:** `Utilidad/SQL Server/042_FIX_CIERRE_CAJA_MONTO_INICIAL.sql`

**Cambios:**
- Se agregó consulta para obtener el monto inicial de apertura desde `MovimientosCaja`
- Se añadió columna `MontoInicial` en el resultado del SP
- Se actualizó la fórmula de cálculo de `EfectivoEnCaja`

```sql
-- Obtener monto inicial
SELECT @MontoInicial = ISNULL(Monto, 0)
FROM MovimientosCaja
WHERE CajaID = @CajaID
  AND TipoMovimiento = 'APERTURA'
  AND CAST(FechaMovimiento AS DATE) = @Fecha
ORDER BY FechaMovimiento DESC;

-- Nuevo cálculo
(@MontoInicial + @VentasEfectivo - @GastosEfectivo - @TotalRetiros) AS EfectivoEnCaja
```

### 2. **Modelo C# - CierreCajaConGastos**
**Archivo:** `CapaModelo/Gasto.cs`

**Cambio:**
- Se agregó propiedad `MontoInicial` al modelo

```csharp
public class CierreCajaConGastos
{
    public int CajaID { get; set; }
    public DateTime Fecha { get; set; }
    public decimal MontoInicial { get; set; }  // ← NUEVO
    public decimal TotalVentas { get; set; }
    // ... resto de propiedades
}
```

### 3. **Capa de Datos - CD_Gasto**
**Archivo:** `CapaDatos/CD_Gasto.cs`

**Cambio:**
- Se lee el campo `MontoInicial` del stored procedure

```csharp
cierre = new CierreCajaConGastos
{
    CajaID = reader.GetInt32(reader.GetOrdinal("CajaID")),
    Fecha = reader.GetDateTime(reader.GetOrdinal("Fecha")),
    MontoInicial = reader.GetDecimal(reader.GetOrdinal("MontoInicial")),  // ← NUEVO
    TotalVentas = reader.GetDecimal(reader.GetOrdinal("TotalVentas")),
    // ...
};
```

### 4. **Vista - CierreCaja.cshtml**
**Archivo:** `VentasWeb/Views/Gastos/CierreCaja.cshtml`

**Cambios:**
- Se agregó info-box para mostrar el monto inicial
- Se actualizó la tabla de "Efectivo Esperado en Caja" para incluir el monto inicial
- Se reorganizaron los indicadores en dos filas

**Nueva estructura:**

**Fila 1:**
- 🏦 Monto Inicial (azul)
- 🛒 Total Ventas (verde)
- 💰 Total Gastos (rojo)
- 📈 Ganancia Neta (amarillo)

**Fila 2:**
- 💵 Efectivo Esperado en Caja (aqua) - Con fórmula visible

**Tabla detallada:**
```
Monto Inicial (Apertura):  $3,000.00
(+) Ventas en Efectivo:    $    0.00
(-) Gastos en Efectivo:    $  500.00
(-) Retiros:               $    0.00
= EFECTIVO EN CAJA:        $2,500.00
```

### 5. **JavaScript - CierreCaja.js**
**Archivo:** `VentasWeb/Scripts/Gastos/CierreCaja.js`

**Cambios:**
- Se agregó actualización del campo `#montoInicial`
- Se agregó actualización del campo `#resumenMontoInicial`

```javascript
$('#montoInicial').text('$' + formatMoney(data.MontoInicial));
$('#resumenMontoInicial').text('$' + formatMoney(data.MontoInicial));
```

---

## Prueba de Funcionamiento

**Ejecución del SP actualizado:**
```sql
EXEC sp_CierreCajaConGastos @CajaID = 1, @Fecha = '2026-01-04';
```

**Resultado:**
```
CajaID: 1
Fecha: 2026-01-04
MontoInicial: $3,000.00
TotalVentas: $0.00
VentasEfectivo: $0.00
GastosEfectivo: $500.00
TotalRetiros: $0.00
EfectivoEnCaja: $2,500.00  ← Correcto!
GananciaNeta: -$500.00
```

**Fórmula aplicada:**
```
$2,500.00 = $3,000.00 + $0.00 - $500.00 - $0.00
           (Inicial)  (Ventas) (Gastos) (Retiros)
```

---

## Archivos Modificados

| Archivo | Tipo | Descripción |
|---------|------|-------------|
| `SQL Server/040_MODULO_GASTOS.sql` | SQL | Actualizado el SP (referencia) |
| `Utilidad/SQL Server/042_FIX_CIERRE_CAJA_MONTO_INICIAL.sql` | SQL | Script de actualización |
| `CapaModelo/Gasto.cs` | C# | Agregada propiedad MontoInicial |
| `CapaDatos/CD_Gasto.cs` | C# | Lectura del nuevo campo |
| `VentasWeb/Views/Gastos/CierreCaja.cshtml` | HTML/Razor | UI actualizada |
| `VentasWeb/Scripts/Gastos/CierreCaja.js` | JavaScript | Lógica de visualización |

---

## Estado Final

✅ **Compilación exitosa** (0 errores)  
✅ **Stored procedure actualizado** en base de datos  
✅ **Modelo C# actualizado** con nuevo campo  
✅ **Vista actualizada** con diseño mejorado  
✅ **JavaScript actualizado** para mostrar monto inicial  
✅ **Prueba exitosa** con datos reales  

---

## Beneficios

1. **Cálculo correcto**: El efectivo en caja ahora refleja el valor real esperado
2. **Trazabilidad**: Se puede ver claramente el monto inicial de cada día
3. **Control**: Facilita la detección de faltantes o sobrantes de efectivo
4. **Transparencia**: La fórmula es visible en la interfaz

---

## Notas Técnicas

### Tabla MovimientosCaja
El monto inicial se registra con:
```sql
INSERT INTO MovimientosCaja (CajaID, TipoMovimiento, Monto, ...)
VALUES (@CajaID, 'APERTURA', @MontoInicial, ...);
```

### Procedimiento de Apertura
El stored procedure `AperturaCaja` se encarga de registrar el monto inicial cada día.

**Verificación:**
```sql
SELECT * FROM MovimientosCaja 
WHERE TipoMovimiento = 'APERTURA' 
ORDER BY FechaMovimiento DESC;
```

---

**Fecha de Implementación:** 2026-01-04  
**Estado:** ✅ COMPLETADO Y PROBADO  
**Impacto:** CRÍTICO - Corrige cálculo de efectivo en caja
