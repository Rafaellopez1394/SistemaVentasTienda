<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DetallePolizas.aspx.cs" Inherits="BalorFinanciera.Contabilidad.Formas.DetallePolizas" %>
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
        <label class="lblTituloContenedor" >Pólizas</label>
      </div>
      <div id="mainContainer" class="container_body">
          <div>
        <fieldset>
          <legend>Filtros</legend>
          <div>
              <div class="inlineblock">
                  <label for="">Periodo</label>
                   <input type="text" id="txtPeriodo" class="inputcorto" placeholder="AAAAMM" maxlength="6" />
              </div>
            <div class="inlineblock">
                <label>Tipo</label>
                <select id="ddlTipPol" class="inputcorto">
                <option></option>                
                </select>
            </div> 
            <div class="inlineblock" style="padding-top:15px;">
                <label for="chkPendiente" class="subtitulo"><input id="chkPendiente" type="checkbox" />Pendientes</label>
            </div>        
          </div>
        <div>        
            <input id="btnConsultar" value="Consultar" type="button" />
        </div>
        </fieldset>
        </div>
        <br /><br />
        <div id="divinfo" style="margin:auto">        
        <table id="TablaPolizas" style="margin:0 auto;" class="svGrid">
        <thead>
            <tr class="tr_header">
                <th>Fecha</th>
                <th>Tip Pol</th>
                <th>Num Pol</th>
                <th>Concepto</th>
                <th>Importe</th>
                <th>Usuario</th>
                <th>Facturas</th>
                <th>Complementos</th>
                <th>Nomina</th>
                <th>Imprimir</th>
                <th>Ver Poliza</th>
            </tr>
        </thead>
        <tbody>          
        </tbody>
        </table>
        </div>
        <footer>                    
        </footer>        
      </div>

        <div id="Facturas" class="popup hidden" >
        <div id="Facturas-close" class="close" title="Cerrar">X</div>
        <div id="Facturas-header" class="popup-header"><span id="Span2">Facturas de Compras</span></div>
        <div id="Facturas-body" class="popup-body" >
            <div style="text-align:center;">
                    <label id="lblTitulo" class="subtitulo"></label>
            </div>
            <br /><br />
            <table id="TablaFacturas" style="margin: 0 auto;" class="svGrid">
                    <thead>
                        <tr class="tr_header">
                            <th>Factura</th>
                            <th>Fecha</th>
                            <th>Subtotal</th>
                            <th>Iva</th>
                            <th>Ret_Iva</th>
                            <th>Ret_Isr</th>
                            <th>Total</th>
                        </tr>
                    </thead>
                    <tbody>
                    </tbody>
                    <tfoot>
                        <tr>
                            <td colspan="2" style="font-size: 14px; text-align: right; color: white;">Totales</td>
                            <td id="total-subtotal" class="total-subtotal" style="font-size: 12px; text-align: right; color: white;"></td>
                            <td id="total-iva" class="total-iva" style="font-size: 12px; text-align: right; color: white;"></td>
                            <td id="total-ret-iva" class="total-ret-iva" style="font-size: 12px; text-align: right;  color: white;"></td>
                            <td id="total-ret-isr" class="total-ret-isr" style="font-size: 12px; text-align: right; color: white;"></td>
                            <td id="total-total" class="total-total" style="font-size: 12px; text-align: right; color: white;"></td>
                        </tr>
                    </tfoot>
                </table>
                <br />
                <!--label id="lblSubtotalXML" class="subtitulo" style="color:blue">SUBTOTAL:</label-->                  
        </div>
        <div id="Facturas-footer" class="popup-footer">            
        </div>
        </div>



        <div id="Complementos" class="popup hidden" >
        <div id="Complementos-close" class="close" title="Cerrar">X</div>
        <div id="Complementos-header" class="popup-header"><span id="SpanComplementos">Complementos de Compras</span></div>
        <div id="Complementos-body" class="popup-body" >
            <div style="text-align:center;">
                    <label id="lblTituloComplementos" class="subtitulo"></label>
            </div>
            <br /><br />
            <table id="TablaComplementos" style="margin: 0 auto;" class="svGrid">
                    <thead>
                        <tr class="tr_header">
                            <th>Factura</th>
                            <th>Metodo</th>
                            <th>Fecha</th>
                            <th>Subtotal</th>
                            <th>Iva</th>
                            <th>Total</th>
                            <th>Complemento</th>
                        </tr>
                    </thead>
                    <tbody>
                    </tbody>
                    <tfoot>
                        <tr>
                            <td colspan="3" style="font-size: 14px; text-align: right; color: white;">Totales</td>
                            <td id="totalc-subtotal" class="totalc-subtotal" style="font-size: 12px; text-align: right; color: white;"></td>
                            <td id="totalc-iva" class="totalc-iva" style="font-size: 12px; text-align: right; color: white;"></td>
                            <td id="totalc-total" class="totalc-total" style="font-size: 12px; text-align: right; color: white;"></td>
                            <th></th>
                        </tr>
                    </tfoot>
                </table>
                <br />
                <!--label id="lblSubtotalComplementos" class="subtitulo" style="color:blue">SUBTOTAL:</label-->                
        </div>
        <div id="Complementos-footer" class="popup-footer">            
        </div>
        </div>

        <div id="Complementosnomina" class="popup hidden" >
        <div id="Complementosnomina-close" class="close" title="Cerrar">X</div>
        <div id="Complementosnomina-header" class="popup-header"><span id="SpanComplementosnomina">Complementos de Nómina</span></div>
        <div id="Complementosnomina-body" class="popup-body" >
            <div style="text-align:center;">
                    <label id="lblTituloComplementosnomina" class="subtitulo"></label>
            </div>
            <br /><br />
            <table id="TablaFacturasNomina" style="margin: 0 auto;" class="svGrid">
                    <thead>
                        <tr class="tr">
                            <th colspan="6" style="background-color:#fff !important;"></th>
                            <th colspan="4" style="background-color:#90EE90 !important;">PERCEPCIONES</th>
                            <th colspan="4" style="background-color:#FF4D4D !important;">DEDUCCIONES</th>
                            <th style="background-color:#fff !important;"></th>                            
                        </tr>
                        <tr class="tr_header">
                            <th>Serie</th>
                            <th>Factura</th>
                            <th>Fecha</th>
                            <th>Subtotal</th>
                            <th>Empleado</th>
                            <th>RFC</th>
                            <th>Sueldo</th>
                            <th>Premio <br />Por<br /> Asistencia</th>                            
                            <th>Premio <br />Por <br />Puntualidad</th>
                            <th>Total <br />Percepciones</th>
                            <th>I.S.R.<br />(Mes)</th>
                            <th>I.M.S.S.</th>
                            <th>Préstamo <br />Infonavit<br />(PORC)</th>
                            <th>Total <br />Deducciones</th>
                            <th>Total</th>
                            
                        </tr>
                    </thead>
                    <tbody>
                    </tbody>
                    <tfoot>
                        <tr>
                            <td colspan="6" style="font-size: 14px; text-align: right; color: white;">Totales</td>
                            <td id="totaln-sueldo" class="totaln-sueldo" style="font-size: 12px; text-align: right; color: white;"></td>
                            <td id="totaln-premioasistencia" class="totaln-premioasistencia" style="font-size: 12px; text-align: right; color: white;"></td>
                            <td id="totaln-premiopuntualidad" class="totaln-premiopuntualidad" style="font-size: 12px; text-align: right;  color: white;"></td>
                            <td id="totaln-persepciones" class="totaln-persepciones" style="font-size: 12px; text-align: right; color: white;"></td>
                            <td id="totaln-isrmes" class="totaln-isrmes" style="font-size: 12px; text-align: right; color: white;"></td>
                            <td id="totaln-imss" class="totaln-imss" style="font-size: 12px; text-align: right; color: white;"></td>
                            <td id="totaln-infonavit" class="totaln-infonavit" style="font-size: 12px; text-align: right; color: white;"></td>
                            <td id="totaln-deducciones" class="totaln-deducciones" style="font-size: 12px; text-align: right; color: white;"></td>
                            <td id="totaln-total" class="totaln-total" style="font-size: 12px; text-align: right; color: white;"></td>
                        </tr>
                    </tfoot>
                </table>
                <br />
            <!--div class="inlineblock" style="text-align:right; width:90%; margin:0px; padding:0px;">
                <label id="lblSubtotalXMLNomina" style="color:black; font-weight:bold;">Subtotal</label>
                <br />
                <label id="lblPercepciones" style="color:black; font-weight:bold;">Percepciones</label>
                <br />
                <label id="lblDeducciones" style="color:black; font-weight:bold;">Deducciones</label>
                <br />
                <label id="lblTotal" style="color:black; font-weight:bold;">Total</label>              
            </div>
            <div class="inlineblock" style="text-align:right; width:9%; margin:0px; padding:0px;">
                <label id="lblImporteSubtotalXMLNomina" style="color:black; font-weight:400;"></label>
                <br />
                <label id="lblImportePercepciones" style="color:black; font-weight:400;"></label>
                <br />
                <label id="lblImporteDeducciones" style="color:black; font-weight:400;"></label>
                <br />
                <label id="lblImporteTotal" style="color:black; font-weight:400;"></label>              
            </div-->
                
        </div>
        <div id="Complementosnomina-footer" class="popup-footer">            
        </div>
        </div>




    </form>
  <script src="../../Base/js/vendor/jquery-1.11.0.min.js" type="text/javascript"></script>  
  <script src="../../Base/js/vendor/amplify.min.js" type="text/javascript"></script>    
  <script src="../../Base/js/plugins.js" type="text/javascript"></script>    
  <script src="../JavaScript/DetallePolizas.js?v=1.1" type="text/javascript"></script>
</body>
</html>
