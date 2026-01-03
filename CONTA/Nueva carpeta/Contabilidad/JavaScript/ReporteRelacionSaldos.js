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

    var TraerUltimaCuentaContable = function () {
        var parametros = {};
        parametros.empresaid = $('#ddl-empresa option:selected').val();
        ejecutaAjax('ReporteRelacionSaldos.aspx/TraerUltimaCuentaContable', 'POST', parametros, function (d) {
            if (d.d.EsValido) {
                $('#AyudaCuentaFinal_Code').val(formatoCuenta(d.d.Datos.Cuenta));
                $('#AyudaCuentaFinal_lBox').val(d.d.Datos.Descripcion);
                $('#AyudaCuentaFinal_ValorID').val(d.d.Datos.Cuentaid);

            } else {
                showMsg('Alerta', d.d.Mensaje, 'error');
            }
        }, function (d) {
            showMsg('Alerta', d.responseText, 'error');
        }, true);
    };

    var TraerPrimeraCuentaContable = function () {
        var parametros = {};
        parametros.empresaid = $('#ddl-empresa option:selected').val();
        ejecutaAjax('ReporteRelacionSaldos.aspx/TraerPrimeraCuentaContable', 'POST', parametros, function (d) {
            if (d.d.EsValido) {
                $('#AyudaCuentaIncial_Code').val(formatoCuenta(d.d.Datos.Cuenta));
                $('#AyudaCuentaIncial_lBox').val(d.d.Datos.Descripcion);
                $('#AyudaCuentaIncial_ValorID').val(d.d.Datos.Cuentaid);

            } else {
                showMsg('Alerta', d.d.Mensaje, 'error');
            }
        }, function (d) {
            showMsg('Alerta', d.responseText, 'error');
        }, true);
    };



    var AyudaCuentaIncial_FindByCode = function () {
        var parametros = $('#AyudaCuentaIncial').getValuesByCode(),
        codigo = llenarSubcuentas(parametros.Codigo).replace(/-/g, '');
        escondeMsg();
        parametros.Codigo = padRight(codigo, 24);
        parametros.ID = $('#ddl-empresa option:selected').val();
        $(parametros).ToRequest({
            url: 'ReporteRelacionSaldos.aspx/AyudaCuenta_FindByCode',
            success: function (d) {
                if (d.d.EsValido) {
                    $('#AyudaCuentaIncial').showFindByCode(d.d.Datos);
                } else {
                    $('#AyudaCuentaIncial_HelpLoad').css({ "display": "none" });
                    showMsg('Alerta', d.d.Mensaje, 'warning');
                }
            },
            failure: function (d) {
                showMsg('Alerta', d.responseText, 'error');
            },
            Titulo: 'Espere Por Favor'
        });
    };

    var AyudaCuentaIncial_FindByPopUp = function () {
        escondeMsg();
        var parametros = $('#AyudaCuentaIncial').getValuesByPopUp();
        parametros.ID = $('#ddl-empresa option:selected').val();
        $(parametros).ToRequest({
            url: 'ReporteRelacionSaldos.aspx/AyudaCuenta_FindByPopUp',
            success: function (d) {
                if (d.d.EsValido) {
                    $('#AyudaCuentaIncial').showFindByPopUp(d.d.Datos);
                } else {
                    $('#AyudaCuentaIncial_HelpLoad').css({ "display": "none" });
                    showMsg('Alerta', d.d.Mensaje, 'warning');
                }
            },
            failure: function (d) {
                showMsg('Alerta', d.responseText, 'error');
            },
            Titulo: 'Espere Por Favor'
        });
    };

    var AyudaCuentaIncial_onElementFound = function () {
        var cadena = $('#AyudaCuentaIncial_Code').val();
        $('#AyudaCuentaIncial_Code').val(formatoCuenta(cadena));
    };


    var AyudaCuentaFinal_FindByCode = function () {
        var parametros = $('#AyudaCuentaFinal').getValuesByCode(),
        codigo = llenarSubcuentas(parametros.Codigo).replace(/-/g, '');
        escondeMsg();
        parametros.Codigo = padRight(codigo, 24);
        parametros.ID = $('#ddl-empresa option:selected').val();
        $(parametros).ToRequest({
            url: 'ReporteRelacionSaldos.aspx/AyudaCuenta_FindByCode',
            success: function (d) {
                if (d.d.EsValido) {
                    $('#AyudaCuentaFinal').showFindByCode(d.d.Datos);
                } else {
                    $('#AyudaCuentaFinal_HelpLoad').css({ "display": "none" });
                    showMsg('Alerta', d.d.Mensaje, 'warning');
                }
            },
            failure: function (d) {
                showMsg('Alerta', d.responseText, 'error');
            },
            Titulo: 'Espere Por Favor'
        });
    };

    var AyudaCuentaFinal_FindByPopUp = function () {
        escondeMsg();
        var parametros = $('#AyudaCuentaFinal').getValuesByPopUp();
        parametros.ID = $('#ddl-empresa option:selected').val();
        $(parametros).ToRequest({
            url: 'ReporteRelacionSaldos.aspx/AyudaCuenta_FindByPopUp',
            success: function (d) {
                if (d.d.EsValido) {
                    $('#AyudaCuentaFinal').showFindByPopUp(d.d.Datos);
                } else {
                    $('#AyudaCuentaFinal_HelpLoad').css({ "display": "none" });
                    showMsg('Alerta', d.d.Mensaje, 'warning');
                }
            },
            failure: function (d) {
                showMsg('Alerta', d.responseText, 'error');
            },
            Titulo: 'Espere Por Favor'
        });
    };

    var AyudaCuentaFinal_onElementFound = function () {
        var cadena = $('#AyudaCuentaFinal_Code').val();
        $('#AyudaCuentaFinal_Code').val(formatoCuenta(cadena));
    };

    var iniciaAyudas = function () {
        $('#AyudaCuentaIncial').helpField({
            title: 'Cuenta Inicial',
            codeNumeric: false,
            maxlengthCode: 30,
            enableCode: true,
            enableDescription: false,
            codePaste: false,
            nextControlID: 'AyudaCuentaFinal',
            widthCode: 170,
            requeridField: false,
            cultureResourcesPopUp: {
                popUpHeader: 'Búsqueda de Cuentas'
            },
            findByPopUp: AyudaCuentaIncial_FindByPopUp,
            findByCode: AyudaCuentaIncial_FindByCode,
            onElementFound: AyudaCuentaIncial_onElementFound
        });

        $('#AyudaCuentaFinal').helpField({
            title: 'Cuenta Final',
            codeNumeric: false,
            maxlengthCode: 30,
            enableCode: true,
            enableDescription: false,
            codePaste: false,
            nextControlID: 'ComboNivel',
            widthCode: 170,
            requeridField: false,
            cultureResourcesPopUp: {
                popUpHeader: 'Búsqueda de Cuentas'
            },
            findByPopUp: AyudaCuentaFinal_FindByPopUp,
            findByCode: AyudaCuentaFinal_FindByCode,
            onElementFound: AyudaCuentaFinal_onElementFound
        });
    };

    $('body').on('keypress', '#AyudaCuentaFinal_Code', null, function (e) {
        var input = $('#AyudaCuentaFinal_Code'),
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


    $('body').on('keypress', '#AyudaCuentaIncial_Code', null, function (e) {
        var input = $('#AyudaCuentaIncial_Code'),
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
        if ($('#AyudaCuentaIncial').getValuesByCode().Codigo.trim() == "") {
            showMsg('Alerta', "Debe ingresar la cuenta inicial", 'warning');
            $("#AyudaCuentaIncial_Code").focus();
            return false;
        }
        if ($('#AyudaCuentaFinal').getValuesByCode().Codigo.trim() == "") {
            showMsg('Alerta', "Debe ingresar la cuenta final", 'warning');
            $("#AyudaCuentaFinal_Code").focus();
            return false;
        }
        return true;
    }


    $('body').on('click', '#BotonImprimir', null, function () {
        if (ValidaImprimir()) {
            var parametros = "&fechaInicial=" + $("#fechaInicio").val().trim();
            parametros += "&fechaFinal=" + $("#fechaFinal").val().trim();
            parametros += "&empresaid=" + $('#ddl-empresa option:selected').val();
            parametros += "&cuentaInicial=" + $('#AyudaCuentaIncial').getValuesByCode().Codigo.trim();
            parametros += "&cuentaFinal=" + $('#AyudaCuentaFinal').getValuesByCode().Codigo.trim();
            parametros += "&nivel=" + $('#ComboNivel option:selected').val();
            parametros += "&Ingles=" + "0";
            parametros += "&Formato=" + $("input[type='radio'][name='timprimir']:checked").val();
            parametros += "&ExcluirDemandados=" + $('#chkExcluirDemandado').is(':checked');
            window.open("../../Base/Formas/ReporteProduccion.aspx?Formato=0&Nombre=RelacionSaldos" + parametros, 'w_PopRelacionSaldos' + $("#fechaInicio").val().trim().replace("/", "") + $("#fechaFinal").val().trim().replace("/", "") + $('#ddl-empresa option:selected').val(), 'toolbar=no,location=no,directories=no,status=no,menubar=no,scrollbars=yes,resizable=yes,width=760,height=560');
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



    $('body').on('click', '#btnEnvio', null, function () {
        if (ValidaImprimir()) {
            var parametros = {};
            parametros.EmpresaID = $('#ddl-empresa option:selected').val();
            parametros.FechaInicial = $('#fechaInicio').val();
            parametros.FechaFinal = $("#fechaFinal").val().trim();
            parametros.CuentaInicial = $('#AyudaCuentaIncial').getValuesByCode().Codigo.trim();
            parametros.CuentaFinal = $('#AyudaCuentaFinal').getValuesByCode().Codigo.trim();
            parametros.Nivel = $('#ComboNivel option:selected').val();
            parametros.Ingles = "0";
            parametros.TipoEnvio = $("input[type='radio'][name='tenvio']:checked").val(); //balor.radioGetCheckedValue('');
            parametros.ExcluirDemandados = $('#chkExcluirDemandado').is(':checked');

            ShowLightBox(true, "Espere porfavor");
            ejecutaAjax('ReporteRelacionSaldos.aspx/EnviarSAT', 'POST', parametros, function (d) {
                ShowLightBox(false, "");
                if (d.d.EsValido) {
                    if (d.d.Datos.Genero) {
                        //Codigo de descarga
                        $("#myDownloaderFrame").attr("src", "../../Base/Formas/DownloadFile.aspx?ruta=" + d.d.Datos.Ruta + "&archivo=" + d.d.Datos.FileName);
                    }
                    else {
                        if (d.d.Datos.Imprimir) {
                            //Codigo de impresion
                            var parametros = "&empresaid=" + $('#ddl-empresa option:selected').val();
                            window.open("../../Base/Formas/ReporteProduccion.aspx?Formato=0&Nombre=CtaSATSinClasificar" + parametros, 'w_PopCtaSATSinClasificar', 'toolbar=no,location=no,directories=no,status=no,menubar=no,scrollbars=yes,resizable=yes,width=760,height=560');
                        } else {
                            showMsg('Alerta', d.d.Datos.Mjs, 'warning'); 
                        }
                    }
                } else {
                    ShowLightBox(false, "");
                    showMsg('Alerta', d.d.Mensaje, 'error');
                }
            }, function (d) {
                showMsg('Alerta', d.responseText, 'error');
            }, true);
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
        iniciaAyudas();
        ejecutaAjax('Polizas.aspx/TraerEmpresas', 'POST', null, llenarComboEmpresas, null, false);
        TraerPrimeraCuentaContable();
        TraerUltimaCuentaContable();
    });
} (jQuery, window.balor, window.amplify));