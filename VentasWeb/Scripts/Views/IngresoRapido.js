// IngresoRapido.js - Formulario optimizado para ingreso diario de productos congelados
// ============================================================================

var productosCongelados = []; // Catálogo de productos congelados

$(document).ready(function () {
    cargarSucursales();
    cargarProveedores();
    cargarProductosCongelados();
    cargarHistorialHoy();
    configurarEventos();
    
    // Establecer fecha de caducidad por defecto (30 días)
    establecerFechaCaducidadDefecto();
});

// ============================================================================
// CARGA DE DATOS
// ============================================================================

function cargarSucursales() {
    $.get('/Sucursal/Obtener', function (response) {
        if (response.success) {
            var $combo = $('#cboSucursal');
            $combo.empty().append('<option value="">-- Seleccione --</option>');
            response.data.forEach(function (sucursal) {
                $combo.append('<option value="' + sucursal.SucursalID + '">' + sucursal.Nombre + '</option>');
            });
            
            // Seleccionar primera sucursal por defecto
            if (response.data.length > 0) {
                $combo.val(response.data[0].SucursalID);
            }
        }
    });
}

function cargarProveedores() {
    $.get('/Proveedor/Obtener', function (response) {
        if (response.success) {
            var $combo = $('#cboProveedor');
            $combo.empty().append('<option value="">-- Seleccione --</option>');
            response.data.forEach(function (proveedor) {
                $combo.append('<option value="' + proveedor.ProveedorID + '">' + proveedor.RazonSocial + '</option>');
            });
        }
    });
}

function cargarProductosCongelados() {
    // Cargar productos con VentaPorGramaje = true (productos pesables)
    $.get('/Producto/Obtener', function (response) {
        if (response.success) {
            productosCongelados = response.data.filter(p => p.VentaPorGramaje === true);
            
            // Cargar en todos los combos de productos
            $('.cboProducto').each(function () {
                var $combo = $(this);
                $combo.empty().append('<option value="">-- Seleccione producto --</option>');
                productosCongelados.forEach(function (producto) {
                    $combo.append('<option value="' + producto.ProductoID + '" data-precio="' + (producto.PrecioPorKilo || 0) + '">' + 
                                producto.Nombre + '</option>');
                });
            });
        }
    });
}

function cargarHistorialHoy() {
    var fechaHoy = new Date().toISOString().split('T')[0];
    
    $.get('/Compra/ObtenerPorFecha', { fecha: fechaHoy }, function (response) {
        if (response.success && response.data.length > 0) {
            var $tbody = $('#tablaHistorial tbody');
            $tbody.empty();
            
            response.data.forEach(function (item) {
                var hora = new Date(item.FechaCreacion).toLocaleTimeString('es-MX', { hour: '2-digit', minute: '2-digit' });
                var fila = '<tr>' +
                    '<td>' + hora + '</td>' +
                    '<td>' + item.NombreProducto + '</td>' +
                    '<td>' + item.Cantidad + ' kg</td>' +
                    '<td>$' + item.PrecioCompra.toFixed(2) + '</td>' +
                    '<td class="text-success font-weight-bold">$' + (item.Cantidad * item.PrecioCompra).toFixed(2) + '</td>' +
                    '<td>' + item.NombreProveedor + '</td>' +
                    '</tr>';
                $tbody.append(fila);
            });
        }
    });
}

// ============================================================================
// CONFIGURACIÓN DE EVENTOS
// ============================================================================

function configurarEventos() {
    // Agregar producto
    $('#btnAgregarProducto').on('click', agregarFilaProducto);
    
    // Eliminar producto
    $(document).on('click', '.btnEliminarFila', function () {
        $(this).closest('tr').remove();
        actualizarTotales();
        actualizarBotonesEliminar();
    });
    
    // Calcular precio de venta sugerido al cambiar precio de compra
    $(document).on('change', '.txtPrecioCompra', function () {
        var $fila = $(this).closest('tr');
        var precioCompra = parseFloat($(this).val()) || 0;
        var precioVentaSugerido = precioCompra * 1.35; // 35% de margen
        $fila.find('.txtPrecioVenta').val(precioVentaSugerido.toFixed(2));
        actualizarTotales();
    });
    
    // Actualizar totales al cambiar cantidad
    $(document).on('change', '.txtCantidad', actualizarTotales);
    
    // Autocompletar precio de venta al seleccionar producto
    $(document).on('change', '.cboProducto', function () {
        var $fila = $(this).closest('tr');
        var precioVenta = $(this).find(':selected').data('precio') || 0;
        if (precioVenta > 0) {
            $fila.find('.txtPrecioVenta').val(precioVenta.toFixed(2));
        }
    });
    
    // Submit del formulario
    $('#frmIngresoRapido').on('submit', function (e) {
        e.preventDefault();
        guardarIngresoRapido();
    });
}

function agregarFilaProducto() {
    var nuevaFila = `
        <tr class="fila-producto">
            <td>
                <select class="form-control form-control-sm cboProducto" required>
                    <option value="">-- Seleccione producto --</option>
                </select>
            </td>
            <td>
                <input type="number" step="0.01" min="0.01" class="form-control form-control-sm txtCantidad" placeholder="0.00" required>
            </td>
            <td>
                <input type="number" step="0.01" min="0.01" class="form-control form-control-sm txtPrecioCompra" placeholder="0.00" required>
            </td>
            <td>
                <input type="number" step="0.01" min="0.01" class="form-control form-control-sm txtPrecioVenta" placeholder="0.00">
            </td>
            <td>
                <input type="date" class="form-control form-control-sm txtFechaCaducidad">
            </td>
            <td class="text-center">
                <button type="button" class="btn btn-sm btn-danger btnEliminarFila">
                    <i class="fas fa-trash"></i>
                </button>
            </td>
        </tr>
    `;
    
    $('#tablaProductos tbody').append(nuevaFila);
    
    // Cargar productos en el nuevo combo
    var $ultimoCombo = $('#tablaProductos tbody tr:last .cboProducto');
    $ultimoCombo.empty().append('<option value="">-- Seleccione producto --</option>');
    productosCongelados.forEach(function (producto) {
        $ultimoCombo.append('<option value="' + producto.ProductoID + '" data-precio="' + (producto.PrecioPorKilo || 0) + '">' + 
                          producto.Nombre + '</option>');
    });
    
    // Establecer fecha de caducidad
    establecerFechaCaducidadDefecto();
    actualizarBotonesEliminar();
}

function actualizarBotonesEliminar() {
    var totalFilas = $('.fila-producto').length;
    if (totalFilas === 1) {
        $('.btnEliminarFila').prop('disabled', true);
    } else {
        $('.btnEliminarFila').prop('disabled', false);
    }
}

function establecerFechaCaducidadDefecto() {
    // Fecha de caducidad: 30 días desde hoy
    var fecha = new Date();
    fecha.setDate(fecha.getDate() + 30);
    var fechaStr = fecha.toISOString().split('T')[0];
    
    $('.txtFechaCaducidad').each(function () {
        if (!$(this).val()) {
            $(this).val(fechaStr);
        }
    });
}

// ============================================================================
// CÁLCULOS
// ============================================================================

function actualizarTotales() {
    var totalProductos = 0;
    var totalKg = 0;
    var totalMonto = 0;
    
    $('.fila-producto').each(function () {
        var cantidad = parseFloat($(this).find('.txtCantidad').val()) || 0;
        var precioCompra = parseFloat($(this).find('.txtPrecioCompra').val()) || 0;
        
        if (cantidad > 0 && precioCompra > 0) {
            totalProductos++;
            totalKg += cantidad;
            totalMonto += cantidad * precioCompra;
        }
    });
    
    $('#lblTotalProductos').text(totalProductos);
    $('#lblTotalKg').text(totalKg.toFixed(2));
    $('#lblTotal').text('$' + totalMonto.toFixed(2));
}

// ============================================================================
// GUARDAR
// ============================================================================

function guardarIngresoRapido() {
    // Validaciones
    var sucursalId = $('#cboSucursal').val();
    var proveedorId = $('#cboProveedor').val();
    
    if (!sucursalId) {
        toastr.warning('Seleccione una sucursal');
        return;
    }
    
    if (!proveedorId) {
        toastr.warning('Seleccione un proveedor');
        return;
    }
    
    // Construir detalle de productos
    var detalle = [];
    var valido = true;
    
    $('.fila-producto').each(function () {
        var productoId = $(this).find('.cboProducto').val();
        var cantidad = parseFloat($(this).find('.txtCantidad').val()) || 0;
        var precioCompra = parseFloat($(this).find('.txtPrecioCompra').val()) || 0;
        var precioVenta = parseFloat($(this).find('.txtPrecioVenta').val()) || 0;
        var fechaCaducidad = $(this).find('.txtFechaCaducidad').val();
        
        if (!productoId) {
            toastr.warning('Seleccione todos los productos');
            valido = false;
            return false;
        }
        
        if (cantidad <= 0) {
            toastr.warning('Ingrese cantidades válidas');
            valido = false;
            return false;
        }
        
        if (precioCompra <= 0) {
            toastr.warning('Ingrese precios de compra válidos');
            valido = false;
            return false;
        }
        
        detalle.push({
            ProductoID: parseInt(productoId),
            Cantidad: cantidad,
            PrecioCompra: precioCompra,
            PrecioVenta: precioVenta > 0 ? precioVenta : precioCompra * 1.35,
            FechaCaducidad: fechaCaducidad || null
        });
    });
    
    if (!valido) return;
    
    if (detalle.length === 0) {
        toastr.warning('Agregue al menos un producto');
        return;
    }
    
    // Construir objeto de compra
    var compra = {
        SucursalID: parseInt(sucursalId),
        ProveedorID: parseInt(proveedorId),
        FolioFactura: $('#txtFolioFactura').val() || 'INGRESO-' + new Date().getTime(),
        Observaciones: $('#txtObservaciones').val(),
        Detalle: detalle,
        Usuario: '@User.Identity.Name' || 'system'
    };
    
    // Mostrar loading
    Swal.fire({
        title: 'Guardando...',
        text: 'Registrando ingreso de productos',
        allowOutsideClick: false,
        showConfirmButton: false,
        willOpen: () => {
            Swal.showLoading();
        }
    });
    
    // Enviar al servidor
    $.ajax({
        url: '/Compra/RegistrarIngresoRapido',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(compra),
        success: function (response) {
            Swal.close();
            
            if (response.success) {
                Swal.fire({
                    icon: 'success',
                    title: '¡Ingreso Registrado!',
                    html: '<p>Se han creado <strong>' + detalle.length + '</strong> lote(s) nuevos.</p>' +
                          '<p>Los productos están disponibles en inventario.</p>',
                    confirmButtonText: 'Aceptar'
                }).then(() => {
                    // Limpiar formulario
                    limpiarFormulario();
                    // Recargar historial
                    cargarHistorialHoy();
                });
            } else {
                Swal.fire({
                    icon: 'error',
                    title: 'Error',
                    text: response.mensaje || 'Ocurrió un error al guardar'
                });
            }
        },
        error: function (xhr) {
            Swal.close();
            var mensaje = xhr.responseJSON?.mensaje || 'Error de conexión';
            Swal.fire({
                icon: 'error',
                title: 'Error',
                text: mensaje
            });
        }
    });
}

function limpiarFormulario() {
    // Mantener sucursal y proveedor
    var sucursalActual = $('#cboSucursal').val();
    var proveedorActual = $('#cboProveedor').val();
    
    // Limpiar productos (dejar solo una fila)
    $('#tablaProductos tbody').empty();
    agregarFilaProducto();
    
    // Restaurar valores
    $('#cboSucursal').val(sucursalActual);
    $('#cboProveedor').val(proveedorActual);
    $('#txtFolioFactura').val('');
    $('#txtObservaciones').val('');
    
    actualizarTotales();
}
