<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CatProveedores.aspx.cs" Inherits="BalorFinanciera.Contabilidad.Formas.CatProveedores" %>

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
            <label class="lblTituloContenedor">Catalogo de Proveedores</label>
        </div>

        <div id="mainContainer" class="container_body">
            <input type="hidden" id="hUltimaAct" />
            <div class="inlineblock">
                <label>RFC</label>
                <input type="text" id="txtRFC"/>
            </div>
            <div class="inlineblock">
                <label for="ddlTipoOperacion">Tipo Operaci&oacute;n</label>
                <select id="ddlTipoOperacion"></select>
            </div>
            <div>
                <div id="AyudaProveedores"></div>
            </div>
            <div>
                <div id="AyudaCuenta"></div>
            </div>

            <fieldset>
                <legend>Domicilio</legend>
                <div>
                    <div class="inlineblock">
                        <label for="txtcalle">Calle</label>
                        <input type="text" id="txtcalle" required />
                    </div>
                    <div class="inlineblock">
                        <label for="txtnumexterior">N&uacute;mero exterior</label>
                        <input type="text" id="txtnumexterior" />
                    </div>
                    <div class="inlineblock">
                        <label for="txtnuminterior">N&uacute;mero interior</label>
                        <input type="text" id="txtnuminterior" />
                    </div>
                    <div class="inlineblock">
                        <label for="txtcodpostal">C&oacute;digo Postal</label>
                        <input type="text" id="txtcodpostal" required />
                    </div>
                </div>
                <!--<div>
                    <div class="inlineblock">
                        <label for="ddlpais">Pais</label>
                        <select id="ddlpais" required></select>
                    </div>
                    <div class="inlineblock">
                        <label for="ddlestado">Estado</label>
                        <select id="ddlestado" required></select>
                    </div>
                    <div class="inlineblock">
                        <label for="ddlmunicipio">Delegaci&oacute;n o municipio</label>
                        <select id="ddlmunicipio" required></select>
                    </div>
                    <div class="inlineblock">
                        <label for="ddlciudad">Ciudad o poblaci&oacute;n</label>
                        <select id="ddlciudad" required></select>
                    </div>
                    <div class="inlineblock">
                        <label for="ddlcolonia">Colonia</label>
                        <select id="ddlcolonia" required></select>
                    </div>
                </div>-->
            </fieldset>
            <footer>
                <input id="btnGuardar" type="button" value="Guardar" />
                <input id="btnLimpiar" type="button" value="Limpiar" />
            </footer>
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
    <script src="../JavaScript/CatProveedores.js?v=1" type="text/javascript"></script>
</body>
</html>
