-- =====================================================
-- Script: 025_CREAR_TABLA_CERTIFICADOS_DIGITALES.sql
-- Descripción: Crea tabla para gestionar certificados
--              digitales del SAT (CSD)
-- Base de datos: DB_TIENDA
-- =====================================================

USE DB_TIENDA
GO

PRINT '========================================='
PRINT 'CREANDO TABLA CERTIFICADOS DIGITALES'
PRINT '========================================='
PRINT ''

-- Tabla para almacenar certificados digitales del SAT
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'CertificadosDigitales')
BEGIN
    CREATE TABLE CertificadosDigitales
    (
        CertificadoID INT IDENTITY(1,1) PRIMARY KEY,
        NombreCertificado VARCHAR(200) NOT NULL,
        
        -- Datos del certificado
        NoCertificado VARCHAR(20) NOT NULL UNIQUE,
        RFC VARCHAR(13) NOT NULL,
        RazonSocial VARCHAR(300) NOT NULL,
        
        -- Archivos del certificado (almacenados como binario)
        ArchivoCER VARBINARY(MAX) NOT NULL,
        ArchivoKEY VARBINARY(MAX) NOT NULL,
        PasswordKEY VARCHAR(500) NOT NULL, -- Se debe encriptar en producción
        
        -- Vigencia del certificado
        FechaVigenciaInicio DATETIME NOT NULL,
        FechaVigenciaFin DATETIME NOT NULL,
        
        -- Ruta física de respaldo (opcional)
        RutaCER VARCHAR(500) NULL,
        RutaKEY VARCHAR(500) NULL,
        
        -- Estado del certificado
        Activo BIT NOT NULL DEFAULT 1,
        EsPredeterminado BIT NOT NULL DEFAULT 0,
        
        -- Auditoría
        UsuarioCreacion VARCHAR(50) NOT NULL,
        FechaCreacion DATETIME NOT NULL DEFAULT GETDATE(),
        UsuarioModificacion VARCHAR(50) NULL,
        FechaModificacion DATETIME NULL,
        
        -- Notas
        Observaciones VARCHAR(500) NULL
    )

    PRINT '✓ Tabla CertificadosDigitales creada correctamente'
    
    -- Índices para mejorar rendimiento
    CREATE INDEX IX_CertificadosDigitales_RFC ON CertificadosDigitales(RFC)
    CREATE INDEX IX_CertificadosDigitales_Activo ON CertificadosDigitales(Activo)
    CREATE INDEX IX_CertificadosDigitales_Vigencia ON CertificadosDigitales(FechaVigenciaFin)
    CREATE INDEX IX_CertificadosDigitales_Predeterminado ON CertificadosDigitales(EsPredeterminado, Activo)
    
    PRINT '✓ Índices creados correctamente'
END
ELSE
BEGIN
    PRINT '⚠ La tabla CertificadosDigitales ya existe'
END
GO

-- Stored Procedure: Obtener certificado predeterminado activo
IF OBJECT_ID('SP_ObtenerCertificadoPredeterminado', 'P') IS NOT NULL
    DROP PROCEDURE SP_ObtenerCertificadoPredeterminado
GO

CREATE PROCEDURE SP_ObtenerCertificadoPredeterminado
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT TOP 1
        CertificadoID,
        NombreCertificado,
        NoCertificado,
        RFC,
        RazonSocial,
        ArchivoCER,
        ArchivoKEY,
        PasswordKEY,
        FechaVigenciaInicio,
        FechaVigenciaFin,
        RutaCER,
        RutaKEY
    FROM CertificadosDigitales
    WHERE Activo = 1
      AND EsPredeterminado = 1
      AND FechaVigenciaFin > GETDATE()
    ORDER BY FechaCreacion DESC
    
    -- Si no hay predeterminado, obtener el más reciente activo y vigente
    IF @@ROWCOUNT = 0
    BEGIN
        SELECT TOP 1
            CertificadoID,
            NombreCertificado,
            NoCertificado,
            RFC,
            RazonSocial,
            ArchivoCER,
            ArchivoKEY,
            PasswordKEY,
            FechaVigenciaInicio,
            FechaVigenciaFin,
            RutaCER,
            RutaKEY
        FROM CertificadosDigitales
        WHERE Activo = 1
          AND FechaVigenciaFin > GETDATE()
        ORDER BY FechaCreacion DESC
    END
END
GO

PRINT '✓ SP_ObtenerCertificadoPredeterminado creado correctamente'
GO

-- Stored Procedure: Validar vigencia de certificados
IF OBJECT_ID('SP_ValidarVigenciaCertificados', 'P') IS NOT NULL
    DROP PROCEDURE SP_ValidarVigenciaCertificados
GO

CREATE PROCEDURE SP_ValidarVigenciaCertificados
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Desactivar certificados vencidos
    UPDATE CertificadosDigitales
    SET Activo = 0,
        UsuarioModificacion = 'SISTEMA',
        FechaModificacion = GETDATE(),
        Observaciones = 'Certificado vencido - desactivado automáticamente'
    WHERE Activo = 1
      AND FechaVigenciaFin < GETDATE()
    
    SELECT @@ROWCOUNT AS CertificadosDesactivados
    
    -- Listar certificados próximos a vencer (30 días)
    SELECT 
        CertificadoID,
        NombreCertificado,
        RFC,
        FechaVigenciaFin,
        DATEDIFF(DAY, GETDATE(), FechaVigenciaFin) AS DiasRestantes
    FROM CertificadosDigitales
    WHERE Activo = 1
      AND FechaVigenciaFin > GETDATE()
      AND FechaVigenciaFin <= DATEADD(DAY, 30, GETDATE())
    ORDER BY FechaVigenciaFin
END
GO

PRINT '✓ SP_ValidarVigenciaCertificados creado correctamente'
GO

-- Stored Procedure: Establecer certificado predeterminado
IF OBJECT_ID('SP_EstablecerCertificadoPredeterminado', 'P') IS NOT NULL
    DROP PROCEDURE SP_EstablecerCertificadoPredeterminado
GO

CREATE PROCEDURE SP_EstablecerCertificadoPredeterminado
    @CertificadoID INT,
    @Usuario VARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRANSACTION
    
    BEGIN TRY
        -- Quitar predeterminado de todos
        UPDATE CertificadosDigitales
        SET EsPredeterminado = 0,
            UsuarioModificacion = @Usuario,
            FechaModificacion = GETDATE()
        WHERE EsPredeterminado = 1
        
        -- Establecer nuevo predeterminado
        UPDATE CertificadosDigitales
        SET EsPredeterminado = 1,
            Activo = 1,
            UsuarioModificacion = @Usuario,
            FechaModificacion = GETDATE()
        WHERE CertificadoID = @CertificadoID
        
        IF @@ROWCOUNT = 0
        BEGIN
            RAISERROR('Certificado no encontrado', 16, 1)
            ROLLBACK TRANSACTION
            RETURN
        END
        
        COMMIT TRANSACTION
        PRINT '✓ Certificado predeterminado establecido correctamente'
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION
        THROW
    END CATCH
END
GO

PRINT '✓ SP_EstablecerCertificadoPredeterminado creado correctamente'
GO

PRINT ''
PRINT '========================================='
PRINT 'SCRIPT COMPLETADO EXITOSAMENTE'
PRINT '========================================='
PRINT ''
PRINT 'Funcionalidades creadas:'
PRINT '1. Tabla CertificadosDigitales'
PRINT '2. SP_ObtenerCertificadoPredeterminado'
PRINT '3. SP_ValidarVigenciaCertificados'
PRINT '4. SP_EstablecerCertificadoPredeterminado'
PRINT ''
PRINT 'Próximos pasos:'
PRINT '1. Obtener certificados del SAT (CSD)'
PRINT '2. Cargar certificados en el sistema'
PRINT '3. Configurar PAC en producción'
PRINT '4. Actualizar RFC real en ConfiguracionGeneral'
GO
