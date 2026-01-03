-- =====================================================
-- CONFIGURAR PERMISOS POR ROL
-- Fecha: 2025-12-16
-- =====================================================

USE DBVENTAS_WEB
GO

PRINT '=== CONFIGURANDO PERMISOS POR ROL ==='
PRINT ''

-- =====================================================
-- 1. CREAR ROL CONTADOR (si no existe)
-- =====================================================

IF NOT EXISTS (SELECT 1 FROM ROL WHERE Descripcion = 'CONTADOR')
BEGIN
    INSERT INTO ROL(Descripcion) VALUES ('CONTADOR')
    PRINT '✓ Rol CONTADOR creado'
END
ELSE
BEGIN
    PRINT '  Rol CONTADOR ya existe'
END
GO

-- =====================================================
-- 2. CREAR USUARIO CONTADOR
-- =====================================================

DECLARE @RolContadorID INT
SELECT @RolContadorID = RolID FROM ROL WHERE Descripcion = 'CONTADOR'

IF NOT EXISTS (SELECT 1 FROM usuario WHERE Correo = 'contador@gmail.com')
BEGIN
    INSERT INTO usuario(Nombres,Apellidos,Correo,Clave,SucursalID,RolID)
    VALUES('Contador','Fiscal','contador@gmail.com',
           '8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92', -- 123456
           (SELECT TOP 1 SucursalID FROM SUCURSAL),
           @RolContadorID)
    
    PRINT '✓ Usuario contador@gmail.com creado'
    PRINT '  Contraseña: 123456'
END
ELSE
BEGIN
    PRINT '  Usuario contador@gmail.com ya existe'
END
GO

-- =====================================================
-- 3. CONFIGURAR PERMISOS PARA ROL EMPLEADO
-- =====================================================

DECLARE @RolEmpleadoID INT
SELECT @RolEmpleadoID = RolID FROM ROL WHERE Descripcion = 'EMPLEADO'

-- Activar solo permisos básicos para empleado
UPDATE PERMISOS 
SET Activo = 0 
WHERE RolID = @RolEmpleadoID

-- POS y Ventas
UPDATE p SET Activo = 1
FROM PERMISOS p
INNER JOIN SUBMENU s ON p.SubMenuID = s.SubMenuID
INNER JOIN MENU m ON s.MenuID = m.MenuID
WHERE p.RolID = @RolEmpleadoID
AND (
    -- Clientes
    (m.Nombre = 'Clientes') OR
    -- Ventas
    (m.Nombre = 'Ventas' AND s.Nombre IN ('Registrar Venta', 'Consultar Venta')) OR
    -- Productos (solo consulta)
    (m.Nombre = 'Mantenedor' AND s.Nombre = 'Productos')
)

PRINT '✓ Permisos de EMPLEADO configurados'
PRINT '  - Acceso a: POS, Ventas, Clientes, Productos (consulta)'
GO

-- =====================================================
-- 4. CONFIGURAR PERMISOS PARA ROL CONTADOR
-- =====================================================

DECLARE @RolContadorID INT
SELECT @RolContadorID = RolID FROM ROL WHERE Descripcion = 'CONTADOR'

-- Primero, insertar permisos si no existen
INSERT INTO PERMISOS(RolID, SubMenuID, Activo)
SELECT @RolContadorID, SubMenuID, 1
FROM SUBMENU
WHERE NOT EXISTS (
    SELECT 1 FROM PERMISOS 
    WHERE RolID = @RolContadorID 
    AND SubMenuID = SUBMENU.SubMenuID
)

-- Desactivar todos los permisos del contador
UPDATE PERMISOS 
SET Activo = 0 
WHERE RolID = @RolContadorID

-- Activar solo permisos contables
UPDATE p SET Activo = 1
FROM PERMISOS p
INNER JOIN SUBMENU s ON p.SubMenuID = s.SubMenuID
INNER JOIN MENU m ON s.MenuID = m.MenuID
WHERE p.RolID = @RolContadorID
AND (
    -- Reportes
    (m.Nombre = 'Reportes') OR
    -- Solo visualización de ventas/compras
    (m.Nombre = 'Ventas' AND s.Nombre = 'Consultar Venta') OR
    (m.Nombre = 'Compras' AND s.Nombre = 'Consultar Compra')
)

PRINT '✓ Permisos de CONTADOR configurados'
PRINT '  - Acceso a: Contabilidad, Pólizas, Nómina, Reportes'
GO

-- =====================================================
-- 5. RESUMEN DE CONFIGURACIÓN
-- =====================================================

PRINT ''
PRINT '=== RESUMEN DE USUARIOS Y ROLES ==='
PRINT ''

SELECT 
    u.Nombres + ' ' + u.Apellidos AS Usuario,
    u.Correo,
    r.Descripcion AS Rol,
    CASE WHEN u.Activo = 1 THEN 'Activo' ELSE 'Inactivo' END AS Estado
FROM usuario u
INNER JOIN ROL r ON u.RolID = r.RolID
ORDER BY r.Descripcion, u.Nombres

PRINT ''
PRINT '=== CREDENCIALES DE ACCESO ==='
PRINT ''
PRINT '📧 ADMINISTRADOR'
PRINT '   Email: admin@gmail.com'
PRINT '   Contraseña: admin123'
PRINT ''
PRINT '📧 EMPLEADO (Sucursal)'
PRINT '   Email: sucursal@gmail.com'
PRINT '   Contraseña: 123456'
PRINT ''
PRINT '📧 CONTADOR'
PRINT '   Email: contador@gmail.com'
PRINT '   Contraseña: 123456'
PRINT ''
PRINT '✅ Configuración completada'
GO
