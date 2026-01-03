<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Polizas.aspx.cs" Inherits="BalorFinanciera.Contabilidad.Formas.Polizas" %>

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
    <!--[if lt IE 9]>
        <script src="//html5shiv.googlecode.com/svn/trunk/html5.js"></script>
    <![endif]-->
    <style type="text/css">
        .table {
            width: 100%;
            border-collapse: collapse;
            margin-top: 10px;
        }

            .table th, .table td {
                padding: 8px;
                text-align: left;
                border: 1px solid #ddd;
            }

            .table th {
                background-color: #f2f2f2;
                font-weight: bold;
            }

            .table tbody tr:nth-child(even) {
                background-color: #f9f9f9;
            }

            .table tbody tr:hover {
                background-color: #f5f5f5;
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
            <label class="lblTituloContenedor">Pólizas Administrativas</label>
        </div>
        <div id="mainContainer" class="container_body">
            <h1>Empresa</h1>
            <select id="ddl-empresa" class="select-ajustado">
            </select>

            <div>
                <div class="inlineblock">
                    <label for="txtejercicio">Ejercicio</label>
                    <input id="txtejercicio" type="text" class="inputcorto" maxlength="4" />
                </div>
            </div>

            <footer>
                <input id="btnConsultar" type="button" value="Consultar" />
                <input id="btnLimpiar" type="button" value="Limpiar" />
            </footer>
            <div id="divContabilidades" style="margin-top: 20px;">
                <table id="tblContabilidades" class="table" border="1">
                    <thead>
                        <tr>
                            <th>Año</th>
                            <th>Tipo Contable</th>
                        </tr>
                    </thead>
                    <tbody id="tbodyContabilidades">
                        <!-- Rows will be populated dynamically -->
                    </tbody>
                </table>
            </div>
        </div>
    </form>
    <script src="../../Base/js/vendor/jquery-1.11.0.min.js" type="text/javascript"></script>
    <script src="../../Base/js/vendor/amplify.min.js" type="text/javascript"></script>
    <script src="../../Base/js/plugins.js" type="text/javascript"></script>
    <script src="../JavaScript/Polizas.js?v=1.0.0" type="text/javascript"></script>
</body>
</html>
