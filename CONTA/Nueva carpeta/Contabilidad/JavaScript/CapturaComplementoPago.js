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

    var formatoFechaJSON = function (f) {
        var fi = f.replace(/\/Date\((-?\d+)\)\//, "$1");
        var fecha = new Date(parseInt(fi));

        var dd = ('0' + fecha.getDate().toString()).slice(-2)
        var mm = ('0' + (fecha.getMonth() + 1).toString()).slice(-2)
        var y = fecha.getFullYear().toString();
        return dd + '/' + mm + '/' + y;
    };



    body.on("click", '#upload', function (e) {
        $("input:file").val("");
    });

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

    var fileupload = document.getElementById('upload');
    fileupload.querySelector('input').onchange = function () {
        procesarXMLComplemento(this.files);
    };

    function procesarXMLComplemento(files) {
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
        var xhr = new XMLHttpRequest();
        xhr.open('POST', 'ProcesarComplemento.ashx');
        xhr.onload = function () {
            //AQUI SE PROCESA LA RESPUESTA DEL SERVIDOR
            if (xhr.responseText.indexOf("rror:") > 0) {
                alert(xhr.responseText);
                ShowLightBox(false, "");
                return;
            }
            var XML = JSON.parse(xhr.responseText);
            var XMLVigentes = [];
            var XMLError = [];

            //Procesamos cada una de las facturas cargadas en el sistema
            for (var i = 0; i < XML.length; i++) {

                XML[i].Facturapagoid = 0;
                XML[i].UltimaAct = 0;

                //Validamos si la factura aun esta vigente en el SAT
                if (XML[i].TipoRespuesta != 0) {
                    XMLError.push(XML[i]);
                    continue;
                }

                if (ExisteUUIDGrid(XML[i].UUID)) {
                    XML[i].Descripcion = "LA FACTURA YA ESTA CARGADA EN EL GRID";
                    XMLError.push(XML[i]);
                    continue;
                }
                //Validamos que la factura corresponda a la empresa correspondiente segun sea el caso a balor o a factur                
                if ($.trim(XML[i].Receptorrfc).toUpperCase() != $.trim($("#CajaEmpresaRFC").val()).toUpperCase()) {
                    XML[i].Descripcion = "LA FACTURA NO FUE EMITIDA PARA LA EMPRESA: " + $("#CajaEmpresaRFC").val();
                    XMLError.push(XML[i]);
                    continue;
                }
                XMLVigentes.push(XML[i]);
            }

            if (XMLVigentes.length > 0)
                $("#btnGuardar").removeAttr("disabled");

            ShowLightBox(false, "");
            ProcesarXML(XMLVigentes);
            if (XMLError.length > 0) {
                ProcesarXMLError(XMLError);
            }
        };
        xhr.send(formData);
    };

    var ExisteUUIDGrid = function (uuid) {
        var existe = false;
        $("#TablaComplementos tbody tr").each(function () {
            if ($(this).attr("data-Uuid") == uuid) {
                existe = true;
            }
        });
        return existe;
    };

    var ProcesarXMLError = function (Datos) {
        var mensaje = "";
        for (var i = 0; i < Datos.length; i++) {
            mensaje += "Archivo: " + Datos[i].NomArchivo + " - " + Datos[i].Descripcion + "  \n";
        }
        alert(mensaje);
    };

    var ProcesarXML = function (Datos) {
        var resHtml = '';
        var importetotal = 0;
        var ivaacum = 0;
        for (var i = 0; i < Datos.length; i++) {
            resHtml += '<tr class="rgFactura" data-Uuid="' + Datos[i].UUID + '" data-file="' + Datos[i].NomArchivo + '" data-Facturapagoid="' + Datos[i].Facturapagoid + '" data-UltimaAct="' + Datos[i].UltimaAct + '" data-Proveedorid="' + Datos[i].Proveedorid + '" data-rfcEmisor ="' + Datos[i].Emisorrfc + '">';
            resHtml += '<td>' + Datos[i].Emisornombre + '</td>';
            resHtml += '<td>' + Datos[i].Factura + '</td>';
            resHtml += '<td>' + formatoFechaJSON(Datos[i].Fechatimbrado) + '</td>';
            resHtml += '<td>' + Datos[i].NoComplementos + '</td>';            
            resHtml += '<td class="CTotal">' + balor.aplicarFormatoMoneda(Datos[i].Total, "$") + '</td>';
            resHtml += '<td class="tdeliminar centrado"><span class="closebtn elim-Factura" >X</span></td>';
            resHtml += '</tr>';
        }
        $("#TablaComplementos tbody").append(resHtml);
    };

    $('body').on('click', '.elim-Factura', null, function () {
        if (confirm('¿Desea eliminar esta factura?')) {
            var tr = $(this).parents('tr');
            var fp = $(this.parentNode.parentNode).attr("data-Facturapagoid");
            if (fp != 0) {
                var parametros = { Facturapagoid: fp, usuario: getUsuario() };
                ejecutaAjax('CapturaComplementoPago.aspx/EliminarComplemento', 'POST', parametros, function (d) {
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
        }
    });


   $('body').on('click', '#btnLimpiar', null, function () {
       window.location.replace('CapturaComplementoPago.aspx');
    });

    $('body').on('click', '#btnGuardar', null, function () {
        guardar();
    });







    var guardar = function () {
        var parametros = GetComplementos();
        escondeMsg();
        ShowLightBox(true, "Espere porfavor");
        ejecutaAjax('CapturaComplementoPago.aspx/Guardar', 'POST', { value: JSON.stringify(parametros) }, function (d) {
            ShowLightBox(false, "");
            if (d.d.EsValido) {
                //showMsg('Guardado', 'Guardado correctamente', 'success');
                alert('Guardado correctamente.');
                window.location.replace("CapturaComplementoPago.aspx");

            } else {
                ShowLightBox(false, "");
                showMsg('Alerta', 'No se guardó el complemento. ' + d.d.Mensaje, 'warning');
            }
        }, function (d) {
            ShowLightBox(false, "");
            showMsg('Alerta', d.responseText, 'error');
        }, true);
    };



    var GetComplementos = function () {
        var ListaComplementos = [];
        $("#TablaComplementos tbody tr").each(function () {
            var Facturapagoid = $(this).attr("data-Facturapagoid");
            if (Facturapagoid == 0) {
                var Catfacturaspago = {};
                //Catfacturaspago.Facturapagoid = $(this).attr("data-Facturapagoid");
                Catfacturaspago.Empresaid = amplify.store.sessionStorage("EmpresaID");
                Catfacturaspago.Proveedorid = $(this).attr("data-Proveedorid");
                Catfacturaspago.ProveedorRFC = $(this).attr("data-rfcemisor");
                //Catfacturasproveedor.Subtotal = balor.quitarFormatoMoneda($(this).find(".CSubtotal")[0].innerText);
                //Catfacturasproveedor.Iva = balor.quitarFormatoMoneda($(this).find(".CIva")[0].innerText);
                //Catfacturasproveedor.RetIva = balor.quitarFormatoMoneda($(this).find(".CRetIva")[0].innerText);
                //Catfacturasproveedor.RetIsr = balor.quitarFormatoMoneda($(this).find(".CRetIsr")[0].innerText);
                //Catfacturasproveedor.Total = balor.quitarFormatoMoneda($(this).find(".CTotal")[0].innerText);
                //Catfacturasproveedor.Uuid = $(this).attr("data-Uuid");
                //Catfacturasproveedor.Fechatimbre = $(this).attr("data-Fechatimbre");
                //Catfacturasproveedor.Xml = "";
                Catfacturaspago.NomArchivo = $(this).attr("data-file");
                Catfacturaspago.Usuario = getUsuario();
                //Catfacturaspago.Estatus = 1;
                //Catfacturaspago.UltimaAct = $(this).attr("data-UltimaAct");
                ListaComplementos.push(Catfacturaspago);
            }
        });
        return ListaComplementos;
    };















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


    $(document).ready(function () {
        if (amplify.store.sessionStorage("UsuarioID") == null || amplify.store.sessionStorage("UsuarioID") == "") {
            window.location.replace("../../login.aspx");
        }
        $("#Menu").MostrarMenuBar(JSON.parse(amplify.store.sessionStorage("Funcionalidades")), amplify.store.sessionStorage("Nombre"), amplify.store.sessionStorage("NombreEmpresa"), amplify.store.sessionStorage("UsuarioID"));
        $('.popup').setDraggable('.popup-body, .popup-footer, .close');
        ActualizaMenuBar();
        AplicarEstilo();
        $('#btnGuardar, #btnImprimir').attr('disabled', 'disabled');
        ontenerRFCPorEmpresa();
    });
}(jQuery, window.balor, window.amplify));