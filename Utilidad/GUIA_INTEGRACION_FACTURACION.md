# GUÍA DE INTEGRACIÓN - MÓDULO DE FACTURACIÓN CFDI 4.0

## ESTADO ACTUAL: 70% COMPLETADO ✅

### ✅ COMPONENTES IMPLEMENTADOS

#### 1. **Base de Datos** (100%)
- ✅ 7 tablas creadas (script 007)
- ✅ Relación VentasClientes→Facturas (script 008)
- ✅ Catálogos SAT cargados (24 usos CFDI, 19 regímenes)
- ✅ Configuración Finkok Sandbox activa
- ✅ Stored Procedures: MarcarVentaFacturada, ObtenerVentasPendientesFacturar

#### 2. **Backend C#** (95%)
- ✅ **Modelos**: `Factura.cs` (9 clases)
- ✅ **Capa de Datos**: `CD_Factura.cs` (8 métodos + factory)
- ✅ **Abstracción PAC**: `IProveedorPAC.cs` (3 métodos async)
- ✅ **Proveedor Finkok**: `FinkokPAC.cs` (TimbrarAsync completo)
- ✅ **Generador XML**: `CFDI40XMLGenerator.cs` (cumple Anexo 20)
- ✅ **Controlador**: `FacturaController.cs` (6 endpoints)
- ⚠️ Pendiente: Firma digital para cancelación (.CER/.KEY)

#### 3. **Frontend** (80%)
- ✅ **Vista Index**: Listado de facturas con DataTables
- ✅ **Modal Generar**: Formulario completo con validaciones
- ✅ **Scripts JS**: Factura_Index.js + GenerarFactura.js
- ✅ **Descarga XML**: Endpoint funcional
- ⚠️ Pendiente: Integración en historial de ventas

---

## 🚀 PASOS PARA COMPLETAR LA INTEGRACIÓN

### **PASO 1: Agregar Botón de Facturar en Historial de Ventas**

**Archivo**: `Views/Venta/Index.cshtml` o `Views/VentaPOS/Index.cshtml`

**Agregar columna en DataTable**:
```javascript
// En la configuración de columnas de DataTables
{
    data: null,
    orderable: false,
    render: function (data) {
        var botones = '';
        
        // Botón de facturar solo si no está facturada
        if (!data.EstaFacturada) {
            botones += '<button class="btn btn-sm btn-success" ' +
                      'onclick="mostrarModalFacturar(\'' + data.VentaID + '\')" ' +
                      'title="Generar Factura">' +
                      '<i class="fas fa-file-invoice"></i>' +
                      '</button> ';
        } else {
            botones += '<button class="btn btn-sm btn-secondary" disabled ' +
                      'title="Ya facturada">' +
                      '<i class="fas fa-check"></i>' +
                      '</button> ';
        }
        
        return botones;
    }
}
```

**Incluir modal y scripts**:
```html
<!-- Al final de la vista, antes de @section scripts -->
@Html.Partial("~/Views/Shared/_ModalGenerarFactura.cshtml")

@section scripts {
    <script src="~/Scripts/Factura/GenerarFactura.js"></script>
}
```

---

### **PASO 2: Actualizar CD_Factura para Marcar Venta como Facturada**

**Archivo**: `CapaDatos/CD_Factura.cs`

**En el método `GenerarYTimbrarFactura`, después de línea 514 (después del timbrado exitoso)**:
```csharp
// 6. Actualizar factura con datos del timbrado
if (!ActualizarConTimbrado(factura.FacturaID, respuestaTimbrado, out string mensajeActualizar))
{
    respuesta.Mensaje = mensajeActualizar;
    return respuesta;
}

// AGREGAR AQUÍ: Marcar venta como facturada
using (SqlConnection cnx = new SqlConnection(Conexion.CN))
{
    SqlCommand cmd = new SqlCommand("MarcarVentaFacturada", cnx);
    cmd.CommandType = CommandType.StoredProcedure;
    cmd.Parameters.AddWithValue("@VentaID", request.VentaID);
    cmd.Parameters.AddWithValue("@FacturaID", factura.FacturaID);
    cnx.Open();
    cmd.ExecuteNonQuery();
}

// 7. Respuesta exitosa
```

---

### **PASO 3: Agregar Columna EstaFacturada en Query de Ventas**

**Archivo**: `CapaDatos/CD_Venta.cs` (o donde obtienes el listado de ventas)

**Modificar query SELECT para incluir**:
```sql
SELECT 
    v.VentaID,
    v.NumeroDocumento,
    v.FechaVenta,
    v.MontoTotal,
    v.EstaFacturada,  -- AGREGAR ESTA COLUMNA
    v.FacturaID,      -- AGREGAR ESTA COLUMNA
    -- ... resto de columnas
FROM VentasClientes v
```

---

### **PASO 4: Configurar Datos del Emisor** (IMPORTANTE)

**Archivo**: `CapaDatos/CD_Factura.cs`

**En el método `CrearFacturaDesdeVenta`, líneas 113-115, reemplazar valores demo**:
```csharp
// CAMBIAR ESTOS VALORES POR LOS REALES DE TU EMPRESA
factura.EmisorRFC = "TU_RFC_AQUI";  // Ej: "ABC123456789"
factura.EmisorNombre = "TU RAZON SOCIAL AQUI";  // Ej: "EMPRESA DEMO SA DE CV"
factura.EmisorRegimenFiscal = "601";  // Consultar régimen fiscal de tu empresa
```

**Mejor práctica**: Crear tabla `Configuracion` con datos del emisor:
```sql
CREATE TABLE ConfiguracionEmisor (
    ConfigID INT PRIMARY KEY IDENTITY,
    RFC VARCHAR(13) NOT NULL,
    RazonSocial VARCHAR(250) NOT NULL,
    RegimenFiscal VARCHAR(3) NOT NULL,
    CodigoPostal VARCHAR(5) NOT NULL,
    Logo VARBINARY(MAX),
    Activo BIT DEFAULT 1
);

-- Insertar datos reales
INSERT INTO ConfiguracionEmisor (RFC, RazonSocial, RegimenFiscal, CodigoPostal)
VALUES ('ABC123456789', 'MI EMPRESA SA DE CV', '601', '00000');
```

---

### **PASO 5: Configurar Certificados SAT** (Producción)

**Para producción**, necesitas:

1. **Obtener certificados del SAT**:
   - Archivo `.CER` (Certificado de Sello Digital)
   - Archivo `.KEY` (Llave Privada)
   - Contraseña de la llave privada

2. **Guardar archivos en servidor**:
   ```
   C:\Certificados\
       ├── certificado.cer
       └── llave_privada.key
   ```

3. **Actualizar configuración en BD**:
   ```sql
   UPDATE ConfiguracionPAC
   SET RutaCertificado = 'C:\Certificados\certificado.cer',
       RutaLlavePrivada = 'C:\Certificados\llave_privada.key',
       PasswordLlave = 'TU_CONTRASEÑA_AQUI',  -- ⚠️ Encriptar en producción
       EsProduccion = 1,  -- Cambiar a producción
       Usuario = 'TU_USUARIO_FINKOK_REAL',
       Password = 'TU_PASSWORD_FINKOK_REAL'
   WHERE ProveedorPAC = 'Finkok';
   ```

---

### **PASO 6: Implementar Generador de PDF** (Opcional pero recomendado)

**Instalar paquete NuGet**:
```powershell
Install-Package iTextSharp
```

**Crear clase** `CapaDatos/PDF/FacturaPDFGenerator.cs`:
```csharp
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;

public class FacturaPDFGenerator
{
    public string GenerarPDF(Factura factura, string rutaSalida)
    {
        Document documento = new Document(PageSize.LETTER);
        PdfWriter writer = PdfWriter.GetInstance(documento, new FileStream(rutaSalida, FileMode.Create));
        
        documento.Open();
        
        // Header con logo
        // Datos emisor y receptor
        // Tabla de conceptos
        // Totales
        // QR Code con UUID
        // Sello digital (truncado)
        // Cadena original (truncada)
        
        documento.Close();
        return rutaSalida;
    }
}
```

**Integrar en CD_Factura.GenerarYTimbrarFactura** (después del timbrado):
```csharp
// 7. Generar PDF
var generadorPDF = new FacturaPDFGenerator();
string rutaPDF = Server.MapPath($"~/Facturas/PDF/{factura.Serie}{factura.Folio}.pdf");
generadorPDF.GenerarPDF(factura, rutaPDF);
GuardarRutaPDF(factura.FacturaID, rutaPDF, out _);
```

---

### **PASO 7: Testing en Sandbox**

**Datos de prueba Finkok**:
- RFC emisor pruebas: `EKU9003173C9`
- RFC receptor pruebas: `XAXX010101000`
- URL Sandbox: `https://demo-facturacion.finkok.com/servicios/soap/stamp.wsdl`

**Flujo de prueba**:
1. Crear venta de prueba ($1000.00 con IVA)
2. Generar factura con RFC `XAXX010101000`
3. Verificar que XML cumple Anexo 20
4. Enviar a Finkok Sandbox
5. Validar UUID recibido (formato: 8-4-4-4-12 caracteres)
6. Descargar XML timbrado
7. Probar descarga (botón XML)

---

## ⚠️ PENDIENTES CRÍTICOS

### 1. **Firma Digital para Cancelación** (Prioridad Alta)
El método `FinkokPAC.CancelarAsync` requiere firmar el XML de cancelación con los certificados .CER/.KEY. Esto requiere:
- Leer certificado X509 desde archivo .CER
- Leer llave privada desde archivo .KEY
- Generar cadena original del XML de cancelación
- Aplicar SHA-256 + RSA para crear sello digital

**Referencias**:
- https://wiki.finkok.com/doku.php?id=cancelacion
- https://www.sat.gob.mx/aplicacion/operacion/31274/consulta-y-recupera-comprobantes

### 2. **Validación XSD del SAT** (Prioridad Media)
El método `CFDI40XMLGenerator.ValidarXML` debe validar contra esquemas XSD oficiales:
- Descargar: http://www.sat.gob.mx/sitio_internet/cfd/4/cfdv40.xsd
- Descargar: http://www.sat.gob.mx/sitio_internet/cfd/TimbreFiscalDigital/TimbreFiscalDigitalv11.xsd

### 3. **Manejo de Errores Finkok** (Prioridad Media)
Mapear códigos de error de Finkok a mensajes amigables:
- `307`: CFDI duplicado
- `302`: Certificado inválido
- `305`: Certificado revocado
- `403`: Límite de timbres excedido

---

## 📋 CHECKLIST ANTES DE PRODUCCIÓN

- [ ] Configurar RFC, Razón Social y Régimen Fiscal real del emisor
- [ ] Subir certificados .CER y .KEY al servidor
- [ ] Actualizar credenciales Finkok de Sandbox a Producción
- [ ] Configurar contraseña de llave privada (encriptada)
- [ ] Agregar botón "Facturar" en historial de ventas
- [ ] Probar flujo completo: Venta → Factura → Timbrado → Descarga XML
- [ ] Implementar generador de PDF (opcional)
- [ ] Configurar SMTP para envío de emails
- [ ] Implementar firma digital para cancelación
- [ ] Validar contra XSD del SAT
- [ ] Probar con 5-10 facturas en Sandbox
- [ ] Documentar proceso para usuarios finales
- [ ] Capacitar usuarios en uso de módulo
- [ ] Backup de base de datos antes de Go-Live

---

## 🎯 BENEFICIOS IMPLEMENTADOS

✅ **Cumplimiento Legal**: CFDI 4.0 según Anexo 20 del SAT  
✅ **Arquitectura Flexible**: Cambiar PAC sin modificar código (IProveedorPAC)  
✅ **Seguridad**: Sandbox para pruebas, configuración por ambiente  
✅ **Trazabilidad**: Relación Venta→Factura bidireccional  
✅ **Auditoría**: Registro completo de timbrado y cancelaciones  
✅ **Experiencia de Usuario**: Modal intuitivo con validaciones en tiempo real  

---

## 📞 SOPORTE

**Finkok**:
- Sandbox: https://demo-facturacion.finkok.com
- Documentación: https://wiki.finkok.com
- Soporte: soporte@finkok.com

**SAT**:
- Portal: https://www.sat.gob.mx
- Anexo 20: http://omawww.sat.gob.mx/tramitesyservicios/Paginas/anexo_20.htm
- Estatus CFDI: https://verificacfdi.facturaelectronica.sat.gob.mx/

---

**Generado**: 2025-12-14  
**Versión**: CFDI 4.0  
**Framework**: ASP.NET MVC 5 + .NET Framework 4.6
