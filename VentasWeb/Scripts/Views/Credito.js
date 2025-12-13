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
                        if (cred.Criterio === 'Dinero') detalle += `Límite $${cred.LimiteDinero.toFixed(2)}`;
                        if (cred.Criterio === 'Producto') detalle += `Límite ${cred.LimiteProducto} unidades`;
                        if (cred.Criterio === 'Tiempo') detalle += `Plazo de ${cred.PlazoDias} días`;
                        detalle += `</li>`;
                        infoCreditoHtml += detalle;
                    });
                    infoCreditoHtml += `</ul>`;
                } else {
                    infoCreditoHtml += "<br/><strong>Crédito:</strong> Cliente sin créditos activos asignados.";
                }
                $("#lblInfoExtraCliente").html(infoCreditoHtml);
            }).fail(function () {
                $("#lblInfoExtraCliente").html(infoCreditoHtml + "<br/><em>No se pudo cargar la información del crédito.</em>");
            });

            // Cargar tabla de ventas a crédito
            cargarVentasCredito(clienteSeleccionado.ClienteID);
        }
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
                saldoTotal += venta.SaldoPendiente;
                const fechaVenta = parsearFechaNet(venta.FechaVenta).toLocaleDateString();
                tbody.append(`
                    <tr>
                        <td>${venta.VentaID.substr(0, 8).toUpperCase()}</td>
                        <td>${fechaVenta}</td>
                        <td>$${venta.Total.toFixed(2)}</td>
                        <td class="text-danger font-weight-bold">$${venta.SaldoPendiente.toFixed(2)}</td>
                        <td>
                            <button class="btn btn-success btn-sm mr-1" onclick="abrirModalPago('${venta.VentaID}', '${venta.ClienteID}', '${venta.RazonSocial}', ${venta.Total}, ${venta.SaldoPendiente})" title="Registrar Pago">
                                <i class="fas fa-dollar-sign"></i>
                            </button>
                            <button class="btn btn-info btn-sm" onclick="verDetalleVenta('${venta.VentaID}')" title="Ver Detalle">
                                <i class="fas fa-eye"></i>
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
        
        // Mostrar botón "Pagar Todo" si hay saldo pendiente
        if (saldoTotal > 0) {
            $("#btnPagarTodo").show();
        } else {
            $("#btnPagarTodo").hide();
        }
        
        $("#seccionCreditos").removeClass("d-none");
    });
}

/**
 * Convierte fecha de formato .NET JSON (/Date(...)/) a objeto Date
 */
function parsearFechaNet(fechaNet) {
    if (!fechaNet) return new Date();
    const timestamp = parseInt(fechaNet.replace(/\/Date\((\d+)\)\//, '$1'));
    return new Date(timestamp);
}

/**
 * Muestra el detalle de una venta específica en un modal.
 * @param {string} ventaId - El ID de la venta a detallar.
 */
function verDetalleVenta(ventaId) {
    // Primero mostrar el modal con datos de carga
    $("#modalDetalleVenta").modal("show");

    $.get("/Venta/ObtenerDetalleVenta", { ventaId: ventaId }, function (response) {
        if (response.success && response.data) {
            var venta = response.data;

            document.getElementById("modalDetalleVentaLabel").innerHTML = "Detalle de Venta - " + ventaId.substr(0, 8).toUpperCase();
            document.getElementById("lblDetalleCliente").innerHTML = venta.RazonSocial || "N/A";
            document.getElementById("lblDetalleFecha").innerHTML = parsearFechaNet(venta.FechaVenta).toLocaleDateString();
            document.getElementById("lblDetalleTotal").innerHTML = "$" + venta.Total.toFixed(2);
            document.getElementById("lblDetalleSaldo").innerHTML = "$" + venta.SaldoPendiente.toFixed(2);

            var tbody = document.querySelector("#tblDetalleVenta tbody");
            var filasHTML = "";

            if (venta.Detalle && venta.Detalle.length > 0) {
                for (var i = 0; i < venta.Detalle.length; i++) {
                    var item = venta.Detalle[i];
                    var subtotal = item.Cantidad * item.PrecioVenta;
                    filasHTML += "<tr>" +
                        "<td>" + item.Producto + "</td>" +
                        "<td>" + item.Cantidad + "</td>" +
                        "<td>$" + item.PrecioVenta.toFixed(2) + "</td>" +
                        "<td>$" + subtotal.toFixed(2) + "</td>" +
                        "</tr>";
                }
            } else {
                filasHTML = '<tr><td colspan="4" class="text-center">Sin productos</td></tr>';
            }

            tbody.innerHTML = filasHTML;
        } else {
            $("#modalDetalleVenta").modal("hide");
            alert("No se pudo cargar el detalle de la venta");
        }
    }).fail(function () {
        $("#modalDetalleVenta").modal("hide");
        alert("Error al obtener el detalle de la venta");
    });
}

/**
 * Abre el modal para registrar un pago
 */
function abrirModalPago(ventaId, clienteId, razonSocial, total, saldoPendiente) {
    $("#pagoVentaId").val(ventaId);
    $("#pagoClienteId").val(clienteId);
    $("#pagoCliente").text(razonSocial);
    $("#pagoFolio").text(ventaId.substr(0, 8).toUpperCase());
    $("#pagoTotal").text("$" + total.toFixed(2));
    $("#pagoSaldoPendiente").text("$" + saldoPendiente.toFixed(2));

    $("#pagoMonto").val(saldoPendiente.toFixed(2));
    $("#pagoFormaPago").val("");
    $("#pagoReferencia").val("");
    $("#pagoComentario").val("");
    $("#pagoGenerarFactura").prop("checked", false);
    $("#pagoGenerarComplemento").prop("checked", false);

    $("#modalRegistrarPago").modal("show");
}

/**
 * Abre el modal para pagar todo el saldo pendiente del cliente
 */
$("#btnPagarTodo").click(function () {
    if (!clienteSeleccionado) {
        alert("No hay cliente seleccionado");
        return;
    }

    const saldoTotalTexto = $("#lblSaldoTotal").text().replace("$", "");
    const saldoTotal = parseFloat(saldoTotalTexto);

    if (saldoTotal <= 0) {
        alert("No hay saldo pendiente para pagar");
        return;
    }

    // Usar el modal existente pero con valores especiales (Guid vacío indica pago total)
    $("#pagoVentaId").val("00000000-0000-0000-0000-000000000000");
    $("#pagoClienteId").val(clienteSeleccionado.ClienteID);
    $("#pagoCliente").text(clienteSeleccionado.RazonSocial);
    $("#pagoFolio").text("PAGO TOTAL");
    $("#pagoTotal").text("$" + saldoTotal.toFixed(2));
    $("#pagoSaldoPendiente").text("$" + saldoTotal.toFixed(2));

    $("#pagoMonto").val(saldoTotal.toFixed(2));
    $("#pagoFormaPago").val("");
    $("#pagoReferencia").val("");
    $("#pagoComentario").val("Pago total de saldo pendiente");
    $("#pagoGenerarFactura").prop("checked", false);
    $("#pagoGenerarComplemento").prop("checked", false);

    $("#modalRegistrarPago").modal("show");
});

/**
 * Registra el pago de una venta a crédito
 */
$("#btnConfirmarPago").click(function () {
    const ventaId = $("#pagoVentaId").val();
    const clienteId = $("#pagoClienteId").val();
    const monto = parseFloat($("#pagoMonto").val());
    const formaPago = $("#pagoFormaPago").val();
    const referencia = $("#pagoReferencia").val();
    const comentario = $("#pagoComentario").val();
    const generarFactura = $("#pagoGenerarFactura").is(":checked");
    const generarComplemento = $("#pagoGenerarComplemento").is(":checked");

    if (!monto || monto <= 0) {
        alert("Por favor ingrese un monto válido");
        $("#pagoMonto").focus();
        return;
    }

    if (!formaPago) {
        alert("Por favor seleccione una forma de pago");
        $("#pagoFormaPago").focus();
        return;
    }

    const saldoPendiente = parseFloat($("#pagoSaldoPendiente").text().replace("$", ""));
    if (monto > saldoPendiente) {
        alert("El monto del pago no puede ser mayor al saldo pendiente");
        $("#pagoMonto").focus();
        return;
    }

    const pago = {
        VentaID: ventaId,
        ClienteID: clienteId,
        Monto: monto,
        FormaPago: formaPago,
        Referencia: referencia,
        Comentario: comentario,
        GenerarFactura: generarFactura,
        GenerarComplemento: generarComplemento
    };

    const montoPagoStr = monto.toFixed(2);
    const esPagoTotal = ventaId === "00000000-0000-0000-0000-000000000000";
    const mensaje = esPagoTotal 
        ? `¿Confirma el pago total de $${montoPagoStr} para liquidar todas las ventas pendientes?`
        : `¿Confirma el registro del pago por $${montoPagoStr}?`;

    if (!confirm(mensaje)) {
        return;
    }

    $("#btnConfirmarPago")
        .prop("disabled", true)
        .html('<i class="fas fa-spinner fa-spin"></i> Procesando...');

    // Si es pago total, usar endpoint especial
    const url = esPagoTotal ? "/Venta/RegistrarPagoTotal" : "/Venta/RegistrarPago";

    $.ajax({
        url: url,
        method: "POST",
        contentType: "application/json",
        data: JSON.stringify(pago),
        success: function (response) {
            if (response.success) {
                alert("Pago registrado correctamente");
                $("#modalRegistrarPago").modal("hide");

                const clienteIdActual = $("#pagoClienteId").val();
                cargarVentasCredito(clienteIdActual);
            } else {
                alert("Error al registrar el pago: " + (response.mensaje || "Error desconocido"));
            }
        },
        error: function () {
            alert("Error de comunicación con el servidor");
        },
        complete: function () {
            $("#btnConfirmarPago").prop("disabled", false).html('<i class="fas fa-check"></i> Registrar Pago');
        }
    });
});
