/// <reference path="../../Base/js/vendor/jquery-1.11.0-vsdoc.js" />
/// <reference path="../../Base/js/plugins.js" />
/// <reference path="../../Base/Scripts/librerias/extensions/jquery.HelpField-1.1.js" />
(function ($, balor, amplify) {
    var msgbx = $('#divmsg');
    var clock = null;
    var idpaismexico = "d693195e-0c3c-490f-a664-cba97d790f42";

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

    InstanciarAyudas = function () {
        $('#AyudaProveedores').helpField({
            title: 'Proveedores',
            codeNumeric: false,
            maxlengthCode: 30,
            enableCode: false,
            enableDescription: true,
            codePaste: false,
            //nextControlID: 'txtcalle',
            widthCode: 170,
            //widthDescription: '0',
            requeridField: false,
            cultureResourcesPopUp: {
                popUpHeader: 'Búsqueda de Proveedores'
            },
            findByPopUp: AyudaProveedores_FindByPopUp,
            findByCode: AyudaProveedores_FindByCode,
            onElementFound: AyudaProveedores_onElementFound
        });

        $('#AyudaCuenta').helpField({
            title: 'Cuenta',
            codeNumeric: true,
            maxlengthCode: 30,
            enableCode: true,
            enableDescription: true,
            codePaste: false,
            //nextControlID: 'txtImporte',
            widthCode: 170,
            //widthDescription: '0',
            requeridField: false,
            cultureResourcesPopUp: {
                popUpHeader: 'Búsqueda de Cuentas'
            },
            findByPopUp: ayudaCuenta_FindByPopUp,
            findByCode: ayudaCuenta_FindByCode,
            onElementFound: ayudaCuenta_onElementFound
        });
    }

    var AyudaProveedores_FindByPopUp = function () {
        var paramAyuda = $('#AyudaProveedores').getValuesByPopUp();
        paramAyuda.ID = amplify.store.sessionStorage("EmpresaID");
        var parametros = { value: JSON.stringify(paramAyuda) };
        ejecutaAjax('CatProveedores.aspx/AyudaProveedores_FindByPopUp', 'POST', parametros, function (d) {
            if (d.d.EsValido) {
                $('#AyudaProveedores').showFindByPopUp(d.d.Datos);
            } else {
                showMsg('Alerta', d.d.Mensaje, 'warning');
            }
        }, function (d) {
            showMsg('Alerta', d.responseText, 'error');
        }, true);
    };

    var AyudaProveedores_FindByCode = function () {
        var paramAyuda = $('#AyudaProveedores').getValuesByCode();
        paramAyuda.ID = amplify.store.sessionStorage("EmpresaID");
        var parametros = { value: JSON.stringify(paramAyuda) };
        ejecutaAjax('CatProveedores.aspx/AyudaProveedores_FindByCode', 'POST', parametros, function (d) {
            if (d.d.EsValido) {
                $('#AyudaProveedores').showFindByCode(d.d.Datos);

            } else {
                $('#AyudaProveedores_HelpLoad').css({ "display": "none" });
                showMsg('Alerta', d.d.Mensaje, 'warning');
            }
        }, function (d) {
            showMsg('Alerta', d.responseText, 'error');
        }, true);
    };

    var AyudaProveedores_onElementFound = function () {
        //Se dispara cuando se encuentra o selecciona un item de la ayuda
        var parametros = { Codigo: $('#AyudaProveedores_Code').val(), Empresaid: amplify.store.sessionStorage("EmpresaID") }
        ejecutaAjax('CatProveedores.aspx/TraerProveedorPorCodigo', 'POST', parametros, function (d) {
            if (d.d.EsValido) {
                ProcesaProveedor(d.d.Datos);
            } else {
                $('#AyudaProveedores_HelpLoad').css({ "display": "none" });
                showMsg('Alerta', d.d.Mensaje, 'warning');
            }
        }, function (d) {
            showMsg('Alerta', d.responseText, 'error');
        }, true);
    };

    var ayudaCuenta_FindByCode = function () {
        var paramAyuda = $('#AyudaCuenta').getValuesByCode();
        var codigo = llenarSubcuentas(paramAyuda.Codigo).replace(/-/g, '');
        paramAyuda.Codigo = padRight(codigo, 24);
        paramAyuda.ID = amplify.store.sessionStorage("EmpresaID");
        var parametros = { value: JSON.stringify(paramAyuda) };
        ejecutaAjax('CapturaPolizas.aspx/AyudaCuenta_FindByCode', 'POST', parametros, function (d) {
            $('#AyudaCuenta').showFindByCode(d.d.Datos);
        }, function (d) {
            showMsg('Alerta', d.responseText, 'error');
        }, true);
    };

    var ayudaCuenta_FindByPopUp = function () {
        var paramAyuda = $('#AyudaCuenta').getValuesByPopUp();
        paramAyuda.ID = amplify.store.sessionStorage("EmpresaID");
        var parametros = { value: JSON.stringify(paramAyuda) };
        ejecutaAjax('CapturaPolizas.aspx/AyudaCuenta_FindByPopUp', 'POST', parametros, function (d) {
            if (d.d.EsValido) {
                $('#AyudaCuenta').showFindByPopUp(d.d.Datos);
            } else {
                showMsg('Alerta', d.d.Mensaje, 'warning');
            }
        }, function (d) {
            showMsg('Alerta', d.responseText, 'error');
        }, true);
    };

    var ayudaCuenta_onElementFound = function () {
        var cadena = $('#AyudaCuenta_Code').val();
        var parametros = { EmpresaID: amplify.store.sessionStorage("EmpresaID"), Cuentacontable: cadena };
        ejecutaAjax('CatProveedores.aspx/TraerProveedorPorCuentaContable', 'POST', parametros, function (d) {
            if (d.d.EsValido) {
                if (d.d.Datos.Proveedorid == "00000000-0000-0000-0000-000000000000") {
                    $('#AyudaCuenta_Code').val(cadena);
                }
                else {
                    showMsg('En uso', 'La cuenta contable ya esta con el proveedor ' + d.d.Datos.Nombre, 'warning');
                    var value = [{ ID: "", Codigo: "", Descripcion: "" }];
                    $('#AyudaCuenta_ValorID').val("");
                    $('#AyudaCuenta_Code').val("");
                    $('#AyudaCuenta_lBox').val("");
                }
            } else {
                showMsg('Alerta', d.d.Mensaje, 'error');
            }
        }, function (d) {
            showMsg('Alerta', d.responseText, 'error');
        }, false);
    };
    
    $("#btnLimpiar").click(function () {
        window.location.replace("CatProveedores.aspx");
    });

    $('input').not('input[type=button]').on('keypress', null, function (e) {
        focusNextControl(this, e);
    });
    $("#AyudaProveedores_lBox").on('keypress', null, function (e) {
        focusNextControl(this, e);
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

    var llenacombo = function (comboid, dependencia, webmethod, msgSelect, defaultSelected) {
        dependencia = dependencia || {};
        ejecutaAjax('../../Analisis/Formas/Prospectos.aspx/' + webmethod, 'POST', dependencia, function (d) {
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

    //var llenacomboTipoOperacion = function (tipoOperacion, defaultSelected) {
    //    llenacombo('ddlTipoOperacion', { tipoOperacion: tipoOperacion }, 'TraerTipoOperacion', 'Seleccione tipo operacion', defaultSelected);
    //};

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

    var codpostal = function (cp) {
        ejecutaAjax('../../Analisis/Formas/Prospectos.aspx/consultacp', 'POST', { cp: cp }, function (d) {
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


    $('body').on('keypress', '#txtRFC', null, function (e) {
        var code = (e.keyCode ? e.keyCode : e.which);
        if (code === 13) {
            e.preventDefault();
            ClientePorRFC(this.value);
        }
    });

    $('body').on('blur', '#txtRFC', null, function (e) {
            ClientePorRFC(this.value);        
    });

    var ClientePorRFC = function (rfcCliente) {
        var parametros = { RFC: rfcCliente, EmpresaID: amplify.store.sessionStorage("EmpresaID") };
        ejecutaAjax('CatProveedores.aspx/TraerDatosProveedor', 'POST', parametros, function (d) {
            if (d.d.EsValido) {
                ProcesaProveedor(d.d.Datos);
            } else {
                showMsg('Alerta', d.d.Mensaje, 'error');
            }
        }, function (d) {
            showMsg('Alerta', d.responseText, 'error');
        }, false);
    };

    var ProcesaProveedor = function (DatosProv) {
        if (DatosProv.Proveedorid != "00000000-0000-0000-0000-000000000000")
        {
            $("#txtRFC").attr("disabled", "disabled");
            $("#txtRFC").val(DatosProv.Rfc);
            //$("#txtCuentaContable").val(formatoCuenta(Datos.Cuentacontable));
            $("#txtcalle").val(DatosProv.Calle);
            $("#txtnumexterior").val(DatosProv.Noexterior);
            $("#txtnuminterior").val(DatosProv.Nointerior);
            $("#txtcodpostal").val(DatosProv.Codigopostal);
            $("#hUltimaAct").val(DatosProv.UltimaAct);
            $("#ddlTipoOperacion").val(DatosProv.Tipooperacion);
            
            var value = [{ ID: DatosProv.Proveedorid, Codigo: DatosProv.Codigo, Descripcion: DatosProv.Nombre }];
            $('#AyudaProveedores').setValuesByCode(value);
            //llenacomboEstadoDom(DatosProv.Paisid, DatosProv.Estadoid);
            //llenacomboMunicipioDom(DatosProv.Estadoid, DatosProv.Municipioid);
            //llenacomboCiudadDom(DatosProv.Municipioid, DatosProv.Ciudadid);
            //llenacomboColoniaDom(DatosProv.Ciudadid, DatosProv.Coloniaid);

            if (DatosProv.Cuentacontable != "" && DatosProv.Cuentacontable != null)
            {
                var parametros = { Cuentacontable: DatosProv.Cuentacontable, EmpresaID: amplify.store.sessionStorage("EmpresaID") };
                ejecutaAjax('CatProveedores.aspx/TraerDatosCuenta', 'POST', parametros, function (d) {
                    if (d.d.EsValido) {
                        var value = [{ ID: d.d.Datos.Cuentaid, Codigo: formatoCuenta(d.d.Datos.Cuenta), Descripcion: d.d.Datos.Descripcion }];
                        $('#AyudaCuenta').setValuesByCode(value);
                    } else {
                        showMsg('Alerta', d.d.Mensaje, 'error');
                    }
                }, function (d) {
                    showMsg('Alerta', d.responseText, 'error');
                }, false);
            }
        }
        else
        {
            var value = [{ ID: DatosProv.Proveedorid, Codigo: DatosProv.Codigo, Descripcion: "" }];
            $('#AyudaProveedores').setValuesByCode(value);
            //$("#txtCuentaContable").val(formatoCuenta(Datos.Cuentacontable));
            $("#hUltimaAct").val(0);
        }
    };

    $('body').on('click', '#btnGuardar', null, function (e) {
        
        var parametros = {};
        parametros.Proveedorid = $('#AyudaProveedores_ValorID').val();
        parametros.Codigo = $('#AyudaProveedores_Code').val();
        parametros.Nombre = $('#AyudaProveedores_lBox').val().toUpperCase();
        parametros.Empresaid = amplify.store.sessionStorage("EmpresaID");
        parametros.Cuentacontable = $('#AyudaCuenta_Code').val().replace(/-/g, '');
        parametros.Rfc = $('#txtRFC').val().toUpperCase();
        //parametros.Paisid = $('#ddlpais').val();
        //parametros.Estadoid = $('#ddlestado').val();
        //parametros.Municipioid = $('#ddlmunicipio').val();
        //parametros.Ciudadid = $('#ddlciudad').val();
        //parametros.Coloniaid = $('#ddlcolonia').val();
        parametros.Calle = $('#txtcalle').val().toUpperCase();
        parametros.Noexterior = $('#txtnumexterior').val().toUpperCase();
        parametros.Nointerior = $('#txtnuminterior').val().toUpperCase();
        parametros.Codigopostal = $('#txtcodpostal').val();
        parametros.Estatus = 1;
        parametros.Usuario = getUsuario();
        parametros.UltimaAct = $('#hUltimaAct').val();
        parametros.Tipooperacion = $('#ddlTipoOperacion').val();

        ejecutaAjax('CatProveedores.aspx/GuardarProveedor', 'POST', { value: JSON.stringify(parametros) }, function (d) {
            if (d.d.EsValido) {
                ProcesaProveedor(d.d.Datos);
                showMsg('Guardado', 'El proveedor se guardo correctamente', 'success');
            } else {
                showMsg('Alerta', d.d.Mensaje, 'error');
            }
        }, function (d) {
            showMsg('Alerta', d.responseText, 'error');
        }, false);
    });











    
    //llenacomboPaisDom();

    $(document).ready(function () {
        if (amplify.store.sessionStorage("UsuarioID") == null || amplify.store.sessionStorage("UsuarioID") == "") {
            window.location.replace("../../login.aspx");
        }
        $("#Menu").MostrarMenuBar(JSON.parse(amplify.store.sessionStorage("Funcionalidades")), amplify.store.sessionStorage("Nombre"), amplify.store.sessionStorage("NombreEmpresa"), amplify.store.sessionStorage("UsuarioID"));
        $('.popup').setDraggable('.popup-body, .popup-footer, .close');
        ActualizaMenuBar();
        AplicarEstilo();
        InstanciarAyudas();
        $("#AyudaProveedores_Code").attr("disabled", "disabled");
        $("#txtRFC").focus();

        llenacombo('ddlTipoOperacion', {}, 'TraerTipoOperacion', 'Seleccione tipo operacion', '');
    });
}(jQuery, window.balor, window.amplify));