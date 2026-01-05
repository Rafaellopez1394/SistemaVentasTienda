var alertasData = [];
var tablaDT = null;

$(document).ready(function () {
    cargarAlertas();
    cargarSucursales();

    $('#btnActualizar').on('click', function () {
        cargarAlertas();
    });

    $('#btnExportar').on('click', function () {
        exportarReporte();
    });

    $('#filtroSucursal, #filtroNivel, #filtroCategoria').on('change', function () {
        aplicarFiltros();
    });

    $('#btnGuardarStockMinimo').on('click', function () {
        actualizarStockMinimo();
    });
});

function cargarAlertas() {
    $('#btnActualizar').prop('disabled', true).html('<i class="fa fa-spinner fa-spin"></i> Actualizando...');
    
    var sucursalId = $('#filtroSucursal').val();
    
    $.ajax({
        url: '/AlertasInventario/ObtenerAlertas',
        type: 'GET',
        data: { sucursalId: sucursalId },
        success: function (response) {
            $('#btnActualizar').prop('disabled', false).html('<i class="fa fa-refresh"></i> Actualizar');
            
            if (response.success && response.data) {
                alertasData = response.data;
                cargarConteo();
                renderizarTabla(alertasData);
                cargarCategoriasUnicas();
            } else {
                Swal.fire('Sin Datos', 'No hay alertas de inventario en este momento', 'info');
                $('#tbodyAlertas').html('<tr><td colspan="12" class="text-center text-muted">No hay alertas</td></tr>');
            }
        },
        error: function (xhr, status, error) {
            $('#btnActualizar').prop('disabled', false).html('<i class="fa fa-refresh"></i> Actualizar');
            Swal.fire('Error', 'Error al cargar alertas: ' + error, 'error');
        }
    });
}

function cargarConteo() {
    var sucursalId = $('#filtroSucursal').val();
    
    $.ajax({
        url: '/AlertasInventario/ObtenerConteo',
        type: 'GET',
        data: { sucursalId: sucursalId },
        success: function (response) {
            if (response.success && response.data) {
                $('#conteoAgotado').text(response.data.AGOTADO);
                $('#conteoCritico').text(response.data.CRITICO);
                $('#conteoBajo').text(response.data.BAJO);
                $('#conteoTotal').text(response.data.TOTAL);
            }
        }
    });
}

function renderizarTabla(data) {
    if (tablaDT) {
        tablaDT.destroy();
    }

    var tbody = $('#tbodyAlertas');
    tbody.empty();

    if (!data || data.length === 0) {
        tbody.html('<tr><td colspan="12" class="text-center text-muted">No hay alertas de inventario</td></tr>');
        return;
    }

    data.forEach(function (alerta) {
        var badgeNivel = '';
        switch (alerta.NivelAlerta) {
            case 'AGOTADO':
                badgeNivel = '<span class="label label-danger"><i class="fa fa-times-circle"></i> AGOTADO</span>';
                break;
            case 'CRITICO':
                badgeNivel = '<span class="label label-warning"><i class="fa fa-warning"></i> CRÍTICO</span>';
                break;
            case 'BAJO':
                badgeNivel = '<span class="label label-default"><i class="fa fa-exclamation"></i> BAJO</span>';
                break;
        }

        var ultimaCompra = alerta.UltimaCompra ? 
            new Date(alerta.UltimaCompra).toLocaleDateString('es-MX') : 
            '<span class="text-muted">Sin compras</span>';

        var diasColor = '';
        if (alerta.DiasDesdeUltimaCompra > 90) diasColor = 'text-danger';
        else if (alerta.DiasDesdeUltimaCompra > 60) diasColor = 'text-warning';

        var porcentajeBar = '';
        if (alerta.PorcentajeStock === 0) {
            porcentajeBar = '<div class="progress"><div class="progress-bar progress-bar-danger" style="width: 100%">0%</div></div>';
        } else {
            var colorBar = alerta.PorcentajeStock <= 25 ? 'danger' : (alerta.PorcentajeStock <= 50 ? 'warning' : 'success');
            porcentajeBar = '<div class="progress"><div class="progress-bar progress-bar-' + colorBar + '" style="width: ' + alerta.PorcentajeStock + '%">' + alerta.PorcentajeStock.toFixed(1) + '%</div></div>';
        }

        var row = '<tr>' +
            '<td>' + badgeNivel + '</td>' +
            '<td><code>' + (alerta.CodigoInterno || 'N/A') + '</code></td>' +
            '<td><strong>' + alerta.NombreProducto + '</strong></td>' +
            '<td>' + alerta.Categoria + '</td>' +
            '<td><span class="label label-primary">' + alerta.NombreSucursal + '</span></td>' +
            '<td class="text-center"><span class="badge bg-blue">' + alerta.StockActual + '</span></td>' +
            '<td class="text-center"><span class="badge bg-gray">' + alerta.StockMinimo + '</span></td>' +
            '<td class="text-center"><span class="badge bg-red">' + alerta.Diferencia + '</span></td>' +
            '<td>' + porcentajeBar + '</td>' +
            '<td>' + ultimaCompra + '</td>' +
            '<td class="text-center ' + diasColor + '"><strong>' + alerta.DiasDesdeUltimaCompra + '</strong></td>' +
            '<td class="text-center">' +
            '<button class="btn btn-xs btn-warning" onclick="abrirModalStockMinimo(' + alerta.ProductoID + ', \'' + alerta.NombreProducto.replace(/'/g, "\\'") + '\', ' + alerta.StockActual + ', ' + alerta.StockMinimo + ')" title="Actualizar stock mínimo">' +
            '<i class="fa fa-edit"></i>' +
            '</button> ' +
            '<a href="/Producto/Editar?id=' + alerta.ProductoID + '" class="btn btn-xs btn-info" title="Ver producto">' +
            '<i class="fa fa-eye"></i>' +
            '</a>' +
            '</td>' +
            '</tr>';

        tbody.append(row);
    });

    // Inicializar DataTable
    tablaDT = $('#tablaAlertas').DataTable({
        language: {
            url: '//cdn.datatables.net/plug-ins/1.13.6/i18n/es-MX.json'
        },
        order: [[0, 'asc'], [7, 'desc']], // Ordenar por nivel y diferencia
        pageLength: 25,
        responsive: true
    });
}

function aplicarFiltros() {
    var nivel = $('#filtroNivel').val();
    var categoria = $('#filtroCategoria').val();

    var dataFiltrada = alertasData;

    if (nivel) {
        dataFiltrada = dataFiltrada.filter(function (a) {
            return a.NivelAlerta === nivel;
        });
    }

    if (categoria) {
        dataFiltrada = dataFiltrada.filter(function (a) {
            return a.Categoria === categoria;
        });
    }

    renderizarTabla(dataFiltrada);
}

function cargarSucursales() {
    $.ajax({
        url: '/Sucursal/ObtenerTodos',
        type: 'GET',
        success: function (response) {
            if (response.data && response.data.length > 0) {
                var select = $('#filtroSucursal');
                response.data.forEach(function (sucursal) {
                    select.append('<option value="' + sucursal.SucursalID + '">' + sucursal.Nombre + '</option>');
                });
            }
        }
    });
}

function cargarCategoriasUnicas() {
    var categorias = [...new Set(alertasData.map(a => a.Categoria))].sort();
    var select = $('#filtroCategoria');
    select.find('option:not(:first)').remove();
    
    categorias.forEach(function (cat) {
        select.append('<option value="' + cat + '">' + cat + '</option>');
    });
}

function abrirModalStockMinimo(productoId, nombreProducto, stockActual, stockMinimoActual) {
    $('#modalProductoID').val(productoId);
    $('#modalNombreProducto').text(nombreProducto);
    $('#modalStockActual').text(stockActual + ' unidades');
    $('#modalNuevoStockMinimo').val(stockMinimoActual);
    $('#modalStockMinimo').modal('show');
}

function actualizarStockMinimo() {
    var productoId = $('#modalProductoID').val();
    var nuevoStockMinimo = $('#modalNuevoStockMinimo').val();

    if (!nuevoStockMinimo || nuevoStockMinimo < 0) {
        Swal.fire('Error', 'Ingrese un valor válido para el stock mínimo', 'error');
        return;
    }

    $('#btnGuardarStockMinimo').prop('disabled', true).html('<i class="fa fa-spinner fa-spin"></i> Guardando...');

    $.ajax({
        url: '/AlertasInventario/ActualizarStockMinimo',
        type: 'POST',
        data: {
            productoId: productoId,
            stockMinimo: nuevoStockMinimo
        },
        success: function (response) {
            $('#btnGuardarStockMinimo').prop('disabled', false).html('<i class="fa fa-save"></i> Guardar');
            
            if (response.success) {
                Swal.fire('Éxito', response.mensaje, 'success');
                $('#modalStockMinimo').modal('hide');
                cargarAlertas();
            } else {
                Swal.fire('Error', response.mensaje, 'error');
            }
        },
        error: function (xhr, status, error) {
            $('#btnGuardarStockMinimo').prop('disabled', false).html('<i class="fa fa-save"></i> Guardar');
            Swal.fire('Error', 'Error al actualizar: ' + error, 'error');
        }
    });
}

function exportarReporte() {
    var sucursalId = $('#filtroSucursal').val();
    window.location.href = '/AlertasInventario/ExportarReporte?sucursalId=' + sucursalId;
}
