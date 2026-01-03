(function ($, balor, amplify) {
    /*
    ==================================================
    REGION DE FUNCIONES GENERALES
    ==================================================
    */
    var msgbx = $('#divmsg');    
    var body = $('body');

    function redondear(value, places) {
        var multiplier = Math.pow(10, places);
        return (Math.round(value * multiplier) / multiplier);
    }

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

    /*
    ==================================================
    REGION DE AYUDAS
    ==================================================
    */
    var AyudaCompraFindByCode = function () {
        var paramAyuda = $('#AyudaCompra').getValuesByCode();
        paramAyuda.ID = amplify.store.sessionStorage("EmpresaID");
        var parametros = { value: JSON.stringify(paramAyuda) };
        ejecutaAjax('CapturaCompras.aspx/AyudaCompra_FindByCode', 'POST', parametros, function (d) {
            $('#AyudaCompra').showFindByCode(d.d.Datos);
        }, function (d) {
            showMsg('Alerta', d.responseText, 'error');
        }, true);
    };

    var AyudaCompraFindByPopUp = function () {
        var paramAyuda = $('#AyudaCompra').getValuesByPopUp();
        paramAyuda.ID = amplify.store.sessionStorage("EmpresaID");
        var parametros = { value: JSON.stringify(paramAyuda) };
        ejecutaAjax('CapturaCompras.aspx/AyudaCompra_FindByPopUp', 'POST', parametros, function (d) {
            if (d.d.EsValido) {
                $('#AyudaCompra').showFindByPopUp(d.d.Datos);
            } else {
                showMsg('Alerta', d.d.Mensaje, 'warning');
            }
        }, function (d) {
            showMsg('Alerta', d.responseText, 'error');
        }, true);
    };

    var AyudaCompra_onElementFound = function () {
        var parametros = {};
        parametros.CompraID = $('#AyudaCompra').getValuesByCode().ID;
        ShowLightBox(true, "Espere porfavor");
        ejecutaAjax('CapturaCompras.aspx/TraerCompra', 'POST', parametros, function (d) {
            ShowLightBox(false, "");
            if (d.d.EsValido) {
                if (d.d.Datos.Poliza.UltimaAct != 0) {
                    $("#btnImprimir").removeAttr("disabled");
                    $("#PolizaID").val(d.d.Datos.Poliza.Polizaid);                    
                }
                $("#AyudaCompra_Code").val(d.d.Datos.Compra.Folio);
                $("#AyudaCompra_ValorID").val(d.d.Datos.Compra.Compraid);
                $("#txtFechaPol").val(d.d.Datos.Compra.Fecha);

                $("#AyudaProveedor_Code").val(d.d.Datos.Proveedor.Codigo);
                $("#AyudaProveedor_lBox").val(d.d.Datos.Proveedor.Nombre);
                $("#AyudaProveedor_ValorID").val(d.d.Datos.Proveedor.Proveedorid);
                $("#AyudaProveedor").EnableHelpField(false);

                ProcesarGridFacturas(d.d.Datos.ListaCatfacturasproveedor);
                $("#ContenedorFacturas").show("slow");
                $('#TablaFacturas th:eq(9)').hide();
                $('#TablaFacturas td:nth-child(10)').hide();
                $("#btnGuardar").attr("disabled", "disabled");
                $("#btnCancelar").removeAttr("disabled");

            } else {
                showMsg('Alerta', d.d.Mensaje, 'warning');
            }
        }, function (d) {
            ShowLightBox(false, "");
            showMsg('Alerta', d.responseText, 'error');
        }, true);
    };


    var AyudaProveedorFindByCode = function () {
        var paramAyuda = $('#AyudaProveedor').getValuesByCode();
        paramAyuda.ID = amplify.store.sessionStorage("EmpresaID");        
        var parametros = { value: JSON.stringify(paramAyuda) };
        ejecutaAjax('CapturaCompras.aspx/AyudaProveedor_FindByCode', 'POST', parametros, function (d) {
            $('#AyudaProveedor').showFindByCode(d.d.Datos);
        }, function (d) {
            showMsg('Alerta', d.responseText, 'error');
        }, true);
    };

    var AyudaProveedorFindByPopUp = function () {
        var paramAyuda = $('#AyudaProveedor').getValuesByPopUp();        
        paramAyuda.ID = amplify.store.sessionStorage("EmpresaID");
        var parametros = { value: JSON.stringify(paramAyuda) };
        ejecutaAjax('CapturaCompras.aspx/AyudaProveedor_FindByPopUp', 'POST', parametros, function (d) {
            if (d.d.EsValido) {
                $('#AyudaProveedor').showFindByPopUp(d.d.Datos);
            } else {
                showMsg('Alerta', d.d.Mensaje, 'warning');
            }
        }, function (d) {
            showMsg('Alerta', d.responseText, 'error');
        }, true);
    };
    var AyudaProveedor_onElementFound = function () {
        $("#upload,#Manual").removeClass("hidden");
        $("#ContenedorFacturas").show("slow");
        $("#maxFacturas").hide();
        $("#minFacturas").show();
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
        $('#AyudaCuenta_Code').val(formatoCuenta(cadena));
    };

    
    var iniciaAyudas = function () {
        $('#AyudaCompra').helpField({
            title: 'Compra',
            codeNumeric: true,
            maxlengthCode: 6,
            enableCode: true,
            enableDescription: false,
            codePaste: false,            
            widthDescription: '0',
            requeridField: false,
            cultureResourcesPopUp: {
                popUpHeader: 'Búsqueda de compras'
            },
            findByPopUp: AyudaCompraFindByPopUp,
            findByCode: AyudaCompraFindByCode,
            onElementFound: AyudaCompra_onElementFound
        });

        $('#AyudaProveedor').helpField({
            title: 'Proveedor',
            codeNumeric: true,
            maxlengthCode: 6,
            enableCode: true,
            enableDescription: false,
            codePaste: false,
            //nextControlID: 'ddlzona',
            //widthDescription: '0',
            requeridField: false,
            cultureResourcesPopUp: {
                popUpHeader: 'Búsqueda de proveedores'
            },
            findByPopUp: AyudaProveedorFindByPopUp,
            findByCode: AyudaProveedorFindByCode,
            onElementFound: AyudaProveedor_onElementFound
        });

        $('#AyudaCuenta').helpField({
            title: 'Cuenta',
            codeNumeric: false,
            maxlengthCode: 30,
            enableCode: true,
            enableDescription: false,
            codePaste: false,
            nextControlID: 'txtImporte',
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
    };

    body.on('keypress', 'input:not(input[type=button])', function (e) {
        focusNextControl(this, e);
    });


/*
  ==================================================
  REGION FACTURAS
  ==================================================
*/
    var ProcesarGridFacturas = function (Datos) {
        var resHtml = '';
        var importetotal = 0;
        for (var i = 0; i < Datos.length; i++) {
            var ImporteAsignado = 0;
            for (var j = 0; j < Datos[i].Detalle.length; j++)
                ImporteAsignado += Datos[i].Detalle[j].Importe;
            var jsonDetalle = { DetalleContable: Datos[i].Detalle };
            resHtml += '<tr ' + (ImporteAsignado == 0 ? '' : (redondear(ImporteAsignado, 10) == redondear(Datos[i].Subtotal, 10) ? 'style="background-color:lightgreen;"' : 'style="background-color:blanchedalmond;"')) + ' class="rgFactura" data-Factura="' + Datos[i].Factura + '" data-Uuid="' + Datos[i].Uuid + '" data-Fechatimbre="' + Datos[i].Fechatimbre + '"  data-file="' + Datos[i].Rutaxml + '" data-Compraid="' + Datos[i].Compraid + '" data-Facturaproveedorid="' + Datos[i].Facturaproveedorid + '" data-Proveedorid="' + Datos[i].Proveedorid + '" data-UltimaAct="' + Datos[i].UltimaAct + '" data-Pagada="' + Datos[i].Pagada + '" data-Estatus="' + Datos[i].Estatus + '" >';
            resHtml += '<td >' + Datos[i].Factura + '' + "<input type='hidden' class='CDetalle' value='" + JSON.stringify(jsonDetalle) + "' />" + '</td>';
            resHtml += '<td>' + Datos[i].Fechatimbre + '</td>';
            resHtml += '<td class="CSubtotal">' + balor.aplicarFormatoMoneda(Datos[i].Subtotal, "$") + '</td>';
            resHtml += '<td class="CIva">' + balor.aplicarFormatoMoneda(Datos[i].Iva, "$") + '</td>';
            resHtml += '<td class="CRetIva">' + balor.aplicarFormatoMoneda(Datos[i].RetIva, "$") + '</td>';
            resHtml += '<td class="CRetIsr">' + balor.aplicarFormatoMoneda(Datos[i].RetIsr, "$") + '</td>';
            resHtml += '<td class="CTotal">' + balor.aplicarFormatoMoneda(Datos[i].Total, "$") + '</td>';
            resHtml += '<td><input type="checkbox" class="chkPasivo" ' + (Datos[i].Generapasivo ? "checked" : "") + ' /></td>';
            resHtml += '<td><input type="checkbox" class="chkDlls" ' + (Datos[i].Dlls ? "checked" : "") + ' /></td>';
            resHtml += '<td class="tdeliminar centrado"><span class="closebtn elim-Factura" >X</span></td>';
            resHtml += '</tr>';
            importetotal += Datos[i].Total;
        }
        $("#TablaFacturas tbody").html(resHtml);
    };
    var calcularTotales = function () {
        //var ImporteTotal = 0;
        //var ImportePasivo = 0;
        //$("#TablaFacturas tbody tr").each(function () {
        //    if ($(this).find('.chkPasivo').is(":checked"))
        //        ImportePasivo += balor.quitarFormatoMoneda($(this).find('.CTotal')[0].innerText);
        //    else
        //        ImporteTotal += balor.quitarFormatoMoneda($(this).find('.CTotal')[0].innerText);
        //});
        //$("#ImporteTotal")[0].innerText = balor.aplicarFormatoMoneda(ImporteTotal, "$");
        //$("#ImporteTotalPasivo")[0].innerText = balor.aplicarFormatoMoneda(ImportePasivo, "$");
    };

    $('body').on('click', '.elim-Factura', null, function () {
        if (confirm('¿Desea eliminar esta factura?')) {
            $(this).parents('tr').remove();
            $("#facturaenproceso").val("");
            $("#uuidenproceso").val("");
            $("#ContenedorGasto").hide("slow");
            $("#maxDetGasto").show();
            $("#minDetGasto").hide();
            $("#AyudaCuenta").clearHelpField();
            $("#txtConceptoPoliza, #txtImporte").val("");
            $("#lblDetalleGasto")[0].innerText = "Detalle";
            $("#lblImporteFactura")[0].innerText = "";
            $("#lblCargado")[0].innerText = "";
            $("#lblPorAplicar")[0].innerText = "";
            $("#tg_cargos")[0].innerText = "";
            ProcesarGridGastos([]);
        }
    });

    //$('body').on('click', '.chkPasivo', null, function (e) {
    //    if ($(this).is(":checked")) {
    //    } else {
    //    }
    //});

    $('body').on('dblclick', '.rgFactura', null, function () {
        var jsonDetalle = JSON.parse($(this).find(".CDetalle").val());
        ProcesarGridGastos(jsonDetalle.DetalleContable);
        var ImporteAsignado = getImporteGastoAsignado();
        $("#facturaenproceso").val($(this).attr("data-Factura"));
        $("#uuidenproceso").val($(this).attr("data-Uuid"));
        $("#lblDetalleGasto")[0].innerText = "Factura: " + $(this).attr("data-Factura");
        $("#lblImporteFactura")[0].innerText = "Importe: " + $(this).find(".CSubtotal")[0].innerText;
        $("#lblCargado")[0].innerText = "Cargado: " + balor.aplicarFormatoMoneda(ImporteAsignado, "$");
        $("#lblPorAplicar")[0].innerText = "Por Aplicar: " + balor.aplicarFormatoMoneda((balor.quitarFormatoMoneda($(this).find(".CSubtotal")[0].innerText) - ImporteAsignado), "$");
        $("#tg_cargos")[0].innerText = balor.aplicarFormatoMoneda(ImporteAsignado);
        
        $("#ContenedorGasto").show("slow");
        $("#maxDetGasto").hide();
        $("#minDetGasto").show();
        $("#AyudaCuenta").clearHelpField();
        $("#txtConceptoPoliza, #txtImporte").val("");
        $("#txtConceptoPoliza").focus();
    });


/*
   ==================================================
   REGION DE CARGA MANUAL
   ==================================================
*/
    body.on("click", "#CargaFacturaManual", function (e) {
        AgregarFacturaManual();
    });

    var AgregarFacturaManual = function () {
        var item = getFacturaManual();

        if (item.Factura == "") {
            alert("Ingresa el numero de factura");
            $("#CajaFactura").focus()
            return;
        }

        if (item.Fechatimbre == "") {
            alert("Ingresa la fecha de la factura");
            $("#CajaFechaFactura").focus()
            return;
        }

        if (item.Subtotal == 0) {
            alert("Ingresa el subtotal de la factura");
            $("#CajaSubtotal").focus()
            return;
        }

        if (item.Total == 0) {
            alert("Ingresa el total de la factura");
            $("#CajaTotal").focus()
            return;
        }
        var existe = false;
        $("#TablaFacturas tbody tr").each(function () {
            if ($(this).attr("data-Factura").toUpperCase().trim() == item.Factura.toUpperCase().trim()) {
                existe = true;
            }
        });
        if (existe) {
            alert("Esta factura ya existe en el grild");
            $("#CajaFactura").focus()
            return;
        }
        var Datos = GetFacturasProveedor();
        Datos.push(item);
        ProcesarGridFacturas(Datos);
        $("#DivCargaManual").hide("slow");
        LimpiarCamposFacturaManual();
    };


    var getFacturaManual = function () {
        var Catfacturasproveedor = {};
        Catfacturasproveedor.Facturaproveedorid = "";
        Catfacturasproveedor.Compraid = "";
        Catfacturasproveedor.Empresaid = amplify.store.sessionStorage("EmpresaID");;
        Catfacturasproveedor.Proveedorid = $("#AyudaProveedor").getValuesByCode().ID;
        Catfacturasproveedor.Factura = $("#CajaFactura").val();
        Catfacturasproveedor.Subtotal = balor.quitarFormatoMoneda($("#CajaSubtotal").val());
        Catfacturasproveedor.Iva = balor.quitarFormatoMoneda($("#CajaIva").val());
        Catfacturasproveedor.RetIva = balor.quitarFormatoMoneda($("#CajaRetIva").val());
        Catfacturasproveedor.RetIsr = balor.quitarFormatoMoneda($("#CajaRetIsr").val());
        Catfacturasproveedor.Total = balor.quitarFormatoMoneda($("#CajaTotal").val());
        Catfacturasproveedor.Uuid = "";
        Catfacturasproveedor.Fechatimbre = $("#CajaFechaFactura").val();
        Catfacturasproveedor.Xml = "";
        Catfacturasproveedor.Rutaxml = "";
        Catfacturasproveedor.Generapasivo = false;
        Catfacturasproveedor.Dlls = false;
        Catfacturasproveedor.Detalle = [];
        Catfacturasproveedor.Pagada = false;
        Catfacturasproveedor.Estatus = 1;
        Catfacturasproveedor.UltimaAct = 0;
        return Catfacturasproveedor;
    };

    body.on("click", "#btnManual", function (e) {
        $("#DivCargaManual").show("slow");
        $("#CajaFactura").focus();
    });

    body.on("click", "#CancelarFacturaManual", function (e) {
        $("#DivCargaManual").hide("slow");
        LimpiarCamposFacturaManual();
    });


    body.on("click", "#maxFacturas", function (e) {
        $("#ContenedorFacturas").show("slow");
        $("#maxFacturas").hide();
        $("#minFacturas").show();
    });
    body.on("click", "#minFacturas", function (e) {
        $("#ContenedorFacturas").hide("slow");
        $("#maxFacturas").show();
        $("#minFacturas").hide();
    });

  

    var LimpiarCamposFacturaManual = function () {
        $("#CajaFactura,#CajaFechaFactura,#CajaSubtotal,#CajaIva,#CajaRetIva,#CajaRetIsr,#CajaTotal").val("");
    };

   

    body.on('click', '#CajaSubtotal,#CajaIva,#CajaRetIva,#CajaRetIsr,#CajaTotal,#txtImporte', function (e) {
        $(this).select();
    });

    body.on('keypress', '#CajaSubtotal,#CajaIva,#CajaRetIva,#CajaRetIsr,#CajaTotal,#txtImporte', function (e) {
        return balor.onlyNumeric(e);
    });

    $('body').on('blur', '#CajaSubtotal,#CajaIva,#CajaRetIva,#CajaRetIsr,#CajaTotal,#txtImporte', null, function (e) {
        $(this).val(balor.aplicarFormatoMoneda($(this).val(), "$"));
    });
    $('body').on('focus', '#CajaSubtotal,#CajaIva,#CajaRetIva,#CajaRetIsr,#CajaTotal,#txtImporte', null, function () {
        $(this).val(balor.quitarFormatoMoneda($(this).val())).select();
    });
    




/*
       ==================================================
       REGION DE CARGA AUTOMATICA DE FACTURAS
       ==================================================
*/
    var fileupload = document.getElementById('upload');
    function readfiles(files) {
        if (files.length <= 0)
            return;
        //Validamos que los archivos seleccionados sean XML
        for (var i = 0; i < files.length; i++) {
            if (files[i].name.toLowerCase().indexOf(".xml") <= 0) {
                alert("El archivo: " + files[i].name + " no es del tipo XML");
                return;
            }
        }
        ShowLightBox(true, "Validando Facturas en el SAT");
        var formData = new FormData();
        for (var i = 0; i < files.length; i++) {
            formData.append('file', files[i]);
        }
        formData.append('empresaid', amplify.store.sessionStorage("EmpresaID"));

        var xhr = new XMLHttpRequest();
        xhr.open('POST', 'ProcessXML_Contable.ashx');
        xhr.onload = function () {
            //AQUI SE PROCESA LA RESPUESTA DEL SERVIDOR
            if (xhr.responseText.indexOf("rror:") > 0) {
                alert(xhr.responseText);
                ShowLightBox(false, "");
                return;
            }
            var facturas = JSON.parse(xhr.responseText);
            var FacturasVigentes = [];
            var FacturasError = [];

            //Procesamos cada una de las facturas cargadas en el sistema
            for (var i = 0; i < facturas.length; i++) {
                //Validamos si la factura aun esta vigente en el SAT
                if (facturas[i].estatus != "Vigente" && facturas[i].response != "Error: unavailable service 503") {
                    facturas[i].textoxml = "FACTURA NO ENCONTRADA EN EL SAT";
                    FacturasError.push(facturas[i]);
                    continue;
                }
                //Validamos si el folio fiscal ya fue capturado con anterioridad en el sistema
                var continuar = true;
                var parametros = { uuid: facturas[i].foliofiscal };
                ejecutaAjax('CapturaCompras.aspx/ExisteUUID', 'POST', parametros, function (d) {
                    if (d.d.EsValido) {
                        if (d.d.Datos.Existe) {
                            facturas[i].textoxml = "LA FACTURA YA EXISTE EN EL SISTEMA";
                            FacturasError.push(facturas[i]);
                            continuar = false;
                        }
                    } else {
                        showMsg('Alerta', d.d.Mensaje, 'error');
                    }
                }, function (d) {
                    ShowLightBox(false, "");
                    continuar = false;
                    showMsg('Alerta', d.responseText, 'error');
                }, false);
                if (!continuar)
                    continue;

                if (ExisteUUIDGrid(facturas[i].foliofiscal)) {
                    facturas[i].textoxml = "LA FACTURA YA ESTA CARGADA EN EL GRID";
                    FacturasError.push(facturas[i]);
                    continue;
                }
                //Validamos que la factura corresponda a la empresa correspondiente segun sea el caso a balor o a factur                
                if ($.trim(facturas[i].rfcreceptor).toUpperCase() != $.trim($("#CajaEmpresaRFC").val()).toUpperCase()) {
                    facturas[i].textoxml = "LA FACTURA NO FUE EMITIDA PARA LA EMPRESA: " + $("#CajaEmpresaRFC").val();
                    FacturasError.push(facturas[i]);
                    continue;
                }
                FacturasVigentes.push(facturas[i]);
            }
            ShowLightBox(false, "");
            ProcesarFacturasVigentes(FacturasVigentes);
            if (FacturasError.length > 0) {
                ProcesarFacturasConError(FacturasError);
            }
        };
        xhr.send(formData);
    };
    fileupload.querySelector('input').onchange = function () {
        readfiles(this.files);
    };


    var ExisteUUIDGrid = function (uuid) {
        var existe = false;
        $("#TablaFacturas tbody tr").each(function () {
            if ($(this).attr("data-Uuid") == uuid) {
                existe = true;
            }
        });
        return existe;
    };

    
    var ProcesarFacturasConError = function (Datos) {
        var mensaje = "";
        for (var i = 0; i < Datos.length; i++) {
            mensaje += "Archivo: " + Datos[i].NombreArchivo + " - " + Datos[i].textoxml + "  \n";
        }
        alert(mensaje);
    };

    var ProcesarFacturasVigentes = function (Datos) {
        var Facturas = [];
        for (var i = 0; i < Datos.length; i++) {
            var Factura = {};
            Factura.Proveedorid = $("#AyudaProveedor").getValuesByCode().ID;
            Factura.File = Datos[i].NombreArchivo;
            Facturas.push(Factura);
        }

        var parametros = { value: JSON.stringify(Facturas) };
        ejecutaAjax('CapturaCompras.aspx/ProcesarXmlFacturas', 'POST', parametros, function (d) {
            var Datos = GetFacturasProveedor();
            for (var i = 0; i < d.d.Datos.length; i++) {
                Datos.push(d.d.Datos[i]);
            }
            ProcesarGridFacturas(Datos);
        }, function (d) {
            showMsg('Alerta', d.responseText, 'error');
        }, true);
    };

    body.on("click", '#upload', function (e) {
        $("input:file").val("");
    });

    var GetFacturasProveedor = function () {
        var ListaCatfacturasproveedor = [];
        $("#TablaFacturas tbody tr").each(function () {
            var jsonDetalle = JSON.parse($(this).find(".CDetalle").val());
            var Catfacturasproveedor = {};
            Catfacturasproveedor.Facturaproveedorid = $(this).attr("data-Facturaproveedorid");
            Catfacturasproveedor.Compraid = $(this).attr("data-Compraid");
            Catfacturasproveedor.Empresaid = amplify.store.sessionStorage("EmpresaID");;
            Catfacturasproveedor.Proveedorid = $(this).attr("data-Proveedorid");
            Catfacturasproveedor.Factura = $(this).attr("data-Factura");
            Catfacturasproveedor.Subtotal = balor.quitarFormatoMoneda($(this).find(".CSubtotal")[0].innerText);
            Catfacturasproveedor.Iva = balor.quitarFormatoMoneda($(this).find(".CIva")[0].innerText);
            Catfacturasproveedor.RetIva = balor.quitarFormatoMoneda($(this).find(".CRetIva")[0].innerText);
            Catfacturasproveedor.RetIsr = balor.quitarFormatoMoneda($(this).find(".CRetIsr")[0].innerText);
            Catfacturasproveedor.Total = balor.quitarFormatoMoneda($(this).find(".CTotal")[0].innerText);
            Catfacturasproveedor.Uuid = $(this).attr("data-Uuid");
            Catfacturasproveedor.Fechatimbre = $(this).attr("data-Fechatimbre");
            Catfacturasproveedor.Xml = "";
            Catfacturasproveedor.Rutaxml = $(this).attr("data-file");
            Catfacturasproveedor.Generapasivo = ($(this).find('.chkPasivo').is(":checked") ? true : false);
            Catfacturasproveedor.Dlls = ($(this).find('.chkDlls').is(":checked") ? true : false);
            Catfacturasproveedor.Detalle = jsonDetalle.DetalleContable;
            Catfacturasproveedor.Pagada = $(this).attr("data-Pagada");
            Catfacturasproveedor.Usuario = getUsuario();
            Catfacturasproveedor.Estatus = $(this).attr("data-Estatus");
            Catfacturasproveedor.UltimaAct = $(this).attr("data-UltimaAct");
            ListaCatfacturasproveedor.push(Catfacturasproveedor);
        });
        return ListaCatfacturasproveedor;
    }


    /*
    ========================================================
    REGION GRID DE GASTOS
    ========================================================    
    */

    var ProcesarGridGastos = function (Datos) {
        var resHtml = '';
        for (var i = 0; i < Datos.length; i++) {
            resHtml += '<tr data-Facturaproveedordetid="' + Datos[i].Facturaproveedordetid + '" data-UltimaAct="' + Datos[i].UltimaAct + '" >';
            resHtml += '<td class="CCuenta">' + formatoCuenta(Datos[i].Cuenta) + '</td>';
            resHtml += '<td class="CConcepto">' + Datos[i].Concepto + '</td>';
            resHtml += '<td class="CImporte">' + balor.aplicarFormatoMoneda(Datos[i].Importe, "$") + '</td>';
            resHtml += '<td class="tdeliminar centrado"><span class="closebtn elim-Gasto" >X</span></td>';
            resHtml += '</tr>';
        }
        $("#tablaDetalle tbody").html(resHtml);
    };

    var getImporteGastoAsignado = function () {
        var Importe = 0;
        $("#tablaDetalle tbody tr").each(function () {

            Importe += redondear(balor.quitarFormatoMoneda($(this).find(".CImporte")[0].innerText), 10);
        });
        return Importe;
    };


    body.on("click", "#maxDetGasto", function (e) {
        $("#ContenedorGasto").show("slow");
        $("#maxDetGasto").hide();
        $("#minDetGasto").show();
    });

    body.on("click", "#minDetGasto", function (e) {
        $("#ContenedorGasto").hide("slow");
        $("#maxDetGasto").show();
        $("#minDetGasto").hide();
    });


    body.on("click", "#btnAgregar", function (e) {
        if (!ValidarAgregarGasto())
            return;
        AgregarNuevoGasto();        
        $("#txtImporte").val("");
        $("#AyudaCuenta").clearHelpField().setFocusHelp();
    });

    var ValidarAgregarGasto = function () {
        if ($("#facturaenproceso").val() == "") {
            alert("Primero seleccione una factura para poder utilizar esta opcion");
            return false;
        }
        if ($("#AyudaCuenta").getValuesByCode().ID == "") {
            alert("Por favor especifique un numero de cuenta contable valido");
            $("#AyudaCuenta").setFocusHelp();
            return false;
        }
        if ($("#txtConceptoPoliza").val() == "") {
            alert("Por favor especifique un concepto para el detalle del gasto");
            $("#txtConceptoPoliza").focus();
            return false;
        }
        if ($("#txtImporte").val() == "" || balor.quitarFormatoMoneda($("#txtImporte").val()) == 0) {
            alert("Por favor especifique un importe");
            $("#txtImporte").focus();
            return false;
        }

        var ImporteGastos = getImporteGastoAsignado();
        ImporteGastos += redondear(parseFloat($("#txtImporte").val().replace(/,/g, '').replace("$", "")), 10);
        var ImporteFactura = parseFloat($("#lblImporteFactura")[0].innerText.replace("Importe: ", "").replace(/,/g, '').replace("$", ""))
        if (ImporteGastos > ImporteFactura)
        {
            alert("Importe especificado es mayor que el importe de la factura");
            $("#txtImporte").focus();
            return false;
        }
        return true;
    };

    $('body').on('click', '.elim-Gasto', null, function () {
        if (confirm('¿Desea eliminar este movimiento?')) {
            $(this).parents('tr').remove();
            ActualizaEliminados();
        }
    });

    var ActualizaEliminados = function () {
        UpdateGastosFactura($("#facturaenproceso").val(), $("#uuidenproceso").val());
        var ImporteAsignado = getImporteGastoAsignado();
        $("#tg_cargos")[0].innerText = balor.aplicarFormatoMoneda(redondear(parseFloat(ImporteAsignado), 10));
        $("#lblCargado")[0].innerText = "Cargado: " + balor.aplicarFormatoMoneda(redondear(parseFloat(ImporteAsignado), 10), "$");
        var impfactura = parseFloat($("#lblImporteFactura")[0].innerText.replace("Importe: ", "").replace(/,/g, '').replace("$", ""));
        $("#lblPorAplicar")[0].innerText = "Por Aplicar: " + balor.aplicarFormatoMoneda(redondear((impfactura - (redondear(parseFloat(ImporteAsignado), 10))), 10), "$");
    };

    var AgregarNuevoGasto = function () {
        var Gastos = getGastos($("#facturaenproceso").val(), $("#uuidenproceso").val());
        var item = GetItemGasto();
        Gastos.push(item);
        ProcesarGridGastos(Gastos);
        UpdateGastosFactura($("#facturaenproceso").val(), $("#uuidenproceso").val());
        var ImporteAsignado = getImporteGastoAsignado();
        $("#tg_cargos")[0].innerText = balor.aplicarFormatoMoneda(redondear(parseFloat(ImporteAsignado), 10));
        $("#lblCargado")[0].innerText = "Cargado: " + balor.aplicarFormatoMoneda(redondear(parseFloat(ImporteAsignado), 10), "$");
        var impfactura = parseFloat($("#lblImporteFactura")[0].innerText.replace("Importe: ", "").replace(/,/g, '').replace("$", ""));
        $("#lblPorAplicar")[0].innerText = "Por Aplicar: " + balor.aplicarFormatoMoneda(redondear((impfactura - (redondear(parseFloat(ImporteAsignado), 10))), 10), "$");
    };

    var getGastos = function (Factura, Uuid) {
        var ListaGastos = [];
        $("#TablaFacturas tbody tr").each(function () {
            if ($(this).attr("data-Factura").toUpperCase().trim() == Factura.toUpperCase().trim() && $(this).attr("data-Uuid").toUpperCase().trim() == Uuid.toUpperCase().trim()) {
                var jsonDetalle = JSON.parse($(this).find(".CDetalle").val());
                ListaGastos = jsonDetalle.DetalleContable;
            }
        });
        return ListaGastos;
    };

    var UpdateGastosFactura = function (Factura, Uuid) {
        var ListaGastos = [];
        $("#tablaDetalle tbody tr").each(function () {
            var Codigo = llenarSubcuentas($(this).find(".CCuenta")[0].innerText).replace(/-/g, '');
            Codigo = padRight(Codigo, 24);
            var item = {};
            item.Facturaproveedordetid = $(this).attr("data-Facturaproveedordetid");
            item.Cuenta = Codigo;
            item.Concepto = $(this).find(".CConcepto")[0].innerText;
            item.Importe = balor.quitarFormatoMoneda($(this).find(".CImporte")[0].innerText);
            item.Estatus = 1;
            item.Usuario = getUsuario();
            item.UltimaAct = $(this).attr("data-UltimaAct");
            ListaGastos.push(item);
        });
        var jsonDetalle = { DetalleContable: ListaGastos };
        $("#TablaFacturas tbody tr").each(function () {
            if ($(this).attr("data-Factura").toUpperCase().trim() == Factura.toUpperCase().trim() && $(this).attr("data-Uuid").toUpperCase().trim() == Uuid.toUpperCase().trim()) {
                $(this).find(".CDetalle").val(JSON.stringify(jsonDetalle));
                var ImporteAsignado = 0;
                for (var j = 0; j < ListaGastos.length; j++)
                    ImporteAsignado += ListaGastos[j].Importe;
                var ImporteFact = balor.quitarFormatoMoneda($(this).find(".CSubtotal")[0].innerText);
                if (ImporteAsignado == 0)
                    $(this).css("background-color", "");
                else {
                    if (redondear(ImporteAsignado, 10) == redondear(ImporteFact, 10))
                        $(this).css("background-color", "lightgreen");
                    else
                        $(this).css("background-color", "blanchedalmond");
                }
            }
        });
    };

    var GetItemGasto = function () {
        var Codigo = llenarSubcuentas($('#AyudaCuenta').getValuesByCode().Codigo).replace(/-/g, '');
        Codigo = padRight(Codigo, 24);
        var item = {};
        item.Facturaproveedordetid = "";
        item.Cuenta = Codigo;
        item.Concepto = $("#txtConceptoPoliza").val();
        item.Importe = balor.quitarFormatoMoneda($("#txtImporte").val());
        item.Estatus = 1;
        item.Usuario = getUsuario();
        item.UltimaAct = 0;
        return item;
    };

   

    /*
    ========================================================
    REGION CUENTA CONTABLE Y FUNCIONES
    ========================================================
    */
    var formatMoney = function (numero, c, d, t) {
        var n = numero,
    s = n < 0 ? '-' : '',
    i = parseInt(n = Math.abs(+n || 0).toFixed(c)) + '',
    j = (j = i.length) > 3 ? j % 3 : 0;

        c = isNaN(c = Math.abs(c)) ? 2 : c;
        d = d ? d : '.';
        t = t ? t : ',';
        s = n == 0 ? '' : s;
        return s + (j ? i.substr(0, j) + t : "") + i.substr(j).replace(/(\d{3})(?=\d)/g, "$1" + t) + (c ? d + Math.abs(n - i).toFixed(c).slice(2) : '');
    };

    // event.type must be keypress
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

    $('body').on('keypress', '#AyudaCuenta_Code', null, function (e) {
        var input = $('#AyudaCuenta_Code'),
      strCuenta = input.val(),
      key = getChar(e);
        if (strCuenta.length < 29) {
            if (key) {
                if (/[0-9]|\-/.test(key)) {
                    if (key === '-') {
                        var cuentaNueva = llenarSubcuentas(strCuenta);
                        input.val(cuentaNueva);
                        if (cuentaNueva.length === 29) {
                            return false;
                        }
                    } else {
                        strCuenta += key;
                        var cuenta2 = formatoCuenta(strCuenta);
                        input.val(cuenta2);
                        return false;
                    }
                } else {
                    return false;
                }
            }
        } else {
            return false;
        }
    });
    /*
    =====================================================
    REGION PARA GUARDAR LA COMPRA
    =====================================================
    */
    $("#btnGuardar").click(function () {
        if (confirm('¿Desea guardar estas facturas en el sistema?')) {
            if (ValidaGuardarCompra())
                GuardarComprasProveedor();
        }
    });

    var ValidaGuardarCompra = function () {
        var Datos = GetFacturasProveedor();
        for (var i = 0; i < Datos.length; i++) {
            //Validamos cada una de las facturas agregadas al detalle
            var Importe = 0;
            for (var j = 0; j < Datos[i].Detalle.length; j++) {
                Importe += redondear(Datos[i].Detalle[j].Importe, 10);
            }
            if (redondear(Importe, 10) < redondear(Datos[i].Subtotal, 10)) {
                alert("Debe asignar todas las facturas a un gasto");
                return false;
            }
        }

        if ($("#txtFechaPol").val() == "") {
            alert("Por favor ingrese la fecha");
            $("#txtFechaPol").focus();
            return false;
        }

        if (GeneraPasivo()) {
            var TieneCuenta = false;
            var parametros = { proveedorid: $("#AyudaProveedor").getValuesByCode().ID };
            ejecutaAjax('CapturaCompras.aspx/TieneCuentaContable', 'POST', parametros, function (d) {
                if (d.d.EsValido) {
                    if (d.d.Datos.Existe)
                        TieneCuenta = true;
                } else {
                    showMsg('Alerta', d.d.Mensaje, 'error');
                }
            }, function (d) {
                ShowLightBox(false, "");
                showMsg('Alerta', d.responseText, 'error');
            }, false);

            if (!TieneCuenta) {
                alert("Este proveedor no tiene relacionada una cuenta contable en el sistema, no puedes generarle pasivo");
                return false;
            }
        }
        return true;
    }

    var GeneraPasivo = function () {
        var pasivo = false;
        $("#TablaFacturas tbody tr").each(function () {
            if ($(this).find('.chkPasivo').is(":checked"))
                pasivo = true;
        });
        return pasivo;
    };


    var GuardarComprasProveedor = function () {
        var ListaCatfacturasproveedor = GetFacturasProveedor();
        var parametros = { value: JSON.stringify(ListaCatfacturasproveedor), fecha: $("#txtFechaPol").val() };
        ShowLightBox(true, "Guardando, espere porfavor");
        ejecutaAjax('CapturaCompras.aspx/GuardarComprasProveedor', 'POST', parametros, function (d) {
            ShowLightBox(false, "");
            if (d.d.EsValido) {
                if (d.d.Datos.Poliza.UltimaAct != 0) {
                    $("#btnImprimir").removeAttr("disabled");
                    $("#PolizaID").val(d.d.Datos.Poliza.Polizaid);
                    imprimirPoliza(d.d.Datos.Poliza.Polizaid);
                }
                $("#AyudaCompra_Code").val(d.d.Datos.Compra.Folio);
                $("#AyudaCompra_ValorID").val(d.d.Datos.Compra.Compraid);
                ProcesarGridFacturas(d.d.Datos.ListaCatfacturasproveedor);
                $("#btnGuardar").attr("disabled", "disabled");
                $("#btnCancelar").removeAttr("disabled");
                $('#TablaFacturas th:eq(9)').hide();
                $('#TablaFacturas td:nth-child(10)').hide();
            } else {
                showMsg('Alerta', d.d.Mensaje, 'error');
            }
        }, function (d) {
            ShowLightBox(false, "");
            showMsg('Alerta', d.responseText, 'error');
        }, true);
    };
    var CancelarCompra = function () {
        if (!ValidaCancelacion()) {
            alert("Esta compra no se puede cancelar por que contiene facturas pagadas, avisarle a sistemas");
            return;
        }

        var parametros = {};
        parametros.CompraID = $('#AyudaCompra').getValuesByCode().ID;
        parametros.Usuario = getUsuario();
        ShowLightBox(true, "Espere porfavor");
        ejecutaAjax('CapturaCompras.aspx/CancelaCompra', 'POST', parametros, function (d) {
            ShowLightBox(false, "");
            if (d.d.EsValido) {
                alert("Compra Cancelada");
                window.location.replace("CapturaCompras.aspx");
            } else {
                showMsg('Alerta', d.d.Mensaje, 'warning');
            }
        }, function (d) {
            ShowLightBox(false, "");
            showMsg('Alerta', d.responseText, 'error');
        }, true);
    };

    var ValidaCancelacion = function () {
        var cancela = true;
        $("#TablaFacturas tbody tr").each(function () {
            if ($(this).attr("data-Pagada") == "true") {
                cancela = false;
            }
        });
        return cancela;
    };

    $('body').on('click', '#btnCancelar', null, function () {
        if (confirm('¿Desea eliminar esta compra?')) {
            CancelarCompra();
        }
    });


    $('body').on('click', '#btnImprimir', null, function () {
        imprimirPoliza($("#PolizaID").val());
    });

    

    $("#btnLimpiar").click(function () {
        window.location.replace("CapturaCompras.aspx");
    });

    var imprimirPoliza = function (polizaid) {
        var parametros = '&polizaid=' + polizaid;
        parametros += "&Ingles=0";
        window.open('../../Base/Formas/ReporteProduccion.aspx?Formato=0&Nombre=ReportePoliza' + parametros, 'w_PopImprimir', 'toolbar=no,location=no,directories=no,status=no,menubar=no,scrollbars=yes,resizable=yes,width=760,height=560');
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
        $(".svGrid").css({
            'margin': '0 0 0 0'
            
        });
    };

    var ontenerRFCPorEmpresa = function () {
        var parametros = { empresaid: amplify.store.sessionStorage("EmpresaID") };
        ejecutaAjax('CapturaCompras.aspx/ontenerRFCPorEmpresa', 'POST', parametros, function (d) {
            if (d.d.EsValido) {
                if (d.d.Datos.Existe) {
                    $("#CajaEmpresaRFC").val(d.d.Datos.RFC);
                }
            } else {
                showMsg('Alerta', d.d.Mensaje, 'error');
            }
        }, function (d) {
            ShowLightBox(false, "");
            showMsg('Alerta', d.responseText, 'error');
        }, true);
    };

    $(document).ready(function () {
        if (amplify.store.sessionStorage("UsuarioID") == null || amplify.store.sessionStorage("UsuarioID") == "") {
            window.location.replace("../../login.aspx");
        }
        $("#Menu").MostrarMenuBar(JSON.parse(amplify.store.sessionStorage("Funcionalidades")), amplify.store.sessionStorage("Nombre"), amplify.store.sessionStorage("NombreEmpresa"), amplify.store.sessionStorage("UsuarioID"));
        $('.popup').setDraggable('.popup-body, .popup-footer, .close');
        //ActualizaMenuBar();
        AplicarEstilo();
        iniciaAyudas();
        ontenerRFCPorEmpresa();
        $("#DivCargaManual").hide();
        $("#ContenedorGasto,#ContenedorFacturas").hide();
        $('#AyudaCompra').setFocusHelp();
    });
}(jQuery, window.balor, window.amplify));