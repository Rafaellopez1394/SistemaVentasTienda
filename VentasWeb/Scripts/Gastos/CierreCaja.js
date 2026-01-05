$(document).ready(function () {
    // Inicializar fecha con hoy
    $('#fechaCierre').val(formatDate(new Date()));

    // Cargar cajas (si existe endpoint)
    cargarCajas();

    // Consultar cierre
    $('#btnConsultarCierre').on('click', function () {
        consultarCierre();
    });

    // Imprimir cierre
    $('#btnImprimirCierre').on('click', function () {
        imprimirCierre();
    });
});

function cargarCajas() {
    // TODO: Implementar endpoint para obtener cajas
    // Por ahora, si hay caja activa en sesión, usarla
    var cajaActiva = 1; // Obtener de sesión o API
    $('#cajaID').append('<option value="' + cajaActiva + '" selected>Caja ' + cajaActiva + '</option>');
}

function consultarCierre() {
    var cajaID = $('#cajaID').val();
    var fecha = $('#fechaCierre').val();

    if (!cajaID) {
        Swal.fire('Error', 'Debe seleccionar una caja', 'error');
        return;
    }

    if (!fecha) {
        Swal.fire('Error', 'Debe seleccionar una fecha', 'error');
        return;
    }

    $('#btnConsultarCierre').prop('disabled', true).html('<i class="fa fa-spinner fa-spin"></i> Consultando...');

    $.ajax({
        url: '/Gastos/ObtenerCierreCaja',
        type: 'GET',
        data: { cajaID: cajaID, fecha: fecha },
        success: function (response) {
            $('#btnConsultarCierre').prop('disabled', false).html('<i class="fa fa-search"></i> Consultar Cierre');

            if (response.success && response.data) {
                mostrarCierre(response.data);
                // Cargar concentrado de gastos
                cargarConcentradoGastos(cajaID, fecha);
                $('#resultadoCierre').fadeIn();
            } else {
                Swal.fire('Sin Datos', 'No hay información de cierre para la fecha seleccionada', 'info');
                $('#resultadoCierre').fadeOut();
            }
        },
        error: function (xhr, status, error) {
            $('#btnConsultarCierre').prop('disabled', false).html('<i class="fa fa-search"></i> Consultar Cierre');
            Swal.fire('Error', 'Error al consultar cierre: ' + error, 'error');
        }
    });
}

function mostrarCierre(data) {
    // Resumen general (info boxes)
    $('#totalVentas').text('$' + formatMoney(data.TotalVentas));
    $('#totalGastos').text('$' + formatMoney(data.TotalGastos));
    $('#efectivoCaja').text('$' + formatMoney(data.EfectivoEnCaja));
    $('#gananciaNeta').text('$' + formatMoney(data.GananciaNeta));

    // Aplicar color según ganancia
    if (data.GananciaNeta < 0) {
        $('#gananciaNeta').parent().parent().removeClass('bg-yellow').addClass('bg-red');
    } else {
        $('#gananciaNeta').parent().parent().removeClass('bg-red').addClass('bg-yellow');
    }

    // Desglose de ventas
    $('#ventasEfectivo').text('$' + formatMoney(data.VentasEfectivo));
    $('#ventasTarjeta').text('$' + formatMoney(data.VentasTarjeta));
    $('#ventasTransferencia').text('$' + formatMoney(data.VentasTransferencia));
    $('#totalVentasTable').text('$' + formatMoney(data.TotalVentas));

    // Detalle de gastos
    var tbody = $('#detalleGastos');
    tbody.empty();

    if (data.DetalleGastos && data.DetalleGastos.length > 0) {
        data.DetalleGastos.forEach(function (gasto) {
            var row = '<tr>' +
                '<td><span class="label label-info">' + gasto.Categoria + '</span></td>' +
                '<td>' + gasto.Concepto + '</td>' +
                '<td class="text-right"><strong>$' + formatMoney(gasto.Monto) + '</strong></td>' +
                '</tr>';
            tbody.append(row);
        });
    } else {
        tbody.append('<tr><td colspan="3" class="text-center text-muted">No hay gastos registrados</td></tr>');
    }

    // Resumen final
    $('#resumenVentas').text('$' + formatMoney(data.TotalVentas));
    $('#resumenGastos').text('$' + formatMoney(data.TotalGastos));
    $('#resumenRetiros').text('$' + formatMoney(data.TotalRetiros));
    $('#resumenGananciaNeta').text('$' + formatMoney(data.GananciaNeta));

    $('#resumenEfectivoVentas').text('$' + formatMoney(data.VentasEfectivo));
    $('#resumenEfectivoGastos').text('$' + formatMoney(data.GastosEfectivo));
    $('#resumenEfectivoRetiros').text('$' + formatMoney(data.TotalRetiros));
    $('#resumenEfectivoCaja').text('$' + formatMoney(data.EfectivoEnCaja));
}

function cargarConcentradoGastos(cajaID, fecha) {
    $.ajax({
        url: '/Gastos/ObtenerConcentradoGastosCierre',
        type: 'GET',
        data: { 
            cajaID: cajaID, 
            fecha: fecha 
        },
        success: function (response) {
            if (response.success && response.data && response.data.length > 0) {
                mostrarConcentradoGastos(response.data);
            } else {
                $('#concentradoGastos').html('<tr><td colspan="4" class="text-center text-muted">No hay gastos registrados</td></tr>');
                $('#totalConcentrado').hide();
            }
        },
        error: function (xhr, status, error) {
            console.error('Error al cargar concentrado de gastos:', error);
            $('#concentradoGastos').html('<tr><td colspan="4" class="text-center text-danger">Error al cargar concentrado</td></tr>');
        }
    });
}

function mostrarConcentradoGastos(data) {
    var tbody = $('#concentradoGastos');
    tbody.empty();
    
    var totalNumero = 0;
    var totalMonto = 0;
    
    // Calcular totales
    data.forEach(function(item) {
        totalNumero += item.NumeroGastos;
        totalMonto += item.TotalMonto;
    });
    
    // Generar filas
    data.forEach(function(item) {
        var porcentaje = totalMonto > 0 ? ((item.TotalMonto / totalMonto) * 100).toFixed(1) : 0;
        
        var row = '<tr>' +
            '<td><strong>' + item.Categoria + '</strong></td>' +
            '<td class="text-center"><span class="badge bg-blue">' + item.NumeroGastos + '</span></td>' +
            '<td class="text-right"><strong>$' + formatMoney(item.TotalMonto) + '</strong></td>' +
            '<td class="text-right">' + porcentaje + '%</td>' +
            '</tr>';
        tbody.append(row);
    });
    
    // Actualizar totales
    $('#totalNumeroGastos').text(totalNumero);
    $('#totalMontoGastos').text('$' + formatMoney(totalMonto));
    $('#totalConcentrado').show();
}

function imprimirCierre() {
    window.print();
}

function formatMoney(amount) {
    return parseFloat(amount).toFixed(2).replace(/\d(?=(\d{3})+\.)/g, '$&,');
}

function formatDate(date) {
    var d = new Date(date);
    var month = '' + (d.getMonth() + 1);
    var day = '' + d.getDate();
    var year = d.getFullYear();

    if (month.length < 2) month = '0' + month;
    if (day.length < 2) day = '0' + day;

    return [year, month, day].join('-');
}

// Estilos de impresión
var style = document.createElement('style');
style.innerHTML = '@media print { ' +
    '.content-header, .box-tools, .breadcrumb, .btn, .main-sidebar, .main-header { display: none !important; } ' +
    '.content-wrapper { margin: 0 !important; padding: 0 !important; } ' +
    '.box { box-shadow: none !important; border: 1px solid #000 !important; } ' +
    '}';
document.head.appendChild(style);
