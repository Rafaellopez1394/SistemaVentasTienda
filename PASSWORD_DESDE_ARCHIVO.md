# ✅ Sistema Configurado: Password desde Archivo

## 🎯 Cambio Implementado

El sistema ahora lee el **password del certificado desde un archivo** en lugar de la base de datos. Esto mejora la seguridad y facilita la gestión de certificados.

## 📁 Estructura en CapaDatos/Certifies/

```
CapaDatos/Certifies/
├── .gitignore                    (protege certificados)
├── README.md                     (documentación)
├── EKU9003173C9.cer             (certificado - pendiente)
├── EKU9003173C9.key             (llave privada - pendiente)
└── password                      (✅ creado con: 12345678a)
```

## ✅ Estado Actual

✅ **Base de datos actualizada:**
- `NombreArchivoCertificado = 'EKU9003173C9.cer'`
- `NombreArchivoLlavePrivada = 'EKU9003173C9.key'`
- `NombreArchivoPassword = 'password'`

✅ **Código compilado:** Sin errores

✅ **Archivo password creado:** Contiene `12345678a`

⏳ **Pendiente:** Copiar archivos `.cer` y `.key` a la carpeta

## 📋 Para Completar la Configuración

### Opción 1: Certificados de Prueba (FiscalAPI)

```powershell
# Descargar automáticamente
.\DESCARGAR_CERTIFICADOS_PRUEBA.ps1

# O manualmente:
# 1. Descargar: https://test.fiscalapi.com/files/tax-files/tax-files.zip
# 2. Extraer EKU9003173C9.cer y EKU9003173C9.key
# 3. Copiar a: CapaDatos\Certifies\
```

### Opción 2: Tus Certificados Reales

```powershell
# Copiar tus archivos
Copy-Item "ruta\tu_certificado.cer" "CapaDatos\Certifies\"
Copy-Item "ruta\tu_llave.key" "CapaDatos\Certifies\"

# Si tu password es diferente, editar el archivo
"tu_password_real" | Out-File "CapaDatos\Certifies\password" -NoNewline -Encoding ASCII

# Actualizar nombres en BD
sqlcmd -S "SISTEMAS\SERVIDOR" -d DB_TIENDA -E -Q "UPDATE ConfiguracionEmpresa SET NombreArchivoCertificado='tu_certificado.cer', NombreArchivoLlavePrivada='tu_llave.key' WHERE ConfigEmpresaID=1"
```

## 🔐 Seguridad Mejorada

### ✅ Ventajas del Password en Archivo:

1. **No en base de datos** - El password no se almacena en SQL
2. **Fácil rotación** - Solo editar el archivo `password`
3. **Protegido por .gitignore** - No se sube a Git
4. **Permisos de archivo** - Control a nivel de sistema operativo

### 📝 Formato del Archivo `password`:

- **Tipo:** Archivo de texto plano sin extensión
- **Contenido:** Solo el password, sin espacios ni saltos de línea extra
- **Ejemplo:** `12345678a`

```powershell
# Ver contenido
Get-Content "CapaDatos\Certifies\password"

# Resultado: 12345678a
```

## 🔄 Flujo de Lectura

```
1. CD_Factura lee nombres de archivos desde ConfiguracionEmpresa
   ↓ NombreArchivoCertificado = "EKU9003173C9.cer"
   ↓ NombreArchivoLlavePrivada = "EKU9003173C9.key"
   ↓ NombreArchivoPassword = "password"

2. FiscalAPIPAC.TimbrarConCertificadosDesdeArchivosAsync()
   ↓ Busca archivos en CapaDatos/Certifies/
   ↓ Lee EKU9003173C9.cer → bytes → Base64
   ↓ Lee EKU9003173C9.key → bytes → Base64
   ↓ Lee password → texto → "12345678a"

3. Crea Invoice con TaxCredentials
   ↓ Envía a FiscalAPI con certificados y password

4. FiscalAPI valida y timbra
```

## 📊 Verificación

### Verificar archivos existentes:

```powershell
Get-ChildItem "CapaDatos\Certifies\" -File
```

**Resultado esperado:**
```
.gitignore           (147 bytes)
password             (10 bytes)   ✅
README.md            (2.4 KB)
EKU9003173C9.cer     (~1.6 KB)   ⏳ Pendiente
EKU9003173C9.key     (~1.3 KB)   ⏳ Pendiente
```

### Verificar base de datos:

```sql
SELECT 
    RFC,
    NombreArchivoCertificado,
    NombreArchivoLlavePrivada,
    NombreArchivoPassword
FROM ConfiguracionEmpresa
WHERE ConfigEmpresaID = 1;
```

**Resultado esperado:**
```
RFC            Certificado          Llave               Password
-------------  -------------------  ------------------  --------
GAMA6111156JA  EKU9003173C9.cer    EKU9003173C9.key    password
```

## 🚀 Siguiente Acción

```powershell
# 1. Copiar certificados (elige una opción)
# Opción A - Automático (prueba):
.\DESCARGAR_CERTIFICADOS_PRUEBA.ps1

# Opción B - Manual (reales):
Copy-Item "C:\ruta\certificado.cer" "CapaDatos\Certifies\"
Copy-Item "C:\ruta\llave.key" "CapaDatos\Certifies\"

# 2. Verificar
Get-ChildItem "CapaDatos\Certifies\*.cer"
Get-ChildItem "CapaDatos\Certifies\*.key"
Get-Content "CapaDatos\Certifies\password"

# 3. Reiniciar aplicación
# F5 en Visual Studio

# 4. Probar facturación
# POST http://localhost:64927/Factura/GenerarFactura
```

## 🔍 Debug

**Logs en Visual Studio Output:**
```
Buscando certificados en: C:\...\CapaDatos\Certifies
Certificado: C:\...\CapaDatos\Certifies\EKU9003173C9.cer
Llave privada: C:\...\CapaDatos\Certifies\EKU9003173C9.key
Archivo password: C:\...\CapaDatos\Certifies\password
Certificado cargado: 1618 bytes
Llave privada cargada: 1298 bytes
Password leído desde archivo
```

## 📝 Archivos Modificados

### Base de Datos:
- ✅ Agregada columna `NombreArchivoPassword`
- ✅ Eliminada columna `PasswordCertificado`

### Código:
- ✅ `CapaModelo/ConfiguracionEmpresa.cs` - NombreArchivoPassword
- ✅ `CapaModelo/Factura.cs` - EmisorNombreArchivoPassword
- ✅ `CapaDatos/CD_Factura.cs` - Lee NombreArchivoPassword desde BD
- ✅ `CapaDatos/PAC/FiscalAPIPAC.cs` - Lee password desde archivo

### Archivos Nuevos:
- ✅ `CapaDatos/Certifies/password` - Contiene `12345678a`
- ✅ `CapaDatos/Certifies/.gitignore` - Protege password*

---

**✨ Sistema actualizado para leer password desde archivo `CapaDatos/Certifies/password`**
