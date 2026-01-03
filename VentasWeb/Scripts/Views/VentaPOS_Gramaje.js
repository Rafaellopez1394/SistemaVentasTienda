// VentasWeb/Scripts/Views/VentaPOS_Gramaje.js
// ============================================================================
// FUNCIONALIDADES PARA VENTA POR GRAMAJE
// ============================================================================

// Modal para ingresar gramos
function mostrarModalGramaje(producto) {
    if (!producto.VentaPorGramaje) {
        toastr.error('Este producto no está configurado para venta por gramaje');
        return;
    }

    if (!producto.PrecioPorKilo || producto.PrecioPorKilo <= 0) {
        toastr.error('Este producto no tiene precio por kilo configurado');
        return;
    }

    // Crear modal dinámicamente
    const modalHTML = `
        <div class="modal fade" id="modalGramaje" tabindex="-1" role="dialog">
            <div class="modal-dialog" role="document">
                <div class="modal-content">
                    <div class="modal-header bg-primary text-white">
                        <h5 class="modal-title">
                            <i class="fas fa-weight"></i> Venta por Gramaje
                        </h5>
                        <button type="button" class="close text-white" data-dismiss="modal">
                            <span>&times;</span>
                        </button>
                    </div>
                    <div class="modal-body">
                        <h6>${producto.Nombre}</h6>
                        <p class="text-muted mb-3">
                            Precio por Kilo: <strong class="text-success">$${producto.PrecioPorKilo.toFixed(2)}</strong>
                        </p>

                        <div class="form-group">
                            <label>Cantidad en Gramos:</label>
                            <div class="input-group input-group-lg">
                                <input type="number" class="form-control" id="txtGramos" 
                                       min="1" step="1" value="500" placeholder="Ej: 500, 1000, 250">
                                <span class="input-group-text">g</span>
                            </div>
                            <small class="form-text text-muted">
                                Equivalente: <strong id="lblEquivalenteKg">0.500</strong> kg
                            </small>
                        </div>

                        <div class="alert alert-info mt-3">
                            <h5 class="mb-2">Precio Calculado:</h5>
                            <h3 class="mb-0 text-primary">
                                <strong>$<span id="lblPrecioCalculado">0.00</span></strong>
                            </h3>
                        </div>

                        <!-- Botones rápidos -->
                        <div class="mt-3">
                            <label>Cantidades Rápidas:</label><br>
                            <button type="button" class="btn btn-sm btn-outline-primary m-1" onclick="setGramos(250)">250g</button>
                            <button type="button" class="btn btn-sm btn-outline-primary m-1" onclick="setGramos(500)">500g</button>
                            <button type="button" class="btn btn-sm btn-outline-primary m-1" onclick="setGramos(1000)">1kg</button>
                            <button type="button" class="btn btn-sm btn-outline-primary m-1" onclick="setGramos(2000)">2kg</button>
                            <button type="button" class="btn btn-sm btn-outline-primary m-1" onclick="setGramos(5000)">5kg</button>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-secondary" data-dismiss="modal">Cancelar</button>
                        <button type="button" class="btn btn-success btn-lg" onclick="agregarPorGramaje()">
                            <i class="fas fa-cart-plus"></i> Agregar al Carrito
                        </button>
                    </div>
                </div>
            </div>
        </div>
    `;

    // Eliminar modal anterior si existe
    $('#modalGramaje').remove();
    
    // Agregar modal al body
    $('body').append(modalHTML);
    
    // Guardar producto en variable global temporal
    window.productoGramajeActual = producto;
    
    // Mostrar modal
    $('#modalGramaje').modal('show');
    
    // Configurar eventos
    $('#txtGramos').on('input', calcularPrecioPorGramaje);
    
    // Calcular precio inicial
    calcularPrecioPorGramaje();
    
    // Focus en el input
    setTimeout(() => $('#txtGramos').focus().select(), 500);
}

// Establecer gramos rápidos
function setGramos(gramos) {
    $('#txtGramos').val(gramos);
    calcularPrecioPorGramaje();
}

// Calcular precio según los gramos ingresados
function calcularPrecioPorGramaje() {
    const gramos = parseFloat($('#txtGramos').val()) || 0;
    const producto = window.productoGramajeActual;
    
    if (!producto || !producto.PrecioPorKilo) {
        return;
    }
    
    // Calcular equivalente en kg
    const kilogramos = gramos / 1000;
    $('#lblEquivalenteKg').text(kilogramos.toFixed(3));
    
    // Calcular precio: (PrecioPorKilo / 1000) * Gramos
    const precioCalculado = (producto.PrecioPorKilo / 1000) * gramos;
    $('#lblPrecioCalculado').text(precioCalculado.toFixed(2));
}

// Agregar producto por gramaje al carrito
function agregarPorGramaje() {
    const gramos = parseFloat($('#txtGramos').val());
    const producto = window.productoGramajeActual;
    
    if (!gramos || gramos <= 0) {
        toastr.warning('Ingrese una cantidad válida de gramos');
        return;
    }
    
    if (gramos < 1) {
        toastr.warning('La cantidad mínima es 1 gramo');
        return;
    }
    
    // Calcular precio
    const precioCalculado = (producto.PrecioPorKilo / 1000) * gramos;
    const kilogramos = gramos / 1000;
    
    // Buscar si ya existe un item con este producto y mismos gramos
    const itemExistente = carrito.find(item => 
        item.ProductoID === producto.ProductoID && 
        item.Gramos === gramos
    );
    
    if (itemExistente) {
        toastr.info('Este producto con la misma cantidad ya está en el carrito');
        $('#modalGramaje').modal('hide');
        return;
    }
    
    // Agregar al carrito
    carrito.push({
        ProductoID: producto.ProductoID,
        LoteID: producto.LoteID,
        Nombre: producto.Nombre,
        CodigoInterno: producto.CodigoInterno,
        Cantidad: 1, // Siempre 1 para productos por gramaje
        PrecioVenta: precioCalculado,
        PrecioCompra: producto.PrecioCompra,
        TasaIVA: producto.TasaIVA || 0,
        StockDisponible: producto.StockDisponible,
        // Campos específicos de gramaje
        VentaPorGramaje: true,
        Gramos: gramos,
        Kilogramos: kilogramos,
        PrecioPorKilo: producto.PrecioPorKilo,
        PrecioCalculado: precioCalculado
    });
    
    actualizarCarrito();
    $('#modalGramaje').modal('hide');
    toastr.success(`${gramos}g de ${producto.Nombre} agregado al carrito`);
}

// Modificar la función agregarAlCarrito original para detectar productos por gramaje
const agregarAlCarritoOriginal = window.agregarAlCarrito;
window.agregarAlCarrito = function(producto) {
    // Si el producto se vende por gramaje, mostrar modal
    if (producto.VentaPorGramaje && producto.PrecioPorKilo) {
        mostrarModalGramaje(producto);
        $('#txtBuscarProducto').val('').focus();
        $('#resultadosBusqueda').empty();
    } else {
        // Usar función original
        agregarAlCarritoOriginal(producto);
    }
};

// Modificar la función actualizarCarrito para mostrar info de gramaje
const actualizarCarritoOriginal = window.actualizarCarrito;
window.actualizarCarrito = function() {
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

        // Si es venta por gramaje, mostrar información diferente
        if (item.VentaPorGramaje) {
            html += `
                <div class="carrito-item">
                    <div class="row align-items-center">
                        <div class="col-6">
                            <strong>${item.Nombre}</strong>
                            <br><small class="text-muted">${item.CodigoInterno || ''}</small>
                            <br><small class="text-info">IVA ${item.TasaIVA}%</small>
                            <br><span class="badge bg-warning text-dark">
                                <i class="fas fa-weight"></i> ${item.Gramos}g (${item.Kilogramos.toFixed(3)} kg)
                            </span>
                        </div>
                        <div class="col-3 text-center">
                            <small class="text-muted">$${item.PrecioPorKilo.toFixed(2)}/kg</small>
                            <br><strong class="text-primary">$${item.PrecioCalculado.toFixed(2)}</strong>
                        </div>
                        <div class="col-2 text-end">
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
        } else {
            // Productos normales - usar función original
            html += `
                <div class="carrito-item">
                    <div class="row align-items-center">
                        <div class="col-5">
                            <strong>${item.Nombre}</strong>
                            <br><small class="text-muted">${item.CodigoInterno || ''}</small>
                            <br><small class="text-info">IVA ${item.TasaIVA}%</small>
                        </div>
                        <div class="col-3">
                            <div class="input-group input-group-sm">
                                <button class="btn btn-outline-secondary" onclick="cambiarCantidad(${index}, -1)">-</button>
                                <input type="number" class="form-control cantidad-input" value="${item.Cantidad}" 
                                       onchange="cambiarCantidadDirecta(${index}, this.value)" min="1" max="${item.StockDisponible}" />
                                <button class="btn btn-outline-secondary" onclick="cambiarCantidad(${index}, 1)">+</button>
                            </div>
                            <small class="text-muted">$${item.PrecioVenta.toFixed(2)} c/u</small>
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
        }
    });

    $('#carritoItems').html(html);
    actualizarTotales();
};

// Agregar indicador de gramaje en los resultados de búsqueda
$(document).on('DOMSubtreeModified', '#resultadosBusqueda', function() {
    // Este evento se dispara cuando se actualizan los resultados
    // Podemos agregar badges visuales para productos por gramaje
});

console.log('Módulo de venta por gramaje cargado correctamente');
