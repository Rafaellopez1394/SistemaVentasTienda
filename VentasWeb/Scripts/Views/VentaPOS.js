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

            // Métodos de pago
            const $metodo = $('#cboMetodoPago').empty().append('<option value="">Seleccionar...</option>');
            res.metodosPago.forEach(mp => {
                $metodo.append(`<option value="${mp.MetodoPagoID}">${mp.Descripcion}</option>`);
            });

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

    $('#cboFormaPago').val('');
    $('#cboMetodoPago').val('');
    $('#txtEfectivoRecibido').val('');
    $('#panelEfectivo').hide();
    $('#panelCambio').hide();

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
    // Obtener datos del negocio
    $.get('/Configuracion/ObtenerDatosNegocio', function (res) {
        var datosNegocio = res.success ? res.data : {
            NombreNegocio: 'MI TIENDA',
            RFC: '',
            Direccion: '',
            Telefono: '',
            MensajeTicket: 'Gracias por su compra',
            ImprimirTicketAutomatico: false
        };
        
        var clienteNombre = clienteActual ? clienteActual.RazonSocial : 'PUBLICO GENERAL';
        
        var datosTicket = {
            negocio: datosNegocio.NombreNegocio,
            rfc: datosNegocio.RFC,
            direccion: datosNegocio.Direccion,
            telefono: datosNegocio.Telefono,
            folio: ventaId ? ventaId.substring(0, 8).toUpperCase() : 'N/A',
            fecha: new Date().toLocaleString(),
            cliente: clienteNombre,
            productos: productos.map(p => ({
                nombre: p.Producto || p.nombre || 'Producto',
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
        
        var contenidoHTML = generarTicketHTML(datosTicket);
        
        // Imprimir directamente al finalizar la venta
        imprimirTicket(contenidoHTML);
        
    }).fail(function() {
        // Si falla obtener config, usar valores por defecto e imprimir igual
        var clienteNombre = clienteActual ? clienteActual.RazonSocial : 'PUBLICO GENERAL';
        
        var datosTicket = {
            negocio: 'MI TIENDA',
            folio: ventaId ? ventaId.substring(0, 8).toUpperCase() : 'N/A',
            fecha: new Date().toLocaleString(),
            cliente: clienteNombre,
            productos: productos.map(p => ({
                nombre: p.Producto || p.nombre || 'Producto',
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
        
        // Imprimir directamente
        imprimirTicket(generarTicketHTML(datosTicket));
    });
}

function generarTicketHTML(datos) {
    var linea = '--------------------------------';
    var html = '';
    
    // Usar el mismo formato que el ticket de prueba (con clases CSS simples)
    html += '<div class="ticket-container">';
    
    // Encabezado del negocio
    html += '<div class="ticket-header">';
    html += '<div class="negocio-nombre">' + (datos.negocio || 'MI TIENDA') + '</div>';
    if (datos.rfc) html += '<div>RFC: ' + datos.rfc + '</div>';
    if (datos.direccion) html += '<div class="direccion">' + datos.direccion + '</div>';
    if (datos.telefono) html += '<div>Tel: ' + datos.telefono + '</div>';
    html += '</div>';
    
    html += '<div class="linea">' + linea + '</div>';
    
    // Datos de la venta
    html += '<div class="ticket-info">';
    html += '<div class="titulo-venta">TICKET DE VENTA</div>';
    html += '<div>Folio: ' + datos.folio + '</div>';
    html += '<div>Fecha: ' + datos.fecha + '</div>';
    html += '<div>Cliente: ' + (datos.cliente || 'PUBLICO GENERAL') + '</div>';
    html += '</div>';
    
    html += '<div class="linea">' + linea + '</div>';
    
    // Lista de productos (formato vertical para 58mm)
    html += '<div class="productos-lista">';
    if (datos.productos && datos.productos.length > 0) {
        datos.productos.forEach(function (p) {
            html += '<div class="producto-item">';
            html += '<div class="producto-nombre">' + (p.nombre || 'Producto').substring(0, 22) + '</div>';
            html += '<div class="producto-detalle">';
            html += '<span>' + p.cantidad + ' x $' + p.precio.toFixed(2) + '</span>';
            html += '<span class="producto-total">$' + p.subtotal.toFixed(2) + '</span>';
            html += '</div>';
            html += '</div>';
        });
    }
    html += '</div>';
    
    html += '<div class="linea">' + linea + '</div>';
    
    // Totales
    html += '<div class="totales">';
    html += '<div class="total-row"><span>Subtotal:</span><span>$' + datos.subtotal.toFixed(2) + '</span></div>';
    html += '<div class="total-row"><span>IVA 16%:</span><span>$' + datos.iva.toFixed(2) + '</span></div>';
    html += '<div class="total-row total-final"><span>TOTAL:</span><span>$' + datos.total.toFixed(2) + '</span></div>';
    
    if (datos.efectivo && datos.efectivo > 0) {
        html += '<div class="pago-info">';
        html += '<div class="total-row"><span>Efectivo:</span><span>$' + datos.efectivo.toFixed(2) + '</span></div>';
        html += '<div class="total-row"><span>Cambio:</span><span>$' + datos.cambio.toFixed(2) + '</span></div>';
        html += '</div>';
    }
    html += '</div>';
    
    html += '<div class="linea">' + linea + '</div>';
    
    // Mensaje de pie
    html += '<div class="ticket-footer">';
    html += '<div>' + (datos.mensaje || 'Gracias por su compra') + '</div>';
    html += '<div class="conserve">*** Conserve su ticket ***</div>';
    html += '</div>';
    html += '</div>';
    
    return html;
}

function imprimirTicket(contenidoHTML) {
    // Enviar a imprimir directamente a la impresora configurada en el servidor
    $.ajax({
        url: '/Configuracion/ImprimirTicketDirecto',
        type: 'POST',
        data: { contenidoTicket: contenidoHTML },
        success: function (res) {
            if (res.success) {
                console.log('Ticket enviado a imprimir correctamente');
            } else {
                toastr.warning(res.mensaje || 'No se pudo imprimir. Verifique la impresora.');
                console.log('Error impresion:', res.mensaje);
            }
        },
        error: function (xhr, status, error) {
            var mensajeError = 'Error al imprimir: ' + error;
            if (xhr.responseJSON && xhr.responseJSON.mensaje) {
                mensajeError = xhr.responseJSON.mensaje;
            } else if (xhr.responseText) {
                console.log('Response Text:', xhr.responseText);
                mensajeError += ' - Revisa la consola para mas detalles';
            }
            toastr.error(mensajeError);
        }
    });
}
