# 📊 Módulo de Gastos Operativos

## 📝 Descripción General

El módulo de gastos permite registrar y controlar todos los egresos operativos del negocio, reflejándolos automáticamente en el cierre de caja para calcular la **ganancia neta real** del día.

## ✅ ¿Qué se implementó?

### 1. Base de Datos
- ✅ **Tabla `CatCategoriasGastos`**: 7 categorías predefinidas (Servicios, Papelería, Limpieza, Mantenimiento, Transporte, Alimentación, Otros)
- ✅ **Tabla `Gastos`**: Registro completo de gastos con aprobación, auditoría y seguimiento
- ✅ **Vista `vw_GastosDetalle`**: Vista consolidada con información completa
- ✅ **4 Stored Procedures**:
  - `sp_RegistrarGasto`: Registro con validaciones automáticas
  - `sp_ObtenerGastosPorFecha`: Consulta de gastos por período
  - `sp_ResumenGastos`: Resumen estadístico por categoría
  - `sp_CierreCajaConGastos`: Cierre de caja con ganancia neta

### 2. Modelos y Lógica de Negocio
- ✅ **Gasto.cs**: Modelo principal con propiedades completas
- ✅ **CategoriaGasto.cs**: Categorías configurables
- ✅ **ResumenGastos.cs**: Resumen por categoría
- ✅ **CierreCajaConGastos.cs**: Cierre con ventas, gastos y ganancia neta
- ✅ **CD_Gasto.cs**: Capa de datos con 7 métodos principales

### 3. Interfaz Web
- ✅ **GastosController.cs**: 7 endpoints RESTful
- ✅ **Registrar.cshtml**: Interfaz para registrar gastos del día
- ✅ **CierreCaja.cshtml**: Reporte de cierre con ganancia neta
- ✅ **JavaScript**: Validaciones, cálculos en tiempo real, SweetAlert2
- ✅ **Menú lateral**: Dropdown de Gastos con 2 opciones

## 🎯 Funcionalidades Clave

### Registro de Gastos
```
- Selección de categoría con validación de montos máximos
- Concepto y descripción detallada
- Monto con forma de pago (efectivo, tarjeta, transferencia)
- Número de factura y proveedor (opcional)
- Observaciones y notas
- Aprobación automática o manual según monto
```

### Cierre de Caja Completo
```
Fórmula de Ganancia Neta:
  Ventas Totales: $10,000.00
  (-) Gastos:     $ 1,500.00
  (-) Retiros:    $   500.00
  = GANANCIA NETA: $ 8,000.00

Efectivo en Caja:
  Ventas Efectivo:   $7,000.00
  (-) Gastos Efectivo: $1,200.00
  (-) Retiros:         $  500.00
  = EFECTIVO CAJA:    $5,300.00
```

### Controles de Aprobación
- Gastos menores al monto máximo: **Aprobación automática**
- Gastos que exceden límite: **Requieren aprobación manual**
- Categoría "Otros Gastos": Siempre requiere aprobación

### Auditoría Completa
- Usuario que registra
- Fecha y hora exacta
- Historial de cambios
- Motivo de cancelación si aplica

## 📂 Estructura de Archivos

### Base de Datos
```
SQL Server/
└── 040_MODULO_GASTOS.sql (414 líneas)
    ├── CatCategoriasGastos (tabla)
    ├── Gastos (tabla con FK a Cajas, FormasPago)
    ├── vw_GastosDetalle (vista)
    ├── sp_RegistrarGasto (SP)
    ├── sp_ObtenerGastosPorFecha (SP)
    ├── sp_ResumenGastos (SP)
    └── sp_CierreCajaConGastos (SP)
```

### Backend
```
CapaModelo/
└── Gasto.cs (86 líneas)
    ├── Gasto (clase principal)
    ├── CategoriaGasto
    ├── ResumenGastos
    ├── CierreCajaConGastos
    └── GastoDetalleCierre

CapaDatos/
└── CD_Gasto.cs (410 líneas)
    ├── RegistrarGasto()
    ├── ObtenerGastosPorFecha()
    ├── ObtenerResumenGastos()
    ├── ObtenerCierreCajaConGastos()
    ├── ObtenerCategoriasGastos()
    ├── CancelarGasto()
    └── ObtenerTotalGastosDia()
```

### Frontend
```
VentasWeb/
├── Controllers/
│   └── GastosController.cs (143 líneas)
│       ├── Registrar (GET/POST)
│       ├── ObtenerGastos
│       ├── ObtenerResumen
│       ├── CancelarGasto
│       ├── CierreCaja (GET)
│       ├── ObtenerCierreCaja
│       └── ObtenerCategorias
├── Views/Gastos/
│   ├── Registrar.cshtml (176 líneas)
│   └── CierreCaja.cshtml (289 líneas)
└── Scripts/Gastos/
    ├── Registrar.js (276 líneas)
    └── CierreCaja.js (179 líneas)
```

## 🔗 Rutas y URLs

| Ruta | Método | Descripción |
|------|--------|-------------|
| `/Gastos/Registrar` | GET | Formulario de registro |
| `/Gastos/RegistrarGasto` | POST | Guarda nuevo gasto |
| `/Gastos/ObtenerGastos` | GET | Lista gastos por fecha |
| `/Gastos/ObtenerResumen` | GET | Resumen por categoría |
| `/Gastos/CancelarGasto` | POST | Cancela un gasto |
| `/Gastos/CierreCaja` | GET | Vista de cierre |
| `/Gastos/ObtenerCierreCaja` | GET | Datos del cierre |

## 📊 Categorías Predefinidas

| ID | Categoría | Monto Máximo | Aprobación |
|----|-----------|--------------|------------|
| 1 | Servicios | Sin límite | Manual |
| 2 | Papelería | $500.00 | Automática |
| 3 | Limpieza | $500.00 | Automática |
| 4 | Mantenimiento | $2,000.00 | Automática |
| 5 | Transporte | $1,000.00 | Automática |
| 6 | Alimentación | $300.00 | Automática |
| 7 | Otros Gastos | $1,000.00 | **Siempre Manual** |

## 🎨 Interfaz de Usuario

### Pantalla de Registro
- **Columna izquierda**: Formulario de registro con validaciones
- **Columna derecha**: Lista de gastos del día con totales
- **Panel inferior**: Resumen por categoría con info-boxes
- **Alertas**: Monto máximo excedido muestra advertencia

### Pantalla de Cierre de Caja
- **4 Info-boxes superiores**: Ventas, Gastos, Efectivo, Ganancia Neta
- **Desglose de ventas**: Por forma de pago
- **Detalle de gastos**: Tabla con categorías y montos
- **Resumen final**: Fórmulas de ganancia neta y efectivo

## 🔒 Seguridad y Validaciones

### Validaciones de Backend
- ✅ Categoría debe existir y estar activa
- ✅ Monto debe ser mayor a 0
- ✅ Concepto es obligatorio
- ✅ Usuario registrador es obligatorio
- ✅ Validación de monto máximo por categoría

### Validaciones de Frontend
- ✅ Campos obligatorios marcados
- ✅ Validación de formato de monto (decimal)
- ✅ Alerta visual si excede monto máximo
- ✅ Confirmación antes de cancelar
- ✅ Motivo obligatorio al cancelar

### Control de Acceso
- Solo usuarios con rol **ADMINISTRADOR** o **EMPLEADO** pueden:
  - Registrar gastos
  - Ver gastos del día
  - Consultar cierre de caja
- Solo **ADMINISTRADOR** puede:
  - Aprobar gastos pendientes
  - Cancelar gastos

## 📈 Casos de Uso

### Caso 1: Registro de Gasto Simple
```
1. Empleado va a Gastos → Registrar
2. Selecciona "Limpieza"
3. Concepto: "Detergente y cloro"
4. Monto: $250.00
5. Forma de Pago: Efectivo
6. Click en "Registrar Gasto"
7. Sistema aprueba automáticamente (< $500)
8. Gasto se refleja en lista del día
```

### Caso 2: Gasto que Requiere Aprobación
```
1. Empleado registra gasto de "Mantenimiento"
2. Monto: $3,500.00 (excede $2,000)
3. Sistema marca como "Pendiente Aprobación"
4. Administrador revisa y aprueba
5. Gasto se refleja en cierre de caja
```

### Caso 3: Cierre de Caja del Día
```
1. Cajero va a Gastos → Cierre de Caja
2. Selecciona caja y fecha (hoy)
3. Click en "Consultar Cierre"
4. Sistema muestra:
   - Total ventas: $15,230.00
   - Total gastos: $2,150.00
   - Ganancia neta: $13,080.00
   - Efectivo esperado: $8,450.00
5. Click en "Imprimir" para reporte físico
```

## 🔄 Flujo de Datos

```
Registro de Gasto
    ↓
CD_Gasto.RegistrarGasto()
    ↓
sp_RegistrarGasto (SQL)
    ↓
Validar categoría y monto
    ↓
Insertar en tabla Gastos
    ↓
Devolver GastoID

Cierre de Caja
    ↓
CD_Gasto.ObtenerCierreCajaConGastos()
    ↓
sp_CierreCajaConGastos (SQL)
    ↓
Calcular: Ventas - Gastos - Retiros
    ↓
Devolver CierreCajaConGastos
```

## 💡 Ventajas del Módulo

1. **Transparencia financiera**: Registro detallado de todos los egresos
2. **Control de gastos**: Límites por categoría con aprobaciones
3. **Ganancia real**: Cálculo automático de utilidad neta del día
4. **Auditoría completa**: Quién, cuándo, cuánto y por qué
5. **Reportes instantáneos**: Cierre de caja en tiempo real
6. **Trazabilidad**: Cada gasto asociado a usuario y caja
7. **Prevención de fraude**: Aprobaciones obligatorias en montos altos

## 📝 Notas Importantes

1. **Gastos y Caja**: Los gastos se asocian a la caja activa en sesión
2. **Fechas**: Todos los gastos se registran con fecha/hora exacta
3. **Cancelaciones**: Un gasto cancelado NO se elimina, se marca como cancelado
4. **Aprobaciones**: Los gastos aprobados no pueden modificarse
5. **Cierre de caja**: Muestra solo gastos NO cancelados
6. **Formas de pago**: Un gasto en efectivo reduce el efectivo en caja
7. **Retiros**: Actualmente en $0.00 (pendiente de implementar)

## 🚀 Próximas Mejoras

- [ ] Módulo de retiros de caja
- [ ] Aprobación de gastos pendientes (interfaz)
- [ ] Alertas por correo cuando gasto requiere aprobación
- [ ] Reporte mensual de gastos por categoría
- [ ] Comparativo de gastos mes a mes
- [ ] Exportar cierre de caja a PDF
- [ ] Gráficas de gastos por categoría
- [ ] Presupuesto mensual por categoría
- [ ] Alertas de límite de presupuesto

## ✅ Estado del Módulo

**COMPLETADO** ✅

- ✅ Base de datos instalada
- ✅ 7 categorías creadas
- ✅ Modelos C# creados
- ✅ Capa de datos implementada
- ✅ Controlador web funcional
- ✅ Vistas responsive creadas
- ✅ JavaScript con validaciones
- ✅ Menú integrado en navegación
- ✅ Sin errores de compilación

## 📚 Documentación Relacionada

- [Manual de Usuario - Registro de Gastos](pendiente)
- [Guía de Cierre de Caja](pendiente)
- [API Reference - GastosController](pendiente)
- [Configuración de Categorías](pendiente)

---

**Fecha de implementación**: 4 de enero de 2026  
**Versión**: 1.0.0  
**Autor**: GitHub Copilot  
**Líneas de código**: ~1,900 (incluyendo SQL, C#, Razor, JavaScript)
