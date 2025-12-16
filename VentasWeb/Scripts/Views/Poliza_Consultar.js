var tablaPolizas;

$(document).ready(function () {
    cargarPolizas();

    $('#btnBuscar').click(function () {
        tablaPolizas.ajax.reload();
    });
});

function cargarPolizas() {
    tablaPolizas = $('#tbPolizas').DataTable({
        ajax: {
            url: '/Poliza/ObtenerFiltradas',
            type: 'GET',
            data: function (d) {
                d.fechaInicio = $('#txtFechaInicio').val();
                d.fechaFin = $('#txtFechaFin').val();
                d.tipoPoliza = $('#cboTipoPoliza').val();
            },
            dataSrc: 'data'
        },
        columns: [
            { data: 'PolizaID' },
            { data: 'TipoPoliza' },
            {
                data: 'FechaPoliza',
                render: function (data) {
                    if (!data) return '';
                    var fecha = new Date(data);
                    return fecha.toLocaleDateString('es-MX');
                }
            },
            {
                data: 'Concepto',
                render: function (data) {
                    return data.length > 50 ? data.substring(0, 50) + '...' : data;
                }
            },
            {
                data: 'TotalDebe',
                render: function (data) {
                    return '$' + parseFloat(data || 0).toFixed(2);
                },
                className: 'text-right'
            },
            {
                data: 'TotalHaber',
                render: function (data) {
                    return '$' + parseFloat(data || 0).toFixed(2);
                },
                className: 'text-right'
            },
            { data: 'Usuario' },
            {
                data: null,
                orderable: false,
                render: function (data) {
                    return `<button class="btn btn-info btn-sm" onclick="verDetalle(${data.PolizaID})">
                                <i class="fas fa-eye"></i> Ver
                            </button>`;
                }
            }
        ],
        order: [[0, 'desc']],
        language: {
            url: '/Content/Plugins/datatables/js/datatable_spanish.json'
        }
    });
}

function verDetalle(polizaId) {
    $.ajax({
        url: '/Poliza/ObtenerDetalle',
        type: 'GET',
        data: { polizaId: polizaId },
        success: function (res) {
            if (res.poliza) {
                var p = res.poliza;
                $('#lblPolizaID').text(p.PolizaID);
                $('#lblTipo').text(p.TipoPoliza);
                $('#lblFecha').text(new Date(p.FechaPoliza).toLocaleDateString('es-MX'));
                $('#lblConcepto').text(p.Concepto);
                $('#lblUsuario').text(p.Usuario || 'N/A');
                $('#lblFechaReg').text(p.FechaRegistro ? new Date(p.FechaRegistro).toLocaleString('es-MX') : 'N/A');

                var html = '';
                var totalDebe = 0, totalHaber = 0;

                if (res.detalles && res.detalles.length > 0) {
                    res.detalles.forEach(function (d) {
                        totalDebe += parseFloat(d.Debe || 0);
                        totalHaber += parseFloat(d.Haber || 0);

                        html += '<tr>';
                        html += '<td>' + d.CuentaID + (d.NombreCuenta ? ' - ' + d.NombreCuenta : '') + '</td>';
                        html += '<td>' + (d.Concepto || '') + '</td>';
                        html += '<td class="text-right">$' + parseFloat(d.Debe || 0).toFixed(2) + '</td>';
                        html += '<td class="text-right">$' + parseFloat(d.Haber || 0).toFixed(2) + '</td>';
                        html += '</tr>';
                    });
                }

                $('#tbodyDetalle').html(html);
                $('#totalDebe').text('$' + totalDebe.toFixed(2));
                $('#totalHaber').text('$' + totalHaber.toFixed(2));

                var diferencia = Math.abs(totalDebe - totalHaber);
                if (diferencia > 0.01) {
                    $('#diferencia').text('$' + diferencia.toFixed(2));
                    $('#rowDiferencia').show();
                } else {
                    $('#rowDiferencia').hide();
                }

                $('#modalDetalle').modal('show');
            }
        },
        error: function () {
            alert('Error al cargar el detalle de la póliza');
        }
    });
}
