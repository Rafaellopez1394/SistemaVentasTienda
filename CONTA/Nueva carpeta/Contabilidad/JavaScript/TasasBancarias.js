(function ($, balor, amplify) {
    var msgbx = $('#divmsg');
    var clock = null;

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

    var body = $('body');

    body.on('keypress', '#TablaTasas input:not(input[type=button])', function (e) {
        focusNextControlGridVertical(this, e);
    });

    $('body').on('click', '#btnConsultar', null, function () {
        ShowLightBox(true, "Espere porfavor");
        ejecutaAjax('TasasBancarias.aspx/ConsultaTasas', 'POST', { anio: $("#txtAnio").val() }, function (d) {
            ShowLightBox(false, "");
            if (d.d.EsValido) {
                ProcesarTasasBancarias(d.d.Datos)
            } else {
                showMsg('Alerta', d.d.Mensaje, 'error');
            }
        }, function (d) {
            ShowLightBox(false, "");
            showMsg('Alerta', d.responseText, 'error');
        }, true);
    });

    var ProcesarTasasBancarias = function (Datos) {
        var lstMeses = [];
        for (var i = 1; i <= 12; i++)
        {
            var Mes = {}
            Mes.CeteTiieID = 0;
            Mes.NumMes = i;
            Mes.NombreMes = NombreMes(i);
            Mes.Cetes = 0.0;
            Mes.Tiie = 0.0;
            Mes.UltimaAct = 0;
            lstMeses.push(Mes);
        }

        for (var i = 0; i < Datos.length; i++) {
            var NumMes = (Datos[i].Mes - 1);
            lstMeses[NumMes].CeteTiieID = Datos[i].Cetetiieid;
            lstMeses[NumMes].Cetes = Datos[i].Cetes;
            lstMeses[NumMes].Tiie = Datos[i].Tiie;
            lstMeses[NumMes].UltimaAct = Datos[i].UltimaAct;
        }
        

        var resHtml = '';
        var old = "odd";
        var entra = false;
        var index1 = 10;
        var index2 = 50;
        for (var i = 0; i < lstMeses.length; i++) {
            resHtml += '<tr class="trTasa ' + (entra ? old : '') + '" data-NumMes = "' + lstMeses[i].NumMes + '" data-CeteTiieID="' + lstMeses[i].CeteTiieID + '" data-UltimaAct="' + lstMeses[i].UltimaAct + '">';
            resHtml += '<td class="NomMes">' + lstMeses[i].NombreMes + '</td>';
            resHtml += '<td ><input type="text" tabindex="' + (index1 + i) + '" class="txtCetes inputcorto" onfocus="this.select()" value="' + lstMeses[i].Cetes + '"></td>';
            resHtml += '<td ><input type="text" tabindex="' + (index2 + i) + '" class="TxtTiee inputcorto" onfocus="this.select()" value="' + lstMeses[i].Tiie + '"></td>';
            resHtml += '</tr>';
            entra = (!entra ? true : false);
        }
        $("#TablaTasas tbody").html(resHtml);
    };

    $("#btnLimpiar").click(function () {
        window.location.replace("TasasBancarias.aspx");
    });

    var NombreMes = function (NumMes) {

        switch (NumMes)
        {
            case 1:
                return "Enero";
                break;
            case 2:
                return "Febrero";
                break;
            case 3:
                return "Marzo";
                break;
            case 4:
                return "Abril";
                break;
            case 5:
                return "Mayo";
                break;
            case 6:
                return "Junio";
                break;
            case 7:
                return "Julio";
                break;
            case 8:
                return "Agosto";
                break;
            case 9:
                return "Septiembre";
                break;
            case 10:
                return "Octubre";
                break;
            case 11:
                return "Noviembre";
                break;
            case 12:
                return "Diciembre";
                break;
        }
    };


    $('body').on('click', '#btnGuardar', null, function () {
        var lstMeses = []
        $("#TablaTasas tbody tr").each(function () {
            var Mes = {}
            Mes.Cetetiieid = $(this).attr("data-CeteTiieID");
            Mes.Año = $("#txtAnio").val();
            Mes.Mes = $(this).attr("data-NumMes");
            Mes.Cetes = $(this).find(".txtCetes").val();
            Mes.Tiie = $(this).find(".TxtTiee").val();
            Mes.Estatus = 1;
            Mes.Usuario = getUsuario();
            Mes.UltimaAct = $(this).attr("data-UltimaAct");
            lstMeses.push(Mes);
        })


        ejecutaAjax('TasasBancarias.aspx/GuardarTasas', 'POST', { value: JSON.stringify(lstMeses) }, function (d) {
            ShowLightBox(false, "");
            if (d.d.EsValido) {
                showMsg('Alerta', "", 'success');
                ProcesarTasasBancarias(d.d.Datos);
            } else {
                showMsg('Alerta', d.d.Mensaje, 'error');
            }
        }, function (d) {
            ShowLightBox(false, "");
            showMsg('Alerta', d.responseText, 'error');
        }, true);
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
        
        $('#txtAnio').focus();

        $('#TablaTasas').css({ "width": "200px", "margin": "" });
    });
}(jQuery, window.balor, window.amplify));