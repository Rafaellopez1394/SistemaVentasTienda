<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RevisionClientesEnOfac.aspx.cs" Inherits="BalorFinanciera.Contabilidad.Formas.RevisionClientesEnOfac" %>

  <!DOCTYPE html>
    <html lang="es-mx">
    <head id="Head1" runat="server">
    <title>Balor Financiera</title>
    <meta charset="UTF-8">
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <link href="../../Base/css/normalize.css" rel="stylesheet" type="text/css" />
    <link href="../../Base/css/balor.css" rel="stylesheet" type="text/css" />
    <link href="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/css/bootstrap.min.css" rel="stylesheet" />
    <style>
        html, body {
            margin: 0; padding: 0; height: 100%; width: 100%;
            font-size: 12px !important;
          }

            .btn-header {
              background-color: #d32f2f; /* mismo rojo que el header */
              color: white;
              border: none;
              padding: 8px 16px;
              border-radius: 4px;
              cursor: pointer;
            }
            .btn-header:hover {
              opacity: 0.85;
            }
         .no-fullscreen {
              height: auto !important;
              width: auto !important;
              overflow: visible !important;
            }

            .fieldset-compact {
              max-width: 720px;        /* ajusta a gusto */
              margin-left: 16px;       /* pega a la izquierda */
              padding: 8px 12px;       /* acolchado cómodo */
            }

            /* Si quieres que SOLO los resultados tengan scroll */
            .resultados-scroll {
              max-height: 60vh;        /* o calc(100vh - 300px) si quieres preciso */
              overflow-y: auto;
            }
          .fieldset-auto {
           height: auto !important;
           width: 100% !important;

        }
          .modal-left {
        margin-left: 0 !important;
        margin-right: auto !important;
        }
      ieldset:not(.no-fullscreen) {
          height: 98vh;
          width: 98vw;
          overflow: auto;
}
             
    </style>
    <script>
        function cargarListaClientes() {
            const hidden = document.getElementById('<%= HiddenListaOfac.ClientID %>');
            const tbody = document.getElementById("tablaResultados").querySelector("tbody");

            if (!hidden || !hidden.value) {
                console.warn("⚠️ No hay coincidencias en HiddenListaOfac");
                return;
            }

            let coincidencias = [];
            try {
                coincidencias = JSON.parse(hidden.value);
            } catch (e) {
                console.error("❌ Error al parsear JSON:", e);
                return;
            }

            tbody.innerHTML = "";

            if (!coincidencias.length) {
                tbody.innerHTML = `<tr><td colspan="3">❌ No se encontraron coincidencias</td></tr>`;
                return;
            }

            coincidencias.forEach(p => {
                tbody.insertAdjacentHTML("beforeend", `
                    <tr>
                        <td style="border:1px solid #ccc; padding:5px;">${p.Cliente}</td>
                        <td style="border:1px solid #ccc; padding:5px;">${p.Match}</td>
                        <td style="border:1px solid #ccc; padding:5px;">${p.Nivel ?? "⚪ Sin definir"}</td>
                    </tr>`);
            });

            // Mensaje visual
            document.getElementById("divmsg").innerHTML =
                `<div style="background-color:#d4edda;color:#155724;padding:8px;border-radius:4px;">
                    ✅ ${coincidencias.length} coincidencias encontradas en la lista OFAC.
                </div>`;
        }
    </script>

    <!-- 🔹 Scripts de búsqueda manual -->
    <script>
        document.addEventListener("DOMContentLoaded", function () {
            const btnBuscar = document.getElementById("BtnBuscar");
            const tbody = document.getElementById("tablaResultados").querySelector("tbody");
            const rdbOfac = document.getElementById("rdbOfac");
            const rdbClientes = document.getElementById("rdbClientes");
            const hidden = document.getElementById('<%= HiddenListaOfac.ClientID %>');

            btnBuscar.addEventListener("click", async () => {
                const txtNombre = document.getElementById("txtNombre");
                const txtApellido = document.getElementById("txtApellido");

                const nombre = (txtNombre && txtNombre.value ? txtNombre.value.trim().toLowerCase() : "");
                const apellido = (txtApellido && txtApellido.value ? txtApellido.value.trim().toLowerCase() : "");

                tbody.innerHTML = "";

                if (!nombre && !apellido) {
                    alert("Debes ingresar al menos un nombre o apellido.");
                    return;
                }

                // 🔹 Buscar en OFAC (ya cargado en servidor)
                if (rdbOfac.checked) {
                    let listaOfac = [];

                    try {
                        listaOfac = JSON.parse(hidden.value);
                    } catch (e) {
                        alert("Error al interpretar la lista OFAC.");
                        return;
                    }

                    const resultados = listaOfac.filter(p =>
                        p.Cliente.toLowerCase().includes(nombre) &&
                        p.Cliente.toLowerCase().includes(apellido)
                    );

                    if (resultados.length === 0) {
                        tbody.innerHTML = `<tr><td colspan="3">❌ No se encontraron coincidencias en la lista OFAC</td></tr>`;
                    } else {
                        resultados.forEach(p => {
                            tbody.insertAdjacentHTML("beforeend", `
                                <tr>
                                    <td style="border:1px solid #ccc; padding:5px;">${p.Cliente}</td>
                                    <td style="border:1px solid #ccc; padding:5px;">${p.Match}</td>
                                    <td style="border:1px solid #ccc; padding:5px;">${p.Nivel}</td>
                                </tr>`);
                        });
                    }
                }

                // 🔹 Buscar en clientes (desde servidor)
                else if (rdbClientes.checked) {
                    try {
                        const response = await fetch('RevisionClientesEnOfac.aspx/BuscarClientesPorNombre', {
                            method: 'POST',
                            headers: { 'Content-Type': 'application/json; charset=utf-8' },
                            body: JSON.stringify({ nombre: nombre, apellido: apellido })
                        });

                        const data = await response.json();
                        let coincidencias = [];

                        if (typeof data.d === 'string' && data.d.startsWith('ERROR:')) {
                            alert(data.d);
                            return;
                        }

                        coincidencias = JSON.parse(data.d);

                        if (!coincidencias.length) {
                            tbody.innerHTML = `<tr><td colspan="3">❌ No se encontraron coincidencias en clientes</td></tr>`;
                        } else {
                            coincidencias.forEach(p => {
                                tbody.insertAdjacentHTML("beforeend", `
                                    <tr>
                                        <td style="border:1px solid #ccc; padding:5px;">${p.RazonSocial}</td>
                                        <td style="border:1px solid #ccc; padding:5px;">${p.Nombre}</td>
                                        <td style="border:1px solid #ccc; padding:5px;">${p.Tipo}</td>
                                    </tr>`);
                            });
                        }
                    } catch (err) {
                        console.error('❌ Error al consultar clientes:', err);
                        alert('Error al consultar clientes en el servidor.');
                    }
                }
            });
        });
    </script>

    <script>
        function iniciarCuentaAtras() {
            var lbl = document.getElementById('<%= lblResultado.ClientID %>');
            lbl.style.color = 'blue';
            lbl.innerHTML = "⏳ Procesando archivo... tiempo estimado: <span id='countdown'>60</span> segundos.";

            var segundos = 60;
            var countdown = document.getElementById('countdown');
            var intervalo = setInterval(function () {
                segundos--;
                if (countdown) countdown.textContent = segundos;
                if (segundos <= 0) {
                    clearInterval(intervalo);
                }
            }, 1000);
        }
    </script>
</head>

<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePartialRendering="true" />
        <div id="divmsg" style="display: none;" class="mt-2 alert-container"></div>

        <div id="divcontentMenu">
            <section class="logo ControlMenu"></section>
            <section id="Menu" class="ControlMenu"></section>
        </div>

        <div id="div_tituloPrincipal" class="content_header">
            <label class="lblTituloContenedor">REVISIÓN DE OFAC</label>
        </div>

        <div id="mainContainer" class="container_body">
               <fieldset style="padding: 0px; ">
                    <legend>CONSULTA DE PERSONAS FÍSICAS</legend>
                    <div style="max-height:300px; overflow-y:auto; padding:0 0 8px 0;">
                        <div style="display:flex; gap:2px; margin-top:10px;">
      
                            <asp:Button ID="btnProcess" runat="server" Text="BARRIDO DE CLIENTES"
                                        OnClientClick="iniciarCuentaAtras();" OnClick="btnProcess_Click" />
                            <asp:Label ID="lblResultado" runat="server" ForeColor="Green" />
                            <asp:HiddenField ID="HiddenListaOfac" runat="server" />
                        </div>

                        <legend>Buscar manualmente</legend>
                        <div style="margin-bottom:8px;">
                            <label>
                                <input type="radio" name="tipoBusqueda" id="rdbOfac" value="ofac" checked />
                                Buscar en lista OFAC
                            </label>
                            &nbsp;&nbsp;
                            <label>
                                <input type="radio" name="tipoBusqueda" id="rdbClientes" value="clientes" />
                                Buscar en nombres de clientes
                            </label>
                        </div>

                        <label>Nombre: <input type="text" id="txtNombre" /></label>
                        <label>Apellido: <input type="text" id="txtApellido" /></label>
                        <div>
                            <input type="button" id="BtnBuscar" value="Buscar"
                                style="background-color:#28a745; color:white; padding:6px 20px; border-radius:5px; border:none; cursor:pointer;" />
                        </div>
                      </div>
                </fieldset>
            </div>

            <!-- Resultados -->
            <fieldset>
                <legend>Resultados</legend>
                <table id="tablaResultados" style="width:100%; border-collapse:collapse; margin-top:10px;">
                    <thead>
                        <tr>
                            <th style="border:1px solid #ccc; padding:5px;">Nombre</th>
                            <th style="border:1px solid #ccc; padding:5px;">Coincidencia</th>
                            <th style="border:1px solid #ccc; padding:5px;">Nivel</th>
                        </tr>
                    </thead>
                    <tbody></tbody>
                </table>
            </fieldset>
        </div>
    </form>

    <!-- Scripts externos -->
    <script src="https://code.jquery.com/jquery-3.5.1.min.js"></script>
    <script src="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/js/bootstrap.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/pdf.js/2.13.216/pdf.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/xlsx/0.19.2/xlsx.full.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/pdfmake/0.2.7/pdfmake.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/pdfmake/0.2.7/vfs_fonts.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/xlsx/0.18.5/xlsx.full.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/xlsx/0.18.5/xlsx.style.min.js"></script>
    <script src="../../Base/Scripts/librerias/extensions/autoNumeric-1.9.17.min.js"></script>
    <script src="../../Base/js/vendor/amplify.min.js"></script>
    <script src="../../Base/js/vendor/moment-with-locales.min.js"></script>
    <script src="../../Base/Scripts/librerias/jtemplates/jquery-jtemplates.js"></script>
    <script src="../../Base/Scripts/librerias/extensions/jquery.HelpField-1.1.js"></script>
    <script src="../../Base/js/plugins.js"></script>
    <script src="../../Base/Scripts/librerias/extensions/jquery.ToRequest.min.js"></script>
    <script src="../../Base/js/DateTimeFormat.js"></script>
    <script src="../JavaScript/ConciliacionCFDI.js"></script>
</body>
</html>