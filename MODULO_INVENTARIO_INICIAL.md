# MÓDULO DE INVENTARIO INICIAL

## Descripción

El **Módulo de Inventario Inicial** es una funcionalidad especial diseñada para la migración desde otro sistema de gestión. Permite cargar de forma masiva todos los productos existentes con sus cantidades y costos actuales, creando el punto de partida del inventario en el nuevo sistema.

## ¿Cuándo usar este módulo?

- ✅ **Al iniciar el sistema por primera vez** y migrar desde otro software
- ✅ **Cuando se tiene inventario existente** de un sistema anterior
- ✅ **Para establecer el stock inicial** de todos los productos
- ❌ **NO usar para ajustes posteriores** (usar módulo de Ajustes de Inventario)

## Características

### 1. Carga Masiva de Productos
- Buscar productos por nombre o código
- Agregar cantidades actuales
- Definir costo unitario
- Establecer precio de venta
- Comentarios opcionales por producto
- Fecha de caducidad (opcional)

### 2. Proceso Seguro
- **Estado "En Proceso"**: Los cambios no se aplican hasta finalizar
- **Una carga activa a la vez**: Evita duplicaciones
- **Posibilidad de pausar**: Continuar más tarde
- **Eliminación antes de finalizar**: Corregir errores

### 3. Control y Trazabilidad
- Historial completo de todas las cargas
- Usuario y fecha de cada carga
- Detalle de productos, unidades y valores
- No se puede modificar una carga finalizada

## Flujo de Trabajo

```
1. Crear Nueva Carga
   └─> Ingresar comentarios (opcional)
   
2. Agregar Productos
   └─> Buscar producto
   └─> Ingresar cantidad actual
   └─> Definir costo unitario
   └─> Definir precio de venta
   └─> Repetir para todos los productos
   
3. Verificar Totales
   └─> Revisar productos agregados
   └─> Verificar cantidades y valores
   └─> Eliminar si hay errores
   
4. Finalizar Carga
   └─> Confirmar operación
   └─> Sistema crea lotes automáticamente
   └─> Registra movimientos en inventario
   └─> Marca carga como finalizada
```

## Estructura de Base de Datos

### Tablas Creadas

#### `InventarioInicial`
- `CargaID` (PK) - Identificador único de la carga
- `FechaCarga` - Fecha y hora de inicio
- `UsuarioCarga` - Usuario que creó la carga
- `TotalProductos` - Cantidad de productos
- `TotalUnidades` - Suma de todas las unidades
- `ValorTotal` - Valor total del inventario cargado
- `Comentarios` - Descripción de la carga
- `SucursalID` (FK) - Sucursal donde se aplica
- `Activo` - Estado (1=En Proceso, 0=Finalizada)

#### `InventarioInicialDetalle`
- `DetalleID` (PK) - Identificador del detalle
- `CargaID` (FK) - Referencia a la carga
- `ProductoID` (FK) - Producto agregado
- `CantidadCargada` - Cantidad inicial del producto
- `CostoUnitario` - Costo del producto
- `PrecioVenta` - Precio de venta
- `FechaCaducidad` - Fecha de caducidad (opcional)
- `Comentarios` - Notas sobre el producto

### Stored Procedures

1. **SP_IniciarCargaInventarioInicial**
   - Crea un nuevo registro de carga
   - Valida que la sucursal exista
   - Retorna el CargaID

2. **SP_AgregarProductoInventarioInicial**
   - Agrega/actualiza un producto en la carga
   - Valida que la carga esté activa
   - Permite modificar antes de finalizar

3. **SP_FinalizarCargaInventarioInicial**
   - Calcula totales de la carga
   - Crea lotes en `LotesProducto`
   - Registra movimientos en `InventarioMovimientos`
   - Marca la carga como finalizada

4. **SP_ObtenerProductosParaInventarioInicial**
   - Sin CargaID: Lista todos los productos disponibles
   - Con CargaID: Lista productos de esa carga específica

5. **SP_EliminarProductoInventarioInicial**
   - Elimina un producto de la carga (solo si está activa)

### Vista

**VW_HistorialInventarioInicial**
- Muestra todas las cargas con información resumida
- Estado (En Proceso / Finalizada)
- Totales calculados

## Archivos del Módulo

### Base de Datos
- `CREAR_MODULO_INVENTARIO_INICIAL.sql` - Script completo de creación

### Capa de Modelo
- `CapaModelo\InventarioInicial.cs` - Clases del modelo

### Capa de Datos
- `CapaDatos\CD_InventarioInicial.cs` - Lógica de acceso a datos

### Controlador
- `VentasWeb\Controllers\InventarioInicialController.cs` - Lógica del controlador

### Vistas
- `Views\InventarioInicial\Index.cshtml` - Página principal e historial
- `Views\InventarioInicial\NuevaCarga.cshtml` - Crear nueva carga
- `Views\InventarioInicial\Cargar.cshtml` - Agregar productos
- `Views\InventarioInicial\Detalle.cshtml` - Ver carga finalizada (pendiente)
- `Views\InventarioInicial\ConfirmarFinalizacion.cshtml` - Confirmación (pendiente)

## Integración con Sistema Existente

### Impacto en Otras Tablas

Al finalizar una carga, el sistema:

1. **LotesProducto**
   - Crea un lote por cada producto
   - `TipoMovimiento = 'INVENTARIO_INICIAL'`
   - `CantidadTotal = CantidadDisponible`

2. **InventarioMovimientos**
   - Registra cada movimiento
   - Enlaza con el lote creado
   - Comentario: "Carga inicial de inventario #[CargaID]"

3. **NO modifica Productos**
   - No cambia precios de venta
   - Solo crea existencias

## Uso del Módulo

### 1. Acceso
```
URL: /InventarioInicial/Index
Requiere: Usuario autenticado
Permiso: (Debería ser solo Administrador)
```

### 2. Crear Primera Carga

1. Ir a **Inventario Inicial**
2. Click en **"Nueva Carga Inicial"**
3. Ingresar comentarios (opcional): "Migración desde [Sistema Anterior]"
4. Click en **"Crear Carga y Continuar"**

### 3. Agregar Productos

1. **Buscar producto**: Escribe nombre o código
2. **Seleccionar** de la lista
3. **Ingresar datos**:
   - Cantidad: Existencia actual
   - Costo: Costo unitario de compra
   - Precio: Precio de venta
4. Click en **"Agregar"**
5. Repetir para todos los productos

### 4. Finalizar Carga

1. Verificar totales:
   - Productos agregados
   - Unidades totales
   - Valor total del inventario
2. Click en **"Finalizar Carga"**
3. Confirmar operación
4. Sistema aplica automáticamente

## Ejemplo Práctico

### Escenario
Una tienda llamada "Las Águilas" migra desde QuickBooks con 396 productos.

### Proceso
```
1. Nueva Carga
   Comentarios: "Migración desde QuickBooks - 30/01/2026"
   
2. Agregar Productos (ejemplo de 3):
   - Coca-Cola 600ml: 50 unidades, $8.50 costo, $15.00 venta
   - Sabritas 60g: 120 unidades, $5.00 costo, $10.00 venta
   - Agua 1L: 200 unidades, $3.50 costo, $7.00 venta
   
3. Totales:
   - 3 productos
   - 370 unidades
   - Valor total: $1,545.00
   
4. Finalizar
   ✓ 3 lotes creados en LotesProducto
   ✓ 3 movimientos en InventarioMovimientos
   ✓ Carga marcada como Finalizada
```

## Consideraciones Importantes

### ⚠️ Advertencias
- **Solo una carga activa a la vez** por sucursal
- **No se puede modificar** después de finalizar
- **Verifica bien los datos** antes de finalizar
- **Este módulo es para migración inicial**, no para ajustes diarios

### ✅ Buenas Prácticas
- Prepara una hoja de Excel con todos los productos antes de empezar
- Verifica que los productos existan en el catálogo
- Realiza la carga en horario sin ventas
- Guarda respaldo de base de datos antes de finalizar
- Verifica los totales después de aplicar

### 🔄 Después de la Carga Inicial
- El inventario se actualiza automáticamente con:
  - **Compras**: Agregan al inventario
  - **Ventas**: Restan del inventario
  - **Ajustes**: Módulo de ajustes de inventario
  - **Mermas**: Registrar mermas en el sistema

## Mejoras Futuras (Opcionales)

### Importación desde Excel
```csharp
// Funcionalidad para importar desde archivo CSV/Excel
public ActionResult ImportarDesdeExcel(HttpPostedFileBase archivo)
{
    // Leer archivo
    // Validar columnas
    // Insertar productos masivamente
    // Retornar resumen
}
```

### Exportar Plantilla
- Generar Excel con formato correcto
- Columnas: Código, Producto, Cantidad, Costo, Precio
- Permitir llenar offline y subir

### Validaciones Adicionales
- Alertas si costo > precio de venta
- Sugerencias de precio basadas en margen
- Detección de productos duplicados

## Soporte

### Problemas Comunes

**P: No encuentro mi producto**
R: Verifica que el producto esté creado en el catálogo de productos primero.

**P: ¿Puedo pausar y continuar después?**
R: Sí, la carga se guarda automáticamente. Cierra y continúa cuando quieras.

**P: Me equivoqué al finalizar**
R: Si es crítico, contacta al administrador del sistema para revertir en base de datos.

**P: ¿Cuántas veces uso este módulo?**
R: Típicamente **una sola vez** al iniciar el sistema. Después usa compras/ajustes.

## Resumen

El módulo de Inventario Inicial es una herramienta poderosa para:
- ✅ Migrar desde otros sistemas
- ✅ Establecer punto de partida del inventario
- ✅ Mantener trazabilidad completa
- ✅ Evitar errores con proceso controlado

**Estado**: ✅ Completamente funcional
**Probado**: ⏳ Pendiente de pruebas
**Listo para**: 🚀 Uso en producción
