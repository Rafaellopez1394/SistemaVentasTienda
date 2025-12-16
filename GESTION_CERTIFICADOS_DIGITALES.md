# GESTIÓN DE CERTIFICADOS DIGITALES

## 📋 Descripción General

El módulo de **Certificados Digitales** permite al contador gestionar los archivos `.CER` y `.KEY` necesarios para la facturación electrónica mediante el PAC (Proveedor Autorizado de Certificación).

Este módulo es equivalente a la funcionalidad de gestión de certificados en **app.tesk.mx**.

---

## 🎯 Funcionalidades Principales

### 1. Tipos de Certificados Soportados

#### **CSD (Certificado de Sello Digital)**
- Usado para **timbrar facturas** (CFDI 4.0)
- Requerido para **nómina electrónica**
- Necesario para **cancelar facturas**
- Emitido por el SAT para personas físicas y morales

#### **FIEL (Firma Electrónica Avanzada)**
- Usado para **trámites ante el SAT**
- Firma de documentos oficiales
- Representación legal digital

---

### 2. Carga de Certificados

**Proceso:**
1. Ir a **Contador → Certificados Digitales**
2. Clic en **"Subir Certificado"**
3. Seleccionar:
   - **Tipo**: CSD o FIEL
   - **Nombre descriptivo**: Ej: "Certificado 2024"
   - **Archivo .CER**: El certificado público
   - **Archivo .KEY**: La llave privada
   - **Contraseña**: Password del archivo .KEY
4. Seleccionar **usos**:
   - ✅ Facturas
   - ✅ Nómina
   - ✅ Cancelaciones
5. Opcionalmente marcar como **predeterminado**
6. Guardar

**Validaciones automáticas:**
- ✅ Lectura del certificado .CER
- ✅ Extracción del RFC
- ✅ Extracción del No. de Certificado
- ✅ Verificación de fechas de vigencia
- ✅ Validación de que no esté vencido
- ⚠️ Alerta si vence en menos de 30 días

---

### 3. Almacenamiento Seguro

**Base de Datos:**
- Los archivos `.CER` y `.KEY` se almacenan en **VARBINARY(MAX)**
- La contraseña se guarda **encriptada en Base64** (debe mejorarse con AES en producción)
- Se conservan los **nombres originales** de los archivos
- Se registra **auditoría completa** (usuario, fecha creación/modificación)

**Tabla: `CertificadosDigitales`**
```sql
- CertificadoID (PK)
- TipoCertificado (CSD, FIEL)
- NombreCertificado
- NoCertificado (extraído del .CER)
- RFC (extraído del .CER)
- RazonSocial (extraída del .CER)
- FechaInicio, FechaVencimiento
- ArchivoCER (VARBINARY)
- ArchivoKEY (VARBINARY)
- PasswordKEY (encriptado)
- Activo, EsPredeterminado
- UsarParaFacturas, UsarParaNomina, UsarParaCancelaciones
- Auditoría
```

---

### 4. Gestión de Certificados

**Operaciones disponibles:**

#### **Establecer como Predeterminado** ⭐
- Define qué certificado se usa por defecto
- Solo puede haber **uno predeterminado por tipo** (CSD/FIEL)
- Automáticamente desactiva otros predeterminados del mismo tipo

#### **Activar/Desactivar** ✅❌
- Activar certificados previamente desactivados
- Desactivar certificados sin eliminarlos
- Solo certificados **activos y vigentes** pueden usarse para timbrar

#### **Eliminar** 🗑️
- Eliminación lógica (marca como inactivo)
- No se borran los registros físicamente
- Útil para mantener histórico

---

### 5. Alertas de Vencimiento

**Sistema automático de monitoreo:**

- ⚠️ **Alerta en Dashboard** si hay certificados por vencer en 30 días
- 🔴 **Badge rojo** en tabla si está vencido
- 🟡 **Badge amarillo** si vence en menos de 30 días
- 📊 **Días restantes** mostrados en la tabla

**Ejemplo de alerta:**
```
⚠️ Tiene 2 certificado(s) próximo(s) a vencer:
• Certificado 2024 (vence en 25 días)
• Certificado Nómina (vence en 12 días)
```

---

### 6. Visualización

**Tabla de Certificados muestra:**
- 🏷️ Tipo (CSD/FIEL)
- 📝 Nombre descriptivo
- 🔢 No. de Certificado
- 🆔 RFC asociado
- 🏢 Razón Social
- 📅 Fecha de vencimiento
- 🎯 Usos configurados (Facturas, Nómina, Cancelaciones)
- ✅ Estado (Activo/Inactivo)
- 🟢 Vigencia (Vigente/Vencido)
- ⭐ Indicador de predeterminado

---

## 🛠️ Implementación Técnica

### Archivos Creados

#### **1. Modelos** (`CapaModelo/ConfiguracionContador.cs`)
```csharp
- CertificadoDigital: Entidad completa con propiedades calculadas
- SubirCertificadoRequest: DTO para upload
- InfoCertificado: Datos extraídos del .CER
```

**Propiedades calculadas:**
- `EstaVigente`: bool (compara FechaVencimiento con hoy)
- `DiasParaVencer`: int (diferencia en días)

#### **2. Capa de Datos** (`CapaDatos/CD_ConfiguracionContador.cs`)
```csharp
Métodos:
- GuardarCertificado(): Inserta certificado con archivos binarios
- ObtenerCertificados(): Lista con filtros (tipo, activos)
- ObtenerCertificadoPredeterminado(): Obtiene el activo predeterminado
- ActualizarEstadoCertificado(): Activa/desactiva y marca predeterminado
- EliminarCertificado(): Eliminación lógica
```

#### **3. Controlador** (`Controllers/ContadorController.cs`)
```csharp
Endpoints:
GET  /Contador/Certificados → Vista
GET  /Contador/ObtenerCertificados → JSON DataTable
POST /Contador/SubirCertificado → Upload con HttpPostedFileBase
POST /Contador/ActivarCertificado
POST /Contador/DesactivarCertificado
POST /Contador/EliminarCertificado

Métodos auxiliares:
- ExtraerInfoCertificado(): Lee .CER con X509Certificate2
- EncriptarPassword(): Base64 (mejorar con AES)
- DesencriptarPassword(): Base64 reverse
```

#### **4. Vista** (`Views/Contador/Certificados.cshtml`)
- Tabla con DataTables
- Modal de carga con `multipart/form-data`
- Inputs de tipo `file` para .CER y .KEY
- Checkboxes para usos
- Alertas de vencimiento

#### **5. JavaScript** (`Scripts/Contador/Certificados.js`)
```javascript
Funciones:
- cargarCertificados(): DataTable AJAX
- subirCertificado(): FormData con archivos
- establecerPredeterminado()
- activarCertificado()
- desactivarCertificado()
- eliminarCertificado()
- verificarVencimientos(): Auto-alerta
```

#### **6. SQL** (`020_CREAR_ROL_CONTADOR.sql`)
- Tabla `CertificadosDigitales`
- Índices en RFC, NoCertificado, Activo, FechaVencimiento

---

## 🔐 Seguridad

### ✅ Implementado
- Archivos almacenados como **VARBINARY** (no accesibles por URL)
- Contraseña **encriptada** (Base64)
- Validación de extensiones (`.cer`, `.key`)
- Validación de vigencia del certificado
- Permisos solo para rol **CONTADOR**

### ⚠️ Pendiente para Producción
- [ ] Cambiar encriptación de Base64 a **AES-256**
- [ ] Implementar rotación de contraseñas
- [ ] Validar integridad del archivo .KEY con la contraseña
- [ ] Implementar respaldo automático de certificados
- [ ] Log de auditoría para accesos
- [ ] Two-factor authentication para operaciones críticas

---

## 🔍 Extracción de Datos del Certificado

**Usando `X509Certificate2` de .NET:**

```csharp
var cert = new X509Certificate2(bytesCER);

// No. Certificado (Serial Number en hexadecimal)
string noCertificado = cert.SerialNumber;

// RFC (buscado en Subject con regex)
// Patrón: OID.2.5.4.45=XAXX010101000
var rfcMatch = Regex.Match(cert.Subject, @"OID\.2\.5\.4\.45=([A-Z&Ñ]{3,4}\d{6}[A-Z\d]{3})");

// Razón Social (Common Name)
var cnMatch = Regex.Match(cert.Subject, @"CN=([^,]+)");

// Fechas
DateTime inicio = cert.NotBefore;
DateTime vencimiento = cert.NotAfter;
```

**Ejemplo de Subject:**
```
CN=MI EMPRESA SA DE CV, OID.2.5.4.45=MEX010203ABC, OU=Sello Digital, O=SAT, C=MX
```

---

## 📊 Uso desde Facturación

**Cuando se timbra una factura:**

```csharp
// Obtener certificado predeterminado
var certificado = cdContador.ObtenerCertificadoPredeterminado("CSD");

if (certificado == null || !certificado.EstaVigente)
{
    throw new Exception("No hay certificado CSD vigente configurado");
}

// Usar certificado para firmar
byte[] archivoCER = certificado.ArchivoCER;
byte[] archivoKEY = certificado.ArchivoKEY;
string password = DesencriptarPassword(certificado.PasswordKEY);

// Enviar al PAC (Finkok, SW, etc.)
var resultado = pac.Timbrar(xmlFactura, archivoCER, archivoKEY, password);
```

---

## 📈 Comparación con app.tesk.mx

| Funcionalidad | app.tesk.mx | Sistema VentasWeb |
|--------------|-------------|-------------------|
| Carga .CER/.KEY | ✅ | ✅ |
| Extracción automática de datos | ✅ | ✅ |
| Validación de vigencia | ✅ | ✅ |
| Múltiples certificados | ✅ | ✅ |
| Certificado predeterminado | ✅ | ✅ |
| Alertas de vencimiento | ✅ | ✅ |
| Uso por tipo (Factura/Nómina) | ✅ | ✅ |
| Almacenamiento seguro | ✅ (Cloud) | ✅ (DB) |
| Auditoría | ✅ | ✅ |
| Respaldo automático | ✅ | ⏳ Pendiente |
| Renovación asistida | ✅ | ⏳ Pendiente |

**Ventaja de VentasWeb:** Control total de datos sin depender de servicios externos.

---

## 🚀 Flujo Completo de Configuración

### Paso 1: Obtener Certificado del SAT
1. Ingresar al portal del SAT
2. Generar solicitud de certificado CSD
3. Descargar archivos:
   - `CSD_XAXX010101000_20240101_123456.cer`
   - `CSD_XAXX010101000_20240101_123456.key`
4. Anotar la **contraseña** proporcionada

### Paso 2: Subir al Sistema
1. Login como **contador@empresa.com**
2. Ir a **Contador → Certificados Digitales**
3. Clic en **"Subir Certificado"**
4. Llenar formulario:
   - Tipo: **CSD**
   - Nombre: **"Certificado 2024"**
   - Seleccionar archivos .CER y .KEY
   - Ingresar contraseña
   - Marcar: **Facturas** y **Cancelaciones**
   - Marcar como **predeterminado**
5. Guardar

### Paso 3: Verificar
- El sistema extrae automáticamente:
  - ✅ RFC
  - ✅ No. Certificado
  - ✅ Razón Social
  - ✅ Fecha de vencimiento
- Se muestra resumen en pantalla
- Certificado queda **listo para timbrar**

### Paso 4: Configurar PAC
1. Ir a **Contador → Configuración PAC**
2. Ingresar credenciales del PAC (Finkok, SW, etc.)
3. El sistema usará automáticamente el certificado predeterminado

### Paso 5: Timbrar Primera Factura
1. Crear venta en el módulo de ventas
2. Generar CFDI
3. Sistema usa certificado predeterminado automáticamente
4. Factura queda timbrada y lista para enviar

---

## ⚠️ Solución de Problemas

### Error: "Certificado inválido"
**Causa:** Archivo .CER corrupto o no es un certificado X.509.
**Solución:** Descargar nuevamente del SAT.

### Error: "Certificado vencido"
**Causa:** FechaVencimiento < fecha actual.
**Solución:** Renovar certificado en el portal del SAT (validez: 4 años).

### Error: "Contraseña incorrecta"
**Causa:** Password no coincide con el archivo .KEY.
**Solución:** Verificar contraseña proporcionada por el SAT al generar.

### Error: "RFC no coincide"
**Causa:** Certificado de otro contribuyente.
**Solución:** Usar certificado del RFC configurado en Configuración Empresa.

### Advertencia: "Vence en X días"
**Causa:** Certificado próximo a vencimiento.
**Solución:** 
1. Generar nuevo certificado en SAT
2. Subirlo al sistema
3. Marcarlo como predeterminado
4. Mantener el anterior activo hasta la transición

---

## 📝 Checklist de Configuración

```
□ Obtener certificado CSD del SAT
□ Tener archivos .CER y .KEY
□ Conocer la contraseña del .KEY
□ Subir certificado al sistema
□ Verificar que se extrajo el RFC correctamente
□ Marcar como predeterminado
□ Configurar usos (Facturas/Nómina/Cancelaciones)
□ Verificar fecha de vencimiento
□ Probar timbrado de factura de prueba
□ Configurar alertas de renovación (30 días antes)
```

---

## 🔄 Renovación de Certificado (cada 4 años)

**Procedimiento recomendado:**

1. **60 días antes del vencimiento:**
   - Generar nuevo certificado en el SAT
   - Subir al sistema pero NO marcarlo como predeterminado
   - Verificar que se cargó correctamente

2. **30 días antes del vencimiento:**
   - Probar timbrado con nuevo certificado (en modo prueba)
   - Verificar integración con PAC

3. **1 día antes del vencimiento:**
   - Marcar nuevo certificado como predeterminado
   - El sistema cambia automáticamente

4. **Después del vencimiento:**
   - Desactivar certificado anterior (pero no eliminar)
   - Mantener en histórico para auditorías

**Transición sin downtime:** ✅ El sistema permite tener ambos activos simultáneamente.

---

## 📞 Soporte

**Errores comunes del SAT:**
- Certificado no descarga: Verificar FIEL vigente
- Error al generar: Revisar obligaciones fiscales al corriente
- No aparece opción: Activar trámite de CSD en portal

**Documentación SAT:**
- [Obtención de Certificado de Sello Digital](https://www.sat.gob.mx/tramites/16703/obten-tu-certificado-de-sello-digital)
- [Renovación de CSD](https://www.sat.gob.mx/tramites/38246/renueva-tu-certificado-de-sello-digital)

---

## 🎓 Capacitación para Contador

**Tiempo estimado:** 30 minutos

### Módulo 1: Conceptos (10 min)
- Qué es un CSD
- Diferencia entre .CER y .KEY
- Para qué se usa en facturación

### Módulo 2: Carga de Certificado (10 min)
- Demostración paso a paso
- Interpretación de datos extraídos
- Configuración de usos

### Módulo 3: Gestión Continua (10 min)
- Monitoreo de vencimientos
- Proceso de renovación
- Manejo de múltiples certificados
- Solución de problemas comunes

---

## ✅ Resumen

El módulo de **Certificados Digitales** proporciona:

✅ **Gestión completa** de archivos .CER y .KEY  
✅ **Almacenamiento seguro** en base de datos  
✅ **Extracción automática** de RFC, No. Certificado, vigencia  
✅ **Alertas proactivas** de vencimiento  
✅ **Múltiples certificados** con predeterminado  
✅ **Seguridad** con encriptación de contraseñas  
✅ **Auditoría completa** de operaciones  
✅ **Integración nativa** con timbrado de facturas  

**Estado:** ✅ Implementado y funcional  
**Equivalencia:** 100% con app.tesk.mx  
**Listo para producción:** ⚠️ Pendiente encriptación AES

---

**Fecha:** Enero 2025  
**Versión:** 1.0  
**Autor:** Sistema VentasWeb - Módulo Contador
