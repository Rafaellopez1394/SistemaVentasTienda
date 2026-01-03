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


    var TraerFolioMaximoPorTipoPoliza = function () {
        var check = $('input[name=rdfechacierre]:radio:checked');
        var parametros = { tippol: $('#ddlTipPol').val(), EmpresaId: amplify.store.sessionStorage("EmpresaID"), fechapol: check.val() };
        ejecutaAjax('CapturaPolizas.aspx/TraerFolioMaximoPorTipoPoliza', 'POST', parametros, function (d) {
            if (d.d.EsValido) {
                $("#txtNumPoliza").val(d.d.Datos.Folio);
            } else {
                showMsg('Alerta', d.d.Mensaje, 'warning');
            }
        }, function (d) {
            showMsg('Error', d.responseText, 'error');
        }, false);
    }



    var llenaComboTipPol = function () {
        var parametros = {};
        ejecutaAjax('CapturaPolizas.aspx/TraerTipPol', 'POST', parametros, function (d) {
            if (d.d.EsValido) {
                var tippol = d.d.Datos.reverse(), i = tippol.length, opt = '';
                for (i; i--; ) {
                    opt += '<option value="' + tippol[i].id + '">' + tippol[i].nombre + '</option>';
                }
                $('#ddlTipPol').html(opt);
                $("#ddlTipPol").val("DR");
                TraerFolioMaximoPorTipoPoliza();
            } else {
                $('#ddlTipPol').html('');
                showMsg('Alerta', d.d.Mensaje, 'warning');
            }
        }, function (d) {
            showMsg('Error', d.responseText, 'error');
        }, false);
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

    var ObtenerFechaCierre = function () {
        var parametros = {};
        parametros.EmpresaID = amplify.store.sessionStorage("EmpresaID");
        ejecutaAjax('InteresesDevengados.aspx/ObtenerFechaCierre', 'POST', parametros, function (d) {
            if (d.d.EsValido) {
                if (d.d.Datos.length>0) {
                    ProcesarDatos(d.d.Datos);
                    llenaComboTipPol();
                    $("#fechaInicio").val($('input[name=rdfechacierre]:radio:checked').val());
                    $("#btnGenerar").removeAttr("disabled");
                } else {
                    showMsg('Alerta', "No existen cierres por procesar", 'warning');
                }
            } else {
                showMsg('Alerta', d.d.Mensaje, 'warning');
            }
        }, function (d) {
            showMsg('Error', d.responseText, 'error');
        }, false);
    };

    var ProcesarDatos = function (Datos) {
        var resHtml = '';
        var reg = 1;
        resHtml += '<tr>';
        var first = true;
        for (var i = 0; i < Datos.length; i++) {
            if (first) {
                resHtml += '<td>' + '<label><input type="radio" name="rdfechacierre" value="' + Datos[i].Fecha + '" checked disabled/>' + Datos[i].Fecha + '</label>' + '</td>';
                first = false;
            } else {
                resHtml += '<td>' + '<label><input type="radio" name="rdfechacierre" value="' + Datos[i].Fecha + '" disabled/>' + Datos[i].Fecha + '</label>' + '</td>';
            }
            
            reg++;
            if (reg > 3) {
                resHtml += '</tr>';
                resHtml += '<tr>';
                reg = 1;
            }            
        }

        if (reg < 4) {
            for (var i = reg; i < 4; i++)
                resHtml += '<td></td>';
            resHtml += '</tr>';
        }
            

        $("#tblCierres tbody").html(resHtml);
    };

    $('body').on('change', '#ddlTipPol', null, function () {
        TraerFolioMaximoPorTipoPoliza();
    });

    var GenerarPolizaMensual = function () {
        ShowLightBox(true, "Generando, espere porfavor");
        var parametros = { tippol: $('#ddlTipPol').val(), EmpresaId: amplify.store.sessionStorage("EmpresaID"), fechapol: $('#fechaInicio').val(), NumPol: $('#txtNumPoliza').val(), Usuario: getUsuario() };
        ejecutaAjax('InteresesDevengados.aspx/GenerarPolizaMensual', 'POST', parametros, function (d) {
            ShowLightBox(false, "");
            if (d.d.EsValido) {
                alert("Cierre diario terminado correctamente");
                window.location.replace("InteresesDevengados.aspx");
            } else {
                showMsg('Alerta', d.d.Mensaje, 'warning');
            }
        }, function (d) {
            ShowLightBox(false, "");
            showMsg('Error', d.responseText, 'error');
        }, true);
    };

    $('body').on('click', '#btnGenerar', null, function () {
        if (confirm('¿Desea generar la poliza de interes devengados del mes?')) {
            GenerarPolizaMensual();
        }
    });

    $('body').on('click', '#btnLimpiar', null, function () {
        window.location.replace("InteresesDevengados.aspx");
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
        $("#Menu").MostrarMenuBar(JSON.parse(amplify.store.sessionStorage("Funcionalidades")), amplify.store.sessionStorage("Nombre"), amplify.store.sessionStorage("NombreEmpresa"), amplify.store.sessionStorage("UsuarioID"));
        $('.popup').setDraggable('.popup-body, .popup-footer, .close');
        ActualizaMenuBar();
        AplicarEstilo();        
        ObtenerFechaCierre();
    });
} (jQuery, window.balor, window.amplify));