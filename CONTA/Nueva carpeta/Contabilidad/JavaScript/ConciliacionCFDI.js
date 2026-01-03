
(function ($, balor, amplify) {
    var msgbx = $('#divmsg');
    var body = $('body');
    var clock = null;

    var showMsg = function (titulo, mensaje, tipoMensaje) {
        $('#divmsg').mostrarMensaje("Título", "Mensaje", "success");
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
    
    var validarCombosYArchivo = function () {
        const empresa = $('#Empresas').val();
        const mes = $('#mes').val();
        const anio = $('#año').val();
        const archivo = $('#FileUploadCFDI').val().toLowerCase();

        const habilitarFile = empresa && mes && anio;
        $('#FileUploadCFDI').prop('disabled', !habilitarFile);

        const esZip = archivo.endsWith('.zip');
        $('#BtnProcesarZip').prop('disabled', !(habilitarFile && esZip));
    };

    var checkZipFile = function () {
        validarCombosYArchivo();
    };


    var leerPDF = function () {
        const fileInput = document.getElementById('pdfInput');
        const file = fileInput.files[0];
        console.log("leerPDF() ejecutado correctamente", file);

        if (!file || !file.name.endsWith('.pdf')) {
            alert('Seleccione un archivo PDF válido.');
            return;
        }

        const reader = new FileReader();
        reader.onload = function () {
            const typedArray = new Uint8Array(this.result);

            pdfjsLib.getDocument(typedArray).promise.then(function (pdf) {
                let totalText = "";
                let pagesProcessed = 0;

                for (let i = 1; i <= pdf.numPages; i++) {
                    pdf.getPage(i).then(function (page) {
                        page.getTextContent().then(function (textContent) {
                            const pageText = textContent.items.map(item => item.str).join(' ');
                            totalText += pageText + "\n";
                            pagesProcessed++;

                            if (pagesProcessed === pdf.numPages) {
                                extraerDatosDesdeTexto(totalText);
                            }
                        });
                    });
                }
            });
        };
        reader.readAsArrayBuffer(file);
    };

    var extraerDatosDesdeTexto = function (texto) {
        console.log("Texto PDF extraído:", texto);
        const nombreExtranjero = (texto.match(/(Canva Pty Ltd|Meta Platforms Ireland Limited|Zoom Communications, Inc.|Zoom Video Communications Inc\.)/) || [])[1] || "NO ENCONTRADO";
                const rfc = "XEXX010101000";
                let pais = "NO ENCONTRADO";
        // Extraer VAT según la empresa
        if (nombreExtranjero === "Canva Pty Ltd") {
            const match = texto.match(/VAT\s*#:\s*([A-Z0-9]+)/i);
            vat = match ? match[1] : "NO ENCONTRADO";
        } else if (nombreExtranjero === "Meta Platforms Ireland Limited") {
            const match = texto.match(/VAT Reg\. No\.\s*([A-Z0-9]+)/i);
            vat = match ? match[1] : "NO ENCONTRADO";
        } else if (nombreExtranjero === "Zoom Communications, Inc."|"Zoom Video Communications Inc.") {
            const match = texto.match(/Zoom Tax Number:\s*([A-Z0-9]+)/i);
            vat = match ? match[1] : "NO ENCONTRADO";
        }
        

        if (/San Jose,\s*CA/i.test(texto)) {
            pais = "USA";
        } else if (/Dublin\s+\d+\s+D\d{2}\s+[A-Z0-9]{4}\s+Ireland/i.test(texto)) {
            pais = "IRL";
        } else if (/Surry Hills\s+2010\s+AU/i.test(texto) || /Australia/i.test(texto)) {
            pais = "AUS";
        } else if (/Ireland/i.test(texto)) {
            pais = "IRL";
        } else if (/USA/i.test(texto)) {
            pais = "USA";
        }

        let importe = "NO ENCONTRADO", tipoIVA = "NO ENCONTRADO", totalIVA = "NO ENCONTRADO", totalPDF = "NO ENCONTRADO", totalCalculado = "";

        // Solo aplicar extracción de importe/IVA/total si es Canva Pty Ltd
        if (nombreExtranjero === "Canva Pty Ltd") {
            // Buscar línea con patrón: Total <importe> <iva> MXN <total>
            const lineaTotalMatch = texto.match(/Total\s+([\d,]+\.\d{2})\s+([\d,]+\.\d{2})\s+MXN\s*([\d,]+\.\d{2})/i);

            if (lineaTotalMatch) {
                importe = parseFloat(lineaTotalMatch[1].replace(/,/g, ''));
                totalIVA = parseFloat(lineaTotalMatch[2].replace(/,/g, ''));
                totalPDF = parseFloat(lineaTotalMatch[3].replace(/,/g, ''));

                tipoIVA = +(totalIVA / importe * 100).toFixed(2);

                const suma = +(importe + totalIVA).toFixed(2);
                const coincide = Math.abs(suma - totalPDF) < 0.01;

                totalCalculado = coincide
                    ? totalPDF.toFixed(2)
                    : `<span style="color:red">${suma.toFixed(2)} (≠ ${totalPDF})</span>`;
            }
        }

        if (nombreExtranjero === "Meta Platforms Ireland Limited") {
            const matchPagado = texto.match(/Pagado\s+\$([\d,.]+)\s+MXN/i);
            if (matchPagado) {
                totalPDF = parseFloat(matchPagado[1].replace(/\./g, '').replace(',', '.'));

                // Establecer IVA en cero explícitamente
                tipoIVA = "exento";
                totalIVA = 0;
                importe = totalPDF;

                totalCalculado = totalPDF.toFixed(2);
            }
        }
        if (nombreExtranjero === "Zoom Communications, Inc."|"Zoom Video Communications Inc.") {
            // Establecer valores como "Dólares"
            totalPDF = "Dólares";
            importe = "Dólares";
            totalIVA = "Dólares";
            if (/Value[\s\S]*?Added[\s\S]*?Tax[\s\S]*?VAT[\s\S]*?16\.000%/i.test(texto)) {
                tipoIVA = "16";
            }
            totalCalculado = "Dólares";
        }
        const efectoFiscal = "01";

        const html = `
        <table class="table table-bordered table-sm">
          <tbody>
            <tr><th>Tipo de tercero</th><td><input type="text" id="TipoTercero" value="05" class="form-control" /></td></tr>
            <tr><th>Tipo de Operación</th><td><input type="text" id="TipoOperacion" value="07" class="form-control" /></td></tr>
            <tr><th>RFC</th><td><input type="text" id="inputRFC" value="${rfc}" class="form-control" /></td></tr>
            <tr><th>ID Fiscal </th><td><input type="text" id="inputVAT" value="${vat}" class="form-control" /></td></tr>
            <tr><th>Nombre Extranjero</th><td><input type="text" id="inputNombreExtranjero" value="${nombreExtranjero}" class="form-control" /></td></tr>
            <tr><th>País de residencia</th><td><input type="text" id="inputPais" value="${pais}" class="form-control" /></td></tr>
            <tr><th>Importe</th><td><input type="text" id="inputImporte" value="${importe}" class="form-control" /></td></tr>
            <tr><th>IVA (${tipoIVA}%)</th><td><input type="text" id="inputIVA" value="${totalIVA}" class="form-control" /></td></tr>
            <tr><th>Total PDF</th><td><input type="text" id="inputTotalPDF" value="${totalPDF}" class="form-control" /></td></tr>
            <tr><th>Total Calculado</th><td>${totalCalculado}</td></tr>
            <tr><th>Efecto Fiscal</th><td><input type="text" id="inputEfectoFiscal" value="01" class="form-control" /></td></tr>
          </tbody>
        </table>
    `;
        mostrarTablaListaPDF();
        document.getElementById("resultadoPDF").innerHTML = html;

        const datos = {
            TipoTercero: "05",
            TipoOperacion: "07",
            RFC: rfc,
            IDFiscal: vat,
            NombreExtranjero: nombreExtranjero,
            PaisResidencia: pais,
            Importe: importe,
            TasaIVA: tipoIVA,
            IVA: totalIVA,
            TotalPDF: totalPDF,
            EfectosFiscales: "01"
        };
        $('#btnAgregarPDFManual').prop('disabled', false);
    };

    document.addEventListener("DOMContentLoaded", function () {
        var hidden = document.getElementById("HiddenJsonPDFLista");
        if (hidden) {
            var json = hidden.value;
            console.log(json);
        } else {
            console.warn("HiddenJsonPDFLista no encontrado en DOM");
        }
    });

    function guardarValoresEditados() {
        const datosEditados = {
            TipoTercero: document.getElementById("TipoTercero").value,
            TipoOperacion: document.getElementById("TipoOperacion").value,
            RFC: document.getElementById("inputRFC").value,
            IDFiscal: document.getElementById("inputVAT").value,
            NombreExtranjero: document.getElementById("inputNombreExtranjero").value,
            PaisResidencia: document.getElementById("inputPais").value,
            Importe: document.getElementById("inputImporte").value,
            TasaIVA: obtenerPorcentajeIVA(), 
            IVA: document.getElementById("inputIVA").value,
            TotalPDF: document.getElementById("inputTotalPDF").value,
            EfectosFiscales: document.getElementById("inputEfectoFiscal").value
        };

        document.getElementById("HiddenJsonPDF").value = JSON.stringify(datosEditados);
    }

    function obtenerPorcentajeIVA() {
        const ths = document.querySelectorAll("th");
        for (let th of ths) {
            if (th.innerText.startsWith("IVA (")) {
                const match = th.innerText.match(/IVA\s+\((\d+)%\)/);
                return match ? match[1] : "0";
            }
        }
        return "0";
    }

    function mostrarTablaListaPDF() {
        const hidden = document.getElementById("HiddenJsonPDFLista");

        if (!hidden) {
            console.warn("⚠️ El campo HiddenJsonPDFLista no se encontró en el DOM.");
            return;
        }

        const json = hidden.value;
        let datos = [];

        try {
            datos = JSON.parse(json);
        } catch (e) {
            console.warn("❌ Error al parsear JSON de HiddenJsonPDFLista:", e);
            return;
        }

        const div = document.getElementById("tablaListaDatosPDF");
        div.innerHTML = ""; // Limpiar contenido anterior

        if (!Array.isArray(datos) || datos.length === 0) {
            div.innerHTML = `<div class="alert alert-warning">No hay datos de PDF cargados.</div>`;
            return;
        }

        let html = `
        <div class="table-responsive">
            <table class="table table-bordered table-sm">
                <thead class="thead-light">
                    <tr>
                        <th>#</th>
                        <th>RFC</th>
                        <th>ID Fiscal</th>
                        <th>Nombre Extranjero</th>
                        <th>País</th>
                        <th>Importe</th>
                        <th>IVA</th>
                        <th>Total</th>
                        <th>Acciones</th>
                    </tr>
                </thead>
                <tbody>
    `;

        datos.forEach((item, i) => {
            html += `
            <tr>
                <td>${i + 1}</td>
                <td>${item.RFC || ''}</td>
                <td>${item.IDFiscal || ''}</td>
                <td>${item.NombreExtranjero || ''}</td>
                <td>${item.PaisResidencia || ''}</td>
                <td>${item.Importe || ''}</td>
                <td>${item.IVA || ''}</td>
                <td>${item.TotalPDF || ''}</td>
                <td>
                    <button type="button" class="btn btn-danger btn-sm" onclick="eliminarPDF(${i})">Eliminar</button>
                </td>
            </tr>
        `;
        });

        html += `
                </tbody>
            </table>
        </div>
    `;

        div.innerHTML = html;
    }

    function eliminarPDF(indice) {
        const json = document.getElementById("HiddenJsonPDFLista").value;
        let datos = [];

        try {
            datos = JSON.parse(json);
        } catch (e) {
            console.error("Error al parsear JSON de PDF:", e);
            return;
        }

        if (indice >= 0 && indice < datos.length) {
            datos.splice(indice, 1); // Elimina el elemento
            document.getElementById("HiddenJsonPDFLista").value = JSON.stringify(datos);

            // Llamar al servidor para actualizar la sesión
            $.ajax({
                type: "POST",
                url: "ConciliacionCfdi.aspx/ActualizarListaPDF",
                data: JSON.stringify({ lista: datos }),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function () {
                    mostrarTablaListaPDF();
                    mostrarMensaje("Éxito", "Registro eliminado correctamente.", "success");
                },
                error: function (xhr, status, error) {
                    mostrarMensaje("Error", "No se pudo eliminar el registro.", "danger");
                }
            });
        }
    }

    $('#btnAgregarPDFManual').on('click', function () {
        guardarValoresEditados();

        const json = document.getElementById("HiddenJsonPDF").value;
        if (!json || json === "{}") {
            mostrarMensaje("Error", "Primero debes procesar un PDF y verificar los datos.", "warning");
            return;
        }

        const datos = JSON.parse(json);

        if (datos.Importe === "Dólares" || datos.IVA === "Dólares" || datos.TotalPDF === "Dólares") {
            mostrarMensaje("Advertencia", "Los campos Importe, IVA o Total PDF no pueden tener el valor 'Dólares'. Por favor ingresa una cantidad numérica válida.", "warning");
            return;
        }

        document.getElementById("BtnAgregarPDF").click();
    });


    

    $(document).ready(function () {
        $('#FileUploadCFDI').prop('disabled', true);
        $('#BtnProcesarZip').prop('disabled', true);
        $('#Empresas, #mes, #año').on('change', validarCombosYArchivo);
        $('#FileUploadCFDI').on('change', checkZipFile);

        validarCombosYArchivo();

        mostrarTablaListaPDF();
        // Vincular botón de leer PDF si se requiere fuera del HTML directamente
        $('#btnLeerPDF').on('click', leerPDF);
    });
   
    var ActualizaMenuBar = function () {
        if (screen.width < 600) {
            $("#MobileMenu").css("display", "line");
            var menu = $("#Menu").html();
            $("#divcontentMenu").remove();
            $("#MobileMenuDet").html(menu);
            $("#nav").css("top", "0");
        }
    }

    mostrarMensaje = function (titulo, mensaje, tipoMensaje) {
        if (typeof Swal !== 'undefined') {
            Swal.fire({
                title: titulo,
                text: mensaje,
                icon: tipoMensaje,
                confirmButtonText: 'Aceptar'
            });
        } 

        return this;
    };

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

    $(document).ready(function () {
        if (amplify.store.sessionStorage("UsuarioID") == null || amplify.store.sessionStorage("UsuarioID") == "") {
            window.location.replace("../../login.aspx");
        }
        $("#Menu").MostrarMenuBar(JSON.parse(amplify.store.sessionStorage("Funcionalidades")), amplify.store.sessionStorage("Nombre"), amplify.store.sessionStorage("NombreEmpresa"), amplify.store.sessionStorage("UsuarioID"));
        $('.popup').setDraggable('.popup-body, .popup-footer, .close');
        ActualizaMenuBar();
        AplicarEstilo();
    });

  
    window.leerPDF = leerPDF
    window.eliminarPDF = eliminarPDF;
    window.showMsg = showMsg;
    window.checkZipFile = checkZipFile;
}(jQuery, window.balor, window.amplify));
