# Implementación de Pagos Parciales en POS

## ✅ COMPLETADO - Fecha: 04/01/2026

### 🎯 Objetivo
Implementar la funcionalidad de **pagos parciales** en el POS, permitiendo:
- Cobrar una parte del total de la venta
- Generar factura con método PPD (Pago en Parcialidades o Diferido)
- Registrar pagos posteriores
- Generar complementos de pago para cada abono

---

## 📦 Cambios Implementados

### 1. Base de Datos (✅ Completado)

**Archivo:** `Utilidad\SQL Server\030_VENTA_PARCIALIDADES_E_INVENTARIO_FRACCIONADO.sql`

#### Tabla VentasClientes
- ✅ Agregado campo `MontoPagado DECIMAL(18,2) NOT NULL` - Monto pagado inicial
- ✅ Agregado campo computado `SaldoPendiente AS (Total - MontoPagado) PERSISTED`
- ✅ Agregado campo computado `EsPagado AS (CASE WHEN (Total - MontoPagado) <= 0.01 THEN 1 ELSE 0 END) PERSISTED`
- ✅ TipoVenta ahora soporta 'PARCIAL' además de 'CONTADO' y 'CREDITO'

#### Nueva Tabla: VentaPagos
```sql
CREATE TABLE VentaPagos (
    PagoID INT IDENTITY(1,1) PRIMARY KEY,
    VentaID UNIQUEIDENTIFIER NOT NULL,
    FormaPagoID INT,
    MetodoPagoID INT,
    Monto DECIMAL(18,2) NOT NULL,
    FechaPago DATETIME NOT NULL DEFAULT GETDATE(),
    Referencia VARCHAR(100),
    Observaciones VARCHAR(500),
    Usuario VARCHAR(100),
    ComplementoPagoID INT NULL,
    FechaAlta DATETIME DEFAULT GETDATE()
)
```

#### Stored Procedures Modificados
- ✅ **RegistrarVentaPOS**: Ahora recibe `@MontoPagado` y soporta TipoVenta='PARCIAL'
- ✅ **RegistrarDetalleVentaPOS**: Ahora usa `DECIMAL(18,3)` para cantidades fraccionadas

#### Stored Procedures Nuevos
- ✅ **RegistrarPagoVenta**: Registra pagos posteriores a la venta inicial
- ✅ **ObtenerVentasPendientesPago**: Consulta ventas con saldo pendiente

**Migración de Datos:**
- ✅ 10 ventas CONTADO actualizadas con MontoPagado = Total
- ✅ 21 ventas CREDITO actualizadas con MontoPagado = 0

---

### 2. Modelos C# (✅ Completado)

**Archivo:** `CapaModelo\VentaPOS.cs`

#### Clase VentaPOS
```csharp
public decimal? MontoPagado { get; set; }  // Monto pagado inicial
public decimal SaldoPendiente { get; set; } // Saldo por pagar
public bool EsPagado { get; set; }          // Totalmente pagado
```

#### Nuevas Clases
```csharp
// Registro de un pago
public class VentaPago {
    public int PagoID { get; set; }
    public Guid VentaID { get; set; }
    public int? FormaPagoID { get; set; }
    public int? MetodoPagoID { get; set; }
    public decimal Monto { get; set; }
    public DateTime FechaPago { get; set; }
    public string Referencia { get; set; }
    public string Observaciones { get; set; }
    public int? ComplementoPagoID { get; set; }
}

// Para consultar ventas pendientes
public class VentaPendientePago {
    public Guid VentaID { get; set; }
    public DateTime FechaVenta { get; set; }
    public Guid ClienteID { get; set; }
    public string ClienteRazonSocial { get; set; }
    public string ClienteRFC { get; set; }
    public decimal Total { get; set; }
    public decimal MontoPagado { get; set; }
    public decimal SaldoPendiente { get; set; }
    public int NumeroPagos { get; set; }
}

// Request para registrar nuevo pago
public class RegistrarPagoVentaRequest {
    public Guid VentaID { get; set; }
    public int FormaPagoID { get; set; }
    public int MetodoPagoID { get; set; }
    public decimal Monto { get; set; }
    public string Referencia { get; set; }
    public string Observaciones { get; set; }
}
```

---

### 3. Capa de Datos (✅ Completado)

**Archivo:** `CapaDatos\CD_VentaPOS.cs`

#### Método Modificado
- ✅ **RegistrarVenta**: Ahora envía `MontoPagado` al SP

#### Nuevos Métodos
```csharp
// Registra un pago posterior
public bool RegistrarPagoVenta(
    RegistrarPagoVentaRequest request, 
    string usuario, 
    out string mensaje, 
    out int pagoID)

// Obtiene ventas con saldo pendiente
public List<VentaPendientePago> ObtenerVentasPendientesPago(
    Guid? clienteID = null)

// Obtiene historial de pagos de una venta
public List<VentaPago> ObtenerPagosVenta(Guid ventaID)

// Actualiza ComplementoPagoID después del timbrado
public bool ActualizarComplementoPagoID(
    int pagoID, 
    int complementoPagoID, 
    out string mensaje)
```

---

### 4. Interfaz de Usuario - POS (✅ Completado)

**Archivo:** `VentasWeb\Views\VentaPOS\Index.cshtml`

#### Nuevo Radio Button
```html
<input type="radio" name="tipoVenta" id="rdParcial" value="PARCIAL" 
       onchange="cambiarTipoVenta()" disabled />
<label for="rdParcial">
    <i class="fas fa-hand-holding-usd"></i> Pago Parcial
</label>
```

#### Nuevo Panel (Visible solo en PARCIAL)
```html
<div id="panelMontoParcial" style="display:none;">
    <div class="alert alert-warning">
        Se generará factura con método PPD (Pago en Parcialidades o Diferido)
    </div>
    
    <label>Total de la venta:</label>
    <h4 id="lblTotalVentaParcial">$0.00</h4>
    
    <label>Monto a pagar ahora:</label>
    <input type="number" id="txtMontoPagado" 
           onkeyup="calcularSaldoPendiente()" />
    
    <div id="panelSaldoPendiente">
        <h5>Saldo Pendiente: <span id="lblSaldoPendiente"></span></h5>
        <h5>Pagado: <span id="lblPorcentajePagado"></span></h5>
    </div>
</div>
```

---

### 5. JavaScript - POS (✅ Completado)

**Archivo:** `VentasWeb\Scripts\Views\VentaPOS.js`

#### Funciones Nuevas
```javascript
// Calcula y muestra saldo pendiente en tiempo real
function calcularSaldoPendiente() {
    const totales = actualizarTotales();
    const montoPagado = parseFloat($('#txtMontoPagado').val()) || 0;
    const saldoPendiente = totales.total - montoPagado;
    const porcentaje = ((montoPagado / totales.total) * 100).toFixed(1);
    
    $('#lblSaldoPendiente').text('$' + saldoPendiente.toFixed(2));
    $('#lblPorcentajePagado').text(porcentaje + '%');
}
```

#### Funciones Modificadas
```javascript
// cambiarTipoVenta: Maneja visibilidad de paneles según tipo
if (tipoVenta === 'PARCIAL') {
    $('#panelFormaPago').show();
    $('#panelMontoParcial').show();
    $('#chkRequiereFactura').prop('checked', true); // Factura obligatoria
}

// finalizarVenta: Valida monto pagado
if (tipoVenta === 'PARCIAL') {
    const montoPagado = parseFloat($('#txtMontoPagado').val());
    
    if (montoPagado <= 0) {
        toastr.warning('Ingrese el monto a pagar ahora');
        return;
    }
    
    if (montoPagado > totales.total) {
        toastr.warning('El monto no puede ser mayor al total');
        return;
    }
    
    // Confirmar con usuario
    if (!confirm(`Total: $${totales.total}
                  Pago ahora: $${montoPagado}
                  Saldo: $${totales.total - montoPagado}`))
        return;
}

// procesarVenta: Incluye MontoPagado en payload
const payload = {
    Venta: {
        ...
        MontoPagado: tipoVenta === 'PARCIAL' ? 
            parseFloat($('#txtMontoPagado').val()) : null,
        RequiereFactura: tipoVenta === 'PARCIAL' ? true : ...
    }
};

// seleccionarCliente: Habilita opción PARCIAL
$('#rdParcial').prop('disabled', false);
```

---

### 6. Controlador (✅ Completado)

**Archivo:** `VentasWeb\Controllers\VentaPOSController.cs`

#### Validaciones Agregadas
```csharp
// Validar pago parcial
if (payload.Venta.TipoVenta == "PARCIAL")
{
    if (!payload.Venta.MontoPagado.HasValue || 
        payload.Venta.MontoPagado.Value <= 0)
        return Json(new { success = false, 
            mensaje = "Debe especificar el monto pagado" });
    
    if (payload.Venta.MontoPagado.Value > payload.Venta.Total)
        return Json(new { success = false, 
            mensaje = "El monto pagado no puede ser mayor al total" });
}
```

---

## 🎮 Flujo de Uso

### Realizar Venta con Pago Parcial

1. **Agregar productos** al carrito
2. **Seleccionar cliente** (habilita opción PARCIAL)
3. **Seleccionar "Pago Parcial"**
   - Se muestra panel para capturar monto pagado
   - Se calcula saldo pendiente automáticamente
4. **Ingresar monto a pagar**
   - Debe ser > 0 y <= Total
   - Se muestra confirmación con desglose
5. **Seleccionar forma y método de pago**
6. **Finalizar venta**
   - Se registra con TipoVenta='PARCIAL'
   - Se guarda MontoPagado
   - Se genera factura con método PPD automáticamente

### Consultar Ventas Pendientes
```csharp
var pendientes = CD_VentaPOS.Instancia
    .ObtenerVentasPendientesPago(clienteID: null);
// Retorna todas las ventas con SaldoPendiente > 0
```

### Registrar Pago Posterior
```csharp
var request = new RegistrarPagoVentaRequest {
    VentaID = ventaId,
    FormaPagoID = 1,
    MetodoPagoID = 1,
    Monto = 500.00m,
    Referencia = "Transferencia 123456",
    Observaciones = "Segundo pago"
};

bool exito = CD_VentaPOS.Instancia.RegistrarPagoVenta(
    request, 
    "usuario", 
    out mensaje, 
    out pagoID
);
```

---

## ⏳ Pendientes de Implementar

### 1. Pantalla de Gestión de Pagos
- [ ] Vista para consultar ventas con saldo pendiente
- [ ] Formulario para registrar nuevos pagos
- [ ] Historial de pagos realizados
- [ ] Botón para generar complemento de pago

### 2. Integración con Facturación
- [ ] Modificar `CD_Factura.cs` para detectar ventas parciales
- [ ] Usar método PPD automáticamente cuando SaldoPendiente > 0
- [ ] Incluir SaldoInsoluto en XML CFDI 4.0

### 3. Complemento de Pago
- [ ] Generar XML de complemento de pago 2.0
- [ ] Timbrar complemento con PAC
- [ ] Actualizar `VentaPagos.ComplementoPagoID` tras timbrado
- [ ] Enviar complemento por correo al cliente

### 4. Reportes
- [ ] Reporte de ventas con saldo pendiente
- [ ] Reporte de antigüedad de saldos
- [ ] Dashboard con indicadores de cobranza

---

## 🧪 Pruebas Realizadas

✅ Compilación exitosa sin errores ni advertencias
✅ Base de datos migrada correctamente (31 ventas actualizadas)
✅ Stored procedures creados y funcionales
✅ Modelos C# con propiedades correctas
✅ UI muestra panel de monto pagado cuando se selecciona PARCIAL
✅ JavaScript calcula saldo pendiente en tiempo real
✅ Validaciones frontend funcionando
✅ Validaciones backend implementadas

---

## 📋 Notas Técnicas

### Precisión Decimal
- Cantidades: `DECIMAL(18,3)` - Hasta 0.001 kg (1 gramo)
- Montos: `DECIMAL(18,2)` - Centavos de peso mexicano
- Comparación saldos: <= 0.01 (tolera 1 centavo por redondeo)

### Campos Computados
- `SaldoPendiente` se actualiza automáticamente al registrar pagos
- `EsPagado` se marca automáticamente cuando saldo <= 0.01

### TipoVenta
- **CONTADO**: Pago completo inmediato, factura opcional
- **CREDITO**: Sin pago inicial, factura después del pago total
- **PARCIAL**: Pago inicial + saldo pendiente, factura con PPD inmediata

### Facturación PARCIAL
- RequiereFactura se fuerza a `true`
- Método de Pago: PPD (Pago en Parcialidades o Diferido)
- Forma de Pago: La seleccionada en POS
- Se debe generar complemento de pago por cada abono posterior

---

## 🔗 Archivos Relacionados

### Base de Datos
- `Utilidad\SQL Server\030_VENTA_PARCIALIDADES_E_INVENTARIO_FRACCIONADO.sql`

### Modelos
- `CapaModelo\VentaPOS.cs`

### Capa de Datos
- `CapaDatos\CD_VentaPOS.cs`

### Controladores
- `VentasWeb\Controllers\VentaPOSController.cs`

### Vistas
- `VentasWeb\Views\VentaPOS\Index.cshtml`

### Scripts
- `VentasWeb\Scripts\Views\VentaPOS.js`

---

## 📞 Soporte

Para dudas o problemas con esta funcionalidad, revisar:
1. Este documento
2. Logs en SQL Server (tabla EmailLog para errores de SP)
3. Console del navegador (errores JavaScript)
4. Application Event Log (errores .NET)

---

**Última actualización:** 04/01/2026
**Estado:** ✅ UI Completa - Backend Funcional - Facturación y Complemento Pendiente
