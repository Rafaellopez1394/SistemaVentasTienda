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
    
    var sucursalId = $('#filtroSucursal').val() || '0';
    
    console.log('Cargando alertas con sucursalId:', sucursalId);
    
    $.ajax({
        url: '/AlertasInventario/ObtenerAlertas',
        type: 'GET',
        data: { sucursalId: sucursalId },
        success: function (response) {
            console.log('Respuesta recibida:', response);
            console.log('response.success:', response.success);
            console.log('response.data type:', typeof response.data);
            console.log('response.data:', response.data);
            console.log('Cantidad de alertas:', response.data ? response.data.length : 0);
            
            $('#btnActualizar').prop('disabled', false).html('<i class="fa fa-refresh"></i> Actualizar');
            
            // Verificar si hay error en la respuesta
            if (response.success === false) {
                console.error('Error del servidor:', response.mensaje);
                Swal.fire('Error', response.mensaje || 'Error desconocido', 'error');
                $('#tbodyAlertas').html('<tr><td colspan="12" class="text-center text-danger">Error: ' + (response.mensaje || 'Error desconocido') + '</td></tr>');
                return;
            }
            
            // Verificar si response.data es un array
            if (!Array.isArray(response.data)) {
                console.error('response.data no es un array:', response.data);
                Swal.fire('Error', 'Formato de respuesta inválido', 'error');
                return;
            }
            
            // Si hay datos, renderizar
            if (response.data.length > 0) {
                console.log('Renderizando', response.data.length, 'alertas');
                alertasData = response.data;
                cargarConteo();
                renderizarTabla(alertasData);
                cargarCategoriasUnicas();
            } else {
                console.warn('Array de datos está vacío');
                Swal.fire('Sin Datos', 'No hay alertas de inventario en este momento', 'info');
                $('#tbodyAlertas').html('<tr><td colspan="12" class="text-center text-muted">No hay alertas</td></tr>');
                // Aún así llamar a cargar conteo para mostrar 0s
                cargarConteo();
            }
        },
        error: function (xhr, status, error) {
            console.error('Error en AJAX:', xhr, status, error);
            console.error('Respuesta completa:', xhr.responseText);
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
            "sProcessing": "Procesando...",
            "sLengthMenu": "Mostrar _MENU_ registros",
            "sZeroRecords": "No se encontraron resultados",
            "sEmptyTable": "Ningún dato disponible en esta tabla",
            "sInfo": "Mostrando registros del _START_ al _END_ de un total de _TOTAL_ registros",
            "sInfoEmpty": "Mostrando registros del 0 al 0 de un total de 0 registros",
            "sInfoFiltered": "(filtrado de un total de _MAX_ registros)",
            "sInfoPostFix": "",
            "sSearch": "Buscar:",
            "sUrl": "",
            "sInfoThousands": ",",
            "sLoadingRecords": "Cargando...",
            "oPaginate": {
                "sFirst": "Primero",
                "sLast": "Último",
                "sNext": "Siguiente",
                "sPrevious": "Anterior"
            },
            "oAria": {
                "sSortAscending": ": Activar para ordenar la columna de manera ascendente",
                "sSortDescending": ": Activar para ordenar la columna de manera descendente"
            }
        },
        order: [[0, 'asc'], [7, 'desc']], // Ordenar por nivel y diferencia
        pageLength: 25,
        responsive: true,
        deferRender: true,
        processing: false,
        autoWidth: false
    });
    
    console.log('DataTable inicializada correctamente');
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
    console.log('Cargando sucursales...');
    
    var select = $('#filtroSucursal');
    
    $.ajax({
        url: '/Sucursal/ObtenerTodos',
        type: 'GET',
        success: function (response) {
            console.log('Respuesta de sucursales:', response);
            console.log('Cantidad de sucursales:', response.data ? response.data.length : 0);
            
            if (response.data && response.data.length > 0) {
                console.log('Select encontrado:', select.length > 0);
                
                // Limpiar completamente el select y reconstruir
                select.empty();
                select.append('<option value="0">Todas las sucursales</option>');
                
                response.data.forEach(function (sucursal) {
                    console.log('Agregando sucursal:', sucursal.Nombre, '(ID:', sucursal.SucursalID + ')');
                    select.append('<option value="' + sucursal.SucursalID + '">' + sucursal.Nombre + '</option>');
                });
                
                console.log('Opciones finales:', select.find('option').length);
                console.log('Sucursales cargadas correctamente');
            } else {
                console.warn('No hay sucursales en la respuesta');
                select.html('<option value="0">Todas las sucursales</option>');
            }
        },
        error: function(xhr, status, error) {
            console.error('Error al cargar sucursales:', error);
            console.error('Status:', status);
            console.error('Response:', xhr.responseText);
            select.html('<option value="0">Todas las sucursales</option>');
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
