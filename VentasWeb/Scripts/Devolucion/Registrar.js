// Scripts/Devolucion/Registrar.js
var ventaActual = null;
var productosSeleccionados = [];

$(document).ready(function () {
    $('#txtNumeroVenta').focus();
    
    // Enter en búsqueda
    $('#txtNumeroVenta').keypress(function (e) {
        if (e.which === 13) {
            buscarVenta();
        }
    });
});

function buscarVenta() {
    var numeroVenta = $('#txtNumeroVenta').val().trim();
    
    if (!numeroVenta) {
        Swal.fire('Advertencia', 'Ingrese el número de venta', 'warning');
        return;
    }
    
    $.ajax({
        url: '/Devolucion/BuscarVentaPorNumero',
        type: 'GET',
        data: { numeroVenta: numeroVenta },
        beforeSend: function () {
            Swal.fire({
                title: 'Buscando...',
                allowOutsideClick: false,
                didOpen: () => {
                    Swal.showLoading();
                }
            });
        },
        success: function (response) {
            Swal.close();
            
            if (response.success) {
                ventaActual = response.data;
                mostrarDetalleVenta(ventaActual);
                
                if (response.devolucionesPrevias > 0) {
                    Swal.fire({
                        icon: 'warning',
                        title: 'Advertencia',
                        text: 'Esta venta ya tiene ' + response.devolucionesPrevias + ' devolución(es) registrada(s)'
                    });
                }
            } else {
                Swal.fire('Error', response.mensaje, 'error');
                limpiarFormulario();
            }
        },
        error: function () {
            Swal.close();
            Swal.fire('Error', 'No se pudo buscar la venta', 'error');
        }
    });
}

function mostrarDetalleVenta(venta) {
    // Mostrar info de la venta
    $('#ventaFecha').text(new Date(venta.FechaVenta).toLocaleString('es-MX'));
    $('#ventaCliente').text(venta.NombreCliente || 'PÚBLICO EN GENERAL');
    $('#ventaTotal').text('$' + formatMoney(venta.Total));
    $('#ventaSucursal').text(venta.NombreSucursal || '');
    
    $('#infoVenta').slideDown();
    
    // Renderizar productos
    renderizarProductos(venta.DetalleVenta);
    
    $('#cardProductos').slideDown();
    $('#cardDatos').slideDown();
}

function renderizarProductos(productos) {
    var html = '';
    productosSeleccionados = [];
    
    productos.forEach(function (prod, index) {
        html += '<tr data-index="' + index + '">';
        html += '<td class="text-center">';
        html += '<input type="checkbox" class="check-producto" data-index="' + index + '" onchange="actualizarSeleccion()" />';
        html += '</td>';
        html += '<td>' + prod.CodigoProducto + '</td>';
        html += '<td>' + prod.NombreProducto + '</td>';
        html += '<td class="text-center">' + prod.Cantidad + '</td>';
        html += '<td>';
        html += '<input type="number" class="form-control form-control-sm cantidad-devolver" ';
        html += 'data-index="' + index + '" min="0" max="' + prod.Cantidad + '" value="0" ';
        html += 'onchange="actualizarTotal()" />';
        html += '</td>';
        html += '<td class="text-right">$' + formatMoney(prod.PrecioVenta) + '</td>';
        html += '<td class="text-right subtotal" data-index="' + index + '">$0.00</td>';
        html += '</tr>';
        
        // Guardar producto
        productosSeleccionados.push({
            ProductoID: prod.ProductoID,
            LoteID: prod.LoteID || 0,
            CodigoProducto: prod.CodigoProducto,
            NombreProducto: prod.NombreProducto,
            CantidadOriginal: prod.Cantidad,
            PrecioVenta: prod.PrecioVenta,
            CantidadDevolver: 0,
            Seleccionado: false
        });
    });
    
    $('#tbodyProductos').html(html);
    $('#totalDevolver').text('$0.00');
}

function seleccionarTodos() {
    var checked = $('#checkTodos').prop('checked');
    $('.check-producto').prop('checked', checked);
    
    if (checked) {
        $('.cantidad-devolver').each(function () {
            var index = $(this).data('index');
            var max = parseFloat($(this).attr('max'));
            $(this).val(max);
            productosSeleccionados[index].CantidadDevolver = max;
            productosSeleccionados[index].Seleccionado = true;
        });
    } else {
        $('.cantidad-devolver').val(0);
        productosSeleccionados.forEach(function (p) {
            p.CantidadDevolver = 0;
            p.Seleccionado = false;
        });
    }
    
    actualizarTotal();
}

function actualizarSeleccion() {
    $('.check-producto:checked').each(function () {
        var index = $(this).data('index');
        var max = $('.cantidad-devolver[data-index="' + index + '"]').attr('max');
        $('.cantidad-devolver[data-index="' + index + '"]').val(max);
        productosSeleccionados[index].CantidadDevolver = parseFloat(max);
        productosSeleccionados[index].Seleccionado = true;
    });
    
    $('.check-producto:not(:checked)').each(function () {
        var index = $(this).data('index');
        $('.cantidad-devolver[data-index="' + index + '"]').val(0);
        productosSeleccionados[index].CantidadDevolver = 0;
        productosSeleccionados[index].Seleccionado = false;
    });
    
    actualizarTotal();
}

function actualizarTotal() {
    var total = 0;
    
    $('.cantidad-devolver').each(function () {
        var index = $(this).data('index');
        var cantidad = parseFloat($(this).val()) || 0;
        var precio = productosSeleccionados[index].PrecioVenta;
        var subtotal = cantidad * precio;
        
        productosSeleccionados[index].CantidadDevolver = cantidad;
        $('.subtotal[data-index="' + index + '"]').text('$' + formatMoney(subtotal));
        
        total += subtotal;
    });
    
    $('#totalDevolver').text('$' + formatMoney(total));
}

function registrarDevolucion() {
    // Validaciones
    if (!ventaActual) {
        Swal.fire('Error', 'Debe buscar una venta primero', 'error');
        return;
    }
    
    var tipo = $('#cboTipoDevolucion').val();
    if (!tipo) {
        Swal.fire('Advertencia', 'Seleccione el tipo de devolución', 'warning');
        return;
    }
    
    var formaReintegro = $('#cboFormaReintegro').val();
    if (!formaReintegro) {
        Swal.fire('Advertencia', 'Seleccione la forma de reintegro', 'warning');
        return;
    }
    
    var motivo = $('#txtMotivo').val().trim();
    if (!motivo) {
        Swal.fire('Advertencia', 'Ingrese el motivo de la devolución', 'warning');
        return;
    }
    
    // Obtener productos a devolver
    var productosADevolver = productosSeleccionados.filter(function (p) {
        return p.CantidadDevolver > 0;
    });
    
    if (productosADevolver.length === 0) {
        Swal.fire('Advertencia', 'Debe seleccionar al menos un producto para devolver', 'warning');
        return;
    }
    
    // Preparar payload
    var payload = {
        VentaID: ventaActual.VentaID,
        TipoDevolucion: tipo,
        MotivoDevolucion: motivo,
        FormaReintegro: formaReintegro,
        Productos: productosADevolver.map(function (p) {
            return {
                ProductoID: p.ProductoID,
                LoteID: p.LoteID,
                Cantidad: p.CantidadDevolver,
                PrecioVenta: p.PrecioVenta
            };
        })
    };
    
    // Confirmar
    Swal.fire({
        title: '¿Confirmar Devolución?',
        html: '<p>Se devolverán <strong>' + productosADevolver.length + '</strong> producto(s)</p>' +
              '<p>Total: <strong>$' + $('#totalDevolver').text().replace('$', '') + '</strong></p>' +
              '<p class="text-danger"><small>Los productos se reintegrarán al inventario</small></p>',
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'Sí, Registrar',
        cancelButtonText: 'Cancelar',
        confirmButtonColor: '#28a745',
        cancelButtonColor: '#6c757d'
    }).then((result) => {
        if (result.isConfirmed) {
            ejecutarRegistro(payload);
        }
    });
}

function ejecutarRegistro(payload) {
    $.ajax({
        url: '/Devolucion/RegistrarDevolucion',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(payload),
        beforeSend: function () {
            Swal.fire({
                title: 'Registrando...',
                allowOutsideClick: false,
                didOpen: () => {
                    Swal.showLoading();
                }
            });
        },
        success: function (response) {
            Swal.close();
            
            if (response.success) {
                Swal.fire({
                    icon: 'success',
                    title: 'Devolución Registrada',
                    text: response.mensaje,
                    confirmButtonText: 'Ver Historial'
                }).then(() => {
                    window.location.href = '/Devolucion/Index';
                });
            } else {
                Swal.fire('Error', response.mensaje, 'error');
            }
        },
        error: function () {
            Swal.close();
            Swal.fire('Error', 'No se pudo registrar la devolución', 'error');
        }
    });
}

function cancelar() {
    Swal.fire({
        title: '¿Cancelar?',
        text: 'Se perderán los datos ingresados',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: 'Sí, Cancelar',
        cancelButtonText: 'No'
    }).then((result) => {
        if (result.isConfirmed) {
            limpiarFormulario();
        }
    });
}

function limpiarFormulario() {
    ventaActual = null;
    productosSeleccionados = [];
    
    $('#txtNumeroVenta').val('');
    $('#infoVenta').hide();
    $('#cardProductos').hide();
    $('#cardDatos').hide();
    $('#tbodyProductos').empty();
    $('#cboTipoDevolucion').val('');
    $('#cboFormaReintegro').val('');
    $('#txtMotivo').val('');
    $('#totalDevolver').text('$0.00');
    $('#checkTodos').prop('checked', false);
    
    $('#txtNumeroVenta').focus();
}

function formatMoney(amount) {
    return parseFloat(amount).toFixed(2).replace(/\d(?=(\d{3})+\.)/g, '$&,');
}
