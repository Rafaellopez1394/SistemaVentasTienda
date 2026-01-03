(function ($, balor, amplify, moment) {
    'use strict';
    var msgbx = $('#divmsg');

    var showMsg = function (titulo, mensaje, tipoMensaje) {
        msgbx.mostrarMensaje(titulo, mensaje, tipoMensaje);
    };

    var hideMsg = function () {
        msgbx.animate({ top: -msgbx.outerHeight(), opacity: 0 }, 500);
    };

    var lightboxOn = function () {
        var lightBox = '<div class="lightbox"></div>';
        $(lightBox).appendTo('body');
    };

    var lightboxOff = function () {
        $('.lightbox').remove();
    };

    var focusNextControl = function (control, e) {
        var code = (e.keyCode ? e.keyCode : e.which);
        if (code === 13) {
            var inputs = $(control).closest('form')
                    .find('input[type=text]:enabled, input[type=time]:enabled, input[type=password]:enabled, input[type=button]:enabled, input[type=checkbox]:enabled, input[type=radio]:enabled, input[type=date]:enabled, input[type=tel]:enabled, input[type=email]:enabled, input[type=number]:enabled, select:enabled, textarea:enabled')
                    .not('input[readonly=readonly], fieldset:hidden *, *:hidden, .nofocus');
            inputs.eq(inputs.index(control) + 1).focus();
            e.preventDefault();
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

    var getEmpresaid = function () {
        return amplify.store.sessionStorage("EmpresaID");
    };

    var body = $('body');
    body.on('keypress', '.entero', function (e) {
        return balor.onlyInteger(e);
    });

    body.on('keypress', 'input:not(input[type=button])', function (e) {
        focusNextControl(this, e);
    });

    $('select').on('keypress', null, function (e) {
        focusNextControl(this, e);
    });

    var validaSesion = function () {
        if (!amplify.store.sessionStorage("UsuarioID")) {
            window.location.replace("../../login.aspx");
        }
    };





    var ValidaImprimir = function () {
        if ($("#fltr-fecha").val() == "") {
            showMsg('Alerta', "Ingrese la fecha inicial del reporte", 'warning');
            $("#fltr-fecha").focus();
            return false;
        }
        if ($("#fltr-fecha2").val() == "") {
            showMsg('Alerta', "Ingrese la fecha final del reporte", 'warning');
            $("#fltr-fecha2").focus();
            return false;
        }

        return true;
    };

    var imprimir = function () {
        var parametros = "&empresaid=" + $("#fltr-empresa").val();
        parametros += "&fecha=" + $("#fltr-fecha").val();
        parametros += "&fecha2=" + $("#fltr-fecha2").val();
        parametros += "&Formato=" + $("input[type='radio'][name='timprimir']:checked").val();
        window.open("../../Base/Formas/ReporteProduccion.aspx?Formato=0&Nombre=ComplementoRecepcionPago" + parametros, 'w_PopComplementoRecepcionPago', 'toolbar=no,location=no,directories=no,status=no,menubar=no,scrollbars=yes,resizable=yes,width=760,height=560');
    };

    body.on('click', '#btnimprimir', null, function () {
        if (ValidaImprimir()) {
            imprimir();
        }
    });

    body.on('click', '#btnLimpiar', null, function () {
        window.location.replace('ReporteComplementoRecepcionPago.aspx');
    });

    var llenarComboEmpresas = function (d) {
        if (d.d.EsValido) {
            $('#fltr-empresa').llenaCombo(d.d.Datos);
        }
    };



    $(document).ready(function () {
        validaSesion();
        $("#Menu").MostrarMenuBar(JSON.parse(amplify.store.sessionStorage("Funcionalidades")), amplify.store.sessionStorage("Nombre"), amplify.store.sessionStorage("NombreEmpresa"), amplify.store.sessionStorage("UsuarioID"));
        $("#fltr-fecha").focus();

        ejecutaAjax('Polizas.aspx/TraerEmpresas', 'POST', null, llenarComboEmpresas, null, false);
        $('#fltr-empresa').focus();
    });
}(jQuery, window.balor, window.amplify, window.moment));