-- =============================================
-- MÓDULO DE NÓMINA - SISTEMA DE VENTAS TIENDA
-- Cumplimiento SAT y Contabilidad Electrónica
-- =============================================

USE DB_TIENDA
GO

-- =============================================
-- TABLA: Empleados
-- =============================================
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Empleados')
BEGIN
    CREATE TABLE Empleados (
        EmpleadoID INT IDENTITY(1,1) PRIMARY KEY,
        NumeroEmpleado VARCHAR(20) NOT NULL UNIQUE,
        Nombre VARCHAR(100) NOT NULL,
        ApellidoPaterno VARCHAR(100) NOT NULL,
        ApellidoMaterno VARCHAR(100) NULL,
        RFC VARCHAR(13) NOT NULL UNIQUE,
        CURP VARCHAR(18) NOT NULL UNIQUE,
        NSS VARCHAR(11) NULL,
        FechaNacimiento DATE NOT NULL,
        FechaIngreso DATE NOT NULL,
        FechaBaja DATE NULL,
        
        -- Información laboral
        SucursalID INT NOT NULL,
        Puesto VARCHAR(100) NOT NULL,
        Departamento VARCHAR(100) NULL,
        TipoContrato VARCHAR(50) NOT NULL, -- PLANTA, TEMPORAL, HONORARIOS
        TipoJornada VARCHAR(50) NOT NULL, -- DIURNA, NOCTURNA, MIXTA
        PeriodicidadPago VARCHAR(50) NOT NULL, -- SEMANAL, QUINCENAL, MENSUAL
        
        -- Salario
        SalarioDiario DECIMAL(18,2) NOT NULL,
        SalarioMensual DECIMAL(18,2) NOT NULL,
        SalarioDiarioIntegrado DECIMAL(18,2) NULL,
        
        -- Contacto
        Telefono VARCHAR(20) NULL,
        Email VARCHAR(100) NULL,
        Domicilio VARCHAR(255) NULL,
        CodigoPostal VARCHAR(5) NULL,
        
        -- Datos bancarios
        Banco VARCHAR(100) NULL,
        CuentaBancaria VARCHAR(20) NULL,
        CLABE VARCHAR(18) NULL,
        
        -- Control
        Estatus VARCHAR(20) NOT NULL DEFAULT 'ACTIVO', -- ACTIVO, BAJA, SUSPENDIDO
        Usuario VARCHAR(100) NOT NULL,
        FechaAlta DATETIME NOT NULL DEFAULT GETDATE(),
        UltimaAct DATETIME NOT NULL DEFAULT GETDATE(),
        
        FOREIGN KEY (SucursalID) REFERENCES Sucursal(SucursalID)
    )
    
    CREATE INDEX IX_Empleados_RFC ON Empleados(RFC)
    CREATE INDEX IX_Empleados_Estatus ON Empleados(Estatus)
    CREATE INDEX IX_Empleados_SucursalID ON Empleados(SucursalID)
    
    PRINT 'Tabla Empleados creada'
END
GO

-- =============================================
-- TABLA: CatPercepcionesNomina
-- =============================================
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'CatPercepcionesNomina')
BEGIN
    CREATE TABLE CatPercepcionesNomina (
        PercepcionID INT IDENTITY(1,1) PRIMARY KEY,
        ClavePercepcion VARCHAR(10) NOT NULL UNIQUE, -- Según catálogo SAT
        Nombre VARCHAR(200) NOT NULL,
        Descripcion VARCHAR(500) NULL,
        TipoPercepcion VARCHAR(50) NOT NULL, -- FIJA, VARIABLE, EXTRAORDINARIA
        GravaISR BIT NOT NULL DEFAULT 1,
        GravaIMSS BIT NOT NULL DEFAULT 1,
        EsIntegrable BIT NOT NULL DEFAULT 1, -- Para salario integrado
        CuentaContableID INT NULL,
        Activo BIT NOT NULL DEFAULT 1,
        FechaAlta DATETIME NOT NULL DEFAULT GETDATE(),
        
        FOREIGN KEY (CuentaContableID) REFERENCES CatalogoContable(CuentaID)
    )
    
    -- Insertar percepciones estándar según SAT
    INSERT INTO CatPercepcionesNomina (ClavePercepcion, Nombre, TipoPercepcion, GravaISR, GravaIMSS, EsIntegrable) VALUES
    ('001', 'Sueldos, Salarios, Rayas y Jornales', 'FIJA', 1, 1, 1),
    ('002', 'Gratificación Anual (Aguinaldo)', 'EXTRAORDINARIA', 1, 0, 0),
    ('003', 'Participación de los Trabajadores en las Utilidades (PTU)', 'EXTRAORDINARIA', 1, 0, 0),
    ('004', 'Reembolso de Gastos Médicos, Dentales y Hospitalarios', 'VARIABLE', 0, 0, 0),
    ('005', 'Fondo de Ahorro', 'VARIABLE', 0, 0, 0),
    ('006', 'Caja de Ahorro', 'VARIABLE', 0, 0, 0),
    ('009', 'Contribuciones a Cargo del Trabajador Pagadas por el Patrón', 'VARIABLE', 1, 0, 0),
    ('010', 'Premios por Puntualidad', 'VARIABLE', 1, 1, 1),
    ('011', 'Prima de Seguro de Vida', 'VARIABLE', 0, 0, 0),
    ('012', 'Seguro de Gastos Médicos Mayores', 'VARIABLE', 0, 0, 0),
    ('013', 'Cuotas Sindicales Pagadas por el Patrón', 'VARIABLE', 1, 0, 0),
    ('019', 'Horas Extra', 'VARIABLE', 1, 1, 1),
    ('020', 'Prima Dominical', 'VARIABLE', 1, 1, 1),
    ('021', 'Prima Vacacional', 'EXTRAORDINARIA', 1, 0, 0),
    ('022', 'Prima por Antigüedad', 'EXTRAORDINARIA', 1, 0, 0),
    ('023', 'Pagos por Separación', 'EXTRAORDINARIA', 1, 0, 0),
    ('024', 'Seguro de Retiro', 'VARIABLE', 0, 0, 0),
    ('025', 'Indemnizaciones', 'EXTRAORDINARIA', 0, 0, 0),
    ('028', 'Subsidio para el Empleo (Efectivamente Entregado al Trabajador)', 'VARIABLE', 0, 0, 0),
    ('029', 'Becas para Trabajadores o Hijos', 'VARIABLE', 0, 0, 0),
    ('031', 'Subsidio Causado', 'VARIABLE', 0, 0, 0),
    ('032', 'Apoyo Transporte', 'VARIABLE', 0, 0, 0),
    ('033', 'Apoyo Renta', 'VARIABLE', 0, 0, 0),
    ('034', 'Apoyo Cargos Sociales', 'VARIABLE', 0, 0, 0),
    ('038', 'Vales de Despensa', 'VARIABLE', 0, 0, 0),
    ('039', 'Vales de Restaurante', 'VARIABLE', 0, 0, 0),
    ('044', 'Jubilaciones, Pensiones o Haberes de Retiro', 'EXTRAORDINARIA', 1, 0, 0),
    ('050', 'Viáticos', 'VARIABLE', 0, 0, 0)
    
    PRINT 'Tabla CatPercepcionesNomina creada con percepciones SAT'
END
GO

-- =============================================
-- TABLA: CatDeduccionesNomina
-- =============================================
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'CatDeduccionesNomina')
BEGIN
    CREATE TABLE CatDeduccionesNomina (
        DeduccionID INT IDENTITY(1,1) PRIMARY KEY,
        ClaveDeduccion VARCHAR(10) NOT NULL UNIQUE, -- Según catálogo SAT
        Nombre VARCHAR(200) NOT NULL,
        Descripcion VARCHAR(500) NULL,
        TipoDeduccion VARCHAR(50) NOT NULL, -- LEGAL, CONTRACTUAL, VOLUNTARIA
        CuentaContableID INT NULL,
        Activo BIT NOT NULL DEFAULT 1,
        FechaAlta DATETIME NOT NULL DEFAULT GETDATE(),
        
        FOREIGN KEY (CuentaContableID) REFERENCES CatalogoContable(CuentaID)
    )
    
    -- Insertar deducciones estándar según SAT
    INSERT INTO CatDeduccionesNomina (ClaveDeduccion, Nombre, TipoDeduccion) VALUES
    ('001', 'Seguridad Social (IMSS)', 'LEGAL'),
    ('002', 'ISR', 'LEGAL'),
    ('003', 'Aportaciones a Retiro, Cesantía en Edad Avanzada y Vejez', 'LEGAL'),
    ('004', 'Otros', 'VOLUNTARIA'),
    ('005', 'Aportaciones a Fondo de Vivienda (INFONAVIT)', 'LEGAL'),
    ('006', 'Descuento por Incapacidad', 'LEGAL'),
    ('007', 'Pensión Alimenticia', 'LEGAL'),
    ('008', 'Renta', 'VOLUNTARIA'),
    ('009', 'Préstamos Provenientes del Fondo Nacional de la Vivienda para los Trabajadores', 'CONTRACTUAL'),
    ('010', 'Pago por Crédito de Vivienda', 'CONTRACTUAL'),
    ('011', 'Pago de Abonos INFONACOT', 'CONTRACTUAL'),
    ('012', 'Anticipo de Salarios', 'CONTRACTUAL'),
    ('013', 'Pagos Hechos con Exceso al Trabajador', 'CONTRACTUAL'),
    ('014', 'Errores', 'CONTRACTUAL'),
    ('015', 'Pérdidas', 'CONTRACTUAL'),
    ('016', 'Averías', 'CONTRACTUAL'),
    ('017', 'Adquisición de Artículos Producidos por la Empresa o Establecimiento', 'VOLUNTARIA'),
    ('018', 'Cuotas para la Constitución y Fomento de Sociedades Cooperativas y de Cajas de Ahorro', 'VOLUNTARIA'),
    ('019', 'Cuotas Sindicales', 'VOLUNTARIA'),
    ('020', 'Ausencias (Ausentismo)', 'LEGAL'),
    ('021', 'Cuotas Obrero Patronales', 'LEGAL'),
    ('022', 'Impuestos Locales', 'LEGAL'),
    ('023', 'Aportaciones Voluntarias', 'VOLUNTARIA'),
    ('024', 'Ajuste en Gratificación Anual (Aguinaldo) Exento', 'LEGAL'),
    ('025', 'Ajuste en Gratificación Anual (Aguinaldo) Gravado', 'LEGAL'),
    ('107', 'Ajuste en Subsidio para el Empleo', 'LEGAL')
    
    PRINT 'Tabla CatDeduccionesNomina creada con deducciones SAT'
END
GO

-- =============================================
-- TABLA: Nominas (Encabezado)
-- =============================================
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Nominas')
BEGIN
    CREATE TABLE Nominas (
        NominaID INT IDENTITY(1,1) PRIMARY KEY,
        Folio VARCHAR(50) NOT NULL UNIQUE,
        Periodo VARCHAR(50) NOT NULL, -- Ej: "2025-01-Q1" para primera quincena de enero
        FechaInicio DATE NOT NULL,
        FechaFin DATE NOT NULL,
        FechaPago DATE NOT NULL,
        TipoNomina VARCHAR(50) NOT NULL, -- ORDINARIA, EXTRAORDINARIA, FINIQUITO
        
        -- Totales
        TotalPercepciones DECIMAL(18,2) NOT NULL DEFAULT 0,
        TotalDeducciones DECIMAL(18,2) NOT NULL DEFAULT 0,
        TotalNeto DECIMAL(18,2) NOT NULL DEFAULT 0,
        NumeroEmpleados INT NOT NULL DEFAULT 0,
        
        -- Control de proceso
        Estatus VARCHAR(20) NOT NULL DEFAULT 'CALCULADA', -- CALCULADA, TIMBRADA, PAGADA, CONTABILIZADA, CANCELADA
        FechaCalculo DATETIME NOT NULL DEFAULT GETDATE(),
        FechaTimbrado DATETIME NULL,
        FechaPagado DATETIME NULL,
        FechaContabilizada DATETIME NULL,
        
        -- Relación con póliza contable
        PolizaID UNIQUEIDENTIFIER NULL,
        
        -- Control
        Usuario VARCHAR(100) NOT NULL,
        FechaAlta DATETIME NOT NULL DEFAULT GETDATE(),
        UltimaAct DATETIME NOT NULL DEFAULT GETDATE(),
        
        FOREIGN KEY (PolizaID) REFERENCES Polizas(PolizaID)
    )
    
    CREATE INDEX IX_Nominas_Periodo ON Nominas(Periodo)
    CREATE INDEX IX_Nominas_Estatus ON Nominas(Estatus)
    CREATE INDEX IX_Nominas_FechaPago ON Nominas(FechaPago)
    
    PRINT 'Tabla Nominas creada'
END
GO

-- =============================================
-- TABLA: NominaDetalle (Recibos individuales)
-- =============================================
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'NominaDetalle')
BEGIN
    CREATE TABLE NominaDetalle (
        NominaDetalleID INT IDENTITY(1,1) PRIMARY KEY,
        NominaID INT NOT NULL,
        EmpleadoID INT NOT NULL,
        
        -- Días y periodo
        DiasTrabajados DECIMAL(5,2) NOT NULL,
        FechaInicio DATE NOT NULL,
        FechaFin DATE NOT NULL,
        
        -- Salarios
        SalarioDiario DECIMAL(18,2) NOT NULL,
        SalarioBase DECIMAL(18,2) NOT NULL,
        
        -- Totales del recibo
        TotalPercepciones DECIMAL(18,2) NOT NULL DEFAULT 0,
        TotalPercepcionesGravadas DECIMAL(18,2) NOT NULL DEFAULT 0,
        TotalPercepcionesExentas DECIMAL(18,2) NOT NULL DEFAULT 0,
        
        TotalDeducciones DECIMAL(18,2) NOT NULL DEFAULT 0,
        TotalImpuestosRetenidos DECIMAL(18,2) NOT NULL DEFAULT 0,
        TotalOtrasDeducciones DECIMAL(18,2) NOT NULL DEFAULT 0,
        
        NetoPagar DECIMAL(18,2) NOT NULL DEFAULT 0,
        
        -- Timbrado CFDI
        UUID VARCHAR(36) NULL,
        FechaTimbrado DATETIME NULL,
        SelloCFD VARCHAR(MAX) NULL,
        SelloSAT VARCHAR(MAX) NULL,
        CadenaOriginal VARCHAR(MAX) NULL,
        
        -- Control
        Estatus VARCHAR(20) NOT NULL DEFAULT 'CALCULADO', -- CALCULADO, TIMBRADO, PAGADO, CANCELADO
        FechaAlta DATETIME NOT NULL DEFAULT GETDATE(),
        UltimaAct DATETIME NOT NULL DEFAULT GETDATE(),
        
        FOREIGN KEY (NominaID) REFERENCES Nominas(NominaID),
        FOREIGN KEY (EmpleadoID) REFERENCES Empleados(EmpleadoID)
    )
    
    CREATE INDEX IX_NominaDetalle_NominaID ON NominaDetalle(NominaID)
    CREATE INDEX IX_NominaDetalle_EmpleadoID ON NominaDetalle(EmpleadoID)
    CREATE INDEX IX_NominaDetalle_UUID ON NominaDetalle(UUID)
    
    PRINT 'Tabla NominaDetalle creada'
END
GO

-- =============================================
-- TABLA: NominaPercepciones
-- =============================================
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'NominaPercepciones')
BEGIN
    CREATE TABLE NominaPercepciones (
        NominaPercepcionID INT IDENTITY(1,1) PRIMARY KEY,
        NominaDetalleID INT NOT NULL,
        PercepcionID INT NOT NULL,
        
        Clave VARCHAR(10) NOT NULL,
        Concepto VARCHAR(200) NOT NULL,
        TipoPercepcion VARCHAR(50) NOT NULL,
        
        ImporteGravado DECIMAL(18,2) NOT NULL DEFAULT 0,
        ImporteExento DECIMAL(18,2) NOT NULL DEFAULT 0,
        ImporteTotal DECIMAL(18,2) NOT NULL DEFAULT 0,
        
        FechaAlta DATETIME NOT NULL DEFAULT GETDATE(),
        
        FOREIGN KEY (NominaDetalleID) REFERENCES NominaDetalle(NominaDetalleID),
        FOREIGN KEY (PercepcionID) REFERENCES CatPercepcionesNomina(PercepcionID)
    )
    
    CREATE INDEX IX_NominaPercepciones_NominaDetalleID ON NominaPercepciones(NominaDetalleID)
    
    PRINT 'Tabla NominaPercepciones creada'
END
GO

-- =============================================
-- TABLA: NominaDeducciones
-- =============================================
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'NominaDeducciones')
BEGIN
    CREATE TABLE NominaDeducciones (
        NominaDeduccionID INT IDENTITY(1,1) PRIMARY KEY,
        NominaDetalleID INT NOT NULL,
        DeduccionID INT NOT NULL,
        
        Clave VARCHAR(10) NOT NULL,
        Concepto VARCHAR(200) NOT NULL,
        TipoDeduccion VARCHAR(50) NOT NULL,
        
        Importe DECIMAL(18,2) NOT NULL DEFAULT 0,
        
        FechaAlta DATETIME NOT NULL DEFAULT GETDATE(),
        
        FOREIGN KEY (NominaDetalleID) REFERENCES NominaDetalle(NominaDetalleID),
        FOREIGN KEY (DeduccionID) REFERENCES CatDeduccionesNomina(DeduccionID)
    )
    
    CREATE INDEX IX_NominaDeducciones_NominaDetalleID ON NominaDeducciones(NominaDetalleID)
    
    PRINT 'Tabla NominaDeducciones creada'
END
GO

-- =============================================
-- TABLA: TablaISR (Tabla mensual 2025)
-- =============================================
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'TablaISR')
BEGIN
    CREATE TABLE TablaISR (
        TablaISRID INT IDENTITY(1,1) PRIMARY KEY,
        LimiteInferior DECIMAL(18,2) NOT NULL,
        LimiteSuperior DECIMAL(18,2) NOT NULL,
        CuotaFija DECIMAL(18,2) NOT NULL,
        PorcentajeExcedente DECIMAL(5,2) NOT NULL,
        Periodicidad VARCHAR(20) NOT NULL, -- MENSUAL, QUINCENAL, SEMANAL
        Anio INT NOT NULL,
        Activo BIT NOT NULL DEFAULT 1
    )
    
    -- Insertar tabla ISR 2025 MENSUAL (ejemplo - verificar con tablas SAT actuales)
    INSERT INTO TablaISR (LimiteInferior, LimiteSuperior, CuotaFija, PorcentajeExcedente, Periodicidad, Anio) VALUES
    (0.01, 746.04, 0.00, 1.92, 'MENSUAL', 2025),
    (746.05, 6332.05, 14.32, 6.40, 'MENSUAL', 2025),
    (6332.06, 11128.01, 371.83, 10.88, 'MENSUAL', 2025),
    (11128.02, 12935.82, 893.63, 16.00, 'MENSUAL', 2025),
    (12935.83, 15487.71, 1182.88, 17.92, 'MENSUAL', 2025),
    (15487.72, 31236.49, 1640.18, 21.36, 'MENSUAL', 2025),
    (31236.50, 49233.00, 5004.12, 23.52, 'MENSUAL', 2025),
    (49233.01, 93993.90, 9236.89, 30.00, 'MENSUAL', 2025),
    (93993.91, 125325.20, 22665.17, 32.00, 'MENSUAL', 2025),
    (125325.21, 375975.61, 32691.18, 34.00, 'MENSUAL', 2025),
    (375975.62, 999999999.99, 117912.32, 35.00, 'MENSUAL', 2025)
    
    PRINT 'Tabla TablaISR creada con valores 2025'
END
GO

-- =============================================
-- TABLA: TablaSubsidioEmpleo (Tabla mensual 2025)
-- =============================================
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'TablaSubsidioEmpleo')
BEGIN
    CREATE TABLE TablaSubsidioEmpleo (
        TablaSubsidioID INT IDENTITY(1,1) PRIMARY KEY,
        LimiteInferior DECIMAL(18,2) NOT NULL,
        LimiteSuperior DECIMAL(18,2) NOT NULL,
        MontoSubsidio DECIMAL(18,2) NOT NULL,
        Periodicidad VARCHAR(20) NOT NULL, -- MENSUAL, QUINCENAL, SEMANAL
        Anio INT NOT NULL,
        Activo BIT NOT NULL DEFAULT 1
    )
    
    -- Insertar tabla Subsidio al Empleo 2025 MENSUAL
    INSERT INTO TablaSubsidioEmpleo (LimiteInferior, LimiteSuperior, MontoSubsidio, Periodicidad, Anio) VALUES
    (0.01, 1768.96, 407.02, 'MENSUAL', 2025),
    (1768.97, 2653.38, 406.83, 'MENSUAL', 2025),
    (2653.39, 3472.84, 406.62, 'MENSUAL', 2025),
    (3472.85, 3537.87, 392.77, 'MENSUAL', 2025),
    (3537.88, 4446.15, 382.46, 'MENSUAL', 2025),
    (4446.16, 4717.18, 354.23, 'MENSUAL', 2025),
    (4717.19, 5335.42, 324.87, 'MENSUAL', 2025),
    (5335.43, 6224.67, 294.63, 'MENSUAL', 2025),
    (6224.68, 7113.90, 253.54, 'MENSUAL', 2025),
    (7113.91, 7382.33, 217.61, 'MENSUAL', 2025),
    (7382.34, 999999999.99, 0.00, 'MENSUAL', 2025)
    
    PRINT 'Tabla TablaSubsidioEmpleo creada con valores 2025'
END
GO

PRINT ''
PRINT '========================================='
PRINT 'MÓDULO DE NÓMINA CREADO EXITOSAMENTE'
PRINT '========================================='
PRINT 'Tablas creadas:'
PRINT '- Empleados'
PRINT '- CatPercepcionesNomina (con 27 percepciones SAT)'
PRINT '- CatDeduccionesNomina (con 27 deducciones SAT)'
PRINT '- Nominas'
PRINT '- NominaDetalle'
PRINT '- NominaPercepciones'
PRINT '- NominaDeducciones'
PRINT '- TablaISR (2025)'
PRINT '- TablaSubsidioEmpleo (2025)'
PRINT '========================================='
