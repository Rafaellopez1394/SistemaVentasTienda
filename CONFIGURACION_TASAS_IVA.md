# Configuración de Tasas de IVA por Producto

## El Problema

Actualmente todos los productos en el sistema están configurados con IVA del 16%, pero en México existen diferentes tasas de IVA según el tipo de producto:

- **IVA 16%** - Tasa General
- **IVA 8%** - Zona Fronteriza
- **IVA 0%** - Tasa Cero (productos básicos)
- **Exento** - Sin IVA (libros, revistas)

## La Solución

El sistema **YA TIENE** soporte para diferentes tasas de IVA. Solo necesita configurarse correctamente.

### 1. Ejecutar Script de Configuración

```sql
-- Ubicación del script
c:\Users\Rafael Lopez\Documents\SistemaVentasTienda\Utilidad\SQL Server\021_CONFIGURAR_TASAS_IVA.sql
```

Este script:
- ✅ Crea la tabla `CatTasaIVA` si no existe
- ✅ Inserta las 4 tasas de IVA disponibles en México
- ✅ Agrega las columnas necesarias a la tabla PRODUCTO
- ✅ Actualiza productos existentes con IVA 16% por defecto
- ✅ Crea vista para consultar productos con su IVA
- ✅ Configura tabla de IEPS (impuestos especiales)

### 2. Productos por Tipo de IVA

#### IVA 16% - Tasa General
La mayoría de productos llevan esta tasa:
- ✓ Refrescos, jugos industrializados
- ✓ Dulces, chocolates, galletas
- ✓ Productos de limpieza
- ✓ Electrónicos
- ✓ Ropa y calzado
- ✓ Cosméticos
- ✓ Alimentos procesados no básicos

#### IVA 0% - Tasa Cero
Productos de consumo básico:
- ✓ Pan y tortillas
- ✓ Leche y derivados lácteos básicos
- ✓ Huevos
- ✓ Carne, pollo, pescado fresco
- ✓ Frutas y verduras frescas
- ✓ Aceites vegetales comestibles
- ✓ Harinas, frijol, arroz
- ✓ Medicinas de patente

#### Exento de IVA
- ✓ Libros
- ✓ Periódicos y revistas
- ✓ Servicios educativos

#### IVA 8% - Zona Fronteriza
Solo aplica para comercios en zona fronteriza (se usa en lugar del 16%)

### 3. Cómo Configurar en el Sistema

#### Opción A: Desde la Interfaz Web

1. **Ir a Productos**
   - Menú: Mantenedor → Productos

2. **Crear o Editar Producto**
   - Al crear/editar un producto, verás el campo "Tasa IVA"
   - Selecciona la tasa correcta según el tipo de producto:
     * **IVA 16%** - Para productos generales
     * **IVA 0%** - Para alimentos básicos
     * **Exento** - Para libros/revistas
     * **IVA 8%** - Solo zona fronteriza

3. **Guardar**
   - El sistema aplicará automáticamente la tasa correcta en ventas

#### Opción B: Actualización Masiva por SQL

```sql
-- Ejemplo: Actualizar productos de alimentos básicos a IVA 0%
UPDATE PRODUCTO
SET TasaIVAID = (SELECT TasaIVAID FROM CatTasaIVA WHERE Porcentaje = 0.00)
WHERE CategoriaID = (SELECT CategoriaID FROM CATEGORIA WHERE Descripcion = 'Frutas');

-- Ejemplo: Actualizar productos de limpieza a IVA 16%
UPDATE PRODUCTO
SET TasaIVAID = (SELECT TasaIVAID FROM CatTasaIVA WHERE Porcentaje = 16.00)
WHERE CategoriaID = (SELECT CategoriaID FROM CATEGORIA WHERE Descripcion = 'Limpieza');
```

### 4. Verificar Configuración

```sql
-- Ver todos los productos con su tasa de IVA
SELECT * FROM vw_ProductosConIVA;

-- Ver productos por tasa de IVA
SELECT 
    t.Descripcion AS TasaIVA,
    COUNT(*) AS TotalProductos
FROM PRODUCTO p
INNER JOIN CatTasaIVA t ON p.TasaIVAID = t.TasaIVAID
WHERE p.Activo = 1
GROUP BY t.Descripcion;
```

### 5. Impacto en Ventas

Una vez configurado, el sistema:

✅ **Calcula automáticamente** el IVA según el producto
✅ **Muestra el desglose** de IVA en tickets y facturas
✅ **Genera reportes** de IVA trasladado por tasa (0%, 8%, 16%)
✅ **Cumple con CFDI 4.0** para facturación electrónica
✅ **Registra pólizas contables** con el IVA correcto por cuenta

### 6. Ejemplo de Venta con Diferentes Tasas

**Ticket:**
```
Producto                  Cantidad  Precio    IVA%   Subtotal   IVA      Total
------------------------------------------------------------------------
Coca Cola 600ml           2         $15.00    16%    $30.00     $4.80    $34.80
Pan Integral              1         $35.00    0%     $35.00     $0.00    $35.00
Leche 1L                  1         $22.00    0%     $22.00     $0.00    $22.00
Jabón Líquido             1         $45.00    16%    $45.00     $7.20    $52.20
------------------------------------------------------------------------
                                              SUBTOTAL:          $132.00
                                              IVA 16%:           $12.00
                                              IVA 0%:            $0.00
                                              TOTAL:             $144.00
```

### 7. Reportes Contables

El sistema genera automáticamente:

- **Reporte de IVA Trasladado** (cobrado en ventas)
  - IVA 16% trasladado
  - IVA 8% trasladado
  - IVA 0% trasladado

- **Reporte de IVA Acreditable** (pagado en compras)
  - IVA 16% acreditable
  - IVA 8% acreditable

- **IVA a Pagar** = IVA Trasladado - IVA Acreditable

### 8. Referencias Legales

Según la Ley del IVA en México:

**Artículo 2-A (Tasa 0%)**
- Alimentos básicos
- Medicinas de patente
- Productos agropecuarios no industrializados

**Artículo 9 (Exento)**
- Libros, periódicos y revistas
- Servicios educativos
- Vivienda en arrendamiento

**Artículo 1 (Tasa 16%)**
- Todos los demás productos y servicios no especificados

### 9. Próximos Pasos

1. ✅ **Ejecutar** el script `021_CONFIGURAR_TASAS_IVA.sql`
2. 📝 **Revisar** el catálogo de productos actual
3. 🔧 **Configurar** la tasa correcta para cada producto
4. ✅ **Verificar** que los cálculos sean correctos en ventas
5. 📊 **Generar** reporte de IVA del mes

## Soporte Técnico

El sistema ya tiene implementado:
- ✅ Modelo de datos: `CapaModelo/Producto.cs` (TasaIVAID, TasaIVAPorcentaje)
- ✅ Capa de datos: `CapaDatos/CD_Catalogo.cs` (ObtenerTasasIVA)
- ✅ Controlador: `ProductoController.cs` (ViewBag.TasasIVA)
- ✅ Vista: `Producto/Index.cshtml` (Dropdown de tasas)
- ✅ Cálculos: Stored procedures de ventas incluyen la tasa del producto
- ✅ Reportes: `CD_ReportesContables.cs` (desglose por tasa)

**Todo está listo, solo falta configurar las tasas correctas en cada producto.**
