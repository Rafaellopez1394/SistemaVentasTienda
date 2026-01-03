/// <reference path="../../Base/js/vendor/jquery-1.11.0-vsdoc.js" />
/// <reference path="../../Base/js/plugins.js" />
/// <reference path="../../Base/Scripts/librerias/extensions/jquery.HelpField-1.1.js" />
(function ($, balor, amplify) {
    var msgbx = $('#divmsg');
    var clock = null;

    var showMsg = function (titulo, mensaje, tipoMensaje) {
        msgbx.mostrarMensaje(titulo, mensaje, tipoMensaje);
    };

    var escondeMsg = function () {
        msgbx.animate({ top: -msgbx.outerHeight(), opacity: 0 }, 500);
    };

    var lightboxOn = function () {
        var lightBox = '<div class="lightbox"></div>';
        $(lightBox).appendTo('body');
    };

    var lightboxOff = function () {
        $('.lightbox').remove();
    };

    var ShowLightBox = function (Mostrar, msj) {
        if (Mostrar == true) {
            var html = '<div class="CtlBalor_Lightbox"></div>';
            html += '<div class="CtlBalor_panel_pop">';
            html += '<span>' + msj + '</span>';
            html += '<br />';
            html += '<img src="../../Base/img/loading.gif" />';
            html += '</div>';
            $(html).appendTo('body');
        } else {
            $(".CtlBalor_Lightbox, .CtlBalor_panel_pop").remove();
        }
    };

    var ejecutaAjax = function (url, tipo, datos, successfunc, errorfunc, async) {
        $.ajax(url, {
            type: tipo || 'POST',
            cache: false,
            async: async,
            dataType: 'json',
            data: datos ? JSON.stringify(datos) : '{}',
            contentType: 'application/json; charset=utf-8',
            success: successfunc,
            error: errorfunc || function (d) {
                console.log(d.responseText);
            }
        });
    };

    var llenarComboEmpresas = function (d) {
        if (d.d.EsValido) {
            //llenaCombo('ddl-empresa', {}, 'Consolidado');
            $('#ddl-empresa').llenaCombo(d.d.Datos);
        }
    };

    var elemById = function (id) {
        return document.getElementById(id);
    };

    var focusNextControl = function (control, e) {
        var code = (e.keyCode ? e.keyCode : e.which);
        if (code === 13) {
            var inputs = $(control).closest('form').find('input[type=text]:enabled, input[type=password]:enabled, input[type=button]:enabled, input[type=checkbox]:enabled, input[type=radio]:enabled, input[type=date]:enabled, input[type=tel]:enabled, input[type=email]:enabled, input[type=number]:enabled, select:enabled').not('input[readonly=readonly], fieldset:hidden *');
            inputs.eq(inputs.index(control) + 1).focus();
            e.preventDefault();
        }
    };

    var fechaActual = function () {
        var fecha = new Date(),
        dd = ('0' + fecha.getDate().toString()).slice(-2),
        mm = ('0' + (fecha.getMonth() + 1).toString()).slice(-2),
        y = fecha.getFullYear().toString();

        return dd + '/' + mm + '/' + y;
    };

    /*
    Codigo de la pagina que se esta migrando
    */

    var getChar = function (event) {
        if (event.which === null) {
            return String.fromCharCode(event.keyCode);  // IE
        } else if (event.which !== 0 && event.charCode !== 0) {
            return String.fromCharCode(event.which);    // the rest
        } else {
            return null;  // special key
        }
    };

    var padLeft = function (nr, n, str) {
        return Array(n - String(nr).length + 1).join(str || '0') + nr;
    };

    var padRight = function (nr, n, str) {
        return nr + Array(n - String(nr).length + 1).join(str || '0');
    };

      var getUsuario = function () {
        return amplify.store.sessionStorage("Usuario");
    };

    var getVendedor = function () {
        return amplify.store.sessionStorage("VendedorID");
    };

    var ValidaImprimir = function () {
        if ($("#fechaInicio").val() == "") {
            showMsg('Alerta', "Debe ingresar la fecha de inicio", 'warning');
            $("#fechaInicio").focus();
            return false;
        }
        if ($("#fechaFinal").val() == "") {
            showMsg('Alerta', "Debe ingresar la fecha final", 'warning');
            $("#fechaFinal").focus();
            return false;
        }
        return true;
    }


    $('body').on('click', '#BotonImprimir', null, function () {
        if (ValidaImprimir()) {
            var parametros = "&fechaInicial=" + $("#fechaInicio").val().trim();
            parametros += "&fechaFinal=" + $("#fechaFinal").val().trim();
            parametros += "&empresaid=" + $('#ddl-empresa option:selected').val();
            //parametros += "&Formato=" + $("input[type='radio'][name='timprimir']:checked").val();
            //window.open("../../Base/Formas/ReporteProduccion.aspx?Formato=0&Nombre=ReporteSaldos" + parametros, 'w_PopReporteSaldos' + $("#fechaInicio").val().trim().replace("/", "") + $("#fechaFinal").val().trim().replace("/", "") + $('#ddl-empresa option:selected').val(), 'toolbar=no,location=no,directories=no,status=no,menubar=no,scrollbars=yes,resizable=yes,width=760,height=560');
            //window.open("../../Base/Formas/ReporteProduccion.aspx?Formato=0&Nombre=ReporteSaldos" + parametros, ('w_PopReporteSaldos' + 'wfewf9840935325').replace("/", ""), 'toolbar=no,location=no,directories=no,status=no,menubar=no,scrollbars=yes,resizable=yes,width=760,height=560');
            window.open("../../Base/Formas/ReporteProduccion.aspx?Formato=0&Nombre=Saldos" + parametros, 'w_PopReporteSaldos' + $("#fechaInicio").val().trim().replace("/", "") + $("#fechaFinal").val().trim().replace("/", "") + $('#ddl-empresa option:selected').val(), 'toolbar=no,location=no,directories=no,status=no,menubar=no,scrollbars=yes,resizable=yes,width=760,height=560');
            //            var TImpresion = 
            //            if (TImpresion === "P") {                
            //            } else {
            //                $("#myDownloaderFrame").attr("src", "../../Base/Formas/GenerarExcel.aspx?NomReporte=RelacionSaldos" + parametros);
            //            }
        }
    });


    $('input').not('input[type=button]').on('keypress', null, function (e) {
        focusNextControl(this, e);
    });

    $('body').on('click', '#MobileMenu', null, function () {
        $("#MobileMenuDet").slideToggle("slow");
    });

    $('body').on('blur', '#fechaInicio,#fechaFinal', null, function () {
        var ffecha = $(this).val();
        $(this).val(balor.appliFormatDate(ffecha));
        if (!balor.isValidDate($(this).val())) {
            $(this).val("");
        }
    });

    var MostrarPagina = function (url) {
        if (url != "") {
            window.location.replace(url);
        }
    };
    var ActualizaMenuBar = function () {
        if (screen.width < 600) {
            $("#MobileMenu").css("display", "line");
            var menu = $("#Menu").html();
            $("#divcontentMenu").remove();
            $("#MobileMenuDet").html(menu);
            $("#nav").css("top", "0");
        }
    }
    var AplicarEstilo = function () {
        $(".svGrid th, .svGrid .header, .svGrid tfoot").css({
            'background': '#ED1D24',
            'font': 'bold 14px Arial, Helvetica, Sans-serif',
            'padding': '3px',
            'color': '#FFFFFF'
        });
        $(".svGrid tbody").css({
            'font-size': '11px',
            'font-weight': 'normal',
            'color': '#036'
        });
        $("#divnvo").css("margin-top", "20px");
        $("#txtConceptoPoliza").css("width", "500px");
    };

    $(document).ready(function () {
        if (amplify.store.sessionStorage("UsuarioID") == null || amplify.store.sessionStorage("UsuarioID") == "") {
            window.location.replace("../../login.aspx");
        }
        $("#Menu").MostrarMenuBar(JSON.parse(amplify.store.sessionStorage("Funcionalidades")), amplify.store.sessionStorage("Nombre"), amplify.store.sessionStorage("NombreEmpresa"));
        $('.popup').setDraggable('.popup-body, .popup-footer, .close');
        ActualizaMenuBar();
        AplicarEstilo();
        //iniciaAyudas();
        ejecutaAjax('ReportePresupuestoDeGastos.aspx/TraerEmpresas', 'POST', null, llenarComboEmpresas, null, false);
    });
}(jQuery, window.balor, window.amplify));