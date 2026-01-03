<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReporteResumenRelacionSaldos.aspx.cs" Inherits="BalorFinanciera.Cartera.Formas.ReporteResumenRelacionSaldos" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
     <title>Balor Financiera</title>
    <meta charset="UTF-8">
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <link href="../../Base/css/normalize.css" rel="stylesheet" type="text/css" />
    <link href="../../Base/css/main.css" rel="stylesheet" type="text/css" />
    <link href="../../Base/css/balor.css" rel="stylesheet" type="text/css" />
</head>
<body>
     <form id="form1" runat="server">
        <div id="divmsg" class="message">
        </div>
        <div id="divcontentMenu">
            <section class="logo ControlMenu"></section>
            <section id="Menu" class="ControlMenu"></section>
        </div>
        <div id="div_tituloPrincipal" class="content_header">

            <label class="lblTituloContenedor">Reportes Financieros</label>
        </div>
        <div id="mainContainer" class="container_body">
            <div class="inlineblock">
                <span style="font-weight:bold">Seleccione empresa</span>
                <select id="ddl-empresa" class="select-ajustado">
                </select>
            </div>
            <div class="inlineblock">
                <div class="inlineblock">
                    <span style="font-weight:bold">Fecha</span>
                    <input type="text" id="fecha" placeholder="dd/mm/aaaa" pattern="^[0-3][0-9]\/[0-1][0-9]\/[0-9]{4}$"
                        maxlength="10" onkeyup="this.value=formateafecha(this.value);" />
                </div>                      
            </div>
            <div>
                <div style="margin-top:20px">
                    <fieldset class="inlineblock" id="fsRelacionSaldos">
                        <legend>Relación De Saldos</legend>
                        <div>
                            <div class="inlineblock" style="padding:5px;">
                                <input type="checkbox" id="chkRelacionSaldos"  checked="checked" style="padding:10px;" />Relación De Saldos
                            </div>   
                            <div class="inlineblock" style="padding:5px;">                            
                                <input type="checkbox" id="chkmostrarcomprobacion" checked="checked" style="padding:10px;" />Mostrar comprobación de saldos                        
                            </div>
                            <div class="inlineblock" style="padding:5px;">                        
                                <input type="checkbox" id="chkmostrardetallecomprobacion"  />Mostrar detalle de comprobación de saldos                        
                            </div>
                        </div>
                    </fieldset>   
                </div>        

                <div style="margin-top:20px">
                    <fieldset class="inlineblock" id="fsRelacionGastos">
                        <legend>Relación de Gastos</legend>
                        <div style="padding:5px;">
                            <input type="checkbox" id="chkGastos" checked="checked" />Gastos
                        </div>
                    </fieldset>
                    
                </div>     
                <div style="margin-top:20px">
                    <fieldset class="inlineblock"  id="fsEstadoResultados">
                        <legend>Estado de Resultados</legend>
                        <div>
                            <div style="padding:5px;" class="inlineblock">
                                <input type="checkbox" id="chkEstadoResultados"  checked="checked" />Empresa seleccionada
                            </div>                         
                            <div style="padding:5px;" class="inlineblock">
                                <input type="checkbox" id="chkEstadosResultadosConsolidado" />Consolidado
                            </div>
                        </div>
                    </fieldset>

                </div>

            </div>
              
            <footer>
                <input id="BotonImprimir" type="button" value="Imprimir" />          
                <input id="btnLimpiar" type="button" value="Limpiar" />
            </footer>
        </div>
    </form>
    <script src="../../Base/js/vendor/jquery-1.11.0.min.js" type="text/javascript"></script>
    <script src="../../Base/js/vendor/amplify.min.js" type="text/javascript"></script>
    <script src="../../Base/Scripts/librerias/jtemplates/jquery-jtemplates.js" type="text/javascript"></script>
    <script src="../../Base/Scripts/librerias/extensions/jquery.HelpField-1.1.js" type="text/javascript"></script>
    <script src="../../Base/js/plugins.js" type="text/javascript"></script>
    <script src="../../Base/Scripts/librerias/extensions/jquery.ToRequest.min.js" type="text/javascript"></script>
    <script src="../../Base/js/DateTimeFormat.js" type="text/javascript"></script>
    <script src="../JavaScript/ReporteResumenRelacionSaldos.js" type="text/javascript"></script>
</body>
</html>
