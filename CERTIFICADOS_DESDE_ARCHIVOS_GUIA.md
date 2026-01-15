# ✅ Certificados desde Archivos - Configuración Completada

## 🎯 Cambio Implementado

Se cambió el sistema para **leer certificados desde archivos** en la carpeta `CapaDatos/Certifies/` en lugar de Base64 desde la base de datos. Esto es más simple y seguro.

## 📁 Estructura de Archivos

```
SistemaVentasTienda/
└── CapaDatos/
    └── Certifies/              ← Carpeta de certificados
        ├── .gitignore          (seguridad)
        ├── README.md           (documentación)
        ├── tu_certificado.cer  (copiar aquí)
        └── tu_llave.key        (copiar aquí)
```

## 🚀 Guía de Configuración (3 Pasos)

### Paso 1: Copiar Certificados

**Opción A - Certificados de Prueba:**
```powershell
# Descargar de FiscalAPI
Invoke-WebRequest -Uri "https://test.fiscalapi.com/files/tax-files/tax-files.zip" -OutFile "tax-files.zip"
Expand-Archive -Path "tax-files.zip" -DestinationPath "temp"

# Copiar a la carpeta Certifies
Copy-Item "temp/EKU9003173C9.cer" "CapaDatos/Certifies/"
Copy-Item "temp/EKU9003173C9.key" "CapaDatos/Certifies/"
```

**Opción B - Tus Certificados Reales:**
```powershell
# Copiar tus archivos
Copy-Item "C:\ruta\tu_certificado.cer" "CapaDatos/Certifies/"
Copy-Item "C:\ruta\tu_llave.key" "CapaDatos/Certifies/"
```

### Paso 2: Configurar Base de Datos

Ya ejecutado ✅. La base de datos está configurada con:
- `NombreArchivoCertificado = 'EKU9003173C9.cer'`
- `NombreArchivoLlavePrivada = 'EKU9003173C9.key'`
- `PasswordCertificado = '12345678a'`

Si usas certificados diferentes, actualiza:
```sql
UPDATE ConfiguracionEmpresa
SET 
    NombreArchivoCertificado = 'tu_certificado.cer',
    NombreArchivoLlavePrivada = 'tu_llave.key',
    PasswordCertificado = 'tu_password'
WHERE ConfigEmpresaID = 1;
```

### Paso 3: Probar Facturación

```http
POST http://localhost:64927/Factura/GenerarFactura
Content-Type: application/json

{
    "VentaID": "6bc16123-7b85-418e-a4aa-62384726aa44",
    "ReceptorRFC": "XAXX010101000",
    "ReceptorNombre": "Público en General",
    "ReceptorRegimenFiscal": "616",
    "UsoCFDI": "S01",
    "FormaPago": "01",
    "MetodoPago": "PUE"
}
```

## 🔄 Cómo Funciona

### 1. Flujo de Lectura de Certificados

```
1. CD_Factura lee nombres de archivos desde ConfiguracionEmpresa
   ↓ NombreArchivoCertificado, NombreArchivoLlavePrivada, PasswordCertificado

2. FiscalAPIPAC.TimbrarConCertificadosDesdeArchivosAsync()
   ↓ Busca archivos en CapaDatos/Certifies/
   ↓ Lee archivos como bytes
   ↓ Convierte a Base64

3. FiscalAPIPAC.TimbrarConCertificadosAsync()
   ↓ Crea Invoice con TaxCredentials
   ↓ Envía a FiscalAPI

4. FiscalAPI timbra con SAT
```

### 2. Búsqueda de Certificados

El sistema busca los archivos en estas ubicaciones (en orden):

1. `bin/Certifies/` - Para producción con IIS
2. `CapaDatos/Certifies/` - Para desarrollo

### 3. Logs de Debug

Ver en Visual Studio Output → Debug:
```
Buscando certificados en: C:\...\CapaDatos\Certifies
Certificado: C:\...\CapaDatos\Certifies\EKU9003173C9.cer
Llave privada: C:\...\CapaDatos\Certifies\EKU9003173C9.key
Certificado cargado: 1234 bytes
Llave privada cargada: 2345 bytes
Timbrando con FiscalAPI en modo 'Por Valores' (certificados desde CapaDatos/Certifies)
```

## ✅ Verificar Configuración

### Verificar Archivos Existen

```powershell
# Listar archivos en Certifies
Get-ChildItem "CapaDatos\Certifies\" -File

# Resultado esperado:
# .gitignore
# README.md
# EKU9003173C9.cer
# EKU9003173C9.key
```

### Verificar Base de Datos

```sql
SELECT 
    RFC,
    NombreArchivoCertificado,
    NombreArchivoLlavePrivada,
    PasswordCertificado,
    CASE 
        WHEN NombreArchivoCertificado IS NOT NULL THEN '✓ Configurado'
        ELSE '✗ Falta configurar'
    END AS Estado
FROM ConfiguracionEmpresa
WHERE ConfigEmpresaID = 1;
```

## 🔐 Seguridad

### ✅ Implementado:
- `.gitignore` en Certifies/ para NO subir certificados a Git
- Archivos solo en servidor local
- Lectura con permisos de aplicación web

### ⚠️ Importante:
1. **NO** subir certificados a Git/repositorio
2. **NO** compartir las llaves privadas (.key)
3. Mantener backup de certificados en lugar seguro
4. Al renovar, reemplazar archivos y reiniciar aplicación

## 📦 Archivos Modificados

### Base de Datos:
- ✅ Eliminadas columnas `CertificadoBase64`, `LlavePrivadaBase64`
- ✅ Agregadas columnas `NombreArchivoCertificado`, `NombreArchivoLlavePrivada`
- ✅ Configurado con certificados de prueba EKU9003173C9

### Código:
- ✅ `CapaModelo/ConfiguracionEmpresa.cs` - Propiedades para nombres de archivos
- ✅ `CapaModelo/Factura.cs` - Propiedades para nombres de archivos
- ✅ `CapaDatos/CD_Factura.cs` - Lee nombres desde BD, pasa a Factura
- ✅ `CapaDatos/PAC/FiscalAPIPAC.cs` - Nuevo método `TimbrarConCertificadosDesdeArchivosAsync()`

### Scripts:
- ✅ `CONFIGURAR_CERTIFICADOS_DESDE_ARCHIVOS.sql` - Configuración BD
- ✅ `CapaDatos/Certifies/README.md` - Documentación
- ✅ `CapaDatos/Certifies/.gitignore` - Seguridad

## 🎯 Estado Actual

✅ **Base de datos configurada** con nombres de archivos  
✅ **Código compilado** sin errores  
✅ **Carpeta Certifies creada** en `CapaDatos/`  
✅ **Seguridad implementada** con .gitignore  
⏳ **Pendiente:** Copiar archivos .cer y .key a `CapaDatos/Certifies/`

## 🚀 Siguiente Acción

```powershell
# 1. Copiar certificados a la carpeta
Copy-Item "ruta_a_tu_certificado.cer" "CapaDatos\Certifies\"
Copy-Item "ruta_a_tu_llave.key" "CapaDatos\Certifies\"

# 2. Verificar que existen
Get-ChildItem "CapaDatos\Certifies\*.cer"
Get-ChildItem "CapaDatos\Certifies\*.key"

# 3. Reiniciar aplicación (F5 en Visual Studio)

# 4. Probar facturación
# POST http://localhost:64927/Factura/GenerarFactura
```

## 📞 Referencias

- Carpeta: [CapaDatos/Certifies/](CapaDatos/Certifies/)
- Script SQL: [CONFIGURAR_CERTIFICADOS_DESDE_ARCHIVOS.sql](CONFIGURAR_CERTIFICADOS_DESDE_ARCHIVOS.sql)
- Certificados Prueba: https://test.fiscalapi.com/files/tax-files/tax-files.zip
- Documentación FiscalAPI: https://docs.fiscalapi.com

---

**✨ Sistema actualizado para leer certificados desde archivos en `CapaDatos/Certifies/`**
