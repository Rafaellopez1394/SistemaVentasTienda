/// <reference path="../../Base/js/vendor/jquery-1.11.0-vsdoc.js" />
/// <reference path="../../Base/js/plugins.js" />
/// <reference path="../../Base/Scripts/librerias/extensions/jquery.HelpField-1.1.js" />
(function ($, balor, amplify) {
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

    var ayudaClienteFindByCode = function () {
        var Param = $('#ayudaCliente').getValuesByCode();
        Param.ID = amplify.store.sessionStorage("EmpresaID");
        var parametros = { value: JSON.stringify(Param) };
        ejecutaAjax('CatalogoClientesIntercompañia.aspx/ayudaCliente_FindByCode', 'POST', parametros, function (d) {
            if (d.d.EsValido) {
                $('#ayudaCliente').showFindByCode(d.d.Datos);
            } else {
                $('#ayudaCliente_HelpLoad').css({ "display": "none" });
                showMsg('Alerta', d.d.Mensaje, 'warning');
            }
        }, function (d) {
            showMsg('Alerta', d.responseText, 'error');
        }, true);
    };

    var ayudaClienteFindByPopUp = function () {
        var Param = $('#ayudaCliente').getValuesByPopUp();
        Param.ID = amplify.store.sessionStorage("EmpresaID");
        var parametros = { value: JSON.stringify(Param) };
        ejecutaAjax('CatalogoClientesIntercompañia.aspx/ayudaCliente_FindByPopUp', 'POST', parametros, function (d) {
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
        var parametros = { clienteid: $('#ayudaCliente').getValuesByCode().ID };
        ejecutaAjax('CatalogoClientesIntercompañia.aspx/TraerCliente', 'POST', parametros, function (d) {
            if (d.d.EsValido) {
                mostrarDatosCliente(d.d.Datos);
            } else {
                showMsg('Alerta', d.d.Mensaje, 'warning');
            }
        }, function (d) {
            showMsg('Alerta', d.responseText, 'error');
        }, true);
    };

    var mostrarDatosCliente = function (Cliente) {
        var pais = Cliente.Paisid,
        estado = Cliente.Estadoid,
        municipio = Cliente.Municipioid,
        ciudad = Cliente.Ciudadid,
        colonia = Cliente.Coloniaid;

        $("#ayudaCliente_Code").val(Cliente.Codigo);
        $("#ayudaCliente_lBox").val(Cliente.Nombre);
        $("#ayudaCliente_ValorID").val(Cliente.Clienteid);
        $("#hiddenultimaact").val(Cliente.UltimaAct);
        $("#txtrazonsocial").val(Cliente.Nombre)
        $("#txtrfcpmoral").val(Cliente.Rfc);
        elemById('txtcalle').value = Cliente.Calle;
        elemById('txtnumexterior').value = Cliente.Noexterior;
        elemById('txtnuminterior').value = Cliente.Nointerior;
        elemById('txtcodpostal').value = Cliente.Codigopostal;
        elemById('ddlpais').value = pais;
        llenacomboEstadoDom(pais, estado);
        llenacomboMunicipioDom(estado, municipio);
        llenacomboCiudadDom(municipio, ciudad);
        llenacomboColoniaDom(ciudad, colonia);
    }




    var iniciaAyudas = function () {
        $('#ayudaCliente').helpField({
            title: 'Cliente',
            codeNumeric: true,
            maxlengthCode: 6,
            enableCode: true,
            enableDescription: false,
            codePaste: false,
            //nextControlID: '',
            widthDescription: '0',
            requeridField: true,
            cultureResourcesPopUp: {
                popUpHeader: 'Búsqueda de Clientes'
            },
            findByPopUp: ayudaClienteFindByPopUp,
            findByCode: ayudaClienteFindByCode,
            onElementFound: ayudaCliente_onElementFound
        });

        $('#ayudaCliente').helpField({
            title: 'Cliente',
            codeNumeric: true,
            maxlengthCode: 6,
            enableCode: true,
            enableDescription: false,
            codePaste: false,
            //nextControlID: '',
            widthDescription: '0',
            requeridField: true,
            cultureResourcesPopUp: {
                popUpHeader: 'Búsqueda de Clientes'
            },
            findByPopUp: ayudaClienteFindByPopUp,
            findByCode: ayudaClienteFindByCode,
            onElementFound: ayudaCliente_onElementFound
        });

    };

    var getUsuario = function () {
        return amplify.store.sessionStorage("Usuario");
    };

    var llenacombo = function (comboid, dependencia, webmethod, msgSelect, defaultSelected) {
        dependencia = dependencia || {};
        ejecutaAjax('CatalogoClientesIntercompañia.aspx/' + webmethod, 'POST', dependencia, function (d) {
            if (d.d.EsValido) {
                $('#' + comboid).llenaCombo(d.d.Datos, msgSelect, defaultSelected);
            } else {
                showMsg('Alerta', d.d.Mensaje, 'warning');
            }
        }, null, true);
    };

    var llenacomboPaisDom = function () {
        llenacombo('ddlpais', {}, 'TraerPaises', 'Seleccione Pais', idpaismexico);
        llenacomboEstadoDom(idpaismexico);
        llenacomboMunicipioDom('');
        llenacomboCiudadDom('');
        llenacomboColoniaDom('');
    };

    var llenacomboEstadoDom = function (pais, defaultSelected) {
        llenacombo('ddlestado', { pais: pais }, 'TraerEstadosPorPais', 'Seleccione Estado', defaultSelected);
    };

    var llenacomboMunicipioDom = function (estado, defaultSelected) {
        llenacombo('ddlmunicipio', { estado: estado }, 'TraerMunicipiosPorEstado', 'Seleccione Municipio', defaultSelected);
    };

    var llenacomboCiudadDom = function (municipio, defaultSelected) {
        llenacombo('ddlciudad', { municipio: municipio }, 'TraerCiudadesPorMunicipio', 'Seleccione Ciudad', defaultSelected);
    };

    var llenacomboColoniaDom = function (ciudad, defaultSelected) {
        llenacombo('ddlcolonia', { ciudad: ciudad }, 'TraerColoniasPorCiudad', 'Seleccione Colonia', defaultSelected);
    };

    var codpostal = function (cp) {
        ejecutaAjax('CatalogoClientesIntercompañia.aspx/consultacp', 'POST', { cp: cp }, function (d) {
            if (d.d.EsValido) {
                var items = d.d.Datos;
                if (items && items.length) {
                    var elem = items[0], pais = elem.PaisID,
            estado = elem.EstadoID,
            municipio = elem.MunicipioID,
            ciudad = elem.CiudadID,
            msgSelect = 'Selecciona Colonia';
                    elemById('ddlpais').value = pais;
                    llenacomboEstadoDom(pais, estado);
                    llenacomboMunicipioDom(estado, municipio);
                    llenacomboCiudadDom(municipio, ciudad);

                    $('#ddlcolonia').llenaCombo(items, msgSelect);
                } else {
                    elemById('ddlpais').selectedIndex = 0;
                    llenacomboEstadoDom('');
                    llenacomboMunicipioDom('');
                    llenacomboCiudadDom('');
                    llenacomboColoniaDom('');
                }
            } else {
                showMsg('Alerta', d.d.Mensaje, 'warning');
            }
        }, null, true);
    };

    var validaGuardar = function () {
        var tipoPersona = $('input[name=persona]:radio:checked').val(),
        invalidFields = $('#form1').find(":invalid").not('fieldset'),
        validFields = $('#form1').find(":valid"),
        titulo = 'Hay valores incorrectos',
        tipo = 'error',        
        camposPerMoral = [
          { id: 'txtrazonsocial', msg: 'Favor de ingresar la razón social.' },          
          { id: 'txtrfcpmoral', msg: 'Favor de ingresar el RFC con el formato correcto ABC999999XXX, donde (ABC) son las letras correspondientes al nombre , 999999 es la fecha de nacimiento (aammdd), XXX es la homoclave.' }
          ],
        camposGeneral = [
          { id: 'txtcalle', msg: 'Favor de ingresar la calle.' },
          { id: 'txtcodpostal', msg: 'Favor de ingresar el código postal.' },
          { id: 'ddlpais', msg: 'Favor de seleccionar el pais.' },
          { id: 'ddlestado', msg: 'Favor de seleccionar el estado.' },
          { id: 'ddlmunicipio', msg: 'Favor de seleccionar el municipio.' },
          { id: 'ddlciudad', msg: 'Favor de seleccionar la ciudad.' },
          { id: 'ddlcolonia', msg: 'Favor de seleccionar la colonia.' }],
        arrayDatos = [camposGeneral];

        invalidFields.addClass('invalido');
        validFields.removeClass('invalido');

        arrayDatos.push(camposPerMoral);
        arrayDatos.reverse();

        for (var j = 0; j < arrayDatos.length; j++) {
            var l = arrayDatos[j].length,
          datos = arrayDatos[j],
          i;
            for (i = 0; i < l; i++) {
                var elem = elemById(datos[i].id);
                if (!elem.checkValidity()) {
                    showMsg(titulo, datos[i].msg, tipo);
                    elem.focus();
                    return false;
                }
            }
        }
        return true;
    };

    var GuardarCliente = function () {
        var cliente = obtenerCliente();
        ejecutaAjax('CatalogoClientesIntercompañia.aspx/GuardarCliente', 'POST', { value: JSON.stringify(cliente) }, function (d) {
            if (d.d.EsValido) {
                showMsg('Cliente Guardado', 'El cliente fue guardado correctamente.', 'success');
                mostrarDatosCliente(d.d.Datos);
            } else {
                showMsg('Error al guardar el cliente', 'No se pudo guardar el cliente. ' + d.d.Mensaje, 'error');
            }
        }, function (d) {
            showMsg('Alerta', d.responseText, 'error');
        }, true);
    };

    var obtenerCliente = function () {
        var ayuda = $('#ayudaCliente');
        var cliente = {
            Clienteid: ayuda.getValuesByCode().ID,
            Empresaid: amplify.store.sessionStorage("EmpresaID"),
            Nombre: $("#txtrazonsocial").val(),
            Rfc: elemById('txtrfcpmoral').value,
            Calle: elemById('txtcalle').value,
            Noexterior: elemById('txtnumexterior').value,
            Nointerior: elemById('txtnuminterior').value,
            Codigopostal: elemById('txtcodpostal').value,
            Paisid: elemById('ddlpais').value,
            Estadoid: elemById('ddlestado').value,
            Municipioid: elemById('ddlmunicipio').value,
            Ciudadid: elemById('ddlciudad').value,
            Coloniaid: elemById('ddlcolonia').value,
            Usuario: getUsuario(),
            UltimaAct: elemById('hiddenultimaact').value || 0
        };
        return cliente;
    };

    $('input').on('blur', function () {
        var type = $(this).attr('type');
        if (/date|email|month|number|search|tel|text|time|url|week/.test(type)) {
            if (this.checkValidity()) {
                $(this).removeClass('invalido');
            } else {
                $(this).addClass('invalido');
            }
            this.title = this.validationMessage;
        }
    });

    $('select').on('blur', function () {
        if (this.checkValidity()) {
            $(this).removeClass('invalido');
        } else {
            $(this).addClass('invalido');
        }
        this.title = this.validationMessage;
    });

    $('body').on('change', '#ddlpais', null, function () {
        llenacomboEstadoDom(elemById('ddlpais').value);
        llenacomboMunicipioDom('');
        llenacomboCiudadDom('');
        llenacomboColoniaDom('');
    });

    $('body').on('change', '#ddlestado', null, function () {
        llenacomboMunicipioDom(elemById('ddlestado').value);
        llenacomboCiudadDom('');
        llenacomboColoniaDom('');
    });

    $('body').on('change', '#ddlmunicipio', null, function () {
        llenacomboCiudadDom(elemById('ddlmunicipio').value);
        llenacomboColoniaDom('');
    });

    $('body').on('change', '#ddlciudad', null, function () {
        llenacomboColoniaDom(elemById('ddlciudad').value);
    });

    $('body').on('change', '#ddlzona', null, function () {
        llenacomboZonaDetalle(elemById('ddlzona').value);
    });

    $('body').on('change', '#ddlzonapmoral', null, function () {
        llenacomboZonaDetalleMoral(elemById('ddlzonapmoral').value);
    });


    $('body').on('change', '#ddlcolonia', null, function () {

        var parametros = { coloniaid: elemById('ddlcolonia').value };
        ejecutaAjax('../../Analisis/Formas/FuncionesGenericas.aspx/TraerColoniasPorId', 'POST', parametros, function (d) {
            if (d.d.EsValido) {
                if (d.d.Datos[0].Codigopostal != $("#txtcodpostal").val()) {
                    $("#txtcodpostal").val(d.d.Datos[0].Codigopostal);
                }
            } else {
                showMsg('Alerta', d.d.Mensaje, 'error');
            }
        }, function (d) {
            showMsg('Alerta', d.responseText, 'error');
        }, false);
    });
    $('body').on('keypress', '#txtcodpostal', null, function (e) {
        var code = (e.keyCode ? e.keyCode : e.which);
        if (code === 13) {
            e.preventDefault();
            codpostal(this.value);
        }
    });

    $('body').on('blur', '#txtcodpostal', null, function (e) {
        var combo = elemById('ddlcolonia');
        if (combo.selectedIndex <= 0) {
            codpostal(this.value);
        }
    });

    $('body').on('keypress', 'input[type=text], input[type=email], input[type=tel]', function (e) {
        return balor.onlyAlfanumeric(e);
    });
    $('input').not('input[type=button]').on('keypress', null, function (e) {
        focusNextControl(this, e);
    });
    $('select').on('keypress', null, function (e) {
        focusNextControl(this, e);
    });

    $('body').on('click', '#btnGuardar', null, function () {
        if (confirm('¿Desea guardar el cliente?')) {
            if (validaGuardar()) {
                GuardarCliente();
            }
        }
    });
    $('body').on('click', '#btnLimpiar', null, function () {
        window.location.replace('CatalogoClientesIntercompañia.aspx');
    });
    llenacomboPaisDom();
    iniciaAyudas();
    $('#ayudaCliente').setFocusHelp();

    var MostrarPagina = function (url) {
        if (url != "") {
            window.location.replace(url);
        }
    };

    $(document).ready(function () {
        if (amplify.store.sessionStorage("UsuarioID") == null || amplify.store.sessionStorage("UsuarioID") == "") {
            window.location.replace("../../login.aspx");
        }
        $("#Menu").MostrarMenuBar(JSON.parse(amplify.store.sessionStorage("Funcionalidades")), amplify.store.sessionStorage("Nombre"), amplify.store.sessionStorage("NombreEmpresa"), amplify.store.sessionStorage("UsuarioID"));
        $('.popup').setDraggable('.popup-body, .popup-footer, .close');
    });
}(jQuery, window.balor, window.amplify));