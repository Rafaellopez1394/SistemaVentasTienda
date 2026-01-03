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

    var focusNextControl = function (control, e) {
        var code = (e.keyCode ? e.keyCode : e.which);
        if (code === 13) {
            var inputs = $(control).closest('form').find('input[type=text]:enabled, input[type=password]:enabled, input[type=button]:enabled, input[type=checkbox]:enabled, input[type=radio]:enabled, input[type=date]:enabled, input[type=tel]:enabled, input[type=email]:enabled, input[type=number]:enabled, select:enabled').not('input[readonly=readonly], fieldset:hidden *');
            inputs.eq(inputs.index(control) + 1).focus();
            e.preventDefault();
        }
    };

    var exportaContaExcel = {
        getUsuario: function () {
            return amplify.store.sessionStorage("Usuario");
        },
        getVendedor: function () {
            return amplify.store.sessionStorage("VendedorID");
        },
        ejecutaAjax : function (url, tipo, datos, successfunc, errorfunc, async) {
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
        },
        inicializarComponentes: function () {

            exportaContaExcel.inicializarControles();
            exportaContaExcel.inicializarMenuBar();
            exportaContaExcel.ActualizaMenuBar();
            exportaContaExcel.ejecutaAjax('Polizas.aspx/TraerEmpresas', 'POST', null, exportaContaExcel.llenarComboEmpresas, null, false);

        },
        inicializarControles:function(){

            $('body').on('click', '#BotonImprimir', null, function () {
                if (exportaContaExcel.ValidaImprimir()) {
                    var parametros = "&fechaInicial=" + $("#fechaInicio").val().trim();
                    parametros += "&fechaFinal=" + $("#fechaFinal").val().trim();
                    parametros += "&empresaid=" + $('#ddl-empresa option:selected').val();
                    parametros += "&Impresion=" + $("input[type='radio'][name='timprimir']:checked").val();
                    //parametros += "&Impresion=1";//EXCEL                    
                    window.open("../../Base/Formas/ReporteProduccion.aspx?Formato=0&Nombre=ExportaContabilidadExcel" + parametros, 'w_PopContabilidadExcel' + $("#fechaInicio").val().trim().replace("/", "") + $("#fechaFinal").val().trim().replace("/", "") + $('#ddl-empresa option:selected').val(), 'toolbar=no,location=no,directories=no,status=no,menubar=no,scrollbars=yes,resizable=yes,width=760,height=560');
                }
            });

            $('body').on('click', '#btnLimpiar', null, function () {
                exportaContaExcel.limpiarControles();
            });


            $('input').not('input[type=button]').on('keypress', null, function (e) {
                focusNextControl(this, e);
            });

            $('body').on('blur', '#fechaInicio,#fechaFinal', null, function () {
                var ffecha = $(this).val();
                $(this).val(balor.appliFormatDate(ffecha));
                if (!balor.isValidDate($(this).val())) {
                    $(this).val("");
                }
            });
        },
        inicializarMenuBar: function(){
            if (amplify.store.sessionStorage("UsuarioID") == null || amplify.store.sessionStorage("UsuarioID") == "") {
                window.location.replace("../../login.aspx");
            }
            $("#Menu").MostrarMenuBar(JSON.parse(amplify.store.sessionStorage("Funcionalidades")), amplify.store.sessionStorage("Nombre"), amplify.store.sessionStorage("NombreEmpresa"), amplify.store.sessionStorage("UsuarioID"));
            $('.popup').setDraggable('.popup-body, .popup-footer, .close');
        },
        ActualizaMenuBar: function () {
            if (screen.width < 600) {
                $("#MobileMenu").css("display", "line");
                var menu = $("#Menu").html();
                $("#divcontentMenu").remove();
                $("#MobileMenuDet").html(menu);
                $("#nav").css("top", "0");
            }
        },
        llenarComboEmpresas: function (d) {
            if (d.d.EsValido) {
                $('#ddl-empresa').llenaCombo(d.d.Datos);
            }
        },
        ValidaImprimir: function () {
            var bRegresa = true;

            if ($("#fechaInicio").val() == "") {
                showMsg('Alerta', "Debe ingresar la fecha de inicio", 'warning');
                $("#fechaInicio").focus();
                bRegresa = false;
            }
            if ( bRegresa == true && $("#fechaFinal").val() == "") {
                showMsg('Alerta', "Debe ingresar la fecha final", 'warning');
                $("#fechaFinal").focus();
                bRegresa = false;
            }

            return bRegresa;
        },
        limpiarControles() {
            $("#fechaInicio").val("");
            $("#fechaFinal").val("");
        }
    };

    $(document).ready(function () {
        exportaContaExcel.inicializarComponentes();
    });

}(jQuery, window.balor, window.amplify));