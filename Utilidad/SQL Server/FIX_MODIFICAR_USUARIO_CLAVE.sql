-- =============================================
-- FIX: Permitir actualizar contraseña al modificar usuario
-- Fecha: 2025-12-16
-- =============================================

USE DBVENTAS_WEB
GO

-- Eliminar procedimiento anterior
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'usp_ModificarUsuario')
DROP PROCEDURE usp_ModificarUsuario
GO

-- Crear procedimiento mejorado que actualiza la contraseña
CREATE PROCEDURE usp_ModificarUsuario(
    @UsuarioID int,
    @Nombres varchar(50),
    @Apellidos varchar(50),
    @Correo varchar(50),
    @Clave varchar(100), -- Aumentado para soportar hash SHA256 (64 chars)
    @SucursalID int,
    @RolID int,
    @Activo bit,
    @Resultado bit output
)
AS
BEGIN
    SET @Resultado = 1
    
    -- Verificar que el correo no esté en uso por otro usuario
    IF NOT EXISTS (SELECT * FROM USUARIO WHERE Correo = @Correo AND UsuarioID != @UsuarioID)
    BEGIN
        -- Actualizar usuario INCLUYENDO la contraseña
        UPDATE USUARIO SET 
            Nombres = @Nombres,
            Apellidos = @Apellidos,
            Correo = @Correo,
            Clave = @Clave, -- AHORA SE ACTUALIZA LA CONTRASEÑA
            SucursalID = @SucursalID,
            RolID = @RolID,
            Activo = @Activo
        WHERE UsuarioID = @UsuarioID
    END
    ELSE
    BEGIN
        SET @Resultado = 0
    END
END
GO

PRINT '✅ Stored procedure usp_ModificarUsuario actualizado correctamente'
PRINT '   Ahora permite cambiar la contraseña al modificar un usuario'
GO
