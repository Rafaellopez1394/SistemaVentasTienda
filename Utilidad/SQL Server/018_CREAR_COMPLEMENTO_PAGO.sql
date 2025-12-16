-- =============================================
-- Script: 018_CREAR_COMPLEMENTO_PAGO.sql
-- Descripción: Tablas para Complemento de Pago 2.0 (Recibos de Pago)
-- Autor: Sistema
-- Fecha: 2025-12-14
-- =============================================

USE DBVENTAS
GO

-- Tabla principal de complementos de pago
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ComplementosPago')
BEGIN
    CREATE TABLE ComplementosPago
    (
        ComplementoPagoID INT IDENTITY(1,1) PRIMARY KEY,
        
        -- Datos del CFDI
        UUID UNIQUEIDENTIFIER NULL,
        Serie VARCHAR(25) NULL,
        Folio INT NULL,
        FechaEmision DATETIME NOT NULL DEFAULT GETDATE(),
        FechaTimbrado DATETIME NULL,
        
        -- Receptor (Cliente)
        ClienteID INT NOT NULL,
        ReceptorRFC VARCHAR(13) NOT NULL,
        ReceptorNombre VARCHAR(254) NOT NULL,
        ReceptorDomicilioFiscal VARCHAR(5) NOT NULL, -- CP
        ReceptorRegimenFiscal VARCHAR(3) NOT NULL DEFAULT '616', -- Sin obligaciones fiscales
        ReceptorUsoCFDI VARCHAR(3) NOT NULL DEFAULT 'CP01', -- Pagos
        
        -- Totales
        MontoTotalPagos DECIMAL(18,2) NOT NULL,
        
        -- Estados
        EstadoTimbrado VARCHAR(20) NOT NULL DEFAULT 'PENDIENTE',
        
        -- XMLs
        XMLSinTimbrar NVARCHAR(MAX) NULL,
        XMLTimbrado NVARCHAR(MAX) NULL,
        
        -- Datos del timbre
        SelloCFD VARCHAR(MAX) NULL,
        SelloSAT VARCHAR(MAX) NULL,
        NoCertificadoSAT VARCHAR(20) NULL,
        CadenaOriginal NVARCHAR(MAX) NULL,
        RFCProveedorCertificacion VARCHAR(13) NULL,
        
        -- Errores
        CodigoError VARCHAR(50) NULL,
        MensajeError VARCHAR(500) NULL,
        
        -- Auditoría
        FechaRegistro DATETIME NOT NULL DEFAULT GETDATE(),
        UsuarioRegistro VARCHAR(50) NULL,
        
        CONSTRAINT FK_ComplementosPago_Cliente FOREIGN KEY (ClienteID) 
            REFERENCES Cliente(ClienteID),
        
        CONSTRAINT CK_EstadoTimbrado_CP CHECK (EstadoTimbrado IN ('PENDIENTE', 'TIMBRADO', 'ERROR', 'CANCELADO'))
    );

    PRINT 'Tabla ComplementosPago creada correctamente';
END
ELSE
BEGIN
    PRINT 'La tabla ComplementosPago ya existe';
END
GO

-- Tabla de pagos (un complemento puede tener múltiples pagos)
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ComplementoPagoPagos')
BEGIN
    CREATE TABLE ComplementoPagoPagos
    (
        PagoID INT IDENTITY(1,1) PRIMARY KEY,
        ComplementoPagoID INT NOT NULL,
        
        -- Datos del pago
        FechaPago DATETIME NOT NULL,
        FormaDePagoP VARCHAR(2) NOT NULL, -- 01=Efectivo, 02=Cheque, 03=Transferencia, etc.
        MonedaP VARCHAR(3) NOT NULL DEFAULT 'MXN',
        TipoCambioP DECIMAL(18,6) NULL,
        Monto DECIMAL(18,2) NOT NULL,
        
        -- Datos bancarios ordenante (quien paga)
        NumOperacion VARCHAR(100) NULL,
        RfcEmisorCtaOrd VARCHAR(13) NULL,
        NomBancoOrdExt VARCHAR(300) NULL,
        CtaOrdenante VARCHAR(50) NULL,
        
        -- Datos bancarios beneficiario (quien recibe)
        RfcEmisorCtaBen VARCHAR(13) NULL,
        CtaBeneficiario VARCHAR(50) NULL,
        TipoCadPago VARCHAR(2) NULL, -- 01=SPEI, 02=Otros
        CertPago VARCHAR(MAX) NULL,
        CadPago VARCHAR(MAX) NULL,
        SelloPago VARCHAR(MAX) NULL,
        
        CONSTRAINT FK_ComplementoPagoPagos_Complemento FOREIGN KEY (ComplementoPagoID) 
            REFERENCES ComplementosPago(ComplementoPagoID)
    );

    PRINT 'Tabla ComplementoPagoPagos creada correctamente';
END
ELSE
BEGIN
    PRINT 'La tabla ComplementoPagoPagos ya existe';
END
GO

-- Tabla de documentos relacionados (facturas que se están pagando)
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ComplementoPagoDocumentos')
BEGIN
    CREATE TABLE ComplementoPagoDocumentos
    (
        DocumentoID INT IDENTITY(1,1) PRIMARY KEY,
        PagoID INT NOT NULL,
        FacturaID INT NOT NULL,
        
        -- Identificación del documento
        IdDocumento UNIQUEIDENTIFIER NOT NULL, -- UUID de la factura
        Serie VARCHAR(25) NULL,
        Folio VARCHAR(40) NULL,
        MonedaDR VARCHAR(3) NOT NULL DEFAULT 'MXN',
        EquivalenciaDR DECIMAL(18,6) NULL DEFAULT 1,
        
        -- Parcialidades
        NumParcialidad INT NOT NULL,
        ImpSaldoAnt DECIMAL(18,2) NOT NULL, -- Saldo anterior
        ImpPagado DECIMAL(18,2) NOT NULL, -- Monto pagado en esta aplicación
        ImpSaldoInsoluto DECIMAL(18,2) NOT NULL, -- Saldo restante
        
        -- Impuestos del documento relacionado
        ObjetoImpDR VARCHAR(2) NOT NULL DEFAULT '02', -- 02=Sí objeto de impuesto
        
        CONSTRAINT FK_ComplementoPagoDocumentos_Pago FOREIGN KEY (PagoID) 
            REFERENCES ComplementoPagoPagos(PagoID),
        
        CONSTRAINT FK_ComplementoPagoDocumentos_Factura FOREIGN KEY (FacturaID) 
            REFERENCES Factura(IdFactura)
    );

    PRINT 'Tabla ComplementoPagoDocumentos creada correctamente';
END
ELSE
BEGIN
    PRINT 'La tabla ComplementoPagoDocumentos ya existe';
END
GO

-- Tabla de impuestos trasladados de documentos relacionados
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ComplementoPagoImpuestosDR')
BEGIN
    CREATE TABLE ComplementoPagoImpuestosDR
    (
        ImpuestoDRID INT IDENTITY(1,1) PRIMARY KEY,
        DocumentoID INT NOT NULL,
        
        -- Tipo de impuesto
        TipoImpuesto VARCHAR(20) NOT NULL, -- TRASLADO o RETENCION
        
        -- Datos del impuesto
        BaseDR DECIMAL(18,2) NOT NULL,
        ImpuestoDR VARCHAR(3) NOT NULL, -- 002=IVA, 001=ISR
        TipoFactorDR VARCHAR(10) NOT NULL, -- Tasa, Cuota, Exento
        TasaOCuotaDR DECIMAL(8,6) NULL,
        ImporteDR DECIMAL(18,2) NULL,
        
        CONSTRAINT FK_ComplementoPagoImpuestosDR_Documento FOREIGN KEY (DocumentoID) 
            REFERENCES ComplementoPagoDocumentos(DocumentoID),
        
        CONSTRAINT CK_TipoImpuesto_CP CHECK (TipoImpuesto IN ('TRASLADO', 'RETENCION'))
    );

    PRINT 'Tabla ComplementoPagoImpuestosDR creada correctamente';
END
ELSE
BEGIN
    PRINT 'La tabla ComplementoPagoImpuestosDR ya existe';
END
GO

-- Agregar campos a Factura para control de pagos
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Factura') AND name = 'SaldoPendiente')
BEGIN
    ALTER TABLE Factura ADD SaldoPendiente DECIMAL(18,2) NULL;
    PRINT 'Campo SaldoPendiente agregado a tabla Factura';
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Factura') AND name = 'EsPagada')
BEGIN
    ALTER TABLE Factura ADD EsPagada BIT NOT NULL DEFAULT 0;
    PRINT 'Campo EsPagada agregado a tabla Factura';
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Factura') AND name = 'NumeroParcialidades')
BEGIN
    ALTER TABLE Factura ADD NumeroParcialidades INT NOT NULL DEFAULT 0;
    PRINT 'Campo NumeroParcialidades agregado a tabla Factura';
END
GO

-- Actualizar saldos pendientes de facturas existentes
UPDATE Factura
SET SaldoPendiente = MontoTotal,
    NumeroParcialidades = 0
WHERE SaldoPendiente IS NULL AND EsCancelada = 0;

PRINT 'Saldos pendientes inicializados para facturas existentes';
GO

-- Índices
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ComplementosPago_UUID')
BEGIN
    CREATE INDEX IX_ComplementosPago_UUID ON ComplementosPago(UUID);
    PRINT 'Índice IX_ComplementosPago_UUID creado';
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ComplementosPago_Cliente')
BEGIN
    CREATE INDEX IX_ComplementosPago_Cliente ON ComplementosPago(ClienteID);
    PRINT 'Índice IX_ComplementosPago_Cliente creado';
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ComplementosPago_Estado')
BEGIN
    CREATE INDEX IX_ComplementosPago_Estado ON ComplementosPago(EstadoTimbrado);
    PRINT 'Índice IX_ComplementosPago_Estado creado';
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ComplementoPagoDocumentos_Factura')
BEGIN
    CREATE INDEX IX_ComplementoPagoDocumentos_Factura ON ComplementoPagoDocumentos(FacturaID);
    PRINT 'Índice IX_ComplementoPagoDocumentos_Factura creado';
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Factura_SaldoPendiente')
BEGIN
    CREATE INDEX IX_Factura_SaldoPendiente ON Factura(SaldoPendiente) WHERE SaldoPendiente > 0;
    PRINT 'Índice IX_Factura_SaldoPendiente creado';
END
GO

PRINT '';
PRINT '=== CATÁLOGO DE FORMAS DE PAGO (c_FormaPago SAT) ===';
PRINT '01 - Efectivo';
PRINT '02 - Cheque nominativo';
PRINT '03 - Transferencia electrónica de fondos';
PRINT '04 - Tarjeta de crédito';
PRINT '28 - Tarjeta de débito';
PRINT '99 - Por definir';
PRINT '';

PRINT 'Script 018 ejecutado correctamente';
GO
