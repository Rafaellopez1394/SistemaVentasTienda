// ============================================================================
// CONFIGURACION.JS - Script para el modulo de configuracion
// ============================================================================

$(document).ready(function () {
    // Cargar datos iniciales
    cargarDatosNegocio();
    cargarImpresoras();
    cargarUsuarios();
    cargarRoles();
    
    // Navegacion del menu
    $('#menuConfiguracion a').click(function (e) {
        e.preventDefault();
        
        $('#menuConfiguracion a').removeClass('active');
        $(this).addClass('active');
        
        $('.seccion-config').addClass('d-none');
        
        var seccion = $(this).data('seccion');
        $('#seccion' + seccion.charAt(0).toUpperCase() + seccion.slice(1)).removeClass('d-none');
    });
});

// ============================================================================
// DATOS DEL NEGOCIO
// ============================================================================
function cargarDatosNegocio() {
    $.get('/Configuracion/ObtenerConfigGeneralJSON', function (res) {
        if (res.success && res.data) {
            $('#configID').val(res.data.ConfigID || 0);
            $('#txtNombreNegocio').val(res.data.NombreNegocio || '');
            $('#txtRFC').val(res.data.RFC || '');
            $('#txtDireccion').val(res.data.Direccion || '');
            $('#txtTelefono').val(res.data.Telefono || '');
            $('#txtEmail').val(res.data.Email || '');
            
            // Cargar tambien datos de tickets
            $('#txtMensajeTicket').val(res.data.MensajeTicket || '');
            $('#chkImprimirAuto').prop('checked', res.data.ImprimirTicketAutomatico);
            $('#chkMostrarLogo').prop('checked', res.data.MostrarLogoEnTicket);
        }
    }).fail(function() {
        console.log('No se pudo cargar configuracion');
    });
}

function guardarDatosNegocio() {
    var config = {
        ConfigID: parseInt($('#configID').val()) || 0,
        NombreNegocio: $('#txtNombreNegocio').val(),
        RFC: $('#txtRFC').val(),
        Direccion: $('#txtDireccion').val(),
        Telefono: $('#txtTelefono').val(),
        Email: $('#txtEmail').val(),
        // Incluir tambien los datos de ticket para no perderlos
        MensajeTicket: $('#txtMensajeTicket').val() || '',
        ImprimirTicketAutomatico: $('#chkImprimirAuto').is(':checked'),
        MostrarLogoEnTicket: $('#chkMostrarLogo').is(':checked')
    };
    
    if (!config.NombreNegocio) {
        toastr.warning('El nombre del negocio es requerido');
        return;
    }
    
    $.ajax({
        url: '/Configuracion/GuardarConfigGeneral',
        type: 'POST',
        data: config,
        success: function (res) {
            if (res.success) {
                toastr.success('Datos guardados correctamente');
                // Recargar para obtener el ConfigID si es nuevo
                cargarDatosNegocio();
            } else {
                toastr.error(res.mensaje || 'Error al guardar');
            }
        },
        error: function () {
            toastr.error('Error de conexion');
        }
    });
}

// ============================================================================
// IMPRESORAS
// ============================================================================

function cargarImpresoras() {
    $.get('/Configuracion/ListarImpresorasSistema', function (res) {
        if (res.success && res.data) {
            var $combo = $('#cboImpresoraTicket');
            $combo.empty().append('<option value="">-- Seleccione --</option>');
            
            res.data.forEach(function (impresora) {
                $combo.append('<option value="' + impresora + '">' + impresora + '</option>');
            });
            
            // Cargar impresora configurada
            $.get('/Configuracion/ObtenerImpresoraTickets', function (resConfig) {
                if (resConfig.success && resConfig.data) {
                    $combo.val(resConfig.data.NombreImpresora);
                    $('#cboAnchoTicket').val(resConfig.data.AnchoPapel);
                }
            });
        }
    });
}

function guardarImpresoraTicket() {
    var config = {
        TipoImpresion: 'TICKET',
        NombreImpresora: $('#cboImpresoraTicket').val(),
        AnchoPapel: parseInt($('#cboAnchoTicket').val()),
        Activo: true
    };
    
    if (!config.NombreImpresora) {
        toastr.warning('Seleccione una impresora');
        return;
    }
    
    $.ajax({
        url: '/Configuracion/GuardarImpresora',
        type: 'POST',
        data: config,
        success: function (res) {
            if (res.success) {
                toastr.success('Impresora configurada correctamente');
            } else {
                toastr.error(res.mensaje || 'Error al guardar');
            }
        },
        error: function () {
            toastr.error('Error de conexion');
        }
    });
}

function imprimirTicketPrueba() {
    // Cargar datos reales del negocio
    $.get('/Configuracion/ObtenerConfigGeneralJSON', function (res) {
        var config = res.success ? res.data : {};
        
        var datosTicket = {
            negocio: config.NombreNegocio || 'MI TIENDA',
            rfc: config.RFC || '',
            direccion: config.Direccion || '',
            telefono: config.Telefono || '',
            folio: 'PRUEBA-' + Math.floor(Math.random() * 1000),
            fecha: new Date().toLocaleString('es-MX'),
            cliente: 'PUBLICO GENERAL',
            productos: [
                { nombre: 'COCA COLA 600ML', cantidad: 2, precio: 18.00, subtotal: 36.00 },
                { nombre: 'SABRITAS ORIGINAL', cantidad: 3, precio: 22.50, subtotal: 67.50 },
                { nombre: 'GALLETAS MARIAS', cantidad: 1, precio: 15.00, subtotal: 15.00 }
            ],
            subtotal: 102.16,
            iva: 16.34,
            total: 118.50,
            efectivo: 150.00,
            cambio: 31.50,
            mensaje: config.MensajeTicket || 'Gracias por su compra'
        };
        
        var contenidoHTML = generarTicketHTMLConfig(datosTicket);
        mostrarVistaPrevia(contenidoHTML);
        
    }).fail(function() {
        toastr.error('No se pudo cargar la configuracion del negocio');
    });
}

// Generar HTML del ticket para configuracion - Optimizado para 58mm
function generarTicketHTMLConfig(datos) {
    var linea = '--------------------------------';
    var html = '';
    
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
    html += '<div>Cliente: ' + datos.cliente + '</div>';
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

// Mostrar vista previa del ticket - Optimizado para impresora 58mm JP58H
function mostrarVistaPrevia(contenidoHTML) {
    var estilos = `
        <style>
            * { margin: 0; padding: 0; box-sizing: border-box; }
            body { 
                font-family: 'Courier New', Courier, monospace; 
                background: #f0f0f0;
                padding: 15px;
            }
            .ticket-container {
                width: 58mm;
                max-width: 220px;
                margin: 0 auto;
                background: white;
                padding: 8px;
                box-shadow: 0 2px 10px rgba(0,0,0,0.2);
                font-size: 12px;
                line-height: 1.4;
            }
            .ticket-header {
                text-align: center;
                margin-bottom: 6px;
            }
            .negocio-nombre {
                font-size: 16px;
                font-weight: bold;
                margin-bottom: 3px;
            }
            .direccion {
                font-size: 11px;
            }
            .linea {
                text-align: center;
                font-size: 10px;
                color: #666;
                margin: 6px 0;
            }
            .ticket-info {
                margin: 6px 0;
                font-size: 12px;
            }
            .titulo-venta {
                font-size: 14px;
                font-weight: bold;
                text-align: center;
                margin-bottom: 6px;
            }
            .productos-lista {
                margin: 6px 0;
            }
            .producto-item {
                margin: 5px 0;
                font-size: 12px;
            }
            .producto-nombre {
                font-weight: normal;
            }
            .producto-detalle {
                display: flex;
                justify-content: space-between;
            }
            .producto-total {
                font-weight: bold;
            }
            .totales {
                margin: 6px 0;
                font-size: 12px;
            }
            .total-row {
                display: flex;
                justify-content: space-between;
                margin: 3px 0;
            }
            .total-final {
                font-size: 16px;
                font-weight: bold;
                border-top: 1px solid #000;
                padding-top: 4px;
                margin-top: 4px;
            }
            .pago-info {
                margin-top: 6px;
                border-top: 1px dashed #000;
                padding-top: 4px;
            }
            .ticket-footer {
                text-align: center;
                margin-top: 8px;
                font-size: 12px;
            }
            .conserve {
                margin-top: 6px;
                font-size: 11px;
                color: #000;
                padding-bottom: 25px;
            }
            .btn-imprimir {
                display: block;
                width: 100%;
                max-width: 220px;
                margin: 15px auto;
                padding: 10px;
                background: #28a745;
                color: white;
                border: none;
                border-radius: 5px;
                font-size: 14px;
                cursor: pointer;
            }
            .btn-imprimir:hover {
                background: #218838;
            }
            @media print {
                body { background: white; padding: 0; }
                .btn-imprimir { display: none; }
                .ticket-container { box-shadow: none; max-width: none; }
                @page { size: 58mm auto; margin: 0; }
            }
        </style>
    `;
    
    var ventana = window.open('', 'TICKET_PREVIEW', 'width=320,height=550,scrollbars=yes');
    ventana.document.write('<!DOCTYPE html><html><head>');
    ventana.document.write('<meta charset="UTF-8">');
    ventana.document.write('<title>Vista Previa - Ticket 58mm</title>');
    ventana.document.write(estilos);
    ventana.document.write('</head><body>');
    ventana.document.write(contenidoHTML);
    ventana.document.write('<button class="btn-imprimir" onclick="window.print()">IMPRIMIR TICKET</button>');
    ventana.document.write('</body></html>');
    ventana.document.close();
}

// ============================================================================
// USUARIOS
// ============================================================================
function cargarUsuarios() {
    console.log('🔍 Iniciando carga de usuarios...');
    
    $.get('/Usuario/Listar')
        .done(function (res) {
            console.log('✅ Respuesta recibida de /Usuario/Listar:', res);
            console.log('   - Tipo de dato:', typeof res);
            console.log('   - Es array?', Array.isArray(res));
            console.log('   - Cantidad de elementos:', res ? res.length : 0);
            
            var tbody = $('#tblUsuarios tbody');
            tbody.empty();
            
            if (res && res.length > 0) {
                console.log('📋 Procesando ' + res.length + ' usuarios');
                res.forEach(function (u) {
                    var estado = u.Activo !== false ? 
                        '<span class="badge badge-success">Activo</span>' : 
                        '<span class="badge badge-danger">Inactivo</span>';
                    
                    // Nombre de usuario: usar Correo como identificador si NombreUsuario es null
                    var nombreUsuario = u.NombreUsuario || u.Correo || 'N/A';
                    
                    // Nombre completo
                    var nombreCompleto = ((u.Nombres || '') + ' ' + (u.Apellidos || '')).trim() || 'Sin nombre';
                    
                    // Rol desde el objeto oRol
                    var rolDescripcion = (u.oRol && u.oRol.Descripcion) ? u.oRol.Descripcion : 'Sin rol';
                    
                    // Ultimo acceso - no existe en el modelo actual
                    var ultimoAcceso = 'N/A';
                        
                    tbody.append(
                        '<tr>' +
                        '<td>' + nombreUsuario + '</td>' +
                        '<td>' + nombreCompleto + '</td>' +
                        '<td><span class="badge badge-info">' + rolDescripcion + '</span></td>' +
                        '<td>' + estado + '</td>' +
                        '<td>' + ultimoAcceso + '</td>' +
                        '<td>' +
                            '<button class="btn btn-sm btn-primary mr-1" onclick="editarUsuario(' + u.UsuarioID + ')" title="Editar"><i class="fas fa-edit"></i></button>' +
                            '<button class="btn btn-sm btn-warning" onclick="toggleUsuario(' + u.UsuarioID + ', ' + (u.Activo !== false) + ')" title="Cambiar estado"><i class="fas fa-power-off"></i></button>' +
                        '</td>' +
                        '</tr>'
                    );
                });
                console.log('✅ Tabla de usuarios actualizada correctamente');
            } else {
                console.warn('⚠️ No hay usuarios en la respuesta');
                tbody.append('<tr><td colspan="6" class="text-center">No hay usuarios registrados</td></tr>');
            }
        })
        .fail(function (xhr, status, error) {
            console.error('❌ Error al cargar usuarios:');
            console.error('   - Status:', status);
            console.error('   - Error:', error);
            console.error('   - Código HTTP:', xhr.status);
            console.error('   - Respuesta del servidor:', xhr.responseText);
            
            var tbody = $('#tblUsuarios tbody');
            tbody.empty();
            tbody.append('<tr><td colspan="6" class="text-center text-danger">Error al cargar usuarios. Revise la consola para más detalles.</td></tr>');
        });
}

function cargarRoles() {
    console.log('🔍 Iniciando carga de roles...');
    
    $.get('/Rol/Listar')
        .done(function (res) {
            console.log('✅ Respuesta recibida de /Rol/Listar:', res);
            console.log('   - Tipo de dato:', typeof res);
            console.log('   - Es array?', Array.isArray(res));
            console.log('   - Cantidad de elementos:', res ? res.length : 0);
            
            var $combo = $('#cboRol');
            $combo.empty().append('<option value="">-- Seleccione --</option>');
            
            if (res && res.length > 0) {
                console.log('📋 Agregando ' + res.length + ' roles al combo');
                res.forEach(function (r) {
                    $combo.append('<option value="' + r.RolID + '">' + r.Descripcion + '</option>');
                });
                console.log('✅ Combo de roles actualizado correctamente');
            } else {
                console.warn('⚠️ No hay roles en la respuesta');
            }
        })
        .fail(function (xhr, status, error) {
            console.error('❌ Error al cargar roles:');
            console.error('   - Status:', status);
            console.error('   - Error:', error);
            console.error('   - Código HTTP:', xhr.status);
            console.error('   - Respuesta del servidor:', xhr.responseText);
        });
}

function mostrarModalUsuario(id) {
    $('#usuarioID').val(0);
    $('#txtUsuario').val('').prop('disabled', false);
    $('#txtNombreCompleto').val('');
    $('#txtPassword').val('');
    $('#cboRol').val('');
    $('#chkUsuarioActivo').prop('checked', true);
    $('#modalUsuarioTitle').text('Nuevo Usuario');
    $('#lblPassReq').show();
    $('#txtPassHelp').text('Minimo 6 caracteres');
    
    $('#modalUsuario').modal('show');
}

function editarUsuario(id) {
    $.get('/Usuario/Obtener', { id: id }, function (res) {
        if (res) {
            $('#usuarioID').val(res.UsuarioID);
            $('#txtUsuario').val(res.Correo || res.NombreUsuario || '').prop('disabled', true);
            $('#txtNombreCompleto').val(((res.Nombres || '') + ' ' + (res.Apellidos || '')).trim());
            $('#txtPassword').val('');
            $('#cboRol').val(res.RolID);
            $('#chkUsuarioActivo').prop('checked', res.Activo !== false);
            $('#modalUsuarioTitle').text('Editar Usuario');
            $('#lblPassReq').hide();
            $('#txtPassHelp').text('Dejar vacio para mantener la actual');
            
            $('#modalUsuario').modal('show');
        } else {
            toastr.error('No se pudo cargar el usuario');
        }
    });
}

function guardarUsuario() {
    var id = parseInt($('#usuarioID').val());
    
    // Separar nombre completo en Nombres y Apellidos
    var nombreCompleto = $('#txtNombreCompleto').val().trim();
    var partes = nombreCompleto.split(' ');
    var nombres = partes[0] || '';
    var apellidos = partes.slice(1).join(' ') || '';
    
    var usuario = {
        UsuarioID: id,
        Correo: $('#txtUsuario').val(),
        Nombres: nombres,
        Apellidos: apellidos,
        Clave: $('#txtPassword').val(),
        RolID: parseInt($('#cboRol').val()),
        SucursalID: 1, // Por defecto sucursal 1
        Activo: $('#chkUsuarioActivo').is(':checked')
    };
    
    if (!usuario.Correo) {
        toastr.warning('El correo es requerido');
        return;
    }
    
    if (!usuario.Nombres) {
        toastr.warning('El nombre es requerido');
        return;
    }
    
    if (id === 0 && (!usuario.Clave || usuario.Clave.length < 6)) {
        toastr.warning('La contrasena debe tener al menos 6 caracteres');
        return;
    }
    
    if (!usuario.RolID) {
        toastr.warning('Seleccione un rol');
        return;
    }
    
    $.ajax({
        url: '/Usuario/Guardar',
        type: 'POST',
        data: usuario,
        success: function (res) {
            if (res.resultado) {
                toastr.success('Usuario guardado correctamente');
                $('#modalUsuario').modal('hide');
                cargarUsuarios();
            } else {
                toastr.error('Error al guardar usuario');
            }
        },
        error: function () {
            toastr.error('Error de conexion');
        }
    });
}

function toggleUsuario(id, activo) {
    var accion = activo ? 'inhabilitar' : 'habilitar';
    
    if (!confirm('¿Desea ' + accion + ' este usuario?')) return;
    
    $.post('/Usuario/CambiarEstado', { id: id, activo: !activo }, function (res) {
        if (res.valor) {
            toastr.success('Estado actualizado');
            cargarUsuarios();
        } else {
            toastr.error(res.msg || 'Error al actualizar');
        }
    });
}

// ============================================================================
// CONFIGURACION DE TICKETS
// ============================================================================
function guardarConfigTickets() {
    var datos = {
        mensajeTicket: $('#txtMensajeTicket').val() || '',
        imprimirAuto: $('#chkImprimirAuto').is(':checked'),
        mostrarLogo: $('#chkMostrarLogo').is(':checked')
    };
    
    $.ajax({
        url: '/Configuracion/GuardarConfigTickets',
        type: 'POST',
        data: datos,
        success: function (res) {
            if (res.success) {
                toastr.success('Configuracion de tickets guardada');
            } else {
                toastr.error(res.mensaje || 'Error al guardar');
            }
        },
        error: function () {
            toastr.error('Error de conexion');
        }
    });
}

// ============================================================================
// GENERACION E IMPRESION DE TICKETS
// ============================================================================
function generarTicketHTML(datos) {
    var linea = '================================';
    var html = '<div style="font-family: monospace; font-size: 12px; width: 280px; padding: 10px;">';
    
    // Encabezado
    html += '<div style="text-align: center;">';
    html += '<strong style="font-size: 14px;">' + (datos.negocio || 'MI TIENDA') + '</strong><br>';
    if (datos.rfc) html += 'RFC: ' + datos.rfc + '<br>';
    if (datos.direccion) html += datos.direccion + '<br>';
    if (datos.telefono) html += 'Tel: ' + datos.telefono + '<br>';
    html += '</div>';
    
    html += '<div style="text-align: center;">' + linea + '</div>';
    
    // Datos de venta
    html += '<div>';
    html += '<strong>TICKET DE VENTA</strong><br>';
    html += 'Folio: ' + datos.folio + '<br>';
    html += 'Fecha: ' + datos.fecha + '<br>';
    html += 'Cliente: ' + datos.cliente + '<br>';
    html += '</div>';
    
    html += '<div style="text-align: center;">' + linea + '</div>';
    
    // Productos
    html += '<table style="width: 100%; font-size: 11px;">';
    html += '<tr><th style="text-align: left;">Producto</th><th>Cant</th><th style="text-align: right;">Importe</th></tr>';
    
    datos.productos.forEach(function (p) {
        html += '<tr>';
        html += '<td style="text-align: left;">' + p.nombre.substring(0, 15) + '</td>';
        html += '<td style="text-align: center;">' + p.cantidad + '</td>';
        html += '<td style="text-align: right;">$' + p.subtotal.toFixed(2) + '</td>';
        html += '</tr>';
    });
    
    html += '</table>';
    
    html += '<div style="text-align: center;">' + linea + '</div>';
    
    // Totales
    html += '<div style="text-align: right;">';
    html += 'Subtotal: $' + datos.subtotal.toFixed(2) + '<br>';
    html += 'IVA: $' + datos.iva.toFixed(2) + '<br>';
    html += '<strong style="font-size: 14px;">TOTAL: $' + datos.total.toFixed(2) + '</strong><br>';
    
    if (datos.efectivo) {
        html += '<br>Efectivo: $' + datos.efectivo.toFixed(2) + '<br>';
        html += 'Cambio: $' + datos.cambio.toFixed(2) + '<br>';
    }
    html += '</div>';
    
    html += '<div style="text-align: center;">' + linea + '</div>';
    
    // Pie
    html += '<div style="text-align: center; margin-top: 10px;">';
    html += (datos.mensaje || 'Gracias por su compra') + '<br>';
    html += '</div>';
    
    html += '</div>';
    
    return html;
}

function imprimirTicket(contenidoHTML) {
    var ventana = window.open('', 'TICKET', 'width=320,height=600');
    ventana.document.write('<html><head><title>Ticket</title>');
    ventana.document.write('<style>@media print { body { margin: 0; } }</style>');
    ventana.document.write('</head><body>');
    ventana.document.write(contenidoHTML);
    ventana.document.write('</body></html>');
    ventana.document.close();
    
    setTimeout(function () {
        ventana.print();
        ventana.close();
    }, 250);
}
