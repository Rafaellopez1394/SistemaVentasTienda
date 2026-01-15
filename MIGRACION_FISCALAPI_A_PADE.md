# 🚀 Migración de FiscalAPI a PADE (Prodigia)

## 📋 Información General

**Sistema actual**: FiscalAPI (a eliminar)  
**Nuevo PAC**: PADE (Prodigia)  
**Ambiente ACTUAL**: https://pruebas.pade.mx ⚠️ **SOLO PRUEBAS POR AHORA**  
**Ambiente de Producción**: https://timbrado.pade.mx *(no se usará por el momento)*  
**Documentación API**: https://docs.prodigia.com.mx/api-timbrado-xml.html#servicio_rest

---

## ✅ Cambios Implementados

### 1. **Código Backend**
- ✅ Clase `ProdigiaService` implementada ([CapaDatos/PAC/ProdigiaService.cs](CapaDatos/PAC/ProdigiaService.cs))
- ✅ Modelos de datos Prodigia ([CapaDatos/PAC/ProdigiaModels.cs](CapaDatos/PAC/ProdigiaModels.cs))
- ✅ Configuración Prodigia ([CapaModelo/ConfiguracionProdigia.cs](CapaModelo/ConfiguracionProdigia.cs))
- ✅ Generador de XML CFDI 4.0 actualizado ([CapaDatos/Generadores/CFDI40XMLGenerator.cs](CapaDatos/Generadores/CFDI40XMLGenerator.cs))
- ✅ Métodos de timbrado en `CD_Factura` actualizados

### 2. **Base de Datos**
- ✅ Tabla `ConfiguracionProdigia` creada
- ✅ Script de configuración: [CONFIGURAR_PADE_PRODIGIA.sql](CONFIGURAR_PADE_PRODIGIA.sql)

### 3. **URLs Configuradas**
```csharp
// En ConfiguracionProdigia.cs - Propiedad UrlApi
public string UrlApi
{
    get
    {
        if (Ambiente == "TEST")
            return "https://pruebas.pade.mx/";
        else
            return "https://timbrado.pade.mx/";
    }
}
```

---

## 🔧 Pasos para Configurar

### **Paso 1: Ejecutar Script de Base de Datos**

```sql
-- Ejecutar desde SQL Server Management Studio
USE DB_TIENDA
GO

-- Ejecutar el archivo completo:
CONFIGURAR_PADE_PRODIGIA.sql
```

Este script:
- Crea la tabla `ConfiguracionProdigia` si no existe
- Desactiva FiscalAPI
- Inserta configuración de prueba
- Muestra instrucciones detalladas

### **Paso 2: Obtener Credenciales de PADE (Ambiente de Pruebas)**

1. **Solicitar acceso al ambiente de pruebas**:
   - URL de pruebas: https://pruebas.pade.mx
   - Contacto: soporte@pade.mx
   - Solicitar credenciales de **webservice de pruebas**

2. **Recibirás credenciales de prueba**:
   - Usuario de webservice (para pruebas)
   - Contraseña de webservice (para pruebas)
   - Código de contrato (para pruebas)

⚠️ **IMPORTANTE**: Estas son credenciales de **PRUEBAS**, no de producción

### **Paso 3: Subir Certificados CSD al Portal de Pruebas**

1. Ingresar al portal de pruebas: https://pruebas.pade.mx
2. Ir a: **Configuración → Certificados**
3. Subir archivos de prueba:
   - `.CER` (certificado de prueba o real)
   - `.KEY` (llave privada de prueba o real)
4. Ingresar contraseña de la llave
5. **PADE los almacenará y usará automáticamente** (modo CERT_DEFAULT)

⚠️ **Puedes usar certificados CSD reales en pruebas sin riesgo**

### **Paso 4: Actualizar Credenciales en la BD**

```sql
USE DB_TIENDA
GO

UPDATE ConfiguracionProdigia
SET 
    Usuario = 'tu_usuario_webservice',       -- Usuario real de PADE
    Password = 'tu_password_webservice',     -- Contraseña real
    Contrato = 'tu_codigo_contrato',         -- Código de contrato
    RfcEmisor = 'ABC123456XYZ',              -- RFC de tu empresa
    NombreEmisor = 'TU EMPRESA SA DE CV',    -- Razón social
    CodigoPostal = '12345',                  -- CP de expedición
    RegimenFiscal = '601',                   -- Tu régimen fiscal
    Ambiente = 'TEST',                       -- TEST o PRODUCCION
    FechaModificacion = GETDATE()
WHERE ConfiguracionID = 1

-- Verificar
SELECT * FROM ConfiguracionProdigia WHERE ConfiguracionID = 1
```

### **Paso 5: Probar Timbrado**

1. Ir al módulo de **Facturación** en el sistema web
2. Crear una factura de prueba
3. El sistema usará PADE automáticamente
4. Verificar que se genere el UUID y XML timbrado

---

## 📌 Endpoints de la API PADE

### **Timbrado de CFDI**
```
POST https://pruebas.pade.mx/servicio/rest/timbrado40/timbrarCfdi?contrato=TU_CONTRATO
Authorization: Basic [usuario:password en Base64]
Content-Type: application/json

{
  "xmlaCFDI": "PD94bW....", // XML pre-firmado en Base64
  "opciones": {
    "CALCULAR_SELLO": true,
    "CERT_DEFAULT": true,
    "ESTABLECER_NO_CERTIFICADO": true
  }
}
```

### **Cancelación de CFDI**
```
POST https://pruebas.pade.mx/servicio/rest/cancelacion/cancelarCfdi?contrato=TU_CONTRATO
Authorization: Basic [usuario:password en Base64]
Content-Type: application/json

{
  "rfcEmisor": "ABC123456XYZ",
  "uuid": "12345678-1234-1234-1234-123456789012",
  "motivo": "02",
  "uuidSustitucion": ""
}
```

---

## 🔄 Diferencias entre FiscalAPI y PADE

| Característica | FiscalAPI | PADE (Prodigia) |
|---------------|-----------|-----------------|
| **Tipo de API** | REST con SDK | REST Nativa |
| **Autenticación** | API Key + Tenant | Basic Auth (usuario:password) |
| **Certificados** | Se envían en cada request | CERT_DEFAULT en portal |
| **Formato Request** | JSON | JSON |
| **Formato Response** | JSON | XML en Base64 |
| **Documentación** | SDK específico | API REST estándar |
| **Código .NET** | Requiere SDK externo | HttpClient nativo |

---

## 🗂️ Archivos Clave del Sistema

### **Backend (C#)**
- [CapaDatos/PAC/ProdigiaService.cs](CapaDatos/PAC/ProdigiaService.cs) - Cliente HTTP para PADE
- [CapaDatos/PAC/ProdigiaModels.cs](CapaDatos/PAC/ProdigiaModels.cs) - Modelos de request/response
- [CapaDatos/CD_Factura.cs](CapaDatos/CD_Factura.cs) - Lógica de facturación (línea 936+)
- [CapaDatos/Generadores/CFDI40XMLGenerator.cs](CapaDatos/Generadores/CFDI40XMLGenerator.cs) - Generador XML
- [CapaModelo/ConfiguracionProdigia.cs](CapaModelo/ConfiguracionProdigia.cs) - Modelo de configuración

### **Base de Datos**
- [CONFIGURAR_PADE_PRODIGIA.sql](CONFIGURAR_PADE_PRODIGIA.sql) - Script de configuración
- [CONFIGURAR_PRODIGIA.sql](CONFIGURAR_PRODIGIA.sql) - Script original

### **Frontend (Pendiente actualizar)**
- `VentasWeb/Views/CertificadoDigital/Index.cshtml` - Eliminar referencias a FiscalAPI
- `VentasWeb/Controllers/CertificadoDigitalController.cs` - Actualizar controlador

---

## ⚠️ Tareas Pendientes

### 1. **Eliminar Referencias a FiscalAPI**
- [ ] Buscar y eliminar código de `FiscalAPIDirectHTTP.cs`
- [ ] Buscar y eliminar código de `FiscalAPIPAC.cs`
- [ ] Actualizar vistas que mencionen FiscalAPI
- [ ] Eliminar tabla `ConfiguracionFiscalAPI` (opcional)

### 2. **Actualizar Frontend**
- [ ] Eliminar botón "Subir a FiscalAPI" en certificados
- [ ] Eliminar tab "FiscalAPI" en vista de certificados
- [ ] Actualizar textos de ayuda/tooltips

### 3. **Pruebas Completas**
- [ ] Timbrado de factura normal
- [ ] Cancelación de factura
- [ ] Manejo de errores
- [ ] Validación de certificados

---

## 🆘 Solución de Problemas

### **Error: "No se encontró configuración de Prodigia"**
```sql
-- Verificar que existe configuración activa
SELECT * FROM ConfiguracionProdigia WHERE Activo = 1

-- Si no existe, ejecutar CONFIGURAR_PADE_PRODIGIA.sql
```

### **Error: "Credenciales inválidas"**
- Verificar usuario y contraseña en la BD
- Confirmar que las credenciales son de webservice (no del portal)
- Contactar a soporte@pade.mx

### **Error: "Certificado inválido"**
- Asegurarse de haber subido certificados al portal PADE
- Verificar que no estén vencidos (duran 4 años)
- Verificar que la contraseña de la llave sea correcta

### **Error: "Contrato no válido"**
- Confirmar el código de contrato con PADE
- Verificar que esté activo y con saldo

---

## 📞 Contactos y Recursos

**PADE (Prodigia) - Ambiente de Pruebas**
- Portal de pruebas: https://pruebas.pade.mx
- Documentación API: https://docs.prodigia.com.mx/
- Soporte: soporte@pade.mx
- ⚠️ **Actualmente usando SOLO ambiente de pruebas**

**SAT (Certificados CSD)**
- Portal: https://www.sat.gob.mx
- Sección CSD: https://www.sat.gob.mx/tramites/16703/obten-tu-certificado-de-sello-digital
- Atención telefónica: 55 627 22 728

---

## ✅ Checklist de Migración

- [x] Implementar clases de Prodigia (ProdigiaService, ProdigiaModels)
- [x] Crear tabla ConfiguracionProdigia
- [x] Configurar URLs de ambiente (TEST/PRODUCCION)
- [x] Crear script de configuración SQL
- [x] Documentar proceso de migración
- [ ] Obtener credenciales de PADE
- [ ] Actualizar credenciales en BD
- [ ] Subir certificados CSD al portal PADE
- [ ] Probar timbrado en TEST
- [ ] Eliminar código de FiscalAPI
- [ ] Actualizar frontend
- [ ] Probar en PRODUCCION

---

## 📝 Notas Adicionales

### **Opción CERT_DEFAULT**
PADE permite dos métodos para manejar certificados:

1. **CERT_DEFAULT** (Recomendado):
   - Subes certificados al portal PADE una vez
   - PADE los almacena de forma segura
   - En cada request solo envías: `"CERT_DEFAULT": true`
   - Más simple y seguro

2. **Enviar certificados en cada request**:
   - Almacenar CertificadoBase64 y LlavePrivadaBase64 en BD
   - Enviar en cada petición
   - Más pesado pero funciona sin portal

**El sistema actual usa CERT_DEFAULT por defecto.**

### **Régimen Fiscal (Catálogo SAT)**
- `601`: General de Ley Personas Morales
- `603`: Personas Morales con Fines no Lucrativos
- `605`: Sueldos y Salarios
- `606`: Arrendamiento
- `612`: Personas Físicas con Actividades Empresariales
- `621`: Régimen de Incorporación Fiscal (RIF)
- `625`: Régimen de las Actividades Empresariales con ingresos
- `626`: Régimen Simplificado de Confianza

Consulta el catálogo completo del SAT para más opciones.

---

**Documento creado**: 2026-01-14  
**Última actualización**: 2026-01-14  
**Versión**: 1.0
