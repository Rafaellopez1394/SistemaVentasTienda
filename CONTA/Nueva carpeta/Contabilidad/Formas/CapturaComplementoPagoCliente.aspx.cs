using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BalorFinanciera.Contabilidad.Formas
{
    public partial class CapturaComplementoPagoCliente : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        [WebMethod]
        public static Entity.Response<object> EliminarComplemento(int Facturapagoid, string usuario)
        {
            try
            {
                Entity.Operacion.Catfacturaspago pago = MobileBO.DIOT.TraerCatfacturaspagos(Facturapagoid);
                List<Entity.Operacion.Catfacturaspago> lstPago = new List<Entity.Operacion.Catfacturaspago>();
                if (pago != null)
                {
                    pago.Estatus = 2;
                    pago.Fecha = DateTime.Today;
                    pago.Usuario = usuario;
                    lstPago.Add(pago);

                    MobileBO.DIOT.GuardarFacturasPago(ref lstPago);
                }
                else
                    throw new Exception("Factura no encontrada...");




                return Entity.Response<object>.CrearResponse<object>(true, new { Existe = false });
            }
            catch (Exception ex)
            {
                return Entity.Response<object>.CrearResponseVacio<object>(false, ex.Message);
            }
        }

        [WebMethod]
        public static Entity.Response<object> Guardar(string value)
        {
            try
            {
                List<Entity.ModeloFacturaPagoCliente> lstmodelo = MobileBO.Utilerias.Deserializar<List<Entity.ModeloFacturaPagoCliente>>(value);

                

                List<string> lstArchivos = new List<string>();
                Entity.ModeloDIOTCliente modeloDIOTCliente;
                List<Entity.Operacion.Facturaspagoscliente> lstPagos;

                foreach (Entity.ModeloFacturaPagoCliente m in lstmodelo)
                {
                    Entity.Analisis.Catcliente _cliente = MobileBO.ControlAnalisis.TraerCatclientesPorRfc(m.ClienteRFC);
                    m.Clienteid = _cliente.Clienteid;
                    lstPagos = new List<Entity.Operacion.Facturaspagoscliente>();
                    lstArchivos = new List<string>();
                    string Location = pathXML(m.ClienteRFC, m.NomArchivo, m.Empresaid, DateTime.Today);

                    lstArchivos.Add(Location);

                    modeloDIOTCliente = new Entity.ModeloDIOTCliente();
                    modeloDIOTCliente.Cod_Empresa = "0";
                    modeloDIOTCliente.Cod_Cliente = "0";
                    modeloDIOTCliente.Empresaid = m.Empresaid;
                    modeloDIOTCliente.Clienteid = m.Clienteid;
                    modeloDIOTCliente.usuario = m.Usuario;
                    modeloDIOTCliente.listaArchivos = lstArchivos;

                    lstPagos = MobileBO.DIOT.ProcesarXMLComplementoCliente(modeloDIOTCliente);
                }


                return Entity.Response<object>.CrearResponse<object>(true, new { Existe = false });
            }
            catch (Exception ex)
            {
                return Entity.Response<object>.CrearResponseVacio<object>(false, ex.Message);
            }
        }

        public static string pathXML(string rfc, string File, string EmpresaID, DateTime Fecha)
        {
            Entity.Analisis.Catcliente _cliente = MobileBO.ControlAnalisis.TraerCatclientesPorRfc(rfc);
            if (_cliente == null)
                throw new Exception("Error al obtener el cliente de la factura: " + File);
            Entity.Configuracion.Catempresa Empresa = MobileBO.ControlConfiguracion.TraerCatempresas(_cliente.Empresaid);
            string directorio = System.AppDomain.CurrentDomain.RelativeSearchPath;
            //directorio += "\\Proveedores\\" + (Empresa.Empresa == 1 ? "Factur" : "Balor") + " \\" + Fecha.Year.ToString() + "\\" + Fecha.Month + "\\" + proveedor.Rfc;
            directorio += "\\Clientes\\Complemento\\" + (Empresa.Empresa == 1 ? "Factur" : "Balor") + " \\" + Fecha.Year.ToString() + "\\" + Fecha.Month + "\\" + _cliente.Rfc;
            string Location = directorio + "\\" + File;
            return Location;
        }
    }
}