// Variables globales
let chartVentas;
let periodoActual = 'semana';

$(document).ready(function () {
    // Configurar eventos para botones de periodo
    configurarBotonesPeriodo();
    
    // El gráfico se inicializa en Index.cshtml, solo guardamos referencia
    // cuando se carga el dashboard
});

function configurarBotonesPeriodo() {
    $('.btn-group button').click(function() {
        const periodo = $(this).data('periodo');
        
        // Actualizar botones activos
        $('.btn-group button').removeClass('active');
        $(this).addClass('active');
        
        // Actualizar título del gráfico
        let titulo = '';
        switch(periodo) {
            case 'semana':
                titulo = 'Ventas de la Semana';
                break;
            case 'mes':
                titulo = 'Ventas del Último Mes';
                break;
            case 'año':
                titulo = 'Ventas del Último Año';
                break;
        }
        $('.dashboard-card h4').first().html('<i class="fas fa-chart-line"></i> ' + titulo);
        
        // Cargar datos del periodo
        cargarVentasPorPeriodo(periodo);
    });
}

function cargarVentasPorPeriodo(periodo) {
    periodoActual = periodo;
    
    $.get('/Home/ObtenerVentasPorPeriodo', { periodo: periodo }, function(response) {
        if (response.success) {
            // Actualizar el gráfico con los nuevos datos
            if (window.chartVentas) {
                window.chartVentas.data.labels = response.etiquetas;
                window.chartVentas.data.datasets[0].data = response.ventas;
                
                // Actualizar título del gráfico según periodo
                let titulo = '';
                switch(periodo.toLowerCase()) {
                    case 'semana':
                        titulo = 'Ventas de la Semana';
                        break;
                    case 'mes':
                        titulo = 'Ventas del Último Mes';
                        break;
                    case 'año':
                        titulo = 'Ventas del Último Año';
                        break;
                }
                
                window.chartVentas.options.plugins.title = {
                    display: false // El título está fuera del canvas
                };
                
                window.chartVentas.update();
                
                // Actualizar información adicional
                const promedio = response.totalPeriodo / response.ventas.length;
                const ventaMayor = Math.max(...response.ventas);
                
                toastr.success(`Datos actualizados: ${periodo.charAt(0).toUpperCase() + periodo.slice(1)}`);
            }
        } else {
            toastr.error('Error al cargar datos: ' + response.mensaje);
        }
    }).fail(function() {
        toastr.error('Error al conectar con el servidor');
    });
}

// Función para formatear moneda
function formatCurrency(value) {
    return '$' + parseFloat(value).toLocaleString('es-MX', {
        minimumFractionDigits: 2,
        maximumFractionDigits: 2
    });
}