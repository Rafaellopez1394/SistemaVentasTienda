(function ($, balor, amplify) {
    var msgbx = $('#divmsg');
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
            $('#fltr-empresa').llenaCombo(d.d.Datos);
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

    var getUsuario = function () {
        return amplify.store.sessionStorage("Usuario");
    };

    var getVendedor = function () {
        return amplify.store.sessionStorage("VendedorID");
    };

   

    var ValidaImprimir = function () {
        if ($('input[name=reporte]:radio:checked').val() == "1") {
            if ($("#txtEjercicio").val().trim() == "") {
                showMsg("Alerta", "Debe ingresar el año del ejercicio", "warning");
                $('#txtEjercicio').focus();
                return false;
            }
        } else {
            if ($("#fltr-fecha").val().trim() == "") {
                showMsg("Alerta", "La fecha de corte", "warning");
                $('#fltr-fecha').focus();
                return false;
            }
        }
        return true;
    };
    $('body').on('click', '#btnimprimir', null, function () {
        if (ValidaImprimir()) {
            var formato = $('input[name=formato]:radio:checked').val();
            if ($('input[name=reporte]:radio:checked').val() == "1") {
                var parametros = "&empresaid=" + $('#fltr-empresa option:selected').val();
                parametros += "&anio=" + $('#txtEjercicio').val();
                parametros += "&formatoespecial=" + formato;
                window.open("../../Base/Formas/ReporteProduccion.aspx?Formato=" + $('input[name=timprimir]:radio:checked').val() + "&Nombre=PresupuestoContable" + parametros, 'w_PopPresupuestoContable' + $('#txtAnio').val() + $('#fltr-empresa option:selected').val(), 'toolbar=no,location=no,directories=no,status=no,menubar=no,scrollbars=yes,resizable=yes,width=760,height=560');
            } else {
                var parametros = "&empresaid=" + $('#fltr-empresa option:selected').val();
                parametros += "&Fecha=" + $('#fltr-fecha').val();
                parametros += "&formatoespecial=" + formato;
                window.open("../../Base/Formas/ReporteProduccion.aspx?Formato=" + $('input[name=timprimir]:radio:checked').val() + "&Nombre=PresupuestoVsReal" + parametros, 'w_PopPresupuestoContableVsReal' + $('#fltr-fecha').val() + $('#fltr-empresa option:selected').val(), 'toolbar=no,location=no,directories=no,status=no,menubar=no,scrollbars=yes,resizable=yes,width=760,height=560');
            }
            /*
            var parametros = "&fechaInicial=" + $("#fechaInicio").val().trim();
            parametros += "&fechaFinal=" + $("#fechaFinal").val().trim();
            parametros += "&empresaid=" + $('#ddl-empresa option:selected').val();
            parametros += "&VendInicial=" + $("#fltr-vendedorini").val() || 0;
            parametros += "&VendFinal=" + $("#fltr-vendedorfin").val() || 1000;
            parametros += "&Equipo=" + $('#ayudaEqVenta').getValuesByCode().Codigo || 0;
            parametros += "&Zona=" + $("#fltr-zonaini").val() || 0;
            parametros += "&Factoraje=" + ($("#chkFactoraje").is(':checked') ? 1 : 0);            
            window.open("../../Base/Formas/ReporteProduccion.aspx?Formato=0&Nombre=ResultadoCesion" + parametros, 'w_PopResultadoCesion' + $('#ddl-empresa option:selected').val(), 'toolbar=no,location=no,directories=no,status=no,menubar=no,scrollbars=yes,resizable=yes,width=760,height=560');
            */
        }
    });



    $('input').not('input[type=button]').on('keypress', null, function (e) {
        focusNextControl(this, e);
    });

    $('body').on('click', 'input[name=reporte]:radio', null, function () {
        if (this.value == "1") {
            $("#divfec").hide();
            $("#diveje").show();
        } else {
            $("#divfec").show();
            $("#diveje").hide();
        }
        
    });

    var MostrarPagina = function (url) {
        if (url != "") {
            window.location.replace(url);
        }
    };    
    $(document).ready(function () {
        if (amplify.store.sessionStorage("UsuarioID") == null || amplify.store.sessionStorage("UsuarioID") == "") {
            window.location.replace("../../login.aspx");
        }
        $("#Menu").MostrarMenuBar(JSON.parse(amplify.store.sessionStorage("Funcionalidades")), amplify.store.sessionStorage("Nombre"), amplify.store.sessionStorage("NombreEmpresa"), amplify.store.sessionStorage("UsuarioID"));
        $('.popup').setDraggable('.popup-body, .popup-footer, .close');
        ejecutaAjax('ReportePresupuestoDeGastos.aspx/TraerEmpresas', 'POST', null, llenarComboEmpresas, null, false);
        $("#fltr-fecha").val(fechaActual());
    });
}(jQuery, window.balor, window.amplify));