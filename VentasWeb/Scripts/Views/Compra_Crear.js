var tabladata;
var tablaproveedor;
var tablatienda;
var tablaproducto;


$(document).ready(function () {
    //OBTENER PROVEEDORES
    tablaproveedor = $('#tbProveedor').DataTable({
        "ajax": {
            "url": $.MisUrls.url._ObtenerProveedores,
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            {
                "data": "ProveedorID", "render": function (data, type, row, meta) {
                    return "<button class='btn btn-sm btn-primary ml-2' type='button' onclick='proveedorSelect(" + JSON.stringify(row) + ")'><i class='fas fa-check'></i></button>"
                },
                "orderable": false,
                "searchable": false,
                "width": "90px"
            },
            { "data": "RFC" },
            { "data": "RazonSocial" },
            { "data": "Direccion" }

        ],
        "language": {
            "url": $.MisUrls.url.Url_datatable_spanish
        },
        responsive: true
    });

    //OBTENER Sucursales
    tablatienda = $('#tbSucursal').DataTable({
        "ajax": {
            "url": $.MisUrls.url._ObtenerSucursales,
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            {
                "data": "SucursalID", "render": function (data, type, row, meta) {
                    return "<button class='btn btn-sm btn-primary ml-2' type='button' onclick='Sucursaleselect(" + JSON.stringify(row) + ")'><i class='fas fa-check'></i></button>" 
                },
                "orderable": false,
                "searchable": false,
                "width": "90px"
            },
            { "data": "RFC" },
            { "data": "Nombre" },
            { "data": "Direccion" }

        ],
        "language": {
            "url": $.MisUrls.url.Url_datatable_spanish
        },
        responsive: true
    });

    //OBTENER PRODUCTOS
    tablaproducto = $('#tbProducto').DataTable({
        "ajax": {
            "url": $.MisUrls.url._ObtenerProductosPorSucursal + "?SucursalID=0",
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            {
                "data": "ProductoID", "render": function (data, type, row, meta) {
                    return "<button class='btn btn-sm btn-primary ml-2' type='button' onclick='productoSelect(" + JSON.stringify(row) + ")'><i class='fas fa-check'></i></button>"
                },
                "orderable": false,
                "searchable": false,
                "width": "90px"
            },
            { "data": "CodigoInterno", defaultContent: "" },
            { "data": "Nombre" },
            { "data": "NombreCategoria", defaultContent: "Sin categoría" }

        ],
        "language": {
            "url": $.MisUrls.url.Url_datatable_spanish
        },
        responsive: true
    });

})

// Helper seguro para mostrar mensajes (SweetAlert2 / SweetAlert / alert)
function mostrarMensaje(titulo, texto, tipo) {
    try {
        if (window.Swal && typeof Swal.fire === 'function') {
            Swal.fire(titulo || '', texto || '', tipo || 'info');
            return;
        }
        if (typeof swal === 'function') {
            swal(titulo || '', texto || '', tipo || 'info');
            return;
        }
    } catch (e) { /* ignorar y hacer fallback */ }
    alert(((titulo ? (titulo + ': ') : '') + (texto || '')));
}

function buscarProveedor() {
    tablaproveedor.ajax.reload();
    $('#modalProveedor').modal('show');
}


function buscarSucursal() {
    tablatienda.ajax.reload();
    $('#modalSucursal').modal('show');
}

function buscarProducto() {
    if (parseInt($("#txtsucursalid").val()) == 0) {
        mostrarMensaje("Mensaje", "Debe seleccionar una tienda primero", "warning")
        return;
    }
    tablaproducto.ajax.url($.MisUrls.url._ObtenerProductosPorSucursal + "?SucursalID=" + $("#txtsucursalid").val()).load();

    $('#modalProducto').modal('show');
}

function proveedorSelect(json) {

    $("#txtIdProveedor").val(json.ProveedorID);
    $("#txtRFCProveedor").val(json.RFC);
    $("#txtRazonSocialProveedor").val(json.RazonSocial);

    $('#modalProveedor').modal('hide');
}

function Sucursaleselect(json) {
    $("#txtsucursalid").val(json.SucursalID);
    $("#txtRFCSucursal").val(json.RFC);
    $("#txtNombreSucursal").val(json.Nombre);

    $('#modalSucursal').modal('hide');
}

function productoSelect(json) {
    $("#txtproductoid").val(json.ProductoID);
    $("#txtCodigoProducto").val(json.Codigo);
    $("#txtNombreProducto").val(json.Nombre);

    $('#modalProducto').modal('hide');
}



$("#txtCodigoProducto").on('keypress', function (e) {

    if (e.which == 13) {
        
        //OBTENER PRODUCTOS
        jQuery.ajax({
            url: $.MisUrls.url._ObtenerProductos,
            type: "GET",
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                $("#txtCodigoProducto").LoadingOverlay("hide");
                var encontrado = false;
                if (data.data != null) {
                    $.each(data.data, function (i, item) {
                        if (item.Activo == true && item.Codigo == $("#txtCodigoProducto").val()) {

                            $("#txtproductoid").val(item.ProductoID);
                            $("#txtCodigoProducto").val(item.Codigo);
                            $("#txtNombreProducto").val(item.Nombre);

                            encontrado = true;
                            return false;
                        }
                    })

                    if (!encontrado) {
                        $("#txtproductoid").val("0");
                        $("#txtNombreProducto").val("");
                    }
                }
            },
            error: function (error) {
                console.log(error)
            },
            beforeSend: function () {
                $("#txtCodigoProducto").LoadingOverlay("show");
            },
        });


    }
});

$.fn.inputFilter = function (inputFilter) {
    return this.on("input keydown keyup mousedown mouseup select contextmenu drop", function () {
        if (inputFilter(this.value)) {
            this.oldValue = this.value;
            this.oldSelectionStart = this.selectionStart;
            this.oldSelectionEnd = this.selectionEnd;
        } else if (this.hasOwnProperty("oldValue")) {
            this.value = this.oldValue;
            this.setSelectionRange(this.oldSelectionStart, this.oldSelectionEnd);
        } else {
            this.value = "";
        }
    });
};

$("#txtCantidadProducto").inputFilter(function (value) {
    return /^-?\d*$/.test(value);
});

$("#txtPrecioCompraProducto").inputFilter(function (value) {
    return /^-?\d*[.]?\d{0,2}$/.test(value);
});

$("#txtPrecioVentaProducto").inputFilter(function (value) {
    return /^-?\d*[.]?\d{0,2}$/.test(value);
});



$('#btnAgregarCompra').on('click', function () {

    var existe_codigo = false;
    if (
        parseInt($("#txtIdProveedor").val()) == 0 ||
        parseInt($("#txtsucursalid").val()) == 0 ||
        parseInt($("#txtproductoid").val()) == 0 ||
        parseFloat($("#txtCantidadProducto").val()) == 0 ||
        parseFloat($("#txtPrecioCompraProducto").val()) == 0 ||
        parseFloat($("#txtPrecioVentaProducto").val()) == 0
    ) {
        mostrarMensaje("Mensaje", "Debe completar todos los campos", "warning")
        return;
    }

    $('#tbCompra > tbody  > tr').each(function (index, tr) {
        var fila = tr;
        var codigoproducto = $(fila).find("td.codigoproducto").text();

        if (codigoproducto == $("#txtCodigoProducto").val()) {
            existe_codigo = true;
            return false;
        }

    });

    if (!existe_codigo) {
        $("<tr>").append(
            $("<td>").append(
                $("<button>").addClass("btn btn-danger btn-sm").text("Eliminar")
            ),
            $("<td>").append($("#txtRFCProveedor").val()),
            $("<td>").append($("#txtRFCSucursal").val()),
            $("<td>").addClass("codigoproducto").data("idproducto", $("#txtproductoid").val()).append($("#txtCodigoProducto").val()),
            $("<td>").append($("#txtNombreProducto").val()),
            $("<td>").addClass("cantidad").append($("#txtCantidadProducto").val()),
            $("<td>").addClass("preciocompra").append($("#txtPrecioCompraProducto").val()),
            $("<td>").addClass("precioventa").append($("#txtPrecioVentaProducto").val()),
        ).appendTo("#tbCompra tbody");

        $("#txtproductoid").val("0");
        $("#txtCodigoProducto").val("");
        $("#txtNombreProducto").val("");
        $("#txtCantidadProducto").val("0");
        $("#txtPrecioCompraProducto").val("0");
        $("#txtPrecioVentaProducto").val("0");

    } else {
        mostrarMensaje("Mensaje", "El producto ya existe en la compra", "warning")
    }
})

$('#tbCompra tbody').on('click', 'button[class="btn btn-danger btn-sm"]', function () {
    $(this).parents("tr").remove();
})



$('#btnTerminarGuardarCompra').on('click', function () {


    if ($('#tbCompra > tbody  > tr').length == 0) {
        mostrarMensaje("Mensaje", "No existen detalles", "warning")
        return;
    }

    var $xml = "";
    var compra = "";
    var detallecompra = ""
    var detalle = "";
    var totalcostocompra = 0;

    $xml = "<DETALLE>";
    compra = "<COMPRA>" +
        "<UsuarioID>!UsuarioID¡</UsuarioID>" +
        "<ProveedorID>" + $("#txtIdProveedor").val() + "</ProveedorID>" +
        "<SucursalID>" + $("#txtsucursalid").val() + "</SucursalID>" +
        "<TotalCosto>!totalcosto¡</TotalCosto>" +
        "</COMPRA>";
    detallecompra = "<DETALLE_COMPRA>"

    $('#tbCompra > tbody  > tr').each(function (index, tr) {

        var fila = tr;
        var idproducto = parseFloat($(fila).find("td.codigoproducto").data("idproducto"));
        var cantidad = parseFloat($(fila).find("td.cantidad").text());
        var preciocompra = parseFloat($(fila).find("td.preciocompra").text());
        var precioventa = parseFloat($(fila).find("td.precioventa").text());
        var totalcosto = parseFloat(cantidad) * parseFloat(preciocompra);

        detalle = detalle + "<DETALLE>" +
            "<CompraID>0</CompraID>" +
            "<ProductoID>" + idproducto + "</ProductoID>" +
            "<Cantidad>" + cantidad + "</Cantidad>" +
            "<PrecioUnidadCompra>" + preciocompra + "</PrecioUnidadCompra>" +
            "<PrecioUnidadVenta>" + precioventa + "</PrecioUnidadVenta>" +
            "<TotalCosto>" + totalcosto.toString() + "</TotalCosto>" +
            "</DETALLE>";
        totalcostocompra = totalcostocompra + totalcosto;

    });

    compra = compra.replace("!totalcosto¡", totalcostocompra.toString());
    $xml = $xml + compra + detallecompra + detalle + "</DETALLE_COMPRA></DETALLE>";

    var request = { xml: $xml };



    jQuery.ajax({
        url: $.MisUrls.url._GuardarCompra,
        type: "POST",
        data: JSON.stringify(request),
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        success: function (data) {
            $.LoadingOverlay("hide");

            if (data.resultado) {

                //PROVEEDOR
                $("#txtIdProveedor").val("0");
                $("#txtRFCProveedor").val("");
                $("#txtRazonSocialProveedor").val("");

                //TIENDA
                $("#txtsucursalid").val("0");
                $("#txtRFCSucursal").val("");
                $("#txtNombreSucursal").val("");

                //PRODUCTO
                $("#txtproductoid").val("0");
                $("#txtCodigoProducto").val("");
                $("#txtNombreProducto").val("");
                $("#txtCantidadProducto").val("0");
                $("#txtPrecioCompraProducto").val("0");
                $("#txtPrecioVentaProducto").val("0");

                $("#tbCompra tbody").html("");

                mostrarMensaje("Mensaje", "Se registro la compra", "success")
            } else {

                mostrarMensaje("Mensaje", "No se pudo registrar la compra", "warning")
            }
        },
        error: function (error) {
            console.log(error)
        },
        beforeSend: function () {
            $.LoadingOverlay("show");
        },
    });

 

})
