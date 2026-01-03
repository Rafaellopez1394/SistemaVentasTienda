

(function ($, balor, amplify) {
    var msgbx = $('#divmsg');
    var body = $('body');
    var clock = null;

    var showMsg = function (titulo, mensaje, tipoMensaje) {
        msgbx.mostrarMensaje(titulo, mensaje, tipoMensaje);
    };

    var escondeMsg = function () {
        msgbx.animate({ top: -msgbx.outerHeight(), opacity: 0 }, 500);
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

    $('body').on('click', '#BtnGenerarArchivoPagos', function () {
        if ($("#fechaInicio").val() == "" && $("#fechaFin").val() == "") {
            showMsg("Campo requerido", "Favor de seleccionar una fecha", "warning");
            return;
        }
        else {
            saveArchivoPago();
        }
    });



    $('body').on('click', '#btnGenerarArchivoCredito', null, function () {
        if ($("#fechaInicio").val() == "" || $("#fechaFin").val() == "") {
            showMsg("Campo requerido", "Favor de seleccionar una fecha", "warning");
            return;
        }
        else {
            saveArchivoCredito();
        }

    });


    var saveArchivoCredito = function () 
    {
        
            ejecutaAjax('GeneracionPLD.aspx/TraerDatosCuenta', 'POST', { fechaInicio: $('#fechaInicio').val(), fechaFin: $('#fechaFin').val() },
            function (data) {
                if (data.d.EsValido) {
                    let fileName = data.d.Datos.NombreArchivo;
                    let base64String = data.d.Datos.ContenidoBase64;

                    showMsg('Alerta', 'Archivo generado correctamente', 'success');
                    $('#divDescarga').css('display', 'inline');
                    $('#btnDescargarExcel').attr('href', 'data:text/csv;base64,' + data.d.Datos.NombreArchivo);
                    let csvContent = 'data:text/csv;base64,' + data.d.Datos.ContenidoBase64;
                    let link = document.createElement('a');
                    link.href = csvContent;
                    link.download = data.d.Datos.NombreArchivo; 
                    document.body.appendChild(link);
                    link.click();
                    document.body.removeChild(link);

                } else {
                    showMsg("ocurrio un error ", data.d.Mensaje, "Error");
                }
            },
            function (d) {
                ShowLightBox(false, "");
                showMsg("Error", d.responseText, "Error");
            }, true);
        
    };

   

    var saveArchivoPago = function () {

        ejecutaAjax('GeneracionPLD.aspx/TraerDatosPagos', 'POST', { fechaInicio: $('#fechaInicio').val(), fechaFin: $('#fechaFin').val() },
             function (data) {
                 if (data.d.EsValido) {
                     let fileName = data.d.Datos.NombreArchivo;
                     let base64String = data.d.Datos.ContenidoBase64;

                     showMsg('Alerta', 'Archivo generado correctamente', 'success');
                     $('#divDescarga').css('display', 'inline');
                     $('#btnDescargarExcel').attr('href', 'data:text/csv;base64,' + data.d.Datos.NombreArchivo);
                     let csvContent = 'data:text/csv;base64,' + data.d.Datos.ContenidoBase64;
                     let link = document.createElement('a');
                     link.href = csvContent;
                     link.download = data.d.Datos.NombreArchivo;
                     document.body.appendChild(link);
                     link.click();
                     document.body.removeChild(link);

                 } else {
                     showMsg("ocurrio un error ", data.d.Mensaje, "Error");
                 }
             },
             function (d) {
                 ShowLightBox(false, "");
                 showMsg("Error", d.responseText, "Error");
             }, true);
    };

        document.getElementById('input-excel').addEventListener('change', function (e) {
            const file = e.target.files[0];
            const reader = new FileReader();

            reader.onload = function (e) {
                const data = new Uint8Array(e.target.result);
                const workbook = XLSX.read(data, { type: 'array' });

                const nombreHoja = workbook.SheetNames[0];
                const hoja = workbook.Sheets[nombreHoja];
                const datos = XLSX.utils.sheet_to_json(hoja, { header: 1 });

                let filaInicio = -1;
                let idxIdCredito = -1;
                let idxNoCredito = -1;

                for (let i = 0; i < datos.length; i++) {
                    const fila = datos[i].map(c => (c || '').toString().toLowerCase().trim());
                    if (fila.includes("id cliente") && fila.includes("no. credito") && fila.includes("id cliente")
                        && fila.includes("tipo de credito") && fila.includes("fecha de inicio") && fila.includes("rfc")) {
                        filaInicio = i;
                        idxIdCredito = fila.indexOf("id credito");
                        idxNoCredito = fila.indexOf("no. credito");
                        idxIdCliente = fila.indexOf("id cliente");
                        idxTipoDeCredito = fila.indexOf("tipo de credito");
                        idxFechaDeInicio = fila.indexOf("fecha de inicio");
                        idxRFC = fila.indexOf("rfc");
                        break;
                    }
                }

                if (filaInicio !== -1 && idxIdCredito !== -1 && idxNoCredito !== -1) {
                    const datosExtraidos = [];

                    for (let i = filaInicio + 1; i < datos.length; i++) {
                        const fila = datos[i];
                        const idCredito = fila[idxIdCredito] || '';
                        const noCredito = fila[idxNoCredito] || '';
                        const idcliente = fila[idxIdCliente] || '';
                        const tipodecredito = fila[idxTipoDeCredito] || '';
                        const fechadeinicio = fila[idxFechaDeInicio] || '';
                        const rfc = fila[idxRFC] || '';
                        if (idCredito || noCredito) {
                            datosExtraidos.push({ idCredito, noCredito, idcliente, tipodecredito, fechadeinicio, rfc });
                        }
                    }

                    enviarDatosAServidor(datosExtraidos);
                } else {
                    document.getElementById('tabla-contenedor').innerHTML = "<p>No se encontraron las columnas 'ID Credito' y 'No. Credito'.</p>";
                }
            };

            reader.readAsArrayBuffer(file);
        });

    function enviarDatosAServidor(datosExtraidos) {
        console.log("Enviando datos:", JSON.stringify(datosExtraidos));

        fetch('GeneracionPLD.aspx/btnProcess_Click', { // Asegúrate que la URL sea correcta
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ datosExtraidos }) // Debe coincidir con el parámetro en C#
        })
        .then(response => response.json()) // Convertir respuesta en JSON
        .then(data => console.log("Respuesta del servidor:", data))
        .catch(error => console.error("Error al enviar datos:", error));
    }
    $(document).ready(function () {
        if (amplify.store.sessionStorage("UsuarioID") == null || amplify.store.sessionStorage("UsuarioID") == "") {
            window.location.replace("../../login.aspx");
        }
        $("#Menu").MostrarMenuBar(JSON.parse(amplify.store.sessionStorage("Funcionalidades")), amplify.store.sessionStorage("Nombre"), amplify.store.sessionStorage("NombreEmpresa"), amplify.store.sessionStorage("UsuarioID"));
        $('.popup').setDraggable('.popup-body, .popup-footer, .close');
        ActualizaMenuBar();
        AplicarEstilo();
    });

}(jQuery, window.balor, window.amplify));