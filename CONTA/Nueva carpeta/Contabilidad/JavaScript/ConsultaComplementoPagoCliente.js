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


    var formatoFechaJSON = function (f) {
        var fi = f.replace(/\/Date\((-?\d+)\)\//, "$1");
        var fecha = new Date(parseInt(fi));

        var dd = ('0' + fecha.getDate().toString()).slice(-2)
        var mm = ('0' + (fecha.getMonth() + 1).toString()).slice(-2)
        var y = fecha.getFullYear().toString();
        return dd + '/' + mm + '/' + y;
    };


    $('body').on('click', '#btnLimpiar', null, function () {
        window.location.replace('CapturaComplementoPago.aspx');
    });

    

    $('body').on('click', '#MobileMenu', null, function () {
        $("#MobileMenuDet").slideToggle("slow");
    });


    $('body').on('click', '#btnconsultar', null, function () {
        var clienteid = $('#ayudaCliente_ValorID').val() == "" ? null : $('#ayudaCliente_ValorID').val();
        var soloconsaldo = $("#chksoloconsaldo").is(":checked") ? 1 : null;
        traeFacturasConComplementosFaltantes(clienteid, null, null, soloconsaldo)
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


    var ayudaClienteFindByCode = function () {
        var parametros = { value: JSON.stringify($('#ayudaCliente').getValuesByCode()) };
        ejecutaAjax('../../Analisis/Formas/FuncionesGenericas.aspx/AyudaCliente_FindByCode', 'POST', parametros, function (d) {
            $('#ayudaCliente').showFindByCode(d.d.Datos);
        }, function (d) {
            showMsg('Alerta', d.responseText, 'error');
        }, true);
    };

    var ayudaClienteFindByPopUp = function () {
        var parametros = { value: JSON.stringify($('#ayudaCliente').getValuesByPopUp()) };
        ejecutaAjax('../../Analisis/Formas/FuncionesGenericas.aspx/AyudaCliente_FindByPopUp', 'POST', parametros, function (d) {
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
        
    };

    var traeFacturasConComplementosFaltantes = function (clienteid, facturaid, uuid, soloconsaldo) {
        ShowLightBox(true, "Buscando facturas...");
        var parametros = { "clienteid": clienteid, "facturaid": facturaid, "uuid": uuid, "soloconsaldo": soloconsaldo };
        ejecutaAjax('../../Contabilidad/Formas/ConsultaComplementoPagoCliente.aspx/TraeFacturasComplementos', 'POST', parametros, function (d) {
            if (d.d.EsValido) {
                ShowLightBox(false);
                procesaFacturas(d.d.Datos);                
            } else {
                ShowLightBox(false);
                showMsg('Alerta', d.d.Mensaje, 'warning');                
            }
        }, function (d) {
            ShowLightBox(false);
            showMsg('Alerta', d.responseText, 'error');

        }, true);
    };

    var procesaFacturas = function (datos) {
        var h = "";
        var totalImporteFacturas = 0;
        var totalImporteComplementos = 0;
        var totalImporteRestante = 0;
        var cantidadFacturas = 0;
        var cantidadComplementos = 0;

        var facturasagrupado = datos.Agrupado;
        var facturasdetalle = datos.Facturas;

        h += "<div class='encabezado'>";
        h += "<div class='nombre inlineblock'>NOMBRE DE CLIENTE</div>";
        h += "<div class='total-facturas inlineblock'>#FACTURAS</div>";
        h += "<div class='importe-facturas inlineblock'>IMPORTE FACTURAS</div>";
        h += "<div class='total-complementos inlineblock'>#COMPLEMENTOS</div>";
        h += "<div class='importe-facturas inlineblock'>IMPORTE COMPLEMENTOS</div>";
        h += "<div class='restante-facturas inlineblock'>RESTANTE</div>";
        h += "</div>"
        for (var i = 0; i < facturasagrupado.length; i++) {
            h += "<div class='cliente' data-clienteid='" + facturasagrupado[i].ClienteID + "'>";
            h += "<div class='nombre inlineblock'>" + facturasagrupado[i].Codigo + " - " + facturasagrupado[i].NombreCompleto + "</div>";
            h += "<div class='total-facturas inlineblock'>" + facturasagrupado[i].TotalFacturas + "</div>";
            h += "<div class='importe-facturas inlineblock'>" + balor.aplicarFormatoMoneda(facturasagrupado[i].ImporteFacturas, "$") + "</div>";
            h += "<div class='total-complementos inlineblock'>" + facturasagrupado[i].TotalComplementos + "</div>";
            h += "<div class='importe-facturas inlineblock'>" + balor.aplicarFormatoMoneda(facturasagrupado[i].ImporteComplementos, "$") + "</div>";
            h += "<div class='restante-facturas inlineblock'>" + balor.aplicarFormatoMoneda(facturasagrupado[i].ImporteRestante, "$") + "</div>";
            h += "</div>"

            h += "<div class='detallefacturas' id='" + facturasagrupado[i].ClienteID + "'>";                        
            h += "<table class='svGrid'>";

            h += "<tr>";
            h += "<th>Factura</th>";
            h += "<th>UUID</th>";
            h += "<th>Importe Factura</th>";
            h += "<th># Complementos</th>";
            h += "<th>Importe Complementos</th>";
            h += "<th>Restante</th>";
            h += "</tr>";

            for (var j = 0; j < facturasdetalle.length; j++) {
                if (facturasdetalle[j].ClienteID == facturasagrupado[i].ClienteID) {

                    h += "<tr>";                    
                    h += "<td style='text-align:center;'>" + (facturasdetalle[j].Serie + " " + facturasdetalle[j].Folio) + "</td>";
                    h += "<td style='text-align:center;'>" + facturasdetalle[j].UUID + "</td>";
                    h += "<td style='text-align:right;'>" + balor.aplicarFormatoMoneda(facturasdetalle[j].ImporteFactura, "$") + "</td>";
                    h += "<td style='text-align:center;'>" + facturasdetalle[j].NumeroComplementos + "</td>";
                    h += "<td style='text-align:right;'>" + balor.aplicarFormatoMoneda(facturasdetalle[j].ImporteComplementos, "$") + "</td>";
                    h += "<td style='text-align:right;'>" + balor.aplicarFormatoMoneda(facturasdetalle[j].Restante, "$") + "</td>";
                    h += "</tr>";
                }
            }
            h += "</table>";            
            h += "</div>";
        }

        $("#agrupado").html(h);

        h = "";

        //$("#tblfacturas tbody").html("");
        //for (var i = 0; i < datos.length; i++) {
        //    h += "<tr>";
        //    h += "<td>" + datos[i].Codigo + " - " + datos[i].NombreCompleto + "</td>";
        //    h += "<td style='text-align:center;'>" + (datos[i].Serie + " " + datos[i].Folio) + "</td>";
        //    h += "<td style='text-align:center;'>" + datos[i].UUID + "</td>";
        //    h += "<td style='text-align:right;'>" + balor.aplicarFormatoMoneda(datos[i].ImporteFactura, "$") + "</td>";
        //    h += "<td style='text-align:center;'>" + datos[i].NumeroComplementos + "</td>";
        //    h += "<td style='text-align:right;'>" + balor.aplicarFormatoMoneda(datos[i].ImporteComplementos, "$") + "</td>";
        //    h += "<td style='text-align:right;'>" + balor.aplicarFormatoMoneda(datos[i].Restante, "$") + "</td>";
        //    h += "</tr>";

        //    totalImporteFacturas += datos[i].ImporteFactura;
        //    totalImporteComplementos += datos[i].ImporteComplementos;
        //    totalImporteRestante += datos[i].Restante;
        //    cantidadFacturas += 1;
        //    cantidadComplementos += datos[i].NumeroComplementos;
        //}
        //$("#tblfacturas tbody").html(h);

        
        //h = "<tr>";
        //h += "<td></td>"
        //h += "<td></td>"
        //h += "<td style='font-size:14px;'>Cantidad Facturas: " + cantidadFacturas + "</td>";
        //h += "<td style='text-align:right;font-size:14px;'>" + balor.aplicarFormatoMoneda(totalImporteFacturas, "$") + "</td>";
        //h += "<td style='text-align:center;font-size:14px;'>" + cantidadComplementos + "</td>";
        //h += "<td style='text-align:right;font-size:14px;'>" + balor.aplicarFormatoMoneda(totalImporteComplementos, "$") + "</td>";
        //h += "<td style='text-align:right;font-size:14px;'>" + balor.aplicarFormatoMoneda(totalImporteRestante, "$") + "</td>";
        //h += "</tr>";
        //$("#tblfacturas tfoot").html(h);
    };

    var iniciaAyudas = function () {
        $('#ayudaCliente').helpField({
            title: 'Cliente',
            codeNumeric: true,
            maxlengthCode: 6,
            enableCode: true,
            enableDescription: false,
            codePaste: false,
            nextControlID: 'txtfecha',
            //widthDescription: '0',
            requeridField: false,
            cultureResourcesPopUp: {
                popUpHeader: 'Búsqueda de Clientes'
            },
            findByPopUp: ayudaClienteFindByPopUp,
            findByCode: ayudaClienteFindByCode,
            onElementFound: ayudaCliente_onElementFound
        });
    };

    $("body").on("click", ".cliente", null, function (e) {
        var clienteid= $(this).attr("data-clienteid");
        $("#" + clienteid).toggle(500);
    });

    $(document).ready(function () {
        if (amplify.store.sessionStorage("UsuarioID") == null || amplify.store.sessionStorage("UsuarioID") == "") {
            window.location.replace("../../login.aspx");
        }
        $("#Menu").MostrarMenuBar(JSON.parse(amplify.store.sessionStorage("Funcionalidades")), amplify.store.sessionStorage("Nombre"), amplify.store.sessionStorage("NombreEmpresa"), amplify.store.sessionStorage("UsuarioID"));
        $('.popup').setDraggable('.popup-body, .popup-footer, .close');
        ActualizaMenuBar();
        AplicarEstilo();
        $('#btnGuardar, #btnImprimir').attr('disabled', 'disabled');
        iniciaAyudas();
        traeFacturasConComplementosFaltantes(null, null, null, 1);
    });
}(jQuery, window.balor, window.amplify));