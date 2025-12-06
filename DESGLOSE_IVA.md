# Sistema de Desglose de IVA en Pólizas Contables

## Resumen

Se implementó un sistema automático que desglosa el IVA al crear pólizas contables para ventas y compras, respetando las tasas de IVA definidas en el catálogo de productos.

## Arquitectura

### Modelo de Datos
- **Tabla `MapeoContableIVA`**: Mapea tasas de IVA (0%, 8%, 16%, exento) a cuentas contables:
  - `CuentaDeudora`: Cuenta para débitos (ej. Clientes en ventas, IVA pagado en compras)
  - `CuentaAcreedora`: Cuenta para créditos (ej. IVA cobrado en ventas, Proveedores en compras)

### Modelos
- **`MapeoIVA`** (CapaModelo): Entidad para mapear tasas a cuentas
- **`VentaDetalleCliente`** (CapaModelo): Ahora incluye `TasaIVAPorcentaje` y `Exento`
- **`CompraDetalle`** (CapaModelo): Ahora incluye `TasaIVAPorcentaje` y `Exento`

### Capa de Datos
- **`CD_MapeoIVA`**: Consulta mapeos de IVA por tasa
- **`CD_Venta.RegistrarVentaCredito`**: Crea póliza con desglose de IVA
- **`CD_Compra.RegistrarCompraConLotes`**: Crea póliza con desglose de IVA

## Flujo de Generación de Póliza (Venta)

1. Se calcula para cada línea:
   - Base: `PrecioVenta * Cantidad`
   - IVA: `Base * (TasaIVA / 100)` (si no es exento)

2. Se agrupan líneas por tasa de IVA

3. Se crea una póliza con las siguientes líneas:
   - **Débito**: Clientes/Caja = Total (base + IVA)
   - **Débito**: Costo de Ventas = COGS
   - **Crédito**: Ventas = Base (sin IVA)
   - **Crédito**: IVA Cobrado (por cada tasa) = IVA
   - **Crédito**: Inventario = COGS

## Flujo de Generación de Póliza (Compra)

1. Se calcula para cada línea:
   - Base: `PrecioCompra * Cantidad`
   - IVA: `Base * (TasaIVA / 100)` (si no es exento)

2. Se agrupan líneas por tasa de IVA

3. Se crea una póliza con las siguientes líneas:
   - **Débito**: Inventario = Base (sin IVA)
   - **Débito**: IVA Pagado (por cada tasa) = IVA
   - **Crédito**: Proveedores = Total (base + IVA)

## Configuración

### 1. Crear la tabla en BD
Ejecutar el script SQL incluido:
```
Utilidad/SQL Server/01_CrearTablaMapeoIVA.sql
```

### 2. Configurar mapeos de IVA
Editar la tabla `MapeoContableIVA` para ajustar las cuentas contables según tu catálogo:

```sql
INSERT INTO MapeoContableIVA (TasaIVA, Exento, CuentaDeudora, CuentaAcreedora, Descripcion)
VALUES (16.00, 0, 2052, 2052, 'IVA 16%')
```

### 3. Asegurar que productos tengan TasaIVA
Al dar de alta un producto, especificar:
- `TasaIVAPorcentaje`: 0, 8, 16, etc.
- `Exento`: true/false

### 4. Enviar datos en detalles de venta/compra
La UI debe enviar en cada línea de detalle:
```json
{
  "ProductoID": 123,
  "Cantidad": 5,
  "PrecioVenta": 100,
  "TasaIVAPorcentaje": 16,
  "Exento": false
}
```

## Cuentas por Defecto (ajustables)

| Concepto | CuentaID |
|----------|----------|
| Clientes | 1100 |
| Caja | 1010 |
| Ventas | 4000 |
| Costo de Ventas | 5000 |
| Inventario | 1400 |
| Proveedores | 2100 |
| IVA 0% (Deudor/Acreedor) | 2050 |
| IVA 8% (Deudor/Acreedor) | 2051 |
| IVA 16% (Deudor/Acreedor) | 2052 |
| Exento (Deudor/Acreedor) | 2053 |

## Validaciones

- Si `TasaIVAPorcentaje <= 0` y `Exento = true`, no se suma IVA
- Se agrupa por tasa para evitar líneas duplicadas en la póliza
- Se valida que `Debe = Haber` al crear la póliza

## Ejemplo de Póliza Generada

**Venta a crédito: 2 productos (IVA 16%)**

| Cuenta | Concepto | Debe | Haber |
|--------|----------|------|-------|
| 1100 | Clientes (Venta) | 1,160.00 | |
| 5000 | Costo de Ventas | 500.00 | |
| 4000 | Ingresos por Ventas | | 1,000.00 |
| 2052 | IVA Cobrado 16% | | 160.00 |
| 1400 | Inventario (reducción) | | 500.00 |
| **TOTALES** | | **1,660.00** | **1,660.00** |

## Próximos Pasos (Opcionales)

- [ ] Agregar UI para configurar MapeoContableIVA
- [ ] Soportar retenciones (ISR, IVA retenido)
- [ ] Exportar pólizas a formato contable externo
- [ ] Reportes de IVA cobrado/pagado
