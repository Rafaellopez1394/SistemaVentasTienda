// Scripts/Utilidades/FechaUtils.js
// Utilidades para manejo de fechas .NET

/**
 * Convierte fechas del formato .NET /Date(timestamp)/ a formato legible DD/MM/YYYY HH:mm
 * @param {string} fechaDotNet - Fecha en formato /Date(1234567890)/
 * @returns {string} Fecha formateada o 'N/A' si es inválida
 */
function convertirFechaDotNet(fechaDotNet) {
    if (!fechaDotNet) return 'N/A';
    
    // Si ya es una fecha normal, retornarla
    if (!fechaDotNet.toString().includes('/Date(')) {
        return fechaDotNet;
    }
    
    try {
        // Extraer el timestamp del formato /Date(1234567890)/
        var timestamp = parseInt(fechaDotNet.replace(/\/Date\((\d+)\)\//, '$1'));
        var fecha = new Date(timestamp);
        
        // Validar que la fecha sea válida
        if (isNaN(fecha.getTime())) {
            return 'N/A';
        }
        
        // Formatear como DD/MM/YYYY HH:mm
        var dia = ('0' + fecha.getDate()).slice(-2);
        var mes = ('0' + (fecha.getMonth() + 1)).slice(-2);
        var anio = fecha.getFullYear();
        var horas = ('0' + fecha.getHours()).slice(-2);
        var minutos = ('0' + fecha.getMinutes()).slice(-2);
        
        return dia + '/' + mes + '/' + anio + ' ' + horas + ':' + minutos;
    } catch (e) {
        console.error('Error al convertir fecha:', fechaDotNet, e);
        return 'N/A';
    }
}

/**
 * Convierte fechas del formato .NET /Date(timestamp)/ a formato corto DD/MM/YYYY
 * @param {string} fechaDotNet - Fecha en formato /Date(1234567890)/
 * @returns {string} Fecha formateada o 'N/A' si es inválida
 */
function convertirFechaDotNetCorta(fechaDotNet) {
    if (!fechaDotNet) return 'N/A';
    
    if (!fechaDotNet.toString().includes('/Date(')) {
        return fechaDotNet;
    }
    
    try {
        var timestamp = parseInt(fechaDotNet.replace(/\/Date\((\d+)\)\//, '$1'));
        var fecha = new Date(timestamp);
        
        if (isNaN(fecha.getTime())) {
            return 'N/A';
        }
        
        var dia = ('0' + fecha.getDate()).slice(-2);
        var mes = ('0' + (fecha.getMonth() + 1)).slice(-2);
        var anio = fecha.getFullYear();
        
        return dia + '/' + mes + '/' + anio;
    } catch (e) {
        console.error('Error al convertir fecha:', fechaDotNet, e);
        return 'N/A';
    }
}

/**
 * Parsea fecha .NET a objeto Date de JavaScript
 * @param {string} fechaDotNet - Fecha en formato /Date(1234567890)/
 * @returns {Date} Objeto Date o null si es inválida
 */
function parsearFechaNet(fechaDotNet) {
    if (!fechaDotNet) return null;
    
    if (!fechaDotNet.toString().includes('/Date(')) {
        return new Date(fechaDotNet);
    }
    
    try {
        var timestamp = parseInt(fechaDotNet.replace(/\/Date\((\d+)\)\//, '$1'));
        var fecha = new Date(timestamp);
        return isNaN(fecha.getTime()) ? null : fecha;
    } catch (e) {
        console.error('Error al parsear fecha:', fechaDotNet, e);
        return null;
    }
}
