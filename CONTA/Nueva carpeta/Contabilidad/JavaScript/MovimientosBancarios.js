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
        $('#ddl-empresa').llenaCombo(d.d.Datos);
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
    //**************************************EVENTOS AYUDA CUENTA INICIAL Y CUENTA FINAL
var getUsuario = function () {
    return amplify.store.sessionStorage("Usuario");
};

var getVendedor = function () {
    return amplify.store.sessionStorage("VendedorID");
};

var TraerMovimientosBancarios = function () {
    escondeMsg();
    var parametros = { empresaid: $('#ddl-empresa option:selected').val() };
    ejecutaAjax('MovimientosBancarios.aspx/TraerMvtosBancosPorEmpresa', 'POST', parametros, function (d) {
        if (d.d.EsValido) {
            MuestraDivBancos(d.d.Datos);
        } else {
            showMsg('Error', d.d.Mensaje, 'error');
        }
    }, function (d) {
        showMsg('Alerta', d.responseText, 'error');
    }, true);
};

var MuestraDivBancos = function (Datos) {
    var resHtml = '';
    for (var i = 0; i < Datos.length; i++) {
        resHtml += '<a href="javascript:TraerDetalleMovimientosBancarios(\'' + Datos[i].BancoID + '\');" titile="' + Datos[i].Banco + '" class="subtitulo" >' + Datos[i].Banco + '</a>';
    }
    $("#divBancos").html(resHtml);
};

var TraerDetalleMovimientosBancarios = function (BancoID) {
    ShowLightBox(true, "Espere un momento porfavor");
    var parametros = { empresaid: $('#ddl-empresa option:selected').val(), bancoid: BancoID };
    ejecutaAjax('MovimientosBancarios.aspx/TraerMovimientosContablesBancarios', 'POST', parametros, function (d) {
        ShowLightBox(false, "");
        if (d.d.EsValido) {
            if (d.d.Datos.length > 0)
                $("#divEncabezado").html('<h2>Detalle de movimientos del banco ' + d.d.Datos[0].Banco + '</h2>');
            else $("#divEncabezado").html('<h2>No se encontraron movimientos para el banco seleccionado</h2>');
            ProcesarGridMovimientosBancarios(d.d.Datos);
        } else {
            showMsg('Error', d.d.Mensaje, 'error');
        }
    }, function (d) {
        ShowLightBox(false, "");
        showMsg('Alerta', d.responseText, 'error');
    }, true);
};

var ProcesarGridMovimientosBancarios = function (Datos) {
    var resHtml = '';
    var old = "odd";
    var entra = false;
    for (var i = 0; i < Datos.length; i++) {
        resHtml += '<tr class="' + (entra ? old : "") + ' rprospecto" data-id="' + Datos[i].AcvGralID + '" >';
        resHtml += '<td>' + Datos[i].Fec_Pol + '</td>';
        resHtml += '<td>' + Datos[i].Tip_Pol + '</td>';
        resHtml += '<td>' + Datos[i].Codigo + '</td>';        
        resHtml += '<td>' + Datos[i].Beneficiario + '</td>';
        resHtml += '<td>' + Datos[i].Importe + '</td>';
        resHtml += '<td>' + Datos[i].Usuario + '</td>';
        resHtml += '<td>' + Datos[i].Flujo + '</td>';
        resHtml += '<td>' + Datos[i].TM + '</td>';
        resHtml += '<td>' + Datos[i].Cheque + '</td>';
        resHtml += '<td><a class="svbtn show-print"><i class="svicon"></i> Imprimir</a></td>';
        resHtml += '<td><a class="svbtn show-process"><i class="svicon"></i> Ver</a></td>';
        resHtml += '</tr>';
        entra = (!entra ? true : false);
    }
    $("#TablaMovtoBco tbody").html(resHtml);
};

var TraerDatosPoliza = function (AcvGralID) {
    var parametros = { empresaid: amplify.store.sessionStorage("EmpresaID"), acvgralid: AcvGralID };
    ejecutaAjax('MovimientosBancarios.aspx/TraerDatosPoliza', 'POST', parametros, function (d) {
        ShowLightBox(false, "");
        if (d.d.EsValido) {
            if (d.d.Datos[0].MuestraPoliza) {
                var parametros = "Polizaid=" + d.d.Datos[0].Polizaid;
                window.location.replace("../../Contabilidad/Formas/CapturaPolizas.aspx?" + parametros);                
            } else {
                showMsg('Advertencia de seguridad de informacion', 'Para poder ver el detalle de esta poliza te debes logear con la empresa seleccionada en el combo', 'warning');
            }
        } else {
            showMsg('Error', d.d.Mensaje, 'error');
        }
    }, function (d) {
        ShowLightBox(false, "");
        showMsg('Alerta', d.responseText, 'error');
    }, true);
};


var ImpPoliza = function (AcvGralID) {
    var parametros = { empresaid: amplify.store.sessionStorage("EmpresaID"), acvgralid: AcvGralID };
    ejecutaAjax('MovimientosBancarios.aspx/TraerDatosPoliza', 'POST', parametros, function (d) {
        if (d.d.EsValido) {
            imprimirPoliza(d.d.Datos[0].Polizaid);
        } else {
            showMsg('Error', d.d.Mensaje, 'error');
        }
    }, function (d) {
        ShowLightBox(false, "");
        showMsg('Alerta', d.responseText, 'error');
    }, true);
};
var imprimirPoliza = function (id) {
    var parametros = '&polizaid=' + id;
    parametros += "&Ingles=0";
    window.open('../../Base/Formas/ReporteProduccion.aspx?Formato=0&Nombre=ReportePoliza' + parametros, 'w_PopImprimir', 'toolbar=no,location=no,directories=no,status=no,menubar=no,scrollbars=yes,resizable=yes,width=760,height=560');
};
$('body').on('click', '.show-process', null, function () {
    TraerDatosPoliza($(this.parentNode.parentNode).attr("data-id"));
});

$('body').on('click', '.show-print', null, function () {
    ImpPoliza($(this.parentNode.parentNode).attr("data-id"));
});



$('body').on('change', '#ddl-empresa', null, function () {
    $("#TablaMovtoBco tbody").html('');
    $("#divEncabezado").html('');
    TraerMovimientosBancarios();
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
};

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
    ejecutaAjax('Polizas.aspx/TraerEmpresas', 'POST', null, llenarComboEmpresas, null, false);
    TraerMovimientosBancarios();
});
