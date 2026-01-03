<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReporteDIOT.aspx.cs" Inherits="BalorFinanciera.Contabilidad.Formas.ReporteDIOT" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Balor Financiera</title>
    <meta charset="UTF-8">
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <link href="../../Base/css/normalize.css" rel="stylesheet">
    <link href="../../Base/css/main.css" rel="stylesheet">
    <link href="../../Base/css/balor.css" rel="stylesheet">    
</head>
<body>
    <form id="form1" runat="server">
        <div id="divmsg" class="message"></div>
        
        <div id="divcontentMenu">
            <section class="logo ControlMenu" ></section>
            <section id="Menu" class="ControlMenu"></section>
        </div>
        
        <div id="div_tituloPrincipal" class="content_header">
            <label class="lblTituloContenedor" >Reporte DIOT</label>
        </div>
        
        <div>
            <div>
                <label style="font-weight:bold; font-size:14px;" >Periodo:</label>
            </div>
            <div style="vertical-align:central">
                <input type="hidden" id="txtIniciaPeriodo" />
                <input type="hidden" id="txtTerminaPeriodo" />

                <input id="btnPeriodoAnterior" type="button" value="<" />
			    <label id="lblPeriodo" class="subtitulo">01/01/2018 - 31/01/2018</label>
                <input id="btnPeriodoPosterior" type="button" value=">" />

            </div>
        </div>

        <div id="mainContainer" class="container_body">
            <a id="btnDescargar" href="#" class="btn btn-primary disabled" aria-disabled="true" download=""></a>
            <input id="btnGenerar" type="button" value="Generar" />
        </div>
    </form>

    <script src="../../Base/js/vendor/jquery-1.11.0.min.js" type="text/javascript"></script>  
    <script src="../../Base/js/vendor/amplify.min.js" type="text/javascript"></script>
    <script src="../../Base/Scripts/librerias/jtemplates/jquery-jtemplates.js" type="text/javascript"></script>
    <script src="../../Base/Scripts/librerias/extensions/jquery.HelpField-1.1.js" type="text/javascript"></script>
    <script src="../../Base/js/plugins.js" type="text/javascript"></script>
    <script src="../../Base/js/DateTimeFormat.js" type="text/javascript"></script>
    <script src="../JavaScript/ReporteDIOT.js?v=1.0.1" type="text/javascript"></script>
</body>
</html>
