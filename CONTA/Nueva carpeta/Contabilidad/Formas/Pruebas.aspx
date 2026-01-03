<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Pruebas.aspx.cs" Inherits="BalorFinanciera.Contabilidad.Formas.Pruebas" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Procesar Archivo de Créditos</title>
    <style>
        .container {
            margin: 20px;
            font-family: Arial, sans-serif;
        }
        .form-group {
            margin-bottom: 15px;
        }
        .form-group label {
            display: block;
            margin-bottom: 5px;
        }
        .form-group input[type="file"] {
            margin-bottom: 10px;
        }
        .btn {
            padding: 10px 20px;
            background-color: #007bff;
            color: white;
            border: none;
            cursor: pointer;
        }
        .btn:hover {
            background-color: #0056b3;
        }
        .message {
            margin-top: 15px;
            padding: 10px;
            border-radius: 5px;
        }
        .message.error {
            background-color: #f8d7da;
            color: #721c24;
        }
        .message.success {
            background-color: #d4edda;
            color: #155724;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <h2>Procesar Archivo de Créditos</h2>
            <div class="form-group">
                <label for="fileUpload">Selecciona el archivo Excel:</label>
                <asp:FileUpload ID="fileUpload" runat="server" />
            </div>
            <asp:Button ID="btnProcess" runat="server" Text="Procesar" CssClass="btn" OnClick="btnProcess_Click" />
            <div>
                <asp:Label ID="lblMessage" runat="server" CssClass="message" Visible="false"></asp:Label>
            </div>
        </div>
    </form>
</body>
</html>