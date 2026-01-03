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

    var muestraPopup = function (popid) {
        lightboxOn();
        var popup = $('#' + popid),
        width = popup.actual('width'),
        left = (window.innerWidth / 2) - (width / 2),
        top = window.innerHeight * 0.05;
        popup.css({ 'position': 'fixed', 'top': top, 'left': left });
        popup.addClass('fadeInDown').removeClass('hidden');
    };

    var cerrarPopup = function (popid) {
        $('#' + popid).addClass('hidden').removeClass('fadeInDown');
        lightboxOff();
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

    var llenarSubcuentas = function (cuenta) {
        var datos = formatoCuenta(cuenta).split('-'),
        len = datos.length < 6 ? datos.length : 6;
        for (var i = 0; i < len; i++) {
            if (datos[i].length < 4) {
                datos[i] = padLeft(datos[i], 4);
            }
        }
        return datos.join('-');
    };

    var formatoCuenta = function (cuenta) {
        var singuion = cuenta.replace(/-/g, ''),
        len = singuion.length,
        div = Math.floor(len / 4),
        arreglo = [];
        for (var i = 0; i <= div; i++) {
            var substr = singuion.substring(4 * i, (4 * i) + 4);
            if (substr) {
                arreglo.push(substr);
            }
        }
        return arreglo.join('-').substring(0, 29);
    };
    //**************************************EVENTOS AYUDA CUENTA INICIAL Y CUENTA FINAL


    var getUsuario = function () {
        return amplify.store.sessionStorage("Usuario");
    };

    var getVendedor = function () {
        return amplify.store.sessionStorage("VendedorID");
    };

    var ValidaImprimir = function () {
        if ($("#fecha").val() == "") {
            showMsg('Alerta', "Debe ingresar la fecha de inicio", 'warning');
            $("#fechaInicio").focus();
            return false;
        }
        return true;
    }


    $('body').on('click', '#BotonImprimir', null, function () {
        if (ValidaImprimir()) {
            var parametros = "&fecha=" + $("#fecha").val().trim();
            parametros += "&empresaid=" + $('#ddl-empresa option:selected').val();
            parametros += "&mostrarcomprobacion=" + ($("#chkmostrarcomprobacion").prop('checked') ? "1" : "0");
            parametros += "&mostrarcomprobacionadetalle=" + ($("#chkmostrardetallecomprobacion").prop('checked') ? "1" : "0");

            var randomnumber = Math.floor((Math.random() * 100) + 1);
            if ($("#chkRelacionSaldos").is(":checked")){

                window.open("../../Base/Formas/ReporteProduccion.aspx?Formato=0&Nombre=ResumenRelacionSaldos" + parametros, 'w_PopRelacionSaldos' + randomnumber, 'toolbar=no,location=no,directories=no,status=no,menubar=no,scrollbars=yes,resizable=yes,width=760,height=560');
            }
            randomnumber = Math.floor((Math.random() * 100) + 1);
            if($("#chkGastos").is(":checked")){
                window.open("../../Base/Formas/ReporteProduccion.aspx?Formato=0&Nombre=Gastos" + parametros, 'w_PopRelacionSaldos' + randomnumber, 'toolbar=no,location=no,directories=no,status=no,menubar=no,scrollbars=yes,resizable=yes,width=760,height=560');
            }
            randomnumber = Math.floor((Math.random() * 100) + 1);
            if ($("#chkEstadoResultados").prop("checked")) {
                window.open("../../Base/Formas/ReporteProduccion.aspx?Formato=0&Nombre=EstadoDeResultadosDetalle" + parametros, 'w_PopRelacionSaldos' + randomnumber, 'toolbar=no,location=no,directories=no,status=no,menubar=no,scrollbars=yes,resizable=yes,width=760,height=560');
            }
            randomnumber = Math.floor((Math.random() * 100) + 1);
            if ($("#chkEstadosResultadosConsolidado").prop("checked")) {
                window.open("../../Base/Formas/ReporteProduccion.aspx?Formato=0&Nombre=EstadoDeResultadosDetalleConsolidado" + parametros, 'w_PopRelacionSaldos' + randomnumber, 'toolbar=no,location=no,directories=no,status=no,menubar=no,scrollbars=yes,resizable=yes,width=760,height=560');
            }
        }
    });


    $('input').not('input[type=button]').on('keypress', null, function (e) {
        focusNextControl(this, e);
    });

    $('body').on('click', '#MobileMenu', null, function () {
        $("#MobileMenuDet").slideToggle("slow");
    });

    $('body').on('blur', '#fecha', null, function () {
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
        $("#Menu").MostrarMenuBar(JSON.parse(amplify.store.sessionStorage("Funcionalidades")), amplify.store.sessionStorage("Nombre"), amplify.store.sessionStorage("NombreEmpresa"), amplify.store.sessionStorage("UsuarioID"));
        $('.popup').setDraggable('.popup-body, .popup-footer, .close');
        ActualizaMenuBar();
        AplicarEstilo();       
        ejecutaAjax('../../Contabilidad/Formas/Polizas.aspx/TraerEmpresas', 'POST', null, llenarComboEmpresas, null, false);

        //#######################################################
        //ponemos todos los fieldsets del mismo tamaño en anchura
        //#######################################################
        var size = $("#fsRelacionSaldos").width();
        $("#fsRelacionGastos").css("min-width", size);
        $("#fsEstadoResultados").css("min-width", size);
        //#######################################################


    });
}(jQuery, window.balor, window.amplify));