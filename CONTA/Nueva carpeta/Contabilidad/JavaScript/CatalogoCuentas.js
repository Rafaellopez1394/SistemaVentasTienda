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

    var ayudaCuenta_FindByCode = function () {
        var parametros = $('#AyudaCuenta').getValuesByCode();
        var codigo = llenarSubcuentas(parametros.Codigo).replace(/-/g, '');
        escondeMsg();
        parametros.Codigo = padRight(codigo, 24);
        parametros.ID = amplify.store.sessionStorage("EmpresaID");
        var mayor = ($("#rdCuentaMayor").is(':checked') ? 1 : 0);
        if (mayor == 1) {
            var cuentaMayor = (parametros.Codigo.substring(4, 24) * 1);
            if (cuentaMayor > 0) {
                showMsg('Alerta', "Las cuentas de mayor no pueden contener mas de un nivel", 'warning');
                $('#AyudaCuenta_Code').val("");
                return;
            }
        }
        ejecutaAjax('CatalogoCuentas.aspx/AyudaCuenta_FindByCode', 'POST', { "value": JSON.stringify(parametros), "mayor": mayor }, function (d) {
            if (($("#CajaNuevo").val() * 1) == 1) {
                //Nuevo
                if (d.d.EsValido) {
                    showMsg('Alerta', 'La cuenta especificada ya existe en el catalogo, Intenta con otra', 'warning');
                    $('#AyudaCuenta_HelpLoad').css({ "display": "none" });
                } else {
                    if (mayor == 1) {
                        $('#AyudaCuenta_HelpLoad').css({ "display": "none" });
                        $('#AyudaCuenta_Code').val(formatoCuenta(parametros.Codigo));
                    } else {
                        ValidarNivelAnterior(formatoCuenta(parametros.Codigo));
                    }
                }
            } else {
                //Consulta
                if (d.d.EsValido) {
                    $('#AyudaCuenta').showFindByCode(d.d.Datos);
                } else {
                    $('#AyudaCuenta_HelpLoad').css({ "display": "none" });
                    showMsg('Error', d.d.Mensaje, 'error');
                }
            }
        }, function (d) {
            showMsg('Alerta', d.responseText, 'error');
        }, true);
    };

    ValidarNivelAnterior = function (cuenta) {
        var Niveles = cuenta.split('-');
        var nivel = 0;
        for (i = 0; i < Niveles.length; i++) {
            if ((Niveles[i] * 1) > 0) {
                nivel = nivel + 1;
            }
        }
        var nivelAnterior = nivel - 1;
        var CuentaNivelAnterior = "";
        for (i = 0; i < nivelAnterior; i++) {
            if ((Niveles[i] * 1) > 0) {
                CuentaNivelAnterior += Niveles[i];
            }
        }
        CuentaNivelAnterior = llenarSubcuentas(CuentaNivelAnterior).replace(/-/g, '');
        CuentaNivelAnterior = padRight(CuentaNivelAnterior, 24);

        ejecutaAjax('CatalogoCuentas.aspx/ConsultarCuentaPorCodigo', 'POST', { "cuentaContable": CuentaNivelAnterior, "empresaid": amplify.store.sessionStorage("EmpresaID") }, function (d) {
            if (d.d.EsValido) {
                if (d.d.Datos[0].Codigo == "True") {
                    showMsg('Alerta', 'La cuenta contable del nivel anterior es afectable, no puedes registrar un nivel mas', 'warning');
                    $('#AyudaCuenta_HelpLoad').css({ "display": "none" });
                } else {
                    $('#AyudaCuenta_HelpLoad').css({ "display": "none" });
                    $('#AyudaCuenta_Code').val(cuenta);
                }
            } else {
                showMsg('Alerta', 'La cuenta contable del nivel anterior no existe en el catalogo', 'warning');
                $('#AyudaCuenta_HelpLoad').css({ "display": "none" });
            }
        }, function (d) {
            showMsg('Alerta', d.responseText, 'error');
        }, true);
    };

    var ayudaCuenta_FindByPopUp = function () {
        escondeMsg();
        var parametros = $('#AyudaCuenta').getValuesByPopUp();
        parametros.ID = amplify.store.sessionStorage("EmpresaID");
        var mayor = ($("#rdCuentaMayor").is(':checked') ? 1 : 0);
        ejecutaAjax('CatalogoCuentas.aspx/AyudaCuenta_FindByPopUp', 'POST', { "value": JSON.stringify(parametros), "mayor": mayor }, function (d) {
            if (d.d.EsValido) {
                $('#AyudaCuenta').showFindByPopUp(d.d.Datos);
            } else {
                $('#AyudaCuenta_HelpLoad').css({ "display": "none" });
                showMsg('Error', d.d.Mensaje, 'error');
            }
        }, function (d) {
            showMsg('Alerta', d.responseText, 'error');
        }, true);
    };

    var ConsultarInfoCuenta = function () {
        escondeMsg();
        ShowLightBox(true, "Cargando Informacion...");
        var cadena = $('#AyudaCuenta_Code').val();
        $('#AyudaCuenta_Code').val(formatoCuenta(cadena));
        var CuentaID = $('#AyudaCuenta').getValuesByCode().ID;
        ejecutaAjax('CatalogoCuentas.aspx/TraerDatosCuentas', 'POST', { "CuentaID": CuentaID }, function (d) {
            ShowLightBox(false, "");
            if (d.d.EsValido) {
                $("#CajaDescripcionIngles").val(d.d.Datos[0].DescripcionIngles);
                $("#AcvCtamID").val(d.d.Datos[0].AcvCtamID);
                $("#UltimaActCuenta").val(d.d.Datos[0].UltimaActCuenta);
                $("#UltimaActCuentaMayor").val(d.d.Datos[0].UltimaActCuentaMayor);

                var DatosFlujoCargo = { ID: d.d.Datos[0].FlujoIDCargo, Codigo: d.d.Datos[0].Cod_FlujoCargo, Descripcion: d.d.Datos[0].DescripcionCargo };

                var DatosFlujoAbono = { ID: d.d.Datos[0].FlujoIDAbono, Codigo: d.d.Datos[0].Cod_FlujoAbono, Descripcion: d.d.Datos[0].DescripcionAbono };

                if (DatosFlujoCargo.ID != '00000000-0000-0000-0000-000000000000') {
                    $("#AyudaFlujoCargo_ValorID").val(DatosFlujoCargo.ID);
                    $('#AyudaFlujoCargo_Code').val(DatosFlujoCargo.Codigo);
                    $('#AyudaFlujoCargo_lBox').val(DatosFlujoCargo.Descripcion);
                }

                if (DatosFlujoAbono.ID != '00000000-0000-0000-0000-000000000000') {
                    $("#AyudaFlujoAbono_ValorID").val(DatosFlujoAbono.ID);
                    $('#AyudaFlujoAbono_Code').val(DatosFlujoAbono.Codigo);
                    $('#AyudaFlujoAbono_lBox').val(DatosFlujoAbono.Descripcion);
                }

                if (d.d.Datos[0].CtaSat != '') {
                    $("#AyudaCtaSat_ValorID").val(d.d.Datos[0].CtaSat);
                    $('#AyudaCtaSat_Code').val(d.d.Datos[0].CtaSat);
                    $('#AyudaCtaSat_lBox').val(d.d.Datos[0].DescripcionCtaSat);
                }

                if (d.d.Datos[0].Afecta == true) {
                    $("#chkAfectable").attr("checked", "checked");
                }
                if (d.d.Datos[0].IETU == true) {
                    $("#chkIETU").attr("checked", "checked");
                }
                if (d.d.Datos[0].ISR == true) {
                    $("#chkISR").attr("checked", "checked");
                }
                if (d.d.Datos[0].Sistema == true) {
                    $("#chkSistema").attr("checked", "checked");
                }
                //
                if (d.d.Datos[0].Moneda == 2) {
                    $("#chkDolar").attr("checked", "checked");
                }
                //Naturaleza de la Cuenta
                if (d.d.Datos[0].Nat_Cta == "D") {
                    $("#rdAcreedora").removeAttr("checked");
                    $("#rdDeudora").attr("checked", "checked");
                } else {
                    $("#rdDeudora").removeAttr("checked");
                    $("#rdAcreedora").attr("checked", "checked");
                }
                //Establecemos el Grupo al que pertenece la cuenta
                $("#ComboGrupo").val(d.d.Datos[0].Cod_Gpo);

                //Tipificar las cuentas contables
                if (d.d.Datos[0].Cod_Gpo == "07") {
                    $("#ComboTipo").val(d.d.Datos[0].Tipo_Cta);
                }
                
                //Es una cuenta contable que no ha tenido movimientos por lo tanto se puede modificar si es afectable o no..
                /*
                if ((d.d.Datos[0].Movimientos * 1) == 0) {
                if ((d.d.Datos[0].MovimientosHijos * 1) == 0) {
                $("#AyudaCuenta_Code").attr("disabled", "disabled");
                HabilitarControles();
                } else {
                $("#rdCuentaMayor,#rdSubCuenta,#BotonNuevo,#AyudaCuenta_Code").attr("disabled", "disabled");
                }
                } else {
                $("#rdCuentaMayor,#rdSubCuenta,#BotonNuevo,#AyudaCuenta_Code").attr("disabled", "disabled");
                }
                */
                //Se habilito la modificacion del catalogo de cuentas
                $("#AyudaCuenta_Code").attr("disabled", "disabled");
                HabilitarControles();

                if (d.d.Datos[0].Nivel == 1) {
                    $("#chkAfectable").attr("disabled", "disabled");
                }
                if (d.d.Datos[0].Nivel != 1 && d.d.Datos[0].Afecta == false && d.d.Datos[0].Movimientos != 0) {
                    $("#chkAfectable").attr("disabled", "disabled");
                }

            } else {
                ShowLightBox(false, "");
                $('#AyudaCuenta_HelpLoad').css({ "display": "none" });
                showMsg('Error', d.d.Mensaje, 'error');
            }
        }, function (d) {
            ShowLightBox(false, "");
            showMsg('Alerta', d.responseText, 'error');
        }, true);
    };


    var ayudaCuenta_onElementFound = function () {
        ConsultarInfoCuenta();
    };


    HabilitarControles = function () {
        $("#rdCuentaMayor,#rdSubCuenta,#BotonNuevo").attr("disabled", "disabled");
        $('#AyudaFlujoCargo').EnableHelpField(true);
        $('#AyudaFlujoAbono').EnableHelpField(true);
        $("#chkAfectable,#chkIETU,#chkISR,#chkSistema, #chkDolar, #BotonGuardar").removeAttr("disabled");
    };

    /**************************************************************** A Y U D A    C U E N T A S   ***************************************************/

    InstanciarAyudas = function () {
        $('#AyudaCuenta').helpField({
            title: 'Cuenta',
            codeNumeric: false,
            maxlengthCode: 30,
            enableCode: true,
            enableDescription: true,
            codePaste: false,
            nextControlID: '',
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

        $('#AyudaFlujoCargo').helpField({
            title: 'Flujo Cargo',
            codeNumeric: false,
            maxlengthCode: 30,
            enableCode: true,
            enableDescription: false,
            codePaste: false,
            nextControlID: '',
            widthCode: 170,
            //widthDescription: '0',
            requeridField: false,
            cultureResourcesPopUp: {
                popUpHeader: 'Búsqueda de Flujos'
            },
            findByPopUp: AyudaFlujoCargo_FindByPopUp,
            findByCode: AyudaFlujoCargo_FindByCode
            //onElementFound: 'ayudaCuenta_onElementFound'
        });
        $('#AyudaFlujoAbono').helpField({
            title: 'Flujo Abono',
            codeNumeric: false,
            maxlengthCode: 30,
            enableCode: true,
            enableDescription: false,
            codePaste: false,
            nextControlID: '',
            widthCode: 170,
            //widthDescription: '0',
            requeridField: false,
            cultureResourcesPopUp: {
                popUpHeader: 'Búsqueda de Abonos'
            },
            findByPopUp: AyudaFlujoAbono_FindByPopUp,
            findByCode: AyudaFlujoAbono_FindByCode
            //onElementFound: 'ayudaCuenta_onElementFound'
        });
        $('#AyudaCtaSat').helpField({
            title: 'Cuenta Sat',
            codeNumeric: false,
            //maxlengthCode: 30,
            enableCode: true,
            enableDescription: false,
            codePaste: false,
            nextControlID: 'AyudaFlujoCargo',
            //widthCode: 170,
            //widthDescription: '0',
            requeridField: false,
            cultureResourcesPopUp: {
                popUpHeader: 'Búsqueda de Cuenta Sat'
            },
            findByPopUp: AyudaCtaSat_FindByPopUp,
            findByCode: AyudaCtaSat_FindByCode
            //onElementFound: 'ayudaCuenta_onElementFound'
        });
    }
    var AyudaCtaSat_FindByPopUp = function () {
        ejecutaAjax('CatalogoCuentas.aspx/AyudaCtaSat_FindByPopUp', 'POST', { value: JSON.stringify($('#AyudaCtaSat').getValuesByPopUp()) }, function (d) {
            if (d.d.EsValido) {
                $("#AyudaCtaSat").showFindByPopUp(d.d.Datos);
            } else {
                showMsg('Alerta', d.d.Mensaje, 'warning');
            }
        }, function (d) {
            showMsg('Alerta', d.responseText, 'error');
        }, false);
    };

    var AyudaCtaSat_FindByCode = function () {
        ejecutaAjax('CatalogoCuentas.aspx/AyudaCtaSat_FindByCode', 'POST', { value: JSON.stringify($('#AyudaCtaSat').getValuesByCode()) }, function (d) {
            if (d.d.EsValido) {
                $("#AyudaCtaSat").showFindByCode(d.d.Datos);
            } else {
                $('#AyudaCtaSat_HelpLoad').css({ "display": "none" });
                showMsg('Alerta', d.d.Mensaje, 'warning');
            }
        }, function (d) {
            showMsg('Alerta', d.responseText, 'error');
        }, false);
    };

    var AyudaFlujoAbono_FindByCode = function () {
        ejecutaAjax('CatalogoCuentas.aspx/TraerFlujosPorCodigo', 'POST', { value: JSON.stringify($('#AyudaFlujoAbono').getValuesByCode()) }, function (d) {
            if (d.d.EsValido) {
                $("#AyudaFlujoAbono").showFindByCode(d.d.Datos);
            } else {
                showMsg('Alerta', d.d.Mensaje, 'warning');
            }
        }, function (d) {
            showMsg('Alerta', d.responseText, 'error');
        }, false);
    };

    var AyudaFlujoAbono_FindByPopUp = function () {
        ejecutaAjax('CatalogoCuentas.aspx/TraerFlujosPorCodigo', 'POST', { value: JSON.stringify($('#AyudaFlujoAbono').getValuesByPopUp()) }, function (d) {
            if (d.d.EsValido) {
                $("#AyudaFlujoAbono").showFindByPopUp(d.d.Datos);
            } else {
                showMsg('Alerta', d.d.Mensaje, 'warning');
            }
        }, function (d) {
            showMsg('Alerta', d.responseText, 'error');
        }, false);
    };

    var AyudaFlujoCargo_FindByCode = function () {
        ejecutaAjax('CatalogoCuentas.aspx/TraerFlujosPorCodigo', 'POST', { value: JSON.stringify($('#AyudaFlujoCargo').getValuesByCode()) }, function (d) {
            if (d.d.EsValido) {
                $("#AyudaFlujoCargo").showFindByCode(d.d.Datos);
            } else {
                showMsg('Alerta', d.d.Mensaje, 'warning');
            }
        }, function (d) {
            showMsg('Alerta', d.responseText, 'error');
        }, false);
    };

    var AyudaFlujoCargo_FindByPopUp = function () {
        ejecutaAjax('CatalogoCuentas.aspx/TraerFlujosPorCodigo', 'POST', { value: JSON.stringify($('#AyudaFlujoCargo').getValuesByPopUp()) }, function (d) {
            if (d.d.EsValido) {
                $("#AyudaFlujoCargo").showFindByPopUp(d.d.Datos);
            } else {
                showMsg('Alerta', d.d.Mensaje, 'warning');
            }
        }, function (d) {
            showMsg('Alerta', d.responseText, 'error');
        }, false);
    };

    var ObtenerNivel = function (cuenta) {
        var Niveles = cuenta.split('-');
        var nivel = 0;
        for (i = 0; i < Niveles.length; i++) {
            if ((Niveles[i] * 1) > 0) {
                nivel = nivel + 1;
            }
        }
        return nivel;
    };

    var GuardarCuentaContable = function () {
        var parametros = $('#AyudaCuenta').getValuesByCode();
        var codigo = llenarSubcuentas(parametros.Codigo).replace(/-/g, '');
        parametros.Codigo = padRight(codigo, 24);
        CuentaContable = {};
        CuentaContable.Cuentaid = parametros.ID;
        CuentaContable.Empresaid = amplify.store.sessionStorage("EmpresaID");
        //CuentaContable.CodEmpresa = "1";
        CuentaContable.Cuenta = parametros.Codigo;
        CuentaContable.Descripcion = parametros.Descripcion;
        CuentaContable.Descripcioningles = $("#CajaDescripcionIngles").val();
        CuentaContable.Nivel = ObtenerNivel($('#AyudaCuenta').getValuesByCode().Codigo);
        CuentaContable.Afecta = $("#chkAfectable").is(':checked');
        CuentaContable.Moneda = $("#chkDolar").is(':checked')? 2 : 1;
        CuentaContable.Sistema = $("#chkSistema").is(':checked');
        CuentaContable.Ietu = $("#chkIETU").is(':checked');
        CuentaContable.Isr = $("#chkISR").is(':checked');
        CuentaContable.Saldo = 0;
        CuentaContable.FlujoCar = $('#AyudaFlujoCargo').getValuesByCode().Codigo;
        CuentaContable.FlujoAbo = $('#AyudaFlujoAbono').getValuesByCode().Codigo;
        CuentaContable.Estatus = 1;
        CuentaContable.CtaSat = $('#AyudaCtaSat').getValuesByCode().ID;
        CuentaContable.UltimaAct = $("#UltimaActCuenta").val();
        var CuentaMayor = {};
        var mayor = ($("#rdCuentaMayor").is(':checked') ? 1 * 1 : 0 * 1);
        if (mayor == 1) {
            var Naturaleza = ($("#rdDeudora").is(':checked') ? "D" : "A");
            CuentaMayor.Acvctamid = $("#AcvCtamID").val();
            CuentaMayor.Empresaid = amplify.store.sessionStorage("EmpresaID");
            //CuentaMayor.CodEmpresa = "1";
            CuentaMayor.Cuenta = parametros.Codigo;
            CuentaMayor.NatCta = Naturaleza;
            CuentaMayor.CodGpo = $("#ComboGrupo").val();
            CuentaMayor.TipoCta = $("#ComboTipo").val();
            CuentaMayor.Estatus = "1";
            //CuentaMayor.Fecha = "01/01/1990";
            CuentaMayor.UltimaAct = $("#UltimaActCuentaMayor").val(); ;
        }
        ejecutaAjax('CatalogoCuentas.aspx/GuardarCuentaContable', 'POST', { "cuentaContable": JSON.stringify(CuentaContable), "cuentaMayor": JSON.stringify(CuentaMayor), "mayor": mayor }, function (d) {
            if (d.d.EsValido) {
                showMsg('Alerta', 'Cuenta Contable Guardada Correctamente', 'success');
                $("#AcvCtamID").val(d.d.Datos.AcvCtamID);
                $("#UltimaActCuenta").val(d.d.Datos.UltimaActCuenta);
                $("#UltimaActCuentaMayor").val(d.d.Datos.UltimaActCuentaMayor);
                $("#AyudaCuenta_ValorID").val(d.d.Datos.CuentaID);
                ConsultarInfoCuenta();
            } else {
                showMsg('Alerta', d.d.Mensaje, 'warning');
            }
        }, function (d) {
            showMsg('Alerta', d.responseText, 'error');
        }, true);
    };

    var GenerarEnvioSat = function () {

        var parametros = {};
        parametros.empresaid = amplify.store.sessionStorage("EmpresaID");
        parametros.usuario = getUsuario();
        parametros.fecha = $("#fechaInicio").val();
        ShowLightBox(true, "Generando XML espere porfavor");
        ejecutaAjax('CatalogoCuentas.aspx/GenerarXMLCatalogoCtas', 'POST', parametros, function (d) {
            ShowLightBox(false, "");
            if (d.d.EsValido) {
                showMsg('Alerta', 'Archivo generado existosamente en un momento iniciara la descarga', 'success');
                $("#myDownloaderFrame").attr("src", "../../Base/Formas/DownloadFile.aspx?ruta=" + d.d.Datos.Ruta + "&archivo=" + d.d.Datos.FileName);
            } else {
                showMsg('Alerta', d.d.Mensaje, 'warning');
            }
        }, function (d) {
            ShowLightBox(false, "");
            showMsg('Alerta', d.responseText, 'error');
        }, true);
    };

    $("#BotonGuardar").click(function () {
        GuardarCuentaContable();
    });

    $('body').on('click', 'input[name=cuenta]:radio', null, function () {
        if (this.value == "1") {
            $("#rdAcreedora,#rdDeudora,#ComboGrupo,#ComboTipo").removeAttr("disabled");
        } else {
            $("#rdAcreedora,#rdDeudora,#ComboGrupo,#ComboTipo").attr("disabled", "disabled");
        }
    });

    $("#BotonGenerarSat").click(function () {
        if (!$("#fechaInicio").val()) {
            alert("Ingresa la fecha de corte del periodo");
            $("#fechaInicio").focus();
            return;
        }
        GenerarEnvioSat();
    });

    $("#btnLimpiar").click(function () {
        window.location.replace("CatalogoCuentas.aspx");
    });
    $("#BotonNuevo").click(function () {
        $("#CajaNuevo").val(1);
        HabilitarControles();
    });

    $('body').on('click', '#BotonImprimir', null, function () {
        var parametros = "&Ingles=" + "0";
        parametros += "&empresaid=" + amplify.store.sessionStorage("EmpresaID");
        var TImpresion = balor.radioGetCheckedValue('timprimir');
        if (TImpresion === "P") {
            window.open("../../Base/Formas/ReporteProduccion.aspx?Formato=0&Nombre=CatalogoCuentas" + parametros, 'w_PopCatalogoCuentas', 'toolbar=no,location=no,directories=no,status=no,menubar=no,scrollbars=yes,resizable=yes,width=760,height=560');
        } else {
            $("#myDownloaderFrame").attr("src", "../../Base/Formas/GenerarExcel.aspx?NomReporte=CatalogoCuentas" + parametros);
        }
    });

    $('input').not('input[type=button]').on('keypress', null, function (e) {
        focusNextControl(this, e);
    });

    $('body').on('click', '#MobileMenu', null, function () {
        $("#MobileMenuDet").slideToggle("slow");
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
        InstanciarAyudas();
    });
} (jQuery, window.balor, window.amplify));