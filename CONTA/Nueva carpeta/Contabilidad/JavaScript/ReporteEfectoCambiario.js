(function ($, balor, amplify) {
    var msgbx = $('#divmsg');
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
            var _optSelected = "<option value='0' selected>CONSOLIDADO</option>";
            //$('#ddl-empresa').html("<option value='0' selected>CONSOLIDADO</option>");
            //$('#ddl-empresa').llenaCombo(d.d.Datos, _optSelected, amplify.store.sessionStorage("EmpresaID"));
            $('#ddl-empresa').llenaCombo(d.d.Datos, "CONSOLIDADO", '');
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

    var getUsuario = function () {
        return amplify.store.sessionStorage("Usuario");
    };

    var getVendedor = function () {
        return amplify.store.sessionStorage("VendedorID");
    };

    var ValidaImprimir = function () {
        //if ($("#fechaInicio").val() == "") {
        //    showMsg('Alerta', "Debe ingresar la fecha de inicio", 'warning');
        //    $("#fechaInicio").focus();
        //    return false;
        //}
        if ($("#fechaFinal").val() == "") {
            showMsg('Alerta', "Debe ingresar la fecha final", 'warning');
            $("#fechaFinal").focus();
            return false;
        }
        return true;
    };

    $('body').on('blur', '#fechaInicio,#fechaFinal', null, function () {
        var ffecha = $(this).val();
        $(this).val(balor.appliFormatDate(ffecha));
        if (!balor.isValidDate($(this).val())) {
            $(this).val("");
        }
    });

    $('body').on('click', '#BotonImprimir', null, function () {
        if (ValidaImprimir()) {
            ShowLightBox(true, "Generando reporte, espere...");
            var TipoImpresion = $("input[type='radio'][name='timprimir']:checked").val();

            if (TipoImpresion == '3') { //PDF
                var parametros = "&empresaid=" + $('#ddl-empresa option:selected').val();
                parametros += "&fechaFinal=" + $("#fechaFinal").val().trim();
                parametros += "&Formato=" + $("input[type='radio'][name='timprimir']:checked").val();
                parametros += "&TipoImpresion=" + TipoImpresion;
                window.open("../../Base/Formas/ReporteProduccion.aspx?Formato=0&Nombre=EfectoCambiario" + parametros, 'w_PopEfectoCambiario' /*+ $("#fechaInicio").val().trim().replace("/", "") */ + $("#fechaFinal").val().trim().replace("/", "") + $('#ddl-empresa option:selected').val(), 'toolbar=no,location=no,directories=no,status=no,menubar=no,scrollbars=yes,resizable=yes,width=760,height=560');
                ShowLightBox(false, "");
            } else { //EXCEL
                ejecutaAjax('ReporteEfectoCambiario.aspx/GenerarReporteEfectoCambiario', 'POST', { TipoImpresion: TipoImpresion, EmpresaId: $('#ddl-empresa option:selected').val(), Fecha: $("#fechaFinal").val().trim() }, function (d) {
                    if (d.d.EsValido) {
                        ShowLightBox(false, "");
                        var url = d.d.Datos;
                        var a = document.createElement("a");
                        a.href = url;
                        document.body.appendChild(a);
                        //$(a).trigger('click');
                        $(a).click();
                        document.body.removeChild(a);
                        window.open(url);
                    } else {
                        ShowLightBox(false, "");
                        alert(d.d.Mensaje);
                    }
                }, function (d) {
                    ShowLightBox(false, "");
                    alert(d.responseText);
                }, true);
            }
        }
    });

    $('body').on('click', '#btnLimpiar', null, function () {
        window.location.replace("ReporteIvaAcreditable.aspx");
    });

    $('input').not('input[type=button]').on('keypress', null, function (e) {
        focusNextControl(this, e);
    });

    $('body').on('click', '#MobileMenu', null, function () {
        $("#MobileMenuDet").slideToggle("slow");
    });

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

    var VerificarTipoCambioBM = function () {

        var f = new Date();
        var fechaInicial = f.getFullYear()-1 + "-12-01";
        var fechaFinal = f.getFullYear() + "-" + (f.getMonth() + 1) + "-01";
        var sUsuario = getUsuario()
        var sSerieFinal = "SF18560";
        var sSerieInicial = "SF17906";
        var ListaSerie = [];

        var sToken = "dcf97c1a53e97cbc82b49423801be85ee106d0db6bd2d967c3560dcecda1c89f";
        //var sUrl = "https://www.banxico.org.mx/SieAPIRest/service/v1/series/" + sSerie + "/datos/oportuno?token=" + sToken;
        var sUrl = "https://www.banxico.org.mx/SieAPIRest/service/v1/series/" + sSerieInicial + ',' + sSerieFinal + "/datos/"+fechaInicial+"/"+fechaFinal+"/?token=" + sToken;
        var FechaTipoCambio = "";
        var TipoCambio = "";

        $.ajax({
            url: sUrl,
            jsonp: "callback",
            dataType: "jsonp",                      //Se utiliza JSONP para realizar la consulta cross-site
            success: function (response) {          //Handler de la respuesta
                var Respuestaseries = response.bmx.series;
                

                for (var i in Respuestaseries) {

                    var serie = Respuestaseries[i].datos;
                    var SerieId = Respuestaseries[i].idSerie;
                    var iTipo = 0;

                    if (SerieId.trim() == sSerieInicial.trim()) {
                        iTipo = 1;
                    } else {
                        iTipo = 2;
                    }
                    
                    for (var x in serie) {
                        var registro = serie[x];
                        var item = {};
                        item.FechaTipoCambio = registro.fecha;
                        item.Importetipocambio = registro.dato;
                        item.Usuario = sUsuario;
                        item.Serie = SerieId;
                        item.Tipo = iTipo;
                        item.UltimaAct = 0;
                        ListaSerie.push(item);
                    }
                    //FechaTipoCambio = serie.datos[0].fecha;
                    //TipoCambio = serie.datos[0].dato;
                }

                if (ListaSerie.length> 0) {

                    ShowLightBox(true, "Espere porfavor");
                    ejecutaAjax('CapturaTipoDeCambio.aspx/GuardarTipoCambioBM', 'POST', { value: JSON.stringify(ListaSerie) }, function (d) {
                        ShowLightBox(false, "");
                        
                        if (d.d.EsValido) {
                            //showMsg('Alerta', "Datos guardados correctamente", 'success');
                            console.log("Tipo Cambio: Datos guardados correctamente");
                        } else {
                            //showMsg('Alerta', d.d.Mensaje, 'error');
                            console.log("Tipo Cambio: " + d.d.Mensaje);
                        }
                        
                    }, function (d) {
                        ShowLightBox(false, "");
                        //showMsg('Alerta', d.responseText, 'error');
                        console.log(d.responseText);
                    }, true);
                }
            },
            error: function (d) {
                console.log(d.responseText);
            }
        });
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

        VerificarTipoCambioBM();
    });

}(jQuery, window.balor, window.amplify));