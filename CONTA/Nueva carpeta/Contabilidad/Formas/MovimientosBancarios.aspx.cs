using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;
using System.Data;
using Entity;
namespace BalorFinanciera.Contabilidad.Formas
{
    public partial class MovimientosBancarios : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }
        [WebMethod]
        public static Entity.Response<List<object>> TraerMvtosBancosPorEmpresa(string empresaid)
        {
            try
            {
                List<object> resultado = new List<object>();
                DataSet ds = MobileBO.ControlContabilidad.TraerMvtosBancosPorEmpresa(empresaid);
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    resultado.Add(new
                    {
                        BancoID = row["BancoID"].ToString(),
                        Banco = row["Banco"].ToString()
                    });
                }
                return Entity.Response<List<object>>.CrearResponse<List<object>>(true, resultado);
            }
            catch (Exception ex)
            {
                return Entity.Response<List<object>>.CrearResponseVacio<List<object>>(false, ex.Message);
            }
        }
        [WebMethod]
        public static Entity.Response<List<object>> TraerMovimientosContablesBancarios(string empresaid,string bancoid)
        {
            try
            {
                List<object> resultado = new List<object>();
                DataSet ds = MobileBO.ControlContabilidad.TraerMovimientosContablesBancarios(empresaid, bancoid);
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    resultado.Add(new
                    {
                        AcvGralID = row["AcvGralID"].ToString(),
                        Fec_Pol = DateTime.Parse(row["Fec_Pol"].ToString()).ToShortDateString(),
                        Tip_Pol = row["Tip_Pol"].ToString(),
                        Codigo = row["Codigo"].ToString(),
                        Banco = row["Banco"].ToString(),
                        Beneficiario = row["Beneficiario"].ToString(),
                        Importe = row["Importe"].ToString(),
                        Usuario = row["Usuario"].ToString(),
                        Flujo = row["Flujo"].ToString(),
                        TM = row["TM"].ToString(),
                        Cheque = row["Cheque"].ToString()
                    });
                }
                return Entity.Response<List<object>>.CrearResponse<List<object>>(true, resultado);
            }
            catch (Exception ex)
            {
                return Entity.Response<List<object>>.CrearResponseVacio<List<object>>(false, ex.Message);
            }
        }

        [WebMethod]
        public static Entity.Response<List<object>> TraerDatosPoliza(string empresaid, string acvgralid)
        {
            try
            {
                List<object> resultado = new List<object>();
                bool muestraPoliza = false;
                Entity.Contabilidad.Acvgral acvGral = MobileBO.ControlContabilidad.TraerAcvgral(acvgralid);
                if (empresaid.Trim().ToUpper() == acvGral.EmpresaId.Trim().ToUpper())
                    muestraPoliza = true;
                resultado.Add(new
                {
                    MuestraPoliza = muestraPoliza,
                    Polizaid = (acvGral.ReferenciaId == string.Empty || acvGral.ReferenciaId == null ? Guid.Empty.ToString() : acvGral.ReferenciaId)
                });
                
                return Entity.Response<List<object>>.CrearResponse<List<object>>(true, resultado);
            }
            catch (Exception ex)
            {
                return Entity.Response<List<object>>.CrearResponseVacio<List<object>>(false, ex.Message);
            }
        }

        


    }
}