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
    public partial class ConsultaComplementoPagoCliente : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        [WebMethod]
        public static Entity.Response<object> TraeFacturasComplementos(string clienteid, string facturaid, string uuid, int? soloconsaldo)
        {
            List<object> facturas = new List<object>();

            try
            {
                DataSet ds = MobileBO.ControlOperacion.TraeFacturasConComplementosFaltantes(clienteid, facturaid, uuid, soloconsaldo);
                foreach(DataRow r in ds.Tables[0].Rows)
                {
                    var f = new
                    {
                        ClienteID = r["ClienteID"].ToString(),
                        Codigo = r["Codigo"].ToString(),
                        NombreCompleto = r["NombreCompleto"].ToString(),
                        FacturaID = r["FacturaID"].ToString(),
                        UUID = r["UUID"].ToString(),
                        Serie = r["Serie"].ToString(),
                        Folio = r["Folio"].ToString(),
                        ImporteFactura = r["ImporteFactura"],
                        NumeroComplementos = Convert.ToInt32(r["NumeroComplementos"]),
                        ImporteComplementos = Convert.ToDecimal(r["ImporteComplementos"]),
                        Restante = Convert.ToDecimal(r["Restante"])
                    };
                    facturas.Add(f);
                }

                var agrupado = ds.Tables[0].AsEnumerable()
                    .GroupBy(x => new
                    {
                        ClienteID = x.Field<Guid>("ClienteID"),
                        Codigo = x.Field<int>("Codigo"),
                        NombreCompleto = x.Field<string>("NombreCompleto")
                    })
                    .Select(s => new
                    {
                        ClienteID = s.Key.ClienteID,
                        Codigo = s.Key.Codigo,
                        NombreCompleto = s.Key.NombreCompleto,
                        TotalFacturas = s.Count(),
                        ImporteFacturas = s.Sum(x => x.Field<decimal>("ImporteFactura")),
                        TotalComplementos = s.Sum(x => x.Field<int>("NumeroComplementos")),
                        ImporteComplementos = s.Sum(x => x.Field<decimal>("ImporteComplementos")),
                        ImporteRestante = s.Sum(x => x.Field<decimal>("Restante"))                        
                    });

                var o = new
                {
                    Agrupado = agrupado,
                    Facturas = facturas
                };

                return Entity.Response<object>.CrearResponse<object>(true, o);
            }
            catch(Exception ex)
            {
                return Entity.Response<object>.CrearResponseVacio<object>(false, ex.Message);
            }
        }
    }
}