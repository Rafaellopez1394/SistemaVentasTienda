-- Script SQL para crear tabla de Nóminas CFDI (Timbrado de Recibos de Nómina)
-- Complemento de Nómina 1.2 - CFDI 4.0
-- Ejecutar en la base de datos DB_TIENDA

PRINT 'Creando tabla NominasCFDI para timbrado de recibos de nómina...';

-- Tabla principal para almacenar los CFDIs de nómina timbrados
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[NominasCFDI]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[NominasCFDI] (
        NominaCFDIID INT IDENTITY(1,1) PRIMARY KEY,
        NominaDetalleID INT NOT NULL, -- FK a NominaDetalle (un CFDI por empleado)
        
        -- Datos del CFDI
        UUID UNIQUEIDENTIFIER NULL, -- Se genera después del timbrado
        FolioCFDI VARCHAR(50) NOT NULL,
        SerieCFDI VARCHAR(20) NULL,
        
        -- Control de timbrado
        FechaTimbrado DATETIME NULL,
        EstadoTimbrado VARCHAR(20) NOT NULL DEFAULT 'PENDIENTE', -- PENDIENTE, TIMBRADO, ERROR, CANCELADO
        
        -- XMLs
        XMLSinTimbrar NVARCHAR(MAX) NULL,
        XMLTimbrado NVARCHAR(MAX) NULL,
        
        -- Sellos digitales
        SelloCFD VARCHAR(MAX) NULL,
        SelloSAT VARCHAR(MAX) NULL,
        NoCertificadoSAT VARCHAR(50) NULL,
        NoCertificadoCFD VARCHAR(50) NULL,
        
        -- Cadena original del complemento de certificación
        CadenaOriginal NVARCHAR(MAX) NULL,
        
        -- Información fiscal
        RFCProveedorCertificacion VARCHAR(20) NULL, -- RFC del PAC (Finkok, etc.)
        FechaCertificacion DATETIME NULL,
        
        -- Control de cancelación
        FechaCancelacion DATETIME NULL,
        MotivoCancelacion VARCHAR(100) NULL,
        FolioSustitucion VARCHAR(50) NULL,
        EstatusCancelacion VARCHAR(20) NULL, -- NULL, CANCELADO, RECHAZO
        
        -- Totales del CFDI
        SubTotal DECIMAL(18,2) NOT NULL,
        Descuento DECIMAL(18,2) NULL,
        Total DECIMAL(18,2) NOT NULL,
        
        -- Errores
        CodigoError VARCHAR(50) NULL,
        MensajeError NVARCHAR(500) NULL,
        
        -- Auditoría
        UsuarioRegistro VARCHAR(100) NOT NULL,
        FechaAlta DATETIME NOT NULL DEFAULT GETDATE(),
        UltimaActualizacion DATETIME NOT NULL DEFAULT GETDATE(),
        
        -- FK
        CONSTRAINT FK_NominasCFDI_NominaDetalle FOREIGN KEY (NominaDetalleID) 
            REFERENCES NominaDetalle(NominaDetalleID),
        
        -- Constraints
        CONSTRAINT CK_NominasCFDI_Estado CHECK (EstadoTimbrado IN ('PENDIENTE', 'TIMBRADO', 'ERROR', 'CANCELADO')),
        CONSTRAINT UQ_NominasCFDI_UUID UNIQUE (UUID)
    );
    
    -- Índices para performance
    CREATE INDEX IX_NominasCFDI_NominaDetalle ON NominasCFDI(NominaDetalleID);
    CREATE INDEX IX_NominasCFDI_UUID ON NominasCFDI(UUID);
    CREATE INDEX IX_NominasCFDI_Estado ON NominasCFDI(EstadoTimbrado);
    CREATE INDEX IX_NominasCFDI_FechaTimbrado ON NominasCFDI(FechaTimbrado);
    
    PRINT '  ✓ Tabla NominasCFDI creada exitosamente';
END
ELSE
    PRINT '  ○ Tabla NominasCFDI ya existe';

PRINT '';
PRINT 'Verificando campo UUID en NominaDetalle...';

-- Asegurar que NominaDetalle tenga el campo UUID actualizado
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[NominaDetalle]') AND name = 'UUID')
BEGIN
    ALTER TABLE NominaDetalle ADD UUID UNIQUEIDENTIFIER NULL;
    PRINT '  ✓ Campo UUID agregado a NominaDetalle';
END
ELSE
    PRINT '  ○ Campo UUID ya existe en NominaDetalle';

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[NominaDetalle]') AND name = 'FechaTimbrado')
BEGIN
    ALTER TABLE NominaDetalle ADD FechaTimbrado DATETIME NULL;
    PRINT '  ✓ Campo FechaTimbrado agregado a NominaDetalle';
END
ELSE
    PRINT '  ○ Campo FechaTimbrado ya existe en NominaDetalle';

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[NominaDetalle]') AND name = 'EstatusTimbre')
BEGIN
    ALTER TABLE NominaDetalle ADD EstatusTimbre VARCHAR(20) NULL DEFAULT 'SIN_TIMBRAR';
    PRINT '  ✓ Campo EstatusTimbre agregado a NominaDetalle';
END
ELSE
    PRINT '  ○ Campo EstatusTimbre ya existe en NominaDetalle';

PRINT '';
PRINT 'Script completado exitosamente.';
PRINT '';
PRINT '═══════════════════════════════════════════════════════════';
PRINT 'Resumen de estructura para CFDI Nómina:';
PRINT '═══════════════════════════════════════════════════════════';
PRINT 'Tabla: NominasCFDI';
PRINT '  • Almacena un CFDI por cada recibo de nómina (NominaDetalle)';
PRINT '  • Incluye XMLs (sin timbrar y timbrado)';
PRINT '  • Guarda UUID, sellos digitales, cadena original';
PRINT '  • Control de estados: PENDIENTE → TIMBRADO → CANCELADO';
PRINT '  • Campos de error para debugging';
PRINT '';
PRINT 'Campos agregados a NominaDetalle:';
PRINT '  • UUID: Identificador único del CFDI';
PRINT '  • FechaTimbrado: Momento del timbrado exitoso';
PRINT '  • EstatusTimbre: Estado actual (SIN_TIMBRAR, TIMBRADO, etc.)';
PRINT '';
PRINT 'Próximos pasos:';
PRINT '  1. Crear CFDINomina12XMLGenerator.cs';
PRINT '  2. Implementar TimbrarNominaAsync() en FinkokPAC';
PRINT '  3. Agregar método TimbrarCFDINomina() en CD_Nomina';
PRINT '  4. Crear endpoint en NominaController';
PRINT '  5. Actualizar vistas con botón de timbrado';
PRINT '═══════════════════════════════════════════════════════════';
