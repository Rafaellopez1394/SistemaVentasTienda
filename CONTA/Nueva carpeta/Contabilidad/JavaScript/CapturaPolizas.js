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
        if ($("#HiddenNuevo").val() != "1") {
            var parametros = { folio: $('#AyudaPoliza').getValuesByCode().Codigo, tippol: $('#ddlTipPol').val(), EmpresaId: amplify.store.sessionStorage("EmpresaID"), fechapol: $('#txtFechaPol').val() };
            escondeMsg();
            $(parametros).ToRequest({
                url: 'CapturaPolizas.aspx/AyudaPoliza_FindByCode',
                success: function (d) {
                    if (d.d.EsValido) {
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
        } else {
            $('#AyudaPoliza_HelpLoad').css({ "display": "none" });
        }
    };

    var ayudaPoliza_FindByPopUp = function () {
        var parametros = { descripcion: $('#AyudaPoliza').getValuesByPopUp().Descripcion, tippol: $('#ddlTipPol').val(), EmpresaId: amplify.store.sessionStorage("EmpresaID"), fechapol: $('#txtFechaPol').val(), Pendiente: $('#chkPendiente').is(':checked') };
        escondeMsg();
        $(parametros).ToRequest({
            url: 'CapturaPolizas.aspx/AyudaPoliza_FindByPopUp',
            success: function (d) {
                if (d.d.EsValido) {
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

    var ayudaPoliza_onElementFound = function () {
        var parametros = { polizaid: $('#AyudaPoliza').getValuesByCode().ID };
        escondeMsg();

        ejecutaAjax('CapturaPolizas.aspx/ConsultarPoliza', 'POST', parametros, function (d) {
            if (d.d.EsValido) {
                muestraPoliza(d.d.Datos);
                mostrararchivosPoliza(d.d.Datos);
                $('#btnNuevo, #AyudaPoliza_Code, #ddlTipPol, #txtFechaPol').attr('disabled', 'disabled');
                if ($('#HiddenEstatus').val() === '2') {
                    $('#lblcancelado').removeClass('display_none');
                    $('#btnGuardar,#btnCancelar,#btnImprimir').attr('disabled', 'disabled');
                } else {
                    $("#lblCambiarPoliza,#lblCambiarFecha").show();
                    $('#btnGuardar,#btnCancelar,#btnImprimir').removeAttr('disabled');
                    $('#lblcancelado').addClass('display_none');
                }

                if ($('#ddlTipPol').val() == "IF") {
                    if ($("#chkXML").is(":checked")) {
                        CargaDatosFacturaPoliza();
                    }
                    else {
                        $('#chkXML').attr('checked', true)
                        CargaDatosFacturaPoliza();
                    }
                }
                habilitaBotonCopiaPoliza();
                habilitaBtnCopiaPoliza();

            } else {
                showMsg('Error', d.d.Mensaje, 'error');
            }
        }, function (d) {
            showMsg('Alerta', d.responseText, 'error');
        }, false);


        ejecutaAjax('CapturaPolizas.aspx/ConsultaPagoProgramado', 'POST', parametros, function (d) {
            if (d.d.EsValido) {
                if (d.d.Datos != null)
                    $('#chkPagoProgramado').attr('disabled', 'disabled')
                else
                    $('#chkPagoProgramado').removeAttr('disabled')
            } else {
                showMsg('Error', d.d.Mensaje, 'error');
            }
        }, function (d) {
            showMsg('Alerta', d.responseText, 'error');
        }, false);

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




    //var AyudaCuentaTabla_FindByCode = function () {
    //    var parametros = $('#AyudaCuentaTabla').getValuesByCode(),
    //    codigo = llenarSubcuentas(parametros.Codigo).replace(/-/g, '');
    //    escondeMsg();
    //    parametros.Codigo = padRight(codigo, 24);
    //    parametros.ID = amplify.store.sessionStorage("EmpresaID");
    //    $(parametros).ToRequest({
    //        url: 'CapturaPolizas.aspx/AyudaCuenta_FindByCode',
    //        success: function (d) {
    //            if (d.d.EsValido) {
    //                $('#AyudaCuentaTabla').showFindByCode(d.d.Datos);
    //            } else {
    //                $('#AyudaCuentaTabla_HelpLoad').css({ "display": "none" });
    //                showMsg('Alerta', d.d.Mensaje, 'warning');
    //            }
    //        },
    //        failure: function (d) {
    //            showMsg('Error', d.responseText, 'error');
    //        },
    //        Titulo: 'Espere Por Favor'
    //    });
    //};

    var AyudaCuentaTabla_FindByPopUp = function () {
        escondeMsg();
        var parametros = $('#AyudaCuentaTabla').getValuesByPopUp();
        parametros.ID = amplify.store.sessionStorage("EmpresaID");
        $(parametros).ToRequest({
            url: 'CapturaPolizas.aspx/AyudaCuenta_FindByPopUp',
            success: function (d) {
                if (d.d.EsValido) {
                    $('#AyudaCuentaTabla').showFindByPopUp(d.d.Datos);
                } else {
                    $('#AyudaCuentaTabla_HelpLoad').css({ "display": "none" });
                    showMsg('Alerta', d.d.Mensaje, 'warning');
                }
            },
            failure: function (d) {
                showMsg('Error', d.responseText, 'error');
            },
            Titulo: 'Espere Por Favor'
        });
    };

    var AyudaCuentaTabla_onElementFound = function () {
        var cadena = $('#AyudaCuentaTabla_Code').val();
        $('#AyudaCuentaTabla_Code').val(formatoCuenta(cadena));
        ActualizaCuenta();
    };
















    var llenaCombo = function () {
        var presupuestoid = { presupuestoid: $('#AyudaPresupuesto').getValuesByCode().ID };
        escondeMsg();
        $(presupuestoid).ToRequest({
            url: 'CapturaPolizas.aspx/TraerConceptos',
            success: function (d) {
                if (d.d.EsValido) {
                    var conceptos = d.d.Datos.reverse(),
            i = conceptos.length,
            opt = [];
                    for (i; i--;) {
                        opt.push('<option value="' + conceptos[i].id + '">' + conceptos[i].nombre + '</option>');
                    }
                    $('#ddlConcepto').html(opt.join(''));
                } else {
                    //$('#AyudaPresupuesto_HelpLoad').css({ "display": "none" });
                    $('#ddlConcepto').html('');
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

    var llenaComboGeneral = function (url, parametros, idcombo, conSeleccionar) {
        parametros = parametros || {};
        escondeMsg();
        $(parametros).ToRequest({
            url: url,
            success: function (d) {
                var opt = conSeleccionar ? '<option value="">Seleccione</option>' : '';
                if (d.d.EsValido) {
                    var lista = d.d.Datos.reverse(),
            i = lista.length;
                    for (i; i--;) {
                        opt += '<option value="' + lista[i].id + '">' + lista[i].nombre + '</option>';
                    }
                } else {
                    showMsg('Alerta', d.d.Mensaje, 'warning');
                }
                $('#' + idcombo).html(opt);
            },
            failure: function (d) {
                showMsg('Error', d.responseText, 'error');
            },
            Titulo: 'Espere Por Favor',
            ConvertirObjetoJson: false
        });
    };


    var iniciaAyudas = function () {
        $('#AyudaPoliza').helpField({
            title: 'Póliza',
            //codeNumeric: true,
            maxlengthCode: 9,
            enableCode: true,
            enableDescription: false,
            codePaste: false,
            nextControlID: 'txtConceptoPoliza',
            widthDescription: '0',
            requeridField: false,
            cultureResourcesPopUp: {
                popUpHeader: 'Búsqueda de Pólizas'
            },
            findByPopUp: ayudaPoliza_FindByPopUp,
            findByCode: ayudaPoliza_FindByCode,
            onElementFound: ayudaPoliza_onElementFound,
            camposSalida: [{ header: 'Tipo', value: 'Tipo', code: false, description: false }, { header: 'Folio', value: 'Folio', code: true, description: false }, { header: 'Fecha', value: 'Fecha', code: false, description: false }, { header: 'Descripción', value: 'Descripcion', code: false, description: true }]
        });

        $('#AyudaCuenta').helpField({
            title: 'Cuenta',
            codeNumeric: false,
            maxlengthCode: 30,
            enableCode: true,
            enableDescription: false,
            codePaste: false,
            nextControlID: 'txtImporte',
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

        $('#AyudaCuentaTabla').helpField({
            title: 'Cuenta',
            codeNumeric: false,
            maxlengthCode: 30,
            enableCode: true,
            enableDescription: false,
            codePaste: false,
            //nextControlID: 'txtImporte',
            widthCode: 170,
            //widthDescription: '0',
            requeridField: false,
            cultureResourcesPopUp: {
                popUpHeader: 'Búsqueda de Cuentas'
            },
            findByPopUp: AyudaCuentaTabla_FindByPopUp,
            findByCode: function () { },
            onElementFound: AyudaCuentaTabla_onElementFound
        });
    };

    var muestraPoliza = function (poliza) {
        $('#HiddenPolizaid').val(poliza.Polizaid);
        $('#txtFechaPol').val(poliza.Fechapol);
        //$('#txtImporte').val(formatMoney(poliza.Importe || 0, 2));
        $('#txtConceptoPoliza').val(poliza.Concepto);
        $('#HiddenUsuario').val(poliza.Usuario);
        $('#HiddenEstatus').val(poliza.Estatus);
        $('#HiddenUltimaAct').val(poliza.UltimaAct);
        $('#chkPendiente').attr('checked', poliza.Pendiente);
        if (poliza.FacturasProveedor.length > 0) {
            $('#chkXML').attr('checked', true);
            $('.recuadro').show("slow");
            ProcesarGridFacturas(poliza.FacturasProveedor);
        }
        if (poliza.ListaModeloRespuestaNomina.length > 0) {
            $('#chkXMLNomina').attr('checked', true);
            $('.recuadro-nomina').show("slow");            
            ProcesarGridNomina(poliza.ListaModeloRespuestaNomina);

        }
        $('#chkPagoProgramado').attr('checked', poliza.Pagoprogramado);

        // Llena detalle
        var len = poliza.ListaPolizaDetalle.length,
        lista = poliza.ListaPolizaDetalle,
        detalleCuentas = '';

        for (var i = 0; i < len; i++) {
            detalleCuentas += rowDetalle(lista[i]);
        }
        if (detalleCuentas) {
            $('#trvacio').remove();
            $('#tablaDetalle tbody').html(detalleCuentas);
            $('#tablaDetalle tbody .numerico').autoNumeric('init', { mDec: 2, vMin: '-999999999.99' });
            calculaTotales();
            //aplicarEstilosGrid('tablaDetalle');
        }
    };

    var obtenerDatosPoliza = function () {
      var detalle = $('#tablaDetalle tbody tr'),
      len = detalle.length,
      poldetalle = [],
      poliza = {
          Polizaid: $('#HiddenPolizaid').val(),
          EmpresaId: amplify.store.sessionStorage("EmpresaID"),
          Folio: $('#AyudaPoliza').getValuesByCode().Codigo || '0',
          TipPol: $('#ddlTipPol').val(),
          Fechapol: $('#txtFechaPol').val(),
          Concepto: $('#txtConceptoPoliza').val(),
          Importe: $('#tg_cargos').text().replace(/,/g, '') || 0,
          Pendiente: $('#chkPendiente').is(':checked'),
          Usuario: getUsuario(), //parent.document.getElementById("OcultoPropiedades").value, // $('#HiddenUsuario').val(),
          Estatus: $('#HiddenEstatus').val() || 1,
          //Fecha:
          UltimaAct: $('#HiddenUltimaAct').val() || 0,
          Pagoprogramado: $('#chkPagoProgramado').is(':checked'),
          Empresabancoid: $("#ddlBancos").val() == "*" ? null : $("#ddlBancos").val()

      };

        for (var i = 0; i < len; i++) {
            var tr = $(detalle[i]),
            cuenta = {
                Polizadetalleid: tr.attr('data-polizadetalleid'),
                Polizaid: '',
                Cuentaid: tr.attr('data-cuentaid'),
                Cuenta: '',
                CuentaDesc: '',
                TipMov: tr.attr('data-tipmov'),
                Concepto: $('.conceptodet', tr).val(),
                Cantidad: 1,
                Importe: $('.importe', tr).val().replace(/,/g, '') || 0,
                Estatus: tr.attr('data-estatus') || 1,
                Fecha: tr.attr('data-fecha'),
                Usuario: getUsuario(), // parent.document.getElementById("OcultoPropiedades").value, // tr.attr('data-usuario'),
                UltimaAct: tr.attr('data-ultimaact') || 0
            };
            poldetalle.push(cuenta);
        }

        poliza.ListaPolizaDetalle = poldetalle;
        if ($("#chkXML").is(":checked")) {
            poliza.FacturasProveedor = GetFacturasProveedor();
        } else {
            poliza.FacturasProveedor = [];
        }

        if ($("#chkXMLNomina").is(":checked")) {
            poliza.ListaPolizasNomina = getListadoNomina();
        } else {
            poliza.ListaPolizasNomina = [];
        }

        return poliza;
    };

    var llenaComboTipPol = function () {
        escondeMsg();

        ejecutaAjax('CapturaPolizas.aspx/TraerTipPol', 'POST', {}, function (d) {
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
        }, function (d) {
            showMsg('Alerta', d.responseText, 'error');
        }, false);


        //$({}).ToRequest({
        //    url: 'CapturaPolizas.aspx/TraerTipPol',
        //    success: function (d) {
        //        if (d.d.EsValido) {
        //            var tippol = d.d.Datos.reverse(),
        //            i = tippol.length,
        //            opt = '';
        //            for (i; i--;) {
        //                opt += '<option value="' + tippol[i].id + '">' + tippol[i].nombre + '</option>';
        //            }
        //            $('#ddlTipPol').html(opt);
        //            habilitaBanco();
        //        } else {
        //            $('#ddlTipPol').html('');
        //            showMsg('Alerta', d.d.Mensaje, 'warning');
        //        }
        //    },
        //    failure: function (d) {
        //        showMsg('Error', d.responseText, 'error');
        //    },
        //    Titulo: 'Espere Por Favor',
        //    ConvertirObjetoJson: false
        //});
    };

    var existeCuenta = function (numcuenta) {
        return $('#' + numcuenta).length > 0;
    };

    var agregarCuenta = function () {

        var iCargo = ($('#txtImporte').val().replace(/,/g, '') * 1);
        var iAbono = ($('#txtImporteAbono').val().replace(/,/g, '') * 1);

        //if ((iCargo > 0 && iAbono > 0) || (iCargo == 0 && iAbono == 0)) {
        if ((iCargo > 0 && iAbono > 0)) {
            alert("Revisa los importes del cargo y el abono");
            $("#txtImporteAbono").focus();
            return;
        }
        var tipmov = "1";
        if (iAbono > 0)
            tipmov = "2";
        var ayuda = $('#AyudaCuenta'),
      cuenta = $('#AyudaCuenta').getValuesByCode();

        if (cuenta.ID) {
            //if (!existeCuenta(cuenta.Codigo)) {
            var detalle = {
                Polizadetalleid: '',
                Polizaid: '',
                Cuentaid: cuenta.ID,
                Cuenta: cuenta.Codigo,
                CuentaDesc: cuenta.Descripcion,
                TipMov: tipmov,
                //Concepto: cuenta.Descripcion,
                Concepto: $("#txtConceptoPoliza").val(),
                Cantidad: 1,
                Importe: (iCargo > 0 ? iCargo : iAbono),
                Estatus: 1,
                Fecha: '',
                Usuario: getUsuario(), //parent.document.getElementById("OcultoPropiedades").value,
                UltimaAct: 0
            },
        tr = rowDetalle(detalle);

            $('#trvacio').remove();
            $(tr).appendTo('#tablaDetalle tbody');
            $('#tablaDetalle tbody .numerico').autoNumeric('init', { mDec: 2, vMin: '-999999999.99' });
            calculaTotales();
            aplicarEstilosGrid('tablaDetalle');

            $('#txtImporte,#txtImporteAbono').val('');
            $('#rdocargo').attr('checked', true);
            ayuda.clearHelpField();
            ayuda.setFocusHelp();
            //}
        } else {
            showMsg('Alerta', "Ingresa el codigo de la cuenta contable", 'warning');
            ayuda.setFocusHelp();
        }
    };

    var rowDetalle = function (detalle) {

        var nrow = parseInt($("#hiddenNumeroRow").val()) + 1;

        var inputImporte = '<input type="text" class="numerico hmx_txt_Derecha importe" onfocus="this.select()" value="' + detalle.Importe + '" />',
        inputConcepto = '<input type="text" class="hmx_txt_Normal conceptodet" onfocus="this.select()" value="' + detalle.Concepto + '" />',
        data = ' data-nrow="' + nrow + '" data-polizadetalleid="' + detalle.Polizadetalleid + '" data-cuentaid="' + detalle.Cuentaid + '" data-tipmov="' + detalle.TipMov + '" data-estatus="' + detalle.Estatus + '" data-fecha="' + detalle.Fecha + '" data-usuario="' + detalle.Usuario + '" data-ultimaact="' + detalle.UltimaAct + '"  data-presupuestodetalleid="' + (detalle.PresupuestodetalleId || '') + '" data-inventariocostoid="' + (detalle.Inventariocostoid || '') + '" ',
        tr, tdcargo, tdabono;

        if (detalle.TipMov === '1') {
            tdcargo = '<td class="cargo">' + inputImporte + '</td>';
            tdabono = '<td class="abono"></td>';
        } else {
            tdcargo = '<td class="cargo"></td>';
            tdabono = '<td class="abono">' + inputImporte + '</td>';
        }
        tr = '<tr id="' + detalle.Cuenta + '" ' + data + '><td class="tdcuenta">' + formatoCuenta(detalle.Cuenta) + ' ' + detalle.CuentaDesc + '</td><td class="tdconcepto">' + inputConcepto + '</td>' + tdcargo + tdabono + '<td class="tdeliminar"><img src="../../Base/img/delete.gif" class="imgeliminar" class="imgeliminar"/></td></tr>';

        $("#hiddenNumeroRow").val(nrow);

        return tr;
    };

    var validaDetalleVacio = function () {
        var trvacio = '<tr id="trvacio" class="detalle"><td class="tdcuenta"></td><td class="tdconcepto"></td><td class="cargo"><input type="text" class="numerico hmx_txt_Derecha" value="0.00" /></td><td class="abono"><input type="text" class="numerico hmx_txt_Derecha" value="0.00" /></td><td class="tdeliminar"></td></tr>';
        var detalle = $('#tablaDetalle tbody tr');
        if (detalle.length <= 0) {
            $('#tablaDetalle tbody').html(trvacio);
        }
    };

    var tieneDetalle = function () {
        var detalle = $('#tablaDetalle tbody tr').not('#trvacio');
        if (detalle.length <= 0) {
            return false;
        }
        return true;
    };

    var aplicarEstilosGrid = function (id) {
        var index = 0;
        $('#' + id + ' tbody tr').removeClass('tr_alternate').each(function () {
            if (index++ % 2) {
                $(this).addClass('odd');
            }
        });
        $(".conceptodet").css("width", "340px");
    };

    var totales = function (tipoMov) {
        var total = 0,
        tipo = tipoMov === 1 ? '.cargo' : '.abono';
        $('#tablaDetalle tbody tr ' + tipo + ' .importe').each(function () {
            var importe = parseFloat($(this).val().replace(/,/g, ''));
            total += isNaN(importe) ? 0 : importe;
        });
        return total;
    };

    var totalCargos = function () {
        return totales(1);
    };

    var totalAbonos = function () {
        return totales(2);
    };

    var calculaTotales = function () {
        $('#tg_cargos').text(formatMoney(totalCargos(), 2));
        $('#tg_abonos').text(formatMoney(totalAbonos(), 2));
        $("#txtDiferencia").val(formatMoney((totalCargos() * 1) - (totalAbonos() * 1), 2));
    };

    var cuentasValidas = function () {
        return $('#tg_cargos').text() === $('#tg_abonos').text();
    };

    var validaGuardar = function () {
        var tippol = $('#ddlTipPol').val();

        var codigo = $('#AyudaPoliza_Code').val();
        if (!codigo || codigo.length <= 0) {
            showMsg('Alerta', 'Favor de capturar el número de póliza.', 'warning');
            $('#AyudaPoliza').setFocusHelp();
            return false;
        }

        if (!$('#txtConceptoPoliza').val()) {
            showMsg('Alerta', 'Favor de capturar el concepto.', 'warning');
            $('#txtConceptoPoliza').focus();
            return false;
        }
        if (!tieneDetalle()) {
            showMsg('Alerta', 'Favor de capturar el detalle de cuentas para la póliza.', 'warning');
            return false;
        }
        if (!cuentasValidas()) {
            showMsg('Alerta', 'Las cuentas deben estar cuadradas.', 'warning');
            return false;
        }


        //Validamos que el folio de la poliza y el tipo y la fecha no existan en el sistema solo polizas nuevas
        var ExistePoliza = false;
        if ($('#HiddenPolizaid').val() == "" && $('#ddlTipPol').val() != "CH") {
            var parametros = {};
            parametros.NumPol = $('#AyudaPoliza_Code').val();
            parametros.TipPol = $('#ddlTipPol').val();
            parametros.FecPol = $('#txtFechaPol').val();
            parametros.EmpresaID = amplify.store.sessionStorage("EmpresaID");
            parametros.Pendiente = $('#chkPendiente').is(':checked');

            ejecutaAjax('CapturaPolizas.aspx/VerificarFolioPoliza', 'POST', parametros, function (d) {
                if (d.d.EsValido) {
                    if (d.d.Datos.Existe) {
                        ExistePoliza = true;
                    }
                } else {
                    showMsg('Alerta', 'No se guardó la póliza. ' + d.d.Mensaje, 'warning');
                }
            }, function (d) {
                showMsg('Alerta', d.responseText, 'error');
            }, false);
        }
        if (ExistePoliza) {
            showMsg('Alerta', 'La poliza que intentas guardar ya existe en el sistema con el mismo folio', 'warning');
            return false;
        }
        return true;
    };
    
    var guardarPoliza = function () {
        var parametros = obtenerDatosPoliza();
        escondeMsg();
        ShowLightBox(true, "Espere porfavor");
        ejecutaAjax('CapturaPolizas.aspx/Guardar', 'POST', { value: JSON.stringify(parametros) }, function (d) {
            ShowLightBox(false, "");
            if (d.d.EsValido) {
                EliminarArchivoAdicional();
                if (d.d.Datos.FacturasProveedor.length > 0) {
                    DocumentosFactura(d.d.Datos);
                }else if(getDatosPoliza().FacturasProveedor.length > 0){
                    DocumentosFactura(getDatosPoliza());
                }
                GuardarArchivosAdicionales(d.d.Datos);
                muestraPoliza(d.d.Datos);
                imprimirPoliza();
                //showMsg('Exito', 'La póliza se guardó correctamente.', 'success');
                //alert('La póliza se guardó correctamente.');
                window.location.replace("CapturaPolizas.aspx");
                //$('#btnGuardar, input[type=text], #btnAgregar').attr('disabled', 'disabled');
                //$('#AyudaCuenta,#AyudaPoliza').EnableHelpField(false).desHabilitaHlpBtn();
                //$('#btnImprimir').removeAttr('disabled');

            } else {
                ShowLightBox(false, "");
                showMsg('Alerta', 'No se guardó la póliza. ' + d.d.Mensaje, 'warning');
            }
        }, function (d) {
            ShowLightBox(false, "");
            showMsg('Alerta', d.responseText, 'error');
        }, true);
    };

    var cancelaPoliza = function () {
        var parametros = obtenerDatosPolizaCancelar();
        escondeMsg();
        $(parametros).ToRequest({
            url: 'CapturaPolizas.aspx/Guardar',
            success: function (d) {
                if (d.d.EsValido) {
                    cancelaFacturasProveedores();
                    cancelaFacturaViaticos();
                    showMsg('Alerta', 'La póliza cancelada correctamente.', 'success');
                    $('#btnGuardar,#btnCancelar, input[type=text], #btnAgregar').attr('disabled', 'disabled');
                    //$('#AyudaCuenta_HelpButton').die('click');
                    //$('#AyudaPoliza_HelpButton').die('click');
                    $('#AyudaCuenta,#AyudaPoliza').EnableHelpField(false).desHabilitaHlpBtn();
                    //$('.imgeliminar').die();
                } else {
                    showMsg('Alerta', 'No se guardó la póliza. ' + d.d.Mensaje, 'warning');
                }
            },
            Titulo: 'Espere por favor',
            EsArreglo: false
        });
    };
    var cancelaFacturasProveedores = function () {
        var Polizaid = $('#HiddenPolizaid').val();
        var EmpresaId = amplify.store.sessionStorage("EmpresaID");
        var parametros = { polizaid: Polizaid, usuario: getUsuario() };
        $(parametros).ToRequest({
            url: 'CapturaPolizas.aspx/EliminarFacturasProveedores',
            success: function (d) {
                if (d.d.EsValido) {

                } else {
                   // showMsg('Alerta', 'No se guardó la póliza. ' + d.d.Mensaje, 'warning');
                }
            },
            Titulo: 'Espere por favor',
            EsArreglo: false
        });
    };
    var cancelaFacturaViaticos = function () {
        
        const params = {
            NumPol: $('#AyudaPoliza').getValuesByCode().Codigo || '0',
            TipPol: $('#ddlTipPol').val(),
            FecPol: $('#txtFechaPol').val(),
        };

        //const url = 'http://localhost:46802/CancelarFacturaViaticos.ashx';
        const url = 'http://192.168.11.237/services/CancelarFacturaViaticos.ashx';

        fetch(url, {
            method: 'POST',
            body: new URLSearchParams(params),
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded',
            },
        })
        .then(response => {
            if (!response.ok) {
                throw new Error('Hubo un problema con la solicitud: ' + response.status);
            }
            return response.text();
        })
        .then(data => {
            console.log('Respuesta del servidor:', data);
        })
        .catch(error => {
            console.error('Error al realizar la solicitud:', error);
        });
    };
    var obtenerDatosPolizaCancelar = function () {
        var poliza = obtenerDatosPoliza();
        poliza.Estatus = 2;
        poliza.FacturasProveedor = [];
        return poliza;
    };

    var imprimirPoliza = function () {
        var parametros = '&polizaid=' + $('#HiddenPolizaid').val();
        parametros += "&Ingles=0"; // + ($("#imprimirIngles").is(':checked') ? "1" : "0");
        window.open('../../Base/Formas/ReporteProduccion.aspx?Formato=0&Nombre=ReportePoliza' + parametros, 'w_PopImprimir', 'toolbar=no,location=no,directories=no,status=no,menubar=no,scrollbars=yes,resizable=yes,width=760,height=560');
    };

    var aplicaEventoAyudaPoliza = function () {
        /*$('#AyudaPoliza_Code').die('keydown').live('keydown', function (e) {            
        });*/
        if ($('#btnNuevo').attr('disabled') === 'disabled') {
            //var code = (e.keyCode ? e.keyCode : e.which);
            //if (code === 13) {
            //    e.preventDefault();
            $('#txtConceptoPoliza').focus();
            //}
        }
    };

    $('body').on('click', '#Asignar-close', null, function () {
        cerrarPopup('Asignar');
    });

    $('body').on('click', '#lblCambiarFecha', null, function () {
        muestraPopup('Asignar');
    });

    $('body').on('click', '#CamNumPol-close', null, function () {
        cerrarPopup('CamNumPol');
    });

    $('body').on('click', '#lblCambiarPoliza', null, function () {
        muestraPopup('CamNumPol');
    });


    $('body').on('click', '#btnGuardaCF', null, function () {
        if (!$("#txtFechaNueva").val()) {
            alert("Ingrese la fecha");
            $("#txtFechaNueva").focus();
            return;
        }
        muestraAdvertencia(function yes() {
            cerrarPopup('confirmBox');
            guardarCF();
        }, function no() {
            cerrarPopup('confirmBox');
        }, "txtFechaNueva");
    });

    var guardarCF = function () {
        ShowLightBoxPopUp(true, "Cambiando fecha", "#Asignar");
        var parametros = {};
        parametros.fecha = $("#txtFechaNueva").val();
        parametros.polizaid = $('#AyudaPoliza').getValuesByCode().ID;
        parametros.usuario = getUsuario();
        ejecutaAjax('CapturaPolizas.aspx/GuardarCambioFechaPoliza', 'POST', parametros, function (d) {
            ShowLightBoxPopUp(false, "", "");
            if (d.d.EsValido) {
                if (d.d.Datos.Guardo) {
                    window.location.replace('CapturaPolizas.aspx');
                } else {
                    alert(d.d.Datos.msg);
                }
            } else {
                showMsg('Error', d.d.Mensaje, 'error');
            }
        }, function (d) {
            ShowLightBoxPopUp(false, "", "");
            showMsg('Alerta', d.responseText, 'error');
        }, true);
    };

    $('body').on('click', '#btnGuardaNumPol', null, function () {
        if (!$("#txtNumPolNuevo").val()) {
            alert("Ingrese el numero de poliza");
            $("#txtNumPolNuevo").focus();
            return;
        }
        ShowLightBoxPopUp(true, "Cambiando numero", "#CamNumPol");
        var parametros = {};
        parametros.numpol = $("#txtNumPolNuevo").val();
        parametros.polizaid = $('#AyudaPoliza').getValuesByCode().ID;
        parametros.usuario = getUsuario();
        ejecutaAjax('CapturaPolizas.aspx/GuardarCambioNumeroPoliza', 'POST', parametros, function (d) {
            ShowLightBoxPopUp(false, "", "");
            if (d.d.EsValido) {
                if (d.d.Datos.Guardo) {
                    window.location.replace('CapturaPolizas.aspx');
                } else {
                    alert(d.d.Datos.msg);
                }
            } else {
                showMsg('Error', d.d.Mensaje, 'error');
            }
        }, function (d) {
            ShowLightBoxPopUp(false, "", "");
            showMsg('Alerta', d.responseText, 'error');
        }, true);
    });


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

    $('body').on('keyup', '.importe', null, function () {
        calculaTotales();
    });

    $('body').on('click', '#chkPendiente', null, function () {
        /*
        var tippol = $('#ddlTipPol').val(),
        ayudapolcode = $('#AyudaPoliza_Code');

        ayudapolcode.val('');
        if (this.checked) {
        ayudapolcode.removeAttr('disabled');
        aplicaEventoAyudaPoliza();
        } else {
        if (tippol !== 'CH' && tippol !== 'EG') {
        ayudapolcode.attr('disabled', 'disabled');
        } else {
        aplicaEventoAyudaPoliza();
        }
        }
        */
        $('#txtFechaPol').focus();
        
    });

    $('body').on('change', '#chkPendiente', null, function () {
      
        habilitaBotonCopiaPoliza();
    });

    $('body').on('click', '#btnNuevo', null, function () {
        var tippol = $('#ddlTipPol').val(),
      ayudapolcode = $('#AyudaPoliza_Code');

        ayudapolcode.val('');
        if (tippol !== 'CH' && tippol !== 'EG' && !$('#chkPendiente').is(':checked')) {
            //ayudapolcode.attr('disabled', 'disabled');
        } else {
            aplicaEventoAyudaPoliza();
        }
        //$('#AyudaPoliza_HelpButton').die('click');
        //$('#AyudaPoliza').EnableHelpField(false).desHabilitaHlpBtn();
        $('#btnGuardar').removeAttr('disabled');
        $('#btnNuevo, #AyudaPoliza_HelpButton, #ddlTipPol').attr('disabled', 'disabled');
        $('#txtFechaPol').focus();
        if ($('#ddlTipPol').val() != "CH") {
            TraerFolioMaximoPorTipoPoliza();
        }
        $("#ddlTipPol").css("background-color", "rgb(235, 235, 228)");
        $("#HiddenNuevo").val("1");
    });

    $('body').on('blur', '#txtFechaPol', null, function (e) {
        var ffecha = $(this).val();
        $(this).val(balor.appliFormatDate(ffecha));

        if (balor.isValidDate($(this).val())) {
            if ($('#ddlTipPol').val() != "CH") {
                TraerFolioMaximoPorTipoPoliza();
            }
            muestraAdvertencia(function yes() {
                cerrarPopup('confirmBox');
                $("#AyudaPoliza_Code").focus();
            }, function no() {
                cerrarPopup('confirmBox');
                $('#txtFechaPol').focus();
            }, "txtFechaPol");

        } else {
            $(this).val("");
        }
    });

    var muestraAdvertencia = function (yesFn, noFn, id) {
        var anioActual = (new Date).getFullYear();
        var text = $("#" + id).val().split("/");
        var anioPoliza = new Date(text[2], (text[1] * 1) - 1, text[0]).getFullYear();
        if (anioActual != anioPoliza) {
            var msj = "La fecha de póliza NO concuerda con el ejercicio actual, ¿Continuar?";
            var confirmBox = $("#confirmBox");
            confirmBox.find(".confirmBox-message").html(msj);
            confirmBox.find(".btn-CYes").click(yesFn);
            confirmBox.find(".btn-CNo").click(noFn);
            muestraPopup('confirmBox');
        } else {
            yesFn();
        }
    };

    //$('body').on('click', '#btn-ok', null, function () {
    //    cerrarPopup('confirmBox');
    //});

    //$('body').on('click', '#btn-Cancelar', null, function () {
    //    cerrarPopup('confirmBox');
    //});

    $('body').on('click', '#btnAgregar', null, function () {
        agregarCuenta();
    });


    $('body').on('keypress', '#txtConceptoPoliza', null, function (e) {
        var code = (e.keyCode ? e.keyCode : e.which);
        if (code === 13) {
            e.preventDefault();
            $('#AyudaCuenta').setFocusHelp();
        }
    });
    /*
    $('body').on('keydown', '#txtImporte', null, function (e) {
    var code = (e.keyCode ? e.keyCode : e.which);
    if (code === 13) {
    $("#txtImporteAbono").focus();
    //var cuenta = $('#AyudaCuenta').getValuesByCode();
    //agregarCuenta();
    }
    });
    */

    $('body').on('keydown', '#ddlTipPol', null, function (e) {
        var code = (e.keyCode ? e.keyCode : e.which);
        if (code === 13) {
            $('#txtFechaPol').focus();
        }
    });
    $('body').on('change', '#ddlTipPol', null, function (e) {
        habilitaBanco();
    });

    var habilitaBanco = function () {
        var tipopoliza = $("#ddlTipPol").val();
        if (tipopoliza == "CH") {
            $("#divBanco").slideDown(500);
        }
        else {
            $("#ddlBancos").val("*");
            $("#divBanco").slideUp(500);
            $("#txtconsecutivo").val("");
            $("#AyudaPoliza_Code").val("");
        }
    }

    $('body').on('keydown', '#ddlConcepto', null, function (e) {
        var code = (e.keyCode ? e.keyCode : e.which);
        if (code === 13) {
            e.preventDefault();
            $('#ddlInvCostos').focus();
        }
    });

    $('body').on('keypress', '#txtFechaPol', null, function (e) {
        focusNextControl(this, e);
    });

    $('body').on('keypress', '#AyudaPoliza_Code', null, function (e) {
        if ($("#HiddenNuevo").val() == "1")
            focusNextControl(this, e);
    });

    $('body').on('keypress', '#txtImporte,#txtImporteAbono', null, function (e) {
        focusNextControl(this, e);
    });

    /*
    $('body').on('keypress', 'input[name=tipomov]', null, function (e) {
    var code = (e.keyCode ? e.keyCode : e.which);
    if (code === 13) {
    e.preventDefault();
    $('#txtImporte').focus();
    }
    });
    */
    $('body').on('keypress', '#tablaDetalle tbody input', null, function (e) {
        focusNextControl(this, e);
    });

    $('body').on('keypress', '.conceptodet', null, function (e) {
        var code = (e.keyCode ? e.keyCode : e.which);
        if (code === 13) {
            if ($(this).val() == "") {
                $(this).val($("#txtConceptoPoliza").val());
            }
        }
    });


    $('body').on('dblclick', '.abono', null, function () {
        if ($(this).find(".importe").length == 0 && $(this.parentNode).find(".cargo").find(".importe").length > 0) {
            var importe = $(this.parentNode).find(".cargo").find(".importe").val();
            $(this.parentNode).find(".cargo").find(".importe").remove();
            $(this).html('<input type="text" class="numerico hmx_txt_Derecha importe" onfocus="this.select()" value="' + importe + '">');
            $(this.parentNode).attr("data-tipmov", "2");
            calculaTotales();
        }
    });

    $('body').on('dblclick', '.cargo', null, function () {
        if ($(this).find(".importe").length == 0 && $(this.parentNode).find(".abono").find(".importe").length > 0) {
            var importe = $(this.parentNode).find(".abono").find(".importe").val();
            $(this.parentNode).find(".abono").find(".importe").remove();
            $(this).html('<input type="text" class="numerico hmx_txt_Derecha importe" onfocus="this.select()" value="' + importe + '">');
            $(this.parentNode).attr("data-tipmov", "1");
            calculaTotales();
        }
    });


    //

    $('body').on('click', '.imgeliminar', null, function () {
        var item = $(this.parentNode.parentNode);
        item.fadeOut(1000, function () {
            item.remove();
            validaDetalleVacio();
            aplicarEstilosGrid('tablaDetalle');
            calculaTotales();
        });
    });

    $('body').on('click', '#btnLimpiar', null, function () {
        window.location.replace('CapturaPolizas.aspx');
    });

    $('body').on('click', '#btnGuardar', null, function () {

        var anioActual = (new Date).getFullYear();
        var text = $("#txtFechaPol").val().split("/");
        var anioPoliza = new Date(text[2], (text[1] * 1) - 1, text[0]).getFullYear();

        if (anioActual != anioPoliza) {
            if (confirm('La fecha de póliza NO concuerda con el ejercicio actual, ¿continuar?')) {
                if (confirm('¿Desea guardar la póliza?')) {
                    if (validaGuardar()) {
                        guardarPoliza();
                    }
                }
            }
            else {
                $('#txtFechaPol').focus();
            }
        }
        else {
            if (confirm('¿Desea guardar la póliza?')) {
                if (validaGuardar()) {
                    guardarPoliza();
                }
            }
        }
    });

    $('body').on('click', '#btnCancelar', null, function () {
        if (confirm('¿Desea cancelar la póliza?')) {
            if (validaGuardar()) {
                cancelaPoliza();
            }
        }
    });

    var TraerFolioMaximoPorTipoPoliza = function () {
        escondeMsg();
        var parametros = { tippol: $('#ddlTipPol').val(), EmpresaId: amplify.store.sessionStorage("EmpresaID"), fechapol: $('#txtFechaPol').val() };
        ejecutaAjax('CapturaPolizas.aspx/TraerFolioMaximoPorTipoPoliza', 'POST', parametros, function (d) {
            if (d.d.EsValido) {
                $("#AyudaPoliza_Code").val(d.d.Datos.Folio);
            } else {
                showMsg('Alerta', d.d.Mensaje, 'warning');
            }
        }, function (d) {
            showMsg('Error', d.responseText, 'error');
        }, true);
    }

    $('body').on('click', '#btnImprimir', null, function () {
        imprimirPoliza();
    });


    $('body').on('change', '#ddlTipPol', null, function () {
        if ($("#chkXML").is(":checked") && $("#ddlTipPol").val() == "IF") {
            $(".CuadroFacturas").show("slow");
            $(".recuadro").hide("slow")
        }
        else if ($("#chkXML").is(":checked") && $("#ddlTipPol").val() != "IF") {
            $(".recuadro").show("slow");
            $(".CuadroFacturas").hide("slow")
        }

    });


    $('body').on('dblclick', '.tdcuenta', null, function () {
        var cuenta = $(this)[0].innerText;
        //$(this).html('<input type="text" class="nuevaCuenta" onfocus="this.select()" value="' + cuenta.substring(0, 29) + '">');
        $(this).html('<input type="text" class="nuevaCuenta" onfocus="this.select()" value="">');
        $(".nuevaCuenta").focus();
        $('#hiddenRowAfectedDescription').val(cuenta);

        //$('#AyudaCuentaTabla_HelpButton').click();
    });

    $('body').on('blur', '.nuevaCuenta', null, function () {
        var cuenta = $(this).val();
        var td = $(this.parentNode);
        var tr = $(this.parentNode.parentNode);

        if (!$(this).val()) {
            $(this).remove();
            td.html($('#hiddenRowAfectedDescription').val());
            return;
        }

        var codigo = llenarSubcuentas(cuenta).replace(/-/g, '');
        codigo = padRight(codigo, 24);
        cuenta = formatoCuenta(codigo);

        var parametros = {}
        parametros.Codigo = codigo;
        parametros.ID = amplify.store.sessionStorage("EmpresaID");
        ejecutaAjax('CapturaPolizas.aspx/AyudaCuenta_FindByCode', 'POST', { value: JSON.stringify(parametros) }, function (d) {
            if (d.d.EsValido) {
                tr.attr("id", codigo);
                tr.attr("data-cuentaid", d.d.Datos[0].ID);
                cuenta = cuenta + ' ' + d.d.Datos[0].Descripcion;

                $(this).remove();
                td.html(cuenta);

            } else {
                showMsg('Alerta', d.d.Mensaje, 'warning');
                $(this).remove();
                td.html($('#hiddenRowAfectedDescription').val());
            }
        }, function (d) {
            showMsg('Alerta', d.responseText, 'error');
        }, false);
    });

    $('body').on('keypress', '.nuevaCuenta', null, function (e) {
        var code = (e.keyCode ? e.keyCode : e.which);
        var cuenta = $(this).val();
        var td = $(this.parentNode);


        if (code === 13) {
            if (!$(this).val()) {
                alert("debe ingresar una cuenta");
                return;
            }
            //var codigo = llenarSubcuentas(cuenta).replace(/-/g, '');
            //codigo = padRight(codigo, 24);
            //cuenta = formatoCuenta(codigo);


            //$(this).remove();
            //td.html(cuenta);
        }
        else {
            var input = $(this),
                strCuenta = cuenta,
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
        }
    });

    $('body').on('keydown', '.nuevaCuenta', null, function (e) {
        var code = e.which;
        if (code === 118) {
            var cuenta = $(this).val();
            var td = $(this.parentNode);
            var tr = $(this.parentNode.parentNode);
            $('#hiddenRowAfectedID').val(tr.attr("data-nrow"));
            td.html(cuenta);
            $('#AyudaCuentaTabla_HelpButton').click();
        }
    });

    var ActualizaCuenta = function () {
        $("#tablaDetalle tbody tr").each(function () {
            if ($(this).attr("data-nrow") == $("#hiddenRowAfectedID").val()) {
                var ayuda = $("#AyudaCuentaTabla").getValuesByCode();
                //$(this).attr("data-polizadetalleid", ayuda.ID);
                $(this).attr("data-cuentaid", ayuda.ID);
                $(this).attr("id", ayuda.Codigo.replace(/-/g, ''));
                $(this).find(".tdcuenta")[0].innerText = ayuda.Codigo + ' ' + ayuda.Descripcion;
            }
        });
    };

    //****************CODIGO PANTALLA DE POLIZAS TERMINA
    /*
      ==================================================
      REGION FACTURAS
      ==================================================
    */
    var ProcesarGridFacturas = function (Datos) {
        var resHtml = '';
        var importetotal = 0;
        var ivaacum = 0;
        for (var i = 0; i < Datos.length; i++) {
            resHtml += '<tr class="rgFactura ' + Datos[i].Factura + '" data-Factura="' + Datos[i].Factura + '" data-Uuid="' + Datos[i].Uuid + '" data-file="' + Datos[i].NomArchivo + '" data-Facturaproveedorid="' + Datos[i].Facturaproveedorid + '" data-UltimaAct="' + Datos[i].UltimaAct + '" data-Estatus="' + Datos[i].Estatus + '" data-Proveedorid="' + Datos[i].Proveedorid + '">';
            resHtml += '<td >' + Datos[i].Factura + '</td>';
            resHtml += '<td>' + formatoFechaJSON(Datos[i].Fechatimbrado) + '</td>';
            resHtml += '<td class="CSubtotal" style="text-align: right;">' + balor.aplicarFormatoMoneda(Datos[i].Subtotal, "$") + '</td>';
            resHtml += '<td class="CIva" style="text-align: right;">' + balor.aplicarFormatoMoneda(Datos[i].Iva, "$") + '</td>';
            resHtml += '<td class="CRetIva" style="text-align: right;">' + balor.aplicarFormatoMoneda(Datos[i].IvaRet, "$") + '</td>';
            resHtml += '<td class="CRetIsr" style="text-align: right;">' + balor.aplicarFormatoMoneda(Datos[i].IsrRet, "$") + '</td>';
            resHtml += '<td class="CTotal" style="text-align: right;">' + balor.aplicarFormatoMoneda(Datos[i].Total, "$") + '</td>';
            if (Datos[i].RutaXml !== undefined && Datos[i].RutaXml !== null) {
                var pdfPath = Datos[i].RutaXml;
                resHtml += '<td class=" centrado"><div class="icon-check xml" data-path="' + pdfPath + '"></div></td>';
                $("#btndescargarTodo").removeAttr("style");
            } else {
                resHtml += '<td class="centrado" ><div class="fileUploadXml btnExcel "data-Name="" data-Uuid="' + Datos[i].Uuid + '" data-Facturaproveedorid="' + Datos[i].Facturaproveedorid + '" data-Proveedorid="' + Datos[i].Proveedorid + '"  data-UltimaAct="' + Datos[i].UltimaAct + '" style="position: relative; overflow: hidden; margin-top: 7px;"><span>Cargar XML</span><input type="file" class="uploadxml" style="position: absolute; top: 0; right: 0; margin: 0; padding: 0; font-size: 20px; cursor: pointer; opacity: 0;" multiple=""/></div></td>';
            }
            if (Datos[i].RutaPdfCfdi !== undefined && Datos[i].RutaPdfCfdi !== null) {
                var pdfPath = Datos[i].RutaPdfCfdi;
                resHtml += '<td class=" centrado"><div class="icon-check pdfcfdi" data-path="' + pdfPath + '"></div></td>';
                $("#btndescargarTodo").removeAttr("style");
            } else {
                resHtml += '<td class="centrado" ><div class="fileUploadCfdi btnExcel" "data-Name="" data-Uuid="' + Datos[i].Uuid + '" data-Facturaproveedorid="' + Datos[i].Facturaproveedorid + '" data-Proveedorid="' + Datos[i].Proveedorid + '"  data-UltimaAct="' + Datos[i].UltimaAct + '" style="position: relative; overflow: hidden; margin-top: 7px;"><span>Cargar PDF</span><input type="file" class="upload" style="position: absolute; top: 0; right: 0; margin: 0; padding: 0; font-size: 20px; cursor: pointer; opacity: 0;" multiple=""/></div></td>';
            }

            if (Datos[i].RutaPdfValidacion !== undefined && Datos[i].RutaPdfValidacion !== null) {
                var pdfPath = Datos[i].RutaPdfValidacion;
                resHtml += '<td class=" centrado"><div class="icon-check pdfvalidacion" data-path="' + pdfPath + '"></div></td>';
                $("#btndescargarTodo").removeAttr("style");
            } else {
                resHtml += '<td class="centrado" ><div class="fileUploadValidacion btnExcel" "data-Name="" data-Uuid="' + Datos[i].UUID + '" data-Facturaproveedorid="' + Datos[i].Facturaproveedorid + '" data-Proveedorid="' + Datos[i].Proveedorid + '" data-UltimaAct="' + Datos[i].UltimaAct + '" style="position:relative;overflow:hidden;margin-top:7px;"> <span>Cargar PDF</span> <input type="file" class="upload" style="position: absolute; top: 0; right: 0; margin: 0; padding: 0; font-size: 20px; cursor: pointer; opacity: 0;" multiple=""/></div></td>';
            }
            
            resHtml += '<td class="tdeliminar centrado"><span class="closebtn elim-Factura" >X</span></td>';
            resHtml += '</tr>';
            importetotal += Datos[i].Subtotal;
            ivaacum += Datos[i].Iva;

        }
        $("#TablaFacturas tbody").append(resHtml);
        
        calcularTotales();

        $("#TablaFacturas tbody").on('change', '.fileUploadXml input', function () {
            var fileUploadXml = $(this).closest('.fileUploadXml');
            validateFileType(fileUploadXml, this.files, { xml: [".xml"] }, "xml");
        });

        $("#TablaFacturas tbody").on('change', '.fileUploadCfdi input', function () {
            var fileUploadcfdi = $(this).closest('.fileUploadCfdi');
            validateFileType(fileUploadcfdi, this.files, { cfdi: [".pdf"] }, "cfdi");
        });

        $("#TablaFacturas tbody").on('change', '.fileUploadValidacion input', function () {
            var fileUploadValidacion = $(this).closest('.fileUploadValidacion');
            validateFileType(fileUploadValidacion, this.files, { cfdi: [".pdf"] }, "cfdi");
        });

        $(".xml").on("click", function (event) {
            event.preventDefault();
            var xmlPath = $(this).data("path");
            prueb(xmlPath);
        });

        $(".pdfcfdi").on("click", function (event) {
            event.preventDefault();
            var pdfPath = $(this).data("path");
            console.log("pdfPath:", pdfPath);
            prueb(pdfPath);
        });

        $(".pdfvalidacion").on("click", function (event) {
            event.preventDefault();
            var pdfPath = $(this).data("path");
            console.log("pdfPath:", pdfPath);
            prueb(pdfPath);
        });

        /*function prueb(Path) {
            //var servicioBaseURL = "http://localhost:46802/";
            var servicioBaseURL = "http://192.168.11.237/services/";
            var rutaDocumento = Path;
            var ext = Path.split('.').pop().toLowerCase();

            if (ext === "pdf") {
                nombreArchivo = rutaDocumento.match(/([^\\]+\.pdf)$/i)[1];
                rutaDocumento = rutaDocumento.replace(/\\[^\\]+\.pdf$/i, "");
            } else if (ext === "xml") {
                nombreArchivo = rutaDocumento.match(/([^\\]+\.xml)$/i)[1];
                rutaDocumento = rutaDocumento.replace(/\\[^\\]+\.xml$/i, "");
            }

            //window.location.href = servicioBaseURL + "DescargarArchivos.ashx?rutaDocumento=" + encodeURIComponent(rutaDocumento) + "&nombreArchivo=" + encodeURIComponent(nombreArchivo);
            var url = servicioBaseURL + "DescargarArchivos.ashx?rutaDocumento=" + encodeURIComponent(rutaDocumento) + "&nombreArchivo=" + encodeURIComponent(nombreArchivo);

            console.log(url); // Agregar para ver la URL generada

            fetch(url)
                .then(response => {
                    if (!response.ok) {
                        throw new Error('Network response was not ok ' + response.statusText);
                    }
                    return response.text(); // Cambiar a .text() temporalmente para ver la respuesta
                })
                .then(data => console.log(data))
                .catch(error => {
                    console.error('There has been a problem with your fetch operation:', error);
                });
        };*/
        function prueb(Path) {
            //var servicioBaseURL = "http://localhost:46802/";
            var servicioBaseURL = "http://192.168.11.237/services/";
            var rutaDocumento = Path;
            var ext = Path.split('.').pop().toLowerCase();
            var nombreArchivo;

            if (ext === "pdf") {
                nombreArchivo = rutaDocumento.match(/([^\\]+\.pdf)$/i)[1];
                rutaDocumento = rutaDocumento.replace(/\\[^\\]+\.pdf$/i, "");
            } else if (ext === "xml") {
                nombreArchivo = rutaDocumento.match(/([^\\]+\.xml)$/i)[1];
                rutaDocumento = rutaDocumento.replace(/\\[^\\]+\.xml$/i, "");
            }

            var url = servicioBaseURL + "DescargarArchivos.ashx?rutaDocumento=" + encodeURIComponent(rutaDocumento) + "&nombreArchivo=" + encodeURIComponent(nombreArchivo);

            fetch(url)
                .then(response => {
                    if (!response.ok) {
                        throw new Error('Error al descargar el archivo.');
                    }
                    return response.blob();
                })
                .then(blob => {
                    const urlBlob = window.URL.createObjectURL(blob);
                    const a = document.createElement('a');
                    a.href = urlBlob;
                    a.download = nombreArchivo;
                    document.body.appendChild(a);
                    a.click();
                    document.body.removeChild(a);
                })
                .catch(error => {
                    console.error('Error:', error);
                });
        }

    }
    
    var calcularTotales = function () {
        var totalSubtotal = 0;
        var totalIva = 0;
        var totalRetIva = 0;
        var totalRetIsr = 0;
        var totalTotal = 0;

        var nomina_sueldo = 0;
        var nomina_premioasistencia = 0;
        var nomina_premiopuntualidad = 0;
        var nomina_vacaciones = 0;
        var nomina_primavacacional = 0;
        var nomina_aguinaldo = 0;
        var nomina_gastosmedicosmayores = 0;
        var nomina_segurodevida = 0;
        var nomina_indemnizacion = 0;
        var nomina_primadeantiguedad = 0;
        var nomina_ptu = 0;
        var nomina_totalpercepciones = 0;
        var nomina_subsidioalempleo = 0;
        var nomina_isrretenido = 0;
        var nomina_imss = 0;
        var nomina_infonavit = 0;
        var nomina_fonacot = 0;
        var nomina_primaspagadas = 0;
        var nomina_isrart174 = 0;
        var nomina_prestamoinfonavitcf = 0;
        var nomina_totaldeducciones = 0;
        var nomina_total = 0;

        $("#TablaFacturas tbody tr").each(function () {
            totalSubtotal += balor.quitarFormatoMoneda($(this).find('.CSubtotal').text());
            totalIva += balor.quitarFormatoMoneda($(this).find('.CIva').text());
            totalRetIva += balor.quitarFormatoMoneda($(this).find('.CRetIva').text());
            totalRetIsr += balor.quitarFormatoMoneda($(this).find('.CRetIsr').text());
            totalTotal += balor.quitarFormatoMoneda($(this).find('.CTotal').text());
        });
        // Actualizar los valores de las celdas de totales
        $('.total-subtotal').text(balor.aplicarFormatoMoneda(totalSubtotal, "$"));
        $('.total-iva').text(balor.aplicarFormatoMoneda(totalIva, "$"));
        $('.total-ret-iva').text(balor.aplicarFormatoMoneda(totalRetIva, "$"));
        $('.total-ret-isr').text(balor.aplicarFormatoMoneda(totalRetIsr, "$"));
        $('.total-total').text(balor.aplicarFormatoMoneda(totalTotal, "$"));


        //XML de Nómina
        $("#TablaFacturasNomina tbody tr").each(function () {
            nomina_sueldo += balor.quitarFormatoMoneda($(this).find('.sueldo').text());
            nomina_premioasistencia += balor.quitarFormatoMoneda($(this).find('.premioasistencia').text());
            nomina_premiopuntualidad += balor.quitarFormatoMoneda($(this).find('.premiopuntualidad').text());
            nomina_vacaciones += balor.quitarFormatoMoneda($(this).find('.vacaciones').text());
            nomina_primavacacional += balor.quitarFormatoMoneda($(this).find('.primavacacional').text());
            nomina_aguinaldo += balor.quitarFormatoMoneda($(this).find('.aguinaldo').text());
            nomina_gastosmedicosmayores += balor.quitarFormatoMoneda($(this).find('.gastosmedicosmayores').text());
            nomina_segurodevida += balor.quitarFormatoMoneda($(this).find('.segurodevida').text());
            nomina_indemnizacion += balor.quitarFormatoMoneda($(this).find('.indemnizacion').text());
            nomina_primadeantiguedad += balor.quitarFormatoMoneda($(this).find('.primadeantiguedad').text());
            nomina_ptu += balor.quitarFormatoMoneda($(this).find('.ptu').text());
            nomina_totalpercepciones += balor.quitarFormatoMoneda($(this).find('.totalpercepciones').text());
            nomina_subsidioalempleo += balor.quitarFormatoMoneda($(this).find('.subsidioalempleo').text());
            nomina_isrretenido += balor.quitarFormatoMoneda($(this).find('.isrmensual').text());
            nomina_imss += balor.quitarFormatoMoneda($(this).find('.imss').text());
            nomina_infonavit += balor.quitarFormatoMoneda($(this).find('.infonavit').text());
            nomina_fonacot += balor.quitarFormatoMoneda($(this).find('.fonacot').text());
            nomina_primaspagadas += balor.quitarFormatoMoneda($(this).find('.primaspagadaspatron').text());
            nomina_isrart174 += balor.quitarFormatoMoneda($(this).find('.isrart174').text());
            nomina_prestamoinfonavitcf += balor.quitarFormatoMoneda($(this).find('.prestamoinfonavitcf').text());
            nomina_totaldeducciones += balor.quitarFormatoMoneda($(this).find('.totaldeducciones').text());
            nomina_total += balor.quitarFormatoMoneda($(this).find('.total').text());
        });
        /*$("#lblSubtotalXMLNomina").text("Subtotal: " + balor.aplicarFormatoMoneda(nomina_subtotal, "$"));
        $("#lblPercepciones").text("Percepciones: " + balor.aplicarFormatoMoneda(nomina_percepciones, "$"));
        $("#lblDeducciones").text("Deducciones: " + balor.aplicarFormatoMoneda(nomina_deducciones, "$"));
        $("#lblTotal").text("Total: " + balor.aplicarFormatoMoneda(nomina_total, "$"));*/

        // Actualizar los valores de las celdas de totales nomina
        $('.totaln-sueldo').text(balor.aplicarFormatoMoneda(nomina_sueldo, "$"));
        $('.totaln-premioasistencia').text(balor.aplicarFormatoMoneda(nomina_premioasistencia, "$"));
        $('.totaln-premiopuntualidad').text(balor.aplicarFormatoMoneda(nomina_premiopuntualidad, "$"));
        $('.totaln-vacaciones').text(balor.aplicarFormatoMoneda(nomina_vacaciones, "$"));
        $('.totaln-primavacacional').text(balor.aplicarFormatoMoneda(nomina_primavacacional, "$"));
        $('.totaln-aguinaldo').text(balor.aplicarFormatoMoneda(nomina_aguinaldo, "$"));
        $('.totaln-gastosmedicosmayores').text(balor.aplicarFormatoMoneda(nomina_gastosmedicosmayores, "$"));
        $('.totaln-segurodevida').text(balor.aplicarFormatoMoneda(nomina_segurodevida, "$"));
        $('.totaln-indemnizacion').text(balor.aplicarFormatoMoneda(nomina_indemnizacion, "$"));
        $('.totaln-primadeantiguedad').text(balor.aplicarFormatoMoneda(nomina_primadeantiguedad, "$"));
        $('.totaln-ptu').text(balor.aplicarFormatoMoneda(nomina_ptu, "$"));
        $('.totaln-persepciones').text(balor.aplicarFormatoMoneda(nomina_totalpercepciones, "$"));
        $('.totaln-subsidioalempleo').text(balor.aplicarFormatoMoneda(nomina_subsidioalempleo, "$"));
        $('.totaln-isrmes').text(balor.aplicarFormatoMoneda(nomina_isrretenido, "$"));
        $('.totaln-imss').text(balor.aplicarFormatoMoneda(nomina_imss, "$"));
        $('.totaln-infonavit').text(balor.aplicarFormatoMoneda(nomina_infonavit, "$"));
        $('.totaln-fonacot').text(balor.aplicarFormatoMoneda(nomina_fonacot, "$"));
        $('.totaln-primaspagadaspatron').text(balor.aplicarFormatoMoneda(nomina_primaspagadas, "$"));
        $('.totaln-isrart174').text(balor.aplicarFormatoMoneda(nomina_isrart174, "$"));
        $('.totaln-prestamoinfonavitcf').text(balor.aplicarFormatoMoneda(nomina_prestamoinfonavitcf, "$"));
        $('.totaln-deducciones').text(balor.aplicarFormatoMoneda(nomina_totaldeducciones, "$"));
        $('.totaln-total').text(balor.aplicarFormatoMoneda(nomina_total, "$"));
    };

    $('body').on('click', '.elim-Factura', null, function () {
        if (confirm('¿Desea eliminar esta factura?')) {
            var tr = $(this).parents('tr');
            var fp = $(this.parentNode.parentNode).attr("data-Facturaproveedorid");
            if (fp != 0) {
                EliminarDocumentosFacturas(fp);
                var parametros = { FacturaProveedorid: fp, usuario: getUsuario() };
                ejecutaAjax('CapturaPolizas.aspx/EliminarFacturaProveedor', 'POST', parametros, function (d) {
                    if (d.d.EsValido) {
                        tr.remove();
                    } else {
                        showMsg('Error', d.d.Mensaje, 'error');
                    }
                }, function (d) {
                    showMsg('Alerta', d.responseText, 'error');
                }, true);
            }
            else {
                $(this).parents('tr').remove();
            }
            calcularTotales();
        }
    });



    var ProcesarGridFacturaPorPoliza = function (Datos) {
        var resHtml = '';
        var importetotal = 0;
        var i = 0
        //for (var i = 0; i < Datos.length; i++) {
        if (Datos.UUID != null) {
            resHtml += '<tr class="rgFacturaPoliza" data-Factura="' + Datos.Factura + '" data-Uuid="' + Datos.UUID + '" data-Fechatimbre="' + Datos.FechaTimbrado + '"  data-file="' + Datos.Rutaxml + '" data-UltimaAct="' + Datos.UltimaAct + '" data-Estatus="' + Datos.Estatus + '" >';
            resHtml += '<td >' + Datos.Factura + '</td>';
            resHtml += '<td>' + Datos.FechaTimbrado + '</td>';
            resHtml += '<td class="CSubtotal">' + balor.aplicarFormatoMoneda(Datos.SubTotal, "$") + '</td>';
            resHtml += '<td class="CIva">' + balor.aplicarFormatoMoneda(Datos.Iva, "$") + '</td>';
            resHtml += '<td class="CTotal">' + balor.aplicarFormatoMoneda(Datos.Total, "$") + '</td>';
            //resHtml += '<td class="tdeliminar centrado"><span class="closebtn elim-Factura" >X</span></td>';
            resHtml += '</tr>';
            importetotal += Datos.SubTotal;
        //}

        }
        $("#lblSubtotalXMLFacturasPolizas")[0].innerText = "Subtotal: " + balor.aplicarFormatoMoneda(importetotal, "$");
        $("#TablasFacturasPolizas tbody").html(resHtml);
    };


    /*
       ==================================================
       REGION DE CARGA AUTOMATICA DE FACTURAS
       ==================================================
*/
    var fileupload = document.getElementById('upload');    
    //no se usa readfiles - se usa procesarXML
    function readfiles(files) {
        if (files.length <= 0)
            return;
        //Validamos que los archivos seleccionados sean XML
        for (var i = 0; i < files.length; i++) {
            if (files[i].name.toLowerCase().indexOf(".xml") <= 0) {
                alert("El archivo: " + files[i].name + " no es del tipo XML");
                return;
            }
        }
        ShowLightBox(true, "Validando Facturas en el SAT");
        var formData = new FormData();
        for (var i = 0; i < files.length; i++) {
            formData.append('file', files[i]);
        }
        formData.append('empresaid', amplify.store.sessionStorage("EmpresaID"));
        formData.append('fecha', $("#txtFechaPol").val());
        var xhr = new XMLHttpRequest();
        xhr.open('POST', 'ProcessXML_Contable.ashx');
        xhr.onload = function () {
            //AQUI SE PROCESA LA RESPUESTA DEL SERVIDOR
            if (xhr.responseText.indexOf("rror:") > 0) {
                alert(xhr.responseText);
                ShowLightBox(false, "");
                return;
            }
            var facturas = JSON.parse(xhr.responseText);
            var FacturasVigentes = [];
            var FacturasError = [];

            //Procesamos cada una de las facturas cargadas en el sistema
            for (var i = 0; i < facturas.length; i++) {
                //Validamos si la factura aun esta vigente en el SAT
                if (facturas[i].estatus != "Vigente" && facturas[i].response != "Error: unavailable service 503") {
                    facturas[i].textoxml = "FACTURA NO ENCONTRADA EN EL SAT";
                    FacturasError.push(facturas[i]);
                    continue;
                }

                if (ArchivoDuplicado(FacturasVigentes, facturas[i].foliofiscal)) {
                    facturas[i].textoxml = "Archivo Duplicado";
                    FacturasError.push(facturas[i]);
                    continue;
                }

                //Validamos si el folio fiscal ya fue capturado con anterioridad en el sistema
                var continuar = true;
                var parametros = { uuid: facturas[i].foliofiscal };
                ejecutaAjax('CapturaPolizas.aspx/ExisteUUID', 'POST', parametros, function (d) {
                    if (d.d.EsValido) {
                        if (d.d.Datos.Existe) {
                            facturas[i].textoxml = "LA FACTURA YA EXISTE EN EL SISTEMA";
                            FacturasError.push(facturas[i]);
                            continuar = false;
                        }
                    } else {
                        showMsg('Alerta', d.d.Mensaje, 'error');
                    }
                }, function (d) {
                    ShowLightBox(false, "");
                    continuar = false;
                    showMsg('Alerta', d.responseText, 'error');
                }, false);
                if (!continuar)
                    continue;

                // validar rfc emisor vs lista negra sat 2021-05-31
                var continuarListaNegra = true;
                var rfcValidaListaNegra = $.trim(facturas[i].Emisorrfc).toUpperCase();
                var situacionListaNegra = '';

                var parametros = { rfc: rfcValidaListaNegra };
                ejecutaAjax('../../Operacion/Formas/MonitorSolicitudes.aspx/RevisarListaNegraSat', 'POST', parametros, function (d) {
                    if (d.d.EsValido) {
                        if (d.d.Datos.Existe) {

                            //Situacion
                            situacionListaNegra = d.d.Datos.Situacion;
                            facturas[i].textoxml = "EL RFC DEL EMISOR ESTÁ EN LISTA NEGRA DEL SAT (Situación: " + situacionListaNegra + ")";
                            XMLError.push(facturas[i]);
                            continuarListaNegra = false;
                        }
                    } else {
                        showMsg('Alerta', d.d.Mensaje, 'error');
                    }
                }, function (d) {
                    ShowLightBox(false, "");
                    continuarListaNegra = false;
                    showMsg('Alerta', d.responseText, 'error');
                }, false);
                if (!continuarListaNegra)
                    continue;

                if (ExisteUUIDGrid(facturas[i].foliofiscal)) {
                    facturas[i].textoxml = "LA FACTURA YA ESTA CARGADA EN EL GRID";
                    FacturasError.push(facturas[i]);
                    continue;
                }
                //Validamos que la factura corresponda a la empresa correspondiente segun sea el caso a balor o a factur                
                if ($.trim(facturas[i].rfcreceptor).toUpperCase() != $.trim($("#CajaEmpresaRFC").val()).toUpperCase()) {
                    facturas[i].textoxml = "LA FACTURA NO FUE EMITIDA PARA LA EMPRESA: " + $("#CajaEmpresaRFC").val();
                    FacturasError.push(facturas[i]);
                    continue;
                }
                FacturasVigentes.push(facturas[i]);
            }
            ShowLightBox(false, "");
            ProcesarFacturasVigentes(FacturasVigentes);
            if (FacturasError.length > 0) {
                ProcesarFacturasConError(FacturasError);
            }
        };
        xhr.send(formData);
    };

    fileupload.querySelector('input').onchange = function () {
        procesarXML(this.files, this);
    };

  

    var ontenerRFCPorEmpresa = function () {
        var parametros = { empresaid: amplify.store.sessionStorage("EmpresaID") };
        ejecutaAjax('CapturaPolizas.aspx/ontenerRFCPorEmpresa', 'POST', parametros, function (d) {
            if (d.d.EsValido) {
                if (d.d.Datos.Existe) {
                    $("#CajaEmpresaRFC").val(d.d.Datos.RFC);
                }
            } else {
                showMsg('Alerta', d.d.Mensaje, 'error');
            }
        }, function (d) {
            ShowLightBox(false, "");
            showMsg('Alerta', d.responseText, 'error');
        }, true);
    };


    var ExisteUUIDGrid = function (uuid) {
        var existe = false;
        $("#TablaFacturas tbody tr").each(function () {
            if ($(this).attr("data-Uuid") == uuid) {
                existe = true;
            }
        });
        return existe;
    };

    var ArchivoDuplicado = function (Datos, uuid) {
        var Encontrada = false;
        for (var i = 0; i < Datos.length; i++) {
            if (Datos[i].foliofiscal === uuid) {
                Encontrada = true;
                break;
            }
        }
        return Encontrada;
    }

    var ArchivoDuplicadoNomina = function (Datos, uuid) {
        var Encontrada = false;
        for (var i = 0; i < Datos.length; i++) {
            if (Datos[i].foliofiscal === uuid) {
                Encontrada = true;
                break;
            }
        }
        return Encontrada;
    }

    var ProcesarFacturasConError = function (Datos) {
        var mensaje = "";
        for (var i = 0; i < Datos.length; i++) {
            mensaje += "Archivo: " + Datos[i].NombreArchivo + " - " + Datos[i].textoxml + "  \n";
        }
        alert(mensaje);
    };

    var ProcesarFacturasVigentes = function (Datos) {
        var Facturas = [];
        for (var i = 0; i < Datos.length; i++) {
            var Factura = {};
            Factura.RFC = Datos[i].rfcemisor;
            Factura.File = Datos[i].NombreArchivo;
            Factura.EmpresaID = amplify.store.sessionStorage("EmpresaID");
            Facturas.push(Factura);
        }

        var parametros = { value: JSON.stringify(Facturas), fecha: $("#txtFechaPol").val() };
        ejecutaAjax('CapturaPolizas.aspx/ProcesarXmlFacturas', 'POST', parametros, function (d) {
            var Datos = GetFacturasProveedor();
            for (var i = 0; i < d.d.Datos.length; i++) {
                Datos.push(d.d.Datos[i]);
            }
            ProcesarGridFacturas(Datos);
        }, function (d) {
            showMsg('Alerta', d.responseText, 'error');
        }, true);
    };

    body.on("click", '#upload', function (e) {
        $(this).closest('div').find("input:file").val("");
    });

    body.on("click", '#uploadNomina', function (e) {
        $(this).closest('div').find("input:file").val("");
    });
    body.on("click", '#uploadpdfcfdi', function (e) {
        $(this).closest('div').find("input:file").val("");
    });
    body.on("click", '#uploadpdfvalidacion', function (e) {
        $(this).closest('div').find("input:file").val("");
    });
    body.on("click", '#uploadxml', function (e) {
        $(this).closest('div').find("input:file").val("");
    });
    var GetFacturasProveedor = function () {
        var ListaCatfacturasproveedor = [];
        $("#TablaFacturas tbody tr").each(function () {
            var Facturaproveedorid = $(this).attr("data-Facturaproveedorid");
            //if (Facturaproveedorid == 0) {
                var Catfacturasproveedor = {};
                Catfacturasproveedor.Facturaproveedorid = $(this).attr("data-Facturaproveedorid");
                Catfacturasproveedor.Empresaid = amplify.store.sessionStorage("EmpresaID");
                Catfacturasproveedor.Proveedorid = $(this).attr("data-Proveedorid");
                //Catfacturasproveedor.Factura = $(this).attr("data-Factura");
                //Catfacturasproveedor.Subtotal = balor.quitarFormatoMoneda($(this).find(".CSubtotal")[0].innerText);
                //Catfacturasproveedor.Iva = balor.quitarFormatoMoneda($(this).find(".CIva")[0].innerText);
                //Catfacturasproveedor.RetIva = balor.quitarFormatoMoneda($(this).find(".CRetIva")[0].innerText);
                //Catfacturasproveedor.RetIsr = balor.quitarFormatoMoneda($(this).find(".CRetIsr")[0].innerText);
                //Catfacturasproveedor.Total = balor.quitarFormatoMoneda($(this).find(".CTotal")[0].innerText);
                //Catfacturasproveedor.Uuid = $(this).attr("data-Uuid");
                //Catfacturasproveedor.Fechatimbre = $(this).attr("data-Fechatimbre");
                //Catfacturasproveedor.Xml = "";
                //var dataPath = $(this).find(".icon-check.xml").data("path");
                //var nombreArchivo = obtenerNombreArchivo(dataPath);
                //Catfacturasproveedor.NomArchivo = $(this).attr("data-file");// != 'null' ? $(this).attr("data-file") : nombreArchivo;
                Catfacturasproveedor.Factura = $(this).attr("data-Factura") || null;
                Catfacturasproveedor.Uuid = $(this).attr("data-Uuid") || null;
                var xmlPath = $(this).find(".icon-check.xml").data("path");
                var nombreArchivo = xmlPath ? xmlPath.split('\\').pop() : null;
                var nombreA = $(this).find('div.fileUploadXml').attr('data-name');
                Catfacturasproveedor.NomArchivo = $(this).attr("data-file") != ('null' || null || '') ? $(this).attr("data-file") : (nombreArchivo != null ? nombreArchivo : nombreA);
                Catfacturasproveedor.Usuario = getUsuario();
                Catfacturasproveedor.Estatus = $(this).attr("data-Estatus") ? $(this).attr("data-Estatus") : 1;
                Catfacturasproveedor.UltimaAct = $(this).attr("data-UltimaAct");
                ListaCatfacturasproveedor.push(Catfacturasproveedor);
            /*}else {
                var Catfacturasproveedor = {};
                Catfacturasproveedor.Facturaproveedorid = $(this).attr("data-Facturaproveedorid");
                Catfacturasproveedor.Empresaid = amplify.store.sessionStorage("EmpresaID");
                Catfacturasproveedor.Proveedorid = $(this).attr("data-Proveedorid");
                Catfacturasproveedor.Factura = $(this).attr("data-Factura");
                Catfacturasproveedor.Uuid = $(this).attr("data-Uuid");
                Catfacturasproveedor.Usuario = getUsuario();
                Catfacturasproveedor.Estatus = $(this).attr("data-Estatus");
                Catfacturasproveedor.UltimaAct = $(this).attr("data-UltimaAct");
                ListaCatfacturasproveedor.push(Catfacturasproveedor);
            }*/
        });
        return ListaCatfacturasproveedor;
    };




    $('body').on('click', '#chkXML', null, function () {
        if ($(this).is(":checked")) {
            if ($('#ddlTipPol').val() != "IF")
                $(".recuadro").show("slow");
            else {
                if ($('#AyudaPoliza_ValorID').val() != "")
                    CargaDatosFacturaPoliza();
            }

        } else {
            $(".recuadro").hide("slow")
            $(".CuadroFacturas").hide("slow")
            ProcesarGridFacturas([]);
        }
    });

    $('body').on('click', '#chkXMLNomina', null, function () {
        if ($(this).is(":checked")) {
            if ($('#ddlTipPol').val() != "IF")
                $(".recuadro-nomina").show("slow");
            else {
                if ($('#AyudaPoliza_ValorID').val() != "")
                    CargaDatosFacturaPolizaPoliza();
            }

        } else {
            $(".recuadro-nomina").hide("slow")
            //$(".CuadroFacturas").hide("slow")
            ProcesarGridFacturas([]);
        }
    });

    var CargaDatosFacturaPoliza = function () {
        var parametros = { Polizaid: $('#AyudaPoliza_ValorID').val() };
        ejecutaAjax('CapturaPolizas.aspx/ConsultarFacturasPorPoliza', 'POST', parametros, function (d) {
            if (d.d.EsValido) {
                ProcesarGridFacturaPorPoliza(d.d.Datos);
            } else {
                showMsg('Error', d.d.Mensaje, 'error');
            }
        }, function (d) {
            showMsg('Alerta', d.responseText, 'error');
        }, true);

        $(".CuadroFacturas").show("slow");
    };


    /*
    $('input').not('input[type=button]').on('keypress', null, function (e) {
    focusNextControl(this, e);
    });
    */

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

    var TraerInformacionPolizaForanea = function () {
        escondeMsg();
        var parametros = { Polizaid: $('#hiddenPolizaForaneaID').val() };
        ejecutaAjax('CapturaPolizas.aspx/TraerDatosPolizaForanea', 'POST', parametros, function (d) {
            if (d.d.EsValido) {
                $("#ddlTipPol").val(d.d.Datos.TipPol);
                $("#txtFechaPol").val(d.d.Datos.Fechapol);
                $("#AyudaPoliza_Code").val(d.d.Datos.Folio);
                $("#AyudaPoliza_ValorID").val($('#hiddenPolizaForaneaID').val());
                $("#txtConceptoPoliza").val(d.d.Datos.Concepto);
                if (d.d.Datos.Pendiente)
                    $("#chkPendiente").prop("checked", "checked");

                ayudaPoliza_onElementFound();
            } else {
                showMsg('Error', d.d.Mensaje, 'error');
            }
        }, function (d) {
            showMsg('Alerta', d.responseText, 'error');
        }, true);
    };


    /* PROCESAR XML PARA DIOT  --> */
    function procesarXML(files,binp) {
        if (files.length <= 0)
            return;
        //Validamos que los archivos seleccionados sean XML
        for (var i = 0; i < files.length; i++) {
            if (files[i].name.toLowerCase().indexOf(".xml") <= 0) {
                alert("El archivo: " + files[i].name + " no es del tipo XML");
                return;
            }
        }
        ShowLightBox(true, "Validando Facturas en el SAT");
        var formData = new FormData();
        for (var i = 0; i < files.length; i++) {
            formData.append('file', files[i]);
        }
        formData.append('empresaid', amplify.store.sessionStorage("EmpresaID"));
        formData.append('fecha', $("#txtFechaPol").val());
        var xhr = new XMLHttpRequest();
        xhr.open('POST', 'ProcessXML_Contable.ashx');
        xhr.onload = function () {
            //AQUI SE PROCESA LA RESPUESTA DEL SERVIDOR
            if (xhr.responseText.indexOf("rror:") > 0) {
                alert(xhr.responseText);
                ShowLightBox(false, "");
                return;
            }
            var facturas = JSON.parse(xhr.responseText);
            var XMLVigentes = [];
            var XMLError = [];

            //Procesamos cada una de las facturas cargadas en el sistema
            for (var i = 0; i < facturas.length; i++) {
                //Validamos si la factura aun esta vigente en el SAT
                facturas[i].Facturaproveedorid = 0;
                facturas[i].UltimaAct = 0;
                if (facturas[i].TipoRespuesta != 0) {
                    XMLError.push(facturas[i]);
                    continue;

                }

                // validar rfc emisor vs lista negra sat 2021-05-31
                var continuarListaNegra = true;
                var rfcValidaListaNegra = $.trim(facturas[i].Emisorrfc).toUpperCase();
                var situacionListaNegra = '';

                var parametros = { rfc: rfcValidaListaNegra };
                ejecutaAjax('../../Operacion/Formas/MonitorSolicitudes.aspx/RevisarListaNegraSat', 'POST', parametros, function (d) {
                    if (d.d.EsValido) {
                        if (d.d.Datos.Existe) {

                            //Situacion
                            situacionListaNegra = d.d.Datos.Situacion;
                            facturas[i].Descripcion = "EL RFC DEL EMISOR ESTÁ EN LISTA NEGRA DEL SAT (Situación: " + situacionListaNegra + ")";
                            XMLError.push(facturas[i]);
                            continuarListaNegra = false;
                        }
                    } else {
                        showMsg('Alerta', d.d.Mensaje, 'error');
                    }
                }, function (d) {
                    ShowLightBox(false, "");
                    continuarListaNegra = false;
                    showMsg('Alerta', d.responseText, 'error');
                }, false);
                if (!continuarListaNegra)
                    continue;

                if (ExisteUUIDGrid(facturas[i].UUID)) {
                    facturas[i].Descripcion = "LA FACTURA YA ESTA CARGADA EN EL GRID";
                    XMLError.push(facturas[i]);
                    continue;
                }
                //Validamos que la factura corresponda a la empresa correspondiente segun sea el caso a balor o a factur                
                if ($.trim(facturas[i].Receptorrfc).toUpperCase() != $.trim($("#CajaEmpresaRFC").val()).toUpperCase()) {
                    facturas[i].Descripcion = "LA FACTURA NO FUE EMITIDA PARA LA EMPRESA: " + $("#CajaEmpresaRFC").val();
                    XMLError.push(facturas[i]);
                    continue;
                }
                XMLVigentes.push(facturas[i]);
            }
            ShowLightBox(false, "");
            ProcesarXML(XMLVigentes, files, binp);
            if (XMLError.length > 0) {
                ProcesarXMLError(XMLError);
            }
        };
        xhr.send(formData);
    };

    var ProcesarXMLError = function (Datos) {
        var mensaje = "";
        for (var i = 0; i < Datos.length; i++) {
            mensaje += "Archivo: " + Datos[i].NomArchivo + " - " + Datos[i].Descripcion + "  \n";
        }
        alert(mensaje);
    };
    function validateFileType(fileContainer, files, allowedExtensions, fileType) {
        if (files.length <= 0) {
            resetFileInput(fileContainer, fileType);
            return;
        }

        for (var i = 0; i < files.length; i++) {
            var isValidExtension = allowedExtensions[fileType].some(function (ext) {
                return files[i].name.toLowerCase().endsWith(ext);
            });

            if (!isValidExtension) {
                alert("El archivo: " + files[i].name + " no es del tipo " + fileType);
                resetFileInput(fileContainer);
                return;
            }
        }
        // Crear la cadena con los nombres de los archivos
        const fileNameText = Array.from(files).map(file => file.name).join(', ');

        // Actualizar el atributo data-Name del contenedor
        fileContainer.attr('data-Name', fileNameText);

        var buttonText = fileContainer[0] ? fileContainer[0].querySelector('span') : fileContainer.find('span').get(0);
        if (buttonText) {
            buttonText.textContent = '';
        }
        fileContainer.removeClass('btnExcel').addClass('icon-check');
        //fileContainer.classList.remove('btnExcel');fileContainer.classList.add('icon-check');
    }

    function resetFileInput(fileContainer, fileType) {
        var fileInput = fileContainer.querySelector("input[type='file']");
        if (fileInput) {
            fileInput.value = null;
        }

        var buttonText = fileContainer[0] ? fileContainer[0].querySelector('span') : fileContainer.find('span').get(0);
        if (buttonText) {
            buttonText.textContent = 'Cargar ' + fileType.toUpperCase() +'';
        }
        fileContainer.classList.add('btnExcel');
        fileContainer.classList.remove('icon-check');
    }

    var ProcesarXML = function (Datos, fil, inp) {
        var resHtml = '';
        var importetotal = 0;
        var ivaacum = 0;

        for (var i = 0; i < Datos.length; i++) {
            var factura = Datos[i].Factura === null ? 0 : Datos[i].Factura;
            resHtml += '<tr class="rgFactura ' + factura + '" data-Factura="' + factura + '" data-Uuid="' + Datos[i].UUID + '" data-file="' + Datos[i].NomArchivo + '" data-Facturaproveedorid="' + Datos[i].Facturaproveedorid + '" data-UltimaAct="' + Datos[i].UltimaAct + '" data-Proveedorid="' + Datos[i].Proveedorid + '">';
            resHtml += '<td >' + factura + '</td>';
            resHtml += '<td>' + formatoFechaJSON(Datos[i].Fechatimbrado) + '</td>';
            resHtml += '<td class="CSubtotal" style="text-align: right;">' + balor.aplicarFormatoMoneda(Datos[i].Subtotal, "$") + '</td>';
            resHtml += '<td class="CIva" style="text-align: right;">' + balor.aplicarFormatoMoneda(Datos[i].Iva, "$") + '</td>';
            resHtml += '<td class="CRetIva" style="text-align: right;">' + balor.aplicarFormatoMoneda(Datos[i].IvaRet, "$") + '</td>';
            resHtml += '<td class="CRetIsr" style="text-align: right;">' + balor.aplicarFormatoMoneda(Datos[i].IsrRet, "$") + '</td>';
            resHtml += '<td class="CTotal" style="text-align: right;">' + balor.aplicarFormatoMoneda(Datos[i].Total, "$") + '</td>';
            resHtml += '<td class="centrado" ><div class="fileUploadXml btnExcel" data-Uuid="' + Datos[i].UUID + '" data-Facturaproveedorid="' + Datos[i].Facturaproveedorid + '" data-Proveedorid="' + Datos[i].Proveedorid + '"  data-UltimaAct="' + Datos[i].UltimaAct + '" style="position: relative; overflow: hidden; margin-top: 7px;"><span>Cargar XML</span><input type="file" class="uploadxml" style="position: absolute; top: 0; right: 0; margin: 0; padding: 0; font-size: 20px; cursor: pointer; opacity: 0;" multiple=""/></div></td>';
            resHtml += '<td class="centrado" ><div class="fileUploadCfdi btnExcel" data-Uuid="' + Datos[i].UUID + '" data-Facturaproveedorid="' + Datos[i].Facturaproveedorid + '" data-Proveedorid="' + Datos[i].Proveedorid + '"  data-UltimaAct="' + Datos[i].UltimaAct + '" style="position: relative; overflow: hidden; margin-top: 7px;"><span>Cargar PDF</span><input type="file" class="upload" style="position: absolute; top: 0; right: 0; margin: 0; padding: 0; font-size: 20px; cursor: pointer; opacity: 0;" multiple=""/></div></td>';
            resHtml += '<td class="centrado" ><div class="fileUploadValidacion btnExcel" data-Uuid="' + Datos[i].UUID + '" data-Facturaproveedorid="' + Datos[i].Facturaproveedorid + '" data-Proveedorid="' + Datos[i].Proveedorid + '" data-UltimaAct="' + Datos[i].UltimaAct + '" style="position:relative;overflow:hidden;margin-top:7px;"> <span>Cargar PDF</span> <input type="file" class="upload" style="position: absolute; top: 0; right: 0; margin: 0; padding: 0; font-size: 20px; cursor: pointer; opacity: 0;" multiple=""/></div></td>';
            resHtml += '<td class="tdeliminar centrado"><span class="closebtn elim-Factura" >X</span></td>';
            resHtml += '</tr>';
            importetotal += Datos[i].Subtotal;
            ivaacum += Datos[i].Iva;
            
        }
        $("#TablaFacturas tbody").append(resHtml);
        calcularTotales();
        
        
        for (var i = 0; i < Datos.length; i++) {
            $("#TablaFacturas tbody tr").each(function () {
                var factura = Datos[i].Factura === null ? 0 : Datos[i].Factura;
                var facturaClass = factura;
                var fila = $(this);

                if (fila.hasClass(facturaClass)) {
                    var fileUploadContainer = fila.find(".fileUploadXml");
                    var inputElement = fileUploadContainer.find(".uploadxml")[0];
                    var fileList = new DataTransfer();
                    //for (var j = 0; j < fil.length; j++) {
                    fileList.items.add(fil[i]);
                    //}
                    inputElement.files = fileList.files;
                    readfilesxml(fileUploadContainer, fileList.files, "xml");
                }
            });
        }
        $("#TablaFacturas tbody").on('change', '.fileUploadCfdi input', function () {
            var fileUploadcfdi = $(this).closest('.fileUploadCfdi');
            validateFileType(fileUploadcfdi, this.files, { cfdi: [".pdf"] }, "cfdi");
        });

        $("#TablaFacturas tbody").on('change', '.fileUploadValidacion input', function () {
            var fileUploadValidacion = $(this).closest('.fileUploadValidacion');
            validateFileType(fileUploadValidacion, this.files, { validacion: [".pdf"] }, "validacion");
        });
    };

    function readfilesxml(filexml, filesxml, nomarchivoxml) {
        var info = filexml;
        if (filesxml.length <= 0)
            return;

        for (var i = 0; i < filesxml.length; i++) {
            if (filesxml[i].name.toLowerCase().indexOf(".xml") <= 0) {
                alert("El archivo: " + filesxml[i].name + " no es del tipo XML");
                filexml.querySelector("input[type='file']").value = null;
                if (filexml) {
                    filexml.classList.remove('icon-check');
                }
                return;
            }
        }
        if (filexml) {
            var textoBoton = filexml.find('span');
            if (textoBoton.length > 0) {
                textoBoton.text('');
            }
            if (filexml.hasClass('btnExcel')) {
                filexml.removeClass('btnExcel').addClass('icon-check');
            }
        }
    };
    function readfilespdf(file, files,nomarchivo) {
        var info = file;
        if (files.length <= 0)
            return;
        for (var i = 0; i < files.length; i++) {
            if (files[i].name.toLowerCase().indexOf(".pdf") <= 0) {
                alert("El archivo: " + files[i].name + " no es del tipo PDF");
                file.querySelector("input[type='file']").value = null;
                if (file) {
                    var textoBoton = file.querySelector('span');
                    if (textoBoton) {
                        textoBoton.textContent = 'Cargar PDF'; 
                    }
                    file.classList.remove('icon-check');
                    file.classList.add('btnExcel');
                }
                return;
            }
        }
        if (file) {
            var textoBoton = file.querySelector('span');
            if (textoBoton) {
                textoBoton.textContent = ''; 
            }
            file.classList.remove('btnExcel');
            file.classList.add('icon-check');
        }
    }
    var formatoFechaJSON = function (f) {
        var fi = f.replace(/\/Date\((-?\d+)\)\//, "$1");
        var fecha = new Date(parseInt(fi));

        var dd = ('0' + fecha.getDate().toString()).slice(-2)
        var mm = ('0' + (fecha.getMonth() + 1).toString()).slice(-2)
        var y = fecha.getFullYear().toString();
        return dd + '/' + mm + '/' + y;
    };


    /* <-- PROCESAR XML PARA DIOT */





    /* PROCESAR XML DE NÓMINA*/

    $('body').on('click', '.elim-xmlnomina', null, function () {
        if (confirm('¿Desea eliminar este complemento de nómina?')) {
            var tr = $(this).parents('tr');
            var pn = $(this.parentNode.parentNode).attr("data-polizanominaid");
            if (pn != 0) {
                var parametros = { polizanominaid: pn, usuario: getUsuario() };
                ejecutaAjax('CapturaPolizas.aspx/EliminarPolizaNomina', 'POST', parametros, function (d) {
                    if (d.d.EsValido) {
                        tr.remove();
                        calcularTotales();
                    } else {
                        showMsg('Error', d.d.Mensaje, 'error');
                    }
                }, function (d) {
                    showMsg('Alerta', d.responseText, 'error');
                }, true);
            }
            else {
                $(this).parents('tr').remove();
            }
            
        }
    });


    var ProcesarGridNomina = function (Datos) {
        var resHtml = '';
        var subtotal = 0;
        var totalPercepciones = 0;
        var totalDeducciones = 0;
        var total = 0;
        var ivaacum = 0;
        for (var i = 0; i < Datos.length; i++) {
            //resHtml += '<tr class="rgFactura" data-Factura="' + Datos[i].Factura + '" data-Uuid="' + Datos[i].Uuid + '" data-Fechatimbre="' + Datos[i].Fechatimbre + '"  data-file="' + Datos[i].Rutaxml + '" data-Compraid="' + Datos[i].Compraid + '" data-Facturaproveedorid="' + Datos[i].Facturaproveedorid + '" data-Proveedorid="' + Datos[i].Proveedorid + '" data-UltimaAct="' + Datos[i].UltimaAct + '" data-Estatus="' + Datos[i].Estatus + '" >';
            resHtml += '<tr class="rgnomina" data-polizanominaid="' + Datos[i].Polizanominaid + '" data-polizaid="' + Datos[i].Polizaid + '" data-uuid="' + Datos[i].UUID + '" data-file="' + Datos[i].NomArchivo + '" data-UltimaAct="' + Datos[i].UltimaAct + '" ';
            resHtml += 'data-rfcemisor="' + Datos[i].Emisorrfc + '" ';
            resHtml += 'data-Nombreemisor="' + Datos[i].Emisornombre + '" ';
            resHtml += 'data-Rfcreceptor="' + Datos[i].Receptorrfc + '" ';
            resHtml += 'data-Nombrereceptor="' + Datos[i].Receptornombre + '"> ';

            resHtml += '<td  style="text-align:center;" class="serie">' + Datos[i].Serie + '</td>';
            resHtml += '<td  style="text-align:center;" class="folio">' + Datos[i].Factura + '</td>';
            resHtml += '<td style="text-align:center;" class="fechatimbrado">' + formatoFechaJSON(Datos[i].Fechatimbrado) + '</td>';
            resHtml += '<td  style="text-align:right;"class="CSubtotal subtotal">' + balor.aplicarFormatoMoneda(Datos[i].Subtotal, "$") + '</td>';            
            resHtml += '<td class="receptornombre" >' + Datos[i].Receptornombre + '</td>';
            resHtml += '<td style="text-align:center;" class="receptorrfc">' + Datos[i].Receptorrfc + '</td>';
            resHtml += '<td style="text-align:right;" class="CTotal sueldo">' + balor.aplicarFormatoMoneda(Datos[i].Sueldo, "$") + '</td>';
            resHtml += '<td style="text-align:right;" class="CTotal premioasistencia">' + balor.aplicarFormatoMoneda(Datos[i].PremioPorAsistencia, "$") + '</td>';
            resHtml += '<td style="text-align:right;" class="CTotal premiopuntualidad">' + balor.aplicarFormatoMoneda(Datos[i].PremioPorPuntualidad, "$") + '</td>';
            resHtml += '<td style="text-align:right;" class="CTotal vacaciones">' + balor.aplicarFormatoMoneda(Datos[i].Vacaciones, "$") + '</td>';
            resHtml += '<td style="text-align:right;" class="CTotal primavacacional">' + balor.aplicarFormatoMoneda(Datos[i].PrimaVacacional, "$") + '</td>';
            resHtml += '<td style="text-align:right;" class="CTotal aguinaldo">' + balor.aplicarFormatoMoneda(Datos[i].Aguinaldo, "$") + '</td>';
            resHtml += '<td style="text-align:right;" class="CTotal gastosmedicosmayores">' + balor.aplicarFormatoMoneda(Datos[i].GastosMedicosMayores, "$") + '</td>';
            resHtml += '<td style="text-align:right;" class="CTotal segurodevida">' + balor.aplicarFormatoMoneda(Datos[i].SeguroDeVida, "$") + '</td>';
            resHtml += '<td style="text-align:right;" class="CTotal indemnizacion">' + balor.aplicarFormatoMoneda(Datos[i].Indemnizacion, "$") + '</td>';
            resHtml += '<td style="text-align:right;" class="CTotal primadeantiguedad">' + balor.aplicarFormatoMoneda(Datos[i].PrimaDeAntiguedad, "$") + '</td>';
            resHtml += '<td style="text-align:right;" class="CTotal ptu">' + balor.aplicarFormatoMoneda(Datos[i].PTU, "$") + '</td>';
            resHtml += '<td style="text-align:right;" class="CTotal totalpercepciones">' + balor.aplicarFormatoMoneda(Datos[i].TotalPercepciones, "$") + '</td>';

            resHtml += '<td style="text-align:right;" class="CTotal subsidioalempleo">' + balor.aplicarFormatoMoneda(Datos[i].SubsidioAlEmpleo, "$") + '</td>';
            resHtml += '<td style="text-align:right;" class="CTotal isrmensual">' + balor.aplicarFormatoMoneda(Datos[i].IsrMensual, "$") + '</td>';
            resHtml += '<td style="text-align:right;" class="CTotal imss">' + balor.aplicarFormatoMoneda(Datos[i].Imss, "$") + '</td>';
            resHtml += '<td style="text-align:right;" class="CTotal infonavit">' + balor.aplicarFormatoMoneda(Datos[i].Infonavit, "$") + '</td>';
            resHtml += '<td style="text-align:right;" class="CTotal fonacot">' + balor.aplicarFormatoMoneda(Datos[i].Fonacot, "$") + '</td>';
            resHtml += '<td style="text-align:right;" class="CTotal primaspagadaspatron">' + balor.aplicarFormatoMoneda(Datos[i].PrimasPagadasPatron, "$") + '</td>';
            resHtml += '<td style="text-align:right;" class="CTotal isrart174">' + balor.aplicarFormatoMoneda(Datos[i].IsrArt174, "$") + '</td>';
            resHtml += '<td style="text-align:right;" class="CTotal prestamoinfonavitcf">' + balor.aplicarFormatoMoneda(Datos[i].PrestamoInfonavitCF, "$") + '</td>';
            resHtml += '<td style="text-align:right;" class="CTotal totaldeducciones">' + balor.aplicarFormatoMoneda(Datos[i].TotalDeducciones, "$") + '</td>';

            resHtml += '<td style="text-align:right;" class="CTotal total">' + balor.aplicarFormatoMoneda(Datos[i].Total, "$") + '</td>';
            resHtml += '<td class="tdeliminar centrado"><span class="closebtn elim-xmlnomina" >X</span></td>';
            resHtml += '</tr>';
            subtotal += Datos[i].Subtotal;
            totalPercepciones += Datos[i].TotalPercepciones;
            totalDeducciones += Datos[i].TotalDeducciones;
            total += Datos[i].Total;
            
        }
        /*$("#lblSubtotalXMLNomina")[0].innerText = "Subtotal: " + balor.aplicarFormatoMoneda(subtotal, "$");
        $("#lblPercepciones")[0].innerText = "Percepciones: " + balor.aplicarFormatoMoneda(totalPercepciones, "$");
        $("#lblDeducciones")[0].innerText = "Deducciones: " + balor.aplicarFormatoMoneda(totalDeducciones, "$");
        $("#lblTotal")[0].innerText = "Total: " + balor.aplicarFormatoMoneda(total, "$");*/
        $("#TablaFacturasNomina tbody").append(resHtml);
        calcularTotales();
    };

    var getListadoNomina = function () {
        var lista = [];
        $("#TablaFacturasNomina tbody tr").each(function () {
            var nomina = {};
            nomina.Polizanominaid = $(this).attr("data-polizanominaid");
            nomina.Polizaid = $(this).attr("data-polizaid");
            nomina.UUID = $(this).attr("data-uuid");
            nomina.NombreArchivo = $(this).attr("data-file");
            nomina.Rfcemisor = $(this).attr("data-Rfcemisor");
            nomina.Nombreemisor = $(this).attr("data-Nombreemisor");
            nomina.Rfcreceptor = $(this).attr("data-Rfcreceptor");
            nomina.Nombrereceptor = $(this).attr("data-Nombrereceptor");

            nomina.Serie = $(this).find(".serie").text();
            nomina.Folio = $(this).find(".folio").text();
            nomina.Subtotal = balor.quitarFormatoMoneda($(this).find(".subtotal").text());
            nomina.Sueldo = balor.quitarFormatoMoneda($(this).find(".sueldo").text());
            nomina.Premioasistencia = balor.quitarFormatoMoneda($(this).find(".premioasistencia").text());
            nomina.Premiopuntualidad = balor.quitarFormatoMoneda($(this).find(".premiopuntualidad").text());

            nomina.Vacaciones = balor.quitarFormatoMoneda($(this).find(".vacaciones").text());
            nomina.Primavacacional = balor.quitarFormatoMoneda($(this).find(".primavacacional").text());
            nomina.Aguinaldo = balor.quitarFormatoMoneda($(this).find(".aguinaldo").text());
            nomina.Gastosmedicosmayores = balor.quitarFormatoMoneda($(this).find(".gastosmedicosmayores").text());
            nomina.Segurodevida = balor.quitarFormatoMoneda($(this).find(".segurodevida").text());
            nomina.Indemnizacion = balor.quitarFormatoMoneda($(this).find(".indemnizacion").text());
            nomina.Primadeantiguedad = balor.quitarFormatoMoneda($(this).find(".primadeantiguedad").text());
            nomina.Ptu = balor.quitarFormatoMoneda($(this).find(".ptu").text());

            nomina.TotalPercepciones = balor.quitarFormatoMoneda($(this).find(".totalpercepciones").text());

            nomina.Subsidioalempleo = balor.quitarFormatoMoneda($(this).find(".subsidioalempleo").text());
            nomina.Isrretenido = balor.quitarFormatoMoneda($(this).find(".isrmensual").text());
            nomina.Imss = balor.quitarFormatoMoneda($(this).find(".imss").text());
            nomina.Infonavit = balor.quitarFormatoMoneda($(this).find(".infonavit").text());
            nomina.Fonacot = balor.quitarFormatoMoneda($(this).find(".fonacot").text());
            nomina.PrimasPagadas = balor.quitarFormatoMoneda($(this).find(".primaspagadaspatron").text());
            nomina.IsrArt174 = balor.quitarFormatoMoneda($(this).find(".isrart174").text());
            nomina.PrestamoInfonavitCF = balor.quitarFormatoMoneda($(this).find(".prestamoinfonavitcf").text());
            nomina.TotalDeducciones = balor.quitarFormatoMoneda($(this).find(".totaldeducciones").text());

            nomina.Total = balor.quitarFormatoMoneda($(this).find(".total").text());
            lista.push(nomina);
        });
        return lista;
    }


    function readfilesNomina(files) {
        if (files.length <= 0)
            return;
        //Validamos que los archivos seleccionados sean XML
        for (var i = 0; i < files.length; i++) {
            if (files[i].name.toLowerCase().indexOf(".xml") <= 0) {
                alert("El archivo: " + files[i].name + " no es del tipo XML");
                return;
            }
        }
        ShowLightBox(true, "Validando Facturas en el SAT");
        var formData = new FormData();
        for (var i = 0; i < files.length; i++) {
            formData.append('file', files[i]);
        }
        formData.append('empresaid', amplify.store.sessionStorage("EmpresaID"));
        formData.append('fecha', $("#txtFechaPol").val());
        var xhr = new XMLHttpRequest();
        xhr.open('POST', 'ProcesarXMLNomina.ashx');
        xhr.onload = function () {
            //AQUI SE PROCESA LA RESPUESTA DEL SERVIDOR
            if (xhr.responseText.indexOf("rror:") > 0) {
                alert(xhr.responseText);
                ShowLightBox(false, "");
                return;
            }
            var facturas = JSON.parse(xhr.responseText);
            var FacturasVigentes = [];
            var FacturasError = [];

            //Procesamos cada una de las facturas cargadas en el sistema
            for (var i = 0; i < facturas.length; i++) {
                //Validamos si la factura aun esta vigente en el SAT
                //if (facturas[i].estatus != "Vigente" && facturas[i].response != "Error: unavailable service 503") {
                if (facturas[i].Estatus != "Vigente") {
                    facturas[i].textoxml = "FACTURA NO ENCONTRADA EN EL SAT";
                    FacturasError.push(facturas[i]);
                    continue;
                }

                if (ArchivoDuplicadoNomina(FacturasVigentes, facturas[i].UUID)) {
                    facturas[i].textoxml = "Archivo de Nómina Duplicado";
                    FacturasError.push(facturas[i]);
                    continue;
                }

                //Validamos si el folio fiscal ya fue capturado con anterioridad en el sistema
                var continuar = true;
                var parametros = { uuid: facturas[i].UUID };
                ejecutaAjax('CapturaPolizas.aspx/ExisteUUIDNomina', 'POST', parametros, function (d) {
                    if (d.d.EsValido) {
                        if (d.d.Datos.Existe) {
                            facturas[i].textoxml = "EL COMPLEMENTO DE NÓMINA YA EXISTE EN EL SISTEMA";
                            FacturasError.push(facturas[i]);
                            continuar = false;
                        }
                    } else {
                        showMsg('Alerta', d.d.Mensaje, 'error');
                    }
                }, function (d) {
                    ShowLightBox(false, "");
                    continuar = false;
                    showMsg('Alerta', d.responseText, 'error');
                }, false);
                if (!continuar)
                    continue;

                // validar rfc emisor vs lista negra sat 2021-05-31
                var continuarListaNegra = true;
                var rfcValidaListaNegra = $.trim(facturas[i].Emisorrfc).toUpperCase();
                var situacionListaNegra = '';

                var parametros = { rfc: rfcValidaListaNegra };
                ejecutaAjax('../../Operacion/Formas/MonitorSolicitudes.aspx/RevisarListaNegraSat', 'POST', parametros, function (d) {
                    if (d.d.EsValido) {
                        if (d.d.Datos.Existe) {

                            //Situacion
                            situacionListaNegra = d.d.Datos.Situacion;
                            facturas[i].textoxml = "EL RFC DEL EMISOR ESTÁ EN LISTA NEGRA DEL SAT (Situación: " + situacionListaNegra + ")";
                            XMLError.push(facturas[i]);
                            continuarListaNegra = false;
                        }
                    } else {
                        showMsg('Alerta', d.d.Mensaje, 'error');
                    }
                }, function (d) {
                    ShowLightBox(false, "");
                    continuarListaNegra = false;
                    showMsg('Alerta', d.responseText, 'error');
                }, false);
                if (!continuarListaNegra)
                    continue;

                if (ExisteUUIDGrid(facturas[i].foliofiscal)) {
                    facturas[i].textoxml = "EL COMPLEMENTO DE NÓMINA YA ESTA CARGADA EN EL GRID";
                    FacturasError.push(facturas[i]);
                    continue;
                }
                //Validamos que la factura corresponda a la empresa correspondiente segun sea el caso a balor o a factur                
                //if ($.trim(facturas[i].rfcreceptor).toUpperCase() != $.trim($("#CajaEmpresaRFC").val()).toUpperCase()) {
                //    facturas[i].textoxml = "LA FACTURA NO FUE EMITIDA PARA LA EMPRESA: " + $("#CajaEmpresaRFC").val();
                //    FacturasError.push(facturas[i]);
                //    continue;
                //}
                FacturasVigentes.push(facturas[i]);
            }
            ShowLightBox(false, "");
            ProcesarGridNomina(FacturasVigentes);
            if (FacturasError.length > 0) {
                ProcesarFacturasConError(FacturasError);
            }
        };
        xhr.send(formData);
    };

    var fileuploadnomina = document.getElementById('uploadNomina');

    fileuploadnomina.querySelector('input').onchange = function () {
        //readfiles(this.files);
        readfilesNomina(this.files);
    };
    $('body').on('change', '#ddlBancos', null, function () {
        var cuentacheque = $("#ddlBancos option:selected").attr("data-cuentacheque");
        var consecutivo = parseInt($("#ddlBancos option:selected").attr("data-consecutivo"));
        //$("#txtcuentacheque").val(cuentacheque);
        $("#txtconsecutivo").val(consecutivo + 1);
        $("#AyudaPoliza_Code").val(consecutivo + 1);
    });
   
    var traerBancosEmpresas = function (empresaid) {
        var datos = { "empresaid": empresaid };
        ejecutaAjax('CapturaPolizas.aspx/traerBancosEmpresas', 'POST', datos, function (d) {
            if (d.d.EsValido) {
                $("#ddlBancos").html("");
                var datos = d.d.Datos;
                var h = "";
                h = "<option value='*' data-cuentacheque='' data-consecutivo='' data-moneda=''>Ninguno</option>";
                for (var i = 0; i < datos.length; i++) {
                    h += "<option value='" + datos[i].Empresabancoid + "' data-cuenta='" + datos[i].Cuenta + "' data-cuentacheque='" + datos[i].CtaCheq + "' data-consecutivo='" + datos[i].Consecutivo + "' data-moneda='" + datos[i].Moneda + "' >" + datos[i].Nombre + "(" + datos[i].CtaCheq + ")</option>";
                }
                $("#ddlBancos").html(h);
            } else {
                showMsg('Alerta', d.d.Mensaje, 'error');
            }
        }, function (d) {
            ShowLightBox(false, "");
            continuar = false;
            showMsg('Alerta', d.responseText, 'error');
        }, false);
    };


    /*FIN PROCESAR XML DE NÓMINA*/


    $('body').on('click', '#btnCopiarPendiente', null, function () {
        var parametros = obtenerDatosPoliza();
        escondeMsg();
        ShowLightBox(true, "Espere porfavor");
        ejecutaAjax('CapturaPolizas.aspx/GuardarCopiaDePoliza', 'POST', { value: JSON.stringify(parametros) }, function (d) {
            ShowLightBox(false, "");
            if (d.d.EsValido) {
                if (!d.d.Datos.Guardo) {
                    alert(d.d.Datos.msg);
                }
                else {
                    alert('La póliza se copió como pendiente correctamente!');
                    window.location.replace("CapturaPolizas.aspx");
                }
            } else {
                ShowLightBox(false, "");
                showMsg('Alerta', 'No se pudo copiar la póliza. ' + d.d.Mensaje, 'warning');
            }
        }, function (d) {
            ShowLightBox(false, "");
            showMsg('Alerta', d.responseText, 'error');
        }, true);
    });

    $('body').on('click', '#btnCopiar', null, function () {
        if (JSON.parse(sessionStorage.getItem('__amplify__EmpresaID')).data != 'a7d3e5a4-6508-483b-8a3d-0e379ff06755') {
            abrirModalCopiaPoliza();
        } else {
            abrirModalCopiaPolizaFacturco();
        }
    });

    var habilitaBotonCopiaPoliza = function () {
        if ($("#chkPendiente").is(":checked")) {
            $("#btnCopiarPendiente").prop("disabled", true);
        }
        else {
            $("#btnCopiarPendiente").prop("disabled", false);
        }
    }
    var habilitaBtnCopiaPoliza = function () {
        if ($("#chkPendiente").is(":checked")) {
            $("#btnCopiar").prop("disabled", true);
        }
        else {
            $("#btnCopiar").prop("disabled", false);
        }
    }
    function abrirModalCopiaPoliza() {
        muestraPopup('CopiaPoliza', false);
    }
    
    function cerrarModalCopiaPoliza() {
        var modal = document.getElementById('CopiaPoliza');
        modal.style.display = 'none';
    }

    window.onclick = function (event) {
        var modal = document.getElementById('CopiaPoliza');
        if (event.target == modal) {
            modal.style.display = 'none';
        }
    };
    $('body').on('click', '#CopiaPoliza-close', null, function () {
        cerrarPopup('CopiaPoliza', false);
    });
    $('body').on('click', '#CopiaPoliza-btn', null, function () {
        if ($("#txtFechaPolNew").val() == "") {
            alert("Dedes ingresar una fecha");
            $("#txtFechaPolNew").focus();
            return;
        }
        cerrarPopup('CopiaPoliza', false);
        guardarCopiaPoliza();
    });
    var guardarCopiaPoliza = function () {
        console.log($("#txtFechaPolNew").val());
        
        var parametros = obtenerDatosPolizaFechaNew();
        var param = OtPoliza = {
            foliopolOt: $('#AyudaPoliza_Code').val(),
            TipPolOt: $('#ddlTipPol').val(),
            FechapolOt: $('#txtFechaPol').val(),
            checkgrab: $('#chkPendiente').is(':checked')
        };
        escondeMsg();
        ShowLightBox(true, "Espere porfavor");
        ejecutaAjax('CapturaPolizas.aspx/GuardarCopiaPolizaFecha', 'POST', {
            value: JSON.stringify(parametros), othersvalue: JSON.stringify(param)
        }, function (d) {
            ShowLightBox(false, "");
            if (d.d.EsValido) {
                muestraPoliza(d.d.Datos);
                alert("La póliza se copió correctamente! Tipo: " + d.d.Datos.TipPol + " Folio: " + d.d.Datos.Folio + " Fecha: " + d.d.Datos.Fechapol + "");
                console.log(d.d.Datos);
                window.location.replace("CapturaPolizas.aspx");
            } else {
                ShowLightBox(false, "");
                showMsg('Alerta', 'No se pudo copiar la póliza. ' + d.d.Mensaje, 'warning');
        }
        }, function (d) {
            ShowLightBox(false, "");
            showMsg('Alerta', d.responseText, 'error');
        }, true);
    }

    function abrirModalCopiaPolizaFacturco() {
        muestraPopup('CopiaPolizaFacturco', false);
    }
    function cerrarModalCopiaPolizaFacturco() {
        var modal = document.getElementById('CopiaPolizaFacturco');
        modal.style.display = 'none';
    }

    window.onclick = function (event) {
        var modal = document.getElementById('CopiaPolizaFacturco');
        if (event.target == modal) {
            modal.style.display = 'none';
        }
    };
    $('body').on('click', '#CopiaPolizaFacturco-close', null, function () {
        cerrarPopup('CopiaPolizaFacturco', false);
    });
    $('body').on('click', '#CopiaPolizaFacturco-btn', null, function () {
        if ($("#txtFechaPolFacturcoNew").val() == "") {
            alert("Dedes ingresar una fecha");
            $("#txtFechaPolFacturcoNew").focus();
            return;
        }
        cerrarPopup('CopiaPolizaFacturco', false);
        guardarCopiaPolizaFacturco();
    })
    var guardarCopiaPolizaFacturco = function () {
        console.log($("#txtFechaPolFacturcoNew").val());

        var parametros = obtenerDatosPolizaFacturcoFechaNew();
        var param = OtPoliza = {
            foliopolOt : $('#AyudaPoliza_Code').val(),
            TipPolOt : $('#ddlTipPol').val(),
            FechapolOt : $('#txtFechaPol').val(),
            checkgrab : $('#chkPendiente').is(':checked'),
            mesnuevo : $('#txtMesPolizaNew').val(), 
            anonuevo : $('#txtAnoPolizaNew').val()
        };
        escondeMsg();
        ShowLightBox(true, "Espere porfavor");
        ejecutaAjax('CapturaPolizas.aspx/GuardarCopiaPolizaFecha', 'POST', {
            value: JSON.stringify(parametros), othersvalue: JSON.stringify(param)
        }, function (d) {
            ShowLightBox(false, "");
            if (d.d.EsValido) {
                muestraPoliza(d.d.Datos);
                alert("La póliza se copió correctamente! Tipo: " + d.d.Datos.TipPol + " Folio: " + d.d.Datos.Folio + " Fecha: " + d.d.Datos.Fechapol + "");
                console.log(d.d.Datos);
                window.location.replace("CapturaPolizas.aspx");
            } else {
                ShowLightBox(false, "");
                showMsg('Alerta', 'No se pudo copiar la póliza. ' + d.d.Mensaje, 'warning');
            }
        }, function (d) {
            ShowLightBox(false, "");
            showMsg('Alerta', d.responseText, 'error');
        }, true);
    }
    var obtenerDatosPolizaFechaNew = function () {
        var detalle = $('#tablaDetalle tbody tr'),
        len = detalle.length,
        poldetalle = [],
        poliza = {
            Polizaid: "00000000-0000-0000-0000-000000000000",
            EmpresaId: amplify.store.sessionStorage("EmpresaID"),
            Folio: $('#AyudaPoliza_Code').val(),
            TipPol: $('#ddlTipPol').val(),
            Fechapol: $('#txtFechaPolNew').val(),
            Concepto: $('#txtConceptoPolizaNew').val(),
            Importe: $('#tg_cargos').text().replace(/,/g, '') || 0,
            Pendiente: $('#chkPendiente').is(':checked'),
            Usuario: getUsuario(), //parent.document.getElementById("OcultoPropiedades").value, // $('#HiddenUsuario').val(),
            Estatus: $('#HiddenEstatus').val() || 1,
            //Fecha:
            UltimaAct: 0,
            Pagoprogramado: $('#chkPagoProgramado').is(':checked'),
            Empresabancoid: $("#ddlBancos").val() == "*" ? null : $("#ddlBancos").val()

        };

        for (var i = 0; i < len; i++) {
            var tr = $(detalle[i]),
            cuenta = {
                Polizadetalleid: "00000000-0000-0000-0000-000000000000",
                Polizaid: '',
                Cuentaid: tr.attr('data-cuentaid'),
                Cuenta: '',
                CuentaDesc: '',
                TipMov: tr.attr('data-tipmov'),
                //Concepto: $('.conceptodet', tr).val(),
                Concepto: $('#txtConceptoPolizaNew').val(),
                Cantidad: 1,
                Importe: $('.importe', tr).val().replace(/,/g, '') || 0,
                Estatus: tr.attr('data-estatus') || 1,
                //Fecha: tr.attr('data-fecha'),
                Fecha: $('#txtFechaPolNew').val(),
                Usuario: getUsuario(), // parent.document.getElementById("OcultoPropiedades").value, // tr.attr('data-usuario'),
                //UltimaAct: tr.attr('data-ultimaact') || 0
                UltimaAct: 0
            };
            poldetalle.push(cuenta);
        }
        poliza.ListaPolizaDetalle = poldetalle;
        return poliza;
    };
    var obtenerDatosPolizaFacturcoFechaNew = function () {
        var detalle = $('#tablaDetalle tbody tr'),
        len = detalle.length,
        poldetalle = [],
        poliza = {
            Polizaid: "00000000-0000-0000-0000-000000000000",
            EmpresaId: amplify.store.sessionStorage("EmpresaID"),
            Folio: $('#AyudaPoliza_Code').val(),
            TipPol: $('#ddlTipPol').val(),
            Fechapol: $('#txtFechaPolFacturcoNew').val(),
            Concepto: modificarMesYAnio($('#txtConceptoPoliza').val(), $('#txtMesPolizaNew').val(), $('#txtAnoPolizaNew').val()),
            Importe: $('#tg_cargos').text().replace(/,/g, '') || 0,
            Pendiente: $('#chkPendiente').is(':checked'),
            Usuario: getUsuario(), //parent.document.getElementById("OcultoPropiedades").value, // $('#HiddenUsuario').val(),
            Estatus: $('#HiddenEstatus').val() || 1,
            //Fecha:
            UltimaAct: 0,
            Pagoprogramado: $('#chkPagoProgramado').is(':checked'),
            Empresabancoid: $("#ddlBancos").val() == "*" ? null : $("#ddlBancos").val()

        };

        for (var i = 0; i < len; i++) {
            var tr = $(detalle[i]),
            cuenta = {
                Polizadetalleid: "00000000-0000-0000-0000-000000000000",
                Polizaid: '',
                Cuentaid: tr.attr('data-cuentaid'),
                Cuenta: '',
                CuentaDesc: '',
                TipMov: tr.attr('data-tipmov'),
                //Concepto: $('.conceptodet', tr).val(),
                Concepto: modificarMesYAnio($('.conceptodet', tr).val(), $('#txtMesPolizaNew').val(), $('#txtAnoPolizaNew').val()),
                Cantidad: 1,
                Importe: $('.importe', tr).val().replace(/,/g, '') || 0,
                Estatus: tr.attr('data-estatus') || 1,
                //Fecha: tr.attr('data-fecha'),
                Fecha: $('#txtFechaPolFacturcoNew').val(),
                Usuario: getUsuario(), // parent.document.getElementById("OcultoPropiedades").value, // tr.attr('data-usuario'),
                //UltimaAct: tr.attr('data-ultimaact') || 0
                UltimaAct: 0
            };
            poldetalle.push(cuenta);
        }
        poliza.ListaPolizaDetalle = poldetalle;
        return poliza;
    };
    $(document).ready(function () {
        $(".recuadro").hide();
        $(".recuadro-nomina").hide();
        $(".CuadroFacturas").hide();
        if (amplify.store.sessionStorage("UsuarioID") == null || amplify.store.sessionStorage("UsuarioID") == "") {
            window.location.replace("../../login.aspx");
        }
        $("#Menu").MostrarMenuBar(JSON.parse(amplify.store.sessionStorage("Funcionalidades")), amplify.store.sessionStorage("Nombre"), amplify.store.sessionStorage("NombreEmpresa"), amplify.store.sessionStorage("UsuarioID"));
        $('.popup').setDraggable('.popup-body, .popup-footer, .close');
        ActualizaMenuBar();
        iniciaAyudas();
        llenaComboTipPol();
        llenarComboArchivos();
        $('#btnGuardar, #btnImprimir').attr('disabled', 'disabled');
        $('#ddlTipPol').focus();
        $("#txtFechaPol").val(fechaActual());
        AplicarEstilo();
        if ($("#hiddenPolizaForaneaID").val() != "") {
            TraerInformacionPolizaForanea();
        }
        $('.numerico').autoNumeric('init', { mDec: 2, vMin: '-999999999.99' });
        $("#lblCambiarPoliza,#lblCambiarFecha").hide();
        ontenerRFCPorEmpresa();


        $("#TablaFacturasNomina").css("border-top-style", "none");
        $("#TablaFacturasNomina").css("border-left-style", "none");
        $("#TablaFacturasNomina").css("border-right-style", "none");
        
        //Encabezados

        $("#TablaFacturasNomina > thead > tr:eq(1) > th").css({ "font-weight": "lighter" });

        $("#TablaFacturasNomina > thead > tr:eq(0) > th:eq(0)").css("background-color", "#fff");
        $("#TablaFacturasNomina > thead > tr:eq(0) > th:eq(0)").css("border-style", "none");
        $("#TablaFacturasNomina > thead > tr:eq(0) > th:eq(1)").css({ "background-color": "#8ED84B", "color": "#000", "border-color": "#000", "border": "1px solid #000", "font-weight": "lighter" });
        $("#TablaFacturasNomina > thead > tr:eq(0) > th:eq(2)").css({ "background-color": "#EEDC82", "color": "#000", "border-color": "#000", "border": "1px solid #000", "font-weight": "lighter" });
        $("#TablaFacturasNomina > thead > tr:eq(0) > th:eq(3)").css("background-color", "#fff");
        

        //Percepciones        
        $("#TablaFacturasNomina > thead > tr:eq(1) > th:eq(6)").css({ "background-color": "#8ED84B", "color": "#000", "border": "1px solid #000", "font-weight": "lighter" });
        $("#TablaFacturasNomina > thead > tr:eq(1) > th:eq(7)").css({ "background-color": "#8ED84B", "color": "#000", "border": "1px solid #000", "font-weight": "lighter" });
        $("#TablaFacturasNomina > thead > tr:eq(1) > th:eq(8)").css({ "background-color": "#8ED84B", "color": "#000", "border": "1px solid #000", "font-weight": "lighter" });
        $("#TablaFacturasNomina > thead > tr:eq(1) > th:eq(9)").css({ "background-color": "#8ED84B", "color": "#000", "border": "1px solid #000", "font-weight": "lighter" });
        $("#TablaFacturasNomina > thead > tr:eq(1) > th:eq(10)").css({ "background-color": "#8ED84B", "color": "#000", "border": "1px solid #000", "font-weight": "lighter" });
        $("#TablaFacturasNomina > thead > tr:eq(1) > th:eq(11)").css({ "background-color": "#8ED84B", "color": "#000", "border": "1px solid #000", "font-weight": "lighter" });
        $("#TablaFacturasNomina > thead > tr:eq(1) > th:eq(12)").css({ "background-color": "#8ED84B", "color": "#000", "border": "1px solid #000", "font-weight": "lighter" });
        $("#TablaFacturasNomina > thead > tr:eq(1) > th:eq(13)").css({ "background-color": "#8ED84B", "color": "#000", "border": "1px solid #000", "font-weight": "lighter" });
        $("#TablaFacturasNomina > thead > tr:eq(1) > th:eq(14)").css({ "background-color": "#8ED84B", "color": "#000", "border": "1px solid #000", "font-weight": "lighter" });
        $("#TablaFacturasNomina > thead > tr:eq(1) > th:eq(15)").css({ "background-color": "#8ED84B", "color": "#000", "border": "1px solid #000", "font-weight": "lighter" });
        $("#TablaFacturasNomina > thead > tr:eq(1) > th:eq(16)").css({ "background-color": "#8ED84B", "color": "#000", "border": "1px solid #000", "font-weight": "lighter" });
        $("#TablaFacturasNomina > thead > tr:eq(1) > th:eq(17)").css({ "background-color": "#8ED84B", "color": "#000", "border": "1px solid #000", "font-weight": "lighter" });
        
        //Deducciones                
        $("#TablaFacturasNomina > thead > tr:eq(1) > th:eq(18)").css({ "background-color": "#EEDC82", "border-color": "#000", "color": "#000", "border": "1px solid #000", "font-weight": "lighter" });
        $("#TablaFacturasNomina > thead > tr:eq(1) > th:eq(19)").css({ "background-color": "#EEDC82", "border-color": "#000", "color": "#000", "border": "1px solid #000", "font-weight": "lighter" });
        $("#TablaFacturasNomina > thead > tr:eq(1) > th:eq(20)").css({ "background-color": "#EEDC82", "border-color": "#000", "color": "#000", "border": "1px solid #000", "font-weight": "lighter" });
        $("#TablaFacturasNomina > thead > tr:eq(1) > th:eq(21)").css({ "background-color": "#EEDC82", "border-color": "#000", "color": "#000", "border": "1px solid #000", "font-weight": "lighter" });
        $("#TablaFacturasNomina > thead > tr:eq(1) > th:eq(22)").css({ "background-color": "#EEDC82", "border-color": "#000", "color": "#000", "border": "1px solid #000", "font-weight": "lighter" });
        $("#TablaFacturasNomina > thead > tr:eq(1) > th:eq(23)").css({ "background-color": "#EEDC82", "border-color": "#000", "color": "#000", "border": "1px solid #000", "font-weight": "lighter" });

        $("#TablaFacturasNomina > thead > tr:eq(1) > th:eq(24)").css({ "background-color": "#EEDC82", "border-color": "#000", "color": "#000", "border": "1px solid #000", "font-weight": "lighter" });
        $("#TablaFacturasNomina > thead > tr:eq(1) > th:eq(25)").css({ "background-color": "#EEDC82", "border-color": "#000", "color": "#000", "border": "1px solid #000", "font-weight": "lighter" });
        $("#TablaFacturasNomina > thead > tr:eq(1) > th:eq(26)").css({ "background-color": "#EEDC82", "border-color": "#000", "color": "#000", "border": "1px solid #000", "font-weight": "lighter" });
        $("#TablaFacturasNomina > thead > tr:eq(1) > th:eq(27)").css({ "background-color": "#EEDC82", "border-color": "#000", "color": "#000", "border": "1px solid #000", "font-weight": "lighter" });

        $("#TablaFacturasNomina > thead > tr:eq(0) > th:eq(3)").css({ "border-right": "0px solid #fff !important" });

        traerBancosEmpresas(amplify.store.sessionStorage("EmpresaID"));
        
    });
    var llenarComboArchivos = function () {
        ejecutaAjax('CapturaPolizas.aspx/ConsultarCatDocumentosPolizas', 'POST', {}, function (d) {
            if (d.d.EsValido) {
                var opciones = d.d.Datos; 
                var selectElement = document.getElementById("cmbArchivos");
                selectElement.innerHTML = '';

                for (var i = 0; i < opciones.length; i++) {
                    var option = document.createElement("option");
                    option.value = opciones[i].DocumentoID; 
                    option.text = opciones[i].NombreDocumento;
                    selectElement.add(option);
                }
            } else {
                console.log("La respuesta no es válida");
            }
        }, function (d) {
            showMsg('Alerta', d.responseText, 'error');
        }, false);
    };
    var mostrararchivosPoliza = function (poliza) {
        //poliza.Polizaid;
        var parametros = {};
        parametros.Polizaid = poliza.Polizaid;
        ejecutaAjax('CapturaPolizas.aspx/ConsultarDocumentosPorPoliza', 'POST',  parametros , function (d) {
            if (d.d.EsValido) {
                if (d.d.Datos) {
                    for (var i = 0; i < d.d.Datos.length; i++) {
                        var datos = d.d.Datos[i];

                        var nuevaFila = $("<tr data-id='" + datos.Uuid + "' data-UltimaAct='" + datos.UltimaAct + "' data-Uuid='" + datos.Uuid + "'>");

                        nuevaFila.append("<td>" + datos.Documento + "</td>");
                        nuevaFila.append("<td class='tdeliminar centrado'><span class='closebtn elimina-com'>X</span></td>");
                        nuevaFila.append('<td class="centrado"><div class="icon-check archivo" data-path="' + datos.Ruta + '"></div></td>');

                        $("#ArchivosAdicionales-Detalle tbody").append(nuevaFila);

                        nuevaFila.find('.archivo').on("click", function (event) {
                            event.preventDefault();
                            var Path = $(this).data("path");
                            console.log("pdfPath:", Path);
                            descargardoc(Path);
                        });
                    }
                    if ($("#ArchivosAdicionales-Detalle tbody tr").length > 0) {
                        $("#btndescargarTodo").removeAttr("style");
                    } 
                }
            }
        });
        function descargardoc(Path) {
            //var servicioBaseURL = "http://localhost:46802/";
            var servicioBaseURL = "http://192.168.11.237/services/";
            var rutaDocumento = Path;
            var ext = Path.split('.').pop();
            if (ext == "pdf" || ext == "PDF") {
                nombreArchivo = rutaDocumento.match(/([^\\]+\.pdf)$/i)[1];
                rutaDocumento = rutaDocumento.replace(/\\[^\\]+\.pdf$/i, "");
            }
            if (ext == "xml") {
                nombreArchivo = rutaDocumento.match(/([^\\]+\.xml)$/i)[1];
                rutaDocumento = rutaDocumento.replace(/\\[^\\]+\.xml$/i, "");
            }
            if (ext == "png") {
                nombreArchivo = rutaDocumento.match(/([^\\]+\.png)$/i)[1];
                rutaDocumento = rutaDocumento.replace(/\\[^\\]+\.png$/i, "");
            }
            if (ext == "jpg") {
                nombreArchivo = rutaDocumento.match(/([^\\]+\.jpg)$/i)[1];
                rutaDocumento = rutaDocumento.replace(/\\[^\\]+\.jpg$/i, "");
            }
            if (ext == "jpeg") {
                nombreArchivo = rutaDocumento.match(/([^\\]+\.jpeg)$/i)[1];
                rutaDocumento = rutaDocumento.replace(/\\[^\\]+\.jpeg$/i, "");
            } 
            var width = window.innerWidth * 0.66;
            var height = width * window.innerHeight / window.innerWidth;

            window.location.href = servicioBaseURL + "DescargarArchivos.ashx?rutaDocumento=" + encodeURIComponent(rutaDocumento) + "&nombreArchivo=" + encodeURIComponent(nombreArchivo);
        }
   }
}(jQuery, window.balor, window.amplify));
function getFileExtension(file) {
    const parts = file.split('.');
    if (parts.length > 1) {
        return parts[parts.length - 1].toLowerCase();
    } else {
        return ''; // No extension found
    }
}
var getUsuario = function () {
    return amplify.store.sessionStorage("Usuario");
};
var listArchAd = null;
function subirArchivoDetalle(listaArchivos, facturasProveedor) {
    //Object.values(listaArchivos).forEach(function (archivo) {
        //Object.entries(listaArchivos).forEach(function ([key, archivo]) {
       listaArchivos.forEach(function (archivo) {
        var filesXml = archivo.elementxml;
        var filesCfdi = archivo.elementCfdi;
        var filesValidacion = archivo.elementValidacion;
        var poliza = archivo.poliza;
        var dataFactura = archivo.vueltas;
        var uuid = archivo.uuid;
        var proveedorid = archivo.proveedorid;
        var filexml = archivo.uploadXmlElement;
        var filecfdi = archivo.uploadPdfCfdiElement;
        var filevalidacion = archivo.uploadPdfValidacionElement;

        var nombreArchivoxml = "";
        var nombreArchivocfdi = "";
        var nombreArchivovalidacion = "";
        
        if (filesXml && filesXml.files.length > 0 || (poliza.FacturasProveedor && poliza.FacturasProveedor.some(f => f.Facturaproveedorid === dataFactura))) {
            nombreArchivoxml = filesXml && filesXml.files.length > 0 ? filesXml.files[0].name: poliza.FacturasProveedor.find(f => f.Facturaproveedorid === dataFactura).nombreArchivoXml;
        }

        if (filesCfdi && filesCfdi.files.length > 0 || (poliza.FacturasProveedor && poliza.FacturasProveedor.some(f => f.Facturaproveedorid === dataFactura))) {
            nombreArchivocfdi = filesCfdi && filesCfdi.files.length > 0 ? filesCfdi.files[0].name: poliza.FacturasProveedor.find(f => f.Facturaproveedorid === dataFactura).nombreArchivoCfdi;
        }

        if (filesValidacion && filesValidacion.files.length > 0 || (poliza.FacturasProveedor && poliza.FacturasProveedor.some(f => f.Facturaproveedorid === dataFactura))) {
            nombreArchivovalidacion = filesValidacion && filesValidacion.files.length > 0 ? filesValidacion.files[0].name: poliza.FacturasProveedor.find(f => f.Facturaproveedorid === dataFactura).nombreArchivoValidacion;
        }   

        var extencionxml = getFileExtension(nombreArchivoxml);
        var extencioncfdi = getFileExtension(nombreArchivocfdi);
        var extencionvalidacion = getFileExtension(nombreArchivovalidacion);

        var formdata = new FormData();
        formdata.append("FechaPol", $("#txtFechaPol").val());
        formdata.append("TipoPol", $("#ddlTipPol").val());

        if (poliza) {
            formdata.append("PolizaID", poliza.Folio);

            var facturaProveedor = facturasProveedor.find(factura => factura.Factura === dataFactura);

            if (facturaProveedor) {
                formdata.append("facturaProveedorId", facturaProveedor.Facturaproveedorid);
                formdata.append("proveedorId", facturaProveedor.Proveedorid);
                formdata.append("ultimaAct", facturaProveedor.UltimaAct);
                formdata.append("Empresa", facturaProveedor.Empresaid);
                formdata.append("uuid", facturaProveedor.Uuid);
            } else {
                formdata.append("Empresa", amplify.store.sessionStorage("EmpresaID"));
                formdata.append("facturaProveedorId", 0);
                formdata.append("uuid", uuid);
                formdata.append("proveedorId", proveedorid);
            }
        }
        formdata.append("nombreArchivoXml", nombreArchivoxml);
        formdata.append("nombreArchivoCfdi", nombreArchivocfdi);
        formdata.append("nombreArchivoValidacion", nombreArchivovalidacion);
        formdata.append("extensionCfdi", extencioncfdi);
        formdata.append("extensionXml", extencionxml);
        formdata.append("extensionValidacion", extencionvalidacion);
        formdata.append("usuario", getUsuario());
        formdata.append("tipo", "FACTURA");

        if (filesXml && filesXml.files.length > 0) {
            formdata.append("archivoxml", filesXml.files[0]);
        }

        if (filesCfdi && filesCfdi.files.length > 0) {
            formdata.append("archivocfdi", filesCfdi.files[0]);
        }

        if (filesValidacion && filesValidacion.files.length > 0) {
            formdata.append("archivovalideacion", filesValidacion.files[0]);
        }

        $.ajax({
            //url: "http://localhost:46802/CargaComplementosFacturas.ashx",
            url: "http://192.168.11.237/services/CargaComplementosFacturas.ashx",
            type: "POST",
            data: formdata,
            cache: false,
            contentType: false,
            processData: false,
            async: false,
            success: function (d) {
                if (d.EsValido) {
                    // Código en caso de éxito
                    alert('La póliza se guardó correctamente.');
                } else {
                    //console.log(d.Mensaje);
                    alert('La póliza se guardó, pero ocurrio un eror al guardar los complementos de la factura: ' + d.Mensaje);
                }
            },
            error: function (xhr, error, status) {
                // Manejo de errores
            }
        });
    });
}

var DocumentosFactura = function (poliza) {
    var listaArchivos =[];
    var facturas = document.getElementById('TablaFacturas');
    var tbody = facturas.getElementsByTagName('tbody')[0];
    var filas = tbody.getElementsByTagName('tr');

    for (var i = 0; i < filas.length; i++) {
        var uploadXmlElement = "";
        var uploadPdfCfdiElement = "";
        var uploadPdfValidacionElement = "";
        var elementxml = "";
        var elementCfdi = "";
        var elementValidacion = "";
        var celdas = filas[i].getElementsByTagName('td');

            // Obtener los elementos con clases específicas
        uploadXmlElement = filas[i].querySelector('.fileUploadXml.icon-check');
        uploadPdfCfdiElement = filas[i].querySelector('.fileUploadCfdi.icon-check');
        uploadPdfValidacionElement = filas[i].querySelector('.fileUploadValidacion.icon-check');

        // Obtener los inputs de archivo si existen
        if (uploadXmlElement) {
            elementxml = uploadXmlElement.querySelector('input[type="file"]');
            } else {
                console.log('Elemento "uploadXmlElement" no encontrado en la fila ' +(i +1));
}
        if (uploadPdfCfdiElement) {
            elementCfdi = uploadPdfCfdiElement.querySelector('input[type="file"]');
            } else {
            console.log('Elemento "uploadpdfcfdi" no encontrado en la fila ' +(i +1));
            }
        if (uploadPdfValidacionElement) {
            elementValidacion = uploadPdfValidacionElement.querySelector('input[type="file"]');
            } else {
                console.log('Elemento "uploadpdfvalidacion" no encontrado en la fila ' +(i +1));
                }

        // Verificar si al menos uno de los elementos existe
        if (elementxml != "" || elementCfdi != "" || elementValidacion != "") {
            var dataFactura = filas[i].getAttribute('data-factura');
            var uuid = filas[i].getAttribute('data-uuid');
            var proveedorid = filas[i].getAttribute('data-proveedorid');
            listaArchivos[dataFactura] = {
                elementxml: elementxml,
                elementCfdi: elementCfdi,
                elementValidacion: elementValidacion,
                poliza: poliza,
                vueltas: dataFactura,
                uuid: uuid,
                proveedorid : proveedorid,
                uploadXmlElement: uploadXmlElement,
                uploadPdfCfdiElement: uploadPdfCfdiElement,
                uploadPdfValidacionElement: uploadPdfValidacionElement
            };
        }
    }
    subirArchivoDetalle(listaArchivos, poliza.FacturasProveedor);
}

function abrirModal() {
    muestraPopup('ArchivosAdicionales', false);
}

function cerrarModal() {
    var modal = document.getElementById('ArchivosAdicionales');
    modal.style.display = 'none';
}

window.onclick = function (event) {
    var modal = document.getElementById('ArchivosAdicionales');
    if (event.target == modal) {
        modal.style.display = 'none';
    }
};

$('body').on('click', '#ArchivosAdicionales-close', null, function () {
    cerrarPopup('ArchivosAdicionales', false);
});

var cerrarPopup = function (popid, principal) {
    $('#' + popid).addClass('hidden').removeClass('fadeInDown');
    principal = (principal == null ? true : principal);
    if (principal)
        lightboxOff();
};

var muestraPopup = function (popid, principal) {
    principal = (principal == null ? true : principal);
    if (principal)
        lightboxOn();
    var popup = $('#' + popid),
    width = popup.actual('width'),
    left = (window.innerWidth / 2) - (width / 2),
    top = window.innerHeight * 0.1;
    popup.css({ 'position': 'fixed', 'top': top, 'left': left });
    popup.addClass('fadeInDown').removeClass('hidden');
};
var dataid = 0;
$('body').on('click', '#ArchivosAdicionales-btn', null, function () {
    if ($("#cmbArchivos option:selected").text() == "") {
        alert("Dedes seleccionar un documento");
        $("#cmbArchivos").focus();
        return;
    }

    dataid = dataid + 1;
    AgregarDocumentoDetalle();
});

var AgregarDocumentoDetalle = function () {
    
    var parametros = {};
    parametros.DocumentoID = $("#cmbArchivos").val();
    parametros.Documento = $("#cmbArchivos option:selected").text();
    parametros.usuario = getUsuario();

    var tabla = $("#ArchivosAdicionales-Detalle");
    var cuerpoTabla = tabla.find("tbody");
    var nuevaFila = $("<tr data-id= '" + dataid + "' data-Uuid='' data-UltimaAct=''>");
    nuevaFila.append("<td>" + parametros.Documento + "</td>");
    nuevaFila.append("<td class='tdeliminar centrado'><span class='closebtn elimina-com'>X</span></td>");
    
    nuevaFila.append("<td class='centrado' ><div id='uploaddoc' class='fileUpload btnExcel' data-documentoid='" + parametros.DocumentoID + "' style='position: relative; overflow: hidden; margin-top: 7px;'><span>Cargar</span><input type='file' class='upload doc' data-documentoid='" + parametros.DocumentoID + "' style='position: absolute; top: 0; right: 0; margin: 0; padding: 0; font-size: 20px; cursor: pointer; opacity: 0;' multiple=''/></div></td>")
    
    cuerpoTabla.append(nuevaFila);
    $("#cmbArchivos").val("");

    $(".upload.doc").on("change", function () {
        var nombreArchivo = $(this).val();
        if (nombreArchivo != "") {
            var textoBoton = $(this).closest('.fileUpload').find('span');
            if (textoBoton.length > 0) {
                textoBoton.text('');
            }
            $(this).closest('.fileUpload').removeClass('btnExcel').addClass('icon-check');
        } else {
            var fileUpload = $(this).closest('.fileUpload');
            fileUpload.removeClass('icon-check').addClass('btnExcel');

            var nuevoSpan = $('<span>').text('Cargar');
            fileUpload.find('span').remove(); 
            fileUpload.append(nuevoSpan);
        }
    });
};

$('body').on('click', '.tdeliminar', null, function () {
    var dataid = $(this).closest('tr').attr('data-id');
    listArchAd = listArchAd || [];
    listArchAd.push(dataid);
    $(this).closest('tr').remove();
});
var EliminarArchivoAdicional = function() {
    var parametros = {};
    parametros.Archivos = listArchAd;
    parametros.usuario = getUsuario();
    parametros.EmpresaID = amplify.store.sessionStorage("EmpresaID");
    var jsonData = JSON.stringify(parametros);
    $.ajax({
        //url: "http://localhost:46802/EliminarArchivosAdicionales.ashx",
        url: "http://192.168.11.237/services/EliminarArchivosAdicionales.ashx",
        type: "POST",
        contentType: "application/json",
        data: jsonData,
        async: false,
        success: function (data) {
            console.log("Respuesta del servidor:", data);
        },
        error: function (error) {
            console.error("Error en la solicitud:", error);
        }
    });
};

var GuardarArchivosAdicionales = function (poliza) {
    var tabla = $("#ArchivosAdicionales-Detalle");
    var cuerpoTabla = tabla.find("tbody");

    cuerpoTabla.find("tr").each(function (index, fila) {
        var formdata = new FormData();
        formdata.append("FechaPol", $("#txtFechaPol").val());
        formdata.append("TipoPol", $("#ddlTipPol").val());

        if (poliza) {
            formdata.append("polizaID", poliza.Polizaid);
            formdata.append("Folio", poliza.Folio);
        }

        var dataId = $(fila).data("id");
        var documento = $(fila).find("td:first").text();
        var documentoID = $(fila).find(".fileUpload").data("documentoid");

        var nombreArchivoInput = $(fila).find(".upload.doc");
        var nombreArchivo = nombreArchivoInput.val();
        if (nombreArchivo && nombreArchivo.trim() !== "") {
            nombreArchivo = nombreArchivo.replace(/C:\\fakepath\\/i, '');
            console.log("Nombre del archivo:", nombreArchivo);
            var extension = obtenerExtension(nombreArchivo);
            formdata.append("archivo", nombreArchivoInput[0].files[0]);
        }

        var ultimaAct = $(fila).data("ultimaact") != '' ? $(fila).data("ultimaact") : 0;
        var uuid = $(fila).data("uuid") != '' ? $(fila).data("uuid") : "00000000-0000-0000-0000-000000000000";

        formdata.append("ultimaAct", ultimaAct);
        formdata.append("Empresa", amplify.store.sessionStorage("EmpresaID"));
        formdata.append("Documento", documento);
        formdata.append("nombreArchivo", nombreArchivo);
        formdata.append("extension", extension);
        formdata.append("usuario", getUsuario());
        formdata.append("uuid", uuid);

        $.ajax({
            //url: "http://localhost:46802/CargarArchivos.ashx",
            url: "http://192.168.11.237/services/CargarArchivos.ashx",
            type: "POST",
            data: formdata,
            cache: false,
            contentType: false,
            processData: false,
            async: false,
            success: function (d) {
                if (d.EsValido) {
                } else {
                }
            },
            error: function (xhr, error, status) {
            }
        });
    });
}


function obtenerExtension(nombreArchivo) {
    var partes = nombreArchivo.split('.');
    if (partes.length === 1) {
        return ""; 
    }
    return partes.pop().toLowerCase();
}

function construirRuta(fechaPol, tipoPol, poliza) {
    var partesFecha = fechaPol.split("/");
    var fechaObjeto = new Date(partesFecha[2], partesFecha[1] - 1, partesFecha[0]);
    var año = fechaObjeto.getFullYear();
    var mes = fechaObjeto.getMonth() + 1; 
    var día = fechaObjeto.getDate();
    var fecha = (día < 10 ? '0' : '') + día.toString() + (mes < 10 ? '0' : '') + mes.toString() + año.toString();
    var carpeta = fecha + '_' + tipoPol + '_' + poliza.toString() + '_';
    return carpeta;
}
function descargarTodo() {
    //var servicioBaseURL = "http://localhost:46802/";
    var servicioBaseURL = "http://192.168.11.237/services/";
    var Empresa = amplify.store.sessionStorage("EmpresaID");
    var carpeta = construirRuta($('#txtFechaPol').val(), $('#ddlTipPol').val(), $('#AyudaPoliza_Code').val());
    window.location.href = servicioBaseURL + "DescargarArchivosRutaZip.ashx?Empresa=" + Empresa + "&Carpeta=" + carpeta;  
}
function obtenerNombreArchivo(ruta) {
    var partesRuta = ruta.split("\\");
    var nombreArchivo = partesRuta[partesRuta.length - 1];
    return nombreArchivo;
}
var getDatosPoliza = function () {
    var detalle = $('#tablaDetalle tbody tr'),
  len = detalle.length,
  poldetalle = [],
  poliza = {
      Polizaid: $('#HiddenPolizaid').val(),
      EmpresaId: amplify.store.sessionStorage("EmpresaID"),
      Folio: $('#AyudaPoliza').getValuesByCode().Codigo || '0',
      TipPol: $('#ddlTipPol').val(),
      Fechapol: $('#txtFechaPol').val(),
      Concepto: $('#txtConceptoPoliza').val(),
      Importe: $('#tg_cargos').text().replace(/,/g, '') || 0,
      Pendiente: $('#chkPendiente').is(':checked'),
      Usuario: getUsuario(), //parent.document.getElementById("OcultoPropiedades").value, // $('#HiddenUsuario').val(),
      Estatus: $('#HiddenEstatus').val() || 1,
      //Fecha:
      UltimaAct: $('#HiddenUltimaAct').val() || 0,
      Pagoprogramado: $('#chkPagoProgramado').is(':checked'),
      Empresabancoid: $("#ddlBancos").val() == "*" ? null : $("#ddlBancos").val()

  };

    for (var i = 0; i < len; i++) {
        var tr = $(detalle[i]),
    cuenta = {
        Polizadetalleid: tr.attr('data-polizadetalleid'),
        Polizaid: '',
        Cuentaid: tr.attr('data-cuentaid'),
        Cuenta: '',
        CuentaDesc: '',
        TipMov: tr.attr('data-tipmov'),
        Concepto: $('.conceptodet', tr).val(),
        Cantidad: 1,
        Importe: $('.importe', tr).val().replace(/,/g, '') || 0,
        Estatus: tr.attr('data-estatus') || 1,
        Fecha: tr.attr('data-fecha'),
        Usuario: getUsuario(), // parent.document.getElementById("OcultoPropiedades").value, // tr.attr('data-usuario'),
        UltimaAct: tr.attr('data-ultimaact') || 0
    };
        poldetalle.push(cuenta);
    }

    poliza.ListaPolizaDetalle = poldetalle;
    if ($("#chkXML").is(":checked")) {
        poliza.FacturasProveedor = obtenerFacturasProveedor();
    } else {
        poliza.FacturasProveedor = [];
    }

    /*if ($("#chkXMLNomina").is(":checked")) {
        poliza.ListaPolizasNomina = getListadoNomina();
    } else {
        poliza.ListaPolizasNomina = [];
    }*/

    return poliza;
};
var obtenerFacturasProveedor = function () {
    var ListaCatfacturasproveedor = [];
    $("#TablaFacturas tbody tr").each(function () {
        var Facturaproveedorid = $(this).attr("data-Facturaproveedorid");
        var Catfacturasproveedor = {};
        Catfacturasproveedor.Facturaproveedorid = $(this).attr("data-Facturaproveedorid");
        Catfacturasproveedor.Empresaid = amplify.store.sessionStorage("EmpresaID");
        Catfacturasproveedor.Proveedorid = $(this).attr("data-Proveedorid");
        //Catfacturasproveedor.Factura = $(this).attr("data-Factura");
        //Catfacturasproveedor.Subtotal = balor.quitarFormatoMoneda($(this).find(".CSubtotal")[0].innerText);
        //Catfacturasproveedor.Iva = balor.quitarFormatoMoneda($(this).find(".CIva")[0].innerText);
        //Catfacturasproveedor.RetIva = balor.quitarFormatoMoneda($(this).find(".CRetIva")[0].innerText);
        //Catfacturasproveedor.RetIsr = balor.quitarFormatoMoneda($(this).find(".CRetIsr")[0].innerText);
        //Catfacturasproveedor.Total = balor.quitarFormatoMoneda($(this).find(".CTotal")[0].innerText);
        Catfacturasproveedor.Uuid = $(this).attr("data-Uuid");
        //Catfacturasproveedor.Fechatimbre = $(this).attr("data-Fechatimbre");
        //Catfacturasproveedor.Xml = "";

        var dataPathxml = $(this).find(".icon-check.xml").data("path");
        var dataPathcfdi = $(this).find(".icon-check.pdfcfdi").data("path");
        var dataPathvalidacion = $(this).find(".icon-check.pdfvalidacion").data("path");
        var nombreArchivoXml;
        var nombreArchivoCfdi;
        var nombreArchivoValidacion;
        if (dataPathxml !== undefined && dataPathxml !== null && dataPathxml !== "") {
            nombreArchivoXml = obtenerNombreArchivo(dataPathxml);
        }
        if (dataPathcfdi !== undefined && dataPathcfdi !== null && dataPathcfdi !== "") {
            nombreArchivoCfdi = obtenerNombreArchivo(dataPathcfdi);
        }
        if (dataPathvalidacion !== undefined && dataPathvalidacion !== null && dataPathvalidacion !== "") {
            nombreArchivoValidacion = obtenerNombreArchivo(dataPathvalidacion);
        }
        Catfacturasproveedor.nombreArchivoXml = nombreArchivoXml;
        Catfacturasproveedor.nombreArchivoCfdi = nombreArchivoCfdi;// != 'null' ? $(this).attr("data-file") : nombreArchivo;
        Catfacturasproveedor.nombreArchivoValidacion = nombreArchivoValidacion;
        Catfacturasproveedor.Usuario = getUsuario();
        Catfacturasproveedor.Estatus = $(this).attr("data-Estatus");
        Catfacturasproveedor.UltimaAct = $(this).attr("data-UltimaAct");
        ListaCatfacturasproveedor.push(Catfacturasproveedor);
    });
    return ListaCatfacturasproveedor;
};
function readfilesxml(file, files, nomarchivo) {
    var info = file;
    if (files.length <= 0)
        return;
    //Validamos que los archivos seleccionados sean xml
    for (var i = 0; i < files.length; i++) {
        if (files[i].name.toLowerCase().indexOf(".xml") <= 0) {
            alert("El archivo: " + files[i].name + " no es del tipo XML");
            file.querySelector("input[type='file']").value = null;
            if (file) {
                var textoBoton = file.querySelector('span');
                if (textoBoton) {
                    textoBoton.textContent = 'Cargar XML'; // Esto borra el texto dentro del botón
                }
                file.classList.remove('icon-check');
                file.classList.add('btnExcel');
            }
            return;
        }
    }
    if (file) {
        var textoBoton = file.querySelector('span');
        if (textoBoton) {
            textoBoton.textContent = ''; // Esto borra el texto dentro del botón
        }
        file.classList.remove('btnExcel');
        file.classList.add('icon-check');
    }
}

var EliminarDocumentosFacturas = function (facprovid) {
    var parametros = {};
    parametros.facturaprovedor = facprovid;
    parametros.usuario = getUsuario();
    parametros.EmpresaID = amplify.store.sessionStorage("EmpresaID");
    var jsonData = JSON.stringify(parametros);
    $.ajax({
        //url: "http://localhost:46802/EliminarDocumentosFacturas.ashx",
        url: "http://192.168.11.237/services/EliminarDocumentosFacturas.ashx",
        type: "POST",
        contentType: "application/json",
        data: jsonData,
        async: false,
        success: function (data) {
            console.log("Respuesta del servidor:", data);
        },
        error: function (error) {
            console.log("Error en la solicitud:", error);
        }
    });
};

/*function modificarMesYAnio(texto, mesNuevo, anioNuevo) {
    const patron = /(\b(?:enero|febrero|marzo|abril|mayo|junio|julio|agosto|septiembre|octubre|noviembre|diciembre)\b) (\d{4})/i;

    const textoModificado = texto.replace(patron, `${mesNuevo} ${anioNuevo}`);

    return textoModificado;
}*/
function modificarMesYAnio(texto, mesNuevo, anioNuevo) {
    const patron = /(\b(?:enero|febrero|marzo|abril|mayo|junio|julio|agosto|septiembre|octubre|noviembre|diciembre)\b)\s+(\d{4})/i;

    const textoModificado = texto.replace(patron, `${mesNuevo} ${anioNuevo}`);

    return textoModificado;
}

