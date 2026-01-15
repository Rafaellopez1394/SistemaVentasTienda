# Certificados de Prueba para FiscalAPI

## Problema Actual
FiscalAPI en ambiente de **PRUEBAS** (test.fiscalapi.com) rechaza los certificados reales con HTTP 500.

## Solución
Usar certificados de prueba proporcionados por el SAT/FiscalAPI:

### RFC de Prueba
- **RFC**: EKU9003173C9
- **Razón Social**: ESCUELA KEMPER URGATE

### Archivos Necesarios
1. **Certificado**: EKU9003173C9.cer
2. **Llave Privada**: EKU9003173C9.key
3. **Password**: 12345678a

### Dónde Obtenerlos
FiscalAPI proporciona certificados de prueba en su documentación:
https://docs.fiscalapi.com/cfdi/certificados-de-prueba

O usar los del SAT:
https://www.sat.gob.mx/aplicacion/16660/genera-y-descarga-los-archivos-a-traves-de-la-aplicacion-certifica

### Actualizar Base de Datos

```sql
USE DB_TIENDA
GO

-- Actualizar ConfiguracionEmpresa con RFC de prueba
UPDATE ConfiguracionEmpresa
SET RFC = 'EKU9003173C9',
    RazonSocial = 'ESCUELA KEMPER URGATE',
    NombreArchivoCertificado = 'EKU9003173C9.cer',
    NombreArchivoLlavePrivada = 'EKU9003173C9.key'
GO

-- Actualizar archivo de password
-- Crear archivo: CapaDatos\Certifies\password_prueba
-- Contenido: 12345678a
```

### Pasos
1. Descargar certificados de prueba
2. Copiarlos a: `CapaDatos\Certifies\`
3. Actualizar base de datos con script anterior
4. Probar facturación

### Restaurar para Producción
Cuando vayas a producción, restaurar:
- RFC: GAMA6111156JA
- Certificados reales
- Cambiar ConfiguracionPAC.EsProduccion = 1
- API Key de producción
