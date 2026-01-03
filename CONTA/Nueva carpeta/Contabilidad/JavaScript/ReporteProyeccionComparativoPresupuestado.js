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
            //$('#fltr-empresa').remove();
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


    var formatMoney = function (numero, c, d, t) {
        var n = numero,
    s = n < 0 ? '-' : '',
    i = parseInt(n = Math.abs(+n || 0).toFixed(c)) + '',
    j = (j = i.length) > 3 ? j % 3 : 0;

        c = isNaN(c = Math.abs(c)) ? 2 : c;
        d = d ? d : '.';
        t = t ? t : ',';
        s = n == 0 ? '' : s;
        return s + (j ? i.substr(0, j) + t : "") + i.substr(j).replace(/(\d{3})(?=\d)/g, "$1" + t) + (c ? d + Math.abs(n - i).toFixed(c).slice(2) : '');
    };

    // event.type must be keypress
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
    var TraerUltimaCuentaContable = function (cuenta) {
        var parametros = {};
        parametros.cuenta = cuenta;
        parametros.empresaid = $('#fltr-empresa option:selected').val();
        ejecutaAjax('ReporteProyeccionComparativoPresupuestado.aspx/TraerUltimaCuentaContable', 'POST', parametros, function (d) {
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






    var AyudaCuentaIncial_FindByCode = function () {
        var parametros = $('#AyudaCuentaIncial').getValuesByCode(),
        codigo = llenarSubcuentas(parametros.Codigo).replace(/-/g, '');
        escondeMsg();
        parametros.Codigo = padRight(codigo, 24);
        parametros.ID = $('#fltr-empresa option:selected').val();
        $(parametros).ToRequest({
            url: 'ReporteProyeccionComparativoPresupuestado.aspx/AyudaCuenta_FindByCode',
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
        parametros.ID = $('#fltr-empresa option:selected').val();
        $(parametros).ToRequest({
            url: 'ReporteProyeccionComparativoPresupuestado.aspx/AyudaCuenta_FindByPopUp',
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
        TraerUltimaCuentaContable(cadena);

    };


    var AyudaCuentaFinal_FindByCode = function () {
        var parametros = $('#AyudaCuentaFinal').getValuesByCode(),
        codigo = llenarSubcuentas(parametros.Codigo).replace(/-/g, '');
        escondeMsg();
        parametros.Codigo = padRight(codigo, 24);
        parametros.ID = $('#fltr-empresa option:selected').val();
        $(parametros).ToRequest({
            url: 'ReporteProyeccionComparativoPresupuestado.aspx/AyudaCuenta_FindByCode',
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
        parametros.ID = $('#fltr-empresa option:selected').val();
        $(parametros).ToRequest({
            url: 'ReporteProyeccionComparativoPresupuestado.aspx/AyudaCuenta_FindByPopUp',
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


    $('body').on('click', '#btnimprimir', null, function () {
        if (ValidaImprimir()) {
            var cuentaini = $("#AyudaCuentaIncial_Code").val();
            var cuentafin = $("#AyudaCuentaFinal_Code").val();
            var tiporeporte = $('input[name=formato]:radio:checked').val();
            var formato = $('input[name=timprimir]:radio:checked').val();

            var parametros = "&empresaid=" + $('#fltr-empresa option:selected').val();            
            parametros += "&cuentaini=" + cuentaini;
            parametros += "&cuentafin=" + cuentafin;
            parametros += "&ejercicio=" + $('#txtEjercicio').val();
            parametros += "&mes=" + $("#ddlMeses option:selected").val();
            parametros += "&tiporeporte=" + $('input[name=formato]:radio:checked').val();
            
            window.open("../../Base/Formas/ReporteProduccion.aspx?Formato=" + formato + "&Nombre=ProyeccionComparativoPresupuestado" + parametros, 'w_PopPresupuestoContable' + $('#txtEjercicio').val() + $('#fltr-empresa option:selected').val(), 'toolbar=no,location=no,directories=no,status=no,menubar=no,scrollbars=yes,resizable=yes,width=760,height=560');
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

    $('body').on('click', '#btnLimpiar', null, function () {
        limpiar();
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

    var limpiar = function () {
        $("#AyudaCuentaIncial_Code").val("");
        $("#AyudaCuentaIncial_lBox").val("");
        $("#AyudaCuentaFinal_Code").val("");
        $("#AyudaCuentaFinal_lBox").val("");
        $('input[name=formato]:radio:eq(0)').prop('checked', true);
        $('input[name=timprimir]:radio:eq(0)').prop('checked', true);
        $('#fltr-empresa').prop('selectedIndex', 0);
        $('#txtEjercicio').val("");
        $("#ddlMeses").prop('selectedIndex', 0);
        $('input[name=formato]:radio:eq(0)').prop('checked', true);       
    }

    $(document).ready(function () {
        if (amplify.store.sessionStorage("UsuarioID") == null || amplify.store.sessionStorage("UsuarioID") == "") {
            window.location.replace("../../login.aspx");
        }
        $("#Menu").MostrarMenuBar(JSON.parse(amplify.store.sessionStorage("Funcionalidades")), amplify.store.sessionStorage("Nombre"), amplify.store.sessionStorage("NombreEmpresa"), amplify.store.sessionStorage("UsuarioID"));
        $('.popup').setDraggable('.popup-body, .popup-footer, .close');
        ejecutaAjax('ReportePresupuestoDeGastos.aspx/TraerEmpresas', 'POST', null, llenarComboEmpresas, null, false);
        $("#fltr-fecha").val(fechaActual());
        iniciaAyudas();
    });
}(jQuery, window.balor, window.amplify));