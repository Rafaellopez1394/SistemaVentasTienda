-- ========================================================
-- AGREGAR COLUMNA SucursalID A LotesProducto
-- Para manejar inventario por sucursal
-- ========================================================
USE DB_TIENDA
GO

PRINT '🔧 Agregando columna SucursalID a LotesProducto...'

-- Verificar si ya existe la columna
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
               WHERE TABLE_NAME = 'LotesProducto' AND COLUMN_NAME = 'SucursalID')
BEGIN
    -- Agregar columna SucursalID como nullable primero
    ALTER TABLE LotesProducto
    ADD SucursalID INT NULL;
    
    PRINT '✅ Columna SucursalID agregada'
END
ELSE
BEGIN
    PRINT '⚠️ La columna SucursalID ya existe'
END
GO

-- Actualizar lotes existentes con la primera sucursal disponible
DECLARE @PrimeraSucursal INT;
SELECT TOP 1 @PrimeraSucursal = SucursalID FROM SUCURSAL ORDER BY SucursalID;

IF @PrimeraSucursal IS NOT NULL
BEGIN
    UPDATE LotesProducto
    SET SucursalID = @PrimeraSucursal
    WHERE SucursalID IS NULL;
    
    PRINT '✅ Lotes existentes asignados a sucursal ' + CAST(@PrimeraSucursal AS VARCHAR(10))
END
GO

-- Agregar foreign key constraint si no existe
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_LotesProducto_Sucursal')
BEGIN
    ALTER TABLE LotesProducto
    ADD CONSTRAINT FK_LotesProducto_Sucursal 
    FOREIGN KEY (SucursalID) REFERENCES SUCURSAL(SucursalID);
    
    PRINT '✅ Foreign Key agregada'
END
GO

-- Hacer la columna NOT NULL ahora que todos tienen valor
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
           WHERE TABLE_NAME = 'LotesProducto' AND COLUMN_NAME = 'SucursalID' AND IS_NULLABLE = 'YES')
BEGIN
    ALTER TABLE LotesProducto
    ALTER COLUMN SucursalID INT NOT NULL;
    
    PRINT '✅ Columna SucursalID configurada como NOT NULL'
END
GO

-- Verificar resultado
SELECT 
    COLUMN_NAME,
    DATA_TYPE,
    IS_NULLABLE,
    COLUMN_DEFAULT
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'LotesProducto'
  AND COLUMN_NAME = 'SucursalID';

PRINT ''
PRINT '📊 Resumen de lotes por sucursal:'
SELECT 
    S.Nombre AS Sucursal,
    COUNT(*) AS TotalLotes,
    SUM(L.CantidadDisponible) AS UnidadesDisponibles
FROM LotesProducto L
INNER JOIN SUCURSAL S ON L.SucursalID = S.SucursalID
GROUP BY S.Nombre
ORDER BY S.Nombre;

PRINT ''
PRINT '✅ Estructura actualizada correctamente'
GO
