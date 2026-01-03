(function ($, balor, amplify) {

    var colVendedor = 0;
    var colFechaSolicitud = 1;
    var colFechaAutorizacion = 2;
    var colFechaRechazo = 3;
    var colAutoriza = 4;
    var colRechaza = 5;
    var colLongitud = 6;
    var colLatitud = 7;
    var colOpciones = 8;

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

    var getUsuario = function () {
        return amplify.store.sessionStorage("Usuario");
    };


    var focusNextControl = function (control, e) {
        var code = (e.keyCode ? e.keyCode : e.which);
        if (code === 13) {
            var inputs = $(control).closest('form').find('input[type=text]:enabled,input[type=number]:enabled, input[type=password]:enabled, input[type=button]:enabled, input[type=checkbox]:enabled, input[type=radio]:enabled, input[type=date]:enabled, input[type=tel]:enabled, input[type=email]:enabled, input[type=number]:enabled, select:enabled').not('input[readonly=readonly], fieldset:hidden *');
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

   

    $("#btnbuscar").on("click", null, function (e) {
        traerSolicitudes();
    });

    var ayudaVendedores_FindByCode = function () {
        var parametros = { value: JSON.stringify($('#ayudaVendedor').getValuesByCode()) };
        ejecutaAjax('SolicitudAltaCombustible.aspx/AyudaVendedores_FindByCode', 'POST', parametros, function (d) {
            if (d.d.EsValido) {
                $('#ayudaVendedor').showFindByCode(d.d.Datos);
            } else {
                $('#ayudaVendedor_HelpLoad').css({ "display": "none" });
                showMsg('Alerta', d.d.Mensaje, 'warning');
            }
        }, function (d) {
            showMsg('Alerta', d.responseText, 'error');
        }, true);
    };

    var ayudaVendedores_FindByPopUp = function () {
        var parametros = { value: JSON.stringify($('#ayudaVendedor').getValuesByPopUp()) };
        ejecutaAjax('SolicitudAltaCombustible.aspx/AyudaVendedores_FindByPopUp', 'POST', parametros, function (d) {
            if (d.d.EsValido) {
                $('#ayudaVendedor').showFindByPopUp(d.d.Datos);
            } else {
                showMsg('Alerta', d.d.Mensaje, 'warning');
            }
        }, function (d) {
            showMsg('Alerta', d.responseText, 'error');
        }, true);
    };

    var ayudaVendedores_onElementFound = function () {
        var parametros = { vendedorid: $('#ayudaVendedor').getValuesByCode().ID };
        ejecutaAjax('SolicitudAltaCombustible.aspx/TraerVendedor', 'POST', parametros, function (d) {
            if (d.d.EsValido) {
                $('#ayudaVendedor_Code').val(d.d.Datos.Codigovendedor);
                $('#ayudaVendedor_Code').attr("vendedorid", d.d.Datos.Vendedorid);
            } else {
                $('#ayudaVendedor_Code').attr("vendedorid", "");
                $('#ayudaVendedor_HelpLoad').css({ "display": "none" });
                showMsg('Alerta', d.d.Mensaje, 'warning');
            }
        }, function (d) {
            showMsg('Alerta', d.responseText, 'error');
        }, true);
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

    var iniciaAyudas = function () {
        $('#ayudaVendedor').helpField({
            title: 'Vendedor',
            //codeNumeric: true,
            maxlengthCode: 9,
            enableCode: true,
            enableDescription: false,
            codePaste: true,
            nextControlID: 'cboEstatus',
            widthDescription: '250',
            requeridField: false,
            cultureResourcesPopUp: {
                popUpHeader: 'Búsqueda de Vendedores'
            },
            findByPopUp: ayudaVendedores_FindByPopUp,
            findByCode: ayudaVendedores_FindByCode,
            onElementFound: ayudaVendedores_onElementFound,
            camposSalida: [{ header: 'Tipo', value: 'Tipo', code: false, description: false }, { header: 'Folio', value: 'Folio', code: true, description: false }, { header: 'Fecha', value: 'Fecha', code: false, description: false }, { header: 'Descripción', value: 'Descripcion', code: false, description: true }]
        });

    };

    var traerSolicitudes = function () {
        var vendedorid = $('#ayudaVendedor_ValorID').val();
        var estatus = $("#cboEstatus option:selected").val();
        ejecutaAjax("SolicitudAltaCombustible.aspx/TraerSolicitudes", "POST", { "vendedorid": vendedorid, "estatus": estatus },
            function (d) {
                if (d.d.EsValido) {
                    var datos = d.d.Datos;
                    var h = "";
                    for (var i = 0; i < datos.length; i++) {
                        h += "<tr data-solicitudid='" + datos[i].Solicitudid + "'>";
                        h += "<td>" + datos[i].Nombre + "</td>";
                        h += "<td>" + datos[i].Fechasolicitudstring + "</td>";
                        h += "<td>" + datos[i].Fechaaceptadostring + "</td>";
                        h += "<td>" + datos[i].Fecharechazadostring + "</td>";
                        h += "<td>" + datos[i].Usuarioaceptanombre + "</td>";
                        h += "<td>" + datos[i].Usuariorechazanombre + "</td>";
                        h += "<td>" + datos[i].Latitud + "</td>";
                        h += "<td>" + datos[i].Longitud + "</td>";
                        h += "<td align='center'>";
                        if (datos[i].Estatussolicitudid == 0) {
                            h += "<span class='aceptar' title='Aceptar solicitud' ><i class='fa fa-check-circle' style='cursor:pointer'></i></span>"
                            h += "<span class='rechazar' title='Rechazar solicitud' style='cursor:pointer'><i class='fa fa-times-circle'></i></span>"
                        }
                        h += "<span class='mapa' title='Ver localización'><i class='fa fa-map-marker'></i></span>"
                        h += "</td>";
                        h += "</tr>";
                    }
                    $("#tblSolicitudes tbody").html(h);

                    $("#tblSolicitudes tbody tr").each(function () {
                        $(this).find("td:eq(" + colOpciones + ")").find(".aceptar").click(function () {
                            aceptar(this);
                        });
                        $(this).find("td:eq(" + colOpciones + ")").find(".rechazar").click(function () {
                            rechazar(this);
                        });                        
                    });
                }
                else {
                    showMsg("Alerta", d.d.Mensaje, "warning");
                }
            },
            function (d) {
            },
            true)
    };

    var aceptar = function (elem) {
        if (!confirm("¿Aceptar solicitud?")) {
            return;
        }
        var solicitudid= $(elem).closest("tr").attr("data-solicitudid");
        ejecutaAjax("SolicitudAltaCombustible.aspx/AceptarSolicitud", "POST", { "solicitudid": solicitudid, "usuario": getUsuario()},
            function (d) {
                if (d.d.EsValido) {
                    showMsg("Solicitud aceptada con éxito", "", "success");
                    traerSolicitudes();
                }
                else {
                    showMsg("Alerta", d.d.Mensaje, "warning");
                }
            },
            function (d) {
                showMsg("Alerta", d.responseText, "error");
            },
            true);
    };

    var rechazar = function (elem) {
        if (!confirm("¿Rechazar solicitud?")) {
            return;
        }
        var solicitudid = $(elem).closest("tr").attr("data-solicitudid");
        ejecutaAjax("SolicitudAltaCombustible.aspx/RechazarSolicitud", "POST", { "solicitudid": solicitudid, "usuario": getUsuario() },
            function (d) {
                if (d.d.EsValido) {
                    showMsg("Solicitud rechazada con éxito", "", "success");
                    traerSolicitudes();
                }
                else {
                    showMsg("Alerta", d.d.Mensaje, "warning");
                }
            },
            function (d) {
                showMsg("Alerta", d.responseText, "error");
            },
            true);
    };

    $(document).ready(function () {
        
        if (amplify.store.sessionStorage("UsuarioID") == null || amplify.store.sessionStorage("UsuarioID") == "") {
            window.location.replace("../../login.aspx");
        }
        $("#Menu").MostrarMenuBar(JSON.parse(amplify.store.sessionStorage("Funcionalidades")), amplify.store.sessionStorage("Nombre"), amplify.store.sessionStorage("NombreEmpresa"), amplify.store.sessionStorage("UsuarioID"));
        ActualizaMenuBar();
        AplicarEstilo();
        $('.popup').setDraggable('.popup-body, .popup-footer, .close');
        iniciaAyudas();
        traerSolicitudes();


    });

}(jQuery, window.balor, window.amplify));