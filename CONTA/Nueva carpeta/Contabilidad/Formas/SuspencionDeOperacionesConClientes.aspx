<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SuspencionDeOperacionesConClientes.aspx.cs" Inherits="BalorFinanciera.Contabilidad.Formas.SuspencionDeOperacionesConClientes" %>

  <!DOCTYPE html>
    <html lang="es-mx">
    <head id="Head1" runat="server">
    <title>Balor Financiera</title>
    <meta charset="UTF-8">
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <link href="../../Base/css/normalize.css" rel="stylesheet" type="text/css" />
    <link href="../../Base/css/balor.css" rel="stylesheet" type="text/css" />
    <link href="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/css/bootstrap.min.css" rel="stylesheet" />
         <script>
    window.onload = function() {
        var usuario = amplify.store.sessionStorage("Usuario");
        document.getElementById('<%= usuarioHidden.ClientID %>').value = usuario;
    };
</script>

         <style>
      html, body {
        margin: 0; padding: 0; height: 100%; width: 100%;
        font-size: 12px !important;
      }

        .btn-header {
          background-color: #d32f2f; /* mismo rojo que el header */
          color: white;
          border: none;
          padding: 8px 16px;
          border-radius: 4px;
          cursor: pointer;
        }
        .btn-header:hover {
          opacity: 0.85;
        }
      fieldset:not(.no-fullscreen) {
            height: 98vh; 
            width: 98vw; 
            box-sizing: border-box;
            padding: 5px; 
            border: 3px solid #ccc; 
            overflow: auto;
            background-color: #fdfdfd;
        }
      .fieldset-auto {
       height: auto !important;
       width: 100% !important;

    }
      .modal-left {
    margin-left: 0 !important;
    margin-right: auto !important;
}
             
    </style>
</head>
<body>
<form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePartialRendering="true" />
    <div id="divmsg" style="display: none;" class="mt-2 alert-container"></div>

    <div id="divcontentMenu">
        <section class="logo ControlMenu"></section>
        <section id="Menu" class="ControlMenu"></section>
    </div>
    <asp:HiddenField ID="usuarioHidden" runat="server" />
    <div id="div_tituloPrincipal" class="content_header">
        <label class="lblTituloContenedor">
            LISTADO DE PERSONAS FÍSICAS Y MORALES CON SUSPENSIÓN TEMPORAL DE OPERACIONES
        </label>
    </div>

    <div class="d-flex justify-content-center mt-3">

        <!-- ✅ Lista de suspendidos -->
        <fieldset class="mb-3">
            <legend>📋 Personas con Suspensión de Operaciones</legend>
            <asp:UpdatePanel ID="upSuspendidos" runat="server">
                <ContentTemplate>
                    <asp:GridView ID="gvSuspendidos" runat="server" AutoGenerateColumns="False"
                        CssClass="table table-bordered table-striped table-sm"
                        OnRowCommand="gvSuspendidos_RowCommand">
                        <Columns>
                            <asp:BoundField DataField="BloqueoID" HeaderText="Bloqueo ID" />
                            <asp:BoundField DataField="TipoPersona" HeaderText="Tipo" />
                            <asp:BoundField DataField="Nombre" HeaderText="Nombre / Razón Social" />
                            <asp:BoundField DataField="ApellidoPaterno" HeaderText="Apellido Paterno" />
                            <asp:BoundField DataField="ApellidoMaterno" HeaderText="Apellido Materno" />
                            <asp:BoundField DataField="Nacionalidad" HeaderText="Nacionalidad" />
                            <asp:BoundField DataField="TipoDocumento" HeaderText="Documento" />
                            <asp:BoundField DataField="NumeroDocumento" HeaderText="Número" />
                            <asp:BoundField DataField="ListaOFAC" HeaderText="Lista" />
                            <asp:BoundField DataField="FechaInclusion" HeaderText="F. Inclusión" DataFormatString="{0:dd/MM/yyyy}" />
                            <asp:BoundField DataField="Motivo" HeaderText="Motivo" />
                            <asp:BoundField DataField="UsuarioRegistro" HeaderText="Registrado por" />
                            <asp:BoundField DataField="FechaRegistro" HeaderText="F. Registro" DataFormatString="{0:dd/MM/yyyy HH:mm}" />
                            <asp:TemplateField HeaderText="Acciones">
                                <ItemTemplate>
                                    <asp:LinkButton ID="btnCambiarEstatus" runat="server"
                                        CommandName="CambiarEstatus"
                                        CommandArgument='<%# Eval("BloqueoID") %>'
                                        CssClass="btn btn-sm btn-warning">
                                        ⚙️ Cambiar Estatus
                                    </asp:LinkButton>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Estatus">
                                <ItemTemplate>
                                    <%# GetEstatusTexto(Eval("Estatus")) %>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </ContentTemplate>
            </asp:UpdatePanel>
        </fieldset>

        <!-- ✅ Modal Cambiar Estatus -->
        <div class="modal fade" id="modalCambiarEstatus" tabindex="-1" role="dialog" aria-labelledby="modalCambiarEstatusLabel" aria-hidden="true">
            <div class="modal-dialog" role="document">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title" id="modalCambiarEstatusLabel">Cambiar Estatus del Cliente</h5>
                        <button type="button" class="close" data-dismiss="modal" aria-label="Cerrar">
                            <span aria-hidden="true">&times;</span>
                        </button>
                    </div>
                    <div class="modal-body">
                        <asp:Label ID="lblClienteSeleccionado" runat="server" CssClass="font-weight-bold d-block mb-2"></asp:Label>
                        <label for="ddlEstatus">Nuevo Estatus:</label>
                        <asp:DropDownList ID="ddlEstatus" runat="server" CssClass="form-control mb-3">
                            <asp:ListItem Text="🔒 Bloquear" Value="Bloquear" />
                            <asp:ListItem Text="🔓 Desbloquear" Value="Desbloquear" />
                            <asp:ListItem Text="🚫 Permanente" Value="Permanente" />
                        </asp:DropDownList>

                        <label for="txtMotivo">Motivo / Descripción:</label>
                        <asp:TextBox ID="txtMotivo" runat="server" TextMode="MultiLine"
                            CssClass="form-control" Height="80px"
                            Placeholder="Escriba el motivo del cambio..."></asp:TextBox>
                    </div>
                    <div class="modal-footer">
                        <asp:Button ID="btnGuardarEstatus" runat="server" Text="Guardar Cambios" CssClass="btn btn-success" OnClick="btnGuardarEstatus_Click" />
                        <button type="button" class="btn btn-secondary" data-dismiss="modal">Cancelar</button>
                    </div>
                </div>
            </div>
        </div>
      </div>
      <div class="d-flex justify-content-center mt-3">
        <!-- ✅ Sección búsqueda -->
        <fieldset class="mb-3">
            <legend>🔍 Búsqueda de Clientes y Avales</legend>
            <div class="d-flex align-items-center gap-2">
                <asp:TextBox ID="txtBuscar" runat="server" Width="300px" CssClass="form-control" Placeholder="Buscar por nombre..." />
                <asp:Button ID="btnBuscar" runat="server" Text="Buscar" CssClass="btn btn-primary" OnClick="btnBuscar_Click" />
            </div>

        <!-- ✅ Grid de clientes -->
        <asp:UpdatePanel ID="upGrid" runat="server">
            <ContentTemplate>
                <asp:GridView ID="gvClientes" runat="server" AutoGenerateColumns="False"
                    CssClass="table table-bordered table-striped mt-3"
                    OnSelectedIndexChanged="gvClientes_SelectedIndexChanged">
                    <Columns>
                        <asp:BoundField DataField="ClienteID" HeaderText="ID Cliente" />
                        <asp:BoundField DataField="NombreCliente" HeaderText="Cliente" />
                        <asp:BoundField DataField="FechaAlta" HeaderText="Fecha Alta" DataFormatString="{0:dd/MM/yyyy}" />
                        <asp:BoundField DataField="AvalID" HeaderText="ID Aval" />
                        <asp:BoundField DataField="NombreAval" HeaderText="Aval" />
                        <asp:BoundField DataField="RepresentanteLegal" HeaderText="Representante" />
                        <asp:CheckBoxField DataField="EsAval" HeaderText="Es Aval" />
                        <asp:CheckBoxField DataField="EsAccionista" HeaderText="Accionista" />
                        <asp:CheckBoxField DataField="EsRepresentante" HeaderText="Representante" />
                        <asp:CheckBoxField DataField="EsAccionistaMayoritario" HeaderText="Mayoritario" />
                        <asp:TemplateField HeaderText="Acciones">
                            <ItemTemplate>
                                <asp:LinkButton ID="btnSeleccionar" runat="server" CommandName="Select"
                                    CssClass="btn btn-sm btn-primary mb-1">➕ Seleccionar</asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </ContentTemplate>
        </asp:UpdatePanel>

        </fieldset>
        <asp:Label ID="lblResultado" runat="server" CssClass="mt-2 d-block" ForeColor="Blue"></asp:Label>
    </div>
        <!-- ✅ Modal Agregar Descripción -->
    <div class="modal fade" id="modalDescripcion" tabindex="-1" role="dialog" aria-labelledby="modalDescripcionLabel" aria-hidden="true">
        <div class="modal-dialog" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title" id="modalDescripcionLabel">Agregar descripción</h5>
                    <button type="button" class="close" data-dismiss="modal" aria-label="Cerrar">
                        <span aria-hidden="true">&times;</span>
                    </button>
                </div>
                <div class="modal-body">
                    <!-- ⚙️ Estos son los controles que faltaban -->
                    <asp:Label ID="lblSeleccion" runat="server" CssClass="font-weight-bold d-block mb-2"></asp:Label>
                    <asp:TextBox ID="txtDescripcion" runat="server" TextMode="MultiLine"
                        CssClass="form-control" Height="80px"
                        Placeholder="Escriba una descripción..."></asp:TextBox>
                </div>
                <div class="modal-footer">
                    <asp:Button ID="btnGuardar" runat="server" Text="Guardar" CssClass="btn btn-success" OnClick="btnGuardar_Click" />
                    <button type="button" class="btn btn-secondary" data-dismiss="modal">Cancelar</button>
                </div>
            </div>
        </div>
    </div>
    <!-- ✅ Librerías Bootstrap -->
    <script src="https://code.jquery.com/jquery-3.5.1.min.js"></script>
    <script src="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/js/bootstrap.bundle.min.js"></script>
    
    <script src="../../Base/Scripts/librerias/extensions/autoNumeric-1.9.17.min.js"></script>
    <script src="../../Base/js/vendor/amplify.min.js"></script>
    <script src="../../Base/js/vendor/moment-with-locales.min.js"></script>
    <script src="../../Base/Scripts/librerias/jtemplates/jquery-jtemplates.js"></script>
    <script src="../../Base/Scripts/librerias/extensions/jquery.HelpField-1.1.js"></script>
    <script src="../../Base/js/plugins.js"></script>
    <script src="../../Base/Scripts/librerias/extensions/jquery.ToRequest.min.js"></script>
    <script src="../../Base/js/DateTimeFormat.js"></script>
    <script src="../JavaScript/ConciliacionCFDI.js"></script>

   
</form>
</body>
</html>