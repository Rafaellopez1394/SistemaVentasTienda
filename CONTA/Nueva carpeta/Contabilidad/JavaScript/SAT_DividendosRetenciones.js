/// <reference path="../../Base/js/vendor/jquery-1.11.0-vsdoc.js" />
/// <reference path="../../Base/js/plugins.js" />
/// <reference path="../../Base/Scripts/librerias/extensions/jquery.HelpField-1.1.js" />
(function ($, balor, amplify) {
    //*******************************************************************************F U N C I O N E S   G E N E R A L E S*******************************************************************************************
    'use strict';
    var msgbx = $('#divmsg');
    var body = $('body');

    var showMsg = function (titulo, mensaje, tipoMensaje) {
        msgbx.mostrarMensaje(titulo, mensaje, tipoMensaje);
    };

    var hideMsg = function () {
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
        top = window.innerHeight * 0.1;

        popup.css({ 'position': 'fixed', 'top': top, 'left': left });
        popup.addClass('fadeInDown').removeClass('hidden');
    };

    var cerrarPopup = function (popid) {
        $('#' + popid).addClass('hidden').removeClass('fadeInDown');
        lightboxOff();
    };

    var getUsuario = function () {
        return amplify.store.sessionStorage("Usuario");
    };

    var elemById = function (id) {
        return document.getElementById(id);
    };

    var focusNextControl = function (control, e) {
        var code = (e.keyCode ? e.keyCode : e.which);
        if (code === 13) {
            var inputs = $(control).closest('form').find('input[type=text]:enabled, input[type=time]:enabled, input[type=password]:enabled, input[type=button]:enabled, input[type=checkbox]:enabled, input[type=radio]:enabled, input[type=date]:enabled, input[type=tel]:enabled, input[type=email]:enabled, input[type=number]:enabled, select:enabled, textarea:enabled').not('input[readonly=readonly], fieldset:hidden *, *:hidden');
            inputs.eq(inputs.index(control) + 1).focus();
            e.preventDefault();
        }
    };

    var fechaActual = function () {
        var fecha = new Date(), dd = ('0' + fecha.getDate().toString()).slice(-2), mm = ('0' + (fecha.getMonth() + 1).toString()).slice(-2), y = fecha.getFullYear().toString();
        return dd + '/' + mm + '/' + y;
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

    var ayudaRetencionFindByCode = function () {
        var payuda = $('#ayudaRetencion').getValuesByCode();
        payuda.ID = $("#ddlEmpresa").val();
        var parametros = { value: JSON.stringify(payuda) };
        ejecutaAjax('SAT_DividendosRetenciones.aspx/ayudaRetencion_FindByCode', 'POST', parametros, function (d) {
            $('#ayudaRetencion').showFindByCode(d.d.Datos);
        }, function (d) {
            showMsg('Alerta', d.responseText, 'error');
        }, true);
    };

    var ayudaRetencionFindByPopUp = function () {
        var payuda = $('#ayudaRetencion').getValuesByPopUp();
        payuda.ID = $("#ddlEmpresa").val();
        var parametros = { value: JSON.stringify(payuda) };
        ejecutaAjax('SAT_DividendosRetenciones.aspx/ayudaRetencion_FindByPopUp', 'POST', parametros, function (d) {
            if (d.d.EsValido) {
                $('#ayudaRetencion').showFindByPopUp(d.d.Datos);
            } else {
                showMsg('Alerta', d.d.Mensaje, 'warning');
            }
        }, function (d) {
            showMsg('Alerta', d.responseText, 'error');
        }, true);
    };

    var ayudaRetencion_onElementFound = function () {
        ejecutaAjax('SAT_DividendosRetenciones.aspx/traerInfoRetencion', 'POST', { retencionid: $('#ayudaRetencion').getValuesByCode().ID }, function (d) {
            if (d.d.EsValido) {
                muestraInfoRetencion(d.d.Datos);
            } else {
                showMsg('Alerta', d.d.Mensaje, 'warning');
            }
        }, function (d) {
            showMsg('Alerta', d.responseText, 'error');
        }, true);
    };

    var muestraInfoRetencion = function (retencion) {
        $("#txtfecha").val(retencion.Fecharet);
        $("#ddlRetencion").val(retencion.Cveretenc);

        if (retencion.Cveretenc == 14)
            $("#divTipoDividendo").show();

        $("#ddlDividendo").val(retencion.Cvetipdivoutil);
        $("input[name=rdNacionalidad][value=" + retencion.Nacionalidad + "]").prop('checked', true);

        if (retencion.Nacionalidad == "Nacional") {
            $("#DivNacional").show();
            $("#DivExtranjero").hide();
        } else {
            $("#DivNacional").hide();
            $("#DivExtranjero").show();
        }


        $("#txtRfc").val(retencion.Nacrfcrecep);
        $("#txtNombreRazonSocial").val(retencion.Nacnomdenrazsocr);
        $("#txtIDFiscal").val(retencion.Extnumregidtrib);
        $("#txtNombreRazonSocialExt").val(retencion.Extnomdenrazsocr);
        $("#txtEjercicio").val(retencion.Ejerc);
        $("#txtMesIni").val(retencion.Mesini);
        $("#txtMesFin").val(retencion.Mesfin);
        $("#txtTotalOperacion").val(balor.aplicarFormatoMoneda(retencion.Montototoperacion, "$"));
        $("#txtImporteGravado").val(balor.aplicarFormatoMoneda(retencion.Montototgrav, "$"));
        $("#txtImporteExcento").val(balor.aplicarFormatoMoneda(retencion.Montototexent, "$"));
        $("#txtImporteRetenido").val(balor.aplicarFormatoMoneda(retencion.Montototret, "$"));
        $("#ddlTipoSociedad").val(retencion.Tiposocdistrdiv);
        $("#MontISRAcredRetMexico").val(balor.aplicarFormatoMoneda(retencion.Montisracredretmexico, "$"));
        $("#MontISRAcredRetExtranjero").val(balor.aplicarFormatoMoneda(retencion.Montisracredretextranjero, "$"));
        $("#MontISRAcredNal").val(balor.aplicarFormatoMoneda(retencion.Montisracrednal, "$"));
        $("#MontDivAcumNal").val(balor.aplicarFormatoMoneda(retencion.Montdivacumnal, "$"));
        $("#Montdivacumext").val(balor.aplicarFormatoMoneda(retencion.Montisracredretmexico, "$"));
        $("#Proporcionrem").val(retencion.Proporcionrem);
        $("#hiddenRetencionID").val(retencion.Timbradoretencionid);
        $("#btnTimbrar").attr("disabled", "disabled");
        $("#btnImprimir").removeAttr("disabled");

    };

    var iniciaAyudas = function () {
        $('#ayudaRetencion').helpField({
            title: 'Retención',
            codeNumeric: true,
            maxlengthCode: 6,
            enableCode: true,
            enableDescription: false,
            codePaste: false,
            //widthDescription: '0',
            requeridField: false,
            cultureResourcesPopUp: {
                popUpHeader: 'Búsqueda de Retenciones'
            },
            findByPopUp: ayudaRetencionFindByPopUp,
            findByCode: ayudaRetencionFindByCode,
            onElementFound: ayudaRetencion_onElementFound
        });
    };

    var llenarComboEmpresas = function (d) {
        if (d.d.EsValido) {
            $('#ddlEmpresa').llenaCombo(d.d.Datos, null, amplify.store.sessionStorage("EmpresaID"));
        }
    };

    var llenarComboRetencion = function (d) {
        if (d.d.EsValido) {
            $('#ddlRetencion').llenaCombo(d.d.Datos, null);
        }
    };

    var llenarComboDividendo = function (d) {
        if (d.d.EsValido) {
            $('#ddlDividendo').llenaCombo(d.d.Datos, null);
        }
    };


    body.on('change', '#ddlRetencion', function () {
        $("#ddlDividendo").val("");
        if ($(this).val() == "14") {
            $("#divTipoDividendo").show();
        } else {
            $("#divTipoDividendo").hide();
        }
    });

    body.on('click', 'input:radio[name=rdNacionalidad]', function () {
        $("#txtRfc,#txtNombreRazonSocial,#txtIDFiscal,#txtNombreRazonSocialExt").val(); 
        if ($("input:radio[name=rdNacionalidad]:checked").val() == "Nacional") {
            $("#DivNacional").show();
            $("#DivExtranjero").hide();
        } else {
            $("#DivNacional").hide();
            $("#DivExtranjero").show();
        }
    });

    body.on('keypress', '.Moneda,.Numerico', null, function (e) {
        return balor.onlyNumeric(e);
    });

    body.on('blur', '.Moneda', null, function (e) {
        $(this).val(balor.aplicarFormatoMoneda($(this).val(), "$"));
    });

    body.on('focus', '.Moneda,.Numerico', null, function () {
        $(this).val(balor.quitarFormatoMoneda($(this).val()));
        $(this).select();
    });

    body.on('keypress', 'input:not(input[type=button])', function (e) {
        focusNextControl(this, e);
    });

    body.on('click', '#btnTimbrar', function () {
        if (ValidarGuardar()) {
            if (confirm('¿Desea generar el Timbre del comprobante?')) {
                GuardarTimbrado();
            }
        }
    });

    body.on('click', '#btnLimpiar', function () {
        window.location.replace('SAT_DividendosRetenciones.aspx');
    });

    body.on('click', '#btnImprimir', function () {
        var parametros = "&retencionid=" + $("#hiddenRetencionID").val();
        window.open("../../Base/Formas/ReporteProduccion.aspx?Formato=0&Nombre=TimbradoRetenciones" + parametros, 'w_PopRetencionesYPagos' + $("#hiddenRetencionID").val(), 'toolbar=no,location=no,directories=no,status=no,menubar=no,scrollbars=yes,resizable=yes,width=760,height=560');
    });

    var GuardarTimbrado = function () {
        var retencion = getEntityTimbradoretencion();
        ShowLightBox(true, "Timbrando retencion porfavor espere");
        ejecutaAjax('SAT_DividendosRetenciones.aspx/GuardarRetencion', 'POST', { value: JSON.stringify(retencion) }, function (d) {
            if (d.d.EsValido) {
                ShowLightBox(false, "");
                showMsg('Alerta', "Se ha generado correctamente el XML del timbrado, favor de revisarlo en tu carpeta donde se encuentran estos comprobantes", 'success');
                $("input:not(input[type=button])").attr("disabled", "disabled");
                $("#btnTimbrar").attr("disabled", "disabled");
                var parametros = "&retencionid=" + d.d.Datos.Timbradoretencionid;
                window.open("../../Base/Formas/ReporteProduccion.aspx?Formato=0&Nombre=TimbradoRetenciones" + parametros, 'w_PopRetencionesYPagos', 'toolbar=no,location=no,directories=no,status=no,menubar=no,scrollbars=yes,resizable=yes,width=760,height=560');
            } else {
                ShowLightBox(false, "");
                showMsg('Alerta', d.d.Mensaje, 'error');
            }
        }, function (d) {
            ShowLightBox(false, "");
            showMsg('Alerta', d.responseText, 'error');
        }, true);
    };

    var getEntityTimbradoretencion = function () {
        var Timbradoretencion = {};

        Timbradoretencion.Empresaid = $("#ddlEmpresa").val();
        Timbradoretencion.Fecharet = $("#txtfecha").val();
        Timbradoretencion.Cveretenc = $("#ddlRetencion").val();
        Timbradoretencion.Descretenc = $("#ddlRetencion option:selected").text();
        Timbradoretencion.Nacionalidad = $("input:radio[name=rdNacionalidad]:checked").val();
        Timbradoretencion.Nacrfcrecep = $("#txtRfc").val();
        Timbradoretencion.Nacnomdenrazsocr = $("#txtNombreRazonSocial").val();
        Timbradoretencion.Extnumregidtrib = $("#txtIDFiscal").val();
        Timbradoretencion.Extnomdenrazsocr = $("#txtNombreRazonSocialExt").val();
        Timbradoretencion.Mesini = $("#txtMesIni").val();
        Timbradoretencion.Mesfin = $("#txtMesFin").val();
        Timbradoretencion.Ejerc = $("#txtEjercicio").val();
        Timbradoretencion.Montototoperacion = balor.quitarFormatoMoneda($("#txtTotalOperacion").val());
        Timbradoretencion.Montototgrav = balor.quitarFormatoMoneda($("#txtImporteGravado").val());
        Timbradoretencion.Montototexent = balor.quitarFormatoMoneda($("#txtImporteExcento").val());
        Timbradoretencion.Montototret = balor.quitarFormatoMoneda($("#txtImporteRetenido").val());
        Timbradoretencion.Usuario = getUsuario();
        Timbradoretencion.Estatus = 1;

        Timbradoretencion.DomicilioFiscalR = $("#txtcodigopostal").val();
        Timbradoretencion.BaseRet = balor.quitarFormatoMoneda($("#txtBaseRetencion").val());        
        Timbradoretencion.MontoRet = balor.quitarFormatoMoneda($("#txtMontoRetenido").val());
        Timbradoretencion.ImpuestoRet = $("#ddlTipoImpuesto").val();
        Timbradoretencion.TipoPagoRet = $("#ddlTipoPagoRetenido").val();

        //Complemento de pagos extranjeros
        if ($("#ddlRetencion").val() == "18") {
        }

        //Complemento de dividendos o retiros de socios
        if ($("#ddlRetencion").val() == "14") {
            Timbradoretencion.Cvetipdivoutil = $("#ddlDividendo").val();
            Timbradoretencion.Montisracredretmexico = balor.quitarFormatoMoneda($("#MontISRAcredRetMexico").val());
            Timbradoretencion.Montisracredretextranjero = balor.quitarFormatoMoneda($("#MontISRAcredRetExtranjero").val());
            Timbradoretencion.Montretextdivext = balor.quitarFormatoMoneda($("#MontRetExtDivExt").val());
            Timbradoretencion.Tiposocdistrdiv = $("#ddlTipoSociedad").val();
            Timbradoretencion.Montisracrednal = balor.quitarFormatoMoneda($("#MontISRAcredNal").val());
            Timbradoretencion.Montdivacumnal = balor.quitarFormatoMoneda($("#MontDivAcumNal").val());
            Timbradoretencion.Montdivacumext = balor.quitarFormatoMoneda($("#MontDivAcumExt").val());
            Timbradoretencion.Proporcionrem = balor.quitarFormatoMoneda($("#Proporcionrem").val());
        };

        return Timbradoretencion;

    };


    var ValidarGuardar = function () {

        if ($("#ddlRetencion").val() == "") {
            showMsg("Alerta", "Debe seleccionar el tipo de retención que va a timbrar", "warning");
            $("#ddlRetencion").focus();
            return false;
        }

        if ($("#ddlRetencion").val() == "14") {
            if ($("#ddlDividendo").val() == "") {
                showMsg("Alerta", "Debe seleccionar el tipo de dividendo que va a timbrar", "warning");
                $("#ddlDividendo").focus();
                return false;
            }
        }
        if ($("input:radio[name=rdNacionalidad]:checked").val() == "Nacional") {
            if ($("#txtRfc").val() == "") {
                showMsg("Alerta", "Debe ingresar el rfc del receptor", "warning");
                $("#txtRfc").focus();
                return false;
            }
            if ($("#txtNombreRazonSocial").val() == "") {
                showMsg("Alerta", "Debe ingresar el nombre del receptor", "warning");
                $("#txtNombreRazonSocial").focus();
                return false;
            }
        } else {

            if ($("#txtIDFiscal").val() == "") {
                showMsg("Alerta", "Debe ingresar el id fiscal del receptor", "warning");
                $("#txtIDFiscal").focus();
                return false;
            }
            if ($("#txtNombreRazonSocialExt").val() == "") {
                showMsg("Alerta", "Debe ingresar el nombre del receptor", "warning");
                $("#txtNombreRazonSocialExt").focus();
                return false;
            }
        }

        if ($("#txtEjercicio").val() == "") {
            showMsg("Alerta", "Debe ingresar el ejercicio", "warning");
            $("#txtEjercicio").focus();
            return false;
        }

        if ($("#txtMesIni").val() == "") {
            showMsg("Alerta", "Debe ingresar el mes inicial", "warning");
            $("#txtMesIni").focus();
            return false;
        }

        if ($("#txtMesFin").val() == "") {
            showMsg("Alerta", "Debe ingresar el mes final", "warning");
            $("#txtMesFin").focus();
            return false;
        }

        if ($("#txtTotalOperacion").val() == "") {
            showMsg("Alerta", "Debe ingresar el total de la operación", "warning");
            $("#txtTotalOperacion").focus();
            return false;
        }

        if ($("#txtImporteGravado").val() == "") {
            showMsg("Alerta", "Debe ingresar el importe gravado de la operación", "warning");
            $("#txtImporteGravado").focus();
            return false;
        }

        if ($("#txtImporteExcento").val() == "") {
            showMsg("Alerta", "Debe ingresar el importe excento de la operación", "warning");
            $("#txtImporteExcento").focus();
            return false;
        }

        if ($("#txtImporteRetenido").val() == "") {
            showMsg("Alerta", "Debe ingresar el importe retenido de la operación", "warning");
            $("#txtImporteRetenido").focus();
            return false;
        }

        if ($("#ddlRetencion").val() == "14") {

            if ($("#MontISRAcredRetMexico").val() == "") {
                showMsg("Alerta", "Debe ingresar el importe MontISRAcredRetMexico de la operación", "warning");
                $("#MontISRAcredRetMexico").focus();
                return false;
            }

            if ($("#MontISRAcredRetExtranjero").val() == "") {
                showMsg("Alerta", "Debe ingresar el importe MontISRAcredRetExtranjero de la operación", "warning");
                $("#MontISRAcredRetExtranjero").focus();
                return false;
            }

            if ($("#MontRetExtDivExt").val() == "") {
                showMsg("Alerta", "Debe ingresar el importe MontRetExtDivExt de la operación", "warning");
                $("#MontRetExtDivExt").focus();
                return false;
            }

            if ($("#MontISRAcredNal").val() == "") {
                showMsg("Alerta", "Debe ingresar el importe MontISRAcredNal de la operación", "warning");
                $("#MontISRAcredNal").focus();
                return false;
            }

            if ($("#MontDivAcumNal").val() == "") {
                showMsg("Alerta", "Debe ingresar el importe MontDivAcumNal de la operación", "warning");
                $("#MontDivAcumNal").focus();
                return false;
            }

            if ($("#MontDivAcumExt").val() == "") {
                showMsg("Alerta", "Debe ingresar el importe MontDivAcumExt de la operación", "warning");
                $("#MontDivAcumExt").focus();
                return false;
            }

        }
        if ($("#ddlRetencion").val() == "18") {
            showMsg("Alerta", "El sistema no esta preparado para timbrar esta opción, favor de pedir el apoyo del area de sistemas", "warning");
            return false;
        }

        return true;


    };

    var llenacomboRegimenFiscal = function () {
        ejecutaAjax('../../Cartera/Formas/ClientesDiversos.aspx/TraerCatalogoRegimenFiscal', "POST", {},
            function (d) {
                var h = "";
                if (d.d.EsValido) {
                    var datos = d.d.Datos;
                    for (var i = 0; i < datos.length; i++) {
                        h += "<option value='" + datos[i].id + "'>" + datos[i].nombre + "</option>";
                    }
                    $("#ddlRegimenFiscal").html(h);
                }
                else {
                    showMsg('Alerta', d.d.Mensaje, 'error');
                }

            },
            function (d) {
                showMsg('Alerta', d.responseText, 'error');
            },
            true);
    };

    
    body.on('click', '#btnGenerarDatos', function () {
                
        var parametros = { "timbreretencionid":  $("#hiddenRetencionID").val() };
        ejecutaAjax('SAT_DividendosRetenciones.aspx/GeneraDatosDeTimbreFiscalDigitalRetenciones', 'POST', parametros, function (d) {
            if (d.d.EsValido) {
                showMsg('success', "Se han generado los datos con éxito", '');
            } else {
                showMsg('Alerta', d.d.Mensaje, 'warning');
            }
        }, function (d) {
            showMsg('Alerta', d.responseText, 'error');
        }, true);
    });

    $(document).ready(function () {
        if (!amplify.store.sessionStorage("UsuarioID")) {
            window.location.replace("../../login.aspx");
        }
        $("#Menu").MostrarMenuBar(JSON.parse(amplify.store.sessionStorage("Funcionalidades")), amplify.store.sessionStorage("Nombre"), amplify.store.sessionStorage("NombreEmpresa"), amplify.store.sessionStorage("UsuarioID"));
        iniciaAyudas();
        elemById('txtfecha').value = fechaActual();
        ejecutaAjax('SAT_DividendosRetenciones.aspx/TraerEmpresas', 'POST', null, llenarComboEmpresas, null, true);
        ejecutaAjax('SAT_DividendosRetenciones.aspx/TraerTipoRetencion', 'POST', null, llenarComboRetencion, null, true);
        ejecutaAjax('SAT_DividendosRetenciones.aspx/TraerTipoDividendo', 'POST', null, llenarComboDividendo, null, true);
        $("#divTipoDividendo").hide();
        $("#DivNacional").show();
        $("#DivExtranjero").hide();
        //llenacomboRegimenFiscal();
        if (getUsuario() == "fmontenegro") {
            $("#btnGenerarDatos").css("display", "inline-block");
        }
        
    });
}(jQuery, window.balor, window.amplify));