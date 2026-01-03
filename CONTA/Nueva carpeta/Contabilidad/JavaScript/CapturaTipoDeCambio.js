(function ($, balor, amplify) {
    var msgbx = $('#divmsg');
    var body = $('body');

    var showMsg = function (titulo, mensaje, tipoMensaje) {
        msgbx.mostrarMensaje(titulo, mensaje, tipoMensaje);
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
        top = window.innerHeight * 0.1;
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

    var focusNextControlGridVertical = function (control, e) {
        var code = (e.keyCode ? e.keyCode : e.which);
        if (code === 13) {
            var pos = $(control.parentNode).index(),
          ind1 = (pos + 1),
          ind2 = (pos + 2),
          tabla = elemById('TablaTasas'),
          inputs = $(tabla.querySelectorAll('td:nth-child(' + ind1 + ') input[type=text]:enabled, th:nth-child(' + ind1 + ') input[type=text]:enabled')),
          inputs2 = $(tabla.querySelectorAll('td:nth-child(' + ind2 + ') input[type=text]:enabled, th:nth-child(' + ind2 + ') input[type=text]:enabled'));

            $.merge(inputs, inputs2).eq(inputs.index(control) + 1).focus();
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

    $('input').not('input[type=button]').on('keypress', null, function (e) {
        focusNextControl(this, e);
    });

    body.on("click", '#upload', function (e) {
        $("input:file").val("");
    });

    body.on('keypress', '#TablaTasas input:not(input[type=button])', function (e) {
        focusNextControlGridVertical(this, e);
    });

    $('body').on('focus', '#txtTipoCambio', null, function () {
        $(this).val(balor.quitarFormatoMoneda($(this).val()));
        $(this).select();
    });

    $('body').on('blur', '#txtTipoCambio', null, function () {
        $(this).val(balor.aplicarFormatoMoneda($(this).val(), "$"));

    });

    $('body').on('click', '#txtTipoCambio', null, function () {
        $(this).select();
    });


    $("#rbIndividual").click(function () {
        $("#dviIndividual").show("slow");
        $("#dviDesdeExcel").hide("slow");
        $("#TablaTipoCambio tbody").html("");
    });

    $("#rbDesdeExcel").click(function () {
        $("#txtFecha,#txtTipoCambio").val("");
        $("#dviIndividual").hide("slow")
        $("#dviDesdeExcel").show("slow");
    });

    $('body').on('keyup', "#txtFecha", function (e) {        
            $("#txtTipoCambio").val("");
    });

    $('body').on('blur', '#txtFecha', null, function () {
        if ($(this).val() != "") {
            ejecutaAjax('CapturaTipoDeCambio.aspx/ConsultarTipoCambio', 'POST', { fecha: $("#txtFecha").val() }, function (d) {
                ShowLightBox(false, "");
                if (d.d.EsValido) {
                    $("#txtTipoCambio").val(balor.aplicarFormatoMoneda(d.d.Datos.Importetipocambio, '$'))
                    $("#txtTipoCambio").focus();                    
                } else {
                    showMsg('Alerta', d.d.Mensaje, 'error');
                }
            }, function (d) {
                ShowLightBox(false, "");
                showMsg('Alerta', d.responseText, 'error');
            }, true);
        }
    });

    var fileupload = document.getElementById('upload');
    fileupload.querySelector('input').onchange = function () {
        readfiles(this.files);
    };

    function readfiles(files) {
        if (files.length <= 0)
            return;

        if (files[0].name.toLowerCase().indexOf(".xls") <= 0) {
            alert("Solo se permite subir archivos de excel");
            return;
        }
        var formData = new FormData();
        for (var i = 0; i < files.length; i++) {
            formData.append('file', files[i]);
        }
        ShowLightBox(true, "Espere porfavor...");
        var xhr = new XMLHttpRequest();
        xhr.open('POST', '../../Analisis/Formas/excelajson.ashx');
        xhr.onload = function () {
            ShowLightBox(false, "");
            var listabc = JSON.parse(xhr.responseText),
                len = listabc.length,
                i = 0,
                tr = '';

            for (; i < len; i++) {
                var bc = listabc[i];
                tr += renBC(bc);
            }
            $(tr).appendTo('#TablaTipoCambio tbody');
        };
        xhr.send(formData);

    };
    var renBC = function (bc) {
        bc = bc || {};
        var d ="";
        if (bc.Fecha != null && bc.Fecha != "") {
            var dvalue = bc.Fecha.match(/\d+/)[0];
            d = balor.FormartDate( new Date(parseInt(dvalue)));
        }
        var tr = '<tr data-Importe="' + bc.Importe + '">';
        tr += '<td class="CCFecha">' + d + '</td>';
        tr += '<td class="CCImporte">' + bc.Importe + '</td>';        
        tr += ' </tr>';
        return tr;
    };
    var validaCargaIndividual = function () {
        if (!$("#txtFecha").val()) {
            alert("Favor de ingresar la fecha");
            $("#txtFecha").focus();
            return false;
        }
        if (!$("#txtTipoCambio").val() || parseInt(balor.quitarFormatoMoneda($("#txtTipoCambio").val())) == 0) {
            alert("Favor de ingresar un importe valido");
            $("#txtTipoCambio").focus();
            return false;
        }
        return true;
    };

    var validaCargaMasiva = function () {
        var items = 0;
        var tieneCeros = false;
        var tieneFechaInvalida = false;
        $('#TablaTipoCambio tbody tr').each(function () {
            items++;
            if ($(this).find(".CCImporte")[0].innerText == 0)
                tieneCeros = true;
            if ($(this).find(".CCFecha")[0].innerText == "")
                tieneFechaInvalida = true;
        });
        if (items == 0) {
            alert("Favor de cargar el archivo de excel con las columnas Fecha,Importe con sus respectivos valores");
            return false;
        }
        if (tieneFechaInvalida) {
            alert("El detalle contiene fechas en blanco");
            return false;
        }
        if (tieneCeros) {
            alert("El detalle contiene importes en cero");
            return false;
        }

        return true;
        

    }

    var getEntityIndividual = function () {
        var respuesta = [];
        var item = {};
        item.Fechatipocambio = $("#txtFecha").val();
        item.Importetipocambio = balor.quitarFormatoMoneda($("#txtTipoCambio").val());
        item.Usuario = getUsuario();
        item.UltimaAct = 0;
        respuesta.push(item);
        return respuesta;
    }

    var getEntityMasiva = function () {
        var respuesta = [];
        $('#TablaTipoCambio tbody tr').each(function () {
            var item = {};
            item.Fechatipocambio = $(this).find(".CCFecha")[0].innerText;
            item.Importetipocambio = $(this).find(".CCImporte")[0].innerText;
            item.Usuario = getUsuario();
            item.UltimaAct = 0;
            respuesta.push(item);
        });
        return respuesta;
    };

    $('body').on('click', '#btnGuardar', null, function () {
        var CatTipoCambio = [];

        if ($("input[type='radio'][name='rTipo']:checked").val() == "I") {
            if (!validaCargaIndividual())
                return;
            CatTipoCambio = getEntityIndividual();
        } else {
            if (!validaCargaMasiva())
                return;
            CatTipoCambio = getEntityMasiva();

        }
        ShowLightBox(true, "Espere porfavor");
        ejecutaAjax('CapturaTipoDeCambio.aspx/GuardarTipoCambio', 'POST', { value: JSON.stringify(CatTipoCambio) }, function (d) {
            ShowLightBox(false, "");
            if (d.d.EsValido) {
                showMsg('Alerta', "Datos guardados correctamente", 'success');
            } else {
                showMsg('Alerta', d.d.Mensaje, 'error');
            }
        }, function (d) {
            ShowLightBox(false, "");
            showMsg('Alerta', d.responseText, 'error');
        }, true);
    });

    $("#btnLimpiar").click(function () {
        window.location.replace("CapturaTipoDeCambio.aspx");
    });





    var AplicarEstilo = function () {
        $(".svGrid th, .svGrid .header").css({
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
    };

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
            $("#Asignar-body").css({ 'height': '300px', 'width': '200px', 'overflow-x': 'hidden', 'overflow-y': 'scroll' });
        }
    };
    $(document).ready(function () {
        if (amplify.store.sessionStorage("UsuarioID") == null || amplify.store.sessionStorage("UsuarioID") == "") {
            window.location.replace("../../login.aspx");
        }
        $("#Menu").MostrarMenuBar(JSON.parse(amplify.store.sessionStorage("Funcionalidades")), amplify.store.sessionStorage("Nombre"), amplify.store.sessionStorage("NombreEmpresa"), amplify.store.sessionStorage("UsuarioID"));
        $('.popup').setDraggable('.popup-body, .popup-footer, .close');
        ActualizaMenuBar();
        AplicarEstilo();

        $('#TablaTipoCambio').css({ "width": "200px", "margin": "" });
    });
}(jQuery, window.balor, window.amplify));