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
            $('#ddl-empresa').llenaCombo(d.d.Datos, null, amplify.store.sessionStorage("EmpresaID"));
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

    var AyudaCuentaResultado_FindByCode = function () {
        var parametros = $('#AyudaCuentaResultado').getValuesByCode(),
        codigo = llenarSubcuentas(parametros.Codigo).replace(/-/g, '');
        escondeMsg();
        parametros.Codigo = padRight(codigo, 24);
        parametros.ID = $('#ddl-empresa').val();
        $(parametros).ToRequest({
            url: 'ReporteRelacionSaldos.aspx/AyudaCuenta_FindByCode',
            success: function (d) {
                if (d.d.EsValido) {
                    $('#AyudaCuentaResultado').showFindByCode(d.d.Datos);
                } else {
                    $('#AyudaCuentaResultado_HelpLoad').css({ "display": "none" });
                    showMsg('Alerta', d.d.Mensaje, 'warning');
                }
            },
            failure: function (d) {
                showMsg('Alerta', d.responseText, 'error');
            },
            Titulo: 'Espere Por Favor'
        });
    };

    var AyudaCuentaResultado_FindByPopUp = function () {
        escondeMsg();
        var parametros = $('#AyudaCuentaResultado').getValuesByPopUp();
        parametros.ID = $('#ddl-empresa').val();
        $(parametros).ToRequest({
            url: 'ReporteRelacionSaldos.aspx/AyudaCuenta_FindByPopUp',
            success: function (d) {
                if (d.d.EsValido) {
                    $('#AyudaCuentaResultado').showFindByPopUp(d.d.Datos);
                } else {
                    $('#AyudaCuentaResultado_HelpLoad').css({ "display": "none" });
                    showMsg('Alerta', d.d.Mensaje, 'warning');
                }
            },
            failure: function (d) {
                showMsg('Alerta', d.responseText, 'error');
            },
            Titulo: 'Espere Por Favor'
        });
    };

    var AyudaCuentaResultado_onElementFound = function () {
        var cadena = $('#AyudaCuentaResultado_Code').val();
        ejecutaAjax('CancelarCuentasResultado.aspx/CuentaAfectable', 'POST', { cuenta: cadena, empresaid: $('#ddl-empresa').val() }, function (d) {
            if (d.d.EsValido) {
                if (d.d.Datos) {
                    $('#AyudaCuentaResultado_Code').val(formatoCuenta(cadena));
                }
                else {
                    $('#AyudaCuentaResultado').clearHelpField();
                    showMsg('Alerta', 'La cuenta indicada no es afectable', 'warning');
                }
            } else {
                showMsg('Alerta', d.d.Mensaje, 'warning');
            }
        }, function (d) {
            showMsg('Error', d.responseText, 'error');
        }, true);
    };

    var iniciaAyudas = function () {
        $('#AyudaCuentaResultado').helpField({
            title: 'Cuenta de Resultados:',
            codeNumeric: false,
            maxlengthCode: 30,
            enableCode: true,
            enableDescription: false,
            codePaste: false,
            nextControlID: 'txtComentario',
            widthCode: 170,
            requeridField: false,
            cultureResourcesPopUp: {
                popUpHeader: 'Búsqueda de Cuentas'
            },
            findByPopUp: AyudaCuentaResultado_FindByPopUp,
            findByCode: AyudaCuentaResultado_FindByCode,
            onElementFound: AyudaCuentaResultado_onElementFound
        });
    };

    var llenaComboTipPol = function () {
        $("#ddlTipPol option").remove();
        ejecutaAjax('../Formas/CapturaPolizas.aspx/TraerTipPol', 'POST', {}, function (d) {
            if (d.d.EsValido) {
                $("#ddlTipPol").append("<option value='0'>Seleccionar...</option>");
                for (var i = 0; i < d.d.Datos.length; i++) {
                    $("#ddlTipPol").append("<option value=" + d.d.Datos[i].id + ">" + d.d.Datos[i].nombre + "</option>");
                }
            } else {
                $('#ddlTipPol').html('');
                showMsg('Alerta', d.d.Mensaje, 'warning');
            }
            ShowLightBox(false, "");
        }, function (d) {
            ShowLightBox(false, "");
            showMsg('Alerta', d.responseText, 'error');
        }, false);
    };


    $('body').on('change', '#ddl-empresa', null, function () {
        $("#ddlTipPol").val(0);
        $("#numPol").val("");
    });

    $('body').on('blur', '#anioPoliza', null, function () {
        llenaComboTipPol();
        $("#numPol").val("");
    });

    $('body').on('change', '#ddlTipPol', null, function () {
        TraerFolioMaximoPorTipoPoliza('31/12/' + $("#anioPoliza").val());
    });

    $('body').on('keypress', '#AyudaCuentaResultado_Code', null, function (e) {
        var input = $('#AyudaCuentaResultado_Code'),
      strCuenta = input.val(),
      key = getChar(e);
        if (strCuenta.length < 29) {
            if (key) {
                if (/[0-9]|\-/.test(key)) {
                    if (key === '-') {
                        var cuentaNueva = llenarSubcuentas(strCuenta);
                        input.val(cuentaNueva);
                        if (cuentaNueva.length === 29) {
                            return false;
                        }
                    } else {
                        strCuenta += key;
                        var cuenta2 = formatoCuenta(strCuenta);
                        input.val(cuenta2);
                        return false;
                    }
                } else {
                    return false;
                }
            }
        } else {
            return false;
        }
    });

    $('body').on('click', '#BotonGenerar', null, function () {

        if (!validaGuardar())
            return;

        ShowLightBox(true, "Espere porfavor");
        var parametros = {};
        parametros.empresaid = $("#ddl-empresa").val();
        parametros.anio = $("#anioPoliza").val();
        parametros.NumPol = $("#numPol").val();
        parametros.tippol = $("#ddlTipPol").val();
        parametros.cuentaResultado = $('#AyudaCuentaResultado').getValuesByCode().Codigo;
        parametros.comentario = $("#txtComentario").val();
        parametros.usuario = getUsuario();

        ejecutaAjax('CancelarCuentasResultado.aspx/ProcesarCancelacionCuenta', 'POST', parametros, function (d) {
            ShowLightBox(false, "");
            if (d.d.EsValido) {
                $("#BotonGenerar").attr('disabled', 'disabled');
                showMsg('Alerta', 'Poliza generada correctamente', 'success');
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

    $('body').on('click', '#btnLimpiar', null, function () {
        window.location.replace("CancelarCuentasResultado.aspx");
    });

    var validaGuardar = function () {
        var respuesta = true;
        if ($("#anioPoliza").val() == "" || parseInt($("#anioPoliza").val()) == 0) {
            showMsg('Alerta', 'El Año no es valido', 'warning');
            respuesta = false;
        }
        if ($("#ddlTipPol").val() == 0) {
            showMsg('Alerta', 'Favor de indicar el tipo de poliza', 'warning');
            respuesta = false;
        }
        if ($('#AyudaCuentaResultado').getValuesByCode().Codigo == "") {
            showMsg('Alerta', 'Favor de indicar la cuenta', 'warning');
            respuesta = false;
        }
        if ($('#txtComentario').val() == "") {
            showMsg('Alerta', 'Favor de agregar un comentario', 'warning');
            respuesta = false;
        }

        return respuesta;
    }

    var TraerFolioMaximoPorTipoPoliza = function (fechaPol) {
        var parametros = { tippol: $('#ddlTipPol').val(), EmpresaId: $('#ddl-empresa').val(), fechapol: fechaPol };
        ejecutaAjax('CancelarCuentasResultado.aspx/TraerFolioMaximoPorTipoPoliza', 'POST', parametros, function (d) {
            if (d.d.EsValido) {
                $("#numPol").val(d.d.Datos.Folio);
            } else {
                showMsg('Alerta', d.d.Mensaje, 'warning');
            }
        }, function (d) {
            showMsg('Error', d.responseText, 'error');
        }, true);
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
        iniciaAyudas();
        //llenaComboTipPol();
        AplicarEstilo();
        ejecutaAjax('BonoProductividad.aspx/TraerEmpresas', 'POST', null, llenarComboEmpresas, null, false);
    });
}(jQuery, window.balor, window.amplify));