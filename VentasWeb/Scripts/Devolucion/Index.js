// Scripts/Devolucion/Index.js
var dataTable;

$(document).ready(function () {
    // Establecer fechas por defecto (últimos 30 días)
    var hoy = new Date();
    var hace30Dias = new Date();
    hace30Dias.setDate(hoy.getDate() - 30);
    
    $('#filtroFechaInicio').val(formatDate(hace30Dias));
    $('#filtroFechaFin').val(formatDate(hoy));
    
    // Cargar devoluciones
    cargarDevoluciones();
    
    // Inicializar DataTable
    dataTable = $('#tablaDevoluciones').DataTable({
        language: {
            url: '//cdn.datatables.net/plug-ins/1.13.6/i18n/es-MX.json'
        },
        order: [[0, 'desc']],
        pageLength: 25
    });
});

function formatDate(date) {
    var d = new Date(date),
        month = '' + (d.getMonth() + 1),
        day = '' + d.getDate(),
        year = d.getFullYear();

    if (month.length < 2) month = '0' + month;
    if (day.length < 2) day = '0' + day;

    return [year, month, day].join('-');
}

function cargarDevoluciones() {
    var fechaInicio = $('#filtroFechaInicio').val();
    var fechaFin = $('#filtroFechaFin').val();
    
    if (!fechaInicio || !fechaFin) {
        Swal.fire('Advertencia', 'Debe seleccionar ambas fechas', 'warning');
        return;
    }
    
    $('#tbodyDevoluciones').html('<tr><td colspan="12" class="text-center"><i class="fas fa-spinner fa-spin"></i> Cargando...</td></tr>');
    
    $.ajax({
        url: '/Devolucion/ObtenerDevoluciones',
        type: 'GET',
        data: { fechaInicio: fechaInicio, fechaFin: fechaFin },
        success: function (response) {
            if (response.success) {
                renderizarTabla(response.data);
            } else {
                Swal.fire('Error', response.mensaje, 'error');
            }
        },
        error: function () {
            Swal.fire('Error', 'No se pudieron cargar las devoluciones', 'error');
        }
    });
}

function renderizarTabla(data) {
    if (dataTable) {
        dataTable.destroy();
    }
    
    var html = '';
    
    if (data.length === 0) {
        html = '<tr><td colspan="12" class="text-center text-muted">No hay devoluciones registradas</td></tr>';
    } else {
        data.forEach(function (item) {
            var badgeTipo = item.TipoDevolucion === 'TOTAL' 
                ? '<span class="badge badge-danger">TOTAL</span>' 
                : '<span class="badge badge-warning">PARCIAL</span>';
            
            var badgeReintegro = '';
            if (item.FormaReintegro === 'EFECTIVO') {
                badgeReintegro = '<span class="badge badge-success">Efectivo</span>';
            } else if (item.FormaReintegro === 'CREDITO_CLIENTE') {
                badgeReintegro = '<span class="badge badge-info">Crédito Cliente</span>';
            } else {
                badgeReintegro = '<span class="badge badge-secondary">Cambio</span>';
            }
            
            var fecha = new Date(item.FechaDevolucion).toLocaleString('es-MX');
            
            html += '<tr>';
            html += '<td>' + item.DevolucionID + '</td>';
            html += '<td>' + fecha + '</td>';
            html += '<td><strong>' + item.NumeroVenta + '</strong></td>';
            html += '<td>' + item.NombreCliente + '</td>';
            html += '<td>' + badgeTipo + '</td>';
            html += '<td>' + truncate(item.MotivoDevolucion, 50) + '</td>';
            html += '<td class="text-right"><strong>$' + formatMoney(item.TotalDevuelto) + '</strong></td>';
            html += '<td>' + badgeReintegro + '</td>';
            html += '<td class="text-center"><span class="badge badge-primary">' + item.TotalProductos + '</span></td>';
            html += '<td>' + item.NombreSucursal + '</td>';
            html += '<td>' + item.UsuarioRegistro + '</td>';
            html += '<td class="text-center">';
            html += '<button class="btn btn-sm btn-info" onclick="verDetalle(' + item.DevolucionID + ')" title="Ver Detalle">';
            html += '<i class="fas fa-eye"></i></button>';
            html += '</td>';
            html += '</tr>';
        });
    }
    
    $('#tbodyDevoluciones').html(html);
    
    dataTable = $('#tablaDevoluciones').DataTable({
        language: {
            url: '//cdn.datatables.net/plug-ins/1.13.6/i18n/es-MX.json'
        },
        order: [[0, 'desc']],
        pageLength: 25
    });
}

function verDetalle(devolucionID) {
    $.ajax({
        url: '/Devolucion/ObtenerDetalle',
        type: 'GET',
        data: { devolucionId: devolucionID },
        success: function (response) {
            if (response.success) {
                mostrarModalDetalle(response.data);
            } else {
                Swal.fire('Error', response.mensaje, 'error');
            }
        },
        error: function () {
            Swal.fire('Error', 'No se pudo obtener el detalle', 'error');
        }
    });
}

function mostrarModalDetalle(data) {
    var fecha = new Date(data.FechaDevolucion).toLocaleString('es-MX');
    
    var html = '<div class="row">';
    html += '<div class="col-md-6"><p><strong>ID:</strong> ' + data.DevolucionID + '</p></div>';
    html += '<div class="col-md-6"><p><strong>Fecha:</strong> ' + fecha + '</p></div>';
    html += '<div class="col-md-6"><p><strong>Venta:</strong> ' + data.NumeroVenta + '</p></div>';
    html += '<div class="col-md-6"><p><strong>Cliente:</strong> ' + data.NombreCliente + '</p></div>';
    html += '<div class="col-md-6"><p><strong>Tipo:</strong> ' + data.TipoDevolucion + '</p></div>';
    html += '<div class="col-md-6"><p><strong>Forma Reintegro:</strong> ' + data.FormaReintegro + '</p></div>';
    html += '<div class="col-md-12"><p><strong>Motivo:</strong> ' + data.MotivoDevolucion + '</p></div>';
    html += '<div class="col-md-6"><p><strong>Total Devuelto:</strong> <span class="text-success">$' + formatMoney(data.TotalDevuelto) + '</span></p></div>';
    html += '<div class="col-md-6"><p><strong>Usuario:</strong> ' + data.UsuarioRegistro + '</p></div>';
    html += '</div>';
    
    html += '<hr />';
    html += '<h6><i class="fas fa-box"></i> Productos Devueltos</h6>';
    html += '<div class="table-responsive">';
    html += '<table class="table table-sm table-bordered">';
    html += '<thead class="thead-light">';
    html += '<tr><th>Código</th><th>Producto</th><th>Cantidad</th><th>Precio</th><th>SubTotal</th></tr>';
    html += '</thead>';
    html += '<tbody>';
    
    data.Productos.forEach(function (prod) {
        html += '<tr>';
        html += '<td>' + prod.CodigoInterno + '</td>';
        html += '<td>' + prod.NombreProducto + '</td>';
        html += '<td class="text-center">' + prod.CantidadDevuelta + '</td>';
        html += '<td class="text-right">$' + formatMoney(prod.PrecioVenta) + '</td>';
        html += '<td class="text-right"><strong>$' + formatMoney(prod.SubTotal) + '</strong></td>';
        html += '</tr>';
    });
    
    html += '</tbody>';
    html += '</table>';
    html += '</div>';
    
    $('#detalleDevolucion').html(html);
    $('#modalDetalle').modal('show');
}

function formatMoney(amount) {
    return parseFloat(amount).toFixed(2).replace(/\d(?=(\d{3})+\.)/g, '$&,');
}

function truncate(str, n) {
    return (str.length > n) ? str.substr(0, n - 1) + '...' : str;
}
