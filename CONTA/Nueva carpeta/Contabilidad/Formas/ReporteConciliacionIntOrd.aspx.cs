using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BalorFinanciera.Contabilidad.Formas
{
    public partial class ReporteConciliacionIntOrd : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
    }

    public class ControladorReporteConciliacionIntOrd : Base.Clases.BaseReportes
    {
        #region Inicializa Reporte
        public override void InicializaReporte(System.Collections.Specialized.NameValueCollection parametros)
        {
            //OBTENEMOS LOS PARAMETROS DEL REPORTE
            string empresaid = parametros.Get("empresaid");
            DateTime fechaInicio = DateTime.Parse(parametros.Get("fecha"));
            DateTime fechaFinal = DateTime.Parse(parametros.Get("fecha2"));
            string clienteini = parametros.Get("clienteini");
            string clientefin = parametros.Get("clientefin");
            int formato = int.Parse(parametros.Get("Formato").Split(',')[1]);

            int? paramClienteini = clienteini != string.Empty ? int.Parse(clienteini) : int.MinValue;
            int? paramClientefin = clientefin != string.Empty ? int.Parse(clientefin) : int.MinValue;

            try
            {
                DataSet dsempr = MobileBO.ControlConfiguracion.TraerDsEmpresasFull(empresaid);
                dsempr.Tables[0].TableName = "DatosEmpresa";

                DataSet ds = MobileBO.ControlOperacion.TraerDatosConciliacionIntOrd(empresaid, fechaInicio, fechaFinal, paramClienteini, paramClientefin);
                ds.Tables[0].TableName= "DatosReporte";
                ds.Tables[0].Columns.Add("Financiamiento", typeof(decimal));

                Entity.ModeloSaldoCesion saldo;
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    string cesionid = row["Cesionid"].ToString().ToLower();

                    if (decimal.Parse(row["Diferencia"].ToString()) <= 10) //Pagada
                    {
                        if (DateTime.Parse(row["UltimoPago"].ToString()) <= fechaFinal)
                        {
                            if (DateTime.Parse(row["FechaVende"].ToString()) < DateTime.Parse(row["UltimoPago"].ToString()))
                            {
                                saldo = MobileBO.ControlOperacion.CalculaInteresOptimizado(row["Cesionid"].ToString(), DateTime.Parse(row["FechaVende"].ToString()), true);
                                row["IntOrd"] = saldo.InteresOrdinario;
                                row["ComiAnalisis"] = saldo.ComisionAnalisis;
                                row["ComiDisposicion"] = saldo.ComisionDisposicion;
                            }
                        }
                        //saldo = MobileBO.ControlOperacion.CalculaInteresOptimizado(row["Cesionid"].ToString(), DateTime.Parse(row["FechaVende"].ToString()), true);
                    }
                    //else //Sin Pagar
                    //{
                    //    //DateTime fecha;
                    //    //if (DateTime.Parse(row["UltimoPago"].ToString()) >= DateTime.Parse(row["FechaVende"].ToString()))
                    //    //    fecha = DateTime.Parse(row["FechaVende"].ToString());
                    //    //else
                    //    //    fecha = DateTime.Parse(row["UltimoPago"].ToString());

                    //    //saldo = MobileBO.ControlOperacion.CalculaInteresOptimizado(row["Cesionid"].ToString(), fecha, true);
                    //}
                }


                ds.Tables.Add(dsempr.Tables[0].Copy());
                DataTable tDatos = new DataTable();
                tDatos.Columns.Add("FechaInicial", typeof(DateTime));
                tDatos.Columns.Add("FechaFinal", typeof(DateTime));
                tDatos.Rows.Add(tDatos.NewRow());
                tDatos.Rows[0]["FechaInicial"] = fechaInicio;
                tDatos.Rows[0]["FechaFinal"] = fechaFinal;
                tDatos.TableName = "Parametros";
                ds.Tables.Add(tDatos);

                base.NombreReporte = "ConciliacionIntOrd";
                base.FormatoReporte = formato;
                base.RutaReporte = "Contabilidad\\Reportes";
                base.DataSource = ds;
                base.DataSource.WriteXml("c:\\Reportes\\ConciliacionIntOrd.xml", System.Data.XmlWriteMode.WriteSchema);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion Inicializa Reporte
    }
}