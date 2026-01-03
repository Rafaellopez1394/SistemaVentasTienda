<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ConsultaComplementoPagoCliente.aspx.cs" Inherits="BalorFinanciera.Contabilidad.Formas.ConsultaComplementoPagoCliente" %>

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
  <link href="../Estilos/ConsultaComplementoPagoCliente.css" rel="stylesheet" />
  <!--[if lt IE 9]>
        <script src="//html5shiv.googlecode.com/svn/trunk/html5.js"></script>
    <![endif]-->
</head>
<body>
    <form id="form1" runat="server">
        <div id="divmsg" class="message"></div>
        
        <div id="divcontentMenu">
            <section class="logo ControlMenu" ></section>
            <section id="Menu" class="ControlMenu"></section>
        </div>
        
        <div id="div_tituloPrincipal" class="content_header">
            <label class="lblTituloContenedor" >Facturas con Complementos Faltantes</label>
        </div>
        
        <div id="mainContainer" class="container_body">
            <label id="nomempresa" class="subtitulo"></label>
            
            <div id="mensaje"></div>
            
            <div id="sl-msg-container" class="msg-container hidden">
                <div id="sl-msg-body" class="msg-body"></div>
            </div>
           
            <div class="inlineblock">
                <div id="ayudaCliente"></div>
            </div>
            <%--<div>
                <div class="inlineblock">
                    <label id="serie">Serie</label>
                    <input type="text" id="txtserie" style="width:50px; text-align:center;" />
                </div>
                <div class="inlineblock">
                    <label id="folio">Folio</label>
                    <input type="text" id="txtfolio"  style="width:100px; text-align:center;"/>
                </div>
                <div class="inlineblock" style="vertical-align:bottom">
                    <input type="button" id="btnfacturas" value="Ver todas las facturas" />
                </div>
            </div>--%>
            <div>
                <div class="inlineblock" style="vertical-align:bottom; margin:20px;">
                    <input type="checkbox" id="chksoloconsaldo" checked="checked" />Solo con Saldo
                </div>
            </div>
            <div class="inlineblock" style="vertical-align:bottom">
                <input type="button" id="btnconsultar" value="Consultar" />
            </div>
            <br />

            <div style="margin:20px; width:90%" id="agrupado">
                
            </div>

            <div style="margin:20px; width:90%">
                <%--<table id="tblfacturas" style="margin: 0 auto;" class="svGrid">
                    <thead>
                        <tr>
                            <th>Cliente</th>
                            <th>Factura</th>
                            <th>UUID</th>
                            <th>Importe Factura</th>
                            <th># Complementos</th>
                            <th>Importe Complementos</th>
                            <th>Restante</th>
                        </tr>
                    </thead>
                    <tbody></tbody>
                    <tfoot style="font-size:14px;"></tfoot>
                </table>--%>
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
    <script src="../JavaScript/ConsultaComplementoPagoCliente.js"></script>    
</body>

</html>

