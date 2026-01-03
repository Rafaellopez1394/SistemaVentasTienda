using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Services;
using System.Diagnostics;

namespace BalorFinanciera.Contabilidad.Formas
{
    public partial class PolizasFacturasProveedores : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        [WebMethod]
        public static Entity.Response<List<Dictionary<string, object>>> ConsultarPolizaPorUUID(string UUID)
        {
            try
            {
                if (string.IsNullOrEmpty(UUID) || UUID.Length != 36)
                {
                    return Entity.Response<List<Dictionary<string, object>>>.CrearResponseVacio<List<Dictionary<string, object>>>(false, "Por favor, ingrese un UUID válido (36 caracteres).");
                }

                Debug.WriteLine($"ConsultarPolizaPorUUID: UUID = {UUID}");
                DataSet ds = MobileBO.ControlContabilidad.TraerPolizasPorUUID(UUID);
                Debug.WriteLine($"ConsultarPolizaPorUUID: DataSet Tables Count = {ds?.Tables.Count ?? 0}");

                // Convertir DataSet a lista de diccionarios
                List<Dictionary<string, object>> resultados = new List<Dictionary<string, object>>();
                if (ds != null && ds.Tables.Count > 0)
                {
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        var dict = new Dictionary<string, object>
                        {
                            { "Fecha", Convert.ToDateTime(row["Fecha"]).ToString("yyyy-MM-dd") },
                            { "TipPol", row["TipPol"] },
                            { "NumPol", row["NumPol"] },
                            { "RFC", row["RFC"] },
                            { "Proveedor", row["Proveedor"] },
                            { "Importe", row["Importe"] }
                        };

                        resultados.Add(dict);
                    }
                }

                return Entity.Response<List<Dictionary<string, object>>>.CrearResponse(true, resultados);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error en ConsultarPolizaPorUUID: {ex.Message}\n{ex.StackTrace}");
                return Entity.Response<List<Dictionary<string, object>>>.CrearResponseVacio<List<Dictionary<string, object>>>(false, $"Error al consultar: {ex.Message}");
            }
        }
    }
}