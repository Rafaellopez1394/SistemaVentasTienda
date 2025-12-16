// Dashboard.js - Dashboard del Contador

$(document).ready(function () {
    cargarDashboard();
});

function cargarDashboard() {
    $.ajax({
        url: '/Contador/ObtenerDashboard',
        type: 'GET',
        success: function (response) {
            if (response.success) {
                actualizarKPIs(response.data);
                mostrarAlertas(response.data.Alertas);
            } else {
                toastr.error(response.mensaje || 'Error al cargar dashboard');
            }
        },
        error: function () {
            toastr.error('Error de conexión al cargar dashboard');
        }
    });
}

function actualizarKPIs(data) {
    // Facturas
    $('#kpiFacturasMes').text(data.FacturasMes);
    $('#kpiTotalFacturado').text('$' + parseFloat(data.TotalFacturadoMes).toLocaleString('es-MX', { minimumFractionDigits: 2, maximumFractionDigits: 2 }));
    
    // Nómina
    $('#kpiRecibosMes').text(data.RecibosMes);
    $('#kpiNominaMes').text('$' + parseFloat(data.TotalNominaMes).toLocaleString('es-MX', { minimumFractionDigits: 2, maximumFractionDigits: 2 }));
    
    // Cuentas por Pagar
    $('#kpiCuentasPendientes').text(data.CuentasPendientes);
    $('#kpiPorPagar').text('$' + parseFloat(data.TotalPorPagar).toLocaleString('es-MX', { minimumFractionDigits: 2, maximumFractionDigits: 2 }));
    
    // Pólizas
    $('#kpiPolizasMes').text(data.PolizasMes);
}

function mostrarAlertas(alertas) {
    var container = $('#alertasContainer');
    container.empty();
    
    if (!alertas || alertas.length === 0) {
        return;
    }
    
    alertas.forEach(function (alerta) {
        var colorClass = alerta.Color || 'info';
        var icono = alerta.Icono || 'fa-info-circle';
        
        var alertHtml = `
            <div class="col-12">
                <div class="alert alert-${colorClass} alert-dismissible fade show" role="alert">
                    <i class="fas ${icono} mr-2"></i>
                    <strong>${alerta.Titulo}:</strong> ${alerta.Mensaje}
                    <button type="button" class="close" data-dismiss="alert" aria-label="Close">
                        <span aria-hidden="true">&times;</span>
                    </button>
                </div>
            </div>
        `;
        
        container.append(alertHtml);
    });
}
