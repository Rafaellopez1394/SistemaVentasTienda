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

var cerrarPopup = function (popid, principal) {
    $('#' + popid).addClass('hidden').removeClass('fadeInDown');
    principal = (principal == null ? true : principal);
    if (principal)
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

var formatoFechaJSON = function (f) {
    var fi = f.replace(/\/Date\((-?\d+)\)\//, "$1");
    var fecha = new Date(parseInt(fi));

    var dd = ('0' + fecha.getDate().toString()).slice(-2)
    var mm = ('0' + (fecha.getMonth() + 1).toString()).slice(-2)
    var y = fecha.getFullYear().toString();
    return dd + '/' + mm + '/' + y;
};

//************************************** EVENTOS AYUDA CUENTA INICIAL Y CUENTA FINAL
var getUsuario = function () {
    return amplify.store.sessionStorage("Usuario");
};

var getVendedor = function () {
    return amplify.store.sessionStorage("VendedorID");
};
var TraerPolizas = function (año, mes, pendiente, tip_pol) {
    ShowLightBox(true, "Espere un momento porfavor");
    var parametros = {};
    parametros.empresaid = amplify.store.sessionStorage("EmpresaID");
    parametros.año = año;
    parametros.mes = mes;
    parametros.pendiente = pendiente;
    parametros.tip_pol = tip_pol;
    ejecutaAjax('DetallePolizas.aspx/TraerPolizas', 'POST', parametros, function (d) {        
        if (d.d.EsValido) {
            ProcesarGridPolizas(d.d.Datos);
        } else {
            showMsg('Error', d.d.Mensaje, 'error');
        }
        ShowLightBox(false, "");
    }, function (d) {
        ShowLightBox(false, "");
        showMsg('Alerta', d.responseText, 'error');
    }, true);
};

var ProcesarGridPolizas = function (Datos) {
    var resHtml = '';
    var old = "odd";
    var entra = false;
    for (var i = 0; i < Datos.length; i++) {
        resHtml += '<tr class="' + (entra ? old : "") + ' rprospecto" data-id="' + Datos[i].Polizaid + '" data-Fec_pol="' + Datos[i].Fec_pol + '" data-TipPol="' + Datos[i].TipPol + '" data-Folio="' + Datos[i].Folio + '" >';
        resHtml += '<td style="text-align:center;">' + Datos[i].Fec_pol + '</td>';
        resHtml += '<td style="text-align:center;">' + Datos[i].TipPol + '</td>';
        resHtml += '<td style="text-align:center;">' + Datos[i].Folio + '</td>';
        resHtml += '<td>' + Datos[i].Concepto + '</td>';
        resHtml += '<td style="text-align:right;">' + balor.aplicarFormatoMoneda(Datos[i].Importe) + '</td>';
        resHtml += '<td style="text-align:center;">' + Datos[i].Usuario + '</td>';
        resHtml += '<td class="CIcon">' + (Datos[i].Tienefacturas ? '<img class="DescargarArchivo" src="../../Base/img/ok32.png" />' : '') + '</td>';
        resHtml += '<td class="CIcon">' + (Datos[i].TieneComplemento == 1 ? '<img class="DescargarComplemento" src="../../Base/img/ok32.png" />' : (Datos[i].TieneComplemento == 2 ? '<img class="DescargarComplemento" src="../../Base/img/ok32b.png" />' : '')) + '</td>';
        resHtml += '<td class="CIcon">' + (Datos[i].TieneComplementoDeNomina == 1 ? '<img class="DescargarComplementoNomina" src="../../Base/img/ok32.png" />' : '') + '</td>';
        resHtml += '<td style="text-align:center;"><a class="svbtn show-print"><i class="svicon"></i> Imprimir</a></td>';
        resHtml += '<td style="text-align:center;"><a class="svbtn show-process"><i class="svicon"></i> Ver</a></td>';
        resHtml += '</tr>';
        entra = (!entra ? true : false);
    }
    $("#TablaPolizas tbody").html(resHtml);
};

var TraerDatosPoliza = function (Polizaid) {
    var parametros = "Polizaid=" + Polizaid;
    window.location.replace("../../Contabilidad/Formas/CapturaPolizas.aspx?" + parametros);
};


var ImpPoliza = function (Polizaid) {
    imprimirPoliza(Polizaid);
};
var imprimirPoliza = function (id) {
    var parametros = '&polizaid=' + id;
    parametros += "&Ingles=0";
    window.open('../../Base/Formas/ReporteProduccion.aspx?Formato=0&Nombre=ReportePoliza' + parametros, 'w_PopImprimir', 'toolbar=no,location=no,directories=no,status=no,menubar=no,scrollbars=yes,resizable=yes,width=760,height=560');
};

var llenaComboTipPol = function () {
    ejecutaAjax('CapturaPolizas.aspx/TraerTipPol', 'POST', {}, function (d) {
        if (d.d.EsValido) {
            var tippol = d.d.Datos.reverse(),
            i = tippol.length,
            opt = '<option value="">TODOS</option>';
            for (i; i--;) {
                opt += '<option value="' + tippol[i].id + '">' + tippol[i].nombre + '</option>';
            }
            $('#ddlTipPol').html(opt);
        } else {
            $('#ddlTipPol').html('');
            showMsg('Alerta', d.d.Mensaje, 'warning');
        }
        ShowLightBox(false, "");
    }, function (d) {
        ShowLightBox(false, "");
        showMsg('Alerta', d.responseText, 'error');
    }, true);
};

var ProcesarGridFacturas = function (Datos) {
    var resHtml = '';
    var importetotal = 0;
    for (var i = 0; i < Datos.length; i++) {
        //resHtml += '<tr class="rgFactura" data-Factura="' + Datos[i].Factura + '" data-Uuid="' + Datos[i].Uuid + '" data-Fechatimbre="' + Datos[i].Fechatimbre + '"  data-file="' + Datos[i].Rutaxml + '" data-Compraid="' + Datos[i].Compraid + '" data-Facturaproveedorid="' + Datos[i].Facturaproveedorid + '" data-Proveedorid="' + Datos[i].Proveedorid + '" data-UltimaAct="' + Datos[i].UltimaAct + '" data-Estatus="' + Datos[i].Estatus + '" >';
        resHtml += '<tr class="rgFactura" data-Uuid="' + Datos[i].UUID + '" data-file="' + Datos[i].NomArchivo + '" data-Facturaproveedorid="' + Datos[i].Facturaproveedorid + '" data-UltimaAct="' + Datos[i].UltimaAct + '" data-Proveedorid="' + Datos[i].Proveedorid + '">';
        resHtml += '<td >' + Datos[i].Factura + '</td>';
        resHtml += '<td>' + formatoFechaJSON(Datos[i].Fechatimbrado) + '</td>';
        resHtml += '<td style="text-align:right;" class="CSubtotal">' + balor.aplicarFormatoMoneda(Datos[i].Subtotal, "$") + '</td>';
        resHtml += '<td style="text-align:right;" class="CIva">' + balor.aplicarFormatoMoneda(Datos[i].Iva, "$") + '</td>';
        resHtml += '<td style="text-align:right;" class="CRetIva">' + balor.aplicarFormatoMoneda(Datos[i].RetIva, "$") + '</td>';
        resHtml += '<td style="text-align:right;" class="CRetIsr">' + balor.aplicarFormatoMoneda(Datos[i].RetIsr, "$") + '</td>';
        resHtml += '<td style="text-align:right;" class="CTotal">' + balor.aplicarFormatoMoneda(Datos[i].Total, "$") + '</td>';
        resHtml += '</tr>';
        importetotal += Datos[i].Subtotal;
    }
    //$("#lblSubtotalXML")[0].innerText = "Subtotal: " + balor.aplicarFormatoMoneda(importetotal, "$");
    $("#TablaFacturas tbody").html(resHtml);
    calcularTotales();
};
var calcularTotales = function () {
    var totalSubtotal = 0;
    var totalIva = 0;
    var totalRetIva = 0;
    var totalRetIsr = 0;
    var totalTotal = 0;

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
};

var ProcesarGridFacturasIF = function (Datos) {
    var resHtml = '';
    var importetotal = 0;
    if (Datos.Factura != null) {
        //resHtml += '<tr class="rgFactura" data-Factura="' + Datos[i].Factura + '" data-Uuid="' + Datos[i].Uuid + '" data-Fechatimbre="' + Datos[i].Fechatimbre + '"  data-file="' + Datos[i].Rutaxml + '" data-Compraid="' + Datos[i].Compraid + '" data-Facturaproveedorid="' + Datos[i].Facturaproveedorid + '" data-Proveedorid="' + Datos[i].Proveedorid + '" data-UltimaAct="' + Datos[i].UltimaAct + '" data-Estatus="' + Datos[i].Estatus + '" >';
        //resHtml += '<tr class="rgFactura" data-Uuid="' + Datos.UUID + '" data-file="' + Datos.NomArchivo + '" data-Facturaproveedorid="' + Datos.Facturaproveedorid + '" data-UltimaAct="' + Datos.UltimaAct + '" data-Proveedorid="' + Datos.Proveedorid + '">';
        resHtml += '<tr class="rgFacturaPoliza" data-Factura="' + Datos.Factura + '" data-Uuid="' + Datos.UUID + '" data-Fechatimbre="' + Datos.FechaTimbrado + '" data-UltimaAct="' + Datos.UltimaAct + '" data-Estatus="' + Datos.Estatus + '" >';
        resHtml += '<td >' + Datos.Factura + '</td>';
        resHtml += '<td>' + Datos.FechaTimbrado + '</td>';
        resHtml += '<td style="text-align:right;" class="CSubtotal">' + balor.aplicarFormatoMoneda(Datos.SubTotal, "$") + '</td>';
        resHtml += '<td style="text-align:right;" class="CIva">' + balor.aplicarFormatoMoneda(Datos.Iva, "$") + '</td>';
        resHtml += '<td style="text-align:right;" class="CRetIva">' + balor.aplicarFormatoMoneda(Datos.RetIva, "$") + '</td>';
        resHtml += '<td style="text-align:right;" class="CRetIsr">' + balor.aplicarFormatoMoneda(0, "$") + '</td>';
        resHtml += '<td style="text-align:right;" class="CTotal">' + balor.aplicarFormatoMoneda(Datos.Total, "$") + '</td>';
        resHtml += '</tr>';
        importetotal += Datos.SubTotal;
    }
    //$("#lblSubtotalXML")[0].innerText = "Subtotal: " + balor.aplicarFormatoMoneda(importetotal, "$");
    $("#TablaFacturas tbody").html(resHtml);
    calcularTotales();
};

$('body').on('keypress', '#txtPeriodo', null, function (e) {
    return balor.onlyNumeric(e);
});

$('body').on('click', '.show-process', null, function () {
    TraerDatosPoliza($(this.parentNode.parentNode).attr("data-id"));
});

$('body').on('click', '.DescargarArchivo', null, function () {
    ShowLightBox(true, "Espere un momento porfavor");
    var parametros = {};    
    parametros.polizaid = $(this.parentNode.parentNode).attr("data-id");
    var tipPol = $(this.parentNode.parentNode).attr("data-TipPol");
    $("#lblTitulo")[0].innerText = "Poliza: " + $(this.parentNode.parentNode).attr("data-Folio") + " Tipo: " + tipPol + " Fecha: " + $(this.parentNode.parentNode).attr("data-Fec_pol");
    if (tipPol != "IF") {
        ejecutaAjax('DetallePolizas.aspx/Traerfacturas', 'POST', parametros, function (d) {
            if (d.d.EsValido) {
                ProcesarGridFacturas(d.d.Datos);
                muestraPopup('Facturas');
            } else {
                showMsg('Error', d.d.Mensaje, 'error');
            }
            ShowLightBox(false, "");
        }, function (d) {
            ShowLightBox(false, "");
            showMsg('Alerta', d.responseText, 'error');
        }, true);
    }
    else 
    {
        ejecutaAjax('DetallePolizas.aspx/TraerfacturasIF', 'POST', parametros, function (d) {
            if (d.d.EsValido) {
                ProcesarGridFacturasIF(d.d.Datos);
                muestraPopup('Facturas');
            } else {
                showMsg('Error', d.d.Mensaje, 'error');
            }
            ShowLightBox(false, "");
        }, function (d) {
            ShowLightBox(false, "");
            showMsg('Alerta', d.responseText, 'error');
        }, true);
    }
});

$('body').on('click', '.DescargarComplemento', null, function () {
    ShowLightBox(true, "Espere un momento porfavor");
    var parametros = {};
    parametros.polizaid = $(this.parentNode.parentNode).attr("data-id");
    $("#lblTituloComplementos")[0].innerText = "Poliza: " + $(this.parentNode.parentNode).attr("data-Folio") + " Tipo: " + $(this.parentNode.parentNode).attr("data-TipPol") + " Fecha: " + $(this.parentNode.parentNode).attr("data-Fec_pol");
    ejecutaAjax('DetallePolizas.aspx/TraerfacturasComplementos', 'POST', parametros, function (d) {
            if (d.d.EsValido) {
                ProcesarGridFacturasComplementos(d.d.Datos);
                muestraPopup('Complementos');
            } else {
                showMsg('Error', d.d.Mensaje, 'error');
            }
            ShowLightBox(false, "");
        }, function (d) {
            ShowLightBox(false, "");
            showMsg('Alerta', d.responseText, 'error');
        }, true);
});

$('body').on('click', '.DescargarComplementoNomina', null, function () {
    ShowLightBox(true, "Espere un momento porfavor");
    var parametros = {};
    parametros.polizaid = $(this.parentNode.parentNode).attr("data-id");
    //$("#lblTituloComplementos")[0].innerText = "Poliza: " + $(this.parentNode.parentNode).attr("data-Folio") + " Tipo: " + $(this.parentNode.parentNode).attr("data-TipPol") + " Fecha: " + $(this.parentNode.parentNode).attr("data-Fec_pol");
    ejecutaAjax('DetallePolizas.aspx/TraerComplementosNomina', 'POST', parametros, function (d) {
        if (d.d.EsValido) {
            ProcesarGridComplementosNominas(d.d.Datos);
            muestraPopup('Complementosnomina');
        } else {
            showMsg('Error', d.d.Mensaje, 'error');
        }
        ShowLightBox(false, "");
    }, function (d) {
        ShowLightBox(false, "");
        showMsg('Alerta', d.responseText, 'error');
    }, true);
});

var ProcesarGridFacturasComplementos = function (Datos) {
    var resHtml = '';
    var importetotal = 0;
    for (var i = 0; i < Datos.length; i++) {
        resHtml += '<tr class="rgFacturaComplemento" data-Uuid="' + Datos[i].UUID + '">';
        resHtml += '<td>' + Datos[i].Factura + '</td>';
        resHtml += '<td>' + Datos[i].MetodoPago + '</td>';
        resHtml += '<td>' + formatoFechaJSON(Datos[i].Fecha) + '</td>';
        resHtml += '<td style="text-align:right;" class="CSubtotal">' + balor.aplicarFormatoMoneda(Datos[i].Subtotal, "$") + '</td>';
        resHtml += '<td style="text-align:right;" class="CIva">' + balor.aplicarFormatoMoneda(Datos[i].Iva, "$") + '</td>';
        resHtml += '<td style="text-align:right;" class="CTotal">' + balor.aplicarFormatoMoneda(Datos[i].Total, "$") + '</td>';
        resHtml += '<td style="text-align:right;" class="CIcon">' + (Datos[i].Complemento == 1 ? '<img class="DescargarArchivo" src="../../Base/img/ok16.png" />' : '<img class="DescargarArchivo" src="../../Base/img/ok16b.png" />') + '</td>';
        resHtml += '</tr>';
        importetotal += Datos[i].Subtotal;
    }
    //$("#lblSubtotalComplementos")[0].innerText = "Subtotal: " + balor.aplicarFormatoMoneda(importetotal, "$");
    $("#TablaComplementos tbody").html(resHtml);
    calcularTotalesC();
};
var calcularTotalesC = function () {
    var totalSubtotal = 0;
    var totalIva = 0;
    var totalTotal = 0;

    $("#TablaComplementos tbody tr").each(function () {
        totalSubtotal += balor.quitarFormatoMoneda($(this).find('.CSubtotal').text());
        totalIva += balor.quitarFormatoMoneda($(this).find('.CIva').text());
        totalTotal += balor.quitarFormatoMoneda($(this).find('.CTotal').text());
    });
    // Actualizar los valores de las celdas de totales
    $('.totalc-subtotal').text(balor.aplicarFormatoMoneda(totalSubtotal, "$"));
    $('.totalc-iva').text(balor.aplicarFormatoMoneda(totalIva, "$"));
    $('.totalc-total').text(balor.aplicarFormatoMoneda(totalTotal, "$"));
};

var ProcesarGridComplementosNominas = function (Datos) {
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
        resHtml += '<td style="text-align:right;" class="CTotal totalpercepciones">' + balor.aplicarFormatoMoneda(Datos[i].TotalPercepciones, "$") + '</td>';

        resHtml += '<td style="text-align:right;" class="CTotal isrmensual">' + balor.aplicarFormatoMoneda(Datos[i].IsrMensual, "$") + '</td>';
        resHtml += '<td style="text-align:right;" class="CTotal imss">' + balor.aplicarFormatoMoneda(Datos[i].Imss, "$") + '</td>';
        resHtml += '<td style="text-align:right;" class="CTotal infonavit">' + balor.aplicarFormatoMoneda(Datos[i].Infonavit, "$") + '</td>';
        resHtml += '<td style="text-align:right;" class="CTotal totaldeducciones">' + balor.aplicarFormatoMoneda(Datos[i].TotalDeducciones, "$") + '</td>';

        resHtml += '<td style="text-align:right;" class="CTotal total">' + balor.aplicarFormatoMoneda(Datos[i].Total, "$") + '</td>';        
        resHtml += '</tr>';
        subtotal += Datos[i].Subtotal;
        totalPercepciones += Datos[i].TotalPercepciones;
        totalDeducciones += Datos[i].TotalDeducciones;
        total += Datos[i].Total;

    }
    /*$("#lblImporteSubtotalXMLNomina")[0].innerText = balor.aplicarFormatoMoneda(subtotal, "$");
    $("#lblImportePercepciones")[0].innerText = balor.aplicarFormatoMoneda(totalPercepciones, "$");
    $("#lblImporteDeducciones")[0].innerText = balor.aplicarFormatoMoneda(totalDeducciones, "$");
    $("#lblImporteTotal")[0].innerText = balor.aplicarFormatoMoneda(total, "$");*/
    $("#TablaFacturasNomina tbody").html(resHtml);
    calcularTotalesNomina();
};
var calcularTotalesNomina = function () {
    var nomina_sueldo = 0;
    var nomina_premioasistencia = 0;
    var nomina_premiopuntualidad = 0;
    var nomina_totalpercepciones = 0;
    var nomina_isrretenido = 0;
    var nomina_imss = 0;
    var nomina_infonavit = 0;
    var nomina_totaldeducciones = 0;
    var nomina_total = 0;


    //XML de Nómina
    $("#TablaFacturasNomina tbody tr").each(function () {
        nomina_sueldo += balor.quitarFormatoMoneda($(this).find('.sueldo').text());
        nomina_premioasistencia += balor.quitarFormatoMoneda($(this).find('.premioasistencia').text());
        nomina_premiopuntualidad += balor.quitarFormatoMoneda($(this).find('.premiopuntualidad').text());
        nomina_totalpercepciones += balor.quitarFormatoMoneda($(this).find('.totalpercepciones').text());
        nomina_isrretenido += balor.quitarFormatoMoneda($(this).find('.isrmensual').text());
        nomina_imss += balor.quitarFormatoMoneda($(this).find('.imss').text());
        nomina_infonavit += balor.quitarFormatoMoneda($(this).find('.infonavit').text());
        nomina_totaldeducciones += balor.quitarFormatoMoneda($(this).find('.totaldeducciones').text());
        nomina_total += balor.quitarFormatoMoneda($(this).find('.total').text());
    });

    // Actualizar los valores de las celdas de totales nomina
    $('.totaln-sueldo').text(balor.aplicarFormatoMoneda(nomina_sueldo, "$"));
    $('.totaln-premioasistencia').text(balor.aplicarFormatoMoneda(nomina_premioasistencia, "$"));
    $('.totaln-premiopuntualidad').text(balor.aplicarFormatoMoneda(nomina_premiopuntualidad, "$"));
    $('.totaln-persepciones').text(balor.aplicarFormatoMoneda(nomina_totalpercepciones, "$"));
    $('.totaln-isrmes').text(balor.aplicarFormatoMoneda(nomina_isrretenido, "$"));
    $('.totaln-imss').text(balor.aplicarFormatoMoneda(nomina_imss, "$"));
    $('.totaln-infonavit').text(balor.aplicarFormatoMoneda(nomina_infonavit, "$"));
    $('.totaln-deducciones').text(balor.aplicarFormatoMoneda(nomina_totaldeducciones, "$"));
    $('.totaln-total').text(balor.aplicarFormatoMoneda(nomina_total, "$"));
};

var aplicaEstiloComplementosNomina = function(){
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
        
    //Deducciones                
    $("#TablaFacturasNomina > thead > tr:eq(1) > th:eq(10)").css({ "background-color": "#EEDC82", "border-color": "#000", "color": "#000", "border": "1px solid #000", "font-weight": "lighter" });
    $("#TablaFacturasNomina > thead > tr:eq(1) > th:eq(11)").css({ "background-color": "#EEDC82", "border-color": "#000", "color": "#000", "border": "1px solid #000", "font-weight": "lighter" });
    $("#TablaFacturasNomina > thead > tr:eq(1) > th:eq(12)").css({ "background-color": "#EEDC82", "border-color": "#000", "color": "#000", "border": "1px solid #000", "font-weight": "lighter" });
    $("#TablaFacturasNomina > thead > tr:eq(1) > th:eq(13)").css({ "background-color": "#EEDC82", "border-color": "#000", "color": "#000", "border": "1px solid #000", "font-weight": "lighter" });

    $("#TablaFacturasNomina > thead > tr:eq(0) > th:eq(3)").css({ "border-right": "0px solid #fff !important" });
}

$('body').on('click', '#Facturas-close', null, function () {
    cerrarPopup('Facturas');
});

$('body').on('click', '#Complementos-close', null, function () {
    cerrarPopup('Complementos');
});

$('body').on('click', '#Complementosnomina-close', null, function () {
    cerrarPopup('Complementosnomina');
});



$('body').on('click', '#btnConsultar', null, function () {
    if ($("#txtPeriodo").val().length < 6) {
        alert("ingrese al año y mes a consultar");
        $("#txtPeriodo").focus();
        return;
    }

    
    TraerPolizas($("#txtPeriodo").val().substring(0, 4), $("#txtPeriodo").val().substring(4, 6), $("#chkPendiente").is(":checked"), $("#ddlTipPol").val());

});

$('body').on('click', '.show-print', null, function () {
    ImpPoliza($(this.parentNode.parentNode).attr("data-id"));
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
    llenaComboTipPol();
    TraerPolizas(0, 0, false, "");
    aplicaEstiloComplementosNomina();
});
