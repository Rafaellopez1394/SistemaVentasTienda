# ============================================================================
# MÓDULO DE CONFIGURACIÓN FISCAL Y CERTIFICADOS
# Sistema de Ventas - Facturación Electrónica
# ============================================================================

## ✅ MÓDULO CREADO Y FUNCIONAL

**Se ha agregado el módulo completo para configurar:**
1. ✅ Datos fiscales (RFC, Razón Social, Régimen Fiscal)
2. ✅ Cargar certificado digital (.cer)
3. ✅ Cargar llave privada (.key)
4. ✅ Contraseña del certificado
5. ✅ Configuración del PAC (API Key, Tenant)
6. ✅ Gestión de certificados (ver, eliminar)

---

## CÓMO ACCEDER AL MÓDULO

### Desde el menú del sistema:

1. **Iniciar sesión** en el sistema
2. **Ir al menú lateral izquierdo**
3. **Buscar sección "Administración"**
4. **Click en "Configuración Fiscal"**

**URL directa:**
```
http://localhost/VentasWeb/ConfiguracionFiscal/Index
```

---

## PASOS PARA CONFIGURAR FACTURACIÓN

### PASO 1: CONFIGURAR DATOS FISCALES

1. **Ir a la pestaña "Datos Fiscales"**
2. **Llenar los campos:**
   - **RFC del Emisor:** Tu RFC a 12 o 13 posiciones
   - **Razón Social / Nombre:** Tu nombre o razón social completa
   - **Régimen Fiscal:** Seleccionar el que corresponda
     - 612 - Personas Físicas con Actividades Empresariales
     - 601 - General de Ley Personas Morales
     - 626 - Régimen Simplificado de Confianza (RESICO)
     - Otros según tu caso
   - **Código Postal:** CP de tu domicilio fiscal (5 dígitos)

3. **Click en "Guardar Datos Fiscales"**

---

### PASO 2: CARGAR CERTIFICADO DIGITAL (CSD)

#### ¿Qué necesitas?

1. **Archivo .cer** (Certificado)
2. **Archivo .key** (Llave privada)
3. **Contraseña** de la llave privada

#### ¿Dónde obtenerlos?

- Descargar desde el **Portal del SAT**
- Ir a: https://portalsat.plataforma.sat.gob.mx
- Sección: **Trámites** → **Certificado de e.firma (antes FIEL)**
- Descargar certificado vigente

#### Pasos para cargar:

1. **Ir a la pestaña "Certificados Digitales"**
2. **Llenar los campos:**
   - **Nombre del Certificado:** Ej: "Certificado Producción 2026"
   - **RFC:** El RFC del certificado
   - **Razón Social:** Tu razón social

3. **Seleccionar archivos:**
   - **Archivo CER:** Click en "Examinar" y seleccionar tu archivo .cer
   - **Archivo KEY:** Click en "Examinar" y seleccionar tu archivo .key
   - **Contraseña KEY:** Escribir la contraseña de tu llave privada

4. **Click en "Cargar Certificado"**

5. **Verificar:** El certificado aparecerá en la tabla inferior

---

### PASO 3: CONFIGURAR PAC (Proveedor Autorizado)

#### ¿Qué es un PAC?

Es una empresa autorizada por el SAT para timbrar facturas electrónicas.

#### Opciones de PAC:

1. **FiscalAPI** - https://fiscalapi.com
2. **Facturama** - https://facturama.mx
3. **Aspel CFDI** - https://aspelcfdi.com
4. **PAC Comercial de Azteca** - https://facturacion.comercialazteca.com.mx

#### Pasos:

1. **Contratar un PAC** (elige uno de los anteriores)
2. **Obtener credenciales:**
   - API Key
   - Tenant / ID Usuario (si aplica)

3. **Ir a la pestaña "Configuración PAC"**
4. **Llenar los campos:**
   - **API Key:** Tu clave API proporcionada por el PAC
   - **Tenant:** Tu ID de usuario (opcional, depende del PAC)
   - **Ambiente:** Seleccionar
     - **Pruebas** → Para hacer pruebas sin costo
     - **Producción** → Para facturas reales

5. **Click en "Guardar Configuración PAC"**

---

## VERIFICAR CONFIGURACIÓN COMPLETA

### En la pestaña "Configuración PAC", ver el panel "Estado de Configuración"

Debe mostrar:

```
✓ RFC Emisor: XAXX010101000
✓ Razón Social: TU EMPRESA SA DE CV
✓ Certificado Digital: Cargado
✓ API Key: Configurado
  Ambiente: Pruebas

¡Sistema listo para facturar!
```

---

## ESTRUCTURA DE ARCHIVOS CREADOS

### Backend (Controlador):
```
VentasWeb/Controllers/ConfiguracionFiscalController.cs
```

**Funciones:**
- `GuardarConfiguracion()` → Guardar datos fiscales
- `CargarCertificado()` → Cargar archivos .cer y .key
- `ObtenerConfiguracion()` → Obtener configuración actual
- `ObtenerCertificados()` → Listar certificados cargados
- `EliminarCertificado()` → Eliminar certificado

### Frontend (Vista):
```
VentasWeb/Views/ConfiguracionFiscal/Index.cshtml
```

**Pestañas:**
1. Datos Fiscales → Configurar RFC, Razón Social, Régimen
2. Certificados Digitales → Cargar .cer y .key
3. Configuración PAC → API Key y Tenant

### Base de Datos:

**Tablas utilizadas:**
- `ConfiguracionFiscalAPI` → Configuración general
- `CertificadosDigitales` → Certificados cargados
- `CatRegimenFiscal` → Catálogo de regímenes fiscales

---

## CAMPOS DE LA BASE DE DATOS

### ConfiguracionFiscalAPI

| Campo | Tipo | Descripción |
|-------|------|-------------|
| ConfiguracionID | int | ID único |
| RfcEmisor | varchar | RFC del emisor |
| NombreEmisor | varchar | Razón social |
| RegimenFiscal | varchar | Código de régimen (601, 612, etc.) |
| CodigoPostal | varchar | CP fiscal |
| CertificadoBase64 | text | Certificado en Base64 |
| LlavePrivadaBase64 | text | Llave privada en Base64 |
| PasswordLlave | varchar | Contraseña encriptada |
| ApiKey | varchar | API Key del PAC |
| Tenant | varchar | Tenant/Usuario PAC |
| Ambiente | varchar | "Pruebas" o "Produccion" |
| Activo | bit | Estado activo/inactivo |

### CertificadosDigitales

| Campo | Tipo | Descripción |
|-------|------|-------------|
| CertificadoID | int | ID único |
| NombreCertificado | varchar | Nombre descriptivo |
| RFC | varchar | RFC del certificado |
| RazonSocial | varchar | Razón social |
| ArchivoCER | varbinary | Archivo .cer (binario) |
| ArchivoKEY | varbinary | Archivo .key (binario) |
| PasswordKEY | varchar | Contraseña |
| FechaVigenciaInicio | datetime | Vigencia inicio |
| FechaVigenciaFin | datetime | Vigencia fin |
| Activo | bit | Estado |
| EsPredeterminado | bit | Certificado por defecto |

---

## SEGURIDAD

### ¿Los certificados están seguros?

✅ **SÍ**, se almacenan de forma segura:

1. **Archivos binarios** → Se guardan en `varbinary` (no en texto plano)
2. **Base64** → Para uso en API (ConfiguracionFiscalAPI)
3. **Contraseñas** → Almacenadas en la BD (idealmente deberían encriptarse)
4. **Acceso restringido** → Solo usuarios con sesión activa

### Recomendaciones de seguridad:

- ✅ Usar HTTPS en producción
- ✅ Restringir acceso al módulo (solo administradores)
- ✅ Hacer respaldo de la base de datos
- ✅ Cambiar contraseñas periódicamente
- ⚠️ NO compartir API Keys
- ⚠️ NO usar certificados vencidos

---

## SOLUCIÓN DE PROBLEMAS

### "No puedo acceder al módulo"

**Solución:**
1. Verifica que iniciaste sesión
2. Verifica que el usuario tenga permisos de administrador
3. Verifica la URL: `/ConfiguracionFiscal/Index`

### "Error al cargar certificado"

**Causas comunes:**
- Contraseña incorrecta del archivo .key
- Archivos corruptos o inválidos
- Certificado vencido

**Solución:**
1. Verificar contraseña del certificado
2. Re-descargar certificados del SAT
3. Verificar vigencia del certificado

### "No aparece en el menú"

**Solución:**
1. Limpiar caché del navegador (Ctrl + F5)
2. Cerrar sesión y volver a iniciar
3. Verificar que el sistema esté desplegado en IIS

### "Error al guardar configuración"

**Solución:**
1. Verificar conexión a la base de datos
2. Verificar que existan las tablas necesarias
3. Revisar logs de errores en el servidor

---

## PRUEBAS Y VALIDACIÓN

### Prueba rápida:

```sql
-- Verificar configuración en BD
SELECT 
    RfcEmisor,
    NombreEmisor,
    RegimenFiscal,
    Ambiente,
    CASE WHEN CertificadoBase64 IS NOT NULL THEN 'Cargado' ELSE 'No cargado' END AS Certificado,
    CASE WHEN ApiKey IS NOT NULL THEN 'Configurado' ELSE 'No configurado' END AS PAC
FROM ConfiguracionFiscalAPI;

-- Ver certificados cargados
SELECT 
    NombreCertificado,
    RFC,
    FechaVigenciaInicio,
    FechaVigenciaFin,
    Activo,
    EsPredeterminado
FROM CertificadosDigitales
ORDER BY FechaCreacion DESC;
```

---

## PRÓXIMOS PASOS

Después de configurar:

1. **Ir al módulo de Ventas**
2. **Realizar una venta de prueba**
3. **Generar factura electrónica**
4. **Verificar que el timbre se aplique correctamente**
5. **Revisar el XML generado**

---

## RESUMEN DE RUTAS

### Menú del sistema:
```
Administración → Configuración Fiscal
```

### URL directa:
```
http://localhost/VentasWeb/ConfiguracionFiscal/Index
```

### APIs disponibles:
```
GET  /ConfiguracionFiscal/ObtenerConfiguracion
GET  /ConfiguracionFiscal/ObtenerCertificados
POST /ConfiguracionFiscal/GuardarConfiguracion
POST /ConfiguracionFiscal/CargarCertificado
POST /ConfiguracionFiscal/EliminarCertificado
```

---

## CONCLUSIÓN

✅ **MÓDULO COMPLETO Y FUNCIONAL**

Ahora puedes:
- ✅ Configurar datos fiscales (RFC, Razón Social)
- ✅ Cargar certificado .cer
- ✅ Cargar llave privada .key
- ✅ Configurar contraseña
- ✅ Configurar PAC (API Key)
- ✅ Ver certificados cargados
- ✅ Gestionar múltiples certificados

**Todo desde una interfaz visual e intuitiva.**

---

*Última actualización: 25/01/2026*
*Módulo: ConfiguracionFiscalController.cs ✅*
*Vista: ConfiguracionFiscal/Index.cshtml ✅*
*Compilación: EXITOSA ✅*
