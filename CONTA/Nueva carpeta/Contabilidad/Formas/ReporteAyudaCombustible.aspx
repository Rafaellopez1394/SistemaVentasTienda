<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReporteAyudaCombustible.aspx.cs" Inherits="BalorFinanciera.Contabilidad.Formas.ReporteAyudaCombustible" %>

<!DOCTYPE html>

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
        <div>
            <div id="divmsg" class="message"></div>
        
            <div id="divcontentMenu">
                <section class="logo ControlMenu" ></section>
                <section id="Menu" class="ControlMenu"></section>
            </div>
        
            <div id="div_tituloPrincipal" class="content_header">
                <label class="lblTituloContenedor" >Reporte de Apoyo en Combustible</label>
            </div>

             <div id="mainContainer" class="container_body">
                <div class="inlineblock">
                    <div class="inlineblock">
                        <div id="ayudaCliente"></div>
                    </div>
                </div>                         
                <div>     
                                
                    <div class="inlineblock">
                        <label for="fechaInicio">Fecha inicial</label>
                        <input id="fechaInicio" type="text" placeholder="dd/mm/aaaa" pattern="^[0-3][0-9]\/[0-1][0-9]\/[0-9]{4}$" maxlength="10" onkeyup="this.value=formateafecha(this.value);">
                    </div>                   
                    <div class="inlineblock">
                        <label for="fechaFinal">Fecha final</label>
                        <input id="fechaFinal" type="text" placeholder="dd/mm/aaaa" pattern="^[0-3][0-9]\/[0-1][0-9]\/[0-9]{4}$" maxlength="10" onkeyup="this.value=formateafecha(this.value);">
                    </div>   
                    
                    
                </div>
               <%--  <div>
                     <div class="inlineblock">
                        <label for="ddlEmpresas">Empresa</label>
                        <select id="ddlEmpresas" class="cjComboBox" required style="min-width:450px"></select>
                    </div>
                 </div>--%>
                 <div>
                     <div class="inlineblock">
                        <label for="ddlVendedores">Vendedor</label>
                        <select id="ddlVendedores" class="cjComboBox" required style="min-width:450px"></select>
                    </div>                     
                 </div>
               <%--  <div>
                     <div class="inlineblock">
                        <label for="ddlTipoVendedores">Tipo Vendedor</label>
                        <select id="ddlTipoVendedores" class="cjComboBox" required style="min-width:100px">
                            <option value="*">Todos..</option>
                            <option value="A1">A1</option>
                            <option value="B1">B1</option>
                            <option value="C1">C1</option>
                            <option value="D1">Gerentes</option>
                            <option value="E1">Director Comercial</option>
                        </select>
                    </div>
                    <div class="inlineblock" style="vertical-align:middle">
                        <label for="chkAgrupado">Agrupado</label>
                        <input type="checkbox" id="chkAgrupado" checked="checked"  required />
                    </div>    
                 </div>--%>
                 
                <br />
                 <%--<input id="btnGenerar" type="button" value="Calcular comisiones" class="boton DisabledGral" />--%>            
                <%--<input id="btnGuardar" type="button" value="Guardar" class="boton DisabledGral" />--%>            
                 <input id="btnImprimir" type="button" value="Imprimir" class="boton DisabledGral" />            
                 <%--<input id="btnImprimirTipoVendedor" type="button" value="Imprimir por tipo de vendedor" class="boton DisabledGral" />--%>            
                <input id="btnLimpiar" type="button" value="Limpiar" class="boton"/>

              
            </div>
        </div>
    </form>

     <script src="../../Base/js/vendor/jquery-1.11.0.min.js" type="text/javascript"></script>  
    <script src="../../Base/js/vendor/amplify.min.js" type="text/javascript"></script>
    <script src="../../Base/Scripts/librerias/jtemplates/jquery-jtemplates.js" type="text/javascript"></script>
    <script src="../../Base/Scripts/librerias/extensions/jquery.HelpField-1.1.js" type="text/javascript"></script>
    <script src="../../Base/js/plugins.js" type="text/javascript"></script>
    <script src="../../Base/js/DateTimeFormat.js" type="text/javascript"></script>
    <script src="../JavaScript/ReporteAyudaCombustible.js"></script>
</body>
</html>
