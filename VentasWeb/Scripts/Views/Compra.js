let proveedorActual = null;
let carrito = [];

$(document).ready(function () {
    // Autocompletado de proveedor
    $("#txtBuscarProveedor").autocomplete({
        source: function (request, response) {
            $.get("/Compra/BuscarProveedor", { texto: request.term }, function (data) {
                response(data.map(p => ({
                    label: p.RazonSocial + " - " + p.RFC,
                    value: p.RFC,
                    datos: p
                })));
            });
        },
        select: function (event, ui) {
            proveedorActual = ui.item.datos;
            mostrarInfoProveedor();
        }
    });

    // Autocompletado de productos
    $("#txtBuscarProducto").autocomplete({
        source: "/Compra/ObtenerProductos",
        select: function (event, ui) {
            agregarProductoAlCarrito(ui.item);
            $(this).val('');
            return false;
        }
    });
});

function mostrarInfoProveedor() {
    if (!proveedorActual) return;
    $("#lblProveedor").text(proveedorActual.RazonSocial);
    $("#lblDatosProveedor").text(`RFC: ${proveedorActual.RFC} | ${proveedorActual.ContactoNombre || 'Sin contacto'}`);
    $("#infoProveedor").removeClass("d-none");
}

function agregarProductoAlCarrito(producto) {
    const existente = carrito.find(c => c.ProductoID === producto.ProductoID);
    if (existente) {
        existente.Cantidad++;
    } else {
        carrito.push({
            ProductoID: producto.ProductoID,
            NombreProducto: producto.Nombre,
            Cantidad: 1,
            PrecioCompra: 0,
            PrecioVenta: 0
        });
    }
    actualizarCarrito();
}

function actualizarCarrito() {
    const tbody = $("#tbCarrito tbody");
    tbody.empty();
    let total = 0;

    carrito.forEach((item, i) => {
        const subtotal = item.Cantidad * item.PrecioCompra;
        total += subtotal;

        tbody.append(`
            <tr>
                <td>${item.NombreProducto}</td>
                <td><input type="number" min="1" value="${item.Cantidad}" class="form-control form-control-sm w-75" onchange="actualizarCantidad(${i}, this.value)"></td>
                <td><input type="number" step="0.01" value="${item.PrecioCompra}" class="form-control form-control-sm" onchange="actualizarPrecioCompra(${i}, this.value)"></td>
                <td><input type="number" step="0.01" value="${item.PrecioVenta}" class="form-control form-control-sm" onchange="actualizarPrecioVenta(${i}, this.value)"></td>
                <td class="text-right">$${subtotal.toFixed(2)}</td>
                <td><button class="btn btn-danger btn-sm" onclick="eliminarItem(${i})">X</button></td>
            </tr>
        `);
    });
    $("#lblTotal").text(total.toFixed(2));
}

function finalizarCompra() {
    if (!proveedorActual) { mostrarToast("Selecciona un proveedor", "warning"); return; }
    if (carrito.length === 0) { mostrarToast("Agrega al menos un producto", "warning"); return; }
    if (!$('#txtFolioFactura').val().trim()) { mostrarToast("Ingresa el folio de factura", "warning"); return; }

    const compra = {
        ProveedorID: proveedorActual.ProveedorID,
        FolioFactura: $('#txtFolioFactura').val().trim(),
        FechaCompra: new Date(),
        Total: parseFloat($("#lblTotal").text()),
        Detalle: carrito.map(c => ({
            ProductoID: c.ProductoID,
            Cantidad: c.Cantidad,
            PrecioCompra: c.PrecioCompra,
            PrecioVenta: c.PrecioVenta
        }))
    };

    $.ajax({
        url: "/Compra/RegistrarCompra",
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify(compra)
    }).done(res => {
        if (res.resultado) {
            mostrarToast(res.mensaje || "Compra registrada con éxito", "success");
            carrito = [];
            actualizarCarrito();
            proveedorActual = null;
            $("#infoProveedor").addClass("d-none");
            $("#txtFolioFactura").val('');
        } else {
            mostrarToast(res.mensaje || "Error al registrar compra", "danger");
        }
    });
}

// Funciones auxiliares
function actualizarCantidad(i, val) { carrito[i].Cantidad = parseInt(val(|| 1)); actualizarCarrito(); }
function actualizarPrecioCompra(i, val) { carrito[i].PrecioCompra = parseFloat(val || 0); actualizarCarrito(); }
function actualizarPrecioVenta(i, val) { carrito[i].PrecioVenta = parseFloat(val || 0); actualizarCarrito(); }
function eliminarItem(i) { carrito.splice(i, 1); actualizarCarrito(); }

function mostrarToast(mensaje, tipo = "info") {
    $("#toastTitulo").text(tipo === "success" ? "Éxito" : "Error");
    $("#toastMensaje").text(mensaje);
    $("#liveToast").removeClass().addClass(`toast bg-${tipo === "success" ? "success" : tipo === "danger" ? "danger" : "warning"} text-white`);
    new bootstrap.Toast(document.getElementById('liveToast')).show();
}