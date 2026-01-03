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

    var elemById = function (id) {
        return document.getElementById(id);
    };

    var focusNextControl = function (control, e) {
        var code = (e.keyCode ? e.keyCode : e.which);
        if (code === 13) {
            var inputs = $(control).closest('form').find('input[type=text]:enabled, input[type=password]:enabled, input[type=button]:enabled, input[type=checkbox]:enabled, input[type=radio]:enabled, input[type=date]:enabled, input[type=tel]:enabled, input[type=email]:enabled, input[type=number]:enabled, #txtConcepto, select:enabled').not('input[readonly=readonly], fieldset:hidden *');
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

    var getEmpresaid = function () {
        return amplify.store.sessionStorage("EmpresaID");
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

    var ayudaPoliza_FindByCode = function () {
        if ($("#txtFechaPoliza").val() == "") {
            showMsg('Alerta', 'Primero seleccione una Fecha', 'warning');
            $("#AyudaPoliza_Code").val("");
            $("#txtFechaPoliza").focus();
            return;
        }
        var parametros = { folio: $('#AyudaPoliza').getValuesByCode().Codigo, tippol: $('#ddlTipPol').val(), EmpresaId: $("#ddl-empresa").val(), fechapol: $("#txtFechaPoliza").val() };
        escondeMsg();
        ShowLightBox(true, "Espere porfavor");
        $(parametros).ToRequest({
            url: 'CapturaPolizas.aspx/AyudaPoliza_FindByCode',
            success: function (d) {
                if (d.d.EsValido) {
                    ShowLightBox(false, "");
                    $('#AyudaPoliza').showFindByCode(d.d.Datos);
                } else {
                    $('#AyudaPoliza_HelpLoad').css({ "display": "none" });
                    showMsg('Alerta', d.d.Mensaje, 'warning');
                }
            },
            failure: function (d) {
                showMsg('Error', d.responseText, 'error');
            },
            Titulo: 'Espere Por Favor',
            ConvertirObjetoJson: false
        });
    };

    var ayudaPoliza_FindByPopUp = function () {
        var parametros = { descripcion: $('#AyudaPoliza').getValuesByPopUp().Descripcion, tippol: $('#ddlTipPol').val(), EmpresaId: $("#ddl-empresa").val(), fechapol: $("#txtFechaPoliza").val(), Pendiente: $('#chkPendiente').is(':checked') };
        escondeMsg();
        ShowLightBox(true, "Espere porfavor");
        $(parametros).ToRequest({
            url: 'CapturaPolizas.aspx/AyudaPoliza_FindByPopUp',
            success: function (d) {
                if (d.d.EsValido) {
                    ShowLightBox(false, "");
                    $('#AyudaPoliza').showFindByPopUp(d.d.Datos);
                } else {
                    $('#AyudaPoliza_HelpLoad').css({ "display": "none" });
                    showMsg('Alerta', d.d.Mensaje, 'warning');
                }
            },
            failure: function (d) {
                showMsg('Error', d.responseText, 'error');
            },
            Titulo: 'Espere Por Favor',
            ConvertirObjetoJson: false
        });
    };

    $('body').on('click', '#AyudaPoliza_HelpButton', null, function () {
        if ($("#txtFechaPoliza").val() == "") {
            showMsg('Alerta', 'Primero seleccione una Fecha', 'warning');
        }
    });
    

    var ayudaPoliza_onElementFound = function () {
        var parametros = { empresaid: $("#ddl-empresa").val(), polizaid: $('#AyudaPoliza').getValuesByCode().ID };
        escondeMsg();

        ejecutaAjax('ProgramacionDePagos.aspx/ValidaPolizaProgamada', 'POST', parametros, function (d) {
            if (d.d.EsValido) {
                if (d.d.Datos == 0)
                {
                    showMsg('Alerta', 'Esta poliza contiene una cuenta para excluir', 'warning');
                    $('#AyudaPoliza').clearHelpField();
                }
                if (d.d.Datos == 2)
                {
                    showMsg('Alerta', 'Esta poliza no esta registrada para pago programado', 'warning');
                    $('#AyudaPoliza').clearHelpField();
                }

                
            } else {
                showMsg('Error', d.d.Mensaje, 'error');
            }
        }, function (d) {
            showMsg('Alerta', d.responseText, 'error');
        }, false);
    };

    var AyudaProveedores_FindByPopUp = function () {
        var paramAyuda = $('#AyudaProveedores').getValuesByPopUp();
        paramAyuda.ID = amplify.store.sessionStorage("EmpresaID");
        var parametros = { value: JSON.stringify(paramAyuda) };
        ejecutaAjax('CatProveedores.aspx/AyudaProveedores_FindByPopUp', 'POST', parametros, function (d) {
            if (d.d.EsValido) {
                $('#AyudaProveedores').showFindByPopUp(d.d.Datos);
            } else {
                showMsg('Alerta', d.d.Mensaje, 'warning');
            }
        }, function (d) {
            showMsg('Alerta', d.responseText, 'error');
        }, true);
    };

    var AyudaProveedores_FindByCode = function () {
        var paramAyuda = $('#AyudaProveedores').getValuesByCode();
        paramAyuda.ID = amplify.store.sessionStorage("EmpresaID");
        var parametros = { value: JSON.stringify(paramAyuda) };
        ejecutaAjax('CatProveedores.aspx/AyudaProveedores_FindByCode', 'POST', parametros, function (d) {
            if (d.d.EsValido) {
                $('#AyudaProveedores').showFindByCode(d.d.Datos);

            } else {
                $('#AyudaProveedores_HelpLoad').css({ "display": "none" });
                showMsg('Alerta', d.d.Mensaje, 'warning');
            }
        }, function (d) {
            showMsg('Alerta', d.responseText, 'error');
        }, true);
    };

    var AyudaProveedores_onElementFound = function () {
        //Se dispara cuando se encuentra o selecciona un item de la ayuda
        var parametros = { Codigo: $('#AyudaProveedores_Code').val(), Empresaid: amplify.store.sessionStorage("EmpresaID") }
        ejecutaAjax('CatProveedores.aspx/TraerProveedorPorCodigo', 'POST', parametros, function (d) {
            if (d.d.EsValido) {
               //ProcesaProveedor(d.d.Datos);
            } else {
                $('#AyudaProveedores_HelpLoad').css({ "display": "none" });
                showMsg('Alerta', d.d.Mensaje, 'warning');
            }
        }, function (d) {
            showMsg('Alerta', d.responseText, 'error');
        }, true);
    };

    var ayudaCuenta_FindByCode = function () {
        var parametros = $('#AyudaCuenta').getValuesByCode(),
        codigo = llenarSubcuentas(parametros.Codigo).replace(/-/g, '');
        escondeMsg();
        parametros.Codigo = padRight(codigo, 24);
        parametros.ID = amplify.store.sessionStorage("EmpresaID");
        $(parametros).ToRequest({
            url: 'CapturaPolizas.aspx/AyudaCuenta_FindByCode',
            success: function (d) {
                if (d.d.EsValido) {
                    $('#AyudaCuenta').showFindByCode(d.d.Datos);
                } else {
                    $('#AyudaCuenta_HelpLoad').css({ "display": "none" });
                    showMsg('Alerta', d.d.Mensaje, 'warning');
                }
            },
            failure: function (d) {
                showMsg('Error', d.responseText, 'error');
            },
            Titulo: 'Espere Por Favor'
        });
    };

    var ayudaCuenta_FindByPopUp = function () {
        escondeMsg();
        var parametros = $('#AyudaCuenta').getValuesByPopUp();
        parametros.ID = amplify.store.sessionStorage("EmpresaID");
        $(parametros).ToRequest({
            url: 'CapturaPolizas.aspx/AyudaCuenta_FindByPopUp',
            success: function (d) {
                if (d.d.EsValido) {
                    $('#AyudaCuenta').showFindByPopUp(d.d.Datos);
                } else {
                    $('#AyudaCuenta_HelpLoad').css({ "display": "none" });
                    showMsg('Alerta', d.d.Mensaje, 'warning');
                }
            },
            failure: function (d) {
                showMsg('Error', d.responseText, 'error');
            },
            Titulo: 'Espere Por Favor'
        });
    };

    var ayudaCuenta_onElementFound = function () {
        var cadena = $('#AyudaCuenta_Code').val();
        $('#AyudaCuenta_Code').val(formatoCuenta(cadena));
    };

    $('input').not('input[type=button]').on('keypress', null, function (e) {
        focusNextControl(this, e);
    });






    var TraerSolicitantes = function () {
        var parametros = {};
        ejecutaAjax("ProgramacionDePagos.aspx/TraerSolicitantes", 'POST', parametros, function (d) {
            if (d.d.EsValido) {
                ProcesarDatosSolicitantes(d.d.Datos);
            } else {
                showMsg('Alerta', d.d.Mensaje, 'warning');
            }
        }, null, true);
    };

    var ProcesarDatosSolicitantes = function (Datos) {
        $("#ddlSolicitantes option").remove();
        $("#ddlSolicitantes").append("<option value='0'>Seleccionar...</option>");
        for (var i = 0; i < Datos.length; i++) {
            $("#ddlSolicitantes").append("<option value=" + Datos[i].Solicitanteid + ">" + Datos[i].Solicitante + "</option>");
        }
        $("#ddlSolicitantes").val(0);
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

    $('body').on('click', '#btnBuscarCaptura', null, function () {
        var tipo = $("#txtTipo").val();
        CargaPagosProgramados(tipo, $("#txtFICaptura").val(), $("#txtFFCaptura").val());
    });

    $('body').on('click', '#btnBuscarAsignar', null, function () {
        var tipo = $("#txtTipo").val();
        CargaPagosProgramados(tipo, $("#txtFIAsignar").val(), $("#txtFFAsignar").val());
    });

    $('body').on('click', '#btnGuardar', null, function () {
        var tipo = $("#txtTipo").val();

        if (!ValidaGuardar(tipo))
            return;
        
        var id = ($("#hnProgramacionPagoID").val() != "" ? $("#hnProgramacionPagoID").val() : "00000000-0000-0000-0000-000000000000");
        var parametros = {};
        ShowLightBox(true, "Espere porfavor");
        if (tipo == "1")
        {
            parametros.programacionpagoid = id;
            parametros.empresaid = $("#ddl-empresa").val();
            parametros.proveedorid = $('#AyudaProveedores').getValuesByCode().ID;
            parametros.fechapago = $("#txtFechaPago").val();
            parametros.concepto = $("#txtConcepto").val();
            parametros.importe = balor.quitarFormatoMoneda($("#txtImporte").val());
            parametros.solicitante = $("#ddlSolicitantes").val();
            parametros.usuario = getUsuario();

            ejecutaAjax("ProgramacionDePagos.aspx/GuardarPago", 'POST', parametros, function (d) {
                ShowLightBox(false, "");
                if (d.d.EsValido) {
                    showMsg('Alerta', 'Pago guardado correctamente', 'success');
                    ProcesarProgramacionPagos(d.d.Datos);
                } else {
                    $('#AyudaProveedores_HelpLoad').css({ "display": "none" });
                    showMsg('Alerta', d.d.Mensaje, 'warning');
                }
            }, function (d) {
                showMsg('Alerta', d.responseText, 'error');
            }, true);
        }
        else
        {
            parametros.empresaid = $("#ddl-empresa").val();
            parametros.programacionpagoid = $("#hnProgramacionPagoID").val();
            parametros.polizaid = $("#AyudaPoliza").getValuesByCode().ID;
            parametros.usuario = getUsuario();

            ejecutaAjax("ProgramacionDePagos.aspx/AsignarPago ", 'POST', parametros, function (d) {
                ShowLightBox(false, "");
                if (d.d.EsValido) {
                    showMsg('Alerta', 'Asignacion guardada', 'success');
                    ProcesarPagosAsignacion(d.d.Datos);
                } else {
                    $('#AyudaProveedores_HelpLoad').css({ "display": "none" });
                    showMsg('Alerta', d.d.Mensaje, 'warning');
                }
            }, function (d) {
                showMsg('Alerta', d.responseText, 'error');
            }, true);
        }

        Limpiar(tipo);

    });

    $('body').on('click', '#btnListadoPagos', null, function () {
        var tipo = $("#txtTipo").val();
        ImprimirListado(tipo);
    });

    var ImprimirListado = function (tipo) {
        var parametros = "";
        parametros += "&empresaid=" + $("#ddl-empresa").val();
        window.open("../../Base/Formas/ReporteProduccion.aspx?Formato=0&Nombre=ListadoPagos" + parametros, 'w_PopListadoPagos', 'toolbar=no,location=no,directories=no,status=no,menubar=no,scrollbars=yes,resizable=yes,width=760,height=560');
    };

    $('body').on('click', '#btnRelacionPagos', null, function () {
        var tipo = $("#txtTipo").val();
        ImprimirRelacionPagos(tipo);
    });

    var ImprimirRelacionPagos = function (tipo) {
        var parametros = "";
        if (tipo == 1) {
            parametros = "&fechaInicial=" + $("#txtFICaptura").val().trim();
            parametros += "&fechaFin=" + $("#txtFFCaptura").val().trim();
            parametros += "&empresaid=" + $("#ddl-empresa").val();
        }
        else {
            parametros = "&fechaInicial=" + $("#txtFIAsignar").val().trim();
            parametros += "&fechaFin=" + $("#txtFFAsignar").val().trim();
            parametros += "&empresaid=" + $("#ddl-empresa").val();
        }
        window.open("../../Base/Formas/ReporteProduccion.aspx?Formato=0&Nombre=RelacionPagosPagosProgramados" + parametros, 'w_PopRelacionPagos', 'toolbar=no,location=no,directories=no,status=no,menubar=no,scrollbars=yes,resizable=yes,width=760,height=560');
    };

    $('body').on('click', '#btnLimpiar', null, function () {
        var tipo = $("#txtTipo").val();
        Limpiar(tipo);
        //window.location.replace('ProgramacionDePagos.aspx');
    });

    $('body').on('click', '.EliminarPago', null, function () {
        var id = $(this.parentNode).attr('data-PagoID');

        var parametros = {};
        parametros.empresaid = $("#ddl-empresa").val();
        parametros.programacionpagoid = id;
        parametros.usuario = getUsuario()

        ejecutaAjax("ProgramacionDePagos.aspx/EliminarPagoProgramado ", 'POST', parametros, function (d) {
            ShowLightBox(false, "");
            if (d.d.EsValido) {
                showMsg('Alerta', 'Pago eliminado', 'success');
                ProcesarProgramacionPagos(d.d.Datos);
            } else {
                $('#AyudaProveedores_HelpLoad').css({ "display": "none" });
                showMsg('Alerta', d.d.Mensaje, 'warning');
            }
        }, function (d) {
            showMsg('Alerta', d.responseText, 'error');
        }, true);
    });

    var ProcesarProgramacionPagos = function (Datos) {
        var resHtml = '';
        var old = "odd";
        var entra = false;
        var sumaImportePagos = 0.0;;
        for (var i = 0; i < Datos.length; i++) {
            resHtml += '<tr class="' + (entra ? old : '') + '" data-PagoID="' + Datos[i].Pagoid + '">';
            resHtml += '<td>' + Datos[i].FechaProgramada + '</td>';
            resHtml += '<td>' + Datos[i].Concepto + '</td>';
            resHtml += '<td>' + balor.aplicarFormatoMoneda(Datos[i].Importe,'$') + '</td>';
            resHtml += '<td>' + Datos[i].Solicitante + '</td>';
            resHtml += '<td class="EliminarPago centrado"><span class="closebtn elim-Pago">X</span></td>';
            resHtml += '</tr>';
            entra = (!entra ? true : false);
            sumaImportePagos += Datos[i].Importe;
        }
        $("#tablaPagosCapturados tbody").html(resHtml);
        $("#sumaImportePagos").html( balor.aplicarFormatoMoneda(sumaImportePagos,'$'));
    };

    var ProcesarPagosAsignacion = function (Datos) {
        var resHtml = '';
        var old = "odd";
        var entra = false;
        var sumaImportePagos = 0.0;;
        for (var i = 0; i < Datos.length; i++) {
            resHtml += '<tr class="' + (entra ? old : '') + '" data-PagoID="' + Datos[i].Pagoid + '">';
            resHtml += '<td>' + Datos[i].FechaProgramada + '</td>';
            resHtml += '<td>' + Datos[i].Concepto + '</td>';
            resHtml += '<td>' + balor.aplicarFormatoMoneda(Datos[i].Importe, '$') + '</td>';
            resHtml += '<td>' + Datos[i].Solicitante + '</td>';
            resHtml += '<td class="centrado"><input type="checkbox" style="height:20px; width:20px" class="ckAsignar"/></td>';
            resHtml += '</tr>';
            entra = (!entra ? true : false);
            sumaImportePagos += Datos[i].Importe;
        }
        $("#tablaPagosAsignar tbody").html(resHtml);
        $("#sumaImporteAsignar").html(balor.aplicarFormatoMoneda(sumaImportePagos, '$'));
    };

    var CargaPagosProgramados = function (tipo, fi, ff) {
        var parametros = {};
        parametros.empresaid = $("#ddl-empresa").val();
        parametros.fi = fi;
        parametros.ff = ff;
        ShowLightBox(true, "Espere porfavor");
        ejecutaAjax("ProgramacionDePagos.aspx/TraerPagosProgramados", 'POST', parametros, function (d) {
            ShowLightBox(false, "");
            if (d.d.EsValido) {
                if (tipo == 2)
                    ProcesarPagosAsignacion(d.d.Datos);
                else
                    ProcesarProgramacionPagos(d.d.Datos);
            } else {
                $('#AyudaProveedores_HelpLoad').css({ "display": "none" });
                showMsg('Alerta', d.d.Mensaje, 'warning');
            }
        }, function (d) {
            showMsg('Alerta', d.responseText, 'error');
        }, true);
    };

    var CargaTodosPagosProgramados = function (tipo) {
        var parametros = {};
        parametros.empresaid = $("#ddl-empresa").val();
        ShowLightBox(true, "Espere porfavor");
        ejecutaAjax("ProgramacionDePagos.aspx/TraerTodosPagosProgramados", 'POST', parametros, function (d) {
            ShowLightBox(false, "");
            if (d.d.EsValido) {
                if (tipo == 2)
                    ProcesarPagosAsignacion(d.d.Datos);
                else
                    ProcesarProgramacionPagos(d.d.Datos);
            } else {
                $('#AyudaProveedores_HelpLoad').css({ "display": "none" });
                showMsg('Alerta', d.d.Mensaje, 'warning');
            }
        }, function (d) {
            showMsg('Alerta', d.responseText, 'error');
        }, true);
    };

    $('body').on('click', '.ckAsignar', null, function () {
        var id = $(this.parentNode.parentNode).attr('data-PagoID');

        $("#tablaPagosAsignar tbody tr").each(function () {
            var tr = $(this);
            if (tr.attr('data-PagoID') == id) {
                $("#hnProgramacionPagoID").val(id);
                tr.find(".ckAsignar").attr('checked', 'checked')
            }
            else {
                tr.find(".ckAsignar").removeAttr('checked')
            }
        });
    });

    var ValidaGuardar = function (tipo) {
        var res = true;
        if (tipo == 1)
        {
            //if ($("#AyudaCuenta_Code").val() == "")
            //{ showMsg('Alerta', 'Favor seleccionar una cuenta', 'warning'); res = false; }
            
            if ($("#AyudaProveedores_Code").val() == "")
            { showMsg('Alerta', 'Favor seleccionar un proveedor', 'warning'); res = false; }
            else if ($("#txtFechaPago").val() == "")
            { showMsg('Alerta', 'Favor de indicar una fecha de pago', 'warning'); res = false; }
            else if ($("#txtConcepto").val() == "")
            { showMsg('Alerta', 'Favor de indicar un concepto', 'warning'); res = false; }
            else if ($("#txtImporte").val() == "")
            { showMsg('Alerta', 'Favor de indicar el importe', 'warning'); res = false; }
            else if ($("#ddlSolicitantes").val() == "0")
            { showMsg('Alerta', 'Favor de seleccionar un solicitante', 'warning'); res = false; }
        }
        else
        {
            if ($("#txtFechaPoliza").val() == "")
            { showMsg('Alerta', 'Favor de indicar una fecha de poliza', 'warning'); res = false; }
            else if ($("#AyudaPoliza_Code").val() == "")
            { showMsg('Alerta', 'Favor de indicar un numero de poliza', 'warning'); res = false; }
            else
            {
                var selecc = 0;

                $("#tablaPagosAsignar tbody tr").each(function () {
                    var tr = $(this);   
                    if (tr.find(".ckAsignar").is(":checked")) {
                        selecc = 1;
                    }
                });

                if (selecc == 0)
                { showMsg('Alerta', 'Favor de seleccionar un pago programado', 'warning'); res = false; }
            }
        }
        
        return res;
    };


    var Limpiar = function (tipo) {
        if (tipo == 1)
        {
            $("#hnProgramacionPagoID").val("");
            $("#AyudaCuenta").clearHelpField();
            $("#AyudaProveedores").clearHelpField();
            $("#txtFechaPago").val("");
            $("#txtConcepto").val("");
            $("#txtImporte").val("");
            $("#ddlSolicitantes").val(0);
        }
        else
        {
            $("#hnProgramacionPagoID").val("");
            $("#txtFechaPoliza").val("");
            $("#AyudaPoliza").clearHelpField();
        }
    };






    var iniciaAyudas = function () {
        $('#AyudaPoliza').helpField({
            title: 'Póliza',
            //codeNumeric: true,
            maxlengthCode: 9,
            enableCode: true,
            enableDescription: false,
            codePaste: false,
            //nextControlID: '',
            //widthDescription: '0',
            requeridField: false,
            cultureResourcesPopUp: {
                popUpHeader: 'Búsqueda de Pólizas'
            },
            findByPopUp: ayudaPoliza_FindByPopUp,
            findByCode: ayudaPoliza_FindByCode,
            onElementFound: ayudaPoliza_onElementFound,
            camposSalida: [{ header: 'Tipo', value: 'Tipo', code: false, description: false }, { header: 'Folio', value: 'Folio', code: true, description: false }, { header: 'Fecha', value: 'Fecha', code: false, description: false }, { header: 'Descripción', value: 'Descripcion', code: false, description: true }]
        });

        $('#AyudaProveedores').helpField({
            title: 'Proveedores',
            codeNumeric: false,
            maxlengthCode: 30,
            enableCode: true,
            enableDescription: false,
            codePaste: false,
            nextControlID: 'txtFechaPago',
            widthCode: 170,
            //widthDescription: '0',
            requeridField: false,
            cultureResourcesPopUp: {
                popUpHeader: 'Búsqueda de Proveedores'
            },
            findByPopUp: AyudaProveedores_FindByPopUp,
            findByCode: AyudaProveedores_FindByCode,
            onElementFound: AyudaProveedores_onElementFound
        });

        $('#AyudaCuenta').helpField({
            title: 'Cuenta',
            codeNumeric: false,
            maxlengthCode: 30,
            enableCode: true,
            enableDescription: false,
            codePaste: false,
            nextControlID: 'AyudaProveedores',
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

    var llenarComboEmpresas = function (d) {
        if (d.d.EsValido) {
            $('#ddl-empresa').llenaCombo(d.d.Datos);
        }
    };

    var llenaComboTipPol = function () {
        escondeMsg();
        $({}).ToRequest({
            url: 'CapturaPolizas.aspx/TraerTipPol',
            success: function (d) {
                if (d.d.EsValido) {
                    var tippol = d.d.Datos.reverse(),
                    i = tippol.length,
                    opt = '';
                    for (i; i--;) {
                        opt += '<option value="' + tippol[i].id + '">' + tippol[i].nombre + '</option>';
                    }
                    $('#ddlTipPol').html(opt);
                } else {
                    $('#ddlTipPol').html('');
                    showMsg('Alerta', d.d.Mensaje, 'warning');
                }
            },
            failure: function (d) {
                showMsg('Error', d.responseText, 'error');
            },
            Titulo: 'Espere Por Favor',
            ConvertirObjetoJson: false
        });
    };

    $(document).ready(function () {
        $(".recuadro").hide();
        $(".CuadroFacturas").hide();
        if (amplify.store.sessionStorage("UsuarioID") == null || amplify.store.sessionStorage("UsuarioID") == "") {
            window.location.replace("../../login.aspx");
        }
        $("#Menu").MostrarMenuBar(JSON.parse(amplify.store.sessionStorage("Funcionalidades")), amplify.store.sessionStorage("Nombre"), amplify.store.sessionStorage("NombreEmpresa"), amplify.store.sessionStorage("UsuarioID"));
        $('.popup').setDraggable('.popup-body, .popup-footer, .close');
        ActualizaMenuBar();
        iniciaAyudas();
        AplicarEstilo();
        TraerSolicitantes();
        ejecutaAjax('BonoProductividad.aspx/TraerEmpresas', 'POST', null, llenarComboEmpresas, null, false);
        $("#ddl-empresa").val(getEmpresaid());

        if (getUsuario() == "Isabel" || getUsuario().toUpperCase() == "GILDA") {
            llenaComboTipPol();
            $("#capturaPago").css("display", "none")
            $("#AsignarPoliza").css("display", "inline")
            $("#txtTipo").val("2");
            CargaTodosPagosProgramados(2);
            $("#txtFechaPoliza").focus();
            $("#divEmpresa").css("display", "inline");
        }
        else {
            $("#capturaPago").css("display", "inline")
            $("#AsignarPoliza").css("display", "none")
            $("#txtTipo").val("1");
            CargaTodosPagosProgramados(1);
            $("#AyudaProveedores_Code").focus();
            $("#divEmpresa").css("display", "none");
        }
    });
}(jQuery, window.balor, window.amplify));