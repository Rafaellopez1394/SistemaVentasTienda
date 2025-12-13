
var tabladata;

$(document).ready(function () {
    activarMenu("Compras");

    $.datepicker.regional['es'] = {
        closeText: 'Cerrar',
        prevText: '< Ant',
        nextText: 'Sig >',
        currentText: 'Hoy',
        monthNames: ['Enero', 'Febrero', 'Marzo', 'Abril', 'Mayo', 'Junio', 'Julio', 'Agosto', 'Septiembre', 'Octubre', 'Noviembre', 'Diciembre'],
        monthNamesShort: ['Ene', 'Feb', 'Mar', 'Abr', 'May', 'Jun', 'Jul', 'Ago', 'Sep', 'Oct', 'Nov', 'Dic'],
        dayNames: ['Domingo', 'Lunes', 'Martes', 'Miércoles', 'Jueves', 'Viernes', 'Sábado'],
        dayNamesShort: ['Dom', 'Lun', 'Mar', 'Mié', 'Juv', 'Vie', 'Sáb'],
        dayNamesMin: ['Do', 'Lu', 'Ma', 'Mi', 'Ju', 'Vi', 'Sá'],
        weekHeader: 'Sm',
        dateFormat: 'dd/mm/yy',
        firstDay: 1,
        isRTL: false,
        showMonthAfterYear: false,
        yearSuffix: ''
    };
    $.datepicker.setDefaults($.datepicker.regional['es']);


    $("#txtFechaInicio").datepicker();
    $("#txtFechaFin").datepicker();
    $("#txtFechaInicio").val(ObtenerFecha());
    $("#txtFechaFin").val(ObtenerFecha());
  

    //OBTENER PROVEEDORES
    jQuery.ajax({
        url: $.MisUrls.url._ObtenerProveedores,
        type: "GET",
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        success: function (data) {
            $("#cboProveedor").LoadingOverlay("hide");
            $("#cboProveedor").html("");

            $("<option>").attr({ "value": 0 }).text("-- Seleccionar todas--").appendTo("#cboProveedor");
            if (data.data != null)
                $.each(data.data, function (i, item) {

                    if (item.Activo == true) {
                        $("<option>").attr({ "value": item.ProveedorID }).text(item.RazonSocial).appendTo("#cboProveedor");
                    }
                })
        },
        error: function (error) {
            console.log(error)
        },
        beforeSend: function () {
            $("#cboProveedor").LoadingOverlay("show");
        },
    });



    //OBTENER Sucursales
    jQuery.ajax({
        url: $.MisUrls.url._ObtenerSucursales,
        type: "GET",
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        success: function (data) {

            $("#cboSucursal").LoadingOverlay("hide");
            $("#cboSucursal").html("");

            $("<option>").attr({ "value": 0 }).text("-- Seleccionar todas--").appendTo("#cboSucursal");
            if (data.data != null)
                $.each(data.data, function (i, item) {

                    if (item.Activo == true) {
                        $("<option>").attr({ "value": item.SucursalID }).text(item.Nombre).appendTo("#cboSucursal");
                    }
                })
        },
        error: function (error) {
            console.log(error)
        },
        beforeSend: function () {
            $("#cboSucursal").LoadingOverlay("show");
        },
    });




    tabladata = $('#tbCompras').DataTable({
        "ajax": {
            "url": $.MisUrls.url._ObtenerCompras + "?fechainicio=" + ObtenerFecha() + "&fechafin=" + ObtenerFecha() + "&idproveedor=0&SucursalID=0",
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            {
                "data": "CompraID", render: function (data) {
                    return "<button class='btn btn-success btn-sm ml-2' type='button' onclick='Imprimir(" + data + ")'><i class='far fa-clipboard'></i> Ver</button>"
                }
            },
            { "data": "NumeroCompra" },
            {
                "data": "oProveedor", render: function (data) {
                    return data.RazonSocial
                }
            },
            {
                "data": "oSucursal", render: function (data) {
                    return data.Nombre
                }
            },
            { "data": "FechaCompra" },
            {
                "data": "TotalCosto", render: function (data) {

                    return "S./ " + (data).toString().replace(/(\d)(?=(\d{3})+(?!\d))/g, "$1,");
                }
            },
           

        ],
        "language": {
            "url": $.MisUrls.url.Url_datatable_spanish
        },
        responsive: true
    });


})


function buscar() {

    if ($("#txtFechaInicio").val().trim() == "" || $("#txtFechaFin").val().trim() == "") {
        swal("Mensaje", "Debe ingresar fechas", "warning")
        return;
    }

    tabladata.ajax.url($.MisUrls.url._ObtenerCompras + "?"+
        "fechainicio=" + $("#txtFechaInicio").val().trim() +
        "&fechafin=" + $("#txtFechaFin").val().trim() +
        "&idproveedor=" + $("#cboProveedor").val() +
        "&SucursalID=" + $("#cboSucursal").val()).load();
}

function ObtenerFecha() {

    var d = new Date();
    var month = d.getMonth() + 1;
    var day = d.getDate();
    var output = (('' + day).length < 2 ? '0' : '') + day + '/' + (('' + month).length < 2 ? '0' : '') + month + '/' + d.getFullYear();

    return output;
}


function Imprimir(id) {
    var texto = $.MisUrls.url._DocumentoCompra + "?compraid=" + id;

    // Open the page in a new tab or window
    var w = window.open(texto);

    //w.onload = function () {
    //    w.print();
    //}

}