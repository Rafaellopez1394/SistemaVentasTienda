var tablaNominas;
var nominaSeleccionada = null;

$(document).ready(function () {
    cargarNominas();

    $('#btnBuscar').click(function () {
        tablaNominas.ajax.reload();
    });
});

function cargarNominas() {
    tablaNominas = $('#tbNominas').DataTable({
        ajax: {
            url: '/Nomina/ObtenerNominas',
            type: 'GET',
            dataSrc: 'data'
        },
        columns: [
            { data: 'Folio' },
            { data: 'Periodo' },
            {
                data: 'FechaPago',
                render: function (data) {
                    if (!data) return 'N/A';
                    if (data.toString().includes('/Date(')) {
                        var timestamp = parseInt(data.replace(/\/Date\((\d+)\)\//, '$1'));
                        var fecha = new Date(timestamp);
                        var dia = ('0' + fecha.getDate()).slice(-2);
                        var mes = ('0' + (fecha.getMonth() + 1)).slice(-2);
                        var anio = fecha.getFullYear();
                        return dia + '/' + mes + '/' + anio;
                    }
                    return new Date(data).toLocaleDateString('es-MX');
                }
            },
            { data: 'TipoNomina' },
            {
                data: 'NumeroEmpleados',
                className: 'text-center'
            },
            {
                data: 'TotalPercepciones',
                render: function (data) {
                    return '$' + parseFloat(data).toLocaleString('es-MX', { minimumFractionDigits: 2 });
                },
                className: 'text-right'
            },
            {
                data: 'TotalDeducciones',
                render: function (data) {
                    return '$' + parseFloat(data).toLocaleString('es-MX', { minimumFractionDigits: 2 });
                },
                className: 'text-right'
            },
            {
                data: 'TotalNeto',
                render: function (data) {
                    return '<strong>$' + parseFloat(data).toLocaleString('es-MX', { minimumFractionDigits: 2 }) + '</strong>';
                },
                className: 'text-right'
            },
            {
                data: 'Estatus',
                render: function (data) {
                    var badge = 'secondary';
                    if (data === 'CALCULADA') badge = 'info';
                    else if (data === 'CONTABILIZADA') badge = 'warning';
                    else if (data === 'PAGADA') badge = 'success';
                    else if (data === 'CANCELADA') badge = 'danger';
                    return '<span class="badge badge-' + badge + '">' + data + '</span>';
                },
                className: 'text-center'
            },
            {
                data: null,
                orderable: false,
                render: function (data) {
                    return '<button class="btn btn-sm btn-info" onclick="abrirModalAcciones(' + data.NominaID + ', \'' + data.Folio + '\', \'' + data.Periodo + '\', \'' + data.Estatus + '\')">' +
                        '<i class="fas fa-cog"></i> Acciones</button>';
                },
                className: 'text-center'
            }
        ],
        language: {
            url: 'https://cdn.datatables.net/plug-ins/1.13.4/i18n/es-ES.json'
        },
        order: [[2, 'desc']],
        responsive: true,
        autoWidth: false
    });
}

function abrirModalAcciones(nominaId, folio, periodo, estatus) {
    nominaSeleccionada = nominaId;
    $('#hdnNominaId').val(nominaId);
    $('#spanFolio').text(folio);
    $('#spanPeriodo').text(periodo);
    $('#spanEstatus').html('<span class="badge badge-info">' + estatus + '</span>');

    // Habilitar/deshabilitar botones según estatus
    if (estatus === 'CALCULADA') {
        $('#btnContabilizar').prop('disabled', false);
        $('#btnMarcarPagada').prop('disabled', true);
        $('#btnCancelar').prop('disabled', false);
    } else if (estatus === 'CONTABILIZADA') {
        $('#btnContabilizar').prop('disabled', true);
        $('#btnMarcarPagada').prop('disabled', false);
        $('#btnCancelar').prop('disabled', true);
    } else if (estatus === 'PAGADA') {
        $('#btnContabilizar').prop('disabled', true);
        $('#btnMarcarPagada').prop('disabled', true);
        $('#btnCancelar').prop('disabled', true);
    } else if (estatus === 'CANCELADA') {
        $('#btnContabilizar').prop('disabled', true);
        $('#btnMarcarPagada').prop('disabled', true);
        $('#btnCancelar').prop('disabled', true);
    }

    $('#modalAcciones').modal('show');
}

function verDetalle() {
    var nominaId = $('#hdnNominaId').val();
    window.location.href = '/Nomina/Detalle/' + nominaId;
}

function contabilizarNomina() {
    var nominaId = $('#hdnNominaId').val();

    if (!confirm('¿Está seguro de contabilizar esta nómina? Se generará automáticamente la póliza contable.')) {
        return;
    }

    $.ajax({
        url: '/Nomina/ContabilizarNomina',
        type: 'POST',
        data: { nominaId: nominaId },
        success: function (response) {
            if (response.success) {
                Swal.fire('Éxito', response.message, 'success');
                $('#modalAcciones').modal('hide');
                tablaNominas.ajax.reload();
            } else {
                Swal.fire('Error', response.message, 'error');
            }
        },
        error: function () {
            Swal.fire('Error', 'Error al contabilizar nómina', 'error');
        }
    });
}

function marcarComoPagada() {
    var nominaId = $('#hdnNominaId').val();

    Swal.fire({
        title: 'Marcar como Pagada',
        html: '<label>Fecha de Pago:</label><input type="date" id="swalFechaPago" class="swal2-input" value="' + new Date().toISOString().split('T')[0] + '">',
        showCancelButton: true,
        confirmButtonText: 'Confirmar',
        cancelButtonText: 'Cancelar',
        preConfirm: () => {
            const fechaPago = document.getElementById('swalFechaPago').value;
            if (!fechaPago) {
                Swal.showValidationMessage('Debe ingresar una fecha');
            }
            return { fechaPago: fechaPago };
        }
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: '/Nomina/MarcarComoPagada',
                type: 'POST',
                data: {
                    nominaId: nominaId,
                    fechaPago: result.value.fechaPago
                },
                success: function (response) {
                    if (response.success) {
                        Swal.fire('Éxito', response.message, 'success');
                        $('#modalAcciones').modal('hide');
                        tablaNominas.ajax.reload();
                    } else {
                        Swal.fire('Error', response.message, 'error');
                    }
                },
                error: function () {
                    Swal.fire('Error', 'Error al marcar como pagada', 'error');
                }
            });
        }
    });
}

function cancelarNomina() {
    var nominaId = $('#hdnNominaId').val();

    Swal.fire({
        title: '¿Cancelar Nómina?',
        text: 'Esta acción no se puede deshacer',
        icon: 'warning',
        input: 'textarea',
        inputPlaceholder: 'Motivo de cancelación',
        showCancelButton: true,
        confirmButtonColor: '#d33',
        cancelButtonColor: '#3085d6',
        confirmButtonText: 'Sí, cancelar',
        cancelButtonText: 'No'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: '/Nomina/CancelarNomina',
                type: 'POST',
                data: {
                    nominaId: nominaId,
                    motivo: result.value || 'Sin motivo especificado'
                },
                success: function (response) {
                    if (response.success) {
                        Swal.fire('Cancelada', response.message, 'success');
                        $('#modalAcciones').modal('hide');
                        tablaNominas.ajax.reload();
                    } else {
                        Swal.fire('Error', response.message, 'error');
                    }
                },
                error: function () {
                    Swal.fire('Error', 'Error al cancelar nómina', 'error');
                }
            });
        }
    });
}
