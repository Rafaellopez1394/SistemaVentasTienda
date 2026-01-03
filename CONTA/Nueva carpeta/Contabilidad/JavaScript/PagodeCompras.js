/// <reference path="../../Base/js/vendor/jquery-1.11.0-vsdoc.js" />
/// <reference path="../../Base/js/plugins.js" />
/// <reference path="../../Base/Scripts/librerias/extensions/jquery.HelpField-1.1.js" />
(function ($, balor, amplify) {
    var msgbx = $('#divmsg');
    var clock = null;
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

 
    $("#btnLimpiar").click(function () {
        window.location.replace("PagodeCompras.aspx");
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

    InstanciarAyudas = function () {
        $('#AyudaProveedores').helpField({
            title: 'Proveedores',
            codeNumeric: true,
            //maxlengthCode: 30,
            enableCode: true,
            enableDescription: false,
            codePaste: false, 
            //nextControlID: 'txtcalle',
            //widthCode: 170,
            //widthDescription: '0',
            requeridField: false,
            cultureResourcesPopUp: {
                popUpHeader: 'Búsqueda de Proveedores'
            },
            findByPopUp: AyudaProveedores_FindByPopUp,
            findByCode: AyudaProveedores_FindByCode,
            onElementFound: AyudaProveedores_onElementFound
        });
    }

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
        DatosFacturas()
    };


    //$('body').on('click', '.AyudaCuentaInterno', null, function (e) {
    //    $("#FacturaActiva").val($(this).parents('tr').attr("data-Uuid"));
    //    $('#AyudaCuenta_HelpButton').click();
    //});

    //$('body').on('focus', '.CCuentaContable', null, function (e) {
    //    $("#FacturaActiva").val($(this).parents('tr').attr("data-Uuid"));
    //});



























    var DatosFacturas = function () {
        var parametros = { Proveedorid: $('#AyudaProveedores').getValuesByCode().ID, Empresaid: amplify.store.sessionStorage("EmpresaID") }
        ejecutaAjax('PagodeCompras.aspx/TraerCatfacturasproveedor', 'POST', parametros, function (d) {
            if (d.d.EsValido) {
                ProcesaDatosFacturas(d.d.Datos);
            } else {
                $('#AyudaProveedores_HelpLoad').css({ "display": "none" });
                showMsg('Alerta', d.d.Mensaje, 'warning');
            }
        }, function (d) {
            showMsg('Alerta', d.responseText, 'error');
        }, true);
    };


    var ProcesaDatosFacturas = function (Datos) {
        var resHtml = '';
        var importetotal = 0;
        for (var i = 0; i < Datos.length; i++) {
            resHtml += '<tr data-Uuid="' + Datos[i].Uuid + '" data-file="' + Datos[i].Rutaxml + '" data-Compraid="' + Datos[i].Compraid + '" data-Facturaproveedorid="' + Datos[i].Facturaproveedorid + '" data-Proveedorid="' + Datos[i].Proveedorid + '" >';
            resHtml += '<td>' + Datos[i].Factura + '</td>';
            resHtml += '<td>' + Datos[i].Fechatimbre + '</td>';
            resHtml += '<td>' + balor.aplicarFormatoMoneda(Datos[i].Total, "$") + '</td>';
            resHtml += '<td>' + balor.aplicarFormatoMoneda(Datos[i].Abonado, "$") + '</td>';
            resHtml += '<td>' + balor.aplicarFormatoMoneda((Datos[i].Total - Datos[i].Abonado), "$") + '</td>';
            resHtml += '<td><input type="text"  class="txtAplicar"/></td>';
            //resHtml += '<td>' + (Datos[i].Uuid == "00000000-0000-0000-0000-000000000000" ? ' <input type="button" class="btn" value ="+" />' : '') + '</td>';
            resHtml += '<td ' + (Datos[i].Uuid == "00000000-0000-0000-0000-000000000000" ? ' class="centrado"><span class="closebtn" style="font-size:22px" >+</span>' : '' ) + '</td>';
            resHtml += '</tr>';
            importetotal += Datos[i].Total;
        }
        $("#TablaFacturas tbody").html(resHtml);
        $(".CCuentaContable").css("display", "inline");
        //$("#ImporteTotal")[0].innerText = balor.aplicarFormatoMoneda(importetotal, "$");
    };

    $('body').on('keypress', '#txtImporte', null, function (e) {
        var code = (e.keyCode ? e.keyCode : e.which);
        if (code === 13) {
            e.preventDefault();
            CalculaTotales();
        }
    });

    $('body').on('keypress', '.txtAplicar', null, function (e) {
        var code = (e.keyCode ? e.keyCode : e.which);
        if (code === 13) {
            e.preventDefault();
            CalculaTotales();
        }
    });

    $('body').on('keypress', '.txtAplicar', null, function (e) {
        focusNextControl(this, e);
    });

    var CalculaTotales = function () {
        if ($("#txtImporte").val() != "")
        {
            var SumImporte = 0;
            $("#TablaFacturas tbody tr").each(function () {
                var input = $(this).find('.txtAplicar');
                var num = (isNaN(parseFloat(input.val())) ? 0 : parseFloat(input.val()));
                SumImporte += num
            });
            $("#ImporteAplicado").text(balor.aplicarFormatoMoneda(SumImporte, "$"));
            var imp = (isNaN(parseFloat($("#txtImporte").val())) ? 0 : parseFloat($("#txtImporte").val()))
            $("#ImporteDiferencia").text(balor.aplicarFormatoMoneda(imp - SumImporte, "$"));
        }
    };


    $('body').on('click', '#btnGuardar', null, function (e) {

    });

    var llenarComboEmpresas = function (d) {
        if (d.d.EsValido) {
            $('#ddlBancos').llenaCombo(d.d.Datos);
        }
    };

    var traerCuentasEmpresa = function (empresaid) {
        ejecutaAjax('PagodeCompras.aspx/TraerCuentasPorEmpresa', 'POST', { empresaid: empresaid }, function (d) {
            if (d.d.EsValido) {
                var lista = d.d.Datos,
            len = lista.length,
            cuentas = [],
            i = 0,
            infoBanco;

                for (; i < len; i++) {
                    infoBanco = lista[i];
                    cuentas[i] = { id: infoBanco.Empresabancoid + '|' + infoBanco.Moneda , nombre: infoBanco.Banco + ' - ' + infoBanco.CuentaCheques };
                }
                $('#ddlBancos').llenaCombo(cuentas);
                TipoMoneda();
            }
        }, function (d) {
            showMsg('Alerta', d.responseText, 'error');
        }, true);
    };

    var TipoMoneda = function () {
        var valor = $("#ddlBancos").val().toString().split('|')
        $("#txtTipoMoneda").val((valor[1] == "P" ? "Pesos" : "Dolares"))
        $("#divTipoCambio").css("display", (valor[1] == "P" ? "none" : "inline"))
        if (valor[1] == "P")
            $("#txtTipoCambio").attr("disabled");
        else
            $("#txtTipoCambio").removeAttr("disabled");
    };

    $("#ddlBancos").change(function () {
        TipoMoneda();
    });



    $(document).ready(function () {
        if (amplify.store.sessionStorage("UsuarioID") == null || amplify.store.sessionStorage("UsuarioID") == "") {
            window.location.replace("../../login.aspx");
        }
        $("#Menu").MostrarMenuBar(JSON.parse(amplify.store.sessionStorage("Funcionalidades")), amplify.store.sessionStorage("Nombre"), amplify.store.sessionStorage("NombreEmpresa"), amplify.store.sessionStorage("UsuarioID"));
        $('.popup').setDraggable('.popup-body, .popup-footer, .close');
        ActualizaMenuBar();
        InstanciarAyudas();
        AplicarEstilo();
        traerCuentasEmpresa(amplify.store.sessionStorage("EmpresaID"));
    });
}(jQuery, window.balor, window.amplify));