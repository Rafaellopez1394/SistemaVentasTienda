(function ($, balor, amplify) {
    'use strict';

    //*******************************************************************************F U N C I O N E S   G E N E R A L E S*******************************************************************************************
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

    var elemById = function (id) {
        return document.getElementById(id);
    };

    var focusNextControl = function (control, e) {
        var code = (e.keyCode ? e.keyCode : e.which);
        if (code === 13) {
            var inputs = $(control).closest('form').find('input[type=text]:enabled, input[type=password]:enabled, input[type=button]:enabled, input[type=checkbox]:enabled, input[type=radio]:enabled, input[type=date]:enabled, input[type=tel]:enabled, input[type=email]:enabled, input[type=number]:enabled, select:enabled, textarea:enabled').not('input[readonly=readonly], fieldset:hidden *, *:hidden');
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

    var body = $('body');

    var getEmpresaid = function () {
        return amplify.store.sessionStorage("EmpresaID");
    };







    var TraerPeriodoActual = function () {
        ejecutaAjax('ReporteDIOT.aspx/PeriodoActual', 'POST', {}, function (d) {
            if (d.d.EsValido) {
                $("#txtIniciaPeriodo").val(d.d.Datos.IniciaPeriodo);
                $("#txtTerminaPeriodo").val(d.d.Datos.TerminaPeriodo);
                $("#lblPeriodo").text(d.d.Datos.txtPeriodo);
                if (d.d.Datos.esMayor)
                    $("#btnGenerar").attr('disabled', 'disabled')
                else
                    $("#btnGenerar").removeAttr('disabled');

            } else {
                showMsg('Alerta', d.d.Mensaje, 'warning');
            }
        }, function (d) {
            showMsg('Alerta', d.responseText, 'error');
        }, true);
    };


    body.on('click', '#btnPeriodoAnterior', null, function () {
        TraerPeriodoAnterior($("#txtIniciaPeriodo").val());
    });
    var TraerPeriodoAnterior = function (iniciaPeriodo) {
        ejecutaAjax('ReporteDIOT.aspx/PeriodoAnterior', 'POST', { IniciaActual: iniciaPeriodo }, function (d) {
            if (d.d.EsValido) {
                $("#txtIniciaPeriodo").val(d.d.Datos.IniciaPeriodo);
                $("#txtTerminaPeriodo").val(d.d.Datos.TerminaPeriodo);
                $("#lblPeriodo").text(d.d.Datos.txtPeriodo);

                if (d.d.Datos.esMayor)
                    $("#btnGenerar").attr('disabled', 'disabled')
                else
                    $("#btnGenerar").removeAttr('disabled');

            } else {
                showMsg('Alerta', d.d.Mensaje, 'warning');
            }
        }, function (d) {
            showMsg('Alerta', d.responseText, 'error');
        }, true);

    };

    body.on('click', '#btnPeriodoPosterior', null, function () {
        TraerPeriodoPosterior($("#txtTerminaPeriodo").val());
    });
    var TraerPeriodoPosterior = function (terminaPeriodo) {
        ejecutaAjax('ReporteDIOT.aspx/PeriodoPosterior', 'POST', { TerminaActual: terminaPeriodo }, function (d) {
            if (d.d.EsValido) {
                $("#txtIniciaPeriodo").val(d.d.Datos.IniciaPeriodo);
                $("#txtTerminaPeriodo").val(d.d.Datos.TerminaPeriodo);
                $("#lblPeriodo").text(d.d.Datos.txtPeriodo);

                if (d.d.Datos.esMayor)
                    $("#btnGenerar").attr('disabled', 'disabled')
                else
                    $("#btnGenerar").removeAttr('disabled');

            } else {
                showMsg('Alerta', d.d.Mensaje, 'warning');
            }
        }, function (d) {
            showMsg('Alerta', d.responseText, 'error');
        }, true);

    };




    body.on('click', '#btnGenerar', null, function () {
        ShowLightBox(true, "Espere porfavor");
        GenerarDIOT();
        /*var parametros = {};
        parametros.empresaid = getEmpresaid();
        parametros.iniciaPeriodo = $("#txtIniciaPeriodo").val();
        parametros.terminaPeriodo = $("#txtTerminaPeriodo").val();
        

        ejecutaAjax('ReporteDIOT.aspx/GenerarDIOT', 'POST', parametros, function (d) {
            if (d.d.EsValido) {
                ShowLightBox(false, "");
                showMsg('Dato', 'Archivo generado correctamente', 'success')
                $('#btnDescargar').attr('href', d.d.Datos)
                $("#btnDescargar")[0].click()
            } else {
                ShowLightBox(false, "");
                showMsg('Alerta', d.d.Mensaje, 'error');
            }
        }, function (d) {
            ShowLightBox(false, "");
            showMsg('Alerta', d.responseText, 'error');
        }, true);*/
    });

    var GenerarDIOT = function () {

        const params = {
            empresaid: getEmpresaid(),
            iniciaPeriodo: $("#txtIniciaPeriodo").val(),
            terminaPeriodo: $("#txtTerminaPeriodo").val()
        };
        const url = 'http://192.168.10.250:8080/Services/GenerarDIOT.ashx';

        fetch(url, {
            method: 'POST',
            body: JSON.stringify(params),
            headers: {
                'Content-Type': 'application/json',
            },
        })
        .then(response => {
            if (!response.ok) {
                throw new Error('Hubo un problema con la solicitud: ' + response.status);
            }
            return response.json(); 
        })
        .then(data => {
            console.log('Respuesta del servidor:', data);
            ShowLightBox(false, "");
            if (data.EsValido) {
                showMsg('Dato', 'Archivo generado correctamente', 'success')
                $('#btnDescargar').attr('href', data.Datos)
                $("#btnDescargar")[0].click()
            } else {
                showMsg('Alerta', data.Mensaje || 'Ocurrió un problema', 'warning');
                ShowLightBox(false, "");
            }
        })
        .catch(error => {
            ShowLightBox(false, "");
            showMsg('Error', error.message || 'Error desconocido', 'error');
        });
    };

    setInterval(function () {
        if (!amplify.store.sessionStorage("UsuarioID")) {
            window.location.replace("../../login.aspx");
        }
    }, 1000);

    $(document).ready(function () {
        if (!amplify.store.sessionStorage("UsuarioID")) {
            window.location.replace("../../login.aspx");
        }
        $("#Menu").MostrarMenuBar(JSON.parse(amplify.store.sessionStorage("Funcionalidades")), amplify.store.sessionStorage("Nombre"), amplify.store.sessionStorage("NombreEmpresa"), amplify.store.sessionStorage("UsuarioID"));
        $('.popup').setDraggable('.popup-body, .popup-footer, .close');

        TraerPeriodoActual();
    });
}(jQuery, window.balor, window.amplify));