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

        var fechaFin = moment(cert.FechaVigenciaFin).format('DD/MM/YYYY');

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
