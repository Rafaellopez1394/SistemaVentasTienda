using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BalorFinanciera.Contabilidad.Formas
{
    public partial class CapturaComplementoPago : System.Web.UI.Page
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
                List<Entity.ModeloFacturaPago> lstmodelo = MobileBO.Utilerias.Deserializar<List<Entity.ModeloFacturaPago>>(value);

                List<string> lstArchivos = new List<string>();
                Entity.ModeloDIOT modeloDIOT;
                List<Entity.Operacion.Catfacturaspago> lstPagos;

                foreach (Entity.ModeloFacturaPago m in lstmodelo)
                {
                    lstPagos = new List<Entity.Operacion.Catfacturaspago>();
                    lstArchivos = new List<string>();
                    string Location = pathXML(m.ProveedorRFC, m.NomArchivo, m.Empresaid, DateTime.Today);

                    lstArchivos.Add(Location);

                    modeloDIOT = new Entity.ModeloDIOT();
                    modeloDIOT.Cod_Empresa = "0";
                    modeloDIOT.Cod_Proveedor = "0";
                    modeloDIOT.Empresaid = m.Empresaid;
                    modeloDIOT.Proveedorid = m.Proveedorid;
                    modeloDIOT.usuario = m.Usuario;
                    modeloDIOT.listaArchivos = lstArchivos;

                    lstPagos = MobileBO.DIOT.ProcesarXMLComplemento(modeloDIOT);
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
            Entity.Contabilidad.Catproveedor proveedor = MobileBO.ControlContabilidad.TraerCatproveedores(null, rfc, EmpresaID);
            if (proveedor == null)
                throw new Exception("Error al obtener el proveedor de la factura: " + File);
            Entity.Configuracion.Catempresa Empresa = MobileBO.ControlConfiguracion.TraerCatempresas(proveedor.Empresaid);
            string directorio = System.AppDomain.CurrentDomain.RelativeSearchPath;
            //directorio += "\\Proveedores\\" + (Empresa.Empresa == 1 ? "Factur" : "Balor") + " \\" + Fecha.Year.ToString() + "\\" + Fecha.Month + "\\" + proveedor.Rfc;
            directorio += "\\Proveedores\\Complemento\\" + (Empresa.Empresa == 1 ? "Factur" : "Balor") + " \\" + Fecha.Year.ToString() + "\\" + Fecha.Month + "\\" + proveedor.Rfc;
            string Location = directorio + "\\" + File;
            return Location;
        }
    }
}