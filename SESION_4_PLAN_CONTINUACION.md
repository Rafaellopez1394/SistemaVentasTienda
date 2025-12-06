# 🚀 Sesión 4: Plan de Continuación - Tipos de Crédito UI & Integración

## 📌 Punto de Partida (Estado Actual)

**Sesión 3 Completada:**
- ✅ Modelos: TipoCredito.cs (3 clases)
- ✅ Data Layer: CD_TipoCredito.cs (8 métodos)
- ✅ Controller: CreditoController.cs (7 acciones)
- ✅ SQL: 04_TiposCredito_Init.sql (tablas + datos)
- ✅ Compilación: 0 Errores (CapaModelo + CapaDatos)

**Completitud:** 60% de Tipos de Crédito

**Pendiente:** 40% (UI + Integración)

---

## 🎯 Objetivo de Sesión 4

**Completar 100% de Tipos de Crédito** implementando:
1. Vistas HTML/MVC
2. Scripts AJAX
3. Integración en VentaController
4. Pruebas integrales

**Tiempo estimado:** 3-4 horas
**Resultado:** Sistema de tipos de crédito 100% funcional

---

## 📋 PLAN DETALLADO (4 Tareas)

### ✅ TAREA 1: Crear Vista Index.cshtml (30 min)
**Archivo:** `VentasWeb/Views/Credito/Index.cshtml`
**Propósito:** Listado de tipos de crédito disponibles

#### Contenido esperado:
```html
<!-- Header -->
<h2>Tipos de Crédito</h2>

<!-- Botón Nuevo -->
<button class="btn btn-primary">Nuevo Tipo</button>

<!-- Tabla con tipos -->
<table class="table table-striped" id="tblTiposCredito">
    <thead>
        <tr>
            <th>Código</th>
            <th>Nombre</th>
            <th>Descripción</th>
            <th>Criterio</th>
            <th>Estado</th>
            <th>Acciones</th>
        </tr>
    </thead>
    <tbody>
        @foreach(var tipo in Model)
        {
            <tr>
                <td>@tipo.Codigo</td>
                <td>@tipo.Nombre</td>
                <td>@tipo.Descripcion</td>
                <td><span class="badge">@tipo.Criterio</span></td>
                <td>@(tipo.Activo ? "Activo" : "Inactivo")</td>
                <td>
                    <button onclick="editarTipo(@tipo.TipoCreditoID)">Editar</button>
                    <button onclick="eliminarTipo(@tipo.TipoCreditoID)">Eliminar</button>
                </td>
            </tr>
        }
    </tbody>
</table>
```

#### Características:
- [ ] Tabla con datos paginados
- [ ] Botón "Nuevo Tipo"
- [ ] Botones Editar/Eliminar
- [ ] Badges para Criterio
- [ ] Indicador de estado Activo/Inactivo
- [ ] DataTables para ordenar/filtrar

#### Referencias:
- Ver: `VentasWeb/Views/Cliente/Index.cshtml` (estructura similar)
- Usar: Bootstrap 4.6.0 (ya en proyecto)
- Script: `Views/Shared/_Layout.cshtml` para librerías

---

### ✅ TAREA 2: Crear Script Credito.js (45 min)
**Archivo:** `VentasWeb/Scripts/Views/Credito.js`
**Propósito:** Funciones AJAX para gestión de créditos

#### Funciones esperadas:

```javascript
// 1. Cargar créditos del cliente
function cargarCreditosCliente(clienteId) {
    $.ajax({
        url: '/Credito/ObtenerCreditosCliente',
        data: { clienteId: clienteId },
        type: 'GET',
        success: function(data) {
            // Mostrar tabla de créditos
            mostrarCreditosEnTabla(data);
        }
    });
}

// 2. Mostrar resumen en modal
function mostrarResumenCredito(clienteId) {
    $.ajax({
        url: '/Credito/ObtenerResumenCredito',
        data: { clienteId: clienteId },
        type: 'GET',
        success: function(res) {
            if (res.success) {
                $('#lblLimiteTotal').text(formatoCurrency(res.data.limiteDineroTotal));
                $('#lblSaldoUtilizado').text(formatoCurrency(res.data.saldoDineroUtilizado));
                $('#lblSaldoDisponible').text(formatoCurrency(res.data.saldoDineroDisponible));
                $('#lblEstado').text(res.data.estado);
                $('#modalResumenCredito').modal('show');
            }
        }
    });
}

// 3. Asignar nuevo crédito a cliente
function asignarCreditoAjax(clienteId, tipoCreditoId, limiteDinero) {
    $.ajax({
        url: '/Credito/AsignarCredito',
        data: {
            clienteId: clienteId,
            tipoCreditoId: tipoCreditoId,
            limiteDinero: limiteDinero
        },
        type: 'POST',
        success: function(res) {
            if (res.success) {
                alert('Crédito asignado');
                // Recargar créditos
                cargarCreditosCliente(clienteId);
            } else {
                alert('Error: ' + res.error);
            }
        }
    });
}

// 4. Suspender/Reactivar crédito
function suspenderCreditoAjax(clienteTipoCreditoId, suspender) {
    $.ajax({
        url: '/Credito/SuspenderCredito',
        data: {
            clienteTipoCreditoId: clienteTipoCreditoId,
            suspender: suspender
        },
        type: 'POST',
        success: function(res) {
            if (res.success) {
                alert(res.message);
                location.reload();
            }
        }
    });
}

// 5. Validar crédito (pre-venta)
function validarCreditoParaVenta(clienteId, tipoCreditoId, monto) {
    $.ajax({
        url: '/Credito/ValidarCredito',
        data: {
            clienteId: clienteId,
            tipoCreditoId: tipoCreditoId,
            montoSolicitado: monto
        },
        type: 'POST',
        success: function(res) {
            if (res.success) {
                // Permitir venta
                window.puedeVender = true;
            } else {
                alert('No hay crédito disponible: ' + res.error);
                window.puedeVender = false;
            }
        }
    });
}

// Función auxiliar
function formatoCurrency(valor) {
    return '$' + valor.toLocaleString('es-CO', {
        minimumFractionDigits: 2,
        maximumFractionDigits: 2
    });
}
```

#### Ubicación:
```
VentasWeb/Scripts/Views/Credito.js
```

#### Referencias a crear/vincular:
- [ ] Incluir en layout o view
- [ ] Usar jQuery (ya en proyecto)
- [ ] Usar Bootstrap (ya en proyecto)

---

### ✅ TAREA 3: Crear Vista AsignarCliente.cshtml (45 min)
**Archivo:** `VentasWeb/Views/Credito/AsignarCliente.cshtml`
**Propósito:** Modal/Form para asignar crédito a cliente

#### Contenido esperado:

```html
@model Guid <!-- ClienteID -->

<div class="modal fade" id="modalAsignarCredito">
    <div class="modal-dialog">
        <div class="modal-content">
            <div class="modal-header">
                <h5>Asignar Crédito a Cliente</h5>
                <button type="button" class="close" data-dismiss="modal">&times;</button>
            </div>
            
            <div class="modal-body">
                <!-- Seleccionar Cliente -->
                <div class="form-group">
                    <label>Cliente</label>
                    <select id="ddlCliente" class="form-control">
                        <option value="">-- Seleccionar --</option>
                        @* Cargar desde AJAX *@
                    </select>
                </div>
                
                <!-- Seleccionar Tipo de Crédito -->
                <div class="form-group">
                    <label>Tipo de Crédito</label>
                    <select id="ddlTipoCredito" class="form-control" onchange="cambioTipoCredito()">
                        <option value="">-- Seleccionar --</option>
                        @* Cargar desde CreditoController.ObtenerTodos() *@
                    </select>
                </div>
                
                <!-- Límite Dinero (si Criterio = Dinero) -->
                <div class="form-group" id="divLimiteDinero" style="display:none;">
                    <label>Límite en Pesos</label>
                    <input type="number" id="txtLimiteDinero" class="form-control" 
                           placeholder="10000" min="1000" step="100">
                </div>
                
                <!-- Límite Producto (si Criterio = Producto) -->
                <div class="form-group" id="divLimiteProducto" style="display:none;">
                    <label>Límite en Unidades</label>
                    <input type="number" id="txtLimiteProducto" class="form-control" 
                           placeholder="100" min="1" step="1">
                </div>
                
                <!-- Plazo Días (si Criterio = Tiempo) -->
                <div class="form-group" id="divPlazoDias" style="display:none;">
                    <label>Plazo en Días</label>
                    <input type="number" id="txtPlazoDias" class="form-control" 
                           placeholder="30" min="1" max="365" step="1">
                </div>
            </div>
            
            <div class="modal-footer">
                <button type="button" class="btn btn-secondary" data-dismiss="modal">Cancelar</button>
                <button type="button" class="btn btn-primary" onclick="guardarAsignacion()">Asignar</button>
            </div>
        </div>
    </div>
</div>

<script>
function cambioTipoCredito() {
    var tipoCreditoId = $('#ddlTipoCredito').val();
    // Obtener criterio desde AJAX
    $.ajax({
        url: '/Credito/ObtenerPorId',
        data: { id: tipoCreditoId },
        type: 'GET',
        success: function(data) {
            // Mostrar campos según criterio
            $('#divLimiteDinero').hide();
            $('#divLimiteProducto').hide();
            $('#divPlazoDias').hide();
            
            if (data.criterio === 'Dinero') $('#divLimiteDinero').show();
            else if (data.criterio === 'Producto') $('#divLimiteProducto').show();
            else if (data.criterio === 'Tiempo') $('#divPlazoDias').show();
        }
    });
}

function guardarAsignacion() {
    var clienteId = $('#ddlCliente').val();
    var tipoCreditoId = $('#ddlTipoCredito').val();
    var limiteDinero = $('#txtLimiteDinero').val() || null;
    var limiteProducto = $('#txtLimiteProducto').val() || null;
    var plazoDias = $('#txtPlazoDias').val() || null;
    
    asignarCreditoAjax(clienteId, tipoCreditoId, limiteDinero);
    $('#modalAsignarCredito').modal('hide');
}
</script>
```

#### Características:
- [ ] Seleccionar Cliente (dropdown)
- [ ] Seleccionar Tipo de Crédito (dropdown)
- [ ] Mostrar campo de límite según criterio
- [ ] Validación de campos
- [ ] Botones Cancelar/Asignar
- [ ] Comunicación AJAX

---

### ✅ TAREA 4: Integración en VentaController (30 min)
**Archivo:** `VentasWeb/Controllers/VentaController.cs`
**Propósito:** Validar crédito antes de crear venta

#### Cambios esperados en método Crear():

```csharp
[HttpPost]
public ActionResult Crear(VentaCreateViewModel model)
{
    try
    {
        // 1. Validar modelo
        if (!ModelState.IsValid)
            return Json(new { success = false, error = "Datos inválidos" });
        
        // 2. NUEVO: Validar crédito si es a crédito
        if (model.EsCredito && model.ClienteID.HasValue)
        {
            // Determinar tipo de crédito (por ahora: Dinero)
            int tipoCreditoId = 1; // CR001 - Crédito por Dinero
            
            // Validar
            bool puedeComprar = CD_TipoCredito.Instancia.PuedoUsarCredito(
                model.ClienteID.Value, 
                tipoCreditoId, 
                model.Total
            );
            
            if (!puedeComprar)
                return Json(new { 
                    success = false, 
                    error = "Cliente no tiene crédito disponible para este monto" 
                });
        }
        
        // 3. Crear venta (código existente)
        Venta venta = new Venta
        {
            VentaID = Guid.NewGuid(),
            ClienteID = model.ClienteID,
            Total = model.Total,
            EsCredito = model.EsCredito,
            // ... otros campos
        };
        
        bool resultado = CD_Venta.Instancia.Crear(venta, model.Detalles);
        
        if (!resultado)
            return Json(new { success = false, error = "Error al crear venta" });
        
        // 4. NUEVO: Si fue a crédito, registrar en ClienteTiposCredito
        if (model.EsCredito && model.ClienteID.HasValue)
        {
            // Nota: El saldo se actualiza en CD_Cliente.ObtenerSaldoActual()
            // porque suma automáticamente VentasCredito pendientes
            // No necesitamos update adicional aquí
        }
        
        return Json(new { success = true, ventaId = venta.VentaID });
    }
    catch (Exception ex)
    {
        return Json(new { success = false, error = ex.Message });
    }
}
```

#### Ubicación de cambios:
```
VentasWeb/Controllers/VentaController.cs
Método: Crear()
```

#### Pasos:
- [ ] Importar using CapaDatos
- [ ] Antes de crear venta: PuedoUsarCredito()
- [ ] Si false: retornar error JSON
- [ ] Si true: continuar creación normal
- [ ] Nada especial para saldo (se calcula en CD_Cliente.ObtenerSaldoActual())

---

## 🧪 TESTING (1 hora)

### Test 1: Verificar Tipos de Crédito Maestros
```sql
SELECT * FROM TiposCredito;
-- Verificar: 3 registros (CR001, CR002, CR003)
```

### Test 2: Asignar Crédito a Cliente
```
1. Abrir http://localhost/Cliente/
2. Seleccionar un cliente
3. En el panel de crédito, seleccionar tipo CR001 (Dinero)
4. Asignar límite: $10,000
5. Guardar
6. Verificar en BD:
   SELECT * FROM ClienteTiposCredito WHERE ClienteID = ...
```

### Test 3: Ver Resumen de Crédito
```
1. Abrir cliente nuevamente
2. En modal de edición, debe mostrarse resumen
3. Límite: $10,000
4. Saldo disponible: $10,000 (no hay ventas aún)
5. Estado: NORMAL
```

### Test 4: Crear Venta a Crédito (Bloqueada si no hay crédito)
```
1. Abrir Venta/Crear
2. Seleccionar cliente con crédito asignado
3. Seleccionar "Es a Crédito"
4. Ingresar monto: $5,000
5. Guardar
6. Verificar:
   - Se crea venta
   - Saldo disponible baja a $5,000
   - Estado sigue en NORMAL
```

### Test 5: Llenar Crédito (Estado ALERTA)
```
1. Crear otra venta a crédito por $9,000 (total $14,000)
2. Verificar:
   - Error: No hay crédito disponible (límite es $10,000)
   - O: Venta rechazada
```

### Test 6: Suspender Crédito
```
1. Ir a Credito/Index
2. Seleccionar cliente con crédito
3. Botón "Suspender"
4. Intentar crear venta: Debe fallar
5. Reactivar crédito
6. Venta debe funcionar de nuevo
```

### Test 7: Crédito a Plazo (Tiempo)
```
1. Asignar crédito CR003 (Tiempo) por 30 días
2. Verificar BD:
   FechaAsignacion = HOY
   FechaVencimiento = HOY + 30 días
3. Crear venta a crédito: OK
4. Esperar a que venza (UPDATE para test):
   UPDATE ClienteTiposCredito SET FechaVencimiento = GETDATE() - 1
5. Intenta crear venta: Debe fallar (vencido)
```

---

## 📂 ESTRUCTURA FINAL POST SESIÓN 4

```
VentasWeb/
├── Views/
│   ├── Credito/
│   │   ├── Index.cshtml ................... ✅ NUEVO
│   │   ├── AsignarCliente.cshtml .......... ✅ NUEVO
│   │   └── ResumenCliente.cshtml ......... (opcional)
│   └── [otras vistas]
├── Scripts/
│   └── Views/
│       ├── Cliente.js
│       └── Credito.js ..................... ✅ NUEVO
└── Controllers/
    ├── CreditoController.cs
    └── VentaController.cs ................ ✅ MODIFICADO
```

---

## ✅ COMPLETITUD POST SESIÓN 4

| Componente | Antes | Después | % |
|-----------|-------|---------|---|
| Modelos | ✅ | ✅ | 100% |
| Data Layer | ✅ | ✅ | 100% |
| Controller | ✅ | ✅ | 100% |
| Vistas | ❌ | ✅ | 100% |
| Scripts | ❌ | ✅ | 100% |
| Integración | ❌ | ✅ | 100% |
| **TOTAL** | **60%** | **100%** | **100%** |

---

## 📝 Checklist para Sesión 4

### Preparación (Antes de empezar)
- [ ] Leer este documento completamente
- [ ] Verificar que BD_TIENDA tiene tablas de tipos de crédito
- [ ] Compilar CapaDatos y CapaModelo
- [ ] Abrir VentasWeb en Visual Studio

### Implementación
- [ ] **Tarea 1:** Crear Credito/Index.cshtml (30 min)
- [ ] **Tarea 2:** Crear Credito.js (45 min)
- [ ] **Tarea 3:** Crear Credito/AsignarCliente.cshtml (45 min)
- [ ] **Tarea 4:** Integrar en VentaController (30 min)

### Testing
- [ ] Test 1: Maestros en BD
- [ ] Test 2: Asignar crédito
- [ ] Test 3: Ver resumen
- [ ] Test 4: Crear venta (OK)
- [ ] Test 5: Llenar crédito (Fail)
- [ ] Test 6: Suspender
- [ ] Test 7: Vencimiento

### Finalización
- [ ] Compilación: 0 Errores
- [ ] Todas las pruebas OK
- [ ] Documentar resultados
- [ ] Crear PR o commit final

---

## 🎯 Objective Verification

**Objetivo Sesión 4:** Completar 100% de Tipos de Crédito

**Criterios de aceptación:**
- ✅ Vistas CRUD para tipos de crédito
- ✅ Scripts AJAX para asignar/consultar
- ✅ Integración con VentaController
- ✅ Validación de crédito pre-venta
- ✅ Estados automáticos (NORMAL, ALERTA, CRÍTICO, VENCIDO)
- ✅ 0 Errores de compilación
- ✅ 7/7 tests de integración OK

---

## 🚀 Post Sesión 4

Una vez completado 100% de Tipos de Crédito, el sistema estará listo para:

1. **Gestión de Productos y Lotes** (próximo módulo)
2. **Flujo de Ventas POS Completo** (módulo posterior)
3. **Pagos y Cobranza** (con gestión de crédito)

---

**Documento de planificación generado:** 2024  
**Próxima sesión:** Implementación de UI e Integración  
**Estimado:** 3-4 horas  
**Resultado esperado:** Tipos de Crédito 100% funcional ✅
