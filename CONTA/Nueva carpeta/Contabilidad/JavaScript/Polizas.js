/// <reference path="../../Base/js/vendor/jquery-1.11.0-vsdoc.js" />
/// <reference path="../../Base/js/plugins.js" />
/// <reference path="../../Base/Scripts/librerias/extensions/jquery.HelpField-1.1.js" />
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
            $('#ddl-empresa').llenaCombo(d.d.Datos, null, amplify.store.sessionStorage("EmpresaID"));
        }
    };
    var elemById = function (id) {
        return document.getElementById(id);
    };
    var consultar = function () {
        var ejercicio = $('#txtejercicio').val();
        var parametros = {};
        parametros.ejercicio = (isNaN(ejercicio) ? 0 : ejercicio);
        parametros.Empresaid = $('#ddl-empresa option:selected').val();
        ShowLightBox(true, "Espere porfavor...");
        ejecutaAjax('Polizas.aspx/Consultar', 'post', parametros, function (d) {
            ShowLightBox(false, "");
            if (d.d.EsValido) {
                showMsg('Alerta', d.d.Datos, 'success');
                llenarTablaContabilidades(parametros.Empresaid);
            } else {
                showMsg('Alerta', d.d.Mensaje, 'error');
            }
        }, function (d) {
            ShowLightBox(false, "");
            showMsg('Alerta', d.responseText, 'error');
        }, true);
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


    $('input').not('input[type=button]').on('keypress', null, function (e) {
        focusNextControl(this, e);
    });

    $('body').on('keypress', '#txtejercicio', null, function (e) {
        return balor.onlyNumeric(e);
    });

    $('body').on('click', '#btnConsultar', null, function () {
        if ($('#txtejercicio').val()) {
            consultar();
        } else {
            $('#txtejercicio').focus();
            showMsg("Ingresar", "Ingrese el año", "warning");
        }
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
    }

    var llenarTablaContabilidades = function (empresaid) {
        ejecutaAjax('Polizas.aspx/ConsultarContabilidades', 'POST', { Empresaid: empresaid }, function (d) {
            if (d.d.EsValido) {
                var datos = JSON.parse(d.d.Datos); // Parsear los datos JSON
                var tbody = $('#tbodyContabilidades');
                tbody.empty(); // Limpiar filas existentes

                if (datos.length > 0) {
                    // Iterar sobre los datos y crear filas en la tabla
                    $.each(datos, function (index, row) {
                        var tr = $('<tr></tr>');
                        tr.append('<td>' + row.Año + '</td>');
                        tr.append('<td>' + row.TipoContable + '</td>');
                        tbody.append(tr);
                    });
                } else {
                    tbody.append('<tr><td colspan="2">No se encontraron datos.</td></tr>');
                }
            } else {
                showMsg('Error', d.d.Mensaje, 'error');
                $('#tbodyContabilidades').empty().append('<tr><td colspan="2">No se encontraron datos.</td></tr>');
            }
        }, function (d) {
            showMsg('Error', 'Error al consultar contabilidades.', 'error');
            $('#tbodyContabilidades').empty().append('<tr><td colspan="2">Error al cargar los datos.</td></tr>');
        }, true);
    };
    $(document).ready(function () {
        if (amplify.store.sessionStorage("UsuarioID") == null || amplify.store.sessionStorage("UsuarioID") == "") {
            window.location.replace("../../login.aspx");
        }
        $("#Menu").MostrarMenuBar(JSON.parse(amplify.store.sessionStorage("Funcionalidades")), amplify.store.sessionStorage("Nombre"), amplify.store.sessionStorage("NombreEmpresa"), amplify.store.sessionStorage("UsuarioID"));
        $('.popup').setDraggable('.popup-body, .popup-footer, .close');
        ActualizaMenuBar();
        ejecutaAjax('Polizas.aspx/TraerEmpresas', 'POST', null, llenarComboEmpresas, null, false);
        $('#txtejercicio').focus();

        // Activar la carga de la tabla al cambiar la empresa
        $('#ddl-empresa').on('change', function () {
            var empresaid = $(this).val();
            if (empresaid) {
                llenarTablaContabilidades(empresaid);
            } else {
                $('#tbodyContabilidades').empty().append('<tr><td colspan="2">Seleccione una empresa.</td></tr>');
            }
        });

        // Carga inicial para la empresa seleccionada por defecto
        var defaultEmpresaid = $('#ddl-empresa').val();
        if (defaultEmpresaid) {
            llenarTablaContabilidades(defaultEmpresaid);
        }
    });
} (jQuery, window.balor, window.amplify));