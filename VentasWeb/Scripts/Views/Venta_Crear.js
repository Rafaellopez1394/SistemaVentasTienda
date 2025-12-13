var tablaproducto;
var tablacliente;
let tiposPrecio = [];           // Catálogo de tipos de precio
let clienteSeleccionado = null; // Cliente actual

$(document).ready(function () {
    activarMenu("Ventas");
    $("#txtproductocantidad").val("0");
    $("#txtfechaventa").val(ObtenerFecha());

    // CARGAR DATOS DEL USUARIO Y SUCURSAL
    $.ajax({
        url: $.MisUrls.url._ObtenerUsuario,
        type: "GET",
        dataType: "json",
        success: function (data) {
            if (data && data.oSucursal) {
                $("#txtsucursalid").val(data.oSucursal.SucursalID || 0);
                $("#lbltiendanombre").text(data.oSucursal.Nombre || "Sin nombre");
                $("#txtsucursalrfc").text(data.oSucursal.RFC || "-");
                $("#lbltiendadireccion").text(data.oSucursal.Direccion || "-");
            } else {
                swal("Error", "El usuario no tiene sucursal asignada", "error");
                $("#txtsucursalid").val("0");
            }

            $("#txtusuarioid").val(data.UsuarioID || 0);
            $("#lblempleadonombre").text(data.Nombres || "");
            $("#lblempleadoapellido").text(data.Apellidos || "");
            $("#lblempleadocorreo").text(data.Correo || "");

            inicializarTablaProductos();
        },
        error: function () {
            swal("Error", "No se pudieron cargar los datos del usuario", "error");
        }
    });

    // CARGAR TIPOS DE PRECIO y luego inicializar la tabla de clientes
    $.getJSON('/Cliente/ObtenerCatalogos', function (res) {
        tiposPrecio = res.tiposPrecio || [];
        inicializarTablaClientes();
    });

    // Inicialización de filtros de inputs
    $("#txtproductocantidad").inputFilter(function (value) {
        return /^-?\d*$/.test(value);
    });

    $("#txtmontopago").inputFilter(function (value) {
        return /^-?\d*[.]?\d{0,2}$/.test(value);
    });

    // Botones de búsqueda
    $("#btnBuscarProducto").on("click", function () {
        if (tablaproducto) {
            tablaproducto.ajax
                .url($.MisUrls.url._ObtenerProductoStockPorSucursal + "?SucursalID=" + $("#txtsucursalid").val())
                .load();
        }
        $("#modalProducto").modal("show");
    });

    $("#btnBuscarCliente").on("click", function () {
        if (tablacliente) {
            tablacliente.ajax.reload();
        }
        $("#modalCliente").modal("show");
    });

    // Botón calcular cambio
    $("#btncalcular").on("click", function () {
        calcularCambio();
    });

    // Botón agregar producto
    $("#btnAgregar").on("click", function () {
        agregarProducto();
    });

    // Botón terminar venta
    $("#btnTerminarGuardarVenta").on("click", function () {
        registrarVenta();
    });

    // Input código de producto
    $("#txtproductocodigo").on("keypress", function (e) {
        if (e.which == 13) {
            buscarProductoPorCodigo();
        }
    });

    // Eliminar fila de venta
    $("#tbVenta tbody").on("click", "button.btn-danger", function () {
        $(this).parents("tr").remove();
        recalcularPreciosConDescuento();
    });
});

// ==================== FUNCIONES DE TABLAS ====================
function inicializarTablaProductos() {
    if (!$.fn.DataTable.isDataTable('#tbProducto')) {
        tablaproducto = $('#tbProducto').DataTable({
            ajax: {
                url: $.MisUrls.url._ObtenerProductoStockPorSucursal + "?SucursalID=" + $("#txtsucursalid").val(),
                type: "GET",
                dataType: "json",
                dataSrc: "data"
            },
            columns: [
                {
                    data: null,
                    render: data => `<button class='btn btn-sm btn-primary' onclick='productoSelect(${JSON.stringify(data)})' title='Seleccionar'><i class='fas fa-check'></i></button>`,
                    orderable: false,
                    width: "80px"
                },
                { data: "oProducto.Codigo" },
                { data: "oProducto.Nombre" },
                { data: "oProducto.Descripcion" },
                { data: "Stock" },
                { data: "PrecioUnidadVenta", render: d => parseFloat(d).toFixed(2) }
            ],
            language: { url: $.MisUrls.url.Url_datatable_spanish },
            responsive: true,
            autoWidth: false,
            destroy: true
        });
    }
}

function inicializarTablaClientes() {
    if (!$.fn.DataTable.isDataTable('#tbcliente')) {
        tablacliente = $('#tbcliente').DataTable({
            ajax: {
                url: '/Cliente/Obtener',
                type: "GET",
                dataType: "json",
                dataSrc: "data"
            },
            columns: [
                {
                    data: null,
                    render: data => `<button class='btn btn-sm btn-success' onclick='clienteSelect(${JSON.stringify(data)})' title='Seleccionar cliente'><i class='fas fa-check'></i></button>`,
                    orderable: false,
                    width: "80px"
                },
                { data: "RFC" },
                { data: "Nombre" },
                { data: "Telefono" },
                { data: "Ciudad" },
                {
                    data: "TipoPrecioID",
                    render: function (data) {
                        if (!tiposPrecio || tiposPrecio.length === 0) return "Cargando...";
                        const tipo = tiposPrecio.find(t => t.TipoPrecioID == data);
                        return tipo ? `<strong>${tipo.Nombre}</strong>` : "Público General";
                    }
                }
            ],
            language: { url: $.MisUrls.url.Url_datatable_spanish },
            responsive: true,
            autoWidth: false,
            destroy: true
        });
    }
}

// ==================== FUNCIONES AUXILIARES ====================
function ObtenerFecha() {
    var d = new Date();
    var month = d.getMonth() + 1;
    var day = d.getDate();
    return (('' + day).length < 2 ? '0' : '') + day + '/' +
           (('' + month).length < 2 ? '0' : '') + month + '/' + d.getFullYear();
}

$.fn.inputFilter = function (inputFilter) {
    return this.on(
        "input keydown keyup mousedown mouseup select contextmenu drop",
        function () {
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
        }
    );
};

// ==================== SELECCIONAR PRODUCTO ====================
function productoSelect(json) {
    $("#txtproductoid").val(json.oProducto.ProductoID);
    $("#txtproductocodigo").val(json.oProducto.Codigo);
    $("#txtproductonombre").val(json.oProducto.Nombre);
    $("#txtproductodescripcion").val(json.oProducto.Descripcion);
    $("#txtproductostock").val(json.Stock);
    $("#txtproductoprecio").val(json.PrecioUnidadVenta);
    $("#txtproductocantidad").val("0");
    $("#modalProducto").modal("hide");
}

// ==================== SELECCIONAR CLIENTE ====================
function clienteSelect(json) {
    clienteSeleccionado = json;

    $("#txtclientedocumento").val(json.RFC || "");
    $("#txtclientenombres").val(json.Nombre || "");
    $("#txtclientedireccion").val(json.Direccion || "");
    $("#txtclientetelefono").val(json.Telefono || "");

    const tipo = tiposPrecio.find((t) => t.TipoPrecioID == json.TipoPrecioID);
    $("#lblTipoCliente").text(tipo ? tipo.Nombre : "Público General");

    $("#modalCliente").modal("hide");
    recalcularPreciosConDescuento();
}

// ==================== DESCUENTO AUTOMÁTICO ====================
function recalcularPreciosConDescuento() {
    const tipo = tiposPrecio.find(
        (t) => t.TipoPrecioID == (clienteSeleccionado?.TipoPrecioID || 1)
    );
    const porcentajeDescuento = tipo?.Cargo || 0;

    let total = 0;
    $("#tbVenta tbody tr").each(function () {
        const precioBase = parseFloat($(this).find("td.productoprecio").text());
        const cantidad = parseInt($(this).find("td.productocantidad").text());

        const precioFinal = precioBase * (1 + porcentajeDescuento / 100);
        const importe = precioFinal * cantidad;

        $(this).find("td.productoprecio").text(precioFinal.toFixed(2));
        $(this).find("td.importetotal").text(importe.toFixed(2));

        total += importe;
    });

    const igv = total * 0.18;
    $("#txtsubtotal").val((total / 1.18).toFixed(2));
    $("#txtigv").val(igv.toFixed(2));
    $("#txttotal").val(total.toFixed(2));
}

// ==================== FUNCIONES DE PRODUCTOS ====================
function agregarProducto() {
    const cantidad = parseInt($("#txtproductocantidad").val() || 0);
    if ($("#txtproductoid").val() == "0" || cantidad === 0) {
        swal("Error", "Completa producto y cantidad", "warning");
        return;
    }

    let precio = parseFloat($("#txtproductoprecio").val());
    const tipo = tiposPrecio.find(
        (t) => t.TipoPrecioID == (clienteSeleccionado?.TipoPrecioID || 1)
    );
    if (tipo && tipo.Cargo !== 0) {
        precio = precio * (1 + tipo.Cargo / 100);
    }

    const importe = precio * cantidad;

    $("<tr>")
        .append(
            $("<td>").append(
                $("<button>")
                    .addClass("btn btn-danger btn-sm")
                    .text("X")
                    .data("idproducto", $("#txtproductoid").val())
                    .data("cantidadproducto", cantidad)
            ),
            $("<td class='productocantidad'>").text(cantidad),
            $("<td class='producto'>")
                .data("idproducto", $("#txtproductoid").val())
                .text($("#txtproductonombre").val()),
            $("<td>").text($("#txtproductodescripcion").val()),
            $("<td class='productoprecio'>").text(precio.toFixed(2)),
            $("<td class='importetotal'>").text(importe.toFixed(2))
        )
        .appendTo("#tbVenta tbody");

    limpiarCamposProducto();
    recalcularPreciosConDescuento();
}

function limpiarCamposProducto() {
    $("#txtproductoid, #txtproductocodigo, #txtproductonombre, #txtproductodescripcion, #txtproductostock, #txtproductoprecio").val("");
    $("#txtproductocantidad").val("1");
    $("#txtproductocodigo").focus();
}

// ==================== REGISTRAR VENTA ====================
function registrarVenta() {
    // Validaciones
    if ($("#txtclientedocumento").val().trim() == "" || $("#txtclientenombres").val().trim() == "") {
        swal("Mensaje", "Complete los datos del cliente", "warning");
        return;
    }
    if ($("#tbVenta tbody tr").length == 0) {
        swal("Mensaje", "Debe registrar mínimo un producto en la venta", "warning");
        return;
    }
    if ($("#txtmontopago").val().trim() == "") {
        swal("Mensaje", "Ingrese el monto de pago", "warning");
        return;
    }

    let totalProductos = 0;
    let totalImportes = 0;
    let DATOS_VENTA = "";

    $("#tbVenta > tbody > tr").each(function (index, tr) {
        var fila = tr;
        var cantidad = parseInt($(fila).find("td.productocantidad").text());
        var idproducto = $(fila).find("td.producto").data("idproducto");
        var precio = parseFloat($(fila).find("td.productoprecio").text());
        var importe = parseFloat($(fila).find("td.importetotal").text());

        totalProductos += cantidad;
        totalImportes += importe;

        DATOS_VENTA += `<DATOS><VentaID>0</VentaID><ProductoID>${idproducto}</ProductoID><Cantidad>${cantidad}</Cantidad><PrecioUnidad>${precio}</PrecioUnidad><ImporteTotal>${importe}</ImporteTotal></DATOS>`;
    });

    const VENTA = `<VENTA><SucursalID>${$("#txtsucursalid").val()}</SucursalID><UsuarioID>${$("#txtusuarioid").val()}</UsuarioID><ClienteID>0</ClienteID><TipoDocumento>${$("#cboventatipodocumento").val()}</TipoDocumento><CantidadProducto>${$("#tbVenta tbody tr").length}</CantidadProducto><CantidadTotal>${totalProductos}</CantidadTotal><TotalCosto>${totalImportes}</TotalCosto><ImporteRecibido>${$("#txtmontopago").val()}</ImporteRecibido><ImporteCambio>${$("#txtcambio").val()}</ImporteCambio></VENTA>`;

    const DETALLE_CLIENTE = `<DETALLE_CLIENTE><DATOS><TipoDocumento>${$("#cboclientetipodocumento").val()}</TipoDocumento><NumeroDocumento>${$("#txtclientedocumento").val()}</NumeroDocumento><Nombre>${$("#txtclientenombres").val()}</Nombre><Direccion>${$("#txtclientedireccion").val()}</Direccion><Telefono>${$("#txtclientetelefono").val()}</Telefono></DATOS></DETALLE_CLIENTE>`;

    const DETALLE_VENTA = `<DETALLE_VENTA>${DATOS_VENTA}</DETALLE_VENTA>`;
    const DETALLE = `<DETALLE>${VENTA}${DETALLE_CLIENTE}${DETALLE_VENTA}</DETALLE>`;

    const request = { xml: DETALLE };

    jQuery.ajax({
        url: $.MisUrls.url._RegistrarVenta,
        type: "POST",
        data: JSON.stringify(request),
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        beforeSend: function () {
            $(".card-venta").LoadingOverlay("show");
        },
        success: function (data) {
            $(".card-venta").LoadingOverlay("hide");
            if (data.estado) {
                limpiarVenta();
                window.open($.MisUrls.url._DocumentoVenta + "?VentaID=" + data.valor);
            } else {
                swal("Mensaje", "No se pudo registrar la venta", "warning");
            }
        },
        error: function (error) {
            console.log(error);
            $(".card-venta").LoadingOverlay("hide");
        }
    });
}

// ==================== FUNCIONES DE CAMBIO ====================
function calcularCambio() {
    var montopago = $("#txtmontopago").val().trim() === "" ? 0 : parseFloat($("#txtmontopago").val().trim());
    var totalcosto = parseFloat($("#txttotal").val().trim());
    var cambio = (montopago <= totalcosto ? totalcosto : montopago) - totalcosto;
    $("#txtcambio").val(cambio.toFixed(2));
}

// ==================== LIMPIAR VENTA ====================
function limpiarVenta() {
    $("#cboventatipodocumento").val("Boleta");
    $("#cboclientetipodocumento").val("DNI");
    $("#txtclientedocumento, #txtclientenombres, #txtclientedireccion, #txtclientetelefono").val("");
    $("#txtproductoid, #txtproductocodigo, #txtproductonombre, #txtproductodescripcion, #txtproductostock, #txtproductoprecio").val("");
    $("#txtproductocantidad").val("0");
    $("#txtsubtotal, #txtigv, #txttotal, #txtmontopago, #txtcambio").val("0");
    $("#tbVenta tbody").html("");
}

// ==================== BUSCAR PRODUCTO POR CODIGO ====================
function buscarProductoPorCodigo() {
    var codigo = $("#txtproductocodigo").val().trim();
    if (!codigo) return;

    jQuery.ajax({
        url: $.MisUrls.url._ObtenerProductoStockPorSucursal + "?SucursalID=" + parseInt($("#txtsucursalid").val()),
        type: "GET",
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        success: function (data) {
            var encontrado = false;
            if (data.data != null) {
                $.each(data.data, function (i, item) {
                    if (item.oProducto.Codigo == codigo) {
                        productoSelect(item);
                        encontrado = true;
                        return false;
                    }
                });
            }
            if (!encontrado) {
                limpiarCamposProducto();
                $("#txtproductocodigo").val("");
            }
        },
        error: function (error) { console.log(error); },
        beforeSend: function () { $("#cboProveedor").LoadingOverlay("show"); }
    });
}

// ==================== CONTROL DE STOCK ====================
function controlarStock($idproducto, $SucursalID, $cantidad, $restar) {
    var request = { idproducto: $idproducto, SucursalID: $SucursalID, cantidad: $cantidad, restar: $restar };
    jQuery.ajax({
        url: $.MisUrls.url._ControlarStockProducto,
        type: "POST",
        data: JSON.stringify(request),
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        success: function () {},
        error: function (error) { console.log(error); }
    });
}

window.onbeforeunload = function () {
    $("#tbVenta > tbody > tr").each(function (index, tr) {
        var fila = tr;
        var cantidad = parseInt($(fila).find("td.productocantidad").text());
        var idproducto = $(fila).find("td.producto").data("idproducto");
        controlarStock(parseInt(idproducto), parseInt($("#txtsucursalid").val()), cantidad, false);
    });
};
