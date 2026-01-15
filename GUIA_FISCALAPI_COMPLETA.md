# Guía Completa de Integración FiscalAPI

## 📋 Resumen

Sistema de facturación electrónica CFDI 4.0 completamente integrado con FiscalAPI, incluyendo timbrado, cancelación, complementos de pago, consultas y descarga masiva.

## 🎯 Funcionalidades Implementadas

### ✅ 1. Timbrado de Facturas (CFDI 4.0)
- **Facturas de Ingreso** (Tipo "I")
- **Notas de Crédito** (Tipo "E")
- **Complementos de Pago** (Tipo "P")
- Generación automática de XML CFDI 4.0
- Validación contra especificaciones SAT
- Timbrado con FiscalAPI
- Almacenamiento de UUID y XML timbrado

### ✅ 2. Cancelación de CFDIs
- Cancelación con motivo SAT
- Folio de sustitución opcional
- Soporte para certificados CSD
- Acuse de cancelación XML
- Estados: Vigente, Cancelado, En proceso

### ✅ 3. Complementos de Pago 2.0
- Generación de XML para pagos
- Documentos relacionados con parcialidades
- Soporte para múltiples monedas (MXN, USD, EUR)
- Tipo de cambio automático
- Saldos: anterior, pagado, insoluto

### ✅ 4. Consultas SAT
- Consulta de estatus de facturas
- Validación EFOS (Empresas que Facturan Operaciones Simuladas)
- Verificación de estado de cancelación
- Estatus cancelable/no cancelable

### ✅ 5. Generación de PDF
- PDF profesional con logo personalizable
- Colores de banda y fuente configurables
- Código QR automático
- Formato compatible con SAT

### ✅ 6. Envío por Email
- Envío automático de PDF + XML
- Personalización de correos
- Múltiples destinatarios
- Seguimiento de envíos

### ✅ 7. Gestión de Certificados (CSD/FIEL)
- Subida de archivos .cer y .key
- Almacenamiento seguro en FiscalAPI
- Validación de vigencia
- Certificados por defecto

### ✅ 8. Descarga Masiva SAT
- Solicitudes de descarga automáticas
- Filtros por fecha, RFC, tipo
- Descarga de XML y metadatos
- Paquetes ZIP comprimidos

### ✅ 9. Catálogos SAT
- Claves de productos y servicios (c_ClaveProdServ)
- Claves de unidad (c_ClaveUnidad)
- Formas de pago (c_FormaPago)
- Métodos de pago (c_MetodoPago)
- Usos de CFDI (c_UsoCFDI)
- Regímenes fiscales (c_RegimenFiscal)
- Búsqueda avanzada

## 📂 Estructura de Archivos

```
CapaDatos/
├── FiscalAPI/
│   └── FiscalApiSDK.cs           # SDK completo de FiscalAPI (1,020 líneas)
├── Generadores/
│   ├── CFDI40XMLGenerator.cs     # Generador XML CFDI 4.0 (240 líneas)
│   └── ComplementoPago20XMLGenerator.cs  # Generador XML Pagos 2.0 (180 líneas)
├── CD_Factura.cs                 # Lógica de facturación
├── CD_ComplementoPago.cs         # Lógica de complementos de pago
└── Conexion.cs                   # Conexión a base de datos

CapaModelo/
├── Factura.cs                    # Modelo de factura CFDI
├── FacturaDetalle.cs             # Conceptos de factura
├── ComplementoPago.cs            # Modelo de complemento de pago
└── ConfiguracionPAC.cs           # Configuración de FiscalAPI
```

## 🔧 Configuración Inicial

### 1. Registro en FiscalAPI

1. Crear cuenta en https://fiscalapi.com/
2. Obtener credenciales:
   - **API Key**: Tu clave de API
   - **Tenant**: Tu identificador de tenant
   - **Environment**: test (pruebas) o live (producción)

### 2. Configurar en Base de Datos

```sql
-- Insertar configuración de FiscalAPI
INSERT INTO ConfiguracionPAC (
    ProveedorPAC,
    UsuarioAPI,
    ContrasenaAPI,
    URLWebService,
    URLWebServiceTest,
    Activo,
    EsPrueba
) VALUES (
    'FiscalAPI',                              -- Proveedor
    '<tu-api-key>',                           -- API Key
    '<tu-tenant>',                            -- Tenant
    'https://live.fiscalapi.com',             -- URL Producción
    'https://test.fiscalapi.com',             -- URL Pruebas
    1,                                        -- Activo
    1                                         -- Modo pruebas (cambiar a 0 en producción)
);
```

### 3. Subir Certificados CSD

Los certificados digitales (.cer y .key) deben subirse a FiscalAPI:

#### Opción A: Manual (Dashboard de FiscalAPI)
1. Ingresar a https://dashboard.fiscalapi.com/
2. Ir a Certificados > Agregar Certificado
3. Subir archivo .cer (certificado)
4. Subir archivo .key (llave privada)
5. Ingresar contraseña del certificado

#### Opción B: API (Programático)

```csharp
var fiscalApi = FiscalApiClient.Create(new FiscalapiSettings
{
    ApiUrl = "https://test.fiscalapi.com",
    ApiKey = "<tu-api-key>",
    Tenant = "<tu-tenant>",
    ApiVersion = "v4"
});

// Crear persona (emisor)
var persona = new Person
{
    LegalName = "MI EMPRESA SA DE CV",
    Email = "contacto@miempresa.com",
    Password = "password123",
    Tin = "EKU9003173C9",  // RFC
    TaxRegimeCode = "601",  // Régimen General
    ZipCode = "42501"
};
var personaResponse = await fiscalApi.Persons.CreateAsync(persona);
string personId = personaResponse.Data.Id;

// Subir certificado .cer
var certificado = new TaxFile
{
    PersonId = personId,
    Tin = "EKU9003173C9",
    Base64File = Convert.ToBase64String(File.ReadAllBytes("certificado.cer")),
    FileType = FileType.CertificateCsd,
    Password = "12345678a"
};
await fiscalApi.TaxFiles.CreateAsync(certificado);

// Subir llave privada .key
var llavePrivada = new TaxFile
{
    PersonId = personId,
    Tin = "EKU9003173C9",
    Base64File = Convert.ToBase64String(File.ReadAllBytes("llave_privada.key")),
    FileType = FileType.PrivateKeyCsd,
    Password = "12345678a"
};
await fiscalApi.TaxFiles.CreateAsync(llavePrivada);
```

## 🚀 Uso del Sistema

### Flujo de Facturación Completo

1. **Venta en POS** → Se registra la venta
2. **Generar Factura** → Se crea la factura con datos del cliente
3. **Generar XML** → CFDI40XMLGenerator crea el XML
4. **Timbrar con FiscalAPI** → Se envía a FiscalAPI para timbrado
5. **Recibir UUID** → FiscalAPI devuelve UUID y XML timbrado
6. **Guardar en BD** → Se almacena UUID y XML
7. **Generar PDF** → Se crea el PDF de la factura
8. **Enviar por Email** → Se envía PDF + XML al cliente

### Código de Ejemplo - Facturación

```csharp
// En CD_Factura.cs - GenerarYTimbrarFactura()

// 1. Crear factura desde venta
var factura = CrearFacturaDesdeVenta(ventaId, configuracionEmpresa, datosReceptor);

// 2. Generar XML CFDI 4.0
var generadorXML = new Generadores.CFDI40XMLGenerator();
string xmlSinTimbrar = generadorXML.GenerarXML(factura);

// 3. Validar XML
string errorValidacion;
if (!generadorXML.ValidarXML(xmlSinTimbrar, out errorValidacion))
{
    throw new Exception($"XML inválido: {errorValidacion}");
}

// 4. Guardar factura (obtener ID)
factura.XMLOriginal = xmlSinTimbrar;
var respuestaGuardar = GuardarFactura(factura);

// 5. Obtener proveedor PAC (FiscalAPI)
var proveedorPAC = ObtenerProveedorPAC(config.ProveedorPAC);

// 6. Timbrar con FiscalAPI
var respuestaTimbrado = await proveedorPAC.TimbrarAsync(xmlSinTimbrar, config);

if (respuestaTimbrado.estado)
{
    // 7. Actualizar con datos del timbrado
    factura.UUID = respuestaTimbrado.objeto.Uuid;
    factura.XMLTimbrado = respuestaTimbrado.objeto.XmlTimbrado;
    factura.FechaTimbrado = DateTime.Now;
    
    ActualizarConTimbrado(factura);
    
    // 8. Generar PDF
    GenerarPDF(factura.FacturaID);
    
    // 9. Enviar por email (opcional)
    if (!string.IsNullOrEmpty(factura.ReceptorEmail))
    {
        EnviarPorEmail(factura.FacturaID, factura.ReceptorEmail);
    }
}
```

### Código de Ejemplo - Cancelación

```csharp
// En CD_Factura.cs - CancelarFactura()

var factura = ObtenerFacturaPorId(facturaId);

// Preparar solicitud de cancelación
var solicitud = new Fiscalapi.CancelInvoiceRequest
{
    InvoiceUuid = factura.UUID,
    Tin = factura.EmisorRFC,
    CancellationReasonCode = motivoCancelacion,  // "01", "02", "03", "04"
    ReplacementUuid = folioSustitucion,          // Opcional
    TaxCredentials = new List<Fiscalapi.TaxCredential>
    {
        new Fiscalapi.TaxCredential
        {
            Base64File = certificadoCerBase64,
            FileType = Fiscalapi.FileType.CertificateCsd,
            Password = passwordCertificado
        },
        new Fiscalapi.TaxCredential
        {
            Base64File = llavePrivadaKeyBase64,
            FileType = Fiscalapi.FileType.PrivateKeyCsd,
            Password = passwordCertificado
        }
    }
};

// Cancelar con FiscalAPI
var respuesta = await fiscalApi.Invoices.CancelAsync(solicitud);

if (respuesta.Succeeded)
{
    // Actualizar factura como cancelada
    factura.EsCancelada = true;
    factura.FechaCancelacion = DateTime.Now;
    factura.MotivoCancelacion = motivoCancelacion;
    factura.FolioSustitucion = folioSustitucion;
    
    ActualizarFactura(factura);
}
```

### Código de Ejemplo - Complemento de Pago

```csharp
// En CD_ComplementoPago.cs - GenerarYTimbrarComplementoPago()

// 1. Crear complemento de pago
var complemento = new ComplementoPago
{
    EmisorRFC = empresa.RFC,
    ReceptorRFC = cliente.RFC,
    // ... datos del complemento
};

// Agregar pago
var pago = new ComplementoPagoPago
{
    FechaPago = DateTime.Now,
    FormaPago = "03",  // Transferencia
    Moneda = "MXN",
    TipoCambio = 1.0m,
    Monto = montoPagado
};

// Agregar documentos relacionados (facturas que se pagan)
pago.DocumentosRelacionados.Add(new ComplementoPagoDocumento
{
    IdDocumento = facturaUuid,
    NumParcialidad = 1,
    ImpSaldoAnterior = saldoAnterior,
    ImpPagado = montoPagado,
    ImpSaldoInsoluto = saldoAnterior - montoPagado
});

complemento.Pagos.Add(pago);

// 2. Generar XML del complemento
var generator = new Generadores.ComplementoPago20XMLGenerator();
string xmlSinTimbrar = generator.GenerarXML(complemento, empresa);

// 3. Timbrar con FiscalAPI
var proveedorPAC = ObtenerProveedorPAC(configPAC.ProveedorPAC);
var respuesta = await proveedorPAC.TimbrarAsync(xmlSinTimbrar, configPAC);

if (respuesta.estado)
{
    complemento.UUID = respuesta.objeto.Uuid;
    complemento.XMLTimbrado = respuesta.objeto.XmlTimbrado;
    GuardarComplementoPago(complemento);
}
```

## 📊 Consultas y Reportes

### Consultar Estatus de Factura en SAT

```csharp
var solicitud = new InvoiceStatusRequest
{
    Id = facturaId,  // Si tienes el ID en FiscalAPI
    // O por valores:
    IssuerTin = "EKU9003173C9",
    RecipientTin = "XAXX010101000",
    InvoiceUuid = "12345678-1234-1234-1234-123456789012",
    InvoiceTotal = 1160.00m,
    Last8DigitsIssuerSignature = "AB12CD34"
};

var respuesta = await fiscalApi.Invoices.GetStatusAsync(solicitud);

if (respuesta.Succeeded)
{
    var status = respuesta.Data;
    Console.WriteLine($"Estado: {status.Status}");              // Vigente, Cancelado, No Encontrado
    Console.WriteLine($"Cancelable: {status.CancelableStatus}"); // Cancelable con/sin aceptación
    Console.WriteLine($"EFOS: {status.EfosValidation}");        // Validación lista negra
}
```

### Descarga Masiva de CFDIs

```csharp
// 1. Crear solicitud de descarga
var solicitud = new DownloadRequest
{
    IssuerTin = "EKU9003173C9",      // RFC emisor (opcional si eres receptor)
    RequesterTin = "EKU9003173C9",   // RFC del solicitante
    StartDate = new DateTime(2024, 1, 1),
    EndDate = new DateTime(2024, 1, 31),
    SatQueryTypeId = "CFDI",         // CFDI o Metadata
    SatInvoiceTypeId = "I",          // I=Ingreso, E=Egreso, T=Traslado, P=Pago
    SatInvoiceStatusId = "1"         // 1=Vigentes, 0=Cancelados
};

var respuesta = await fiscalApi.DownloadRequests.CreateAsync(solicitud);
string requestId = respuesta.Data.Id;

// 2. Esperar a que se procese (puede tomar minutos)
// Consultar estado periódicamente
var estado = await fiscalApi.DownloadRequests.GetByIdAsync(requestId);

// 3. Cuando esté terminado, descargar metadatos
if (estado.Data.DownloadRequestStatusId == "TERMINADA")
{
    var metadatos = await fiscalApi.DownloadRequests.GetMetadataAsync(requestId);
    
    foreach (var item in metadatos.Data)
    {
        Console.WriteLine($"UUID: {item.InvoiceUuid}");
        Console.WriteLine($"Emisor: {item.IssuerName}");
        Console.WriteLine($"Total: {item.Total}");
        Console.WriteLine($"Estado: {item.Status}");  // 1=Vigente, 0=Cancelado
    }
    
    // 4. Descargar XMLs en paquetes ZIP
    var paquetes = await fiscalApi.DownloadRequests.DownloadPackageAsync(requestId);
    
    foreach (var paquete in paquetes.Data)
    {
        byte[] zipBytes = Convert.FromBase64String(paquete.Base64Content);
        File.WriteAllBytes($"cfdi_{requestId}.zip", zipBytes);
    }
}
```

## 🔐 Seguridad

### Protección de Credenciales

1. **Nunca** hardcodear API Keys en el código
2. Almacenar en base de datos encriptada
3. Usar variables de entorno en producción
4. Rotar credenciales periódicamente

### Certificados CSD

1. Almacenar en FiscalAPI (no en tu servidor)
2. Usar contraseñas fuertes
3. Renovar antes de vencimiento
4. Mantener respaldo seguro

## 📈 Monitoreo y Logs

### Logs de Facturación

```csharp
// En CD_Factura.cs - siempre loguear eventos importantes
try
{
    var resultado = await GenerarYTimbrarFactura(ventaId, ...);
    
    // Log exitoso
    Logger.Info($"Factura timbrada: UUID={resultado.UUID}, Venta={ventaId}");
}
catch (Exception ex)
{
    // Log de error
    Logger.Error($"Error al timbrar factura: {ex.Message}", ex);
    throw;
}
```

### Métricas Importantes

- Tasa de éxito de timbrado
- Tiempo promedio de timbrado
- Errores frecuentes
- Facturas canceladas
- Complementos de pago emitidos

## 🧪 Pruebas

### Ambiente de Pruebas

FiscalAPI proporciona ambiente de pruebas:
- **URL**: https://test.fiscalapi.com
- **RFC de prueba**: EKU9003173C9
- **Certificados de prueba**: https://docs.fiscalapi.com/tax-files-info

### Certificados de Prueba SAT

Descargar certificados de prueba del SAT:
```
Emisor: EKU9003173C9
Certificado (.cer): https://docs.fiscalapi.com/test-certificates/certificate.cer
Llave Privada (.key): https://docs.fiscalapi.com/test-certificates/private_key.key
Contraseña: 12345678a
```

### Flujo de Prueba Completo

1. Configurar ambiente de pruebas (EsPrueba = 1)
2. Subir certificados de prueba
3. Crear venta de prueba en POS
4. Generar factura con RFC de prueba
5. Verificar que se timbre correctamente
6. Consultar estatus en SAT
7. Probar cancelación
8. Generar complemento de pago
9. Verificar PDF generado
10. Probar envío por email

## 🐛 Troubleshooting

### Error: "No se encontró configuración PAC activa"

```sql
-- Verificar configuración
SELECT * FROM ConfiguracionPAC WHERE Activo = 1;

-- Activar FiscalAPI
UPDATE ConfiguracionPAC 
SET Activo = 1 
WHERE ProveedorPAC = 'FiscalAPI';
```

### Error: "Certificados inválidos o expirados"

1. Verificar vigencia en Dashboard de FiscalAPI
2. Renovar certificados antes del vencimiento
3. Subir nuevos certificados
4. Actualizar PersonId en configuración

### Error: "XML no cumple con la especificación CFDI 4.0"

1. Verificar que todos los campos obligatorios estén presentes
2. Validar formato de RFC (12-13 caracteres)
3. Verificar códigos de catálogos SAT
4. Usar ValidarXML() antes de timbrar

### Error: "Factura ya fue timbrada"

- Cada venta solo puede facturarse una vez
- Verificar campo VentaID en tabla Facturas
- Usar UUID existente si ya se timbró

### Error de timeout en timbrado

```csharp
// Aumentar timeout del HttpClient
var httpClient = new HttpClient();
httpClient.Timeout = TimeSpan.FromSeconds(120);
```

## 📚 Recursos Adicionales

- **Documentación FiscalAPI**: https://docs.fiscalapi.com/
- **GitHub FiscalAPI SDK**: https://github.com/FiscalAPI/fiscalapi-net
- **Especificación CFDI 4.0**: http://www.sat.gob.mx/sitio_internet/cfd/4/cfdv40.xsd
- **Complemento Pagos 2.0**: http://www.sat.gob.mx/sitio_internet/cfd/Pagos/Pagos20.xsd
- **Catálogos SAT**: http://omawww.sat.gob.mx/tramitesyservicios/Paginas/catalogos_emision_cfdi_complemento.htm
- **Portal SAT**: https://www.sat.gob.mx/

## ✅ Checklist de Producción

Antes de pasar a producción:

- [ ] Configurar credenciales de producción (live.fiscalapi.com)
- [ ] Subir certificados CSD reales y vigentes
- [ ] Cambiar EsPrueba = 0 en ConfiguracionPAC
- [ ] Validar datos de empresa (RFC, régimen fiscal, domicilio)
- [ ] Probar timbrado real con cliente de prueba
- [ ] Verificar generación de PDF con logo real
- [ ] Configurar emails correctamente
- [ ] Implementar sistema de logs robusto
- [ ] Configurar backups automáticos de BD
- [ ] Capacitar usuarios en el sistema
- [ ] Documentar procedimientos de operación
- [ ] Preparar plan de contingencia

## 🎓 Capacitación de Usuarios

### Para Cajeros/Vendedores

1. Cómo realizar una venta en POS
2. Cómo solicitar datos de facturación al cliente
3. Qué hacer si la facturación falla
4. Cómo reimprimir una factura

### Para Administradores

1. Configuración de FiscalAPI
2. Gestión de certificados CSD
3. Cancelación de facturas
4. Complementos de pago
5. Consultas y reportes
6. Descarga masiva del SAT
7. Resolución de problemas comunes

## 📞 Soporte

### FiscalAPI
- Email: support@fiscalapi.com
- Chat: https://fiscalapi.com/chat
- Documentación: https://docs.fiscalapi.com/

### SAT
- Teléfono: 55 627 22 728
- Portal: https://www.sat.gob.mx/
- Chat: http://chat.sat.gob.mx/

---

**Versión**: 1.0  
**Última actualización**: Enero 2026  
**Sistema**: SistemaVentasTienda  
**PAC**: FiscalAPI
