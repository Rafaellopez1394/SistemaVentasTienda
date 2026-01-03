/// <reference path="../../Base/js/vendor/jquery-1.11.0-vsdoc.js" />
/// <reference path="../../Base/js/plugins.js" />
/// <reference path="../../Base/Scripts/librerias/extensions/jquery.HelpField-1.1.js" />
(function ($, balor, amplify) {
    var msgbx = $('#divmsg');    
    var body = $('body');

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
        left = 350, //(window.innerWidth / 2) - (width / 2),
        top = window.innerHeight * 0.1;
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

    var llenarComboEmpresas = function (d) {
        if (d.d.EsValido) {
            $('#ddl-empresa').llenaCombo(d.d.Datos);
        }
    };


    //****************CODIGO PANTALLA DE POLIZAS

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

    var ayudaCuenta_FindByCode = function () {
        var parametros = $('#AyudaCuenta').getValuesByCode(),
        codigo = llenarSubcuentas(parametros.Codigo).replace(/-/g, '');
        escondeMsg();
        parametros.Codigo = padRight(codigo, 24);
        parametros.ID = $('#ddl-empresa option:selected').val();
        ejecutaAjax('PresupuestoContable.aspx/AyudaCuenta_FindByCode', 'POST', { value: JSON.stringify(parametros) }, function (d) {
            ShowLightBox(false, "");
            if (d.d.EsValido) {
                $('#AyudaCuenta').showFindByCode(d.d.Datos);
            } else {
                $('#AyudaCuenta_HelpLoad').css({ "display": "none" });
                showMsg('Alerta', d.d.Mensaje, 'warning');
            }
        }, function (d) {
            ShowLightBox(false, "");
            showMsg('Alerta', d.responseText, 'error');
        }, true);
    };

    var ayudaCuenta_FindByPopUp = function () {
        escondeMsg();
        var parametros = $('#AyudaCuenta').getValuesByPopUp();
        parametros.ID = $('#ddl-empresa option:selected').val();

        ejecutaAjax('PresupuestoContable.aspx/AyudaCuenta_FindByPopUp', 'POST', { value: JSON.stringify(parametros) }, function (d) {
            ShowLightBox(false, "");
            if (d.d.EsValido) {
                $('#AyudaCuenta').showFindByPopUp(d.d.Datos);
            } else {
                $('#AyudaCuenta_HelpLoad').css({ "display": "none" });
                showMsg('Alerta', d.d.Mensaje, 'warning');
            }
        }, function (d) {
            ShowLightBox(false, "");
            showMsg('Alerta', d.responseText, 'error');
        }, true);
    };

    var ayudaCuenta_onElementFound = function () {
        var cadena = $('#AyudaCuenta_Code').val();
        $('#AyudaCuenta_Code').val(formatoCuenta(cadena));
    };

    $('body').on('keypress', '#AyudaCuenta_Code', null, function (e) {
        var input = $('#AyudaCuenta_Code'),
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
        $('#AyudaCuenta').helpField({
            title: 'Cuenta',
            codeNumeric: false,
            maxlengthCode: 30,
            enableCode: true,
            enableDescription: false,
            codePaste: false,
            nextControlID: 'txtAnio',
            widthCode: 170,
            //widthDescription: '0',
            requeridField: false,
            cultureResourcesPopUp: {
                popUpHeader: 'Búsqueda de Cuentas'
            },
            findByPopUp: ayudaCuenta_FindByPopUp,
            findByCode: ayudaCuenta_FindByCode,
            onElementFound: ayudaCuenta_onElementFound
        });
    };



    $('body').on('keypress', '.KeyPressGrid', function (e) {
        focusNextControl(this, e);
    });

    $('input').not('input[type=button]').on('keypress', null, function (e) {
        focusNextControl(this, e);
    });



    $('body').on('click', '#btnConsultar', null, function () {
        TraerPresupuestos(1);
    });

    $('body').on('click', '#btnConsultarSinEdicion', null, function () {
        TraerPresupuestos(0);
    });

    $('body').on('focus', '.Moneda', function () {
        this.value = balor.quitarFormatoMoneda(this.value) || '';
        $(this).select();
    });

    $('body').on('focusout', '.vasioMoneda', function () {
        this.value = balor.aplicarFormatoMoneda(this.value, '$') || '0';
    });

    $('body').on('focus', '.Cero', function () {
        this.value = (this.value > 0 ? this.value : '');
        $(this).select();
    });

    $('body').on('focusout', '.vasioCero', function () {
        this.value = (this.value == '' ? '0' : this.value);
    });

    $('body').on('keypress', '.entero', function (e) {
        return balor.onlyInteger(e);
    });


    var ValidaImprimir = function () {
        if ($("#txtAnio").val() == "") {
            showMsg('Alerta', "Debe ingresar el año a consultar", 'warning');
            $("#txtAnio").focus();
            return false;
        }       
        return true;
    };


    $('body').on('click', '#btnImprimir', null, function () {
        if (ValidaImprimir()) {
            var parametros = "&empresaid=" + ($("#rdIndividual").is(':checked') ? $('#ddl-empresa option:selected').val() : "");
            parametros += "&anio=" + $('#txtAnio').val();
            window.open("../../Base/Formas/ReporteProduccion.aspx?Formato=0&Nombre=PresupuestoContable" + parametros, 'w_PopPresupuestoContable' + $('#txtAnio').val() + $('#ddl-empresa option:selected').val(), 'toolbar=no,location=no,directories=no,status=no,menubar=no,scrollbars=yes,resizable=yes,width=760,height=560');
        }
    });

    var TraerPresupuestos = function (PermisoEditar) {
        $("#tbPresupuestos tbody").html("");
        $("#hdnAfecta").val(0);

        var parametros = {};
        ShowLightBox(true, "Espere porfavor");
        parametros.empresaid = $('#ddl-empresa option:selected').val();
        parametros.ejercicio = $("#txtAnio").val();
        var codigo = llenarSubcuentas($('#AyudaCuenta').getValuesByCode().Codigo).replace(/-/g, '');
        parametros.cuenta = padRight(codigo, 24);

        $("#hdnAfecta").val(TraerCuentaAfecta(parametros.empresaid, parametros.cuenta));

        ejecutaAjax('PresupuestoContable.aspx/TraerProyeccionContable', 'POST', parametros, function (d) {
            ShowLightBox(false, "");
            if (d.d.EsValido) {
                ProcesarGridPresupuestos(d.d.Datos, PermisoEditar)
            } else {
                showMsg('Alerta', d.d.Mensaje, 'error');
            }
        }, function (d) {
            ShowLightBox(false, "");
            showMsg('Alerta', d.responseText, 'error');
        }, true);

    };

    //TraerCuentaAfecta
    var TraerCuentaAfecta = function (EmpresaID, CuentaID) {
        var parametros = {};
        parametros.empresaid = EmpresaID;
        parametros.cuenta = CuentaID;
        var iAfecta = 0;

        ejecutaAjax('PresupuestoContable.aspx/TraerCuentaAfecta', 'POST', parametros, function (d) {
            if (d.d.EsValido) {
                if(d.d.Datos.Afecta == true)
                {
                    iAfecta = 1;
                }
            } else {
                showMsg('Alerta', d.d.Mensaje, 'error');
            }
        }, function (d) {
            showMsg('Alerta', d.responseText, 'error');
        }, false);

        return iAfecta;
    };

    function mayus(e) {
        e.value = e.value.toUpperCase();
    }

    var ProcesarGridPresupuestos = function (Datos, PermisoEditar) {
        var resHtml = '';
        var old = "odd";
        var entra = false;
        for (var i = 1; i < 13; i++) {
            if (PermisoEditar == 1) {
                resHtml += '<tr class="' + (entra ? old : '') + '" data-UltimaAct="' + Datos[0].UltimaAct + '" data-Proyeccionid="' + Datos[0].Proyeccionid + '" data-EmpresaID="' + Datos[0].Empresaid + '" data-idMes = "' + i + '" >';
                resHtml += '<td>' + nombreMes(i) + '</td>';
                columnaPresupuesto = "Datos[0].Presupuesto" + i.toString();
                resHtml += '<td><input type="text"  class="txtPresupuestoMensual KeyPressGrid inputcorto Moneda vasioMoneda derecha "  value="' + balor.aplicarFormatoMoneda(eval(columnaPresupuesto), "$") + '"/></td>';
                columnaObservacion = "Datos[0].Observacion" + i.toString();
                resHtml += '<td><input type="text" maxlength="1000" onKeyUp="this.value=this.value.toUpperCase();" style="width:400px" class="txtObservacionMensual" value="' + eval(columnaObservacion) + '"/></td>';
                //' + (Afecta = 0 ? 'readonly' : '') + '
                resHtml += '</tr>';
                entra = (!entra ? true : false);
            } else {
                resHtml += '<tr class="' + (entra ? old : '') + '" data-UltimaAct="' + Datos[0].UltimaAct + '" data-Proyeccionid="' + Datos[0].Proyeccionid + '" data-EmpresaID="' + Datos[0].Empresaid + '" data-idMes = "' + i + '" >';
                resHtml += '<td>' + nombreMes(i) + '</td>';
                columnaPresupuesto = "Datos[0].Presupuesto" + i.toString();
                resHtml += '<td><input type="text" disabled class="txtPresupuestoMensual KeyPressGrid inputcorto Moneda vasioMoneda derecha "  value="' + balor.aplicarFormatoMoneda(eval(columnaPresupuesto), "$") + '"/></td>';
                columnaObservacion = "Datos[0].Observacion" + i.toString();
                resHtml += '<td><input type="text" maxlength="1000" onKeyUp="this.value=this.value.toUpperCase();" style="width:400px" class="txtObservacionMensual" value="' + eval(columnaObservacion) + '"/></td>';
                //' + (Afecta = 0 ? 'readonly' : '') + '
                resHtml += '</tr>';
                entra = (!entra ? true : false);
            }
            
        }
        $("#tbPresupuestos tbody").html(resHtml);
    };

    var nombreMes = function (numMes) {
        var nomMes = "";
        switch (numMes) {
            case 1:
                nomMes = "Enero";
                break;
            case 2:
                nomMes = "Febrero";
                break;
            case 3:
                nomMes = "Marzo";
                break;
            case 4:
                nomMes = "Abril";
                break;
            case 5:
                nomMes = "Mayo";
                break;
            case 6:
                nomMes = "Junio";
                break;
            case 7:
                nomMes = "Julio";
                break;
            case 8:
                nomMes = "Agosto";
                break;
            case 9:
                nomMes = "Septiembre";
                break;
            case 10:
                nomMes = "Octubre";
                break;
            case 11:
                nomMes = "Noviembre";
                break;
            case 12:
                nomMes = "Diciembre";
                break;
        }
        return nomMes;
    };

    $('body').on('click', '#btnGuardar', null, function () {
        $("#btnGuardar").attr("disabled", "disabled");
        if ($("#hdnAfecta").val() == 1) {
            GuardarPresupuestos();
        } else {
            ShowLightBox(false, "");
            showMsg('Alerta', 'Solo esta permitida la captura de cuentas afectables, favor de verificar.', 'error');
        }
    });

    var GuardarPresupuestos = function () {
        var lstPropuestas = [];
        var datosPresupuesto = {};
        datosPresupuesto.hola = "hola";
        datosPresupuesto.Empresaid = $('#ddl-empresa option:selected').val();
        datosPresupuesto.Ejercicio = $("#txtAnio").val();
        var codigo = llenarSubcuentas($('#AyudaCuenta').getValuesByCode().Codigo).replace(/-/g, '');        
        datosPresupuesto.Cuenta = padRight(codigo, 24);
        datosPresupuesto.Capturado = true;
        $('#tbPresupuestos tbody tr').each(function () {
            var columna = "datosPresupuesto.Presupuesto" + $(this).attr("data-idMes") + " = " + balor.quitarFormatoMoneda($(this).find('.txtPresupuestoMensual').val()) + ";";
            eval(columna);
            var columnaObservacion = "datosPresupuesto.Observacion" + ($(this).attr("data-idMes")) + " = '" + ($(this).find('.txtObservacionMensual').val()) + "';";
            eval(columnaObservacion);
            datosPresupuesto.Observacion = $(this).find('.txtObservacionMensual').val();
            datosPresupuesto.Proyeccionid = $(this).attr("data-Proyeccionid");
            datosPresupuesto.UltimaAct = $(this).attr("data-UltimaAct")
            datosPresupuesto.Estatus = 1;
            datosPresupuesto.Usuario = getUsuario();
        });
        lstPropuestas.push(datosPresupuesto);

        ShowLightBox(true, "Espere porfavor");
        ejecutaAjax('PresupuestoContable.aspx/GuardarPresupuestos', 'POST', { value: JSON.stringify(lstPropuestas) }, function (d) {
            ShowLightBox(false, "");
            if (d.d.EsValido) {
                showMsg('Mensaje', "Datos guardados correctamente", 'success');
                TraerPresupuestos();
            } else {
                showMsg('Alerta', d.d.Mensaje, 'error');
            }
        }, function (d) {
            ShowLightBox(false, "");
            showMsg('Alerta', d.responseText, 'error');
        }, true);
    };


    $('body').on('click', '#btnLimpiar', null, function () {
        window.location.replace('PresupuestoContable.aspx');
    });

    var AplicarEstilo = function () {
        $(".svGrid th, .svGrid .header").css({
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
            $("#Asignar-body").css({ 'height': '300px', 'width': '200px', 'overflow-x': 'hidden', 'overflow-y': 'scroll' });
        }
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
        ejecutaAjax('PresupuestoContable.aspx/TraerEmpresas', 'POST', null, llenarComboEmpresas, null, false);
        $('#ddl-empresa').focus();
    });
}(jQuery, window.balor, window.amplify));