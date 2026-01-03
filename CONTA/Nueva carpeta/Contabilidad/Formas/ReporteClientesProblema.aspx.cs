using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using Entity;
using System.Data;
using Homex.Core.Utilities;

namespace BalorFinanciera.Contabilidad.Formas
{
    public partial class ReporteClientesProblema : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
    }

    public class ControladorReporteSaldoClientesProblema : Base.Clases.BaseReportes
    {
        public override void InicializaReporte(System.Collections.Specialized.NameValueCollection parametros)
        {
            DataSet ds = new DataSet();
            try
            {
                string empresaid = parametros.Get("empresa");
                string fc = parametros.Get("fc");
                string ConSaldoDemandado = parametros.Get("ConSaldoDemandado");

                Entity.Configuracion.Catempresa catempresa = MobileBO.ControlConfiguracion.TraerCatempresas(empresaid);

                DateTime FechaCorte = DateTime.Parse(fc);
                DataSet CesionesPorCobrar = MobileBO.ControlOperacion.CesionesPorCobrar_Select(null, FechaCorte, catempresa.Empresa);

                List<ModeloSaldoCesion> ListaCesionesBalor = new List<ModeloSaldoCesion>();
                Entity.Operacion.Catclientesmoroso moroso;
                foreach (DataRow row in CesionesPorCobrar.Tables[0].Rows)
                {
                    //PROBLEMA
                    moroso = MobileBO.ControlOperacion.TraerCatclientesmorosos(null, null, row["ClienteID"].ToString(), 3);

                    if (moroso == null)
                    {
                        //MOROSO
                        moroso = MobileBO.ControlOperacion.TraerCatclientesmorosos(null, null, row["ClienteID"].ToString(), 1);

                        if (moroso == null)
                        {
                            ModeloSaldoCesion Saldo;
                            if (ConSaldoDemandado == "0")
                                Saldo = MobileBO.ControlOperacion.CalculaInteresOptimizado(row["CesionID"].ToString(), FechaCorte, true);
                            else
                                Saldo = MobileBO.ControlOperacion.CalculaInteresOptimizadoConSaldoDemandado(row["CesionID"].ToString(), FechaCorte, true);

                            if (Saldo.Vencida=="SI")
                            {
                                ListaCesionesBalor.Add(Saldo);
                            }
                        }
                    }
                }                

                DataTable CesionesBalor = ListaCesionesBalor.ToDataTable();
                CesionesBalor.TableName = "Cesiones";
                CesionesBalor.Columns.Add("Codigo", typeof(int));
                CesionesBalor.Columns.Add("Cliente", typeof(string));

                Entity.Analisis.Catcliente cliente;
                foreach (DataRow r in CesionesBalor.Rows)
                {
                    cliente = MobileBO.ControlAnalisis.TraerCatclientes(r["ClienteID"].ToString(), null, null);
                    r["Codigo"] = cliente.Codigo;
                    r["Cliente"] = cliente.Nombrecompleto;
                }
                ds.Tables.Add(CesionesBalor);

                DataSet empresa = MobileBO.ControlConfiguracion.TraerDsEmpresasFull(empresaid);
                empresa.Tables[0].TableName="empresa";
                ds.Tables.Add(empresa.Tables[0].Copy());

                DataTable Datos = new DataTable();
                Datos.Columns.Add("FechaCorte", typeof(DateTime));
                DataRow datosRow = Datos.NewRow();
                datosRow["FechaCorte"] = FechaCorte;
                Datos.Rows.Add(datosRow);
                Datos.TableName = "Datos";
                ds.Tables.Add(Datos);

                //ds.WriteXml("c:\\Reportes\\ReporteClientesProblema.xml", System.Data.XmlWriteMode.WriteSchema);
                
                base.NombreReporte = "rptClientesProblema";
                //if (Desglosado == "1")
                //    base.NombreReporte = "ReporteSaldoCesionesBalorDesglosado";
                base.FormatoReporte = 5;
                base.RutaReporte = "Contabilidad\\Reportes";
                base.DataSource = ds;
                base.DataSource.WriteXml("c:\\Reportes\\ReporteClientesProblema.xml", System.Data.XmlWriteMode.WriteSchema);

            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}