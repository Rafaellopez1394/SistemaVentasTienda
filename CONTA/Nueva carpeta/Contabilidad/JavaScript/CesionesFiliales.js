(function ($, balor, amplify) {
    var msgbx = $('#divmsg');
    var iconSearch = '<img alt="search" width="20" height="20" style="cursor:pointer;" class="AyudaCuentaInterno"  src="data:image/png;base64,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" />';

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

    var traerCuentasEmpresa = function (empresaid) {
        ejecutaAjax('CesionesFiliales.aspx/TraerCuentasPorEmpresa', 'POST', { empresaid: empresaid }, function (d) {
            if (d.d.EsValido) {
                var lista = d.d.Datos,
            len = lista.length,
            cuentas = [],
            i = 0,
            infoBanco;

                for (; i < len; i++) {
                    infoBanco = lista[i];
                    cuentas[i] = { id: infoBanco.Empresabancoid + '|' + infoBanco.Moneda, nombre: infoBanco.Banco + ' - ' + infoBanco.CuentaCheques };
                }
                $('#ddlBancos').llenaCombo(cuentas);
            }
        }, function (d) {
            showMsg('Alerta', d.responseText, 'error');
        }, true);
    };


    var ayudaClienteFindByCode = function () {
        var Param = $('#ayudaCliente').getValuesByCode();
        Param.ID = amplify.store.sessionStorage("EmpresaID");
        var parametros = { value: JSON.stringify(Param) };
        ejecutaAjax('CatalogoClientesIntercompañia.aspx/ayudaCliente_FindByCode', 'POST', parametros, function (d) {
            if (d.d.EsValido) {
                $('#ayudaCliente').showFindByCode(d.d.Datos);
            } else {
                $('#ayudaCliente_HelpLoad').css({ "display": "none" });
                showMsg('Alerta', d.d.Mensaje, 'warning');
            }
        }, function (d) {
            showMsg('Alerta', d.responseText, 'error');
        }, true);
    };

    var ayudaClienteFindByPopUp = function () {
        var Param = $('#ayudaCliente').getValuesByPopUp();
        Param.ID = amplify.store.sessionStorage("EmpresaID");
        var parametros = { value: JSON.stringify(Param) };
        ejecutaAjax('CatalogoClientesIntercompañia.aspx/ayudaCliente_FindByPopUp', 'POST', parametros, function (d) {
            if (d.d.EsValido) {
                $('#ayudaCliente').showFindByPopUp(d.d.Datos);
            } else {
                showMsg('Alerta', d.d.Mensaje, 'warning');
            }
        }, function (d) {
            showMsg('Alerta', d.responseText, 'error');
        }, true);
    };

    var ayudaCliente_onElementFound = function () {
        var parametros = { clienteid: $('#ayudaCliente').getValuesByCode().ID };
        ejecutaAjax('CatalogoClientesIntercompañia.aspx/TraerCliente', 'POST', parametros, function (d) {
            if (d.d.EsValido) {
                
            } else {
                showMsg('Alerta', d.d.Mensaje, 'warning');
            }
        }, function (d) {
            showMsg('Alerta', d.responseText, 'error');
        }, true);
    };


    //AYUDA DE CESIONES DE FILIAL
    var ayudaCesionFilialFindByCode = function () {       
    };

    var ayudaCesionFilialFindByPopUp = function () {      
    };

    var ayudaCesionFilial_onElementFound = function () {       
    };

    //AYUDA DE LA CESION DE PAGOS Y DISPOSICIONES

    var ayudaCesionFilial2FindByCode = function () {
    };

    var ayudaCesionFilial2FindByPopUp = function () {
    };

    var ayudaCesionFilial2_onElementFound = function () {
    };

    var iniciaAyudas = function () {
        $('#ayudaCliente').helpField({
            title: 'Cliente',
            codeNumeric: true,
            maxlengthCode: 6,
            enableCode: true,
            enableDescription: false,
            codePaste: false,
            //nextControlID: '',
            //widthDescription: '0',
            requeridField: true,
            cultureResourcesPopUp: {
                popUpHeader: 'Búsqueda de Clientes'
            },
            findByPopUp: ayudaClienteFindByPopUp,
            findByCode: ayudaClienteFindByCode,
            onElementFound: ayudaCliente_onElementFound
        });

        $('#ayudaCesionFilial').helpField({
            title: 'Cesion',
            codeNumeric: true,
            maxlengthCode: 6,
            enableCode: true,
            enableDescription: false,
            codePaste: false,
            //nextControlID: '',
            widthDescription: '0',
            requeridField: true,
            cultureResourcesPopUp: {
                popUpHeader: 'Búsqueda de Cesiones de Filiales'
            },
            findByPopUp: ayudaCesionFilialFindByPopUp,
            findByCode: ayudaCesionFilialFindByCode,
            onElementFound: ayudaCesionFilial_onElementFound
        });

        $('#ayudaCesionFilial2').helpField({
            title: 'Cesion',
            codeNumeric: true,
            maxlengthCode: 6,
            enableCode: true,
            enableDescription: false,
            codePaste: false,
            //nextControlID: '',
            widthDescription: '0',
            requeridField: true,
            cultureResourcesPopUp: {
                popUpHeader: 'Búsqueda de Cesiones de Filiales'
            },
            findByPopUp: ayudaCesionFilial2FindByPopUp,
            findByCode: ayudaCesionFilial2FindByCode,
            onElementFound: ayudaCesionFilial2_onElementFound
        });

    };

    $('body').on('click', 'input[name=tipoContrato]:radio', null, function () {
        if (this.value == "1") {
            $("#lblTipoCredito")[0].innerText = "Tasa Fija Anual";
        } else {
            $("#lblTipoCredito")[0].innerText = "Cetes + Puntos";
        }
    });

    $('body').on('click', 'input[name=tipoOperacion]:radio', null, function () {
        if (this.value == "1") {
            $("#divBanco").css("display", "none");
        } else {
            $("#divBanco").css("display", "inline");
        }
        $("#txtImporteFinanciamiento").focus();
    });

    $('body').on('click', 'input[name=tipoMovimiento]:radio', null, function () {
        if (this.value == "1") {
            $("#divRegistrarContrato").css("display", "inline");
            $("#divDisposicionesPagos").css("display", "none");
        } else {
            $("#divRegistrarContrato").css("display", "none");
            $("#divDisposicionesPagos").css("display", "inline");
        }
    });

    //

    $("#btnLimpiar").click(function () {
        window.location.replace("CesionesFiliales.aspx");
    });

    $('input').not('input[type=button]').on('keypress', null, function (e) {
        focusNextControl(this, e);
    });

    var MostrarPagina = function (url) {
        if (url != "") {
            window.location.replace(url);
        }
    };


    $(document).ready(function () {
        if (amplify.store.sessionStorage("UsuarioID") == null || amplify.store.sessionStorage("UsuarioID") == "") {
            window.location.replace("../../login.aspx");
        }
        $("#Menu").MostrarMenuBar(JSON.parse(amplify.store.sessionStorage("Funcionalidades")), amplify.store.sessionStorage("Nombre"), amplify.store.sessionStorage("NombreEmpresa"), amplify.store.sessionStorage("UsuarioID"));
        $('.popup').setDraggable('.popup-body, .popup-footer, .close');
        traerCuentasEmpresa(amplify.store.sessionStorage("EmpresaID"));
        iniciaAyudas();
    });
}(jQuery, window.balor, window.amplify));