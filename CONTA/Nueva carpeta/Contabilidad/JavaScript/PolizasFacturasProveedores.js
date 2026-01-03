/// <reference path="../../Base/js/vendor/jquery-1.11.0-vsdoc.js" />
/// <reference path="../../Base/js/plugins.js" />
/// <reference path="../../Base/Scripts/librerias/extensions/jquery.HelpField-1.1.js" />
(function ($, balor, amplify) {
    var msgbx = $('#divmsg');

    var showMsg = function (titulo, mensaje, tipoMensaje) {
        msgbx.mostrarMensaje(titulo, mensaje, tipoMensaje);
    };

    var ShowLightBox = function (Mostrar, msj) {
        if (Mostrar) {
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

    var formatDate = function (dateString) {
        var date = new Date(dateString);
        return date.getDate().toString().padStart(2, '0') + "/" +
               (date.getMonth() + 1).toString().padStart(2, '0') + "/" +
               date.getFullYear();
    };

    var formatCurrency = function (amount) {
        return parseFloat(amount).toLocaleString('es-MX', { style: 'currency', currency: 'MXN' });
    };

    var consultar = function () {
        var uuid = $('#txtUUID').val().trim();
        if (!uuid || uuid.length !== 36) {
            showMsg('Alerta', 'Por favor, ingrese un UUID válido (36 caracteres).', 'error');
            $('#Resultados').hide();
            $('#txtUUID').focus();
            return;
        }

        var parametros = { UUID: uuid };
        ShowLightBox(true, "Espere por favor...");
        ejecutaAjax('PolizasFacturasProveedores.aspx/ConsultarPolizaPorUUID', 'POST', parametros, function (d) {
            ShowLightBox(false, "");
            if (d.d.EsValido) {
                var data = d.d.Datos;
                if (data && data.length > 0) {
                    var tbody = $('#tblPolizas tbody');
                    tbody.empty();
                    $.each(data, function (index, row) {
                        var tr = $('<tr>');
                        tr.append('<td>' + formatDate(row.Fecha) + '</td>');
                        tr.append('<td>' + row.TipPol + '</td>');
                        tr.append('<td>' + row.NumPol + '</td>');
                        tr.append('<td>' + row.RFC + '</td>');
                        tr.append('<td>' + row.Proveedor + '</td>');
                        tr.append('<td>' + formatCurrency(row.Importe) + '</td>');
                        if (index % 2 === 0) {
                            tr.addClass('highlight');
                        }
                        tbody.append(tr);
                    });
                    $('#Resultados').show();
                    showMsg('Alerta', 'Consulta realizada correctamente.', 'success');
                } else {
                    $('#Resultados').hide();
                    showMsg('Alerta', 'No se encontraron pólizas para el UUID proporcionado.', 'error');
                }
            } else {
                $('#Resultados').hide();
                showMsg('Alerta', d.d.Mensaje, 'error');
            }
            $('#btnConsultar').attr('disabled', false);
        }, function (d) {
            ShowLightBox(false, "");
            showMsg('Alerta', d.responseText, 'error');
            $('#Resultados').hide();
            $('#btnConsultar').attr('disabled', false);
        }, true);
    };

    $(document).ready(function () {
        if (amplify.store.sessionStorage('UsuarioID') == null || amplify.store.sessionStorage('UsuarioID') == '') {
            window.location.replace('../../login.aspx');
        }
        $('#Menu').MostrarMenuBar(JSON.parse(amplify.store.sessionStorage('Funcionalidades')), amplify.store.sessionStorage('Nombre'), amplify.store.sessionStorage('NombreEmpresa'));
        $('.popup').setDraggable('.popup-body, .popup-footer, .close');

        $('body').on('click', '#btnConsultar', null, function () {
            $('#btnConsultar').attr('disabled', true);
            consultar();
        });

        $('#txtUUID').focus();
    });
})(jQuery, window.balor, window.amplify);