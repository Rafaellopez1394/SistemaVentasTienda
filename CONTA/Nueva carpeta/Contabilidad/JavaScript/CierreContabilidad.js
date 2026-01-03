/// <reference path="../../Base/js/vendor/jquery-1.11.0-vsdoc.js" />
/// <reference path="../../Base/js/plugins.js" />
/// <reference path="../../Base/Scripts/librerias/extensions/jquery.HelpField-1.1.js" />
(function ($, balor, amplify) {
    var msgbx = $('#divmsg');
    var body = $('body');
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

    var ShowLightBoxPopUp = function (Mostrar, msj, item) {
        if (Mostrar == true) {
            var html = '<div class="LightboxPopUp"></div>';
            html += '<div class="panel_pop_PopUp">';
            html += '<span>' + msj + '</span>';
            html += '<br />';
            html += '<img src="../../Base/img/loading.gif" />';
            html += '</div>';
            $(item).append(html);
        } else {
            $(".LightboxPopUp, .panel_pop_PopUp").remove();
        }
    };

    var muestraPopup = function (popid) {
        lightboxOn();
        var popup = $('#' + popid),
        width = popup.actual('width'),
        left = (window.innerWidth / 2) - (width / 2),
        top = window.innerHeight * 0.05;
        popup.css({ 'position': 'absolute', 'top': top, 'left': left });
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

    var getUsuario = function () {
        return amplify.store.sessionStorage("Usuario");
    };

    var getVendedor = function () {
        return amplify.store.sessionStorage("VendedorID");
    };

    var getEmpresaid = function () {
        return amplify.store.sessionStorage("EmpresaID");
    };


    $('body').on('change', '#ddl-empresa', null, function () {
        TraerFechaEmpresa($("#ddl-empresa").val());
    });

    var TraerFechaEmpresa = function (empid) {
        ShowLightBox(true, "Espere porfavor");
        ejecutaAjax('CierreContabilidad.aspx/TraerFechaCierre', 'POST', { empresaid: empid }, function (d) {
            ShowLightBox(false, "");
            if (d.d.EsValido) {
                if (d.d.Datos != null)
                    $("#txtfecha").val(formatoFechaJSON(d.d.Datos.Fechacierre));
                else
                    $("#txtfecha").val("");
            } else {
                showMsg('Alerta', d.d.Mensaje, 'warning');
            }
        }, function (d) {
            ShowLightBox(false, "");
            showMsg('Error', d.responseText, 'error');
        }, true);
    };

    var formatoFechaJSON = function (f) {
        var fi = f.replace(/\/Date\((-?\d+)\)\//, '$1');
        var fecha = new Date(parseInt(fi));

        var dd = ('0' + fecha.getDate().toString()).slice(-2)
        var mm = ('0' + (fecha.getMonth() + 1).toString()).slice(-2)
        var y = fecha.getFullYear().toString();
        return dd + '/' + mm + '/' + y;
    };

    $('body').on('click', '#BotonCerrar', null, function () {
        ShowLightBox(true, "Espere porfavor");
        var parametros = {};
        parametros.empresaid = $("#ddl-empresa").val();
        parametros.fecha = $("#txtfecha").val();
        parametros.usuario = getUsuario();

        ejecutaAjax('CierreContabilidad.aspx/GuardarFechaCierre', 'POST', parametros, function (d) {
            ShowLightBox(false, "");
            if (d.d.EsValido) {
                showMsg('Alerta', 'Fecha guardada correctamente', 'success');
            } else {
                showMsg('Alerta', d.d.Mensaje, 'warning');
            }
        }, function (d) {
            ShowLightBox(false, "");
            showMsg('Error', d.responseText, 'error');
        }, true);
    });

    $('input').not('input[type=button]').on('keypress', null, function (e) {
        focusNextControl(this, e);
    });



    var ActualizaMenuBar = function () {
        if (screen.width < 600) {
            $("#MobileMenu").css("display", "line");
            var menu = $("#Menu").html();
            $("#divcontentMenu").remove();
            $("#MobileMenuDet").html(menu);
            $("#nav").css("top", "0");
        }
    }

    $(document).ready(function () {
        if (amplify.store.sessionStorage("UsuarioID") == null || amplify.store.sessionStorage("UsuarioID") == "") {
            window.location.replace("../../login.aspx");
        }
        $("#Menu").MostrarMenuBar(JSON.parse(amplify.store.sessionStorage("Funcionalidades")), amplify.store.sessionStorage("Nombre"), amplify.store.sessionStorage("NombreEmpresa"), amplify.store.sessionStorage("UsuarioID"));
        $('.popup').setDraggable('.popup-body, .popup-footer, .close');
        ActualizaMenuBar();
        ejecutaAjax('BonoProductividad.aspx/TraerEmpresas', 'POST', null, llenarComboEmpresas, null, false);
        TraerFechaEmpresa($("#ddl-empresa").val());
    });
}(jQuery, window.balor, window.amplify));