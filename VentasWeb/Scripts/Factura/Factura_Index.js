// Factura_Index.js - Gestión de facturas electrónicas

var tablaFacturas;
var ventaIdParam = null;

$(document).ready(function () {
    inicializarTabla();
    configurarFiltros();
    cargarCatalogos();
    
    // Obtener ventaId del ViewBag si existe
    ventaIdParam = $('#ventaIdHidden').val();
    
    if (ventaIdParam) {
        // Si viene con ventaId, abrir modal automáticamente
        setTimeout(function() {
            abrirModalFacturar(ventaIdParam);
        }, 500);
    }
    
    // Evento botón nueva factura
    $('#btnNuevaFactura').click(function() {
        abrirModalFacturar(null);
    });
    
    // Evento botón generar y timbrar
    $('#btnGenerarYTimbrar').click(function() {
        generarYTimbrarFactura();
    });
    
    // Evento agregar concepto
    $('#btnAgregarConcepto').click(function() {
        agregarConcepto();
    });
});

function cargarCatalogos() {
    // Cargar Usos CFDI
    $.get('/Factura/ObtenerUsosCFDI', function(response) {
        if (response.success && response.data) {
            var options = '<option value="">-- Seleccione --</option>';
            response.data.forEach(function(item) {
                options += '<option value="' + item.Clave + '">' + item.Clave + ' - ' + item.Descripcion + '</option>';
            });
            $('#cboUsoCFDI').html(options);
        }
    });
    
    // Cargar Regímenes Fiscales
    $.get('/Factura/ObtenerRegimenesFiscales', function(response) {
        if (response.success && response.data) {
            var options = '<option value="">-- Seleccione --</option>';
            response.data.forEach(function(item) {
                options += '<option value="' + item.Clave + '">' + item.Clave + ' - ' + item.Descripcion + '</option>';
            });
            $('#cboRegimenFiscal').html(options);
        }
    });
}

function abrirModalFacturar(ventaId) {
    // Limpiar formulario
    $('#formFactura')[0].reset();
    $('#tablaConceptos tbody').empty();
    $('#txtVentaID').val('');
    
    if (ventaId) {
        // Cargar datos de la venta
        $.get('/Factura/ObtenerDatosVenta', { ventaId: ventaId }, function(response) {
            if (response.success) {
                var data = response.data;
                
                $('#txtVentaID').val(data.ventaId);
                $('#txtReceptorRFC').val(data.clienteRFC);
                $('#txtReceptorNombre').val(data.clienteNombre);
                $('#txtSubtotal').val(data.subtotal.toFixed(2));
                $('#txtIVA').val(data.iva.toFixed(2));
                $('#txtTotal').val(data.total.toFixed(2));
                $('#cboUsoCFDI').val(data.usoCFDI);
                $('#cboMetodoPago').val(data.metodoPago);
                $('#cboFormaPago').val(data.formaPago);
                
                // Cargar conceptos
                if (data.conceptos && data.conceptos.length > 0) {
                    data.conceptos.forEach(function(concepto) {
                        agregarConcepto(concepto.Producto, concepto.Cantidad, concepto.PrecioVenta);
                    });
                }
            } else {
                toastr.error(response.mensaje || 'Error al cargar datos de venta');
            }
        }).fail(function() {
            toastr.error('Error de conexión al cargar venta');
        });
    }
    
    $('#modalGenerarFactura').modal('show');
}

function agregarConcepto(descripcion = '', cantidad = 1, precioUnitario = 0) {
    var html = '<tr>';
    html += '<td><input type="text" class="form-control form-control-sm concepto-descripcion" value="' + descripcion + '" required></td>';
    html += '<td><input type="number" class="form-control form-control-sm concepto-cantidad" value="' + cantidad + '" min="1" required></td>';
    html += '<td><input type="number" class="form-control form-control-sm concepto-precio" value="' + precioUnitario.toFixed(2) + '" step="0.01" min="0" required></td>';
    html += '<td><input type="number" class="form-control form-control-sm concepto-importe" value="' + (cantidad * precioUnitario).toFixed(2) + '" readonly></td>';
    html += '<td><button type="button" class="btn btn-sm btn-danger" onclick="$(this).closest(\'tr\').remove()"><i class="fas fa-trash"></i></button></td>';
    html += '</tr>';
    
    $('#tablaConceptos tbody').append(html);
    
    // Eventos para calcular importe
    $('#tablaConceptos tbody tr:last .concepto-cantidad, #tablaConceptos tbody tr:last .concepto-precio').on('input', function() {
        var row = $(this).closest('tr');
        var cantidad = parseFloat(row.find('.concepto-cantidad').val()) || 0;
        var precio = parseFloat(row.find('.concepto-precio').val()) || 0;
        row.find('.concepto-importe').val((cantidad * precio).toFixed(2));
    });
}

function generarYTimbrarFactura() {
    // Validar formulario
    if (!$('#formFactura')[0].checkValidity()) {
        $('#formFactura')[0].reportValidity();
        return;
    }
    
    // Validar que haya conceptos
    if ($('#tablaConceptos tbody tr').length === 0) {
        toastr.warning('Debe agregar al menos un concepto');
        return;
    }
    
    // Construir request
    var conceptos = [];
    $('#tablaConceptos tbody tr').each(function() {
        conceptos.push({
            ClaveProdServ: '01010101', // Por defecto
            Descripcion: $(this).find('.concepto-descripcion').val(),
            Cantidad: parseFloat($(this).find('.concepto-cantidad').val()),
            ValorUnitario: parseFloat($(this).find('.concepto-precio').val()),
            Importe: parseFloat($(this).find('.concepto-importe').val()),
            ClaveUnidad: 'E48', // Unidad de servicio
            Unidad: 'Servicio'
        });
    });
    
    var request = {
        VentaID: $('#txtVentaID').val() || null,
        ReceptorRFC: $('#txtReceptorRFC').val(),
        ReceptorNombre: $('#txtReceptorNombre').val(),
        ReceptorRegimenFiscal: $('#cboRegimenFiscal').val(),
        UsoCFDI: $('#cboUsoCFDI').val(),
        FormaPago: $('#cboFormaPago').val(),
        MetodoPago: $('#cboMetodoPago').val(),
        Conceptos: conceptos
    };
    
    // Mostrar loader
    $('#btnGenerarYTimbrar').prop('disabled', true).html('<i class="fas fa-spinner fa-spin"></i> Timbrando...');
    
    // Enviar request
    $.ajax({
        url: '/Factura/GenerarFactura',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(request),
        success: function(response) {
            if (response.success) {
                toastr.success(response.mensaje || 'Factura generada y timbrada correctamente');
                $('#modalGenerarFactura').modal('hide');
                
                // Recargar tabla si existe
                if (tablaFacturas) {
                    tablaFacturas.ajax.reload();
                }
                
                // Si venía de una venta, redirigir al historial
                if ($('#txtVentaID').val()) {
                    setTimeout(function() {
                        window.location.href = '/Venta/Consultar';
                    }, 1500);
                }
            } else {
                toastr.error(response.mensaje || 'Error al generar factura');
            }
        },
        error: function(xhr) {
            var msg = 'Error de conexión';
            try {
                var response = JSON.parse(xhr.responseText);
                msg = response.mensaje || msg;
            } catch(e) {}
            toastr.error(msg);
        },
        complete: function() {
            $('#btnGenerarYTimbrar').prop('disabled', false).html('<i class="fas fa-stamp"></i> Generar y Timbrar');
        }
    });
}

function inicializarTabla() {
    tablaFacturas = $('#tbFacturas').DataTable({
        ajax: {
            url: '/Factura/ObtenerFacturas',
            type: 'GET',
            dataSrc: 'data'
        },
        columns: [
            { 
                data: null, 
                render: function (data) {
                    return data.Serie + data.Folio;
                }
            },
            { 
                data: 'UUID',
                render: function (data) {
                    return data ? data.substring(0, 8) + '...' : 'N/A';
                }
            },
            { 
                data: 'FechaEmision',
                render: function (data) {
                    return moment(data).format('DD/MM/YYYY HH:mm');
                }
            },
            { data: 'ReceptorRFC' },
            { data: 'ReceptorNombre' },
            { 
                data: 'Total',
                render: function (data) {
                    return '$' + parseFloat(data).toFixed(2);
                }
            },
            { 
                data: 'Estatus',
                render: function (data) {
                    return '<span class="badge badge-' + data + '">' + data + '</span>';
                }
            },
            {
                data: null,
                orderable: false,
                render: function (data) {
                    var btnDetalle = '<button class="btn btn-sm btn-info" onclick="verDetalle(\'' + data.FacturaID + '\')"><i class="fas fa-eye"></i></button> ';
                    var btnXML = '<button class="btn btn-sm btn-primary" onclick="descargarXML(\'' + data.FacturaID + '\')"><i class="fas fa-file-code"></i></button> ';
                    var btnPDF = '<button class="btn btn-sm btn-danger" onclick="descargarPDF(\'' + data.FacturaID + '\')"><i class="fas fa-file-pdf"></i></button> ';
                    var btnEmail = '<button class="btn btn-sm btn-success" onclick="enviarEmail(\'' + data.FacturaID + '\')"><i class="fas fa-envelope"></i></button> ';
                    
                    var botones = btnDetalle + btnXML + btnPDF + btnEmail;
                    
                    if (data.Estatus === 'TIMBRADA') {
                        botones += '<button class="btn btn-sm btn-warning" onclick="cancelarFactura(\'' + data.FacturaID + '\')"><i class="fas fa-ban"></i></button>';
                    }
                    
                    return botones;
                }
            }
        ],
        language: {
            url: '//cdn.datatables.net/plug-ins/1.10.24/i18n/Spanish.json'
        },
        responsive: true,
        order: [[2, 'desc']]
    });
}

function configurarFiltros() {
    $('#txtFiltroRFC, #txtFechaDesde, #txtFechaHasta, #cboEstatus').on('change', function () {
        tablaFacturas.ajax.reload();
    });
}

function verDetalle(facturaId) {
    $.ajax({
        url: '/Factura/ObtenerDetalle',
        type: 'GET',
        data: { facturaId: facturaId },
        success: function (response) {
            if (response.success) {
                var factura = response.data;
                
                $('#detalleSerieFolio').text(factura.Serie + factura.Folio);
                $('#detalleUUID').text(factura.UUID || 'N/A');
                $('#detalleFecha').text(moment(factura.FechaEmision).format('DD/MM/YYYY HH:mm'));
                $('#detalleRFC').text(factura.ReceptorRFC);
                $('#detalleCliente').text(factura.ReceptorNombre);
                $('#detalleTotal').text('$' + parseFloat(factura.Total).toFixed(2));
                
                // Conceptos
                var htmlConceptos = '';
                factura.Conceptos.forEach(function (concepto) {
                    htmlConceptos += '<tr>';
                    htmlConceptos += '<td>' + concepto.Descripcion + '</td>';
                    htmlConceptos += '<td>' + concepto.Cantidad + '</td>';
                    htmlConceptos += '<td>$' + parseFloat(concepto.ValorUnitario).toFixed(2) + '</td>';
                    htmlConceptos += '<td>$' + parseFloat(concepto.Importe).toFixed(2) + '</td>';
                    htmlConceptos += '</tr>';
                });
                $('#detalleConceptos').html(htmlConceptos);
                
                $('#modalDetalleFactura').modal('show');
            } else {
                toastr.error(response.mensaje);
            }
        },
        error: function () {
            toastr.error('Error al obtener detalle de factura');
        }
    });
}

function descargarXML(facturaId) {
    window.open('/Factura/DescargarXML?facturaId=' + facturaId, '_blank');
}

function descargarPDF(facturaId) {
    window.open('/Factura/DescargarPDF?facturaId=' + facturaId, '_blank');
}

function enviarEmail(facturaId) {
    // Obtener datos de la factura para mostrar en el modal
    $.ajax({
        url: '/Factura/ObtenerDetalle',
        type: 'GET',
        data: { facturaId: facturaId },
        success: function (response) {
            if (response.success && response.data) {
                var factura = response.data;
                var serieFolio = factura.Serie + factura.Folio;
                var uuid = factura.UUID || '';
                var emailCliente = factura.EmailCliente || '';
                
                // Abrir modal con los datos
                abrirModalEnviarEmail(facturaId, uuid, serieFolio, emailCliente);
            } else {
                toastr.error('No se pudo cargar la información de la factura');
            }
        },
        error: function () {
            toastr.error('Error al cargar la factura');
        }
    });
}

function cancelarFactura(facturaId) {
    // Modal para capturar motivo de cancelación
    var motivo = prompt('Motivo de cancelación:\n01 - Comprobante emitido con errores con relación\n02 - Comprobante emitido con errores sin relación\n03 - No se llevó a cabo la operación\n04 - Operación nominativa relacionada en una factura global');
    
    if (motivo) {
        var folioSustitucion = '';
        if (motivo === '01') {
            folioSustitucion = prompt('Ingrese el UUID del comprobante que sustituye:');
        }
        
        $.ajax({
            url: '/Factura/CancelarFactura',
            type: 'POST',
            data: {
                facturaId: facturaId,
                motivo: motivo,
                folioSustitucion: folioSustitucion
            },
            success: function (response) {
                if (response.success) {
                    toastr.success('Solicitud de cancelación enviada');
                    tablaFacturas.ajax.reload();
                } else {
                    toastr.error(response.mensaje);
                }
            },
            error: function () {
                toastr.error('Error al cancelar factura');
            }
        });
    }
}

// Funciones para cancelación con modal mejorado
function abrirModalCancelar(facturaId, uuid, serieFolio, cliente, total, fechaTimbrado) {
    // Cargar datos en el modal
    $('#cancelarFacturaId').val(facturaId);
    $('#cancelarUUID').text(uuid);
    $('#cancelarSerieFolio').text(serieFolio);
    $('#cancelarCliente').text(cliente);
    $('#cancelarTotal').text('$' + parseFloat(total).toFixed(2));
    $('#cancelarFechaTimbrado').text(fechaTimbrado);
    
    // Limpiar formulario
    $('#cboMotivoCancelacion').val('');
    $('#txtUuidSustitucion').val('');
    $('#divUuidSustitucion').hide();
    
    // Mostrar modal
    $('#modalCancelarFactura').modal('show');
}

// Evento cambio en motivo de cancelación
$(document).on('change', '#cboMotivoCancelacion', function () {
    var motivo = $(this).val();
    if (motivo === '01') {
        $('#divUuidSustitucion').show();
        $('#txtUuidSustitucion').attr('required', 'required');
    } else {
        $('#divUuidSustitucion').hide();
        $('#txtUuidSustitucion').removeAttr('required');
        $('#txtUuidSustitucion').val('');
    }
});

// Evento confirmar cancelación
$(document).on('click', '#btnConfirmarCancelacion', function () {
    var facturaId = $('#cancelarFacturaId').val();
    var motivo = $('#cboMotivoCancelacion').val();
    var uuidSustitucion = $('#txtUuidSustitucion').val();
    
    // Validaciones
    if (!motivo) {
        toastr.warning('Debe seleccionar un motivo de cancelación');
        return;
    }
    
    if (motivo === '01' && !uuidSustitucion) {
        toastr.warning('El motivo 01 requiere UUID de sustitución');
        return;
    }
    
    // Confirmación final con SweetAlert
    Swal.fire({
        title: '¿Confirmar cancelación?',
        html: '<p>Esta operación es <strong>irreversible</strong></p>' +
              '<p class="text-muted"><small>Se enviará la solicitud al SAT a través del PAC</small></p>',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: 'Sí, cancelar CFDI',
        cancelButtonText: 'No, regresar',
        confirmButtonColor: '#dc3545',
        showLoaderOnConfirm: true,
        allowOutsideClick: () => !Swal.isLoading(),
        preConfirm: () => {
            return $.ajax({
                url: '/Factura/CancelarFactura',
                type: 'POST',
                data: {
                    facturaId: parseInt(facturaId),
                    motivo: motivo,
                    uuidSustitucion: uuidSustitucion || null
                },
                dataType: 'json'
            }).then(response => {
                if (!response.success) {
                    throw new Error(response.mensaje || 'Error al cancelar');
                }
                return response;
            }).catch(error => {
                Swal.showValidationMessage('Error: ' + (error.message || error.responseText || 'Error de conexión'));
            });
        }
    }).then((result) => {
        if (result.isConfirmed) {
            $('#modalCancelarFactura').modal('hide');
            
            Swal.fire({
                title: '¡Cancelación Exitosa!',
                html: '<p><strong>Estatus SAT:</strong> ' + result.value.estatusSAT + '</p>' +
                      '<p><strong>Fecha:</strong> ' + result.value.fechaCancelacion + '</p>',
                icon: 'success',
                confirmButtonText: 'Aceptar'
            }).then(() => {
                // Recargar tabla
                tablaFacturas.ajax.reload();
            });
        }
    });
});

// Funciones para envío de email
function abrirModalEnviarEmail(facturaId, uuid, serieFolio, emailCliente) {
    $('#emailFacturaId').val(facturaId);
    $('#emailSerieFolio').text(serieFolio);
    $('#emailUUID').text(uuid);
    
    // Pre-llenar email del cliente si existe
    if (emailCliente) {
        $('#txtEmailDestinatario').val(emailCliente);
    } else {
        $('#txtEmailDestinatario').val('');
    }
    
    $('#modalEnviarEmail').modal('show');
}

// Evento confirmar envío de email
$(document).on('click', '#btnConfirmarEnvio', function () {
    var facturaId = parseInt($('#emailFacturaId').val());
    var email = $('#txtEmailDestinatario').val().trim();
    
    // Validar email
    if (!email) {
        toastr.warning('Ingrese el email del destinatario');
        return;
    }
    
    // Validar formato
    var emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (!emailRegex.test(email)) {
        toastr.warning('Formato de email inválido');
        return;
    }
    
    // Confirmación con SweetAlert
    Swal.fire({
        title: '¿Enviar CFDI por email?',
        html: '<p>Se enviará el CFDI en formato PDF y XML a:</p>' +
              '<p><strong>' + email + '</strong></p>',
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'Sí, enviar',
        cancelButtonText: 'Cancelar',
        confirmButtonColor: '#17a2b8',
        showLoaderOnConfirm: true,
        allowOutsideClick: () => !Swal.isLoading(),
        preConfirm: () => {
            return $.ajax({
                url: '/Factura/EnviarPorEmail',
                type: 'POST',
                data: {
                    facturaId: facturaId,
                    email: email
                },
                dataType: 'json'
            }).then(response => {
                if (!response.success) {
                    throw new Error(response.mensaje || 'Error al enviar');
                }
                return response;
            }).catch(error => {
                Swal.showValidationMessage('Error: ' + (error.message || error.responseText || 'Error de conexión'));
            });
        }
    }).then((result) => {
        if (result.isConfirmed) {
            $('#modalEnviarEmail').modal('hide');
            
            Swal.fire({
                title: '¡Email Enviado!',
                html: '<p>' + result.value.mensaje + '</p>' +
                      '<p class="mt-2"><small>Fecha de envío: ' + result.value.fechaEnvio + '</small></p>',
                icon: 'success',
                confirmButtonText: 'Aceptar'
            });
        }
    });
});
