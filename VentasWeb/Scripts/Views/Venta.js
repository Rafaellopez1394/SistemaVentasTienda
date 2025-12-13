let clienteActual = null;
let carrito = [];

$(document).ready(function () {
    $("#txtBuscarCliente").autocomplete({
        source: function (request, response) {
            $.get("/Venta/BuscarCliente", { texto: request.term }, function (data) {
                response(data.map(c => ({ label: c.RazonSocial + " - " + c.RFC, value: c.RFC, datos: c })));
            });
        },
        select: function (event, ui) {
            clienteActual = ui.item.datos;
            mostrarInfoCliente();
        }
    });

    $("#txtBuscarProducto").autocomplete({
        source: function (request, response) {
            $.get("/Venta/ObtenerProductos", { termino: request.term }, function (data) {
                response(data.map(item => ({
                    label: item.Nombre, // Texto que se muestra en la lista de sugerencias
                    value: item.Nombre, // Valor que se pondrX­a en el input (aunque lo prevenimos)
                    datos: item       // Objeto completo del producto para usarlo en la funciX³n 'select'
                })));
            });
        },
        minLength: 1, // Empezar a buscar desde el primer caracter
        select: function (event, ui) {
            // Al seleccionar un producto, llamamos a agregarProducto con el objeto completo
            agregarProducto(ui.item.datos);
            $(this).val(''); // Limpiamos el input de bXºsqueda despuX©s de seleccionar
            return false;    // Prevenimos que el 'value' (nombre del producto) se quede en el input
        }
    });
});

function mostrarInfoCliente() {
    if (!clienteActual) return;
    $("#lblCliente").text(clienteActual.RazonSocial);
    $.get("/Venta/ObtenerCreditosCliente", { clienteId: clienteActual.ClienteID })
        .done(creditos => {
            const limite = creditos.reduce((a, c) => a + (c.LimiteDinero || 0), 0);
            $("#lblCredito").text(`Lñ¯Â¿Â½mite de crñ¯Â¿Â½dito: $${limite.toFixed(2)}`);
        });
    $("#infoCliente").removeClass("d-none");
}
function agregarProducto(producto) {
    $.get("/Venta/ObtenerLotesProducto", { productoId: producto.ProductoID })
        .done(lotes => {
            if (lotes.length === 0) {
                alert("Sin stock disponible");
                return;
            }
            const lote = lotes[0]; // FEFO automático

            if (lote.Stock <= 0) {
                alert("Sin stock disponible para el lote " + lote.LoteID);
                return;
            }

            const existente = carrito.find(c => c.ProductoID === producto.ProductoID && c.LoteID === lote.LoteID);

            if (existente) {
                if (existente.Cantidad < existente.Stock) {
                    existente.Cantidad++;
                } else {
                    alert("Stock máximo alcanzado para este producto/lote.");
                }
            } else {
                carrito.push({
                    ProductoID: producto.ProductoID,
                    Producto: producto.Nombre,
                    LoteID: lote.LoteID,
                    Cantidad: 1,
                    PrecioVenta: lote.PrecioVenta,
                    PrecioCompra: lote.PrecioCompra,
                    Stock: lote.Stock // Guardamos el stock del lote
                });
            }
            actualizarCarrito();
        });
}


function actualizarCarrito() {
    const tbody = $("#tbCarrito tbody");
    tbody.empty();
    let total = 0;

    carrito.forEach((item, i) => {
        const precio = parseFloat(item.PrecioVenta || 0);
        const subtotal = item.Cantidad * precio;
        total += subtotal;
        tbody.append(`
            <tr>
                <td>${item.Producto}</td>
                <td>${item.LoteID}</td>
                <td><input type="number" min="1" value="${item.Cantidad}" onchange="actualizarCantidad(${i}, this.value)" class="form-control form-control-sm w-50"></td>
                <td>$${precio.toFixed(2)}</td>
                <td>$${subtotal.toFixed(2)}</td>
                <td><button class="btn btn-danger btn-sm" onclick="eliminarItem(${i})">X</button></td>
            </tr>
        `);
    });
    $("#lblTotal").text(total.toFixed(2));
}

function finalizarVenta() {
    if (!clienteActual) { alert("Selecciona un cliente"); return; }
    if (carrito.length === 0) { alert("Agrega productos"); return; }

    const venta = {
        ClienteID: clienteActual.ClienteID,
        Total: parseFloat($("#lblTotal").text()),
        Estatus: $("#cboTipoVenta").val(),
        FechaVenta: new Date(),
        Detalle: carrito.map(c => ({
            ProductoID: c.ProductoID,
            LoteID: c.LoteID,
            Cantidad: c.Cantidad
        }))
    };

    $.ajax({
        url: "/Venta/RegistrarVenta",
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify(venta)
    }).done(res => {
        if (res.resultado) {
            alert(res.mensaje || "Venta registrada");
            carrito = [];
            actualizarCarrito();
            clienteActual = null;
            $("#infoCliente").addClass("d-none");
        } else {
            alert(res.mensaje);
        }
    });
}

// Funciones auxiliares
function actualizarCantidad(i, cant) {
    const item = carrito[i];
    const nuevaCantidad = parseInt(cant) || 1;

    // Esta validacion dependera de que 'item.Stock' se aX±ada en 'agregarProducto'
    if (item.Stock && nuevaCantidad > item.Stock) {
        alert('La cantidad no puede ser mayor al stock disponible (' + item.Stock + ').');
        item.Cantidad = item.Stock; // Se ajusta al maximo disponible
    } else if (nuevaCantidad < 1) {
        item.Cantidad = 1;
    } else {
        item.Cantidad = nuevaCantidad;
    }
    actualizarCarrito();
}
function eliminarItem(i) { carrito.splice(i, 1); actualizarCarrito(); }