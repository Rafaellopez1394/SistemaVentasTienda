# 🏪 SELECCIÓN DE SUCURSAL EN COMPRAS - GUÍA RÁPIDA

## ✅ FUNCIONALIDAD MEJORADA

La selección de sucursal en el módulo de compras ya existía, pero ahora está más visible y clara.

---

## 📍 UBICACIÓN

En la vista de **Compras → Registrar Manual**, encontrarás una sección destacada:

```
┌─────────────────────────────────────────┐
│ 🏪 Sucursal Destino [REQUERIDO]        │
│                                          │
│ RFC: [Sin seleccionar]                  │
│ Nombre: [Sin seleccionar]               │
│ [Seleccionar] ← Click aquí             │
└─────────────────────────────────────────┘
```

---

## 🎯 CÓMO USAR

### Paso 1: Seleccionar Proveedor
1. En la sección **Detalle Proveedor**
2. Click en **Buscar**
3. Selecciona el proveedor de la lista

### Paso 2: Seleccionar Sucursal ⭐ NUEVO
1. En la sección **Sucursal Destino**
2. Click en el botón **Seleccionar**
3. Se abre un modal con la lista de sucursales
4. Click en el ícono ✅ de la sucursal deseada
5. Los datos se llenan automáticamente
6. El badge rojo **[REQUERIDO]** desaparece

### Paso 3: Agregar Productos
1. Click en **Buscar** de la sección Producto
2. **NOTA:** Solo aparecen productos si ya seleccionaste una sucursal
3. Selecciona productos y agrega cantidades/precios

### Paso 4: Registrar Compra
1. Revisa la tabla de productos
2. Click en **Registrar Compra**
3. ✅ Los lotes se crean en la sucursal seleccionada

---

## 🔍 VALIDACIONES

El sistema valida:

✅ **Proveedor requerido:** Debe seleccionar un proveedor
✅ **Sucursal requerida:** Debe seleccionar una sucursal
✅ **Productos requeridos:** Debe agregar al menos un producto

Si falta la sucursal, verás el mensaje:
```
⚠️ "Debe seleccionar una tienda primero"
```

---

## 💡 BENEFICIOS

### Antes:
- Los lotes se creaban sin saber a qué sucursal pertenecían
- No se podía hacer seguimiento de inventario por sucursal

### Ahora:
- ✅ Cada lote se vincula a la sucursal donde se recibe la mercancía
- ✅ El inventario se lleva por sucursal
- ✅ Permite hacer traspasos entre sucursales
- ✅ Reportes de inventario por ubicación

---

## 🗂️ IMPACTO EN LOTES

Cuando registras una compra con sucursal seleccionada:

```sql
-- Ejemplo de lote creado:
INSERT INTO LotesProducto (
    ProductoID,
    SucursalID,        ← ⭐ NUEVO: Vinculado a sucursal
    CantidadInicial,
    CantidadDisponible,
    PrecioUnitarioCompra,
    FechaRecepcion,
    Tipo
) VALUES (
    10,                 -- ProductoID
    1,                  -- SucursalID (Matriz)
    50.000,            -- Cantidad
    50.000,
    120.00,
    GETDATE(),
    'COMPRA'
);
```

---

## 📊 EJEMPLO PRÁCTICO

**Escenario:** Compra de 50 kg de camarón para la sucursal Matriz

1. **Seleccionar proveedor:** Mariscos del Pacífico
2. **Seleccionar sucursal:** Matriz (RFC: AAA123456BBB)
3. **Agregar producto:**
   - Camarón 41-50
   - Cantidad: 50 kg
   - Precio: $120.00/kg
4. **Registrar**
5. **Resultado:**
   - Se crea lote en LotesProducto con SucursalID=1
   - El inventario queda:
     ```
     Sucursal Matriz: 50 kg de camarón
     ```

---

## 🔗 INTEGRACIÓN CON TRASPASOS

Este cambio habilita el módulo de traspasos:

1. **Compra en Matriz:** 50 kg camarón
2. **Traspaso de Matriz → Centro:** 10 kg
3. **Resultado:**
   - Matriz: 40 kg
   - Centro: 10 kg

Cada sucursal tiene su propio inventario independiente.

---

## ⚠️ IMPORTANTE

### Para Compras Anteriores

Si tienes compras registradas antes de esta actualización:

```sql
-- Verificar lotes sin sucursal asignada:
SELECT COUNT(*) 
FROM LotesProducto 
WHERE SucursalID IS NULL;

-- El script 050_MODULO_TRASPASOS.sql ya asignó todos 
-- los lotes existentes a SucursalID = 1 (Matriz)
```

### Para Nuevas Compras

- ⚠️ **OBLIGATORIO** seleccionar sucursal
- Sin sucursal, no puedes buscar productos
- Sin sucursal, no puedes registrar la compra

---

## 🆘 TROUBLESHOOTING

### Problema: No aparecen sucursales en el modal
**Solución:**
```sql
-- Verificar que existen sucursales activas:
SELECT * FROM SUCURSAL WHERE Activo = 1;
```

### Problema: El botón "Buscar" productos no muestra nada
**Solución:** Primero debes seleccionar una sucursal.

### Problema: Badge rojo [REQUERIDO] no desaparece
**Solución:** Asegúrate de hacer clic en la sucursal dentro del modal, no solo cerrar el modal.

---

## 📝 CAMBIOS REALIZADOS

### Interfaz Mejorada:
- ✅ Badge rojo **[REQUERIDO]** para destacar campo obligatorio
- ✅ Icono 🏪 en el título
- ✅ Placeholder "Sin seleccionar" en campos vacíos
- ✅ Botón renombrado de "Buscar" a "Seleccionar"
- ✅ Badge se oculta automáticamente al seleccionar sucursal

### JavaScript Actualizado:
- ✅ Función `Sucursaleselect()` oculta badge al seleccionar
- ✅ Al limpiar formulario, badge vuelve a aparecer

---

## ✨ RESUMEN

La funcionalidad de selección de sucursal en compras:
- ✅ **YA EXISTÍA** pero no era evidente
- ✅ **AHORA MEJORADA** con indicadores visuales claros
- ✅ **ES OBLIGATORIA** para el control de inventario por sucursal
- ✅ **HABILITA** el módulo de traspasos entre sucursales

**Ubicación:** Compras → Registrar Manual
**Acción:** Click en botón "Seleccionar" de la sección "Sucursal Destino"

---

**Fecha:** Enero 4, 2026
**Módulo:** Compras
**Estado:** ✅ MEJORADO
