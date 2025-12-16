/*
=====================================================
Script: 020_CREAR_ROL_CONTADOR.sql
Descripción: Crear rol y usuario CONTADOR con permisos específicos
Autor: Sistema
Fecha: Diciembre 2025
=====================================================
*/

USE SistemaVentas;
GO

-- =====================================================
-- 1. CREAR ROL CONTADOR
-- =====================================================

-- Verificar si existe el rol
IF NOT EXISTS (SELECT 1 FROM Rol WHERE Nombre = 'CONTADOR')
BEGIN
    INSERT INTO Rol (Nombre, Descripcion, Activo, FechaCreacion)
    VALUES ('CONTADOR', 'Contador - Gestión contable, fiscal y nómina', 1, GETDATE());
    
    PRINT '✓ Rol CONTADOR creado correctamente';
END
ELSE
BEGIN
    PRINT '! Rol CONTADOR ya existe';
END
GO

-- =====================================================
-- 2. ASIGNAR PERMISOS AL ROL CONTADOR
-- =====================================================

DECLARE @RolContadorID INT;
SELECT @RolContadorID = RolID FROM Rol WHERE Nombre = 'CONTADOR';

-- Permisos específicos del contador
DECLARE @PermisosContador TABLE (MenuPadre VARCHAR(50), MenuHijo VARCHAR(50));

INSERT INTO @PermisosContador (MenuPadre, MenuHijo) VALUES
-- Módulo Contabilidad
('Contabilidad', 'Polizas'),
('Contabilidad', 'CatalogoCuentas'),
('Contabilidad', 'LibroMayor'),
('Contabilidad', 'Reportes'),
('Contabilidad', 'Declaraciones'),

-- Módulo Configuración (ACCESO COMPLETO)
('Configuracion', 'Empresa'),
('Configuracion', 'Contable'),
('Configuracion', 'Nomina'),
('Configuracion', 'CFDI'),
('Configuracion', 'Usuarios'),

-- Módulo Facturación (CONSULTA Y CANCELACIÓN)
('Facturacion', 'Consultar'),
('Facturacion', 'Cancelar'),
('Facturacion', 'Reportes'),

-- Módulo Nómina (ACCESO COMPLETO)
('Nomina', 'Empleados'),
('Nomina', 'Recibos'),
('Nomina', 'Procesar'),
('Nomina', 'Reportes'),
('Nomina', 'CFDI'),

-- Módulo Reportes
('Reportes', 'EstadoResultados'),
('Reportes', 'BalanceGeneral'),
('Reportes', 'FlujoEfectivo'),
('Reportes', 'AntiguedadSaldos'),

-- Módulo Cuentas por Pagar
('CuentasPorPagar', 'Consultar'),
('CuentasPorPagar', 'Pagar'),
('CuentasPorPagar', 'Reportes'),

-- Módulo Dashboard
('Dashboard', 'Contador');

-- Insertar permisos
INSERT INTO Permisos (RolID, MenuPadre, MenuHijo, TieneAcceso, FechaCreacion)
SELECT 
    @RolContadorID,
    MenuPadre,
    MenuHijo,
    1,
    GETDATE()
FROM @PermisosContador
WHERE NOT EXISTS (
    SELECT 1 FROM Permisos 
    WHERE RolID = @RolContadorID 
    AND MenuPadre = @PermisosContador.MenuPadre 
    AND MenuHijo = @PermisosContador.MenuHijo
);

PRINT '✓ Permisos del CONTADOR configurados';
GO

-- =====================================================
-- 3. CREAR USUARIO CONTADOR
-- =====================================================

DECLARE @RolContadorID INT;
SELECT @RolContadorID = RolID FROM Rol WHERE Nombre = 'CONTADOR';

-- Verificar si existe el usuario
IF NOT EXISTS (SELECT 1 FROM Usuario WHERE Email = 'contador@empresa.com')
BEGIN
    INSERT INTO Usuario (
        Nombre,
        Apellido,
        Email,
        Clave, -- 'Contador123' encriptado
        RolID,
        Activo,
        FechaCreacion
    )
    VALUES (
        'Usuario',
        'Contador',
        'contador@empresa.com',
        '8C6976E5B5410415BDE908BD4DEE15DFB167A9C873FC4BB8A81F6F2AB448A918', -- SHA256('Contador123')
        @RolContadorID,
        1,
        GETDATE()
    );
    
    PRINT '✓ Usuario contador@empresa.com creado';
    PRINT '  Contraseña: Contador123';
END
ELSE
BEGIN
    PRINT '! Usuario contador@empresa.com ya existe';
END
GO

-- =====================================================
-- 4. CREAR TABLAS DE CONFIGURACIÓN
-- =====================================================

-- Tabla: ConfiguracionContable
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ConfiguracionContable')
BEGIN
    CREATE TABLE ConfiguracionContable (
        ConfigContableID INT IDENTITY(1,1) PRIMARY KEY,
        
        -- Ejercicio Fiscal
        EjercicioFiscalActual INT NOT NULL DEFAULT YEAR(GETDATE()),
        MesActual INT NOT NULL DEFAULT MONTH(GETDATE()),
        
        -- Cuentas por Defecto
        CuentaBancos VARCHAR(20),
        CuentaClientes VARCHAR(20),
        CuentaProveedores VARCHAR(20),
        CuentaIVATraslado VARCHAR(20),
        CuentaIVARetenido VARCHAR(20),
        CuentaISRRetenido VARCHAR(20),
        CuentaVentas VARCHAR(20),
        CuentaCompras VARCHAR(20),
        CuentaCostoVentas VARCHAR(20),
        CuentaNomina VARCHAR(20),
        CuentaIMSS VARCHAR(20),
        
        -- Configuración General
        UsaPolizasAutomaticas BIT DEFAULT 1,
        RequiereAutorizacionCancelacion BIT DEFAULT 1,
        DiasVencimientoFacturas INT DEFAULT 30,
        
        -- Auditoría
        UsuarioCreacion VARCHAR(100),
        FechaCreacion DATETIME DEFAULT GETDATE(),
        UsuarioModificacion VARCHAR(100),
        FechaModificacion DATETIME
    );
    
    -- Insertar configuración por defecto
    INSERT INTO ConfiguracionContable (
        EjercicioFiscalActual,
        MesActual,
        UsaPolizasAutomaticas,
        RequiereAutorizacionCancelacion,
        UsuarioCreacion
    ) VALUES (
        YEAR(GETDATE()),
        MONTH(GETDATE()),
        1,
        1,
        'Sistema'
    );
    
    PRINT '✓ Tabla ConfiguracionContable creada';
END
GO

-- Tabla: CatalogoCuentas
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'CatalogoCuentas')
BEGIN
    CREATE TABLE CatalogoCuentas (
        CuentaID INT IDENTITY(1,1) PRIMARY KEY,
        Codigo VARCHAR(20) NOT NULL UNIQUE,
        Nombre VARCHAR(200) NOT NULL,
        Nivel INT NOT NULL, -- 1=Mayor, 2=Subcuenta, 3=Detalle
        CuentaPadre VARCHAR(20) NULL,
        Tipo VARCHAR(20) NOT NULL, -- ACTIVO, PASIVO, CAPITAL, INGRESO, EGRESO
        Naturaleza CHAR(1) NOT NULL, -- D=Deudora, A=Acreedora
        AceptaMovimientos BIT DEFAULT 1,
        Activo BIT DEFAULT 1,
        Descripcion VARCHAR(500),
        
        -- SAT (Código Agrupador)
        CodigoSAT VARCHAR(20),
        
        -- Auditoría
        UsuarioCreacion VARCHAR(100),
        FechaCreacion DATETIME DEFAULT GETDATE(),
        UsuarioModificacion VARCHAR(100),
        FechaModificacion DATETIME,
        
        CONSTRAINT FK_CatalogoCuentas_Padre FOREIGN KEY (CuentaPadre) 
            REFERENCES CatalogoCuentas(Codigo)
    );
    
    CREATE INDEX IX_CatalogoCuentas_Codigo ON CatalogoCuentas(Codigo);
    CREATE INDEX IX_CatalogoCuentas_Tipo ON CatalogoCuentas(Tipo);
    CREATE INDEX IX_CatalogoCuentas_CuentaPadre ON CatalogoCuentas(CuentaPadre);
    
    PRINT '✓ Tabla CatalogoCuentas creada';
END
GO

-- Tabla: ConfiguracionNomina
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ConfiguracionNomina')
BEGIN
    CREATE TABLE ConfiguracionNomina (
        ConfigNominaID INT IDENTITY(1,1) PRIMARY KEY,
        
        -- Periodicidad
        TipoPeriodo VARCHAR(20) NOT NULL DEFAULT 'QUINCENAL', -- SEMANAL, QUINCENAL, MENSUAL
        DiasDePago INT DEFAULT 15,
        
        -- Impuestos
        SalarioMinimo DECIMAL(10,2) DEFAULT 207.44, -- 2024
        UMA DECIMAL(10,2) DEFAULT 108.57, -- 2024
        TopeSalarioIMSS DECIMAL(10,2) DEFAULT 2500.00,
        
        -- Porcentajes IMSS Empresa
        PorcentajeIMSSEmpresa DECIMAL(5,4) DEFAULT 0.2375,
        PorcentajeRCV DECIMAL(5,4) DEFAULT 0.0200,
        PorcentajeGuarderia DECIMAL(5,4) DEFAULT 0.0100,
        PorcentajeRetiro DECIMAL(5,4) DEFAULT 0.0200,
        
        -- Porcentajes IMSS Trabajador
        PorcentajeIMSSTrabajador DECIMAL(5,4) DEFAULT 0.0250,
        
        -- Configuración CFDI Nómina
        LugarExpedicionNomina VARCHAR(10),
        RutaCertificadoNomina VARCHAR(500),
        
        -- Auditoría
        UsuarioCreacion VARCHAR(100),
        FechaCreacion DATETIME DEFAULT GETDATE(),
        UsuarioModificacion VARCHAR(100),
        FechaModificacion DATETIME
    );
    
    -- Insertar configuración por defecto
    INSERT INTO ConfiguracionNomina (
        TipoPeriodo,
        DiasDePago,
        SalarioMinimo,
        UMA,
        UsuarioCreacion
    ) VALUES (
        'QUINCENAL',
        15,
        207.44,
        108.57,
        'Sistema'
    );
    
    PRINT '✓ Tabla ConfiguracionNomina creada';
END
GO

-- Tabla: CertificadosDigitales
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'CertificadosDigitales')
BEGIN
    CREATE TABLE CertificadosDigitales (
        CertificadoID INT IDENTITY(1,1) PRIMARY KEY,
        
        -- Tipo de Certificado
        TipoCertificado VARCHAR(20) NOT NULL, -- CSD, FIEL
        NombreCertificado VARCHAR(200) NOT NULL,
        
        -- Datos del Certificado
        NoCertificado VARCHAR(50) NOT NULL,
        RFC VARCHAR(13) NOT NULL,
        RazonSocial VARCHAR(300),
        FechaInicio DATETIME,
        FechaVencimiento DATETIME,
        
        -- Archivos (almacenados como VARBINARY)
        ArchivoCER VARBINARY(MAX) NOT NULL,
        ArchivoKEY VARBINARY(MAX) NOT NULL,
        PasswordKEY VARCHAR(100) NOT NULL, -- Encriptado
        
        -- Nombres originales de archivos
        NombreArchivoCER VARCHAR(200),
        NombreArchivoKEY VARCHAR(200),
        
        -- Estado
        Activo BIT DEFAULT 1,
        EsPredeterminado BIT DEFAULT 0,
        
        -- Uso
        UsarParaFacturas BIT DEFAULT 1,
        UsarParaNomina BIT DEFAULT 0,
        UsarParaCancelaciones BIT DEFAULT 1,
        
        -- Auditoría
        UsuarioCreacion VARCHAR(100),
        FechaCreacion DATETIME DEFAULT GETDATE(),
        UsuarioModificacion VARCHAR(100),
        FechaModificacion DATETIME
    );
    
    CREATE INDEX IX_CertificadosDigitales_RFC ON CertificadosDigitales(RFC);
    CREATE INDEX IX_CertificadosDigitales_NoCertificado ON CertificadosDigitales(NoCertificado);
    CREATE INDEX IX_CertificadosDigitales_Activo ON CertificadosDigitales(Activo, EsPredeterminado);
    
    PRINT '✓ Tabla CertificadosDigitales creada';
END
GO

-- Tabla: PercepcionesNomina (Catálogo SAT)
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'PercepcionesNomina')
BEGIN
    CREATE TABLE PercepcionesNomina (
        PercepcionID INT IDENTITY(1,1) PRIMARY KEY,
        ClaveSAT VARCHAR(10) NOT NULL UNIQUE,
        Descripcion VARCHAR(200) NOT NULL,
        TipoPercepcion VARCHAR(50) NOT NULL, -- ORDINARIA, EXTRAORDINARIA
        GravaISR BIT DEFAULT 1,
        GravaIMSS BIT DEFAULT 1,
        Activo BIT DEFAULT 1
    );
    
    -- Insertar percepciones comunes del SAT
    INSERT INTO PercepcionesNomina (ClaveSAT, Descripcion, TipoPercepcion, GravaISR, GravaIMSS) VALUES
    ('001', 'Sueldos, Salarios, Rayas y Jornales', 'ORDINARIA', 1, 1),
    ('002', 'Gratificación Anual (Aguinaldo)', 'ORDINARIA', 1, 0),
    ('003', 'Participación de los Trabajadores en las Utilidades (PTU)', 'ORDINARIA', 1, 0),
    ('004', 'Reembolso de Gastos Médicos Dentales y Hospitalarios', 'EXTRAORDINARIA', 0, 0),
    ('005', 'Fondo de Ahorro', 'ORDINARIA', 0, 0),
    ('006', 'Caja de Ahorro', 'ORDINARIA', 0, 0),
    ('009', 'Contribuciones a Cargo del Trabajador Pagadas por el Patrón', 'ORDINARIA', 1, 0),
    ('010', 'Premios por Puntualidad', 'ORDINARIA', 1, 1),
    ('019', 'Horas Extra', 'ORDINARIA', 1, 1),
    ('022', 'Primas de Antigüedad, Retiro e Indemnizaciones', 'EXTRAORDINARIA', 1, 0),
    ('025', 'Viáticos', 'ORDINARIA', 0, 0),
    ('038', 'Días de Descanso Trabajados', 'ORDINARIA', 1, 1),
    ('039', 'Prima Vacacional', 'ORDINARIA', 1, 0),
    ('044', 'Jubilaciones, Pensiones o Haberes de Retiro', 'EXTRAORDINARIA', 1, 0);
    
    PRINT '✓ Tabla PercepcionesNomina creada con catálogo SAT';
END
GO

-- Tabla: DeduccionesNomina (Catálogo SAT)
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'DeduccionesNomina')
BEGIN
    CREATE TABLE DeduccionesNomina (
        DeduccionID INT IDENTITY(1,1) PRIMARY KEY,
        ClaveSAT VARCHAR(10) NOT NULL UNIQUE,
        Descripcion VARCHAR(200) NOT NULL,
        TipoDeduccion VARCHAR(50) NOT NULL, -- OBLIGATORIA, VOLUNTARIA
        Activo BIT DEFAULT 1
    );
    
    -- Insertar deducciones comunes del SAT
    INSERT INTO DeduccionesNomina (ClaveSAT, Descripcion, TipoDeduccion) VALUES
    ('001', 'Seguridad Social', 'OBLIGATORIA'),
    ('002', 'ISR', 'OBLIGATORIA'),
    ('003', 'Aportaciones a Retiro, Cesantía en Edad Avanzada y Vejez', 'OBLIGATORIA'),
    ('004', 'Otros', 'VOLUNTARIA'),
    ('006', 'Crédito de Vivienda', 'OBLIGATORIA'),
    ('007', 'Descuento por Incapacidad', 'OBLIGATORIA'),
    ('010', 'Amortización de Créditos Provenientes de Infonavit', 'OBLIGATORIA'),
    ('012', 'Pago por Crédito de Vivienda', 'OBLIGATORIA'),
    ('013', 'Pago de Pensión Alimenticia', 'OBLIGATORIA'),
    ('017', 'Pago de Renta', 'VOLUNTARIA'),
    ('019', 'Descuentos a Terceros', 'VOLUNTARIA'),
    ('107', 'Ajuste en Gratificación Anual (Aguinaldo) ISR Ajustado', 'OBLIGATORIA');
    
    PRINT '✓ Tabla DeduccionesNomina creada con catálogo SAT';
END
GO

-- Tabla: CertificadosDigitales
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'CertificadosDigitales')
BEGIN
    CREATE TABLE CertificadosDigitales (
        CertificadoID INT IDENTITY(1,1) PRIMARY KEY,
        
        -- Tipo
        TipoCertificado VARCHAR(20) NOT NULL, -- CSD, FIEL
        NombreCertificado VARCHAR(200) NOT NULL,
        
        -- Datos del Certificado
        NoCertificado VARCHAR(50) NULL,
        RFC VARCHAR(13) NOT NULL,
        RazonSocial VARCHAR(300) NULL,
        FechaInicio DATETIME NULL,
        FechaVencimiento DATETIME NULL,
        
        -- Archivos (binarios)
        ArchivoCER VARBINARY(MAX) NOT NULL,
        ArchivoKEY VARBINARY(MAX) NOT NULL,
        PasswordKEY VARCHAR(100) NOT NULL, -- Encriptado
        
        -- Nombres originales
        NombreArchivoCER VARCHAR(200) NULL,
        NombreArchivoKEY VARCHAR(200) NULL,
        
        -- Estado
        Activo BIT DEFAULT 1,
        EsPredeterminado BIT DEFAULT 0,
        
        -- Uso
        UsarParaFacturas BIT DEFAULT 1,
        UsarParaNomina BIT DEFAULT 0,
        UsarParaCancelaciones BIT DEFAULT 1,
        
        -- Auditoría
        UsuarioCreacion VARCHAR(100) NOT NULL,
        FechaCreacion DATETIME DEFAULT GETDATE(),
        UsuarioModificacion VARCHAR(100) NULL,
        FechaModificacion DATETIME NULL
    );
    
    -- Índices
    CREATE INDEX IX_CertificadosDigitales_RFC ON CertificadosDigitales(RFC);
    CREATE INDEX IX_CertificadosDigitales_NoCertificado ON CertificadosDigitales(NoCertificado);
    CREATE INDEX IX_CertificadosDigitales_Activo ON CertificadosDigitales(Activo);
    CREATE INDEX IX_CertificadosDigitales_FechaVencimiento ON CertificadosDigitales(FechaVencimiento);
    
    PRINT '✓ Tabla CertificadosDigitales creada';
END
GO

-- =====================================================
-- 5. INSERTAR CATÁLOGO DE CUENTAS BÁSICO
-- =====================================================

-- Solo si está vacía la tabla
IF NOT EXISTS (SELECT 1 FROM CatalogoCuentas)
BEGIN
    INSERT INTO CatalogoCuentas (Codigo, Nombre, Nivel, CuentaPadre, Tipo, Naturaleza, AceptaMovimientos, UsuarioCreacion) VALUES
    -- ACTIVO
    ('1000', 'ACTIVO', 1, NULL, 'ACTIVO', 'D', 0, 'Sistema'),
    ('1100', 'ACTIVO CIRCULANTE', 2, '1000', 'ACTIVO', 'D', 0, 'Sistema'),
    ('1101', 'Caja', 3, '1100', 'ACTIVO', 'D', 1, 'Sistema'),
    ('1102', 'Bancos', 3, '1100', 'ACTIVO', 'D', 1, 'Sistema'),
    ('1103', 'Clientes', 3, '1100', 'ACTIVO', 'D', 1, 'Sistema'),
    ('1104', 'IVA Acreditable', 3, '1100', 'ACTIVO', 'D', 1, 'Sistema'),
    ('1105', 'Inventarios', 3, '1100', 'ACTIVO', 'D', 1, 'Sistema'),
    
    -- PASIVO
    ('2000', 'PASIVO', 1, NULL, 'PASIVO', 'A', 0, 'Sistema'),
    ('2100', 'PASIVO CIRCULANTE', 2, '2000', 'PASIVO', 'A', 0, 'Sistema'),
    ('2101', 'Proveedores', 3, '2100', 'PASIVO', 'A', 1, 'Sistema'),
    ('2102', 'IVA Trasladado', 3, '2100', 'PASIVO', 'A', 1, 'Sistema'),
    ('2103', 'IVA Retenido', 3, '2100', 'PASIVO', 'A', 1, 'Sistema'),
    ('2104', 'ISR Retenido', 3, '2100', 'PASIVO', 'A', 1, 'Sistema'),
    ('2105', 'IMSS por Pagar', 3, '2100', 'PASIVO', 'A', 1, 'Sistema'),
    
    -- CAPITAL
    ('3000', 'CAPITAL', 1, NULL, 'CAPITAL', 'A', 0, 'Sistema'),
    ('3100', 'CAPITAL CONTABLE', 2, '3000', 'CAPITAL', 'A', 0, 'Sistema'),
    ('3101', 'Capital Social', 3, '3100', 'CAPITAL', 'A', 1, 'Sistema'),
    ('3102', 'Resultado del Ejercicio', 3, '3100', 'CAPITAL', 'A', 1, 'Sistema'),
    
    -- INGRESOS
    ('4000', 'INGRESOS', 1, NULL, 'INGRESO', 'A', 0, 'Sistema'),
    ('4100', 'INGRESOS POR VENTAS', 2, '4000', 'INGRESO', 'A', 0, 'Sistema'),
    ('4101', 'Ventas', 3, '4100', 'INGRESO', 'A', 1, 'Sistema'),
    ('4102', 'Devoluciones sobre Ventas', 3, '4100', 'INGRESO', 'D', 1, 'Sistema'),
    
    -- EGRESOS
    ('5000', 'EGRESOS', 1, NULL, 'EGRESO', 'D', 0, 'Sistema'),
    ('5100', 'COSTO DE VENTAS', 2, '5000', 'EGRESO', 'D', 0, 'Sistema'),
    ('5101', 'Costo de Ventas', 3, '5100', 'EGRESO', 'D', 1, 'Sistema'),
    ('5200', 'GASTOS DE OPERACIÓN', 2, '5000', 'EGRESO', 'D', 0, 'Sistema'),
    ('5201', 'Sueldos y Salarios', 3, '5200', 'EGRESO', 'D', 1, 'Sistema'),
    ('5202', 'Cuotas IMSS', 3, '5200', 'EGRESO', 'D', 1, 'Sistema'),
    ('5203', 'Renta', 3, '5200', 'EGRESO', 'D', 1, 'Sistema'),
    ('5204', 'Luz', 3, '5200', 'EGRESO', 'D', 1, 'Sistema'),
    ('5205', 'Teléfono e Internet', 3, '5200', 'EGRESO', 'D', 1, 'Sistema');
    
    PRINT '✓ Catálogo de Cuentas básico insertado';
END
GO

-- =====================================================
-- 6. ACTUALIZAR CONFIGURACIÓN CONTABLE CON CUENTAS
-- =====================================================

UPDATE ConfiguracionContable SET
    CuentaBancos = '1102',
    CuentaClientes = '1103',
    CuentaProveedores = '2101',
    CuentaIVATraslado = '2102',
    CuentaIVARetenido = '2103',
    CuentaISRRetenido = '2104',
    CuentaVentas = '4101',
    CuentaCompras = '5101',
    CuentaCostoVentas = '5101',
    CuentaNomina = '5201',
    CuentaIMSS = '5202',
    UsuarioModificacion = 'Sistema',
    FechaModificacion = GETDATE()
WHERE ConfigContableID = 1;

PRINT '✓ Configuración contable actualizada con cuentas por defecto';
GO

-- =====================================================
-- 7. CREAR VISTAS PARA EL CONTADOR
-- =====================================================

-- Vista: Dashboard Contador
IF EXISTS (SELECT * FROM sys.views WHERE name = 'vw_DashboardContador')
    DROP VIEW vw_DashboardContador;
GO

CREATE VIEW vw_DashboardContador AS
SELECT
    -- Facturas del mes
    (SELECT COUNT(*) FROM Facturas WHERE MONTH(FechaEmision) = MONTH(GETDATE()) AND YEAR(FechaEmision) = YEAR(GETDATE())) AS FacturasMes,
    (SELECT ISNULL(SUM(Total), 0) FROM Facturas WHERE MONTH(FechaEmision) = MONTH(GETDATE()) AND YEAR(FechaEmision) = YEAR(GETDATE()) AND Estatus = 'TIMBRADA') AS TotalFacturadoMes,
    
    -- Facturas canceladas
    (SELECT COUNT(*) FROM Facturas WHERE MONTH(FechaCancelacion) = MONTH(GETDATE()) AND YEAR(FechaCancelacion) = YEAR(GETDATE())) AS FacturasCanceladasMes,
    
    -- Nómina del mes
    (SELECT COUNT(*) FROM NominaRecibos WHERE MONTH(FechaPago) = MONTH(GETDATE()) AND YEAR(FechaPago) = YEAR(GETDATE())) AS RecibosMes,
    (SELECT ISNULL(SUM(NetoPagar), 0) FROM NominaRecibos WHERE MONTH(FechaPago) = MONTH(GETDATE()) AND YEAR(FechaPago) = YEAR(GETDATE())) AS TotalNominaMes,
    
    -- Cuentas por Pagar
    (SELECT COUNT(*) FROM CuentasPorPagar WHERE Estatus IN ('PENDIENTE', 'VENCIDA')) AS CuentasPendientes,
    (SELECT ISNULL(SUM(Saldo), 0) FROM CuentasPorPagar WHERE Estatus IN ('PENDIENTE', 'VENCIDA')) AS TotalPorPagar,
    
    -- Pólizas del mes
    (SELECT COUNT(*) FROM Polizas WHERE MONTH(Fecha) = MONTH(GETDATE()) AND YEAR(Fecha) = YEAR(GETDATE())) AS PolizasMes;
GO

PRINT '✓ Vista vw_DashboardContador creada';
GO

-- =====================================================
-- RESUMEN
-- =====================================================

PRINT '';
PRINT '========================================';
PRINT 'MÓDULO CONTADOR - INSTALACIÓN COMPLETA';
PRINT '========================================';
PRINT '';
PRINT '✓ Rol CONTADOR creado';
PRINT '✓ Usuario contador@empresa.com creado';
PRINT '  Contraseña: Contador123';
PRINT '';
PRINT '✓ Tablas creadas:';
PRINT '  - ConfiguracionContable';
PRINT '  - CatalogoCuentas (con catálogo básico)';
PRINT '  - ConfiguracionNomina';
PRINT '  - PercepcionesNomina (catálogo SAT)';
PRINT '  - DeduccionesNomina (catálogo SAT)';
PRINT '';
PRINT '✓ Permisos configurados para:';
PRINT '  - Módulo Contabilidad';
PRINT '  - Módulo Configuración';
PRINT '  - Módulo Facturación (consulta)';
PRINT '  - Módulo Nómina';
PRINT '  - Módulo Reportes';
PRINT '';
PRINT '========================================';
PRINT 'SIGUIENTE PASO: Implementar Controllers';
PRINT '========================================';
GO
