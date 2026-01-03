using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Web.Services;
using Homex.Core.Utilities;
using Entity;

namespace BalorFinanciera.Contabilidad.Formas
{
    public partial class InteresMoratorioFactur : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
    }
    public class ControladorReporteInteresMoratorioFactur : Base.Clases.BaseReportes
    {
        #region Inicializa Reporte
        public override void InicializaReporte(System.Collections.Specialized.NameValueCollection parametros)
        {
            //OBTENEMOS LOS PARAMETROS DEL REPORTE            
            string fecha = parametros.Get("fecha");
            try
            {
                DataSet ds = MobileBO.ControlOperacion.TraerCesionesProveedorVencidas(DateTime.Parse(fecha), 1);
                List<DatosClientesDemandado> ListaClientes = new List<DatosClientesDemandado>();
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    ModeloSaldoCesion Saldo = MobileBO.ControlOperacion.CalculaInteresOptimizado(row["CesionID"].ToString(), DateTime.Parse(fecha), true);
                    if (Saldo.Vencida == "SI")
                    {
                        Entity.Analisis.Catcliente cliente = MobileBO.ControlAnalisis.TraerCatclientes(Saldo.ClienteID, null, null);
                        DatosClientesDemandado item = new DatosClientesDemandado();
                        item.Cliente = cliente.Codigo;
                        item.Nombre = cliente.Nombrecompleto;
                        item.Cesion = Saldo.Cesion;
                        item.TipoContrato = Saldo.TipoContrato;
                        item.Fecha_Docu = DateTime.Parse(Saldo.FechaDocu);
                        item.Fecha_Vence = DateTime.Parse(Saldo.FechaVence);
                        item.Capital = Saldo.SaldoFinanciado;
                        item.InteresOrdinario = Saldo.InteresOrdinario - Saldo.PagoInteresOrdinario;
                        item.InteresMoratorio = Saldo.InteresMoratorio - Saldo.PagoInteresMoratorio;
                        ListaClientes.Add(item);
                    }
                }
                ds.Tables.Add(ListaClientes.ToDataTable());
                base.NombreReporte = "ReporteDineroColocado";
                base.FormatoReporte = int.Parse(parametros.Get("Formato").Split(',')[1]);
                base.RutaReporte = "Operacion\\Reportes";
                base.DataSource = ds;
                base.DataSource.WriteXml("c:\\Reportes\\ReporteDineroColocado.xml", System.Data.XmlWriteMode.WriteSchema);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion Inicializa Reporte
    }
    public class DatosClientesDemandado
    {
        public int Cliente { get; set; }
        public string Nombre { get; set; }
        public string Cesion { get; set; }
        public int TipoContrato { get; set; }
        public DateTime Fecha_Docu { get; set; }
        public DateTime Fecha_Vence { get; set; }
        public decimal Capital { get; set; }
        public decimal InteresOrdinario { get; set; }
        public decimal InteresMoratorio { get; set; }
        public decimal Iva { get; set; }
    }
}