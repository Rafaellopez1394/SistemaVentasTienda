<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PolizasFacturasProveedores.aspx.cs" Inherits="BalorFinanciera.Contabilidad.Formas.PolizasFacturasProveedores" %>

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
    <style>
        .grid { width: 100%; border-collapse: collapse; margin: 20px 0; }
        .grid th, .grid td { border: 1px solid #ddd; padding: 8px; text-align: left; }
        .grid th { background-color: #f2f2f2; }
        .error { color: red; }
        .container_body { margin: 20px; }
        .inlineblock { display: inline-block; margin-right: 10px; }
        .form-control { padding: 5px; width: 250px; }
        .boton { padding: 5px 10px; cursor: pointer; }
        .highlight { background-color: #e0f7fa; }
        #Resultados {
    color: black !important;
    background-color: white !important;
}

#tblPolizas td, #tblPolizas th {
    color: black !important;
    background-color: white !important;
    border: 1px solid #ccc;
}

#tblPolizas .highlight {
    background-color: #f0f8ff !important;
}

    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div id="divmsg" class="message"></div>
        <div id="divcontentMenu">
            <section class="logo ControlMenu"></section>
            <section id="Menu" class="ControlMenu"></section>
        </div>
        <div id="div_tituloPrincipal" class="content_header">
            <label class="lblTituloContenedor">Consulta de Pólizas por UUID</label>
        </div>
        <div id="mainContainer" class="container_body">
            <div class="inlineblock">
                <label for="txtUUID">UUID</label>
                <input id="txtUUID" type="text" maxlength="36" placeholder="Ingrese el UUID" class="form-control" style="width: 300px;" />
            </div>
            <div class="inlineblock">
                <input id="btnConsultar" type="button" value="Consultar" class="boton" />
            </div>
        </div>
        <div id="Resultados" style="display:none;">
            <fieldset>
                <legend>Resultados de la Consulta</legend>
                <table id="tblPolizas" class="grid">
                    <thead>
                        <tr>
                            <th>Fecha</th>
                            <th>Tipo Póliza</th>
                            <th>Número Póliza</th>
                            <th>RFC</th>
                            <th>Proveedor</th>
                            <th>Importe</th>
                        </tr>
                    </thead>
                    <tbody></tbody>
                </table>
            </fieldset>
        </div>
    </form>
    <script src="../../Base/js/vendor/jquery-1.11.0.min.js" type="text/javascript"></script>
    <script src="../../Base/js/vendor/amplify.min.js" type="text/javascript"></script>
    <script src="../../Base/js/plugins.js" type="text/javascript"></script>
    <script src="../JavaScript/PolizasFacturasProveedores.js" type="text/javascript"></script>
</body>
</html>