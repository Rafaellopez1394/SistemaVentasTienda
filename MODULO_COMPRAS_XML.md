# 📦 Módulo de Compras con Carga de Facturas XML

## 📝 Descripción General

Sistema completo para registrar compras desde **facturas XML (CFDI 4.0/3.3)** con desglose automático de cantidades y generación de lotes de inventario. Permite convertir unidades de compra (cajas, paquetes) a unidades de venta (piezas individuales).

## ✅ ¿Qué se implementó?

### 1. Parser de XML CFDI
- ✅ **CFDICompraParser.cs** (457 líneas): Extrae todos los datos del XML
  - Datos del comprobante (serie, folio, fecha, total)
  - Emisor/Proveedor (RFC, razón social, régimen fiscal)
  - Receptor (nuestra empresa)
  - Conceptos con impuestos (IVA trasladado/retenido)
  - Timbre fiscal digital (UUID, sellos SAT)
- ✅ Validación de estructura XML
- ✅ Soporte CFDI 4.0 y 3.3

### 2. Lógica de Negocio
- ✅ **RegistrarCompraDesdeXML()**: Procesa XML y crea compra con lotes
- ✅ **BuscarOCrearProveedor()**: Auto-registro de proveedores por RFC
- ✅ **GuardarXMLRespaldo()**: Copia de seguridad en carpeta XMLCompras/
- ✅ **Factor de conversión**: 2 cajas × 8 piezas = 16 unidades en lote
- ✅ Actualización automática de inventario por lotes
- ✅ Generación de pólizas contables con desglose de IVA

### 3. Interfaz Web
- ✅ **CargarXML.cshtml** (214 líneas): Wizard de 3 pasos
  1. **Paso 1**: Cargar archivo XML
  2. **Paso 2**: Verificar datos de factura (proveedor, totales, UUID)
  3. **Paso 3**: Mapear productos y configurar factor de conversión
- ✅ **CargarXML.js** (265 líneas): Lógica cliente con Select2
- ✅ Búsqueda inteligente de productos
- ✅ Cálculo en tiempo real de cantidades finales

### 4. Modelos Extendidos
- ✅ **Compra.cs**: Propiedades UUID, XMLOriginal, EsDesdeXML
- ✅ **ProductoCompraXML**: Mapeo entre XML y sistema
- ✅ **DatosFacturaCompra**: Estructura completa del CFDI
- ✅ **ConceptoFacturaCompra**: Productos con impuestos
- ✅ **ImpuestoConcepto**: Desglose de IVA por tasa

## 🎯 Casos de Uso

### Caso 1: Compra con Desglose Simple
```
XML dice:
  - 2 Cajas de galletas @ $80.00/caja = $160.00

Usuario configura:
  - Factor de conversión: 8 (cada caja tiene 8 paquetes)

Sistema registra:
  - 16 paquetes en inventario
  - Precio unitario: $10.00/paquete
  - 1 lote con 16 unidades disponibles
```

### Caso 2: Compra con Múltiples Productos
```
XML contiene:
  1. 5 Cajas de leche @ $120/caja
  2. 10 Paquetes de pan @ $25/paquete
  3. 3 Costales de harina @ $350/costal

Usuario configura:
  1. Leche: Factor 12 (12 litros/caja) = 60 litros
  2. Pan: Factor 6 (6 piezas/paquete) = 60 piezas
  3. Harina: Factor 20 (20 kg/costal) = 60 kg

Sistema crea:
  - 3 lotes independientes
  - Inventario actualizado con cantidades desglosadas
  - Póliza contable con IVA desglosado
```

### Caso 3: Proveedor Nuevo desde XML
```
XML de proveedor desconocido:
  RFC: ABC123456XYZ
  Nombre: Distribuidora Nueva SA de CV

Sistema automáticamente:
  1. Busca RFC en base de datos
  2. No encuentra → Crea nuevo proveedor
  3. Registra compra asociada al nuevo proveedor
  4. Usuario puede completar datos después
```

## 📊 Estructura de Archivos

### Backend
```
CapaDatos/
├── Utilidades/
│   └── CFDICompraParser.cs (457 líneas)
│       ├── DatosFacturaCompra
│       ├── ConceptoFacturaCompra
│       ├── ImpuestoConcepto
│       ├── ParsearXML()
│       ├── ParsearXMLDesdeTexto()
│       └── ValidarEstructura()
└── CD_Compra.cs (modificado, +153 líneas)
    ├── RegistrarCompraDesdeXML()
    ├── BuscarOCrearProveedor()
    └── GuardarXMLRespaldo()

CapaModelo/
└── Compra.cs (modificado)
    ├── UUID
    ├── XMLOriginal
    └── EsDesdeXML
```

### Frontend
```
VentasWeb/
├── Controllers/
│   └── CompraController.cs (+167 líneas)
│       ├── CargarXML() [GET]
│       ├── ProcesarXML() [POST]
│       ├── RegistrarCompraDesdeXML() [POST]
│       └── BuscarProductoParaMapeo() [GET]
├── Views/Compra/
│   └── CargarXML.cshtml (214 líneas)
└── Scripts/Compra/
    └── CargarXML.js (265 líneas)
```

## 🔄 Flujo de Proceso

### 1. Carga de XML
```
Usuario → Selecciona archivo .xml
       ↓
Controller → ProcesarXML()
       ↓
CFDICompraParser → Valida estructura
       ↓
CFDICompraParser → Extrae datos
       ↓
Vista → Muestra datos (Paso 2)
```

### 2. Mapeo de Productos
```
Por cada concepto XML:
  ↓
  Buscar producto en sistema (Select2)
  ↓
  Configurar factor de conversión
  ↓
  Calcular cantidad final = Cantidad XML × Factor
  ↓
  Calcular precio unitario = Precio XML ÷ Factor
```

### 3. Registro de Compra
```
Usuario → Confirma registro
       ↓
Controller → RegistrarCompraDesdeXML()
       ↓
CD_Compra → RegistrarCompraConLotes()
       ↓
Para cada detalle:
    - Crear lote en LotesProducto
    - Cantidad desglosada
    - Precio unitario desglosado
       ↓
Generar póliza contable
       ↓
Crear cuenta por pagar (si aplica)
       ↓
Guardar XML respaldo
       ↓
Success ✓
```

## 📋 Datos Extraídos del XML

### Comprobante
- ✅ Serie y Folio
- ✅ Fecha de emisión
- ✅ Forma de pago (01=Efectivo, 04=Tarjeta, etc.)
- ✅ Método de pago (PUE/PPD)
- ✅ Moneda y tipo de cambio
- ✅ Subtotal, descuento, total
- ✅ Lugar de expedición (CP)

### Emisor (Proveedor)
- ✅ RFC
- ✅ Razón social / Nombre
- ✅ Régimen fiscal

### Conceptos
- ✅ Clave producto/servicio (SAT)
- ✅ No. Identificación (SKU/código proveedor)
- ✅ Cantidad y unidad
- ✅ Descripción
- ✅ Valor unitario e importe
- ✅ Descuentos
- ✅ **Impuestos trasladados** (IVA 16%, 8%, 0%)
- ✅ **Impuestos retenidos** (ISR, IVA ret.)

### Timbre Fiscal
- ✅ UUID (folio fiscal)
- ✅ Fecha de timbrado
- ✅ Sello digital CFD
- ✅ Sello SAT
- ✅ Certificado SAT

## 💡 Factor de Conversión

### Ejemplos Prácticos

| Unidad XML | Cantidad XML | Factor | Unidad Final | Cantidad Final |
|------------|--------------|--------|--------------|----------------|
| Caja | 5 | 12 | Latas | 60 |
| Paquete | 10 | 6 | Piezas | 60 |
| Costal | 2 | 25 | Kilos | 50 |
| Cartón | 3 | 24 | Botellas | 72 |
| Caja | 1 | 1 | Caja | 1 |

### Cálculos Automáticos
```javascript
Cantidad Final = Cantidad XML × Factor de Conversión
Precio Unitario Final = Precio XML ÷ Factor de Conversión

Ejemplo:
  XML: 2 cajas @ $80.00/caja
  Factor: 8 piezas/caja
  
  Cantidad Final = 2 × 8 = 16 piezas
  Precio Final = $80.00 ÷ 8 = $10.00/pieza
```

## 🔒 Validaciones

### XML
- ✅ Extensión debe ser .xml
- ✅ Estructura CFDI válida
- ✅ Versión 4.0 o 3.3
- ✅ Nodos obligatorios presentes
- ✅ Atributos requeridos completos

### Mapeo
- ✅ Todos los conceptos deben tener producto asignado
- ✅ Factor de conversión > 0
- ✅ Producto existe en sistema
- ✅ No duplicar conceptos

### Registro
- ✅ Proveedor válido o creado
- ✅ UUID único (no duplicar factura)
- ✅ Cantidades > 0
- ✅ Precios > 0
- ✅ Transacción atómica (todo o nada)

## 📈 Ventajas del Sistema

1. **Automatización**: Evita captura manual de 20-50 productos
2. **Precisión**: Datos directos del XML sin errores de transcripción
3. **Trazabilidad**: XML guardado como respaldo
4. **Flexibilidad**: Factor de conversión ajustable por producto
5. **Inventario por lotes**: PEPS automático
6. **Contabilidad**: Pólizas generadas con IVA desglosado
7. **Cumplimiento fiscal**: UUID y datos SAT preservados
8. **Proveedores automáticos**: Crea proveedores desde XML

## 🚀 Rutas y URLs

| Ruta | Método | Descripción |
|------|--------|-------------|
| `/Compra/CargarXML` | GET | Vista principal de carga |
| `/Compra/ProcesarXML` | POST | Procesa archivo XML |
| `/Compra/BuscarProductoParaMapeo` | GET | Búsqueda de productos |
| `/Compra/RegistrarCompraDesdeXML` | POST | Guarda compra final |

## 📁 Respaldos de XML

```
Ruta: ~/App_Data/XMLCompras/
Formato: {UUID}_{yyyyMMddHHmmss}.xml

Ejemplo:
  12345678-ABCD-1234-ABCD-123456789ABC_20260104153045.xml
```

## ⚙️ Configuraciones Necesarias

### Web.config
```xml
<system.web>
    <!-- Permitir carga de archivos grandes -->
    <httpRuntime maxRequestLength="10240" /> <!-- 10MB -->
</system.web>

<system.webServer>
    <security>
        <requestFiltering>
            <requestLimits maxAllowedContentLength="10485760" /> <!-- 10MB -->
        </requestFiltering>
    </security>
</system.webServer>
```

### Permisos de Carpetas
```
~/App_Data/TempXML/     - Escritura (temporal)
~/App_Data/XMLCompras/  - Escritura (respaldos)
```

## 🔧 Mantenimiento

### Limpieza de Archivos Temporales
```sql
-- Script para limpiar XMLs temporales (ejecutar periódicamente)
DELETE FROM TempXML WHERE FechaCreacion < DATEADD(day, -7, GETDATE())
```

### Verificación de Respaldos
```csharp
// Verificar que XMLs respaldados existan
SELECT CompraID, UUID, FechaCompra
FROM Compras
WHERE UUID IS NOT NULL
  AND NOT EXISTS (
      SELECT 1 FROM XMLComprasRespaldo 
      WHERE UUID = Compras.UUID
  )
```

## 🐛 Troubleshooting

### Problema: "El archivo está vacío"
**Solución**: Verificar que el XML no esté corrupto, abrirlo en navegador

### Problema: "Versión de CFDI no soportada"
**Solución**: Solo se aceptan CFDI 3.3 y 4.0

### Problema: "No se encontró mapeo para el concepto"
**Solución**: Asegurarse de asignar todos los productos antes de registrar

### Problema: "Error al crear lote"
**Solución**: Verificar que la tabla LotesProducto tenga columnas correctas

## 📊 Estadísticas de Implementación

- **Líneas de código**: ~1,293 (C#: 777, JavaScript: 265, Razor: 214, SQL: 37)
- **Archivos creados**: 4
- **Archivos modificados**: 4
- **Tiempo estimado de desarrollo**: 6-8 horas
- **Tiempo de captura ahorrado**: 15-30 min por factura

## ✅ Estado del Módulo

**COMPLETADO** ✅

- ✅ Parser CFDI funcional
- ✅ Validación de XML
- ✅ Extracción de datos completa
- ✅ Interfaz de 3 pasos
- ✅ Mapeo de productos con Select2
- ✅ Factor de conversión dinámico
- ✅ Registro por lotes
- ✅ Proveedores automáticos
- ✅ Respaldo de XML
- ✅ Menú integrado
- ✅ Sin errores de compilación

## 🔜 Mejoras Futuras

- [ ] Importación masiva (múltiples XMLs)
- [ ] Mapeo automático por código de producto
- [ ] Historial de XMLs cargados
- [ ] Visualizador de XML integrado
- [ ] Comparación de precios vs compras anteriores
- [ ] Alertas de precios inusuales
- [ ] Exportar mapeos para reutilizar
- [ ] Validación contra catálogo SAT
- [ ] Integración con PAC para consultar UUID

---

**Fecha de implementación**: 4 de enero de 2026  
**Versión**: 1.0.0  
**Desarrollador**: GitHub Copilot  
**Compatibilidad**: CFDI 4.0, CFDI 3.3
