
var table;



$(document).ready(function () {
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
    // activarMenu("Reportes"); // No necesario - el menú se activa automáticamente

    $("#txtFechaInicio").datepicker();
    $("#txtFechaFin").datepicker();
    $("#txtFechaInicio").val(ObtenerFecha());
    $("#txtFechaFin").val(ObtenerFecha());


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

});

$('#btnBuscar').on('click', function () {

    jQuery.ajax({
        url: $.MisUrls.url._ObtenerReporteVenta + "?fechainicio=" + $("#txtFechaInicio").val() + "&fechafin=" + $("#txtFechaFin").val() + "&SucursalID=" + $("#cboSucursal").val() ,
        type: "GET",
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        success: function (data) {

            if (data != undefined && data != null) {

                $("#tbReporte tbody").html("");


                $.each(data, function (i, row) {

                    // Convertir fecha .NET
                    var fechaFormateada = row["FechaVenta"];
                    if (fechaFormateada && fechaFormateada.includes('/Date(')) {
                        var timestamp = parseInt(fechaFormateada.replace(/\/Date\((\d+)\)\//, '$1'));
                        var fecha = new Date(timestamp);
                        var dia = ('0' + fecha.getDate()).slice(-2);
                        var mes = ('0' + (fecha.getMonth() + 1)).slice(-2);
                        var anio = fecha.getFullYear();
                        fechaFormateada = dia + '/' + mes + '/' + anio;
                    }

                    $('<tr>').append(
                        $('<td>').text(fechaFormateada),
                        $('<td>').text(row['NumeroDocumento']),
                        $('<td>').text(row['TipoDocumento']),
                        $('<td>').text(row['NombreSucursal']),
                        $('<td>').text(row['RucSucursal']),
                        $('<td>').text(row['NombreEmpleado']),
                        $('<td>').text(row['CantidadUnidadesVendidas']),
                        $('<td>').text(row['CantidadProductos']),
                        $('<td>').text(row['TotalVenta'])

                    ).appendTo('#tbReporte tbody');

                })

            }

        },
        error: function (error) {
            console.log(error)
        },
        beforeSend: function () {
        },
    });
})



function ObtenerFecha() {

    var d = new Date();
    var month = d.getMonth() + 1;
    var day = d.getDate();
    var output = (('' + day).length < 2 ? '0' : '') + day + '/' + (('' + month).length < 2 ? '0' : '') + month + '/' + d.getFullYear();

    return output;
}

function printData() {

    if ($('#tbReporte tbody tr').length == 0) {
        swal("Mensaje", "No existen datos para imprimir", "warning")
        return;
    }

    var divToPrint = document.getElementById("tbReporte");

    var style = "<style>";
    style = style + "table {width: 100%;font: 17px Calibri;}";
    style = style + "table, th, td {border: solid 1px #DDD; border-collapse: collapse;";
    style = style + "padding: 2px 3px;text-align: center;}";
    style = style + "</style>";

    newWin = window.open("");


    newWin.document.write(style);
    newWin.document.write("<h3>Reporte de Ventas</h3>");
    newWin.document.write(divToPrint.outerHTML);
    newWin.print();
    newWin.close();
}