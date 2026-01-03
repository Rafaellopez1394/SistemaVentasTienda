<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CapturaDePresupuestos.aspx.cs" Inherits="BalorFinanciera.Contabilidad.Formas.CapturaDePresupuestos" %>

<!DOCTYPE html>
<html lang="es-mx">
<head id="Head1" runat="server">
    <title>Balor Financiera</title>
    <meta charset="UTF-8">
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <link href="../../Base/css/normalize.css" rel="stylesheet">
    <link href="../../Base/css/main.css" rel="stylesheet">
    <link href="../../Base/css/balor.css" rel="stylesheet">
    <link href="../../Base/css/flipclock.css" rel="stylesheet">
    <link href="../Estilos/CapturaDePresupuestos.css" rel="stylesheet" type="text/css" />
    <!--[if lt IE 9]>
        <script src="//html5shiv.googlecode.com/svn/trunk/html5.js"></script>
    <![endif]-->
</head>
<body>
    <form id="form1" runat="server">
    <div id="divmsg" class="message">
    </div>
    <div id="MobileMenu" style="display: none">Menu</div>
    <div id="MobileMenuDet" style="display: none"></div>
    <div id="divcontentMenu">
        <section class="logo ControlMenu"></section>
        <section id="Menu" class="ControlMenu"></section>
    </div>
    <div id="div_tituloPrincipal" class="content_header">
        <label class="lblTituloContenedor">Captura de Presupuestos</label>
    </div>
    <div id="mainContainer" class="container_body">
        <div>
            <div>
                <label id="Label2" class="subtitulo">Empresa</label>
                <select id="ddl-empresa" class="select-ajustado"></select>
            </div>  
            <div>
                <div class="inlineblock">
                    <div id="ayudaVendedor"></div>
                </div>
            </div>
            <div>
                <div class="inlineblock">    
                    <label id="Label3">Año</label>
                    <input type="text" id="txtAnio"/>
                    <input type="button" id="btnConsultar" value="Consultar" />
                </div>      
            </div>
        </div>
        <br />
        <div id="dviCuerpo" style="display:none">
            <div id="divinfo" style="margin: auto">
                <table id="tbPresupuestos" class="svGridPresupuesto">
                    <thead>
                        <tr class="tr_header">
                            <th>Mes</th>
                            <th>Ctes. Autorizados</th>
                            <th>Ctes. Nvos. Operando</th>
                            <th>Colocacion Cte. Nvo.</th>
                            <th>Colocacion Aumentos</th>
                            <th>Colocacion Operativa</th>
                        </tr>
                    </thead>
                    <tbody>
                    </tbody>
                </table>
            </div>
            <br />
            <input type="button" id="btnGuardar" value="Guardar"/>
            <input type="button" id="btnLimpiar" value="Limpiar"/>
        </div>
    </div>
    




    </form>
    <script src="../../Base/js/vendor/jquery-1.11.0.min.js"></script>
    <script src="../../Base/js/vendor/amplify.min.js"></script>    
    <script src="../../Base/js/plugins.js"></script>   
    <script src="../../Base/js/DateTimeFormat.js" type="text/javascript"></script> 
    <script src="../../Base/Scripts/librerias/jtemplates/jquery-jtemplates.js"></script>
    <script src="../../Base/Scripts/librerias/extensions/jquery.HelpField-1.1.js"></script>
    <script src="../JavaScript/CapturaDePresupuestos.js" type="text/javascript"></script>
</body>
</html>

