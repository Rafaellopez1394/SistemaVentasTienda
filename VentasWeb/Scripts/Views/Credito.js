let clienteSeleccionado = null;

$(document).ready(function () {
    // 1. INICIALIZAR AUTOCOMPLETADO PARA BÚSQUEDA DE CLIENTES
    $("#txtBuscarCliente").autocomplete({
        source: function (request, response) {
            // Reutilizamos el endpoint que busca clientes del módulo de Ventas
            $.get("/Venta/BuscarCliente", { texto: request.term }, function (data) {
                response(data.map(c => ({
                    label: c.RFC + " - " + c.RazonSocial,
                    value: c.RazonSocial,
                    datos: c
                })));
            });
        },
        minLength: 2,
        select: function (event, ui) {
            event.preventDefault();
            clienteSeleccionado = ui.item.datos;

            // Mostrar info básica del cliente
            $("#lblNombreCliente").text(clienteSeleccionado.RazonSocial);
            $("#infoCliente").removeClass("d-none");
            $("#txtBuscarCliente").val(clienteSeleccionado.RazonSocial);

            // Cargar y mostrar info detallada del crédito
            let infoCreditoHtml = "<strong>RFC:</strong> " + clienteSeleccionado.RFC;
            $.get("/Venta/ObtenerCreditosCliente", { clienteId: clienteSeleccionado.ClienteID }, function (creditos) {
                const creditosActivos = creditos.filter(c => c.Estatus);

                if (creditosActivos.length > 0) {
                    infoCreditoHtml += `<br/><strong>Créditos Activos:</strong><ul>`;
                    creditosActivos.forEach(cred => {
                        let detalle = `<li>${cred.TipoCredito} (${cred.Criterio}): `;
                        if(cred.Criterio === 'Dinero') detalle += `Límite $${cred.LimiteDinero.toFixed(2)}`;
                        if(cred.Criterio === 'Producto') detalle += `Límite ${cred.LimiteProducto} unidades`;
                        if(cred.Criterio === 'Tiempo') detalle += `Plazo de ${cred.PlazoDias} días`;
                        detalle += `</li>`;
                        infoCreditoHtml += detalle;
                    });
                    infoCreditoHtml += `</ul>`;
                } else {
                    infoCreditoHtml += "<br/><strong>Crédito:</strong> Cliente sin créditos activos asignados.";
                }
                $("#lblInfoExtraCliente").html(infoCreditoHtml);
            }).fail(function() {
                $("#lblInfoExtraCliente").html(infoCreditoHtml + "<br/><em>No se pudo cargar la información del crédito.</em>");
            });


            // Cargar tabla de ventas a crédito
            cargarVentasCredito(clienteSeleccionado.ClienteID);
        }
    });

    // 2. LÓGICA PARA REGISTRAR UN PAGO
    $("#btnRegistrarPago").on("click", function () {
        const monto = parseFloat($("#txtMontoPago").val());

        if (!clienteSeleccionado) {
            alert("Por favor, seleccione un cliente.");
            return;
        }
        if (isNaN(monto) || monto <= 0) {
            alert("El monto del pago debe ser un número mayor a cero.");
            return;
        }

        const pago = {
            ClienteID: clienteSeleccionado.ClienteID,
            Monto: monto,
            Comentario: $("#txtComentarioPago").val()
        };

        // Reutilizamos el endpoint de VentaController para registrar el pago
        $.ajax({
            url: "/Venta/RegistrarPago",
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify(pago),
            success: function (response) {
                if (response.resultado) {
                    alert("Pago registrado con éxito.");
                    // Limpiar campos y recargar las ventas
                    $("#txtMontoPago").val("");
                    $("#txtComentarioPago").val("");
                    cargarVentasCredito(clienteSeleccionado.ClienteID);
                } else {
                    alert("Hubo un error al registrar el pago.");
                }
            },
            error: function () {
                alert("Error de comunicación con el servidor.");
            }
        });
    });
});

/**
 * Carga la tabla de ventas a crédito pendientes para un cliente específico.
 * @param {string} clienteId - El ID del cliente.
 */
function cargarVentasCredito(clienteId) {
    // Reutilizamos el endpoint de VentaController
    $.get("/Venta/ObtenerVentasCliente", { clienteId: clienteId }, function (response) {
        const tbody = $("#tblVentasCredito tbody");
        tbody.empty();
        let saldoTotal = 0;

        if (response.data && response.data.length > 0) {
            response.data.forEach(venta => {
                saldoTotal += venta.Saldo;
                tbody.append(`
                    <tr>
                        <td>${venta.Folio}</td>
                        <td>${new Date(parseInt(venta.Fecha.substr(6))).toLocaleDateString()}</td>
                        <td>$${venta.Total.toFixed(2)}</td>
                        <td>$${venta.Saldo.toFixed(2)}</td>
                        <td>
                            <button class="btn btn-info btn-sm" onclick="verDetalleVenta('${venta.VentaID}')">
                                <i class="fas fa-eye"></i> Ver Detalle
                            </button>
                        </td>
                    </tr>
                `);
            });
        } else {
            tbody.append('<tr><td colspan="5" class="text-center">Este cliente no tiene créditos pendientes.</td></tr>');
        }

        // Actualizar el saldo total y mostrar la sección
        $("#lblSaldoTotal").text(`$${saldoTotal.toFixed(2)}`);
        $("#seccionCreditos").removeClass("d-none");
    });
}

/**
 * (Función PENDIENTE) Muestra el detalle de una venta específica.
 * @param {string} ventaId - El ID de la venta a detallar.
 */
function verDetalleVenta(ventaId) {
    // Aquí se podría abrir un modal para mostrar los productos de esa venta.
    // Esto requeriría un nuevo endpoint en el controlador.
    alert("Función para ver detalle no implementada. ID de Venta: " + ventaId);
}
