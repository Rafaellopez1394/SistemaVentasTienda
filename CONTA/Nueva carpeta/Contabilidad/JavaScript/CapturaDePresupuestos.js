/// <reference path="../../Base/js/vendor/jquery-1.11.0-vsdoc.js" />
/// <reference path="../../Base/js/plugins.js" />
/// <reference path="../../Base/Scripts/librerias/extensions/jquery.HelpField-1.1.js" />
(function ($, balor, amplify) {
    var msgbx = $('#divmsg');
    var clock = null;
    var CesionesFiltro = [];

    var showMsg = function (titulo, mensaje, tipoMensaje) {
        msgbx.mostrarMensaje(titulo, mensaje, tipoMensaje);
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

    //    var getVendedor = function () {
    //        return amplify.store.sessionStorage("VendedorID");
    //    };

    var llenarComboEmpresas = function (d) {
        if (d.d.EsValido) {
            $('#ddl-empresa').llenaCombo(d.d.Datos);
        }
    };

    var ayudaVendedorFindByCode = function () {
        var parametros = { value: JSON.stringify($('#ayudaVendedor').getValuesByCode()) },
        tipo = "1";  //balor.radioGetCheckedValue('tipovendedor'),
        webmethod = tipo === '1' ? 'AyudaVendedorFindByCode' : 'AyudaGerenteFindByCode';

        ejecutaAjax('../../Analisis/Formas/vendedores.aspx/' + webmethod, 'POST', parametros, function (d) {
            $('#ayudaVendedor').showFindByCode(d.d.Datos);
        }, function (d) {
            showMsg('Alerta', d.responseText, 'error');
        }, true);
    };

    var ayudaVendedorFindByPopUp = function () {
        var parametros = { value: JSON.stringify($('#ayudaVendedor').getValuesByPopUp()) },
        tipo = "1"; // balor.radioGetCheckedValue('tipovendedor'),
        webmethod = tipo === '1' ? 'AyudaVendedorFindByPopUp' : 'AyudaGerenteFindByPopUp';

        ejecutaAjax('../../Analisis/Formas/vendedores.aspx/' + webmethod, 'POST', parametros, function (d) {
            if (d.d.EsValido) {
                $('#ayudaVendedor').showFindByPopUp(d.d.Datos);
            } else {
                showMsg('Alerta', d.d.Mensaje, 'warning');
            }
        }, function (d) {
            showMsg('Alerta', d.responseText, 'error');
        }, true);
    };

    var ayudaVendedorOnElementFound = function () {
        //        var tipo = balor.radioGetCheckedValue('tipovendedor'),
        //        id = $('#ayudaVendedor').getValuesByCode().ID,
        //        parametros = tipo === '1' ? { vendedorid: id} : { gerenteid: id },
        //        webmethod = tipo === '1' ? 'TraerVendedor' : 'TraerGerente';

        //        ejecutaAjax('vendedores.aspx/' + webmethod, 'POST', parametros, function (d) {
        //            if (d.d.EsValido) {
        //                if (tipo === '1') {
        //                    muestraVendedor(d.d.Datos);
        //                } else {
        //                    muestraGerente(d.d.Datos);
        //                }
        //                $('#ayudaVendedor').EnableHelpField(false);
        //                //$('input[name=tipovendedor]').attr('disabled', 'disabled');
        //                $('body').off('click', '#ayudaVendedor_HelpButton');
        //                elemById('txtappaterno').focus();
        //            } else {
        //                showMsg('Alerta', d.d.Mensaje, 'warning');
        //            }
        //        }, function (d) {
        //            showMsg('Alerta', d.responseText, 'error');
        //        }, true);
    };


    var iniciaAyudas = function () {
        $('#ayudaVendedor').helpField({
            title: 'Vendedor',
            codeNumeric: true,
            maxlengthCode: 6,
            enableCode: true,
            enableDescription: false,
            codePaste: false,
            nextControlID: 'txtAnio',
            //widthDescription: '0',
            requeridField: false,
            cultureResourcesPopUp: {
                popUpHeader: 'Búsqueda'
            },
            findByPopUp: ayudaVendedorFindByPopUp,
            findByCode: ayudaVendedorFindByCode,
            onElementFound: ayudaVendedorOnElementFound
        });
    };

    $('body').on('keypress', '.KeyPressGrid', function (e) {
        focusNextControl(this, e);
    });

    $('input').not('input[type=button]').on('keypress', null, function (e) {
        focusNextControl(this, e);
    });

    $('body').on('click', '#btnConsultar', null, function () {
        if ($('#ayudaVendedor').getValuesByCode().ID != "") {
            if ($("#txtAnio").val() != "") {
                TraerPresupuestos();
                $("#dviCuerpo").css({ "display": "inline" })
            }
            else {
                showMsg('Alerta', 'Proporcione el año', 'warning');
            }
        }
        else {
            showMsg('Alerta', 'Proporcione el vendedor', 'warning');
        }
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

    var TraerPresupuestos = function () {
        $("#tbPresupuestos tbody").html("");
        var parametros = {};
        ShowLightBox(true, "Espere porfavor");
        parametros.empresa = $('#ddl-empresa option:selected').val();
        parametros.anio = $("#txtAnio").val()
        parametros.vendedor = $('#ayudaVendedor').getValuesByCode().ID;
        ejecutaAjax('CapturaDePresupuestos.aspx/TraerPresupuestos', 'POST', parametros, function (d) {
            ShowLightBox(false, "");
            if (d.d.EsValido) {
                ProcesarGridPresupuestos(d.d.Datos)
            } else {
                showMsg('Alerta', d.d.Mensaje, 'error');
            }
        }, function (d) {
            ShowLightBox(false, "");
            showMsg('Alerta', d.responseText, 'error');
        }, true);

    };

    var ProcesarGridPresupuestos = function (Datos) {
        var resHtml = '';
        var old = "odd";
        var entra = false;
        var noReg = Object.keys(Datos).length;
        for (var i = 0; i < 12; i++) {
            resHtml += '<tr class="' + (entra ? old : '') + '" data-UltimaAct="' + (noReg > 0 ? Datos[i].UltimaAct : '0') + '" data-PresupuestoOperacionID="' + (noReg > 0 ? Datos[i].Presupuestooperacionid : '00000000-0000-0000-0000-000000000000') + '" data-VendedorID="' + (noReg > 0 ? Datos[i].Vendedorid : $('#ayudaVendedor').getValuesByCode().ID) + '" data-EmpresaID="' + (noReg > 0 ? Datos[i].Empresaid : $('#ddl-empresa option:selected').val()) + '" data-idMes = "' + (noReg > 0 ? Datos[i].Mes : i + 1) + '" >';
            resHtml += '<td>' + nombreMes((noReg > 0 ? Datos[i].Mes : i + 1)) + '</td>';
            resHtml += '<td><input type="text" class="txtCtesAutorizados KeyPressGrid Cero vasioCero inputcorto entero"  value="' + (noReg > 0 ? Datos[i].Clientesautorizados : 0) + '"/></td>';
            resHtml += '<td><input type="text" class="txtCtesNvosOperando KeyPressGrid Cero vasioCero inputcorto entero"  value="' + (noReg > 0 ? Datos[i].Clientesnuevosoperando : 0) + '"/></td>';
            resHtml += '<td><input type="text" class="txtColocacionCteNvo KeyPressGrid Moneda vasioMoneda inputcorto" onfocus="this.value;" value="' + balor.aplicarFormatoMoneda((noReg > 0 ? Datos[i].Colocacionclientenuevo : 0), '$') + '"/></td>';
            resHtml += '<td><input type="text" class="txtColocacionAumentos KeyPressGrid Moneda vasioMoneda inputcorto" onfocus="this.value;" value="' + balor.aplicarFormatoMoneda((noReg > 0 ? Datos[i].Colocacionaumentos : 0), '$') + '"/></td>';
            resHtml += '<td><input type="text" class="txtColocacionOperativa KeyPressGrid Moneda vasioMoneda inputcorto" onfocus="this.value;" value="' + balor.aplicarFormatoMoneda((noReg > 0 ? Datos[i].Colocacionoperativa : 0), '$') + '"/></td>';
            resHtml += '</tr>';
            entra = (!entra ? true : false);
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
        GuardarPresupuestos();
    });

    var GuardarPresupuestos = function () {
        var lstPropuestas = [];
        $('#tbPresupuestos tbody tr').each(function () {
            var datosPresupuesto = {}
            datosPresupuesto.Presupuestooperacionid = $(this).attr("data-PresupuestoOperacionID");
            datosPresupuesto.Vendedorid = $(this).attr("data-VendedorID");
            datosPresupuesto.Empresaid = $(this).attr("data-EmpresaID");
            datosPresupuesto.Año = $("#txtAnio").val();
            datosPresupuesto.Mes = $(this).attr("data-idMes");
            datosPresupuesto.Clientesautorizados = $(this).find('.txtCtesAutorizados').val();
            datosPresupuesto.Clientesnuevosoperando = $(this).find('.txtCtesNvosOperando').val();
            datosPresupuesto.Colocacionclientenuevo = balor.quitarFormatoMoneda($(this).find('.txtColocacionCteNvo').val());
            datosPresupuesto.Colocacionaumentos = balor.quitarFormatoMoneda($(this).find('.txtColocacionAumentos').val());
            datosPresupuesto.Colocacionoperativa = balor.quitarFormatoMoneda($(this).find('.txtColocacionOperativa').val());
            datosPresupuesto.Estatus = 1;
            datosPresupuesto.Usuario = getUsuario();
            datosPresupuesto.UltimaAct = $(this).attr("data-UltimaAct")

            lstPropuestas.push(datosPresupuesto);
        });


        ShowLightBox(true, "Espere porfavor");
        ejecutaAjax('CapturaDePresupuestos.aspx/GuardarPropuestas', 'POST', { value: JSON.stringify(lstPropuestas) }, function (d) {
            ShowLightBox(false, "");
            if (d.d.EsValido) {

                //                $('#TablaCesiones tbody tr').each(function () {
                //                    $(this).find('.txtFechaCompromiso').val("")
                //                    $(this).find('.txtComentario').val("")
                //                });
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
        window.location.replace('CapturaDePresupuestos.aspx');
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
        ejecutaAjax('ReporteGerencial.aspx/TraerEmpresas', 'POST', null, llenarComboEmpresas, null, false);

        $('#ddl-empresa').focus();
    });
} (jQuery, window.balor, window.amplify));