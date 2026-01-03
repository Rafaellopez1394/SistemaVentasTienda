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
        //alert(Mostrar);
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

    var muestraPopup = function (popid, principal) {
        principal = (principal == null ? true : principal);
        if (principal)
            lightboxOn();
        var popup = $('#' + popid),
        width = popup.actual('width'),
        left = (window.innerWidth / 2) - (width / 2),
        //top = window.innerHeight * 0.05;
        top = window.scrollY + 10;
        popup.css({ 'position': 'absolute', 'top': top, 'left': left });
        popup.addClass('fadeInDown').removeClass('hidden');
        $('#documento-body').animate({
            scrollTop: 0
        }, 'fast');

    };

    var cerrarPopup = function (popid, principal) {
        $('#' + popid).addClass('hidden').removeClass('fadeInDown');
        principal = (principal == null ? true : principal);
        if (principal)
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

    var ActualizaMenuBar = function () {
        if (screen.width < 600) {
            $("#MobileMenu").css("display", "line");
            var menu = $("#Menu").html();
            $("#divcontentMenu").remove();
            $("#MobileMenuDet").html(menu);
            $("#nav").css("top", "0");
        }
    }

    /*
       ==================================================
       REGION DE CARGA ARCHIVO CSV
       ==================================================
    */

    body.on("click", '#upload', function (e) {
        $("input:file").val("");
    });
    
    var fileupload = document.getElementById('upload');
    function readfiles(files) {
        //alert(files.length);
        if (files.length <= 0)
            return;
        //Validamos que los archivos seleccionados sean XML
        for (var i = 0; i < files.length; i++) {
            if (files[i].name.toLowerCase().indexOf(".csv") <= 0) {
                alert("El archivo: " + files[i].name + " no es del tipo CSV");
                return;
            }
        }
        ShowLightBox(true, "Procesando archivo");
        var formData = new FormData();
        for (var i = 0; i < files.length; i++) {
            formData.append('file', files[i]);
        }
    
        //alert('inicio');
        //ShowLightBox(false, "");
        //return;

        var xhr = new XMLHttpRequest();
        //var params = 'upload=start&usuario=' + getUsuario();
        xhr.open('POST', 'CargaArchivoListaNegraSat.ashx?upload=start&usuario=' + getUsuario() + '&sesionid=' + $("#hSesionId").val());
        //xhr.send(params);
        xhr.onload = function () {
            //AQUI SE PROCESA LA RESPUESTA DEL SERVIDOR
            if (xhr.responseText.indexOf("rror:") > 0) {
                alert(xhr.responseText);
                ShowLightBox(false, "");
                return;
            }
            
            var mensaje = xhr.responseText;
           

            if (xhr.responseText == 'Success') {
                showMsg('Datos importados', 'El archivo fue procesado correctamente.', 'success');
                TraerGridContribuyentes();
            }
            else {
                alert(xhr.responseText);
            }
                        
            ShowLightBox(false, "");
                        
        };
        xhr.send(formData);
    };
    fileupload.querySelector('input').onchange = function () {
        readfiles(this.files);
    };



    // GRID

    $('body').on('click', 'input[name=rbSoloBalor]:radio', null, function () {
            
        TraerGridContribuyentes();
    });

    var TraerGridContribuyentes = function () {
        var parametros = {};
      

        var soloBalor = true;
        var rbSoloBalor = $('input[name=rbSoloBalor]:radio:checked').val();
        
        if (rbSoloBalor == 0)
        {
            soloBalor = false;
        }

        //alert(rbSoloBalor);

        ShowLightBox(true, "Cargando datos");
        parametros.solobalor = soloBalor;
        parametros.nombre = $("#txtnombre").val();
        parametros.rfc = $("#txtrfc").val();

        ejecutaAjax('ListaNegraSat.aspx/ConsultarContribuyentes', 'POST', parametros, function (d) {
            if (d.d.EsValido) {
                ProcesarGridContribuyentes(d.d.Datos);
                ShowLightBox(false, "");
            } else {
                showMsg('Alerta', d.d.Mensaje, 'error');
                ShowLightBox(false, "");
            }
        }, function (d) {
            showMsg('Alerta', d.responseText, 'error');
            ShowLightBox(false, "");
        }, true);

       
    };

    var ProcesarGridContribuyentes = function (Datos) {
        var resHtml = '';
        var odd = "odd";
        var entra = false;
        for (var i = 0; i < Datos.length; i++) {
            resHtml += '<tr class="' + (entra ? odd : "") + '" data-id="' + Datos[i].Numero + '" data-Proveedorid="' + Datos[i].ProveedorID + '" >';

            resHtml += '<td style="font-size:small;">' + Datos[i].Numero + '</td>';
            resHtml += '<td style="font-size:small;">' + Datos[i].Rfc + '</td>';
            resHtml += '<td style="font-size:small;">' + Datos[i].Nombre + '</td>';
            resHtml += '<td style="font-size:small;">' + Datos[i].Situacion + '</td>';
            
            resHtml += '<td style="font-size:small;"><a class="svbtn show-process"><i class="svicon"></i> Ver detalle</a></td>';
            if (Datos[i].ProveedorID.length > 0)
            {
                if (Datos[i].Facturas > 0) {
                    resHtml += '<td style="font-size:small;"><a class="svbtn show-polizas"><i class="svicon"></i>Ver polizas/facturas</a></td>';
                }
                else
                {
                    resHtml += '<td style="font-size:small;">Sin polizas</td>';
                }
               
            }
            else
            {
                resHtml += '<td style="font-size:small;"></td>';
            }
           
           
            resHtml += '</tr>';
            entra = (!entra ? true : false);
        }
        $("#TablaContribuyentes tbody").html(resHtml);
      
    };

    $('body').on('click', '#btnConsultar', null, function () {
        TraerGridContribuyentes();
    });

    $('body').on('click', '#btnReset', null, function () {
        $("#txtnombre").val("");
        $("#txtrfc").val("");
        $("input[name=rbSoloBalor][value='1']").prop("checked", true);


        TraerGridContribuyentes();
    });

    // POP

    $('body').on('click', '#polizas-close', null, function () {

        cerrarPopup('polizas');

    });

    $('body').on('click', '.show-polizas', null, function () {

        var parametros = {};
       
        var proveedorid = $(this.parentNode.parentNode).attr("data-proveedorid");

        parametros.proveedorid = proveedorid;
        //traer datos de polizas
        
        ejecutaAjax('ListaNegraSat.aspx/ConsultarPolizasProveedor', 'POST', parametros, function (d) {
            if (d.d.EsValido) {
                ProcesarGridDetallePolizas(d.d.Datos);
                ShowLightBox(false, "");
            } else {
                showMsg('Alerta', d.d.Mensaje, 'error');
                ShowLightBox(false, "");
            }
        }, function (d) {
            showMsg('Alerta', d.responseText, 'error');
            ShowLightBox(false, "");
        }, true);


        muestraPopup('polizas');
    });

    var ProcesarGridDetallePolizas = function (Datos) {
        var resHtml = '';
        var odd = "odd";
        var entra = false;
        for (var i = 0; i < Datos.length; i++) {
            resHtml += '<tr class="' + (entra ? odd : "") + '" data-id="' + Datos[i].num_pol + '" data-Proveedorid="' + Datos[i].ProveedorID + '" >';

            resHtml += '<td style="font-size:small;">' + Datos[i].num_pol + '</td>';
            resHtml += '<td style="font-size:small;">' + Datos[i].Tip_pol + '</td>';
            resHtml += '<td style="font-size:small;">' + Datos[i].Fec_Pol + '</td>';
            resHtml += '<td style="font-size:small;">' + Datos[i].ImportePoliza + '</td>';
            resHtml += '<td style="font-size:small;">' + Datos[i].Serie + ' ' + Datos[i].Factura + '</td>';
            resHtml += '<td style="font-size:small;">' + Datos[i].EmisorRFC + '<br/>' + Datos[i].EmisorNombre + '</td>';
            resHtml += '<td style="font-size:small;">' + Datos[i].ReceptorRFC + '<br/>' + Datos[i].ReceptorNombre + '</td>';
            resHtml += '<td style="font-size:small;">' + Datos[i].Concepto + '</td>';
            
            resHtml += '<td style="font-size:small;">' + Datos[i].FechaFactura + '</td>';
            
            resHtml += '</tr>';
            entra = (!entra ? true : false);
        }
        $("#TablaPolizas tbody").html(resHtml);

    };

    $('body').on('click', '#contribuyente-close', null, function () {
       
        cerrarPopup('contribuyente');
               
    });

    $('body').on('click', '.show-process', null, function () {
        
        var parametros = {};
        var numero = $(this.parentNode.parentNode).attr("data-id");
        var proveedorid = $(this.parentNode.parentNode).attr("data-proveedorid");

        parametros.numero = numero;
        //traer datos de lista negra

      
        ejecutaAjax('ListaNegraSat.aspx/ConsultarDetalle', 'POST', parametros, function (d) {
            if (d.d.EsValido) {
                ProcesarGridDetalle(d.d.Datos);
                ShowLightBox(false, "");
            } else {
                showMsg('Alerta', d.d.Mensaje, 'error');
                ShowLightBox(false, "");
            }
        }, function (d) {
            showMsg('Alerta', d.responseText, 'error');
            ShowLightBox(false, "");
        }, true);
       
       
        muestraPopup('contribuyente');
    });

    var ProcesarGridDetalle = function (Datos) {
        var resHtml = '';
        var odd = "odd";
        var entra = false;
        for (var i = 0; i < Datos.length; i++) {
            resHtml += '<tr class="' + (entra ? odd : "") + '" >';
            //Numero
            resHtml += '<td style="font-size:small;font-weight: bolder;">Número</td>';
            resHtml += '<td style="font-size:small;">' + Datos[i].Numero + '</td>';
            resHtml += '</tr>';
            entra = !entra;
            //RFC
            resHtml += '<tr class="' + (entra ? odd : "") + '" >';
            resHtml += '<td style="font-size:small;font-weight: bolder;">RFC</td>';
            resHtml += '<td style="font-size:small;">' + Datos[i].RFC + '</td>';
            resHtml += '</tr>';
            entra = !entra;
                   
            //Nombre
            resHtml += '<tr class="' + (entra ? odd : "") + '" >';
            resHtml += '<td style="font-size:small;font-weight: bolder;">Nombre del contribuyente</td>';
            resHtml += '<td style="font-size:small;">' + Datos[i].Nombre + '</td>';
            resHtml += '</tr>';
            entra = !entra;
            //Situacion
            resHtml += '<tr class="' + (entra ? odd : "") + '" >';
            resHtml += '<td style="font-size:small;font-weight: bolder;">Situación del contribuyente</td>';
            resHtml += '<td style="font-size:small;">' + Datos[i].Situacion + '</td>';
            resHtml += '</tr>';
            entra = !entra;
            //NumeroFechaPresuncionSat
            resHtml += '<tr class="' + (entra ? odd : "") + '" >';
            resHtml += '<td style="font-size:small;font-weight: bolder;">Número y fecha de oficio global<br/>de presunción</td>';
            resHtml += '<td style="font-size:small;">' + Datos[i].NumeroFechaPresuncionSat + '</td>';
            resHtml += '</tr>';
            entra = !entra;
            //PublicacionSatPresuntos
            resHtml += '<tr class="' + (entra ? odd : "") + '" >';
            resHtml += '<td style="font-size:small;font-weight: bolder;">Publicación página SAT presuntos</td>';
            resHtml += '<td style="font-size:small;">' + Datos[i].PublicacionSatPresuntos + '</td>';
            resHtml += '</tr>';
            entra = !entra;
            //NumeroFechaPresuncionDof
            resHtml += '<tr class="' + (entra ? odd : "") + '" >';
            resHtml += '<td style="font-size:small;font-weight: bolder;">Número y fecha de oficio global<br/>de presunción</td>';
            resHtml += '<td style="font-size:small;">' + Datos[i].NumeroFechaPresuncionDof + '</td>';
            resHtml += '</tr>';
            entra = !entra;
            //PublicacionDofPresuntos
            resHtml += '<tr class="' + (entra ? odd : "") + '" >';
            resHtml += '<td style="font-size:small;font-weight: bolder;">Publicación DOF presuntos</td>';
            resHtml += '<td style="font-size:small;">' + Datos[i].PublicacionDofPresuntos + '</td>';
            resHtml += '</tr>';
            entra = !entra;
		
            //NumeroFechaDesvirtuaronSat
            resHtml += '<tr class="' + (entra ? odd : "") + '" >';
            resHtml += '<td style="font-size:small;font-weight: bolder;">Número y fecha de oficio global<br/>de contribuyentes que desvirtuaron SAT</td>';
            resHtml += '<td style="font-size:small;">' + Datos[i].NumeroFechaDesvirtuaronSat + '</td>';
            resHtml += '</tr>';
            entra = !entra;
            //PublicacionSatDesvirtuados
            resHtml += '<tr class="' + (entra ? odd : "") + '" >';
            resHtml += '<td style="font-size:small;font-weight: bolder;">Publicación página SAT desvirtuados</td>';
            resHtml += '<td style="font-size:small;">' + Datos[i].PublicacionSatDesvirtuados + '</td>';
            resHtml += '</tr>';
            entra = !entra;
            //NumeroFechaDesvirtuaronDof
            resHtml += '<tr class="' + (entra ? odd : "") + '" >';
            resHtml += '<td style="font-size:small;font-weight: bolder;">Número y fecha de oficio global<br/>de contribuyentes que desvirtuaron DOF</td>';
            resHtml += '<td style="font-size:small;">' + Datos[i].NumeroFechaDesvirtuaronDof + '</td>';
            resHtml += '</tr>';
            entra = !entra;
            //PublicacionDofDesvirtuados
            resHtml += '<tr class="' + (entra ? odd : "") + '" >';
            resHtml += '<td style="font-size:small;font-weight: bolder;">Publicación DOF desvirtuados</td>';
            resHtml += '<td style="font-size:small;">' + Datos[i].PublicacionDofDesvirtuados + '</td>';
            resHtml += '</tr>';
            entra = !entra;

            //NumeroFechaDefinitivosSat
            resHtml += '<tr class="' + (entra ? odd : "") + '" >';
            resHtml += '<td style="font-size:small;font-weight: bolder;">Número y fecha de oficio global<br/>de definitivos	SAT</td>';
            resHtml += '<td style="font-size:small;">' + Datos[i].NumeroFechaDefinitivosSat + '</td>';
            resHtml += '</tr>';
            entra = !entra;
            //PublicacionSatDefinitivos
            resHtml += '<tr class="' + (entra ? odd : "") + '" >';
            resHtml += '<td style="font-size:small;font-weight: bolder;">Publicación página SAT definitivos</td>';
            resHtml += '<td style="font-size:small;">' + Datos[i].PublicacionSatDefinitivos + '</td>';
            resHtml += '</tr>';
            entra = !entra;
            //NumeroFechaDefinitivosDof
            resHtml += '<tr class="' + (entra ? odd : "") + '" >';
            resHtml += '<td style="font-size:small;font-weight: bolder;">Número y fecha de oficio global<br/>de definitivos DOF</td>';
            resHtml += '<td style="font-size:small;">' + Datos[i].NumeroFechaDefinitivosDof + '</td>';
            resHtml += '</tr>';
            entra = !entra;
            //PublicacionDofDefinitivos
            resHtml += '<tr class="' + (entra ? odd : "") + '" >';
            resHtml += '<td style="font-size:small;font-weight: bolder;">Publicación DOF definitivos</td>';
            resHtml += '<td style="font-size:small;">' + Datos[i].PublicacionDofDefinitivos + '</td>';
            resHtml += '</tr>';
            entra = !entra;

            //NumeroFechaSentenciaFavorableSat,
            resHtml += '<tr class="' + (entra ? odd : "") + '" >';
            resHtml += '<td style="font-size:small;font-weight: bolder;">Número y fecha de oficio global<br/>de sentencia favorable</td>';
            resHtml += '<td style="font-size:small;">' + Datos[i].NumeroFechaSentenciaFavorableSat + '</td>';
            resHtml += '</tr>';
            entra = !entra;
            //PublicacionSatSentenciaFavorable
            resHtml += '<tr class="' + (entra ? odd : "") + '" >';
            resHtml += '<td style="font-size:small;font-weight: bolder;">Publicación página SAT sentencia<br/>favorable</td>';
            resHtml += '<td style="font-size:small;">' + Datos[i].PublicacionSatSentenciaFavorable + '</td>';
            resHtml += '</tr>';
            entra = !entra;
            //NumeroFechaSentenciaFavorableDof
            resHtml += '<tr class="' + (entra ? odd : "") + '" >';
            resHtml += '<td style="font-size:small;font-weight: bolder;">Número y fecha de oficio global<br/>de sentencia favorable</td>';
            resHtml += '<td style="font-size:small;">' + Datos[i].NumeroFechaSentenciaFavorableDof + '</td>';
            resHtml += '</tr>';
            entra = !entra;
            //PublicacionDofSentenciaFavorable
            resHtml += '<tr class="' + (entra ? odd : "") + '" >';
            resHtml += '<td style="font-size:small;font-weight: bolder;">Publicación DOF sentencia favorable</td>';
            resHtml += '<td style="font-size:small;">' + Datos[i].PublicacionDofSentenciaFavorable + '</td>';
            resHtml += '</tr>';
            entra = !entra;
           
           


            
            entra = (!entra ? true : false);
        }
        $("#TablaContribuyente tbody").html(resHtml);

    };



    var traerSesionId = function () {

        var parametros = {};
        ejecutaAjax('ListaNegraSat.aspx/TraerSesionId', 'POST', parametros, function (d) {
            if (d.d.EsValido) {
                var datos = d.d.Datos;

                var SesionId = datos.SesionId;

                //hidden 
                $('#hSesionId').val(SesionId);

            } else {
                //showMsg('Alerta', d.d.Mensaje, 'error');
            }
        }, function (d) {
            showMsg('Alerta', d.responseText, 'error');
        }, false);

    };

    $(document).ready(function () {
        if (amplify.store.sessionStorage("UsuarioID") == null || amplify.store.sessionStorage("UsuarioID") == "") {
            window.location.replace("../../login.aspx");
        }
        $("#Menu").MostrarMenuBar(JSON.parse(amplify.store.sessionStorage("Funcionalidades")), amplify.store.sessionStorage("Nombre"), amplify.store.sessionStorage("NombreEmpresa"), amplify.store.sessionStorage("UsuarioID"));
        $('.popup').setDraggable('.popup-body, .popup-footer, .close');
        //llenacombo('ddlvendedor', {}, 'TraerVendedores', 'Todos');
        ActualizaMenuBar();

        //session
        traerSesionId();

        //gird

        TraerGridContribuyentes();
        //TraerGridDocumentos(5); //prospectos en tramite -primer radio button
        //ActualizaEstilosArbol();
        //iniciaAyudas();

        //$("#aCliente").css("display", "none");

    });
}(jQuery, window.balor, window.amplify));