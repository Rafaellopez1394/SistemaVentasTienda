-- =====================================================
-- Script MEJORADO para corregir encoding en TODAS las tablas
-- Corrige caracteres mal codificados (dÃ©bito → débito)
-- Versión 2.0 - Automático y completo
-- =====================================================

USE DB_TIENDA;
GO

SET NOCOUNT ON;
GO

PRINT '=======================================================';
PRINT '🔧 CORRECCIÓN AUTOMÁTICA DE ENCODING UTF-8';
PRINT '=======================================================';
PRINT '';

-- Variables
DECLARE @TableName NVARCHAR(128);
DECLARE @ColumnName NVARCHAR(128);
DECLARE @DataType NVARCHAR(128);
DECLARE @SQL NVARCHAR(MAX);
DECLARE @RowsAffected INT = 0;
DECLARE @TotalRows INT = 0;

-- Cursor para todas las columnas de texto en todas las tablas
DECLARE column_cursor CURSOR FOR
SELECT 
    t.TABLE_NAME,
    c.COLUMN_NAME,
    c.DATA_TYPE
FROM INFORMATION_SCHEMA.TABLES t
INNER JOIN INFORMATION_SCHEMA.COLUMNS c ON t.TABLE_NAME = c.TABLE_NAME
WHERE t.TABLE_TYPE = 'BASE TABLE'
    AND c.DATA_TYPE IN ('varchar', 'nvarchar', 'char', 'nchar', 'text', 'ntext')
    AND t.TABLE_NAME NOT LIKE 'sys%'
ORDER BY t.TABLE_NAME, c.COLUMN_NAME;

OPEN column_cursor;

FETCH NEXT FROM column_cursor INTO @TableName, @ColumnName, @DataType;

WHILE @@FETCH_STATUS = 0
BEGIN
    -- Verificar si hay datos con problemas de encoding
    SET @SQL = N'SELECT @Count = COUNT(*) FROM [' + @TableName + '] WHERE [' + @ColumnName + '] LIKE ''%Ã%''';
    
    DECLARE @Count INT;
    EXEC sp_executesql @SQL, N'@Count INT OUTPUT', @Count = @Count OUTPUT;
    
    IF @Count > 0
    BEGIN
        PRINT '📋 Corrigiendo: ' + @TableName + '.' + @ColumnName + ' (' + CAST(@Count AS VARCHAR(10)) + ' registros)';
        
        -- Ejecutar UPDATE para corregir el encoding
        SET @SQL = N'
        UPDATE [' + @TableName + ']
        SET [' + @ColumnName + '] = 
            REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(
            REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(
                [' + @ColumnName + '],
                ''Ã©'', ''é''),   -- é
                ''Ã¡'', ''á''),   -- á
                ''Ã­'', ''í''),   -- í
                ''Ã³'', ''ó''),   -- ó
                ''Ãº'', ''ú''),   -- ú
                ''Ã±'', ''ñ''),   -- ñ
                ''Ã'', ''Ñ''),    -- Ñ
                ''Ã‰'', ''É''),   -- É
                ''Ãƒ'', ''Á''),   -- Á
                ''Ã'', ''Í''),    -- Í
                ''Ã"'', ''Ó''),   -- Ó
                ''Ãš'', ''Ú''),   -- Ú
                ''Ã¼'', ''ü''),   -- ü
                ''Ãœ'', ''Ü''),   -- Ü
                ''Â¿'', ''¿''),   -- ¿
                ''Â¡'', ''¡''),   -- ¡
                ''Â°'', ''°''),   -- °
                ''â‚¬'', ''€''),  -- €
                ''â€œ'', ''"''),  -- "
                ''â€'', ''"'')   -- "
        WHERE [' + @ColumnName + '] LIKE ''%Ã%'' OR [' + @ColumnName + '] LIKE ''%Â%'' OR [' + @ColumnName + '] LIKE ''%â%'';
        ';
        
        EXEC sp_executesql @SQL;
        
        SET @RowsAffected = @@ROWCOUNT;
        SET @TotalRows = @TotalRows + @RowsAffected;
        
        PRINT '   ✅ ' + CAST(@RowsAffected AS VARCHAR(10)) + ' registros corregidos';
    END
    
    FETCH NEXT FROM column_cursor INTO @TableName, @ColumnName, @DataType;
END

CLOSE column_cursor;
DEALLOCATE column_cursor;

PRINT '';
PRINT '=======================================================';
PRINT '✅ PROCESO COMPLETADO';
PRINT '📊 Total de registros corregidos: ' + CAST(@TotalRows AS VARCHAR(10));
PRINT '=======================================================';
PRINT '';
PRINT '🔍 Verificar resultados con:';
PRINT '   SELECT * FROM CatFormasPago;';
PRINT '   SELECT * FROM CatMetodosPago;';
PRINT '   SELECT * FROM CatCuentas WHERE NombreCuenta LIKE ''%é%'' OR NombreCuenta LIKE ''%á%'';';
PRINT '';

SET NOCOUNT OFF;
GO
