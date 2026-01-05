// Scripts/Compra/CargarXML.js

var datosXML = null;
var rutaXMLTemporal = null;
var conceptosXML = [];
var tablaSucursales = null;

$(document).ready(function () {
    // Inicializar DataTable de sucursales
    try {
        tablaSucursales = $('#tablaSucursales').DataTable({
            ajax: {
                url: $.MisUrls.url._ObtenerSucursales,
                dataSrc: 'data',
                error: function(xhr, error, thrown) {
                    console.error('Error al cargar sucursales:', error, thrown);
                    toastr.error('Error al cargar las sucursales');
                }
            },
            columns: [
                {
                    data: null,
                    orderable: false,
                    render: function (data, type, row) {
                        return '<button class="btn btn-sm btn-success" onclick="seleccionarSucursal(' + 
                               row.SucursalID + ',\'' + (row.RFC || '').replace(/'/g, "\\'") + '\',\'' + 
                               (row.Nombre || '').replace(/'/g, "\\'") + '\')" title="Seleccionar">' +
                               '<i class="fa fa-check"></i></button>';
                    }
                },
                { data: 'RFC' },
                { data: 'Nombre' },
                { data: 'Direccion' }
            ],
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
            paging: true,
            searching: true,
            info: true
        });
        console.log('DataTable de sucursales inicializado correctamente');
    } catch(e) {
        console.error('Error al inicializar DataTable de sucursales:', e);
    }

    // Procesar XML
    $('#btnProcesarXML').on('click', function () {
        procesarXML();
    });

    // Registrar compra
    $('#btnRegistrarCompra').on('click', function () {
        registrarCompraDesdeXML();
    });

    // Cancelar
    $('#btnCancelar').on('click', function () {
        if (confirm('¿Está seguro de cancelar? Se perderán los datos cargados.')) {
            location.reload();
        }
    });

    // Cambio en factor de conversión
    $(document).on('change', '.factor-conversion', function () {
        calcularCantidadFinal($(this));
    });
});

function procesarXML() {
    var archivoInput = document.getElementById('archivoXML');
    
    if (!archivoInput.files || archivoInput.files.length === 0) {
        Swal.fire('Error', 'Debe seleccionar un archivo XML', 'error');
        return;
    }

    var archivo = archivoInput.files[0];
    
    // Validar extensión
    if (!archivo.name.toLowerCase().endsWith('.xml')) {
        Swal.fire('Error', 'El archivo debe tener extensión .xml', 'error');
        return;
    }

    $('#btnProcesarXML').prop('disabled', true).html('<i class="fa fa-spinner fa-spin"></i> Procesando...');

    var formData = new FormData();
    formData.append('file', archivo);

    $.ajax({
        url: '/Compra/ProcesarXML',
        type: 'POST',
        data: formData,
        processData: false,
        contentType: false,
        success: function (response) {
            $('#btnProcesarXML').prop('disabled', false).html('<i class="fa fa-cogs"></i> Procesar XML');

            if (response.success) {
                datosXML = response.datos;
                rutaXMLTemporal = response.rutaTemporal;
                conceptosXML = response.datos.conceptos;
                
                mostrarDatosFactura();
                mostrarConceptos();
                
                $('#paso1').slideUp();
                $('#paso2').slideDown();
                $('#paso3').slideDown();
                
                Swal.fire('Éxito', 'XML procesado correctamente', 'success');
            } else {
                Swal.fire('Error', response.mensaje, 'error');
            }
        },
        error: function (xhr, status, error) {
            $('#btnProcesarXML').prop('disabled', false).html('<i class="fa fa-cogs"></i> Procesar XML');
            Swal.fire('Error', 'Error al procesar XML: ' + error, 'error');
        }
    });
}

function mostrarDatosFactura() {
    // Proveedor
    $('#proveedorRFC').text(datosXML.proveedorRFC);
    $('#proveedorNombre').text(datosXML.proveedorNombre);
    
    // Comprobante
    $('#serieFolio').text((datosXML.serie || '') + '-' + (datosXML.folio || ''));
    $('#fecha').text(formatDateTime(datosXML.fecha));
    $('#uuid').text(datosXML.uuid || 'Sin UUID');
    
    // Totales
    $('#subTotal').text('$' + formatMoney(datosXML.subTotal));
    $('#descuento').text('$' + formatMoney(datosXML.descuento));
    $('#total').text('$' + formatMoney(datosXML.total));
}

function mostrarConceptos() {
    var tbody = $('#tbodyConceptos');
    tbody.empty();
    
    $('#contadorProductos').text(conceptosXML.length + ' producto(s)');
    
    conceptosXML.forEach(function (concepto, index) {
        var row = '<tr>' +
            '<td>' + (index + 1) + '</td>' +
            '<td>' + (concepto.noIdentificacion || concepto.claveProdServ) + '</td>' +
            '<td><small>' + concepto.descripcion + '</small></td>' +
            '<td class="text-center">' + concepto.cantidad + ' ' + (concepto.unidad || '') + '</td>' +
            '<td class="text-right">$' + formatMoney(concepto.valorUnitario) + '</td>' +
            '<td>' +
            '   <input type="number" class="form-control factor-conversion" data-index="' + index + '" ' +
            '          value="1" min="1" step="0.001" style="width:80px;" />' +
            '</td>' +
            '<td class="text-center cantidad-final" data-index="' + index + '">' +
            '   <strong>' + concepto.cantidad + '</strong>' +
            '</td>' +
            '<td>' +
            '   <select class="form-control select-producto" data-index="' + index + '" style="width:100%;">' +
            '       <option value="">Buscar producto...</option>' +
            '   </select>' +
            '</td>' +
            '</tr>';
        
        tbody.append(row);
        
        // Inicializar Select2 para búsqueda de productos
        var selectProducto = tbody.find('.select-producto[data-index="' + index + '"]');
        inicializarSelect2Producto(selectProducto, concepto.descripcion);
    });
}

function inicializarSelect2Producto(selectElement, descripcionXML) {
    selectElement.select2({
        ajax: {
            url: '/Compra/BuscarProductoParaMapeo',
            dataType: 'json',
            delay: 250,
            data: function (params) {
                return {
                    termino: params.term || descripcionXML.substring(0, 30)
                };
            },
            processResults: function (data) {
                return {
                    results: data.map(function (item) {
                        return {
                            id: item.productoID,
                            text: item.nombreProducto + ' (' + item.codigoProducto + ')',
                            precioVenta: item.precioVenta,
                            codigoProducto: item.codigoProducto
                        };
                    })
                };
            },
            cache: true
        },
        placeholder: 'Buscar producto del sistema...',
        minimumInputLength: 0,
        language: {
            noResults: function () {
                return "No se encontraron productos";
            },
            searching: function () {
                return "Buscando...";
            }
        }
    });

    // Búsqueda automática inicial
    selectElement.select2('open');
    setTimeout(function () {
        selectElement.select2('close');
    }, 100);
}

function calcularCantidadFinal(input) {
    var index = input.data('index');
    var factor = parseFloat(input.val()) || 1;
    var cantidadXML = conceptosXML[index].cantidad;
    var cantidadFinal = cantidadXML * factor;
    
    $('.cantidad-final[data-index="' + index + '"]').html('<strong>' + cantidadFinal.toFixed(3) + '</strong>');
    
    // Actualizar concepto
    conceptosXML[index].factorConversion = factor;
    conceptosXML[index].cantidadDesglosada = cantidadFinal;
    conceptosXML[index].precioUnitarioDesglosado = conceptosXML[index].valorUnitario / factor;
}

function registrarCompraDesdeXML() {
    // Validar que se haya seleccionado una sucursal
    var sucursalID = parseInt($('#sucursalID').val());
    if (!sucursalID || sucursalID === 0) {
        Swal.fire({
            title: 'Sucursal requerida',
            text: 'Debe seleccionar una sucursal destino antes de registrar la compra',
            icon: 'warning',
            confirmButtonText: 'Entendido'
        });
        return;
    }
    
    // Validar que todos los conceptos tengan producto mapeado
    var mapeos = [];
    var errores = [];
    
    conceptosXML.forEach(function (concepto, index) {
        var selectProducto = $('.select-producto[data-index="' + index + '"]');
        var productoID = selectProducto.val();
        
        if (!productoID || productoID === '') {
            errores.push('Concepto ' + (index + 1) + ': ' + concepto.descripcion.substring(0, 50));
            return;
        }
        
        var factor = parseFloat($('.factor-conversion[data-index="' + index + '"]').val()) || 1;
        var precioVentaSugerido = selectProducto.select2('data')[0].precioVenta || 0;
        
        mapeos.push({
            ProductoID: parseInt(productoID),
            NoIdentificacion: concepto.noIdentificacion || concepto.claveProdServ,
            Descripcion: concepto.descripcion,
            FactorConversion: factor,
            PrecioVentaSugerido: precioVentaSugerido
        });
    });
    
    if (errores.length > 0) {
        Swal.fire({
            title: 'Productos sin mapear',
            html: 'Los siguientes conceptos no tienen producto asignado:<br><br>' + 
                  errores.join('<br>'),
            icon: 'warning'
        });
        return;
    }
    
    // Confirmar registro
    Swal.fire({
        title: '¿Registrar compra?',
        html: 'Se registrará la compra con <strong>' + mapeos.length + ' productos</strong><br>' +
              'Total: <strong>$' + formatMoney(datosXML.total) + '</strong><br>' +
              'Se crearán los lotes automáticamente en el inventario.',
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'Sí, registrar',
        cancelButtonText: 'Cancelar'
    }).then((result) => {
        if (result.isConfirmed) {
            ejecutarRegistroCompra(mapeos);
        }
    });
}

function ejecutarRegistroCompra(mapeos) {
    $('#btnRegistrarCompra').prop('disabled', true).html('<i class="fa fa-spinner fa-spin"></i> Registrando...');
    
    $.ajax({
        url: '/Compra/RegistrarCompraDesdeXML',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify({
            rutaXML: rutaXMLTemporal,
            mapeos: mapeos,
            sucursalID: parseInt($('#sucursalID').val())
        }),
        success: function (response) {
            $('#btnRegistrarCompra').prop('disabled', false).html('<i class="fa fa-save"></i> Registrar Compra y Crear Lotes');
            
            if (response.success) {
                Swal.fire({
                    title: 'Éxito',
                    text: response.mensaje,
                    icon: 'success',
                    confirmButtonText: 'Ver compras'
                }).then(() => {
                    window.location.href = '/Compra/Index';
                });
            } else {
                Swal.fire('Error', response.mensaje, 'error');
            }
        },
        error: function (xhr, status, error) {
            $('#btnRegistrarCompra').prop('disabled', false).html('<i class="fa fa-save"></i> Registrar Compra y Crear Lotes');
            Swal.fire('Error', 'Error al registrar compra: ' + error, 'error');
        }
    });
}

function formatMoney(amount) {
    return parseFloat(amount).toFixed(2).replace(/\d(?=(\d{3})+\.)/g, '$&,');
}

function formatDateTime(dateStr) {
    var date = new Date(dateStr);
    return date.toLocaleDateString('es-MX') + ' ' + date.toLocaleTimeString('es-MX');
}

// Funciones para selección de sucursal
function abrirModalSucursales() {
    console.log('Abriendo modal de sucursales...');
    
    if (tablaSucursales) {
        console.log('Recargando datos de sucursales...');
        tablaSucursales.ajax.reload(function(json) {
            console.log('Datos cargados:', json);
        });
    } else {
        console.error('tablaSucursales no está inicializado');
        toastr.error('Error: tabla de sucursales no inicializada');
        return;
    }
    
    $('#modalSucursales').modal('show');
    console.log('Modal abierto');
}

function seleccionarSucursal(sucursalID, rfc, nombre) {
    $('#sucursalID').val(sucursalID);
    $('#sucursalRFC').val(rfc);
    $('#sucursalNombre').val(nombre);
    $('#modalSucursales').modal('hide');
    
    // Opcional: mostrar mensaje de confirmación
    toastr.success('Sucursal seleccionada: ' + nombre);
}
