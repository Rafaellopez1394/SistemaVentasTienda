// Version: 20260126-134036
// VentasWeb/Scripts/Views/VentaPOS.js
// ============================================================================
// VARIABLES GLOBALES
// ============================================================================
let carrito = [];
let clienteActual = null;
let catalogos = {};
const CLIENTE_GENERAL_ID = '00000000-0000-0000-0000-000000000000';

// Configuración básica para códigos de barras de balanza (EAN-13)
// Torrey suele usar EAN-13 con prefijos configurables (común: '27' para peso)
// Dígitos: 3-7 PLU y 8-12 peso (varía por configuración de la balanza)
const SCALE_BARCODE = {
    prefixWeightList: ['27'],      // Ajustar según Torrey (puede ser '26'-'29' según modo)
    productDigits: [2, 7],         // índice inicial (incl) y final (excl) del código producto (PLU)
    weightDigits: [7, 12],         // índice inicial (incl) y final (excl) del peso
    weightMultiplier: 1            // 1 si los dígitos representan gramos directamente
};

// ============================================================================
// INICIALIZACIÓN
// ============================================================================
$(document).ready(function () {
    cargarCatalogos();
    configurarEventos();
    
    // Cliente General por defecto
    $('#clienteID').val(CLIENTE_GENERAL_ID);
    $('#clienteGeneral').show();
    $('#clienteInfo').hide();
});

function configurarEventos() {
    // Enter en búsqueda de producto
    $('#txtBuscarProducto').on('keypress', function (e) {
        if (e.which === 13) {
            buscarProducto();
        }
    });
    // Click en botón buscar
    $('#btnBuscarProducto').on('click', function(){
        window.buscarProducto();
    });

    // Enter en búsqueda de cliente
    $('#txtBuscarCliente').on('keypress', function (e) {
        if (e.which === 13) {
            buscarCliente();
        }
    });

    // Enter en efectivo recibido
    $('#txtEfectivoRecibido').on('keypress', function (e) {
        if (e.which === 13) {
            finalizarVenta();
        }
    });
}

// ============================================================================
// CATÁLOGOS
// ============================================================================
function cargarCatalogos() {
    $.get('/VentaPOS/ObtenerCatalogos', function (res) {
        if (res.success) {
            catalogos = res;

            // Formas de pago
            const $forma = $('#cboFormaPago').empty().append('<option value="">Seleccionar...</option>');
            res.formasPago.forEach(fp => {
                $forma.append(`<option value="${fp.FormaPagoID}" data-cambio="${fp.RequiereCambio}">${fp.Descripcion}</option>`);
            });
            // Seleccionar "Efectivo" por defecto (normalmente es el primer ID o buscar por descripción)
            const efectivoOption = res.formasPago.find(fp => fp.Descripcion.toLowerCase().includes('efectivo'));
            if (efectivoOption) {
                $forma.val(efectivoOption.FormaPagoID);
                cambiarFormaPago(); // Mostrar panel de efectivo
            }

            // Métodos de pago
            const $metodo = $('#cboMetodoPago').empty().append('<option value="">Seleccionar...</option>');
            res.metodosPago.forEach(mp => {
                $metodo.append(`<option value="${mp.MetodoPagoID}">${mp.Descripcion}</option>`);
            });
            // Seleccionar "PUE - Pago en una sola exhibición" por defecto
            const pueOption = res.metodosPago.find(mp => mp.Descripcion.includes('PUE') || mp.Descripcion.toLowerCase().includes('una') || mp.Descripcion.toLowerCase().includes('exhibici'));
            if (pueOption) {
                $metodo.val(pueOption.MetodoPagoID);
            }

            // Usos CFDI
            const $uso = $('#cboUsoCFDI').empty().append('<option value="">Seleccionar...</option>');
            res.usosCFDI.forEach(u => {
                $uso.append(`<option value="${u.Value}">${u.Value} - ${u.Text}</option>`);
            });
            } else {
                toastr.error('Error al cargar catálogos');
            }
        });
}

// ============================================================================
// BÚSQUEDA DE PRODUCTOS
// ============================================================================
function buscarProducto() {
    const texto = $('#txtBuscarProducto').val().trim();
    if (!texto) {
        $('#resultadosBusqueda').empty();
        return;
    }

    // Decodificación de código de balanza: intenta extraer producto + gramos o precio
    if (/^\d{13}$/.test(texto) && SCALE_BARCODE.prefixWeightList.some(p => texto.startsWith(p))) {
        const parsed = decodeScaleBarcode(texto);
        if (parsed) {
            // Buscar por código interno PLU
            $.get('/VentaPOS/BuscarProducto', { texto: parsed.productCode }, function (res) {
                if (res.success && res.data.length > 0) {
                    const producto = res.data.find(p => (p.CodigoInterno || '') === parsed.productCode) || res.data[0];
                    if (producto.VentaPorGramaje && producto.PrecioPorKilo) {
                        if (parsed.grams && parsed.grams > 0) {
                            agregarPorGramajeAutomatico(producto, parsed.grams);
                            $('#txtBuscarProducto').val('').focus();
                            $('#resultadosBusqueda').empty();
                            return;
                        } else if (parsed.totalPrice && parsed.totalPrice > 0) {
                            const gramos = Math.round((parsed.totalPrice / (producto.PrecioPorKilo / 1000)));
                            agregarPorGramajeAutomatico(producto, gramos);
                            $('#txtBuscarProducto').val('').focus();
                            $('#resultadosBusqueda').empty();
                            return;
                        } else {
                            mostrarModalGramaje(producto);
                            $('#txtBuscarProducto').val('').focus();
                            $('#resultadosBusqueda').empty();
                            return;
                        }
                    }
                }
                continueStandardSearch(texto);
            });
            return; // evitar doble request
        }
    }

    continueStandardSearch(texto);
}

// Exponer función para uso desde atributos onclick en la vista
window.buscarProducto = buscarProducto;
window.finalizarVenta = finalizarVenta;
window.cancelarVenta = cancelarVenta;
window.limpiarCarrito = limpiarCarrito;

function continueStandardSearch(texto) {
    $.get('/VentaPOS/BuscarProducto', { texto: texto }, function (res) {
        if (res.success && res.data.length > 0) {
            let html = '';
            res.data.forEach(p => {
                const stock = p.StockDisponible > 0 ? `<span class="badge bg-success">${p.StockDisponible} disponibles</span>` : '<span class="badge bg-danger">Sin stock</span>';
                html += `
                    <div class="search-result-item" onclick='agregarAlCarrito(${JSON.stringify(p)})'>
                        <div class="d-flex justify-content-between">
                            <div>
                                <strong>${p.Nombre}</strong>
                                <br><small class="text-muted">${p.CodigoInterno || 'Sin código'}</small>
                            </div>
                            <div class="text-end">
                                <strong class="text-success">$${p.PrecioVenta.toFixed(2)}</strong>
                                <br>${stock}
                            </div>
                        </div>
                    </div>
                `;
            });
            $('#resultadosBusqueda').html(html);
        } else {
            $('#resultadosBusqueda').html('<p class="text-center text-muted p-3">No se encontraron productos</p>');
        }
    });
}

function decodeScaleBarcode(code) {
    try {
        const pStart = SCALE_BARCODE.productDigits[0];
        const pEnd = SCALE_BARCODE.productDigits[1];
        const wStart = SCALE_BARCODE.weightDigits[0];
        const wEnd = SCALE_BARCODE.weightDigits[1];
        const productCode = code.substring(pStart, pEnd);
        const digitsRaw = code.substring(wStart, wEnd);
        if (SCALE_BARCODE.mode === 'weight' || SCALE_BARCODE.mode === 'auto') {
            const grams = parseInt(digitsRaw, 10) * SCALE_BARCODE.weightMultiplier;
            if (productCode && !isNaN(grams) && grams > 0) return { productCode, grams };
        }
        if (SCALE_BARCODE.mode === 'price' || SCALE_BARCODE.mode === 'auto') {
            const cents = parseInt(digitsRaw, 10);
            const totalPrice = (isNaN(cents) ? NaN : (cents * SCALE_BARCODE.priceMultiplier));
            if (productCode && !isNaN(totalPrice) && totalPrice > 0) return { productCode, totalPrice };
        }
        return null;
    } catch (e) {
        return null;
    }
}

function agregarPorGramajeAutomatico(producto, gramos) {
    if (!gramos || gramos <= 0) {
        mostrarToast('Cantidad de gramos inválida', 'warning');
        return;
    }
    const kilogramos = gramos / 1000;
    const precioCalculado = (producto.PrecioPorKilo / 1000) * gramos;
    carrito.push({
        ProductoID: producto.ProductoID,
        LoteID: producto.LoteID,
        Nombre: producto.Nombre,
        CodigoInterno: producto.CodigoInterno,
        Cantidad: 1,
        PrecioVenta: precioCalculado,
        PrecioCompra: producto.PrecioCompra,
        TasaIVA: producto.TasaIVA || 0,
        StockDisponible: producto.StockDisponible,
        VentaPorGramaje: true,
        Gramos: gramos,
        Kilogramos: kilogramos,
        PrecioPorKilo: producto.PrecioPorKilo,
        PrecioCalculado: precioCalculado
    });
    actualizarCarrito();
    toastr.success(`${gramos}g de ${producto.Nombre} agregado al carrito`);
}

// (Nota) `decodeWeightBarcode` se reemplazó por `decodeScaleBarcode` arriba

// ============================================================================
// CARRITO DE COMPRA
// ============================================================================
function agregarAlCarrito(producto) {
    if (producto.StockDisponible <= 0) {
        toastr.warning('Producto sin stock disponible');
        return;
    }

    if (!producto.LoteID || producto.LoteID === 0) {
        toastr.error('No hay lotes disponibles para este producto');
        return;
    }

    // Verificar si el producto ya está en el carrito
    const itemExistente = carrito.find(item => item.ProductoID === producto.ProductoID);
    
    if (itemExistente) {
        if (itemExistente.Cantidad < producto.StockDisponible) {
            itemExistente.Cantidad++;
        } else {
            toastr.warning('No hay más stock disponible');
            return;
        }
    } else {
        carrito.push({
            ProductoID: producto.ProductoID,
            LoteID: producto.LoteID,
            Nombre: producto.Nombre,
            CodigoInterno: producto.CodigoInterno,
            Cantidad: 1,
            PrecioVenta: producto.PrecioVenta,
            PrecioCompra: producto.PrecioCompra,
            TasaIVA: producto.TasaIVA || 0,
            StockDisponible: producto.StockDisponible
        });
    }

    actualizarCarrito();
    $('#txtBuscarProducto').val('').focus();
    $('#resultadosBusqueda').empty();
}

function actualizarCarrito() {
    if (carrito.length === 0) {
        $('#carritoItems').html('<p class="text-center text-muted">El carrito está vacío</p>');
        actualizarTotales();
        return;
    }

    let html = '';
    carrito.forEach((item, index) => {
        const subtotal = item.Cantidad * item.PrecioVenta;
        const montoIVA = subtotal * (item.TasaIVA / 100);
        const total = subtotal + montoIVA;
        
        // Formatear cantidad: mostrar hasta 3 decimales si es necesario
        const cantidadFormateada = item.Cantidad % 1 === 0 ? item.Cantidad : item.Cantidad.toFixed(3).replace(/\.?0+$/, '');
        const unidad = item.Cantidad !== Math.floor(item.Cantidad) ? ' kg' : '';

        html += `
            <div class="carrito-item">
                <div class="row align-items-center">
                    <div class="col-5">
                        <strong>${item.Nombre}</strong>
                        <br><small class="text-muted">${item.CodigoInterno || ''}</small>
                        <br><small class="text-info">IVA ${item.TasaIVA}%</small>
                        ${unidad ? '<br><small class="text-warning"><i class="fas fa-weight"></i> Peso variable</small>' : ''}
                    </div>
                    <div class="col-3">
                        <div class="input-group input-group-sm">
                            <button class="btn btn-outline-secondary" onclick="cambiarCantidad(${index}, -1)">-</button>
                            <input type="number" class="form-control cantidad-input" value="${item.Cantidad}" 
                                   onchange="cambiarCantidadDirecta(${index}, this.value)" min="0.001" max="${item.StockDisponible}" step="0.001" />
                            <button class="btn btn-outline-secondary" onclick="cambiarCantidad(${index}, 1)">+</button>
                        </div>
                        <small class="text-muted">$${item.PrecioVenta.toFixed(2)}${unidad ? '/kg' : ' c/u'}</small>
                    </div>
                    <div class="col-3 text-end">
                        <strong class="text-success">$${total.toFixed(2)}</strong>
                    </div>
                    <div class="col-1">
                        <button class="btn btn-sm btn-danger" onclick="eliminarDelCarrito(${index})">
                            <i class="fas fa-trash"></i>
                        </button>
                    </div>
                </div>
            </div>
        `;
    });

    $('#carritoItems').html(html);
    actualizarTotales();
}

function cambiarCantidad(index, delta) {
    const item = carrito[index];
    // Permitir incrementos/decrementos de 0.1 para productos fraccionados
    const incremento = delta > 0 ? 0.1 : -0.1;
    const nuevaCantidad = Math.round((item.Cantidad + incremento) * 1000) / 1000;

    if (nuevaCantidad <= 0) {
        eliminarDelCarrito(index);
        return;
    }

    if (nuevaCantidad > item.StockDisponible) {
        toastr.warning('No hay más stock disponible');
        return;
    }

    item.Cantidad = nuevaCantidad;
    actualizarCarrito();
}

function cambiarCantidadDirecta(index, valor) {
    const cantidad = parseFloat(valor);
    const item = carrito[index];

    if (isNaN(cantidad) || cantidad <= 0) {
        toastr.warning('Cantidad no válida');
        actualizarCarrito();
        return;
    }

    // Redondear a 3 decimales para evitar problemas de precisión
    const cantidadRedondeada = Math.round(cantidad * 1000) / 1000;

    if (cantidadRedondeada > item.StockDisponible) {
        toastr.warning('Cantidad mayor al stock disponible');
        actualizarCarrito();
        return;
    }

    item.Cantidad = cantidadRedondeada;
    actualizarCarrito();
}

function eliminarDelCarrito(index) {
    carrito.splice(index, 1);
    actualizarCarrito();
}

function limpiarCarrito() {
    if (carrito.length === 0) return;

    if (confirm('¿Está seguro de limpiar el carrito?')) {
        carrito = [];
        actualizarCarrito();
        toastr.info('Carrito limpiado');
    }
}

// ============================================================================
// CÁLCULO DE TOTALES
// ============================================================================
function actualizarTotales() {
    let subtotal = 0;
    let totalIVA = 0;

    carrito.forEach(item => {
        const itemSubtotal = item.Cantidad * item.PrecioVenta;
        const itemIVA = itemSubtotal * (item.TasaIVA / 100);
        
        subtotal += itemSubtotal;
        totalIVA += itemIVA;
    });

    const total = subtotal + totalIVA;

    $('#lblSubtotal').text('$' + subtotal.toFixed(2));
    $('#lblIVA').text('$' + totalIVA.toFixed(2));
    $('#lblTotal').text('$' + total.toFixed(2));
    $('#lblTotalPagar').text('$' + total.toFixed(2));

    return { subtotal, iva: totalIVA, total };
}

// ============================================================================
// BÚSQUEDA DE CLIENTES
// ============================================================================
function buscarCliente() {
    const texto = $('#txtBuscarCliente').val().trim();
    if (!texto) {
        $('#resultadosCliente').empty();
        return;
    }

    $.get('/VentaPOS/BuscarCliente', { texto: texto }, function (res) {
        if (res.success && res.data.length > 0) {
            let html = '';
            res.data.forEach(c => {
                html += `
                    <div class="search-result-item" onclick="seleccionarCliente('${c.ClienteID}')">
                        <strong>${c.RazonSocial}</strong>
                        <br><small class="text-muted">RFC: ${c.RFC}</small>
                    </div>
                `;
            });
            $('#resultadosCliente').html(html);
        } else {
            $('#resultadosCliente').html('<p class="text-center text-muted p-2">No se encontraron clientes</p>');
        }
    });
}

function seleccionarCliente(clienteID) {
    $.get('/VentaPOS/ObtenerCliente', { id: clienteID }, function (res) {
        if (res.success) {
            clienteActual = res;
            $('#clienteID').val(clienteID);
            
            $('#clienteNombre').text(res.cliente.RazonSocial);
            $('#clienteRFC').text('RFC: ' + res.cliente.RFC);
            $('#txtFacturaRFC').val(res.cliente.RFC);

            // Mostrar información de crédito
            if (res.limiteTotal > 0) {
                const disponiblePct = (res.creditoDisponible / res.limiteTotal) * 100;
                const claseCredito = disponiblePct > 50 ? 'credito-success' : 'credito-warning';
                
                $('#clienteCredito').html(`
                    <div class="${claseCredito}">
                        <small><strong>Crédito:</strong></small><br>
                        <small>Límite: $${res.limiteTotal.toFixed(2)}</small><br>
                        <small>Disponible: $${res.creditoDisponible.toFixed(2)}</small><br>
                        <small>Utilizado: $${res.saldoActual.toFixed(2)}</small>
                    </div>
                `);

                // Habilitar opción de crédito y pago parcial
                $('#rdCredito').prop('disabled', false);
                $('#rdParcial').prop('disabled', false);
            } else {
                $('#clienteCredito').html('<small class="text-muted">Sin línea de crédito</small>');
                $('#rdCredito').prop('disabled', true);
                $('#rdParcial').prop('disabled', false); // Pago parcial disponible aunque no tenga crédito
                $('#rdContado').prop('checked', true);
            }

            $('#clienteGeneral').hide();
            $('#clienteInfo').show();

            const tipoVenta = $('input[name="tipoVenta"]:checked').val();
            if (tipoVenta === 'CONTADO') {
                $('#panelFacturacion').show();
            }
            $('#resultadosCliente').empty();
            $('#txtBuscarCliente').val('');
        } else {
            toastr.error(res.mensaje || 'Error al cargar cliente');
        }
    });
}

function seleccionarClienteGeneral() {
    clienteActual = null;
    $('#clienteID').val(CLIENTE_GENERAL_ID);
    $('#clienteGeneral').show();
    $('#clienteInfo').hide();
    $('#panelFacturacion').hide();
    $('#rdCredito').prop('disabled', true);
    $('#rdParcial').prop('disabled', true);
    $('#rdContado').prop('checked', true);
    cambiarTipoVenta();
}

// ============================================================================
// TIPO DE VENTA
// ============================================================================
function cambiarTipoVenta() {
    const tipoVenta = $('input[name="tipoVenta"]:checked').val();
    
    if (tipoVenta === 'CONTADO') {
        $('#panelFormaPago').show();
        $('#panelFacturacion').show();
        $('#panelMontoParcial').hide();
    } else if (tipoVenta === 'PARCIAL') {
        // Pago parcial: mostrar panel de monto pagado, forma de pago obligatoria
        $('#panelFormaPago').show();
        $('#panelFacturacion').show();
        $('#panelMontoParcial').show();
        $('#panelEfectivo').hide();
        $('#panelCambio').hide();
        
        // Actualizar total en panel parcial
        const totales = actualizarTotales();
        $('#lblTotalVentaParcial').text('$' + totales.total.toFixed(2));
        $('#txtMontoPagado').val('');
        $('#panelSaldoPendiente').hide();
        
        // Factura siempre requerida en PARCIAL con PPD
        $('#chkRequiereFactura').prop('checked', true);
        $('#datosFacturacion').show();
    } else {
        // Crédito: ocultar pago y facturación
        $('#panelFormaPago').hide();
        $('#panelEfectivo').hide();
        $('#panelCambio').hide();
        $('#panelFacturacion').hide();
        $('#panelMontoParcial').hide();
        $('#chkRequiereFactura').prop('checked', false);
        $('#datosFacturacion').hide();
    }
}

// ============================================================================
// FORMA DE PAGO
// ============================================================================
function cambiarFormaPago() {
    const formaPagoID = $('#cboFormaPago').val();
    const option = $('#cboFormaPago option:selected');
    const requiereCambio = option.data('cambio');

    if (requiereCambio) {
        $('#panelEfectivo').show();
        $('#txtEfectivoRecibido').focus();
    } else {
        $('#panelEfectivo').hide();
        $('#panelCambio').hide();
        $('#txtEfectivoRecibido').val('');
    }
}

function calcularCambio() {
    const totales = actualizarTotales();
    const efectivo = parseFloat($('#txtEfectivoRecibido').val()) || 0;
    const cambio = efectivo - totales.total;

    if (efectivo >= totales.total) {
        $('#lblCambio').text('$' + cambio.toFixed(2));
        $('#panelCambio').show();
    } else {
        $('#panelCambio').hide();
    }
}

function calcularSaldoPendiente() {
    const totales = actualizarTotales();
    const montoPagado = parseFloat($('#txtMontoPagado').val()) || 0;
    const saldoPendiente = totales.total - montoPagado;
    const porcentaje = totales.total > 0 ? ((montoPagado / totales.total) * 100).toFixed(1) : 0;
    
    if (montoPagado > 0) {
        $('#lblSaldoPendiente').text('$' + saldoPendiente.toFixed(2));
        $('#lblPorcentajePagado').text(porcentaje + '%');
        $('#panelSaldoPendiente').show();
        
        // Cambiar color según el saldo
        if (saldoPendiente <= 0.01) {
            $('#lblSaldoPendiente').removeClass('text-danger').addClass('text-success');
        } else {
            $('#lblSaldoPendiente').removeClass('text-success').addClass('text-danger');
        }
    } else {
        $('#panelSaldoPendiente').hide();
    }
}

// ============================================================================
// FACTURACIÓN
// ============================================================================
function toggleFacturacion() {
    if ($('#chkRequiereFactura').is(':checked')) {
        $('#datosFacturacion').show();
    } else {
        $('#datosFacturacion').hide();
    }
}

// ============================================================================
// FINALIZAR VENTA
// ============================================================================
function finalizarVenta() {
    // Validar carrito
    if (carrito.length === 0) {
        toastr.warning('Agregue productos al carrito');
        return;
    }

    // Obtener tipo de venta
    const tipoVenta = $('input[name="tipoVenta"]:checked').val();
    const totales = actualizarTotales();
    const clienteID = $('#clienteID').val();

    // Validar cliente
    if (!clienteID) {
        toastr.warning('Seleccione un cliente');
        return;
    }

    // Validar crédito si aplica
    if (tipoVenta === 'CREDITO') {
        $.post('/VentaPOS/ValidarCredito', {
            clienteID: clienteID,
            montoVenta: totales.total
        }, function (res) {
            if (res.success && res.validacion.TieneCredito) {
                procesarVenta(tipoVenta, totales, clienteID);
            } else {
                toastr.error(res.validacion.Mensaje || 'El cliente no tiene crédito suficiente');
            }
        });
        return;
    }

    // Validar forma de pago
    const formaPagoID = $('#cboFormaPago').val();
    if (!formaPagoID) {
        toastr.warning('Seleccione una forma de pago');
        return;
    }

    // Validaciones específicas para PARCIAL
    if (tipoVenta === 'PARCIAL') {
        const montoPagado = parseFloat($('#txtMontoPagado').val()) || 0;
        
        if (montoPagado <= 0) {
            toastr.warning('Ingrese el monto a pagar ahora');
            $('#txtMontoPagado').focus();
            return;
        }
        
        if (montoPagado > totales.total) {
            toastr.warning('El monto pagado no puede ser mayor al total de la venta');
            $('#txtMontoPagado').focus();
            return;
        }
        
        // Confirmar pago parcial
        const saldoPendiente = totales.total - montoPagado;
        if (!confirm(`¿Confirmar pago parcial?\n\nTotal: $${totales.total.toFixed(2)}\nPago ahora: $${montoPagado.toFixed(2)}\nSaldo pendiente: $${saldoPendiente.toFixed(2)}`)) {
            return;
        }
    }

    // Validar efectivo recibido si es necesario
    const option = $('#cboFormaPago option:selected');
    const requiereCambio = option.data('cambio');
    
    if (requiereCambio && tipoVenta === 'CONTADO') {
        const efectivo = parseFloat($('#txtEfectivoRecibido').val()) || 0;
        if (efectivo < totales.total) {
            toastr.warning('El efectivo recibido es menor al total');
            $('#txtEfectivoRecibido').focus();
            return;
        }
    }

    procesarVenta(tipoVenta, totales, clienteID);
}

function procesarVenta(tipoVenta, totales, clienteID) {
    const formaPagoID = $('#cboFormaPago').val() || null;
    const metodoPagoID = $('#cboMetodoPago').val() || null;
    const efectivoRecibido = parseFloat($('#txtEfectivoRecibido').val()) || null;
    const cambio = efectivoRecibido ? efectivoRecibido - totales.total : null;

    const requiereFactura = (tipoVenta === 'CONTADO' && $('#chkRequiereFactura').is(':checked')) || tipoVenta === 'PARCIAL';
    
    // Capturar MontoPagado para ventas parciales
    let montoPagado = null;
    if (tipoVenta === 'PARCIAL') {
        montoPagado = parseFloat($('#txtMontoPagado').val()) || null;
    }

    const detalle = carrito.map(item => ({
        ProductoID: item.ProductoID,
        LoteID: item.LoteID,
        Nombre: item.Nombre,
        Cantidad: item.Cantidad,
        PrecioVenta: item.PrecioVenta,
        PrecioCompra: item.PrecioCompra,
        TasaIVA: item.TasaIVA,
        MontoIVA: (item.Cantidad * item.PrecioVenta) * (item.TasaIVA / 100),
        // Campos de gramaje si existen
        Gramos: typeof item.Gramos !== 'undefined' ? item.Gramos : undefined,
        Kilogramos: typeof item.Kilogramos !== 'undefined' ? item.Kilogramos : undefined,
        PrecioPorKilo: typeof item.PrecioPorKilo !== 'undefined' ? item.PrecioPorKilo : undefined
    }));

    const payload = {
        Venta: {
            ClienteID: clienteID,
            TipoVenta: tipoVenta,
            FormaPagoID: formaPagoID,
            MetodoPagoID: metodoPagoID,
            EfectivoRecibido: efectivoRecibido,
            Cambio: cambio,
            Subtotal: totales.subtotal,
            IVA: totales.iva,
            Total: totales.total,
            MontoPagado: montoPagado,
            RequiereFactura: requiereFactura,
            CajaID: 1
        },
        Detalle: detalle
    };

    const $btnFinalizar = $('button:contains("FINALIZAR VENTA")');
    $btnFinalizar.prop('disabled', true).html('<i class="fas fa-spinner fa-spin"></i> Procesando...');

    $.ajax({
        url: '/VentaPOS/RegistrarVenta',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(payload),
        success: function (res) {
            if (res.success) {
                toastr.success(res.mensaje);
                
                // Generar e imprimir ticket
                generarTicketVenta(res.ventaId, detalle, totales, efectivoRecibido, cambio);
                
                if (cambio && cambio > 0) {
                    setTimeout(() => {
                        alert(`CAMBIO: $${cambio.toFixed(2)}`);
                    }, 500);
                }

                // Si requiere factura, redirigir a la página de facturación
                if (requiereFactura) {
                    setTimeout(() => {
                        if (confirm('¿Desea generar la factura ahora?\n\nVentaID: ' + res.ventaId)) {
                            window.location.href = '/Factura/Index?ventaId=' + res.ventaId;
                        } else {
                            limpiarFormulario();
                        }
                    }, 1000);
                } else {
                    limpiarFormulario();
                }
            } else {
                toastr.error(res.mensaje || 'Error al registrar la venta');
            }
        },
        error: function () {
            toastr.error('Error de conexion con el servidor');
        },
        complete: function () {
            $btnFinalizar.prop('disabled', false).html('<i class="fas fa-check-circle"></i> FINALIZAR VENTA');
        }
    });
}

// ============================================================================
// LIMPIAR FORMULARIO
// ============================================================================
function limpiarFormulario() {
    carrito = [];
    actualizarCarrito();

    seleccionarClienteGeneral();

    // Restaurar forma de pago a EFECTIVO por defecto
    const efectivoOption = catalogos.formasPago.find(fp => fp.Descripcion.toLowerCase().includes('efectivo'));
    if (efectivoOption) {
        $('#cboFormaPago').val(efectivoOption.FormaPagoID);
        cambiarFormaPago(); // Mostrar panel de efectivo
    } else {
        $('#cboFormaPago').val('');
        $('#panelEfectivo').hide();
        $('#panelCambio').hide();
    }
    
    // Restaurar método de pago a PUE por defecto
    const pueOption = catalogos.metodosPago.find(mp => mp.Descripcion.includes('PUE') || mp.Descripcion.toLowerCase().includes('una') || mp.Descripcion.toLowerCase().includes('exhibici'));
    if (pueOption) {
        $('#cboMetodoPago').val(pueOption.MetodoPagoID);
    } else {
        $('#cboMetodoPago').val('');
    }
    
    $('#txtEfectivoRecibido').val('');

    $('#chkRequiereFactura').prop('checked', false);
    $('#datosFacturacion').hide();
    $('#cboUsoCFDI').val('');
    $('#txtCorreoFactura').val('');

    $('#rdContado').prop('checked', true);
    cambiarTipoVenta();

    $('#txtBuscarProducto').focus();
}

// ============================================================================
// CANCELAR VENTA
// ============================================================================
function cancelarVenta() {
    if (carrito.length > 0) {
        if (!confirm('¿Está seguro de cancelar esta venta?')) {
            return;
        }
    }
    limpiarFormulario();
}

// ============================================================================
// UTILIDADES
// ============================================================================
function mostrarToast(mensaje, tipo = 'info') {
    toastr[tipo](mensaje);
}

// ============================================================================
// GENERACION DE TICKET DE VENTA
// ============================================================================
function generarTicketVenta(ventaId, productos, totales, efectivo, cambio) {
    // Obtener configuración de impresora para ancho de papel
    $.get('/Configuracion/ObtenerImpresoraTickets', function (resImpresora) {
        var anchoPapel = (resImpresora.success && resImpresora.data) ? resImpresora.data.AnchoPapel : 58;
        
        // Obtener datos del negocio
        $.get('/Configuracion/ObtenerDatosNegocio', function (res) {
        var datosNegocio = res.success ? res.data : {
            NombreNegocio: 'MI TIENDA',
            RFC: '',
            Direccion: '',
            Telefono: '',
            LogoPath: '',
            MostrarLogoEnTicket: false,
            MensajeTicket: 'Gracias por su compra',
            ImprimirTicketAutomatico: false
        };
        
        var clienteNombre = clienteActual ? clienteActual.RazonSocial : 'PUBLICO GENERAL';
        
        var datosTicket = {
            negocio: datosNegocio.NombreNegocio,
            rfc: datosNegocio.RFC,
            direccion: datosNegocio.Direccion,
            telefono: datosNegocio.Telefono,
            logo: datosNegocio.LogoPath,
            mostrarLogo: datosNegocio.MostrarLogoEnTicket,
            anchoPapel: anchoPapel,
            folio: ventaId ? ventaId.substring(0, 8).toUpperCase() : 'N/A',
            fecha: new Date().toLocaleString(),
            cliente: clienteNombre,
            productos: productos.map(p => ({
                nombre: p.Nombre || p.nombre || 'Producto',
                cantidad: p.Cantidad,
                precio: p.PrecioVenta,
                subtotal: p.Cantidad * p.PrecioVenta
            })),
            subtotal: totales.subtotal,
            iva: totales.iva,
            total: totales.total,
            efectivo: efectivo || 0,
            cambio: cambio || 0,
            mensaje: datosNegocio.MensajeTicket
        };
        
        // Imprimir directamente al finalizar la venta (pasar datosTicket)
        imprimirTicket(null, datosTicket);
        
        }).fail(function() {
            // Si falla obtener datos del negocio, usar valores por defecto
            var clienteNombre = clienteActual ? clienteActual.RazonSocial : 'PUBLICO GENERAL';
            
            var datosTicket = {
                negocio: 'MI TIENDA',
                rfc: '',
                direccion: '',
                telefono: '',
                anchoPapel: anchoPapel,
                folio: ventaId ? ventaId.substring(0, 8).toUpperCase() : 'N/A',
                fecha: new Date().toLocaleString(),
                cliente: clienteNombre,
                productos: productos.map(p => ({
                    nombre: p.Nombre || p.nombre || 'Producto',
                    cantidad: p.Cantidad,
                    precio: p.PrecioVenta,
                    subtotal: p.Cantidad * p.PrecioVenta
                })),
                subtotal: totales.subtotal,
                iva: totales.iva,
                total: totales.total,
                efectivo: efectivo || 0,
                cambio: cambio || 0,
                mensaje: 'Gracias por su compra'
            };
            
            imprimirTicket(null, datosTicket);
        });
    }).fail(function() {
        // Si falla obtener configuración de impresora, usar ancho por defecto
        var anchoPapel = 58;
        var clienteNombre = clienteActual ? clienteActual.RazonSocial : 'PUBLICO GENERAL';
        
        var datosTicket = {
            negocio: 'MI TIENDA',
            anchoPapel: anchoPapel,
            folio: ventaId ? ventaId.substring(0, 8).toUpperCase() : 'N/A',
            fecha: new Date().toLocaleString(),
            cliente: clienteNombre,
            productos: productos.map(p => ({
                nombre: p.Nombre || p.nombre || 'Producto',
                cantidad: p.Cantidad,
                precio: p.PrecioVenta,
                subtotal: p.Cantidad * p.PrecioVenta
            })),
            subtotal: totales.subtotal,
            iva: totales.iva,
            total: totales.total,
            efectivo: efectivo || 0,
            cambio: cambio || 0,
            mensaje: 'Gracias por su compra'
        };
        
        // Imprimir directamente (pasar datosTicket)
        imprimirTicket(null, datosTicket);
    });
}

function generarTicketHTML(datos, esCopia) {
    var html = '';
    
    // ===== LOGO (si existe y está configurado) =====
    if (datos.mostrarLogo && datos.logo && datos.logo.trim()) {
        // Agregar logo como imagen - el backend lo procesará
        html += '<div style="text-align:center;">';
        html += '<img src="' + datos.logo + '" style="max-width:150px;max-height:80px;" />';
        html += '</div>';
    }
    
    var texto = '';
    // Ancho fijo de 32 caracteres para impresoras de 58mm
    var ancho = 32;
    
    // Función auxiliar para centrar texto
    function centrar(str) {
        if (!str) return '';
        str = String(str);
        var espacios = Math.max(0, Math.floor((ancho - str.length) / 2));
        return ' '.repeat(espacios) + str;
    }
    
    // Función para formatear moneda
    function formatoMoneda(num) {
        return '$' + parseFloat(num || 0).toFixed(2);
    }
    
    // Función auxiliar para dos columnas (etiqueta a la izquierda, valor a la derecha)
    function dosColumnas(etiqueta, valor) {
        etiqueta = String(etiqueta || '');
        valor = String(valor || '');
        
        // Limpiar espacios
        etiqueta = etiqueta.trim();
        valor = valor.trim();
        
        // DEBUG: Asegurar que el valor está completo
        console.log('dosColumnas - Etiqueta: "' + etiqueta + '", Valor: "' + valor + '"');
        
        // Si la línea completa es más larga que el ancho
        var longitudTotal = etiqueta.length + valor.length;
        
        if (longitudTotal >= ancho) {
            // Truncar etiqueta para que el valor SIEMPRE se vea completo
            var maxEtiqueta = ancho - valor.length - 1;
            if (maxEtiqueta > 0 && etiqueta.length > maxEtiqueta) {
                etiqueta = etiqueta.substring(0, maxEtiqueta);
            }
        }
        
        // Calcular espacios de relleno
        var espacios = ancho - etiqueta.length - valor.length;
        if (espacios < 1) espacios = 1;
        
        var resultado = etiqueta + ' '.repeat(espacios) + valor;
        
        // DEBUG: Verificar resultado
        console.log('dosColumnas - Resultado longitud: ' + resultado.length + ', Contenido: "' + resultado + '"');
        
        return resultado;
    }
    
    // Función para línea de producto con cantidad, precio unitario y total
    function lineaProducto(cantidad, precioUnitario, totalProducto) {
        var cantStr = parseFloat(cantidad).toFixed(3);
        var precioStr = formatoMoneda(precioUnitario);
        var totalStr = formatoMoneda(totalProducto);
        
        var parteIzq = cantStr + ' x ' + precioStr;
        
        // Calcular espacios para alinear el total a la derecha
        var espacios = ancho - parteIzq.length - totalStr.length;
        if (espacios < 1) {
            // Si no cabe, acortar la parte izquierda
            var maxIzq = ancho - totalStr.length - 1;
            parteIzq = parteIzq.substring(0, maxIzq);
            espacios = 1;
        }
        
        return parteIzq + ' '.repeat(espacios) + totalStr;
    }
    
    var separador = '='.repeat(ancho);
    var linea = '-'.repeat(ancho);
    
    // ===== ENCABEZADO =====
    texto += '\n';
    texto += centrar((datos.negocio || 'MI TIENDA').toUpperCase()) + '\n';
    
    if (datos.rfc && datos.rfc.trim()) {
        texto += centrar('RFC: ' + datos.rfc) + '\n';
    }
    
    if (datos.direccion && datos.direccion.trim()) {
        var dir = datos.direccion;
        if (dir.length > ancho) {
            // Dividir dirección en múltiples líneas
            var palabras = dir.split(' ');
            var lineaActual = '';
            palabras.forEach(function(palabra) {
                if ((lineaActual + ' ' + palabra).trim().length <= ancho) {
                    lineaActual += (lineaActual ? ' ' : '') + palabra;
                } else {
                    if (lineaActual) texto += centrar(lineaActual) + '\n';
                    lineaActual = palabra;
                }
            });
            if (lineaActual) texto += centrar(lineaActual) + '\n';
        } else {
            texto += centrar(dir) + '\n';
        }
    }
    
    if (datos.telefono && datos.telefono.trim()) {
        texto += centrar('Tel: ' + datos.telefono) + '\n';
    }
    
    texto += separador + '\n';
    texto += '\n';
    
    // ===== TITULO =====
    texto += centrar('TICKET DE VENTA') + '\n';
    if (esCopia) {
        texto += centrar('*** COPIA - NEGOCIO ***') + '\n';
    }
    texto += '\n';
    texto += separador + '\n';
    
    // ===== DATOS DE VENTA =====
    texto += 'Folio: ' + (datos.folio || 'N/A') + '\n';
    texto += 'Fecha: ' + (datos.fecha || new Date().toLocaleString('es-MX', {
        year: 'numeric', month: '2-digit', day: '2-digit',
        hour: '2-digit', minute: '2-digit', hour12: true
    })) + '\n';
    
    // Cliente con word wrap
    var cliente = datos.cliente || 'PUBLICO GENERAL';
    if (cliente.length > ancho) {
        var palabrasCliente = cliente.split(' ');
        var lineaCliente = 'Cliente: ';
        palabrasCliente.forEach(function(palabra) {
            if ((lineaCliente + palabra).length <= ancho) {
                lineaCliente += palabra + ' ';
            } else {
                texto += lineaCliente.trim() + '\n';
                lineaCliente = '  ' + palabra + ' ';
            }
        });
        texto += lineaCliente.trim() + '\n';
    } else {
        texto += 'Cliente: ' + cliente + '\n';
    }
    
    texto += linea + '\n';
    
    // ===== PRODUCTOS =====
    if (datos.productos && datos.productos.length > 0) {
        datos.productos.forEach(function (p) {
            // Nombre del producto con word wrap
            var nombreProducto = (p.nombre || 'Producto').toUpperCase();
            if (nombreProducto.length > ancho) {
                var palabrasNombre = nombreProducto.split(' ');
                var lineaNombre = '';
                palabrasNombre.forEach(function(palabra) {
                    if ((lineaNombre + (lineaNombre ? ' ' : '') + palabra).length <= ancho) {
                        lineaNombre += (lineaNombre ? ' ' : '') + palabra;
                    } else {
                        if (lineaNombre) texto += lineaNombre + '\n';
                        lineaNombre = palabra;
                    }
                });
                if (lineaNombre) texto += lineaNombre + '\n';
            } else {
                texto += nombreProducto + '\n';
            }
            
            // Línea de detalle: cantidad x precio unitario       total
            var cantidad = parseFloat(p.cantidad || 0);
            var precio = parseFloat(p.precio || 0);
            var subtotal = parseFloat(p.subtotal || 0);
            
            texto += lineaProducto(cantidad, precio, subtotal) + '\n';
        });
        
        texto += separador + '\n';
    }
    
    // ===== TOTALES =====
    // Subtotal
    texto += dosColumnas('Subtotal:', formatoMoneda(datos.subtotal)) + '\n';
    
    // IVA
    var tasaIVA = 0;
    if (datos.subtotal && datos.subtotal > 0 && datos.iva) {
        tasaIVA = Math.round((datos.iva / datos.subtotal) * 100);
    }
    texto += dosColumnas('IVA (' + tasaIVA + '%):', formatoMoneda(datos.iva)) + '\n';
    
    // Separador antes del total
    texto += separador + '\n';
    texto += '\n';
    
    // TOTAL (destacado)
    texto += dosColumnas('TOTAL:', formatoMoneda(datos.total)) + '\n';
    
    texto += '\n';
    texto += separador + '\n';
    
    // ===== PAGO =====
    if (datos.efectivo && parseFloat(datos.efectivo) > 0) {
        texto += dosColumnas('Efectivo:', formatoMoneda(datos.efectivo)) + '\n';
        texto += dosColumnas('Cambio:', formatoMoneda(datos.cambio || 0)) + '\n';
        texto += separador + '\n';
    }
    
    // ===== PIE DE TICKET =====
    texto += '\n';
    var mensaje = datos.mensaje || 'Gracias por su compra';
    texto += centrar(mensaje) + '\n';
    texto += '\n';
    texto += centrar('¡Vuelva pronto!') + '\n';
    if (!esCopia) {
        texto += centrar('*** Conserve su ticket ***') + '\n';
    }
    texto += '\n';
    texto += '\n';
    texto += '\n';
    
    // Combinar logo HTML + texto envuelto en divs
    html += '<div>' + texto.replace(/\n/g, '</div><div>') + '</div>';
    return html;
}

function imprimirTicket(contenidoHTML, datosTicket) {
    // Imprimir TICKET 1: Para el cliente (sin marca de copia)
    var ticketCliente = datosTicket ? generarTicketHTML(datosTicket, false) : contenidoHTML;
    
    $.ajax({
        url: '/Configuracion/ImprimirTicketDirecto',
        type: 'POST',
        data: { contenidoTicket: ticketCliente },
        success: function (res) {
            if (res.success) {
                console.log('Ticket 1/2 (CLIENTE) enviado a imprimir correctamente');
            } else {
                toastr.warning(res.mensaje || 'No se pudo imprimir ticket cliente.');
                console.log('Error impresion ticket cliente:', res.mensaje);
            }
        },
        error: function (xhr, status, error) {
            var mensajeError = 'Error al imprimir ticket cliente: ' + error;
            if (xhr.responseJSON && xhr.responseJSON.mensaje) {
                mensajeError = xhr.responseJSON.mensaje;
            }
            toastr.error(mensajeError);
        }
    });
    
    // Esperar 500ms y luego imprimir TICKET 2: Para el negocio (con marca COPIA)
    setTimeout(function() {
        var ticketNegocio = datosTicket ? generarTicketHTML(datosTicket, true) : contenidoHTML;
        
        $.ajax({
            url: '/Configuracion/ImprimirTicketDirecto',
            type: 'POST',
            data: { contenidoTicket: ticketNegocio },
            success: function (res) {
                if (res.success) {
                    console.log('Ticket 2/2 (COPIA NEGOCIO) enviado a imprimir correctamente');
                    toastr.success('2 tickets impresos: 1 Cliente + 1 Copia Negocio');
                } else {
                    toastr.warning(res.mensaje || 'No se pudo imprimir copia negocio.');
                    console.log('Error impresion copia negocio:', res.mensaje);
                }
            },
            error: function (xhr, status, error) {
                var mensajeError = 'Error al imprimir copia negocio: ' + error;
                if (xhr.responseJSON && xhr.responseJSON.mensaje) {
                    mensajeError = xhr.responseJSON.mensaje;
                } else if (xhr.responseText) {
                    console.log('Response Text:', xhr.responseText);
                }
                toastr.error(mensajeError);
            }
        });
    }, 500);
}

