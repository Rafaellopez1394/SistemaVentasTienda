// Scripts/Views/certificado-digital.js
$(document).ready(function () {
    cargarCertificados();
    validarVigencia();

    // Actualizar label de archivos
    $('.custom-file-input').on('change', function () {
        var fileName = $(this).val().split('\\').pop();
        $(this).siblings('.custom-file-label').addClass('selected').html(fileName);
    });

    // Transformar RFC a mayúsculas
    $('#rfc').on('input', function () {
        $(this).val($(this).val().toUpperCase());
    });
});

function cargarCertificados() {
    $.ajax({
        url: '/CertificadoDigital/ObtenerTodos',
        type: 'GET',
        success: function (response) {
            if (response.success) {
                mostrarCertificados(response.data);
            } else {
                toastr.error('Error al cargar certificados');
            }
        },
        error: function () {
            toastr.error('Error de conexión');
        }
    });
}

function mostrarCertificados(certificados) {
    var tbody = $('#tablaCertificados tbody');
    tbody.empty();

    if (certificados.length === 0) {
        tbody.append(`
            <tr>
                <td colspan="7" class="text-center">
                    <i class="fas fa-inbox"></i> No hay certificados registrados<br>
                    <button class="btn btn-sm btn-primary mt-2" onclick="mostrarModalCargar()">
                        <i class="fas fa-plus"></i> Cargar Primer Certificado
                    </button>
                </td>
            </tr>
        `);
        return;
    }

    certificados.forEach(function (cert) {
        var estadoBadge = '';
        if (cert.EstadoVigencia === 'VIGENTE') {
            estadoBadge = '<span class="badge badge-vigente">VIGENTE</span>';
        } else if (cert.EstadoVigencia === 'POR VENCER') {
            estadoBadge = '<span class="badge badge-por-vencer">POR VENCER (' + cert.DiasRestantes + ' días)</span>';
        } else {
            estadoBadge = '<span class="badge badge-vencido">VENCIDO</span>';
        }

        var activoBadge = cert.Activo
            ? '<span class="badge badge-success">Activo</span>'
            : '<span class="badge badge-secondary">Inactivo</span>';

        var predeterminadoBadge = cert.EsPredeterminado
            ? '<i class="fas fa-check-circle text-success" title="Predeterminado"></i> Sí'
            : '<button class="btn btn-sm btn-outline-primary" onclick="establecerPredeterminado(' + cert.CertificadoID + ')">Establecer</button>';

        // Formatear fecha sin moment.js
        var fechaFin = 'Sin fecha';
        if (cert.FechaVigenciaFin) {
            var fecha = new Date(cert.FechaVigenciaFin);
            if (!isNaN(fecha.getTime())) {
                var dia = String(fecha.getDate()).padStart(2, '0');
                var mes = String(fecha.getMonth() + 1).padStart(2, '0');
                var anio = fecha.getFullYear();
                fechaFin = dia + '/' + mes + '/' + anio;
            }
        }

        tbody.append(`
            <tr>
                <td>
                    <strong>${cert.NombreCertificado}</strong><br>
                    <small class="text-muted">${cert.RazonSocial}</small>
                </td>
                <td>${cert.RFC}</td>
                <td><code>${cert.NoCertificado}</code></td>
                <td>${fechaFin}</td>
                <td>${estadoBadge}</td>
                <td class="text-center">${predeterminadoBadge}</td>
                <td class="text-center">
                    <button class="btn btn-sm btn-danger" onclick="eliminarCertificado(${cert.CertificadoID}, '${cert.NombreCertificado}')" 
                            ${cert.EsPredeterminado ? 'disabled' : ''}>
                        <i class="fas fa-trash"></i>
                    </button>
                </td>
            </tr>
        `);
    });
}

function mostrarModalCargar() {
    $('#formCargarCertificado')[0].reset();
    $('.custom-file-label').removeClass('selected').html('Seleccionar archivo');
    $('#modalCargarCertificado').modal('show');
}

function cargarCertificado() {
    var form = $('#formCargarCertificado')[0];
    
    // Validar formulario
    if (!form.checkValidity()) {
        form.reportValidity();
        return;
    }

    var formData = new FormData(form);
    formData.append('esPredeterminado', $('#esPredeterminado').is(':checked'));

    // Mostrar loading
    Swal.fire({
        title: 'Cargando certificado...',
        text: 'Por favor espere',
        allowOutsideClick: false,
        didOpen: () => {
            Swal.showLoading();
        }
    });

    $.ajax({
        url: '/CertificadoDigital/CargarCertificado',
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
                    text: response.mensaje,
                    timer: 2000
                });
                $('#modalCargarCertificado').modal('hide');
                cargarCertificados();
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
                text: 'Error al cargar certificado: ' + (xhr.responseText || 'Error de conexión')
            });
        }
    });
}

function establecerPredeterminado(certificadoID) {
    Swal.fire({
        title: '¿Establecer como predeterminado?',
        text: 'Este certificado se usará para todas las facturas',
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'Sí, establecer',
        cancelButtonText: 'Cancelar'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: '/CertificadoDigital/EstablecerPredeterminado',
                type: 'POST',
                data: { certificadoID: certificadoID },
                success: function (response) {
                    if (response.success) {
                        toastr.success(response.mensaje);
                        cargarCertificados();
                    } else {
                        toastr.error(response.mensaje);
                    }
                },
                error: function () {
                    toastr.error('Error al establecer certificado');
                }
            });
        }
    });
}

function eliminarCertificado(certificadoID, nombre) {
    Swal.fire({
        title: '¿Eliminar certificado?',
        text: 'Se eliminará: ' + nombre,
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#d33',
        confirmButtonText: 'Sí, eliminar',
        cancelButtonText: 'Cancelar'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: '/CertificadoDigital/Eliminar',
                type: 'POST',
                data: { certificadoID: certificadoID },
                success: function (response) {
                    if (response.success) {
                        toastr.success(response.mensaje);
                        cargarCertificados();
                    } else {
                        toastr.error(response.mensaje);
                    }
                },
                error: function () {
                    toastr.error('Error al eliminar certificado');
                }
            });
        }
    });
}

function validarVigencia() {
    $.ajax({
        url: '/CertificadoDigital/ValidarVigencia',
        type: 'GET',
        success: function (response) {
            if (response.success && response.data && response.data.length > 0) {
                var mensaje = 'Hay ' + response.data.length + ' certificado(s) próximo(s) a vencer. Renuévelos antes de su vencimiento.';
                $('#mensajeVigencia').text(mensaje);
                $('#alertaVigencia').show();
            }
        }
    });
}

// ========================================================================
// FUNCIONES PARA FISCALAPI
// ========================================================================

function mostrarModalSubirFiscalAPI() {
    $('#formSubirFiscalAPI')[0].reset();
    $('.custom-file-label').html('Seleccionar archivo');
    $('#modalSubirFiscalAPI').modal('show');
}

function cargarCertificadosFiscalAPI() {
    var tbody = $('#tablaCertificadosFiscalAPI tbody');
    tbody.html('<tr><td colspan="6" class="text-center"><i class="fas fa-spinner fa-spin"></i> Cargando certificados...</td></tr>');

    $.ajax({
        url: '/CertificadoDigital/ListarFiscalAPI',
        type: 'GET',
        success: function (response) {
            if (response.success) {
                $('#ambienteFiscalAPI').text(response.ambiente || 'Desconocido');
                mostrarCertificadosFiscalAPI(response.data);
            } else {
                tbody.html('<tr><td colspan="6" class="text-center text-danger"><i class="fas fa-exclamation-triangle"></i> ' + response.mensaje + '</td></tr>');
            }
        },
        error: function () {
            tbody.html('<tr><td colspan="6" class="text-center text-danger"><i class="fas fa-times"></i> Error al cargar certificados de FiscalAPI</td></tr>');
        }
    });
}

function mostrarCertificadosFiscalAPI(certificados) {
    var tbody = $('#tablaCertificadosFiscalAPI tbody');
    tbody.empty();

    if (!certificados || certificados.length === 0) {
        tbody.append(`
            <tr>
                <td colspan="6" class="text-center">
                    <i class="fas fa-inbox"></i> No hay certificados en FiscalAPI<br>
                    <button class="btn btn-sm btn-success mt-2" onclick="mostrarModalSubirFiscalAPI()">
                        <i class="fas fa-cloud-upload-alt"></i> Subir Primer Certificado
                    </button>
                </td>
            </tr>
        `);
        return;
    }

    certificados.forEach(function (cert) {
        var validFrom = cert.validFrom ? new Date(cert.validFrom).toLocaleDateString('es-MX') : 'N/A';
        var validTo = cert.validTo ? new Date(cert.validTo).toLocaleDateString('es-MX') : 'N/A';
        
        var estadoBadge = '<span class="badge badge-success">Activo</span>';
        if (cert.status && cert.status !== 'Active') {
            estadoBadge = '<span class="badge badge-secondary">' + cert.status + '</span>';
        }

        var row = `
            <tr>
                <td><strong>${cert.tin || cert.rfc || 'N/A'}</strong></td>
                <td>${cert.legalName || cert.razonSocial || 'N/A'}</td>
                <td>${validFrom}</td>
                <td>${validTo}</td>
                <td>${estadoBadge}</td>
                <td>
                    <button class="btn btn-sm btn-danger" onclick="eliminarCertificadoFiscalAPI('${cert.id}', '${cert.tin || cert.rfc}')">
                        <i class="fas fa-trash"></i>
                    </button>
                </td>
            </tr>
        `;
        tbody.append(row);
    });
}

function subirCertificadoFiscalAPI() {
    var form = $('#formSubirFiscalAPI')[0];
    
    if (!form.checkValidity()) {
        form.reportValidity();
        return;
    }

    var formData = new FormData(form);

    // Mostrar loading
    Swal.fire({
        title: 'Subiendo certificado...',
        html: 'Por favor espere. Esto puede tomar unos segundos.',
        allowOutsideClick: false,
        didOpen: () => {
            Swal.showLoading();
        }
    });

    $.ajax({
        url: '/CertificadoDigital/SubirFiscalAPI',
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
                    text: response.mensaje,
                    showConfirmButton: true
                }).then(() => {
                    $('#modalSubirFiscalAPI').modal('hide');
                    cargarCertificadosFiscalAPI();
                });
            } else {
                Swal.fire({
                    icon: 'error',
                    title: 'Error al subir certificado',
                    html: '<p>' + response.mensaje + '</p>' +
                          (response.detalle ? '<details><summary>Detalles técnicos</summary><pre>' + JSON.stringify(JSON.parse(response.detalle), null, 2) + '</pre></details>' : ''),
                    width: '600px'
                });
            }
        },
        error: function (xhr) {
            Swal.close();
            var errorMsg = 'Error de conexión al subir certificado';
            
            try {
                var errorResponse = JSON.parse(xhr.responseText);
                errorMsg = errorResponse.mensaje || errorMsg;
            } catch (e) { }
            
            Swal.fire({
                icon: 'error',
                title: 'Error',
                text: errorMsg
            });
        }
    });
}

function eliminarCertificadoFiscalAPI(id, rfc) {
    Swal.fire({
        title: '¿Eliminar certificado?',
        html: 'Se eliminará el certificado <strong>' + rfc + '</strong> de FiscalAPI.<br>Esta acción no se puede deshacer.',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#d33',
        cancelButtonColor: '#3085d6',
        confirmButtonText: 'Sí, eliminar',
        cancelButtonText: 'Cancelar'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: '/CertificadoDigital/EliminarFiscalAPI',
                type: 'POST',
                data: { id: id },
                success: function (response) {
                    if (response.success) {
                        Swal.fire({
                            icon: 'success',
                            title: 'Eliminado',
                            text: response.mensaje,
                            timer: 2000
                        });
                        cargarCertificadosFiscalAPI();
                    } else {
                        Swal.fire({
                            icon: 'error',
                            title: 'Error',
                            text: response.mensaje
                        });
                    }
                },
                error: function () {
                    Swal.fire({
                        icon: 'error',
                        title: 'Error',
                        text: 'Error al eliminar certificado'
                    });
                }
            });
        }
    });
}

function mostrarInfoCertificadosPrueba() {
    $('#modalInfoPrueba').modal('show');
}

// Exponer funciones al scope global para los handlers inline
window.mostrarModalSubirFiscalAPI = mostrarModalSubirFiscalAPI;
window.cargarCertificadosFiscalAPI = cargarCertificadosFiscalAPI;
window.mostrarInfoCertificadosPrueba = mostrarInfoCertificadosPrueba;
window.mostrarModalCargar = mostrarModalCargar;
window.cargarCertificado = cargarCertificado;
window.subirCertificadoFiscalAPI = subirCertificadoFiscalAPI;
window.eliminarCertificadoFiscalAPI = eliminarCertificadoFiscalAPI;
window.eliminarCertificado = eliminarCertificado;
window.establecerPredeterminado = establecerPredeterminado;
window.cargarCertificados = cargarCertificados;
window.mostrarCertificados = mostrarCertificados;
window.validarVigencia = validarVigencia;
