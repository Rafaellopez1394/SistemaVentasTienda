-- ============================================================
-- SCRIPT: 007_CREAR_MODULO_FACTURACION.sql
-- DESCRIPCIÓN: Módulo completo de Facturación Electrónica CFDI 4.0
-- AUTOR: Sistema
-- FECHA: 2025-12-13
-- PROVEEDOR PAC: Finkok (configurable)
-- ============================================================

USE DB_TIENDA;
GO

PRINT '=== INICIANDO CREACIÓN MÓDULO FACTURACIÓN CFDI 4.0 ===';
GO

-- ============================================================
-- 1. CATÁLOGO DE USOS CFDI (SAT)
-- ============================================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'U' AND name = 'CatUsosCFDI')
BEGIN
    CREATE TABLE CatUsosCFDI (
        UsoCFDIID INT IDENTITY(1,1) PRIMARY KEY,
        Clave VARCHAR(5) NOT NULL UNIQUE,
        Descripcion VARCHAR(200) NOT NULL,
        AplicaPersonaFisica BIT DEFAULT 1,
        AplicaPersonaMoral BIT DEFAULT 1,
        Estatus BIT DEFAULT 1
    );

    -- Catálogo oficial SAT actualizado
    INSERT INTO CatUsosCFDI (Clave, Descripcion, AplicaPersonaFisica, AplicaPersonaMoral) VALUES
    ('G01', 'Adquisición de mercancías', 1, 1),
    ('G02', 'Devoluciones, descuentos o bonificaciones', 1, 1),
    ('G03', 'Gastos en general', 1, 1),
    ('I01', 'Construcciones', 1, 1),
    ('I02', 'Mobiliario y equipo de oficina por inversiones', 1, 1),
    ('I03', 'Equipo de transporte', 1, 1),
    ('I04', 'Equipo de cómputo y accesorios', 1, 1),
    ('I05', 'Dados, troqueles, moldes, matrices y herramental', 1, 1),
    ('I06', 'Comunicaciones telefónicas', 1, 1),
    ('I07', 'Comunicaciones satelitales', 1, 1),
    ('I08', 'Otra maquinaria y equipo', 1, 1),
    ('D01', 'Honorarios médicos, dentales y gastos hospitalarios', 1, 0),
    ('D02', 'Gastos médicos por incapacidad o discapacidad', 1, 0),
    ('D03', 'Gastos funerales', 1, 0),
    ('D04', 'Donativos', 1, 0),
    ('D05', 'Intereses reales efectivamente pagados por créditos hipotecarios', 1, 0),
    ('D06', 'Aportaciones voluntarias al SAR', 1, 0),
    ('D07', 'Primas por seguros de gastos médicos', 1, 0),
    ('D08', 'Gastos de transportación escolar obligatoria', 1, 0),
    ('D09', 'Depósitos en cuentas para el ahorro, primas', 1, 0),
    ('D10', 'Pagos por servicios educativos (colegiaturas)', 1, 0),
    ('S01', 'Sin efectos fiscales', 1, 1),
    ('CP01', 'Pagos', 1, 1),
    ('CN01', 'Nómina', 1, 1);

    PRINT '✓ Tabla CatUsosCFDI creada con 24 registros';
END
ELSE
BEGIN
    PRINT '✓ Tabla CatUsosCFDI ya existe';
END
GO

-- ============================================================
-- 2. CATÁLOGO DE REGÍMENES FISCALES (SAT)
-- ============================================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'U' AND name = 'CatRegimenesFiscales')
BEGIN
    CREATE TABLE CatRegimenesFiscales (
        RegimenFiscalID INT IDENTITY(1,1) PRIMARY KEY,
        Clave VARCHAR(5) NOT NULL UNIQUE,
        Descripcion VARCHAR(200) NOT NULL,
        AplicaPersonaFisica BIT DEFAULT 1,
        AplicaPersonaMoral BIT DEFAULT 1,
        Estatus BIT DEFAULT 1
    );

    INSERT INTO CatRegimenesFiscales (Clave, Descripcion, AplicaPersonaFisica, AplicaPersonaMoral) VALUES
    ('601', 'General de Ley Personas Morales', 0, 1),
    ('603', 'Personas Morales con Fines no Lucrativos', 0, 1),
    ('605', 'Sueldos y Salarios e Ingresos Asimilados a Salarios', 1, 0),
    ('606', 'Arrendamiento', 1, 0),
    ('607', 'Régimen de Enajenación o Adquisición de Bienes', 1, 0),
    ('608', 'Demás ingresos', 1, 0),
    ('610', 'Residentes en el Extranjero sin Establecimiento Permanente', 1, 1),
    ('611', 'Ingresos por Dividendos (socios y accionistas)', 1, 0),
    ('612', 'Personas Físicas con Actividades Empresariales y Profesionales', 1, 0),
    ('614', 'Ingresos por intereses', 1, 0),
    ('615', 'Régimen de los ingresos por obtención de premios', 1, 0),
    ('616', 'Sin obligaciones fiscales', 1, 0),
    ('620', 'Sociedades Cooperativas de Producción que optan por diferir sus ingresos', 0, 1),
    ('621', 'Incorporación Fiscal', 1, 0),
    ('622', 'Actividades Agrícolas, Ganaderas, Silvícolas y Pesqueras', 0, 1),
    ('623', 'Opcional para Grupos de Sociedades', 0, 1),
    ('624', 'Coordinados', 0, 1),
    ('625', 'Régimen de las Actividades Empresariales con ingresos a través de Plataformas', 1, 0),
    ('626', 'Régimen Simplificado de Confianza', 1, 1);

    PRINT '✓ Tabla CatRegimenesFiscales creada con 19 registros';
END
GO

-- ============================================================
-- 3. TABLA PRINCIPAL: Facturas
-- ============================================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'U' AND name = 'Facturas')
BEGIN
    CREATE TABLE Facturas (
        FacturaID UNIQUEIDENTIFIER DEFAULT NEWID() PRIMARY KEY,
        
        -- Relación con venta original
        VentaID UNIQUEIDENTIFIER NULL,
        FOREIGN KEY (VentaID) REFERENCES VentasClientes(VentaID),
        
        -- Datos del comprobante
        Serie VARCHAR(10) NULL,
        Folio VARCHAR(20) NOT NULL,
        FechaEmision DATETIME DEFAULT GETDATE(),
        Version VARCHAR(5) DEFAULT '4.0',
        TipoComprobante VARCHAR(2) DEFAULT 'I', -- I=Ingreso, E=Egreso, T=Traslado, N=Nómina, P=Pago
        
        -- Montos
        Subtotal DECIMAL(18,2) NOT NULL,
        Descuento DECIMAL(18,2) DEFAULT 0,
        Total DECIMAL(18,2) NOT NULL,
        
        -- Impuestos
        TotalImpuestosTrasladados DECIMAL(18,2) DEFAULT 0,
        TotalImpuestosRetenidos DECIMAL(18,2) DEFAULT 0,
        
        -- Datos fiscales emisor (de configuración)
        EmisorRFC VARCHAR(13) NOT NULL,
        EmisorNombre VARCHAR(200) NOT NULL,
        EmisorRegimenFiscal VARCHAR(5) NOT NULL,
        
        -- Datos fiscales receptor
        ReceptorRFC VARCHAR(13) NOT NULL,
        ReceptorNombre VARCHAR(200) NOT NULL,
        ReceptorUsoCFDI VARCHAR(5) NOT NULL,
        ReceptorDomicilioFiscalCP VARCHAR(5) NULL,
        ReceptorRegimenFiscalReceptor VARCHAR(5) NULL,
        ReceptorEmail VARCHAR(200) NULL,
        
        -- Forma y método de pago
        FormaPago VARCHAR(5) NULL, -- Catálogo SAT c_FormaPago
        MetodoPago VARCHAR(5) NOT NULL, -- PUE, PPD
        
        -- Datos del PAC (Finkok)
        UUID VARCHAR(36) NULL UNIQUE, -- Folio Fiscal asignado por SAT
        FechaTimbrado DATETIME NULL,
        NoCertificadoSAT VARCHAR(20) NULL,
        NoCertificadoEmisor VARCHAR(20) NULL,
        SelloCFD TEXT NULL,
        SelloSAT TEXT NULL,
        CadenaOriginalSAT TEXT NULL,
        ProveedorPAC VARCHAR(50) DEFAULT 'Finkok',
        
        -- Archivos
        XMLOriginal XML NULL,
        XMLTimbrado XML NULL,
        RutaPDF VARCHAR(500) NULL,
        
        -- Estado
        Estatus VARCHAR(20) DEFAULT 'PENDIENTE', -- PENDIENTE, TIMBRADA, CANCELADA, ERROR
        FechaCancelacion DATETIME NULL,
        MotivoCancelacion VARCHAR(500) NULL,
        FolioSustitucion VARCHAR(36) NULL,
        
        -- Auditoría
        UsuarioCreacion VARCHAR(50) NOT NULL,
        FechaCreacion DATETIME DEFAULT GETDATE(),
        UsuarioModificacion VARCHAR(50) NULL,
        FechaModificacion DATETIME NULL
    );

    CREATE INDEX IX_Facturas_VentaID ON Facturas(VentaID);
    CREATE INDEX IX_Facturas_UUID ON Facturas(UUID);
    CREATE INDEX IX_Facturas_RFC ON Facturas(ReceptorRFC);
    CREATE INDEX IX_Facturas_Fecha ON Facturas(FechaEmision);
    CREATE INDEX IX_Facturas_Estatus ON Facturas(Estatus);

    PRINT '✓ Tabla Facturas creada';
END
GO

-- ============================================================
-- 4. TABLA: FacturasDetalle (Conceptos)
-- ============================================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'U' AND name = 'FacturasDetalle')
BEGIN
    CREATE TABLE FacturasDetalle (
        FacturaDetalleID INT IDENTITY(1,1) PRIMARY KEY,
        FacturaID UNIQUEIDENTIFIER NOT NULL,
        FOREIGN KEY (FacturaID) REFERENCES Facturas(FacturaID),
        
        Secuencia INT NOT NULL,
        
        -- Producto/Servicio
        ClaveProdServ VARCHAR(10) NOT NULL, -- Catálogo SAT c_ClaveProdServ
        NoIdentificacion VARCHAR(100) NULL, -- SKU o código interno
        Cantidad DECIMAL(18,6) NOT NULL,
        ClaveUnidad VARCHAR(10) NOT NULL, -- Catálogo SAT c_ClaveUnidad
        Unidad VARCHAR(50) NULL,
        Descripcion VARCHAR(1000) NOT NULL,
        ValorUnitario DECIMAL(18,6) NOT NULL,
        Importe DECIMAL(18,2) NOT NULL,
        Descuento DECIMAL(18,2) DEFAULT 0,
        ObjetoImp VARCHAR(2) NOT NULL DEFAULT '02', -- 01=No objeto, 02=Sí objeto, 03=Sí objeto no obligado
        
        -- Impuestos del concepto
        TotalImpuestosTrasladados DECIMAL(18,2) DEFAULT 0,
        TotalImpuestosRetenidos DECIMAL(18,2) DEFAULT 0
    );

    CREATE INDEX IX_FacturasDetalle_FacturaID ON FacturasDetalle(FacturaID);

    PRINT '✓ Tabla FacturasDetalle creada';
END
GO

-- ============================================================
-- 5. TABLA: FacturasImpuestos (Traslados y Retenciones)
-- ============================================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'U' AND name = 'FacturasImpuestos')
BEGIN
    CREATE TABLE FacturasImpuestos (
        ImpuestoID INT IDENTITY(1,1) PRIMARY KEY,
        FacturaDetalleID INT NOT NULL,
        FOREIGN KEY (FacturaDetalleID) REFERENCES FacturasDetalle(FacturaDetalleID),
        
        TipoImpuesto VARCHAR(20) NOT NULL, -- TRASLADO, RETENCION
        Impuesto VARCHAR(5) NOT NULL, -- 001=ISR, 002=IVA, 003=IEPS
        TipoFactor VARCHAR(10) NOT NULL, -- Tasa, Cuota, Exento
        TasaOCuota DECIMAL(10,6) NULL,
        Base DECIMAL(18,2) NOT NULL,
        Importe DECIMAL(18,2) NULL
    );

    CREATE INDEX IX_FacturasImpuestos_Detalle ON FacturasImpuestos(FacturaDetalleID);

    PRINT '✓ Tabla FacturasImpuestos creada';
END
GO

-- ============================================================
-- 6. TABLA: FacturasCancelacion (Historial)
-- ============================================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'U' AND name = 'FacturasCancelacion')
BEGIN
    CREATE TABLE FacturasCancelacion (
        CancelacionID INT IDENTITY(1,1) PRIMARY KEY,
        FacturaID UNIQUEIDENTIFIER NOT NULL,
        FOREIGN KEY (FacturaID) REFERENCES Facturas(FacturaID),
        
        FechaSolicitud DATETIME DEFAULT GETDATE(),
        MotivoCancelacion VARCHAR(5) NOT NULL, -- 01, 02, 03, 04 (catálogo SAT)
        FolioSustitucion VARCHAR(36) NULL,
        
        -- Respuesta del PAC
        EstatusSAT VARCHAR(20) NULL, -- ACEPTADA, RECHAZADA, EN_PROCESO
        FechaRespuestaSAT DATETIME NULL,
        MensajeSAT VARCHAR(500) NULL,
        AcuseCancelacion XML NULL,
        
        UsuarioSolicita VARCHAR(50) NOT NULL,
        Observaciones VARCHAR(500) NULL
    );

    CREATE INDEX IX_FacturasCancelacion_FacturaID ON FacturasCancelacion(FacturaID);

    PRINT '✓ Tabla FacturasCancelacion creada';
END
GO

-- ============================================================
-- 7. TABLA: ConfiguracionPAC
-- ============================================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'U' AND name = 'ConfiguracionPAC')
BEGIN
    CREATE TABLE ConfiguracionPAC (
        ConfigID INT IDENTITY(1,1) PRIMARY KEY,
        ProveedorPAC VARCHAR(50) NOT NULL, -- Finkok, SW-Sapien, etc.
        
        -- Ambientes
        EsProduccion BIT DEFAULT 0, -- 0=Pruebas, 1=Producción
        UrlTimbrado VARCHAR(500) NOT NULL,
        UrlCancelacion VARCHAR(500) NOT NULL,
        UrlConsulta VARCHAR(500) NOT NULL,
        
        -- Credenciales (ENCRIPTAR EN PRODUCCIÓN)
        Usuario VARCHAR(100) NOT NULL,
        Password VARCHAR(100) NOT NULL, -- ⚠️ ENCRIPTAR
        
        -- Certificados
        RutaCertificado VARCHAR(500) NULL,
        RutaLlavePrivada VARCHAR(500) NULL,
        PasswordLlave VARCHAR(100) NULL, -- ⚠️ ENCRIPTAR
        
        -- Configuración adicional
        TimeoutSegundos INT DEFAULT 30,
        Activo BIT DEFAULT 1,
        FechaAlta DATETIME DEFAULT GETDATE(),
        FechaModificacion DATETIME NULL
    );

    -- Configuración inicial Finkok SANDBOX
    INSERT INTO ConfiguracionPAC (
        ProveedorPAC, EsProduccion, UrlTimbrado, UrlCancelacion, UrlConsulta,
        Usuario, Password, Activo
    ) VALUES (
        'Finkok',
        0, -- Sandbox
        'https://demo-facturacion.finkok.com/servicios/soap/stamp.wsdl',
        'https://demo-facturacion.finkok.com/servicios/soap/cancel.wsdl',
        'https://demo-facturacion.finkok.com/servicios/soap/utilities.wsdl',
        'cfdi@facturacionmoderna.com', -- Usuario demo Finkok
        '2y4e9w8u', -- Password demo Finkok
        1
    );

    PRINT '✓ Tabla ConfiguracionPAC creada con configuración Finkok Sandbox';
END
GO

-- ============================================================
-- 8. STORED PROCEDURE: Generar Folio Consecutivo
-- ============================================================
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'GenerarFolioFactura')
    DROP PROCEDURE GenerarFolioFactura;
GO

CREATE PROCEDURE GenerarFolioFactura
(
    @Serie VARCHAR(10),
    @Folio VARCHAR(20) OUTPUT
)
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @UltimoNumero INT;
    
    -- Obtener último folio de la serie
    SELECT @UltimoNumero = ISNULL(MAX(CAST(Folio AS INT)), 0)
    FROM Facturas
    WHERE Serie = @Serie
      AND ISNUMERIC(Folio) = 1;
    
    SET @Folio = RIGHT('00000000' + CAST(@UltimoNumero + 1 AS VARCHAR), 8);
END
GO

PRINT '✓ SP GenerarFolioFactura creado';
GO

PRINT '';
PRINT '=== MÓDULO DE FACTURACIÓN CREADO EXITOSAMENTE ===';
PRINT '';
PRINT '📋 Resumen:';
PRINT '  ✓ 7 tablas creadas (Facturas, Detalle, Impuestos, Cancelación, Catálogos)';
PRINT '  ✓ Configuración Finkok Sandbox lista';
PRINT '  ✓ Catálogos SAT cargados (24 usos CFDI, 19 regímenes fiscales)';
PRINT '';
PRINT '🔐 IMPORTANTE:';
PRINT '  ⚠ Credenciales demo Finkok cargadas (solo para pruebas)';
PRINT '  ⚠ En producción, encriptar passwords y configurar certificados .CER/.KEY';
PRINT '  ⚠ Solicitar credenciales reales en: https://www.finkok.com';
PRINT '';
PRINT '📖 Próximos pasos:';
PRINT '  1. Crear modelos C# (Factura.cs, FacturaDetalle.cs)';
PRINT '  2. Implementar CD_Factura.cs (capa de datos)';
PRINT '  3. Crear servicio FinkokPAC.cs (integración PAC)';
PRINT '  4. Implementar generador XML CFDI 4.0';
GO
