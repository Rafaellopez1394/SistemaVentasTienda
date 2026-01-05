$(document).ready(function () {
    // Inicializar fechas
    var hoy = new Date();
    $('#fechaInicio').val(formatDate(hoy));
    $('#fechaFin').val(formatDate(hoy));

    // Cargar gastos del día
    cargarGastos();
    cargarResumen();

    // Registrar gasto
    $('#formGasto').on('submit', function (e) {
        e.preventDefault();
        registrarGasto();
    });

    // Cambio de fechas
    $('#fechaInicio, #fechaFin').on('change', function () {
        cargarGastos();
        cargarResumen();
    });

    // Refrescar
    $('#btnRefrescar').on('click', function () {
        cargarGastos();
        cargarResumen();
    });

    // Limpiar formulario
    $('#btnLimpiar').on('click', function () {
        limpiarFormulario();
    });

    // Validar monto máximo
    $('#categoriaGastoID, #monto').on('change', function () {
        validarMontoMaximo();
    });
});

function registrarGasto() {
    var gasto = {
        CategoriaGastoID: parseInt($('#categoriaGastoID').val()),
        Concepto: $('#concepto').val().trim(),
        Descripcion: $('#descripcion').val().trim() || null,
        Monto: parseFloat($('#monto').val()),
        NumeroFactura: $('#numeroFactura').val().trim() || null,
        Proveedor: $('#proveedor').val().trim() || null,
        FormaPagoID: $('#formaPagoID').val() ? parseInt($('#formaPagoID').val()) : null,
        Observaciones: $('#observaciones').val().trim() || null
    };

    // Validaciones
    if (!gasto.CategoriaGastoID || gasto.CategoriaGastoID <= 0) {
        Swal.fire('Error', 'Debe seleccionar una categoría', 'error');
        return;
    }

    if (!gasto.Concepto || gasto.Concepto.length === 0) {
        Swal.fire('Error', 'Debe ingresar un concepto', 'error');
        return;
    }

    if (!gasto.Monto || gasto.Monto <= 0) {
        Swal.fire('Error', 'Debe ingresar un monto válido', 'error');
        return;
    }

    $('#btnRegistrar').prop('disabled', true).html('<i class="fa fa-spinner fa-spin"></i> Registrando...');

    $.ajax({
        url: '/Gastos/RegistrarGasto',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(gasto),
        success: function (response) {
            $('#btnRegistrar').prop('disabled', false).html('<i class="fa fa-save"></i> Registrar Gasto');

            if (response.success) {
                Swal.fire('Éxito', response.mensaje, 'success');
                limpiarFormulario();
                cargarGastos();
                cargarResumen();
            } else {
                Swal.fire('Error', response.mensaje, 'error');
            }
        },
        error: function (xhr, status, error) {
            $('#btnRegistrar').prop('disabled', false).html('<i class="fa fa-save"></i> Registrar Gasto');
            Swal.fire('Error', 'Error al registrar gasto: ' + error, 'error');
        }
    });
}

function cargarGastos() {
    var fechaInicio = $('#fechaInicio').val();
    var fechaFin = $('#fechaFin').val();

    console.log('Cargando gastos:', fechaInicio, fechaFin);

    $.ajax({
        url: '/Gastos/ObtenerGastos',
        type: 'GET',
        data: { fechaInicio: fechaInicio, fechaFin: fechaFin },
        success: function (response) {
            console.log('Respuesta recibida:', response);
            if (response.success) {
                mostrarGastos(response.data);
            } else {
                console.error('Error en respuesta:', response.mensaje);
                $('#tbodyGastos').html('<tr><td colspan="7" class="text-center text-danger"><i class="fa fa-exclamation-triangle"></i> ' + response.mensaje + '</td></tr>');
                Swal.fire('Error', response.mensaje, 'error');
            }
        },
        error: function (xhr, status, error) {
            console.error('Error al cargar gastos:', error, xhr.status, xhr.responseText);
            if (xhr.status === 401 || xhr.status === 403) {
                $('#tbodyGastos').html('<tr><td colspan="7" class="text-center text-warning"><i class="fa fa-lock"></i> Debe iniciar sesión para ver los gastos</td></tr>');
            } else if (xhr.status === 200 && xhr.responseText.indexOf('login') > -1) {
                $('#tbodyGastos').html('<tr><td colspan="7" class="text-center text-warning"><i class="fa fa-lock"></i> Sesión expirada. Por favor, <a href="/Login">inicie sesión</a></td></tr>');
            } else {
                $('#tbodyGastos').html('<tr><td colspan="7" class="text-center text-danger"><i class="fa fa-exclamation-triangle"></i> Error al cargar gastos</td></tr>');
                Swal.fire('Error', 'Error al cargar gastos: ' + error, 'error');
            }
        }
    });
}

function mostrarGastos(gastos) {
    console.log('Mostrando gastos:', gastos.length, gastos);
    var tbody = $('#tbodyGastos');
    tbody.empty();

    if (!gastos || gastos.length === 0) {
        tbody.append('<tr><td colspan="7" class="text-center text-muted"><i class="fa fa-info-circle"></i> No hay gastos registrados</td></tr>');
        $('#totalGastos').text('$0.00');
        return;
    }

    var total = 0;

    gastos.forEach(function (gasto, index) {
        try {
            total += gasto.Monto;

            // Parsear fecha - Manejar ambos formatos
            var fecha;
            if (typeof gasto.FechaGasto === 'string' && gasto.FechaGasto.indexOf('/Date(') === 0) {
                // Formato JSON de .NET: /Date(1234567890000)/
                fecha = new Date(parseInt(gasto.FechaGasto.substr(6)));
            } else {
                // Formato ISO o string normal
                fecha = new Date(gasto.FechaGasto);
            }
            
            var estadoBadge = getEstadoBadge(gasto.Estado);

            var row = '<tr>' +
                '<td>' + formatDateTime(fecha) + '</td>' +
                '<td><span class="label label-info">' + (gasto.Categoria || 'Sin categoría') + '</span></td>' +
                '<td>' + (gasto.Concepto || '') + (gasto.Descripcion ? '<br><small class="text-muted">' + gasto.Descripcion + '</small>' : '') + '</td>' +
                '<td class="text-right"><strong>$' + formatMoney(gasto.Monto) + '</strong></td>' +
                '<td>' + estadoBadge + '</td>' +
                '<td>' + (gasto.UsuarioRegistro || '') + '</td>' +
                '<td>' +
                '<button class="btn btn-xs btn-info" onclick="verDetalleGasto(\'' + gasto.GastoID + '\')"><i class="fa fa-eye"></i></button> ';

            if (!gasto.Cancelado) {
                row += '<button class="btn btn-xs btn-danger" onclick="cancelarGasto(\'' + gasto.GastoID + '\')"><i class="fa fa-times"></i></button>';
            }

            row += '</td></tr>';

            tbody.append(row);
        } catch (error) {
            console.error('Error al procesar gasto #' + index, error, gasto);
        }
    });

    $('#totalGastos').text('$' + formatMoney(total));
    console.log('Total calculado:', total, 'gastos procesados');
}

function cargarResumen() {
    var fechaInicio = $('#fechaInicio').val();
    var fechaFin = $('#fechaFin').val();

    $.ajax({
        url: '/Gastos/ObtenerResumen',
        type: 'GET',
        data: { fechaInicio: fechaInicio, fechaFin: fechaFin },
        success: function (response) {
            if (response.success) {
                mostrarResumen(response.data);
            }
        },
        error: function (xhr, status, error) {
            console.error('Error al cargar resumen:', error);
        }
    });
}

function mostrarResumen(resumen) {
    var container = $('#resumenCategorias');
    container.empty();

    if (resumen.length === 0) {
        container.append('<div class="col-md-12 text-center text-muted"><i class="fa fa-info-circle"></i> No hay datos para mostrar</div>');
        return;
    }

    resumen.forEach(function (item) {
        var box = '<div class="col-md-4">' +
            '<div class="info-box bg-aqua">' +
            '<span class="info-box-icon"><i class="fa fa-tag"></i></span>' +
            '<div class="info-box-content">' +
            '<span class="info-box-text">' + item.Categoria + '</span>' +
            '<span class="info-box-number">$' + formatMoney(item.MontoTotal) + '</span>' +
            '<div class="progress"><div class="progress-bar" style="width: 100%"></div></div>' +
            '<span class="progress-description">' + item.TotalGastos + ' gasto(s) - Promedio: $' + formatMoney(item.PromedioGasto) + '</span>' +
            '</div></div></div>';

        container.append(box);
    });
}

function cancelarGasto(gastoID) {
    Swal.fire({
        title: '¿Cancelar gasto?',
        text: 'Ingrese el motivo de cancelación',
        input: 'textarea',
        inputPlaceholder: 'Motivo de cancelación...',
        showCancelButton: true,
        confirmButtonText: 'Sí, cancelar',
        cancelButtonText: 'No',
        inputValidator: (value) => {
            if (!value) {
                return 'Debe ingresar un motivo de cancelación';
            }
        }
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: '/Gastos/CancelarGasto',
                type: 'POST',
                data: { gastoID: gastoID, motivo: result.value },
                success: function (response) {
                    if (response.success) {
                        Swal.fire('Cancelado', response.mensaje, 'success');
                        cargarGastos();
                        cargarResumen();
                    } else {
                        Swal.fire('Error', response.mensaje, 'error');
                    }
                },
                error: function (xhr, status, error) {
                    Swal.fire('Error', 'Error al cancelar gasto: ' + error, 'error');
                }
            });
        }
    });
}

function validarMontoMaximo() {
    var categoriaOption = $('#categoriaGastoID option:selected');
    var montoMaximo = categoriaOption.data('maximo');
    var monto = parseFloat($('#monto').val()) || 0;

    if (montoMaximo && monto > montoMaximo) {
        $('#alertaMontoMaximo').show();
    } else {
        $('#alertaMontoMaximo').hide();
    }
}

function limpiarFormulario() {
    $('#formGasto')[0].reset();
    $('#alertaMontoMaximo').hide();
}

function getEstadoBadge(estado) {
    switch (estado) {
        case 'APROBADO':
            return '<span class="label label-success">APROBADO</span>';
        case 'PENDIENTE APROBACION':
            return '<span class="label label-warning">PENDIENTE</span>';
        case 'CANCELADO':
            return '<span class="label label-danger">CANCELADO</span>';
        default:
            return '<span class="label label-default">' + estado + '</span>';
    }
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

function formatDateTime(date) {
    var options = { year: 'numeric', month: '2-digit', day: '2-digit', hour: '2-digit', minute: '2-digit' };
    return date.toLocaleDateString('es-MX', options);
}

function formatMoney(amount) {
    return parseFloat(amount).toFixed(2).replace(/\d(?=(\d{3})+\.)/g, '$&,');
}

function verDetalleGasto(gastoID) {
    // TODO: Implementar modal con detalles completos
    Swal.fire('Info', 'Función en desarrollo', 'info');
}
