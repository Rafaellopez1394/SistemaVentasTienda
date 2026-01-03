/// <reference path="../../Base/js/vendor/jquery-1.11.0-vsdoc.js" />
/// <reference path="../../Base/js/plugins.js" />
/// <reference path="../../Base/Scripts/librerias/extensions/jquery.HelpField-1.1.js" />
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

    var getUsuario = function () {
        return amplify.store.sessionStorage("Usuario");
    };

    var getVendedor = function () {
        return amplify.store.sessionStorage("VendedorID");
    };

    var ayudaVendedorFindByCode = function () {
        var parametros = { value: JSON.stringify($('#ayudaVendedor').getValuesByCode()) };
        ejecutaAjax('BonoProductividad.aspx/AyudaVendedorFindByCode', 'POST', parametros, function (d) {
            $('#ayudaVendedor').showFindByCode(d.d.Datos);
        }, function (d) {
            showMsg('Alerta', d.responseText, 'error');
        }, true);
    };

    var ayudaVendedorFindByPopUp = function () {
        var parametros = { value: JSON.stringify($('#ayudaVendedor').getValuesByPopUp()) };
        ejecutaAjax('BonoProductividad.aspx/AyudaVendedorFindByPopUp', 'POST', parametros, function (d) {
            if (d.d.EsValido) {
                $('#ayudaVendedor').showFindByPopUp(d.d.Datos);
            } else {
                showMsg('Alerta', d.d.Mensaje, 'warning');
            }
        }, function (d) {
            showMsg('Alerta', d.responseText, 'error');
        }, true);
    };
    var iniciaAyudas = function () {
        $('#ayudaVendedor').helpField({
            title: 'Vendedor',
            codeNumeric: true,
            maxlengthCode: 6,
            enableCode: true,
            //enableDescription: false,
            codePaste: false,
            nextControlID: 'BotonImprimir',
            //widthDescription: '0',
            requeridField: false,
            cultureResourcesPopUp: {
                popUpHeader: 'Búsqueda'
            },
            findByPopUp: ayudaVendedorFindByPopUp,
            findByCode: ayudaVendedorFindByCode
        });
    };

    var ValidaImprimir = function () {
        if ($("#fechaInicio").val().trim() == "") {
            showMsg("Alerta", "La fecha Inicial", "warning");
            $('#fechaInicio').focus();
            return false;
        }
        if ($("#fechaFinal").val().trim() == "") {
            showMsg("Alerta", "La fecha Final", "warning");
            $('#fechaFinal').focus();
            return false;
        }
        if ($('#ayudaVendedor').getValuesByCode().ID == "") {
            showMsg("Alerta", "Ingrese el vendedor a generar las comisiones", "warning");
            $('#ayudaVendedor_Code').focus();
            return false;
        }
        return true;
    };


    $('body').on('click', '#BotonImprimir', null, function () {
        /*
        var parametros = {};
        ejecutaAjax('BonoProductividad.aspx/reporte', 'POST', parametros, function (d) {
        }, function (d) {
            showMsg('Alerta', d.responseText, 'error');
        }, true);
        */
        if (ValidaImprimir()) {
        var parametros = "&fechaInicial=" + $("#fechaInicio").val().trim();
        parametros += "&fechaFinal=" + $("#fechaFinal").val().trim();
        parametros += "&empresaid=" + $('#ddl-empresa option:selected').val();
        parametros += "&vendedorid=" + $('#ayudaVendedor').getValuesByCode().ID;
        parametros += "&Formato=" + $("input[type='radio'][name='timprimir']:checked").val(); //balor.radioGetCheckedValue('');
        window.open("../../Base/Formas/ReporteProduccion.aspx?Formato=0&Nombre=Comisiones" + parametros, 'w_PopBono' + $('#ddl-empresa option:selected').val() + $('#ayudaVendedor').getValuesByCode().ID, 'toolbar=no,location=no,directories=no,status=no,menubar=no,scrollbars=yes,resizable=yes,width=760,height=560');
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
    };
    $(document).ready(function () {
        if (amplify.store.sessionStorage("UsuarioID") == null || amplify.store.sessionStorage("UsuarioID") == "") {
            window.location.replace("../../login.aspx");
        }
        $("#Menu").MostrarMenuBar(JSON.parse(amplify.store.sessionStorage("Funcionalidades")), amplify.store.sessionStorage("Nombre"), amplify.store.sessionStorage("NombreEmpresa"), amplify.store.sessionStorage("UsuarioID"));
        $('.popup').setDraggable('.popup-body, .popup-footer, .close');
        ActualizaMenuBar();
        AplicarEstilo();
        ejecutaAjax('BonoProductividad.aspx/TraerEmpresas', 'POST', null, llenarComboEmpresas, null, false);
        iniciaAyudas();
    });
} (jQuery, window.balor, window.amplify));