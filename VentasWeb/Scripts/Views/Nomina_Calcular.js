var nominaCalculadaId = null;

$(document).ready(function () {
    cargarEmpleadosActivos();
    configurarFechas();

    $('#txtFechaInicio, #txtFechaFin').on('change', function () {
        validarPeriodo();
    });

    $('#frmCalcularNomina').on('submit', function (e) {
        e.preventDefault();
        procesarNomina();
    });
});

function configurarFechas() {
    var hoy = new Date();
    var primerDia, ultimoDia;

    // Si estamos en la primera quincena (día 1-15)
    if (hoy.getDate() <= 15) {
        primerDia = new Date(hoy.getFullYear(), hoy.getMonth(), 1);
        ultimoDia = new Date(hoy.getFullYear(), hoy.getMonth(), 15);
    } else {
        // Segunda quincena (día 16-fin de mes)
        primerDia = new Date(hoy.getFullYear(), hoy.getMonth(), 16);
        ultimoDia = new Date(hoy.getFullYear(), hoy.getMonth() + 1, 0);
    }

    $('#txtFechaInicio').val(primerDia.toISOString().split('T')[0]);
    $('#txtFechaFin').val(ultimoDia.toISOString().split('T')[0]);
    $('#txtFechaPago').val(new Date(ultimoDia.getTime() + 86400000).toISOString().split('T')[0]);

    validarPeriodo();
}

function cargarEmpleadosActivos() {
    $.ajax({
        url: '/Nomina/ObtenerEmpleadosActivos',
        type: 'GET',
        success: function (response) {
            if (response.success) {
                var empleados = response.data;
                var html = '<div class="info-box">' +
                    '<span class="info-box-icon bg-success"><i class="fas fa-users"></i></span>' +
                    '<div class="info-box-content">' +
                    '<span class="info-box-text">Empleados Activos</span>' +
                    '<span class="info-box-number">' + empleados.length + '</span>' +
                    '</div></div>';

                if (empleados.length === 0) {
                    html += '<div class="alert alert-warning">No hay empleados activos para procesar</div>';
                } else {
                    html += '<small class="text-muted">Se procesarán ' + empleados.length + ' empleado(s)</small>';
                }

                $('#infoEmpleados').html(html);
            } else {
                $('#infoEmpleados').html('<div class="alert alert-danger">Error al cargar empleados</div>');
            }
        },
        error: function () {
            $('#infoEmpleados').html('<div class="alert alert-danger">Error de conexión</div>');
        }
    });
}

function validarPeriodo() {
    var fechaInicio = $('#txtFechaInicio').val();
    var fechaFin = $('#txtFechaFin').val();

    if (!fechaInicio || !fechaFin) return;

    $.ajax({
        url: '/Nomina/ValidarPeriodo',
        type: 'GET',
        data: { fechaInicio: fechaInicio, fechaFin: fechaFin },
        success: function (response) {
            if (response.success) {
                $('#alertPeriodo').show();
                $('#spanMensajePeriodo').text('Periodo: ' + response.periodo);

                if (response.yaExiste) {
                    $('#alertAdvertencia').show();
                    $('#spanAdvertencia').text(response.mensaje + '. Verifique que no exista duplicidad.');
                } else {
                    $('#alertAdvertencia').hide();
                }
            }
        }
    });
}

function procesarNomina() {
    var fechaInicio = $('#txtFechaInicio').val();
    var fechaFin = $('#txtFechaFin').val();
    var fechaPago = $('#txtFechaPago').val();
    var tipoNomina = $('#cboTipoNomina').val();

    // Validaciones
    if (!fechaInicio || !fechaFin || !fechaPago) {
        Swal.fire('Advertencia', 'Complete todos los campos requeridos', 'warning');
        return;
    }

    if (new Date(fechaInicio) > new Date(fechaFin)) {
        Swal.fire('Error', 'La fecha de inicio no puede ser mayor a la fecha fin', 'error');
        return;
    }

    if (new Date(fechaPago) < new Date(fechaFin)) {
        Swal.fire('Error', 'La fecha de pago debe ser posterior a la fecha fin del periodo', 'error');
        return;
    }

    // Mostrar modal de progreso
    $('#modalProgreso').modal('show');

    $.ajax({
        url: '/Nomina/ProcesarNomina',
        type: 'POST',
        data: {
            fechaInicio: fechaInicio,
            fechaFin: fechaFin,
            fechaPago: fechaPago,
            tipoNomina: tipoNomina
        },
        success: function (response) {
            $('#modalProgreso').modal('hide');

            if (response.success) {
                nominaCalculadaId = response.data.nominaId;

                // Mostrar resultado
                $('#spanResultFolio').text(response.data.folio);
                $('#spanResultPeriodo').text(response.data.periodo);
                $('#spanResultEmpleados').text(response.data.numEmpleados);
                $('#spanResultPercepciones').text('$' + parseFloat(response.data.totalPercepciones).toLocaleString('es-MX', { minimumFractionDigits: 2 }));
                $('#spanResultDeducciones').text('$' + parseFloat(response.data.totalDeducciones).toLocaleString('es-MX', { minimumFractionDigits: 2 }));
                $('#spanResultNeto').text('$' + parseFloat(response.data.totalNeto).toLocaleString('es-MX', { minimumFractionDigits: 2 }));

                $('#modalResultado').modal('show');
            } else {
                Swal.fire('Error', response.message, 'error');
            }
        },
        error: function (xhr) {
            $('#modalProgreso').modal('hide');
            var errorMsg = xhr.responseJSON && xhr.responseJSON.message ? xhr.responseJSON.message : 'Error al procesar nómina';
            Swal.fire('Error', errorMsg, 'error');
        }
    });
}

function verDetalleNomina() {
    if (nominaCalculadaId) {
        window.location.href = '/Nomina/Detalle/' + nominaCalculadaId;
    }
}

function nuevaNomina() {
    $('#modalResultado').modal('hide');
    $('#frmCalcularNomina')[0].reset();
    nominaCalculadaId = null;
    configurarFechas();
}
