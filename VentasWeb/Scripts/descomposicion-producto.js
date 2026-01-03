// Scripts/descomposicion-producto.js
// Script para gestión de descomposición de productos

let productosResultantes = [];
let sucursalID = 1; // Obtener de la sesión o contexto

$(document).ready(function () {
    inicializarSelect2();
    cargarProductosOrigen();
    cargarProductosResultantes();
    cargarHistorial();
});

// Inicializar Select2
function inicializarSelect2() {
    $('.select2').select2({
        theme: 'bootstrap4',
        language: 'es',
        placeholder: '-- Seleccione --'
    });
}

// Cargar productos con stock para descomponer
function cargarProductosOrigen() {
    $.ajax({
        url: '/DescomposicionProducto/ObtenerProductosConStock',
        type: 'GET',
        data: { sucursalID: sucursalID },
        success: function (response) {
            if (response.success) {
                const ddl = $('#ddlProductoOrigen');
                ddl.empty();
                ddl.append('<option value="">-- Seleccione --</option>');
                
                response.data.forEach(function (producto) {
                    ddl.append(`<option value="${producto.ProductoID}" data-stock="${producto.Stock || 0}">
                        ${producto.Nombre} ${producto.CodigoInterno ? '(' + producto.CodigoInterno + ')' : ''}
                    </option>`);
                });
                
                ddl.trigger('change');
            } else {
                toastr.error(response.mensaje || 'Error al cargar productos');
            }
        },
        error: function () {
            toastr.error('Error de conexión al cargar productos origen');
        }
    });
}

// Cargar todos los productos para seleccionar como resultantes
function cargarProductosResultantes() {
    $.ajax({
        url: '/DescomposicionProducto/ObtenerProductos',
        type: 'GET',
        success: function (response) {
            if (response.success) {
                const ddl = $('#ddlProductoResultante');
                ddl.empty();
                ddl.append('<option value="">-- Seleccione --</option>');
                
                response.data.forEach(function (producto) {
                    ddl.append(`<option value="${producto.ProductoID}">
                        ${producto.Nombre} ${producto.CodigoInterno ? '(' + producto.CodigoInterno + ')' : ''}
                    </option>`);
                });
                
                ddl.trigger('change');
            } else {
                toastr.error(response.mensaje || 'Error al cargar productos');
            }
        },
        error: function () {
            toastr.error('Error de conexión al cargar productos resultantes');
        }
    });
}

// Evento cuando cambia el producto origen
$('#ddlProductoOrigen').on('change', function () {
    const selectedOption = $(this).find('option:selected');
    const stock = selectedOption.data('stock') || 0;
    $('#txtStockDisponible').val(stock);
    
    // Validar cantidad según stock
    $('#txtCantidadOrigen').attr('max', stock);
});

// Agregar producto resultante a la lista
function agregarProductoResultante() {
    const productoID = parseInt($('#ddlProductoResultante').val());
    const cantidad = parseFloat($('#txtCantidadResultante').val());
    const pesoUnidad = $('#txtPesoUnidad').val() ? parseFloat($('#txtPesoUnidad').val()) : null;
    
    // Validaciones
    if (!productoID) {
        toastr.warning('Seleccione un producto resultante');
        return;
    }
    
    if (!cantidad || cantidad <= 0) {
        toastr.warning('Ingrese una cantidad válida');
        return;
    }
    
    // Verificar que no esté duplicado
    const existe = productosResultantes.find(p => p.ProductoResultanteID === productoID);
    if (existe) {
        toastr.warning('Este producto ya fue agregado');
        return;
    }
    
    // Obtener nombre del producto
    const nombreProducto = $('#ddlProductoResultante option:selected').text();
    
    // Agregar al array
    productosResultantes.push({
        ProductoResultanteID: productoID,
        NombreProducto: nombreProducto,
        CantidadResultante: cantidad,
        PesoUnidad: pesoUnidad
    });
    
    // Renderizar tabla
    renderizarTablaResultantes();
    
    // Limpiar campos
    $('#ddlProductoResultante').val('').trigger('change');
    $('#txtCantidadResultante').val(1);
    $('#txtPesoUnidad').val('');
    
    toastr.success('Producto agregado');
}

// Renderizar tabla de productos resultantes
function renderizarTablaResultantes() {
    const tbody = $('#tbodyProductosResultantes');
    tbody.empty();
    
    if (productosResultantes.length === 0) {
        tbody.append(`
            <tr id="trNoProductos">
                <td colspan="5" class="text-center text-muted">No se han agregado productos resultantes</td>
            </tr>
        `);
        return;
    }
    
    productosResultantes.forEach(function (producto, index) {
        const totalPeso = producto.PesoUnidad ? (producto.CantidadResultante * producto.PesoUnidad).toFixed(3) : '-';
        const pesoDisplay = producto.PesoUnidad ? producto.PesoUnidad.toFixed(3) : '-';
        
        tbody.append(`
            <tr>
                <td>${producto.NombreProducto}</td>
                <td class="text-center">${producto.CantidadResultante}</td>
                <td class="text-center">${pesoDisplay}</td>
                <td class="text-center">${totalPeso}</td>
                <td class="text-center">
                    <button type="button" class="btn btn-sm btn-danger" onclick="eliminarProductoResultante(${index})">
                        <i class="fas fa-trash"></i>
                    </button>
                </td>
            </tr>
        `);
    });
}

// Eliminar producto resultante
function eliminarProductoResultante(index) {
    productosResultantes.splice(index, 1);
    renderizarTablaResultantes();
    toastr.info('Producto eliminado');
}

// Registrar descomposición
function registrarDescomposicion() {
    // Validaciones
    const productoOrigenID = parseInt($('#ddlProductoOrigen').val());
    const cantidadOrigen = parseFloat($('#txtCantidadOrigen').val());
    const observaciones = $('#txtObservaciones').val().trim();
    
    if (!productoOrigenID) {
        toastr.warning('Seleccione el producto origen');
        return;
    }
    
    if (!cantidadOrigen || cantidadOrigen <= 0) {
        toastr.warning('Ingrese una cantidad válida a descomponer');
        return;
    }
    
    if (productosResultantes.length === 0) {
        toastr.warning('Debe agregar al menos un producto resultante');
        return;
    }
    
    // Validar stock disponible
    const stockDisponible = parseFloat($('#txtStockDisponible').val() || 0);
    if (cantidadOrigen > stockDisponible) {
        toastr.error('La cantidad a descomponer excede el stock disponible');
        return;
    }
    
    // Preparar payload
    const payload = {
        ProductoOrigenID: productoOrigenID,
        CantidadOrigen: cantidadOrigen,
        SucursalID: sucursalID,
        Observaciones: observaciones,
        Detalle: productosResultantes
    };
    
    // Mostrar loading
    Swal.fire({
        title: 'Procesando',
        text: 'Registrando descomposición...',
        allowOutsideClick: false,
        didOpen: () => {
            Swal.showLoading();
        }
    });
    
    // Enviar petición
    $.ajax({
        url: '/DescomposicionProducto/RegistrarDescomposicion',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(payload),
        success: function (response) {
            Swal.close();
            
            if (response.success) {
                Swal.fire({
                    icon: 'success',
                    title: 'Éxito',
                    text: response.mensaje,
                    timer: 2000
                });
                
                limpiarFormulario();
                cargarProductosOrigen();
                cargarHistorial();
            } else {
                Swal.fire({
                    icon: 'error',
                    title: 'Error',
                    text: response.mensaje
                });
            }
        },
        error: function (xhr) {
            Swal.close();
            Swal.fire({
                icon: 'error',
                title: 'Error',
                text: 'Error de conexión al registrar descomposición'
            });
        }
    });
}

// Limpiar formulario
function limpiarFormulario() {
    $('#ddlProductoOrigen').val('').trigger('change');
    $('#txtCantidadOrigen').val(1);
    $('#txtStockDisponible').val('');
    $('#txtObservaciones').val('');
    
    productosResultantes = [];
    renderizarTablaResultantes();
}

// Cargar historial de descomposiciones
function cargarHistorial() {
    $.ajax({
        url: '/DescomposicionProducto/ObtenerHistorial',
        type: 'GET',
        success: function (response) {
            if (response.success) {
                const table = $('#tblHistorial').DataTable({
                    destroy: true,
                    data: response.data,
                    language: {
                        url: '//cdn.datatables.net/plug-ins/1.13.4/i18n/es-ES.json'
                    },
                    columns: [
                        {
                            data: 'FechaDescomposicion',
                            render: function (data) {
                                return moment(data).format('DD/MM/YYYY HH:mm');
                            }
                        },
                        { data: 'Usuario' },
                        {
                            data: null,
                            render: function (row) {
                                return `${row.ProductoOrigen} (${row.CantidadOrigen} ${row.UnidadOrigen || 'unidades'})`;
                            }
                        },
                        { data: 'CantidadOrigen' },
                        { data: 'ProductosResultantes' },
                        { data: 'Observaciones' },
                        {
                            data: null,
                            orderable: false,
                            render: function (row) {
                                return `
                                    <button class="btn btn-sm btn-info" onclick="verDetalle(${row.DescomposicionID})">
                                        <i class="fas fa-eye"></i>
                                    </button>
                                `;
                            }
                        }
                    ],
                    order: [[0, 'desc']]
                });
            } else {
                toastr.error(response.mensaje || 'Error al cargar historial');
            }
        },
        error: function () {
            toastr.error('Error de conexión al cargar historial');
        }
    });
}

// Ver detalle de descomposición
function verDetalle(descomposicionID) {
    $.ajax({
        url: '/DescomposicionProducto/ObtenerDetalle',
        type: 'GET',
        data: { id: descomposicionID },
        success: function (response) {
            if (response.success) {
                const data = response.data;
                
                let html = `
                    <div class="row">
                        <div class="col-md-6">
                            <strong>Producto Origen:</strong> ${data.ProductoOrigenNombre}<br>
                            <strong>Cantidad Descompuesta:</strong> ${data.CantidadOrigen}<br>
                            <strong>Fecha:</strong> ${moment(data.FechaDescomposicion).format('DD/MM/YYYY HH:mm')}<br>
                            <strong>Usuario:</strong> ${data.Usuario}
                        </div>
                        <div class="col-md-6">
                            <strong>Observaciones:</strong><br>
                            ${data.Observaciones || 'Sin observaciones'}
                        </div>
                    </div>
                    <hr>
                    <h5>Productos Resultantes:</h5>
                    <table class="table table-sm table-bordered">
                        <thead class="bg-light">
                            <tr>
                                <th>Producto</th>
                                <th>Cantidad</th>
                                <th>Peso c/u (kg)</th>
                                <th>Total Peso (kg)</th>
                            </tr>
                        </thead>
                        <tbody>
                `;
                
                data.Detalle.forEach(function (detalle) {
                    const totalPeso = detalle.PesoUnidad ? (detalle.CantidadResultante * detalle.PesoUnidad).toFixed(3) : '-';
                    const pesoDisplay = detalle.PesoUnidad ? detalle.PesoUnidad.toFixed(3) : '-';
                    
                    html += `
                        <tr>
                            <td>${detalle.ProductoResultanteNombre}</td>
                            <td class="text-center">${detalle.CantidadResultante}</td>
                            <td class="text-center">${pesoDisplay}</td>
                            <td class="text-center">${totalPeso}</td>
                        </tr>
                    `;
                });
                
                html += `
                        </tbody>
                    </table>
                `;
                
                $('#detalleContent').html(html);
                $('#modalDetalle').modal('show');
            } else {
                toastr.error(response.mensaje || 'Error al obtener detalle');
            }
        },
        error: function () {
            toastr.error('Error de conexión al obtener detalle');
        }
    });
}
