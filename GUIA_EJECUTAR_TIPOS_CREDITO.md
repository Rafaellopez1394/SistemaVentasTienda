# Guía: Ejecutar Scripts SQL de Tipos de Crédito

## 📋 Prerequisitos

- SQL Server Management Studio (SSMS) instalado
- Base de datos `DB_TIENDA` existente
- Permisos de admin en la BD
- Conexión integrada (Integrated Security)

## 🔧 Paso 1: Abriendo SSMS

```
1. Abrir SQL Server Management Studio
2. Server name: . (punto = localhost)
3. Authentication: Windows Authentication
4. Connect
```

## 📂 Paso 2: Ejecutar Script de Tipos de Crédito

### Opción A: Interfaz Gráfica de SSMS

```
1. File → Open → File
2. Seleccionar: SQL Server/04_TiposCredito_Init.sql
3. Verify conexión: El dropdown debe mostrar "DB_TIENDA"
4. Execute (F5 o botón verde Execute)
5. Ver resultados en "Messages" tab
```

### Opción B: PowerShell

```powershell
cd "c:\Users\Rafael Lopez\Documents\SistemaVentasTienda\SQL Server"

sqlcmd -S . -d DB_TIENDA -i "04_TiposCredito_Init.sql"

# Esperado: Mensajes de confirmación
# ===============================================
# Inicialización de Tipos de Crédito Completada
# ===============================================
```

### Opción C: CMD

```cmd
cd "C:\Users\Rafael Lopez\Documents\SistemaVentasTienda\SQL Server"

sqlcmd -S . -d DB_TIENDA -i 04_TiposCredito_Init.sql

REM Esperar mensajes de éxito
```

## ✅ Verificación Post-Ejecución

### Verificar que las tablas se crearon:

```sql
-- En SSMS, ejecutar:
USE DB_TIENDA;

-- Ver tablas creadas
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_NAME IN ('TiposCredito', 'ClienteTiposCredito', 'HistorialCreditoCliente');

-- Resultado esperado:
-- TiposCredito
-- ClienteTiposCredito
-- HistorialCreditoCliente
```

### Verificar datos maestros:

```sql
-- Ver tipos de crédito insertados
SELECT * FROM TiposCredito;

-- Resultado esperado:
-- TipoCreditoID | Codigo | Nombre                  | Criterio | Activo
-- 1             | CR001  | Crédito por Dinero     | Dinero   | 1
-- 2             | CR002  | Crédito por Producto   | Producto | 1
-- 3             | CR003  | Crédito a Plazo        | Tiempo   | 1
```

### Verificar que no hay duplicados:

```sql
-- Contar registros
SELECT COUNT(*) FROM TiposCredito;
-- Debe retornar: 3

-- Verificar claves únicas
SELECT Codigo, COUNT(*) FROM TiposCredito GROUP BY Codigo;
-- Debe retornar: CR001=1, CR002=1, CR003=1
```

## 🔍 Troubleshooting

### Error: "Could not find stored procedure..."

```
SOLUCIÓN: El script crear los SPs. Si falla al crear:
1. Verificar que tengas permisos admin
2. Ejecutar script de nuevo
3. Los SPs son idempotentes (safe re-run)
```

### Error: "Invalid column name 'Criterio'..."

```
SOLUCIÓN: La tabla TiposCredito tiene nuevas columnas
1. Verificar que ejecutaste el script completo
2. No editaste la estructura de TiposCredito antes
```

### Error: "The INSERT statement conflicted with a UNIQUE KEY..."

```
SOLUCIÓN: Los maestros CR001, CR002, CR003 ya existen
- NORMAL: El script tiene validación IF NOT EXISTS
- Si persiste: Ejecutar DROP TABLE y reintentar
```

### Error: "Database 'DB_TIENDA' does not exist"

```
SOLUCIÓN: Crear primero la BD
1. Ejecutar: CREATE DATABASE DB_TIENDA;
2. Luego ejecutar el script de tipos de crédito
```

## 📊 Verificación Completa

Ejecutar este script para verificar todo:

```sql
USE DB_TIENDA;

PRINT '=== VERIFICACIÓN DE TIPOS DE CRÉDITO ===';

-- 1. Tablas
PRINT '';
PRINT '1. TABLAS CREADAS:';
SELECT 'TiposCredito' AS Tabla WHERE EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='TiposCredito')
UNION ALL
SELECT 'ClienteTiposCredito' WHERE EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='ClienteTiposCredito')
UNION ALL
SELECT 'HistorialCreditoCliente' WHERE EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='HistorialCreditoCliente');

-- 2. Datos Maestros
PRINT '';
PRINT '2. TIPOS DE CRÉDITO MAESTROS:';
SELECT TipoCreditoID, Codigo, Nombre, Criterio FROM TiposCredito ORDER BY Codigo;

-- 3. Procedimientos
PRINT '';
PRINT '3. PROCEDIMIENTOS CREADOS:';
SELECT 'SP_RegistrarHistorialCredito' AS Procedure WHERE EXISTS (SELECT 1 FROM sys.objects WHERE type='P' AND name='SP_RegistrarHistorialCredito')
UNION ALL
SELECT 'SP_ObtenerClientesEnAlerta' WHERE EXISTS (SELECT 1 FROM sys.objects WHERE type='P' AND name='SP_ObtenerClientesEnAlerta');

-- 4. Triggers
PRINT '';
PRINT '4. TRIGGERS CREADOS:';
SELECT 'TR_ClienteTiposCredito_CalcularVencimiento' AS Trigger WHERE EXISTS (SELECT 1 FROM sys.objects WHERE type='TR' AND name='TR_ClienteTiposCredito_CalcularVencimiento');

PRINT '';
PRINT '=== VERIFICACIÓN COMPLETADA ===';
```

## 🧪 Prueba Básica: Asignar Crédito

```sql
-- 1. Ver clientes existentes
SELECT TOP 5 ClienteID, RazonSocial FROM Clientes;

-- Copiar un ClienteID (por ej: 12345678-1234-1234-1234-123456789012)

-- 2. Asignar crédito por dinero a un cliente
INSERT INTO ClienteTiposCredito (ClienteID, TipoCreditoID, LimiteDinero, FechaAsignacion, Estatus)
VALUES ('12345678-1234-1234-1234-123456789012', 1, 10000.00, GETDATE(), 1);

-- 3. Verificar asignación
SELECT ctc.ClienteTipoCreditoID, c.RazonSocial, tc.Nombre, ctc.LimiteDinero, ctc.Estatus
FROM ClienteTiposCredito ctc
INNER JOIN Clientes c ON ctc.ClienteID = c.ClienteID
INNER JOIN TiposCredito tc ON ctc.TipoCreditoID = tc.TipoCreditoID
WHERE ctc.ClienteID = '12345678-1234-1234-1234-123456789012';

-- 4. Ver crédito a plazo (automático cálculo de vencimiento)
INSERT INTO ClienteTiposCredito (ClienteID, TipoCreditoID, PlazoDias, FechaAsignacion, Estatus)
VALUES ('12345678-1234-1234-1234-123456789012', 3, 30, GETDATE(), 1);

SELECT ClienteTipoCreditoID, FechaAsignacion, PlazoDias, FechaVencimiento
FROM ClienteTiposCredito
WHERE TipoCreditoID = 3 AND ClienteID = '12345678-1234-1234-1234-123456789012';

-- Resultado esperado: FechaVencimiento = HOY + 30 días
```

## 📝 Notas Importantes

### Sobre la Idempotencia del Script:

```sql
-- El script usa IF NOT EXISTS para evitar errores en re-ejecuciones
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'TiposCredito')
BEGIN
    CREATE TABLE TiposCredito ( ... )
END

-- Puedes ejecutar el script múltiples veces sin problemas
-- EXCEPTO: Si cambias datos maestros manualmente entre ejecuciones
```

### Sobre Permisos Necesarios:

```
- CREATE TABLE: Necesitas permisos DDL
- CREATE PROCEDURE: Necesitas permisos de objeto
- INSERT: Necesitas permisos DML
- Normalmente: Usar admin o acceso db_owner
```

### Backup Antes de Ejecutar:

```sql
-- Opcional pero recomendado:
BACKUP DATABASE DB_TIENDA TO DISK = 'C:\backup\DB_TIENDA_pre_creditos.bak';
```

## 🎯 Checklist de Ejecución

- [ ] Abrí SSMS y conecté a . (localhost)
- [ ] Seleccioné DB_TIENDA como base de datos
- [ ] Abrí el archivo 04_TiposCredito_Init.sql
- [ ] Ejecuté el script (F5)
- [ ] Ví mensajes de éxito en la ventana de salida
- [ ] Verifiqué que las 3 tablas existen
- [ ] Verifiqué que los 3 tipos maestros están inserados
- [ ] Ejecuté script de verificación sin errores
- [ ] Pude asignar un crédito de prueba a un cliente
- [ ] ✅ LISTO para usar en aplicación

## 📞 Soporte

Si falta algo:
1. Copiar error exacto
2. Ejecutar script de verificación
3. Validar que tengas permisos admin
4. Reintentar

Para eliminar y comenzar de cero:

```sql
-- CUIDADO: Esto elimina los datos de crédito
DROP TABLE IF EXISTS HistorialCreditoCliente;
DROP TABLE IF EXISTS ClienteTiposCredito;
DROP TABLE IF EXISTS TiposCredito;

-- Luego ejecutar el script de nuevo
```

---
**Guía completada.** Script de tipos de crédito está listo para ejecutar en BD_TIENDA.
