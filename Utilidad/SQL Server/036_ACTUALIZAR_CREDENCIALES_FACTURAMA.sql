-- ========================================================
-- SCRIPT: Actualizar credenciales de Facturama
-- ========================================================
-- INSTRUCCIONES:
-- 1. Crear cuenta en https://facturama.mx/
-- 2. Ir a Configuración → API
-- 3. Copiar Usuario y Contraseña
-- 4. Reemplazar los valores abajo
-- 5. Ejecutar este script
-- ========================================================

USE DB_TIENDA
GO

-- ⚠️ REEMPLAZA ESTOS VALORES CON TUS CREDENCIALES REALES
DECLARE @Usuario VARCHAR(100) = 'TU_USUARIO_AQUI';
DECLARE @Password VARCHAR(100) = 'TU_PASSWORD_AQUI';

-- Actualizar configuración
UPDATE ConfiguracionPAC
SET 
    Usuario = @Usuario,
    Password = @Password,
    UrlTimbrado = 'https://apisandbox.facturama.mx/2/cfdis',
    UrlCancelacion = 'https://apisandbox.facturama.mx/cfdi',
    UrlConsulta = 'https://apisandbox.facturama.mx/cfdi',
    EsProduccion = 0,  -- 0 = Sandbox/Pruebas
    Activo = 1
WHERE ProveedorPAC = 'Facturama';

-- Verificar
SELECT 
    ProveedorPAC,
    Usuario,
    REPLICATE('*', LEN(Password)) AS Password_Oculto,
    UrlTimbrado,
    CASE EsProduccion WHEN 0 THEN 'SANDBOX (Pruebas)' ELSE 'PRODUCCION' END AS Ambiente,
    CASE Activo WHEN 1 THEN 'ACTIVO' ELSE 'INACTIVO' END AS Estado
FROM ConfiguracionPAC
WHERE ProveedorPAC = 'Facturama';

PRINT '';
PRINT '========================================================';
PRINT 'PASOS PARA OBTENER CREDENCIALES DE FACTURAMA:';
PRINT '========================================================';
PRINT '1. Ir a https://facturama.mx/';
PRINT '2. Click en "CREAR CUENTA GRATIS" o "INICIAR SESIÓN"';
PRINT '3. Crear cuenta con tu email';
PRINT '4. Ir a Configuración → API';
PRINT '5. Copiar Usuario y Token/Password';
PRINT '6. Actualizar este script y ejecutar';
PRINT '========================================================';
GO
