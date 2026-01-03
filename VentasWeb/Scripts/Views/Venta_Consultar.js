// Scripts/Views/Venta_Consultar.js
var tabladata;

// Función para convertir fechas .NET /Date(timestamp)/ a formato legible
function convertirFechaDotNet(fechaDotNet) {
    if (!fechaDotNet) return 'N/A';
    
    // Si ya es una fecha normal, retornarla
    if (!fechaDotNet.toString().includes('/Date(')) {
        return fechaDotNet;
    }
    
    // Extraer el timestamp del formato /Date(1234567890)/
    var timestamp = parseInt(fechaDotNet.replace(/\/Date\((\d+)\)\//, '$1'));
    var fecha = new Date(timestamp);
    
    // Formatear como DD/MM/YYYY HH:mm
    var dia = ('0' + fecha.getDate()).slice(-2);
    var mes = ('0' + (fecha.getMonth() + 1)).slice(-2);
    var anio = fecha.getFullYear();
    var horas = ('0' + fecha.getHours()).slice(-2);
    var minutos = ('0' + fecha.getMinutes()).slice(-2);
    
    return dia + '/' + mes + '/' + anio + ' ' + horas + ':' + minutos;
}

$(document).ready(function () {
    // Inicializar tabla
    tabladata = $('#tbVentas').DataTable({
        "ajax": {
            "url": "/Venta/ObtenerTodasVentas",
            "type": "GET",
            "datatype": "json",
            "data": function(d) {
                d.fechaInicio = $("#txtFechaInicio").val();
                d.fechaFin = $("#txtFechaFin").val();
                d.codigoVenta = $("#txtCodigoVenta").val();
                d.documentoCliente = $("#txtDocumentoCliente").val();
                d.nombreCliente = $("#txtNombreCliente").val();
            }
        },
        "columns": [
            {
                "data": null,
                "orderable": false,
                "render": function (data, type, row) {
                    var botones = '<div class="btn-group btn-group-sm" role="group">';
                    
                    // Botón Ver Detalle
                    botones += '<button class="btn btn-info" title="Ver Detalle" onclick="verDetalle(\'' + row.VentaID + '\')"><i class="fas fa-eye"></i></button>';
                    
                    // Botón Facturar (solo si no está facturada)
                    if (row.EstadoFactura === 'Sin Facturar') {
                        botones += '<button class="btn btn-success" title="Generar Factura" onclick="facturar(\'' + row.VentaID + '\')"><i class="fas fa-file-invoice"></i></button>';
                    }
                    
                    // Botón Ver Factura (solo si está facturada)
                    if (row.EstadoFactura === 'Facturada') {
                        botones += '<button class="btn btn-primary" title="Ver Factura" onclick="verFactura(\'' + row.VentaID + '\')"><i class="fas fa-file-pdf"></i></button>';
                    }
                    
                    botones += '</div>';
                    return botones;
                }
            },
            { "data": "TipoDocumento" },
            {
                "data": "CodigoDocumento",
                "render": function(data) {
                    return data.substring(0, 8) + '...'; // Mostrar solo primeros 8 caracteres del GUID
                }
            },
            { 
                "data": "FechaCreacion",
                "render": function(data) {
                    return convertirFechaDotNet(data);
                }
            },
            { "data": "DocumentoCliente" },
            { "data": "NombreCliente" },
            {
                "data": "TotalVenta",
                "render": function(data) {
                    return '$' + parseFloat(data).toFixed(2).replace(/\d(?=(\d{3})+\.)/g, '$&,');
                }
            },
            {
                "data": "Estatus",
                "render": function(data) {
                    var clase = data === 'CREDITO' ? 'badge-warning' : 'badge-success';
                    return '<span class="badge ' + clase + '">' + data + '</span>';
                }
            },
            {
                "data": "TotalPagado",
                "render": function(data) {
                    return '$' + parseFloat(data).toFixed(2).replace(/\d(?=(\d{3})+\.)/g, '$&,');
                }
            },
            {
                "data": "SaldoPendiente",
                "render": function(data) {
                    var clase = parseFloat(data) > 0 ? 'text-danger font-weight-bold' : 'text-success';
                    return '<span class="' + clase + '">$' + parseFloat(data).toFixed(2).replace(/\d(?=(\d{3})+\.)/g, '$&,') + '</span>';
                }
            },
            {
                "data": "EstadoFactura",
                "render": function(data) {
                    var clase = data === 'Facturada' ? 'badge-success' : 'badge-secondary';
                    return '<span class="badge ' + clase + '">' + data + '</span>';
                }
            }
        ],
        language: {
            "url": "https://cdn.datatables.net/plug-ins/1.10.24/i18n/Spanish.json"
        },
        "order": [[3, "desc"]], // Ordenar por fecha descendente
        "pageLength": 25,
        "responsive": true,
        "dom": 'Bfrtip',
        "buttons": [
            {
                extend: 'excelHtml5',
                text: '<i class="fas fa-file-excel"></i> Excel',
                className: 'btn btn-success btn-sm'
            },
            {
                extend: 'pdfHtml5',
                text: '<i class="fas fa-file-pdf"></i> PDF',
                className: 'btn btn-danger btn-sm'
            }
        ]
    });

    // Evento botón buscar
    $("#btnBuscar").click(function() {
        tabladata.ajax.reload();
    });

    // Buscar al presionar Enter en los campos
    $("#txtFechaInicio, #txtFechaFin, #txtCodigoVenta, #txtDocumentoCliente, #txtNombreCliente").keypress(function(e) {
        if (e.which === 13) {
            tabladata.ajax.reload();
        }
    });
});

function verDetalle(ventaId) {
    $.ajax({
        url: '/Venta/ObtenerDetalleVenta',
        type: 'GET',
        data: { ventaId: ventaId },
        success: function(response) {
            if (response.success) {
                mostrarModalDetalle(response.data);
            } else {
                toastr.error('Error al obtener detalle de venta');
            }
        },
        error: function() {
            toastr.error('Error de conexión');
        }
    });
}

function facturar(ventaId) {
    // Redirigir al módulo de facturación con el ID de venta
    window.location.href = '/Factura/Index?ventaId=' + ventaId;
}

function verFactura(ventaId) {
    // Abrir modal o ventana con la factura
    window.open('/Factura/DescargarXML?ventaId=' + ventaId, '_blank');
}

function mostrarModalDetalle(venta) {
    var html = '<div class="modal fade" id="modalDetalle" tabindex="-1">';
    html += '<div class="modal-dialog modal-lg">';
    html += '<div class="modal-content">';
    html += '<div class="modal-header bg-info text-white">';
    html += '<h5 class="modal-title"><i class="fas fa-file-invoice"></i> Detalle de Venta</h5>';
    html += '<button type="button" class="close text-white" data-dismiss="modal">&times;</button>';
    html += '</div>';
    html += '<div class="modal-body">';
    html += '<p><strong>Cliente:</strong> ' + venta.RazonSocial + '</p>';
    html += '<p><strong>Fecha:</strong> ' + convertirFechaDotNet(venta.FechaVenta) + '</p>';
    html += '<p><strong>Total:</strong> $' + (venta.Total || 0).toFixed(2) + '</p>';
    html += '<p><strong>Estatus:</strong> ' + (venta.Estatus || 'N/A') + '</p>';
    
    if (venta.Detalle && venta.Detalle.length > 0) {
        html += '<h6 class="mt-3">Productos:</h6>';
        html += '<table class="table table-sm table-bordered">';
        html += '<thead><tr><th>Producto</th><th>Cantidad</th><th>Precio</th><th>Subtotal</th></tr></thead>';
        html += '<tbody>';
        venta.Detalle.forEach(function(d) {
            var cantidad = parseFloat(d.Cantidad) || 0;
            var precio = parseFloat(d.PrecioVenta) || 0; // Backend usa PrecioVenta, no Precio
            var subtotal = cantidad * precio;
            
            html += '<tr>';
            html += '<td>' + (d.Producto || 'Sin nombre') + '</td>'; // Backend usa Producto, no NombreProducto
            html += '<td>' + cantidad + '</td>';
            html += '<td>$' + precio.toFixed(2) + '</td>';
            html += '<td>$' + subtotal.toFixed(2) + '</td>';
            html += '</tr>';
        });
        html += '</tbody></table>';
    }
    
    html += '</div>';
    html += '<div class="modal-footer">';
    html += '<button type="button" class="btn btn-secondary" data-dismiss="modal">Cerrar</button>';
    html += '</div>';
    html += '</div></div></div>';
    
    $('body').append(html);
    $('#modalDetalle').modal('show');
    $('#modalDetalle').on('hidden.bs.modal', function () {
        $(this).remove();
    });
}