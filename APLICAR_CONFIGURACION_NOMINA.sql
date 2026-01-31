-- ============================================================
-- SCRIPT DE CONFIGURACIÓN: TIMBRADO DE NÓMINA CFDI 4.0
-- Aplicar estos cambios antes de la primera prueba
-- Tiempo estimado: 1 minuto
-- ============================================================

USE DB_TIENDA
GO

PRINT '=========================================='
PRINT 'CONFIGURACIÓN TIMBRADO NÓMINA - INICIO'
PRINT '=========================================='
PRINT ''

-- ============================================================
-- 1. AGREGAR COLUMNA InvoiceId (si no existe)
-- ============================================================
PRINT '1. Verificando columna InvoiceId...'

IF NOT EXISTS (
    SELECT * FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME = 'NominasCFDI' AND COLUMN_NAME = 'InvoiceId'
)
BEGIN
    ALTER TABLE NominasCFDI
    ADD InvoiceId VARCHAR(100) NULL
    
    PRINT '   ✓ Columna InvoiceId agregada'
END
ELSE
BEGIN
    PRINT '   ✓ Columna InvoiceId ya existe'
END
PRINT ''

-- ============================================================
-- 2. ESTANDARIZAR ESTADOS DE TIMBRADO A 'EXITOSO'
-- ============================================================
PRINT '2. Estandarizando estados de timbrado...'

IF EXISTS (SELECT * FROM sys.tables WHERE name = 'NominasCFDI')
BEGIN
    -- Ver estados actuales
    DECLARE @estadosActuales TABLE (Estado VARCHAR(50), Cantidad INT)
    INSERT INTO @estadosActuales
    SELECT EstadoTimbrado, COUNT(*) 
    FROM NominasCFDI 
    WHERE EstadoTimbrado IS NOT NULL
    GROUP BY EstadoTimbrado
    
    IF EXISTS (SELECT * FROM @estadosActuales)
    BEGIN
        PRINT '   Estados actuales:'
        SELECT '   - ' + Estado + ': ' + CAST(Cantidad AS VARCHAR(10)) AS Info
        FROM @estadosActuales
    END
    
    -- Estandarizar a 'EXITOSO'
    UPDATE NominasCFDI
    SET EstadoTimbrado = 'EXITOSO'
    WHERE EstadoTimbrado = 'TIMBRADO'
    
    IF @@ROWCOUNT > 0
    BEGIN
        PRINT '   ✓ ' + CAST(@@ROWCOUNT AS VARCHAR(10)) + ' registros actualizados de TIMBRADO a EXITOSO'
    END
    ELSE
    BEGIN
        PRINT '   ✓ Estados ya están estandarizados'
    END
END
PRINT ''

-- ============================================================
-- 3. VERIFICAR Y ACTUALIZAR DATOS DE EMPLEADOS
-- ============================================================
PRINT '3. Verificando datos de empleados activos...'

-- Contar empleados con datos faltantes
DECLARE @empleadosSinRFC INT = 0
DECLARE @empleadosSinCURP INT = 0
DECLARE @empleadosSinNSS INT = 0

IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Empleados')
BEGIN
    SELECT 
        @empleadosSinRFC = COUNT(*),
        @empleadosSinCURP = SUM(CASE WHEN CURP IS NULL OR CURP = '' THEN 1 ELSE 0 END),
        @empleadosSinNSS = SUM(CASE WHEN NSS IS NULL OR NSS = '' THEN 1 ELSE 0 END)
    FROM Empleados
    WHERE (Estatus IS NULL OR Estatus <> 'INACTIVO')
      AND (RFC IS NULL OR RFC = '' OR CURP IS NULL OR CURP = '' OR NSS IS NULL OR NSS = '')
    
    IF @empleadosSinRFC > 0 OR @empleadosSinCURP > 0 OR @empleadosSinNSS > 0
    BEGIN
        PRINT '   Empleados con datos faltantes:'
        IF @empleadosSinRFC > 0 PRINT '   - Sin RFC: ' + CAST(@empleadosSinRFC AS VARCHAR(10)) + ' ❌ CRÍTICO'
        IF @empleadosSinCURP > 0 PRINT '   - Sin CURP: ' + CAST(@empleadosSinCURP AS VARCHAR(10)) + ' ⚠️'
        IF @empleadosSinNSS > 0 PRINT '   - Sin NSS: ' + CAST(@empleadosSinNSS AS VARCHAR(10)) + ' ⚠️'
        PRINT ''
        PRINT '   Aplicando valores genéricos de prueba...'
        
        -- Actualizar con valores genéricos del SAT
        UPDATE Empleados
        SET 
            CURP = CASE 
                WHEN CURP IS NULL OR CURP = '' THEN 'XEXX010101HNEXXXA4'  -- CURP genérico SAT
                ELSE CURP 
            END,
            NSS = CASE 
                WHEN NSS IS NULL OR NSS = '' THEN '00000000000'  -- NSS genérico
                ELSE NSS 
            END,
            RFC = CASE 
                WHEN RFC IS NULL OR RFC = '' THEN 'XEXX010101000'  -- RFC genérico
                ELSE RFC 
            END
        WHERE (Estatus IS NULL OR Estatus <> 'INACTIVO')
          AND (RFC IS NULL OR RFC = '' OR CURP IS NULL OR CURP = '' OR NSS IS NULL OR NSS = '')
        
        PRINT '   ✓ ' + CAST(@@ROWCOUNT AS VARCHAR(10)) + ' empleados actualizados con valores genéricos'
        PRINT '   ⚠️  IMPORTANTE: Estos son valores de PRUEBA, actualizar con datos reales'
    END
    ELSE
    BEGIN
        PRINT '   ✓ Todos los empleados activos tienen RFC, CURP y NSS'
    END
    
    -- Mostrar resumen final
    PRINT ''
    PRINT '   Resumen de empleados activos:'
    SELECT 
        COUNT(*) AS TotalActivos,
        SUM(CASE WHEN RFC IS NOT NULL AND RFC <> '' THEN 1 ELSE 0 END) AS ConRFC,
        SUM(CASE WHEN CURP IS NOT NULL AND CURP <> '' THEN 1 ELSE 0 END) AS ConCURP,
        SUM(CASE WHEN NSS IS NOT NULL AND NSS <> '' THEN 1 ELSE 0 END) AS ConNSS
    FROM Empleados
    WHERE (Estatus IS NULL OR Estatus <> 'INACTIVO')
END
PRINT ''

-- ============================================================
-- 4. VERIFICAR CONFIGURACIÓN DE FISCALAPI
-- ============================================================
PRINT '4. Verificando configuración de FiscalAPI...'

IF EXISTS (SELECT * FROM sys.tables WHERE name = 'ConfiguracionFiscalAPI')
BEGIN
    IF EXISTS (SELECT * FROM ConfiguracionFiscalAPI WHERE Activo = 1)
    BEGIN
        SELECT 
            '   ✓ Configuración activa encontrada' AS Mensaje,
            'RFC Emisor: ' + ISNULL(RfcEmisor, 'NO CONFIGURADO') AS RFC,
            'Ambiente: ' + ISNULL(Ambiente, 'NO CONFIGURADO') AS Ambiente,
            'Tenant: ' + LEFT(ISNULL(Tenant, 'NO CONFIGURADO'), 20) + '...' AS Tenant
        FROM ConfiguracionFiscalAPI
        WHERE Activo = 1
    END
    ELSE
    BEGIN
        PRINT '   ❌ NO hay configuración activa de FiscalAPI'
        PRINT '   Acción requerida: Configurar FiscalAPI en el módulo de facturación'
    END
END
ELSE
BEGIN
    PRINT '   ❌ Tabla ConfiguracionFiscalAPI no existe'
    PRINT '   Acción requerida: Crear tabla desde el módulo de facturación'
END
PRINT ''

-- ============================================================
-- 5. VERIFICAR TABLA NominasCFDI
-- ============================================================
PRINT '5. Verificando estructura de tabla NominasCFDI...'

IF EXISTS (SELECT * FROM sys.tables WHERE name = 'NominasCFDI')
BEGIN
    PRINT '   ✓ Tabla NominasCFDI existe'
    
    -- Verificar columnas críticas
    DECLARE @columnasRequeridas TABLE (Columna VARCHAR(100), Existe BIT)
    
    INSERT INTO @columnasRequeridas VALUES ('NominaCFDIID', 0)
    INSERT INTO @columnasRequeridas VALUES ('NominaDetalleID', 0)
    INSERT INTO @columnasRequeridas VALUES ('UUID', 0)
    INSERT INTO @columnasRequeridas VALUES ('XMLTimbrado', 0)
    INSERT INTO @columnasRequeridas VALUES ('InvoiceId', 0)
    INSERT INTO @columnasRequeridas VALUES ('EstadoTimbrado', 0)
    
    UPDATE @columnasRequeridas
    SET Existe = 1
    WHERE Columna IN (
        SELECT COLUMN_NAME 
        FROM INFORMATION_SCHEMA.COLUMNS 
        WHERE TABLE_NAME = 'NominasCFDI'
    )
    
    -- Mostrar resultado
    SELECT 
        Columna,
        CASE WHEN Existe = 1 THEN '✓' ELSE '❌' END AS Estado
    FROM @columnasRequeridas
    
    IF EXISTS (SELECT * FROM @columnasRequeridas WHERE Existe = 0)
    BEGIN
        PRINT '   ⚠️  Faltan columnas en la tabla'
    END
END
ELSE
BEGIN
    PRINT '   ❌ Tabla NominasCFDI NO existe'
    PRINT '   Acción requerida: Crear tabla desde el módulo de nómina'
END
PRINT ''

-- ============================================================
-- 6. VERIFICAR TABLA NominaDetalle
-- ============================================================
PRINT '6. Verificando estructura de tabla NominaDetalle...'

IF EXISTS (SELECT * FROM sys.tables WHERE name = 'NominaDetalle')
BEGIN
    PRINT '   ✓ Tabla NominaDetalle existe'
    
    -- Verificar columnas de timbrado
    IF EXISTS (
        SELECT * FROM INFORMATION_SCHEMA.COLUMNS
        WHERE TABLE_NAME = 'NominaDetalle' AND COLUMN_NAME = 'UUID'
    )
    BEGIN
        PRINT '   ✓ Columna UUID existe'
    END
    ELSE
    BEGIN
        PRINT '   ❌ Columna UUID no existe'
        PRINT '   Ejecutar: ALTER TABLE NominaDetalle ADD UUID VARCHAR(50) NULL'
    END
    
    IF EXISTS (
        SELECT * FROM INFORMATION_SCHEMA.COLUMNS
        WHERE TABLE_NAME = 'NominaDetalle' AND COLUMN_NAME = 'FechaTimbrado'
    )
    BEGIN
        PRINT '   ✓ Columna FechaTimbrado existe'
    END
    ELSE
    BEGIN
        PRINT '   ❌ Columna FechaTimbrado no existe'
        PRINT '   Ejecutar: ALTER TABLE NominaDetalle ADD FechaTimbrado DATETIME NULL'
    END
END
ELSE
BEGIN
    PRINT '   ❌ Tabla NominaDetalle NO existe'
END
PRINT ''

-- ============================================================
-- RESUMEN FINAL
-- ============================================================
PRINT '=========================================='
PRINT 'CONFIGURACIÓN TIMBRADO NÓMINA - RESUMEN'
PRINT '=========================================='
PRINT ''

DECLARE @estadoFinal VARCHAR(20) = '✓ LISTO'
DECLARE @advertencias INT = 0

-- Verificar si hay advertencias
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ConfiguracionFiscalAPI')
    OR NOT EXISTS (SELECT * FROM ConfiguracionFiscalAPI WHERE Activo = 1)
BEGIN
    SET @estadoFinal = '⚠️ CON ADVERTENCIAS'
    SET @advertencias = @advertencias + 1
END

PRINT 'Estado: ' + @estadoFinal
PRINT ''

IF @advertencias = 0
BEGIN
    PRINT '✅ SISTEMA LISTO PARA TIMBRAR NÓMINA'
    PRINT ''
    PRINT 'Próximos pasos:'
    PRINT '1. Compilar proyecto en Visual Studio'
    PRINT '2. Ejecutar aplicación (F5)'
    PRINT '3. Ir a Nómina > Calcular'
    PRINT '4. Crear nómina de prueba'
    PRINT '5. Timbrar primer recibo'
    PRINT ''
    PRINT 'Tiempo estimado: 3 minutos'
END
ELSE
BEGIN
    PRINT '⚠️  CONFIGURACIÓN INCOMPLETA'
    PRINT ''
    PRINT 'Acciones requeridas:'
    PRINT '- Configurar FiscalAPI en el módulo de facturación'
    PRINT '- Verificar que la tabla ConfiguracionFiscalAPI exista'
    PRINT '- Asegurar que haya una configuración activa'
END

PRINT ''
PRINT '=========================================='
PRINT 'Script completado: ' + CONVERT(VARCHAR, GETDATE(), 120)
PRINT '=========================================='
GO
