# Sistema de Control de Ajustes de Inventario

## Implementación Completa

### 📋 Resumen
Se ha implementado un sistema completo de control y auditoría para cambios en la cantidad de inventario al editar lotes, que incluye:

1. **Modal de justificación obligatoria**
2. **Registro en bitácora de movimientos**
3. **Generación automática de pólizas contables**

---

## 🎯 Funcionalidades Implementadas

### 1. Vista EditarLote.cshtml (FRONTEND)

#### Cambios realizados:
- ✅ Input oculto para capturar cantidad original
- ✅ Input oculto para almacenar motivo del ajuste
- ✅ Modal Bootstrap con:
  - Comparativa visual (cantidad original vs nueva)
  - Select con motivos predefinidos (MERMA, CADUCIDAD, AJUSTE_CONTEO, etc.)
  - Textarea para descripción detallada (mínimo 10 caracteres)
  - Validación de campos requeridos
- ✅ JavaScript para:
  - Detectar cambios en cantidad al enviar formulario
  - Mostrar modal solo si hay diferencia en cantidad
  - Validar y construir motivo completo
  - Cancelar ajuste restaurando cantidad original

#### Motivos disponibles:
**Incrementos:**
- AJUSTE_CONTEO: Ajuste por conteo físico
- CORRECCION_ENTRADA: Corrección de entrada
- DEVOLUCION: Devolución de cliente

**Disminuciones:**
- MERMA: Merma
- CADUCIDAD: Producto caducado
- DAÑADO: Producto dañado
- ROBO: Robo o pérdida
- CORRECCION_SISTEMA: Corrección de sistema

---

### 2. ProductoController.cs (BACKEND)

#### Método EditarLote [HttpPost] - MODIFICADO:
```csharp
public ActionResult EditarLote(LoteProducto lote, string motivoAjuste)
```

**Lógica implementada:**
1. Obtiene lote original de BD para comparar cantidades
2. Calcula diferencia de cantidad
3. **Si hay diferencia:**
   - Valida que exista motivo (obligatorio)
   - Ajusta `CantidadDisponible` proporcionalmente
   - Registra movimiento en bitácora
   - Crea póliza contable según tipo de ajuste
4. Actualiza lote en BD
5. Muestra mensaje de éxito con detalle del ajuste

#### Método CrearPolizaAjusteInventario (NUEVO):
```csharp
private void CrearPolizaAjusteInventario(LoteProducto lote, int diferenciaCantidad, string motivo)
```

**Pólizas generadas:**

**Para INCREMENTOS:**
```
DEBE: Inventario (Activo)     $XXX
HABER: Ajuste Inventario       $XXX
```

**Para DISMINUCIONES (Merma/Caducidad/Daño):**
```
DEBE: Costo de Ventas/Mermas  $XXX
HABER: Inventario (Activo)     $XXX
```

**Para DISMINUCIONES (Otras):**
```
DEBE: Ajustes de Inventario    $XXX
HABER: Inventario (Activo)     $XXX
```

#### Método BitacoraInventario [HttpGet] (NUEVO):
Vista para consultar historial de movimientos con filtros opcionales.

---

### 3. CD_Producto.cs (CAPA DE DATOS)

#### Método ActualizarLote - MODIFICADO:
Ahora actualiza también:
- `CantidadTotal`
- `CantidadDisponible`
- `Usuario`

#### Método RegistrarMovimientoInventario (NUEVO):
```csharp
public bool RegistrarMovimientoInventario(MovimientoInventario movimiento)
```
Inserta registro en tabla `MovimientosInventario` con:
- LoteID, ProductoID
- TipoMovimiento (AJUSTE_ENTRADA, AJUSTE_SALIDA, MERMA, etc.)
- Cantidad, CostoUnitario
- Usuario, Fecha
- Comentarios (motivo completo)

#### Método ObtenerMovimientosInventario (NUEVO):
```csharp
public List<MovimientoInventario> ObtenerMovimientosInventario(
    int? productoId, int? loteId, DateTime? fechaInicio, DateTime? fechaFin)
```
Consulta bitácora con filtros opcionales.

---

### 4. Base de Datos (SCRIPTS SQL)

#### Tabla MovimientosInventario:
```sql
CREATE TABLE MovimientosInventario (
    MovimientoID INT IDENTITY(1,1) PRIMARY KEY,
    LoteID INT NOT NULL,
    ProductoID INT NOT NULL,
    TipoMovimiento VARCHAR(50) NOT NULL,
    Cantidad INT NOT NULL,
    CostoUnitario DECIMAL(18,2) NOT NULL,
    Usuario VARCHAR(100) NOT NULL,
    Fecha DATETIME NOT NULL DEFAULT GETDATE(),
    Comentarios VARCHAR(500),
    -- Foreign Keys y índices
);
```

#### Vista vw_BitacoraInventario:
Unifica información de movimientos con datos de productos, lotes y categorías.

**Campos disponibles:**
- MovimientoID, Fecha, TipoMovimiento
- Cantidad, CostoUnitario, CostoTotal
- Usuario, Comentarios
- LoteID, LoteCantidadTotal, LoteCantidadDisponible
- ProductoID, ProductoNombre, ProductoCodigo
- CategoriaNombre

---

## 🔐 Seguridad y Auditoría

### Trazabilidad completa:
- ✅ **Usuario** que realizó el cambio
- ✅ **Fecha y hora** exacta
- ✅ **Cantidad anterior** y **cantidad nueva** (implícito por diferencia)
- ✅ **Motivo clasificado** (tipo de movimiento)
- ✅ **Descripción detallada** del ajuste
- ✅ **Póliza contable** asociada automáticamente

### Validaciones:
- ✅ No permite guardar sin justificación si cambió la cantidad
- ✅ Motivo obligatorio (select)
- ✅ Descripción mínima de 10 caracteres
- ✅ Validación de precios (venta > compra)

---

## 📊 Consultas de Ejemplo

### Últimos 50 movimientos:
```sql
SELECT TOP 50 * FROM vw_BitacoraInventario
ORDER BY Fecha DESC;
```

### Resumen por tipo (último mes):
```sql
SELECT 
    TipoMovimiento,
    COUNT(*) AS TotalMovimientos,
    SUM(Cantidad) AS TotalUnidades,
    SUM(CostoTotal) AS CostoTotalAcumulado
FROM vw_BitacoraInventario
WHERE Fecha >= DATEADD(MONTH, -1, GETDATE())
GROUP BY TipoMovimiento
ORDER BY CostoTotalAcumulado DESC;
```

### Movimientos de un producto específico:
```sql
SELECT * FROM vw_BitacoraInventario
WHERE ProductoID = 123
ORDER BY Fecha DESC;
```

---

## 🚀 Pasos para Activar

### 1. Ejecutar scripts SQL:
```bash
sqlcmd -S . -d DB_TIENDA -i crear_tabla_movimientos_inventario.sql
sqlcmd -S . -d DB_TIENDA -i crear_vista_bitacora_inventario.sql
```

### 2. Compilar proyecto:
Los cambios en código ya están aplicados en:
- `VentasWeb/Views/Producto/EditarLote.cshtml`
- `VentasWeb/Controllers/ProductoController.cs`
- `CapaDatos/CD_Producto.cs`

### 3. Probar funcionalidad:
1. Ir a Productos → Ver Lotes
2. Editar un lote
3. Cambiar la Cantidad Total
4. Al guardar, aparecerá el modal
5. Completar motivo y descripción
6. Confirmar → Se guarda con registro completo

---

## 📝 Notas Técnicas

- **CantidadDisponible** se ajusta proporcionalmente al cambio en CantidadTotal
- **Pólizas** se generan automáticamente según el tipo de ajuste
- **Modal** es obligatorio solo cuando cambia la cantidad
- **Bitácora** es consultable desde `Producto/BitacoraInventario`
- **IDs de cuentas contables** (1, 50, 60) deben ajustarse según catálogo real

---

## ✅ Checklist de Implementación

- [x] Modal de justificación en frontend
- [x] Validación JavaScript de campos
- [x] Método EditarLote modificado para detectar cambios
- [x] Registro en MovimientosInventario
- [x] Generación de pólizas contables
- [x] Método para consultar bitácora
- [x] Script SQL de tabla MovimientosInventario
- [x] Script SQL de vista vw_BitacoraInventario
- [x] Ajuste proporcional de CantidadDisponible
- [x] Mensajes de éxito informativos
- [x] Manejo de errores con try-catch

---

## 🎨 Interfaz de Usuario

El modal muestra claramente:
- **Cantidad Original**: 100 unidades
- **Cantidad Nueva**: 85 unidades
- **Diferencia**: -15 unidades (Disminución) ← en rojo
- O **Diferencia**: +20 unidades (Incremento) ← en verde

El usuario **no puede** guardar cambios en cantidad sin justificar el motivo.
