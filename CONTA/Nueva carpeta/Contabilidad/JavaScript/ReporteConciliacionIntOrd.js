(function ($, balor, amplify, moment) {
    'use strict';
    var msgbx = $('#divmsg');

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

    var focusNextControl = function (control, e) {
        var code = (e.keyCode ? e.keyCode : e.which);
        if (code === 13) {
            var inputs = $(control).closest('form')
                    .find('input[type=text]:enabled, input[type=time]:enabled, input[type=password]:enabled, input[type=button]:enabled, input[type=checkbox]:enabled, input[type=radio]:enabled, input[type=date]:enabled, input[type=tel]:enabled, input[type=email]:enabled, input[type=number]:enabled, select:enabled, textarea:enabled')
                    .not('input[readonly=readonly], fieldset:hidden *, *:hidden, .nofocus');
            inputs.eq(inputs.index(control) + 1).focus();
            e.preventDefault();
        }
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

    var body = $('body');
    body.on('keypress', '.entero', function (e) {
        return balor.onlyInteger(e);
    });

    body.on('keypress', 'input:not(input[type=button])', function (e) {
        focusNextControl(this, e);
    });

    $('select').on('keypress', null, function (e) {
        focusNextControl(this, e);
    });

    var validaSesion = function () {
        if (!amplify.store.sessionStorage("UsuarioID")) {
            window.location.replace("../../login.aspx");
        }
    };










    //var ayudaGerenteFindByCode = function () {
    //    var parametros = { value: JSON.stringify($('#ayudaEqVenta').getValuesByCode()) };

    //    ejecutaAjax('carteraclientes.aspx/AyudaGerenteFindByCode', 'POST', parametros, function (d) {
    //        $('#ayudaEqVenta').showFindByCode(d.d.Datos);
    //    }, function (d) {
    //        showMsg('Alerta', d.responseText, 'error');
    //    }, true);
    //};

    //var ayudaGerenteFindByPopUp = function () {
    //    var parametros = { value: JSON.stringify($('#ayudaEqVenta').getValuesByPopUp()) };

    //    ejecutaAjax('carteraclientes.aspx/AyudaGerenteFindByPopUp', 'POST', parametros, function (d) {
    //        if (d.d.EsValido) {
    //            $('#ayudaEqVenta').showFindByPopUp(d.d.Datos);
    //        } else {
    //            showMsg('Alerta', d.d.Mensaje, 'warning');
    //        }
    //    }, function (d) {
    //        showMsg('Alerta', d.responseText, 'error');
    //    }, true);
    //};

    //var iniciaAyudas = function () {
    //    $('#ayudaEqVenta').helpField({
    //        title: 'Equipo de venta',
    //        codeNumeric: true,
    //        maxlengthCode: 6,
    //        enableCode: true,
    //        enableDescription: false,
    //        codePaste: false,
    //        nextControlID: 'ayudaClientesIni',
    //        //widthDescription: '0',
    //        requeridField: false,
    //        cultureResourcesPopUp: {
    //            popUpHeader: 'Búsqueda de equipo de venta'
    //        },
    //        findByPopUp: ayudaGerenteFindByPopUp,
    //        findByCode: ayudaGerenteFindByCode
    //    });
    //};
    var ValidaImprimir = function () {
        if ($("#fltr-fecha").val() == "") {
            showMsg('Alerta', "Ingrese la fecha inicial del reporte", 'warning');
            $("#fltr-fecha").focus();
            return false;
        }
        if ($("#fltr-fecha2").val() == "") {
            showMsg('Alerta', "Ingrese la fecha final del reporte", 'warning');
            $("#fltr-fecha2").focus();
            return false;
        }

        return true;
    };


    var imprimir = function () {
        var parametros = "&empresaid=" + $("#fltr-empresa").val();
        parametros += "&fecha=" + $("#fltr-fecha").val();
        parametros += "&fecha2=" + $("#fltr-fecha2").val();
        parametros += "&clienteini=" + $('#ayudaClientesIni').getValuesByCode().Codigo;
        parametros += "&clientefin=" + $('#ayudaClientesFin').getValuesByCode().Codigo;
        parametros += "&Formato=" + $("input[type='radio'][name='timprimir']:checked").val();
        window.open("../../Base/Formas/ReporteProduccion.aspx?Formato=0&Nombre=ConciliacionIntOrd" + parametros, 'w_PopRepConciliacionIntOrd', 'toolbar=no,location=no,directories=no,status=no,menubar=no,scrollbars=yes,resizable=yes,width=760,height=560');
    };





    body.on('click', '#btnimprimir', null, function () {
        if (ValidaImprimir()) {
            imprimir();
        }

    });

    body.on('click', '#btnLimpiar', null, function () {
        window.location.replace('ReporteCobranza.aspx');
    });







    var ayudaClientesIniFindByCode = function () {
        var parametros = { value: JSON.stringify($('#ayudaClientesIni').getValuesByCode()) };
        ejecutaAjax('../../Analisis/Formas/FuncionesGenericas.aspx/AyudaCliente_FindByCode', 'POST', parametros, function (d) {
            $('#ayudaClientesIni').showFindByCode(d.d.Datos);
        }, function (d) {
            showMsg('Alerta', d.responseText, 'error');
        }, true);
    };

    var ayudaClientesIniFindByPopUp = function () {
        var parametros = { value: JSON.stringify($('#ayudaClientesIni').getValuesByPopUp()) };
        ejecutaAjax('../../Analisis/Formas/FuncionesGenericas.aspx/AyudaCliente_FindByPopUp', 'POST', parametros, function (d) {
            if (d.d.EsValido) {
                $('#ayudaClientesIni').showFindByPopUp(d.d.Datos);
            } else {
                showMsg('Alerta', d.d.Mensaje, 'warning');
            }
        }, function (d) {
            showMsg('Alerta', d.responseText, 'error');
        }, true);
    };

    var ayudaClientesIni_onElementFound = function () {
        //        var parametros = { clienteid: $('#ayudaClientes').getValuesByCode().ID };
        //        ejecutaAjax('CatalogoClientes.aspx/TraerClienteCompleto', 'POST', parametros, function (d) {
        //            ConsultarProspectoDelCliente(d.d.Datos.Cliente[0].Prospectoid);
        //        }, function (d) {
        //            showMsg('Alerta', d.responseText, 'error');
        //        }, true);
    };

    var iniciaAyudas = function () {
        $('#ayudaClientesIni').helpField({
            title: 'Cliente',
            codeNumeric: true,
            maxlengthCode: 6,
            enableCode: true,
            enableDescription: false,
            codePaste: false,
            //nextControlID: '',
            //widthDescription: '0',
            requeridField: false,
            cultureResourcesPopUp: {
                popUpHeader: 'Búsqueda de Clientes'
            },
            findByPopUp: ayudaClientesIniFindByPopUp,
            findByCode: ayudaClientesIniFindByCode,
            onElementFound: ayudaClientesIni_onElementFound
        });

        $('#ayudaClientesFin').helpField({
            title: 'Cliente',
            codeNumeric: true,
            maxlengthCode: 6,
            enableCode: true,
            enableDescription: false,
            codePaste: false,
            //nextControlID: '',
            //widthDescription: '0',
            requeridField: false,
            cultureResourcesPopUp: {
                popUpHeader: 'Búsqueda de Clientes'
            },
            findByPopUp: ayudaClientesFinFindByPopUp,
            findByCode: ayudaClientesFinFindByCode,
            onElementFound: ayudaClientesFin_onElementFound
        });
    };




    var ayudaClientesFinFindByCode = function () {
        var parametros = { value: JSON.stringify($('#ayudaClientesFin').getValuesByCode()) };
        ejecutaAjax('../../Analisis/Formas/FuncionesGenericas.aspx/AyudaCliente_FindByCode', 'POST', parametros, function (d) {
            $('#ayudaClientesFin').showFindByCode(d.d.Datos);
        }, function (d) {
            showMsg('Alerta', d.responseText, 'error');
        }, true);
    };

    var ayudaClientesFinFindByPopUp = function () {
        var parametros = { value: JSON.stringify($('#ayudaClientesFin').getValuesByPopUp()) };
        ejecutaAjax('../../Analisis/Formas/FuncionesGenericas.aspx/AyudaCliente_FindByPopUp', 'POST', parametros, function (d) {
            if (d.d.EsValido) {
                $('#ayudaClientesFin').showFindByPopUp(d.d.Datos);
            } else {
                showMsg('Alerta', d.d.Mensaje, 'warning');
            }
        }, function (d) {
            showMsg('Alerta', d.responseText, 'error');
        }, true);
    };

    var ayudaClientesFin_onElementFound = function () {
        //        var parametros = { clienteid: $('#ayudaClientes').getValuesByCode().ID };
        //        ejecutaAjax('CatalogoClientes.aspx/TraerClienteCompleto', 'POST', parametros, function (d) {
        //            ConsultarProspectoDelCliente(d.d.Datos.Cliente[0].Prospectoid);
        //        }, function (d) {
        //            showMsg('Alerta', d.responseText, 'error');
        //        }, true);
    };







    $(document).ready(function () {
        validaSesion();
        $("#Menu").MostrarMenuBar(JSON.parse(amplify.store.sessionStorage("Funcionalidades")), amplify.store.sessionStorage("Nombre"), amplify.store.sessionStorage("NombreEmpresa"), amplify.store.sessionStorage("UsuarioID"));
        iniciaAyudas();
        $("#ayudaClientesIni_LabelHelp").text("Cliente Inicio");
        $("#ayudaClientesFin_LabelHelp").text("Cliente Fin");
    });

    setInterval(function () {
        validaSesion();
    }, 1000);

    var llenarComboEmpresas = function (d) {
        if (d.d.EsValido) {
            $('#fltr-empresa').llenaCombo(d.d.Datos);
        }
    };

    ejecutaAjax('../../Cartera/Formas/carteraclientes.aspx/TraerEmpresas', 'POST', null, llenarComboEmpresas, null, false);
    $('#fltr-empresa').focus();
}(jQuery, window.balor, window.amplify, window.moment));