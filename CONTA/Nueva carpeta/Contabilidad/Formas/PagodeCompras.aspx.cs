using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BalorFinanciera.Contabilidad.Formas
{
    public partial class PagodeCompras : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        [WebMethod]
        public static Entity.Response<List<Entity.Contabilidad.Catfacturasproveedor>> TraerCatfacturasproveedor(string Proveedorid, string Empresaid)
        {
            try
            {
                List<Entity.Contabilidad.Catfacturasproveedor> lstFacturas = MobileBO.ControlContabilidad.TraerCatfacturasPorProveedorEmpresa(Proveedorid, Empresaid);
                return Entity.Response<List<Entity.Contabilidad.Catfacturasproveedor>>.CrearResponse<List<Entity.Contabilidad.Catfacturasproveedor>>(true, lstFacturas);
            }
            catch (Exception ex)
            {
                return Entity.Response<List<Entity.Contabilidad.Catfacturasproveedor>>.CrearResponseVacio<List<Entity.Contabilidad.Catfacturasproveedor>>(false, ex.Message);
            }
        }

        [WebMethod]
        public static Entity.Response<List<object>> TraerCuentasPorEmpresa(string empresaid)
        {
            try
            {
                List<object> bancos = new List<object>();

                // Bancos y cuentas que maneja el cliente
                DataSet dsBancos = new DataSet();
                dsBancos = MobileBO.ControlOperacion.TraerCuentasPorEmpresa(empresaid, null, null);
                foreach (DataRow row in dsBancos.Tables[0].Rows)
                {
                    object elemento = new { Empresabancoid = row["EmpresaBancoID"].ToString(), Banco = row["Banco"].ToString(), CuentaCheques = row["Cta_Cheq"].ToString(), Bancoid = row["BancoID"].ToString(), Moneda = row["Moneda"].ToString() };
                    bancos.Add(elemento);
                }

                return Entity.Response<List<object>>.CrearResponse<List<object>>(true, bancos);
            }
            catch (Exception ex)
            {
                return Entity.Response<List<object>>.CrearResponseVacio<List<object>>(false, ex.Message);
            }
        }









    }
}