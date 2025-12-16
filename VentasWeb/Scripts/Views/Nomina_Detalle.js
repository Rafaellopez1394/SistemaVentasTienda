$(document).ready(function () {
    $('#tbRecibos').DataTable({
        language: {
            url: '//cdn.datatables.net/plug-ins/1.13.4/i18n/es-ES.json'
        },
        order: [[1, 'asc']],
        responsive: true,
        autoWidth: false
    });
});

function contabilizarNomina(nominaId) {
    if (!confirm('¿Está seguro de contabilizar esta nómina? Se generará automáticamente la póliza contable.')) {
        return;
    }

    $.ajax({
        url: '/Nomina/ContabilizarNomina',
        type: 'POST',
        data: { nominaId: nominaId },
        success: function (response) {
            if (response.success) {
                Swal.fire('Éxito', response.message, 'success').then(() => {
                    location.reload();
                });
            } else {
                Swal.fire('Error', response.message, 'error');
            }
        },
        error: function () {
            Swal.fire('Error', 'Error al contabilizar nómina', 'error');
        }
    });
}

function cancelarNomina(nominaId) {
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
                        Swal.fire('Cancelada', response.message, 'success').then(() => {
                            location.reload();
                        });
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

function exportarExcel(nominaId) {
    window.open('/Nomina/ExportarNominaExcel?nominaId=' + nominaId, '_blank');
}
