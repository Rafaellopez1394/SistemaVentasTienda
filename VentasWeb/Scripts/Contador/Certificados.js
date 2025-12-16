// =====================================================
// CERTIFICADOS DIGITALES
// =====================================================

var tablaCertificados;

$(document).ready(function () {
    cargarCertificados();
    configurarEventos();
});

// Cargar certificados en DataTable
function cargarCertificados() {
    if ($.fn.DataTable.isDataTable('#tablaCertificados')) {
        $('#tablaCertificados').DataTable().destroy();
    }

    tablaCertificados = $('#tablaCertificados').DataTable({
        ajax: {
            url: '/Contador/ObtenerCertificados',
            type: 'GET',
            dataSrc: 'data'
        },
        columns: [
            { 
                data: 'TipoCertificado',
                render: function (data) {
                    var badge = data === 'CSD' ? 'badge-info' : 'badge-warning';
                    return '<span class="badge ' + badge + '">' + data + '</span>';
                }
            },
            { data: 'NombreCertificado' },
            { data: 'NoCertificado' },
            { data: 'RFC' },
            { 
                data: 'RazonSocial',
                render: function (data) {
                    return data || '<em class="text-muted">N/A</em>';
                }
            },
            {
                data: 'FechaVencimiento',
                render: function (data, type, row) {
                    if (!data) return '<em class="text-muted">N/A</em>';
                    
                    var html = data;
                    
                    // Advertencia si está próximo a vencer (30 días)
                    if (row.DiasParaVencer <= 30 && row.DiasParaVencer > 0) {
                        html += ' <span class="badge badge-warning" title="Vence en ' + row.DiasParaVencer + ' días">' +
                                '<i class="fas fa-exclamation-triangle"></i> ' + row.DiasParaVencer + 'd</span>';
                    }
                    
                    return html;
                }
            },
            {
                data: null,
                render: function (data, type, row) {
                    var usos = [];
                    if (row.UsarParaFacturas) usos.push('Facturas');
                    if (row.UsarParaNomina) usos.push('Nómina');
                    if (row.UsarParaCancelaciones) usos.push('Cancelaciones');
                    
                    return usos.length > 0 ? usos.join(', ') : '<em class="text-muted">Ninguno</em>';
                }
            },
            {
                data: 'Estado',
                render: function (data, type, row) {
                    var html = '<span class="badge ' + row.ClaseEstado + '">' + data + '</span>';
                    if (row.EsPredeterminado) {
                        html += ' <span class="badge badge-primary" title="Predeterminado"><i class="fas fa-star"></i></span>';
                    }
                    return html;
                }
            },
            {
                data: 'EstadoVigencia',
                render: function (data, type, row) {
                    return '<span class="badge ' + row.ClaseVigencia + '">' + data + '</span>';
                }
            },
            {
                data: null,
                orderable: false,
                render: function (data, type, row) {
                    var botones = '';
                    
                    if (row.Activo && row.EstaVigente) {
                        if (!row.EsPredeterminado) {
                            botones += '<button class="btn btn-sm btn-warning mr-1" onclick="establecerPredeterminado(' + row.CertificadoID + ')" ' +
                                      'title="Establecer como predeterminado">' +
                                      '<i class="fas fa-star"></i></button>';
                        }
                        botones += '<button class="btn btn-sm btn-secondary mr-1" onclick="desactivarCertificado(' + row.CertificadoID + ')" ' +
                                  'title="Desactivar">' +
                                  '<i class="fas fa-ban"></i></button>';
                    } else if (!row.Activo) {
                        botones += '<button class="btn btn-sm btn-success mr-1" onclick="activarCertificado(' + row.CertificadoID + ')" ' +
                                  'title="Activar">' +
                                  '<i class="fas fa-check"></i></button>';
                    }
                    
                    botones += '<button class="btn btn-sm btn-danger" onclick="eliminarCertificado(' + row.CertificadoID + ')" ' +
                              'title="Eliminar">' +
                              '<i class="fas fa-trash"></i></button>';
                    
                    return botones;
                }
            }
        ],
        language: {
            url: '//cdn.datatables.net/plug-ins/1.10.24/i18n/Spanish.json'
        },
        order: [[5, 'desc']], // Ordenar por fecha vencimiento
        drawCallback: function () {
            verificarVencimientos();
        }
    });
}

// Configurar eventos
function configurarEventos() {
    // Submit del formulario
    $('#formSubirCertificado').on('submit', function (e) {
        e.preventDefault();
        subirCertificado();
    });
}

// Subir certificado
function subirCertificado() {
    var formData = new FormData();
    
    // Agregar datos del formulario
    formData.append('TipoCertificado', $('#tipoCertificado').val());
    formData.append('NombreCertificado', $('#nombreCertificado').val());
    formData.append('PasswordKEY', $('#passwordKEY').val());
    formData.append('UsarParaFacturas', $('#usarParaFacturas').is(':checked'));
    formData.append('UsarParaNomina', $('#usarParaNomina').is(':checked'));
    formData.append('UsarParaCancelaciones', $('#usarParaCancelaciones').is(':checked'));
    formData.append('EsPredeterminado', $('#esPredeterminado').is(':checked'));
    
    // Agregar archivos
    var archivoCER = $('#archivoCER')[0].files[0];
    var archivoKEY = $('#archivoKEY')[0].files[0];
    
    if (archivoCER) {
        formData.append('archivoCER', archivoCER);
    }
    
    if (archivoKEY) {
        formData.append('archivoKEY', archivoKEY);
    }
    
    // Mostrar loading
    Swal.fire({
        title: 'Procesando certificado...',
        text: 'Por favor espere',
        allowOutsideClick: false,
        didOpen: () => {
            Swal.showLoading();
        }
    });
    
    // Enviar al servidor
    $.ajax({
        url: '/Contador/SubirCertificado',
        type: 'POST',
        data: formData,
        processData: false,
        contentType: false,
        success: function (response) {
            Swal.close();
            
            if (response.success) {
                Swal.fire({
                    icon: 'success',
                    title: '¡Éxito!',
                    html: '<strong>Certificado guardado correctamente</strong><br><br>' +
                          'No. Certificado: ' + response.certificado.NoCertificado + '<br>' +
                          'RFC: ' + response.certificado.RFC + '<br>' +
                          'Vence: ' + response.certificado.FechaVencimiento,
                    confirmButtonText: 'Aceptar'
                });
                
                // Cerrar modal y recargar tabla
                $('#modalSubirCertificado').modal('hide');
                $('#formSubirCertificado')[0].reset();
                $('.custom-file-label').removeClass('selected').html('Seleccionar archivo');
                tablaCertificados.ajax.reload();
            } else {
                Swal.fire({
                    icon: 'error',
                    title: 'Error',
                    text: response.mensaje
                });
            }
        },
        error: function () {
            Swal.close();
            Swal.fire({
                icon: 'error',
                title: 'Error',
                text: 'Error al conectar con el servidor'
            });
        }
    });
}

// Establecer certificado como predeterminado
function establecerPredeterminado(certificadoID) {
    Swal.fire({
        title: '¿Establecer como predeterminado?',
        text: 'Este certificado se usará por defecto para timbrar',
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'Sí, establecer',
        cancelButtonText: 'Cancelar'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: '/Contador/ActivarCertificado',
                type: 'POST',
                data: { 
                    certificadoID: certificadoID,
                    esPredeterminado: true
                },
                success: function (response) {
                    if (response.success) {
                        Swal.fire('¡Listo!', response.mensaje, 'success');
                        tablaCertificados.ajax.reload();
                    } else {
                        Swal.fire('Error', response.mensaje, 'error');
                    }
                }
            });
        }
    });
}

// Activar certificado
function activarCertificado(certificadoID) {
    Swal.fire({
        title: '¿Activar certificado?',
        text: 'El certificado estará disponible para uso',
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'Sí, activar',
        cancelButtonText: 'Cancelar'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: '/Contador/ActivarCertificado',
                type: 'POST',
                data: { certificadoID: certificadoID },
                success: function (response) {
                    if (response.success) {
                        Swal.fire('¡Activado!', response.mensaje, 'success');
                        tablaCertificados.ajax.reload();
                    } else {
                        Swal.fire('Error', response.mensaje, 'error');
                    }
                }
            });
        }
    });
}

// Desactivar certificado
function desactivarCertificado(certificadoID) {
    Swal.fire({
        title: '¿Desactivar certificado?',
        text: 'El certificado no se podrá usar hasta que lo actives nuevamente',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: 'Sí, desactivar',
        cancelButtonText: 'Cancelar'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: '/Contador/DesactivarCertificado',
                type: 'POST',
                data: { certificadoID: certificadoID },
                success: function (response) {
                    if (response.success) {
                        Swal.fire('¡Desactivado!', response.mensaje, 'success');
                        tablaCertificados.ajax.reload();
                    } else {
                        Swal.fire('Error', response.mensaje, 'error');
                    }
                }
            });
        }
    });
}

// Eliminar certificado
function eliminarCertificado(certificadoID) {
    Swal.fire({
        title: '¿Eliminar certificado?',
        text: 'Esta acción no se puede deshacer',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: 'Sí, eliminar',
        cancelButtonText: 'Cancelar',
        confirmButtonColor: '#d33'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: '/Contador/EliminarCertificado',
                type: 'POST',
                data: { certificadoID: certificadoID },
                success: function (response) {
                    if (response.success) {
                        Swal.fire('¡Eliminado!', response.mensaje, 'success');
                        tablaCertificados.ajax.reload();
                    } else {
                        Swal.fire('Error', response.mensaje, 'error');
                    }
                }
            });
        }
    });
}

// Verificar vencimientos próximos
function verificarVencimientos() {
    var data = tablaCertificados.rows().data();
    var certificadosProximos = [];
    
    for (var i = 0; i < data.length; i++) {
        var cert = data[i];
        if (cert.Activo && cert.DiasParaVencer <= 30 && cert.DiasParaVencer > 0) {
            certificadosProximos.push(cert);
        }
    }
    
    if (certificadosProximos.length > 0) {
        var mensaje = 'Tiene ' + certificadosProximos.length + ' certificado(s) próximo(s) a vencer:<br>';
        certificadosProximos.forEach(function (cert) {
            mensaje += '• ' + cert.NombreCertificado + ' (vence en ' + cert.DiasParaVencer + ' días)<br>';
        });
        
        $('#mensajeVencimiento').html(mensaje);
        $('#alertaVencimiento').show();
    } else {
        $('#alertaVencimiento').hide();
    }
}
