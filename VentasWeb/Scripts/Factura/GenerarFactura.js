// Agregar a VentaPOS o Venta Index para botón de facturar

function mostrarModalFacturar(ventaId) {
    // Cargar datos de la venta
    $.ajax({
        url: '/Venta/ObtenerDetalle',
        type: 'GET',
        data: { ventaId: ventaId },
        success: function (response) {
            if (response.success) {
                var venta = response.data;
                
                $('#hdnVentaID').val(ventaId);
                $('#lblVentaTotal').text('$' + parseFloat(venta.MontoTotal).toFixed(2));
                $('#lblVentaFecha').text(moment(venta.FechaVenta).format('DD/MM/YYYY HH:mm'));
                
                // Cargar catálogos
                cargarUsosCFDI();
                cargarRegimenesFiscales();
                
                $('#modalGenerarFactura').modal('show');
            } else {
                toastr.error(response.mensaje);
            }
        },
        error: function () {
            toastr.error('Error al cargar datos de venta');
        }
    });
}

function cargarUsosCFDI() {
    $.ajax({
        url: '/Factura/ObtenerUsosCFDI',
        type: 'GET',
        success: function (response) {
            if (response.success) {
                var options = '<option value="">Seleccione...</option>';
                response.data.forEach(function (uso) {
                    options += '<option value="' + uso.Clave + '">' + uso.Clave + ' - ' + uso.Descripcion + '</option>';
                });
                $('#cboUsoCFDI').html(options);
            }
        }
    });
}

function cargarRegimenesFiscales() {
    $.ajax({
        url: '/Factura/ObtenerRegimenesFiscales',
        type: 'GET',
        success: function (response) {
            if (response.success) {
                var options = '<option value="">Seleccione...</option>';
                response.data.forEach(function (regimen) {
                    options += '<option value="' + regimen.Clave + '">' + regimen.Clave + ' - ' + regimen.Descripcion + '</option>';
                });
                $('#cboRegimenFiscal').html(options);
            }
        }
    });
}

function procesarFacturacion() {
    // Validaciones
    var rfc = $('#txtRFC').val().trim().toUpperCase();
    var nombre = $('#txtNombreReceptor').val().trim();
    var usoCFDI = $('#cboUsoCFDI').val();
    var regimenFiscal = $('#cboRegimenFiscal').val();
    var cp = $('#txtCP').val().trim();
    var email = $('#txtEmailReceptor').val().trim();
    var formaPago = $('#cboFormaPago').val();
    
    if (!rfc) {
        toastr.warning('Ingrese el RFC del receptor');
        $('#txtRFC').focus();
        return;
    }
    
    if (rfc.length < 12 || rfc.length > 13) {
        toastr.warning('RFC inválido (debe tener 12 o 13 caracteres)');
        $('#txtRFC').focus();
        return;
    }
    
    if (!nombre) {
        toastr.warning('Ingrese el nombre del receptor');
        $('#txtNombreReceptor').focus();
        return;
    }
    
    if (!usoCFDI) {
        toastr.warning('Seleccione el uso de CFDI');
        return;
    }
    
    if (!regimenFiscal) {
        toastr.warning('Seleccione el régimen fiscal');
        return;
    }
    
    if (!cp || cp.length !== 5) {
        toastr.warning('Ingrese un código postal válido (5 dígitos)');
        $('#txtCP').focus();
        return;
    }
    
    if (!formaPago) {
        toastr.warning('Seleccione la forma de pago');
        return;
    }
    
    // Email opcional pero si se proporciona debe ser válido
    if (email && !validarEmail(email)) {
        toastr.warning('Ingrese un email válido');
        $('#txtEmailReceptor').focus();
        return;
    }
    
    // Deshabilitar botón y mostrar spinner
    $('#btnGenerarFactura').prop('disabled', true).html('<i class="fas fa-spinner fa-spin"></i> Generando factura...');
    
    var request = {
        VentaID: $('#hdnVentaID').val(),
        ReceptorRFC: rfc,
        ReceptorNombre: nombre,
        ReceptorUsoCFDI: usoCFDI,
        ReceptorCP: cp,
        ReceptorRegimenFiscal: regimenFiscal,
        ReceptorEmail: email,
        FormaPago: formaPago,
        MetodoPago: 'PUE' // Pago en una sola exhibición
    };
    
    $.ajax({
        url: '/Factura/GenerarFactura',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(request),
        success: function (response) {
            $('#btnGenerarFactura').prop('disabled', false).html('<i class="fas fa-file-invoice"></i> Generar Factura');
            
            if (response.success) {
                toastr.success(response.mensaje);
                
                // Mostrar información de la factura generada
                var data = response.data;
                var mensaje = '<strong>Factura generada exitosamente</strong><br>';
                mensaje += 'Serie-Folio: ' + data.Serie + data.Folio + '<br>';
                mensaje += 'UUID: ' + data.UUID + '<br>';
                mensaje += 'Total: $' + parseFloat(data.Total).toFixed(2);
                
                toastr.success(mensaje, 'Factura Timbrada', {
                    timeOut: 10000,
                    closeButton: true,
                    progressBar: true
                });
                
                $('#modalGenerarFactura').modal('hide');
                
                // Opcional: Abrir modal para descargar XML/PDF
                if (confirm('¿Desea descargar el XML de la factura?')) {
                    window.open('/Factura/DescargarXML?facturaId=' + data.FacturaID, '_blank');
                }
            } else {
                toastr.error(response.mensaje, 'Error al Facturar');
            }
        },
        error: function (xhr) {
            $('#btnGenerarFactura').prop('disabled', false).html('<i class="fas fa-file-invoice"></i> Generar Factura');
            
            var mensaje = 'Error al generar factura';
            if (xhr.responseJSON && xhr.responseJSON.mensaje) {
                mensaje = xhr.responseJSON.mensaje;
            }
            toastr.error(mensaje);
        }
    });
}

function validarRFC() {
    var rfc = $('#txtRFC').val().trim().toUpperCase();
    $('#txtRFC').val(rfc);
    
    if (rfc.length === 12 || rfc.length === 13) {
        // RFC válido, buscar si existe en clientes
        $.ajax({
            url: '/Cliente/BuscarPorRFC',
            type: 'GET',
            data: { rfc: rfc },
            success: function (response) {
                if (response.success && response.data) {
                    // Autocompletar datos del cliente
                    $('#txtNombreReceptor').val(response.data.RazonSocial || response.data.Nombre);
                    $('#txtCP').val(response.data.CodigoPostal);
                    $('#txtEmailReceptor').val(response.data.Email);
                    
                    toastr.info('Datos del cliente cargados');
                }
            }
        });
    }
}

function validarEmail(email) {
    var re = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return re.test(email);
}

// Event listeners
$('#txtRFC').on('blur', validarRFC);
