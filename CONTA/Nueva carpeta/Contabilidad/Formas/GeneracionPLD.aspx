<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GeneracionPLD.aspx.cs" Inherits="BalorFinanciera.Contabilidad.Formas.GeneraPLD" %>

<!DOCTYPE html>
<html lang="es-mx">
<head id="Head1" runat="server">
    <title>Balor Financiera</title>
    <meta charset="UTF-8">
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <link href="../../Base/css/normalize.css" rel="stylesheet" type="text/css" />
    <link href="../../Base/css/main.css" rel="stylesheet" type="text/css" />
    <link href="../../Base/css/balor.css" rel="stylesheet" type="text/css" />
    <link href="../Estilos/CapturaPolizas.css" rel="stylesheet" type="text/css" />
    <!--[if lt IE 9]>
        <script src="//html5shiv.googlecode.com/svn/trunk/html5.js"></script>
    <![endif]-->
</head>
<body>
    <form id="form1" runat="server">
        <div id="divmsg" class="message"></div>

        <div id="divcontentMenu">
            <section class="logo ControlMenu"></section>
            <section id="Menu" class="ControlMenu"></section>
        </div>
        <div id="div_tituloPrincipal" class="content_header">
            <label class="lblTituloContenedor">Generacion de PLD</label>
        </div>
        <div id="mainContainer" class="container_body">
            <div>
                <div class="inlineblock">
                    <fieldset>
                        <legend>Archivo de credito</legend>
                        <p>Seleccionar las fechas deseadas a generar</p>
                        <div class="input-group">
                            <label for="fechaInicio">Fecha de inicio:</label>
                            <input type="date" id="fechaInicio">
                        </div>
                        <div class="input-group">
                            <label for="fechaFin">Fecha de fin:</label>
                            <input type="date" id="fechaFin">
                        </div>
                        <footer>
                            <div>
                                <input id="btnGenerarArchivoCredito" class="btn btn-success" type="button" value="Generar" />
                            </div>
                        </footer>
                    </fieldset>
                </div>
            </div>
            <!-- Región de generación -->
            <div class="inlineblock">
                <fieldset>
                    <legend>Archivo de Pagos</legend>
                    <p>Generar archivo de Pagos.</p>

                    <div id="btnExaminarArchivoCreditos" style="margin px10; margin-top: 15px; width: 118px;">
                        <div id="input-excel" class="fileUpload btnExcel" style="overflow: hidden; margin: 12px;">
                            <span>CARGAR EXCEL</span>
                            <input type="file" class="input-excel" style="position: absolute; top: 0; right: 0; margin: 0; padding: 0; font-size: 20px; cursor: pointer; opacity: 0;" multiple />
                        </div>
                    </div>
                    <footer>
                        <div>
                            <input id="BtnGenerarArchivoPagos" class="btn btn-success" type="button" value="Generar" />
                        </div>
                    </footer>
                </fieldset>
            </div>
        </div>
    </form>
    <script src="../../Base/js/vendor/jquery-1.11.0.min.js" type="text/javascript"></script>
    <script src="../../Base/Scripts/librerias/extensions/autoNumeric-1.9.17.min.js" type="text/javascript"></script>
    <script src="../../Base/js/vendor/amplify.min.js" type="text/javascript"></script>
    <script src="../../Base/js/vendor/moment-with-locales.min.js" type="text/javascript"></script>
    <script src="../../Base/Scripts/librerias/jtemplates/jquery-jtemplates.js" type="text/javascript"></script>
    <script src="../../Base/Scripts/librerias/extensions/jquery.HelpField-1.1.js" type="text/javascript"></script>
    <script src="../../Base/js/plugins.js" type="text/javascript"></script>
    <script src="../../Base/Scripts/librerias/extensions/jquery.ToRequest.min.js" type="text/javascript"></script>
    <script src="../../Base/js/DateTimeFormat.js" type="text/javascript"></script>
    <script src="../JavaScript/GeneracionPLD.js" type="text/javascript"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/xlsx/0.19.2/xlsx.full.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/xlsx/0.17.1/xlsx.full.min.js"></script>
</body>
</html>
