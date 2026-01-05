// ===============================================
// CONTROL DE CAJA - FUNCIONES PRINCIPALES
// ===============================================

const CAJA_ID = 1; // Caja por defecto
let estadoCajaActual = null;

// Inicializar estado de caja al cargar página
$(document).ready(function() {
    verificarEstadoCaja();
    // Actualizar estado cada 30 segundos
    setInterval(verificarEstadoCaja, 30000);
});

// Verificar estado actual de la caja
function verificarEstadoCaja() {
    $.ajax({
        url: '/VentaPOS/ObtenerEstadoCaja',
        type: 'GET',
        data: { cajaID: CAJA_ID },
        success: function(response) {
            console.log('Respuesta de ObtenerEstadoCaja:', response);
            if (response.success) {
                estadoCajaActual = response.data;
                console.log('Estado de caja actualizado:', estadoCajaActual);
                actualizarUICaja(response.data);
            } else {
                console.error('Error en respuesta:', response.mensaje);
            }
        },
        error: function(xhr, status, error) {
            console.error('Error al verificar estado de caja:', error, xhr.status);
        }
    });
}

// Actualizar interfaz según estado de caja
function actualizarUICaja(estado) {
    console.log('Actualizando UI de caja con estado:', estado);
    
    const panel = $('#estadoCajaPanel');
    const btnAbrir = $('#btnAbrirCaja');
    const btnCerrar = $('#btnCerrarCaja');
    const estadoTexto = $('#cajaEstado');
    const detalles = $('#cajaDetalles');

    if (estado.EstaAbierta) {
        // Caja ABIERTA
        panel.removeClass('caja-cerrada').addClass('caja-abierta');
        estadoTexto.text('ABIERTA').removeClass('text-danger').addClass('text-success');
        btnAbrir.hide();
        btnCerrar.show();
        detalles.show();

        // Actualizar detalles con todos los datos
        const fondoInicial = estado.FondoInicial || 0;
        const totalVentas = estado.TotalVentas || 0;
        const totalGastos = estado.TotalGastos || 0;
        const saldoEsperado = fondoInicial + totalVentas - totalGastos;
        
        console.log('Valores calculados:', {
            fondoInicial,
            totalVentas,
            totalGastos,
            saldoEsperado
        });
        
        $('#cajaFechaApertura').text(moment(estado.FechaApertura).format('DD/MM/YYYY HH:mm'));
        $('#cajaFondoInicial').text(fondoInicial.toFixed(2));
        $('#cajaNumVentas').text(estado.NumeroVentas);
        $('#cajaTotalVentas').text(totalVentas.toFixed(2));
        $('#cajaTotalGastos').text(totalGastos.toFixed(2));
        $('#cajaSaldoEsperado').text(saldoEsperado.toFixed(2));
        
        console.log('UI actualizada. FondoInicial:', fondoInicial, 'TotalGastos:', totalGastos, 'Esperado:', saldoEsperado);
    } else {
        // Caja CERRADA
        panel.removeClass('caja-abierta').addClass('caja-cerrada');
        estadoTexto.text('CERRADA').removeClass('text-success').addClass('text-danger');
        btnAbrir.show();
        btnCerrar.hide();
        detalles.hide();
        console.log('Caja cerrada');
    }
}

// Mostrar modal de apertura
function mostrarModalApertura() {
    $('#txtFondoInicial').val('');
    $('#modalAperturaCaja').modal('show');
}

// Procesar apertura de caja
function procesarApertura() {
    const fondoInicial = parseFloat($('#txtFondoInicial').val()) || 0;

    if (fondoInicial < 0) {
        toastr.error('El fondo inicial no puede ser negativo');
        return;
    }

    if (!confirm(`¿Confirma apertura de caja con fondo inicial de $${fondoInicial.toFixed(2)}?`)) {
        return;
    }

    $.ajax({
        url: '/VentaPOS/AperturaCaja',
        type: 'POST',
        data: { 
            cajaID: CAJA_ID,
            montoInicial: fondoInicial
        },
        success: function(response) {
            if (response.success) {
                toastr.success(response.mensaje);
                $('#modalAperturaCaja').modal('hide');
                verificarEstadoCaja();
            } else {
                toastr.error(response.mensaje);
            }
        },
        error: function() {
            toastr.error('Error al procesar apertura de caja');
        }
    });
}

// Mostrar modal de cierre
function mostrarModalCierre() {
    if (!estadoCajaActual || !estadoCajaActual.EstaAbierta) {
        toastr.error('La caja no está abierta');
        return;
    }

    // Llenar datos del resumen con valores reales
    const fondoInicial = estadoCajaActual.FondoInicial || 0;
    const totalVentas = estadoCajaActual.TotalVentas || 0;
    const totalGastos = estadoCajaActual.TotalGastos || 0;
    const montoEsperado = fondoInicial + totalVentas - totalGastos;
    
    $('#cierreFondoInicial').text(fondoInicial.toFixed(2));
    $('#cierreTotalVentas').text(totalVentas.toFixed(2));
    $('#cierreNumVentas').text(estadoCajaActual.NumeroVentas);
    $('#cierreTotalGastos').text(totalGastos.toFixed(2));
    $('#cierreMontoEsperado').text(montoEsperado.toFixed(2));

    // Limpiar campos
    $('#txtMontoEfectivo').val('');
    $('#txtMontoTarjeta').val('');
    $('#txtMontoTransferencia').val('');
    $('#txtObservaciones').val('');
    $('#cierreMontoReal').text('0.00');
    $('#alertDiferencia').hide();

    $('#modalCierreCaja').modal('show');
}

// Calcular diferencia en tiempo real
function calcularDiferencia() {
    const efectivo = parseFloat($('#txtMontoEfectivo').val()) || 0;
    const tarjeta = parseFloat($('#txtMontoTarjeta').val()) || 0;
    const transferencia = parseFloat($('#txtMontoTransferencia').val()) || 0;
    const montoEsperado = parseFloat($('#cierreMontoEsperado').text()) || 0;

    const montoReal = efectivo + tarjeta + transferencia;
    const diferencia = montoReal - montoEsperado;

    $('#cierreMontoReal').text(montoReal.toFixed(2));

    const alertDiv = $('#alertDiferencia');
    
    if (Math.abs(diferencia) < 0.01) {
        // CUADRADO
        alertDiv.removeClass('alert-danger alert-warning')
                .addClass('alert-success')
                .html('<i class="fas fa-check-circle"></i> <strong>Caja cuadrada</strong> - Sin diferencias')
                .show();
    } else if (diferencia < 0) {
        // FALTANTE
        alertDiv.removeClass('alert-success alert-warning')
                .addClass('alert-danger')
                .html(`<i class="fas fa-exclamation-triangle"></i> <strong>FALTANTE:</strong> $${Math.abs(diferencia).toFixed(2)}`)
                .show();
    } else {
        // SOBRANTE
        alertDiv.removeClass('alert-success alert-danger')
                .addClass('alert-warning')
                .html(`<i class="fas fa-info-circle"></i> <strong>SOBRANTE:</strong> $${diferencia.toFixed(2)}`)
                .show();
    }
}

// Procesar cierre de caja
function procesarCierre() {
    const efectivo = parseFloat($('#txtMontoEfectivo').val()) || 0;
    const tarjeta = parseFloat($('#txtMontoTarjeta').val()) || 0;
    const transferencia = parseFloat($('#txtMontoTransferencia').val()) || 0;
    const observaciones = $('#txtObservaciones').val();

    if (efectivo === 0 && tarjeta === 0 && transferencia === 0) {
        toastr.error('Debe ingresar al menos un monto para el arqueo');
        return;
    }

    if (!confirm('¿Confirma el cierre de caja? Esta acción no se puede deshacer.')) {
        return;
    }

    $.ajax({
        url: '/VentaPOS/CierreCajaCompleto',
        type: 'POST',
        data: {
            cajaID: CAJA_ID,
            montoEfectivo: efectivo,
            montoTarjeta: tarjeta,
            montoTransferencia: transferencia,
            observaciones: observaciones
        },
        success: function(response) {
            if (response.success) {
                const msg = response.mensaje + 
                    `<br><strong>Corte #${response.corteID}</strong>` +
                    `<br>Diferencia: $${Math.abs(response.diferencia).toFixed(2)}`;
                toastr.success(msg);
                $('#modalCierreCaja').modal('hide');
                verificarEstadoCaja();
            } else {
                toastr.error(response.mensaje);
            }
        },
        error: function() {
            toastr.error('Error al procesar cierre de caja');
        }
    });
}

// ===============================================
// VALIDACIÓN ANTES DE FINALIZAR VENTA
// ===============================================

// Reemplazar o modificar la función finalizarVenta original
const finalizarVentaOriginal = window.finalizarVenta;

window.finalizarVenta = function() {
    // Verificar que la caja esté abierta
    if (!estadoCajaActual || !estadoCajaActual.EstaAbierta) {
        toastr.error('No se puede procesar la venta. La caja está cerrada.', 'Apertura Requerida', {
            timeOut: 5000,
            closeButton: true
        });
        
        // Ofrecer abrir caja
        if (confirm('¿Desea abrir la caja ahora?')) {
            mostrarModalApertura();
        }
        return;
    }

    // Si la caja está abierta, proceder con la venta original
    if (finalizarVentaOriginal) {
        finalizarVentaOriginal();
    }
};

// Agregar CajaID al payload de venta
$(document).on('finalizarVenta:antes', function(e, payload) {
    if (payload && payload.Venta) {
        payload.Venta.CajaID = CAJA_ID;
    }
});

console.log('✅ Módulo Control de Caja cargado');
