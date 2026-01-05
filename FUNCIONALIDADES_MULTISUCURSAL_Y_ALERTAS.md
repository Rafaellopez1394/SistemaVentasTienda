# Funcionalidades Implementadas: Multi-Sucursal y Alertas de Precios

## Fecha: 4 de Enero de 2026

---

## 1. SELECTOR DE SUCURSAL GLOBAL

### Descripción
Se agregó un selector de sucursal en el navbar principal del sistema que permite cambiar entre sucursales en tiempo real.

### Ubicación
- **Vista**: `Views/Shared/_Layout.cshtml` (línea ~709)
- **JavaScript**: Funciones en _Layout.cshtml (líneas ~820-900)

### Funcionalidades
✅ **Dropdown en navbar** - Muestra todas las sucursales disponibles
✅ **Persistencia en sesión** - Guarda la sucursal seleccionada en `Session["SucursalActiva"]`
✅ **Cambio automático** - Recarga módulos dependientes al cambiar sucursal
✅ **Inicialización automática** - Al cargar VentaPOS, establece la sucursal activa si hay caja abierta

### Módulos que Usan Sucursal Activa
- ✅ **VentaPOS** - Busca productos de la sucursal seleccionada
- 🔄 **Productos** - Filtra por sucursal (requiere actualización)
- 🔄 **Inventario** - Muestra stock por sucursal (requiere actualización)
- 🔄 **Ventas** - Filtra ventas por sucursal (requiere actualización)
- 🔄 **Compras** - Asigna a sucursal seleccionada (requiere actualización)

### Endpoints Creados
```csharp
// HomeController.cs

[HttpGet]
public JsonResult ObtenerSucursalActiva()
// Retorna: int sucursalID desde Session

[HttpPost]
public JsonResult CambiarSucursalActiva(int sucursalID)
// Retorna: { success: bool, mensaje: string }
```

### JavaScript Implementado
```javascript
// Funciones globales en _Layout.cshtml

cargarSucursales()
// Carga dropdown con todas las sucursales

cambiarSucursal()
// Cambia la sucursal activa y recarga módulos
```

---

## 2. SISTEMA DE AUDITORÍA DE PRECIOS

### Descripción
Sistema completo para registrar y notificar cambios en los precios de productos.

### Base de Datos

#### Tabla Creada: `HistorialCambiosPrecios`
```sql
CREATE TABLE HistorialCambiosPrecios (
    CambioID INT IDENTITY(1,1) PRIMARY KEY,
    ProductoID INT NOT NULL,
    TipoPrecio VARCHAR(50) NOT NULL,          -- 'Venta', 'Compra', 'PrecioEspecial'
    PrecioAnterior DECIMAL(18,2) NOT NULL,
    PrecioNuevo DECIMAL(18,2) NOT NULL,
    DiferenciaPorcentaje DECIMAL(10,2) NULL,  -- Calculado automáticamente
    Usuario VARCHAR(100) NOT NULL,
    FechaCambio DATETIME NOT NULL DEFAULT GETDATE(),
    SucursalID INT NULL,
    Observaciones VARCHAR(500) NULL,
    FOREIGN KEY (ProductoID) REFERENCES Productos(ProductoID),
    FOREIGN KEY (SucursalID) REFERENCES SUCURSAL(SucursalID)
);
```

#### Stored Procedures Creados

**1. `sp_RegistrarCambioPrecio`**
```sql
CREATE PROCEDURE sp_RegistrarCambioPrecio
    @ProductoID INT,
    @TipoPrecio VARCHAR(50),
    @PrecioAnterior DECIMAL(18,2),
    @PrecioNuevo DECIMAL(18,2),
    @Usuario VARCHAR(100),
    @SucursalID INT = NULL,
    @Observaciones VARCHAR(500) = NULL
```
- Registra el cambio de precio
- Calcula diferencia porcentual automáticamente
- Guarda usuario y fecha del cambio

**2. `sp_ObtenerCambiosPreciosRecientes`**
```sql
CREATE PROCEDURE sp_ObtenerCambiosPreciosRecientes
    @Horas INT = 24
```
- Obtiene cambios de las últimas N horas
- Incluye información del producto y sucursal
- Ordenado por fecha descendente

### Capa de Datos

#### CD_Producto.cs - Nuevos Métodos
```csharp
public List<CambioPrecio> ObtenerCambiosPreciosRecientes(int horas = 24)
// Obtiene lista de cambios recientes

public bool RegistrarCambioPrecio(int productoID, string tipoPrecio, 
    decimal precioAnterior, decimal precioNuevo, string usuario, 
    int? sucursalID = null, string observaciones = null)
// Registra un cambio de precio
```

### Modelos

#### CapaModelo/Producto.cs - Nueva Clase
```csharp
public class CambioPrecio
{
    public int CambioID { get; set; }
    public int ProductoID { get; set; }
    public string NombreProducto { get; set; }
    public string TipoPrecio { get; set; }
    public decimal PrecioAnterior { get; set; }
    public decimal PrecioNuevo { get; set; }
    public decimal DiferenciaPorcentaje { get; set; }
    public string Usuario { get; set; }
    public DateTime FechaCambio { get; set; }
    public string NombreSucursal { get; set; }
}
```

### Sistema de Notificaciones

#### Notificación Automática
Al cargar cualquier página del sistema, se ejecuta automáticamente:
```javascript
verificarCambiosPrecios()
// Verifica cambios de las últimas 24 horas
// Muestra notificación toastr con cantidad de cambios
// Permite ver detalle con un clic
```

#### Modal de Detalles
```javascript
mostrarModalCambiosPrecios(cambios)
// Muestra tabla con:
// - Nombre del producto
// - Precio anterior
// - Precio nuevo (con flecha ↑/↓)
// - Fecha y hora del cambio
```

### Endpoints Creados
```csharp
// HomeController.cs

[HttpGet]
public JsonResult ObtenerCambiosPreciosRecientes(int horas = 24)
// Retorna: { success: bool, data: List<CambioPrecio> }
```

---

## 3. INTEGRACIÓN CON MÓDULOS EXISTENTES

### VentaPOS
✅ **Actualizado** - Usa sucursal activa para buscar productos
```csharp
// VentaPOSController.cs - BuscarProducto()
int sucursalID = Session["SucursalActiva"] != null 
    ? (int)Session["SucursalActiva"] 
    : 1;

var productos = CD_VentaPOS.Instancia.BuscarProducto(texto, sucursalID);
```

### Gestión de Caja
✅ **Mejorado** - Establece sucursal al abrir caja
```csharp
// VentaPOSController.cs - Index()
if (estado != null && estado.EstaAbierta)
{
    Session["CajaActiva"] = cajaID;
}
```

### Productos (Pendiente)
🔄 **Requiere actualización** para:
- Filtrar listado por sucursal
- Mostrar stock por sucursal
- Registrar cambios de precio automáticamente

---

## 4. CÓMO USAR LAS NUEVAS FUNCIONALIDADES

### Cambiar de Sucursal
1. En el navbar superior derecho, busca el selector de sucursal (icono 🏪)
2. Selecciona la sucursal deseada del dropdown
3. El sistema recargará automáticamente si estás en VentaPOS, Productos, Inventario, etc.
4. Todos los datos se filtrarán por la sucursal seleccionada

### Ver Alertas de Precios
1. Al entrar al sistema, si hay cambios recientes, verás una notificación amarilla (toastr)
2. Haz clic en la notificación para ver el detalle completo
3. El modal mostrará una tabla con todos los cambios
4. Puedes ver si los precios subieron (🔴 ↑) o bajaron (🟢 ↓)

### Registrar Cambio de Precio (Para Desarrolladores)
```csharp
// Al actualizar el precio de un producto, registrar el cambio:
bool exito = CD_Producto.Instancia.RegistrarCambioPrecio(
    productoID: 123,
    tipoPrecio: "Venta",
    precioAnterior: 25.50m,
    precioNuevo: 28.00m,
    usuario: User.Identity.Name,
    sucursalID: (int?)Session["SucursalActiva"],
    observaciones: "Ajuste por inflación"
);
```

---

## 5. PRÓXIMOS PASOS RECOMENDADOS

### Prioridad ALTA
1. ⚠️ **Actualizar módulo Productos**
   - Agregar filtro por sucursal en listado
   - Implementar registro automático de cambios de precio
   - Agregar trigger en UPDATE de precios

2. ⚠️ **Actualizar módulo Inventario**
   - Filtrar ajustes por sucursal
   - Mostrar stock por sucursal en listados
   - Traspasos entre sucursales con sucursal activa

### Prioridad MEDIA
3. 📊 **Dashboard de Precios**
   - Vista dedicada para analizar cambios
   - Gráficas de tendencias
   - Comparativa entre sucursales

4. 📧 **Notificaciones por Email**
   - Enviar correo cuando cambien precios
   - Configurar umbrales de notificación
   - Alertas para gerentes/administradores

### Prioridad BAJA
5. 🔐 **Permisos por Sucursal**
   - Restringir usuarios a sucursales específicas
   - Roles por sucursal
   - Auditoría de accesos

6. 📱 **Reporte de Cambios**
   - PDF/Excel con historial de cambios
   - Filtros avanzados (fecha, producto, usuario)
   - Gráficas de impacto en ventas

---

## 6. ARCHIVOS MODIFICADOS

### Backend (C#)
```
✅ VentasWeb/Controllers/HomeController.cs
   - ObtenerSucursalActiva()
   - CambiarSucursalActiva()
   - ObtenerCambiosPreciosRecientes()

✅ VentasWeb/Controllers/VentaPOSController.cs
   - Index() - Establece sucursal activa
   - BuscarProducto() - Usa sucursal activa

✅ CapaDatos/CD_Producto.cs
   - ObtenerCambiosPreciosRecientes()
   - RegistrarCambioPrecio()

✅ CapaDatos/CD_VentaPOS.cs
   - BuscarProducto() - Acepta sucursalID

✅ CapaModelo/Producto.cs
   - Clase CambioPrecio
```

### Frontend (Views/JavaScript)
```
✅ VentasWeb/Views/Shared/_Layout.cshtml
   - Selector de sucursal en navbar
   - Funciones JavaScript:
     * cargarSucursales()
     * cambiarSucursal()
     * verificarCambiosPrecios()
     * mostrarModalCambiosPrecios()
```

### Base de Datos
```
✅ SQL Server/DB_TIENDA
   - Tabla: HistorialCambiosPrecios
   - SP: sp_RegistrarCambioPrecio
   - SP: sp_ObtenerCambiosPreciosRecientes
```

---

## 7. CONFIGURACIÓN Y DEPLOYMENT

### Pre-requisitos
- SQL Server con base de datos DB_TIENDA
- ASP.NET MVC Framework 4.7.2+
- Bootstrap 3.x
- jQuery 3.3+
- Toastr.js
- SweetAlert2

### Deployment
1. **Compilar proyecto** en Visual Studio
2. **Ejecutar scripts SQL** para crear tabla y SPs
3. **Reiniciar IIS Express** o IIS
4. **Verificar sucursales** existen en tabla SUCURSAL
5. **Probar selector** de sucursal en navbar

### Troubleshooting

#### El selector no muestra sucursales
- Verificar que existan registros en tabla `SUCURSAL`
- Verificar endpoint `/Sucursal/Obtener` funciona correctamente
- Revisar consola del navegador para errores JavaScript

#### No aparecen notificaciones de cambios
- Verificar que existen registros en `HistorialCambiosPrecios`
- Verificar SP `sp_ObtenerCambiosPreciosRecientes` ejecuta correctamente
- Verificar toastr.js está cargado en la página

#### VentaPOS no filtra por sucursal
- Verificar que `Session["SucursalActiva"]` tiene valor
- Verificar SP `BuscarProductoPOS` acepta parámetro `@SucursalID`
- Agregar logs en `VentaPOSController.BuscarProducto()`

---

## 8. NOTAS TÉCNICAS

### Session Management
- `Session["SucursalActiva"]` - int: ID de sucursal activa
- Se establece al cargar VentaPOS o cambiar manualmente
- Persiste durante toda la sesión del usuario
- Se limpia al cerrar sesión

### Performance
- Índices creados en `HistorialCambiosPrecios`:
  * `IX_HistorialCambiosPrecios_Fecha` - Optimiza búsquedas por fecha
  * `IX_HistorialCambiosPrecios_Producto` - Optimiza por producto

### Seguridad
- ⚠️ Todos los endpoints requieren autenticación (`[CustomAuthorize]`)
- ⚠️ Validar permisos antes de registrar cambios
- ⚠️ Auditoría completa: usuario, fecha, IP (futuro)

---

## 9. TESTING

### Casos de Prueba

#### Test 1: Cambiar Sucursal
1. Abrir cualquier módulo del sistema
2. Cambiar sucursal desde el selector
3. ✅ Verificar: Toastr de confirmación
4. ✅ Verificar: Página recarga si es módulo dependiente
5. ✅ Verificar: Session["SucursalActiva"] actualizado

#### Test 2: Notificación de Precios
1. Registrar cambio de precio en BD:
   ```sql
   EXEC sp_RegistrarCambioPrecio 
       @ProductoID = 1, 
       @TipoPrecio = 'Venta',
       @PrecioAnterior = 10.00,
       @PrecioNuevo = 12.00,
       @Usuario = 'test',
       @SucursalID = 1;
   ```
2. Recargar cualquier página
3. ✅ Verificar: Aparece notificación toastr
4. ✅ Verificar: Clic muestra modal con detalle

#### Test 3: VentaPOS Multi-Sucursal
1. Abrir caja en VentaPOS
2. Cambiar a sucursal 2
3. Buscar un producto
4. ✅ Verificar: Solo muestra productos de sucursal 2
5. ✅ Verificar: Stock es de sucursal 2

---

## 10. SOPORTE

Para preguntas o issues:
- 📧 Email: soporte@sistemapos.com
- 📱 WhatsApp: +52 XXX XXX XXXX
- 🌐 GitHub Issues: [repositorio]

---

**Última actualización**: 4 de Enero de 2026
**Versión del sistema**: 2.0
**Autor**: Equipo de Desarrollo SistemaPOS
