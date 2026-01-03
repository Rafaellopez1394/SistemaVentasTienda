var msgbx = $('#divmsg');

var idpaismexico = "d693195e-0c3c-490f-a664-cba97d790f42";

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

var muestraPopup = function (popid) {
    lightboxOn();
    var popup = $('#' + popid),
    width = popup.actual('width'),
    left = (window.innerWidth / 2) - (width / 2),
    top = window.innerHeight * 0.1;
    popup.css({ 'position': 'fixed', 'top': top, 'left': left });
    popup.addClass('fadeInDown').removeClass('hidden');
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
var limpiarCamposRequeridos = function () {
    var invalidFields = $('#form1').find(":invalid").not('fieldset');
    var validFields = $('#form1').find(":valid");
    invalidFields.removeClass('invalido');
    validFields.removeClass('invalido');
};

var cerrarPopup = function () {
    $('#LibretaDireccion-captura input[type=text], #ClienteNuevo-captura input[type=text]').val('');
    $('#LibretaDireccion-captura, #ClienteNuevo-captura').addClass('hidden').removeClass('fadeInDown');
    //    $('#ClienteNuevo-captura input[type=text]').val('');
    //    $('#ClienteNuevo-captura').addClass('hidden').removeClass('fadeInDown');
    lightboxOff();
};

var getUsuario = function () {
    return amplify.store.sessionStorage("Usuario");
};

var getVendedor = function () {
    return amplify.store.sessionStorage("VendedorID");
};

var esVendedor = function () {
    if (amplify.store.sessionStorage("Vendedor").toUpperCase() == "TRUE")
        return true;
    else
        return false;
};
var elemById = function (id) {
    return document.getElementById(id);
};

var focusNextControl = function (control, e) {
    var code = (e.keyCode ? e.keyCode : e.which);
    if (code === 13) {
        var inputs = $(control).closest('form').find('input[type=text]:enabled, input[type=password]:enabled, input[type=button]:enabled, input[type=checkbox]:enabled, input[type=radio]:enabled, input[type=date]:enabled, input[type=tel]:enabled, input[type=email]:enabled, input[type=number]:enabled, select:enabled').not('input[readonly=readonly], fieldset:hidden *');
        inputs.eq(inputs.index(control) + 1).focus();
        inputs.eq(inputs.index(control) + 1).select();
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

var elemById = function (id) {
    return document.getElementById(id);
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


function llenaDllAño() {
    var h = "";
    var anioinicial = 2018
    var _fecha = new Date();
    for (i = 2018; i <= anioinicial + 1; i++) {
        h += "<option value='" + i + "'>" + i + "</option>";
    }
    $("#ddlAnio").html(h);
    $("#ddlAnio option[value='" + _fecha.getYear() + "']").prop("selected", true);
  
}

function llenaMeses() {
    var _fecha = new Date();
    var h = "";
    h += "<option value='0'>Enero</option>";
    h += "<option value='1'>Febrero</option>";
    h += "<option value='2'>Marzo</option>";
    h += "<option value='3'>Abril</option>";
    h += "<option value='4'>Mayo</option>";
    h += "<option value='5'>Junio</option>";
    h += "<option value='6'>Julio</option>";
    h += "<option value='7'>Agosto</option>";
    h += "<option value='8'>Septiembre</option>";
    h += "<option value='9'>Octubre</option>";
    h += "<option value='10'>Noviembre</option>";
    h += "<option value='11'>Diciembre</option>";
    $("#ddlMes").html(h);
    $("#ddlMes option[value='" + _fecha.getMonth()  + "']").prop("selected", true);

}

$("#ddlAnio").change(function () {
    validaFechaParaGenerar();
});

$("#ddlMes").change(function () {
    validaFechaParaGenerar();
});

$("#btnGenerar").click(function () {
    generarComisiones();
});

$("#btnGuardar").click(function () {
    guardarComisiones();
});

$("#btnImprimir").click(function () {
    imprimirComisiones();
});

$("#btnImprimirTipoVendedor").click(function () {
    imprimirComisionesPorTipoDeVendedor();
});

function validaFechaParaGenerar() {
    var anio = $("#ddlAnio").val();
    var mes = $("#ddlMes").val();
    ejecutaAjax("../../Ventas/formas/GenerarComisiones.aspx/ValidaFecha", "POST", { 'anio': anio, 'mes': mes },
        function (d) {
            if (d.d.EsValido) {
                if (d.d.Datos.Existe == 1) {
                    $("#btnGenerar").prop("disable", "dsable");
                    traeComisiones();
                }
                else {
                    $("#btnGenerar").prop("disable", "");
                    traeComisiones();
                }
            }
        },

        function (d) {

        }, true);
}

function generarComisiones() {
    var anio = $("#ddlAnio").val();
    var mes = $("#ddlMes").val();


    var fechaini = $("#fechaInicio").val();
    var fechafin = $("#fechaFinal").val();
    var gerenteid = null;
    var vendedorid = $("#ddlVendedores option:selected").val();
    var candidatoid = null;

    ShowLightBoxPopUp(true, 'Generando reporte, espere por favor...', 'body');
    ejecutaAjax("../../Contabilidad/formas/ReporteAyudaCombustible.aspx/GenerarReporte", "POST", { 'fechaini': fechaini, 'fechafin': fechafin, 'gerenteid': gerenteid, 'vendedorid': vendedorid, 'candidatoid':candidatoid },
        function (d) {
            if (d.d.EsValido) {

                ShowLightBoxPopUp(false, "", "");
                var h = "";
                var i = 0;

                var _comisionesC1 = d.d.Datos.ComisionesC1;
                var _comisionesB1 = d.d.Datos.ComisionesB1;
                var _comisionesA1B1 = d.d.Datos.ComisionesA1B1
                var _comisionesA1 = d.d.Datos.ComisionesA1;
                var _comisionesGerentes = d.d.Datos.ComisionesGerente;

                h = "";
                var vendedor = "";
                var total = 0;
                var diff = false;

                var _vendedores = [];



                //Comisiones C1


                for (i = 0; i < _comisionesC1.length; i++) {
                    var j = 0;
                    var encontrado = false;
                    for (j = 0; j < _vendedores.length; j++) {
                        if (_vendedores[j] == _comisionesC1[i].Vendedor) {
                            encontrado = true;
                        }
                    }
                    if (!encontrado) {
                        _vendedores.push(_comisionesC1[i].Vendedor);
                    }

                }

                for (j = 0; j < _vendedores.length; j++) {
                    vendedor = _vendedores[j];
                    total = 0;
                    for (i = 0; i < _comisionesC1.length; i++) {
                        if (_comisionesC1[i].Vendedor == vendedor) {
                            h += "<tr ";
                            h += "data-candidatoid='" + _comisionesC1[i].Candidatoid + "' ";
                            h += "data-comisionvendedor='" + _comisionesC1[i].Comisionvendedor + "' ";
                            h += "data-contactoid='" + _comisionesC1[i].Contactoid + "' ";
                            h += "data-incentivoconcepto='" + _comisionesC1[i].Incentivoconcepto + "' ";
                            h += "data-incentivoid='" + _comisionesC1[i].Incentivoid + "' ";
                            h += "data-nombrecandidato='" + _comisionesC1[i].Nombrecandidato + "' ";
                            h += "data-nombrevendedor='" + _comisionesC1[i].Nombrevendedor + "' ";
                            h += "data-nombrevendedorasignado='" + _comisionesC1[i].Nombrevendedorasignado + "' ";
                            h += "data-nombrevendedorasignadoid='" + _comisionesC1[i].Nombrevendedorasignadoid + "' ";
                            h += "data-vendedirid='" + _comisionesC1[i].Vendedorid + "' >";
                            h += "<td>" + _comisionesC1[i].Nombrevendedor + "</td>";
                            h += "<td>" + _comisionesC1[i].Nombrecandidato + "</td>";
                            h += "<td>" + _comisionesC1[i].Nombrevendedorasignado + "</td>";
                            h += "<td align='right'>" + formatMoney(_comisionesC1[i].Comisionvendedor.toFixed(2), ",", ".") + "</td>";
                            h += "</tr>";
                            total += _comisionesC1[i].Comisionvendedor;
                        }
                        
                    }
                    h += "<tr>";
                    h += "<td></td>";
                    h += "<td></td>";
                    h += "<td align='right'></td>";
                    h += "<td align='right'>Total <span style='font-weight:bold'>" + formatMoney(total.toFixed(2), ",", ".") + "</span></td>";
                    h += "</tr>";
                    h += "<tr style='background-color:#CFE9F7'><td></td><td></td><td></td><td></td></tr>"
                }
                $("#tblComisionesC1 tbody").html(h);
                




                total = 0;
                _vendedores.length = 0;
                h = "";
                for (i = 0; i < _comisionesB1.length; i++) {
                    var j = 0;
                    var encontrado = false;
                    for (j = 0; j < _vendedores.length; j++) {
                        if (_vendedores[j] == _comisionesB1[i].Vendedor) {
                            encontrado = true;
                        }
                    }
                    if (!encontrado) {
                        _vendedores.push(_comisionesB1[i].Vendedor);
                    }

                }

                for (j = 0; j < _vendedores.length; j++) {
                    vendedor = _vendedores[j];
                    total = 0;
                    for (i = 0; i < _comisionesB1.length; i++) {
                        if (_comisionesB1[i].Vendedor == vendedor) {
                            h += "<tr ";
                            //h += "data-candidatoid='" + _comisionesB1[i].Candidatoid + "' ";
                            //h += "data-comisionvendedor='" + _comisionesB1[i].Comisionvendedor + "' ";
                            //h += "data-contactoid='" + _comisionesB1[i].Contactoid + "' ";
                            //h += "data-incentivoconcepto='" + _comisionesB1[i].Incentivoconcepto + "' ";
                            //h += "data-incentivoid='" + _comisionesB1[i].Incentivoid + "' ";
                            //h += "data-nombrecandidato='" + _comisionesB1[i].Nombrecandidato + "' ";
                            //h += "data-nombrevendedor='" + _comisionesB1[i].Nombrevendedor + "' ";
                            //h += "data-nombrevendedorasignado='" + _comisionesB1[i].Nombrevendedorasignado + "' ";
                            //h += "data-nombrevendedorasignadoid='" + _comisionesB1[i].Nombrevendedorasignadoid + "' ";
                            //h += "data-vendedirid='" + _comisionesB1[i].Vendedorid + "' "
                            h += ">";

                            h += "<td>" + _comisionesB1[i].Vendedor + "</td>";
                            h += "<td>" + _comisionesB1[i].Empresa + "</td>";
                            h += "<td>" + _comisionesB1[i].Nombreempresa + "</td>";
                            h += "<td align='center'>" + _comisionesB1[i].Numerocontrato + "</td>";
                            h += "<td align='center'>" + _comisionesB1[i].Vigenciameses + "</td>";
                            h += "<td align='right'>" + formatMoney(_comisionesB1[i].Comisionvendedor.toFixed(2), ",", ".") + "</td>";
                            h += "</tr>";
                            total += _comisionesB1[i].Comisionvendedor;
                        }

                    }
                    h += "<tr>";
                    h += "<td></td>";
                    h += "<td></td>";
                    h += "<td></td>";
                    h += "<td></td>";
                    h += "<td align='right'></td>";
                    h += "<td align='right'>Total <span style='font-weight:bold'>" + formatMoney(total.toFixed(2), ",", ".") + "</span></td>";
                    h += "</tr>";
                    h += "<tr style='background-color:#CFE9F7'><td></td><td></td><td></td><td></td><td></td><td></td></tr>"
                }
                $("#tblComisionesB1 tbody").html(h);



                total = 0;
                _vendedores.length = 0;
                h = "";
                for (i = 0; i < _comisionesA1B1.length; i++) {
                    var j = 0;
                    var encontrado = false;
                    for (j = 0; j < _vendedores.length; j++) {
                        if (_vendedores[j] == _comisionesA1B1[i].Vendedor) {
                            encontrado = true;
                        }
                    }
                    if (!encontrado) {
                        _vendedores.push(_comisionesA1B1[i].Vendedor);
                    }

                }

                for (j = 0; j < _vendedores.length; j++) {
                    vendedor = _vendedores[j];
                    total = 0;
                    for (i = 0; i < _comisionesA1B1.length; i++) {

                        if (_comisionesA1B1[i].Vendedor == vendedor) {

                            h += "<tr ";
                            //h += "data-candidatoid='" + _comisionesB1[i].Candidatoid + "' ";
                            //h += "data-comisionvendedor='" + _comisionesB1[i].Comisionvendedor + "' ";
                            //h += "data-contactoid='" + _comisionesB1[i].Contactoid + "' ";
                            //h += "data-incentivoconcepto='" + _comisionesB1[i].Incentivoconcepto + "' ";
                            //h += "data-incentivoid='" + _comisionesB1[i].Incentivoid + "' ";
                            //h += "data-nombrecandidato='" + _comisionesB1[i].Nombrecandidato + "' ";
                            //h += "data-nombrevendedor='" + _comisionesB1[i].Nombrevendedor + "' ";
                            //h += "data-nombrevendedorasignado='" + _comisionesB1[i].Nombrevendedorasignado + "' ";
                            //h += "data-nombrevendedorasignadoid='" + _comisionesB1[i].Nombrevendedorasignadoid + "' ";
                            //h += "data-vendedirid='" + _comisionesB1[i].Vendedorid + "' "
                            h += ">";

                            h += "<td>" + _comisionesA1B1[i].Vendedor + "</td>";
                            h += "<td align='center'>" + _comisionesA1B1[i].Tipo + "</td>";
                            h += "<td>" + _comisionesA1B1[i].Nombrecompleto + "</td>";
                            h += "<td>" + _comisionesA1B1[i].Nombreempresa + "</td>";
                            h += "<td align='center'>" + _comisionesA1B1[i].Foliocesion + "</td>";
                            h += "<td align='right'>" + formatMoney(_comisionesA1B1[i].Comisionvendedor.toFixed(2), ",", ".") + "</td>";
                            h += "</tr>";
                            total += _comisionesA1B1[i].Comisionvendedor
                        }

                    }

                    h += "<tr>";
                    h += "<td></td>";
                    h += "<td></td>";
                    h += "<td></td>";
                    h += "<td></td>";
                    h += "<td align='right'></td>";
                    h += "<td align='right'>Total <span style='font-weight:bold'>" + formatMoney(total.toFixed(2),",",".") + "</span></td>";
                    h += "</tr>";
                    h += "<tr style='background-color:#CFE9F7'><td></td><td></td><td></td><td></td><td></td><td></td></tr>"
                }
                $("#tblComisionesA1B1 tbody").html(h);




                total = 0;
                _vendedores.length = 0;
                h = "";
                for (i = 0; i < _comisionesA1.length; i++) {
                    var j = 0;
                    var encontrado = false;
                    for (j = 0; j < _vendedores.length; j++) {
                        if (_vendedores[j] == _comisionesA1[i].Vendedor) {
                            encontrado = true;
                        }
                    }
                    if (!encontrado) {
                        _vendedores.push(_comisionesA1[i].Vendedor);
                    }

                }

                for (j = 0; j < _vendedores.length; j++) {
                    vendedor = _vendedores[j];
                    total = 0;
                    for (i = 0; i < _comisionesA1.length; i++) {

                        if (_comisionesA1[i].Vendedor == vendedor) {

                            h += "<tr ";
                            //h += "data-candidatoid='" + _comisionesB1[i].Candidatoid + "' ";
                            //h += "data-comisionvendedor='" + _comisionesB1[i].Comisionvendedor + "' ";
                            //h += "data-contactoid='" + _comisionesB1[i].Contactoid + "' ";
                            //h += "data-incentivoconcepto='" + _comisionesB1[i].Incentivoconcepto + "' ";
                            //h += "data-incentivoid='" + _comisionesB1[i].Incentivoid + "' ";
                            //h += "data-nombrecandidato='" + _comisionesB1[i].Nombrecandidato + "' ";
                            //h += "data-nombrevendedor='" + _comisionesB1[i].Nombrevendedor + "' ";
                            //h += "data-nombrevendedorasignado='" + _comisionesB1[i].Nombrevendedorasignado + "' ";
                            //h += "data-nombrevendedorasignadoid='" + _comisionesB1[i].Nombrevendedorasignadoid + "' ";
                            //h += "data-vendedirid='" + _comisionesB1[i].Vendedorid + "' "
                            h += ">";

                            h += "<td>" + _comisionesA1[i].Vendedor + "</td>";
                            h += "<td align='center'>" + _comisionesA1[i].Tipo + "</td>";
                            h += "<td>" + _comisionesA1[i].Nombrecompleto + "</td>";
                            h += "<td>" + _comisionesA1[i].Nombreempresa + "</td>";
                            h += "<td align='center'>" + _comisionesA1[i].Foliocesion + "</td>";
                            h += "<td align='right'>" + formatMoney(_comisionesA1[i].Comisionvendedor.toFixed(2), ",", ".") + "</td>";
                            h += "</tr>";
                            total += _comisionesA1[i].Comisionvendedor
                        }

                    }

                    h += "<tr>";
                    h += "<td></td>";
                    h += "<td></td>";
                    h += "<td></td>";
                    h += "<td></td>";
                    h += "<td align='right'></td>";
                    h += "<td align='right'>Total <span style='font-weight:bold'>" + formatMoney(total.toFixed(2), ",", ".") + "</span></td>";
                    h += "</tr>";
                    h += "<tr style='background-color:#CFE9F7'><td></td><td></td><td></td><td></td><td></td><td></td></tr>"

                }
                $("#tblComisionesA1 tbody").html(h);


                h = "";
                //Tabla de gerentes
                for (i = 0; i < _comisionesGerentes.length; i++) {

                    h += "<tr ";
                    h += ">";
                    h += "<td>" + _comisionesGerentes[i].Nombregerente + "</td>";
                    h += "<td align='right'>" + formatMoney(_comisionesGerentes[i].Comisionvendedor.toFixed(2), ",", ".") + "</td>";
                    h += "</tr>";
                    total += _comisionesA1[i].Comisionvendedor
                }
                $("#tblComisionesGerentes tbody").html(h);

            }
            else {
                ShowLightBoxPopUp(false, "", "");
                //alert(d.d.Mensaje);
                mostrarMensaje(d.d.Mensaje);
            }
        },

        function (d) {

            ShowLightBoxPopUp(false, "", "");
            //alert(d.responseText);
            mostrarMensaje(d.responseText);
        }, true);
}

function guardarComisiones() {
    var fechaini = $("#fechaInicio").val();
    var fechafin = $("#fechaFinal").val();
    var empresaid = $("#ddlEmpresas option:selected").val();
    var vendedorid = $("#ddlVendedores option:selected").val();
    var usuario = getUsuario();
    ShowLightBoxPopUp(true, 'Grabando comisiones, espere por favor...', 'body');
    ejecutaAjax("../../Ventas/formas/GenerarComisiones.aspx/GuardarComisiones", "POST", { 'fechaini': fechaini, 'fechafin': fechafin, 'empresaid': empresaid, 'vendedorid': vendedorid, 'usuario': usuario },
        function (d) {
            if (d.d.EsValido) {
                ShowLightBoxPopUp(false, "", "");
                showMsg('Éxito', "Se han grabado las comisiones con éxito", 'success'); 
                
            }
            else {
                ShowLightBoxPopUp(false, "", "");
                showMsg('Alerta', d.d.Mensaje, 'warning');
            }
        },
        function (d) {
            ShowLightBoxPopUp(false, "", "");
            showMsg('Alerta', d.responseText, 'error');
        },
        true);
}

function imprimirComisiones() {
    var parametros = "";
    parametros += "&fechaini=" + $("#fechaInicio").val();
    parametros += "&fechafin=" + $("#fechaFinal").val();
    parametros += "&empresaid=" + $("#ddlEmpresas option:selected").val();
    parametros += "&vendedorid=" + $("#ddlVendedores option:selected").val();
    parametros += "&tipovendedor=" + $("#ddlTipoVendedores option:selected").val();
    parametros += "&agrupado=" + ($("#chkAgrupado").is(":checked") ? 1 : 0);
    window.open("../../Base/Formas/ReporteProduccion.aspx?Formato=0&Nombre=AyudaCombustible" + parametros, 'w_PopReporteComisionVendedores' + getUsuario(), 'toolbar=no,location=no,directories=no,status=no,menubar=no,scrollbars=yes,resizable=yes,width=760,height=560');

}

function imprimirComisionesPorTipoDeVendedor() {
    var parametros = "";
    parametros += "&fechaini=" + $("#fechaInicio").val();
    parametros += "&fechafin=" + $("#fechaFinal").val();
    parametros += "&empresaid=" + $("#ddlEmpresas option:selected").val();
    parametros += "&vendedorid=" + $("#ddlVendedores option:selected").val();
    parametros += "&tipovendedor=" + $("#ddlTipoVendedores option:selected").val();
    parametros += "&agrupado=" + ($("#chkAgrupado").is(":checked") ? 1 : 0);
    window.open("../../Base/Formas/ReporteProduccion.aspx?Formato=1&Nombre=ComisionesVendedores" + parametros, 'w_PopReporteComisionVendedores' + getUsuario(), 'toolbar=no,location=no,directories=no,status=no,menubar=no,scrollbars=yes,resizable=yes,width=760,height=560');

}


function traerEmpresas() {
    ejecutaAjax('ReporteAyudaCombustible.aspx/TraerEmpresas', 'POST', null, function (d) {
        if (d.d.EsValido) {
            var h = "";
            var i = 0;
            var datos = d.d.Datos;
            h += "<option value='*' selected>Seleccione...</option>";
            for (i = 0; i < datos.length; i++) {
                h += "<option value='" + datos[i].id + "'>" + datos[i].nombre + "</option>";
            }
            $("#ddlEmpresas").html(h);
            //$('#ddlEmpresas').llenaCombo(d.d.Datos, null, amplify.store.sessionStorage("EmpresaID"));
        }
        else {
            alert(d.d.Mensaje);
        }
    }, function (d) {
        alert(d.responseText);
    }, true);
}

function traerVendedores() {
    ejecutaAjax('ReporteAyudaCombustible.aspx/TraerVendedores', 'POST', null, function (d) {
        if (d.d.EsValido) {
            var h = "";
            var i = 0;
            var datos = d.d.Datos;
            h += "<option value='*'selected>Seleccione...</option>";
            for (i = 0; i < datos.length; i++) {
                h += "<option value='" + datos[i].id + "'>" + datos[i].nombre.toUpperCase() + "</option>";
            }
            $("#ddlVendedores").html(h);
            //$('#ddlEmpresas').llenaCombo(d.d.Datos, null, amplify.store.sessionStorage("EmpresaID"));
        }
        else {
            //alert(d.d.Mensaje);
            mostrarMensaje(d.d.Mensaje);
        }
    }, function (d) {
        //alert(d.responseText);
        mostrarMensaje(d.responseText);
    }, true);
}

function formatMoney(n, c, d, t) {
    var c = isNaN(c = Math.abs(c)) ? 2 : c,
      d = d == undefined ? "." : d,
      t = t == undefined ? "," : t,
      s = n < 0 ? "-" : "",
      i = String(parseInt(n = Math.abs(Number(n) || 0).toFixed(c))),
      j = (j = i.length) > 3 ? j % 3 : 0;

    return s + (j ? i.substr(0, j) + t : "") + i.substr(j).replace(/(\d{3})(?=\d)/g, "$1" + t) + (c ? d + Math.abs(n - i).toFixed(c).slice(2) : "");
};

$(document).ready(function () {
    if (amplify.store.sessionStorage("UsuarioID") == null || amplify.store.sessionStorage("UsuarioID") == "") {
        window.location.replace("../../login.aspx");
    }
    $("#Menu").MostrarMenuBar(JSON.parse(amplify.store.sessionStorage("Funcionalidades")), amplify.store.sessionStorage("Nombre"), amplify.store.sessionStorage("NombreEmpresa"), amplify.store.sessionStorage("UsuarioID"));
    $('.popup').setDraggable('.popup-body, .popup-footer, .close');
    ActualizaMenuBar();
    AplicarEstilo();    
    traerEmpresas();
    traerVendedores();
});

document.getElementById("fechaInicio").addEventListener("keyup", function (event) {
    if (event.keyCode === 13) {
        document.getElementById("fechaFinal").focus();
    }
});
