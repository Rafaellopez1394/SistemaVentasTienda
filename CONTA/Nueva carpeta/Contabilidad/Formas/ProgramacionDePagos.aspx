<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ProgramacionDePagos.aspx.cs" Inherits="BalorFinanciera.Contabilidad.Formas.ProgramacionDePagos" %>

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
</head>
<body>
    <form id="form1" runat="server">
        <div id="divmsg" class="message"></div>
        
        <div id="divcontentMenu">
            <section class="logo ControlMenu" ></section>
            <section id="Menu" class="ControlMenu"></section>
        </div>
        
        <div id="div_tituloPrincipal" class="content_header">
            <label class="lblTituloContenedor" >Programación de pagos</label>
        </div>
        
        <div id="mainContainer" class="container_body">
            <label id="nomempresa" class="subtitulo"></label>
            
            <div id="mensaje"></div>
            
            <div id="sl-msg-container" class="msg-container hidden">
                <div id="sl-msg-body" class="msg-body"></div>
            </div>
            <input id="txtTipo" type="hidden" />
            
            <div id="divEmpresa" style="display:none">
                <h1>Empresa</h1>
                <select id="ddl-empresa" class="select-ajustado"></select>
            </div>
            
            <div id="capturaPago" style="display:inline">
                <input id="hnProgramacionPagoID" type="hidden" />
                <label class="subtitulo">Captura de Pago</label>
                <hr />
                <div style="display:none">
                    <div>
                        <div id="AyudaCuenta"></div>
                    </div>
                </div>
                <div>
                    <div>
                        <div id="AyudaProveedores"></div>
                    </div>
                </div>
                <div>
                    <div class="inlineblock">
                        <label>Fecha Pago:</label>
                        <input id="txtFechaPago" type="text" placeholder="dd/mm/aaaa" pattern="^[0-3][0-9]\/[0-1][0-9]\/[0-9]{4}$" maxlength="10" onKeyUp = "this.value=formateafecha(this.value);" />
                    </div> 
                    <div class="inlineblock">
                        <label for="txtImporte">Importe:</label>
                        <input id="txtImporte" type="text" />
                    </div>
                    <div class="inlineblock">
                        <label for="ddlSolicitantes">Depto Solicitante:</label>
                        <select id="ddlSolicitantes">
                        </select>
                    </div>
                </div>
                <div>
                    <div>
                        <label>Concepto:</label>
                        <textarea id="txtConcepto" style="width:400px; height:100px; max-width:400px; max-height:100px; min-height:20px" maxlength="250" cols="30" rows="15"></textarea>
                    </div>
                </div>
                <div style="width:70%">
                    <div style="margin-top:20px;">
                        <div class="inlineblock">
                            <input id="txtFICaptura" type="text" placeholder="dd/mm/aaaa" pattern="^[0-3][0-9]\/[0-1][0-9]\/[0-9]{4}$" maxlength="10" onKeyUp = "this.value=formateafecha(this.value);" />
                        </div>
                        <div class="inlineblock">
                            <input class="inlineblock" id="txtFFCaptura" type="text" placeholder="dd/mm/aaaa" pattern="^[0-3][0-9]\/[0-1][0-9]\/[0-9]{4}$" maxlength="10" onKeyUp = "this.value=formateafecha(this.value);" />
                        </div>
                        <div class="inlineblock" style="margin-top:5px">
                            <input class="inlineblock" id="btnBuscarCaptura" type="button" value="Buscar" />
                        </div>
                    </div>
                    <div>
                        <table id="tablaPagosCapturados"  class="svGrid" >
                            <thead>
                                <tr class="tr_header">
                                    <th>Fecha</th>
                                    <th>Concepto</th>
                                    <th>Importe</th>
                                    <th>Solicita</th>
                                    <th>Eliminar</th>
                                </tr>
                            </thead>
                            <tbody></tbody>
                            <tfoot>
                                <tr class="">
                                    <td colspan="2"><span>Total</span></td>
                                    <td><span id="sumaImportePagos">0.00</span></td>
                                    <td colspan="2"></td>
                                </tr>
                            </tfoot>
                        </table>
                    </div>

                </div>
            </div>


            <div id="AsignarPoliza" style="display:inline" >
                <label class="subtitulo">Asignar poliza a pago</label>
                <hr />
                <div class="inlineblock">
                    <label>Tipo</label>
                    <select id="ddlTipPol" title="Seleccione el tipo de póliza">
                    <option></option>                
                    </select>
                </div>
                <div>
                    <label for="txtFechaPoliza">Fecha Poliza:</label>
                    <input id="txtFechaPoliza" type="text" placeholder="dd/mm/aaaa" pattern="^[0-3][0-9]\/[0-1][0-9]\/[0-9]{4}$" maxlength="10" onKeyUp = "this.value=formateafecha(this.value);" />
                </div>
                <div class="inlineblock">
                    <div id="AyudaPoliza"></div>
                </div>
                <div style="margin-top:20px">
                    <div class="inlineblock">
                        <input id="txtFIAsignar" type="text" placeholder="dd/mm/aaaa" pattern="^[0-3][0-9]\/[0-1][0-9]\/[0-9]{4}$" maxlength="10" onKeyUp = "this.value=formateafecha(this.value);" />
                    </div>
                    <div class="inlineblock">
                        <input id="txtFFAsignar" type="text" placeholder="dd/mm/aaaa" pattern="^[0-3][0-9]\/[0-1][0-9]\/[0-9]{4}$" maxlength="10" onKeyUp = "this.value=formateafecha(this.value);" />
                    </div>
                    <div class="inlineblock" style="margin-top:5px">
                        <input class="inlineblock" id="btnBuscarAsignar" type="button" value="Buscar" />
                    </div>
                </div>
                <div style="width:70%">
                    <table id="tablaPagosAsignar"  class="svGrid" >
                        <thead>
                            <tr class="tr_header">
                                <th>Fecha</th>
                                <th>Concepto</th>
                                <th>Importe</th>
                                <th>Solicita</th>
                                <th>Asignar</th>
                            </tr>
                        </thead>
                        <tbody></tbody>
                        <tfoot>
                            <tr class="total_general tr_total ">
                                <td colspan="2"><span>Total</span></td>
                                <td><span id="sumaImporteAsignar">0.00</span></td>
                                <td colspan="2"></td>
                            </tr>
                        </tfoot>
                    </table>
                </div>
            </div>
            


        <footer>
          <input id="btnGuardar" type="button" value="Guardar" />
          <input id="btnListadoPagos" type="button" value="Imprimir" />
            <input id="btnRelacionPagos" type="button" value="Relacion Pagos" />
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
  <script src="../JavaScript/ProgramacionDePagos.js?v1" type="text/javascript"></script>
</body>
</html>
