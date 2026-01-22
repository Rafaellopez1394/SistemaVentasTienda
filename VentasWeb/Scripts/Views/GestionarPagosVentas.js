// ============================================================================
// GESTIÓN DE PAGOS DE VENTAS PARCIALES
// ============================================================================

let ventasPendientes = [];
let ventaSeleccionada = null;
let clienteSeleccionado = null;

// ============================================================================
// INICIALIZACIÓN
// ============================================================================
$(document).ready(function () {
    cargarFormasPago();
    cargarMetodosPago();
    
    // Verificar si viene un ventaId en la URL
    const urlParams = new URLSearchParams(window.location.search);
    const ventaId = urlParams.get('ventaId');
    
    if (ventaId) {
        console.log('VentaId detectado en URL:', ventaId);
        // Cargar directamente la venta específica
        cargarVentaEspecifica(ventaId);
    } else {
        // Cargar todas las ventas pendientes
        cargarVentasPendientes();
    }

    // Configurar fecha actual en el modal de pago
    const now = new Date();
    const fechaLocal = new Date(now.getTime() - now.getTimezoneOffset() * 60000).toISOString().slice(0, 16);
    $('#txtFechaPago').val(fechaLocal);

    // Buscar cliente al presionar Enter
    $('#txtBuscarCliente').on('keypress', function (e) {
        if (e.which === 13) {
            buscarCliente();
        }
    });
});

// ============================================================================
// BUSCAR CLIENTE
// ============================================================================
function buscarCliente() {
    const termino = $('#txtBuscarCliente').val().trim();
    
    if (termino.length < 3) {
        toastr.warning('Ingrese al menos 3 caracteres para buscar');
        return;
    }

    $.get('/Cliente/BuscarClientes', { termino: termino }, function (res) {
        if (res.success && res.data.length > 0) {
            let html = '<div class="list-group">';
            
            res.data.forEach(cliente => {
                html += `
                    <a href="javascript:void(0)" 
                       class="list-group-item list-group-item-action" 
                       onclick="seleccionarClienteFiltro('${cliente.ClienteID}', '${cliente.RazonSocial}', '${cliente.RFC}')">
                        <div class="d-flex justify-content-between align-items-center">
                            <div>
                                <h6 class="mb-0">${cliente.RazonSocial}</h6>
                                <small class="text-muted">RFC: ${cliente.RFC}</small>
                            </div>
                            <i class="fas fa-chevron-right"></i>
                        </div>
                    </a>
                `;
            });
            
            html += '</div>';
            $('#resultadosCliente').html(html);
        } else {
            $('#resultadosCliente').html('<div class="alert alert-warning">No se encontraron clientes</div>');
        }
    }).fail(function () {
        toastr.error('Error al buscar clientes');
    });
}

function seleccionarClienteFiltro(clienteID, nombre, rfc) {
    clienteSeleccionado = { ClienteID: clienteID, Nombre: nombre, RFC: rfc };
    $('#txtBuscarCliente').val(nombre);
    $('#resultadosCliente').empty();
    cargarVentasPendientes(clienteID);
}

function limpiarFiltroCliente() {
    clienteSeleccionado = null;
    $('#txtBuscarCliente').val('');
    $('#resultadosCliente').empty();
    cargarVentasPendientes();
}

// ============================================================================
// CARGAR VENTAS PENDIENTES
// ============================================================================
function cargarVentaEspecifica(ventaId) {
    console.log('Cargando venta específica:', ventaId);
    
    // Primero intentar obtener desde el endpoint de ventas pendientes
    $.get('/Pagos/ObtenerVentasPendientes', { clienteID: null }, function (res) {
        console.log('Respuesta de ventas pendientes:', res);
        
        if (res.success && res.data) {
            // Buscar la venta específica en los resultados
            const ventaEncontrada = res.data.find(v => v.VentaID === ventaId);
            
            if (ventaEncontrada) {
                ventasPendientes = [ventaEncontrada];
                console.log('Venta encontrada:', ventaEncontrada);
                mostrarVentas(ventasPendientes);
                actualizarResumen(ventasPendientes);
                toastr.info('Mostrando venta específica. Use "Actualizar Lista" para ver todas las ventas pendientes.');
            } else {
                console.warn('Venta no encontrada en ventas pendientes, cargando todas...');
                ventasPendientes = res.data;
                mostrarVentas(ventasPendientes);
                actualizarResumen(ventasPendientes);
                toastr.warning('La venta solicitada no se encontró o ya está pagada. Mostrando todas las ventas pendientes.');
            }
        } else {
            console.error('Error al cargar ventas:', res.mensaje || res);
            toastr.error(res.mensaje || 'No se pudieron cargar las ventas');
        }
    }).fail(function (xhr, status, error) {
        console.error('Error en petición AJAX:', status, error);
        console.error('Respuesta del servidor:', xhr.responseText);
        toastr.error('Error al cargar las ventas');
    });
}

function cargarVentasPendientes(clienteID = null) {
    console.log('Cargando ventas pendientes. ClienteID:', clienteID);
    
    $.get('/Pagos/ObtenerVentasPendientes', { clienteID: clienteID }, function (res) {
        console.log('Respuesta recibida:', res);
        
        if (res.success) {
            ventasPendientes = res.data;
            console.log('Ventas pendientes:', ventasPendientes.length, 'registros');
            mostrarVentas(ventasPendientes);
            actualizarResumen(ventasPendientes);
        } else {
            console.error('Error en respuesta:', res.mensaje);
            toastr.error(res.mensaje || 'Error al cargar ventas');
        }
    }).fail(function (xhr, status, error) {
        console.error('Error en petición AJAX:', status, error);
        console.error('Respuesta del servidor:', xhr.responseText);
        toastr.error('Error al cargar ventas pendientes');
    });
}

function mostrarVentas(ventas) {
    if (ventas.length === 0) {
        $('#listaVentas').html(`
            <div class="col-12">
                <div class="alert alert-info text-center">
                    <i class="fas fa-check-circle"></i> No hay ventas pendientes de pago
                </div>
            </div>
        `);
        return;
    }

    let html = '';
    
    ventas.forEach(venta => {
        const porcentajePagado = (venta.MontoPagado / venta.Total * 100).toFixed(1);
        const fechaVenta = moment(venta.FechaVenta).format('DD/MM/YYYY HH:mm');
        const diasVencidos = moment().diff(moment(venta.FechaVenta), 'days');
        
        const claseSaldo = venta.SaldoPendiente > 0 ? 'venta-card' : 'venta-card venta-pagada';
        const badgeEstatus = venta.SaldoPendiente <= 0.01 
            ? '<span class="badge bg-success">PAGADA</span>'
            : `<span class="badge bg-warning text-dark">PENDIENTE</span>`;

        html += `
            <div class="col-md-6 col-lg-4 mb-3">
                <div class="card ${claseSaldo}">
                    <div class="card-header d-flex justify-content-between align-items-center">
                        <div>
                            <strong>${venta.ClienteRazonSocial}</strong>
                            ${badgeEstatus}
                        </div>
                        <small class="text-muted">${fechaVenta}</small>
                    </div>
                    <div class="card-body">
                        <p class="mb-1"><small class="text-muted">RFC:</small> ${venta.ClienteRFC}</p>
                        <p class="mb-1"><small class="text-muted">Días transcurridos:</small> ${diasVencidos}</p>
                        
                        <hr />
                        
                        <div class="row mb-2">
                            <div class="col-6">
                                <small class="text-muted">Total:</small>
                                <h5>$${venta.Total.toFixed(2)}</h5>
                            </div>
                            <div class="col-6">
                                <small class="text-muted">Pagado:</small>
                                <h5 class="monto-pagado">$${venta.MontoPagado.toFixed(2)}</h5>
                            </div>
                        </div>
                        
                        <div class="alert alert-${venta.SaldoPendiente > 0 ? 'danger' : 'success'} mb-2">
                            <small>Saldo Pendiente:</small>
                            <div class="saldo-pendiente">$${venta.SaldoPendiente.toFixed(2)}</div>
                        </div>
                        
                        <div class="progress mb-3" style="height: 25px;">
                            <div class="progress-bar bg-success" role="progressbar" 
                                 style="width: ${porcentajePagado}%" 
                                 aria-valuenow="${porcentajePagado}" aria-valuemin="0" aria-valuemax="100">
                                ${porcentajePagado}% pagado
                            </div>
                        </div>
                        
                        <p class="mb-2">
                            <i class="fas fa-receipt"></i> 
                            <strong>Pagos registrados:</strong> ${venta.NumeroPagos}
                        </p>
                    </div>
                    <div class="card-footer">
                        <div class="btn-group w-100" role="group">
                            ${venta.SaldoPendiente > 0.01 ? `
                                <button class="btn btn-success" onclick="mostrarModalPago('${venta.VentaID}')">
                                    <i class="fas fa-dollar-sign"></i> Pagar
                                </button>
                            ` : ''}
                            <button class="btn btn-info" onclick="verHistorial('${venta.VentaID}')">
                                <i class="fas fa-history"></i> Historial
                            </button>
                        </div>
                    </div>
                </div>
            </div>
        `;
    });
    
    $('#listaVentas').html(html);
}

function actualizarResumen(ventas) {
    const totalVentas = ventas.length;
    const saldoTotal = ventas.reduce((sum, v) => sum + v.SaldoPendiente, 0);
    const totalPagado = ventas.reduce((sum, v) => sum + v.MontoPagado, 0);
    
    $('#lblTotalVentas').text(totalVentas);
    $('#lblSaldoTotal').text('$' + saldoTotal.toFixed(2));
    $('#lblTotalPagado').text('$' + totalPagado.toFixed(2));
}

// ============================================================================
// MODAL REGISTRAR PAGO
// ============================================================================
function mostrarModalPago(ventaID) {
    ventaSeleccionada = ventasPendientes.find(v => v.VentaID === ventaID);
    
    if (!ventaSeleccionada) {
        toastr.error('No se encontró la venta');
        return;
    }

    // Llenar información de la venta
    $('#pagoVentaID').val(ventaSeleccionada.VentaID);
    $('#pagoClienteNombre').text(ventaSeleccionada.ClienteRazonSocial);
    $('#pagoClienteRFC').text(ventaSeleccionada.ClienteRFC);
    $('#pagoFechaVenta').text(moment(ventaSeleccionada.FechaVenta).format('DD/MM/YYYY HH:mm'));
    $('#pagoTotalVenta').text(ventaSeleccionada.Total.toFixed(2));
    $('#pagoMontoPagado').text(ventaSeleccionada.MontoPagado.toFixed(2));
    $('#pagoSaldoPendiente').text(ventaSeleccionada.SaldoPendiente.toFixed(2));
    
    // Limpiar formulario
    $('#formRegistrarPago')[0].reset();
    $('#txtMontoPago').val('');
    $('#txtNuevoSaldo').text('');
    
    // Mostrar modal
    const modal = new bootstrap.Modal(document.getElementById('modalRegistrarPago'));
    modal.show();
}

function validarMontoPago() {
    const monto = parseFloat($('#txtMontoPago').val()) || 0;
    const saldoPendiente = ventaSeleccionada.SaldoPendiente;
    
    if (monto > saldoPendiente) {
        $('#txtNuevoSaldo').text('⚠️ El monto excede el saldo pendiente').addClass('text-danger');
    } else if (monto > 0) {
        const nuevoSaldo = saldoPendiente - monto;
        $('#txtNuevoSaldo').text(`Nuevo saldo: $${nuevoSaldo.toFixed(2)}`).removeClass('text-danger').addClass('text-success');
    } else {
        $('#txtNuevoSaldo').text('');
    }
}

function procesarPago() {
    // Validar formulario
    const monto = parseFloat($('#txtMontoPago').val());
    const formaPagoID = $('#cboFormaPagoPago').val();
    const metodoPagoID = $('#cboMetodoPagoPago').val();
    
    if (!monto || monto <= 0) {
        toastr.warning('Ingrese el monto del pago');
        $('#txtMontoPago').focus();
        return;
    }
    
    if (monto > ventaSeleccionada.SaldoPendiente) {
        toastr.warning('El monto no puede ser mayor al saldo pendiente');
        $('#txtMontoPago').focus();
        return;
    }
    
    if (!formaPagoID) {
        toastr.warning('Seleccione una forma de pago');
        $('#cboFormaPagoPago').focus();
        return;
    }
    
    if (!metodoPagoID) {
        toastr.warning('Seleccione un método de pago');
        $('#cboMetodoPagoPago').focus();
        return;
    }

    // Confirmar
    const nuevoSaldo = ventaSeleccionada.SaldoPendiente - monto;
    if (!confirm(`¿Confirmar pago?\n\nMonto: $${monto.toFixed(2)}\nNuevo saldo: $${nuevoSaldo.toFixed(2)}`)) {
        return;
    }

    // Preparar datos
    const request = {
        VentaID: $('#pagoVentaID').val(),
        FormaPagoID: parseInt(formaPagoID),
        MetodoPagoID: parseInt(metodoPagoID),
        Monto: monto,
        Referencia: $('#txtReferencia').val(),
        Observaciones: $('#txtObservaciones').val()
    };

    // Enviar
    $.ajax({
        url: '/Pagos/RegistrarPagoVenta',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(request),
        success: function (res) {
            if (res.success) {
                toastr.success(res.mensaje);
                
                // Cerrar modal
                bootstrap.Modal.getInstance(document.getElementById('modalRegistrarPago')).hide();
                
                // Recargar ventas
                if (clienteSeleccionado) {
                    cargarVentasPendientes(clienteSeleccionado.ClienteID);
                } else {
                    cargarVentasPendientes();
                }
            } else {
                toastr.error(res.mensaje);
            }
        },
        error: function () {
            toastr.error('Error al registrar el pago');
        }
    });
}

// ============================================================================
// MODAL HISTORIAL
// ============================================================================
function verHistorial(ventaID) {
    ventaSeleccionada = ventasPendientes.find(v => v.VentaID === ventaID);
    
    if (!ventaSeleccionada) {
        toastr.error('No se encontró la venta');
        return;
    }

    // Llenar información
    $('#historialClienteNombre').text(ventaSeleccionada.ClienteRazonSocial);
    $('#historialTotalVenta').text(ventaSeleccionada.Total.toFixed(2));
    $('#listaHistorialPagos').html('<div class="text-center"><i class="fas fa-spinner fa-spin"></i> Cargando...</div>');
    
    // Mostrar modal
    const modal = new bootstrap.Modal(document.getElementById('modalHistorialPagos'));
    modal.show();

    // Cargar historial
    $.get('/Pagos/ObtenerHistorialPagos', { ventaID: ventaID }, function (res) {
        if (res.success) {
            mostrarHistorialPagos(res.data);
        } else {
            $('#listaHistorialPagos').html('<div class="alert alert-danger">Error: ' + res.mensaje + '</div>');
        }
    }).fail(function () {
        $('#listaHistorialPagos').html('<div class="alert alert-danger">Error al cargar historial</div>');
    });
}

function mostrarHistorialPagos(pagos) {
    if (pagos.length === 0) {
        $('#listaHistorialPagos').html('<div class="alert alert-info">No hay pagos registrados</div>');
        return;
    }

    let html = '<div class="table-responsive"><table class="table table-hover">';
    html += '<thead><tr>';
    html += '<th>#</th>';
    html += '<th>Fecha</th>';
    html += '<th>Forma de Pago</th>';
    html += '<th>Método</th>';
    html += '<th>Monto</th>';
    html += '<th>Referencia</th>';
    html += '<th>Complemento</th>';
    html += '</tr></thead><tbody>';

    pagos.forEach((pago, index) => {
        const fecha = moment(pago.FechaPago).format('DD/MM/YYYY HH:mm');
        const badgeComplemento = pago.ComplementoPagoID 
            ? `<span class="badge bg-success">Timbrado</span>`
            : `<span class="badge bg-warning text-dark">Pendiente</span>`;

        html += `<tr>`;
        html += `<td>${index + 1}</td>`;
        html += `<td>${fecha}</td>`;
        html += `<td>${pago.FormaPago || 'N/A'}</td>`;
        html += `<td>${pago.MetodoPago || 'N/A'}</td>`;
        html += `<td class="text-success fw-bold">$${pago.Monto.toFixed(2)}</td>`;
        html += `<td>${pago.Referencia || '-'}</td>`;
        html += `<td>${badgeComplemento}</td>`;
        html += `</tr>`;
    });

    html += '</tbody></table></div>';
    $('#listaHistorialPagos').html(html);
}

// ============================================================================
// CATÁLOGOS
// ============================================================================
function cargarFormasPago() {
    $.get('/Pagos/ObtenerFormasPago', function (res) {
        if (res.success) {
            let opciones = '<option value="">Seleccionar...</option>';
            res.data.forEach(forma => {
                opciones += `<option value="${forma.FormaPagoID}">${forma.Clave} - ${forma.Descripcion}</option>`;
            });
            $('#cboFormaPagoPago').html(opciones);
        }
    });
}

function cargarMetodosPago() {
    $.get('/Pagos/ObtenerMetodosPago', function (res) {
        if (res.success) {
            let opciones = '<option value="">Seleccionar...</option>';
            res.data.forEach(metodo => {
                opciones += `<option value="${metodo.MetodoPagoID}">${metodo.Clave} - ${metodo.Descripcion}</option>`;
            });
            $('#cboMetodoPagoPago').html(opciones);
        }
    });
}
